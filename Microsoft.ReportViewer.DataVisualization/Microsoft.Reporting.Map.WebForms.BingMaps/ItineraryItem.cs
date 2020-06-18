using System;
using System.Runtime.Serialization;

namespace Microsoft.Reporting.Map.WebForms.BingMaps
{
	[DataContract]
	public class ItineraryItem
	{
		[DataMember(Name = "childItineraryItems", EmitDefaultValue = false)]
		public ItineraryItem[] ChildItineraryItems
		{
			get;
			set;
		}

		[DataMember(Name = "compassDirection", EmitDefaultValue = false)]
		public string CompassDirection
		{
			get;
			set;
		}

		[DataMember(Name = "details", EmitDefaultValue = false)]
		public Detail[] Details
		{
			get;
			set;
		}

		[DataMember(Name = "exit", EmitDefaultValue = false)]
		public string Exit
		{
			get;
			set;
		}

		[DataMember(Name = "hints", EmitDefaultValue = false)]
		public Hint[] Hints
		{
			get;
			set;
		}

		[DataMember(Name = "iconType", EmitDefaultValue = false)]
		public string IconType
		{
			get;
			set;
		}

		[DataMember(Name = "instruction", EmitDefaultValue = false)]
		public Instruction Instruction
		{
			get;
			set;
		}

		[DataMember(Name = "maneuverPoint", EmitDefaultValue = false)]
		public Point ManeuverPoint
		{
			get;
			set;
		}

		[DataMember(Name = "sideOfStreet", EmitDefaultValue = false)]
		public string SideOfStreet
		{
			get;
			set;
		}

		[DataMember(Name = "signs", EmitDefaultValue = false)]
		public string[] Signs
		{
			get;
			set;
		}

		[DataMember(Name = "time", EmitDefaultValue = false)]
		public string Time
		{
			get;
			set;
		}

		public DateTime TimeUtc
		{
			get
			{
				if (string.IsNullOrEmpty(Time))
				{
					return DateTime.Now;
				}
				return DateTimeHelper.FromOdataJson(Time);
			}
			set
			{
				string text = DateTimeHelper.ToOdataJson(value);
				if (text != null)
				{
					Time = text;
				}
				else
				{
					Time = string.Empty;
				}
			}
		}

		[DataMember(Name = "tollZone", EmitDefaultValue = false)]
		public string TollZone
		{
			get;
			set;
		}

		[DataMember(Name = "towardsRoadName", EmitDefaultValue = false)]
		public string TowardsRoadName
		{
			get;
			set;
		}

		[DataMember(Name = "transitLine", EmitDefaultValue = false)]
		public TransitLine TransitLine
		{
			get;
			set;
		}

		[DataMember(Name = "transitStopId", EmitDefaultValue = false)]
		public int TransitStopId
		{
			get;
			set;
		}

		[DataMember(Name = "transitTerminus", EmitDefaultValue = false)]
		public string TransitTerminus
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

		[DataMember(Name = "travelMode", EmitDefaultValue = false)]
		public string TravelMode
		{
			get;
			set;
		}

		[DataMember(Name = "warning", EmitDefaultValue = false)]
		public Warning[] Warning
		{
			get;
			set;
		}
	}
}
