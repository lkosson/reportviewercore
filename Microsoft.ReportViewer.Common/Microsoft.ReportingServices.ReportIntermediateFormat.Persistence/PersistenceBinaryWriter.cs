using System;
using System.IO;

namespace Microsoft.ReportingServices.ReportIntermediateFormat.Persistence
{
	internal class PersistenceBinaryWriter : BinaryWriter
	{
		private static int m_MaxEncVal = 2097152;

		internal PersistenceBinaryWriter(Stream str)
			: base(str)
		{
		}

		internal void WriteNull()
		{
			base.Write((byte)0);
		}

		internal void WriteEnum(int value)
		{
			Write7BitEncodedInt(value);
		}

		internal void Write(DateTime dateTime, Token token)
		{
			if (token == Token.DateTimeWithKind)
			{
				((BinaryWriter)this).Write((byte)dateTime.Kind);
			}
			((BinaryWriter)this).Write(dateTime.Ticks);
		}

		internal void Write(DateTimeOffset dateTimeOffset)
		{
			Write(dateTimeOffset.DateTime, Token.DateTime);
			Write(dateTimeOffset.Offset);
		}

		internal void Write(TimeSpan timeSpan)
		{
			((BinaryWriter)this).Write(timeSpan.Ticks);
		}

		public override void Write(bool value)
		{
			if (value)
			{
				base.Write((byte)1);
			}
			else
			{
				base.Write((byte)0);
			}
		}

		internal void Write(Guid guid)
		{
			base.Write(guid.ToByteArray());
		}

		public override void Write(decimal value)
		{
			int num = 0;
			int[] bits = decimal.GetBits(value);
			num = (byte)(bits[3] >> 24);
			bits[3] &= int.MaxValue;
			bits[3] >>= 16;
			for (int i = 0; i < 3; i++)
			{
				if (bits[i] != 0)
				{
					num = ((bits[i] > m_MaxEncVal || bits[i] <= 0) ? (num | (1 << i * 2)) : (num | (3 << i * 2)));
				}
			}
			if (bits[3] != 0)
			{
				num |= 0x40;
			}
			base.Write((byte)num);
			for (int i = 0; i < 3; i++)
			{
				if (bits[i] != 0)
				{
					if (bits[i] <= m_MaxEncVal && bits[i] > 0)
					{
						Write7BitEncodedInt(bits[i]);
					}
					else
					{
						base.Write(bits[i]);
					}
				}
			}
			if (bits[3] != 0)
			{
				base.Write((byte)bits[3]);
			}
		}

		public override void Write(string value)
		{
			Write(value, writeObjType: true);
		}

		internal void Write(string str, bool writeObjType)
		{
			if (str == null)
			{
				WriteNull();
				return;
			}
			if (writeObjType)
			{
				Write(ObjectType.String);
			}
			base.Write(str);
		}

		internal void WriteDictionaryStart(ObjectType type, int size)
		{
			Write7BitEncodedInt((int)type);
			Write7BitEncodedInt(size);
		}

		internal void WriteListStart(ObjectType type, int size)
		{
			Write7BitEncodedInt((int)type);
			Write7BitEncodedInt(size);
		}

		internal void WriteArrayStart(ObjectType type, int size)
		{
			Write7BitEncodedInt((int)type);
			Write7BitEncodedInt(size);
		}

		internal void Write2DArrayStart(ObjectType type, int xSize, int ySize)
		{
			Write7BitEncodedInt((int)type);
			Write7BitEncodedInt(xSize);
			Write7BitEncodedInt(ySize);
		}

		internal void Write(ObjectType type)
		{
			Write7BitEncodedInt((int)type);
		}

		internal void Write(MemberName name)
		{
			Write7BitEncodedInt((int)name);
		}

		internal void Write(Token token)
		{
			((BinaryWriter)this).Write((byte)token);
		}

		internal void Write(Declaration decl)
		{
			Write(decl.ObjectType);
			Write(decl.BaseObjectType);
			int count = decl.MemberInfoList.Count;
			Write7BitEncodedInt(count);
			for (int i = 0; i < count; i++)
			{
				MemberInfo member = decl.MemberInfoList[i];
				Write(member);
			}
		}

		internal void Write(MemberInfo member)
		{
			Write(member.MemberName);
			Write(member.ObjectType);
			Write(member.Token);
			Write(member.ContainedType);
		}

		internal void Write(float[] array)
		{
			if (array == null)
			{
				WriteNull();
				return;
			}
			WriteArrayStart(ObjectType.PrimitiveTypedArray, array.Length);
			for (int i = 0; i < array.Length; i++)
			{
				((BinaryWriter)this).Write(array[i]);
			}
		}

		internal void Write(int[] array)
		{
			if (array == null)
			{
				WriteNull();
				return;
			}
			WriteArrayStart(ObjectType.PrimitiveTypedArray, array.Length);
			for (int i = 0; i < array.Length; i++)
			{
				((BinaryWriter)this).Write(array[i]);
			}
		}

		internal void Write(long[] array)
		{
			if (array == null)
			{
				WriteNull();
				return;
			}
			WriteArrayStart(ObjectType.PrimitiveTypedArray, array.Length);
			for (int i = 0; i < array.Length; i++)
			{
				((BinaryWriter)this).Write(array[i]);
			}
		}

		internal void Write(double[] array)
		{
			if (array == null)
			{
				WriteNull();
				return;
			}
			WriteArrayStart(ObjectType.PrimitiveTypedArray, array.Length);
			for (int i = 0; i < array.Length; i++)
			{
				((BinaryWriter)this).Write(array[i]);
			}
		}

		public override void Write(char[] array)
		{
			if (array == null)
			{
				WriteNull();
				return;
			}
			WriteArrayStart(ObjectType.PrimitiveTypedArray, array.Length);
			for (int i = 0; i < array.Length; i++)
			{
				((BinaryWriter)this).Write(array[i]);
			}
		}

		public override void Write(byte[] array)
		{
			if (array == null)
			{
				WriteNull();
				return;
			}
			WriteArrayStart(ObjectType.PrimitiveTypedArray, array.Length);
			Write(array, 0, array.Length);
		}

		public void Write(bool[] array)
		{
			if (array == null)
			{
				WriteNull();
				return;
			}
			WriteArrayStart(ObjectType.PrimitiveTypedArray, array.Length);
			for (int i = 0; i < array.Length; i++)
			{
				((BinaryWriter)this).Write(array[i]);
			}
		}
	}
}
