using System.Collections.Generic;
using KZLib.UI.Widgets.Debug;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KZLib.UI.Widgets.Debug
{
	public interface IImmediateOverlay
	{
		bool IsActive { get; }
		void Refresh();
	}

	public interface IPeriodicOverlay
	{
		bool IsActive { get; }
		void Refresh(int frameRate,float frameTime);
	}

	public abstract class ImmediateOverlayBehaviour : MonoBehaviour,IImmediateOverlay
	{
		public virtual bool IsActive => gameObject.activeInHierarchy;

		public abstract void Refresh();
	}

	public abstract class PeriodicOverlayBehaviour : MonoBehaviour,IPeriodicOverlay
	{
		public virtual bool IsActive => gameObject.activeInHierarchy;

		public abstract void Refresh(int frameRate,float frameTime);
	}
}

namespace KZLib.UI
{
	public class DebugOverlayPanel : BasePanel
	{
		[SerializeField,ListDrawerSettings(ShowFoldout = false)]
		private List<ImmediateOverlayBehaviour> m_immediateList = new();

		[SerializeField,ListDrawerSettings(ShowFoldout = false)]
		private List<PeriodicOverlayBehaviour> m_periodicList = new();

		public void RefreshImmediate()
		{
			for(var i=0;i<m_immediateList.Count;i++)
			{
				var overlay = m_immediateList[i];

				if(overlay && overlay.IsActive)
				{
					overlay.Refresh();
				}
			}
		}

		public void RefreshPeriodic(int frameRate,float frameTime)
		{
			for(var i=0;i<m_periodicList.Count;i++)
			{
				var overlay = m_periodicList[i];

				if(overlay && overlay.IsActive)
				{
					overlay.Refresh(frameRate,frameTime);
				}
			}
		}
	}
}
