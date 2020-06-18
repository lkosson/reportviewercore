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
	internal sealed class PointerImage : BaseGaugeImage, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private ExpressionInfo m_hueColor;

		private ExpressionInfo m_transparency;

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

		internal PointerImage()
		{
		}

		internal PointerImage(GaugePanel gaugePanel)
			: base(gaugePanel)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.PointerImageStart();
			base.Initialize(context);
			if (m_hueColor != null)
			{
				m_hueColor.Initialize("HueColor", context);
				context.ExprHostBuilder.PointerImageHueColor(m_hueColor);
			}
			if (m_transparency != null)
			{
				m_transparency.Initialize("Transparency", context);
				context.ExprHostBuilder.PointerImageTransparency(m_transparency);
			}
			if (m_offsetX != null)
			{
				m_offsetX.Initialize("OffsetX", context);
				context.ExprHostBuilder.PointerImageOffsetX(m_offsetX);
			}
			if (m_offsetY != null)
			{
				m_offsetY.Initialize("OffsetY", context);
				context.ExprHostBuilder.PointerImageOffsetY(m_offsetY);
			}
			context.ExprHostBuilder.PointerImageEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			PointerImage pointerImage = (PointerImage)base.PublishClone(context);
			if (m_hueColor != null)
			{
				pointerImage.m_hueColor = (ExpressionInfo)m_hueColor.PublishClone(context);
			}
			if (m_transparency != null)
			{
				pointerImage.m_transparency = (ExpressionInfo)m_transparency.PublishClone(context);
			}
			if (m_offsetX != null)
			{
				pointerImage.m_offsetX = (ExpressionInfo)m_offsetX.PublishClone(context);
			}
			if (m_offsetY != null)
			{
				pointerImage.m_offsetY = (ExpressionInfo)m_offsetY.PublishClone(context);
			}
			return pointerImage;
		}

		internal void SetExprHost(PointerImageExprHost exprHost, ObjectModelImpl reportObjectModel)
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
			list.Add(new MemberInfo(MemberName.OffsetX, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.OffsetY, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PointerImage, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BaseGaugeImage, list);
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
				case MemberName.Transparency:
					m_transparency = (ExpressionInfo)reader.ReadRIFObject();
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
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PointerImage;
		}

		internal string EvaluateHueColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluatePointerImageHueColorExpression(this, m_gaugePanel.Name);
		}

		internal double EvaluateTransparency(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluatePointerImageTransparencyExpression(this, m_gaugePanel.Name);
		}

		internal string EvaluateOffsetX(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluatePointerImageOffsetXExpression(this, m_gaugePanel.Name);
		}

		internal string EvaluateOffsetY(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluatePointerImageOffsetYExpression(this, m_gaugePanel.Name);
		}
	}
}
