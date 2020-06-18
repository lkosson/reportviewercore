using System;
using System.ComponentModel;
using System.Drawing;

namespace Microsoft.Reporting.Map.WebForms
{
	internal abstract class AutoSizePanel : DockablePanel
	{
		private bool autoSize = true;

		private float maximumPanelAutoSize = 100f;

		[SRCategory("CategoryAttribute_Layout")]
		[SRDescription("DescriptionAttributeAutoSizePanel_AutoSize")]
		[NotifyParentProperty(true)]
		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue(true)]
		public bool AutoSize
		{
			get
			{
				return autoSize;
			}
			set
			{
				if (autoSize != value)
				{
					autoSize = value;
					Size.AutoSize = value;
					SizeLocationChanged(SizeLocationChangeInfo.Size);
				}
			}
		}

		public override MapSize Size
		{
			get
			{
				return base.Size;
			}
			set
			{
				if (Size != value)
				{
					value.AutoSize = AutoSize;
					base.Size = value;
				}
			}
		}

		public override int BorderWidth
		{
			get
			{
				return base.BorderWidth;
			}
			set
			{
				if (base.BorderWidth != value)
				{
					base.BorderWidth = value;
					if (AutoSize)
					{
						InvalidateAndLayout();
					}
				}
			}
		}

		internal abstract bool IsEmpty
		{
			get;
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Layout")]
		[DefaultValue(100f)]
		[SRDescription("DescriptionAttributeAutoSizePanel_MaxAutoSize")]
		public virtual float MaxAutoSize
		{
			get
			{
				return maximumPanelAutoSize;
			}
			set
			{
				if (value < 0f)
				{
					throw new ArgumentOutOfRangeException("MaxAutoSize", SR.ExceptionMaximumLegendAutoSize);
				}
				maximumPanelAutoSize = value;
				SizeLocationChanged(SizeLocationChangeInfo.Size);
			}
		}

		public AutoSizePanel()
			: this(null)
		{
		}

		internal AutoSizePanel(CommonElements common)
			: base(common)
		{
		}

		internal abstract SizeF GetOptimalSize(MapGraphics g, SizeF maxSizeAbs);

		internal void AdjustAutoSize(MapGraphics g)
		{
			if (!AutoSize || Common == null)
			{
				return;
			}
			SizeF empty = SizeF.Empty;
			if (!base.DockedInsideViewport)
			{
				empty = new SizeF(Common.MapCore.Width, Common.MapCore.Height);
			}
			else
			{
				empty = Common.MapCore.Viewport.GetSizeInPixels();
				empty.Width -= Common.MapCore.Viewport.Margins.Left + Common.MapCore.Viewport.Margins.Right;
				empty.Height -= Common.MapCore.Viewport.Margins.Top + Common.MapCore.Viewport.Margins.Bottom;
			}
			empty.Width -= base.Margins.Right + base.Margins.Left;
			empty.Height -= base.Margins.Top + base.Margins.Bottom;
			if (MaxAutoSize > 0f && MaxAutoSize < 100f)
			{
				switch (Dock)
				{
				case PanelDockStyle.Top:
				case PanelDockStyle.Bottom:
					empty.Height = empty.Height * MaxAutoSize / 100f;
					break;
				case PanelDockStyle.Left:
				case PanelDockStyle.Right:
					empty.Width = empty.Width * MaxAutoSize / 100f;
					break;
				case PanelDockStyle.None:
					empty = CalculateUndockedAutoSize(empty);
					break;
				}
			}
			if ((double)empty.Width <= 0.1 || (double)empty.Height <= 0.1)
			{
				SetSizeInPixels(new SizeF(0.1f, 0.1f));
				return;
			}
			SizeF optimalSize = GetOptimalSize(g, empty);
			if (!float.IsNaN(optimalSize.Height) && !float.IsNaN(optimalSize.Width))
			{
				SizeF sizeInPixels = GetSizeInPixels();
				optimalSize.Width += base.Margins.Left + base.Margins.Right;
				optimalSize.Height += base.Margins.Top + base.Margins.Bottom;
				if (sizeInPixels.Width != optimalSize.Width || sizeInPixels.Height != optimalSize.Height)
				{
					SetSizeInPixels(optimalSize);
				}
			}
		}

		protected virtual SizeF CalculateUndockedAutoSize(SizeF size)
		{
			if (size.Width < size.Height)
			{
				size.Height = size.Height * MaxAutoSize / 100f;
			}
			else
			{
				size.Width = size.Width * MaxAutoSize / 100f;
			}
			return size;
		}

		internal override bool IsVisible()
		{
			bool flag = base.Visible;
			if (Common != null && !Common.MapControl.IsDesignMode())
			{
				flag &= !IsEmpty;
			}
			return flag;
		}

		internal override void Invalidate()
		{
			Invalidate(layout: false);
		}

		protected void Invalidate(bool layout)
		{
			if (layout && AutoSize)
			{
				base.InvalidateAndLayout();
			}
			else
			{
				base.Invalidate();
			}
		}

		internal override bool IsRenderVisible(MapGraphics g, RectangleF clipRect)
		{
			if (base.IsRenderVisible(g, clipRect))
			{
				return !IsEmpty;
			}
			return false;
		}
	}
}
