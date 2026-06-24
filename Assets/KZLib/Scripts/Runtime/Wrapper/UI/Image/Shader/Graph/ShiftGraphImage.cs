using UnityEngine;

/// <summary>
/// Rolling graph: shifts history left and appends a new sample, normalized by the window maximum.
/// </summary>
public class ShiftGraphImage : GraphImage
{
	/// <summary>Appends one sample by shifting history; normalizes by the window max.</summary>
	public void RefreshGraph(float value)
	{
		if(!_EnsureGraphArray())
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

		var divisor = Mathf.Max(maxValue,Mathf.Epsilon);

		for(var i=0;i<m_graphArray.Length;i++)
		{
			m_graphArray[i] = valueArray[i]/divisor;
		}

		SetGraphArray();
	}

	protected override void _Reset()
	{
		base._Reset();

		m_graphLength = 150;
	}
}
