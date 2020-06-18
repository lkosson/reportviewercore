using Microsoft.Reporting.Chart.WebForms.ChartTypes;
using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Microsoft.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeSmartLabels_SmartLabels")]
	internal class SmartLabels
	{
		internal ArrayList smartLabelsPositions;

		internal bool checkAllCollisions;

		internal int markersCount;

		internal void Reset(CommonElements common, ChartArea area)
		{
			smartLabelsPositions = new ArrayList();
		}

		internal PointF AdjustSmartLabelPosition(CommonElements common, ChartGraphics graph, ChartArea area, SmartLabelsStyle smartLabelsStyle, PointF labelPosition, SizeF labelSize, ref StringFormat format, PointF markerPosition, SizeF markerSize, LabelAlignmentTypes labelAlignment)
		{
			return AdjustSmartLabelPosition(common, graph, area, smartLabelsStyle, labelPosition, labelSize, ref format, markerPosition, markerSize, labelAlignment, checkCalloutLineOverlapping: false);
		}

		internal PointF AdjustSmartLabelPosition(CommonElements common, ChartGraphics graph, ChartArea area, SmartLabelsStyle smartLabelsStyle, PointF labelPosition, SizeF labelSize, ref StringFormat format, PointF markerPosition, SizeF markerSize, LabelAlignmentTypes labelAlignment, bool checkCalloutLineOverlapping)
		{
			if (smartLabelsStyle.Enabled)
			{
				bool num = smartLabelsPositions.Count == 0;
				AddMarkersPosition(common, area);
				if (num)
				{
					markersCount = smartLabelsPositions.Count;
				}
				if (IsSmartLabelCollide(common, graph, area, smartLabelsStyle, labelPosition, labelSize, markerPosition, format, labelAlignment, checkCalloutLineOverlapping) && (FindNewPosition(common, graph, area, smartLabelsStyle, ref labelPosition, labelSize, ref format, markerPosition, ref markerSize, ref labelAlignment, checkCalloutLineOverlapping) || labelAlignment == LabelAlignmentTypes.BottomLeft || labelAlignment == LabelAlignmentTypes.BottomRight || labelAlignment == LabelAlignmentTypes.TopLeft || labelAlignment == LabelAlignmentTypes.TopRight) && !labelPosition.IsEmpty)
				{
					DrawCallout(common, graph, area, smartLabelsStyle, labelPosition, labelSize, format, markerPosition, markerSize, labelAlignment);
				}
				AddSmartLabelPosition(graph, area, labelPosition, labelSize, format);
			}
			return labelPosition;
		}

		private bool FindNewPosition(CommonElements common, ChartGraphics graph, ChartArea area, SmartLabelsStyle smartLabelsStyle, ref PointF labelPosition, SizeF labelSize, ref StringFormat format, PointF markerPosition, ref SizeF markerSize, ref LabelAlignmentTypes labelAlignment, bool checkCalloutLineOverlapping)
		{
			SizeF sizeF = SizeF.Empty;
			PointF pointF = PointF.Empty;
			int i = 0;
			float num = 0f;
			bool flag = false;
			LabelAlignmentTypes[] array = new LabelAlignmentTypes[9]
			{
				LabelAlignmentTypes.Top,
				LabelAlignmentTypes.Bottom,
				LabelAlignmentTypes.Left,
				LabelAlignmentTypes.Right,
				LabelAlignmentTypes.TopLeft,
				LabelAlignmentTypes.TopRight,
				LabelAlignmentTypes.BottomLeft,
				LabelAlignmentTypes.BottomRight,
				LabelAlignmentTypes.Center
			};
			SizeF relativeSize = graph.GetRelativeSize(new SizeF(1f, 1f));
			bool flag2 = false;
			float num2 = 2f;
			float num3 = (float)Math.Min(smartLabelsStyle.MinMovingDistance, smartLabelsStyle.MaxMovingDistance);
			float num4 = (float)Math.Max(smartLabelsStyle.MinMovingDistance, smartLabelsStyle.MaxMovingDistance);
			num = num3;
			while (!flag2 && num <= num4)
			{
				sizeF = new SizeF(markerSize.Width + num * (relativeSize.Width * 2f), markerSize.Height + num * (relativeSize.Height * 2f));
				for (i = 0; i < array.Length; i++)
				{
					if ((array[i] != LabelAlignmentTypes.Center || num == num3) && (smartLabelsStyle.MovingDirection & array[i]) == array[i])
					{
						pointF = CalculatePosition(array[i], markerPosition, sizeF, labelSize, ref format);
						if (!IsSmartLabelCollide(common, null, area, smartLabelsStyle, pointF, labelSize, markerPosition, format, array[i], checkCalloutLineOverlapping))
						{
							flag2 = true;
							flag = ((num != 0f) ? true : false);
							break;
						}
					}
				}
				num += num2;
			}
			if (flag2)
			{
				markerSize = sizeF;
				labelPosition = pointF;
				labelAlignment = array[i];
			}
			if (!flag2 && smartLabelsStyle.HideOverlapped)
			{
				labelPosition = PointF.Empty;
			}
			if (!(flag && flag2))
			{
				return false;
			}
			return true;
		}

		internal virtual void DrawCallout(CommonElements common, ChartGraphics graph, ChartArea area, SmartLabelsStyle smartLabelsStyle, PointF labelPosition, SizeF labelSize, StringFormat format, PointF markerPosition, SizeF markerSize, LabelAlignmentTypes labelAlignment)
		{
			RectangleF absoluteRectangle = graph.GetAbsoluteRectangle(GetLabelPosition(graph, labelPosition, labelSize, format, adjustForDrawing: true));
			Pen pen = new Pen(smartLabelsStyle.CalloutLineColor, smartLabelsStyle.CalloutLineWidth);
			pen.DashStyle = graph.GetPenStyle(smartLabelsStyle.CalloutLineStyle);
			if (smartLabelsStyle.CalloutStyle == LabelCalloutStyle.Box)
			{
				if (smartLabelsStyle.CalloutBackColor != Color.Transparent)
				{
					Brush brush = new SolidBrush(smartLabelsStyle.CalloutBackColor);
					graph.FillRectangle(brush, absoluteRectangle);
				}
				graph.DrawRectangle(pen, absoluteRectangle.X, absoluteRectangle.Y, absoluteRectangle.Width, absoluteRectangle.Height);
			}
			else if (smartLabelsStyle.CalloutStyle == LabelCalloutStyle.Underlined)
			{
				switch (labelAlignment)
				{
				case LabelAlignmentTypes.Right:
					graph.DrawLine(pen, absoluteRectangle.X, absoluteRectangle.Top, absoluteRectangle.X, absoluteRectangle.Bottom);
					break;
				case LabelAlignmentTypes.Left:
					graph.DrawLine(pen, absoluteRectangle.Right, absoluteRectangle.Top, absoluteRectangle.Right, absoluteRectangle.Bottom);
					break;
				case LabelAlignmentTypes.Bottom:
					graph.DrawLine(pen, absoluteRectangle.X, absoluteRectangle.Top, absoluteRectangle.Right, absoluteRectangle.Top);
					break;
				default:
					graph.DrawLine(pen, absoluteRectangle.X, absoluteRectangle.Bottom, absoluteRectangle.Right, absoluteRectangle.Bottom);
					break;
				}
			}
			PointF absolutePoint = graph.GetAbsolutePoint(labelPosition);
			switch (labelAlignment)
			{
			case LabelAlignmentTypes.Top:
				absolutePoint.Y = absoluteRectangle.Bottom;
				break;
			case LabelAlignmentTypes.Bottom:
				absolutePoint.Y = absoluteRectangle.Top;
				break;
			}
			if (smartLabelsStyle.CalloutStyle == LabelCalloutStyle.Underlined && (labelAlignment == LabelAlignmentTypes.TopLeft || labelAlignment == LabelAlignmentTypes.TopRight || labelAlignment == LabelAlignmentTypes.BottomLeft || labelAlignment == LabelAlignmentTypes.BottomRight))
			{
				absolutePoint.Y = absoluteRectangle.Bottom;
			}
			if (smartLabelsStyle.CalloutLineAnchorCap == LineAnchorCap.Arrow)
			{
				pen.StartCap = LineCap.Custom;
				pen.CustomStartCap = new AdjustableArrowCap(pen.Width + 2f, pen.Width + 3f, isFilled: true);
			}
			else if (smartLabelsStyle.CalloutLineAnchorCap == LineAnchorCap.Diamond)
			{
				pen.StartCap = LineCap.DiamondAnchor;
			}
			else if (smartLabelsStyle.CalloutLineAnchorCap == LineAnchorCap.Round)
			{
				pen.StartCap = LineCap.RoundAnchor;
			}
			else if (smartLabelsStyle.CalloutLineAnchorCap == LineAnchorCap.Square)
			{
				pen.StartCap = LineCap.SquareAnchor;
			}
			PointF absolutePoint2 = graph.GetAbsolutePoint(markerPosition);
			graph.DrawLine(pen, absolutePoint2.X, absolutePoint2.Y, absolutePoint.X, absolutePoint.Y);
		}

		internal virtual bool IsSmartLabelCollide(CommonElements common, ChartGraphics graph, ChartArea area, SmartLabelsStyle smartLabelsStyle, PointF position, SizeF size, PointF markerPosition, StringFormat format, LabelAlignmentTypes labelAlignment, bool checkCalloutLineOverlapping)
		{
			bool flag = false;
			RectangleF labelPosition = GetLabelPosition(graph, position, size, format, adjustForDrawing: false);
			if (labelPosition.X < 0f || labelPosition.Y < 0f || labelPosition.Bottom > 100f || labelPosition.Right > 100f)
			{
				flag = true;
			}
			if (!flag && area != null)
			{
				if (area.chartAreaIsCurcular)
				{
					using (GraphicsPath graphicsPath = new GraphicsPath())
					{
						graphicsPath.AddEllipse(area.PlotAreaPosition.ToRectangleF());
						if (smartLabelsStyle.AllowOutsidePlotArea == LabelOutsidePlotAreaStyle.Partial)
						{
							PointF point = new PointF(labelPosition.X + labelPosition.Width / 2f, labelPosition.Y + labelPosition.Height / 2f);
							if (!graphicsPath.IsVisible(point))
							{
								flag = true;
							}
						}
						else if (smartLabelsStyle.AllowOutsidePlotArea == LabelOutsidePlotAreaStyle.No && (!graphicsPath.IsVisible(labelPosition.Location) || !graphicsPath.IsVisible(new PointF(labelPosition.Right, labelPosition.Y)) || !graphicsPath.IsVisible(new PointF(labelPosition.Right, labelPosition.Bottom)) || !graphicsPath.IsVisible(new PointF(labelPosition.X, labelPosition.Bottom))))
						{
							flag = true;
						}
					}
				}
				else if (smartLabelsStyle.AllowOutsidePlotArea == LabelOutsidePlotAreaStyle.Partial)
				{
					PointF pt = new PointF(labelPosition.X + labelPosition.Width / 2f, labelPosition.Y + labelPosition.Height / 2f);
					if (!area.PlotAreaPosition.ToRectangleF().Contains(pt))
					{
						flag = true;
					}
				}
				else if (smartLabelsStyle.AllowOutsidePlotArea == LabelOutsidePlotAreaStyle.No && !area.PlotAreaPosition.ToRectangleF().Contains(labelPosition))
				{
					flag = true;
				}
			}
			bool flag2 = (labelAlignment == LabelAlignmentTypes.Center && !smartLabelsStyle.MarkerOverlapping) ? true : false;
			if (checkAllCollisions)
			{
				flag2 = false;
			}
			if (!flag && smartLabelsPositions != null)
			{
				int num = -1;
				{
					foreach (RectangleF smartLabelsPosition in smartLabelsPositions)
					{
						num++;
						bool flag3 = smartLabelsPosition.IntersectsWith(labelPosition);
						if (!flag3 && checkCalloutLineOverlapping && num >= markersCount && LineIntersectRectangle(point2: new PointF(labelPosition.X + labelPosition.Width / 2f, labelPosition.Y + labelPosition.Height / 2f), rect: smartLabelsPosition, point1: markerPosition))
						{
							flag3 = true;
						}
						if (flag3)
						{
							if (!flag2)
							{
								return true;
							}
							flag2 = false;
						}
					}
					return flag;
				}
			}
			return flag;
		}

		private bool LineIntersectRectangle(RectangleF rect, PointF point1, PointF point2)
		{
			if (point1.X == point2.X)
			{
				if (point1.X >= rect.X && point1.X <= rect.Right)
				{
					if (point1.Y < rect.Y && point2.Y < rect.Y)
					{
						return false;
					}
					if (point1.Y > rect.Bottom && point2.Y > rect.Bottom)
					{
						return false;
					}
					return true;
				}
				return false;
			}
			if (point1.Y == point2.Y)
			{
				if (point1.Y >= rect.Y && point1.Y <= rect.Bottom)
				{
					if (point1.X < rect.X && point2.X < rect.X)
					{
						return false;
					}
					if (point1.X > rect.Right && point2.X > rect.Right)
					{
						return false;
					}
					return true;
				}
				return false;
			}
			if (point1.X < rect.X && point2.X < rect.X)
			{
				return false;
			}
			if (point1.X > rect.Right && point2.X > rect.Right)
			{
				return false;
			}
			if (point1.Y < rect.Y && point2.Y < rect.Y)
			{
				return false;
			}
			if (point1.Y > rect.Bottom && point2.Y > rect.Bottom)
			{
				return false;
			}
			if (rect.Contains(point1) || rect.Contains(point2))
			{
				return true;
			}
			PointF intersectionY = CalloutAnnotation.GetIntersectionY(point1, point2, rect.Y);
			if (rect.Contains(intersectionY))
			{
				return true;
			}
			intersectionY = CalloutAnnotation.GetIntersectionY(point1, point2, rect.Bottom);
			if (rect.Contains(intersectionY))
			{
				return true;
			}
			intersectionY = CalloutAnnotation.GetIntersectionX(point1, point2, rect.X);
			if (rect.Contains(intersectionY))
			{
				return true;
			}
			intersectionY = CalloutAnnotation.GetIntersectionX(point1, point2, rect.Right);
			if (rect.Contains(intersectionY))
			{
				return true;
			}
			return false;
		}

		internal virtual void AddMarkersPosition(CommonElements common, ChartArea area)
		{
			if (smartLabelsPositions.Count != 0 || area == null)
			{
				return;
			}
			ChartTypeRegistry chartTypeRegistry = common.ChartTypeRegistry;
			foreach (Series item in common.DataManager.Series)
			{
				if (item.ChartArea == area.Name && item.SmartLabels.Enabled && !item.SmartLabels.MarkerOverlapping)
				{
					chartTypeRegistry.GetChartType(item.ChartTypeName).AddSmartLabelMarkerPositions(common, area, item, smartLabelsPositions);
				}
			}
			Axis[] axes = area.Axes;
			foreach (Axis axis in axes)
			{
				if (!(axis.ScaleBreakStyle.Spacing > 0.0) || axis.ScaleSegments.Count <= 0)
				{
					continue;
				}
				for (int j = 0; j < axis.ScaleSegments.Count - 1; j++)
				{
					RectangleF breakLinePosition = axis.ScaleSegments[j].GetBreakLinePosition(common.graph, axis.ScaleSegments[j + 1]);
					breakLinePosition = common.graph.GetRelativeRectangle(breakLinePosition);
					if (smartLabelsPositions == null)
					{
						smartLabelsPositions = new ArrayList();
					}
					smartLabelsPositions.Add(breakLinePosition);
				}
			}
		}

		internal void AddSmartLabelPosition(ChartGraphics graph, ChartArea area, PointF position, SizeF size, StringFormat format)
		{
			RectangleF labelPosition = GetLabelPosition(graph, position, size, format, adjustForDrawing: false);
			if (smartLabelsPositions == null)
			{
				smartLabelsPositions = new ArrayList();
			}
			smartLabelsPositions.Add(labelPosition);
		}

		internal RectangleF GetLabelPosition(ChartGraphics graph, PointF position, SizeF size, StringFormat format, bool adjustForDrawing)
		{
			RectangleF empty = RectangleF.Empty;
			empty.Width = size.Width;
			empty.Height = size.Height;
			SizeF sizeF = SizeF.Empty;
			if (graph != null)
			{
				sizeF = graph.GetRelativeSize(new SizeF(1f, 1f));
			}
			if (format.Alignment == StringAlignment.Far)
			{
				empty.X = position.X - size.Width;
				if (adjustForDrawing && !sizeF.IsEmpty)
				{
					empty.X -= 4f * sizeF.Width;
					empty.Width += 4f * sizeF.Width;
				}
			}
			else if (format.Alignment == StringAlignment.Near)
			{
				empty.X = position.X;
				if (adjustForDrawing && !sizeF.IsEmpty)
				{
					empty.Width += 4f * sizeF.Width;
				}
			}
			else if (format.Alignment == StringAlignment.Center)
			{
				empty.X = position.X - size.Width / 2f;
				if (adjustForDrawing && !sizeF.IsEmpty)
				{
					empty.X -= 2f * sizeF.Width;
					empty.Width += 4f * sizeF.Width;
				}
			}
			if (format.LineAlignment == StringAlignment.Far)
			{
				empty.Y = position.Y - size.Height;
			}
			else if (format.LineAlignment == StringAlignment.Near)
			{
				empty.Y = position.Y;
			}
			else if (format.LineAlignment == StringAlignment.Center)
			{
				empty.Y = position.Y - size.Height / 2f;
			}
			return empty;
		}

		private PointF CalculatePosition(LabelAlignmentTypes labelAlignment, PointF markerPosition, SizeF sizeMarker, SizeF sizeFont, ref StringFormat format)
		{
			format.Alignment = StringAlignment.Near;
			format.LineAlignment = StringAlignment.Center;
			PointF result = new PointF(markerPosition.X, markerPosition.Y);
			switch (labelAlignment)
			{
			case LabelAlignmentTypes.Center:
				format.Alignment = StringAlignment.Center;
				break;
			case LabelAlignmentTypes.Bottom:
				format.Alignment = StringAlignment.Center;
				result.Y += sizeMarker.Height / 1.75f;
				result.Y += sizeFont.Height / 2f;
				break;
			case LabelAlignmentTypes.Top:
				format.Alignment = StringAlignment.Center;
				result.Y -= sizeMarker.Height / 1.75f;
				result.Y -= sizeFont.Height / 2f;
				break;
			case LabelAlignmentTypes.Left:
				format.Alignment = StringAlignment.Far;
				result.X -= sizeMarker.Height / 1.75f;
				break;
			case LabelAlignmentTypes.TopLeft:
				format.Alignment = StringAlignment.Far;
				result.X -= sizeMarker.Height / 1.75f;
				result.Y -= sizeMarker.Height / 1.75f;
				result.Y -= sizeFont.Height / 2f;
				break;
			case LabelAlignmentTypes.BottomLeft:
				format.Alignment = StringAlignment.Far;
				result.X -= sizeMarker.Height / 1.75f;
				result.Y += sizeMarker.Height / 1.75f;
				result.Y += sizeFont.Height / 2f;
				break;
			case LabelAlignmentTypes.Right:
				result.X += sizeMarker.Height / 1.75f;
				break;
			case LabelAlignmentTypes.TopRight:
				result.X += sizeMarker.Height / 1.75f;
				result.Y -= sizeMarker.Height / 1.75f;
				result.Y -= sizeFont.Height / 2f;
				break;
			case LabelAlignmentTypes.BottomRight:
				result.X += sizeMarker.Height / 1.75f;
				result.Y += sizeMarker.Height / 1.75f;
				result.Y += sizeFont.Height / 2f;
				break;
			}
			return result;
		}
	}
}
