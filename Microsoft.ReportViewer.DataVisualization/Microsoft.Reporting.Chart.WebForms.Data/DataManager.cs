using Microsoft.Reporting.Chart.WebForms.ChartTypes;
using Microsoft.Reporting.Chart.WebForms.Design;
using Microsoft.Reporting.Chart.WebForms.Utilities;
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;

namespace Microsoft.Reporting.Chart.WebForms.Data
{
	internal class DataManager : IServiceProvider
	{
		private SeriesCollection series;

		internal IServiceContainer serviceContainer;

		private ChartColorPalette colorPalette = ChartColorPalette.BrightPastel;

		private Color[] paletteCustomColors = new Color[0];

		[SRCategory("CategoryAttributeData")]
		[Bindable(true)]
		public SeriesCollection Series => series;

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributePalette")]
		[DefaultValue(ChartColorPalette.BrightPastel)]
		public ChartColorPalette Palette
		{
			get
			{
				return colorPalette;
			}
			set
			{
				colorPalette = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[SerializationVisibility(SerializationVisibility.Attribute)]
		[SRDescription("DescriptionAttributeDataManager_PaletteCustomColors")]
		[TypeConverter(typeof(ColorArrayConverter))]
		public Color[] PaletteCustomColors
		{
			get
			{
				return paletteCustomColors;
			}
			set
			{
				paletteCustomColors = value;
			}
		}

		private DataManager()
		{
		}

		public DataManager(IServiceContainer container)
		{
			if (container == null)
			{
				throw new ArgumentNullException(SR.ExceptionInvalidServiceContainer);
			}
			serviceContainer = container;
			series = new SeriesCollection(container);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public object GetService(Type serviceType)
		{
			if (serviceType == typeof(DataManager))
			{
				return this;
			}
			throw new ArgumentException(SR.ExceptionDataManagerUnsupportedType(serviceType.ToString()));
		}

		internal void Initialize()
		{
			ChartImage obj = (ChartImage)serviceContainer.GetService(typeof(ChartImage));
			obj.BeforePaint += ChartPicture_BeforePaint;
			obj.AfterPaint += ChartPicture_AfterPaint;
		}

		private void ChartPicture_BeforePaint(object sender, ChartPaintEventArgs e)
		{
			int num = 1;
			for (int i = 0; i < Series.Count; i++)
			{
				Series series = Series[i];
				series.xValuesZerosChecked = false;
				series.xValuesZeros = false;
				IChartType chartType = e.CommonElements.ChartTypeRegistry.GetChartType(series.ChartTypeName);
				bool pointsApplyPaletteColors = chartType.ApplyPaletteColorsToPoints;
				if (series.Palette != 0)
				{
					pointsApplyPaletteColors = true;
				}
				PrepareData(Palette != 0 || PaletteCustomColors.Length != 0, pointsApplyPaletteColors, series.Name);
				if (series.tempMarkerStyleIsSet)
				{
					series.MarkerStyle = MarkerStyle.None;
					series.tempMarkerStyleIsSet = false;
				}
				if (chartType.GetLegendImageStyle(series) == LegendImageStyle.Marker && series.MarkerStyle == MarkerStyle.None)
				{
					series.MarkerStyle = (MarkerStyle)(num++);
					series.tempMarkerStyleIsSet = true;
					if (num > 9)
					{
						num = 1;
					}
				}
			}
		}

		private void ChartPicture_AfterPaint(object sender, ChartPaintEventArgs e)
		{
			if ((Chart)serviceContainer.GetService(typeof(Chart)) == null)
			{
				return;
			}
			for (int i = 0; i < Series.Count; i++)
			{
				if (Series[i].UnPrepareData(null))
				{
					i--;
				}
			}
		}

		internal void ApplyPaletteColors()
		{
			if (Palette == ChartColorPalette.None && PaletteCustomColors.Length == 0)
			{
				return;
			}
			int num = 0;
			Color[] array = (PaletteCustomColors.Length != 0) ? PaletteCustomColors : ChartPaletteColors.GetPaletteColors(colorPalette);
			foreach (Series item in this.series)
			{
				bool flag = false;
				if (item.ChartArea.Length > 0)
				{
					ChartImage chartImage = (ChartImage)serviceContainer.GetService(typeof(ChartImage));
					if (chartImage != null)
					{
						foreach (ChartArea chartArea in chartImage.ChartAreas)
						{
							if (chartArea.Name == item.ChartArea)
							{
								flag = true;
								break;
							}
						}
						if (!flag && item.ChartArea == "Default" && chartImage.ChartAreas.Count > 0)
						{
							flag = true;
						}
					}
				}
				if (flag && (item.Color == Color.Empty || item.tempColorIsSet))
				{
					item.color = array[num++];
					item.tempColorIsSet = true;
					if (num >= array.Length)
					{
						num = 0;
					}
				}
			}
		}

		internal void PrepareData(bool seriesApplyPaletteColors, bool pointsApplyPaletteColors, params string[] series)
		{
			if (seriesApplyPaletteColors)
			{
				ApplyPaletteColors();
			}
			if ((Chart)serviceContainer.GetService(typeof(Chart)) != null)
			{
				foreach (string parameter in series)
				{
					Series[parameter].PrepareData(null, pointsApplyPaletteColors);
				}
			}
		}

		private bool IsPointSkipped(DataPoint point)
		{
			if (point.Empty)
			{
				return true;
			}
			return false;
		}

		internal int GetNumberOfPoints(params string[] series)
		{
			int num = 0;
			foreach (string parameter in series)
			{
				num = Math.Max(num, this.series[parameter].Points.Count);
			}
			return num;
		}

		internal double GetMaxYValue(int valueIndex, params string[] series)
		{
			double num = double.MinValue;
			foreach (string parameter in series)
			{
				foreach (DataPoint point in this.series[parameter].Points)
				{
					if (!IsPointSkipped(point) && !double.IsNaN(point.YValues[valueIndex]))
					{
						num = Math.Max(num, point.YValues[valueIndex]);
					}
				}
			}
			return num;
		}

		internal double GetMaxYWithRadiusValue(ChartArea area, params string[] series)
		{
			double num = double.MinValue;
			foreach (string parameter in series)
			{
				foreach (DataPoint point in this.series[parameter].Points)
				{
					if (!IsPointSkipped(point) && !double.IsNaN(point.YValues[0]))
					{
						num = ((point.YValues.Length <= 1) ? Math.Max(num, point.YValues[0]) : Math.Max(num, point.YValues[0] + BubbleChart.AxisScaleBubbleSize(area.Common.graph, area.Common, area, point.YValues[1], yValue: true)));
					}
				}
			}
			return num;
		}

		internal double GetMaxXWithRadiusValue(ChartArea area, params string[] series)
		{
			double num = double.MinValue;
			foreach (string parameter in series)
			{
				foreach (DataPoint point in this.series[parameter].Points)
				{
					if (!point.EmptyX && (!this.series[parameter].EmptyX || point.XValue != 0.0) && !IsPointSkipped(point) && !double.IsNaN(point.XValue))
					{
						num = ((point.YValues.Length <= 1) ? Math.Max(num, point.XValue) : Math.Max(num, point.XValue + BubbleChart.AxisScaleBubbleSize(area.Common.graph, area.Common, area, point.XValue, yValue: false)));
					}
				}
			}
			return num;
		}

		internal double GetMinXWithRadiusValue(ChartArea area, params string[] series)
		{
			double num = double.MaxValue;
			foreach (string parameter in series)
			{
				foreach (DataPoint point in this.series[parameter].Points)
				{
					if (!point.EmptyX && (!this.series[parameter].EmptyX || point.XValue != 0.0) && !IsPointSkipped(point) && !double.IsNaN(point.XValue))
					{
						num = ((point.YValues.Length <= 1) ? Math.Min(num, point.XValue) : Math.Min(num, point.XValue - BubbleChart.AxisScaleBubbleSize(area.Common.graph, area.Common, area, point.YValues[1], yValue: false)));
					}
				}
			}
			return num;
		}

		internal double GetMaxYValue(params string[] series)
		{
			double num = double.MinValue;
			foreach (string parameter in series)
			{
				foreach (DataPoint point in this.series[parameter].Points)
				{
					if (IsPointSkipped(point))
					{
						continue;
					}
					double[] yValues = point.YValues;
					foreach (double num2 in yValues)
					{
						if (!double.IsNaN(num2))
						{
							num = Math.Max(num, num2);
						}
					}
				}
			}
			return num;
		}

		internal double GetMaxXValue(params string[] series)
		{
			double num = double.MinValue;
			foreach (string parameter in series)
			{
				foreach (DataPoint point in this.series[parameter].Points)
				{
					if (!point.EmptyX && (!this.series[parameter].EmptyX || point.XValue != 0.0))
					{
						num = Math.Max(num, point.XValue);
					}
				}
			}
			return num;
		}

		internal void GetMinMaxXValue(out double min, out double max, params string[] series)
		{
			max = double.MinValue;
			min = double.MaxValue;
			foreach (string parameter in series)
			{
				foreach (DataPoint point in this.series[parameter].Points)
				{
					max = Math.Max(max, point.XValue);
					min = Math.Min(min, point.XValue);
				}
			}
		}

		internal void GetMinMaxYValue(int valueIndex, out double min, out double max, params string[] series)
		{
			max = double.MinValue;
			min = double.MaxValue;
			foreach (string parameter in series)
			{
				foreach (DataPoint point in this.series[parameter].Points)
				{
					if (!IsPointSkipped(point))
					{
						double num = point.YValues[valueIndex];
						if (!double.IsNaN(num))
						{
							max = Math.Max(max, num);
							min = Math.Min(min, num);
						}
					}
				}
			}
		}

		internal void GetMinMaxYValue(out double min, out double max, params string[] series)
		{
			max = double.MinValue;
			min = double.MaxValue;
			foreach (string parameter in series)
			{
				foreach (DataPoint point in this.series[parameter].Points)
				{
					if (IsPointSkipped(point))
					{
						continue;
					}
					double[] yValues = point.YValues;
					foreach (double num in yValues)
					{
						if (!double.IsNaN(num))
						{
							max = Math.Max(max, num);
							min = Math.Min(min, num);
						}
					}
				}
			}
		}

		internal void GetMinMaxYValue(ArrayList seriesList, out double min, out double max)
		{
			max = double.MinValue;
			min = double.MaxValue;
			foreach (Series series2 in seriesList)
			{
				foreach (DataPoint point in series2.Points)
				{
					if (IsPointSkipped(point))
					{
						continue;
					}
					double[] yValues = point.YValues;
					foreach (double num in yValues)
					{
						if (!double.IsNaN(num))
						{
							max = Math.Max(max, num);
							min = Math.Min(min, num);
						}
					}
				}
			}
		}

		internal double GetMaxStackedYValue(int valueIndex, params string[] series)
		{
			double num = 0.0;
			double num2 = GetNumberOfPoints(series);
			for (int i = 0; (double)i < num2; i++)
			{
				double num3 = 0.0;
				double num4 = 0.0;
				foreach (string parameter in series)
				{
					if (this.series[parameter].Points.Count <= i)
					{
						continue;
					}
					IChartType chartType = ((ChartTypeRegistry)serviceContainer.GetService(typeof(ChartTypeRegistry))).GetChartType(this.series[parameter].ChartTypeName);
					if (!chartType.StackSign)
					{
						continue;
					}
					if (chartType.Stacked)
					{
						if (this.series[parameter].Points[i].YValues[valueIndex] > 0.0)
						{
							num3 += this.series[parameter].Points[i].YValues[valueIndex];
						}
					}
					else
					{
						num4 = Math.Max(num4, this.series[parameter].Points[i].YValues[valueIndex]);
					}
				}
				num3 = Math.Max(num3, num4);
				num = Math.Max(num, num3);
			}
			return num;
		}

		internal double GetMaxUnsignedStackedYValue(int valueIndex, params string[] series)
		{
			double num = 0.0;
			double num2 = double.MinValue;
			double num3 = GetNumberOfPoints(series);
			for (int i = 0; (double)i < num3; i++)
			{
				double num4 = 0.0;
				double num5 = 0.0;
				foreach (string parameter in series)
				{
					if (this.series[parameter].Points.Count <= i)
					{
						continue;
					}
					IChartType chartType = ((ChartTypeRegistry)serviceContainer.GetService(typeof(ChartTypeRegistry))).GetChartType(this.series[parameter].ChartTypeName);
					if (chartType.StackSign || double.IsNaN(this.series[parameter].Points[i].YValues[valueIndex]))
					{
						continue;
					}
					if (chartType.Stacked)
					{
						num2 = double.MinValue;
						num4 += this.series[parameter].Points[i].YValues[valueIndex];
						if (num4 > num2)
						{
							num2 = num4;
						}
					}
					else
					{
						num5 = Math.Max(num5, this.series[parameter].Points[i].YValues[valueIndex]);
					}
				}
				num2 = Math.Max(num2, num5);
				num = Math.Max(num, num2);
			}
			return num;
		}

		internal double GetMaxStackedXValue(params string[] series)
		{
			double num = 0.0;
			double num2 = GetNumberOfPoints(series);
			for (int i = 0; (double)i < num2; i++)
			{
				double num3 = 0.0;
				foreach (string parameter in series)
				{
					if (this.series[parameter].Points.Count > i && this.series[parameter].Points[i].XValue > 0.0)
					{
						num3 += this.series[parameter].Points[i].XValue;
					}
				}
				num = Math.Max(num, num3);
			}
			return num;
		}

		internal double GetMinYValue(int valueIndex, params string[] series)
		{
			double num = double.MaxValue;
			foreach (string parameter in series)
			{
				foreach (DataPoint point in this.series[parameter].Points)
				{
					if (!IsPointSkipped(point) && !double.IsNaN(point.YValues[valueIndex]))
					{
						num = Math.Min(num, point.YValues[valueIndex]);
					}
				}
			}
			return num;
		}

		internal double GetMinYWithRadiusValue(ChartArea area, params string[] series)
		{
			double num = double.MaxValue;
			foreach (string parameter in series)
			{
				foreach (DataPoint point in this.series[parameter].Points)
				{
					if (!IsPointSkipped(point) && !double.IsNaN(point.YValues[0]))
					{
						num = ((point.YValues.Length <= 1) ? Math.Min(num, point.YValues[0]) : Math.Min(num, point.YValues[0] - BubbleChart.AxisScaleBubbleSize(area.Common.graph, area.Common, area, point.YValues[1], yValue: true)));
					}
				}
			}
			return num;
		}

		internal double GetMinYValue(params string[] series)
		{
			double num = double.MaxValue;
			foreach (string parameter in series)
			{
				foreach (DataPoint point in this.series[parameter].Points)
				{
					if (IsPointSkipped(point))
					{
						continue;
					}
					double[] yValues = point.YValues;
					foreach (double num2 in yValues)
					{
						if (!double.IsNaN(num2))
						{
							num = Math.Min(num, num2);
						}
					}
				}
			}
			return num;
		}

		internal double GetMinXValue(params string[] series)
		{
			double num = double.MaxValue;
			foreach (string parameter in series)
			{
				foreach (DataPoint point in this.series[parameter].Points)
				{
					if (!point.EmptyX && (!this.series[parameter].EmptyX || point.XValue != 0.0))
					{
						num = Math.Min(num, point.XValue);
					}
				}
			}
			return num;
		}

		internal double GetMinStackedYValue(int valueIndex, params string[] series)
		{
			double num = double.MaxValue;
			double num2 = GetNumberOfPoints(series);
			for (int i = 0; (double)i < num2; i++)
			{
				double num3 = 0.0;
				double num4 = 0.0;
				foreach (string parameter in series)
				{
					if (this.series[parameter].Points.Count <= i)
					{
						continue;
					}
					IChartType chartType = ((ChartTypeRegistry)serviceContainer.GetService(typeof(ChartTypeRegistry))).GetChartType(this.series[parameter].ChartTypeName);
					if (!chartType.StackSign || double.IsNaN(this.series[parameter].Points[i].YValues[valueIndex]))
					{
						continue;
					}
					if (chartType.Stacked)
					{
						if (this.series[parameter].Points[i].YValues[valueIndex] < 0.0)
						{
							num3 += this.series[parameter].Points[i].YValues[valueIndex];
						}
					}
					else
					{
						num4 = Math.Min(num4, this.series[parameter].Points[i].YValues[valueIndex]);
					}
				}
				num3 = Math.Min(num3, num4);
				if (num3 == 0.0)
				{
					num3 = this.series[series[0]].Points[this.series[series[0]].Points.Count - 1].YValues[valueIndex];
				}
				num = Math.Min(num, num3);
			}
			return num;
		}

		internal double GetMinUnsignedStackedYValue(int valueIndex, params string[] series)
		{
			double num = double.MaxValue;
			double num2 = double.MaxValue;
			double num3 = GetNumberOfPoints(series);
			for (int i = 0; (double)i < num3; i++)
			{
				double num4 = 0.0;
				double val = 0.0;
				num2 = double.MaxValue;
				foreach (string parameter in series)
				{
					if (this.series[parameter].Points.Count <= i)
					{
						continue;
					}
					IChartType chartType = ((ChartTypeRegistry)serviceContainer.GetService(typeof(ChartTypeRegistry))).GetChartType(this.series[parameter].ChartTypeName);
					if (chartType.StackSign || double.IsNaN(this.series[parameter].Points[i].YValues[valueIndex]))
					{
						continue;
					}
					if (chartType.Stacked)
					{
						if (this.series[parameter].Points[i].YValues[valueIndex] < 0.0)
						{
							num4 += this.series[parameter].Points[i].YValues[valueIndex];
							if (num4 < num2)
							{
								num2 = num4;
							}
						}
					}
					else
					{
						val = Math.Min(val, this.series[parameter].Points[i].YValues[valueIndex]);
					}
				}
				num2 = Math.Min(val, num2);
				num = Math.Min(num, num2);
			}
			return num;
		}

		internal double GetMinStackedXValue(params string[] series)
		{
			double num = 0.0;
			double num2 = GetNumberOfPoints(series);
			for (int i = 0; (double)i < num2; i++)
			{
				double num3 = 0.0;
				foreach (string parameter in series)
				{
					if (this.series[parameter].Points[i].XValue < 0.0)
					{
						num3 += this.series[parameter].Points[i].XValue;
					}
				}
				num = Math.Min(num, num3);
			}
			return num;
		}

		internal double GetMaxHundredPercentStackedYValue(bool supportNegative, int valueIndex, params string[] series)
		{
			double num = 0.0;
			Series[] array = new Series[series.Length];
			int num2 = 0;
			foreach (string parameter in series)
			{
				array[num2++] = this.series[parameter];
			}
			try
			{
				for (int j = 0; j < this.series[series[0]].Points.Count; j++)
				{
					double num3 = 0.0;
					double num4 = 0.0;
					Series[] array2 = array;
					foreach (Series series2 in array2)
					{
						num3 = ((!supportNegative) ? (num3 + series2.Points[j].YValues[0]) : (num3 + Math.Abs(series2.Points[j].YValues[0])));
						if (series2.Points[j].YValues[0] > 0.0 || !supportNegative)
						{
							num4 += series2.Points[j].YValues[0];
						}
					}
					num3 = Math.Abs(num3);
					if (num3 != 0.0)
					{
						num = Math.Max(num, num4 / num3 * 100.0);
					}
				}
				return num;
			}
			catch (Exception)
			{
				throw new InvalidOperationException(SR.ExceptionDataManager100StackedSeriesPointsNumeberMismatch);
			}
		}

		internal double GetMinHundredPercentStackedYValue(bool supportNegative, int valueIndex, params string[] series)
		{
			double num = 0.0;
			Series[] array = new Series[series.Length];
			int num2 = 0;
			foreach (string parameter in series)
			{
				array[num2++] = this.series[parameter];
			}
			try
			{
				for (int j = 0; j < this.series[series[0]].Points.Count; j++)
				{
					double num3 = 0.0;
					double num4 = 0.0;
					Series[] array2 = array;
					foreach (Series series2 in array2)
					{
						num3 = ((!supportNegative) ? (num3 + series2.Points[j].YValues[0]) : (num3 + Math.Abs(series2.Points[j].YValues[0])));
						if (series2.Points[j].YValues[0] < 0.0 || !supportNegative)
						{
							num4 += series2.Points[j].YValues[0];
						}
					}
					num3 = Math.Abs(num3);
					if (num3 != 0.0)
					{
						num = Math.Min(num, num4 / num3 * 100.0);
					}
				}
				return num;
			}
			catch (Exception)
			{
				throw new InvalidOperationException(SR.ExceptionDataManager100StackedSeriesPointsNumeberMismatch);
			}
		}
	}
}
