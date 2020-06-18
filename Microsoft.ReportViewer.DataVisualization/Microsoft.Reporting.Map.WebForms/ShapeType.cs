namespace Microsoft.Reporting.Map.WebForms
{
	internal enum ShapeType
	{
		NullShape = 0,
		Point = 1,
		PolyLine = 3,
		Polygon = 5,
		MultiPoint = 8,
		PointZ = 11,
		PolyLineZ = 13,
		PolygonZ = 0xF,
		MultiPointZ = 18,
		PointM = 21,
		PolyLineM = 23,
		PolygonM = 25,
		MultiPointM = 28,
		MultiPatch = 0x1F
	}
}
