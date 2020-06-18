using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

namespace Microsoft.Reporting.Gauge.WebForms
{
	[TypeConverter(typeof(CircularRangeConverter))]
	internal sealed class CircularRange : RangeBase, ISelectable
	{
		[Browsable(false)]
		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeStartValue")]
		[DefaultValue(70.0)]
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
		[SRDescription("DescriptionAttributeCircularRange_StartWidth")]
		[ValidateBound(0.0, 100.0)]
		[DefaultValue(15f)]
		public override float StartWidth
		{
			get
			{
				return base.StartWidth;
			}
			set
			{
				if (value < 0f || value > 1000f)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", 0, 1000));
				}
				base.StartWidth = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeCircularRange_EndWidth")]
		[ValidateBound(0.0, 100.0)]
		[DefaultValue(30f)]
		public override float EndWidth
		{
			get
			{
				return base.EndWidth;
			}
			set
			{
				if (value < 0f || value > 1000f)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", 0, 1000));
				}
				base.EndWidth = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeDistanceFromScale8")]
		[ValidateBound(0.0, 100.0)]
		[DefaultValue(30f)]
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

		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributePlacement7")]
		[DefaultValue(Placement.Inside)]
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

		public CircularRange()
		{
			base.StartValue = 70.0;
			base.EndValue = 100.0;
			base.StartWidth = 15f;
			base.EndWidth = 30f;
			base.DistanceFromScale = 30f;
			base.Placement = Placement.Inside;
		}

		public override string ToString()
		{
			return Name;
		}

		public CircularGauge GetGauge()
		{
			if (Collection == null)
			{
				return null;
			}
			return (CircularGauge)Collection.parent;
		}

		public CircularScale GetScale()
		{
			if (GetGauge() == null)
			{
				return null;
			}
			if (base.ScaleName == string.Empty)
			{
				return null;
			}
			if (base.ScaleName == "Default" && GetGauge().Scales.Count == 0)
			{
				return null;
			}
			return GetGauge().Scales[base.ScaleName];
		}

		internal override void Render(GaugeGraphics g)
		{
			if (Common == null || !base.Visible || GetScale() == null)
			{
				return;
			}
			Common.GaugeCore.TraceWrite("GaugePaint", SR.TraceStartRendering(Name));
			g.StartHotRegion(this);
			GetScale().SetDrawRegion(g);
			RectangleF rectangleF = CalculateRangeRectangle();
			CircularScale scale = GetScale();
			double valueLimit = scale.GetValueLimit(StartValue);
			double valueLimit2 = scale.GetValueLimit(EndValue);
			float positionFromValue = scale.GetPositionFromValue(valueLimit);
			float num = scale.GetPositionFromValue(valueLimit2) - positionFromValue;
			if (Math.Round(num, 4) == 0.0)
			{
				g.RestoreDrawRegion();
				g.EndHotRegion();
				Common.GaugeCore.TraceWrite("GaugePaint", SR.TraceRenderingComplete(Name));
				return;
			}
			Pen pen = null;
			Brush brush = null;
			GraphicsPath graphicsPath = null;
			try
			{
				graphicsPath = g.GetCircularRangePath(rectangleF, positionFromValue + 90f, num, StartWidth, EndWidth, Placement);
				if (graphicsPath == null)
				{
					g.RestoreDrawRegion();
					g.EndHotRegion();
					Common.GaugeCore.TraceWrite("GaugePaint", SR.TraceRenderingComplete(Name));
					return;
				}
				RectangleF rect = rectangleF;
				if (Placement != 0)
				{
					float num2 = StartWidth;
					if (num2 < EndWidth)
					{
						num2 = EndWidth;
					}
					if (Placement == Placement.Outside)
					{
						rect.Inflate(num2, num2);
					}
					else
					{
						rect.Inflate(num2 / 2f, num2 / 2f);
					}
				}
				RangeGradientType fillGradientType = base.FillGradientType;
				brush = g.GetCircularRangeBrush(rect, positionFromValue + 90f, num, base.FillColor, base.FillHatchStyle, "", GaugeImageWrapMode.Unscaled, Color.Empty, GaugeImageAlign.TopLeft, fillGradientType, base.FillGradientEndColor);
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
				g.RestoreDrawRegion();
				throw;
			}
			Common.GaugeCore.HotRegionList.SetHotRegion(this, g.GetAbsolutePoint(GetScale().GetPivotPoint()), graphicsPath);
			g.RestoreDrawRegion();
			g.EndHotRegion();
			Common.GaugeCore.TraceWrite("GaugePaint", SR.TraceRenderingComplete(Name));
		}

		internal RectangleF CalculateRangeRectangle()
		{
			CircularScale scale = GetScale();
			PointF pivotPoint = GetScale().GetPivotPoint();
			float radius = GetScale().GetRadius();
			RectangleF result = new RectangleF(pivotPoint.X - radius, pivotPoint.Y - radius, radius * 2f, radius * 2f);
			if (Placement == Placement.Outside)
			{
				result.Inflate(DistanceFromScale, DistanceFromScale);
				result.Inflate(scale.Width / 2f, scale.Width / 2f);
			}
			else if (Placement == Placement.Inside)
			{
				result.Inflate(0f - DistanceFromScale, 0f - DistanceFromScale);
				result.Inflate((0f - scale.Width) / 2f, (0f - scale.Width) / 2f);
			}
			else
			{
				result.Inflate(0f - DistanceFromScale, 0f - DistanceFromScale);
			}
			return result;
		}

		internal GraphicsPath GetPath(GaugeGraphics g, bool getShadowPath)
		{
			if (getShadowPath && (!base.Visible || base.ShadowOffset == 0f))
			{
				return null;
			}
			GetScale().SetDrawRegion(g);
			CircularScale scale = GetScale();
			RectangleF rect = CalculateRangeRectangle();
			double valueLimit = scale.GetValueLimit(StartValue);
			double valueLimit2 = scale.GetValueLimit(EndValue);
			float positionFromValue = scale.GetPositionFromValue(valueLimit);
			float positionFromValue2 = scale.GetPositionFromValue(valueLimit2);
			if (Math.Round(positionFromValue2 - positionFromValue, 4) == 0.0)
			{
				g.RestoreDrawRegion();
				return null;
			}
			GraphicsPath circularRangePath = g.GetCircularRangePath(rect, positionFromValue + 90f, positionFromValue2 - positionFromValue, StartWidth, EndWidth, Placement);
			if (circularRangePath != null && getShadowPath)
			{
				using (Matrix matrix = new Matrix())
				{
					matrix.Translate(base.ShadowOffset, base.ShadowOffset);
					circularRangePath.Transform(matrix);
				}
			}
			PointF pointF = new PointF(g.Graphics.Transform.OffsetX, g.Graphics.Transform.OffsetY);
			g.RestoreDrawRegion();
			PointF pointF2 = new PointF(g.Graphics.Transform.OffsetX, g.Graphics.Transform.OffsetY);
			if (circularRangePath != null)
			{
				using (Matrix matrix2 = new Matrix())
				{
					matrix2.Translate(pointF.X - pointF2.X, pointF.Y - pointF2.Y);
					circularRangePath.Transform(matrix2);
					return circularRangePath;
				}
			}
			return circularRangePath;
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
			CircularRange circularRange = new CircularRange();
			binaryFormatSerializer.Deserialize(circularRange, stream);
			return circularRange;
		}
	}
}
