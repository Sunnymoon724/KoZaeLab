using UnityEngine;

public static partial class CommonUtility
{
	public static float GradientToRadian(float _x,float _y)
	{
		return GradientToRadian(new Vector2(_x,_y));
	}

	public static float GradientToRadian(Vector2 _gradient)
	{
		return Mathf.Atan2(_gradient.y,_gradient.x);
	}

	public static Vector2 RadianToGradient(float _angle)
	{
		return new Vector2(Mathf.Cos(_angle),Mathf.Sin(_angle));
	}

	public static float GradientToToDegree(float _x,float _y)
	{
		return GradientToToDegree(new Vector2(_x,_y));
	}

	public static float GradientToToDegree(Vector2 _gradient)
	{
		return GradientToRadian(_gradient)*Mathf.Rad2Deg;
	}

	public static Vector2 DegreeToGradient(float _angle)
	{
		return new Vector2(Mathf.Cos(_angle*Mathf.Deg2Rad),Mathf.Sin(_angle*Mathf.Deg2Rad));
	}

	public static Vector2 Rotate(Vector2 _vector,float _radians)
	{
		float sin = Mathf.Sin(_radians);
		float cos = Mathf.Cos(_radians);

		return new Vector2(Mathf.Abs(_vector.x*cos)+Mathf.Abs(_vector.y*sin),Mathf.Abs(_vector.x*sin)+Mathf.Abs(_vector.y*cos));
	}

	public static float RotateTowards(float _from,float _to,float _maxAngle)
	{
		var angle = (_to-_from).ToWrapAngle();

		if(Mathf.Abs(angle) > _maxAngle)
		{
			angle = _maxAngle*Mathf.Sign(angle);
		}

		return _from+angle;
	}
}