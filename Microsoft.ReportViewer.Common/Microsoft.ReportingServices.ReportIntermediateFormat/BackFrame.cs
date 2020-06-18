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
	internal sealed class BackFrame : GaugePanelStyleContainer, IPersistable
	{
		[NonSerialized]
		private BackFrameExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private ExpressionInfo m_frameStyle;

		private ExpressionInfo m_frameShape;

		private ExpressionInfo m_frameWidth;

		private ExpressionInfo m_glassEffect;

		private FrameBackground m_frameBackground;

		private FrameImage m_frameImage;

		internal ExpressionInfo FrameStyle
		{
			get
			{
				return m_frameStyle;
			}
			set
			{
				m_frameStyle = value;
			}
		}

		internal ExpressionInfo FrameShape
		{
			get
			{
				return m_frameShape;
			}
			set
			{
				m_frameShape = value;
			}
		}

		internal ExpressionInfo FrameWidth
		{
			get
			{
				return m_frameWidth;
			}
			set
			{
				m_frameWidth = value;
			}
		}

		internal ExpressionInfo GlassEffect
		{
			get
			{
				return m_glassEffect;
			}
			set
			{
				m_glassEffect = value;
			}
		}

		internal FrameBackground FrameBackground
		{
			get
			{
				return m_frameBackground;
			}
			set
			{
				m_frameBackground = value;
			}
		}

		internal FrameImage FrameImage
		{
			get
			{
				return m_frameImage;
			}
			set
			{
				m_frameImage = value;
			}
		}

		internal string OwnerName => m_gaugePanel.Name;

		internal BackFrameExprHost ExprHost => m_exprHost;

		internal BackFrame()
		{
		}

		internal BackFrame(GaugePanel gaugePanel)
			: base(gaugePanel)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.BackFrameStart();
			base.Initialize(context);
			if (m_frameStyle != null)
			{
				m_frameStyle.Initialize("FrameStyle", context);
				context.ExprHostBuilder.BackFrameFrameStyle(m_frameStyle);
			}
			if (m_frameShape != null)
			{
				m_frameShape.Initialize("FrameShape", context);
				context.ExprHostBuilder.BackFrameFrameShape(m_frameShape);
			}
			if (m_frameWidth != null)
			{
				m_frameWidth.Initialize("FrameWidth", context);
				context.ExprHostBuilder.BackFrameFrameWidth(m_frameWidth);
			}
			if (m_glassEffect != null)
			{
				m_glassEffect.Initialize("GlassEffect", context);
				context.ExprHostBuilder.BackFrameGlassEffect(m_glassEffect);
			}
			if (m_frameBackground != null)
			{
				m_frameBackground.Initialize(context);
			}
			if (m_frameImage != null)
			{
				m_frameImage.Initialize(context);
			}
			context.ExprHostBuilder.BackFrameEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			BackFrame backFrame = (BackFrame)base.PublishClone(context);
			if (m_frameStyle != null)
			{
				backFrame.m_frameStyle = (ExpressionInfo)m_frameStyle.PublishClone(context);
			}
			if (m_frameShape != null)
			{
				backFrame.m_frameShape = (ExpressionInfo)m_frameShape.PublishClone(context);
			}
			if (m_frameWidth != null)
			{
				backFrame.m_frameWidth = (ExpressionInfo)m_frameWidth.PublishClone(context);
			}
			if (m_glassEffect != null)
			{
				backFrame.m_glassEffect = (ExpressionInfo)m_glassEffect.PublishClone(context);
			}
			if (m_frameBackground != null)
			{
				backFrame.m_frameBackground = (FrameBackground)m_frameBackground.PublishClone(context);
			}
			if (m_frameImage != null)
			{
				backFrame.m_frameImage = (FrameImage)m_frameImage.PublishClone(context);
			}
			return backFrame;
		}

		internal void SetExprHost(BackFrameExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			base.SetExprHost(exprHost, reportObjectModel);
			m_exprHost = exprHost;
			if (m_frameBackground != null && m_exprHost.FrameBackgroundHost != null)
			{
				m_frameBackground.SetExprHost(m_exprHost.FrameBackgroundHost, reportObjectModel);
			}
			if (m_frameImage != null && m_exprHost.FrameImageHost != null)
			{
				m_frameImage.SetExprHost(m_exprHost.FrameImageHost, reportObjectModel);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.FrameStyle, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.FrameShape, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.FrameWidth, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.GlassEffect, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.FrameBackground, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.FrameBackground));
			list.Add(new MemberInfo(MemberName.FrameImage, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.FrameImage));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BackFrame, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePanelStyleContainer, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.FrameStyle:
					writer.Write(m_frameStyle);
					break;
				case MemberName.FrameShape:
					writer.Write(m_frameShape);
					break;
				case MemberName.FrameWidth:
					writer.Write(m_frameWidth);
					break;
				case MemberName.GlassEffect:
					writer.Write(m_glassEffect);
					break;
				case MemberName.FrameBackground:
					writer.Write(m_frameBackground);
					break;
				case MemberName.FrameImage:
					writer.Write(m_frameImage);
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
				case MemberName.FrameStyle:
					m_frameStyle = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.FrameShape:
					m_frameShape = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.FrameWidth:
					m_frameWidth = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.GlassEffect:
					m_glassEffect = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.FrameBackground:
					m_frameBackground = (FrameBackground)reader.ReadRIFObject();
					break;
				case MemberName.FrameImage:
					m_frameImage = (FrameImage)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BackFrame;
		}

		internal GaugeFrameStyles EvaluateFrameStyle(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return EnumTranslator.TranslateGaugeFrameStyles(context.ReportRuntime.EvaluateBackFrameFrameStyleExpression(this, m_gaugePanel.Name), context.ReportRuntime);
		}

		internal GaugeFrameShapes EvaluateFrameShape(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return EnumTranslator.TranslateGaugeFrameShapes(context.ReportRuntime.EvaluateBackFrameFrameShapeExpression(this, m_gaugePanel.Name), context.ReportRuntime);
		}

		internal double EvaluateFrameWidth(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateBackFrameFrameWidthExpression(this, m_gaugePanel.Name);
		}

		internal GaugeGlassEffects EvaluateGlassEffect(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return EnumTranslator.TranslateGaugeGlassEffects(context.ReportRuntime.EvaluateBackFrameGlassEffectExpression(this, m_gaugePanel.Name), context.ReportRuntime);
		}
	}
}
