using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	[CompilerGenerated]
	internal class SR
	{
		[CompilerGenerated]
		public class Keys
		{
			private static ResourceManager resourceManager = new ResourceManager(typeof(SR).FullName, typeof(SR).Module.Assembly);

			private static CultureInfo _culture = null;

			public const string Language_bn = "Language_bn";

			public const string Language_or = "Language_or";

			public const string Language_lo = "Language_lo";

			public const string Language_bo = "Language_bo";

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

		public static string Language_bn => Keys.GetString("Language_bn");

		public static string Language_or => Keys.GetString("Language_or");

		public static string Language_lo => Keys.GetString("Language_lo");

		public static string Language_bo => Keys.GetString("Language_bo");

		protected SR()
		{
		}
	}
}
