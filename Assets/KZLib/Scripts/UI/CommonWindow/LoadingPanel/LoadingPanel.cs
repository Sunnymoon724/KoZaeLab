using System.Collections.Generic;
using KZLib.UI.Widgets;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KZLib.UI
{
	/// <summary>
	/// Full-screen loading panel that forwards normalized progress to child <see cref="LoadingProgressWidget"/> instances.
	/// Scene changes update progress via <see cref="SetLoadingProgress"/> from <c>SceneStateManager</c>;
	/// <see cref="KZLib.UIManager.PlayLoadingAsync"/> passes an <see cref="System.Action{float}"/> callback to the work task.
	/// </summary>
	public class LoadingPanel : BasePanel
	{
		[SerializeField,ValidateInput(nameof(_HasAnyWidget),"At least one loading widget is required.",InfoMessageType.Warning)]
		private List<LoadingProgressWidget> m_loadingWidgetList = new();

		private bool _HasAnyWidget()
		{
			if(m_loadingWidgetList.IsNullOrEmpty())
			{
				return false;
			}

			for(var i=0;i<m_loadingWidgetList.Count;i++)
			{
				if(m_loadingWidgetList[i])
				{
					return true;
				}
			}

			return false;
		}

		public override void Open(object param)
		{
			SetLoadingProgress(0.0f);

			base.Open(param);

			if(!_HasAnyWidget())
			{
				LogChannel.UI.W($"{NameTag} has no loading widgets assigned.");
			}
		}

		/// <summary>Updates all assigned widgets. <paramref name="progress"/> is clamped to [0, 1].</summary>
		public void SetLoadingProgress(float progress)
		{
			progress = Mathf.Clamp01(progress);

			if(!_HasAnyWidget())
			{
				return;
			}

			for(var i=0;i<m_loadingWidgetList.Count;i++)
			{
				var widget = m_loadingWidgetList[i];

				if(widget)
				{
					widget.SetLoadingProgress(progress);
				}
			}
		}
	}
}