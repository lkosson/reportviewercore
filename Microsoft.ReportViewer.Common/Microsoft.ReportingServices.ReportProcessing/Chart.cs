using Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel;
using Microsoft.ReportingServices.ReportProcessing.Persistence;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class Chart : Pivot, IRunningValueHolder
	{
		internal enum ChartTypes
		{
			Column,
			Bar,
			Line,
			Pie,
			Scatter,
			Bubble,
			Area,
			Doughnut,
			Stock
		}

		internal enum ChartSubTypes
		{
			Default,
			Stacked,
			PercentStacked,
			Plain,
			Smooth,
			Exploded,
			Line,
			SmoothLine,
			HighLowClose,
			OpenHighLowClose,
			Candlestick
		}

		internal enum ChartPalette
		{
			Default,
			EarthTones,
			Excel,
			GrayScale,
			Light,
			Pastel,
			SemiTransparent
		}

		private ChartHeading m_columns;

		private ChartHeading m_rows;

		private ChartDataPointList m_cellDataPoints;

		private RunningValueInfoList m_cellRunningValues;

		private MultiChart m_multiChart;

		private Legend m_legend;

		private Axis m_categoryAxis;

		private Axis m_valueAxis;

		[Reference]
		private ChartHeading m_staticColumns;

		[Reference]
		private ChartHeading m_staticRows;

		private ChartTypes m_type;

		private ChartSubTypes m_subType;

		private ChartPalette m_palette;

		private ChartTitle m_title;

		private int m_pointWidth;

		private ThreeDProperties m_3dProperties;

		private PlotArea m_plotArea;

		[NonSerialized]
		private ChartExprHost m_exprHost;

		[NonSerialized]
		private IntList m_numberOfSeriesDataPoints;

		[NonSerialized]
		private BoolList m_seriesPlotType;

		[NonSerialized]
		private bool m_hasSeriesPlotTypeLine;

		[NonSerialized]
		private bool m_hasDataValueAggregates;

		internal override ObjectType ObjectType => ObjectType.Chart;

		internal override PivotHeading PivotColumns => m_columns;

		internal override PivotHeading PivotRows => m_rows;

		internal ChartHeading Columns
		{
			get
			{
				return m_columns;
			}
			set
			{
				m_columns = value;
			}
		}

		internal ChartHeading Rows
		{
			get
			{
				return m_rows;
			}
			set
			{
				m_rows = value;
			}
		}

		internal MultiChart MultiChart
		{
			get
			{
				return m_multiChart;
			}
			set
			{
				m_multiChart = value;
			}
		}

		internal ChartDataPointList ChartDataPoints
		{
			get
			{
				return m_cellDataPoints;
			}
			set
			{
				m_cellDataPoints = value;
			}
		}

		internal override RunningValueInfoList PivotCellRunningValues => m_cellRunningValues;

		internal RunningValueInfoList CellRunningValues
		{
			get
			{
				return m_cellRunningValues;
			}
			set
			{
				m_cellRunningValues = value;
			}
		}

		internal Legend Legend
		{
			get
			{
				return m_legend;
			}
			set
			{
				m_legend = value;
			}
		}

		internal Axis CategoryAxis
		{
			get
			{
				return m_categoryAxis;
			}
			set
			{
				m_categoryAxis = value;
			}
		}

		internal Axis ValueAxis
		{
			get
			{
				return m_valueAxis;
			}
			set
			{
				m_valueAxis = value;
			}
		}

		internal override PivotHeading PivotStaticColumns => m_staticColumns;

		internal override PivotHeading PivotStaticRows => m_staticRows;

		internal ChartHeading StaticColumns
		{
			get
			{
				return m_staticColumns;
			}
			set
			{
				m_staticColumns = value;
			}
		}

		internal ChartHeading StaticRows
		{
			get
			{
				return m_staticRows;
			}
			set
			{
				m_staticRows = value;
			}
		}

		internal ChartTypes Type
		{
			get
			{
				return m_type;
			}
			set
			{
				m_type = value;
			}
		}

		internal ChartSubTypes SubType
		{
			get
			{
				return m_subType;
			}
			set
			{
				m_subType = value;
			}
		}

		internal ChartTitle Title
		{
			get
			{
				return m_title;
			}
			set
			{
				m_title = value;
			}
		}

		internal int PointWidth
		{
			get
			{
				return m_pointWidth;
			}
			set
			{
				m_pointWidth = value;
			}
		}

		internal ThreeDProperties ThreeDProperties
		{
			get
			{
				return m_3dProperties;
			}
			set
			{
				m_3dProperties = value;
			}
		}

		internal ChartPalette Palette
		{
			get
			{
				return m_palette;
			}
			set
			{
				m_palette = value;
			}
		}

		internal PlotArea PlotArea
		{
			get
			{
				return m_plotArea;
			}
			set
			{
				m_plotArea = value;
			}
		}

		internal ChartExprHost ChartExprHost => m_exprHost;

		protected override DataRegionExprHost DataRegionExprHost => m_exprHost;

		internal IntList NumberOfSeriesDataPoints
		{
			get
			{
				return m_numberOfSeriesDataPoints;
			}
			set
			{
				m_numberOfSeriesDataPoints = value;
			}
		}

		internal BoolList SeriesPlotType
		{
			get
			{
				return m_seriesPlotType;
			}
			set
			{
				m_seriesPlotType = value;
			}
		}

		internal bool HasSeriesPlotTypeLine
		{
			get
			{
				return m_hasSeriesPlotTypeLine;
			}
			set
			{
				m_hasSeriesPlotTypeLine = value;
			}
		}

		internal bool HasDataValueAggregates
		{
			get
			{
				return m_hasDataValueAggregates;
			}
			set
			{
				m_hasDataValueAggregates = value;
			}
		}

		internal int StaticSeriesCount => ((PivotStaticRows != null) ? ((ChartHeading)PivotStaticRows).Labels : null)?.Count ?? 1;

		internal int StaticCategoryCount => ((PivotStaticColumns != null) ? ((ChartHeading)PivotStaticColumns).Labels : null)?.Count ?? 1;

		internal Chart(ReportItem parent)
			: base(parent)
		{
		}

		internal Chart(int id, ReportItem parent)
			: base(id, parent)
		{
			m_cellDataPoints = new ChartDataPointList();
			m_cellRunningValues = new RunningValueInfoList();
		}

		RunningValueInfoList IRunningValueHolder.GetRunningValueList()
		{
			return m_runningValues;
		}

		void IRunningValueHolder.ClearIfEmpty()
		{
			Global.Tracer.Assert(m_cellRunningValues != null);
			if (m_cellRunningValues.Count == 0)
			{
				m_cellRunningValues = null;
			}
			Global.Tracer.Assert(m_runningValues != null);
			if (m_runningValues.Count == 0)
			{
				m_runningValues = null;
			}
		}

		internal static object[] CreateStyle(ReportProcessing.ProcessingContext pc, Style styleDef, string objectName, int uniqueName)
		{
			object[] array = null;
			if (styleDef != null && styleDef.ExpressionList != null && 0 < styleDef.ExpressionList.Count)
			{
				array = new object[styleDef.ExpressionList.Count];
				ReportProcessing.RuntimeRICollection.EvaluateStyleAttributes(ObjectType.Chart, objectName, styleDef, uniqueName, array, pc);
			}
			return array;
		}

		internal override bool Initialize(InitializationContext context)
		{
			context.ObjectType = ObjectType;
			context.ObjectName = m_name;
			if ((context.Location & LocationFlags.InDetail) != 0 && (context.Location & LocationFlags.InGrouping) == 0)
			{
				context.ErrorContext.Register((m_parent is Table) ? ProcessingErrorCode.rsDataRegionInTableDetailRow : ProcessingErrorCode.rsDataRegionInDetailList, Severity.Error, context.ObjectType, context.ObjectName, null);
			}
			else
			{
				context.RegisterDataRegion(this);
				InternalInitialize(context);
				context.UnRegisterDataRegion(this);
			}
			return false;
		}

		private void InternalInitialize(InitializationContext context)
		{
			context.Location = (context.Location | LocationFlags.InDataSet | LocationFlags.InDataRegion);
			context.ExprHostBuilder.ChartStart(m_name);
			base.Initialize(context);
			context.RegisterRunningValues(m_runningValues);
			if (m_visibility != null)
			{
				m_visibility.Initialize(context, isContainer: true, tableRowCol: false);
			}
			CornerInitialize(context);
			context.Location &= ~LocationFlags.InMatrixCellTopLevelItem;
			bool computedSubtotal = false;
			bool flag = false;
			ColumnsInitialize(context, out int expectedNumberOfCategories, out computedSubtotal);
			flag = computedSubtotal;
			RowsInitialize(context, out int expectedNumberOfSeries, out computedSubtotal);
			if (computedSubtotal)
			{
				flag = true;
			}
			ChartDataPointInitialize(context, expectedNumberOfCategories, expectedNumberOfSeries, flag);
			if (m_visibility != null)
			{
				m_visibility.UnRegisterReceiver(context);
			}
			context.UnRegisterRunningValues(m_runningValues);
			CopyHeadingAggregates(m_rows);
			m_rows.TransferHeadingAggregates();
			CopyHeadingAggregates(m_columns);
			m_columns.TransferHeadingAggregates();
			base.ExprHostID = context.ExprHostBuilder.ChartEnd();
		}

		private void CornerInitialize(InitializationContext context)
		{
			if (m_categoryAxis != null)
			{
				context.ExprHostBuilder.ChartCategoryAxisStart();
				m_categoryAxis.Initialize(context, Axis.Mode.CategoryAxis);
				context.ExprHostBuilder.ChartCategoryAxisEnd();
			}
			if (m_valueAxis != null)
			{
				context.ExprHostBuilder.ChartValueAxisStart();
				m_valueAxis.Initialize(context, Axis.Mode.ValueAxis);
				context.ExprHostBuilder.ChartValueAxisEnd();
			}
			if (m_multiChart != null)
			{
				m_multiChart.Initialize(context);
			}
			if (m_legend != null)
			{
				m_legend.Initialize(context);
			}
			if (m_title != null)
			{
				m_title.Initialize(context);
			}
			if (m_3dProperties != null)
			{
				m_3dProperties.Initialize(context);
			}
			if (m_plotArea != null)
			{
				m_plotArea.Initialize(context);
			}
			if (m_categoryAxis == null || !m_categoryAxis.Scalar)
			{
				return;
			}
			Global.Tracer.Assert(m_columns != null);
			if (m_columns.SubHeading != null)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsMultipleGroupingsOnChartScalarAxis, Severity.Error, context.ObjectType, context.ObjectName, "CategoryAxis");
				return;
			}
			if (StaticColumns != null && StaticColumns.Labels != null)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsStaticGroupingOnChartScalarAxis, Severity.Error, context.ObjectType, context.ObjectName, "CategoryAxis");
				return;
			}
			if (m_columns.Grouping != null && m_columns.Grouping.GroupExpressions != null && 1 < m_columns.Grouping.GroupExpressions.Count)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsMultipleGroupExpressionsOnChartScalarAxis, Severity.Error, context.ObjectType, context.ObjectName, "CategoryAxis");
				return;
			}
			Global.Tracer.Assert(m_columns.SubHeading == null);
			Global.Tracer.Assert(StaticColumns == null || StaticColumns.Labels == null);
			m_columns.ChartGroupExpression = true;
			if (m_columns.Labels != null && m_columns.Grouping != null && m_columns.Grouping.GroupExpressions != null && ReportProcessing.CompareWithInvariantCulture(m_columns.Labels[0].Value, m_columns.Grouping.GroupExpressions[0].Value, ignoreCase: true) != 0)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsLabelExpressionOnChartScalarAxisIsIgnored, Severity.Warning, context.ObjectType, context.ObjectName, "CategoryAxis");
				m_columns.Labels = null;
			}
			if (m_columns.Grouping != null && ChartTypes.Area == m_type)
			{
				Global.Tracer.Assert(m_columns.Grouping.GroupExpressions != null);
				if (m_columns.Sorting == null || m_columns.Sorting.SortExpressions == null || m_columns.Sorting.SortExpressions[0] == null)
				{
					m_columns.Grouping.GroupAndSort = true;
					m_columns.Grouping.SortDirections = new BoolList(1);
					m_columns.Grouping.SortDirections.Add(true);
				}
				else if (ReportProcessing.CompareWithInvariantCulture(m_columns.Grouping.GroupExpressions[0].Value, m_columns.Sorting.SortExpressions[0].Value, ignoreCase: true) != 0)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsUnsortedCategoryInAreaChart, Severity.Error, context.ObjectType, context.ObjectName, "CategoryGrouping", m_columns.Grouping.Name);
				}
			}
			else
			{
				if (m_columns.Grouping == null || (ChartTypes.Line != m_type && (m_type != 0 || !m_hasSeriesPlotTypeLine)))
				{
					return;
				}
				Global.Tracer.Assert(m_columns.Grouping.GroupExpressions != null);
				if (!m_columns.Grouping.GroupAndSort)
				{
					bool flag = false;
					if (m_columns.Sorting == null || m_columns.Sorting.SortExpressions == null || m_columns.Sorting.SortExpressions[0] == null)
					{
						flag = true;
					}
					else if (ReportProcessing.CompareWithInvariantCulture(m_columns.Grouping.GroupExpressions[0].Value, m_columns.Sorting.SortExpressions[0].Value, ignoreCase: true) != 0)
					{
						flag = true;
					}
					if (flag)
					{
						context.ErrorContext.Register(ProcessingErrorCode.rsLineChartMightScatter, Severity.Warning, context.ObjectType, context.ObjectName, "CategoryGrouping");
					}
				}
			}
		}

		private void ColumnsInitialize(InitializationContext context, out int expectedNumberOfCategories, out bool computedSubtotal)
		{
			Global.Tracer.Assert(m_columns != null);
			computedSubtotal = false;
			m_columns.DynamicInitialize(column: true, 0, context);
			m_columns.StaticInitialize(context);
			expectedNumberOfCategories = ((m_staticColumns == null) ? 1 : m_staticColumns.NumberOfStatics);
			if (m_columns.Grouping == null)
			{
				Global.Tracer.Assert(m_columns != null);
				context.SpecialTransferRunningValues(m_columns.RunningValues);
			}
		}

		private void RowsInitialize(InitializationContext context, out int expectedNumberOfSeries, out bool computedSubtotal)
		{
			Global.Tracer.Assert(m_rows != null);
			computedSubtotal = false;
			m_rows.DynamicInitialize(column: false, 0, context);
			m_rows.StaticInitialize(context);
			expectedNumberOfSeries = ((m_staticRows == null) ? 1 : m_staticRows.NumberOfStatics);
			if (m_rows != null && m_rows.Grouping == null)
			{
				context.SpecialTransferRunningValues(m_rows.RunningValues);
			}
			if (!m_hasSeriesPlotTypeLine || m_seriesPlotType == null)
			{
				return;
			}
			if (m_type == ChartTypes.Column)
			{
				if (m_staticRows == null)
				{
					m_type = ChartTypes.Line;
				}
				else
				{
					m_staticRows.PlotTypesLine = m_seriesPlotType;
				}
			}
			else
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsChartSeriesPlotTypeIgnored, Severity.Warning, context.ObjectType, context.ObjectName, "PlotType");
			}
		}

		private void ChartDataPointInitialize(InitializationContext context, int expectedNumberOfCategories, int expectedNumberOfSeries, bool computedCells)
		{
			if (m_cellDataPoints == null || m_cellDataPoints.Count == 0)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsMissingChartDataPoints, Severity.Error, context.ObjectType, context.ObjectName, "ChartData");
				return;
			}
			if (expectedNumberOfSeries != m_numberOfSeriesDataPoints.Count)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsWrongNumberOfChartSeries, Severity.Error, context.ObjectType, context.ObjectName, "ChartSeries");
			}
			bool flag = false;
			for (int i = 0; i < m_numberOfSeriesDataPoints.Count; i++)
			{
				if (flag)
				{
					break;
				}
				if (m_numberOfSeriesDataPoints[i] != expectedNumberOfCategories)
				{
					flag = true;
				}
			}
			if (flag)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsWrongNumberOfChartDataPointsInSeries, Severity.Error, context.ObjectType, context.ObjectName, "ChartSeries");
			}
			int num = expectedNumberOfCategories * expectedNumberOfSeries;
			if (num != m_cellDataPoints.Count)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsWrongNumberOfChartDataPoints, Severity.Error, context.ObjectType, context.ObjectName, "DataPoints", m_cellDataPoints.Count.ToString(CultureInfo.InvariantCulture), num.ToString(CultureInfo.InvariantCulture));
			}
			context.Location |= LocationFlags.InMatrixCell;
			context.MatrixName = m_name;
			context.RegisterTablixCellScope(m_columns.SubHeading == null && m_columns.Grouping == null, m_cellAggregates, m_cellPostSortAggregates);
			for (ChartHeading chartHeading = m_rows; chartHeading != null; chartHeading = chartHeading.SubHeading)
			{
				if (chartHeading.Grouping != null)
				{
					context.Location |= LocationFlags.InGrouping;
					context.RegisterGroupingScopeForTablixCell(chartHeading.Grouping.Name, column: false, chartHeading.Grouping.SimpleGroupExpressions, chartHeading.Aggregates, chartHeading.PostSortAggregates, chartHeading.RecursiveAggregates, chartHeading.Grouping);
				}
			}
			if (m_rows.Grouping != null && m_rows.Subtotal != null && m_staticRows != null)
			{
				context.CopyRunningValues(StaticRows.RunningValues, m_aggregates);
			}
			for (ChartHeading chartHeading = m_columns; chartHeading != null; chartHeading = chartHeading.SubHeading)
			{
				if (chartHeading.Grouping != null)
				{
					context.Location |= LocationFlags.InGrouping;
					context.RegisterGroupingScopeForTablixCell(chartHeading.Grouping.Name, column: true, chartHeading.Grouping.SimpleGroupExpressions, chartHeading.Aggregates, chartHeading.PostSortAggregates, chartHeading.RecursiveAggregates, chartHeading.Grouping);
				}
			}
			if (m_columns.Grouping != null && m_columns.Subtotal != null && m_staticColumns != null)
			{
				context.CopyRunningValues(StaticColumns.RunningValues, m_aggregates);
			}
			Global.Tracer.Assert(m_cellDataPoints != null);
			int count = m_cellDataPoints.Count;
			int num2 = 1;
			switch (m_type)
			{
			case ChartTypes.Stock:
				num2 = ((ChartSubTypes.HighLowClose != m_subType) ? 4 : 3);
				break;
			case ChartTypes.Bubble:
				num2 = 3;
				break;
			case ChartTypes.Scatter:
				num2 = 2;
				break;
			}
			context.RegisterRunningValues(m_cellRunningValues);
			for (int j = 0; j < count; j++)
			{
				Global.Tracer.Assert(m_cellDataPoints[j].DataValues != null);
				if (num2 > m_cellDataPoints[j].DataValues.Count)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsWrongNumberOfDataValues, Severity.Error, context.ObjectType, context.ObjectName, "DataValue", m_cellDataPoints[j].DataValues.Count.ToString(CultureInfo.InvariantCulture), num2.ToString(CultureInfo.InvariantCulture));
				}
				m_cellDataPoints[j].Initialize(context);
			}
			context.UnRegisterRunningValues(m_cellRunningValues);
			if (context.IsRunningValueDirectionColumn())
			{
				m_processingInnerGrouping = ProcessingInnerGroupings.Row;
			}
			for (ChartHeading chartHeading = m_rows; chartHeading != null; chartHeading = chartHeading.SubHeading)
			{
				if (chartHeading.Grouping != null)
				{
					context.UnRegisterGroupingScopeForTablixCell(chartHeading.Grouping.Name, column: false);
				}
			}
			for (ChartHeading chartHeading = m_columns; chartHeading != null; chartHeading = chartHeading.SubHeading)
			{
				if (chartHeading.Grouping != null)
				{
					context.UnRegisterGroupingScopeForTablixCell(chartHeading.Grouping.Name, column: true);
				}
			}
			context.UnRegisterTablixCellScope();
		}

		internal bool IsValidChartSubType()
		{
			if (m_subType == ChartSubTypes.Default)
			{
				ChartTypes type = m_type;
				if (type == ChartTypes.Stock)
				{
					m_subType = ChartSubTypes.HighLowClose;
				}
				else
				{
					m_subType = ChartSubTypes.Plain;
				}
				return true;
			}
			bool result = true;
			switch (m_type)
			{
			case ChartTypes.Column:
			{
				ChartSubTypes subType = m_subType;
				if ((uint)(subType - 1) > 2u)
				{
					result = false;
				}
				break;
			}
			case ChartTypes.Bar:
			{
				ChartSubTypes subType = m_subType;
				if ((uint)(subType - 1) > 2u)
				{
					result = false;
				}
				break;
			}
			case ChartTypes.Line:
			{
				ChartSubTypes subType = m_subType;
				if ((uint)(subType - 1) > 3u)
				{
					result = false;
				}
				break;
			}
			case ChartTypes.Pie:
			{
				ChartSubTypes subType = m_subType;
				if (subType != ChartSubTypes.Plain && subType != ChartSubTypes.Exploded)
				{
					result = false;
				}
				break;
			}
			case ChartTypes.Scatter:
			{
				ChartSubTypes subType = m_subType;
				if (subType != ChartSubTypes.Plain && (uint)(subType - 6) > 1u)
				{
					result = false;
				}
				break;
			}
			case ChartTypes.Bubble:
			{
				ChartSubTypes subType = m_subType;
				if (subType != ChartSubTypes.Plain)
				{
					result = false;
				}
				break;
			}
			case ChartTypes.Area:
			{
				ChartSubTypes subType = m_subType;
				if ((uint)(subType - 1) > 2u)
				{
					result = false;
				}
				break;
			}
			case ChartTypes.Doughnut:
			{
				ChartSubTypes subType = m_subType;
				if (subType != ChartSubTypes.Plain && subType != ChartSubTypes.Exploded)
				{
					result = false;
				}
				break;
			}
			case ChartTypes.Stock:
			{
				ChartSubTypes subType = m_subType;
				if ((uint)(subType - 8) > 2u)
				{
					result = false;
				}
				break;
			}
			default:
				Global.Tracer.Assert(condition: false, string.Empty);
				result = false;
				break;
			}
			return result;
		}

		internal override void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			if (base.ExprHostID < 0)
			{
				return;
			}
			Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null);
			m_exprHost = reportExprHost.ChartHostsRemotable[base.ExprHostID];
			DataRegionSetExprHost(m_exprHost, reportObjectModel);
			if (m_multiChart != null && m_exprHost.MultiChartHost != null)
			{
				m_multiChart.SetExprHost(m_exprHost.MultiChartHost, reportObjectModel);
			}
			IList<ChartDataPointExprHost> chartDataPointHostsRemotable = m_exprHost.ChartDataPointHostsRemotable;
			for (int i = 0; i < m_cellDataPoints.Count; i++)
			{
				ChartDataPoint chartDataPoint = m_cellDataPoints[i];
				if (chartDataPoint != null && chartDataPoint.ExprHostID != -1)
				{
					chartDataPoint.SetExprHost(chartDataPointHostsRemotable[chartDataPoint.ExprHostID], reportObjectModel);
				}
			}
			if (m_categoryAxis != null && m_exprHost.CategoryAxisHost != null)
			{
				m_categoryAxis.SetExprHost(m_exprHost.CategoryAxisHost, reportObjectModel);
			}
			if (m_valueAxis != null && m_exprHost.ValueAxisHost != null)
			{
				m_valueAxis.SetExprHost(m_exprHost.ValueAxisHost, reportObjectModel);
			}
			if (m_title != null && m_exprHost.TitleHost != null)
			{
				m_title.SetExprHost(m_exprHost.TitleHost, reportObjectModel);
			}
			if (m_exprHost.StaticColumnLabelsHost != null)
			{
				m_exprHost.StaticColumnLabelsHost.SetReportObjectModel(reportObjectModel);
			}
			if (m_exprHost.StaticRowLabelsHost != null)
			{
				m_exprHost.StaticRowLabelsHost.SetReportObjectModel(reportObjectModel);
			}
			if (m_legend != null && m_exprHost.LegendHost != null)
			{
				m_legend.SetExprHost(m_exprHost.LegendHost, reportObjectModel);
			}
			if (m_plotArea != null && m_exprHost.PlotAreaHost != null)
			{
				m_plotArea.SetExprHost(m_exprHost.PlotAreaHost, reportObjectModel);
			}
		}

		internal ChartDataPoint GetDataPoint(int seriesIndex, int categoryIndex)
		{
			int index = seriesIndex * StaticCategoryCount + categoryIndex;
			return m_cellDataPoints[index];
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Columns, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ChartHeading));
			memberInfoList.Add(new MemberInfo(MemberName.Rows, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ChartHeading));
			memberInfoList.Add(new MemberInfo(MemberName.CellDataPoints, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ChartDataPointList));
			memberInfoList.Add(new MemberInfo(MemberName.CellRunningValues, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.RunningValueInfoList));
			memberInfoList.Add(new MemberInfo(MemberName.MultiChart, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.MultiChart));
			memberInfoList.Add(new MemberInfo(MemberName.Legend, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.Legend));
			memberInfoList.Add(new MemberInfo(MemberName.CategoryAxis, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.Axis));
			memberInfoList.Add(new MemberInfo(MemberName.ValueAxis, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.Axis));
			memberInfoList.Add(new MemberInfo(MemberName.StaticColumns, Token.Reference, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ChartHeading));
			memberInfoList.Add(new MemberInfo(MemberName.StaticRows, Token.Reference, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ChartHeading));
			memberInfoList.Add(new MemberInfo(MemberName.Type, Token.Enum));
			memberInfoList.Add(new MemberInfo(MemberName.SubType, Token.Enum));
			memberInfoList.Add(new MemberInfo(MemberName.Palette, Token.Enum));
			memberInfoList.Add(new MemberInfo(MemberName.Title, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ChartTitle));
			memberInfoList.Add(new MemberInfo(MemberName.PointWidth, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.ThreeDProperties, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ThreeDProperties));
			memberInfoList.Add(new MemberInfo(MemberName.PlotArea, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.PlotArea));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.Pivot, memberInfoList);
		}
	}
}
