namespace Microsoft.Reporting.Map.WebForms
{
	internal struct HSV
	{
		internal int Hue;

		internal int Saturation;

		internal int value;

		internal HSV(int H, int S, int V)
		{
			Hue = H;
			Saturation = S;
			value = V;
		}
	}
}
