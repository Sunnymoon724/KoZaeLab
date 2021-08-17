using KZLib;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Table
{
    #region 데이터

    [Serializable]
    public class ExcelExampleData
    {
		[SerializeField, HideInInspector]
		private int no;
		
		[SerializeField, HideInInspector]
		private string version;
		
		[SerializeField, HideInInspector]
		private string name;
		
		[SerializeField, HideInInspector]
		private int[] strength;
		
		[SerializeField, HideInInspector]
		private string difficulty;
		
		[BoxGroup("기본 정보")]
		[HorizontalGroup("기본 정보/0"), LabelWidth(100), ShowInInspector, ReadOnly]
		public int No { get => no; private set => no = value; }
		
		[HorizontalGroup("기본 정보/0"), LabelWidth(100), ShowInInspector, ReadOnly]
		public string Version { get => version; private set => version = value; }
		
		[HorizontalGroup("기본 정보/1"), LabelWidth(100), ShowInInspector, ReadOnly]
		public string Name { get => name; private set => name = value; }
		
		[HorizontalGroup("기본 정보/2"), LabelWidth(100), ShowInInspector, ReadOnly]
		public int[] Strength { get => strength; private set => strength = value; }
		
		[HorizontalGroup("기본 정보/1"), LabelWidth(100), ShowInInspector, ReadOnly]
		public string Difficulty { get => difficulty; private set => difficulty = value; }
		

    }

    #endregion 데이터

    public class ExcelExampleTable : TableConfig<ExcelExampleTable,ExcelExampleData>
    {
		[BoxGroup("10",Order = -1,ShowLabel = false)]
		[HorizontalGroup("10/1",Order = 0), Button("Find File",ButtonSizes.Medium), LabelWidth(100), ShowInInspector]
		private void OnFindFile()
		{
			if(File.Exists(sheetPath))
			{
				System.Diagnostics.Process.Start(sheetPath);
			}
		}

		[BoxGroup("2",Order = 2,ShowLabel = false), LabelText("ExcelExampleData"), ListDrawerSettings(Expanded = true), ShowIf("@IsShowAll"), Searchable]
        public List<ExcelExampleData> dataList;

		private const string sheetPath = "D:/Documents/Projects/KoZaeLab/Example.xlsx";
		private const string workSheet = "ExcelExample";

		#region For Editor

		protected override void OnRefresh()
		{
			if(File.Exists(sheetPath))
			{
				var query = new ExcelQuery(sheetPath,workSheet);

				if(query != null && query.IsValid())
				{
					dataList.Clear();

					foreach(var data in query.Deserialize<ExcelExampleData>())
                    {
						if(dataList.Any(x=>x.No.Equals(data.No)))
                        {
							Debug.LogError($"{workSheet} is overlap value {data.No} [{sheetPath}]");
							break;
                        }
						else
                        {
							dataList.Add(data);
						}
                    }
				}
				else
				{
					Debug.LogError($"{workSheet} is not exist in {sheetPath}");
				}
			}
			else
			{
				Debug.LogError($"excel file is not exist.\n - check the patt({sheetPath})");
			}
		}

		protected override void OnShowList()
		{
			IsShowAll = !IsShowAll;

			if(!IsShowAll)
			{
				fillterList.Clear();

				fillterList.AddRange(dataList.Where(x => GameUtil.CheckVersion(x.Version)));
			}
		}

		#endregion For Editor
    }
}