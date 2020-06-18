namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal class RPLItemMeasurement : RPLMeasurement
	{
		private IRPLItemFactory m_rplElement;

		public RPLItem Element
		{
			get
			{
				if (m_rplElement is OffsetItemInfo)
				{
					m_rplElement = m_rplElement.GetRPLItem();
				}
				return (RPLItem)m_rplElement;
			}
			set
			{
				m_rplElement = value;
			}
		}

		internal override OffsetInfo OffsetInfo => m_rplElement as OffsetItemInfo;

		public RPLItemMeasurement()
		{
		}

		public RPLItemMeasurement(RPLMeasurement measurement)
		{
			m_top = measurement.Top;
			m_left = measurement.Left;
			m_height = measurement.Height;
			m_width = measurement.Width;
			m_state = measurement.State;
			m_zindex = measurement.ZIndex;
		}

		internal RPLItemMeasurement(RPLItem rplElement)
		{
			m_rplElement = rplElement;
		}

		internal override void SetOffset(long offset, RPLContext context)
		{
			if (offset >= 0)
			{
				m_rplElement = new OffsetItemInfo(offset, context);
			}
		}
	}
}
