using System;
using System.Collections.Generic;
using KZLib.Utilities;
using MessagePipe;

public class BadgeTag : CustomTag
{
	public BadgeTag Parent { get; }
	public UnlockTag Dependency { get; }

	public static readonly BadgeTag NONE = new(nameof(NONE));

	protected BadgeTag(string name,BadgeTag parent = null, UnlockTag dependency = null) : base(name)
	{
		Parent = parent;
		Dependency = dependency ?? UnlockTag.NONE;
	}
}

public class UnlockTag : CustomTag
{
	public static readonly UnlockTag NONE = new(nameof(NONE));

	protected UnlockTag(string name) : base(name) { }
}

namespace KZLib
{
	public interface IContext
	{
		bool IsUnlocked(UnlockTag tag);
		int GetBadgeCount(BadgeTag tag);

		CustomTag[] TagArray { get; }
	}

	/// <summary>
	/// Context = GameContent
	/// </summary>
	public class ContextManager : Singleton<ContextManager>
	{
		private readonly Dictionary<UnlockTag,List<IContext>> m_unlockListDict = new();
		private readonly Dictionary<BadgeTag,List<IContext>> m_badgeListDict = new();

		private readonly Dictionary<UnlockTag,List<BadgeTag>> m_dependencyBadgeListDict = new();

		private readonly Dictionary<BadgeTag,int> m_badgeCacheDict = new();
		private readonly Dictionary<UnlockTag,bool> m_unlockCacheDict = new();

		public void InitializeContext()
		{
			m_unlockListDict.Clear();
			m_badgeListDict.Clear();
			m_dependencyBadgeListDict.Clear();

			foreach(var type in KZReflectionKit.FindDerivedTypeGroup(typeof(IContext)))
			{
				if(Activator.CreateInstance(type) is not IContext context)
				{
					continue;
				}

				if(context.TagArray == null)
				{
					continue;
				}

				foreach(var tag in context.TagArray)
				{
					if(tag is UnlockTag unlockTag)
					{
						_AddMapping(m_unlockListDict,unlockTag,context);
					}
					else if(tag is BadgeTag badgeTag)
					{
						_AddMapping(m_badgeListDict,badgeTag,context);

						var dependency = badgeTag.Dependency;

						if(dependency != UnlockTag.NONE)
						{
							m_dependencyBadgeListDict.AddOrCreate(dependency,badgeTag);
						}
					}
				}
			}
		}

		private void _AddMapping<TTag>(Dictionary<TTag,List<IContext>> tagListDict,TTag tag,IContext context)
		{
			tagListDict.AddOrCreate(tag,context);
		}

		protected override void _Release(bool disposing)
		{
			if(disposing)
			{
				m_unlockListDict.Clear();
				m_badgeListDict.Clear();
			}

			base._Release(disposing);
		}

		public bool IsUnlocked(UnlockTag unlockTag)
		{
			if(unlockTag == UnlockTag.NONE)
			{
				return true;
			}

			if(m_unlockListDict.TryGetValue(unlockTag,out var unlockList))
			{
				foreach(var unlock in unlockList)
				{
					if(unlock.IsUnlocked(unlockTag))
					{
						return true;
					}
				}
			}

			return false;
		}

		public int GetBadgeCount(BadgeTag badgeTag)
		{
			if(badgeTag == BadgeTag.NONE || !IsUnlocked(badgeTag.Dependency))
			{
				return 0;
			}

			var count = 0;

			if(m_badgeListDict.TryGetValue(badgeTag,out var badgeList))
			{
				foreach(var badge in badgeList)
				{
					count += badge.GetBadgeCount(badgeTag);
				}
			}

			return count;
		}

		public void NotifyBadgeChanged(BadgeTag badgeTag)
		{
			if(badgeTag == BadgeTag.NONE)
			{
				return;
			}

			var current = GetBadgeCount(badgeTag);

			if(!m_badgeCacheDict.TryGetValue(badgeTag,out var last) || last != current)
			{
				m_badgeCacheDict[badgeTag] = current;

				GlobalMessagePipe.GetPublisher<BadgeTag,bool>().Publish(badgeTag,current > 0);
				GlobalMessagePipe.GetPublisher<BadgeTag,int>().Publish(badgeTag,current);

				if(badgeTag.Parent != null)
				{
					NotifyBadgeChanged(badgeTag.Parent);
				}
			}
		}

		public void NotifyUnlockChanged(UnlockTag unlockTag)
		{
			var current = IsUnlocked(unlockTag);

			if(!m_unlockCacheDict.TryGetValue(unlockTag,out var last) || last != current)
			{
				m_unlockCacheDict[unlockTag] = current;

				GlobalMessagePipe.GetPublisher<UnlockTag,bool>().Publish(unlockTag,current);
				
				if(m_dependencyBadgeListDict.TryGetValue(unlockTag,out var badgeList))
				{
					foreach(var badge in badgeList)
					{
						NotifyBadgeChanged(badge);
					}
				}
			}
		}
	}
}