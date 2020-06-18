using System.Runtime.Serialization;

namespace Microsoft.Reporting.Map.WebForms.BingMaps
{
	[DataContract(Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1")]
	public class BirdseyeMetadata : ImageryMetadata
	{
		[DataMember(Name = "orientation", EmitDefaultValue = false)]
		public double Orientation
		{
			get;
			set;
		}

		[DataMember(Name = "tilesX", EmitDefaultValue = false)]
		public int TilesX
		{
			get;
			set;
		}

		[DataMember(Name = "tilesY", EmitDefaultValue = false)]
		public int TilesY
		{
			get;
			set;
		}
	}
}
