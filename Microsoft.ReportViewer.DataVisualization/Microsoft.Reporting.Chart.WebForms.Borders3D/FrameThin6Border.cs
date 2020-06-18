namespace Microsoft.Reporting.Chart.WebForms.Borders3D
{
	internal class FrameThin6Border : FrameThin1Border
	{
		public override string Name => "FrameThin6";

		public override float Resolution
		{
			set
			{
				base.Resolution = value;
				float num = resolution / 96f;
				float[] array = innerCorners = new float[8]
				{
					num,
					num,
					num,
					num,
					num,
					num,
					num,
					num
				};
			}
		}

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
