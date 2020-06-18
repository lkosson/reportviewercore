using System.Drawing;

namespace Microsoft.Reporting.Chart.WebForms.Borders3D
{
	internal class FrameTitle7Border : FrameTitle1Border
	{
		public override string Name => "FrameTitle7";

		public override float Resolution
		{
			set
			{
				base.Resolution = value;
				sizeRightBottom = new SizeF(0f, sizeRightBottom.Height);
				float num = 15f * resolution / 96f;
				float num2 = 1f * resolution / 96f;
				float[] array = innerCorners = new float[8]
				{
					num,
					num2,
					num2,
					num2,
					num2,
					num,
					num,
					num
				};
			}
		}

		public FrameTitle7Border()
		{
			sizeRightBottom = new SizeF(0f, sizeRightBottom.Height);
			float[] array = innerCorners = new float[8]
			{
				15f,
				1f,
				1f,
				1f,
				1f,
				15f,
				15f,
				15f
			};
		}
	}
}
