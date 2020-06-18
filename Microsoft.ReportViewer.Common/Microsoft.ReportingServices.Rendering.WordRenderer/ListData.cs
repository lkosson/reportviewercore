using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer
{
	internal class ListData
	{
		private int m_lsid;

		internal ListLevelOnFile[] m_levels;

		private const short NilStyle = 4095;

		internal virtual int Lsid => m_lsid;

		internal virtual ListLevelOnFile[] Levels => m_levels;

		internal ListData(int listID)
		{
			m_lsid = listID;
			m_levels = new ListLevelOnFile[9];
		}

		internal virtual void SetLevel(int index, ListLevelOnFile level)
		{
			m_levels[index] = level;
		}

		internal void Write(BinaryWriter dataWriter, BinaryWriter levelWriter, Word97Writer writer)
		{
			dataWriter.Write(m_lsid);
			dataWriter.Write(m_lsid);
			for (int i = 0; i < 9; i++)
			{
				dataWriter.Write((short)4095);
			}
			dataWriter.Write((short)0);
			for (int j = 0; j < m_levels.Length; j++)
			{
				if (m_levels[j] == null)
				{
					m_levels[j] = new ListLevelOnFile(j, RPLFormat.ListStyles.Numbered, writer);
				}
				m_levels[j].Write(levelWriter);
			}
		}
	}
}
