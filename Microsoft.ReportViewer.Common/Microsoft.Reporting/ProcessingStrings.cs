using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.Reporting
{
	[CompilerGenerated]
	internal class ProcessingStrings
	{
		[CompilerGenerated]
		public class Keys
		{
			private static ResourceManager resourceManager = new ResourceManager(typeof(ProcessingStrings).FullName, typeof(ProcessingStrings).Module.Assembly);

			private static CultureInfo _culture = null;

			public const string MissingDefinition = "MissingDefinition";

			public const string pvInvalidDefinition = "pvInvalidDefinition";

			public const string MainReport = "MainReport";

			public const string RdlCompile_CouldNotWriteStateFile = "RdlCompile_CouldNotWriteStateFile";

			public const string RdlCompile_CouldNotOpenFile = "RdlCompile_CouldNotOpenFile";

			public const string DataSetExtensionName = "DataSetExtensionName";

			public const string MissingDataReader = "MissingDataReader";

			public const string CasPolicyUnavailableForCurrentAppDomain = "CasPolicyUnavailableForCurrentAppDomain";

			public const string MapTileServerConfiguration_MaxConnectionsOutOfRange = "MapTileServerConfiguration_MaxConnectionsOutOfRange";

			public const string MapTileServerConfiguration_TimeoutOutOfRange = "MapTileServerConfiguration_TimeoutOutOfRange";

			public static CultureInfo Culture
			{
				get
				{
					return _culture;
				}
				set
				{
					_culture = value;
				}
			}

			private Keys()
			{
			}

			public static string GetString(string key)
			{
				return resourceManager.GetString(key, _culture);
			}

			public static string GetString(string key, object arg0)
			{
				return string.Format(CultureInfo.CurrentCulture, resourceManager.GetString(key, _culture), arg0);
			}

			public static string GetString(string key, object arg0, object arg1)
			{
				return string.Format(CultureInfo.CurrentCulture, resourceManager.GetString(key, _culture), arg0, arg1);
			}
		}

		public static CultureInfo Culture
		{
			get
			{
				return Keys.Culture;
			}
			set
			{
				Keys.Culture = value;
			}
		}

		public static string MainReport => Keys.GetString("MainReport");

		public static string RdlCompile_CouldNotWriteStateFile => Keys.GetString("RdlCompile_CouldNotWriteStateFile");

		public static string RdlCompile_CouldNotOpenFile => Keys.GetString("RdlCompile_CouldNotOpenFile");

		public static string DataSetExtensionName => Keys.GetString("DataSetExtensionName");

		public static string MissingDataReader => Keys.GetString("MissingDataReader");

		public static string CasPolicyUnavailableForCurrentAppDomain => Keys.GetString("CasPolicyUnavailableForCurrentAppDomain");

		protected ProcessingStrings()
		{
		}

		public static string MissingDefinition(string reportName)
		{
			return Keys.GetString("MissingDefinition", reportName);
		}

		public static string pvInvalidDefinition(string reportPath)
		{
			return Keys.GetString("pvInvalidDefinition", reportPath);
		}

		public static string MapTileServerConfiguration_MaxConnectionsOutOfRange(int minValue, int maxValue)
		{
			return Keys.GetString("MapTileServerConfiguration_MaxConnectionsOutOfRange", minValue, maxValue);
		}

		public static string MapTileServerConfiguration_TimeoutOutOfRange(int minValue, int maxValue)
		{
			return Keys.GetString("MapTileServerConfiguration_TimeoutOutOfRange", minValue, maxValue);
		}
	}
}
