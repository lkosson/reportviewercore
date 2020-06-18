using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Globalization;

namespace Microsoft.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeDataPointCollection_DataPointCollection")]
	internal class DataPointCollection : IList, ICollection, IEnumerable
	{
		internal ArrayList array = new ArrayList();

		internal Series series;

		public DataPoint this[int index]
		{
			get
			{
				return (DataPoint)array[index];
			}
			set
			{
				DataPointInit(ref value);
				array[index] = value;
			}
		}

		object IList.this[int index]
		{
			get
			{
				return array[index];
			}
			set
			{
				array[index] = value;
			}
		}

		public bool IsFixedSize => array.IsFixedSize;

		public bool IsReadOnly => array.IsReadOnly;

		public int Count => array.Count;

		public bool IsSynchronized => array.IsSynchronized;

		public object SyncRoot => array.SyncRoot;

		private DataPointCollection()
		{
			series = null;
		}

		public DataPointCollection(Series series)
		{
			this.series = series;
		}

		internal void DataPointInit(ref DataPoint dataPoint)
		{
			DataPointInit(series, ref dataPoint);
		}

		internal static void DataPointInit(Series series, ref DataPoint dataPoint)
		{
			dataPoint.series = series;
			if (dataPoint.AxisLabel.Length > 0 && series != null)
			{
				series.noLabelsInPoints = false;
			}
		}

		internal static void ParsePointFieldsParameter(string otherFields, ref string[] otherAttributeNames, ref string[] otherFieldNames, ref string[] otherValueFormat)
		{
			if (otherFields == null || otherFields.Length <= 0)
			{
				return;
			}
			otherAttributeNames = otherFields.Replace(",,", "\n").Split(',');
			otherFieldNames = new string[otherAttributeNames.Length];
			otherValueFormat = new string[otherAttributeNames.Length];
			int num = 0;
			while (true)
			{
				if (num < otherAttributeNames.Length)
				{
					int num2 = otherAttributeNames[num].IndexOf('=');
					if (num2 <= 0)
					{
						break;
					}
					otherFieldNames[num] = otherAttributeNames[num].Substring(num2 + 1);
					otherAttributeNames[num] = otherAttributeNames[num].Substring(0, num2);
					int num3 = otherFieldNames[num].IndexOf('{');
					if (num3 > 0 && otherFieldNames[num][otherFieldNames[num].Length - 1] == '}')
					{
						otherValueFormat[num] = otherFieldNames[num].Substring(num3 + 1);
						otherValueFormat[num] = otherValueFormat[num].Trim('{', '}');
						otherFieldNames[num] = otherFieldNames[num].Substring(0, num3);
					}
					otherAttributeNames[num] = otherAttributeNames[num].Trim().Replace("\n", ",");
					otherFieldNames[num] = otherFieldNames[num].Trim().Replace("\n", ",");
					num++;
					continue;
				}
				return;
			}
			throw new ArgumentException(SR.ExceptionParameterFormatInvalid, "otherFields");
		}

		public void DataBind(IEnumerable dataSource, string xField, string yFields, string otherFields)
		{
			series.TraceWrite("ChartDataBinding", SR.TraceMessagePopulatingSeriesDataPoints(series.Name));
			string[] array = null;
			if (yFields != null)
			{
				array = yFields.Replace(",,", "\n").Split(',');
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = array[i].Replace("\n", ",");
				}
			}
			string[] otherAttributeNames = null;
			string[] otherFieldNames = null;
			string[] otherValueFormat = null;
			ParsePointFieldsParameter(otherFields, ref otherAttributeNames, ref otherFieldNames, ref otherValueFormat);
			if (dataSource == null)
			{
				throw new ArgumentNullException("dataSource", SR.ExceptionDataPointInsertionNoDataSource);
			}
			if (dataSource is string)
			{
				throw new ArgumentException(SR.ExceptionDataBindSeriesToString, "dataSource");
			}
			if (array == null || array.GetLength(0) > series.YValuesPerPoint)
			{
				throw new ArgumentOutOfRangeException("yFields", SR.ExceptionDataPointYValuesCountMismatch(series.YValuesPerPoint.ToString(CultureInfo.InvariantCulture)));
			}
			this.array.Clear();
			IEnumerator dataSourceEnumerator = GetDataSourceEnumerator(dataSource);
			if (dataSourceEnumerator.GetType() != typeof(DbEnumerator))
			{
				dataSourceEnumerator.Reset();
			}
			bool flag = true;
			object[] array2 = new object[array.Length];
			object obj = null;
			bool flag2 = true;
			do
			{
				if (flag)
				{
					flag = dataSourceEnumerator.MoveNext();
				}
				if (flag2)
				{
					flag2 = false;
					AutoDetectValuesType(series, dataSourceEnumerator, xField, dataSourceEnumerator, array[0]);
				}
				if (!flag)
				{
					continue;
				}
				DataPoint dataPoint = new DataPoint(series);
				bool flag3 = false;
				if (xField.Length > 0)
				{
					obj = ConvertEnumerationItem(dataSourceEnumerator.Current, xField);
					if (IsEmptyValue(obj))
					{
						flag3 = true;
						obj = 0.0;
					}
				}
				if (array.Length == 0)
				{
					array2[0] = ConvertEnumerationItem(dataSourceEnumerator.Current, null);
					if (IsEmptyValue(array2[0]))
					{
						flag3 = true;
						array2[0] = 0.0;
					}
				}
				else
				{
					for (int j = 0; j < array.Length; j++)
					{
						array2[j] = ConvertEnumerationItem(dataSourceEnumerator.Current, array[j]);
						if (IsEmptyValue(array2[j]))
						{
							flag3 = true;
							array2[j] = 0.0;
						}
					}
				}
				if (otherAttributeNames != null && otherAttributeNames.Length != 0)
				{
					for (int k = 0; k < otherFieldNames.Length; k++)
					{
						object obj2 = ConvertEnumerationItem(dataSourceEnumerator.Current, otherFieldNames[k]);
						if (!IsEmptyValue(obj2))
						{
							dataPoint.SetPointAttribute(obj2, otherAttributeNames[k], otherValueFormat[k]);
						}
					}
				}
				if (flag3)
				{
					if (obj != null)
					{
						dataPoint.SetValueXY(obj, array2);
					}
					else
					{
						dataPoint.SetValueXY(0, array2);
					}
					DataPointInit(ref dataPoint);
					dataPoint.Empty = true;
					this.array.Add(dataPoint);
				}
				else
				{
					if (obj != null)
					{
						dataPoint.SetValueXY(obj, array2);
					}
					else
					{
						dataPoint.SetValueXY(0, array2);
					}
					DataPointInit(ref dataPoint);
					this.array.Add(dataPoint);
				}
			}
			while (flag);
			series.TraceWrite("ChartDataBinding", SR.TraceMessageSeriesPopulatedWithDataPoints(series.Name, this.array.Count.ToString(CultureInfo.CurrentCulture)));
		}

		public void DataBindY(params IEnumerable[] yValue)
		{
			DataBindXY(null, yValue);
		}

		public void DataBindXY(IEnumerable xValue, params IEnumerable[] yValues)
		{
			series.TraceWrite("ChartDataBinding", SR.TraceMessagePopulatingSeriesDataPoints(series.Name));
			for (int i = 0; i < yValues.Length; i++)
			{
				if (yValues[i] is string)
				{
					throw new ArgumentException(SR.ExceptionDataBindYValuesToString, "yValues");
				}
			}
			if (yValues == null || yValues.GetLength(0) == 0)
			{
				throw new ArgumentNullException("yValues", SR.ExceptionDataPointBindingYValueNotSpecified);
			}
			if (yValues.GetLength(0) > series.YValuesPerPoint)
			{
				throw new ArgumentOutOfRangeException("yValues", SR.ExceptionDataPointYValuesBindingCountMismatch(series.YValuesPerPoint.ToString(CultureInfo.InvariantCulture)));
			}
			this.array.Clear();
			IEnumerator enumerator = null;
			IEnumerator[] array = new IEnumerator[yValues.GetLength(0)];
			if (xValue != null)
			{
				if (xValue is string)
				{
					throw new ArgumentException(SR.ExceptionDataBindXValuesToString, "xValue");
				}
				enumerator = GetDataSourceEnumerator(xValue);
				if (enumerator.GetType() != typeof(DbEnumerator))
				{
					enumerator.Reset();
				}
			}
			for (int j = 0; j < yValues.Length; j++)
			{
				array[j] = GetDataSourceEnumerator(yValues[j]);
				if (array[j].GetType() != typeof(DbEnumerator))
				{
					array[j].Reset();
				}
			}
			bool flag = false;
			bool flag2 = true;
			object[] array2 = new object[series.YValuesPerPoint];
			object obj = null;
			bool flag3 = true;
			do
			{
				flag2 = true;
				for (int k = 0; k < yValues.Length; k++)
				{
					if (flag2)
					{
						flag2 = array[k].MoveNext();
					}
				}
				if (xValue != null)
				{
					flag = enumerator.MoveNext();
					if (flag2 && !flag)
					{
						throw new ArgumentOutOfRangeException("xValue", SR.ExceptionDataPointInsertionXValuesQtyIsLessYValues);
					}
				}
				if (flag3)
				{
					flag3 = false;
					AutoDetectValuesType(series, enumerator, null, array[0], null);
				}
				if (!(flag || flag2))
				{
					continue;
				}
				DataPoint dataPoint = new DataPoint(series);
				bool flag4 = false;
				if (flag)
				{
					obj = ConvertEnumerationItem(enumerator.Current, null);
					if (obj is DBNull || obj == null)
					{
						flag4 = true;
						obj = 0.0;
					}
				}
				for (int l = 0; l < yValues.Length; l++)
				{
					array2[l] = ConvertEnumerationItem(array[l].Current, null);
					if (array2[l] is DBNull || array2[l] == null)
					{
						flag4 = true;
						array2[l] = 0.0;
					}
				}
				if (flag4)
				{
					if (obj != null)
					{
						dataPoint.SetValueXY(obj, array2);
					}
					else
					{
						dataPoint.SetValueXY(0, array2);
					}
					DataPointInit(ref dataPoint);
					dataPoint.Empty = true;
					this.array.Add(dataPoint);
				}
				else
				{
					if (obj != null)
					{
						dataPoint.SetValueXY(obj, array2);
					}
					else
					{
						dataPoint.SetValueXY(0, array2);
					}
					DataPointInit(ref dataPoint);
					this.array.Add(dataPoint);
				}
			}
			while (flag || flag2);
			series.TraceWrite("ChartDataBinding", SR.TraceMessageSeriesPopulatedWithDataPoints(series.Name, this.array.Count.ToString(CultureInfo.CurrentCulture)));
		}

		public void DataBindY(IEnumerable yValue, string yFields)
		{
			DataBindXY(null, null, yValue, yFields);
		}

		public void DataBindXY(IEnumerable xValue, string xField, IEnumerable yValue, string yFields)
		{
			series.TraceWrite("ChartDataBinding", SR.TraceMessagePopulatingSeriesDataPoints(series.Name));
			string[] array = null;
			if (yFields != null)
			{
				array = yFields.Replace(",,", "\n").Split(',');
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = array[i].Replace("\n", ",");
				}
			}
			if (yValue == null)
			{
				throw new ArgumentNullException("yValue", SR.ExceptionDataPointInsertionYValueNotSpecified);
			}
			if (yValue is string)
			{
				throw new ArgumentException(SR.ExceptionDataBindYValuesToString, "yValue");
			}
			if (xValue is string)
			{
				throw new ArgumentException(SR.ExceptionDataBindXValuesToString, "xValue");
			}
			if (array == null || array.GetLength(0) > series.YValuesPerPoint)
			{
				throw new ArgumentOutOfRangeException("yValue", SR.ExceptionDataPointYValuesCountMismatch(series.YValuesPerPoint.ToString(CultureInfo.InvariantCulture)));
			}
			this.array.Clear();
			IEnumerator enumerator = null;
			IEnumerator dataSourceEnumerator = GetDataSourceEnumerator(yValue);
			if (dataSourceEnumerator.GetType() != typeof(DbEnumerator))
			{
				dataSourceEnumerator.Reset();
			}
			if (xValue != null)
			{
				if (xValue != yValue)
				{
					enumerator = GetDataSourceEnumerator(xValue);
					if (enumerator.GetType() != typeof(DbEnumerator))
					{
						enumerator.Reset();
					}
				}
				else
				{
					enumerator = dataSourceEnumerator;
				}
			}
			bool flag = false;
			bool flag2 = true;
			object[] array2 = new object[array.Length];
			object obj = null;
			bool flag3 = true;
			do
			{
				if (flag2)
				{
					flag2 = dataSourceEnumerator.MoveNext();
				}
				if (xValue != null)
				{
					if (xValue != yValue)
					{
						flag = enumerator.MoveNext();
						if (flag2 && !flag)
						{
							throw new ArgumentOutOfRangeException("xValue", SR.ExceptionDataPointInsertionXValuesQtyIsLessYValues);
						}
					}
					else
					{
						flag = flag2;
					}
				}
				if (flag3)
				{
					flag3 = false;
					AutoDetectValuesType(series, enumerator, xField, dataSourceEnumerator, array[0]);
				}
				if (!(flag || flag2))
				{
					continue;
				}
				DataPoint dataPoint = new DataPoint(series);
				bool flag4 = false;
				if (flag)
				{
					obj = ConvertEnumerationItem(enumerator.Current, xField);
					if (IsEmptyValue(obj))
					{
						flag4 = true;
						obj = 0.0;
					}
				}
				if (array.Length == 0)
				{
					array2[0] = ConvertEnumerationItem(dataSourceEnumerator.Current, null);
					if (IsEmptyValue(array2[0]))
					{
						flag4 = true;
						array2[0] = 0.0;
					}
				}
				else
				{
					for (int j = 0; j < array.Length; j++)
					{
						array2[j] = ConvertEnumerationItem(dataSourceEnumerator.Current, array[j]);
						if (IsEmptyValue(array2[j]))
						{
							flag4 = true;
							array2[j] = 0.0;
						}
					}
				}
				if (flag4)
				{
					if (obj != null)
					{
						dataPoint.SetValueXY(obj, array2);
					}
					else
					{
						dataPoint.SetValueXY(0, array2);
					}
					DataPointInit(ref dataPoint);
					dataPoint.Empty = true;
					this.array.Add(dataPoint);
				}
				else
				{
					if (obj != null)
					{
						dataPoint.SetValueXY(obj, array2);
					}
					else
					{
						dataPoint.SetValueXY(0, array2);
					}
					DataPointInit(ref dataPoint);
					this.array.Add(dataPoint);
				}
			}
			while (flag || flag2);
			series.TraceWrite("ChartDataBinding", SR.TraceMessageSeriesPopulatedWithDataPoints(series.Name, this.array.Count.ToString(CultureInfo.CurrentCulture)));
		}

		internal static bool IsEmptyValue(object val)
		{
			if (val is DBNull || val == null)
			{
				return true;
			}
			if (val is double && double.IsNaN((double)val))
			{
				return true;
			}
			if (val is float && float.IsNaN((float)val))
			{
				return true;
			}
			return false;
		}

		public int AddY(double yValue)
		{
			DataPoint dataPoint = new DataPoint(series);
			dataPoint.SetValueY(yValue);
			DataPointInit(ref dataPoint);
			return array.Add(dataPoint);
		}

		public int AddY(params object[] yValue)
		{
			if (series.YValueType == ChartValueTypes.Auto && yValue.Length != 0 && yValue[0] != null)
			{
				if (yValue[0] is DateTime)
				{
					series.YValueType = ChartValueTypes.DateTime;
					series.autoYValueType = true;
				}
				else if (yValue[0] is DateTimeOffset)
				{
					series.YValueType = ChartValueTypes.DateTimeOffset;
					series.autoYValueType = true;
				}
			}
			DataPoint dataPoint = new DataPoint(series);
			dataPoint.SetValueY(yValue);
			DataPointInit(ref dataPoint);
			return array.Add(dataPoint);
		}

		public int AddXY(double xValue, double yValue)
		{
			DataPoint dataPoint = new DataPoint(series);
			dataPoint.SetValueXY(xValue, yValue);
			DataPointInit(ref dataPoint);
			return array.Add(dataPoint);
		}

		public int AddXY(object xValue, params object[] yValue)
		{
			if (series.XValueType == ChartValueTypes.Auto)
			{
				if (xValue is DateTime)
				{
					series.XValueType = ChartValueTypes.DateTime;
				}
				if (xValue is DateTimeOffset)
				{
					series.XValueType = ChartValueTypes.DateTimeOffset;
				}
				if (xValue is string)
				{
					series.XValueType = ChartValueTypes.String;
				}
				series.autoXValueType = true;
			}
			if (series.YValueType == ChartValueTypes.Auto && yValue.Length != 0 && yValue[0] != null)
			{
				if (yValue[0] is DateTime)
				{
					series.YValueType = ChartValueTypes.DateTime;
					series.autoYValueType = true;
				}
				else if (yValue[0] is DateTimeOffset)
				{
					series.YValueType = ChartValueTypes.DateTimeOffset;
					series.autoYValueType = true;
				}
			}
			DataPoint dataPoint = new DataPoint(series);
			dataPoint.SetValueXY(xValue, yValue);
			DataPointInit(ref dataPoint);
			return array.Add(dataPoint);
		}

		public void InsertXY(int index, object xValue, params object[] yValue)
		{
			DataPoint dataPoint = new DataPoint(series);
			dataPoint.SetValueXY(xValue, yValue);
			DataPointInit(ref dataPoint);
			array.Insert(index, dataPoint);
		}

		public void InsertY(int index, params object[] yValue)
		{
			DataPoint dataPoint = new DataPoint(series);
			dataPoint.SetValueY(yValue);
			DataPointInit(ref dataPoint);
			array.Insert(index, dataPoint);
		}

		internal static IEnumerator GetDataSourceEnumerator(IEnumerable dataSource)
		{
			if (dataSource is DataView)
			{
				return ((DataView)dataSource).GetEnumerator();
			}
			if (dataSource is DataSet)
			{
				DataSet dataSet = (DataSet)dataSource;
				if (dataSet.Tables.Count > 0)
				{
					return dataSet.Tables[0].Rows.GetEnumerator();
				}
			}
			return dataSource.GetEnumerator();
		}

		internal static object ConvertEnumerationItem(object item, string fieldName)
		{
			object result = item;
			if (item is DataRow)
			{
				if (fieldName != null && fieldName.Length > 0)
				{
					bool flag = true;
					if (((DataRow)item).Table.Columns.Contains(fieldName))
					{
						result = ((DataRow)item)[fieldName];
						flag = false;
					}
					else
					{
						try
						{
							int num = int.Parse(fieldName, CultureInfo.InvariantCulture);
							if (num < ((DataRow)item).Table.Columns.Count && num >= 0)
							{
								result = ((DataRow)item)[num];
								flag = false;
							}
						}
						catch
						{
						}
					}
					if (flag)
					{
						throw new ArgumentException(SR.ExceptionColumnNameNotFound(fieldName));
					}
				}
				else
				{
					result = ((DataRow)item)[0];
				}
			}
			else if (item is DataRowView)
			{
				if (fieldName != null && fieldName.Length > 0)
				{
					bool flag2 = true;
					if (((DataRowView)item).DataView.Table.Columns.Contains(fieldName))
					{
						result = ((DataRowView)item)[fieldName];
						flag2 = false;
					}
					else
					{
						try
						{
							int num2 = int.Parse(fieldName, CultureInfo.InvariantCulture);
							if (num2 < ((DataRowView)item).DataView.Table.Columns.Count && num2 >= 0)
							{
								result = ((DataRowView)item)[num2];
								flag2 = false;
							}
						}
						catch
						{
						}
					}
					if (flag2)
					{
						throw new ArgumentException(SR.ExceptionColumnNameNotFound(fieldName));
					}
				}
				else
				{
					result = ((DataRowView)item)[0];
				}
			}
			else if (item is DbDataRecord)
			{
				if (fieldName != null && fieldName.Length > 0)
				{
					bool flag3 = true;
					if (!char.IsNumber(fieldName, 0))
					{
						try
						{
							result = ((DbDataRecord)item)[fieldName];
							flag3 = false;
						}
						catch (Exception)
						{
						}
					}
					if (flag3)
					{
						try
						{
							int i = int.Parse(fieldName, CultureInfo.InvariantCulture);
							result = ((DbDataRecord)item)[i];
							flag3 = false;
						}
						catch
						{
						}
					}
					if (flag3)
					{
						throw new ArgumentException(SR.ExceptionColumnNameNotFound(fieldName));
					}
				}
				else
				{
					result = ((DbDataRecord)item)[0];
				}
			}
			else if (fieldName != null && fieldName.Length > 0)
			{
				PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(item).Find(fieldName, ignoreCase: true);
				if (propertyDescriptor != null)
				{
					result = propertyDescriptor.GetValue(item);
					return result ?? null;
				}
			}
			return result;
		}

		internal static void AutoDetectValuesType(Series series, IEnumerator xEnumerator, string xField, IEnumerator yEnumerator, string yField)
		{
			if (series.XValueType == ChartValueTypes.Auto)
			{
				series.XValueType = GetValueType(xEnumerator, xField);
				if (series.XValueType != 0)
				{
					series.autoXValueType = true;
				}
			}
			if (series.YValueType == ChartValueTypes.Auto)
			{
				series.YValueType = GetValueType(yEnumerator, yField);
				if (series.YValueType != 0)
				{
					series.autoYValueType = true;
				}
			}
		}

		private static ChartValueTypes GetValueType(IEnumerator enumerator, string field)
		{
			ChartValueTypes result = ChartValueTypes.Auto;
			Type left = null;
			if (enumerator == null)
			{
				return result;
			}
			try
			{
				if (enumerator.Current == null)
				{
					return result;
				}
			}
			catch (Exception)
			{
				return result;
			}
			if (enumerator.Current is DataRow)
			{
				if (field != null && field.Length > 0)
				{
					bool flag = true;
					if (((DataRow)enumerator.Current).Table.Columns.Contains(field))
					{
						left = ((DataRow)enumerator.Current).Table.Columns[field].DataType;
						flag = false;
					}
					if (flag)
					{
						try
						{
							int index = int.Parse(field, CultureInfo.InvariantCulture);
							left = ((DataRow)enumerator.Current).Table.Columns[index].DataType;
							flag = false;
						}
						catch
						{
						}
					}
					if (flag)
					{
						throw new ArgumentException(SR.ExceptionColumnNameNotFound(field));
					}
				}
				else if (((DataRow)enumerator.Current).Table.Columns.Count > 0)
				{
					left = ((DataRow)enumerator.Current).Table.Columns[0].DataType;
				}
			}
			else if (enumerator.Current is DataRowView)
			{
				if (field != null && field.Length > 0)
				{
					bool flag2 = true;
					if (((DataRowView)enumerator.Current).DataView.Table.Columns.Contains(field))
					{
						left = ((DataRowView)enumerator.Current).DataView.Table.Columns[field].DataType;
						flag2 = false;
					}
					if (flag2)
					{
						try
						{
							int index2 = int.Parse(field, CultureInfo.InvariantCulture);
							left = ((DataRowView)enumerator.Current).DataView.Table.Columns[index2].DataType;
							flag2 = false;
						}
						catch
						{
						}
					}
					if (flag2)
					{
						throw new ArgumentException(SR.ExceptionColumnNameNotFound(field));
					}
				}
				else if (((DataRowView)enumerator.Current).DataView.Table.Columns.Count > 0)
				{
					left = ((DataRowView)enumerator.Current).DataView.Table.Columns[0].DataType;
				}
			}
			else if (enumerator.Current is DbDataRecord)
			{
				if (field != null && field.Length > 0)
				{
					bool flag3 = true;
					int num = 0;
					if (!char.IsNumber(field, 0))
					{
						try
						{
							num = ((DbDataRecord)enumerator.Current).GetOrdinal(field);
							left = ((DbDataRecord)enumerator.Current).GetFieldType(num);
							flag3 = false;
						}
						catch
						{
						}
					}
					if (flag3)
					{
						try
						{
							num = int.Parse(field, CultureInfo.InvariantCulture);
							left = ((DbDataRecord)enumerator.Current).GetFieldType(num);
							flag3 = false;
						}
						catch
						{
						}
					}
					if (flag3)
					{
						throw new ArgumentException(SR.ExceptionColumnNameNotFound(field));
					}
				}
				else if (((DbDataRecord)enumerator.Current).FieldCount > 0)
				{
					left = ((DbDataRecord)enumerator.Current).GetFieldType(0);
				}
			}
			else
			{
				if (field != null && field.Length > 0)
				{
					PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(enumerator.Current).Find(field, ignoreCase: true);
					if (propertyDescriptor != null)
					{
						left = propertyDescriptor.PropertyType;
					}
				}
				if (left == null)
				{
					left = enumerator.Current.GetType();
				}
			}
			if (left != null)
			{
				if (left == typeof(DateTime))
				{
					result = ChartValueTypes.DateTime;
				}
				else if (left == typeof(DateTimeOffset))
				{
					result = ChartValueTypes.DateTimeOffset;
				}
				else if (left == typeof(TimeSpan))
				{
					result = ChartValueTypes.Time;
				}
				else if (left == typeof(double))
				{
					result = ChartValueTypes.Double;
				}
				else if (left == typeof(int))
				{
					result = ChartValueTypes.Int;
				}
				else if (left == typeof(long))
				{
					result = ChartValueTypes.Long;
				}
				else if (left == typeof(float))
				{
					result = ChartValueTypes.Single;
				}
				else if (left == typeof(string))
				{
					result = ChartValueTypes.String;
				}
				else if (left == typeof(uint))
				{
					result = ChartValueTypes.UInt;
				}
				else if (left == typeof(ulong))
				{
					result = ChartValueTypes.ULong;
				}
			}
			return result;
		}

		public DataPoint FindValue(double valueToFind, string useValue, ref int startFromIndex)
		{
			while (startFromIndex < Count)
			{
				if (this[startFromIndex].GetValueByName(useValue) == valueToFind)
				{
					return this[startFromIndex];
				}
				startFromIndex++;
			}
			startFromIndex = -1;
			return null;
		}

		public DataPoint FindValue(double valueToFind, string useValue)
		{
			int startFromIndex = 0;
			return FindValue(valueToFind, useValue, ref startFromIndex);
		}

		public DataPoint FindValue(double valueToFind)
		{
			int startFromIndex = 0;
			return FindValue(valueToFind, "Y", ref startFromIndex);
		}

		public DataPoint FindMaxValue(string useValue, ref int startFromIndex)
		{
			double num = double.MinValue;
			int num2 = -1;
			for (num2 = 0; num2 < Count; num2++)
			{
				if ((!this[num2].Empty || !useValue.StartsWith("Y", StringComparison.OrdinalIgnoreCase)) && num < this[num2].GetValueByName(useValue))
				{
					num = this[num2].GetValueByName(useValue);
				}
			}
			for (num2 = startFromIndex; num2 < Count; num2++)
			{
				if (this[num2].GetValueByName(useValue) == num)
				{
					startFromIndex = num2;
					return this[num2];
				}
			}
			startFromIndex = -1;
			return null;
		}

		public DataPoint FindMaxValue(string useValue)
		{
			int startFromIndex = 0;
			return FindMaxValue(useValue, ref startFromIndex);
		}

		public DataPoint FindMaxValue()
		{
			int startFromIndex = 0;
			return FindMaxValue("Y", ref startFromIndex);
		}

		public DataPoint FindMinValue(string useValue, ref int startFromIndex)
		{
			double num = double.MaxValue;
			int num2 = -1;
			for (num2 = 0; num2 < Count; num2++)
			{
				if ((!this[num2].Empty || !useValue.StartsWith("Y", StringComparison.OrdinalIgnoreCase)) && num > this[num2].GetValueByName(useValue))
				{
					num = this[num2].GetValueByName(useValue);
				}
			}
			for (num2 = startFromIndex; num2 < Count; num2++)
			{
				if (this[num2].GetValueByName(useValue) == num)
				{
					startFromIndex = num2;
					return this[num2];
				}
			}
			startFromIndex = -1;
			return null;
		}

		public DataPoint FindMinValue(string useValue)
		{
			int startFromIndex = 0;
			return FindMinValue(useValue, ref startFromIndex);
		}

		public DataPoint FindMinValue()
		{
			int startFromIndex = 0;
			return FindMinValue("Y", ref startFromIndex);
		}

		public void Clear()
		{
			array.Clear();
		}

		bool IList.Contains(object value)
		{
			return array.Contains(value);
		}

		public bool Contains(DataPoint value)
		{
			return array.Contains(value);
		}

		int IList.IndexOf(object value)
		{
			return array.IndexOf(value);
		}

		public int IndexOf(DataPoint value)
		{
			return array.IndexOf(value);
		}

		void IList.Remove(object value)
		{
			array.Remove(value);
		}

		public void Remove(DataPoint value)
		{
			array.Remove(value);
		}

		public void RemoveAt(int index)
		{
			array.RemoveAt(index);
		}

		public void RemoveRange(int index, int count)
		{
			array.RemoveRange(index, count);
		}

		public int Add(DataPoint value)
		{
			return Add((object)value);
		}

		public int Add(object value)
		{
			if (value is DataPoint)
			{
				DataPoint dataPoint = (DataPoint)value;
				DataPointInit(ref dataPoint);
				return array.Add(value);
			}
			DataPoint dataPoint2 = new DataPoint(series);
			DataPointInit(ref dataPoint2);
			dataPoint2.SetValueY(value);
			return array.Add(dataPoint2);
		}

		public void Insert(int index, DataPoint value)
		{
			Insert(index, (object)value);
		}

		public void Insert(int index, object value)
		{
			if (value is DataPoint)
			{
				DataPoint dataPoint = (DataPoint)value;
				DataPointInit(ref dataPoint);
				array.Insert(index, value);
			}
			else
			{
				DataPoint dataPoint2 = new DataPoint(series);
				DataPointInit(ref dataPoint2);
				dataPoint2.SetValueY(value);
				array.Insert(index, dataPoint2);
			}
		}

		public void CopyTo(Array array, int index)
		{
			this.array.CopyTo(array, index);
		}

		public IEnumerator GetEnumerator()
		{
			return array.GetEnumerator();
		}
	}
}
