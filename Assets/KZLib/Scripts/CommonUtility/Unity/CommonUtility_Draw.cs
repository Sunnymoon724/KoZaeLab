using UnityEngine;

public static partial class CommonUtility
{
	#region Square
	/// <summary>
	/// 사각형을 그린다. _thickness가 0이하면 체워진 사각형을 그린다.
	/// </summary>
	public static Color32[] DrawSquare(string _hexColor,int _width,int _height,int _thickness = 1)
	{
		return DrawSquare(_hexColor.ToColor(),_width,_height,_thickness);
	}

	/// <summary>
	/// 사각형을 그린다. _thickness가 0이하면 체워진 사각형을 그린다.
	/// </summary>
	public static Color32[] DrawSquare(Color _color,int _width,int _height,int _thickness = 1)
	{
		var colorArray = new Color32[_width*_height];

		for(var i=0;i<_width;i++)
		{
			for(var j=0;j<_height;j++)
			{
				var index = j*_width+i;

				if(_thickness <= 0)
				{
					DrawPoint(colorArray,_width,i,j,_color);
				}
				else
				{
					var isLine = i < _thickness || i >= _width-_thickness || j < _thickness || j >= _height-_thickness;

					DrawPoint(colorArray,_width,i,j,isLine ? _color : new Color32(0x00,0x00,0x00,0x00));
				}
			}
		}

		return colorArray;
	}
	#endregion Square

	#region Circle
	/// <summary>
	/// 원을 그린다. _thickness가 0이하면 체워진 원을 그린다.
	/// </summary>
	public static Color32[] DrawCircle(string _hexColor,int _radius,int _thickness = 1)
	{
		return DrawCircle(_hexColor.ToColor(),_radius,_thickness);
	}

	/// <summary>
	/// 원을 그린다. _thickness가 0이하면 체워진 원을 그린다.
	/// </summary>
	public static Color32[] DrawCircle(Color _color,int _radius,int _thickness = 1)
	{
		var size = 2*_radius;
		var colorArray = new Color32[size*size];

		for(var i=0;i<size;i++)
		{
			for(var j=0;j<size;j++)
			{
				var dx = i-_radius;
				var dy = j-_radius;

				var distance = dx*dx+dy*dy;

				if(_thickness <= 0)
				{
					DrawPoint(colorArray,size,i,j,(distance <= _radius*_radius) ? _color : Color.clear);
				}
				else
				{
					var inner = _radius-_thickness;

					DrawPoint(colorArray,size,i,j,(distance >= inner*inner && distance <= _radius*_radius) ? _color : Color.clear);
				}
			}
		}

		return colorArray;
	}
	#endregion Circle

	#region Ellipse
	/// <summary>
	/// 타원을 그린다. _thickness가 0이하면 체워진 타원을 그린다.
	/// </summary>
	public static Color32[] DrawEllipse(string _hexColor,int _width,int _height,int _thickness = 1)
	{
		return DrawEllipse(_hexColor.ToColor(),_width,_height,_thickness);
	}

	/// <summary>
	/// 타원을 그린다. _thickness가 0이하면 체워진 타원을 그린다.
	/// </summary>
	public static Color32[] DrawEllipse(Color _color,int _width,int _height,int _thickness = 1)
	{
		var colorArray = new Color32[_width*_height];
		var radius = new Vector2Int(_width/2,_height/2);

		for(var i=0;i<_width;i++)
		{
			for(var j=0;j<_height;j++)
			{
				int dx = i-radius.x;
				int dy = j-radius.y;

				if(_thickness <= 0)
				{
					DrawPoint(colorArray,_width,i,j,(dx*dx*radius.y*radius.y+dy*dy*radius.x*radius.x <= radius.x*radius.x*radius.y*radius.y) ? _color : Color.clear);
				}
				else
				{
					var inner = new Vector2Int(radius.x-_thickness,radius.y-_thickness);

					DrawPoint(colorArray,_width,i,j,((dx*dx*inner.y*inner.y+dy*dy*inner.x*inner.x >= inner.x*inner.x*inner.y*inner.y) && (dx*dx*radius.y*radius.y+dy*dy*radius.x*radius.x <= radius.x*radius.x*radius.y*radius.y)) ? _color : Color.clear);
				}
			}
		}

		return colorArray;
	}
	#endregion Ellipse

	#region Line
	/// <summary>
	/// 타원을 그린다. _thickness가 0이하면 체워진 타원을 그린다.
	/// </summary>
	public static Color32[] DrawLine(string _hexColor,Vector2Int _point0,Vector2Int _point1,int _thickness = 1)
	{
		return DrawLine(_hexColor.ToColor(),_point0,_point1,_thickness);
	}
	public static Color32[] DrawLine(Color _color,Vector2Int _point0,Vector2Int _point1,int _thickness = 1)
    {
		var width = Mathf.Abs(_point1.x-_point0.x);
        var height = Mathf.Abs(_point1.y-_point0.y);
		var colorArray = new Color32[width*height];
		var step = new Vector2Int(_point0.x < _point1.x ? 1 : -1,_point0.y < _point1.y ? 1 : -1);
		var error = width-height;

		while(true)
		{
			DrawPoint(colorArray,width,_point0.x,_point0.y,_color);

			if(_point0.x == _point1.x && _point0.y == _point1.y)
			{
				break;
			}

			if(2*error > -height)
			{
				error -= height;
				_point0.x += step.x;
			}

			if(2*error < width)
			{
				error += width;
				_point0.y += step.y;
			}
		}

		return colorArray;
	}
	#endregion Line

	#region Point
	public static void DrawPoint(Color32[] _colorArray,int _width,int _pointX,int _pointY,Color _color)
	{
		var index = _pointY*_width+_pointX;

		if(0 <= index  && index < _colorArray.Length)
		{
			_colorArray[index] = _color;
		}
	}
	#endregion Point
}