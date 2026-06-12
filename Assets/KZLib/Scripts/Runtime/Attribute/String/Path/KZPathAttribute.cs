using System;
using System.Diagnostics;

namespace KZLib.Attributes
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,AllowMultiple = false,Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public abstract class KZPathAttribute : Attribute
	{
		public bool AddOpenButton { get; }
		public bool AddChangeButton { get; }

		public bool IsIncludeAssets { get; }

		protected KZPathAttribute(bool addOpenBtn,bool changePathBtn,bool isIncludeAssets) : base()
		{
			AddOpenButton = addOpenBtn;
			AddChangeButton = changePathBtn;

			IsIncludeAssets = isIncludeAssets;
		}
	}
}