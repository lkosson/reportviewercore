using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.ReportIntermediateFormat.Persistence
{
	internal class PersistenceBinaryReader : BinaryReader
	{
		internal long StreamPosition
		{
			get
			{
				return BaseStream.Position;
			}
			set
			{
				BaseStream.Position = value;
			}
		}

		internal bool EOS => base.BaseStream.Position == base.BaseStream.Length;

		internal PersistenceBinaryReader(Stream str)
			: base(str)
		{
		}

		internal bool ReadReference(out int refID, out ObjectType declaredRefType)
		{
			declaredRefType = ReadObjectType();
			if (declaredRefType != 0)
			{
				refID = ReadInt32();
				return true;
			}
			refID = -1;
			return false;
		}

		internal bool ReadListStart(ObjectType objectType, out int listSize)
		{
			if (ReadObjectType() == ObjectType.Null)
			{
				listSize = -1;
				return false;
			}
			listSize = Read7BitEncodedInt();
			return true;
		}

		internal bool ReadDictionaryStart(ObjectType objectType, out int dictionarySize)
		{
			if (ReadObjectType() == ObjectType.Null)
			{
				dictionarySize = -1;
				return false;
			}
			dictionarySize = Read7BitEncodedInt();
			return true;
		}

		internal bool ReadArrayStart(ObjectType objectType, out int arraySize)
		{
			if (ReadObjectType() == ObjectType.Null)
			{
				arraySize = -1;
				return false;
			}
			arraySize = Read7BitEncodedInt();
			return true;
		}

		internal bool Read2DArrayStart(ObjectType objectType, out int arrayXLength, out int arrayYLength)
		{
			if (ReadObjectType() == ObjectType.Null)
			{
				arrayXLength = -1;
				arrayYLength = -1;
				return false;
			}
			arrayXLength = Read7BitEncodedInt();
			arrayYLength = Read7BitEncodedInt();
			return true;
		}

		internal bool[] ReadBooleanArray()
		{
			bool[] array = null;
			if (ReadArrayStart(ObjectType.PrimitiveTypedArray, out int arraySize) && arraySize > 0)
			{
				array = new bool[arraySize];
				for (int i = 0; i < arraySize; i++)
				{
					array[i] = ReadBoolean();
				}
			}
			return array;
		}

		internal byte[] ReadByteArray()
		{
			byte[] result = null;
			if (ReadArrayStart(ObjectType.PrimitiveTypedArray, out int arraySize))
			{
				result = ReadBytes(arraySize);
			}
			return result;
		}

		internal float[] ReadFloatArray()
		{
			float[] array = null;
			if (ReadArrayStart(ObjectType.PrimitiveTypedArray, out int arraySize) && arraySize > 0)
			{
				array = new float[arraySize];
				for (int i = 0; i < arraySize; i++)
				{
					array[i] = ReadSingle();
				}
			}
			return array;
		}

		internal double[] ReadDoubleArray()
		{
			double[] array = null;
			if (ReadArrayStart(ObjectType.PrimitiveTypedArray, out int arraySize) && arraySize > 0)
			{
				array = new double[arraySize];
				for (int i = 0; i < arraySize; i++)
				{
					array[i] = ReadDouble();
				}
			}
			return array;
		}

		internal char[] ReadCharArray()
		{
			char[] array = null;
			if (ReadArrayStart(ObjectType.PrimitiveTypedArray, out int arraySize) && arraySize > 0)
			{
				array = new char[arraySize];
				for (int i = 0; i < arraySize; i++)
				{
					array[i] = ReadChar();
				}
			}
			return array;
		}

		internal int[] ReadInt32Array()
		{
			int[] array = null;
			if (ReadArrayStart(ObjectType.PrimitiveTypedArray, out int arraySize) && arraySize > 0)
			{
				array = new int[arraySize];
				for (int i = 0; i < arraySize; i++)
				{
					array[i] = ReadInt32();
				}
			}
			return array;
		}

		internal long[] ReadInt64Array()
		{
			long[] array = null;
			if (ReadArrayStart(ObjectType.PrimitiveTypedArray, out int arraySize) && arraySize > 0)
			{
				array = new long[arraySize];
				for (int i = 0; i < arraySize; i++)
				{
					array[i] = ReadInt64();
				}
			}
			return array;
		}

		public override bool ReadBoolean()
		{
			return ReadByte() != 0;
		}

		internal Guid ReadGuid()
		{
			return new Guid(base.ReadBytes(16));
		}

		public override decimal ReadDecimal()
		{
			byte b = 0;
			int[] array = new int[4];
			b = base.ReadByte();
			for (int i = 0; i < 3; i++)
			{
				if ((b & (1 << i * 2)) != 0)
				{
					if ((b & (2 << i * 2)) != 0)
					{
						array[i] = Read7BitEncodedInt();
					}
					else
					{
						array[i] = base.ReadInt32();
					}
				}
			}
			if ((b & 0x40) != 0)
			{
				array[3] = (base.ReadByte() & 0xFF);
				array[3] <<= 16;
			}
			if ((b & 0x80) != 0)
			{
				array[3] |= int.MinValue;
			}
			return new decimal(array);
		}

		public override string ReadString()
		{
			return ReadString(checkforNull: true);
		}

		internal string ReadString(bool checkforNull)
		{
			if (checkforNull && ReadObjectType() == ObjectType.Null)
			{
				return null;
			}
			return base.ReadString();
		}

		internal DateTime ReadDateTime()
		{
			return new DateTime(ReadInt64());
		}

		internal DateTime ReadDateTimeWithKind()
		{
			DateTimeKind kind = (DateTimeKind)ReadByte();
			return DateTime.SpecifyKind(new DateTime(ReadInt64()), kind);
		}

		internal DateTimeOffset ReadDateTimeOffset()
		{
			DateTime dateTime = ReadDateTime();
			TimeSpan offset = ReadTimeSpan();
			return new DateTimeOffset(dateTime, offset);
		}

		internal TimeSpan ReadTimeSpan()
		{
			return new TimeSpan(ReadInt64());
		}

		internal int ReadEnum()
		{
			return Read7BitEncodedInt();
		}

		internal Token ReadToken()
		{
			return (Token)ReadByte();
		}

		internal ObjectType ReadObjectType()
		{
			return (ObjectType)Read7BitEncodedInt();
		}

		private MemberName ReadMemberName()
		{
			return (MemberName)Read7BitEncodedInt();
		}

		internal Declaration ReadDeclaration()
		{
			ObjectType type = ReadObjectType();
			ObjectType baseType = ReadObjectType();
			int num = Read7BitEncodedInt();
			List<MemberInfo> list = new List<MemberInfo>(num);
			for (int i = 0; i < num; i++)
			{
				list.Add(new MemberInfo(ReadMemberName(), ReadObjectType(), ReadToken(), ReadObjectType()));
			}
			return new Declaration(type, baseType, list);
		}

		internal void SkipString()
		{
			if (ReadObjectType() != 0)
			{
				SkipBytes(Read7BitEncodedInt());
			}
		}

		internal void SkipBytes(int bytesToSkip)
		{
			if (bytesToSkip > 0)
			{
				Stream baseStream = BaseStream;
				if (baseStream.CanSeek)
				{
					baseStream.Seek(bytesToSkip, SeekOrigin.Current);
				}
				else
				{
					ReadBytes(bytesToSkip);
				}
			}
		}

		internal void SkipMultiByteInt()
		{
			Read7BitEncodedInt();
		}

		internal void SkipTypedArray(int elementSize)
		{
			int num = Read7BitEncodedInt();
			SkipBytes(num * elementSize);
		}

		internal void Seek(long newPosition, SeekOrigin seekOrigin)
		{
			Stream baseStream = BaseStream;
			if (baseStream.CanSeek)
			{
				baseStream.Seek(newPosition, seekOrigin);
			}
			else
			{
				Global.Tracer.Assert(condition: false, "Seek not supported for this stream.");
			}
		}
	}
}
