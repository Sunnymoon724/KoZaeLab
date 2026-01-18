using UnityEngine;

public class ShiftGraphImage : GraphImage
{
	public void RefreshGraph(float value)
	{
		if(IsValidGraph)
		{
			return;
		}

		var valueArray = new float[m_graphArray.Length];
		var maxValue = value;

		for(var i=0;i<valueArray.Length-1;i++)
		{
			valueArray[i] = m_graphArray[i+1];

			maxValue = Mathf.Max(maxValue,valueArray[i]);
		}

		valueArray[^1] = value;

		for(var i=0;i<m_graphArray.Length;i++)
		{
			m_graphArray[i] = valueArray[i]/maxValue;
		}

		SetGraphArray();
	}

	protected override void Reset()
	{
		base.Reset();

		m_graphLength = 150;
	}
}