namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLHeaderFooterPropsDef : RPLItemPropsDef
	{
		private bool m_printOnFirstPage;

		private bool m_printBetweenSections;

		public bool PrintOnFirstPage
		{
			get
			{
				return m_printOnFirstPage;
			}
			set
			{
				m_printOnFirstPage = value;
			}
		}

		public bool PrintBetweenSections
		{
			get
			{
				return m_printBetweenSections;
			}
			set
			{
				m_printBetweenSections = value;
			}
		}

		internal RPLHeaderFooterPropsDef()
		{
		}
	}
}
