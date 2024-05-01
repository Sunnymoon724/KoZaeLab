using System;
using UnityEngine;

public static class Rigidbody2DExtension
{
	private static readonly Collider2D[] s_Collider2DArray = new Collider2D[1];
	private static readonly RaycastHit2D[] s_Raycast2DArray = new RaycastHit2D[1];
	private static readonly RaycastHit2D[] s_CachedRaycast2DArray = new RaycastHit2D[32];

	public static bool IsOverlapping(this Rigidbody2D _rigid,Collider2D _collider)
	{
		return _rigid.Distance(_collider).isOverlapped;
	}
	
	public static bool IsTouchingNow(this Rigidbody2D _rigid,Collider2D _collider)
	{
		return _rigid.Distance(_collider).distance <= 0;
	}

	public static bool IsOverlapping(this Rigidbody2D _rigid,LayerMask _layerMask,Vector2? _offset = null)
	{
		return IsOverlapping(_rigid,_layerMask,default,_offset);
	}

	public static bool IsOverlapping(this Rigidbody2D _rigid,LayerMask _layerMask,ContactFilter2D _contactFilter,Vector2? _offset = null)
	{
		_rigid.position = _offset.HasValue ? _rigid.position+_offset.Value : _rigid.position;
		_contactFilter.SetLayerMask(_layerMask | _contactFilter.layerMask);

		int count = _rigid.OverlapCollider(_contactFilter,s_Collider2DArray);

		Array.Clear(s_Collider2DArray,0,count);

		return count > 0;
	}
	
	public static bool IsCastOverlapping(this Rigidbody2D _rigid,LayerMask _layerMask,Vector2 _offset)
	{
		return IsCastOverlapping(_rigid,_layerMask,_offset,default);
	}
	
	public static bool IsCastOverlapping(this Rigidbody2D _rigid,LayerMask _layerMask,Vector2 _offset,ContactFilter2D _contactFilter)
	{
		_contactFilter.SetLayerMask(_layerMask | _contactFilter.layerMask);

		var distance = CommonUtility.GetOffsetDistance(ref _offset);
		var buffer = s_Raycast2DArray;
		var count = _rigid.Cast(_offset,_contactFilter,buffer,distance);
		var overlapping = buffer[0];

		Array.Clear(s_Raycast2DArray,0,count);

		return overlapping;
	}

	public static bool IsOverlapping(this Rigidbody2D _rigid,LayerMask _layerMask,out Collider2D _collider,Vector2? _offset = null)
	{
		return IsOverlapping(_rigid,_layerMask,default,out _collider,_offset);
	}

	public static bool IsOverlapping(this Rigidbody2D _rigid,LayerMask _layerMask,ContactFilter2D _contactFilter,out Collider2D _collider,Vector2? _offset = null)
	{
		_rigid.position = _offset.HasValue ? _rigid.position+_offset.Value : _rigid.position;
		_contactFilter.SetLayerMask(_layerMask | _contactFilter.layerMask);

		var count = _rigid.OverlapCollider(_contactFilter,s_Collider2DArray);
		_collider = s_Collider2DArray[0];

		Array.Clear(s_Collider2DArray,0,count);

		return count > 0;
	}

	public static bool IsCastOverlapping(this Rigidbody2D _rigid,LayerMask _layerMask,Vector2 _offset,out Collider2D _collider)
	{
		return IsCastOverlapping(_rigid,_layerMask,_offset,default,out _collider);
	}
	
	public static bool IsCastOverlapping(this Rigidbody2D _rigid,LayerMask _layerMask,Vector2 _offset,ContactFilter2D _contactFilter,out Collider2D _collider)
	{
		_contactFilter.SetLayerMask(_layerMask | _contactFilter.layerMask);

		var distance = CommonUtility.GetOffsetDistance(ref _offset);
		var buffer = s_CachedRaycast2DArray;
		var count = _rigid.Cast(_offset,_contactFilter,buffer,distance);

		_collider = count > 0 ? CommonUtility.ClosestHit(buffer,count).collider : null;

		Array.Clear(s_CachedRaycast2DArray,0,count);

		return _collider != null;
	}
	
	public static bool IsCastOverlapping(this Rigidbody2D _rigid,LayerMask _layerMask,Vector2 _offset,out RaycastHit2D _hit)
	{
		return IsCastOverlapping(_rigid,_layerMask,_offset,default,out _hit);
	}
	
	public static bool IsCastOverlapping(this Rigidbody2D _rigid,LayerMask _layerMask,Vector2 _offset,ContactFilter2D _contactFilter,out RaycastHit2D _hit)
	{
		_contactFilter.SetLayerMask(_layerMask | _contactFilter.layerMask);

		var distance = CommonUtility.GetOffsetDistance(ref _offset);
		var buffer = s_CachedRaycast2DArray;
		int count = _rigid.Cast(_offset,_contactFilter,buffer,distance);

		_hit = count > 0 ? CommonUtility.ClosestHit(buffer,count) : default;

		Array.Clear(s_CachedRaycast2DArray,0,count);

		return _hit;
	}

	public static void ChangeDirection(this Rigidbody2D _rigid,Vector2 _direction)
	{
		_rigid.velocity = _direction*_rigid.velocity.magnitude;
	}
}