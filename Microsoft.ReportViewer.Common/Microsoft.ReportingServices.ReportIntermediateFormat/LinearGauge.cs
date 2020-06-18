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
	internal sealed class LinearGauge : Gauge, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private List<LinearScale> m_gaugeScales;

		private ExpressionInfo m_orientation;

		internal List<LinearScale> GaugeScales
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

		internal ExpressionInfo Orientation
		{
			get
			{
				return m_orientation;
			}
			set
			{
				m_orientation = value;
			}
		}

		internal LinearGauge()
		{
		}

		internal LinearGauge(GaugePanel gaugePanel, int id)
			: base(gaugePanel, id)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.LinearGaugeStart(m_name);
			base.Initialize(context);
			if (m_gaugeScales != null)
			{
				for (int i = 0; i < m_gaugeScales.Count; i++)
				{
					m_gaugeScales[i].Initialize(context);
				}
			}
			if (m_orientation != null)
			{
				m_orientation.Initialize("Orientation", context);
				context.ExprHostBuilder.LinearGaugeOrientation(m_orientation);
			}
			m_exprHostID = context.ExprHostBuilder.LinearGaugeEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			LinearGauge linearGauge = (LinearGauge)base.PublishClone(context);
			if (m_gaugeScales != null)
			{
				linearGauge.m_gaugeScales = new List<LinearScale>(m_gaugeScales.Count);
				foreach (LinearScale gaugeScale in m_gaugeScales)
				{
					linearGauge.m_gaugeScales.Add((LinearScale)gaugeScale.PublishClone(context));
				}
			}
			if (m_orientation != null)
			{
				linearGauge.m_orientation = (ExpressionInfo)m_orientation.PublishClone(context);
			}
			return linearGauge;
		}

		internal void SetExprHost(LinearGaugeExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			SetExprHost((GaugeExprHost)exprHost, reportObjectModel);
			m_exprHost = exprHost;
			IList<LinearScaleExprHost> linearScalesHostsRemotable = ((LinearGaugeExprHost)m_exprHost).LinearScalesHostsRemotable;
			if (m_gaugeScales == null || linearScalesHostsRemotable == null)
			{
				return;
			}
			for (int i = 0; i < m_gaugeScales.Count; i++)
			{
				LinearScale linearScale = m_gaugeScales[i];
				if (linearScale != null && linearScale.ExpressionHostID > -1)
				{
					linearScale.SetExprHost(linearScalesHostsRemotable[linearScale.ExpressionHostID], reportObjectModel);
				}
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.GaugeScales, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LinearScale));
			list.Add(new MemberInfo(MemberName.Orientation, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LinearGauge, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Gauge, list);
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
				case MemberName.Orientation:
					writer.Write(m_orientation);
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
					m_gaugeScales = reader.ReadGenericListOfRIFObjects<LinearScale>();
					break;
				case MemberName.Orientation:
					m_orientation = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LinearGauge;
		}

		internal GaugeOrientations EvaluateOrientation(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return EnumTranslator.TranslateGaugeOrientations(context.ReportRuntime.EvaluateLinearGaugeOrientationExpression(this, m_gaugePanel.Name), context.ReportRuntime);
		}
	}
}
