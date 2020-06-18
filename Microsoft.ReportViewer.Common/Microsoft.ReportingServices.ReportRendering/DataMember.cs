using Microsoft.ReportingServices.ReportProcessing;
using System.Collections;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class DataMember : Group
	{
		private CustomReportItemHeading m_headingDef;

		private CustomReportItemHeadingInstance m_headingInstance;

		private DataGroupingCollection m_children;

		private DataMember m_parent;

		private bool m_isSubtotal;

		private int m_index;

		public override string ID => m_headingDef.ID.ToString(CultureInfo.InvariantCulture);

		internal override TextBox ToggleParent => null;

		public override bool IsToggleChild => false;

		public override SharedHiddenState SharedHidden => SharedHiddenState.Never;

		public override bool Hidden => false;

		public override CustomPropertyCollection CustomProperties
		{
			get
			{
				CustomPropertyCollection customPropertyCollection = m_customProperties;
				if (m_customProperties == null)
				{
					if (m_headingDef.CustomProperties == null)
					{
						return null;
					}
					customPropertyCollection = ((m_headingInstance != null) ? new CustomPropertyCollection(m_headingDef.CustomProperties, m_headingInstance.CustomPropertyInstances) : new CustomPropertyCollection(m_headingDef.CustomProperties, null));
					if (m_ownerItem.UseCache)
					{
						m_customProperties = customPropertyCollection;
					}
				}
				return customPropertyCollection;
			}
		}

		public ValueCollection GroupValues
		{
			get
			{
				if (m_groupingDef == null || m_groupingDef.GroupExpressions == null || m_groupingDef.GroupExpressions.Count == 0)
				{
					return null;
				}
				int count = m_groupingDef.GroupExpressions.Count;
				ArrayList arrayList = new ArrayList(count);
				for (int i = 0; i < count; i++)
				{
					object obj = null;
					obj = ((m_groupingDef.GroupExpressions[i].Type != ExpressionInfo.Types.Constant) ? ((m_headingInstance != null && m_headingInstance.GroupExpressionValues != null) ? m_headingInstance.GroupExpressionValues[i] : null) : m_groupingDef.GroupExpressions[i].Value);
					arrayList.Add(obj);
				}
				return new ValueCollection(arrayList);
			}
		}

		public override string Label
		{
			get
			{
				string result = null;
				if (m_groupingDef != null && m_groupingDef.GroupLabel != null)
				{
					result = ((m_groupingDef.GroupLabel.Type == ExpressionInfo.Types.Constant) ? m_groupingDef.GroupLabel.Value : ((m_headingInstance != null) ? m_headingInstance.Label : null));
				}
				return result;
			}
		}

		public DataMember Parent => m_parent;

		public DataGroupingCollection Children
		{
			get
			{
				CustomReportItemHeadingList innerHeadings = m_headingDef.InnerHeadings;
				if (innerHeadings == null)
				{
					return null;
				}
				DataGroupingCollection dataGroupingCollection = m_children;
				if (m_children == null)
				{
					CustomReportItemHeadingInstanceList headingInstances = null;
					if (m_headingInstance == null)
					{
						return null;
					}
					if (m_headingInstance != null)
					{
						headingInstances = m_headingInstance.SubHeadingInstances;
					}
					dataGroupingCollection = new DataGroupingCollection((CustomReportItem)m_ownerItem, this, innerHeadings, headingInstances);
					if (m_ownerItem.UseCache)
					{
						m_children = dataGroupingCollection;
					}
				}
				return dataGroupingCollection;
			}
		}

		public bool IsTotal
		{
			get
			{
				Global.Tracer.Assert((m_isSubtotal && !m_headingDef.Subtotal) || !m_isSubtotal);
				return m_isSubtotal;
			}
		}

		public int MemberCellIndex
		{
			get
			{
				if (m_headingInstance == null)
				{
					return -1;
				}
				return m_headingInstance.HeadingCellIndex;
			}
		}

		public int MemberHeadingSpan
		{
			get
			{
				if (m_headingInstance == null)
				{
					return -1;
				}
				return m_headingInstance.HeadingSpan;
			}
		}

		public override string DataElementName
		{
			get
			{
				if (m_headingDef.Grouping == null)
				{
					if (m_headingInstance != null && m_headingInstance.Label != null)
					{
						return m_headingInstance.Label;
					}
					if (!m_headingDef.IsColumn)
					{
						return "Row" + m_index.ToString(CultureInfo.InvariantCulture);
					}
					return "Column" + m_index.ToString(CultureInfo.InvariantCulture);
				}
				return base.DataElementName;
			}
		}

		public override DataElementOutputTypes DataElementOutput
		{
			get
			{
				if (m_headingDef.Grouping == null)
				{
					return DataElementOutputTypes.Output;
				}
				return base.DataElementOutput;
			}
		}

		public bool IsStatic => m_headingDef.Static;

		internal DataMember(CustomReportItem owner, DataMember parent, CustomReportItemHeading headingDef, CustomReportItemHeadingInstance headingInstance, bool isSubtotal, int index)
			: base(owner, headingDef.Grouping)
		{
			Global.Tracer.Assert(headingDef != null);
			m_parent = parent;
			m_headingDef = headingDef;
			m_headingInstance = headingInstance;
			m_index = index;
			m_isSubtotal = isSubtotal;
			m_uniqueName = -1;
		}
	}
}
