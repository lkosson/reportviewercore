using System.IO;

namespace Microsoft.ReportingServices.ProgressivePackaging
{
	internal class ImageResponseMessageWriter
	{
		private IMessageWriter m_writer;

		public ImageResponseMessageWriter(IMessageWriter writer)
		{
			m_writer = writer;
		}

		public void WriteElement(ImageResponseMessageElement messageElement)
		{
			using (Stream output = m_writer.CreateWritableStream("getExternalImagesResponse"))
			{
				BinaryWriter binaryWriter = new BinaryWriter(output, MessageUtil.StringEncoding);
				messageElement.Write(binaryWriter);
				binaryWriter.Flush();
			}
		}
	}
}
