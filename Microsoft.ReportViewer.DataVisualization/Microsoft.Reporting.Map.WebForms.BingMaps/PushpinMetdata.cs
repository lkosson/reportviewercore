using System.Runtime.Serialization;

namespace Microsoft.Reporting.Map.WebForms.BingMaps
{
	[DataContract(Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1")]
	public class PushpinMetdata
	{
		[DataMember(Name = "anchor", EmitDefaultValue = false)]
		public Pixel Anchor
		{
			get;
			set;
		}

		[DataMember(Name = "bottomRightOffset", EmitDefaultValue = false)]
		public Pixel BottomRightOffset
		{
			get;
			set;
		}

		[DataMember(Name = "topLeftOffset", EmitDefaultValue = false)]
		public Pixel TopLeftOffset
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
	}
}
