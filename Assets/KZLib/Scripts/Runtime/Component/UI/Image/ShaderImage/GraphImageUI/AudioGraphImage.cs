using UnityEngine;

public class AudioGraphImage : GraphImage
{
	private const float c_dbScale = 20.0f;
	private const float c_minDecibel = -160.0f;
	private const float c_gapMarker = -1.0f;

	private const int c_smoothingGroup = 3;
	private const int c_defaultGraphLength = 81;

	public override void RefreshGraph(float[] valueArray)
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

			if((i+1)%c_smoothingGroup == 0 && i>1)
			{
				var value = (normalized+m_graphArray[i-1]+m_graphArray[i-2])/(float)c_smoothingGroup;

				m_graphArray[i] = value;
				m_graphArray[i-1] = value;
				m_graphArray[i-2] = c_gapMarker;
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
		return Mathf.Clamp(Mathf.Log10(value)*c_dbScale,c_minDecibel,0.0f);
	}

	private float _DecibelNormalized(float value)
	{
		return (value-c_minDecibel)/-c_minDecibel;
	}

	protected override void _Reset()
	{
		base._Reset();

		m_graphLength = c_defaultGraphLength;
	}
}