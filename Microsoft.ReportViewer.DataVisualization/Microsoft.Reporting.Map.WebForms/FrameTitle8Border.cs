using System.Drawing;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class FrameTitle8Border : FrameTitle1Border
	{
		public override string Name => "FrameTitle8";

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
