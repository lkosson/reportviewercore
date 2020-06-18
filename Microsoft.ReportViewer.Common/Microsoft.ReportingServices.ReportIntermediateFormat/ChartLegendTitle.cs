using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class ChartLegendTitle : ChartTitleBase, IPersistable
	{
		private ExpressionInfo m_titleSeparator;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal ExpressionInfo TitleSeparator
		{
			get
			{
				return m_titleSeparator;
			}
			set
			{
				m_titleSeparator = value;
			}
		}

		internal ChartLegendTitle()
		{
		}

		internal ChartLegendTitle(Chart chart)
			: base(chart)
		{
			m_chart = chart;
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.ChartLegendTitleStart();
			base.Initialize(context);
			if (m_titleSeparator != null)
			{
				m_titleSeparator.Initialize("TitleSeparator", context);
				context.ExprHostBuilder.ChartLegendTitleSeparator(m_titleSeparator);
			}
			context.ExprHostBuilder.ChartLegendTitleEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			ChartLegendTitle chartLegendTitle = (ChartLegendTitle)base.PublishClone(context);
			if (m_titleSeparator != null)
			{
				chartLegendTitle.m_titleSeparator = (ExpressionInfo)m_titleSeparator.PublishClone(context);
			}
			return chartLegendTitle;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.TitleSeparator, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartLegendTitle, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartTitleBase, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.TitleSeparator)
				{
					writer.Write(m_titleSeparator);
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				if (memberName == MemberName.TitleSeparator)
				{
					m_titleSeparator = (ExpressionInfo)reader.ReadRIFObject();
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public override void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			base.ResolveReferences(memberReferencesCollection, referenceableItems);
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartLegendTitle;
		}

		internal ChartSeparators EvaluateTitleSeparator(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return EnumTranslator.TranslateChartSeparator(context.ReportRuntime.EvaluateChartLegendTitleTitleSeparatorExpression(this, m_chart.Name), context.ReportRuntime);
		}
	}
}
