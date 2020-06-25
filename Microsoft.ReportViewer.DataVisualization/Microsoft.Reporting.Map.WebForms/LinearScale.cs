using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Globalization;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class LinearScale : MapObject, IToolTipProvider
	{
		private LinearLabelStyle labelStyle;

		private float position = 50f;

		private LinearMajorTickMark majorTickMark;

		private LinearMinorTickMark minorTickMark;

		internal LinearSpecialPosition minimumPin;

		internal LinearSpecialPosition maximumPin;

		internal float _startPosition;

		internal float _endPosition = 100f;

		internal float _sweepPosition = 100f;

		internal float coordSystemRatio = 3.6f;

		internal ArrayList markers = new ArrayList();

		internal ArrayList labels = new ArrayList();

		internal const double MaxMajorTickMarks = 16.0;

		internal bool staticRendering = true;

		private double minimum;

		private double maximum = 100.0;

		private double multiplier = 1.0;

		private double interval = double.NaN;

		private double intervalOffset = double.NaN;

		private string toolTip = "";

		private string href = "";

		private bool tickMarksOnTop;

		private bool reversed;

		private bool logarithmic;

		private double logarithmicBase = 10.0;

		private bool visible = true;

		private float width = 5f;

		private Color borderColor = Color.DarkGray;

		private MapDashStyle borderStyle = MapDashStyle.Solid;

		private int borderWidth = 1;

		private Color fillColor = Color.CornflowerBlue;

		private GradientType fillGradientType;

		private Color fillSecondaryColor = Color.White;

		private MapHatchStyle fillHatchStyle;

		private float shadowOffset = 1f;

		[SRCategory("CategoryAttribute_LabelsAndTickMarks")]
		[Description("The label attributes of this scale.")]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		internal LinearLabelStyle LabelStyle
		{
			get
			{
				return labelStyle;
			}
			set
			{
				labelStyle = value;
				labelStyle.Parent = this;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Layout")]
		[SRDescription("DescriptionAttributeLinearScale_Position")]
		[TypeConverter(typeof(FloatAutoValueConverter))]
		[NotifyParentProperty(true)]
		[DefaultValue(50f)]
		public float Position
		{
			get
			{
				return position;
			}
			set
			{
				position = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Layout")]
		[SRDescription("DescriptionAttributeLinearScale_StartMargin")]
		[DefaultValue(8f)]
		public float StartMargin
		{
			get
			{
				return _startPosition;
			}
			set
			{
				if (value > 100f || value < 0f)
				{
					throw new ArgumentOutOfRangeException(SR.out_of_range(0.0, 100.0));
				}
				_startPosition = Math.Min(value, _endPosition);
				InvalidateSweepPosition();
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Layout")]
		[SRDescription("DescriptionAttributeLinearScale_EndMargin")]
		[DefaultValue(8f)]
		public float EndMargin
		{
			get
			{
				return 100f - _endPosition;
			}
			set
			{
				if (value > 100f || value < 0f)
				{
					throw new ArgumentOutOfRangeException(SR.out_of_range(0.0, 100.0));
				}
				_endPosition = Math.Max(100f - value, _startPosition);
				InvalidateSweepPosition();
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_LabelsAndTickMarks")]
		[SRDescription("DescriptionAttributeLinearScale_MajorTickMark")]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LinearMajorTickMark MajorTickMark
		{
			get
			{
				return majorTickMark;
			}
			set
			{
				majorTickMark = value;
				majorTickMark.Parent = this;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_LabelsAndTickMarks")]
		[SRDescription("DescriptionAttributeLinearScale_MinorTickMark")]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LinearMinorTickMark MinorTickMark
		{
			get
			{
				return minorTickMark;
			}
			set
			{
				minorTickMark = value;
				minorTickMark.Parent = this;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Layout")]
		[SRDescription("DescriptionAttributeLinearScale_MinimumPin")]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LinearSpecialPosition MinimumPin
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

		[SRCategory("CategoryAttribute_Layout")]
		[SRDescription("DescriptionAttributeLinearScale_MaximumPin")]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LinearSpecialPosition MaximumPin
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

		internal ZoomPanel ParentGauge => GetGauge();

		[SRCategory("CategoryAttribute_Layout")]
		[SRDescription("DescriptionAttributeLinearScale_Minimum")]
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
						throw new ArgumentException(SR.min_max_error);
					}
					if (Logarithmic && value < 0.0)
					{
						throw new ArgumentException(SR.min_log_error);
					}
					if (Logarithmic && value == 0.0 && Maximum <= 1.0)
					{
						throw new ArgumentException(SR.min_log_error);
					}
					if (double.IsNaN(value) || double.IsInfinity(value))
					{
						throw new ArgumentException(SR.invalid_param(value));
					}
				}
				minimum = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Layout")]
		[SRDescription("DescriptionAttributeLinearScale_Maximum")]
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
						throw new ArgumentException(SR.max_min_error);
					}
					if (Logarithmic && value == 0.0 && Maximum <= 1.0)
					{
						throw new ArgumentException(SR.min_log_error);
					}
					if (double.IsNaN(value) || double.IsInfinity(value))
					{
						throw new ArgumentException(SR.invalid_param(value));
					}
				}
				maximum = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeLinearScale_Multiplier")]
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

		[SRCategory("CategoryAttribute_Layout")]
		[SRDescription("DescriptionAttributeLinearScale_Interval")]
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
				if (value < 0.0)
				{
					throw new ArgumentException(SR.interval_negative);
				}
				if (value == 0.0)
				{
					value = double.NaN;
				}
				interval = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Layout")]
		[SRDescription("DescriptionAttributeLinearScale_IntervalOffset")]
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
					throw new ArgumentException(SR.interval_offset_negative);
				}
				intervalOffset = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeLinearScale_ToolTip")]
		[Localizable(true)]
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

		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeLinearScale_Href")]
		[Localizable(true)]
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

		[SRCategory("CategoryAttribute_LabelsAndTickMarks")]
		[SRDescription("DescriptionAttributeLinearScale_TickMarksOnTop")]
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

		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeLinearScale_Reversed")]
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

		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeLinearScale_Logarithmic")]
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
					throw new ArgumentException(SR.min_log_error);
				}
				logarithmic = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeLinearScale_LogarithmicBase")]
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
					throw new ArgumentOutOfRangeException(SR.out_of_range_min_open(1.0));
				}
				logarithmicBase = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeLinearScale_Visible")]
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

		[SRCategory("CategoryAttribute_Layout")]
		[SRDescription("DescriptionAttributeLinearScale_Width")]
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
					throw new ArgumentException(SR.must_in_range(0.0, 100.0));
				}
				width = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeLinearScale_BorderColor")]
		[DefaultValue(typeof(Color), "DarkGray")]
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

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeLinearScale_BorderStyle")]
		[DefaultValue(MapDashStyle.Solid)]
		public MapDashStyle BorderStyle
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

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeLinearScale_BorderWidth")]
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
					throw new ArgumentException(SR.must_in_range(0.0, 100.0));
				}
				borderWidth = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeLinearScale_FillColor")]
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

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeLinearScale_FillGradientType")]
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

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeLinearScale_FillSecondaryColor")]
		[DefaultValue(typeof(Color), "White")]
		public Color FillSecondaryColor
		{
			get
			{
				return fillSecondaryColor;
			}
			set
			{
				fillSecondaryColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeLinearScale_FillHatchStyle")]
		[DefaultValue(MapHatchStyle.None)]
		public MapHatchStyle FillHatchStyle
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

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeLinearScale_ShadowOffset")]
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
					throw new ArgumentException(SR.must_in_range(-100.0, 100.0));
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

		internal float StartPosition
		{
			get
			{
				object parent = Parent;
				if (parent is ZoomPanel)
				{
					ZoomPanel zoomPanel = (ZoomPanel)parent;
					if (zoomPanel.GetOrientation() == Orientation.Vertical)
					{
						if (GetReversed())
						{
							return 100f - (_endPosition - GetMaxOffset(zoomPanel));
						}
						return 100f - _endPosition;
					}
					if (GetReversed())
					{
						return _startPosition;
					}
					return _startPosition + GetMaxOffset(zoomPanel);
				}
				return _startPosition;
			}
		}

		internal float EndPosition
		{
			get
			{
				object parent = Parent;
				if (parent is ZoomPanel)
				{
					ZoomPanel zoomPanel = (ZoomPanel)parent;
					if (zoomPanel.GetOrientation() == Orientation.Vertical)
					{
						if (GetReversed())
						{
							return 100f - _startPosition;
						}
						return 100f - (_startPosition + GetMaxOffset(zoomPanel));
					}
					if (GetReversed())
					{
						return _endPosition - GetMaxOffset(zoomPanel);
					}
				}
				return _endPosition;
			}
		}

		internal float SweepPosition => _sweepPosition;

		public LinearScale()
			: this(null)
		{
		}

		public LinearScale(object parent)
			: base(parent)
		{
			_startPosition = 8f;
			_endPosition = 92f;
			coordSystemRatio = 1f;
			width = 5f;
			InvalidateSweepPosition();
			majorTickMark = new LinearMajorTickMark(this);
			minorTickMark = new LinearMinorTickMark(this);
			labelStyle = new LinearLabelStyle(this);
			maximumPin = new LinearSpecialPosition(this);
			minimumPin = new LinearSpecialPosition(this);
		}

		public ZoomPanel GetGauge()
		{
			return (ZoomPanel)Parent;
		}

		private GraphicsPath GetBarPath(float barOffsetInside, float barOffsetOutside)
		{
			MapGraphics graph = Common.Graph;
			GraphicsPath graphicsPath = new GraphicsPath();
			float num = 0f;
			if (MajorTickMark.Visible)
			{
				num = MajorTickMark.Width / 2f;
			}
			if (MinorTickMark.Visible)
			{
				num = Math.Max(num, MinorTickMark.Width / 2f);
			}
			RectangleF relative = new RectangleF(0f, 0f, 0f, 0f);
			if (ParentGauge.GetOrientation() == Orientation.Horizontal)
			{
				relative.X = StartPosition;
				relative.Width = EndPosition - StartPosition;
				relative.Y = Position - barOffsetInside;
				relative.Height = barOffsetInside + barOffsetOutside;
				relative = graph.GetAbsoluteRectangle(relative);
				relative.Inflate(graph.GetAbsoluteDimension(num), 0f);
			}
			else
			{
				relative.Y = StartPosition;
				relative.Height = EndPosition - StartPosition;
				relative.X = Position - barOffsetInside;
				relative.Width = barOffsetInside + barOffsetOutside;
				relative = graph.GetAbsoluteRectangle(relative);
				relative.Inflate(0f, graph.GetAbsoluteDimension(num));
			}
			if (relative.Width <= 0f)
			{
				relative.Width = 1E-06f;
			}
			if (relative.Height <= 0f)
			{
				relative.Height = 1E-06f;
			}
			graphicsPath.AddRectangle(relative);
			return graphicsPath;
		}

		private void SetScaleHitTestPath(MapGraphics g)
		{
			Gap gap = new Gap(Position);
			gap.SetOffset(Placement.Cross, Width);
			gap.SetBase();
			if (MajorTickMark.Visible)
			{
				gap.SetOffsetBase(MajorTickMark.Placement, MajorTickMark.Length);
			}
			if (MinorTickMark.Visible)
			{
				gap.SetOffsetBase(MinorTickMark.Placement, MinorTickMark.Length);
			}
			using (GraphicsPath graphicsPath = GetBarPath(gap.Inside, gap.Outside))
			{
				Common.MapCore.HotRegionList.SetHotRegion(g, this, graphicsPath);
			}
		}

		internal GraphicsPath GetShadowPath()
		{
			if (Visible && ShadowOffset != 0f && Width > 0f)
			{
				GraphicsPath barPath = GetBarPath(Width / 2f, Width / 2f);
				using (Matrix matrix = new Matrix())
				{
					matrix.Translate(ShadowOffset, ShadowOffset);
					barPath.Transform(matrix);
					return barPath;
				}
			}
			return null;
		}

		private void RenderBar(MapGraphics g)
		{
			using (GraphicsPath path = GetBarPath(Width / 2f, Width / 2f))
			{
				g.DrawPathAbs(path, FillColor, FillHatchStyle, "", MapImageWrapMode.Unscaled, Color.Empty, MapImageAlign.Center, FillGradientType, FillSecondaryColor, BorderColor, BorderWidth, BorderStyle, PenAlignment.Outset);
			}
		}

		internal void DrawTickMark(MapGraphics g, CustomTickMark tickMark, double value, float offset)
		{
			float num = GetPositionFromValue(value);
			PointF absolutePoint = g.GetAbsolutePoint(GetPoint(num, offset));
			using (Matrix matrix = new Matrix())
			{
				if (ParentGauge.GetOrientation() == Orientation.Vertical)
				{
					matrix.RotateAt(90f, absolutePoint);
				}
				if (tickMark.Placement == Placement.Outside)
				{
					matrix.RotateAt(180f, absolutePoint);
				}
				DrawTickMark(g, tickMark, value, offset, matrix);
			}
		}

		internal LinearLabelStyle GetLabelStyle()
		{
			return LabelStyle;
		}

		private void DrawLabel(Placement placement, string labelStr, double value, float labelPos, float rotateLabelAngle, Font font, Color color, FontUnit fontUnit)
		{
			StringFormat stringFormat = new StringFormat();
			stringFormat.Alignment = StringAlignment.Center;
			stringFormat.LineAlignment = StringAlignment.Near;
			float num = GetPositionFromValue(value);
			MarkerPosition markerPosition = new MarkerPosition((float)Math.Round(num), value, placement);
			if (MarkerPosition.IsExistsInArray(labels, markerPosition))
			{
				return;
			}
			if (!string.IsNullOrEmpty(labelStr))
			{
				labels.Add(markerPosition);
			}
			MapGraphics graph = Common.Graph;
			using (Brush brush2 = new SolidBrush(color))
			{
				Font resizedFont = GetResizedFont(font, fontUnit);
				try
				{
					float num2 = 0f;
					if (ParentGauge.GetOrientation() == Orientation.Vertical)
					{
						num2 = 90f;
					}
					SizeF size = graph.MeasureString(labelStr, resizedFont);
					float contactPointOffset = Utils.GetContactPointOffset(size, rotateLabelAngle - num2);
					PointF absolutePoint = graph.GetAbsolutePoint(GetPoint(num, labelPos));
					switch (placement)
					{
					case Placement.Inside:
						if (ParentGauge.GetOrientation() == Orientation.Vertical)
						{
							absolutePoint.X -= contactPointOffset;
						}
						else
						{
							absolutePoint.Y -= contactPointOffset;
						}
						break;
					case Placement.Outside:
						if (ParentGauge.GetOrientation() == Orientation.Vertical)
						{
							absolutePoint.X += contactPointOffset;
						}
						else
						{
							absolutePoint.Y += contactPointOffset;
						}
						break;
					}
					RectangleF rectangleF = new RectangleF(absolutePoint, new SizeF(0f, 0f));
					rectangleF.Inflate(size.Width / 2f, size.Height / 2f);
					Matrix transform = graph.Transform;
					Matrix matrix = graph.Transform.Clone();
					try
					{
						if (rotateLabelAngle == 0f)
						{
							if (ShadowOffset != 0f)
							{
								using (Brush brush = graph.GetShadowBrush())
								{
									RectangleF layoutRectangle = rectangleF;
									layoutRectangle.Offset(ShadowOffset, ShadowOffset);
									graph.DrawString(labelStr, resizedFont, brush, layoutRectangle, stringFormat);
								}
							}
							graph.DrawString(labelStr, resizedFont, brush2, rectangleF, stringFormat);
							return;
						}
						TextRenderingHint textRenderingHint = graph.TextRenderingHint;
						try
						{
							if (textRenderingHint == TextRenderingHint.ClearTypeGridFit)
							{
								graph.TextRenderingHint = TextRenderingHint.AntiAlias;
							}
							if (ShadowOffset != 0f)
							{
								using (Brush brush3 = graph.GetShadowBrush())
								{
									using (Matrix matrix2 = matrix.Clone())
									{
										matrix2.Translate(ShadowOffset, ShadowOffset);
										matrix2.RotateAt(rotateLabelAngle, absolutePoint);
										graph.Transform = matrix2;
										graph.DrawString(labelStr, resizedFont, brush3, rectangleF, stringFormat);
									}
								}
							}
							matrix.RotateAt(rotateLabelAngle, absolutePoint);
							graph.Transform = matrix;
							graph.DrawString(labelStr, resizedFont, brush2, rectangleF, stringFormat);
						}
						finally
						{
							graph.TextRenderingHint = textRenderingHint;
						}
					}
					finally
					{
						matrix.Dispose();
						graph.Transform = transform;
					}
				}
				finally
				{
					if (resizedFont != font)
					{
						resizedFont.Dispose();
					}
				}
			}
		}

		private void RenderLabels(MapGraphics g)
		{
			if (!LabelStyle.Visible)
			{
				return;
			}
			double num = GetInterval(IntervalTypes.Labels);
			float offsetLabelPos = GetOffsetLabelPos(LabelStyle.Placement, LabelStyle.DistanceFromScale, Position);
			double minimumLog = MinimumLog;
			double num2 = GetIntervalOffset(IntervalTypes.Labels);
			Color textColor = LabelStyle.TextColor;
			if (LabelStyle.ShowEndLabels && num2 > 0.0)
			{
				textColor = LabelStyle.TextColor;
				string labelStr = string.Format(CultureInfo.CurrentCulture, LabelStyle.GetFormatStr(), minimumLog * Multiplier);
				DrawLabel(LabelStyle.Placement, labelStr, minimumLog, offsetLabelPos, LabelStyle.FontAngle, LabelStyle.Font, textColor, LabelStyle.FontUnit);
				DrawTickMark(g, MinorTickMark, minimumLog, GetTickMarkOffset(MinorTickMark));
			}
			minimumLog += num2;
			double num3 = 0.0;
			while (minimumLog <= Maximum)
			{
				bool flag = true;
				if (!LabelStyle.ShowEndLabels && (minimumLog == MinimumLog || minimumLog == Maximum))
				{
					flag = false;
				}
				if (flag)
				{
					textColor = LabelStyle.TextColor;
					string labelStr2 = string.Format(CultureInfo.CurrentCulture, LabelStyle.GetFormatStr(), minimumLog * Multiplier);
					DrawLabel(LabelStyle.Placement, labelStr2, minimumLog, offsetLabelPos, LabelStyle.FontAngle, LabelStyle.Font, textColor, LabelStyle.FontUnit);
				}
				num3 = minimumLog;
				minimumLog = GetNextPosition(minimumLog, num, forceLinear: false);
			}
			if (LabelStyle.ShowEndLabels && num3 < Maximum)
			{
				minimumLog = Maximum;
				textColor = LabelStyle.TextColor;
				string labelStr3 = string.Format(CultureInfo.CurrentCulture, LabelStyle.GetFormatStr(), minimumLog * Multiplier);
				DrawLabel(LabelStyle.Placement, labelStr3, minimumLog, offsetLabelPos, LabelStyle.FontAngle, LabelStyle.Font, textColor, LabelStyle.FontUnit);
				DrawTickMark(g, MinorTickMark, minimumLog, GetTickMarkOffset(MinorTickMark));
			}
		}

		internal void DrawSpecialPosition(MapGraphics g, SpecialPosition label, float angle)
		{
			if (label.Enable)
			{
				LinearPinLabel linearPinLabel = ((LinearSpecialPosition)label).LabelStyle;
				if (linearPinLabel.Text != string.Empty && staticRendering)
				{
					DrawLabel(linearPinLabel.Placement, linearPinLabel.Text, GetValueFromPosition(angle), GetOffsetLabelPos(linearPinLabel.Placement, linearPinLabel.DistanceFromScale, Position), linearPinLabel.FontAngle, linearPinLabel.Font, linearPinLabel.TextColor, linearPinLabel.FontUnit);
				}
				if ((label.Visible && !TickMarksOnTop) || !staticRendering)
				{
					float tickMarkOffset = GetTickMarkOffset(label);
					DrawTickMark(g, label, GetValueFromPosition(angle), tickMarkOffset);
				}
			}
		}

		internal void RenderStaticElements(MapGraphics g)
		{
			if (!Visible)
			{
				return;
			}
			g.StartHotRegion(this);
			GraphicsState gstate = g.Save();
			try
			{
				staticRendering = true;
				if (!TickMarksOnTop)
				{
					markers.Clear();
				}
				labels.Clear();
				RenderBar(g);
				if (!TickMarksOnTop)
				{
					RenderGrid(g);
				}
				RenderLabels(g);
				RenderPins(g);
				SetScaleHitTestPath(g);
				if (!TickMarksOnTop)
				{
					markers.Sort();
				}
			}
			finally
			{
				g.Restore(gstate);
				g.EndHotRegion();
			}
		}

		internal void RenderDynamicElements(MapGraphics g)
		{
			if (Visible && TickMarksOnTop)
			{
				GraphicsState gstate = g.Save();
				try
				{
					staticRendering = false;
					markers.Clear();
					RenderGrid(g);
					RenderPins(g);
					markers.Sort();
				}
				finally
				{
					g.Restore(gstate);
				}
			}
		}

		protected bool IsReversed()
		{
			if (Parent != null && ParentGauge.GetOrientation() == Orientation.Vertical)
			{
				return !GetReversed();
			}
			return GetReversed();
		}

		protected PointF GetPoint(float position, float offset)
		{
			PointF empty = PointF.Empty;
			if (ParentGauge.GetOrientation() == Orientation.Horizontal)
			{
				empty.X = position;
				empty.Y = Position + offset;
			}
			else
			{
				empty.Y = position;
				empty.X = Position + offset;
			}
			return empty;
		}

		internal double GetValue(PointF c, PointF p)
		{
			if (Common != null)
			{
				HotRegionList hotRegionList = Common.MapCore.HotRegionList;
				int num = hotRegionList.FindHotRegionOfObject(GetGauge());
				if (num != -1)
				{
					RectangleF boundingRectangle = ((HotRegion)hotRegionList.List[num]).BoundingRectangle;
					float num2 = (ParentGauge.GetOrientation() != 0) ? ((p.Y - boundingRectangle.Y) / boundingRectangle.Height * 100f) : ((p.X - boundingRectangle.X) / boundingRectangle.Width * 100f);
					return GetValueFromPosition(num2);
				}
			}
			return double.NaN;
		}

		private float GetMaxOffset(ZoomPanel gauge)
		{
			_ = gauge.AbsoluteSize.IsEmpty;
			return 0f;
		}

		internal bool GetReversed()
		{
			return Reversed;
		}

		internal Brush GetLightBrush(MapGraphics g, CustomTickMark tickMark, Color fillColor, GraphicsPath path)
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

		internal void DrawTickMark(MapGraphics g, CustomTickMark tickMark, double value, float offset, Matrix matrix)
		{
			if (tickMark.Width <= 0f || tickMark.Length <= 0f)
			{
				return;
			}
			float num = GetPositionFromValue(value);
			MarkerPosition markerPosition = new MarkerPosition((float)Math.Round(num), value, tickMark.Placement);
			if (MarkerPosition.IsExistsInArray(markers, markerPosition))
			{
				return;
			}
			markers.Add(markerPosition);
			PointF absolutePoint = g.GetAbsolutePoint(GetPoint(num, offset));
			if (tickMark.Image != string.Empty)
			{
				DrawTickMarkImage(g, tickMark, matrix, absolutePoint, drawShadow: false);
				return;
			}
			SizeF sizeF = new SizeF(g.GetAbsoluteDimension(tickMark.Width), g.GetAbsoluteDimension(tickMark.Length));
			Color color = tickMark.FillColor;
			using (GraphicsPath graphicsPath = g.CreateMarker(absolutePoint, sizeF.Width, sizeF.Height, tickMark.Shape))
			{
				using (Brush brush = GetLightBrush(g, tickMark, color, graphicsPath))
				{
					graphicsPath.Transform(matrix);
					if (tickMark.EnableGradient)
					{
						if (brush is LinearGradientBrush)
						{
							((LinearGradientBrush)brush).Transform = matrix;
						}
						else if (brush is PathGradientBrush)
						{
							((PathGradientBrush)brush).Transform = matrix;
						}
					}
					if (ShadowOffset != 0f)
					{
						g.DrawPathShadowAbs(graphicsPath, g.GetShadowColor(), ShadowOffset);
					}
					g.FillPath(brush, graphicsPath, 0f, useBrushOffset: false, circularFill: false);
					if (tickMark.BorderWidth > 0)
					{
						using (Pen pen = new Pen(tickMark.BorderColor, tickMark.BorderWidth))
						{
							pen.Alignment = PenAlignment.Outset;
							g.DrawPath(pen, graphicsPath);
						}
					}
				}
			}
		}

		internal void DrawTickMarkImage(MapGraphics g, CustomTickMark tickMark, Matrix matrix, PointF centerPoint, bool drawShadow)
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
					colorMatrix.Matrix33 = Common.MapCore.ShadowIntensity / 100f;
					imageAttributes.SetColorMatrix(colorMatrix);
					matrix2.Translate(ShadowOffset, ShadowOffset, MatrixOrder.Append);
				}
				destRect.X = (int)(centerPoint.X - (float)(destRect.Width / 2));
				destRect.Y = (int)(centerPoint.Y - (float)(destRect.Height / 2));
				g.Transform = matrix2;
				g.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
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
				throw new InvalidOperationException(SR.invalid_placement_type);
			}
		}

		internal void RenderTicks(MapGraphics g, TickMark tickMark, double interval, double max, double min, double intOffset, bool forceLinear)
		{
			float tickMarkOffset = GetTickMarkOffset(tickMark);
			for (double num = min + intOffset; num <= max; num = GetNextPosition(num, interval, forceLinear))
			{
				DrawTickMark(g, tickMark, num, tickMarkOffset);
			}
		}

		internal void RenderGrid(MapGraphics g)
		{
			if (MajorTickMark.Visible)
			{
				RenderTicks(g, MajorTickMark, GetInterval(IntervalTypes.Major), Maximum, MinimumLog, GetIntervalOffset(IntervalTypes.Major), forceLinear: false);
			}
			if (!MinorTickMark.Visible)
			{
				return;
			}
			if (!Logarithmic)
			{
				RenderTicks(g, MinorTickMark, GetInterval(IntervalTypes.Minor), Maximum, MinimumLog, GetIntervalOffset(IntervalTypes.Minor), forceLinear: false);
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
				RenderTicks(g, MinorTickMark, nextPosition / num4, Math.Min(nextPosition, Maximum), num2, num, forceLinear: true);
				num2 = nextPosition;
				nextPosition = GetNextPosition(nextPosition, num3, forceLinear: false);
			}
		}

		internal float GetOffsetLabelPos(Placement placement, float scaleOffset, float scalePosition)
		{
			Gap gap = new Gap(scalePosition);
			gap.SetOffset(Placement.Cross, Width);
			gap.SetBase();
			if (MajorTickMark.Visible)
			{
				gap.SetOffsetBase(MajorTickMark.Placement, MajorTickMark.Length);
			}
			if (MinorTickMark.Visible)
			{
				gap.SetOffsetBase(MinorTickMark.Placement, MinorTickMark.Length);
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
				throw new InvalidOperationException(SR.invalid_placement_type);
			}
		}

		internal Font GetResizedFont(Font font, FontUnit fontUnit)
		{
			if (fontUnit == FontUnit.Percent)
			{
				float absoluteDimension = Common.Graph.GetAbsoluteDimension(font.Size);
				return new Font(font.FontFamily.Name, absoluteDimension, font.Style, GraphicsUnit.Pixel, font.GdiCharSet, font.GdiVerticalFont);
			}
			return font;
		}

		internal void RenderPins(MapGraphics g)
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
			float num = StartPosition - MinimumPin.Location;
			float num2 = EndPosition + MaximumPin.Location;
			if (IsReversed())
			{
				num = EndPosition + MinimumPin.Location;
				num2 = StartPosition - MaximumPin.Location;
			}
			if (double.IsNaN(value))
			{
				if (MinimumPin.Enable)
				{
					return GetValueFromPosition(num);
				}
				return MinimumLog;
			}
			double num3 = MinimumLog;
			if (MinimumPin.Enable)
			{
				num3 = GetValueFromPosition(num);
			}
			double valueFromPosition = Maximum;
			if (MaximumPin.Enable)
			{
				valueFromPosition = GetValueFromPosition(num2);
			}
			if (value < num3)
			{
				return num3;
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
				num = MinorTickMark.IntervalOffset;
				if (double.IsNaN(num))
				{
					num = GetIntervalOffset(IntervalTypes.Major) % GetInterval(IntervalTypes.Minor);
				}
				break;
			case IntervalTypes.Major:
				num = MajorTickMark.IntervalOffset;
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
				num = MinorTickMark.Interval;
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
				num = MajorTickMark.Interval;
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
					while (Math.Round(num3, 0) != num3 || num3 > num5)
					{
						num += num2;
						num3 = (Maximum - MinimumLog) / num;
						if (num3 <= Math.Max(num5 / 2.0, 1.0))
						{
							break;
						}
					}
				}
				else
				{
					num = 1.0;
				}
				break;
			}
			if ((Maximum - MinimumLog) / num > 1000.0)
			{
				return (Maximum - MinimumLog) / 1000.0;
			}
			return num;
		}

		internal double GetNextPosition(double position, double interval, bool forceLinear)
		{
			position = ((!forceLinear && Logarithmic) ? Math.Pow(LogarithmicBase, Math.Log(position, LogarithmicBase) + interval) : (position + interval));
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
			throw new ApplicationException(SR.gdi_noninitialized);
		}

		internal override void BeginInit()
		{
			base.BeginInit();
		}

		internal override void EndInit()
		{
			base.EndInit();
		}

		internal override void Invalidate()
		{
			base.Invalidate();
		}

		string IToolTipProvider.GetToolTip()
		{
			return ToolTip;
		}
	}
}
