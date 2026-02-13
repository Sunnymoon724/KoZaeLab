using UnityEngine;

namespace KZLib.Data.Transition
{
	public record TransitionInfo
	{
		private const float c_fadeTime = 1.0f;

		public float Duration { get; }
		public Color Color { get; }

		public Texture2D Texture { get; }
		public bool IsCutout { get; }

		public TransitionInfo(Texture2D texture2D,bool isCutout,float duration = c_fadeTime,Color? color = null)
		{
			Duration = duration;
			Color = color ?? Color.black;

			Texture = texture2D;
			IsCutout = isCutout;
		}

		public TransitionInfo(float duration = c_fadeTime,Color? color = null) : this(null,false,duration,color) { }

		public bool IsFade => Texture == null;
	}
}