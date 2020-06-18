namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLSubReportPropsDef : RPLItemPropsDef
	{
		private string m_reportName;

		public string ReportName
		{
			get
			{
				return m_reportName;
			}
			set
			{
				m_reportName = value;
			}
		}

		internal RPLSubReportPropsDef()
		{
		}
	}
}
