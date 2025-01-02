using System;
using KZLib.KZUtility;
using Newtonsoft.Json;
using UnityEngine;

namespace GameData
{
	public class GraphicOption : Option
	{
		protected override string OPTION_KEY => "Graphic Option";
		protected override EventTag Tag => EventTag.ChangeGraphicOption;

		[Serializable]
		private class GraphicData
		{
			[JsonProperty("CurrentResolution")]
			private Vector2Int m_CurrentResolution = GameSettings.In.ScreenResolution;

			[JsonIgnore]
			public Vector2Int CurrentResolution => m_CurrentResolution;

			public bool SetCurrentResolution(Vector2Int _resolution)
			{
				if(m_CurrentResolution == _resolution)
				{
					return false;
				}

				m_CurrentResolution = _resolution;

				Screen.SetResolution(m_CurrentResolution.x,m_CurrentResolution.y,ScreenMode);

				return true;
			}

			[JsonProperty("CurrentResolution")]
			private FullScreenMode m_ScreenMode = FullScreenMode.FullScreenWindow;

			[JsonIgnore]
			public FullScreenMode ScreenMode => m_ScreenMode;

			public bool SetScreenMode(FullScreenMode _mode)
			{
				if(m_ScreenMode == _mode)
				{
					return false;
				}

				m_ScreenMode = _mode;

				Screen.SetResolution(CurrentResolution.x,CurrentResolution.y,m_ScreenMode);

				return true;
			}

			[JsonProperty("CurrentFrameRate")]
			private int m_CurrentFrameRate = GameSettings.In.FrameRate;

			[JsonIgnore]
			public int CurrentFrameRate => m_CurrentFrameRate;

			public bool SetCurrentFrameRate(int _frameRate)
			{
				if(m_CurrentFrameRate == _frameRate)
				{
					return false;
				}

				m_CurrentFrameRate = _frameRate;

				Application.targetFrameRate = m_CurrentFrameRate;

				Resolution currentResolution = Screen.currentResolution;

				return true;
			}

			[JsonProperty("GraphicQuality")]
			private long m_GraphicQuality = GraphicQualityPresetSettings.In.GetPresetQuality(GameSettings.In.GraphicQualityPreset);

			public bool AddGraphicQuality(GraphicQualityType qualityType)
			{
				var quality = m_GraphicQuality.AddFlag(qualityType.QualityOption);

				if(m_GraphicQuality == quality)
				{
					return false;
				}

				m_GraphicQuality = quality;

				CheckGraphicQuality();

				return true;
			}

			public bool RemoveGraphicQuality(GraphicQualityType qualityType)
			{
				var quality = m_GraphicQuality.RemoveFlag(qualityType.QualityOption);

				if(m_GraphicQuality == quality)
				{
					return false;
				}

				m_GraphicQuality = quality;

				CheckGraphicQuality();

				return true;
			}


			private void CheckGraphicQuality()
			{
				QualitySettings.globalTextureMipmapLimit = IsIncludeGraphicQualityOption(GraphicQualityType.GlobalTextureMipmapLimit) ? 0 : 1;
				QualitySettings.anisotropicFiltering = IsIncludeGraphicQualityOption(GraphicQualityType.AnisotropicFiltering) ? AnisotropicFiltering.ForceEnable : AnisotropicFiltering.Disable;
				QualitySettings.vSyncCount = IsIncludeGraphicQualityOption(GraphicQualityType.VerticalSync) ? 1 : 0;
			}

			public bool IsIncludeGraphicQualityOption(GraphicQualityType qualityType)
			{
				return m_GraphicQuality.HasFlag(qualityType.QualityOption);
			}
		}

		private GraphicData m_GraphicData = null;

		public override void Initialize()
		{
			base.Initialize();

			LoadOption(ref m_GraphicData);
		}

		public override void Release()
		{
			SaveOption(m_GraphicData,false);
		}

		public Vector2Int GameResolution
		{
			get => m_GraphicData.CurrentResolution;
			set
			{
				if(!m_GraphicData.SetCurrentResolution(value))
				{
					return;
				}

				LogTag.System.I($"ScreenResolution is changed. [{value}]");

				SaveOption(m_GraphicData,true);
			}
		}

		public int GameFrameRate
		{
			get => m_GraphicData.CurrentFrameRate;
			set
			{
				if(!m_GraphicData.SetCurrentFrameRate(value))
				{
					return;
				}

				LogTag.System.I($"FrameRate is changed. [{value}]");

				SaveOption(m_GraphicData,true);
			}
		}

		public bool IsIncludeGraphicQualityOption(GraphicQualityType qualityType)
		{
			return m_GraphicData.IsIncludeGraphicQualityOption(qualityType);
		}

		public void AddGraphicQuality(GraphicQualityType qualityType)
		{
			if(m_GraphicData.AddGraphicQuality(qualityType))
			{
				return;
			}

			LogTag.System.I($"Quality is added. [+{qualityType}]");

			SaveOption(m_GraphicData,true);
		}

		public void RemoveGraphicQuality(GraphicQualityType qualityType)
		{
			if(m_GraphicData.RemoveGraphicQuality(qualityType))
			{
				return;
			}

			LogTag.System.I($"Quality is removed. [-{qualityType}]");

			SaveOption(m_GraphicData,true);
		}
    }
}