// using UnityEngine;
// using KZLib;
// using Sirenix.OdinInspector;

// public abstract class RenderCam : BaseCamera
// {
// 	[SerializeField,LabelText("메인 쉐이더")]
// 	private Shader m_RenderShader = null;

// 	protected Material m_RenderMaterial = null;

// 	protected override void Awake()
// 	{
// 		base.Awake();

// 		if(CameraMgr.HasInstance)
// 		{
// 			CameraMgr.In.AddSubCamera(this);
// 		}

// 		m_RenderMaterial = new Material(m_RenderShader)
// 		{
// 			hideFlags = HideFlags.DontSave,
// 		};
// 	}

// 	protected override void OnDestroy()
// 	{
// 		base.Awake();

// 		if(CameraMgr.HasInstance)
// 		{
// 			CameraMgr.In.RemoveSubCamera(this);
// 		}
// 	}

// 	protected virtual void OnRenderImage(RenderTexture _source,RenderTexture _destination)
// 	{
// 		Graphics.Blit(_source,_destination,m_RenderMaterial);
// 	}
// }