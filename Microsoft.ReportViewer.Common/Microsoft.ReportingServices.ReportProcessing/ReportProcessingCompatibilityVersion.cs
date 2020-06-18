using Microsoft.ReportingServices.Diagnostics;
using System;
using System.Diagnostics;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal static class ReportProcessingCompatibilityVersion
	{
		public const int SQL11CTP3 = 2;

		public const int SQL11RC0 = 100;

		public const int O15CTP3 = 200;

		public const int SQL16CTP3 = 300;

		public const int SQL16RC1 = 400;

		public const int UndefinedVersion = 0;

		public static readonly int CompatVersion = 300;

		public static readonly int CurrentVersion = 400;

		public static int GetCompatibilityVersion(IConfiguration configuration)
		{
			if (configuration == null)
			{
				return CurrentVersion;
			}
			return TranslateToVersionNumber(configuration.UpgradeState);
		}

		public static int TranslateToVersionNumber(ProcessingUpgradeState upgradeState)
		{
			switch (upgradeState)
			{
			case ProcessingUpgradeState.CurrentVersion:
				return CurrentVersion;
			case ProcessingUpgradeState.PreviousVersionCompat:
				return CompatVersion;
			default:
				Global.Tracer.Assert(condition: false, "Invalid ProcessingUpgradeState");
				throw new InvalidOperationException("Invalid ProcessingUpgradeState");
			}
		}

		public static void TraceCompatibilityVersion(IConfiguration configuration)
		{
			if (Global.Tracer.TraceVerbose)
			{
				int compatibilityVersion = GetCompatibilityVersion(configuration);
				Global.Tracer.Trace(TraceLevel.Verbose, "Processing compatibility version information: ProcessingUpgradeState: {0} Active Compat Version Number: {1}", (configuration == null) ? "<null>" : configuration.UpgradeState.ToString(), compatibilityVersion);
			}
		}
	}
}
