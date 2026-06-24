using System.Diagnostics;

namespace KZLib.Attributes
{
	/// <summary>ScriptableObject asset path field.</summary>
	[Conditional("UNITY_EDITOR")]
	public class KZScriptableObjectPathAttribute : KZResourcePathAttribute
	{
		public KZScriptableObjectPathAttribute(bool addChangeButton = false,bool isIncludeAssets = false,string wrongHexColor = null) : base(addChangeButton,isIncludeAssets,wrongHexColor) { }
	}
}
