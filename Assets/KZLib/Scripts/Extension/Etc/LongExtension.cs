
public static class LongExtension
{
	public static long Sign(this long _long)
	{
		return _long < 0L ? -1L : _long > 0L ? 1L : 0L;
	}

	public static string ToStringComma(this long _long)
	{
		return string.Format("{0:n0}",_long);
	}

	public static string ToStringSign(this long _long)
	{
		return string.Format("{0:+#;-#;0}",_long);
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
}