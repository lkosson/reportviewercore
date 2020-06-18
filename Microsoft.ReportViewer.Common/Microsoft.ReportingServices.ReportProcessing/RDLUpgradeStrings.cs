using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[CompilerGenerated]
	internal class RDLUpgradeStrings
	{
		[CompilerGenerated]
		public class Keys
		{
			private static ResourceManager resourceManager = new ResourceManager(typeof(RDLUpgradeStrings).FullName, typeof(RDLUpgradeStrings).Module.Assembly);

			private static CultureInfo _culture = null;

			public const string rdlInvalidTargetNamespace = "rdlInvalidTargetNamespace";

			public const string rdlInvalidXmlContents = "rdlInvalidXmlContents";

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

		protected RDLUpgradeStrings()
		{
		}

		public static string rdlInvalidTargetNamespace(string @namespace)
		{
			return Keys.GetString("rdlInvalidTargetNamespace", @namespace);
		}

		public static string rdlInvalidXmlContents(string innerMessage)
		{
			return Keys.GetString("rdlInvalidXmlContents", innerMessage);
		}
	}
}
