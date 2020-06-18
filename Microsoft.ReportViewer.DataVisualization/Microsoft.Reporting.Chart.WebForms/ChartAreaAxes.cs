using Microsoft.Reporting.Chart.WebForms.ChartTypes;
using System;
using System.Collections;

namespace Microsoft.Reporting.Chart.WebForms
{
	internal class ChartAreaAxes : ChartElement
	{
		internal Axis axisY;

		internal Axis axisX;

		internal Axis axisX2;

		internal Axis axisY2;

		internal ArrayList series = new ArrayList();

		internal ArrayList chartTypes = new ArrayList();

		internal string name = "";

		private string intervalSeriesList = "";

		internal double intervalData = double.NaN;

		internal double intervalLogData = double.NaN;

		internal Series intervalSeries;

		internal bool intervalSameSize;

		internal bool diffIntervalAlignmentChecked;

		internal bool stacked;

		internal bool secondYScale;

		internal bool switchValueAxes;

		internal bool requireAxes = true;

		internal bool chartAreaIsCurcular;

		internal bool hundredPercent;

		internal bool hundredPercentNegative;

		internal bool IsSubAxesSupported
		{
			get
			{
				if (((ChartArea)this).Area3DStyle.Enable3D || ((ChartArea)this).chartAreaIsCurcular)
				{
					return false;
				}
				return true;
			}
		}

		internal ArrayList Series => series;

		internal ArrayList ChartTypes => chartTypes;

		internal Axis GetAxis(AxisName axisName, AxisType axisType, string subAxisName)
		{
			if (((ChartArea)this).Area3DStyle.Enable3D)
			{
				subAxisName = string.Empty;
			}
			if (axisName == AxisName.X || axisName == AxisName.X2)
			{
				if (axisType == AxisType.Primary)
				{
					return ((ChartArea)this).AxisX.GetSubAxis(subAxisName);
				}
				return ((ChartArea)this).AxisX2.GetSubAxis(subAxisName);
			}
			if (axisType == AxisType.Primary)
			{
				return ((ChartArea)this).AxisY.GetSubAxis(subAxisName);
			}
			return ((ChartArea)this).AxisY2.GetSubAxis(subAxisName);
		}

		internal void SetDefaultAxesValues()
		{
			if (switchValueAxes)
			{
				axisY.AxisPosition = AxisPosition.Bottom;
				axisX.AxisPosition = AxisPosition.Left;
				axisX2.AxisPosition = AxisPosition.Right;
				axisY2.AxisPosition = AxisPosition.Top;
			}
			else
			{
				axisY.AxisPosition = AxisPosition.Left;
				axisX.AxisPosition = AxisPosition.Bottom;
				axisX2.AxisPosition = AxisPosition.Top;
				axisY2.AxisPosition = AxisPosition.Right;
			}
			Axis[] axes = ((ChartArea)this).Axes;
			for (int i = 0; i < axes.Length; i++)
			{
				axes[i].oppositeAxis = null;
			}
			if (chartAreaIsCurcular)
			{
				axisX.SetAutoMaximum(360.0);
				axisX.SetAutoMinimum(0.0);
				axisX.SetInterval = Math.Abs(axisX.maximum - axisX.minimum) / 12.0;
			}
			else
			{
				SetDefaultFromIndexesOrData(axisX, AxisType.Primary);
			}
			SetDefaultFromIndexesOrData(axisX2, AxisType.Secondary);
			if (GetYAxesSeries(AxisType.Primary, string.Empty).Count != 0)
			{
				SetDefaultFromData(axisY);
				axisY.EstimateAxis();
			}
			if (GetYAxesSeries(AxisType.Secondary, string.Empty).Count != 0)
			{
				SetDefaultFromData(axisY2);
				axisY2.EstimateAxis();
			}
			axisX.SetAxisPosition();
			axisX2.SetAxisPosition();
			axisY.SetAxisPosition();
			axisY2.SetAxisPosition();
			EnableAxes();
			axes = new Axis[2]
			{
				axisY,
				axisY2
			};
			foreach (Axis axis in axes)
			{
				axis.ScaleBreakStyle.GetAxisSegmentForScaleBreaks(axis.ScaleSegments);
				if (axis.ScaleSegments.Count > 0)
				{
					axis.scaleSegmentsUsed = true;
					if (axis.minimum < axis.ScaleSegments[0].ScaleMinimum)
					{
						axis.minimum = axis.ScaleSegments[0].ScaleMinimum;
					}
					if (axis.minimum > axis.ScaleSegments[axis.ScaleSegments.Count - 1].ScaleMaximum)
					{
						axis.minimum = axis.ScaleSegments[axis.ScaleSegments.Count - 1].ScaleMaximum;
					}
				}
			}
			Axis[] array = new Axis[4]
			{
				axisX,
				axisX2,
				axisY,
				axisY2
			};
			axes = array;
			foreach (Axis axis2 in axes)
			{
				if (axis2.ScaleSegments.Count <= 0)
				{
					axis2.FillLabels(removeFirstRow: true);
					continue;
				}
				bool removeFirstRow = true;
				int num = 0;
				foreach (AxisScaleSegment scaleSegment in axis2.ScaleSegments)
				{
					scaleSegment.SetTempAxisScaleAndInterval();
					axis2.FillLabels(removeFirstRow);
					removeFirstRow = false;
					scaleSegment.RestoreAxisScaleAndInterval();
					if (num < axis2.ScaleSegments.Count - 1 && axis2.CustomLabels.Count > 0)
					{
						axis2.CustomLabels.RemoveAt(axis2.CustomLabels.Count - 1);
					}
					num++;
				}
			}
			axes = array;
			for (int i = 0; i < axes.Length; i++)
			{
				axes[i].PostFillLabels();
			}
		}

		private void SetDefaultFromIndexesOrData(Axis axis, AxisType axisType)
		{
			ArrayList xAxesSeries = GetXAxesSeries(axisType, axis.SubAxisName);
			bool flag = true;
			foreach (string item in xAxesSeries)
			{
				if (!ChartElement.IndexedSeries(base.Common.DataManager.Series[item]))
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				if (axis.Logarithmic)
				{
					throw new InvalidOperationException(SR.ExceptionChartAreaAxisScaleLogarithmicUnsuitable);
				}
				SetDefaultFromIndexes(axis);
			}
			else
			{
				SetDefaultFromData(axis);
				axis.EstimateAxis();
			}
		}

		private void EnableAxes()
		{
			if (series == null)
			{
				return;
			}
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			foreach (string item in series)
			{
				Series obj = base.Common.DataManager.Series[item];
				if (obj.XAxisType == AxisType.Primary)
				{
					flag = true;
					Activate(axisX, active: true);
				}
				else
				{
					flag3 = true;
					Activate(axisX2, active: true);
				}
				if (obj.YAxisType == AxisType.Primary)
				{
					flag2 = true;
					Activate(axisY, active: true);
				}
				else
				{
					flag4 = true;
					Activate(axisY2, active: true);
				}
			}
			if (!flag)
			{
				Activate(axisX, active: false);
			}
			if (!flag2)
			{
				Activate(axisY, active: false);
			}
			if (!flag3)
			{
				Activate(axisX2, active: false);
			}
			if (!flag4)
			{
				Activate(axisY2, active: false);
			}
		}

		private void Activate(Axis axis, bool active)
		{
			if (axis.autoEnabled)
			{
				axis.enabled = active;
			}
		}

		private bool AllEmptyPoints()
		{
			foreach (string item in series)
			{
				foreach (DataPoint point in base.Common.DataManager.Series[item].Points)
				{
					if (!point.EmptyX && !point.Empty)
					{
						return false;
					}
				}
			}
			return true;
		}

		private void SetDefaultFromData(Axis axis)
		{
			if (!double.IsNaN(axis.View.Position) && !double.IsNaN(axis.View.Size) && !axis.refreshMinMaxFromData && axis.Logarithmic)
			{
				return;
			}
			GetValuesFromData(axis, out double autoMinimum, out double autoMaximum);
			if ((axis.enabled && (axis.autoMaximum || double.IsNaN(axis.Maximum)) && (autoMaximum == double.MaxValue || autoMaximum == double.MinValue)) || ((axis.autoMinimum || double.IsNaN(axis.Minimum)) && (autoMinimum == double.MaxValue || autoMinimum == double.MinValue)))
			{
				if (AllEmptyPoints())
				{
					autoMaximum = 8.0;
					autoMinimum = 1.0;
				}
				else if (!base.Common.ChartPicture.SuppressExceptions)
				{
					throw new InvalidOperationException(SR.ExceptionAxisMinimumMaximumInvalid);
				}
			}
			axis.marginView = 0.0;
			if (axis.margin == 100.0 && (axis.axisType == AxisName.X || axis.axisType == AxisName.X2))
			{
				axis.marginView = GetPointsInterval(logarithmic: false, 10.0);
			}
			if (autoMaximum == autoMinimum && axis.Maximum == axis.Minimum)
			{
				axis.marginView = 1.0;
			}
			if (axis.Logarithmic)
			{
				axis.marginView = 0.0;
			}
			if (axis.autoMaximum)
			{
				if (!axis.roundedXValues && (axis.axisType == AxisName.X || axis.axisType == AxisName.X2))
				{
					axis.SetAutoMaximum(autoMaximum + axis.marginView);
				}
				else if (axis.startFromZero && autoMaximum < 0.0)
				{
					axis.SetAutoMaximum(0.0);
				}
				else
				{
					axis.SetAutoMaximum(autoMaximum);
				}
			}
			if (axis.autoMinimum)
			{
				if (axis.Logarithmic)
				{
					if (autoMinimum < 1.0)
					{
						axis.SetAutoMinimum(autoMinimum);
					}
					else if (axis.startFromZero)
					{
						axis.SetAutoMinimum(1.0);
					}
					else
					{
						axis.SetAutoMinimum(autoMinimum);
					}
				}
				else if (autoMinimum > 0.0)
				{
					if (!axis.roundedXValues && (axis.axisType == AxisName.X || axis.axisType == AxisName.X2))
					{
						axis.SetAutoMinimum(autoMinimum - axis.marginView);
					}
					else if (axis.startFromZero && !SeriesDateTimeType(axis.axisType, axis.SubAxisName))
					{
						axis.SetAutoMinimum(0.0);
					}
					else
					{
						axis.SetAutoMinimum(autoMinimum);
					}
				}
				else if (axis.axisType == AxisName.X || axis.axisType == AxisName.X2)
				{
					axis.SetAutoMinimum(autoMinimum - axis.marginView);
				}
				else
				{
					axis.SetAutoMinimum(autoMinimum);
				}
			}
			if (axis.Logarithmic && axis.logarithmicConvertedToLinear)
			{
				if (!axis.autoMinimum)
				{
					axis.minimum = axis.logarithmicMinimum;
				}
				if (!axis.autoMaximum)
				{
					axis.maximum = axis.logarithmicMaximum;
				}
				axis.logarithmicConvertedToLinear = false;
			}
			if (base.Common.ChartPicture.SuppressExceptions && axis.maximum == axis.minimum)
			{
				axis.minimum = axis.maximum;
				axis.maximum = axis.minimum + 1.0;
			}
		}

		internal bool SeriesIntegerType(AxisName axisName, string subAxisName)
		{
			foreach (string item in this.series)
			{
				Series series = base.Common.DataManager.Series[item];
				switch (axisName)
				{
				case AxisName.X:
					if (series.XAxisType == AxisType.Primary)
					{
						if (series.XValueType != ChartValueTypes.Int && series.XValueType != ChartValueTypes.UInt && series.XValueType != ChartValueTypes.ULong && series.XValueType != ChartValueTypes.Long)
						{
							return false;
						}
						return true;
					}
					break;
				case AxisName.X2:
					if (series.XAxisType == AxisType.Secondary)
					{
						if (series.XValueType != ChartValueTypes.Int && series.XValueType != ChartValueTypes.UInt && series.XValueType != ChartValueTypes.ULong && series.XValueType != ChartValueTypes.Long)
						{
							return false;
						}
						return true;
					}
					break;
				case AxisName.Y:
					if (series.YAxisType == AxisType.Primary)
					{
						if (series.YValueType != ChartValueTypes.Int && series.YValueType != ChartValueTypes.UInt && series.YValueType != ChartValueTypes.ULong && series.YValueType != ChartValueTypes.Long)
						{
							return false;
						}
						return true;
					}
					break;
				case AxisName.Y2:
					if (series.YAxisType == AxisType.Secondary)
					{
						if (series.YValueType != ChartValueTypes.Int && series.YValueType != ChartValueTypes.UInt && series.YValueType != ChartValueTypes.ULong && series.YValueType != ChartValueTypes.Long)
						{
							return false;
						}
						return true;
					}
					break;
				}
			}
			return false;
		}

		internal bool SeriesDateTimeType(AxisName axisName, string subAxisName)
		{
			foreach (string item in this.series)
			{
				Series series = base.Common.DataManager.Series[item];
				switch (axisName)
				{
				case AxisName.X:
					if (series.XAxisType == AxisType.Primary)
					{
						if (series.XValueType != ChartValueTypes.Date && series.XValueType != ChartValueTypes.DateTime && series.XValueType != ChartValueTypes.Time && series.XValueType != ChartValueTypes.DateTimeOffset)
						{
							return false;
						}
						return true;
					}
					break;
				case AxisName.X2:
					if (series.XAxisType == AxisType.Secondary)
					{
						if (series.XValueType != ChartValueTypes.Date && series.XValueType != ChartValueTypes.DateTime && series.XValueType != ChartValueTypes.Time && series.XValueType != ChartValueTypes.DateTimeOffset)
						{
							return false;
						}
						return true;
					}
					break;
				case AxisName.Y:
					if (series.YAxisType == AxisType.Primary)
					{
						if (series.YValueType != ChartValueTypes.Date && series.YValueType != ChartValueTypes.DateTime && series.YValueType != ChartValueTypes.Time && series.YValueType != ChartValueTypes.DateTimeOffset)
						{
							return false;
						}
						return true;
					}
					break;
				case AxisName.Y2:
					if (series.YAxisType == AxisType.Secondary)
					{
						if (series.YValueType != ChartValueTypes.Date && series.YValueType != ChartValueTypes.DateTime && series.YValueType != ChartValueTypes.Time && series.YValueType != ChartValueTypes.DateTimeOffset)
						{
							return false;
						}
						return true;
					}
					break;
				}
			}
			return false;
		}

		private void GetValuesFromData(Axis axis, out double autoMinimum, out double autoMaximum)
		{
			int numberOfAllPoints = GetNumberOfAllPoints();
			if (!axis.refreshMinMaxFromData && !double.IsNaN(axis.minimumFromData) && !double.IsNaN(axis.maximumFromData) && axis.numberOfPointsInAllSeries == numberOfAllPoints)
			{
				autoMinimum = axis.minimumFromData;
				autoMaximum = axis.maximumFromData;
				return;
			}
			AxisType type = AxisType.Primary;
			if (axis.axisType == AxisName.X2 || axis.axisType == AxisName.Y2)
			{
				type = AxisType.Secondary;
			}
			string[] array = (string[])GetXAxesSeries(type, axis.SubAxisName).ToArray(typeof(string));
			string[] seriesNames = (string[])GetYAxesSeries(type, axis.SubAxisName).ToArray(typeof(string));
			if (axis.axisType == AxisName.X2 || axis.axisType == AxisName.X)
			{
				if (stacked)
				{
					try
					{
						base.Common.DataManager.GetMinMaxXValue(out autoMinimum, out autoMaximum, array);
					}
					catch (Exception)
					{
						throw new InvalidOperationException(SR.ExceptionAxisStackedChartsDataPointsNumberMismatch);
					}
				}
				else if (secondYScale)
				{
					autoMaximum = base.Common.DataManager.GetMaxXWithRadiusValue((ChartArea)this, array);
					autoMinimum = base.Common.DataManager.GetMinXWithRadiusValue((ChartArea)this, array);
					ChartValueTypes xValueType = base.Common.DataManager.Series[array[0]].XValueType;
					if (xValueType != ChartValueTypes.Date && xValueType != ChartValueTypes.DateTime && xValueType != ChartValueTypes.Time && xValueType != ChartValueTypes.DateTimeOffset)
					{
						axis.roundedXValues = true;
					}
				}
				else
				{
					base.Common.DataManager.GetMinMaxXValue(out autoMinimum, out autoMaximum, array);
				}
			}
			else if (stacked)
			{
				try
				{
					if (hundredPercent)
					{
						autoMaximum = base.Common.DataManager.GetMaxHundredPercentStackedYValue(hundredPercentNegative, 0, seriesNames);
						autoMinimum = base.Common.DataManager.GetMinHundredPercentStackedYValue(hundredPercentNegative, 0, seriesNames);
					}
					else
					{
						double val = double.MinValue;
						double val2 = double.MaxValue;
						double num = double.MinValue;
						double num2 = double.MaxValue;
						foreach (string[] item in SplitSeriesInStackedGroups(seriesNames))
						{
							double maxStackedYValue = base.Common.DataManager.GetMaxStackedYValue(0, item);
							double minStackedYValue = base.Common.DataManager.GetMinStackedYValue(0, item);
							double maxUnsignedStackedYValue = base.Common.DataManager.GetMaxUnsignedStackedYValue(0, item);
							double minUnsignedStackedYValue = base.Common.DataManager.GetMinUnsignedStackedYValue(0, item);
							val = Math.Max(val, maxStackedYValue);
							val2 = Math.Min(val2, minStackedYValue);
							num = Math.Max(num, maxUnsignedStackedYValue);
							num2 = Math.Min(num2, minUnsignedStackedYValue);
						}
						autoMaximum = Math.Max(val, num);
						autoMinimum = Math.Min(val2, num2);
					}
					if (axis.Logarithmic && autoMinimum < 1.0)
					{
						autoMinimum = 1.0;
					}
				}
				catch (Exception)
				{
					throw new InvalidOperationException(SR.ExceptionAxisStackedChartsDataPointsNumberMismatch);
				}
			}
			else if (secondYScale)
			{
				autoMaximum = base.Common.DataManager.GetMaxYWithRadiusValue((ChartArea)this, seriesNames);
				autoMinimum = base.Common.DataManager.GetMinYWithRadiusValue((ChartArea)this, seriesNames);
			}
			else
			{
				bool flag = false;
				if (base.Common != null && base.Common.Chart != null)
				{
					foreach (Series item2 in base.Common.Chart.Series)
					{
						if (item2.ChartArea == ((ChartArea)this).Name)
						{
							IChartType chartType = base.Common.ChartTypeRegistry.GetChartType(item2.ChartTypeName);
							if (chartType != null && chartType.ExtraYValuesConnectedToYAxis)
							{
								flag = true;
								break;
							}
						}
					}
				}
				if (flag)
				{
					base.Common.DataManager.GetMinMaxYValue(out autoMinimum, out autoMaximum, seriesNames);
				}
				else
				{
					base.Common.DataManager.GetMinMaxYValue(0, out autoMinimum, out autoMaximum, seriesNames);
				}
			}
			axis.maximumFromData = autoMaximum;
			axis.minimumFromData = autoMinimum;
			axis.refreshMinMaxFromData = false;
			axis.numberOfPointsInAllSeries = numberOfAllPoints;
		}

		private ArrayList SplitSeriesInStackedGroups(string[] seriesNames)
		{
			Hashtable hashtable = new Hashtable();
			foreach (string text in seriesNames)
			{
				Series series = base.Common.Chart.Series[text];
				string key = string.Empty;
				if (StackedColumnChart.IsSeriesStackGroupNameSupported(series))
				{
					key = StackedColumnChart.GetSeriesStackGroupName(series);
				}
				if (hashtable.ContainsKey(key))
				{
					((ArrayList)hashtable[key]).Add(text);
					continue;
				}
				ArrayList arrayList = new ArrayList();
				arrayList.Add(text);
				hashtable.Add(key, arrayList);
			}
			ArrayList arrayList2 = new ArrayList();
			foreach (DictionaryEntry item in hashtable)
			{
				ArrayList arrayList3 = (ArrayList)item.Value;
				if (arrayList3.Count <= 0)
				{
					continue;
				}
				int num = 0;
				string[] array = new string[arrayList3.Count];
				foreach (string item2 in arrayList3)
				{
					array[num++] = item2;
				}
				arrayList2.Add(array);
			}
			return arrayList2;
		}

		private int GetNumberOfAllPoints()
		{
			int num = 0;
			foreach (Series item in base.Common.DataManager.Series)
			{
				num += item.Points.Count;
			}
			return num;
		}

		private void SetDefaultFromIndexes(Axis axis)
		{
			axis.SetTempAxisOffset();
			AxisType type = AxisType.Primary;
			if (axis.axisType == AxisName.X2 || axis.axisType == AxisName.Y2)
			{
				type = AxisType.Secondary;
			}
			double num = base.Common.DataManager.GetNumberOfPoints((string[])GetXAxesSeries(type, axis.SubAxisName).ToArray(typeof(string)));
			double num2 = 0.0;
			axis.marginView = 0.0;
			if (axis.margin == 100.0)
			{
				axis.marginView = 1.0;
			}
			if (num + axis.margin / 100.0 == num2 - axis.margin / 100.0 + 1.0)
			{
				axis.SetAutoMaximum(num + 1.0);
				axis.SetAutoMinimum(num2);
			}
			else
			{
				axis.SetAutoMaximum(num + axis.margin / 100.0);
				axis.SetAutoMinimum(num2 - axis.margin / 100.0 + 1.0);
			}
			double num3 = (!(axis.GetViewMaximum() - axis.GetViewMinimum() <= 10.0)) ? axis.CalcInterval((axis.GetViewMaximum() - axis.GetViewMinimum()) / 5.0) : 1.0;
			if (((ChartArea)this).Area3DStyle.Enable3D && !double.IsNaN(axis.interval3DCorrection))
			{
				num3 = Math.Ceiling(num3 / axis.interval3DCorrection);
				axis.interval3DCorrection = double.NaN;
				if (num3 > 1.0 && num3 < 4.0 && axis.GetViewMaximum() - axis.GetViewMinimum() <= 4.0)
				{
					num3 = 1.0;
				}
			}
			axis.SetInterval = num3;
			if (axis.offsetTempSet)
			{
				axis.minorGrid.intervalOffset -= axis.MajorGrid.Interval;
				axis.minorTickMark.intervalOffset -= axis.MajorTickMark.Interval;
			}
		}

		internal void SetData()
		{
			SetData(initializeAxes: true);
		}

		internal void SetData(bool initializeAxes)
		{
			stacked = false;
			switchValueAxes = false;
			requireAxes = true;
			hundredPercent = false;
			hundredPercentNegative = false;
			chartAreaIsCurcular = false;
			secondYScale = false;
			bool flag = false;
			this.series.Clear();
			ChartAreaCollection chartAreas = base.Common.Chart.ChartAreas;
			bool flag2 = chartAreas.Count > 0 && chartAreas[0] == this && chartAreas.GetIndex("Default") == -1;
			foreach (Series item in base.Common.DataManager.Series)
			{
				if (item.IsVisible() && (name == item.ChartArea || (flag2 && string.Compare(item.ChartArea, "Default", StringComparison.Ordinal) == 0)) && base.Common.DataManager.GetNumberOfPoints(item.Name) != 0)
				{
					series.Add(item.Name);
				}
			}
			chartTypes.Clear();
			foreach (Series item2 in base.Common.DataManager.Series)
			{
				IChartType chartType = base.Common.ChartTypeRegistry.GetChartType(item2.ChartTypeName);
				bool flag3 = false;
				if ((!item2.IsVisible() && !chartType.RequireAxes) || (!(name == item2.ChartArea) && (!flag2 || string.Compare(item2.ChartArea, "Default", StringComparison.Ordinal) != 0)))
				{
					continue;
				}
				foreach (string chartType2 in chartTypes)
				{
					if (chartType2 == item2.ChartTypeName)
					{
						flag3 = true;
					}
				}
				if (flag3)
				{
					continue;
				}
				if (chartType.Stacked)
				{
					stacked = true;
				}
				if (!flag)
				{
					if (chartType.SwitchValueAxes)
					{
						switchValueAxes = true;
					}
					if (!chartType.RequireAxes)
					{
						requireAxes = false;
					}
					if (chartType.CircularChartArea)
					{
						chartAreaIsCurcular = true;
					}
					if (chartType.HundredPercent)
					{
						hundredPercent = true;
					}
					if (chartType.HundredPercentSupportNegative)
					{
						hundredPercentNegative = true;
					}
					if (chartType.SecondYScale)
					{
						secondYScale = true;
					}
					flag = true;
				}
				else if (chartType.SwitchValueAxes != switchValueAxes)
				{
					throw new InvalidOperationException(SR.ExceptionChartAreaChartTypesCanNotCombine);
				}
				if (base.Common.DataManager.GetNumberOfPoints(item2.Name) != 0)
				{
					chartTypes.Add(item2.ChartTypeName);
				}
			}
			for (int i = 0; i <= 1; i++)
			{
				ArrayList xAxesSeries = GetXAxesSeries((i != 0) ? AxisType.Secondary : AxisType.Primary, string.Empty);
				if (xAxesSeries.Count <= 0)
				{
					continue;
				}
				bool flag4 = false;
				string text = "";
				foreach (string item3 in xAxesSeries)
				{
					text = text + item3.Replace(",", "\\,") + ",";
					if (base.Common.DataManager.Series[item3].XValueIndexed)
					{
						flag4 = true;
					}
				}
				if (flag4)
				{
					try
					{
						base.Common.DataManipulator.CheckXValuesAlignment(base.Common.DataManipulator.ConvertToSeriesArray(text.TrimEnd(','), createNew: false));
					}
					catch (Exception ex)
					{
						throw new ArgumentException(SR.ExceptionAxisSeriesNotAligned + ex.Message);
					}
				}
			}
			if (initializeAxes)
			{
				SetDefaultAxesValues();
			}
		}

		internal ArrayList GetSeriesFromChartType(string chartType)
		{
			ArrayList arrayList = new ArrayList();
			foreach (string item in series)
			{
				if (string.Compare(chartType, base.Common.DataManager.Series[item].ChartTypeName, StringComparison.OrdinalIgnoreCase) == 0)
				{
					arrayList.Add(base.Common.DataManager.Series[item].Name);
				}
			}
			return arrayList;
		}

		internal ArrayList GetSeries()
		{
			ArrayList arrayList = new ArrayList();
			foreach (string item in series)
			{
				arrayList.Add(base.Common.DataManager.Series[item]);
			}
			return arrayList;
		}

		internal ArrayList GetXAxesSeries(AxisType type, string subAxisName)
		{
			ArrayList arrayList = new ArrayList();
			if (series.Count == 0)
			{
				return arrayList;
			}
			if (!IsSubAxesSupported && subAxisName.Length > 0)
			{
				return arrayList;
			}
			foreach (string item in series)
			{
				if (base.Common.DataManager.Series[item].XAxisType == type)
				{
					arrayList.Add(item);
				}
			}
			if (arrayList.Count == 0)
			{
				if (type == AxisType.Secondary)
				{
					return GetXAxesSeries(AxisType.Primary, string.Empty);
				}
				return GetXAxesSeries(AxisType.Secondary, string.Empty);
			}
			return arrayList;
		}

		internal ArrayList GetYAxesSeries(AxisType type, string subAxisName)
		{
			ArrayList arrayList = new ArrayList();
			foreach (string item in series)
			{
				AxisType axisType = base.Common.DataManager.Series[item].YAxisType;
				if (base.Common.DataManager.Series[item].ChartType == SeriesChartType.Radar || base.Common.DataManager.Series[item].ChartType == SeriesChartType.Polar)
				{
					axisType = AxisType.Primary;
					_ = string.Empty;
				}
				if (axisType == type)
				{
					arrayList.Add(item);
				}
			}
			if (arrayList.Count == 0 && type == AxisType.Secondary)
			{
				return GetYAxesSeries(AxisType.Primary, string.Empty);
			}
			return arrayList;
		}

		internal Series GetFirstSeries()
		{
			if (series.Count == 0)
			{
				throw new InvalidOperationException(SR.ExceptionChartAreaSeriesNotFound);
			}
			return base.Common.DataManager.Series[series[0]];
		}

		internal double GetPointsInterval(bool logarithmic, double logarithmBase)
		{
			bool sameInterval;
			return GetPointsInterval(series, logarithmic, logarithmBase, checkSameInterval: false, out sameInterval);
		}

		internal double GetPointsInterval(ArrayList seriesList, bool logarithmic, double logarithmBase, bool checkSameInterval, out bool sameInterval)
		{
			Series series = null;
			return GetPointsInterval(seriesList, logarithmic, logarithmBase, checkSameInterval, out sameInterval, out series);
		}

		internal double GetPointsInterval(ArrayList seriesList, bool logarithmic, double logarithmicBase, bool checkSameInterval, out bool sameInterval, out Series series)
		{
			long ticksInterval = long.MaxValue;
			int monthsInteval = 0;
			double num = double.MinValue;
			double num2 = double.MaxValue;
			sameInterval = true;
			series = null;
			string text = "";
			if (seriesList != null)
			{
				foreach (string series4 in seriesList)
				{
					text = text + series4 + ",";
				}
			}
			if (!checkSameInterval || diffIntervalAlignmentChecked)
			{
				if (!logarithmic)
				{
					if (!double.IsNaN(intervalData) && intervalSeriesList == text)
					{
						sameInterval = intervalSameSize;
						series = intervalSeries;
						return intervalData;
					}
				}
				else if (!double.IsNaN(intervalLogData) && intervalSeriesList == text)
				{
					sameInterval = intervalSameSize;
					series = intervalSeries;
					return intervalLogData;
				}
			}
			int num3 = 0;
			Series series2 = null;
			ArrayList[] array = new ArrayList[seriesList.Count];
			foreach (string series5 in seriesList)
			{
				Series series3 = base.Common.DataManager.Series[series5];
				bool flag = series3.IsXValueDateTime();
				array[num3] = new ArrayList();
				bool flag2 = false;
				double num4 = double.MinValue;
				double num5 = 0.0;
				if (series3.Points.Count > 0)
				{
					num4 = ((!logarithmic) ? series3.Points[0].XValue : Math.Log(series3.Points[0].XValue, logarithmicBase));
				}
				foreach (DataPoint point in series3.Points)
				{
					num5 = ((!logarithmic) ? point.XValue : Math.Log(point.XValue, logarithmicBase));
					if (num4 > num5)
					{
						flag2 = true;
					}
					array[num3].Add(num5);
					num4 = num5;
				}
				if (flag2)
				{
					array[num3].Sort();
				}
				for (int i = 1; i < array[num3].Count; i++)
				{
					double num6 = Math.Abs((double)array[num3][i - 1] - (double)array[num3][i]);
					if (sameInterval)
					{
						if (flag)
						{
							if (ticksInterval == long.MaxValue)
							{
								GetDateInterval((double)array[num3][i - 1], (double)array[num3][i], out monthsInteval, out ticksInterval);
							}
							else
							{
								long ticksInterval2 = long.MaxValue;
								int monthsInteval2 = 0;
								GetDateInterval((double)array[num3][i - 1], (double)array[num3][i], out monthsInteval2, out ticksInterval2);
								if (monthsInteval2 != monthsInteval || ticksInterval2 != ticksInterval)
								{
									sameInterval = false;
								}
							}
						}
						else if (num != num6 && num != double.MinValue)
						{
							sameInterval = false;
						}
					}
					num = num6;
					if (num2 > num6 && num6 != 0.0)
					{
						num2 = num6;
						series2 = series3;
					}
				}
				num3++;
			}
			diffIntervalAlignmentChecked = false;
			if (checkSameInterval && !sameInterval && array.Length > 1)
			{
				bool flag3 = false;
				diffIntervalAlignmentChecked = true;
				int num7 = 0;
				ArrayList[] array2 = array;
				foreach (ArrayList arrayList in array2)
				{
					for (int k = 0; k < arrayList.Count; k++)
					{
						if (flag3)
						{
							break;
						}
						double num8 = (double)arrayList[k];
						for (int l = num7 + 1; l < array.Length; l++)
						{
							if (flag3)
							{
								break;
							}
							if ((k < array[l].Count && (double)array[l][k] == num8) || array[l].Contains(num8))
							{
								flag3 = true;
								break;
							}
						}
					}
					num7++;
				}
				if (flag3)
				{
					sameInterval = true;
				}
			}
			if (num2 == double.MaxValue)
			{
				num2 = 1.0;
			}
			intervalSameSize = sameInterval;
			if (!logarithmic)
			{
				intervalData = num2;
				intervalSeries = series2;
				series = intervalSeries;
				intervalSeriesList = text;
				return intervalData;
			}
			intervalLogData = num2;
			intervalSeries = series2;
			series = intervalSeries;
			intervalSeriesList = text;
			return intervalLogData;
		}

		private void GetDateInterval(double value1, double value2, out int monthsInteval, out long ticksInterval)
		{
			DateTime dateTime = DateTime.FromOADate(value1);
			DateTime dateTime2 = DateTime.FromOADate(value2);
			monthsInteval = dateTime2.Month - dateTime.Month;
			monthsInteval += (dateTime2.Year - dateTime.Year) * 12;
			ticksInterval = 0L;
			ticksInterval += (dateTime2.Day - dateTime.Day) * 864000000000L;
			ticksInterval += (dateTime2.Hour - dateTime.Hour) * 36000000000L;
			ticksInterval += (long)(dateTime2.Minute - dateTime.Minute) * 600000000L;
			ticksInterval += (long)(dateTime2.Second - dateTime.Second) * 10000000L;
			ticksInterval += (long)(dateTime2.Millisecond - dateTime.Millisecond) * 10000L;
		}
	}
}
