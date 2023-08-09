using System.Collections.Generic;
using System.Linq;

public static class WeightedRandomizer
{
	public static WeightedRandomizer<T> Create<T>(Dictionary<T,int> _weightDict)
	{
		return new WeightedRandomizer<T>(_weightDict);
	}
}

public class WeightedRandomizer<T>
{
	private readonly List<KeyValuePair<T,int>> m_SortedList;
	private readonly int m_Sum;

	public WeightedRandomizer(Dictionary<T,int> _weightDict)
	{
		m_SortedList = new List<KeyValuePair<T,int>>(_weightDict);
		m_Sum = 0;

		m_SortedList.Sort((x,y)=>x.Value.CompareTo(y.Value));

		foreach(var weight in _weightDict.Values)
		{
			m_Sum += weight;
		}
	}

	public T Number
	{
		get
		{
			var value = Tools.GetRndInt(0,m_Sum-1);
			var selected = m_SortedList.Last().Key;

			foreach (var data in m_SortedList)
			{
				if (value < data.Value)
				{
					selected = data.Key;

					break;
				}

				value -= data.Value;
			}

			return selected;
		}
	}
}