using System.Globalization;

namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
	internal sealed class MillimeterSize : ISize
	{
		private float m_sizeInMm;

		public MillimeterSize(float mSizeInMm)
		{
			m_sizeInMm = mSizeInMm;
		}

		public void Render(IOutputStream outputStream)
		{
			outputStream.Write(m_sizeInMm.ToString(CultureInfo.InvariantCulture));
			outputStream.Write(HTMLElements.m_mm);
		}
	}
}
