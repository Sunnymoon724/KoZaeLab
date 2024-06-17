using System.Text;
using UnityEngine;

public static partial class TransformExtension
{
	/// <summary>
	/// 현재 transform 부터 root 까지 full path 반환.
	/// </summary>
	public static string GetHierarchy(this Transform _transform,StringBuilder _builder = null)
	{
		if(_builder == null)
		{
			_builder = new StringBuilder();
			_builder.Append(_transform.name);
		}
		else
		{
			_builder.Insert(0,string.Format("{0}/",_transform.name));
		}

		if(!_transform.parent)
		{
			return _builder.ToString();
		}

		return GetHierarchy(_transform.parent,_builder);
	}

	/// <summary>
	/// 현재 transform 부터 root 까지 0부터 차례대로 index 반환.
	/// </summary>
	public static string GetHierarchyInOrder(this Transform _transform,StringBuilder _builder = null)
	{
		if(_builder == null)
		{
			_builder = new StringBuilder();

			_builder.Append(string.Format("{0}",_transform.GetSiblingIndex()));
		}
		else
		{
			_builder.Insert(0,string.Format("{0}/",_transform.GetSiblingIndex()));
		}

		if(!_transform.parent)
		{
			return _builder.ToString();
		}

		return GetHierarchyInOrder(_transform.parent,_builder);
	}

	/// <summary>
	/// 루트에서 찾기
	/// </summary>
	public static Transform FindFromRoot(this Transform _transform,string _name)
	{
		return _transform.root.Find(_name);
	}

	/// <summary>
	/// 같은 계층에서 찾기
	/// </summary>
	public static Transform FindSibling(this Transform _transform,string _name)
	{
		if(_transform.parent)
		{
			return _transform.parent.Find(_name);
		}

		return null;
	}
}