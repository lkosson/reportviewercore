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
	internal sealed class ChartAxisTitle : ChartTitleBase, IPersistable
	{
		private ExpressionInfo m_position;

		private ExpressionInfo m_textOrientation;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

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

		internal ExpressionInfo TextOrientation
		{
			get
			{
				return m_textOrientation;
			}
			set
			{
				m_textOrientation = value;
			}
		}

		internal ChartAxisTitle()
		{
		}

		internal ChartAxisTitle(Chart chart)
			: base(chart)
		{
			m_chart = chart;
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.ChartAxisTitleStart();
			base.Initialize(context);
			if (m_position != null)
			{
				m_position.Initialize("Position", context);
				context.ExprHostBuilder.ChartTitlePosition(m_position);
			}
			if (m_textOrientation != null)
			{
				m_textOrientation.Initialize("TextOrientation", context);
				context.ExprHostBuilder.ChartAxisTitleTextOrientation(m_textOrientation);
			}
			context.ExprHostBuilder.ChartAxisTitleEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			ChartAxisTitle chartAxisTitle = (ChartAxisTitle)base.PublishClone(context);
			if (m_position != null)
			{
				chartAxisTitle.m_position = (ExpressionInfo)m_position.PublishClone(context);
			}
			if (m_textOrientation != null)
			{
				chartAxisTitle.m_textOrientation = (ExpressionInfo)m_textOrientation.PublishClone(context);
			}
			return chartAxisTitle;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Position, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.TextOrientation, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartAxisTitle, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartTitle, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Position:
					writer.Write(m_position);
					break;
				case MemberName.TextOrientation:
					writer.Write(m_textOrientation);
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
				case MemberName.Position:
					m_position = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.TextOrientation:
					m_textOrientation = (ExpressionInfo)reader.ReadRIFObject();
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
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartAxisTitle;
		}

		internal ChartAxisTitlePositions EvaluatePosition(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, instance);
			return EnumTranslator.TranslateChartAxisTitlePosition(context.ReportRuntime.EvaluateChartAxisTitlePositionExpression(this, base.Name, "Position"), context.ReportRuntime);
		}

		internal TextOrientations EvaluateTextOrientation(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return EnumTranslator.TranslateTextOrientations(context.ReportRuntime.EvaluateChartAxisTitleTextOrientationExpression(this, m_chart.Name), context.ReportRuntime);
		}
	}
}
