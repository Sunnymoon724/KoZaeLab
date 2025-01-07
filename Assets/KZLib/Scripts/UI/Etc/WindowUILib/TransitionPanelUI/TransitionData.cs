using UnityEngine;

namespace TransitionPanel
{
	public record TransitionData
	{
		public float Duration { get; }
		public Color TransitionColor { get; }

		public Texture2D TransitionTexture { get; }
		public bool IsCutout { get; }

		public TransitionData(Texture2D texture2D,bool isCutout,float duration = Global.FADE_TIME,Color? transitionColor = null)
		{
			Duration = duration;
			TransitionColor = transitionColor ?? Color.black;

			TransitionTexture = texture2D;
			IsCutout = isCutout;
		}

		public TransitionData(float duration = Global.FADE_TIME,Color? transitionColor = null) : this(null,false,duration,transitionColor) { }

		public bool IsFade => TransitionTexture == null;
	}
}