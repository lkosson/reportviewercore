using System;

namespace Microsoft.Reporting.Gauge.WebForms
{
	[AttributeUsage(AttributeTargets.Property)]
	internal sealed class DoubleConverterHint : Attribute
	{
		private double bound;

		public double Bound => bound;

		public DoubleConverterHint(double bound)
		{
			this.bound = bound;
		}
	}
}
