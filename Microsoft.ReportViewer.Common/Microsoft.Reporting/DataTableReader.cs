using Microsoft.ReportingServices.DataProcessing;
using System;
using System.Data;

namespace Microsoft.Reporting
{
	internal class DataTableReader : Microsoft.ReportingServices.DataProcessing.IDataReader, IDisposable
	{
		private int m_currentRowNumber = -1;

		private DataTable m_dataTable;

		public int FieldCount
		{
			get
			{
				if (m_dataTable.Columns == null)
				{
					return 0;
				}
				return m_dataTable.Columns.Count;
			}
		}

		internal DataTableReader(DataTable dataTable)
		{
			m_dataTable = dataTable;
		}

		public string GetName(int fieldIndex)
		{
			return m_dataTable.Columns[fieldIndex].ColumnName;
		}

		public int GetOrdinal(string fieldName)
		{
			int result = -1;
			if (fieldName != null && m_dataTable.Columns != null && m_dataTable.Columns.Count > 0)
			{
				DataColumn dataColumn = m_dataTable.Columns[fieldName];
				if (dataColumn != null)
				{
					result = dataColumn.Ordinal;
				}
			}
			return result;
		}

		public Type GetFieldType(int fieldIndex)
		{
			return m_dataTable.Columns[fieldIndex].GetType();
		}

		public bool Read()
		{
			if (m_dataTable.Rows == null)
			{
				return false;
			}
			m_currentRowNumber++;
			if (m_currentRowNumber < m_dataTable.Rows.Count)
			{
				return true;
			}
			return false;
		}

		public object GetValue(int fieldIndex)
		{
			return m_dataTable.Rows[m_currentRowNumber][fieldIndex];
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
		}
	}
}
