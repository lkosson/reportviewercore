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
	internal sealed class PointerCap : GaugePanelStyleContainer, IPersistable
	{
		[NonSerialized]
		private PointerCapExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private CapImage m_capImage;

		private ExpressionInfo m_onTop;

		private ExpressionInfo m_reflection;

		private ExpressionInfo m_capStyle;

		private ExpressionInfo m_hidden;

		private ExpressionInfo m_width;

		internal CapImage CapImage
		{
			get
			{
				return m_capImage;
			}
			set
			{
				m_capImage = value;
			}
		}

		internal ExpressionInfo OnTop
		{
			get
			{
				return m_onTop;
			}
			set
			{
				m_onTop = value;
			}
		}

		internal ExpressionInfo Reflection
		{
			get
			{
				return m_reflection;
			}
			set
			{
				m_reflection = value;
			}
		}

		internal ExpressionInfo CapStyle
		{
			get
			{
				return m_capStyle;
			}
			set
			{
				m_capStyle = value;
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

		internal PointerCapExprHost ExprHost => m_exprHost;

		internal PointerCap()
		{
		}

		internal PointerCap(GaugePanel gaugePanel)
			: base(gaugePanel)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.PointerCapStart();
			base.Initialize(context);
			if (m_capImage != null)
			{
				m_capImage.Initialize(context);
			}
			if (m_onTop != null)
			{
				m_onTop.Initialize("OnTop", context);
				context.ExprHostBuilder.PointerCapOnTop(m_onTop);
			}
			if (m_reflection != null)
			{
				m_reflection.Initialize("Reflection", context);
				context.ExprHostBuilder.PointerCapReflection(m_reflection);
			}
			if (m_capStyle != null)
			{
				m_capStyle.Initialize("CapStyle", context);
				context.ExprHostBuilder.PointerCapCapStyle(m_capStyle);
			}
			if (m_hidden != null)
			{
				m_hidden.Initialize("Hidden", context);
				context.ExprHostBuilder.PointerCapHidden(m_hidden);
			}
			if (m_width != null)
			{
				m_width.Initialize("Width", context);
				context.ExprHostBuilder.PointerCapWidth(m_width);
			}
			context.ExprHostBuilder.PointerCapEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			PointerCap pointerCap = (PointerCap)base.PublishClone(context);
			if (m_capImage != null)
			{
				pointerCap.m_capImage = (CapImage)m_capImage.PublishClone(context);
			}
			if (m_onTop != null)
			{
				pointerCap.m_onTop = (ExpressionInfo)m_onTop.PublishClone(context);
			}
			if (m_reflection != null)
			{
				pointerCap.m_reflection = (ExpressionInfo)m_reflection.PublishClone(context);
			}
			if (m_capStyle != null)
			{
				pointerCap.m_capStyle = (ExpressionInfo)m_capStyle.PublishClone(context);
			}
			if (m_hidden != null)
			{
				pointerCap.m_hidden = (ExpressionInfo)m_hidden.PublishClone(context);
			}
			if (m_width != null)
			{
				pointerCap.m_width = (ExpressionInfo)m_width.PublishClone(context);
			}
			return pointerCap;
		}

		internal void SetExprHost(PointerCapExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			base.SetExprHost(exprHost, reportObjectModel);
			m_exprHost = exprHost;
			if (m_capImage != null && m_exprHost.CapImageHost != null)
			{
				m_capImage.SetExprHost(m_exprHost.CapImageHost, reportObjectModel);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.CapImage, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.CapImage));
			list.Add(new MemberInfo(MemberName.OnTop, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Reflection, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.CapStyle, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Hidden, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Width, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PointerCap, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePanelStyleContainer, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.CapImage:
					writer.Write(m_capImage);
					break;
				case MemberName.OnTop:
					writer.Write(m_onTop);
					break;
				case MemberName.Reflection:
					writer.Write(m_reflection);
					break;
				case MemberName.CapStyle:
					writer.Write(m_capStyle);
					break;
				case MemberName.Hidden:
					writer.Write(m_hidden);
					break;
				case MemberName.Width:
					writer.Write(m_width);
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
				case MemberName.CapImage:
					m_capImage = (CapImage)reader.ReadRIFObject();
					break;
				case MemberName.OnTop:
					m_onTop = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Reflection:
					m_reflection = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.CapStyle:
					m_capStyle = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Hidden:
					m_hidden = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Width:
					m_width = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PointerCap;
		}

		internal bool EvaluateOnTop(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluatePointerCapOnTopExpression(this, m_gaugePanel.Name);
		}

		internal bool EvaluateReflection(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluatePointerCapReflectionExpression(this, m_gaugePanel.Name);
		}

		internal GaugeCapStyles EvaluateCapStyle(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return EnumTranslator.TranslateGaugeCapStyles(context.ReportRuntime.EvaluatePointerCapCapStyleExpression(this, m_gaugePanel.Name), context.ReportRuntime);
		}

		internal bool EvaluateHidden(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluatePointerCapHiddenExpression(this, m_gaugePanel.Name);
		}

		internal double EvaluateWidth(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluatePointerCapWidthExpression(this, m_gaugePanel.Name);
		}
	}
}
