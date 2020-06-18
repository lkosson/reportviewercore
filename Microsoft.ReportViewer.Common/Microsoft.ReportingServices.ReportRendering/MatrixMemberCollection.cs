using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class MatrixMemberCollection
	{
		private Matrix m_owner;

		private MatrixHeading m_headingDef;

		private MatrixHeadingInstanceList m_headingInstances;

		private MatrixMember[] m_members;

		private MatrixMember m_firstMember;

		private MatrixMember m_parent;

		private int m_subTotalPosition = -1;

		private bool m_isParentSubTotal;

		private List<int> m_memberMapping;

		public MatrixMember this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, Count);
				}
				MatrixMember matrixMember = null;
				if (index == 0)
				{
					matrixMember = m_firstMember;
				}
				else if (m_members != null)
				{
					matrixMember = m_members[index - 1];
				}
				if (matrixMember == null)
				{
					bool isSubtotal = false;
					MatrixHeadingInstance matrixHeadingInstance = null;
					if (m_memberMapping != null && index < m_memberMapping.Count)
					{
						matrixHeadingInstance = m_headingInstances[m_memberMapping[index]];
						isSubtotal = matrixHeadingInstance.IsSubtotal;
					}
					else if (m_subTotalPosition >= 0 && index == m_subTotalPosition)
					{
						isSubtotal = true;
					}
					matrixMember = new MatrixMember(m_owner, m_parent, m_headingDef, matrixHeadingInstance, isSubtotal, m_isParentSubTotal, index);
					if (m_owner.RenderingContext.CacheState)
					{
						if (index == 0)
						{
							m_firstMember = matrixMember;
						}
						else
						{
							if (m_members == null)
							{
								m_members = new MatrixMember[Count - 1];
							}
							m_members[index - 1] = matrixMember;
						}
					}
				}
				return matrixMember;
			}
		}

		public int Count
		{
			get
			{
				if (m_owner.NoRows)
				{
					if (m_headingDef.Subtotal == null)
					{
						if (m_headingDef.Grouping == null)
						{
							return m_headingDef.ReportItems.Count;
						}
						return 1;
					}
					return 2;
				}
				return m_memberMapping.Count;
			}
		}

		internal MatrixHeading MatrixHeadingDef => m_headingDef;

		internal MatrixMemberCollection(Matrix owner, MatrixMember parent, MatrixHeading headingDef, MatrixHeadingInstanceList headingInstances, List<int> memberMapping, bool isParentSubTotal)
		{
			m_owner = owner;
			m_parent = parent;
			m_headingInstances = headingInstances;
			m_headingDef = headingDef;
			m_memberMapping = memberMapping;
			m_isParentSubTotal = isParentSubTotal;
			if (!owner.NoRows)
			{
				return;
			}
			Subtotal subtotal = m_headingDef.Subtotal;
			if (subtotal != null)
			{
				if (subtotal.Position == Subtotal.PositionType.After)
				{
					m_subTotalPosition = 1;
				}
				else
				{
					m_subTotalPosition = 0;
				}
			}
		}
	}
}
