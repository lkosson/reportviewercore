using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal class ChartHeadingInstanceInfo : InstanceInfo
	{
		private object m_headingLabel;

		private int m_headingCellIndex;

		private int m_headingSpan = 1;

		private object m_groupExpressionValue;

		private int m_staticGroupingIndex = -1;

		private DataValueInstanceList m_customPropertyInstances;

		internal object HeadingLabel
		{
			get
			{
				return m_headingLabel;
			}
			set
			{
				m_headingLabel = value;
			}
		}

		internal int HeadingCellIndex
		{
			get
			{
				return m_headingCellIndex;
			}
			set
			{
				m_headingCellIndex = value;
			}
		}

		internal int HeadingSpan
		{
			get
			{
				return m_headingSpan;
			}
			set
			{
				m_headingSpan = value;
			}
		}

		internal object GroupExpressionValue
		{
			get
			{
				return m_groupExpressionValue;
			}
			set
			{
				m_groupExpressionValue = value;
			}
		}

		internal int StaticGroupingIndex
		{
			get
			{
				return m_staticGroupingIndex;
			}
			set
			{
				m_staticGroupingIndex = value;
			}
		}

		internal DataValueInstanceList CustomPropertyInstances
		{
			get
			{
				return m_customPropertyInstances;
			}
			set
			{
				m_customPropertyInstances = value;
			}
		}

		internal ChartHeadingInstanceInfo(ReportProcessing.ProcessingContext pc, int headingCellIndex, ChartHeading chartHeadingDef, int labelIndex, VariantList groupExpressionValues)
		{
			m_headingCellIndex = headingCellIndex;
			if (chartHeadingDef.ChartGroupExpression)
			{
				if (groupExpressionValues == null || DBNull.Value == groupExpressionValues[0])
				{
					m_groupExpressionValue = null;
				}
				else
				{
					m_groupExpressionValue = groupExpressionValues[0];
				}
			}
			if (chartHeadingDef.Labels != null)
			{
				ExpressionInfo expressionInfo = chartHeadingDef.Labels[labelIndex];
				if (expressionInfo != null)
				{
					if (chartHeadingDef.Grouping != null)
					{
						m_headingLabel = pc.ReportRuntime.EvaluateChartDynamicHeadingLabelExpression(chartHeadingDef, expressionInfo, chartHeadingDef.DataRegionDef.Name);
					}
					else
					{
						m_headingLabel = pc.ReportRuntime.EvaluateChartStaticHeadingLabelExpression(chartHeadingDef, expressionInfo, chartHeadingDef.DataRegionDef.Name);
					}
				}
			}
			if (chartHeadingDef.Grouping == null)
			{
				m_staticGroupingIndex = labelIndex;
			}
			else if (chartHeadingDef.Grouping.CustomProperties != null)
			{
				m_customPropertyInstances = chartHeadingDef.Grouping.CustomProperties.EvaluateExpressions(chartHeadingDef.DataRegionDef.ObjectType, chartHeadingDef.DataRegionDef.Name, chartHeadingDef.Grouping.Name + ".", pc);
			}
		}

		internal ChartHeadingInstanceInfo()
		{
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.HeadingLabel, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.Variant));
			memberInfoList.Add(new MemberInfo(MemberName.HeadingCellIndex, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.HeadingSpan, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.GroupExpressionValue, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.Variant));
			memberInfoList.Add(new MemberInfo(MemberName.StaticGroupingIndex, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.CustomPropertyInstances, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.DataValueInstanceList));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.InstanceInfo, memberInfoList);
		}
	}
}
