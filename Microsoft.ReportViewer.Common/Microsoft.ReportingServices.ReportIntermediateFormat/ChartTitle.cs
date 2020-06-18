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
	internal class ChartTitle : ChartTitleBase, IPersistable, IActionOwner
	{
		private string m_name;

		private ExpressionInfo m_position;

		protected int m_exprHostID;

		private ExpressionInfo m_hidden;

		private ExpressionInfo m_docking;

		private string m_dockToChartArea;

		private ExpressionInfo m_dockOutsideChartArea;

		private ExpressionInfo m_dockOffset;

		private ExpressionInfo m_toolTip;

		private Action m_action;

		private ExpressionInfo m_textOrientation;

		private ChartElementPosition m_chartElementPosition;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

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
				return null;
			}
			set
			{
			}
		}

		internal string TitleName
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

		internal ExpressionInfo Docking
		{
			get
			{
				return m_docking;
			}
			set
			{
				m_docking = value;
			}
		}

		internal string DockToChartArea
		{
			get
			{
				return m_dockToChartArea;
			}
			set
			{
				m_dockToChartArea = value;
			}
		}

		internal ExpressionInfo DockOutsideChartArea
		{
			get
			{
				return m_dockOutsideChartArea;
			}
			set
			{
				m_dockOutsideChartArea = value;
			}
		}

		internal ExpressionInfo DockOffset
		{
			get
			{
				return m_dockOffset;
			}
			set
			{
				m_dockOffset = value;
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

		internal ExpressionInfo TextOrientation
		{
			get
			{
				return m_textOrientation;
			}
			set
			{
				m_textOrientation = value;
			}
		}

		internal ChartElementPosition ChartElementPosition
		{
			get
			{
				return m_chartElementPosition;
			}
			set
			{
				m_chartElementPosition = value;
			}
		}

		internal int ExpressionHostID => m_exprHostID;

		internal ChartTitle()
		{
		}

		internal ChartTitle(Chart chart)
			: base(chart)
		{
			m_chart = chart;
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.ChartTitleStart(m_name);
			InitializeInternal(context);
			m_exprHostID = context.ExprHostBuilder.ChartTitleEnd();
		}

		protected void InitializeInternal(InitializationContext context)
		{
			base.Initialize(context);
			if (m_position != null)
			{
				m_position.Initialize("Position", context);
				context.ExprHostBuilder.ChartTitlePosition(m_position);
			}
			if (m_hidden != null)
			{
				m_hidden.Initialize("Hidden", context);
				context.ExprHostBuilder.ChartTitleHidden(m_hidden);
			}
			if (m_docking != null)
			{
				m_docking.Initialize("Docking", context);
				context.ExprHostBuilder.ChartTitleDocking(m_docking);
			}
			_ = m_dockToChartArea;
			if (m_dockOutsideChartArea != null)
			{
				m_dockOutsideChartArea.Initialize("DockOutsideChartArea", context);
				context.ExprHostBuilder.ChartTitleDockOutsideChartArea(m_dockOutsideChartArea);
			}
			if (m_dockOffset != null)
			{
				m_dockOffset.Initialize("DockOffset", context);
				context.ExprHostBuilder.ChartTitleDockOffset(m_dockOffset);
			}
			if (m_toolTip != null)
			{
				m_toolTip.Initialize("ToolTip", context);
				context.ExprHostBuilder.ChartTitleToolTip(m_toolTip);
			}
			if (m_action != null)
			{
				m_action.Initialize(context);
			}
			if (m_textOrientation != null)
			{
				m_textOrientation.Initialize("TextOrientation", context);
				context.ExprHostBuilder.ChartTitleTextOrientation(m_textOrientation);
			}
			if (m_chartElementPosition != null)
			{
				m_chartElementPosition.Initialize(context);
			}
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			ChartTitle chartTitle = (ChartTitle)base.PublishClone(context);
			if (m_position != null)
			{
				chartTitle.m_position = (ExpressionInfo)m_position.PublishClone(context);
			}
			if (m_hidden != null)
			{
				chartTitle.m_hidden = (ExpressionInfo)m_hidden.PublishClone(context);
			}
			if (m_docking != null)
			{
				chartTitle.m_docking = (ExpressionInfo)m_docking.PublishClone(context);
			}
			if (m_dockToChartArea != null)
			{
				chartTitle.m_dockToChartArea = (string)m_dockToChartArea.Clone();
			}
			if (m_dockOutsideChartArea != null)
			{
				chartTitle.m_dockOutsideChartArea = (ExpressionInfo)m_dockOutsideChartArea.PublishClone(context);
			}
			if (m_dockOffset != null)
			{
				chartTitle.m_dockOffset = (ExpressionInfo)m_dockOffset.PublishClone(context);
			}
			if (m_toolTip != null)
			{
				chartTitle.m_toolTip = (ExpressionInfo)m_toolTip.PublishClone(context);
			}
			if (m_action != null)
			{
				chartTitle.m_action = (Action)m_action.PublishClone(context);
			}
			if (m_textOrientation != null)
			{
				chartTitle.m_textOrientation = (ExpressionInfo)m_textOrientation.PublishClone(context);
			}
			if (m_chartElementPosition != null)
			{
				chartTitle.m_chartElementPosition = (ChartElementPosition)m_chartElementPosition.PublishClone(context);
			}
			return chartTitle;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Name, Token.String));
			list.Add(new MemberInfo(MemberName.Position, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.Hidden, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Docking, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.DockToChartArea, Token.String));
			list.Add(new MemberInfo(MemberName.DockOutsideChartArea, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.DockOffset, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ToolTip, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Action, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Action));
			list.Add(new MemberInfo(MemberName.TextOrientation, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ChartElementPosition, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartElementPosition));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartTitle, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartTitleBase, list);
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
				case MemberName.Position:
					writer.Write(m_position);
					break;
				case MemberName.ExprHostID:
					writer.Write(m_exprHostID);
					break;
				case MemberName.Hidden:
					writer.Write(m_hidden);
					break;
				case MemberName.Docking:
					writer.Write(m_docking);
					break;
				case MemberName.DockToChartArea:
					writer.Write(m_dockToChartArea);
					break;
				case MemberName.DockOutsideChartArea:
					writer.Write(m_dockOutsideChartArea);
					break;
				case MemberName.DockOffset:
					writer.Write(m_dockOffset);
					break;
				case MemberName.ToolTip:
					writer.Write(m_toolTip);
					break;
				case MemberName.Action:
					writer.Write(m_action);
					break;
				case MemberName.TextOrientation:
					writer.Write(m_textOrientation);
					break;
				case MemberName.ChartElementPosition:
					writer.Write(m_chartElementPosition);
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
				case MemberName.Position:
					m_position = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ExprHostID:
					m_exprHostID = reader.ReadInt32();
					break;
				case MemberName.Hidden:
					m_hidden = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Docking:
					m_docking = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.DockToChartArea:
					m_dockToChartArea = reader.ReadString();
					break;
				case MemberName.DockOutsideChartArea:
					m_dockOutsideChartArea = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.DockOffset:
					m_dockOffset = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ToolTip:
					m_toolTip = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Action:
					m_action = (Action)reader.ReadRIFObject();
					break;
				case MemberName.TextOrientation:
					m_textOrientation = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ChartElementPosition:
					m_chartElementPosition = (ChartElementPosition)reader.ReadRIFObject();
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
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartTitle;
		}

		internal override void SetExprHost(StyleExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
			if (m_action != null && ((ChartTitleExprHost)exprHost).ActionInfoHost != null)
			{
				m_action.SetExprHost(((ChartTitleExprHost)exprHost).ActionInfoHost, reportObjectModel);
			}
			if (m_chartElementPosition != null && ((ChartTitleExprHost)exprHost).ChartElementPositionHost != null)
			{
				m_chartElementPosition.SetExprHost(((ChartTitleExprHost)exprHost).ChartElementPositionHost, reportObjectModel);
			}
		}

		internal bool EvaluateHidden(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, instance);
			return context.ReportRuntime.EvaluateEvaluateChartTitleHiddenExpression(this, base.Name, "Hidden");
		}

		internal ChartTitleDockings EvaluateDocking(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, instance);
			return EnumTranslator.TranslateChartTitleDocking(context.ReportRuntime.EvaluateChartTitleDockingExpression(this, base.Name, "Docking"), context.ReportRuntime);
		}

		internal ChartTitlePositions EvaluatePosition(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, instance);
			return EnumTranslator.TranslateChartTitlePosition(context.ReportRuntime.EvaluateChartTitlePositionExpression(this, base.Name, "Position"), context.ReportRuntime);
		}

		internal bool EvaluateDockOutsideChartArea(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, instance);
			return context.ReportRuntime.EvaluateChartTitleDockOutsideChartAreaExpression(this, base.Name, "DockOutsideChartArea");
		}

		internal int EvaluateDockOffset(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, instance);
			return context.ReportRuntime.EvaluateChartTitleDockOffsetExpression(this, base.Name, "DockOffset");
		}

		internal string EvaluateToolTip(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, instance);
			return context.ReportRuntime.EvaluateChartTitleToolTipExpression(this, base.Name, "ToolTip");
		}

		internal TextOrientations EvaluateTextOrientation(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return EnumTranslator.TranslateTextOrientations(context.ReportRuntime.EvaluateChartTitleTextOrientationExpression(this, m_chart.Name), context.ReportRuntime);
		}
	}
}
