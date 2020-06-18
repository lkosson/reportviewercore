namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class NumericRangeConverter : CollectionItemTypeConverter
	{
		public NumericRangeConverter()
		{
			simpleType = typeof(NumericRange);
		}
	}
}
