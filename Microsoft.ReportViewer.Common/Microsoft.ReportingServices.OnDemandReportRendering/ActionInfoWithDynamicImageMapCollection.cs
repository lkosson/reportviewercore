using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportRendering;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ActionInfoWithDynamicImageMapCollection : ReportElementCollectionBase<ActionInfoWithDynamicImageMap>
	{
		private List<ActionInfoWithDynamicImageMap> m_list;

		public override ActionInfoWithDynamicImageMap this[int index]
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

		internal List<ActionInfoWithDynamicImageMap> InternalList => m_list;

		internal ActionInfoWithDynamicImageMapCollection()
		{
			m_list = new List<ActionInfoWithDynamicImageMap>();
		}

		internal ActionInfoWithDynamicImageMapCollection(RenderingContext renderingContext, ImageMapAreasCollection imageMaps)
		{
			int count = imageMaps.Count;
			m_list = new List<ActionInfoWithDynamicImageMap>(count);
			for (int i = 0; i < count; i++)
			{
				Microsoft.ReportingServices.ReportRendering.ImageMapArea imageMapArea = imageMaps[i];
				if (imageMapArea != null && imageMapArea.ActionInfo != null)
				{
					ImageMapAreasCollection imageMapAreasCollection = new ImageMapAreasCollection(1);
					imageMapAreasCollection.Add(imageMapArea);
					m_list.Add(new ActionInfoWithDynamicImageMap(renderingContext, imageMapArea.ActionInfo, imageMapAreasCollection));
				}
			}
		}

		internal ActionInfoWithDynamicImageMap Add(RenderingContext renderingContext, ReportItem owner, IROMActionOwner romActionOwner)
		{
			ActionInfoWithDynamicImageMap actionInfoWithDynamicImageMap = new ActionInfoWithDynamicImageMap(renderingContext, owner, romActionOwner);
			m_list.Add(actionInfoWithDynamicImageMap);
			return actionInfoWithDynamicImageMap;
		}

		internal void ConstructDefinitions()
		{
			foreach (ActionInfoWithDynamicImageMap item in m_list)
			{
				item.ConstructActionDefinition();
			}
		}
	}
}
