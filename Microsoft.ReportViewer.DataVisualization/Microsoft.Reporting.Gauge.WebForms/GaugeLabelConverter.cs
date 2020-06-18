namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class GaugeLabelConverter : CollectionItemTypeConverter
	{
		public GaugeLabelConverter()
		{
			simpleType = typeof(GaugeLabel);
		}
	}
}
