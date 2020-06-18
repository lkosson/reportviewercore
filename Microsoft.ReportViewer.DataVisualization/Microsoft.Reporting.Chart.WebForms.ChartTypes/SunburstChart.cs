using Microsoft.Reporting.Chart.WebForms.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace Microsoft.Reporting.Chart.WebForms.ChartTypes
{
	internal sealed class SunburstChart : IChartType
	{
		private static StringFormat format;

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

		static SunburstChart()
		{
			format = new StringFormat();
			format.Alignment = StringAlignment.Center;
			format.LineAlignment = StringAlignment.Center;
			format.FormatFlags = (StringFormatFlags.FitBlackBox | StringFormatFlags.NoClip);
			format.Trimming = StringTrimming.None;
		}

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
			RectangleF plottingAreaRelative = GetPlottingAreaRelative(graph, area);
			graph.SetClip(plottingAreaRelative);
			RenderNodes(common, graph, area, plottingAreaRelative);
			graph.ResetClip();
		}

		private static RectangleF GetPlottingAreaRelative(ChartGraphics graph, ChartArea area)
		{
			RectangleF rectangleF = area.InnerPlotPosition.Auto ? new RectangleF(area.Position.ToRectangleF().X, area.Position.ToRectangleF().Y, area.Position.ToRectangleF().Width, area.Position.ToRectangleF().Height) : new RectangleF(area.PlotAreaPosition.ToRectangleF().X, area.PlotAreaPosition.ToRectangleF().Y, area.PlotAreaPosition.ToRectangleF().Width, area.PlotAreaPosition.ToRectangleF().Height);
			SizeF absoluteSize = graph.GetAbsoluteSize(new SizeF(rectangleF.Width, rectangleF.Height));
			float num = (absoluteSize.Width < absoluteSize.Height) ? absoluteSize.Width : absoluteSize.Height;
			SizeF relativeSize = graph.GetRelativeSize(new SizeF(num, num));
			PointF pointF = new PointF(rectangleF.X + rectangleF.Width / 2f, rectangleF.Y + rectangleF.Height / 2f);
			return new RectangleF(pointF.X - relativeSize.Width / 2f, pointF.Y - relativeSize.Height / 2f, relativeSize.Width, relativeSize.Height);
		}

		private static void RenderNodes(CommonElements common, ChartGraphics graph, ChartArea area, RectangleF plottingAreaRelative)
		{
			if (!(plottingAreaRelative.Width < 0f) && !(plottingAreaRelative.Height < 0f))
			{
				CategoryNodeCollection categoryNodes = area.CategoryNodes;
				if (categoryNodes != null)
				{
					List<Series> chartAreaSeries = GetChartAreaSeries(area.Name, common.DataManager.Series);
					categoryNodes.Calculate(chartAreaSeries);
					double totalAbsoluetValue = categoryNodes.GetTotalAbsoluetValue();
					SortSeriesByAbsoluteValue(chartAreaSeries, categoryNodes);
					int num = 2 * (categoryNodes.GetDepth() + 1);
					float num2 = plottingAreaRelative.Width / (float)num;
					float num3 = plottingAreaRelative.Height / (float)num;
					float num4 = num2 * 4f;
					float num5 = num3 * 4f;
					RectangleF rectRelative = new RectangleF(plottingAreaRelative.X + plottingAreaRelative.Width / 2f - num4 / 2f, plottingAreaRelative.Y + plottingAreaRelative.Height / 2f - num5 / 2f, num4, num5);
					RenderNodes(common, graph, categoryNodes, rectRelative, totalAbsoluetValue, num2, num3, chartAreaSeries);
				}
			}
		}

		private static void RenderNodes(CommonElements common, ChartGraphics graph, CategoryNodeCollection nodes, RectangleF rectRelative, double chartTotal, float incrementXRelative, float incrementYRelative, List<Series> seriesCollection)
		{
			float startAngle = 270f;
			foreach (Series item in seriesCollection)
			{
				RenderNodes(common, graph, nodes, rectRelative, 1, chartTotal, ref startAngle, 360f, incrementXRelative, incrementYRelative, item, GetFirstNonEmptyDataPointsAttributes(item));
			}
		}

		private static DataPointAttributes GetFirstNonEmptyDataPointsAttributes(Series series)
		{
			foreach (DataPoint point in series.Points)
			{
				if (!point.Empty)
				{
					return point;
				}
			}
			return series;
		}

		private static void RenderNodes(CommonElements common, ChartGraphics graph, CategoryNodeCollection nodes, RectangleF rectRelative, int level, double parentValue, ref float startAngle, float parentSweepAngle, float incrementXRelative, float incrementYRelative, Series series, DataPointAttributes dataPointAttributes)
		{
			if (nodes.AreAllNodesEmpty(series))
			{
				return;
			}
			nodes.SortByAbsoluteValue(series);
			RectangleF absoluteRectangle = graph.GetAbsoluteRectangle(rectRelative);
			float thresholdAngle = (float)(360.0 / (Math.PI * 2.0 * (double)absoluteRectangle.Width));
			PointF centerAbsolute = new PointF(absoluteRectangle.X + absoluteRectangle.Width / 2f, absoluteRectangle.Y + absoluteRectangle.Height / 2f);
			float centerRadiusAbsolute = graph.GetAbsoluteWidth(rectRelative.Width - incrementXRelative) / 2f;
			float absoluteWidth = graph.GetAbsoluteWidth(rectRelative.Width / 2f);
			foreach (CategoryNode node in nodes)
			{
				RenderNode(common, graph, node, rectRelative, level, parentValue, ref startAngle, parentSweepAngle, thresholdAngle, incrementXRelative, incrementYRelative, centerAbsolute, centerRadiusAbsolute, absoluteWidth, series, dataPointAttributes);
			}
		}

		private static void RenderNode(CommonElements common, ChartGraphics graph, CategoryNode node, RectangleF rectRelative, int level, double parentValue, ref float startAngle, float parentSweepAngle, float thresholdAngle, float incrementXRelative, float incrementYRelative, PointF centerAbsolute, float centerRadiusAbsolute, float edgeRadiusAbsolute, Series series, DataPointAttributes dataPointAttributes)
		{
			double absoluteValue = node.GetValues(series).AbsoluteValue;
			if (absoluteValue != 0.0)
			{
				CategoryNode dataPointNode = node.GetDataPointNode(series);
				DataPoint dataPoint;
				int dataPointIndex;
				if (dataPointNode != null)
				{
					dataPoint = dataPointNode.GetDataPoint(series);
					dataPointIndex = dataPointNode.Index;
				}
				else
				{
					dataPoint = null;
					dataPointIndex = -1;
				}
				DataPointAttributes dataPointAttributes2 = (dataPoint != null) ? dataPoint : dataPointAttributes;
				float num = (float)(absoluteValue / parentValue * (double)parentSweepAngle);
				float sweepAngle = num - thresholdAngle;
				using (GraphicsPath sliceGraphicsPath = RenderSlice(common, graph, node, dataPoint, dataPointAttributes2, rectRelative, startAngle, sweepAngle, centerAbsolute, edgeRadiusAbsolute, level, dataPointIndex))
				{
					RenderLabel(common, graph, node, dataPoint, dataPointAttributes2, GetLabelText(node, dataPoint, series, dataPointAttributes2), startAngle, num, centerAbsolute, centerRadiusAbsolute, dataPointIndex, sliceGraphicsPath);
				}
				if (node.Children != null)
				{
					float startAngle2 = startAngle;
					RenderNodes(common, graph, node.Children, RectangleF.Inflate(rectRelative, incrementXRelative, incrementYRelative), level + 1, absoluteValue, ref startAngle2, num, incrementXRelative, incrementYRelative, series, dataPointAttributes);
				}
				startAngle += num;
			}
		}

		private static string GetLabelText(CategoryNode categoryNode, DataPoint dataPoint, Series series, DataPointAttributes dataPointAttributes)
		{
			if (dataPoint != null)
			{
				if (TreeMapChart.IsLabelVisible(dataPoint))
				{
					string labelText = PieChart.GetLabelText(dataPoint);
					if (!string.IsNullOrEmpty(labelText))
					{
						return labelText;
					}
					return GetCategoryNodeLabelText(categoryNode, series, dataPoint);
				}
			}
			else if (TreeMapChart.IsLabelVisible(dataPointAttributes))
			{
				return GetCategoryNodeLabelText(categoryNode, series, dataPointAttributes);
			}
			return string.Empty;
		}

		private static string GetCategoryNodeLabelText(CategoryNode categoryNode, Series series, DataPointAttributes dataPointAttributes)
		{
			if (dataPointAttributes.ShowLabelAsValue)
			{
				return ValueConverter.FormatValue(series.chart, null, categoryNode.GetValues(series).Value, dataPointAttributes.LabelFormat, series.YValueType, ChartElementType.DataPoint);
			}
			return categoryNode.Label;
		}

		private static GraphicsPath RenderSlice(CommonElements common, ChartGraphics graph, CategoryNode node, DataPoint dataPoint, DataPointAttributes dataPointAttributes, RectangleF rectRelative, float startAngle, float sweepAngle, PointF centerAbsolute, float radiusAbsolute, int level, int dataPointIndex)
		{
			float doughnutRadius = 1f / (float)(level + 1) * 100f;
			GraphicsPath controlGraphicsPath = null;
			graph.DrawPieRel(rectRelative, startAngle, sweepAngle, dataPointAttributes.Color, dataPointAttributes.BackHatchStyle, dataPointAttributes.BackImage, dataPointAttributes.BackImageMode, dataPointAttributes.BackImageTransparentColor, dataPointAttributes.BackImageAlign, dataPointAttributes.BackGradientType, dataPointAttributes.BackGradientEndColor, dataPointAttributes.BorderColor, dataPointAttributes.BorderWidth, dataPointAttributes.BorderStyle, PenAlignment.Inset, shadow: false, 0.0, doughnut: true, doughnutRadius, explodedShadow: false, PieDrawingStyle.Default, out controlGraphicsPath);
			if (dataPoint != null)
			{
				PieChart.Map(common, dataPoint, startAngle, sweepAngle, rectRelative, doughnut: true, doughnutRadius, graph, dataPointIndex);
				dataPoint.positionRel = GetSliceCenterRelative(graph, GetSliceCenterAngle(startAngle, sweepAngle), centerAbsolute, radiusAbsolute);
			}
			else
			{
				MapCategoryNode(common, node, startAngle, sweepAngle, rectRelative, doughnutRadius, graph);
			}
			return controlGraphicsPath;
		}

		public static void MapCategoryNode(CommonElements common, CategoryNode node, float startAngle, float sweepAngle, RectangleF rectangle, float doughnutRadius, ChartGraphics graph)
		{
			if (PieChart.CreateMapAreaPath(startAngle, sweepAngle, rectangle, doughnut: true, doughnutRadius, graph, out GraphicsPath path, out float[] _))
			{
				common.HotRegionsList.AddHotRegion(graph, path, relativePath: false, node.ToolTip, node.Href, "", node, ChartElementType.Nothing);
			}
		}

		private static float GetSliceCenterAngle(float startAngle, float sweepAngle)
		{
			return startAngle + sweepAngle / 2f;
		}

		private static float NormalizeAngle(float angle)
		{
			if (angle > 360f)
			{
				return angle - 360f;
			}
			if (angle < 0f)
			{
				return 360f - angle;
			}
			return angle;
		}

		private static float GetLabelAngle(float sliceCenterAngle)
		{
			float num = NormalizeAngle(sliceCenterAngle);
			if (90f < num && num < 270f)
			{
				if (num < 180f)
				{
					return num + 180f;
				}
				return num - 180f;
			}
			return num;
		}

		private static PointF GetSliceCenterRelative(ChartGraphics graph, float centerAngle, PointF centerAbsolute, float radiusAbsolute)
		{
			double num = (double)centerAngle * Math.PI / 180.0;
			PointF absolute = new PointF((float)((double)centerAbsolute.X + Math.Cos(num) * (double)radiusAbsolute), (float)((double)centerAbsolute.Y + Math.Sin(num) * (double)radiusAbsolute));
			return graph.GetRelativePoint(absolute);
		}

		private static float FindOptimalWidth(float maxWidth, ChartGraphics graph, GraphicsPath sliceGraphicsPath, RectangleF labelRelativeRect, int labelRotationAngle)
		{
			RectangleF labelRelativeRect2 = new RectangleF(labelRelativeRect.Location, labelRelativeRect.Size);
			float width = labelRelativeRect.Width;
			int num = 4;
			while (graph.CanLabelFitInSlice(sliceGraphicsPath, labelRelativeRect2, labelRotationAngle) && num-- >= 0)
			{
				width = labelRelativeRect2.Width;
				labelRelativeRect2.Width = width + (maxWidth - width) / 2f;
			}
			return width;
		}

		private static bool CanFitInResizedArea(string text, Font textFont, SizeF relativeSize, PointF sliceCenterRelative, ChartGraphics graph, GraphicsPath sliceGraphicsPath, RectangleF labelRelativeRect, int labelRotationAngle, float radiusAbsolute, out RectangleF resizedRect)
		{
			float num = relativeSize.Width / (float)text.Length;
			float num2 = relativeSize.Height / 8f;
			float num3 = relativeSize.Width + num;
			float width = labelRelativeRect.Width;
			float height = relativeSize.Height;
			resizedRect = labelRelativeRect;
			for (int i = 2; i <= 4; i++)
			{
				float num4 = num3 / (float)i + num;
				float num5 = height * (float)i + num2;
				labelRelativeRect = new RectangleF(sliceCenterRelative.X - num4 / 2f, sliceCenterRelative.Y - num5 / 2f, num4, num5);
				if (graph.CanLabelFitInSlice(sliceGraphicsPath, labelRelativeRect, labelRotationAngle))
				{
					labelRelativeRect.Width = FindOptimalWidth(width, graph, sliceGraphicsPath, labelRelativeRect, labelRotationAngle);
					StringFormat stringFormat = new StringFormat(StringFormat.GenericTypographic);
					stringFormat.FormatFlags = format.FormatFlags;
					graph.MeasureString(text.Replace("\\n", "\n"), textFont, labelRelativeRect.Size, stringFormat, out int charactersFitted, out int _);
					if (charactersFitted == text.Length)
					{
						resizedRect = labelRelativeRect;
						return true;
					}
				}
			}
			return false;
		}

		private static void RenderLabel(CommonElements common, ChartGraphics graph, CategoryNode node, DataPoint dataPoint, DataPointAttributes dataPointAttributes, string text, float startAngle, float sweepAngle, PointF centerAbsolute, float radiusAbsolute, int dataPointIndex, GraphicsPath sliceGraphicsPath)
		{
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			SizeF size = graph.MeasureString(text.Replace("\\n", "\n"), dataPointAttributes.Font, new SizeF(1000f, 1000f), new StringFormat(StringFormat.GenericTypographic));
			SizeF relativeSize = graph.GetRelativeSize(size);
			float num = relativeSize.Width / (float)text.Length;
			float num2 = relativeSize.Width + num;
			float num3 = relativeSize.Height + relativeSize.Height / 8f;
			float sliceCenterAngle = GetSliceCenterAngle(startAngle, sweepAngle);
			float labelAngle = GetLabelAngle(sliceCenterAngle);
			PointF sliceCenterRelative = GetSliceCenterRelative(graph, sliceCenterAngle, centerAbsolute, radiusAbsolute);
			RectangleF resizedRect = new RectangleF(sliceCenterRelative.X - num2 / 2f, sliceCenterRelative.Y - num3 / 2f, num2, num3);
			if (resizedRect.IsEmpty)
			{
				return;
			}
			int num4 = (int)labelAngle + dataPointAttributes.FontAngle;
			if (graph.CanLabelFitInSlice(sliceGraphicsPath, resizedRect, num4) || CanFitInResizedArea(text, dataPointAttributes.Font, relativeSize, sliceCenterRelative, graph, sliceGraphicsPath, resizedRect, num4, radiusAbsolute, out resizedRect))
			{
				if (dataPoint != null)
				{
					graph.DrawPointLabelStringRel(common, text, dataPoint.Font, new SolidBrush(dataPoint.FontColor), resizedRect, format, (int)labelAngle + dataPoint.FontAngle, resizedRect, dataPoint.LabelBackColor, dataPoint.LabelBorderColor, dataPoint.LabelBorderWidth, dataPoint.LabelBorderStyle, dataPoint.series, dataPoint, dataPointIndex);
					return;
				}
				graph.DrawLabelBackground(num4, sliceCenterRelative, resizedRect, dataPointAttributes.LabelBackColor, dataPointAttributes.LabelBorderColor, dataPointAttributes.LabelBorderWidth, dataPointAttributes.LabelBorderStyle);
				graph.MapCategoryNodeLabel(common, node, resizedRect);
				graph.DrawStringRel(text, dataPointAttributes.Font, new SolidBrush(dataPointAttributes.FontColor), resizedRect, format, num4);
			}
		}

		private static List<Series> GetChartAreaSeries(string chartAreaName, SeriesCollection chartSeries)
		{
			List<Series> list = new List<Series>();
			foreach (Series item in chartSeries)
			{
				if (item.IsVisible() && !(item.ChartArea != chartAreaName))
				{
					list.Add(item);
				}
			}
			return list;
		}

		private static void SortSeriesByAbsoluteValue(List<Series> seriesCollection, CategoryNodeCollection nodes)
		{
			seriesCollection.Sort((Series series1, Series series2) => nodes.GetTotalAbsoluteValue(series2).CompareTo(nodes.GetTotalAbsoluteValue(series1)));
		}
	}
}
