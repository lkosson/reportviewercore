using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal class MatrixHeadingInstanceInfo : InstanceInfo
	{
		private NonComputedUniqueNames m_contentUniqueNames;

		private bool m_startHidden;

		private int m_headingCellIndex;

		private int m_headingSpan = 1;

		private object m_groupExpressionValue;

		private string m_label;

		private DataValueInstanceList m_customPropertyInstances;

		internal NonComputedUniqueNames ContentUniqueNames
		{
			get
			{
				return m_contentUniqueNames;
			}
			set
			{
				m_contentUniqueNames = value;
			}
		}

		internal bool StartHidden
		{
			get
			{
				return m_startHidden;
			}
			set
			{
				m_startHidden = value;
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

		internal MatrixHeadingInstanceInfo(ReportProcessing.ProcessingContext pc, int headingCellIndex, MatrixHeading matrixHeadingDef, MatrixHeadingInstance owner, bool isSubtotal, int reportItemDefIndex, VariantList groupExpressionValues, out NonComputedUniqueNames nonComputedUniqueNames)
		{
			ReportItemCollection reportItems;
			if (isSubtotal)
			{
				reportItems = matrixHeadingDef.Subtotal.ReportItems;
			}
			else
			{
				reportItems = matrixHeadingDef.ReportItems;
				if (matrixHeadingDef.OwcGroupExpression)
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
			}
			if (0 < reportItems.Count && !reportItems.IsReportItemComputed(reportItemDefIndex))
			{
				m_contentUniqueNames = NonComputedUniqueNames.CreateNonComputedUniqueNames(pc, reportItems[reportItemDefIndex]);
			}
			nonComputedUniqueNames = m_contentUniqueNames;
			m_headingCellIndex = headingCellIndex;
			if (!isSubtotal && pc.ShowHideType != 0)
			{
				m_startHidden = pc.ProcessReceiver(owner.UniqueName, matrixHeadingDef.Visibility, matrixHeadingDef.ExprHost, matrixHeadingDef.DataRegionDef.ObjectType, matrixHeadingDef.DataRegionDef.Name);
			}
			if (matrixHeadingDef.Grouping != null && matrixHeadingDef.Grouping.GroupLabel != null)
			{
				m_label = pc.NavigationInfo.RegisterLabel(pc.ReportRuntime.EvaluateGroupingLabelExpression(matrixHeadingDef.Grouping, matrixHeadingDef.DataRegionDef.ObjectType, matrixHeadingDef.DataRegionDef.Name));
			}
			if (matrixHeadingDef.Grouping != null && matrixHeadingDef.Grouping.CustomProperties != null)
			{
				m_customPropertyInstances = matrixHeadingDef.Grouping.CustomProperties.EvaluateExpressions(matrixHeadingDef.DataRegionDef.ObjectType, matrixHeadingDef.DataRegionDef.Name, matrixHeadingDef.Grouping.Name + ".", pc);
			}
			matrixHeadingDef.StartHidden = m_startHidden;
		}

		internal MatrixHeadingInstanceInfo()
		{
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.ContentUniqueNames, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.NonComputedUniqueNames));
			memberInfoList.Add(new MemberInfo(MemberName.StartHidden, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.HeadingCellIndex, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.HeadingSpan, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.GroupExpressionValue, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.Variant));
			memberInfoList.Add(new MemberInfo(MemberName.Label, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.CustomPropertyInstances, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.DataValueInstanceList));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.InstanceInfo, memberInfoList);
		}
	}
}
