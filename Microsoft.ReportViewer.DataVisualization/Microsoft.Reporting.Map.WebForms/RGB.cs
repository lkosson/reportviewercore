using System;

namespace Microsoft.Reporting.Map.WebForms
{
	internal struct RGB
	{
		internal int Red;

		internal int Green;

		internal int Blue;

		internal RGB(int R, int G, int B)
		{
			Red = Math.Max(Math.Min(R, 255), 0);
			Green = Math.Max(Math.Min(G, 255), 0);
			Blue = Math.Max(Math.Min(B, 255), 0);
		}
	}
}
