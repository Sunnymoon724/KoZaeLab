using UnityEngine;

public class AudioGraphImageUI : GraphImageUI
{
	public override void UpdateGraph(float[] _dataArray)
	{
		if(IsValidate)
		{
			return;
		}

		var iteration = Mathf.FloorToInt(_dataArray.Length/(float)m_GraphLength);

		for(var i=0;i<m_GraphArray.Length;i++)
		{
			var current = 0.0f;

			for(var j=0;j<iteration;j++)
			{
				current += _dataArray[i*iteration+j];
			}

			var normalized = DecibelNormalized(LinearToDecibel(current/iteration));

			if((i+1)%3 == 0 && i>1)
			{
				var value = (normalized+m_GraphArray[i-1]+m_GraphArray[i-2])/3.0f;

				m_GraphArray[i] = value;
				m_GraphArray[i-1] = value;
				m_GraphArray[i-2] = -1;
			}
			else
			{
				m_GraphArray[i] = normalized;
			}
		}

		SetGraphArray();
	}

	private float LinearToDecibel(float _value)
	{
		return Mathf.Clamp(Mathf.Log10(_value)*20.0f,-160.0f,0.0f);
	}

	private float DecibelNormalized(float _value)
	{
		return (_value+160.0f)/160.0f;
	}

	protected override void Reset()
	{
		base.Reset();

		m_GraphLength = 81;
	}
}