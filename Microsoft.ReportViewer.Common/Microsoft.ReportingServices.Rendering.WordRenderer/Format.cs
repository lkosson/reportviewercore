namespace Microsoft.ReportingServices.Rendering.WordRenderer
{
	internal abstract class Format
	{
		protected SprmBuffer m_grpprl;

		internal Format(int initialSize, int initialOffset)
		{
			m_grpprl = new SprmBuffer(initialSize, initialOffset);
		}

		internal void AddSprm(ushort sprmCode, int param, byte[] varParam)
		{
			m_grpprl.AddSprm(sprmCode, param, varParam);
		}
	}
}
