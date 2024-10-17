using System;
using UnityEngine;

public enum EaseType
{
	Linear,
	RiseFall, FallRise, Mountain, Valley,
	InSine, OutSine, InOutSine, OutInSine,

	InQuad, OutQuad, InOutQuad, OutInQuad,
	InCubic, OutCubic, InOutCubic, OutInCubic,
	InQuart, OutQuart, InOutQuart, OutInQuart,
	InQuint, OutQuint, InOutQuint, OutInQuint,

	InExpo, OutExpo, InOutExpo, OutInExpo,
	InCirc, OutCirc, InOutCirc, OutInCirc,

	InBounce, OutBounce, InOutBounce, OutInBounce,

	InElastic, OutElastic, InOutElastic, OutInElastic,

	InBack, OutBack, InOutBack, OutInBack,
	// Flash, InFlash, OutFlash, InOutFlash
}

public static partial class MathUtility
{
	/// <summary>
	/// To EaseType
	/// </summary>
	public static AnimationCurve GetEaseCurve(EaseType _type)
	{
		switch(_type)
		{
			#region Sine
			case EaseType.InSine:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,0.0f),new Keyframe(1.0f,1.0f,1.5f,0.0f));
			}
			case EaseType.OutSine:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,1.5f),new Keyframe(1.0f,1.0f,0.0f,0.0f));
			}
			case EaseType.InOutSine:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,0.0f),new Keyframe(0.5f,0.5f,1.5f,1.5f),new Keyframe(1.0f,1.0f,0.0f,0.0f));
			}
			case EaseType.OutInSine:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,1.5f),new Keyframe(0.5f,0.5f,0.0f,0.0f),new Keyframe(1.0f,1.0f,1.5f,0.0f));
			}
			#endregion Sine

			#region Quad
			case EaseType.InQuad:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,0.0f),new Keyframe(1.0f,1.0f,2.0f,0.0f));
			}
			case EaseType.OutQuad:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,2.0f),new Keyframe(1.0f,1.0f,0.0f,0.0f));
			}
			case EaseType.InOutQuad:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,0.0f),new Keyframe(0.5f,0.5f,2.0f,2.0f),new Keyframe(1.0f,1.0f,0.0f,0.0f));
			}
			case EaseType.OutInQuad:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,2.0f),new Keyframe(0.5f,0.5f,0.0f,0.0f),new Keyframe(1.0f,1.0f,2.0f,0.0f));
			}
			#endregion Quad

			#region Cubic
			case EaseType.InCubic:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,0.0f),new Keyframe(1.0f,1.0f,3.0f,0.0f));
			}
			case EaseType.OutCubic:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,3.0f),new Keyframe(1.0f,1.0f,0.0f,0.0f));
			}
			case EaseType.InOutCubic:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,0.0f),new Keyframe(0.5f,0.5f,3.0f,3.0f),new Keyframe(1.0f,1.0f,0.0f,0.0f));
			}
			case EaseType.OutInCubic:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,3.0f),new Keyframe(0.5f,0.5f,0.0f,0.0f),new Keyframe(1.0f,1.0f,3.0f,0.0f));
			}
			#endregion Cubic

			#region Quartic
			case EaseType.InQuart:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,0.0f),new Keyframe(1.0f,1.0f,4.0f,0.0f));
			}
			case EaseType.OutQuart:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,4.0f),new Keyframe(1.0f,1.0f,0.0f,0.0f));
			}
			case EaseType.InOutQuart:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,0.0f),new Keyframe(0.5f,0.5f,4.0f,4.0f),new Keyframe(1.0f,1.0f,0.0f,0.0f));
			}
			case EaseType.OutInQuart:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,4.0f),new Keyframe(0.5f,0.5f,0.0f,0.0f),new Keyframe(1.0f,1.0f,4.0f,0.0f));
			}
			#endregion Quartic

			#region Quintic
			case EaseType.InQuint:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,0.0f),new Keyframe(1.0f,1.0f,5.0f,0.0f));
			}
			case EaseType.OutQuint:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,5.0f),new Keyframe(1.0f,1.0f,0.0f,0.0f));
			}
			case EaseType.InOutQuint:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,0.0f),new Keyframe(0.5f,0.5f,5.0f,5.0f),new Keyframe(1.0f,1.0f,0.0f,0.0f));
			}
			case EaseType.OutInQuint:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,5.0f),new Keyframe(0.5f,0.5f,0.0f,0.0f),new Keyframe(1.0f,1.0f,5.0f,0.0f));
			}
			#endregion Quintic

			#region Expo
			case EaseType.InExpo:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,0.0f),new Keyframe(1.0f,1.0f,7.0f,0.0f));
			}
			case EaseType.OutExpo:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,7.0f),new Keyframe(1.0f,1.0f,0.0f,0.0f));
			}
			case EaseType.InOutExpo:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,0.0f),new Keyframe(0.5f,0.5f,7.0f,7.0f),new Keyframe(1.0f,1.0f,0.0f,0.0f));
			}
			case EaseType.OutInExpo:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,7.0f),new Keyframe(0.5f,0.5f,0.0f,0.0f),new Keyframe(1.0f,1.0f,7.0f,0.0f));
			}
			#endregion Expo

			#region Circle
			case EaseType.InCirc:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,0.0f),new Keyframe(1.0f,1.0f,460.0f,0.0f));
			}
			case EaseType.OutCirc:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,460.0f),new Keyframe(1.0f,1.0f,0.0f,0.0f));
			}
			case EaseType.InOutCirc:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,0.0f),new Keyframe(0.5f,0.5f,460.0f,460.0f),new Keyframe(1.0f,1.0f,0.0f,0.0f));
			}
			case EaseType.OutInCirc:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,460.0f),new Keyframe(0.5f,0.5f,0.0f,0.0f),new Keyframe(1.0f,1.0f,460.0f,0.0f));
			}
			#endregion Circle

			#region Bounce
			case EaseType.InBounce:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,0.7f),new Keyframe(0.1f,0.0f,-0.7f,1.4f),new Keyframe(0.3f,0.0f,-1.4f,2.7f),new Keyframe(0.6f,0.0f,-2.7f,5.5f),new Keyframe(1.0f,1.0f,0.0f,0.0f));
			}
			case EaseType.OutBounce:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,0.0f),new Keyframe(0.4f,1.0f,5.5f,-2.7f),new Keyframe(0.7f,1.0f,2.7f,-1.4f),new Keyframe(0.9f,1.0f,1.4f,-0.7f),new Keyframe(1.0f,1.0f,0.7f,0.0f));
			}
			case EaseType.InOutBounce:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,0.7f),new Keyframe(0.1f,0.0f,-0.7f,1.4f),new Keyframe(0.2f,0.0f,-1.4f,2.7f),new Keyframe(0.3f,0.0f,-2.7f,5.5f),new Keyframe(0.5f,0.5f,0.0f,0.0f),new Keyframe(0.6f,1.0f,5.5f,-2.7f),new Keyframe(0.8f,1.0f,2.7f,-1.4f),new Keyframe(0.9f,1.0f,1.4f,-0.7f),new Keyframe(1.0f,1.0f,0.7f,0.0f));
			}
			case EaseType.OutInBounce:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,0.0f),new Keyframe(0.2f,1.0f,5.5f,-2.7f),new Keyframe(0.3f,1.0f,2.7f,-1.4f),new Keyframe(0.4f,1.0f,1.4f,-0.7f),new Keyframe(0.5f,0.5f,0.7f,0.7f),new Keyframe(0.6f,0.0f,-0.7f,1.4f),new Keyframe(0.7f,0.0f,-1.4f,2.7f),new Keyframe(0.8f,0.0f,-2.7f,5.5f),new Keyframe(1.0f,1.0f,0.0f,0.0f));
			}
			#endregion Bounce

			#region Back
			case EaseType.InBack:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,0.0f),new Keyframe(1.0f,1.0f,4.7f,0.0f));
			}
			case EaseType.OutBack:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,4.7f),new Keyframe(1.0f,1.0f,0.0f,0.0f));
			}
			case EaseType.InOutBack:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,0.0f),new Keyframe(0.5f,0.5f,4.7f,4.7f),new Keyframe(1.0f,1.0f,0.0f,0.0f));
			}
			case EaseType.OutInBack:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,4.7f),new Keyframe(0.5f,0.5f,0.0f,0.0f),new Keyframe(1.0f,1.0f,4.7f,0.0f));
			}
			#endregion Back

			#region Elastic
			case EaseType.InElastic:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,0.0f),new Keyframe(0.2f,0.0f,0.0f,-0.1f),new Keyframe(0.5f,0.0f,-0.4f,-0.6f),new Keyframe(0.8f,0.0f,-3.2f,-4.4f),new Keyframe(1.0f,1.0f,12.5f,0.0f));
			}
			case EaseType.OutElastic:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,12.5f),new Keyframe(0.2f,1.0f,-4.4f,-3.2f),new Keyframe(0.5f,1.0f,-0.6f,-0.4f),new Keyframe(0.8f,1.0f,-0.1f,0.0f),new Keyframe(1.0f,1.0f,0.5f,0.0f));
			}
			case EaseType.InOutElastic:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,0.0f),new Keyframe(0.1f,0.0f,0.0f,-0.1f),new Keyframe(0.3f,0.0f,-0.4f,-0.6f),new Keyframe(0.4f,0.0f,-3.2f,-4.4f),new Keyframe(0.5f,0.5f,12.5f,12.5f),new Keyframe(0.6f,1.0f,-4.4f,-3.2f),new Keyframe(0.7f,1.0f,-0.6f,-0.4f),new Keyframe(0.9f,1.0f,-0.1f,0.0f),new Keyframe(1.0f,1.0f,0.5f,0.0f));
			}
			case EaseType.OutInElastic:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,12.5f),new Keyframe(0.1f,1.0f,-4.4f,-3.2f),new Keyframe(0.3f,1.0f,-0.6f,-0.4f),new Keyframe(0.4f,1.0f,-0.1f,0.0f),new Keyframe(0.5f,0.5f,0.5f,0.0f),new Keyframe(0.6f,0.0f,0.0f,-0.1f),new Keyframe(0.7f,0.0f,-0.4f,-0.6f),new Keyframe(0.9f,0.0f,-3.2f,-4.4f),new Keyframe(1.0f,1.0f,12.5f,0.0f));
			}
			#endregion Elastic

			#region Linear
			case EaseType.Linear:
			{
				return AnimationCurve.Linear(0.0f,0.0f,1.0f,1.0f);
			}
			#endregion Linear

			#region Custom
			case EaseType.RiseFall:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,+2.0f),new Keyframe(0.5f,1.0f,+2.0f,-2.0f),new Keyframe(1.0f,0.0f,-2.0f,0.0f));
			}
			case EaseType.FallRise:
			{
				return new AnimationCurve(new Keyframe(0.0f,1.0f,0.0f,-2.0f),new Keyframe(0.5f,0.0f,-2.0f,+2.0f),new Keyframe(1.0f,1.0f,+2.0f,0.0f));
			}
			case EaseType.Mountain:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,0.0f),new Keyframe(0.25f,0.0f,0.0f,+4.0f),new Keyframe(0.5f,1.0f,+4.0f,-4.0f),new Keyframe(0.75f,0.0f,-4.0f,0.0f),new Keyframe(1.0f,0.0f,0.0f,0.0f));
			}
			case EaseType.Valley:
			{
				return new AnimationCurve(new Keyframe(0.0f,1.0f,0.0f,0.0f),new Keyframe(0.25f,1.0f,0.0f,-4.0f),new Keyframe(0.5f,0.0f,-4.0f,+4.0f),new Keyframe(0.75f,1.0f,+4.0f,0.0f),new Keyframe(1.0f,1.0f,0.0f,0.0f));
			}
			#endregion Custom
		}

		throw new ArgumentException("Not Defined.");
	}

	/// <summary>
	/// Increase -> Decrease or Decrease -> Increase
	/// </summary>
	public static AnimationCurve GetIncrementalCurve(bool _increaseStart)
	{
		var curve = new AnimationCurve();

		if(_increaseStart)
		{
			curve.AddKey(new Keyframe(0.0f,0.0f,+0.0f,+2.0f));
			curve.AddKey(new Keyframe(0.5f,1.0f,+2.0f,-2.0f));
			curve.AddKey(new Keyframe(1.0f,0.0f,-2.0f,+0.0f));
		}
		else
		{
			curve.AddKey(new Keyframe(0.0f,1.0f,+0.0f,-2.0f));
			curve.AddKey(new Keyframe(0.5f,0.0f,-2.0f,+2.0f));
			curve.AddKey(new Keyframe(1.0f,1.0f,+2.0f,+0.0f));
		}

		return curve;
	}
}