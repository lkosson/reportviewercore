using System.Runtime.Serialization;

namespace Microsoft.Reporting.Map.WebForms.BingMaps
{
	[DataContract]
	public class Instruction
	{
		[DataMember(Name = "maneuverType", EmitDefaultValue = false)]
		public string ManeuverType
		{
			get;
			set;
		}

		[DataMember(Name = "text", EmitDefaultValue = false)]
		public string Text
		{
			get;
			set;
		}

		[DataMember(Name = "formattedText", EmitDefaultValue = false)]
		public string FormattedText
		{
			get;
			set;
		}
	}
}
