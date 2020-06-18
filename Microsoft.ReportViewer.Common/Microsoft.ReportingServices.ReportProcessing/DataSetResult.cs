namespace Microsoft.ReportingServices.ReportProcessing
{
	internal sealed class DataSetResult
	{
		private ParameterInfoCollection m_parameters;

		private ProcessingMessageList m_warnings;

		private UserProfileState m_usedUserProfileState;

		private bool m_successfulCompletion;

		public ParameterInfoCollection Parameters => m_parameters;

		public ProcessingMessageList Warnings => m_warnings;

		public UserProfileState UsedUserProfileState => m_usedUserProfileState;

		public bool SuccessfulCompletion => m_successfulCompletion;

		public DataSetResult(ParameterInfoCollection finalParameters, ProcessingMessageList warnings, UserProfileState usedUserProfileState, bool successfulCompletion)
		{
			m_parameters = finalParameters;
			m_warnings = warnings;
			m_usedUserProfileState = usedUserProfileState;
			m_successfulCompletion = successfulCompletion;
		}
	}
}
