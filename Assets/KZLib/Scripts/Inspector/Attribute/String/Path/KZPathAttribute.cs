using System;
using System.Diagnostics;

namespace KZLib.Attributes
{
	/// <summary>
	/// Base inspector options for path string fields.
	/// </summary>
	/// <remarks>
	/// When <see cref="IsIncludeAssets"/> is true, paths are stored relative to <c>Assets/</c>; otherwise absolute project paths are used.<br/>
	/// Drawer: <c>OnlyEditor/AttributeDrawer/String/Path/KZPathAttribute.cs</c>
	/// </remarks>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,AllowMultiple = false,Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public abstract class KZPathAttribute : Attribute
	{
		/// <summary>Whether to show the open-path button.</summary>
		public bool AddOpenButton { get; }
		/// <summary>Whether to show the change-path (file/folder picker) button.</summary>
		public bool AddChangeButton { get; }
		/// <summary><c>true</c>: Assets-relative path / <c>false</c>: absolute path.</summary>
		public bool IsIncludeAssets { get; }
		/// <summary>Label color (hex) for invalid paths. Null uses <see cref="Global.WrongHexColor"/>.</summary>
		public string WrongHexColor { get; }

		protected KZPathAttribute(bool addOpenButton,bool addChangeButton,bool isIncludeAssets,string wrongHexColor = null) : base()
		{
			AddOpenButton = addOpenButton;
			AddChangeButton = addChangeButton;

			IsIncludeAssets = isIncludeAssets;
			WrongHexColor = wrongHexColor;
		}
	}
}
