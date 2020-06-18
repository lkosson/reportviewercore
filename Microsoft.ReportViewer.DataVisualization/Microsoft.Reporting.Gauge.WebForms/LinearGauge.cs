using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Microsoft.Reporting.Gauge.WebForms
{
	[TypeConverter(typeof(LinearGaugeConverter))]
	internal class LinearGauge : GaugeBase, ISelectable
	{
		private LinearScaleCollection scales;

		private LinearRangeCollection ranges;

		private LinearPointerCollection pointers;

		private GaugeOrientation orientation = GaugeOrientation.Auto;

		private SizeF absoluteSize = SizeF.Empty;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LinearScaleCollection Scales => scales;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LinearRangeCollection Ranges => ranges;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LinearPointerCollection Pointers => pointers;

		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeLinearGauge_Orientation")]
		[DefaultValue(GaugeOrientation.Auto)]
		public GaugeOrientation Orientation
		{
			get
			{
				return orientation;
			}
			set
			{
				orientation = value;
				Invalidate();
			}
		}

		internal SizeF AbsoluteSize
		{
			get
			{
				return absoluteSize;
			}
			set
			{
				absoluteSize = value;
			}
		}

		internal override CommonElements Common
		{
			get
			{
				return base.Common;
			}
			set
			{
				base.Common = value;
				Scales.Common = value;
				Ranges.Common = value;
				Pointers.Common = value;
			}
		}

		public LinearGauge()
		{
			base.Frame = new BackFrame(this, BackFrameStyle.Edged, BackFrameShape.Rectangular);
			base.Frame.Parent = this;
			scales = new LinearScaleCollection(this, common);
			ranges = new LinearRangeCollection(this, common);
			pointers = new LinearPointerCollection(this, common);
		}

		public override string ToString()
		{
			return Name;
		}

		internal override void BeginInit()
		{
			base.BeginInit();
			Scales.BeginInit();
			Ranges.BeginInit();
			Pointers.BeginInit();
		}

		internal override void EndInit()
		{
			base.EndInit();
			Scales.EndInit();
			Ranges.EndInit();
			Pointers.EndInit();
		}

		protected override void OnDispose()
		{
			base.OnDispose();
			Scales.Dispose();
			Ranges.Dispose();
			Pointers.Dispose();
		}

		internal override void ReconnectData(bool exact)
		{
			base.ReconnectData(exact);
			Pointers.ReconnectData(exact);
		}

		internal override void Notify(MessageType msg, NamedElement element, object param)
		{
			base.Notify(msg, element, param);
			Scales.Notify(msg, element, param);
			Ranges.Notify(msg, element, param);
			Pointers.Notify(msg, element, param);
		}

		internal override IEnumerable GetRanges()
		{
			return ranges;
		}

		internal GaugeOrientation GetOrientation()
		{
			if (Orientation == GaugeOrientation.Auto)
			{
				if (AbsoluteSize.Width < AbsoluteSize.Height)
				{
					return GaugeOrientation.Vertical;
				}
				return GaugeOrientation.Horizontal;
			}
			return Orientation;
		}

		internal override void PointerValueChanged(PointerBase sender)
		{
			ScaleBase scaleBase = sender.GetScaleBase();
			if (scaleBase == null)
			{
				return;
			}
			foreach (RangeBase range in Ranges)
			{
				if (range.ScaleName == scaleBase.Name)
				{
					range.PointerValueChanged(sender.Data);
				}
			}
			scaleBase.PointerValueChanged(sender);
		}

		internal void RenderTopImage(GaugeGraphics g)
		{
			if (base.TopImage != "")
			{
				ImageAttributes imageAttributes = new ImageAttributes();
				if (base.TopImageTransColor != Color.Empty)
				{
					imageAttributes.SetColorKey(base.TopImageTransColor, base.TopImageTransColor, ColorAdjustType.Default);
				}
				Image image = Common.ImageLoader.LoadImage(base.TopImage);
				Rectangle destRect = Rectangle.Round(g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f)));
				if (!base.TopImageHueColor.IsEmpty)
				{
					Color color = g.TransformHueColor(base.TopImageHueColor);
					ColorMatrix colorMatrix = new ColorMatrix();
					colorMatrix.Matrix00 = (float)(int)color.R / 255f;
					colorMatrix.Matrix11 = (float)(int)color.G / 255f;
					colorMatrix.Matrix22 = (float)(int)color.B / 255f;
					imageAttributes.SetColorMatrix(colorMatrix);
				}
				ImageSmoothingState imageSmoothingState = new ImageSmoothingState(g);
				imageSmoothingState.Set();
				g.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
				imageSmoothingState.Restore();
			}
		}

		internal override RectangleF GetAspectRatioBounds()
		{
			if (double.IsNaN(base.AspectRatio))
			{
				return base.Position.Rectangle;
			}
			RectangleF rectangle = base.Position.Rectangle;
			float num = (float)Common.GaugeCore.GetWidth() / 100f;
			float num2 = (float)Common.GaugeCore.GetHeight() / 100f;
			rectangle.X *= num;
			rectangle.Y *= num2;
			rectangle.Width *= num;
			rectangle.Height *= num2;
			if (rectangle.Width > rectangle.Height * base.AspectRatio)
			{
				float width = rectangle.Width;
				rectangle.Width = rectangle.Height * base.AspectRatio;
				rectangle.X += (width - rectangle.Width) / 2f;
			}
			else
			{
				float height = rectangle.Height;
				rectangle.Height = rectangle.Width / base.AspectRatio;
				rectangle.Y += (height - rectangle.Height) / 2f;
			}
			rectangle.X /= num;
			rectangle.Y /= num2;
			rectangle.Width /= num;
			rectangle.Height /= num2;
			return rectangle;
		}

		internal override RectangleF GetBoundRect(GaugeGraphics g)
		{
			if (Common != null && !float.IsNaN(base.AspectRatio))
			{
				RectangleF absoluteRectangle = g.GetAbsoluteRectangle(base.Position.Rectangle);
				if (absoluteRectangle.Width > absoluteRectangle.Height * base.AspectRatio)
				{
					float width = absoluteRectangle.Width;
					absoluteRectangle.Width = absoluteRectangle.Height * base.AspectRatio;
					absoluteRectangle.X += (width - absoluteRectangle.Width) / 2f;
					return g.GetRelativeRectangle(absoluteRectangle);
				}
				float height = absoluteRectangle.Height;
				absoluteRectangle.Height = absoluteRectangle.Width / base.AspectRatio;
				absoluteRectangle.Y += (height - absoluteRectangle.Height) / 2f;
				return g.GetRelativeRectangle(absoluteRectangle);
			}
			return base.Position.Rectangle;
		}

		internal override void RenderStaticElements(GaugeGraphics g)
		{
			if (!base.Visible)
			{
				return;
			}
			AbsoluteSize = g.GetAbsoluteSize(base.Size);
			g.StartHotRegion(this);
			base.BackFrame.RenderFrame(g);
			GraphicsState gstate = g.Save();
			if (base.ClipContent && base.BackFrame.FrameStyle != 0 && base.BackFrame.Image == string.Empty)
			{
				GraphicsPath graphicsPath = null;
				try
				{
					graphicsPath = ((base.BackFrame.FrameStyle != 0) ? base.BackFrame.GetBackPath(g) : base.BackFrame.GetFramePath(g, 0f));
					g.SetClip(graphicsPath, CombineMode.Intersect);
				}
				finally
				{
					graphicsPath?.Dispose();
				}
			}
			Common.GaugeCore.HotRegionList.SetHotRegion(this, PointF.Empty, base.BackFrame.GetFramePath(g, 0f));
			g.EndHotRegion();
			RenderStaticShadows(g);
			foreach (LinearRange range in Ranges)
			{
				range.Render(g);
			}
			foreach (LinearScale scale in Scales)
			{
				scale.RenderStaticElements(g);
			}
			if (base.ClipContent && base.BackFrame.FrameStyle != 0 && base.BackFrame.Image == string.Empty)
			{
				g.Restore(gstate);
			}
		}

		internal override void RenderDynamicElements(GaugeGraphics g)
		{
			if (!base.Visible)
			{
				return;
			}
			if (Common != null && Common.GaugeCore.renderContent == RenderContent.Dynamic)
			{
				AbsoluteSize = g.GetAbsoluteSize(base.Size);
			}
			GraphicsState gstate = g.Save();
			if (base.ClipContent && base.BackFrame.FrameStyle != 0 && base.BackFrame.Image == string.Empty)
			{
				GraphicsPath graphicsPath = null;
				try
				{
					graphicsPath = ((base.BackFrame.FrameStyle != 0) ? base.BackFrame.GetBackPath(g) : base.BackFrame.GetFramePath(g, 0f));
					g.SetClip(graphicsPath, CombineMode.Intersect);
				}
				finally
				{
					graphicsPath?.Dispose();
				}
			}
			RenderDynamicShadows(g);
			foreach (LinearPointer pointer in Pointers)
			{
				pointer.Render(g);
			}
			foreach (LinearScale scale in Scales)
			{
				scale.RenderDynamicElements(g);
			}
			if (base.ClipContent && base.BackFrame.FrameStyle != 0 && base.BackFrame.Image == string.Empty)
			{
				g.Restore(gstate);
			}
			base.BackFrame.RenderGlassEffect(g);
			RenderTopImage(g);
		}

		internal void RenderDynamicShadows(GaugeGraphics g)
		{
			using (GraphicsPath graphicsPath = new GraphicsPath())
			{
				foreach (LinearPointer pointer in Pointers)
				{
					GraphicsPath shadowPath = pointer.GetShadowPath(g);
					if (shadowPath != null)
					{
						graphicsPath.AddPath(shadowPath, connect: false);
					}
				}
				graphicsPath.FillMode = FillMode.Winding;
				using (Brush brush = g.GetShadowBrush())
				{
					g.FillPath(brush, graphicsPath);
				}
			}
		}

		internal void RenderStaticShadows(GaugeGraphics g)
		{
			using (GraphicsPath graphicsPath = new GraphicsPath())
			{
				foreach (LinearRange range in Ranges)
				{
					GraphicsPath path = range.GetPath(g, getShadowPath: true);
					if (path != null)
					{
						graphicsPath.AddPath(path, connect: false);
					}
				}
				foreach (LinearScale scale in Scales)
				{
					GraphicsPath shadowPath = scale.GetShadowPath();
					if (shadowPath != null)
					{
						graphicsPath.AddPath(shadowPath, connect: false);
					}
				}
				graphicsPath.FillMode = FillMode.Winding;
				using (Brush brush = g.GetShadowBrush())
				{
					g.FillPath(brush, graphicsPath);
				}
			}
		}

		void ISelectable.DrawSelection(GaugeGraphics g, bool designTimeSelection)
		{
			Stack stack = new Stack();
			for (NamedElement namedElement = base.ParentObject; namedElement != null; namedElement = (NamedElement)((IRenderable)namedElement).GetParentRenderable())
			{
				stack.Push(namedElement);
			}
			foreach (IRenderable item in stack)
			{
				g.CreateDrawRegion(item.GetBoundRect(g));
			}
			g.CreateDrawRegion(((IRenderable)this).GetBoundRect(g));
			g.DrawSelection(g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f)), -3f / g.Graphics.PageScale, designTimeSelection, Common.GaugeCore.SelectionBorderColor, Common.GaugeCore.SelectionMarkerColor);
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
			LinearGauge linearGauge = new LinearGauge();
			binaryFormatSerializer.Deserialize(linearGauge, stream);
			return linearGauge;
		}
	}
}
