namespace Microsoft.ReportingServices.Diagnostics
{
	internal interface IConfiguration
	{
		IRdlSandboxConfig RdlSandboxing
		{
			get;
		}

		bool ShowSubreportErrorDetails
		{
			get;
		}

		IMapTileServerConfiguration MapTileServerConfiguration
		{
			get;
		}

		ProcessingUpgradeState UpgradeState
		{
			get;
		}

		bool ProhibitSerializableValues
		{
			get;
		}
	}
}
