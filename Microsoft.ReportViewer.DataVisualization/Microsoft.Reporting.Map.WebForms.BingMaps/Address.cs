using System.Runtime.Serialization;

namespace Microsoft.Reporting.Map.WebForms.BingMaps
{
	[DataContract]
	public class Address
	{
		[DataMember(Name = "addressLine", EmitDefaultValue = false)]
		public string AddressLine
		{
			get;
			set;
		}

		[DataMember(Name = "adminDistrict", EmitDefaultValue = false)]
		public string AdminDistrict
		{
			get;
			set;
		}

		[DataMember(Name = "adminDistrict2", EmitDefaultValue = false)]
		public string AdminDistrict2
		{
			get;
			set;
		}

		[DataMember(Name = "countryRegion", EmitDefaultValue = false)]
		public string CountryRegion
		{
			get;
			set;
		}

		[DataMember(Name = "locality", EmitDefaultValue = false)]
		public string Locality
		{
			get;
			set;
		}

		[DataMember(Name = "postalCode", EmitDefaultValue = false)]
		public string PostalCode
		{
			get;
			set;
		}

		[DataMember(Name = "countryRegionIso2", EmitDefaultValue = false)]
		public string CountryRegionIso2
		{
			get;
			set;
		}

		[DataMember(Name = "formattedAddress", EmitDefaultValue = false)]
		public string FormattedAddress
		{
			get;
			set;
		}

		[DataMember(Name = "neighborhood", EmitDefaultValue = false)]
		public string Neighborhood
		{
			get;
			set;
		}

		[DataMember(Name = "landmark", EmitDefaultValue = false)]
		public string Landmark
		{
			get;
			set;
		}
	}
}
