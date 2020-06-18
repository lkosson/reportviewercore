namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLLinePropsDef : RPLItemPropsDef
	{
		private bool m_slant;

		public bool Slant
		{
			get
			{
				return m_slant;
			}
			set
			{
				m_slant = value;
			}
		}

		internal RPLLinePropsDef()
		{
		}
	}
}
