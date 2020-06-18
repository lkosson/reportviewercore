using System.Collections.Generic;

namespace Microsoft.ReportingServices.Rendering.ExcelRenderer.Excel.BIFF8
{
	internal class ExternSheetInfo
	{
		internal class XTI
		{
			private ushort m_firstTab;

			private ushort m_lastTab;

			private ushort m_supBookIndex;

			internal const int LENGTH = 6;

			internal ushort SupBookIndex => m_supBookIndex;

			internal ushort FirstTab => m_firstTab;

			internal ushort LastTab => m_lastTab;

			internal XTI(ushort supBookIndex, ushort firstTab, ushort lastTab)
			{
				m_firstTab = firstTab;
				m_lastTab = lastTab;
				m_supBookIndex = supBookIndex;
			}
		}

		private List<XTI> m_xtiStructures;

		internal List<XTI> XTIStructures => m_xtiStructures;

		internal ExternSheetInfo()
		{
			m_xtiStructures = new List<XTI>();
		}

		internal int AddXTI(ushort supBookIndex, ushort firstTab, ushort lastTab)
		{
			m_xtiStructures.Add(new XTI(supBookIndex, firstTab, lastTab));
			return m_xtiStructures.Count - 1;
		}
	}
}
