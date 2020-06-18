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
	internal sealed class ChartGridLines : ChartStyleContainer, IPersistable
	{
		private ExpressionInfo m_enabled;

		private ExpressionInfo m_interval;

		private ExpressionInfo m_intervalType;

		private ExpressionInfo m_intervalOffset;

		private ExpressionInfo m_intervalOffsetType;

		[NonSerialized]
		private ChartGridLinesExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

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

		internal ChartGridLinesExprHost ExprHost => m_exprHost;

		internal ChartGridLines()
		{
		}

		internal ChartGridLines(Chart chart)
			: base(chart)
		{
		}

		internal void Initialize(InitializationContext context, bool isMajor)
		{
			context.ExprHostBuilder.ChartGridLinesStart(isMajor);
			base.Initialize(context);
			if (m_enabled != null)
			{
				m_enabled.Initialize("Enabled", context);
				context.ExprHostBuilder.ChartGridLinesEnabled(m_enabled);
			}
			if (m_interval != null)
			{
				m_interval.Initialize("Interval", context);
				context.ExprHostBuilder.ChartGridLinesInterval(m_interval);
			}
			if (m_intervalType != null)
			{
				m_intervalType.Initialize("IntervalType", context);
				context.ExprHostBuilder.ChartGridLinesEnabledIntervalType(m_intervalType);
			}
			if (m_intervalOffset != null)
			{
				m_intervalOffset.Initialize("IntervalOffset", context);
				context.ExprHostBuilder.ChartGridLinesIntervalOffset(m_intervalOffset);
			}
			if (m_intervalOffsetType != null)
			{
				m_intervalOffsetType.Initialize("IntervalOffsetType", context);
				context.ExprHostBuilder.ChartGridLinesIntervalOffsetType(m_intervalOffsetType);
			}
			context.ExprHostBuilder.ChartGridLinesEnd(isMajor);
		}

		internal void SetExprHost(ChartGridLinesExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			base.SetExprHost(exprHost, reportObjectModel);
			m_exprHost = exprHost;
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			ChartGridLines chartGridLines = (ChartGridLines)base.PublishClone(context);
			if (m_enabled != null)
			{
				chartGridLines.m_enabled = (ExpressionInfo)m_enabled.PublishClone(context);
			}
			if (m_interval != null)
			{
				chartGridLines.m_interval = (ExpressionInfo)m_interval.PublishClone(context);
			}
			if (m_intervalType != null)
			{
				chartGridLines.m_intervalType = (ExpressionInfo)m_intervalType.PublishClone(context);
			}
			if (m_intervalOffset != null)
			{
				chartGridLines.m_intervalOffset = (ExpressionInfo)m_intervalOffset.PublishClone(context);
			}
			if (m_intervalOffsetType != null)
			{
				chartGridLines.m_intervalOffsetType = (ExpressionInfo)m_intervalOffsetType.PublishClone(context);
			}
			return chartGridLines;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Enabled, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Interval, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.IntervalType, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.IntervalOffset, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.IntervalOffsetType, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GridLines, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartStyleContainer, list);
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
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GridLines;
		}

		internal ChartAutoBool EvaluateEnabled(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return EnumTranslator.TranslateChartAutoBool(context.ReportRuntime.EvaluateChartGridLinesEnabledExpression(this, m_chart.Name), context.ReportRuntime);
		}

		internal double EvaluateInterval(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, instance);
			return context.ReportRuntime.EvaluateChartGridLinesIntervalExpression(this, m_chart.Name, "Interval");
		}

		internal ChartIntervalType EvaluateIntervalType(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, instance);
			return EnumTranslator.TranslateChartIntervalType(context.ReportRuntime.EvaluateChartGridLinesIntervalTypeExpression(this, m_chart.Name, "IntervalType"), context.ReportRuntime);
		}

		internal double EvaluateIntervalOffset(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, instance);
			return context.ReportRuntime.EvaluateChartGridLinesIntervalOffsetExpression(this, m_chart.Name, "IntervalOffset");
		}

		internal ChartIntervalType EvaluateIntervalOffsetType(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, instance);
			return EnumTranslator.TranslateChartIntervalType(context.ReportRuntime.EvaluateChartGridLinesIntervalOffsetTypeExpression(this, m_chart.Name, "IntervalOffsetType"), context.ReportRuntime);
		}
	}
}
