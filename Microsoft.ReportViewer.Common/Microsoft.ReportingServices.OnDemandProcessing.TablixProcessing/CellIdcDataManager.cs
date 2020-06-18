using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.RdlExpressions;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;

namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	internal sealed class CellIdcDataManager : BaseIdcDataManager
	{
		private readonly IntersectJoinInfo m_joinInfo;

		private readonly bool m_shareOuterGroupDataSet;

		private readonly IRIFReportIntersectionScope m_cellScope;

		private readonly Microsoft.ReportingServices.ReportIntermediateFormat.DataSet m_sharedDataSet;

		private RuntimeDataTablixGroupLeafObjReference m_lastOuterGroupLeafRef;

		private RuntimeDataTablixGroupLeafObjReference m_lastInnerGroupLeafRef;

		private readonly Relationship m_activeOuterRelationship;

		private readonly Relationship m_activeInnerRelationship;

		private Microsoft.ReportingServices.RdlExpressions.VariantResult[] m_lastOuterPrimaryKeyValues;

		private Microsoft.ReportingServices.RdlExpressions.VariantResult[] m_lastInnerPrimaryKeyValues;

		public CellIdcDataManager(OnDemandProcessingContext odpContext, IRIFReportDataScope idcReportDataScope)
			: base(odpContext, idcReportDataScope.DataScopeInfo.DataSet)
		{
			m_joinInfo = (idcReportDataScope.DataScopeInfo.JoinInfo as IntersectJoinInfo);
			Global.Tracer.Assert(m_joinInfo != null, "Did not find expected IntersectionJoinInfo");
			m_cellScope = (IRIFReportIntersectionScope)idcReportDataScope;
			if (!m_cellScope.IsColumnOuterGrouping)
			{
				m_activeOuterRelationship = m_joinInfo.GetActiveRowRelationship(m_idcDataSet);
				m_activeInnerRelationship = m_joinInfo.GetActiveColumnRelationship(m_idcDataSet);
				m_sharedDataSet = m_joinInfo.RowParentDataSet;
			}
			else
			{
				m_activeInnerRelationship = m_joinInfo.GetActiveRowRelationship(m_idcDataSet);
				m_activeOuterRelationship = m_joinInfo.GetActiveColumnRelationship(m_idcDataSet);
				m_sharedDataSet = m_joinInfo.ColumnParentDataSet;
			}
			m_shareOuterGroupDataSet = (m_activeOuterRelationship == null);
		}

		public void RegisterActiveIntersection(RuntimeDataTablixGroupLeafObjReference innerGroupLeafRef, RuntimeDataTablixGroupLeafObjReference outerGroupLeafRef)
		{
			if (m_lastOuterGroupLeafRef != outerGroupLeafRef)
			{
				if (m_lastOuterGroupLeafRef != null)
				{
					using (m_lastOuterGroupLeafRef.PinValue())
					{
						m_lastOuterGroupLeafRef.Value().ResetStreamingModeIdcRowBuffer();
					}
				}
				m_lastOuterGroupLeafRef = outerGroupLeafRef;
				if (m_activeOuterRelationship != null)
				{
					m_lastOuterPrimaryKeyValues = EvaluatePrimaryKeyExpressions(m_lastOuterGroupLeafRef, m_activeOuterRelationship);
				}
			}
			if (m_lastInnerGroupLeafRef != innerGroupLeafRef)
			{
				m_lastInnerGroupLeafRef = innerGroupLeafRef;
				m_lastInnerPrimaryKeyValues = EvaluatePrimaryKeyExpressions(m_lastInnerGroupLeafRef, m_activeInnerRelationship);
			}
		}

		private Microsoft.ReportingServices.RdlExpressions.VariantResult[] EvaluatePrimaryKeyExpressions(RuntimeDataTablixGroupLeafObjReference groupLeafRef, Relationship relationship)
		{
			groupLeafRef.Value().DataRows[0].RestoreDataSetAndSetFields(m_odpContext, relationship.RelatedDataSet.DataSetCore.FieldsContext);
			return relationship.EvaluateJoinConditionKeys(evaluatePrimaryKeys: true, m_odpContext.ReportRuntime);
		}

		public override void Advance()
		{
			using (m_lastInnerGroupLeafRef.PinValue())
			{
				using (m_lastOuterGroupLeafRef.PinValue())
				{
					RuntimeDataTablixGroupLeafObj runtimeDataTablixGroupLeafObj = m_lastInnerGroupLeafRef.Value();
					RuntimeDataTablixGroupLeafObj rowGroupLeaf = m_lastOuterGroupLeafRef.Value();
					OnDemandStateManager stateManager = m_odpContext.StateManager;
					ObjectModelImpl reportObjectModel = m_odpContext.ReportObjectModel;
					if (m_idcDataSet.DataSetCore.FieldsContext != null)
					{
						reportObjectModel.RestoreFields(m_idcDataSet.DataSetCore.FieldsContext);
					}
					while (SetupNextRow(m_lastOuterPrimaryKeyValues, m_activeOuterRelationship, m_lastInnerPrimaryKeyValues, m_activeInnerRelationship))
					{
						ApplyGroupingFieldsForServerAggregates(m_cellScope);
						bool rowAccepted = runtimeDataTablixGroupLeafObj.GetOrCreateCell(rowGroupLeaf).NextRow();
						if (stateManager.ShouldStopPipelineAdvance(rowAccepted))
						{
							break;
						}
					}
				}
			}
		}

		private bool SetupNextRow(Microsoft.ReportingServices.RdlExpressions.VariantResult[] rowPrimaryKeys, Relationship rowRelationship, Microsoft.ReportingServices.RdlExpressions.VariantResult[] columnPrimaryKeys, Relationship columnRelationship)
		{
			if (!ReadRowFromDataSet())
			{
				return false;
			}
			bool flag = false;
			if (m_shareOuterGroupDataSet)
			{
				flag = Correlate(columnRelationship, columnPrimaryKeys, advancedRowCursor: true);
			}
			else if (flag = Correlate(rowRelationship, rowPrimaryKeys, advancedRowCursor: true))
			{
				flag &= Correlate(columnRelationship, columnPrimaryKeys, advancedRowCursor: true);
			}
			if (!flag)
			{
				PushBackLastRow();
			}
			return flag;
		}

		protected override void PushBackLastRow()
		{
			if (m_shareOuterGroupDataSet)
			{
				using (m_lastOuterGroupLeafRef.PinValue())
				{
					m_lastOuterGroupLeafRef.Value().PushBackStreamingModeIdcRowToBuffer();
				}
			}
			else
			{
				base.PushBackLastRow();
			}
		}

		protected override bool ReadRowFromDataSet()
		{
			if (m_shareOuterGroupDataSet)
			{
				using (m_lastOuterGroupLeafRef.PinValue())
				{
					return m_lastOuterGroupLeafRef.Value().ReadStreamingModeIdcRowFromBufferOrDataSet(m_sharedDataSet.DataSetCore.FieldsContext);
				}
			}
			return base.ReadRowFromDataSet();
		}

		protected override void SetupRelationshipQueryRestart()
		{
			AddRelationshipRestartPosition(m_activeOuterRelationship, m_lastOuterPrimaryKeyValues);
			AddRelationshipRestartPosition(m_activeInnerRelationship, m_lastInnerPrimaryKeyValues);
		}

		public override void Close()
		{
			base.Close();
			m_lastOuterGroupLeafRef = null;
			m_lastInnerGroupLeafRef = null;
		}
	}
}
