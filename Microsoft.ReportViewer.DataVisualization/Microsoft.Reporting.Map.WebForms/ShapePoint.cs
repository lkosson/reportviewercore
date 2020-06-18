using System.IO;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class ShapePoint
	{
		public double X;

		public double Y;

		public void Read(BinaryReader reader)
		{
			X = reader.ReadDouble();
			Y = reader.ReadDouble();
		}
	}
}
