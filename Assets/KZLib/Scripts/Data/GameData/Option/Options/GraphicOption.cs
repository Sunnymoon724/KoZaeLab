using KZLib.KZUtility;
using Newtonsoft.Json;
using UnityEngine;

namespace GameData
{
	public class GraphicOption : Option
	{
		protected override string OptionKey => "Graphic_Option";
		protected override EventTag OptionTag => EventTag.ChangeGraphicOption;

		private class GraphicData : IOptionData
		{
			[JsonProperty("CurrentResolution")]
			private Vector2Int m_currentResolution = GameSettings.In.ScreenResolution;

			[JsonIgnore]
			public Vector2Int CurrentResolution => m_currentResolution;

			public bool TrySetCurrentResolution(Vector2Int resolution)
			{
				if(m_currentResolution == resolution)
				{
					return false;
				}

				m_currentResolution = resolution;

				Screen.SetResolution(m_currentResolution.x,m_currentResolution.y,ScreenMode);

				return true;
			}

			[JsonProperty("CurrentResolution")]
			private FullScreenMode m_screenMode = FullScreenMode.FullScreenWindow;

			[JsonIgnore]
			public FullScreenMode ScreenMode => m_screenMode;

			public bool SetScreenMode(FullScreenMode screenMode)
			{
				if(m_screenMode == screenMode)
				{
					return false;
				}

				m_screenMode = screenMode;

				Screen.SetResolution(CurrentResolution.x,CurrentResolution.y,m_screenMode);

				return true;
			}

			[JsonProperty("CurrentFrameRate")]
			private int m_currentFrameRate = GameSettings.In.FrameRate;

			[JsonIgnore]
			public int CurrentFrameRate => m_currentFrameRate;

			public bool TrySetCurrentFrameRate(int frameRate)
			{
				if(m_currentFrameRate == frameRate)
				{
					return false;
				}

				m_currentFrameRate = frameRate;

				Application.targetFrameRate = m_currentFrameRate;

				return true;
			}

			[JsonProperty("GraphicQuality")]
			private long m_graphicQuality = GraphicQualityPresetSettings.In.GetPresetQuality(GameSettings.In.GraphicQualityPreset);

			public bool AddGraphicQuality(GraphicQualityType qualityType)
			{
				var quality = m_graphicQuality.AddFlag(qualityType.QualityOption);

				if(m_graphicQuality == quality)
				{
					return false;
				}

				m_graphicQuality = quality;

				CheckGraphicQuality();

				return true;
			}

			public bool RemoveGraphicQuality(GraphicQualityType qualityType)
			{
				var quality = m_graphicQuality.RemoveFlag(qualityType.QualityOption);

				if(m_graphicQuality == quality)
				{
					return false;
				}

				m_graphicQuality = quality;

				CheckGraphicQuality();

				return true;
			}


			private void CheckGraphicQuality()
			{
				QualitySettings.globalTextureMipmapLimit = IsIncludeGraphicQuality(GraphicQualityType.GlobalTextureMipmapLimit) ? 0 : 1;
				QualitySettings.anisotropicFiltering = IsIncludeGraphicQuality(GraphicQualityType.AnisotropicFiltering) ? AnisotropicFiltering.ForceEnable : AnisotropicFiltering.Disable;
				QualitySettings.vSyncCount = IsIncludeGraphicQuality(GraphicQualityType.VerticalSync) ? 1 : 0;
			}

			public bool IsIncludeGraphicQuality(GraphicQualityType qualityType)
			{
				return m_graphicQuality.HasFlag(qualityType.QualityOption);
			}
		}

		private GraphicData m_graphicData = null;

		public override void Initialize()
		{
			base.Initialize();

			m_graphicData = LoadOptionData<GraphicData>();
		}

		public override void Release()
		{
			SaveOptionData(m_graphicData,false);
		}

		public Vector2Int GameResolution
		{
			get => m_graphicData.CurrentResolution;
			set
			{
				if(!m_graphicData.TrySetCurrentResolution(value))
				{
					return;
				}

				LogTag.System.I($"ScreenResolution is changed. [{value}]");

				SaveOptionData(m_graphicData,true);
			}
		}

		public int GameFrameRate
		{
			get => m_graphicData.CurrentFrameRate;
			set
			{
				if(!m_graphicData.TrySetCurrentFrameRate(value))
				{
					return;
				}

				LogTag.System.I($"FrameRate is changed. [{value}]");

				SaveOptionData(m_graphicData,true);
			}
		}

		public bool IsIncludeGraphicQuality(GraphicQualityType qualityType)
		{
			return m_graphicData.IsIncludeGraphicQuality(qualityType);
		}

		public void AddGraphicQuality(GraphicQualityType qualityType)
		{
			if(m_graphicData.AddGraphicQuality(qualityType))
			{
				return;
			}

			LogTag.System.I($"Quality is added. [+{qualityType}]");

			SaveOptionData(m_graphicData,true);
		}

		public void RemoveGraphicQuality(GraphicQualityType qualityType)
		{
			if(m_graphicData.RemoveGraphicQuality(qualityType))
			{
				return;
			}

			LogTag.System.I($"Quality is removed. [-{qualityType}]");

			SaveOptionData(m_graphicData,true);
		}
    }
}