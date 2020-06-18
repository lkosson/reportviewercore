using Microsoft.ReportingServices.ReportProcessing;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class ListContent : Group, IDocumentMapEntry
	{
		private ListContentInstance m_listContentInstance;

		private ListContentInstanceInfo m_listContentInstanceInfo;

		private ReportItemCollection m_reportItemCollection;

		public override string DataElementName
		{
			get
			{
				Microsoft.ReportingServices.ReportProcessing.List list = (Microsoft.ReportingServices.ReportProcessing.List)base.OwnerDataRegion.ReportItemDef;
				if (list.Grouping == null)
				{
					return list.DataInstanceName;
				}
				return list.Grouping.DataElementName;
			}
		}

		public override DataElementOutputTypes DataElementOutput
		{
			get
			{
				Microsoft.ReportingServices.ReportProcessing.List list = (Microsoft.ReportingServices.ReportProcessing.List)base.OwnerDataRegion.ReportItemDef;
				if (list.Grouping == null)
				{
					return list.DataInstanceElementOutput;
				}
				return list.Grouping.DataElementOutput;
			}
		}

		public override string ID
		{
			get
			{
				if (base.OwnerDataRegion.ReportItemDef.RenderingModelID == null)
				{
					base.OwnerDataRegion.ReportItemDef.RenderingModelID = ((Microsoft.ReportingServices.ReportProcessing.List)base.OwnerDataRegion.ReportItemDef).ListContentID.ToString(CultureInfo.InvariantCulture);
				}
				return base.OwnerDataRegion.ReportItemDef.RenderingModelID;
			}
		}

		public ReportItemCollection ReportItemCollection
		{
			get
			{
				ReportItemCollection reportItemCollection = m_reportItemCollection;
				if (m_reportItemCollection == null)
				{
					ReportItemColInstance reportItemColInstance = null;
					if (m_listContentInstance != null)
					{
						reportItemColInstance = m_listContentInstance.ReportItemColInstance;
					}
					reportItemCollection = new ReportItemCollection(((Microsoft.ReportingServices.ReportProcessing.List)base.OwnerDataRegion.ReportItemDef).ReportItems, reportItemColInstance, base.OwnerDataRegion.RenderingContext, null);
					if (base.OwnerDataRegion.RenderingContext.CacheState)
					{
						m_reportItemCollection = reportItemCollection;
					}
				}
				return reportItemCollection;
			}
		}

		public override string Label
		{
			get
			{
				string result = null;
				if (m_groupingDef != null && m_groupingDef.GroupLabel != null)
				{
					result = ((m_groupingDef.GroupLabel.Type == ExpressionInfo.Types.Constant) ? m_groupingDef.GroupLabel.Value : ((m_listContentInstance != null) ? InstanceInfo.Label : null));
				}
				return result;
			}
		}

		public bool InDocumentMap
		{
			get
			{
				if (m_listContentInstance != null && m_groupingDef != null)
				{
					return m_groupingDef.GroupLabel != null;
				}
				return false;
			}
		}

		public override bool Hidden
		{
			get
			{
				if (m_listContentInstance == null)
				{
					return RenderingContext.GetDefinitionHidden(base.OwnerDataRegion.ReportItemDef.Visibility);
				}
				if (base.OwnerDataRegion.ReportItemDef.Visibility == null)
				{
					return false;
				}
				if (base.OwnerDataRegion.ReportItemDef.Visibility.Toggle != null)
				{
					return base.OwnerDataRegion.RenderingContext.IsItemHidden(m_listContentInstance.UniqueName, potentialSender: false);
				}
				return InstanceInfo.StartHidden;
			}
		}

		public override CustomPropertyCollection CustomProperties
		{
			get
			{
				CustomPropertyCollection customPropertyCollection = m_customProperties;
				if (m_customProperties == null)
				{
					if (m_groupingDef == null || m_groupingDef.CustomProperties == null)
					{
						return null;
					}
					customPropertyCollection = ((m_listContentInstance != null) ? new CustomPropertyCollection(m_groupingDef.CustomProperties, InstanceInfo.CustomPropertyInstances) : new CustomPropertyCollection(m_groupingDef.CustomProperties, null));
					if (base.OwnerDataRegion.RenderingContext.CacheState)
					{
						m_customProperties = customPropertyCollection;
					}
				}
				return customPropertyCollection;
			}
		}

		internal ListContentInstanceInfo InstanceInfo
		{
			get
			{
				if (m_listContentInstance == null)
				{
					return null;
				}
				if (m_listContentInstanceInfo == null)
				{
					m_listContentInstanceInfo = m_listContentInstance.GetInstanceInfo(base.OwnerDataRegion.RenderingContext.ChunkManager);
				}
				return m_listContentInstanceInfo;
			}
		}

		internal ListContent(List owner, int instanceIndex)
			: base(owner, ((Microsoft.ReportingServices.ReportProcessing.List)owner.ReportItemDef).Grouping, owner.ReportItemDef.Visibility)
		{
			if (owner.ReportItemInstance == null)
			{
				return;
			}
			ListContentInstanceList listContents = ((ListInstance)owner.ReportItemInstance).ListContents;
			if (listContents == null)
			{
				return;
			}
			if (instanceIndex < listContents.Count)
			{
				m_listContentInstance = listContents[instanceIndex];
				if (m_listContentInstance != null)
				{
					m_uniqueName = m_listContentInstance.UniqueName;
				}
			}
			else
			{
				Global.Tracer.Assert(listContents.Count == 0);
			}
		}
	}
}
