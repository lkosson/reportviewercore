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
	internal class MapDockableSubItem : MapSubItem, IPersistable, IActionOwner
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private Action m_action;

		private int m_id;

		[NonSerialized]
		private List<string> m_fieldsUsedInValueExpression;

		private ExpressionInfo m_position;

		private ExpressionInfo m_dockOutsideViewport;

		private ExpressionInfo m_hidden;

		private ExpressionInfo m_toolTip;

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

		internal int ID => m_id;

		internal ExpressionInfo Position
		{
			get
			{
				return m_position;
			}
			set
			{
				m_position = value;
			}
		}

		internal ExpressionInfo DockOutsideViewport
		{
			get
			{
				return m_dockOutsideViewport;
			}
			set
			{
				m_dockOutsideViewport = value;
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

		internal new MapDockableSubItemExprHost ExprHost => (MapDockableSubItemExprHost)m_exprHost;

		internal MapDockableSubItem()
		{
		}

		internal MapDockableSubItem(Map map, int id)
			: base(map)
		{
			m_id = id;
		}

		internal override void Initialize(InitializationContext context)
		{
			base.Initialize(context);
			if (m_action != null)
			{
				m_action.Initialize(context);
			}
			if (m_position != null)
			{
				m_position.Initialize("Position", context);
				context.ExprHostBuilder.MapDockableSubItemPosition(m_position);
			}
			if (m_dockOutsideViewport != null)
			{
				m_dockOutsideViewport.Initialize("DockOutsideViewport", context);
				context.ExprHostBuilder.MapDockableSubItemDockOutsideViewport(m_dockOutsideViewport);
			}
			if (m_hidden != null)
			{
				m_hidden.Initialize("Hidden", context);
				context.ExprHostBuilder.MapDockableSubItemHidden(m_hidden);
			}
			if (m_toolTip != null)
			{
				m_toolTip.Initialize("ToolTip", context);
				context.ExprHostBuilder.MapDockableSubItemToolTip(m_toolTip);
			}
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapDockableSubItem mapDockableSubItem = (MapDockableSubItem)base.PublishClone(context);
			if (m_action != null)
			{
				mapDockableSubItem.m_action = (Action)m_action.PublishClone(context);
			}
			if (m_position != null)
			{
				mapDockableSubItem.m_position = (ExpressionInfo)m_position.PublishClone(context);
			}
			if (m_dockOutsideViewport != null)
			{
				mapDockableSubItem.m_dockOutsideViewport = (ExpressionInfo)m_dockOutsideViewport.PublishClone(context);
			}
			if (m_hidden != null)
			{
				mapDockableSubItem.m_hidden = (ExpressionInfo)m_hidden.PublishClone(context);
			}
			if (m_toolTip != null)
			{
				mapDockableSubItem.m_toolTip = (ExpressionInfo)m_toolTip.PublishClone(context);
			}
			return mapDockableSubItem;
		}

		internal void SetExprHost(MapDockableSubItemExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			SetExprHost((MapSubItemExprHost)exprHost, reportObjectModel);
			if (m_action != null && ExprHost.ActionInfoHost != null)
			{
				m_action.SetExprHost(ExprHost.ActionInfoHost, reportObjectModel);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Action, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Action));
			list.Add(new MemberInfo(MemberName.ID, Token.Int32));
			list.Add(new MemberInfo(MemberName.Position, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.DockOutsideViewport, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Hidden, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ToolTip, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapDockableSubItem, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapSubItem, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Action:
					writer.Write(m_action);
					break;
				case MemberName.ID:
					writer.Write(m_id);
					break;
				case MemberName.Position:
					writer.Write(m_position);
					break;
				case MemberName.DockOutsideViewport:
					writer.Write(m_dockOutsideViewport);
					break;
				case MemberName.Hidden:
					writer.Write(m_hidden);
					break;
				case MemberName.ToolTip:
					writer.Write(m_toolTip);
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
				case MemberName.Action:
					m_action = (Action)reader.ReadRIFObject();
					break;
				case MemberName.ID:
					m_id = reader.ReadInt32();
					break;
				case MemberName.Position:
					m_position = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.DockOutsideViewport:
					m_dockOutsideViewport = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Hidden:
					m_hidden = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ToolTip:
					m_toolTip = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapDockableSubItem;
		}

		internal MapPosition EvaluatePosition(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return EnumTranslator.TranslateMapPosition(context.ReportRuntime.EvaluateMapDockableSubItemPositionExpression(this, m_map.Name), context.ReportRuntime);
		}

		internal bool EvaluateDockOutsideViewport(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapDockableSubItemDockOutsideViewportExpression(this, m_map.Name);
		}

		internal bool EvaluateHidden(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapDockableSubItemHiddenExpression(this, m_map.Name);
		}

		internal string EvaluateToolTip(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			Microsoft.ReportingServices.RdlExpressions.VariantResult result = context.ReportRuntime.EvaluateMapDockableSubItemToolTipExpression(this, m_map.Name);
			return m_map.GetFormattedStringFromValue(ref result, context);
		}
	}
}
