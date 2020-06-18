using System.Runtime.Serialization;

namespace Microsoft.Reporting.Map.WebForms.BingMaps
{
	[DataContract]
	public class RoadShield
	{
		[DataMember(Name = "bucket", EmitDefaultValue = false)]
		public int Bucket
		{
			get;
			set;
		}

		[DataMember(Name = "shields", EmitDefaultValue = false)]
		public Shield[] Shields
		{
			get;
			set;
		}
	}
}
