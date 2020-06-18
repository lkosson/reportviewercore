using System.IO;

namespace Microsoft.ReportingServices.Rendering.ExcelRenderer.Excel.BIFF8
{
	internal class AreaInfo
	{
		private ushort m_FirstRow;

		private ushort m_LastRow;

		private ushort m_FirstColumn;

		private ushort m_LastColumn;

		internal AreaInfo(ushort firstRow, ushort lastRow, ushort firstCol, ushort lastCol)
		{
			m_FirstRow = firstRow;
			m_LastRow = lastRow;
			m_FirstColumn = firstCol;
			m_LastColumn = lastCol;
		}

		internal AreaInfo(int firstRow, int lastRow, int firstCol, int lastCol)
			: this((ushort)firstRow, (ushort)lastRow, (ushort)firstCol, (ushort)lastCol)
		{
		}

		internal void WriteToStream(BinaryWriter output)
		{
			output.Write(m_FirstRow);
			output.Write(m_LastRow);
			output.Write(m_FirstColumn);
			output.Write(m_LastColumn);
		}
	}
}
