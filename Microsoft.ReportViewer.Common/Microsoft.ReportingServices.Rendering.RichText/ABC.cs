namespace Microsoft.ReportingServices.Rendering.RichText
{
	internal struct ABC
	{
		internal int abcA;

		internal uint abcB;

		internal int abcC;

		internal int Width => (int)(abcA + abcB + abcC);

		internal void SetToZeroWidth()
		{
			abcA = 0;
			abcB = 0u;
			abcC = 0;
		}
	}
}
