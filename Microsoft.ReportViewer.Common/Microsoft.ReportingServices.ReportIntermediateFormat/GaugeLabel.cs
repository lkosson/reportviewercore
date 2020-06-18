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
	internal sealed class GaugeLabel : GaugePanelItem, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		[NonSerialized]
		private Formatter m_formatter;

		private ExpressionInfo m_text;

		private ExpressionInfo m_angle;

		private ExpressionInfo m_resizeMode;

		private ExpressionInfo m_textShadowOffset;

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

		internal ExpressionInfo Angle
		{
			get
			{
				return m_angle;
			}
			set
			{
				m_angle = value;
			}
		}

		internal ExpressionInfo ResizeMode
		{
			get
			{
				return m_resizeMode;
			}
			set
			{
				m_resizeMode = value;
			}
		}

		internal ExpressionInfo TextShadowOffset
		{
			get
			{
				return m_textShadowOffset;
			}
			set
			{
				m_textShadowOffset = value;
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

		internal GaugeLabel()
		{
		}

		internal GaugeLabel(GaugePanel gaugePanel, int id)
			: base(gaugePanel, id)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.GaugeLabelStart(m_name);
			base.Initialize(context);
			if (m_text != null)
			{
				m_text.Initialize("Text", context);
				context.ExprHostBuilder.GaugeLabelText(m_text);
			}
			if (m_angle != null)
			{
				m_angle.Initialize("Angle", context);
				context.ExprHostBuilder.GaugeLabelAngle(m_angle);
			}
			if (m_resizeMode != null)
			{
				m_resizeMode.Initialize("ResizeMode", context);
				context.ExprHostBuilder.GaugeLabelResizeMode(m_resizeMode);
			}
			if (m_textShadowOffset != null)
			{
				m_textShadowOffset.Initialize("TextShadowOffset", context);
				context.ExprHostBuilder.GaugeLabelTextShadowOffset(m_textShadowOffset);
			}
			if (m_useFontPercent != null)
			{
				m_useFontPercent.Initialize("UseFontPercent", context);
				context.ExprHostBuilder.GaugeLabelUseFontPercent(m_useFontPercent);
			}
			m_exprHostID = context.ExprHostBuilder.GaugeLabelEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			GaugeLabel gaugeLabel = (GaugeLabel)base.PublishClone(context);
			if (m_text != null)
			{
				gaugeLabel.m_text = (ExpressionInfo)m_text.PublishClone(context);
			}
			if (m_angle != null)
			{
				gaugeLabel.m_angle = (ExpressionInfo)m_angle.PublishClone(context);
			}
			if (m_resizeMode != null)
			{
				gaugeLabel.m_resizeMode = (ExpressionInfo)m_resizeMode.PublishClone(context);
			}
			if (m_textShadowOffset != null)
			{
				gaugeLabel.m_textShadowOffset = (ExpressionInfo)m_textShadowOffset.PublishClone(context);
			}
			if (m_useFontPercent != null)
			{
				gaugeLabel.m_useFontPercent = (ExpressionInfo)m_useFontPercent.PublishClone(context);
			}
			return gaugeLabel;
		}

		internal void SetExprHost(GaugeLabelExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			SetExprHost((GaugePanelItemExprHost)exprHost, reportObjectModel);
			m_exprHost = exprHost;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Text, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Angle, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ResizeMode, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.TextShadowOffset, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.UseFontPercent, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeLabel, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePanelItem, list);
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
				case MemberName.Angle:
					writer.Write(m_angle);
					break;
				case MemberName.ResizeMode:
					writer.Write(m_resizeMode);
					break;
				case MemberName.TextShadowOffset:
					writer.Write(m_textShadowOffset);
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
				case MemberName.Angle:
					m_angle = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ResizeMode:
					m_resizeMode = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.TextShadowOffset:
					m_textShadowOffset = (ExpressionInfo)reader.ReadRIFObject();
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
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeLabel;
		}

		internal Microsoft.ReportingServices.RdlExpressions.VariantResult EvaluateText(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugeLabelTextExpression(this, m_gaugePanel.Name);
		}

		internal string FormatText(Microsoft.ReportingServices.RdlExpressions.VariantResult result, OnDemandProcessingContext context)
		{
			string result2 = null;
			if (result.ErrorOccurred)
			{
				result2 = RPRes.rsExpressionErrorValue;
			}
			else if (result.Value != null)
			{
				result2 = Formatter.Format(result.Value, ref m_formatter, m_gaugePanel.StyleClass, m_styleClass, context, Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel, m_gaugePanel.Name);
			}
			return result2;
		}

		internal double EvaluateAngle(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugeLabelAngleExpression(this, m_gaugePanel.Name);
		}

		internal GaugeResizeModes EvaluateResizeMode(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return EnumTranslator.TranslateGaugeResizeModes(context.ReportRuntime.EvaluateGaugeLabelResizeModeExpression(this, m_gaugePanel.Name), context.ReportRuntime);
		}

		internal string EvaluateTextShadowOffset(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugeLabelTextShadowOffsetExpression(this, m_gaugePanel.Name);
		}

		internal bool EvaluateUseFontPercent(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugeLabelUseFontPercentExpression(this, m_gaugePanel.Name);
		}
	}
}
