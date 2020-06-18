using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Microsoft.Reporting.Gauge.WebForms
{
	[TypeConverter(typeof(CircularGaugeConverter))]
	internal sealed class CircularGauge : GaugeBase, ISelectable
	{
		internal RectangleF absoluteRect = RectangleF.Empty;

		private CircularScaleCollection scales;

		private CircularRangeCollection ranges;

		private CircularPointerCollection pointers;

		private KnobCollection knobs;

		private GaugeLocation pivotPoint;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CircularScaleCollection Scales => scales;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CircularRangeCollection Ranges => ranges;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CircularPointerCollection Pointers => pointers;

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public KnobCollection Knobs => knobs;

		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeCircularGauge_PivotPoint")]
		[TypeConverter(typeof(LocationConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[DefaultValue(typeof(GaugeLocation), "50F, 50F")]
		[ValidateBound(100.0, 100.0)]
		public GaugeLocation PivotPoint
		{
			get
			{
				if (pivotPoint == null)
				{
					pivotPoint = new GaugeLocation(this, 50f, 50f);
				}
				return pivotPoint;
			}
			set
			{
				pivotPoint = value;
				pivotPoint.Parent = this;
				Invalidate();
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
				Knobs.Common = value;
			}
		}

		public CircularGauge()
		{
			base.BackFrame = new BackFrame(this, BackFrameStyle.Edged, BackFrameShape.Circular);
			scales = new CircularScaleCollection(this, Common);
			ranges = new CircularRangeCollection(this, Common);
			pointers = new CircularPointerCollection(this, Common);
			knobs = new KnobCollection(this, Common);
		}

		public override string ToString()
		{
			return Name;
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
			if (!float.IsNaN(base.AspectRatio))
			{
				if (base.AspectRatio >= 1f)
				{
					float width = rectangle.Width;
					rectangle.Width /= base.AspectRatio;
					rectangle.X += (width - rectangle.Width) / 2f;
				}
				else
				{
					float height = rectangle.Height;
					rectangle.Height *= base.AspectRatio;
					rectangle.Y += (height - rectangle.Height) / 2f;
				}
			}
			if (rectangle.Width != rectangle.Height)
			{
				if (rectangle.Width > rectangle.Height)
				{
					rectangle.Offset((rectangle.Width - rectangle.Height) / 2f, 0f);
					rectangle.Width = rectangle.Height;
				}
				else if (rectangle.Width < rectangle.Height)
				{
					rectangle.Offset(0f, (rectangle.Height - rectangle.Width) / 2f);
					rectangle.Height = rectangle.Width;
				}
			}
			rectangle.X /= num;
			rectangle.Y /= num2;
			rectangle.Width /= num;
			rectangle.Height /= num2;
			return rectangle;
		}

		internal override void BeginInit()
		{
			base.BeginInit();
			Scales.BeginInit();
			Ranges.BeginInit();
			Pointers.BeginInit();
			Knobs.BeginInit();
		}

		internal override void EndInit()
		{
			base.EndInit();
			Scales.EndInit();
			Ranges.EndInit();
			Pointers.EndInit();
			Knobs.EndInit();
		}

		protected override void OnDispose()
		{
			base.OnDispose();
			Scales.Dispose();
			Ranges.Dispose();
			Pointers.Dispose();
			Knobs.Dispose();
		}

		internal override void ReconnectData(bool exact)
		{
			base.ReconnectData(exact);
			Pointers.ReconnectData(exact);
			Knobs.ReconnectData(exact);
		}

		internal override void Notify(MessageType msg, NamedElement element, object param)
		{
			base.Notify(msg, element, param);
			Scales.Notify(msg, element, param);
			Ranges.Notify(msg, element, param);
			Pointers.Notify(msg, element, param);
			Knobs.Notify(msg, element, param);
		}

		internal override IEnumerable GetRanges()
		{
			return ranges;
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

		internal override RectangleF GetBoundRect(GaugeGraphics g)
		{
			if (Common != null)
			{
				RectangleF absolute = absoluteRect = g.GetAbsoluteRectangle(base.Position.Rectangle);
				if (!float.IsNaN(base.AspectRatio))
				{
					if (base.AspectRatio >= 1f)
					{
						float width = absolute.Width;
						absolute.Width /= base.AspectRatio;
						absolute.X += (width - absolute.Width) / 2f;
					}
					else
					{
						float height = absolute.Height;
						absolute.Height *= base.AspectRatio;
						absolute.Y += (height - absolute.Height) / 2f;
					}
				}
				if (absolute.Width != absolute.Height)
				{
					if (absolute.Width > absolute.Height)
					{
						absolute.Offset((absolute.Width - absolute.Height) / 2f, 0f);
						absolute.Width = absolute.Height;
					}
					else if (absolute.Width < absolute.Height)
					{
						absolute.Offset(0f, (absolute.Height - absolute.Width) / 2f);
						absolute.Height = absolute.Width;
					}
				}
				return g.GetRelativeRectangle(absolute);
			}
			return base.Position.Rectangle;
		}

		internal override void RenderStaticElements(GaugeGraphics g)
		{
			if (!base.Visible)
			{
				return;
			}
			g.StartHotRegion(this);
			base.BackFrame.RenderFrame(g);
			GraphicsState gstate = g.Save();
			if (base.ClipContent && base.BackFrame.FrameStyle != 0 && base.BackFrame.Image == string.Empty)
			{
				GraphicsPath graphicsPath = null;
				try
				{
					graphicsPath = base.BackFrame.GetBackPath(g);
					g.SetClip(graphicsPath, CombineMode.Intersect);
				}
				finally
				{
					graphicsPath?.Dispose();
				}
			}
			Common.GaugeCore.HotRegionList.SetHotRegion(this, g.GetAbsolutePoint(PivotPoint.ToPoint()), base.BackFrame.GetFramePath(g, 0f));
			g.EndHotRegion();
			g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f));
			RenderStaticShadows(g);
			foreach (CircularRange range in Ranges)
			{
				range.Render(g);
			}
			foreach (CircularScale scale in Scales)
			{
				scale.RenderStaticElements(g);
			}
			foreach (CircularPointer pointer in Pointers)
			{
				pointer.ResetCachedXamlRenderer();
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
			foreach (Knob knob in Knobs)
			{
				knob.Render(g);
			}
			foreach (CircularPointer pointer in Pointers)
			{
				pointer.Render(g);
			}
			foreach (CircularScale scale in Scales)
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
				foreach (Knob knob in Knobs)
				{
					GraphicsPath shadowPath = knob.GetShadowPath(g);
					if (shadowPath != null)
					{
						graphicsPath.AddPath(shadowPath, connect: false);
					}
				}
				foreach (CircularPointer pointer in Pointers)
				{
					GraphicsPath shadowPath2 = pointer.GetShadowPath(g);
					if (shadowPath2 != null)
					{
						graphicsPath.AddPath(shadowPath2, connect: false);
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
				foreach (CircularRange range in Ranges)
				{
					GraphicsPath path = range.GetPath(g, getShadowPath: true);
					if (path != null)
					{
						graphicsPath.AddPath(path, connect: false);
					}
				}
				foreach (CircularScale scale in Scales)
				{
					GraphicsPath shadowPath = scale.GetShadowPath(g);
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
			RectangleF frameRectangle = base.Frame.GetFrameRectangle(g);
			g.DrawSelection(frameRectangle, -3f / g.Graphics.PageScale, designTimeSelection, Common.GaugeCore.SelectionBorderColor, Common.GaugeCore.SelectionMarkerColor);
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
			CircularGauge circularGauge = new CircularGauge();
			binaryFormatSerializer.Deserialize(circularGauge, stream);
			return circularGauge;
		}
	}
}
