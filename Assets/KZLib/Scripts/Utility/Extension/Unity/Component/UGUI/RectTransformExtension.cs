using UnityEngine;

/// <summary>
/// Extension methods for <see cref="RectTransform"/> coordinate conversion, bounds calculation, and layout helpers.
/// </summary>
public static partial class RectTransformExtension
{
	/// <summary>
	/// Converts a world-space point to local coordinates within this rect using the UI camera.
	/// </summary>
	public static Vector2 WorldToCanvasLocalPosition(this RectTransform rectTrans,Vector2 point,Camera uiCamera)
	{
		var screen = RectTransformUtility.WorldToScreenPoint(uiCamera,point);

		return GetLocalCanvasPositionFromScreenPosition(rectTrans,screen,uiCamera);
	}

	public static Vector2 GetLocalCanvasPositionFromScreenPosition(this RectTransform rectTrans,Vector2 screenPoint,Camera camera)
	{
		RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTrans,screenPoint,camera,out var point);

		return point;
	}

	/// <summary>
	/// Projects a world position through <paramref name="worldCamera"/> and maps it to local canvas coordinates.
	/// </summary>
	public static Vector2 ConvertWorldPositionToCanvasPosition(this RectTransform rectTrans,Vector3 position,Camera worldCamera, Camera uiCamera )
	{
		var point = worldCamera.WorldToScreenPoint(position);

		return GetLocalCanvasPositionFromScreenPosition(rectTrans,point,uiCamera);
	}

	/// <summary>
	/// Returns the axis-aligned world-space bounds derived from <see cref="RectTransform.GetWorldCorners"/>.
	/// </summary>
	public static Rect CalculateWorldRect(this RectTransform rectTrans)
	{
		if(!_IsValid(rectTrans))
		{
			return default;
		}

		var cornerArray = _GetCornerArray(rectTrans);

		return new Rect(cornerArray[0],new Vector2(Vector3.Distance(cornerArray[0],cornerArray[1]),Vector3.Distance(cornerArray[1],cornerArray[2])));
	}

	public static Vector2 CalculateWorldCenter(this RectTransform rectTrans)
	{
		if(!_IsValid(rectTrans))
		{
			return default;
		}

		var cornerArray = _GetCornerArray(rectTrans);

		return new Vector2((cornerArray[0].x+cornerArray[3].x)/2.0f,(cornerArray[0].y+cornerArray[1].y)/2.0f);
	}

	public static Vector2 CalculateWorldSize(this RectTransform rectTrans)
	{
		if(!_IsValid(rectTrans))
		{
			return default;
		}

		var cornerArray = _GetCornerArray(rectTrans);

		return new Vector2(Vector3.Distance(cornerArray[1],cornerArray[2]),Vector3.Distance(cornerArray[0],cornerArray[1]));
	}

	public static float CalculateWorldLeft(this RectTransform rectTrans)
	{
		if(!_IsValid(rectTrans))
		{
			return default;
		}

		var cornerArray = _GetCornerArray(rectTrans);

		return cornerArray[0].x;
	}

	public static float CalculateWorldRight(this RectTransform rectTrans)
	{
		if(!_IsValid(rectTrans))
		{
			return default;
		}

		var cornerArray = _GetCornerArray(rectTrans);

		return cornerArray[2].x;
	}

	public static float CalculateWorldTop(this RectTransform rectTrans)
	{
		if(!_IsValid(rectTrans))
		{
			return default;
		}

		var cornerArray = _GetCornerArray(rectTrans);

		return cornerArray[1].y;
	}

	public static float CalculateWorldBottom(this RectTransform rectTrans)
	{
		if(!_IsValid(rectTrans))
		{
			return default;
		}

		var cornerArray = _GetCornerArray(rectTrans);

		return cornerArray[0].y;
	}

	public static Vector2 CalculateWorldTopLeft(this RectTransform rectTrans)
	{
		if(!_IsValid(rectTrans))
		{
			return default;
		}

		var cornerArray = _GetCornerArray(rectTrans);

		return new Vector2(cornerArray[0].x,cornerArray[1].y);
	}

	public static Vector2 CalculateWorldTopRight(this RectTransform rectTrans)
	{
		if(!_IsValid(rectTrans))
		{
			return default;
		}

		var cornerArray = _GetCornerArray(rectTrans);

		return new Vector2(cornerArray[2].x,cornerArray[1].y);
	}

	public static Vector2 CalculateWorldBottomLeft(this RectTransform rectTrans)
	{
		if(!_IsValid(rectTrans))
		{
			return default;
		}

		var cornerArray = _GetCornerArray(rectTrans);

		return new Vector2(cornerArray[0].x, cornerArray[0].y);
	}

	public static Vector2 CalculateWorldBottomRight(this RectTransform rectTrans)
	{
		if(!_IsValid(rectTrans))
		{
			return default;
		}

		var cornerArray = _GetCornerArray(rectTrans);

		return new Vector2(cornerArray[2].x,cornerArray[0].y);
	}

	/// <summary>
	/// Returns a screen-space rect with Y flipped from Unity world coordinates to screen coordinates.
	/// </summary>
	public static Rect ToScreenSpace(this RectTransform rectTrans) 
	{
		var size = Vector2.Scale(rectTrans.rect.size,rectTrans.lossyScale);
		var rect = new Rect(rectTrans.position.x,Screen.height-rectTrans.position.y,size.x,size.y);

		rect.x -= rectTrans.pivot.x*size.x;
		rect.y -= (1.0f-rectTrans.pivot.y)*size.y;

		return rect;
	}

	/// <summary>
	/// Maps the world rect of this transform to screen pixels relative to the parent canvas size.
	/// </summary>
	public static Rect CalculateScreenRect(this RectTransform rectTrans)
	{
		if(!_IsValid(rectTrans))
		{
			return default;
		}

		var worldRect = rectTrans.CalculateWorldRect();

		var canvas = rectTrans.GetComponentInParent<Canvas>();

		if(!canvas)
		{
			LogChannel.Kit.E("Canvas not found in parent.");

			return default;
		}

		var canvasRectTrans = canvas.GetComponent<RectTransform>();
		var canvasRect = canvasRectTrans.CalculateWorldRect();

		if(canvasRect.size.x.ApproximatelyZero() || canvasRect.size.y.ApproximatelyZero())
		{
			return default;
		}

		var screenWidth = Screen.width;
		var screenHeight = Screen.height;

		var size = new Vector2(screenWidth/canvasRect.size.x*worldRect.size.x,screenHeight/canvasRect.size.y*worldRect.size.y);
		
		return new Rect(screenWidth*((worldRect.x-canvasRect.x)/canvasRect.size.x),screenHeight*((-canvasRect.y+worldRect.y)/canvasRect.size.y),size.x,size.y);
	}

	/// <summary>
	/// Returns the rect in the local space of the parent canvas.
	/// </summary>
	public static Rect CalculateCanvasRect(this RectTransform rectTrans)
	{
		if(!_IsValid(rectTrans))
		{
			return default;
		}

		var canvas = rectTrans.FindParentCanvas();

		if(canvas == null)
		{
			LogChannel.Kit.E("Canvas is null.");

			return default;
		}

		var worldCornerArray = _GetCornerArray(rectTrans);
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

	/// <summary>
	/// Walks up the hierarchy to find a <see cref="Canvas"/>, with a depth limit of 50 levels.
	/// </summary>
	public static Canvas FindParentCanvas(this RectTransform rectTrans)
	{
		if(!_IsValid(rectTrans))
		{
			return null;
		}

		var canvas = rectTrans.GetComponentInParent<Canvas>();
		var parent = rectTrans.parent;
		var idx = 0;

		while(!canvas && parent && idx < 50)
		{
			canvas = parent.GetComponent<Canvas>();
			parent = parent.parent;
			idx++;
		}

		return canvas;
	}

	private static Vector3[] _GetCornerArray(RectTransform rectTrans)
	{
		if(!_IsValid(rectTrans))
		{
			return null;
		}

		var cornerArray = new Vector3[Global.RectCornerCount];

		rectTrans.GetWorldCorners(cornerArray);

		return cornerArray;
	}

	/// <summary>
	/// Stretches anchors to fill the parent and clears <see cref="RectTransform.sizeDelta"/>.
	/// </summary>
	public static void ExpandAnchorSize(this RectTransform rectTrans)
	{
		if(!_IsValid(rectTrans))
		{
			return;
		}

		rectTrans.anchorMin = Vector2.zero;
		rectTrans.anchorMax = Vector2.one;
		rectTrans.sizeDelta = Vector2.zero;
	}

	/// <summary>
	/// Resets anchors to full stretch and clears offset min/max for a zero-margin fill layout.
	/// </summary>
	public static void ResetTransform(this RectTransform rectTrans)
	{
		if(!_IsValid(rectTrans))
		{
			return;
		}

		rectTrans.anchorMin = Vector2.zero;
		rectTrans.anchorMax = Vector2.one;
		rectTrans.offsetMin = Vector2.zero;
		rectTrans.offsetMax = Vector2.zero;
	}

	public static void SetAnchoredPosition(this RectTransform rectTrans,Vector2 point)
	{
		if(!_IsValid(rectTrans))
		{
			return;
		}

		rectTrans.anchoredPosition = point;
	}

	public static void SetAnchoredPositionX(this RectTransform rectTrans,float x)
	{
		if(!_IsValid(rectTrans))
		{
			return;
		}

		rectTrans.SetAnchoredPosition(new(x,rectTrans.anchoredPosition.y));
	}

	public static void SetAnchoredPositionY(this RectTransform rectTrans,float y)
	{
		if(!_IsValid(rectTrans))
		{
			return;
		}

		rectTrans.SetAnchoredPosition(new(rectTrans.anchoredPosition.x,y));
	}

	public static void SetSizeDelta(this RectTransform rectTrans,Vector2 sizeDelta)
	{
		if(!_IsValid(rectTrans))
		{
			return;
		}

		rectTrans.sizeDelta = sizeDelta;
	}

	public static void SetWidth(this RectTransform rectTrans,float width)
	{
		if(!_IsValid(rectTrans))
		{
			return;
		}

		rectTrans.SetSizeDelta(new(width,rectTrans.sizeDelta.y));
	}

	public static void SetHeight(this RectTransform rectTrans,float height)
	{
		if(!_IsValid(rectTrans))
		{
			return;
		}

		rectTrans.SetSizeDelta(new(rectTrans.sizeDelta.x,height));
	}

	/// <summary>
	/// Sets <see cref="RectTransform.anchorMin.x"/> while preserving the current y anchor.
	/// </summary>
	public static void SetLeftAnchorOffset(this RectTransform rectTrans,float left)
	{
		if(!_IsValid(rectTrans))
		{
			return;
		}

		rectTrans.anchorMin = new Vector2(left,rectTrans.anchorMin.y);
	}

	/// <summary>
	/// Sets <see cref="RectTransform.anchorMax.x"/> as <c>1 - right</c> while preserving the current y anchor.
	/// </summary>
	public static void SetRightAnchorOffset(this RectTransform rectTrans,float right)
	{
		if(!_IsValid(rectTrans))
		{
			return;
		}

		rectTrans.anchorMax = new Vector2(1.0f-right,rectTrans.anchorMax.y);
	}

	/// <summary>
	/// Sets <see cref="RectTransform.anchorMax.y"/> as <c>1 - top</c> while preserving the current x anchor.
	/// </summary>
	public static void SetTopAnchorOffset(this RectTransform rectTrans,float top)
	{
		if(!_IsValid(rectTrans))
		{
			return;
		}

		rectTrans.anchorMax = new Vector2(rectTrans.anchorMax.x,1.0f-top);
	}

	/// <summary>
	/// Sets <see cref="RectTransform.anchorMin.y"/> while preserving the current x anchor.
	/// </summary>
	public static void SetBottomAnchorOffset(this RectTransform rectTrans,float bottom)
	{
		if(!_IsValid(rectTrans))
		{
			return;
		}

		rectTrans.anchorMin = new Vector2(rectTrans.anchorMin.x,bottom);
	}

	/// <summary>
	/// Sets all four anchor offsets (left, top, right, bottom) as normalized fractions from each edge.
	/// </summary>
	public static void SetAnchorOffset(this RectTransform rectTrans,float left,float top,float right,float bottom)
	{
		if(!_IsValid(rectTrans))
		{
			return;
		}

		rectTrans.anchorMin = new Vector2(left,bottom);
		rectTrans.anchorMax = new Vector2(1.0f-right,1.0f-top);
	}

	/// <summary>
	/// Sets <see cref="RectTransform.offsetMin.x"/> (left margin in offset space).
	/// </summary>
	public static void SetLeft(this RectTransform rectTrans,float left)
	{
		if(!_IsValid(rectTrans))
		{
			return;
		}

		rectTrans.offsetMin = new Vector2(left,rectTrans.offsetMin.y);
	}
	
	/// <summary>
	/// Sets <see cref="RectTransform.offsetMax.x"/> as <c>-right</c> (right margin in offset space).
	/// </summary>
	public static void SetRight(this RectTransform rectTrans,float right)
	{
		if(!_IsValid(rectTrans))
		{
			return;
		}

		rectTrans.offsetMax = new Vector2(-right,rectTrans.offsetMax.y);
	}

	/// <summary>
	/// Sets <see cref="RectTransform.offsetMax.y"/> as <c>-top</c> (top margin in offset space).
	/// </summary>
	public static void SetTop(this RectTransform rectTrans,float top)
	{
		if(!_IsValid(rectTrans))
		{
			return;
		}

		rectTrans.offsetMax = new Vector2(rectTrans.offsetMax.x,-top);
	}

	/// <summary>
	/// Sets <see cref="RectTransform.offsetMin.y"/> (bottom margin in offset space).
	/// </summary>
	public static void SetBottom(this RectTransform rectTrans,float bottom)
	{
		if(!_IsValid(rectTrans))
		{
			return;
		}

		rectTrans.offsetMin = new Vector2(rectTrans.offsetMin.x,bottom);
	}

	private static bool _IsValid(RectTransform rectTrans)
	{
		if(!rectTrans)
		{
			LogChannel.Kit.E("RectTransform is null.");

			return false;
		}

		return true;
	}
}
