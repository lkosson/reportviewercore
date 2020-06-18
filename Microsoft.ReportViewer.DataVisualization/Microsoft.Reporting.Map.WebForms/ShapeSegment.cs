namespace Microsoft.Reporting.Map.WebForms
{
	internal struct ShapeSegment
	{
		public SegmentType Type;

		public int Length;

		public MapPoint MinimumExtent;

		public MapPoint MaximumExtent;

		public double PolygonSignedArea;

		public MapPoint PolygonCentroid;
	}
}
