using System;
using System.Collections.Generic;
using UnityEngine;

public static partial class CommonUtility
{
	/// <summary>
	/// <para> 만약 음수가 나오면 순환이므로 최대값을 반환한다.</para>
	/// </summary>
	public static int LoopClamp(int _index,int _size)
	{
		return _size < 1 ? 0 : _index < 0 ? _size-1+(_index+1)%_size : _index%_size;
	}

	/// <summary>
	/// <para> 만약 음수가 나오면 순환이므로 최대값을 반환한다.</para>
	/// </summary>
	public static float LoopClamp(float _index,int _size)
	{
		return _size < 1 ? 0 : _index < 0.0f ? _size-1+(_index+1)%_size : _index%_size;
	}

	/// <summary>
	/// 비교 가능한 오브젝트를 범위 안의 오브젝트로 자른다.
	/// </summary>
	public static TCompare Clamp<TCompare>(TCompare _value,TCompare _minimum,TCompare _maximum) where TCompare : IComparable<TCompare>
	{
		return _value.CompareTo(_minimum) < 0 ? _minimum : _value.CompareTo(_maximum) > 0 ? _maximum : _value;
	}

	/// <summary>
	/// 비교 가능한 오브젝트를 범위 안의 오브젝트로 자른다.
	/// </summary>
	public static TCompare MinClamp<TCompare>(TCompare _value,TCompare _minimum) where TCompare : IComparable<TCompare>
	{
		return _value.CompareTo(_minimum) < 0 ? _minimum : _value;
	}

	/// <summary>
	/// 비교 가능한 오브젝트를 범위 안의 오브젝트로 자른다.
	/// </summary>
	public static TCompare MaxClamp<TCompare>(TCompare _value,TCompare _maximum) where TCompare : IComparable<TCompare>
	{
		return _value.CompareTo(_maximum) > 0 ? _maximum : _value;
	}

	public static string GetRndText(int _length,bool _overlap = true)
	{
		var text = "abcdefghijklmnopqrstuvwxyz";
		var charList = new List<char>(_length);

		if(_overlap)
		{
			for(var i=0;i<_length;i++)
			{
				charList.Add(text.GetRndValue());
			}
		}
		else
		{
			var length = Mathf.Max(text.Length,_length);

			charList.AddRange(text);
			charList = charList.GetRndValueList(length);
		}

		charList.Randomize();

		return string.Concat(charList);
	}
	
	public static bool TryDispose<TClass>(ref TClass _object) where TClass : class
	{
		if(_object == null)
		{
			return false;
		}

		if(_object is IDisposable disposable)
		{
			disposable.Dispose();
		}

		_object = null;

		return true;
	}

	public static bool Dispose<TClass>(ref TClass _object) where TClass : class,IDisposable
	{
		if(_object == null)
		{
			return false;
		}

		_object.Dispose();
		_object = null;

		return true;
	}

	public static bool ReplaceDisposable<TClass>(ref TClass _object,TClass _replace) where TClass : class,IDisposable
	{
		return ReplaceDisposable(ref _object,_replace,EqualityComparer<TClass>.Default);
	}

	public static bool ReplaceDisposable<TClass>(ref TClass _object,TClass _replace,IEqualityComparer<TClass> _comparer) where TClass : class,IDisposable
	{
		if(_comparer.Equals(_object,_replace))
		{
			return false;
		}

		_object?.Dispose();
		_object = _replace;

		return true;
	}

	public static bool Replace<TObject>(ref TObject _object,TObject _replace)
	{
		return Replace(ref _object,_replace,EqualityComparer<TObject>.Default);
	}
	
	public static bool Replace<TObject>(ref TObject _object,TObject _replace,IEqualityComparer<TObject> _comparer)
	{
		if(_comparer.Equals(_object,_replace))
		{
			return false;
		}

		_object = _replace;

		return true;
	}

	public static bool CompareExchange<TObject>(ref TObject _object,TObject _expected,TObject _replace)
	{
		return CompareExchange(ref _object,_expected,_replace,EqualityComparer<TObject>.Default);
	}
	
	public static bool CompareExchange<TObject>(ref TObject _object,TObject _expected,TObject _replace,IEqualityComparer<TObject> _comparer)
	{
		if(!_comparer.Equals(_object,_expected))
		{
			return false;
		}

		_object = _replace;

		return true;
	}
}