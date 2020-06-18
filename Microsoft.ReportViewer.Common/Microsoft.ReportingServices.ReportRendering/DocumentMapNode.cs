using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class DocumentMapNode
	{
		private Microsoft.ReportingServices.ReportProcessing.DocumentMapNode m_underlyingNode;

		private DocumentMapNode[] m_childrenWrappers;

		private object m_nonPersistedRenderingInfo;

		public string Label => m_underlyingNode.Label;

		public string UniqueName => m_underlyingNode.Id;

		public int Page => m_underlyingNode.Page;

		public object NonPersistedRenderingInfo
		{
			get
			{
				return m_nonPersistedRenderingInfo;
			}
			set
			{
				m_nonPersistedRenderingInfo = value;
			}
		}

		public DocumentMapNode[] Children
		{
			get
			{
				if (m_childrenWrappers == null)
				{
					Microsoft.ReportingServices.ReportProcessing.DocumentMapNode[] children = m_underlyingNode.Children;
					if (children != null)
					{
						m_childrenWrappers = new DocumentMapNode[children.Length];
						for (int i = 0; i < children.Length; i++)
						{
							m_childrenWrappers[i] = new DocumentMapNode(children[i]);
						}
					}
				}
				return m_childrenWrappers;
			}
		}

		internal DocumentMapNode(Microsoft.ReportingServices.ReportProcessing.DocumentMapNode underlyingNode)
		{
			Global.Tracer.Assert(underlyingNode != null, "The document map node being wrapped cannot be null.");
			m_underlyingNode = underlyingNode;
		}
	}
}
