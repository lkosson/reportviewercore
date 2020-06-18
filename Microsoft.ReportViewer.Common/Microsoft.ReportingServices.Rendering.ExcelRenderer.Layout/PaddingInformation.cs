namespace Microsoft.ReportingServices.Rendering.ExcelRenderer.Layout
{
	internal class PaddingInformation
	{
		private int m_paddingLeft;

		private int m_paddingRight;

		private int m_paddingTop;

		private int m_paddingBottom;

		internal int PaddingLeft => m_paddingLeft;

		internal int PaddingRight => m_paddingRight;

		internal int PaddingTop => m_paddingTop;

		internal int PaddingBottom => m_paddingBottom;

		internal PaddingInformation(int paddingLeft, int paddingRight, int paddingTop, int paddingBottom)
		{
			m_paddingLeft = paddingLeft;
			m_paddingRight = paddingRight;
			m_paddingTop = paddingTop;
			m_paddingBottom = paddingBottom;
		}
	}
}
