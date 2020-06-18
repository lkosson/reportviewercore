using System.Runtime.Serialization;

namespace Microsoft.Reporting.Map.WebForms.BingMaps
{
	[DataContract]
	public class Shield
	{
		[DataMember(Name = "labels", EmitDefaultValue = false)]
		public string[] Labels
		{
			get;
			set;
		}

		[DataMember(Name = "roadShieldType", EmitDefaultValue = false)]
		public int RoadShieldType
		{
			get;
			set;
		}
	}
}
