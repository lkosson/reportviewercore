using Microsoft.ReportingServices.ReportProcessing;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class ChartMember : Group
	{
		public enum SortOrders
		{
			None,
			Ascending,
			Descending
		}

		private ChartHeading m_headingDef;

		private ChartHeadingInstance m_headingInstance;

		private ChartHeadingInstanceInfo m_headingInstanceInfo;

		private ChartMemberCollection m_children;

		private ChartMember m_parent;

		private int m_index;

		private int m_cachedMemberDataPointIndex = -1;

		public override string ID
		{
			get
			{
				if (m_headingDef.Grouping == null && m_headingDef.IDs != null)
				{
					return m_headingDef.IDs[m_index].ToString(CultureInfo.InvariantCulture);
				}
				return m_headingDef.ID.ToString(CultureInfo.InvariantCulture);
			}
		}

		internal override TextBox ToggleParent => null;

		public override SharedHiddenState SharedHidden
		{
			get
			{
				if (IsStatic)
				{
					return SharedHiddenState.Never;
				}
				return Visibility.GetSharedHidden(m_visibilityDef);
			}
		}

		public override bool IsToggleChild => false;

		public override bool Hidden
		{
			get
			{
				if (m_headingInstance == null)
				{
					return RenderingContext.GetDefinitionHidden(m_headingDef.Visibility);
				}
				if (m_headingDef.Visibility == null)
				{
					return false;
				}
				if (m_headingDef.Visibility.Toggle != null)
				{
					return base.OwnerDataRegion.RenderingContext.IsItemHidden(m_headingInstance.UniqueName, potentialSender: false);
				}
				return false;
			}
		}

		public override CustomPropertyCollection CustomProperties
		{
			get
			{
				CustomPropertyCollection customPropertyCollection = m_customProperties;
				if (m_customProperties == null)
				{
					if (m_headingDef.Grouping == null || m_headingDef.Grouping.CustomProperties == null)
					{
						return null;
					}
					customPropertyCollection = ((m_headingInstance != null) ? new CustomPropertyCollection(m_headingDef.Grouping.CustomProperties, InstanceInfo.CustomPropertyInstances) : new CustomPropertyCollection(m_headingDef.Grouping.CustomProperties, null));
					if (base.OwnerDataRegion.RenderingContext.CacheState)
					{
						m_customProperties = customPropertyCollection;
					}
				}
				return customPropertyCollection;
			}
		}

		public override string Label => null;

		public object MemberLabel
		{
			get
			{
				if (IsFakedStatic)
				{
					return null;
				}
				if (m_headingInstance == null)
				{
					if (m_headingDef.Labels != null && m_headingDef.Labels[m_index] != null && ExpressionInfo.Types.Constant == m_headingDef.Labels[m_index].Type)
					{
						return m_headingDef.Labels[m_index].Value;
					}
					return null;
				}
				if (m_headingDef.ChartGroupExpression)
				{
					return InstanceInfo.GroupExpressionValue;
				}
				return InstanceInfo.HeadingLabel;
			}
		}

		public ChartMember Parent => m_parent;

		public bool IsInnerMostMember => m_headingDef.SubHeading == null;

		public ChartMemberCollection Children
		{
			get
			{
				ChartHeading subHeading = m_headingDef.SubHeading;
				if (subHeading == null)
				{
					return null;
				}
				ChartMemberCollection chartMemberCollection = m_children;
				if (m_children == null)
				{
					ChartHeadingInstanceList headingInstances = null;
					if (m_headingInstance != null)
					{
						headingInstances = m_headingInstance.SubHeadingInstances;
					}
					chartMemberCollection = new ChartMemberCollection((Chart)base.OwnerDataRegion, this, subHeading, headingInstances);
					if (base.OwnerDataRegion.RenderingContext.CacheState)
					{
						m_children = chartMemberCollection;
					}
				}
				return chartMemberCollection;
			}
		}

		public int MemberDataPointIndex
		{
			get
			{
				if (m_headingInstance == null)
				{
					if (m_headingDef.Grouping == null)
					{
						return m_index;
					}
					return 0;
				}
				return InstanceInfo.HeadingCellIndex;
			}
		}

		internal int CachedMemberDataPointIndex
		{
			get
			{
				if (m_cachedMemberDataPointIndex < 0)
				{
					m_cachedMemberDataPointIndex = MemberDataPointIndex;
				}
				return m_cachedMemberDataPointIndex;
			}
		}

		public int MemberHeadingSpan
		{
			get
			{
				if (m_headingInstance == null)
				{
					return 1;
				}
				return InstanceInfo.HeadingSpan;
			}
		}

		private bool IsFakedStatic
		{
			get
			{
				if (m_headingDef.Grouping == null && m_headingDef.Labels == null)
				{
					return true;
				}
				return false;
			}
		}

		public bool IsStatic
		{
			get
			{
				if (m_headingDef.Grouping == null)
				{
					return true;
				}
				return false;
			}
		}

		public SortOrders SortOrder
		{
			get
			{
				SortOrders result = SortOrders.None;
				if (!IsStatic)
				{
					BoolList boolList = (m_headingDef.Sorting == null) ? m_headingDef.Grouping.SortDirections : m_headingDef.Sorting.SortDirections;
					if (boolList != null && 0 < boolList.Count)
					{
						result = (boolList[0] ? SortOrders.Ascending : SortOrders.Descending);
					}
				}
				return result;
			}
		}

		public override string DataElementName
		{
			get
			{
				if (IsStatic)
				{
					if (m_headingInstance != null && InstanceInfo.HeadingLabel != null)
					{
						return DataTypeUtility.ConvertToInvariantString(InstanceInfo.HeadingLabel);
					}
					if (!m_headingDef.IsColumn)
					{
						return "Series" + m_index.ToString(CultureInfo.InvariantCulture);
					}
					return "Category" + m_index.ToString(CultureInfo.InvariantCulture);
				}
				return base.DataElementName;
			}
		}

		public override DataElementOutputTypes DataElementOutput
		{
			get
			{
				if (IsStatic)
				{
					return DataElementOutputForStatic(null);
				}
				return base.DataElementOutput;
			}
		}

		internal ExpressionInfo LabelDefinition
		{
			get
			{
				if (!IsFakedStatic && m_headingDef.Labels != null)
				{
					return m_headingDef.Labels[m_index];
				}
				return null;
			}
		}

		internal object LabelValue
		{
			get
			{
				if (IsFakedStatic || m_headingDef.Labels == null)
				{
					return null;
				}
				if (m_headingInstance == null)
				{
					if (m_headingDef.Labels != null && m_headingDef.Labels[m_index] != null && ExpressionInfo.Types.Constant == m_headingDef.Labels[m_index].Type)
					{
						return m_headingDef.Labels[m_index].Value;
					}
					return null;
				}
				return InstanceInfo.HeadingLabel;
			}
		}

		internal ChartHeadingInstanceInfo InstanceInfo
		{
			get
			{
				if (m_headingInstance == null)
				{
					return null;
				}
				if (m_headingInstanceInfo == null)
				{
					m_headingInstanceInfo = m_headingInstance.GetInstanceInfo(base.OwnerDataRegion.RenderingContext.ChunkManager);
				}
				return m_headingInstanceInfo;
			}
		}

		internal ChartMember(Chart owner, ChartMember parent, ChartHeading headingDef, ChartHeadingInstance headingInstance, int index)
			: base(owner, headingDef.Grouping, headingDef.Visibility)
		{
			m_parent = parent;
			m_headingDef = headingDef;
			m_headingInstance = headingInstance;
			m_index = index;
			if (m_headingInstance != null)
			{
				m_uniqueName = m_headingInstance.UniqueName;
			}
		}

		public DataElementOutputTypes DataElementOutputForStatic(ChartMember staticHeading)
		{
			if (!IsStatic)
			{
				return DataElementOutput;
			}
			if (staticHeading != null && (!staticHeading.IsStatic || staticHeading.Parent == Parent))
			{
				staticHeading = null;
			}
			if (staticHeading != null)
			{
				int index;
				int index2;
				if (m_headingDef.IsColumn)
				{
					index = staticHeading.m_index;
					index2 = m_index;
				}
				else
				{
					index = m_index;
					index2 = staticHeading.m_index;
				}
				return GetDataElementOutputTypeFromDataPoint(index, index2);
			}
			Microsoft.ReportingServices.ReportProcessing.Chart chart = (Microsoft.ReportingServices.ReportProcessing.Chart)base.OwnerDataRegion.ReportItemDef;
			if (chart.PivotStaticColumns == null || chart.PivotStaticRows == null)
			{
				return GetDataElementOutputTypeFromDataPoint(0, m_index);
			}
			Global.Tracer.Assert(chart.PivotStaticColumns != null && chart.PivotStaticRows != null);
			return GetDataElementOutputTypeForSeriesCategory(m_index);
		}

		internal bool IsPlotTypeLine()
		{
			if (m_headingInstance == null)
			{
				return false;
			}
			if (0 <= InstanceInfo.StaticGroupingIndex)
			{
				Global.Tracer.Assert(m_headingDef != null);
				if (m_headingDef.PlotTypesLine != null)
				{
					return m_headingDef.PlotTypesLine[InstanceInfo.StaticGroupingIndex];
				}
			}
			return false;
		}

		private DataElementOutputTypes GetDataElementOutputTypeFromDataPoint(int seriesIndex, int categoryIndex)
		{
			return ((Microsoft.ReportingServices.ReportProcessing.Chart)base.OwnerDataRegion.ReportItemDef).GetDataPoint(seriesIndex, categoryIndex).DataElementOutput;
		}

		private DataElementOutputTypes GetDataElementOutputTypeForSeriesCategory(int index)
		{
			Microsoft.ReportingServices.ReportProcessing.Chart chart = (Microsoft.ReportingServices.ReportProcessing.Chart)base.OwnerDataRegion.ReportItemDef;
			int num;
			int num2;
			int num3;
			if (m_headingDef.IsColumn)
			{
				num = 0;
				num2 = index;
				num3 = chart.StaticSeriesCount;
			}
			else
			{
				num = index;
				num2 = 0;
				num3 = chart.StaticCategoryCount;
			}
			while (true)
			{
				if (chart.GetDataPoint(num, num2).DataElementOutput != DataElementOutputTypes.NoOutput)
				{
					return DataElementOutputTypes.Output;
				}
				if (m_headingDef.IsColumn)
				{
					num++;
					if (num >= num3)
					{
						break;
					}
				}
				else
				{
					num2++;
					if (num2 >= num3)
					{
						break;
					}
				}
			}
			return DataElementOutputTypes.NoOutput;
		}
	}
}
