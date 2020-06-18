namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal class RPLActionInfo
	{
		private RPLAction[] m_actions;

		public RPLAction[] Actions
		{
			get
			{
				return m_actions;
			}
			set
			{
				m_actions = value;
			}
		}

		internal RPLActionInfo()
		{
		}

		internal RPLActionInfo(int count)
		{
			m_actions = new RPLAction[count];
		}
	}
}
