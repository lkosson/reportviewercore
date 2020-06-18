using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.HPBProcessing;
using Microsoft.ReportingServices.Rendering.RichText;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace Microsoft.ReportingServices.Rendering.ImageRenderer
{
	internal class Graphics : GraphicsBase
	{
		private EncoderParameters m_encoderParameters;

		private Bitmap m_firstImage;

		protected Win32ObjectSafeHandle m_hBitmap;

		protected Win32DCSafeHandle m_hdcBitmap;

		private static ImageCodecInfo[] m_encoders = GetGdiImageEncoders();

		internal Graphics(float dpiX, float dpiY)
			: base(dpiX, dpiY)
		{
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (m_firstImage != null)
				{
					m_firstImage.Dispose();
					m_firstImage = null;
				}
				if (m_encoderParameters != null)
				{
					m_encoderParameters.Dispose();
					m_encoderParameters = null;
				}
			}
			if (m_hdcBitmap != null)
			{
				m_hdcBitmap.Close();
				m_hdcBitmap = null;
			}
			if (m_hBitmap != null)
			{
				m_hBitmap.Close();
				m_hBitmap = null;
			}
			base.Dispose(disposing);
		}

		internal virtual void Save(Stream outputStream, PaginationSettings.FormatEncoding outputFormat)
		{
			Bitmap bitmap = null;
			bool flag = true;
			try
			{
				bitmap = System.Drawing.Image.FromHbitmap(m_hBitmap.Handle);
				switch (outputFormat)
				{
				case PaginationSettings.FormatEncoding.BMP:
					bitmap.Save(outputStream, ImageFormat.Bmp);
					break;
				case PaginationSettings.FormatEncoding.GIF:
					bitmap.Save(outputStream, ImageFormat.Gif);
					break;
				case PaginationSettings.FormatEncoding.JPEG:
					bitmap.Save(outputStream, ImageFormat.Jpeg);
					break;
				case PaginationSettings.FormatEncoding.PNG:
					bitmap.Save(outputStream, ImageFormat.Png);
					break;
				case PaginationSettings.FormatEncoding.TIFF:
					if (m_firstImage == null)
					{
						m_firstImage = bitmap;
						flag = false;
						m_encoderParameters = new EncoderParameters(2);
						m_encoderParameters.Param[0] = new EncoderParameter(Encoder.SaveFlag, 18L);
						m_encoderParameters.Param[1] = new EncoderParameter(Encoder.ColorDepth, 24L);
						m_firstImage.Save(outputStream, GetEncoderInfo("image/tiff"), m_encoderParameters);
						EncoderParameter encoderParameter = m_encoderParameters.Param[0];
						m_encoderParameters.Param[0] = new EncoderParameter(Encoder.SaveFlag, 23L);
						if (encoderParameter != null)
						{
							encoderParameter.Dispose();
							encoderParameter = null;
						}
					}
					else
					{
						m_firstImage.SaveAdd(bitmap, m_encoderParameters);
					}
					break;
				}
				outputStream.Flush();
			}
			finally
			{
				if (flag && bitmap != null)
				{
					bitmap.Dispose();
					bitmap = null;
				}
			}
		}

		internal void NewPage(float pageWidth, float pageHeight, int dpiX, int dpiY)
		{
			if (m_graphicsBase != null)
			{
				ReleaseCachedHdc(releaseHdc: true);
				m_graphicsBase.Dispose();
				m_graphicsBase = null;
			}
			if (m_hdcBitmap != null)
			{
				m_hdcBitmap.Close();
				m_hdcBitmap = null;
			}
			if (m_hBitmap != null)
			{
				m_hBitmap.Close();
				m_hBitmap = null;
			}
			IntPtr intPtr = IntPtr.Zero;
			try
			{
				intPtr = Microsoft.ReportingServices.Rendering.RichText.Win32.GetDC(IntPtr.Zero);
				HandleError(intPtr);
				m_hdcBitmap = Microsoft.ReportingServices.Rendering.RichText.Win32.CreateCompatibleDC(intPtr);
				HandleError(m_hdcBitmap);
				Microsoft.ReportingServices.Rendering.RichText.Win32.BITMAPINFOHEADER pbmi = new Microsoft.ReportingServices.Rendering.RichText.Win32.BITMAPINFOHEADER(ConvertToPixels(pageWidth), ConvertToPixels(pageHeight), base.DpiX, base.DpiY);
				IntPtr ppvBits = IntPtr.Zero;
				m_hBitmap = Microsoft.ReportingServices.Rendering.RichText.Win32.CreateDIBSection(m_hdcBitmap, ref pbmi, 0u, ref ppvBits, IntPtr.Zero, 0u);
				HandleError(m_hBitmap);
				Microsoft.ReportingServices.Rendering.RichText.Win32.SelectObject(m_hdcBitmap, m_hBitmap);
				m_graphicsBase = System.Drawing.Graphics.FromHdc(m_hdcBitmap.Handle);
				SetGraphicsProperties(m_graphicsBase);
				m_graphicsBase.Clear(Color.White);
			}
			catch (Exception)
			{
				if (m_hdcBitmap != null)
				{
					m_hdcBitmap.Close();
					m_hdcBitmap = null;
				}
				if (m_hBitmap != null)
				{
					m_hBitmap.Close();
					m_hBitmap = null;
				}
				throw;
			}
			finally
			{
				if (IntPtr.Zero != intPtr)
				{
					Microsoft.ReportingServices.Rendering.RichText.Win32.ReleaseDC(IntPtr.Zero, intPtr);
				}
			}
		}

		private void HandleError(Win32DCSafeHandle handle)
		{
			if (handle.IsInvalid)
			{
				throw new ReportRenderingException(Marshal.GetExceptionForHR(Marshal.GetLastWin32Error()));
			}
		}

		private void HandleError(Win32ObjectSafeHandle handle)
		{
			if (handle.IsInvalid)
			{
				throw new ReportRenderingException(Marshal.GetExceptionForHR(Marshal.GetLastWin32Error()));
			}
		}

		private void HandleError(IntPtr handle)
		{
			if (IntPtr.Zero == handle)
			{
				throw new ReportRenderingException(Marshal.GetExceptionForHR(Marshal.GetLastWin32Error()));
			}
		}

		internal void DrawLine(Pen pen, float x1, float y1, float x2, float y2)
		{
			ReleaseCachedHdc(releaseHdc: true);
			ExecuteSync(delegate
			{
				m_graphicsBase.DrawLine(pen, ConvertToPixels(x1), ConvertToPixels(y1), ConvertToPixels(x2), ConvertToPixels(y2));
			});
		}

		internal void DrawImage(System.Drawing.Image image, RectangleF destination, RectangleF source)
		{
			DrawImage(image, destination, source, tile: true);
		}

		internal void DrawImage(System.Drawing.Image image, RectangleF destination, RectangleF source, bool tile)
		{
			ReleaseCachedHdc(releaseHdc: true);
			ExecuteSync(delegate
			{
				ImageAttributes imageAttributes = null;
				try
				{
					if (tile)
					{
						imageAttributes = new ImageAttributes();
						imageAttributes.SetWrapMode(WrapMode.Tile);
					}
					PointF[] destPoints = new PointF[3]
					{
						new PointF(ConvertToPixels(destination.Location.X), ConvertToPixels(destination.Location.Y)),
						new PointF(ConvertToPixels(destination.Location.X + destination.Width), ConvertToPixels(destination.Location.Y)),
						new PointF(ConvertToPixels(destination.Location.X), ConvertToPixels(destination.Location.Y + destination.Height))
					};
					m_graphicsBase.DrawImage(image, destPoints, source, GraphicsUnit.Pixel, imageAttributes);
				}
				finally
				{
					if (imageAttributes != null)
					{
						imageAttributes.Dispose();
						imageAttributes = null;
					}
				}
			});
		}

		internal void DrawRectangle(Pen pen, RectangleF rectangle)
		{
			ReleaseCachedHdc(releaseHdc: true);
			ExecuteSync(delegate
			{
				m_graphicsBase.DrawRectangle(pen, ConvertToPixels(rectangle.X), ConvertToPixels(rectangle.Y), ConvertToPixels(rectangle.Width), ConvertToPixels(rectangle.Height));
			});
		}

		internal void FillPolygon(Brush brush, PointF[] polygon)
		{
			ReleaseCachedHdc(releaseHdc: true);
			ExecuteSync(delegate
			{
				Point[] array = new Point[polygon.Length];
				for (int i = 0; i < polygon.Length; i++)
				{
					PointF pointF = polygon[i];
					array[i].X = ConvertToPixels(pointF.X);
					array[i].Y = ConvertToPixels(pointF.Y);
				}
				m_graphicsBase.FillPolygon(brush, array);
			});
		}

		internal void FillRectangle(Brush brush, RectangleF rectangle)
		{
			ReleaseCachedHdc(releaseHdc: true);
			ExecuteSync(delegate
			{
				m_graphicsBase.FillRectangle(brush, ConvertToPixels(rectangle.X), ConvertToPixels(rectangle.Y), ConvertToPixels(rectangle.Width), ConvertToPixels(rectangle.Height));
			});
		}

		internal void ResetClipAndTransform(RectangleF bounds)
		{
			ReleaseCachedHdc(releaseHdc: true);
			ExecuteSync(delegate
			{
				m_graphicsBase.ResetClip();
				m_graphicsBase.ResetTransform();
				System.Drawing.Rectangle clip = new System.Drawing.Rectangle(ConvertToPixels(bounds.X), ConvertToPixels(bounds.Y), ConvertToPixels(bounds.Width), ConvertToPixels(bounds.Height));
				m_graphicsBase.SetClip(clip);
				using (Matrix matrix = new Matrix())
				{
					matrix.Translate(clip.Left, clip.Top);
					m_graphicsBase.Transform = matrix;
				}
			});
		}

		internal void RotateTransform(float angle)
		{
			ReleaseCachedHdc(releaseHdc: true);
			ExecuteSync(delegate
			{
				m_graphicsBase.RotateTransform(angle);
			});
		}

		internal void EndReport(PaginationSettings.FormatEncoding outputFormat)
		{
			if (outputFormat == PaginationSettings.FormatEncoding.TIFF)
			{
				EncoderParameter encoderParameter = m_encoderParameters.Param[0];
				m_encoderParameters.Param[0] = new EncoderParameter(Encoder.SaveFlag, 20L);
				if (encoderParameter != null)
				{
					encoderParameter.Dispose();
					encoderParameter = null;
				}
				m_firstImage.SaveAdd(m_encoderParameters);
			}
		}

		protected static void SetGraphicsProperties(System.Drawing.Graphics graphics)
		{
			graphics.CompositingMode = CompositingMode.SourceOver;
			graphics.PageUnit = GraphicsUnit.Pixel;
			graphics.PixelOffsetMode = PixelOffsetMode.Default;
			graphics.SmoothingMode = SmoothingMode.Default;
			graphics.TextRenderingHint = TextRenderingHint.SystemDefault;
		}

		private static ImageCodecInfo GetEncoderInfo(string mimeType)
		{
			if (m_encoders == null)
			{
				return null;
			}
			for (int i = 0; i < m_encoders.Length; i++)
			{
				if (m_encoders[i].MimeType == mimeType)
				{
					return m_encoders[i];
				}
			}
			return null;
		}

		private static ImageCodecInfo[] GetGdiImageEncoders()
		{
			ImageCodecInfo[] array = null;
			if (m_encoders == null)
			{
				return ImageCodecInfo.GetImageEncoders();
			}
			return m_encoders;
		}
	}
}
