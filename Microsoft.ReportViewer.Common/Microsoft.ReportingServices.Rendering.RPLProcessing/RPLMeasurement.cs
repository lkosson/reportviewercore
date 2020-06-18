namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal class RPLMeasurement : RPLSizes
	{
		protected int m_zindex;

		protected byte m_state;

		protected IRPLObjectFactory m_offsetInfo;

		public int ZIndex
		{
			get
			{
				return m_zindex;
			}
			set
			{
				m_zindex = value;
			}
		}

		public byte State
		{
			get
			{
				return m_state;
			}
			set
			{
				m_state = value;
			}
		}

		internal virtual OffsetInfo OffsetInfo => m_offsetInfo as OffsetInfo;

		public RPLMeasurement()
		{
		}

		public RPLMeasurement(RPLMeasurement measures)
			: base(measures.Top, measures.Left, measures.Height, measures.Width)
		{
			m_state = measures.State;
			m_zindex = measures.ZIndex;
		}

		internal virtual void SetOffset(long offset, RPLContext context)
		{
			if (offset >= 0)
			{
				m_offsetInfo = new OffsetInfo(offset, context);
			}
		}
	}
}
