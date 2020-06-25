using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Microsoft.Reporting.Gauge.WebForms
{
	[TypeConverter(typeof(LinearPointerConverter))]
	internal class LinearPointer : PointerBase, ISelectable
	{
		private LinearPointerType type;

		private Placement placement = Placement.Outside;

		private float thermometerBulbOffset = 5f;

		private float thermometerBulbSize = 50f;

		private Color thermometerBackColor = Color.Empty;

		private GradientType thermometerBackGradientType;

		private Color thermometerBackGradientEndColor = Color.Empty;

		private GaugeHatchStyle thermometerBackHatchStyle;

		private ThermometerStyle thermometerStyle;

		private bool selected;

		private GraphicsPath[] hotRegions = new GraphicsPath[2];

		[SRCategory("CategoryTypeSpecific")]
		[SRDescription("DescriptionAttributeLinearPointer_Type")]
		[ParenthesizePropertyName(true)]
		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue(LinearPointerType.Marker)]
		public LinearPointerType Type
		{
			get
			{
				return type;
			}
			set
			{
				type = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributePlacement")]
		[DefaultValue(Placement.Outside)]
		public Placement Placement
		{
			get
			{
				return placement;
			}
			set
			{
				placement = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeWidth")]
		[ValidateBound(0.0, 100.0)]
		[DefaultValue(20f)]
		public override float Width
		{
			get
			{
				return base.Width;
			}
			set
			{
				if (value < 0f || value > 200f)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", 0, 200));
				}
				base.Width = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryTypeSpecific")]
		[SRDescription("DescriptionAttributeMarkerStyle4")]
		[DefaultValue(MarkerStyle.Triangle)]
		public override MarkerStyle MarkerStyle
		{
			get
			{
				return base.MarkerStyle;
			}
			set
			{
				base.MarkerStyle = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryTypeSpecific")]
		[SRDescription("DescriptionAttributeMarkerLength4")]
		[ValidateBound(0.0, 100.0)]
		[DefaultValue(20f)]
		public override float MarkerLength
		{
			get
			{
				return base.MarkerLength;
			}
			set
			{
				if (value < 0f || value > 200f)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", 0, 200));
				}
				base.MarkerLength = value;
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeFillGradientType6")]
		[DefaultValue(GradientType.DiagonalLeft)]
		public override GradientType FillGradientType
		{
			get
			{
				return base.FillGradientType;
			}
			set
			{
				base.FillGradientType = value;
			}
		}

		[SRCategory("CategoryTypeSpecific")]
		[SRDescription("DescriptionAttributeLinearPointer_ThermometerBulbOffset")]
		[ValidateBound(0.0, 30.0)]
		[DefaultValue(5f)]
		public float ThermometerBulbOffset
		{
			get
			{
				return thermometerBulbOffset;
			}
			set
			{
				if (value < 0f || value > 100f)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", 0, 100));
				}
				thermometerBulbOffset = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryTypeSpecific")]
		[SRDescription("DescriptionAttributeLinearPointer_ThermometerBulbSize")]
		[ValidateBound(0.0, 100.0)]
		[DefaultValue(50f)]
		public float ThermometerBulbSize
		{
			get
			{
				return thermometerBulbSize;
			}
			set
			{
				if (value < 0f || value > 1000f)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", 0, 1000));
				}
				thermometerBulbSize = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryTypeSpecific")]
		[SRDescription("DescriptionAttributeLinearPointer_ThermometerBackColor")]
		[DefaultValue(typeof(Color), "Empty")]
		public Color ThermometerBackColor
		{
			get
			{
				return thermometerBackColor;
			}
			set
			{
				thermometerBackColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryTypeSpecific")]
		[SRDescription("DescriptionAttributeLinearPointer_ThermometerBackGradientType")]
		[DefaultValue(GradientType.None)]
		public GradientType ThermometerBackGradientType
		{
			get
			{
				return thermometerBackGradientType;
			}
			set
			{
				thermometerBackGradientType = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryTypeSpecific")]
		[SRDescription("DescriptionAttributeLinearPointer_ThermometerBackGradientEndColor")]
		[DefaultValue(typeof(Color), "Empty")]
		public Color ThermometerBackGradientEndColor
		{
			get
			{
				return thermometerBackGradientEndColor;
			}
			set
			{
				thermometerBackGradientEndColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryTypeSpecific")]
		[SRDescription("DescriptionAttributeLinearPointer_ThermometerBackHatchStyle")]
		[DefaultValue(GaugeHatchStyle.None)]
		public GaugeHatchStyle ThermometerBackHatchStyle
		{
			get
			{
				return thermometerBackHatchStyle;
			}
			set
			{
				thermometerBackHatchStyle = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryTypeSpecific")]
		[SRDescription("DescriptionAttributeLinearPointer_ThermometerStyle")]
		[DefaultValue(ThermometerStyle.Standard)]
		public ThermometerStyle ThermometerStyle
		{
			get
			{
				return thermometerStyle;
			}
			set
			{
				thermometerStyle = value;
				Invalidate();
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeCursor")]
		[DefaultValue(GaugeCursor.Default)]
		public override GaugeCursor Cursor
		{
			get
			{
				return base.Cursor;
			}
			set
			{
				base.Cursor = value;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeDampeningEnabled3")]
		[DefaultValue(false)]
		public override bool DampeningEnabled
		{
			get
			{
				return base.DampeningEnabled;
			}
			set
			{
				base.DampeningEnabled = value;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeSelected")]
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

		public LinearPointer()
			: base(MarkerStyle.Triangle, 20f, 20f, GradientType.DiagonalLeft)
		{
		}

		internal override void Render(GaugeGraphics g)
		{
			if (Common == null || !Visible || GetScale() == null)
			{
				return;
			}
			Common.GaugeCore.TraceWrite("GaugePaint", SR.TraceStartRendering(Name));
			g.StartHotRegion(this);
			if (Image != "")
			{
				DrawImage(g, drawShadow: false);
				SetAllHotRegions(g);
				g.EndHotRegion();
				Common.GaugeCore.TraceWrite("GaugePaint", SR.TraceRenderingComplete(Name));
				return;
			}
			Pen pen = new Pen(base.BorderColor, base.BorderWidth);
			pen.DashStyle = g.GetPenStyle(base.BorderStyle);
			if (pen.DashStyle != 0)
			{
				pen.Alignment = PenAlignment.Center;
			}
			if (Type == LinearPointerType.Bar)
			{
				BarStyleAttrib barStyleAttrib = GetBarStyleAttrib(g);
				try
				{
					if (barStyleAttrib.primaryPath != null)
					{
						g.FillPath(barStyleAttrib.primaryBrush, barStyleAttrib.primaryPath);
					}
					if (barStyleAttrib.secondaryPaths != null)
					{
						int num = 0;
						GraphicsPath[] secondaryPaths = barStyleAttrib.secondaryPaths;
						foreach (GraphicsPath graphicsPath in secondaryPaths)
						{
							if (graphicsPath != null && barStyleAttrib.secondaryBrushes[num] != null)
							{
								g.FillPath(barStyleAttrib.secondaryBrushes[num], graphicsPath);
							}
							num++;
						}
					}
					if (base.BorderWidth > 0 && barStyleAttrib.primaryBrush != null && base.BorderStyle != 0)
					{
						g.DrawPath(pen, barStyleAttrib.primaryPath);
					}
				}
				catch (Exception)
				{
					barStyleAttrib.Dispose();
				}
				if (barStyleAttrib.primaryPath != null)
				{
					AddHotRegion(barStyleAttrib.primaryPath, primary: true);
				}
			}
			else if (Type == LinearPointerType.Thermometer)
			{
				BarStyleAttrib thermometerStyleAttrib = GetThermometerStyleAttrib(g);
				try
				{
					if (thermometerStyleAttrib.totalPath != null)
					{
						g.FillPath(thermometerStyleAttrib.totalBrush, thermometerStyleAttrib.totalPath);
					}
					if (thermometerStyleAttrib.primaryPath != null)
					{
						g.FillPath(thermometerStyleAttrib.primaryBrush, thermometerStyleAttrib.primaryPath);
					}
					if (thermometerStyleAttrib.secondaryPaths != null)
					{
						int num2 = 0;
						GraphicsPath[] secondaryPaths = thermometerStyleAttrib.secondaryPaths;
						foreach (GraphicsPath graphicsPath2 in secondaryPaths)
						{
							if (graphicsPath2 != null && thermometerStyleAttrib.secondaryBrushes[num2] != null)
							{
								g.FillPath(thermometerStyleAttrib.secondaryBrushes[num2], graphicsPath2);
							}
							num2++;
						}
					}
					if (base.BorderWidth > 0 && thermometerStyleAttrib.primaryBrush != null && base.BorderStyle != 0)
					{
						g.DrawPath(pen, thermometerStyleAttrib.totalPath);
					}
				}
				catch (Exception)
				{
					thermometerStyleAttrib.Dispose();
				}
				if (thermometerStyleAttrib.primaryPath != null)
				{
					AddHotRegion(thermometerStyleAttrib.primaryPath, primary: true);
				}
			}
			else
			{
				MarkerStyleAttrib markerStyleAttrib = GetMarkerStyleAttrib(g);
				try
				{
					if (markerStyleAttrib.path != null)
					{
						bool circularFill = (MarkerStyle == MarkerStyle.Circle) ? true : false;
						g.FillPath(markerStyleAttrib.brush, markerStyleAttrib.path, 0f, useBrushOffset: true, circularFill);
					}
					if (base.BorderWidth > 0 && markerStyleAttrib.path != null && base.BorderStyle != 0)
					{
						g.DrawPath(pen, markerStyleAttrib.path);
					}
				}
				catch (Exception)
				{
					markerStyleAttrib.Dispose();
				}
				if (markerStyleAttrib.path != null)
				{
					AddHotRegion(markerStyleAttrib.path, primary: true);
				}
			}
			SetAllHotRegions(g);
			g.EndHotRegion();
			Common.GaugeCore.TraceWrite("GaugePaint", SR.TraceRenderingComplete(Name));
		}

		internal BarStyleAttrib GetBarStyleAttrib(GaugeGraphics g)
		{
			BarStyleAttrib barStyleAttrib = new BarStyleAttrib();
			if (Image != "")
			{
				return barStyleAttrib;
			}
			LinearScale scale = GetScale();
			double num = scale.GetValueLimit(GetBarStartValue());
			if ((Type == LinearPointerType.Thermometer || BarStart == BarStart.ScaleStart) && num > scale.Minimum)
			{
				num = scale.Minimum;
			}
			double valueLimit = scale.GetValueLimit(base.Position);
			float positionFromValue = scale.GetPositionFromValue(num);
			_ = ThermometerBulbOffset;
			float positionFromValue2 = scale.GetPositionFromValue(valueLimit);
			if (Math.Round(positionFromValue2 - positionFromValue, 4) == 0.0)
			{
				return barStyleAttrib;
			}
			if (base.BarStyle == BarStyle.Style1)
			{
				barStyleAttrib.primaryPath = g.GetLinearRangePath(positionFromValue, positionFromValue2, Width, Width, scale.Position, GetGauge().GetOrientation(), DistanceFromScale, Placement, scale.Width);
				if (barStyleAttrib.primaryPath == null)
				{
					return barStyleAttrib;
				}
				barStyleAttrib.primaryBrush = g.GetLinearRangeBrush(barStyleAttrib.primaryPath.GetBounds(), FillColor, FillHatchStyle, (RangeGradientType)Enum.Parse(typeof(RangeGradientType), FillGradientType.ToString()), FillGradientEndColor, GetGauge().GetOrientation(), GetScale().GetReversed(), 0.0, 0.0);
				LinearRange[] colorRanges = GetColorRanges();
				if (colorRanges != null)
				{
					barStyleAttrib.secondaryPaths = new GraphicsPath[colorRanges.Length];
					barStyleAttrib.secondaryBrushes = new Brush[colorRanges.Length];
					int num2 = 0;
					LinearRange[] array = colorRanges;
					foreach (LinearRange linearRange in array)
					{
						double num3 = scale.GetValueLimit(linearRange.StartValue);
						if (num3 < num)
						{
							num3 = num;
						}
						if (num3 > valueLimit)
						{
							num3 = valueLimit;
						}
						double num4 = scale.GetValueLimit(linearRange.EndValue);
						if (num4 < num)
						{
							num4 = num;
						}
						if (num4 > valueLimit)
						{
							num4 = valueLimit;
						}
						float positionFromValue3 = scale.GetPositionFromValue(num3);
						float positionFromValue4 = scale.GetPositionFromValue(num4);
						if (Math.Round(positionFromValue4 - positionFromValue3, 4) == 0.0)
						{
							barStyleAttrib.secondaryPaths[num2] = null;
							barStyleAttrib.secondaryBrushes[num2] = null;
						}
						else
						{
							barStyleAttrib.secondaryPaths[num2] = g.GetLinearRangePath(positionFromValue3, positionFromValue4, Width, Width, scale.Position, GetGauge().GetOrientation(), DistanceFromScale, Placement, scale.Width);
							barStyleAttrib.secondaryBrushes[num2] = g.GetLinearRangeBrush(barStyleAttrib.primaryPath.GetBounds(), linearRange.InRangeBarPointerColor, FillHatchStyle, (RangeGradientType)Enum.Parse(typeof(RangeGradientType), FillGradientType.ToString()), FillGradientEndColor, GetGauge().GetOrientation(), GetScale().GetReversed(), 0.0, 0.0);
						}
						num2++;
					}
				}
			}
			return barStyleAttrib;
		}

		internal BarStyleAttrib GetThermometerStyleAttrib(GaugeGraphics g)
		{
			BarStyleAttrib barStyleAttrib = new BarStyleAttrib();
			if (Image != "")
			{
				return barStyleAttrib;
			}
			LinearScale scale = GetScale();
			double num = scale.GetValueLimit(GetBarStartValue());
			if ((Type == LinearPointerType.Thermometer || BarStart == BarStart.ScaleStart) && num > scale.MinimumLog)
			{
				num = scale.MinimumLog;
			}
			double valueLimit = scale.GetValueLimit(base.Position);
			float positionFromValue = scale.GetPositionFromValue(num);
			float positionFromValue2 = scale.GetPositionFromValue(valueLimit);
			float num2 = positionFromValue2 - positionFromValue;
			float width = Width;
			float bulbSize = ThermometerBulbSize;
			float bulbOffset = thermometerBulbOffset;
			float distanceFromScale = DistanceFromScale;
			if (Math.Round(num2, 4) == 0.0 && Type != LinearPointerType.Thermometer)
			{
				return barStyleAttrib;
			}
			double num3 = scale.GetValueLimit(double.PositiveInfinity);
			if (num3 < scale.Maximum)
			{
				num3 = scale.Maximum;
			}
			float positionFromValue3 = scale.GetPositionFromValue(num3);
			barStyleAttrib.primaryPath = g.GetThermometerPath(positionFromValue, positionFromValue2, width, scale.Position, GetGauge().GetOrientation(), distanceFromScale, Placement, scale.GetReversed(), scale.Width, bulbOffset, bulbSize, ThermometerStyle);
			if (barStyleAttrib.primaryPath == null)
			{
				return barStyleAttrib;
			}
			barStyleAttrib.totalPath = g.GetThermometerPath(positionFromValue, positionFromValue3, Width, scale.Position, GetGauge().GetOrientation(), DistanceFromScale, Placement, scale.GetReversed(), scale.Width, ThermometerBulbOffset, ThermometerBulbSize, ThermometerStyle);
			barStyleAttrib.totalBrush = g.GetLinearRangeBrush(barStyleAttrib.totalPath.GetBounds(), ThermometerBackColor, ThermometerBackHatchStyle, (RangeGradientType)Enum.Parse(typeof(RangeGradientType), ThermometerBackGradientType.ToString()), ThermometerBackGradientEndColor, GetGauge().GetOrientation(), GetScale().GetReversed(), 0.0, 0.0);
			barStyleAttrib.primaryBrush = g.GetLinearRangeBrush(barStyleAttrib.primaryPath.GetBounds(), FillColor, FillHatchStyle, (RangeGradientType)Enum.Parse(typeof(RangeGradientType), FillGradientType.ToString()), FillGradientEndColor, GetGauge().GetOrientation(), GetScale().GetReversed(), 0.0, 0.0);
			LinearRange[] colorRanges = GetColorRanges();
			if (colorRanges != null)
			{
				barStyleAttrib.secondaryPaths = new GraphicsPath[colorRanges.Length];
				barStyleAttrib.secondaryBrushes = new Brush[colorRanges.Length];
				int num4 = 0;
				LinearRange[] array = colorRanges;
				foreach (LinearRange linearRange in array)
				{
					double num5 = scale.GetValueLimit(linearRange.StartValue);
					if (num5 < num)
					{
						num5 = num;
					}
					if (num5 > valueLimit)
					{
						num5 = valueLimit;
					}
					double num6 = scale.GetValueLimit(linearRange.EndValue);
					if (num6 < num)
					{
						num6 = num;
					}
					if (num6 > valueLimit)
					{
						num6 = valueLimit;
					}
					float positionFromValue4 = scale.GetPositionFromValue(num5);
					float positionFromValue5 = scale.GetPositionFromValue(num6);
					if (Math.Round(positionFromValue5 - positionFromValue4, 4) == 0.0)
					{
						barStyleAttrib.secondaryPaths[num4] = null;
						barStyleAttrib.secondaryBrushes[num4] = null;
					}
					else
					{
						barStyleAttrib.secondaryPaths[num4] = g.GetLinearRangePath(positionFromValue4, positionFromValue5, width, width, scale.Position, GetGauge().GetOrientation(), distanceFromScale, Placement, scale.Width);
						barStyleAttrib.secondaryBrushes[num4] = g.GetLinearRangeBrush(barStyleAttrib.primaryPath.GetBounds(), linearRange.InRangeBarPointerColor, FillHatchStyle, (RangeGradientType)Enum.Parse(typeof(RangeGradientType), FillGradientType.ToString()), FillGradientEndColor, GetGauge().GetOrientation(), GetScale().GetReversed(), 0.0, 0.0);
					}
					num4++;
				}
			}
			return barStyleAttrib;
		}

		private LinearRange[] GetColorRanges()
		{
			LinearGauge gauge = GetGauge();
			LinearScale scale = GetScale();
			if (gauge == null || scale == null)
			{
				return null;
			}
			double barStartValue = GetBarStartValue();
			double position = base.Position;
			ArrayList arrayList = null;
			foreach (LinearRange range in gauge.Ranges)
			{
				if (range.GetScale() != scale || !(range.InRangeBarPointerColor != Color.Empty))
				{
					continue;
				}
				double valueLimit = scale.GetValueLimit(range.StartValue);
				double valueLimit2 = scale.GetValueLimit(range.EndValue);
				if ((barStartValue >= valueLimit && barStartValue <= valueLimit2) || (position >= valueLimit && position <= valueLimit2) || (valueLimit >= barStartValue && valueLimit <= position) || (valueLimit2 >= barStartValue && valueLimit2 <= position))
				{
					if (arrayList == null)
					{
						arrayList = new ArrayList();
					}
					arrayList.Add(range);
				}
			}
			if (arrayList == null)
			{
				return null;
			}
			return (LinearRange[])arrayList.ToArray(typeof(LinearRange));
		}

		private double GetBarStartValue()
		{
			LinearScale scale = GetScale();
			if (Type == LinearPointerType.Thermometer)
			{
				return double.NegativeInfinity;
			}
			if (BarStart == BarStart.ScaleStart)
			{
				return double.NegativeInfinity;
			}
			if (scale.Logarithmic)
			{
				return 1.0;
			}
			return 0.0;
		}

		internal void DrawImage(GaugeGraphics g, bool drawShadow)
		{
			if (!Visible || (drawShadow && base.ShadowOffset == 0f))
			{
				return;
			}
			Image image = Common.ImageLoader.LoadImage(Image);
			if (image.Width == 0 || image.Height == 0)
			{
				return;
			}
			Point point = new Point(ImageOrigin.X, ImageOrigin.Y);
			if (point.X == 0)
			{
				point.X = image.Width / 2;
			}
			if (point.Y == 0)
			{
				point.Y = image.Height / 2;
			}
			float absoluteDimension = g.GetAbsoluteDimension(Width);
			float absoluteDimension2 = g.GetAbsoluteDimension(MarkerLength);
			float num = absoluteDimension / (float)image.Width;
			float num2 = absoluteDimension2 / (float)image.Height;
			float num3 = CalculateMarkerDistance();
			float positionFromValue = GetScale().GetPositionFromValue(base.Position);
			PointF pointF = Point.Empty;
			pointF = ((GetGauge().GetOrientation() != 0) ? g.GetAbsolutePoint(new PointF(num3, positionFromValue)) : g.GetAbsolutePoint(new PointF(positionFromValue, num3)));
			Matrix transform = g.Transform;
			Matrix matrix = g.Transform.Clone();
			Rectangle rectangle = new Rectangle((int)(pointF.X - (float)point.X * num) + 1, (int)(pointF.Y - (float)point.Y * num2) + 1, (int)((float)image.Width * num) + 1, (int)((float)image.Height * num2) + 1);
			ImageAttributes imageAttributes = new ImageAttributes();
			if (ImageTransColor != Color.Empty)
			{
				imageAttributes.SetColorKey(ImageTransColor, ImageTransColor, ColorAdjustType.Default);
			}
			if (drawShadow)
			{
				ColorMatrix colorMatrix = new ColorMatrix();
				colorMatrix.Matrix00 = 0f;
				colorMatrix.Matrix11 = 0f;
				colorMatrix.Matrix22 = 0f;
				colorMatrix.Matrix33 = Common.GaugeCore.ShadowIntensity / 100f;
				imageAttributes.SetColorMatrix(colorMatrix);
				matrix.Translate(base.ShadowOffset, base.ShadowOffset, MatrixOrder.Append);
			}
			else
			{
				ColorMatrix colorMatrix2 = new ColorMatrix();
				if (!ImageHueColor.IsEmpty)
				{
					Color color = g.TransformHueColor(ImageHueColor);
					colorMatrix2.Matrix00 = (float)(int)color.R / 255f;
					colorMatrix2.Matrix11 = (float)(int)color.G / 255f;
					colorMatrix2.Matrix22 = (float)(int)color.B / 255f;
				}
				colorMatrix2.Matrix33 = 1f - ImageTransparency / 100f;
				imageAttributes.SetColorMatrix(colorMatrix2);
			}
			g.Transform = matrix;
			g.DrawImage(image, rectangle, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
			g.Transform = transform;
			if (!drawShadow)
			{
				GraphicsPath graphicsPath = new GraphicsPath();
				graphicsPath.AddRectangle(rectangle);
				AddHotRegion(graphicsPath, primary: true);
			}
		}

		internal MarkerStyleAttrib GetMarkerStyleAttrib(GaugeGraphics g)
		{
			MarkerStyleAttrib markerStyleAttrib = new MarkerStyleAttrib();
			float absoluteDimension = g.GetAbsoluteDimension(MarkerLength);
			float absoluteDimension2 = g.GetAbsoluteDimension(Width);
			markerStyleAttrib.path = g.CreateMarker(new PointF(0f, 0f), absoluteDimension2, absoluteDimension, MarkerStyle);
			float num = 0f;
			if (Placement == Placement.Cross || Placement == Placement.Inside)
			{
				num += 180f;
			}
			if (GetGauge().GetOrientation() == GaugeOrientation.Vertical)
			{
				num += 270f;
			}
			if (num > 0f)
			{
				using (Matrix matrix = new Matrix())
				{
					matrix.Rotate(num);
					markerStyleAttrib.path.Transform(matrix);
				}
			}
			float num2 = CalculateMarkerDistance();
			LinearScale scale = GetScale();
			float positionFromValue = scale.GetPositionFromValue(scale.GetValueLimit(base.Position));
			PointF pointF = Point.Empty;
			pointF = ((GetGauge().GetOrientation() != 0) ? g.GetAbsolutePoint(new PointF(num2, positionFromValue)) : g.GetAbsolutePoint(new PointF(positionFromValue, num2)));
			markerStyleAttrib.brush = g.GetMarkerBrush(markerStyleAttrib.path, MarkerStyle, pointF, 0f, FillColor, FillGradientType, FillGradientEndColor, FillHatchStyle);
			using (Matrix matrix2 = new Matrix())
			{
				matrix2.Translate(pointF.X, pointF.Y, MatrixOrder.Append);
				markerStyleAttrib.path.Transform(matrix2);
				return markerStyleAttrib;
			}
		}

		internal float CalculateMarkerDistance()
		{
			if (Placement == Placement.Cross)
			{
				return GetScale().Position - DistanceFromScale;
			}
			if (Placement == Placement.Inside)
			{
				return GetScale().Position - GetScale().Width / 2f - DistanceFromScale - MarkerLength / 2f;
			}
			return GetScale().Position + GetScale().Width / 2f + DistanceFromScale + MarkerLength / 2f;
		}

		internal GraphicsPath GetPointerPath(GaugeGraphics g)
		{
			if (!Visible)
			{
				return null;
			}
			GraphicsPath graphicsPath = new GraphicsPath();
			LinearScale scale = GetScale();
			scale.GetPositionFromValue(scale.GetValueLimit(base.Position));
			if (Type == LinearPointerType.Marker || Image != string.Empty)
			{
				MarkerStyleAttrib markerStyleAttrib = GetMarkerStyleAttrib(g);
				if (markerStyleAttrib.path != null)
				{
					graphicsPath.AddPath(markerStyleAttrib.path, connect: false);
				}
			}
			else if (Type == LinearPointerType.Bar)
			{
				BarStyleAttrib barStyleAttrib = GetBarStyleAttrib(g);
				if (barStyleAttrib.primaryPath == null)
				{
					graphicsPath.Dispose();
					return null;
				}
				if (barStyleAttrib.primaryPath != null)
				{
					graphicsPath.AddPath(barStyleAttrib.primaryPath, connect: false);
				}
			}
			else if (Type == LinearPointerType.Thermometer)
			{
				BarStyleAttrib thermometerStyleAttrib = GetThermometerStyleAttrib(g);
				if (thermometerStyleAttrib.primaryPath == null)
				{
					graphicsPath.Dispose();
					return null;
				}
				if (thermometerStyleAttrib.totalPath != null)
				{
					graphicsPath.AddPath(thermometerStyleAttrib.primaryPath, connect: false);
				}
			}
			return graphicsPath;
		}

		internal GraphicsPath GetShadowPath(GaugeGraphics g)
		{
			if (base.ShadowOffset == 0f || GetScale() == null)
			{
				return null;
			}
			if (Image != "")
			{
				DrawImage(g, drawShadow: true);
				return null;
			}
			GraphicsPath pointerPath = GetPointerPath(g);
			if (pointerPath == null || pointerPath.PointCount == 0)
			{
				return null;
			}
			using (Matrix matrix = new Matrix())
			{
				matrix.Translate(base.ShadowOffset, base.ShadowOffset);
				pointerPath.Transform(matrix);
				return pointerPath;
			}
		}

		internal void AddHotRegion(GraphicsPath path, bool primary)
		{
			if (primary)
			{
				hotRegions[0] = path;
			}
			else
			{
				hotRegions[1] = path;
			}
		}

		internal void SetAllHotRegions(GaugeGraphics g)
		{
			Common.GaugeCore.HotRegionList.SetHotRegion(this, PointF.Empty, hotRegions[0], hotRegions[1]);
			hotRegions[0] = null;
			hotRegions[1] = null;
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
			return (LinearScale)GetScaleBase();
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
			using (GraphicsPath graphicsPath = GetPointerPath(g))
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
			LinearPointer linearPointer = new LinearPointer();
			binaryFormatSerializer.Deserialize(linearPointer, stream);
			return linearPointer;
		}
	}
}
