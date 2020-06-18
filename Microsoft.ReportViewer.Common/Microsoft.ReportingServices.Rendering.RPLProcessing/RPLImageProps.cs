namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLImageProps : RPLItemProps
	{
		private RPLImageData m_image;

		private RPLActionInfo m_actionInfo;

		private RPLActionInfoWithImageMap[] m_actionImageMapAreas;

		public RPLImageData Image
		{
			get
			{
				return m_image;
			}
			set
			{
				m_image = value;
			}
		}

		public RPLActionInfoWithImageMap[] ActionImageMapAreas
		{
			get
			{
				return m_actionImageMapAreas;
			}
			set
			{
				m_actionImageMapAreas = value;
			}
		}

		public RPLActionInfo ActionInfo
		{
			get
			{
				return m_actionInfo;
			}
			set
			{
				m_actionInfo = value;
			}
		}

		internal RPLImageProps()
		{
		}
	}
}
