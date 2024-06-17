using UnityEngine;

public static class MaterialExtension
{
	public static Material SetAlpha(this Material _material,float _alpha)
	{
		_material.color = _material.color.MaskAlpha(_alpha);

		return _material;
	}

	public static Material Color(this Material _material,Color _color)
	{
		_material.color = _color;
		
		return _material;
	}

	public static Material Color(this Material _material,int _nameId,Color _color)
	{
		_material.SetColor(_nameId,_color);

		return _material;
	}

	public static Material Color(this Material _material,string _name,Color _color)
	{
		_material.SetColor(_name,_color);

		return _material;
	}

	public static Material Float(this Material _material,int _nameId,float _value)
	{
		_material.SetFloat(_nameId,_value);

		return _material;
	}

	public static Material Float(this Material _material,string _name,float _value)
	{
		_material.SetFloat(_name,_value);

		return _material;
	}

	public static Material Keyword(this Material _material,string _keyword,bool _enable)
	{
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