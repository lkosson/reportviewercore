using System;

namespace Microsoft.Reporting.Gauge.WebForms
{
	[AttributeUsage(AttributeTargets.Property)]
	internal sealed class ValidateBound : Attribute
	{
		private double minimum;

		private double maximum;

		private bool required = true;

		internal double Minimum => minimum;

		internal double Maximum => maximum;

		internal bool Required => required;

		internal ValidateBound(double minimum, double maximum)
		{
			this.minimum = minimum;
			this.maximum = maximum;
		}

		internal ValidateBound(double minimum, double maximum, bool required)
		{
			this.minimum = minimum;
			this.maximum = maximum;
			this.required = required;
		}
	}
}
