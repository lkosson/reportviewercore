using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.ReportingServices.Rendering.RPLRendering
{
	[CompilerGenerated]
	internal class RenderRes
	{
		[CompilerGenerated]
		public class Keys
		{
			private static ResourceManager resourceManager = new ResourceManager(typeof(RenderRes).FullName, typeof(RenderRes).Module.Assembly);

			private static CultureInfo _culture = null;

			public const string RPLLocalizedName = "RPLLocalizedName";

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

		public static string RPLLocalizedName => Keys.GetString("RPLLocalizedName");

		protected RenderRes()
		{
		}
	}
}
