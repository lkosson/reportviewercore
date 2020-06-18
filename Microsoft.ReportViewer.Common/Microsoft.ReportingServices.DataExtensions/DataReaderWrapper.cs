using Microsoft.ReportingServices.DataProcessing;
using System;
using System.Data;

namespace Microsoft.ReportingServices.DataExtensions
{
	internal class DataReaderWrapper : BaseDataWrapper, Microsoft.ReportingServices.DataProcessing.IDataReader, IDisposable
	{
		public virtual int FieldCount => UnderlyingReader.FieldCount;

		public System.Data.IDataReader UnderlyingReader => (System.Data.IDataReader)base.UnderlyingObject;

		public DataReaderWrapper(System.Data.IDataReader underlyingReader)
			: base(underlyingReader)
		{
		}

		public virtual string GetName(int fieldIndex)
		{
			return UnderlyingReader.GetName(fieldIndex);
		}

		public virtual int GetOrdinal(string fieldName)
		{
			int result = -1;
			if (fieldName != null)
			{
				try
				{
					result = UnderlyingReader.GetOrdinal(fieldName);
					return result;
				}
				catch (IndexOutOfRangeException)
				{
					return result;
				}
			}
			return result;
		}

		public virtual bool Read()
		{
			if (UnderlyingReader.Read())
			{
				return true;
			}
			UnderlyingReader.NextResult();
			return false;
		}

		public virtual Type GetFieldType(int fieldIndex)
		{
			return UnderlyingReader.GetFieldType(fieldIndex);
		}

		public virtual object GetValue(int fieldIndex)
		{
			return UnderlyingReader.GetValue(fieldIndex);
		}
	}
}
