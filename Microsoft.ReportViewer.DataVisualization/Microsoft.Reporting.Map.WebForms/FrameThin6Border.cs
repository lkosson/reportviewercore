namespace Microsoft.Reporting.Map.WebForms
{
	internal class FrameThin6Border : FrameThin1Border
	{
		public override string Name => "FrameThin6";

		public FrameThin6Border()
		{
			float[] array = innerCorners = new float[8]
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
