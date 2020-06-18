using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer
{
	internal class SectionFormat : Format
	{
		private const int OneSectionSize = 38;

		private List<int> m_cpOffsets = new List<int>();

		private bool m_useTitlePage;

		internal bool UseTitlePage
		{
			get
			{
				return m_useTitlePage;
			}
			set
			{
				m_useTitlePage = value;
			}
		}

		internal int SectionCount => m_cpOffsets.Count;

		internal SectionFormat()
			: base(38, 0)
		{
		}

		internal void EndSection(int cpOffset)
		{
			m_cpOffsets.Add(cpOffset);
		}

		internal void WriteTo(BinaryWriter tableWriter, BinaryWriter mainWriter, int lastCp)
		{
			tableWriter.Write(0);
			int count = m_cpOffsets.Count;
			for (int i = 0; i < count; i++)
			{
				tableWriter.Write(m_cpOffsets[i]);
			}
			tableWriter.Write(lastCp);
			SprmBuffer sprmBuffer = m_grpprl;
			if (UseTitlePage)
			{
				SprmBuffer obj = (SprmBuffer)m_grpprl.Clone();
				obj.AddSprm(12298, 1, null);
				sprmBuffer = obj;
			}
			for (int j = 0; j < count + 1; j++)
			{
				tableWriter.Write(new byte[2]);
				tableWriter.Write((int)mainWriter.BaseStream.Position);
				tableWriter.Write(new byte[6]);
				if (j == 1)
				{
					sprmBuffer = m_grpprl;
					AddSprm(12297, 0, null);
				}
				mainWriter.Write((short)sprmBuffer.Offset);
				mainWriter.Write(sprmBuffer.Buf);
			}
		}
	}
}
