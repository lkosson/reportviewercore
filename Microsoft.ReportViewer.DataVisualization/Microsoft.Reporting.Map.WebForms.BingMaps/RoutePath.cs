using System.Runtime.Serialization;

namespace Microsoft.Reporting.Map.WebForms.BingMaps
{
	[DataContract]
	public class RoutePath
	{
		[DataMember(Name = "line", EmitDefaultValue = false)]
		public Line Line
		{
			get;
			set;
		}

		[DataMember(Name = "generalizations", EmitDefaultValue = false)]
		public Generalization[] Generalizations
		{
			get;
			set;
		}
	}
}
