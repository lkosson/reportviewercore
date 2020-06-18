using System;
using System.Data.SqlTypes;
using System.IO;

namespace Microsoft.Reporting.Map.WebForms
{
	internal abstract class RecordFileReader : IDisposable
	{
		private readonly SqlBytesReader _reader;

		public long Length => _reader.Length;

		public long Position => _reader.Position;

		public SqlBytesReader Reader => _reader;

		public abstract bool ReadRecord();

		public RecordFileReader(SqlBytes data)
		{
			if (data.IsNull)
			{
				throw new InvalidDataException(SR.EmptyFile);
			}
			_reader = new SqlBytesReader(data);
		}

		public void Dispose()
		{
			_ = _reader;
			GC.SuppressFinalize(this);
		}
	}
}
