using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

namespace Microsoft.Reporting.Gauge.WebForms
{
	[TypeConverter(typeof(LinearRangeConverter))]
	internal sealed class LinearRange : RangeBase, ISelectable
	{
		[Browsable(false)]
		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeStartValue")]
		[DefaultValue(60.0)]
		public override double StartValue
		{
			get
			{
				return base.StartValue;
			}
			set
			{
				base.StartValue = value;
				Invalidate();
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeEndValue3")]
		[DefaultValue(100.0)]
		public override double EndValue
		{
			get
			{
				return base.EndValue;
			}
			set
			{
				base.EndValue = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeLinearRange_StartWidth")]
		[ValidateBound(0.0, 100.0)]
		[DefaultValue(10f)]
		public override float StartWidth
		{
			get
			{
				return base.StartWidth;
			}
			set
			{
				if (value < 0f || value > 100f)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", 0, 100));
				}
				base.StartWidth = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeLinearRange_EndWidth")]
		[ValidateBound(0.0, 100.0)]
		[DefaultValue(10f)]
		public override float EndWidth
		{
			get
			{
				return base.EndWidth;
			}
			set
			{
				if (value < 0f || value > 100f)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", 0, 100));
				}
				base.EndWidth = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributePlacement7")]
		[DefaultValue(Placement.Outside)]
		public override Placement Placement
		{
			get
			{
				return base.Placement;
			}
			set
			{
				base.Placement = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeDistanceFromScale8")]
		[ValidateBound(0.0, 100.0)]
		[DefaultValue(10f)]
		public override float DistanceFromScale
		{
			get
			{
				return base.DistanceFromScale;
			}
			set
			{
				if (value < -100f || value > 100f)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", -100, 100));
				}
				base.DistanceFromScale = value;
			}
		}

		public LinearRange()
		{
			base.StartValue = 60.0;
			base.EndValue = 100.0;
			base.StartWidth = 10f;
			base.EndWidth = 10f;
			base.DistanceFromScale = 10f;
			base.Placement = Placement.Outside;
		}

		public override string ToString()
		{
			return Name;
		}

		public LinearGauge GetGauge()
		{
			if (Collection == null)
			{
				return null;
			}
			return (LinearGauge)Collection.parent;
		}

		public LinearScale GetScale()
		{
			if (GetGauge() == null)
			{
				return null;
			}
			LinearScale linearScale = null;
			try
			{
				return GetGauge().Scales[base.ScaleName];
			}
			catch
			{
				return null;
			}
		}

		internal override void Render(GaugeGraphics g)
		{
			if (Common == null || !base.Visible || GetScale() == null || double.IsNaN(StartValue) || double.IsNaN(EndValue))
			{
				return;
			}
			Common.GaugeCore.TraceWrite("GaugePaint", SR.TraceStartRendering(Name));
			g.StartHotRegion(this);
			LinearScale scale = GetScale();
			Pen pen = null;
			Brush brush = null;
			GraphicsPath graphicsPath = null;
			try
			{
				graphicsPath = g.GetLinearRangePath(scale.GetPositionFromValue(StartValue), scale.GetPositionFromValue(EndValue), StartWidth, EndWidth, scale.Position, GetGauge().GetOrientation(), DistanceFromScale, Placement, scale.Width);
				if (graphicsPath == null || !g.Graphics.VisibleClipBounds.IntersectsWith(graphicsPath.GetBounds()))
				{
					g.EndHotRegion();
					Common.GaugeCore.TraceWrite("GaugePaint", SR.TraceRenderingComplete(Name));
					return;
				}
				brush = g.GetLinearRangeBrush(graphicsPath.GetBounds(), base.FillColor, base.FillHatchStyle, base.FillGradientType, base.FillGradientEndColor, GetGauge().GetOrientation(), GetScale().GetReversed(), StartValue, EndValue);
				pen = new Pen(base.BorderColor, base.BorderWidth);
				pen.DashStyle = g.GetPenStyle(base.BorderStyle);
				g.FillPath(brush, graphicsPath);
				if (base.BorderStyle != 0 && base.BorderWidth > 0)
				{
					g.DrawPath(pen, graphicsPath);
				}
			}
			catch (Exception)
			{
				graphicsPath?.Dispose();
				pen?.Dispose();
				brush?.Dispose();
				throw;
			}
			Common.GaugeCore.HotRegionList.SetHotRegion(this, PointF.Empty, graphicsPath);
			g.EndHotRegion();
			Common.GaugeCore.TraceWrite("GaugePaint", SR.TraceRenderingComplete(Name));
		}

		internal GraphicsPath GetPath(GaugeGraphics g, bool getShadowPath)
		{
			if (getShadowPath && (!base.Visible || base.ShadowOffset == 0f))
			{
				return null;
			}
			if (double.IsNaN(StartValue) || double.IsNaN(EndValue))
			{
				return null;
			}
			LinearScale scale = GetScale();
			GraphicsPath linearRangePath = g.GetLinearRangePath(scale.GetPositionFromValue(StartValue), scale.GetPositionFromValue(EndValue), StartWidth, EndWidth, scale.Position, GetGauge().GetOrientation(), DistanceFromScale, Placement, scale.Width);
			if (getShadowPath)
			{
				using (Matrix matrix = new Matrix())
				{
					matrix.Translate(base.ShadowOffset, base.ShadowOffset);
					linearRangePath.Transform(matrix);
					return linearRangePath;
				}
			}
			return linearRangePath;
		}

		void ISelectable.DrawSelection(GaugeGraphics g, bool designTimeSelection)
		{
			if (GetGauge() == null || GetScale() == null)
			{
				return;
			}
			Stack stack = new Stack();
			for (NamedElement namedElement = GetGauge().ParentObject; namedElement != null; namedElement = (NamedElement)((IRenderable)namedElement).GetParentRenderable())
			{
				stack.Push(namedElement);
			}
			foreach (IRenderable item in stack)
			{
				g.CreateDrawRegion(item.GetBoundRect(g));
			}
			g.CreateDrawRegion(((IRenderable)GetGauge()).GetBoundRect(g));
			using (GraphicsPath graphicsPath = GetPath(g, getShadowPath: false))
			{
				if (graphicsPath != null)
				{
					RectangleF bounds = graphicsPath.GetBounds();
					g.DrawSelection(bounds, designTimeSelection, Common.GaugeCore.SelectionBorderColor, Common.GaugeCore.SelectionMarkerColor);
				}
			}
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
			LinearRange linearRange = new LinearRange();
			binaryFormatSerializer.Deserialize(linearRange, stream);
			return linearRange;
		}
	}
}
