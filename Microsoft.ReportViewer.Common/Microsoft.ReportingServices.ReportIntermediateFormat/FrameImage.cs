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
	internal sealed class FrameImage : BaseGaugeImage, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private ExpressionInfo m_hueColor;

		private ExpressionInfo m_transparency;

		private ExpressionInfo m_clipImage;

		internal ExpressionInfo HueColor
		{
			get
			{
				return m_hueColor;
			}
			set
			{
				m_hueColor = value;
			}
		}

		internal ExpressionInfo Transparency
		{
			get
			{
				return m_transparency;
			}
			set
			{
				m_transparency = value;
			}
		}

		internal ExpressionInfo ClipImage
		{
			get
			{
				return m_clipImage;
			}
			set
			{
				m_clipImage = value;
			}
		}

		internal FrameImage()
		{
		}

		internal FrameImage(GaugePanel gaugePanel)
			: base(gaugePanel)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.FrameImageStart();
			base.Initialize(context);
			if (m_hueColor != null)
			{
				m_hueColor.Initialize("HueColor", context);
				context.ExprHostBuilder.FrameImageHueColor(m_hueColor);
			}
			if (m_transparency != null)
			{
				m_transparency.Initialize("Transparency", context);
				context.ExprHostBuilder.FrameImageTransparency(m_transparency);
			}
			if (m_clipImage != null)
			{
				m_clipImage.Initialize("ClipImage", context);
				context.ExprHostBuilder.FrameImageClipImage(m_clipImage);
			}
			context.ExprHostBuilder.FrameImageEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			FrameImage frameImage = (FrameImage)base.PublishClone(context);
			if (m_hueColor != null)
			{
				frameImage.m_hueColor = (ExpressionInfo)m_hueColor.PublishClone(context);
			}
			if (m_transparency != null)
			{
				frameImage.m_transparency = (ExpressionInfo)m_transparency.PublishClone(context);
			}
			if (m_clipImage != null)
			{
				frameImage.m_clipImage = (ExpressionInfo)m_clipImage.PublishClone(context);
			}
			return frameImage;
		}

		internal void SetExprHost(FrameImageExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			SetExprHost((BaseGaugeImageExprHost)exprHost, reportObjectModel);
			m_exprHost = exprHost;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.HueColor, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Transparency, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ClipImage, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.FrameImage, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BaseGaugeImage, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.HueColor:
					writer.Write(m_hueColor);
					break;
				case MemberName.Transparency:
					writer.Write(m_transparency);
					break;
				case MemberName.ClipImage:
					writer.Write(m_clipImage);
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
				case MemberName.HueColor:
					m_hueColor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Transparency:
					m_transparency = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ClipImage:
					m_clipImage = (ExpressionInfo)reader.ReadRIFObject();
					break;
				}
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.FrameImage;
		}

		internal string EvaluateHueColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateFrameImageHueColorExpression(this, m_gaugePanel.Name);
		}

		internal double EvaluateTransparency(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateFrameImageTransparencyExpression(this, m_gaugePanel.Name);
		}

		internal bool EvaluateClipImage(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateFrameImageClipImageExpression(this, m_gaugePanel.Name);
		}
	}
}
