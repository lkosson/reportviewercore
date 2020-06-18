namespace Microsoft.Reporting.Map.WebForms
{
	internal class FrameThin4Border : FrameThin1Border
	{
		public override string Name => "FrameThin4";

		public FrameThin4Border()
		{
			float[] array = cornerRadius = new float[8]
			{
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f
			};
		}
	}
}
