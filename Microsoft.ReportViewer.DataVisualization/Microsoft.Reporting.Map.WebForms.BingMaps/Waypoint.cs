using System.Runtime.Serialization;

namespace Microsoft.Reporting.Map.WebForms.BingMaps
{
	[DataContract]
	public class Waypoint : Point
	{
		[DataMember(Name = "description", EmitDefaultValue = false)]
		public string Description
		{
			get;
			set;
		}

		[DataMember(Name = "isVia", EmitDefaultValue = false)]
		public bool IsVia
		{
			get;
			set;
		}

		[DataMember(Name = "locationIdentifier", EmitDefaultValue = false)]
		public string LocationIdentifier
		{
			get;
			set;
		}

		[DataMember(Name = "routePathIndex", EmitDefaultValue = false)]
		public int RoutePathIndex
		{
			get;
			set;
		}
	}
}
