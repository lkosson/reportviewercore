using System.Runtime.Serialization;

namespace Microsoft.Reporting.Map.WebForms.BingMaps
{
	[DataContract]
	public class TransitLine
	{
		[DataMember(Name = "verboseName", EmitDefaultValue = false)]
		public string VerboseName
		{
			get;
			set;
		}

		[DataMember(Name = "abbreviatedName", EmitDefaultValue = false)]
		public string AbbreviatedName
		{
			get;
			set;
		}

		[DataMember(Name = "agencyId", EmitDefaultValue = false)]
		public long AgencyId
		{
			get;
			set;
		}

		[DataMember(Name = "agencyName", EmitDefaultValue = false)]
		public string AgencyName
		{
			get;
			set;
		}

		[DataMember(Name = "lineColor", EmitDefaultValue = false)]
		public long LineColor
		{
			get;
			set;
		}

		[DataMember(Name = "lineTextColor", EmitDefaultValue = false)]
		public long LineTextColor
		{
			get;
			set;
		}

		[DataMember(Name = "uri", EmitDefaultValue = false)]
		public string Uri
		{
			get;
			set;
		}

		[DataMember(Name = "phoneNumber", EmitDefaultValue = false)]
		public string PhoneNumber
		{
			get;
			set;
		}

		[DataMember(Name = "providerInfo", EmitDefaultValue = false)]
		public string ProviderInfo
		{
			get;
			set;
		}
	}
}
