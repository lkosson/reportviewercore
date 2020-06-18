namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLTablixMemberDef
	{
		private string m_definitionPath;

		private int m_memberCellIndex;

		private int m_level;

		private byte m_state;

		public string DefinitionPath
		{
			get
			{
				return m_definitionPath;
			}
			set
			{
				m_definitionPath = value;
			}
		}

		public int MemberCellIndex
		{
			get
			{
				return m_memberCellIndex;
			}
			set
			{
				m_memberCellIndex = value;
			}
		}

		public int Level
		{
			get
			{
				return m_level;
			}
			set
			{
				m_level = value;
			}
		}

		public bool StaticHeadersTree => (m_state & 4) > 0;

		public bool IsStatic => (m_state & 2) > 0;

		public bool IsColumn => (m_state & 1) > 0;

		internal byte State
		{
			get
			{
				return m_state;
			}
			set
			{
				m_state = value;
			}
		}

		internal RPLTablixMemberDef()
		{
		}

		internal RPLTablixMemberDef(string definitionPath, int memberCellIndex, byte state, int defTreeLevel)
		{
			m_definitionPath = definitionPath;
			m_memberCellIndex = memberCellIndex;
			m_state = state;
			m_level = defTreeLevel;
		}
	}
}
