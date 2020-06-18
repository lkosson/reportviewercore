using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimDocumentMap : DocumentMap, IDocumentMap, IEnumerator<OnDemandDocumentMapNode>, IDisposable, IEnumerator
	{
		private Microsoft.ReportingServices.ReportProcessing.DocumentMapNode m_oldDocMap;

		private DocumentMapNode m_current;

		private Stack<IEnumerator<Microsoft.ReportingServices.ReportProcessing.DocumentMapNode>> m_nodeInfoStack;

		OnDemandDocumentMapNode IEnumerator<OnDemandDocumentMapNode>.Current => Current;

		public override DocumentMapNode Current
		{
			get
			{
				if (m_current == null)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				return m_current;
			}
		}

		internal ShimDocumentMap(Microsoft.ReportingServices.ReportProcessing.DocumentMapNode aOldDocMap)
		{
			m_oldDocMap = aOldDocMap;
			Reset();
		}

		public override void Close()
		{
			m_oldDocMap = null;
			m_isClosed = true;
		}

		public override void Dispose()
		{
			Close();
		}

		public override bool MoveNext()
		{
			m_current = null;
			if (m_nodeInfoStack == null)
			{
				m_nodeInfoStack = new Stack<IEnumerator<Microsoft.ReportingServices.ReportProcessing.DocumentMapNode>>();
				m_current = new DocumentMapNode(m_oldDocMap.Label, m_oldDocMap.Id, m_nodeInfoStack.Count + 1);
				m_nodeInfoStack.Push(((IEnumerable<Microsoft.ReportingServices.ReportProcessing.DocumentMapNode>)m_oldDocMap.Children).GetEnumerator());
				return true;
			}
			if (m_nodeInfoStack.Count == 0)
			{
				return false;
			}
			while (m_nodeInfoStack.Count > 0 && !m_nodeInfoStack.Peek().MoveNext())
			{
				m_nodeInfoStack.Pop();
			}
			if (m_nodeInfoStack.Count == 0)
			{
				return false;
			}
			Microsoft.ReportingServices.ReportProcessing.DocumentMapNode current = m_nodeInfoStack.Peek().Current;
			m_current = new DocumentMapNode(current.Label, current.Id, m_nodeInfoStack.Count + 1);
			if (current.Children != null && current.Children.Length != 0)
			{
				m_nodeInfoStack.Push(((IEnumerable<Microsoft.ReportingServices.ReportProcessing.DocumentMapNode>)current.Children).GetEnumerator());
			}
			return true;
		}

		public override void Reset()
		{
			m_current = null;
			m_nodeInfoStack = null;
		}
	}
}
