using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public static partial class TransformExtension
{
	/// <summary>
	/// 자식 추가
	/// </summary>
	public static Transform AddChild(this Transform _origin,string _name)
	{
		var child = new GameObject(_name);

		_origin.SetChild(child.transform);

		return child.transform;
	}

	public static Transform[] AddChildren(this Transform _origin,string[] _nameArray)
	{
		var dataArray = new Transform[_nameArray.Length];

		for(var i=0;i<_nameArray.Length;i++)
		{
			dataArray[i] = AddChild(_origin,_nameArray[i]);
		}

		return dataArray;
	}

	public static Transform[] AddChildren(this Transform _origin,string _name,int _count)
	{
		var dataArray = new Transform[_count];

		for(var i=0;i<_count;i++)
		{
			dataArray[i] = AddChild(_origin,string.Format("{0}_{1}",_name,i));
		}

		return dataArray;
	}

	public static void SetChild(this Transform _origin,Transform _child,bool _sameLayer = true)
	{
		_origin.SetChildInside(_child,true,_sameLayer);
	}

	public static void SetUIChild(this Transform _origin,Transform _child,bool _sameLayer = true)
	{
		_origin.SetChildInside(_child,false,_sameLayer);
	}

	private static void SetChildInside(this Transform _origin,Transform _child,bool _stays,bool _sameLayer = true)
	{
		_child.SetParent(_origin,_stays);

		if(_sameLayer)
		{
			_child.gameObject.layer = _origin.gameObject.layer;
		}
	}

	/// <summary>
	/// prefab로부터 Instance를 만들고 Parent의 자식으로 만든다.
	/// 자식의 transform값이 변할 수 있다. (월드상의 현재 위치와 스케일을 유지하기 위해 부모에 대한 상대 값들로 변경된다.)
	/// 자식은 부모의 레이어와 동일하게 설정한다.
	/// </summary>
	public static Transform AddChild(this Transform _origin,GameObject _prefab)
	{
		var child = CommonUtility.CopyObject(_prefab);

		_origin.SetChildInside(child.transform,true);

		return child.transform;
	}

	public static Transform[] AddChildren(this Transform _origin,GameObject _prefab,int _count)
	{
		var dataArray = new Transform[_count];

		for(var i=0;i<_count;i++)
		{
			dataArray[i] = AddChild(_origin,_prefab);
		}

		return dataArray;
	}

	/// <summary>
	/// prefab로부터 Instance를 만들고 Parent의 자식으로 만든다.
	/// 자식의 transform값을 유지한다.
	/// 자식은 부모의 레이어와 동일하게 설정한다.
	/// </summary>
	public static Transform AddUIChild(this Transform _origin,GameObject _prefab)
	{
		var child = CommonUtility.CopyObject(_prefab);

		_origin.SetChildInside(child.transform,false);

		return child.transform;
	}

	public static Transform[] AddUIChildren(this Transform _origin,GameObject _prefab,int _count)
	{
		var dataArray = new Transform[_count];

		for(var i=0;i<_count;i++)
		{
			dataArray[i] = AddUIChild(_origin,_prefab);
		}

		return dataArray;
	}

	/// <summary>
	/// 자식에서 찾기
	/// </summary>
	public static Transform FindInChild(this Transform _transform,string _name)
	{
		if(_transform.name.IsEqual(_name))
		{
			return _transform;
		}

		for(var i=0;i<_transform.childCount;i++)
		{
			var result = FindInChild(_transform.GetChild(i),_name);

			if(result)
			{
				return result;
			}
		}

		return null;
	}

	/// <summary>
	/// 자식에서 모두 찾기
	/// </summary>
	public static void FindAllChildren(this Transform _transform,string _text,ref List<Transform> _resultList)
	{
		for(var i=0;i<_transform.childCount;i++)
		{
			var child = _transform.GetChild(i);

			child.FindAllChildren(_text,ref _resultList);

			if(child.name.IsEqual(_text))
			{
				_resultList.Add(child);
			}
		}
	}

	// public static Transform GetChild(this Transform _origin,string _name)
	// {
	// 	if(!_origin)
	// 	{
	// 		return null;
	// 	}

	// 	return _origin.Find(_name);
	// }

	public static void GetAllChildrenInHierarchy(this Transform _transform,ref List<Transform> _resultList)
	{
		for(var i=0;i<_transform.childCount;i++)
		{
			var child = _transform.GetChild(i);

			_resultList.Add(child);
			child.GetAllChildrenInHierarchy(ref _resultList);
		}
	}

	// public static Transform DestroyChildren(this Transform _transform,bool _activeOnly = false)
	// {
	// 	if(_transform.childCount == 0)
	// 	{
	// 		return _transform;
	// 	}

	// 	for(var i=0;i<_transform.childCount;i++)
	// 	{
	// 		var child = _transform.GetChild(i);

	// 		if(!child || (_activeOnly && !child.gameObject.activeSelf))
	// 		{
	// 			continue;
	// 		}

	// 		child.SetParent(null);

	// 		CommonUtility.DestroyObject(child.gameObject);
	// 	}

	// 	return _transform;
	// }

	public static Transform DestroyChildren(this Transform _transform,params Transform[] _exceptionArray)
	{
		var count = _transform.childCount;

		if(count == 0)
		{
			return _transform;
		}

		for(var i=count-1;i>=0;i--)
		{
			var child = _transform.GetChild(i);

			if(!child || _exceptionArray.Contains(child))
			{
				continue;
			}

			CommonUtility.DestroyObject(child.gameObject);
		}

		return _transform;
	}

	/// <summary>
	/// 순회하면서 onAction을 실행한다.
	/// </summary>
	public static void TraverseChildren(this Transform _parent,Action<Transform> _onAction)
	{
		if(!_parent)
		{
			return;
		}
		
		_onAction?.Invoke(_parent);
		
		for(var i=0;i<_parent.childCount;i++)
		{
			var child = _parent.GetChild(i);

			if(!child)
			{
				continue;
			}

			child.TraverseChildren(_onAction);
		}
	}
}