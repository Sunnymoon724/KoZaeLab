using UnityEngine;

public class AudioGraphImageUI : GraphImageUI
{
	public override void UpdateGraph(float[] valueArray)
	{
		if(IsValidGraph)
		{
			return;
		}

		var iteration = Mathf.FloorToInt(valueArray.Length/(float)m_graphLength);

		for(var i=0;i<m_graphArray.Length;i++)
		{
			var current = 0.0f;

			for(var j=0;j<iteration;j++)
			{
				current += valueArray[i*iteration+j];
			}

			var normalized = _DecibelNormalized(_LinearToDecibel(current/iteration));

			if((i+1)%3 == 0 && i>1)
			{
				var value = (normalized+m_graphArray[i-1]+m_graphArray[i-2])/3.0f;

				m_graphArray[i] = value;
				m_graphArray[i-1] = value;
				m_graphArray[i-2] = -1;
			}
			else
			{
				m_graphArray[i] = normalized;
			}
		}

		SetGraphArray();
	}

	private float _LinearToDecibel(float value)
	{
		return Mathf.Clamp(Mathf.Log10(value)*20.0f,-160.0f,0.0f);
	}

	private float _DecibelNormalized(float value)
	{
		return (value+160.0f)/160.0f;
	}

	protected override void Reset()
	{
		base.Reset();

		m_graphLength = 81;
	}
}