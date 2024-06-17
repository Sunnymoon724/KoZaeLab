
public static class LongExtension
{
	public static long Sign(this long _int64)
	{
		return _int64 < 0L ? -1L : _int64 > 0L ? 1L : 0L;
	}

	public static string ToStringComma(this long _int64)
	{
		return string.Format("{0:n0}",_int64);
	}

	public static string ToStringSign(this long _int64)
	{
		return string.Format("{0:+#;-#;0}",_int64);
	}

	public static double ToSeconds(this long _milliSeconds)
	{
		return _milliSeconds/1000.0d;
	}

	public static bool HasFlag(this long _pivot,long _target)
	{
		return (_pivot & _target) != 0;
	}

	public static void AddFlag(this ref long _pivot,long _target)
	{
		_pivot |= _target;
	}

	public static void RemoveFlag(this ref long _pivot,long _target)
	{
		_pivot &= ~_target;
	}

	public static void ChangeFlag(this ref long _pivot,long _add,long _remove)
	{
		_pivot.AddFlag(_add);
		_pivot.RemoveFlag(_remove);
	}
}