using KZLib;
using KZLib.KZDevelop;
using Newtonsoft.Json;
using UnityEngine;

namespace GameData
{
	public partial class Option : IGameData
	{
		private const string GRAPHIC_OPTION = "Graphic Option";

		public partial class Graphic
		{
			[SerializeField,JsonProperty("ScreenWidth")]
			private int m_ScreenWidth = 0;
			[SerializeField,JsonProperty("ScreenHeight")]
			private int m_ScreenHeight = 0;
			[SerializeField,JsonProperty("FullScreen")]
			private bool m_FullScreen = false;
			[JsonIgnore]
			public ScreenResolutionData ScreenResolution
			{
				get => new(m_ScreenWidth,m_ScreenHeight,m_FullScreen);
				set
				{
					if((m_ScreenWidth == value.Width) && (m_ScreenWidth == value.Width) && (m_FullScreen == value.IsFull))
					{
						return;
					}

					m_ScreenWidth = value.Width;
					m_ScreenHeight = value.Height;
					m_FullScreen = value.IsFull;

					Screen.SetResolution(m_ScreenWidth,m_ScreenHeight,m_FullScreen);

					SaveGraphic();
				}
			}

			[SerializeField,JsonProperty("FrameRate")]
			private int m_FrameRate = 0;
			[JsonIgnore]
			public int FrameRate
			{
				get => m_FrameRate;
				set
				{
					if(m_FrameRate == value)
					{
						return;
					}

					m_FrameRate = value;

					Application.targetFrameRate = m_FrameRate;

					SaveGraphic();
				}
			}

			[SerializeField,JsonProperty("GraphicQuality")]
			private long m_GraphicQuality = 0L;
			[JsonIgnore]
			public long GraphicQuality
			{
				get => m_GraphicQuality;
				set
				{
					if(m_GraphicQuality == value)
					{
						return;
					}

					m_GraphicQuality = value;

					ChangeTextureQuality();
					ChangeMotionBlur();

					SaveGraphic();
				}
			}

			public Graphic()
			{
				var resolution = GameSettings.In.ScreenResolution;

				m_ScreenWidth = resolution.x;
				m_ScreenHeight = resolution.y;

				m_FullScreen = GameSettings.In.FullScreen;

				m_FrameRate = GameSettings.In.FrameRate;
				m_GraphicQuality = GameSettings.In.GraphicQuality;
			}

			private void ChangeTextureQuality()
			{
				// QualitySettings.globalTextureMipmapLimit = IsIncludeGraphicQualityOption(GraphicQualityOption.TEXTURE_QUALITY_FULL) ? 0 : 1;
				QualitySettings.anisotropicFiltering = IsIncludeGraphicQualityOption(GraphicQualityOption.ANISOTROPIC_FILTERING_ENABLE) ? AnisotropicFiltering.ForceEnable : AnisotropicFiltering.Disable;
				QualitySettings.vSyncCount = IsIncludeGraphicQualityOption(GraphicQualityOption.VERTICAL_SYNC_ENABLE) ? 1 : 0;
			}
			
			private void ChangeMotionBlur()
			{
				// if(CameraHandler.In.postProcessingManager != null)
				// {
				// 	bool enabled = (option == GraphicQualityOption.MOTION_BLUR_ENABLE);
				// 	CameraHandler.Instance.postProcessingManager.motionBlurEnabled = enabled;
				// }
			}

			public bool IsIncludeGraphicQualityOption(long _option)
			{
				return GraphicQuality.HasFlag(_option);
			}

			private void SaveGraphic()
			{
				s_SaveHandler.SetObject(GRAPHIC_OPTION,this);

				Broadcaster.SendEvent(EventTag.ChangeGraphicOption);
			}
		}

		public Graphic GraphicOption { get; private set; }

		private void InitializeGraphic()
		{
			GraphicOption = s_SaveHandler.GetObject(GRAPHIC_OPTION,new Graphic());
		}

		private void ReleaseGraphic() { }
    }
}