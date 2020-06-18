using Microsoft.ReportingServices.RdlExpressions;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System;
using System.Diagnostics;

namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	internal abstract class BaseIdcDataManager : IDisposable
	{
		protected OnDemandProcessingContext m_odpContext;

		protected readonly Microsoft.ReportingServices.ReportIntermediateFormat.DataSet m_idcDataSet;

		private readonly bool m_needsServerAggregateTranslation;

		private RuntimeIdcIncrementalDataSource m_dataSource;

		private DataFieldRow m_nextDataFieldRowToProcess;

		private long m_skippedRowCount;

		protected RowSkippingFilter m_skippingFilter;

		protected bool IsDataPipelineSetup => m_dataSource != null;

		public BaseIdcDataManager(OnDemandProcessingContext odpContext, Microsoft.ReportingServices.ReportIntermediateFormat.DataSet idcDataSet)
		{
			Global.Tracer.Assert(!odpContext.InSubreport, "IDC currently cannot be used inside subreports");
			m_odpContext = odpContext;
			m_idcDataSet = idcDataSet;
			m_needsServerAggregateTranslation = m_idcDataSet.HasScopeWithCustomAggregates;
		}

		public abstract void Advance();

		protected void ApplyGroupingFieldsForServerAggregates(IRIFReportDataScope idcReportDataScope)
		{
			if (m_needsServerAggregateTranslation)
			{
				idcReportDataScope.DataScopeInfo.ApplyGroupingFieldsForServerAggregates(m_odpContext.ReportObjectModel.FieldsImpl);
			}
		}

		protected virtual void PushBackLastRow()
		{
			FieldsImpl fieldsImplForUpdate = m_odpContext.ReportObjectModel.GetFieldsImplForUpdate(m_idcDataSet);
			if (fieldsImplForUpdate.IsAggregateRow)
			{
				m_nextDataFieldRowToProcess = new AggregateRow(fieldsImplForUpdate, getAndSave: true);
			}
			else
			{
				m_nextDataFieldRowToProcess = new DataFieldRow(fieldsImplForUpdate, getAndSave: true);
			}
		}

		protected virtual bool ReadRowFromDataSet()
		{
			if (m_nextDataFieldRowToProcess != null)
			{
				m_nextDataFieldRowToProcess.SetFields(m_odpContext.ReportObjectModel.GetFieldsImplForUpdate(m_idcDataSet));
				m_nextDataFieldRowToProcess = null;
			}
			else
			{
				if (m_dataSource == null)
				{
					if (m_odpContext.QueryRestartInfo != null)
					{
						SetupRelationshipQueryRestart();
					}
					m_dataSource = new RuntimeIdcIncrementalDataSource(m_idcDataSet, m_odpContext);
					m_dataSource.Initialize();
				}
				if (!m_dataSource.SetupNextRow())
				{
					return false;
				}
			}
			return true;
		}

		protected bool Correlate(Relationship relationship, Microsoft.ReportingServices.RdlExpressions.VariantResult[] primaryKeys, bool advancedRowCursor)
		{
			SortDirection[] sortDirections = relationship.GetSortDirections();
			bool flag = false;
			bool flag2 = true;
			while (flag2 && !flag)
			{
				Microsoft.ReportingServices.RdlExpressions.VariantResult[] array = relationship.EvaluateJoinConditionKeys(evaluatePrimaryKeys: false, m_odpContext.ReportRuntime);
				flag = true;
				int num = 0;
				while (flag && primaryKeys != null && num < primaryKeys.Length)
				{
					int num2 = m_odpContext.CompareAndStopOnError(primaryKeys[num].Value, array[num].Value, ObjectType.DataSet, m_idcDataSet.Name, "JoinCondition", extendedTypeComparisons: false);
					flag2 = ((sortDirections[num] != 0) ? (num2 <= 0) : (num2 >= 0));
					flag = (flag && num2 == 0);
					num++;
				}
				if (flag2 && flag && m_skippingFilter != null)
				{
					flag2 = m_skippingFilter.ShouldSkipCurrentRow();
					flag = !flag2;
				}
				if (flag2 && !flag)
				{
					if (advancedRowCursor)
					{
						m_skippedRowCount++;
					}
					if (!ReadRowFromDataSet())
					{
						return false;
					}
					advancedRowCursor = true;
				}
			}
			return flag;
		}

		protected abstract void SetupRelationshipQueryRestart();

		protected void AddRelationshipRestartPosition(Relationship relationship, Microsoft.ReportingServices.RdlExpressions.VariantResult[] primaryKeys)
		{
			if (relationship != null)
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo[] foreignKeyExpressions = relationship.GetForeignKeyExpressions();
				if (foreignKeyExpressions != null && primaryKeys != null)
				{
					RelationshipRestartContext relationshipRestart = new RelationshipRestartContext(foreignKeyExpressions, primaryKeys, relationship.GetSortDirections(), m_idcDataSet);
					m_odpContext.QueryRestartInfo.AddRelationshipRestartPosition(m_idcDataSet, relationshipRestart);
				}
			}
		}

		[Conditional("DEBUG")]
		protected void AssertPrimaryKeysMatchForeignKeys(Relationship relationship, Array primaryKeys, Array foreignKeys)
		{
			if (relationship.NaturalJoin && primaryKeys == null)
			{
			}
		}

		public virtual void Close()
		{
			if (m_dataSource != null)
			{
				m_dataSource.RecordTimeDataRetrieval();
				m_dataSource.RecordSkippedRowCount(m_skippedRowCount);
				m_dataSource.Teardown();
				m_dataSource = null;
			}
		}

		public void Dispose()
		{
			Close();
		}
	}
}
