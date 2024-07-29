#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace KZLib.KZMenu
{
	public static partial class AssetsMenuItem
	{
		private const int PIVOT_ORDER = 2000;

		private struct AssetsCategory
		{
			public const int Total				= PIVOT_ORDER+0*Global.MENU_LINE;
			public const int Prefab				= PIVOT_ORDER+1*Global.MENU_LINE;
			public const int Script				= PIVOT_ORDER+2*Global.MENU_LINE;
			public const int Texture			= PIVOT_ORDER+3*Global.MENU_LINE;
			public const int ScriptableObject	= PIVOT_ORDER+4*Global.MENU_LINE;
		}

		private static Dictionary<string,List<string>> s_AssetsPathListDict = null;

		private static Dictionary<string,List<string>> AssetsPathListDict
		{
			get
			{
				if(s_AssetsPathListDict == null)
				{
					LoadAssetsPath();
				}

				return s_AssetsPathListDict;
			}
		}

		private static void LoadAssetsPath()
		{
			if(UnityUtility.DisplayCancelableProgressBar("에셋 파인더 로드","에셋 파인더 로드 중",0.1f))
			{
				UnityUtility.ClearProgressBar();

				return;
			}

			var pathGroup = UnityUtility.GetAssetPathGroup();
			var totalCount = pathGroup.Count();
			var index = 0;
			var dependantDict = new Dictionary<string,string[]>();

			foreach(var path in pathGroup)
			{
				dependantDict.AddOrUpdate(path,AssetDatabase.GetDependencies(path,false));

				if(UnityUtility.DisplayCancelableProgressBar("에셋 파인더 로드",string.Format("에셋 파인더 로드 중 [{0}/{1}]",index,totalCount),index++,totalCount))
				{
					UnityUtility.ClearProgressBar();

					return;
				}
			}

			s_AssetsPathListDict = new Dictionary<string,List<string>>();

			foreach(var pair in dependantDict)
			{
				foreach(var dependant in pair.Value)
				{
					if(!s_AssetsPathListDict.ContainsKey(dependant))
					{
						s_AssetsPathListDict.Add(dependant,new List<string>());
					}

					s_AssetsPathListDict[dependant].Add(pair.Key);
				}
			}

			UnityUtility.ClearProgressBar();
		}

		public static void ReLoad()
		{
			s_AssetsPathListDict = null;
		}
	}
}
#endif