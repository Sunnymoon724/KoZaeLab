using UnityEngine;

public static class Matrix4x4Extension
{
	public static Quaternion ExtractRotation(this Matrix4x4 _matrix)
	{
		return Quaternion.LookRotation(new Vector3(_matrix.m02,_matrix.m12,_matrix.m22),new Vector3(_matrix.m01,_matrix.m11,_matrix.m21));
	}

	public static Vector3 ExtractPosition(this Matrix4x4 _matrix)
	{
		return new Vector3(_matrix.m03,_matrix.m13,_matrix.m23);
	}

	public static Vector3 ExtractScale(this Matrix4x4 _matrix)
	{
		return new Vector3(new Vector4(_matrix.m00,_matrix.m10,_matrix.m20,_matrix.m30).magnitude,new Vector4(_matrix.m01,_matrix.m11,_matrix.m21,_matrix.m31).magnitude,new Vector4(_matrix.m02,_matrix.m12,_matrix.m22,_matrix.m32).magnitude);
	}
}