namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal abstract class RPLElement
	{
		internal RPLContext m_context;

		protected RPLElementProps m_rplElementProps;

		public virtual RPLElementProps ElementProps
		{
			get
			{
				return m_rplElementProps;
			}
			set
			{
				m_rplElementProps = value;
			}
		}

		public virtual RPLElementPropsDef ElementPropsDef
		{
			get
			{
				if (m_rplElementProps != null)
				{
					return m_rplElementProps.Definition;
				}
				return null;
			}
		}

		protected RPLElement()
		{
		}

		internal RPLElement(RPLContext context)
		{
			m_context = context;
		}

		protected RPLElement(RPLElementProps rplElementProps)
		{
			m_rplElementProps = rplElementProps;
		}
	}
}
