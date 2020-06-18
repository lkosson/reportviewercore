using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class PageSectionInstance : ReportItemInstance, IIndexInto
	{
		private int m_pageNumber;

		private ReportItemColInstance m_reportItemColInstance;

		internal int PageNumber
		{
			get
			{
				return m_pageNumber;
			}
			set
			{
				m_pageNumber = value;
			}
		}

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

		internal PageSectionInstance(ReportProcessing.ProcessingContext pc, int pageNumber, PageSection reportItemDef)
			: base(pc.CreateUniqueName(), reportItemDef)
		{
			m_instanceInfo = new PageSectionInstanceInfo(pc, reportItemDef, this);
			m_pageNumber = pageNumber;
			m_reportItemColInstance = new ReportItemColInstance(pc, reportItemDef.ReportItems);
		}

		internal PageSectionInstance()
		{
		}

		object IIndexInto.GetChildAt(int index, out NonComputedUniqueNames nonCompNames)
		{
			return ((IIndexInto)m_reportItemColInstance).GetChildAt(index, out nonCompNames);
		}

		internal override ReportItemInstanceInfo ReadInstanceInfo(IntermediateFormatReader reader)
		{
			Global.Tracer.Assert(m_instanceInfo is OffsetInfo);
			return reader.ReadPageSectionInstanceInfo((PageSection)m_reportItemDef);
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.PageNumber, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.ReportItemColInstance, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemColInstance));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemInstance, memberInfoList);
		}
	}
}
