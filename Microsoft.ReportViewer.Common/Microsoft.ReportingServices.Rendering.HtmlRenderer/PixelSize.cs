namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
	internal sealed class PixelSize : ISize
	{
		private float m_sizeInPx;

		public PixelSize(float mSizeInPx)
		{
			m_sizeInPx = mSizeInPx;
		}

		public void Render(IOutputStream outputStream)
		{
			outputStream.Write(m_sizeInPx.ToString());
			outputStream.Write(HTMLElements.m_px);
		}
	}
}
