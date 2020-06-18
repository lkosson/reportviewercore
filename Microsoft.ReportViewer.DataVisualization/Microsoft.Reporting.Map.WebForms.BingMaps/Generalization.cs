using System.Runtime.Serialization;

namespace Microsoft.Reporting.Map.WebForms.BingMaps
{
	[DataContract]
	public class Generalization
	{
		[DataMember(Name = "pathIndices", EmitDefaultValue = false)]
		public int[] PathIndices
		{
			get;
			set;
		}

		[DataMember(Name = "latLongTolerance", EmitDefaultValue = false)]
		public double LatLongTolerance
		{
			get;
			set;
		}
	}
}
