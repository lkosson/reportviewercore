using System;

namespace Microsoft.ReportingServices.Rendering.RichText
{
	internal sealed class GlyphShapeData
	{
		internal int GlyphCount;

		internal short[] Glyphs;

		internal short[] Clusters;

		internal SCRIPT_VISATTR[] VisAttrs;

		internal GlyphShapeData(int maxglyphs, int numChars)
		{
			Glyphs = new short[maxglyphs];
			Clusters = new short[numChars];
			VisAttrs = new SCRIPT_VISATTR[maxglyphs];
		}

		internal void TrimToGlyphCount()
		{
			if (GlyphCount < Glyphs.Length)
			{
				Array.Resize(ref Glyphs, GlyphCount);
				Array.Resize(ref VisAttrs, GlyphCount);
			}
		}
	}
}
