using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.RdlExpressions;
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
	internal sealed class ChartFormulaParameter : IPersistable
	{
		private string m_name;

		private int m_exprHostID;

		[Reference]
		private Chart m_chart;

		[Reference]
		private ChartSeries m_sourceSeries;

		[NonSerialized]
		private ChartDerivedSeries m_parentDerivedSeries;

		private ExpressionInfo m_value;

		private string m_source;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		[NonSerialized]
		private ChartFormulaParameterExprHost m_exprHost;

		internal string FormulaParameterName
		{
			get
			{
				return m_name;
			}
			set
			{
				m_name = value;
			}
		}

		internal ChartFormulaParameterExprHost ExprHost => m_exprHost;

		internal int ExpressionHostID => m_exprHostID;

		internal ExpressionInfo Value
		{
			get
			{
				return m_value;
			}
			set
			{
				m_value = value;
			}
		}

		internal string Source
		{
			get
			{
				return m_source;
			}
			set
			{
				m_source = value;
			}
		}

		private ChartSeries SourceSeries
		{
			get
			{
				if (m_sourceSeries == null && m_parentDerivedSeries != null)
				{
					m_sourceSeries = m_parentDerivedSeries.SourceSeries;
				}
				return m_sourceSeries;
			}
		}

		internal ChartFormulaParameter()
		{
		}

		internal ChartFormulaParameter(Chart chart, ChartDerivedSeries parentDerivedSeries)
		{
			m_chart = chart;
			m_parentDerivedSeries = parentDerivedSeries;
		}

		internal void SetExprHost(ChartFormulaParameterExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			m_exprHost = exprHost;
			m_exprHost.SetReportObjectModel(reportObjectModel);
		}

		internal void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.ChartFormulaParameterStart(m_name);
			if (m_value != null)
			{
				m_value.Initialize("Value", context);
				context.ExprHostBuilder.ChartFormulaParameterValue(m_value);
			}
			m_exprHostID = context.ExprHostBuilder.ChartFormulaParameterEnd();
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			ChartFormulaParameter chartFormulaParameter = (ChartFormulaParameter)MemberwiseClone();
			chartFormulaParameter.m_chart = (Chart)context.CurrentDataRegionClone;
			if (m_value != null)
			{
				chartFormulaParameter.m_value = (ExpressionInfo)m_value.PublishClone(context);
			}
			return chartFormulaParameter;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Name, Token.String));
			list.Add(new MemberInfo(MemberName.Value, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Source, Token.String));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.Chart, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Chart, Token.Reference));
			list.Add(new MemberInfo(MemberName.SourceSeries, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartSeries, Token.Reference));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartFormulaParameter, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		internal Microsoft.ReportingServices.RdlExpressions.VariantResult EvaluateValue(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(SourceSeries, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartFormulaParameterValueExpression(this, m_chart.Name);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Name:
					writer.Write(m_name);
					break;
				case MemberName.Value:
					writer.Write(m_value);
					break;
				case MemberName.Source:
					writer.Write(m_source);
					break;
				case MemberName.ExprHostID:
					writer.Write(m_exprHostID);
					break;
				case MemberName.Chart:
					writer.WriteReference(m_chart);
					break;
				case MemberName.SourceSeries:
					writer.WriteReference(SourceSeries);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Name:
					m_name = reader.ReadString();
					break;
				case MemberName.Value:
					m_value = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Source:
					m_source = reader.ReadString();
					break;
				case MemberName.ExprHostID:
					m_exprHostID = reader.ReadInt32();
					break;
				case MemberName.Chart:
					m_chart = reader.ReadReference<Chart>(this);
					break;
				case MemberName.SourceSeries:
					m_sourceSeries = reader.ReadReference<ChartSeries>(this);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			if (!memberReferencesCollection.TryGetValue(m_Declaration.ObjectType, out List<MemberReference> value))
			{
				return;
			}
			foreach (MemberReference item in value)
			{
				switch (item.MemberName)
				{
				case MemberName.Chart:
					Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
					m_chart = (Chart)referenceableItems[item.RefID];
					break;
				case MemberName.SourceSeries:
					Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
					m_sourceSeries = (ChartSeries)referenceableItems[item.RefID];
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartFormulaParameter;
		}
	}
}
