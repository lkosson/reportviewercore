using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal abstract class DataPipelineManager
	{
		protected readonly OnDemandProcessingContext m_odpContext;

		protected readonly Microsoft.ReportingServices.ReportIntermediateFormat.DataSet m_dataSet;

		public abstract IOnDemandScopeInstance GroupTreeRoot
		{
			get;
		}

		protected abstract RuntimeDataSource RuntimeDataSource
		{
			get;
		}

		public int DataSetIndex => m_dataSet.IndexInCollection;

		public DataPipelineManager(OnDemandProcessingContext odpContext, Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet)
		{
			m_odpContext = odpContext;
			m_dataSet = dataSet;
		}

		public void StartProcessing()
		{
			if (!m_odpContext.InSubreport)
			{
				m_odpContext.ExecutionLogContext.StartTablixProcessingTimer();
			}
			bool isTablixProcessingMode = m_odpContext.IsTablixProcessingMode;
			UserProfileState? userProfileState = null;
			try
			{
				m_odpContext.IsTablixProcessingMode = true;
				userProfileState = m_odpContext.ReportObjectModel.UserImpl.UpdateUserProfileLocationWithoutLocking(UserProfileState.InReport);
				InternalStartProcessing();
			}
			finally
			{
				m_odpContext.ReportRuntime.CurrentScope = null;
				m_odpContext.IsTablixProcessingMode = isTablixProcessingMode;
				if (userProfileState.HasValue)
				{
					m_odpContext.ReportObjectModel.UserImpl.UpdateUserProfileLocationWithoutLocking(userProfileState.Value);
				}
				if (!m_odpContext.InSubreport)
				{
					m_odpContext.ExecutionLogContext.StopTablixProcessingTimer();
				}
			}
		}

		protected abstract void InternalStartProcessing();

		public void StopProcessing()
		{
			m_dataSet.ClearDataRegionStreamingScopeInstances();
			if (RuntimeDataSource != null)
			{
				RuntimeDataSource.RecordTimeDataRetrieval();
			}
			InternalStopProcessing();
		}

		protected abstract void InternalStopProcessing();

		public abstract void Advance();

		public virtual void Abort()
		{
		}
	}
}
