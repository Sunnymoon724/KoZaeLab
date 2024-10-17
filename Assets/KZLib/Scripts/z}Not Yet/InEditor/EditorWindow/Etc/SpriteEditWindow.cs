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
		[BoxGroup("버튼 그룹",ShowLabel = false,Order = 1)]
		[HorizontalGroup("버튼 그룹/0"),Button("이미지 가져오기",ButtonSizes.Large)]
        private void OnGetImage()
        {
			m_SpritePath = FileUtility.GetFilePathInPanel("위치를 수정 합니다.",".png");
		}

		[HorizontalGroup("버튼 그룹/0"),Button("이미지 변환하기",ButtonSizes.Large),EnableIf(nameof(IsExist))]
		private void OnSetImage()
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
						UnityUtility.DisplayInfo("이미지 변경 완료");
					}
				}
				else
				{
					throw new Exception("파일을 읽지 못했습니다.");
				}
			}
			catch(Exception _ex)
			{
				UnityUtility.DisplayError(string.Format("이미지 변경 실패 [{0}]",_ex.Message));
			}
		}
#pragma warning restore IDE0051

		private bool IsExist => !m_SpritePath.IsEmpty() && !m_BeforeColor.Equals(m_AfterColor);

		[BoxGroup("변수 그룹",ShowLabel = false,Order = 2)]
		[HorizontalGroup("변수 그룹/0"),LabelText("스프라이트 경로"),SerializeField,KZTexturePath]
		private string m_SpritePath = null;
		[HorizontalGroup("변수 그룹/1"),LabelText("변경 전 색상"),SerializeField]
		private Color32 m_BeforeColor = Color.white;
		[HorizontalGroup("변수 그룹/1"),LabelText("변경 후 색상"),SerializeField]
		private Color32 m_AfterColor = Color.white;

		[BoxGroup("옵션 그룹",ShowLabel = false,Order = 3)]
		[HorizontalGroup("옵션 그룹/0"),LabelText("오차 범위"),SerializeField]
		private uint m_ErrorRange = 0;
		[HorizontalGroup("옵션 그룹/1"),LabelText("알파 포함 변경"),SerializeField]
		private bool m_IncludeAlpha = false;

		private bool ChangeColor(Color32 _color)
		{
			return Mathf.Abs(_color.r-m_BeforeColor.r) <= m_ErrorRange && Mathf.Abs(_color.g-m_BeforeColor.g) <= m_ErrorRange && Mathf.Abs(_color.b-m_BeforeColor.b) <= m_ErrorRange;
		}
	}
}
#endif