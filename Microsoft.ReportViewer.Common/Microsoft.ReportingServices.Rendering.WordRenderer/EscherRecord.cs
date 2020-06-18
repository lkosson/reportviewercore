using Microsoft.ReportingServices.Rendering.Utilities;
using System;
using System.Collections;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer
{
	internal abstract class EscherRecord : ICloneable
	{
		internal class EscherRecordHeader
		{
			private ushort options;

			private ushort recordId;

			private int remainingBytes;

			internal virtual ushort Options => options;

			internal virtual ushort RecordId => recordId;

			internal virtual int RemainingBytes => remainingBytes;

			internal EscherRecordHeader()
			{
			}

			internal static EscherRecordHeader readHeader(byte[] data, int offset)
			{
				return new EscherRecordHeader
				{
					options = LittleEndian.getUShort(data, offset),
					recordId = LittleEndian.getUShort(data, offset + 2),
					remainingBytes = LittleEndian.getInt(data, offset + 4)
				};
			}

			public override string ToString()
			{
				return "EscherRecordHeader{options=" + options + ", recordId=" + recordId + ", remainingBytes=" + remainingBytes + "}";
			}
		}

		internal const int HEADER_SIZE = 8;

		private ushort options;

		private ushort recordId;

		internal virtual bool ContainerRecord => (options & 0xF) == 15;

		internal abstract int RecordSize
		{
			get;
		}

		internal virtual IList ChildRecords
		{
			get
			{
				return ArrayList.ReadOnly(new ArrayList());
			}
			set
			{
				throw new ArgumentException("This record does not support child records.");
			}
		}

		internal abstract string RecordName
		{
			get;
		}

		internal virtual short Instance => (short)(options >> 4);

		internal EscherRecord()
		{
		}

		protected internal virtual int readHeader(byte[] data, int offset)
		{
			EscherRecordHeader escherRecordHeader = EscherRecordHeader.readHeader(data, offset);
			options = escherRecordHeader.Options;
			recordId = escherRecordHeader.RecordId;
			return escherRecordHeader.RemainingBytes;
		}

		internal virtual ushort getOptions()
		{
			return options;
		}

		internal virtual void setOptions(ushort options)
		{
			this.options = options;
		}

		internal abstract int Serialize(BinaryWriter dataWriter);

		internal virtual ushort GetRecordId()
		{
			return recordId;
		}

		internal virtual void SetRecordId(ushort recordId)
		{
			this.recordId = recordId;
		}

		public virtual object Clone()
		{
			throw new SystemException("The class " + GetType().FullName + " needs to define a clone method");
		}

		internal virtual EscherRecord GetChild(int index)
		{
			return (EscherRecord)ChildRecords[index];
		}

		internal virtual void Display(StreamWriter w, int indent)
		{
			for (int i = 0; i < indent * 4; i++)
			{
				w.Write(' ');
			}
			w.WriteLine(RecordName);
		}
	}
}
