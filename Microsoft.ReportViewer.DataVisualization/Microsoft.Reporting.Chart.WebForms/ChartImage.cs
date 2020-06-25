using Microsoft.Reporting.Chart.WebForms.Utilities;
using Microsoft.Win32;
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Permissions;
using System.Xml;

namespace Microsoft.Reporting.Chart.WebForms
{
	internal class ChartImage : ChartPicture
	{
		private int compression;

		private object dataSource;

		internal bool boundToDataSource;

		private ChartImageType imageType = ChartImageType.Png;

		[SRCategory("CategoryAttributeData")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeDataSource")]
		[DefaultValue(null)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public object DataSource
		{
			get
			{
				return dataSource;
			}
			set
			{
				if (dataSource != value)
				{
					dataSource = value;
					boundToDataSource = false;
				}
			}
		}

		[SRCategory("CategoryAttributeImage")]
		[Bindable(true)]
		[DefaultValue(ChartImageType.Png)]
		[SRDescription("DescriptionAttributeImageType")]
		public ChartImageType ImageType
		{
			get
			{
				return imageType;
			}
			set
			{
				imageType = value;
			}
		}

		[SRCategory("CategoryAttributeImage")]
		[Bindable(true)]
		[DefaultValue(0)]
		[SRDescription("DescriptionAttributeChartImage_Compression")]
		public int Compression
		{
			get
			{
				return compression;
			}
			set
			{
				if (value < 0 || value > 100)
				{
					throw new ArgumentOutOfRangeException("value", SR.ExceptionChartCompressionInvalid);
				}
				compression = value;
			}
		}

		public ChartImage(IServiceContainer container)
			: base(container)
		{
		}

		public void SaveIntoMetafile(Stream imageStream, EmfType emfType)
		{
			Bitmap bitmap = new Bitmap(base.Width, base.Height);
			Graphics graphics = Graphics.FromImage(bitmap);
			IntPtr hdc = graphics.GetHdc();
			Metafile metafile = new Metafile(imageStream, hdc, new Rectangle(0, 0, base.Width, base.Height), MetafileFrameUnit.Pixel, emfType);
			Graphics graphics2 = Graphics.FromImage(metafile);
			if (base.BorderSkinAttributes.SkinStyle != 0)
			{
				graphics2.Clip = new Region(new Rectangle(0, 0, base.Width, base.Height));
			}
			chartGraph.IsMetafile = true;
			Paint(graphics2, paintTopLevelElementOnly: false);
			chartGraph.IsMetafile = false;
			byte[] data = new byte[12]
			{
				68,
				117,
				110,
				100,
				97,
				115,
				32,
				67,
				104,
				97,
				114,
				116
			};
			graphics2.AddMetafileComment(data);
			graphics2.Dispose();
			metafile.Dispose();
			graphics.ReleaseHdc(hdc);
			graphics.Dispose();
			bitmap.Dispose();
		}

		public Bitmap GetImage(float resolution)
		{
			Bitmap bitmap = null;
			if (base.Width == 0 || base.Height == 0)
			{
				bitmap = new Bitmap(Math.Max(1, base.Width), Math.Max(1, base.Height));
				bitmap.SetResolution(resolution, resolution);
			}
			else
			{
				while (bitmap == null)
				{
					try
					{
						bitmap = new Bitmap(base.Width, base.Height);
						bitmap.SetResolution(resolution, resolution);
					}
					catch
					{
						bitmap = null;
						float num = Math.Max(resolution / 2f, 96f);
						base.Width = (int)Math.Ceiling((float)base.Width * num / resolution);
						base.Height = (int)Math.Ceiling((float)base.Height * num / resolution);
						resolution = num;
					}
				}
			}
			Graphics graphics = Graphics.FromImage(bitmap);
			Color color = (!(base.BackColor != Color.Empty)) ? Color.White : base.BackColor;
			Pen pen = new Pen(color);
			graphics.DrawRectangle(pen, 0, 0, base.Width, base.Height);
			pen.Dispose();
			Paint(graphics, paintTopLevelElementOnly: false);
			graphics.Dispose();
			return bitmap;
		}

		public void GetSvgImage(XmlTextWriter svgTextWriter, string documentTitle, bool resizable, bool preserveAspectRatio)
		{
			bool softShadows = base.SoftShadows;
			base.SoftShadows = false;
			using (Bitmap image = new Bitmap(base.Width, base.Height))
			{
				using (Graphics graph = Graphics.FromImage(image))
				{
					Paint(graph, paintTopLevelElementOnly: false, RenderingType.Svg, svgTextWriter, null, documentTitle, resizable, preserveAspectRatio);
				}
			}
			base.SoftShadows = softShadows;
		}

		internal static bool IsValidDataSource(object dataSource)
		{
			if (dataSource is IEnumerable || dataSource is DataSet || dataSource is DataView || dataSource is DataTable || dataSource is OleDbCommand || dataSource is SqlCommand || dataSource is OleDbDataAdapter || dataSource is SqlDataAdapter || dataSource.GetType().GetInterface("IDataSource") != null)
			{
				return true;
			}
			return false;
		}

		internal static ArrayList GetDataSourceMemberNames(object dataSource, bool usedForYValue)
		{
			ArrayList arrayList = new ArrayList();
			if (dataSource != null)
			{
				try
				{
					if (dataSource.GetType().GetInterface("IDataSource") != null)
					{
						try
						{
							MethodInfo method = dataSource.GetType().GetMethod("Select");
							if (method != null)
							{
								if (method.GetParameters().Length == 1)
								{
									ConstructorInfo constructor = dataSource.GetType().Assembly.GetType("System.Web.UI.DataSourceSelectArguments", throwOnError: true).GetConstructor(new Type[0]);
									dataSource = method.Invoke(dataSource, new object[1]
									{
										constructor.Invoke(new object[0])
									});
								}
								else
								{
									dataSource = method.Invoke(dataSource, new object[0]);
								}
							}
						}
						catch
						{
						}
					}
					DataTable dataTable = null;
					if (dataSource is DataTable)
					{
						dataTable = (DataTable)dataSource;
					}
					else if (dataSource is DataView)
					{
						dataTable = ((DataView)dataSource).Table;
					}
					else if (dataSource is DataSet && ((DataSet)dataSource).Tables.Count > 0)
					{
						dataTable = ((DataSet)dataSource).Tables[0];
					}
					else if (dataSource is OleDbDataAdapter)
					{
						dataTable = new DataTable();
						dataTable.Locale = CultureInfo.CurrentCulture;
						dataTable = ((OleDbDataAdapter)dataSource).FillSchema(dataTable, SchemaType.Mapped);
					}
					else if (dataSource is SqlDataAdapter)
					{
						dataTable = new DataTable();
						dataTable.Locale = CultureInfo.CurrentCulture;
						dataTable = ((SqlDataAdapter)dataSource).FillSchema(dataTable, SchemaType.Mapped);
					}
					else if (dataSource is OleDbDataReader)
					{
						for (int i = 0; i < ((OleDbDataReader)dataSource).FieldCount; i++)
						{
							if (!usedForYValue || ((OleDbDataReader)dataSource).GetFieldType(i) != typeof(string))
							{
								arrayList.Add(((OleDbDataReader)dataSource).GetName(i));
							}
						}
					}
					else if (dataSource is SqlDataReader)
					{
						for (int j = 0; j < ((SqlDataReader)dataSource).FieldCount; j++)
						{
							if (!usedForYValue || ((SqlDataReader)dataSource).GetFieldType(j) != typeof(string))
							{
								arrayList.Add(((SqlDataReader)dataSource).GetName(j));
							}
						}
					}
					else if (dataSource is OleDbCommand)
					{
						OleDbCommand oleDbCommand = (OleDbCommand)dataSource;
						if (oleDbCommand.Connection != null)
						{
							oleDbCommand.Connection.Open();
							OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
							if (oleDbDataReader.Read())
							{
								for (int k = 0; k < oleDbDataReader.FieldCount; k++)
								{
									if (!usedForYValue || oleDbDataReader.GetFieldType(k) != typeof(string))
									{
										arrayList.Add(oleDbDataReader.GetName(k));
									}
								}
							}
							oleDbDataReader.Close();
							oleDbCommand.Connection.Close();
						}
					}
					else if (dataSource is SqlCommand)
					{
						SqlCommand sqlCommand = (SqlCommand)dataSource;
						if (sqlCommand.Connection != null)
						{
							sqlCommand.Connection.Open();
							SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
							if (sqlDataReader.Read())
							{
								for (int l = 0; l < sqlDataReader.FieldCount; l++)
								{
									if (!usedForYValue || sqlDataReader.GetFieldType(l) != typeof(string))
									{
										arrayList.Add(sqlDataReader.GetName(l));
									}
								}
							}
							sqlDataReader.Close();
							sqlCommand.Connection.Close();
						}
					}
					if (dataTable != null)
					{
						foreach (DataColumn column in dataTable.Columns)
						{
							if (!usedForYValue || column.DataType != typeof(string))
							{
								arrayList.Add(column.ColumnName);
							}
						}
					}
					else if (arrayList.Count == 0 && dataSource is ITypedList)
					{
						foreach (PropertyDescriptor itemProperty in ((ITypedList)dataSource).GetItemProperties(null))
						{
							if (!usedForYValue || itemProperty.PropertyType != typeof(string))
							{
								arrayList.Add(itemProperty.Name);
							}
						}
					}
					else if (arrayList.Count == 0 && dataSource is IEnumerable)
					{
						IEnumerator enumerator2 = ((IEnumerable)dataSource).GetEnumerator();
						enumerator2.Reset();
						enumerator2.MoveNext();
						foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(enumerator2.Current))
						{
							if (!usedForYValue || property.PropertyType != typeof(string))
							{
								arrayList.Add(property.Name);
							}
						}
					}
				}
				catch
				{
				}
				if (arrayList.Count == 0)
				{
					arrayList.Add("0");
				}
			}
			return arrayList;
		}

		internal void DataBind()
		{
			boundToDataSource = true;
			object selectCommand = DataSource;
			if (selectCommand == null)
			{
				return;
			}
			if (selectCommand is OleDbDataAdapter)
			{
				selectCommand = ((OleDbDataAdapter)selectCommand).SelectCommand;
			}
			else if (selectCommand is SqlDataAdapter)
			{
				selectCommand = ((SqlDataAdapter)selectCommand).SelectCommand;
			}
			if (selectCommand is DataSet && ((DataSet)selectCommand).Tables.Count > 0)
			{
				selectCommand = ((DataSet)selectCommand).DefaultViewManager.CreateDataView(((DataSet)selectCommand).Tables[0]);
			}
			else if (selectCommand is DataTable)
			{
				selectCommand = new DataView((DataTable)selectCommand);
			}
			else
			{
				if (selectCommand is OleDbCommand)
				{
					OleDbCommand obj = (OleDbCommand)selectCommand;
					obj.Connection.Open();
					OleDbDataReader oleDbDataReader = obj.ExecuteReader();
					DataBind(oleDbDataReader, null);
					oleDbDataReader.Close();
					obj.Connection.Close();
					return;
				}
				if (selectCommand is SqlCommand)
				{
					SqlCommand obj2 = (SqlCommand)selectCommand;
					obj2.Connection.Open();
					SqlDataReader sqlDataReader = obj2.ExecuteReader();
					DataBind(sqlDataReader, null);
					sqlDataReader.Close();
					obj2.Connection.Close();
					return;
				}
				selectCommand = ((!(selectCommand is IList)) ? (selectCommand as IEnumerable) : (selectCommand as IList));
			}
			DataBind(selectCommand as IEnumerable, null);
		}

		internal void DataBind(IEnumerable dataSource, ArrayList seriesList)
		{
			if (dataSource == null || common == null)
			{
				return;
			}
			if (seriesList == null)
			{
				seriesList = new ArrayList();
				foreach (Series item in common.Chart.Series)
				{
					if (common.Chart.IsDesignMode())
					{
						if (item.ValueMembersY.Length > 0)
						{
							seriesList.Add(item);
						}
					}
					else
					{
						seriesList.Add(item);
					}
				}
			}
			foreach (Series series4 in seriesList)
			{
				if (series4.ValueMemberX.Length > 0 || series4.ValueMembersY.Length > 0)
				{
					series4.Points.Clear();
				}
			}
			IEnumerator enumerator2 = dataSource.GetEnumerator();
			if (enumerator2.GetType() != typeof(DbEnumerator))
			{
				try
				{
					enumerator2.Reset();
				}
				catch
				{
				}
			}
			bool flag = true;
			bool flag2 = true;
			do
			{
				flag = enumerator2.MoveNext();
				foreach (Series series5 in seriesList)
				{
					if (series5.ValueMemberX.Length <= 0 && series5.ValueMembersY.Length <= 0)
					{
						continue;
					}
					string[] array = null;
					if (series5.ValueMembersY.Length > 0)
					{
						array = series5.ValueMembersY.Replace(",,", "\n").Split(',');
						for (int i = 0; i < array.Length; i++)
						{
							array[i] = array[i].Replace("\n", ",").Trim();
						}
					}
					if (dataSource is string)
					{
						throw new ArgumentException(SR.ExceptionDataBindYValuesToString, "dataSource");
					}
					if (array == null || array.GetLength(0) > series5.YValuesPerPoint)
					{
						throw new ArgumentOutOfRangeException("yValue", SR.ExceptionDataPointYValuesCountMismatch(series5.YValuesPerPoint.ToString(CultureInfo.InvariantCulture)));
					}
					if (!flag)
					{
						continue;
					}
					if (flag2)
					{
						flag2 = false;
						string text = array[0];
						int num = 1;
						while (text.Length == 0 && num < array.Length)
						{
							text = array[num++];
						}
						DataPointCollection.AutoDetectValuesType(series5, enumerator2, series5.ValueMemberX.Trim(), enumerator2, text);
					}
					DataPoint dataPoint = new DataPoint(series5);
					bool flag3 = false;
					bool flag4 = false;
					object[] array2 = new object[array.Length];
					object obj2 = null;
					if (series5.ValueMemberX.Length > 0)
					{
						obj2 = DataPointCollection.ConvertEnumerationItem(enumerator2.Current, series5.ValueMemberX.Trim());
						if (obj2 is DBNull || obj2 == null)
						{
							flag4 = true;
							flag3 = true;
							obj2 = 0.0;
						}
					}
					if (array.Length == 0)
					{
						array2[0] = DataPointCollection.ConvertEnumerationItem(enumerator2.Current, null);
						if (array2[0] is DBNull || array2[0] == null)
						{
							flag3 = true;
							array2[0] = 0.0;
						}
					}
					else
					{
						for (int j = 0; j < array.Length; j++)
						{
							if (array[j].Length > 0)
							{
								array2[j] = DataPointCollection.ConvertEnumerationItem(enumerator2.Current, array[j]);
								if (array2[j] is DBNull || array2[j] == null)
								{
									flag3 = true;
									array2[j] = 0.0;
								}
							}
							else
							{
								array2[j] = (((Series)seriesList[0]).IsYValueDateTime() ? DateTime.Now.Date.ToOADate() : 0.0);
							}
						}
					}
					if (!flag4)
					{
						if (flag3)
						{
							if (obj2 != null)
							{
								dataPoint.SetValueXY(obj2, array2);
							}
							else
							{
								dataPoint.SetValueXY(0, array2);
							}
							series5.Points.DataPointInit(ref dataPoint);
							dataPoint.Empty = true;
							series5.Points.Add(dataPoint);
						}
						else
						{
							if (obj2 != null)
							{
								dataPoint.SetValueXY(obj2, array2);
							}
							else
							{
								dataPoint.SetValueXY(0, array2);
							}
							series5.Points.DataPointInit(ref dataPoint);
							series5.Points.Add(dataPoint);
						}
					}
					if (common.Chart.IsDesignMode())
					{
						series5["TempDesignData"] = "true";
					}
				}
			}
			while (flag);
		}

		internal void AlignDataPointsByAxisLabel(bool sortAxisLabels, PointsSortOrder sortingOrder)
		{
			string text = string.Empty;
			foreach (ChartArea chartArea3 in base.ChartAreas)
			{
				if (text.Length == 0)
				{
					text = chartArea3.Name;
				}
				if (chartArea3.Name == "Default")
				{
					text = chartArea3.Name;
					break;
				}
			}
			foreach (ChartArea chartArea4 in base.ChartAreas)
			{
				if (!chartArea4.Visible)
				{
					continue;
				}
				ArrayList arrayList = new ArrayList();
				ArrayList arrayList2 = new ArrayList();
				foreach (Series item in common.Chart.Series)
				{
					if ((item.ChartArea == chartArea4.Name || (item.ChartArea == "Default" && chartArea4.Name == text)) && item.XSubAxisName.Length == 0)
					{
						if (item.XAxisType == AxisType.Primary)
						{
							arrayList.Add(item);
						}
						else
						{
							arrayList2.Add(item);
						}
					}
				}
				AlignDataPointsByAxisLabel(arrayList, sortAxisLabels, sortingOrder);
				AlignDataPointsByAxisLabel(arrayList2, sortAxisLabels, sortingOrder);
			}
		}

		internal void AlignDataPointsByAxisLabel(ArrayList seriesList, bool sortAxisLabels, PointsSortOrder sortingOrder)
		{
			if (seriesList.Count == 0)
			{
				return;
			}
			bool flag = true;
			bool flag2 = true;
			ArrayList arrayList = new ArrayList();
			foreach (Series series4 in seriesList)
			{
				ArrayList arrayList2 = new ArrayList();
				foreach (DataPoint point in series4.Points)
				{
					if (!series4.XValueIndexed && point.XValue != 0.0)
					{
						flag = false;
						break;
					}
					if (point.AxisLabel.Length == 0)
					{
						flag2 = false;
						break;
					}
					if (arrayList2.Contains(point.AxisLabel))
					{
						flag2 = false;
						break;
					}
					if (!arrayList.Contains(point.AxisLabel))
					{
						arrayList.Add(point.AxisLabel);
					}
					arrayList2.Add(point.AxisLabel);
				}
			}
			if (sortAxisLabels)
			{
				arrayList.Sort();
				if (sortingOrder == PointsSortOrder.Descending)
				{
					arrayList.Reverse();
				}
			}
			if (!flag)
			{
				throw new InvalidOperationException(SR.ExceptionChartDataPointsAlignmentFaild);
			}
			if (!flag2)
			{
				throw new InvalidOperationException(SR.ExceptionChartDataPointsAlignmentFaildAxisLabelsInvalid);
			}
			if (!(flag && flag2))
			{
				return;
			}
			foreach (Series series5 in seriesList)
			{
				foreach (DataPoint point2 in series5.Points)
				{
					point2.XValue = arrayList.IndexOf(point2.AxisLabel) + 1;
				}
				series5.Sort(PointsSortOrder.Ascending, "X");
			}
			foreach (Series series6 in seriesList)
			{
				series6.XValueIndexed = true;
				for (int i = 0; i < arrayList.Count; i++)
				{
					if (i >= series6.Points.Count || series6.Points[i].XValue != (double)(i + 1))
					{
						DataPoint dataPoint3 = new DataPoint(series6);
						dataPoint3.AxisLabel = (string)arrayList[i];
						dataPoint3.XValue = i + 1;
						dataPoint3.YValues[0] = 0.0;
						dataPoint3.Empty = true;
						series6.Points.Insert(i, dataPoint3);
					}
				}
			}
		}

		internal void DataBindCrossTab(IEnumerable dataSource, string seriesGroupByField, string xField, string yFields, string otherFields, bool sort, PointsSortOrder sortingOrder)
		{
			ArrayList arrayList = new ArrayList();
			ArrayList arrayList2 = new ArrayList();
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
			DataPointCollection.ParsePointFieldsParameter(otherFields, ref otherAttributeNames, ref otherFieldNames, ref otherValueFormat);
			if (dataSource == null)
			{
				throw new ArgumentNullException("dataSource", SR.ExceptionDataPointInsertionNoDataSource);
			}
			if (dataSource is string)
			{
				throw new ArgumentException(SR.ExceptionDataBindSeriesToString, "dataSource");
			}
			if (array == null || array.Length == 0)
			{
				throw new ArgumentOutOfRangeException("yFields", SR.ExceptionChartDataPointsInsertionFailedYValuesEmpty);
			}
			if (seriesGroupByField == null || seriesGroupByField.Length == 0)
			{
				throw new ArgumentException(SR.ExceptionDataBindSeriesGroupByParameterIsEmpty, "seriesGroupByField");
			}
			IEnumerator dataSourceEnumerator = DataPointCollection.GetDataSourceEnumerator(dataSource);
			if (dataSourceEnumerator.GetType() != typeof(DbEnumerator))
			{
				try
				{
					dataSourceEnumerator.Reset();
				}
				catch
				{
				}
			}
			bool flag = true;
			object[] array2 = new object[array.Length];
			object obj2 = null;
			bool flag2 = true;
			do
			{
				if (flag)
				{
					flag = dataSourceEnumerator.MoveNext();
				}
				if (!flag)
				{
					continue;
				}
				object obj3 = DataPointCollection.ConvertEnumerationItem(dataSourceEnumerator.Current, seriesGroupByField);
				Series series = null;
				int num = arrayList2.IndexOf(obj3);
				if (num >= 0)
				{
					series = (Series)arrayList[num];
				}
				else
				{
					series = new Series();
					series.YValuesPerPoint = array.GetLength(0);
					if (arrayList.Count > 0)
					{
						series.XValueType = ((Series)arrayList[0]).XValueType;
						series.autoXValueType = ((Series)arrayList[0]).autoXValueType;
						series.YValueType = ((Series)arrayList[0]).YValueType;
						series.autoYValueType = ((Series)arrayList[0]).autoYValueType;
					}
					if (obj3 is string)
					{
						series.Name = (string)obj3;
					}
					else
					{
						series.Name = seriesGroupByField + " - " + obj3.ToString();
					}
					arrayList2.Add(obj3);
					arrayList.Add(series);
				}
				if (flag2)
				{
					flag2 = false;
					DataPointCollection.AutoDetectValuesType(series, dataSourceEnumerator, xField, dataSourceEnumerator, array[0]);
				}
				DataPoint dataPoint = new DataPoint(series);
				bool flag3 = false;
				if (xField.Length > 0)
				{
					obj2 = DataPointCollection.ConvertEnumerationItem(dataSourceEnumerator.Current, xField);
					if (DataPointCollection.IsEmptyValue(obj2))
					{
						flag3 = true;
						obj2 = 0.0;
					}
				}
				if (array.Length == 0)
				{
					array2[0] = DataPointCollection.ConvertEnumerationItem(dataSourceEnumerator.Current, null);
					if (DataPointCollection.IsEmptyValue(array2[0]))
					{
						flag3 = true;
						array2[0] = 0.0;
					}
				}
				else
				{
					for (int j = 0; j < array.Length; j++)
					{
						array2[j] = DataPointCollection.ConvertEnumerationItem(dataSourceEnumerator.Current, array[j]);
						if (DataPointCollection.IsEmptyValue(array2[j]))
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
						object obj4 = DataPointCollection.ConvertEnumerationItem(dataSourceEnumerator.Current, otherFieldNames[k]);
						if (!DataPointCollection.IsEmptyValue(obj4))
						{
							dataPoint.SetPointAttribute(obj4, otherAttributeNames[k], otherValueFormat[k]);
						}
					}
				}
				if (flag3)
				{
					if (obj2 != null)
					{
						dataPoint.SetValueXY(obj2, array2);
					}
					else
					{
						dataPoint.SetValueXY(0, array2);
					}
					DataPointCollection.DataPointInit(series, ref dataPoint);
					dataPoint.Empty = true;
					series.Points.Add(dataPoint);
				}
				else
				{
					if (obj2 != null)
					{
						dataPoint.SetValueXY(obj2, array2);
					}
					else
					{
						dataPoint.SetValueXY(0, array2);
					}
					DataPointCollection.DataPointInit(series, ref dataPoint);
					series.Points.Add(dataPoint);
				}
			}
			while (flag);
			if (sort)
			{
				ArrayList arrayList3 = (ArrayList)arrayList2.Clone();
				arrayList2.Sort();
				if (sortingOrder == PointsSortOrder.Descending)
				{
					arrayList2.Reverse();
				}
				ArrayList arrayList4 = new ArrayList();
				foreach (object item in arrayList2)
				{
					arrayList4.Add(arrayList[arrayList3.IndexOf(item)]);
				}
				arrayList = arrayList4;
			}
			foreach (Series item2 in arrayList)
			{
				common.Chart.Series.Add(item2);
			}
		}

		internal void DataBindTable(IEnumerable dataSource, string xField)
		{
			if (dataSource == null)
			{
				return;
			}
			ArrayList dataSourceMemberNames = GetDataSourceMemberNames(dataSource, usedForYValue: true);
			if (xField != null && xField.Length > 0)
			{
				int num = dataSourceMemberNames.IndexOf(xField);
				if (num >= 0)
				{
					dataSourceMemberNames.RemoveAt(num);
				}
				else
				{
					try
					{
						num = int.Parse(xField, CultureInfo.InvariantCulture);
						if (num >= 0 && num < dataSourceMemberNames.Count)
						{
							dataSourceMemberNames.RemoveAt(num);
						}
					}
					catch
					{
					}
				}
			}
			if (dataSourceMemberNames.Count <= 0)
			{
				return;
			}
			ArrayList arrayList = new ArrayList();
			int num2 = 0;
			foreach (string item in dataSourceMemberNames)
			{
				Series series = new Series(item);
				series.ValueMembersY = item;
				series.ValueMemberX = xField;
				arrayList.Add(series);
				num2++;
			}
			DataBind(dataSource, arrayList);
			foreach (Series item2 in arrayList)
			{
				item2.ValueMembersY = string.Empty;
				item2.ValueMemberX = string.Empty;
				common.Chart.Series.Add(item2);
			}
		}

		internal static bool CheckLicense()
		{
			bool result = false;
			try
			{
				string fileName = "Microsoft.Reporting.Chart.WebForms.Chart.lic";
				result = CheckLicense("SOFTWARE\\Dundas Software\\Charting\\WebControlVS2005", fileName);
				return result;
			}
			catch
			{
				return result;
			}
		}

		private static bool CheckLicense(string keyName, string fileName)
		{
			bool result = false;
			return result;
		}
	}
}
