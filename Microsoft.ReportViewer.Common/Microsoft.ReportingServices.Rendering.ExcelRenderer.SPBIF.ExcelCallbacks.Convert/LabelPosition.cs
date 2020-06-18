namespace Microsoft.ReportingServices.Rendering.ExcelRenderer.SPBIF.ExcelCallbacks.Convert
{
	internal sealed class LabelPosition
	{
		private string m_label;

		private long m_position;

		private long m_startPosition;

		internal string Label => m_label;

		internal long Position => m_position;

		internal long StartPosition => m_startPosition;

		internal LabelPosition(string label, long position)
		{
			m_label = label;
			m_position = position;
		}

		internal LabelPosition(string label, long position, long startPosition)
		{
			m_label = label;
			m_position = position;
			m_startPosition = startPosition;
		}
	}
}
