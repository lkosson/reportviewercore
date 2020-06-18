using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ListInstance : ReportItemInstance, IPageItem
	{
		private ListContentInstanceList m_listContentInstances;

		private RenderingPagesRangesList m_renderingPages;

		[NonSerialized]
		private int m_numberOfContentsOnThisPage;

		[NonSerialized]
		private int m_startPage = -1;

		[NonSerialized]
		private int m_endPage = -1;

		internal ListContentInstanceList ListContents
		{
			get
			{
				return m_listContentInstances;
			}
			set
			{
				m_listContentInstances = value;
			}
		}

		internal RenderingPagesRangesList ChildrenStartAndEndPages
		{
			get
			{
				return m_renderingPages;
			}
			set
			{
				m_renderingPages = value;
			}
		}

		internal int NumberOfContentsOnThisPage
		{
			get
			{
				return m_numberOfContentsOnThisPage;
			}
			set
			{
				m_numberOfContentsOnThisPage = value;
			}
		}

		int IPageItem.StartPage
		{
			get
			{
				return m_startPage;
			}
			set
			{
				m_startPage = value;
			}
		}

		int IPageItem.EndPage
		{
			get
			{
				return m_endPage;
			}
			set
			{
				m_endPage = value;
			}
		}

		internal ListInstance(ReportProcessing.ProcessingContext pc, List reportItemDef)
			: base(pc.CreateUniqueName(), reportItemDef)
		{
			m_instanceInfo = new ListInstanceInfo(pc, reportItemDef, this);
			m_listContentInstances = new ListContentInstanceList();
			m_renderingPages = new RenderingPagesRangesList();
		}

		internal ListInstance(ReportProcessing.ProcessingContext pc, List reportItemDef, ListContentInstanceList listContentInstances, RenderingPagesRangesList renderingPages)
			: base(pc.CreateUniqueName(), reportItemDef)
		{
			m_instanceInfo = new ListInstanceInfo(pc, reportItemDef, this);
			m_listContentInstances = listContentInstances;
			m_renderingPages = renderingPages;
		}

		internal ListInstance()
		{
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.ListContentInstances, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ListContentInstanceList));
			memberInfoList.Add(new MemberInfo(MemberName.ChildrenStartAndEndPages, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.RenderingPagesRangesList));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemInstance, memberInfoList);
		}

		protected override object SearchChildren(int targetUniqueName, ref NonComputedUniqueNames nonCompNames, ChunkManager.RenderingChunkManager chunkManager)
		{
			int count = m_listContentInstances.Count;
			for (int i = 0; i < count; i++)
			{
				object obj = ((ISearchByUniqueName)m_listContentInstances[i]).Find(targetUniqueName, ref nonCompNames, chunkManager);
				if (obj != null)
				{
					return obj;
				}
			}
			return null;
		}

		internal override ReportItemInstanceInfo ReadInstanceInfo(IntermediateFormatReader reader)
		{
			Global.Tracer.Assert(m_instanceInfo is OffsetInfo);
			return reader.ReadListInstanceInfo((List)m_reportItemDef);
		}
	}
}
