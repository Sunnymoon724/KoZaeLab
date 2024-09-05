using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HudPanel
{
	public class DetailTabViewUI : TabViewUI
	{
		[SerializeField,LabelText("창 딕셔너리")]
		private Dictionary<TabButtonUI,GameObject> m_WindowDict = null;

		private TabButtonUI m_Selected = null;

		protected override void Initialize()
		{
			base.Initialize();

			foreach(var pair in m_WindowDict)
			{
				pair.Value.SetActiveSelf(false);
			}
		}

		public override void Notify(TabButtonUI _button)
		{
			m_Selected = m_Selected == _button ? null : _button;

			foreach(var pair in m_WindowDict)
			{
				pair.Value.SetActiveSelf(pair.Key.Equals(m_Selected));
			}
		}
	}
}