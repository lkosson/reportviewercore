using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class ChartDerivedSeries : IPersistable
	{
		private int m_exprHostID;

		[Reference]
		private Chart m_chart;

		[NonSerialized]
		private ChartSeries m_sourceSeries;

		private ChartSeries m_series;

		private ExpressionInfo m_sourceChartSeriesName;

		private ExpressionInfo m_derivedSeriesFormula;

		private List<ChartFormulaParameter> m_chartFormulaParameters;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		[NonSerialized]
		private ChartDerivedSeriesExprHost m_exprHost;

		internal ChartSeries SourceSeries
		{
			get
			{
				if (m_sourceSeries == null)
				{
					m_sourceSeries = m_chart.ChartSeriesCollection.GetByName(SourceChartSeriesName);
				}
				return m_sourceSeries;
			}
		}

		internal ChartDerivedSeriesExprHost ExprHost => m_exprHost;

		internal int ExpressionHostID => m_exprHostID;

		internal ChartSeries Series
		{
			get
			{
				return m_series;
			}
			set
			{
				m_series = value;
			}
		}

		internal List<ChartFormulaParameter> FormulaParameters
		{
			get
			{
				return m_chartFormulaParameters;
			}
			set
			{
				m_chartFormulaParameters = value;
			}
		}

		internal string SourceChartSeriesName
		{
			get
			{
				if (m_sourceChartSeriesName != null)
				{
					return m_sourceChartSeriesName.StringValue;
				}
				return null;
			}
			set
			{
				if (value != null)
				{
					m_sourceChartSeriesName = ExpressionInfo.CreateConstExpression(value);
				}
				else
				{
					m_sourceChartSeriesName = null;
				}
			}
		}

		internal ChartSeriesFormula DerivedSeriesFormula
		{
			get
			{
				if (m_derivedSeriesFormula != null)
				{
					return EnumTranslator.TranslateChartSeriesFormula(m_derivedSeriesFormula.StringValue);
				}
				return ChartSeriesFormula.BollingerBands;
			}
			set
			{
				m_derivedSeriesFormula = ExpressionInfo.CreateConstExpression(value.ToString());
			}
		}

		internal ChartDerivedSeries()
		{
		}

		internal ChartDerivedSeries(Chart chart)
		{
			m_chart = chart;
		}

		internal void SetExprHost(ChartDerivedSeriesExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			m_exprHost = exprHost;
			m_exprHost.SetReportObjectModel(reportObjectModel);
			if (m_series != null && m_exprHost.ChartSeriesHost != null)
			{
				m_series.SetExprHost(m_exprHost.ChartSeriesHost, reportObjectModel);
			}
			IList<ChartFormulaParameterExprHost> chartFormulaParametersHostsRemotable = m_exprHost.ChartFormulaParametersHostsRemotable;
			if (m_chartFormulaParameters == null || chartFormulaParametersHostsRemotable == null)
			{
				return;
			}
			for (int i = 0; i < m_chartFormulaParameters.Count; i++)
			{
				ChartFormulaParameter chartFormulaParameter = m_chartFormulaParameters[i];
				if (chartFormulaParameter != null && chartFormulaParameter.ExpressionHostID > -1)
				{
					chartFormulaParameter.SetExprHost(chartFormulaParametersHostsRemotable[chartFormulaParameter.ExpressionHostID], reportObjectModel);
				}
			}
		}

		internal void Initialize(InitializationContext context, int index)
		{
			context.ExprHostBuilder.ChartDerivedSeriesStart(index);
			if (m_series != null)
			{
				m_series.Initialize(context, index.ToString(CultureInfo.InvariantCulture));
			}
			if (m_chartFormulaParameters != null)
			{
				for (int i = 0; i < m_chartFormulaParameters.Count; i++)
				{
					m_chartFormulaParameters[i].Initialize(context);
				}
			}
			m_exprHostID = context.ExprHostBuilder.ChartDerivedSeriesEnd();
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			ChartDerivedSeries chartDerivedSeries = (ChartDerivedSeries)MemberwiseClone();
			chartDerivedSeries.m_chart = (Chart)context.CurrentDataRegionClone;
			if (m_series != null)
			{
				chartDerivedSeries.m_series = (ChartSeries)m_series.PublishClone(context);
			}
			if (m_sourceChartSeriesName != null)
			{
				chartDerivedSeries.m_sourceChartSeriesName = (ExpressionInfo)m_sourceChartSeriesName.PublishClone(context);
			}
			if (m_derivedSeriesFormula != null)
			{
				chartDerivedSeries.m_derivedSeriesFormula = (ExpressionInfo)m_derivedSeriesFormula.PublishClone(context);
			}
			if (m_chartFormulaParameters != null)
			{
				chartDerivedSeries.m_chartFormulaParameters = new List<ChartFormulaParameter>(m_chartFormulaParameters.Count);
				{
					foreach (ChartFormulaParameter chartFormulaParameter in m_chartFormulaParameters)
					{
						chartDerivedSeries.m_chartFormulaParameters.Add((ChartFormulaParameter)chartFormulaParameter.PublishClone(context));
					}
					return chartDerivedSeries;
				}
			}
			return chartDerivedSeries;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Series, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartSeries));
			list.Add(new MemberInfo(MemberName.SourceChartSeriesName, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.DerivedSeriesFormula, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ChartFormulaParameters, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartFormulaParameter));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.Chart, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Chart, Token.Reference));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartDerivedSeries, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.ExprHostID:
					writer.Write(m_exprHostID);
					break;
				case MemberName.Chart:
					writer.WriteReference(m_chart);
					break;
				case MemberName.Series:
					writer.Write(m_series);
					break;
				case MemberName.ChartFormulaParameters:
					writer.Write(m_chartFormulaParameters);
					break;
				case MemberName.SourceChartSeriesName:
					writer.Write(m_sourceChartSeriesName);
					break;
				case MemberName.DerivedSeriesFormula:
					writer.Write(m_derivedSeriesFormula);
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
				case MemberName.ExprHostID:
					m_exprHostID = reader.ReadInt32();
					break;
				case MemberName.Chart:
					m_chart = reader.ReadReference<Chart>(this);
					break;
				case MemberName.Series:
					m_series = (ChartSeries)reader.ReadRIFObject();
					break;
				case MemberName.ChartFormulaParameters:
					m_chartFormulaParameters = reader.ReadGenericListOfRIFObjects<ChartFormulaParameter>();
					break;
				case MemberName.SourceChartSeriesName:
					m_sourceChartSeriesName = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.DerivedSeriesFormula:
					m_derivedSeriesFormula = (ExpressionInfo)reader.ReadRIFObject();
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
				MemberName memberName = item.MemberName;
				if (memberName == MemberName.Chart)
				{
					Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
					m_chart = (Chart)referenceableItems[item.RefID];
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartDerivedSeries;
		}
	}
}
