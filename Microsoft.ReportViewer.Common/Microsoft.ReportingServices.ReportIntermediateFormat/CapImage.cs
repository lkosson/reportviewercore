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
	internal sealed class CapImage : BaseGaugeImage, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private ExpressionInfo m_hueColor;

		private ExpressionInfo m_offsetX;

		private ExpressionInfo m_offsetY;

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

		internal ExpressionInfo OffsetX
		{
			get
			{
				return m_offsetX;
			}
			set
			{
				m_offsetX = value;
			}
		}

		internal ExpressionInfo OffsetY
		{
			get
			{
				return m_offsetY;
			}
			set
			{
				m_offsetY = value;
			}
		}

		internal CapImage()
		{
		}

		internal CapImage(GaugePanel gaugePanel)
			: base(gaugePanel)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.CapImageStart();
			base.Initialize(context);
			if (m_hueColor != null)
			{
				m_hueColor.Initialize("HueColor", context);
				context.ExprHostBuilder.CapImageHueColor(m_hueColor);
			}
			if (m_offsetX != null)
			{
				m_offsetX.Initialize("OffsetX", context);
				context.ExprHostBuilder.CapImageOffsetX(m_offsetX);
			}
			if (m_offsetY != null)
			{
				m_offsetY.Initialize("OffsetY", context);
				context.ExprHostBuilder.CapImageOffsetY(m_offsetY);
			}
			context.ExprHostBuilder.CapImageEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			CapImage capImage = (CapImage)base.PublishClone(context);
			if (m_hueColor != null)
			{
				capImage.m_hueColor = (ExpressionInfo)m_hueColor.PublishClone(context);
			}
			if (m_offsetX != null)
			{
				capImage.m_offsetX = (ExpressionInfo)m_offsetX.PublishClone(context);
			}
			if (m_offsetY != null)
			{
				capImage.m_offsetY = (ExpressionInfo)m_offsetY.PublishClone(context);
			}
			return capImage;
		}

		internal void SetExprHost(CapImageExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			SetExprHost((BaseGaugeImageExprHost)exprHost, reportObjectModel);
			m_exprHost = exprHost;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.HueColor, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.OffsetX, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.OffsetY, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.CapImage, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BaseGaugeImage, list);
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
				case MemberName.OffsetX:
					writer.Write(m_offsetX);
					break;
				case MemberName.OffsetY:
					writer.Write(m_offsetY);
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
				case MemberName.OffsetX:
					m_offsetX = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.OffsetY:
					m_offsetY = (ExpressionInfo)reader.ReadRIFObject();
					break;
				}
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.CapImage;
		}

		internal string EvaluateHueColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateCapImageHueColorExpression(this, m_gaugePanel.Name);
		}

		internal string EvaluateOffsetX(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateCapImageOffsetXExpression(this, m_gaugePanel.Name);
		}

		internal string EvaluateOffsetY(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateCapImageOffsetYExpression(this, m_gaugePanel.Name);
		}
	}
}
