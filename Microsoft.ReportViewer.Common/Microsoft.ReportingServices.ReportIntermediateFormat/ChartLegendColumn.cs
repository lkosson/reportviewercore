using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandReportRendering;
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
	internal sealed class ChartLegendColumn : ChartStyleContainer, IPersistable, IActionOwner
	{
		private string m_name;

		private int m_exprHostID;

		private Action m_action;

		private ExpressionInfo m_columnType;

		private ExpressionInfo m_value;

		private ExpressionInfo m_toolTip;

		private ExpressionInfo m_minimumWidth;

		private ExpressionInfo m_maximumWidth;

		private ExpressionInfo m_seriesSymbolWidth;

		private ExpressionInfo m_seriesSymbolHeight;

		private ChartLegendColumnHeader m_header;

		private int m_id;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		[NonSerialized]
		private ChartLegendColumnExprHost m_exprHost;

		[NonSerialized]
		private List<string> m_fieldsUsedInValueExpression;

		internal string LegendColumnName
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

		internal ChartLegendColumnExprHost ExprHost => m_exprHost;

		internal int ExpressionHostID => m_exprHostID;

		internal int ID => m_id;

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

		internal ExpressionInfo ColumnType
		{
			get
			{
				return m_columnType;
			}
			set
			{
				m_columnType = value;
			}
		}

		internal ExpressionInfo Value
		{
			get
			{
				return m_value;
			}
			set
			{
				m_value = value;
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

		internal ExpressionInfo MinimumWidth
		{
			get
			{
				return m_minimumWidth;
			}
			set
			{
				m_minimumWidth = value;
			}
		}

		internal ExpressionInfo MaximumWidth
		{
			get
			{
				return m_maximumWidth;
			}
			set
			{
				m_maximumWidth = value;
			}
		}

		internal ExpressionInfo SeriesSymbolWidth
		{
			get
			{
				return m_seriesSymbolWidth;
			}
			set
			{
				m_seriesSymbolWidth = value;
			}
		}

		internal ExpressionInfo SeriesSymbolHeight
		{
			get
			{
				return m_seriesSymbolHeight;
			}
			set
			{
				m_seriesSymbolHeight = value;
			}
		}

		internal ChartLegendColumnHeader Header
		{
			get
			{
				return m_header;
			}
			set
			{
				m_header = value;
			}
		}

		internal ChartLegendColumn()
		{
		}

		internal ChartLegendColumn(Chart chart, int id)
			: base(chart)
		{
			m_id = id;
		}

		internal void SetExprHost(ChartLegendColumnExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
			m_exprHost = exprHost;
			if (m_header != null && m_exprHost.HeaderHost != null)
			{
				m_header.SetExprHost(m_exprHost.HeaderHost, reportObjectModel);
			}
			if (m_action != null && m_exprHost.ActionInfoHost != null)
			{
				m_action.SetExprHost(m_exprHost.ActionInfoHost, reportObjectModel);
			}
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.ChartLegendColumnStart(m_name);
			base.Initialize(context);
			if (m_action != null)
			{
				m_action.Initialize(context);
			}
			if (m_columnType != null)
			{
				m_columnType.Initialize("ColumnType", context);
				context.ExprHostBuilder.ChartLegendColumnColumnType(m_columnType);
			}
			if (m_value != null)
			{
				m_value.Initialize("Value", context);
				context.ExprHostBuilder.ChartLegendColumnValue(m_value);
			}
			if (m_toolTip != null)
			{
				m_toolTip.Initialize("ToolTip", context);
				context.ExprHostBuilder.ChartLegendColumnToolTip(m_toolTip);
			}
			if (m_minimumWidth != null)
			{
				m_minimumWidth.Initialize("MinimumWidth", context);
				context.ExprHostBuilder.ChartLegendColumnMinimumWidth(m_minimumWidth);
			}
			if (m_maximumWidth != null)
			{
				m_maximumWidth.Initialize("MaximumWidth", context);
				context.ExprHostBuilder.ChartLegendColumnMaximumWidth(m_maximumWidth);
			}
			if (m_seriesSymbolWidth != null)
			{
				m_seriesSymbolWidth.Initialize("SeriesSymbolWidth", context);
				context.ExprHostBuilder.ChartLegendColumnSeriesSymbolWidth(m_seriesSymbolWidth);
			}
			if (m_seriesSymbolHeight != null)
			{
				m_seriesSymbolHeight.Initialize("SeriesSymbolHeight", context);
				context.ExprHostBuilder.ChartLegendColumnSeriesSymbolHeight(m_seriesSymbolHeight);
			}
			if (m_header != null)
			{
				m_header.Initialize(context);
			}
			m_exprHostID = context.ExprHostBuilder.ChartLegendColumnEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			ChartLegendColumn chartLegendColumn = (ChartLegendColumn)base.PublishClone(context);
			if (m_action != null)
			{
				chartLegendColumn.m_action = (Action)m_action.PublishClone(context);
			}
			if (m_columnType != null)
			{
				chartLegendColumn.m_columnType = (ExpressionInfo)m_columnType.PublishClone(context);
			}
			if (m_value != null)
			{
				chartLegendColumn.m_value = (ExpressionInfo)m_value.PublishClone(context);
			}
			if (m_toolTip != null)
			{
				chartLegendColumn.m_toolTip = (ExpressionInfo)m_toolTip.PublishClone(context);
			}
			if (m_minimumWidth != null)
			{
				chartLegendColumn.m_minimumWidth = (ExpressionInfo)m_minimumWidth.PublishClone(context);
			}
			if (m_maximumWidth != null)
			{
				chartLegendColumn.m_maximumWidth = (ExpressionInfo)m_maximumWidth.PublishClone(context);
			}
			if (m_seriesSymbolWidth != null)
			{
				chartLegendColumn.m_seriesSymbolWidth = (ExpressionInfo)m_seriesSymbolWidth.PublishClone(context);
			}
			if (m_seriesSymbolHeight != null)
			{
				chartLegendColumn.m_seriesSymbolHeight = (ExpressionInfo)m_seriesSymbolHeight.PublishClone(context);
			}
			if (m_header != null)
			{
				chartLegendColumn.m_header = (ChartLegendColumnHeader)m_header.PublishClone(context);
			}
			return chartLegendColumn;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Name, Token.String));
			list.Add(new MemberInfo(MemberName.Action, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Action));
			list.Add(new MemberInfo(MemberName.ColumnType, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Value, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ToolTip, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MinimumWidth, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MaximumWidth, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.SeriesSymbolWidth, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.SeriesSymbolHeight, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Header, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartLegendColumnHeader));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.ID, Token.Int32));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartLegendColumn, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartStyleContainer, list);
		}

		internal ChartColumnType EvaluateColumnType(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return EnumTranslator.TranslateChartColumnType(context.ReportRuntime.EvaluateChartLegendColumnColumnTypeExpression(this, m_chart.Name), context.ReportRuntime);
		}

		internal string EvaluateValue(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendColumnValueExpression(this, m_chart.Name);
		}

		internal string EvaluateToolTip(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendColumnToolTipExpression(this, m_chart.Name);
		}

		internal string EvaluateMinimumWidth(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendColumnMinimumWidthExpression(this, m_chart.Name);
		}

		internal string EvaluateMaximumWidth(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendColumnMaximumWidthExpression(this, m_chart.Name);
		}

		internal int EvaluateSeriesSymbolWidth(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendColumnSeriesSymbolWidthExpression(this, m_chart.Name);
		}

		internal int EvaluateSeriesSymbolHeight(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendColumnSeriesSymbolHeightExpression(this, m_chart.Name);
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
				case MemberName.Action:
					writer.Write(m_action);
					break;
				case MemberName.ColumnType:
					writer.Write(m_columnType);
					break;
				case MemberName.Value:
					writer.Write(m_value);
					break;
				case MemberName.ToolTip:
					writer.Write(m_toolTip);
					break;
				case MemberName.MinimumWidth:
					writer.Write(m_minimumWidth);
					break;
				case MemberName.MaximumWidth:
					writer.Write(m_maximumWidth);
					break;
				case MemberName.SeriesSymbolWidth:
					writer.Write(m_seriesSymbolWidth);
					break;
				case MemberName.SeriesSymbolHeight:
					writer.Write(m_seriesSymbolHeight);
					break;
				case MemberName.Header:
					writer.Write(m_header);
					break;
				case MemberName.ExprHostID:
					writer.Write(m_exprHostID);
					break;
				case MemberName.ID:
					writer.Write(m_id);
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
				case MemberName.Action:
					m_action = (Action)reader.ReadRIFObject();
					break;
				case MemberName.ColumnType:
					m_columnType = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Value:
					m_value = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ToolTip:
					m_toolTip = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MinimumWidth:
					m_minimumWidth = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MaximumWidth:
					m_maximumWidth = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.SeriesSymbolWidth:
					m_seriesSymbolWidth = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.SeriesSymbolHeight:
					m_seriesSymbolHeight = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Header:
					m_header = (ChartLegendColumnHeader)reader.ReadRIFObject();
					break;
				case MemberName.ExprHostID:
					m_exprHostID = reader.ReadInt32();
					break;
				case MemberName.ID:
					m_id = reader.ReadInt32();
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
			if (m_id == 0)
			{
				m_id = m_chart.GenerateActionOwnerID();
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartLegendColumn;
		}
	}
}
