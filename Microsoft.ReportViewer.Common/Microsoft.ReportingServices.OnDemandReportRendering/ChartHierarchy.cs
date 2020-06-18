namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartHierarchy : MemberHierarchy<ChartMember>
	{
		private Chart OwnerChart => m_owner as Chart;

		public ChartMemberCollection MemberCollection
		{
			get
			{
				if (m_members == null)
				{
					if (OwnerChart.IsOldSnapshot)
					{
						OwnerChart.ResetMemberCellDefinitionIndex(0);
						m_members = new ShimChartMemberCollection(this, OwnerChart, m_isColumn, null, m_isColumn ? OwnerChart.RenderChart.CategoryMemberCollection : OwnerChart.RenderChart.SeriesMemberCollection);
					}
					else
					{
						OwnerChart.ResetMemberCellDefinitionIndex(0);
						m_members = new InternalChartMemberCollection(this, OwnerChart, null, m_isColumn ? OwnerChart.ChartDef.CategoryMembers : OwnerChart.ChartDef.SeriesMembers);
					}
				}
				return (ChartMemberCollection)m_members;
			}
		}

		internal ChartHierarchy(Chart owner, bool isColumn)
			: base((ReportItem)owner, isColumn)
		{
		}

		internal override void ResetContext()
		{
			if (m_members != null)
			{
				if (OwnerChart.IsOldSnapshot)
				{
					((ShimChartMemberCollection)m_members).UpdateContext();
				}
				else
				{
					((IDataRegionMemberCollection)m_members).SetNewContext();
				}
			}
		}
	}
}
