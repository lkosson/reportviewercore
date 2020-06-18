using Microsoft.ReportingServices.DataExtensions;
using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Library;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Security.Permissions;

namespace Microsoft.Reporting
{
	[Serializable]
	internal class LocalExecutionSession
	{
		private ControlSnapshot __ReportSnapshot;

		private ControlSnapshot __compiledReport;

		public ControlSnapshot Snapshot
		{
			[PermissionSet(SecurityAction.Demand, Unrestricted = true)]
			get
			{
				return __ReportSnapshot;
			}
			[PermissionSet(SecurityAction.Demand, Unrestricted = true)]
			set
			{
				__ReportSnapshot = value;
				ExecutionInfo.HasSnapshot = (value != null);
			}
		}

		public ControlSnapshot CompiledReport
		{
			[PermissionSet(SecurityAction.Demand, Unrestricted = true)]
			get
			{
				return __compiledReport;
			}
			[PermissionSet(SecurityAction.Demand, Unrestricted = true)]
			set
			{
				__compiledReport = value;
				ExecutionInfo.IsCompiled = (value != null);
			}
		}

		public EventInformation EventInfo
		{
			get;
			set;
		}

		public LocalExecutionInfo ExecutionInfo
		{
			get;
			private set;
		}

		public DatasourceCredentialsCollection Credentials
		{
			get;
			private set;
		}

		internal DataSourceInfoCollection CompiledDataSources
		{
			get;
			set;
		}

		public LocalExecutionSession()
		{
			ExecutionInfo = new LocalExecutionInfo();
			Credentials = new DatasourceCredentialsCollection();
		}

		public void ResetExecution()
		{
			Snapshot = null;
			EventInfo = null;
			CompiledDataSources = null;
			ExecutionInfo.Reset();
		}

		public void SaveProcessingResult(OnDemandProcessingResult result)
		{
			if (result != null)
			{
				result.Save();
				if (result.EventInfoChanged)
				{
					EventInfo = result.NewEventInfo;
				}
				ExecutionInfo.SaveProcessingResult(result);
			}
		}
	}
}
