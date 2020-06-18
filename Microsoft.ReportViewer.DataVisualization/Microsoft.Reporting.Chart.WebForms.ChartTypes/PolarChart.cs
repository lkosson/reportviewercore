using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Microsoft.Reporting.Chart.WebForms.ChartTypes
{
	internal class PolarChart : RadarChart
	{
		public override string Name => "Polar";

		public override bool RequireClosedFigure()
		{
			return false;
		}

		public override bool XAxisCrossingSupported()
		{
			return true;
		}

		public override bool XAxisLabelsSupported()
		{
			return true;
		}

		public override bool RadialGridLinesSupported()
		{
			return true;
		}

		public override int GetNumerOfSectors(ChartArea area, SeriesCollection seriesCollection)
		{
			int result = 12;
			double interval = area.AxisX.Interval;
			if (area.AxisX.LabelStyle.Interval > 0.0)
			{
				interval = area.AxisX.LabelStyle.Interval;
			}
			if (interval != 0.0)
			{
				double num = area.AxisX.autoMaximum ? 360.0 : area.AxisX.Maximum;
				double num2 = area.AxisX.autoMinimum ? 0.0 : area.AxisX.Minimum;
				result = (int)(Math.Abs(num - num2) / interval);
			}
			return result;
		}

		public override float[] GetYAxisLocations(ChartArea area)
		{
			float[] array = new float[1]
			{
				0f
			};
			if (!double.IsNaN(area.AxisX.Crossing))
			{
				array[0] = (float)area.AxisX.Crossing;
				while (array[0] < 0f)
				{
					array[0] = 360f + array[0];
				}
			}
			return array;
		}

		protected override RadarDrawingStyle GetDrawingStyle(Series ser, DataPoint point)
		{
			RadarDrawingStyle result = RadarDrawingStyle.Line;
			if (point.IsAttributeSet("PolarDrawingStyle") || ser.IsAttributeSet("PolarDrawingStyle"))
			{
				string text = point.IsAttributeSet("PolarDrawingStyle") ? point["PolarDrawingStyle"] : ser["PolarDrawingStyle"];
				if (string.Compare(text, "Line", StringComparison.OrdinalIgnoreCase) == 0)
				{
					result = RadarDrawingStyle.Line;
				}
				else
				{
					if (string.Compare(text, "Marker", StringComparison.OrdinalIgnoreCase) != 0)
					{
						throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid(text, "PolarDrawingStyle"));
					}
					result = RadarDrawingStyle.Marker;
				}
			}
			return result;
		}

		protected override PointF[] GetPointsPosition(ChartGraphics graph, ChartArea area, Series series)
		{
			PointF[] array = new PointF[series.Points.Count + 1];
			int num = 0;
			foreach (DataPoint point in series.Points)
			{
				double yValue = GetYValue(common, area, series, point, num, 0);
				double position = area.AxisY.GetPosition(yValue);
				double num2 = area.circularCenter.X;
				array[num] = graph.GetAbsolutePoint(new PointF((float)num2, (float)position));
				float angle = area.CircularPositionToAngle(point.XValue);
				Matrix matrix = new Matrix();
				matrix.RotateAt(angle, graph.GetAbsolutePoint(area.circularCenter));
				PointF[] array2 = new PointF[1]
				{
					array[num]
				};
				matrix.TransformPoints(array2);
				array[num] = array2[0];
				num++;
			}
			array[num] = graph.GetAbsolutePoint(area.circularCenter);
			return array;
		}
	}
}
