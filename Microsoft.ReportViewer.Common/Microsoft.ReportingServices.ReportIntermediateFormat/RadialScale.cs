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
	internal sealed class RadialScale : GaugeScale, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private List<RadialPointer> m_gaugePointers;

		private ExpressionInfo m_radius;

		private ExpressionInfo m_startAngle;

		private ExpressionInfo m_sweepAngle;

		internal List<RadialPointer> GaugePointers
		{
			get
			{
				return m_gaugePointers;
			}
			set
			{
				m_gaugePointers = value;
			}
		}

		internal ExpressionInfo Radius
		{
			get
			{
				return m_radius;
			}
			set
			{
				m_radius = value;
			}
		}

		internal ExpressionInfo StartAngle
		{
			get
			{
				return m_startAngle;
			}
			set
			{
				m_startAngle = value;
			}
		}

		internal ExpressionInfo SweepAngle
		{
			get
			{
				return m_sweepAngle;
			}
			set
			{
				m_sweepAngle = value;
			}
		}

		internal RadialScale()
		{
		}

		internal RadialScale(GaugePanel gaugePanel, int id)
			: base(gaugePanel, id)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.RadialScaleStart(m_name);
			base.Initialize(context);
			if (m_gaugePointers != null)
			{
				for (int i = 0; i < m_gaugePointers.Count; i++)
				{
					m_gaugePointers[i].Initialize(context);
				}
			}
			if (m_radius != null)
			{
				m_radius.Initialize("Radius", context);
				context.ExprHostBuilder.RadialScaleRadius(m_radius);
			}
			if (m_startAngle != null)
			{
				m_startAngle.Initialize("StartAngle", context);
				context.ExprHostBuilder.RadialScaleStartAngle(m_startAngle);
			}
			if (m_sweepAngle != null)
			{
				m_sweepAngle.Initialize("SweepAngle", context);
				context.ExprHostBuilder.RadialScaleSweepAngle(m_sweepAngle);
			}
			m_exprHostID = context.ExprHostBuilder.RadialScaleEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			RadialScale radialScale = (RadialScale)base.PublishClone(context);
			if (m_gaugePointers != null)
			{
				radialScale.m_gaugePointers = new List<RadialPointer>(m_gaugePointers.Count);
				foreach (RadialPointer gaugePointer in m_gaugePointers)
				{
					radialScale.m_gaugePointers.Add((RadialPointer)gaugePointer.PublishClone(context));
				}
			}
			if (m_radius != null)
			{
				radialScale.m_radius = (ExpressionInfo)m_radius.PublishClone(context);
			}
			if (m_startAngle != null)
			{
				radialScale.m_startAngle = (ExpressionInfo)m_startAngle.PublishClone(context);
			}
			if (m_sweepAngle != null)
			{
				radialScale.m_sweepAngle = (ExpressionInfo)m_sweepAngle.PublishClone(context);
			}
			return radialScale;
		}

		internal void SetExprHost(RadialScaleExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			SetExprHost((GaugeScaleExprHost)exprHost, reportObjectModel);
			m_exprHost = exprHost;
			IList<RadialPointerExprHost> radialPointersHostsRemotable = ((RadialScaleExprHost)m_exprHost).RadialPointersHostsRemotable;
			if (m_gaugePointers == null || radialPointersHostsRemotable == null)
			{
				return;
			}
			for (int i = 0; i < m_gaugePointers.Count; i++)
			{
				RadialPointer radialPointer = m_gaugePointers[i];
				if (radialPointer != null && radialPointer.ExpressionHostID > -1)
				{
					radialPointer.SetExprHost(radialPointersHostsRemotable[radialPointer.ExpressionHostID], reportObjectModel);
				}
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.GaugePointers, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RadialPointer));
			list.Add(new MemberInfo(MemberName.Radius, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.StartAngle, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.SweepAngle, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RadialScale, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeScale, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.GaugePointers:
					writer.Write(m_gaugePointers);
					break;
				case MemberName.Radius:
					writer.Write(m_radius);
					break;
				case MemberName.StartAngle:
					writer.Write(m_startAngle);
					break;
				case MemberName.SweepAngle:
					writer.Write(m_sweepAngle);
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
				case MemberName.GaugePointers:
					m_gaugePointers = reader.ReadGenericListOfRIFObjects<RadialPointer>();
					break;
				case MemberName.Radius:
					m_radius = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.StartAngle:
					m_startAngle = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.SweepAngle:
					m_sweepAngle = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RadialScale;
		}

		internal double EvaluateRadius(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateRadialScaleRadiusExpression(this, m_gaugePanel.Name);
		}

		internal double EvaluateStartAngle(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateRadialScaleStartAngleExpression(this, m_gaugePanel.Name);
		}

		internal double EvaluateSweepAngle(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateRadialScaleSweepAngleExpression(this, m_gaugePanel.Name);
		}
	}
}
