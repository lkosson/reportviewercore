using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class DynamicImageInstance : DataRegionInstance
	{
		public enum ImageType
		{
			PNG,
			EMF
		}

		protected float m_dpiX = 96f;

		protected float m_dpiY = 96f;

		protected double? m_widthOverride;

		protected double? m_heightOverride;

		protected virtual int WidthInPixels => MappingHelper.ToIntPixels(((ReportItem)m_reportElementDef).Width, m_dpiX);

		protected virtual int HeightInPixels => MappingHelper.ToIntPixels(((ReportItem)m_reportElementDef).Height, m_dpiX);

		internal DynamicImageInstance(DataRegion reportItemDef)
			: base(reportItemDef)
		{
		}

		public virtual void SetDpi(int xDpi, int yDpi)
		{
			m_dpiX = xDpi;
			m_dpiY = yDpi;
		}

		public void SetSize(double width, double height)
		{
			m_widthOverride = width;
			m_heightOverride = height;
		}

		public Stream GetImage()
		{
			bool hasImageMap;
			return GetImage(ImageType.PNG, out hasImageMap);
		}

		public Stream GetImage(ImageType type)
		{
			bool hasImageMap;
			return GetImage(type, out hasImageMap);
		}

		public Stream GetImage(out ActionInfoWithDynamicImageMapCollection actionImageMaps)
		{
			return GetImage(ImageType.PNG, out actionImageMaps);
		}

		public virtual Stream GetImage(ImageType type, out ActionInfoWithDynamicImageMapCollection actionImageMaps)
		{
			try
			{
				GetImage(type, out actionImageMaps, out Stream image);
				return image;
			}
			catch (Exception exception)
			{
				actionImageMaps = null;
				return CreateExceptionImage(exception);
			}
		}

		protected virtual Stream GetImage(ImageType type, out bool hasImageMap)
		{
			ActionInfoWithDynamicImageMapCollection actionImageMaps;
			Stream image = GetImage(type, out actionImageMaps);
			hasImageMap = (actionImageMaps != null);
			return image;
		}

		protected MemoryStream CreateExceptionImage(Exception exception)
		{
			return CreateExceptionImage(exception, WidthInPixels, HeightInPixels, m_dpiX, m_dpiY);
		}

		internal static MemoryStream CreateExceptionImage(Exception exception, int width, int height, float dpiX, float dpiY)
		{
			Bitmap bitmap = null;
			Graphics graphics = null;
			Brush brush = null;
			Brush brush2 = null;
			Pen pen = null;
			Pen pen2 = null;
			Font font = null;
			MemoryStream memoryStream = new MemoryStream();
			try
			{
				bitmap = new Bitmap(width, height);
				bitmap.SetResolution(dpiX, dpiY);
				graphics = Graphics.FromImage(bitmap);
				brush = new SolidBrush(Color.White);
				graphics.FillRectangle(brush, 0, 0, width, height);
				float num = (float)MappingHelper.ToPixels(new ReportSize("1pt"), dpiX);
				float num2 = (float)MappingHelper.ToPixels(new ReportSize("1pt"), dpiY);
				pen = new Pen(Color.Black, num);
				pen2 = new Pen(Color.Black, num2);
				graphics.DrawLine(pen, num, num2, (float)width - num, num2);
				graphics.DrawLine(pen2, (float)width - num, num2, (float)width - num, (float)height - num2);
				graphics.DrawLine(pen, (float)width - num, (float)height - num2, num, (float)height - num2);
				graphics.DrawLine(pen2, num, (float)height - num2, num, num2);
				brush2 = new SolidBrush(Color.Black);
				font = MappingHelper.GetDefaultFont();
				graphics.DrawString(GetInnerMostException(exception).Message, font, brush2, new RectangleF(num, num2, (float)width - num, (float)height - num2));
				bitmap.Save(memoryStream, ImageFormat.Png);
				return memoryStream;
			}
			catch (Exception ex)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(ex))
				{
					throw;
				}
				Global.Tracer.Trace(TraceLevel.Verbose, ex.Message);
				return null;
			}
			finally
			{
				if (brush != null)
				{
					brush.Dispose();
					brush = null;
				}
				if (pen != null)
				{
					pen.Dispose();
					pen = null;
				}
				if (pen2 != null)
				{
					pen2.Dispose();
					pen2 = null;
				}
				if (brush2 != null)
				{
					brush2.Dispose();
					brush2 = null;
				}
				if (font != null)
				{
					font.Dispose();
					font = null;
				}
				if (graphics != null)
				{
					graphics.Dispose();
					graphics = null;
				}
				if (bitmap != null)
				{
					bitmap.Dispose();
					bitmap = null;
				}
			}
		}

		protected abstract void GetImage(ImageType type, out ActionInfoWithDynamicImageMapCollection actionImageMaps, out Stream image);

		private static Exception GetInnerMostException(Exception exception)
		{
			Exception ex = exception;
			while (ex.InnerException != null)
			{
				ex = ex.InnerException;
			}
			return ex;
		}
	}
}
