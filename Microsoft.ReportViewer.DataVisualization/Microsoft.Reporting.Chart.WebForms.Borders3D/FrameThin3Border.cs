namespace Microsoft.Reporting.Chart.WebForms.Borders3D
{
	internal class FrameThin3Border : FrameThin1Border
	{
		public override string Name => "FrameThin3";

		public override float Resolution
		{
			set
			{
				base.Resolution = value;
				float num = resolution / 96f;
				innerCorners = (cornerRadius = new float[8]
				{
					num,
					num,
					num,
					num,
					num,
					num,
					num,
					num
				});
			}
		}

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
