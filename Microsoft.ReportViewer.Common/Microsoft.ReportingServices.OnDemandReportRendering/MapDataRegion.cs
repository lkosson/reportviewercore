using Microsoft.ReportingServices.ReportIntermediateFormat;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapDataRegion : DataRegion, IMapObjectCollectionItem
	{
		private MapMember m_innerMostMampMember;

		private MapMember m_mapMember;

		public MapMember MapMember
		{
			get
			{
				if (m_mapMember == null)
				{
					m_mapMember = new MapMember(ReportScope, this, this, null, MapDataRegionDef.MapMember);
				}
				return m_mapMember;
			}
		}

		internal MapMember InnerMostMapMember
		{
			get
			{
				if (m_innerMostMampMember == null)
				{
					m_innerMostMampMember = MapMember;
					while (m_innerMostMampMember.ChildMapMember != null)
					{
						m_innerMostMampMember = m_innerMostMampMember.ChildMapMember;
					}
				}
				return m_innerMostMampMember;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.MapDataRegion MapDataRegionDef => m_reportItemDef as Microsoft.ReportingServices.ReportIntermediateFormat.MapDataRegion;

		internal override bool HasDataCells => false;

		internal override IDataRegionRowCollection RowCollection => null;

		public new MapDataRegionInstance Instance => (MapDataRegionInstance)GetOrCreateInstance();

		internal MapDataRegion(IDefinitionPath parentDefinitionPath, int indexIntoParentCollectionDef, Microsoft.ReportingServices.ReportIntermediateFormat.MapDataRegion reportItemDef, RenderingContext renderingContext)
			: base(parentDefinitionPath, indexIntoParentCollectionDef, reportItemDef, renderingContext)
		{
		}

		void IMapObjectCollectionItem.SetNewContext()
		{
			SetNewContext();
		}

		internal List<MapVectorLayer> GetChildLayers()
		{
			MapLayerCollection mapLayers = ((Map)m_parentDefinitionPath).MapLayers;
			List<MapVectorLayer> list = new List<MapVectorLayer>();
			foreach (MapLayer item in mapLayers)
			{
				if (!(item is MapTileLayer) && ((MapVectorLayer)item).MapDataRegion == this)
				{
					list.Add((MapVectorLayer)item);
				}
			}
			return list;
		}

		internal override ReportItemInstance GetOrCreateInstance()
		{
			if (m_instance == null)
			{
				m_instance = new MapDataRegionInstance(this);
			}
			return m_instance;
		}

		internal override void SetNewContextChildren()
		{
			if (m_mapMember != null)
			{
				m_mapMember.ResetContext();
			}
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
		}
	}
}
