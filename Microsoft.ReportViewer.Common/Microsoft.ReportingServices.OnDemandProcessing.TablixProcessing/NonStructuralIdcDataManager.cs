using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;

namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	internal sealed class NonStructuralIdcDataManager : LinearIdcDataManager
	{
		private readonly IRIFReportDataScope m_sourceDataScope;

		private bool m_lastCorrelationHadMatchingRow;

		private IReference<IOnDemandScopeInstance> m_lastParentScopeInstance;

		internal IRIFReportDataScope SourceDataScope => m_sourceDataScope;

		internal IReference<IOnDemandScopeInstance> LastParentScopeInstance => m_lastParentScopeInstance;

		public NonStructuralIdcDataManager(OnDemandProcessingContext odpContext, Microsoft.ReportingServices.ReportIntermediateFormat.DataSet targetDataSet, IRIFReportDataScope sourceDataScope)
			: base(odpContext, targetDataSet, GetActiveRelationship(targetDataSet, sourceDataScope))
		{
			m_sourceDataScope = sourceDataScope;
		}

		private static Relationship GetActiveRelationship(Microsoft.ReportingServices.ReportIntermediateFormat.DataSet targetDataSet, IRIFReportDataScope sourceDataScope)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet = sourceDataScope.DataScopeInfo.DataSet;
			Relationship defaultRelationship = targetDataSet.GetDefaultRelationship(dataSet);
			Global.Tracer.Assert(defaultRelationship != null, "Could not find active relationship");
			return defaultRelationship;
		}

		protected override void UpdateActiveParent(IReference<IOnDemandScopeInstance> parentScopeInstanceRef)
		{
			m_lastParentScopeInstance = parentScopeInstanceRef;
		}

		public override void Advance()
		{
			_ = m_odpContext.StateManager;
			ObjectModelImpl reportObjectModel = m_odpContext.ReportObjectModel;
			if (m_idcDataSet.DataSetCore.FieldsContext != null)
			{
				reportObjectModel.RestoreFields(m_idcDataSet.DataSetCore.FieldsContext);
			}
			if (SetupCorrelatedRow(startWithCurrentRow: true))
			{
				m_lastCorrelationHadMatchingRow = true;
				return;
			}
			m_lastCorrelationHadMatchingRow = false;
			reportObjectModel.ResetFieldValues();
		}

		public override void Close()
		{
			base.Close();
			m_lastParentScopeInstance = null;
		}

		public void SetupEnvironment()
		{
			m_odpContext.ReportObjectModel.RestoreFields(m_idcDataSet.DataSetCore.FieldsContext);
			if (!m_lastCorrelationHadMatchingRow)
			{
				m_odpContext.ReportObjectModel.ResetFieldValues();
			}
		}
	}
}
