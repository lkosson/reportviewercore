using Microsoft.Reporting.Chart.WebForms;
using Microsoft.Reporting.Chart.WebForms.Data;
using Microsoft.Reporting.Chart.WebForms.Utilities;
using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;

namespace Microsoft.Reporting.Chart.Helpers
{
	internal class CollectedPieHelper
	{
		public double CollectedPercentage = 5.0;

		protected RectangleF ChartAreaPosition = new RectangleF(5f, 5f, 90f, 90f);

		public bool ShowCollectedDataAsOneSlice;

		public Color SliceColor = Color.Empty;

		public float ChartAreaSpacing = 5f;

		public float SupplementedAreaSizeRatio = 0.9f;

		public Color ConnectionLinesColor = Color.FromArgb(64, 64, 64);

		public string CollectedLabel = "Other";

		public bool ShowCollectedLegend;

		public bool ShowCollectedPointLabels;

		private Microsoft.Reporting.Chart.WebForms.Chart chartControl;

		private Series series;

		private Series supplementalSeries;

		private ChartArea originalChartArea;

		private ChartArea supplementalChartArea;

		private float collectedPieSliceAngle;

		private bool ignorePaintEvent = true;

		public CollectedPieHelper(Microsoft.Reporting.Chart.WebForms.Chart chartControl)
		{
			this.chartControl = chartControl;
			this.chartControl.PostPaint += chart_PostPaint;
		}

		public void ShowSmallSegmentsAsSupplementalPie(Series collectedSeries)
		{
			series = collectedSeries;
			if (chartControl == null)
			{
				throw new ArgumentNullException("chartControl");
			}
			if (CollectedPercentage > 100.0 || CollectedPercentage < 0.0)
			{
				throw new ArgumentException("Value must be in range from 0 to 100 percent.", "CollectedPercentage");
			}
			if (series.ChartType != SeriesChartType.Pie && series.ChartType != SeriesChartType.Doughnut)
			{
				throw new InvalidOperationException("Only series with Pie or Doughnut chart type can be used.");
			}
			if (series.Points.Count == 0)
			{
				throw new InvalidOperationException("Cannot perform operatiuon on an empty series.");
			}
			supplementalChartArea = null;
			ignorePaintEvent = true;
			if (!CreateCollectedPie())
			{
				return;
			}
			float num = (ChartAreaPosition.Width - ChartAreaSpacing) / 2f * SupplementedAreaSizeRatio;
			originalChartArea = chartControl.ChartAreas[series.ChartArea];
			foreach (Legend legend in chartControl.Legends)
			{
				legend.Position.Auto = false;
				legend.DockInsideChartArea = false;
				legend.DockToChartArea = "";
			}
			foreach (Title title in chartControl.Titles)
			{
				title.Position.Auto = false;
				title.DockInsideChartArea = false;
				title.DockToChartArea = "";
			}
			originalChartArea.Position.X = ChartAreaPosition.X;
			originalChartArea.Position.Y = ChartAreaPosition.Y;
			originalChartArea.Position.Width = ChartAreaPosition.Width - num - ChartAreaSpacing;
			originalChartArea.Position.Height = ChartAreaPosition.Height;
			originalChartArea.Area3DStyle.Enable3D = false;
			supplementalChartArea = new ChartArea();
			supplementalChartArea.Name = originalChartArea.Name + "_Supplemental";
			supplementalChartArea.Position.X = originalChartArea.Position.Right() + ChartAreaSpacing;
			supplementalChartArea.Position.Y = ChartAreaPosition.Y;
			supplementalChartArea.Position.Width = num;
			supplementalChartArea.Position.Height = ChartAreaPosition.Height;
			supplementalSeries.ChartArea = supplementalChartArea.Name;
			chartControl.ChartAreas.Add(supplementalChartArea);
			supplementalChartArea.BackColor = originalChartArea.BackColor;
			supplementalChartArea.BorderColor = originalChartArea.BorderColor;
			supplementalChartArea.BorderWidth = originalChartArea.BorderWidth;
			supplementalChartArea.ShadowOffset = originalChartArea.ShadowOffset;
			foreach (ChartArea chartArea in chartControl.ChartAreas)
			{
				chartArea.Position.Auto = false;
			}
			ignorePaintEvent = false;
		}

		private bool CreateCollectedPie()
		{
			supplementalSeries = new Series();
			double num = 0.0;
			foreach (DataPoint point in series.Points)
			{
				if (!point.Empty && !double.IsNaN(point.YValues[0]))
				{
					num += Math.Abs(point.YValues[0]);
				}
			}
			double num2 = num / 100.0 * CollectedPercentage;
			int num3 = 0;
			int num4 = 0;
			for (int i = 0; i < series.Points.Count; i++)
			{
				if (series.Points[i].Empty || double.IsNaN(series.Points[i].YValues[0]))
				{
					num4++;
				}
				else if (Math.Abs(series.Points[i].YValues[0]) <= num2)
				{
					num3++;
				}
			}
			if (series.Points.Count - num4 - num3 <= 1 || num3 <= 1)
			{
				return false;
			}
			DataPoint dataPoint2 = null;
			if (ShowCollectedDataAsOneSlice)
			{
				dataPoint2 = series.Points[0].Clone();
				dataPoint2.YValues[0] = 0.0;
				series.Points.Add(dataPoint2);
			}
			double num5 = 0.0;
			for (int j = 0; j < series.Points.Count; j++)
			{
				if (!series.Points[j].Empty && !double.IsNaN(series.Points[j].YValues[0]))
				{
					double num6 = Math.Abs(series.Points[j].YValues[0]);
					if (num6 <= num2 && series.Points[j] != dataPoint2)
					{
						num5 += num6;
						supplementalSeries.Points.Add(series.Points[j].Clone());
						series.Points.RemoveAt(j);
						j--;
					}
				}
			}
			if (num5 > 0.0)
			{
				supplementalSeries.Name = series.Name + "_Supplemental";
				supplementalSeries.ChartArea = series.ChartArea;
				chartControl.Series.Add(supplementalSeries);
				supplementalSeries.ChartType = series.ChartType;
				supplementalSeries.Palette = series.Palette;
				supplementalSeries.ShadowOffset = series.ShadowOffset;
				supplementalSeries.BorderColor = series.BorderColor;
				supplementalSeries.BorderWidth = series.BorderWidth;
				supplementalSeries.ShowLabelAsValue = series.ShowLabelAsValue;
				supplementalSeries.LabelBackColor = series.LabelBackColor;
				supplementalSeries.LabelBorderColor = series.LabelBorderColor;
				supplementalSeries.LabelBorderWidth = series.LabelBorderWidth;
				supplementalSeries.Label = series.Label;
				supplementalSeries.LabelFormat = series.LabelFormat;
				supplementalSeries.labelBorderStyle = series.LabelBorderStyle;
				supplementalSeries.Font = series.Font;
				supplementalSeries.CustomAttributes = series.CustomAttributes;
				if (ShowCollectedLegend)
				{
					supplementalSeries.Legend = series.Legend;
					supplementalSeries.ShowInLegend = true;
					foreach (DataPoint point2 in supplementalSeries.Points)
					{
						point2.ShowInLegend = true;
					}
				}
				else
				{
					supplementalSeries.ShowInLegend = false;
					foreach (DataPoint point3 in supplementalSeries.Points)
					{
						point3.ShowInLegend = false;
					}
				}
				if (!ShowCollectedPointLabels)
				{
					supplementalSeries.Label = string.Empty;
					supplementalSeries.LabelFormat = string.Empty;
					supplementalSeries.ShowLabelAsValue = false;
					supplementalSeries.SetAttribute("AutoAxisLabels", "false");
					foreach (DataPoint point4 in supplementalSeries.Points)
					{
						point4.ShowLabelAsValue = false;
					}
				}
				else
				{
					supplementalSeries.ShowLabelAsValue = true;
				}
				if (ShowCollectedDataAsOneSlice)
				{
					dataPoint2.YValues[0] = num5;
					dataPoint2.Label = CollectedLabel;
					dataPoint2.AxisLabel = CollectedLabel;
					dataPoint2.LegendText = CollectedLabel;
					if (!SliceColor.IsEmpty)
					{
						dataPoint2.Color = SliceColor;
					}
				}
				collectedPieSliceAngle = (float)(3.5999999046325684 * (num5 / (num / 100.0)));
				int num7 = (int)Math.Round((double)collectedPieSliceAngle / 2.0);
				series["PieStartAngle"] = num7.ToString(CultureInfo.InvariantCulture);
				ApplyPaletteColors();
				MemoryStream imageStream = new MemoryStream();
				chartControl.Save(imageStream);
				ChartAreaPosition = new RectangleF(chartControl.ChartAreas[series.ChartArea].Position.X, chartControl.ChartAreas[series.ChartArea].Position.Y, chartControl.ChartAreas[series.ChartArea].Position.Width, chartControl.ChartAreas[series.ChartArea].Position.Height);
				return true;
			}
			if (dataPoint2 != null)
			{
				series.Points.Remove(dataPoint2);
			}
			return false;
		}

		private void ApplyPaletteColors()
		{
			ChartColorPalette palette = series.Palette;
			DataManager dataManager = (DataManager)series.serviceContainer.GetService(typeof(DataManager));
			if (palette == ChartColorPalette.None)
			{
				palette = dataManager.Palette;
			}
			if (palette == ChartColorPalette.None && dataManager.PaletteCustomColors.Length == 0)
			{
				return;
			}
			int num = 0;
			Color[] array = (dataManager.PaletteCustomColors.Length != 0) ? dataManager.PaletteCustomColors : ChartPaletteColors.GetPaletteColors(palette);
			ArrayList arrayList = new ArrayList(series.Points);
			arrayList.AddRange(supplementalSeries.Points);
			foreach (DataPoint item in arrayList)
			{
				if (!item.Empty && !double.IsNaN(item.YValues[0]) && (!item.IsAttributeSet(CommonAttributes.Color) || item.tempColorIsSet))
				{
					item.Color = array[num];
					num++;
					if (num >= array.Length)
					{
						num = 0;
					}
				}
			}
		}

		private void chart_PostPaint(object sender, ChartPaintEventArgs e)
		{
			if (ignorePaintEvent || !(sender is ChartArea))
			{
				return;
			}
			ChartArea chartArea = (ChartArea)sender;
			if (supplementalChartArea != null && chartArea.Name == supplementalChartArea.Name)
			{
				RectangleF chartAreaPlottingPosition = GetChartAreaPlottingPosition(originalChartArea, e.ChartGraphics);
				RectangleF chartAreaPlottingPosition2 = GetChartAreaPlottingPosition(supplementalChartArea, e.ChartGraphics);
				PointF rotatedPlotAreaPoint = GetRotatedPlotAreaPoint(chartAreaPlottingPosition2, 325f);
				PointF rotatedPlotAreaPoint2 = GetRotatedPlotAreaPoint(chartAreaPlottingPosition2, 215f);
				PointF rotatedPlotAreaPoint3 = GetRotatedPlotAreaPoint(chartAreaPlottingPosition, 90f - collectedPieSliceAngle / 2f);
				PointF rotatedPlotAreaPoint4 = GetRotatedPlotAreaPoint(chartAreaPlottingPosition, 90f + collectedPieSliceAngle / 2f);
				using (Pen pen = new Pen(ConnectionLinesColor, 1f))
				{
					e.ChartGraphics.DrawLine(pen, rotatedPlotAreaPoint, rotatedPlotAreaPoint3);
					e.ChartGraphics.DrawLine(pen, rotatedPlotAreaPoint2, rotatedPlotAreaPoint4);
				}
			}
		}

		private PointF GetRotatedPlotAreaPoint(RectangleF areaPosition, float angle)
		{
			PointF[] array = new PointF[1]
			{
				new PointF(areaPosition.X + areaPosition.Width / 2f, areaPosition.Y)
			};
			using (Matrix matrix = new Matrix())
			{
				matrix.RotateAt(angle, new PointF(areaPosition.X + areaPosition.Width / 2f, areaPosition.Y + areaPosition.Height / 2f));
				matrix.TransformPoints(array);
			}
			return array[0];
		}

		private RectangleF GetChartAreaPlottingPosition(ChartArea area, ChartGraphics chartGraphics)
		{
			RectangleF relative = area.Position.ToRectangleF();
			relative.X += area.Position.Width / 100f * area.InnerPlotPosition.X;
			relative.Y += area.Position.Height / 100f * area.InnerPlotPosition.Y;
			relative.Width = area.Position.Width / 100f * area.InnerPlotPosition.Width;
			relative.Height = area.Position.Height / 100f * area.InnerPlotPosition.Height;
			return chartGraphics.GetAbsoluteRectangle(relative);
		}
	}
}
