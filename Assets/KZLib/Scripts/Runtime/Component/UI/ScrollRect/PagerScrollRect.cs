using UnityEngine;
using UnityEngine.EventSystems;
using Cysharp.Threading.Tasks;
using System.Threading;
using Sirenix.OdinInspector;

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

	private void Start()
	{
		m_positionArray = new float[m_itemCount];

		for(var i=0;i<m_itemCount;i++)
		{
			m_positionArray[i] = (float) i/(m_itemCount-1);
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
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
		var min = (m_currentIndex > 0) ? m_positionArray[m_currentIndex-1] : m_positionArray[m_currentIndex];
		var max = (m_currentIndex < m_itemCount-1) ? m_positionArray[m_currentIndex+1] : m_positionArray[m_currentIndex];

		m_scrollRect.horizontalNormalizedPosition = Mathf.Clamp(m_scrollRect.horizontalNormalizedPosition,min,max);
	}

	private async UniTaskVoid _SmoothMoveAsync(float from,float to,CancellationToken token)
	{
		float time = 0.0f;

		while(time < 1.0f)
		{
			time += Time.deltaTime*m_scrollSpeed;

			m_scrollRect.horizontalNormalizedPosition = Mathf.Lerp(from,to,time);

			await UniTask.Yield(token);
		}
	}
}
