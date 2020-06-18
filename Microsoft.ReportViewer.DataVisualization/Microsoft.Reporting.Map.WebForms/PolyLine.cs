using System.IO;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class PolyLine
	{
		public double[] Box = new double[4];

		public int NumParts;

		public int NumPoints;

		public int[] Parts;

		public ShapePoint[] Points;

		public void Read(BinaryReader reader)
		{
			Box[0] = reader.ReadDouble();
			Box[1] = reader.ReadDouble();
			Box[2] = reader.ReadDouble();
			Box[3] = reader.ReadDouble();
			NumParts = reader.ReadInt32();
			NumPoints = reader.ReadInt32();
			Parts = new int[NumParts];
			for (int i = 0; i < NumParts; i++)
			{
				Parts[i] = reader.ReadInt32();
			}
			Points = new ShapePoint[NumPoints];
			for (int j = 0; j < NumPoints; j++)
			{
				ShapePoint shapePoint = new ShapePoint();
				shapePoint.Read(reader);
				Points[j] = shapePoint;
			}
		}
	}
}
