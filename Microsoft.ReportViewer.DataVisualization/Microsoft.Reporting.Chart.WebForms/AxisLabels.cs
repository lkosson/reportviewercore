using Microsoft.Reporting.Chart.WebForms.ChartTypes;
using Microsoft.Reporting.Chart.WebForms.Design;
using Microsoft.Reporting.Chart.WebForms.Utilities;
using System;
using System.Collections;
using System.ComponentModel;

namespace Microsoft.Reporting.Chart.WebForms
{
	internal class AxisLabels : AxisScale
	{
		private CustomLabelsCollection customLabels;

		[SRCategory("CategoryAttributeLabels")]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeLabelStyle")]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		public Label LabelStyle
		{
			get
			{
				return labelStyle;
			}
			set
			{
				labelStyle = value;
				labelStyle.axis = (Axis)this;
				CustomLabels.axis = (Axis)this;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeLabels")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeCustomLabels")]
		[Browsable(false)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public CustomLabelsCollection CustomLabels => customLabels;

		public AxisLabels()
		{
			labelStyle = new Label((Axis)this);
			customLabels = new CustomLabelsCollection((Axis)this);
		}

		internal bool IsCustomGridLines()
		{
			if (CustomLabels.Count > 0)
			{
				foreach (CustomLabel customLabel in CustomLabels)
				{
					if ((customLabel.GridTicks & GridTicks.Gridline) == GridTicks.Gridline)
					{
						return true;
					}
				}
			}
			return false;
		}

		internal bool IsCustomTickMarks()
		{
			if (CustomLabels.Count > 0)
			{
				foreach (CustomLabel customLabel in CustomLabels)
				{
					if ((customLabel.GridTicks & GridTicks.TickMark) == GridTicks.TickMark)
					{
						return true;
					}
				}
			}
			return false;
		}

		internal AxisType GetAxisType()
		{
			if (axisType == AxisName.X || axisType == AxisName.Y)
			{
				return AxisType.Primary;
			}
			return AxisType.Secondary;
		}

		internal ArrayList GetAxisSeries()
		{
			ArrayList arrayList = new ArrayList();
			foreach (string item in chartArea.Series)
			{
				Series series = base.Common.DataManager.Series[item];
				if (axisType == AxisName.X || axisType == AxisName.X2)
				{
					if (series.XAxisType == GetAxisType())
					{
						arrayList.Add(series);
					}
				}
				else if (series.YAxisType == GetAxisType())
				{
					arrayList.Add(series);
				}
			}
			return arrayList;
		}

		internal Axis GetOtherTypeAxis()
		{
			return chartArea.GetAxis(axisType, (GetAxisType() == AxisType.Primary) ? AxisType.Secondary : AxisType.Primary, string.Empty);
		}

		internal void PostFillLabels()
		{
			foreach (CustomLabel customLabel2 in CustomLabels)
			{
				if (customLabel2.customLabel)
				{
					return;
				}
			}
			if (!LabelStyle.Enabled || !enabled || !string.IsNullOrEmpty(((Axis)this).SubAxisName) || axisType == AxisName.Y || axisType == AxisName.Y2 || GetAxisSeries().Count > 0)
			{
				return;
			}
			CustomLabels.Clear();
			foreach (CustomLabel customLabel3 in GetOtherTypeAxis().CustomLabels)
			{
				CustomLabels.Add(customLabel3.Clone());
			}
		}

		internal void FillLabels(bool removeFirstRow)
		{
			if (!LabelStyle.Enabled || !enabled)
			{
				return;
			}
			if (chartArea != null && chartArea.chartAreaIsCurcular && axisType != AxisName.Y)
			{
				ICircularChartType circularChartType = chartArea.GetCircularChartType();
				if (circularChartType == null || !circularChartType.XAxisLabelsSupported())
				{
					return;
				}
			}
			bool flag = false;
			foreach (CustomLabel customLabel in CustomLabels)
			{
				if (customLabel.customLabel && (customLabel.RowIndex == 0 || chartArea.chartAreaIsCurcular))
				{
					flag = true;
				}
				if (customLabel.customLabel && base.Common.Chart != null && base.Common.Chart.LocalizeTextHandler != null)
				{
					customLabel.Text = base.Common.Chart.LocalizeTextHandler(customLabel, customLabel.Text, 0, ChartElementType.AxisLabels);
				}
			}
			if (removeFirstRow)
			{
				if (flag)
				{
					return;
				}
				for (int i = 0; i < CustomLabels.Count; i++)
				{
					if (CustomLabels[i].RowIndex == 0)
					{
						CustomLabels.RemoveAt(i);
						i = -1;
					}
				}
			}
			ArrayList arrayList = null;
			switch (axisType)
			{
			case AxisName.X:
				arrayList = chartArea.GetXAxesSeries(AxisType.Primary, ((Axis)this).SubAxisName);
				break;
			case AxisName.Y:
				arrayList = chartArea.GetYAxesSeries(AxisType.Primary, ((Axis)this).SubAxisName);
				break;
			case AxisName.X2:
				arrayList = chartArea.GetXAxesSeries(AxisType.Secondary, ((Axis)this).SubAxisName);
				break;
			case AxisName.Y2:
				arrayList = chartArea.GetYAxesSeries(AxisType.Secondary, ((Axis)this).SubAxisName);
				break;
			}
			if (arrayList.Count == 0)
			{
				return;
			}
			string[] array = new string[arrayList.Count];
			for (int j = 0; j < arrayList.Count; j++)
			{
				array[j] = (string)arrayList[j];
			}
			bool flag2 = SeriesXValuesZeros(array);
			bool indexedSeries = true;
			if (!flag2)
			{
				indexedSeries = IndexedSeries(array);
			}
			int num = 0;
			if (labelStyle.ShowEndLabels)
			{
				num = 1;
			}
			IChartType chartType = base.Common.ChartTypeRegistry.GetChartType(chartArea.GetFirstSeries().ChartTypeName);
			bool flag3 = false;
			if (!chartType.RequireAxes)
			{
				return;
			}
			flag3 = ((axisType != AxisName.Y && axisType != AxisName.Y2) ? true : false);
			if (flag3 && !SeriesXValuesZeros((string[])arrayList.ToArray(typeof(string))))
			{
				flag3 = false;
			}
			if (flag3 && (labelStyle.IntervalOffset != 0.0 || labelStyle.Interval != 0.0))
			{
				flag3 = false;
			}
			ChartValueTypes chartValueTypes = (axisType != 0 && axisType != AxisName.X2) ? base.Common.DataManager.Series[arrayList[0]].YValueType : base.Common.DataManager.Series[arrayList[0]].indexedXValueType;
			if (labelStyle.IntervalType != 0 && labelStyle.IntervalType != DateTimeIntervalType.Number && chartValueTypes != ChartValueTypes.Time && chartValueTypes != ChartValueTypes.Date && chartValueTypes != ChartValueTypes.DateTimeOffset)
			{
				chartValueTypes = ChartValueTypes.DateTime;
			}
			double viewMaximum = GetViewMaximum();
			double viewMinimum = GetViewMinimum();
			if (flag3)
			{
				int numberOfPoints = base.Common.DataManager.GetNumberOfPoints((string[])arrayList.ToArray(typeof(string)));
				if (num == 1)
				{
					CustomLabels.Add(-0.5, 0.5, ValueConverter.FormatValue(base.Common.Chart, this, 0.0, LabelStyle.Format, chartValueTypes, ChartElementType.AxisLabels), customLabel: false);
				}
				for (int k = 0; k < numberOfPoints; k++)
				{
					CustomLabels.Add((double)k + 0.5, (double)k + 1.5, ValueConverter.FormatValue(base.Common.Chart, this, k + 1, LabelStyle.Format, chartValueTypes, ChartElementType.AxisLabels), customLabel: false);
				}
				if (num == 1)
				{
					CustomLabels.Add((double)numberOfPoints + 0.5, (double)numberOfPoints + 1.5, ValueConverter.FormatValue(base.Common.Chart, this, numberOfPoints + 1, LabelStyle.Format, chartValueTypes, ChartElementType.AxisLabels), customLabel: false);
				}
				foreach (string item in arrayList)
				{
					int l = (num == 1) ? 1 : 0;
					foreach (DataPoint point in base.Common.DataManager.Series[item].Points)
					{
						for (; CustomLabels[l].RowIndex > 0; l++)
						{
						}
						if ((axisType == AxisName.X || axisType == AxisName.X2) && point.AxisLabel.Length > 0)
						{
							CustomLabels[l].Text = point.AxisLabel;
							if (base.Common.Chart != null && base.Common.Chart.LocalizeTextHandler != null)
							{
								CustomLabels[l].Text = base.Common.Chart.LocalizeTextHandler(point, CustomLabels[l].Text, point.ElementId, ChartElementType.DataPoint);
							}
						}
						l++;
					}
				}
			}
			else
			{
				if (viewMinimum == viewMaximum)
				{
					return;
				}
				Series series = null;
				if (axisType == AxisName.X || axisType == AxisName.X2)
				{
					ArrayList xAxesSeries = chartArea.GetXAxesSeries((axisType != 0) ? AxisType.Secondary : AxisType.Primary, ((Axis)this).SubAxisName);
					if (xAxesSeries.Count > 0)
					{
						series = base.Common.DataManager.Series[xAxesSeries[0]];
						if (series != null && !series.XValueIndexed)
						{
							series = null;
						}
					}
				}
				DateTimeIntervalType dateTimeIntervalType = (labelStyle.IntervalOffsetType == DateTimeIntervalType.Auto) ? labelStyle.IntervalType : labelStyle.IntervalOffsetType;
				double num2 = viewMinimum;
				if (!chartArea.chartAreaIsCurcular || axisType == AxisName.Y || axisType == AxisName.Y2)
				{
					num2 = AlignIntervalStart(num2, labelStyle.Interval, labelStyle.IntervalType, series);
				}
				if (labelStyle.IntervalOffset != 0.0 && series == null)
				{
					num2 += GetIntervalSize(num2, labelStyle.IntervalOffset, dateTimeIntervalType, series, 0.0, DateTimeIntervalType.Number, forceIntIndex: true, forceAbsInterval: false);
				}
				if (chartValueTypes == ChartValueTypes.DateTime || chartValueTypes == ChartValueTypes.Date || chartValueTypes == ChartValueTypes.Time || chartValueTypes == ChartValueTypes.DateTimeOffset || series != null)
				{
					double num3 = num2;
					if ((viewMaximum - num2) / GetIntervalSize(num2, labelStyle.Interval, labelStyle.IntervalType, series, 0.0, DateTimeIntervalType.Number, forceIntIndex: true) > 10000.0)
					{
						return;
					}
					int num4 = 0;
					double num5 = viewMaximum - GetIntervalSize(viewMaximum, labelStyle.Interval, labelStyle.IntervalType, series, labelStyle.IntervalOffset, dateTimeIntervalType, forceIntIndex: true) / 2.0;
					double num6 = viewMinimum + GetIntervalSize(viewMinimum, labelStyle.Interval, labelStyle.IntervalType, series, labelStyle.IntervalOffset, dateTimeIntervalType, forceIntIndex: true) / 2.0;
					while ((decimal)num3 <= (decimal)viewMaximum)
					{
						double intervalSize = GetIntervalSize(num3, labelStyle.Interval, labelStyle.IntervalType, series, labelStyle.IntervalOffset, dateTimeIntervalType, forceIntIndex: true);
						double num7 = num3;
						if (base.Logarithmic)
						{
							num7 = Math.Pow(logarithmBase, num7);
						}
						if (num4++ > 10000 || (num == 0 && num3 >= num5))
						{
							break;
						}
						double num8 = num3 - intervalSize * 0.5;
						double toPosition = num3 + intervalSize * 0.5;
						if (num == 0 && num3 <= num6)
						{
							num3 += intervalSize;
							continue;
						}
						if ((decimal)num8 > (decimal)viewMaximum)
						{
							num3 += intervalSize;
							continue;
						}
						string pointLabel = GetPointLabel(arrayList, num7, !flag2, indexedSeries);
						if (pointLabel.Length == 0)
						{
							if (num3 <= maximum && (num3 != maximum || !base.Common.DataManager.Series[arrayList[0]].XValueIndexed))
							{
								CustomLabels.Add(num8, toPosition, ValueConverter.FormatValue(base.Common.Chart, this, num7, LabelStyle.Format, chartValueTypes, ChartElementType.AxisLabels), customLabel: false);
							}
						}
						else
						{
							CustomLabels.Add(num8, toPosition, pointLabel, customLabel: false);
						}
						num3 += intervalSize;
					}
					return;
				}
				if (num2 != viewMinimum)
				{
					num = 1;
				}
				int num9 = 0;
				for (double num10 = num2 - (double)num * labelStyle.Interval; num10 < viewMaximum - 1.5 * labelStyle.Interval * (double)(1 - num); num10 = (double)((decimal)num10 + (decimal)labelStyle.Interval))
				{
					num9++;
					if (num9 > 10000)
					{
						break;
					}
					double num7 = Axis.RemoveNoiseFromDoubleMath(num10) + Axis.RemoveNoiseFromDoubleMath(labelStyle.Interval);
					double value = Math.Log(labelStyle.Interval);
					double value2 = Math.Log(Math.Abs(num7));
					int num11 = (int)Math.Abs(value) + 5;
					if (num11 > 15)
					{
						num11 = 15;
					}
					if (Math.Abs(value) < Math.Abs(value2) - 5.0)
					{
						num7 = Math.Round(num7, num11);
					}
					if ((viewMaximum - num2) / labelStyle.Interval > 10000.0)
					{
						break;
					}
					if (base.Logarithmic)
					{
						num7 = Math.Pow(logarithmBase, num7);
					}
					double num8 = (double)((decimal)num10 + (decimal)labelStyle.Interval * 0.5m);
					double toPosition = (double)((decimal)num10 + (decimal)labelStyle.Interval * 1.5m);
					if (!((decimal)num8 > (decimal)viewMaximum) && !((decimal)((num8 + toPosition) / 2.0) > (decimal)viewMaximum))
					{
						string pointLabel2 = GetPointLabel(arrayList, num7, !flag2, indexedSeries);
						if (pointLabel2.Length > 15 && num7 < 1E-06)
						{
							num7 = 0.0;
						}
						if (pointLabel2.Length == 0)
						{
							if (!base.Common.DataManager.Series[arrayList[0]].XValueIndexed || !(num10 > maximum))
							{
								CustomLabels.Add(num8, toPosition, ValueConverter.FormatValue(base.Common.Chart, this, num7, LabelStyle.Format, chartValueTypes, ChartElementType.AxisLabels), customLabel: false);
							}
						}
						else
						{
							CustomLabels.Add(num8, toPosition, pointLabel2, customLabel: false);
						}
					}
				}
			}
		}

		private string GetPointLabel(ArrayList series, double valuePosition, bool nonZeroXValues, bool indexedSeries)
		{
			int num = 0;
			foreach (string item in series)
			{
				Series series2 = base.Common.DataManager.Series[item];
				num = Math.Max(num, series2.Points.Count);
			}
			bool flag = true;
			foreach (string item2 in series)
			{
				Series series3 = base.Common.DataManager.Series[item2];
				if ((axisType == AxisName.X || axisType == AxisName.X2) && (margin != 0.0 || num == 1) && !series3.XValueIndexed && series3.Points[0].AxisLabel.Length > 0 && series3.Points[series3.Points.Count - 1].AxisLabel.Length > 0)
				{
					flag = false;
				}
				if (!series3.noLabelsInPoints || (nonZeroXValues && indexedSeries))
				{
					string pointLabel = GetPointLabel(series3, valuePosition, nonZeroXValues, indexedSeries);
					if (pointLabel != "")
					{
						return pointLabel;
					}
				}
				string text = series3["__IndexedSeriesLabelsSource__"];
				if (string.IsNullOrEmpty(text))
				{
					continue;
				}
				Series series4 = base.Common.DataManager.Series[text];
				if (series4 != null)
				{
					string pointLabel2 = GetPointLabel(series4, valuePosition, nonZeroXValues, indexedSeries: true);
					if (pointLabel2 != "")
					{
						return pointLabel2;
					}
				}
			}
			if (!flag)
			{
				return " ";
			}
			return "";
		}

		private string GetPointLabel(Series series, double valuePosition, bool nonZeroXValues, bool indexedSeries)
		{
			int num = 1;
			if (axisType == AxisName.Y || axisType == AxisName.Y2)
			{
				return "";
			}
			if ((axisType != 0 || series.XAxisType != 0) && (axisType != AxisName.X2 || series.XAxisType != AxisType.Secondary))
			{
				return "";
			}
			foreach (DataPoint point in series.Points)
			{
				if (indexedSeries)
				{
					if (valuePosition == (double)num)
					{
						if (point.AxisLabel.Length == 0 && nonZeroXValues)
						{
							return ValueConverter.FormatValue(base.Common.Chart, this, point.XValue, LabelStyle.Format, series.XValueType, ChartElementType.AxisLabels);
						}
						string text = point.ReplaceKeywords(point.AxisLabel);
						if (text.Length > 0 && series.chart != null && series.chart.LocalizeTextHandler != null)
						{
							text = series.chart.LocalizeTextHandler(point, text, point.ElementId, ChartElementType.DataPoint);
						}
						return text;
					}
				}
				else if (point.XValue == valuePosition)
				{
					string text2 = point.ReplaceKeywords(point.AxisLabel);
					if (text2.Length > 0 && series.chart != null && series.chart.LocalizeTextHandler != null)
					{
						text2 = series.chart.LocalizeTextHandler(point, text2, point.ElementId, ChartElementType.DataPoint);
					}
					return text2;
				}
				num++;
			}
			return "";
		}
	}
}
