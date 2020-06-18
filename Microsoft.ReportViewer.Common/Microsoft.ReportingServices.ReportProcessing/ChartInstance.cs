using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ChartInstance : ReportItemInstance, IPageItem
	{
		private MultiChartInstanceList m_multiCharts;

		[NonSerialized]
		private int m_currentCellOuterIndex;

		[NonSerialized]
		private int m_currentCellInnerIndex;

		[NonSerialized]
		private int m_currentOuterStaticIndex;

		[NonSerialized]
		private int m_currentInnerStaticIndex;

		[NonSerialized]
		private int m_startPage = -1;

		[NonSerialized]
		private int m_endPage = -1;

		internal MultiChartInstanceList MultiCharts
		{
			get
			{
				return m_multiCharts;
			}
			set
			{
				m_multiCharts = value;
			}
		}

		private MultiChartInstance CurrentMultiChart
		{
			get
			{
				if (0 >= m_multiCharts.Count)
				{
					m_multiCharts.Add(new MultiChartInstance((Chart)m_reportItemDef));
				}
				return m_multiCharts[0];
			}
		}

		internal ChartHeadingInstanceList ColumnInstances => CurrentMultiChart.ColumnInstances;

		internal ChartHeadingInstanceList RowInstances => CurrentMultiChart.RowInstances;

		internal ChartDataPointInstancesList DataPoints => CurrentMultiChart.DataPoints;

		internal int DataPointSeriesCount
		{
			get
			{
				if (CurrentMultiChart.DataPoints != null)
				{
					return CurrentMultiChart.DataPoints.Count;
				}
				return 0;
			}
		}

		internal int DataPointCategoryCount
		{
			get
			{
				ChartDataPointInstancesList dataPoints = CurrentMultiChart.DataPoints;
				if (dataPoints != null)
				{
					Global.Tracer.Assert(dataPoints[0] != null);
					return dataPoints[0].Count;
				}
				return 0;
			}
		}

		internal int CurrentCellOuterIndex => m_currentCellOuterIndex;

		internal int CurrentCellInnerIndex => m_currentCellInnerIndex;

		internal int CurrentOuterStaticIndex
		{
			set
			{
				m_currentOuterStaticIndex = value;
			}
		}

		internal int CurrentInnerStaticIndex
		{
			set
			{
				m_currentInnerStaticIndex = value;
			}
		}

		internal ChartHeadingInstanceList InnerHeadingInstanceList
		{
			get
			{
				return CurrentMultiChart.InnerHeadingInstanceList;
			}
			set
			{
				CurrentMultiChart.InnerHeadingInstanceList = value;
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

		internal ChartInstance(ReportProcessing.ProcessingContext pc, Chart reportItemDef)
			: base(pc.CreateUniqueName(), reportItemDef)
		{
			m_instanceInfo = new ChartInstanceInfo(pc, reportItemDef, this);
			pc.Pagination.EnterIgnoreHeight(reportItemDef.StartHidden);
			m_multiCharts = new MultiChartInstanceList();
			pc.QuickFind.Add(base.UniqueName, this);
		}

		internal ChartInstance()
		{
		}

		internal ChartDataPoint GetCellDataPoint(int cellDPIndex)
		{
			if (-1 == cellDPIndex)
			{
				cellDPIndex = GetCurrentCellDPIndex();
			}
			return ((Chart)m_reportItemDef).ChartDataPoints[cellDPIndex];
		}

		internal ChartDataPointInstance AddCell(ReportProcessing.ProcessingContext pc, int currCellDPIndex)
		{
			ChartDataPointInstancesList dataPoints = CurrentMultiChart.DataPoints;
			Chart chart = (Chart)m_reportItemDef;
			int num = (currCellDPIndex < 0) ? GetCurrentCellDPIndex() : currCellDPIndex;
			ChartDataPointInstance chartDataPointInstance = new ChartDataPointInstance(pc, chart, GetCellDataPoint(num), num);
			if (chart.ProcessingInnerGrouping == Pivot.ProcessingInnerGroupings.Column)
			{
				dataPoints[m_currentCellOuterIndex].Add(chartDataPointInstance);
			}
			else
			{
				if (m_currentCellOuterIndex == 0)
				{
					Global.Tracer.Assert(dataPoints.Count == m_currentCellInnerIndex);
					ChartDataPointInstanceList value = new ChartDataPointInstanceList();
					dataPoints.Add(value);
				}
				dataPoints[m_currentCellInnerIndex].Add(chartDataPointInstance);
			}
			m_currentCellInnerIndex++;
			return chartDataPointInstance;
		}

		internal void NewOuterCells()
		{
			ChartDataPointInstancesList dataPoints = CurrentMultiChart.DataPoints;
			if (0 < m_currentCellInnerIndex || dataPoints.Count == 0)
			{
				if (((Chart)m_reportItemDef).ProcessingInnerGrouping == Pivot.ProcessingInnerGroupings.Column)
				{
					ChartDataPointInstanceList value = new ChartDataPointInstanceList();
					dataPoints.Add(value);
				}
				if (0 < m_currentCellInnerIndex)
				{
					m_currentCellOuterIndex++;
					m_currentCellInnerIndex = 0;
				}
			}
		}

		internal int GetCurrentCellDPIndex()
		{
			Chart chart = (Chart)m_reportItemDef;
			int num = (chart.StaticColumns == null || chart.StaticColumns.Labels == null) ? 1 : chart.StaticColumns.Labels.Count;
			if (chart.ProcessingInnerGrouping == Pivot.ProcessingInnerGroupings.Column)
			{
				return m_currentOuterStaticIndex * num + m_currentInnerStaticIndex;
			}
			return m_currentInnerStaticIndex * num + m_currentOuterStaticIndex;
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.MultiCharts, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.MultiChartInstanceList));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemInstance, memberInfoList);
		}

		internal override ReportItemInstanceInfo ReadInstanceInfo(IntermediateFormatReader reader)
		{
			Global.Tracer.Assert(m_instanceInfo is OffsetInfo);
			return reader.ReadChartInstanceInfo((Chart)m_reportItemDef);
		}
	}
}
