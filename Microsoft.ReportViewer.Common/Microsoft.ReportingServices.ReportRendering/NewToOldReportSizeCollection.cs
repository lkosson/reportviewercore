namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class NewToOldReportSizeCollection : ReportSizeCollection
	{
		private ReportSizeCollection m_col;

		public override ReportSize this[int index]
		{
			get
			{
				return new ReportSize(m_col[index]);
			}
			set
			{
				m_col[index] = new ReportSize(value);
			}
		}

		public override int Count => m_col.Count;

		internal NewToOldReportSizeCollection(ReportSizeCollection col)
		{
			m_col = col;
		}
	}
}
