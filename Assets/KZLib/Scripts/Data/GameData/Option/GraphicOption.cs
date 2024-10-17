using System;
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
			private ScreenResolution m_CurrentResolution = new(GameSettings.In.ScreenResolution,true);

			[JsonIgnore]
			public ScreenResolution CurrentResolution => m_CurrentResolution;

			public bool SetCurrentResolution(ScreenResolution _resolution)
			{
				if(m_CurrentResolution == _resolution)
				{
					return false;
				}

				m_CurrentResolution = _resolution;

				Screen.SetResolution(m_CurrentResolution.width,m_CurrentResolution.height,m_CurrentResolution.full);

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

				return true;
			}

			[JsonProperty("GraphicQuality")]
			private long m_GraphicQuality = GraphicQualityPresetSettings.In.GetPresetQuality(GameSettings.In.GraphicQualityPreset);

			public bool AddGraphicQuality(GraphicQualityTag _qualityTag)
			{
				var quality = m_GraphicQuality.AddFlag(_qualityTag.QualityOption);

				if(m_GraphicQuality == quality)
				{
					return false;
				}

				m_GraphicQuality = quality;

				CheckGraphicQuality();

				return true;
			}

			public bool RemoveGraphicQuality(GraphicQualityTag _qualityTag)
			{
				var quality = m_GraphicQuality.RemoveFlag(_qualityTag.QualityOption);

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
				QualitySettings.globalTextureMipmapLimit = IsIncludeGraphicQualityOption(GraphicQualityTag.GlobalTextureMipmapLimit) ? 0 : 1;
				QualitySettings.anisotropicFiltering = IsIncludeGraphicQualityOption(GraphicQualityTag.AnisotropicFiltering) ? AnisotropicFiltering.ForceEnable : AnisotropicFiltering.Disable;
				QualitySettings.vSyncCount = IsIncludeGraphicQualityOption(GraphicQualityTag.VerticalSync) ? 1 : 0;
			}

			public bool IsIncludeGraphicQualityOption(GraphicQualityTag _qualityTag)
			{
				return m_GraphicQuality.HasFlag(_qualityTag.QualityOption);
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

		public ScreenResolution GameResolution
		{
			get => m_GraphicData.CurrentResolution;
			set
			{
				if(!m_GraphicData.SetCurrentResolution(value))
				{
					return;
				}

				LogTag.Data.I($"ScreenResolution is changed. [{value}]");

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

				LogTag.Data.I($"FrameRate is changed. [{value}]");

				SaveOption(m_GraphicData,true);
			}
		}

		public bool IsIncludeGraphicQualityOption(GraphicQualityTag _qualityTag)
		{
			return m_GraphicData.IsIncludeGraphicQualityOption(_qualityTag);
		}

		public void AddGraphicQuality(GraphicQualityTag _qualityTag)
		{
			if(m_GraphicData.AddGraphicQuality(_qualityTag))
			{
				return;
			}

			LogTag.Data.I($"Quality is added. [+{_qualityTag}]");

			SaveOption(m_GraphicData,true);
		}

		public void RemoveGraphicQuality(GraphicQualityTag _qualityTag)
		{
			if(m_GraphicData.RemoveGraphicQuality(_qualityTag))
			{
				return;
			}

			LogTag.Data.I($"Quality is removed. [-{_qualityTag}]");

			SaveOption(m_GraphicData,true);
		}
    }
}