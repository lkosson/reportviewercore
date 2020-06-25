using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.Windows.Forms;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal abstract class ScaleBase : NamedElement, IToolTipProvider, IImageMapProvider
	{
		internal float _startPosition;

		internal float _endPosition = 100f;

		internal float _sweepPosition = 100f;

		internal float coordSystemRatio = 3.6f;

		internal LinearLabelStyle baseLabelStyle;

		internal ArrayList markers = new ArrayList();

		internal ArrayList labels = new ArrayList();

		internal const double MaxMajorTickMarks = 16.0;

		internal bool staticRendering = true;

		private CustomLabelCollection customLabels;

		private double minimum;

		private double maximum = 100.0;

		private double multiplier = 1.0;

		private double interval = double.NaN;

		private double intervalOffset = double.NaN;

		private string toolTip = "";

		private string href = "";

		private string mapAreaAttributes = "";

		internal TickMark majorTickMarkA;

		internal TickMark minorTickMarkA;

		private bool tickMarksOnTop;

		private bool reversed;

		private bool logarithmic;

		private double logarithmicBase = 10.0;

		internal SpecialPosition minimumPin;

		internal SpecialPosition maximumPin;

		private bool visible = true;

		private float width = 5f;

		private Color borderColor = Color.Black;

		private GaugeDashStyle borderStyle;

		private int borderWidth = 1;

		private Color fillColor = Color.CornflowerBlue;

		private GradientType fillGradientType;

		private Color fillGradientEndColor = Color.White;

		private GaugeHatchStyle fillHatchStyle;

		private float shadowOffset = 1f;

		private bool selected;

		private object imageMapProviderTag;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CustomLabelCollection CustomLabels => customLabels;

		[SRCategory("CategoryMisc")]
		[SRDescription("DescriptionAttributeName11")]
		public override string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				base.Name = value;
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeMinimum3")]
		[DefaultValue(0.0)]
		public double Minimum
		{
			get
			{
				return minimum;
			}
			set
			{
				if (Common != null)
				{
					if (value >= Maximum)
					{
						throw new ArgumentException(Utils.SRGetStr("ExceptionMinMax"));
					}
					if (Logarithmic && value < 0.0)
					{
						throw new ArgumentException(Utils.SRGetStr("ExceptionMinLog"));
					}
					if (Logarithmic && value == 0.0 && Maximum <= 1.0)
					{
						throw new ArgumentException(Utils.SRGetStr("ExceptionMinLog"));
					}
					if (double.IsNaN(value) || double.IsInfinity(value))
					{
						throw new ArgumentException(Utils.SRGetStr("ExceptionInvalidValue"));
					}
				}
				minimum = value;
				Invalidate();
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeMaximum3")]
		[DefaultValue(100.0)]
		public double Maximum
		{
			get
			{
				return maximum;
			}
			set
			{
				if (Common != null)
				{
					if (value <= Minimum)
					{
						throw new ArgumentException(Utils.SRGetStr("ExceptionMaxMin"));
					}
					if (Logarithmic && value == 0.0 && Maximum <= 1.0)
					{
						throw new ArgumentException(Utils.SRGetStr("ExceptionMinLog"));
					}
					if (double.IsNaN(value) || double.IsInfinity(value))
					{
						throw new ArgumentException(Utils.SRGetStr("ExceptionInvalidValue"));
					}
				}
				maximum = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeMultiplier")]
		[DefaultValue(1.0)]
		public double Multiplier
		{
			get
			{
				return Math.Round(multiplier, 8);
			}
			set
			{
				multiplier = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeInterval4")]
		[TypeConverter(typeof(DoubleAutoValueConverter))]
		[DefaultValue(double.NaN)]
		public double Interval
		{
			get
			{
				return interval;
			}
			set
			{
				if (value < 0.0 || value > 7.9228162514264338E+28)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", 0, decimal.MaxValue));
				}
				if (value == 0.0)
				{
					value = double.NaN;
				}
				interval = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeIntervalOffset4")]
		[TypeConverter(typeof(DoubleAutoValueConverter))]
		[NotifyParentProperty(true)]
		[DefaultValue(double.NaN)]
		public double IntervalOffset
		{
			get
			{
				return intervalOffset;
			}
			set
			{
				if (value < 0.0)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionIntervalOffsetNegative"));
				}
				intervalOffset = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeToolTip3")]
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
			}
		}

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeHref7")]
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

		[SRCategory("CategoryLabelsAndTickMarks")]
		[SRDescription("DescriptionAttributeMajorTickMarkInt")]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		internal TickMark MajorTickMarkInt
		{
			get
			{
				if (majorTickMarkA == null)
				{
					if (this is CircularScale)
					{
						return ((CircularScale)this).MajorTickMark;
					}
					if (this is LinearScale)
					{
						return ((LinearScale)this).MajorTickMark;
					}
				}
				return majorTickMarkA;
			}
			set
			{
				majorTickMarkA = value;
				majorTickMarkA.Parent = this;
				Invalidate();
			}
		}

		[SRCategory("CategoryLabelsAndTickMarks")]
		[SRDescription("DescriptionAttributeMinorTickMarkInt")]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		internal TickMark MinorTickMarkInt
		{
			get
			{
				if (minorTickMarkA == null)
				{
					if (this is CircularScale)
					{
						return ((CircularScale)this).MinorTickMark;
					}
					if (this is LinearScale)
					{
						return ((LinearScale)this).MinorTickMark;
					}
				}
				return minorTickMarkA;
			}
			set
			{
				minorTickMarkA = value;
				minorTickMarkA.Parent = this;
				Invalidate();
			}
		}

		[SRCategory("CategoryLabelsAndTickMarks")]
		[SRDescription("DescriptionAttributeTickMarksOnTop")]
		[DefaultValue(false)]
		public bool TickMarksOnTop
		{
			get
			{
				return tickMarksOnTop;
			}
			set
			{
				tickMarksOnTop = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeReversed")]
		[DefaultValue(false)]
		public bool Reversed
		{
			get
			{
				return reversed;
			}
			set
			{
				reversed = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeLogarithmic")]
		[DefaultValue(false)]
		public bool Logarithmic
		{
			get
			{
				return logarithmic;
			}
			set
			{
				if (value && (Minimum < 0.0 || (Minimum == 0.0 && Maximum < 1.0)))
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionMinLog"));
				}
				logarithmic = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeLogarithmicBase")]
		[DefaultValue(10.0)]
		public double LogarithmicBase
		{
			get
			{
				return logarithmicBase;
			}
			set
			{
				if (value <= 1.0)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionOutOfrange_min_open", 1));
				}
				logarithmicBase = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeMinimumPin")]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		internal SpecialPosition MinimumPin
		{
			get
			{
				return minimumPin;
			}
			set
			{
				minimumPin = value;
				minimumPin.Parent = this;
				Invalidate();
			}
		}

		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeMaximumPin")]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		internal SpecialPosition MaximumPin
		{
			get
			{
				return maximumPin;
			}
			set
			{
				maximumPin = value;
				maximumPin.Parent = this;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeVisible10")]
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

		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeWidth9")]
		[ValidateBound(0.0, 30.0)]
		[DefaultValue(5f)]
		public virtual float Width
		{
			get
			{
				return width;
			}
			set
			{
				if (value < 0f || value > 100f)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", 0, 100));
				}
				width = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeBorderColor6")]
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
		[SRDescription("DescriptionAttributeBorderStyle9")]
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
		[SRDescription("DescriptionAttributeBorderWidth10")]
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
		[SRDescription("DescriptionAttributeFillColor5")]
		[DefaultValue(typeof(Color), "CornflowerBlue")]
		public Color FillColor
		{
			get
			{
				return fillColor;
			}
			set
			{
				fillColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeFillGradientType3")]
		[DefaultValue(GradientType.None)]
		public GradientType FillGradientType
		{
			get
			{
				return fillGradientType;
			}
			set
			{
				fillGradientType = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeFillGradientEndColor3")]
		[DefaultValue(typeof(Color), "White")]
		public Color FillGradientEndColor
		{
			get
			{
				return fillGradientEndColor;
			}
			set
			{
				fillGradientEndColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeFillHatchStyle5")]
		public GaugeHatchStyle FillHatchStyle
		{
			get
			{
				return fillHatchStyle;
			}
			set
			{
				fillHatchStyle = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeShadowOffset3")]
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

		internal double MinimumLog
		{
			get
			{
				if (Minimum == 0.0 && Logarithmic)
				{
					return 1.0;
				}
				return Minimum;
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeSelected10")]
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

		internal float StartPosition
		{
			get
			{
				object parentElement = ParentElement;
				if (parentElement is LinearGauge)
				{
					LinearGauge linearGauge = (LinearGauge)parentElement;
					if (linearGauge.GetOrientation() == GaugeOrientation.Vertical)
					{
						if (GetReversed())
						{
							return 100f - (_endPosition - GetMaxOffset(linearGauge));
						}
						return 100f - _endPosition;
					}
					if (GetReversed())
					{
						return _startPosition;
					}
					return _startPosition + GetMaxOffset(linearGauge);
				}
				return _startPosition;
			}
		}

		internal float EndPosition
		{
			get
			{
				object parentElement = ParentElement;
				float num = _endPosition;
				if (parentElement is LinearGauge)
				{
					LinearGauge linearGauge = (LinearGauge)parentElement;
					if (linearGauge.GetOrientation() == GaugeOrientation.Vertical)
					{
						num = ((!GetReversed()) ? (100f - (_startPosition + GetMaxOffset(linearGauge))) : (100f - _startPosition));
					}
					else if (GetReversed())
					{
						num = _endPosition - GetMaxOffset(linearGauge);
					}
				}
				if (num < StartPosition)
				{
					num = StartPosition;
				}
				return num;
			}
		}

		internal float SweepPosition => _sweepPosition;

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

		internal ScaleBase()
		{
			customLabels = new CustomLabelCollection(this, common);
			maximumPin = new SpecialPosition(this);
			minimumPin = new SpecialPosition(this);
		}

		private float GetMaxOffset(LinearGauge gauge)
		{
			SizeF absoluteSize = gauge.AbsoluteSize;
			if (absoluteSize.IsEmpty)
			{
				return 0f;
			}
			float num = 0f;
			foreach (LinearPointer pointer in gauge.Pointers)
			{
				if (pointer.Type == LinearPointerType.Thermometer)
				{
					float num2 = (gauge.GetOrientation() != GaugeOrientation.Vertical) ? (absoluteSize.Height / absoluteSize.Width) : (absoluteSize.Width / absoluteSize.Height);
					float val = (pointer.ThermometerBulbSize + pointer.ThermometerBulbOffset) * num2;
					num = Math.Max(num, val);
				}
			}
			return num;
		}

		internal GaugeBase GetGauge()
		{
			return (GaugeBase)Collection.ParentElement;
		}

		internal bool GetReversed()
		{
			GaugeBase gauge = GetGauge();
			if (Common != null && Common.GaugeContainer != null && Common.GaugeContainer.RightToLeft == RightToLeft.Yes)
			{
				if (gauge is CircularGauge)
				{
					return !Reversed;
				}
				if (((LinearGauge)gauge).GetOrientation() == GaugeOrientation.Horizontal)
				{
					return !Reversed;
				}
			}
			return Reversed;
		}

		internal CustomTickMark GetEndLabelTickMark()
		{
			if (MinorTickMarkInt.Visible)
			{
				return MinorTickMarkInt;
			}
			if (MajorTickMarkInt.Visible)
			{
				return MajorTickMarkInt;
			}
			return null;
		}

		internal Brush GetLightBrush(GaugeGraphics g, CustomTickMark tickMark, Color fillColor, GraphicsPath path)
		{
			Brush brush = null;
			if (tickMark.EnableGradient)
			{
				HSV hsv = ColorHandler.ColorToHSV(fillColor);
				hsv.value = (int)((double)hsv.value * 0.2);
				Color color = ColorHandler.HSVtoColor(hsv);
				color = Color.FromArgb(fillColor.A, color.R, color.G, color.B);
				RectangleF bounds = path.GetBounds();
				float num = 1f - tickMark.GradientDensity / 100f;
				if (tickMark.Shape == MarkerStyle.Circle)
				{
					brush = new PathGradientBrush(path);
					((PathGradientBrush)brush).CenterColor = fillColor;
					((PathGradientBrush)brush).CenterPoint = new PointF(bounds.X + bounds.Width / 2f, bounds.Y + bounds.Height / 2f);
					((PathGradientBrush)brush).SurroundColors = new Color[1]
					{
						color
					};
					Blend blend = new Blend();
					blend.Factors = new float[2]
					{
						num,
						1f
					};
					blend.Positions = new float[2]
					{
						0f,
						1f
					};
					((PathGradientBrush)brush).Blend = blend;
				}
				else
				{
					brush = new LinearGradientBrush(path.GetBounds(), color, fillColor, LinearGradientMode.Vertical);
					Blend blend2 = new Blend();
					blend2.Factors = new float[3]
					{
						num,
						1f,
						num
					};
					blend2.Positions = new float[3]
					{
						0f,
						0.5f,
						1f
					};
					((LinearGradientBrush)brush).Blend = blend2;
				}
			}
			else
			{
				brush = new SolidBrush(fillColor);
			}
			return brush;
		}

		internal abstract void DrawTickMark(GaugeGraphics g, CustomTickMark tickMark, double value, float offset);

		internal void DrawTickMark(GaugeGraphics g, CustomTickMark tickMark, double value, float offset, Matrix matrix)
		{
			if (tickMark.Width <= 0f || tickMark.Length <= 0f)
			{
				return;
			}
			float position = GetPositionFromValue(value);
			MarkerPosition markerPosition = new MarkerPosition(position, value, tickMark.Placement);
			if (MarkerPosition.IsExistsInArray(markers, markerPosition) && !tickMark.GetType().Equals(typeof(CircularSpecialPosition)) && !tickMark.GetType().Equals(typeof(LinearSpecialPosition)))
			{
				return;
			}
			markers.Add(markerPosition);
			PointF absolutePoint = g.GetAbsolutePoint(GetPoint(position, offset));
			if (tickMark.Image != string.Empty)
			{
				DrawTickMarkImage(g, tickMark, matrix, absolutePoint, drawShadow: false);
				return;
			}
			SizeF sizeF = new SizeF(g.GetAbsoluteDimension(tickMark.Width), g.GetAbsoluteDimension(tickMark.Length));
			Color rangeTickMarkColor = GetRangeTickMarkColor(value, tickMark.FillColor);
			using (GraphicsPath graphicsPath = g.CreateMarker(absolutePoint, sizeF.Width, sizeF.Height, tickMark.Shape))
			{
				using (Brush brush = GetLightBrush(g, tickMark, rangeTickMarkColor, graphicsPath))
				{
					graphicsPath.Transform(matrix);
					if (tickMark.EnableGradient && brush is LinearGradientBrush)
					{
						((LinearGradientBrush)brush).Transform = matrix;
					}
					if (ShadowOffset != 0f)
					{
						g.DrawPathShadowAbs(graphicsPath, g.GetShadowColor(), ShadowOffset);
					}
					g.FillPath(brush, graphicsPath, 0f, useBrushOffset: false, circularFill: false);
					if (tickMark.BorderWidth > 0 && tickMark.BorderStyle != 0)
					{
						using (Pen pen = new Pen(tickMark.BorderColor, tickMark.BorderWidth))
						{
							pen.DashStyle = g.GetPenStyle(tickMark.BorderStyle);
							pen.Alignment = PenAlignment.Outset;
							g.DrawPath(pen, graphicsPath);
						}
					}
				}
			}
		}

		internal void DrawTickMarkImage(GaugeGraphics g, CustomTickMark tickMark, Matrix matrix, PointF centerPoint, bool drawShadow)
		{
			float absoluteDimension = g.GetAbsoluteDimension(tickMark.Length);
			Image image = null;
			image = Common.ImageLoader.LoadImage(tickMark.Image);
			if (image.Width != 0 && image.Height != 0)
			{
				float num = image.Height;
				float num2 = absoluteDimension / num;
				Rectangle destRect = new Rectangle(0, 0, (int)((float)image.Width * num2), (int)((float)image.Height * num2));
				ImageAttributes imageAttributes = new ImageAttributes();
				if (tickMark.ImageTransColor != Color.Empty)
				{
					imageAttributes.SetColorKey(tickMark.ImageTransColor, tickMark.ImageTransColor, ColorAdjustType.Default);
				}
				Matrix transform = g.Transform;
				Matrix matrix2 = g.Transform.Clone();
				matrix2.Multiply(matrix, MatrixOrder.Prepend);
				if (drawShadow)
				{
					ColorMatrix colorMatrix = new ColorMatrix();
					colorMatrix.Matrix00 = 0f;
					colorMatrix.Matrix11 = 0f;
					colorMatrix.Matrix22 = 0f;
					colorMatrix.Matrix33 = Common.GaugeCore.ShadowIntensity / 100f;
					imageAttributes.SetColorMatrix(colorMatrix);
					matrix2.Translate(ShadowOffset, ShadowOffset, MatrixOrder.Append);
				}
				else if (!tickMark.ImageHueColor.IsEmpty)
				{
					Color color = g.TransformHueColor(tickMark.ImageHueColor);
					ColorMatrix colorMatrix2 = new ColorMatrix();
					colorMatrix2.Matrix00 = (float)(int)color.R / 255f;
					colorMatrix2.Matrix11 = (float)(int)color.G / 255f;
					colorMatrix2.Matrix22 = (float)(int)color.B / 255f;
					imageAttributes.SetColorMatrix(colorMatrix2);
				}
				destRect.X = (int)(centerPoint.X - (float)(destRect.Width / 2));
				destRect.Y = (int)(centerPoint.Y - (float)(destRect.Height / 2));
				g.Transform = matrix2;
				ImageSmoothingState imageSmoothingState = new ImageSmoothingState(g);
				imageSmoothingState.Set();
				g.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
				imageSmoothingState.Restore();
				g.Transform = transform;
			}
		}

		internal float GetTickMarkOffset(CustomTickMark tickMark)
		{
			float num = 0f;
			switch (tickMark.Placement)
			{
			case Placement.Inside:
				return (0f - Width) / 2f - tickMark.Length / 2f - tickMark.DistanceFromScale;
			case Placement.Cross:
				return 0f - tickMark.DistanceFromScale;
			case Placement.Outside:
				return Width / 2f + tickMark.Length / 2f + tickMark.DistanceFromScale;
			default:
				throw new InvalidOperationException(Utils.SRGetStr("ExceptionInvalidPlacementType"));
			}
		}

		internal void RenderTicks(GaugeGraphics g, TickMark tickMark, double interval, double max, double min, double intOffset, bool forceLinear)
		{
			float tickMarkOffset = GetTickMarkOffset(tickMark);
			double num = min + intOffset;
			while (num <= max)
			{
				DrawTickMark(g, tickMark, num, tickMarkOffset);
				try
				{
					num = GetNextPosition(num, interval, forceLinear);
				}
				catch (OverflowException)
				{
					return;
				}
			}
		}

		internal void RenderGrid(GaugeGraphics g)
		{
			if (MajorTickMarkInt.Visible)
			{
				RenderTicks(g, MajorTickMarkInt, GetInterval(IntervalTypes.Major), Maximum, MinimumLog, GetIntervalOffset(IntervalTypes.Major), forceLinear: false);
			}
			if (!MinorTickMarkInt.Visible)
			{
				return;
			}
			if (!Logarithmic)
			{
				RenderTicks(g, MinorTickMarkInt, GetInterval(IntervalTypes.Minor), Maximum, MinimumLog, GetIntervalOffset(IntervalTypes.Minor), forceLinear: false);
				return;
			}
			double num = GetIntervalOffset(IntervalTypes.Minor);
			double num2 = MinimumLog + num;
			double num3 = GetInterval(IntervalTypes.Major);
			double nextPosition = GetNextPosition(num2, num3, forceLinear: false);
			double num4 = GetInterval(IntervalTypes.Minor);
			num4 = 1.0 / num4 * LogarithmicBase;
			while (num2 <= nextPosition && num2 < Maximum)
			{
				RenderTicks(g, MinorTickMarkInt, nextPosition / num4, Math.Min(nextPosition, Maximum), num2, num, forceLinear: true);
				num2 = nextPosition;
				try
				{
					nextPosition = GetNextPosition(nextPosition, num3, forceLinear: false);
				}
				catch (OverflowException)
				{
					return;
				}
			}
		}

		internal abstract void DrawCustomLabel(CustomLabel label);

		internal abstract LinearLabelStyle GetLabelStyle();

		internal float GetOffsetLabelPos(Placement placement, float scaleOffset, float scalePosition)
		{
			Gap gap = new Gap(scalePosition);
			gap.SetOffset(Placement.Cross, Width);
			gap.SetBase();
			if (MajorTickMarkInt.Visible)
			{
				gap.SetOffsetBase(MajorTickMarkInt.Placement, MajorTickMarkInt.Length);
			}
			if (MinorTickMarkInt.Visible)
			{
				gap.SetOffsetBase(MinorTickMarkInt.Placement, MinorTickMarkInt.Length);
			}
			gap.SetBase();
			float num = 0f;
			switch (placement)
			{
			case Placement.Inside:
				return 0f - gap.Inside - scaleOffset;
			case Placement.Cross:
				return 0f - scaleOffset;
			case Placement.Outside:
				return gap.Outside + scaleOffset;
			default:
				throw new InvalidOperationException(Utils.SRGetStr("ExceptionInvalidPlacementType"));
			}
		}

		internal Font GetResizedFont(Font font, FontUnit fontUnit)
		{
			if (fontUnit == FontUnit.Percent)
			{
				float absoluteDimension = Common.Graph.GetAbsoluteDimension(font.Size);
				absoluteDimension = Math.Max(absoluteDimension, 0.001f);
				return new Font(font.FontFamily.Name, absoluteDimension, font.Style, GraphicsUnit.Pixel, font.GdiCharSet, font.GdiVerticalFont);
			}
			return font;
		}

		internal void RenderCustomLabels(GaugeGraphics g)
		{
			foreach (CustomLabel customLabel in CustomLabels)
			{
				if (customLabel.Visible)
				{
					DrawCustomLabel(customLabel);
				}
			}
		}

		internal abstract void DrawSpecialPosition(GaugeGraphics g, SpecialPosition label, float angle);

		internal void RenderPins(GaugeGraphics g)
		{
			if (!IsReversed())
			{
				DrawSpecialPosition(g, MinimumPin, StartPosition - MinimumPin.Location);
				DrawSpecialPosition(g, MaximumPin, EndPosition + MaximumPin.Location);
			}
			else
			{
				DrawSpecialPosition(g, MinimumPin, EndPosition + MinimumPin.Location);
				DrawSpecialPosition(g, MaximumPin, StartPosition - MaximumPin.Location);
			}
		}

		protected void InvalidateEndPosition()
		{
			_endPosition = _startPosition + _sweepPosition;
		}

		protected void InvalidateSweepPosition()
		{
			_sweepPosition = _endPosition - _startPosition;
		}

		internal Color GetRangeTickMarkColor(double value, Color color)
		{
			foreach (RangeBase range in GetGauge().GetRanges())
			{
				if (range.InRangeTickMarkColor != Color.Empty && range.ScaleName == Name && range.StartValue <= value && range.EndValue >= value)
				{
					return range.InRangeTickMarkColor;
				}
			}
			return color;
		}

		internal Color GetRangeLabelsColor(double value, Color color)
		{
			foreach (RangeBase range in GetGauge().GetRanges())
			{
				if (range.InRangeLabelColor != Color.Empty && range.ScaleName == Name && range.StartValue <= value && range.EndValue >= value)
				{
					return range.InRangeLabelColor;
				}
			}
			return color;
		}

		internal virtual double GetValueLimit(double value, bool snapEnable, double snapInterval)
		{
			double valueLimit = GetValueLimit(value);
			if (snapEnable)
			{
				if (snapInterval == 0.0)
				{
					return MarkerPosition.Snap(markers, valueLimit);
				}
				if (Logarithmic)
				{
					snapInterval = Math.Pow(LogarithmicBase, snapInterval);
					return Math.Max(minimum, GetValueLimit(snapInterval * Math.Round(valueLimit / snapInterval)));
				}
				return GetValueLimit(snapInterval * Math.Round(valueLimit / snapInterval));
			}
			return valueLimit;
		}

		internal virtual double GetValueLimit(double value)
		{
			float position = StartPosition - MinimumPin.Location;
			float position2 = EndPosition + MaximumPin.Location;
			if (IsReversed())
			{
				position = EndPosition + MinimumPin.Location;
				position2 = StartPosition - MaximumPin.Location;
			}
			if (double.IsNaN(value))
			{
				if (MinimumPin.Enable)
				{
					return GetValueFromPosition(position);
				}
				return MinimumLog;
			}
			double num = MinimumLog;
			if (MinimumPin.Enable)
			{
				num = GetValueFromPosition(position);
			}
			double valueFromPosition = Maximum;
			if (MaximumPin.Enable)
			{
				valueFromPosition = GetValueFromPosition(position2);
			}
			if (value < num)
			{
				return num;
			}
			if (value > valueFromPosition)
			{
				return valueFromPosition;
			}
			return value;
		}

		internal double GetIntervalOffset(IntervalTypes type)
		{
			double num = 0.0;
			switch (type)
			{
			case IntervalTypes.Minor:
				num = MinorTickMarkInt.IntervalOffset;
				if (double.IsNaN(num))
				{
					num = GetIntervalOffset(IntervalTypes.Major) % GetInterval(IntervalTypes.Minor);
				}
				break;
			case IntervalTypes.Major:
				num = MajorTickMarkInt.IntervalOffset;
				if (double.IsNaN(num))
				{
					num = GetIntervalOffset(IntervalTypes.Main);
				}
				break;
			case IntervalTypes.Labels:
				num = GetLabelStyle().IntervalOffset;
				if (double.IsNaN(num))
				{
					num = GetIntervalOffset(IntervalTypes.Major);
				}
				break;
			case IntervalTypes.Main:
				num = IntervalOffset;
				if (double.IsNaN(num))
				{
					num = 0.0;
				}
				break;
			}
			return num;
		}

		internal double GetInterval(IntervalTypes type)
		{
			double num = (Maximum - MinimumLog) / 10.0;
			switch (type)
			{
			case IntervalTypes.Minor:
				num = MinorTickMarkInt.Interval;
				if (!double.IsNaN(num))
				{
					break;
				}
				if (!Logarithmic)
				{
					double num6 = GetInterval(IntervalTypes.Major);
					double num7 = SweepPosition / coordSystemRatio;
					if (coordSystemRatio < 3f)
					{
						num7 /= 2.0;
					}
					double a = Math.Round(96.0 * (num7 / 100.0)) / ((Maximum - MinimumLog) / num6);
					if (Math.Pow(10.0, Math.Round(Math.Log10(num6))) == num6)
					{
						return num6 / 5.0;
					}
					int num8 = (int)(0.0 - (Math.Round(Math.Log10(num6)) - 1.0));
					double num9 = Math.Pow(10.0, -num8) * 2.0;
					for (int i = 0; i < 2; i++)
					{
						for (int num10 = (int)Math.Round(a); num10 > 0; num10--)
						{
							double num11 = num6 / (double)num10;
							if ((num11 % num9 != 0.0 || i != 0) && Utils.Round(num11, num8) == num11)
							{
								return num11;
							}
						}
					}
					num = Math.Pow(10.0, Math.Floor(Math.Log10(num6)) - 1.0);
					if (num6 % 2.0 == 0.0)
					{
						return num * 2.0;
					}
					if (num6 % 5.0 == 0.0)
					{
						return num * 5.0;
					}
					if (num6 % 3.0 == 0.0)
					{
						return num * 3.0;
					}
					return num;
				}
				num = 1.0;
				break;
			case IntervalTypes.Major:
				num = MajorTickMarkInt.Interval;
				if (double.IsNaN(num))
				{
					num = GetInterval(IntervalTypes.Main);
				}
				break;
			case IntervalTypes.Labels:
				num = GetLabelStyle().Interval;
				if (double.IsNaN(num))
				{
					num = GetInterval(IntervalTypes.Major);
				}
				break;
			case IntervalTypes.Main:
				num = Interval;
				if (!double.IsNaN(num))
				{
					break;
				}
				if (!Logarithmic)
				{
					double num2 = Math.Pow(10.0, Math.Round(Math.Log10(Maximum - MinimumLog)) - 1.0);
					if ((Maximum - MinimumLog) / num2 < 7.0)
					{
						num2 /= 10.0;
					}
					num = num2;
					double num3 = (Maximum - MinimumLog) / num;
					double num4 = SweepPosition / coordSystemRatio;
					if (coordSystemRatio < 3f)
					{
						num4 /= 2.0;
					}
					double num5 = Math.Round(16.0 * (num4 / 100.0));
					List<double> list = new List<double>();
					bool flag = false;
					while (Math.Round(num3, 0) != num3 || num3 > num5)
					{
						num += num2;
						num3 = (Maximum - MinimumLog) / num;
						if (num3 <= Math.Max(num5 / 2.0, 1.0))
						{
							if (Math.Round(num3, 0) == num3)
							{
								break;
							}
							list.Add(num);
							if (num3 <= Math.Max(num5 / 3.0, 1.0))
							{
								flag = true;
								break;
							}
						}
					}
					if (flag)
					{
						num = list[0];
					}
				}
				else
				{
					num = 1.0;
				}
				break;
			}
			if (!Logarithmic)
			{
				while ((Maximum - MinimumLog) / num > 1000.0)
				{
					num *= 10.0;
				}
			}
			return num;
		}

		internal double GetNextPosition(double position, double interval, bool forceLinear)
		{
			if (forceLinear || !Logarithmic)
			{
				interval = double.Parse(interval.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
				position += interval;
				position = double.Parse(position.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
			}
			else
			{
				position = Math.Pow(LogarithmicBase, Math.Log(position, LogarithmicBase) + interval);
			}
			return position;
		}

		protected virtual double GetValueAgainstScaleRatio(double value)
		{
			double result = 0.0;
			if (!Logarithmic)
			{
				result = (value - MinimumLog) / (Maximum - MinimumLog);
			}
			else if (Logarithmic)
			{
				double num = Math.Log(Maximum, LogarithmicBase);
				double num2 = Math.Log(MinimumLog, LogarithmicBase);
				result = (Math.Log(value, LogarithmicBase) - num2) / (num - num2);
			}
			return result;
		}

		protected virtual double GetValueByRatio(float ratio)
		{
			double result = 0.0;
			if (!Logarithmic)
			{
				result = MinimumLog + (Maximum - MinimumLog) * (double)ratio;
			}
			else if (Logarithmic)
			{
				double num = Math.Log(Maximum, LogarithmicBase);
				double num2 = Math.Log(MinimumLog, LogarithmicBase);
				result = Math.Pow(LogarithmicBase, num2 + (num - num2) * (double)ratio);
			}
			return result;
		}

		protected virtual bool IsReversed()
		{
			return GetReversed();
		}

		protected float GetPositionFromValue(double value, float startPos, float endPos)
		{
			double valueAgainstScaleRatio = GetValueAgainstScaleRatio(value);
			double num = endPos - startPos;
			float num2 = 0f;
			if (IsReversed())
			{
				return (float)((double)endPos - num * valueAgainstScaleRatio);
			}
			return (float)((double)startPos + num * valueAgainstScaleRatio);
		}

		internal virtual float GetPositionFromValue(double value)
		{
			return GetPositionFromValue(value, StartPosition / coordSystemRatio, EndPosition / coordSystemRatio) * coordSystemRatio;
		}

		internal virtual double GetValueFromPosition(float position)
		{
			double num = (position - StartPosition) / (EndPosition - StartPosition);
			if (IsReversed())
			{
				num = 1.0 - num;
			}
			return GetValueByRatio((float)num);
		}

		internal abstract double GetValue(PointF c, PointF p);

		protected abstract PointF GetPoint(float position, float offset);

		internal virtual PointF GetPointRel(double value, float offset)
		{
			return GetPoint(GetPositionFromValue(value), offset);
		}

		internal virtual PointF GetPointAbs(double value, float offset)
		{
			if (Common != null)
			{
				return Common.Graph.GetAbsolutePoint(GetPointRel(value, offset));
			}
			throw new ApplicationException(Utils.SRGetStr("ExceptionGdiNonInitialized"));
		}

		internal virtual void PointerValueChanged(PointerBase sender)
		{
			if (Common != null && !double.IsNaN(sender.Data.OldValue))
			{
				bool playbackMode = false;
				if (((IValueConsumer)sender.Data).GetProvider() != null)
				{
					playbackMode = ((IValueConsumer)sender.Data).GetProvider().GetPlayBackMode();
				}
				if ((sender.Data.OldValue >= minimum && sender.Data.Value < minimum) || (sender.Data.OldValue <= maximum && sender.Data.Value > maximum))
				{
					Common.GaugeContainer.OnValueScaleLeave(this, new ValueRangeEventArgs(sender.Data.Value, sender.Data.DateValueStamp, Name, playbackMode, sender));
				}
				if ((sender.Data.OldValue < minimum && sender.Data.Value >= minimum) || (sender.Data.OldValue > maximum && sender.Data.Value <= maximum))
				{
					Common.GaugeContainer.OnValueScaleEnter(this, new ValueRangeEventArgs(sender.Data.Value, sender.Data.DateValueStamp, Name, playbackMode, this));
				}
			}
		}

		internal override void OnAdded()
		{
			base.OnAdded();
			CustomLabels.Common = Common;
			CustomLabels.parent = this;
			Maximum = Maximum;
			Minimum = Minimum;
		}

		internal override void OnRemove()
		{
			base.OnRemove();
			CustomLabels.Common = null;
			CustomLabels.parent = null;
		}

		internal override void BeginInit()
		{
			base.BeginInit();
			CustomLabels.BeginInit();
		}

		internal override void EndInit()
		{
			base.EndInit();
			CustomLabels.EndInit();
		}

		internal override void Invalidate()
		{
			if (Common != null)
			{
				Common.GaugeCore.Notify(MessageType.DataInvalidated, this, false);
			}
			base.Invalidate();
		}

		string IToolTipProvider.GetToolTip(HitTestResult ht)
		{
			if (Common != null && Common.GaugeCore != null)
			{
				string original = Common.GaugeCore.ResolveAllKeywords(ToolTip, this);
				return Common.GaugeCore.ResolveKeyword(original, "#VALUE", ht.ScaleValue);
			}
			return ToolTip;
		}

		string IImageMapProvider.GetToolTip()
		{
			if (Common != null && Common.GaugeCore != null)
			{
				return Common.GaugeCore.ResolveAllKeywords(ToolTip, this);
			}
			return ToolTip;
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
	}
}
