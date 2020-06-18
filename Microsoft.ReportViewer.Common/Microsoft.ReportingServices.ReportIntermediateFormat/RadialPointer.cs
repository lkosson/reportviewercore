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
	internal sealed class RadialPointer : GaugePointer, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private ExpressionInfo m_type;

		private PointerCap m_pointerCap;

		private ExpressionInfo m_needleStyle;

		internal ExpressionInfo Type
		{
			get
			{
				return m_type;
			}
			set
			{
				m_type = value;
			}
		}

		internal PointerCap PointerCap
		{
			get
			{
				return m_pointerCap;
			}
			set
			{
				m_pointerCap = value;
			}
		}

		internal ExpressionInfo NeedleStyle
		{
			get
			{
				return m_needleStyle;
			}
			set
			{
				m_needleStyle = value;
			}
		}

		internal RadialPointer()
		{
		}

		internal RadialPointer(GaugePanel gaugePanel, int id)
			: base(gaugePanel, id)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.RadialPointerStart(m_name);
			base.Initialize(context);
			if (m_type != null)
			{
				m_type.Initialize("Type", context);
				context.ExprHostBuilder.RadialPointerType(m_type);
			}
			if (m_pointerCap != null)
			{
				m_pointerCap.Initialize(context);
			}
			if (m_needleStyle != null)
			{
				m_needleStyle.Initialize("NeedleStyle", context);
				context.ExprHostBuilder.RadialPointerNeedleStyle(m_needleStyle);
			}
			m_exprHostID = context.ExprHostBuilder.RadialPointerEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			RadialPointer radialPointer = (RadialPointer)base.PublishClone(context);
			if (m_type != null)
			{
				radialPointer.m_type = (ExpressionInfo)m_type.PublishClone(context);
			}
			if (m_pointerCap != null)
			{
				radialPointer.m_pointerCap = (PointerCap)m_pointerCap.PublishClone(context);
			}
			if (m_needleStyle != null)
			{
				radialPointer.m_needleStyle = (ExpressionInfo)m_needleStyle.PublishClone(context);
			}
			return radialPointer;
		}

		internal void SetExprHost(RadialPointerExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			SetExprHost((GaugePointerExprHost)exprHost, reportObjectModel);
			m_exprHost = exprHost;
			if (m_pointerCap != null && ((RadialPointerExprHost)m_exprHost).PointerCapHost != null)
			{
				m_pointerCap.SetExprHost(((RadialPointerExprHost)m_exprHost).PointerCapHost, reportObjectModel);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Type, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.PointerCap, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PointerCap));
			list.Add(new MemberInfo(MemberName.NeedleStyle, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RadialPointer, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePointer, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Type:
					writer.Write(m_type);
					break;
				case MemberName.PointerCap:
					writer.Write(m_pointerCap);
					break;
				case MemberName.NeedleStyle:
					writer.Write(m_needleStyle);
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
				case MemberName.Type:
					m_type = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.PointerCap:
					m_pointerCap = (PointerCap)reader.ReadRIFObject();
					break;
				case MemberName.NeedleStyle:
					m_needleStyle = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RadialPointer;
		}

		internal RadialPointerTypes EvaluateType(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return EnumTranslator.TranslateRadialPointerTypes(context.ReportRuntime.EvaluateRadialPointerTypeExpression(this, m_gaugePanel.Name), context.ReportRuntime);
		}

		internal RadialPointerNeedleStyles EvaluateNeedleStyle(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return EnumTranslator.TranslateRadialPointerNeedleStyles(context.ReportRuntime.EvaluateRadialPointerNeedleStyleExpression(this, m_gaugePanel.Name), context.ReportRuntime);
		}
	}
}
