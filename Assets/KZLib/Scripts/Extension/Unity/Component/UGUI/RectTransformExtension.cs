using UnityEngine;

public static partial class RectTransformExtension
{
	public static Rect GetWorldRect(this RectTransform _rectTransform)
	{
		var cornerArray = GetCornerArray(_rectTransform);

		return new Rect(cornerArray[0],new Vector2(Vector3.Distance(cornerArray[0],cornerArray[1]),Vector3.Distance(cornerArray[1],cornerArray[2])));
	}

	public static Vector2 GetWorldCenter(this RectTransform _rectTransform)
	{
		var cornerArray = GetCornerArray(_rectTransform);

		return new Vector2((cornerArray[0].x+cornerArray[3].x)/2.0f,(cornerArray[0].y+cornerArray[1].y)/2.0f);
	}

	public static Vector2 GetWorldSize(this RectTransform _rectTransform)
	{
		var cornerArray = GetCornerArray(_rectTransform);

		return new Vector2(Vector3.Distance(cornerArray[1],cornerArray[2]),Vector3.Distance(cornerArray[0],cornerArray[1]));
	}

	public static float GetWorldLeft(this RectTransform _rectTransform)
	{
		var cornerArray = GetCornerArray(_rectTransform);

		return cornerArray[0].x;
	}

	public static float GetWorldRight(this RectTransform _rectTransform)
	{
		var cornerArray = GetCornerArray(_rectTransform);

		return cornerArray[2].x;
	}

	public static float GetWorldTop(this RectTransform _rectTransform)
	{
		var cornerArray = GetCornerArray(_rectTransform);

		return cornerArray[1].y;
	}

	public static float GetWorldBottom(this RectTransform _rectTransform)
	{
		var cornerArray = GetCornerArray(_rectTransform);

		return cornerArray[0].y;
	}

	public static Vector2 GetWorldTopLeft(this RectTransform _rectTransform)
	{
		var cornerArray = GetCornerArray(_rectTransform);

		return new Vector2(cornerArray[0].x,cornerArray[1].y);
	}

	public static Vector2 GetWorldTopRight(this RectTransform _rectTransform)
	{
		var cornerArray = GetCornerArray(_rectTransform);

		return new Vector2(cornerArray[2].x,cornerArray[1].y);
	}

	public static Vector2 GetWorldBottomLeft(this RectTransform _rectTransform)
	{
		var cornerArray = GetCornerArray(_rectTransform);

		return new Vector2(cornerArray[0].x, cornerArray[0].y);
	}

	public static Vector2 GetWorldBottomRight(this RectTransform _rectTransform)
	{
		var cornerArray = GetCornerArray(_rectTransform);

		return new Vector2(cornerArray[2].x,cornerArray[0].y);
	}

	public static Rect ToScreenSpace(this RectTransform _rectTransform) 
	{
		var size = Vector2.Scale(_rectTransform.rect.size,_rectTransform.lossyScale);
		var rect = new Rect(_rectTransform.position.x,Screen.height-_rectTransform.position.y,size.x,size.y);

		rect.x -= _rectTransform.pivot.x*size.x;
		rect.y -= (1.0f-_rectTransform.pivot.y)*size.y;

		return rect;
	}

	public static Rect GetScreenRect(this RectTransform _rectTransform)
	{
		var worldRect = _rectTransform.GetWorldRect();
		var canvas = _rectTransform.GetComponentInParent<Canvas>();
		var canvasRect = canvas.GetComponent<RectTransform>().GetWorldRect();

		var screenWidth = Screen.width;
		var screenHeight = Screen.height;

		var size = new Vector2(screenWidth/canvasRect.size.x*worldRect.size.x,screenHeight/canvasRect.size.y*worldRect.size.y);
		
		return new Rect(screenWidth*((worldRect.x-canvasRect.x)/canvasRect.size.x),screenHeight*((-canvasRect.y+worldRect.y)/canvasRect.size.y),size.x,size.y);
	}

	public static Rect GetCanvasRect(this RectTransform _rectTransform)
	{
		var canvas = _rectTransform.GetParentCanvas();

		if(canvas == null)
		{
			return Rect.zero;
		}

		var worldCornerArray = GetCornerArray(_rectTransform);
		var localCornerArray = new Vector3[worldCornerArray.Length];

		for(var i=0;i<worldCornerArray.Length;i++)
		{
			localCornerArray[i] = canvas.transform.InverseTransformPoint(worldCornerArray[i]);
		}

		var x		= Mathf.Min(localCornerArray[0].x,localCornerArray[1].x,localCornerArray[2].x, localCornerArray[3].x);
		var y		= Mathf.Min(localCornerArray[0].y,localCornerArray[1].y,localCornerArray[2].y, localCornerArray[3].y);
		var width	= Mathf.Max(localCornerArray[0].x,localCornerArray[1].x,localCornerArray[2].x, localCornerArray[3].x)-x;
		var height	= Mathf.Max(localCornerArray[0].y,localCornerArray[1].y,localCornerArray[2].y, localCornerArray[3].y)-y;

		return new Rect(x,y,width,height);
	}

	public static Canvas GetParentCanvas(this RectTransform _rectTransform)
	{
		var canvas = _rectTransform.GetComponentInParent<Canvas>();
		var parent = _rectTransform.parent;
		var index = 0;

		while(!canvas && parent && index < 50)
		{
			canvas = parent.GetComponent<Canvas>();
			parent = parent.parent;
			index++;
		}

		return canvas;
	}

	private static Vector3[] GetCornerArray(RectTransform _rectTransform)
	{
		var cornerArray = new Vector3[4];

		_rectTransform.GetWorldCorners(cornerArray);

		return cornerArray;
	}

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