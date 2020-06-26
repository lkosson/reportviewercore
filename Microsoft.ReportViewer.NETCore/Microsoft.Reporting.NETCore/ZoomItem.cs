namespace Microsoft.Reporting.NETCore
{
	internal sealed class ZoomItem
	{
		private ZoomMode m_zoomMode;

		private int m_zoomPercent;

		public ZoomMode ZoomMode => m_zoomMode;

		public int ZoomPercent => m_zoomPercent;

		public ZoomItem(ZoomMode zoomMode, int zoomPercent)
		{
			m_zoomMode = zoomMode;
			m_zoomPercent = zoomPercent;
		}

		public override string ToString()
		{
			if (m_zoomMode == ZoomMode.FullPage)
			{
				return LocalizationHelper.Current.ZoomToWholePage;
			}
			if (m_zoomMode == ZoomMode.PageWidth)
			{
				return LocalizationHelper.Current.ZoomToPageWidth;
			}
			return ReportPreviewStrings.ZoomPercent(m_zoomPercent);
		}
	}
}
