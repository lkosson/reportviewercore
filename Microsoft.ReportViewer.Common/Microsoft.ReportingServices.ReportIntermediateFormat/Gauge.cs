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
	internal class Gauge : GaugePanelItem, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private BackFrame m_backFrame;

		private ExpressionInfo m_clipContent;

		private TopImage m_topImage;

		private ExpressionInfo m_aspectRatio;

		internal BackFrame BackFrame
		{
			get
			{
				return m_backFrame;
			}
			set
			{
				m_backFrame = value;
			}
		}

		internal ExpressionInfo ClipContent
		{
			get
			{
				return m_clipContent;
			}
			set
			{
				m_clipContent = value;
			}
		}

		internal TopImage TopImage
		{
			get
			{
				return m_topImage;
			}
			set
			{
				m_topImage = value;
			}
		}

		internal ExpressionInfo AspectRatio
		{
			get
			{
				return m_aspectRatio;
			}
			set
			{
				m_aspectRatio = value;
			}
		}

		internal Gauge()
		{
		}

		internal Gauge(GaugePanel gaugePanel, int id)
			: base(gaugePanel, id)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			base.Initialize(context);
			if (m_backFrame != null)
			{
				m_backFrame.Initialize(context);
			}
			if (m_clipContent != null)
			{
				m_clipContent.Initialize("ClipContent", context);
				context.ExprHostBuilder.GaugeClipContent(m_clipContent);
			}
			if (m_topImage != null)
			{
				m_topImage.Initialize(context);
			}
			if (m_aspectRatio != null)
			{
				m_aspectRatio.Initialize("AspectRatio", context);
				context.ExprHostBuilder.GaugeAspectRatio(m_aspectRatio);
			}
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			Gauge gauge = (Gauge)base.PublishClone(context);
			if (m_backFrame != null)
			{
				gauge.m_backFrame = (BackFrame)m_backFrame.PublishClone(context);
			}
			if (m_clipContent != null)
			{
				gauge.m_clipContent = (ExpressionInfo)m_clipContent.PublishClone(context);
			}
			if (m_topImage != null)
			{
				gauge.m_topImage = (TopImage)m_topImage.PublishClone(context);
			}
			if (m_aspectRatio != null)
			{
				gauge.m_aspectRatio = (ExpressionInfo)m_aspectRatio.PublishClone(context);
			}
			return gauge;
		}

		internal void SetExprHost(GaugeExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			SetExprHost((GaugePanelItemExprHost)exprHost, reportObjectModel);
			m_exprHost = exprHost;
			if (m_backFrame != null && ((GaugeExprHost)m_exprHost).BackFrameHost != null)
			{
				m_backFrame.SetExprHost(((GaugeExprHost)m_exprHost).BackFrameHost, reportObjectModel);
			}
			if (m_topImage != null && ((GaugeExprHost)m_exprHost).TopImageHost != null)
			{
				m_topImage.SetExprHost(((GaugeExprHost)m_exprHost).TopImageHost, reportObjectModel);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.BackFrame, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BackFrame));
			list.Add(new MemberInfo(MemberName.ClipContent, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.TopImage, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TopImage));
			list.Add(new MemberInfo(MemberName.AspectRatio, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Gauge, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePanelItem, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.BackFrame:
					writer.Write(m_backFrame);
					break;
				case MemberName.ClipContent:
					writer.Write(m_clipContent);
					break;
				case MemberName.TopImage:
					writer.Write(m_topImage);
					break;
				case MemberName.AspectRatio:
					writer.Write(m_aspectRatio);
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
				case MemberName.BackFrame:
					m_backFrame = (BackFrame)reader.ReadRIFObject();
					break;
				case MemberName.ClipContent:
					m_clipContent = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.TopImage:
					m_topImage = (TopImage)reader.ReadRIFObject();
					break;
				case MemberName.AspectRatio:
					m_aspectRatio = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Gauge;
		}

		internal bool EvaluateClipContent(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugeClipContentExpression(this, m_gaugePanel.Name);
		}

		internal double EvaluateAspectRatio(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugeAspectRatioExpression(this, m_gaugePanel.Name);
		}
	}
}
