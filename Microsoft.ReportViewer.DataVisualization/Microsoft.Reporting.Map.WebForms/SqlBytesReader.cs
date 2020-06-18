using System;
using System.Data.SqlTypes;
using System.IO;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class SqlBytesReader
	{
		private SqlBytes _data;

		private int _position;

		private int _length;

		private byte[] _shortBinData = new byte[8];

		internal int Position => _position;

		internal int Length => _length;

		internal bool EOF => _position == _length;

		internal SqlBytesReader(SqlBytes data)
		{
			_data = data;
			_length = (int)_data.Length;
		}

		internal byte ReadByte()
		{
			if (EOF)
			{
				throw new EndOfStreamException();
			}
			return _data[_position++];
		}

		internal int ReadInt32()
		{
			if (_position + 4 > _length)
			{
				throw new EndOfStreamException();
			}
			_position += (int)_data.Read(_position, _shortBinData, 0, 4);
			return BitConverter.ToInt32(_shortBinData, 0);
		}

		internal double ReadDouble()
		{
			if (_position + 8 > _length)
			{
				throw new EndOfStreamException();
			}
			_position += (int)_data.Read(_position, _shortBinData, 0, 8);
			return BitConverter.ToDouble(_shortBinData, 0);
		}

		internal ushort ReadUInt16()
		{
			if (_position + 2 > _length)
			{
				throw new EndOfStreamException();
			}
			_position += (int)_data.Read(_position, _shortBinData, 0, 2);
			return BitConverter.ToUInt16(_shortBinData, 0);
		}

		internal uint ReadUInt32()
		{
			if (_position + 4 > _length)
			{
				throw new EndOfStreamException();
			}
			_position += (int)_data.Read(_position, _shortBinData, 0, 4);
			return BitConverter.ToUInt32(_shortBinData, 0);
		}

		internal char ReadChar()
		{
			if (EOF)
			{
				throw new EndOfStreamException();
			}
			return (char)_data[_position++];
		}

		internal char[] ReadChars(int count)
		{
			if (EOF)
			{
				throw new EndOfStreamException();
			}
			if (_position + count > _length)
			{
				count = _length - _position;
			}
			char[] array = new char[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = (char)_data[_position++];
			}
			return array;
		}
	}
}
