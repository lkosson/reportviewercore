using Microsoft.ReportingServices.Rendering.RichText;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text;

namespace Microsoft.ReportingServices.Rendering.ImageRenderer
{
	internal sealed class GDIFont : IDisposable
	{
		private Font m_font;

		private float m_height;

		private string m_family;

		private float m_size;

		private string m_key;

		internal Font Font => m_font;

		internal float Height => m_height;

		internal string Family
		{
			get
			{
				if (m_family == null)
				{
					m_family = m_font.FontFamily.GetName(1033);
				}
				return m_family;
			}
		}

		internal float Size
		{
			get
			{
				if (m_size == 0f)
				{
					m_size = m_font.Size;
				}
				return m_size;
			}
		}

		internal string Key => m_key;

		internal GDIFont(string key, Font font, float height)
		{
			m_key = key;
			m_font = font;
			m_height = height;
		}

		private void Dispose(bool disposing)
		{
			if (disposing && m_font != null)
			{
				m_font.Dispose();
				m_font = null;
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		~GDIFont()
		{
			Dispose(disposing: false);
		}

		internal static GDIFont GetOrCreateFont(Dictionary<string, GDIFont> gdiFonts, string fontFamily, float fontSize, RPLFormat.FontWeights fontWeight, RPLFormat.FontStyles fontStyle, RPLFormat.TextDecorations textDecoration)
		{
			string key = GetKey(fontFamily, fontSize, fontWeight, fontStyle, textDecoration);
			if (gdiFonts.TryGetValue(key, out GDIFont value))
			{
				return value;
			}
			bool bold = SharedRenderer.IsWeightBold(fontWeight);
			bool italic = fontStyle == RPLFormat.FontStyles.Italic;
			bool underLine = false;
			bool lineThrough = false;
			switch (textDecoration)
			{
			case RPLFormat.TextDecorations.Underline:
				underLine = true;
				break;
			case RPLFormat.TextDecorations.LineThrough:
				lineThrough = true;
				break;
			}
			Font font = null;
			try
			{
				font = FontCache.CreateGdiPlusFont(fontFamily, fontSize, ref bold, ref italic, lineThrough, underLine);
				value = new GDIFont(key, font, fontSize);
				gdiFonts.Add(key, value);
				return value;
			}
			catch
			{
				if (font != null && !gdiFonts.ContainsKey(key))
				{
					font.Dispose();
					font = null;
				}
				throw;
			}
		}

		private static string GetKey(string fontFamily, float fontSize, RPLFormat.FontWeights fontWeight, RPLFormat.FontStyles fontStyle, RPLFormat.TextDecorations textDecoration)
		{
			StringBuilder stringBuilder = new StringBuilder("FO");
			stringBuilder.Append(fontFamily);
			stringBuilder.Append(fontSize.ToString(CultureInfo.InvariantCulture));
			if (SharedRenderer.IsWeightBold(fontWeight))
			{
				stringBuilder.Append('b');
			}
			else
			{
				stringBuilder.Append('n');
			}
			if (fontStyle == RPLFormat.FontStyles.Italic)
			{
				stringBuilder.Append('i');
			}
			else
			{
				stringBuilder.Append('n');
			}
			switch (textDecoration)
			{
			case RPLFormat.TextDecorations.Underline:
				stringBuilder.Append('u');
				break;
			case RPLFormat.TextDecorations.LineThrough:
				stringBuilder.Append('s');
				break;
			default:
				stringBuilder.Append('n');
				break;
			}
			return stringBuilder.ToString();
		}
	}
}
