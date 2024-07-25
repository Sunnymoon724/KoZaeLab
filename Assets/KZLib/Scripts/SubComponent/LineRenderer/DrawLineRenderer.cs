using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class DrawLineRenderer : BaseLineRenderer
{
	protected override bool UseGizmos => true;

#if UNITY_EDITOR
	private bool m_StartGizmo = false;
#endif

	private CancellationTokenSource m_TokenSource = null;

	protected override void OnDisable()
	{
		base.OnDisable();

		KillRenderer();
	}

	protected override void Release()
	{
		base.Release();

		KillRenderer();
	}

	private void KillRenderer()
	{
		m_LineRenderer.positionCount = 0;

		CommonUtility.KillTokenSource(ref m_TokenSource);
	}

	public async UniTask DrawLineAsync(Vector3[] _pointArray,float _speed,bool _ignoreTimescale)
	{
		CommonUtility.RecycleTokenSource(ref m_TokenSource);

		m_LineRenderer.positionCount = 1;
		m_LineRenderer.SetPosition(0,_pointArray[0]);

		var duration = MathUtility.GetTotalDistance(_pointArray)/_speed;

#if UNITY_EDITOR
		m_StartGizmo = true;
#endif

		await CommonUtility.ExecuteOverTimeAsync(0.0f,_pointArray.Length-1,duration,(progress)=>
		{
			var prev = Mathf.FloorToInt(progress);
			var next = Mathf.CeilToInt(progress);
			var time = progress-prev;

			if(m_LineRenderer.positionCount <= prev)
			{
				var start = m_LineRenderer.positionCount;
				m_LineRenderer.positionCount = prev+1;

				for(var i=start;i<m_LineRenderer.positionCount;i++)
				{
					m_LineRenderer.SetPosition(i,_pointArray[i]);
				}
			}
			else
			{
				var position = Vector3.Lerp(_pointArray[prev],_pointArray[next],time);

				m_LineRenderer.SetPosition(prev,position);
			}
		},_ignoreTimescale,null,m_TokenSource.Token);

#if UNITY_EDITOR
		m_StartGizmo = false;
#endif
	}

#if UNITY_EDITOR
	protected override void DrawGizmo()
	{
		if(!m_StartGizmo || m_LineRenderer.positionCount < 2)
		{
			return;
		}

		var cached = Gizmos.color;

		Gizmos.color = Color.blue;

		Gizmos.DrawSphere(m_LineRenderer.GetPosition(m_LineRenderer.positionCount-1),0.05f);

		Gizmos.color = cached;
	}
#endif
}