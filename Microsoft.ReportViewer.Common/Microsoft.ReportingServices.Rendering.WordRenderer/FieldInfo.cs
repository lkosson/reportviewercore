using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer
{
	internal abstract class FieldInfo
	{
		internal enum Location
		{
			Start,
			Middle,
			End
		}

		internal const byte StartCode = 19;

		internal const byte MiddleCode = 20;

		internal const byte EndCode = 21;

		protected int m_offset;

		protected Location m_location;

		internal int Offset => m_offset;

		internal abstract byte[] Start
		{
			get;
		}

		internal abstract byte[] Middle
		{
			get;
		}

		internal abstract byte[] End
		{
			get;
		}

		internal FieldInfo(int offset, Location location)
		{
			m_offset = offset;
			m_location = location;
		}

		internal void WriteData(BinaryWriter dataWriter)
		{
			switch (m_location)
			{
			case Location.Start:
				dataWriter.Write(Start);
				break;
			case Location.Middle:
				dataWriter.Write(Middle);
				break;
			case Location.End:
				dataWriter.Write(End);
				break;
			}
		}
	}
}
