using System.Runtime.Serialization;

namespace Microsoft.Reporting.Map.WebForms.BingMaps
{
	[DataContract(Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1")]
	public class Location : Resource
	{
		[DataMember(Name = "name", EmitDefaultValue = false)]
		public string Name
		{
			get;
			set;
		}

		[DataMember(Name = "point", EmitDefaultValue = false)]
		public Point Point
		{
			get;
			set;
		}

		[DataMember(Name = "entityType", EmitDefaultValue = false)]
		public string EntityType
		{
			get;
			set;
		}

		[DataMember(Name = "address", EmitDefaultValue = false)]
		public Address Address
		{
			get;
			set;
		}

		[DataMember(Name = "confidence", EmitDefaultValue = false)]
		public string Confidence
		{
			get;
			set;
		}

		[DataMember(Name = "matchCodes", EmitDefaultValue = false)]
		public string[] MatchCodes
		{
			get;
			set;
		}

		[DataMember(Name = "geocodePoints", EmitDefaultValue = false)]
		public Point[] GeocodePoints
		{
			get;
			set;
		}

		[DataMember(Name = "queryParseValues", EmitDefaultValue = false)]
		public QueryParseValue[] QueryParseValues
		{
			get;
			set;
		}
	}
}
