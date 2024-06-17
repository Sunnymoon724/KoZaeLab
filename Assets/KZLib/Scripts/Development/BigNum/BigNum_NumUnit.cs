using System;
using System.Collections.Generic;
using System.Linq;

namespace KZLib.KZDevelop
{
	public partial struct BigNum : IComparable<BigNum>,IEquatable<BigNum>
	{
		private static class NumUnit
		{
			private static readonly Dictionary<int,string> m_UnitDict = InitializeUnitDict();

			private static Dictionary<int,string> InitializeUnitDict()
			{
				var unitDict = new Dictionary<int,string>();
				var basicArray = new string[] { "","K","M","B","T","aa" };

				for(var i=0;i<basicArray.Length;i++)
				{
					unitDict.Add(i*3,basicArray[i]);
				}

				while(CreateUnit(unitDict));

				return unitDict;
			}

			private static bool CreateUnit(Dictionary<int,string> unitDict)
			{
				var lastKey = unitDict.Keys.Last();
				var unit = unitDict[lastKey];

				if(unit.IsEqual("zz"))
				{
					return false;
				}

				var lastArray = unitDict[lastKey].ToCharArray();

				for(var i=0;i<(999999/3)+1;i++)
				{
					if(lastArray[1] =='z')
					{
						lastArray[0]++;
						lastArray[1] = 'a';
					}
					else
					{
						lastArray[1]++;
					}

					var result = string.Join("",lastArray);

					unitDict.Add(lastKey+((i+1)*3),result);

					if(result.IsEqual("zz"))
					{
						return false;
					}
				}

				return true;
			}

			public static int GetExpByUnit(string _unit)
			{
				return m_UnitDict.ContainsValue(_unit) ? m_UnitDict.First(x=>x.Value.IsEqual(_unit)).Key : 0;
			}

			public static string GetUnit(int _power)
			{
				var maximum = m_UnitDict.Keys.Last();

				if(_power >= maximum)
				{
					return MAX_VALUE;
				}

				var previousKey = m_UnitDict.Keys.Last(key => key <= _power);

				return m_UnitDict[previousKey];
			}
		}
	}
}