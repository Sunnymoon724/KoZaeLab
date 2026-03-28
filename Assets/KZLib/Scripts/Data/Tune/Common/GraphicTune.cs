using KZLib.Development;
using R3;
using UnityEngine;

namespace KZLib.Data
{
	public class GraphicTune : Tune
	{
		private ScreenResolution m_resolution = ScreenResolution.fhd;
		public ScreenResolution CurrentResolution => m_resolution;

		public Observable<ScreenResolution> OnChangedResolution => OnChangedWithStart(nameof(m_resolution)).Select(_GetScreenResolution);

		private ScreenResolution _GetScreenResolution(Unit _)
		{
			return CurrentResolution;
		}


		private int m_frameRate = Global.FRAME_RATE_60;
		public int CurrentFrameRate => m_frameRate;

		public Observable<int> OnChangedFrameRate => OnChangedWithStart(nameof(m_frameRate)).Select(_GetFrameRate);

		private int _GetFrameRate(Unit _)
		{
			return CurrentFrameRate;
		}


		private long m_graphicQuality = GraphicQualityOption.Highest;
		public long CurrentGraphicQuality => m_graphicQuality;

		public Observable<long> OnChangedGraphicQuality => OnChangedWithStart(nameof(m_graphicQuality)).Select(_GetGraphicQuality);

		private long _GetGraphicQuality(Unit _)
		{
			return CurrentGraphicQuality;
		}

		protected override void _LoadAll()
		{
			m_resolution		= _LoadValue(nameof(m_resolution),ScreenResolution.TryParse,ScreenResolution.fhd);
			m_frameRate			= _LoadValue(nameof(m_frameRate),int.TryParse,Global.FRAME_RATE_60);
			m_graphicQuality	= _LoadValue(nameof(m_graphicQuality),long.TryParse,GraphicQualityOption.Highest);
		}

		public void SetScreenResolution(ScreenResolution newResolution)
		{
			void _SetResolution()
			{
				Screen.SetResolution(m_resolution.width,m_resolution.height,m_resolution.fullscreen);
			}

			_SetValue(ref m_resolution,newResolution,nameof(m_resolution),_SetResolution);
		}

		public void SetFrameRate(int newFrameRate)
		{
			void _SetFrameRate()
			{
				Application.targetFrameRate = m_frameRate;
			}

			_SetValue(ref m_frameRate,newFrameRate,nameof(m_frameRate),_SetFrameRate);
		}

		public void AddGraphicQuality(long graphicQuality)
		{
			_SetGraphicQuality(m_graphicQuality.AddFlag(graphicQuality));
		}

		public void RemoveGraphicQuality(long graphicQuality)
		{
			_SetGraphicQuality(m_graphicQuality.RemoveFlag(graphicQuality));
		}

		private void _SetGraphicQuality(long newGraphicQuality)
		{
			_SetValue(ref m_graphicQuality,newGraphicQuality,nameof(m_graphicQuality),_CheckGraphicQuality);
		}

		private void _CheckGraphicQuality()
		{
			QualitySettings.globalTextureMipmapLimit = GraphicQualityOption.In.GetOptionValue<int>(m_graphicQuality,Global.GLOBAL_TEXTURE_MIPMAP_LIMIT);
			QualitySettings.anisotropicFiltering = GraphicQualityOption.In.GetOptionValue<AnisotropicFiltering>(m_graphicQuality,Global.ANISOTROPIC_FILTERING);
			QualitySettings.vSyncCount = GraphicQualityOption.In.GetOptionValue<int>(m_graphicQuality,Global.VERTICAL_SYNC_COUNT);
		}
	}
}