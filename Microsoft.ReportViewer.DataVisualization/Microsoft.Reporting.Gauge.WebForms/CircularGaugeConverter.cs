namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class CircularGaugeConverter : CollectionItemTypeConverter
	{
		public CircularGaugeConverter()
		{
			simpleType = typeof(CircularGauge);
		}
	}
}
