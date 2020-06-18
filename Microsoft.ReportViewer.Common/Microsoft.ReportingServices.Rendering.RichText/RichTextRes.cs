using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.ReportingServices.Rendering.RichText
{
	[CompilerGenerated]
	internal class RichTextRes
	{
		[CompilerGenerated]
		public class Keys
		{
			private static ResourceManager resourceManager = new ResourceManager(typeof(RichTextRes).FullName, typeof(RichTextRes).Module.Assembly);

			private static CultureInfo _culture = null;

			public const string Win32ErrorInfo = "Win32ErrorInfo";

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

		public static string Win32ErrorInfo => Keys.GetString("Win32ErrorInfo");

		protected RichTextRes()
		{
		}
	}
}
