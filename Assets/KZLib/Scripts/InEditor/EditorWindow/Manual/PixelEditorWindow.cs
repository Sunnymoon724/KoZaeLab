#if UNITY_EDITOR
using System;
using KZLib.Attributes;
using KZLib.Utilities;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace KZLib.Windows
{
	public class PixelEditorWindow : OdinEditorWindow
	{
		[BoxGroup("Button",ShowLabel = false,Order = 1)]
		[HorizontalGroup("Button/0"),Button("Find Image",ButtonSizes.Large)]
		protected void OnFindImage()
		{
			m_spritePath = KZEditorKit.FindFilePathInPanel("Change new path.","png");
		}

		[HorizontalGroup("Button/0"),Button("Convert Image",ButtonSizes.Large),EnableIf(nameof(IsExist))]
		protected void OnConvertImage()
		{
			var texture2D = new Texture2D(1,1);

			if(!texture2D.LoadImage(KZFileKit.ReadFileToBytes(m_spritePath)))
			{
				KZEditorKit.DisplayError(new Exception("Fail to load image."));

				return;
			}

			var afterColor = m_afterColor;
			var pixelArray = texture2D.GetPixels32();
			var changed = false;

			for(var i=0;i<pixelArray.Length;i++)
			{
				if(_CanChangeColor(pixelArray[i]))
				{
					if(!m_includeAlpha)
					{
						afterColor.a = pixelArray[i].a;
					}

					pixelArray[i] = afterColor;

					changed = true;
				}

				KZEditorKit.DisplayCancelableProgressBar("Change Color",$"Change Color : {i/pixelArray.Length}",i/(float)pixelArray.Length);
			}

			if(changed)
			{
				texture2D.SetPixels32(pixelArray);

				var convertPath = string.Concat(KZFileKit.GetPathWithoutExtension(m_spritePath),"_Convert.png");

				KZFileKit.WriteByteToFile(convertPath,texture2D.EncodeToPNG());

				KZEditorKit.DisplayInfo("Image change completed");
			}
			else
			{
				KZEditorKit.DisplayInfo("No color to change");
			}

			KZEditorKit.ClearProgressBar();
		}

		private bool IsExist => !m_spritePath.IsEmpty() && !m_beforeColor.Equals(m_afterColor);

		[BoxGroup("Variable",ShowLabel = false,Order = 2)]
		[HorizontalGroup("Variable/0"),SerializeField,KZTexturePath]
		private string m_spritePath = null;
		[HorizontalGroup("Variable/1"),SerializeField]
		private Color32 m_beforeColor = Color.white;
		[HorizontalGroup("Variable/1"),SerializeField]
		private Color32 m_afterColor = Color.white;

		[BoxGroup("Option",ShowLabel = false,Order = 3)]
		[HorizontalGroup("Option/0"),SerializeField,Range(0,255)]
		private int m_errorRange = 0;
		[HorizontalGroup("Option/1"),SerializeField]
		private bool m_includeAlpha = false;

		private bool _CanChangeColor(Color32 color32)
		{
			return Mathf.Abs(color32.r-m_beforeColor.r) <= m_errorRange && Mathf.Abs(color32.g-m_beforeColor.g) <= m_errorRange && Mathf.Abs(color32.b-m_beforeColor.b) <= m_errorRange;
		}
	}
}
#endif