using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.IO;

namespace Microsoft.Reporting.Gauge.WebForms
{
	[TypeConverter(typeof(GaugeLabelConverter))]
	internal class GaugeLabel : NamedElement, IRenderable, IToolTipProvider, ISelectable, IImageMapProvider
	{
		private string parent = string.Empty;

		private int zOrder;

		private string toolTip = "";

		private string href = "";

		private string mapAreaAttributes = "";

		private GaugeLocation location;

		private GaugeSize size;

		private ContentAlignment textAlignment = ContentAlignment.TopLeft;

		private bool visible = true;

		private Font font = new Font("Microsoft Sans Serif", 8.25f);

		private ResizeMode resizeMode = ResizeMode.AutoFit;

		private FontUnit fontUnit = FontUnit.Default;

		private Color borderColor = Color.Black;

		private GaugeDashStyle borderStyle;

		private int borderWidth = 1;

		private Color backColor = Color.Empty;

		private Color textColor = Color.Black;

		private GradientType backGradientType;

		private Color backGradientEndColor = Color.Empty;

		private GaugeHatchStyle backHatchStyle;

		private string text = "Text";

		private int backShadowOffset;

		private int textShadowOffset;

		private float angle;

		private bool selected;

		private NamedElement parentSystem;

		private bool defaultParent = true;

		private object imageMapProviderTag;

		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeGaugeLabel_Parent")]
		[TypeConverter(typeof(ParentSourceConverter))]
		[NotifyParentProperty(true)]
		public string Parent
		{
			get
			{
				return parent;
			}
			set
			{
				string text = parent;
				if (value == "(none)")
				{
					value = string.Empty;
				}
				parent = value;
				try
				{
					ConnectToParent(exact: true);
				}
				catch
				{
					parent = text;
					throw;
				}
				DefaultParent = false;
				Invalidate();
			}
		}

		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeGaugeLabel_ZOrder")]
		[DefaultValue(0)]
		public int ZOrder
		{
			get
			{
				return zOrder;
			}
			set
			{
				zOrder = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeGaugeLabel_ToolTip")]
		[Localizable(true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
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
				Invalidate();
			}
		}

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeGaugeLabel_Href")]
		[Localizable(true)]
		[Browsable(false)]
		[DefaultValue("")]
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

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeMapAreaAttributes4")]
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

		[SRCategory("CategoryMisc")]
		[SRDescription("DescriptionAttributeGaugeLabel_Name")]
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

		[SRCategory("CategoryPosition")]
		[SRDescription("DescriptionAttributeGaugeLabel_Location")]
		[TypeConverter(typeof(LocationConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[ValidateBound(100.0, 100.0)]
		public GaugeLocation Location
		{
			get
			{
				return location;
			}
			set
			{
				location = value;
				location.Parent = this;
				Invalidate();
			}
		}

		[SRCategory("CategoryPosition")]
		[SRDescription("DescriptionAttributeGaugeLabel_Size")]
		[TypeConverter(typeof(SizeConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[ValidateBound(100.0, 100.0)]
		public GaugeSize Size
		{
			get
			{
				return size;
			}
			set
			{
				size = value;
				size.Parent = this;
				Invalidate();
			}
		}

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeGaugeLabel_TextAlignment")]
		[DefaultValue(ContentAlignment.TopLeft)]
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

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeGaugeLabel_Visible")]
		[ParenthesizePropertyName(true)]
		[DefaultValue(true)]
		public bool Visible
		{
			get
			{
				return visible;
			}
			set
			{
				visible = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeGaugeLabel_Font")]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 8.25pt")]
		public Font Font
		{
			get
			{
				return font;
			}
			set
			{
				font = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeResizeMode")]
		[DefaultValue(ResizeMode.AutoFit)]
		public ResizeMode ResizeMode
		{
			get
			{
				return resizeMode;
			}
			set
			{
				resizeMode = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeFontUnit3")]
		[DefaultValue(FontUnit.Default)]
		public FontUnit FontUnit
		{
			get
			{
				return fontUnit;
			}
			set
			{
				fontUnit = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeGaugeLabel_BorderColor")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "Black")]
		public Color BorderColor
		{
			get
			{
				return borderColor;
			}
			set
			{
				borderColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeGaugeLabel_BorderStyle")]
		[NotifyParentProperty(true)]
		[DefaultValue(GaugeDashStyle.NotSet)]
		public GaugeDashStyle BorderStyle
		{
			get
			{
				return borderStyle;
			}
			set
			{
				borderStyle = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeGaugeLabel_BorderWidth")]
		[NotifyParentProperty(true)]
		[DefaultValue(1)]
		public int BorderWidth
		{
			get
			{
				return borderWidth;
			}
			set
			{
				if (value < 0 || value > 100)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", 0, 100));
				}
				borderWidth = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeGaugeLabel_BackColor")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "Empty")]
		public Color BackColor
		{
			get
			{
				return backColor;
			}
			set
			{
				backColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeGaugeLabel_TextColor")]
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

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeGaugeLabel_BackGradientType")]
		[NotifyParentProperty(true)]
		[DefaultValue(GradientType.None)]
		public GradientType BackGradientType
		{
			get
			{
				return backGradientType;
			}
			set
			{
				backGradientType = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeGaugeLabel_BackGradientEndColor")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "Empty")]
		public Color BackGradientEndColor
		{
			get
			{
				return backGradientEndColor;
			}
			set
			{
				backGradientEndColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeGaugeLabel_BackHatchStyle")]
		[NotifyParentProperty(true)]
		[DefaultValue(GaugeHatchStyle.None)]
		public GaugeHatchStyle BackHatchStyle
		{
			get
			{
				return backHatchStyle;
			}
			set
			{
				backHatchStyle = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeGaugeLabel_Text")]
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
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeGaugeLabel_BackShadowOffset")]
		[NotifyParentProperty(true)]
		[ValidateBound(-5.0, 5.0)]
		[DefaultValue(0)]
		public int BackShadowOffset
		{
			get
			{
				return backShadowOffset;
			}
			set
			{
				if (value < -100 || value > 100)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", -100, 100));
				}
				backShadowOffset = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeGaugeLabel_TextShadowOffset")]
		[NotifyParentProperty(true)]
		[ValidateBound(-5.0, 5.0)]
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
					throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", -100, 100));
				}
				textShadowOffset = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryPosition")]
		[SRDescription("DescriptionAttributeGaugeLabel_Angle")]
		[DefaultValue(0f)]
		[ValidateBound(0.0, 360.0)]
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
					throw new ArgumentException(Utils.SRGetStr("ExceptionOutOfrange", 0, 360));
				}
				angle = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeGaugeLabel_Selected")]
		[Browsable(false)]
		[DefaultValue(false)]
		public bool Selected
		{
			get
			{
				return selected;
			}
			set
			{
				selected = value;
				Invalidate();
			}
		}

		internal Position Position => new Position(Location, Size, ContentAlignment.TopLeft);

		[Bindable(false)]
		[Browsable(false)]
		[DefaultValue(null)]
		[SRDescription("DescriptionAttributeParentObject3")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public NamedElement ParentObject => parentSystem;

		internal bool DefaultParent
		{
			get
			{
				return defaultParent;
			}
			set
			{
				defaultParent = value;
			}
		}

		object IImageMapProvider.Tag
		{
			get
			{
				return imageMapProviderTag;
			}
			set
			{
				imageMapProviderTag = value;
			}
		}

		public GaugeLabel()
		{
			location = new GaugeLocation(this, 26f, 47f);
			size = new GaugeSize(this, 12f, 6f);
			location.DefaultValues = true;
			size.DefaultValues = true;
		}

		public override string ToString()
		{
			return Name;
		}

		internal override void EndInit()
		{
			base.EndInit();
			ConnectToParent(exact: true);
		}

		private void ConnectToParent(bool exact)
		{
			if (Common != null && !Common.GaugeCore.isInitializing)
			{
				if (parent == string.Empty)
				{
					parentSystem = null;
					return;
				}
				Common.ObjectLinker.IsParentElementValid(this, this, exact);
				parentSystem = Common.ObjectLinker.GetElement(parent);
			}
		}

		internal override void Notify(MessageType msg, NamedElement element, object param)
		{
			base.Notify(msg, element, param);
			switch (msg)
			{
			case MessageType.NamedElementRemove:
				if (parentSystem == element)
				{
					Parent = string.Empty;
				}
				break;
			case MessageType.NamedElementRename:
				if (parentSystem == element)
				{
					parent = element.GetNameAsParent((string)param);
				}
				break;
			}
		}

		internal override void OnAdded()
		{
			base.OnAdded();
			ConnectToParent(exact: true);
		}

		internal GraphicsPath GetBackPath(GaugeGraphics g)
		{
			if (!Visible)
			{
				return null;
			}
			GraphicsPath graphicsPath = new GraphicsPath();
			RectangleF absoluteRectangle = g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f));
			graphicsPath.AddRectangle(absoluteRectangle);
			if (Angle != 0f)
			{
				PointF point = new PointF(absoluteRectangle.X + absoluteRectangle.Width / 2f, absoluteRectangle.Y + absoluteRectangle.Height / 2f);
				using (Matrix matrix = new Matrix())
				{
					matrix.RotateAt(Angle, point);
					graphicsPath.Transform(matrix);
					return graphicsPath;
				}
			}
			return graphicsPath;
		}

		internal GraphicsPath GetTextPath(GaugeGraphics g)
		{
			if (!Visible)
			{
				return null;
			}
			if (Text.Length == 0)
			{
				return null;
			}
			RectangleF absoluteRectangle = g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f));
			GraphicsPath graphicsPath = new GraphicsPath();
			string text = Text;
			Font font = Font;
			text = text.Replace("\\n", "\n");
			float emSize;
			if (ResizeMode == ResizeMode.AutoFit)
			{
				SizeF sizeF = g.MeasureString(text, font);
				SizeF absoluteSize = g.GetAbsoluteSize(new SizeF(100f, 100f));
				float num = absoluteSize.Width / sizeF.Width;
				float num2 = absoluteSize.Height / sizeF.Height;
				emSize = ((!(num < num2)) ? (font.SizeInPoints * num2 * 1.3f * g.Graphics.DpiY / 96f) : (font.SizeInPoints * num * 1.3f * g.Graphics.DpiY / 96f));
			}
			else
			{
				if (FontUnit == FontUnit.Percent)
				{
					g.RestoreDrawRegion();
					emSize = g.GetAbsoluteDimension(font.Size);
					RectangleF boundRect = ((IRenderable)this).GetBoundRect(g);
					g.CreateDrawRegion(boundRect);
				}
				else
				{
					emSize = font.SizeInPoints * g.Graphics.DpiY / 96f;
				}
				emSize *= 1.3f;
			}
			StringFormat stringFormat = new StringFormat();
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
			graphicsPath.AddString(text, font.FontFamily, (int)font.Style, emSize, absoluteRectangle, stringFormat);
			if (Angle != 0f)
			{
				PointF point = new PointF(absoluteRectangle.X + absoluteRectangle.Width / 2f, absoluteRectangle.Y + absoluteRectangle.Height / 2f);
				using (Matrix matrix = new Matrix())
				{
					matrix.RotateAt(Angle, point);
					graphicsPath.Transform(matrix);
					return graphicsPath;
				}
			}
			return graphicsPath;
		}

		internal Brush GetBackBrush(GaugeGraphics g)
		{
			RectangleF absoluteRectangle = g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f));
			Brush brush = null;
			Color color = BackColor;
			Color color2 = BackGradientEndColor;
			GradientType gradientType = BackGradientType;
			GaugeHatchStyle gaugeHatchStyle = BackHatchStyle;
			if (gaugeHatchStyle != 0)
			{
				brush = GaugeGraphics.GetHatchBrush(gaugeHatchStyle, color, color2);
			}
			else if (gradientType != 0)
			{
				brush = g.GetGradientBrush(absoluteRectangle, color, color2, gradientType);
				if (Angle != 0f)
				{
					PointF pointF = new PointF(absoluteRectangle.X + absoluteRectangle.Width / 2f, absoluteRectangle.Y + absoluteRectangle.Height / 2f);
					if (brush is LinearGradientBrush)
					{
						((LinearGradientBrush)brush).TranslateTransform(0f - pointF.X, 0f - pointF.Y, MatrixOrder.Append);
						((LinearGradientBrush)brush).RotateTransform(Angle, MatrixOrder.Append);
						((LinearGradientBrush)brush).TranslateTransform(pointF.X, pointF.Y, MatrixOrder.Append);
					}
					else if (brush is PathGradientBrush)
					{
						((PathGradientBrush)brush).TranslateTransform(0f - pointF.X, 0f - pointF.Y, MatrixOrder.Append);
						((PathGradientBrush)brush).RotateTransform(Angle, MatrixOrder.Append);
						((PathGradientBrush)brush).TranslateTransform(pointF.X, pointF.Y, MatrixOrder.Append);
					}
				}
			}
			else
			{
				brush = new SolidBrush(color);
			}
			return brush;
		}

		internal Pen GetPen(GaugeGraphics g)
		{
			if (BorderWidth <= 0 || BorderStyle == GaugeDashStyle.NotSet)
			{
				return null;
			}
			_ = BorderColor;
			_ = BorderWidth;
			_ = BorderStyle;
			return new Pen(BorderColor, BorderWidth)
			{
				DashStyle = g.GetPenStyle(BorderStyle),
				Alignment = PenAlignment.Center
			};
		}

		void IRenderable.RenderStaticElements(GaugeGraphics g)
		{
		}

		void IRenderable.RenderDynamicElements(GaugeGraphics g)
		{
			if (!Visible)
			{
				return;
			}
			g.StartHotRegion(this);
			GraphicsPath graphicsPath = null;
			GraphicsPath graphicsPath2 = null;
			Brush brush = null;
			Brush brush2 = null;
			Brush brush3 = null;
			Pen pen = null;
			try
			{
				graphicsPath = GetBackPath(g);
				graphicsPath2 = GetTextPath(g);
				if (graphicsPath != null)
				{
					if (BackShadowOffset != 0)
					{
						using (Matrix matrix = new Matrix())
						{
							brush3 = g.GetShadowBrush();
							matrix.Translate(BackShadowOffset, BackShadowOffset, MatrixOrder.Append);
							graphicsPath.Transform(matrix);
							g.FillPath(brush3, graphicsPath);
							matrix.Reset();
							matrix.Translate(-BackShadowOffset, -BackShadowOffset, MatrixOrder.Append);
							graphicsPath.Transform(matrix);
						}
					}
					brush2 = GetBackBrush(g);
					g.FillPath(brush2, graphicsPath);
					pen = GetPen(g);
					if (pen != null)
					{
						g.DrawPath(pen, graphicsPath);
					}
					Common.GaugeCore.HotRegionList.SetHotRegion(this, Point.Empty, (GraphicsPath)graphicsPath.Clone());
				}
				if (graphicsPath2 == null)
				{
					return;
				}
				if (TextShadowOffset != 0)
				{
					using (Matrix matrix2 = new Matrix())
					{
						brush3 = g.GetShadowBrush();
						matrix2.Translate(TextShadowOffset, TextShadowOffset, MatrixOrder.Append);
						graphicsPath2.Transform(matrix2);
						g.FillPath(brush3, graphicsPath2);
						matrix2.Reset();
						matrix2.Translate(-TextShadowOffset, -TextShadowOffset, MatrixOrder.Append);
						graphicsPath2.Transform(matrix2);
					}
				}
				brush = new SolidBrush(TextColor);
				AntiAliasing antiAliasing = Common.GaugeContainer.AntiAliasing;
				AntiAliasing antiAliasing2 = g.AntiAliasing;
				if (Common.GaugeContainer.AntiAliasing == AntiAliasing.Text)
				{
					antiAliasing = AntiAliasing.Graphics;
				}
				else if (Common.GaugeContainer.AntiAliasing == AntiAliasing.Graphics)
				{
					antiAliasing = AntiAliasing.None;
				}
				g.AntiAliasing = antiAliasing;
				g.FillPath(brush, graphicsPath2);
				g.AntiAliasing = antiAliasing2;
			}
			finally
			{
				graphicsPath?.Dispose();
				graphicsPath2?.Dispose();
				brush3?.Dispose();
				brush2?.Dispose();
				brush?.Dispose();
				pen?.Dispose();
				g.EndHotRegion();
			}
		}

		int IRenderable.GetZOrder()
		{
			return ZOrder;
		}

		RectangleF IRenderable.GetBoundRect(GaugeGraphics g)
		{
			return Position.Rectangle;
		}

		object IRenderable.GetParentRenderable()
		{
			return ParentObject;
		}

		string IRenderable.GetParentRenderableName()
		{
			return parent;
		}

		string IToolTipProvider.GetToolTip(HitTestResult ht)
		{
			if (Common != null && Common.GaugeCore != null)
			{
				return Common.GaugeCore.ResolveAllKeywords(ToolTip, this);
			}
			return ToolTip;
		}

		string IImageMapProvider.GetToolTip()
		{
			return ((IToolTipProvider)this).GetToolTip((HitTestResult)null);
		}

		string IImageMapProvider.GetHref()
		{
			if (Common != null && Common.GaugeCore != null)
			{
				return Common.GaugeCore.ResolveAllKeywords(Href, this);
			}
			return Href;
		}

		string IImageMapProvider.GetMapAreaAttributes()
		{
			if (Common != null && Common.GaugeCore != null)
			{
				return Common.GaugeCore.ResolveAllKeywords(MapAreaAttributes, this);
			}
			return MapAreaAttributes;
		}

		void ISelectable.DrawSelection(GaugeGraphics g, bool designTimeSelection)
		{
			Stack stack = new Stack();
			for (NamedElement namedElement = ParentObject; namedElement != null; namedElement = (NamedElement)((IRenderable)namedElement).GetParentRenderable())
			{
				stack.Push(namedElement);
			}
			foreach (IRenderable item in stack)
			{
				g.CreateDrawRegion(item.GetBoundRect(g));
			}
			g.CreateDrawRegion(((IRenderable)this).GetBoundRect(g));
			g.DrawSelection(g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f)), designTimeSelection, Common.GaugeCore.SelectionBorderColor, Common.GaugeCore.SelectionMarkerColor);
			g.RestoreDrawRegion();
			foreach (IRenderable item2 in stack)
			{
				_ = item2;
				g.RestoreDrawRegion();
			}
		}

		public override object Clone()
		{
			MemoryStream stream = new MemoryStream();
			BinaryFormatSerializer binaryFormatSerializer = new BinaryFormatSerializer();
			binaryFormatSerializer.Serialize(this, stream);
			GaugeLabel gaugeLabel = new GaugeLabel();
			binaryFormatSerializer.Deserialize(gaugeLabel, stream);
			return gaugeLabel;
		}
	}
}
