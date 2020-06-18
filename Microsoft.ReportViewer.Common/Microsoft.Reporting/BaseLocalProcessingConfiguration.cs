using Microsoft.ReportingServices.Diagnostics;

namespace Microsoft.Reporting
{
	internal abstract class BaseLocalProcessingConfiguration : IConfiguration
	{
		private bool m_showSubReportErrorDetails;

		public bool ShowSubreportErrorDetails
		{
			get
			{
				return m_showSubReportErrorDetails;
			}
			set
			{
				m_showSubReportErrorDetails = value;
			}
		}

		public IRdlSandboxConfig RdlSandboxing => null;

		public abstract IMapTileServerConfiguration MapTileServerConfiguration
		{
			get;
		}

		public ProcessingUpgradeState UpgradeState => ProcessingUpgradeState.CurrentVersion;

		public bool ProhibitSerializableValues => false;
	}
}
