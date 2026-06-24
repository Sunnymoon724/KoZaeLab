using System.Diagnostics;

namespace KZLib.Attributes
{
	/// <summary>Texture image path field.</summary>
	[Conditional("UNITY_EDITOR")]
	public class KZTexturePathAttribute : KZResourcePathAttribute
	{
		public KZTexturePathAttribute(bool addChangeButton = false,bool isIncludeAssets = false,string wrongHexColor = null) : base(addChangeButton,isIncludeAssets,wrongHexColor) { }
	}
}
