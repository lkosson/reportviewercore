namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
	internal class PaddingSharedInfo
	{
		private double m_padH;

		private double m_padV;

		private int m_paddingContext;

		internal double PadH => m_padH;

		internal double PadV => m_padV;

		internal int PaddingContext => m_paddingContext;

		internal PaddingSharedInfo(int paddingContext, double padH, double padV)
		{
			m_padH = padH;
			m_padV = padV;
			m_paddingContext = paddingContext;
		}
	}
}
