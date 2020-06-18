namespace Microsoft.Reporting.Map.WebForms
{
	internal class FrameThin2Border : FrameThin1Border
	{
		public override string Name => "FrameThin2";

		public FrameThin2Border()
		{
			innerCorners = (cornerRadius = new float[8]
			{
				15f,
				15f,
				15f,
				1f,
				1f,
				1f,
				1f,
				15f
			});
		}
	}
}
