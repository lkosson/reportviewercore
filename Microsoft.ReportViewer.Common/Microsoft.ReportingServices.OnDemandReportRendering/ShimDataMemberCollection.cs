using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportRendering;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimDataMemberCollection : DataMemberCollection
	{
		private bool m_isStatic;

		private bool m_isColumnMember;

		private int m_definitionStartIndex = -1;

		private int m_definitionEndIndex = -1;

		private DataGroupingCollection m_definitionGroups;

		public override DataMember this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, Count);
				}
				return (DataMember)m_children[index];
			}
		}

		public override int Count => m_children.Length;

		internal ShimDataMemberCollection(IDefinitionPath parentDefinitionPath, CustomReportItem owner, bool isColumnMember, ShimDataMember parent, DataGroupingCollection definitionGroups)
			: base(parentDefinitionPath, owner)
		{
			m_isColumnMember = isColumnMember;
			m_definitionGroups = definitionGroups;
			m_definitionStartIndex = owner.GetCurrentMemberCellDefinitionIndex();
			if (definitionGroups[0] != null && definitionGroups[0][0] != null)
			{
				m_isStatic = definitionGroups[0][0].IsStatic;
			}
			int count = definitionGroups.Count;
			m_children = new ShimDataMember[count];
			for (int i = 0; i < count; i++)
			{
				m_children[i] = new ShimDataMember(this, owner, parent, i, m_isColumnMember, m_isStatic, definitionGroups[i], i);
			}
			m_definitionEndIndex = owner.GetCurrentMemberCellDefinitionIndex();
		}

		internal void UpdateContext()
		{
			if (m_children != null)
			{
				if (m_isColumnMember)
				{
					ResetContext(base.OwnerCri.RenderCri.CustomData.DataColumnGroupings);
				}
				else
				{
					ResetContext(base.OwnerCri.RenderCri.CustomData.DataRowGroupings);
				}
			}
		}

		internal void ResetContext(DataGroupingCollection definitionGroups)
		{
			if (m_children == null)
			{
				return;
			}
			if (definitionGroups != null)
			{
				m_definitionGroups = definitionGroups;
			}
			if (m_isStatic)
			{
				for (int i = 0; i < m_children.Length; i++)
				{
					((ShimDataMember)m_children[i]).ResetContext(m_definitionGroups[i]);
				}
			}
		}
	}
}
