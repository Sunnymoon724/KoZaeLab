using System.Diagnostics;

namespace KZLib.Attributes
{
	/// <summary>
	/// Folder path field with folder picker and open button.
	/// </summary>
	/// <remarks>
	/// Constructor: <c>addChangeButton</c>, <c>isIncludeAssets</c>, <c>wrongHexColor</c> — see <see cref="KZPathAttribute"/>.
	/// </remarks>
	[Conditional("UNITY_EDITOR")]
	public class KZFolderPathAttribute : KZPathAttribute
	{
		public KZFolderPathAttribute(bool addChangeButton = true,bool isIncludeAssets = true,string wrongHexColor = null) : base(true,addChangeButton,isIncludeAssets,wrongHexColor) { }
	}
}
