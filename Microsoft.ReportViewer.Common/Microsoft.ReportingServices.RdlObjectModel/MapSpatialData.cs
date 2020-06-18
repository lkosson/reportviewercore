using Microsoft.ReportingServices.RdlObjectModel.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	[XmlElementClass("MapShapefile", typeof(MapShapefile))]
	[XmlElementClass("MapSpatialDataSet", typeof(MapSpatialDataSet))]
	[XmlElementClass("MapSpatialDataRegion", typeof(MapSpatialDataRegion))]
	internal abstract class MapSpatialData : ReportObject
	{
		public MapSpatialData()
		{
		}

		internal MapSpatialData(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
		}
	}
}
