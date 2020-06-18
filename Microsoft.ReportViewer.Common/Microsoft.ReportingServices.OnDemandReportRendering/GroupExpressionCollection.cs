using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class GroupExpressionCollection : ReportElementCollectionBase<ReportVariantProperty>
	{
		private List<ReportVariantProperty> m_list;

		public override ReportVariantProperty this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, Count);
				}
				return m_list[index];
			}
		}

		public override int Count => m_list.Count;

		internal GroupExpressionCollection()
		{
			m_list = new List<ReportVariantProperty>();
		}

		internal GroupExpressionCollection(Microsoft.ReportingServices.ReportIntermediateFormat.Grouping grouping)
		{
			if (grouping == null || grouping.GroupExpressions == null)
			{
				m_list = new List<ReportVariantProperty>();
				return;
			}
			int count = grouping.GroupExpressions.Count;
			m_list = new List<ReportVariantProperty>(count);
			for (int i = 0; i < count; i++)
			{
				m_list.Add(new ReportVariantProperty(grouping.GroupExpressions[i]));
			}
		}

		internal GroupExpressionCollection(Microsoft.ReportingServices.ReportProcessing.Grouping grouping)
		{
			if (grouping == null || grouping.GroupExpressions == null)
			{
				m_list = new List<ReportVariantProperty>();
				return;
			}
			int count = grouping.GroupExpressions.Count;
			m_list = new List<ReportVariantProperty>(count);
			for (int i = 0; i < count; i++)
			{
				m_list.Add(new ReportVariantProperty(grouping.GroupExpressions[i]));
			}
		}
	}
}
