using Microsoft.ReportingServices.ReportProcessing;
using System.Collections;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class Rectangle : ReportItem
	{
		private NonComputedUniqueNames[] m_childrenNonComputedUniqueNames;

		private ReportItemCollection m_reportItemCollection;

		public override object SharedRenderingInfo
		{
			get
			{
				Hashtable sharedRenderingInfo = base.RenderingContext.RenderingInfoManager.SharedRenderingInfo;
				if (base.ReportItemDef is Microsoft.ReportingServices.ReportProcessing.Report)
				{
					return sharedRenderingInfo[((Microsoft.ReportingServices.ReportProcessing.Report)base.ReportItemDef).BodyID];
				}
				return sharedRenderingInfo[base.ReportItemDef.ID];
			}
			set
			{
				Hashtable sharedRenderingInfo = base.RenderingContext.RenderingInfoManager.SharedRenderingInfo;
				if (base.ReportItemDef is Microsoft.ReportingServices.ReportProcessing.Report)
				{
					sharedRenderingInfo[((Microsoft.ReportingServices.ReportProcessing.Report)base.ReportItemDef).BodyID] = value;
				}
				else
				{
					sharedRenderingInfo[base.ReportItemDef.ID] = value;
				}
			}
		}

		public ReportItemCollection ReportItemCollection
		{
			get
			{
				ReportItemCollection reportItemCollection = m_reportItemCollection;
				if (m_reportItemCollection == null)
				{
					RenderingContext renderingContext = (!base.RenderingContext.InPageSection) ? base.RenderingContext : new RenderingContext(base.RenderingContext, base.UniqueName);
					Microsoft.ReportingServices.ReportProcessing.ReportItemCollection reportItems;
					if (base.ReportItemDef is Microsoft.ReportingServices.ReportProcessing.Report)
					{
						reportItems = ((Microsoft.ReportingServices.ReportProcessing.Report)base.ReportItemDef).ReportItems;
					}
					else
					{
						Global.Tracer.Assert(base.ReportItemDef is Microsoft.ReportingServices.ReportProcessing.Rectangle);
						reportItems = ((Microsoft.ReportingServices.ReportProcessing.Rectangle)base.ReportItemDef).ReportItems;
					}
					ReportItemColInstance reportItemColInstance = null;
					if (base.ReportItemInstance != null)
					{
						if (base.ReportItemDef is Microsoft.ReportingServices.ReportProcessing.Report)
						{
							reportItemColInstance = ((ReportInstance)base.ReportItemInstance).ReportItemColInstance;
						}
						else
						{
							Global.Tracer.Assert(base.ReportItemDef is Microsoft.ReportingServices.ReportProcessing.Rectangle);
							reportItemColInstance = ((RectangleInstance)base.ReportItemInstance).ReportItemColInstance;
						}
					}
					reportItemCollection = new ReportItemCollection(reportItems, reportItemColInstance, renderingContext, m_childrenNonComputedUniqueNames);
					if (base.RenderingContext.CacheState)
					{
						m_reportItemCollection = reportItemCollection;
					}
				}
				return reportItemCollection;
			}
		}

		public bool PageBreakAtEnd
		{
			get
			{
				if (base.ReportItemDef is Microsoft.ReportingServices.ReportProcessing.Rectangle)
				{
					return ((Microsoft.ReportingServices.ReportProcessing.Rectangle)base.ReportItemDef).PageBreakAtEnd;
				}
				return false;
			}
		}

		public bool PageBreakAtStart
		{
			get
			{
				if (base.ReportItemDef is Microsoft.ReportingServices.ReportProcessing.Rectangle)
				{
					return ((Microsoft.ReportingServices.ReportProcessing.Rectangle)base.ReportItemDef).PageBreakAtStart;
				}
				return false;
			}
		}

		public override int LinkToChild
		{
			get
			{
				if (base.ReportItemDef is Microsoft.ReportingServices.ReportProcessing.Rectangle)
				{
					return ((Microsoft.ReportingServices.ReportProcessing.Rectangle)base.ReportItemDef).LinkToChild;
				}
				return -1;
			}
		}

		internal Rectangle(string uniqueName, int intUniqueName, Microsoft.ReportingServices.ReportProcessing.ReportItem reportItemDef, ReportItemInstance reportItemInstance, RenderingContext renderingContext, NonComputedUniqueNames[] childrenNonComputedUniqueNames)
			: base(uniqueName, intUniqueName, reportItemDef, reportItemInstance, renderingContext)
		{
			m_childrenNonComputedUniqueNames = childrenNonComputedUniqueNames;
		}

		internal override bool Search(SearchContext searchContext)
		{
			if (base.SkipSearch)
			{
				return false;
			}
			return ReportItemCollection?.Search(searchContext) ?? false;
		}
	}
}
