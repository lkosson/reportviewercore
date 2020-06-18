using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal sealed class DataAggregateObj : IErrorContext
	{
		private bool m_nonAggregateMode;

		private string m_name;

		private StringList m_duplicateNames;

		private DataAggregate m_aggregator;

		private DataAggregateInfo m_aggregateDef;

		private ReportRuntime m_reportRT;

		private bool m_usedInExpression;

		private DataAggregateObjResult m_aggregateResult;

		internal string Name => m_name;

		internal StringList DuplicateNames => m_duplicateNames;

		internal bool NonAggregateMode => m_nonAggregateMode;

		internal DataAggregateInfo AggregateDef => m_aggregateDef;

		internal bool UsedInExpression
		{
			get
			{
				return m_usedInExpression;
			}
			set
			{
				m_usedInExpression = value;
			}
		}

		internal DataAggregateObj(DataAggregateInfo aggInfo, ReportProcessing.ProcessingContext processingContext)
		{
			m_nonAggregateMode = false;
			m_name = aggInfo.Name;
			m_duplicateNames = aggInfo.DuplicateNames;
			switch (aggInfo.AggregateType)
			{
			case DataAggregateInfo.AggregateTypes.First:
				m_aggregator = new First();
				break;
			case DataAggregateInfo.AggregateTypes.Last:
				m_aggregator = new Last();
				break;
			case DataAggregateInfo.AggregateTypes.Sum:
				m_aggregator = new Sum();
				break;
			case DataAggregateInfo.AggregateTypes.Avg:
				m_aggregator = new Avg();
				break;
			case DataAggregateInfo.AggregateTypes.Max:
				m_aggregator = new Max(processingContext.CompareInfo, processingContext.ClrCompareOptions);
				break;
			case DataAggregateInfo.AggregateTypes.Min:
				m_aggregator = new Min(processingContext.CompareInfo, processingContext.ClrCompareOptions);
				break;
			case DataAggregateInfo.AggregateTypes.CountDistinct:
				m_aggregator = new CountDistinct();
				break;
			case DataAggregateInfo.AggregateTypes.CountRows:
				m_aggregator = new CountRows();
				break;
			case DataAggregateInfo.AggregateTypes.Count:
				m_aggregator = new Count();
				break;
			case DataAggregateInfo.AggregateTypes.StDev:
				m_aggregator = new StDev();
				break;
			case DataAggregateInfo.AggregateTypes.Var:
				m_aggregator = new Var();
				break;
			case DataAggregateInfo.AggregateTypes.StDevP:
				m_aggregator = new StDevP();
				break;
			case DataAggregateInfo.AggregateTypes.VarP:
				m_aggregator = new VarP();
				break;
			case DataAggregateInfo.AggregateTypes.Aggregate:
				m_aggregator = new Aggregate();
				break;
			case DataAggregateInfo.AggregateTypes.Previous:
				m_aggregator = new Previous();
				break;
			default:
				Global.Tracer.Assert(condition: false, "Unsupport aggregate type.");
				break;
			}
			m_aggregateDef = aggInfo;
			m_reportRT = processingContext.ReportRuntime;
			if (m_reportRT.ReportExprHost != null)
			{
				m_aggregateDef.SetExprHosts(m_reportRT.ReportExprHost, processingContext.ReportObjectModel);
			}
			m_aggregateResult = default(DataAggregateObjResult);
			Init();
		}

		internal DataAggregateObj(DataAggregateInfo aggrDef, DataAggregateObjResult aggrResult)
		{
			m_nonAggregateMode = true;
			m_aggregateDef = aggrDef;
			m_aggregateResult = aggrResult;
		}

		internal void Init()
		{
			if (!m_nonAggregateMode)
			{
				m_aggregator.Init();
				m_aggregateResult = default(DataAggregateObjResult);
			}
		}

		internal void Update()
		{
			if (m_aggregateResult.ErrorOccurred || m_nonAggregateMode)
			{
				return;
			}
			if (m_aggregateDef.FieldsUsedInValueExpression == null)
			{
				m_reportRT.ReportObjectModel.FieldsImpl.ResetUsedInExpression();
			}
			m_aggregateResult.ErrorOccurred = EvaluateParameters(out object[] values, out DataFieldStatus fieldStatus);
			if (fieldStatus != 0)
			{
				m_aggregateResult.HasCode = true;
				m_aggregateResult.FieldStatus = fieldStatus;
			}
			if (m_aggregateDef.FieldsUsedInValueExpression == null)
			{
				m_aggregateDef.FieldsUsedInValueExpression = new List<string>();
				m_reportRT.ReportObjectModel.FieldsImpl.AddFieldsUsedInExpression(m_aggregateDef.FieldsUsedInValueExpression);
			}
			if (!m_aggregateResult.ErrorOccurred)
			{
				try
				{
					m_aggregator.Update(values, this);
				}
				catch (ReportProcessingException)
				{
					m_aggregateResult.ErrorOccurred = true;
				}
			}
		}

		internal DataAggregateObjResult AggregateResult()
		{
			if (!m_nonAggregateMode && !m_aggregateResult.ErrorOccurred)
			{
				try
				{
					m_aggregateResult.Value = m_aggregator.Result();
				}
				catch (ReportProcessingException)
				{
					m_aggregateResult.ErrorOccurred = true;
					m_aggregateResult.Value = null;
				}
			}
			return m_aggregateResult;
		}

		internal bool EvaluateParameters(out object[] values, out DataFieldStatus fieldStatus)
		{
			bool flag = false;
			fieldStatus = DataFieldStatus.None;
			values = new object[m_aggregateDef.Expressions.Length];
			for (int i = 0; i < m_aggregateDef.Expressions.Length; i++)
			{
				VariantResult variantResult = m_reportRT.EvaluateAggregateVariantOrBinaryParamExpr(m_aggregateDef, i, this);
				values[i] = variantResult.Value;
				flag |= variantResult.ErrorOccurred;
				if (variantResult.FieldStatus != 0)
				{
					fieldStatus = variantResult.FieldStatus;
				}
			}
			return flag;
		}

		internal void Set(DataAggregateObjResult aggregateResult)
		{
			m_nonAggregateMode = true;
			m_aggregateResult = aggregateResult;
		}

		void IErrorContext.Register(ProcessingErrorCode code, Severity severity, params string[] arguments)
		{
			if (!m_aggregateResult.HasCode)
			{
				m_aggregateResult.HasCode = true;
				m_aggregateResult.Code = code;
				m_aggregateResult.Severity = severity;
				m_aggregateResult.Arguments = arguments;
			}
		}

		void IErrorContext.Register(ProcessingErrorCode code, Severity severity, ObjectType objectType, string objectName, string propertyName, params string[] arguments)
		{
			if (!m_aggregateResult.HasCode)
			{
				m_aggregateResult.HasCode = true;
				m_aggregateResult.Code = code;
				m_aggregateResult.Severity = severity;
				m_aggregateResult.Arguments = arguments;
			}
		}
	}
}
