using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace Microsoft.Reporting.Map.WebForms
{
	[TypeConverter(typeof(MapLabelConverter))]
	internal class MapLabel : AutoSizePanel
	{
		private ContentAlignment textAlignment = ContentAlignment.TopCenter;

		private Font font = new Font("Microsoft Sans Serif", 12f);

		private Color textColor = Color.Black;

		private string text = "Text";

		private int textShadowOffset;

		private float angle = float.NaN;

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeMapLabel_ToolTip")]
		[Localizable(true)]
		[DefaultValue("")]
		public override string ToolTip
		{
			get
			{
				return base.ToolTip;
			}
			set
			{
				base.ToolTip = value;
			}
		}

		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeMapLabel_Href")]
		[Localizable(true)]
		[DefaultValue("")]
		public override string Href
		{
			get
			{
				return base.Href;
			}
			set
			{
				base.Href = value;
			}
		}

		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeMapLabel_MapAreaAttributes")]
		[DefaultValue("")]
		public override string MapAreaAttributes
		{
			get
			{
				return base.MapAreaAttributes;
			}
			set
			{
				base.MapAreaAttributes = value;
			}
		}

		[SRCategory("CategoryAttribute_Data")]
		[SRDescription("DescriptionAttributeMapLabel_Name")]
		[Browsable(true)]
		[EditorBrowsable(EditorBrowsableState.Always)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[SerializationVisibility(SerializationVisibility.Attribute)]
		public override string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				base.Name = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeMapLabel_TextAlignment")]
		[DefaultValue(ContentAlignment.TopCenter)]
		public ContentAlignment TextAlignment
		{
			get
			{
				return textAlignment;
			}
			set
			{
				textAlignment = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeMapLabel_Visible")]
		[ParenthesizePropertyName(true)]
		[DefaultValue(true)]
		public override bool Visible
		{
			get
			{
				return base.Visible;
			}
			set
			{
				base.Visible = value;
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeMapLabel_Font")]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 12pt")]
		public Font Font
		{
			get
			{
				return font;
			}
			set
			{
				font = value;
				Invalidate(layout: true);
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeMapLabel_BorderStyle")]
		[NotifyParentProperty(true)]
		[DefaultValue(MapDashStyle.Solid)]
		public override MapDashStyle BorderStyle
		{
			get
			{
				return base.BorderStyle;
			}
			set
			{
				base.BorderStyle = value;
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeMapLabel_BackColor")]
		[NotifyParentProperty(true)]
		public override Color BackColor
		{
			get
			{
				return base.BackColor;
			}
			set
			{
				base.BackColor = value;
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeMapLabel_TextColor")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "Black")]
		public Color TextColor
		{
			get
			{
				return textColor;
			}
			set
			{
				textColor = value;
				Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeMapLabel_BackGradientType")]
		public override GradientType BackGradientType
		{
			get
			{
				return base.BackGradientType;
			}
			set
			{
				base.BackGradientType = value;
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeMapLabel_BackSecondaryColor")]
		[NotifyParentProperty(true)]
		public override Color BackSecondaryColor
		{
			get
			{
				return base.BackSecondaryColor;
			}
			set
			{
				base.BackSecondaryColor = value;
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeMapLabel_BackHatchStyle")]
		[NotifyParentProperty(true)]
		public override MapHatchStyle BackHatchStyle
		{
			get
			{
				return base.BackHatchStyle;
			}
			set
			{
				base.BackHatchStyle = value;
			}
		}

		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeMapLabel_Text")]
		[Localizable(true)]
		[DefaultValue("Text")]
		public string Text
		{
			get
			{
				return text;
			}
			set
			{
				text = value;
				Invalidate(layout: true);
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeMapLabel_BackShadowOffset")]
		[NotifyParentProperty(true)]
		[DefaultValue(0)]
		public override int BackShadowOffset
		{
			get
			{
				return base.BackShadowOffset;
			}
			set
			{
				base.BackShadowOffset = value;
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeMapLabel_TextShadowOffset")]
		[NotifyParentProperty(true)]
		[DefaultValue(0)]
		public int TextShadowOffset
		{
			get
			{
				return textShadowOffset;
			}
			set
			{
				if (value < -100 || value > 100)
				{
					throw new ArgumentException(SR.must_in_range(-100.0, 100.0));
				}
				textShadowOffset = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Position")]
		[SRDescription("DescriptionAttributeMapLabel_Angle")]
		[TypeConverter(typeof(FloatAutoValueConverter))]
		[DefaultValue(float.NaN)]
		public float Angle
		{
			get
			{
				return angle;
			}
			set
			{
				if (value > 360f || value < 0f)
				{
					throw new ArgumentOutOfRangeException(SR.out_of_range(0.0, 360.0));
				}
				angle = value;
				Invalidate(layout: true);
			}
		}

		internal Position Position => new Position(Location, Size, ContentAlignment.TopLeft);

		internal override bool IsEmpty
		{
			get
			{
				if (Common != null && Common.MapCore.IsDesignMode())
				{
					return false;
				}
				return string.IsNullOrEmpty(Text);
			}
		}

		public MapLabel()
			: this(null)
		{
		}

		internal MapLabel(CommonElements common)
			: base(common)
		{
			BackShadowOffset = 0;
			Location.DefaultValues = true;
			Size.DefaultValues = true;
			BorderStyle = MapDashStyle.Solid;
			Visible = true;
		}

		public override string ToString()
		{
			return Name;
		}

		internal override bool ShouldRenderBackground()
		{
			return true;
		}

		internal override void Render(MapGraphics g)
		{
			if (IsVisible())
			{
				if (TextShadowOffset != 0)
				{
					DrawText(g, drawShadow: true);
				}
				DrawText(g, drawShadow: false);
			}
		}

		private void DrawText(MapGraphics g, bool drawShadow)
		{
			if (string.IsNullOrEmpty(Text))
			{
				return;
			}
			RectangleF absoluteRectangle = g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f));
			string text = Text;
			_ = Font;
			text = text.Replace("\\n", "\n");
			StringFormat stringFormat = GetStringFormat();
			TextRenderingHint textRenderingHint = g.TextRenderingHint;
			float num = DetermineAngle();
			if (num % 90f != 0f)
			{
				g.TextRenderingHint = TextRenderingHint.AntiAlias;
			}
			Brush brush = null;
			brush = ((!drawShadow) ? new SolidBrush(TextColor) : g.GetShadowBrush());
			try
			{
				if (num != 0f)
				{
					RectangleF layoutRectangle = DetermineTextRectangle(g, stringFormat);
					PointF point = new PointF(layoutRectangle.X + layoutRectangle.Width / 2f, layoutRectangle.Y + layoutRectangle.Height / 2f);
					Matrix transform = g.Transform;
					Matrix matrix = g.Transform.Clone();
					matrix.RotateAt(num, point, MatrixOrder.Prepend);
					if (drawShadow)
					{
						matrix.Translate(TextShadowOffset, TextShadowOffset, MatrixOrder.Append);
					}
					g.Transform = matrix;
					StringFormat stringFormat2 = new StringFormat();
					stringFormat2.Alignment = StringAlignment.Center;
					stringFormat2.LineAlignment = StringAlignment.Center;
					stringFormat2.Trimming = StringTrimming.EllipsisCharacter;
					layoutRectangle.Inflate(1000f, 1000f);
					g.DrawString(text, Font, brush, layoutRectangle, stringFormat2);
					g.Transform = transform;
				}
				else
				{
					if (drawShadow)
					{
						absoluteRectangle.X += TextShadowOffset;
						absoluteRectangle.Y += TextShadowOffset;
					}
					g.DrawString(text, Font, brush, absoluteRectangle, stringFormat);
				}
			}
			finally
			{
				brush?.Dispose();
			}
			g.Graphics.TextRenderingHint = textRenderingHint;
		}

		private RectangleF DetermineTextRectangle(MapGraphics g, StringFormat stringFormat)
		{
			RectangleF result = default(RectangleF);
			RectangleF absoluteRectangle = g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f));
			SizeF sizeF = DetermineTextSizeAfterRotation(g);
			if (stringFormat.Alignment == StringAlignment.Near)
			{
				result.X = absoluteRectangle.X;
			}
			else if (stringFormat.Alignment == StringAlignment.Center)
			{
				result.X = absoluteRectangle.X + absoluteRectangle.Width / 2f - sizeF.Width / 2f;
			}
			else if (stringFormat.Alignment == StringAlignment.Far)
			{
				result.X = absoluteRectangle.Right - sizeF.Width;
			}
			if (stringFormat.LineAlignment == StringAlignment.Near)
			{
				result.Y = absoluteRectangle.Y;
			}
			else if (stringFormat.LineAlignment == StringAlignment.Center)
			{
				result.Y = absoluteRectangle.Y + absoluteRectangle.Height / 2f - sizeF.Height / 2f;
			}
			else if (stringFormat.LineAlignment == StringAlignment.Far)
			{
				result.Y = absoluteRectangle.Bottom - sizeF.Height;
			}
			result.Width = sizeF.Width;
			result.Height = sizeF.Height;
			return result;
		}

		private StringFormat GetStringFormat()
		{
			StringFormat stringFormat = new StringFormat();
			stringFormat.Trimming = StringTrimming.EllipsisCharacter;
			if (TextAlignment == ContentAlignment.TopLeft)
			{
				stringFormat.Alignment = StringAlignment.Near;
				stringFormat.LineAlignment = StringAlignment.Near;
			}
			else if (TextAlignment == ContentAlignment.TopCenter)
			{
				stringFormat.Alignment = StringAlignment.Center;
				stringFormat.LineAlignment = StringAlignment.Near;
			}
			else if (TextAlignment == ContentAlignment.TopRight)
			{
				stringFormat.Alignment = StringAlignment.Far;
				stringFormat.LineAlignment = StringAlignment.Near;
			}
			else if (TextAlignment == ContentAlignment.MiddleLeft)
			{
				stringFormat.Alignment = StringAlignment.Near;
				stringFormat.LineAlignment = StringAlignment.Center;
			}
			else if (TextAlignment == ContentAlignment.MiddleCenter)
			{
				stringFormat.Alignment = StringAlignment.Center;
				stringFormat.LineAlignment = StringAlignment.Center;
			}
			else if (TextAlignment == ContentAlignment.MiddleRight)
			{
				stringFormat.Alignment = StringAlignment.Far;
				stringFormat.LineAlignment = StringAlignment.Center;
			}
			else if (TextAlignment == ContentAlignment.BottomLeft)
			{
				stringFormat.Alignment = StringAlignment.Near;
				stringFormat.LineAlignment = StringAlignment.Far;
			}
			else if (TextAlignment == ContentAlignment.BottomCenter)
			{
				stringFormat.Alignment = StringAlignment.Center;
				stringFormat.LineAlignment = StringAlignment.Far;
			}
			else
			{
				stringFormat.Alignment = StringAlignment.Far;
				stringFormat.LineAlignment = StringAlignment.Far;
			}
			return stringFormat;
		}

		internal GraphicsPath GetPath(MapGraphics g)
		{
			if (!IsVisible())
			{
				return null;
			}
			GraphicsPath graphicsPath = new GraphicsPath();
			RectangleF absoluteRectangle = g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f));
			graphicsPath.AddRectangle(absoluteRectangle);
			float num = DetermineAngle();
			if (num != 0f)
			{
				PointF point = new PointF(absoluteRectangle.X + absoluteRectangle.Width / 2f, absoluteRectangle.Y + absoluteRectangle.Height / 2f);
				using (Matrix matrix = new Matrix())
				{
					matrix.RotateAt(num, point);
					graphicsPath.Transform(matrix);
					return graphicsPath;
				}
			}
			return graphicsPath;
		}

		internal Brush GetBackBrush(MapGraphics g)
		{
			RectangleF absoluteRectangle = g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f));
			Brush brush = null;
			Color backColor = BackColor;
			Color backSecondaryColor = BackSecondaryColor;
			GradientType backGradientType = BackGradientType;
			MapHatchStyle backHatchStyle = BackHatchStyle;
			if (backHatchStyle != 0)
			{
				brush = MapGraphics.GetHatchBrush(backHatchStyle, backColor, backSecondaryColor);
			}
			else if (backGradientType != 0)
			{
				brush = g.GetGradientBrush(absoluteRectangle, backColor, backSecondaryColor, backGradientType);
				float num = DetermineAngle();
				if (num != 0f)
				{
					PointF pointF = new PointF(absoluteRectangle.X + absoluteRectangle.Width / 2f, absoluteRectangle.Y + absoluteRectangle.Height / 2f);
					if (brush is LinearGradientBrush)
					{
						((LinearGradientBrush)brush).TranslateTransform(0f - pointF.X, 0f - pointF.Y, MatrixOrder.Append);
						((LinearGradientBrush)brush).RotateTransform(num, MatrixOrder.Append);
						((LinearGradientBrush)brush).TranslateTransform(pointF.X, pointF.Y, MatrixOrder.Append);
					}
					else if (brush is PathGradientBrush)
					{
						((PathGradientBrush)brush).TranslateTransform(0f - pointF.X, 0f - pointF.Y, MatrixOrder.Append);
						((PathGradientBrush)brush).RotateTransform(num, MatrixOrder.Append);
						((PathGradientBrush)brush).TranslateTransform(pointF.X, pointF.Y, MatrixOrder.Append);
					}
				}
			}
			else
			{
				brush = new SolidBrush(backColor);
			}
			return brush;
		}

		internal Pen GetPen()
		{
			if (BorderWidth <= 0 || BorderStyle == MapDashStyle.None)
			{
				return null;
			}
			_ = BorderColor;
			_ = BorderWidth;
			_ = BorderStyle;
			return new Pen(BorderColor, BorderWidth)
			{
				DashStyle = MapGraphics.GetPenStyle(BorderStyle),
				Alignment = PenAlignment.Center
			};
		}

		internal override object GetDefaultPropertyValue(string prop, object currentValue)
		{
			object obj = null;
			switch (prop)
			{
			case "Size":
				return new MapSize(null, 100f, 10f);
			case "SizeUnit":
				return CoordinateUnit.Percent;
			case "Dock":
				return PanelDockStyle.Top;
			case "DockAlignment":
				return DockAlignment.Center;
			case "BackColor":
				return Color.Empty;
			case "BackGradientType":
				return GradientType.None;
			case "BorderWidth":
				return 0;
			default:
				return base.GetDefaultPropertyValue(prop, currentValue);
			}
		}

		internal override SizeF GetOptimalSize(MapGraphics g, SizeF maxSizeAbs)
		{
			SizeF result = DetermineTextSizeAfterRotation(g);
			SizeF sizeF;
			if (base.DockedInsideViewport)
			{
				sizeF = Common.MapCore.Viewport.GetSizeInPixels();
				sizeF.Width -= Common.MapCore.Viewport.Margins.Left + Common.MapCore.Viewport.Margins.Right;
				sizeF.Height -= Common.MapCore.Viewport.Margins.Top + Common.MapCore.Viewport.Margins.Bottom;
			}
			else
			{
				sizeF = Common.MapCore.CalculateMapDockBounds(g).Size;
			}
			if (Dock == PanelDockStyle.Top || Dock == PanelDockStyle.Bottom)
			{
				result.Width = sizeF.Width - (float)base.Margins.Left - (float)base.Margins.Right;
			}
			else if (Dock == PanelDockStyle.Left || Dock == PanelDockStyle.Right)
			{
				result.Height = sizeF.Height - (float)base.Margins.Top - (float)base.Margins.Bottom;
			}
			return result;
		}

		private SizeF DetermineTextSizeAfterRotation(MapGraphics g)
		{
			string text = Text.Replace("\\n", "\n");
			SizeF unrotatedSize = g.MeasureString(text, Font, new SizeF(0f, 0f), GetStringFormat());
			unrotatedSize.Width += 1f;
			unrotatedSize.Height += 1f;
			return CalculateRotatedSize(unrotatedSize, DetermineAngle());
		}

		internal SizeF CalculateRotatedSize(SizeF unrotatedSize, float andgleOfRotation)
		{
			andgleOfRotation %= 180f;
			if (andgleOfRotation > 90f)
			{
				andgleOfRotation %= 90f;
				float width = unrotatedSize.Width;
				unrotatedSize.Width = unrotatedSize.Height;
				unrotatedSize.Height = width;
			}
			double num = (double)andgleOfRotation * Math.PI / 180.0;
			double num2 = Math.Cos(num);
			double num3 = Math.Cos(Math.PI / 2.0 - num);
			return new SizeF((float)Math.Abs((double)unrotatedSize.Width * num2 + (double)unrotatedSize.Height * num3), (float)Math.Abs((double)unrotatedSize.Height * num2 + (double)unrotatedSize.Width * num3));
		}

		internal float DetermineAngle()
		{
			if (double.IsNaN(Angle))
			{
				if (Dock == PanelDockStyle.None)
				{
					SizeF sizeInPixels = GetSizeInPixels();
					if (sizeInPixels.Width >= sizeInPixels.Height)
					{
						return 0f;
					}
					return 90f;
				}
				if (Dock == PanelDockStyle.Left)
				{
					return 270f;
				}
				if (Dock == PanelDockStyle.Right)
				{
					return 90f;
				}
				return 0f;
			}
			return Angle;
		}
	}
}
