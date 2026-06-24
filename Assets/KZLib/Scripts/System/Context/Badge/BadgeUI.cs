using System.Collections;
using System.Collections.Generic;
using KZLib;
using KZLib.Utilities;
using MessagePipe;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>Shows or hides badge objects when the bound <see cref="BadgeTag"/> count is greater than zero.</summary>
/// <remarks>Assign the tag via inspector dropdown. Game project must declare <see cref="BadgeTag"/> static instances.</remarks>
public class BadgeUI : MonoBehaviour
{
	[SerializeField]
	private List<GameObject> m_badgeList = new();

	[SerializeField,ValueDropdown(nameof(BadgeTagGroup))]
	private string m_badgeName = null;

	private List<BadgeTag> m_badgeTagList = null;
	private IEnumerable BadgeTagGroup => m_badgeTagList ??= CustomTag.CollectCustomTagList<BadgeTag>();

	private BadgeTag _ResolveBadgeTag()
	{
		if(m_badgeName.IsEmpty() || !m_badgeName.TryToCustomTag(out BadgeTag badgeTag))
		{
			return BadgeTag.NONE;
		}

		return badgeTag;
	}

	private void Awake()
	{
		var badgeTag = _ResolveBadgeTag();

		if(badgeTag == BadgeTag.NONE)
		{
			return;
		}

		_RefreshBadge(ContextManager.In.GetBadgeCount(badgeTag));

		GlobalMessagePipe.GetSubscriber<BadgeTag,int>().Subscribe(badgeTag,_RefreshBadge).RegisterTo(destroyCancellationToken);
	}

	private void OnEnable()
	{
		var badgeTag = _ResolveBadgeTag();

		if(badgeTag == BadgeTag.NONE)
		{
			return;
		}

		_RefreshBadge(ContextManager.In.GetBadgeCount(badgeTag));
	}

	private void _RefreshBadge(int currentCount)
	{
		var hasBadge = currentCount > 0;

		for(int i=0;i<m_badgeList.Count;i++)
		{
			var badge = m_badgeList[i];

			if(badge)
			{
				badge.SetActive(hasBadge);
			}
		}
	}
}
