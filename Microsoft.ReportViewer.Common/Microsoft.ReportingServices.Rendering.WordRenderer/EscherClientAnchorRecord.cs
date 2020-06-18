using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer
{
	internal class EscherClientAnchorRecord : EscherRecord
	{
		internal static ushort RECORD_ID = 61456;

		internal const string RECORD_DESCRIPTION = "MsofbtClientAnchor";

		private ushort field_1_flag;

		private ushort field_2_col1;

		private ushort field_3_dx1;

		private ushort field_4_row1;

		private ushort field_5_dy1;

		private ushort field_6_col2;

		private ushort field_7_dx2;

		private ushort field_8_row2;

		private ushort field_9_dy2;

		private byte[] remainingData;

		private bool shortRecord;

		internal override int RecordSize
		{
			get
			{
				if (!shortRecord)
				{
					return 26 + ((remainingData != null) ? remainingData.Length : 0);
				}
				return 8 + remainingData.Length;
			}
		}

		internal override string RecordName => "ClientAnchor";

		internal virtual ushort Flag
		{
			get
			{
				return field_1_flag;
			}
			set
			{
				field_1_flag = value;
			}
		}

		internal virtual ushort Col1
		{
			get
			{
				return field_2_col1;
			}
			set
			{
				field_2_col1 = value;
			}
		}

		internal virtual ushort Dx1
		{
			get
			{
				return field_3_dx1;
			}
			set
			{
				field_3_dx1 = value;
			}
		}

		internal virtual ushort Row1
		{
			get
			{
				return field_4_row1;
			}
			set
			{
				field_4_row1 = value;
			}
		}

		internal virtual ushort Dy1
		{
			get
			{
				return field_5_dy1;
			}
			set
			{
				field_5_dy1 = value;
			}
		}

		internal virtual ushort Col2
		{
			get
			{
				return field_6_col2;
			}
			set
			{
				field_6_col2 = value;
			}
		}

		internal virtual ushort Dx2
		{
			get
			{
				return field_7_dx2;
			}
			set
			{
				field_7_dx2 = value;
			}
		}

		internal virtual ushort Row2
		{
			get
			{
				return field_8_row2;
			}
			set
			{
				field_8_row2 = value;
			}
		}

		internal virtual ushort Dy2
		{
			get
			{
				return field_9_dy2;
			}
			set
			{
				field_9_dy2 = value;
			}
		}

		internal virtual byte[] RemainingData
		{
			get
			{
				return remainingData;
			}
			set
			{
				remainingData = value;
			}
		}

		internal virtual bool ShortRecord
		{
			set
			{
				shortRecord = value;
			}
		}

		internal EscherClientAnchorRecord()
		{
			remainingData = new byte[4];
			shortRecord = true;
		}

		internal override int Serialize(BinaryWriter dataWriter)
		{
			if (remainingData == null)
			{
				remainingData = new byte[0];
			}
			dataWriter.Write(getOptions());
			dataWriter.Write(GetRecordId());
			int num = 8;
			if (shortRecord)
			{
				dataWriter.Write(remainingData.Length);
				dataWriter.Write(remainingData);
				return num + remainingData.Length;
			}
			int value = remainingData.Length + 18;
			dataWriter.Write(value);
			dataWriter.Write(field_1_flag);
			dataWriter.Write(field_2_col1);
			dataWriter.Write(field_3_dx1);
			dataWriter.Write(field_4_row1);
			dataWriter.Write(field_5_dy1);
			dataWriter.Write(field_6_col2);
			dataWriter.Write(field_7_dx2);
			dataWriter.Write(field_8_row2);
			dataWriter.Write(field_9_dy2);
			dataWriter.Write(remainingData);
			return num + 18 + remainingData.Length;
		}

		internal override ushort GetRecordId()
		{
			return RECORD_ID;
		}
	}
}
