using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportRendering;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ImageMapAreaInstanceCollection : ReportElementCollectionBase<ImageMapAreaInstance>
	{
		private List<ImageMapAreaInstance> m_list;

		public override ImageMapAreaInstance this[int index]
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

		internal List<ImageMapAreaInstance> InternalList => m_list;

		internal ImageMapAreaInstanceCollection()
		{
			m_list = new List<ImageMapAreaInstance>();
		}

		internal ImageMapAreaInstanceCollection(ImageMapAreasCollection imageMaps)
		{
			if (imageMaps == null)
			{
				m_list = new List<ImageMapAreaInstance>();
				return;
			}
			int count = imageMaps.Count;
			m_list = new List<ImageMapAreaInstance>(count);
			for (int i = 0; i < count; i++)
			{
				m_list.Add(new ImageMapAreaInstance(imageMaps[i]));
			}
		}

		internal ImageMapAreaInstance Add(ImageMapArea.ImageMapAreaShape shape, float[] coordinates)
		{
			return Add(shape, coordinates, null);
		}

		internal ImageMapAreaInstance Add(ImageMapArea.ImageMapAreaShape shape, float[] coordinates, string toolTip)
		{
			ImageMapAreaInstance imageMapAreaInstance = new ImageMapAreaInstance(shape, coordinates, toolTip);
			m_list.Add(imageMapAreaInstance);
			return imageMapAreaInstance;
		}
	}
}
