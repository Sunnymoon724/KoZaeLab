using System.Diagnostics;

namespace KZLib.Attributes
{
	/// <summary>
	/// AudioClip path field. <c>isIncludeAssets</c> is fixed to <c>true</c> on <see cref="KZResourcePathAttribute"/>.
	/// </summary>
	[Conditional("UNITY_EDITOR")]
	public class KZAudioClipPathAttribute : KZResourcePathAttribute
	{
		public KZAudioClipPathAttribute(bool addChangeButton = false,string wrongHexColor = null) : base(addChangeButton,true,wrongHexColor) { }
	}
}
