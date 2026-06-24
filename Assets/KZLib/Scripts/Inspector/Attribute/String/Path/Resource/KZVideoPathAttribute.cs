using System.Diagnostics;

namespace KZLib.Attributes
{
	/// <summary>
	/// Video file path field. <c>isIncludeAssets</c> is fixed to <c>true</c> on <see cref="KZResourcePathAttribute"/>.
	/// </summary>
	[Conditional("UNITY_EDITOR")]
	public class KZVideoPathAttribute : KZResourcePathAttribute
	{
		public KZVideoPathAttribute(bool addChangeButton = false,string wrongHexColor = null) : base(addChangeButton,true,wrongHexColor) { }
	}
}
