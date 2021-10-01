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
