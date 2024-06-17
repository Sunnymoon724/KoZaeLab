using UnityEngine;

public static partial class TransformExtension
{
	/// <summary>
	/// 부모에서 찾기
	/// </summary>
	public static Transform FindInParent(this Transform _transform,string _name)
	{
		if(_transform.name.IsEqual(_name))
		{
			return _transform;
		}

		if(_transform.parent)
		{
			return FindInParent(_transform.parent,_name);
		}

		return null;
	}

	public static Transform GetParent(this Transform _origin)
	{
		if(!_origin)
		{
			return null;
		}

		return _origin.parent;
	}
}