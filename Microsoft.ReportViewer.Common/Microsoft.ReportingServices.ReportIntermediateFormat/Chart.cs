using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.RdlExpressions;
using Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class Chart : DataRegion, IPersistable
	{
		private ChartMemberList m_categoryMembers;

		private ChartMemberList m_seriesMembers;

		private ChartSeriesList m_chartSeriesCollection;

		private List<ChartDerivedSeries> m_chartDerivedSeriesCollection;

		private ExpressionInfo m_palette;

		private ExpressionInfo m_paletteHatchBehavior;

		private List<ChartArea> m_chartAreas;

		private List<ChartLegend> m_legends;

		private List<ChartTitle> m_titles;

		private List<ChartCustomPaletteColor> m_customPaletteColors;

		private DataValueList m_codeParameters;

		private ChartBorderSkin m_borderSkin;

		private ChartNoDataMessage m_noDataMessage;

		private ExpressionInfo m_dynamicHeight;

		private ExpressionInfo m_dynamicWidth;

		private bool m_dataValueSequenceRendering;

		private bool m_columnGroupingIsSwitched;

		private bool m_enableCategoryDrilldown;

		[NonSerialized]
		private bool m_hasDataValueAggregates;

		[NonSerialized]
		private bool m_hasSeriesPlotTypeLine;

		[NonSerialized]
		private bool? m_hasStaticColumns;

		[NonSerialized]
		private bool? m_hasStaticRows;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		[NonSerialized]
		private int m_actionOwnerCounter;

		[NonSerialized]
		private ChartExprHost m_chartExprHost;

		internal override Microsoft.ReportingServices.ReportProcessing.ObjectType ObjectType => Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart;

		internal override HierarchyNodeList ColumnMembers => m_categoryMembers;

		internal override HierarchyNodeList RowMembers => m_seriesMembers;

		public override bool IsColumnGroupingSwitched => m_columnGroupingIsSwitched;

		internal override RowList Rows => m_chartSeriesCollection;

		internal ChartMemberList CategoryMembers
		{
			get
			{
				return m_categoryMembers;
			}
			set
			{
				m_categoryMembers = value;
			}
		}

		internal ChartMemberList SeriesMembers
		{
			get
			{
				return m_seriesMembers;
			}
			set
			{
				m_seriesMembers = value;
			}
		}

		internal ChartSeriesList ChartSeriesCollection
		{
			get
			{
				return m_chartSeriesCollection;
			}
			set
			{
				m_chartSeriesCollection = value;
			}
		}

		internal List<ChartDerivedSeries> DerivedSeriesCollection
		{
			get
			{
				return m_chartDerivedSeriesCollection;
			}
			set
			{
				m_chartDerivedSeriesCollection = value;
			}
		}

		internal bool HasStaticColumns
		{
			get
			{
				if (m_hasStaticColumns.HasValue)
				{
					return m_hasStaticColumns.Value;
				}
				if (m_categoryMembers == null || m_categoryMembers.Count == 0)
				{
					return false;
				}
				if (m_categoryMembers.Count > 1)
				{
					m_hasStaticColumns = true;
				}
				ChartMember member = m_categoryMembers[0];
				m_hasStaticColumns = ContainsStatic(member);
				return m_hasStaticColumns.Value;
			}
		}

		internal bool HasStaticRows
		{
			get
			{
				if (m_hasStaticRows.HasValue)
				{
					return m_hasStaticRows.Value;
				}
				if (m_seriesMembers == null || m_seriesMembers.Count == 0)
				{
					return false;
				}
				if (m_seriesMembers.Count > 1)
				{
					m_hasStaticRows = true;
				}
				ChartMember member = m_seriesMembers[0];
				m_hasStaticRows = ContainsStatic(member);
				return m_hasStaticRows.Value;
			}
		}

		internal ExpressionInfo DynamicWidth
		{
			get
			{
				return m_dynamicWidth;
			}
			set
			{
				m_dynamicWidth = value;
			}
		}

		internal ExpressionInfo DynamicHeight
		{
			get
			{
				return m_dynamicHeight;
			}
			set
			{
				m_dynamicHeight = value;
			}
		}

		internal List<ChartArea> ChartAreas
		{
			get
			{
				return m_chartAreas;
			}
			set
			{
				m_chartAreas = value;
			}
		}

		internal List<ChartLegend> Legends
		{
			get
			{
				return m_legends;
			}
			set
			{
				m_legends = value;
			}
		}

		internal List<ChartTitle> Titles
		{
			get
			{
				return m_titles;
			}
			set
			{
				m_titles = value;
			}
		}

		internal ExpressionInfo Palette
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

		internal ExpressionInfo PaletteHatchBehavior
		{
			get
			{
				return m_paletteHatchBehavior;
			}
			set
			{
				m_paletteHatchBehavior = value;
			}
		}

		internal DataValueList CodeParameters
		{
			get
			{
				return m_codeParameters;
			}
			set
			{
				m_codeParameters = value;
			}
		}

		internal List<ChartCustomPaletteColor> CustomPaletteColors
		{
			get
			{
				return m_customPaletteColors;
			}
			set
			{
				m_customPaletteColors = value;
			}
		}

		internal ChartBorderSkin BorderSkin
		{
			get
			{
				return m_borderSkin;
			}
			set
			{
				m_borderSkin = value;
			}
		}

		internal ChartNoDataMessage NoDataMessage
		{
			get
			{
				return m_noDataMessage;
			}
			set
			{
				m_noDataMessage = value;
			}
		}

		internal ChartExprHost ChartExprHost => m_chartExprHost;

		protected override IndexedExprHost UserSortExpressionsHost
		{
			get
			{
				if (m_chartExprHost == null)
				{
					return null;
				}
				return m_chartExprHost.UserSortExpressionsHost;
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

		internal int SeriesCount
		{
			get
			{
				return base.RowCount;
			}
			set
			{
				base.RowCount = value;
			}
		}

		internal int CategoryCount
		{
			get
			{
				return base.ColumnCount;
			}
			set
			{
				base.ColumnCount = value;
			}
		}

		internal bool DataValueSequenceRendering => m_dataValueSequenceRendering;

		internal bool EnableCategoryDrilldown
		{
			get
			{
				return m_enableCategoryDrilldown;
			}
			set
			{
				m_enableCategoryDrilldown = value;
			}
		}

		internal Chart(ReportItem parent)
			: base(parent)
		{
		}

		internal Chart(int id, ReportItem parent)
			: base(id, parent)
		{
		}

		internal void SetColumnGroupingDirection(bool isOuter)
		{
			m_columnGroupingIsSwitched = isOuter;
		}

		private bool ContainsStatic(ChartMember member)
		{
			while (member != null)
			{
				if (member.Grouping == null)
				{
					return true;
				}
				if (member.ChartMembers != null && member.ChartMembers.Count > 0)
				{
					if (member.ChartMembers.Count > 1)
					{
						return true;
					}
					member = member.ChartMembers[0];
				}
				else
				{
					member = null;
				}
			}
			return false;
		}

		internal override bool Initialize(InitializationContext context)
		{
			context.ObjectType = ObjectType;
			context.ObjectName = m_name;
			if ((context.Location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InDetail) != 0 && (context.Location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InGrouping) == 0)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsDataRegionInDetailList, Severity.Error, context.ObjectType, context.ObjectName, null);
			}
			else
			{
				if (!context.RegisterDataRegion(this))
				{
					return false;
				}
				context.Location |= (Microsoft.ReportingServices.ReportPublishing.LocationFlags.InDataSet | Microsoft.ReportingServices.ReportPublishing.LocationFlags.InDataRegion);
				context.ExprHostBuilder.DataRegionStart(Microsoft.ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode.Chart, m_name);
				base.Initialize(context);
				base.ExprHostID = context.ExprHostBuilder.DataRegionEnd(Microsoft.ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode.Chart);
				context.UnRegisterDataRegion(this);
			}
			return false;
		}

		protected override bool InitializeMembers(InitializationContext context)
		{
			bool num = base.InitializeMembers(context);
			if (num)
			{
				_ = m_hasSeriesPlotTypeLine;
			}
			return num;
		}

		protected override void InitializeCorner(InitializationContext context)
		{
			if (m_chartAreas != null)
			{
				for (int i = 0; i < m_chartAreas.Count; i++)
				{
					m_chartAreas[i].Initialize(context);
				}
			}
			if (m_legends != null)
			{
				for (int j = 0; j < m_legends.Count; j++)
				{
					m_legends[j].Initialize(context);
				}
			}
			if (m_titles != null)
			{
				for (int k = 0; k < m_titles.Count; k++)
				{
					m_titles[k].Initialize(context);
				}
			}
			if (m_codeParameters != null)
			{
				m_codeParameters.Initialize("CodeParameters", context);
			}
			if (m_customPaletteColors != null)
			{
				for (int l = 0; l < m_customPaletteColors.Count; l++)
				{
					m_customPaletteColors[l].Initialize(context, l);
				}
			}
			if (m_borderSkin != null)
			{
				m_borderSkin.Initialize(context);
			}
			if (m_noDataMessage != null)
			{
				m_noDataMessage.Initialize(context);
			}
			if (m_palette != null)
			{
				m_palette.Initialize("Palette", context);
				context.ExprHostBuilder.ChartPalette(m_palette);
			}
			if (m_dynamicHeight != null)
			{
				m_dynamicHeight.Initialize("DynamicHeight", context);
				context.ExprHostBuilder.DynamicHeight(m_dynamicHeight);
			}
			if (m_dynamicWidth != null)
			{
				m_dynamicWidth.Initialize("DynamicWidth", context);
				context.ExprHostBuilder.DynamicWidth(m_dynamicWidth);
			}
			if (m_paletteHatchBehavior != null)
			{
				m_paletteHatchBehavior.Initialize("PaletteHatchBehavior", context);
				context.ExprHostBuilder.ChartPaletteHatchBehavior(m_paletteHatchBehavior);
			}
			m_dataValueSequenceRendering = CalculateDataValueSequenceRendering();
		}

		protected override bool ValidateInnerStructure(InitializationContext context)
		{
			if (m_chartSeriesCollection == null || m_chartSeriesCollection.Count == 0)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsMissingChartDataPoints, Severity.Error, context.ObjectType, context.ObjectName, "ChartData");
				return false;
			}
			return true;
		}

		private bool CalculateDataValueSequenceRendering()
		{
			if (m_customProperties != null && m_chartSeriesCollection != null)
			{
				for (int i = 0; i < m_customProperties.Count; i++)
				{
					DataValue dataValue = m_customProperties[i];
					if (dataValue == null)
					{
						continue;
					}
					ExpressionInfo name = dataValue.Name;
					ExpressionInfo value = dataValue.Value;
					if (name == null || value == null || name.IsExpression || value.IsExpression || !(name.StringValue == "__Upgraded2005__") || !(value.StringValue == "__Upgraded2005__"))
					{
						continue;
					}
					for (int j = 0; j < m_chartSeriesCollection.Count; j++)
					{
						ChartSeries chartSeries = m_chartSeriesCollection[j];
						if (chartSeries.Type != null)
						{
							if (chartSeries.Type.IsExpression || (chartSeries.Subtype != null && chartSeries.Subtype.IsExpression))
							{
								return false;
							}
							if (!IsYukonDataRendererType(chartSeries.Type.StringValue, (chartSeries.Subtype != null) ? chartSeries.Subtype.StringValue : null))
							{
								return false;
							}
						}
					}
					return true;
				}
			}
			return false;
		}

		private bool IsYukonDataRendererType(string type, string subType)
		{
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(type, "Column") || Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(type, "Bar") || Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(type, "Line") || Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(type, "Shape") || Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(type, "Scatter") || Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(type, "Area"))
			{
				return true;
			}
			if (Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(type, "Range") && (subType == null || Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(subType, "Stock") || Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(subType, "CandleStick")))
			{
				return true;
			}
			return false;
		}

		internal ChartDataPoint GetDataPoint(int seriesIndex, int categoryIndex)
		{
			return m_chartSeriesCollection[seriesIndex].DataPoints[categoryIndex];
		}

		internal ChartDataPoint GetDataPoint(int cellIndex)
		{
			int index = cellIndex / CategoryCount;
			int index2 = cellIndex % CategoryCount;
			return m_chartSeriesCollection[index].DataPoints[index2];
		}

		internal ChartMember GetChartMember(ChartSeries chartSeries)
		{
			try
			{
				int memberCellIndex = m_chartSeriesCollection.IndexOf(chartSeries);
				return GetChartMember(m_seriesMembers, memberCellIndex);
			}
			catch (Exception e)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(e))
				{
					throw;
				}
				return null;
			}
		}

		internal ChartMember GetChartMember(ChartMemberList chartMemberList, int memberCellIndex)
		{
			foreach (ChartMember chartMember3 in chartMemberList)
			{
				if (chartMember3.ChartMembers == null)
				{
					if (chartMember3.MemberCellIndex == memberCellIndex)
					{
						return chartMember3;
					}
					continue;
				}
				ChartMember chartMember2 = GetChartMember(chartMember3.ChartMembers, memberCellIndex);
				if (chartMember2 != null)
				{
					return chartMember2;
				}
			}
			return null;
		}

		internal List<ChartDerivedSeries> GetChildrenDerivedSeries(string chartSeriesName)
		{
			if (m_chartDerivedSeriesCollection == null)
			{
				return null;
			}
			List<ChartDerivedSeries> list = null;
			foreach (ChartDerivedSeries item in m_chartDerivedSeriesCollection)
			{
				if (Microsoft.ReportingServices.ReportProcessing.ReportProcessing.CompareWithInvariantCulture(item.SourceChartSeriesName, chartSeriesName, ignoreCase: false) == 0)
				{
					if (list == null)
					{
						list = new List<ChartDerivedSeries>();
					}
					list.Add(item);
				}
			}
			return list;
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			Chart chart = (Chart)(context.CurrentDataRegionClone = (Chart)base.PublishClone(context));
			if (m_categoryMembers != null)
			{
				chart.m_categoryMembers = new ChartMemberList(m_categoryMembers.Count);
				foreach (ChartMember categoryMember in m_categoryMembers)
				{
					chart.m_categoryMembers.Add(categoryMember.PublishClone(context, chart));
				}
			}
			if (m_seriesMembers != null)
			{
				chart.m_seriesMembers = new ChartMemberList(m_seriesMembers.Count);
				foreach (ChartMember seriesMember in m_seriesMembers)
				{
					chart.m_seriesMembers.Add(seriesMember.PublishClone(context, chart));
				}
			}
			if (m_chartSeriesCollection != null)
			{
				chart.m_chartSeriesCollection = new ChartSeriesList(m_chartSeriesCollection.Count);
				foreach (ChartSeries item in m_chartSeriesCollection)
				{
					chart.m_chartSeriesCollection.Add((ChartSeries)item.PublishClone(context));
				}
			}
			if (m_chartDerivedSeriesCollection != null)
			{
				chart.m_chartDerivedSeriesCollection = new List<ChartDerivedSeries>(m_chartDerivedSeriesCollection.Count);
				foreach (ChartDerivedSeries item2 in m_chartDerivedSeriesCollection)
				{
					chart.m_chartDerivedSeriesCollection.Add((ChartDerivedSeries)item2.PublishClone(context));
				}
			}
			if (m_chartAreas != null)
			{
				chart.m_chartAreas = new List<ChartArea>(m_chartAreas.Count);
				foreach (ChartArea chartArea in m_chartAreas)
				{
					chart.m_chartAreas.Add((ChartArea)chartArea.PublishClone(context));
				}
			}
			if (m_legends != null)
			{
				chart.m_legends = new List<ChartLegend>(m_legends.Count);
				foreach (ChartLegend legend in m_legends)
				{
					chart.m_legends.Add((ChartLegend)legend.PublishClone(context));
				}
			}
			if (m_titles != null)
			{
				chart.m_titles = new List<ChartTitle>(m_titles.Count);
				foreach (ChartTitle title in m_titles)
				{
					chart.m_titles.Add((ChartTitle)title.PublishClone(context));
				}
			}
			if (m_codeParameters != null)
			{
				chart.m_codeParameters = new DataValueList(m_codeParameters.Count);
				foreach (DataValue codeParameter in m_codeParameters)
				{
					chart.m_codeParameters.Add((DataValue)codeParameter.PublishClone(context));
				}
			}
			if (m_customPaletteColors != null)
			{
				chart.m_customPaletteColors = new List<ChartCustomPaletteColor>(m_customPaletteColors.Count);
				foreach (ChartCustomPaletteColor customPaletteColor in m_customPaletteColors)
				{
					chart.m_customPaletteColors.Add((ChartCustomPaletteColor)customPaletteColor.PublishClone(context));
				}
			}
			if (m_noDataMessage != null)
			{
				chart.m_noDataMessage = (ChartNoDataMessage)m_noDataMessage.PublishClone(context);
			}
			if (m_borderSkin != null)
			{
				chart.m_borderSkin = (ChartBorderSkin)m_borderSkin.PublishClone(context);
			}
			if (m_dynamicHeight != null)
			{
				chart.m_dynamicHeight = (ExpressionInfo)m_dynamicHeight.PublishClone(context);
			}
			if (m_dynamicWidth != null)
			{
				chart.m_dynamicWidth = (ExpressionInfo)m_dynamicWidth.PublishClone(context);
			}
			if (m_palette != null)
			{
				chart.m_palette = (ExpressionInfo)m_palette.PublishClone(context);
			}
			if (m_paletteHatchBehavior != null)
			{
				chart.m_paletteHatchBehavior = (ExpressionInfo)m_paletteHatchBehavior.PublishClone(context);
			}
			return chart;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.CategoryMembers, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartMember));
			list.Add(new MemberInfo(MemberName.SeriesMembers, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartMember));
			list.Add(new MemberInfo(MemberName.ChartSeriesCollection, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartSeries));
			list.Add(new MemberInfo(MemberName.ChartDerivedSeriesCollection, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartDerivedSeries));
			list.Add(new MemberInfo(MemberName.Palette, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ChartAreas, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartArea));
			list.Add(new MemberInfo(MemberName.ChartLegends, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartLegend));
			list.Add(new MemberInfo(MemberName.Titles, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartTitle));
			list.Add(new MemberInfo(MemberName.CodeParameters, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataValue));
			list.Add(new MemberInfo(MemberName.CustomPaletteColors, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartCustomPaletteColor));
			list.Add(new MemberInfo(MemberName.BorderSkin, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartBorderSkin));
			list.Add(new MemberInfo(MemberName.NoDataMessage, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartNoDataMessage));
			list.Add(new MemberInfo(MemberName.DynamicHeight, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.DynamicWidth, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.DataValueSequenceRendering, Token.Boolean));
			list.Add(new MemberInfo(MemberName.PaletteHatchBehavior, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ColumnGroupingIsSwitched, Token.Boolean));
			list.Add(new MemberInfo(MemberName.EnableCategoryDrilldown, Token.Boolean, Lifetime.AddedIn(200)));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Chart, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataRegion, list);
		}

		internal int GenerateActionOwnerID()
		{
			return ++m_actionOwnerCounter;
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.CategoryMembers:
					writer.Write(m_categoryMembers);
					break;
				case MemberName.SeriesMembers:
					writer.Write(m_seriesMembers);
					break;
				case MemberName.ChartSeriesCollection:
					writer.Write(m_chartSeriesCollection);
					break;
				case MemberName.ChartDerivedSeriesCollection:
					writer.Write(m_chartDerivedSeriesCollection);
					break;
				case MemberName.Palette:
					writer.Write(m_palette);
					break;
				case MemberName.ChartAreas:
					writer.Write(m_chartAreas);
					break;
				case MemberName.Titles:
					writer.Write(m_titles);
					break;
				case MemberName.ChartLegends:
					writer.Write(m_legends);
					break;
				case MemberName.NoDataMessage:
					writer.Write(m_noDataMessage);
					break;
				case MemberName.BorderSkin:
					writer.Write(m_borderSkin);
					break;
				case MemberName.DynamicHeight:
					writer.Write(m_dynamicHeight);
					break;
				case MemberName.DynamicWidth:
					writer.Write(m_dynamicWidth);
					break;
				case MemberName.CodeParameters:
					writer.Write(m_codeParameters);
					break;
				case MemberName.CustomPaletteColors:
					writer.Write(m_customPaletteColors);
					break;
				case MemberName.DataValueSequenceRendering:
					writer.Write(m_dataValueSequenceRendering);
					break;
				case MemberName.PaletteHatchBehavior:
					writer.Write(m_paletteHatchBehavior);
					break;
				case MemberName.ColumnGroupingIsSwitched:
					writer.Write(m_columnGroupingIsSwitched);
					break;
				case MemberName.EnableCategoryDrilldown:
					writer.Write(m_enableCategoryDrilldown);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.CategoryMembers:
					m_categoryMembers = reader.ReadListOfRIFObjects<ChartMemberList>();
					break;
				case MemberName.SeriesMembers:
					m_seriesMembers = reader.ReadListOfRIFObjects<ChartMemberList>();
					break;
				case MemberName.ChartSeriesCollection:
					m_chartSeriesCollection = reader.ReadListOfRIFObjects<ChartSeriesList>();
					break;
				case MemberName.ChartDerivedSeriesCollection:
					m_chartDerivedSeriesCollection = reader.ReadGenericListOfRIFObjects<ChartDerivedSeries>();
					break;
				case MemberName.Palette:
					m_palette = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ChartLegends:
					m_legends = reader.ReadGenericListOfRIFObjects<ChartLegend>();
					break;
				case MemberName.ChartAreas:
					m_chartAreas = reader.ReadGenericListOfRIFObjects<ChartArea>();
					break;
				case MemberName.Titles:
					m_titles = reader.ReadGenericListOfRIFObjects<ChartTitle>();
					break;
				case MemberName.DynamicHeight:
					m_dynamicHeight = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.DynamicWidth:
					m_dynamicWidth = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.CodeParameters:
					m_codeParameters = reader.ReadListOfRIFObjects<DataValueList>();
					break;
				case MemberName.CustomPaletteColors:
					m_customPaletteColors = reader.ReadGenericListOfRIFObjects<ChartCustomPaletteColor>();
					break;
				case MemberName.NoDataMessage:
					m_noDataMessage = (ChartNoDataMessage)reader.ReadRIFObject();
					break;
				case MemberName.BorderSkin:
					m_borderSkin = (ChartBorderSkin)reader.ReadRIFObject();
					break;
				case MemberName.DataValueSequenceRendering:
					m_dataValueSequenceRendering = reader.ReadBoolean();
					break;
				case MemberName.PaletteHatchBehavior:
					m_paletteHatchBehavior = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ColumnGroupingIsSwitched:
					m_columnGroupingIsSwitched = reader.ReadBoolean();
					break;
				case MemberName.EnableCategoryDrilldown:
					m_enableCategoryDrilldown = reader.ReadBoolean();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			base.ResolveReferences(memberReferencesCollection, referenceableItems);
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Chart;
		}

		internal override void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			if (base.ExprHostID >= 0)
			{
				Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null, "(reportExprHost != null && reportObjectModel != null)");
				m_chartExprHost = reportExprHost.ChartHostsRemotable[base.ExprHostID];
				DataRegionSetExprHost(m_chartExprHost, m_chartExprHost.SortHost, m_chartExprHost.FilterHostsRemotable, m_chartExprHost.UserSortExpressionsHost, m_chartExprHost.PageBreakExprHost, m_chartExprHost.JoinConditionExprHostsRemotable, reportObjectModel);
			}
		}

		internal override void DataRegionContentsSetExprHost(ObjectModelImpl reportObjectModel, bool traverseDataRegions)
		{
			if (m_chartExprHost == null)
			{
				return;
			}
			IList<ChartAreaExprHost> chartAreasHostsRemotable = m_chartExprHost.ChartAreasHostsRemotable;
			if (m_chartAreas != null && chartAreasHostsRemotable != null)
			{
				for (int i = 0; i < m_chartAreas.Count; i++)
				{
					ChartArea chartArea = m_chartAreas[i];
					if (chartArea != null && chartArea.ExpressionHostID > -1)
					{
						chartArea.SetExprHost(chartAreasHostsRemotable[chartArea.ExpressionHostID], reportObjectModel);
					}
				}
			}
			IList<ChartTitleExprHost> titlesHostsRemotable = m_chartExprHost.TitlesHostsRemotable;
			if (m_titles != null && titlesHostsRemotable != null)
			{
				for (int j = 0; j < m_titles.Count; j++)
				{
					ChartTitle chartTitle = m_titles[j];
					if (chartTitle != null && chartTitle.ExpressionHostID > -1)
					{
						chartTitle.SetExprHost(titlesHostsRemotable[chartTitle.ExpressionHostID], reportObjectModel);
					}
				}
			}
			IList<ChartLegendExprHost> legendsHostsRemotable = m_chartExprHost.LegendsHostsRemotable;
			if (m_legends != null && legendsHostsRemotable != null)
			{
				for (int k = 0; k < m_legends.Count; k++)
				{
					ChartLegend chartLegend = m_legends[k];
					if (chartLegend != null && chartLegend.ExpressionHostID > -1)
					{
						chartLegend.SetExprHost(legendsHostsRemotable[chartLegend.ExpressionHostID], reportObjectModel);
					}
				}
			}
			IList<ChartCustomPaletteColorExprHost> customPaletteColorHostsRemotable = m_chartExprHost.CustomPaletteColorHostsRemotable;
			if (m_customPaletteColors != null && customPaletteColorHostsRemotable != null)
			{
				for (int l = 0; l < m_customPaletteColors.Count; l++)
				{
					ChartCustomPaletteColor chartCustomPaletteColor = m_customPaletteColors[l];
					if (chartCustomPaletteColor != null && chartCustomPaletteColor.ExpressionHostID > -1)
					{
						chartCustomPaletteColor.SetExprHost(customPaletteColorHostsRemotable[chartCustomPaletteColor.ExpressionHostID], reportObjectModel);
					}
				}
			}
			if (m_codeParameters != null && m_chartExprHost.CodeParametersHostsRemotable != null)
			{
				m_codeParameters.SetExprHost(m_chartExprHost.CodeParametersHostsRemotable, reportObjectModel);
			}
			if (m_borderSkin != null && m_chartExprHost.BorderSkinHost != null)
			{
				m_borderSkin.SetExprHost(m_chartExprHost.BorderSkinHost, reportObjectModel);
			}
			if (m_noDataMessage != null && m_chartExprHost.NoDataMessageHost != null)
			{
				m_noDataMessage.SetExprHost(m_chartExprHost.NoDataMessageHost, reportObjectModel);
			}
			IList<ChartSeriesExprHost> seriesCollectionHostsRemotable = m_chartExprHost.SeriesCollectionHostsRemotable;
			IList<ChartDataPointExprHost> cellHostsRemotable = m_chartExprHost.CellHostsRemotable;
			Global.Tracer.Assert(m_chartSeriesCollection != null, "(m_chartSeriesCollection != null)");
			for (int m = 0; m < m_chartSeriesCollection.Count; m++)
			{
				ChartSeries chartSeries = m_chartSeriesCollection[m];
				Global.Tracer.Assert(chartSeries != null, "(null != series)");
				if (seriesCollectionHostsRemotable != null && chartSeries.ExpressionHostID > -1)
				{
					chartSeries.SetExprHost(seriesCollectionHostsRemotable[chartSeries.ExpressionHostID], reportObjectModel);
				}
				if (cellHostsRemotable == null)
				{
					continue;
				}
				Global.Tracer.Assert(chartSeries.DataPoints != null, "(null != series.DataPoints)");
				for (int n = 0; n < chartSeries.DataPoints.Count; n++)
				{
					ChartDataPoint chartDataPoint = chartSeries.DataPoints[n];
					Global.Tracer.Assert(chartDataPoint != null, "(null != dataPoint)");
					if (chartDataPoint.ExpressionHostID > -1)
					{
						chartDataPoint.SetExprHost(cellHostsRemotable[chartDataPoint.ExpressionHostID], reportObjectModel);
					}
				}
			}
		}

		internal override object EvaluateNoRowsMessageExpression()
		{
			return m_chartExprHost.NoRowsExpr;
		}

		internal string EvaluateDynamicWidth(Microsoft.ReportingServices.OnDemandReportRendering.ChartInstance chartInstance, OnDemandProcessingContext context)
		{
			if (m_dynamicWidth == null)
			{
				return null;
			}
			context.SetupContext(this, chartInstance);
			return context.ReportRuntime.EvaluateChartDynamicSizeExpression(this, m_dynamicWidth, "DynamicWidth", isDynamicWidth: true);
		}

		internal string EvaluateDynamicHeight(Microsoft.ReportingServices.OnDemandReportRendering.ChartInstance chartInstance, OnDemandProcessingContext context)
		{
			if (m_dynamicHeight == null)
			{
				return null;
			}
			context.SetupContext(this, chartInstance);
			return context.ReportRuntime.EvaluateChartDynamicSizeExpression(this, m_dynamicHeight, "DynamicHeight", isDynamicWidth: false);
		}

		internal ChartPalette EvaluatePalette(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, reportScopeInstance);
			return EnumTranslator.TranslateChartPalette(context.ReportRuntime.EvaluateChartPaletteExpression(this, base.Name), context.ReportRuntime);
		}

		internal PaletteHatchBehavior EvaluatePaletteHatchBehavior(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, reportScopeInstance);
			return EnumTranslator.TranslatePaletteHatchBehavior(context.ReportRuntime.EvaluateChartPaletteHatchBehaviorExpression(this, base.Name), context.ReportRuntime);
		}

		protected override ReportHierarchyNode CreateHierarchyNode(int id)
		{
			return new ChartMember(id, this);
		}

		protected override Row CreateRow(int id, int columnCount)
		{
			return new ChartSeries(this, id)
			{
				DataPoints = new ChartDataPointList(columnCount)
			};
		}

		protected override Cell CreateCell(int id, int rowIndex, int colIndex)
		{
			return new ChartDataPoint(id, this);
		}
	}
}
