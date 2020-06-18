using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportRendering;
using System;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class Rectangle : ReportItem, IPageBreakItem
	{
		private Microsoft.ReportingServices.ReportRendering.Rectangle m_renderRectangle;

		private ListContent m_listContents;

		private ReportItemCollection m_reportItems;

		private PageBreak m_pageBreak;

		private ReportStringProperty m_pageName;

		public override string Name
		{
			get
			{
				if (m_isListContentsRectangle)
				{
					return m_listContents.OwnerDataRegion.Name + "_Contents";
				}
				return base.Name;
			}
		}

		public override int LinkToChild
		{
			get
			{
				if (m_isOldSnapshot)
				{
					if (m_isListContentsRectangle)
					{
						return -1;
					}
					return m_renderRectangle.LinkToChild;
				}
				return ((Microsoft.ReportingServices.ReportIntermediateFormat.Rectangle)base.ReportItemDef).LinkToChild;
			}
		}

		public override DataElementOutputTypes DataElementOutput
		{
			get
			{
				if (m_isOldSnapshot && m_isListContentsRectangle)
				{
					return DataElementOutputTypes.ContentsOnly;
				}
				return base.DataElementOutput;
			}
		}

		public override ReportSize Top
		{
			get
			{
				if (m_isListContentsRectangle)
				{
					return new ReportSize("0 mm", 0.0);
				}
				return base.Top;
			}
		}

		public override ReportSize Left
		{
			get
			{
				if (m_isListContentsRectangle)
				{
					return new ReportSize("0 mm", 0.0);
				}
				return base.Left;
			}
		}

		public bool IsSimple
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return m_isListContentsRectangle;
				}
				return ((Microsoft.ReportingServices.ReportIntermediateFormat.Rectangle)m_reportItemDef).IsSimple;
			}
		}

		public ReportItemCollection ReportItemCollection
		{
			get
			{
				if (m_reportItems == null)
				{
					if (m_isOldSnapshot)
					{
						if (m_isListContentsRectangle)
						{
							m_reportItems = new ReportItemCollection(this, m_inSubtotal, m_listContents.ReportItemCollection, m_renderingContext);
						}
						else
						{
							m_reportItems = new ReportItemCollection(this, m_inSubtotal, m_renderRectangle.ReportItemCollection, m_renderingContext);
						}
					}
					else
					{
						m_reportItems = new ReportItemCollection(ReportScope, this, ((Microsoft.ReportingServices.ReportIntermediateFormat.Rectangle)m_reportItemDef).ReportItems, m_renderingContext);
					}
				}
				return m_reportItems;
			}
		}

		public PageBreak PageBreak
		{
			get
			{
				if (m_pageBreak == null)
				{
					if (m_isOldSnapshot)
					{
						m_pageBreak = new PageBreak(pageBreaklocation: (!m_isListContentsRectangle) ? PageBreakHelper.GetPageBreakLocation(m_renderRectangle.PageBreakAtStart, m_renderRectangle.PageBreakAtEnd) : PageBreakHelper.GetPageBreakLocation(pageBreakAtStart: false, pageBreakAtEnd: false), renderingContext: base.RenderingContext, reportScope: ReportScope);
					}
					else
					{
						IPageBreakOwner pageBreakOwner = (Microsoft.ReportingServices.ReportIntermediateFormat.Rectangle)m_reportItemDef;
						m_pageBreak = new PageBreak(base.RenderingContext, ReportScope, pageBreakOwner);
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
						m_pageName = new ReportStringProperty(((Microsoft.ReportingServices.ReportIntermediateFormat.Rectangle)m_reportItemDef).PageName);
					}
				}
				return m_pageName;
			}
		}

		public bool KeepTogether
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return m_isListContentsRectangle;
				}
				return ((Microsoft.ReportingServices.ReportIntermediateFormat.Rectangle)base.ReportItemDef).KeepTogether;
			}
		}

		public bool OmitBorderOnPageBreak
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return false;
				}
				return ((Microsoft.ReportingServices.ReportIntermediateFormat.Rectangle)base.ReportItemDef).OmitBorderOnPageBreak;
			}
		}

		[Obsolete("Use PageBreak.BreakLocation instead.")]
		PageBreakLocation IPageBreakItem.PageBreakLocation
		{
			get
			{
				if (m_isOldSnapshot)
				{
					if (m_isListContentsRectangle)
					{
						return PageBreakHelper.GetPageBreakLocation(pageBreakAtStart: false, pageBreakAtEnd: false);
					}
					return PageBreakHelper.GetPageBreakLocation(m_renderRectangle.PageBreakAtStart, m_renderRectangle.PageBreakAtEnd);
				}
				if (((IPageBreakOwner)base.ReportItemDef).PageBreak != null)
				{
					PageBreak pageBreak = PageBreak;
					if (pageBreak.HasEnabledInstance)
					{
						return pageBreak.BreakLocation;
					}
				}
				return PageBreakLocation.None;
			}
		}

		internal override Microsoft.ReportingServices.ReportRendering.ReportItem RenderReportItem
		{
			get
			{
				if (m_isListContentsRectangle)
				{
					return m_listContents.OwnerDataRegion;
				}
				return m_renderReportItem;
			}
		}

		public override Visibility Visibility
		{
			get
			{
				if (m_isListContentsRectangle)
				{
					return null;
				}
				return base.Visibility;
			}
		}

		internal bool IsListContentsRectangle => m_isListContentsRectangle;

		internal Rectangle(IReportScope reportScope, IDefinitionPath parentDefinitionPath, int indexIntoParentCollectionDef, Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem reportItemDef, RenderingContext renderingContext)
			: base(reportScope, parentDefinitionPath, indexIntoParentCollectionDef, reportItemDef, renderingContext)
		{
		}

		internal Rectangle(IDefinitionPath parentDefinitionPath, int indexIntoParentCollectionDef, bool inSubtotal, Microsoft.ReportingServices.ReportRendering.Rectangle renderRectangle, RenderingContext renderingContext)
			: base(parentDefinitionPath, indexIntoParentCollectionDef, inSubtotal, renderRectangle, renderingContext)
		{
			m_renderRectangle = renderRectangle;
		}

		internal Rectangle(IDefinitionPath parentDefinitionPath, bool inSubtotal, ListContent renderContents, RenderingContext renderingContext)
			: base(parentDefinitionPath, inSubtotal, renderingContext)
		{
			m_listContents = renderContents;
		}

		internal override ReportItemInstance GetOrCreateInstance()
		{
			if (m_instance == null)
			{
				m_instance = new RectangleInstance(this);
			}
			return m_instance;
		}

		internal override void SetNewContextChildren()
		{
			if (m_reportItems != null)
			{
				m_reportItems.SetNewContext();
			}
			if (m_pageBreak != null)
			{
				m_pageBreak.SetNewContext();
			}
		}

		internal void UpdateListContents(ListContent listContents)
		{
			if (!m_isOldSnapshot)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
			}
			SetNewContext();
			if (listContents != null)
			{
				m_listContents = listContents;
			}
			if (m_reportItems != null)
			{
				m_reportItems.UpdateRenderReportItem(listContents.ReportItemCollection);
			}
		}

		internal override void UpdateRenderReportItem(Microsoft.ReportingServices.ReportRendering.ReportItem renderReportItem)
		{
			base.UpdateRenderReportItem(renderReportItem);
			m_renderRectangle = (Microsoft.ReportingServices.ReportRendering.Rectangle)renderReportItem;
			if (m_reportItems != null && renderReportItem is Microsoft.ReportingServices.ReportRendering.Rectangle)
			{
				m_reportItems.UpdateRenderReportItem(m_renderRectangle.ReportItemCollection);
			}
		}
	}
}
