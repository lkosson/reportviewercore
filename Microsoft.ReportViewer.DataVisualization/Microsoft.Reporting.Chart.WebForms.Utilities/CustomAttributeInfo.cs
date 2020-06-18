using System;

namespace Microsoft.Reporting.Chart.WebForms.Utilities
{
	internal class CustomAttributeInfo
	{
		public string Name = string.Empty;

		public Type ValueType = typeof(int);

		public object DefaultValue;

		public string Description = string.Empty;

		public SeriesChartType[] AppliesToChartType;

		public bool AppliesToSeries = true;

		public bool AppliesToDataPoint = true;

		public bool AppliesTo3D = true;

		public bool AppliesTo2D = true;

		public object MinValue;

		public object MaxValue;

		public CustomAttributeInfo(string name, Type valueType, object defaultValue, string description, SeriesChartType[] appliesToChartType, bool appliesToSeries, bool appliesToDataPoint)
		{
			Name = name;
			ValueType = valueType;
			DefaultValue = defaultValue;
			Description = description;
			AppliesToChartType = appliesToChartType;
			AppliesToSeries = appliesToSeries;
			AppliesToDataPoint = appliesToDataPoint;
		}
	}
}
