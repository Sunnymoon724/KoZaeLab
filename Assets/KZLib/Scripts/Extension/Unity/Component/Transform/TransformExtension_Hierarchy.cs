using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static partial class TransformExtension
{
	/// <summary>
	/// AAA/BBB
	/// </summary>
	public static string FindHierarchy(this Transform origin)
	{
		if(!IsValid(origin))
		{
			return string.Empty;
		}

		var stringBuilder = new StringBuilder();
		var nameStack = new Stack<string>();

		while(origin)
		{
			nameStack.Push(origin.name);
			origin = origin.parent;
		}

		while(nameStack.Count > 0)
		{
			stringBuilder.Append("/").Append(nameStack.Pop());
		}

		return stringBuilder.ToString();
	}

	public static Transform FindFromRoot(this Transform origin,string name)
	{
		if(!IsValid(origin))
		{
			return null;
		}

		return origin.root.Find(name);
	}

	public static Transform FindSibling(this Transform origin,string name)
	{
		if(!IsValid(origin))
		{
			return null;
		}

		return origin.parent ? origin.parent.Find(name) : null;
	}

	public static Transform FindInParentHierarchy(this Transform origin,string name)
	{
		if(!IsValid(origin))
		{
			return null;
		}

		for(var current = origin.parent;current != null;current = current.parent)
		{
			if(current.name.IsEqual(name))
			{
				return current;
			}
		}

		return null;
	}

	public static Transform GetParent(this Transform origin)
	{
		if(!IsValid(origin))
		{
			return null;
		}

		return origin ? origin.parent : null;
	}

	public static Transform AddChild(this Transform origin,string name)
	{
		if(!IsValid(origin))
		{
			return null;
		}

		var child = new GameObject(name);

		origin.SetChild(child.transform);

		return child.transform;
	}

	public static Transform[] AddChildren(this Transform origin,string[] nameArray)
	{
		if(!IsValid(origin))
		{
			return null;
		}

		var dataArray = new Transform[nameArray.Length];

		for(var i=0;i<nameArray.Length;i++)
		{
			dataArray[i] = AddChild(origin,nameArray[i]);
		}

		return dataArray;
	}

	public static Transform[] AddChildren(this Transform origin,string name,int count)
	{
		if(!IsValid(origin))
		{
			return null;
		}

		var nameArray = new string[count];

		for(var i=0;i<count;i++)
		{
			nameArray[i] = $"{name}_{i}";
		}

		return AddChildren(origin,nameArray);
	}

	public static void SetChild(this Transform origin,Transform child,bool isSameLayer = true)
	{
		if(!IsValid(origin))
		{
			return;
		}

		_SetChild(origin,child,true,isSameLayer);
	}

	public static void SetUIChild(this Transform origin,Transform child,bool isSameLayer = true)
	{
		if(!IsValid(origin))
		{
			return;
		}

		_SetChild(origin,child,false,isSameLayer);
	}

	private static void _SetChild(Transform origin,Transform child,bool worldPositionStays,bool isSameLayer)
	{
		if(!IsValid(origin))
		{
			return;
		}

		child.SetParent(origin,worldPositionStays);

		if(isSameLayer)
		{
			child.gameObject.layer = origin.gameObject.layer;
		}
	}

	private static Transform AddChildInside(Transform origin,GameObject prefab,bool worldPositionStays,bool isSameLayer)
	{
		if(!IsValid(origin))
		{
			return null;
		}

		var child = prefab.CopyObject() as GameObject;

		_SetChild(origin,child.transform,worldPositionStays,isSameLayer);

		return child.transform;
	}

	public static Transform AddChild(this Transform origin,GameObject prefab,bool isSameLayer = true)
	{
		if(!IsValid(origin))
		{
			return null;
		}

		return AddChildInside(origin,prefab,true,isSameLayer);
	}

	public static Transform[] AddChildren(this Transform origin,GameObject prefab,int count,bool isSameLayer = true)
	{
		if(!IsValid(origin))
		{
			return null;
		}

		var dataArray = new Transform[count];

		for(var i=0;i<count;i++)
		{
			dataArray[i] = AddChild(origin,prefab,isSameLayer);
		}

		return dataArray;
	}

	public static Transform AddUIChild(this Transform origin,GameObject prefab,bool isSameLayer = true)
	{
		if(!IsValid(origin))
		{
			return null;
		}

		return AddChildInside(origin,prefab,false,isSameLayer);
	}

	public static Transform[] AddUIChildren(this Transform origin,GameObject prefab,int count,bool isSameLayer = true)
	{
		if(!IsValid(origin))
		{
			return null;
		}

		var dataArray = new Transform[count];

		for(var i=0;i<count;i++)
		{
			dataArray[i] = AddUIChild(origin,prefab,isSameLayer);
		}

		return dataArray;
	}

	public static Transform FindChild(this Transform origin,string name)
	{
		if(!IsValid(origin))
		{
			return null;
		}

		var queue = new Queue<Transform>();
		queue.Enqueue(origin);

		while(queue.Count > 0)
		{
			var current = queue.Dequeue();

			if(current.name.IsEqual(name))
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

	public static IEnumerable<Transform> FindChildGroup(this Transform origin,string name = null)
	{
		if(!IsValid(origin))
		{
			yield break;
		}

		var queue = new Queue<Transform>();
		queue.Enqueue(origin);

		while(queue.Count > 0)
		{
			var current = queue.Dequeue();

			if(name.IsEmpty() || current.name.IsEqual(name))
			{
				yield return current;
			}

			for(var i=0;i<current.childCount;i++)
			{
				queue.Enqueue(current.GetChild(i));
			}
		}
	}

	public static Transform GetChild(this Transform origin,string name)
	{
		if(!IsValid(origin))
		{
			return null;
		}

		return origin.Find(name);
	}

	public static Transform DestroyChildren(this Transform origin,bool isActiveOnly = false,params Transform[] exceptionArray)
	{
		if(!IsValid(origin))
		{
			return null;
		}

		var count = origin.childCount;

		if(count == 0)
		{
			return origin;
		}

		var exceptionSet = new HashSet<Transform>(exceptionArray);

		for(var i=count-1;i>=0;i--)
		{
			var child = origin.GetChild(i);

			if(isActiveOnly && !child.gameObject.activeSelf)
			{
				continue;
			}

			if(exceptionSet.Contains(child))
			{
				continue;
			}

			child.SetParent(null);
			child.gameObject.DestroyObject();
		}

		return origin;
	}

	public static void TraverseChildren(this Transform origin,Action<Transform> onAction)
	{
		if(!IsValid(origin))
		{
			return;
		}

		var queue = new Queue<Transform>();
		queue.Enqueue(origin);

		while(queue.Count > 0)
		{
			var current = queue.Dequeue();

			onAction?.Invoke(current);

			for(var i=0;i<current.childCount;i++)
			{
				queue.Enqueue(current.GetChild(i));
			}
		}
	}
}