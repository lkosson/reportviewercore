namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLRectanglePropsDef : RPLItemPropsDef
	{
		private string m_linkToChildId;

		public string LinkToChildId
		{
			get
			{
				return m_linkToChildId;
			}
			set
			{
				m_linkToChildId = value;
			}
		}

		internal RPLRectanglePropsDef()
		{
		}
	}
}
