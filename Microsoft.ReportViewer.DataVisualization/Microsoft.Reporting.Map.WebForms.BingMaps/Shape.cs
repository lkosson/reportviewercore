using System.Runtime.Serialization;

namespace Microsoft.Reporting.Map.WebForms.BingMaps
{
	[DataContract]
	[KnownType(typeof(Point))]
	public class Shape
	{
		[DataMember(Name = "boundingBox", EmitDefaultValue = false)]
		public double[] BoundingBox
		{
			get;
			set;
		}
	}
}
