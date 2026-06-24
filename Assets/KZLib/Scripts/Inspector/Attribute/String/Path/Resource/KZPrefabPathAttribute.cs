using System.Diagnostics;

namespace KZLib.Attributes
{
	/// <summary>Prefab (<c>.prefab</c>) path field.</summary>
	[Conditional("UNITY_EDITOR")]
	public class KZPrefabPathAttribute : KZResourcePathAttribute
	{
		public KZPrefabPathAttribute(bool addChangeButton = false,bool isIncludeAssets = false,string wrongHexColor = null) : base(addChangeButton,isIncludeAssets,wrongHexColor) { }
	}
}
