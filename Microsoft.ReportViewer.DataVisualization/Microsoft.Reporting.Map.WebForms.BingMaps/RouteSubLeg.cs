using System.Runtime.Serialization;

namespace Microsoft.Reporting.Map.WebForms.BingMaps
{
	[DataContract]
	public class RouteSubLeg
	{
		[DataMember(Name = "endWaypoint", EmitDefaultValue = false)]
		public Waypoint EndWaypoint
		{
			get;
			set;
		}

		[DataMember(Name = "startWaypoint", EmitDefaultValue = false)]
		public Waypoint StartWaypoint
		{
			get;
			set;
		}

		[DataMember(Name = "travelDistance", EmitDefaultValue = false)]
		public double TravelDistance
		{
			get;
			set;
		}

		[DataMember(Name = "travelDuration", EmitDefaultValue = false)]
		public double TravelDuration
		{
			get;
			set;
		}

		[DataMember(Name = "travelDurationTraffic", EmitDefaultValue = false)]
		public double TravelDurationTraffic
		{
			get;
			set;
		}
	}
}
