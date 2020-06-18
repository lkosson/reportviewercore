using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Reporting.WinForms.Internal.Soap.ReportingServices2005.Execution
{
	[Serializable]
	[XmlInclude(typeof(ExecutionInfo2))]
	[XmlInclude(typeof(ExecutionInfo3))]
	[GeneratedCode("wsdl", "2.0.50727.42")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class ExecutionInfo
	{
		private bool hasSnapshotField;

		private bool needsProcessingField;

		private bool allowQueryExecutionField;

		private bool credentialsRequiredField;

		private bool parametersRequiredField;

		private DateTime expirationDateTimeField;

		private DateTime executionDateTimeField;

		private int numPagesField;

		private ReportParameter[] parametersField;

		private DataSourcePrompt[] dataSourcePromptsField;

		private bool hasDocumentMapField;

		private string executionIDField;

		private string reportPathField;

		private string historyIDField;

		private PageSettings reportPageSettingsField;

		private int autoRefreshIntervalField;

		public bool HasSnapshot
		{
			get
			{
				return hasSnapshotField;
			}
			set
			{
				hasSnapshotField = value;
			}
		}

		public bool NeedsProcessing
		{
			get
			{
				return needsProcessingField;
			}
			set
			{
				needsProcessingField = value;
			}
		}

		public bool AllowQueryExecution
		{
			get
			{
				return allowQueryExecutionField;
			}
			set
			{
				allowQueryExecutionField = value;
			}
		}

		public bool CredentialsRequired
		{
			get
			{
				return credentialsRequiredField;
			}
			set
			{
				credentialsRequiredField = value;
			}
		}

		public bool ParametersRequired
		{
			get
			{
				return parametersRequiredField;
			}
			set
			{
				parametersRequiredField = value;
			}
		}

		public DateTime ExpirationDateTime
		{
			get
			{
				return expirationDateTimeField;
			}
			set
			{
				expirationDateTimeField = value;
			}
		}

		public DateTime ExecutionDateTime
		{
			get
			{
				return executionDateTimeField;
			}
			set
			{
				executionDateTimeField = value;
			}
		}

		public int NumPages
		{
			get
			{
				return numPagesField;
			}
			set
			{
				numPagesField = value;
			}
		}

		public ReportParameter[] Parameters
		{
			get
			{
				return parametersField;
			}
			set
			{
				parametersField = value;
			}
		}

		public DataSourcePrompt[] DataSourcePrompts
		{
			get
			{
				return dataSourcePromptsField;
			}
			set
			{
				dataSourcePromptsField = value;
			}
		}

		public bool HasDocumentMap
		{
			get
			{
				return hasDocumentMapField;
			}
			set
			{
				hasDocumentMapField = value;
			}
		}

		public string ExecutionID
		{
			get
			{
				return executionIDField;
			}
			set
			{
				executionIDField = value;
			}
		}

		public string ReportPath
		{
			get
			{
				return reportPathField;
			}
			set
			{
				reportPathField = value;
			}
		}

		public string HistoryID
		{
			get
			{
				return historyIDField;
			}
			set
			{
				historyIDField = value;
			}
		}

		public PageSettings ReportPageSettings
		{
			get
			{
				return reportPageSettingsField;
			}
			set
			{
				reportPageSettingsField = value;
			}
		}

		public int AutoRefreshInterval
		{
			get
			{
				return autoRefreshIntervalField;
			}
			set
			{
				autoRefreshIntervalField = value;
			}
		}
	}
}
