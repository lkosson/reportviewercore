namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLSubReportProps : RPLItemProps
	{
		private string m_language;

		public string Language
		{
			get
			{
				return m_language;
			}
			set
			{
				m_language = value;
			}
		}

		internal RPLSubReportProps()
		{
		}
	}
}
