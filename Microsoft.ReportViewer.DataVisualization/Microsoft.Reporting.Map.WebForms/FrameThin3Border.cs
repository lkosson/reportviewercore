namespace Microsoft.Reporting.Map.WebForms
{
	internal class FrameThin3Border : FrameThin1Border
	{
		public override string Name => "FrameThin3";

		public FrameThin3Border()
		{
			innerCorners = (cornerRadius = new float[8]
			{
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f
			});
		}
	}
}
