using Microsoft.ReportingServices.ReportRendering;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimRenderGroups
	{
		private DataRegion.Type m_type;

		private bool m_beforeSubtotal;

		private bool m_afterSubtotal;

		private ListContentCollection m_renderListContents;

		private TableGroupCollection m_renderTableGroups;

		private MatrixMemberCollection m_renderMatrixMembers;

		private Microsoft.ReportingServices.ReportRendering.ChartMemberCollection m_renderChartMembers;

		private Microsoft.ReportingServices.ReportRendering.DataMemberCollection m_renderDataMembers;

		internal int Count
		{
			get
			{
				switch (m_type)
				{
				case DataRegion.Type.List:
					return m_renderListContents.Count;
				case DataRegion.Type.Table:
					return m_renderTableGroups.Count;
				case DataRegion.Type.Matrix:
					if (m_afterSubtotal || m_beforeSubtotal)
					{
						return m_renderMatrixMembers.Count - 1;
					}
					return m_renderMatrixMembers.Count;
				case DataRegion.Type.Chart:
					return m_renderChartMembers.Count;
				case DataRegion.Type.CustomReportItem:
					return m_renderDataMembers.Count;
				default:
					return 0;
				}
			}
		}

		internal int MatrixMemberCollectionCount
		{
			get
			{
				DataRegion.Type type = m_type;
				if (type == DataRegion.Type.Matrix)
				{
					return m_renderMatrixMembers.Count;
				}
				return -1;
			}
		}

		internal Microsoft.ReportingServices.ReportRendering.Group this[int index]
		{
			get
			{
				switch (m_type)
				{
				case DataRegion.Type.List:
					return m_renderListContents[index];
				case DataRegion.Type.Table:
					return m_renderTableGroups[index];
				case DataRegion.Type.Matrix:
					if (m_beforeSubtotal)
					{
						return m_renderMatrixMembers[index + 1];
					}
					return m_renderMatrixMembers[index];
				case DataRegion.Type.Chart:
					return m_renderChartMembers[index];
				case DataRegion.Type.CustomReportItem:
					return m_renderDataMembers[index];
				default:
					return null;
				}
			}
		}

		internal ShimRenderGroups(ListContentCollection renderGroups)
		{
			m_type = DataRegion.Type.List;
			m_renderListContents = renderGroups;
		}

		internal ShimRenderGroups(TableGroupCollection renderGroups)
		{
			m_type = DataRegion.Type.Table;
			m_renderTableGroups = renderGroups;
		}

		internal ShimRenderGroups(MatrixMemberCollection renderGroups, bool beforeSubtotal, bool afterSubtotal)
		{
			m_type = DataRegion.Type.Matrix;
			m_renderMatrixMembers = renderGroups;
			m_beforeSubtotal = beforeSubtotal;
			m_afterSubtotal = afterSubtotal;
		}

		internal ShimRenderGroups(Microsoft.ReportingServices.ReportRendering.ChartMemberCollection renderGroups)
		{
			m_type = DataRegion.Type.Chart;
			m_renderChartMembers = renderGroups;
		}

		internal ShimRenderGroups(Microsoft.ReportingServices.ReportRendering.DataMemberCollection renderGroups)
		{
			m_type = DataRegion.Type.CustomReportItem;
			m_renderDataMembers = renderGroups;
		}
	}
}
