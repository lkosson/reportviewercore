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
	internal sealed class ChartAxisScaleBreak : ChartStyleContainer, IPersistable
	{
		private ExpressionInfo m_enabled;

		private ExpressionInfo m_breakLineType;

		private ExpressionInfo m_collapsibleSpaceThreshold;

		private ExpressionInfo m_maxNumberOfBreaks;

		private ExpressionInfo m_spacing;

		private ExpressionInfo m_includeZero;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		[NonSerialized]
		private ChartAxisScaleBreakExprHost m_exprHost;

		internal ChartAxisScaleBreakExprHost ExprHost => m_exprHost;

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

		internal ExpressionInfo BreakLineType
		{
			get
			{
				return m_breakLineType;
			}
			set
			{
				m_breakLineType = value;
			}
		}

		internal ExpressionInfo CollapsibleSpaceThreshold
		{
			get
			{
				return m_collapsibleSpaceThreshold;
			}
			set
			{
				m_collapsibleSpaceThreshold = value;
			}
		}

		internal ExpressionInfo MaxNumberOfBreaks
		{
			get
			{
				return m_maxNumberOfBreaks;
			}
			set
			{
				m_maxNumberOfBreaks = value;
			}
		}

		internal ExpressionInfo Spacing
		{
			get
			{
				return m_spacing;
			}
			set
			{
				m_spacing = value;
			}
		}

		internal ExpressionInfo IncludeZero
		{
			get
			{
				return m_includeZero;
			}
			set
			{
				m_includeZero = value;
			}
		}

		internal ChartAxisScaleBreak()
		{
		}

		internal ChartAxisScaleBreak(Chart chart)
			: base(chart)
		{
		}

		internal void SetExprHost(ChartAxisScaleBreakExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
			m_exprHost = exprHost;
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.ChartAxisScaleBreakStart();
			base.Initialize(context);
			if (m_enabled != null)
			{
				m_enabled.Initialize("Enabled", context);
				context.ExprHostBuilder.ChartAxisScaleBreakEnabled(m_enabled);
			}
			if (m_breakLineType != null)
			{
				m_breakLineType.Initialize("BreakLineType", context);
				context.ExprHostBuilder.ChartAxisScaleBreakBreakLineType(m_breakLineType);
			}
			if (m_collapsibleSpaceThreshold != null)
			{
				m_collapsibleSpaceThreshold.Initialize("CollapsibleSpaceThreshold", context);
				context.ExprHostBuilder.ChartAxisScaleBreakCollapsibleSpaceThreshold(m_collapsibleSpaceThreshold);
			}
			if (m_maxNumberOfBreaks != null)
			{
				m_maxNumberOfBreaks.Initialize("MaxNumberOfBreaks", context);
				context.ExprHostBuilder.ChartAxisScaleBreakMaxNumberOfBreaks(m_maxNumberOfBreaks);
			}
			if (m_spacing != null)
			{
				m_spacing.Initialize("Spacing", context);
				context.ExprHostBuilder.ChartAxisScaleBreakSpacing(m_spacing);
			}
			if (m_includeZero != null)
			{
				m_includeZero.Initialize("IncludeZero", context);
				context.ExprHostBuilder.ChartAxisScaleBreakIncludeZero(m_includeZero);
			}
			context.ExprHostBuilder.ChartAxisScaleBreakEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			ChartAxisScaleBreak chartAxisScaleBreak = (ChartAxisScaleBreak)base.PublishClone(context);
			if (m_enabled != null)
			{
				chartAxisScaleBreak.m_enabled = (ExpressionInfo)m_enabled.PublishClone(context);
			}
			if (m_breakLineType != null)
			{
				chartAxisScaleBreak.m_breakLineType = (ExpressionInfo)m_breakLineType.PublishClone(context);
			}
			if (m_collapsibleSpaceThreshold != null)
			{
				chartAxisScaleBreak.m_collapsibleSpaceThreshold = (ExpressionInfo)m_collapsibleSpaceThreshold.PublishClone(context);
			}
			if (m_maxNumberOfBreaks != null)
			{
				chartAxisScaleBreak.m_maxNumberOfBreaks = (ExpressionInfo)m_maxNumberOfBreaks.PublishClone(context);
			}
			if (m_spacing != null)
			{
				chartAxisScaleBreak.m_spacing = (ExpressionInfo)m_spacing.PublishClone(context);
			}
			if (m_includeZero != null)
			{
				chartAxisScaleBreak.m_includeZero = (ExpressionInfo)m_includeZero.PublishClone(context);
			}
			return chartAxisScaleBreak;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Enabled, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.BreakLineType, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.CollapsibleSpaceThreshold, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MaxNumberOfBreaks, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Spacing, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.IncludeZero, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartAxisScaleBreak, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartStyleContainer, list);
		}

		internal bool EvaluateEnabled(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartAxisScaleBreakEnabledExpression(this, m_chart.Name);
		}

		internal ChartBreakLineType EvaluateBreakLineType(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return EnumTranslator.TranslateChartBreakLineType(context.ReportRuntime.EvaluateChartAxisScaleBreakBreakLineTypeExpression(this, m_chart.Name), context.ReportRuntime);
		}

		internal int EvaluateCollapsibleSpaceThreshold(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartAxisScaleBreakCollapsibleSpaceThresholdExpression(this, m_chart.Name);
		}

		internal int EvaluateMaxNumberOfBreaks(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartAxisScaleBreakMaxNumberOfBreaksExpression(this, m_chart.Name);
		}

		internal double EvaluateSpacing(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartAxisScaleBreakSpacingExpression(this, m_chart.Name);
		}

		internal string EvaluateIncludeZero(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartAxisScaleBreakIncludeZeroExpression(this, m_chart.Name);
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
				case MemberName.BreakLineType:
					writer.Write(m_breakLineType);
					break;
				case MemberName.CollapsibleSpaceThreshold:
					writer.Write(m_collapsibleSpaceThreshold);
					break;
				case MemberName.MaxNumberOfBreaks:
					writer.Write(m_maxNumberOfBreaks);
					break;
				case MemberName.Spacing:
					writer.Write(m_spacing);
					break;
				case MemberName.IncludeZero:
					writer.Write(m_includeZero);
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
				case MemberName.BreakLineType:
					m_breakLineType = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.CollapsibleSpaceThreshold:
					m_collapsibleSpaceThreshold = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MaxNumberOfBreaks:
					m_maxNumberOfBreaks = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Spacing:
					m_spacing = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.IncludeZero:
					m_includeZero = (ExpressionInfo)reader.ReadRIFObject();
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
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartAxisScaleBreak;
		}
	}
}
