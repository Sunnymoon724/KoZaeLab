using System;
using UnityEngine;

namespace KZLib
{
	[Serializable]
	public class RecordData
	{
		#region AnimationCurveVector3
		[Serializable]
		private class AnimationCurveVector3
		{
			private readonly AnimationCurve m_CurveX = new();
			private readonly AnimationCurve m_CurveY = new();
			private readonly AnimationCurve m_CurveZ = new();

			public AnimationCurveVector3()
			{
				// m_CurveX.
			}

			private float? m_Previous = null;

			public void AddVector(float _time,Vector3 _vector)
			{
				if(m_Previous.HasValue)
				{
					var previous = GetVector(m_Previous.Value);

					if(previous.IsEquals(_vector))
					{
						return;
					}
				}

				m_CurveX.AddKey(_time,_vector.x);
				m_CurveY.AddKey(_time,_vector.y);
				m_CurveZ.AddKey(_time,_vector.z);

				m_Previous = _time;
			}

			public Vector3 GetVector(float _time)
			{
				return new Vector3(m_CurveX.Evaluate(_time),m_CurveY.Evaluate(_time),m_CurveZ.Evaluate(_time));
			}
		}
		#endregion AnimationCurveVector3

		#region AnimationCurveQuaternion
		[Serializable]
		private class AnimationCurveQuaternion
		{
			private readonly AnimationCurve m_CurveX = new();
			private readonly AnimationCurve m_CurveY = new();
			private readonly AnimationCurve m_CurveZ = new();
			private readonly AnimationCurve m_CurveW = new();

			private float? m_Previous = null;

			public void AddQuaternion(float _time,Quaternion _quaternion)
			{
				if(m_Previous.HasValue)
				{
					var previous = GetQuaternion(m_Previous.Value);

					if(previous.IsEquals(_quaternion))
					{
						return;
					}
				}

				m_CurveX.AddKey(_time,_quaternion.x);
				m_CurveY.AddKey(_time,_quaternion.y);
				m_CurveZ.AddKey(_time,_quaternion.z);
				m_CurveW.AddKey(_time,_quaternion.w);

				m_Previous = _time;
			}

			public Quaternion GetQuaternion(float _time)
			{
				return new Quaternion(m_CurveX.Evaluate(_time),m_CurveY.Evaluate(_time),m_CurveZ.Evaluate(_time),m_CurveW.Evaluate(_time));
			}
		}
		#endregion AnimationCurveQuaternion

		private readonly AnimationCurveVector3 m_Position = new();
		private readonly AnimationCurveQuaternion m_Rotation = new();
		private readonly AnimationCurveVector3 m_LocalScale = new();

		public void AddTransform(float _time,Transform _transform)
		{
			m_Position.AddVector(_time,_transform.position);
			m_Rotation.AddQuaternion(_time,_transform.rotation);
			m_LocalScale.AddVector(_time,_transform.localScale);
		}

		public void SetTransform(float _time,Transform _transform)
		{
			_transform.SetPositionAndRotation(m_Position.GetVector(_time),m_Rotation.GetQuaternion(_time));
            _transform.localScale = m_LocalScale.GetVector(_time);
		}
	}
}