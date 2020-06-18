using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.ReportingServices.Rendering.HPBProcessing
{
	[CompilerGenerated]
	internal class HPBRes
	{
		[CompilerGenerated]
		public class Keys
		{
			private static ResourceManager resourceManager = new ResourceManager(typeof(HPBRes).FullName, typeof(HPBRes).Module.Assembly);

			private static CultureInfo _culture = null;

			public const string RenderSubreportError = "RenderSubreportError";

			public const string ReportItemCannotBeFound = "ReportItemCannotBeFound";

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

		protected HPBRes()
		{
		}

		public static string ReportItemCannotBeFound(string name)
		{
			return Keys.GetString("ReportItemCannotBeFound", name);
		}
	}
}
