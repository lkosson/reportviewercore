using Microsoft.ReportingServices.ReportProcessing;
using System.Collections;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class PageReportItems
	{
		private ArrayList m_innerArrayList = new ArrayList();

		public ReportItem this[int index]
		{
			get
			{
				if (0 > index || index >= Count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, Count);
				}
				return (ReportItem)m_innerArrayList[index];
			}
			set
			{
				if (0 > index || index >= Count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, Count);
				}
				m_innerArrayList[index] = value;
			}
		}

		public int Count => m_innerArrayList.Count;

		public void Add(ReportItem value)
		{
			if (value != null)
			{
				m_innerArrayList.Add(value);
			}
		}

		public void Clear()
		{
			m_innerArrayList.Clear();
		}
	}
}
