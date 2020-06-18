using System.IO;

namespace Microsoft.ReportingServices.Rendering.ExcelRenderer.Excel.BIFF8
{
	internal struct CellRenderingDetails
	{
		private BinaryWriter m_Writer;

		private int m_row;

		private short m_col;

		private ushort m_ixfe;

		internal BinaryWriter Output => m_Writer;

		internal int Row => m_row;

		internal short Column => m_col;

		internal ushort Ixfe => m_ixfe;

		internal void Initialize(BinaryWriter writer, int row, short col, ushort ixfe)
		{
			m_Writer = writer;
			m_row = row;
			m_col = col;
			m_ixfe = ixfe;
		}
	}
}
