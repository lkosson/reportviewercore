using System.Runtime.Serialization;

namespace Microsoft.Reporting.Map.WebForms.BingMaps
{
	[DataContract]
	public class CoverageArea
	{
		[DataMember(Name = "bbox", EmitDefaultValue = false)]
		public double[] BoundingBox
		{
			get;
			set;
		}

		[DataMember(Name = "zoomMax", EmitDefaultValue = false)]
		public int ZoomMax
		{
			get;
			set;
		}

		[DataMember(Name = "zoomMin", EmitDefaultValue = false)]
		public int ZoomMin
		{
			get;
			set;
		}
	}
}
