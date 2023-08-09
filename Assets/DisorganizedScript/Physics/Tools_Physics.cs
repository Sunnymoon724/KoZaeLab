using UnityEngine;

public static partial class Tools
{
	public static RaycastHit ClosestHit(RaycastHit[] _hitArray,int _length)
	{
		var distance = float.MaxValue;
		var closest = default(RaycastHit);
		
		for(var i=0;i<_length;i++)
		{
			var checking = _hitArray[i];

			if(checking.distance < distance)
			{
				distance = checking.distance;
				closest = checking;
			}
		}

		return closest;
	}

	public static RaycastHit ClosestHitNonZero(RaycastHit[] _hitArray,int _length)
	{
		var distance = float.MaxValue;
		var closest = default(RaycastHit);
		
		for(var i=0;i<_length;i++)
		{
			var checking = _hitArray[i];

			if(checking.distance > 0 && checking.distance < distance)
			{
				distance = checking.distance;
				closest = checking;
			}
		}

		return closest;
	}

	public static RaycastHit2D ClosestHit(RaycastHit2D[] _hit2DArray,int _length)
	{
		var distance = float.MaxValue;
		var closest = default(RaycastHit2D);

		for(var i=0;i<_length;i++)
		{
			var checking = _hit2DArray[i];

			if(checking.distance < distance)
			{
				distance = checking.distance;
				closest = checking;
			}
		}

		return closest;
	}

	public static RaycastHit2D ClosestHitNonZero(RaycastHit2D[] _hit2DArray,int _length)
	{
		var distance = float.MaxValue;
		var closest = default(RaycastHit2D);

		for(var i=0;i<_length;i++)
		{
			var checking = _hit2DArray[i];

			if(checking.distance > 0 && checking.distance < distance)
			{
				distance = checking.distance;
				closest = checking;
			}
		}

		return closest;
	}

	public static float GetOffsetDistance(ref Vector2 _offset)
	{
		float distance = _offset.magnitude;
		_offset.x /= distance;
		_offset.y /= distance;

		return distance;
	}
	
	public static bool CheckCollider2D<T>(RaycastHit2D[] _raycastArray,int _length,T _filter,out RaycastHit2D _raycast) where T : Collider2D
	{		
		for(var i=_length-1;i>=0;i--)
		{
			if(_filter.Equals(_raycastArray[i].collider))
			{
				_raycast = _raycastArray[i];

				return true;
			}
		}

		_raycast = default;

		return false;
	}
	
	public static bool CheckGameObject<T>(RaycastHit2D[] _raycastArray,int _length,T _filter,out RaycastHit2D _raycast) where T : Object
	{
		for(var i=_length-1;i>=0;i--)
		{
			if(_filter.Equals(_raycastArray[i].collider.gameObject))
			{
				_raycast = _raycastArray[i];

				return true;
			}
		}

		_raycast = default;

		return false;
	}

	static public int FilterCollider2D<T>(Collider2D[] _colliderArray,int _length,T _filter) where T : Collider2D
	{
		for(var i=_length-1;i>=0;i--)
		{
			if(!_filter.Equals(_colliderArray[i]) && _colliderArray.RemoveSafe(i))
			{
				_length--;
			}
				
		}

		return _length;
	}

	public static int FilterGameObject<T>(Collider2D[] _colliderArray,int _length,T _filter) where T : Object
	{
		for(var i=_length-1;i>=0;i--)
		{
			if(!_filter.Equals(_colliderArray[i].gameObject) && _colliderArray.RemoveSafe(i))
			{
				_length--;
			}
		}

		return _length;
	}

	public static int FilterCollider2D<T>(RaycastHit2D[] _raycastArray,int _length,T _filter) where T : Collider2D
	{
		for(var i=_length-1;i>=0;i--)
		{
			if(!_filter.Equals(_raycastArray[i].collider) && _raycastArray.RemoveSafe(i))
			{
				_length--;
			}
		}

		return _length;
	}
	static public int FilterGameObject<T>(RaycastHit2D[] _raycastArray,int _length,T _filter) where T : Object
	{
		for(var i=_length-1;i>=0;i--)
		{
			if(!_filter.Equals(_raycastArray[i].collider.gameObject) && _raycastArray.RemoveSafe(i))
			{
				_length--;
			}
		}

		return _length;
	}
}