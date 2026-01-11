using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class DrawLineRenderer : BaseLineRenderer
{
	protected override bool UseGizmos => true;

#if UNITY_EDITOR
	private bool m_startGizmo = false;
#endif

	private CancellationTokenSource m_tokenSource = null;
	private Tween m_tween = null;

	protected override void OnDisable()
	{
		base.OnDisable();

		_KillRenderer();
	}

	protected override void _Release()
	{
		base._Release();

		_KillRenderer();
	}

	private void _KillRenderer()
	{
		m_lineRenderer.positionCount = 0;

		CommonUtility.KillTokenSource(ref m_tokenSource);

		m_tween?.Kill();
	}

	public async UniTask DrawLineAsync(Vector3[] positionArray,float speed,bool ignoreTimescale)
	{
		_StartDrawLine(positionArray,speed,ignoreTimescale);

#if UNITY_EDITOR
		m_startGizmo = true;
#endif

		var duration = _GetDuration(positionArray,speed);

		void _Progress(float progress)
		{
			_DrawLine(progress,positionArray);
		}

		await CommonUtility.ExecuteProgressAsync(0.0f,positionArray.Length-1,duration,_Progress,ignoreTimescale,null,m_tokenSource.Token).SuppressCancellationThrow();

#if UNITY_EDITOR
		m_startGizmo = false;
#endif
	}

	public Tween DrawLineTween(Vector3[] positionArray,float speed,bool ignoreTimescale)
	{
		_StartDrawLine(positionArray,speed,ignoreTimescale);

		var duration = _GetDuration(positionArray,speed);

		void _Progress(float progress)
		{
			_DrawLine(progress,positionArray);
		}

		m_tween = CommonUtility.SetTweenProgress(0.0f,positionArray.Length-1,duration,_Progress).SetUpdate(ignoreTimescale).SetEase(Ease.Linear);

#if UNITY_EDITOR

		void _Start()
		{
			m_startGizmo = true;
		}

		void _Finish()
		{
			m_startGizmo = false;
		}

		m_tween.OnStart(_Start).OnComplete(_Finish);
#endif
		return m_tween;
	}

	private void _StartDrawLine(Vector3[] positionArray,float speed,bool ignoreTimescale)
	{
		_KillRenderer();

		m_lineRenderer.positionCount = 1;
		m_lineRenderer.SetPosition(0,positionArray[0]);
	}

	private void _DrawLine(float progress,Vector3[] positionArray)
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
				m_lineRenderer.SetPosition(i,positionArray[i]);
			}
		}
		else
		{
			var position = Vector3.Lerp(positionArray[prev],positionArray[next],time);

			m_lineRenderer.SetPosition(prev,position);
		}
	}

	private float _GetDuration(Vector3[] positionArray,float speed)
	{
		return CommonUtility.GetTotalDistance(positionArray)/speed;
	}

#if UNITY_EDITOR
	protected override void _DrawGizmo()
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