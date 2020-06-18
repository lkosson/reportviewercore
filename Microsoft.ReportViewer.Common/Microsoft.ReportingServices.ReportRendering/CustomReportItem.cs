using Microsoft.ReportingServices.ReportProcessing;
using System;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class CustomReportItem : ReportItem
	{
		private ReportItem m_altReportItem;

		private CustomData m_customData;

		private bool m_isProcessing;

		private NonComputedUniqueNames[] m_childrenNonComputedUniqueNames;

		public string Type => ((Microsoft.ReportingServices.ReportProcessing.CustomReportItem)base.ReportItemDef).Type;

		public ReportItem AltReportItem
		{
			get
			{
				if (m_isProcessing)
				{
					return null;
				}
				ReportItem reportItem = m_altReportItem;
				if (m_altReportItem == null)
				{
					Microsoft.ReportingServices.ReportProcessing.CustomReportItem customReportItem = (Microsoft.ReportingServices.ReportProcessing.CustomReportItem)base.ReportItemDef;
					Microsoft.ReportingServices.ReportProcessing.ReportItem reportItem2 = null;
					Global.Tracer.Assert(customReportItem.RenderReportItem != null || customReportItem.AltReportItem != null);
					if (customReportItem.RenderReportItem != null && 1 == customReportItem.RenderReportItem.Count)
					{
						reportItem2 = customReportItem.RenderReportItem[0];
					}
					else if (customReportItem.AltReportItem != null && 1 == customReportItem.AltReportItem.Count)
					{
						Global.Tracer.Assert(customReportItem.RenderReportItem == null);
						reportItem2 = customReportItem.AltReportItem[0];
					}
					if (reportItem2 != null)
					{
						ReportItemInstance reportItemInstance = null;
						NonComputedUniqueNames[] childrenNonComputedUniqueNames = m_childrenNonComputedUniqueNames;
						if (base.ReportItemInstance != null)
						{
							CustomReportItemInstance customReportItemInstance = (CustomReportItemInstance)base.ReportItemInstance;
							Global.Tracer.Assert(customReportItemInstance != null);
							if (customReportItemInstance.AltReportItemColInstance != null)
							{
								if (customReportItemInstance.AltReportItemColInstance.ReportItemInstances != null && 0 < customReportItemInstance.AltReportItemColInstance.ReportItemInstances.Count)
								{
									reportItemInstance = customReportItemInstance.AltReportItemColInstance[0];
								}
								else
								{
									if (customReportItemInstance.AltReportItemColInstance.ChildrenNonComputedUniqueNames != null)
									{
										childrenNonComputedUniqueNames = customReportItemInstance.AltReportItemColInstance.ChildrenNonComputedUniqueNames;
									}
									if (childrenNonComputedUniqueNames == null)
									{
										childrenNonComputedUniqueNames = customReportItemInstance.AltReportItemColInstance.GetInstanceInfo(RenderingContext.ChunkManager, RenderingContext.InPageSection).ChildrenNonComputedUniqueNames;
									}
								}
							}
						}
						reportItem = ReportItem.CreateItem(0, reportItem2, reportItemInstance, RenderingContext, (childrenNonComputedUniqueNames == null) ? null : childrenNonComputedUniqueNames[0]);
						if (base.UseCache)
						{
							m_altReportItem = reportItem;
						}
					}
				}
				return reportItem;
			}
		}

		public CustomData CustomData
		{
			get
			{
				CustomData customData = m_customData;
				if (m_customData == null)
				{
					customData = new CustomData(this);
					if (base.UseCache)
					{
						m_customData = customData;
					}
				}
				return customData;
			}
		}

		public override bool Hidden
		{
			get
			{
				if (!m_isProcessing)
				{
					return base.Hidden;
				}
				if (base.ReportItemDef.Visibility == null)
				{
					return false;
				}
				return base.InstanceInfo.StartHidden;
			}
		}

		internal new TextBox ToggleParent
		{
			get
			{
				if (!m_isProcessing)
				{
					return base.ToggleParent;
				}
				return null;
			}
		}

		public new bool IsToggleChild
		{
			get
			{
				if (!m_isProcessing)
				{
					return base.IsToggleChild;
				}
				return false;
			}
		}

		public override object SharedRenderingInfo
		{
			get
			{
				if (!m_isProcessing)
				{
					return base.SharedRenderingInfo;
				}
				return null;
			}
			set
			{
				if (!m_isProcessing)
				{
					base.SharedRenderingInfo = value;
					return;
				}
				throw new NotSupportedException();
			}
		}

		public new object RenderingInfo
		{
			get
			{
				if (!m_isProcessing)
				{
					return base.RenderingInfo;
				}
				return null;
			}
			set
			{
				if (!m_isProcessing)
				{
					base.RenderingInfo = value;
					return;
				}
				throw new NotSupportedException();
			}
		}

		internal Microsoft.ReportingServices.ReportProcessing.CustomReportItem CriDefinition => base.ReportItemDef as Microsoft.ReportingServices.ReportProcessing.CustomReportItem;

		internal CustomReportItemInstance CriInstance => base.ReportItemInstance as CustomReportItemInstance;

		internal new RenderingContext RenderingContext
		{
			get
			{
				if (m_isProcessing)
				{
					return null;
				}
				return base.RenderingContext;
			}
		}

		internal CustomReportItem(Microsoft.ReportingServices.ReportProcessing.CustomReportItem criDef, CustomReportItemInstance criInstance, CustomReportItemInstanceInfo instanceInfo)
			: base(criDef, criInstance, instanceInfo)
		{
			m_isProcessing = true;
		}

		internal CustomReportItem(string uniqueName, int intUniqueName, Microsoft.ReportingServices.ReportProcessing.ReportItem reportItemDef, ReportItemInstance reportItemInstance, RenderingContext renderingContext, NonComputedUniqueNames[] childrenNonComputedUniqueNames)
			: base(uniqueName, intUniqueName, reportItemDef, reportItemInstance, renderingContext)
		{
			m_isProcessing = false;
			m_childrenNonComputedUniqueNames = childrenNonComputedUniqueNames;
		}
	}
}
