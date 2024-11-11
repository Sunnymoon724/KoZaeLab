using System;
using System.Globalization;

public struct SoundVolume : IEquatable<SoundVolume>,IFormattable
{
	public float volume;
	public bool mute;

	public SoundVolume(float _volume,bool _mute)
	{
		volume = _volume;
		mute = _mute;
	}

	public bool Equals(SoundVolume _other)
	{
		return volume == _other.volume && mute == _other.mute;
	}

	public override string ToString()
	{
		return ToString(null,null);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(volume,mute);
	}

	public override bool Equals(object _other)
	{
		return _other is SoundVolume other && Equals(other);
	}

	public string ToString(string _format,IFormatProvider _formatProvider)
	{
		if(_format.IsEmpty())
		{
			_format = "F2";
		}

		_formatProvider ??= CultureInfo.InvariantCulture.NumberFormat;

		return $"{volume.ToString(_format,_formatProvider)} - Mute : {(mute ? "O" : "X")}";
	}

	public static bool operator ==(SoundVolume _left,SoundVolume _right)
	{
		return _left.Equals(_right);
	}

	public static bool operator !=(SoundVolume _left,SoundVolume _right)
	{
		return !_left.Equals(_right);
	}
}