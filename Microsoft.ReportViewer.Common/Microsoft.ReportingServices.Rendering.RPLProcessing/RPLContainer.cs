namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal class RPLContainer : RPLItem
	{
		private RPLItemMeasurement[] m_children;

		public RPLItemMeasurement[] Children
		{
			get
			{
				return m_children;
			}
			set
			{
				m_children = value;
			}
		}

		internal RPLContainer()
		{
		}

		internal RPLContainer(long startOffset, RPLContext context, RPLItemMeasurement[] children)
			: base(startOffset, context)
		{
			m_children = children;
		}

		internal RPLContainer(RPLItemProps rplElementProps)
			: base(rplElementProps)
		{
		}

		internal static RPLItem CreateItem(long offset, RPLContext context, RPLItemMeasurement[] children, byte type)
		{
			switch (type)
			{
			case 4:
			case 5:
				return new RPLHeaderFooter(offset, context, children);
			case 6:
				return new RPLBody(offset, context, children);
			case 12:
				return new RPLSubReport(offset, context, children);
			case 10:
				return new RPLRectangle(offset, context, children);
			default:
				return new RPLContainer(offset, context, children);
			}
		}
	}
}
