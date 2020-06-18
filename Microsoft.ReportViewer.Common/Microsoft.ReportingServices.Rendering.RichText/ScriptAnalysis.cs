namespace Microsoft.ReportingServices.Rendering.RichText
{
	internal sealed class ScriptAnalysis
	{
		internal int eScript;

		internal int fRTL;

		internal int fLayoutRTL;

		internal int fLinkBefore;

		internal int fLinkAfter;

		internal int fLogicalOrder;

		internal int fNoGlyphIndex;

		internal ScriptState s;

		internal ScriptAnalysis(ushort word1)
		{
			eScript = (word1 & 0x3FF);
			fRTL = ((word1 >> 10) & 1);
			fLayoutRTL = ((word1 >> 11) & 1);
			fLinkBefore = ((word1 >> 12) & 1);
			fLinkAfter = ((word1 >> 13) & 1);
			fLogicalOrder = ((word1 >> 14) & 1);
			fNoGlyphIndex = ((word1 >> 15) & 1);
		}

		internal SCRIPT_ANALYSIS GetAs_SCRIPT_ANALYSIS()
		{
			SCRIPT_ANALYSIS result = default(SCRIPT_ANALYSIS);
			result.word1 = (ushort)((eScript & 0x3FF) | ((fRTL & 1) << 10) | ((fLayoutRTL & 1) << 11) | ((fLinkBefore & 1) << 12) | ((fLinkAfter & 1) << 13) | ((fLogicalOrder & 1) << 14) | ((fNoGlyphIndex & 1) << 15));
			result.state = s.GetAs_SCRIPT_STATE();
			return result;
		}
	}
}
