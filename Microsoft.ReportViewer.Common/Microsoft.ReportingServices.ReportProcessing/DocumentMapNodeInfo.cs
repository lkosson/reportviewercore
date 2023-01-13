using System.Security.Permissions;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal sealed class DocumentMapNodeInfo
	{
		private string m_id;

		private string m_label;

		private DocumentMapNodeInfo[] m_children;

		public string Label => m_label;

		public string Id => m_id;

		public DocumentMapNodeInfo[] Children => m_children;

		internal DocumentMapNodeInfo(DocumentMapNode docMapNode, DocumentMapNodeInfo[] children)
		{
			m_id = docMapNode.Id;
			m_label = docMapNode.Label;
			m_children = children;
		}
	}
}
