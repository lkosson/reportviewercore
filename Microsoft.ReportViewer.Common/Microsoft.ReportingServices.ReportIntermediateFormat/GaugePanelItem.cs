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
	internal class GaugePanelItem : GaugePanelStyleContainer, IPersistable, IActionOwner
	{
		private Action m_action;

		protected int m_exprHostID;

		[NonSerialized]
		private List<string> m_fieldsUsedInValueExpression;

		[NonSerialized]
		protected GaugePanelItemExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		protected string m_name;

		private ExpressionInfo m_top;

		private ExpressionInfo m_left;

		private ExpressionInfo m_height;

		private ExpressionInfo m_width;

		private ExpressionInfo m_zIndex;

		private ExpressionInfo m_hidden;

		private ExpressionInfo m_toolTip;

		private string m_parentItem;

		private int m_id;

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

		internal int ID => m_id;

		internal ExpressionInfo Top
		{
			get
			{
				return m_top;
			}
			set
			{
				m_top = value;
			}
		}

		internal ExpressionInfo Left
		{
			get
			{
				return m_left;
			}
			set
			{
				m_left = value;
			}
		}

		internal ExpressionInfo Height
		{
			get
			{
				return m_height;
			}
			set
			{
				m_height = value;
			}
		}

		internal ExpressionInfo Width
		{
			get
			{
				return m_width;
			}
			set
			{
				m_width = value;
			}
		}

		internal ExpressionInfo ZIndex
		{
			get
			{
				return m_zIndex;
			}
			set
			{
				m_zIndex = value;
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

		internal string ParentItem
		{
			get
			{
				return m_parentItem;
			}
			set
			{
				m_parentItem = value;
			}
		}

		internal string OwnerName => m_gaugePanel.Name;

		internal GaugePanelItemExprHost ExprHost => m_exprHost;

		internal int ExpressionHostID => m_exprHostID;

		internal GaugePanelItem()
		{
		}

		internal GaugePanelItem(GaugePanel gaugePanel, int id)
			: base(gaugePanel)
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
			if (m_top != null)
			{
				m_top.Initialize("Top", context);
				context.ExprHostBuilder.GaugePanelItemTop(m_top);
			}
			if (m_left != null)
			{
				m_left.Initialize("Left", context);
				context.ExprHostBuilder.GaugePanelItemLeft(m_left);
			}
			if (m_height != null)
			{
				m_height.Initialize("Height", context);
				context.ExprHostBuilder.GaugePanelItemHeight(m_height);
			}
			if (m_width != null)
			{
				m_width.Initialize("Width", context);
				context.ExprHostBuilder.GaugePanelItemWidth(m_width);
			}
			if (m_zIndex != null)
			{
				m_zIndex.Initialize("ZIndex", context);
				context.ExprHostBuilder.GaugePanelItemZIndex(m_zIndex);
			}
			if (m_hidden != null)
			{
				m_hidden.Initialize("Hidden", context);
				context.ExprHostBuilder.GaugePanelItemHidden(m_hidden);
			}
			if (m_toolTip != null)
			{
				m_toolTip.Initialize("ToolTip", context);
				context.ExprHostBuilder.GaugePanelItemToolTip(m_toolTip);
			}
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			GaugePanelItem gaugePanelItem = (GaugePanelItem)base.PublishClone(context);
			if (m_action != null)
			{
				gaugePanelItem.m_action = (Action)m_action.PublishClone(context);
			}
			if (m_top != null)
			{
				gaugePanelItem.m_top = (ExpressionInfo)m_top.PublishClone(context);
			}
			if (m_left != null)
			{
				gaugePanelItem.m_left = (ExpressionInfo)m_left.PublishClone(context);
			}
			if (m_height != null)
			{
				gaugePanelItem.m_height = (ExpressionInfo)m_height.PublishClone(context);
			}
			if (m_width != null)
			{
				gaugePanelItem.m_width = (ExpressionInfo)m_width.PublishClone(context);
			}
			if (m_zIndex != null)
			{
				gaugePanelItem.m_zIndex = (ExpressionInfo)m_zIndex.PublishClone(context);
			}
			if (m_hidden != null)
			{
				gaugePanelItem.m_hidden = (ExpressionInfo)m_hidden.PublishClone(context);
			}
			if (m_toolTip != null)
			{
				gaugePanelItem.m_toolTip = (ExpressionInfo)m_toolTip.PublishClone(context);
			}
			return gaugePanelItem;
		}

		internal void SetExprHost(GaugePanelItemExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			base.SetExprHost(exprHost, reportObjectModel);
			m_exprHost = exprHost;
			if (m_action != null && exprHost.ActionInfoHost != null)
			{
				m_action.SetExprHost(exprHost.ActionInfoHost, reportObjectModel);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Name, Token.String));
			list.Add(new MemberInfo(MemberName.Action, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Action));
			list.Add(new MemberInfo(MemberName.Top, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Left, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Height, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Width, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ZIndex, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Hidden, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ToolTip, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ParentItem, Token.String));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.ID, Token.Int32));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePanelItem, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePanelStyleContainer, list);
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
				case MemberName.Name:
					writer.Write(m_name);
					break;
				case MemberName.Top:
					writer.Write(m_top);
					break;
				case MemberName.Left:
					writer.Write(m_left);
					break;
				case MemberName.Height:
					writer.Write(m_height);
					break;
				case MemberName.Width:
					writer.Write(m_width);
					break;
				case MemberName.ZIndex:
					writer.Write(m_zIndex);
					break;
				case MemberName.Hidden:
					writer.Write(m_hidden);
					break;
				case MemberName.ToolTip:
					writer.Write(m_toolTip);
					break;
				case MemberName.ParentItem:
					writer.Write(m_parentItem);
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
				case MemberName.Action:
					m_action = (Action)reader.ReadRIFObject();
					break;
				case MemberName.Name:
					m_name = reader.ReadString();
					break;
				case MemberName.Top:
					m_top = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Left:
					m_left = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Height:
					m_height = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Width:
					m_width = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ZIndex:
					m_zIndex = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Hidden:
					m_hidden = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ToolTip:
					m_toolTip = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ParentItem:
					m_parentItem = reader.ReadString();
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
				m_id = m_gaugePanel.GenerateActionOwnerID();
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePanelItem;
		}

		internal double EvaluateTop(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugePanelItemTopExpression(this, m_gaugePanel.Name);
		}

		internal double EvaluateLeft(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugePanelItemLeftExpression(this, m_gaugePanel.Name);
		}

		internal double EvaluateHeight(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugePanelItemHeightExpression(this, m_gaugePanel.Name);
		}

		internal double EvaluateWidth(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugePanelItemWidthExpression(this, m_gaugePanel.Name);
		}

		internal int EvaluateZIndex(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugePanelItemZIndexExpression(this, m_gaugePanel.Name);
		}

		internal bool EvaluateHidden(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugePanelItemHiddenExpression(this, m_gaugePanel.Name);
		}

		internal string EvaluateToolTip(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugePanelItemToolTipExpression(this, m_gaugePanel.Name);
		}
	}
}
