using System.Drawing;

namespace Microsoft.Reporting.Chart.WebForms.Borders3D
{
	internal class FrameThin1Border : RaisedBorder
	{
		protected float[] innerCorners = new float[8]
		{
			15f,
			15f,
			15f,
			15f,
			15f,
			15f,
			15f,
			15f
		};

		public override string Name => "FrameThin1";

		public override float Resolution
		{
			set
			{
				base.Resolution = value;
				float num = 15f * resolution / 96f;
				innerCorners = new float[8]
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
				sizeLeftTop = new SizeF(defaultRadiusSize * 0.8f, defaultRadiusSize * 0.8f);
				sizeRightBottom = new SizeF(defaultRadiusSize * 0.8f, defaultRadiusSize * 0.8f);
			}
		}

		public FrameThin1Border()
		{
			sizeLeftTop = new SizeF(defaultRadiusSize * 0.8f, defaultRadiusSize * 0.8f);
			sizeRightBottom = new SizeF(defaultRadiusSize * 0.8f, defaultRadiusSize * 0.8f);
		}

		public override void DrawBorder(ChartGraphics graph, BorderSkinAttributes borderSkin, RectangleF rect, Color backColor, ChartHatchStyle backHatchStyle, string backImage, ChartImageWrapMode backImageMode, Color backImageTranspColor, ChartImageAlign backImageAlign, GradientType backGradientType, Color backGradientEndColor, Color borderColor, int borderWidth, ChartDashStyle borderStyle)
		{
			drawBottomShadow = true;
			sunken = false;
			outsideShadowRate = 0.9f;
			drawOutsideTopLeftShadow = false;
			bool drawScrews = base.drawScrews;
			base.drawScrews = false;
			base.DrawBorder(graph, borderSkin, rect, borderSkin.FrameBackColor, borderSkin.FrameBackHatchStyle, borderSkin.FrameBackImage, borderSkin.FrameBackImageMode, borderSkin.FrameBackImageTransparentColor, borderSkin.FrameBackImageAlign, borderSkin.FrameBackGradientType, borderSkin.FrameBackGradientEndColor, borderSkin.FrameBorderColor, borderSkin.FrameBorderWidth, borderSkin.FrameBorderStyle);
			base.drawScrews = drawScrews;
			rect.X += sizeLeftTop.Width;
			rect.Y += sizeLeftTop.Height;
			rect.Width -= sizeRightBottom.Width + sizeLeftTop.Width;
			rect.Height -= sizeRightBottom.Height + sizeLeftTop.Height;
			if (rect.Width > 0f && rect.Height > 0f)
			{
				float[] array = new float[8];
				array = (float[])cornerRadius.Clone();
				cornerRadius = innerCorners;
				drawBottomShadow = false;
				sunken = true;
				drawOutsideTopLeftShadow = true;
				outsideShadowRate = 1.4f;
				Color pageColor = borderSkin.PageColor;
				borderSkin.PageColor = Color.Transparent;
				base.DrawBorder(graph, borderSkin, rect, backColor, backHatchStyle, backImage, backImageMode, backImageTranspColor, backImageAlign, backGradientType, backGradientEndColor, borderColor, borderWidth, borderStyle);
				borderSkin.PageColor = pageColor;
				cornerRadius = array;
			}
		}
	}
}
