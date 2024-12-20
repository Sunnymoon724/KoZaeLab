﻿using KZLib;

public class EventTag : Enumeration
{
	// Language
	public static readonly EventTag ChangeLanguageOption	= new(nameof(ChangeLanguageOption));

	// Graphic Option
	public static readonly EventTag ChangeGraphicOption		= new(nameof(ChangeGraphicOption));
	// Sound Option
	public static readonly EventTag ChangeSoundOption		= new(nameof(ChangeSoundOption));

	// Native Option
	public static readonly EventTag ChangeNativeOption		= new(nameof(ChangeNativeOption));

	// Touch
	public static readonly EventTag TouchDownPoint			= new(nameof(TouchDownPoint));
	public static readonly EventTag TouchHoldingPoint		= new(nameof(TouchHoldingPoint));
	public static readonly EventTag TouchUpPoint			= new(nameof(TouchUpPoint));

	// Update Account
	public static readonly EventTag UpdateAccount			= new(nameof(UpdateAccount));

	public EventTag(string _name) : base($"[Event] {_name}") { }
}