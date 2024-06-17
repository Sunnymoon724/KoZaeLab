using System.ComponentModel;
using UnityEngine;

namespace System.Runtime.CompilerServices
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal class IsExternalInit { }
}

public record MessageData(string Head,string Body);

public record TransitionData
{
	public float Duration { get; }
	public Color TransitionColor { get; }

	public Texture2D WipeTexture { get; }
	public bool IsCutout { get; }

	public TransitionData(Texture2D _texture,bool _cutout,float _duration = Global.FADE_TIME,Color? _color = null)
	{
		Duration = _duration;
		TransitionColor = _color ?? Color.black;

		WipeTexture = _texture;
		IsCutout = _cutout;
	}

	public TransitionData(float _duration = Global.FADE_TIME,Color? _color = null) : this(null,false,_duration,_color) { }

	public bool IsFade => WipeTexture == null;
}