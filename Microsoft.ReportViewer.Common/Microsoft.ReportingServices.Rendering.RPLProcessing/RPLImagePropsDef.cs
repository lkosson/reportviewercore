namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLImagePropsDef : RPLItemPropsDef
	{
		private RPLFormat.Sizings m_sizing;

		public RPLFormat.Sizings Sizing
		{
			get
			{
				return m_sizing;
			}
			set
			{
				m_sizing = value;
			}
		}

		internal RPLImagePropsDef()
		{
		}
	}
}
