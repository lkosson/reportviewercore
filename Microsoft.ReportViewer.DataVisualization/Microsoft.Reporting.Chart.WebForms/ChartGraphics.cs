using Microsoft.Reporting.Chart.WebForms.Borders3D;
using Microsoft.Reporting.Chart.WebForms.Utilities;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Threading;

namespace Microsoft.Reporting.Chart.WebForms
{
	internal class ChartGraphics : ChartGraphics3D
	{
		internal CommonElements common;

		internal Pen pen;

		private SolidBrush solidBrush;

		private Matrix myMatrix;

		private int width;

		private int height;

		internal bool softShadows = true;

		private AntiAliasingTypes antiAliasing = AntiAliasingTypes.All;

		internal bool IsMetafile;

		public new Graphics Graphics
		{
			get
			{
				return base.Graphics;
			}
			set
			{
				base.Graphics = value;
			}
		}

		internal AntiAliasingTypes AntiAliasing
		{
			get
			{
				return antiAliasing;
			}
			set
			{
				antiAliasing = value;
				if (Graphics != null)
				{
					if ((antiAliasing & AntiAliasingTypes.Graphics) == AntiAliasingTypes.Graphics)
					{
						base.SmoothingMode = SmoothingMode.AntiAlias;
					}
					else
					{
						base.SmoothingMode = SmoothingMode.None;
					}
				}
			}
		}

		internal void DrawLineRel(Color color, int width, ChartDashStyle style, PointF firstPointF, PointF secondPointF)
		{
			DrawLineAbs(color, width, style, GetAbsolutePoint(firstPointF), GetAbsolutePoint(secondPointF));
		}

		internal void DrawLineAbs(Color color, int width, ChartDashStyle style, PointF firstPoint, PointF secondPoint)
		{
			if (width != 0 && style != 0)
			{
				if (pen.Color != color)
				{
					pen.Color = color;
				}
				if (pen.Width != (float)width)
				{
					pen.Width = width;
				}
				if (pen.DashStyle != GetPenStyle(style))
				{
					pen.DashStyle = GetPenStyle(style);
				}
				SmoothingMode smoothingMode = base.SmoothingMode;
				if (width <= 1 && style != ChartDashStyle.Solid && (firstPoint.X == secondPoint.X || firstPoint.Y == secondPoint.Y))
				{
					base.SmoothingMode = SmoothingMode.Default;
				}
				DrawLine(pen, (float)Math.Round(firstPoint.X), (float)Math.Round(firstPoint.Y), (float)Math.Round(secondPoint.X), (float)Math.Round(secondPoint.Y));
				base.SmoothingMode = smoothingMode;
			}
		}

		internal void DrawLineRel(Color color, int width, ChartDashStyle style, PointF firstPoint, PointF secondPoint, Color shadowColor, int shadowOffset)
		{
			DrawLineAbs(color, width, style, GetAbsolutePoint(firstPoint), GetAbsolutePoint(secondPoint), shadowColor, shadowOffset);
		}

		internal void DrawLineAbs(Color color, int width, ChartDashStyle style, PointF firstPoint, PointF secondPoint, Color shadowColor, int shadowOffset)
		{
			if (shadowOffset != 0)
			{
				Color color2 = (shadowColor.A == byte.MaxValue) ? Color.FromArgb((int)color.A / 2, shadowColor) : shadowColor;
				PointF firstPoint2 = new PointF(firstPoint.X + (float)shadowOffset, firstPoint.Y + (float)shadowOffset);
				PointF secondPoint2 = new PointF(secondPoint.X + (float)shadowOffset, secondPoint.Y + (float)shadowOffset);
				shadowDrawingMode = true;
				DrawLineAbs(color2, width, style, firstPoint2, secondPoint2);
				shadowDrawingMode = false;
			}
			DrawLineAbs(color, width, style, firstPoint, secondPoint);
		}

		public Brush GetHatchBrush(ChartHatchStyle hatchStyle, Color backColor, Color foreColor)
		{
			return new HatchBrush((HatchStyle)Enum.Parse(typeof(HatchStyle), hatchStyle.ToString()), foreColor, backColor);
		}

		internal Brush GetTextureBrush(string name, Color backImageTranspColor, ChartImageWrapMode mode, Color backColor)
		{
			Image image = common.ImageLoader.LoadImage(name);
			ImageAttributes imageAttributes = new ImageAttributes();
			imageAttributes.SetWrapMode((WrapMode)((mode == ChartImageWrapMode.Unscaled) ? ChartImageWrapMode.Scaled : mode));
			if (backImageTranspColor != Color.Empty)
			{
				imageAttributes.SetColorKey(backImageTranspColor, backImageTranspColor, ColorAdjustType.Default);
			}
			if (backImageTranspColor == Color.Empty && image is Metafile && backColor != Color.Transparent)
			{
				TextureBrush textureBrush = null;
				Bitmap image2 = new Bitmap(image.Width, image.Height);
				using (Graphics graphics = Graphics.FromImage(image2))
				{
					using (SolidBrush brush = new SolidBrush(backColor))
					{
						graphics.FillRectangle(brush, 0, 0, image.Width, image.Height);
						graphics.DrawImageUnscaled(image, 0, 0);
						return new TextureBrush(image2, new RectangleF(0f, 0f, image.Width, image.Height), imageAttributes);
					}
				}
			}
			TextureBrush result;
			if (ImageLoader.DoDpisMatch(image, Graphics))
			{
				result = new TextureBrush(image, new RectangleF(0f, 0f, image.Width, image.Height), imageAttributes);
			}
			else
			{
				Image scaledImage = ImageLoader.GetScaledImage(image, Graphics);
				result = new TextureBrush(scaledImage, new RectangleF(0f, 0f, scaledImage.Width, scaledImage.Height), imageAttributes);
				scaledImage.Dispose();
			}
			return result;
		}

		public Brush GetGradientBrush(RectangleF rectangle, Color firstColor, Color secondColor, GradientType type)
		{
			SetGradient(firstColor, secondColor, type);
			rectangle.Inflate(1f, 1f);
			Brush brush = null;
			float angle = 0f;
			if (rectangle.Height == 0f || rectangle.Width == 0f)
			{
				return new SolidBrush(Color.Black);
			}
			switch (type)
			{
			case GradientType.LeftRight:
			case GradientType.VerticalCenter:
				angle = 0f;
				break;
			case GradientType.TopBottom:
			case GradientType.HorizontalCenter:
				angle = 90f;
				break;
			case GradientType.DiagonalLeft:
				angle = (float)(Math.Atan(rectangle.Width / rectangle.Height) * 180.0 / Math.PI);
				break;
			case GradientType.DiagonalRight:
				angle = (float)(180.0 - Math.Atan(rectangle.Width / rectangle.Height) * 180.0 / Math.PI);
				break;
			}
			if (type == GradientType.TopBottom || type == GradientType.LeftRight || type == GradientType.DiagonalLeft || type == GradientType.DiagonalRight || type == GradientType.HorizontalCenter || type == GradientType.VerticalCenter)
			{
				RectangleF rect = new RectangleF(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
				switch (type)
				{
				case GradientType.HorizontalCenter:
					rect.Height /= 2f;
					brush = new LinearGradientBrush(rect, firstColor, secondColor, angle);
					((LinearGradientBrush)brush).WrapMode = WrapMode.TileFlipX;
					break;
				case GradientType.VerticalCenter:
					rect.Width /= 2f;
					brush = new LinearGradientBrush(rect, firstColor, secondColor, angle);
					((LinearGradientBrush)brush).WrapMode = WrapMode.TileFlipX;
					break;
				default:
					brush = new LinearGradientBrush(rectangle, firstColor, secondColor, angle);
					break;
				}
				return brush;
			}
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.AddRectangle(rectangle);
			brush = new PathGradientBrush(graphicsPath);
			((PathGradientBrush)brush).CenterColor = firstColor;
			Color[] surroundColors = new Color[1]
			{
				secondColor
			};
			((PathGradientBrush)brush).SurroundColors = surroundColors;
			graphicsPath?.Dispose();
			return brush;
		}

		internal Brush GetPieGradientBrush(RectangleF rectangle, Color firstColor, Color secondColor)
		{
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.AddEllipse(rectangle);
			PathGradientBrush pathGradientBrush = new PathGradientBrush(graphicsPath);
			pathGradientBrush.CenterColor = firstColor;
			Color[] array2 = pathGradientBrush.SurroundColors = new Color[1]
			{
				secondColor
			};
			graphicsPath?.Dispose();
			return pathGradientBrush;
		}

		internal DashStyle GetPenStyle(ChartDashStyle style)
		{
			switch (style)
			{
			case ChartDashStyle.Dash:
				return DashStyle.Dash;
			case ChartDashStyle.DashDot:
				return DashStyle.DashDot;
			case ChartDashStyle.DashDotDot:
				return DashStyle.DashDotDot;
			case ChartDashStyle.Dot:
				return DashStyle.Dot;
			default:
				return DashStyle.Solid;
			}
		}

		public PointF[] CreateStarPolygon(RectangleF rect, int numberOfCorners)
		{
			int num = checked(numberOfCorners * 2);
			bool flag = true;
			PointF[] array = new PointF[num];
			PointF[] array2 = new PointF[1];
			for (int i = 0; i < num; i++)
			{
				array2[0] = new PointF(rect.X + rect.Width / 2f, flag ? rect.Y : (rect.Y + rect.Height / 4f));
				Matrix matrix = new Matrix();
				matrix.RotateAt((float)i * (360f / ((float)numberOfCorners * 2f)), new PointF(rect.X + rect.Width / 2f, rect.Y + rect.Height / 2f));
				matrix.TransformPoints(array2);
				array[i] = array2[0];
				flag = !flag;
			}
			return array;
		}

		internal void DrawMarkerRel(PointF point, MarkerStyle markerStyle, int markerSize, Color markerColor, Color markerBorderColor, int markerBorderSize, string markerImage, Color markerImageTranspColor, int shadowSize, Color shadowColor, RectangleF imageScaleRect)
		{
			DrawMarkerAbs(GetAbsolutePoint(point), markerStyle, markerSize, markerColor, markerBorderColor, markerBorderSize, markerImage, markerImageTranspColor, shadowSize, shadowColor, imageScaleRect, forceAntiAlias: false);
		}

		internal void DrawMarkerAbs(PointF point, MarkerStyle markerStyle, int markerSize, Color markerColor, Color markerBorderColor, int markerBorderSize, string markerImage, Color markerImageTranspColor, int shadowSize, Color shadowColor, RectangleF imageScaleRect, bool forceAntiAlias)
		{
			if (markerBorderSize <= 0)
			{
				markerBorderColor = Color.Transparent;
			}
			if (markerImage.Length > 0)
			{
				Image image = common.ImageLoader.LoadImage(markerImage);
				RectangleF empty = RectangleF.Empty;
				if (imageScaleRect == RectangleF.Empty)
				{
					SizeF size = default(SizeF);
					ImageLoader.GetAdjustedImageSize(image, Graphics, ref size);
					imageScaleRect.Width = size.Width;
					imageScaleRect.Height = size.Height;
				}
				empty.X = point.X - imageScaleRect.Width / 2f;
				empty.Y = point.Y - imageScaleRect.Height / 2f;
				empty.Width = imageScaleRect.Width;
				empty.Height = imageScaleRect.Height;
				ImageAttributes imageAttributes = new ImageAttributes();
				if (markerImageTranspColor != Color.Empty)
				{
					imageAttributes.SetColorKey(markerImageTranspColor, markerImageTranspColor, ColorAdjustType.Default);
				}
				if (shadowSize != 0 && shadowColor != Color.Empty)
				{
					ImageAttributes imageAttributes2 = new ImageAttributes();
					imageAttributes2.SetColorKey(markerImageTranspColor, markerImageTranspColor, ColorAdjustType.Default);
					ColorMatrix colorMatrix = new ColorMatrix();
					colorMatrix.Matrix00 = 0.25f;
					colorMatrix.Matrix11 = 0.25f;
					colorMatrix.Matrix22 = 0.25f;
					colorMatrix.Matrix33 = 0.5f;
					colorMatrix.Matrix44 = 1f;
					imageAttributes2.SetColorMatrix(colorMatrix);
					shadowDrawingMode = true;
					DrawImage(image, new Rectangle((int)empty.X + shadowSize, (int)empty.Y + shadowSize, (int)empty.Width, (int)empty.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes2);
					shadowDrawingMode = false;
				}
				DrawImage(image, new Rectangle((int)empty.X, (int)empty.Y, (int)empty.Width, (int)empty.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
			}
			else
			{
				if (markerStyle == MarkerStyle.None || markerSize <= 0 || !(markerColor != Color.Empty))
				{
					return;
				}
				SmoothingMode smoothingMode = base.SmoothingMode;
				if (forceAntiAlias)
				{
					base.SmoothingMode = SmoothingMode.AntiAlias;
				}
				SolidBrush brush = new SolidBrush(markerColor);
				RectangleF empty2 = RectangleF.Empty;
				empty2.X = point.X - (float)markerSize / 2f;
				empty2.Y = point.Y - (float)markerSize / 2f;
				empty2.Width = markerSize;
				empty2.Height = markerSize;
				switch (markerStyle)
				{
				case MarkerStyle.Star4:
				case MarkerStyle.Star5:
				case MarkerStyle.Star6:
				case MarkerStyle.Star10:
				{
					int numberOfCorners = 4;
					switch (markerStyle)
					{
					case MarkerStyle.Star5:
						numberOfCorners = 5;
						break;
					case MarkerStyle.Star6:
						numberOfCorners = 6;
						break;
					case MarkerStyle.Star10:
						numberOfCorners = 10;
						break;
					}
					PointF[] points = CreateStarPolygon(empty2, numberOfCorners);
					if (shadowSize != 0 && shadowColor != Color.Empty)
					{
						Matrix matrix4 = base.Transform.Clone();
						matrix4.Translate(shadowSize, shadowSize);
						Matrix transform5 = base.Transform;
						base.Transform = matrix4;
						shadowDrawingMode = true;
						FillPolygon(new SolidBrush((shadowColor.A != byte.MaxValue) ? shadowColor : Color.FromArgb((int)markerColor.A / 2, shadowColor)), points);
						shadowDrawingMode = false;
						base.Transform = transform5;
					}
					FillPolygon(brush, points);
					DrawPolygon(new Pen(markerBorderColor, markerBorderSize), points);
					break;
				}
				case MarkerStyle.Circle:
					if (shadowSize != 0 && shadowColor != Color.Empty)
					{
						shadowDrawingMode = true;
						if (!softShadows)
						{
							SolidBrush brush2 = new SolidBrush((shadowColor.A != byte.MaxValue) ? shadowColor : Color.FromArgb((int)markerColor.A / 2, shadowColor));
							RectangleF rect = empty2;
							rect.X += shadowSize;
							rect.Y += shadowSize;
							FillEllipse(brush2, rect);
						}
						else
						{
							GraphicsPath graphicsPath3 = new GraphicsPath();
							graphicsPath3.AddEllipse(empty2.X + (float)shadowSize - 1f, empty2.Y + (float)shadowSize - 1f, empty2.Width + 2f, empty2.Height + 2f);
							PathGradientBrush pathGradientBrush3 = new PathGradientBrush(graphicsPath3);
							pathGradientBrush3.CenterColor = shadowColor;
							Color[] array8 = pathGradientBrush3.SurroundColors = new Color[1]
							{
								Color.Transparent
							};
							pathGradientBrush3.CenterPoint = new PointF(point.X, point.Y);
							PointF focusScales3 = new PointF(1f - 2f * (float)shadowSize / empty2.Width, 1f - 2f * (float)shadowSize / empty2.Height);
							if (focusScales3.X < 0f)
							{
								focusScales3.X = 0f;
							}
							if (focusScales3.Y < 0f)
							{
								focusScales3.Y = 0f;
							}
							pathGradientBrush3.FocusScales = focusScales3;
							FillPath(pathGradientBrush3, graphicsPath3);
						}
						shadowDrawingMode = false;
					}
					FillEllipse(brush, empty2);
					DrawEllipse(new Pen(markerBorderColor, markerBorderSize), empty2);
					break;
				case MarkerStyle.Square:
					if (shadowSize != 0 && shadowColor != Color.Empty)
					{
						shadowDrawingMode = true;
						FillRectangleShadowAbs(empty2, shadowColor, shadowSize, shadowColor);
						shadowDrawingMode = false;
					}
					FillRectangle(brush, empty2);
					DrawRectangle(new Pen(markerBorderColor, markerBorderSize), (int)Math.Round(empty2.X, 0), (int)Math.Round(empty2.Y, 0), (int)Math.Round(empty2.Width, 0), (int)Math.Round(empty2.Height, 0));
					break;
				case MarkerStyle.Cross:
				{
					float num = (float)Math.Ceiling((float)markerSize / 4f);
					float num2 = markerSize;
					PointF[] array4 = new PointF[12];
					array4[0].X = point.X - num2 / 2f;
					array4[0].Y = point.Y + num / 2f;
					array4[1].X = point.X - num2 / 2f;
					array4[1].Y = point.Y - num / 2f;
					array4[2].X = point.X - num / 2f;
					array4[2].Y = point.Y - num / 2f;
					array4[3].X = point.X - num / 2f;
					array4[3].Y = point.Y - num2 / 2f;
					array4[4].X = point.X + num / 2f;
					array4[4].Y = point.Y - num2 / 2f;
					array4[5].X = point.X + num / 2f;
					array4[5].Y = point.Y - num / 2f;
					array4[6].X = point.X + num2 / 2f;
					array4[6].Y = point.Y - num / 2f;
					array4[7].X = point.X + num2 / 2f;
					array4[7].Y = point.Y + num / 2f;
					array4[8].X = point.X + num / 2f;
					array4[8].Y = point.Y + num / 2f;
					array4[9].X = point.X + num / 2f;
					array4[9].Y = point.Y + num2 / 2f;
					array4[10].X = point.X - num / 2f;
					array4[10].Y = point.Y + num2 / 2f;
					array4[11].X = point.X - num / 2f;
					array4[11].Y = point.Y + num / 2f;
					Matrix matrix2 = new Matrix();
					matrix2.RotateAt(45f, point);
					matrix2.TransformPoints(array4);
					if (shadowSize != 0 && shadowColor != Color.Empty)
					{
						shadowDrawingMode = true;
						Matrix matrix3 = base.Transform.Clone();
						matrix3.Translate(softShadows ? (shadowSize + 1) : shadowSize, softShadows ? (shadowSize + 1) : shadowSize);
						Matrix transform2 = base.Transform;
						base.Transform = matrix3;
						if (!softShadows)
						{
							FillPolygon(new SolidBrush((shadowColor.A != byte.MaxValue) ? shadowColor : Color.FromArgb((int)markerColor.A / 2, shadowColor)), array4);
						}
						else
						{
							GraphicsPath graphicsPath2 = new GraphicsPath();
							graphicsPath2.AddPolygon(array4);
							PathGradientBrush pathGradientBrush2 = new PathGradientBrush(graphicsPath2);
							pathGradientBrush2.CenterColor = shadowColor;
							Color[] array6 = pathGradientBrush2.SurroundColors = new Color[1]
							{
								Color.Transparent
							};
							pathGradientBrush2.CenterPoint = new PointF(point.X, point.Y);
							PointF focusScales2 = new PointF(1f - 2f * (float)shadowSize / empty2.Width, 1f - 2f * (float)shadowSize / empty2.Height);
							if (focusScales2.X < 0f)
							{
								focusScales2.X = 0f;
							}
							if (focusScales2.Y < 0f)
							{
								focusScales2.Y = 0f;
							}
							pathGradientBrush2.FocusScales = focusScales2;
							FillPath(pathGradientBrush2, graphicsPath2);
						}
						base.Transform = transform2;
						shadowDrawingMode = false;
					}
					Matrix transform3 = base.Transform.Clone();
					Matrix transform4 = base.Transform;
					base.Transform = transform3;
					FillPolygon(brush, array4);
					DrawPolygon(new Pen(markerBorderColor, markerBorderSize), array4);
					base.Transform = transform4;
					break;
				}
				case MarkerStyle.Diamond:
				{
					PointF[] array9 = new PointF[4];
					array9[0].X = empty2.X;
					array9[0].Y = empty2.Y + empty2.Height / 2f;
					array9[1].X = empty2.X + empty2.Width / 2f;
					array9[1].Y = empty2.Top;
					array9[2].X = empty2.Right;
					array9[2].Y = empty2.Y + empty2.Height / 2f;
					array9[3].X = empty2.X + empty2.Width / 2f;
					array9[3].Y = empty2.Bottom;
					if (shadowSize != 0 && shadowColor != Color.Empty)
					{
						shadowDrawingMode = true;
						Matrix matrix5 = base.Transform.Clone();
						matrix5.Translate((!softShadows) ? shadowSize : 0, (!softShadows) ? shadowSize : 0);
						Matrix transform6 = base.Transform;
						base.Transform = matrix5;
						if (!softShadows)
						{
							FillPolygon(new SolidBrush((shadowColor.A != byte.MaxValue) ? shadowColor : Color.FromArgb((int)markerColor.A / 2, shadowColor)), array9);
						}
						else
						{
							float num3 = (float)markerSize * (float)Math.Sin(Math.PI / 4.0);
							RectangleF empty3 = RectangleF.Empty;
							empty3.X = point.X - num3 / 2f;
							empty3.Y = point.Y - num3 / 2f - (float)shadowSize;
							empty3.Width = num3;
							empty3.Height = num3;
							matrix5.RotateAt(45f, point);
							base.Transform = matrix5;
							FillRectangleShadowAbs(empty3, shadowColor, shadowSize, shadowColor);
						}
						base.Transform = transform6;
						shadowDrawingMode = false;
					}
					FillPolygon(brush, array9);
					DrawPolygon(new Pen(markerBorderColor, markerBorderSize), array9);
					break;
				}
				case MarkerStyle.Triangle:
				{
					PointF[] array = new PointF[3];
					array[0].X = empty2.X;
					array[0].Y = empty2.Bottom;
					array[1].X = empty2.X + empty2.Width / 2f;
					array[1].Y = empty2.Top;
					array[2].X = empty2.Right;
					array[2].Y = empty2.Bottom;
					if (shadowSize != 0 && shadowColor != Color.Empty)
					{
						shadowDrawingMode = true;
						Matrix matrix = base.Transform.Clone();
						matrix.Translate(softShadows ? (shadowSize - 1) : shadowSize, softShadows ? (shadowSize + 1) : shadowSize);
						Matrix transform = base.Transform;
						base.Transform = matrix;
						if (!softShadows)
						{
							FillPolygon(new SolidBrush((shadowColor.A != byte.MaxValue) ? shadowColor : Color.FromArgb((int)markerColor.A / 2, shadowColor)), array);
						}
						else
						{
							GraphicsPath graphicsPath = new GraphicsPath();
							graphicsPath.AddPolygon(array);
							PathGradientBrush pathGradientBrush = new PathGradientBrush(graphicsPath);
							pathGradientBrush.CenterColor = shadowColor;
							Color[] array3 = pathGradientBrush.SurroundColors = new Color[1]
							{
								Color.Transparent
							};
							pathGradientBrush.CenterPoint = new PointF(point.X, point.Y);
							PointF focusScales = new PointF(1f - 2f * (float)shadowSize / empty2.Width, 1f - 2f * (float)shadowSize / empty2.Height);
							if (focusScales.X < 0f)
							{
								focusScales.X = 0f;
							}
							if (focusScales.Y < 0f)
							{
								focusScales.Y = 0f;
							}
							pathGradientBrush.FocusScales = focusScales;
							FillPath(pathGradientBrush, graphicsPath);
						}
						base.Transform = transform;
						shadowDrawingMode = false;
					}
					FillPolygon(brush, array);
					DrawPolygon(new Pen(markerBorderColor, markerBorderSize), array);
					break;
				}
				default:
					throw new InvalidOperationException(SR.ExceptionGraphicsMarkerStyleUnknown);
				}
				if (forceAntiAlias)
				{
					base.SmoothingMode = smoothingMode;
				}
			}
		}

		public SizeF MeasureString(string text, Font font, SizeF layoutArea, StringFormat stringFormat, TextOrientation textOrientation)
		{
			if (textOrientation == TextOrientation.Stacked)
			{
				text = GetStackedText(text);
			}
			return MeasureString(text, font, layoutArea, stringFormat);
		}

		internal SizeF MeasureStringRel(string text, Font font, SizeF layoutArea, StringFormat stringFormat, TextOrientation textOrientation)
		{
			if (textOrientation == TextOrientation.Stacked)
			{
				text = GetStackedText(text);
			}
			return MeasureStringRel(text, font, layoutArea, stringFormat);
		}

		public void DrawString(string text, Font font, Brush brush, RectangleF rect, StringFormat format, TextOrientation textOrientation)
		{
			if (textOrientation == TextOrientation.Stacked)
			{
				text = GetStackedText(text);
			}
			DrawString(text, font, brush, rect, format);
		}

		internal void DrawStringRel(string text, Font font, Brush brush, PointF position, StringFormat format, int angle, TextOrientation textOrientation)
		{
			if (textOrientation == TextOrientation.Stacked)
			{
				text = GetStackedText(text);
			}
			DrawStringRel(text, font, brush, position, format, angle);
		}

		internal void DrawStringRel(string text, Font font, Brush brush, RectangleF position, StringFormat format, TextOrientation textOrientation)
		{
			if (textOrientation == TextOrientation.Stacked)
			{
				text = GetStackedText(text);
			}
			DrawStringRel(text, font, brush, position, format);
		}

		internal static string GetStackedText(string text)
		{
			string text2 = string.Empty;
			for (int i = 0; i < text.Length; i++)
			{
				char c = text[i];
				text2 += c;
				if (c != '\n')
				{
					text2 += "\n";
				}
			}
			return text2;
		}

		internal void DrawPointLabelStringRel(CommonElements common, string text, Font font, Brush brush, RectangleF position, StringFormat format, int angle, RectangleF backPosition, Color backColor, Color borderColor, int borderWidth, ChartDashStyle borderStyle, Series series, DataPoint point, int pointIndex)
		{
			StartHotRegion(point, labelRegion: true);
			DrawPointLabelBackground(common, angle, PointF.Empty, backPosition, backColor, borderColor, borderWidth, borderStyle, series, point, pointIndex);
			EndHotRegion();
			DrawStringRel(text, font, brush, position, format, angle);
		}

		internal void DrawPointLabelStringRel(CommonElements common, string text, Font font, Brush brush, PointF position, StringFormat format, int angle, RectangleF backPosition, Color backColor, Color borderColor, int borderWidth, ChartDashStyle borderStyle, Series series, DataPoint point, int pointIndex)
		{
			StartHotRegion(point, labelRegion: true);
			DrawPointLabelBackground(common, angle, position, backPosition, backColor, borderColor, borderWidth, borderStyle, series, point, pointIndex);
			EndHotRegion();
			DrawStringRel(text, font, brush, position, format, angle);
		}

		private void DrawPointLabelBackground(CommonElements common, int angle, PointF textPosition, RectangleF backPosition, Color backColor, Color borderColor, int borderWidth, ChartDashStyle borderStyle, Series series, DataPoint point, int pointIndex)
		{
			if (backPosition.IsEmpty)
			{
				return;
			}
			RectangleF rect = Round(GetAbsoluteRectangle(backPosition));
			PointF empty = PointF.Empty;
			empty = ((!textPosition.IsEmpty) ? GetAbsolutePoint(textPosition) : new PointF(rect.X + rect.Width / 2f, rect.Y + rect.Height / 2f));
			myMatrix = base.Transform.Clone();
			myMatrix.RotateAt(angle, empty);
			GraphicsState gstate = Save();
			base.Transform = myMatrix;
			if (!backColor.IsEmpty || !borderColor.IsEmpty)
			{
				using (Brush brush = new SolidBrush(backColor))
				{
					FillRectangle(brush, rect);
				}
				if (borderWidth > 0 && !borderColor.IsEmpty && borderStyle != 0)
				{
					AntiAliasingTypes antiAliasingTypes = AntiAliasing;
					try
					{
						AntiAliasing = AntiAliasingTypes.None;
						using (Pen pen = new Pen(borderColor, borderWidth))
						{
							pen.DashStyle = GetPenStyle(borderStyle);
							DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
						}
					}
					finally
					{
						AntiAliasing = antiAliasingTypes;
					}
				}
			}
			else
			{
				using (Brush brush2 = new SolidBrush(Color.Transparent))
				{
					FillRectangle(brush2, rect);
				}
			}
			Restore(gstate);
			if (common != null && common.ProcessModeRegions)
			{
				common.HotRegionsList.FindInsertIndex();
				string toolTip = point.ToolTip;
				string href = point.Href;
				string mapAreaAttributes = point.MapAreaAttributes;
				object tag = ((IMapAreaAttributes)point).Tag;
				point.ToolTip = point.LabelToolTip;
				point.Href = point.LabelHref;
				point.MapAreaAttributes = point.LabelMapAreaAttributes;
				((IMapAreaAttributes)point).Tag = point.LabelTag;
				if (angle == 0)
				{
					common.HotRegionsList.AddHotRegion(this, backPosition, point, series.Name, pointIndex);
				}
				else
				{
					GraphicsPath graphicsPath = new GraphicsPath();
					graphicsPath.AddRectangle(rect);
					graphicsPath.Transform(myMatrix);
					common.HotRegionsList.AddHotRegion(graphicsPath, relativePath: false, this, point, series.Name, pointIndex);
				}
				point.ToolTip = toolTip;
				point.Href = href;
				point.MapAreaAttributes = mapAreaAttributes;
				((IMapAreaAttributes)point).Tag = tag;
				if (common.HotRegionsList.List != null)
				{
					((HotRegion)common.HotRegionsList.List[common.HotRegionsList.List.Count - 1]).Type = ChartElementType.DataPointLabel;
				}
			}
		}

		internal void DrawStringRel(string text, Font font, Brush brush, PointF position, StringFormat format, int angle)
		{
			DrawStringAbs(text, font, brush, GetAbsolutePoint(position), format, angle);
		}

		internal void DrawStringAbs(string text, Font font, Brush brush, PointF absPosition, StringFormat format, int angle)
		{
			myMatrix = base.Transform.Clone();
			myMatrix.RotateAt(angle, absPosition);
			GraphicsState gstate = Save();
			base.Transform = myMatrix;
			DrawString(text, font, brush, absPosition, format);
			Restore(gstate);
		}

		internal GraphicsPath GetTranformedTextRectPath(PointF center, SizeF size, int angle)
		{
			size.Width += 10f;
			size.Height += 10f;
			PointF absolutePoint = GetAbsolutePoint(center);
			PointF[] array = new PointF[4]
			{
				new PointF(absolutePoint.X - size.Width / 2f, absolutePoint.Y - size.Height / 2f),
				new PointF(absolutePoint.X + size.Width / 2f, absolutePoint.Y - size.Height / 2f),
				new PointF(absolutePoint.X + size.Width / 2f, absolutePoint.Y + size.Height / 2f),
				new PointF(absolutePoint.X - size.Width / 2f, absolutePoint.Y + size.Height / 2f)
			};
			Matrix matrix = base.Transform.Clone();
			matrix.RotateAt(angle, absolutePoint);
			matrix.TransformPoints(array);
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.AddLines(array);
			graphicsPath.CloseAllFigures();
			return graphicsPath;
		}

		internal void DrawLabelStringRel(Axis axis, int labelRowIndex, LabelMark labelMark, Color markColor, string text, string image, Color imageTranspColor, Font font, Brush brush, RectangleF position, StringFormat format, int angle, RectangleF boundaryRect, CustomLabel label, bool truncatedLeft, bool truncatedRight)
		{
			StringFormat stringFormat = (StringFormat)format.Clone();
			SizeF sizeF = SizeF.Empty;
			if (position.Width == 0f || position.Height == 0f)
			{
				return;
			}
			RectangleF rectangleF = GetAbsoluteRectangle(position);
			if (rectangleF.Width < 1f)
			{
				rectangleF.Width = 1f;
			}
			if (rectangleF.Height < 1f)
			{
				rectangleF.Height = 1f;
			}
			CommonElements commonElements = axis.Common;
			if (commonElements.ProcessModeRegions)
			{
				commonElements.HotRegionsList.AddHotRegion(Rectangle.Round(rectangleF), label, ChartElementType.AxisLabels, relativeCoordinates: false, insertAtBeginning: true);
			}
			if (labelRowIndex > 0)
			{
				stringFormat.LineAlignment = StringAlignment.Center;
				stringFormat.Alignment = StringAlignment.Center;
				angle = 0;
				if (axis.AxisPosition == AxisPosition.Left)
				{
					angle = -90;
				}
				else if (axis.AxisPosition == AxisPosition.Right)
				{
					angle = 90;
				}
				else if (axis.AxisPosition != AxisPosition.Top)
				{
					_ = axis.AxisPosition;
					_ = 3;
				}
			}
			PointF empty = PointF.Empty;
			if (axis.AxisPosition == AxisPosition.Left)
			{
				empty.X = rectangleF.Right;
				empty.Y = rectangleF.Y + rectangleF.Height / 2f;
			}
			else if (axis.AxisPosition == AxisPosition.Right)
			{
				empty.X = rectangleF.Left;
				empty.Y = rectangleF.Y + rectangleF.Height / 2f;
			}
			else if (axis.AxisPosition == AxisPosition.Top)
			{
				empty.X = rectangleF.X + rectangleF.Width / 2f;
				empty.Y = rectangleF.Bottom;
			}
			else if (axis.AxisPosition == AxisPosition.Bottom)
			{
				empty.X = rectangleF.X + rectangleF.Width / 2f;
				empty.Y = rectangleF.Top;
			}
			if ((axis.AxisPosition == AxisPosition.Top || axis.AxisPosition == AxisPosition.Bottom) && angle != 0)
			{
				empty.X = rectangleF.X + rectangleF.Width / 2f;
				empty.Y = ((axis.AxisPosition == AxisPosition.Top) ? rectangleF.Bottom : rectangleF.Y);
				RectangleF empty2 = RectangleF.Empty;
				empty2.X = rectangleF.X + rectangleF.Width / 2f;
				empty2.Y = rectangleF.Y - rectangleF.Width / 2f;
				empty2.Height = rectangleF.Width;
				empty2.Width = rectangleF.Height;
				if (axis.AxisPosition == AxisPosition.Bottom)
				{
					if (angle < 0)
					{
						empty2.X -= empty2.Width;
					}
					stringFormat.Alignment = StringAlignment.Near;
					if (angle < 0)
					{
						stringFormat.Alignment = StringAlignment.Far;
					}
					stringFormat.LineAlignment = StringAlignment.Center;
				}
				if (axis.AxisPosition == AxisPosition.Top)
				{
					empty2.Y += rectangleF.Height;
					if (angle > 0)
					{
						empty2.X -= empty2.Width;
					}
					stringFormat.Alignment = StringAlignment.Far;
					if (angle < 0)
					{
						stringFormat.Alignment = StringAlignment.Near;
					}
					stringFormat.LineAlignment = StringAlignment.Center;
				}
				rectangleF = empty2;
			}
			if ((axis.AxisPosition == AxisPosition.Left || axis.AxisPosition == AxisPosition.Right) && (angle == 90 || angle == -90))
			{
				empty.X = rectangleF.X + rectangleF.Width / 2f;
				empty.Y = rectangleF.Y + rectangleF.Height / 2f;
				RectangleF empty3 = RectangleF.Empty;
				empty3.X = empty.X - rectangleF.Height / 2f;
				empty3.Y = empty.Y - rectangleF.Width / 2f;
				empty3.Height = rectangleF.Width;
				empty3.Width = rectangleF.Height;
				rectangleF = empty3;
				StringAlignment alignment = stringFormat.Alignment;
				stringFormat.Alignment = stringFormat.LineAlignment;
				stringFormat.LineAlignment = alignment;
				if (angle == 90)
				{
					if (stringFormat.LineAlignment == StringAlignment.Far)
					{
						stringFormat.LineAlignment = StringAlignment.Near;
					}
					else if (stringFormat.LineAlignment == StringAlignment.Near)
					{
						stringFormat.LineAlignment = StringAlignment.Far;
					}
				}
				if (angle == -90)
				{
					if (stringFormat.Alignment == StringAlignment.Far)
					{
						stringFormat.Alignment = StringAlignment.Near;
					}
					else if (stringFormat.Alignment == StringAlignment.Near)
					{
						stringFormat.Alignment = StringAlignment.Far;
					}
				}
			}
			Matrix matrix = null;
			if (angle != 0)
			{
				myMatrix = base.Transform.Clone();
				myMatrix.RotateAt(angle, empty);
				matrix = base.Transform;
				base.Transform = myMatrix;
			}
			RectangleF rect = Rectangle.Empty;
			float num = 0f;
			float num2 = 0f;
			if (angle != 0 && angle != 90 && angle != -90)
			{
				sizeF = MeasureString(text.Replace("\\n", "\n"), font, rectangleF.Size, stringFormat);
				rect.Width = sizeF.Width;
				rect.Height = sizeF.Height;
				if (stringFormat.Alignment == StringAlignment.Far)
				{
					rect.X = rectangleF.Right - sizeF.Width;
				}
				else if (stringFormat.Alignment == StringAlignment.Near)
				{
					rect.X = rectangleF.X;
				}
				else if (stringFormat.Alignment == StringAlignment.Center)
				{
					rect.X = rectangleF.X + rectangleF.Width / 2f - sizeF.Width / 2f;
				}
				if (stringFormat.LineAlignment == StringAlignment.Far)
				{
					rect.Y = rectangleF.Bottom - sizeF.Height;
				}
				else if (stringFormat.LineAlignment == StringAlignment.Near)
				{
					rect.Y = rectangleF.Y;
				}
				else if (stringFormat.LineAlignment == StringAlignment.Center)
				{
					rect.Y = rectangleF.Y + rectangleF.Height / 2f - sizeF.Height / 2f;
				}
				num = (float)Math.Sin((double)((float)(90 - angle) / 180f) * Math.PI) * rect.Height / 2f;
				num2 = (float)Math.Sin((double)((float)Math.Abs(angle) / 180f) * Math.PI) * rect.Height / 2f;
				if (axis.AxisPosition == AxisPosition.Left)
				{
					myMatrix.Translate(0f - num2, 0f);
				}
				else if (axis.AxisPosition == AxisPosition.Right)
				{
					myMatrix.Translate(num2, 0f);
				}
				else if (axis.AxisPosition == AxisPosition.Top)
				{
					myMatrix.Translate(0f, 0f - num);
				}
				else if (axis.AxisPosition == AxisPosition.Bottom)
				{
					myMatrix.Translate(0f, num);
				}
				if (boundaryRect != RectangleF.Empty)
				{
					Region region = new Region(rect);
					region.Transform(myMatrix);
					if (axis.AxisPosition == AxisPosition.Left)
					{
						boundaryRect.Width += boundaryRect.X;
						boundaryRect.X = 0f;
					}
					else if (axis.AxisPosition == AxisPosition.Right)
					{
						boundaryRect.Width = (float)common.Width - boundaryRect.X;
					}
					else if (axis.AxisPosition == AxisPosition.Top)
					{
						boundaryRect.Height += boundaryRect.Y;
						boundaryRect.Y = 0f;
					}
					else if (axis.AxisPosition == AxisPosition.Bottom)
					{
						boundaryRect.Height = (float)common.Height - boundaryRect.Y;
					}
					region.Exclude(GetAbsoluteRectangle(boundaryRect));
					if (!region.IsEmpty(Graphics))
					{
						base.Transform = matrix;
						float num3 = region.GetBounds(Graphics).Width / (float)Math.Cos((double)((float)Math.Abs(angle) / 180f) * Math.PI);
						if (axis.AxisPosition == AxisPosition.Left)
						{
							num3 -= rect.Height * (float)Math.Tan((double)((float)Math.Abs(angle) / 180f) * Math.PI);
							rectangleF.Y = rect.Y;
							rectangleF.X = rect.X + num3;
							rectangleF.Width = rect.Width - num3;
							rectangleF.Height = rect.Height;
						}
						else if (axis.AxisPosition == AxisPosition.Right)
						{
							num3 -= rect.Height * (float)Math.Tan((double)((float)Math.Abs(angle) / 180f) * Math.PI);
							rectangleF.Y = rect.Y;
							rectangleF.X = rect.X;
							rectangleF.Width = rect.Width - num3;
							rectangleF.Height = rect.Height;
						}
						else if (axis.AxisPosition == AxisPosition.Top)
						{
							rectangleF.Y = rect.Y;
							rectangleF.X = rect.X;
							rectangleF.Width = rect.Width - num3;
							rectangleF.Height = rect.Height;
							if (angle > 0)
							{
								rectangleF.X += num3;
							}
						}
						else if (axis.AxisPosition == AxisPosition.Bottom)
						{
							rectangleF.Y = rect.Y;
							rectangleF.X = rect.X;
							rectangleF.Width = rect.Width - num3;
							rectangleF.Height = rect.Height;
							if (angle < 0)
							{
								rectangleF.X += num3;
							}
						}
					}
				}
				base.Transform = myMatrix;
			}
			RectangleF rectangleF2 = new RectangleF(rectangleF.Location, rectangleF.Size);
			Image image2 = null;
			SizeF size = default(SizeF);
			if (image.Length > 0)
			{
				ImageLoader.GetAdjustedImageSize(image2, Graphics, ref size);
				rectangleF2.Width -= image2.Size.Width;
				rectangleF2.X += image2.Size.Width;
				if (rectangleF2.Width < 1f)
				{
					rectangleF2.Width = 1f;
				}
			}
			if (labelRowIndex > 0 && labelMark != 0)
			{
				sizeF = MeasureString(text.Replace("\\n", "\n"), font, rectangleF2.Size, stringFormat);
				SizeF labelSize = new SizeF(sizeF.Width, sizeF.Height);
				if (image2 != null)
				{
					labelSize.Width += image2.Width;
				}
				DrawSecondRowLabelMark(axis, markColor, rectangleF, labelSize, labelMark, truncatedLeft, truncatedRight, matrix);
			}
			if ((stringFormat.FormatFlags & StringFormatFlags.LineLimit) != 0)
			{
				stringFormat.FormatFlags ^= StringFormatFlags.LineLimit;
				if (MeasureString("I", font, rectangleF.Size, stringFormat).Height < rectangleF.Height)
				{
					stringFormat.FormatFlags |= StringFormatFlags.LineLimit;
				}
			}
			else
			{
				if ((stringFormat.FormatFlags & StringFormatFlags.NoClip) != 0)
				{
					stringFormat.FormatFlags ^= StringFormatFlags.NoClip;
				}
				SizeF sizeF2 = MeasureString("I", font, rectangleF.Size, stringFormat);
				stringFormat.FormatFlags ^= StringFormatFlags.NoClip;
				if (sizeF2.Height > rectangleF.Height)
				{
					float num4 = sizeF2.Height - rectangleF.Height;
					rectangleF.Y -= num4 / 2f;
					rectangleF.Height += num4;
				}
			}
			DrawString(text.Replace("\\n", "\n"), font, brush, rectangleF2, stringFormat);
			if (commonElements.ProcessModeRegions)
			{
				GraphicsPath graphicsPath = new GraphicsPath();
				graphicsPath.AddRectangle(rectangleF2);
				graphicsPath.Transform(base.Transform);
				string empty4 = string.Empty;
				string empty5 = string.Empty;
				empty4 = label.Href;
				empty5 = label.MapAreaAttributes;
				commonElements.HotRegionsList.AddHotRegion(this, graphicsPath, relativePath: false, label.ToolTip, empty4, empty5, label, ChartElementType.AxisLabels);
			}
			if (image2 != null)
			{
				if (sizeF.IsEmpty)
				{
					sizeF = MeasureString(text.Replace("\\n", "\n"), font, rectangleF2.Size, stringFormat);
				}
				RectangleF rectangleF3 = new RectangleF(rectangleF.X + (rectangleF.Width - (float)image2.Size.Width - sizeF.Width) / 2f, rectangleF.Y + (rectangleF.Height - (float)image2.Size.Height) / 2f, image2.Size.Width, image2.Size.Height);
				if (stringFormat.LineAlignment == StringAlignment.Center)
				{
					rectangleF3.Y = rectangleF.Y + (rectangleF.Height - (float)image2.Size.Height) / 2f;
				}
				else if (stringFormat.LineAlignment == StringAlignment.Far)
				{
					rectangleF3.Y = rectangleF.Bottom - (sizeF.Height + (float)image2.Size.Height) / 2f;
				}
				else if (stringFormat.LineAlignment == StringAlignment.Near)
				{
					rectangleF3.Y = rectangleF.Top + (sizeF.Height - (float)image2.Size.Height) / 2f;
				}
				if (stringFormat.Alignment == StringAlignment.Center)
				{
					rectangleF3.X = rectangleF.X + (rectangleF.Width - (float)image2.Size.Width - sizeF.Width) / 2f;
				}
				else if (stringFormat.Alignment == StringAlignment.Far)
				{
					rectangleF3.X = rectangleF.Right - (float)image2.Size.Width - sizeF.Width;
				}
				else if (stringFormat.Alignment == StringAlignment.Near)
				{
					rectangleF3.X = rectangleF.X;
				}
				ImageAttributes imageAttributes = new ImageAttributes();
				if (imageTranspColor != Color.Empty)
				{
					imageAttributes.SetColorKey(imageTranspColor, imageTranspColor, ColorAdjustType.Default);
				}
				DrawImage(image2, Rectangle.Round(rectangleF3), 0, 0, image2.Width, image2.Height, GraphicsUnit.Pixel, imageAttributes);
				if (commonElements.ProcessModeRegions)
				{
					GraphicsPath graphicsPath2 = new GraphicsPath();
					graphicsPath2.AddRectangle(rectangleF3);
					graphicsPath2.Transform(base.Transform);
					string empty6 = string.Empty;
					string empty7 = string.Empty;
					empty6 = label.ImageHref;
					empty7 = label.ImageMapAreaAttributes;
					commonElements.HotRegionsList.AddHotRegion(this, graphicsPath2, relativePath: false, string.Empty, empty6, empty7, label, ChartElementType.AxisLabelImage);
				}
			}
			if (matrix != null)
			{
				base.Transform = matrix;
			}
		}

		private void DrawSecondRowLabelBoxMark(Axis axis, Color markColor, RectangleF absPosition, SizeF labelSize, bool truncatedLeft, bool truncatedRight, Matrix originalTransform)
		{
			Matrix transform = base.Transform;
			if (originalTransform != null)
			{
				base.Transform = originalTransform;
			}
			PointF value = new PointF(absPosition.X + absPosition.Width / 2f, absPosition.Y + absPosition.Height / 2f);
			Point.Round(value);
			if (axis.AxisPosition == AxisPosition.Left || axis.AxisPosition == AxisPosition.Right)
			{
				RectangleF empty = RectangleF.Empty;
				empty.X = value.X - absPosition.Height / 2f;
				empty.Y = value.Y - absPosition.Width / 2f;
				empty.Height = absPosition.Width;
				empty.Width = absPosition.Height;
				absPosition = empty;
			}
			float num = (float)axis.GetAxisPosition(ignoreCrossing: true);
			PointF relative = new PointF(num, num);
			relative = GetAbsolutePoint(relative);
			Rectangle rectangle = Rectangle.Round(absPosition);
			rectangle.Width = (int)Math.Round(absPosition.Right) - rectangle.X;
			rectangle.Height = (int)Math.Round(absPosition.Bottom) - rectangle.Y;
			Pen pen = new Pen(markColor.IsEmpty ? axis.MajorTickMark.LineColor : markColor, axis.MajorTickMark.LineWidth);
			pen.DashStyle = GetPenStyle(axis.MajorTickMark.LineStyle);
			if (axis.AxisPosition == AxisPosition.Left || axis.AxisPosition == AxisPosition.Right)
			{
				DrawLine(pen, rectangle.Left, rectangle.Top, rectangle.Left, rectangle.Bottom);
				DrawLine(pen, rectangle.Right, rectangle.Top, rectangle.Right, rectangle.Bottom);
			}
			else
			{
				DrawLine(pen, rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Top);
				DrawLine(pen, rectangle.Left, rectangle.Bottom, rectangle.Right, rectangle.Bottom);
			}
			if (!truncatedLeft)
			{
				if (axis.AxisPosition == AxisPosition.Left || axis.AxisPosition == AxisPosition.Right)
				{
					DrawLine(pen, (axis.AxisPosition == AxisPosition.Left) ? rectangle.Left : rectangle.Right, rectangle.Bottom, relative.X, rectangle.Bottom);
				}
				else
				{
					DrawLine(pen, rectangle.Left, (axis.AxisPosition == AxisPosition.Top) ? rectangle.Top : rectangle.Bottom, rectangle.Left, relative.Y);
				}
			}
			if (!truncatedRight)
			{
				if (axis.AxisPosition == AxisPosition.Left || axis.AxisPosition == AxisPosition.Right)
				{
					DrawLine(pen, (axis.AxisPosition == AxisPosition.Left) ? rectangle.Left : rectangle.Right, rectangle.Top, relative.X, rectangle.Top);
				}
				else
				{
					DrawLine(pen, rectangle.Right, (axis.AxisPosition == AxisPosition.Top) ? rectangle.Top : rectangle.Bottom, rectangle.Right, relative.Y);
				}
			}
			pen?.Dispose();
			if (originalTransform != null)
			{
				base.Transform = transform;
			}
		}

		private void DrawSecondRowLabelMark(Axis axis, Color markColor, RectangleF absPosition, SizeF labelSize, LabelMark labelMark, bool truncatedLeft, bool truncatedRight, Matrix oldTransform)
		{
			if (axis.MajorTickMark.LineWidth == 0 || axis.MajorTickMark.LineStyle == ChartDashStyle.NotSet || axis.MajorTickMark.LineColor == Color.Empty)
			{
				return;
			}
			SmoothingMode smoothingMode = base.SmoothingMode;
			base.SmoothingMode = SmoothingMode.None;
			if (labelMark == LabelMark.Box)
			{
				DrawSecondRowLabelBoxMark(axis, markColor, absPosition, labelSize, truncatedLeft, truncatedRight, oldTransform);
			}
			else
			{
				Point point = Point.Round(new PointF(absPosition.X + absPosition.Width / 2f, absPosition.Y + absPosition.Height / 2f));
				Rectangle rectangle = Rectangle.Round(absPosition);
				rectangle.Width = (int)Math.Round(absPosition.Right) - rectangle.X;
				rectangle.Height = (int)Math.Round(absPosition.Bottom) - rectangle.Y;
				PointF[] array = new PointF[3];
				PointF[] array2 = new PointF[3];
				array[0].X = rectangle.Left;
				array[0].Y = rectangle.Bottom;
				array[1].X = rectangle.Left;
				array[1].Y = point.Y;
				array[2].X = (float)Math.Round((double)point.X - (double)(labelSize.Width / 2f) - 1.0);
				array[2].Y = point.Y;
				array2[0].X = rectangle.Right;
				array2[0].Y = rectangle.Bottom;
				array2[1].X = rectangle.Right;
				array2[1].Y = point.Y;
				array2[2].X = (float)Math.Round((double)point.X + (double)(labelSize.Width / 2f) - 1.0);
				array2[2].Y = point.Y;
				if (axis.AxisPosition == AxisPosition.Bottom)
				{
					array[0].Y = rectangle.Top;
					array2[0].Y = rectangle.Top;
				}
				if (labelMark == LabelMark.SideMark)
				{
					array[2] = array[1];
					array2[2] = array2[1];
				}
				if (truncatedLeft)
				{
					array[0] = array[1];
				}
				if (truncatedRight)
				{
					array2[0] = array2[1];
				}
				Pen pen = new Pen(markColor.IsEmpty ? axis.MajorTickMark.LineColor : markColor, axis.MajorTickMark.LineWidth);
				pen.DashStyle = GetPenStyle(axis.MajorTickMark.LineStyle);
				DrawLines(pen, array);
				DrawLines(pen, array2);
				pen?.Dispose();
			}
			base.SmoothingMode = smoothingMode;
		}

		internal SizeF MeasureStringRel(string text, Font font)
		{
			SizeF size = MeasureString(text, font);
			return GetRelativeSize(size);
		}

		internal SizeF MeasureStringRel(string text, Font font, SizeF layoutArea, StringFormat stringFormat)
		{
			SizeF absoluteSize = GetAbsoluteSize(layoutArea);
			SizeF size = MeasureString(text, font, absoluteSize, stringFormat);
			return GetRelativeSize(size);
		}

		internal Size MeasureStringAbs(string text, Font font)
		{
			SizeF sizeF = MeasureString(text, font);
			return new Size((int)Math.Ceiling(sizeF.Width), (int)Math.Ceiling(sizeF.Height));
		}

		internal Size MeasureStringAbs(string text, Font font, SizeF layoutArea, StringFormat stringFormat)
		{
			SizeF sizeF = MeasureString(text, font, layoutArea, stringFormat);
			return new Size((int)Math.Ceiling(sizeF.Width), (int)Math.Ceiling(sizeF.Height));
		}

		internal void DrawStringRel(string text, Font font, Brush brush, RectangleF layoutRectangle, StringFormat format)
		{
			if (layoutRectangle.Width != 0f && layoutRectangle.Height != 0f)
			{
				RectangleF absoluteRectangle = GetAbsoluteRectangle(layoutRectangle);
				DrawString(text, font, brush, absoluteRectangle, format);
			}
		}

		internal void DrawStringRel(string text, Font font, Brush brush, RectangleF layoutRectangle, StringFormat format, int angle)
		{
			PointF empty = PointF.Empty;
			if (layoutRectangle.Width != 0f && layoutRectangle.Height != 0f)
			{
				RectangleF absoluteRectangle = GetAbsoluteRectangle(layoutRectangle);
				SizeF sizeF = MeasureString(text, font, absoluteRectangle.Size, format);
				if (format.Alignment == StringAlignment.Near)
				{
					empty.X = absoluteRectangle.X + sizeF.Width / 2f;
					empty.Y = (absoluteRectangle.Bottom + absoluteRectangle.Top) / 2f;
				}
				else if (format.Alignment == StringAlignment.Far)
				{
					empty.X = absoluteRectangle.Right - sizeF.Width / 2f;
					empty.Y = (absoluteRectangle.Bottom + absoluteRectangle.Top) / 2f;
				}
				else
				{
					empty.X = (absoluteRectangle.Left + absoluteRectangle.Right) / 2f;
					empty.Y = (absoluteRectangle.Bottom + absoluteRectangle.Top) / 2f;
				}
				myMatrix = base.Transform.Clone();
				myMatrix.RotateAt(angle, empty);
				Matrix transform = base.Transform;
				base.Transform = myMatrix;
				DrawString(text, font, brush, absoluteRectangle, format);
				base.Transform = transform;
			}
		}

		internal void DrawRectangleBarStyle(BarDrawingStyle barDrawingStyle, bool isVertical, RectangleF rect, int borderWidth)
		{
			if (barDrawingStyle == BarDrawingStyle.Default || !(rect.Width > 0f) || !(rect.Height > 0f))
			{
				return;
			}
			switch (barDrawingStyle)
			{
			case BarDrawingStyle.Cylinder:
			{
				RectangleF rect2 = rect;
				if (isVertical)
				{
					rect2.Width *= 0.3f;
				}
				else
				{
					rect2.Height *= 0.3f;
				}
				if (rect2.Width > 0f && rect2.Height > 0f)
				{
					FillRectangleAbs(rect2, Color.Transparent, ChartHatchStyle.None, string.Empty, ChartImageWrapMode.Scaled, Color.Empty, ChartImageAlign.Center, isVertical ? GradientType.LeftRight : GradientType.TopBottom, Color.FromArgb(120, Color.White), Color.Empty, 0, ChartDashStyle.NotSet, PenAlignment.Inset);
					if (isVertical)
					{
						rect2.X += rect2.Width + 1f;
						rect2.Width = rect.Right - rect2.X;
					}
					else
					{
						rect2.Y += rect2.Height + 1f;
						rect2.Height = rect.Bottom - rect2.Y;
					}
					FillRectangleAbs(rect2, Color.FromArgb(120, Color.White), ChartHatchStyle.None, string.Empty, ChartImageWrapMode.Scaled, Color.Empty, ChartImageAlign.Center, isVertical ? GradientType.LeftRight : GradientType.TopBottom, Color.FromArgb(150, Color.Black), Color.Empty, 0, ChartDashStyle.NotSet, PenAlignment.Inset);
				}
				break;
			}
			case BarDrawingStyle.Emboss:
			{
				float num2 = 3f;
				if (rect.Width < 6f || rect.Height < 6f)
				{
					num2 = 1f;
				}
				else if (rect.Width < 15f || rect.Height < 15f)
				{
					num2 = 2f;
				}
				using (GraphicsPath graphicsPath4 = new GraphicsPath())
				{
					PointF[] points = new PointF[6]
					{
						new PointF(rect.Left, rect.Bottom),
						new PointF(rect.Left, rect.Top),
						new PointF(rect.Right, rect.Top),
						new PointF(rect.Right - num2, rect.Top + num2),
						new PointF(rect.Left + num2, rect.Top + num2),
						new PointF(rect.Left + num2, rect.Bottom - num2)
					};
					graphicsPath4.AddPolygon(points);
					using (SolidBrush brush4 = new SolidBrush(Color.FromArgb(100, Color.White)))
					{
						FillPath(brush4, graphicsPath4);
					}
				}
				using (GraphicsPath graphicsPath5 = new GraphicsPath())
				{
					PointF[] points2 = new PointF[6]
					{
						new PointF(rect.Right, rect.Top),
						new PointF(rect.Right, rect.Bottom),
						new PointF(rect.Left, rect.Bottom),
						new PointF(rect.Left + num2, rect.Bottom - num2),
						new PointF(rect.Right - num2, rect.Bottom - num2),
						new PointF(rect.Right - num2, rect.Top + num2)
					};
					graphicsPath5.AddPolygon(points2);
					using (SolidBrush brush5 = new SolidBrush(Color.FromArgb(80, Color.Black)))
					{
						FillPath(brush5, graphicsPath5);
					}
				}
				break;
			}
			case BarDrawingStyle.LightToDark:
			{
				float num3 = 4f;
				if (rect.Width < 6f || rect.Height < 6f)
				{
					num3 = 2f;
				}
				else if (rect.Width < 15f || rect.Height < 15f)
				{
					num3 = 3f;
				}
				RectangleF rect3 = rect;
				rect3.Inflate(0f - num3, 0f - num3);
				if (isVertical)
				{
					rect3.Height = (float)Math.Floor(rect3.Height / 3f);
				}
				else
				{
					rect3.X = rect3.Right - (float)Math.Floor(rect3.Width / 3f);
					rect3.Width = (float)Math.Floor(rect3.Width / 3f);
				}
				if (rect3.Width > 0f && rect3.Height > 0f)
				{
					FillRectangleAbs(rect3, isVertical ? Color.FromArgb(120, Color.White) : Color.Transparent, ChartHatchStyle.None, string.Empty, ChartImageWrapMode.Scaled, Color.Empty, ChartImageAlign.Center, (!isVertical) ? GradientType.LeftRight : GradientType.TopBottom, isVertical ? Color.Transparent : Color.FromArgb(120, Color.White), Color.Empty, 0, ChartDashStyle.NotSet, PenAlignment.Inset);
					rect3 = rect;
					rect3.Inflate(0f - num3, 0f - num3);
					if (isVertical)
					{
						rect3.Y = rect3.Bottom - (float)Math.Floor(rect3.Height / 3f);
						rect3.Height = (float)Math.Floor(rect3.Height / 3f);
					}
					else
					{
						rect3.Width = (float)Math.Floor(rect3.Width / 3f);
					}
					FillRectangleAbs(rect3, (!isVertical) ? Color.FromArgb(80, Color.Black) : Color.Transparent, ChartHatchStyle.None, string.Empty, ChartImageWrapMode.Scaled, Color.Empty, ChartImageAlign.Center, (!isVertical) ? GradientType.LeftRight : GradientType.TopBottom, (!isVertical) ? Color.Transparent : Color.FromArgb(80, Color.Black), Color.Empty, 0, ChartDashStyle.NotSet, PenAlignment.Inset);
				}
				break;
			}
			case BarDrawingStyle.Wedge:
			{
				float num = isVertical ? (rect.Width / 2f) : (rect.Height / 2f);
				if (isVertical && 2f * num > rect.Height)
				{
					num = rect.Height / 2f;
				}
				if (!isVertical && 2f * num > rect.Width)
				{
					num = rect.Width / 2f;
				}
				RectangleF rectangleF = rect;
				using (GraphicsPath graphicsPath = new GraphicsPath())
				{
					if (isVertical)
					{
						graphicsPath.AddLine(rectangleF.X + rectangleF.Width / 2f, rectangleF.Y + num, rectangleF.X + rectangleF.Width / 2f, rectangleF.Bottom - num);
						graphicsPath.AddLine(rectangleF.X + rectangleF.Width / 2f, rectangleF.Bottom - num, rectangleF.Right, rectangleF.Bottom);
						graphicsPath.AddLine(rectangleF.Right, rectangleF.Bottom, rectangleF.Right, rectangleF.Y);
					}
					else
					{
						graphicsPath.AddLine(rectangleF.X + num, rectangleF.Y + rectangleF.Height / 2f, rectangleF.Right - num, rectangleF.Y + rectangleF.Height / 2f);
						graphicsPath.AddLine(rectangleF.Right - num, rectangleF.Y + rectangleF.Height / 2f, rectangleF.Right, rectangleF.Bottom);
						graphicsPath.AddLine(rectangleF.Right, rectangleF.Bottom, rectangleF.Left, rectangleF.Bottom);
					}
					graphicsPath.CloseAllFigures();
					using (SolidBrush brush = new SolidBrush(Color.FromArgb(90, Color.Black)))
					{
						FillPath(brush, graphicsPath);
					}
				}
				using (GraphicsPath graphicsPath2 = new GraphicsPath())
				{
					if (isVertical)
					{
						graphicsPath2.AddLine(rectangleF.X, rectangleF.Y, rectangleF.X + rectangleF.Width / 2f, rectangleF.Y + num);
						graphicsPath2.AddLine(rectangleF.X + rectangleF.Width / 2f, rectangleF.Y + num, rectangleF.Right, rectangleF.Y);
					}
					else
					{
						graphicsPath2.AddLine(rectangleF.Right, rectangleF.Y, rectangleF.Right - num, rectangleF.Y + rectangleF.Height / 2f);
						graphicsPath2.AddLine(rectangleF.Right - num, rectangleF.Y + rectangleF.Height / 2f, rectangleF.Right, rectangleF.Bottom);
					}
					using (SolidBrush brush2 = new SolidBrush(Color.FromArgb(50, Color.Black)))
					{
						FillPath(brush2, graphicsPath2);
						using (Pen pen = new Pen(Color.FromArgb(20, Color.Black), 1f))
						{
							DrawPath(pen, graphicsPath2);
							if (isVertical)
							{
								DrawLine(pen, rect.X + rect.Width / 2f, rect.Y + num, rect.X + rect.Width / 2f, rect.Bottom - num);
							}
							else
							{
								DrawLine(pen, rect.X + num, rect.Y + rect.Height / 2f, rect.X + num, rect.Bottom - rect.Height / 2f);
							}
						}
						using (Pen pen2 = new Pen(Color.FromArgb(40, Color.White), 1f))
						{
							DrawPath(pen2, graphicsPath2);
							if (isVertical)
							{
								DrawLine(pen2, rect.X + rect.Width / 2f, rect.Y + num, rect.X + rect.Width / 2f, rect.Bottom - num);
							}
							else
							{
								DrawLine(pen2, rect.X + num, rect.Y + rect.Height / 2f, rect.X + num, rect.Bottom - rect.Height / 2f);
							}
						}
					}
				}
				using (GraphicsPath graphicsPath3 = new GraphicsPath())
				{
					if (isVertical)
					{
						graphicsPath3.AddLine(rectangleF.X, rectangleF.Bottom, rectangleF.X + rectangleF.Width / 2f, rectangleF.Bottom - num);
						graphicsPath3.AddLine(rectangleF.X + rectangleF.Width / 2f, rectangleF.Bottom - num, rectangleF.Right, rectangleF.Bottom);
					}
					else
					{
						graphicsPath3.AddLine(rectangleF.X, rectangleF.Y, rectangleF.X + num, rectangleF.Y + rectangleF.Height / 2f);
						graphicsPath3.AddLine(rectangleF.X + num, rectangleF.Y + rectangleF.Height / 2f, rectangleF.X, rectangleF.Bottom);
					}
					using (SolidBrush brush3 = new SolidBrush(Color.FromArgb(50, Color.Black)))
					{
						FillPath(brush3, graphicsPath3);
						using (Pen pen3 = new Pen(Color.FromArgb(20, Color.Black), 1f))
						{
							DrawPath(pen3, graphicsPath3);
						}
						using (Pen pen4 = new Pen(Color.FromArgb(40, Color.White), 1f))
						{
							DrawPath(pen4, graphicsPath3);
						}
					}
				}
				break;
			}
			}
		}

		internal void FillRectangleRel(RectangleF rectF, Color backColor, ChartHatchStyle backHatchStyle, string backImage, ChartImageWrapMode backImageMode, Color backImageTranspColor, ChartImageAlign backImageAlign, GradientType backGradientType, Color backGradientEndColor, Color borderColor, int borderWidth, ChartDashStyle borderStyle, Color shadowColor, int shadowOffset, PenAlignment penAlignment, BarDrawingStyle barDrawingStyle, bool isVertical)
		{
			FillRectangleRel(rectF, backColor, backHatchStyle, backImage, backImageMode, backImageTranspColor, backImageAlign, backGradientType, backGradientEndColor, borderColor, borderWidth, borderStyle, shadowColor, shadowOffset, penAlignment, circular: false, 0, circle3D: false, barDrawingStyle, isVertical);
		}

		internal void FillRectangleRel(RectangleF rectF, Color backColor, ChartHatchStyle backHatchStyle, string backImage, ChartImageWrapMode backImageMode, Color backImageTranspColor, ChartImageAlign backImageAlign, GradientType backGradientType, Color backGradientEndColor, Color borderColor, int borderWidth, ChartDashStyle borderStyle, Color shadowColor, int shadowOffset, PenAlignment penAlignment)
		{
			FillRectangleRel(rectF, backColor, backHatchStyle, backImage, backImageMode, backImageTranspColor, backImageAlign, backGradientType, backGradientEndColor, borderColor, borderWidth, borderStyle, shadowColor, shadowOffset, penAlignment, circular: false, 0, circle3D: false, BarDrawingStyle.Default, isVertical: true);
		}

		internal void FillRectangleRel(RectangleF rectF, Color backColor, ChartHatchStyle backHatchStyle, string backImage, ChartImageWrapMode backImageMode, Color backImageTranspColor, ChartImageAlign backImageAlign, GradientType backGradientType, Color backGradientEndColor, Color borderColor, int borderWidth, ChartDashStyle borderStyle, Color shadowColor, int shadowOffset, PenAlignment penAlignment, bool circular, int circularSectorsCount, bool circle3D)
		{
			FillRectangleRel(rectF, backColor, backHatchStyle, backImage, backImageMode, backImageTranspColor, backImageAlign, backGradientType, backGradientEndColor, borderColor, borderWidth, borderStyle, shadowColor, shadowOffset, penAlignment, circular, circularSectorsCount, circle3D, BarDrawingStyle.Default, isVertical: true);
		}

		internal void FillRectangleRel(RectangleF rectF, Color backColor, ChartHatchStyle backHatchStyle, string backImage, ChartImageWrapMode backImageMode, Color backImageTranspColor, ChartImageAlign backImageAlign, GradientType backGradientType, Color backGradientEndColor, Color borderColor, int borderWidth, ChartDashStyle borderStyle, Color shadowColor, int shadowOffset, PenAlignment penAlignment, bool circular, int circularSectorsCount, bool circle3D, BarDrawingStyle barDrawingStyle, bool isVertical)
		{
			Brush brush = null;
			Brush brush2 = null;
			SmoothingMode smoothingMode = base.SmoothingMode;
			if (!circular)
			{
				base.SmoothingMode = SmoothingMode.Default;
			}
			if (backColor.IsEmpty)
			{
				backColor = Color.White;
			}
			if (backGradientEndColor.IsEmpty)
			{
				backGradientEndColor = Color.White;
			}
			if (borderColor.IsEmpty || borderStyle == ChartDashStyle.NotSet)
			{
				borderWidth = 0;
			}
			RectangleF absoluteRectangle = GetAbsoluteRectangle(rectF);
			if (absoluteRectangle.Width < 1f && absoluteRectangle.Width > 0f)
			{
				absoluteRectangle.Width = 1f;
			}
			if (absoluteRectangle.Height < 1f && absoluteRectangle.Height > 0f)
			{
				absoluteRectangle.Height = 1f;
			}
			absoluteRectangle = Round(absoluteRectangle);
			RectangleF rectangleF = (penAlignment != PenAlignment.Inset || borderWidth <= 0) ? absoluteRectangle : ((base.ActiveRenderingType != RenderingType.Svg && !IsMetafile) ? ((Graphics.Transform.Elements[0] == 1f && Graphics.Transform.Elements[3] == 1f) ? new RectangleF(absoluteRectangle.X + (float)borderWidth, absoluteRectangle.Y + (float)borderWidth, absoluteRectangle.Width - (float)borderWidth * 2f + 1f, absoluteRectangle.Height - (float)borderWidth * 2f + 1f) : new RectangleF(absoluteRectangle.X, absoluteRectangle.Y, absoluteRectangle.Width, absoluteRectangle.Height)) : new RectangleF(absoluteRectangle.X, absoluteRectangle.Y, absoluteRectangle.Width, absoluteRectangle.Height));
			if (rectangleF.Width > 2f * (float)width)
			{
				rectangleF.Width = 2f * (float)width;
			}
			if (rectangleF.Height > 2f * (float)height)
			{
				rectangleF.Height = 2f * (float)height;
			}
			if (backImage.Length <= 0 || backImageMode == ChartImageWrapMode.Unscaled || backImageMode == ChartImageWrapMode.Scaled)
			{
				brush = ((backHatchStyle != 0) ? GetHatchBrush(backHatchStyle, backColor, backGradientEndColor) : ((backGradientType != 0) ? GetGradientBrush(absoluteRectangle, backColor, backGradientEndColor, backGradientType) : ((!(backColor == Color.Empty) && !(backColor == Color.Transparent)) ? new SolidBrush(backColor) : null)));
			}
			else
			{
				if (backColor != Color.Empty && backColor != Color.Transparent)
				{
					brush2 = new SolidBrush(backColor);
				}
				brush = GetTextureBrush(backImage, backImageTranspColor, backImageMode, backColor);
			}
			FillRectangleShadowAbs(absoluteRectangle, shadowColor, shadowOffset, backColor, circular, circularSectorsCount);
			if (backImage.Length > 0 && (backImageMode == ChartImageWrapMode.Unscaled || backImageMode == ChartImageWrapMode.Scaled))
			{
				Image image = common.ImageLoader.LoadImage(backImage);
				ImageAttributes imageAttributes = new ImageAttributes();
				if (backImageTranspColor != Color.Empty)
				{
					imageAttributes.SetColorKey(backImageTranspColor, backImageTranspColor, ColorAdjustType.Default);
				}
				RectangleF rectangleF2 = default(RectangleF);
				rectangleF2.X = rectangleF.X;
				rectangleF2.Y = rectangleF.Y;
				rectangleF2.Width = rectangleF.Width;
				rectangleF2.Height = rectangleF.Height;
				if (backImageMode == ChartImageWrapMode.Unscaled)
				{
					SizeF size = default(SizeF);
					ImageLoader.GetAdjustedImageSize(image, Graphics, ref size);
					rectangleF2.Width = Math.Min(rectangleF.Width, size.Width);
					rectangleF2.Height = Math.Min(rectangleF.Height, size.Height);
					if (rectangleF2.Width < rectangleF.Width)
					{
						switch (backImageAlign)
						{
						case ChartImageAlign.TopRight:
						case ChartImageAlign.Right:
						case ChartImageAlign.BottomRight:
							rectangleF2.X = rectangleF.Right - rectangleF2.Width;
							break;
						case ChartImageAlign.Top:
						case ChartImageAlign.Bottom:
						case ChartImageAlign.Center:
							rectangleF2.X = rectangleF.X + (rectangleF.Width - rectangleF2.Width) / 2f;
							break;
						}
					}
					if (rectangleF2.Height < rectangleF.Height)
					{
						switch (backImageAlign)
						{
						case ChartImageAlign.BottomRight:
						case ChartImageAlign.Bottom:
						case ChartImageAlign.BottomLeft:
							rectangleF2.Y = rectangleF.Bottom - rectangleF2.Height;
							break;
						case ChartImageAlign.Right:
						case ChartImageAlign.Left:
						case ChartImageAlign.Center:
							rectangleF2.Y = rectangleF.Y + (rectangleF.Height - rectangleF2.Height) / 2f;
							break;
						}
					}
				}
				if (brush != null)
				{
					if (circular)
					{
						DrawCircleAbs(null, brush, rectangleF, circularSectorsCount, circle3D);
					}
					else
					{
						FillRectangle(brush, rectangleF);
					}
				}
				DrawImage(image, new Rectangle((int)Math.Round(rectangleF2.X), (int)Math.Round(rectangleF2.Y), (int)Math.Round(rectangleF2.Width), (int)Math.Round(rectangleF2.Height)), 0f, 0f, (backImageMode == ChartImageWrapMode.Unscaled) ? rectangleF2.Width : ((float)image.Width), (backImageMode == ChartImageWrapMode.Unscaled) ? rectangleF2.Height : ((float)image.Height), GraphicsUnit.Pixel, imageAttributes);
			}
			else
			{
				if (brush2 != null && backImageTranspColor != Color.Empty)
				{
					if (circular)
					{
						DrawCircleAbs(null, brush2, rectangleF, circularSectorsCount, circle3D);
					}
					else
					{
						FillRectangle(brush2, rectangleF);
					}
				}
				if (brush != null)
				{
					if (circular)
					{
						DrawCircleAbs(null, brush, rectangleF, circularSectorsCount, circle3D);
					}
					else
					{
						FillRectangle(brush, rectangleF);
					}
				}
			}
			DrawRectangleBarStyle(barDrawingStyle, isVertical, rectangleF, (borderStyle != 0) ? borderWidth : 0);
			if (borderWidth > 0 && borderStyle != 0)
			{
				if (pen.Color != borderColor)
				{
					pen.Color = borderColor;
				}
				if (pen.Width != (float)borderWidth)
				{
					pen.Width = borderWidth;
				}
				if (pen.Alignment != penAlignment)
				{
					pen.Alignment = penAlignment;
				}
				if (pen.DashStyle != GetPenStyle(borderStyle))
				{
					pen.DashStyle = GetPenStyle(borderStyle);
				}
				if (circular)
				{
					DrawCircleAbs(pen, null, absoluteRectangle, circularSectorsCount, circle3D: false);
				}
				else
				{
					if (pen.Alignment == PenAlignment.Inset && pen.Width > 1f)
					{
						absoluteRectangle.Width += 1f;
						absoluteRectangle.Height += 1f;
					}
					DrawRectangle(pen, absoluteRectangle.X, absoluteRectangle.Y, absoluteRectangle.Width, absoluteRectangle.Height);
				}
			}
			brush?.Dispose();
			base.SmoothingMode = smoothingMode;
		}

		public void FillRectangleShadowAbs(RectangleF rect, Color shadowColor, float shadowOffset, Color backColor)
		{
			FillRectangleShadowAbs(rect, shadowColor, shadowOffset, backColor, circular: false, 0);
		}

		internal void FillRectangleShadowAbs(RectangleF rect, Color shadowColor, float shadowOffset, Color backColor, bool circular, int circularSectorsCount)
		{
			if (rect.Height == 0f || rect.Width == 0f || shadowOffset == 0f || shadowOffset == 0f || shadowColor == Color.Empty)
			{
				return;
			}
			bool flag = false;
			Region clip = null;
			if (!circular && backColor == Color.Transparent)
			{
				flag = true;
				clip = base.Clip;
				Region region = new Region();
				region.MakeInfinite();
				region.Xor(rect);
				base.Clip = region;
			}
			if (!softShadows || circularSectorsCount > 2)
			{
				RectangleF empty = RectangleF.Empty;
				RectangleF rectangleF = Round(rect);
				SolidBrush brush = new SolidBrush((shadowColor.A != byte.MaxValue) ? shadowColor : Color.FromArgb((int)backColor.A / 2, shadowColor));
				empty.X = rectangleF.X + shadowOffset;
				empty.Y = rectangleF.Y + shadowOffset;
				empty.Width = rectangleF.Width;
				empty.Height = rectangleF.Height;
				shadowDrawingMode = true;
				if (circular)
				{
					DrawCircleAbs(null, brush, empty, circularSectorsCount, circle3D: false);
				}
				else
				{
					FillRectangle(brush, empty);
				}
				shadowDrawingMode = false;
			}
			else
			{
				RectangleF empty2 = RectangleF.Empty;
				RectangleF rectangleF2 = Round(rect);
				empty2.X = rectangleF2.X + shadowOffset - 1f;
				empty2.Y = rectangleF2.Y + shadowOffset - 1f;
				empty2.Width = rectangleF2.Width + 2f;
				empty2.Height = rectangleF2.Height + 2f;
				float val = shadowOffset * 0.7f;
				val = Math.Max(val, 2f);
				val = Math.Min(val, empty2.Width / 4f);
				val = Math.Min(val, empty2.Height / 4f);
				val = (float)Math.Ceiling(val);
				if (circular)
				{
					val = empty2.Width / 2f;
				}
				GraphicsPath graphicsPath = new GraphicsPath();
				if (circular && empty2.Width != empty2.Height)
				{
					float num = empty2.Width / 2f;
					float num2 = empty2.Height / 2f;
					graphicsPath.AddLine(empty2.X + num, empty2.Y, empty2.Right - num, empty2.Y);
					graphicsPath.AddArc(empty2.Right - 2f * num, empty2.Y, 2f * num, 2f * num2, 270f, 90f);
					graphicsPath.AddLine(empty2.Right, empty2.Y + num2, empty2.Right, empty2.Bottom - num2);
					graphicsPath.AddArc(empty2.Right - 2f * num, empty2.Bottom - 2f * num2, 2f * num, 2f * num2, 0f, 90f);
					graphicsPath.AddLine(empty2.Right - num, empty2.Bottom, empty2.X + num, empty2.Bottom);
					graphicsPath.AddArc(empty2.X, empty2.Bottom - 2f * num2, 2f * num, 2f * num2, 90f, 90f);
					graphicsPath.AddLine(empty2.X, empty2.Bottom - num2, empty2.X, empty2.Y + num2);
					graphicsPath.AddArc(empty2.X, empty2.Y, 2f * num, 2f * num2, 180f, 90f);
				}
				else
				{
					graphicsPath.AddLine(empty2.X + val, empty2.Y, empty2.Right - val, empty2.Y);
					graphicsPath.AddArc(empty2.Right - 2f * val, empty2.Y, 2f * val, 2f * val, 270f, 90f);
					graphicsPath.AddLine(empty2.Right, empty2.Y + val, empty2.Right, empty2.Bottom - val);
					graphicsPath.AddArc(empty2.Right - 2f * val, empty2.Bottom - 2f * val, 2f * val, 2f * val, 0f, 90f);
					graphicsPath.AddLine(empty2.Right - val, empty2.Bottom, empty2.X + val, empty2.Bottom);
					graphicsPath.AddArc(empty2.X, empty2.Bottom - 2f * val, 2f * val, 2f * val, 90f, 90f);
					graphicsPath.AddLine(empty2.X, empty2.Bottom - val, empty2.X, empty2.Y + val);
					graphicsPath.AddArc(empty2.X, empty2.Y, 2f * val, 2f * val, 180f, 90f);
				}
				PathGradientBrush pathGradientBrush = new PathGradientBrush(graphicsPath);
				pathGradientBrush.CenterColor = shadowColor;
				Color[] array2 = pathGradientBrush.SurroundColors = new Color[1]
				{
					Color.Transparent
				};
				pathGradientBrush.CenterPoint = new PointF(empty2.X + empty2.Width / 2f, empty2.Y + empty2.Height / 2f);
				PointF focusScales = new PointF(1f - 2f * shadowOffset / empty2.Width, 1f - 2f * shadowOffset / empty2.Height);
				if (focusScales.X < 0f)
				{
					focusScales.X = 0f;
				}
				if (focusScales.Y < 0f)
				{
					focusScales.Y = 0f;
				}
				pathGradientBrush.FocusScales = focusScales;
				shadowDrawingMode = true;
				FillPath(pathGradientBrush, graphicsPath);
				shadowDrawingMode = false;
			}
			if (flag)
			{
				Region clip2 = base.Clip;
				base.Clip = clip;
				clip2.Dispose();
			}
		}

		internal GraphicsPath GetPolygonCirclePath(RectangleF position, int polygonSectorsNumber)
		{
			PointF pointF = new PointF(position.X + position.Width / 2f, position.Y);
			PointF point = new PointF(position.X + position.Width / 2f, position.Y + position.Height / 2f);
			float num = 0f;
			GraphicsPath graphicsPath = new GraphicsPath();
			PointF pt = PointF.Empty;
			float num2 = 0f;
			num = ((polygonSectorsNumber > 2) ? (360f / (float)polygonSectorsNumber) : 1f);
			for (num2 = 0f; num2 < 360f; num2 += num)
			{
				Matrix matrix = new Matrix();
				matrix.RotateAt(num2, point);
				PointF[] array = new PointF[1]
				{
					pointF
				};
				matrix.TransformPoints(array);
				if (!pt.IsEmpty)
				{
					graphicsPath.AddLine(pt, array[0]);
				}
				pt = array[0];
			}
			graphicsPath.CloseAllFigures();
			return graphicsPath;
		}

		internal void DrawCircleAbs(Pen pen, Brush brush, RectangleF position, int polygonSectorsNumber, bool circle3D)
		{
			bool flag = circle3D && brush != null;
			if (polygonSectorsNumber <= 2 && !flag)
			{
				if (brush != null)
				{
					FillEllipse(brush, position);
				}
				if (pen != null)
				{
					DrawEllipse(pen, position);
				}
				return;
			}
			PointF pointF = new PointF(position.X + position.Width / 2f, position.Y);
			PointF pointF2 = new PointF(position.X + position.Width / 2f, position.Y + position.Height / 2f);
			float num = 0f;
			GraphicsPath graphicsPath = new GraphicsPath();
			PointF pointF3 = PointF.Empty;
			float num2 = 0f;
			SmoothingMode smoothingMode = base.SmoothingMode;
			if (flag)
			{
				base.SmoothingMode = SmoothingMode.None;
			}
			num = ((polygonSectorsNumber > 2) ? (360f / (float)polygonSectorsNumber) : 1f);
			for (num2 = 0f; num2 < 360f; num2 += num)
			{
				Matrix matrix = new Matrix();
				matrix.RotateAt(num2, pointF2);
				PointF[] array = new PointF[1]
				{
					pointF
				};
				matrix.TransformPoints(array);
				if (!pointF3.IsEmpty)
				{
					graphicsPath.AddLine(pointF3, array[0]);
					if (flag)
					{
						graphicsPath.AddLine(array[0], pointF2);
						graphicsPath.AddLine(pointF2, pointF3);
						FillPath(GetSector3DBrush(brush, num2, num), graphicsPath);
						graphicsPath.Reset();
					}
				}
				pointF3 = array[0];
			}
			graphicsPath.CloseAllFigures();
			if (!pointF3.IsEmpty && flag)
			{
				graphicsPath.AddLine(pointF3, pointF);
				graphicsPath.AddLine(pointF, pointF2);
				graphicsPath.AddLine(pointF2, pointF3);
				FillPath(GetSector3DBrush(brush, num2, num), graphicsPath);
				graphicsPath.Reset();
			}
			if (flag)
			{
				base.SmoothingMode = smoothingMode;
			}
			if (brush != null && !circle3D)
			{
				FillPath(brush, graphicsPath);
			}
			if (pen != null)
			{
				DrawPath(pen, graphicsPath);
			}
		}

		internal Brush GetSector3DBrush(Brush brush, float curentSector, float sectorSize)
		{
			Color beginColor = Color.Gray;
			if (brush is HatchBrush)
			{
				beginColor = ((HatchBrush)brush).BackgroundColor;
			}
			else if (brush is LinearGradientBrush)
			{
				beginColor = ((LinearGradientBrush)brush).LinearColors[0];
			}
			else if (brush is PathGradientBrush)
			{
				beginColor = ((PathGradientBrush)brush).CenterColor;
			}
			else if (brush is SolidBrush)
			{
				beginColor = ((SolidBrush)brush).Color;
			}
			curentSector -= sectorSize / 2f;
			if (sectorSize == 72f && curentSector == 180f)
			{
				curentSector *= 0.8f;
			}
			if (curentSector > 180f)
			{
				curentSector = 360f - curentSector;
			}
			curentSector /= 180f;
			beginColor = GetBrightGradientColor(beginColor, curentSector);
			return new SolidBrush(beginColor);
		}

		internal Color GetBrightGradientColor(Color beginColor, double position)
		{
			double num = 0.5;
			if (position < num)
			{
				return GetGradientColor(Color.FromArgb(beginColor.A, 255, 255, 255), beginColor, 1.0 - num + position);
			}
			if (0.0 - num + position < 1.0)
			{
				return GetGradientColor(beginColor, Color.Black, 0.0 - num + position);
			}
			return Color.FromArgb(beginColor.A, 0, 0, 0);
		}

		internal void FillRectangleAbs(RectangleF rect, Color backColor, ChartHatchStyle backHatchStyle, string backImage, ChartImageWrapMode backImageMode, Color backImageTranspColor, ChartImageAlign backImageAlign, GradientType backGradientType, Color backGradientEndColor, Color borderColor, int borderWidth, ChartDashStyle borderStyle, PenAlignment penAlignment)
		{
			Brush brush = null;
			Brush brush2 = null;
			SmoothingMode smoothingMode = base.SmoothingMode;
			base.SmoothingMode = SmoothingMode.None;
			if (backColor.IsEmpty)
			{
				backColor = Color.White;
			}
			if (backGradientEndColor.IsEmpty)
			{
				backGradientEndColor = Color.White;
			}
			if (borderColor.IsEmpty)
			{
				borderColor = Color.White;
				borderWidth = 0;
			}
			pen.Color = borderColor;
			pen.Width = borderWidth;
			pen.Alignment = penAlignment;
			pen.DashStyle = GetPenStyle(borderStyle);
			if (backGradientType == GradientType.None)
			{
				solidBrush.Color = backColor;
				brush = solidBrush;
			}
			else
			{
				brush = GetGradientBrush(rect, backColor, backGradientEndColor, backGradientType);
			}
			if (backHatchStyle != 0)
			{
				brush = GetHatchBrush(backHatchStyle, backColor, backGradientEndColor);
			}
			if (backImage.Length > 0 && backImageMode != ChartImageWrapMode.Unscaled && backImageMode != ChartImageWrapMode.Scaled)
			{
				brush2 = brush;
				brush = GetTextureBrush(backImage, backImageTranspColor, backImageMode, backColor);
			}
			RectangleF rectangleF = new RectangleF(rect.X + (float)borderWidth, rect.Y + (float)borderWidth, rect.Width - (float)(borderWidth * 2), rect.Height - (float)(borderWidth * 2));
			rectangleF.Width += 1f;
			rectangleF.Height += 1f;
			if (backImage.Length > 0 && (backImageMode == ChartImageWrapMode.Unscaled || backImageMode == ChartImageWrapMode.Scaled))
			{
				Image image = common.ImageLoader.LoadImage(backImage);
				ImageAttributes imageAttributes = new ImageAttributes();
				if (backImageTranspColor != Color.Empty)
				{
					imageAttributes.SetColorKey(backImageTranspColor, backImageTranspColor, ColorAdjustType.Default);
				}
				RectangleF rectangleF2 = default(RectangleF);
				rectangleF2.X = rectangleF.X;
				rectangleF2.Y = rectangleF.Y;
				rectangleF2.Width = rectangleF.Width;
				rectangleF2.Height = rectangleF.Height;
				if (backImageMode == ChartImageWrapMode.Unscaled)
				{
					SizeF size = default(SizeF);
					ImageLoader.GetAdjustedImageSize(image, Graphics, ref size);
					rectangleF2.Width = size.Width;
					rectangleF2.Height = size.Height;
					if (rectangleF2.Width < rectangleF.Width)
					{
						switch (backImageAlign)
						{
						case ChartImageAlign.TopRight:
						case ChartImageAlign.Right:
						case ChartImageAlign.BottomRight:
							rectangleF2.X = rectangleF.Right - rectangleF2.Width;
							break;
						case ChartImageAlign.Top:
						case ChartImageAlign.Bottom:
						case ChartImageAlign.Center:
							rectangleF2.X = rectangleF.X + (rectangleF.Width - rectangleF2.Width) / 2f;
							break;
						}
					}
					if (rectangleF2.Height < rectangleF.Height)
					{
						switch (backImageAlign)
						{
						case ChartImageAlign.BottomRight:
						case ChartImageAlign.Bottom:
						case ChartImageAlign.BottomLeft:
							rectangleF2.Y = rectangleF.Bottom - rectangleF2.Height;
							break;
						case ChartImageAlign.Right:
						case ChartImageAlign.Left:
						case ChartImageAlign.Center:
							rectangleF2.Y = rectangleF.Y + (rectangleF.Height - rectangleF2.Height) / 2f;
							break;
						}
					}
				}
				FillRectangle(brush, rect.X, rect.Y, rect.Width + 1f, rect.Height + 1f);
				DrawImage(image, new Rectangle((int)Math.Round(rectangleF2.X), (int)Math.Round(rectangleF2.Y), (int)Math.Round(rectangleF2.Width), (int)Math.Round(rectangleF2.Height)), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
			}
			else
			{
				if (brush2 != null && backImageTranspColor != Color.Empty)
				{
					FillRectangle(brush2, rect.X, rect.Y, rect.Width + 1f, rect.Height + 1f);
				}
				FillRectangle(brush, rect.X, rect.Y, rect.Width + 1f, rect.Height + 1f);
			}
			if (borderStyle != 0)
			{
				if (borderWidth > 1)
				{
					DrawRectangle(pen, rect.X, rect.Y, rect.Width + 1f, rect.Height + 1f);
				}
				else if (borderWidth == 1)
				{
					DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
				}
			}
			if (backGradientType != 0)
			{
				brush.Dispose();
			}
			if (backImage.Length > 0 && backImageMode != ChartImageWrapMode.Unscaled && backImageMode != ChartImageWrapMode.Scaled)
			{
				brush.Dispose();
			}
			if (backHatchStyle != 0)
			{
				brush.Dispose();
			}
			base.SmoothingMode = smoothingMode;
		}

		internal void DrawPathAbs(GraphicsPath path, Color backColor, ChartHatchStyle backHatchStyle, string backImage, ChartImageWrapMode backImageMode, Color backImageTranspColor, ChartImageAlign backImageAlign, GradientType backGradientType, Color backGradientEndColor, Color borderColor, int borderWidth, ChartDashStyle borderStyle, PenAlignment penAlignment, int shadowOffset, Color shadowColor)
		{
			if (shadowOffset != 0 && shadowColor != Color.Transparent)
			{
				GraphicsState gstate = Save();
				TranslateTransform(shadowOffset, shadowOffset);
				if (backColor == Color.Transparent && backGradientEndColor.IsEmpty)
				{
					DrawPathAbs(path, Color.Transparent, ChartHatchStyle.None, string.Empty, ChartImageWrapMode.Scaled, Color.Empty, ChartImageAlign.Center, GradientType.None, Color.Empty, shadowColor, borderWidth, borderStyle, PenAlignment.Center);
				}
				else
				{
					DrawPathAbs(path, shadowColor, ChartHatchStyle.None, string.Empty, ChartImageWrapMode.Scaled, Color.Empty, ChartImageAlign.Center, GradientType.None, Color.Empty, Color.Transparent, 0, ChartDashStyle.NotSet, PenAlignment.Center);
				}
				Restore(gstate);
			}
			DrawPathAbs(path, backColor, backHatchStyle, backImage, backImageMode, backImageTranspColor, backImageAlign, backGradientType, backGradientEndColor, borderColor, borderWidth, borderStyle, penAlignment);
		}

		internal void DrawPathAbs(GraphicsPath path, Color backColor, ChartHatchStyle backHatchStyle, string backImage, ChartImageWrapMode backImageMode, Color backImageTranspColor, ChartImageAlign backImageAlign, GradientType backGradientType, Color backGradientEndColor, Color borderColor, int borderWidth, ChartDashStyle borderStyle, PenAlignment penAlignment)
		{
			Brush brush = null;
			Brush brush2 = null;
			if (backColor.IsEmpty)
			{
				backColor = Color.White;
			}
			if (backGradientEndColor.IsEmpty)
			{
				backGradientEndColor = Color.White;
			}
			if (borderColor.IsEmpty)
			{
				borderColor = Color.White;
				borderWidth = 0;
			}
			pen.Color = borderColor;
			pen.Width = borderWidth;
			pen.Alignment = penAlignment;
			pen.DashStyle = GetPenStyle(borderStyle);
			if (backGradientType == GradientType.None)
			{
				solidBrush.Color = backColor;
				brush = solidBrush;
			}
			else
			{
				RectangleF bounds = path.GetBounds();
				bounds.Inflate(new SizeF(2f, 2f));
				brush = GetGradientBrush(bounds, backColor, backGradientEndColor, backGradientType);
			}
			if (backHatchStyle != 0)
			{
				brush = GetHatchBrush(backHatchStyle, backColor, backGradientEndColor);
			}
			if (backImage.Length > 0 && backImageMode != ChartImageWrapMode.Unscaled && backImageMode != ChartImageWrapMode.Scaled)
			{
				brush2 = brush;
				brush = GetTextureBrush(backImage, backImageTranspColor, backImageMode, backColor);
			}
			RectangleF bounds2 = path.GetBounds();
			if (backImage.Length > 0 && (backImageMode == ChartImageWrapMode.Unscaled || backImageMode == ChartImageWrapMode.Scaled))
			{
				Image image = common.ImageLoader.LoadImage(backImage);
				ImageAttributes imageAttributes = new ImageAttributes();
				if (backImageTranspColor != Color.Empty)
				{
					imageAttributes.SetColorKey(backImageTranspColor, backImageTranspColor, ColorAdjustType.Default);
				}
				RectangleF rectangleF = default(RectangleF);
				rectangleF.X = bounds2.X;
				rectangleF.Y = bounds2.Y;
				rectangleF.Width = bounds2.Width;
				rectangleF.Height = bounds2.Height;
				if (backImageMode == ChartImageWrapMode.Unscaled)
				{
					SizeF size = default(SizeF);
					ImageLoader.GetAdjustedImageSize(image, Graphics, ref size);
					rectangleF.Width = size.Width;
					rectangleF.Height = size.Height;
					if (rectangleF.Width < bounds2.Width)
					{
						switch (backImageAlign)
						{
						case ChartImageAlign.TopRight:
						case ChartImageAlign.Right:
						case ChartImageAlign.BottomRight:
							rectangleF.X = bounds2.Right - rectangleF.Width;
							break;
						case ChartImageAlign.Top:
						case ChartImageAlign.Bottom:
						case ChartImageAlign.Center:
							rectangleF.X = bounds2.X + (bounds2.Width - rectangleF.Width) / 2f;
							break;
						}
					}
					if (rectangleF.Height < bounds2.Height)
					{
						switch (backImageAlign)
						{
						case ChartImageAlign.BottomRight:
						case ChartImageAlign.Bottom:
						case ChartImageAlign.BottomLeft:
							rectangleF.Y = bounds2.Bottom - rectangleF.Height;
							break;
						case ChartImageAlign.Right:
						case ChartImageAlign.Left:
						case ChartImageAlign.Center:
							rectangleF.Y = bounds2.Y + (bounds2.Height - rectangleF.Height) / 2f;
							break;
						}
					}
				}
				FillPath(brush, path);
				Region clip = base.Clip;
				base.Clip = new Region(path);
				DrawImage(image, new Rectangle((int)Math.Round(rectangleF.X), (int)Math.Round(rectangleF.Y), (int)Math.Round(rectangleF.Width), (int)Math.Round(rectangleF.Height)), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
				base.Clip = clip;
			}
			else
			{
				if (brush2 != null && backImageTranspColor != Color.Empty)
				{
					FillPath(brush2, path);
				}
				FillPath(brush, path);
			}
			if (borderColor != Color.Empty && borderWidth > 0 && borderStyle != 0)
			{
				DrawPath(pen, path);
			}
		}

		internal Brush CreateBrush(RectangleF rect, Color backColor, ChartHatchStyle backHatchStyle, string backImage, ChartImageWrapMode backImageMode, Color backImageTranspColor, ChartImageAlign backImageAlign, GradientType backGradientType, Color backGradientEndColor)
		{
			Brush result = new SolidBrush(backColor);
			if (backImage.Length > 0 && backImageMode != ChartImageWrapMode.Unscaled && backImageMode != ChartImageWrapMode.Scaled)
			{
				result = GetTextureBrush(backImage, backImageTranspColor, backImageMode, backColor);
			}
			else if (backHatchStyle != 0)
			{
				result = GetHatchBrush(backHatchStyle, backColor, backGradientEndColor);
			}
			else if (backGradientType != 0)
			{
				result = GetGradientBrush(rect, backColor, backGradientEndColor, backGradientType);
			}
			return result;
		}

		public RectangleF GetRelativeRectangle(RectangleF absolute)
		{
			RectangleF empty = RectangleF.Empty;
			empty.X = absolute.X * 100f / (float)(width - 1);
			empty.Y = absolute.Y * 100f / (float)(height - 1);
			empty.Width = absolute.Width * 100f / (float)(width - 1);
			empty.Height = absolute.Height * 100f / (float)(height - 1);
			return empty;
		}

		public PointF GetRelativePoint(PointF absolute)
		{
			PointF empty = PointF.Empty;
			empty.X = absolute.X * 100f / (float)(width - 1);
			empty.Y = absolute.Y * 100f / (float)(height - 1);
			return empty;
		}

		public SizeF GetRelativeSize(SizeF size)
		{
			SizeF empty = SizeF.Empty;
			empty.Width = size.Width * 100f / (float)(width - 1);
			empty.Height = size.Height * 100f / (float)(height - 1);
			return empty;
		}

		public PointF GetAbsolutePoint(PointF relative)
		{
			PointF empty = PointF.Empty;
			empty.X = relative.X * (float)(width - 1) / 100f;
			empty.Y = relative.Y * (float)(height - 1) / 100f;
			return empty;
		}

		public RectangleF GetAbsoluteRectangle(RectangleF relative)
		{
			RectangleF empty = RectangleF.Empty;
			empty.X = relative.X * (float)(width - 1) / 100f;
			empty.Y = relative.Y * (float)(height - 1) / 100f;
			empty.Width = relative.Width * (float)(width - 1) / 100f;
			empty.Height = relative.Height * (float)(height - 1) / 100f;
			return empty;
		}

		public SizeF GetAbsoluteSize(SizeF relative)
		{
			SizeF empty = SizeF.Empty;
			empty.Width = relative.Width * (float)(width - 1) / 100f;
			empty.Height = relative.Height * (float)(height - 1) / 100f;
			return empty;
		}

		internal GraphicsPath CreateRoundedRectPath(RectangleF rect, float[] cornerRadius)
		{
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.AddLine(rect.X + cornerRadius[0], rect.Y, rect.Right - cornerRadius[1], rect.Y);
			graphicsPath.AddArc(rect.Right - 2f * cornerRadius[1], rect.Y, 2f * cornerRadius[1], 2f * cornerRadius[2], 270f, 90f);
			graphicsPath.AddLine(rect.Right, rect.Y + cornerRadius[2], rect.Right, rect.Bottom - cornerRadius[3]);
			graphicsPath.AddArc(rect.Right - 2f * cornerRadius[4], rect.Bottom - 2f * cornerRadius[3], 2f * cornerRadius[4], 2f * cornerRadius[3], 0f, 90f);
			graphicsPath.AddLine(rect.Right - cornerRadius[4], rect.Bottom, rect.X + cornerRadius[5], rect.Bottom);
			graphicsPath.AddArc(rect.X, rect.Bottom - 2f * cornerRadius[6], 2f * cornerRadius[5], 2f * cornerRadius[6], 90f, 90f);
			graphicsPath.AddLine(rect.X, rect.Bottom - cornerRadius[6], rect.X, rect.Y + cornerRadius[7]);
			graphicsPath.AddArc(rect.X, rect.Y, 2f * cornerRadius[0], 2f * cornerRadius[7], 180f, 90f);
			return graphicsPath;
		}

		internal void DrawRoundedRectShadowAbs(RectangleF rect, float[] cornerRadius, float radius, Color centerColor, Color surroundColor, float shadowScale)
		{
			GraphicsPath graphicsPath = CreateRoundedRectPath(rect, cornerRadius);
			PathGradientBrush pathGradientBrush = new PathGradientBrush(graphicsPath);
			pathGradientBrush.CenterColor = centerColor;
			Color[] array2 = pathGradientBrush.SurroundColors = new Color[1]
			{
				surroundColor
			};
			pathGradientBrush.CenterPoint = new PointF(rect.X + rect.Width / 2f, rect.Y + rect.Height / 2f);
			PointF pointF2 = pathGradientBrush.FocusScales = new PointF(1f - shadowScale * radius / rect.Width, 1f - shadowScale * radius / rect.Height);
			FillPath(pathGradientBrush, graphicsPath);
			graphicsPath?.Dispose();
		}

		internal void Draw3DBorderRel(BorderSkinAttributes borderSkin, RectangleF rect, Color backColor, ChartHatchStyle backHatchStyle, string backImage, ChartImageWrapMode backImageMode, Color backImageTranspColor, ChartImageAlign backImageAlign, GradientType backGradientType, Color backGradientEndColor, Color borderColor, int borderWidth, ChartDashStyle borderStyle)
		{
			Draw3DBorderAbs(borderSkin, GetAbsoluteRectangle(rect), backColor, backHatchStyle, backImage, backImageMode, backImageTranspColor, backImageAlign, backGradientType, backGradientEndColor, borderColor, borderWidth, borderStyle);
		}

		internal void Draw3DBorderAbs(BorderSkinAttributes borderSkin, RectangleF absRect, Color backColor, ChartHatchStyle backHatchStyle, string backImage, ChartImageWrapMode backImageMode, Color backImageTranspColor, ChartImageAlign backImageAlign, GradientType backGradientType, Color backGradientEndColor, Color borderColor, int borderWidth, ChartDashStyle borderStyle)
		{
			if (common != null && borderSkin.SkinStyle != 0 && absRect.Width != 0f && absRect.Height != 0f)
			{
				IBorderType borderType = common.BorderTypeRegistry.GetBorderType(borderSkin.SkinStyle.ToString());
				if (borderType != null)
				{
					borderType.Resolution = Graphics.DpiX;
					borderType.DrawBorder(this, borderSkin, absRect, backColor, backHatchStyle, backImage, backImageMode, backImageTranspColor, backImageAlign, backGradientType, backGradientEndColor, borderColor, borderWidth, borderStyle);
				}
			}
		}

		internal static PieDrawingStyle GetPieDrawingStyle(DataPoint point)
		{
			PieDrawingStyle result = PieDrawingStyle.Default;
			string text = point["PieDrawingStyle"];
			if (text != null)
			{
				if (string.Compare(text, "Default", StringComparison.OrdinalIgnoreCase) == 0)
				{
					result = PieDrawingStyle.Default;
				}
				else if (string.Compare(text, "SoftEdge", StringComparison.OrdinalIgnoreCase) == 0)
				{
					result = PieDrawingStyle.SoftEdge;
				}
				else
				{
					if (string.Compare(text, "Concave", StringComparison.OrdinalIgnoreCase) != 0)
					{
						throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid(text, "PieDrawingStyle"));
					}
					result = PieDrawingStyle.Concave;
				}
			}
			return result;
		}

		internal void DrawPieRel(RectangleF rect, float startAngle, float sweepAngle, Color backColor, ChartHatchStyle backHatchStyle, string backImage, ChartImageWrapMode backImageMode, Color backImageTranspColor, ChartImageAlign backImageAlign, GradientType backGradientType, Color backGradientEndColor, Color borderColor, int borderWidth, ChartDashStyle borderStyle, PenAlignment penAlignment, bool shadow, double shadowOffset, bool doughnut, float doughnutRadius, bool explodedShadow, PieDrawingStyle pieDrawingStyle, out GraphicsPath controlGraphicsPath)
		{
			controlGraphicsPath = null;
			Pen pen = null;
			RectangleF absoluteRectangle = GetAbsoluteRectangle(rect);
			if ((double)doughnutRadius == 100.0)
			{
				doughnut = false;
			}
			if ((double)doughnutRadius == 0.0)
			{
				return;
			}
			Brush brush;
			if (backHatchStyle != 0)
			{
				brush = GetHatchBrush(backHatchStyle, backColor, backGradientEndColor);
			}
			else if (backGradientEndColor.IsEmpty || backGradientType == GradientType.None)
			{
				brush = ((backImage.Length <= 0 || backImageMode == ChartImageWrapMode.Unscaled || backImageMode == ChartImageWrapMode.Scaled) ? new SolidBrush(backColor) : GetTextureBrush(backImage, backImageTranspColor, backImageMode, backColor));
			}
			else if (backGradientType == GradientType.Center)
			{
				brush = GetPieGradientBrush(absoluteRectangle, backColor, backGradientEndColor);
			}
			else
			{
				GraphicsPath graphicsPath = new GraphicsPath();
				graphicsPath.AddPie(absoluteRectangle.X, absoluteRectangle.Y, absoluteRectangle.Width, absoluteRectangle.Height, startAngle, sweepAngle);
				brush = GetGradientBrush(graphicsPath.GetBounds(), backColor, backGradientEndColor, backGradientType);
				graphicsPath?.Dispose();
			}
			pen = new Pen(borderColor, borderWidth);
			pen.DashStyle = GetPenStyle(borderStyle);
			pen.LineJoin = LineJoin.Round;
			if (doughnut)
			{
				GraphicsPath graphicsPath2 = null;
				try
				{
					graphicsPath2 = new GraphicsPath();
					graphicsPath2.AddArc(absoluteRectangle.X + absoluteRectangle.Width * doughnutRadius / 200f - 1f, absoluteRectangle.Y + absoluteRectangle.Height * doughnutRadius / 200f - 1f, absoluteRectangle.Width - absoluteRectangle.Width * doughnutRadius / 100f + 2f, absoluteRectangle.Height - absoluteRectangle.Height * doughnutRadius / 100f + 2f, startAngle, sweepAngle);
					graphicsPath2.AddArc(absoluteRectangle.X, absoluteRectangle.Y, absoluteRectangle.Width, absoluteRectangle.Height, startAngle + sweepAngle, 0f - sweepAngle);
					graphicsPath2.CloseFigure();
					FillPath(brush, graphicsPath2);
					DrawPieGradientEffects(pieDrawingStyle, absoluteRectangle, startAngle, sweepAngle, doughnutRadius, graphicsPath2);
					if (!shadow && borderWidth > 0 && borderStyle != 0)
					{
						DrawPath(pen, graphicsPath2);
					}
				}
				catch
				{
					if (graphicsPath2 != null)
					{
						graphicsPath2.Dispose();
						graphicsPath2 = null;
					}
				}
				finally
				{
					controlGraphicsPath = graphicsPath2;
				}
			}
			else
			{
				if (shadow && softShadows)
				{
					DrawPieSoftShadow(shadowOffset, startAngle, sweepAngle, explodedShadow, absoluteRectangle, backColor);
				}
				else
				{
					shadowDrawingMode = shadow;
					FillPie(brush, absoluteRectangle.X, absoluteRectangle.Y, absoluteRectangle.Width, absoluteRectangle.Height, startAngle, sweepAngle);
					shadowDrawingMode = false;
					DrawPieGradientEffects(pieDrawingStyle, absoluteRectangle, startAngle, sweepAngle, -1f, null);
				}
				if (!shadow && borderWidth > 0 && borderStyle != 0)
				{
					DrawPie(pen, absoluteRectangle.X, absoluteRectangle.Y, absoluteRectangle.Width, absoluteRectangle.Height, startAngle, sweepAngle);
				}
			}
			pen?.Dispose();
			brush?.Dispose();
		}

		internal void DrawPieRel(RectangleF rect, float startAngle, float sweepAngle, Color backColor, ChartHatchStyle backHatchStyle, string backImage, ChartImageWrapMode backImageMode, Color backImageTranspColor, ChartImageAlign backImageAlign, GradientType backGradientType, Color backGradientEndColor, Color borderColor, int borderWidth, ChartDashStyle borderStyle, PenAlignment penAlignment, bool shadow, double shadowOffset, bool doughnut, float doughnutRadius, bool explodedShadow, PieDrawingStyle pieDrawingStyle)
		{
			GraphicsPath controlGraphicsPath = null;
			try
			{
				DrawPieRel(rect, startAngle, sweepAngle, backColor, backHatchStyle, backImage, backImageMode, backImageTranspColor, backImageAlign, backGradientType, backGradientEndColor, borderColor, borderWidth, borderStyle, penAlignment, shadow, shadowOffset, doughnut, doughnutRadius, explodedShadow, pieDrawingStyle, out controlGraphicsPath);
			}
			finally
			{
				controlGraphicsPath?.Dispose();
			}
		}

		private void DrawPieGradientEffects(PieDrawingStyle pieDrawingStyle, RectangleF position, float startAngle, float sweepAngle, float doughnutRadius, GraphicsPath doughnutPath)
		{
			switch (pieDrawingStyle)
			{
			case PieDrawingStyle.Concave:
			{
				float num3 = Math.Min(position.Width, position.Height) * 0.05f;
				RectangleF rectangleF = position;
				rectangleF.Inflate(0f - num3, 0f - num3);
				using (GraphicsPath graphicsPath5 = new GraphicsPath())
				{
					graphicsPath5.AddEllipse(rectangleF);
					using (GraphicsPath graphicsPath6 = new GraphicsPath())
					{
						if (doughnutRadius < 0f)
						{
							graphicsPath6.AddPie(Rectangle.Round(rectangleF), startAngle, sweepAngle);
						}
						else
						{
							graphicsPath6.AddArc(rectangleF.X + position.Width * doughnutRadius / 200f - 1f - num3, rectangleF.Y + position.Height * doughnutRadius / 200f - 1f - num3, rectangleF.Width - position.Width * doughnutRadius / 100f + 2f + 2f * num3, rectangleF.Height - position.Height * doughnutRadius / 100f + 2f + 2f * num3, startAngle, sweepAngle);
							graphicsPath6.AddArc(rectangleF.X, rectangleF.Y, rectangleF.Width, rectangleF.Height, startAngle + sweepAngle, 0f - sweepAngle);
						}
						rectangleF.Inflate(1f, 1f);
						using (LinearGradientBrush linearGradientBrush = new LinearGradientBrush(rectangleF, Color.Red, Color.Green, LinearGradientMode.Vertical))
						{
							ColorBlend colorBlend = new ColorBlend(3);
							colorBlend.Colors[0] = Color.FromArgb(100, Color.Black);
							colorBlend.Colors[1] = Color.Transparent;
							colorBlend.Colors[2] = Color.FromArgb(140, Color.White);
							colorBlend.Positions[0] = 0f;
							colorBlend.Positions[1] = 0.5f;
							colorBlend.Positions[2] = 1f;
							linearGradientBrush.InterpolationColors = colorBlend;
							FillPath(linearGradientBrush, graphicsPath6);
						}
					}
				}
				break;
			}
			case PieDrawingStyle.SoftEdge:
			{
				float num = Math.Min(position.Width, position.Height);
				float num2 = num / 10f;
				if (doughnutRadius > 0f)
				{
					num2 = num * doughnutRadius / 100f / 8f;
				}
				using (GraphicsPath graphicsPath = new GraphicsPath())
				{
					graphicsPath.AddEllipse(position);
					using (GraphicsPath graphicsPath2 = new GraphicsPath())
					{
						graphicsPath2.AddArc(position.X + num2, position.Y + num2, position.Width - num2 * 2f, position.Height - num2 * 2f, startAngle, sweepAngle);
						graphicsPath2.AddArc(position.X, position.Y, position.Width, position.Height, startAngle + sweepAngle, 0f - sweepAngle);
						graphicsPath2.CloseFigure();
						using (PathGradientBrush pathGradientBrush = new PathGradientBrush(graphicsPath))
						{
							pathGradientBrush.CenterColor = Color.Transparent;
							pathGradientBrush.SurroundColors = new Color[1]
							{
								Color.FromArgb(100, Color.Black)
							};
							Blend blend = new Blend(3);
							blend.Positions[0] = 0f;
							blend.Factors[0] = 0f;
							blend.Positions[1] = num2 / (num / 2f);
							blend.Factors[1] = 1f;
							blend.Positions[2] = 1f;
							blend.Factors[2] = 1f;
							pathGradientBrush.Blend = blend;
							FillPath(pathGradientBrush, graphicsPath2);
						}
					}
					if (!(doughnutRadius > 0f))
					{
						break;
					}
					using (GraphicsPath graphicsPath3 = new GraphicsPath())
					{
						RectangleF rect = position;
						rect.Inflate((0f - position.Width) * doughnutRadius / 200f + num2, (0f - position.Height) * doughnutRadius / 200f + num2);
						graphicsPath3.AddEllipse(rect);
						using (GraphicsPath graphicsPath4 = new GraphicsPath())
						{
							graphicsPath4.AddArc(rect.X + num2, rect.Y + num2, rect.Width - 2f * num2, rect.Height - 2f * num2, startAngle, sweepAngle);
							graphicsPath4.AddArc(rect.X, rect.Y, rect.Width, rect.Height, startAngle + sweepAngle, 0f - sweepAngle);
							graphicsPath4.CloseFigure();
							using (PathGradientBrush pathGradientBrush2 = new PathGradientBrush(graphicsPath3))
							{
								pathGradientBrush2.CenterColor = Color.FromArgb(100, Color.Black);
								pathGradientBrush2.SurroundColors = new Color[1]
								{
									Color.Transparent
								};
								Blend blend2 = new Blend(3);
								blend2.Positions[0] = 0f;
								blend2.Factors[0] = 0f;
								blend2.Positions[1] = num2 / (rect.Width / 2f);
								blend2.Factors[1] = 1f;
								blend2.Positions[2] = 1f;
								blend2.Factors[2] = 1f;
								pathGradientBrush2.Blend = blend2;
								FillPath(pathGradientBrush2, graphicsPath4);
							}
						}
					}
				}
				break;
			}
			}
		}

		private void DrawPieSoftShadow(double shadowOffset, float startAngle, float sweepAngle, bool explodedShadow, RectangleF absRect, Color backColor)
		{
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.AddEllipse(absRect.X, absRect.Y, absRect.Width, absRect.Height);
			PathGradientBrush pathGradientBrush = new PathGradientBrush(graphicsPath);
			Color[] colors = new Color[3]
			{
				Color.FromArgb(0, backColor),
				Color.FromArgb(backColor.A, backColor),
				Color.FromArgb(backColor.A, backColor)
			};
			float[] positions = new float[3]
			{
				0f,
				0.05f,
				1f
			};
			ColorBlend colorBlend = new ColorBlend();
			colorBlend.Colors = colors;
			colorBlend.Positions = positions;
			pathGradientBrush.InterpolationColors = colorBlend;
			shadowDrawingMode = true;
			FillPie(pathGradientBrush, absRect.X, absRect.Y, absRect.Width, absRect.Height, startAngle, sweepAngle);
			shadowDrawingMode = false;
		}

		internal void DrawArrowRel(PointF position, ArrowOrientation orientation, ArrowsType type, Color color, int lineWidth, ChartDashStyle lineDashStyle, double shift, double size)
		{
			if (type != 0)
			{
				SolidBrush solidBrush = new SolidBrush(color);
				PointF endPoint = PointF.Empty;
				PointF absolutePoint = GetAbsolutePoint(position);
				switch (type)
				{
				case ArrowsType.Triangle:
				{
					PointF[] arrowShape = GetArrowShape(absolutePoint, orientation, shift, size, lineWidth, type, ref endPoint);
					endPoint = GetRelativePoint(endPoint);
					DrawLineRel(color, lineWidth, lineDashStyle, position, endPoint);
					FillPolygon(solidBrush, arrowShape);
					break;
				}
				case ArrowsType.SharpTriangle:
				{
					PointF[] arrowShape = GetArrowShape(absolutePoint, orientation, shift, size, lineWidth, type, ref endPoint);
					endPoint = GetRelativePoint(endPoint);
					DrawLineRel(color, lineWidth, lineDashStyle, position, endPoint);
					FillPolygon(solidBrush, arrowShape);
					break;
				}
				case ArrowsType.Lines:
				{
					PointF[] arrowShape = GetArrowShape(absolutePoint, orientation, shift, size, lineWidth, type, ref endPoint);
					arrowShape[0] = GetRelativePoint(arrowShape[0]);
					arrowShape[1] = GetRelativePoint(arrowShape[1]);
					arrowShape[2] = GetRelativePoint(arrowShape[2]);
					endPoint = GetRelativePoint(endPoint);
					DrawLineRel(color, lineWidth, lineDashStyle, position, endPoint);
					DrawLineRel(color, lineWidth, lineDashStyle, arrowShape[0], arrowShape[2]);
					DrawLineRel(color, lineWidth, lineDashStyle, arrowShape[1], arrowShape[2]);
					break;
				}
				}
				solidBrush?.Dispose();
			}
		}

		private PointF[] GetArrowShape(PointF position, ArrowOrientation orientation, double shift, double size, int lineWidth, ArrowsType type, ref PointF endPoint)
		{
			PointF[] array = new PointF[3];
			switch (orientation)
			{
			case ArrowOrientation.Top:
			{
				size = GetAbsoluteSize(new SizeF((float)size, (float)size)).Width;
				shift = GetAbsoluteSize(new SizeF((float)shift, (float)shift)).Height;
				double num = (type != ArrowsType.SharpTriangle) ? (size * 2.0) : (size * 4.0);
				array[0].X = position.X - (float)size;
				array[0].Y = position.Y - (float)shift;
				array[1].X = position.X + (float)size;
				array[1].Y = position.Y - (float)shift;
				array[2].X = position.X;
				array[2].Y = position.Y - (float)shift - (float)num;
				endPoint.X = position.X;
				if (type == ArrowsType.SharpTriangle || type == ArrowsType.Triangle)
				{
					endPoint.Y = array[1].Y;
				}
				else
				{
					endPoint.Y = array[2].Y;
				}
				break;
			}
			case ArrowOrientation.Bottom:
			{
				size = GetAbsoluteSize(new SizeF((float)size, (float)size)).Width;
				shift = GetAbsoluteSize(new SizeF((float)shift, (float)shift)).Height;
				double num = (type != ArrowsType.SharpTriangle) ? (size * 2.0) : (size * 4.0);
				array[0].X = position.X - (float)size;
				array[0].Y = position.Y + (float)shift;
				array[1].X = position.X + (float)size;
				array[1].Y = position.Y + (float)shift;
				array[2].X = position.X;
				array[2].Y = position.Y + (float)shift + (float)num;
				endPoint.X = position.X;
				if (type == ArrowsType.SharpTriangle || type == ArrowsType.Triangle)
				{
					endPoint.Y = array[1].Y;
				}
				else
				{
					endPoint.Y = array[2].Y;
				}
				break;
			}
			case ArrowOrientation.Left:
			{
				size = GetAbsoluteSize(new SizeF((float)size, (float)size)).Width;
				shift = GetAbsoluteSize(new SizeF((float)shift, (float)shift)).Width;
				double num = (type != ArrowsType.SharpTriangle) ? (size * 2.0) : (size * 4.0);
				array[0].Y = position.Y - (float)size;
				array[0].X = position.X - (float)shift;
				array[1].Y = position.Y + (float)size;
				array[1].X = position.X - (float)shift;
				array[2].Y = position.Y;
				array[2].X = position.X - (float)shift - (float)num;
				endPoint.Y = position.Y;
				if (type == ArrowsType.SharpTriangle || type == ArrowsType.Triangle)
				{
					endPoint.X = array[1].X;
				}
				else
				{
					endPoint.X = array[2].X;
				}
				break;
			}
			case ArrowOrientation.Right:
			{
				size = GetAbsoluteSize(new SizeF((float)size, (float)size)).Width;
				shift = GetAbsoluteSize(new SizeF((float)shift, (float)shift)).Width;
				double num = (type != ArrowsType.SharpTriangle) ? (size * 2.0) : (size * 4.0);
				array[0].Y = position.Y - (float)size;
				array[0].X = position.X + (float)shift;
				array[1].Y = position.Y + (float)size;
				array[1].X = position.X + (float)shift;
				array[2].Y = position.Y;
				array[2].X = position.X + (float)shift + (float)num;
				endPoint.Y = position.Y;
				if (type == ArrowsType.SharpTriangle || type == ArrowsType.Triangle)
				{
					endPoint.X = array[1].X;
				}
				else
				{
					endPoint.X = array[2].X;
				}
				break;
			}
			}
			return array;
		}

		internal static void Widen(GraphicsPath path, Pen pen)
		{
			try
			{
				path.Widen(pen);
			}
			catch (ThreadAbortException)
			{
				Thread.ResetAbort();
			}
			catch (OutOfMemoryException)
			{
			}
			catch (ArgumentException)
			{
			}
		}

		internal static BarDrawingStyle GetBarDrawingStyle(DataPoint point)
		{
			BarDrawingStyle result = BarDrawingStyle.Default;
			string text = point["DrawingStyle"];
			if (text != null)
			{
				if (string.Compare(text, "Default", StringComparison.OrdinalIgnoreCase) == 0)
				{
					result = BarDrawingStyle.Default;
				}
				else if (string.Compare(text, "Cylinder", StringComparison.OrdinalIgnoreCase) == 0)
				{
					result = BarDrawingStyle.Cylinder;
				}
				else if (string.Compare(text, "Emboss", StringComparison.OrdinalIgnoreCase) == 0)
				{
					result = BarDrawingStyle.Emboss;
				}
				else if (string.Compare(text, "LightToDark", StringComparison.OrdinalIgnoreCase) == 0)
				{
					result = BarDrawingStyle.LightToDark;
				}
				else
				{
					if (string.Compare(text, "Wedge", StringComparison.OrdinalIgnoreCase) != 0)
					{
						throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid(text, "DrawingStyle"));
					}
					result = BarDrawingStyle.Wedge;
				}
			}
			return result;
		}

		internal RectangleF Round(RectangleF rect)
		{
			float num = (float)Math.Round(rect.Left);
			float num2 = (float)Math.Round(rect.Right);
			float num3 = (float)Math.Round(rect.Top);
			float num4 = (float)Math.Round(rect.Bottom);
			return new RectangleF(num, num3, num2 - num, num4 - num3);
		}

		public double GetPositionFromAxis(string chartAreaName, AxisName axis, double axisValue)
		{
			switch (axis)
			{
			case AxisName.X:
				return common.ChartPicture.ChartAreas[chartAreaName].AxisX.GetLinearPosition(axisValue);
			case AxisName.X2:
				return common.ChartPicture.ChartAreas[chartAreaName].AxisX2.GetLinearPosition(axisValue);
			case AxisName.Y:
				return common.ChartPicture.ChartAreas[chartAreaName].AxisY.GetLinearPosition(axisValue);
			case AxisName.Y2:
				return common.ChartPicture.ChartAreas[chartAreaName].AxisY2.GetLinearPosition(axisValue);
			default:
				return 0.0;
			}
		}

		internal void SetPictureSize(int width, int height)
		{
			this.width = width;
			this.height = height;
		}

		internal ChartGraphics(CommonElements common)
		{
			this.common = common;
			pen = new Pen(Color.Black);
			solidBrush = new SolidBrush(Color.Black);
		}

		internal void Dispose()
		{
			if (pen != null)
			{
				pen.Dispose();
			}
			if (solidBrush != null)
			{
				solidBrush.Dispose();
			}
		}

		internal new void SetClip(RectangleF region)
		{
			base.SetClip(GetAbsoluteRectangle(region));
		}

		internal void SetClipAbs(RectangleF region)
		{
			base.SetClip(region);
		}

		internal void StartAnimation()
		{
		}

		internal void StopAnimation()
		{
		}

		public static Color GetGradientColor(Color beginColor, Color endColor, double relativePosition)
		{
			if (relativePosition < 0.0 || relativePosition > 1.0 || double.IsNaN(relativePosition))
			{
				return beginColor;
			}
			int r = beginColor.R;
			int g = beginColor.G;
			int b = beginColor.B;
			int r2 = endColor.R;
			int g2 = endColor.G;
			int b2 = endColor.B;
			double num = (double)r + (double)(r2 - r) * relativePosition;
			double num2 = (double)g + (double)(g2 - g) * relativePosition;
			double num3 = (double)b + (double)(b2 - b) * relativePosition;
			if (num > 255.0)
			{
				num = 255.0;
			}
			if (num < 0.0)
			{
				num = 0.0;
			}
			if (num2 > 255.0)
			{
				num2 = 255.0;
			}
			if (num2 < 0.0)
			{
				num2 = 0.0;
			}
			if (num3 > 255.0)
			{
				num3 = 255.0;
			}
			if (num3 < 0.0)
			{
				num3 = 0.0;
			}
			return Color.FromArgb(beginColor.A, (int)num, (int)num2, (int)num3);
		}

		private GraphicsPath GetLabelBackgroundGraphicsPath(RectangleF backPosition, int rotationAngle)
		{
			RectangleF rect = Round(GetAbsoluteRectangle(backPosition));
			PointF point = new PointF(rect.X + rect.Width / 2f, rect.Y + rect.Height / 2f);
			myMatrix = base.Transform.Clone();
			myMatrix.RotateAt(rotationAngle, point);
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.AddRectangle(rect);
			graphicsPath.Transform(myMatrix);
			return graphicsPath;
		}

		public bool CanLabelFitInSlice(GraphicsPath sliceGraphicsPath, RectangleF labelRelativeRect, int labelRotationAngle)
		{
			if (sliceGraphicsPath == null)
			{
				return false;
			}
			using (GraphicsPath path = GetLabelBackgroundGraphicsPath(labelRelativeRect, labelRotationAngle))
			{
				return sliceGraphicsPath.IsSuperSetOf(path, Graphics);
			}
		}

		public void DrawLabelBackground(int angle, PointF textPosition, RectangleF backPosition, Color backColor, Color borderColor, int borderWidth, ChartDashStyle borderStyle)
		{
			RectangleF rect = Round(GetAbsoluteRectangle(backPosition));
			PointF empty = PointF.Empty;
			empty = ((!textPosition.IsEmpty) ? GetAbsolutePoint(textPosition) : new PointF(rect.X + rect.Width / 2f, rect.Y + rect.Height / 2f));
			myMatrix = base.Transform.Clone();
			myMatrix.RotateAt(angle, empty);
			GraphicsState gstate = Save();
			base.Transform = myMatrix;
			if (!backColor.IsEmpty || !borderColor.IsEmpty)
			{
				using (Brush brush = new SolidBrush(backColor))
				{
					FillRectangle(brush, rect);
				}
				if (borderWidth > 0 && !borderColor.IsEmpty && borderStyle != 0)
				{
					AntiAliasingTypes antiAliasingTypes = AntiAliasing;
					try
					{
						AntiAliasing = AntiAliasingTypes.None;
						using (Pen pen = new Pen(borderColor, borderWidth))
						{
							pen.DashStyle = GetPenStyle(borderStyle);
							DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
						}
					}
					finally
					{
						AntiAliasing = antiAliasingTypes;
					}
				}
			}
			else
			{
				using (Brush brush2 = new SolidBrush(Color.Transparent))
				{
					FillRectangle(brush2, rect);
				}
			}
			Restore(gstate);
		}

		public void MapCategoryNodeLabel(CommonElements common, CategoryNode node, RectangleF backPosition)
		{
			if (common != null && common.ProcessModeRegions)
			{
				GraphicsPath graphicsPath = new GraphicsPath();
				RectangleF rect = Round(GetAbsoluteRectangle(backPosition));
				graphicsPath.AddRectangle(rect);
				graphicsPath.Transform(myMatrix);
				common.HotRegionsList.AddHotRegion(this, graphicsPath, relativePath: false, node.LabelToolTip, node.LabelHref, "", node, ChartElementType.Nothing);
				if (common.HotRegionsList.List != null)
				{
					((HotRegion)common.HotRegionsList.List[common.HotRegionsList.List.Count - 1]).Type = ChartElementType.Nothing;
				}
			}
		}

		public float GetAbsoluteWidth(float widthRelative)
		{
			return widthRelative * (float)(width - 1) / 100f;
		}
	}
}
