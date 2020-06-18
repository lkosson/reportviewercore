using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ImageData
	{
		private byte[] m_data;

		private string m_MIMEType;

		internal string MIMEType => m_MIMEType;

		internal byte[] Data => m_data;

		internal ImageData(byte[] data, string mimeType)
		{
			m_data = data;
			m_MIMEType = mimeType;
		}
	}
}
