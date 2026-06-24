using UnityEngine;
using UnityEngine.EventSystems;
using Cysharp.Threading.Tasks;
using System.Threading;
using Sirenix.OdinInspector;

/// <summary>
/// Horizontal pager: snaps to discrete normalized positions on end-drag and clamps drag between adjacent pages.
/// </summary>
public class PagerScrollRect : BaseScrollRect,IEndDragHandler,IDragHandler
{
	[SerializeField]
	private int m_itemCount = 6;
	[SerializeField]
	private float m_scrollSpeed = 10.0f;

	[SerializeField,ReadOnly]
	private int m_currentIndex = 0;

	private float[] m_positionArray = null;

	private CancellationTokenSource m_tokenSource = null;

	protected override void Start()
	{
		base.Start();

		_EnsurePositionArray();
	}

	protected override void OnDisable()
	{
		KZExternalKit.KillTokenSource(ref m_tokenSource);

		base.OnDisable();
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if(!_EnsurePositionArray())
		{
			return;
		}

		var delta = eventData.pressPosition.x-eventData.position.x;

		if(delta > 0 && m_currentIndex < m_itemCount-1)
		{
			m_currentIndex++;
		}
		else if(delta < 0 && m_currentIndex > 0)
		{
			m_currentIndex--;
		}

		KZExternalKit.RecycleTokenSource(ref m_tokenSource);

		_SmoothMoveAsync(m_scrollRect.horizontalNormalizedPosition,m_positionArray[m_currentIndex],m_tokenSource.Token).Forget();
	}

	public void OnDrag(PointerEventData eventData)
	{
		if(!_EnsurePositionArray())
		{
			return;
		}

		var min = (m_currentIndex > 0) ? m_positionArray[m_currentIndex-1] : m_positionArray[m_currentIndex];
		var max = (m_currentIndex < m_itemCount-1) ? m_positionArray[m_currentIndex+1] : m_positionArray[m_currentIndex];

		m_scrollRect.horizontalNormalizedPosition = Mathf.Clamp(m_scrollRect.horizontalNormalizedPosition,min,max);
	}

	private bool _EnsurePositionArray()
	{
		if(!m_scrollRect)
		{
			return false;
		}

		if(m_positionArray != null && m_positionArray.Length == Mathf.Max(1,m_itemCount))
		{
			return true;
		}

		m_itemCount = Mathf.Max(1,m_itemCount);
		m_positionArray = new float[m_itemCount];

		var divisor = Mathf.Max(1,m_itemCount-1);

		for(var i=0;i<m_itemCount;i++)
		{
			m_positionArray[i] = (float)i/divisor;
		}

		m_currentIndex = Mathf.Clamp(m_currentIndex,0,m_itemCount-1);

		return true;
	}

	private async UniTaskVoid _SmoothMoveAsync(float from,float to,CancellationToken token)
	{
		if(m_scrollSpeed <= 0.0f)
		{
			m_scrollRect.horizontalNormalizedPosition = to;

			return;
		}

		float time = 0.0f;

		while(time < 1.0f)
		{
			time += Time.deltaTime*m_scrollSpeed;

			m_scrollRect.horizontalNormalizedPosition = Mathf.Lerp(from,to,time);

			await UniTask.Yield(token);
		}

		m_scrollRect.horizontalNormalizedPosition = to;
	}
}
