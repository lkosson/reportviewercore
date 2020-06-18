using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Microsoft.Reporting.Chart.WebForms
{
	[DefaultProperty("Enabled")]
	[SRDescription("DescriptionAttributeTickMark_TickMark")]
	internal class TickMark : Grid
	{
		private TickMarkStyle style = TickMarkStyle.Outside;

		private float size = 1f;

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(TickMarkStyle.Outside)]
		[SRDescription("DescriptionAttributeTickMark_Style")]
		public TickMarkStyle Style
		{
			get
			{
				return style;
			}
			set
			{
				style = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(1f)]
		[SRDescription("DescriptionAttributeTickMark_Size")]
		public float Size
		{
			get
			{
				return size;
			}
			set
			{
				size = value;
				Invalidate();
			}
		}

		public TickMark()
			: base(null, major: true)
		{
		}

		public TickMark(Axis axis, bool major)
			: base(axis, major)
		{
		}

		internal void Paint(ChartGraphics graph, bool backElements)
		{
			PointF empty = PointF.Empty;
			PointF empty2 = PointF.Empty;
			if (!enabled)
			{
				return;
			}
			double interval = base.interval;
			DateTimeIntervalType intervalType = base.intervalType;
			double intervalOffset = base.intervalOffset;
			DateTimeIntervalType intervalOffsetType = base.intervalOffsetType;
			if (!majorGridTick && (base.interval == 0.0 || double.IsNaN(base.interval)))
			{
				if (axis.majorGrid.IntervalType == DateTimeIntervalType.Auto)
				{
					base.interval = axis.majorGrid.Interval / 5.0;
				}
				else
				{
					DateTimeIntervalType type = axis.majorGrid.IntervalType;
					base.interval = axis.CalcInterval(axis.GetViewMinimum(), axis.GetViewMinimum() + (axis.GetViewMaximum() - axis.GetViewMinimum()) / 4.0, date: true, out type, ChartValueTypes.DateTime);
					base.intervalType = type;
					base.intervalOffsetType = axis.majorGrid.IntervalOffsetType;
					base.intervalOffset = axis.majorGrid.IntervalOffset;
				}
			}
			if (style == TickMarkStyle.None)
			{
				return;
			}
			if (axis.IsCustomTickMarks())
			{
				PaintCustom(graph, backElements);
				return;
			}
			axis.PlotAreaPosition.ToRectangleF();
			Series series = null;
			if (axis.axisType == AxisName.X || axis.axisType == AxisName.X2)
			{
				ArrayList xAxesSeries = axis.chartArea.GetXAxesSeries((axis.axisType != 0) ? AxisType.Secondary : AxisType.Primary, axis.SubAxisName);
				if (xAxesSeries.Count > 0)
				{
					series = axis.Common.DataManager.Series[xAxesSeries[0]];
					if (series != null && !series.XValueIndexed)
					{
						series = null;
					}
				}
			}
			double num = axis.GetViewMinimum();
			DateTimeIntervalType dateTimeIntervalType = (base.IntervalOffsetType == DateTimeIntervalType.Auto) ? base.IntervalType : base.IntervalOffsetType;
			if (!axis.chartArea.chartAreaIsCurcular || axis.axisType == AxisName.Y || axis.axisType == AxisName.Y2)
			{
				num = axis.AlignIntervalStart(num, base.Interval, base.IntervalType, series, majorGridTick);
			}
			if (base.IntervalOffset != 0.0 && !double.IsNaN(base.IntervalOffset) && series == null)
			{
				num += axis.GetIntervalSize(num, base.IntervalOffset, dateTimeIntervalType, series, 0.0, DateTimeIntervalType.Number, forceIntIndex: true, forceAbsInterval: false);
			}
			if ((axis.GetViewMaximum() - axis.GetViewMinimum()) / axis.GetIntervalSize(num, base.Interval, base.IntervalType, series, 0.0, DateTimeIntervalType.Number, forceIntIndex: true) > 10000.0 || axis.GetViewMaximum() <= axis.GetViewMinimum())
			{
				return;
			}
			float num2 = 0f;
			if (axis.ScrollBar.IsVisible() && axis.ScrollBar.PositionInside && (axis.IsAxisOnAreaEdge() || !axis.MarksNextToAxis))
			{
				num2 = (float)axis.ScrollBar.GetScrollBarRelativeSize();
			}
			if (axis.AxisPosition == AxisPosition.Left)
			{
				float num3 = (!axis.IsMarksNextToAxis()) ? axis.PlotAreaPosition.X : ((float)axis.GetAxisPosition());
				if (style == TickMarkStyle.Inside)
				{
					empty.X = num3;
					empty2.X = num3 + size;
				}
				else if (style == TickMarkStyle.Outside)
				{
					empty.X = num3 - size - num2;
					empty2.X = num3;
				}
				else if (style == TickMarkStyle.Cross)
				{
					empty.X = num3 - size / 2f - num2;
					empty2.X = num3 + size / 2f;
				}
			}
			else if (axis.AxisPosition == AxisPosition.Right)
			{
				float num3 = (!axis.IsMarksNextToAxis()) ? axis.PlotAreaPosition.Right() : ((float)axis.GetAxisPosition());
				if (style == TickMarkStyle.Inside)
				{
					empty.X = num3 - size;
					empty2.X = num3;
				}
				else if (style == TickMarkStyle.Outside)
				{
					empty.X = num3;
					empty2.X = num3 + size + num2;
				}
				else if (style == TickMarkStyle.Cross)
				{
					empty.X = num3 - size / 2f;
					empty2.X = num3 + size / 2f + num2;
				}
			}
			else if (axis.AxisPosition == AxisPosition.Top)
			{
				float num3 = (!axis.IsMarksNextToAxis()) ? axis.PlotAreaPosition.Y : ((float)axis.GetAxisPosition());
				if (style == TickMarkStyle.Inside)
				{
					empty.Y = num3;
					empty2.Y = num3 + size;
				}
				else if (style == TickMarkStyle.Outside)
				{
					empty.Y = num3 - size - num2;
					empty2.Y = num3;
				}
				else if (style == TickMarkStyle.Cross)
				{
					empty.Y = num3 - size / 2f - num2;
					empty2.Y = num3 + size / 2f;
				}
			}
			else if (axis.AxisPosition == AxisPosition.Bottom)
			{
				float num3 = (!axis.IsMarksNextToAxis()) ? axis.PlotAreaPosition.Bottom() : ((float)axis.GetAxisPosition());
				if (style == TickMarkStyle.Inside)
				{
					empty.Y = num3 - size;
					empty2.Y = num3;
				}
				else if (style == TickMarkStyle.Outside)
				{
					empty.Y = num3;
					empty2.Y = num3 + size + num2;
				}
				else if (style == TickMarkStyle.Cross)
				{
					empty.Y = num3 - size / 2f;
					empty2.Y = num3 + size / 2f + num2;
				}
			}
			int num4 = 0;
			int num5 = 1;
			double num6 = num;
			double num7 = 0.0;
			while (num <= axis.GetViewMaximum())
			{
				double num8 = 0.0;
				if (majorGridTick || !axis.Logarithmic)
				{
					num7 = axis.GetIntervalSize(num, base.Interval, base.IntervalType, series, base.IntervalOffset, dateTimeIntervalType, forceIntIndex: true);
				}
				else
				{
					double logMinimum = GetLogMinimum(num, series);
					if (num6 != logMinimum)
					{
						num6 = logMinimum;
						num5 = 1;
					}
					num8 = Math.Log(1.0 + base.interval * (double)num5, axis.logarithmBase);
					num = num6;
					num7 = num8;
					num5++;
					if (GetLogMinimum(num + num8, series) != logMinimum)
					{
						num += num8;
						continue;
					}
				}
				if (num == axis.GetViewMaximum() && series != null)
				{
					num += num7;
					continue;
				}
				if (num7 == 0.0)
				{
					throw new InvalidOperationException(SR.ExceptionTickMarksIntervalIsZero);
				}
				if (num4++ > 10000)
				{
					break;
				}
				if (axis != null && axis.chartArea != null && axis.chartArea.chartAreaIsCurcular && ((!axis.Reverse && num == axis.GetViewMinimum()) || (axis.Reverse && num == axis.GetViewMaximum())))
				{
					num += num7;
					continue;
				}
				if (!majorGridTick && axis.Logarithmic)
				{
					num += num8;
					if (num > axis.GetViewMaximum())
					{
						break;
					}
				}
				if ((decimal)num >= (decimal)axis.GetViewMinimum())
				{
					if (axis.AxisPosition == AxisPosition.Left)
					{
						empty.Y = (float)axis.GetLinearPosition(num);
						empty2.Y = empty.Y;
					}
					else if (axis.AxisPosition == AxisPosition.Right)
					{
						empty.Y = (float)axis.GetLinearPosition(num);
						empty2.Y = empty.Y;
					}
					else if (axis.AxisPosition == AxisPosition.Top)
					{
						empty.X = (float)axis.GetLinearPosition(num);
						empty2.X = empty.X;
					}
					else if (axis.AxisPosition == AxisPosition.Bottom)
					{
						empty.X = (float)axis.GetLinearPosition(num);
						empty2.X = empty.X;
					}
					if (axis.Common.ProcessModeRegions)
					{
						if (axis.chartArea.chartAreaIsCurcular)
						{
							RectangleF relative = new RectangleF(empty.X - 0.5f, empty.Y - 0.5f, Math.Abs(empty2.X - empty.X) + 1f, Math.Abs(empty2.Y - empty.Y) + 1f);
							GraphicsPath graphicsPath = new GraphicsPath();
							graphicsPath.AddRectangle(graph.GetAbsoluteRectangle(relative));
							graphicsPath.Transform(graph.Transform);
							axis.Common.HotRegionsList.AddHotRegion(graphicsPath, relativePath: false, graph, ChartElementType.TickMarks, this);
						}
						else if (!axis.chartArea.Area3DStyle.Enable3D || axis.chartArea.chartAreaIsCurcular)
						{
							RectangleF rectArea = new RectangleF(empty.X - 0.5f, empty.Y - 0.5f, Math.Abs(empty2.X - empty.X) + 1f, Math.Abs(empty2.Y - empty.Y) + 1f);
							axis.Common.HotRegionsList.AddHotRegion(rectArea, this, ChartElementType.TickMarks, relativeCoordinates: true);
						}
						else
						{
							Draw3DTickLine(graph, empty, empty2, axis.AxisPosition == AxisPosition.Left || axis.AxisPosition == AxisPosition.Right, num2, backElements);
						}
					}
					if (axis.Common.ProcessModePaint)
					{
						if (!axis.chartArea.Area3DStyle.Enable3D || axis.chartArea.chartAreaIsCurcular)
						{
							graph.StartAnimation();
							graph.DrawLineRel(borderColor, borderWidth, borderStyle, empty, empty2);
							graph.StopAnimation();
						}
						else
						{
							graph.StartAnimation();
							Draw3DTickLine(graph, empty, empty2, axis.AxisPosition == AxisPosition.Left || axis.AxisPosition == AxisPosition.Right, num2, backElements);
							graph.StopAnimation();
						}
					}
				}
				if (majorGridTick || !axis.Logarithmic)
				{
					num += num7;
				}
			}
			if (!majorGridTick)
			{
				base.interval = interval;
				base.intervalType = intervalType;
				base.intervalOffset = intervalOffset;
				base.intervalOffsetType = intervalOffsetType;
			}
		}

		private double GetLogMinimum(double current, Series axisSeries)
		{
			double num = axis.GetViewMinimum();
			DateTimeIntervalType type = (base.IntervalOffsetType == DateTimeIntervalType.Auto) ? base.IntervalType : base.IntervalOffsetType;
			if (base.IntervalOffset != 0.0 && !double.IsNaN(base.IntervalOffset) && axisSeries == null)
			{
				num += axis.GetIntervalSize(num, base.IntervalOffset, type, axisSeries, 0.0, DateTimeIntervalType.Number, forceIntIndex: true, forceAbsInterval: false);
			}
			return num + Math.Floor(current - num);
		}

		internal void PaintCustom(ChartGraphics graph, bool backElements)
		{
			PointF empty = PointF.Empty;
			PointF empty2 = PointF.Empty;
			axis.PlotAreaPosition.ToRectangleF();
			float num = 0f;
			if (axis.ScrollBar.IsVisible() && axis.ScrollBar.PositionInside && axis.IsAxisOnAreaEdge())
			{
				num = (float)axis.ScrollBar.GetScrollBarRelativeSize();
			}
			if (axis.AxisPosition == AxisPosition.Left)
			{
				float num2 = (!axis.IsMarksNextToAxis()) ? axis.PlotAreaPosition.X : ((float)axis.GetAxisPosition());
				if (style == TickMarkStyle.Inside)
				{
					empty.X = num2;
					empty2.X = num2 + size;
				}
				else if (style == TickMarkStyle.Outside)
				{
					empty.X = num2 - size - num;
					empty2.X = num2;
				}
				else if (style == TickMarkStyle.Cross)
				{
					empty.X = num2 - size / 2f - num;
					empty2.X = num2 + size / 2f;
				}
			}
			else if (axis.AxisPosition == AxisPosition.Right)
			{
				float num2 = (!axis.IsMarksNextToAxis()) ? axis.PlotAreaPosition.Right() : ((float)axis.GetAxisPosition());
				if (style == TickMarkStyle.Inside)
				{
					empty.X = num2 - size;
					empty2.X = num2;
				}
				else if (style == TickMarkStyle.Outside)
				{
					empty.X = num2;
					empty2.X = num2 + size + num;
				}
				else if (style == TickMarkStyle.Cross)
				{
					empty.X = num2 - size / 2f;
					empty2.X = num2 + size / 2f + num;
				}
			}
			else if (axis.AxisPosition == AxisPosition.Top)
			{
				float num2 = (!axis.IsMarksNextToAxis()) ? axis.PlotAreaPosition.Y : ((float)axis.GetAxisPosition());
				if (style == TickMarkStyle.Inside)
				{
					empty.Y = num2;
					empty2.Y = num2 + size;
				}
				else if (style == TickMarkStyle.Outside)
				{
					empty.Y = num2 - size - num;
					empty2.Y = num2;
				}
				else if (style == TickMarkStyle.Cross)
				{
					empty.Y = num2 - size / 2f - num;
					empty2.Y = num2 + size / 2f;
				}
			}
			else if (axis.AxisPosition == AxisPosition.Bottom)
			{
				float num2 = (!axis.IsMarksNextToAxis()) ? axis.PlotAreaPosition.Bottom() : ((float)axis.GetAxisPosition());
				if (style == TickMarkStyle.Inside)
				{
					empty.Y = num2 - size;
					empty2.Y = num2;
				}
				else if (style == TickMarkStyle.Outside)
				{
					empty.Y = num2;
					empty2.Y = num2 + size + num;
				}
				else if (style == TickMarkStyle.Cross)
				{
					empty.Y = num2 - size / 2f;
					empty2.Y = num2 + size / 2f + num;
				}
			}
			foreach (CustomLabel customLabel in axis.CustomLabels)
			{
				if ((customLabel.GridTicks & GridTicks.TickMark) != GridTicks.TickMark)
				{
					continue;
				}
				double num3 = (customLabel.To + customLabel.From) / 2.0;
				if (!(num3 >= axis.GetViewMinimum()) || !(num3 <= axis.GetViewMaximum()))
				{
					continue;
				}
				if (axis.AxisPosition == AxisPosition.Left)
				{
					empty.Y = (float)axis.GetLinearPosition(num3);
					empty2.Y = empty.Y;
				}
				else if (axis.AxisPosition == AxisPosition.Right)
				{
					empty.Y = (float)axis.GetLinearPosition(num3);
					empty2.Y = empty.Y;
				}
				else if (axis.AxisPosition == AxisPosition.Top)
				{
					empty.X = (float)axis.GetLinearPosition(num3);
					empty2.X = empty.X;
				}
				else if (axis.AxisPosition == AxisPosition.Bottom)
				{
					empty.X = (float)axis.GetLinearPosition(num3);
					empty2.X = empty.X;
				}
				if (axis.Common.ProcessModeRegions)
				{
					if (!axis.chartArea.Area3DStyle.Enable3D || axis.chartArea.chartAreaIsCurcular)
					{
						RectangleF rectArea = new RectangleF(empty.X - 0.5f, empty.Y - 0.5f, Math.Abs(empty2.X - empty.X) + 1f, Math.Abs(empty2.Y - empty.Y) + 1f);
						axis.Common.HotRegionsList.AddHotRegion(rectArea, this, ChartElementType.TickMarks, relativeCoordinates: true);
					}
					else
					{
						Draw3DTickLine(graph, empty, empty2, axis.AxisPosition == AxisPosition.Left || axis.AxisPosition == AxisPosition.Right, num, backElements);
					}
				}
				if (axis.Common.ProcessModePaint)
				{
					if (!axis.chartArea.Area3DStyle.Enable3D || axis.chartArea.chartAreaIsCurcular)
					{
						graph.DrawLineRel(borderColor, borderWidth, borderStyle, empty, empty2);
					}
					else
					{
						Draw3DTickLine(graph, empty, empty2, axis.AxisPosition == AxisPosition.Left || axis.AxisPosition == AxisPosition.Right, num, backElements);
					}
				}
			}
		}

		internal void Draw3DTickLine(ChartGraphics graph, PointF point1, PointF point2, bool horizontal, float scrollBarSize, bool backElements)
		{
			Draw3DTickLine(graph, point1, point2, horizontal, scrollBarSize, backElements, selectionMode: false);
		}

		internal void Draw3DTickLine(ChartGraphics graph, PointF point1, PointF point2, bool horizontal, float scrollBarSize, bool backElements, bool selectionMode)
		{
			ChartArea chartArea = axis.chartArea;
			bool axisOnEdge;
			float marksZPosition = axis.GetMarksZPosition(out axisOnEdge);
			bool flag = axisOnEdge;
			if ((flag && axis.MajorTickMark.Style == TickMarkStyle.Cross) || axis.MajorTickMark.Style == TickMarkStyle.Inside || axis.MinorTickMark.Style == TickMarkStyle.Cross || axis.MinorTickMark.Style == TickMarkStyle.Inside)
			{
				flag = false;
			}
			SurfaceNames surfaceName = (marksZPosition != 0f) ? SurfaceNames.Front : SurfaceNames.Back;
			if (!chartArea.ShouldDrawOnSurface(surfaceName, backElements, flag))
			{
				return;
			}
			if (axis.AxisPosition == AxisPosition.Bottom && (!axis.IsMarksNextToAxis() || axisOnEdge) && chartArea.IsBottomSceneWallVisible())
			{
				point2.Y += chartArea.areaSceneWallWidth.Height;
			}
			else if (axis.AxisPosition == AxisPosition.Left && (!axis.IsMarksNextToAxis() || axisOnEdge) && chartArea.IsSideSceneWallOnLeft())
			{
				point1.X -= chartArea.areaSceneWallWidth.Width;
			}
			else if (axis.AxisPosition == AxisPosition.Right && (!axis.IsMarksNextToAxis() || axisOnEdge) && !chartArea.IsSideSceneWallOnLeft())
			{
				point2.X += chartArea.areaSceneWallWidth.Width;
			}
			else if (axis.AxisPosition == AxisPosition.Top && (!axis.IsMarksNextToAxis() || axisOnEdge))
			{
				point1.Y -= chartArea.areaSceneWallWidth.Height;
			}
			Point3D point3D = null;
			Point3D point3D2 = null;
			if (axisOnEdge && chartArea.areaSceneWallWidth.Width != 0f)
			{
				if (axis.AxisPosition == AxisPosition.Top)
				{
					float y = axis.PlotAreaPosition.Y;
					if (style == TickMarkStyle.Inside)
					{
						point1.Y = y;
						point2.Y = y + size;
						point3D = new Point3D(point1.X, point1.Y, 0f - chartArea.areaSceneWallWidth.Width);
						point3D2 = new Point3D(point1.X, point1.Y, 0f);
					}
					else if (style == TickMarkStyle.Outside)
					{
						point1.Y = y;
						point2.Y = y;
						point3D = new Point3D(point1.X, y, marksZPosition);
						point3D2 = new Point3D(point1.X, point1.Y, 0f - size - chartArea.areaSceneWallWidth.Width);
					}
					else if (style == TickMarkStyle.Cross)
					{
						point1.Y = y;
						point2.Y = y + size / 2f;
						point3D = new Point3D(point1.X, y, marksZPosition);
						point3D2 = new Point3D(point1.X, point1.Y, (0f - size) / 2f - chartArea.areaSceneWallWidth.Width);
					}
					if (chartArea.ShouldDrawOnSurface(SurfaceNames.Top, backElements, onEdge: false))
					{
						point3D = null;
						point3D2 = null;
					}
				}
				if (axis.AxisPosition == AxisPosition.Left && !chartArea.IsSideSceneWallOnLeft())
				{
					float x = axis.PlotAreaPosition.X;
					if (style == TickMarkStyle.Inside)
					{
						point1.X = x;
						point2.X = x + size;
						point3D = new Point3D(point1.X, point1.Y, 0f - chartArea.areaSceneWallWidth.Width);
						point3D2 = new Point3D(point1.X, point1.Y, 0f);
					}
					else if (style == TickMarkStyle.Outside)
					{
						point1.X = x;
						point2.X = x;
						point3D = new Point3D(x, point1.Y, marksZPosition);
						point3D2 = new Point3D(x, point1.Y, 0f - size - chartArea.areaSceneWallWidth.Width);
					}
					else if (style == TickMarkStyle.Cross)
					{
						point1.X = x;
						point2.X = x + size / 2f;
						point3D = new Point3D(x, point1.Y, marksZPosition);
						point3D2 = new Point3D(x, point1.Y, (0f - size) / 2f - chartArea.areaSceneWallWidth.Width);
					}
					if (chartArea.ShouldDrawOnSurface(SurfaceNames.Left, backElements, onEdge: false))
					{
						point3D = null;
						point3D2 = null;
					}
				}
				else if (axis.AxisPosition == AxisPosition.Right && chartArea.IsSideSceneWallOnLeft())
				{
					float num = axis.PlotAreaPosition.Right();
					if (style == TickMarkStyle.Inside)
					{
						point1.X = num - size;
						point2.X = num;
						point3D = new Point3D(point2.X, point2.Y, 0f - chartArea.areaSceneWallWidth.Width);
						point3D2 = new Point3D(point2.X, point2.Y, 0f);
					}
					else if (style == TickMarkStyle.Outside)
					{
						point1.X = num;
						point2.X = num;
						point3D = new Point3D(num, point1.Y, marksZPosition);
						point3D2 = new Point3D(num, point1.Y, 0f - size - chartArea.areaSceneWallWidth.Width);
					}
					else if (style == TickMarkStyle.Cross)
					{
						point1.X = num - size / 2f;
						point2.X = num;
						point3D = new Point3D(num, point1.Y, marksZPosition);
						point3D2 = new Point3D(num, point1.Y, (0f - size) / 2f - chartArea.areaSceneWallWidth.Width);
					}
					if (chartArea.ShouldDrawOnSurface(SurfaceNames.Right, backElements, onEdge: false))
					{
						point3D = null;
						point3D2 = null;
					}
				}
			}
			graph.Draw3DLine(chartArea.matrix3D, borderColor, borderWidth, borderStyle, new Point3D(point1.X, point1.Y, marksZPosition), new Point3D(point2.X, point2.Y, marksZPosition), axis.Common, this, ChartElementType.TickMarks);
			if (point3D != null && point3D2 != null)
			{
				graph.Draw3DLine(chartArea.matrix3D, borderColor, borderWidth, borderStyle, point3D, point3D2, axis.Common, this, ChartElementType.TickMarks);
			}
		}
	}
}
