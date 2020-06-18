using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportRendering;
using System;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class DataRegion : ReportItem, IPageBreakItem, IReportScope, IDataRegion
	{
		internal enum Type
		{
			None,
			List,
			Table,
			Matrix,
			Chart,
			GaugePanel,
			CustomReportItem,
			MapDataRegion
		}

		private ReportStringProperty m_noRowsMessage;

		private PageBreak m_pageBreak;

		private ReportStringProperty m_pageName;

		internal Type m_snapshotDataRegionType;

		public PageBreak PageBreak
		{
			get
			{
				if (m_pageBreak == null)
				{
					if (m_isOldSnapshot)
					{
						Microsoft.ReportingServices.ReportRendering.DataRegion dataRegion = (Microsoft.ReportingServices.ReportRendering.DataRegion)m_renderReportItem;
						m_pageBreak = new PageBreak(m_renderingContext, ReportScope, PageBreakHelper.GetPageBreakLocation(dataRegion.PageBreakAtStart, dataRegion.PageBreakAtEnd));
					}
					else
					{
						IPageBreakOwner pageBreakOwner = (Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion)m_reportItemDef;
						m_pageBreak = new PageBreak(m_renderingContext, ReportScope, pageBreakOwner);
					}
				}
				return m_pageBreak;
			}
		}

		public ReportStringProperty PageName
		{
			get
			{
				if (m_pageName == null)
				{
					if (m_isOldSnapshot)
					{
						m_pageName = new ReportStringProperty();
					}
					else
					{
						m_pageName = new ReportStringProperty(((Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion)m_reportItemDef).PageName);
					}
				}
				return m_pageName;
			}
		}

		public ReportStringProperty NoRowsMessage
		{
			get
			{
				if (m_noRowsMessage == null)
				{
					if (m_isOldSnapshot)
					{
						Microsoft.ReportingServices.ReportProcessing.ExpressionInfo noRows = ((Microsoft.ReportingServices.ReportProcessing.DataRegion)m_renderReportItem.ReportItemDef).NoRows;
						m_noRowsMessage = new ReportStringProperty(noRows);
					}
					else
					{
						Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo noRowsMessage = ((Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion)m_reportItemDef).NoRowsMessage;
						m_noRowsMessage = new ReportStringProperty(noRowsMessage);
					}
				}
				return m_noRowsMessage;
			}
		}

		public string DataSetName
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return ((Microsoft.ReportingServices.ReportRendering.DataRegion)m_renderReportItem).DataSetName;
				}
				return ((Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion)base.ReportItemDef).DataSetName;
			}
		}

		public override Visibility Visibility
		{
			get
			{
				if (m_isOldSnapshot && m_snapshotDataRegionType == Type.List)
				{
					return null;
				}
				return base.Visibility;
			}
		}

		[Obsolete("Use PageBreak.BreakLocation instead.")]
		PageBreakLocation IPageBreakItem.PageBreakLocation
		{
			get
			{
				if (m_isOldSnapshot)
				{
					Microsoft.ReportingServices.ReportRendering.DataRegion dataRegion = (Microsoft.ReportingServices.ReportRendering.DataRegion)m_renderReportItem;
					return PageBreakHelper.GetPageBreakLocation(dataRegion.PageBreakAtStart, dataRegion.PageBreakAtEnd);
				}
				PageBreak pageBreak = PageBreak;
				if (pageBreak.HasEnabledInstance)
				{
					return pageBreak.BreakLocation;
				}
				return PageBreakLocation.None;
			}
		}

		internal Type DataRegionType => m_snapshotDataRegionType;

		IReportScopeInstance IReportScope.ReportScopeInstance => (IReportScopeInstance)base.Instance;

		IRIFReportScope IReportScope.RIFReportScope => (IRIFReportScope)m_reportItemDef;

		internal override IReportScope ReportScope => this;

		bool IDataRegion.HasDataCells => HasDataCells;

		internal abstract bool HasDataCells
		{
			get;
		}

		IDataRegionRowCollection IDataRegion.RowCollection => RowCollection;

		internal abstract IDataRegionRowCollection RowCollection
		{
			get;
		}

		internal DataRegion(IDefinitionPath parentDefinitionPath, int indexIntoParentCollectionDef, Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem reportItemDef, RenderingContext renderingContext)
			: base(null, parentDefinitionPath, indexIntoParentCollectionDef, reportItemDef, renderingContext)
		{
		}

		internal DataRegion(IDefinitionPath parentDefinitionPath, int indexIntoParentCollectionDef, bool inSubtotal, Microsoft.ReportingServices.ReportRendering.ReportItem renderDataRegion, RenderingContext renderingContext)
			: base(parentDefinitionPath, indexIntoParentCollectionDef, inSubtotal, renderDataRegion, renderingContext)
		{
		}

		public int[] GetRepeatSiblings()
		{
			if (m_isOldSnapshot)
			{
				return ((Microsoft.ReportingServices.ReportRendering.DataRegion)m_renderReportItem).GetRepeatSiblings();
			}
			Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion dataRegion = (Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion)base.ReportItemDef;
			if (dataRegion.RepeatSiblings == null)
			{
				return new int[0];
			}
			int[] array = new int[dataRegion.RepeatSiblings.Count];
			dataRegion.RepeatSiblings.CopyTo(array);
			return array;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (m_pageBreak != null)
			{
				m_pageBreak.SetNewContext();
			}
			if (!m_isOldSnapshot)
			{
				((Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion)base.ReportItemDef).ClearStreamingScopeInstanceBinding();
			}
		}
	}
}
