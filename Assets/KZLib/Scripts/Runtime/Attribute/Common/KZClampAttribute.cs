using System;
using System.Diagnostics;

namespace KZLib.Attributes
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,AllowMultiple = false,Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class KZClampAttribute : Attribute
	{
		public string MinText { get; }
		public string MaxText { get; }

		public KZClampAttribute(int minValue,int maxValue)				: this(minValue.ToString(),maxValue.ToString()) { }
		public KZClampAttribute(long minValue,long maxValue)			: this(minValue.ToString(),maxValue.ToString()) { }
		public KZClampAttribute(float minValue,float maxValue)			: this(minValue.ToString(),maxValue.ToString()) { }
		public KZClampAttribute(double minValue,double maxValue)		: this(minValue.ToString(),maxValue.ToString()) { }

		public KZClampAttribute(int minValue,string maxExpression)		: this(minValue.ToString(),maxExpression) 		{ }
		public KZClampAttribute(long minValue,string maxExpression)		: this(minValue.ToString(),maxExpression) 		{ }
		public KZClampAttribute(float minValue,string maxExpression)	: this(minValue.ToString(),maxExpression) 		{ }
		public KZClampAttribute(double minValue,string maxExpression)	: this(minValue.ToString(),maxExpression) 		{ }

		public KZClampAttribute(string minExpression,int maxValue)		: this(minExpression,maxValue.ToString()) 		{ }
		public KZClampAttribute(string minExpression,long maxValue)		: this(minExpression,maxValue.ToString()) 		{ }
		public KZClampAttribute(string minExpression,float maxValue)	: this(minExpression,maxValue.ToString()) 		{ }
		public KZClampAttribute(string minExpression,double maxValue)	: this(minExpression,maxValue.ToString()) 		{ }

		protected KZClampAttribute(string minText,string maxText)
		{
			MinText = minText;
			MaxText = maxText;
		}
	}

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,AllowMultiple = false,Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class KZMaxClampAttribute : KZClampAttribute
	{
		public KZMaxClampAttribute(int maxValue)			: base(int.MinValue.ToString(),maxValue.ToString())		{ }
		public KZMaxClampAttribute(long maxValue)			: base(long.MinValue.ToString(),maxValue.ToString())	{ }
		public KZMaxClampAttribute(float maxValue)			: base(float.MinValue.ToString(),maxValue.ToString())	{ }
		public KZMaxClampAttribute(double maxValue)			: base(double.MinValue.ToString(),maxValue.ToString())	{ }
		public KZMaxClampAttribute(string maxExpression)	: base(double.MinValue.ToString(),maxExpression)		{ }
	}

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,AllowMultiple = false,Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class KZMinClampAttribute : KZClampAttribute
	{
		public KZMinClampAttribute(int minValue)			: base(minValue.ToString(),int.MaxValue.ToString())		{ }
		public KZMinClampAttribute(long minValue)			: base(minValue.ToString(),long.MaxValue.ToString())	{ }
		public KZMinClampAttribute(float minValue)			: base(minValue.ToString(),float.MaxValue.ToString())	{ }
		public KZMinClampAttribute(double minValue)			: base(minValue.ToString(),double.MaxValue.ToString())	{ }
		public KZMinClampAttribute(string minExpression)	: base(minExpression,double.MaxValue.ToString())		{ }
	}
}