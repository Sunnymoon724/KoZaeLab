using System;
using System.Diagnostics;

namespace KZLib.Attributes
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,AllowMultiple = false,Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class KZIsValidAttribute : Attribute
	{
		public string CorrectText { get; }
		public string WrongText { get; }

		public string WrongHexColor { get; }

		public KZIsValidAttribute(string correctText = "O",string wrongText = "X",string wrongHexColor = null)
		{
			CorrectText = correctText;
			WrongText = wrongText;
			WrongHexColor = wrongHexColor;
		}
	}
}