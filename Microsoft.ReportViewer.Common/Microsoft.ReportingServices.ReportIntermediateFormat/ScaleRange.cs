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
	internal sealed class ScaleRange : GaugePanelStyleContainer, IPersistable, IActionOwner
	{
		private Action m_action;

		private int m_exprHostID;

		[NonSerialized]
		private List<string> m_fieldsUsedInValueExpression;

		[NonSerialized]
		private ScaleRangeExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private string m_name;

		private ExpressionInfo m_distanceFromScale;

		private GaugeInputValue m_startValue;

		private GaugeInputValue m_endValue;

		private ExpressionInfo m_startWidth;

		private ExpressionInfo m_endWidth;

		private ExpressionInfo m_inRangeBarPointerColor;

		private ExpressionInfo m_inRangeLabelColor;

		private ExpressionInfo m_inRangeTickMarksColor;

		private ExpressionInfo m_backgroundGradientType;

		private ExpressionInfo m_placement;

		private ExpressionInfo m_toolTip;

		private ExpressionInfo m_hidden;

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

		internal ExpressionInfo DistanceFromScale
		{
			get
			{
				return m_distanceFromScale;
			}
			set
			{
				m_distanceFromScale = value;
			}
		}

		internal GaugeInputValue StartValue
		{
			get
			{
				return m_startValue;
			}
			set
			{
				m_startValue = value;
			}
		}

		internal GaugeInputValue EndValue
		{
			get
			{
				return m_endValue;
			}
			set
			{
				m_endValue = value;
			}
		}

		internal ExpressionInfo StartWidth
		{
			get
			{
				return m_startWidth;
			}
			set
			{
				m_startWidth = value;
			}
		}

		internal ExpressionInfo EndWidth
		{
			get
			{
				return m_endWidth;
			}
			set
			{
				m_endWidth = value;
			}
		}

		internal ExpressionInfo InRangeBarPointerColor
		{
			get
			{
				return m_inRangeBarPointerColor;
			}
			set
			{
				m_inRangeBarPointerColor = value;
			}
		}

		internal ExpressionInfo InRangeLabelColor
		{
			get
			{
				return m_inRangeLabelColor;
			}
			set
			{
				m_inRangeLabelColor = value;
			}
		}

		internal ExpressionInfo InRangeTickMarksColor
		{
			get
			{
				return m_inRangeTickMarksColor;
			}
			set
			{
				m_inRangeTickMarksColor = value;
			}
		}

		internal ExpressionInfo BackgroundGradientType
		{
			get
			{
				return m_backgroundGradientType;
			}
			set
			{
				m_backgroundGradientType = value;
			}
		}

		internal ExpressionInfo Placement
		{
			get
			{
				return m_placement;
			}
			set
			{
				m_placement = value;
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

		internal string OwnerName => m_gaugePanel.Name;

		internal ScaleRangeExprHost ExprHost => m_exprHost;

		internal int ExpressionHostID => m_exprHostID;

		internal ScaleRange()
		{
		}

		internal ScaleRange(GaugePanel gaugePanel, int id)
			: base(gaugePanel)
		{
			m_id = id;
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.ScaleRangeStart(m_name);
			base.Initialize(context);
			if (m_action != null)
			{
				m_action.Initialize(context);
			}
			if (m_distanceFromScale != null)
			{
				m_distanceFromScale.Initialize("DistanceFromScale", context);
				context.ExprHostBuilder.ScaleRangeDistanceFromScale(m_distanceFromScale);
			}
			if (m_startWidth != null)
			{
				m_startWidth.Initialize("StartWidth", context);
				context.ExprHostBuilder.ScaleRangeStartWidth(m_startWidth);
			}
			if (m_endWidth != null)
			{
				m_endWidth.Initialize("EndWidth", context);
				context.ExprHostBuilder.ScaleRangeEndWidth(m_endWidth);
			}
			if (m_inRangeBarPointerColor != null)
			{
				m_inRangeBarPointerColor.Initialize("InRangeBarPointerColor", context);
				context.ExprHostBuilder.ScaleRangeInRangeBarPointerColor(m_inRangeBarPointerColor);
			}
			if (m_inRangeLabelColor != null)
			{
				m_inRangeLabelColor.Initialize("InRangeLabelColor", context);
				context.ExprHostBuilder.ScaleRangeInRangeLabelColor(m_inRangeLabelColor);
			}
			if (m_inRangeTickMarksColor != null)
			{
				m_inRangeTickMarksColor.Initialize("InRangeTickMarksColor", context);
				context.ExprHostBuilder.ScaleRangeInRangeTickMarksColor(m_inRangeTickMarksColor);
			}
			if (m_backgroundGradientType != null)
			{
				m_backgroundGradientType.Initialize("BackgroundGradientType", context);
				context.ExprHostBuilder.ScaleRangeBackgroundGradientType(m_backgroundGradientType);
			}
			if (m_placement != null)
			{
				m_placement.Initialize("Placement", context);
				context.ExprHostBuilder.ScaleRangePlacement(m_placement);
			}
			if (m_toolTip != null)
			{
				m_toolTip.Initialize("ToolTip", context);
				context.ExprHostBuilder.ScaleRangeToolTip(m_toolTip);
			}
			if (m_hidden != null)
			{
				m_hidden.Initialize("Hidden", context);
				context.ExprHostBuilder.ScaleRangeHidden(m_hidden);
			}
			m_exprHostID = context.ExprHostBuilder.ScaleRangeEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			ScaleRange scaleRange = (ScaleRange)base.PublishClone(context);
			if (m_action != null)
			{
				scaleRange.m_action = (Action)m_action.PublishClone(context);
			}
			if (m_distanceFromScale != null)
			{
				scaleRange.m_distanceFromScale = (ExpressionInfo)m_distanceFromScale.PublishClone(context);
			}
			if (m_startValue != null)
			{
				scaleRange.m_startValue = (GaugeInputValue)m_startValue.PublishClone(context);
			}
			if (m_endValue != null)
			{
				scaleRange.m_endValue = (GaugeInputValue)m_endValue.PublishClone(context);
			}
			if (m_startWidth != null)
			{
				scaleRange.m_startWidth = (ExpressionInfo)m_startWidth.PublishClone(context);
			}
			if (m_endWidth != null)
			{
				scaleRange.m_endWidth = (ExpressionInfo)m_endWidth.PublishClone(context);
			}
			if (m_inRangeBarPointerColor != null)
			{
				scaleRange.m_inRangeBarPointerColor = (ExpressionInfo)m_inRangeBarPointerColor.PublishClone(context);
			}
			if (m_inRangeLabelColor != null)
			{
				scaleRange.m_inRangeLabelColor = (ExpressionInfo)m_inRangeLabelColor.PublishClone(context);
			}
			if (m_inRangeTickMarksColor != null)
			{
				scaleRange.m_inRangeTickMarksColor = (ExpressionInfo)m_inRangeTickMarksColor.PublishClone(context);
			}
			if (m_backgroundGradientType != null)
			{
				scaleRange.m_backgroundGradientType = (ExpressionInfo)m_backgroundGradientType.PublishClone(context);
			}
			if (m_placement != null)
			{
				scaleRange.m_placement = (ExpressionInfo)m_placement.PublishClone(context);
			}
			if (m_toolTip != null)
			{
				scaleRange.m_toolTip = (ExpressionInfo)m_toolTip.PublishClone(context);
			}
			if (m_hidden != null)
			{
				scaleRange.m_hidden = (ExpressionInfo)m_hidden.PublishClone(context);
			}
			return scaleRange;
		}

		internal void SetExprHost(ScaleRangeExprHost exprHost, ObjectModelImpl reportObjectModel)
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
			list.Add(new MemberInfo(MemberName.DistanceFromScale, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.StartValue, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeInputValue));
			list.Add(new MemberInfo(MemberName.EndValue, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeInputValue));
			list.Add(new MemberInfo(MemberName.StartWidth, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.EndWidth, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.InRangeBarPointerColor, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.InRangeLabelColor, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.InRangeTickMarksColor, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.BackgroundGradientType, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Placement, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ToolTip, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Hidden, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.ID, Token.Int32));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScaleRange, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePanelStyleContainer, list);
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
				case MemberName.DistanceFromScale:
					writer.Write(m_distanceFromScale);
					break;
				case MemberName.StartValue:
					writer.Write(m_startValue);
					break;
				case MemberName.EndValue:
					writer.Write(m_endValue);
					break;
				case MemberName.StartWidth:
					writer.Write(m_startWidth);
					break;
				case MemberName.EndWidth:
					writer.Write(m_endWidth);
					break;
				case MemberName.InRangeBarPointerColor:
					writer.Write(m_inRangeBarPointerColor);
					break;
				case MemberName.InRangeLabelColor:
					writer.Write(m_inRangeLabelColor);
					break;
				case MemberName.InRangeTickMarksColor:
					writer.Write(m_inRangeTickMarksColor);
					break;
				case MemberName.BackgroundGradientType:
					writer.Write(m_backgroundGradientType);
					break;
				case MemberName.Placement:
					writer.Write(m_placement);
					break;
				case MemberName.ToolTip:
					writer.Write(m_toolTip);
					break;
				case MemberName.Hidden:
					writer.Write(m_hidden);
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
				case MemberName.DistanceFromScale:
					m_distanceFromScale = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.StartValue:
					m_startValue = (GaugeInputValue)reader.ReadRIFObject();
					break;
				case MemberName.EndValue:
					m_endValue = (GaugeInputValue)reader.ReadRIFObject();
					break;
				case MemberName.StartWidth:
					m_startWidth = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.EndWidth:
					m_endWidth = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.InRangeBarPointerColor:
					m_inRangeBarPointerColor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.InRangeLabelColor:
					m_inRangeLabelColor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.InRangeTickMarksColor:
					m_inRangeTickMarksColor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.BackgroundGradientType:
					m_backgroundGradientType = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Placement:
					m_placement = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ToolTip:
					m_toolTip = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Hidden:
					m_hidden = (ExpressionInfo)reader.ReadRIFObject();
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
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScaleRange;
		}

		internal double EvaluateDistanceFromScale(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateScaleRangeDistanceFromScaleExpression(this, m_gaugePanel.Name);
		}

		internal double EvaluateStartWidth(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateScaleRangeStartWidthExpression(this, m_gaugePanel.Name);
		}

		internal double EvaluateEndWidth(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateScaleRangeEndWidthExpression(this, m_gaugePanel.Name);
		}

		internal string EvaluateInRangeBarPointerColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateScaleRangeInRangeBarPointerColorExpression(this, m_gaugePanel.Name);
		}

		internal string EvaluateInRangeLabelColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateScaleRangeInRangeLabelColorExpression(this, m_gaugePanel.Name);
		}

		internal string EvaluateInRangeTickMarksColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateScaleRangeInRangeTickMarksColorExpression(this, m_gaugePanel.Name);
		}

		internal BackgroundGradientTypes EvaluateBackgroundGradientType(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return EnumTranslator.TranslateBackgroundGradientTypes(context.ReportRuntime.EvaluateScaleRangeBackgroundGradientTypeExpression(this, m_gaugePanel.Name), context.ReportRuntime);
		}

		internal ScaleRangePlacements EvaluatePlacement(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return EnumTranslator.TranslateScaleRangePlacements(context.ReportRuntime.EvaluateScaleRangePlacementExpression(this, m_gaugePanel.Name), context.ReportRuntime);
		}

		internal string EvaluateToolTip(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateScaleRangeToolTipExpression(this, m_gaugePanel.Name);
		}

		internal bool EvaluateHidden(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateScaleRangeHiddenExpression(this, m_gaugePanel.Name);
		}
	}
}
