namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal abstract class RPLElementPropsDef
	{
		protected string m_id;

		protected RPLStyleProps m_sharedStyle;

		public string ID
		{
			get
			{
				return m_id;
			}
			set
			{
				m_id = value;
			}
		}

		public RPLStyleProps SharedStyle
		{
			get
			{
				return m_sharedStyle;
			}
			set
			{
				m_sharedStyle = value;
			}
		}
	}
}
