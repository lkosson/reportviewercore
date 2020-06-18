using System.IO;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class MultiPoint
	{
		public double[] Box = new double[4];

		public int NumPoints;

		public ShapePoint[] Points;

		public void Read(BinaryReader reader)
		{
			Box[0] = reader.ReadDouble();
			Box[1] = reader.ReadDouble();
			Box[2] = reader.ReadDouble();
			Box[3] = reader.ReadDouble();
			NumPoints = reader.ReadInt32();
			Points = new ShapePoint[NumPoints];
			for (int i = 0; i < NumPoints; i++)
			{
				ShapePoint shapePoint = new ShapePoint();
				shapePoint.Read(reader);
				Points[i] = shapePoint;
			}
		}
	}
}
