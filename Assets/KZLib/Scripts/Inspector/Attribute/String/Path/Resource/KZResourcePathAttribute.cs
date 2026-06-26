using System.Diagnostics;

namespace KZLib.Attributes
{
	/// <summary>
	/// Base for resource file paths under Assets. Provides open, change, and preview buttons.
	/// </summary>
	[Conditional("UNITY_EDITOR")]
	public abstract class KZResourcePathAttribute : KZPathAttribute
	{
		protected KZResourcePathAttribute(bool addChangeButton,bool isIncludeAssets,string wrongHexColor = null) : base(true,addChangeButton,isIncludeAssets,wrongHexColor) { }
	}
}
