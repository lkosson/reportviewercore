using System;

namespace Microsoft.Reporting.Map.WebForms
{
	[AttributeUsage(AttributeTargets.Property)]
	internal class DoubleConverterHint : Attribute
	{
		private double bound;

		public virtual double Bound => bound;

		public DoubleConverterHint(double bound)
		{
			this.bound = bound;
		}
	}
}
