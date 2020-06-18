using System.Drawing;

namespace Microsoft.Reporting.Chart.WebForms.Borders3D
{
	internal class FrameTitle8Border : FrameTitle1Border
	{
		public override string Name => "FrameTitle8";

		public override float Resolution
		{
			set
			{
				base.Resolution = value;
				sizeLeftTop = new SizeF(0f, sizeLeftTop.Height);
				sizeRightBottom = new SizeF(0f, sizeRightBottom.Height);
				float num = 1f * resolution / 96f;
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

		public FrameTitle8Border()
		{
			sizeLeftTop = new SizeF(0f, sizeLeftTop.Height);
			sizeRightBottom = new SizeF(0f, sizeRightBottom.Height);
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
