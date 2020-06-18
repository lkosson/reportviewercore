using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ChartHeadingInstanceList : ArrayList
	{
		[NonSerialized]
		private ChartHeadingInstance m_lastHeadingInstance;

		internal new ChartHeadingInstance this[int index] => (ChartHeadingInstance)base[index];

		internal ChartHeadingInstanceList()
		{
		}

		internal ChartHeadingInstanceList(int capacity)
			: base(capacity)
		{
		}

		internal void Add(ChartHeadingInstance chartHeadingInstance, ReportProcessing.ProcessingContext pc)
		{
			if (m_lastHeadingInstance != null)
			{
				m_lastHeadingInstance.InstanceInfo.HeadingSpan = chartHeadingInstance.InstanceInfo.HeadingCellIndex - m_lastHeadingInstance.InstanceInfo.HeadingCellIndex;
				pc.ChunkManager.AddInstance(m_lastHeadingInstance.InstanceInfo, m_lastHeadingInstance, pc.InPageSection);
			}
			base.Add(chartHeadingInstance);
			m_lastHeadingInstance = chartHeadingInstance;
		}

		internal void SetLastHeadingSpan(int currentCellIndex, ReportProcessing.ProcessingContext pc)
		{
			if (m_lastHeadingInstance != null)
			{
				m_lastHeadingInstance.InstanceInfo.HeadingSpan = currentCellIndex - m_lastHeadingInstance.InstanceInfo.HeadingCellIndex;
				pc.ChunkManager.AddInstance(m_lastHeadingInstance.InstanceInfo, m_lastHeadingInstance, pc.InPageSection);
			}
		}
	}
}
