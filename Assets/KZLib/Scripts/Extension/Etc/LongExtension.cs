
public static class LongExtension
{
	public static long Sign(this long _long)
	{
		return _long < 0L ? -1L : _long > 0L ? 1L : 0L;
	}

	public static string ToStringComma(this long _long)
	{
		return $"{_long:n0}";
	}

	public static string ToStringSign(this long _long)
	{
		return $"{_long:+#;-#;0}";
	}

	public static double ToSeconds(this long _milliSeconds)
	{
		return _milliSeconds/1000.0d;
	}

	public static bool HasFlag(this long _pivot,long _target)
	{
		return (_pivot & _target) != 0;
	}

	public static long AddFlag(this long _pivot,long _target)
	{
		return _pivot |= _target;
	}

	public static long RemoveFlag(this long _pivot,long _target)
	{
		return _pivot &= ~_target;
	}

	public static long ChangeFlag(this long _pivot,long _add,long _remove)
	{
		return _pivot.AddFlag(_add).RemoveFlag(_remove);
	}

	public static string ByteToString(this long _byte)
	{
		_byte.ByteToString(out var _size, out var _unit);

		return $"{_size:N2} {_unit}";
	}

	public static void ByteToString(this long _byte,out double _size,out string _unit)
	{
		var index = 0;
		_size = _byte;

		while(_size >= 1024.0d)
		{
			_size /= 1024.0d;
			index++;
		}

		_unit = Global.BYTE_UNIT_ARRAY[index];
	}

	public static long ByteScaleUpUnit(this long _byte)
	{
		return _byte/1024L;
	}

	public static double ByteScaleUpUnitToDouble(this long _byte)
	{
		return _byte/1024.0d;
	}
}