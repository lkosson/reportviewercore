using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.ImageRenderer
{
	internal sealed class SharedRenderer
	{
		internal const float INCH_TO_MILLIMETER = 25.4f;

		public const float MIN_DOUBLE_BORDER_SIZE = 0.5292f;

		private SharedRenderer()
		{
		}

		internal static void CalculateImageRectangle(RectangleF position, GDIImageProps gdiProperties, RPLFormat.Sizings sizing, out RectangleF imagePositionAndSize, out RectangleF imagePortion)
		{
			CalculateImageRectangle(position, gdiProperties.Width, gdiProperties.Height, gdiProperties.HorizontalResolution, gdiProperties.VerticalResolution, sizing, out imagePositionAndSize, out imagePortion);
		}

		internal static void CalculateImageRectangle(RectangleF position, int width, int height, float horizontalResolution, float verticalResolution, RPLFormat.Sizings sizing, out RectangleF imagePositionAndSize, out RectangleF imagePortion)
		{
			imagePositionAndSize = position;
			if (sizing == RPLFormat.Sizings.Clip)
			{
				imagePortion = new RectangleF(0f, 0f, width, height);
				float num = ConvertToPixels(imagePositionAndSize.Width, horizontalResolution);
				if ((float)width > num)
				{
					imagePortion.Width = num;
				}
				else
				{
					imagePositionAndSize.Width = ConvertToMillimeters(width, horizontalResolution);
				}
				float num2 = ConvertToPixels(imagePositionAndSize.Height, verticalResolution);
				if ((float)height > num2)
				{
					imagePortion.Height = num2;
				}
				else
				{
					imagePositionAndSize.Height = ConvertToMillimeters(height, verticalResolution);
				}
				return;
			}
			imagePortion = new RectangleF(0f, 0f, width, height);
			switch (sizing)
			{
			case RPLFormat.Sizings.AutoSize:
				imagePositionAndSize.Width = ConvertToMillimeters(width, horizontalResolution);
				imagePositionAndSize.Height = ConvertToMillimeters(height, verticalResolution);
				break;
			case RPLFormat.Sizings.FitProportional:
			{
				float num3 = ConvertToMillimeters(width, horizontalResolution);
				float num4 = ConvertToMillimeters(height, verticalResolution);
				float num5 = position.Width / num3;
				float num6 = position.Height / num4;
				if (num5 > num6)
				{
					imagePositionAndSize.Width = num3 * num6;
				}
				else
				{
					imagePositionAndSize.Height = num4 * num5;
				}
				break;
			}
			}
		}

		internal static float ConvertToMillimeters(int pixels, float dpi)
		{
			if (dpi == 0f)
			{
				return float.MaxValue;
			}
			return 1f / dpi * (float)pixels * 25.4f;
		}

		internal static int ConvertToPixels(float mm, float dpi)
		{
			return Convert.ToInt32((double)dpi * 0.03937007874 * (double)mm);
		}

		internal static float ConvertToMillimeters(int pixels, float? dpi, WriterBase writer)
		{
			if (dpi.HasValue)
			{
				return ConvertToMillimeters(pixels, dpi.Value);
			}
			return writer.ConvertToMillimeters(pixels);
		}

		internal static int ConvertToPixels(float mm, float? dpi, WriterBase writer)
		{
			if (dpi.HasValue)
			{
				return ConvertToPixels(mm, dpi.Value);
			}
			return writer.ConvertToPixels(mm);
		}

		internal static void DrawImage(System.Drawing.Graphics graphics, Image image, RectangleF rectDestMM, RectangleF rectSourcePX)
		{
			DrawImage(graphics, image, rectDestMM, rectSourcePX, null);
		}

		internal static void DrawImage(System.Drawing.Graphics graphics, Image image, RectangleF rectDestMM, RectangleF rectSourcePX, ImageAttributes imageAttributes)
		{
			DrawImage(graphics, image, new PointF[3]
			{
				rectDestMM.Location,
				new PointF(rectDestMM.Location.X + rectDestMM.Width, rectDestMM.Location.Y),
				new PointF(rectDestMM.Location.X, rectDestMM.Location.Y + rectDestMM.Height)
			}, rectSourcePX, imageAttributes);
		}

		internal static void DrawImage(System.Drawing.Graphics graphics, Image image, PointF[] pointsDestMM, RectangleF rectSourcePX, ImageAttributes imageAttributes)
		{
			PointF[] destPoints = new PointF[3]
			{
				new PointF(ConvertToPixels(pointsDestMM[0].X, graphics.DpiX), ConvertToPixels(pointsDestMM[0].Y, graphics.DpiY)),
				new PointF(ConvertToPixels(pointsDestMM[1].X, graphics.DpiX), ConvertToPixels(pointsDestMM[1].Y, graphics.DpiY)),
				new PointF(ConvertToPixels(pointsDestMM[2].X, graphics.DpiX), ConvertToPixels(pointsDestMM[2].Y, graphics.DpiY))
			};
			using (Matrix matrix = graphics.Transform)
			{
				using (Matrix transform = new Matrix(matrix.Elements[0], matrix.Elements[1], matrix.Elements[2], matrix.Elements[3], ConvertToPixels(matrix.Elements[4], graphics.DpiX), ConvertToPixels(matrix.Elements[5], graphics.DpiY)))
				{
					graphics.Transform = transform;
					graphics.PageUnit = GraphicsUnit.Pixel;
					graphics.DrawImage(image, destPoints, rectSourcePX, GraphicsUnit.Pixel, imageAttributes);
					graphics.PageUnit = GraphicsUnit.Millimeter;
					graphics.Transform = matrix;
				}
			}
		}

		internal static void GetFontFormatInformation(RPLElementProps elementProperties, out RPLFormat.WritingModes writingMode, out RPLFormat.Directions direction, out RPLFormat.VerticalAlignments verticalAlign, out RPLFormat.TextAlignments textAlign, ref bool stringFormatFromInstance)
		{
			writingMode = (RPLFormat.WritingModes)GetStylePropertyValueObject(elementProperties, 30, ref stringFormatFromInstance);
			direction = (RPLFormat.Directions)GetStylePropertyValueObject(elementProperties, 29, ref stringFormatFromInstance);
			verticalAlign = (RPLFormat.VerticalAlignments)GetStylePropertyValueObject(elementProperties, 26, ref stringFormatFromInstance);
			textAlign = (RPLFormat.TextAlignments)GetStylePropertyValueObject(elementProperties, 25, ref stringFormatFromInstance);
		}

		internal static void GetFontStyleInformation(RPLElementProps elementProperties, out RPLFormat.FontStyles fontStyle, out RPLFormat.FontWeights fontWeight, out RPLFormat.TextDecorations textDecoration, out float fontSize, out string fontFamily, ref bool fontStyleFromInstance)
		{
			fontSize = (float)new RPLReportSize(GetStylePropertyValueString(elementProperties, 21, ref fontStyleFromInstance)).ToPoints();
			fontStyle = (RPLFormat.FontStyles)GetStylePropertyValueObject(elementProperties, 19, ref fontStyleFromInstance);
			fontWeight = (RPLFormat.FontWeights)GetStylePropertyValueObject(elementProperties, 22, ref fontStyleFromInstance);
			textDecoration = (RPLFormat.TextDecorations)GetStylePropertyValueObject(elementProperties, 24, ref fontStyleFromInstance);
			fontFamily = GetStylePropertyValueString(elementProperties, 20, ref fontStyleFromInstance);
		}

		internal static RectangleF GetMeasurementRectangle(RPLMeasurement measurement, RectangleF bounds)
		{
			return new RectangleF(measurement.Left + bounds.Left, measurement.Top + bounds.Top, measurement.Width, measurement.Height);
		}

		internal static bool GetImage(RPLReport rplReport, ref byte[] imageData, long imageDataOffset)
		{
			if (imageData != null)
			{
				return true;
			}
			if (imageDataOffset <= 0)
			{
				return false;
			}
			imageData = rplReport.GetImage(imageDataOffset);
			if (imageData == null)
			{
				return false;
			}
			return true;
		}

		internal static Stream GetEmbeddedImageStream(RPLReport rplReport, long imageDataOffset, CreateAndRegisterStream createAndRegisterStream, string imageName)
		{
			Stream stream = null;
			if (imageDataOffset > 0)
			{
				stream = createAndRegisterStream(imageName, string.Empty, null, null, willSeek: true, StreamOper.CreateOnly);
				rplReport.GetImage(imageDataOffset, stream);
			}
			return stream;
		}

		internal static bool GetImage(RPLReport rplReport, ref byte[] imageData, long imageDataOffset, ref GDIImageProps gdiImageProps)
		{
			if (GetImage(rplReport, ref imageData, imageDataOffset))
			{
				if (gdiImageProps == null)
				{
					try
					{
						using (Image image = Image.FromStream(new MemoryStream(imageData)))
						{
							gdiImageProps = new GDIImageProps(image);
						}
					}
					catch
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		internal static Color GetReportColorStyle(RPLElementStyle properties, byte style)
		{
			object obj = properties[style];
			if (obj != null)
			{
				return new RPLReportColor((string)obj).ToColor();
			}
			return Color.Empty;
		}

		internal static float GetReportSizeStyleMM(RPLElementStyle properties, byte style)
		{
			object obj = properties[style];
			if (obj != null)
			{
				return (float)new RPLReportSize((string)obj).ToMillimeters();
			}
			return float.NaN;
		}

		internal static RPLFormat.BorderStyles GetStylePropertyValueBorderStyle(RPLElementStyle properties, byte style, RPLFormat.BorderStyles defaultStyle)
		{
			object obj = properties[style];
			if (obj != null)
			{
				return (RPLFormat.BorderStyles)obj;
			}
			return defaultStyle;
		}

		internal static RPLFormat.BorderStyles GetStylePropertyValueBorderStyle(RPLElementProps properties, byte style, RPLFormat.BorderStyles defaultStyle)
		{
			object stylePropertyValueObject = GetStylePropertyValueObject(properties, style);
			if (stylePropertyValueObject != null)
			{
				return (RPLFormat.BorderStyles)stylePropertyValueObject;
			}
			return defaultStyle;
		}

		internal static object GetStylePropertyValueObject(RPLElementProps properties, byte style)
		{
			bool fromInstance = false;
			return GetStylePropertyValueObject(properties, style, ref fromInstance);
		}

		internal static object GetStylePropertyValueObject(RPLElementProps properties, byte style, ref bool fromInstance)
		{
			object obj = null;
			if (properties.NonSharedStyle != null)
			{
				obj = properties.NonSharedStyle[style];
				if (obj != null)
				{
					fromInstance = true;
					return obj;
				}
			}
			if (properties.Definition.SharedStyle != null)
			{
				obj = properties.Definition.SharedStyle[style];
			}
			return obj;
		}

		internal static string GetStylePropertyValueString(RPLElementProps properties, byte style)
		{
			bool fromInstance = false;
			return GetStylePropertyValueString(properties, style, ref fromInstance);
		}

		internal static string GetStylePropertyValueString(RPLElementProps properties, byte style, ref bool fromInstance)
		{
			object stylePropertyValueObject = GetStylePropertyValueObject(properties, style, ref fromInstance);
			if (stylePropertyValueObject == null)
			{
				return null;
			}
			return (string)stylePropertyValueObject;
		}

		internal static RPLFormat.TextAlignments GetTextAlignForGeneral(TypeCode typeCode, RPLFormat.Directions direction)
		{
			if ((uint)(typeCode - 5) <= 11u)
			{
				if (direction == RPLFormat.Directions.LTR)
				{
					return RPLFormat.TextAlignments.Right;
				}
				return RPLFormat.TextAlignments.Left;
			}
			if (direction == RPLFormat.Directions.LTR)
			{
				return RPLFormat.TextAlignments.Left;
			}
			return RPLFormat.TextAlignments.Right;
		}

		internal static bool IsWeightBold(RPLFormat.FontWeights weight)
		{
			if (weight == RPLFormat.FontWeights.SemiBold || weight == RPLFormat.FontWeights.Bold || weight == RPLFormat.FontWeights.ExtraBold || weight == RPLFormat.FontWeights.Heavy)
			{
				return true;
			}
			return false;
		}

		internal static void ProcessBottomBorder(WriterBase writer, List<Operation> operations, float borderWidthBottom, RPLFormat.BorderStyles borderStyleBottom, Color borderColorBottom, Color borderColorLeft, Color borderColorRight, float borderBottom, float borderBottomEdge, float borderLeftEdge, float borderRightEdge, float borderBottomEdgeUnclipped, float borderLeftEdgeUnclipped, float borderRightEdgeUnclipped, float borderWidthLeft, float borderWidthRight, float borderWidthBottomUnclipped, float borderWidthLeftUnclipped, float borderWidthRightUnclipped)
		{
			switch (borderStyleBottom)
			{
			case RPLFormat.BorderStyles.None:
				return;
			case RPLFormat.BorderStyles.Solid:
				if (borderWidthBottom > writer.HalfPixelWidthY * 2f && ((borderWidthLeft > 0f && borderColorBottom != borderColorLeft) || (borderWidthRight > 0f && borderColorBottom != borderColorRight)))
				{
					PointF[] polygon = new PointF[4]
					{
						new PointF(borderLeftEdge, borderBottomEdge),
						new PointF(borderRightEdge, borderBottomEdge),
						new PointF(borderRightEdge - borderWidthRight, borderBottomEdge - borderWidthBottom),
						new PointF(borderLeftEdge + borderWidthLeft, borderBottomEdge - borderWidthBottom)
					};
					if (operations == null)
					{
						writer.FillPolygon(borderColorBottom, polygon);
					}
					else
					{
						operations.Add(new FillPolygonOp(borderColorBottom, polygon));
					}
					return;
				}
				break;
			}
			if (borderStyleBottom == RPLFormat.BorderStyles.Double)
			{
				PointF[] array = new PointF[4];
				float num = borderWidthBottomUnclipped / 3f;
				if (borderBottomEdge >= borderBottomEdgeUnclipped - num)
				{
					float num2 = Math.Max(borderWidthLeft - borderWidthLeftUnclipped / 3f * 2f, 0f);
					float num3 = Math.Max(borderWidthRight - borderWidthRightUnclipped / 3f * 2f, 0f);
					array[0] = new PointF(borderLeftEdge, borderBottomEdge);
					array[1] = new PointF(borderRightEdge, borderBottomEdge);
					array[2] = new PointF(borderRightEdge - num3, borderBottomEdgeUnclipped - num);
					array[3] = new PointF(borderLeftEdge + num2, borderBottomEdgeUnclipped - num);
					if (operations == null)
					{
						writer.FillPolygon(borderColorBottom, array);
					}
					else
					{
						operations.Add(new FillPolygonOp(borderColorBottom, array));
					}
				}
				array = new PointF[4];
				float num4 = borderBottomEdgeUnclipped - borderWidthBottomUnclipped / 3f * 2f;
				float x;
				float x2;
				if (borderWidthLeft > 0f)
				{
					x = borderLeftEdgeUnclipped + borderWidthLeftUnclipped / 3f * 2f;
					x2 = borderLeftEdgeUnclipped + borderWidthLeftUnclipped;
				}
				else
				{
					x = (x2 = borderLeftEdge);
				}
				float x3;
				float x4;
				if (borderWidthRight > 0f)
				{
					x3 = borderRightEdgeUnclipped - borderWidthRightUnclipped / 3f * 2f;
					x4 = borderRightEdgeUnclipped - borderWidthRightUnclipped;
				}
				else
				{
					x3 = (x4 = borderRightEdge);
				}
				array[0] = new PointF(x, num4);
				array[1] = new PointF(x3, num4);
				array[2] = new PointF(x4, num4 - num);
				array[3] = new PointF(x2, num4 - num);
				if (operations == null)
				{
					writer.FillPolygon(borderColorBottom, array);
				}
				else
				{
					operations.Add(new FillPolygonOp(borderColorBottom, array));
				}
			}
			else if (operations == null)
			{
				writer.DrawLine(borderColorBottom, borderWidthBottom, borderStyleBottom, borderLeftEdge, borderBottom, borderRightEdge, borderBottom);
			}
			else
			{
				operations.Add(new DrawLineOp(borderColorBottom, borderWidthBottom, borderStyleBottom, borderLeftEdge, borderBottom, borderRightEdge, borderBottom));
			}
		}

		internal static void ProcessLeftBorder(WriterBase writer, List<Operation> operations, float borderWidthLeft, RPLFormat.BorderStyles borderStyleLeft, Color borderColorLeft, Color borderColorTop, Color borderColorBottom, float borderLeft, float borderLeftEdge, float borderTopEdge, float borderBottomEdge, float borderLeftEdgeUnclipped, float borderTopEdgeUnclipped, float borderBottomEdgeUnclipped, float borderWidthTop, float borderWidthBottom, float borderWidthLeftUnclipped, float borderWidthTopUnclipped, float borderWidthBottomUnclipped)
		{
			switch (borderStyleLeft)
			{
			case RPLFormat.BorderStyles.None:
				return;
			case RPLFormat.BorderStyles.Solid:
				if (borderWidthLeft > writer.HalfPixelWidthX * 2f && ((borderWidthTop > 0f && borderColorLeft != borderColorTop) || (borderWidthBottom > 0f && borderColorLeft != borderColorBottom)))
				{
					PointF[] polygon = new PointF[4]
					{
						new PointF(borderLeftEdge, borderTopEdge),
						new PointF(borderLeftEdge, borderBottomEdge),
						new PointF(borderLeftEdge + borderWidthLeft, borderBottomEdge - borderWidthBottom),
						new PointF(borderLeftEdge + borderWidthLeft, borderTopEdge + borderWidthTop)
					};
					if (operations == null)
					{
						writer.FillPolygon(borderColorLeft, polygon);
					}
					else
					{
						operations.Add(new FillPolygonOp(borderColorLeft, polygon));
					}
					return;
				}
				break;
			}
			if (borderStyleLeft == RPLFormat.BorderStyles.Double)
			{
				PointF[] array = new PointF[4];
				float num = borderWidthLeftUnclipped / 3f;
				if (borderLeftEdge <= borderLeftEdgeUnclipped + num)
				{
					float num2 = Math.Max(borderWidthTop - borderWidthTopUnclipped / 3f * 2f, 0f);
					float num3 = Math.Max(borderWidthBottom - borderWidthBottomUnclipped / 3f * 2f, 0f);
					array[0] = new PointF(borderLeftEdge, borderTopEdge);
					array[1] = new PointF(borderLeftEdge, borderBottomEdge);
					array[2] = new PointF(borderLeftEdgeUnclipped + num, borderBottomEdge - num3);
					array[3] = new PointF(borderLeftEdgeUnclipped + num, borderTopEdge + num2);
					if (operations == null)
					{
						writer.FillPolygon(borderColorLeft, array);
					}
					else
					{
						operations.Add(new FillPolygonOp(borderColorLeft, array));
					}
				}
				array = new PointF[4];
				float num4 = borderLeftEdgeUnclipped + borderWidthLeftUnclipped / 3f * 2f;
				float y;
				float y2;
				if (borderWidthTop > 0f)
				{
					y = borderTopEdgeUnclipped + borderWidthTopUnclipped / 3f * 2f;
					y2 = borderTopEdgeUnclipped + borderWidthTopUnclipped;
				}
				else
				{
					y = (y2 = borderTopEdge);
				}
				float y3;
				float y4;
				if (borderWidthBottom > 0f)
				{
					y3 = borderBottomEdgeUnclipped - borderWidthBottomUnclipped / 3f * 2f;
					y4 = borderBottomEdgeUnclipped - borderWidthBottomUnclipped;
				}
				else
				{
					y3 = (y4 = borderBottomEdge);
				}
				array[0] = new PointF(num4, y);
				array[1] = new PointF(num4, y3);
				array[2] = new PointF(num4 + num, y4);
				array[3] = new PointF(num4 + num, y2);
				if (operations == null)
				{
					writer.FillPolygon(borderColorLeft, array);
				}
				else
				{
					operations.Add(new FillPolygonOp(borderColorLeft, array));
				}
			}
			else if (operations == null)
			{
				writer.DrawLine(borderColorLeft, borderWidthLeft, borderStyleLeft, borderLeft, borderTopEdge, borderLeft, borderBottomEdge);
			}
			else
			{
				operations.Add(new DrawLineOp(borderColorLeft, borderWidthLeft, borderStyleLeft, borderLeft, borderTopEdge, borderLeft, borderBottomEdge));
			}
		}

		internal static void ProcessRightBorder(WriterBase writer, List<Operation> operations, float borderWidthRight, RPLFormat.BorderStyles borderStyleRight, Color borderColorRight, Color borderColorTop, Color borderColorBottom, float borderRight, float borderRightEdge, float borderTopEdge, float borderBottomEdge, float borderRightEdgeUnclipped, float borderTopEdgeUnclipped, float borderBottomEdgeUnclipped, float borderWidthTop, float borderWidthBottom, float borderWidthRightUnclipped, float borderWidthTopUnclipped, float borderWidthBottomUnclipped)
		{
			switch (borderStyleRight)
			{
			case RPLFormat.BorderStyles.None:
				return;
			case RPLFormat.BorderStyles.Solid:
				if (borderWidthRight > writer.HalfPixelWidthX * 2f && ((borderWidthTop > 0f && borderColorRight != borderColorTop) || (borderWidthBottom > 0f && borderColorRight != borderColorBottom)))
				{
					PointF[] polygon = new PointF[4]
					{
						new PointF(borderRightEdge, borderTopEdge),
						new PointF(borderRightEdge, borderBottomEdge),
						new PointF(borderRightEdge - borderWidthRight, borderBottomEdge - borderWidthBottom),
						new PointF(borderRightEdge - borderWidthRight, borderTopEdge + borderWidthTop)
					};
					if (operations == null)
					{
						writer.FillPolygon(borderColorRight, polygon);
					}
					else
					{
						operations.Add(new FillPolygonOp(borderColorRight, polygon));
					}
					return;
				}
				break;
			}
			if (borderStyleRight == RPLFormat.BorderStyles.Double)
			{
				PointF[] array = new PointF[4];
				float num = borderWidthRightUnclipped / 3f;
				if (borderRightEdge >= borderRightEdgeUnclipped - num)
				{
					float num2 = Math.Max(borderWidthTop - borderWidthTopUnclipped / 3f * 2f, 0f);
					float num3 = Math.Max(borderWidthBottom - borderWidthBottomUnclipped / 3f * 2f, 0f);
					array[0] = new PointF(borderRightEdge, borderTopEdge);
					array[1] = new PointF(borderRightEdge, borderBottomEdge);
					array[2] = new PointF(borderRightEdgeUnclipped - num, borderBottomEdge - num3);
					array[3] = new PointF(borderRightEdgeUnclipped - num, borderTopEdge + num2);
					if (operations == null)
					{
						writer.FillPolygon(borderColorRight, array);
					}
					else
					{
						operations.Add(new FillPolygonOp(borderColorRight, array));
					}
				}
				array = new PointF[4];
				float num4 = borderRightEdgeUnclipped - borderWidthRightUnclipped / 3f * 2f;
				float y;
				float y2;
				if (borderWidthTop > 0f)
				{
					y = borderTopEdgeUnclipped + borderWidthTopUnclipped / 3f * 2f;
					y2 = borderTopEdgeUnclipped + borderWidthTopUnclipped;
				}
				else
				{
					y = (y2 = borderTopEdge);
				}
				float y3;
				float y4;
				if (borderWidthBottom > 0f)
				{
					y3 = borderBottomEdgeUnclipped - borderWidthBottomUnclipped / 3f * 2f;
					y4 = borderBottomEdgeUnclipped - borderWidthBottomUnclipped;
				}
				else
				{
					y3 = (y4 = borderBottomEdge);
				}
				array[0] = new PointF(num4, y);
				array[1] = new PointF(num4, y3);
				array[2] = new PointF(num4 - num, y4);
				array[3] = new PointF(num4 - num, y2);
				if (operations == null)
				{
					writer.FillPolygon(borderColorRight, array);
				}
				else
				{
					operations.Add(new FillPolygonOp(borderColorRight, array));
				}
			}
			else if (operations == null)
			{
				writer.DrawLine(borderColorRight, borderWidthRight, borderStyleRight, borderRight, borderTopEdge, borderRight, borderBottomEdge);
			}
			else
			{
				operations.Add(new DrawLineOp(borderColorRight, borderWidthRight, borderStyleRight, borderRight, borderTopEdge, borderRight, borderBottomEdge));
			}
		}

		internal static void ProcessTopBorder(WriterBase writer, List<Operation> operations, float borderWidthTop, RPLFormat.BorderStyles borderStyleTop, Color borderColorTop, Color borderColorLeft, Color borderColorRight, float borderTop, float borderTopEdge, float borderLeftEdge, float borderRightEdge, float borderTopEdgeUnclipped, float borderLeftEdgeUnclipped, float borderRightEdgeUnclipped, float borderWidthLeft, float borderWidthRight, float borderWidthTopUnclipped, float borderWidthLeftUnclipped, float borderWidthRightUnclipped)
		{
			switch (borderStyleTop)
			{
			case RPLFormat.BorderStyles.None:
				return;
			case RPLFormat.BorderStyles.Solid:
				if (borderWidthTop > writer.HalfPixelWidthY * 2f && ((borderWidthLeft > 0f && borderColorTop != borderColorLeft) || (borderWidthRight > 0f && borderColorTop != borderColorRight)))
				{
					PointF[] polygon = new PointF[4]
					{
						new PointF(borderLeftEdge, borderTopEdge),
						new PointF(borderRightEdge, borderTopEdge),
						new PointF(borderRightEdge - borderWidthRight, borderTopEdge + borderWidthTop),
						new PointF(borderLeftEdge + borderWidthLeft, borderTopEdge + borderWidthTop)
					};
					if (operations == null)
					{
						writer.FillPolygon(borderColorTop, polygon);
					}
					else
					{
						operations.Add(new FillPolygonOp(borderColorTop, polygon));
					}
					return;
				}
				break;
			}
			if (borderStyleTop == RPLFormat.BorderStyles.Double)
			{
				PointF[] array = new PointF[4];
				float num = borderWidthTopUnclipped / 3f;
				if (borderTopEdge <= borderTopEdgeUnclipped + num)
				{
					float num2 = Math.Max(borderWidthLeft - borderWidthLeftUnclipped / 3f * 2f, 0f);
					float num3 = Math.Max(borderWidthRight - borderWidthRightUnclipped / 3f * 2f, 0f);
					array[0] = new PointF(borderLeftEdge, borderTopEdge);
					array[1] = new PointF(borderRightEdge, borderTopEdge);
					array[2] = new PointF(borderRightEdge - num3, borderTopEdgeUnclipped + num);
					array[3] = new PointF(borderLeftEdge + num2, borderTopEdgeUnclipped + num);
					if (operations == null)
					{
						writer.FillPolygon(borderColorTop, array);
					}
					else
					{
						operations.Add(new FillPolygonOp(borderColorTop, array));
					}
				}
				array = new PointF[4];
				float num4 = borderTopEdgeUnclipped + borderWidthTopUnclipped / 3f * 2f;
				float x;
				float x2;
				if (borderWidthLeft > 0f)
				{
					x = borderLeftEdgeUnclipped + borderWidthLeftUnclipped / 3f * 2f;
					x2 = borderLeftEdgeUnclipped + borderWidthLeftUnclipped;
				}
				else
				{
					x = (x2 = borderLeftEdge);
				}
				float x3;
				float x4;
				if (borderWidthRight > 0f)
				{
					x3 = borderRightEdgeUnclipped - borderWidthRightUnclipped / 3f * 2f;
					x4 = borderRightEdgeUnclipped - borderWidthRightUnclipped;
				}
				else
				{
					x3 = (x4 = borderRightEdge);
				}
				array[0] = new PointF(x, num4);
				array[1] = new PointF(x3, num4);
				array[2] = new PointF(x4, num4 + num);
				array[3] = new PointF(x2, num4 + num);
				if (operations == null)
				{
					writer.FillPolygon(borderColorTop, array);
				}
				else
				{
					operations.Add(new FillPolygonOp(borderColorTop, array));
				}
			}
			else if (operations == null)
			{
				writer.DrawLine(borderColorTop, borderWidthTop, borderStyleTop, borderLeftEdge, borderTop, borderRightEdge, borderTop);
			}
			else
			{
				operations.Add(new DrawLineOp(borderColorTop, borderWidthTop, borderStyleTop, borderLeftEdge, borderTop, borderRightEdge, borderTop));
			}
		}

		internal static bool CalculateImageClippedUnscaledBounds(WriterBase writer, RectangleF bounds, int width, int height, float xOffsetMM, float yOffsetMM, out RectangleF destination, out RectangleF source)
		{
			return CalculateImageClippedUnscaledBounds(writer, bounds, width, height, xOffsetMM, yOffsetMM, null, null, out destination, out source);
		}

		internal static bool CalculateImageClippedUnscaledBounds(WriterBase writer, RectangleF bounds, int width, int height, float xOffsetMM, float yOffsetMM, int? measureImageDpiX, int? measureImageDpiY, out RectangleF destination, out RectangleF source)
		{
			destination = Rectangle.Empty;
			source = Rectangle.Empty;
			if (bounds.Left + xOffsetMM > bounds.Right || bounds.Top + yOffsetMM > bounds.Bottom)
			{
				return false;
			}
			RectangleF rectangleF = default(RectangleF);
			float num = ConvertToMillimeters(width, measureImageDpiX, writer);
			if (xOffsetMM >= 0f)
			{
				rectangleF.X = bounds.Left + xOffsetMM;
				rectangleF.Width = Math.Min(num, bounds.Width - xOffsetMM);
			}
			else
			{
				rectangleF.X = bounds.Left;
				rectangleF.Width = Math.Min(num, num + xOffsetMM);
			}
			float num2 = ConvertToMillimeters(height, measureImageDpiY, writer);
			if (yOffsetMM >= 0f)
			{
				rectangleF.Y = bounds.Top + yOffsetMM;
				rectangleF.Height = Math.Min(num2, bounds.Height - yOffsetMM);
			}
			else
			{
				rectangleF.Y = bounds.Top;
				rectangleF.Height = Math.Min(num2, num2 + yOffsetMM);
			}
			if (rectangleF.Right < 0f || rectangleF.Bottom < 0f)
			{
				return false;
			}
			destination = rectangleF;
			float x = 0f;
			if (xOffsetMM < 0f)
			{
				x = -ConvertToPixels(xOffsetMM, measureImageDpiX, writer);
			}
			float y = 0f;
			if (yOffsetMM < 0f)
			{
				y = -ConvertToPixels(yOffsetMM, measureImageDpiY, writer);
			}
			float width2 = Math.Min(width, ConvertToPixels(rectangleF.Width, measureImageDpiX, writer));
			float height2 = Math.Min(height, ConvertToPixels(rectangleF.Height, measureImageDpiY, writer));
			source = new RectangleF(x, y, width2, height2);
			return true;
		}

		internal static void CalculateColumnZIndexes(RPLTablix tablix, RPLTablixRow row, int currentRow, int[] columnZIndexes)
		{
			if (currentRow < tablix.ColumnHeaderRows)
			{
				for (int i = 0; i < row.NumCells; i++)
				{
					RPLTablixMemberCell rPLTablixMemberCell = row[i] as RPLTablixMemberCell;
					if (rPLTablixMemberCell != null)
					{
						columnZIndexes[rPLTablixMemberCell.ColIndex] = Math.Min(columnZIndexes[rPLTablixMemberCell.ColIndex], CalculateZIndex(rPLTablixMemberCell));
					}
				}
				return;
			}
			for (int j = 0; j < row.NumCells; j++)
			{
				RPLTablixMemberCell rPLTablixMemberCell2 = row[j] as RPLTablixMemberCell;
				if (rPLTablixMemberCell2 != null)
				{
					columnZIndexes[rPLTablixMemberCell2.ColIndex] = CalculateZIndex(rPLTablixMemberCell2);
				}
			}
		}

		internal static int CalculateRowZIndex(RPLTablixRow row)
		{
			int num = int.MaxValue;
			for (int i = 0; i < row.NumCells; i++)
			{
				RPLTablixMemberCell rPLTablixMemberCell = row[i] as RPLTablixMemberCell;
				if (rPLTablixMemberCell != null)
				{
					num = Math.Min(num, CalculateZIndex(rPLTablixMemberCell));
				}
			}
			if (row.OmittedHeaders != null && row.OmittedHeaders.Count > 0)
			{
				for (int j = 0; j < row.OmittedHeaders.Count; j++)
				{
					num = Math.Min(num, CalculateZIndex(row.OmittedHeaders[j]));
				}
			}
			return num;
		}

		internal static int CalculateZIndex(RPLTablixMemberCell header)
		{
			int num = header.TablixMemberDef.Level * 3;
			if (!header.TablixMemberDef.StaticHeadersTree)
			{
				num--;
			}
			return num;
		}
	}
}
