using System;
using System.Globalization;
using UnityEngine;

public struct ScreenResolution : IEquatable<ScreenResolution>,IFormattable
{
	public int width;
	public int height;
	public bool full;

	public ScreenResolution(Vector2Int _resolution,bool _isFull)
	{
		width = _resolution.x;
		height = _resolution.y;
		full = _isFull;
	}

	public ScreenResolution(int _width,int _height,bool _isFull)
	{
		width = _width;
		height = _height;
		full = _isFull;
	}

	public bool Equals(ScreenResolution _other)
	{
		return width == _other.width && height == _other.height && full == _other.full;
	}

	public override string ToString()
	{
		return ToString("D",CultureInfo.InvariantCulture);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(width,height,full);
	}

	public override bool Equals(object _other)
	{
		return _other is ScreenResolution other && Equals(other);
	}

	public string ToString(string _format,IFormatProvider _formatProvider)
	{
		if(_format.IsEmpty())
		{
			_format = "D";
		}

		_formatProvider ??= CultureInfo.InvariantCulture.NumberFormat;

		return $"{width.ToString(_format,_formatProvider)}x{height.ToString(_format,_formatProvider)} - {(full ? "Full" : "Window")}";
	}

	public static bool operator ==(ScreenResolution _left,ScreenResolution _right)
	{
		return _left.Equals(_right);
	}

	public static bool operator !=(ScreenResolution _left,ScreenResolution _right)
	{
		return !_left.Equals(_right);
	}
}