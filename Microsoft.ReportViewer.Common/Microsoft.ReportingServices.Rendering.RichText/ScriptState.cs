namespace Microsoft.ReportingServices.Rendering.RichText
{
	internal sealed class ScriptState
	{
		private ushort m_value;

		internal int uBidiLevel;

		internal int fOverrideDirection;

		internal int fInhibitSymSwap;

		internal int fCharShape;

		internal int fDigitSubstitute;

		internal int fInhibitLigate;

		internal int fDisplayZWG;

		internal int fArabicNumContext;

		internal int fGcpClusters;

		internal int fReserved;

		internal int fEngineReserved;

		internal ScriptState()
		{
		}

		internal ScriptState(ushort value)
		{
			m_value = value;
			uBidiLevel = (m_value & 0x1F);
			fOverrideDirection = ((m_value >> 5) & 1);
			fInhibitSymSwap = ((m_value >> 6) & 1);
			fCharShape = ((m_value >> 7) & 1);
			fDigitSubstitute = ((m_value >> 8) & 1);
			fInhibitLigate = ((m_value >> 9) & 1);
			fDisplayZWG = ((m_value >> 10) & 1);
			fArabicNumContext = ((m_value >> 11) & 1);
			fGcpClusters = ((m_value >> 12) & 1);
			fReserved = ((m_value >> 13) & 1);
			fEngineReserved = ((m_value >> 14) & 3);
		}

		internal static int GetBidiLevel(ushort value)
		{
			return value & 0x1F;
		}

		internal SCRIPT_STATE GetAs_SCRIPT_STATE()
		{
			SCRIPT_STATE result = default(SCRIPT_STATE);
			result.word1 = (ushort)((uBidiLevel & 0x1F) | ((fOverrideDirection & 1) << 5) | ((fInhibitSymSwap & 1) << 6) | ((fCharShape & 1) << 7) | ((fDigitSubstitute & 1) << 8) | ((fInhibitLigate & 1) << 9) | ((fDisplayZWG & 1) << 10) | ((fArabicNumContext & 1) << 11) | ((fGcpClusters & 1) << 12) | ((fReserved & 1) << 13) | ((fEngineReserved & 3) << 14));
			return result;
		}
	}
}
