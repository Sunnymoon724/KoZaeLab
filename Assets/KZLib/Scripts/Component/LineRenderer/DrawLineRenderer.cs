using DG.Tweening;
using UnityEngine;

public class DrawLineRenderer : BaseLineRenderer
{
	protected override bool UseGizmos => true;

#if UNITY_EDITOR
	private bool m_StartGizmo = false;
#endif

	private Tween m_Tween = null;

	private void OnDisable()
	{
		ResetRenderer();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		ResetRenderer();
	}

	private void ResetRenderer()
	{
		m_LineRenderer.positionCount = 0;

		m_Tween.Kill();
	}

	public Tween DrawLineTween(Vector3[] _pointArray,float _speed,bool _ignoreTimescale)
	{
		ResetRenderer();

		m_LineRenderer.positionCount = 1;
		m_LineRenderer.SetPosition(0,_pointArray[0]);

		var duration = CommonUtility.GetTotalDistance(_pointArray)/_speed;

		m_Tween = CommonUtility.SetProgress(0.0f,_pointArray.Length-1,duration,(progress)=>
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
		}).SetUpdate(_ignoreTimescale).SetEase(Ease.Linear);

#if UNITY_EDITOR
		m_Tween.OnStart(()=>
		{
			m_StartGizmo = true;
		}).OnComplete(()=>
		{
			m_StartGizmo = false;
		});
#endif

		return m_Tween;
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