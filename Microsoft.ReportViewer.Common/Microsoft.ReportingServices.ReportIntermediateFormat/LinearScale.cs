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
	internal sealed class LinearScale : GaugeScale, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private List<LinearPointer> m_gaugePointers;

		private ExpressionInfo m_startMargin;

		private ExpressionInfo m_endMargin;

		private ExpressionInfo m_position;

		internal List<LinearPointer> GaugePointers
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

		internal ExpressionInfo StartMargin
		{
			get
			{
				return m_startMargin;
			}
			set
			{
				m_startMargin = value;
			}
		}

		internal ExpressionInfo EndMargin
		{
			get
			{
				return m_endMargin;
			}
			set
			{
				m_endMargin = value;
			}
		}

		internal ExpressionInfo Position
		{
			get
			{
				return m_position;
			}
			set
			{
				m_position = value;
			}
		}

		internal LinearScale()
		{
		}

		internal LinearScale(GaugePanel gaugePanel, int id)
			: base(gaugePanel, id)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.LinearScaleStart(m_name);
			base.Initialize(context);
			if (m_gaugePointers != null)
			{
				for (int i = 0; i < m_gaugePointers.Count; i++)
				{
					m_gaugePointers[i].Initialize(context);
				}
			}
			if (m_startMargin != null)
			{
				m_startMargin.Initialize("StartMargin", context);
				context.ExprHostBuilder.LinearScaleStartMargin(m_startMargin);
			}
			if (m_endMargin != null)
			{
				m_endMargin.Initialize("EndMargin", context);
				context.ExprHostBuilder.LinearScaleEndMargin(m_endMargin);
			}
			if (m_position != null)
			{
				m_position.Initialize("Position", context);
				context.ExprHostBuilder.LinearScalePosition(m_position);
			}
			m_exprHostID = context.ExprHostBuilder.LinearScaleEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			LinearScale linearScale = (LinearScale)base.PublishClone(context);
			if (m_gaugePointers != null)
			{
				linearScale.m_gaugePointers = new List<LinearPointer>(m_gaugePointers.Count);
				foreach (LinearPointer gaugePointer in m_gaugePointers)
				{
					linearScale.m_gaugePointers.Add((LinearPointer)gaugePointer.PublishClone(context));
				}
			}
			if (m_startMargin != null)
			{
				linearScale.m_startMargin = (ExpressionInfo)m_startMargin.PublishClone(context);
			}
			if (m_endMargin != null)
			{
				linearScale.m_endMargin = (ExpressionInfo)m_endMargin.PublishClone(context);
			}
			if (m_position != null)
			{
				linearScale.m_position = (ExpressionInfo)m_position.PublishClone(context);
			}
			return linearScale;
		}

		internal void SetExprHost(LinearScaleExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			SetExprHost((GaugeScaleExprHost)exprHost, reportObjectModel);
			m_exprHost = exprHost;
			IList<LinearPointerExprHost> linearPointersHostsRemotable = ((LinearScaleExprHost)m_exprHost).LinearPointersHostsRemotable;
			if (m_gaugePointers == null || linearPointersHostsRemotable == null)
			{
				return;
			}
			for (int i = 0; i < m_gaugePointers.Count; i++)
			{
				LinearPointer linearPointer = m_gaugePointers[i];
				if (linearPointer != null && linearPointer.ExpressionHostID > -1)
				{
					linearPointer.SetExprHost(linearPointersHostsRemotable[linearPointer.ExpressionHostID], reportObjectModel);
				}
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.GaugePointers, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LinearPointer));
			list.Add(new MemberInfo(MemberName.StartMargin, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.EndMargin, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Position, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LinearScale, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeScale, list);
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
				case MemberName.StartMargin:
					writer.Write(m_startMargin);
					break;
				case MemberName.EndMargin:
					writer.Write(m_endMargin);
					break;
				case MemberName.Position:
					writer.Write(m_position);
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
					m_gaugePointers = reader.ReadGenericListOfRIFObjects<LinearPointer>();
					break;
				case MemberName.StartMargin:
					m_startMargin = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.EndMargin:
					m_endMargin = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Position:
					m_position = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LinearScale;
		}

		internal double EvaluateStartMargin(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateLinearScaleStartMarginExpression(this, m_gaugePanel.Name);
		}

		internal double EvaluateEndMargin(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateLinearScaleEndMarginExpression(this, m_gaugePanel.Name);
		}

		internal double EvaluatePosition(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateLinearScalePositionExpression(this, m_gaugePanel.Name);
		}
	}
}
