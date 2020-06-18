namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLElementStyle : IRPLStyle
	{
		private RPLStyleProps m_sharedProperties;

		private RPLStyleProps m_nonSharedProperties;

		public object this[byte styleName]
		{
			get
			{
				object obj = null;
				if (m_nonSharedProperties != null)
				{
					obj = m_nonSharedProperties[styleName];
				}
				if (obj == null && m_sharedProperties != null)
				{
					obj = m_sharedProperties[styleName];
				}
				return obj;
			}
		}

		public RPLStyleProps SharedProperties => m_sharedProperties;

		public RPLStyleProps NonSharedProperties => m_nonSharedProperties;

		public RPLElementStyle(RPLStyleProps nonSharedProps, RPLStyleProps sharedProps)
		{
			m_nonSharedProperties = nonSharedProps;
			m_sharedProperties = sharedProps;
		}
	}
}
