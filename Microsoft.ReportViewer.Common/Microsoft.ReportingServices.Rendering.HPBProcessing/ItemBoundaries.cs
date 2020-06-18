namespace Microsoft.ReportingServices.Rendering.HPBProcessing
{
	internal sealed class ItemBoundaries
	{
		private long m_startOffset;

		private long m_endOffset;

		internal long StartOffset => m_startOffset;

		internal long EndOffset => m_endOffset;

		internal ItemBoundaries(long start, long end)
		{
			m_startOffset = start;
			m_endOffset = end;
		}
	}
}
