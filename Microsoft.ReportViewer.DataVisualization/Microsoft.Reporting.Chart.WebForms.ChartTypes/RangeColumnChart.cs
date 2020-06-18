using Microsoft.Reporting.Chart.WebForms.Utilities;
using System;
using System.Drawing;
using System.Globalization;

namespace Microsoft.Reporting.Chart.WebForms.ChartTypes
{
	internal class RangeColumnChart : ColumnChart
	{
		public override string Name => "RangeColumn";

		public override bool ZeroCrossing => true;

		public override int YValuesPerPoint => 2;

		public override bool ExtraYValuesConnectedToYAxis => true;

		public RangeColumnChart()
		{
			useTwoValues = true;
			coordinates = (COPCoordinates.X | COPCoordinates.Y);
			yValueIndex = 1;
		}

		public override double GetYValue(CommonElements common, ChartArea area, Series series, DataPoint point, int pointIndex, int yValueIndex)
		{
			if (yValueIndex == -1)
			{
				return 0.0 - (base.GetYValue(common, area, series, point, pointIndex, 1) - base.GetYValue(common, area, series, point, pointIndex, 0));
			}
			return base.GetYValue(common, area, series, point, pointIndex, yValueIndex);
		}

		protected override void DrawLabel(ChartArea area, ChartGraphics graph, CommonElements common, RectangleF columnPosition, DataPoint point, Series series, int pointIndex)
		{
			RectangleF rectangleF = RectangleF.Intersect(columnPosition, area.PlotAreaPosition.ToRectangleF());
			if (rectangleF.Height <= 0f || rectangleF.Width <= 0f)
			{
				return;
			}
			PointF empty = PointF.Empty;
			empty.X = rectangleF.X + rectangleF.Width / 2f;
			empty.Y = rectangleF.Y;
			point.positionRel = new PointF(empty.X, empty.Y);
			int markerSize = point.MarkerSize;
			string markerImage = point.MarkerImage;
			MarkerStyle markerStyle = point.MarkerStyle;
			SizeF markerSize2 = base.GetMarkerSize(graph, common, area, point, markerSize, markerImage);
			if (markerStyle != 0 || markerImage.Length > 0)
			{
				graph.StartHotRegion(point);
				graph.StartAnimation();
				graph.DrawMarkerRel(empty, (markerStyle == MarkerStyle.None) ? MarkerStyle.Circle : markerStyle, (int)markerSize2.Height, (point.MarkerColor == Color.Empty) ? point.Color : point.MarkerColor, (point.MarkerBorderColor == Color.Empty) ? point.BorderColor : point.MarkerBorderColor, GetMarkerBorderSize(point), markerImage, point.MarkerImageTransparentColor, (point.series != null) ? point.series.ShadowOffset : 0, (point.series != null) ? point.series.ShadowColor : Color.Empty, new RectangleF(empty.X, empty.Y, markerSize2.Width, markerSize2.Height));
				graph.StopAnimation();
				graph.EndHotRegion();
				if (common.ProcessModeRegions)
				{
					SizeF relativeSize = graph.GetRelativeSize(markerSize2);
					int insertIndex = common.HotRegionsList.FindInsertIndex();
					if (markerStyle == MarkerStyle.Circle)
					{
						float[] array = new float[3]
						{
							empty.X,
							empty.Y,
							relativeSize.Width / 2f
						};
						common.HotRegionsList.AddHotRegion(insertIndex, graph, array[0], array[1], array[2], point, series.Name, pointIndex - 1);
					}
					else
					{
						common.HotRegionsList.AddHotRegion(graph, new RectangleF(empty.X - relativeSize.Width / 2f, empty.Y - relativeSize.Height / 2f, relativeSize.Width, relativeSize.Height), point, series.Name, pointIndex - 1);
					}
				}
			}
			StringFormat stringFormat = new StringFormat();
			stringFormat.Alignment = StringAlignment.Center;
			stringFormat.LineAlignment = StringAlignment.Center;
			Region clip = graph.Clip;
			graph.Clip = new Region();
			if (point.ShowLabelAsValue || point.Label.Length > 0)
			{
				string text;
				if (point.Label.Length == 0)
				{
					double yValue = GetYValue(common, area, series, point, pointIndex, 0);
					text = ValueConverter.FormatValue(series.chart, point, yValue, point.LabelFormat, series.YValueType, ChartElementType.DataPoint);
				}
				else
				{
					text = point.ReplaceKeywords(point.Label);
					if (series.chart != null && series.chart.LocalizeTextHandler != null)
					{
						text = series.chart.LocalizeTextHandler(point, text, point.ElementId, ChartElementType.DataPoint);
					}
				}
				PointF empty2 = PointF.Empty;
				empty2.X = rectangleF.X + rectangleF.Width / 2f;
				empty2.Y = rectangleF.Y + rectangleF.Height / 2f;
				graph.StartHotRegion(point, labelRegion: true);
				graph.StartAnimation();
				SizeF relativeSize2 = graph.GetRelativeSize(graph.MeasureString(text, point.Font, new SizeF(1000f, 1000f), new StringFormat(StringFormat.GenericTypographic)));
				RectangleF empty3 = RectangleF.Empty;
				SizeF size = new SizeF(relativeSize2.Width, relativeSize2.Height);
				size.Width += size.Width / (float)text.Length;
				size.Height += relativeSize2.Height / 8f;
				empty3 = PointChart.GetLabelPosition(graph, empty2, size, stringFormat, adjustForDrawing: true);
				graph.DrawPointLabelStringRel(common, text, point.Font, new SolidBrush(point.FontColor), empty2, stringFormat, point.FontAngle, empty3, point.LabelBackColor, point.LabelBorderColor, point.LabelBorderWidth, point.LabelBorderStyle, series, point, pointIndex - 1);
				graph.StopAnimation();
				graph.EndHotRegion();
			}
			graph.Clip = clip;
		}

		protected override void ProcessSinglePoint3D(DataPoint3D pointEx, bool selection, ChartGraphics graph, CommonElements common, ChartArea area, RectangleF columnPosition, int pointIndex)
		{
			DataPoint dataPoint = pointEx.dataPoint;
			if (dataPoint.YValues.Length < YValuesPerPoint)
			{
				throw new InvalidOperationException(SR.ExceptionChartTypeRequiresYValues(Name, YValuesPerPoint.ToString(CultureInfo.InvariantCulture)));
			}
			StringFormat stringFormat = new StringFormat();
			stringFormat.Alignment = StringAlignment.Center;
			stringFormat.LineAlignment = StringAlignment.Center;
			Region clip = graph.Clip;
			graph.Clip = new Region();
			if (dataPoint.ShowLabelAsValue || dataPoint.Label.Length > 0)
			{
				string text;
				if (dataPoint.Label.Length == 0)
				{
					double yValue = GetYValue(common, area, pointEx.dataPoint.series, dataPoint, pointEx.index - 1, 0);
					text = ValueConverter.FormatValue(pointEx.dataPoint.series.chart, dataPoint, yValue, dataPoint.LabelFormat, pointEx.dataPoint.series.YValueType, ChartElementType.DataPoint);
				}
				else
				{
					text = dataPoint.ReplaceKeywords(dataPoint.Label);
					if (pointEx.dataPoint.series.chart != null && pointEx.dataPoint.series.chart.LocalizeTextHandler != null)
					{
						text = pointEx.dataPoint.series.chart.LocalizeTextHandler(dataPoint, text, dataPoint.ElementId, ChartElementType.DataPoint);
					}
				}
				PointF empty = PointF.Empty;
				empty.X = columnPosition.X + columnPosition.Width / 2f;
				empty.Y = columnPosition.Y + columnPosition.Height / 2f;
				Point3D[] array = new Point3D[1]
				{
					new Point3D(empty.X, empty.Y, pointEx.zPosition + pointEx.depth)
				};
				area.matrix3D.TransformPoints(array);
				empty.X = array[0].X;
				empty.Y = array[0].Y;
				graph.StartHotRegion(dataPoint, labelRegion: true);
				SizeF relativeSize = graph.GetRelativeSize(graph.MeasureString(text, dataPoint.Font, new SizeF(1000f, 1000f), new StringFormat(StringFormat.GenericTypographic)));
				RectangleF empty2 = RectangleF.Empty;
				SizeF size = new SizeF(relativeSize.Width, relativeSize.Height);
				size.Width += size.Width / (float)text.Length;
				size.Height += relativeSize.Height / 8f;
				empty2 = PointChart.GetLabelPosition(graph, empty, size, stringFormat, adjustForDrawing: true);
				graph.DrawPointLabelStringRel(common, text, dataPoint.Font, new SolidBrush(dataPoint.FontColor), empty, stringFormat, dataPoint.FontAngle, empty2, dataPoint.LabelBackColor, dataPoint.LabelBorderColor, dataPoint.LabelBorderWidth, dataPoint.LabelBorderStyle, dataPoint.series, dataPoint, pointIndex);
				graph.EndHotRegion();
			}
			graph.Clip = clip;
		}
	}
}
