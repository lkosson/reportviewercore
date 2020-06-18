using System.Runtime.Serialization;

namespace Microsoft.Reporting.Map.WebForms.BingMaps
{
	[DataContract]
	public class Response
	{
		[DataMember(Name = "copyright", EmitDefaultValue = false)]
		public string Copyright
		{
			get;
			set;
		}

		[DataMember(Name = "brandLogoUri", EmitDefaultValue = false)]
		public string BrandLogoUri
		{
			get;
			set;
		}

		[DataMember(Name = "statusCode", EmitDefaultValue = false)]
		public int StatusCode
		{
			get;
			set;
		}

		[DataMember(Name = "statusDescription", EmitDefaultValue = false)]
		public string StatusDescription
		{
			get;
			set;
		}

		[DataMember(Name = "authenticationResultCode", EmitDefaultValue = false)]
		public string AuthenticationResultCode
		{
			get;
			set;
		}

		[DataMember(Name = "errorDetails", EmitDefaultValue = false)]
		public string[] errorDetails
		{
			get;
			set;
		}

		[DataMember(Name = "traceId", EmitDefaultValue = false)]
		public string TraceId
		{
			get;
			set;
		}

		[DataMember(Name = "resourceSets", EmitDefaultValue = false)]
		public ResourceSet[] ResourceSets
		{
			get;
			set;
		}
	}
}
