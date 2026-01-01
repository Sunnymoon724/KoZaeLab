using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class PatternRawImageUI : BaseRawImageUI
{
	private enum TilingPatternType { MANUAL, CHILD, SELF, }
	
	[SerializeField,Range(-1.0f,1.0f)]
	private float m_scrollSpeedX = 0.0f;

	[SerializeField,Range(-1.0f,1.0f)]
	private float m_scrollSpeedY = 0.0f;

	[SerializeField]
	private TilingPatternType m_tilingType = TilingPatternType.MANUAL;
	[SerializeField,ShowIf(nameof(IsChildPatternType))]
	private float m_childValue = 1.0f;
	[SerializeField,ShowIf(nameof(IsSelfPatternType))]
	private float m_selfValue = 1.0f;

	private bool IsSelfPatternType => m_tilingType == TilingPatternType.SELF;
	private bool IsChildPatternType => m_tilingType == TilingPatternType.CHILD;

	private CancellationTokenSource m_tokenSource = null;

	protected override void OnEnable()
	{
		base.OnEnable();

		_SetPatternAspect();

		if(Application.isPlaying)
        {
			CommonUtility.RecycleTokenSource(ref m_tokenSource);

            _ScrollPatternAsync(m_tokenSource.Token).Forget();
        }
	}

	protected override void OnDisable()
	{
		base.OnDisable();

		CommonUtility.KillTokenSource(ref m_tokenSource);
	}

	private void _SetPatternAspect()
	{
		var texture = m_rawImage.texture;

		if(texture == null)
		{
			return;
		}

		switch(m_tilingType)
		{
			case TilingPatternType.CHILD:
				{
					var rectTransform = transform.parent.GetComponent<RectTransform>();

					_SetPatternRect(rectTransform,m_childValue);

					break;
				}
			case TilingPatternType.SELF:
				{
					var rectTransform = transform.GetComponent<RectTransform>();

					_SetPatternRect(rectTransform,m_selfValue);

					break;
				}
			case TilingPatternType.MANUAL:
			default:
				{
					break;
				}
		}
	}

	private void _SetPatternRect(RectTransform rectTransform,float value)
	{
		var texture = m_rawImage.texture;
		var scrollRect = m_rawImage.uvRect;

		var rectWidth = rectTransform.rect.width;
		var rectHeight = rectTransform.rect.height;

		var textureWidth = texture.width;
		var textureHeight = texture.height;

		var ratio = (float) textureWidth/textureHeight;

		scrollRect.width = rectWidth/(value*100.0f);
		scrollRect.height = rectHeight/(value*100.0f)*ratio;

		m_rawImage.uvRect = scrollRect;
	}

	private async UniTaskVoid _ScrollPatternAsync(CancellationToken token)
	{
		var scrollRect = m_rawImage.uvRect;

		while(!token.IsCancellationRequested)
		{
			scrollRect.x += Time.unscaledDeltaTime * m_scrollSpeedX;
			scrollRect.y += Time.unscaledDeltaTime * m_scrollSpeedY;

			if(Mathf.Abs(scrollRect.x) >= 1.0f)
			{
				scrollRect.x %= 1.0f;
			}

			if(Mathf.Abs(scrollRect.y) >= 1.0f)
			{
				scrollRect.y %= 1.0f;
			}

			m_rawImage.uvRect = scrollRect;

			await UniTask.Yield(token).SuppressCancellationThrow();
		}
	}

	protected override void Reset()
	{
		base.Reset();

		m_rawImage.raycastTarget = false;

		var rectTransform = m_rawImage.rectTransform;

		rectTransform.offsetMin = Vector2.zero;
		rectTransform.offsetMax = Vector2.zero;

		rectTransform.anchorMin = Vector2.zero;
		rectTransform.anchorMax = Vector2.one;

		rectTransform.pivot = new Vector2(0.5f,0.5f);
	}
}