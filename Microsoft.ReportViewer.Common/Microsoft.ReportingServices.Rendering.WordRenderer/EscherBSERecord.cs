using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer
{
	internal class EscherBSERecord : EscherRecord
	{
		internal static short RECORD_ID = -4089;

		internal const string RECORD_DESCRIPTION = "MsofbtBSE";

		internal const byte BT_ERROR = 0;

		internal const byte BT_UNKNOWN = 1;

		internal const byte BT_EMF = 2;

		internal const byte BT_WMF = 3;

		internal const byte BT_PICT = 4;

		internal const byte BT_JPEG = 5;

		internal const byte BT_PNG = 6;

		internal const byte BT_DIB = 7;

		private byte field_1_blipTypeWin32;

		private byte field_2_blipTypeMacOS;

		private byte[] field_3_uid;

		private ushort field_4_tag;

		private int field_5_size;

		private int field_6_ref;

		private int field_7_offset;

		private byte field_8_usage;

		private byte field_9_name;

		private byte field_10_unused2;

		private byte field_11_unused3;

		private EscherBSESubRecord field_12_sub;

		private bool _hideSub;

		internal override int RecordSize => 44 + ((field_12_sub != null && !_hideSub) ? field_12_sub.RecordSize : 0);

		internal override string RecordName => "BSE";

		internal virtual byte BlipTypeWin32
		{
			get
			{
				return field_1_blipTypeWin32;
			}
			set
			{
				field_1_blipTypeWin32 = value;
			}
		}

		internal virtual byte BlipTypeMacOS
		{
			get
			{
				return field_2_blipTypeMacOS;
			}
			set
			{
				field_2_blipTypeMacOS = value;
			}
		}

		internal virtual byte[] Uid
		{
			get
			{
				return field_3_uid;
			}
			set
			{
				field_3_uid = value;
			}
		}

		internal virtual ushort Tag
		{
			get
			{
				return field_4_tag;
			}
			set
			{
				field_4_tag = value;
			}
		}

		internal virtual int Size
		{
			get
			{
				return field_5_size;
			}
			set
			{
				field_5_size = value;
			}
		}

		internal virtual int Ref
		{
			get
			{
				return field_6_ref;
			}
			set
			{
				field_6_ref = value;
			}
		}

		internal virtual int Offset
		{
			get
			{
				return field_7_offset;
			}
			set
			{
				field_7_offset = value;
			}
		}

		internal virtual byte Usage
		{
			get
			{
				return field_8_usage;
			}
			set
			{
				field_8_usage = value;
			}
		}

		internal virtual byte Name
		{
			get
			{
				return field_9_name;
			}
			set
			{
				field_9_name = value;
			}
		}

		internal virtual byte Unused2
		{
			get
			{
				return field_10_unused2;
			}
			set
			{
				field_10_unused2 = value;
			}
		}

		internal virtual byte Unused3
		{
			get
			{
				return field_11_unused3;
			}
			set
			{
				field_11_unused3 = value;
			}
		}

		internal virtual EscherBSESubRecord SubRecord
		{
			get
			{
				return field_12_sub;
			}
			set
			{
				field_12_sub = value;
			}
		}

		internal EscherBSERecord()
		{
		}

		internal override int Serialize(BinaryWriter dataWriter)
		{
			dataWriter.Write(getOptions());
			dataWriter.Write(GetRecordId());
			int value = ((field_12_sub != null && !_hideSub) ? field_12_sub.RecordSize : 0) + 36;
			dataWriter.Write(value);
			dataWriter.Write(field_1_blipTypeWin32);
			dataWriter.Write(field_2_blipTypeMacOS);
			dataWriter.Write(field_3_uid);
			dataWriter.Write(field_4_tag);
			dataWriter.Write(field_5_size);
			dataWriter.Write(field_6_ref);
			dataWriter.Write(field_7_offset);
			dataWriter.Write(field_8_usage);
			dataWriter.Write(field_9_name);
			dataWriter.Write(field_10_unused2);
			dataWriter.Write(field_11_unused3);
			if (field_12_sub != null && !_hideSub)
			{
				field_12_sub.Serialize(dataWriter);
			}
			return 44 + ((field_12_sub != null && !_hideSub) ? field_12_sub.RecordSize : 0);
		}

		internal virtual string GetBlipType(byte b)
		{
			switch (b)
			{
			case 0:
				return " ERROR";
			case 1:
				return " UNKNOWN";
			case 2:
				return " EMF";
			case 3:
				return " WMF";
			case 4:
				return " PICT";
			case 5:
				return " JPEG";
			case 6:
				return " PNG";
			case 7:
				return " DIB";
			default:
				if (b < 32)
				{
					return " NotKnown";
				}
				return " Client";
			}
		}

		internal virtual void hideSub()
		{
			_hideSub = true;
		}
	}
}
