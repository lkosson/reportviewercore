using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.SqlServer.ReportingServices
{
	[CompilerGenerated]
	internal class SoapExceptionStrings
	{
		[CompilerGenerated]
		public class Keys
		{
			private static ResourceManager resourceManager = new ResourceManager(typeof(SoapExceptionStrings).FullName, typeof(SoapExceptionStrings).Module.Assembly);

			private static CultureInfo _culture = null;

			public const string MissingEndpoint = "MissingEndpoint";

			public const string VersionMismatch = "VersionMismatch";

			public const string RSSoapMessageFormat = "RSSoapMessageFormat";

			public const string SOAPProxySource = "SOAPProxySource";

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

		public static string MissingEndpoint => Keys.GetString("MissingEndpoint");

		public static string VersionMismatch => Keys.GetString("VersionMismatch");

		public static string SOAPProxySource => Keys.GetString("SOAPProxySource");

		protected SoapExceptionStrings()
		{
		}

		public static string RSSoapMessageFormat(string message, string errorCode)
		{
			return Keys.GetString("RSSoapMessageFormat", message, errorCode);
		}
	}
}
