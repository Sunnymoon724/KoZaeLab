using System;
using System.Diagnostics;
using System.Globalization;

namespace KZLib.Attributes
{
	/// <summary>
	/// Clamps a numeric inspector field between min and max.
	/// Supports literal values and member-name (<c>nameof</c>) expressions.
	/// </summary>
	/// <example><code>[KZClamp(0, nameof(MaxCount))]</code></example>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,AllowMultiple = false,Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class KZClampAttribute : Attribute
	{
		/// <summary>Minimum literal or member name.</summary>
		public string MinText { get; }
		/// <summary>Maximum literal or member name.</summary>
		public string MaxText { get; }

		public KZClampAttribute(int minValue,int maxValue)				: this(minValue.ToString(CultureInfo.InvariantCulture),maxValue.ToString(CultureInfo.InvariantCulture)) { }
		public KZClampAttribute(long minValue,long maxValue)			: this(minValue.ToString(CultureInfo.InvariantCulture),maxValue.ToString(CultureInfo.InvariantCulture)) { }
		public KZClampAttribute(float minValue,float maxValue)			: this(minValue.ToString(CultureInfo.InvariantCulture),maxValue.ToString(CultureInfo.InvariantCulture)) { }
		public KZClampAttribute(double minValue,double maxValue)		: this(minValue.ToString(CultureInfo.InvariantCulture),maxValue.ToString(CultureInfo.InvariantCulture)) { }

		public KZClampAttribute(int minValue,string maxExpression)		: this(minValue.ToString(CultureInfo.InvariantCulture),maxExpression) 									{ }
		public KZClampAttribute(long minValue,string maxExpression)		: this(minValue.ToString(CultureInfo.InvariantCulture),maxExpression) 									{ }
		public KZClampAttribute(float minValue,string maxExpression)	: this(minValue.ToString(CultureInfo.InvariantCulture),maxExpression) 									{ }
		public KZClampAttribute(double minValue,string maxExpression)	: this(minValue.ToString(CultureInfo.InvariantCulture),maxExpression) 									{ }

		public KZClampAttribute(string minExpression,int maxValue)		: this(minExpression,maxValue.ToString(CultureInfo.InvariantCulture)) 									{ }
		public KZClampAttribute(string minExpression,long maxValue)		: this(minExpression,maxValue.ToString(CultureInfo.InvariantCulture))  									{ }
		public KZClampAttribute(string minExpression,float maxValue)	: this(minExpression,maxValue.ToString(CultureInfo.InvariantCulture)) 									{ }
		public KZClampAttribute(string minExpression,double maxValue)	: this(minExpression,maxValue.ToString(CultureInfo.InvariantCulture))  									{ }

		protected KZClampAttribute(string minText,string maxText)
		{
			MinText = minText;
			MaxText = maxText;
		}
	}

	/// <summary>Sets only the minimum; maximum uses the field type's <see cref="int.MaxValue"/> etc.</summary>
	/// <example><code>[KZMinClamp(1)]</code> / <code>[KZMinClamp(nameof(MinCount))]</code></example>
	[Conditional("UNITY_EDITOR")]
	public class KZMinClampAttribute : KZClampAttribute
	{
		public KZMinClampAttribute(int minValue)			: base(minValue.ToString(CultureInfo.InvariantCulture),int.MaxValue.ToString(CultureInfo.InvariantCulture))		{ }
		public KZMinClampAttribute(long minValue)			: base(minValue.ToString(CultureInfo.InvariantCulture),long.MaxValue.ToString(CultureInfo.InvariantCulture))	{ }
		public KZMinClampAttribute(float minValue)			: base(minValue.ToString(CultureInfo.InvariantCulture),float.MaxValue.ToString(CultureInfo.InvariantCulture))	{ }
		public KZMinClampAttribute(double minValue)			: base(minValue.ToString(CultureInfo.InvariantCulture),double.MaxValue.ToString(CultureInfo.InvariantCulture))	{ }
		/// <param name="minExpression">Member name for minimum. Drawer resolves maximum from field type.</param>
		public KZMinClampAttribute(string minExpression)	: base(minExpression,null)																						{ }
	}

	/// <summary>Sets only the maximum; minimum uses the field type's <see cref="int.MinValue"/> etc.</summary>
	/// <example><code>[KZMaxClamp(100)]</code> / <code>[KZMaxClamp(nameof(MaxCount))]</code></example>
	[Conditional("UNITY_EDITOR")]
	public class KZMaxClampAttribute : KZClampAttribute
	{
		public KZMaxClampAttribute(int maxValue)			: base(int.MinValue.ToString(CultureInfo.InvariantCulture),maxValue.ToString(CultureInfo.InvariantCulture))		{ }
		public KZMaxClampAttribute(long maxValue)			: base(long.MinValue.ToString(CultureInfo.InvariantCulture),maxValue.ToString(CultureInfo.InvariantCulture))	{ }
		public KZMaxClampAttribute(float maxValue)			: base(float.MinValue.ToString(CultureInfo.InvariantCulture),maxValue.ToString(CultureInfo.InvariantCulture))	{ }
		public KZMaxClampAttribute(double maxValue)			: base(double.MinValue.ToString(CultureInfo.InvariantCulture),maxValue.ToString(CultureInfo.InvariantCulture))	{ }
		/// <param name="maxExpression">Member name for maximum. Drawer resolves minimum from field type.</param>
		public KZMaxClampAttribute(string maxExpression)	: base(null,maxExpression)																						{ }
	}
}
