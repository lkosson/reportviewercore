using System.ComponentModel;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class DockablePanel : Panel
	{
		private PanelDockStyle dockStyle;

		private DockAlignment dockAlignment;

		private bool dockedInsideViewport;

		public override bool Visible
		{
			get
			{
				return base.Visible;
			}
			set
			{
				if (base.Visible != value)
				{
					base.Visible = value;
					InvalidateAndLayout();
				}
			}
		}

		[SRCategory("CategoryAttribute_Layout")]
		[SRDescription("DescriptionAttributeDockablePanel_Dock")]
		[NotifyParentProperty(true)]
		[RefreshProperties(RefreshProperties.All)]
		public virtual PanelDockStyle Dock
		{
			get
			{
				return dockStyle;
			}
			set
			{
				if (dockStyle != value)
				{
					dockStyle = value;
					Location.Docked = (dockStyle != PanelDockStyle.None);
					InvalidateAndLayout();
				}
			}
		}

		[SRCategory("CategoryAttribute_Layout")]
		[SRDescription("DescriptionAttributeDockablePanel_DockAlignment")]
		[NotifyParentProperty(true)]
		public virtual DockAlignment DockAlignment
		{
			get
			{
				return dockAlignment;
			}
			set
			{
				if (dockAlignment != value)
				{
					dockAlignment = value;
					InvalidateAndLayout();
				}
			}
		}

		[SRCategory("CategoryAttribute_Layout")]
		[SRDescription("DescriptionAttributeDockablePanel_DockedInsideViewport")]
		[NotifyParentProperty(true)]
		public bool DockedInsideViewport
		{
			get
			{
				return dockedInsideViewport;
			}
			set
			{
				if (dockedInsideViewport != value)
				{
					dockedInsideViewport = value;
					InvalidateAndLayout();
				}
			}
		}

		protected void ResetDock()
		{
			Dock = (PanelDockStyle)GetDefaultPropertyValue("Dock", Dock);
		}

		protected bool ShouldSerializeDock()
		{
			return !Dock.Equals(GetDefaultPropertyValue("Dock", Dock));
		}

		protected void ResetDockAlignment()
		{
			DockAlignment = (DockAlignment)GetDefaultPropertyValue("DockAlignment", DockAlignment);
		}

		protected bool ShouldSerializeDockAlignment()
		{
			return !DockAlignment.Equals(GetDefaultPropertyValue("DockAlignment", DockAlignment));
		}

		protected void ResetDockedInsideViewport()
		{
			DockedInsideViewport = (bool)GetDefaultPropertyValue("DockedInsideViewport", DockedInsideViewport);
		}

		protected bool ShouldSerializeDockedInsideViewport()
		{
			return !DockedInsideViewport.Equals(GetDefaultPropertyValue("DockedInsideViewport", DockedInsideViewport));
		}

		public DockablePanel()
			: this(null)
		{
		}

		internal DockablePanel(CommonElements common)
			: base(common)
		{
			Dock = (PanelDockStyle)GetDefaultPropertyValue("Dock", null);
			DockAlignment = (DockAlignment)GetDefaultPropertyValue("DockAlignment", null);
			DockedInsideViewport = (bool)GetDefaultPropertyValue("DockedInsideViewport", null);
		}

		internal override void SizeLocationChanged(SizeLocationChangeInfo info)
		{
			base.SizeLocationChanged(info);
			switch (info)
			{
			case SizeLocationChangeInfo.Location:
				Location.Docked = (Dock != PanelDockStyle.None);
				break;
			case SizeLocationChangeInfo.LocationUnit:
			case SizeLocationChangeInfo.Size:
			case SizeLocationChangeInfo.SizeUnit:
			case SizeLocationChangeInfo.ZOrder:
				InvalidateAndLayout();
				break;
			}
		}

		internal override object GetDefaultPropertyValue(string prop, object currentValue)
		{
			object obj = null;
			switch (prop)
			{
			case "Dock":
				return PanelDockStyle.None;
			case "DockAlignment":
				return DockAlignment.Near;
			case "DockedInsideViewport":
				return true;
			default:
				return base.GetDefaultPropertyValue(prop, currentValue);
			}
		}

		internal override bool IsVisible()
		{
			MapCore mapCore = GetMapCore();
			bool flag = true;
			if (mapCore != null && DockedInsideViewport)
			{
				flag = mapCore.Viewport.Visible;
			}
			return base.IsVisible() && flag;
		}
	}
}
