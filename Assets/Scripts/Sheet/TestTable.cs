using KZLib;
using KZLib.AttributeDrawer;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MetaData
{
	#region Test Data
	[Serializable]
	public class TestData : IMetaData
	{
		#region Field
		[SerializeField, HideInInspector] private string m_MetaId;
		[SerializeField, HideInInspector] private string m_Version;
		[SerializeField, HideInInspector] private string m_Name;
		[SerializeField, HideInInspector] private DIFFICULTY m_Difficulty;
		#endregion

		#region Property  
		[BoxGroup("정보")]
		[HorizontalGroup("정보/0"), LabelText("MetaId"), LabelWidth(100), ShowInInspector, ReadOnly]
		public string MetaId { get => m_MetaId; private set => m_MetaId = value; }

		[HorizontalGroup("정보/0"), LabelText("Version"), LabelWidth(100), ShowInInspector, ReadOnly]
		public string Version { get => m_Version; private set => m_Version = value; }

		[HorizontalGroup("정보/0"), LabelText("Name"), LabelWidth(100), ShowInInspector, ReadOnly]
		public string Name { get => m_Name; private set => m_Name = value; }

		[HorizontalGroup("정보/0"), LabelText("Difficulty"), LabelWidth(100), ShowInInspector, ReadOnly]
		public DIFFICULTY Difficulty { get => m_Difficulty; private set => m_Difficulty = value; }

		#endregion

		#region Init
		public TestData() { }
		#endregion
	}
	#endregion

	public class TestTable : TableHandler<TestTable>
	{
		[Space(10)]

		// [VerticalGroup("Table/21",Order = 21), LabelText("데이터 리스트"), ListDrawerSettings(Expanded = true,IsReadOnly = true), ShowIf("@m_IsShowAll"), SerializeField,Searchable,ValidateInput("OnCheckDataList")]
		private List<TestData> m_TestList;

		public override List<IMetaData>.Enumerator MetaDataList => m_TestList.ConvertAll<IMetaData>(x=>x as IMetaData).GetEnumerator();

		#region For Editor
		public override void OnRefresh() 
		{
#if UNITY_EDITOR
			{
				base.OnRefresh();

				var dataList = GetDataList<TestData>();

				m_TestList.Clear();
				m_TestList.AddRange(dataList.OrderBy(x=>x.MetaId));

				UnityEditor.AssetDatabase.Refresh();
			}
#endif
		}

		protected override List<SheetPathData> GetPathList()
		{
			return new List<SheetPathData>()
			{
				new SheetPathData("AAA@Test","D:/Documents/Projects/KoZaeLab/Example.xlsx"),
				new SheetPathData("BBB@Test","D:/Documents/Projects/KoZaeLab/Example.xlsx"),
			};
		}

		private bool OnCheckDataList(List<TestData> _dataList,ref string _message)
		{
			if(_dataList == null)
			{
				_message = "리스트가 비어 있습니다!";

				return false;
			}
			
			var idList = new List<string>();

			foreach(var data in _dataList)
			{
				if(data.MetaId.IsEmpty() == false)
				{
					if(idList.Contains(data.MetaId))
					{
						_message = $"{data.MetaId} 가 중복 입니다!";

						return false;
					}
					else
					{
						idList.Add(data.MetaId);
					}
				}
			}

			return true;
		}
		
        #endregion For Editor
    }
}