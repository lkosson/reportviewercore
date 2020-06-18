using Microsoft.ReportingServices.ReportProcessing;
using System.Collections;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class StyleEnumerator : IEnumerator
	{
		private StyleProperties m_sharedProperties;

		private StyleProperties m_nonSharedProperties;

		private int m_total;

		private int m_current = -1;

		public object Current
		{
			get
			{
				if (0 > m_current)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				int num = 0;
				if (m_sharedProperties != null)
				{
					num = m_sharedProperties.Count;
				}
				if (m_current < num)
				{
					return m_sharedProperties[m_current];
				}
				Global.Tracer.Assert(m_nonSharedProperties != null);
				return m_nonSharedProperties[m_current - num];
			}
		}

		internal StyleEnumerator(StyleProperties sharedProps, StyleProperties nonSharedProps)
		{
			m_sharedProperties = sharedProps;
			m_nonSharedProperties = nonSharedProps;
			m_total = 0;
			if (m_sharedProperties != null)
			{
				m_total += m_sharedProperties.Count;
			}
			if (m_nonSharedProperties != null)
			{
				m_total += m_nonSharedProperties.Count;
			}
		}

		public bool MoveNext()
		{
			if (m_current < m_total - 1)
			{
				m_current++;
				return true;
			}
			return false;
		}

		public void Reset()
		{
			m_current = -1;
		}
	}
}
