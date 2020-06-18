using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Microsoft.Reporting.Chart.WebForms.ChartTypes
{
	internal class StepLineChart : LineChart
	{
		public override string Name => "StepLine";

		public override Image GetImage(ChartTypeRegistry registry)
		{
			return (Image)registry.ResourceManager.GetObject(Name + "ChartType");
		}

		protected override void DrawLine(ChartGraphics graph, CommonElements common, DataPoint point, Series series, PointF[] points, int pointIndex, float tension)
		{
			if (pointIndex <= 0)
			{
				return;
			}
			PointF pointF = points[pointIndex - 1];
			PointF pointF2 = new PointF(points[pointIndex].X, points[pointIndex - 1].Y);
			PointF pointF3 = points[pointIndex];
			graph.DrawLineRel(point.Color, point.BorderWidth, point.BorderStyle, graph.GetRelativePoint(pointF), graph.GetRelativePoint(pointF2), series.ShadowColor, series.ShadowOffset);
			graph.DrawLineRel(point.Color, point.BorderWidth, point.BorderStyle, graph.GetRelativePoint(pointF2), graph.GetRelativePoint(pointF3), series.ShadowColor, series.ShadowOffset);
			if (common.ProcessModeRegions)
			{
				GraphicsPath graphicsPath = new GraphicsPath();
				graphicsPath.AddLine(pointF2, pointF3);
				if (!pointF2.Equals(pointF3))
				{
					ChartGraphics.Widen(graphicsPath, new Pen(point.Color, point.BorderWidth + 2));
				}
				PointF empty = PointF.Empty;
				float[] array = new float[graphicsPath.PointCount * 2];
				PointF[] pathPoints = graphicsPath.PathPoints;
				for (int i = 0; i < graphicsPath.PointCount; i++)
				{
					empty = graph.GetRelativePoint(pathPoints[i]);
					array[2 * i] = empty.X;
					array[2 * i + 1] = empty.Y;
				}
				common.HotRegionsList.AddHotRegion(graph, graphicsPath, relativePath: false, array, point, series.Name, pointIndex);
				graphicsPath = new GraphicsPath();
				graphicsPath.AddLine(pointF, pointF2);
				if (!pointF.Equals(pointF2))
				{
					ChartGraphics.Widen(graphicsPath, new Pen(point.Color, point.BorderWidth + 2));
				}
				array = new float[graphicsPath.PointCount * 2];
				pathPoints = graphicsPath.PathPoints;
				for (int j = 0; j < graphicsPath.PointCount; j++)
				{
					empty = graph.GetRelativePoint(pathPoints[j]);
					array[2 * j] = empty.X;
					array[2 * j + 1] = empty.Y;
				}
				common.HotRegionsList.AddHotRegion(graph, graphicsPath, relativePath: false, array, series.Points[pointIndex - 1], series.Name, pointIndex - 1);
			}
		}

		protected override GraphicsPath Draw3DSurface(ChartArea area, ChartGraphics graph, Matrix3D matrix, LightStyle lightStyle, DataPoint3D prevDataPointEx, float positionZ, float depth, ArrayList points, int pointIndex, int pointLoopIndex, float tension, DrawingOperationTypes operationType, float topDarkening, float bottomDarkening, PointF thirdPointPosition, PointF fourthPointPosition, bool clippedSegment)
		{
			GraphicsPath graphicsPath = ((operationType & DrawingOperationTypes.CalcElementPath) == DrawingOperationTypes.CalcElementPath) ? new GraphicsPath() : null;
			if (centerPointIndex == int.MaxValue)
			{
				centerPointIndex = GetCenterPointIndex(points);
			}
			DataPoint3D dataPoint3D = (DataPoint3D)points[pointIndex];
			int neighborPointIndex = pointIndex;
			DataPoint3D dataPoint3D2 = ChartGraphics3D.FindPointByIndex(points, dataPoint3D.index - 1, multiSeries ? dataPoint3D : null, ref neighborPointIndex);
			DataPoint3D dataPoint3D3 = dataPoint3D;
			if (prevDataPointEx.dataPoint.Empty)
			{
				dataPoint3D3 = prevDataPointEx;
			}
			else if (dataPoint3D2.index > dataPoint3D.index)
			{
				dataPoint3D3 = dataPoint3D2;
			}
			Color backColor = useBorderColor ? dataPoint3D3.dataPoint.BorderColor : dataPoint3D3.dataPoint.Color;
			ChartDashStyle borderStyle = dataPoint3D3.dataPoint.BorderStyle;
			if (dataPoint3D3.dataPoint.Empty && dataPoint3D3.dataPoint.Color == Color.Empty)
			{
				backColor = Color.Gray;
			}
			if (dataPoint3D3.dataPoint.Empty && dataPoint3D3.dataPoint.BorderStyle == ChartDashStyle.NotSet)
			{
				borderStyle = ChartDashStyle.Solid;
			}
			DataPoint3D dataPoint3D4 = new DataPoint3D();
			dataPoint3D4.xPosition = dataPoint3D.xPosition;
			dataPoint3D4.yPosition = dataPoint3D2.yPosition;
			bool flag = true;
			if (pointIndex + 1 < points.Count && ((DataPoint3D)points[pointIndex + 1]).index == dataPoint3D2.index)
			{
				flag = false;
			}
			if (centerPointIndex != int.MaxValue && pointIndex >= centerPointIndex)
			{
				flag = false;
			}
			GraphicsPath graphicsPath2;
			GraphicsPath graphicsPath3;
			if (flag)
			{
				dataPoint3D4.dataPoint = dataPoint3D.dataPoint;
				graphicsPath2 = graph.Draw3DSurface(area, matrix, lightStyle, SurfaceNames.Top, positionZ, depth, backColor, dataPoint3D3.dataPoint.BorderColor, dataPoint3D3.dataPoint.BorderWidth, borderStyle, dataPoint3D2, dataPoint3D4, points, pointIndex, 0f, operationType, LineSegmentType.First, showPointLines ? true : false, forceThickBorder: false, area.reverseSeriesOrder, multiSeries, 0, clipInsideArea: true);
				graph.frontLinePen = null;
				dataPoint3D4.dataPoint = dataPoint3D2.dataPoint;
				graphicsPath3 = graph.Draw3DSurface(area, matrix, lightStyle, SurfaceNames.Top, positionZ, depth, backColor, dataPoint3D3.dataPoint.BorderColor, dataPoint3D3.dataPoint.BorderWidth, borderStyle, dataPoint3D4, dataPoint3D, points, pointIndex, 0f, operationType, LineSegmentType.Last, showPointLines ? true : false, forceThickBorder: false, area.reverseSeriesOrder, multiSeries, 0, clipInsideArea: true);
				graph.frontLinePen = null;
			}
			else
			{
				dataPoint3D4.dataPoint = dataPoint3D2.dataPoint;
				graphicsPath3 = graph.Draw3DSurface(area, matrix, lightStyle, SurfaceNames.Top, positionZ, depth, backColor, dataPoint3D3.dataPoint.BorderColor, dataPoint3D3.dataPoint.BorderWidth, borderStyle, dataPoint3D4, dataPoint3D, points, pointIndex, 0f, operationType, LineSegmentType.Last, showPointLines ? true : false, forceThickBorder: false, area.reverseSeriesOrder, multiSeries, 0, clipInsideArea: true);
				graph.frontLinePen = null;
				dataPoint3D4.dataPoint = dataPoint3D.dataPoint;
				graphicsPath2 = graph.Draw3DSurface(area, matrix, lightStyle, SurfaceNames.Top, positionZ, depth, backColor, dataPoint3D3.dataPoint.BorderColor, dataPoint3D3.dataPoint.BorderWidth, borderStyle, dataPoint3D2, dataPoint3D4, points, pointIndex, 0f, operationType, LineSegmentType.First, showPointLines ? true : false, forceThickBorder: false, area.reverseSeriesOrder, multiSeries, 0, clipInsideArea: true);
				graph.frontLinePen = null;
			}
			if (graphicsPath != null)
			{
				if (area.Common.ProcessModeRegions && graphicsPath2 != null && graphicsPath2.PointCount > 0)
				{
					area.Common.HotRegionsList.AddHotRegion(graphicsPath2, relativePath: false, graph, prevDataPointEx.dataPoint, prevDataPointEx.dataPoint.series.Name, prevDataPointEx.index - 1);
				}
				if (graphicsPath3 != null && graphicsPath3.PointCount > 0)
				{
					graphicsPath.AddPath(graphicsPath3, connect: true);
				}
			}
			return graphicsPath;
		}
	}
}
