using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class TablixBody
	{
		private Tablix m_owner;

		private TablixRowCollection m_rowCollection;

		private TablixColumnCollection m_columnCollection;

		private bool? m_ignoreCellPageBreaks;

		internal bool HasRowCollection => m_rowCollection != null;

		public TablixRowCollection RowCollection
		{
			get
			{
				if (m_rowCollection == null)
				{
					if (m_owner.IsOldSnapshot)
					{
						switch (m_owner.SnapshotTablixType)
						{
						case DataRegion.Type.List:
							m_rowCollection = new ShimListRowCollection(m_owner);
							break;
						case DataRegion.Type.Table:
							m_rowCollection = new ShimTableRowCollection(m_owner);
							break;
						case DataRegion.Type.Matrix:
							m_rowCollection = new ShimMatrixRowCollection(m_owner);
							break;
						}
					}
					else
					{
						m_rowCollection = new InternalTablixRowCollection(m_owner, m_owner.TablixDef.TablixRows);
					}
				}
				return m_rowCollection;
			}
		}

		public TablixColumnCollection ColumnCollection
		{
			get
			{
				if (m_columnCollection == null)
				{
					m_columnCollection = new TablixColumnCollection(m_owner);
				}
				return m_columnCollection;
			}
		}

		public bool IgnoreCellPageBreaks
		{
			get
			{
				if (!m_ignoreCellPageBreaks.HasValue)
				{
					if (m_owner.IsOldSnapshot)
					{
						m_ignoreCellPageBreaks = (DataRegion.Type.List != m_owner.SnapshotTablixType);
					}
					else
					{
						m_ignoreCellPageBreaks = true;
						Microsoft.ReportingServices.ReportIntermediateFormat.Tablix tablixDef = m_owner.TablixDef;
						if (tablixDef.ColumnCount == 1 && tablixDef.ColumnMembers[0].IsStatic && ((Microsoft.ReportingServices.ReportIntermediateFormat.TablixMember)tablixDef.ColumnMembers[0]).TablixHeader == null)
						{
							TablixMemberList members = (TablixMemberList)tablixDef.RowMembers;
							m_ignoreCellPageBreaks = HasHeader(members);
						}
					}
				}
				return m_ignoreCellPageBreaks.Value;
			}
		}

		internal TablixBody(Tablix owner)
		{
			m_owner = owner;
		}

		private bool HasHeader(TablixMemberList members)
		{
			if (members != null)
			{
				for (int i = 0; i < members.Count; i++)
				{
					if (members[i].TablixHeader != null || HasHeader(members[i].SubMembers))
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
