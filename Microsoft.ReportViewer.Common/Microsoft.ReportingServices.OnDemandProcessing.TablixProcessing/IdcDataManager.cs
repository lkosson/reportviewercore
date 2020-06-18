using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	internal sealed class IdcDataManager : LinearIdcDataManager
	{
		private readonly IRIFReportDataScope m_idcReportDataScope;

		private IReference<IDataCorrelation> m_lastRuntimeReceiver;

		public IdcDataManager(OnDemandProcessingContext odpContext, IRIFReportDataScope idcReportDataScope)
			: base(odpContext, idcReportDataScope.DataScopeInfo.DataSet, GetActiveRelationship(idcReportDataScope))
		{
			m_idcReportDataScope = idcReportDataScope;
		}

		private static Relationship GetActiveRelationship(IRIFReportDataScope idcReportDataScope)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet = idcReportDataScope.DataScopeInfo.DataSet;
			LinearJoinInfo linearJoinInfo = idcReportDataScope.DataScopeInfo.JoinInfo as LinearJoinInfo;
			Global.Tracer.Assert(linearJoinInfo != null, "Did not find expected LinearJoinInfo");
			Relationship activeRelationship = linearJoinInfo.GetActiveRelationship(dataSet);
			Global.Tracer.Assert(activeRelationship != null, "Could not find active relationship");
			return activeRelationship;
		}

		internal void SetSkippingFilter(List<Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo> expressions, List<object> values)
		{
			m_skippingFilter = new RowSkippingFilter(m_odpContext, m_idcReportDataScope, expressions, values);
		}

		internal void ClearSkippingFilter()
		{
			m_skippingFilter = null;
		}

		protected override void UpdateActiveParent(IReference<IOnDemandScopeInstance> parentScopeInstanceRef)
		{
			m_lastRuntimeReceiver = parentScopeInstanceRef.Value().GetIdcReceiver(m_idcReportDataScope);
		}

		public override void Advance()
		{
			OnDemandStateManager stateManager = m_odpContext.StateManager;
			ObjectModelImpl reportObjectModel = m_odpContext.ReportObjectModel;
			if (m_idcDataSet.DataSetCore.FieldsContext != null)
			{
				reportObjectModel.RestoreFields(m_idcDataSet.DataSetCore.FieldsContext);
			}
			using (m_lastRuntimeReceiver.PinValue())
			{
				IDataCorrelation dataCorrelation = m_lastRuntimeReceiver.Value();
				while (SetupCorrelatedRow(startWithCurrentRow: false))
				{
					ApplyGroupingFieldsForServerAggregates(m_idcReportDataScope);
					bool rowAccepted = dataCorrelation.NextCorrelatedRow();
					if (stateManager.ShouldStopPipelineAdvance(rowAccepted))
					{
						break;
					}
				}
			}
		}

		public override void Close()
		{
			base.Close();
			m_lastRuntimeReceiver = null;
		}
	}
}
