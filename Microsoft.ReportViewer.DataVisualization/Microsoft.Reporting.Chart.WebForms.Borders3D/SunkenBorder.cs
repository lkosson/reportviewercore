using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Microsoft.Reporting.Chart.WebForms.Borders3D
{
	internal class SunkenBorder : IBorderType
	{
		protected float defaultRadiusSize = 15f;

		protected float outsideShadowRate = 0.9f;

		protected bool sunken = true;

		protected bool drawBottomShadow = true;

		protected bool drawOutsideTopLeftShadow;

		protected float[] cornerRadius = new float[8]
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

		protected SizeF sizeLeftTop = SizeF.Empty;

		protected SizeF sizeRightBottom = SizeF.Empty;

		protected bool drawScrews;

		public float resolution = 96f;

		public virtual string Name => "Sunken";

		public virtual float Resolution
		{
			set
			{
				resolution = value;
				defaultRadiusSize = 15f * resolution / 96f;
				cornerRadius = new float[8]
				{
					defaultRadiusSize,
					defaultRadiusSize,
					defaultRadiusSize,
					defaultRadiusSize,
					defaultRadiusSize,
					defaultRadiusSize,
					defaultRadiusSize,
					defaultRadiusSize
				};
			}
		}

		public virtual RectangleF GetTitlePositionInBorder()
		{
			return RectangleF.Empty;
		}

		public virtual void AdjustAreasPosition(ChartGraphics graph, ref RectangleF areasRect)
		{
			SizeF size = new SizeF(sizeLeftTop);
			SizeF size2 = new SizeF(sizeRightBottom);
			size.Width += defaultRadiusSize * 0.7f;
			size.Height += defaultRadiusSize * 0.85f;
			size2.Width += defaultRadiusSize * 0.7f;
			size2.Height += defaultRadiusSize * 0.7f;
			size = graph.GetRelativeSize(size);
			size2 = graph.GetRelativeSize(size2);
			if (size.Width > 30f)
			{
				size.Width = 0f;
			}
			if (size.Height > 30f)
			{
				size.Height = 0f;
			}
			if (size2.Width > 30f)
			{
				size2.Width = 0f;
			}
			if (size2.Height > 30f)
			{
				size2.Height = 0f;
			}
			areasRect.X += size.Width;
			areasRect.Width -= Math.Min(areasRect.Width, size.Width + size2.Width);
			areasRect.Y += size.Height;
			areasRect.Height -= Math.Min(areasRect.Height, size.Height + size2.Height);
			if (areasRect.Right > 100f)
			{
				if (areasRect.Width > 100f - areasRect.Right)
				{
					areasRect.Width -= 100f - areasRect.Right;
				}
				else
				{
					areasRect.X -= 100f - areasRect.Right;
				}
			}
			if (areasRect.Bottom > 100f)
			{
				if (areasRect.Height > 100f - areasRect.Bottom)
				{
					areasRect.Height -= 100f - areasRect.Bottom;
				}
				else
				{
					areasRect.Y -= 100f - areasRect.Bottom;
				}
			}
		}

		public virtual void DrawBorder(ChartGraphics graph, BorderSkinAttributes borderSkin, RectangleF rect, Color backColor, ChartHatchStyle backHatchStyle, string backImage, ChartImageWrapMode backImageMode, Color backImageTranspColor, ChartImageAlign backImageAlign, GradientType backGradientType, Color backGradientEndColor, Color borderColor, int borderWidth, ChartDashStyle borderStyle)
		{
			RectangleF rectangleF = graph.Round(rect);
			RectangleF rectangleF2 = rectangleF;
			float num = 0.3f + 0.4f * (float)(borderSkin.PageColor.R + borderSkin.PageColor.G + borderSkin.PageColor.B) / 765f;
			Color color = Color.FromArgb((int)((float)(int)backColor.R * num), (int)((float)(int)backColor.G * num), (int)((float)(int)backColor.B * num));
			num += 0.2f;
			Color centerColor = Color.FromArgb((int)((float)(int)borderSkin.PageColor.R * num), (int)((float)(int)borderSkin.PageColor.G * num), (int)((float)(int)borderSkin.PageColor.B * num));
			if (borderSkin.PageColor == Color.Transparent)
			{
				centerColor = Color.FromArgb(60, 0, 0, 0);
			}
			Color.FromArgb((int)((float)(int)backColor.R * num), (int)((float)(int)backColor.G * num), (int)((float)(int)backColor.B * num));
			float val = defaultRadiusSize;
			val = Math.Max(val, 2f * resolution / 96f);
			val = Math.Min(val, rect.Width / 2f);
			val = Math.Min(val, rect.Height / 2f);
			val = (float)Math.Ceiling(val);
			graph.FillRectangle(new SolidBrush(borderSkin.PageColor), rect);
			if (drawOutsideTopLeftShadow)
			{
				rectangleF2 = rectangleF;
				rectangleF2.X -= val * 0.3f;
				rectangleF2.Y -= val * 0.3f;
				rectangleF2.Width -= val * 0.3f;
				rectangleF2.Height -= val * 0.3f;
				graph.DrawRoundedRectShadowAbs(rectangleF2, cornerRadius, val, Color.FromArgb(128, Color.Black), borderSkin.PageColor, outsideShadowRate);
			}
			rectangleF2 = rectangleF;
			rectangleF2.X += val * 0.3f;
			rectangleF2.Y += val * 0.3f;
			rectangleF2.Width -= val * 0.3f;
			rectangleF2.Height -= val * 0.3f;
			graph.DrawRoundedRectShadowAbs(rectangleF2, cornerRadius, val, centerColor, borderSkin.PageColor, outsideShadowRate);
			rectangleF2 = rectangleF;
			rectangleF2.Width -= val * 0.3f;
			rectangleF2.Height -= val * 0.3f;
			GraphicsPath graphicsPath = graph.CreateRoundedRectPath(rectangleF2, cornerRadius);
			graph.DrawPathAbs(graphicsPath, backColor, backHatchStyle, backImage, backImageMode, backImageTranspColor, backImageAlign, backGradientType, backGradientEndColor, borderColor, borderWidth, borderStyle, PenAlignment.Inset);
			graphicsPath?.Dispose();
			if (drawScrews)
			{
				RectangleF empty = RectangleF.Empty;
				float num2 = val * 0.4f;
				empty.X = rectangleF2.X + num2;
				empty.Y = rectangleF2.Y + num2;
				empty.Width = val * 0.55f;
				empty.Height = empty.Width;
				DrawScrew(graph, empty);
				empty.X = rectangleF2.Right - num2 - empty.Width;
				DrawScrew(graph, empty);
				empty.X = rectangleF2.Right - num2 - empty.Width;
				empty.Y = rectangleF2.Bottom - num2 - empty.Height;
				DrawScrew(graph, empty);
				empty.X = rectangleF2.X + num2;
				empty.Y = rectangleF2.Bottom - num2 - empty.Height;
				DrawScrew(graph, empty);
			}
			Region region = null;
			if (drawBottomShadow)
			{
				rectangleF2 = rectangleF;
				rectangleF2.Width -= val * 0.3f;
				rectangleF2.Height -= val * 0.3f;
				region = new Region(graph.CreateRoundedRectPath(new RectangleF(rectangleF2.X - val, rectangleF2.Y - val, rectangleF2.Width + 0.5f * val, rectangleF2.Height + 0.5f * val), cornerRadius));
				region.Complement(graph.CreateRoundedRectPath(rectangleF2, cornerRadius));
				graph.Clip = region;
				rectangleF2.X -= 0.5f * val;
				rectangleF2.Width += 0.5f * val;
				rectangleF2.Y -= 0.5f * val;
				rectangleF2.Height += 0.5f * val;
				graph.DrawRoundedRectShadowAbs(rectangleF2, cornerRadius, val, Color.Transparent, Color.FromArgb(175, sunken ? Color.White : color), 1f);
				graph.Clip = new Region();
			}
			rectangleF2 = rectangleF;
			rectangleF2.Width -= val * 0.3f;
			rectangleF2.Height -= val * 0.3f;
			region = new Region(graph.CreateRoundedRectPath(new RectangleF(rectangleF2.X + val * 0.5f, rectangleF2.Y + val * 0.5f, rectangleF2.Width - 0.2f * val, rectangleF2.Height - 0.2f * val), cornerRadius));
			RectangleF rect2 = rectangleF2;
			rect2.Width += val;
			rect2.Height += val;
			region.Complement(graph.CreateRoundedRectPath(rect2, cornerRadius));
			region.Intersect(graph.CreateRoundedRectPath(rectangleF2, cornerRadius));
			graph.Clip = region;
			graph.DrawRoundedRectShadowAbs(rect2, cornerRadius, val, Color.Transparent, Color.FromArgb(175, sunken ? color : Color.White), 1f);
			graph.Clip = new Region();
		}

		private void DrawScrew(ChartGraphics graph, RectangleF rect)
		{
			Pen pen = new Pen(Color.FromArgb(128, 255, 255, 255), 1f);
			graph.DrawEllipse(pen, rect.X, rect.Y, rect.Width, rect.Height);
			graph.DrawLine(pen, rect.X + 2f * resolution / 96f, rect.Y + rect.Height - 2f * resolution / 96f, rect.Right - 2f * resolution / 96f, rect.Y + 2f * resolution / 96f);
			pen = new Pen(Color.FromArgb(128, Color.Black), 1f);
			graph.DrawEllipse(pen, rect.X + 1f * resolution / 96f, rect.Y + 1f * resolution / 96f, rect.Width, rect.Height);
			graph.DrawLine(pen, rect.X + 3f * resolution / 96f, rect.Y + rect.Height - 1f * resolution / 96f, rect.Right - 1f * resolution / 96f, rect.Y + 3f * resolution / 96f);
		}
	}
}
