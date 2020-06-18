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
	internal sealed class RadialGauge : Gauge, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private List<RadialScale> m_gaugeScales;

		private ExpressionInfo m_pivotX;

		private ExpressionInfo m_pivotY;

		internal List<RadialScale> GaugeScales
		{
			get
			{
				return m_gaugeScales;
			}
			set
			{
				m_gaugeScales = value;
			}
		}

		internal ExpressionInfo PivotX
		{
			get
			{
				return m_pivotX;
			}
			set
			{
				m_pivotX = value;
			}
		}

		internal ExpressionInfo PivotY
		{
			get
			{
				return m_pivotY;
			}
			set
			{
				m_pivotY = value;
			}
		}

		internal RadialGauge()
		{
		}

		internal RadialGauge(GaugePanel gaugePanel, int id)
			: base(gaugePanel, id)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.RadialGaugeStart(m_name);
			base.Initialize(context);
			if (m_gaugeScales != null)
			{
				for (int i = 0; i < m_gaugeScales.Count; i++)
				{
					m_gaugeScales[i].Initialize(context);
				}
			}
			if (m_pivotX != null)
			{
				m_pivotX.Initialize("PivotX", context);
				context.ExprHostBuilder.RadialGaugePivotX(m_pivotX);
			}
			if (m_pivotY != null)
			{
				m_pivotY.Initialize("PivotY", context);
				context.ExprHostBuilder.RadialGaugePivotY(m_pivotY);
			}
			m_exprHostID = context.ExprHostBuilder.RadialGaugeEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			RadialGauge radialGauge = (RadialGauge)base.PublishClone(context);
			if (m_gaugeScales != null)
			{
				radialGauge.m_gaugeScales = new List<RadialScale>(m_gaugeScales.Count);
				foreach (RadialScale gaugeScale in m_gaugeScales)
				{
					radialGauge.m_gaugeScales.Add((RadialScale)gaugeScale.PublishClone(context));
				}
			}
			if (m_pivotX != null)
			{
				radialGauge.m_pivotX = (ExpressionInfo)m_pivotX.PublishClone(context);
			}
			if (m_pivotY != null)
			{
				radialGauge.m_pivotY = (ExpressionInfo)m_pivotY.PublishClone(context);
			}
			return radialGauge;
		}

		internal void SetExprHost(RadialGaugeExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			SetExprHost((GaugeExprHost)exprHost, reportObjectModel);
			m_exprHost = exprHost;
			IList<RadialScaleExprHost> radialScalesHostsRemotable = ((RadialGaugeExprHost)m_exprHost).RadialScalesHostsRemotable;
			if (m_gaugeScales == null || radialScalesHostsRemotable == null)
			{
				return;
			}
			for (int i = 0; i < m_gaugeScales.Count; i++)
			{
				RadialScale radialScale = m_gaugeScales[i];
				if (radialScale != null && radialScale.ExpressionHostID > -1)
				{
					radialScale.SetExprHost(radialScalesHostsRemotable[radialScale.ExpressionHostID], reportObjectModel);
				}
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.GaugeScales, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RadialScale));
			list.Add(new MemberInfo(MemberName.PivotX, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.PivotY, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RadialGauge, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Gauge, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.GaugeScales:
					writer.Write(m_gaugeScales);
					break;
				case MemberName.PivotX:
					writer.Write(m_pivotX);
					break;
				case MemberName.PivotY:
					writer.Write(m_pivotY);
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
				case MemberName.GaugeScales:
					m_gaugeScales = reader.ReadGenericListOfRIFObjects<RadialScale>();
					break;
				case MemberName.PivotX:
					m_pivotX = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.PivotY:
					m_pivotY = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RadialGauge;
		}

		internal double EvaluatePivotX(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateRadialGaugePivotXExpression(this, m_gaugePanel.Name);
		}

		internal double EvaluatePivotY(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateRadialGaugePivotYExpression(this, m_gaugePanel.Name);
		}
	}
}
