using UnityEngine;

public record TransitionData
{
	public float Duration { get; }
	public Color TransitionColor { get; }

	public Texture2D TransitionTexture { get; }
	public bool IsCutout { get; }

	public TransitionData(Texture2D _texture,bool _cutout,float _duration = Global.FADE_TIME,Color? _color = null)
	{
		Duration = _duration;
		TransitionColor = _color ?? Color.black;

		TransitionTexture = _texture;
		IsCutout = _cutout;
	}

	public TransitionData(float _duration = Global.FADE_TIME,Color? _color = null) : this(null,false,_duration,_color) { }

	public bool IsFade => TransitionTexture == null;
}