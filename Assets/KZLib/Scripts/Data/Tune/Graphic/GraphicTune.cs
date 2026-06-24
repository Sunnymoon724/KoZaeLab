using R3;

namespace KZLib.Data
{
	/// <summary>
	/// User graphic preferences: resolution, frame rate, and a quality bitmask (see <see cref="GraphicQualityOption"/>).
	/// Persistence and change notification only; <see cref="KZLib.GraphicManager"/> subscribes and applies Unity settings.
	/// </summary>
	public class GraphicTune : Tune
	{
		protected GraphicTune() { }

		private ScreenResolution m_resolution;
		public ScreenResolution CurrentResolution => m_resolution;

		public Observable<ScreenResolution> OnChangedResolution => OnChangedWithStart(nameof(m_resolution)).Select(_GetScreenResolution);

		private ScreenResolution _GetScreenResolution(Unit _)
		{
			return CurrentResolution;
		}

		private int m_frameRate;
		public int CurrentFrameRate => m_frameRate;

		public Observable<int> OnChangedFrameRate => OnChangedWithStart(nameof(m_frameRate)).Select(_GetFrameRate);

		private int _GetFrameRate(Unit _)
		{
			return CurrentFrameRate;
		}

		private long m_graphicQuality;
		public long CurrentGraphicQuality => m_graphicQuality;

		public Observable<long> OnChangedGraphicQuality => OnChangedWithStart(nameof(m_graphicQuality)).Select(_GetGraphicQuality);

		private long _GetGraphicQuality(Unit _)
		{
			return CurrentGraphicQuality;
		}

		protected override void _LoadAll()
		{
			// SO must load before Highest default and any GraphicQualityOption query.
			_ = GraphicQualityOption.In;

#if UNITY_ANDROID || UNITY_IOS
			var defaultFrameRate = Global.FrameRate30;
#else
			var defaultFrameRate = Global.FrameRate60;
#endif
			m_resolution		= _LoadValue(nameof(m_resolution),ScreenResolution.TryParse,ScreenResolution.fhd);
			m_frameRate			= _LoadValue(nameof(m_frameRate),int.TryParse,defaultFrameRate);
			m_graphicQuality	= _LoadValue(nameof(m_graphicQuality),long.TryParse,GraphicQualityOption.Highest);
		}

		public void SetScreenResolution(ScreenResolution newResolution)
		{
			_SetValue(ref m_resolution,newResolution,nameof(m_resolution),null);
		}

		public void SetFrameRate(int newFrameRate)
		{
			_SetValue(ref m_frameRate,newFrameRate,nameof(m_frameRate),null);
		}

		public void SetGraphicQuality(long graphicQuality)
		{
			_SetValue(ref m_graphicQuality,graphicQuality,nameof(m_graphicQuality),null);
		}

		/// <summary>Sets the quality bitmask from a preset tier defined in <see cref="GraphicQualityOption"/>.</summary>
		public void SetGraphicQualityPreset(GraphicQualityPresetType presetType)
		{
			SetGraphicQuality(GraphicQualityOption.In.GetGraphicQualityInPreset(presetType));
		}

		public void AddGraphicQuality(long graphicQuality)
		{
			SetGraphicQuality(m_graphicQuality.AddFlag(graphicQuality));
		}

		public void RemoveGraphicQuality(long graphicQuality)
		{
			SetGraphicQuality(m_graphicQuality.RemoveFlag(graphicQuality));
		}
	}
}
