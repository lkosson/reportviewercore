using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimChartSeries : ChartSeries
	{
		private List<ShimChartDataPoint> m_cells;

		private bool m_plotAsLine;

		private ReportEnumProperty<ChartSeriesType> m_chartSeriesType;

		private ReportEnumProperty<ChartSeriesSubtype> m_chartSeriesSubtype;

		public override ChartDataPoint this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, Count);
				}
				return m_cells[index];
			}
		}

		public override int Count => m_cells.Count;

		public override string Name => null;

		public override Style Style => null;

		internal override ActionInfo ActionInfo => null;

		public override CustomPropertyCollection CustomProperties => null;

		public override ReportEnumProperty<ChartSeriesType> Type => m_chartSeriesType;

		public override ReportEnumProperty<ChartSeriesSubtype> Subtype => m_chartSeriesSubtype;

		public override ChartSmartLabel SmartLabel => null;

		public override ChartEmptyPoints EmptyPoints => null;

		public override ReportStringProperty LegendName => null;

		internal override ReportStringProperty LegendText => null;

		internal override ReportBoolProperty HideInLegend => null;

		public override ReportStringProperty ChartAreaName => null;

		public override ReportStringProperty ValueAxisName => null;

		public override ReportStringProperty CategoryAxisName => null;

		public override ChartDataLabel DataLabel => null;

		public override ChartMarker Marker => null;

		internal override ReportStringProperty ToolTip => null;

		public override ReportBoolProperty Hidden => null;

		public override ChartItemInLegend ChartItemInLegend => null;

		public override ChartSeriesInstance Instance => null;

		internal ShimChartSeries(Chart owner, int seriesIndex, ShimChartMember seriesParentMember)
			: base(owner, seriesIndex)
		{
			m_cells = new List<ShimChartDataPoint>();
			m_plotAsLine = seriesParentMember.CurrentRenderChartMember.IsPlotTypeLine();
			GenerateChartDataPoints(seriesParentMember, null, owner.CategoryHierarchy.MemberCollection as ShimChartMemberCollection);
		}

		private void GenerateChartDataPoints(ShimChartMember seriesParentMember, ShimChartMember categoryParentMember, ShimChartMemberCollection categoryMembers)
		{
			if (categoryMembers == null)
			{
				m_cells.Add(new ShimChartDataPoint(m_chart, m_seriesIndex, m_cells.Count, seriesParentMember, categoryParentMember));
				TranslateChartType(m_chart.RenderChartDef.Type, m_chart.RenderChartDef.SubType);
				return;
			}
			int count = categoryMembers.Count;
			for (int i = 0; i < count; i++)
			{
				ShimChartMember shimChartMember = categoryMembers[i] as ShimChartMember;
				GenerateChartDataPoints(seriesParentMember, shimChartMember, shimChartMember.Children as ShimChartMemberCollection);
			}
		}

		private void TranslateChartType(Microsoft.ReportingServices.ReportProcessing.Chart.ChartTypes chartType, Microsoft.ReportingServices.ReportProcessing.Chart.ChartSubTypes chartSubType)
		{
			ChartSeriesType value = ChartSeriesType.Column;
			ChartSeriesSubtype value2 = ChartSeriesSubtype.Plain;
			if (m_plotAsLine && chartType != Microsoft.ReportingServices.ReportProcessing.Chart.ChartTypes.Line)
			{
				value = ChartSeriesType.Line;
				value2 = ChartSeriesSubtype.Plain;
			}
			else
			{
				switch (chartType)
				{
				case Microsoft.ReportingServices.ReportProcessing.Chart.ChartTypes.Area:
					value = ChartSeriesType.Area;
					value2 = TranslateChartSubType(chartSubType);
					break;
				case Microsoft.ReportingServices.ReportProcessing.Chart.ChartTypes.Bar:
					value = ChartSeriesType.Bar;
					value2 = TranslateChartSubType(chartSubType);
					break;
				case Microsoft.ReportingServices.ReportProcessing.Chart.ChartTypes.Column:
					value = ChartSeriesType.Column;
					value2 = TranslateChartSubType(chartSubType);
					break;
				case Microsoft.ReportingServices.ReportProcessing.Chart.ChartTypes.Line:
					value = ChartSeriesType.Line;
					value2 = TranslateChartSubType(chartSubType);
					break;
				case Microsoft.ReportingServices.ReportProcessing.Chart.ChartTypes.Pie:
					value = ChartSeriesType.Shape;
					value2 = ((chartSubType != Microsoft.ReportingServices.ReportProcessing.Chart.ChartSubTypes.Exploded) ? ChartSeriesSubtype.Pie : ChartSeriesSubtype.ExplodedPie);
					break;
				case Microsoft.ReportingServices.ReportProcessing.Chart.ChartTypes.Doughnut:
					value = ChartSeriesType.Shape;
					value2 = ((chartSubType != Microsoft.ReportingServices.ReportProcessing.Chart.ChartSubTypes.Exploded) ? ChartSeriesSubtype.Doughnut : ChartSeriesSubtype.ExplodedDoughnut);
					break;
				case Microsoft.ReportingServices.ReportProcessing.Chart.ChartTypes.Scatter:
					if (chartSubType == Microsoft.ReportingServices.ReportProcessing.Chart.ChartSubTypes.Plain)
					{
						value = ChartSeriesType.Scatter;
						value2 = ChartSeriesSubtype.Plain;
					}
					else
					{
						value = ChartSeriesType.Line;
						value2 = ((chartSubType != Microsoft.ReportingServices.ReportProcessing.Chart.ChartSubTypes.Line) ? ChartSeriesSubtype.Smooth : ChartSeriesSubtype.Plain);
					}
					break;
				case Microsoft.ReportingServices.ReportProcessing.Chart.ChartTypes.Bubble:
					value = ChartSeriesType.Scatter;
					value2 = ChartSeriesSubtype.Bubble;
					break;
				case Microsoft.ReportingServices.ReportProcessing.Chart.ChartTypes.Stock:
					value = ChartSeriesType.Range;
					value2 = TranslateChartSubType(chartSubType);
					break;
				}
			}
			m_chartSeriesType = new ReportEnumProperty<ChartSeriesType>(value);
			m_chartSeriesSubtype = new ReportEnumProperty<ChartSeriesSubtype>(value2);
		}

		private ChartSeriesSubtype TranslateChartSubType(Microsoft.ReportingServices.ReportProcessing.Chart.ChartSubTypes chartSubTypes)
		{
			switch (chartSubTypes)
			{
			case Microsoft.ReportingServices.ReportProcessing.Chart.ChartSubTypes.Candlestick:
				return ChartSeriesSubtype.Candlestick;
			case Microsoft.ReportingServices.ReportProcessing.Chart.ChartSubTypes.HighLowClose:
			case Microsoft.ReportingServices.ReportProcessing.Chart.ChartSubTypes.OpenHighLowClose:
				return ChartSeriesSubtype.Stock;
			case Microsoft.ReportingServices.ReportProcessing.Chart.ChartSubTypes.PercentStacked:
				return ChartSeriesSubtype.PercentStacked;
			case Microsoft.ReportingServices.ReportProcessing.Chart.ChartSubTypes.Smooth:
				return ChartSeriesSubtype.Smooth;
			case Microsoft.ReportingServices.ReportProcessing.Chart.ChartSubTypes.Stacked:
				return ChartSeriesSubtype.Stacked;
			default:
				return ChartSeriesSubtype.Plain;
			}
		}

		internal override void SetNewContext()
		{
		}
	}
}
