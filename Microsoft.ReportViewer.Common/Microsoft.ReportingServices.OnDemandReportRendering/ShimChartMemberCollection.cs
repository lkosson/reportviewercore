using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportRendering;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimChartMemberCollection : ChartMemberCollection
	{
		private bool m_isDynamic;

		private bool m_isCategoryGroup;

		private int m_definitionStartIndex = -1;

		private int m_definitionEndIndex = -1;

		public override ChartMember this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, Count);
				}
				return (ChartMember)m_children[index];
			}
		}

		public override int Count => m_children.Length;

		internal ShimChartMemberCollection(IDefinitionPath parentDefinitionPath, Chart owner, bool isCategoryGroup, ShimChartMember parent, Microsoft.ReportingServices.ReportRendering.ChartMemberCollection renderMemberCollection)
			: base(parentDefinitionPath, owner)
		{
			m_isCategoryGroup = isCategoryGroup;
			m_definitionStartIndex = owner.GetCurrentMemberCellDefinitionIndex();
			int count = renderMemberCollection.Count;
			if (renderMemberCollection[0].IsStatic)
			{
				m_isDynamic = false;
				m_children = new ShimChartMember[count];
				for (int i = 0; i < count; i++)
				{
					m_children[i] = new ShimChartMember(this, owner, parent, i, isCategoryGroup, renderMemberCollection[i]);
				}
			}
			else
			{
				m_isDynamic = true;
				m_children = new ShimChartMember[1];
				m_children[0] = new ShimChartMember(this, owner, parent, 0, isCategoryGroup, new ShimRenderGroups(renderMemberCollection));
			}
			m_definitionEndIndex = owner.GetCurrentMemberCellDefinitionIndex();
		}

		internal void UpdateContext()
		{
			if (m_children != null)
			{
				if (m_isCategoryGroup)
				{
					ResetContext(base.OwnerChart.RenderChart.CategoryMemberCollection);
				}
				else
				{
					ResetContext(base.OwnerChart.RenderChart.SeriesMemberCollection);
				}
			}
		}

		internal void ResetContext(Microsoft.ReportingServices.ReportRendering.ChartMemberCollection newRenderMemberCollection)
		{
			if (m_children == null)
			{
				return;
			}
			if (m_isDynamic)
			{
				ShimRenderGroups renderGroups = (newRenderMemberCollection != null) ? new ShimRenderGroups(newRenderMemberCollection) : null;
				((ShimChartMember)m_children[0]).ResetContext(null, renderGroups);
				return;
			}
			for (int i = 0; i < m_children.Length; i++)
			{
				Microsoft.ReportingServices.ReportRendering.ChartMember staticOrSubtotal = newRenderMemberCollection?[i];
				((ShimChartMember)m_children[i]).ResetContext(staticOrSubtotal, null);
			}
		}
	}
}
