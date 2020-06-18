using Microsoft.ReportingServices.DataProcessing;
using System;
using System.Collections;
using System.ComponentModel;

namespace Microsoft.Reporting
{
	internal class DataEnumerableReader : IDataReader, IDisposable
	{
		private IEnumerator m_dataEnumerator;

		private PropertyDescriptorCollection m_columns;

		private object m_row;

		private bool m_firstRow;

		public int FieldCount
		{
			get
			{
				if (m_columns == null)
				{
					return 0;
				}
				return m_columns.Count;
			}
		}

		internal DataEnumerableReader(IEnumerable dataEnumerable)
		{
			m_dataEnumerator = dataEnumerable.GetEnumerator();
			if (m_dataEnumerator != null && m_dataEnumerator.MoveNext())
			{
				m_firstRow = true;
				m_row = m_dataEnumerator.Current;
				m_columns = TypeDescriptor.GetProperties(m_row);
			}
		}

		public string GetName(int fieldIndex)
		{
			return m_columns[fieldIndex].Name;
		}

		public int GetOrdinal(string fieldName)
		{
			int result = -1;
			if (fieldName != null && m_columns != null && m_columns.Count > 0)
			{
				PropertyDescriptor propertyDescriptor = m_columns[fieldName];
				if (propertyDescriptor != null)
				{
					result = m_columns.IndexOf(propertyDescriptor);
				}
			}
			return result;
		}

		public Type GetFieldType(int fieldIndex)
		{
			return m_columns[fieldIndex].PropertyType;
		}

		public bool Read()
		{
			if (m_firstRow)
			{
				m_firstRow = false;
				return true;
			}
			if (m_dataEnumerator == null)
			{
				return false;
			}
			if (m_dataEnumerator.MoveNext())
			{
				m_row = m_dataEnumerator.Current;
				return true;
			}
			return false;
		}

		public object GetValue(int fieldIndex)
		{
			return m_columns[fieldIndex].GetValue(m_row);
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
		}
	}
}
