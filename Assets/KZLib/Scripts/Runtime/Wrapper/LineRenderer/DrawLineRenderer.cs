using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// Animates a <see cref="LineRenderer"/> along a polyline over time.
/// </summary>
/// <remarks>
/// Progress runs from <c>0</c> to <c>positionArray.Length - 1</c>.
/// Completed vertices stay fixed; the active segment is interpolated.
/// Starting a new draw cancels any in-flight async/tween draw.
/// </remarks>
public class DrawLineRenderer : BaseLineRenderer
{
	private const int MinPointCount = 2;

#if UNITY_EDITOR
	private bool m_startGizmo = false;
#endif

	private CancellationTokenSource m_tokenSource = null;
	private Tween m_tween = null;

	protected override void OnDisable()
	{
		_KillRenderer();

		base.OnDisable();
	}

	protected override void _Release()
	{
		_KillRenderer();

		base._Release();
	}

	/// <summary>Animates the line with UniTask. No-op when input is invalid.</summary>
	public async UniTask DrawLineAsync(Vector3[] positionArray,float speed,bool ignoreTimescale)
	{
		if(!_TryValidateDrawInput(positionArray,speed))
		{
			return;
		}

		_KillRenderer();
		KZExternalKit.RecycleTokenSourceInMono(ref m_tokenSource,this);

		if(_TryCompleteInstantly(positionArray))
		{
			return;
		}

		_PrepareFirstPoint(positionArray);

#if UNITY_EDITOR
		m_startGizmo = true;
#endif

		var duration = _GetDuration(positionArray,speed);

		void _Progress(float progress)
		{
			_DrawLine(progress,positionArray);
		}

		await KZExternalKit.ExecuteProgressAsync(0.0f,positionArray.Length-1,duration,_Progress,ignoreTimescale,null,m_tokenSource.Token).SuppressCancellationThrow();

#if UNITY_EDITOR
		m_startGizmo = false;
#endif
	}

	/// <summary>Animates the line with DOTween. Returns null when input is invalid or the path has zero length.</summary>
	public Tween DrawLineTween(Vector3[] positionArray,float speed,bool ignoreTimescale)
	{
		if(!_TryValidateDrawInput(positionArray,speed))
		{
			return null;
		}

		_KillRenderer();

		if(_TryCompleteInstantly(positionArray))
		{
			return null;
		}

		_PrepareFirstPoint(positionArray);

		var duration = _GetDuration(positionArray,speed);

		void _Progress(float progress)
		{
			_DrawLine(progress,positionArray);
		}

		m_tween = KZExternalKit.SetTweenProgress(0.0f,positionArray.Length-1,duration,_Progress).SetUpdate(ignoreTimescale).SetEase(Ease.Linear);

#if UNITY_EDITOR
		void _Start()
		{
			m_startGizmo = true;
		}

		void _Finish()
		{
			m_startGizmo = false;
		}

		m_tween.OnStart(_Start).OnComplete(_Finish).OnKill(_Finish);
#endif

		return m_tween;
	}

	private void _KillRenderer()
	{
		if(m_lineRenderer)
		{
			m_lineRenderer.positionCount = 0;
		}

		KZExternalKit.KillTokenSource(ref m_tokenSource);

		if(m_tween != null)
		{
			m_tween.Kill();
			m_tween = null;
		}
	}

	private void _PrepareFirstPoint(Vector3[] positionArray)
	{
		m_lineRenderer.positionCount = 1;
		m_lineRenderer.SetPosition(0,positionArray[0]);
	}

	/// <summary>Snaps the full polyline when total path length is zero.</summary>
	private bool _TryCompleteInstantly(Vector3[] positionArray)
	{
		if(KZMathKit.GetTotalDistance(positionArray) > 0.0f)
		{
			return false;
		}

		SetPositionArray(positionArray);

		return true;
	}

	private bool _TryValidateDrawInput(Vector3[] positionArray,float speed)
	{
		if(!_EnsureLineRenderer())
		{
			LogChannel.None.W($"LineRenderer is missing on {gameObject.name}.");

			return false;
		}

		if(positionArray == null || positionArray.Length < MinPointCount)
		{
			LogChannel.None.W($"Draw line requires at least {MinPointCount} positions. Object: {gameObject.name}");

			return false;
		}

		if(speed <= 0.0f)
		{
			LogChannel.None.W($"Draw line speed must be greater than zero. Object: {gameObject.name}");

			return false;
		}

		return true;
	}

	/// <summary>Extends fixed vertices and lerps the active segment tip.</summary>
	private void _DrawLine(float progress,Vector3[] positionArray)
	{
		var maxIndex = positionArray.Length-1;
		progress = Mathf.Clamp(progress,0.0f,maxIndex);

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
		return KZMathKit.GetTotalDistance(positionArray)/speed;
	}

#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		if(!m_startGizmo || !m_lineRenderer || m_lineRenderer.positionCount < MinPointCount)
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
