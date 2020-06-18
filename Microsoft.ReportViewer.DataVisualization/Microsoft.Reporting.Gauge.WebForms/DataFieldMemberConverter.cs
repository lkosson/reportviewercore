using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Globalization;
using System.Reflection;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class DataFieldMemberConverter : StringConverter
	{
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return false;
		}

		private void AddToList(ArrayList names, DataTable dataTable, bool includeTableName)
		{
			foreach (DataColumn column in dataTable.Columns)
			{
				if (includeTableName)
				{
					names.Add(dataTable.TableName + "." + column.ColumnName);
				}
				else
				{
					names.Add(column.ColumnName);
				}
			}
		}

		internal static ArrayList GetDataSourceMemberNames(object dataSource)
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
						dataTable.Locale = CultureInfo.InvariantCulture;
						dataTable = ((OleDbDataAdapter)dataSource).FillSchema(dataTable, SchemaType.Mapped);
					}
					else if (dataSource is SqlDataAdapter)
					{
						dataTable = new DataTable();
						dataTable.Locale = CultureInfo.InvariantCulture;
						dataTable = ((SqlDataAdapter)dataSource).FillSchema(dataTable, SchemaType.Mapped);
					}
					else if (dataSource is OleDbDataReader)
					{
						for (int i = 0; i < ((OleDbDataReader)dataSource).FieldCount; i++)
						{
							arrayList.Add(((OleDbDataReader)dataSource).GetName(i));
						}
					}
					else if (dataSource is SqlDataReader)
					{
						for (int j = 0; j < ((SqlDataReader)dataSource).FieldCount; j++)
						{
							arrayList.Add(((SqlDataReader)dataSource).GetName(j));
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
									arrayList.Add(oleDbDataReader.GetName(k));
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
									arrayList.Add(sqlDataReader.GetName(l));
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
							arrayList.Add(column.ColumnName);
						}
					}
					else if (arrayList.Count == 0 && dataSource is ITypedList)
					{
						foreach (PropertyDescriptor itemProperty in ((ITypedList)dataSource).GetItemProperties(null))
						{
							if (itemProperty.PropertyType != typeof(string))
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
							if (property.PropertyType != typeof(string))
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
	}
}
