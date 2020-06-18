namespace Microsoft.Reporting.Chart.WebForms.Borders3D
{
	internal class FrameThin4Border : FrameThin1Border
	{
		public override string Name => "FrameThin4";

		public override float Resolution
		{
			set
			{
				base.Resolution = value;
				float num = 1f * resolution / 96f;
				cornerRadius = new float[8]
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
