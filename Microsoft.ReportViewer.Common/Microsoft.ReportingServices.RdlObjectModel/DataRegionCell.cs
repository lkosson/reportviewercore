namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal abstract class DataRegionCell : ReportObject
	{
		public DataRegionCell()
		{
		}

		internal DataRegionCell(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}
	}
}
