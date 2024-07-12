using UnityEngine;

public static partial class RectTransformExtension
{
	// RectTransform UI 앵커 부모로 최대확장하기
	public static void ExpandAnchorSize(this RectTransform _rectTransform)
	{
		_rectTransform.anchorMin = Vector2.zero;
		_rectTransform.anchorMax = Vector2.one;
		_rectTransform.sizeDelta = Vector2.zero;
	}

	public static void ResetTransform(this RectTransform _rectTransform)
	{
		_rectTransform.anchorMin = Vector2.zero;
		_rectTransform.anchorMax = Vector2.one;
		_rectTransform.offsetMin = Vector2.zero;
		_rectTransform.offsetMax = Vector2.zero;
	}

	public static void SetAnchoredPosition(this RectTransform _rectTransform,Vector2 _point)
	{
		_rectTransform.anchoredPosition = _point;
	}

	public static void SetAnchoredPositionX(this RectTransform _rectTransform,float _x)
	{
		_rectTransform.SetAnchoredPosition(new(_x,_rectTransform.anchoredPosition.y));
	}

	public static void SetAnchoredPositionY(this RectTransform _rectTransform,float _y)
	{
		_rectTransform.SetAnchoredPosition(new(_rectTransform.anchoredPosition.x,_y));
	}

	public static void SetSize(this RectTransform _rectTransform,Vector2 _size)
	{
		_rectTransform.sizeDelta = _size;
	}

	public static void SetWidth(this RectTransform _rectTransform,float _width)
	{
		_rectTransform.SetSize(new(_width,_rectTransform.sizeDelta.y));
	}

	public static void SetHeight(this RectTransform _rectTransform,float _height)
	{
		_rectTransform.SetSize(new(_rectTransform.sizeDelta.x,_height));
	}

	public static void SetLeftAnchorOffset(this RectTransform _rectTransform,float _left)
	{
		_rectTransform.anchorMin = new Vector2(_left,_rectTransform.anchorMin.y);
	}

	public static void SetRightAnchorOffset(this RectTransform _rectTransform,float _right)
	{
		_rectTransform.anchorMax = new Vector2(1.0f-_right,_rectTransform.anchorMax.y);
	}

	public static void SetTopAnchorOffset(this RectTransform _rectTransform,float _top)
	{
		_rectTransform.anchorMax = new Vector2(_rectTransform.anchorMax.x,1.0f-_top);
	}

	public static void SetBottomAnchorOffset(this RectTransform _rectTransform,float _bottom)
	{
		_rectTransform.anchorMin = new Vector2(_rectTransform.anchorMin.x,_bottom);
	}

	public static void SetAnchorOffset(this RectTransform _rectTransform,float _left,float _top,float _right,float _bottom)
	{
		_rectTransform.anchorMin = new Vector2(_left,_bottom);
		_rectTransform.anchorMax = new Vector2(1.0f-_right,1.0f-_top);
	}

	public static void SetLeft(this RectTransform _rectTransform,float _left)
	{
		_rectTransform.offsetMin = new Vector2(_left,_rectTransform.offsetMin.y);
	}
	
	public static void SetRight(this RectTransform _rectTransform,float _right)
	{
		_rectTransform.offsetMax = new Vector2(-_right,_rectTransform.offsetMax.y);
	}

	public static void SetTop(this RectTransform _rectTransform,float _top)
	{
		_rectTransform.offsetMax = new Vector2(_rectTransform.offsetMax.x,-_top);
	}

	public static void SetBottom(this RectTransform _rectTransform,float _bottom)
	{
		_rectTransform.offsetMin = new Vector2(_rectTransform.offsetMin.x,_bottom);
	}
}