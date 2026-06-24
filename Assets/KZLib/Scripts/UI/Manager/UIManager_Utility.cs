using KZLib.Utilities;

namespace KZLib
{
	public partial class UIManager : SingletonMB<UIManager>
	{
		// Shared system UI loaded from a fixed path and marked dont-release on creation.
		private static readonly CommonUINameTag[] s_definedTagArray = new CommonUINameTag[] { CommonUINameTag.TransitionPanel, CommonUINameTag.DebugOverlayPanel, CommonUINameTag.VideoPanel  };

		/// <summary>
		/// Returns the prefab resource path for the given window.
		/// Defined windows use the common UI folder; others use <see cref="m_prefabPath"/> from game config.
		/// </summary>
		private string _GetPrefabPath(CommonUINameTag nameTag)
		{
			return $"{(_IsDefinedWindow(nameTag) ? "Resources/Prefab/UI" : m_prefabPath)}/{nameTag}.prefab";
		}

		/// <summary>
		/// Returns whether the window is a shared system UI shipped with KZLib.
		/// </summary>
		private bool _IsDefinedWindow(CommonUINameTag nameTag)
		{
			foreach(var tag in s_definedTagArray)
			{
				if(tag.Equals(nameTag))
				{
					return true;
				}
			}

			return false;
		}
	}
}
