using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.Reporting.WinForms
{
	[CompilerGenerated]
	internal class ImageRendererRes
	{
		[CompilerGenerated]
		public class Keys
		{
			private static ResourceManager resourceManager = new ResourceManager(typeof(ImageRendererRes).FullName, typeof(ImageRendererRes).Module.Assembly);

			private static CultureInfo _culture = null;

			public const string IMAGELocalizedName = "IMAGELocalizedName";

			public const string PDFLocalizedName = "PDFLocalizedName";

			public const string RGDILocalizedName = "RGDILocalizedName";

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

		public static string IMAGELocalizedName => Keys.GetString("IMAGELocalizedName");

		public static string PDFLocalizedName => Keys.GetString("PDFLocalizedName");

		public static string RGDILocalizedName => Keys.GetString("RGDILocalizedName");

		public static string Win32ErrorInfo => Keys.GetString("Win32ErrorInfo");

		protected ImageRendererRes()
		{
		}
	}
}
