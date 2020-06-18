using Microsoft.Reporting.Chart.WebForms.Design;
using Microsoft.Reporting.Chart.WebForms.Utilities;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Microsoft.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeTitle5")]
	internal class Title : IMapAreaAttributes
	{
		private Chart chart;

		internal int titleBorderSpacing = 4;

		private object mapAreaTag;

		private string name = "Chart Title";

		private string text = string.Empty;

		private TextStyle style;

		private ElementPosition position = new ElementPosition();

		private bool visible = true;

		private Color backColor = Color.Empty;

		private ChartHatchStyle backHatchStyle;

		private string backImage = "";

		private ChartImageWrapMode backImageMode;

		private Color backImageTranspColor = Color.Empty;

		private ChartImageAlign backImageAlign;

		private GradientType backGradientType;

		private Color backGradientEndColor = Color.Empty;

		private int shadowOffset;

		private Color shadowColor = Color.FromArgb(128, 0, 0, 0);

		private Color borderColor = Color.Empty;

		private int borderWidth = 1;

		private ChartDashStyle borderStyle = ChartDashStyle.Solid;

		private Font font = new Font(ChartPicture.GetDefaultFontFamilyName(), 8f);

		private Color color = Color.Black;

		private ContentAlignment alignment = ContentAlignment.MiddleCenter;

		private Docking docking;

		private string dockToChartArea = "NotSet";

		private bool dockInsideChartArea = true;

		private int dockOffset;

		private string toolTip = string.Empty;

		private string href = string.Empty;

		private string mapAreaAttributes = string.Empty;

		private TextOrientation textOrientation;

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(TextOrientation.Auto)]
		[SRDescription("DescriptionAttribute_TextOrientation")]
		[NotifyParentProperty(true)]
		public TextOrientation TextOrientation
		{
			get
			{
				return textOrientation;
			}
			set
			{
				textOrientation = value;
				Invalidate(invalidateTitleOnly: true);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeTitle_Visible")]
		[ParenthesizePropertyName(true)]
		public virtual bool Visible
		{
			get
			{
				return visible;
			}
			set
			{
				visible = value;
				Invalidate(invalidateTitleOnly: false);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[SRCategory("CategoryAttributeMisc")]
		[SRDescription("DescriptionAttributeTitle_Name")]
		[DefaultValue("Chart Title")]
		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				if (name != value && !(name == "Default Title") && !(value == "Chart Title"))
				{
					if (value == null || value.Length == 0)
					{
						throw new ArgumentException(SR.ExceptionTitleNameIsEmpty);
					}
					if (Chart != null && Chart.Titles.IndexOf(value) != -1)
					{
						throw new ArgumentException(SR.ExceptionTitleNameIsNotUnique(value));
					}
					name = value;
				}
			}
		}

		[SRCategory("CategoryAttributeDocking")]
		[Bindable(true)]
		[DefaultValue("NotSet")]
		[SRDescription("DescriptionAttributeTitle_DockToChartArea")]
		[TypeConverter(typeof(LegendAreaNameConverter))]
		[NotifyParentProperty(true)]
		public string DockToChartArea
		{
			get
			{
				return dockToChartArea;
			}
			set
			{
				if (value != dockToChartArea)
				{
					if (value.Length == 0)
					{
						dockToChartArea = "NotSet";
					}
					else
					{
						dockToChartArea = value;
					}
					Invalidate(invalidateTitleOnly: false);
				}
			}
		}

		[SRCategory("CategoryAttributeDocking")]
		[Bindable(true)]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeTitle_DockInsideChartArea")]
		[NotifyParentProperty(true)]
		public bool DockInsideChartArea
		{
			get
			{
				return dockInsideChartArea;
			}
			set
			{
				if (value != dockInsideChartArea)
				{
					dockInsideChartArea = value;
					Invalidate(invalidateTitleOnly: false);
				}
			}
		}

		[SRCategory("CategoryAttributeDocking")]
		[Bindable(true)]
		[DefaultValue(0)]
		[SRDescription("DescriptionAttributeTitle_DockOffset")]
		[NotifyParentProperty(true)]
		public int DockOffset
		{
			get
			{
				return dockOffset;
			}
			set
			{
				if (value != dockOffset)
				{
					dockOffset = value;
					Invalidate(invalidateTitleOnly: false);
				}
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeTitle_Position")]
		[DefaultValue(typeof(ElementPosition), "Auto")]
		[NotifyParentProperty(true)]
		[TypeConverter(typeof(ElementPositionConverter))]
		[SerializationVisibility(SerializationVisibility.Element)]
		public ElementPosition Position
		{
			get
			{
				if (chart != null && chart.serializationStatus == SerializationStatus.Saving)
				{
					if (position.Auto)
					{
						return new ElementPosition();
					}
					ElementPosition elementPosition = new ElementPosition();
					elementPosition.Auto = true;
					elementPosition.SetPositionNoAuto(position.X, position.Y, position.Width, position.Height);
					return elementPosition;
				}
				return position;
			}
			set
			{
				position = value;
				Invalidate(invalidateTitleOnly: false);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeTitle_Text")]
		[NotifyParentProperty(true)]
		[ParenthesizePropertyName(true)]
		public string Text
		{
			get
			{
				return text;
			}
			set
			{
				text = ((value == null) ? string.Empty : value);
				Invalidate(invalidateTitleOnly: false);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(TextStyle.Default)]
		[SRDescription("DescriptionAttributeTitle_Style")]
		[NotifyParentProperty(true)]
		public TextStyle Style
		{
			get
			{
				return style;
			}
			set
			{
				style = value;
				Invalidate(invalidateTitleOnly: true);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeTitle_BackColor")]
		[NotifyParentProperty(true)]
		public Color BackColor
		{
			get
			{
				return backColor;
			}
			set
			{
				backColor = value;
				Invalidate(invalidateTitleOnly: true);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeTitle_BorderColor")]
		[NotifyParentProperty(true)]
		public Color BorderColor
		{
			get
			{
				return borderColor;
			}
			set
			{
				borderColor = value;
				Invalidate(invalidateTitleOnly: true);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(ChartDashStyle.Solid)]
		[SRDescription("DescriptionAttributeTitle_BorderStyle")]
		[NotifyParentProperty(true)]
		public ChartDashStyle BorderStyle
		{
			get
			{
				return borderStyle;
			}
			set
			{
				borderStyle = value;
				Invalidate(invalidateTitleOnly: true);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(1)]
		[SRDescription("DescriptionAttributeTitle_BorderWidth")]
		[NotifyParentProperty(true)]
		public int BorderWidth
		{
			get
			{
				return borderWidth;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("value", SR.ExceptionTitleBorderWidthIsNegative);
				}
				borderWidth = value;
				Invalidate(invalidateTitleOnly: false);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeBackImage17")]
		[NotifyParentProperty(true)]
		public string BackImage
		{
			get
			{
				return backImage;
			}
			set
			{
				backImage = value;
				Invalidate(invalidateTitleOnly: true);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(ChartImageWrapMode.Tile)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeTitle_BackImageMode")]
		public ChartImageWrapMode BackImageMode
		{
			get
			{
				return backImageMode;
			}
			set
			{
				backImageMode = value;
				Invalidate(invalidateTitleOnly: true);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "")]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeTitle_BackImageTransparentColor")]
		public Color BackImageTransparentColor
		{
			get
			{
				return backImageTranspColor;
			}
			set
			{
				backImageTranspColor = value;
				Invalidate(invalidateTitleOnly: true);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(ChartImageAlign.TopLeft)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeBackImageAlign")]
		public ChartImageAlign BackImageAlign
		{
			get
			{
				return backImageAlign;
			}
			set
			{
				backImageAlign = value;
				Invalidate(invalidateTitleOnly: true);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(GradientType.None)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeTitle_BackGradientType")]
		public GradientType BackGradientType
		{
			get
			{
				return backGradientType;
			}
			set
			{
				backGradientType = value;
				Invalidate(invalidateTitleOnly: true);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "")]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeTitle_BackGradientEndColor")]
		public Color BackGradientEndColor
		{
			get
			{
				return backGradientEndColor;
			}
			set
			{
				backGradientEndColor = value;
				Invalidate(invalidateTitleOnly: true);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(ChartHatchStyle.None)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeTitle_BackHatchStyle")]
		public ChartHatchStyle BackHatchStyle
		{
			get
			{
				return backHatchStyle;
			}
			set
			{
				backHatchStyle = value;
				Invalidate(invalidateTitleOnly: true);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 8pt")]
		[SRDescription("DescriptionAttributeTitle_Font")]
		[NotifyParentProperty(true)]
		public Font Font
		{
			get
			{
				return font;
			}
			set
			{
				font = value;
				Invalidate(invalidateTitleOnly: false);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "Black")]
		[SRDescription("DescriptionAttributeTitle_Color")]
		[NotifyParentProperty(true)]
		public Color Color
		{
			get
			{
				return color;
			}
			set
			{
				color = value;
				Invalidate(invalidateTitleOnly: true);
			}
		}

		[SRCategory("CategoryAttributeDocking")]
		[Bindable(true)]
		[DefaultValue(ContentAlignment.MiddleCenter)]
		[SRDescription("DescriptionAttributeTitle_Alignment")]
		[NotifyParentProperty(true)]
		public ContentAlignment Alignment
		{
			get
			{
				return alignment;
			}
			set
			{
				alignment = value;
				Invalidate(invalidateTitleOnly: false);
			}
		}

		[SRCategory("CategoryAttributeDocking")]
		[Bindable(true)]
		[DefaultValue(Docking.Top)]
		[SRDescription("DescriptionAttributeTitle_Docking")]
		[NotifyParentProperty(true)]
		public Docking Docking
		{
			get
			{
				return docking;
			}
			set
			{
				docking = value;
				Invalidate(invalidateTitleOnly: false);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(0)]
		[SRDescription("DescriptionAttributeTitle_ShadowOffset")]
		[NotifyParentProperty(true)]
		public int ShadowOffset
		{
			get
			{
				return shadowOffset;
			}
			set
			{
				shadowOffset = value;
				Invalidate(invalidateTitleOnly: false);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "128, 0, 0, 0")]
		[SRDescription("DescriptionAttributeTitle_ShadowColor")]
		[NotifyParentProperty(true)]
		public Color ShadowColor
		{
			get
			{
				return shadowColor;
			}
			set
			{
				shadowColor = value;
				Invalidate(invalidateTitleOnly: false);
			}
		}

		[SRCategory("CategoryAttributeMapArea")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeTitle_ToolTip")]
		[DefaultValue("")]
		public string ToolTip
		{
			get
			{
				return toolTip;
			}
			set
			{
				toolTip = value;
			}
		}

		[SRCategory("CategoryAttributeMapArea")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeTitle_Href")]
		[DefaultValue("")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string Href
		{
			get
			{
				return href;
			}
			set
			{
				href = value;
			}
		}

		[SRCategory("CategoryAttributeMapArea")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeTitle_MapAreaAttributes")]
		[DefaultValue("")]
		public string MapAreaAttributes
		{
			get
			{
				return mapAreaAttributes;
			}
			set
			{
				mapAreaAttributes = value;
			}
		}

		object IMapAreaAttributes.Tag
		{
			get
			{
				return mapAreaTag;
			}
			set
			{
				mapAreaTag = value;
			}
		}

		internal bool BackGroundIsVisible
		{
			get
			{
				if (!BackColor.IsEmpty || BackImage.Length > 0 || (!BorderColor.IsEmpty && BorderStyle != 0))
				{
					return true;
				}
				return false;
			}
		}

		internal Chart Chart
		{
			get
			{
				return chart;
			}
			set
			{
				chart = value;
				position.chart = value;
			}
		}

		private bool IsTextVertical
		{
			get
			{
				TextOrientation textOrientation = GetTextOrientation();
				if (textOrientation != TextOrientation.Rotated90)
				{
					return textOrientation == TextOrientation.Rotated270;
				}
				return true;
			}
		}

		public Title()
		{
			Initialize(string.Empty, Docking.Top, null, Color.Black);
		}

		public Title(string text)
		{
			Initialize(text, Docking.Top, null, Color.Black);
		}

		public Title(string text, Docking docking)
		{
			Initialize(text, docking, null, Color.Black);
		}

		public Title(string text, Docking docking, Font font, Color color)
		{
			Initialize(text, docking, font, color);
		}

		private void Initialize(string text, Docking docking, Font font, Color color)
		{
			this.text = text;
			this.docking = docking;
			this.color = color;
			if (font != null)
			{
				this.font = font;
			}
		}

		private TextOrientation GetTextOrientation()
		{
			if (TextOrientation == TextOrientation.Auto)
			{
				if (Position.Auto)
				{
					if (Docking == Docking.Left)
					{
						return TextOrientation.Rotated270;
					}
					if (Docking == Docking.Right)
					{
						return TextOrientation.Rotated90;
					}
				}
				return TextOrientation.Horizontal;
			}
			return TextOrientation;
		}

		internal bool IsVisible()
		{
			if (Visible)
			{
				if (DockToChartArea.Length > 0 && Chart != null && Chart.ChartAreas.GetIndex(DockToChartArea) >= 0 && !Chart.ChartAreas[DockToChartArea].Visible)
				{
					return false;
				}
				return true;
			}
			return false;
		}

		internal void Invalidate(bool invalidateTitleOnly)
		{
		}

		internal void Paint(ChartGraphics chartGraph)
		{
			if (!IsVisible())
			{
				return;
			}
			CommonElements common = chart.chartPicture.common;
			string text = Text;
			if (chart != null && chart.LocalizeTextHandler != null)
			{
				text = chart.LocalizeTextHandler(this, text, 0, ChartElementType.Title);
			}
			RectangleF rectangleF = Position.ToRectangleF();
			if (!Position.Auto && chart != null && chart.chartPicture != null && (rectangleF.Width == 0f || rectangleF.Height == 0f))
			{
				SizeF relative = new SizeF((rectangleF.Width == 0f) ? ((float)chart.chartPicture.Width) : rectangleF.Width, (rectangleF.Height == 0f) ? ((float)chart.chartPicture.Height) : rectangleF.Height);
				if (IsTextVertical)
				{
					float width = relative.Width;
					relative.Width = relative.Height;
					relative.Height = width;
				}
				relative = chartGraph.GetAbsoluteSize(relative);
				SizeF size = chartGraph.MeasureString(text.Replace("\\n", "\n"), Font, relative, new StringFormat(), GetTextOrientation());
				if (BackGroundIsVisible)
				{
					size.Width += titleBorderSpacing;
					size.Height += titleBorderSpacing;
				}
				if (IsTextVertical)
				{
					float width2 = size.Width;
					size.Width = size.Height;
					size.Height = width2;
				}
				size = chartGraph.GetRelativeSize(size);
				if (rectangleF.Width == 0f)
				{
					rectangleF.Width = size.Width;
					if (Alignment == ContentAlignment.BottomRight || Alignment == ContentAlignment.MiddleRight || Alignment == ContentAlignment.TopRight)
					{
						rectangleF.X -= rectangleF.Width;
					}
					else if (Alignment == ContentAlignment.BottomCenter || Alignment == ContentAlignment.MiddleCenter || Alignment == ContentAlignment.TopCenter)
					{
						rectangleF.X -= rectangleF.Width / 2f;
					}
				}
				if (rectangleF.Height == 0f)
				{
					rectangleF.Height = size.Height;
					if (Alignment == ContentAlignment.BottomRight || Alignment == ContentAlignment.BottomCenter || Alignment == ContentAlignment.BottomLeft)
					{
						rectangleF.Y -= rectangleF.Height;
					}
					else if (Alignment == ContentAlignment.MiddleCenter || Alignment == ContentAlignment.MiddleLeft || Alignment == ContentAlignment.MiddleRight)
					{
						rectangleF.Y -= rectangleF.Height / 2f;
					}
				}
			}
			RectangleF relative2 = new RectangleF(rectangleF.Location, rectangleF.Size);
			relative2 = chartGraph.GetAbsoluteRectangle(relative2);
			if (BackGroundIsVisible && common.ProcessModePaint)
			{
				chartGraph.StartHotRegion(href, toolTip);
				chartGraph.StartAnimation();
				chartGraph.FillRectangleRel(rectangleF, BackColor, BackHatchStyle, BackImage, BackImageMode, BackImageTransparentColor, BackImageAlign, BackGradientType, BackGradientEndColor, BorderColor, BorderWidth, BorderStyle, ShadowColor, ShadowOffset, PenAlignment.Inset);
				chartGraph.StopAnimation();
				chartGraph.EndHotRegion();
			}
			else
			{
				chartGraph.StartHotRegion(href, toolTip);
				SizeF absoluteSize = chartGraph.GetAbsoluteSize(rectangleF.Size);
				SizeF size2 = chartGraph.MeasureString(text.Replace("\\n", "\n"), Font, absoluteSize, new StringFormat(), GetTextOrientation());
				size2 = chartGraph.GetRelativeSize(size2);
				RectangleF rectF = new RectangleF(rectangleF.X, rectangleF.Y, size2.Width, size2.Height);
				if (Alignment == ContentAlignment.BottomCenter || Alignment == ContentAlignment.BottomLeft || Alignment == ContentAlignment.BottomRight)
				{
					rectF.Y = rectangleF.Bottom - rectF.Height;
				}
				else if (Alignment == ContentAlignment.MiddleCenter || Alignment == ContentAlignment.MiddleLeft || Alignment == ContentAlignment.MiddleRight)
				{
					rectF.Y = rectangleF.Y + rectangleF.Height / 2f - rectF.Height / 2f;
				}
				if (Alignment == ContentAlignment.BottomRight || Alignment == ContentAlignment.MiddleRight || Alignment == ContentAlignment.TopRight)
				{
					rectF.X = rectangleF.Right - rectF.Width;
				}
				else if (Alignment == ContentAlignment.BottomCenter || Alignment == ContentAlignment.MiddleCenter || Alignment == ContentAlignment.TopCenter)
				{
					rectF.X = rectangleF.X + rectangleF.Width / 2f - rectF.Width / 2f;
				}
				if (true)
				{
					chartGraph.FillRectangleRel(rectF, Color.FromArgb(0, Color.White), ChartHatchStyle.None, string.Empty, ChartImageWrapMode.Tile, BackImageTransparentColor, BackImageAlign, GradientType.None, BackGradientEndColor, Color.Transparent, 0, BorderStyle, Color.Transparent, 0, PenAlignment.Inset);
				}
				chartGraph.EndHotRegion();
			}
			if (BackGroundIsVisible)
			{
				relative2.Width -= titleBorderSpacing;
				relative2.Height -= titleBorderSpacing;
				relative2.X += (float)titleBorderSpacing / 2f;
				relative2.Y += (float)titleBorderSpacing / 2f;
			}
			StringFormat stringFormat = new StringFormat();
			stringFormat.Alignment = StringAlignment.Center;
			stringFormat.LineAlignment = StringAlignment.Center;
			if (Alignment == ContentAlignment.BottomCenter || Alignment == ContentAlignment.BottomLeft || Alignment == ContentAlignment.BottomRight)
			{
				stringFormat.LineAlignment = StringAlignment.Far;
			}
			else if (Alignment == ContentAlignment.TopCenter || Alignment == ContentAlignment.TopLeft || Alignment == ContentAlignment.TopRight)
			{
				stringFormat.LineAlignment = StringAlignment.Near;
			}
			if (Alignment == ContentAlignment.BottomLeft || Alignment == ContentAlignment.MiddleLeft || Alignment == ContentAlignment.TopLeft)
			{
				stringFormat.Alignment = StringAlignment.Near;
			}
			else if (Alignment == ContentAlignment.BottomRight || Alignment == ContentAlignment.MiddleRight || Alignment == ContentAlignment.TopRight)
			{
				stringFormat.Alignment = StringAlignment.Far;
			}
			Color gradientColor = ChartGraphics.GetGradientColor(Color, Color.Black, 0.8);
			int num = 1;
			TextStyle textStyle = Style;
			if ((textStyle == TextStyle.Default || textStyle == TextStyle.Shadow) && !BackGroundIsVisible && ShadowOffset != 0)
			{
				textStyle = TextStyle.Shadow;
				gradientColor = ShadowColor;
				num = ShadowOffset;
			}
			text = text.Replace("\\n", "\n");
			Matrix matrix = null;
			if (IsTextVertical)
			{
				if (GetTextOrientation() == TextOrientation.Rotated270)
				{
					stringFormat.FormatFlags |= (StringFormatFlags.DirectionRightToLeft | StringFormatFlags.DirectionVertical);
					matrix = chartGraph.Transform.Clone();
					PointF empty = PointF.Empty;
					empty.X = relative2.X + relative2.Width / 2f;
					empty.Y = relative2.Y + relative2.Height / 2f;
					Matrix matrix2 = chartGraph.Transform.Clone();
					matrix2.RotateAt(180f, empty);
					chartGraph.Transform = matrix2;
				}
				else if (GetTextOrientation() == TextOrientation.Rotated90)
				{
					stringFormat.FormatFlags |= (StringFormatFlags.DirectionRightToLeft | StringFormatFlags.DirectionVertical);
				}
			}
			if (text.Length > 0)
			{
				chartGraph.StartAnimation();
				switch (textStyle)
				{
				case TextStyle.Default:
					chartGraph.StartHotRegion(href, toolTip);
					chartGraph.DrawString(text, Font, new SolidBrush(Color), relative2, stringFormat, GetTextOrientation());
					chartGraph.EndHotRegion();
					break;
				case TextStyle.Frame:
				{
					GraphicsPath graphicsPath = new GraphicsPath();
					graphicsPath.AddString(ChartGraphics.GetStackedText(text), Font.FontFamily, (int)Font.Style, Font.Size * 1.3f, relative2, stringFormat);
					graphicsPath.CloseAllFigures();
					chartGraph.StartHotRegion(href, toolTip);
					chartGraph.DrawPath(new Pen(Color, 1f), graphicsPath);
					chartGraph.EndHotRegion();
					break;
				}
				case TextStyle.Embed:
				{
					RectangleF rect3 = new RectangleF(relative2.Location, relative2.Size);
					rect3.X -= 1f;
					rect3.Y -= 1f;
					chartGraph.DrawString(text, Font, new SolidBrush(gradientColor), rect3, stringFormat, GetTextOrientation());
					rect3.X += 2f;
					rect3.Y += 2f;
					Color gradientColor3 = ChartGraphics.GetGradientColor(Color.White, Color, 0.3);
					chartGraph.DrawString(text, Font, new SolidBrush(gradientColor3), rect3, stringFormat, GetTextOrientation());
					chartGraph.StartHotRegion(href, toolTip);
					chartGraph.DrawString(text, Font, new SolidBrush(Color), relative2, stringFormat, GetTextOrientation());
					chartGraph.EndHotRegion();
					break;
				}
				case TextStyle.Emboss:
				{
					RectangleF rect2 = new RectangleF(relative2.Location, relative2.Size);
					rect2.X += 1f;
					rect2.Y += 1f;
					chartGraph.DrawString(text, Font, new SolidBrush(gradientColor), rect2, stringFormat, GetTextOrientation());
					rect2.X -= 2f;
					rect2.Y -= 2f;
					Color gradientColor2 = ChartGraphics.GetGradientColor(Color.White, Color, 0.3);
					chartGraph.DrawString(text, Font, new SolidBrush(gradientColor2), rect2, stringFormat, GetTextOrientation());
					chartGraph.StartHotRegion(href, toolTip);
					chartGraph.DrawString(text, Font, new SolidBrush(Color), relative2, stringFormat, GetTextOrientation());
					chartGraph.EndHotRegion();
					break;
				}
				case TextStyle.Shadow:
				{
					RectangleF rect = new RectangleF(relative2.Location, relative2.Size);
					rect.X += num;
					rect.Y += num;
					chartGraph.DrawString(text, Font, new SolidBrush(gradientColor), rect, stringFormat, GetTextOrientation());
					chartGraph.StartHotRegion(href, toolTip);
					chartGraph.DrawString(text, Font, new SolidBrush(Color), relative2, stringFormat, GetTextOrientation());
					chartGraph.EndHotRegion();
					break;
				}
				default:
					throw new InvalidOperationException(SR.ExceptionTitleTextDrawingStyleUnknown);
				}
				chartGraph.StopAnimation();
			}
			if (matrix != null)
			{
				chartGraph.Transform = matrix;
			}
			if (common.ProcessModeRegions)
			{
				common.HotRegionsList.AddHotRegion(chartGraph, rectangleF, ToolTip, Href, MapAreaAttributes, this, ChartElementType.Title, string.Empty);
			}
		}

		internal void CalcTitlePosition(ChartGraphics chartGraph, ref RectangleF chartAreasRectangle, ref RectangleF frameTitlePosition, float elementSpacing)
		{
			if (!frameTitlePosition.IsEmpty && Position.Auto && Docking == Docking.Top && DockToChartArea == "NotSet")
			{
				Position.SetPositionNoAuto(frameTitlePosition.X + elementSpacing, frameTitlePosition.Y, frameTitlePosition.Width - 2f * elementSpacing, frameTitlePosition.Height);
				frameTitlePosition = RectangleF.Empty;
				return;
			}
			RectangleF rectangleF = default(RectangleF);
			StringFormat stringFormat = new StringFormat();
			SizeF relative = new SizeF(chartAreasRectangle.Width, chartAreasRectangle.Height);
			if (IsTextVertical)
			{
				float width = relative.Width;
				relative.Width = relative.Height;
				relative.Height = width;
			}
			relative.Width -= 2f * elementSpacing;
			relative.Height -= 2f * elementSpacing;
			relative = chartGraph.GetAbsoluteSize(relative);
			SizeF size = chartGraph.MeasureString(Text.Replace("\\n", "\n"), Font, relative, stringFormat, GetTextOrientation());
			if (BackGroundIsVisible)
			{
				size.Width += titleBorderSpacing;
				size.Height += titleBorderSpacing;
			}
			if (IsTextVertical)
			{
				float width2 = size.Width;
				size.Width = size.Height;
				size.Height = width2;
			}
			size = chartGraph.GetRelativeSize(size);
			rectangleF.Height = size.Height;
			rectangleF.Width = size.Width;
			if (float.IsNaN(size.Height) || float.IsNaN(size.Width))
			{
				return;
			}
			if (Docking == Docking.Top)
			{
				rectangleF.Y = chartAreasRectangle.Y + elementSpacing;
				rectangleF.X = chartAreasRectangle.X + elementSpacing;
				rectangleF.Width = chartAreasRectangle.Right - rectangleF.X - elementSpacing;
				if (rectangleF.Width < 0f)
				{
					rectangleF.Width = 0f;
				}
				chartAreasRectangle.Height -= rectangleF.Height + elementSpacing;
				chartAreasRectangle.Y = rectangleF.Bottom;
			}
			else if (Docking == Docking.Bottom)
			{
				rectangleF.Y = chartAreasRectangle.Bottom - size.Height - elementSpacing;
				rectangleF.X = chartAreasRectangle.X + elementSpacing;
				rectangleF.Width = chartAreasRectangle.Right - rectangleF.X - elementSpacing;
				if (rectangleF.Width < 0f)
				{
					rectangleF.Width = 0f;
				}
				chartAreasRectangle.Height -= rectangleF.Height + elementSpacing;
			}
			if (Docking == Docking.Left)
			{
				rectangleF.X = chartAreasRectangle.X + elementSpacing;
				rectangleF.Y = chartAreasRectangle.Y + elementSpacing;
				rectangleF.Height = chartAreasRectangle.Bottom - rectangleF.Y - elementSpacing;
				if (rectangleF.Height < 0f)
				{
					rectangleF.Height = 0f;
				}
				chartAreasRectangle.Width -= rectangleF.Width + elementSpacing;
				chartAreasRectangle.X = rectangleF.Right;
			}
			if (Docking == Docking.Right)
			{
				rectangleF.X = chartAreasRectangle.Right - size.Width - elementSpacing;
				rectangleF.Y = chartAreasRectangle.Y + elementSpacing;
				rectangleF.Height = chartAreasRectangle.Bottom - rectangleF.Y - elementSpacing;
				if (rectangleF.Height < 0f)
				{
					rectangleF.Height = 0f;
				}
				chartAreasRectangle.Width -= rectangleF.Width + elementSpacing;
			}
			if (DockOffset != 0)
			{
				if (Docking == Docking.Top || Docking == Docking.Bottom)
				{
					rectangleF.Y += DockOffset;
				}
				else
				{
					rectangleF.X += DockOffset;
				}
			}
			Position.SetPositionNoAuto(rectangleF.X, rectangleF.Y, rectangleF.Width, rectangleF.Height);
		}
	}
}
