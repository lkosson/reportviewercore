using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimChartSeriesCollection : ChartSeriesCollection
	{
		private List<ShimChartSeries> m_series;

		public override ChartSeries this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, Count);
				}
				return m_series[index];
			}
		}

		public override int Count => m_series.Count;

		internal ShimChartSeriesCollection(Chart owner)
			: base(owner)
		{
			m_series = new List<ShimChartSeries>();
			AppendChartSeries(null, owner.SeriesHierarchy.MemberCollection as ShimChartMemberCollection);
		}

		private void AppendChartSeries(ShimChartMember seriesParentMember, ShimChartMemberCollection seriesMembers)
		{
			if (seriesMembers == null)
			{
				m_series.Add(new ShimChartSeries(m_owner, m_series.Count, seriesParentMember));
				return;
			}
			int count = seriesMembers.Count;
			for (int i = 0; i < count; i++)
			{
				ShimChartMember shimChartMember = seriesMembers[i] as ShimChartMember;
				AppendChartSeries(shimChartMember, shimChartMember.Children as ShimChartMemberCollection);
			}
		}

		internal void UpdateCells(ShimChartMember innermostMember)
		{
			if (innermostMember == null || innermostMember.Children != null)
			{
				return;
			}
			if (!innermostMember.IsCategory)
			{
				int memberCellIndex = innermostMember.MemberCellIndex;
				int count = m_series[memberCellIndex].Count;
				for (int i = 0; i < count; i++)
				{
					((ShimChartDataPoint)m_series[memberCellIndex][i]).SetNewContext();
				}
			}
			else
			{
				int memberCellIndex2 = innermostMember.MemberCellIndex;
				int count2 = m_series.Count;
				for (int j = 0; j < count2; j++)
				{
					((ShimChartDataPoint)m_series[j][memberCellIndex2]).SetNewContext();
				}
			}
		}
	}
}
