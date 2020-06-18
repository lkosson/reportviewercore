namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal class OffsetInfo : IRPLObjectFactory
	{
		protected long m_endOffset = -1L;

		internal RPLContext m_context;

		public long EndOffset => m_endOffset;

		internal OffsetInfo(long endOffset, RPLContext context)
		{
			m_endOffset = endOffset;
			m_context = context;
		}
	}
}
