using UnityEngine;

public record ScreenResolutionData(int Width,int Height,bool IsFull)
{
	public ScreenResolutionData(Vector2Int _resolution,bool _isFull) : this(_resolution.x,_resolution.y,_isFull) { }
}

namespace GameData
{
	public class GraphicOption : Option
	{
		protected override string OPTION_KEY => "Graphic Option";
		protected override EventTag Tag => EventTag.ChangeGraphicOption;

		private class GraphicData
		{
			public Vector2Int Resolution { get; set; }
			public bool FullScreen { get; set; }
			public int FrameRate { get; set; }
			public long GraphicQuality { get; set; }
		}

		private GraphicData m_GraphicData = null;

		public override void Initialize()
		{
			m_GraphicData = GetOption(new GraphicData()
			{
				Resolution		= GameSettings.In.ScreenResolution,
				FullScreen		= GameSettings.In.FullScreen,
				FrameRate		= GameSettings.In.FrameRate,
				GraphicQuality	= GraphicQualityPresetSettings.In.GetPresetQuality(GameSettings.In.GraphicQualityPreset),
			});

			Screen.SetResolution(m_GraphicData.Resolution.x,m_GraphicData.Resolution.y,m_GraphicData.FullScreen);
			Application.targetFrameRate = m_GraphicData.FrameRate;

			CheckGraphicQuality();
		}

		public override void Release()
		{
			
		}

		public ScreenResolutionData ScreenResolution
		{
			get => new(m_GraphicData.Resolution,m_GraphicData.FullScreen);
			set
			{
				if((m_GraphicData.Resolution.x == value.Width) && (m_GraphicData.Resolution.y == value.Height) && (m_GraphicData.FullScreen == value.IsFull))
				{
					return;
				}

				m_GraphicData.Resolution = new Vector2Int(value.Width,value.Height);
				m_GraphicData.FullScreen = value.IsFull;

				Screen.SetResolution(m_GraphicData.Resolution.x,m_GraphicData.Resolution.y,m_GraphicData.FullScreen);

				SaveOption(m_GraphicData);
			}
		}

		public int FrameRate
		{
			get => m_GraphicData.FrameRate;
			set
			{
				if(m_GraphicData.FrameRate == value)
				{
					return;
				}

				m_GraphicData.FrameRate = value;

				Application.targetFrameRate = m_GraphicData.FrameRate;

				SaveOption(m_GraphicData);
			}
		}

		public void AddGraphicQuality(GraphicQualityTag _qualityTag)
		{
			var quality = m_GraphicData.GraphicQuality |= _qualityTag.QualityOption;

			if(m_GraphicData.GraphicQuality == quality)
			{
				return;
			}

			m_GraphicData.GraphicQuality = quality;

			CheckGraphicQuality();
		}

		public void RemoveGraphicQuality(GraphicQualityTag _qualityTag)
		{
			var quality = m_GraphicData.GraphicQuality & ~_qualityTag.QualityOption;

			if(m_GraphicData.GraphicQuality == quality)
			{
				return;
			}

			m_GraphicData.GraphicQuality = quality;

			CheckGraphicQuality();
		}

		private void CheckGraphicQuality()
		{
			QualitySettings.globalTextureMipmapLimit = IsIncludeGraphicQualityOption(GraphicQualityTag.GlobalTextureMipmapLimit) ? 0 : 1;
			QualitySettings.anisotropicFiltering = IsIncludeGraphicQualityOption(GraphicQualityTag.AnisotropicFiltering) ? AnisotropicFiltering.ForceEnable : AnisotropicFiltering.Disable;
			QualitySettings.vSyncCount = IsIncludeGraphicQualityOption(GraphicQualityTag.VerticalSync) ? 1 : 0;

			SaveOption(m_GraphicData);
		}

		public bool IsIncludeGraphicQualityOption(GraphicQualityTag _tag)
		{
			return m_GraphicData.GraphicQuality.HasFlag(_tag.QualityOption);
		}
    }
}