using UnityEngine;

namespace HudPanel
{
	public class AudioGraph : BaseComponentUI
	{
		private const int GRAPH_RESOLUTION = 81;

		private float[] m_GraphArray = null;

		[SerializeField]
		private GraphImageUI m_GraphImage = null;

		protected override void Initialize()
		{
			base.Initialize();

			m_GraphArray = new float[GRAPH_RESOLUTION];
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			m_GraphImage.SetResolution(GRAPH_RESOLUTION);
		}

		public void UpdateGraph(float[] _spectrumArray)
		{
			var iteration = Mathf.FloorToInt(_spectrumArray.Length/(float)GRAPH_RESOLUTION);

			for(var i=0;i<GRAPH_RESOLUTION;i++)
			{
				var current = 0.0f;

				for(var j=0;j<iteration;j++)
				{
					current += _spectrumArray[i*iteration+j];
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

			m_GraphImage.UpdateGraph(m_GraphArray);
		}

		private float LinearToDecibel(float _value)
        {
            return Mathf.Clamp(Mathf.Log10(_value)*20.0f,-160.0f,0.0f);
        }

		private float DecibelNormalized(float _value)
        {
            return (_value+160.0f)/160.0f;
        }
	}
}