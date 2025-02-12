using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class DrawLineRenderer : BaseLineRenderer
{
	protected override bool UseGizmos => true;

#if UNITY_EDITOR
	private bool m_startGizmo = false;
#endif

	private CancellationTokenSource m_tokenSource = null;

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
		m_lineRenderer.positionCount = 0;

		CommonUtility.KillTokenSource(ref m_tokenSource);
	}

	public async UniTask DrawLineAsync(Vector3[] pointArray,float speed,bool ignoreTimescale)
	{
		CommonUtility.RecycleTokenSource(ref m_tokenSource);

		m_lineRenderer.positionCount = 1;
		m_lineRenderer.SetPosition(0,pointArray[0]);

		var duration = CommonUtility.GetTotalDistance(pointArray)/speed;

#if UNITY_EDITOR
		m_startGizmo = true;
#endif

		await CommonUtility.ExecuteProgressAsync(0.0f,pointArray.Length-1,duration,(progress)=>
		{
			var prev = Mathf.FloorToInt(progress);
			var next = Mathf.CeilToInt(progress);
			var time = progress-prev;

			if(m_lineRenderer.positionCount <= prev)
			{
				var start = m_lineRenderer.positionCount;
				m_lineRenderer.positionCount = prev+1;

				for(var i=start;i<m_lineRenderer.positionCount;i++)
				{
					m_lineRenderer.SetPosition(i,pointArray[i]);
				}
			}
			else
			{
				var position = Vector3.Lerp(pointArray[prev],pointArray[next],time);

				m_lineRenderer.SetPosition(prev,position);
			}
		},ignoreTimescale,null,m_tokenSource.Token);

#if UNITY_EDITOR
		m_startGizmo = false;
#endif
	}

#if UNITY_EDITOR
	protected override void DrawGizmo()
	{
		if(!m_startGizmo || m_lineRenderer.positionCount < 2)
		{
			return;
		}

		var cached = Gizmos.color;

		Gizmos.color = Color.blue;

		Gizmos.DrawSphere(m_lineRenderer.GetPosition(m_lineRenderer.positionCount-1),0.05f);

		Gizmos.color = cached;
	}
#endif
}