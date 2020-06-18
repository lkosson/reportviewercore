using System;
using System.Data;
using System.Data.SqlTypes;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class DBF : RecordFileReader, IDisposable
	{
		private struct DBFColumnHeader
		{
			private string _fieldName;

			private char _fieldType;

			private byte _fieldLength;

			private byte _decimalCount;

			private string _value;

			public object Value
			{
				get
				{
					if (_value.Length == 0 && _fieldType != 'C')
					{
						return null;
					}
					try
					{
						switch (_fieldType)
						{
						case 'D':
							return DateTime.ParseExact(_value, "yyyyMMdd", CultureInfo.InvariantCulture);
						case 'N':
							return decimal.Parse(_value, CultureInfo.InvariantCulture);
						case 'L':
							if (_value == "T")
							{
								return true;
							}
							return false;
						case '+':
						case 'I':
							return int.Parse(_value, CultureInfo.InvariantCulture);
						case 'F':
							return float.Parse(_value, CultureInfo.InvariantCulture);
						case 'O':
							return double.Parse(_value, CultureInfo.InvariantCulture);
						default:
							return _value;
						}
					}
					catch
					{
						return null;
					}
				}
			}

			public string FieldName => _fieldName;

			public Type FieldType
			{
				get
				{
					switch (_fieldType)
					{
					case 'D':
						return typeof(DateTime);
					case 'N':
						return typeof(decimal);
					case 'L':
						return typeof(bool);
					case '+':
					case 'I':
						return typeof(int);
					case 'F':
						return typeof(float);
					case 'O':
						return typeof(double);
					default:
						return typeof(string);
					}
				}
			}

			public void ReadHeader(SqlBytesReader reader)
			{
				char[] array = reader.ReadChars(11);
				int i;
				for (i = 0; i < array.Length && array[i] != 0; i++)
				{
				}
				_fieldName = new string(array, 0, i);
				_fieldType = reader.ReadChar();
				reader.ReadUInt32();
				_fieldLength = reader.ReadByte();
				_decimalCount = reader.ReadByte();
				reader.ReadUInt16();
				reader.ReadByte();
				reader.ReadUInt16();
				reader.ReadByte();
				reader.ReadUInt32();
				reader.ReadUInt16();
				reader.ReadByte();
				reader.ReadByte();
				if (_fieldType == 'C')
				{
					if (_fieldLength >= byte.MaxValue)
					{
						throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture.NumberFormat, SR.UnsupportedDbfFieldLength, _fieldType, _fieldLength));
					}
					return;
				}
				if (_fieldType == 'N')
				{
					if (_fieldLength <= 20)
					{
						return;
					}
					throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture.NumberFormat, SR.UnsupportedDbfFieldLength, _fieldType, _fieldLength));
				}
				if (_fieldType == 'L')
				{
					if (_fieldLength == 1)
					{
						return;
					}
					throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture.NumberFormat, SR.UnsupportedDbfFieldLength, _fieldType, _fieldLength));
				}
				if (_fieldType == 'D')
				{
					if (_fieldLength == 8)
					{
						return;
					}
					throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture.NumberFormat, SR.UnsupportedDbfFieldLength, _fieldType, _fieldLength));
				}
				if (_fieldType == 'F')
				{
					if (_fieldLength <= 20)
					{
						return;
					}
					throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture.NumberFormat, SR.UnsupportedDbfFieldLength, _fieldType, _fieldLength));
				}
				throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture.NumberFormat, SR.UnsupportedDbfField, _fieldType));
			}

			public void ReadFieldData(SqlBytesReader reader)
			{
				char[] value = reader.ReadChars(_fieldLength);
				_value = new string(value).Trim();
			}
		}

		private byte _version;

		private uint _numberOfRecords;

		private ushort _headerLength;

		private ushort _recordLength;

		private bool _dataOK;

		private DBFColumnHeader[] _fields;

		public DBF(SqlBytes data)
			: base(data)
		{
			ReadHeader();
		}

		protected long ReadHeader()
		{
			_dataOK = false;
			_version = base.Reader.ReadByte();
			if ((_version & 7) != 3)
			{
				throw new InvalidDataException(SR.UnsupportedDbfFormat);
			}
			base.Reader.ReadByte();
			base.Reader.ReadByte();
			base.Reader.ReadByte();
			_numberOfRecords = base.Reader.ReadUInt32();
			if (_numberOfRecords >= 1000000000)
			{
				throw new InvalidDataException(SR.UnsupportedDbfNumberOfRecords);
			}
			_headerLength = base.Reader.ReadUInt16();
			_recordLength = base.Reader.ReadUInt16();
			if (_recordLength > 4000)
			{
				throw new InvalidDataException(SR.UnsupportedDbfLongRecords);
			}
			base.Reader.ReadUInt16();
			if (base.Reader.ReadByte() != 0)
			{
				throw new InvalidDataException(SR.UnsupportedDbfTransactions);
			}
			if (base.Reader.ReadByte() != 0)
			{
				throw new InvalidDataException(SR.UnsupportedDbfEncryption);
			}
			base.Reader.ReadUInt32();
			base.Reader.ReadUInt32();
			base.Reader.ReadUInt32();
			base.Reader.ReadByte();
			base.Reader.ReadByte();
			base.Reader.ReadUInt16();
			int num = (_headerLength - 1) / 32 - 1;
			if (num < 1)
			{
				throw new InvalidDataException(SR.UnsupportedDbfNumberOfFields0);
			}
			_fields = new DBFColumnHeader[num];
			for (int i = 0; i < num; i++)
			{
				_fields[i].ReadHeader(base.Reader);
			}
			if (base.Reader.ReadByte() != 13)
			{
				throw new InvalidDataException(SR.UnsupportedDbfHeader);
			}
			return _numberOfRecords;
		}

		public override bool ReadRecord()
		{
			_dataOK = false;
			if (base.Reader.EOF)
			{
				return false;
			}
			switch (base.Reader.ReadByte())
			{
			case 26:
				return false;
			case 42:
				throw new InvalidDataException(SR.UnsupportedDbfDeleted);
			default:
				throw new InvalidDataException(SR.UnsupportedDbfRecordFlag);
			case 32:
			{
				for (int i = 0; i < _fields.Length; i++)
				{
					_fields[i].ReadFieldData(base.Reader);
				}
				_dataOK = true;
				return true;
			}
			}
		}

		public DataTable GetDataTable()
		{
			DataTable dataTable = new DataTable();
			dataTable.Locale = CultureInfo.CurrentCulture;
			DBFColumnHeader[] fields = _fields;
			for (int i = 0; i < fields.Length; i++)
			{
				DBFColumnHeader dBFColumnHeader = fields[i];
				dataTable.Columns.Add(dBFColumnHeader.FieldName, dBFColumnHeader.FieldType);
			}
			dataTable.BeginLoadData();
			while (ReadRecord())
			{
				object[] array = new object[_fields.Length];
				for (int j = 0; j < _fields.Length; j++)
				{
					array[j] = _fields[j].Value;
				}
				dataTable.LoadDataRow(array, fAcceptChanges: true);
			}
			dataTable.EndLoadData();
			return dataTable;
		}

		public SqlXml GetXML()
		{
			SqlXml @null = SqlXml.Null;
			if (!_dataOK)
			{
				return @null;
			}
			StringBuilder stringBuilder = new StringBuilder(4000);
			using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder))
			{
				xmlWriter.WriteStartElement("shapeData");
				for (int i = 0; i < _fields.Length; i++)
				{
					xmlWriter.WriteElementString(XmlConvert.EncodeName(_fields[i].FieldName), XmlConvert.EncodeName(_fields[i].Value.ToString()));
				}
				xmlWriter.WriteEndElement();
				xmlWriter.Close();
			}
			using (StringReader input = new StringReader(stringBuilder.ToString()))
			{
				using (XmlReader value = XmlReader.Create(input))
				{
					return new SqlXml(value);
				}
			}
		}

		void IDisposable.Dispose()
		{
			Dispose();
			GC.SuppressFinalize(this);
		}
	}
}
