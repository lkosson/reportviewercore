using System;

namespace Microsoft.ReportingServices.Rendering.RichText
{
	internal sealed class GlyphData
	{
		internal GlyphShapeData GlyphScriptShapeData;

		private int[] m_advances;

		private GOFFSET[] m_gOffsets;

		internal ABC ABC;

		private float m_scaleFactor = 1f;

		private bool m_needGlyphPlaceData;

		internal bool NeedGlyphPlaceData
		{
			get
			{
				return m_needGlyphPlaceData;
			}
			set
			{
				m_needGlyphPlaceData = value;
			}
		}

		internal float ScaleFactor
		{
			set
			{
				m_scaleFactor = value;
			}
		}

		public int[] RawAdvances => m_advances;

		internal int[] Advances
		{
			get
			{
				if (m_scaleFactor != 1f)
				{
					int[] array = new int[m_advances.Length];
					for (int i = 0; i < m_advances.Length; i++)
					{
						array[i] = Scale(m_advances[i]);
					}
					return array;
				}
				return m_advances;
			}
		}

		internal int[] ScaledAdvances => m_advances;

		internal GOFFSET[] RawGOffsets => m_gOffsets;

		internal GOFFSET[] GOffsets
		{
			get
			{
				if (m_scaleFactor != 1f)
				{
					GOFFSET[] array = new GOFFSET[m_gOffsets.Length];
					for (int i = 0; i < m_gOffsets.Length; i++)
					{
						array[i] = default(GOFFSET);
						array[i].du = Scale(m_gOffsets[i].du);
						array[i].dv = Scale(m_gOffsets[i].dv);
					}
					return array;
				}
				return m_gOffsets;
			}
		}

		internal GOFFSET[] ScaledGOffsets => m_gOffsets;

		internal int ScaledTotalWidth => ABC.abcA + (int)ABC.abcB + ABC.abcC;

		internal int ScaledTotalWidthAtLineEnd => ABC.abcA + (int)ABC.abcB + Math.Abs(ABC.abcC);

		public bool Scaled => m_scaleFactor != 1f;

		internal GlyphData(int maxglyphs, int numChars)
		{
			GlyphScriptShapeData = new GlyphShapeData(maxglyphs, numChars);
		}

		internal GlyphData(GlyphShapeData glyphInfo)
		{
			m_needGlyphPlaceData = true;
			GlyphScriptShapeData = glyphInfo;
			m_advances = new int[glyphInfo.GlyphCount];
			m_gOffsets = new GOFFSET[glyphInfo.GlyphCount];
			ABC = default(ABC);
		}

		internal int GetTotalWidth(bool isAtLineEnd)
		{
			int num = isAtLineEnd ? ScaledTotalWidthAtLineEnd : ScaledTotalWidth;
			if (m_scaleFactor != 1f)
			{
				return Scale(num);
			}
			return num;
		}

		internal int Scale(int value)
		{
			return (int)((float)value / m_scaleFactor + 0.5f);
		}

		internal void TrimToGlyphCount()
		{
			GlyphScriptShapeData.TrimToGlyphCount();
			m_advances = new int[GlyphScriptShapeData.GlyphCount];
			m_gOffsets = new GOFFSET[GlyphScriptShapeData.GlyphCount];
			ABC = default(ABC);
		}
	}
}
