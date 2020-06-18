using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class DocumentMapNode : InstanceInfo
	{
		private string m_id;

		private string m_label;

		private int m_page = -1;

		private DocumentMapNode[] m_children;

		internal string Label
		{
			get
			{
				return m_label;
			}
			set
			{
				m_label = value;
			}
		}

		internal string Id
		{
			get
			{
				return m_id;
			}
			set
			{
				m_id = value;
			}
		}

		internal int Page
		{
			get
			{
				return m_page;
			}
			set
			{
				m_page = value;
			}
		}

		internal DocumentMapNode[] Children
		{
			get
			{
				return m_children;
			}
			set
			{
				m_children = value;
			}
		}

		internal DocumentMapNode()
		{
		}

		internal DocumentMapNode(string id, string label, int page, ArrayList children)
		{
			Global.Tracer.Assert(id != null, "The id of a document map node cannot be null.");
			m_id = id;
			m_label = label;
			m_page = page;
			if (children != null && children.Count > 0)
			{
				m_children = (DocumentMapNode[])children.ToArray(typeof(DocumentMapNode));
			}
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Id, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.Label, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.DocMapPage, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.Children, Token.Array, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.DocumentMapNode));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.InstanceInfo, memberInfoList);
		}
	}
}
