using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static partial class TransformExtension
{
	/// <summary>
	/// Get current transform to root ( AAA/BBB )
	/// </summary>
	public static string GetHierarchy(this Transform _origin)
	{
		var builder = new StringBuilder(_origin.name);

		while(_origin.parent)
		{
			builder.Insert(0, '/');
			builder.Insert(0, _origin.parent.name);

			_origin = _origin.parent;
		}

		return builder.ToString();
	}

	/// <summary>
	/// Get current transform to root ( 0/1/2/3 )
	/// </summary>
	public static string GetHierarchyInOrder(this Transform _origin)
	{
		var builder = new StringBuilder();

		for(var current = _origin;current != null;current = current.parent)
		{
			builder.Insert(0,current.GetSiblingIndex());

			if(current.parent)
			{
				builder.Insert(0,'/');
			}
		}

		return builder.ToString();
	}

	public static Transform FindFromRoot(this Transform _origin,string _name)
	{
		return _origin.root.Find(_name);
	}

	public static Transform FindSibling(this Transform _origin,string _name)
	{
		return _origin.parent ? _origin.parent.Find(_name) : null;
	}

	public static Transform FindInParentHierarchy(this Transform _origin,string _name)
	{
		for(var current = _origin.parent;current != null;current = current.parent)
		{
			if(current.name.IsEqual(_name))
			{
				return current;
			}
		}

		return null;
	}

	public static Transform GetParent(this Transform _origin)
	{
		return _origin ? _origin.parent : null;
	}

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
		var _nameArray = new string[_count];

		for(var i=0;i<_count;i++)
		{
			_nameArray[i] = $"{_name}_{i}";
		}

		return AddChildren(_origin,_nameArray);
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

	public static Transform AddChild(this Transform _origin,GameObject _prefab)
	{
		var child = UnityUtility.CopyObject(_prefab);

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

	public static Transform AddUIChild(this Transform _origin,GameObject _prefab)
	{
		var child = UnityUtility.CopyObject(_prefab);

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

	public static Transform FindInChild(this Transform _transform,string _name)
	{
		var queue = new Queue<Transform>();
		queue.Enqueue(_transform);

		while(queue.Count > 0)
		{
			var current = queue.Dequeue();

			if(current.name.IsEqual(_name))
			{
				return current;
			}

			for(var i=0;i<current.childCount;i++)
			{
				queue.Enqueue(current.GetChild(i));
			}
		}

		return null;
	}

	public static void FindAllChildren(this Transform _transform,string _text,ref List<Transform> _resultList)
	{
		var queue = new Queue<Transform>();
		queue.Enqueue(_transform);

		while(queue.Count > 0)
		{
			var current = queue.Dequeue();

			if(current.name.IsEqual(_text))
			{
				_resultList.Add(current);
			}

			for(var i=0;i<current.childCount;i++)
			{
				queue.Enqueue(current.GetChild(i));
			}
		}
	}

	public static Transform GetChild(this Transform _origin,string _name)
	{
		return _origin ? _origin.Find(_name) : null;
	}

	/// <summary>
	/// Get All Children In Hierarchy
	/// </summary>
	public static void GetAllChildrenInHierarchy(this Transform _transform,ref List<Transform> _resultList)
	{
		var queue = new Queue<Transform>();
		queue.Enqueue(_transform);

		while(queue.Count > 0)
		{
			var current = queue.Dequeue();

			_resultList.Add(current);

			for(var i=0;i<current.childCount;i++)
			{
				queue.Enqueue(current.GetChild(i));
			}
		}
	}

	public static Transform DestroyChildren(this Transform _transform,bool _activeOnly = false)
	{
		var count = _transform.childCount;

		if(count == 0)
		{
			return _transform;
		}

		for(var i=count-1;i>=0;i--)
		{
			var child = _transform.GetChild(i);

			if(_activeOnly && !child.gameObject.activeSelf)
			{
				continue;
			}

			child.SetParent(null);

			UnityUtility.DestroyObject(child.gameObject);
		}

		return _transform;
	}

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
			var flag = false;

			foreach(var exception in _exceptionArray)
			{
				if(child == exception)
				{
					flag = true;

					break;
				}
			}

			if(flag)
			{
				continue;
			}

			UnityUtility.DestroyObject(child.gameObject);
		}

		return _transform;
	}

	public static void TraverseChildren(this Transform _parent,Action<Transform> _onAction)
	{
		if(!_parent)
		{
			return;
		}

		var stack = new Stack<Transform>();
		var current = _parent;

		do
		{
			_onAction?.Invoke(current);

			var count = current.childCount;

			for(var i=count-1;i>=0;i--)
			{
				stack.Push(current.GetChild(i));
			}

			if(stack.Count > 0)
			{
				current = stack.Pop();
			}
			else
			{
				break;
			}
		}
		while(true);
	}
}