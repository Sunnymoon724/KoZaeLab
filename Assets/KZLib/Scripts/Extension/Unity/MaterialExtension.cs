using UnityEngine;

public static class MaterialExtension
{
	/// <summary>
    /// 알파 값을 설정합니다.
    /// </summary>
	public static Material SetAlpha(this Material _material,float _alpha)
	{
		if(!_material)
		{
			return null;
		}

		_material.color = _material.color.MaskAlpha(_alpha);

		return _material;
	}

	/// <summary>
    /// Material의 색상을 설정합니다.
    /// </summary>
	public static Material Color(this Material _material,Color _color)
	{
		if(!_material)
		{
			return null;
		}

		_material.color = _color;

		return _material;
	}

	/// <summary>
    /// Material의 색상을 설정합니다. (NameId 방식)
    /// </summary>
	public static Material Color(this Material _material,int _nameId,Color _color)
	{
		if(!_material)
		{
			return null;
		}

		_material.SetColor(_nameId,_color);

		return _material;
	}

	/// <summary>
    /// Material의 색상을 설정합니다. (String 이름 방식)
    /// </summary>
	public static Material Color(this Material _material,string _name,Color _color)
	{
		if(!_material)
		{
			return null;
		}

		_material.SetColor(_name,_color);

		return _material;
	}

	/// <summary>
    /// Material의 Float 값을 설정합니다. (NameId 방식)
    /// </summary>
	public static Material Float(this Material _material,int _nameId,float _value)
	{
		if(!_material)
		{
			return null;
		}

		_material.SetFloat(_nameId,_value);

		return _material;
	}

	/// <summary>
    /// Material의 Float 값을 설정합니다. (String 이름 방식)
    /// </summary>
	public static Material Float(this Material _material,string _name,float _value)
	{
		if(!_material)
		{
			return null;
		}

		_material.SetFloat(_name,_value);

		return _material;
	}

	/// <summary>
    /// Material의 키워드를 설정합니다.
    /// </summary>
	public static Material Keyword(this Material _material,string _keyword,bool _enable)
	{
		if(!_material)
		{
			return null;
		}

		if(_enable)
		{
			_material.EnableKeyword(_keyword);
		}
		else
		{
			_material.DisableKeyword(_keyword);
		}

		return _material;
	}
}