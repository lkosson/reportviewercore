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
	internal sealed class ChartLegendCustomItemCell : ChartStyleContainer, IPersistable, IActionOwner
	{
		private string m_name;

		private int m_exprHostID;

		private Action m_action;

		private ExpressionInfo m_cellType;

		private ExpressionInfo m_text;

		private ExpressionInfo m_cellSpan;

		private ExpressionInfo m_toolTip;

		private ExpressionInfo m_imageWidth;

		private ExpressionInfo m_imageHeight;

		private ExpressionInfo m_symbolHeight;

		private ExpressionInfo m_symbolWidth;

		private ExpressionInfo m_alignment;

		private ExpressionInfo m_topMargin;

		private ExpressionInfo m_bottomMargin;

		private ExpressionInfo m_leftMargin;

		private ExpressionInfo m_rightMargin;

		private int m_id;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		[NonSerialized]
		private ChartLegendCustomItemCellExprHost m_exprHost;

		[NonSerialized]
		private List<string> m_fieldsUsedInValueExpression;

		internal string LegendCustomItemCellName
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

		internal ChartLegendCustomItemCellExprHost ExprHost => m_exprHost;

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

		internal ExpressionInfo CellType
		{
			get
			{
				return m_cellType;
			}
			set
			{
				m_cellType = value;
			}
		}

		internal ExpressionInfo Text
		{
			get
			{
				return m_text;
			}
			set
			{
				m_text = value;
			}
		}

		internal ExpressionInfo CellSpan
		{
			get
			{
				return m_cellSpan;
			}
			set
			{
				m_cellSpan = value;
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

		internal ExpressionInfo ImageWidth
		{
			get
			{
				return m_imageWidth;
			}
			set
			{
				m_imageWidth = value;
			}
		}

		internal ExpressionInfo ImageHeight
		{
			get
			{
				return m_imageHeight;
			}
			set
			{
				m_imageHeight = value;
			}
		}

		internal ExpressionInfo SymbolHeight
		{
			get
			{
				return m_symbolHeight;
			}
			set
			{
				m_symbolHeight = value;
			}
		}

		internal ExpressionInfo SymbolWidth
		{
			get
			{
				return m_symbolWidth;
			}
			set
			{
				m_symbolWidth = value;
			}
		}

		internal ExpressionInfo Alignment
		{
			get
			{
				return m_alignment;
			}
			set
			{
				m_alignment = value;
			}
		}

		internal ExpressionInfo TopMargin
		{
			get
			{
				return m_topMargin;
			}
			set
			{
				m_topMargin = value;
			}
		}

		internal ExpressionInfo BottomMargin
		{
			get
			{
				return m_bottomMargin;
			}
			set
			{
				m_bottomMargin = value;
			}
		}

		internal ExpressionInfo LeftMargin
		{
			get
			{
				return m_leftMargin;
			}
			set
			{
				m_leftMargin = value;
			}
		}

		internal ExpressionInfo RightMargin
		{
			get
			{
				return m_rightMargin;
			}
			set
			{
				m_rightMargin = value;
			}
		}

		internal ChartLegendCustomItemCell()
		{
		}

		internal ChartLegendCustomItemCell(Chart chart, int id)
			: base(chart)
		{
			m_id = id;
		}

		internal void SetExprHost(ChartLegendCustomItemCellExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
			m_exprHost = exprHost;
			if (m_action != null && m_exprHost.ActionInfoHost != null)
			{
				m_action.SetExprHost(m_exprHost.ActionInfoHost, reportObjectModel);
			}
		}

		internal void Initialize(InitializationContext context, int index)
		{
			context.ExprHostBuilder.ChartLegendCustomItemCellStart(m_name);
			base.Initialize(context);
			if (m_action != null)
			{
				m_action.Initialize(context);
			}
			if (m_cellType != null)
			{
				m_cellType.Initialize("CellType", context);
				context.ExprHostBuilder.ChartLegendCustomItemCellCellType(m_cellType);
			}
			if (m_text != null)
			{
				m_text.Initialize("Text", context);
				context.ExprHostBuilder.ChartLegendCustomItemCellText(m_text);
			}
			if (m_cellSpan != null)
			{
				m_cellSpan.Initialize("CellSpan", context);
				context.ExprHostBuilder.ChartLegendCustomItemCellCellSpan(m_cellSpan);
			}
			if (m_toolTip != null)
			{
				m_toolTip.Initialize("ToolTip", context);
				context.ExprHostBuilder.ChartLegendCustomItemCellToolTip(m_toolTip);
			}
			if (m_imageWidth != null)
			{
				m_imageWidth.Initialize("ImageWidth", context);
				context.ExprHostBuilder.ChartLegendCustomItemCellImageWidth(m_imageWidth);
			}
			if (m_imageHeight != null)
			{
				m_imageHeight.Initialize("ImageHeight", context);
				context.ExprHostBuilder.ChartLegendCustomItemCellImageHeight(m_imageHeight);
			}
			if (m_symbolHeight != null)
			{
				m_symbolHeight.Initialize("SymbolHeight", context);
				context.ExprHostBuilder.ChartLegendCustomItemCellSymbolHeight(m_symbolHeight);
			}
			if (m_symbolWidth != null)
			{
				m_symbolWidth.Initialize("SymbolWidth", context);
				context.ExprHostBuilder.ChartLegendCustomItemCellSymbolWidth(m_symbolWidth);
			}
			if (m_alignment != null)
			{
				m_alignment.Initialize("Alignment", context);
				context.ExprHostBuilder.ChartLegendCustomItemCellAlignment(m_alignment);
			}
			if (m_topMargin != null)
			{
				m_topMargin.Initialize("TopMargin", context);
				context.ExprHostBuilder.ChartLegendCustomItemCellTopMargin(m_topMargin);
			}
			if (m_bottomMargin != null)
			{
				m_bottomMargin.Initialize("BottomMargin", context);
				context.ExprHostBuilder.ChartLegendCustomItemCellBottomMargin(m_bottomMargin);
			}
			if (m_leftMargin != null)
			{
				m_leftMargin.Initialize("LeftMargin", context);
				context.ExprHostBuilder.ChartLegendCustomItemCellLeftMargin(m_leftMargin);
			}
			if (m_rightMargin != null)
			{
				m_rightMargin.Initialize("RightMargin", context);
				context.ExprHostBuilder.ChartLegendCustomItemCellRightMargin(m_rightMargin);
			}
			m_exprHostID = context.ExprHostBuilder.ChartLegendCustomItemCellEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			ChartLegendCustomItemCell chartLegendCustomItemCell = (ChartLegendCustomItemCell)base.PublishClone(context);
			if (m_action != null)
			{
				chartLegendCustomItemCell.m_action = (Action)m_action.PublishClone(context);
			}
			if (m_cellType != null)
			{
				chartLegendCustomItemCell.m_cellType = (ExpressionInfo)m_cellType.PublishClone(context);
			}
			if (m_text != null)
			{
				chartLegendCustomItemCell.m_text = (ExpressionInfo)m_text.PublishClone(context);
			}
			if (m_cellSpan != null)
			{
				chartLegendCustomItemCell.m_cellSpan = (ExpressionInfo)m_cellSpan.PublishClone(context);
			}
			if (m_toolTip != null)
			{
				chartLegendCustomItemCell.m_toolTip = (ExpressionInfo)m_toolTip.PublishClone(context);
			}
			if (m_imageWidth != null)
			{
				chartLegendCustomItemCell.m_imageWidth = (ExpressionInfo)m_imageWidth.PublishClone(context);
			}
			if (m_imageHeight != null)
			{
				chartLegendCustomItemCell.m_imageHeight = (ExpressionInfo)m_imageHeight.PublishClone(context);
			}
			if (m_symbolHeight != null)
			{
				chartLegendCustomItemCell.m_symbolHeight = (ExpressionInfo)m_symbolHeight.PublishClone(context);
			}
			if (m_symbolWidth != null)
			{
				chartLegendCustomItemCell.m_symbolWidth = (ExpressionInfo)m_symbolWidth.PublishClone(context);
			}
			if (m_alignment != null)
			{
				chartLegendCustomItemCell.m_alignment = (ExpressionInfo)m_alignment.PublishClone(context);
			}
			if (m_topMargin != null)
			{
				chartLegendCustomItemCell.m_topMargin = (ExpressionInfo)m_topMargin.PublishClone(context);
			}
			if (m_bottomMargin != null)
			{
				chartLegendCustomItemCell.m_bottomMargin = (ExpressionInfo)m_bottomMargin.PublishClone(context);
			}
			if (m_leftMargin != null)
			{
				chartLegendCustomItemCell.m_leftMargin = (ExpressionInfo)m_leftMargin.PublishClone(context);
			}
			if (m_rightMargin != null)
			{
				chartLegendCustomItemCell.m_rightMargin = (ExpressionInfo)m_rightMargin.PublishClone(context);
			}
			return chartLegendCustomItemCell;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Name, Token.String));
			list.Add(new MemberInfo(MemberName.Action, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Action));
			list.Add(new MemberInfo(MemberName.CellType, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Text, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.CellSpan, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ToolTip, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ImageWidth, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ImageHeight, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.SymbolHeight, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.SymbolWidth, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Alignment, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.TopMargin, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.BottomMargin, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.LeftMargin, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.RightMargin, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.ID, Token.Int32));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartLegendCustomItemCell, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartStyleContainer, list);
		}

		internal ChartCellType EvaluateCellType(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return EnumTranslator.TranslateChartCellType(context.ReportRuntime.EvaluateChartLegendCustomItemCellCellTypeExpression(this, m_chart.Name), context.ReportRuntime);
		}

		internal string EvaluateText(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendCustomItemCellTextExpression(this, m_chart.Name);
		}

		internal int EvaluateCellSpan(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendCustomItemCellCellSpanExpression(this, m_chart.Name);
		}

		internal string EvaluateToolTip(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendCustomItemCellToolTipExpression(this, m_chart.Name);
		}

		internal int EvaluateImageWidth(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendCustomItemCellImageWidthExpression(this, m_chart.Name);
		}

		internal int EvaluateImageHeight(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendCustomItemCellImageHeightExpression(this, m_chart.Name);
		}

		internal int EvaluateSymbolHeight(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendCustomItemCellSymbolHeightExpression(this, m_chart.Name);
		}

		internal int EvaluateSymbolWidth(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendCustomItemCellSymbolWidthExpression(this, m_chart.Name);
		}

		internal ChartCellAlignment EvaluateAlignment(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return EnumTranslator.TranslateChartCellAlignment(context.ReportRuntime.EvaluateChartLegendCustomItemCellAlignmentExpression(this, m_chart.Name), context.ReportRuntime);
		}

		internal int EvaluateTopMargin(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendCustomItemCellTopMarginExpression(this, m_chart.Name);
		}

		internal int EvaluateBottomMargin(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendCustomItemCellBottomMarginExpression(this, m_chart.Name);
		}

		internal int EvaluateLeftMargin(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendCustomItemCellLeftMarginExpression(this, m_chart.Name);
		}

		internal int EvaluateRightMargin(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendCustomItemCellRightMarginExpression(this, m_chart.Name);
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
				case MemberName.ExprHostID:
					writer.Write(m_exprHostID);
					break;
				case MemberName.Action:
					writer.Write(m_action);
					break;
				case MemberName.CellType:
					writer.Write(m_cellType);
					break;
				case MemberName.Text:
					writer.Write(m_text);
					break;
				case MemberName.CellSpan:
					writer.Write(m_cellSpan);
					break;
				case MemberName.ToolTip:
					writer.Write(m_toolTip);
					break;
				case MemberName.ImageWidth:
					writer.Write(m_imageWidth);
					break;
				case MemberName.ImageHeight:
					writer.Write(m_imageHeight);
					break;
				case MemberName.SymbolHeight:
					writer.Write(m_symbolHeight);
					break;
				case MemberName.SymbolWidth:
					writer.Write(m_symbolWidth);
					break;
				case MemberName.Alignment:
					writer.Write(m_alignment);
					break;
				case MemberName.TopMargin:
					writer.Write(m_topMargin);
					break;
				case MemberName.BottomMargin:
					writer.Write(m_bottomMargin);
					break;
				case MemberName.LeftMargin:
					writer.Write(m_leftMargin);
					break;
				case MemberName.RightMargin:
					writer.Write(m_rightMargin);
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
				case MemberName.ExprHostID:
					m_exprHostID = reader.ReadInt32();
					break;
				case MemberName.Action:
					m_action = (Action)reader.ReadRIFObject();
					break;
				case MemberName.CellType:
					m_cellType = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Text:
					m_text = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.CellSpan:
					m_cellSpan = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ToolTip:
					m_toolTip = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ImageWidth:
					m_imageWidth = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ImageHeight:
					m_imageHeight = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.SymbolHeight:
					m_symbolHeight = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.SymbolWidth:
					m_symbolWidth = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Alignment:
					m_alignment = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.TopMargin:
					m_topMargin = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.BottomMargin:
					m_bottomMargin = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.LeftMargin:
					m_leftMargin = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.RightMargin:
					m_rightMargin = (ExpressionInfo)reader.ReadRIFObject();
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
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartLegendCustomItemCell;
		}
	}
}
