using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer
{
	internal class EscherSpRecord : EscherRecord
	{
		internal static ushort RECORD_ID = 61450;

		internal const string RECORD_DESCRIPTION = "MsofbtSp";

		internal const int FLAG_GROUP = 1;

		internal const int FLAG_CHILD = 2;

		internal const int FLAG_PATRIARCH = 4;

		internal const int FLAG_DELETED = 8;

		internal const int FLAG_OLESHAPE = 16;

		internal const int FLAG_HAVEMASTER = 32;

		internal const int FLAG_FLIPHORIZ = 64;

		internal const int FLAG_FLIPVERT = 128;

		internal const int FLAG_CONNECTOR = 256;

		internal const int FLAG_HAVEANCHOR = 512;

		internal const int FLAG_BACKGROUND = 1024;

		internal const int FLAG_HASSHAPETYPE = 2048;

		private int field_1_shapeId;

		private int field_2_flags;

		internal override int RecordSize => 16;

		internal override string RecordName => "Sp";

		internal virtual int ShapeId
		{
			get
			{
				return field_1_shapeId;
			}
			set
			{
				field_1_shapeId = value;
			}
		}

		internal virtual int Flags
		{
			get
			{
				return field_2_flags;
			}
			set
			{
				field_2_flags = value;
			}
		}

		internal EscherSpRecord()
		{
		}

		internal override int Serialize(BinaryWriter dataWriter)
		{
			dataWriter.Write(getOptions());
			dataWriter.Write(GetRecordId());
			int value = 8;
			dataWriter.Write(value);
			dataWriter.Write(field_1_shapeId);
			dataWriter.Write(field_2_flags);
			return 16;
		}

		internal override ushort GetRecordId()
		{
			return RECORD_ID;
		}
	}
}
