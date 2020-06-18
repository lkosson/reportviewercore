using System;
using System.IO;
using System.Xml;

namespace Microsoft.ReportingServices.Common
{
	internal static class ObjectSerializer
	{
		internal static DataTypeCode GetDataTypeCode(object value)
		{
			if (value == null)
			{
				return DataTypeCode.Empty;
			}
			return GetDataTypeCode(value, Type.GetTypeCode(value.GetType()));
		}

		internal static DataTypeCode GetDataTypeCode(object value, TypeCode typeCode)
		{
			switch (typeCode)
			{
			case TypeCode.Empty:
			case TypeCode.DBNull:
				return DataTypeCode.Empty;
			case TypeCode.Decimal:
				return DataTypeCode.Decimal;
			case TypeCode.Double:
				return DataTypeCode.Double;
			case TypeCode.Int64:
				return DataTypeCode.Int64;
			case TypeCode.DateTime:
				return DataTypeCode.DateTime;
			case TypeCode.String:
				return DataTypeCode.String;
			case TypeCode.Boolean:
				return DataTypeCode.Boolean;
			case TypeCode.Byte:
				return DataTypeCode.Byte;
			case TypeCode.Int16:
				return DataTypeCode.Int16;
			case TypeCode.Int32:
				return DataTypeCode.Int32;
			case TypeCode.SByte:
				return DataTypeCode.SByte;
			case TypeCode.Single:
				return DataTypeCode.Single;
			case TypeCode.UInt16:
				return DataTypeCode.UInt16;
			case TypeCode.UInt32:
				return DataTypeCode.UInt32;
			case TypeCode.UInt64:
				return DataTypeCode.UInt64;
			case TypeCode.Char:
				return DataTypeCode.Char;
			case TypeCode.Object:
				if (value is TimeSpan)
				{
					return DataTypeCode.TimeSpan;
				}
				if (value is DateTimeOffset)
				{
					return DataTypeCode.DateTimeOffset;
				}
				if (value is byte[])
				{
					return DataTypeCode.ByteArray;
				}
				break;
			}
			return DataTypeCode.Unknown;
		}

		internal static bool Equals(object obj1, object obj2, DataTypeCode dataTypeCode1, DataTypeCode dataTypeCode2)
		{
			DataTypeCode dataTypeCode3 = (dataTypeCode1 == DataTypeCode.Unknown) ? GetDataTypeCode(obj1) : dataTypeCode1;
			DataTypeCode dataTypeCode4 = (dataTypeCode2 == DataTypeCode.Unknown) ? GetDataTypeCode(obj2) : dataTypeCode2;
			if (dataTypeCode3 != dataTypeCode4)
			{
				return false;
			}
			if (dataTypeCode3 == DataTypeCode.Unknown)
			{
				return false;
			}
			if (obj1 == obj2)
			{
				return true;
			}
			switch (dataTypeCode3)
			{
			case DataTypeCode.Empty:
				if (obj1 == null)
				{
					return obj2 == null;
				}
				return false;
			case DataTypeCode.ByteArray:
			{
				byte[] array = (byte[])obj1;
				byte[] array2 = (byte[])obj2;
				if (array.Length != array2.Length)
				{
					return false;
				}
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] != array2[i])
					{
						return false;
					}
				}
				return true;
			}
			default:
				return obj1.Equals(obj2);
			}
		}

		internal static object Read(BinaryReader binaryReader, DataTypeCode dataTypeCode)
		{
			switch (dataTypeCode)
			{
			case DataTypeCode.Char:
				return binaryReader.ReadChar();
			case DataTypeCode.String:
				return binaryReader.ReadString();
			case DataTypeCode.Boolean:
				return binaryReader.ReadBoolean();
			case DataTypeCode.Byte:
				return binaryReader.ReadByte();
			case DataTypeCode.Int16:
				return binaryReader.ReadInt16();
			case DataTypeCode.Int32:
				return binaryReader.ReadInt32();
			case DataTypeCode.Int64:
				return binaryReader.ReadInt64();
			case DataTypeCode.Single:
				return binaryReader.ReadSingle();
			case DataTypeCode.Decimal:
				return ReadDecimalFromBinary(binaryReader);
			case DataTypeCode.Double:
				return binaryReader.ReadDouble();
			case DataTypeCode.DateTime:
				return DateTime.FromBinary(binaryReader.ReadInt64());
			case DataTypeCode.SByte:
				return binaryReader.ReadSByte();
			case DataTypeCode.UInt16:
				return binaryReader.ReadUInt16();
			case DataTypeCode.UInt32:
				return binaryReader.ReadUInt32();
			case DataTypeCode.UInt64:
				return binaryReader.ReadUInt64();
			case DataTypeCode.TimeSpan:
				return TimeSpan.FromTicks(binaryReader.ReadInt64());
			case DataTypeCode.DateTimeOffset:
			{
				DateTime dateTime = DateTime.FromBinary(binaryReader.ReadInt64());
				TimeSpan offset = TimeSpan.FromTicks(binaryReader.ReadInt64());
				return new DateTimeOffset(dateTime, offset);
			}
			case DataTypeCode.ByteArray:
			{
				int count = binaryReader.ReadInt32();
				return binaryReader.ReadBytes(count);
			}
			default:
				return null;
			}
		}

		internal static void Write(BinaryWriter binaryWriter, object value, DataTypeCode dataTypeCode)
		{
			if (value != null)
			{
				switch (dataTypeCode)
				{
				case DataTypeCode.Byte:
					binaryWriter.Write((byte)value);
					break;
				case DataTypeCode.SByte:
					binaryWriter.Write((sbyte)value);
					break;
				case DataTypeCode.Decimal:
					WriteDecimalToBinary(binaryWriter, (decimal)value);
					break;
				case DataTypeCode.Double:
					binaryWriter.Write((double)value);
					break;
				case DataTypeCode.Int16:
					binaryWriter.Write((short)value);
					break;
				case DataTypeCode.UInt16:
					binaryWriter.Write((ushort)value);
					break;
				case DataTypeCode.Int32:
					binaryWriter.Write((int)value);
					break;
				case DataTypeCode.UInt32:
					binaryWriter.Write((uint)value);
					break;
				case DataTypeCode.Int64:
					binaryWriter.Write((long)value);
					break;
				case DataTypeCode.UInt64:
					binaryWriter.Write((ulong)value);
					break;
				case DataTypeCode.Single:
					binaryWriter.Write((float)value);
					break;
				case DataTypeCode.DateTime:
					WriteDateTimeToBinary(binaryWriter, (DateTime)value);
					break;
				case DataTypeCode.DateTimeOffset:
				{
					DateTimeOffset dateTimeOffset = (DateTimeOffset)value;
					WriteDateTimeToBinary(binaryWriter, dateTimeOffset.DateTime);
					binaryWriter.Write(dateTimeOffset.Offset.Ticks);
					break;
				}
				case DataTypeCode.TimeSpan:
					binaryWriter.Write(((TimeSpan)value).Ticks);
					break;
				case DataTypeCode.Boolean:
					binaryWriter.Write((bool)value);
					break;
				case DataTypeCode.Char:
					binaryWriter.Write((char)value);
					break;
				case DataTypeCode.String:
					binaryWriter.Write(value.ToString());
					break;
				case DataTypeCode.ByteArray:
				{
					byte[] array = (byte[])value;
					binaryWriter.Write(array.Length);
					binaryWriter.Write(array);
					break;
				}
				}
			}
		}

		internal static object Read(XmlReader xmlReader, DataTypeCode dataTypeCode)
		{
			switch (dataTypeCode)
			{
			case DataTypeCode.Byte:
				return XmlConvert.ToByte(xmlReader.Value);
			case DataTypeCode.SByte:
				return XmlConvert.ToSByte(xmlReader.Value);
			case DataTypeCode.Decimal:
				return XmlConvert.ToDecimal(xmlReader.Value);
			case DataTypeCode.Double:
				return XmlConvert.ToDouble(xmlReader.Value);
			case DataTypeCode.Single:
				return XmlConvert.ToSingle(xmlReader.Value);
			case DataTypeCode.Int16:
				return XmlConvert.ToInt16(xmlReader.Value);
			case DataTypeCode.Int32:
				return XmlConvert.ToInt32(xmlReader.Value);
			case DataTypeCode.Int64:
				return XmlConvert.ToInt64(xmlReader.Value);
			case DataTypeCode.UInt16:
				return XmlConvert.ToUInt16(xmlReader.Value);
			case DataTypeCode.UInt32:
				return XmlConvert.ToUInt32(xmlReader.Value);
			case DataTypeCode.UInt64:
				return XmlConvert.ToUInt64(xmlReader.Value);
			case DataTypeCode.DateTime:
				return XmlConvert.ToDateTime(xmlReader.Value, XmlDateTimeSerializationMode.RoundtripKind);
			case DataTypeCode.TimeSpan:
				return XmlConvert.ToTimeSpan(xmlReader.Value);
			case DataTypeCode.DateTimeOffset:
				return XmlConvert.ToDateTimeOffset(xmlReader.Value);
			case DataTypeCode.Boolean:
				return XmlConvert.ToBoolean(xmlReader.Value);
			case DataTypeCode.Char:
				return XmlConvert.ToChar(xmlReader.Value);
			case DataTypeCode.String:
				return xmlReader.Value;
			case DataTypeCode.ByteArray:
				return Convert.FromBase64String(xmlReader.Value);
			default:
				return null;
			}
		}

		internal static void Write(XmlWriter xmlWriter, object value, DataTypeCode dataTypeCode)
		{
			if (value != null)
			{
				switch (dataTypeCode)
				{
				case DataTypeCode.Byte:
					xmlWriter.WriteString(XmlConvert.ToString((byte)value));
					break;
				case DataTypeCode.SByte:
					xmlWriter.WriteString(XmlConvert.ToString((sbyte)value));
					break;
				case DataTypeCode.Decimal:
					xmlWriter.WriteString(XmlConvert.ToString((decimal)value));
					break;
				case DataTypeCode.Double:
					xmlWriter.WriteString(XmlConvert.ToString((double)value));
					break;
				case DataTypeCode.Single:
					xmlWriter.WriteString(XmlConvert.ToString((float)value));
					break;
				case DataTypeCode.Int16:
					xmlWriter.WriteString(XmlConvert.ToString((short)value));
					break;
				case DataTypeCode.Int32:
					xmlWriter.WriteString(XmlConvert.ToString((int)value));
					break;
				case DataTypeCode.Int64:
					xmlWriter.WriteString(XmlConvert.ToString((long)value));
					break;
				case DataTypeCode.UInt16:
					xmlWriter.WriteString(XmlConvert.ToString((ushort)value));
					break;
				case DataTypeCode.UInt32:
					xmlWriter.WriteString(XmlConvert.ToString((uint)value));
					break;
				case DataTypeCode.UInt64:
					xmlWriter.WriteString(XmlConvert.ToString((ulong)value));
					break;
				case DataTypeCode.DateTime:
					xmlWriter.WriteString(XmlConvert.ToString((DateTime)value, XmlDateTimeSerializationMode.RoundtripKind));
					break;
				case DataTypeCode.TimeSpan:
					xmlWriter.WriteString(XmlConvert.ToString((TimeSpan)value));
					break;
				case DataTypeCode.DateTimeOffset:
					xmlWriter.WriteString(XmlConvert.ToString((DateTimeOffset)value));
					break;
				case DataTypeCode.Boolean:
					xmlWriter.WriteString(XmlConvert.ToString((bool)value));
					break;
				case DataTypeCode.Char:
					xmlWriter.WriteString(XmlConvert.ToString((char)value));
					break;
				case DataTypeCode.String:
					xmlWriter.WriteString((string)value);
					break;
				case DataTypeCode.ByteArray:
				{
					byte[] array = (byte[])value;
					xmlWriter.WriteBase64(array, 0, array.Length);
					break;
				}
				}
			}
		}

		private static decimal ReadDecimalFromBinary(BinaryReader reader)
		{
			return reader.ReadDecimal();
		}

		private static void WriteDateTimeToBinary(BinaryWriter writer, DateTime value)
		{
			writer.Write(value.ToBinary());
		}

		private static void WriteDecimalToBinary(BinaryWriter writer, decimal value)
		{
			writer.Write(value);
		}
	}
}
