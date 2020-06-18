using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class RectangleInstance : ReportItemInstance, IShowHideContainer, IIndexInto, IPageItem
	{
		private ReportItemColInstance m_reportItemColInstance;

		[NonSerialized]
		private int m_startPage = -1;

		[NonSerialized]
		private int m_endPage = -1;

		internal ReportItemColInstance ReportItemColInstance
		{
			get
			{
				return m_reportItemColInstance;
			}
			set
			{
				m_reportItemColInstance = value;
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

		internal RectangleInstance(ReportProcessing.ProcessingContext pc, Rectangle reportItemDef, int index)
			: base(pc.CreateUniqueName(), reportItemDef)
		{
			m_instanceInfo = new RectangleInstanceInfo(pc, reportItemDef, this, index);
			pc.Pagination.EnterIgnoreHeight(reportItemDef.StartHidden);
			m_reportItemColInstance = new ReportItemColInstance(pc, reportItemDef.ReportItems);
		}

		internal RectangleInstance()
		{
		}

		internal override int GetDocumentMapUniqueName()
		{
			int linkToChild = ((Rectangle)m_reportItemDef).LinkToChild;
			if (linkToChild >= 0)
			{
				return m_reportItemColInstance.GetReportItemUniqueName(linkToChild);
			}
			return m_uniqueName;
		}

		object IIndexInto.GetChildAt(int index, out NonComputedUniqueNames nonCompNames)
		{
			return ((IIndexInto)m_reportItemColInstance).GetChildAt(index, out nonCompNames);
		}

		protected override object SearchChildren(int targetUniqueName, ref NonComputedUniqueNames nonCompNames, ChunkManager.RenderingChunkManager chunkManager)
		{
			return ((ISearchByUniqueName)m_reportItemColInstance).Find(targetUniqueName, ref nonCompNames, chunkManager);
		}

		void IShowHideContainer.BeginProcessContainer(ReportProcessing.ProcessingContext context)
		{
			context.BeginProcessContainer(m_uniqueName, m_reportItemDef.Visibility);
		}

		void IShowHideContainer.EndProcessContainer(ReportProcessing.ProcessingContext context)
		{
			context.EndProcessContainer(m_uniqueName, m_reportItemDef.Visibility);
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.ReportItemColInstance, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemColInstance));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemInstance, memberInfoList);
		}

		internal override ReportItemInstanceInfo ReadInstanceInfo(IntermediateFormatReader reader)
		{
			Global.Tracer.Assert(m_instanceInfo is OffsetInfo);
			return reader.ReadRectangleInstanceInfo((Rectangle)m_reportItemDef);
		}
	}
}
