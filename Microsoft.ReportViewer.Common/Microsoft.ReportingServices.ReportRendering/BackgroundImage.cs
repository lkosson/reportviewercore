namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class BackgroundImage : IImage
	{
		private InternalImage m_internalImage;

		public byte[] ImageData => m_internalImage.ImageData;

		public string MIMEType => m_internalImage.MIMEType;

		public string StreamName => m_internalImage.StreamName;

		internal BackgroundImage(RenderingContext context, Image.SourceType imageSource, object imageValue, string mimeType)
		{
			m_internalImage = new InternalImage(imageSource, mimeType, imageValue, context);
		}
	}
}
