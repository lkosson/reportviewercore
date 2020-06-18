using Microsoft.Reporting.Chart.WebForms.ChartTypes;
using Microsoft.Reporting.Chart.WebForms.Design;
using Microsoft.Reporting.Chart.WebForms.Utilities;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace Microsoft.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeAxis_Axis")]
	[DefaultProperty("Enabled")]
	internal class Axis : AxisLabels
	{
		internal Chart chart;

		private bool storeValuesEnabled = true;

		private string name = "";

		private Font titleFont = new Font(ChartPicture.GetDefaultFontFamilyName(), 8f);

		private Color titleColor = Color.Black;

		private StringAlignment titleAlignment = StringAlignment.Center;

		private string title = "";

		private int lineWidth = 1;

		private ChartDashStyle lineDashStyle = ChartDashStyle.Solid;

		private Color lineColor = Color.Black;

		private bool autoFit = true;

		private ArrowsType arrows;

		private StripLinesCollection stripLines;

		private bool nextToAxis = true;

		private TextOrientation textOrientation;

		internal float titleSize;

		internal float labelSize;

		internal float labelNearOffset;

		internal float labelFarOffset;

		internal float unRotatedLabelSize;

		internal float markSize;

		internal float scrollBarSize;

		internal float totlaGroupingLabelsSize;

		internal float[] groupingLabelSizes;

		internal float totlaGroupingLabelsSizeAdjustment;

		private LabelsAutoFitStyles labelsAutoFitStyle = LabelsAutoFitStyles.IncreaseFont | LabelsAutoFitStyles.DecreaseFont | LabelsAutoFitStyles.OffsetLabels | LabelsAutoFitStyles.LabelsAngleStep30 | LabelsAutoFitStyles.WordWrap;

		internal Font autoLabelFont;

		internal int autoLabelAngle = -1000;

		internal int autoLabelOffset = -1;

		private float aveLabelFontSize = 10f;

		private float minLabelFontSize = 5f;

		private RectangleF titlePosition = RectangleF.Empty;

		internal const float elementSpacing = 1f;

		private const float maxAxisElementsSize = 75f;

		private const float maxAxisTitleSize = 20f;

		private const float maxAxisLabelRow2Size = 45f;

		private const float maxAxisMarkSize = 20f;

		internal double minimumFromData = double.NaN;

		internal double maximumFromData = double.NaN;

		internal bool refreshMinMaxFromData = true;

		internal int numberOfPointsInAllSeries;

		private double originalViewPosition = double.NaN;

		private bool interlaced;

		private Color interlacedColor = Color.Empty;

		private double intervalOffset;

		internal double interval;

		internal DateTimeIntervalType intervalType;

		internal DateTimeIntervalType intervalOffsetType;

		internal int labelsAutoFitMinFontSize = 6;

		internal int labelsAutoFitMaxFontSize = 10;

		private string toolTip = string.Empty;

		private string href = string.Empty;

		private string mapAreaAttributes = string.Empty;

		public const double maxdecimal = 7.9228162514264338E+28;

		public const double smallestPositiveDecimal = 7.9228162514264341E-28;

		private ChartValueTypes valueType;

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(TextOrientation.Auto)]
		[SRDescription("DescriptionAttribute_TextOrientation")]
		[NotifyParentProperty(true)]
		public TextOrientation TextOrientation
		{
			get
			{
				return textOrientation;
			}
			set
			{
				textOrientation = value;
				Invalidate();
			}
		}

		internal virtual string SubAxisName => string.Empty;

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(false)]
		[SRDescription("DescriptionAttributeInterlaced")]
		[NotifyParentProperty(true)]
		public bool Interlaced
		{
			get
			{
				return interlaced;
			}
			set
			{
				interlaced = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeInterlacedColor")]
		[NotifyParentProperty(true)]
		public Color InterlacedColor
		{
			get
			{
				return interlacedColor;
			}
			set
			{
				interlacedColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[Browsable(false)]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeAxis_Name")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public virtual string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[Browsable(false)]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeType")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public virtual AxisName Type => axisType;

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(ArrowsType.None)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeArrows")]
		public ArrowsType Arrows
		{
			get
			{
				return arrows;
			}
			set
			{
				arrows = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeGridTickMarks")]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeMajorGrid")]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		public Grid MajorGrid
		{
			get
			{
				return majorGrid;
			}
			set
			{
				majorGrid = value;
				majorGrid.axis = this;
				majorGrid.majorGridTick = true;
				if (!majorGrid.intervalChanged)
				{
					majorGrid.Interval = double.NaN;
				}
				if (!majorGrid.intervalOffsetChanged)
				{
					majorGrid.IntervalOffset = double.NaN;
				}
				if (!majorGrid.intervalTypeChanged)
				{
					majorGrid.IntervalType = DateTimeIntervalType.NotSet;
				}
				if (!majorGrid.intervalOffsetTypeChanged)
				{
					majorGrid.IntervalOffsetType = DateTimeIntervalType.NotSet;
				}
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeGridTickMarks")]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeMinorGrid")]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		public Grid MinorGrid
		{
			get
			{
				return minorGrid;
			}
			set
			{
				minorGrid = value;
				minorGrid.Initialize(this, major: false);
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeGridTickMarks")]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeMajorTickMark")]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		public TickMark MajorTickMark
		{
			get
			{
				return majorTickMark;
			}
			set
			{
				majorTickMark = value;
				majorTickMark.axis = this;
				majorTickMark.majorGridTick = true;
				if (!majorTickMark.intervalChanged)
				{
					majorTickMark.Interval = double.NaN;
				}
				if (!majorTickMark.intervalOffsetChanged)
				{
					majorTickMark.IntervalOffset = double.NaN;
				}
				if (!majorTickMark.intervalTypeChanged)
				{
					majorTickMark.IntervalType = DateTimeIntervalType.NotSet;
				}
				if (!majorTickMark.intervalOffsetTypeChanged)
				{
					majorTickMark.IntervalOffsetType = DateTimeIntervalType.NotSet;
				}
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeGridTickMarks")]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeMinorTickMark")]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		public TickMark MinorTickMark
		{
			get
			{
				return minorTickMark;
			}
			set
			{
				minorTickMark = value;
				minorTickMark.Initialize(this, major: false);
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeLabels")]
		[Bindable(true)]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeLabelsAutoFit")]
		[NotifyParentProperty(true)]
		[RefreshProperties(RefreshProperties.All)]
		public bool LabelsAutoFit
		{
			get
			{
				return autoFit;
			}
			set
			{
				autoFit = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeLabels")]
		[Bindable(true)]
		[DefaultValue(6)]
		[SRDescription("DescriptionAttributeLabelsAutoFitMinFontSize")]
		[NotifyParentProperty(true)]
		[RefreshProperties(RefreshProperties.All)]
		public int LabelsAutoFitMinFontSize
		{
			get
			{
				return labelsAutoFitMinFontSize;
			}
			set
			{
				if (value < 5)
				{
					throw new InvalidOperationException(SR.ExceptionAxisLabelsAutoFitMinFontSizeValueInvalid);
				}
				labelsAutoFitMinFontSize = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeLabels")]
		[Bindable(true)]
		[DefaultValue(10)]
		[SRDescription("DescriptionAttributeLabelsAutoFitMaxFontSize")]
		[NotifyParentProperty(true)]
		[RefreshProperties(RefreshProperties.All)]
		public int LabelsAutoFitMaxFontSize
		{
			get
			{
				return labelsAutoFitMaxFontSize;
			}
			set
			{
				if (value < 5)
				{
					throw new InvalidOperationException(SR.ExceptionAxisLabelsAutoFitMaxFontSizeInvalid);
				}
				labelsAutoFitMaxFontSize = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeLabels")]
		[Bindable(true)]
		[DefaultValue(LabelsAutoFitStyles.IncreaseFont | LabelsAutoFitStyles.DecreaseFont | LabelsAutoFitStyles.OffsetLabels | LabelsAutoFitStyles.LabelsAngleStep30 | LabelsAutoFitStyles.WordWrap)]
		[SRDescription("DescriptionAttributeLabelsAutoFitStyle")]
		[NotifyParentProperty(true)]
		public LabelsAutoFitStyles LabelsAutoFitStyle
		{
			get
			{
				return labelsAutoFitStyle;
			}
			set
			{
				labelsAutoFitStyle = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeMarksNextToAxis")]
		[NotifyParentProperty(true)]
		public virtual bool MarksNextToAxis
		{
			get
			{
				return nextToAxis;
			}
			set
			{
				nextToAxis = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeTitle")]
		[Bindable(true)]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeTitle6")]
		[NotifyParentProperty(true)]
		public string Title
		{
			get
			{
				return title;
			}
			set
			{
				title = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeTitle")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "Black")]
		[SRDescription("DescriptionAttributeTitleColor")]
		[NotifyParentProperty(true)]
		public Color TitleColor
		{
			get
			{
				return titleColor;
			}
			set
			{
				titleColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeTitle")]
		[Bindable(true)]
		[DefaultValue(typeof(StringAlignment), "Center")]
		[SRDescription("DescriptionAttributeTitleAlignment")]
		[NotifyParentProperty(true)]
		public StringAlignment TitleAlignment
		{
			get
			{
				return titleAlignment;
			}
			set
			{
				titleAlignment = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeTitle")]
		[Bindable(true)]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 8pt")]
		[SRDescription("DescriptionAttributeTitleFont5")]
		[NotifyParentProperty(true)]
		public Font TitleFont
		{
			get
			{
				return titleFont;
			}
			set
			{
				titleFont = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "Black")]
		[SRDescription("DescriptionAttributeLineColor8")]
		[NotifyParentProperty(true)]
		public Color LineColor
		{
			get
			{
				return lineColor;
			}
			set
			{
				lineColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(1)]
		[SRDescription("DescriptionAttributeLineWidth9")]
		[NotifyParentProperty(true)]
		public int LineWidth
		{
			get
			{
				return lineWidth;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("value", SR.ExceptionAxisWidthIsNegative);
				}
				lineWidth = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(ChartDashStyle.Solid)]
		[SRDescription("DescriptionAttributeLineStyle7")]
		[NotifyParentProperty(true)]
		public ChartDashStyle LineStyle
		{
			get
			{
				return lineDashStyle;
			}
			set
			{
				lineDashStyle = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeStripLines")]
		public StripLinesCollection StripLines => stripLines;

		[SRCategory("CategoryAttributeMapArea")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeToolTip")]
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

		[SRCategory("CategoryAttributeMapArea")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeAxis_Href")]
		[DefaultValue("")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
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

		[SRCategory("CategoryAttributeMapArea")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeAxis_MapAreaAttributes")]
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

		[SRCategory("CategoryAttributeInterval")]
		[Bindable(true)]
		[DefaultValue(0.0)]
		[SRDescription("DescriptionAttributeInterval4")]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(AxisIntervalValueConverter))]
		public double Interval
		{
			get
			{
				return interval;
			}
			set
			{
				if (double.IsNaN(value))
				{
					interval = 0.0;
				}
				else
				{
					interval = value;
				}
				majorGrid.interval = tempMajorGridInterval;
				majorTickMark.interval = tempMajorTickMarkInterval;
				minorGrid.interval = tempMinorGridInterval;
				minorTickMark.interval = tempMinorTickMarkInterval;
				labelStyle.interval = tempLabelInterval;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeInterval")]
		[Bindable(true)]
		[DefaultValue(0.0)]
		[SRDescription("DescriptionAttributeIntervalOffset6")]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(AxisIntervalValueConverter))]
		public double IntervalOffset
		{
			get
			{
				return intervalOffset;
			}
			set
			{
				if (double.IsNaN(value))
				{
					intervalOffset = 0.0;
				}
				else
				{
					intervalOffset = value;
				}
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeInterval")]
		[Bindable(true)]
		[DefaultValue(DateTimeIntervalType.Auto)]
		[SRDescription("DescriptionAttributeIntervalType4")]
		[RefreshProperties(RefreshProperties.All)]
		public DateTimeIntervalType IntervalType
		{
			get
			{
				return intervalType;
			}
			set
			{
				if (value == DateTimeIntervalType.NotSet)
				{
					intervalType = DateTimeIntervalType.Auto;
				}
				else
				{
					intervalType = value;
				}
				majorGrid.intervalType = tempGridIntervalType;
				majorTickMark.intervalType = tempTickMarkIntervalType;
				labelStyle.intervalType = tempLabelIntervalType;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeInterval")]
		[Bindable(true)]
		[DefaultValue(DateTimeIntervalType.Auto)]
		[SRDescription("DescriptionAttributeIntervalOffsetType4")]
		[RefreshProperties(RefreshProperties.All)]
		public DateTimeIntervalType IntervalOffsetType
		{
			get
			{
				return intervalOffsetType;
			}
			set
			{
				if (value == DateTimeIntervalType.NotSet)
				{
					intervalOffsetType = DateTimeIntervalType.Auto;
				}
				else
				{
					intervalOffsetType = value;
				}
				Invalidate();
			}
		}

		private bool IsTextVertical
		{
			get
			{
				TextOrientation textOrientation = GetTextOrientation();
				if (textOrientation != TextOrientation.Rotated90)
				{
					return textOrientation == TextOrientation.Rotated270;
				}
				return true;
			}
		}

		[SRCategory("CategoryAttributeData")]
		[Bindable(true)]
		[Browsable(false)]
		[SRDescription("DescriptionAttributeAxis_ValueType")]
		[DefaultValue(ChartValueTypes.Auto)]
		public ChartValueTypes ValueType
		{
			get
			{
				return valueType;
			}
			set
			{
				valueType = value;
				Invalidate();
			}
		}

		public Axis()
		{
			Initialize(null, AxisName.X);
		}

		internal void Initialize(ChartArea chartArea, AxisName axisType)
		{
			base.chartArea = chartArea;
			base.axisType = axisType;
			if (chartArea != null && chartArea.Common != null && chartArea.Common.Chart != null)
			{
				chart = chartArea.Common.Chart;
			}
			SetName();
			if (minorTickMark == null)
			{
				minorTickMark = new TickMark(this, major: false);
			}
			if (majorTickMark == null)
			{
				majorTickMark = new TickMark(this, major: true);
				majorTickMark.Interval = double.NaN;
				majorTickMark.IntervalOffset = double.NaN;
				majorTickMark.IntervalType = DateTimeIntervalType.NotSet;
				majorTickMark.IntervalOffsetType = DateTimeIntervalType.NotSet;
			}
			if (minorGrid == null)
			{
				minorGrid = new Grid(this, major: false);
			}
			if (majorGrid == null)
			{
				majorGrid = new Grid(this, major: true);
				majorGrid.Interval = double.NaN;
				majorGrid.IntervalOffset = double.NaN;
				majorGrid.IntervalType = DateTimeIntervalType.NotSet;
				majorGrid.IntervalOffsetType = DateTimeIntervalType.NotSet;
			}
			if (stripLines == null)
			{
				stripLines = new StripLinesCollection(this);
			}
			base.ScrollBar.Initialize();
			if (scaleSegments == null)
			{
				scaleSegments = new AxisScaleSegmentCollection(this);
			}
			if (axisScaleBreakStyle == null)
			{
				axisScaleBreakStyle = new AxisScaleBreakStyle(this);
			}
		}

		internal void SetName()
		{
			switch (axisType)
			{
			case AxisName.X:
				name = SR.TitleAxisX;
				break;
			case AxisName.Y:
				name = SR.TitleAxisY;
				break;
			case AxisName.X2:
				name = SR.TitleAxisX2;
				break;
			case AxisName.Y2:
				name = SR.TitleAxisY2;
				break;
			}
		}

		private TextOrientation GetTextOrientation()
		{
			if (TextOrientation == TextOrientation.Auto)
			{
				if (AxisPosition == AxisPosition.Left)
				{
					return TextOrientation.Rotated270;
				}
				if (AxisPosition == AxisPosition.Right)
				{
					return TextOrientation.Rotated90;
				}
				return TextOrientation.Horizontal;
			}
			return TextOrientation;
		}

		internal void PrePaint(ChartGraphics graph)
		{
			if (enabled)
			{
				majorTickMark.Paint(graph, backElements: true);
				minorTickMark.Paint(graph, backElements: true);
				DrawAxisLine(graph, backElements: true);
				labelStyle.Paint(graph, backElements: true);
			}
		}

		internal void Paint(ChartGraphics graph)
		{
			if (chartArea != null && chartArea.chartAreaIsCurcular)
			{
				if (axisType == AxisName.Y && enabled)
				{
					ICircularChartType circularChartType = chartArea.GetCircularChartType();
					if (circularChartType != null)
					{
						Matrix transform = graph.Transform;
						float[] yAxisLocations = circularChartType.GetYAxisLocations(chartArea);
						bool flag = true;
						float[] array = yAxisLocations;
						foreach (float num in array)
						{
							Matrix matrix = transform.Clone();
							matrix.RotateAt(num, graph.GetAbsolutePoint(chartArea.circularCenter));
							graph.Transform = matrix;
							minorTickMark.Paint(graph, backElements: false);
							majorTickMark.Paint(graph, backElements: false);
							DrawAxisLine(graph, backElements: false);
							if (!flag)
							{
								continue;
							}
							flag = false;
							int fontAngle = labelStyle.FontAngle;
							if (labelStyle.FontAngle == 0)
							{
								if (num >= 45f && num <= 180f)
								{
									labelStyle.fontAngle = -90;
								}
								else if (num > 180f && num <= 315f)
								{
									labelStyle.fontAngle = 90;
								}
							}
							labelStyle.Paint(graph, backElements: false);
							labelStyle.fontAngle = fontAngle;
						}
						graph.Transform = transform;
					}
				}
				if (axisType == AxisName.X && enabled)
				{
					labelStyle.PaintCircular(graph);
				}
				DrawAxisTitle(graph);
				return;
			}
			if (enabled)
			{
				minorTickMark.Paint(graph, backElements: false);
				majorTickMark.Paint(graph, backElements: false);
				DrawAxisLine(graph, backElements: false);
				labelStyle.Paint(graph, backElements: false);
				if (chartArea != null && !chartArea.Area3DStyle.Enable3D)
				{
					base.ScrollBar.Paint(graph);
				}
			}
			DrawAxisTitle(graph);
			ResetTempAxisOffset();
		}

		internal void PaintOnSegmentedScalePassOne(ChartGraphics graph)
		{
			if (enabled)
			{
				minorTickMark.Paint(graph, backElements: false);
				majorTickMark.Paint(graph, backElements: false);
			}
		}

		internal void PaintOnSegmentedScalePassTwo(ChartGraphics graph)
		{
			if (enabled)
			{
				DrawAxisLine(graph, backElements: false);
				labelStyle.Paint(graph, backElements: false);
			}
			DrawAxisTitle(graph);
			ResetTempAxisOffset();
		}

		private void DrawAxisTitle(ChartGraphics graph)
		{
			if (!enabled || Title.Length <= 0)
			{
				return;
			}
			Matrix matrix = null;
			if (chartArea.Area3DStyle.Enable3D && !chartArea.chartAreaIsCurcular)
			{
				DrawAxis3DTitle(graph);
				return;
			}
			string text = Title;
			if (chart != null && chart.LocalizeTextHandler != null)
			{
				text = chart.LocalizeTextHandler(this, text, 0, ChartElementType.Axis);
			}
			float num = (float)GetAxisPosition();
			if (AxisPosition == AxisPosition.Bottom)
			{
				if (!IsMarksNextToAxis())
				{
					num = chartArea.PlotAreaPosition.Bottom();
				}
				num = chartArea.PlotAreaPosition.Bottom() - num;
			}
			else if (AxisPosition == AxisPosition.Top)
			{
				if (!IsMarksNextToAxis())
				{
					num = chartArea.PlotAreaPosition.Y;
				}
				num -= chartArea.PlotAreaPosition.Y;
			}
			else if (AxisPosition == AxisPosition.Right)
			{
				if (!IsMarksNextToAxis())
				{
					num = chartArea.PlotAreaPosition.Right();
				}
				num = chartArea.PlotAreaPosition.Right() - num;
			}
			else if (AxisPosition == AxisPosition.Left)
			{
				if (!IsMarksNextToAxis())
				{
					num = chartArea.PlotAreaPosition.X;
				}
				num -= chartArea.PlotAreaPosition.X;
			}
			float num2 = markSize + labelSize;
			num2 -= num;
			if (num2 < 0f)
			{
				num2 = 0f;
			}
			StringFormat stringFormat = new StringFormat();
			stringFormat.Alignment = TitleAlignment;
			stringFormat.Trimming = StringTrimming.EllipsisCharacter;
			stringFormat.FormatFlags |= StringFormatFlags.FitBlackBox;
			titlePosition = chartArea.PlotAreaPosition.ToRectangleF();
			float num3 = titleSize - 1f;
			if (AxisPosition == AxisPosition.Left)
			{
				titlePosition.X = chartArea.PlotAreaPosition.X - num3 - num2;
				titlePosition.Y = chartArea.PlotAreaPosition.Y;
				if (!IsTextVertical)
				{
					SizeF sizeF = new SizeF(num3, chartArea.PlotAreaPosition.Height);
					titlePosition.Width = sizeF.Width;
					titlePosition.Height = sizeF.Height;
					stringFormat.Alignment = StringAlignment.Center;
					if (TitleAlignment == StringAlignment.Far)
					{
						stringFormat.LineAlignment = StringAlignment.Near;
					}
					else if (TitleAlignment == StringAlignment.Near)
					{
						stringFormat.LineAlignment = StringAlignment.Far;
					}
					else
					{
						stringFormat.LineAlignment = StringAlignment.Center;
					}
				}
				else
				{
					SizeF absoluteSize = graph.GetAbsoluteSize(new SizeF(num3, chartArea.PlotAreaPosition.Height));
					absoluteSize = graph.GetRelativeSize(new SizeF(absoluteSize.Height, absoluteSize.Width));
					titlePosition.Width = absoluteSize.Width;
					titlePosition.Height = absoluteSize.Height;
					titlePosition.Y += chartArea.PlotAreaPosition.Height / 2f - titlePosition.Height / 2f;
					titlePosition.X += titleSize / 2f - titlePosition.Width / 2f;
					matrix = SetRotationTransformation(graph, titlePosition);
					stringFormat.LineAlignment = StringAlignment.Center;
				}
			}
			else if (AxisPosition == AxisPosition.Right)
			{
				titlePosition.X = chartArea.PlotAreaPosition.Right() + num2;
				titlePosition.Y = chartArea.PlotAreaPosition.Y;
				if (!IsTextVertical)
				{
					SizeF sizeF2 = new SizeF(num3, chartArea.PlotAreaPosition.Height);
					titlePosition.Width = sizeF2.Width;
					titlePosition.Height = sizeF2.Height;
					stringFormat.Alignment = StringAlignment.Center;
					if (TitleAlignment == StringAlignment.Far)
					{
						stringFormat.LineAlignment = StringAlignment.Near;
					}
					else if (TitleAlignment == StringAlignment.Near)
					{
						stringFormat.LineAlignment = StringAlignment.Far;
					}
					else
					{
						stringFormat.LineAlignment = StringAlignment.Center;
					}
				}
				else
				{
					SizeF absoluteSize2 = graph.GetAbsoluteSize(new SizeF(num3, chartArea.PlotAreaPosition.Height));
					absoluteSize2 = graph.GetRelativeSize(new SizeF(absoluteSize2.Height, absoluteSize2.Width));
					titlePosition.Width = absoluteSize2.Width;
					titlePosition.Height = absoluteSize2.Height;
					titlePosition.Y += chartArea.PlotAreaPosition.Height / 2f - titlePosition.Height / 2f;
					titlePosition.X += titleSize / 2f - titlePosition.Width / 2f;
					matrix = SetRotationTransformation(graph, titlePosition);
					stringFormat.LineAlignment = StringAlignment.Center;
				}
			}
			else if (AxisPosition == AxisPosition.Top)
			{
				titlePosition.Y = chartArea.PlotAreaPosition.Y - num3 - num2;
				titlePosition.Height = num3;
				titlePosition.X = chartArea.PlotAreaPosition.X;
				titlePosition.Width = chartArea.PlotAreaPosition.Width;
				if (IsTextVertical)
				{
					matrix = SetRotationTransformation(graph, titlePosition);
				}
				stringFormat.LineAlignment = StringAlignment.Center;
			}
			else if (AxisPosition == AxisPosition.Bottom)
			{
				titlePosition.Y = chartArea.PlotAreaPosition.Bottom() + num2;
				titlePosition.Height = num3;
				titlePosition.X = chartArea.PlotAreaPosition.X;
				titlePosition.Width = chartArea.PlotAreaPosition.Width;
				if (IsTextVertical)
				{
					matrix = SetRotationTransformation(graph, titlePosition);
				}
				stringFormat.LineAlignment = StringAlignment.Center;
			}
			graph.DrawStringRel(text.Replace("\\n", "\n"), TitleFont, new SolidBrush(TitleColor), titlePosition, stringFormat, GetTextOrientation());
			if (base.Common.ProcessModeRegions)
			{
				RectangleF absoluteRectangle = graph.GetAbsoluteRectangle(titlePosition);
				PointF[] array = new PointF[2]
				{
					new PointF(absoluteRectangle.X, absoluteRectangle.Y),
					new PointF(absoluteRectangle.Right, absoluteRectangle.Bottom)
				};
				graph.Transform.TransformPoints(array);
				absoluteRectangle = new RectangleF(array[0].X, array[0].Y, array[1].X - array[0].X, array[1].Y - array[0].Y);
				if (absoluteRectangle.Width < 0f)
				{
					absoluteRectangle.Width = Math.Abs(absoluteRectangle.Width);
					absoluteRectangle.X -= absoluteRectangle.Width;
				}
				if (absoluteRectangle.Height < 0f)
				{
					absoluteRectangle.Height = Math.Abs(absoluteRectangle.Height);
					absoluteRectangle.Y -= absoluteRectangle.Height;
				}
				base.Common.HotRegionsList.AddHotRegion(absoluteRectangle, this, ChartElementType.AxisTitle, relativeCoordinates: false, insertAtBeginning: false);
			}
			if (matrix != null)
			{
				graph.Transform = matrix;
			}
		}

		private Matrix SetRotationTransformation(ChartGraphics graph, RectangleF titlePosition)
		{
			Matrix result = graph.Transform.Clone();
			PointF empty = PointF.Empty;
			empty.X = titlePosition.X + titlePosition.Width / 2f;
			empty.Y = titlePosition.Y + titlePosition.Height / 2f;
			float angle = (GetTextOrientation() == TextOrientation.Rotated90) ? 90f : (-90f);
			Matrix matrix = graph.Transform.Clone();
			matrix.RotateAt(angle, graph.GetAbsolutePoint(empty));
			graph.Transform = matrix;
			return result;
		}

		internal void DrawRadialLine(object obj, ChartGraphics graph, Color color, int width, ChartDashStyle style, double position)
		{
			RectangleF relative = chartArea.PlotAreaPosition.ToRectangleF();
			relative = graph.GetAbsoluteRectangle(relative);
			if (relative.Width != relative.Height)
			{
				if (relative.Width > relative.Height)
				{
					relative.X += (relative.Width - relative.Height) / 2f;
					relative.Width = relative.Height;
				}
				else
				{
					relative.Y += (relative.Height - relative.Width) / 2f;
					relative.Height = relative.Width;
				}
			}
			float angle = chartArea.CircularPositionToAngle(position);
			Region clip = null;
			if (chartArea.CircularUsePolygons)
			{
				clip = graph.Clip;
				graph.Clip = new Region(graph.GetPolygonCirclePath(relative, chartArea.CircularSectorsNumber));
			}
			PointF absolutePoint = graph.GetAbsolutePoint(chartArea.circularCenter);
			Matrix transform = graph.Transform;
			Matrix matrix = transform.Clone();
			matrix.RotateAt(angle, absolutePoint);
			graph.Transform = matrix;
			PointF pointF = new PointF(relative.X + relative.Width / 2f, relative.Y);
			graph.DrawLineAbs(color, width, style, absolutePoint, pointF);
			if (base.Common.ProcessModeRegions)
			{
				GraphicsPath graphicsPath = new GraphicsPath();
				graphicsPath.AddLine(absolutePoint, pointF);
				graphicsPath.Transform(matrix);
				try
				{
					using (Pen pen = new Pen(Color.Black, width + 2))
					{
						ChartGraphics.Widen(graphicsPath, pen);
						base.Common.HotRegionsList.AddHotRegion(graphicsPath, relativePath: false, graph, ChartElementType.Gridlines, obj);
					}
				}
				catch
				{
				}
			}
			graph.Transform = transform;
			matrix.Dispose();
			if (chartArea.CircularUsePolygons)
			{
				graph.Clip = clip;
			}
		}

		internal void DrawCircularLine(object obj, ChartGraphics graph, Color color, int width, ChartDashStyle style, float position)
		{
			RectangleF relative = chartArea.PlotAreaPosition.ToRectangleF();
			relative = graph.GetAbsoluteRectangle(relative);
			if (relative.Width != relative.Height)
			{
				if (relative.Width > relative.Height)
				{
					relative.X += (relative.Width - relative.Height) / 2f;
					relative.Width = relative.Height;
				}
				else
				{
					relative.Y += (relative.Height - relative.Width) / 2f;
					relative.Height = relative.Width;
				}
			}
			float num = graph.GetAbsolutePoint(new PointF(position, position)).Y - relative.Top;
			relative.Inflate(0f - num, 0f - num);
			Pen pen = new Pen(color, width);
			pen.DashStyle = graph.GetPenStyle(style);
			if (chartArea.CircularUsePolygons)
			{
				graph.DrawCircleAbs(pen, null, relative, chartArea.CircularSectorsNumber, circle3D: false);
			}
			else
			{
				graph.DrawEllipse(pen, relative);
			}
			if (base.Common.ProcessModeRegions && relative.Width >= 1f && relative.Height > 1f)
			{
				GraphicsPath graphicsPath = new GraphicsPath();
				if (chartArea.CircularUsePolygons)
				{
					graphicsPath = graph.GetPolygonCirclePath(relative, chartArea.CircularSectorsNumber);
				}
				else
				{
					graphicsPath.AddEllipse(relative);
				}
				try
				{
					pen.Width += 2f;
					ChartGraphics.Widen(graphicsPath, pen);
					base.Common.HotRegionsList.AddHotRegion(graphicsPath, relativePath: false, graph, ChartElementType.Gridlines, obj);
				}
				catch
				{
				}
			}
		}

		private void DrawAxis3DTitle(ChartGraphics graph)
		{
			if (!enabled)
			{
				return;
			}
			string text = Title;
			if (chart != null && chart.LocalizeTextHandler != null)
			{
				text = chart.LocalizeTextHandler(this, text, 0, ChartElementType.Axis);
			}
			PointF pointF = PointF.Empty;
			int num = 0;
			StringFormat stringFormat = new StringFormat();
			stringFormat.Alignment = TitleAlignment;
			stringFormat.Trimming = StringTrimming.EllipsisCharacter;
			stringFormat.FormatFlags |= StringFormatFlags.LineLimit;
			SizeF sizeF = graph.MeasureString(text.Replace("\\n", "\n"), TitleFont, new SizeF(10000f, 10000f), stringFormat, GetTextOrientation());
			SizeF sizeF2 = SizeF.Empty;
			if (stringFormat.Alignment != StringAlignment.Center)
			{
				sizeF2 = sizeF;
				if (IsTextVertical)
				{
					float height = sizeF2.Height;
					sizeF2.Height = sizeF2.Width;
					sizeF2.Width = height;
				}
				sizeF2 = graph.GetRelativeSize(sizeF2);
				if (chartArea.reverseSeriesOrder)
				{
					if (stringFormat.Alignment == StringAlignment.Near)
					{
						stringFormat.Alignment = StringAlignment.Far;
					}
					else
					{
						stringFormat.Alignment = StringAlignment.Near;
					}
				}
			}
			if (GetTextOrientation() == TextOrientation.Rotated90)
			{
				num = 90;
			}
			else if (GetTextOrientation() == TextOrientation.Rotated270)
			{
				num = -90;
			}
			if (AxisPosition == AxisPosition.Left)
			{
				pointF = new PointF(chartArea.PlotAreaPosition.X, chartArea.PlotAreaPosition.Y + chartArea.PlotAreaPosition.Height / 2f);
				if (stringFormat.Alignment == StringAlignment.Near)
				{
					pointF.Y = chartArea.PlotAreaPosition.Bottom() - sizeF2.Height / 2f;
				}
				else if (stringFormat.Alignment == StringAlignment.Far)
				{
					pointF.Y = chartArea.PlotAreaPosition.Y + sizeF2.Height / 2f;
				}
			}
			else if (AxisPosition == AxisPosition.Right)
			{
				pointF = new PointF(chartArea.PlotAreaPosition.Right(), chartArea.PlotAreaPosition.Y + chartArea.PlotAreaPosition.Height / 2f);
				if (stringFormat.Alignment == StringAlignment.Near)
				{
					pointF.Y = chartArea.PlotAreaPosition.Bottom() - sizeF2.Height / 2f;
				}
				else if (stringFormat.Alignment == StringAlignment.Far)
				{
					pointF.Y = chartArea.PlotAreaPosition.Y + sizeF2.Height / 2f;
				}
			}
			else if (AxisPosition == AxisPosition.Top)
			{
				pointF = new PointF(chartArea.PlotAreaPosition.X + chartArea.PlotAreaPosition.Width / 2f, chartArea.PlotAreaPosition.Y);
				if (stringFormat.Alignment == StringAlignment.Near)
				{
					pointF.X = chartArea.PlotAreaPosition.X + sizeF2.Width / 2f;
				}
				else if (stringFormat.Alignment == StringAlignment.Far)
				{
					pointF.X = chartArea.PlotAreaPosition.Right() - sizeF2.Width / 2f;
				}
			}
			else if (AxisPosition == AxisPosition.Bottom)
			{
				pointF = new PointF(chartArea.PlotAreaPosition.X + chartArea.PlotAreaPosition.Width / 2f, chartArea.PlotAreaPosition.Bottom());
				if (stringFormat.Alignment == StringAlignment.Near)
				{
					pointF.X = chartArea.PlotAreaPosition.X + sizeF2.Width / 2f;
				}
				else if (stringFormat.Alignment == StringAlignment.Far)
				{
					pointF.X = chartArea.PlotAreaPosition.Right() - sizeF2.Width / 2f;
				}
			}
			bool axisOnEdge = false;
			float marksZPosition = GetMarksZPosition(out axisOnEdge);
			Point3D[] array = null;
			float num2 = 0f;
			if (AxisPosition == AxisPosition.Top || AxisPosition == AxisPosition.Bottom)
			{
				array = new Point3D[2]
				{
					new Point3D(pointF.X, pointF.Y, marksZPosition),
					new Point3D(pointF.X - 20f, pointF.Y, marksZPosition)
				};
				chartArea.matrix3D.TransformPoints(array);
				pointF = array[0].PointF;
				array[0].PointF = graph.GetAbsolutePoint(array[0].PointF);
				array[1].PointF = graph.GetAbsolutePoint(array[1].PointF);
				num2 = (float)Math.Atan((array[1].Y - array[0].Y) / (array[1].X - array[0].X));
			}
			else
			{
				array = new Point3D[2]
				{
					new Point3D(pointF.X, pointF.Y, marksZPosition),
					new Point3D(pointF.X, pointF.Y - 20f, marksZPosition)
				};
				chartArea.matrix3D.TransformPoints(array);
				pointF = array[0].PointF;
				array[0].PointF = graph.GetAbsolutePoint(array[0].PointF);
				array[1].PointF = graph.GetAbsolutePoint(array[1].PointF);
				if (array[1].Y != array[0].Y)
				{
					num2 = 0f - (float)Math.Atan((array[1].X - array[0].X) / (array[1].Y - array[0].Y));
				}
			}
			num += (int)Math.Round(num2 * 180f / (float)Math.PI);
			float num3 = labelSize + markSize + titleSize / 2f;
			float num4 = 0f;
			float num5 = 0f;
			if (AxisPosition == AxisPosition.Left)
			{
				num4 = (float)((double)num3 * Math.Cos(num2));
				pointF.X -= num4;
			}
			else if (AxisPosition == AxisPosition.Right)
			{
				num4 = (float)((double)num3 * Math.Cos(num2));
				pointF.X += num4;
			}
			else if (AxisPosition == AxisPosition.Top)
			{
				num5 = (float)((double)num3 * Math.Cos(num2));
				num4 = (float)((double)num3 * Math.Sin(num2));
				pointF.Y -= num5;
				if (num5 > 0f)
				{
					pointF.X += num4;
				}
				else
				{
					pointF.X -= num4;
				}
			}
			else if (AxisPosition == AxisPosition.Bottom)
			{
				num5 = (float)((double)num3 * Math.Cos(num2));
				num4 = (float)((double)num3 * Math.Sin(num2));
				pointF.Y += num5;
				if (num5 > 0f)
				{
					pointF.X -= num4;
				}
				else
				{
					pointF.X += num4;
				}
			}
			stringFormat.LineAlignment = StringAlignment.Center;
			stringFormat.Alignment = StringAlignment.Center;
			if (!pointF.IsEmpty && !float.IsNaN(pointF.X) && !float.IsNaN(pointF.Y))
			{
				graph.DrawStringRel(text.Replace("\\n", "\n"), TitleFont, new SolidBrush(TitleColor), pointF, stringFormat, num, GetTextOrientation());
				if (base.Common.ProcessModeRegions)
				{
					GraphicsPath tranformedTextRectPath = graph.GetTranformedTextRectPath(pointF, sizeF, num);
					base.Common.HotRegionsList.AddHotRegion(tranformedTextRectPath, relativePath: false, graph, ChartElementType.AxisTitle, this);
				}
			}
		}

		internal void DrawAxisLine(ChartGraphics graph, bool backElements)
		{
			DrawAxisLine(graph, selectionMode: false, backElements);
		}

		internal void DrawAxisLine(ChartGraphics graph, bool selectionMode, bool backElements)
		{
			ArrowOrientation arrowOrientation = ArrowOrientation.Top;
			PointF pointF = Point.Empty;
			PointF pointF2 = Point.Empty;
			switch (AxisPosition)
			{
			case AxisPosition.Left:
				pointF.X = (float)GetAxisPosition();
				pointF.Y = base.PlotAreaPosition.Bottom();
				pointF2.X = (float)GetAxisPosition();
				pointF2.Y = base.PlotAreaPosition.Y;
				arrowOrientation = ((!reverse) ? ArrowOrientation.Top : ArrowOrientation.Bottom);
				break;
			case AxisPosition.Right:
				pointF.X = (float)GetAxisPosition();
				pointF.Y = base.PlotAreaPosition.Bottom();
				pointF2.X = (float)GetAxisPosition();
				pointF2.Y = base.PlotAreaPosition.Y;
				arrowOrientation = ((!reverse) ? ArrowOrientation.Top : ArrowOrientation.Bottom);
				break;
			case AxisPosition.Bottom:
				pointF.X = base.PlotAreaPosition.X;
				pointF.Y = (float)GetAxisPosition();
				pointF2.X = base.PlotAreaPosition.Right();
				pointF2.Y = (float)GetAxisPosition();
				arrowOrientation = ((!reverse) ? ArrowOrientation.Right : ArrowOrientation.Left);
				break;
			case AxisPosition.Top:
				pointF.X = base.PlotAreaPosition.X;
				pointF.Y = (float)GetAxisPosition();
				pointF2.X = base.PlotAreaPosition.Right();
				pointF2.Y = (float)GetAxisPosition();
				arrowOrientation = ((!reverse) ? ArrowOrientation.Right : ArrowOrientation.Left);
				break;
			}
			if (chartArea.chartAreaIsCurcular)
			{
				pointF.Y = base.PlotAreaPosition.Y + base.PlotAreaPosition.Height / 2f;
			}
			if (base.Common.ProcessModeRegions)
			{
				if (chartArea.chartAreaIsCurcular)
				{
					GraphicsPath graphicsPath = new GraphicsPath();
					PointF absolutePoint = graph.GetAbsolutePoint(pointF);
					PointF absolutePoint2 = graph.GetAbsolutePoint(pointF2);
					if (AxisPosition == AxisPosition.Bottom)
					{
						graphicsPath.AddLine(absolutePoint.X, absolutePoint.Y - 1.5f, absolutePoint2.X, absolutePoint2.Y - 1.5f);
						graphicsPath.AddLine(absolutePoint2.X, absolutePoint2.Y + 1.5f, absolutePoint.X, absolutePoint.Y + 1.5f);
						graphicsPath.CloseAllFigures();
					}
					else if (AxisPosition == AxisPosition.Top)
					{
						graphicsPath.AddLine(absolutePoint.X, absolutePoint.Y - 1.5f, absolutePoint2.X, absolutePoint2.Y - 1.5f);
						graphicsPath.AddLine(absolutePoint2.X, absolutePoint2.Y + 1.5f, absolutePoint.X, absolutePoint.Y + 1.5f);
						graphicsPath.CloseAllFigures();
					}
					else if (AxisPosition == AxisPosition.Left)
					{
						graphicsPath.AddLine(absolutePoint.X - 1.5f, absolutePoint.Y, absolutePoint2.X - 1.5f, absolutePoint2.Y);
						graphicsPath.AddLine(absolutePoint2.X + 1.5f, absolutePoint2.Y, absolutePoint.X + 1.5f, absolutePoint.Y);
						graphicsPath.CloseAllFigures();
					}
					else if (AxisPosition == AxisPosition.Right)
					{
						graphicsPath.AddLine(absolutePoint.X - 1.5f, absolutePoint.Y, absolutePoint2.X - 1.5f, absolutePoint2.Y);
						graphicsPath.AddLine(absolutePoint2.X + 1.5f, absolutePoint2.Y, absolutePoint.X + 1.5f, absolutePoint.Y);
						graphicsPath.CloseAllFigures();
					}
					graphicsPath.Transform(graph.Transform);
					base.Common.HotRegionsList.AddHotRegion(graph, graphicsPath, relativePath: false, toolTip, href, mapAreaAttributes, this, ChartElementType.Axis);
				}
				else if (!chartArea.Area3DStyle.Enable3D)
				{
					GraphicsPath graphicsPath2 = new GraphicsPath();
					PointF absolutePoint3 = graph.GetAbsolutePoint(pointF);
					PointF absolutePoint4 = graph.GetAbsolutePoint(pointF2);
					if (AxisPosition == AxisPosition.Bottom)
					{
						graphicsPath2.AddLine(absolutePoint3.X, absolutePoint3.Y - 1.5f, absolutePoint4.X, absolutePoint4.Y - 1.5f);
						graphicsPath2.AddLine(absolutePoint4.X, absolutePoint4.Y + 1.5f, absolutePoint3.X, absolutePoint3.Y + 1.5f);
						graphicsPath2.CloseAllFigures();
					}
					else if (AxisPosition == AxisPosition.Top)
					{
						graphicsPath2.AddLine(absolutePoint3.X, absolutePoint3.Y - 1.5f, absolutePoint4.X, absolutePoint4.Y - 1.5f);
						graphicsPath2.AddLine(absolutePoint4.X, absolutePoint4.Y + 1.5f, absolutePoint3.X, absolutePoint3.Y + 1.5f);
						graphicsPath2.CloseAllFigures();
					}
					else if (AxisPosition == AxisPosition.Left)
					{
						graphicsPath2.AddLine(absolutePoint3.X - 1.5f, absolutePoint3.Y, absolutePoint4.X - 1.5f, absolutePoint4.Y);
						graphicsPath2.AddLine(absolutePoint4.X + 1.5f, absolutePoint4.Y, absolutePoint3.X + 1.5f, absolutePoint3.Y);
						graphicsPath2.CloseAllFigures();
					}
					else if (AxisPosition == AxisPosition.Right)
					{
						graphicsPath2.AddLine(absolutePoint3.X - 1.5f, absolutePoint3.Y, absolutePoint4.X - 1.5f, absolutePoint4.Y);
						graphicsPath2.AddLine(absolutePoint4.X + 1.5f, absolutePoint4.Y, absolutePoint3.X + 1.5f, absolutePoint3.Y);
						graphicsPath2.CloseAllFigures();
					}
					base.Common.HotRegionsList.AddHotRegion(graph, graphicsPath2, relativePath: false, toolTip, href, mapAreaAttributes, this, ChartElementType.Axis);
				}
				else
				{
					Draw3DAxisLine(graph, pointF, pointF2, AxisPosition == AxisPosition.Top || AxisPosition == AxisPosition.Bottom, backElements, selectionMode: false);
				}
			}
			if (!base.Common.ProcessModePaint)
			{
				return;
			}
			if (!chartArea.Area3DStyle.Enable3D || chartArea.chartAreaIsCurcular)
			{
				graph.StartHotRegion(href, toolTip);
				InitAnimation(graph, pointF, pointF2);
				graph.StartAnimation();
				graph.DrawLineRel(lineColor, lineWidth, lineDashStyle, pointF, pointF2);
				graph.StopAnimation();
				graph.EndHotRegion();
				Axis axis;
				switch (arrowOrientation)
				{
				case ArrowOrientation.Left:
					axis = chartArea.AxisX;
					break;
				case ArrowOrientation.Right:
					axis = chartArea.AxisX2;
					break;
				case ArrowOrientation.Top:
					axis = chartArea.AxisY2;
					break;
				case ArrowOrientation.Bottom:
					axis = chartArea.AxisY;
					break;
				default:
					axis = chartArea.AxisX;
					break;
				}
				PointF position = (!reverse) ? pointF2 : pointF;
				InitAnimation(graph, pointF, pointF2);
				graph.StartAnimation();
				graph.DrawArrowRel(position, arrowOrientation, arrows, lineColor, lineWidth, lineDashStyle, axis.majorTickMark.Size, lineWidth);
				graph.StopAnimation();
			}
			else
			{
				Draw3DAxisLine(graph, pointF, pointF2, AxisPosition == AxisPosition.Top || AxisPosition == AxisPosition.Bottom, backElements, selectionMode: false);
			}
		}

		private void InitAnimation(ChartGraphics graph, PointF firstPoint, PointF secondPoint)
		{
		}

		internal void Draw3DAxisLine(ChartGraphics graph, PointF point1, PointF point2, bool horizontal, bool backElements, bool selectionMode)
		{
			bool flag = IsAxisOnAreaEdge();
			bool flag2 = flag;
			if ((flag2 && MajorTickMark.Style == TickMarkStyle.Cross) || MajorTickMark.Style == TickMarkStyle.Inside || MinorTickMark.Style == TickMarkStyle.Cross || MinorTickMark.Style == TickMarkStyle.Inside)
			{
				flag2 = false;
			}
			if ((horizontal && point1.X > point2.X) || (!horizontal && point1.Y > point2.Y))
			{
				PointF pointF = new PointF(point1.X, point1.Y);
				point1.X = point2.X;
				point1.Y = point2.Y;
				point2 = pointF;
			}
			float z = chartArea.IsMainSceneWallOnFront() ? chartArea.areaSceneDepth : 0f;
			SurfaceNames surfaceNames = chartArea.IsMainSceneWallOnFront() ? SurfaceNames.Front : SurfaceNames.Back;
			if (chartArea.ShouldDrawOnSurface(SurfaceNames.Back, backElements, flag2))
			{
				graph.StartHotRegion(href, toolTip);
				graph.StartAnimation();
				graph.Draw3DLine(chartArea.matrix3D, lineColor, lineWidth, lineDashStyle, new Point3D(point1.X, point1.Y, z), new Point3D(point2.X, point2.Y, z), base.Common, this, ChartElementType.Axis);
				graph.StopAnimation();
				graph.EndHotRegion();
			}
			z = (chartArea.IsMainSceneWallOnFront() ? 0f : chartArea.areaSceneDepth);
			surfaceNames = ((!chartArea.IsMainSceneWallOnFront()) ? SurfaceNames.Front : SurfaceNames.Back);
			if (chartArea.ShouldDrawOnSurface(surfaceNames, backElements, flag2) && (!flag || (AxisPosition == AxisPosition.Bottom && chartArea.IsBottomSceneWallVisible()) || (AxisPosition == AxisPosition.Left && chartArea.IsSideSceneWallOnLeft()) || (AxisPosition == AxisPosition.Right && !chartArea.IsSideSceneWallOnLeft())))
			{
				graph.StartHotRegion(href, toolTip);
				graph.StartAnimation();
				graph.Draw3DLine(chartArea.matrix3D, lineColor, lineWidth, lineDashStyle, new Point3D(point1.X, point1.Y, z), new Point3D(point2.X, point2.Y, z), base.Common, this, ChartElementType.Axis);
				graph.StopAnimation();
				graph.EndHotRegion();
			}
			SurfaceNames surfaceName = (AxisPosition == AxisPosition.Left || AxisPosition == AxisPosition.Right) ? SurfaceNames.Top : SurfaceNames.Left;
			if (chartArea.ShouldDrawOnSurface(surfaceName, backElements, flag2) && (!flag || (AxisPosition == AxisPosition.Bottom && (chartArea.IsBottomSceneWallVisible() || chartArea.IsSideSceneWallOnLeft())) || (AxisPosition == AxisPosition.Left && chartArea.IsSideSceneWallOnLeft()) || (AxisPosition == AxisPosition.Right && !chartArea.IsSideSceneWallOnLeft()) || (AxisPosition == AxisPosition.Top && chartArea.IsSideSceneWallOnLeft())))
			{
				graph.StartHotRegion(href, toolTip);
				graph.StartAnimation();
				graph.Draw3DLine(chartArea.matrix3D, lineColor, lineWidth, lineDashStyle, new Point3D(point1.X, point1.Y, chartArea.areaSceneDepth), new Point3D(point1.X, point1.Y, 0f), base.Common, this, ChartElementType.Axis);
				graph.StopAnimation();
				graph.EndHotRegion();
			}
			surfaceName = ((AxisPosition == AxisPosition.Left || AxisPosition == AxisPosition.Right) ? SurfaceNames.Bottom : SurfaceNames.Right);
			if (chartArea.ShouldDrawOnSurface(surfaceName, backElements, flag2) && (!flag || (AxisPosition == AxisPosition.Bottom && (chartArea.IsBottomSceneWallVisible() || !chartArea.IsSideSceneWallOnLeft())) || (AxisPosition == AxisPosition.Left && (chartArea.IsSideSceneWallOnLeft() || chartArea.IsBottomSceneWallVisible())) || (AxisPosition == AxisPosition.Right && (!chartArea.IsSideSceneWallOnLeft() || chartArea.IsBottomSceneWallVisible())) || (AxisPosition == AxisPosition.Top && !chartArea.IsSideSceneWallOnLeft())))
			{
				graph.StartHotRegion(href, toolTip);
				graph.StartAnimation();
				graph.Draw3DLine(chartArea.matrix3D, lineColor, lineWidth, lineDashStyle, new Point3D(point2.X, point2.Y, chartArea.areaSceneDepth), new Point3D(point2.X, point2.Y, 0f), base.Common, this, ChartElementType.Axis);
				graph.StopAnimation();
				graph.EndHotRegion();
			}
		}

		internal float GetMarksZPosition(out bool axisOnEdge)
		{
			axisOnEdge = IsAxisOnAreaEdge();
			if (!IsMarksNextToAxis())
			{
				axisOnEdge = true;
			}
			float num = 0f;
			if (AxisPosition == AxisPosition.Bottom && (chartArea.IsBottomSceneWallVisible() || !axisOnEdge))
			{
				num = chartArea.areaSceneDepth;
			}
			if (AxisPosition == AxisPosition.Left && (chartArea.IsSideSceneWallOnLeft() || !axisOnEdge))
			{
				num = chartArea.areaSceneDepth;
			}
			if (AxisPosition == AxisPosition.Right && (!chartArea.IsSideSceneWallOnLeft() || !axisOnEdge))
			{
				num = chartArea.areaSceneDepth;
			}
			if (AxisPosition == AxisPosition.Top && !axisOnEdge)
			{
				num = chartArea.areaSceneDepth;
			}
			if (chartArea.IsMainSceneWallOnFront())
			{
				num = ((num == 0f) ? chartArea.areaSceneDepth : 0f);
			}
			return num;
		}

		internal void PaintGrids(ChartGraphics graph)
		{
			PaintGrids(graph, selectionMode: false, 0, 0, out object _);
		}

		internal void PaintGrids(ChartGraphics graph, bool selectionMode, int x, int y, out object obj)
		{
			obj = null;
			if (enabled)
			{
				minorGrid.Paint(graph);
				majorGrid.Paint(graph);
			}
		}

		internal void PaintStrips(ChartGraphics graph, bool drawLinesOnly)
		{
			PaintStrips(graph, selectionMode: false, 0, 0, out object _, drawLinesOnly);
		}

		internal void PaintStrips(ChartGraphics graph, bool selectionMode, int x, int y, out object obj, bool drawLinesOnly)
		{
			obj = null;
			if (!enabled)
			{
				return;
			}
			bool flag = AddInterlacedStrip();
			foreach (StripLine stripLine in StripLines)
			{
				stripLine.Paint(graph, base.Common, drawLinesOnly);
			}
			if (flag)
			{
				StripLines.RemoveAt(0);
			}
		}

		private bool AddInterlacedStrip()
		{
			bool flag = false;
			if (Interlaced)
			{
				StripLine stripLine = new StripLine();
				stripLine.interlaced = true;
				stripLine.BorderColor = Color.Empty;
				if (MajorGrid.Enabled && MajorGrid.Interval != 0.0)
				{
					flag = true;
					stripLine.Interval = MajorGrid.Interval * 2.0;
					stripLine.IntervalType = MajorGrid.IntervalType;
					stripLine.IntervalOffset = MajorGrid.IntervalOffset;
					stripLine.IntervalOffsetType = MajorGrid.IntervalOffsetType;
					stripLine.StripWidth = MajorGrid.Interval;
					stripLine.StripWidthType = MajorGrid.IntervalType;
				}
				else if (MajorTickMark.Enabled && MajorTickMark.Interval != 0.0)
				{
					flag = true;
					stripLine.Interval = MajorTickMark.Interval * 2.0;
					stripLine.IntervalType = MajorTickMark.IntervalType;
					stripLine.IntervalOffset = MajorTickMark.IntervalOffset;
					stripLine.IntervalOffsetType = MajorTickMark.IntervalOffsetType;
					stripLine.StripWidth = MajorTickMark.Interval;
					stripLine.StripWidthType = MajorTickMark.IntervalType;
				}
				else if (base.LabelStyle.Enabled && base.LabelStyle.Interval != 0.0)
				{
					flag = true;
					stripLine.Interval = base.LabelStyle.Interval * 2.0;
					stripLine.IntervalType = base.LabelStyle.IntervalType;
					stripLine.IntervalOffset = base.LabelStyle.IntervalOffset;
					stripLine.IntervalOffsetType = base.LabelStyle.IntervalOffsetType;
					stripLine.StripWidth = base.LabelStyle.Interval;
					stripLine.StripWidthType = base.LabelStyle.IntervalType;
				}
				if (flag)
				{
					if (InterlacedColor != Color.Empty)
					{
						stripLine.BackColor = InterlacedColor;
					}
					else if (chartArea.BackColor == Color.Empty)
					{
						stripLine.BackColor = (chartArea.Area3DStyle.Enable3D ? Color.DarkGray : Color.LightGray);
					}
					else if (chartArea.BackColor == Color.Transparent)
					{
						if (chart.BackColor != Color.Transparent && chart.BackColor != Color.Black)
						{
							stripLine.BackColor = ChartGraphics.GetGradientColor(chart.BackColor, Color.Black, 0.2);
						}
						else
						{
							stripLine.BackColor = Color.LightGray;
						}
					}
					else
					{
						stripLine.BackColor = ChartGraphics.GetGradientColor(chartArea.BackColor, Color.Black, 0.2);
					}
					StripLines.Insert(0, stripLine);
				}
			}
			return flag;
		}

		public void RoundAxisValues()
		{
			roundedXValues = true;
		}

		internal void ReCalc(ElementPosition position)
		{
			base.PlotAreaPosition = position;
		}

		internal void StoreAxisValues()
		{
			tempLabels = new CustomLabelsCollection(this);
			foreach (CustomLabel customLabel in base.CustomLabels)
			{
				tempLabels.Add(customLabel.Clone());
			}
			paintMode = true;
			if (storeValuesEnabled)
			{
				tempMaximum = maximum;
				tempMinimum = minimum;
				tempCrossing = crossing;
				tempAutoMinimum = autoMinimum;
				tempAutoMaximum = autoMaximum;
				tempMajorGridInterval = majorGrid.interval;
				tempMajorTickMarkInterval = majorTickMark.interval;
				tempMinorGridInterval = minorGrid.interval;
				tempMinorTickMarkInterval = minorTickMark.interval;
				tempGridIntervalType = majorGrid.intervalType;
				tempTickMarkIntervalType = majorTickMark.intervalType;
				tempLabelInterval = labelStyle.interval;
				tempLabelIntervalType = labelStyle.intervalType;
				originalViewPosition = base.View.Position;
				storeValuesEnabled = false;
			}
		}

		internal void ResetAxisValues()
		{
			paintMode = false;
			ResetAutoValues();
			if (tempLabels == null)
			{
				return;
			}
			base.CustomLabels.Clear();
			foreach (CustomLabel tempLabel in tempLabels)
			{
				base.CustomLabels.Add(tempLabel.Clone());
			}
			tempLabels = null;
		}

		internal void ResetAutoValues()
		{
			refreshMinMaxFromData = true;
			maximum = tempMaximum;
			minimum = tempMinimum;
			crossing = tempCrossing;
			autoMinimum = tempAutoMinimum;
			autoMaximum = tempAutoMaximum;
			majorGrid.interval = tempMajorGridInterval;
			majorTickMark.interval = tempMajorTickMarkInterval;
			minorGrid.interval = tempMinorGridInterval;
			minorTickMark.interval = tempMinorTickMarkInterval;
			labelStyle.interval = tempLabelInterval;
			majorGrid.intervalType = tempGridIntervalType;
			majorTickMark.intervalType = tempTickMarkIntervalType;
			labelStyle.intervalType = tempLabelIntervalType;
			if (chart != null && !chart.serializing)
			{
				base.View.Position = originalViewPosition;
			}
			storeValuesEnabled = true;
		}

		internal virtual void Resize(ChartGraphics chartGraph, ElementPosition chartAreaPosition, RectangleF plotArea, float axesNumber, bool autoPlotPosition)
		{
			base.PlotAreaPosition = chartAreaPosition;
			base.PlotAreaPosition.FromRectangleF(plotArea);
			titleSize = 0f;
			if (Title.Length > 0)
			{
				SizeF sizeF = chartGraph.MeasureStringRel(Title.Replace("\\n", "\n"), TitleFont, new SizeF(10000f, 10000f), new StringFormat(), GetTextOrientation());
				float num = 0f;
				if (AxisPosition == AxisPosition.Bottom || AxisPosition == AxisPosition.Top)
				{
					num = plotArea.Height / 100f * (20f / axesNumber);
					if (IsTextVertical)
					{
						titleSize = Math.Min(sizeF.Width, num);
					}
					else
					{
						titleSize = Math.Min(sizeF.Height, num);
					}
				}
				else
				{
					num = plotArea.Width / 100f * (20f / axesNumber);
					if (IsTextVertical)
					{
						sizeF.Width = chartGraph.GetAbsoluteSize(new SizeF(sizeF.Height, sizeF.Height)).Height;
						sizeF.Width = chartGraph.GetRelativeSize(new SizeF(sizeF.Width, sizeF.Width)).Width;
						titleSize = Math.Min(sizeF.Width, plotArea.Width / 100f * (20f / axesNumber));
					}
					else
					{
						titleSize = Math.Min(sizeF.Width, plotArea.Width / 100f * (20f / axesNumber));
					}
				}
			}
			if (titleSize > 0f)
			{
				titleSize += 1f;
			}
			float num2 = 0f;
			SizeF sizeF2 = SizeF.Empty;
			SizeF sizeF3 = SizeF.Empty;
			ArrowOrientation arrowOrientation = ArrowOrientation.Bottom;
			if (axisType == AxisName.X || axisType == AxisName.X2)
			{
				if (chartArea.AxisY.Arrows != 0)
				{
					sizeF2 = chartArea.AxisY.GetArrowSize(out arrowOrientation);
					if (!IsArrowInAxis(arrowOrientation, AxisPosition))
					{
						sizeF2 = SizeF.Empty;
					}
				}
				if (chartArea.AxisY2.Arrows != 0)
				{
					sizeF3 = chartArea.AxisY2.GetArrowSize(out arrowOrientation);
					if (!IsArrowInAxis(arrowOrientation, AxisPosition))
					{
						sizeF3 = SizeF.Empty;
					}
				}
			}
			else
			{
				if (chartArea.AxisX.Arrows != 0)
				{
					sizeF2 = chartArea.AxisX.GetArrowSize(out arrowOrientation);
					if (!IsArrowInAxis(arrowOrientation, AxisPosition))
					{
						sizeF2 = SizeF.Empty;
					}
				}
				if (chartArea.AxisX2.Arrows != 0)
				{
					sizeF3 = chartArea.AxisX2.GetArrowSize(out arrowOrientation);
					if (!IsArrowInAxis(arrowOrientation, AxisPosition))
					{
						sizeF3 = SizeF.Empty;
					}
				}
			}
			num2 = ((AxisPosition != AxisPosition.Bottom && AxisPosition != AxisPosition.Top) ? Math.Max(sizeF2.Width, sizeF3.Width) : Math.Max(sizeF2.Height, sizeF3.Height));
			markSize = 0f;
			float val = 0f;
			if (MajorTickMark.Enabled && MajorTickMark.Style != 0)
			{
				if (MajorTickMark.Style == TickMarkStyle.Inside)
				{
					val = 0f;
				}
				else if (MajorTickMark.Style == TickMarkStyle.Cross)
				{
					val = MajorTickMark.Size / 2f;
				}
				else if (MajorTickMark.Style == TickMarkStyle.Outside)
				{
					val = MajorTickMark.Size;
				}
			}
			float val2 = 0f;
			if (MinorTickMark.Enabled && MinorTickMark.Style != 0 && MinorTickMark.Interval != 0.0)
			{
				if (MinorTickMark.Style == TickMarkStyle.Inside)
				{
					val2 = 0f;
				}
				else if (MinorTickMark.Style == TickMarkStyle.Cross)
				{
					val2 = MinorTickMark.Size / 2f;
				}
				else if (MinorTickMark.Style == TickMarkStyle.Outside)
				{
					val2 = MinorTickMark.Size;
				}
			}
			markSize += Math.Max(val, val2);
			SizeF relativeSize = chartGraph.GetRelativeSize(new SizeF(LineWidth, LineWidth));
			if (AxisPosition == AxisPosition.Bottom || AxisPosition == AxisPosition.Top)
			{
				markSize += relativeSize.Height / 2f;
				markSize = Math.Min(markSize, plotArea.Height / 100f * (20f / axesNumber));
			}
			else
			{
				markSize += relativeSize.Width / 2f;
				markSize = Math.Min(markSize, plotArea.Width / 100f * (20f / axesNumber));
			}
			scrollBarSize = 0f;
			if (base.ScrollBar.IsVisible() && (IsAxisOnAreaEdge() || !MarksNextToAxis))
			{
				if (base.ScrollBar.PositionInside)
				{
					markSize += (float)base.ScrollBar.GetScrollBarRelativeSize();
				}
				else
				{
					scrollBarSize = (float)base.ScrollBar.GetScrollBarRelativeSize();
				}
			}
			if (chartArea.Area3DStyle.Enable3D && !chartArea.chartAreaIsCurcular && chartArea.BackColor != Color.Transparent && chartArea.Area3DStyle.WallWidth > 0)
			{
				SizeF relativeSize2 = chartGraph.GetRelativeSize(new SizeF(chartArea.Area3DStyle.WallWidth, chartArea.Area3DStyle.WallWidth));
				if (AxisPosition == AxisPosition.Bottom || AxisPosition == AxisPosition.Top)
				{
					markSize += relativeSize2.Height;
				}
				else
				{
					markSize += relativeSize2.Width;
				}
			}
			if (num2 > markSize + scrollBarSize + titleSize)
			{
				markSize = Math.Max(markSize, num2 - (markSize + scrollBarSize + titleSize));
				markSize = Math.Min(markSize, plotArea.Width / 100f * (20f / axesNumber));
			}
			float num3 = 0f;
			if (autoPlotPosition)
			{
				num3 = ((AxisPosition != AxisPosition.Bottom && AxisPosition != AxisPosition.Top) ? plotArea.Width : plotArea.Height);
			}
			else
			{
				if (IsMarksNextToAxis())
				{
					if (AxisPosition == AxisPosition.Top)
					{
						num3 = (float)GetAxisPosition() - chartArea.Position.Y;
					}
					else if (AxisPosition == AxisPosition.Bottom)
					{
						num3 = chartArea.Position.Bottom() - (float)GetAxisPosition();
					}
					if (AxisPosition == AxisPosition.Left)
					{
						num3 = (float)GetAxisPosition() - chartArea.Position.X;
					}
					else if (AxisPosition == AxisPosition.Right)
					{
						num3 = chartArea.Position.Right() - (float)GetAxisPosition();
					}
				}
				else
				{
					if (AxisPosition == AxisPosition.Top)
					{
						num3 = plotArea.Y - chartArea.Position.Y;
					}
					else if (AxisPosition == AxisPosition.Bottom)
					{
						num3 = chartArea.Position.Bottom() - plotArea.Bottom;
					}
					if (AxisPosition == AxisPosition.Left)
					{
						num3 = plotArea.X - chartArea.Position.X;
					}
					else if (AxisPosition == AxisPosition.Right)
					{
						num3 = chartArea.Position.Right() - plotArea.Right;
					}
				}
				num3 *= 2f;
			}
			if (base.Enabled != AxisEnabled.False && base.LabelStyle.Enabled && IsVariableLabelCountModeEnabled())
			{
				float num4 = 3f;
				if ((AxisPosition == AxisPosition.Left || AxisPosition == AxisPosition.Right) && (base.LabelStyle.FontAngle == 90 || base.LabelStyle.FontAngle == -90))
				{
					num4 = 0f;
				}
				if ((AxisPosition == AxisPosition.Top || AxisPosition == AxisPosition.Bottom) && (base.LabelStyle.FontAngle == 180 || base.LabelStyle.FontAngle == 0))
				{
					num4 = 0f;
				}
				if (chartArea.Area3DStyle.Enable3D)
				{
					num4 += 1f;
				}
				autoLabelFont = new Font(base.LabelStyle.Font.FontFamily, base.LabelStyle.Font.Size + num4, base.LabelStyle.Font.Style, GraphicsUnit.Point);
				autoLabelAngle = base.LabelStyle.FontAngle;
				autoLabelOffset = (base.LabelStyle.OffsetLabels ? 1 : 0);
				AdjustIntervalToFitLabels(chartGraph, autoPlotPosition, onlyIncreaseInterval: false);
			}
			autoLabelFont = null;
			autoLabelAngle = -1000;
			autoLabelOffset = -1;
			if (LabelsAutoFit && LabelsAutoFitStyle != 0 && !chartArea.chartAreaIsCurcular)
			{
				bool flag = false;
				bool flag2 = false;
				autoLabelAngle = 0;
				autoLabelOffset = 0;
				CustomLabelsCollection customLabelsCollection = null;
				float num5 = 8f;
				num5 = Math.Max(LabelsAutoFitMaxFontSize, LabelsAutoFitMinFontSize);
				minLabelFontSize = Math.Min(LabelsAutoFitMinFontSize, LabelsAutoFitMaxFontSize);
				aveLabelFontSize = minLabelFontSize + Math.Abs(num5 - minLabelFontSize) / 2f;
				if (chartArea.EquallySizedAxesFont)
				{
					num5 = Math.Min(num5, chartArea.axesAutoFontSize);
				}
				autoLabelFont = new Font(base.LabelStyle.Font.FontFamily, num5, base.LabelStyle.Font.Style, GraphicsUnit.Point);
				if ((LabelsAutoFitStyle & LabelsAutoFitStyles.IncreaseFont) != LabelsAutoFitStyles.IncreaseFont)
				{
					autoLabelFont = (Font)base.LabelStyle.Font.Clone();
				}
				float num6 = 0f;
				while (!flag)
				{
					bool checkLabelsFirstRowOnly = true;
					if ((LabelsAutoFitStyle & LabelsAutoFitStyles.DecreaseFont) == LabelsAutoFitStyles.DecreaseFont)
					{
						checkLabelsFirstRowOnly = false;
					}
					flag = CheckLabelsFit(chartGraph, markSize + scrollBarSize + titleSize + num6, autoPlotPosition, checkLabelsFirstRowOnly, secondPass: false);
					if (!flag)
					{
						if (autoLabelFont.SizeInPoints >= aveLabelFontSize && (LabelsAutoFitStyle & LabelsAutoFitStyles.DecreaseFont) == LabelsAutoFitStyles.DecreaseFont)
						{
							autoLabelFont = new Font(autoLabelFont.FontFamily, autoLabelFont.SizeInPoints - 0.5f, autoLabelFont.Style, GraphicsUnit.Point);
							continue;
						}
						if (!chartArea.Area3DStyle.Enable3D && !chartArea.chartAreaIsCurcular && customLabelsCollection == null && autoLabelAngle == 0 && autoLabelOffset == 0 && (LabelsAutoFitStyle & LabelsAutoFitStyles.OffsetLabels) == LabelsAutoFitStyles.OffsetLabels)
						{
							autoLabelOffset = 1;
							continue;
						}
						if (!flag2 && (LabelsAutoFitStyle & LabelsAutoFitStyles.WordWrap) == LabelsAutoFitStyles.WordWrap)
						{
							autoLabelOffset = 0;
							if (customLabelsCollection == null)
							{
								customLabelsCollection = new CustomLabelsCollection(this);
								foreach (CustomLabel customLabel3 in base.CustomLabels)
								{
									customLabelsCollection.Add(customLabel3.Clone());
								}
							}
							if (WordWrapLongestLabel(base.CustomLabels))
							{
								continue;
							}
							flag2 = true;
							if (customLabelsCollection != null)
							{
								base.CustomLabels.Clear();
								foreach (CustomLabel item in customLabelsCollection)
								{
									base.CustomLabels.Add(item.Clone());
								}
								customLabelsCollection = null;
							}
							if (AxisPosition != AxisPosition.Bottom && AxisPosition != AxisPosition.Top)
							{
								continue;
							}
							if ((num6 == 0f || num6 == 30f || num6 == 20f) && ((LabelsAutoFitStyle & LabelsAutoFitStyles.LabelsAngleStep30) == LabelsAutoFitStyles.LabelsAngleStep30 || (LabelsAutoFitStyle & LabelsAutoFitStyles.LabelsAngleStep45) == LabelsAutoFitStyles.LabelsAngleStep45 || (LabelsAutoFitStyle & LabelsAutoFitStyles.LabelsAngleStep90) == LabelsAutoFitStyles.LabelsAngleStep90))
							{
								autoLabelAngle = 90;
								flag2 = false;
								if (num6 == 0f)
								{
									num6 = 30f;
									continue;
								}
								if (num6 == 30f)
								{
									num6 = 20f;
									continue;
								}
								if (num6 == 20f)
								{
									num6 = 5f;
									continue;
								}
								autoLabelAngle = 0;
								flag2 = true;
							}
							else
							{
								num6 = 0f;
							}
							continue;
						}
						if (autoLabelAngle != 90 && ((LabelsAutoFitStyle & LabelsAutoFitStyles.LabelsAngleStep30) == LabelsAutoFitStyles.LabelsAngleStep30 || (LabelsAutoFitStyle & LabelsAutoFitStyles.LabelsAngleStep45) == LabelsAutoFitStyles.LabelsAngleStep45 || (LabelsAutoFitStyle & LabelsAutoFitStyles.LabelsAngleStep90) == LabelsAutoFitStyles.LabelsAngleStep90))
						{
							num6 = 0f;
							autoLabelOffset = 0;
							if ((LabelsAutoFitStyle & LabelsAutoFitStyles.LabelsAngleStep30) == LabelsAutoFitStyles.LabelsAngleStep30)
							{
								autoLabelAngle += (chartArea.Area3DStyle.Enable3D ? 45 : 30);
							}
							else if ((LabelsAutoFitStyle & LabelsAutoFitStyles.LabelsAngleStep45) == LabelsAutoFitStyles.LabelsAngleStep45)
							{
								autoLabelAngle += 45;
							}
							else if ((LabelsAutoFitStyle & LabelsAutoFitStyles.LabelsAngleStep90) == LabelsAutoFitStyles.LabelsAngleStep90)
							{
								autoLabelAngle += 90;
							}
							continue;
						}
						if (autoLabelFont.SizeInPoints > minLabelFontSize && (LabelsAutoFitStyle & LabelsAutoFitStyles.DecreaseFont) == LabelsAutoFitStyles.DecreaseFont)
						{
							autoLabelAngle = 0;
							autoLabelFont = new Font(autoLabelFont.FontFamily, autoLabelFont.SizeInPoints - 0.5f, autoLabelFont.Style, GraphicsUnit.Point);
							continue;
						}
						if ((LabelsAutoFitStyle & LabelsAutoFitStyles.LabelsAngleStep30) == LabelsAutoFitStyles.LabelsAngleStep30 || (LabelsAutoFitStyle & LabelsAutoFitStyles.LabelsAngleStep45) == LabelsAutoFitStyles.LabelsAngleStep45 || (LabelsAutoFitStyle & LabelsAutoFitStyles.LabelsAngleStep90) == LabelsAutoFitStyles.LabelsAngleStep90)
						{
							if (AxisPosition == AxisPosition.Top || AxisPosition == AxisPosition.Bottom)
							{
								autoLabelAngle = 90;
							}
							else
							{
								autoLabelAngle = 0;
							}
						}
						if ((LabelsAutoFitStyle & LabelsAutoFitStyles.OffsetLabels) == LabelsAutoFitStyles.OffsetLabels)
						{
							autoLabelOffset = 0;
						}
						flag = true;
					}
					else if (chartArea.Area3DStyle.Enable3D && !chartArea.chartAreaIsCurcular && autoLabelFont.SizeInPoints > minLabelFontSize)
					{
						autoLabelFont = new Font(autoLabelFont.FontFamily, autoLabelFont.SizeInPoints - 0.5f, autoLabelFont.Style, GraphicsUnit.Point);
					}
				}
				if ((AxisPosition == AxisPosition.Bottom || AxisPosition == AxisPosition.Top) && autoLabelAngle == 90)
				{
					autoLabelAngle = -90;
				}
			}
			labelSize = 0f;
			if (!base.LabelStyle.Enabled)
			{
				return;
			}
			labelSize = 75f - markSize - scrollBarSize - titleSize;
			if (labelSize > 0f)
			{
				groupingLabelSizes = GetRequiredGroupLabelSize(chartGraph, num3 / 100f * 45f);
				totlaGroupingLabelsSize = GetGroupLablesToatalSize();
			}
			labelSize -= totlaGroupingLabelsSize;
			if (labelSize > 0f)
			{
				if (AxisPosition == AxisPosition.Bottom || AxisPosition == AxisPosition.Top)
				{
					labelSize = 1f + GetRequiredLabelSize(chartGraph, num3 / 100f * (75f - markSize - scrollBarSize - titleSize), out unRotatedLabelSize);
				}
				else
				{
					labelSize = 1f + GetRequiredLabelSize(chartGraph, num3 / 100f * (75f - markSize - scrollBarSize - titleSize), out unRotatedLabelSize);
				}
				if (!base.LabelStyle.Enabled)
				{
					labelSize -= 1f;
				}
			}
			else
			{
				labelSize = 0f;
			}
			labelSize += totlaGroupingLabelsSize;
		}

		private void AdjustIntervalToFitLabels(ChartGraphics chartGraph, bool autoPlotPosition, bool onlyIncreaseInterval)
		{
			if (base.ScaleSegments.Count == 0)
			{
				AdjustIntervalToFitLabels(chartGraph, autoPlotPosition, null, onlyIncreaseInterval);
				return;
			}
			base.ScaleSegments.AllowOutOfScaleValues = true;
			foreach (AxisScaleSegment scaleSegment in base.ScaleSegments)
			{
				AdjustIntervalToFitLabels(chartGraph, autoPlotPosition, scaleSegment, onlyIncreaseInterval);
			}
			bool removeFirstRow = true;
			int num = 0;
			ArrayList arrayList = new ArrayList();
			ArrayList arrayList2 = new ArrayList();
			foreach (AxisScaleSegment scaleSegment2 in base.ScaleSegments)
			{
				scaleSegment2.SetTempAxisScaleAndInterval();
				FillLabels(removeFirstRow);
				removeFirstRow = false;
				scaleSegment2.RestoreAxisScaleAndInterval();
				if (num < base.ScaleSegments.Count - 1 && base.CustomLabels.Count > 0)
				{
					arrayList.Add(base.CustomLabels[base.CustomLabels.Count - 1]);
					arrayList2.Add(base.CustomLabels.Count - 1);
					base.CustomLabels.RemoveAt(base.CustomLabels.Count - 1);
				}
				num++;
			}
			int num2 = 0;
			int num3 = 0;
			foreach (CustomLabel item in arrayList)
			{
				int index = (int)arrayList2[num3] + num2;
				if (num3 < base.CustomLabels.Count)
				{
					base.CustomLabels.Insert(index, item);
				}
				else
				{
					base.CustomLabels.Add(item);
				}
				ArrayList arrayList3 = new ArrayList();
				bool flag = CheckLabelsFit(chartGraph, markSize + scrollBarSize + titleSize, autoPlotPosition, checkLabelsFirstRowOnly: true, secondPass: false, (AxisPosition != 0 && AxisPosition != AxisPosition.Right) ? true : false, (AxisPosition == AxisPosition.Left || AxisPosition == AxisPosition.Right) ? true : false, arrayList3);
				if (flag)
				{
					int num4 = 0;
					while (flag && num4 < arrayList3.Count)
					{
						RectangleF rectangleF = (RectangleF)arrayList3[num4];
						int num5 = num4 + 1;
						while (flag && num5 < arrayList3.Count)
						{
							RectangleF rect = (RectangleF)arrayList3[num5];
							if (rectangleF.IntersectsWith(rect))
							{
								flag = false;
							}
							num5++;
						}
						num4++;
					}
				}
				if (!flag)
				{
					base.CustomLabels.RemoveAt(index);
				}
				else
				{
					num2++;
				}
				num3++;
			}
			base.ScaleSegments.AllowOutOfScaleValues = false;
		}

		private bool IsVariableLabelCountModeEnabled()
		{
			if ((base.IntervalAutoMode == IntervalAutoMode.VariableCount || base.ScaleSegments.Count > 0) && !base.Logarithmic && (tempLabelInterval <= 0.0 || (double.IsNaN(tempLabelInterval) && Interval <= 0.0)))
			{
				if (!chartArea.requireAxes)
				{
					return false;
				}
				bool flag = false;
				foreach (CustomLabel customLabel in base.CustomLabels)
				{
					if (customLabel.customLabel && customLabel.RowIndex == 0)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return true;
				}
			}
			return false;
		}

		private void AdjustIntervalToFitLabels(ChartGraphics chartGraph, bool autoPlotPosition, AxisScaleSegment axisScaleSegment, bool onlyIncreaseInterval)
		{
			if (axisScaleSegment != null)
			{
				if (tempLabels != null)
				{
					base.CustomLabels.Clear();
					foreach (CustomLabel tempLabel in tempLabels)
					{
						base.CustomLabels.Add(tempLabel.Clone());
					}
				}
				axisScaleSegment.SetTempAxisScaleAndInterval();
				FillLabels(removeFirstRow: true);
				axisScaleSegment.RestoreAxisScaleAndInterval();
			}
			double minInterval = double.NaN;
			foreach (Series item in AxisScaleBreakStyle.GetAxisSeries(this))
			{
				if (axisType == AxisName.X || axisType == AxisName.X2)
				{
					if (ChartElement.IndexedSeries(item))
					{
						minInterval = 1.0;
					}
					else if (item.XValueType == ChartValueTypes.String || item.XValueType == ChartValueTypes.Int || item.XValueType == ChartValueTypes.UInt || item.XValueType == ChartValueTypes.ULong || item.XValueType == ChartValueTypes.Long)
					{
						minInterval = 1.0;
					}
				}
				else if (item.YValueType == ChartValueTypes.String || item.YValueType == ChartValueTypes.Int || item.YValueType == ChartValueTypes.UInt || item.YValueType == ChartValueTypes.ULong || item.YValueType == ChartValueTypes.Long)
				{
					minInterval = 1.0;
				}
			}
			bool flag = true;
			bool flag2 = true;
			double num = axisScaleSegment?.Interval ?? labelStyle.Interval;
			DateTimeIntervalType dateTimeIntervalType = axisScaleSegment?.IntervalType ?? labelStyle.IntervalType;
			DateTimeIntervalType dateTimeIntervalType2 = dateTimeIntervalType;
			double num2 = num;
			ArrayList arrayList = new ArrayList();
			bool flag3 = false;
			int num3 = 0;
			while (!flag3 && num3 <= 1000)
			{
				bool flag4 = true;
				bool flag5 = CheckLabelsFit(chartGraph, markSize + scrollBarSize + titleSize, autoPlotPosition, checkLabelsFirstRowOnly: true, secondPass: false, (AxisPosition != 0 && AxisPosition != AxisPosition.Right) ? true : false, (AxisPosition == AxisPosition.Left || AxisPosition == AxisPosition.Right) ? true : false, null);
				if (flag)
				{
					flag = false;
					flag2 = (flag5 ? true : false);
					if (onlyIncreaseInterval && flag2)
					{
						flag3 = true;
						continue;
					}
				}
				double num4 = 0.0;
				DateTimeIntervalType dateTimeIntervalType3 = DateTimeIntervalType.Number;
				if (flag2)
				{
					if (flag5)
					{
						num2 = num;
						dateTimeIntervalType2 = dateTimeIntervalType;
						arrayList.Clear();
						foreach (CustomLabel customLabel3 in base.CustomLabels)
						{
							arrayList.Add(customLabel3);
						}
						dateTimeIntervalType3 = dateTimeIntervalType;
						num4 = ReduceLabelInterval(num, minInterval, axisScaleSegment, ref dateTimeIntervalType3);
					}
					else
					{
						num4 = num2;
						dateTimeIntervalType3 = dateTimeIntervalType2;
						flag3 = true;
						flag4 = false;
						base.CustomLabels.Clear();
						foreach (CustomLabel item2 in arrayList)
						{
							base.CustomLabels.Add(item2);
						}
					}
				}
				else if (!flag5 && base.CustomLabels.Count > 1)
				{
					dateTimeIntervalType3 = dateTimeIntervalType;
					num4 = IncreaseLabelInterval(num, axisScaleSegment, ref dateTimeIntervalType3);
				}
				else
				{
					flag3 = true;
				}
				if (num4 != 0.0)
				{
					num = num4;
					dateTimeIntervalType = dateTimeIntervalType3;
					if (axisScaleSegment == null)
					{
						SetIntervalAndType(num4, dateTimeIntervalType3);
					}
					else
					{
						axisScaleSegment.Interval = num4;
						axisScaleSegment.IntervalType = dateTimeIntervalType3;
					}
					if (flag4)
					{
						if (tempLabels != null)
						{
							base.CustomLabels.Clear();
							foreach (CustomLabel tempLabel2 in tempLabels)
							{
								base.CustomLabels.Add(tempLabel2.Clone());
							}
						}
						if (axisScaleSegment == null)
						{
							FillLabels(removeFirstRow: true);
						}
						else
						{
							axisScaleSegment.SetTempAxisScaleAndInterval();
							FillLabels(removeFirstRow: true);
							axisScaleSegment.RestoreAxisScaleAndInterval();
						}
					}
				}
				else
				{
					flag3 = true;
				}
				num3++;
			}
		}

		private double ReduceLabelInterval(double oldInterval, double minInterval, AxisScaleSegment axisScaleSegment, ref DateTimeIntervalType intervalType)
		{
			double num = oldInterval;
			double num2 = maximum - minimum;
			int num3 = 0;
			if (intervalType == DateTimeIntervalType.Auto || intervalType == DateTimeIntervalType.NotSet || intervalType == DateTimeIntervalType.Number)
			{
				double num4 = 2.0;
				do
				{
					num = CalcInterval(num2 / (num2 / (num / num4)));
					if (num == oldInterval)
					{
						num4 *= 2.0;
					}
					num3++;
				}
				while (num == oldInterval && num3 <= 100);
			}
			else
			{
				if (oldInterval > 1.0 || oldInterval < 1.0)
				{
					if (intervalType == DateTimeIntervalType.Minutes || intervalType == DateTimeIntervalType.Seconds)
					{
						if (oldInterval >= 60.0)
						{
							num = Math.Round(oldInterval / 2.0);
						}
						else if (oldInterval >= 30.0)
						{
							num = 15.0;
						}
						else if (oldInterval >= 15.0)
						{
							num = 5.0;
						}
						else if (oldInterval >= 5.0)
						{
							num = 1.0;
						}
					}
					else
					{
						num = Math.Round(oldInterval / 2.0);
					}
					if (num < 1.0)
					{
						num = 1.0;
					}
				}
				if (oldInterval == 1.0)
				{
					if (intervalType == DateTimeIntervalType.Years)
					{
						num = 6.0;
						intervalType = DateTimeIntervalType.Months;
					}
					else if (intervalType == DateTimeIntervalType.Months)
					{
						num = 2.0;
						intervalType = DateTimeIntervalType.Weeks;
					}
					else if (intervalType == DateTimeIntervalType.Weeks)
					{
						num = 2.0;
						intervalType = DateTimeIntervalType.Days;
					}
					else if (intervalType == DateTimeIntervalType.Days)
					{
						num = 12.0;
						intervalType = DateTimeIntervalType.Hours;
					}
					else if (intervalType == DateTimeIntervalType.Hours)
					{
						num = 30.0;
						intervalType = DateTimeIntervalType.Minutes;
					}
					else if (intervalType == DateTimeIntervalType.Minutes)
					{
						num = 30.0;
						intervalType = DateTimeIntervalType.Seconds;
					}
					else if (intervalType == DateTimeIntervalType.Seconds)
					{
						num = 100.0;
						intervalType = DateTimeIntervalType.Milliseconds;
					}
				}
			}
			if (!double.IsNaN(minInterval) && num < minInterval)
			{
				num = 0.0;
			}
			return num;
		}

		private double IncreaseLabelInterval(double oldInterval, AxisScaleSegment axisScaleSegment, ref DateTimeIntervalType intervalType)
		{
			double num = oldInterval;
			double num2 = maximum - minimum;
			int num3 = 0;
			if (intervalType == DateTimeIntervalType.Auto || intervalType == DateTimeIntervalType.NotSet || intervalType == DateTimeIntervalType.Number)
			{
				double num4 = 2.0;
				do
				{
					num = CalcInterval(num2 / (num2 / (num * num4)));
					if (num == oldInterval)
					{
						num4 *= 2.0;
					}
					num3++;
				}
				while (num == oldInterval && num3 <= 100);
			}
			else
			{
				num = oldInterval * 2.0;
				if (intervalType != DateTimeIntervalType.Years)
				{
					if (intervalType == DateTimeIntervalType.Months)
					{
						if (num >= 12.0)
						{
							num = 1.0;
							intervalType = DateTimeIntervalType.Years;
						}
					}
					else if (intervalType == DateTimeIntervalType.Weeks)
					{
						if (num >= 4.0)
						{
							num = 1.0;
							intervalType = DateTimeIntervalType.Months;
						}
					}
					else if (intervalType == DateTimeIntervalType.Days)
					{
						if (num >= 7.0)
						{
							num = 1.0;
							intervalType = DateTimeIntervalType.Weeks;
						}
					}
					else if (intervalType == DateTimeIntervalType.Hours)
					{
						if (num >= 60.0)
						{
							num = 1.0;
							intervalType = DateTimeIntervalType.Days;
						}
					}
					else if (intervalType == DateTimeIntervalType.Minutes)
					{
						if (num >= 60.0)
						{
							num = 1.0;
							intervalType = DateTimeIntervalType.Hours;
						}
					}
					else if (intervalType == DateTimeIntervalType.Seconds)
					{
						if (num >= 60.0)
						{
							num = 1.0;
							intervalType = DateTimeIntervalType.Minutes;
						}
					}
					else if (intervalType == DateTimeIntervalType.Milliseconds && num >= 1000.0)
					{
						num = 1.0;
						intervalType = DateTimeIntervalType.Seconds;
					}
				}
			}
			return num;
		}

		private bool WordWrapLongestLabel(CustomLabelsCollection labels)
		{
			bool flag = false;
			ArrayList arrayList = new ArrayList(labels.Count);
			foreach (CustomLabel label in labels)
			{
				arrayList.Add(label.Text.Split('\n'));
			}
			int num = 5;
			int num2 = -1;
			int num3 = -1;
			int num4 = 0;
			foreach (string[] item in arrayList)
			{
				for (int i = 0; i < item.Length; i++)
				{
					if (item[i].Length > num && item[i].Trim().IndexOf(' ') > 0)
					{
						num = item[i].Length;
						num2 = num4;
						num3 = i;
					}
				}
				num4++;
			}
			if (num2 >= 0 && num3 >= 0)
			{
				string text = ((string[])arrayList[num2])[num3];
				for (num4 = 0; num4 < text.Length / 2 - 1; num4++)
				{
					if (text[text.Length / 2 - num4] == ' ')
					{
						text = text.Substring(0, text.Length / 2 - num4) + "\n" + text.Substring(text.Length / 2 - num4 + 1);
						flag = true;
					}
					else if (text[text.Length / 2 + num4] == ' ')
					{
						text = text.Substring(0, text.Length / 2 + num4) + "\n" + text.Substring(text.Length / 2 + num4 + 1);
						flag = true;
					}
					if (flag)
					{
						((string[])arrayList[num2])[num3] = text;
						break;
					}
				}
				if (flag)
				{
					CustomLabel customLabel2 = labels[num2];
					customLabel2.Text = string.Empty;
					for (int j = 0; j < ((string[])arrayList[num2]).Length; j++)
					{
						if (j > 0)
						{
							customLabel2.Text += "\n";
						}
						customLabel2.Text += ((string[])arrayList[num2])[j];
					}
				}
			}
			return flag;
		}

		internal void GetCircularAxisLabelsAutoFitFont(ChartGraphics graph, ArrayList axisList, CircularAxisLabelsStyle labelsStyle, RectangleF plotAreaRectAbs, RectangleF areaRectAbs, float labelsSizeEstimate)
		{
			autoLabelFont.Dispose();
			autoLabelFont = null;
			if (!LabelsAutoFit || LabelsAutoFitStyle == LabelsAutoFitStyles.None || !base.LabelStyle.Enabled)
			{
				return;
			}
			minLabelFontSize = Math.Min(LabelsAutoFitMinFontSize, LabelsAutoFitMaxFontSize);
			autoLabelFont = new Font(base.LabelStyle.Font.FontFamily, Math.Max(LabelsAutoFitMaxFontSize, LabelsAutoFitMinFontSize), base.LabelStyle.Font.Style, GraphicsUnit.Point);
			if ((LabelsAutoFitStyle & LabelsAutoFitStyles.IncreaseFont) != LabelsAutoFitStyles.IncreaseFont)
			{
				autoLabelFont = (Font)base.LabelStyle.Font.Clone();
			}
			bool flag = false;
			while (!flag)
			{
				flag = CheckCircularLabelsFit(graph, axisList, labelsStyle, plotAreaRectAbs, areaRectAbs, labelsSizeEstimate);
				if (!flag)
				{
					if (autoLabelFont.SizeInPoints > minLabelFontSize && (LabelsAutoFitStyle & LabelsAutoFitStyles.DecreaseFont) == LabelsAutoFitStyles.DecreaseFont)
					{
						autoLabelFont = new Font(autoLabelFont.FontFamily, autoLabelFont.SizeInPoints - 1f, autoLabelFont.Style, GraphicsUnit.Point);
						continue;
					}
					autoLabelAngle = 0;
					autoLabelOffset = 0;
					flag = true;
				}
			}
		}

		internal bool CheckCircularLabelsFit(ChartGraphics graph, ArrayList axisList, CircularAxisLabelsStyle labelsStyle, RectangleF plotAreaRectAbs, RectangleF areaRectAbs, float labelsSizeEstimate)
		{
			bool result = true;
			PointF absolutePoint = graph.GetAbsolutePoint(chartArea.circularCenter);
			float y = graph.GetAbsolutePoint(new PointF(0f, markSize + 1f)).Y;
			RectangleF rect = RectangleF.Empty;
			float num = float.NaN;
			foreach (CircularChartAreaAxis axis in axisList)
			{
				SizeF sizeF = graph.MeasureString(axis.Title.Replace("\\n", "\n"), autoLabelFont);
				switch (labelsStyle)
				{
				case CircularAxisLabelsStyle.Circular:
				case CircularAxisLabelsStyle.Radial:
				{
					if (labelsStyle == CircularAxisLabelsStyle.Radial)
					{
						float width = sizeF.Width;
						sizeF.Width = sizeF.Height;
						sizeF.Height = width;
					}
					float num3 = absolutePoint.Y - plotAreaRectAbs.Y;
					num3 -= labelsSizeEstimate;
					num3 += y;
					float num4 = (float)(Math.Atan(sizeF.Width / 2f / num3) * 180.0 / Math.PI);
					float num5 = axis.AxisPosition + num4;
					num4 = axis.AxisPosition - num4;
					if (!float.IsNaN(num) && num > num4)
					{
						return false;
					}
					num = num5 - 1f;
					PointF pointF = new PointF(absolutePoint.X, plotAreaRectAbs.Y);
					pointF.Y += labelsSizeEstimate;
					pointF.Y -= sizeF.Height;
					pointF.Y -= y;
					PointF[] array2 = new PointF[1]
					{
						pointF
					};
					Matrix matrix2 = new Matrix();
					matrix2.RotateAt(axis.AxisPosition, absolutePoint);
					matrix2.TransformPoints(array2);
					if (!areaRectAbs.Contains(array2[0]))
					{
						return false;
					}
					break;
				}
				case CircularAxisLabelsStyle.Horizontal:
				{
					float num2 = axis.AxisPosition;
					if (num2 > 180f)
					{
						num2 -= 180f;
					}
					PointF[] array = new PointF[1]
					{
						new PointF(absolutePoint.X, plotAreaRectAbs.Y)
					};
					array[0].Y += labelsSizeEstimate;
					array[0].Y -= y;
					Matrix matrix = new Matrix();
					matrix.RotateAt(num2, absolutePoint);
					matrix.TransformPoints(array);
					RectangleF rectangleF = new RectangleF(array[0].X, array[0].Y - sizeF.Height / 2f, sizeF.Width, sizeF.Height);
					if (num2 < 5f)
					{
						rectangleF.X = array[0].X - sizeF.Width / 2f;
						rectangleF.Y = array[0].Y - sizeF.Height;
					}
					if (num2 > 175f)
					{
						rectangleF.X = array[0].X - sizeF.Width / 2f;
						rectangleF.Y = array[0].Y;
					}
					rectangleF.Inflate(0f, (0f - rectangleF.Height) * 0.15f);
					if (!areaRectAbs.Contains(rectangleF))
					{
						return false;
					}
					if (!rect.IsEmpty && rectangleF.IntersectsWith(rect))
					{
						return false;
					}
					rect = rectangleF;
					break;
				}
				}
			}
			return result;
		}

		internal void AdjustLabelFontAtSecondPass(ChartGraphics chartGraph, bool autoPlotPosition)
		{
			if (base.Enabled != AxisEnabled.False && base.LabelStyle.Enabled && IsVariableLabelCountModeEnabled())
			{
				if (autoLabelFont == null)
				{
					autoLabelFont = base.LabelStyle.Font;
				}
				if (autoLabelAngle < 0)
				{
					autoLabelAngle = base.LabelStyle.FontAngle;
				}
				if (autoLabelOffset < 0)
				{
					autoLabelOffset = (base.LabelStyle.OffsetLabels ? 1 : 0);
				}
				if (!CheckLabelsFit(chartGraph, markSize + scrollBarSize + titleSize, autoPlotPosition, checkLabelsFirstRowOnly: true, secondPass: true, (AxisPosition != 0 && AxisPosition != AxisPosition.Right) ? true : false, (AxisPosition == AxisPosition.Left || AxisPosition == AxisPosition.Right) ? true : false, null))
				{
					AdjustIntervalToFitLabels(chartGraph, autoPlotPosition, onlyIncreaseInterval: true);
				}
			}
			totlaGroupingLabelsSizeAdjustment = 0f;
			if (!LabelsAutoFit || LabelsAutoFitStyle == LabelsAutoFitStyles.None || base.Enabled == AxisEnabled.False)
			{
				return;
			}
			bool flag = false;
			if (autoLabelFont == null)
			{
				autoLabelFont = base.LabelStyle.Font;
			}
			float num = totlaGroupingLabelsSize;
			while (!flag)
			{
				flag = CheckLabelsFit(chartGraph, markSize + scrollBarSize + titleSize, autoPlotPosition, checkLabelsFirstRowOnly: true, secondPass: true);
				if (flag)
				{
					continue;
				}
				if (autoLabelFont.SizeInPoints > minLabelFontSize)
				{
					if (chartArea != null && chartArea.EquallySizedAxesFont)
					{
						Axis[] axes = chartArea.Axes;
						foreach (Axis axis in axes)
						{
							if (axis.enabled && axis.LabelsAutoFit && axis.autoLabelFont != null)
							{
								axis.autoLabelFont = new Font(axis.autoLabelFont.FontFamily, autoLabelFont.SizeInPoints - 1f, axis.autoLabelFont.Style, GraphicsUnit.Point);
							}
						}
					}
					else if ((LabelsAutoFitStyle & LabelsAutoFitStyles.DecreaseFont) == LabelsAutoFitStyles.DecreaseFont)
					{
						autoLabelFont = new Font(autoLabelFont.FontFamily, autoLabelFont.SizeInPoints - 1f, autoLabelFont.Style, GraphicsUnit.Point);
					}
					else
					{
						flag = true;
					}
				}
				else
				{
					flag = true;
				}
			}
			totlaGroupingLabelsSizeAdjustment = num - totlaGroupingLabelsSize;
		}

		internal double GetLogValue(double yValue)
		{
			if (base.Logarithmic)
			{
				yValue = Math.Log(yValue, logarithmBase);
			}
			return yValue;
		}

		private bool CheckLabelsFit(ChartGraphics chartGraph, float otherElementsSize, bool autoPlotPosition, bool checkLabelsFirstRowOnly, bool secondPass)
		{
			return CheckLabelsFit(chartGraph, otherElementsSize, autoPlotPosition, checkLabelsFirstRowOnly, secondPass, checkWidth: true, checkHeight: true, null);
		}

		private bool CheckLabelsFit(ChartGraphics chartGraph, float otherElementsSize, bool autoPlotPosition, bool checkLabelsFirstRowOnly, bool secondPass, bool checkWidth, bool checkHeight, ArrayList labelPositions)
		{
			labelPositions?.Clear();
			StringFormat stringFormat = new StringFormat();
			stringFormat.FormatFlags |= StringFormatFlags.LineLimit;
			stringFormat.Trimming = StringTrimming.EllipsisCharacter;
			RectangleF empty = RectangleF.Empty;
			float num = 0f;
			if (autoPlotPosition)
			{
				num = ((AxisPosition != AxisPosition.Bottom && AxisPosition != AxisPosition.Top) ? chartArea.Position.Width : chartArea.Position.Height);
			}
			else
			{
				if (IsMarksNextToAxis())
				{
					if (AxisPosition == AxisPosition.Top)
					{
						num = (float)GetAxisPosition() - chartArea.Position.Y;
					}
					else if (AxisPosition == AxisPosition.Bottom)
					{
						num = chartArea.Position.Bottom() - (float)GetAxisPosition();
					}
					if (AxisPosition == AxisPosition.Left)
					{
						num = (float)GetAxisPosition() - chartArea.Position.X;
					}
					else if (AxisPosition == AxisPosition.Right)
					{
						num = chartArea.Position.Right() - (float)GetAxisPosition();
					}
				}
				else
				{
					if (AxisPosition == AxisPosition.Top)
					{
						num = base.PlotAreaPosition.Y - chartArea.Position.Y;
					}
					else if (AxisPosition == AxisPosition.Bottom)
					{
						num = chartArea.Position.Bottom() - base.PlotAreaPosition.Bottom();
					}
					if (AxisPosition == AxisPosition.Left)
					{
						num = base.PlotAreaPosition.X - chartArea.Position.X;
					}
					else if (AxisPosition == AxisPosition.Right)
					{
						num = chartArea.Position.Right() - base.PlotAreaPosition.Right();
					}
				}
				num *= 2f;
			}
			totlaGroupingLabelsSize = 0f;
			int groupLabelLevelCount = GetGroupLabelLevelCount();
			if (groupLabelLevelCount > 0)
			{
				groupingLabelSizes = new float[groupLabelLevelCount];
				bool flag = true;
				for (int i = 1; i <= groupLabelLevelCount; i++)
				{
					groupingLabelSizes[i - 1] = 0f;
					foreach (CustomLabel customLabel3 in base.CustomLabels)
					{
						if (customLabel3.RowIndex == 0)
						{
							double num2 = (customLabel3.From + customLabel3.To) / 2.0;
							if (num2 < GetViewMinimum() || num2 > GetViewMaximum())
							{
								continue;
							}
						}
						if (customLabel3.RowIndex != i)
						{
							continue;
						}
						double linearPosition = GetLinearPosition(customLabel3.From);
						double linearPosition2 = GetLinearPosition(customLabel3.To);
						if (AxisPosition == AxisPosition.Bottom || AxisPosition == AxisPosition.Top)
						{
							empty.Height = num / 100f * 45f / (float)groupLabelLevelCount;
							empty.X = (float)Math.Min(linearPosition, linearPosition2);
							empty.Width = (float)Math.Max(linearPosition, linearPosition2) - empty.X;
						}
						else
						{
							empty.Width = num / 100f * 45f / (float)groupLabelLevelCount;
							empty.Y = (float)Math.Min(linearPosition, linearPosition2);
							empty.Height = (float)Math.Max(linearPosition, linearPosition2) - empty.Y;
						}
						SizeF sizeF = chartGraph.MeasureStringRel(customLabel3.Text.Replace("\\n", "\n"), autoLabelFont);
						if (customLabel3.Image.Length > 0)
						{
							SizeF size = default(SizeF);
							if (base.Common.ImageLoader.GetAdjustedImageSize(customLabel3.Image, chartGraph.Graphics, ref size))
							{
								SizeF relativeSize = chartGraph.GetRelativeSize(size);
								sizeF.Width += relativeSize.Width;
								sizeF.Height = Math.Max(sizeF.Height, relativeSize.Height);
							}
						}
						if (customLabel3.LabelMark == LabelMark.Box)
						{
							SizeF relativeSize2 = chartGraph.GetRelativeSize(new SizeF(4f, 4f));
							sizeF.Width += relativeSize2.Width;
							sizeF.Height += relativeSize2.Height;
						}
						if (AxisPosition == AxisPosition.Bottom || AxisPosition == AxisPosition.Top)
						{
							groupingLabelSizes[i - 1] = Math.Max(groupingLabelSizes[i - 1], sizeF.Height);
						}
						else
						{
							sizeF.Width = chartGraph.GetAbsoluteSize(new SizeF(sizeF.Height, sizeF.Height)).Height;
							sizeF.Width = chartGraph.GetRelativeSize(new SizeF(sizeF.Width, sizeF.Width)).Width;
							groupingLabelSizes[i - 1] = Math.Max(groupingLabelSizes[i - 1], sizeF.Width);
						}
						if (Math.Round(sizeF.Width) >= Math.Round(empty.Width) && checkWidth)
						{
							flag = false;
						}
						if (Math.Round(sizeF.Height) >= Math.Round(empty.Height) && checkHeight)
						{
							flag = false;
						}
					}
				}
				totlaGroupingLabelsSize = GetGroupLablesToatalSize();
				if (!flag && !checkLabelsFirstRowOnly)
				{
					return false;
				}
			}
			float num3 = autoLabelAngle;
			int num4 = 0;
			foreach (CustomLabel customLabel4 in base.CustomLabels)
			{
				if (customLabel4.RowIndex == 0)
				{
					double num5 = (customLabel4.From + customLabel4.To) / 2.0;
					if (num5 < GetViewMinimum() || num5 > GetViewMaximum())
					{
						continue;
					}
				}
				if (customLabel4.RowIndex != 0)
				{
					continue;
				}
				if (labelPositions != null)
				{
					base.ScaleSegments.EnforceSegment(base.ScaleSegments.FindScaleSegmentForAxisValue((customLabel4.From + customLabel4.To) / 2.0));
				}
				double linearPosition3 = GetLinearPosition(customLabel4.From);
				double linearPosition4 = GetLinearPosition(customLabel4.To);
				if (labelPositions != null)
				{
					base.ScaleSegments.EnforceSegment(null);
				}
				empty.X = base.PlotAreaPosition.X;
				empty.Y = (float)Math.Min(linearPosition3, linearPosition4);
				empty.Height = (float)Math.Max(linearPosition3, linearPosition4) - empty.Y;
				float num6 = 75f;
				if (75f - totlaGroupingLabelsSize > 55f)
				{
					num6 = 55f + totlaGroupingLabelsSize;
				}
				if (AxisPosition == AxisPosition.Bottom || AxisPosition == AxisPosition.Top)
				{
					empty.Width = num / 100f * (num6 - totlaGroupingLabelsSize - otherElementsSize - 1f);
				}
				else
				{
					empty.Width = num / 100f * (num6 - totlaGroupingLabelsSize - otherElementsSize - 1f);
				}
				if (autoLabelOffset == 1)
				{
					empty.Y -= empty.Height / 2f;
					empty.Height *= 2f;
					empty.Width /= 2f;
				}
				if (AxisPosition == AxisPosition.Bottom || AxisPosition == AxisPosition.Top)
				{
					float height = empty.Height;
					empty.Height = empty.Width;
					empty.Width = height;
					if (num3 != 0f)
					{
						stringFormat.FormatFlags |= StringFormatFlags.DirectionVertical;
					}
				}
				else if (num3 == 90f || num3 == -90f)
				{
					num3 = 0f;
					stringFormat.FormatFlags |= StringFormatFlags.DirectionVertical;
				}
				SizeF sizeF2 = chartGraph.MeasureStringRel(customLabel4.Text.Replace("\\n", "\n") + "W", autoLabelFont, secondPass ? empty.Size : chartArea.Position.ToRectangleF().Size, stringFormat);
				if (customLabel4.Text.Length > 0 && (sizeF2.Width == 0f || sizeF2.Height == 0f))
				{
					stringFormat.FormatFlags ^= StringFormatFlags.LineLimit;
					sizeF2 = chartGraph.MeasureStringRel(customLabel4.Text.Replace("\\n", "\n"), autoLabelFont, secondPass ? empty.Size : chartArea.Position.ToRectangleF().Size, stringFormat);
					stringFormat.FormatFlags |= StringFormatFlags.LineLimit;
				}
				if (customLabel4.Image.Length > 0)
				{
					SizeF size2 = default(SizeF);
					if (base.Common.ImageLoader.GetAdjustedImageSize(customLabel4.Image, chartGraph.Graphics, ref size2))
					{
						SizeF relativeSize3 = chartGraph.GetRelativeSize(size2);
						if ((stringFormat.FormatFlags & StringFormatFlags.DirectionVertical) == StringFormatFlags.DirectionVertical)
						{
							sizeF2.Height += relativeSize3.Height;
							sizeF2.Width = Math.Max(sizeF2.Width, relativeSize3.Width);
						}
						else
						{
							sizeF2.Width += relativeSize3.Width;
							sizeF2.Height = Math.Max(sizeF2.Height, relativeSize3.Height);
						}
					}
				}
				if (customLabel4.LabelMark == LabelMark.Box)
				{
					SizeF relativeSize4 = chartGraph.GetRelativeSize(new SizeF(4f, 4f));
					sizeF2.Width += relativeSize4.Width;
					sizeF2.Height += relativeSize4.Height;
				}
				float num7 = sizeF2.Width;
				float num8 = sizeF2.Height;
				if (num3 != 0f)
				{
					empty.Width *= 0.97f;
					if (AxisPosition == AxisPosition.Bottom || AxisPosition == AxisPosition.Top)
					{
						num7 = (float)Math.Cos((double)(Math.Abs(num3) / 180f) * Math.PI) * sizeF2.Height;
						num7 += (float)Math.Sin((double)(Math.Abs(num3) / 180f) * Math.PI) * sizeF2.Width;
						num8 = (float)Math.Sin((double)(Math.Abs(num3) / 180f) * Math.PI) * sizeF2.Height;
						num8 += (float)Math.Cos((double)(Math.Abs(num3) / 180f) * Math.PI) * sizeF2.Width;
					}
					else
					{
						num7 = (float)Math.Cos((double)(Math.Abs(num3) / 180f) * Math.PI) * sizeF2.Width;
						num7 += (float)Math.Sin((double)(Math.Abs(num3) / 180f) * Math.PI) * sizeF2.Height;
						num8 = (float)Math.Sin((double)(Math.Abs(num3) / 180f) * Math.PI) * sizeF2.Width;
						num8 += (float)Math.Cos((double)(Math.Abs(num3) / 180f) * Math.PI) * sizeF2.Height;
					}
				}
				if (labelPositions != null)
				{
					RectangleF rectangleF = empty;
					if (num3 == 0f || num3 == 90f || num3 == -90f)
					{
						if (AxisPosition == AxisPosition.Bottom || AxisPosition == AxisPosition.Top)
						{
							rectangleF.X = rectangleF.X + rectangleF.Width / 2f - num7 / 2f;
							rectangleF.Width = num7;
						}
						else
						{
							rectangleF.Y = rectangleF.Y + rectangleF.Height / 2f - num8 / 2f;
							rectangleF.Height = num8;
						}
					}
					labelPositions.Add(rectangleF);
				}
				if (num3 == 0f)
				{
					if (num7 >= empty.Width && checkWidth)
					{
						return false;
					}
					if (num8 >= empty.Height && checkHeight)
					{
						return false;
					}
				}
				if (num3 == 90f || num3 == -90f)
				{
					if (num7 >= empty.Width && checkWidth)
					{
						return false;
					}
					if (num8 >= empty.Height && checkHeight)
					{
						return false;
					}
				}
				else if (AxisPosition == AxisPosition.Bottom || AxisPosition == AxisPosition.Top)
				{
					if (num7 >= empty.Width * 2f && checkWidth)
					{
						return false;
					}
					if (num8 >= empty.Height * 2f && checkHeight)
					{
						return false;
					}
				}
				else
				{
					if (num7 >= empty.Width * 2f && checkWidth)
					{
						return false;
					}
					if (num8 >= empty.Height * 2f && checkHeight)
					{
						return false;
					}
				}
				num4++;
			}
			return true;
		}

		private float GetRequiredLabelSize(ChartGraphics chartGraph, float maxLabelSize, out float resultSize)
		{
			float num = 0f;
			resultSize = 0f;
			float num2 = (autoLabelAngle < -90) ? base.LabelStyle.FontAngle : autoLabelAngle;
			labelNearOffset = float.MaxValue;
			labelFarOffset = float.MinValue;
			StringFormat stringFormat = new StringFormat();
			stringFormat.FormatFlags |= StringFormatFlags.LineLimit;
			stringFormat.Trimming = StringTrimming.EllipsisCharacter;
			RectangleF rectangleF = chartArea.Position.ToRectangleF();
			foreach (CustomLabel customLabel in base.CustomLabels)
			{
				if (customLabel.RowIndex == 0)
				{
					double value = (customLabel.From + customLabel.To) / 2.0;
					if (RemoveNoiseFromDoubleMath(value) < RemoveNoiseFromDoubleMath(GetViewMinimum()) || RemoveNoiseFromDoubleMath(value) > RemoveNoiseFromDoubleMath(GetViewMaximum()))
					{
						continue;
					}
				}
				if (customLabel.RowIndex != 0)
				{
					continue;
				}
				RectangleF rectangleF2 = rectangleF;
				rectangleF2.Width = maxLabelSize;
				double linearPosition = GetLinearPosition(customLabel.From);
				double linearPosition2 = GetLinearPosition(customLabel.To);
				rectangleF2.Y = (float)Math.Min(linearPosition, linearPosition2);
				rectangleF2.Height = (float)Math.Max(linearPosition, linearPosition2) - rectangleF2.Y;
				if ((autoLabelOffset == -1) ? base.LabelStyle.OffsetLabels : (autoLabelOffset == 1))
				{
					rectangleF2.Y -= rectangleF2.Height / 2f;
					rectangleF2.Height *= 2f;
				}
				if (AxisPosition == AxisPosition.Bottom || AxisPosition == AxisPosition.Top)
				{
					float height = rectangleF2.Height;
					rectangleF2.Height = rectangleF2.Width;
					rectangleF2.Width = height;
					if (num2 != 0f)
					{
						stringFormat.FormatFlags |= StringFormatFlags.DirectionVertical;
					}
				}
				else if (num2 == 90f || num2 == -90f)
				{
					num2 = 0f;
					stringFormat.FormatFlags |= StringFormatFlags.DirectionVertical;
				}
				rectangleF2.Width = (float)Math.Ceiling(rectangleF2.Width);
				rectangleF2.Height = (float)Math.Ceiling(rectangleF2.Height);
				SizeF sizeF = chartGraph.MeasureStringRel(customLabel.Text.Replace("\\n", "\n"), (autoLabelFont != null) ? autoLabelFont : base.LabelStyle.Font, rectangleF2.Size, stringFormat);
				if (sizeF.Width == 0f || sizeF.Height == 0f)
				{
					stringFormat.FormatFlags ^= StringFormatFlags.LineLimit;
					sizeF = chartGraph.MeasureStringRel(customLabel.Text.Replace("\\n", "\n"), (autoLabelFont != null) ? autoLabelFont : base.LabelStyle.Font, rectangleF2.Size, stringFormat);
					stringFormat.FormatFlags |= StringFormatFlags.LineLimit;
				}
				if (customLabel.Image.Length > 0)
				{
					SizeF size = default(SizeF);
					if (base.Common.ImageLoader.GetAdjustedImageSize(customLabel.Image, chartGraph.Graphics, ref size))
					{
						SizeF relativeSize = chartGraph.GetRelativeSize(size);
						if ((stringFormat.FormatFlags & StringFormatFlags.DirectionVertical) == StringFormatFlags.DirectionVertical)
						{
							sizeF.Height += relativeSize.Height;
							sizeF.Width = Math.Max(sizeF.Width, relativeSize.Width);
						}
						else
						{
							sizeF.Width += relativeSize.Width;
							sizeF.Height = Math.Max(sizeF.Height, relativeSize.Height);
						}
					}
				}
				if (customLabel.LabelMark == LabelMark.Box)
				{
					SizeF relativeSize2 = chartGraph.GetRelativeSize(new SizeF(4f, 4f));
					sizeF.Width += relativeSize2.Width;
					sizeF.Height += relativeSize2.Height;
				}
				float num3 = sizeF.Width;
				float num4 = sizeF.Height;
				if (num2 != 0f)
				{
					num3 = (float)Math.Cos((double)((90f - Math.Abs(num2)) / 180f) * Math.PI) * sizeF.Width;
					num3 += (float)Math.Cos((double)(Math.Abs(num2) / 180f) * Math.PI) * sizeF.Height;
					num4 = (float)Math.Sin((double)(Math.Abs(num2) / 180f) * Math.PI) * sizeF.Height;
					num4 += (float)Math.Sin((double)((90f - Math.Abs(num2)) / 180f) * Math.PI) * sizeF.Width;
				}
				num3 = (float)Math.Ceiling(num3) * 1.05f;
				num4 = (float)Math.Ceiling(num4) * 1.05f;
				sizeF.Width = (float)Math.Ceiling(sizeF.Width) * 1.05f;
				sizeF.Height = (float)Math.Ceiling(sizeF.Height) * 1.05f;
				if (AxisPosition == AxisPosition.Bottom || AxisPosition == AxisPosition.Top)
				{
					if (num2 == 90f || num2 == -90f || num2 == 0f)
					{
						resultSize = Math.Max(resultSize, sizeF.Height);
						num = Math.Max(num, sizeF.Height);
						labelNearOffset = (float)Math.Min(labelNearOffset, (linearPosition + linearPosition2) / 2.0 - (double)(sizeF.Width / 2f));
						labelFarOffset = (float)Math.Max(labelFarOffset, (linearPosition + linearPosition2) / 2.0 + (double)(sizeF.Width / 2f));
					}
					else
					{
						resultSize = Math.Max(resultSize, sizeF.Height);
						num = Math.Max(num, num4);
						if (num2 > 0f)
						{
							labelFarOffset = (float)Math.Max(labelFarOffset, (linearPosition + linearPosition2) / 2.0 + (double)(num3 * 1.1f));
						}
						else
						{
							labelNearOffset = (float)Math.Min(labelNearOffset, (linearPosition + linearPosition2) / 2.0 - (double)(num3 * 1.1f));
						}
					}
				}
				else if (num2 == 90f || num2 == -90f || num2 == 0f)
				{
					resultSize = Math.Max(resultSize, sizeF.Width);
					num = Math.Max(num, sizeF.Width);
					labelNearOffset = (float)Math.Min(labelNearOffset, (linearPosition + linearPosition2) / 2.0 - (double)(sizeF.Height / 2f));
					labelFarOffset = (float)Math.Max(labelFarOffset, (linearPosition + linearPosition2) / 2.0 + (double)(sizeF.Height / 2f));
				}
				else
				{
					resultSize = Math.Max(resultSize, sizeF.Width);
					num = Math.Max(num, num3);
					if (num2 > 0f)
					{
						labelFarOffset = (float)Math.Max(labelFarOffset, (linearPosition + linearPosition2) / 2.0 + (double)(num4 * 1.1f));
					}
					else
					{
						labelNearOffset = (float)Math.Min(labelNearOffset, (linearPosition + linearPosition2) / 2.0 - (double)(num4 * 1.1f));
					}
				}
				if (resultSize > maxLabelSize)
				{
					resultSize = maxLabelSize;
				}
			}
			if ((autoLabelOffset == -1) ? base.LabelStyle.OffsetLabels : (autoLabelOffset == 1))
			{
				resultSize *= 2f;
				num *= 2f;
				if (resultSize > maxLabelSize)
				{
					resultSize = maxLabelSize;
					num = maxLabelSize;
				}
			}
			if (chartArea.Area3DStyle.Enable3D && !chartArea.chartAreaIsCurcular)
			{
				resultSize *= 1.1f;
				num *= 1.1f;
			}
			return num;
		}

		internal float GetGroupLablesToatalSize()
		{
			float num = 0f;
			if (groupingLabelSizes != null && groupingLabelSizes.Length != 0)
			{
				float[] array = groupingLabelSizes;
				foreach (float num2 in array)
				{
					num += num2;
				}
			}
			return num;
		}

		internal int GetGroupLabelLevelCount()
		{
			int num = 0;
			foreach (CustomLabel customLabel in base.CustomLabels)
			{
				if (customLabel.RowIndex > 0)
				{
					num = Math.Max(num, customLabel.RowIndex);
				}
			}
			return num;
		}

		private float[] GetRequiredGroupLabelSize(ChartGraphics chartGraph, float maxLabelSize)
		{
			float[] array = null;
			int groupLabelLevelCount = GetGroupLabelLevelCount();
			if (groupLabelLevelCount > 0)
			{
				array = new float[groupLabelLevelCount];
				for (int i = 1; i <= groupLabelLevelCount; i++)
				{
					array[i - 1] = 0f;
					foreach (CustomLabel customLabel in base.CustomLabels)
					{
						if (customLabel.RowIndex == 0)
						{
							double num = (customLabel.From + customLabel.To) / 2.0;
							if (num < GetViewMinimum() || num > GetViewMaximum())
							{
								continue;
							}
						}
						if (customLabel.RowIndex != i)
						{
							continue;
						}
						SizeF sizeF = chartGraph.MeasureStringRel(customLabel.Text.Replace("\\n", "\n"), (autoLabelFont != null) ? autoLabelFont : base.LabelStyle.Font);
						sizeF.Width = (float)Math.Ceiling(sizeF.Width);
						sizeF.Height = (float)Math.Ceiling(sizeF.Height);
						if (customLabel.Image.Length > 0)
						{
							SizeF size = default(SizeF);
							if (base.Common.ImageLoader.GetAdjustedImageSize(customLabel.Image, chartGraph.Graphics, ref size))
							{
								SizeF relativeSize = chartGraph.GetRelativeSize(size);
								sizeF.Width += relativeSize.Width;
								sizeF.Height = Math.Max(sizeF.Height, relativeSize.Height);
							}
						}
						if (customLabel.LabelMark == LabelMark.Box)
						{
							SizeF relativeSize2 = chartGraph.GetRelativeSize(new SizeF(4f, 4f));
							sizeF.Width += relativeSize2.Width;
							sizeF.Height += relativeSize2.Height;
						}
						if (AxisPosition == AxisPosition.Bottom || AxisPosition == AxisPosition.Top)
						{
							array[i - 1] = Math.Max(array[i - 1], sizeF.Height);
						}
						else
						{
							sizeF.Width = chartGraph.GetAbsoluteSize(new SizeF(sizeF.Height, sizeF.Height)).Height;
							sizeF.Width = chartGraph.GetRelativeSize(new SizeF(sizeF.Width, sizeF.Width)).Width;
							array[i - 1] = Math.Max(array[i - 1], sizeF.Width);
						}
						_ = array[i - 1];
						_ = maxLabelSize / (float)groupLabelLevelCount;
					}
				}
			}
			return array;
		}

		internal static double RemoveNoiseFromDoubleMath(double value)
		{
			if (value == 0.0 || (Math.Abs(value) > 7.9228162514264341E-28 && Math.Abs(value) < 7.9228162514264338E+28))
			{
				return (double)(decimal)value;
			}
			if (Math.Abs(value) >= double.MaxValue || Math.Abs(value) <= double.Epsilon)
			{
				return value;
			}
			return double.Parse(value.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
		}

		internal Axis GetSubAxis(string subAxisName)
		{
			return this;
		}

		internal bool IsMarksNextToAxis()
		{
			if (chartArea != null && chartArea.chartAreaIsCurcular)
			{
				return true;
			}
			return MarksNextToAxis;
		}

		internal bool IsSerializing()
		{
			if (chart == null && base.Common != null && base.Common.container != null)
			{
				chart = (Chart)base.Common.container.GetService(typeof(Chart));
			}
			if (chart != null)
			{
				return chart.serializing;
			}
			if (chartArea != null)
			{
				if (chartArea.chart == null && chartArea.Common != null && chartArea.Common.container != null)
				{
					chartArea.chart = (Chart)chartArea.Common.container.GetService(typeof(Chart));
				}
				if (chartArea.chart != null)
				{
					return chartArea.chart.serializing;
				}
			}
			return false;
		}

		internal DateTimeIntervalType GetAxisIntervalType()
		{
			if (base.InternalIntervalType == DateTimeIntervalType.Auto)
			{
				if (GetAxisValuesType() == ChartValueTypes.DateTime || GetAxisValuesType() == ChartValueTypes.Date || GetAxisValuesType() == ChartValueTypes.Time || GetAxisValuesType() == ChartValueTypes.DateTimeOffset)
				{
					return DateTimeIntervalType.Years;
				}
				return DateTimeIntervalType.Number;
			}
			return base.InternalIntervalType;
		}

		internal ChartValueTypes GetAxisValuesType()
		{
			ChartValueTypes result = ChartValueTypes.Double;
			if (base.Common != null && base.Common.DataManager.Series != null && chartArea != null)
			{
				foreach (Series item in base.Common.DataManager.Series)
				{
					bool flag = false;
					if (item.ChartArea == chartArea.Name && item.IsVisible())
					{
						if (axisType == AxisName.X && item.XAxisType == AxisType.Primary)
						{
							flag = true;
						}
						else if (axisType == AxisName.X2 && item.XAxisType == AxisType.Secondary)
						{
							flag = true;
						}
						else if (axisType == AxisName.Y && item.YAxisType == AxisType.Primary)
						{
							flag = true;
						}
						else if (axisType == AxisName.Y2 && item.YAxisType == AxisType.Secondary)
						{
							flag = true;
						}
					}
					if (flag)
					{
						if (axisType == AxisName.X || axisType == AxisName.X2)
						{
							return item.XValueType;
						}
						if (axisType == AxisName.Y || axisType == AxisName.Y2)
						{
							return item.YValueType;
						}
						return result;
					}
				}
				return result;
			}
			return result;
		}

		internal SizeF GetArrowSize(out ArrowOrientation arrowOrientation)
		{
			arrowOrientation = ArrowOrientation.Top;
			switch (AxisPosition)
			{
			case AxisPosition.Left:
				if (reverse)
				{
					arrowOrientation = ArrowOrientation.Bottom;
				}
				else
				{
					arrowOrientation = ArrowOrientation.Top;
				}
				break;
			case AxisPosition.Right:
				if (reverse)
				{
					arrowOrientation = ArrowOrientation.Bottom;
				}
				else
				{
					arrowOrientation = ArrowOrientation.Top;
				}
				break;
			case AxisPosition.Bottom:
				if (reverse)
				{
					arrowOrientation = ArrowOrientation.Left;
				}
				else
				{
					arrowOrientation = ArrowOrientation.Right;
				}
				break;
			case AxisPosition.Top:
				if (reverse)
				{
					arrowOrientation = ArrowOrientation.Left;
				}
				else
				{
					arrowOrientation = ArrowOrientation.Right;
				}
				break;
			}
			Axis axis;
			switch (arrowOrientation)
			{
			case ArrowOrientation.Left:
				axis = chartArea.AxisX;
				break;
			case ArrowOrientation.Right:
				axis = chartArea.AxisX2;
				break;
			case ArrowOrientation.Top:
				axis = chartArea.AxisY2;
				break;
			case ArrowOrientation.Bottom:
				axis = chartArea.AxisY;
				break;
			default:
				axis = chartArea.AxisX;
				break;
			}
			double num;
			double num2;
			if (arrowOrientation == ArrowOrientation.Top || arrowOrientation == ArrowOrientation.Bottom)
			{
				num = lineWidth;
				num2 = (double)lineWidth * (double)base.Common.Width / (double)base.Common.Height;
			}
			else
			{
				num = (double)lineWidth * (double)base.Common.Width / (double)base.Common.Height;
				num2 = lineWidth;
			}
			if (arrows == ArrowsType.SharpTriangle)
			{
				if (arrowOrientation == ArrowOrientation.Top || arrowOrientation == ArrowOrientation.Bottom)
				{
					return new SizeF((float)(num * 2.0), (float)((double)axis.MajorTickMark.Size + num2 * 4.0));
				}
				return new SizeF((float)((double)axis.MajorTickMark.Size + num2 * 4.0), (float)(num * 2.0));
			}
			if (arrows == ArrowsType.None)
			{
				return new SizeF(0f, 0f);
			}
			if (arrowOrientation == ArrowOrientation.Top || arrowOrientation == ArrowOrientation.Bottom)
			{
				return new SizeF((float)(num * 2.0), (float)((double)axis.MajorTickMark.Size + num2 * 2.0));
			}
			return new SizeF((float)((double)axis.MajorTickMark.Size + num2 * 2.0), (float)(num * 2.0));
		}

		private bool IsArrowInAxis(ArrowOrientation arrowOrientation, AxisPosition axisPosition)
		{
			if (axisPosition == AxisPosition.Top && arrowOrientation == ArrowOrientation.Top)
			{
				return true;
			}
			if (axisPosition == AxisPosition.Bottom && arrowOrientation == ArrowOrientation.Bottom)
			{
				return true;
			}
			if (axisPosition == AxisPosition.Left && arrowOrientation == ArrowOrientation.Left)
			{
				return true;
			}
			if (axisPosition == AxisPosition.Right && arrowOrientation == ArrowOrientation.Right)
			{
				return true;
			}
			return false;
		}

		internal float GetPixelInterval(double realInterval)
		{
			double num = (AxisPosition != AxisPosition.Top && AxisPosition != AxisPosition.Bottom) ? ((double)(base.PlotAreaPosition.Bottom() - base.PlotAreaPosition.Y)) : ((double)(base.PlotAreaPosition.Right() - base.PlotAreaPosition.X));
			if (GetViewMaximum() - GetViewMinimum() == 0.0)
			{
				return (float)(num / realInterval);
			}
			return (float)(num / (GetViewMaximum() - GetViewMinimum()) * realInterval);
		}

		internal bool IsAxisOnAreaEdge()
		{
			double num = 0.0;
			if (AxisPosition == AxisPosition.Bottom)
			{
				num = base.PlotAreaPosition.Bottom();
			}
			else if (AxisPosition == AxisPosition.Left)
			{
				num = base.PlotAreaPosition.X;
			}
			else if (AxisPosition == AxisPosition.Right)
			{
				num = base.PlotAreaPosition.Right();
			}
			else if (AxisPosition == AxisPosition.Top)
			{
				num = base.PlotAreaPosition.Y;
			}
			if (Math.Abs(GetAxisPosition() - num) < 0.0015)
			{
				return true;
			}
			return false;
		}

		internal double GetAxisPosition()
		{
			return GetAxisPosition(ignoreCrossing: false);
		}

		internal virtual double GetAxisPosition(bool ignoreCrossing)
		{
			Axis oppositeAxis = GetOppositeAxis();
			if (chartArea != null && chartArea.chartAreaIsCurcular)
			{
				return base.PlotAreaPosition.X + base.PlotAreaPosition.Width / 2f;
			}
			if (oppositeAxis.maximum == oppositeAxis.minimum || double.IsNaN(oppositeAxis.maximum) || double.IsNaN(oppositeAxis.minimum) || maximum == minimum || double.IsNaN(maximum) || double.IsNaN(minimum))
			{
				switch (AxisPosition)
				{
				case AxisPosition.Top:
					return base.PlotAreaPosition.Y;
				case AxisPosition.Bottom:
					return base.PlotAreaPosition.Bottom();
				case AxisPosition.Right:
					return base.PlotAreaPosition.Right();
				case AxisPosition.Left:
					return base.PlotAreaPosition.X;
				}
			}
			if (double.IsNaN(oppositeAxis.crossing) || ignoreCrossing)
			{
				if (axisType == AxisName.X || axisType == AxisName.Y)
				{
					return oppositeAxis.GetLinearPosition(oppositeAxis.GetViewMinimum());
				}
				return oppositeAxis.GetLinearPosition(oppositeAxis.GetViewMaximum());
			}
			oppositeAxis.crossing = oppositeAxis.tempCrossing;
			if (oppositeAxis.crossing < oppositeAxis.GetViewMinimum())
			{
				oppositeAxis.crossing = oppositeAxis.GetViewMinimum();
			}
			else if (oppositeAxis.crossing > oppositeAxis.GetViewMaximum())
			{
				oppositeAxis.crossing = oppositeAxis.GetViewMaximum();
			}
			return oppositeAxis.GetLinearPosition(oppositeAxis.crossing);
		}

		internal double GetAxisProjectionAngle()
		{
			bool axisOnEdge;
			float marksZPosition = GetMarksZPosition(out axisOnEdge);
			float num = (float)GetAxisPosition();
			Point3D[] array = new Point3D[2];
			if (AxisPosition == AxisPosition.Top || AxisPosition == AxisPosition.Bottom)
			{
				array[0] = new Point3D(0f, num, marksZPosition);
				array[1] = new Point3D(100f, num, marksZPosition);
			}
			else
			{
				array[0] = new Point3D(num, 0f, marksZPosition);
				array[1] = new Point3D(num, 100f, marksZPosition);
			}
			chartArea.matrix3D.TransformPoints(array);
			array[0].X = (float)Math.Round(array[0].X, 4);
			array[0].Y = (float)Math.Round(array[0].Y, 4);
			array[1].X = (float)Math.Round(array[1].X, 4);
			array[1].Y = (float)Math.Round(array[1].Y, 4);
			double num2 = 0.0;
			num2 = ((AxisPosition != AxisPosition.Top && AxisPosition != AxisPosition.Bottom) ? Math.Atan((array[1].X - array[0].X) / (array[1].Y - array[0].Y)) : Math.Atan((array[1].Y - array[0].Y) / (array[1].X - array[0].X)));
			return num2 * 180.0 / Math.PI;
		}
	}
}
