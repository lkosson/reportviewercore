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
	internal sealed class CustomLabel : GaugePanelStyleContainer, IPersistable
	{
		private int m_exprHostID;

		[NonSerialized]
		private CustomLabelExprHost m_exprHost;

		[NonSerialized]
		private Formatter m_formatter;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private string m_name;

		private ExpressionInfo m_text;

		private ExpressionInfo m_allowUpsideDown;

		private ExpressionInfo m_distanceFromScale;

		private ExpressionInfo m_fontAngle;

		private ExpressionInfo m_placement;

		private ExpressionInfo m_rotateLabel;

		private TickMarkStyle m_tickMarkStyle;

		private ExpressionInfo m_value;

		private ExpressionInfo m_hidden;

		private ExpressionInfo m_useFontPercent;

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

		internal ExpressionInfo AllowUpsideDown
		{
			get
			{
				return m_allowUpsideDown;
			}
			set
			{
				m_allowUpsideDown = value;
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

		internal ExpressionInfo FontAngle
		{
			get
			{
				return m_fontAngle;
			}
			set
			{
				m_fontAngle = value;
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

		internal ExpressionInfo RotateLabel
		{
			get
			{
				return m_rotateLabel;
			}
			set
			{
				m_rotateLabel = value;
			}
		}

		internal TickMarkStyle TickMarkStyle
		{
			get
			{
				return m_tickMarkStyle;
			}
			set
			{
				m_tickMarkStyle = value;
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

		internal ExpressionInfo UseFontPercent
		{
			get
			{
				return m_useFontPercent;
			}
			set
			{
				m_useFontPercent = value;
			}
		}

		internal string OwnerName => m_gaugePanel.Name;

		internal CustomLabelExprHost ExprHost => m_exprHost;

		internal int ExpressionHostID => m_exprHostID;

		internal CustomLabel()
		{
		}

		internal CustomLabel(GaugePanel gaugePanel)
			: base(gaugePanel)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.CustomLabelStart(m_name);
			base.Initialize(context);
			if (m_text != null)
			{
				m_text.Initialize("Text", context);
				context.ExprHostBuilder.CustomLabelText(m_text);
			}
			if (m_allowUpsideDown != null)
			{
				m_allowUpsideDown.Initialize("AllowUpsideDown", context);
				context.ExprHostBuilder.CustomLabelAllowUpsideDown(m_allowUpsideDown);
			}
			if (m_distanceFromScale != null)
			{
				m_distanceFromScale.Initialize("DistanceFromScale", context);
				context.ExprHostBuilder.CustomLabelDistanceFromScale(m_distanceFromScale);
			}
			if (m_fontAngle != null)
			{
				m_fontAngle.Initialize("FontAngle", context);
				context.ExprHostBuilder.CustomLabelFontAngle(m_fontAngle);
			}
			if (m_placement != null)
			{
				m_placement.Initialize("Placement", context);
				context.ExprHostBuilder.CustomLabelPlacement(m_placement);
			}
			if (m_rotateLabel != null)
			{
				m_rotateLabel.Initialize("RotateLabel", context);
				context.ExprHostBuilder.CustomLabelRotateLabel(m_rotateLabel);
			}
			if (m_tickMarkStyle != null)
			{
				m_tickMarkStyle.Initialize(context);
			}
			if (m_value != null)
			{
				m_value.Initialize("Value", context);
				context.ExprHostBuilder.CustomLabelValue(m_value);
			}
			if (m_hidden != null)
			{
				m_hidden.Initialize("Hidden", context);
				context.ExprHostBuilder.CustomLabelHidden(m_hidden);
			}
			if (m_useFontPercent != null)
			{
				m_useFontPercent.Initialize("UseFontPercent", context);
				context.ExprHostBuilder.CustomLabelUseFontPercent(m_useFontPercent);
			}
			m_exprHostID = context.ExprHostBuilder.CustomLabelEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			CustomLabel customLabel = (CustomLabel)base.PublishClone(context);
			if (m_text != null)
			{
				customLabel.m_text = (ExpressionInfo)m_text.PublishClone(context);
			}
			if (m_allowUpsideDown != null)
			{
				customLabel.m_allowUpsideDown = (ExpressionInfo)m_allowUpsideDown.PublishClone(context);
			}
			if (m_distanceFromScale != null)
			{
				customLabel.m_distanceFromScale = (ExpressionInfo)m_distanceFromScale.PublishClone(context);
			}
			if (m_fontAngle != null)
			{
				customLabel.m_fontAngle = (ExpressionInfo)m_fontAngle.PublishClone(context);
			}
			if (m_placement != null)
			{
				customLabel.m_placement = (ExpressionInfo)m_placement.PublishClone(context);
			}
			if (m_rotateLabel != null)
			{
				customLabel.m_rotateLabel = (ExpressionInfo)m_rotateLabel.PublishClone(context);
			}
			if (m_tickMarkStyle != null)
			{
				customLabel.m_tickMarkStyle = (TickMarkStyle)m_tickMarkStyle.PublishClone(context);
			}
			if (m_value != null)
			{
				customLabel.m_value = (ExpressionInfo)m_value.PublishClone(context);
			}
			if (m_hidden != null)
			{
				customLabel.m_hidden = (ExpressionInfo)m_hidden.PublishClone(context);
			}
			if (m_useFontPercent != null)
			{
				customLabel.m_useFontPercent = (ExpressionInfo)m_useFontPercent.PublishClone(context);
			}
			return customLabel;
		}

		internal void SetExprHost(CustomLabelExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			base.SetExprHost(exprHost, reportObjectModel);
			m_exprHost = exprHost;
			if (m_tickMarkStyle != null && m_exprHost.TickMarkStyleHost != null)
			{
				m_tickMarkStyle.SetExprHost(m_exprHost.TickMarkStyleHost, reportObjectModel);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Name, Token.String));
			list.Add(new MemberInfo(MemberName.Text, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.AllowUpsideDown, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.DistanceFromScale, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.FontAngle, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Placement, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.RotateLabel, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.TickMarkStyle, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TickMarkStyle));
			list.Add(new MemberInfo(MemberName.Value, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Hidden, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.UseFontPercent, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.CustomLabel, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePanelStyleContainer, list);
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
				case MemberName.Text:
					writer.Write(m_text);
					break;
				case MemberName.AllowUpsideDown:
					writer.Write(m_allowUpsideDown);
					break;
				case MemberName.DistanceFromScale:
					writer.Write(m_distanceFromScale);
					break;
				case MemberName.FontAngle:
					writer.Write(m_fontAngle);
					break;
				case MemberName.Placement:
					writer.Write(m_placement);
					break;
				case MemberName.RotateLabel:
					writer.Write(m_rotateLabel);
					break;
				case MemberName.TickMarkStyle:
					writer.Write(m_tickMarkStyle);
					break;
				case MemberName.Value:
					writer.Write(m_value);
					break;
				case MemberName.Hidden:
					writer.Write(m_hidden);
					break;
				case MemberName.UseFontPercent:
					writer.Write(m_useFontPercent);
					break;
				case MemberName.ExprHostID:
					writer.Write(m_exprHostID);
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
				case MemberName.Text:
					m_text = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.AllowUpsideDown:
					m_allowUpsideDown = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.DistanceFromScale:
					m_distanceFromScale = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.FontAngle:
					m_fontAngle = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Placement:
					m_placement = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.RotateLabel:
					m_rotateLabel = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.TickMarkStyle:
					m_tickMarkStyle = (TickMarkStyle)reader.ReadRIFObject();
					break;
				case MemberName.Value:
					m_value = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Hidden:
					m_hidden = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.UseFontPercent:
					m_useFontPercent = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ExprHostID:
					m_exprHostID = reader.ReadInt32();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.CustomLabel;
		}

		internal string EvaluateText(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			Microsoft.ReportingServices.RdlExpressions.VariantResult variantResult = context.ReportRuntime.EvaluateCustomLabelTextExpression(this, m_gaugePanel.Name);
			string result = null;
			if (variantResult.ErrorOccurred)
			{
				result = RPRes.rsExpressionErrorValue;
			}
			else if (variantResult.Value != null)
			{
				result = Formatter.Format(variantResult.Value, ref m_formatter, m_gaugePanel.StyleClass, m_styleClass, context, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, m_gaugePanel.Name);
			}
			return result;
		}

		internal bool EvaluateAllowUpsideDown(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateCustomLabelAllowUpsideDownExpression(this, m_gaugePanel.Name);
		}

		internal double EvaluateDistanceFromScale(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateCustomLabelDistanceFromScaleExpression(this, m_gaugePanel.Name);
		}

		internal double EvaluateFontAngle(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateCustomLabelFontAngleExpression(this, m_gaugePanel.Name);
		}

		internal GaugeLabelPlacements EvaluatePlacement(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return EnumTranslator.TranslateGaugeLabelPlacements(context.ReportRuntime.EvaluateCustomLabelPlacementExpression(this, m_gaugePanel.Name), context.ReportRuntime);
		}

		internal bool EvaluateRotateLabel(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateCustomLabelRotateLabelExpression(this, m_gaugePanel.Name);
		}

		internal double EvaluateValue(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateCustomLabelValueExpression(this, m_gaugePanel.Name);
		}

		internal bool EvaluateHidden(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateCustomLabelHiddenExpression(this, m_gaugePanel.Name);
		}

		internal bool EvaluateUseFontPercent(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateCustomLabelUseFontPercentExpression(this, m_gaugePanel.Name);
		}
	}
}
