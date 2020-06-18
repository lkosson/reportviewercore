using Microsoft.ReportingServices.Rendering.RichText;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Microsoft.ReportingServices.Rendering.ImageRenderer
{
	internal sealed class PDFFont
	{
		internal sealed class GlyphData : IComparable
		{
			internal ushort Glyph;

			internal float Width;

			internal char? Character;

			internal GlyphData(ushort glyph, float width)
			{
				Glyph = glyph;
				Width = width;
				Character = null;
			}

			int IComparable.CompareTo(object o1)
			{
				GlyphData glyphData = (GlyphData)o1;
				if (Glyph < glyphData.Glyph)
				{
					return -1;
				}
				if (Glyph > glyphData.Glyph)
				{
					return 1;
				}
				return 0;
			}
		}

		internal CachedFont CachedFont;

		internal readonly string FontFamily;

		internal string FontPDFFamily;

		internal int FontId = -1;

		internal List<GlyphData> UniqueGlyphs = new List<GlyphData>();

		internal string FontCMap;

		internal string Registry;

		internal string Ordering;

		internal string Supplement;

		internal readonly FontStyle GDIFontStyle;

		internal readonly int EMHeight;

		internal readonly float GridHeight;

		internal readonly float EMGridConversion;

		internal readonly bool InternalFont;

		internal readonly bool SimulateItalic;

		internal readonly bool SimulateBold;

		internal EmbeddedFont EmbeddedFont;

		internal bool IsComposite => !string.IsNullOrEmpty(FontCMap);

		internal PDFFont(CachedFont cachedFont, string fontFamily, string pdfFontFamily, string fontCMap, string registry, string ordering, string supplement, FontStyle gdiFontStyle, int emHeight, float gridHeight, bool internalFont, bool simulateItalic, bool simulateBold)
		{
			CachedFont = cachedFont;
			FontFamily = fontFamily;
			FontPDFFamily = pdfFontFamily;
			FontCMap = fontCMap;
			Registry = registry;
			Ordering = ordering;
			Supplement = supplement;
			GDIFontStyle = gdiFontStyle;
			EMHeight = emHeight;
			GridHeight = gridHeight;
			InternalFont = internalFont;
			EMGridConversion = 1000f / (float)emHeight;
			SimulateItalic = simulateItalic;
			SimulateBold = simulateBold;
		}

		internal GlyphData AddUniqueGlyph(ushort glyph, float width)
		{
			GlyphData glyphData = new GlyphData(glyph, width);
			if (UniqueGlyphs.BinarySearch(glyphData) >= 0)
			{
				return null;
			}
			int i;
			for (i = 0; i < UniqueGlyphs.Count; i++)
			{
				if (glyphData.Glyph < UniqueGlyphs[i].Glyph)
				{
					UniqueGlyphs.Insert(i, glyphData);
					break;
				}
			}
			if (i == UniqueGlyphs.Count)
			{
				UniqueGlyphs.Add(glyphData);
			}
			return glyphData;
		}
	}
}
