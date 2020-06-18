namespace Microsoft.ReportingServices.Rendering.RichText
{
	internal struct SCRIPT_LOGATTR
	{
		private byte m_value;

		internal bool IsWhiteSpace => ((m_value >> 1) & 1) > 0;

		internal bool IsSoftBreak => (m_value & 1 & 1) > 0;
	}
}
