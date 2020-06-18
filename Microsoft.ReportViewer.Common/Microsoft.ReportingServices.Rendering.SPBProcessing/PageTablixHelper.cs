using System.Collections.Generic;

namespace Microsoft.ReportingServices.Rendering.SPBProcessing
{
	internal class PageTablixHelper : PageItemHelper
	{
		private int m_levelForRepeat;

		private List<int> m_tablixCreateState;

		private List<int> m_membersInstanceIndex;

		private bool m_ignoreTotalsOnLastLevel;

		internal int LevelForRepeat
		{
			get
			{
				return m_levelForRepeat;
			}
			set
			{
				m_levelForRepeat = value;
			}
		}

		internal bool IgnoreTotalsOnLastLevel
		{
			get
			{
				return m_ignoreTotalsOnLastLevel;
			}
			set
			{
				m_ignoreTotalsOnLastLevel = value;
			}
		}

		internal List<int> TablixCreateState
		{
			get
			{
				return m_tablixCreateState;
			}
			set
			{
				m_tablixCreateState = value;
			}
		}

		internal List<int> MembersInstanceIndex
		{
			get
			{
				return m_membersInstanceIndex;
			}
			set
			{
				m_membersInstanceIndex = value;
			}
		}

		internal PageTablixHelper(byte type)
			: base(type)
		{
		}
	}
}
