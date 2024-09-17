using UnityEngine;

public class ShiftGraphImageUI : GraphImageUI
{
	public void UpdateGraph(float _data)
	{
		if(IsValidate)
		{
			return;
		}

		var dataArray = new float[m_GraphArray.Length];
		var maxValue = _data;

		for(var i=0;i<dataArray.Length-1;i++)
		{
			dataArray[i] = m_GraphArray[i+1];

			maxValue = Mathf.Max(maxValue,dataArray[i]);
		}

		dataArray[^1] = _data;

		for(var i=0;i<m_GraphArray.Length;i++)
		{
			m_GraphArray[i] = dataArray[i]/maxValue;
		}

		SetGraphArray();
	}

	protected override void Reset()
	{
		base.Reset();

		m_GraphLength = 150;
	}
}