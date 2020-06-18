namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal abstract class RPLElementProps
	{
		protected RPLElementPropsDef m_definition;

		protected string m_uniqueName;

		protected RPLStyleProps m_nonSharedStyle;

		public virtual RPLElementPropsDef Definition
		{
			get
			{
				return m_definition;
			}
			set
			{
				m_definition = value;
			}
		}

		public RPLElementStyle Style => new RPLElementStyle(m_nonSharedStyle, m_definition.SharedStyle);

		public string UniqueName
		{
			get
			{
				return m_uniqueName;
			}
			set
			{
				m_uniqueName = value;
			}
		}

		public RPLStyleProps NonSharedStyle
		{
			get
			{
				return m_nonSharedStyle;
			}
			set
			{
				m_nonSharedStyle = value;
			}
		}
	}
}
