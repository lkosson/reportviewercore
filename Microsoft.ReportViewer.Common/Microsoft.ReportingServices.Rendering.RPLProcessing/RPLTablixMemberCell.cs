namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLTablixMemberCell : RPLTablixCell
	{
		private string m_uniqueName;

		private string m_groupLabel;

		private int m_recursiveToggleLevel = -1;

		private byte m_state;

		private int m_rowSpan = 1;

		private RPLTablixMemberDef m_memberDef;

		public string GroupLabel
		{
			get
			{
				return m_groupLabel;
			}
			set
			{
				m_groupLabel = value;
			}
		}

		public int RecursiveToggleLevel
		{
			get
			{
				return m_recursiveToggleLevel;
			}
			set
			{
				m_recursiveToggleLevel = value;
			}
		}

		public bool IsRecursiveToggle => m_recursiveToggleLevel >= 0;

		public string UniqueName
		{
			get
			{
				return m_uniqueName;
			}
			set
			{
				m_uniqueName = value;
			}
		}

		public bool HasToggle => (m_state & 1) > 0;

		public bool ToggleCollapse => (m_state & 2) > 0;

		public bool IsInnerMost => (m_state & 4) > 0;

		public override int RowSpan
		{
			get
			{
				return m_rowSpan;
			}
			set
			{
				m_rowSpan = value;
			}
		}

		public RPLTablixMemberDef TablixMemberDef
		{
			get
			{
				return m_memberDef;
			}
			set
			{
				m_memberDef = value;
			}
		}

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

		internal RPLTablixMemberCell()
		{
		}

		internal RPLTablixMemberCell(RPLItem element, byte elementState, int rowSpan, int colSpan)
			: base(element, elementState)
		{
			m_rowSpan = rowSpan;
			m_colSpan = colSpan;
		}
	}
}
