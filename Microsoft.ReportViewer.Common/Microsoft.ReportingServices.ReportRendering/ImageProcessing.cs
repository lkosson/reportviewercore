namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class ImageProcessing : ImageBase
	{
		internal byte[] m_imageData;

		internal string m_mimeType;

		internal Image.Sizings m_sizing;

		internal ImageProcessing DeepClone()
		{
			ImageProcessing imageProcessing = new ImageProcessing();
			if (m_imageData != null)
			{
				imageProcessing.m_imageData = new byte[m_imageData.Length];
				m_imageData.CopyTo(imageProcessing.m_imageData, 0);
			}
			if (m_mimeType != null)
			{
				imageProcessing.m_mimeType = string.Copy(m_mimeType);
			}
			imageProcessing.m_sizing = m_sizing;
			return imageProcessing;
		}
	}
}
