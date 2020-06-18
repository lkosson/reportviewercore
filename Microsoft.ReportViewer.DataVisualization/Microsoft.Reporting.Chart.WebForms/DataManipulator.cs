using System;
using System.Collections;
using System.Data;
using System.Globalization;

namespace Microsoft.Reporting.Chart.WebForms
{
	internal class DataManipulator : FormulaData
	{
		private class PointElementFilter : IDataPointFilter
		{
			private DataManipulator dataManipulator;

			private DateRangeType dateRange;

			private int[] rangeElements;

			private PointElementFilter()
			{
			}

			public PointElementFilter(DataManipulator dataManipulator, DateRangeType dateRange, string rangeElements)
			{
				this.dataManipulator = dataManipulator;
				this.dateRange = dateRange;
				this.rangeElements = dataManipulator.ConvertElementIndexesToArray(rangeElements);
			}

			public bool FilterDataPoint(DataPoint point, Series series, int pointIndex)
			{
				return dataManipulator.CheckFilterElementCriteria(dateRange, rangeElements, point, series, pointIndex);
			}
		}

		private class PointValueFilter : IDataPointFilter
		{
			private DataManipulator dataManipulator;

			private CompareMethod compareMethod;

			private string usingValue;

			private double compareValue;

			private PointValueFilter()
			{
			}

			public PointValueFilter(DataManipulator dataManipulator, CompareMethod compareMethod, double compareValue, string usingValue)
			{
				this.dataManipulator = dataManipulator;
				this.compareMethod = compareMethod;
				this.usingValue = usingValue;
				this.compareValue = compareValue;
			}

			public bool FilterDataPoint(DataPoint point, Series series, int pointIndex)
			{
				bool result = false;
				switch (compareMethod)
				{
				case CompareMethod.Equal:
					result = (point.GetValueByName(usingValue) == compareValue);
					break;
				case CompareMethod.Less:
					result = (point.GetValueByName(usingValue) < compareValue);
					break;
				case CompareMethod.LessOrEqual:
					result = (point.GetValueByName(usingValue) <= compareValue);
					break;
				case CompareMethod.More:
					result = (point.GetValueByName(usingValue) > compareValue);
					break;
				case CompareMethod.MoreOrEqual:
					result = (point.GetValueByName(usingValue) >= compareValue);
					break;
				case CompareMethod.NotEqual:
					result = (point.GetValueByName(usingValue) != compareValue);
					break;
				}
				return result;
			}
		}

		private class GroupingFunctionInfo
		{
			internal GroupingFunction function;

			internal int outputIndex;

			internal GroupingFunctionInfo()
			{
			}
		}

		private bool filterSetEmptyPoints;

		private bool filterMatchedPoints = true;

		public bool FilterSetEmptyPoints
		{
			get
			{
				return filterSetEmptyPoints;
			}
			set
			{
				filterSetEmptyPoints = value;
			}
		}

		public bool FilterMatchedPoints
		{
			get
			{
				return filterMatchedPoints;
			}
			set
			{
				filterMatchedPoints = value;
			}
		}

		internal Series[] ConvertToSeriesArray(object obj, bool createNew)
		{
			Series[] array = null;
			if (obj == null)
			{
				return null;
			}
			if (obj.GetType() == typeof(Series))
			{
				array = new Series[1]
				{
					(Series)obj
				};
			}
			else if (obj.GetType() == typeof(string))
			{
				string text = (string)obj;
				int num = 0;
				if (text == "*")
				{
					array = new Series[base.Common.DataManager.Series.Count];
					{
						foreach (Series item in base.Common.DataManager.Series)
						{
							Series series = array[num] = item;
							num++;
						}
						return array;
					}
				}
				if (text.Length > 0)
				{
					text = text.Replace("\\,", "\\x45");
					text = text.Replace("\\=", "\\x46");
					string[] array2 = text.Split(',');
					array = new Series[array2.Length];
					string[] array3 = array2;
					for (int i = 0; i < array3.Length; i++)
					{
						string text2 = array3[i].Replace("\\x45", ",");
						text2 = text2.Replace("\\x46", "=");
						try
						{
							array[num] = base.Common.DataManager.Series[text2.Trim()];
						}
						catch (Exception)
						{
							if (!createNew)
							{
								throw;
							}
							array[num] = base.Common.DataManager.Series.Add(text2.Trim());
						}
						num++;
					}
				}
			}
			return array;
		}

		private void Sort(PointsSortOrder order, string sortBy, Series[] series)
		{
			if (series != null && series.Length != 0)
			{
				DataPointComparer comparer = new DataPointComparer(series[0], order, sortBy);
				Sort(comparer, series);
			}
		}

		private void Sort(IComparer comparer, Series[] series)
		{
			if (series == null || series.Length == 0)
			{
				return;
			}
			if (series.Length > 1)
			{
				CheckXValuesAlignment(series);
				int num = 0;
				foreach (DataPoint point in series[0].Points)
				{
					point["_Index"] = num.ToString(CultureInfo.InvariantCulture);
					num++;
				}
			}
			series[0].Sort(comparer);
			if (series.Length <= 1)
			{
				return;
			}
			int num2 = 0;
			int num3 = 0;
			foreach (DataPoint point2 in series[0].Points)
			{
				num3 = int.Parse(point2["_Index"], CultureInfo.InvariantCulture);
				for (int i = 1; i < series.Length; i++)
				{
					series[i].Points.Insert(num2, series[i].Points[num2 + num3]);
				}
				num2++;
			}
			for (int j = 1; j < series.Length; j++)
			{
				while (series[j].Points.Count > series[0].Points.Count)
				{
					series[j].Points.RemoveAt(series[j].Points.Count - 1);
				}
			}
			foreach (DataPoint point3 in series[0].Points)
			{
				point3.DeleteAttribute("_Index");
			}
		}

		public void Sort(PointsSortOrder order, string sortBy, string seriesName)
		{
			Sort(order, sortBy, ConvertToSeriesArray(seriesName, createNew: false));
		}

		public void Sort(PointsSortOrder order, Series series)
		{
			Sort(order, "Y", ConvertToSeriesArray(series, createNew: false));
		}

		public void Sort(PointsSortOrder order, string seriesName)
		{
			Sort(order, "Y", ConvertToSeriesArray(seriesName, createNew: false));
		}

		public void Sort(PointsSortOrder order, string sortBy, Series series)
		{
			Sort(order, sortBy, ConvertToSeriesArray(series, createNew: false));
		}

		internal void Sort(IComparer comparer, Series series)
		{
			Sort(comparer, ConvertToSeriesArray(series, createNew: false));
		}

		internal void Sort(IComparer comparer, string seriesName)
		{
			Sort(comparer, ConvertToSeriesArray(seriesName, createNew: false));
		}

		private void InsertEmptyPoints(double interval, IntervalType intervalType, double intervalOffset, IntervalType intervalOffsetType, double fromXValue, double toXValue, Series[] series)
		{
			double num = Math.Min(fromXValue, toXValue);
			double num2 = Math.Max(fromXValue, toXValue);
			bool flag = double.IsNaN(num);
			bool flag2 = double.IsNaN(num2);
			Series[] array = series;
			foreach (Series series2 in array)
			{
				if (series2.Points.Count >= 1)
				{
					if (flag2)
					{
						num2 = ((!double.IsNaN(num2)) ? Math.Max(num2, series2.Points[series2.Points.Count - 1].XValue) : series2.Points[series2.Points.Count - 1].XValue);
					}
					if (flag)
					{
						num = ((!double.IsNaN(num)) ? Math.Min(num, series2.Points[0].XValue) : series2.Points[0].XValue);
					}
					if (num > num2)
					{
						double num3 = num;
						num = num2;
						num2 = num3;
					}
				}
			}
			double num4 = num;
			num = AlignIntervalStart(num, interval, ConvertIntervalType(intervalType));
			if (intervalOffset != 0.0)
			{
				num += GetIntervalSize(num, intervalOffset, ConvertIntervalType(intervalOffsetType));
			}
			array = series;
			foreach (Series series3 in array)
			{
				int num5 = 0;
				int num6 = 0;
				double num7 = num;
				while (num7 <= num2)
				{
					bool flag3 = false;
					if ((double.IsNaN(fromXValue) && num7 < num4) || (!double.IsNaN(fromXValue) && num7 < fromXValue))
					{
						flag3 = true;
					}
					else if (num7 > toXValue)
					{
						flag3 = true;
					}
					if (!flag3)
					{
						int num8 = num6;
						for (int j = num6; j < series3.Points.Count; j++)
						{
							if (series3.Points[j].XValue == num7)
							{
								num8 = -1;
								break;
							}
							if (series3.Points[j].XValue > num7)
							{
								num8 = j;
								break;
							}
							if (j == series3.Points.Count - 1)
							{
								num8 = series3.Points.Count;
							}
						}
						if (num8 != -1)
						{
							num6 = num8;
							num5++;
							DataPoint dataPoint = new DataPoint(series3);
							dataPoint.XValue = num7;
							dataPoint.Empty = true;
							series3.Points.Insert(num8, dataPoint);
						}
					}
					num7 += GetIntervalSize(num7, interval, ConvertIntervalType(intervalType));
					if (num5 > 1000)
					{
						num7 = num2 + 1.0;
					}
				}
			}
		}

		private DateTimeIntervalType ConvertIntervalType(IntervalType type)
		{
			switch (type)
			{
			case IntervalType.Milliseconds:
				return DateTimeIntervalType.Milliseconds;
			case IntervalType.Seconds:
				return DateTimeIntervalType.Seconds;
			case IntervalType.Days:
				return DateTimeIntervalType.Days;
			case IntervalType.Hours:
				return DateTimeIntervalType.Hours;
			case IntervalType.Minutes:
				return DateTimeIntervalType.Minutes;
			case IntervalType.Months:
				return DateTimeIntervalType.Months;
			case IntervalType.Number:
				return DateTimeIntervalType.Number;
			case IntervalType.Weeks:
				return DateTimeIntervalType.Weeks;
			case IntervalType.Years:
				return DateTimeIntervalType.Years;
			default:
				return DateTimeIntervalType.Auto;
			}
		}

		public void InsertEmptyPoints(double interval, IntervalType intervalType, Series series)
		{
			InsertEmptyPoints(interval, intervalType, 0.0, IntervalType.Number, series);
		}

		public void InsertEmptyPoints(double interval, IntervalType intervalType, string seriesName)
		{
			InsertEmptyPoints(interval, intervalType, 0.0, IntervalType.Number, seriesName);
		}

		public void InsertEmptyPoints(double interval, IntervalType intervalType, double intervalOffset, IntervalType intervalOffsetType, string seriesName)
		{
			InsertEmptyPoints(interval, intervalType, intervalOffset, intervalOffsetType, double.NaN, double.NaN, seriesName);
		}

		public void InsertEmptyPoints(double interval, IntervalType intervalType, double intervalOffset, IntervalType intervalOffsetType, Series series)
		{
			InsertEmptyPoints(interval, intervalType, intervalOffset, intervalOffsetType, double.NaN, double.NaN, series);
		}

		public void InsertEmptyPoints(double interval, IntervalType intervalType, double intervalOffset, IntervalType intervalOffsetType, double fromXValue, double toXValue, string seriesName)
		{
			InsertEmptyPoints(interval, intervalType, intervalOffset, intervalOffsetType, fromXValue, toXValue, ConvertToSeriesArray(seriesName, createNew: false));
		}

		public void InsertEmptyPoints(double interval, IntervalType intervalType, double intervalOffset, IntervalType intervalOffsetType, double fromXValue, double toXValue, Series series)
		{
			InsertEmptyPoints(interval, intervalType, intervalOffset, intervalOffsetType, fromXValue, toXValue, ConvertToSeriesArray(series, createNew: false));
		}

		internal DataSet ExportSeriesValues(Series[] series)
		{
			DataSet dataSet = new DataSet();
			dataSet.Locale = CultureInfo.CurrentCulture;
			if (series != null)
			{
				foreach (Series series2 in series)
				{
					bool flag = true;
					foreach (DataPoint point in series2.Points)
					{
						if (point.XValue != 0.0)
						{
							flag = false;
							break;
						}
					}
					if (flag && series2.XValueType == ChartValueTypes.String)
					{
						flag = false;
					}
					DataTable dataTable = new DataTable(series2.Name);
					dataTable.Locale = CultureInfo.CurrentCulture;
					Type typeFromHandle = typeof(double);
					if (series2.IsXValueDateTime())
					{
						typeFromHandle = typeof(DateTime);
					}
					else if (series2.XValueType == ChartValueTypes.String)
					{
						typeFromHandle = typeof(string);
					}
					dataTable.Columns.Add("X", typeFromHandle);
					typeFromHandle = typeof(double);
					if (series2.IsYValueDateTime())
					{
						typeFromHandle = typeof(DateTime);
					}
					else if (series2.YValueType == ChartValueTypes.String)
					{
						typeFromHandle = typeof(string);
					}
					for (int j = 0; j < series2.YValuesPerPoint; j++)
					{
						if (j == 0)
						{
							dataTable.Columns.Add("Y", typeFromHandle);
						}
						else
						{
							dataTable.Columns.Add("Y" + (j + 1).ToString(CultureInfo.InvariantCulture), typeFromHandle);
						}
					}
					double num = 1.0;
					foreach (DataPoint point2 in series2.Points)
					{
						if (point2.Empty && base.IgnoreEmptyPoints)
						{
							continue;
						}
						DataRow dataRow = dataTable.NewRow();
						object obj = point2.XValue;
						if (series2.IsXValueDateTime())
						{
							obj = DateTime.FromOADate(point2.XValue);
						}
						else if (series2.XValueType == ChartValueTypes.String)
						{
							obj = point2.AxisLabel;
						}
						dataRow["X"] = (flag ? ((object)num) : obj);
						for (int k = 0; k < series2.YValuesPerPoint; k++)
						{
							object value = point2.YValues[k];
							if (!point2.Empty)
							{
								if (series2.IsYValueDateTime())
								{
									value = DateTime.FromOADate(point2.YValues[k]);
								}
								else if (series2.YValueType == ChartValueTypes.String)
								{
									value = point2.AxisLabel;
								}
							}
							else if (!base.IgnoreEmptyPoints)
							{
								value = DBNull.Value;
							}
							if (k == 0)
							{
								dataRow["Y"] = value;
							}
							else
							{
								dataRow["Y" + (k + 1).ToString(CultureInfo.InvariantCulture)] = value;
							}
						}
						dataTable.Rows.Add(dataRow);
						num += 1.0;
					}
					dataTable.AcceptChanges();
					dataSet.Tables.Add(dataTable);
				}
			}
			return dataSet;
		}

		public DataSet ExportSeriesValues()
		{
			return ExportSeriesValues("*");
		}

		public DataSet ExportSeriesValues(string seriesNames)
		{
			return ExportSeriesValues(ConvertToSeriesArray(seriesNames, createNew: false));
		}

		public DataSet ExportSeriesValues(Series series)
		{
			return ExportSeriesValues(ConvertToSeriesArray(series, createNew: false));
		}

		private void FilterTopN(int pointCount, Series[] inputSeries, Series[] outputSeries, string usingValue, bool getTopValues)
		{
			CheckSeriesArrays(inputSeries, outputSeries);
			CheckXValuesAlignment(inputSeries);
			if (pointCount <= 0)
			{
				throw new ArgumentException(SR.ExceptionDataManipulatorPointCountIsZero, "pointCount");
			}
			Series[] array = new Series[inputSeries.Length];
			for (int i = 0; i < inputSeries.Length; i++)
			{
				array[i] = inputSeries[i];
				if (outputSeries != null && outputSeries.Length > i)
				{
					array[i] = outputSeries[i];
				}
				if (array[i] == inputSeries[i])
				{
					continue;
				}
				array[i].Points.Clear();
				array[i].YValuesPerPoint = inputSeries[i].YValuesPerPoint;
				if (array[i].XValueType == ChartValueTypes.Auto || array[i].autoXValueType)
				{
					array[i].XValueType = inputSeries[i].XValueType;
					array[i].autoXValueType = true;
				}
				if (array[i].YValueType == ChartValueTypes.Auto || array[i].autoYValueType)
				{
					array[i].YValueType = inputSeries[i].YValueType;
					array[i].autoYValueType = true;
				}
				foreach (DataPoint point in inputSeries[i].Points)
				{
					array[i].Points.Add(point.Clone());
				}
			}
			if (inputSeries[0].Points.Count == 0)
			{
				return;
			}
			Sort(getTopValues ? PointsSortOrder.Descending : PointsSortOrder.Ascending, usingValue, array);
			for (int j = 0; j < inputSeries.Length; j++)
			{
				while (array[j].Points.Count > pointCount)
				{
					if (FilterSetEmptyPoints)
					{
						array[j].Points[pointCount].Empty = true;
						pointCount++;
					}
					else
					{
						array[j].Points.RemoveAt(pointCount);
					}
				}
			}
		}

		private void Filter(IDataPointFilter filterInterface, Series[] inputSeries, Series[] outputSeries)
		{
			CheckSeriesArrays(inputSeries, outputSeries);
			CheckXValuesAlignment(inputSeries);
			if (filterInterface == null)
			{
				throw new ArgumentNullException("filterInterface");
			}
			Series[] array = new Series[inputSeries.Length];
			for (int i = 0; i < inputSeries.Length; i++)
			{
				array[i] = inputSeries[i];
				if (outputSeries != null && outputSeries.Length > i)
				{
					array[i] = outputSeries[i];
				}
				if (array[i] != inputSeries[i])
				{
					array[i].Points.Clear();
					array[i].YValuesPerPoint = inputSeries[i].YValuesPerPoint;
					if (array[i].XValueType == ChartValueTypes.Auto || array[i].autoXValueType)
					{
						array[i].XValueType = inputSeries[i].XValueType;
						array[i].autoXValueType = true;
					}
					if (array[i].YValueType == ChartValueTypes.Auto || array[i].autoYValueType)
					{
						array[i].YValueType = inputSeries[i].YValueType;
						array[i].autoYValueType = true;
					}
				}
			}
			if (inputSeries[0].Points.Count == 0)
			{
				return;
			}
			int num = 0;
			int num2 = 0;
			while (num2 < inputSeries[0].Points.Count)
			{
				bool flag = false;
				bool flag2 = filterInterface.FilterDataPoint(inputSeries[0].Points[num2], inputSeries[0], num) == FilterMatchedPoints;
				for (int j = 0; j < inputSeries.Length; j++)
				{
					bool flag3 = flag2;
					if (array[j] != inputSeries[j])
					{
						if (flag3 && !FilterSetEmptyPoints)
						{
							flag3 = false;
						}
						else
						{
							array[j].Points.Add(inputSeries[j].Points[num2].Clone());
						}
					}
					if (!flag3)
					{
						continue;
					}
					if (FilterSetEmptyPoints)
					{
						array[j].Points[num2].Empty = true;
						for (int k = 0; k < array[j].Points[num2].YValues.Length; k++)
						{
							array[j].Points[num2].YValues[k] = 0.0;
						}
					}
					else
					{
						array[j].Points.RemoveAt(num2);
						flag = true;
					}
				}
				if (flag)
				{
					num2--;
				}
				num2++;
				num++;
			}
		}

		private int[] ConvertElementIndexesToArray(string rangeElements)
		{
			string[] array = rangeElements.Split(',');
			if (array.Length == 0)
			{
				throw new ArgumentException(SR.ExceptionDataManipulatorIndexUndefined, "rangeElements");
			}
			int[] array2 = new int[array.Length * 2];
			int num = 0;
			string[] array3 = array;
			foreach (string text in array3)
			{
				if (text.IndexOf('-') != -1)
				{
					string[] array4 = text.Split('-');
					if (array4.Length != 2)
					{
						throw new ArgumentException(SR.ExceptionDataManipulatorIndexFormatInvalid, "rangeElements");
					}
					try
					{
						array2[num] = int.Parse(array4[0], CultureInfo.InvariantCulture);
						array2[num + 1] = int.Parse(array4[1], CultureInfo.InvariantCulture);
						if (array2[num + 1] < array2[num])
						{
							int num2 = array2[num];
							array2[num] = array2[num + 1];
							array2[num + 1] = num2;
						}
					}
					catch (Exception)
					{
						throw new ArgumentException(SR.ExceptionDataManipulatorIndexFormatInvalid, "rangeElements");
					}
				}
				else
				{
					try
					{
						array2[num] = int.Parse(text, CultureInfo.InvariantCulture);
						array2[num + 1] = array2[num];
					}
					catch (Exception)
					{
						throw new ArgumentException(SR.ExceptionDataManipulatorIndexFormatInvalid, "rangeElements");
					}
				}
				num += 2;
			}
			return array2;
		}

		private bool CheckFilterElementCriteria(DateRangeType dateRange, int[] rangeElements, DataPoint point, Series series, int pointIndex)
		{
			DateTime dateTime = DateTime.FromOADate(point.XValue);
			for (int i = 0; i < rangeElements.Length; i += 2)
			{
				switch (dateRange)
				{
				case DateRangeType.Year:
					if (dateTime.Year >= rangeElements[i] && dateTime.Year <= rangeElements[i + 1])
					{
						return true;
					}
					break;
				case DateRangeType.Month:
					if (dateTime.Month >= rangeElements[i] && dateTime.Month <= rangeElements[i + 1])
					{
						return true;
					}
					break;
				case DateRangeType.DayOfWeek:
					if ((int)dateTime.DayOfWeek >= rangeElements[i] && (int)dateTime.DayOfWeek <= rangeElements[i + 1])
					{
						return true;
					}
					break;
				case DateRangeType.DayOfMonth:
					if (dateTime.Day >= rangeElements[i] && dateTime.Day <= rangeElements[i + 1])
					{
						return true;
					}
					break;
				case DateRangeType.Hour:
					if (dateTime.Hour >= rangeElements[i] && dateTime.Hour <= rangeElements[i + 1])
					{
						return true;
					}
					break;
				case DateRangeType.Minute:
					if (dateTime.Minute >= rangeElements[i] && dateTime.Minute <= rangeElements[i + 1])
					{
						return true;
					}
					break;
				}
			}
			return false;
		}

		public void Filter(DateRangeType dateRange, string rangeElements, string inputSeriesNames, string outputSeriesNames)
		{
			Filter(new PointElementFilter(this, dateRange, rangeElements), ConvertToSeriesArray(inputSeriesNames, createNew: false), ConvertToSeriesArray(outputSeriesNames, createNew: true));
		}

		public void Filter(DateRangeType dateRange, string rangeElements, Series inputSeries)
		{
			Filter(dateRange, rangeElements, inputSeries, null);
		}

		public void Filter(DateRangeType dateRange, string rangeElements, Series inputSeries, Series outputSeries)
		{
			Filter(new PointElementFilter(this, dateRange, rangeElements), ConvertToSeriesArray(inputSeries, createNew: false), ConvertToSeriesArray(outputSeries, createNew: false));
		}

		public void Filter(DateRangeType dateRange, string rangeElements, string inputSeriesNames)
		{
			Filter(dateRange, rangeElements, inputSeriesNames, "");
		}

		public void Filter(CompareMethod compareMethod, double compareValue, Series inputSeries)
		{
			Filter(compareMethod, compareValue, inputSeries, null, "Y");
		}

		public void Filter(CompareMethod compareMethod, double compareValue, Series inputSeries, Series outputSeries)
		{
			Filter(new PointValueFilter(this, compareMethod, compareValue, "Y"), ConvertToSeriesArray(inputSeries, createNew: false), ConvertToSeriesArray(outputSeries, createNew: false));
		}

		public void Filter(CompareMethod compareMethod, double compareValue, Series inputSeries, Series outputSeries, string usingValue)
		{
			Filter(new PointValueFilter(this, compareMethod, compareValue, usingValue), ConvertToSeriesArray(inputSeries, createNew: false), ConvertToSeriesArray(outputSeries, createNew: false));
		}

		public void Filter(CompareMethod compareMethod, double compareValue, string inputSeriesNames)
		{
			Filter(compareMethod, compareValue, inputSeriesNames, "", "Y");
		}

		public void Filter(CompareMethod compareMethod, double compareValue, string inputSeriesNames, string outputSeriesNames)
		{
			Filter(new PointValueFilter(this, compareMethod, compareValue, "Y"), ConvertToSeriesArray(inputSeriesNames, createNew: false), ConvertToSeriesArray(outputSeriesNames, createNew: true));
		}

		public void Filter(CompareMethod compareMethod, double compareValue, string inputSeriesNames, string outputSeriesNames, string usingValue)
		{
			Filter(new PointValueFilter(this, compareMethod, compareValue, usingValue), ConvertToSeriesArray(inputSeriesNames, createNew: false), ConvertToSeriesArray(outputSeriesNames, createNew: true));
		}

		public void FilterTopN(int pointCount, string inputSeriesNames, string outputSeriesNames, string usingValue, bool getTopValues)
		{
			FilterTopN(pointCount, ConvertToSeriesArray(inputSeriesNames, createNew: false), ConvertToSeriesArray(outputSeriesNames, createNew: true), usingValue, getTopValues);
		}

		public void FilterTopN(int pointCount, Series inputSeries)
		{
			FilterTopN(pointCount, ConvertToSeriesArray(inputSeries, createNew: false), null, "Y", getTopValues: true);
		}

		public void FilterTopN(int pointCount, Series inputSeries, Series outputSeries)
		{
			FilterTopN(pointCount, ConvertToSeriesArray(inputSeries, createNew: false), ConvertToSeriesArray(outputSeries, createNew: false), "Y", getTopValues: true);
		}

		public void FilterTopN(int pointCount, Series inputSeries, Series outputSeries, string usingValue)
		{
			FilterTopN(pointCount, ConvertToSeriesArray(inputSeries, createNew: false), ConvertToSeriesArray(outputSeries, createNew: false), usingValue, getTopValues: true);
		}

		public void FilterTopN(int pointCount, Series inputSeries, Series outputSeries, string usingValue, bool getTopValues)
		{
			FilterTopN(pointCount, ConvertToSeriesArray(inputSeries, createNew: false), ConvertToSeriesArray(outputSeries, createNew: false), usingValue, getTopValues);
		}

		public void FilterTopN(int pointCount, string inputSeriesNames)
		{
			FilterTopN(pointCount, ConvertToSeriesArray(inputSeriesNames, createNew: false), null, "Y", getTopValues: true);
		}

		public void FilterTopN(int pointCount, string inputSeriesNames, string outputSeriesNames)
		{
			FilterTopN(pointCount, ConvertToSeriesArray(inputSeriesNames, createNew: false), ConvertToSeriesArray(outputSeriesNames, createNew: true), "Y", getTopValues: true);
		}

		public void FilterTopN(int pointCount, string inputSeriesNames, string outputSeriesNames, string usingValue)
		{
			FilterTopN(pointCount, ConvertToSeriesArray(inputSeriesNames, createNew: false), ConvertToSeriesArray(outputSeriesNames, createNew: true), usingValue, getTopValues: true);
		}

		internal void Filter(IDataPointFilter filterInterface, Series inputSeries)
		{
			Filter(filterInterface, ConvertToSeriesArray(inputSeries, createNew: false), null);
		}

		internal void Filter(IDataPointFilter filterInterface, Series inputSeries, Series outputSeries)
		{
			Filter(filterInterface, ConvertToSeriesArray(inputSeries, createNew: false), ConvertToSeriesArray(outputSeries, createNew: false));
		}

		internal void Filter(IDataPointFilter filterInterface, string inputSeriesNames)
		{
			Filter(filterInterface, ConvertToSeriesArray(inputSeriesNames, createNew: false), null);
		}

		internal void Filter(IDataPointFilter filterInterface, string inputSeriesNames, string outputSeriesNames)
		{
			Filter(filterInterface, ConvertToSeriesArray(inputSeriesNames, createNew: false), ConvertToSeriesArray(outputSeriesNames, createNew: true));
		}

		private void GroupByAxisLabel(string formula, Series[] inputSeries, Series[] outputSeries)
		{
			CheckSeriesArrays(inputSeries, outputSeries);
			int outputValuesNumber = 1;
			GroupingFunctionInfo[] groupingFunctions = GetGroupingFunctions(inputSeries, formula, out outputValuesNumber);
			for (int i = 0; i < inputSeries.Length; i++)
			{
				Series series = inputSeries[i];
				Series series2 = series;
				if (outputSeries != null && i < outputSeries.Length)
				{
					series2 = outputSeries[i];
					if (series2.Name != series.Name)
					{
						series2.Points.Clear();
						if (series2.XValueType == ChartValueTypes.Auto || series2.autoXValueType)
						{
							series2.XValueType = series.XValueType;
							series2.autoXValueType = true;
						}
						if (series2.YValueType == ChartValueTypes.Auto || series2.autoYValueType)
						{
							series2.YValueType = series.YValueType;
							series2.autoYValueType = true;
						}
					}
				}
				if (series != series2)
				{
					Series series3 = new Series("Temp", series.YValuesPerPoint);
					foreach (DataPoint point in series.Points)
					{
						DataPoint dataPoint2 = new DataPoint(series3);
						dataPoint2.AxisLabel = point.AxisLabel;
						dataPoint2.XValue = point.XValue;
						point.YValues.CopyTo(dataPoint2.YValues, 0);
						dataPoint2.Empty = point.Empty;
						series3.Points.Add(dataPoint2);
					}
					series = series3;
				}
				if (series.Points.Count == 0)
				{
					continue;
				}
				series2.YValuesPerPoint = outputValuesNumber - 1;
				series.Sort(PointsSortOrder.Ascending, "AxisLabel");
				int num = 0;
				int num2 = 0;
				double[] array = new double[outputValuesNumber];
				string text = null;
				bool flag = false;
				int numberOfEmptyPoints = 0;
				for (int j = 0; j <= series.Points.Count; j++)
				{
					if (flag)
					{
						break;
					}
					bool flag2 = false;
					if (j == series.Points.Count)
					{
						flag = true;
						num2 = j - 1;
						j = num2;
						flag2 = true;
					}
					if (!flag2 && text == null)
					{
						text = series.Points[j].AxisLabel;
					}
					if (!flag2 && series.Points[j].AxisLabel != text)
					{
						num2 = j - 1;
						flag2 = true;
					}
					if (flag2)
					{
						ProcessPointValues(groupingFunctions, array, inputSeries[i], series.Points[j], j, num, num2, finalPass: true, ref numberOfEmptyPoints);
						if (groupingFunctions[0].function == GroupingFunction.Center)
						{
							array[0] = (inputSeries[i].Points[num].XValue + inputSeries[i].Points[num2].XValue) / 2.0;
						}
						else if (groupingFunctions[0].function == GroupingFunction.First)
						{
							array[0] = inputSeries[i].Points[num].XValue;
						}
						if (groupingFunctions[0].function == GroupingFunction.Last)
						{
							array[0] = inputSeries[i].Points[num2].XValue;
						}
						DataPoint dataPoint3 = new DataPoint();
						dataPoint3.ResizeYValueArray(outputValuesNumber - 1);
						dataPoint3.XValue = array[0];
						dataPoint3.AxisLabel = text;
						for (int k = 1; k < array.Length; k++)
						{
							dataPoint3.YValues[k - 1] = array[k];
						}
						int num3 = series2.Points.Count;
						if (series2 == series)
						{
							num3 = num;
							j = num3 + 1;
							for (int l = num; l <= num2; l++)
							{
								series2.Points.RemoveAt(num);
							}
						}
						series2.Points.Insert(num3, dataPoint3);
						num = j;
						num2 = j;
						numberOfEmptyPoints = 0;
						text = null;
						j--;
					}
					else
					{
						ProcessPointValues(groupingFunctions, array, inputSeries[i], series.Points[j], j, num, num2, finalPass: false, ref numberOfEmptyPoints);
					}
				}
			}
		}

		private void Group(string formula, double interval, IntervalType intervalType, double intervalOffset, IntervalType intervalOffsetType, Series[] inputSeries, Series[] outputSeries)
		{
			CheckSeriesArrays(inputSeries, outputSeries);
			int outputValuesNumber = 1;
			GroupingFunctionInfo[] groupingFunctions = GetGroupingFunctions(inputSeries, formula, out outputValuesNumber);
			for (int i = 0; i < inputSeries.Length; i++)
			{
				Series series = inputSeries[i];
				Series series2 = series;
				if (outputSeries != null && i < outputSeries.Length)
				{
					series2 = outputSeries[i];
					if (series2.Name != series.Name)
					{
						series2.Points.Clear();
						if (series2.XValueType == ChartValueTypes.Auto || series2.autoXValueType)
						{
							series2.XValueType = series.XValueType;
							series2.autoXValueType = true;
						}
						if (series2.YValueType == ChartValueTypes.Auto || series2.autoYValueType)
						{
							series2.YValueType = series.YValueType;
							series2.autoYValueType = true;
						}
					}
				}
				if (series.Points.Count == 0)
				{
					continue;
				}
				series2.YValuesPerPoint = outputValuesNumber - 1;
				int num = 0;
				int num2 = 0;
				double num3 = 0.0;
				double num4 = 0.0;
				num3 = series.Points[0].XValue;
				num3 = AlignIntervalStart(num3, interval, ConvertIntervalType(intervalType));
				double num5 = 0.0;
				if (intervalOffset != 0.0)
				{
					num5 = num3 + GetIntervalSize(num3, intervalOffset, ConvertIntervalType(intervalOffsetType));
					if (series.Points[0].XValue < num5)
					{
						num3 = ((intervalType != 0) ? (num5 - GetIntervalSize(num5, interval, ConvertIntervalType(intervalType))) : (num5 + GetIntervalSize(num5, 0.0 - interval, ConvertIntervalType(intervalType))));
						num4 = num5;
					}
					else
					{
						num3 = num5;
						num4 = num3 + GetIntervalSize(num3, interval, ConvertIntervalType(intervalType));
					}
				}
				else
				{
					num4 = num3 + GetIntervalSize(num3, interval, ConvertIntervalType(intervalType));
				}
				double[] array = new double[outputValuesNumber];
				bool flag = false;
				int numberOfEmptyPoints = 0;
				int num6 = 0;
				for (int j = 0; j <= series.Points.Count && !flag; j++)
				{
					bool flag2 = false;
					if (j > 0 && j < series.Points.Count && series.Points[j].XValue < series.Points[j - 1].XValue)
					{
						throw new InvalidOperationException(SR.ExceptionDataManipulatorGroupedSeriesNotSorted);
					}
					if (j == series.Points.Count)
					{
						flag = true;
						num2 = j - 1;
						j = num2;
						flag2 = true;
					}
					if (!flag2 && series.Points[j].XValue >= num4)
					{
						if (j == 0)
						{
							continue;
						}
						num2 = j - 1;
						flag2 = true;
					}
					if (flag2)
					{
						if (num6 > numberOfEmptyPoints)
						{
							ProcessPointValues(groupingFunctions, array, inputSeries[i], series.Points[j], j, num, num2, finalPass: true, ref numberOfEmptyPoints);
							if (groupingFunctions[0].function == GroupingFunction.Center)
							{
								array[0] = (num3 + num4) / 2.0;
							}
							else if (groupingFunctions[0].function == GroupingFunction.First)
							{
								array[0] = num3;
							}
							if (groupingFunctions[0].function == GroupingFunction.Last)
							{
								array[0] = num4;
							}
							DataPoint dataPoint = new DataPoint();
							dataPoint.ResizeYValueArray(outputValuesNumber - 1);
							dataPoint.XValue = array[0];
							for (int k = 1; k < array.Length; k++)
							{
								dataPoint.YValues[k - 1] = array[k];
							}
							int num7 = series2.Points.Count;
							if (series2 == series)
							{
								num7 = num;
								j = num7 + 1;
								for (int l = num; l <= num2; l++)
								{
									series2.Points.RemoveAt(num);
								}
							}
							series2.Points.Insert(num7, dataPoint);
						}
						num3 = num4;
						num4 = num3 + GetIntervalSize(num3, interval, ConvertIntervalType(intervalType));
						num = j;
						num2 = j;
						num6 = 0;
						numberOfEmptyPoints = 0;
						j--;
					}
					else
					{
						ProcessPointValues(groupingFunctions, array, inputSeries[i], series.Points[j], j, num, num2, finalPass: false, ref numberOfEmptyPoints);
						num6++;
					}
				}
			}
		}

		private void ProcessPointValues(GroupingFunctionInfo[] functions, double[] pointTempValues, Series series, DataPoint point, int pointIndex, int intervalFirstIndex, int intervalLastIndex, bool finalPass, ref int numberOfEmptyPoints)
		{
			GroupingFunctionInfo[] array;
			if (pointIndex == intervalFirstIndex && !finalPass)
			{
				int num = 0;
				array = functions;
				foreach (GroupingFunctionInfo groupingFunctionInfo in array)
				{
					if (num > point.YValues.Length)
					{
						break;
					}
					pointTempValues[groupingFunctionInfo.outputIndex] = 0.0;
					if (groupingFunctionInfo.function == GroupingFunction.Min)
					{
						pointTempValues[groupingFunctionInfo.outputIndex] = double.MaxValue;
					}
					else if (groupingFunctionInfo.function == GroupingFunction.Max)
					{
						pointTempValues[groupingFunctionInfo.outputIndex] = double.MinValue;
					}
					else if (groupingFunctionInfo.function == GroupingFunction.First)
					{
						if (num == 0)
						{
							pointTempValues[0] = point.XValue;
						}
						else
						{
							pointTempValues[groupingFunctionInfo.outputIndex] = point.YValues[num - 1];
						}
					}
					else if (groupingFunctionInfo.function == GroupingFunction.HiLo || groupingFunctionInfo.function == GroupingFunction.HiLoOpCl)
					{
						pointTempValues[groupingFunctionInfo.outputIndex] = double.MinValue;
						pointTempValues[groupingFunctionInfo.outputIndex + 1] = double.MaxValue;
						if (groupingFunctionInfo.function == GroupingFunction.HiLoOpCl)
						{
							pointTempValues[groupingFunctionInfo.outputIndex + 2] = point.YValues[num - 1];
							pointTempValues[groupingFunctionInfo.outputIndex + 3] = 0.0;
						}
					}
					num++;
				}
			}
			if (!finalPass)
			{
				if (point.Empty && base.IgnoreEmptyPoints)
				{
					numberOfEmptyPoints++;
					return;
				}
				int num2 = 0;
				array = functions;
				foreach (GroupingFunctionInfo groupingFunctionInfo2 in array)
				{
					if (num2 > point.YValues.Length)
					{
						break;
					}
					if (groupingFunctionInfo2.function == GroupingFunction.Min && !point.Empty && base.IgnoreEmptyPoints)
					{
						pointTempValues[groupingFunctionInfo2.outputIndex] = Math.Min(pointTempValues[groupingFunctionInfo2.outputIndex], point.YValues[num2 - 1]);
					}
					else if (groupingFunctionInfo2.function == GroupingFunction.Max)
					{
						pointTempValues[groupingFunctionInfo2.outputIndex] = Math.Max(pointTempValues[groupingFunctionInfo2.outputIndex], point.YValues[num2 - 1]);
					}
					else if (groupingFunctionInfo2.function == GroupingFunction.Ave || groupingFunctionInfo2.function == GroupingFunction.Sum)
					{
						if (num2 == 0)
						{
							pointTempValues[0] += point.XValue;
						}
						else
						{
							pointTempValues[groupingFunctionInfo2.outputIndex] += point.YValues[num2 - 1];
						}
					}
					else if (groupingFunctionInfo2.function == GroupingFunction.Variance || groupingFunctionInfo2.function == GroupingFunction.Deviation)
					{
						pointTempValues[groupingFunctionInfo2.outputIndex] += point.YValues[num2 - 1];
					}
					else if (groupingFunctionInfo2.function == GroupingFunction.Last)
					{
						if (num2 == 0)
						{
							pointTempValues[0] = point.XValue;
						}
						else
						{
							pointTempValues[groupingFunctionInfo2.outputIndex] = point.YValues[num2 - 1];
						}
					}
					else if (groupingFunctionInfo2.function == GroupingFunction.Count)
					{
						pointTempValues[groupingFunctionInfo2.outputIndex] += 1.0;
					}
					else if (groupingFunctionInfo2.function == GroupingFunction.HiLo || groupingFunctionInfo2.function == GroupingFunction.HiLoOpCl)
					{
						pointTempValues[groupingFunctionInfo2.outputIndex] = Math.Max(pointTempValues[groupingFunctionInfo2.outputIndex], point.YValues[num2 - 1]);
						pointTempValues[groupingFunctionInfo2.outputIndex + 1] = Math.Min(pointTempValues[groupingFunctionInfo2.outputIndex + 1], point.YValues[num2 - 1]);
						if (groupingFunctionInfo2.function == GroupingFunction.HiLoOpCl)
						{
							pointTempValues[groupingFunctionInfo2.outputIndex + 3] = point.YValues[num2 - 1];
						}
					}
					num2++;
				}
			}
			if (!finalPass)
			{
				return;
			}
			int num3 = 0;
			array = functions;
			foreach (GroupingFunctionInfo groupingFunctionInfo3 in array)
			{
				if (num3 > point.YValues.Length)
				{
					break;
				}
				if (groupingFunctionInfo3.function == GroupingFunction.Ave)
				{
					pointTempValues[groupingFunctionInfo3.outputIndex] /= intervalLastIndex - intervalFirstIndex - numberOfEmptyPoints + 1;
				}
				if (groupingFunctionInfo3.function == GroupingFunction.DistinctCount)
				{
					pointTempValues[groupingFunctionInfo3.outputIndex] = 0.0;
					ArrayList arrayList = new ArrayList(intervalLastIndex - intervalFirstIndex + 1);
					for (int j = intervalFirstIndex; j <= intervalLastIndex; j++)
					{
						if ((!series.Points[j].Empty || !base.IgnoreEmptyPoints) && !arrayList.Contains(series.Points[j].YValues[num3 - 1]))
						{
							arrayList.Add(series.Points[j].YValues[num3 - 1]);
						}
					}
					pointTempValues[groupingFunctionInfo3.outputIndex] = arrayList.Count;
				}
				else if (groupingFunctionInfo3.function == GroupingFunction.Variance || groupingFunctionInfo3.function == GroupingFunction.Deviation)
				{
					double num4 = pointTempValues[groupingFunctionInfo3.outputIndex] / (double)(intervalLastIndex - intervalFirstIndex - numberOfEmptyPoints + 1);
					pointTempValues[groupingFunctionInfo3.outputIndex] = 0.0;
					for (int k = intervalFirstIndex; k <= intervalLastIndex; k++)
					{
						if (!series.Points[k].Empty || !base.IgnoreEmptyPoints)
						{
							pointTempValues[groupingFunctionInfo3.outputIndex] += Math.Pow(series.Points[k].YValues[num3 - 1] - num4, 2.0);
						}
					}
					pointTempValues[groupingFunctionInfo3.outputIndex] /= intervalLastIndex - intervalFirstIndex - numberOfEmptyPoints + 1;
					if (groupingFunctionInfo3.function == GroupingFunction.Deviation)
					{
						pointTempValues[groupingFunctionInfo3.outputIndex] = Math.Sqrt(pointTempValues[groupingFunctionInfo3.outputIndex]);
					}
				}
				num3++;
			}
		}

		private GroupingFunctionInfo[] GetGroupingFunctions(Series[] inputSeries, string formula, out int outputValuesNumber)
		{
			int num = 0;
			foreach (Series series in inputSeries)
			{
				num = Math.Max(num, series.YValuesPerPoint);
			}
			GroupingFunctionInfo[] array = new GroupingFunctionInfo[num + 1];
			for (int j = 0; j < array.Length; j++)
			{
				array[j] = new GroupingFunctionInfo();
			}
			string[] array2 = formula.Split(',');
			if (array2.Length == 0)
			{
				throw new ArgumentException(SR.ExceptionDataManipulatorGroupingFormulaUndefined);
			}
			GroupingFunctionInfo groupingFunctionInfo = new GroupingFunctionInfo();
			string[] array3 = array2;
			for (int i = 0; i < array3.Length; i++)
			{
				string text = array3[i].Trim();
				text = text.ToUpper(CultureInfo.InvariantCulture);
				int valueIndex = 1;
				GroupingFunction function = ParseFormulaAndValueType(text, out valueIndex);
				if (groupingFunctionInfo.function == GroupingFunction.None)
				{
					groupingFunctionInfo.function = function;
				}
				if (valueIndex >= array.Length)
				{
					throw new ArgumentException(SR.ExceptionDataManipulatorYValuesIndexExceeded(text));
				}
				if (array[valueIndex].function != 0)
				{
					throw new ArgumentException(SR.ExceptionDataManipulatorGroupingFormulaAlreadyDefined(text));
				}
				array[valueIndex].function = function;
			}
			if (array[0].function == GroupingFunction.None)
			{
				array[0].function = GroupingFunction.First;
			}
			for (int k = 1; k < array.Length; k++)
			{
				if (array[k].function == GroupingFunction.None)
				{
					array[k].function = groupingFunctionInfo.function;
				}
			}
			outputValuesNumber = 0;
			for (int l = 0; l < array.Length; l++)
			{
				array[l].outputIndex = outputValuesNumber;
				if (array[l].function == GroupingFunction.HiLoOpCl)
				{
					outputValuesNumber += 3;
				}
				else if (array[l].function == GroupingFunction.HiLo)
				{
					outputValuesNumber++;
				}
				outputValuesNumber++;
			}
			if (array[0].function != GroupingFunction.First && array[0].function != GroupingFunction.Last && array[0].function != GroupingFunction.Center)
			{
				throw new ArgumentException(SR.ExceptionDataManipulatorGroupingFormulaUnsupported);
			}
			return array;
		}

		private GroupingFunction ParseFormulaAndValueType(string formulaString, out int valueIndex)
		{
			valueIndex = 1;
			string[] array = formulaString.Split(':');
			if (array.Length < 1 && array.Length > 2)
			{
				throw new ArgumentException(SR.ExceptionDataManipulatorGroupingFormulaFormatInvalid(formulaString));
			}
			if (array.Length == 2)
			{
				if (array[0] == "X")
				{
					valueIndex = 0;
				}
				else
				{
					if (!array[0].StartsWith("Y", StringComparison.Ordinal))
					{
						throw new ArgumentException(SR.ExceptionDataManipulatorGroupingFormulaFormatInvalid(formulaString));
					}
					array[0] = array[0].TrimStart('Y');
					if (array[0].Length == 0)
					{
						valueIndex = 1;
					}
					else
					{
						try
						{
							valueIndex = int.Parse(array[0], CultureInfo.InvariantCulture);
						}
						catch (Exception)
						{
							throw new ArgumentException(SR.ExceptionDataManipulatorGroupingFormulaFormatInvalid(formulaString));
						}
					}
				}
			}
			if (array[array.Length - 1] == "MIN")
			{
				return GroupingFunction.Min;
			}
			if (array[array.Length - 1] == "MAX")
			{
				return GroupingFunction.Max;
			}
			if (array[array.Length - 1] == "AVE")
			{
				return GroupingFunction.Ave;
			}
			if (array[array.Length - 1] == "SUM")
			{
				return GroupingFunction.Sum;
			}
			if (array[array.Length - 1] == "FIRST")
			{
				return GroupingFunction.First;
			}
			if (array[array.Length - 1] == "LAST")
			{
				return GroupingFunction.Last;
			}
			if (array[array.Length - 1] == "HILOOPCL")
			{
				return GroupingFunction.HiLoOpCl;
			}
			if (array[array.Length - 1] == "HILO")
			{
				return GroupingFunction.HiLo;
			}
			if (array[array.Length - 1] == "COUNT")
			{
				return GroupingFunction.Count;
			}
			if (array[array.Length - 1] == "DISTINCTCOUNT")
			{
				return GroupingFunction.DistinctCount;
			}
			if (array[array.Length - 1] == "VARIANCE")
			{
				return GroupingFunction.Variance;
			}
			if (array[array.Length - 1] == "DEVIATION")
			{
				return GroupingFunction.Deviation;
			}
			if (array[array.Length - 1] == "CENTER")
			{
				return GroupingFunction.Center;
			}
			throw new ArgumentException(SR.ExceptionDataManipulatorGroupingFormulaNameInvalid(formulaString));
		}

		private void CheckSeriesArrays(Series[] inputSeries, Series[] outputSeries)
		{
			if (inputSeries == null || inputSeries.Length == 0)
			{
				throw new ArgumentException(SR.ExceptionDataManipulatorGroupingInputSeriesUndefined);
			}
			if (outputSeries != null && outputSeries.Length != inputSeries.Length)
			{
				throw new ArgumentException(SR.ExceptionDataManipulatorGroupingInputOutputSeriesNumberMismatch);
			}
		}

		public void Group(string formula, double interval, IntervalType intervalType, Series inputSeries)
		{
			Group(formula, interval, intervalType, inputSeries, null);
		}

		public void Group(string formula, double interval, IntervalType intervalType, string inputSeriesName)
		{
			Group(formula, interval, intervalType, inputSeriesName, "");
		}

		public void Group(string formula, double interval, IntervalType intervalType, double intervalOffset, IntervalType intervalOffsetType, Series inputSeries)
		{
			Group(formula, interval, intervalType, intervalOffset, intervalOffsetType, inputSeries, null);
		}

		public void Group(string formula, double interval, IntervalType intervalType, double intervalOffset, IntervalType intervalOffsetType, string inputSeriesName)
		{
			Group(formula, interval, intervalType, intervalOffset, intervalOffsetType, inputSeriesName, "");
		}

		public void GroupByAxisLabel(string formula, string inputSeriesName, string outputSeriesName)
		{
			GroupByAxisLabel(formula, ConvertToSeriesArray(inputSeriesName, createNew: false), ConvertToSeriesArray(outputSeriesName, createNew: true));
		}

		public void GroupByAxisLabel(string formula, Series inputSeries)
		{
			GroupByAxisLabel(formula, inputSeries, null);
		}

		public void GroupByAxisLabel(string formula, string inputSeriesName)
		{
			GroupByAxisLabel(formula, inputSeriesName, null);
		}

		public void Group(string formula, double interval, IntervalType intervalType, double intervalOffset, IntervalType intervalOffsetType, string inputSeriesName, string outputSeriesName)
		{
			Group(formula, interval, intervalType, intervalOffset, intervalOffsetType, ConvertToSeriesArray(inputSeriesName, createNew: false), ConvertToSeriesArray(outputSeriesName, createNew: true));
		}

		public void Group(string formula, double interval, IntervalType intervalType, Series inputSeries, Series outputSeries)
		{
			Group(formula, interval, intervalType, 0.0, IntervalType.Number, inputSeries, outputSeries);
		}

		public void Group(string formula, double interval, IntervalType intervalType, string inputSeriesName, string outputSeriesName)
		{
			Group(formula, interval, intervalType, 0.0, IntervalType.Number, inputSeriesName, outputSeriesName);
		}

		public void Group(string formula, double interval, IntervalType intervalType, double intervalOffset, IntervalType intervalOffsetType, Series inputSeries, Series outputSeries)
		{
			Group(formula, interval, intervalType, intervalOffset, intervalOffsetType, ConvertToSeriesArray(inputSeries, createNew: false), ConvertToSeriesArray(outputSeries, createNew: false));
		}

		public void GroupByAxisLabel(string formula, Series inputSeries, Series outputSeries)
		{
			GroupByAxisLabel(formula, ConvertToSeriesArray(inputSeries, createNew: false), ConvertToSeriesArray(outputSeries, createNew: false));
		}
	}
}
