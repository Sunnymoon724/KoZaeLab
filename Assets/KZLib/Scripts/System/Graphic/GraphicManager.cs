using KZLib.Data;
using KZLib.Utilities;
using R3;
using UnityEngine;

namespace KZLib
{
	/// <summary>
	/// Applies <see cref="GraphicTune"/> to Unity (Screen, frame rate, QualitySettings, camera far clip).
	/// Same role as <see cref="Sounds.SoundManager"/> for <see cref="SoundTune"/>.
	/// URP Asset is not handled here; use a separate pipeline/preset path when needed.
	/// </summary>
	public class GraphicManager : Singleton<GraphicManager>
	{
		private readonly CompositeDisposable m_disposable = new();

		private GraphicTune m_graphicTune = null;

		private GraphicManager() { }

		protected override void _Initialize()
		{
			base._Initialize();

			m_graphicTune = TuneManager.In.Fetch<GraphicTune>();

			// OnChangedWithStart on the tune side: first subscription applies saved settings (boot restore).
			m_graphicTune.OnChangedResolution.Subscribe(_OnChangeResolution).AddTo(m_disposable);
			m_graphicTune.OnChangedFrameRate.Subscribe(_OnChangeFrameRate).AddTo(m_disposable);
			m_graphicTune.OnChangedGraphicQuality.Subscribe(_OnChangeGraphicQuality).AddTo(m_disposable);
		}

		protected override void _Release(bool disposing)
		{
			if(disposing)
			{
				m_disposable.Dispose();
				m_graphicTune = null;
			}

			base._Release(disposing);
		}

		/// <summary>
		/// Called from <see cref="CameraManager.AttachCamera"/> when the main camera was not ready at last quality apply.
		/// </summary>
		public void ApplyCameraFarClip()
		{
			if(m_graphicTune == null)
			{
				return;
			}

			if(!GraphicQualityOption.In.TryFindOptionValue<float>(m_graphicTune.CurrentGraphicQuality,Global.DisableCameraFarHalf,out var factor))
			{
				return;
			}

			if(!CameraManager.HasInstance)
			{
				return;
			}

			CameraManager.In.ApplyFarClipScale(factor);
		}

		private void _OnChangeResolution(ScreenResolution _)
		{
			_ApplyResolution();
		}

		private void _OnChangeFrameRate(int _)
		{
			_ApplyFrameRate();
		}

		private void _OnChangeGraphicQuality(long _)
		{
			_ApplyGraphicQuality();
		}

		private void _ApplyResolution()
		{
			var resolution = m_graphicTune.CurrentResolution;

			Screen.SetResolution(resolution.width,resolution.height,resolution.fullscreen);
		}

		private void _ApplyFrameRate()
		{
			Application.targetFrameRate = m_graphicTune.CurrentFrameRate;
		}

		/// <summary>Maps <see cref="GraphicTune.CurrentGraphicQuality"/> through <see cref="GraphicQualityOption"/> into QualitySettings.</summary>
		private void _ApplyGraphicQuality()
		{
			var graphicQuality = m_graphicTune.CurrentGraphicQuality;

			QualitySettings.globalTextureMipmapLimit = GraphicQualityOption.In.FindOptionValue<int>(graphicQuality,Global.GlobalTextureMipmapLimit);
			QualitySettings.anisotropicFiltering = GraphicQualityOption.In.FindOptionValue<AnisotropicFiltering>(graphicQuality,Global.AnisotropicFiltering);
			QualitySettings.vSyncCount = GraphicQualityOption.In.FindOptionValue<int>(graphicQuality,Global.VerticalSyncCount);
			QualitySettings.lodBias = GraphicQualityOption.In.FindOptionValue<float>(graphicQuality,Global.LodBias);
			QualitySettings.maximumLODLevel = GraphicQualityOption.In.FindOptionValue<int>(graphicQuality,Global.MaximumLODLevel);
			QualitySettings.antiAliasing = GraphicQualityOption.In.FindOptionValue<int>(graphicQuality,Global.AntiAliasing);
			QualitySettings.skinWeights = GraphicQualityOption.In.FindOptionValue<SkinWeights>(graphicQuality,Global.SkinWeights);
			QualitySettings.shadowDistance = GraphicQualityOption.In.FindOptionValue<float>(graphicQuality,Global.ShadowDistance);
			QualitySettings.realtimeReflectionProbes = GraphicQualityOption.In.FindOptionValue<bool>(graphicQuality,Global.RealtimeReflectionProbes);

			ApplyCameraFarClip();
		}
	}
}