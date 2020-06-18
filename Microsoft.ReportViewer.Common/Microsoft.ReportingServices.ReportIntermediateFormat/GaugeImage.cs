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
	internal sealed class GaugeImage : GaugePanelItem, IPersistable
	{
		private ExpressionInfo m_source;

		private ExpressionInfo m_value;

		private ExpressionInfo m_transparentColor;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal ExpressionInfo Source
		{
			get
			{
				return m_source;
			}
			set
			{
				m_source = value;
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

		internal ExpressionInfo TransparentColor
		{
			get
			{
				return m_transparentColor;
			}
			set
			{
				m_transparentColor = value;
			}
		}

		internal GaugeImage()
		{
		}

		internal GaugeImage(GaugePanel gaugePanel, int id)
			: base(gaugePanel, id)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.GaugeImageStart(m_name);
			base.Initialize(context);
			if (m_source != null)
			{
				m_source.Initialize("Source", context);
				context.ExprHostBuilder.BaseGaugeImageSource(m_source);
			}
			if (m_value != null)
			{
				m_value.Initialize("Value", context);
				context.ExprHostBuilder.BaseGaugeImageValue(m_value);
			}
			if (m_transparentColor != null)
			{
				m_transparentColor.Initialize("TransparentColor", context);
				context.ExprHostBuilder.BaseGaugeImageTransparentColor(m_transparentColor);
			}
			m_exprHostID = context.ExprHostBuilder.GaugeImageEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			GaugeImage gaugeImage = (GaugeImage)base.PublishClone(context);
			if (m_source != null)
			{
				gaugeImage.m_source = (ExpressionInfo)m_source.PublishClone(context);
			}
			if (m_value != null)
			{
				gaugeImage.m_value = (ExpressionInfo)m_value.PublishClone(context);
			}
			if (m_transparentColor != null)
			{
				gaugeImage.m_transparentColor = (ExpressionInfo)m_transparentColor.PublishClone(context);
			}
			return gaugeImage;
		}

		internal void SetExprHost(GaugeImageExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			SetExprHost((GaugePanelItemExprHost)exprHost, reportObjectModel);
			m_exprHost = exprHost;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Source, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Value, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.TransparentColor, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeImage, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePanelItem, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Source:
					writer.Write(m_source);
					break;
				case MemberName.Value:
					writer.Write(m_value);
					break;
				case MemberName.TransparentColor:
					writer.Write(m_transparentColor);
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
				case MemberName.Source:
					m_source = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Value:
					m_value = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.TransparentColor:
					m_transparentColor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeImage;
		}

		internal Microsoft.ReportingServices.OnDemandReportRendering.Image.SourceType EvaluateSource(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return EnumTranslator.TranslateImageSourceType(context.ReportRuntime.EvaluateGaugeImageSourceExpression(this, m_gaugePanel.Name), context.ReportRuntime);
		}

		internal Microsoft.ReportingServices.RdlExpressions.VariantResult EvaluateValue(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugeImageValueExpression(this, m_gaugePanel.Name);
		}

		internal string EvaluateTransparentColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugeImageTransparentColorExpression(this, m_gaugePanel.Name);
		}
	}
}
