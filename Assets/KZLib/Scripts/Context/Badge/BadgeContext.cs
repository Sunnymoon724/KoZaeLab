using System;
using System.Collections.Generic;
using KZLib.Utilities;
using MessagePipe;

public class BadgeTag : CustomTag
{
	private readonly BadgeTag m_parent;

	public BadgeTag Parent => m_parent;

	public static readonly BadgeTag NONE = new(nameof(NONE));

	protected BadgeTag(string name) : this(name,null) { }

	protected BadgeTag(string name,BadgeTag parent) : base(name)
	{
		m_parent = parent;
	}
}

namespace KZLib
{
	public interface IBadgeContext : IContext
	{
		bool HasBadge(BadgeTag badgeTag);
	}

	public abstract class BadgeContext : IBadgeContext
	{
		private record BadgeInfo
		{
			private int m_selfCount;
			private int m_childrenCount;

			public int SelfCount => m_selfCount;
			public int BadgeCount => m_selfCount + m_childrenCount;

			public BadgeTag CurrentBadgeTag { get; init; }
			public BadgeTag ParentBadgeTag { get; init; }

			public BadgeInfo(BadgeTag current,BadgeTag parent)
			{
				CurrentBadgeTag = current;
				ParentBadgeTag = parent;

				m_selfCount = 0;
				m_childrenCount = 0;
			}

			public void AddSelfCount(int count)
			{
				m_selfCount += count;
			}

			public void AddChildrenCount(int count)
			{
				m_childrenCount += count;
			}

			public void ClearSelfCount()
			{
				m_selfCount = 0;
			}
		}

		private readonly Dictionary<BadgeTag,BadgeInfo> m_badgeInfoDict = new();
		protected readonly Dictionary<BadgeTag,Func<int>> m_badgeCheckerDict = new();

		private readonly Dictionary<BadgeTag,HashSet<string>> m_newContentsHashSetDict = new();

		public BadgeContext()
		{
			var badgeTagList = CustomTag.CollectCustomTagList<BadgeTag>(true);

			for(var i=0;i<badgeTagList.Count;i++)
			{
				var badgeTag = badgeTagList[i];

				if(!_IsValid(badgeTag))
				{
					continue;
				}

				m_badgeInfoDict.Add(badgeTag,new BadgeInfo(badgeTag,badgeTag?.Parent));
			}

			_Initialize();

			CheckBadgeAll();
		}

		//! must be create checker
		protected abstract bool _Initialize();

		public void CheckBadgeAll()
		{
			foreach(var pair in m_badgeInfoDict)
			{
				CheckBadge(pair.Key);
			}
		}

		public void CheckBadge(BadgeTag badgeTag)
		{
			if(m_badgeCheckerDict.TryGetValue(badgeTag,out var calculateFunc))
			{
				ClearBadgeCount(badgeTag);

				var count = calculateFunc();

				if(count > 0)
				{
					_IncreaseBadgeCount(badgeTag,count);
				}
			}

			_RecursiveRefreshBadge(badgeTag);
		}

		public void ClearBadgeCount(BadgeTag badgeTag)
		{
			var badgeInfo = _GetBadgeInfo(badgeTag);

			if(badgeInfo == null)
			{
				return;
			}

			var selfCount = badgeInfo.SelfCount;

			if(selfCount <= 0)
			{
				return;
			}

			badgeInfo.ClearSelfCount();

			_RecursiveDecreaseParentBadgeCount(badgeInfo.ParentBadgeTag,selfCount);
			_RecursiveRefreshBadge(badgeTag);
		}

		private void _RecursiveDecreaseParentBadgeCount(BadgeTag badgeTag,int count)
		{
			var badgeInfo = _GetBadgeInfo(badgeTag);

			if(badgeInfo == null)
			{
				return;
			}

			badgeInfo.AddChildrenCount(-count);

			_RecursiveDecreaseParentBadgeCount(badgeInfo.ParentBadgeTag,count);
		}

		private void _IncreaseBadgeCount(BadgeTag badgeTag,int count = 1)
		{
			var badgeInfo = _GetBadgeInfo(badgeTag);

			if(badgeInfo == null)
			{
				return;
			}

			badgeInfo.AddSelfCount(count);

			RecursiveIncreaseParentBadgeCount(badgeInfo.ParentBadgeTag,count);
		}

		private void RecursiveIncreaseParentBadgeCount(BadgeTag badgeTag,int count)
		{
			var badgeInfo = _GetBadgeInfo(badgeTag);

			if(badgeInfo == null)
			{
				return;
			}

			badgeInfo.AddChildrenCount(count);

			RecursiveIncreaseParentBadgeCount(badgeInfo.ParentBadgeTag,count);
		}

		private void _RecursiveRefreshBadge(BadgeTag badgeTag)
		{
			var hasBadge = HasBadge(badgeTag);

			if(!hasBadge)
			{
				return;
			}

			GlobalMessagePipe.GetPublisher<BadgeTag,bool>().Publish(badgeTag,hasBadge);

			_RecursiveRefreshBadge(badgeTag?.Parent);
		}

		public void AddNewContents(BadgeTag badgeTag,string contentsId)
		{
			AddNewContents(badgeTag,new string[]{contentsId});
		}

		public void AddNewContents(BadgeTag badgeTag,IEnumerable<string> contentIdGroup )
		{
			if(contentIdGroup.IsNullOrEmpty())
			{
				return;
			}

			var newContentsHashSet = _GetOrCreateNewContentsHashSet(badgeTag,true);
			var addedCount = 0;

			foreach(var contentId in contentIdGroup)
			{
				if(!newContentsHashSet.Contains(contentId))
				{
					addedCount++;
				}
			}

			if(addedCount > 0)
			{
				IncreaseBadgeCount(badgeTag,addedCount);
			}
		}

		public void IncreaseBadgeCount(BadgeTag badgeTag,int count = 1)
		{
			_IncreaseBadgeCount(badgeTag,count);
			_RecursiveRefreshBadge(badgeTag);
		}

		public bool HasBadge(BadgeTag badgeTag)
		{
			if(!_IsValid(badgeTag))
			{
				return false;
			}

			var badgeInfo = _GetBadgeInfo(badgeTag);

			return badgeInfo.BadgeCount > 0;
		}

		public bool HasNewContents(BadgeTag badgeTag)
		{
			return HasNewContents(badgeTag,string.Empty);
		}

		public bool HasNewContents(BadgeTag badgeTag,string contentId)
		{
			return HasNewContents(badgeTag, new string[] { contentId });
		}

		public bool HasNewContents(BadgeTag badgeTag,IEnumerable<string> contentIdGroup)
		{
			var newContentsHashSet = _GetNewContentsHashSet(badgeTag);

			if(newContentsHashSet.IsNullOrEmpty())
			{
				return false;
			}

			foreach(var contentId in contentIdGroup)
			{
				if(newContentsHashSet.Contains(contentId))
				{
					return true;
				}
			}

			return false;
		}

		public void ClearNewContents(BadgeTag badgeTag)
		{
			var newContentsHashSet = _GetNewContentsHashSet(badgeTag);

			if(newContentsHashSet.IsNullOrEmpty())
			{
				return;
			}

			newContentsHashSet.Clear();

			m_newContentsHashSetDict.Remove(badgeTag);

			ClearBadgeCount(badgeTag);
		}

		public bool RemoveNewContents(BadgeTag badgeTag,string contentID)
		{
			return RemoveNewContents(badgeTag,new string[] { contentID });
		}

		public bool RemoveNewContents(BadgeTag badgeTag,IEnumerable<string> contentIdGroup )
		{
			if(contentIdGroup.IsNullOrEmpty())
			{
				return false;
			}

			var newContentsHashSet = _GetOrCreateNewContentsHashSet(badgeTag,true);

			if(newContentsHashSet.IsNullOrEmpty())
			{
				return false;
			}

			int removedCount = 0;

			foreach(var contentId in contentIdGroup)
			{
				if(newContentsHashSet.Remove(contentId))
				{
					removedCount++;
				}
			}

			if( removedCount > 0 )
			{
				DecreaseBadgeCount(badgeTag,removedCount);
			}

			return removedCount > 0;
		}

		public void DecreaseBadgeCount(BadgeTag badgeTag,int count = 1)
		{
			var badgeInfo = _GetBadgeInfo(badgeTag);

			if(badgeInfo == null)
			{
				return;
			}
			
			var selfCount = badgeInfo.SelfCount;

			if(selfCount <= 0)
			{
				return;
			}
			
			badgeInfo.AddSelfCount(-count);

			_RecursiveDecreaseParentBadgeCount(badgeInfo.ParentBadgeTag,count);
			_RecursiveRefreshBadge(badgeTag);
		}

		private BadgeInfo _GetBadgeInfo(BadgeTag badgeTag)
		{
			return m_badgeInfoDict.TryGetValue(badgeTag, out var badgeInfo) ? badgeInfo : null;
		}

		private HashSet<string> _GetNewContentsHashSet(BadgeTag badgeTag)
		{
			return _GetOrCreateNewContentsHashSet(badgeTag,false);
		}

		private HashSet<string> _GetOrCreateNewContentsHashSet(BadgeTag badgeTag,bool createIfNotExist)
		{
			if(!m_newContentsHashSetDict.TryGetValue(badgeTag,out var contentsList))
			{
				if(createIfNotExist)
				{
					contentsList = new HashSet<string>();

					m_newContentsHashSetDict.Add(badgeTag,contentsList);
				}
				else
				{
					return null;
				}
			}

			return contentsList;
		}

		private bool _IsValid(BadgeTag badgeTag)
		{
			return badgeTag != null && badgeTag != BadgeTag.NONE;
		}
	}
}