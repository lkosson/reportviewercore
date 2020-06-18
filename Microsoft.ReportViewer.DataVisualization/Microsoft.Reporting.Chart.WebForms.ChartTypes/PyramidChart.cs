using System;
using System.Drawing;

namespace Microsoft.Reporting.Chart.WebForms.ChartTypes
{
	internal class PyramidChart : FunnelChart
	{
		public override string Name => "Pyramid";

		public PyramidChart()
		{
			isPyramid = true;
			round3DShape = false;
			funnelLabelStyleAttributeName = "PyramidLabelStyle";
			funnelPointGapAttributeName = "PyramidPointGap";
			funnelRotationAngleAttributeName = "Pyramid3DRotationAngle";
			funnelPointMinHeight = "PyramidMinPointHeight";
			funnel3DDrawingStyleAttributeName = "Pyramid3DDrawingStyle";
			funnelInsideLabelAlignmentAttributeName = "PyramidInsideLabelAlignment";
			funnelOutsideLabelPlacementAttributeName = "PyramidOutsideLabelPlacement";
		}

		protected override void GetPointWidthAndHeight(Series series, int pointIndex, float location, out float height, out float startWidth, out float endWidth)
		{
			PointF empty = PointF.Empty;
			RectangleF absoluteRectangle = graph.GetAbsoluteRectangle(plotAreaPosition);
			float num = absoluteRectangle.Height - funnelSegmentGap * (float)(pointNumber - (ShouldDrawFirstPoint() ? 1 : 2));
			if (num < 0f)
			{
				num = 0f;
			}
			height = (float)((double)num * (GetYValue(series.Points[pointIndex], pointIndex) / yValueTotal));
			height = CheckMinHeight(height);
			PointF linesIntersection = ChartGraphics3D.GetLinesIntersection(absoluteRectangle.X, location - height, absoluteRectangle.Right, location - height, absoluteRectangle.X, absoluteRectangle.Bottom, absoluteRectangle.X + absoluteRectangle.Width / 2f, absoluteRectangle.Y);
			if (linesIntersection.X > absoluteRectangle.X + absoluteRectangle.Width / 2f)
			{
				linesIntersection.X = absoluteRectangle.X + absoluteRectangle.Width / 2f;
			}
			PointF linesIntersection2 = ChartGraphics3D.GetLinesIntersection(absoluteRectangle.X, location, absoluteRectangle.Right, location, absoluteRectangle.X, absoluteRectangle.Bottom, absoluteRectangle.X + absoluteRectangle.Width / 2f, absoluteRectangle.Y);
			if (linesIntersection2.X > absoluteRectangle.X + absoluteRectangle.Width / 2f)
			{
				linesIntersection2.X = absoluteRectangle.X + absoluteRectangle.Width / 2f;
			}
			startWidth = Math.Abs(absoluteRectangle.X + absoluteRectangle.Width / 2f - linesIntersection.X) * 2f;
			endWidth = Math.Abs(absoluteRectangle.X + absoluteRectangle.Width / 2f - linesIntersection2.X) * 2f;
			empty = new PointF(absoluteRectangle.X + absoluteRectangle.Width / 2f, location - height / 2f);
			series.Points[pointIndex].positionRel = graph.GetRelativePoint(empty);
		}
	}
}
