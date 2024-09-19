using System;
using UnityEngine;

namespace GameData
{
	public record ScreenResolutionData(int Width,int Height,bool IsFull)
	{
		public ScreenResolutionData(Vector2Int _resolution,bool _isFull) : this(_resolution.x,_resolution.y,_isFull) { }
	}

	public class GraphicOption : Option
	{
		protected override string OPTION_KEY => "Graphic Option";
		protected override EventTag Tag => EventTag.ChangeGraphicOption;

		[Serializable]
		private class Graphic
		{
			public Vector2Int Resolution { get; set; }
			public bool FullScreen { get; set; }
			public int FrameRate { get; set; }
			public long GraphicQuality { get; set; }
		}

		private Graphic m_Graphic = null;

		public override void Initialize()
		{
			m_Graphic = GetOption(new Graphic()
			{
				Resolution		= GameSettings.In.ScreenResolution,
				FullScreen		= GameSettings.In.FullScreen,
				FrameRate		= GameSettings.In.FrameRate,
				GraphicQuality	= GraphicQualityPresetSettings.In.GetPresetQuality(GameSettings.In.GraphicQualityPreset),
			});

			Screen.SetResolution(m_Graphic.Resolution.x,m_Graphic.Resolution.y,m_Graphic.FullScreen);
			Application.targetFrameRate = m_Graphic.FrameRate;

			CheckGraphicQuality(false);
		}

		public override void Release()
		{
			
		}

		public ScreenResolutionData ScreenResolution
		{
			get => new(m_Graphic.Resolution,m_Graphic.FullScreen);
			set
			{
				if((m_Graphic.Resolution.x == value.Width) && (m_Graphic.Resolution.y == value.Height) && (m_Graphic.FullScreen == value.IsFull))
				{
					return;
				}

				m_Graphic.Resolution = new Vector2Int(value.Width,value.Height);
				m_Graphic.FullScreen = value.IsFull;

				Screen.SetResolution(m_Graphic.Resolution.x,m_Graphic.Resolution.y,m_Graphic.FullScreen);

				SaveOption(m_Graphic);
			}
		}

		public int FrameRate
		{
			get => m_Graphic.FrameRate;
			set
			{
				if(m_Graphic.FrameRate == value)
				{
					return;
				}

				m_Graphic.FrameRate = value;

				Application.targetFrameRate = m_Graphic.FrameRate;

				SaveOption(m_Graphic);
			}
		}

		public void AddGraphicQuality(GraphicQualityTag _qualityTag)
		{
			var quality = m_Graphic.GraphicQuality.AddFlag(_qualityTag.QualityOption);

			if(m_Graphic.GraphicQuality == quality)
			{
				return;
			}

			m_Graphic.GraphicQuality = quality;

			CheckGraphicQuality(true);
		}

		public void RemoveGraphicQuality(GraphicQualityTag _qualityTag)
		{
			var quality = m_Graphic.GraphicQuality.RemoveFlag(_qualityTag.QualityOption);

			if(m_Graphic.GraphicQuality == quality)
			{
				return;
			}

			m_Graphic.GraphicQuality = quality;

			CheckGraphicQuality(true);
		}

		private void CheckGraphicQuality(bool _isSave)
		{
			QualitySettings.globalTextureMipmapLimit = IsIncludeGraphicQualityOption(GraphicQualityTag.GlobalTextureMipmapLimit) ? 0 : 1;
			QualitySettings.anisotropicFiltering = IsIncludeGraphicQualityOption(GraphicQualityTag.AnisotropicFiltering) ? AnisotropicFiltering.ForceEnable : AnisotropicFiltering.Disable;
			QualitySettings.vSyncCount = IsIncludeGraphicQualityOption(GraphicQualityTag.VerticalSync) ? 1 : 0;

			if(_isSave)
			{
				SaveOption(m_Graphic);
			}
		}

		public bool IsIncludeGraphicQualityOption(GraphicQualityTag _tag)
		{
			return m_Graphic.GraphicQuality.HasFlag(_tag.QualityOption);
		}
    }
}