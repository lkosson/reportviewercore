using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class DataBindingHelper
	{
		internal class DataFieldDescriptor
		{
			public readonly string Name = "";

			public readonly Type Type;

			public DataFieldDescriptor(string name, Type type)
			{
				Name = name;
				Type = type;
			}

			public override string ToString()
			{
				return Name;
			}
		}

		public static bool IsValidDataSource(object dataSource)
		{
			if (dataSource == null || dataSource is IEnumerable || dataSource is DataSet || dataSource is DataView || dataSource is DataTable || dataSource is IDbCommand || dataSource is IDbDataAdapter)
			{
				return true;
			}
			return false;
		}

		public static StringCollection GetEnumerationItemFiledNames(object item, string fieldToExclude)
		{
			StringCollection stringCollection = new StringCollection();
			if (item is DataRow)
			{
				foreach (DataColumn column in ((DataRow)item).Table.Columns)
				{
					if (column.ColumnName != fieldToExclude)
					{
						stringCollection.Add(column.ColumnName);
					}
				}
				return stringCollection;
			}
			if (item is DataRowView)
			{
				foreach (DataColumn column2 in ((DataRowView)item).DataView.Table.Columns)
				{
					if (column2.ColumnName != fieldToExclude)
					{
						stringCollection.Add(column2.ColumnName);
					}
				}
				return stringCollection;
			}
			if (item is DbDataRecord)
			{
				DbDataRecord dbDataRecord = (DbDataRecord)item;
				for (int i = 0; i < dbDataRecord.FieldCount; i++)
				{
					string name = dbDataRecord.GetName(i);
					if (name != fieldToExclude)
					{
						stringCollection.Add(name);
					}
				}
			}
			else if (item != null)
			{
				PropertyInfo[] properties = item.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
				if (properties != null)
				{
					PropertyInfo[] array = properties;
					foreach (PropertyInfo propertyInfo in array)
					{
						if (propertyInfo.Name != fieldToExclude)
						{
							stringCollection.Add(propertyInfo.Name);
						}
					}
				}
				FieldInfo[] fields = item.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
				if (fields != null)
				{
					FieldInfo[] array2 = fields;
					foreach (FieldInfo fieldInfo in array2)
					{
						if (fieldInfo.Name != fieldToExclude)
						{
							stringCollection.Add(fieldInfo.Name);
						}
					}
				}
			}
			return stringCollection;
		}

		public static IEnumerable GetDataSourceAsIEnumerable(object data, string dataMember, out bool closeDataReader, out IDbConnection connection)
		{
			object obj = data;
			closeDataReader = false;
			connection = null;
			if (obj != null)
			{
				try
				{
					if (obj is IDbDataAdapter)
					{
						obj = ((IDbDataAdapter)obj).SelectCommand;
					}
					if (obj is DataSet && ((DataSet)obj).Tables.Count > 0)
					{
						obj = ((!(dataMember == string.Empty) && ((DataSet)obj).Tables.Count != 1) ? ((DataSet)obj).Tables[dataMember] : ((DataSet)obj).Tables[0]);
					}
					if (obj is DataTable)
					{
						obj = new DataView((DataTable)obj);
					}
					else if (obj is IDbCommand)
					{
						if (((IDbCommand)obj).Connection.State != ConnectionState.Open)
						{
							connection = ((IDbCommand)obj).Connection;
							connection.Open();
						}
						obj = ((IDbCommand)obj).ExecuteReader();
						closeDataReader = true;
					}
					else
					{
						obj = (obj as IEnumerable);
					}
				}
				catch (Exception innerException)
				{
					throw new ApplicationException(SR.bad_data_src, innerException);
				}
				return obj as IEnumerable;
			}
			return null;
		}

		public static object ConvertEnumerationItem(object item, string fieldName)
		{
			object result = item;
			bool flag = true;
			if (item is DataRow)
			{
				if (!string.IsNullOrEmpty(fieldName))
				{
					if (((DataRow)item).Table.Columns.Contains(fieldName))
					{
						result = ((DataRow)item)[fieldName];
						flag = false;
					}
					else
					{
						try
						{
							int num = int.Parse(fieldName, CultureInfo.CurrentCulture);
							if (((DataRow)item).Table.Columns.Count < num && num >= 0)
							{
								result = ((DataRow)item)[num];
								flag = false;
							}
						}
						catch
						{
						}
					}
				}
			}
			else if (item is DataRowView)
			{
				if (!string.IsNullOrEmpty(fieldName))
				{
					if (((DataRowView)item).DataView.Table.Columns.Contains(fieldName))
					{
						result = ((DataRowView)item)[fieldName];
						flag = false;
					}
					else
					{
						try
						{
							int num2 = int.Parse(fieldName, CultureInfo.CurrentCulture);
							if (((DataRowView)item).DataView.Table.Columns.Count < num2 && num2 >= 0)
							{
								result = ((DataRowView)item)[num2];
								flag = false;
							}
						}
						catch
						{
						}
					}
				}
			}
			else if (item is DbDataRecord)
			{
				if (!string.IsNullOrEmpty(fieldName))
				{
					if (!char.IsNumber(fieldName, 0))
					{
						try
						{
							result = ((DbDataRecord)item)[fieldName];
							flag = false;
						}
						catch (Exception)
						{
						}
					}
					if (flag)
					{
						try
						{
							int i = int.Parse(fieldName, CultureInfo.CurrentCulture);
							result = ((DbDataRecord)item)[i];
							flag = false;
						}
						catch
						{
						}
					}
				}
			}
			else if (item != null)
			{
				try
				{
					PropertyInfo property = item.GetType().GetProperty(fieldName, BindingFlags.Instance | BindingFlags.Public);
					if (property != null)
					{
						result = property.GetValue(item, new object[0]);
						flag = false;
					}
					else
					{
						FieldInfo field = item.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public);
						if (field != null)
						{
							result = field.GetValue(item);
							flag = false;
						}
					}
				}
				catch
				{
				}
			}
			if (flag)
			{
				throw new ArgumentException(SR.column_not_found(fieldName));
			}
			return result;
		}

		public static ArrayList GetDataSourceDataFields(object dataSource, string dataMember, string fieldToExclude)
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
						dataTable = ((!string.IsNullOrEmpty(dataMember)) ? ((DataSet)dataSource).Tables[dataMember] : ((DataSet)dataSource).Tables[0]);
					}
					else if (dataSource is IDataAdapter)
					{
						DataSet dataSet = new DataSet();
						dataSet.Locale = CultureInfo.CurrentCulture;
						DataTable[] array = ((IDataAdapter)dataSource).FillSchema(dataSet, SchemaType.Mapped);
						if (array.Length != 0)
						{
							dataTable = array[0];
						}
					}
					else if (dataSource is IDataReader)
					{
						IDataReader dataReader = (IDataReader)dataSource;
						if (!dataReader.IsClosed)
						{
							DataTable schemaTable = dataReader.GetSchemaTable();
							if (schemaTable != null)
							{
								foreach (DataRow row in schemaTable.Rows)
								{
									string text = (string)row["ColumnName"];
									Type type = (Type)row["DataType"];
									if (text != fieldToExclude)
									{
										arrayList.Add(new DataFieldDescriptor(text, type));
									}
								}
							}
						}
					}
					else if (dataSource is IDbCommand)
					{
						IDbCommand dbCommand = (IDbCommand)dataSource;
						IDataReader dataReader2 = null;
						if (dbCommand.Connection != null)
						{
							try
							{
								dbCommand.Connection.Open();
								dataReader2 = dbCommand.ExecuteReader();
								DataTable schemaTable2 = dataReader2.GetSchemaTable();
								if (schemaTable2 != null)
								{
									foreach (DataRow row2 in schemaTable2.Rows)
									{
										string text2 = (string)row2["ColumnName"];
										Type type2 = (Type)row2["DataType"];
										if (text2 != fieldToExclude)
										{
											arrayList.Add(new DataFieldDescriptor(text2, type2));
										}
									}
								}
								dataReader2.Close();
							}
							finally
							{
								if (dbCommand.Connection != null && dbCommand.Connection.State != 0)
								{
									dbCommand.Connection.Close();
								}
								if (dataReader2 != null && !dataReader2.IsClosed)
								{
									dataReader2.Close();
								}
							}
						}
					}
					if (dataTable != null)
					{
						foreach (DataColumn column in dataTable.Columns)
						{
							if (column.ColumnName != fieldToExclude)
							{
								arrayList.Add(new DataFieldDescriptor(column.ColumnName, column.DataType));
							}
						}
						return arrayList;
					}
					if (arrayList.Count == 0 && dataSource is ITypedList)
					{
						foreach (PropertyDescriptor itemProperty2 in ((ITypedList)dataSource).GetItemProperties(null))
						{
							if (itemProperty2.PropertyType != typeof(string) && itemProperty2.Name != fieldToExclude)
							{
								arrayList.Add(new DataFieldDescriptor(itemProperty2.Name, itemProperty2.PropertyType));
							}
						}
						return arrayList;
					}
					if (arrayList.Count != 0)
					{
						return arrayList;
					}
					if (dataSource is IEnumerable)
					{
						IEnumerator enumerator2 = ((IEnumerable)dataSource).GetEnumerator();
						enumerator2.Reset();
						enumerator2.MoveNext();
						{
							foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(enumerator2.Current))
							{
								if (property.PropertyType != typeof(string) && property.Name != fieldToExclude)
								{
									arrayList.Add(new DataFieldDescriptor(property.Name, property.PropertyType));
								}
							}
							return arrayList;
						}
					}
					return arrayList;
				}
				catch
				{
					return arrayList;
				}
			}
			return arrayList;
		}

		public static StringCollection GetDataSourceDataMembers(object dataSource)
		{
			StringCollection stringCollection = new StringCollection();
			DataSet dataSet = dataSource as DataSet;
			if (dataSet != null)
			{
				foreach (DataTable table in dataSet.Tables)
				{
					stringCollection.Add(table.TableName);
				}
				return stringCollection;
			}
			return stringCollection;
		}

		public static StringCollection GetDataSourceCoordinateFields(object dataSource, string dataMember, string fieldToExclude)
		{
			StringCollection stringCollection = new StringCollection();
			foreach (DataFieldDescriptor dataSourceDataField in GetDataSourceDataFields(dataSource, dataMember, fieldToExclude))
			{
				if (IsValidAsCoordinateType(dataSourceDataField.Type))
				{
					stringCollection.Add(dataSourceDataField.Name);
				}
			}
			return stringCollection;
		}

		public static bool HasRealData(object dataSource)
		{
			StringCollection dataSourceDataMembers = GetDataSourceDataMembers(dataSource);
			if (dataSourceDataMembers.Count == 0)
			{
				bool closeDataReader = false;
				IDbConnection connection = null;
				IEnumerable dataSourceAsIEnumerable = GetDataSourceAsIEnumerable(dataSource, null, out closeDataReader, out connection);
				try
				{
					IEnumerator enumerator = dataSourceAsIEnumerable.GetEnumerator();
					try
					{
						if (enumerator.MoveNext())
						{
							_ = enumerator.Current;
							return true;
						}
					}
					finally
					{
						IDisposable disposable = enumerator as IDisposable;
						if (disposable != null)
						{
							disposable.Dispose();
						}
					}
				}
				finally
				{
					if (closeDataReader)
					{
						((IDataReader)dataSourceAsIEnumerable).Close();
					}
					connection?.Close();
				}
			}
			else
			{
				StringEnumerator enumerator2 = dataSourceDataMembers.GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						string current = enumerator2.Current;
						bool closeDataReader2 = false;
						IDbConnection connection2 = null;
						IEnumerable dataSourceAsIEnumerable2 = GetDataSourceAsIEnumerable(dataSource, current, out closeDataReader2, out connection2);
						try
						{
							IEnumerator enumerator = dataSourceAsIEnumerable2.GetEnumerator();
							try
							{
								if (enumerator.MoveNext())
								{
									_ = enumerator.Current;
									return true;
								}
							}
							finally
							{
								IDisposable disposable2 = enumerator as IDisposable;
								if (disposable2 != null)
								{
									disposable2.Dispose();
								}
							}
						}
						finally
						{
							if (closeDataReader2)
							{
								((IDataReader)dataSourceAsIEnumerable2).Close();
							}
							connection2?.Close();
						}
					}
				}
				finally
				{
					(enumerator2 as IDisposable)?.Dispose();
				}
			}
			return false;
		}

		public static bool IsValidAsCoordinateType(Type type)
		{
			if (type == typeof(string) || type == typeof(double) || type == typeof(MapCoordinate))
			{
				return true;
			}
			return TypeDescriptor.GetConverter(type)?.CanConvertTo(typeof(double)) ?? false;
		}

		public static void InitDesignDataTable(object dataSource, string dataMember, DataTable designDataTable)
		{
			if (dataSource == null)
			{
				return;
			}
			designDataTable.Clear();
			designDataTable.Columns.Clear();
			try
			{
				ArrayList dataSourceDataFields = GetDataSourceDataFields(dataSource, dataMember, string.Empty);
				if (dataSourceDataFields.Count == 0)
				{
					return;
				}
				foreach (DataFieldDescriptor item in dataSourceDataFields)
				{
					designDataTable.Columns.Add(item.Name, Field.ConvertToSupportedType(item.Type));
				}
			}
			catch (Exception)
			{
			}
		}

		public static void PopulateDesignTimeData(DataTable designDataTable, int numberOfSampleRecords)
		{
			designDataTable.Clear();
			try
			{
				for (int i = 0; i < numberOfSampleRecords; i++)
				{
					DataRow dataRow = designDataTable.NewRow();
					foreach (DataColumn column in designDataTable.Columns)
					{
						if (column.DataType == typeof(int))
						{
							dataRow[column] = i;
						}
						else if (column.DataType == typeof(double) || column.DataType == typeof(decimal))
						{
							dataRow[column] = (double)i + (double)i / (double)((int)Math.Log10(i) + 1);
						}
						if (column.DataType == typeof(string))
						{
							dataRow[column] = "ABC";
						}
						if (column.DataType == typeof(DateTime))
						{
							dataRow[column] = new DateTime(2006, 4, 7).AddDays(i);
						}
						if (column.DataType == typeof(TimeSpan))
						{
							dataRow[column] = new TimeSpan(i);
						}
						if (column.DataType == typeof(bool))
						{
							dataRow[column] = ((i % 2 == 0) ? true : false);
						}
					}
					designDataTable.Rows.Add(dataRow);
				}
			}
			catch (Exception)
			{
			}
		}

		public static string GetDataSourceDefaultDataMember(object dataSource)
		{
			string text = "";
			DataSet dataSet = dataSource as DataSet;
			if (dataSet != null && dataSet.Tables.Count > 0)
			{
				return dataSet.Tables[0].TableName;
			}
			return string.Empty;
		}
	}
}
