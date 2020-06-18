using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.OnDemandReportRendering;
using System.Collections.Specialized;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.SPBProcessing
{
	internal sealed class Interactivity
	{
		internal enum EventType
		{
			UserSortEvent,
			FindStringEvent,
			BookmarkNavigationEvent,
			DocumentMapNavigationEvent,
			GetDocumentMap,
			FindChart,
			FindGaugePanel,
			Collect,
			FindImage,
			FindMap,
			ImageConsolidation,
			DrillthroughEvent
		}

		internal class DrillthroughInfo
		{
			private string m_reportName;

			private NameValueCollection m_parameters;

			internal string ReportName
			{
				get
				{
					return m_reportName;
				}
				set
				{
					m_reportName = value;
				}
			}

			internal NameValueCollection Parameters
			{
				get
				{
					return m_parameters;
				}
				set
				{
					m_parameters = value;
				}
			}
		}

		private string m_itemInfo;

		private string m_bookmarkId;

		private bool m_itemFound;

		private string m_streamName;

		private CreateAndRegisterStream m_createAndRegisterStream;

		private EventType m_eventType = EventType.Collect;

		private DrillthroughInfo m_drillthroughResult;

		internal bool Done => m_itemFound;

		internal string ItemInfo => m_itemInfo;

		internal bool NeedPageHeaderFooter
		{
			get
			{
				if (m_eventType == EventType.UserSortEvent || m_eventType == EventType.DocumentMapNavigationEvent || m_eventType == EventType.GetDocumentMap || m_eventType == EventType.FindChart || m_eventType == EventType.FindGaugePanel || m_eventType == EventType.FindMap)
				{
					return false;
				}
				return true;
			}
		}

		internal bool RegisterHiddenItems
		{
			get
			{
				if (m_eventType == EventType.UserSortEvent || m_eventType == EventType.FindStringEvent || m_eventType == EventType.FindChart || m_eventType == EventType.FindGaugePanel || m_eventType == EventType.FindMap || m_eventType == EventType.FindImage || m_eventType == EventType.ImageConsolidation)
				{
					return false;
				}
				return true;
			}
		}

		internal EventType InteractivityEventType => m_eventType;

		internal DrillthroughInfo DrillthroughResult => m_drillthroughResult;

		internal Interactivity()
		{
		}

		internal Interactivity(string itemInfo, EventType eventType, string streamName, CreateAndRegisterStream createAndRegisterStream)
		{
			m_itemInfo = itemInfo;
			m_eventType = eventType;
			m_streamName = streamName;
			m_createAndRegisterStream = createAndRegisterStream;
		}

		internal Interactivity(string itemInfo, EventType eventType)
		{
			m_itemInfo = itemInfo;
			m_eventType = eventType;
		}

		internal Interactivity(EventType eventType)
		{
			m_eventType = eventType;
		}

		internal Interactivity(string bookmarkId)
		{
			m_bookmarkId = bookmarkId;
			m_eventType = EventType.BookmarkNavigationEvent;
		}

		internal bool RegisterItem(PageItem pageItem, PageContext pageContext)
		{
			if (m_itemFound || pageItem == null)
			{
				return false;
			}
			switch (m_eventType)
			{
			case EventType.Collect:
			{
				ReportItemInstance instance5 = pageItem.Source.Instance;
				if (pageContext.Labels != null)
				{
					pageContext.Labels.WriteDocMapLabel(instance5);
				}
				if (pageContext.Bookmarks != null)
				{
					pageContext.Bookmarks.WriteBookmark(instance5);
				}
				if (pageContext.PageBookmarks != null)
				{
					pageContext.RegisterPageBookmark(instance5);
				}
				if (pageItem.ItemState != PageItem.State.OnPageHidden)
				{
					return false;
				}
				break;
			}
			case EventType.BookmarkNavigationEvent:
			{
				ReportItemInstance instance2 = pageItem.Source.Instance;
				if (instance2.Bookmark != null && SPBProcessing.CompareWithOrdinalComparison(m_bookmarkId, instance2.Bookmark, ignoreCase: false) == 0)
				{
					m_itemFound = true;
					m_itemInfo = instance2.UniqueName;
					return false;
				}
				if (pageItem.ItemState != PageItem.State.OnPageHidden)
				{
					return false;
				}
				break;
			}
			case EventType.DrillthroughEvent:
			{
				ReportItemInstance instance = pageItem.Source.Instance;
				TextBoxInstance textBoxInstance = instance as TextBoxInstance;
				if (textBoxInstance != null)
				{
					Microsoft.ReportingServices.OnDemandReportRendering.TextBox textBox = (Microsoft.ReportingServices.OnDemandReportRendering.TextBox)pageItem.Source;
					if (!HasMatchingDrillthrough(textBox.ActionInfo))
					{
						foreach (ParagraphInstance paragraphInstance in textBoxInstance.ParagraphInstances)
						{
							foreach (TextRunInstance textRunInstance in paragraphInstance.TextRunInstances)
							{
								Microsoft.ReportingServices.OnDemandReportRendering.TextRun definition = textRunInstance.Definition;
								if (HasMatchingDrillthrough(definition.ActionInfo))
								{
									return false;
								}
							}
						}
					}
				}
				else
				{
					ImageInstance imageInstance = instance as ImageInstance;
					if (imageInstance != null)
					{
						if (!HasMatchingDrillthrough(imageInstance.ActionInfoWithDynamicImageMapAreas))
						{
							Microsoft.ReportingServices.OnDemandReportRendering.Image image = (Microsoft.ReportingServices.OnDemandReportRendering.Image)pageItem.Source;
							HasMatchingDrillthrough(image.ActionInfo);
						}
					}
					else
					{
						IDynamicImageInstance dynamicImageInstance = instance as IDynamicImageInstance;
						if (dynamicImageInstance != null)
						{
							ActionInfoWithDynamicImageMapCollection actionImageMaps;
							using (dynamicImageInstance.GetImage(DynamicImageInstance.ImageType.PNG, out actionImageMaps))
							{
							}
							HasMatchingDrillthrough(actionImageMaps);
						}
					}
				}
				if (m_itemFound)
				{
					return false;
				}
				if (pageItem.ItemState != PageItem.State.OnPageHidden)
				{
					return false;
				}
				break;
			}
			case EventType.DocumentMapNavigationEvent:
			{
				ReportItemInstance instance3 = pageItem.Source.Instance;
				if (SPBProcessing.CompareWithOrdinalComparison(m_itemInfo, instance3.UniqueName, ignoreCase: true) == 0)
				{
					m_itemFound = true;
					return false;
				}
				if (pageItem.ItemState != PageItem.State.OnPageHidden)
				{
					return false;
				}
				break;
			}
			case EventType.GetDocumentMap:
			{
				ReportItemInstance instance4 = pageItem.Source.Instance;
				if (pageContext.Labels != null)
				{
					pageContext.Labels.WriteDocMapLabel(instance4);
				}
				if (pageItem.ItemState != PageItem.State.OnPageHidden)
				{
					return false;
				}
				break;
			}
			case EventType.FindChart:
			{
				if (pageItem.ItemState == PageItem.State.OnPageHidden)
				{
					break;
				}
				ReportItem source2 = pageItem.Source;
				if (SPBProcessing.CompareWithOrdinalComparison(m_itemInfo, source2.Instance.UniqueName, ignoreCase: true) == 0)
				{
					m_itemFound = true;
					ChartInstance chartInstance2 = source2.Instance as ChartInstance;
					if (chartInstance2 != null)
					{
						WriteDynamicImageStream(chartInstance2.GetImage());
					}
				}
				break;
			}
			case EventType.FindGaugePanel:
			{
				if (pageItem.ItemState == PageItem.State.OnPageHidden)
				{
					break;
				}
				ReportItem source3 = pageItem.Source;
				if (SPBProcessing.CompareWithOrdinalComparison(m_itemInfo, source3.Instance.UniqueName, ignoreCase: true) == 0)
				{
					m_itemFound = true;
					GaugePanelInstance gaugePanelInstance2 = source3.Instance as GaugePanelInstance;
					if (gaugePanelInstance2 != null)
					{
						WriteDynamicImageStream(gaugePanelInstance2.GetImage());
					}
				}
				break;
			}
			case EventType.FindMap:
			{
				if (pageItem.ItemState == PageItem.State.OnPageHidden)
				{
					break;
				}
				ReportItem source5 = pageItem.Source;
				if (SPBProcessing.CompareWithOrdinalComparison(m_itemInfo, source5.Instance.UniqueName, ignoreCase: true) == 0)
				{
					m_itemFound = true;
					MapInstance mapInstance2 = source5.Instance as MapInstance;
					if (mapInstance2 != null)
					{
						WriteDynamicImageStream(mapInstance2.GetImage());
					}
				}
				break;
			}
			case EventType.FindImage:
			{
				if (pageItem.ItemState == PageItem.State.OnPageHidden)
				{
					break;
				}
				ReportItem source4 = pageItem.Source;
				if (SPBProcessing.CompareWithOrdinalComparison(m_itemInfo, source4.Instance.UniqueName, ignoreCase: true) != 0)
				{
					break;
				}
				m_itemFound = true;
				ImageInstance imageInstance2 = source4.Instance as ImageInstance;
				if (imageInstance2 != null)
				{
					Stream stream3 = m_createAndRegisterStream(m_streamName, string.Empty, null, imageInstance2.MIMEType, willSeek: false, StreamOper.CreateAndRegister);
					byte[] imageData = imageInstance2.ImageData;
					if (stream3 != null && imageData != null && imageData.Length != 0)
					{
						stream3.Write(imageData, 0, imageData.Length);
					}
				}
				break;
			}
			case EventType.ImageConsolidation:
			{
				if (pageItem.ItemState == PageItem.State.OnPageHidden)
				{
					break;
				}
				ReportItem source = pageItem.Source;
				GaugePanelInstance gaugePanelInstance = source.Instance as GaugePanelInstance;
				Stream stream = null;
				if (gaugePanelInstance != null)
				{
					stream = gaugePanelInstance.GetImage();
				}
				else
				{
					ChartInstance chartInstance = source.Instance as ChartInstance;
					if (chartInstance != null)
					{
						stream = chartInstance.GetImage();
					}
					else
					{
						MapInstance mapInstance = source.Instance as MapInstance;
						if (mapInstance != null)
						{
							stream = mapInstance.GetImage();
						}
					}
				}
				if (stream != null)
				{
					ImageConsolidation imageConsolidation = pageContext.ImageConsolidation;
					imageConsolidation.AppendImage(stream);
					if (imageConsolidation.CurrentOffset >= imageConsolidation.IgnoreOffsetTill + 1 && imageConsolidation.ImageInfos.Count > 0)
					{
						m_itemFound = true;
					}
				}
				break;
			}
			default:
				FindTextBox(pageItem as TextBox, pageContext);
				break;
			}
			return true;
		}

		private bool HasMatchingDrillthrough(ActionInfoWithDynamicImageMapCollection imageMaps)
		{
			if (imageMaps == null)
			{
				return false;
			}
			foreach (ActionInfoWithDynamicImageMap imageMap in imageMaps)
			{
				if (HasMatchingDrillthrough(imageMap))
				{
					return true;
				}
			}
			return false;
		}

		private bool HasMatchingDrillthrough(ActionInfo actionInfo)
		{
			if (actionInfo == null)
			{
				return false;
			}
			foreach (Action action in actionInfo.Actions)
			{
				ActionDrillthrough drillthrough = action.Drillthrough;
				if (drillthrough == null)
				{
					continue;
				}
				ActionDrillthroughInstance instance = drillthrough.Instance;
				if (instance != null && SPBProcessing.CompareWithOrdinalComparison(m_itemInfo, instance.DrillthroughID, ignoreCase: false) == 0)
				{
					m_drillthroughResult = new DrillthroughInfo();
					m_drillthroughResult.ReportName = instance.ReportName;
					if (drillthrough.Parameters != null)
					{
						m_drillthroughResult.Parameters = drillthrough.Parameters.ToNameValueCollectionForDrillthroughEvent();
					}
					m_itemFound = true;
					return true;
				}
			}
			return false;
		}

		private void WriteDynamicImageStream(Stream startStream)
		{
			Stream stream = m_createAndRegisterStream(m_streamName, "png", null, PageContext.PNG_MIME_TYPE, willSeek: false, StreamOper.CreateAndRegister);
			if (startStream != null && stream != null)
			{
				startStream.Position = 0L;
				byte[] array = new byte[4096];
				for (int num = startStream.Read(array, 0, array.Length); num != 0; num = startStream.Read(array, 0, array.Length))
				{
					stream.Write(array, 0, num);
				}
			}
		}

		private void FindTextBox(TextBox textbox, PageContext pageContext)
		{
			if (m_itemFound || textbox == null || textbox.ItemState == PageItem.State.OnPageHidden)
			{
				return;
			}
			if (m_eventType == EventType.FindStringEvent)
			{
				m_itemFound = textbox.SearchTextBox(m_itemInfo, pageContext);
			}
			else if (m_eventType == EventType.UserSortEvent)
			{
				ReportItem source = textbox.Source;
				if (SPBProcessing.CompareWithOrdinalComparison(m_itemInfo, source.Instance.UniqueName, ignoreCase: true) == 0)
				{
					m_itemFound = true;
				}
			}
		}

		internal bool RegisterHiddenItem(ReportItem reportItem, PageContext pageContext)
		{
			if (m_eventType == EventType.Collect)
			{
				ReportItemInstance instance = reportItem.Instance;
				if (pageContext.Labels != null)
				{
					pageContext.Labels.WriteDocMapLabel(instance);
				}
				if (pageContext.Bookmarks != null)
				{
					pageContext.Bookmarks.WriteBookmark(instance);
				}
				if (pageContext.PageBookmarks != null)
				{
					pageContext.RegisterPageBookmark(instance);
				}
			}
			else if (m_eventType == EventType.GetDocumentMap)
			{
				ReportItemInstance instance2 = reportItem.Instance;
				if (pageContext.Labels != null)
				{
					pageContext.Labels.WriteDocMapLabel(instance2);
				}
			}
			else if (m_eventType == EventType.BookmarkNavigationEvent)
			{
				ReportItemInstance instance3 = reportItem.Instance;
				if (instance3.Bookmark != null && SPBProcessing.CompareWithOrdinalComparison(m_bookmarkId, instance3.Bookmark, ignoreCase: false) == 0)
				{
					m_itemFound = true;
					m_itemInfo = instance3.UniqueName;
					return false;
				}
			}
			else
			{
				if (m_eventType != EventType.DocumentMapNavigationEvent)
				{
					return false;
				}
				ReportItemInstance instance4 = reportItem.Instance;
				if (SPBProcessing.CompareWithOrdinalComparison(m_itemInfo, instance4.UniqueName, ignoreCase: true) == 0)
				{
					m_itemFound = true;
					return false;
				}
			}
			return true;
		}

		internal void RegisterGroupLabel(Group group, PageContext pageContext)
		{
			if (m_itemFound || group == null)
			{
				return;
			}
			if (m_eventType == EventType.Collect || m_eventType == EventType.GetDocumentMap)
			{
				GroupInstance instance = group.Instance;
				if (pageContext.Labels != null)
				{
					pageContext.Labels.WriteDocMapLabel(instance);
				}
			}
			else if (m_eventType == EventType.DocumentMapNavigationEvent)
			{
				GroupInstance instance2 = group.Instance;
				if (SPBProcessing.CompareWithOrdinalComparison(m_itemInfo, instance2.UniqueName, ignoreCase: true) == 0)
				{
					m_itemFound = true;
				}
			}
		}

		internal void RegisterDocMapRootLabel(string rootLabelUniqueName, PageContext pageContext)
		{
			if (m_itemFound || rootLabelUniqueName == null || pageContext.PageNumber != 1)
			{
				return;
			}
			if (m_eventType == EventType.Collect || m_eventType == EventType.GetDocumentMap)
			{
				if (pageContext.Labels != null)
				{
					pageContext.Labels.WriteDocMapRootLabel(rootLabelUniqueName);
				}
			}
			else if (m_eventType == EventType.DocumentMapNavigationEvent && SPBProcessing.CompareWithOrdinalComparison(m_itemInfo, rootLabelUniqueName, ignoreCase: true) == 0)
			{
				m_itemFound = true;
			}
		}
	}
}
