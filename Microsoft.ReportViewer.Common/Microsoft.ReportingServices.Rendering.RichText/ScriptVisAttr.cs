namespace Microsoft.ReportingServices.Rendering.RichText
{
	internal sealed class ScriptVisAttr
	{
		private ushort m_value;

		internal int uJustification;

		internal int fClusterStart;

		internal int fDiacritic;

		internal int fZeroWidth;

		internal ScriptVisAttr(ushort value)
		{
			m_value = value;
			uJustification = (m_value & 0xF);
			fClusterStart = ((m_value >> 4) & 1);
			fDiacritic = ((m_value >> 5) & 1);
			fZeroWidth = ((m_value >> 6) & 1);
		}
	}
}
