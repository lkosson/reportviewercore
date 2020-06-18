namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLTextRun : RPLElement
	{
		private RPLSizes m_contentSizes;

		public RPLSizes ContentSizes
		{
			get
			{
				return m_contentSizes;
			}
			set
			{
				m_contentSizes = value;
			}
		}

		internal RPLTextRun()
		{
			m_rplElementProps = new RPLTextRunProps();
			m_rplElementProps.Definition = new RPLTextRunPropsDef();
		}

		internal RPLTextRun(RPLTextRunProps rplElementProps)
			: base(rplElementProps)
		{
		}
	}
}
