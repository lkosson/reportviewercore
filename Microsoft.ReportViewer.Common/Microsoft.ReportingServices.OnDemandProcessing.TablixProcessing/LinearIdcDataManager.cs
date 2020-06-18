using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.RdlExpressions;
using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	internal abstract class LinearIdcDataManager : BaseIdcDataManager
	{
		private readonly Relationship m_activeRelationship;

		private VariantResult[] m_lastPrimaryKeyValues;

		public LinearIdcDataManager(OnDemandProcessingContext odpContext, DataSet idcDataSet, Relationship activeRelationship)
			: base(odpContext, idcDataSet)
		{
			m_activeRelationship = activeRelationship;
		}

		public void RegisterActiveParent(IReference<IOnDemandScopeInstance> parentScopeInstanceRef)
		{
			using (parentScopeInstanceRef.PinValue())
			{
				parentScopeInstanceRef.Value().SetupEnvironment();
				m_lastPrimaryKeyValues = m_activeRelationship.EvaluateJoinConditionKeys(evaluatePrimaryKeys: true, m_odpContext.ReportRuntime);
				UpdateActiveParent(parentScopeInstanceRef);
			}
		}

		protected abstract void UpdateActiveParent(IReference<IOnDemandScopeInstance> parentScopeInstanceRef);

		protected override void SetupRelationshipQueryRestart()
		{
			AddRelationshipRestartPosition(m_activeRelationship, m_lastPrimaryKeyValues);
		}

		protected bool SetupCorrelatedRow(bool startWithCurrentRow)
		{
			bool advancedRowCursor = false;
			if (!startWithCurrentRow || !base.IsDataPipelineSetup)
			{
				if (!ReadRowFromDataSet())
				{
					return false;
				}
				advancedRowCursor = true;
			}
			bool num = Correlate(m_activeRelationship, m_lastPrimaryKeyValues, advancedRowCursor);
			if (!num)
			{
				PushBackLastRow();
			}
			return num;
		}
	}
}
