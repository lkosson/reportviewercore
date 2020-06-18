using System.Runtime.Serialization;

namespace Microsoft.Reporting.Map.WebForms.BingMaps
{
	[DataContract(Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1")]
	public class SeaLevelData : Resource
	{
		[DataMember(Name = "offsets", EmitDefaultValue = false)]
		public int[] Offsets
		{
			get;
			set;
		}

		[DataMember(Name = "zoomLevel", EmitDefaultValue = false)]
		public int ZoomLevel
		{
			get;
			set;
		}
	}
}
