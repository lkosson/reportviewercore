namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal class RPLItem : RPLElement, IRPLItemFactory
	{
		protected long m_startOffset = -1L;

		public override RPLElementProps ElementProps
		{
			get
			{
				if (m_rplElementProps == null)
				{
					m_rplElementProps = RPLReader.ReadElementProps(m_startOffset, m_context);
				}
				return m_rplElementProps;
			}
			set
			{
				m_rplElementProps = value;
			}
		}

		public override RPLElementPropsDef ElementPropsDef
		{
			get
			{
				if (m_rplElementProps == null)
				{
					return RPLReader.ReadElementPropsDef(m_startOffset, m_context);
				}
				return m_rplElementProps.Definition;
			}
		}

		public long StartOffset => m_startOffset;

		public object RPLSource
		{
			get
			{
				if (m_rplElementProps == null)
				{
					return m_startOffset;
				}
				return m_rplElementProps;
			}
		}

		internal RPLItem()
		{
		}

		internal RPLItem(long startOffset, RPLContext context)
			: base(context)
		{
			m_startOffset = startOffset;
		}

		internal RPLItem(RPLItemProps rplElementProps)
			: base(rplElementProps)
		{
		}

		internal static RPLItem CreateItem(long offset, RPLContext context, byte type)
		{
			switch (type)
			{
			case 8:
				return new RPLLine(offset, context);
			case 9:
				return new RPLImage(offset, context);
			case 11:
				return new RPLChart(offset, context);
			case 14:
				return new RPLGaugePanel(offset, context);
			case 21:
				return new RPLMap(offset, context);
			case 7:
				return new RPLTextBox(offset, context);
			default:
				return new RPLItem(offset, context);
			}
		}

		public RPLItem GetRPLItem()
		{
			return this;
		}
	}
}
