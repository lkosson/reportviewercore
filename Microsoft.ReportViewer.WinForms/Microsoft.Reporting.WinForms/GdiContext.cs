using Microsoft.ReportingServices.Rendering.ImageRenderer;
using Microsoft.ReportingServices.Rendering.RichText;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Reflection;
using System.Resources;

namespace Microsoft.Reporting.WinForms
{
	internal sealed class GdiContext : IDisposable
	{
		internal static Dictionary<string, Bitmap> m_imageResources = InitializeImageResources();

		private System.Drawing.Graphics m_graphics;

		private GdiWriter m_gdiWriter;

		private FontCache m_fontCache;

		internal string SearchText;

		internal List<SearchMatch> SearchMatches;

		internal int SearchMatchIndex;

		internal int TextRunIndexHitStart = -1;

		internal int TextRunIndexHitEnd = -1;

		internal bool SearchHit;

		internal Dictionary<string, Image> SharedImages = new Dictionary<string, Image>();

		internal RPLReport RplReport;

		internal RenderingReport RenderingReport;

		private bool m_firstDraw = true;

		private bool m_testMode;

		internal static Dictionary<string, Bitmap> ImageResources => m_imageResources;

		internal System.Drawing.Graphics Graphics
		{
			get
			{
				return m_graphics;
			}
			set
			{
				m_graphics = value;
				m_graphics.CompositingMode = CompositingMode.SourceOver;
				m_graphics.PageUnit = GraphicsUnit.Millimeter;
				m_graphics.PixelOffsetMode = PixelOffsetMode.Default;
				m_graphics.SmoothingMode = SmoothingMode.Default;
				m_graphics.TextRenderingHint = TextRenderingHint.SystemDefault;
				GdiWriter.Graphics = m_graphics;
			}
		}

		internal GdiWriter GdiWriter
		{
			get
			{
				if (m_gdiWriter == null)
				{
					m_gdiWriter = new GdiWriter();
				}
				return m_gdiWriter;
			}
		}

		internal FontCache FontCache
		{
			get
			{
				if (m_fontCache == null)
				{
					m_fontCache = new FontCache(Graphics.DpiX);
				}
				return m_fontCache;
			}
		}

		internal bool TestMode
		{
			get
			{
				return m_testMode;
			}
			set
			{
				m_testMode = value;
			}
		}

		internal bool FirstDraw
		{
			get
			{
				return m_firstDraw;
			}
			set
			{
				m_firstDraw = value;
			}
		}

		internal GdiContext()
		{
		}

		private void Dispose(bool disposing)
		{
			if (!disposing)
			{
				return;
			}
			if (m_fontCache != null)
			{
				m_fontCache.Dispose();
				m_fontCache = null;
			}
			if (m_gdiWriter != null)
			{
				m_gdiWriter.Dispose();
				m_gdiWriter = null;
			}
			if (SharedImages == null)
			{
				return;
			}
			foreach (Image value in SharedImages.Values)
			{
				value.Dispose();
			}
			SharedImages = null;
		}

		~GdiContext()
		{
			Dispose(disposing: false);
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		private static Dictionary<string, Bitmap> InitializeImageResources()
		{
			return new Dictionary<string, Bitmap>(10)
			{
				{
					"toggleMinus",
					new Bitmap(new MemoryStream(Convert.FromBase64String("R0lGODlhCwALAIEAAP///wAAAP///wAAACH/C05FVFNDQVBFMi4wAwEBAAAh+QQBAAACACwAAAAACwALAAAILwAFCBxIUGCAgwgRGgTAsCGAAAsdMoQoIIDEiQsTWqRo8SLHiw8jSuSoUWHBkwEBADs=")))
				},
				{
					"togglePlus",
					new Bitmap(new MemoryStream(Convert.FromBase64String("R0lGODlhCwALAIEAAP///wAAAP///wAAACH/C05FVFNDQVBFMi4wAwEBAAAh+QQBAAACACwAAAAACwALAAAIMQAFCBxIUGCAgwgRGgTAsCGAAAsZBmgIUcBEiRQXJpxY8eLDjBYpgvTosONGhQVTBgQAOw==")))
				},
				{
					"unsorted",
					new Bitmap(new MemoryStream(Convert.FromBase64String("R0lGODlhEAAQAIIAACsqMEFASUxQXyovO9HS0x8mJAAAAP///yH/C05FVFNDQVBFMi4wAwEBAAAh+QQBAAAHACwAAAAAEAAQAAAIRAAPCBxIsKDBgwgHEkhYkMCAhQwPEDBgoABEhAQAaAwg4KJBAiBDeoxIsqREkSATEhAQQCOAkQ0LUIT58aFJmiZzEgwIADs=")))
				},
				{
					"sortAsc",
					new Bitmap(new MemoryStream(Convert.FromBase64String("R0lGODlhEAAQAIUAACsqMEFASUxQXyovOx8mJP////v7+/r6+vPz8/Hx8e7u7urq6ujo6OLi4tra2tbW1snJycDAwKenp6WlpaSkpJubm5qamnt7e3d3d2xsbGtra2VlZWFhYV5eXltbW1paWlZWVlVVVVJSUlBQUE9PTwAAAP///wAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACH/C05FVFNDQVBFMi4wAwEBAAAh+QQBAAAmACwAAAAAEAAQAAAIfQBNCBxIsKDBgwhNhMigYUOIhCY6REhQoEGEDgg/PCjAkeODDwZBVFDQkaOCCiAKenDQcQBHBA48FOTAgGOJEgQKHGDAoeCHmgCCBhBQgAFIgiIgGNB54EABAxBEGMSwVCdHAxgOjrCwoOMCCyMQkrggYQKFCyQgql3L9mBAADs=")))
				},
				{
					"sortDesc",
					new Bitmap(new MemoryStream(Convert.FromBase64String("R0lGODlhEAAQAIUAACsqMEFASUxQXyovOx8mJP////v7+/r6+vPz8/Hx8e7u7urq6ujo6OLi4tra2tbW1snJycDAwK6urqurq6qqqqampp6enpubm4iIiIaGhoGBgXt7e29vb25ubm1tbWhoaGRkZGBgYF1dXQAAAP///wAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACH/C05FVFNDQVBFMi4wAwEBAAAh+QQBAAAkACwAAAAAEAAQAAAIfQBJCBxIsKDBgwgTHqxwgcIECRcqIMxgYUGBiwssZDiowcDFAxcNaDAoAoLHAygLGIAgoiAIBgUEBABAswADEAU5MABJYMSIiww4FPzgAMHFARcLOPhQEMMFBUkvKriAwWCIB1ELPAiB8EOEBgUSRGCacIOHDhw2KFzLtmBAADs=")))
				},
				{
					"InvalidImage",
					new Bitmap(new MemoryStream(Convert.FromBase64String("Qk3+AAAAAAAAAHYAAAAoAAAADwAAABEAAAABAAQAAAAAAAAAAADEDgAAxA4AABAAAAAQAAAAAAAA/wAAgP8AgAD/AICA/4AAAP+AAID/gIAA/8DAwP+AgID/AAD//wD/AP8A/////wAA//8A/////wD//////3AAAAAAAAAAeHd3d3d3dwB4///////3AHj///////cAeP//////9wB4///////3AHj/+Z/5n/cAeP//mZn/9wB4///5n//3AHj//5mZ//cAeP/5n/mf9wB4///////3AHj///////cAeP//////9wB4///////3AHiIiIiIiIiAd3d3d3d3d3A=")))
				}
			};
		}

		internal static void CalculateUsableReportItemRectangle(RPLElementProps properties, ref RectangleF position)
		{
			GetReportItemPaddingStyleMM(properties, out float paddingLeft, out float paddingTop, out float paddingRight, out float paddingBottom);
			position.X += paddingLeft;
			position.Y += paddingTop;
			position.Width -= paddingLeft + paddingRight;
			position.Height -= paddingTop + paddingBottom;
		}

		internal static RectangleF GetMeasurementRectangle(RPLMeasurement measurement, RectangleF bounds)
		{
			if (measurement != null)
			{
				return new RectangleF(measurement.Left + bounds.Left, measurement.Top + bounds.Top, measurement.Width, measurement.Height);
			}
			return bounds;
		}

		internal static Color GetStylePropertyValueColor(RPLElementProps properties, byte style)
		{
			string text = (string)SharedRenderer.GetStylePropertyValueObject(properties, style);
			if (string.IsNullOrEmpty(text) || string.Compare(text, "TRANSPARENT", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return Color.Empty;
			}
			return new RPLReportColor(text).ToColor();
		}

		internal static float GetStylePropertyValueSizeMM(RPLElementProps properties, byte style, float value)
		{
			string text = (string)SharedRenderer.GetStylePropertyValueObject(properties, style);
			if (string.IsNullOrEmpty(text))
			{
				return value;
			}
			return (float)new RPLReportSize(text).ToMillimeters();
		}

		internal static float GetStylePropertyValueSizeMM(RPLElementProps properties, byte style)
		{
			string text = (string)SharedRenderer.GetStylePropertyValueObject(properties, style);
			if (string.IsNullOrEmpty(text))
			{
				return float.NaN;
			}
			return (float)new RPLReportSize(text).ToMillimeters();
		}

		internal static float GetStylePropertyValueSizePT(RPLElementProps properties, byte style)
		{
			bool fromInstance = false;
			return GetStylePropertyValueSizePT(properties, style, ref fromInstance);
		}

		internal static float GetStylePropertyValueSizePT(RPLElementProps properties, byte style, ref bool fromInstance)
		{
			string text = (string)SharedRenderer.GetStylePropertyValueObject(properties, style, ref fromInstance);
			if (string.IsNullOrEmpty(text))
			{
				return float.NaN;
			}
			return (float)new RPLReportSize(text).ToPoints();
		}

		internal static string GetStylePropertyValueString(RPLElementProps properties, byte style)
		{
			bool fromInstance = false;
			return GetStylePropertyValueString(properties, style, ref fromInstance);
		}

		internal static string GetStylePropertyValueString(RPLElementProps properties, byte style, ref bool fromInstance)
		{
			object stylePropertyValueObject = SharedRenderer.GetStylePropertyValueObject(properties, style, ref fromInstance);
			if (stylePropertyValueObject == null)
			{
				return null;
			}
			return (string)stylePropertyValueObject;
		}

		internal bool IsOnScreen(RectangleF bounds)
		{
			if (!Graphics.ClipBounds.Contains(bounds))
			{
				return Graphics.ClipBounds.IntersectsWith(bounds);
			}
			return true;
		}

		private static void GetReportItemPaddingStyleMM(RPLElementProps instanceProperties, out float paddingLeft, out float paddingTop, out float paddingRight, out float paddingBottom)
		{
			paddingLeft = GetStylePropertyValueSizeMM(instanceProperties, 15, 0f);
			paddingTop = GetStylePropertyValueSizeMM(instanceProperties, 17, 0f);
			paddingRight = GetStylePropertyValueSizeMM(instanceProperties, 16, 0f);
			paddingBottom = GetStylePropertyValueSizeMM(instanceProperties, 18, 0f);
		}
	}
}
