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
	internal sealed class PinLabel : GaugePanelStyleContainer, IPersistable
	{
		[NonSerialized]
		private PinLabelExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private ExpressionInfo m_text;

		private ExpressionInfo m_allowUpsideDown;

		private ExpressionInfo m_distanceFromScale;

		private ExpressionInfo m_fontAngle;

		private ExpressionInfo m_placement;

		private ExpressionInfo m_rotateLabel;

		private ExpressionInfo m_useFontPercent;

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

		internal PinLabelExprHost ExprHost => m_exprHost;

		internal PinLabel()
		{
		}

		internal PinLabel(GaugePanel gaugePanel)
			: base(gaugePanel)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.PinLabelStart();
			base.Initialize(context);
			if (m_text != null)
			{
				m_text.Initialize("Text", context);
				context.ExprHostBuilder.PinLabelText(m_text);
			}
			if (m_allowUpsideDown != null)
			{
				m_allowUpsideDown.Initialize("AllowUpsideDown", context);
				context.ExprHostBuilder.PinLabelAllowUpsideDown(m_allowUpsideDown);
			}
			if (m_distanceFromScale != null)
			{
				m_distanceFromScale.Initialize("DistanceFromScale", context);
				context.ExprHostBuilder.PinLabelDistanceFromScale(m_distanceFromScale);
			}
			if (m_fontAngle != null)
			{
				m_fontAngle.Initialize("FontAngle", context);
				context.ExprHostBuilder.PinLabelFontAngle(m_fontAngle);
			}
			if (m_placement != null)
			{
				m_placement.Initialize("Placement", context);
				context.ExprHostBuilder.PinLabelPlacement(m_placement);
			}
			if (m_rotateLabel != null)
			{
				m_rotateLabel.Initialize("RotateLabel", context);
				context.ExprHostBuilder.PinLabelRotateLabel(m_rotateLabel);
			}
			if (m_useFontPercent != null)
			{
				m_useFontPercent.Initialize("UseFontPercent", context);
				context.ExprHostBuilder.PinLabelUseFontPercent(m_useFontPercent);
			}
			context.ExprHostBuilder.PinLabelEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			PinLabel pinLabel = (PinLabel)base.PublishClone(context);
			if (m_text != null)
			{
				pinLabel.m_text = (ExpressionInfo)m_text.PublishClone(context);
			}
			if (m_allowUpsideDown != null)
			{
				pinLabel.m_allowUpsideDown = (ExpressionInfo)m_allowUpsideDown.PublishClone(context);
			}
			if (m_distanceFromScale != null)
			{
				pinLabel.m_distanceFromScale = (ExpressionInfo)m_distanceFromScale.PublishClone(context);
			}
			if (m_fontAngle != null)
			{
				pinLabel.m_fontAngle = (ExpressionInfo)m_fontAngle.PublishClone(context);
			}
			if (m_placement != null)
			{
				pinLabel.m_placement = (ExpressionInfo)m_placement.PublishClone(context);
			}
			if (m_rotateLabel != null)
			{
				pinLabel.m_rotateLabel = (ExpressionInfo)m_rotateLabel.PublishClone(context);
			}
			if (m_useFontPercent != null)
			{
				pinLabel.m_useFontPercent = (ExpressionInfo)m_useFontPercent.PublishClone(context);
			}
			return pinLabel;
		}

		internal void SetExprHost(PinLabelExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			base.SetExprHost(exprHost, reportObjectModel);
			m_exprHost = exprHost;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Text, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.AllowUpsideDown, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.DistanceFromScale, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.FontAngle, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Placement, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.RotateLabel, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.UseFontPercent, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PinLabel, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePanelStyleContainer, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
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
				case MemberName.UseFontPercent:
					writer.Write(m_useFontPercent);
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
				case MemberName.UseFontPercent:
					m_useFontPercent = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PinLabel;
		}

		internal string EvaluateText(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluatePinLabelTextExpression(this, m_gaugePanel.Name);
		}

		internal bool EvaluateAllowUpsideDown(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluatePinLabelAllowUpsideDownExpression(this, m_gaugePanel.Name);
		}

		internal double EvaluateDistanceFromScale(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluatePinLabelDistanceFromScaleExpression(this, m_gaugePanel.Name);
		}

		internal double EvaluateFontAngle(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluatePinLabelFontAngleExpression(this, m_gaugePanel.Name);
		}

		internal GaugeLabelPlacements EvaluatePlacement(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return EnumTranslator.TranslateGaugeLabelPlacements(context.ReportRuntime.EvaluatePinLabelPlacementExpression(this, m_gaugePanel.Name), context.ReportRuntime);
		}

		internal bool EvaluateRotateLabel(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluatePinLabelRotateLabelExpression(this, m_gaugePanel.Name);
		}

		internal bool EvaluateUseFontPercent(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluatePinLabelUseFontPercentExpression(this, m_gaugePanel.Name);
		}
	}
}
