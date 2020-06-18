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
	internal class GaugePointer : GaugePanelStyleContainer, IPersistable, IActionOwner
	{
		private Action m_action;

		protected int m_exprHostID;

		[NonSerialized]
		private List<string> m_fieldsUsedInValueExpression;

		[NonSerialized]
		protected GaugePointerExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		protected string m_name;

		private GaugeInputValue m_gaugeInputValue;

		private ExpressionInfo m_barStart;

		private ExpressionInfo m_distanceFromScale;

		private PointerImage m_pointerImage;

		private ExpressionInfo m_markerLength;

		private ExpressionInfo m_markerStyle;

		private ExpressionInfo m_placement;

		private ExpressionInfo m_snappingEnabled;

		private ExpressionInfo m_snappingInterval;

		private ExpressionInfo m_toolTip;

		private ExpressionInfo m_hidden;

		private ExpressionInfo m_width;

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

		internal GaugeInputValue GaugeInputValue
		{
			get
			{
				return m_gaugeInputValue;
			}
			set
			{
				m_gaugeInputValue = value;
			}
		}

		internal ExpressionInfo BarStart
		{
			get
			{
				return m_barStart;
			}
			set
			{
				m_barStart = value;
			}
		}

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

		internal PointerImage PointerImage
		{
			get
			{
				return m_pointerImage;
			}
			set
			{
				m_pointerImage = value;
			}
		}

		internal ExpressionInfo MarkerLength
		{
			get
			{
				return m_markerLength;
			}
			set
			{
				m_markerLength = value;
			}
		}

		internal ExpressionInfo MarkerStyle
		{
			get
			{
				return m_markerStyle;
			}
			set
			{
				m_markerStyle = value;
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

		internal ExpressionInfo SnappingEnabled
		{
			get
			{
				return m_snappingEnabled;
			}
			set
			{
				m_snappingEnabled = value;
			}
		}

		internal ExpressionInfo SnappingInterval
		{
			get
			{
				return m_snappingInterval;
			}
			set
			{
				m_snappingInterval = value;
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

		internal string OwnerName => m_gaugePanel.Name;

		internal GaugePointerExprHost ExprHost => m_exprHost;

		internal int ExpressionHostID => m_exprHostID;

		internal GaugePointer()
		{
		}

		internal GaugePointer(GaugePanel gaugePanel, int id)
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
			if (m_barStart != null)
			{
				m_barStart.Initialize("BarStart", context);
				context.ExprHostBuilder.GaugePointerBarStart(m_barStart);
			}
			if (m_distanceFromScale != null)
			{
				m_distanceFromScale.Initialize("DistanceFromScale", context);
				context.ExprHostBuilder.GaugePointerDistanceFromScale(m_distanceFromScale);
			}
			if (m_pointerImage != null)
			{
				m_pointerImage.Initialize(context);
			}
			if (m_markerLength != null)
			{
				m_markerLength.Initialize("MarkerLength", context);
				context.ExprHostBuilder.GaugePointerMarkerLength(m_markerLength);
			}
			if (m_markerStyle != null)
			{
				m_markerStyle.Initialize("MarkerStyle", context);
				context.ExprHostBuilder.GaugePointerMarkerStyle(m_markerStyle);
			}
			if (m_placement != null)
			{
				m_placement.Initialize("Placement", context);
				context.ExprHostBuilder.GaugePointerPlacement(m_placement);
			}
			if (m_snappingEnabled != null)
			{
				m_snappingEnabled.Initialize("SnappingEnabled", context);
				context.ExprHostBuilder.GaugePointerSnappingEnabled(m_snappingEnabled);
			}
			if (m_snappingInterval != null)
			{
				m_snappingInterval.Initialize("SnappingInterval", context);
				context.ExprHostBuilder.GaugePointerSnappingInterval(m_snappingInterval);
			}
			if (m_toolTip != null)
			{
				m_toolTip.Initialize("ToolTip", context);
				context.ExprHostBuilder.GaugePointerToolTip(m_toolTip);
			}
			if (m_hidden != null)
			{
				m_hidden.Initialize("Hidden", context);
				context.ExprHostBuilder.GaugePointerHidden(m_hidden);
			}
			if (m_width != null)
			{
				m_width.Initialize("Width", context);
				context.ExprHostBuilder.GaugePointerWidth(m_width);
			}
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			GaugePointer gaugePointer = (GaugePointer)base.PublishClone(context);
			if (m_action != null)
			{
				gaugePointer.m_action = (Action)m_action.PublishClone(context);
			}
			if (m_gaugeInputValue != null)
			{
				gaugePointer.m_gaugeInputValue = (GaugeInputValue)m_gaugeInputValue.PublishClone(context);
			}
			if (m_barStart != null)
			{
				gaugePointer.m_barStart = (ExpressionInfo)m_barStart.PublishClone(context);
			}
			if (m_distanceFromScale != null)
			{
				gaugePointer.m_distanceFromScale = (ExpressionInfo)m_distanceFromScale.PublishClone(context);
			}
			if (m_pointerImage != null)
			{
				gaugePointer.m_pointerImage = (PointerImage)m_pointerImage.PublishClone(context);
			}
			if (m_markerLength != null)
			{
				gaugePointer.m_markerLength = (ExpressionInfo)m_markerLength.PublishClone(context);
			}
			if (m_markerStyle != null)
			{
				gaugePointer.m_markerStyle = (ExpressionInfo)m_markerStyle.PublishClone(context);
			}
			if (m_placement != null)
			{
				gaugePointer.m_placement = (ExpressionInfo)m_placement.PublishClone(context);
			}
			if (m_snappingEnabled != null)
			{
				gaugePointer.m_snappingEnabled = (ExpressionInfo)m_snappingEnabled.PublishClone(context);
			}
			if (m_snappingInterval != null)
			{
				gaugePointer.m_snappingInterval = (ExpressionInfo)m_snappingInterval.PublishClone(context);
			}
			if (m_toolTip != null)
			{
				gaugePointer.m_toolTip = (ExpressionInfo)m_toolTip.PublishClone(context);
			}
			if (m_hidden != null)
			{
				gaugePointer.m_hidden = (ExpressionInfo)m_hidden.PublishClone(context);
			}
			if (m_width != null)
			{
				gaugePointer.m_width = (ExpressionInfo)m_width.PublishClone(context);
			}
			return gaugePointer;
		}

		internal void SetExprHost(GaugePointerExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			base.SetExprHost(exprHost, reportObjectModel);
			m_exprHost = exprHost;
			if (m_pointerImage != null && m_exprHost.PointerImageHost != null)
			{
				m_pointerImage.SetExprHost(m_exprHost.PointerImageHost, reportObjectModel);
			}
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
			list.Add(new MemberInfo(MemberName.GaugeInputValue, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeInputValue));
			list.Add(new MemberInfo(MemberName.BarStart, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.DistanceFromScale, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.PointerImage, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PointerImage));
			list.Add(new MemberInfo(MemberName.MarkerLength, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MarkerStyle, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Placement, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.SnappingEnabled, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.SnappingInterval, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ToolTip, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Hidden, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Width, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.ID, Token.Int32));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePointer, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePanelStyleContainer, list);
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
				case MemberName.GaugeInputValue:
					writer.Write(m_gaugeInputValue);
					break;
				case MemberName.BarStart:
					writer.Write(m_barStart);
					break;
				case MemberName.DistanceFromScale:
					writer.Write(m_distanceFromScale);
					break;
				case MemberName.PointerImage:
					writer.Write(m_pointerImage);
					break;
				case MemberName.MarkerLength:
					writer.Write(m_markerLength);
					break;
				case MemberName.MarkerStyle:
					writer.Write(m_markerStyle);
					break;
				case MemberName.Placement:
					writer.Write(m_placement);
					break;
				case MemberName.SnappingEnabled:
					writer.Write(m_snappingEnabled);
					break;
				case MemberName.SnappingInterval:
					writer.Write(m_snappingInterval);
					break;
				case MemberName.ToolTip:
					writer.Write(m_toolTip);
					break;
				case MemberName.Hidden:
					writer.Write(m_hidden);
					break;
				case MemberName.Width:
					writer.Write(m_width);
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
				case MemberName.GaugeInputValue:
					m_gaugeInputValue = (GaugeInputValue)reader.ReadRIFObject();
					break;
				case MemberName.BarStart:
					m_barStart = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.DistanceFromScale:
					m_distanceFromScale = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.PointerImage:
					m_pointerImage = (PointerImage)reader.ReadRIFObject();
					break;
				case MemberName.MarkerLength:
					m_markerLength = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MarkerStyle:
					m_markerStyle = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Placement:
					m_placement = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.SnappingEnabled:
					m_snappingEnabled = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.SnappingInterval:
					m_snappingInterval = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ToolTip:
					m_toolTip = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Hidden:
					m_hidden = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Width:
					m_width = (ExpressionInfo)reader.ReadRIFObject();
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
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePointer;
		}

		internal GaugeBarStarts EvaluateBarStart(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return EnumTranslator.TranslateGaugeBarStarts(context.ReportRuntime.EvaluateGaugePointerBarStartExpression(this, m_gaugePanel.Name), context.ReportRuntime);
		}

		internal double EvaluateDistanceFromScale(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugePointerDistanceFromScaleExpression(this, m_gaugePanel.Name);
		}

		internal double EvaluateMarkerLength(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugePointerMarkerLengthExpression(this, m_gaugePanel.Name);
		}

		internal GaugeMarkerStyles EvaluateMarkerStyle(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return EnumTranslator.TranslateGaugeMarkerStyles(context.ReportRuntime.EvaluateGaugePointerMarkerStyleExpression(this, m_gaugePanel.Name), context.ReportRuntime);
		}

		internal GaugePointerPlacements EvaluatePlacement(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return EnumTranslator.TranslateGaugePointerPlacements(context.ReportRuntime.EvaluateGaugePointerPlacementExpression(this, m_gaugePanel.Name), context.ReportRuntime);
		}

		internal bool EvaluateSnappingEnabled(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugePointerSnappingEnabledExpression(this, m_gaugePanel.Name);
		}

		internal double EvaluateSnappingInterval(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugePointerSnappingIntervalExpression(this, m_gaugePanel.Name);
		}

		internal string EvaluateToolTip(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugePointerToolTipExpression(this, m_gaugePanel.Name);
		}

		internal bool EvaluateHidden(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugePointerHiddenExpression(this, m_gaugePanel.Name);
		}

		internal double EvaluateWidth(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugePointerWidthExpression(this, m_gaugePanel.Name);
		}
	}
}
