using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.ReportingServices.Rendering.SPBProcessing
{
	[CompilerGenerated]
	internal class SPBRes
	{
		[CompilerGenerated]
		public class Keys
		{
			private static ResourceManager resourceManager = new ResourceManager(typeof(SPBRes).FullName, typeof(SPBRes).Module.Assembly);

			private static CultureInfo _culture = null;

			public const string RenderSubreportError = "RenderSubreportError";

			public const string InvalidPaginationStream = "InvalidPaginationStream";

			public const string InvalidTokenPaginationProperties = "InvalidTokenPaginationProperties";

			public const string InvalidTokenPaginationItems = "InvalidTokenPaginationItems";

			public const string UnsupportedRPLVersion = "UnsupportedRPLVersion";

			public const string InvalidStartPageNumber = "InvalidStartPageNumber";

			public const string InvalidEndPageNumber = "InvalidEndPageNumber";

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

		public static string RenderSubreportError => Keys.GetString("RenderSubreportError");

		public static string InvalidPaginationStream => Keys.GetString("InvalidPaginationStream");

		public static string InvalidStartPageNumber => Keys.GetString("InvalidStartPageNumber");

		public static string InvalidEndPageNumber => Keys.GetString("InvalidEndPageNumber");

		protected SPBRes()
		{
		}

		public static string InvalidTokenPaginationProperties(string hexToken)
		{
			return Keys.GetString("InvalidTokenPaginationProperties", hexToken);
		}

		public static string InvalidTokenPaginationItems(string hexToken)
		{
			return Keys.GetString("InvalidTokenPaginationItems", hexToken);
		}

		public static string UnsupportedRPLVersion(string requestedVersion)
		{
			return Keys.GetString("UnsupportedRPLVersion", requestedVersion);
		}
	}
}
