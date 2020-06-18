using System;

namespace Microsoft.ReportingServices.Rendering.RichText
{
	internal class TexRunShapeData
	{
		private GlyphShapeData m_glyphData;

		private SCRIPT_ANALYSIS m_analysis;

		private SCRIPT_LOGATTR[] m_scriptLogAttr;

		private CachedFont m_cachedFont;

		private int? m_itemizedScriptId;

		private TextRunState m_runState;

		internal GlyphShapeData GlyphData => m_glyphData;

		internal SCRIPT_ANALYSIS Analysis => m_analysis;

		internal SCRIPT_LOGATTR[] ScriptLogAttr => m_scriptLogAttr;

		internal CachedFont Font => m_cachedFont;

		internal int? ItemizedScriptId => m_itemizedScriptId;

		internal TextRunState State => m_runState;

		internal TexRunShapeData(TextRun run, bool storeGlyph)
		{
			if (storeGlyph && run.GlyphData != null)
			{
				m_glyphData = run.GlyphData.GlyphScriptShapeData;
			}
			m_analysis = run.SCRIPT_ANALYSIS;
			m_scriptLogAttr = run.ScriptLogAttr;
			m_cachedFont = run.CachedFont;
			m_runState = run.State;
			m_itemizedScriptId = run.ItemizedScriptId;
		}

		internal TexRunShapeData(TextRun run, bool storeGlyph, int startIndex)
			: this(run, storeGlyph)
		{
			SCRIPT_LOGATTR[] scriptLogAttr = run.ScriptLogAttr;
			if (startIndex > 0 && scriptLogAttr != null)
			{
				int num = scriptLogAttr.Length - startIndex;
				SCRIPT_LOGATTR[] array = new SCRIPT_LOGATTR[num];
				Array.Copy(scriptLogAttr, startIndex, array, 0, num);
				m_scriptLogAttr = array;
			}
		}
	}
}
