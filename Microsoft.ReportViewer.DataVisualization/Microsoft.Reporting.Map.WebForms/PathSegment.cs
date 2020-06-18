namespace Microsoft.Reporting.Map.WebForms
{
	internal struct PathSegment
	{
		public SegmentType Type;

		public int Length;

		public MapPoint MinimumExtent;

		public MapPoint MaximumExtent;

		public double SegmentLength;
	}
}
