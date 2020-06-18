using System.Drawing;

namespace Microsoft.Reporting.Map.WebForms
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

		public FrameThin1Border()
		{
			sizeLeftTop = new SizeF(defaultRadiusSize * 0.8f, defaultRadiusSize * 0.8f);
			sizeRightBottom = new SizeF(defaultRadiusSize * 0.8f, defaultRadiusSize * 0.8f);
		}

		public override void DrawBorder(MapGraphics graph, Frame borderSkin, RectangleF rect, Color backColor, MapHatchStyle backHatchStyle, string backImage, MapImageWrapMode backImageMode, Color backImageTranspColor, MapImageAlign backImageAlign, GradientType backGradientType, Color backSecondaryColor, Color borderColor, int borderWidth, MapDashStyle borderStyle)
		{
			drawBottomShadow = true;
			sunken = false;
			outsideShadowRate = 0.9f;
			drawOutsideTopLeftShadow = false;
			bool drawScrews = base.drawScrews;
			base.drawScrews = false;
			base.DrawBorder(graph, borderSkin, rect, borderSkin.BackColor, borderSkin.BackHatchStyle, borderSkin.BackImage, borderSkin.BackImageMode, borderSkin.BackImageTranspColor, borderSkin.BackImageAlign, borderSkin.BackGradientType, borderSkin.BackSecondaryColor, borderSkin.BorderColor, borderSkin.BorderWidth, borderSkin.BorderStyle);
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
				base.DrawBorder(graph, borderSkin, rect, backColor, backHatchStyle, backImage, backImageMode, backImageTranspColor, backImageAlign, backGradientType, backSecondaryColor, borderColor, borderWidth, borderStyle);
				borderSkin.PageColor = pageColor;
				cornerRadius = array;
			}
		}
	}
}
