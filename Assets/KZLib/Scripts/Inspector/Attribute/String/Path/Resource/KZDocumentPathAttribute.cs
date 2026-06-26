using System.Diagnostics;

namespace KZLib.Attributes
{
	/// <summary>
	/// Document file path field. Defaults: change button on, Assets-relative path off.
	/// </summary>
	[Conditional("UNITY_EDITOR")]
	public class KZDocumentPathAttribute : KZResourcePathAttribute
	{
		public KZDocumentPathAttribute(bool addChangeButton = true,bool isIncludeAssets = false,string wrongHexColor = null) : base(addChangeButton,isIncludeAssets,wrongHexColor) { }
	}
}
