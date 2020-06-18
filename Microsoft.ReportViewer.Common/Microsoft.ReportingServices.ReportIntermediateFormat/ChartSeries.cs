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
	internal class ChartSeries : Row, IPersistable, IActionOwner, IStyleContainer, ICustomPropertiesHolder
	{
		private ChartDataPointList m_dataPoints;

		private int m_exprHostID;

		private string m_name;

		private Action m_action;

		private ExpressionInfo m_type;

		private ExpressionInfo m_subtype;

		private ChartEmptyPoints m_emptyPoints;

		private ExpressionInfo m_legendName;

		private ExpressionInfo m_legendText;

		private ExpressionInfo m_chartAreaName;

		private ExpressionInfo m_valueAxisName;

		private ExpressionInfo m_categoryAxisName;

		private Style m_styleClass;

		private ExpressionInfo m_hidden;

		private ExpressionInfo m_hideInLegend;

		private ChartSmartLabel m_chartSmartLabel;

		private DataValueList m_customProperties;

		private ChartDataLabel m_dataLabel;

		private ChartMarker m_marker;

		private ExpressionInfo m_toolTip;

		private ChartItemInLegend m_chartItemInLegend;

		[Reference]
		private Chart m_chart;

		[Reference]
		private ChartMember m_parentChartMember;

		[Reference]
		private ChartSeries m_sourceSeries;

		[NonSerialized]
		private ChartDerivedSeries m_parentDerivedSeries;

		[NonSerialized]
		private List<ChartDerivedSeries> m_childrenDerivedSeries;

		[NonSerialized]
		private List<string> m_fieldsUsedInValueExpression;

		[NonSerialized]
		private ChartSeriesExprHost m_exprHost;

		[NonSerialized]
		private Formatter m_formatter;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal override CellList Cells => m_dataPoints;

		internal ChartDataPointList DataPoints
		{
			get
			{
				return m_dataPoints;
			}
			set
			{
				m_dataPoints = value;
			}
		}

		internal ChartSeriesExprHost ExprHost => m_exprHost;

		internal int ExpressionHostID => m_exprHostID;

		internal Action Action
		{
			get
			{
				return m_action;
			}
			set
			{
				m_action = value;
			}
		}

		internal ExpressionInfo Hidden
		{
			get
			{
				return m_hidden;
			}
			set
			{
				m_hidden = value;
			}
		}

		internal ExpressionInfo HideInLegend
		{
			get
			{
				return m_hideInLegend;
			}
			set
			{
				m_hideInLegend = value;
			}
		}

		Action IActionOwner.Action => m_action;

		List<string> IActionOwner.FieldsUsedInValueExpression
		{
			get
			{
				return m_fieldsUsedInValueExpression;
			}
			set
			{
				m_fieldsUsedInValueExpression = value;
			}
		}

		internal ExpressionInfo Type
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

		internal ExpressionInfo Subtype
		{
			get
			{
				return m_subtype;
			}
			set
			{
				m_subtype = value;
			}
		}

		internal ChartEmptyPoints EmptyPoints
		{
			get
			{
				return m_emptyPoints;
			}
			set
			{
				m_emptyPoints = value;
			}
		}

		internal ExpressionInfo LegendName
		{
			get
			{
				return m_legendName;
			}
			set
			{
				m_legendName = value;
			}
		}

		internal ExpressionInfo LegendText
		{
			get
			{
				return m_legendText;
			}
			set
			{
				m_legendText = value;
			}
		}

		internal ExpressionInfo ChartAreaName
		{
			get
			{
				return m_chartAreaName;
			}
			set
			{
				m_chartAreaName = value;
			}
		}

		internal ExpressionInfo ValueAxisName
		{
			get
			{
				return m_valueAxisName;
			}
			set
			{
				m_valueAxisName = value;
			}
		}

		internal ExpressionInfo CategoryAxisName
		{
			get
			{
				return m_categoryAxisName;
			}
			set
			{
				m_categoryAxisName = value;
			}
		}

		internal ChartDataLabel DataLabel
		{
			get
			{
				return m_dataLabel;
			}
			set
			{
				m_dataLabel = value;
			}
		}

		internal ChartMarker Marker
		{
			get
			{
				return m_marker;
			}
			set
			{
				m_marker = value;
			}
		}

		public Style StyleClass
		{
			get
			{
				return m_styleClass;
			}
			set
			{
				m_styleClass = value;
			}
		}

		internal string Name
		{
			get
			{
				return m_name;
			}
			set
			{
				m_name = value;
			}
		}

		internal ExpressionInfo ToolTip
		{
			get
			{
				return m_toolTip;
			}
			set
			{
				m_toolTip = value;
			}
		}

		private ChartSeries SourceSeries
		{
			get
			{
				if (m_sourceSeries == null && m_parentDerivedSeries != null)
				{
					m_sourceSeries = m_parentDerivedSeries.SourceSeries;
				}
				return m_sourceSeries;
			}
		}

		internal ChartItemInLegend ChartItemInLegend
		{
			get
			{
				return m_chartItemInLegend;
			}
			set
			{
				m_chartItemInLegend = value;
			}
		}

		private ChartMember ParentChartMember
		{
			get
			{
				if (m_parentChartMember == null)
				{
					if (SourceSeries == null)
					{
						m_parentChartMember = m_chart.GetChartMember(this);
					}
					else
					{
						m_parentChartMember = SourceSeries.ParentChartMember;
					}
				}
				return m_parentChartMember;
			}
		}

		public override List<InstancePathItem> InstancePath
		{
			get
			{
				if (m_cachedInstancePath == null)
				{
					m_cachedInstancePath = new List<InstancePathItem>();
					if (ParentChartMember != null)
					{
						m_cachedInstancePath.AddRange(ParentChartMember.InstancePath);
					}
				}
				return m_cachedInstancePath;
			}
		}

		IInstancePath IStyleContainer.InstancePath => this;

		IInstancePath ICustomPropertiesHolder.InstancePath => this;

		Microsoft.ReportingServices.ReportProcessing.ObjectType IStyleContainer.ObjectType => Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart;

		string IStyleContainer.Name => m_chart.Name;

		internal DataValueList CustomProperties
		{
			get
			{
				return m_customProperties;
			}
			set
			{
				m_customProperties = value;
			}
		}

		internal ChartSmartLabel ChartSmartLabel
		{
			get
			{
				return m_chartSmartLabel;
			}
			set
			{
				m_chartSmartLabel = value;
			}
		}

		DataValueList ICustomPropertiesHolder.CustomProperties => m_customProperties;

		internal List<ChartDerivedSeries> ChildrenDerivedSeries
		{
			get
			{
				if (m_childrenDerivedSeries == null)
				{
					m_childrenDerivedSeries = m_chart.GetChildrenDerivedSeries(m_name);
				}
				return m_childrenDerivedSeries;
			}
		}

		internal ChartSeries()
		{
		}

		internal ChartSeries(Chart chart, int id)
			: base(id)
		{
			m_chart = chart;
		}

		internal ChartSeries(Chart chart, ChartDerivedSeries parentDerivedSeries, int id)
			: this(chart, id)
		{
			m_parentDerivedSeries = parentDerivedSeries;
		}

		internal void SetExprHost(ChartSeriesExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			m_exprHost = exprHost;
			m_exprHost.SetReportObjectModel(reportObjectModel);
			if (m_customProperties != null && m_exprHost.CustomPropertyHostsRemotable != null)
			{
				m_customProperties.SetExprHost(m_exprHost.CustomPropertyHostsRemotable, reportObjectModel);
			}
			if (m_action != null && m_exprHost.ActionInfoHost != null)
			{
				m_action.SetExprHost(m_exprHost.ActionInfoHost, reportObjectModel);
			}
			if (m_styleClass != null)
			{
				m_styleClass.SetStyleExprHost(m_exprHost);
			}
			if (m_chartSmartLabel != null && m_exprHost.SmartLabelHost != null)
			{
				m_chartSmartLabel.SetExprHost(m_exprHost.SmartLabelHost, reportObjectModel);
			}
			if (m_emptyPoints != null && m_exprHost.EmptyPointsHost != null)
			{
				m_emptyPoints.SetExprHost(m_exprHost.EmptyPointsHost, reportObjectModel);
			}
			if (m_action != null && m_exprHost.ActionInfoHost != null)
			{
				m_action.SetExprHost(m_exprHost.ActionInfoHost, reportObjectModel);
			}
			if (m_dataLabel != null && m_exprHost.DataLabelHost != null)
			{
				m_dataLabel.SetExprHost(m_exprHost.DataLabelHost, reportObjectModel);
			}
			if (m_marker != null && m_exprHost.ChartMarkerHost != null)
			{
				m_marker.SetExprHost(m_exprHost.ChartMarkerHost, reportObjectModel);
			}
			List<ChartDerivedSeries> childrenDerivedSeries = ChildrenDerivedSeries;
			IList<ChartDerivedSeriesExprHost> chartDerivedSeriesCollectionHostsRemotable = m_exprHost.ChartDerivedSeriesCollectionHostsRemotable;
			if (childrenDerivedSeries != null && chartDerivedSeriesCollectionHostsRemotable != null)
			{
				for (int i = 0; i < childrenDerivedSeries.Count; i++)
				{
					ChartDerivedSeries chartDerivedSeries = childrenDerivedSeries[i];
					if (chartDerivedSeries != null && chartDerivedSeries.ExpressionHostID > -1)
					{
						chartDerivedSeries.SetExprHost(chartDerivedSeriesCollectionHostsRemotable[chartDerivedSeries.ExpressionHostID], reportObjectModel);
					}
				}
			}
			if (m_chartItemInLegend != null && m_exprHost.DataPointInLegendHost != null)
			{
				m_chartItemInLegend.SetExprHost(m_exprHost.DataPointInLegendHost, reportObjectModel);
			}
		}

		internal void Initialize(InitializationContext context, string name)
		{
			context.ExprHostBuilder.ChartSeriesStart();
			if (m_customProperties != null)
			{
				m_customProperties.Initialize("ChartSeries" + name, context);
			}
			if (m_styleClass != null)
			{
				m_styleClass.Initialize(context);
			}
			if (m_action != null)
			{
				m_action.Initialize(context);
			}
			if (m_type == null)
			{
				m_type = ExpressionInfo.CreateConstExpression(ChartSeriesType.Column.ToString());
			}
			m_type.Initialize("Type", context);
			context.ExprHostBuilder.ChartSeriesType(m_type);
			if (m_subtype == null)
			{
				m_subtype = ExpressionInfo.CreateConstExpression(ChartSeriesSubtype.Plain.ToString());
			}
			m_subtype.Initialize("Subtype", context);
			context.ExprHostBuilder.ChartSeriesSubtype(m_subtype);
			if (m_chartSmartLabel != null)
			{
				m_chartSmartLabel.Initialize(context);
			}
			if (m_emptyPoints != null)
			{
				m_emptyPoints.Initialize(context);
			}
			if (m_legendName != null)
			{
				m_legendName.Initialize("LegendName", context);
				context.ExprHostBuilder.ChartSeriesLegendName(m_legendName);
			}
			if (m_legendText != null)
			{
				m_legendText.Initialize("LegendText", context);
				context.ExprHostBuilder.ChartSeriesLegendText(m_legendText);
			}
			if (m_chartAreaName != null)
			{
				m_chartAreaName.Initialize("ChartAreaName", context);
				context.ExprHostBuilder.ChartSeriesChartAreaName(m_chartAreaName);
			}
			if (m_valueAxisName != null)
			{
				m_valueAxisName.Initialize("ValueAxisName", context);
				context.ExprHostBuilder.ChartSeriesValueAxisName(m_valueAxisName);
			}
			if (m_categoryAxisName != null)
			{
				m_categoryAxisName.Initialize("CategoryAxisName", context);
				context.ExprHostBuilder.ChartSeriesCategoryAxisName(m_categoryAxisName);
			}
			if (m_hidden != null)
			{
				m_hidden.Initialize("Hidden", context);
				context.ExprHostBuilder.ChartSeriesHidden(m_hidden);
			}
			if (m_hideInLegend != null)
			{
				m_hideInLegend.Initialize("HideInLegend", context);
				context.ExprHostBuilder.ChartSeriesHideInLegend(m_hideInLegend);
			}
			if (m_dataLabel != null)
			{
				m_dataLabel.Initialize(context);
			}
			if (m_marker != null)
			{
				m_marker.Initialize(context);
			}
			List<ChartDerivedSeries> childrenDerivedSeries = ChildrenDerivedSeries;
			if (childrenDerivedSeries != null)
			{
				for (int i = 0; i < childrenDerivedSeries.Count; i++)
				{
					childrenDerivedSeries[i].Initialize(context, i);
				}
			}
			if (m_toolTip != null)
			{
				m_toolTip.Initialize("ToolTip", context);
				context.ExprHostBuilder.ChartSeriesToolTip(m_toolTip);
			}
			if (m_chartItemInLegend != null)
			{
				m_chartItemInLegend.Initialize(context);
			}
			context.ExprHostBuilder.ChartSeriesEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			ChartSeries chartSeries = (ChartSeries)base.PublishClone(context);
			chartSeries.m_chart = (Chart)context.CurrentDataRegionClone;
			if (m_dataPoints != null)
			{
				chartSeries.m_dataPoints = new ChartDataPointList(m_dataPoints.Count);
				foreach (ChartDataPoint dataPoint in m_dataPoints)
				{
					chartSeries.m_dataPoints.Add((ChartDataPoint)dataPoint.PublishClone(context));
				}
			}
			if (m_customProperties != null)
			{
				chartSeries.m_customProperties = new DataValueList(m_customProperties.Count);
				foreach (DataValue customProperty in m_customProperties)
				{
					chartSeries.m_customProperties.Add((DataValue)customProperty.PublishClone(context));
				}
			}
			if (m_styleClass != null)
			{
				chartSeries.m_styleClass = (Style)m_styleClass.PublishClone(context);
			}
			if (m_action != null)
			{
				chartSeries.m_action = (Action)m_action.PublishClone(context);
			}
			if (m_type != null)
			{
				chartSeries.m_type = (ExpressionInfo)m_type.PublishClone(context);
			}
			if (m_subtype != null)
			{
				chartSeries.m_subtype = (ExpressionInfo)m_subtype.PublishClone(context);
			}
			if (m_emptyPoints != null)
			{
				chartSeries.m_emptyPoints = (ChartEmptyPoints)m_emptyPoints.PublishClone(context);
			}
			if (m_legendName != null)
			{
				chartSeries.m_legendName = (ExpressionInfo)m_legendName.PublishClone(context);
			}
			if (m_legendText != null)
			{
				chartSeries.m_legendText = (ExpressionInfo)m_legendText.PublishClone(context);
			}
			if (m_chartAreaName != null)
			{
				chartSeries.m_chartAreaName = (ExpressionInfo)m_chartAreaName.PublishClone(context);
			}
			if (m_valueAxisName != null)
			{
				chartSeries.m_valueAxisName = (ExpressionInfo)m_valueAxisName.PublishClone(context);
			}
			if (m_categoryAxisName != null)
			{
				chartSeries.m_categoryAxisName = (ExpressionInfo)m_categoryAxisName.PublishClone(context);
			}
			if (m_hidden != null)
			{
				chartSeries.m_hidden = (ExpressionInfo)m_hidden.PublishClone(context);
			}
			if (m_hideInLegend != null)
			{
				chartSeries.m_hideInLegend = (ExpressionInfo)m_hideInLegend.PublishClone(context);
			}
			if (m_chartSmartLabel != null)
			{
				chartSeries.m_chartSmartLabel = (ChartSmartLabel)m_chartSmartLabel.PublishClone(context);
			}
			if (m_dataLabel != null)
			{
				chartSeries.m_dataLabel = (ChartDataLabel)m_dataLabel.PublishClone(context);
			}
			if (m_marker != null)
			{
				chartSeries.m_marker = (ChartMarker)m_marker.PublishClone(context);
			}
			if (m_toolTip != null)
			{
				chartSeries.m_toolTip = (ExpressionInfo)m_toolTip.PublishClone(context);
			}
			if (m_chartItemInLegend != null)
			{
				chartSeries.m_chartItemInLegend = (ChartItemInLegend)m_chartItemInLegend.PublishClone(context);
			}
			return chartSeries;
		}

		internal ChartSeriesType EvaluateType(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, reportScopeInstance);
			return EnumTranslator.TranslateChartSeriesType(context.ReportRuntime.EvaluateChartSeriesTypeExpression(this, m_chart.Name), context.ReportRuntime);
		}

		internal ChartSeriesSubtype EvaluateSubtype(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, reportScopeInstance);
			return EnumTranslator.TranslateChartSeriesSubtype(context.ReportRuntime.EvaluateChartSeriesSubtypeExpression(this, m_chart.Name), context.ReportRuntime);
		}

		internal string EvaluateLegendName(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartSeriesLegendNameExpression(this, m_chart.Name);
		}

		internal string EvaluateLegendText(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, reportScopeInstance);
			Microsoft.ReportingServices.RdlExpressions.VariantResult variantResult = context.ReportRuntime.EvaluateChartSeriesLegendTextExpression(this, m_chart.Name);
			string result = null;
			if (variantResult.ErrorOccurred)
			{
				result = RPRes.rsExpressionErrorValue;
			}
			else if (variantResult.Value != null)
			{
				result = Formatter.Format(variantResult.Value, ref m_formatter, m_chart.StyleClass, m_styleClass, context, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, m_chart.Name);
			}
			return result;
		}

		internal string EvaluateChartAreaName(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartSeriesChartAreaNameExpression(this, m_chart.Name);
		}

		internal string EvaluateValueAxisName(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartSeriesValueAxisNameExpression(this, m_chart.Name);
		}

		internal string EvaluateCategoryAxisName(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartSeriesCategoryAxisNameExpression(this, m_chart.Name);
		}

		internal bool EvaluateHidden(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartSeriesHiddenExpression(this, m_chart.Name);
		}

		internal bool EvaluateHideInLegend(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartSeriesHideInLegendExpression(this, m_chart.Name);
		}

		internal string EvaluateToolTip(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, reportScopeInstance);
			Microsoft.ReportingServices.RdlExpressions.VariantResult variantResult = context.ReportRuntime.EvaluateChartSeriesToolTipExpression(this, m_chart.Name);
			string result = null;
			if (variantResult.ErrorOccurred)
			{
				result = RPRes.rsExpressionErrorValue;
			}
			else if (variantResult.Value != null)
			{
				result = Formatter.Format(variantResult.Value, ref m_formatter, m_chart.StyleClass, m_styleClass, context, Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart, m_chart.Name);
			}
			return result;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.ChartDataPoints, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartDataPoint));
			list.Add(new MemberInfo(MemberName.Name, Token.String));
			list.Add(new MemberInfo(MemberName.Action, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Action));
			list.Add(new MemberInfo(MemberName.Type, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Subtype, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.EmptyPoints, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartEmptyPoints));
			list.Add(new MemberInfo(MemberName.LegendName, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.LegendText, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ChartAreaName, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ValueAxisName, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.CategoryAxisName, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.StyleClass, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Style));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.Chart, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Chart, Token.Reference));
			list.Add(new MemberInfo(MemberName.Hidden, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.HideInLegend, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ChartSmartLabel, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartSmartLabel));
			list.Add(new MemberInfo(MemberName.CustomProperties, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataValue));
			list.Add(new MemberInfo(MemberName.DataLabel, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartDataLabel));
			list.Add(new MemberInfo(MemberName.Marker, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartMarker));
			list.Add(new MemberInfo(MemberName.ChartMember, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartMember, Token.Reference));
			list.Add(new MemberInfo(MemberName.SourceSeries, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartSeries, Token.Reference));
			list.Add(new MemberInfo(MemberName.ToolTip, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ChartItemInLegend, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartItemInLegend));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartSeries, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Row, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Name:
					writer.Write(m_name);
					break;
				case MemberName.ChartDataPoints:
					writer.Write(m_dataPoints);
					break;
				case MemberName.ExprHostID:
					writer.Write(m_exprHostID);
					break;
				case MemberName.Chart:
					writer.WriteReference(m_chart);
					break;
				case MemberName.ChartMember:
					writer.WriteReference(ParentChartMember);
					break;
				case MemberName.SourceSeries:
					writer.WriteReference(SourceSeries);
					break;
				case MemberName.Action:
					writer.Write(m_action);
					break;
				case MemberName.Type:
					writer.Write(m_type);
					break;
				case MemberName.Subtype:
					writer.Write(m_subtype);
					break;
				case MemberName.EmptyPoints:
					writer.Write(m_emptyPoints);
					break;
				case MemberName.LegendName:
					writer.Write(m_legendName);
					break;
				case MemberName.LegendText:
					writer.Write(m_legendText);
					break;
				case MemberName.ChartAreaName:
					writer.Write(m_chartAreaName);
					break;
				case MemberName.ValueAxisName:
					writer.Write(m_valueAxisName);
					break;
				case MemberName.CategoryAxisName:
					writer.Write(m_categoryAxisName);
					break;
				case MemberName.StyleClass:
					writer.Write(m_styleClass);
					break;
				case MemberName.Hidden:
					writer.Write(m_hidden);
					break;
				case MemberName.HideInLegend:
					writer.Write(m_hideInLegend);
					break;
				case MemberName.ChartSmartLabel:
					writer.Write(m_chartSmartLabel);
					break;
				case MemberName.CustomProperties:
					writer.Write(m_customProperties);
					break;
				case MemberName.DataLabel:
					writer.Write(m_dataLabel);
					break;
				case MemberName.Marker:
					writer.Write(m_marker);
					break;
				case MemberName.ToolTip:
					writer.Write(m_toolTip);
					break;
				case MemberName.ChartItemInLegend:
					writer.Write(m_chartItemInLegend);
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
				case MemberName.Name:
					m_name = reader.ReadString();
					break;
				case MemberName.ChartDataPoints:
					m_dataPoints = reader.ReadListOfRIFObjects<ChartDataPointList>();
					break;
				case MemberName.ExprHostID:
					m_exprHostID = reader.ReadInt32();
					break;
				case MemberName.Chart:
					m_chart = reader.ReadReference<Chart>(this);
					break;
				case MemberName.ChartMember:
					m_parentChartMember = reader.ReadReference<ChartMember>(this);
					break;
				case MemberName.SourceSeries:
					m_sourceSeries = reader.ReadReference<ChartSeries>(this);
					break;
				case MemberName.Action:
					m_action = (Action)reader.ReadRIFObject();
					break;
				case MemberName.Type:
					m_type = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Subtype:
					m_subtype = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.EmptyPoints:
					m_emptyPoints = (ChartEmptyPoints)reader.ReadRIFObject();
					break;
				case MemberName.LegendName:
					m_legendName = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.LegendText:
					m_legendText = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ChartAreaName:
					m_chartAreaName = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ValueAxisName:
					m_valueAxisName = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.CategoryAxisName:
					m_categoryAxisName = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.StyleClass:
					m_styleClass = (Style)reader.ReadRIFObject();
					break;
				case MemberName.Hidden:
					m_hidden = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.HideInLegend:
					m_hideInLegend = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ChartSmartLabel:
					m_chartSmartLabel = (ChartSmartLabel)reader.ReadRIFObject();
					break;
				case MemberName.CustomProperties:
					m_customProperties = reader.ReadListOfRIFObjects<DataValueList>();
					break;
				case MemberName.DataLabel:
					m_dataLabel = (ChartDataLabel)reader.ReadRIFObject();
					break;
				case MemberName.Marker:
					m_marker = (ChartMarker)reader.ReadRIFObject();
					break;
				case MemberName.ToolTip:
					m_toolTip = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ChartItemInLegend:
					m_chartItemInLegend = (ChartItemInLegend)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			if (!memberReferencesCollection.TryGetValue(m_Declaration.ObjectType, out List<MemberReference> value))
			{
				return;
			}
			foreach (MemberReference item in value)
			{
				switch (item.MemberName)
				{
				case MemberName.Chart:
					Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
					m_chart = (Chart)referenceableItems[item.RefID];
					break;
				case MemberName.ChartMember:
					Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
					m_parentChartMember = (ChartMember)referenceableItems[item.RefID];
					break;
				case MemberName.ChartSeries:
					Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
					m_sourceSeries = (ChartSeries)referenceableItems[item.RefID];
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartSeries;
		}
	}
}
