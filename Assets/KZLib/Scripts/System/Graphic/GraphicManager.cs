using KZLib.Data;
using KZLib.Utilities;
using R3;
using UnityEngine;

namespace KZLib
{
	/// <summary>
	/// Persists graphic settings in PlayerPrefs and applies them to Unity
	/// (Screen, frame rate, <see cref="QualitySettings"/>, camera far clip via <see cref="CameraManager"/>).
	/// URP Asset is not handled here; use a separate pipeline/preset path when needed.
	/// </summary>
	public class GraphicManager : Singleton<GraphicManager>
	{
		private GraphicQualityOption m_graphicQualityOption = null;

		private GraphicManager() { }

		/// <summary>Preset bitmasks from <see cref="GraphicQualityOption"/>. Access via <see cref="Singleton{T}.In"/> only.</summary>
		public long Highest => m_graphicQualityOption.GetGraphicQualityInPreset(GraphicQualityPresetType.QualityHighest);
		/// <inheritdoc cref="Highest"/>
		public long High => m_graphicQualityOption.GetGraphicQualityInPreset(GraphicQualityPresetType.QualityHigh);
		/// <inheritdoc cref="Highest"/>
		public long Middle => m_graphicQualityOption.GetGraphicQualityInPreset(GraphicQualityPresetType.QualityMiddle);
		/// <inheritdoc cref="Highest"/>
		public long Low => m_graphicQualityOption.GetGraphicQualityInPreset(GraphicQualityPresetType.QualityLow);
		/// <inheritdoc cref="Highest"/>
		public long Lowest => m_graphicQualityOption.GetGraphicQualityInPreset(GraphicQualityPresetType.QualityLowest);

		// Access via GraphicManager.In only; Singleton init completes before the instance is returned.
		private ReactivePrefs<ScreenResolution> m_resolution = null;
		public Observable<ScreenResolution> OnChangedResolution => m_resolution.OnChanged;
		public ScreenResolution Resolution
		{
			get => m_resolution.Value;
			set => _ApplyResolution(value);
		}

		private ReactivePrefs<int> m_frameRate = null;
		public Observable<int> OnChangedFrameRate => m_frameRate.OnChanged;
		public int FrameRate
		{
			get => m_frameRate.Value;
			set => _ApplyFrameRate(value);
		}

		private ReactivePrefs<long> m_graphicQuality = null;
		public Observable<long> OnChangedGraphicQuality => m_graphicQuality.OnChanged;
		public long GraphicQuality
		{
			get => m_graphicQuality.Value;
			set => _ApplyGraphicQuality(value);
		}

		private string _PrefsKey(string name) => $"[{nameof(GraphicManager)}] {name}";

		protected override void _Initialize()
		{
			base._Initialize();

			m_graphicQualityOption = Resources.Load<GraphicQualityOption>("ScriptableObject/GraphicQualityOption");

			if(!m_graphicQualityOption)
			{
				throw new System.InvalidOperationException("GraphicQualityOption not found at Resources/ScriptableObject/GraphicQualityOption.");
			}

			m_resolution = new ReactivePrefs<ScreenResolution>(_PrefsKey(nameof(m_resolution)),ScreenResolution.TryParse,ScreenResolution.fhd);
			m_frameRate = new ReactivePrefs<int>(_PrefsKey(nameof(m_frameRate)),int.TryParse,Global.FrameRate60);
			m_graphicQuality = new ReactivePrefs<long>(_PrefsKey(nameof(m_graphicQuality)),long.TryParse,Highest);

			_ApplyResolution(Resolution);
			_ApplyFrameRate(FrameRate);
			_ApplyGraphicQuality(GraphicQuality);
		}

		protected override void _Release(bool disposing)
		{
			if(disposing)
			{
				m_resolution?.Dispose();
				m_frameRate?.Dispose();
				m_graphicQuality?.Dispose();

				m_graphicQualityOption = null;
			}

			base._Release(disposing);
		}

		private void _ApplyResolution(ScreenResolution resolution)
		{
			m_resolution.TrySetValue(resolution);
			Screen.SetResolution(resolution.width,resolution.height,resolution.fullscreen);
		}

		private void _ApplyFrameRate(int frameRate)
		{
			m_frameRate.TrySetValue(frameRate);
			Application.targetFrameRate = frameRate;
		}

		private void _ApplyGraphicQuality(long graphicQuality)
		{
			m_graphicQuality.TrySetValue(graphicQuality);

			if(_TryFindGraphicQualityOptionValue<int>(graphicQuality,Global.GlobalTextureMipmapLimit,out var globalTextureMipmapLimit))
			{
				QualitySettings.globalTextureMipmapLimit = globalTextureMipmapLimit;
			}

			if(_TryFindGraphicQualityOptionValue<AnisotropicFiltering>(graphicQuality,Global.AnisotropicFiltering,out var anisotropicFiltering))
			{
				QualitySettings.anisotropicFiltering = anisotropicFiltering;
			}

			if(_TryFindGraphicQualityOptionValue<float>(graphicQuality,Global.LodBias,out var lodBias))
			{
				QualitySettings.lodBias = lodBias;
			}

			if(_TryFindGraphicQualityOptionValue<int>(graphicQuality,Global.MaximumLODLevel,out var maximumLODLevel))
			{
				QualitySettings.maximumLODLevel = maximumLODLevel;
			}

			if(_TryFindGraphicQualityOptionValue<int>(graphicQuality,Global.AntiAliasing,out var antiAliasing))
			{
				QualitySettings.antiAliasing = antiAliasing;
			}

			if(_TryFindGraphicQualityOptionValue<SkinWeights>(graphicQuality,Global.SkinWeights,out var skinWeights))
			{
				QualitySettings.skinWeights = skinWeights;
			}

			if(_TryFindGraphicQualityOptionValue<float>(graphicQuality,Global.ShadowDistance,out var shadowDistance))
			{
				QualitySettings.shadowDistance = shadowDistance;
			}

			if(_TryFindGraphicQualityOptionValue<bool>(graphicQuality,Global.RealtimeReflectionProbes,out var realtimeReflectionProbes))
			{
				QualitySettings.realtimeReflectionProbes = realtimeReflectionProbes;
			}

			_ApplyCameraFarClip(graphicQuality);
		}

		private void _ApplyCameraFarClip(long graphicQuality)
		{
			if(!_TryFindGraphicQualityOptionValue<float>(graphicQuality,Global.DisableCameraFarHalf,out var factor))
			{
				return;
			}

			if(!CameraManager.HasInstance)
			{
				return;
			}

			CameraManager.In.ApplyFarClipScale(factor);
		}

		public bool TryFindGraphicQualityOptionValue<TValue>(string optionName,out TValue value)
		{
			return _TryFindGraphicQualityOptionValue(GraphicQuality,optionName,out value);
		}

		private bool _TryFindGraphicQualityOptionValue<TValue>(long graphicQuality,string optionName,out TValue value)
		{
			return m_graphicQualityOption.TryFindOptionValue(graphicQuality,optionName,out value);
		}

		public void AddGraphicQuality(long graphicQuality)
		{
			_ApplyGraphicQuality(GraphicQuality.AddFlag(graphicQuality));
		}

		public void RemoveGraphicQuality(long graphicQuality)
		{
			_ApplyGraphicQuality(GraphicQuality.RemoveFlag(graphicQuality));
		}
	}
}