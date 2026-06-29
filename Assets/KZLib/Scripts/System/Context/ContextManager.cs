using System;
using System.Collections.Generic;
using KZLib.Utilities;
using MessagePipe;

/// <summary>Badge (red-dot) identifier. Declare static instances in the game project.</summary>
/// <remarks>
/// <see cref="Parent"/> is used only to bubble <see cref="ContextManager.NotifyBadgeChanged"/> up the menu tree.
/// Parent count is not auto-summed from children — register a provider on the parent tag when needed.
/// <see cref="Dependency"/> gates <see cref="ContextManager.GetBadgeCount"/> to zero while locked.
/// </remarks>
public class BadgeTag : CustomTag
{
	public BadgeTag Parent { get; }
	public UnlockTag Dependency { get; }

	public static readonly BadgeTag NONE = new(nameof(NONE));

	public BadgeTag(string name,BadgeTag parent = null, UnlockTag dependency = null) : base(name)
	{
		Parent = parent;
		Dependency = dependency ?? UnlockTag.NONE;
	}
}

/// <summary>Content-unlock identifier. Declare static instances in the game project.</summary>
public class UnlockTag : CustomTag
{
	public static readonly UnlockTag NONE = new(nameof(NONE));

	public UnlockTag(string name) : base(name) { }
}

namespace KZLib
{
	/// <summary>
	/// Game-content provider for unlock state and badge counts.
	/// Implement in the game project; <see cref="ContextManager"/> discovers concrete types via reflection.
	/// </summary>
	/// <remarks>
	/// List every served tag in <see cref="TagArray"/>. Call <see cref="ContextManager.NotifyUnlockChanged"/> /
	/// <see cref="ContextManager.NotifyBadgeChanged"/> when underlying data changes.
	/// </remarks>
	public interface IContext
	{
		bool IsUnlocked(UnlockTag tag);
		int GetBadgeCount(BadgeTag tag);

		CustomTag[] TagArray { get; }
	}

	/// <summary>
	/// Aggregates unlock/badge state from all <see cref="IContext"/> providers (Context = game content).
	/// </summary>
	/// <remarks>
	/// <see cref="IsUnlocked"/> — OR across providers (any true ⇒ unlocked).
	/// <see cref="GetBadgeCount"/> — SUM across providers.
	/// One instance per concrete <see cref="IContext"/> is created at <see cref="InitializeContext"/>.
	/// </remarks>
	public class ContextManager : Singleton<ContextManager>
	{
		private ContextManager() { }

		private readonly Dictionary<UnlockTag,List<IContext>> m_unlockListDict = new();
		private readonly Dictionary<BadgeTag,List<IContext>> m_badgeListDict = new();

		private readonly Dictionary<UnlockTag,List<BadgeTag>> m_dependencyBadgeListDict = new();

		private readonly Dictionary<BadgeTag,int> m_badgeCacheDict = new();
		private readonly Dictionary<UnlockTag,bool> m_unlockCacheDict = new();

		/// <summary>Scans loaded assemblies for <see cref="IContext"/> types and rebuilds tag mappings.</summary>
		public void InitializeContext()
		{
			m_unlockListDict.Clear();
			m_badgeListDict.Clear();
			m_dependencyBadgeListDict.Clear();
			m_badgeCacheDict.Clear();
			m_unlockCacheDict.Clear();

			foreach(var type in KZReflectionKit.FindDerivedTypeGroup(typeof(IContext),instantiableOnly: true))
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
					if(tag == null)
					{
						continue;
					}

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
							_AddDependencyBadge(dependency,badgeTag);
						}
					}
				}
			}
		}

		private void _AddMapping<TTag>(Dictionary<TTag,List<IContext>> tagListDict,TTag tag,IContext context)
		{
			if(!tagListDict.TryGetValue(tag,out var contextList))
			{
				contextList = new List<IContext>();
				tagListDict.Add(tag,contextList);
			}

			if(!contextList.Contains(context))
			{
				contextList.Add(context);
			}
		}

		private void _AddDependencyBadge(UnlockTag dependency,BadgeTag badgeTag)
		{
			if(!m_dependencyBadgeListDict.TryGetValue(dependency,out var badgeList))
			{
				badgeList = new List<BadgeTag>();
				m_dependencyBadgeListDict.Add(dependency,badgeList);
			}

			if(!badgeList.Contains(badgeTag))
			{
				badgeList.Add(badgeTag);
			}
		}

		protected override void _Release(bool disposing)
		{
			if(disposing)
			{
				m_unlockListDict.Clear();
				m_badgeListDict.Clear();
				m_dependencyBadgeListDict.Clear();
				m_badgeCacheDict.Clear();
				m_unlockCacheDict.Clear();
			}

			base._Release(disposing);
		}

		/// <summary>Returns true when any registered provider reports unlocked.</summary>
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

		/// <summary>Returns the summed badge count from all providers. Zero when locked by <see cref="BadgeTag.Dependency"/>.</summary>
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

		/// <summary>Publishes when the badge count changes; bubbles to <see cref="BadgeTag.Parent"/>.</summary>
		public void NotifyBadgeChanged(BadgeTag badgeTag)
		{
			_NotifyBadgeChanged(badgeTag,null);
		}

		private void _NotifyBadgeChanged(BadgeTag badgeTag,HashSet<BadgeTag> visited)
		{
			if(badgeTag == BadgeTag.NONE)
			{
				return;
			}

			var current = GetBadgeCount(badgeTag);

			if(!m_badgeCacheDict.TryGetValue(badgeTag,out var last) || last != current)
			{
				m_badgeCacheDict[badgeTag] = current;

				GlobalMessagePipe.GetPublisher<BadgeTag,int>().Publish(badgeTag,current);

				var parent = badgeTag.Parent;

				if(parent != null)
				{
					visited ??= new HashSet<BadgeTag>();

					if(visited.Add(parent))
					{
						_NotifyBadgeChanged(parent,visited);
					}
				}
			}
		}

		/// <summary>Publishes when unlock state changes; refreshes badges that depend on this tag.</summary>
		public void NotifyUnlockChanged(UnlockTag unlockTag)
		{
			if(unlockTag == UnlockTag.NONE)
			{
				return;
			}

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
