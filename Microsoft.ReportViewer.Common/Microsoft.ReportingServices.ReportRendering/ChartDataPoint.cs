using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Specialized;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class ChartDataPoint
	{
		private Chart m_owner;

		private int m_seriesIndex;

		private int m_categoryIndex;

		private ChartDataPointInstance m_chartDataPointInstance;

		private ChartDataPointInstanceInfo m_chartDataPointInstanceInfo;

		private CustomPropertyCollection m_customProperties;

		private ActionInfo m_actionInfo;

		public object[] DataValues
		{
			get
			{
				if (InstanceInfo == null)
				{
					return null;
				}
				return InstanceInfo.DataValues;
			}
		}

		public string DataElementName
		{
			get
			{
				Microsoft.ReportingServices.ReportProcessing.Chart chart = (Microsoft.ReportingServices.ReportProcessing.Chart)m_owner.ReportItemDef;
				int index = IndexDataPointDefinition(chart);
				return chart.ChartDataPoints[index].DataElementName;
			}
		}

		public DataElementOutputTypes DataElementOutput
		{
			get
			{
				Microsoft.ReportingServices.ReportProcessing.Chart chart = (Microsoft.ReportingServices.ReportProcessing.Chart)m_owner.ReportItemDef;
				int index = IndexDataPointDefinition(chart);
				return chart.ChartDataPoints[index].DataElementOutput;
			}
		}

		public CustomPropertyCollection CustomProperties
		{
			get
			{
				CustomPropertyCollection customPropertyCollection = m_customProperties;
				if (m_customProperties == null)
				{
					Microsoft.ReportingServices.ReportProcessing.ChartDataPoint chartDataPointDefinition = ChartDataPointDefinition;
					Global.Tracer.Assert(chartDataPointDefinition != null);
					if (chartDataPointDefinition.CustomProperties == null)
					{
						return null;
					}
					if (m_owner.NoRows)
					{
						customPropertyCollection = new CustomPropertyCollection(chartDataPointDefinition.CustomProperties, null);
					}
					else
					{
						ChartDataPointInstanceInfo instanceInfo = InstanceInfo;
						Global.Tracer.Assert(instanceInfo != null);
						customPropertyCollection = new CustomPropertyCollection(chartDataPointDefinition.CustomProperties, instanceInfo.CustomPropertyInstances);
					}
					if (m_owner.RenderingContext.CacheState)
					{
						m_customProperties = customPropertyCollection;
					}
				}
				return customPropertyCollection;
			}
		}

		public ImageMapAreasCollection MapAreas
		{
			get
			{
				if (m_owner.DataPointMapAreas == null)
				{
					return null;
				}
				int num = m_seriesIndex * m_owner.DataPointCollection.CategoryCount + m_categoryIndex;
				Global.Tracer.Assert(num >= 0 && num < m_owner.DataPointMapAreas.Length);
				return m_owner.DataPointMapAreas[num];
			}
		}

		public ReportUrl HyperLinkURL
		{
			get
			{
				ActionInfo actionInfo = m_actionInfo;
				if (actionInfo == null)
				{
					actionInfo = ActionInfo;
				}
				return actionInfo?.Actions[0].HyperLinkURL;
			}
		}

		public ReportUrl DrillthroughReport
		{
			get
			{
				ActionInfo actionInfo = m_actionInfo;
				if (actionInfo == null)
				{
					actionInfo = ActionInfo;
				}
				return actionInfo?.Actions[0].DrillthroughReport;
			}
		}

		public NameValueCollection DrillthroughParameters
		{
			get
			{
				ActionInfo actionInfo = m_actionInfo;
				if (actionInfo == null)
				{
					actionInfo = ActionInfo;
				}
				return actionInfo?.Actions[0].DrillthroughParameters;
			}
		}

		public string BookmarkLink
		{
			get
			{
				ActionInfo actionInfo = m_actionInfo;
				if (actionInfo == null)
				{
					actionInfo = ActionInfo;
				}
				return actionInfo?.Actions[0].BookmarkLink;
			}
		}

		public ActionInfo ActionInfo
		{
			get
			{
				ActionInfo actionInfo = m_actionInfo;
				if (actionInfo == null)
				{
					Microsoft.ReportingServices.ReportProcessing.ChartDataPoint chartDataPointDefinition = ChartDataPointDefinition;
					if (chartDataPointDefinition != null)
					{
						Microsoft.ReportingServices.ReportProcessing.Action action = chartDataPointDefinition.Action;
						if (action != null)
						{
							ActionInstance actionInstance = null;
							string ownerUniqueName = null;
							if (m_chartDataPointInstance != null)
							{
								actionInstance = InstanceInfo.Action;
								ownerUniqueName = m_chartDataPointInstance.UniqueName.ToString(CultureInfo.InvariantCulture);
							}
							actionInfo = new ActionInfo(action, actionInstance, ownerUniqueName, m_owner.RenderingContext);
							if (m_owner.RenderingContext.CacheState)
							{
								m_actionInfo = actionInfo;
							}
						}
					}
				}
				return actionInfo;
			}
		}

		internal ChartDataPointInstanceInfo InstanceInfo
		{
			get
			{
				if (m_chartDataPointInstance == null)
				{
					return null;
				}
				if (m_chartDataPointInstanceInfo == null)
				{
					m_chartDataPointInstanceInfo = m_chartDataPointInstance.GetInstanceInfo(m_owner.RenderingContext.ChunkManager, ((Microsoft.ReportingServices.ReportProcessing.Chart)m_owner.ReportItemDef).ChartDataPoints);
				}
				return m_chartDataPointInstanceInfo;
			}
		}

		private Microsoft.ReportingServices.ReportProcessing.ChartDataPoint ChartDataPointDefinition
		{
			get
			{
				Microsoft.ReportingServices.ReportProcessing.Chart chart = (Microsoft.ReportingServices.ReportProcessing.Chart)m_owner.ReportItemDef;
				if (m_owner.NoRows)
				{
					return chart.ChartDataPoints[m_seriesIndex * chart.StaticCategoryCount + m_categoryIndex];
				}
				if (InstanceInfo != null)
				{
					return chart.ChartDataPoints[InstanceInfo.DataPointIndex];
				}
				return null;
			}
		}

		internal ChartDataPoint(Chart owner, int seriesIndex, int categoryIndex)
		{
			m_owner = owner;
			m_seriesIndex = seriesIndex;
			m_categoryIndex = categoryIndex;
			if (!owner.NoRows)
			{
				ChartDataPointInstancesList dataPoints = ((ChartInstance)owner.ReportItemInstance).DataPoints;
				m_chartDataPointInstance = dataPoints[seriesIndex][categoryIndex];
			}
		}

		private int IndexDataPointDefinition(Microsoft.ReportingServices.ReportProcessing.Chart chartDef)
		{
			int num = 0;
			if (m_owner.NoRows)
			{
				return m_seriesIndex * chartDef.StaticCategoryCount + m_categoryIndex;
			}
			return InstanceInfo.DataPointIndex;
		}
	}
}
