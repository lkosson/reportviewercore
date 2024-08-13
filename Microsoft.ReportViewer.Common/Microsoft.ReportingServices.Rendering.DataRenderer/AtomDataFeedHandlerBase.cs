using System.Collections.Generic;
using Microsoft.ReportingServices.OnDemandReportRendering;

namespace Microsoft.ReportingServices.Rendering.DataRenderer;

internal class AtomDataFeedHandlerBase : HandlerBase
{
	protected AtomDataFeedVisitor m_visitor;

	private List<string> m_definitionPathSteps;

	internal AtomDataFeedHandlerBase(AtomDataFeedVisitor visitor, List<string> definitionPathSteps)
	{
		m_visitor = visitor;
		m_definitionPathSteps = definitionPathSteps;
	}

	protected bool IsPartOfDefinitionPathSteps(string definitionPath)
	{
		foreach (string definitionPathStep in m_definitionPathSteps)
		{
			if (definitionPathStep == definitionPath)
			{
				return true;
			}
		}
		return false;
	}

	protected void ValidateCurrentRow(bool walk, bool output)
	{
		if (!walk && output)
		{
			m_visitor.DiscardRow();
		}
	}

	public override void OnTablixBegin(Tablix tablix, ref bool walkTablix, bool outputTablix)
	{
		bool flag = walkTablix;
		if (!flag)
		{
			base.OnTablixBegin(tablix, ref walkTablix, outputTablix);
		}
		if (walkTablix)
		{
			if (!flag)
			{
				walkTablix = IsPartOfDefinitionPathSteps(tablix.DefinitionPath);
			}
			ValidateCurrentRow(walkTablix, outputTablix);
		}
	}

	public override void OnTablixMemberBegin(TablixMember tablixMember, ref bool walkThisMember, bool outputThisMember)
	{
		bool flag = walkThisMember;
		if (!flag)
		{
			base.OnTablixMemberBegin(tablixMember, ref walkThisMember, outputThisMember);
		}
		if (walkThisMember && tablixMember.Group != null && tablixMember.IsColumn)
		{
			if (!flag)
			{
				walkThisMember = IsPartOfDefinitionPathSteps(tablixMember.DefinitionPath);
			}
			ValidateCurrentRow(walkThisMember, outputThisMember);
		}
	}

	public override void OnChartBegin(Chart chart, bool outputChart, ref bool walkChart)
	{
		bool flag = walkChart;
		if (!flag)
		{
			base.OnChartBegin(chart, outputChart, ref walkChart);
		}
		if (walkChart)
		{
			if (!flag)
			{
				walkChart = IsPartOfDefinitionPathSteps(chart.DefinitionPath);
			}
			ValidateCurrentRow(walkChart, outputChart);
		}
	}

	public override void OnGaugePanelBegin(GaugePanel gaugePanel, bool outputGaugePanelData, ref bool walkGaugePanel)
	{
		base.OnGaugePanelBegin(gaugePanel, outputGaugePanelData, ref walkGaugePanel);
		if (walkGaugePanel)
		{
			walkGaugePanel = IsPartOfDefinitionPathSteps(gaugePanel.DefinitionPath);
			ValidateCurrentRow(walkGaugePanel, outputGaugePanelData);
		}
	}

	public override void OnMapBegin(Map map, bool outputMap, ref bool walkMap)
	{
		bool flag = walkMap;
		if (!flag)
		{
			base.OnMapBegin(map, outputMap, ref walkMap);
		}
		if (walkMap)
		{
			if (!flag)
			{
				walkMap = IsPartOfDefinitionPathSteps(map.DefinitionPath);
			}
			ValidateCurrentRow(walkMap, outputMap);
		}
	}
}
