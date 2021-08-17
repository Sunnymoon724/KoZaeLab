using KZLib;
using System.Collections.Generic;
using Table;

namespace Data
{
    public class ExcelExample : DataHandler<ExcelExample.DataSet>
    {
        private class Data : IData
        {
            public Dictionary<int,ExcelExampleData> ExcelExampleDict { get; private set; }

            public Data(List<ExcelExampleData> _dataList)
            {
                ExcelExampleDict = new Dictionary<int,ExcelExampleData>(_dataList.Count);

                for(var i=0;i<_dataList.Count;i++)
                {
                    if(GameUtil.CheckVersion(_dataList[i].Version))
                    {
                        ExcelExampleDict.Add(_dataList[i].No,_dataList[i]);
                    }
                }
            }
        }

        public class DataSet : DataBase
        {
            public override void Init()
            {
                var table = ResMgr.In.GetTable<ExcelExampleTable>("ExcelExampleTable");

                if(table != null)
                {
                    SetData(new Data(table.dataList));
                }
            }
        }

        public ExcelExampleData GetExcelExampleData(int _no)
        {
            if(DB.GetData<Data>().ExcelExampleDict.TryGetValue(_no,out var data))
            {
                return data;
            }
            else
            {
                Log.Data.E($"{_no}의 데이터가 존재하지 않습니다.");

                return null;
            }
        }
    }
}