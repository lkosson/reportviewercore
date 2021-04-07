using System.Globalization;

namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
	internal sealed class PercentSize : ISize
	{
		private readonly int m_sizeInPercent;

		public PercentSize(int mSizeInPercent)
		{
			m_sizeInPercent = mSizeInPercent;
		}

		public void Render(IOutputStream outputStream)
		{
			outputStream.Write(m_sizeInPercent.ToString(CultureInfo.InvariantCulture));
			outputStream.Write(HTMLElements.m_percentSizeString);
		}
	}
}
