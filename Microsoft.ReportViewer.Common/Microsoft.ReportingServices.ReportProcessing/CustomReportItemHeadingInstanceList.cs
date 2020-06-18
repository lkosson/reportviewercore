using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class CustomReportItemHeadingInstanceList : ArrayList
	{
		[NonSerialized]
		private CustomReportItemHeadingInstance m_lastHeadingInstance;

		internal new CustomReportItemHeadingInstance this[int index] => (CustomReportItemHeadingInstance)base[index];

		internal CustomReportItemHeadingInstanceList()
		{
		}

		internal CustomReportItemHeadingInstanceList(int capacity)
			: base(capacity)
		{
		}

		internal void Add(CustomReportItemHeadingInstance headingInstance, ReportProcessing.ProcessingContext pc)
		{
			if (m_lastHeadingInstance != null)
			{
				m_lastHeadingInstance.HeadingSpan = headingInstance.HeadingCellIndex - m_lastHeadingInstance.HeadingCellIndex;
			}
			base.Add(headingInstance);
			m_lastHeadingInstance = headingInstance;
		}

		internal void SetLastHeadingSpan(int currentCellIndex, ReportProcessing.ProcessingContext pc)
		{
			if (m_lastHeadingInstance != null)
			{
				m_lastHeadingInstance.HeadingSpan = currentCellIndex - m_lastHeadingInstance.HeadingCellIndex;
			}
		}
	}
}
