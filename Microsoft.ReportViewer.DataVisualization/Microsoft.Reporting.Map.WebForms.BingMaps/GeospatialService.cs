using System.Runtime.Serialization;

namespace Microsoft.Reporting.Map.WebForms.BingMaps
{
	[DataContract]
	public class GeospatialService
	{
		[DataMember(Name = "endpoint", EmitDefaultValue = false)]
		public string Endpoint
		{
			get;
			set;
		}

		[DataMember(Name = "fallbackLanguage", EmitDefaultValue = false)]
		public string FallbackLanguage
		{
			get;
			set;
		}

		[DataMember(Name = "languageSupported", EmitDefaultValue = false)]
		public bool LanguageSupported
		{
			get;
			set;
		}

		[DataMember(Name = "serviceName", EmitDefaultValue = false)]
		public string ServiceName
		{
			get;
			set;
		}
	}
}
