using System.Collections.Generic;
using Microsoft.ReportingServices.OnDemandReportRendering;

namespace Microsoft.ReportingServices.Rendering.DataRenderer;

internal class AtomServiceDocumentHandlerSql11 : AtomServiceDocumentHandler
{
	internal AtomServiceDocumentHandlerSql11(AtomSchemaVisitor visitor, Dictionary<string, int> feedNames)
		: base(visitor, feedNames)
	{
	}

	public override void OnChartBegin(Chart chart, bool outputChart, ref bool walkChart)
	{
		bool flag = IsStaticChart(chart);
		if (flag && !m_dynamicElementScope.Peek().IsRoot())
		{
			walkChart = false;
			return;
		}
		base.OnChartBegin(chart, outputChart, ref walkChart);
		if (flag)
		{
			walkChart = false;
		}
	}

	private bool IsStaticChart(Chart chart)
	{
		if (!ContainsDynamicChartMember(chart.SeriesHierarchy.MemberCollection))
		{
			return !ContainsDynamicChartMember(chart.CategoryHierarchy.MemberCollection);
		}
		return false;
	}

	private bool ContainsDynamicChartMember(ChartMemberCollection chartMemberCollection)
	{
		if (chartMemberCollection == null)
		{
			return false;
		}
		foreach (ChartMember item in chartMemberCollection)
		{
			if (!item.IsStatic)
			{
				return true;
			}
			if (ContainsDynamicChartMember(item.Children))
			{
				return true;
			}
		}
		return false;
	}

	public override void OnSubReportBegin(SubReport subReport, ref bool walkSubreport)
	{
		base.OnSubReportBegin(subReport, ref walkSubreport);
		if (walkSubreport)
		{
			OnDynamicMemberBegin(subReport.DefinitionPath, subReport.DataElementName, subReport.Name);
		}
	}

	public override void OnSubReportEnd(SubReport subReport)
	{
		base.OnSubReportEnd(subReport);
		m_dynamicElementScope.Pop();
	}

	public override void OnTopLevelDataRegionOrMap(ReportItem reportItem)
	{
		base.OnTopLevelDataRegionOrMap(reportItem);
		OnDynamicMemberBegin(reportItem.DefinitionPath, reportItem.DataElementName, reportItem.Name);
		m_dynamicElementScope.Pop();
	}
}
