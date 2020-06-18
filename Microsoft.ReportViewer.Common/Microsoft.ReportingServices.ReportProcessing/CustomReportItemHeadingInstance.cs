using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class CustomReportItemHeadingInstance
	{
		private CustomReportItemHeadingInstanceList m_subHeadingInstances;

		[Reference]
		private CustomReportItemHeading m_headingDef;

		private int m_headingCellIndex;

		private int m_headingSpan = 1;

		private DataValueInstanceList m_customPropertyInstances;

		private string m_label;

		private VariantList m_groupExpressionValues;

		[NonSerialized]
		private int m_recursiveLevel = -1;

		internal CustomReportItemHeadingInstanceList SubHeadingInstances
		{
			get
			{
				return m_subHeadingInstances;
			}
			set
			{
				m_subHeadingInstances = value;
			}
		}

		internal CustomReportItemHeading HeadingDefinition
		{
			get
			{
				return m_headingDef;
			}
			set
			{
				m_headingDef = value;
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

		internal string Label
		{
			get
			{
				return m_label;
			}
			set
			{
				m_label = value;
			}
		}

		internal VariantList GroupExpressionValues
		{
			get
			{
				return m_groupExpressionValues;
			}
			set
			{
				m_groupExpressionValues = value;
			}
		}

		internal int RecursiveLevel => m_recursiveLevel;

		internal CustomReportItemHeadingInstance(ReportProcessing.ProcessingContext pc, int headingCellIndex, CustomReportItemHeading headingDef, VariantList groupExpressionValues, int recursiveLevel)
		{
			if (headingDef.InnerHeadings != null)
			{
				m_subHeadingInstances = new CustomReportItemHeadingInstanceList();
			}
			m_headingDef = headingDef;
			m_headingCellIndex = headingCellIndex;
			if (groupExpressionValues != null)
			{
				m_groupExpressionValues = new VariantList(groupExpressionValues.Count);
				for (int i = 0; i < groupExpressionValues.Count; i++)
				{
					if (groupExpressionValues[i] == null || DBNull.Value == groupExpressionValues[i])
					{
						m_groupExpressionValues.Add(null);
					}
					else
					{
						m_groupExpressionValues.Add(groupExpressionValues[i]);
					}
				}
			}
			if (headingDef.Grouping != null && headingDef.Grouping.GroupLabel != null)
			{
				m_label = pc.NavigationInfo.RegisterLabel(pc.ReportRuntime.EvaluateGroupingLabelExpression(headingDef.Grouping, headingDef.DataRegionDef.ObjectType, headingDef.DataRegionDef.Name));
			}
			if (headingDef.CustomProperties != null)
			{
				m_customPropertyInstances = headingDef.CustomProperties.EvaluateExpressions(headingDef.DataRegionDef.ObjectType, headingDef.DataRegionDef.Name, "DataGrouping.", pc);
			}
			m_recursiveLevel = recursiveLevel;
		}

		internal CustomReportItemHeadingInstance()
		{
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.SubHeadingInstances, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.CustomReportItemHeadingInstanceList));
			memberInfoList.Add(new MemberInfo(MemberName.HeadingDefinition, Token.Reference, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.CustomReportItemHeading));
			memberInfoList.Add(new MemberInfo(MemberName.HeadingCellIndex, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.HeadingSpan, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.CustomPropertyInstances, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.DataValueInstanceList));
			memberInfoList.Add(new MemberInfo(MemberName.Label, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.GroupExpressionValue, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.VariantList));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
