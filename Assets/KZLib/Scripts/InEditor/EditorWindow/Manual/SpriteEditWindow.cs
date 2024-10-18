#if UNITY_EDITOR
using System;
using KZLib.KZAttribute;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace KZLib.KZWindow
{
	public class SpriteEditWindow : OdinEditorWindow
	{
#pragma warning disable IDE0051
		[BoxGroup("Button",ShowLabel = false,Order = 1)]
		[HorizontalGroup("Button/0"),Button("Get Image",ButtonSizes.Large)]
        private void OnGetImage()
        {
			m_SpritePath = FileUtility.GetFilePathInPanel("Change new path.",".png");
		}

		[HorizontalGroup("Button/0"),Button("Convert Image",ButtonSizes.Large),EnableIf(nameof(IsExist))]
		private void OnConvertImage()
		{
			var bytes = FileUtility.ReadFileToBytes(m_SpritePath);
			var texture = new Texture2D(1,1);

			try
			{
				if(texture.LoadImage(bytes))
				{
					var color = m_AfterColor;
					var pixelArray = texture.GetPixels32();
					var flag = false;

					for(var i=0;i<pixelArray.Length;i++)
					{
						if(ChangeColor(pixelArray[i]))
						{
							if(!m_IncludeAlpha)
							{
								color.a = pixelArray[i].a;
							}

							pixelArray[i] = color;

							flag = true;
						}
					}

					if(flag)
					{
						texture.SetPixels32(pixelArray);

						FileUtility.WriteTextureToFile(string.Concat(FileUtility.GetPathWithoutExtension(m_SpritePath),"_Convert.png"),texture);

						UnityUtility.DisplayInfo("Image change completed");
					}
				}
				else
				{
					throw new Exception("Fail to load image.");
				}
			}
			catch(Exception _ex)
			{
				UnityUtility.DisplayError($"Image change failed. [{_ex.Message}]");
			}
		}
#pragma warning restore IDE0051

		private bool IsExist => !m_SpritePath.IsEmpty() && !m_BeforeColor.Equals(m_AfterColor);

		[BoxGroup("Variable",ShowLabel = false,Order = 2)]
		[HorizontalGroup("Variable/0"),LabelText("Sprite Path"),SerializeField,KZTexturePath]
		private string m_SpritePath = null;
		[HorizontalGroup("Variable/1"),LabelText("Before Color"),SerializeField]
		private Color32 m_BeforeColor = Color.white;
		[HorizontalGroup("Variable/1"),LabelText("After Color"),SerializeField]
		private Color32 m_AfterColor = Color.white;

		[BoxGroup("Option",ShowLabel = false,Order = 3)]
		[HorizontalGroup("Option/0"),LabelText("Error Range"),SerializeField]
		private uint m_ErrorRange = 0;
		[HorizontalGroup("Option/1"),LabelText("Include Alpha"),SerializeField]
		private bool m_IncludeAlpha = false;

		private bool ChangeColor(Color32 _color)
		{
			return Mathf.Abs(_color.r-m_BeforeColor.r) <= m_ErrorRange && Mathf.Abs(_color.g-m_BeforeColor.g) <= m_ErrorRange && Mathf.Abs(_color.b-m_BeforeColor.b) <= m_ErrorRange;
		}
	}
}
#endif