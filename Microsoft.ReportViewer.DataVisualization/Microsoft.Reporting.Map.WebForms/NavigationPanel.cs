using System;
using System.ComponentModel;
using System.Drawing;

namespace Microsoft.Reporting.Map.WebForms
{
	[TypeConverter(typeof(DockablePanelConverter))]
	internal class NavigationPanel : DockablePanel, IToolTipProvider
	{
		private const int GirdPadding = 4;

		private const int CenterButtonCorrection = 2;

		internal PanelButton buttonNorth;

		internal PanelButton buttonSouth;

		internal PanelButton buttonEast;

		internal PanelButton buttonWest;

		private PanelButton buttonCenter;

		private Color symbolColor = Color.LightGray;

		private Color symbolBorderColor = Color.DimGray;

		private Color buttonColor = Color.White;

		private Color buttonBorderColor = Color.DarkGray;

		private NavigationPanelStyle style;

		private double scrollStep = 10.0;

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeNavigationPanel_SymbolColor")]
		[DefaultValue(typeof(Color), "LightGray")]
		public Color SymbolColor
		{
			get
			{
				return symbolColor;
			}
			set
			{
				symbolColor = value;
				ApplyColors();
				Invalidate();
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeNavigationPanel_SymbolBorderColor")]
		[DefaultValue(typeof(Color), "DimGray")]
		public Color SymbolBorderColor
		{
			get
			{
				return symbolBorderColor;
			}
			set
			{
				symbolBorderColor = value;
				ApplyColors();
				Invalidate();
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeNavigationPanel_ButtonColor")]
		[DefaultValue(typeof(Color), "White")]
		public Color ButtonColor
		{
			get
			{
				return buttonColor;
			}
			set
			{
				buttonColor = value;
				ApplyColors();
				Invalidate();
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeNavigationPanel_ButtonBorderColor")]
		[DefaultValue(typeof(Color), "DarkGray")]
		public Color ButtonBorderColor
		{
			get
			{
				return buttonBorderColor;
			}
			set
			{
				buttonBorderColor = value;
				ApplyColors();
				Invalidate();
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeNavigationPanel_PanelStyle")]
		[DefaultValue(NavigationPanelStyle.RectangularButtons)]
		public NavigationPanelStyle PanelStyle
		{
			get
			{
				return style;
			}
			set
			{
				if (style != value)
				{
					style = value;
					ApplyStyle();
					Invalidate();
				}
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeNavigationPanel_ScrollStep")]
		[DefaultValue(10.0)]
		public double ScrollStep
		{
			get
			{
				return scrollStep;
			}
			set
			{
				if (scrollStep != value)
				{
					scrollStep = value;
					Invalidate();
				}
			}
		}

		internal bool CenterButtonVisible => false;

		public NavigationPanel()
			: this(null)
		{
		}

		internal NavigationPanel(CommonElements common)
			: base(common)
		{
			Name = "NavigationPanel";
			buttonNorth = new PanelButton(this, PanelButtonType.NavigationButton, NavigationButtonClickHandler);
			buttonSouth = new PanelButton(this, PanelButtonType.NaviagateSouth, NavigationButtonClickHandler);
			buttonEast = new PanelButton(this, PanelButtonType.NaviagateEast, NavigationButtonClickHandler);
			buttonWest = new PanelButton(this, PanelButtonType.NaviagateWest, NavigationButtonClickHandler);
			buttonCenter = new PanelButton(this, PanelButtonType.NaviagateCenter, NavigationButtonClickHandler);
			ApplyStyle();
			ApplyColors();
		}

		internal override void Render(MapGraphics g)
		{
			base.Render(g);
			RenderButton(g, buttonNorth);
			RenderButton(g, buttonSouth);
			RenderButton(g, buttonWest);
			RenderButton(g, buttonEast);
		}

		internal override object GetDefaultPropertyValue(string prop, object currentValue)
		{
			object obj = null;
			switch (prop)
			{
			case "Margins":
				return new PanelMargins(5, 5, 5, 5);
			case "Size":
				return new MapSize(null, 90f, 90f);
			case "Dock":
				return PanelDockStyle.Left;
			default:
				return base.GetDefaultPropertyValue(prop, currentValue);
			}
		}

		private void RenderButton(MapGraphics g, PanelButton button)
		{
			float num = g.GetAbsoluteDimension(100f) - 1f;
			SizeF absoluteSize = g.GetAbsoluteSize(new SizeF(100f, 100f));
			absoluteSize.Width -= 1f;
			absoluteSize.Height -= 1f;
			SizeF sizeF = new SizeF((absoluteSize.Width - num) / 2f, (absoluteSize.Height - num) / 2f);
			float num2 = num / 3f;
			PointF empty = PointF.Empty;
			switch (button.Type)
			{
			case PanelButtonType.NavigationButton:
				empty = new PointF(num / 2f, num2 / 2f);
				if (!CenterButtonVisible)
				{
					empty.Y += 2f;
				}
				break;
			case PanelButtonType.NaviagateSouth:
				empty = new PointF(num / 2f, num - num2 / 2f);
				if (!CenterButtonVisible)
				{
					empty.Y -= 2f;
				}
				break;
			case PanelButtonType.NaviagateEast:
				empty = new PointF(num - num2 / 2f, num / 2f);
				if (!CenterButtonVisible)
				{
					empty.X -= 2f;
				}
				break;
			case PanelButtonType.NaviagateWest:
				empty = new PointF(num2 / 2f, num / 2f);
				if (!CenterButtonVisible)
				{
					empty.X += 2f;
				}
				break;
			case PanelButtonType.NaviagateCenter:
				empty = new PointF(num / 2f, num / 2f);
				break;
			default:
				throw new ArgumentException(SR.invalid_button_type);
			}
			num2 -= 4f;
			RectangleF absolute = new RectangleF(sizeF.Width + empty.X - num2 / 2f, sizeF.Height + empty.Y - num2 / 2f, num2, num2);
			button.Bounds = g.GetRelativeRectangle(absolute);
			button.Render(g);
		}

		private void NavigationButtonClickHandler(object sender, EventArgs e)
		{
			PanelButton panelButton = (PanelButton)sender;
			MapCore mapCore = GetMapCore();
			if (mapCore != null)
			{
				switch (panelButton.Type)
				{
				case (PanelButtonType)35:
					break;
				case PanelButtonType.NavigationButton:
					mapCore.Scroll(ScrollDirection.North, ScrollStep);
					break;
				case PanelButtonType.NaviagateSouth:
					mapCore.Scroll(ScrollDirection.South, ScrollStep);
					break;
				case PanelButtonType.NaviagateEast:
					mapCore.Scroll(ScrollDirection.East, ScrollStep);
					break;
				case PanelButtonType.NaviagateWest:
					mapCore.Scroll(ScrollDirection.West, ScrollStep);
					break;
				}
			}
		}

		private void ApplyStyle()
		{
			switch (PanelStyle)
			{
			case NavigationPanelStyle.RectangularButtons:
				buttonNorth.Style = PanelButtonStyle.RoundedRectangle;
				buttonSouth.Style = PanelButtonStyle.RoundedRectangle;
				buttonEast.Style = PanelButtonStyle.RoundedRectangle;
				buttonWest.Style = PanelButtonStyle.RoundedRectangle;
				buttonCenter.Style = PanelButtonStyle.RoundedRectangle;
				break;
			case NavigationPanelStyle.TriangularButtons:
				buttonNorth.Style = PanelButtonStyle.Triangle;
				buttonSouth.Style = PanelButtonStyle.Triangle;
				buttonEast.Style = PanelButtonStyle.Triangle;
				buttonWest.Style = PanelButtonStyle.Triangle;
				buttonCenter.Style = PanelButtonStyle.Circle;
				break;
			}
		}

		private void ApplyColors()
		{
			buttonNorth.BackColor = ButtonColor;
			buttonNorth.SymbolColor = SymbolColor;
			buttonNorth.SymbolBorderColor = SymbolBorderColor;
			buttonNorth.BorderColor = ButtonBorderColor;
			buttonSouth.BackColor = ButtonColor;
			buttonSouth.SymbolColor = SymbolColor;
			buttonSouth.SymbolBorderColor = SymbolBorderColor;
			buttonSouth.BorderColor = ButtonBorderColor;
			buttonEast.BackColor = ButtonColor;
			buttonEast.SymbolColor = SymbolColor;
			buttonEast.SymbolBorderColor = SymbolBorderColor;
			buttonEast.BorderColor = ButtonBorderColor;
			buttonWest.BackColor = ButtonColor;
			buttonWest.SymbolColor = SymbolColor;
			buttonWest.SymbolBorderColor = SymbolBorderColor;
			buttonWest.BorderColor = ButtonBorderColor;
			buttonCenter.BackColor = ButtonColor;
			buttonCenter.SymbolColor = SymbolColor;
			buttonCenter.SymbolBorderColor = SymbolBorderColor;
			buttonCenter.BorderColor = ButtonBorderColor;
		}
	}
}
