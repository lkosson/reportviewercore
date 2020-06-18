using Microsoft.ReportingServices.ReportProcessing;
using System.Collections;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class ImageMapAreasCollection
	{
		private RenderingContext m_renderingContext;

		private ArrayList m_list;

		public ImageMapArea this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, Count);
				}
				if (IsChartOrCustomControlImageMap)
				{
					return m_list[index] as ImageMapArea;
				}
				return new ImageMapArea(((ImageMapAreaInstanceList)m_list)[index], m_renderingContext);
			}
		}

		private bool IsChartOrCustomControlImageMap => m_renderingContext == null;

		public int Count
		{
			get
			{
				if (m_list != null)
				{
					return m_list.Count;
				}
				return 0;
			}
		}

		public ImageMapAreasCollection()
		{
			m_list = new ArrayList();
		}

		public ImageMapAreasCollection(int capacity)
		{
			m_list = new ArrayList(capacity);
		}

		internal ImageMapAreasCollection(ImageMapAreaInstanceList mapAreasInstances, RenderingContext renderingContext)
		{
			Global.Tracer.Assert(renderingContext != null);
			m_renderingContext = renderingContext;
			m_list = mapAreasInstances;
		}

		public void Add(ImageMapArea mapArea)
		{
			m_list.Add(mapArea);
		}

		internal ImageMapAreasCollection DeepClone()
		{
			Global.Tracer.Assert(IsChartOrCustomControlImageMap);
			if (m_list == null || m_list.Count == 0)
			{
				return null;
			}
			int count = m_list.Count;
			ImageMapAreasCollection imageMapAreasCollection = new ImageMapAreasCollection(count);
			for (int i = 0; i < count; i++)
			{
				imageMapAreasCollection.m_list.Add(((ImageMapArea)m_list[i]).DeepClone());
			}
			return imageMapAreasCollection;
		}

		internal ImageMapAreaInstanceList Deconstruct(Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ProcessingContext processingContext, Microsoft.ReportingServices.ReportProcessing.CustomReportItem context)
		{
			Global.Tracer.Assert(context != null && processingContext != null);
			if (m_list == null || m_list.Count == 0)
			{
				return null;
			}
			int count = m_list.Count;
			ImageMapAreaInstanceList imageMapAreaInstanceList = new ImageMapAreaInstanceList(count);
			for (int i = 0; i < count; i++)
			{
				ImageMapAreaInstance value = ((ImageMapArea)m_list[i]).Deconstruct(context);
				imageMapAreaInstanceList.Add(value);
			}
			return imageMapAreaInstanceList;
		}
	}
}
