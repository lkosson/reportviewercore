using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class TableHeaderFooterRows : TableRowCollection
	{
		private bool m_repeatOnNewPage;

		public bool RepeatOnNewPage => m_repeatOnNewPage;

		internal TableHeaderFooterRows(Table owner, bool repeatOnNewPage, TableRowList rowDefs, TableRowInstance[] rowInstances)
			: base(owner, rowDefs, rowInstances)
		{
			m_repeatOnNewPage = repeatOnNewPage;
		}
	}
}
