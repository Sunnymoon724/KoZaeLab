#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;

using Object = UnityEngine.Object;

namespace KZLib.KZWindow
{
	public abstract class ResourceWindow<TResource> : OdinEditorWindow where TResource : Object
	{
		protected TResource m_resource = null;

		public virtual void SetResource(TResource resource)
		{
			m_resource = resource;
		}

		protected override void OnImGUI()
		{
			if(!m_resource)
			{
				return;
			}

			_OnImGUI();
		}

		protected abstract void _OnImGUI();
	}
}

#endif