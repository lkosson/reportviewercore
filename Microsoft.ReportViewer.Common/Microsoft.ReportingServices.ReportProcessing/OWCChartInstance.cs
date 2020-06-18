using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class OWCChartInstance : ReportItemInstance, IPageItem
	{
		[NonSerialized]
		private int m_startPage = -1;

		[NonSerialized]
		private int m_endPage = -1;

		internal OWCChartInstanceInfo InstanceInfo
		{
			get
			{
				if (m_instanceInfo is OffsetInfo)
				{
					Global.Tracer.Assert(condition: false, string.Empty);
					return null;
				}
				return (OWCChartInstanceInfo)m_instanceInfo;
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

		internal OWCChartInstance(ReportProcessing.ProcessingContext pc, OWCChart reportItemDef)
			: base(pc.CreateUniqueName(), reportItemDef)
		{
			m_instanceInfo = new OWCChartInstanceInfo(pc, reportItemDef, this);
			pc.QuickFind.Add(base.UniqueName, this);
		}

		internal OWCChartInstance(ReportProcessing.ProcessingContext pc, OWCChart reportItemDef, VariantList[] chartData)
			: base(pc.CreateUniqueName(), reportItemDef)
		{
			m_instanceInfo = new OWCChartInstanceInfo(pc, reportItemDef, this, chartData);
			pc.QuickFind.Add(base.UniqueName, this);
		}

		internal OWCChartInstance()
		{
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList members = new MemberInfoList();
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemInstance, members);
		}

		internal override ReportItemInstanceInfo ReadInstanceInfo(IntermediateFormatReader reader)
		{
			Global.Tracer.Assert(m_instanceInfo is OffsetInfo);
			return reader.ReadOWCChartInstanceInfo((OWCChart)m_reportItemDef);
		}
	}
}
