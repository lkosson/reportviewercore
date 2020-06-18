using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class SubReportInstance : ReportItemInstance, IPageItem
	{
		private ReportInstance m_reportInstance;

		[NonSerialized]
		private int m_startPage = -1;

		[NonSerialized]
		private int m_endPage = -1;

		internal ReportInstance ReportInstance
		{
			get
			{
				return m_reportInstance;
			}
			set
			{
				m_reportInstance = value;
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

		internal SubReportInstance(ReportProcessing.ProcessingContext pc, SubReport reportItemDef, int index)
			: base(pc.CreateUniqueName(), reportItemDef)
		{
			m_instanceInfo = new SubReportInstanceInfo(pc, reportItemDef, this, index);
			pc.Pagination.EnterIgnoreHeight(reportItemDef.StartHidden);
		}

		internal SubReportInstance()
		{
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.ReportInstance, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportInstance));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemInstance, memberInfoList);
		}

		protected override object SearchChildren(int targetUniqueName, ref NonComputedUniqueNames nonCompNames, ChunkManager.RenderingChunkManager chunkManager)
		{
			if (m_reportInstance == null)
			{
				return null;
			}
			return ((ISearchByUniqueName)m_reportInstance).Find(targetUniqueName, ref nonCompNames, chunkManager);
		}

		internal override ReportItemInstanceInfo ReadInstanceInfo(IntermediateFormatReader reader)
		{
			Global.Tracer.Assert(m_instanceInfo is OffsetInfo);
			return reader.ReadSubReportInstanceInfo((SubReport)m_reportItemDef);
		}
	}
}
