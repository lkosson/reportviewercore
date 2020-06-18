using System.Drawing;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class FrameTitle7Border : FrameTitle1Border
	{
		public override string Name => "FrameTitle7";

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
