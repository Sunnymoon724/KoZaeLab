using System;
using System.Collections.Generic;
using KZLib.KZUtility;
using MessagePipe;

public class RedDotTag : CustomTag
{
	private readonly RedDotTag m_parent;

	public RedDotTag Parent => m_parent;

	public static readonly RedDotTag NONE = new(nameof(NONE));

	protected RedDotTag(string name) : this(name,null) { }

	protected RedDotTag(string name,RedDotTag parent) : base(name)
	{
		m_parent = parent;
	}
}

namespace KZLib
{
	public interface IRedDotContext : IContext
	{
		bool HasRedDot(RedDotTag redDotTag);
	}

	public abstract class RedDotContext : IRedDotContext
	{
		private record RedDotInfo
		{
			private int m_selfCount;
			private int m_childrenCount;

			public int SelfCount => m_selfCount;
			public int RedDotCount => m_selfCount + m_childrenCount;

			public RedDotTag CurrentRedDotTag { get; init; }
			public RedDotTag ParentRedDotTag { get; init; }

			public RedDotInfo(RedDotTag current,RedDotTag parent)
			{
				CurrentRedDotTag = current;
				ParentRedDotTag = parent;

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

		private readonly Dictionary<RedDotTag,RedDotInfo> m_redDotInfoDict = new();
		protected readonly Dictionary<RedDotTag,Func<int>> m_redDotCheckerDict = new();

		private readonly Dictionary<RedDotTag,HashSet<string>> m_newContentsHashSetDict = new();

		public RedDotContext()
		{
			var redDotTagList = CustomTag.CollectCustomTagList<RedDotTag>(true);

			for(var i=0;i<redDotTagList.Count;i++)
			{
				var redDotTag = redDotTagList[i];

				if(!_IsValid(redDotTag))
				{
					continue;
				}

				m_redDotInfoDict.Add(redDotTag,new RedDotInfo(redDotTag,redDotTag?.Parent));
			}

			_Initialize();

			CheckRedDotAll();
		}

		//! must be create checker
		protected abstract bool _Initialize();

		public void CheckRedDotAll()
		{
			foreach(var pair in m_redDotInfoDict)
			{
				CheckRedDot(pair.Key);
			}
		}

		public void CheckRedDot(RedDotTag redDotTag)
		{
			if(m_redDotCheckerDict.TryGetValue(redDotTag,out var calculateFunc))
			{
				ClearRedDotCount(redDotTag);

				var count = calculateFunc();

				if(count > 0)
				{
					_IncreaseRedDotCount(redDotTag,count);
				}
			}

			_RecursiveRefreshRedDot(redDotTag);
		}

		public void ClearRedDotCount(RedDotTag redDotTag)
		{
			var redDotInfo = _GetRedDotInfo(redDotTag);

			if(redDotInfo == null)
			{
				return;
			}

			var selfCount = redDotInfo.SelfCount;

			if(selfCount <= 0)
			{
				return;
			}

			redDotInfo.ClearSelfCount();

			_RecursiveDecreaseParentRedDotCount(redDotInfo.ParentRedDotTag,selfCount);
			_RecursiveRefreshRedDot(redDotTag);
		}

		private void _RecursiveDecreaseParentRedDotCount(RedDotTag redDotTag,int count)
		{
			var redDotInfo = _GetRedDotInfo(redDotTag);

			if(redDotInfo == null)
			{
				return;
			}

			redDotInfo.AddChildrenCount(-count);

			_RecursiveDecreaseParentRedDotCount(redDotInfo.ParentRedDotTag,count);
		}

		private void _IncreaseRedDotCount(RedDotTag redDotTag,int count = 1)
		{
			var redDotInfo = _GetRedDotInfo(redDotTag);

			if(redDotInfo == null)
			{
				return;
			}

			redDotInfo.AddSelfCount(count);

			RecursiveIncreaseParentRedDotCount(redDotInfo.ParentRedDotTag,count);
		}

		private void RecursiveIncreaseParentRedDotCount(RedDotTag redDotTag,int count)
		{
			var redDotInfo = _GetRedDotInfo(redDotTag);

			if(redDotInfo == null)
			{
				return;
			}

			redDotInfo.AddChildrenCount(count);

			RecursiveIncreaseParentRedDotCount(redDotInfo.ParentRedDotTag,count);
		}

		private void _RecursiveRefreshRedDot(RedDotTag redDotTag)
		{
			var hasRedDot = HasRedDot(redDotTag);

			if(!hasRedDot)
			{
				return;
			}

			GlobalMessagePipe.GetPublisher<RedDotTag,bool>().Publish(redDotTag,hasRedDot);

			_RecursiveRefreshRedDot(redDotTag?.Parent);
		}

		public void AddNewContents(RedDotTag redDotTag,string contentsId)
		{
			AddNewContents(redDotTag,new string[]{contentsId});
		}

		public void AddNewContents(RedDotTag redDotTag,IEnumerable<string> contentIdGroup )
		{
			if(contentIdGroup.IsNullOrEmpty())
			{
				return;
			}

			var newContentsHashSet = _GetOrCreateNewContentsHashSet(redDotTag,true);
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
				IncreaseRedDotCount(redDotTag,addedCount);
			}
		}

		public void IncreaseRedDotCount(RedDotTag redDotTag,int count = 1)
		{
			_IncreaseRedDotCount(redDotTag,count);
			_RecursiveRefreshRedDot(redDotTag);
		}

		public bool HasRedDot(RedDotTag redDotTag)
		{
			if(!_IsValid(redDotTag))
			{
				return false;
			}

			var redDotInfo = _GetRedDotInfo(redDotTag);

			return redDotInfo.RedDotCount > 0;
		}

		public bool HasNewContents(RedDotTag redDotTag)
		{
			return HasNewContents(redDotTag,string.Empty);
		}

		public bool HasNewContents(RedDotTag redDotTag,string contentId)
		{
			return HasNewContents(redDotTag, new string[] { contentId });
		}

		public bool HasNewContents(RedDotTag redDotTag,IEnumerable<string> contentIdGroup)
		{
			var newContentsHashSet = _GetNewContentsHashSet(redDotTag);

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

		public void ClearNewContents(RedDotTag redDotTag)
		{
			var newContentsHashSet = _GetNewContentsHashSet(redDotTag);

			if(newContentsHashSet.IsNullOrEmpty())
			{
				return;
			}

			newContentsHashSet.Clear();

			m_newContentsHashSetDict.Remove(redDotTag);

			ClearRedDotCount(redDotTag);
		}

		public bool RemoveNewContents(RedDotTag redDotTag,string contentID)
		{
			return RemoveNewContents(redDotTag,new string[] { contentID });
		}

		public bool RemoveNewContents(RedDotTag redDotTag,IEnumerable<string> contentIdGroup )
		{
			if(contentIdGroup.IsNullOrEmpty())
			{
				return false;
			}

			var newContentsHashSet = _GetOrCreateNewContentsHashSet(redDotTag,true);

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
				DecreaseRedDotCount(redDotTag,removedCount);
			}

			return removedCount > 0;
		}

		public void DecreaseRedDotCount(RedDotTag redDotTag,int count = 1)
		{
			var redDotInfo = _GetRedDotInfo(redDotTag);

			if(redDotInfo == null)
			{
				return;
			}
			
			var selfCount = redDotInfo.SelfCount;

			if(selfCount <= 0)
			{
				return;
			}
			
			redDotInfo.AddSelfCount(-count);

			_RecursiveDecreaseParentRedDotCount(redDotInfo.ParentRedDotTag,count);
			_RecursiveRefreshRedDot(redDotTag);
		}

		private RedDotInfo _GetRedDotInfo(RedDotTag redDotTag)
		{
			return m_redDotInfoDict.TryGetValue(redDotTag, out var redDotInfo) ? redDotInfo : null;
		}

		private HashSet<string> _GetNewContentsHashSet(RedDotTag redDotTag)
		{
			return _GetOrCreateNewContentsHashSet(redDotTag,false);
		}

		private HashSet<string> _GetOrCreateNewContentsHashSet(RedDotTag redDotTag,bool createIfNotExist)
		{
			if(!m_newContentsHashSetDict.TryGetValue(redDotTag,out var contentsList))
			{
				if(createIfNotExist)
				{
					contentsList = new HashSet<string>();

					m_newContentsHashSetDict.Add(redDotTag,contentsList);
				}
				else
				{
					return null;
				}
			}

			return contentsList;
		}

		private bool _IsValid(RedDotTag redDotTag)
		{
			return redDotTag != null && redDotTag != RedDotTag.NONE;
		}
	}
}