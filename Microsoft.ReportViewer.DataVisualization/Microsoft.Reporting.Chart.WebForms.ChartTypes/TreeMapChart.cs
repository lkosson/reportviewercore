using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace Microsoft.Reporting.Chart.WebForms.ChartTypes
{
	internal sealed class TreeMapChart : IChartType
	{
		private static float ChartAreaMargin = 5f;

		private static float DataPointMargin = 1f;

		public string Name => "TreeMap";

		public bool Stacked => false;

		public bool SupportStackedGroups => false;

		public bool StackSign => false;

		public bool RequireAxes => false;

		public bool SecondYScale => false;

		public bool CircularChartArea => false;

		public bool SupportLogarithmicAxes => false;

		public bool SwitchValueAxes => false;

		public bool SideBySideSeries => false;

		public bool ZeroCrossing => false;

		public bool DataPointsInLegend => false;

		public bool ExtraYValuesConnectedToYAxis => false;

		public bool HundredPercent => false;

		public bool HundredPercentSupportNegative => false;

		public bool ApplyPaletteColorsToPoints => false;

		public int YValuesPerPoint => 1;

		public bool Doughnut => false;

		public Image GetImage(ChartTypeRegistry registry)
		{
			return (Image)registry.ResourceManager.GetObject(Name + "ChartType");
		}

		public LegendImageStyle GetLegendImageStyle(Series series)
		{
			return LegendImageStyle.Rectangle;
		}

		public double GetYValue(CommonElements common, ChartArea area, Series series, DataPoint point, int pointIndex, int yValueIndex)
		{
			return point.YValues[yValueIndex];
		}

		public void AddSmartLabelMarkerPositions(CommonElements common, ChartArea area, Series series, ArrayList list)
		{
		}

		public void Paint(ChartGraphics graph, CommonElements common, ChartArea area, Series seriesToDraw)
		{
			foreach (Series item in common.DataManager.Series)
			{
				if (item.IsVisible() && item.ChartArea == area.Name && string.Compare(item.ChartTypeName, Name, ignoreCase: true, CultureInfo.CurrentCulture) != 0 && !common.ChartPicture.SuppressExceptions)
				{
					throw new InvalidOperationException(SR.ExceptionChartCanNotCombine(Name));
				}
			}
			BuildTreeNodes(common, area, out double chartTotal, out List<TreeMapNode> seriesTreeMapNodes);
			RectangleF plottingArea = GetPlottingArea(graph, area);
			TreeMapSquaringAlgorithm.CalculateRectangles(plottingArea, seriesTreeMapNodes, chartTotal);
			graph.SetClip(graph.GetRelativeRectangle(plottingArea));
			RenderDataPoints(graph, common, seriesTreeMapNodes);
			RenderLabels(graph, area, seriesTreeMapNodes);
			graph.ResetClip();
		}

		private static void BuildTreeNodes(CommonElements common, ChartArea area, out double chartTotal, out List<TreeMapNode> seriesTreeMapNodes)
		{
			chartTotal = 0.0;
			seriesTreeMapNodes = new List<TreeMapNode>();
			foreach (Series item in common.DataManager.Series)
			{
				if (!item.IsVisible() || item.ChartArea != area.Name)
				{
					continue;
				}
				double num = 0.0;
				List<TreeMapNode> list = new List<TreeMapNode>();
				foreach (DataPoint point in item.Points)
				{
					TreeMapNode treeMapNode = new TreeMapNode(point);
					list.Add(treeMapNode);
					num += treeMapNode.Value;
				}
				TreeMapNode treeMapNode2 = new TreeMapNode(item, num);
				treeMapNode2.Children = list;
				seriesTreeMapNodes.Add(treeMapNode2);
				chartTotal += treeMapNode2.Value;
			}
		}

		private static void RenderDataPoints(ChartGraphics graph, CommonElements common, List<TreeMapNode> seriesTreeMapNodes)
		{
			foreach (TreeMapNode seriesTreeMapNode in seriesTreeMapNodes)
			{
				int num = 0;
				foreach (TreeMapNode child in seriesTreeMapNode.Children)
				{
					_ = child.Rectangle;
					RenderDataPoint(graph, common, num, child);
					if (seriesTreeMapNode.DataPoint == null && !child.DataPoint.Empty)
					{
						seriesTreeMapNode.DataPoint = child.DataPoint;
					}
					num++;
				}
			}
		}

		private static void RenderDataPoint(ChartGraphics graph, CommonElements common, int index, TreeMapNode dataPointTreeMapNode)
		{
			RectangleF relativeRect = GetRelativeRect(graph, dataPointTreeMapNode);
			DataPoint dataPoint = dataPointTreeMapNode.DataPoint;
			graph.FillRectangleRel(relativeRect, dataPoint.Color, dataPoint.BackHatchStyle, dataPoint.BackImage, dataPoint.BackImageMode, dataPoint.BackImageTransparentColor, dataPoint.BackImageAlign, dataPoint.BackGradientType, dataPoint.BackGradientEndColor, dataPoint.BorderColor, dataPoint.BorderWidth, dataPoint.BorderStyle, dataPoint.series.ShadowColor, dataPoint.series.ShadowOffset, PenAlignment.Inset, ChartGraphics.GetBarDrawingStyle(dataPoint), isVertical: true);
			AddDataPointHotRegion(graph, common, index, dataPoint, relativeRect);
		}

		private static void AddDataPointHotRegion(ChartGraphics graph, CommonElements common, int index, DataPoint point, RectangleF dataPointRelativeRect)
		{
			common.HotRegionsList.AddHotRegion(graph, dataPointRelativeRect, point, point.series.Name, index);
			point.positionRel = new PointF(dataPointRelativeRect.X + dataPointRelativeRect.Width / 2f, dataPointRelativeRect.Y);
		}

		private static void RenderLabels(ChartGraphics graph, ChartArea area, List<TreeMapNode> seriesTreeMapNodes)
		{
			foreach (TreeMapNode seriesTreeMapNode in seriesTreeMapNodes)
			{
				if (seriesTreeMapNode.DataPoint == null)
				{
					continue;
				}
				RectangleF relativeRect = GetRelativeRect(graph, seriesTreeMapNode);
				RectangleF rectangleF = GetSeriesLabelRelativeRect(graph, area, seriesTreeMapNode.Series, relativeRect, seriesTreeMapNode.DataPoint);
				if (!CanLabelFit(relativeRect, rectangleF))
				{
					rectangleF = RectangleF.Empty;
				}
				int num = 0;
				foreach (TreeMapNode child in seriesTreeMapNode.Children)
				{
					_ = child.DataPoint;
					_ = child.Rectangle;
					RenderDataPointLabel(graph, area, num, child, rectangleF);
					num++;
				}
				RenderSeriesLabel(graph, seriesTreeMapNode, rectangleF);
			}
		}

		private static void RenderSeriesLabel(ChartGraphics graph, TreeMapNode seriesTreeMapNode, RectangleF labelRelativeRect)
		{
			if (!labelRelativeRect.IsEmpty)
			{
				Series series = seriesTreeMapNode.Series;
				DataPoint dataPoint = seriesTreeMapNode.DataPoint;
				using (Font font = GetSeriesLabelFont(dataPoint))
				{
					StringFormat stringFormat = new StringFormat();
					stringFormat.Alignment = StringAlignment.Near;
					stringFormat.LineAlignment = StringAlignment.Near;
					graph.DrawStringRel(series.legendText, font, new SolidBrush(dataPoint.FontColor), labelRelativeRect.Location, stringFormat, 0);
				}
			}
		}

		private static void RenderDataPointLabel(ChartGraphics graph, ChartArea area, int index, TreeMapNode dataPointTreeMapNode, RectangleF seriesLabelRelativeRect)
		{
			RectangleF relativeRect = GetRelativeRect(graph, dataPointTreeMapNode);
			string labelText = GetLabelText(dataPointTreeMapNode.DataPoint);
			RenderDataPointLabel(graph, area, index, dataPointTreeMapNode, labelText, GetDataPointLabelRelativeRect(graph, dataPointTreeMapNode, relativeRect, labelText), relativeRect, seriesLabelRelativeRect);
		}

		private static void RenderDataPointLabel(ChartGraphics graph, ChartArea area, int index, TreeMapNode dataPointTreeMapNode, string text, RectangleF labelRelativeRect, RectangleF dataPointRelativeRect, RectangleF seriesLabelRelativeRect)
		{
			if (!labelRelativeRect.IsEmpty && CanLabelFit(dataPointRelativeRect, labelRelativeRect) && !labelRelativeRect.IntersectsWith(seriesLabelRelativeRect))
			{
				StringFormat stringFormat = new StringFormat();
				stringFormat.Alignment = StringAlignment.Near;
				stringFormat.LineAlignment = StringAlignment.Near;
				DataPoint dataPoint = dataPointTreeMapNode.DataPoint;
				graph.DrawPointLabelStringRel(area.Common, text, dataPoint.Font, new SolidBrush(dataPoint.FontColor), labelRelativeRect.Location, stringFormat, dataPoint.FontAngle, labelRelativeRect, dataPoint.LabelBackColor, dataPoint.LabelBorderColor, dataPoint.LabelBorderWidth, dataPoint.LabelBorderStyle, dataPoint.series, dataPoint, index);
			}
		}

		private static RectangleF GetPlottingArea(ChartGraphics graph, ChartArea area)
		{
			RectangleF absoluteRectangle = graph.GetAbsoluteRectangle(area.Position.ToRectangleF());
			absoluteRectangle.X += ChartAreaMargin;
			absoluteRectangle.Y += ChartAreaMargin;
			absoluteRectangle.Width -= 2f * ChartAreaMargin;
			absoluteRectangle.Height -= 2f * ChartAreaMargin;
			return absoluteRectangle;
		}

		private static RectangleF GetRelativeRect(ChartGraphics graph, TreeMapNode treeMapNode)
		{
			return graph.GetRelativeRectangle(new RectangleF(treeMapNode.Rectangle.X, treeMapNode.Rectangle.Y, treeMapNode.Rectangle.Width - DataPointMargin, treeMapNode.Rectangle.Height - DataPointMargin));
		}

		private static RectangleF GetSeriesLabelRelativeRect(ChartGraphics graph, ChartArea area, Series series, RectangleF seriesRelativeRect, DataPoint point)
		{
			if (string.IsNullOrEmpty(series.legendText) || !IsLabelVisible(point))
			{
				return RectangleF.Empty;
			}
			using (Font font = GetSeriesLabelFont(point))
			{
				return GetLabelRelativeRect(graph, font, seriesRelativeRect, series.legendText, LabelAlignmentTypes.TopLeft);
			}
		}

		private static RectangleF GetDataPointLabelRelativeRect(ChartGraphics graph, TreeMapNode dataPointTreeMapNode, RectangleF dataPointRelativeRect, string text)
		{
			DataPoint dataPoint = dataPointTreeMapNode.DataPoint;
			return GetLabelRelativeRect(graph, dataPoint.Font, dataPointRelativeRect, text, GetLabelAlignment(dataPoint));
		}

		private static RectangleF GetLabelRelativeRect(ChartGraphics graph, Font font, RectangleF treeMapNodeRelativeRect, string text, LabelAlignmentTypes labelAlignment)
		{
			if (string.IsNullOrEmpty(text))
			{
				return RectangleF.Empty;
			}
			SizeF relativeSize = graph.GetRelativeSize(graph.MeasureString(text.Replace("\\n", "\n"), font, new SizeF(1000f, 1000f), new StringFormat(StringFormat.GenericTypographic)));
			float num = relativeSize.Width + relativeSize.Width / (float)text.Length;
			float num2 = relativeSize.Height + relativeSize.Height / 8f;
			return new RectangleF(GetLabelXPosition(treeMapNodeRelativeRect, num, labelAlignment), GetLabelYPosition(treeMapNodeRelativeRect, num2, labelAlignment), num, num2);
		}

		private static bool CanLabelFit(RectangleF containerRelativeRect, RectangleF labelRelativeRect)
		{
			if (labelRelativeRect.Width <= containerRelativeRect.Width)
			{
				return labelRelativeRect.Height <= containerRelativeRect.Height;
			}
			return false;
		}

		private static Font GetSeriesLabelFont(DataPoint point)
		{
			return new Font(point.Font, point.Font.Style | FontStyle.Bold);
		}

		public static bool IsLabelVisible(DataPointAttributes point)
		{
			if (point.IsAttributeSet("LabelsVisible"))
			{
				return !string.Equals(point.GetAttribute("LabelsVisible"), "false", StringComparison.OrdinalIgnoreCase);
			}
			return true;
		}

		private static float GetLabelXPosition(RectangleF treeMapNodeRelativeRect, float labelRelativeWidth, LabelAlignmentTypes labelAlignment)
		{
			float result = 0f;
			switch (labelAlignment)
			{
			case LabelAlignmentTypes.Top:
			case LabelAlignmentTypes.Bottom:
			case LabelAlignmentTypes.Center:
				result = treeMapNodeRelativeRect.X + (treeMapNodeRelativeRect.Width - labelRelativeWidth) / 2f;
				break;
			case LabelAlignmentTypes.Left:
			case LabelAlignmentTypes.TopLeft:
			case LabelAlignmentTypes.BottomLeft:
				result = treeMapNodeRelativeRect.X;
				break;
			case LabelAlignmentTypes.Right:
			case LabelAlignmentTypes.TopRight:
			case LabelAlignmentTypes.BottomRight:
				result = treeMapNodeRelativeRect.X + treeMapNodeRelativeRect.Width - labelRelativeWidth;
				break;
			}
			return result;
		}

		private static float GetLabelYPosition(RectangleF treeMapNodeRelativeRect, float labelRelativeHeight, LabelAlignmentTypes labelAlignment)
		{
			float result = 0f;
			switch (labelAlignment)
			{
			case LabelAlignmentTypes.Bottom:
			case LabelAlignmentTypes.BottomLeft:
			case LabelAlignmentTypes.BottomRight:
				result = treeMapNodeRelativeRect.Y + treeMapNodeRelativeRect.Height - labelRelativeHeight;
				break;
			case LabelAlignmentTypes.Right:
			case LabelAlignmentTypes.Left:
			case LabelAlignmentTypes.Center:
				result = treeMapNodeRelativeRect.Y + (treeMapNodeRelativeRect.Height - labelRelativeHeight) / 2f;
				break;
			case LabelAlignmentTypes.Top:
			case LabelAlignmentTypes.TopLeft:
			case LabelAlignmentTypes.TopRight:
				result = treeMapNodeRelativeRect.Y;
				break;
			}
			return result;
		}

		private static LabelAlignmentTypes GetLabelAlignment(DataPoint point)
		{
			string text = point["LabelStyle"];
			if (text != null && text.Length > 0)
			{
				if (string.Compare(text, "Center", StringComparison.OrdinalIgnoreCase) == 0)
				{
					return LabelAlignmentTypes.Center;
				}
				if (string.Compare(text, "Bottom", StringComparison.OrdinalIgnoreCase) == 0)
				{
					return LabelAlignmentTypes.Bottom;
				}
				if (string.Compare(text, "TopLeft", StringComparison.OrdinalIgnoreCase) == 0)
				{
					return LabelAlignmentTypes.TopLeft;
				}
				if (string.Compare(text, "TopRight", StringComparison.OrdinalIgnoreCase) == 0)
				{
					return LabelAlignmentTypes.TopRight;
				}
				if (string.Compare(text, "BottomLeft", StringComparison.OrdinalIgnoreCase) == 0)
				{
					return LabelAlignmentTypes.BottomLeft;
				}
				if (string.Compare(text, "BottomRight", StringComparison.OrdinalIgnoreCase) == 0)
				{
					return LabelAlignmentTypes.BottomRight;
				}
				if (string.Compare(text, "Left", StringComparison.OrdinalIgnoreCase) == 0)
				{
					return LabelAlignmentTypes.Left;
				}
				if (string.Compare(text, "Right", StringComparison.OrdinalIgnoreCase) == 0)
				{
					return LabelAlignmentTypes.Right;
				}
				if (string.Compare(text, "Top", StringComparison.OrdinalIgnoreCase) == 0)
				{
					return LabelAlignmentTypes.Top;
				}
			}
			return LabelAlignmentTypes.BottomLeft;
		}

		private static string GetLabelText(DataPoint point)
		{
			if (!IsLabelVisible(point))
			{
				return string.Empty;
			}
			return PieChart.GetLabelText(point, alwaysIncludeAxisLabel: true);
		}
	}
}
