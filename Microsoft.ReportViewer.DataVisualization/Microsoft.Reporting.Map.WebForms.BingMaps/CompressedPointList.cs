using System.Runtime.Serialization;

namespace Microsoft.Reporting.Map.WebForms.BingMaps
{
	[DataContract(Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1")]
	public class CompressedPointList : Resource
	{
		[DataMember(Name = "value", EmitDefaultValue = false)]
		public string Value
		{
			get;
			set;
		}
	}
}
