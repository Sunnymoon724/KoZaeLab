#if UNITY_EDITOR
using UnityEditor;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace KZLib.KZMenu
{
	public static partial class AssetsMenuItem
	{
		[MenuItem("Assets/KZSubMenu/Find Asset In Use",false,AssetsCategory.Total)]
		private static void OnFindAssetInUse()
		{
			if(!IsValidAsset())
			{
				return;
			}

			var textList = new List<string>();

			foreach(var selected in Selection.GetFiltered(typeof(Object),SelectionMode.Assets))
			{
				var asset = AssetDatabase.GetAssetPath(selected);

				textList.Add(string.Format("<b> {0} </b>을 사용하고 있는 에셋 찾기",FileUtility.GetOnlyName(asset)));

				if(AssetsPathListDict.TryGetValue(asset,out var dependantList))
				{
					foreach(var dependant in dependantList)
					{
						textList.Add(string.Format("<a href=\"{0}\">{1}</a> ",dependant,FileUtility.GetOnlyName(dependant)));
					}
				}
			}

			if(textList.IsNullOrEmpty())
			{
				LogTag.Editor.I("사용하고 있는 에셋이 없습니다.");
			}
			else
			{
				LogTag.Editor.I("사용하는 에셋 리스트 입니다.");

				foreach(var text in textList)
				{
					LogTag.Editor.I(text);
				}
			}
		}

		[MenuItem("Assets/KZSubMenu/Find Asset Used",false,AssetsCategory.Total)]
		private static void OnFindAssetUsed()
		{
			if(!IsValidAsset())
			{
				return;
			}

			var textList = new List<string>();

			foreach(var selected in Selection.GetFiltered(typeof(Object),SelectionMode.Assets))
			{
				var asset = AssetDatabase.GetAssetPath(selected);

				textList.Add(string.Format("<b> {0} </b>에 사용된 에셋 찾기",FileUtility.GetOnlyName(asset)));

				foreach(var dependant in AssetDatabase.GetDependencies(asset,false))
				{
					if(!dependant.StartsWith(Global.ASSETS_HEADER))
					{
						continue;
					}

					textList.Add(string.Format("<a href=\"{0}\">{1}</a> ",dependant,FileUtility.GetOnlyName(dependant)));
				}
			}

			if(textList.IsNullOrEmpty())
			{
				LogTag.Editor.I("사용된 에셋이 없습니다.");
			}
			else
			{
				LogTag.Editor.I("사용된 에셋 리스트 입니다.");

				foreach(var text in textList)
				{
					LogTag.Editor.I(text);
				}
			}
		}

		private static bool IsValidAsset()
		{
			if(Selection.activeObject == null)
			{
				LogTag.Editor.I("선택된 에셋이 없습니다.");

				return false;
			}

			var path = AssetDatabase.GetAssetPath(Selection.activeObject);

			if(!FileUtility.IsFilePath(path))
			{
				LogTag.Editor.I("폴더를 선택했습니다.");

				return false;
			}

			return true;
		}
	}
}	
#endif