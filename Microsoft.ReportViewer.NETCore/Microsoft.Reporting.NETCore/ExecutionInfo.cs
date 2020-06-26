using System;

namespace Microsoft.Reporting.NETCore
{
	internal sealed class ExecutionInfo
	{
		public string ExecutionID
		{
			get;
			private set;
		}

		public string HistoryID
		{
			get;
			private set;
		}

		public string ReportPath
		{
			get;
			private set;
		}

		public int NumPages
		{
			get;
			set;
		}

		public bool HasDocumentMap
		{
			get;
			private set;
		}

		public int AutoRefreshInterval
		{
			get;
			private set;
		}

		public bool CredentialsRequired
		{
			get;
			private set;
		}

		public bool ParametersRequired
		{
			get;
			private set;
		}

		public bool HasSnapshot
		{
			get;
			private set;
		}

		public bool NeedsProcessing
		{
			get;
			private set;
		}

		public DateTime ExpirationDateTime
		{
			get;
			private set;
		}

		public bool AllowQueryExecution
		{
			get;
			private set;
		}

		public PageCountMode PageCountMode
		{
			get;
			private set;
		}

		public ReportDataSourceInfoCollection DataSourcePrompts
		{
			get;
			private set;
		}

		public ReportParameterInfoCollection Parameters
		{
			get;
			private set;
		}

		public ReportPageSettings ReportPageSettings
		{
			get;
			private set;
		}

		public ParametersPaneLayout ParametersPaneLayout
		{
			get;
			private set;
		}

		public ExecutionInfo(string executionId, string historyId, string reportPath, int numPages, bool hasDocumentMap, int autoRefreshInterval, bool credentialsRequired, bool parametersRequired, bool hasSnapshot, bool needsProcessing, DateTime expirationDateTime, bool allowQueryExecution, PageCountMode pageCountMode, ReportDataSourceInfoCollection dataSourcePrompts, ReportParameterInfoCollection parameters, ReportPageSettings pageSettings, ParametersPaneLayout parametersPaneLayout)
		{
			ExecutionID = executionId;
			HistoryID = historyId;
			ReportPath = reportPath;
			NumPages = numPages;
			HasDocumentMap = hasDocumentMap;
			AutoRefreshInterval = autoRefreshInterval;
			CredentialsRequired = credentialsRequired;
			ParametersRequired = parametersRequired;
			HasSnapshot = hasSnapshot;
			NeedsProcessing = needsProcessing;
			ExpirationDateTime = expirationDateTime;
			AllowQueryExecution = allowQueryExecution;
			PageCountMode = pageCountMode;
			DataSourcePrompts = dataSourcePrompts;
			Parameters = parameters;
			ReportPageSettings = pageSettings;
			ParametersPaneLayout = parametersPaneLayout;
		}
	}
}
