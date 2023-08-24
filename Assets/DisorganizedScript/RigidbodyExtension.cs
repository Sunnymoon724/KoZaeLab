using System;
using UnityEngine;

public static class RigidbodyExtension
{
	public static void ChangeDirection(this Rigidbody _rigid,Vector3 _direction)
	{
		_rigid.velocity = _direction*_rigid.velocity.magnitude;
	}
}