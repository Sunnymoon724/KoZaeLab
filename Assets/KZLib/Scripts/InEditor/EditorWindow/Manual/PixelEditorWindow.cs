#if UNITY_EDITOR
using System;
using KZLib.KZAttribute;
using KZLib.KZUtility;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace KZLib.KZWindow
{
	public class PixelEditorWindow : OdinEditorWindow
	{
		[BoxGroup("Button",ShowLabel = false,Order = 1)]
		[HorizontalGroup("Button/0"),Button("Find Image",ButtonSizes.Large)]
		protected void OnFindImage()
		{
			m_spritePath = CommonUtility.FindFilePathInPanel("Change new path.","png");
		}

		[HorizontalGroup("Button/0"),Button("Convert Image",ButtonSizes.Large),EnableIf(nameof(IsExist))]
		protected void OnConvertImage()
		{
			var texture2D = new Texture2D(1,1);

			if(!texture2D.LoadImage(FileUtility.ReadFileToBytes(m_spritePath)))
			{
				CommonUtility.DisplayError(new Exception("Fail to load image."));

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

				CommonUtility.DisplayCancelableProgressBar("Change Color",$"Change Color : {i/pixelArray.Length}",i/(float)pixelArray.Length);
			}

			if(changed)
			{
				texture2D.SetPixels32(pixelArray);

				var convertPath = string.Concat(FileUtility.GetPathWithoutExtension(m_spritePath),"_Convert.png");

				FileUtility.WriteByteToFile(convertPath,texture2D.EncodeToPNG());

				CommonUtility.DisplayInfo("Image change completed");
			}
			else
			{
				CommonUtility.DisplayInfo("No color to change");
			}

			CommonUtility.ClearProgressBar();
		}

		private bool IsExist => !m_spritePath.IsEmpty() && !m_beforeColor.Equals(m_afterColor);

		[BoxGroup("Variable",ShowLabel = false,Order = 2)]
		[HorizontalGroup("Variable/0"),LabelText("Sprite Path"),SerializeField,KZTexturePath]
		private string m_spritePath = null;
		[HorizontalGroup("Variable/1"),LabelText("Before Color"),SerializeField]
		private Color32 m_beforeColor = Color.white;
		[HorizontalGroup("Variable/1"),LabelText("After Color"),SerializeField]
		private Color32 m_afterColor = Color.white;

		[BoxGroup("Option",ShowLabel = false,Order = 3)]
		[HorizontalGroup("Option/0"),LabelText("Error Range"),SerializeField,Range(0,255)]
		private int m_errorRange = 0;
		[HorizontalGroup("Option/1"),LabelText("Include Alpha"),SerializeField]
		private bool m_includeAlpha = false;

		private bool _CanChangeColor(Color32 color32)
		{
			return Mathf.Abs(color32.r-m_beforeColor.r) <= m_errorRange && Mathf.Abs(color32.g-m_beforeColor.g) <= m_errorRange && Mathf.Abs(color32.b-m_beforeColor.b) <= m_errorRange;
		}
	}
}
#endif