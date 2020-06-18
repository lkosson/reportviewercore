namespace Microsoft.ReportingServices.Rendering.ExcelRenderer.Excel.BIFF8
{
	internal class PrintTitleInfo
	{
		private ushort m_externSheetIndex;

		private ushort m_currentSheetIndex;

		private ushort m_firstRow;

		private ushort m_lastRow;

		internal ushort ExternSheetIndex => m_externSheetIndex;

		internal ushort CurrentSheetIndex => m_currentSheetIndex;

		internal ushort FirstRow => m_firstRow;

		internal ushort LastRow => m_lastRow;

		internal PrintTitleInfo(ushort externSheetIndex, ushort currentSheetIndex, ushort firstRow, ushort lastRow)
		{
			m_externSheetIndex = externSheetIndex;
			m_currentSheetIndex = currentSheetIndex;
			m_firstRow = firstRow;
			m_lastRow = lastRow;
		}
	}
}
