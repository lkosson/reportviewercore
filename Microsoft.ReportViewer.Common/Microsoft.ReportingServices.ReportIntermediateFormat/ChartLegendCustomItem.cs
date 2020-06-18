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
	internal sealed class ChartLegendCustomItem : ChartStyleContainer, IPersistable, IActionOwner
	{
		private string m_name;

		private int m_exprHostID;

		private Action m_action;

		private ChartMarker m_marker;

		private ExpressionInfo m_separator;

		private ExpressionInfo m_separatorColor;

		private ExpressionInfo m_toolTip;

		private List<ChartLegendCustomItemCell> m_chartLegendCustomItemCells;

		private int m_id;

		[NonSerialized]
		private List<string> m_fieldsUsedInValueExpression;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		[NonSerialized]
		private ChartLegendCustomItemExprHost m_exprHost;

		internal string LegendCustomItemName
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

		internal ChartLegendCustomItemExprHost ExprHost => m_exprHost;

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

		internal ExpressionInfo Separator
		{
			get
			{
				return m_separator;
			}
			set
			{
				m_separator = value;
			}
		}

		internal ExpressionInfo SeparatorColor
		{
			get
			{
				return m_separatorColor;
			}
			set
			{
				m_separatorColor = value;
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

		internal List<ChartLegendCustomItemCell> LegendCustomItemCells
		{
			get
			{
				return m_chartLegendCustomItemCells;
			}
			set
			{
				m_chartLegendCustomItemCells = value;
			}
		}

		internal ChartLegendCustomItem()
		{
		}

		internal ChartLegendCustomItem(Chart chart, int id)
			: base(chart)
		{
			m_id = id;
		}

		internal void SetExprHost(ChartLegendCustomItemExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
			m_exprHost = exprHost;
			if (m_marker != null && m_exprHost.ChartMarkerHost != null)
			{
				m_marker.SetExprHost(m_exprHost.ChartMarkerHost, reportObjectModel);
			}
			if (m_action != null && m_exprHost.ActionInfoHost != null)
			{
				m_action.SetExprHost(m_exprHost.ActionInfoHost, reportObjectModel);
			}
			IList<ChartLegendCustomItemCellExprHost> chartLegendCustomItemCellsHostsRemotable = m_exprHost.ChartLegendCustomItemCellsHostsRemotable;
			if (m_chartLegendCustomItemCells == null || chartLegendCustomItemCellsHostsRemotable == null)
			{
				return;
			}
			for (int i = 0; i < m_chartLegendCustomItemCells.Count; i++)
			{
				ChartLegendCustomItemCell chartLegendCustomItemCell = m_chartLegendCustomItemCells[i];
				if (chartLegendCustomItemCell != null && chartLegendCustomItemCell.ExpressionHostID > -1)
				{
					chartLegendCustomItemCell.SetExprHost(chartLegendCustomItemCellsHostsRemotable[chartLegendCustomItemCell.ExpressionHostID], reportObjectModel);
				}
			}
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.ChartLegendCustomItemStart(m_name);
			base.Initialize(context);
			if (m_action != null)
			{
				m_action.Initialize(context);
			}
			if (m_marker != null)
			{
				m_marker.Initialize(context);
			}
			if (m_separator != null)
			{
				m_separator.Initialize("Separator", context);
				context.ExprHostBuilder.ChartLegendCustomItemSeparator(m_separator);
			}
			if (m_separatorColor != null)
			{
				m_separatorColor.Initialize("SeparatorColor", context);
				context.ExprHostBuilder.ChartLegendCustomItemSeparatorColor(m_separatorColor);
			}
			if (m_toolTip != null)
			{
				m_toolTip.Initialize("ToolTip", context);
				context.ExprHostBuilder.ChartLegendCustomItemToolTip(m_toolTip);
			}
			if (m_chartLegendCustomItemCells != null)
			{
				for (int i = 0; i < m_chartLegendCustomItemCells.Count; i++)
				{
					m_chartLegendCustomItemCells[i].Initialize(context, i);
				}
			}
			m_exprHostID = context.ExprHostBuilder.ChartLegendCustomItemEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			ChartLegendCustomItem chartLegendCustomItem = (ChartLegendCustomItem)base.PublishClone(context);
			if (m_action != null)
			{
				chartLegendCustomItem.m_action = (Action)m_action.PublishClone(context);
			}
			if (m_marker != null)
			{
				chartLegendCustomItem.m_marker = (ChartMarker)m_marker.PublishClone(context);
			}
			if (m_separator != null)
			{
				chartLegendCustomItem.m_separator = (ExpressionInfo)m_separator.PublishClone(context);
			}
			if (m_separatorColor != null)
			{
				chartLegendCustomItem.m_separatorColor = (ExpressionInfo)m_separatorColor.PublishClone(context);
			}
			if (m_toolTip != null)
			{
				chartLegendCustomItem.m_toolTip = (ExpressionInfo)m_toolTip.PublishClone(context);
			}
			if (m_chartLegendCustomItemCells != null)
			{
				chartLegendCustomItem.m_chartLegendCustomItemCells = new List<ChartLegendCustomItemCell>(m_chartLegendCustomItemCells.Count);
				{
					foreach (ChartLegendCustomItemCell chartLegendCustomItemCell in m_chartLegendCustomItemCells)
					{
						chartLegendCustomItem.m_chartLegendCustomItemCells.Add((ChartLegendCustomItemCell)chartLegendCustomItemCell.PublishClone(context));
					}
					return chartLegendCustomItem;
				}
			}
			return chartLegendCustomItem;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Name, Token.String));
			list.Add(new MemberInfo(MemberName.Action, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Action));
			list.Add(new MemberInfo(MemberName.Marker, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartMarker));
			list.Add(new MemberInfo(MemberName.Separator, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.SeparatorColor, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ToolTip, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ChartLegendCustomItemCells, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartLegendCustomItemCell));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.ID, Token.Int32));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartLegendCustomItem, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartStyleContainer, list);
		}

		internal ChartSeparators EvaluateSeparator(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return EnumTranslator.TranslateChartSeparator(context.ReportRuntime.EvaluateChartLegendCustomItemSeparatorExpression(this, m_chart.Name), context.ReportRuntime);
		}

		internal string EvaluateSeparatorColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendCustomItemSeparatorColorExpression(this, m_chart.Name);
		}

		internal string EvaluateToolTip(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendCustomItemToolTipExpression(this, m_chart.Name);
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
				case MemberName.Marker:
					writer.Write(m_marker);
					break;
				case MemberName.Separator:
					writer.Write(m_separator);
					break;
				case MemberName.SeparatorColor:
					writer.Write(m_separatorColor);
					break;
				case MemberName.ToolTip:
					writer.Write(m_toolTip);
					break;
				case MemberName.ChartLegendCustomItemCells:
					writer.Write(m_chartLegendCustomItemCells);
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
				case MemberName.Marker:
					m_marker = (ChartMarker)reader.ReadRIFObject();
					break;
				case MemberName.Separator:
					m_separator = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.SeparatorColor:
					m_separatorColor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ToolTip:
					m_toolTip = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ChartLegendCustomItemCells:
					m_chartLegendCustomItemCells = reader.ReadGenericListOfRIFObjects<ChartLegendCustomItemCell>();
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
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartLegendCustomItem;
		}
	}
}
