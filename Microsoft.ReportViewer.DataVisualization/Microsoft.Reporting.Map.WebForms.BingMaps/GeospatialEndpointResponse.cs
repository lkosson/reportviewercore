using System.Runtime.Serialization;

namespace Microsoft.Reporting.Map.WebForms.BingMaps
{
	[DataContract(Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1")]
	public class GeospatialEndpointResponse : Resource
	{
		[DataMember(Name = "isDisputedArea", EmitDefaultValue = false)]
		public bool IsDisputedArea
		{
			get;
			set;
		}

		[DataMember(Name = "isSupported", EmitDefaultValue = false)]
		public bool IsSupported
		{
			get;
			set;
		}

		[DataMember(Name = "ur", EmitDefaultValue = false)]
		public string UserRegion
		{
			get;
			set;
		}

		[DataMember(Name = "services", EmitDefaultValue = false)]
		public GeospatialService[] Services
		{
			get;
			set;
		}
	}
}
