using System.Collections.Generic;

namespace Microsoft.ReportingServices.Rendering.ImageRenderer
{
	internal sealed class EmbeddedFont
	{
		private int m_objectId;

		private int m_toUnicodeId;

		private List<PDFFont> m_fonts = new List<PDFFont>();

		internal int ObjectId => m_objectId;

		internal int ToUnicodeId => m_toUnicodeId;

		internal List<PDFFont> PDFFonts => m_fonts;

		internal EmbeddedFont(int objectId, int toUnicodeId)
		{
			m_objectId = objectId;
			m_toUnicodeId = toUnicodeId;
		}

		internal ushort[] GetGlyphIdArray()
		{
			int num = 0;
			foreach (PDFFont font in m_fonts)
			{
				num += font.UniqueGlyphs.Count;
			}
			ushort[] array = new ushort[num];
			int num2 = 0;
			foreach (PDFFont font2 in m_fonts)
			{
				foreach (PDFFont.GlyphData uniqueGlyph in font2.UniqueGlyphs)
				{
					array[num2] = uniqueGlyph.Glyph;
					num2++;
				}
			}
			return array;
		}

		internal IEnumerable<CMapMapping> GetGlyphIdToUnicodeMapping()
		{
			int num = 0;
			foreach (PDFFont font in m_fonts)
			{
				num += font.UniqueGlyphs.Count;
			}
			List<CMapMapping> list = new List<CMapMapping>(num);
			foreach (PDFFont font2 in m_fonts)
			{
				foreach (PDFFont.GlyphData uniqueGlyph in font2.UniqueGlyphs)
				{
					if (uniqueGlyph.Character.HasValue)
					{
						list.Add(new CMapMapping(uniqueGlyph.Glyph, uniqueGlyph.Character.Value));
					}
				}
			}
			return list;
		}
	}
}
