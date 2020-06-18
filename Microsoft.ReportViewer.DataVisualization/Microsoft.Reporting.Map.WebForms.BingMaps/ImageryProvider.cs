using System.Runtime.Serialization;

namespace Microsoft.Reporting.Map.WebForms.BingMaps
{
	[DataContract]
	public class ImageryProvider
	{
		[DataMember(Name = "attribution", EmitDefaultValue = false)]
		public string Attribution
		{
			get;
			set;
		}

		[DataMember(Name = "coverageAreas", EmitDefaultValue = false)]
		public CoverageArea[] CoverageAreas
		{
			get;
			set;
		}
	}
}
