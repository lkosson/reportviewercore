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
	internal class TickMarkStyle : GaugePanelStyleContainer, IPersistable
	{
		[NonSerialized]
		protected TickMarkStyleExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private ExpressionInfo m_distanceFromScale;

		private ExpressionInfo m_placement;

		private ExpressionInfo m_enableGradient;

		private ExpressionInfo m_gradientDensity;

		private TopImage m_tickMarkImage;

		private ExpressionInfo m_length;

		private ExpressionInfo m_width;

		private ExpressionInfo m_shape;

		private ExpressionInfo m_hidden;

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

		internal ExpressionInfo EnableGradient
		{
			get
			{
				return m_enableGradient;
			}
			set
			{
				m_enableGradient = value;
			}
		}

		internal ExpressionInfo GradientDensity
		{
			get
			{
				return m_gradientDensity;
			}
			set
			{
				m_gradientDensity = value;
			}
		}

		internal TopImage TickMarkImage
		{
			get
			{
				return m_tickMarkImage;
			}
			set
			{
				m_tickMarkImage = value;
			}
		}

		internal ExpressionInfo Length
		{
			get
			{
				return m_length;
			}
			set
			{
				m_length = value;
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

		internal ExpressionInfo Shape
		{
			get
			{
				return m_shape;
			}
			set
			{
				m_shape = value;
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

		internal TickMarkStyleExprHost ExprHost => m_exprHost;

		internal TickMarkStyle()
		{
		}

		internal TickMarkStyle(GaugePanel gaugePanel)
			: base(gaugePanel)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.TickMarkStyleStart();
			InitializeInternal(context);
			context.ExprHostBuilder.TickMarkStyleEnd();
		}

		internal void InitializeInternal(InitializationContext context)
		{
			base.Initialize(context);
			if (m_distanceFromScale != null)
			{
				m_distanceFromScale.Initialize("DistanceFromScale", context);
				context.ExprHostBuilder.TickMarkStyleDistanceFromScale(m_distanceFromScale);
			}
			if (m_placement != null)
			{
				m_placement.Initialize("Placement", context);
				context.ExprHostBuilder.TickMarkStylePlacement(m_placement);
			}
			if (m_enableGradient != null)
			{
				m_enableGradient.Initialize("EnableGradient", context);
				context.ExprHostBuilder.TickMarkStyleEnableGradient(m_enableGradient);
			}
			if (m_gradientDensity != null)
			{
				m_gradientDensity.Initialize("GradientDensity", context);
				context.ExprHostBuilder.TickMarkStyleGradientDensity(m_gradientDensity);
			}
			if (m_tickMarkImage != null)
			{
				m_tickMarkImage.Initialize(context);
			}
			if (m_length != null)
			{
				m_length.Initialize("Length", context);
				context.ExprHostBuilder.TickMarkStyleLength(m_length);
			}
			if (m_width != null)
			{
				m_width.Initialize("Width", context);
				context.ExprHostBuilder.TickMarkStyleWidth(m_width);
			}
			if (m_shape != null)
			{
				m_shape.Initialize("Shape", context);
				context.ExprHostBuilder.TickMarkStyleShape(m_shape);
			}
			if (m_hidden != null)
			{
				m_hidden.Initialize("Hidden", context);
				context.ExprHostBuilder.TickMarkStyleHidden(m_hidden);
			}
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			TickMarkStyle tickMarkStyle = (TickMarkStyle)base.PublishClone(context);
			if (m_distanceFromScale != null)
			{
				tickMarkStyle.m_distanceFromScale = (ExpressionInfo)m_distanceFromScale.PublishClone(context);
			}
			if (m_placement != null)
			{
				tickMarkStyle.m_placement = (ExpressionInfo)m_placement.PublishClone(context);
			}
			if (m_enableGradient != null)
			{
				tickMarkStyle.m_enableGradient = (ExpressionInfo)m_enableGradient.PublishClone(context);
			}
			if (m_gradientDensity != null)
			{
				tickMarkStyle.m_gradientDensity = (ExpressionInfo)m_gradientDensity.PublishClone(context);
			}
			if (m_tickMarkImage != null)
			{
				tickMarkStyle.m_tickMarkImage = (TopImage)m_tickMarkImage.PublishClone(context);
			}
			if (m_length != null)
			{
				tickMarkStyle.m_length = (ExpressionInfo)m_length.PublishClone(context);
			}
			if (m_width != null)
			{
				tickMarkStyle.m_width = (ExpressionInfo)m_width.PublishClone(context);
			}
			if (m_shape != null)
			{
				tickMarkStyle.m_shape = (ExpressionInfo)m_shape.PublishClone(context);
			}
			if (m_hidden != null)
			{
				tickMarkStyle.m_hidden = (ExpressionInfo)m_hidden.PublishClone(context);
			}
			return tickMarkStyle;
		}

		internal void SetExprHost(TickMarkStyleExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			base.SetExprHost(exprHost, reportObjectModel);
			m_exprHost = exprHost;
			if (m_tickMarkImage != null && m_exprHost.TickMarkImageHost != null)
			{
				m_tickMarkImage.SetExprHost(m_exprHost.TickMarkImageHost, reportObjectModel);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.DistanceFromScale, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Placement, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.EnableGradient, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.GradientDensity, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.TickMarkImage, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TopImage));
			list.Add(new MemberInfo(MemberName.Length, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Width, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Shape, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Hidden, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TickMarkStyle, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePanelStyleContainer, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.DistanceFromScale:
					writer.Write(m_distanceFromScale);
					break;
				case MemberName.Placement:
					writer.Write(m_placement);
					break;
				case MemberName.EnableGradient:
					writer.Write(m_enableGradient);
					break;
				case MemberName.GradientDensity:
					writer.Write(m_gradientDensity);
					break;
				case MemberName.TickMarkImage:
					writer.Write(m_tickMarkImage);
					break;
				case MemberName.Length:
					writer.Write(m_length);
					break;
				case MemberName.Width:
					writer.Write(m_width);
					break;
				case MemberName.Shape:
					writer.Write(m_shape);
					break;
				case MemberName.Hidden:
					writer.Write(m_hidden);
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
				case MemberName.DistanceFromScale:
					m_distanceFromScale = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Placement:
					m_placement = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.EnableGradient:
					m_enableGradient = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.GradientDensity:
					m_gradientDensity = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.TickMarkImage:
					m_tickMarkImage = (TopImage)reader.ReadRIFObject();
					break;
				case MemberName.Length:
					m_length = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Width:
					m_width = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Shape:
					m_shape = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Hidden:
					m_hidden = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TickMarkStyle;
		}

		internal double EvaluateDistanceFromScale(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateTickMarkStyleDistanceFromScaleExpression(this, m_gaugePanel.Name);
		}

		internal GaugeLabelPlacements EvaluatePlacement(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return EnumTranslator.TranslateGaugeLabelPlacements(context.ReportRuntime.EvaluateTickMarkStylePlacementExpression(this, m_gaugePanel.Name), context.ReportRuntime);
		}

		internal bool EvaluateEnableGradient(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateTickMarkStyleEnableGradientExpression(this, m_gaugePanel.Name);
		}

		internal double EvaluateGradientDensity(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateTickMarkStyleGradientDensityExpression(this, m_gaugePanel.Name);
		}

		internal double EvaluateLength(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateTickMarkStyleLengthExpression(this, m_gaugePanel.Name);
		}

		internal double EvaluateWidth(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateTickMarkStyleWidthExpression(this, m_gaugePanel.Name);
		}

		internal GaugeTickMarkShapes EvaluateShape(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return EnumTranslator.TranslateGaugeTickMarkShapes(context.ReportRuntime.EvaluateTickMarkStyleShapeExpression(this, m_gaugePanel.Name), context.ReportRuntime);
		}

		internal bool EvaluateHidden(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateTickMarkStyleHiddenExpression(this, m_gaugePanel.Name);
		}
	}
}
