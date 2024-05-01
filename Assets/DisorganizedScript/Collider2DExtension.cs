using System;
using UnityEngine;

public static class Collider2DExtension
{
	private static readonly Collider2D[] s_Collider2DArray = new Collider2D[1];
	private static readonly RaycastHit2D[] s_Raycast2DArray = new RaycastHit2D[1];
	private static readonly RaycastHit2D[] s_CachedRaycast2DArray = new RaycastHit2D[32];

	public static bool IsOverlapping(this Collider2D _collider,Collider2D _collider2)
	{
		return _collider.Distance(_collider2).isOverlapped;
	}
	
	public static bool IsTouchingNow(this Collider2D _collider,Collider2D _collider2)
	{
		return _collider.Distance(_collider2).distance <= 0;
	}

	public static bool IsOverlapping(this Collider2D _collider,LayerMask _layerMask,Vector2? _offset = null)
	{
		return IsOverlapping(_collider,_layerMask,default,_offset);
	}

	public static bool IsOverlapping(this Collider2D _collider,LayerMask _layerMask,ContactFilter2D _contactFilter,Vector2? _offset = null)
	{
		_collider.offset = _offset.HasValue ? _collider.offset+_offset.Value : _collider.offset;
		_contactFilter.SetLayerMask(_layerMask | _contactFilter.layerMask);

		var count = _collider.OverlapCollider(_contactFilter,s_Collider2DArray);

		Array.Clear(s_Collider2DArray,0,count);

		return count > 0;
	}
	
	public static bool IsCastOverlapping(this Collider2D _collider,LayerMask _layerMask,Vector2 _offset)
	{
		return IsCastOverlapping(_collider,_layerMask,_offset,default);
	}
	
	public static bool IsCastOverlapping(this Collider2D _collider,LayerMask _layerMask,Vector2 _offset,ContactFilter2D _contactFilter)
	{
		_contactFilter.SetLayerMask(_layerMask | _contactFilter.layerMask);

		var distance = CommonUtility.GetOffsetDistance(ref _offset);
		var buffer = s_Raycast2DArray;
		var count = _collider.Cast(_offset, _contactFilter,buffer,distance);
		var overlapping = buffer[0];

		Array.Clear(s_Raycast2DArray,0,count);

		return overlapping;
	}

	public static bool IsOverlapping(this Collider2D _collider1,LayerMask _layerMask,out Collider2D _collider2,Vector2? _offset = null)
	{
		return IsOverlapping(_collider1,_layerMask,default(ContactFilter2D),out _collider2);
	}
	
	public static bool IsOverlapping(this Collider2D _collider1,LayerMask _layerMask,ContactFilter2D _contactFilter,out Collider2D _collider2,Vector2? _offset = null)
	{
		_collider1.offset = _offset.HasValue ? _collider1.offset+_offset.Value : _collider1.offset;
		_contactFilter.SetLayerMask(_layerMask | _contactFilter.layerMask);

		var count = _collider1.OverlapCollider(_contactFilter,s_Collider2DArray);
		_collider2 = s_Collider2DArray[0];

		Array.Clear(s_Collider2DArray,0,count);

		return count > 0;
	}

	public static bool IsCastOverlapping(this Collider2D _collider1,LayerMask _layerMask,Vector2 _offset,out Collider2D _collider2)
	{
		return IsCastOverlapping(_collider1,_layerMask,_offset,default,out _collider2);
	}
	
	static public bool IsCastOverlapping(this Collider2D _collider1,LayerMask _layerMask,Vector2 _offset,ContactFilter2D _contactFilter,out Collider2D _collider2)
	{
		_contactFilter.SetLayerMask(_layerMask | _contactFilter.layerMask);

		var distance = CommonUtility.GetOffsetDistance(ref _offset);
		var buffer = s_CachedRaycast2DArray;
		var count = _collider1.Cast(_offset,_contactFilter,buffer,distance);

		_collider2 = count > 0 ? CommonUtility.ClosestHit(buffer,count).collider : null;

		Array.Clear(s_CachedRaycast2DArray,0,count);

		return _collider2 != null;
	}
	
	public static bool IsCastOverlapping(this Collider2D _collider,LayerMask _layerMask,Vector2 _offset,out RaycastHit2D _raycast)
	{
		return IsCastOverlapping(_collider,_layerMask,_offset,default,out _raycast);
	}
	
	public static bool IsCastOverlapping(this Collider2D _collider, LayerMask _layerMask, Vector2 _offset,ContactFilter2D _contactFilter,out RaycastHit2D _raycast)
	{
		_contactFilter.SetLayerMask(_layerMask | _contactFilter.layerMask);

		var distance = CommonUtility.GetOffsetDistance(ref _offset);
		var buffer = s_CachedRaycast2DArray;
		int count = _collider.Cast(_offset,_contactFilter,buffer,distance);

		_raycast = count > 0 ? CommonUtility.ClosestHit(buffer,count) : default;

		Array.Clear(s_CachedRaycast2DArray,0,count);

		return _raycast;
	}
}