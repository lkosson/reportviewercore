using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Microsoft.Reporting.Gauge.WebForms
{
	[TypeConverter(typeof(GaugeImageConverter))]
	internal class GaugeImage : NamedElement, IRenderable, IToolTipProvider, ISelectable, IImageMapProvider
	{
		private string parent = string.Empty;

		private int zOrder;

		private string toolTip = "";

		private string href = "";

		private string mapAreaAttributes = "";

		private GaugeLocation location;

		private GaugeSize size;

		private bool visible = true;

		private ResizeMode resizeMode = ResizeMode.AutoFit;

		private string image = "";

		private Color imageTransColor = Color.Empty;

		private float shadowOffset = 1f;

		private float angle;

		private float transparency;

		private bool selected;

		private NamedElement parentSystem;

		private bool defaultParent = true;

		private object imageMapProviderTag;

		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeGaugeImage_Parent")]
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
		[SRDescription("DescriptionAttributeGaugeImage_ZOrder")]
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
		[SRDescription("DescriptionAttributeGaugeImage_ToolTip")]
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
		[SRDescription("DescriptionAttributeGaugeImage_Href")]
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
		[SRDescription("DescriptionAttributeGaugeImage_MapAreaAttributes")]
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
		[SRDescription("DescriptionAttributeGaugeImage_Name")]
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
		[SRDescription("DescriptionAttributeGaugeImage_Location")]
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
		[SRDescription("DescriptionAttributeGaugeImage_Size")]
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

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeGaugeImage_Visible")]
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

		[SRCategory("CategoryImage")]
		[SRDescription("DescriptionAttributeGaugeImage_ResizeMode")]
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

		[SRCategory("CategoryImage")]
		[SRDescription("DescriptionAttributeGaugeImage_Image")]
		[DefaultValue("")]
		public string Image
		{
			get
			{
				return image;
			}
			set
			{
				image = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryImage")]
		[SRDescription("DescriptionAttributeImageTransColor6")]
		[DefaultValue(typeof(Color), "")]
		public Color ImageTransColor
		{
			get
			{
				return imageTransColor;
			}
			set
			{
				imageTransColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeShadowOffset4")]
		[NotifyParentProperty(true)]
		[ValidateBound(-5.0, 5.0)]
		[DefaultValue(1f)]
		public float ShadowOffset
		{
			get
			{
				return shadowOffset;
			}
			set
			{
				if (value < -100f || value > 100f)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", -100, 100));
				}
				shadowOffset = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryPosition")]
		[SRDescription("DescriptionAttributeGaugeImage_Angle")]
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

		[SRCategory("CategoryImage")]
		[SRDescription("DescriptionAttributeGaugeImage_Transparency")]
		[DefaultValue(0f)]
		[ValidateBound(0.0, 100.0)]
		public float Transparency
		{
			get
			{
				return transparency;
			}
			set
			{
				if (value > 100f || value < 0f)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionOutOfrange", 0, 100));
				}
				transparency = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeGaugeImage_Selected")]
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

		public GaugeImage()
		{
			location = new GaugeLocation(this, 26f, 47f);
			size = new GaugeSize(this, 15f, 15f);
			location.DefaultValues = true;
			size.DefaultValues = true;
		}

		public override string ToString()
		{
			return Name;
		}

		internal void DrawImage(GaugeGraphics g, string imageName, bool drawShadow)
		{
			if (drawShadow && ShadowOffset == 0f)
			{
				return;
			}
			Image image = Common.ImageLoader.LoadImage(imageName);
			if (image.Width == 0 || image.Height == 0)
			{
				return;
			}
			RectangleF absoluteRectangle = g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f));
			Rectangle empty = Rectangle.Empty;
			if (ResizeMode == ResizeMode.AutoFit)
			{
				empty = new Rectangle((int)absoluteRectangle.X, (int)absoluteRectangle.Y, (int)absoluteRectangle.Width, (int)absoluteRectangle.Height);
			}
			else
			{
				empty = new Rectangle(0, 0, image.Width, image.Height);
				PointF absolutePoint = g.GetAbsolutePoint(new PointF(50f, 50f));
				empty.X = (int)(absolutePoint.X - (float)(empty.Size.Width / 2));
				empty.Y = (int)(absolutePoint.Y - (float)(empty.Size.Height / 2));
			}
			ImageAttributes imageAttributes = new ImageAttributes();
			if (ImageTransColor != Color.Empty)
			{
				imageAttributes.SetColorKey(ImageTransColor, ImageTransColor, ColorAdjustType.Default);
			}
			float num = (100f - Transparency) / 100f;
			float num2 = Common.GaugeCore.ShadowIntensity / 100f;
			if (drawShadow)
			{
				ColorMatrix colorMatrix = new ColorMatrix();
				colorMatrix.Matrix00 = 0f;
				colorMatrix.Matrix11 = 0f;
				colorMatrix.Matrix22 = 0f;
				colorMatrix.Matrix33 = num2 * num;
				imageAttributes.SetColorMatrix(colorMatrix);
			}
			else if (Transparency > 0f)
			{
				ColorMatrix colorMatrix2 = new ColorMatrix();
				colorMatrix2.Matrix33 = num;
				imageAttributes.SetColorMatrix(colorMatrix2);
			}
			if (Angle != 0f)
			{
				PointF point = new PointF(absoluteRectangle.X + absoluteRectangle.Width / 2f, absoluteRectangle.Y + absoluteRectangle.Height / 2f);
				Matrix transform = g.Transform;
				Matrix matrix = g.Transform.Clone();
				float offsetX = matrix.OffsetX;
				float offsetY = matrix.OffsetY;
				point.X += offsetX;
				point.Y += offsetY;
				matrix.RotateAt(Angle, point, MatrixOrder.Append);
				if (drawShadow)
				{
					matrix.Translate(ShadowOffset, ShadowOffset, MatrixOrder.Append);
				}
				g.Transform = matrix;
				ImageSmoothingState imageSmoothingState = new ImageSmoothingState(g);
				imageSmoothingState.Set();
				g.DrawImage(image, empty, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
				imageSmoothingState.Restore();
				g.Transform = transform;
			}
			else
			{
				if (drawShadow)
				{
					empty.X += (int)ShadowOffset;
					empty.Y += (int)ShadowOffset;
				}
				ImageSmoothingState imageSmoothingState2 = new ImageSmoothingState(g);
				imageSmoothingState2.Set();
				g.DrawImage(image, empty, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
				imageSmoothingState2.Restore();
			}
			if (drawShadow)
			{
				return;
			}
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.AddRectangle(empty);
			if (Angle != 0f)
			{
				PointF point2 = new PointF(absoluteRectangle.X + absoluteRectangle.Width / 2f, absoluteRectangle.Y + absoluteRectangle.Height / 2f);
				using (Matrix matrix2 = new Matrix())
				{
					matrix2.RotateAt(Angle, point2, MatrixOrder.Append);
					graphicsPath.Transform(matrix2);
				}
			}
			Common.GaugeCore.HotRegionList.SetHotRegion(this, Point.Empty, graphicsPath);
		}

		internal GraphicsPath GetTextPath(GaugeGraphics g)
		{
			if (!Visible)
			{
				return null;
			}
			RectangleF absoluteRectangle = g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f));
			GraphicsPath graphicsPath = new GraphicsPath();
			string noImageString = SR.NoImageString;
			Font font = new Font("Microsoft Sans Serif", 8.25f);
			SizeF sizeF = g.MeasureString(noImageString, font);
			SizeF absoluteSize = g.GetAbsoluteSize(new SizeF(100f, 100f));
			float num = absoluteSize.Width / sizeF.Width;
			float num2 = absoluteSize.Height / sizeF.Height;
			float emSize = (!(num < num2)) ? (font.SizeInPoints * num2 * 1.3f) : (font.SizeInPoints * num * 1.3f);
			StringFormat stringFormat = new StringFormat();
			stringFormat.Alignment = StringAlignment.Center;
			stringFormat.LineAlignment = StringAlignment.Center;
			graphicsPath.AddString(noImageString, font.FontFamily, (int)font.Style, emSize, absoluteRectangle, stringFormat);
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

		void IRenderable.RenderStaticElements(GaugeGraphics g)
		{
			if (!Visible)
			{
				return;
			}
			g.StartHotRegion(this);
			if (Image.Length != 0)
			{
				DrawImage(g, image, drawShadow: true);
				DrawImage(g, image, drawShadow: false);
				g.EndHotRegion();
				return;
			}
			GraphicsPath graphicsPath = null;
			GraphicsPath graphicsPath2 = null;
			try
			{
				graphicsPath = GetTextPath(g);
				graphicsPath2 = GetBackPath(g);
				g.FillPath(Brushes.White, graphicsPath2);
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
				g.FillPath(Brushes.Black, graphicsPath);
				g.AntiAliasing = antiAliasing2;
				g.DrawPath(Pens.Black, graphicsPath2);
				Common.GaugeCore.HotRegionList.SetHotRegion(this, Point.Empty, (GraphicsPath)graphicsPath2.Clone());
			}
			finally
			{
				graphicsPath?.Dispose();
				graphicsPath2?.Dispose();
				g.EndHotRegion();
			}
		}

		void IRenderable.RenderDynamicElements(GaugeGraphics g)
		{
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
			GaugeImage gaugeImage = new GaugeImage();
			binaryFormatSerializer.Deserialize(gaugeImage, stream);
			return gaugeImage;
		}
	}
}
