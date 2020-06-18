namespace Microsoft.Reporting.Chart.WebForms.Borders3D
{
	internal class FrameThin2Border : FrameThin1Border
	{
		public override string Name => "FrameThin2";

		public override float Resolution
		{
			set
			{
				base.Resolution = value;
				float num = 15f * resolution / 96f;
				float num2 = 1f * resolution / 96f;
				innerCorners = (cornerRadius = new float[8]
				{
					num,
					num,
					num,
					num2,
					num2,
					num2,
					num2,
					num
				});
			}
		}

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
