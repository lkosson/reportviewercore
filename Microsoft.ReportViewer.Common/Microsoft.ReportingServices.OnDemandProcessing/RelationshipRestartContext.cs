using Microsoft.ReportingServices.DataProcessing;
using Microsoft.ReportingServices.RdlExpressions;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal sealed class RelationshipRestartContext : RestartContext
	{
		private Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo[] m_expressions;

		private Microsoft.ReportingServices.RdlExpressions.VariantResult[] m_values;

		private readonly Microsoft.ReportingServices.ReportIntermediateFormat.DataSet m_idcDataSet;

		private SortDirection[] m_sortDirections;

		public RelationshipRestartContext(Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo[] expressions, Microsoft.ReportingServices.RdlExpressions.VariantResult[] values, SortDirection[] sortDirections, Microsoft.ReportingServices.ReportIntermediateFormat.DataSet idcDataSet)
			: base(RestartMode.Query)
		{
			m_expressions = expressions;
			m_values = values;
			m_idcDataSet = idcDataSet;
			m_sortDirections = sortDirections;
			NormalizeValues(m_values);
		}

		public override RowSkippingControlFlag DoesNotMatchRowRecordField(OnDemandProcessingContext odpContext, Microsoft.ReportingServices.ReportIntermediateFormat.RecordField[] recordFields)
		{
			for (int i = 0; i < m_expressions.Length; i++)
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = m_expressions[i];
				object value = m_values[i].Value;
				Microsoft.ReportingServices.ReportIntermediateFormat.RecordField field = recordFields[expressionInfo.FieldIndex];
				bool isSortedAscending = m_sortDirections[i] == SortDirection.Ascending;
				RowSkippingControlFlag rowSkippingControlFlag = CompareFieldWithScopeValueAndStopOnInequality(odpContext, field, value, isSortedAscending, ObjectType.DataSet, m_idcDataSet.Name, "Relationship.QueryRestart");
				if (rowSkippingControlFlag != 0)
				{
					return rowSkippingControlFlag;
				}
			}
			return RowSkippingControlFlag.ExactMatch;
		}

		private void NormalizeValues(Microsoft.ReportingServices.RdlExpressions.VariantResult[] values)
		{
			for (int i = 0; i < values.Length; i++)
			{
				if (values[i].Value is DBNull)
				{
					values[i].Value = null;
				}
			}
		}

		public override List<ScopeValueFieldName> GetScopeValueFieldNameCollection(Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet)
		{
			List<ScopeValueFieldName> list = new List<ScopeValueFieldName>();
			for (int i = 0; i < m_expressions.Length; i++)
			{
				string dataField = dataSet.Fields[m_expressions[i].FieldIndex].DataField;
				list.Add(new ScopeValueFieldName(dataField, m_values[i].Value));
			}
			return list;
		}

		public override void TraceStartAtRecoveryMessage()
		{
		}
	}
}
