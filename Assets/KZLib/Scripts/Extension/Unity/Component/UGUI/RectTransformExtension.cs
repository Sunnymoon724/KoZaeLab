using UnityEngine;

public static partial class RectTransformExtension
{
	public static Rect CalculateWorldRect(this RectTransform rectTransform)
	{
		if(!_IsValid(rectTransform))
		{
			return default;
		}

		var cornerArray = _GetCornerArray(rectTransform);

		return new Rect(cornerArray[0],new Vector2(Vector3.Distance(cornerArray[0],cornerArray[1]),Vector3.Distance(cornerArray[1],cornerArray[2])));
	}

	public static Vector2 CalculateWorldCenter(this RectTransform rectTransform)
	{
		if(!_IsValid(rectTransform))
		{
			return default;
		}

		var cornerArray = _GetCornerArray(rectTransform);

		return new Vector2((cornerArray[0].x+cornerArray[3].x)/2.0f,(cornerArray[0].y+cornerArray[1].y)/2.0f);
	}

	public static Vector2 CalculateWorldSize(this RectTransform rectTransform)
	{
		if(!_IsValid(rectTransform))
		{
			return default;
		}

		var cornerArray = _GetCornerArray(rectTransform);

		return new Vector2(Vector3.Distance(cornerArray[1],cornerArray[2]),Vector3.Distance(cornerArray[0],cornerArray[1]));
	}

	public static float CalculateWorldLeft(this RectTransform rectTransform)
	{
		if(!_IsValid(rectTransform))
		{
			return default;
		}

		var cornerArray = _GetCornerArray(rectTransform);

		return cornerArray[0].x;
	}

	public static float CalculateWorldRight(this RectTransform rectTransform)
	{
		if(!_IsValid(rectTransform))
		{
			return default;
		}

		var cornerArray = _GetCornerArray(rectTransform);

		return cornerArray[2].x;
	}

	public static float CalculateWorldTop(this RectTransform rectTransform)
	{
		if(!_IsValid(rectTransform))
		{
			return default;
		}

		var cornerArray = _GetCornerArray(rectTransform);

		return cornerArray[1].y;
	}

	public static float CalculateWorldBottom(this RectTransform rectTransform)
	{
		if(!_IsValid(rectTransform))
		{
			return default;
		}

		var cornerArray = _GetCornerArray(rectTransform);

		return cornerArray[0].y;
	}

	public static Vector2 CalculateWorldTopLeft(this RectTransform rectTransform)
	{
		if(!_IsValid(rectTransform))
		{
			return default;
		}

		var cornerArray = _GetCornerArray(rectTransform);

		return new Vector2(cornerArray[0].x,cornerArray[1].y);
	}

	public static Vector2 CalculateWorldTopRight(this RectTransform rectTransform)
	{
		if(!_IsValid(rectTransform))
		{
			return default;
		}

		var cornerArray = _GetCornerArray(rectTransform);

		return new Vector2(cornerArray[2].x,cornerArray[1].y);
	}

	public static Vector2 CalculateWorldBottomLeft(this RectTransform rectTransform)
	{
		if(!_IsValid(rectTransform))
		{
			return default;
		}

		var cornerArray = _GetCornerArray(rectTransform);

		return new Vector2(cornerArray[0].x, cornerArray[0].y);
	}

	public static Vector2 CalculateWorldBottomRight(this RectTransform rectTransform)
	{
		if(!_IsValid(rectTransform))
		{
			return default;
		}

		var cornerArray = _GetCornerArray(rectTransform);

		return new Vector2(cornerArray[2].x,cornerArray[0].y);
	}

	public static Rect ToScreenSpace(this RectTransform rectTransform) 
	{
		var size = Vector2.Scale(rectTransform.rect.size,rectTransform.lossyScale);
		var rect = new Rect(rectTransform.position.x,Screen.height-rectTransform.position.y,size.x,size.y);

		rect.x -= rectTransform.pivot.x*size.x;
		rect.y -= (1.0f-rectTransform.pivot.y)*size.y;

		return rect;
	}

	public static Rect CalculateScreenRect(this RectTransform rectTransform)
	{
		if(!_IsValid(rectTransform))
		{
			return default;
		}

		var worldRect = rectTransform.CalculateWorldRect();
		var canvas = rectTransform.GetComponentInParent<Canvas>();
		var canvasRect = canvas.GetComponent<RectTransform>().CalculateWorldRect();

		var screenWidth = Screen.width;
		var screenHeight = Screen.height;

		var size = new Vector2(screenWidth/canvasRect.size.x*worldRect.size.x,screenHeight/canvasRect.size.y*worldRect.size.y);
		
		return new Rect(screenWidth*((worldRect.x-canvasRect.x)/canvasRect.size.x),screenHeight*((-canvasRect.y+worldRect.y)/canvasRect.size.y),size.x,size.y);
	}

	public static Rect CalculateCanvasRect(this RectTransform rectTransform)
	{
		if(!_IsValid(rectTransform))
		{
			return default;
		}

		var canvas = rectTransform.FindParentCanvas();

		if(canvas == null)
		{
			LogSvc.System.E("Canvas is null.");

			return default;
		}

		var worldCornerArray = _GetCornerArray(rectTransform);
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

	public static Canvas FindParentCanvas(this RectTransform rectTransform)
	{
		if(!_IsValid(rectTransform))
		{
			return null;
		}

		var canvas = rectTransform.GetComponentInParent<Canvas>();
		var parent = rectTransform.parent;
		var idx = 0;

		while(!canvas && parent && idx < 50)
		{
			canvas = parent.GetComponent<Canvas>();
			parent = parent.parent;
			idx++;
		}

		return canvas;
	}

	private static Vector3[] _GetCornerArray(RectTransform rectTransform)
	{
		if(!_IsValid(rectTransform))
		{
			return null;
		}

		var cornerArray = new Vector3[4];

		rectTransform.GetWorldCorners(cornerArray);

		return cornerArray;
	}

	public static void ExpandAnchorSize(this RectTransform rectTransform)
	{
		if(!_IsValid(rectTransform))
		{
			return;
		}

		rectTransform.anchorMin = Vector2.zero;
		rectTransform.anchorMax = Vector2.one;
		rectTransform.sizeDelta = Vector2.zero;
	}

	public static void ResetTransform(this RectTransform rectTransform)
	{
		if(!_IsValid(rectTransform))
		{
			return;
		}

		rectTransform.anchorMin = Vector2.zero;
		rectTransform.anchorMax = Vector2.one;
		rectTransform.offsetMin = Vector2.zero;
		rectTransform.offsetMax = Vector2.zero;
	}

	public static void SetAnchoredPosition(this RectTransform rectTransform,Vector2 point)
	{
		if(!_IsValid(rectTransform))
		{
			return;
		}

		rectTransform.anchoredPosition = point;
	}

	public static void SetAnchoredPositionX(this RectTransform rectTransform,float x)
	{
		if(!_IsValid(rectTransform))
		{
			return;
		}

		rectTransform.SetAnchoredPosition(new(x,rectTransform.anchoredPosition.y));
	}

	public static void SetAnchoredPositionY(this RectTransform rectTransform,float y)
	{
		if(!_IsValid(rectTransform))
		{
			return;
		}

		rectTransform.SetAnchoredPosition(new(rectTransform.anchoredPosition.x,y));
	}

	public static void SetSizeDelta(this RectTransform rectTransform,Vector2 sizeDelta)
	{
		if(!_IsValid(rectTransform))
		{
			return;
		}

		rectTransform.sizeDelta = sizeDelta;
	}

	public static void SetWidth(this RectTransform rectTransform,float width)
	{
		if(!_IsValid(rectTransform))
		{
			return;
		}

		rectTransform.SetSizeDelta(new(width,rectTransform.sizeDelta.y));
	}

	public static void SetHeight(this RectTransform rectTransform,float height)
	{
		if(!_IsValid(rectTransform))
		{
			return;
		}

		rectTransform.SetSizeDelta(new(rectTransform.sizeDelta.x,height));
	}

	public static void SetLeftAnchorOffset(this RectTransform rectTransform,float left)
	{
		if(!_IsValid(rectTransform))
		{
			return;
		}

		rectTransform.anchorMin = new Vector2(left,rectTransform.anchorMin.y);
	}

	public static void SetRightAnchorOffset(this RectTransform rectTransform,float right)
	{
		if(!_IsValid(rectTransform))
		{
			return;
		}

		rectTransform.anchorMax = new Vector2(1.0f-right,rectTransform.anchorMax.y);
	}

	public static void SetTopAnchorOffset(this RectTransform rectTransform,float top)
	{
		if(!_IsValid(rectTransform))
		{
			return;
		}

		rectTransform.anchorMax = new Vector2(rectTransform.anchorMax.x,1.0f-top);
	}

	public static void SetBottomAnchorOffset(this RectTransform rectTransform,float bottom)
	{
		if(!_IsValid(rectTransform))
		{
			return;
		}

		rectTransform.anchorMin = new Vector2(rectTransform.anchorMin.x,bottom);
	}

	public static void SetAnchorOffset(this RectTransform rectTransform,float left,float top,float right,float bottom)
	{
		if(!_IsValid(rectTransform))
		{
			return;
		}

		rectTransform.anchorMin = new Vector2(left,bottom);
		rectTransform.anchorMax = new Vector2(1.0f-right,1.0f-top);
	}

	public static void SetLeft(this RectTransform rectTransform,float left)
	{
		if(!_IsValid(rectTransform))
		{
			return;
		}

		rectTransform.offsetMin = new Vector2(left,rectTransform.offsetMin.y);
	}
	
	public static void SetRight(this RectTransform rectTransform,float right)
	{
		if(!_IsValid(rectTransform))
		{
			return;
		}

		rectTransform.offsetMax = new Vector2(-right,rectTransform.offsetMax.y);
	}

	public static void SetTop(this RectTransform rectTransform,float top)
	{
		if(!_IsValid(rectTransform))
		{
			return;
		}

		rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x,-top);
	}

	public static void SetBottom(this RectTransform rectTransform,float bottom)
	{
		if(!_IsValid(rectTransform))
		{
			return;
		}

		rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x,bottom);
	}

	private static bool _IsValid(RectTransform rectTransform)
	{
		if(!rectTransform)
		{
			LogSvc.System.E("RectTransform is null.");

			return false;
		}

		return true;
	}
}