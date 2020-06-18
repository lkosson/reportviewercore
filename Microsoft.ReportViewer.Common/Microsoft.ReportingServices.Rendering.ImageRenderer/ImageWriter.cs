using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.Rendering.HPBProcessing;
using Microsoft.ReportingServices.Rendering.RichText;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;

namespace Microsoft.ReportingServices.Rendering.ImageRenderer
{
	internal sealed class ImageWriter : WriterBase
	{
		internal const char StreamNameSeparator = '_';

		private Graphics m_graphics;

		private Dictionary<string, System.Drawing.Image> m_cachedImages = new Dictionary<string, System.Drawing.Image>();

		internal PaginationSettings.FormatEncoding OutputFormat;

		private RectangleF MetafileRectangle = RectangleF.Empty;

		private Dictionary<string, Pen> m_pens = new Dictionary<string, Pen>();

		private Dictionary<string, Brush> m_brushes = new Dictionary<string, Brush>();

		private System.Drawing.Rectangle m_bodyRect = System.Drawing.Rectangle.Empty;

		private Microsoft.ReportingServices.Rendering.RichText.Win32.POINT m_prevViewportOrg;

		private int m_dpiX;

		private int m_dpiY;

		private int m_measureImageDpiX;

		private int m_measureImageDpiY;

		private int DEFAULT_RESOLUTION_X = 96;

		private int DYNAMIC_IMAGE_MIN_RESOLUTION_X = 300;

		private int DEFAULT_RESOLUTION_Y = 96;

		private int DYNAMIC_IMAGE_MIN_RESOLUTION_Y = 300;

		internal bool IsEmf
		{
			get
			{
				if (OutputFormat != PaginationSettings.FormatEncoding.EMFPLUS)
				{
					return OutputFormat == PaginationSettings.FormatEncoding.EMF;
				}
				return true;
			}
		}

		internal Stream OutputStream
		{
			set
			{
				m_outputStream = value;
			}
		}

		internal ImageWriter(Renderer renderer, Stream stream, bool disposeRenderer, CreateAndRegisterStream createAndRegisterStream, int measureImageDpiX, int measureImageDpiY)
			: base(renderer, stream, disposeRenderer, createAndRegisterStream)
		{
			m_measureImageDpiX = measureImageDpiX;
			m_measureImageDpiY = measureImageDpiY;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (m_cachedImages != null)
				{
					foreach (string key in m_cachedImages.Keys)
					{
						m_cachedImages[key].Dispose();
					}
					m_cachedImages = null;
				}
				if (m_pens != null)
				{
					foreach (string key2 in m_pens.Keys)
					{
						m_pens[key2].Dispose();
					}
					m_pens = null;
				}
				if (m_brushes != null)
				{
					foreach (string key3 in m_brushes.Keys)
					{
						m_brushes[key3].Dispose();
					}
					m_brushes = null;
				}
				if (m_graphics != null)
				{
					m_graphics.Dispose();
					m_graphics = null;
				}
			}
			base.Dispose(disposing);
		}

		~ImageWriter()
		{
			Dispose(disposing: false);
		}

		internal override void BeginReport(int dpiX, int dpiY)
		{
			m_dpiX = dpiX;
			m_dpiY = dpiY;
			if (!IsEmf)
			{
				m_graphics = new Graphics(dpiX, dpiY);
			}
			else
			{
				m_graphics = new MetafileGraphics(dpiX, dpiY);
			}
			m_commonGraphics = m_graphics;
		}

		internal override void BeginPage(float pageWidth, float pageHeight)
		{
			if (!IsEmf)
			{
				m_graphics.NewPage(pageWidth, pageHeight, m_commonGraphics.DpiX, m_commonGraphics.DpiY);
			}
			else
			{
				((MetafileGraphics)m_graphics).NewPage(m_outputStream, OutputFormat, pageWidth, pageHeight, m_commonGraphics.DpiX, m_commonGraphics.DpiY);
			}
		}

		internal override void BeginPageSection(RectangleF bounds)
		{
			base.BeginPageSection(bounds);
			int dpiX = m_commonGraphics.DpiX;
			int dpiY = m_commonGraphics.DpiY;
			m_bodyRect = new System.Drawing.Rectangle(SharedRenderer.ConvertToPixels(bounds.X, dpiX), SharedRenderer.ConvertToPixels(bounds.Y, dpiY), SharedRenderer.ConvertToPixels(bounds.Width + HalfPixelWidthX, dpiX), SharedRenderer.ConvertToPixels(bounds.Height + HalfPixelWidthY, dpiY));
			m_graphics.ResetClipAndTransform(new RectangleF(bounds.Left, bounds.Top, bounds.Width + HalfPixelWidthX, bounds.Height + HalfPixelWidthY));
		}

		internal override RectangleF CalculateColumnBounds(RPLReportSection reportSection, RPLPageLayout pageLayout, RPLItemMeasurement column, int columnNumber, float top, float columnHeight, float columnWidth)
		{
			return HardPageBreakShared.CalculateColumnBounds(reportSection, pageLayout, columnNumber, top, columnHeight);
		}

		internal override RectangleF CalculateHeaderBounds(RPLReportSection section, RPLPageLayout pageLayout, float top, float width)
		{
			return HardPageBreakShared.CalculateHeaderBounds(section, pageLayout, top, width);
		}

		internal override RectangleF CalculateFooterBounds(RPLReportSection section, RPLPageLayout pageLayout, float top, float width)
		{
			return HardPageBreakShared.CalculateFooterBounds(section, pageLayout, top, width);
		}

		internal override void DrawBackgroundImage(RPLImageData imageData, RPLFormat.BackgroundRepeatTypes repeat, PointF start, RectangleF position)
		{
			System.Drawing.Image image;
			bool image2 = GetImage(imageData.ImageName, imageData.ImageData, imageData.ImageDataOffset, dynamicImage: false, out image);
			if (image == null)
			{
				return;
			}
			RectangleF destination;
			RectangleF source;
			if (repeat == RPLFormat.BackgroundRepeatTypes.Clip)
			{
				if (SharedRenderer.CalculateImageClippedUnscaledBounds(this, position, image.Width, image.Height, start.X, start.Y, m_measureImageDpiX, m_measureImageDpiY, out destination, out source))
				{
					m_graphics.DrawImage(image, destination, source);
				}
			}
			else
			{
				float num = SharedRenderer.ConvertToMillimeters(image.Width, m_measureImageDpiX);
				float num2 = SharedRenderer.ConvertToMillimeters(image.Height, m_measureImageDpiY);
				float num3 = position.Width;
				if (repeat == RPLFormat.BackgroundRepeatTypes.RepeatY)
				{
					num3 = num;
				}
				float num4 = position.Height;
				if (repeat == RPLFormat.BackgroundRepeatTypes.RepeatX)
				{
					num4 = num2;
				}
				for (float num5 = start.X; num5 < num3; num5 += num)
				{
					for (float num6 = start.Y; num6 < num4; num6 += num2)
					{
						if (SharedRenderer.CalculateImageClippedUnscaledBounds(this, position, image.Width, image.Height, num5, num6, m_measureImageDpiX, m_measureImageDpiY, out destination, out source))
						{
							m_graphics.DrawImage(image, destination, source);
						}
					}
				}
			}
			if (!image2)
			{
				image.Dispose();
				image = null;
			}
		}

		internal override void DrawLine(Color color, float size, RPLFormat.BorderStyles style, float x1, float y1, float x2, float y2)
		{
			m_graphics.DrawLine(GDIPen.GetPen(m_pens, color, ConvertToPixels(size), style), x1, y1, x2, y2);
		}

		internal void GetDefaultImage(out System.Drawing.Image gdiImage)
		{
			string key = "__int__InvalidImage";
			if (m_cachedImages.TryGetValue(key, out gdiImage))
			{
				return;
			}
			Bitmap bitmap = Renderer.ImageResources["InvalidImage"];
			Bitmap bitmap2 = null;
			lock (bitmap)
			{
				using (MemoryStream stream = new MemoryStream())
				{
					bitmap.Save(stream, bitmap.RawFormat);
					bitmap2 = new Bitmap(stream);
				}
			}
			bitmap2.SetResolution(m_commonGraphics.DpiX, m_commonGraphics.DpiY);
			gdiImage = bitmap2;
			m_cachedImages.Add(key, gdiImage);
		}

		internal override void DrawDynamicImage(string imageName, Stream imageStream, long imageDataOffset, RectangleF position)
		{
			System.Drawing.Image image;
			bool flag = GetImage(imageName, imageStream, imageDataOffset, dynamicImage: true, out image);
			if (image == null)
			{
				GetDefaultImage(out image);
				flag = true;
			}
			GetScreenDpi(out int dpiX, out int _);
			float num = 1f * (float)DEFAULT_RESOLUTION_X / (float)dpiX;
			RectangleF source = new RectangleF(0f, 0f, num * (float)image.Width, num * (float)image.Height);
			m_graphics.DrawImage(image, position, source, tile: false);
			if (!flag)
			{
				image.Dispose();
				image = null;
			}
		}

		internal override void DrawImage(RectangleF position, RPLImage image, RPLImageProps instanceProperties, RPLImagePropsDef definitionProperties)
		{
			RPLImageData image2 = instanceProperties.Image;
			System.Drawing.Image image3;
			bool flag = GetImage(image2.ImageName, image2.ImageData, image2.ImageDataOffset, dynamicImage: false, out image3);
			RPLFormat.Sizings sizing = definitionProperties.Sizing;
			if (image3 == null)
			{
				GetDefaultImage(out image3);
				flag = true;
				sizing = RPLFormat.Sizings.Clip;
			}
			GDIImageProps gDIImageProps = new GDIImageProps(image3);
			SharedRenderer.CalculateImageRectangle(position, gDIImageProps.Width, gDIImageProps.Height, m_measureImageDpiX, m_measureImageDpiY, sizing, out RectangleF imagePositionAndSize, out RectangleF imagePortion);
			m_graphics.DrawImage(image3, imagePositionAndSize, imagePortion);
			if (!flag)
			{
				image3.Dispose();
				image3 = null;
			}
		}

		internal override void DrawRectangle(Color color, float size, RPLFormat.BorderStyles style, RectangleF rectangle)
		{
			m_graphics.DrawRectangle(GDIPen.GetPen(m_pens, color, ConvertToPixels(size), style), rectangle);
		}

		internal override void DrawTextRun(Win32DCSafeHandle hdc, FontCache fontCache, ReportTextBox textBox, Microsoft.ReportingServices.Rendering.RichText.TextRun run, TypeCode typeCode, RPLFormat.TextAlignments textAlign, RPLFormat.VerticalAlignments verticalAlign, RPLFormat.WritingModes writingMode, RPLFormat.Directions direction, Point pointPosition, System.Drawing.Rectangle layoutRectangle, int lineHeight, int baselineY)
		{
			if (!string.IsNullOrEmpty(run.Text))
			{
				int x;
				int baselineY2;
				switch (writingMode)
				{
				case RPLFormat.WritingModes.Horizontal:
					x = layoutRectangle.X + pointPosition.X;
					baselineY2 = layoutRectangle.Y + baselineY;
					break;
				case RPLFormat.WritingModes.Vertical:
					x = layoutRectangle.X + (layoutRectangle.Width - baselineY);
					baselineY2 = layoutRectangle.Y + pointPosition.X;
					break;
				case RPLFormat.WritingModes.Rotate270:
					x = layoutRectangle.X + baselineY;
					baselineY2 = layoutRectangle.Y + layoutRectangle.Height - pointPosition.X;
					break;
				default:
					throw new NotSupportedException();
				}
				Underline underline = null;
				if (run.UnderlineHeight > 0)
				{
					underline = new Underline(run, hdc, fontCache, layoutRectangle, pointPosition.X, baselineY, writingMode);
				}
				if (!IsEmf)
				{
					Microsoft.ReportingServices.Rendering.RichText.TextBox.DrawTextRun(run, hdc, fontCache, x, baselineY2, underline);
				}
				else
				{
					Microsoft.ReportingServices.Rendering.RichText.TextBox.ExtDrawTextRun(run, hdc, fontCache, x, baselineY2, underline);
				}
			}
		}

		internal override void EndPage()
		{
			m_graphics.ReleaseCachedHdc(releaseHdc: true);
			m_graphics.Save(m_outputStream, OutputFormat);
		}

		internal override void EndReport()
		{
			m_graphics.EndReport(OutputFormat);
			m_outputStream.Flush();
		}

		internal override void FillPolygon(Color color, PointF[] polygon)
		{
			m_graphics.FillPolygon(GDIBrush.GetBrush(m_brushes, color), polygon);
		}

		internal override void FillRectangle(Color color, RectangleF rectangle)
		{
			m_graphics.FillRectangle(GDIBrush.GetBrush(m_brushes, color), rectangle);
		}

		private bool GetImage(string imageName, byte[] imageBytes, long imageDataOffset, bool dynamicImage, out System.Drawing.Image image)
		{
			image = null;
			if (dynamicImage || string.IsNullOrEmpty(imageName) || !m_cachedImages.TryGetValue(imageName, out image))
			{
				if (!SharedRenderer.GetImage(m_renderer.RplReport, ref imageBytes, imageDataOffset))
				{
					return false;
				}
				try
				{
					image = System.Drawing.Image.FromStream(new MemoryStream(imageBytes));
				}
				catch
				{
					return false;
				}
				AddImageToCache(image, dynamicImage, imageName);
			}
			if (!dynamicImage)
			{
				return !string.IsNullOrEmpty(imageName);
			}
			return false;
		}

		private bool GetImage(string imageName, Stream imageStream, long imageDataOffset, bool dynamicImage, out System.Drawing.Image image)
		{
			image = null;
			if (dynamicImage || string.IsNullOrEmpty(imageName) || !m_cachedImages.TryGetValue(imageName, out image))
			{
				if (imageStream == null)
				{
					imageStream = SharedRenderer.GetEmbeddedImageStream(m_renderer.RplReport, imageDataOffset, base.CreateAndRegisterStream, imageName);
					if (imageStream == null)
					{
						return false;
					}
				}
				if (imageStream.Position != 0L && imageStream.CanSeek)
				{
					imageStream.Position = 0L;
				}
				try
				{
					image = System.Drawing.Image.FromStream(imageStream);
				}
				catch
				{
					return false;
				}
				AddImageToCache(image, dynamicImage, imageName);
			}
			if (!dynamicImage)
			{
				return !string.IsNullOrEmpty(imageName);
			}
			return false;
		}

		private void AddImageToCache(System.Drawing.Image image, bool dynamicImage, string imageName)
		{
			Bitmap bitmap = image as Bitmap;
			if (bitmap != null)
			{
				SetResolution(bitmap, dynamicImage);
			}
			if (!dynamicImage && !string.IsNullOrEmpty(imageName))
			{
				m_cachedImages.Add(imageName, image);
			}
		}

		private void SetResolution(Bitmap bitmap, bool dynamicImage)
		{
			int num = m_dpiX;
			int num2 = m_dpiY;
			if (dynamicImage)
			{
				if (DEFAULT_RESOLUTION_X == num)
				{
					num = DYNAMIC_IMAGE_MIN_RESOLUTION_X;
				}
				if (DEFAULT_RESOLUTION_Y == num2)
				{
					num2 = DYNAMIC_IMAGE_MIN_RESOLUTION_Y;
				}
			}
			bitmap.SetResolution(num, num2);
		}

		internal override void ClipTextboxRectangle(Win32DCSafeHandle hdc, RectangleF position)
		{
			if (m_bodyRect.X != 0 || m_bodyRect.Y != 0)
			{
				if (!Microsoft.ReportingServices.Rendering.RichText.Win32.GetViewportOrgEx(hdc, out m_prevViewportOrg))
				{
					int lastWin32Error = Marshal.GetLastWin32Error();
					throw new Exception(string.Format(CultureInfo.InvariantCulture, ImageRendererRes.Win32ErrorInfo, "GetViewportOrgEx", lastWin32Error));
				}
				if (!Microsoft.ReportingServices.Rendering.RichText.Win32.SetViewportOrgEx(hdc, m_bodyRect.X, m_bodyRect.Y, Win32ObjectSafeHandle.Zero))
				{
					int lastWin32Error2 = Marshal.GetLastWin32Error();
					throw new Exception(string.Format(CultureInfo.InvariantCulture, ImageRendererRes.Win32ErrorInfo, "SetViewportOrgEx", lastWin32Error2));
				}
			}
			System.Drawing.Rectangle rectangle = new System.Drawing.Rectangle(SharedRenderer.ConvertToPixels(position.X, m_commonGraphics.DpiX), SharedRenderer.ConvertToPixels(position.Y, m_commonGraphics.DpiY), SharedRenderer.ConvertToPixels(position.Width, m_commonGraphics.DpiX), SharedRenderer.ConvertToPixels(position.Height, m_commonGraphics.DpiY));
			if (position.X < 0f)
			{
				rectangle.Width += rectangle.X;
				rectangle.X = 0;
			}
			if (position.Y < 0f)
			{
				rectangle.Height += rectangle.Y;
				rectangle.Y = 0;
			}
			rectangle.X += m_bodyRect.X;
			rectangle.Y += m_bodyRect.Y;
			if (rectangle.Right > m_bodyRect.Right)
			{
				rectangle.Width = m_bodyRect.Right - rectangle.Left;
			}
			if (rectangle.Bottom > m_bodyRect.Bottom)
			{
				rectangle.Height = m_bodyRect.Bottom - rectangle.Top;
			}
			Win32ObjectSafeHandle win32ObjectSafeHandle = Microsoft.ReportingServices.Rendering.RichText.Win32.CreateRectRgn(rectangle.X, rectangle.Y, rectangle.Right, rectangle.Bottom);
			if (win32ObjectSafeHandle.IsInvalid)
			{
				return;
			}
			try
			{
				if (Microsoft.ReportingServices.Rendering.RichText.Win32.SelectClipRgn(hdc, win32ObjectSafeHandle) == 0)
				{
					int lastWin32Error3 = Marshal.GetLastWin32Error();
					throw new Exception(string.Format(CultureInfo.InvariantCulture, ImageRendererRes.Win32ErrorInfo, "SelectClipRgn", lastWin32Error3));
				}
			}
			finally
			{
				win32ObjectSafeHandle.Close();
			}
		}

		internal override void UnClipTextboxRectangle(Win32DCSafeHandle hdc)
		{
			if (Microsoft.ReportingServices.Rendering.RichText.Win32.SelectClipRgn(hdc, Win32ObjectSafeHandle.Zero) == 0)
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				throw new Exception(string.Format(CultureInfo.InvariantCulture, ImageRendererRes.Win32ErrorInfo, "SelectClipRgn", lastWin32Error));
			}
			if ((m_bodyRect.X != 0 || m_bodyRect.Y != 0) && !Microsoft.ReportingServices.Rendering.RichText.Win32.SetViewportOrgEx(hdc, m_prevViewportOrg.x, m_prevViewportOrg.y, Win32ObjectSafeHandle.Zero))
			{
				int lastWin32Error2 = Marshal.GetLastWin32Error();
				throw new Exception(string.Format(CultureInfo.InvariantCulture, ImageRendererRes.Win32ErrorInfo, "SetViewportOrgEx", lastWin32Error2));
			}
		}

		internal static void GetScreenDpi(out int dpiX, out int dpiY)
		{
			using (Bitmap image = new Bitmap(2, 2))
			{
				using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(image))
				{
					IntPtr hdc = graphics.GetHdc();
					try
					{
						int deviceCaps = Microsoft.ReportingServices.Rendering.RichText.Win32.GetDeviceCaps(hdc, 88);
						int deviceCaps2 = Microsoft.ReportingServices.Rendering.RichText.Win32.GetDeviceCaps(hdc, 90);
						int deviceCaps3 = Microsoft.ReportingServices.Rendering.RichText.Win32.GetDeviceCaps(hdc, 8);
						int deviceCaps4 = Microsoft.ReportingServices.Rendering.RichText.Win32.GetDeviceCaps(hdc, 10);
						int deviceCaps5 = Microsoft.ReportingServices.Rendering.RichText.Win32.GetDeviceCaps(hdc, 118);
						int deviceCaps6 = Microsoft.ReportingServices.Rendering.RichText.Win32.GetDeviceCaps(hdc, 117);
						dpiX = (int)Math.Floor(1.0 * (double)deviceCaps * (double)deviceCaps5 / (double)deviceCaps3);
						dpiY = (int)Math.Floor(1.0 * (double)deviceCaps2 * (double)deviceCaps6 / (double)deviceCaps4);
					}
					finally
					{
						graphics.ReleaseHdc(hdc);
					}
				}
			}
		}
	}
}
