using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.Rendering.HPBProcessing;
using Microsoft.ReportingServices.Rendering.RichText;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Linq;

namespace Microsoft.ReportingServices.Rendering.ImageRenderer
{
	internal sealed class MetafileGraphics : Graphics
	{
		private class BoolRef
		{
			public bool IsValid;

			public bool Value;
		}

		private static BoolRef m_isAnamorphicResult = new BoolRef();

		private static object m_syncRoot = new object();

		private Metafile m_metafile;

		internal MetafileGraphics(float dpiX, float dpiY)
			: base(dpiX, dpiY)
		{
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && m_metafile != null)
			{
				ExecuteSync(delegate
				{
					m_metafile.Dispose();
				});
				m_metafile = null;
			}
			base.Dispose(disposing);
		}

		internal override void CacheHdc(bool createNewHdc)
		{
			base.CacheHdc(createNewHdc: true);
		}

		internal override void ReleaseCachedHdc(bool releaseHdc)
		{
			base.ReleaseCachedHdc(releaseHdc: true);
		}

		internal override void ExecuteSync(SynchronizedOperation synchronizedOperation)
		{
			lock (m_syncRoot)
			{
				synchronizedOperation();
			}
		}

		internal override void Save(Stream outputStream, PaginationSettings.FormatEncoding outputFormat)
		{
			if (m_metafile != null)
			{
				ExecuteSync(delegate
				{
					m_metafile.Dispose();
				});
				m_metafile = null;
			}
			outputStream.Flush();
			(outputStream as IRenderStream)?.Finish();
		}

		internal void NewPage(Stream stream, PaginationSettings.FormatEncoding outputFormat, float pageWidth, float pageHeight, int dpiX, int dpiY)
		{
			if (m_graphicsBase != null)
			{
				ReleaseCachedHdc(releaseHdc: true);
				m_graphicsBase.Dispose();
				m_graphicsBase = null;
			}
			System.Drawing.Graphics graphics = CreateMetafileGraphics();
			IntPtr hdc = graphics.GetHdc();
			EmfType emfType = EmfType.EmfPlusOnly;
			if (outputFormat == PaginationSettings.FormatEncoding.EMF)
			{
				emfType = EmfType.EmfPlusDual;
			}
			try
			{
				RectangleF rect = CalculateMetafileRectangle(hdc, pageWidth, pageHeight, dpiX, dpiY);
				ExecuteSync(delegate
				{
					m_metafile = new Metafile(stream, hdc, rect, MetafileFrameUnit.GdiCompatible, emfType);
					m_graphicsBase = System.Drawing.Graphics.FromImage(m_metafile);
					Graphics.SetGraphicsProperties(m_graphicsBase);
				});
			}
			finally
			{
				if (hdc != IntPtr.Zero)
				{
					graphics.ReleaseHdc(hdc);
				}
				if (graphics != null)
				{
					graphics.Dispose();
					graphics = null;
				}
			}
		}

		internal System.Drawing.Graphics CreateMetafileGraphics()
		{
			if (IsAspectRatioMetafileAnamorphic())
			{
				string[] logicalPrinters = new string[2]
				{
					"Microsoft Print to PDF",
					"Microsoft XPS Document Writer"
				};
				string text = PrinterSettings.InstalledPrinters.OfType<string>().FirstOrDefault((string name) => logicalPrinters.Contains(name));
				if (!string.IsNullOrEmpty(text))
				{
					return new PrinterSettings
					{
						PrinterName = text
					}.CreateMeasurementGraphics();
				}
			}
			return System.Drawing.Graphics.FromImage(m_imageBase);
		}

		internal static bool IsAspectRatioMetafileAnamorphic()
		{
			if (!m_isAnamorphicResult.IsValid)
			{
				lock (m_isAnamorphicResult)
				{
					using (Stream stream = new MemoryStream())
					{
						using (Bitmap image = new Bitmap(2, 2))
						{
							using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(image))
							{
								IntPtr hdc = graphics.GetHdc();
								try
								{
									using (Metafile metafile = new Metafile(stream, hdc, new System.Drawing.Rectangle(0, 0, 100, 100), MetafileFrameUnit.GdiCompatible, EmfType.EmfPlusDual))
									{
										using (System.Drawing.Graphics graphics2 = System.Drawing.Graphics.FromImage(metafile))
										{
											graphics2.DrawRectangle(Pens.Black, new System.Drawing.Rectangle(0, 0, 10, 10));
										}
										float verticalResolution = metafile.VerticalResolution;
										float horizontalResolution = metafile.HorizontalResolution;
										m_isAnamorphicResult.IsValid = true;
										m_isAnamorphicResult.Value = (Math.Abs((verticalResolution - horizontalResolution) / horizontalResolution * 100f) > 3f);
									}
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
			return m_isAnamorphicResult.Value;
		}

		private RectangleF CalculateMetafileRectangle(IntPtr hdc, float pageWidth, float pageHeight, float DpiX, float DpiY)
		{
			int deviceCaps = Microsoft.ReportingServices.Rendering.RichText.Win32.GetDeviceCaps(hdc, 4);
			int deviceCaps2 = Microsoft.ReportingServices.Rendering.RichText.Win32.GetDeviceCaps(hdc, 6);
			int deviceCaps3 = Microsoft.ReportingServices.Rendering.RichText.Win32.GetDeviceCaps(hdc, 118);
			int deviceCaps4 = Microsoft.ReportingServices.Rendering.RichText.Win32.GetDeviceCaps(hdc, 117);
			double num = SharedRenderer.ConvertToPixels(pageWidth, DpiX);
			double num2 = SharedRenderer.ConvertToPixels(pageHeight, DpiY);
			float width = (float)(num * (double)deviceCaps * 100.0) / (float)deviceCaps3;
			float height = (float)(num2 * (double)deviceCaps2 * 100.0) / (float)deviceCaps4;
			return new RectangleF(0f, 0f, width, height);
		}
	}
}
