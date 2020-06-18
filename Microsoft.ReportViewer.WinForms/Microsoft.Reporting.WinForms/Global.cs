namespace Microsoft.Reporting.WinForms
{
	internal static class Global
	{
		public static int ToPixels(double inMM, double dpi)
		{
			return (int)(inMM * dpi / 25.4);
		}

		public static float ToMillimeters(int pixels, double dpi)
		{
			return (float)((double)pixels * 25.4 / dpi);
		}

		public static float ToMillimeters(float pixels, double dpi)
		{
			return (float)((double)pixels * 25.4 / dpi);
		}

		public static int InchToPixels(float value, float dpi)
		{
			return (int)(value * dpi);
		}
	}
}
