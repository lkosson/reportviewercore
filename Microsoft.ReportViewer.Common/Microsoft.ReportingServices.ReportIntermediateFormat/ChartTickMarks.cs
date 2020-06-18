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
	internal sealed class ChartTickMarks : ChartStyleContainer, IPersistable
	{
		private ExpressionInfo m_enabled;

		private ExpressionInfo m_type;

		private ExpressionInfo m_length;

		private ExpressionInfo m_interval;

		private ExpressionInfo m_intervalType;

		private ExpressionInfo m_intervalOffset;

		private ExpressionInfo m_intervalOffsetType;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		[NonSerialized]
		private ChartTickMarksExprHost m_exprHost;

		internal ChartTickMarksExprHost ExprHost => m_exprHost;

		internal ExpressionInfo Enabled
		{
			get
			{
				return m_enabled;
			}
			set
			{
				m_enabled = value;
			}
		}

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

		internal ExpressionInfo Length
		{
			get
			{
				return m_length;
			}
			set
			{
				m_length = value;
			}
		}

		internal ExpressionInfo Interval
		{
			get
			{
				return m_interval;
			}
			set
			{
				m_interval = value;
			}
		}

		internal ExpressionInfo IntervalType
		{
			get
			{
				return m_intervalType;
			}
			set
			{
				m_intervalType = value;
			}
		}

		internal ExpressionInfo IntervalOffset
		{
			get
			{
				return m_intervalOffset;
			}
			set
			{
				m_intervalOffset = value;
			}
		}

		internal ExpressionInfo IntervalOffsetType
		{
			get
			{
				return m_intervalOffsetType;
			}
			set
			{
				m_intervalOffsetType = value;
			}
		}

		internal ChartTickMarks()
		{
		}

		internal ChartTickMarks(Chart chart)
			: base(chart)
		{
		}

		internal void SetExprHost(ChartTickMarksExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			base.SetExprHost(exprHost, reportObjectModel);
			m_exprHost = exprHost;
		}

		internal void Initialize(InitializationContext context, bool isMajor)
		{
			context.ExprHostBuilder.ChartTickMarksStart(isMajor);
			base.Initialize(context);
			if (m_enabled != null)
			{
				m_enabled.Initialize("Enabled", context);
				context.ExprHostBuilder.ChartTickMarksEnabled(m_enabled);
			}
			if (m_type != null)
			{
				m_type.Initialize("Type", context);
				context.ExprHostBuilder.ChartTickMarksType(m_type);
			}
			if (m_length != null)
			{
				m_length.Initialize("Length", context);
				context.ExprHostBuilder.ChartTickMarksLength(m_length);
			}
			if (m_interval != null)
			{
				m_interval.Initialize("Interval", context);
				context.ExprHostBuilder.ChartTickMarksInterval(m_interval);
			}
			if (m_intervalType != null)
			{
				m_intervalType.Initialize("IntervalType", context);
				context.ExprHostBuilder.ChartTickMarksIntervalType(m_intervalType);
			}
			if (m_intervalOffset != null)
			{
				m_intervalOffset.Initialize("IntervalOffset", context);
				context.ExprHostBuilder.ChartTickMarksIntervalOffset(m_intervalOffset);
			}
			if (m_intervalOffsetType != null)
			{
				m_intervalOffsetType.Initialize("IntervalOffsetType", context);
				context.ExprHostBuilder.ChartTickMarksIntervalOffsetType(m_intervalOffsetType);
			}
			context.ExprHostBuilder.ChartTickMarksEnd(isMajor);
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			ChartTickMarks chartTickMarks = (ChartTickMarks)base.PublishClone(context);
			if (m_enabled != null)
			{
				chartTickMarks.m_enabled = (ExpressionInfo)m_enabled.PublishClone(context);
			}
			if (m_type != null)
			{
				chartTickMarks.m_type = (ExpressionInfo)m_type.PublishClone(context);
			}
			if (m_length != null)
			{
				chartTickMarks.m_length = (ExpressionInfo)m_length.PublishClone(context);
			}
			if (m_interval != null)
			{
				chartTickMarks.m_interval = (ExpressionInfo)m_interval.PublishClone(context);
			}
			if (m_intervalType != null)
			{
				chartTickMarks.m_intervalType = (ExpressionInfo)m_intervalType.PublishClone(context);
			}
			if (m_intervalOffset != null)
			{
				chartTickMarks.m_intervalOffset = (ExpressionInfo)m_intervalOffset.PublishClone(context);
			}
			if (m_intervalOffsetType != null)
			{
				chartTickMarks.m_intervalOffsetType = (ExpressionInfo)m_intervalOffsetType.PublishClone(context);
			}
			return chartTickMarks;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Enabled, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Type, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Length, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Interval, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.IntervalType, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.IntervalOffset, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.IntervalOffsetType, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartTickMarks, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartStyleContainer, list);
		}

		internal string EvaluateEnabled(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartTickMarksEnabledExpression(this, m_chart.Name);
		}

		internal ChartTickMarksType EvaluateType(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return EnumTranslator.TranslateChartTickMarksType(context.ReportRuntime.EvaluateChartTickMarksTypeExpression(this, m_chart.Name), context.ReportRuntime);
		}

		internal double EvaluateLength(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartTickMarksLengthExpression(this, m_chart.Name);
		}

		internal double EvaluateInterval(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartTickMarksIntervalExpression(this, m_chart.Name);
		}

		internal ChartIntervalType EvaluateIntervalType(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return EnumTranslator.TranslateChartIntervalType(context.ReportRuntime.EvaluateChartTickMarksIntervalTypeExpression(this, m_chart.Name), context.ReportRuntime);
		}

		internal double EvaluateIntervalOffset(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartTickMarksIntervalOffsetExpression(this, m_chart.Name);
		}

		internal ChartIntervalType EvaluateIntervalOffsetType(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return EnumTranslator.TranslateChartIntervalType(context.ReportRuntime.EvaluateChartTickMarksIntervalOffsetTypeExpression(this, m_chart.Name), context.ReportRuntime);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Enabled:
					writer.Write(m_enabled);
					break;
				case MemberName.Type:
					writer.Write(m_type);
					break;
				case MemberName.Length:
					writer.Write(m_length);
					break;
				case MemberName.Interval:
					writer.Write(m_interval);
					break;
				case MemberName.IntervalType:
					writer.Write(m_intervalType);
					break;
				case MemberName.IntervalOffset:
					writer.Write(m_intervalOffset);
					break;
				case MemberName.IntervalOffsetType:
					writer.Write(m_intervalOffsetType);
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
				case MemberName.Enabled:
					m_enabled = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Type:
					m_type = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Length:
					m_length = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Interval:
					m_interval = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.IntervalType:
					m_intervalType = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.IntervalOffset:
					m_intervalOffset = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.IntervalOffsetType:
					m_intervalOffsetType = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			base.ResolveReferences(memberReferencesCollection, referenceableItems);
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartTickMarks;
		}
	}
}
