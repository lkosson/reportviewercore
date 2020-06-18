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
	internal sealed class ScalePin : TickMarkStyle, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private ExpressionInfo m_location;

		private ExpressionInfo m_enable;

		private PinLabel m_pinLabel;

		internal ExpressionInfo Location
		{
			get
			{
				return m_location;
			}
			set
			{
				m_location = value;
			}
		}

		internal ExpressionInfo Enable
		{
			get
			{
				return m_enable;
			}
			set
			{
				m_enable = value;
			}
		}

		internal PinLabel PinLabel
		{
			get
			{
				return m_pinLabel;
			}
			set
			{
				m_pinLabel = value;
			}
		}

		internal ScalePin()
		{
		}

		internal ScalePin(GaugePanel gaugePanel)
			: base(gaugePanel)
		{
		}

		internal void Initialize(InitializationContext context, bool isMaximum)
		{
			context.ExprHostBuilder.ScalePinStart(isMaximum);
			InitializeInternal(context);
			if (m_location != null)
			{
				m_location.Initialize("Location", context);
				context.ExprHostBuilder.ScalePinLocation(m_location);
			}
			if (m_enable != null)
			{
				m_enable.Initialize("Enable", context);
				context.ExprHostBuilder.ScalePinEnable(m_enable);
			}
			if (m_pinLabel != null)
			{
				m_pinLabel.Initialize(context);
			}
			context.ExprHostBuilder.ScalePinEnd(isMaximum);
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			ScalePin scalePin = (ScalePin)base.PublishClone(context);
			if (m_location != null)
			{
				scalePin.m_location = (ExpressionInfo)m_location.PublishClone(context);
			}
			if (m_enable != null)
			{
				scalePin.m_enable = (ExpressionInfo)m_enable.PublishClone(context);
			}
			if (m_pinLabel != null)
			{
				scalePin.m_pinLabel = (PinLabel)m_pinLabel.PublishClone(context);
			}
			return scalePin;
		}

		internal void SetExprHost(ScalePinExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			SetExprHost((TickMarkStyleExprHost)exprHost, reportObjectModel);
			m_exprHost = exprHost;
			if (m_pinLabel != null && ((ScalePinExprHost)m_exprHost).PinLabelHost != null)
			{
				m_pinLabel.SetExprHost(((ScalePinExprHost)m_exprHost).PinLabelHost, reportObjectModel);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Location, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Enable, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.PinLabel, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PinLabel));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalePin, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TickMarkStyle, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Location:
					writer.Write(m_location);
					break;
				case MemberName.Enable:
					writer.Write(m_enable);
					break;
				case MemberName.PinLabel:
					writer.Write(m_pinLabel);
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
				case MemberName.Location:
					m_location = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Enable:
					m_enable = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.PinLabel:
					m_pinLabel = (PinLabel)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalePin;
		}

		internal double EvaluateLocation(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateScalePinLocationExpression(this, m_gaugePanel.Name);
		}

		internal bool EvaluateEnable(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateScalePinEnableExpression(this, m_gaugePanel.Name);
		}
	}
}
