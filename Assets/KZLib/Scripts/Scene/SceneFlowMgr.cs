using KZLib.KZUtility;
using UnityEngine;

namespace KZLib
{
	public abstract class SceneFlowMgr<TFlow> : SingletonMB<TFlow> where TFlow : MonoBehaviour
	{
		public abstract void InitializeScene(SceneState.StateParam param);

		public abstract void ReleaseScene();
	}
}