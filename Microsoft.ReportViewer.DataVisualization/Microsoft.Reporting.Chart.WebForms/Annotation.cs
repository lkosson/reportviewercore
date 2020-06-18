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
	[SRDescription("DescriptionAttributeAnnotation_Annotation")]
	[DefaultProperty("Name")]
	internal abstract class Annotation : IMapAreaAttributes
	{
		private string name = string.Empty;

		private string clipToChartArea = "NotSet";

		private bool selected;

		private bool sizeAlwaysRelative = true;

		private object tag;

		internal Chart chart;

		private double x = double.NaN;

		private double y = double.NaN;

		private double width = double.NaN;

		private double height = double.NaN;

		private string axisXName = string.Empty;

		private string axisYName = string.Empty;

		private Axis axisX;

		private Axis axisY;

		private bool visible = true;

		private ContentAlignment alignment = ContentAlignment.MiddleCenter;

		private Color textColor = Color.Black;

		private Font textFont = new Font(ChartPicture.GetDefaultFontFamilyName(), 8f);

		private TextStyle textStyle;

		internal Color lineColor = Color.Black;

		private int lineWidth = 1;

		private ChartDashStyle lineStyle = ChartDashStyle.Solid;

		private Color backColor = Color.Empty;

		private ChartHatchStyle backHatchStyle;

		private GradientType backGradientType;

		private Color backGradientEndColor = Color.Empty;

		private Color shadowColor = Color.FromArgb(128, 0, 0, 0);

		private int shadowOffset;

		private string anchorDataPointName = string.Empty;

		private DataPoint anchorDataPoint;

		private DataPoint anchorDataPoint2;

		private double anchorX = double.NaN;

		private double anchorY = double.NaN;

		internal double anchorOffsetX;

		internal double anchorOffsetY;

		internal ContentAlignment anchorAlignment = ContentAlignment.BottomCenter;

		internal RectangleF[] selectionRects;

		internal bool outsideClipRegion;

		private string tooltip = string.Empty;

		internal const int selectionMarkerSize = 6;

		internal RectangleF currentPositionRel = new RectangleF(float.NaN, float.NaN, float.NaN, float.NaN);

		internal PointF currentAnchorLocationRel = new PointF(float.NaN, float.NaN);

		private AnnotationSmartLabelsStyle smartLabelsStyle;

		internal int currentPathPointIndex = -1;

		internal AnnotationGroup annotationGroup;

		private bool allowSelecting = true;

		private bool allowMoving = true;

		private bool allowAnchorMoving = true;

		private bool allowResizing = true;

		private bool allowTextEditing = true;

		private bool allowPathEditing = true;

		internal bool positionChanged;

		internal RectangleF startMovePositionRel = RectangleF.Empty;

		internal GraphicsPath startMovePathRel;

		internal PointF startMoveAnchorLocationRel = PointF.Empty;

		internal PointF lastPlacementPosition = PointF.Empty;

		private string href = string.Empty;

		private string mapAreaAttributes = string.Empty;

		private object mapAreaTag;

		[SRCategory("CategoryAttributeMisc")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeName4")]
		[ParenthesizePropertyName(true)]
		public virtual string Name
		{
			get
			{
				return name;
			}
			set
			{
				if (value != name)
				{
					if (value == null || value.Length == 0)
					{
						throw new ArgumentException(SR.ExceptionAnnotationNameIsEmpty);
					}
					if (chart != null && chart.Annotations.FindByName(value) != null)
					{
						throw new ArgumentException(SR.ExceptionAnnotationNameIsNotUnique(value));
					}
					name = value;
				}
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[Bindable(true)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRDescription("DescriptionAttributeAnnotation_AnnotationType")]
		public abstract string AnnotationType
		{
			get;
		}

		[SRCategory("CategoryAttributeMisc")]
		[DefaultValue("NotSet")]
		[SRDescription("DescriptionAttributeAnnotationClipToChartArea")]
		[TypeConverter(typeof(LegendAreaNameConverter))]
		public virtual string ClipToChartArea
		{
			get
			{
				return clipToChartArea;
			}
			set
			{
				clipToChartArea = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[Browsable(false)]
		[DefaultValue(null)]
		[SRDescription("DescriptionAttributeTag5")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public virtual object Tag
		{
			get
			{
				return tag;
			}
			set
			{
				tag = value;
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[Browsable(false)]
		[DefaultValue(null)]
		[SRDescription("DescriptionAttributeChart")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		internal virtual Chart Chart
		{
			get
			{
				return chart;
			}
			set
			{
				chart = value;
			}
		}

		[Browsable(true)]
		[SRCategory("CategoryAttributeMisc")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeSmartLabels")]
		public AnnotationSmartLabelsStyle SmartLabels
		{
			get
			{
				if (smartLabelsStyle == null)
				{
					smartLabelsStyle = new AnnotationSmartLabelsStyle(this);
				}
				return smartLabelsStyle;
			}
			set
			{
				value.chartElement = this;
				smartLabelsStyle = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributePosition")]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeSizeAlwaysRelative")]
		public virtual bool SizeAlwaysRelative
		{
			get
			{
				return sizeAlwaysRelative;
			}
			set
			{
				sizeAlwaysRelative = value;
				ResetCurrentRelativePosition();
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributePosition")]
		[DefaultValue(double.NaN)]
		[SRDescription("DescriptionAttributeAnnotationBaseX")]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(DoubleNanValueConverter))]
		public virtual double X
		{
			get
			{
				return x;
			}
			set
			{
				x = value;
				ResetCurrentRelativePosition();
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributePosition")]
		[DefaultValue(double.NaN)]
		[SRDescription("DescriptionAttributeAnnotationBaseY")]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(DoubleNanValueConverter))]
		public virtual double Y
		{
			get
			{
				return y;
			}
			set
			{
				y = value;
				ResetCurrentRelativePosition();
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributePosition")]
		[DefaultValue(double.NaN)]
		[SRDescription("DescriptionAttributeAnnotationWidth")]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(DoubleNanValueConverter))]
		public virtual double Width
		{
			get
			{
				return width;
			}
			set
			{
				width = value;
				ResetCurrentRelativePosition();
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributePosition")]
		[DefaultValue(double.NaN)]
		[SRDescription("DescriptionAttributeAnnotationHeight")]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(DoubleNanValueConverter))]
		public virtual double Height
		{
			get
			{
				return height;
			}
			set
			{
				height = value;
				ResetCurrentRelativePosition();
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributePosition")]
		[DefaultValue(double.NaN)]
		[SRDescription("DescriptionAttributeRight3")]
		[RefreshProperties(RefreshProperties.All)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[TypeConverter(typeof(DoubleNanValueConverter))]
		public virtual double Right
		{
			get
			{
				return x + width;
			}
			set
			{
				width = value - x;
				ResetCurrentRelativePosition();
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributePosition")]
		[DefaultValue(double.NaN)]
		[SRDescription("DescriptionAttributeBottom")]
		[RefreshProperties(RefreshProperties.All)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[TypeConverter(typeof(DoubleNanValueConverter))]
		public virtual double Bottom
		{
			get
			{
				return y + height;
			}
			set
			{
				height = value - y;
				ResetCurrentRelativePosition();
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(false)]
		[Browsable(false)]
		[SRDescription("DescriptionAttributeSelected")]
		public virtual bool Selected
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

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(SelectionPointsStyle.Rectangle)]
		[ParenthesizePropertyName(true)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRDescription("DescriptionAttributeSelectionPointsStyle3")]
		internal virtual SelectionPointsStyle SelectionPointsStyle => SelectionPointsStyle.Rectangle;

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeVisible6")]
		[ParenthesizePropertyName(true)]
		public virtual bool Visible
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

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(typeof(ContentAlignment), "MiddleCenter")]
		[SRDescription("DescriptionAttributeAlignment7")]
		public virtual ContentAlignment Alignment
		{
			get
			{
				return alignment;
			}
			set
			{
				alignment = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(typeof(Color), "Black")]
		[SRDescription("DescriptionAttributeTextColor6")]
		public virtual Color TextColor
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

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 8pt")]
		[SRDescription("DescriptionAttributeTextFont")]
		public virtual Font TextFont
		{
			get
			{
				return textFont;
			}
			set
			{
				textFont = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(typeof(TextStyle), "Default")]
		[SRDescription("DescriptionAttributeTextStyle3")]
		public virtual TextStyle TextStyle
		{
			get
			{
				return textStyle;
			}
			set
			{
				textStyle = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(typeof(Color), "Black")]
		[SRDescription("DescriptionAttributeLineColor3")]
		public virtual Color LineColor
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
		[DefaultValue(1)]
		[SRDescription("DescriptionAttributeLineWidth7")]
		public virtual int LineWidth
		{
			get
			{
				return lineWidth;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("value", SR.ExceptionAnnotationLineWidthIsNegative);
				}
				lineWidth = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(ChartDashStyle.Solid)]
		[SRDescription("DescriptionAttributeLineStyle6")]
		public virtual ChartDashStyle LineStyle
		{
			get
			{
				return lineStyle;
			}
			set
			{
				lineStyle = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeBackColor9")]
		[NotifyParentProperty(true)]
		public virtual Color BackColor
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

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(ChartHatchStyle.None)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeBackHatchStyle")]
		public virtual ChartHatchStyle BackHatchStyle
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

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(GradientType.None)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeBackGradientType12")]
		public virtual GradientType BackGradientType
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

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(typeof(Color), "")]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeBackGradientEndColor13")]
		public virtual Color BackGradientEndColor
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

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(typeof(Color), "128,0,0,0")]
		[SRDescription("DescriptionAttributeShadowColor4")]
		public virtual Color ShadowColor
		{
			get
			{
				return shadowColor;
			}
			set
			{
				shadowColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(0)]
		[SRDescription("DescriptionAttributeShadowOffset7")]
		public virtual int ShadowOffset
		{
			get
			{
				return shadowOffset;
			}
			set
			{
				shadowOffset = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAnchorAxes")]
		[DefaultValue("")]
		[Browsable(false)]
		[Bindable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRDescription("DescriptionAttributeAxisXName")]
		public virtual string AxisXName
		{
			get
			{
				if (axisXName.Length == 0 && axisX != null)
				{
					axisXName = GetAxisName(axisX);
				}
				return axisXName;
			}
			set
			{
				axisXName = value;
				axisX = null;
				ResetCurrentRelativePosition();
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAnchorAxes")]
		[Browsable(false)]
		[Bindable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeAxisYName")]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public virtual string AxisYName
		{
			get
			{
				return string.Empty;
			}
			set
			{
				YAxisName = value;
			}
		}

		[SRCategory("CategoryAttributeAnchorAxes")]
		[Browsable(false)]
		[Bindable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeAxisYName")]
		public virtual string YAxisName
		{
			get
			{
				if (axisYName.Length == 0 && axisY != null)
				{
					axisYName = GetAxisName(axisY);
				}
				return axisYName;
			}
			set
			{
				axisYName = value;
				axisY = null;
				ResetCurrentRelativePosition();
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAnchorAxes")]
		[DefaultValue(null)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRDescription("DescriptionAttributeAxisX")]
		[TypeConverter(typeof(AnnotationAxisValueConverter))]
		public virtual Axis AxisX
		{
			get
			{
				if (axisX == null && axisXName.Length > 0)
				{
					axisX = GetAxisByName(axisXName);
				}
				return axisX;
			}
			set
			{
				axisX = value;
				axisXName = string.Empty;
				ResetCurrentRelativePosition();
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAnchorAxes")]
		[DefaultValue(null)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRDescription("DescriptionAttributeAxisY")]
		[TypeConverter(typeof(AnnotationAxisValueConverter))]
		public virtual Axis AxisY
		{
			get
			{
				if (axisY == null && axisYName.Length > 0)
				{
					axisY = GetAxisByName(axisYName);
				}
				return axisY;
			}
			set
			{
				axisY = value;
				axisYName = string.Empty;
				ResetCurrentRelativePosition();
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAnchor")]
		[Browsable(false)]
		[Bindable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeAnchorDataPointName")]
		public virtual string AnchorDataPointName
		{
			get
			{
				if (anchorDataPointName.Length == 0 && anchorDataPoint != null)
				{
					anchorDataPointName = GetDataPointName(anchorDataPoint);
				}
				return anchorDataPointName;
			}
			set
			{
				anchorDataPointName = value;
				anchorDataPoint = null;
				ResetCurrentRelativePosition();
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAnchor")]
		[DefaultValue(null)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRDescription("DescriptionAttributeAnchorDataPoint")]
		[TypeConverter(typeof(AnchorPointValueConverter))]
		public virtual DataPoint AnchorDataPoint
		{
			get
			{
				if (anchorDataPoint == null && anchorDataPointName.Length > 0)
				{
					anchorDataPoint = GetDataPointByName(anchorDataPointName);
				}
				return anchorDataPoint;
			}
			set
			{
				anchorDataPoint = value;
				anchorDataPointName = string.Empty;
				ResetCurrentRelativePosition();
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAnchor")]
		[DefaultValue(double.NaN)]
		[SRDescription("DescriptionAttributeAnchorX")]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(DoubleNanValueConverter))]
		public virtual double AnchorX
		{
			get
			{
				return anchorX;
			}
			set
			{
				anchorX = value;
				ResetCurrentRelativePosition();
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAnchor")]
		[DefaultValue(double.NaN)]
		[SRDescription("DescriptionAttributeAnchorY")]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(DoubleNanValueConverter))]
		public virtual double AnchorY
		{
			get
			{
				return anchorY;
			}
			set
			{
				anchorY = value;
				ResetCurrentRelativePosition();
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAnchor")]
		[DefaultValue(0.0)]
		[SRDescription("DescriptionAttributeAnchorOffsetX3")]
		[RefreshProperties(RefreshProperties.All)]
		public virtual double AnchorOffsetX
		{
			get
			{
				return anchorOffsetX;
			}
			set
			{
				if (value > 100.0 || value < -100.0)
				{
					throw new ArgumentOutOfRangeException("value", SR.ExceptionAnnotationAnchorOffsetInvalid);
				}
				anchorOffsetX = value;
				ResetCurrentRelativePosition();
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAnchor")]
		[DefaultValue(0.0)]
		[SRDescription("DescriptionAttributeAnchorOffsetY3")]
		[RefreshProperties(RefreshProperties.All)]
		public virtual double AnchorOffsetY
		{
			get
			{
				return anchorOffsetY;
			}
			set
			{
				if (value > 100.0 || value < -100.0)
				{
					throw new ArgumentOutOfRangeException("value", SR.ExceptionAnnotationAnchorOffsetInvalid);
				}
				anchorOffsetY = value;
				ResetCurrentRelativePosition();
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAnchor")]
		[DefaultValue(typeof(ContentAlignment), "BottomCenter")]
		[SRDescription("DescriptionAttributeAnchorAlignment3")]
		public virtual ContentAlignment AnchorAlignment
		{
			get
			{
				return anchorAlignment;
			}
			set
			{
				anchorAlignment = value;
				ResetCurrentRelativePosition();
				Invalidate();
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryAttributeEditing")]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeAllowSelecting")]
		public virtual bool AllowSelecting
		{
			get
			{
				return allowSelecting;
			}
			set
			{
				allowSelecting = value;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryAttributeEditing")]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeAllowMoving")]
		public virtual bool AllowMoving
		{
			get
			{
				return allowMoving;
			}
			set
			{
				allowMoving = value;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryAttributeEditing")]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeAllowAnchorMoving3")]
		public virtual bool AllowAnchorMoving
		{
			get
			{
				return allowAnchorMoving;
			}
			set
			{
				allowAnchorMoving = value;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryAttributeEditing")]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeAllowResizing")]
		public virtual bool AllowResizing
		{
			get
			{
				return allowResizing;
			}
			set
			{
				allowResizing = value;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryAttributeEditing")]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeAllowTextEditing")]
		public virtual bool AllowTextEditing
		{
			get
			{
				return allowTextEditing;
			}
			set
			{
				allowTextEditing = value;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryAttributeEditing")]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeAllowPathEditing3")]
		public virtual bool AllowPathEditing
		{
			get
			{
				return allowPathEditing;
			}
			set
			{
				allowPathEditing = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[SRCategory("CategoryAttributeMisc")]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeToolTip4")]
		public virtual string ToolTip
		{
			get
			{
				return tooltip;
			}
			set
			{
				tooltip = value;
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeHref")]
		public virtual string Href
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

		[SRCategory("CategoryAttributeMisc")]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeMapAreaAttributes3")]
		public virtual string MapAreaAttributes
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

		string IMapAreaAttributes.ToolTip
		{
			get
			{
				return ToolTip;
			}
			set
			{
				ToolTip = value;
			}
		}

		string IMapAreaAttributes.Href
		{
			get
			{
				return Href;
			}
			set
			{
				Href = value;
			}
		}

		string IMapAreaAttributes.MapAreaAttributes
		{
			get
			{
				return MapAreaAttributes;
			}
			set
			{
				MapAreaAttributes = value;
			}
		}

		object IMapAreaAttributes.Tag
		{
			get
			{
				return mapAreaTag;
			}
			set
			{
				mapAreaTag = value;
			}
		}

		internal abstract void Paint(Chart chart, ChartGraphics graphics);

		internal virtual void PaintSelectionHandles(ChartGraphics chartGraphics, RectangleF rect, GraphicsPath path)
		{
			Color black = Color.Black;
			Color markerColor = Color.FromArgb(200, 255, 255, 255);
			MarkerStyle markerStyle = MarkerStyle.Square;
			int num = 6;
			bool flag = Selected;
			SizeF relativeSize = chartGraphics.GetRelativeSize(new SizeF(num, num));
			if (!chart.chartPicture.common.ProcessModePaint || chart.chartPicture.isPrinting)
			{
				return;
			}
			selectionRects = null;
			if (!flag)
			{
				return;
			}
			selectionRects = new RectangleF[9];
			if (SelectionPointsStyle == SelectionPointsStyle.TwoPoints)
			{
				selectionRects[0] = new RectangleF(rect.X - relativeSize.Width / 2f, rect.Y - relativeSize.Height / 2f, relativeSize.Width, relativeSize.Height);
				selectionRects[4] = new RectangleF(rect.Right - relativeSize.Width / 2f, rect.Bottom - relativeSize.Height / 2f, relativeSize.Width, relativeSize.Height);
				chartGraphics.DrawMarkerRel(rect.Location, markerStyle, num, markerColor, black, 1, "", Color.Empty, 0, Color.FromArgb(128, 0, 0, 0), RectangleF.Empty);
				chartGraphics.DrawMarkerRel(new PointF(rect.Right, rect.Bottom), markerStyle, num, markerColor, black, 1, "", Color.Empty, 0, Color.FromArgb(128, 0, 0, 0), RectangleF.Empty);
			}
			else if (SelectionPointsStyle == SelectionPointsStyle.Rectangle)
			{
				for (int i = 0; i < 8; i++)
				{
					PointF point = PointF.Empty;
					switch (i)
					{
					case 0:
						point = rect.Location;
						break;
					case 1:
						point = new PointF(rect.X + rect.Width / 2f, rect.Y);
						break;
					case 2:
						point = new PointF(rect.Right, rect.Y);
						break;
					case 3:
						point = new PointF(rect.Right, rect.Y + rect.Height / 2f);
						break;
					case 4:
						point = new PointF(rect.Right, rect.Bottom);
						break;
					case 5:
						point = new PointF(rect.X + rect.Width / 2f, rect.Bottom);
						break;
					case 6:
						point = new PointF(rect.X, rect.Bottom);
						break;
					case 7:
						point = new PointF(rect.X, rect.Y + rect.Height / 2f);
						break;
					}
					selectionRects[i] = new RectangleF(point.X - relativeSize.Width / 2f, point.Y - relativeSize.Height / 2f, relativeSize.Width, relativeSize.Height);
					chartGraphics.DrawMarkerRel(point, markerStyle, num, markerColor, black, 1, "", Color.Empty, 0, Color.FromArgb(128, 0, 0, 0), RectangleF.Empty);
				}
			}
			Axis vertAxis = null;
			Axis horizAxis = null;
			GetAxes(ref vertAxis, ref horizAxis);
			double num2 = double.NaN;
			double num3 = double.NaN;
			bool inRelativeAnchorX = false;
			bool inRelativeAnchorY = false;
			GetAnchorLocation(ref num2, ref num3, ref inRelativeAnchorX, ref inRelativeAnchorY);
			if (!double.IsNaN(num2) && !double.IsNaN(num3))
			{
				if (!inRelativeAnchorX && horizAxis != null)
				{
					num2 = horizAxis.ValueToPosition(num2);
				}
				if (!inRelativeAnchorY && vertAxis != null)
				{
					num3 = vertAxis.ValueToPosition(num3);
				}
				ChartArea chartArea = null;
				if (horizAxis != null && horizAxis.chartArea != null)
				{
					chartArea = horizAxis.chartArea;
				}
				if (vertAxis != null && vertAxis.chartArea != null)
				{
					chartArea = vertAxis.chartArea;
				}
				if (chartArea != null && chartArea.Area3DStyle.Enable3D && !chartArea.chartAreaIsCurcular && chartArea.requireAxes && chartArea.matrix3D.IsInitialized())
				{
					float positionZ = chartArea.areaSceneDepth;
					if (AnchorDataPoint != null && AnchorDataPoint.series != null)
					{
						float depth = 0f;
						chartArea.GetSeriesZPositionAndDepth(AnchorDataPoint.series, out depth, out positionZ);
						positionZ += depth / 2f;
					}
					Point3D[] array = new Point3D[1]
					{
						new Point3D((float)num2, (float)num3, positionZ)
					};
					chartArea.matrix3D.TransformPoints(array);
					num2 = array[0].X;
					num3 = array[0].Y;
				}
				selectionRects[8] = new RectangleF((float)num2 - relativeSize.Width / 2f, (float)num3 - relativeSize.Height / 2f, relativeSize.Width, relativeSize.Height);
				chartGraphics.DrawMarkerRel(new PointF((float)num2, (float)num3), MarkerStyle.Cross, 9, markerColor, black, 1, "", Color.Empty, 0, Color.FromArgb(128, 0, 0, 0), RectangleF.Empty);
			}
			if (path != null && AllowPathEditing)
			{
				PointF[] pathPoints = path.PathPoints;
				RectangleF[] array2 = new RectangleF[pathPoints.Length + 9];
				for (int j = 0; j < selectionRects.Length; j++)
				{
					array2[j] = selectionRects[j];
				}
				selectionRects = array2;
				for (int k = 0; k < pathPoints.Length; k++)
				{
					PointF relativePoint = chartGraphics.GetRelativePoint(pathPoints[k]);
					selectionRects[9 + k] = new RectangleF(relativePoint.X - relativeSize.Width / 2f, relativePoint.Y - relativeSize.Height / 2f, relativeSize.Width, relativeSize.Height);
					chartGraphics.DrawMarkerRel(relativePoint, MarkerStyle.Circle, 7, markerColor, black, 1, "", Color.Empty, 0, Color.FromArgb(128, 0, 0, 0), RectangleF.Empty);
				}
			}
		}

		public virtual void ResizeToContent()
		{
			RectangleF contentPosition = GetContentPosition();
			if (!double.IsNaN(contentPosition.Width))
			{
				Width = contentPosition.Width;
			}
			if (!double.IsNaN(contentPosition.Height))
			{
				Height = contentPosition.Height;
			}
		}

		internal virtual RectangleF GetContentPosition()
		{
			return new RectangleF(float.NaN, float.NaN, float.NaN, float.NaN);
		}

		private void GetAnchorLocation(ref double anchorX, ref double anchorY, ref bool inRelativeAnchorX, ref bool inRelativeAnchorY)
		{
			anchorX = AnchorX;
			anchorY = AnchorY;
			if (AnchorDataPoint == null || AnchorDataPoint.series == null || Chart == null || Chart.chartPicture == null)
			{
				return;
			}
			if (GetAnnotationGroup() != null)
			{
				throw new InvalidOperationException(SR.ExceptionAnnotationGroupedAnchorDataPointMustBeEmpty);
			}
			_ = PointF.Empty;
			if (double.IsNaN(anchorX) || double.IsNaN(anchorY))
			{
				if (double.IsNaN(anchorX))
				{
					anchorX = AnchorDataPoint.positionRel.X;
					inRelativeAnchorX = true;
				}
				if (double.IsNaN(anchorY))
				{
					anchorY = AnchorDataPoint.positionRel.Y;
					inRelativeAnchorY = true;
				}
			}
		}

		internal virtual void GetRelativePosition(out PointF location, out SizeF size, out PointF anchorLocation)
		{
			bool flag = true;
			if (!double.IsNaN(currentPositionRel.X) && !double.IsNaN(currentPositionRel.X))
			{
				location = currentPositionRel.Location;
				size = currentPositionRel.Size;
				anchorLocation = currentAnchorLocationRel;
				return;
			}
			Axis vertAxis = null;
			Axis horizAxis = null;
			GetAxes(ref vertAxis, ref horizAxis);
			if (anchorDataPoint != null && anchorDataPoint2 != null)
			{
				SizeAlwaysRelative = false;
				Height = vertAxis.PositionToValue(anchorDataPoint2.positionRel.Y, validateInput: false) - vertAxis.PositionToValue(anchorDataPoint.positionRel.Y, validateInput: false);
				Width = horizAxis.PositionToValue(anchorDataPoint2.positionRel.X, validateInput: false) - horizAxis.PositionToValue(anchorDataPoint.positionRel.X, validateInput: false);
				anchorDataPoint2 = null;
			}
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = sizeAlwaysRelative ? true : false;
			bool flag5 = sizeAlwaysRelative ? true : false;
			bool inRelativeAnchorX = false;
			bool inRelativeAnchorY = false;
			double num = AnchorX;
			double num2 = AnchorY;
			GetAnchorLocation(ref num, ref num2, ref inRelativeAnchorX, ref inRelativeAnchorY);
			AnnotationGroup annotationGroup = GetAnnotationGroup();
			PointF location2 = PointF.Empty;
			double num3 = 1.0;
			double num4 = 1.0;
			if (annotationGroup != null)
			{
				flag = false;
				SizeF size2 = SizeF.Empty;
				PointF anchorLocation2 = PointF.Empty;
				annotationGroup.GetRelativePosition(out location2, out size2, out anchorLocation2);
				num3 = (double)size2.Width / 100.0;
				num4 = (double)size2.Height / 100.0;
			}
			double num5 = width;
			double num6 = height;
			RectangleF contentPosition = GetContentPosition();
			if (double.IsNaN(num5))
			{
				num5 = contentPosition.Width;
				flag4 = true;
			}
			else
			{
				num5 *= num3;
			}
			if (double.IsNaN(num6))
			{
				num6 = contentPosition.Height;
				flag5 = true;
			}
			else
			{
				num6 *= num4;
			}
			if (Chart != null && Chart.IsDesignMode() && (SizeAlwaysRelative || (vertAxis == null && horizAxis == null)))
			{
				if (double.IsNaN(num5))
				{
					num5 = 20.0;
					flag = false;
				}
				if (double.IsNaN(num6))
				{
					num6 = 20.0;
					flag = false;
				}
			}
			double num7 = X;
			double num8 = Y;
			if (double.IsNaN(num8) && !double.IsNaN(num2))
			{
				flag3 = true;
				double num9 = num2;
				if (!inRelativeAnchorY && vertAxis != null)
				{
					num9 = vertAxis.ValueToPosition(num2);
				}
				if (AnchorAlignment == ContentAlignment.TopCenter || AnchorAlignment == ContentAlignment.TopLeft || AnchorAlignment == ContentAlignment.TopRight)
				{
					num8 = num9 + AnchorOffsetY;
					num8 *= num4;
				}
				else if (AnchorAlignment == ContentAlignment.BottomCenter || AnchorAlignment == ContentAlignment.BottomLeft || AnchorAlignment == ContentAlignment.BottomRight)
				{
					num8 = num9 - AnchorOffsetY;
					num8 *= num4;
					if (num6 != 0.0 && !double.IsNaN(num6))
					{
						if (flag5)
						{
							num8 -= num6;
						}
						else if (vertAxis != null)
						{
							float num10 = (float)vertAxis.PositionToValue(num8);
							float num11 = (float)vertAxis.ValueToPosition((double)num10 + num6);
							num8 -= (double)num11 - num8;
						}
					}
				}
				else
				{
					num8 = num9 + AnchorOffsetY;
					num8 *= num4;
					if (num6 != 0.0 && !double.IsNaN(num6))
					{
						if (flag5)
						{
							num8 -= num6 / 2.0;
						}
						else if (vertAxis != null)
						{
							float num12 = (float)vertAxis.PositionToValue(num8);
							float num13 = (float)vertAxis.ValueToPosition((double)num12 + num6);
							num8 -= ((double)num13 - num8) / 2.0;
						}
					}
				}
			}
			else
			{
				num8 *= num4;
			}
			if (double.IsNaN(num7) && !double.IsNaN(num))
			{
				flag2 = true;
				double num14 = num;
				if (!inRelativeAnchorX && horizAxis != null)
				{
					num14 = horizAxis.ValueToPosition(num);
				}
				if (AnchorAlignment == ContentAlignment.BottomLeft || AnchorAlignment == ContentAlignment.MiddleLeft || AnchorAlignment == ContentAlignment.TopLeft)
				{
					num7 = num14 + AnchorOffsetX;
					num7 *= num3;
				}
				else if (AnchorAlignment == ContentAlignment.BottomRight || AnchorAlignment == ContentAlignment.MiddleRight || AnchorAlignment == ContentAlignment.TopRight)
				{
					num7 = num14 - AnchorOffsetX;
					num7 *= num3;
					if (num5 != 0.0 && !double.IsNaN(num5))
					{
						if (flag4)
						{
							num7 -= num5;
						}
						else if (horizAxis != null)
						{
							float num15 = (float)horizAxis.PositionToValue(num7);
							num7 -= horizAxis.ValueToPosition((double)num15 + num5) - num7;
						}
					}
				}
				else
				{
					num7 = num14 + AnchorOffsetX;
					num7 *= num3;
					if (num5 != 0.0 && !double.IsNaN(num5))
					{
						if (flag4)
						{
							num7 -= num5 / 2.0;
						}
						else if (horizAxis != null)
						{
							float num16 = (float)horizAxis.PositionToValue(num7);
							num7 -= (horizAxis.ValueToPosition((double)num16 + num5) - num7) / 2.0;
						}
					}
				}
			}
			else
			{
				num7 *= num3;
			}
			num7 += (double)location2.X;
			num8 += (double)location2.Y;
			if (double.IsNaN(num7))
			{
				num7 = (double)contentPosition.X * num3;
				flag2 = true;
			}
			if (double.IsNaN(num8))
			{
				num8 = (double)contentPosition.Y * num4;
				flag3 = true;
			}
			if (horizAxis != null)
			{
				if (!flag2)
				{
					num7 = horizAxis.ValueToPosition(num7);
				}
				if (!inRelativeAnchorX)
				{
					num = horizAxis.ValueToPosition(num);
				}
				if (!flag4)
				{
					num5 = horizAxis.ValueToPosition(horizAxis.PositionToValue(num7, validateInput: false) + num5) - num7;
				}
			}
			if (vertAxis != null)
			{
				if (!flag3)
				{
					num8 = vertAxis.ValueToPosition(num8);
				}
				if (!inRelativeAnchorY)
				{
					num2 = vertAxis.ValueToPosition(num2);
				}
				if (!flag5)
				{
					num6 = vertAxis.ValueToPosition(vertAxis.PositionToValue(num8, validateInput: false) + num6) - num8;
				}
			}
			ChartArea chartArea = null;
			if (horizAxis != null && horizAxis.chartArea != null)
			{
				chartArea = horizAxis.chartArea;
			}
			if (vertAxis != null && vertAxis.chartArea != null)
			{
				chartArea = vertAxis.chartArea;
			}
			if (chartArea != null && chartArea.Area3DStyle.Enable3D && !chartArea.chartAreaIsCurcular && chartArea.requireAxes && chartArea.matrix3D.IsInitialized())
			{
				float positionZ = chartArea.areaSceneDepth;
				if (AnchorDataPoint != null && AnchorDataPoint.series != null)
				{
					float depth = 0f;
					chartArea.GetSeriesZPositionAndDepth(AnchorDataPoint.series, out depth, out positionZ);
					positionZ += depth / 2f;
				}
				Point3D[] array = new Point3D[3]
				{
					new Point3D((float)num7, (float)num8, positionZ),
					new Point3D((float)(num7 + num5), (float)(num8 + num6), positionZ),
					new Point3D((float)num, (float)num2, positionZ)
				};
				chartArea.matrix3D.TransformPoints(array);
				num7 = array[0].X;
				num8 = array[0].Y;
				num = array[2].X;
				num2 = array[2].Y;
				if (!(this is TextAnnotation) || !SizeAlwaysRelative)
				{
					num5 = (double)array[1].X - num7;
					num6 = (double)array[1].Y - num8;
				}
			}
			if (Chart != null && Chart.IsDesignMode())
			{
				if (double.IsNaN(num7))
				{
					num7 = location2.X;
					flag = false;
				}
				if (double.IsNaN(num8))
				{
					num8 = location2.Y;
					flag = false;
				}
				if (double.IsNaN(num5))
				{
					num5 = 20.0 * num3;
					flag = false;
				}
				if (double.IsNaN(num6))
				{
					num6 = 20.0 * num4;
					flag = false;
				}
			}
			location = new PointF((float)num7, (float)num8);
			size = new SizeF((float)num5, (float)num6);
			anchorLocation = new PointF((float)num, (float)num2);
			if (SmartLabels.Enabled && annotationGroup == null)
			{
				if (!double.IsNaN(num) && !double.IsNaN(num2) && double.IsNaN(X) && double.IsNaN(Y))
				{
					if (Chart != null && Chart.chartPicture != null)
					{
						double minMovingDistance = SmartLabels.MinMovingDistance;
						double maxMovingDistance = SmartLabels.MaxMovingDistance;
						PointF absolutePoint = GetGraphics().GetAbsolutePoint(new PointF((float)AnchorOffsetX, (float)AnchorOffsetY));
						float num17 = Math.Max(absolutePoint.X, absolutePoint.Y);
						if ((double)num17 > 0.0)
						{
							SmartLabels.MinMovingDistance += num17;
							SmartLabels.MaxMovingDistance += num17;
						}
						LabelAlignmentTypes labelAlignment = LabelAlignmentTypes.Bottom;
						StringFormat format = new StringFormat();
						SizeF markerSize = new SizeF((float)AnchorOffsetX, (float)AnchorOffsetY);
						PointF position = Chart.chartPicture.annotationSmartLabels.AdjustSmartLabelPosition(Chart.chartPicture.common, Chart.chartPicture.chartGraph, chartArea, SmartLabels, location, size, ref format, anchorLocation, markerSize, labelAlignment, this is CalloutAnnotation);
						SmartLabels.MinMovingDistance = minMovingDistance;
						SmartLabels.MaxMovingDistance = maxMovingDistance;
						if (position.IsEmpty)
						{
							location = new PointF(float.NaN, float.NaN);
						}
						else
						{
							location = Chart.chartPicture.annotationSmartLabels.GetLabelPosition(Chart.chartPicture.chartGraph, position, size, format, adjustForDrawing: false).Location;
						}
					}
				}
				else
				{
					StringFormat format2 = new StringFormat();
					Chart.chartPicture.annotationSmartLabels.AddSmartLabelPosition(Chart.chartPicture.chartGraph, chartArea, location, size, format2);
				}
			}
			if (flag)
			{
				currentPositionRel = new RectangleF(location, size);
				currentAnchorLocationRel = new PointF(anchorLocation.X, anchorLocation.Y);
			}
		}

		internal void SetPositionRelative(RectangleF position, PointF anchorPoint)
		{
			SetPositionRelative(position, anchorPoint, userInput: false);
		}

		internal void SetPositionRelative(RectangleF position, PointF anchorPoint, bool userInput)
		{
			double num = position.X;
			double num2 = position.Y;
			double num3 = position.Right;
			double num4 = position.Bottom;
			double num5 = position.Width;
			double num6 = position.Height;
			double num7 = anchorPoint.X;
			double num8 = anchorPoint.Y;
			currentPositionRel = new RectangleF(position.Location, position.Size);
			currentAnchorLocationRel = new PointF(anchorPoint.X, anchorPoint.Y);
			outsideClipRegion = false;
			RectangleF rectangleF = new RectangleF(0f, 0f, 100f, 100f);
			if (ClipToChartArea.Length > 0 && ClipToChartArea != "NotSet")
			{
				int index = chart.ChartAreas.GetIndex(ClipToChartArea);
				if (index >= 0)
				{
					rectangleF = chart.ChartAreas[index].PlotAreaPosition.ToRectangleF();
				}
			}
			if (position.X > rectangleF.Right || position.Y > rectangleF.Bottom || position.Right < rectangleF.X || position.Bottom < rectangleF.Y)
			{
				outsideClipRegion = true;
			}
			Axis vertAxis = null;
			Axis horizAxis = null;
			GetAxes(ref vertAxis, ref horizAxis);
			ChartArea chartArea = null;
			if (horizAxis != null && horizAxis.chartArea != null)
			{
				chartArea = horizAxis.chartArea;
			}
			if (vertAxis != null && vertAxis.chartArea != null)
			{
				chartArea = vertAxis.chartArea;
			}
			if (chartArea != null && chartArea.Area3DStyle.Enable3D)
			{
				if (AnchorDataPoint != null)
				{
					bool inRelativeAnchorX = true;
					bool inRelativeAnchorY = true;
					GetAnchorLocation(ref num7, ref num8, ref inRelativeAnchorX, ref inRelativeAnchorY);
					currentAnchorLocationRel = new PointF((float)num7, (float)num8);
				}
				AnchorDataPoint = null;
				AxisX = null;
				AxisY = null;
				horizAxis = null;
				vertAxis = null;
			}
			if (horizAxis != null)
			{
				num = horizAxis.PositionToValue(num, validateInput: false);
				if (!double.IsNaN(num7))
				{
					num7 = horizAxis.PositionToValue(num7, validateInput: false);
				}
				if (horizAxis.Logarithmic)
				{
					num = Math.Pow(horizAxis.logarithmBase, num);
					if (!double.IsNaN(num7))
					{
						num7 = Math.Pow(horizAxis.logarithmBase, num7);
					}
				}
				if (!SizeAlwaysRelative)
				{
					if (float.IsNaN(position.Right) && !float.IsNaN(position.Width) && !float.IsNaN(anchorPoint.X))
					{
						num3 = horizAxis.PositionToValue(anchorPoint.X + position.Width, validateInput: false);
						if (horizAxis.Logarithmic)
						{
							num3 = Math.Pow(horizAxis.logarithmBase, num3);
						}
						num5 = num3 - num7;
					}
					else
					{
						num3 = horizAxis.PositionToValue(position.Right, validateInput: false);
						if (horizAxis.Logarithmic)
						{
							num3 = Math.Pow(horizAxis.logarithmBase, num3);
						}
						num5 = num3 - num;
					}
				}
			}
			if (vertAxis != null)
			{
				num2 = vertAxis.PositionToValue(num2, validateInput: false);
				if (!double.IsNaN(num8))
				{
					num8 = vertAxis.PositionToValue(num8, validateInput: false);
				}
				if (vertAxis.Logarithmic)
				{
					num2 = Math.Pow(vertAxis.logarithmBase, num2);
					if (!double.IsNaN(num8))
					{
						num8 = Math.Pow(vertAxis.logarithmBase, num8);
					}
				}
				if (!SizeAlwaysRelative)
				{
					if (float.IsNaN(position.Bottom) && !float.IsNaN(position.Height) && !float.IsNaN(anchorPoint.Y))
					{
						num4 = vertAxis.PositionToValue(anchorPoint.Y + position.Height, validateInput: false);
						if (vertAxis.Logarithmic)
						{
							num4 = Math.Pow(vertAxis.logarithmBase, num4);
						}
						num6 = num4 - num8;
					}
					else
					{
						num4 = vertAxis.PositionToValue(position.Bottom, validateInput: false);
						if (vertAxis.Logarithmic)
						{
							num4 = Math.Pow(vertAxis.logarithmBase, num4);
						}
						num6 = num4 - num2;
					}
				}
			}
			X = num;
			Y = num2;
			Width = num5;
			Height = num6;
			AnchorX = num7;
			AnchorY = num8;
			Invalidate();
		}

		internal virtual void AdjustLocationSize(SizeF movingDistance, ResizingMode resizeMode)
		{
			AdjustLocationSize(movingDistance, resizeMode, pixelCoord: true);
		}

		internal virtual void AdjustLocationSize(SizeF movingDistance, ResizingMode resizeMode, bool pixelCoord)
		{
			AdjustLocationSize(movingDistance, resizeMode, pixelCoord, userInput: false);
		}

		internal virtual void AdjustLocationSize(SizeF movingDistance, ResizingMode resizeMode, bool pixelCoord, bool userInput)
		{
			if (movingDistance.IsEmpty)
			{
				return;
			}
			if (pixelCoord)
			{
				movingDistance = chart.chartPicture.chartGraph.GetRelativeSize(movingDistance);
			}
			PointF location = PointF.Empty;
			PointF anchorLocation = PointF.Empty;
			SizeF size = SizeF.Empty;
			if (userInput)
			{
				GetRelativePosition(out location, out size, out anchorLocation);
			}
			else
			{
				GetRelativePosition(out location, out size, out anchorLocation);
			}
			new PointF(location.X + size.Width, location.Y + size.Height);
			switch (resizeMode)
			{
			case ResizingMode.TopLeftHandle:
				location.X -= movingDistance.Width;
				location.Y -= movingDistance.Height;
				size.Width += movingDistance.Width;
				size.Height += movingDistance.Height;
				break;
			case ResizingMode.TopHandle:
				location.Y -= movingDistance.Height;
				size.Height += movingDistance.Height;
				break;
			case ResizingMode.TopRightHandle:
				location.Y -= movingDistance.Height;
				size.Width -= movingDistance.Width;
				size.Height += movingDistance.Height;
				break;
			case ResizingMode.RightHandle:
				size.Width -= movingDistance.Width;
				break;
			case ResizingMode.BottomRightHandle:
				size.Width -= movingDistance.Width;
				size.Height -= movingDistance.Height;
				break;
			case ResizingMode.BottomHandle:
				size.Height -= movingDistance.Height;
				break;
			case ResizingMode.BottomLeftHandle:
				location.X -= movingDistance.Width;
				size.Width += movingDistance.Width;
				size.Height -= movingDistance.Height;
				break;
			case ResizingMode.LeftHandle:
				location.X -= movingDistance.Width;
				size.Width += movingDistance.Width;
				break;
			case ResizingMode.AnchorHandle:
				anchorLocation.X -= movingDistance.Width;
				anchorLocation.Y -= movingDistance.Height;
				break;
			case ResizingMode.Moving:
				location.X -= movingDistance.Width;
				location.Y -= movingDistance.Height;
				break;
			}
			if (resizeMode == ResizingMode.Moving)
			{
				if (double.IsNaN(Width))
				{
					size.Width = float.NaN;
				}
				if (double.IsNaN(Height))
				{
					size.Height = float.NaN;
				}
			}
			if (resizeMode == ResizingMode.AnchorHandle)
			{
				if (double.IsNaN(X))
				{
					location.X = float.NaN;
				}
				if (double.IsNaN(Y))
				{
					location.Y = float.NaN;
				}
			}
			else if (double.IsNaN(AnchorX) || double.IsNaN(AnchorY))
			{
				anchorLocation = new PointF(float.NaN, float.NaN);
			}
			SetPositionRelative(new RectangleF(location, size), anchorLocation, userInput);
		}

		internal virtual bool IsAnchorDrawn()
		{
			return false;
		}

		internal DataPoint GetDataPointByName(string dataPointName)
		{
			DataPoint result = null;
			try
			{
				if (chart != null)
				{
					if (dataPointName.Length > 0)
					{
						int num = dataPointName.IndexOf("\\r", StringComparison.Ordinal);
						if (num > 0)
						{
							string parameter = dataPointName.Substring(0, num);
							string s = dataPointName.Substring(num + 2);
							result = chart.Series[parameter].Points[int.Parse(s, CultureInfo.InvariantCulture)];
							return result;
						}
						return result;
					}
					return result;
				}
				return result;
			}
			catch
			{
				return result;
			}
		}

		private Axis GetAxisByName(string axisName)
		{
			Axis result = null;
			try
			{
				if (chart != null)
				{
					if (axisName.Length > 0)
					{
						int num = axisName.IndexOf("\\r", StringComparison.Ordinal);
						if (num > 0)
						{
							string parameter = axisName.Substring(0, num);
							string value = axisName.Substring(num + 2);
							switch ((AxisName)Enum.Parse(typeof(AxisName), value))
							{
							case AxisName.X:
								result = chart.ChartAreas[parameter].AxisX;
								return result;
							case AxisName.Y:
								result = chart.ChartAreas[parameter].AxisY;
								return result;
							case AxisName.X2:
								result = chart.ChartAreas[parameter].AxisX2;
								return result;
							case AxisName.Y2:
								result = chart.ChartAreas[parameter].AxisY2;
								return result;
							default:
								return result;
							}
						}
						return result;
					}
					return result;
				}
				return result;
			}
			catch
			{
				return result;
			}
		}

		internal string GetDataPointName(DataPoint dataPoint)
		{
			string result = string.Empty;
			if (dataPoint.series != null)
			{
				int num = dataPoint.series.Points.IndexOf(dataPoint);
				if (num >= 0)
				{
					result = dataPoint.series.Name + "\\r" + num.ToString(CultureInfo.InvariantCulture);
				}
			}
			return result;
		}

		private string GetAxisName(Axis axis)
		{
			string result = string.Empty;
			if (axis.chartArea != null)
			{
				result = axis.chartArea.Name + "\\r" + axis.Type;
			}
			return result;
		}

		public virtual void SendToBack()
		{
			AnnotationCollection annotationCollection = null;
			if (chart != null)
			{
				annotationCollection = chart.Annotations;
			}
			AnnotationGroup annotationGroup = GetAnnotationGroup();
			if (annotationGroup != null)
			{
				annotationCollection = annotationGroup.Annotations;
			}
			if (annotationCollection != null)
			{
				Annotation annotation = annotationCollection.FindByName(Name);
				if (annotation != null)
				{
					annotationCollection.Remove(annotation);
					annotationCollection.Insert(0, annotation);
				}
			}
		}

		public virtual void BringToFront()
		{
			AnnotationCollection annotationCollection = null;
			if (chart != null)
			{
				annotationCollection = chart.Annotations;
			}
			AnnotationGroup annotationGroup = GetAnnotationGroup();
			if (annotationGroup != null)
			{
				annotationCollection = annotationGroup.Annotations;
			}
			if (annotationCollection != null)
			{
				Annotation annotation = annotationCollection.FindByName(Name);
				if (annotation != null)
				{
					annotationCollection.Remove(annotation);
					annotationCollection.Add(this);
				}
			}
		}

		public AnnotationGroup GetAnnotationGroup()
		{
			return annotationGroup;
		}

		internal void AddSmartLabelMarkerPositions(CommonElements common, ArrayList list)
		{
			if (!Visible || !IsAnchorDrawn())
			{
				return;
			}
			Axis vertAxis = null;
			Axis horizAxis = null;
			GetAxes(ref vertAxis, ref horizAxis);
			double num = double.NaN;
			double num2 = double.NaN;
			bool inRelativeAnchorX = false;
			bool inRelativeAnchorY = false;
			GetAnchorLocation(ref num, ref num2, ref inRelativeAnchorX, ref inRelativeAnchorY);
			if (double.IsNaN(num) || double.IsNaN(num2))
			{
				return;
			}
			if (!inRelativeAnchorX && horizAxis != null)
			{
				num = horizAxis.ValueToPosition(num);
			}
			if (!inRelativeAnchorY && vertAxis != null)
			{
				num2 = vertAxis.ValueToPosition(num2);
			}
			ChartArea chartArea = null;
			if (horizAxis != null && horizAxis.chartArea != null)
			{
				chartArea = horizAxis.chartArea;
			}
			if (vertAxis != null && vertAxis.chartArea != null)
			{
				chartArea = vertAxis.chartArea;
			}
			if (chartArea != null && chartArea.Area3DStyle.Enable3D && !chartArea.chartAreaIsCurcular && chartArea.requireAxes && chartArea.matrix3D.IsInitialized())
			{
				float positionZ = chartArea.areaSceneDepth;
				if (AnchorDataPoint != null && AnchorDataPoint.series != null)
				{
					float depth = 0f;
					chartArea.GetSeriesZPositionAndDepth(AnchorDataPoint.series, out depth, out positionZ);
					positionZ += depth / 2f;
				}
				Point3D[] array = new Point3D[1]
				{
					new Point3D((float)num, (float)num2, positionZ)
				};
				chartArea.matrix3D.TransformPoints(array);
				num = array[0].X;
				num2 = array[0].Y;
			}
			if (GetGraphics() != null)
			{
				SizeF relativeSize = GetGraphics().GetRelativeSize(new SizeF(1f, 1f));
				RectangleF rectangleF = new RectangleF((float)num - relativeSize.Width / 2f, (float)num2 - relativeSize.Height / 2f, relativeSize.Width, relativeSize.Height);
				list.Add(rectangleF);
			}
		}

		public void Anchor(DataPoint dataPoint)
		{
			Anchor(dataPoint, null);
		}

		public void Anchor(DataPoint dataPoint1, DataPoint dataPoint2)
		{
			X = double.NaN;
			Y = double.NaN;
			AnchorX = double.NaN;
			AnchorY = double.NaN;
			AnchorDataPoint = dataPoint1;
			Axis vertAxis = null;
			Axis horizAxis = null;
			GetAxes(ref vertAxis, ref horizAxis);
			if (dataPoint2 != null && dataPoint1 != null)
			{
				anchorDataPoint2 = dataPoint2;
			}
			Invalidate();
		}

		internal bool IsVisible()
		{
			if (Visible)
			{
				if (Chart != null)
				{
					ChartArea chartArea = null;
					if (AnchorDataPoint != null && AnchorDataPoint.series != null && Chart.ChartAreas.IndexOf(AnchorDataPoint.series.ChartArea) >= 0)
					{
						chartArea = Chart.ChartAreas[AnchorDataPoint.series.ChartArea];
					}
					if (chartArea == null && anchorDataPoint2 != null && anchorDataPoint2.series != null && Chart.ChartAreas.IndexOf(anchorDataPoint2.series.ChartArea) >= 0)
					{
						chartArea = Chart.ChartAreas[anchorDataPoint2.series.ChartArea];
					}
					if (chartArea == null && AxisX != null)
					{
						chartArea = AxisX.chartArea;
					}
					if (chartArea == null && AxisY != null)
					{
						chartArea = AxisY.chartArea;
					}
					if (chartArea != null && !chartArea.Visible)
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		internal void ResetCurrentRelativePosition()
		{
			currentPositionRel = new RectangleF(float.NaN, float.NaN, float.NaN, float.NaN);
			currentAnchorLocationRel = new PointF(float.NaN, float.NaN);
		}

		public void Delete()
		{
			if (Chart != null)
			{
				Chart.Annotations.Remove(this);
			}
		}

		internal string ReplaceKeywords(string strOriginal)
		{
			if (AnchorDataPoint != null)
			{
				return AnchorDataPoint.ReplaceKeywords(strOriginal);
			}
			return strOriginal;
		}

		internal bool IsAnchorVisible()
		{
			Axis vertAxis = null;
			Axis horizAxis = null;
			GetAxes(ref vertAxis, ref horizAxis);
			bool inRelativeAnchorX = false;
			bool inRelativeAnchorY = false;
			double num = AnchorX;
			double num2 = AnchorY;
			GetAnchorLocation(ref num, ref num2, ref inRelativeAnchorX, ref inRelativeAnchorY);
			if (!double.IsNaN(num) && !double.IsNaN(num2) && (AnchorDataPoint != null || AxisX != null || AxisY != null))
			{
				if (!inRelativeAnchorX && horizAxis != null)
				{
					num = horizAxis.ValueToPosition(num);
				}
				if (!inRelativeAnchorY && vertAxis != null)
				{
					num2 = vertAxis.ValueToPosition(num2);
				}
				ChartArea chartArea = null;
				if (horizAxis != null)
				{
					chartArea = horizAxis.chartArea;
				}
				if (chartArea == null && vertAxis != null)
				{
					chartArea = vertAxis.chartArea;
				}
				if (chartArea != null && chartArea.Area3DStyle.Enable3D && !chartArea.chartAreaIsCurcular && chartArea.requireAxes && chartArea.matrix3D.IsInitialized())
				{
					float positionZ = chartArea.areaSceneDepth;
					if (AnchorDataPoint != null && AnchorDataPoint.series != null)
					{
						float depth = 0f;
						chartArea.GetSeriesZPositionAndDepth(AnchorDataPoint.series, out depth, out positionZ);
						positionZ += depth / 2f;
					}
					Point3D[] array = new Point3D[1]
					{
						new Point3D((float)num, (float)num2, positionZ)
					};
					chartArea.matrix3D.TransformPoints(array);
					num = array[0].X;
					num2 = array[0].Y;
				}
				RectangleF rectangleF = chartArea.PlotAreaPosition.ToRectangleF();
				rectangleF.Inflate(1E-05f, 1E-05f);
				if (!rectangleF.Contains((float)num, (float)num2))
				{
					return false;
				}
			}
			return true;
		}

		internal ChartGraphics GetGraphics()
		{
			if (chart != null && chart.chartPicture != null && chart.chartPicture.common != null)
			{
				return chart.chartPicture.common.graph;
			}
			return null;
		}

		private Axis GetDataPointAxis(DataPoint dataPoint, AxisName axisName)
		{
			if (dataPoint != null && dataPoint.series != null && chart != null)
			{
				try
				{
					ChartArea chartArea = chart.ChartAreas[dataPoint.series.ChartArea];
					if ((axisName == AxisName.X || axisName == AxisName.X2) && !chartArea.switchValueAxes)
					{
						return chartArea.GetAxis(axisName, dataPoint.series.XAxisType, dataPoint.series.XSubAxisName);
					}
					return chartArea.GetAxis(axisName, dataPoint.series.YAxisType, dataPoint.series.YSubAxisName);
				}
				catch
				{
				}
			}
			return null;
		}

		internal void GetAxes(ref Axis vertAxis, ref Axis horizAxis)
		{
			vertAxis = null;
			horizAxis = null;
			if (AxisX != null && AxisX.chartArea != null)
			{
				if (AxisX.chartArea.switchValueAxes)
				{
					vertAxis = AxisX;
				}
				else
				{
					horizAxis = AxisX;
				}
			}
			if (AxisY != null && AxisY.chartArea != null)
			{
				if (AxisY.chartArea.switchValueAxes)
				{
					horizAxis = AxisY;
				}
				else
				{
					vertAxis = AxisY;
				}
			}
			if (AnchorDataPoint != null)
			{
				if (horizAxis == null)
				{
					horizAxis = GetDataPointAxis(AnchorDataPoint, AxisName.X);
					if (horizAxis != null && horizAxis.chartArea != null && horizAxis.chartArea.switchValueAxes)
					{
						horizAxis = GetDataPointAxis(AnchorDataPoint, AxisName.Y);
					}
				}
				if (vertAxis == null)
				{
					vertAxis = GetDataPointAxis(AnchorDataPoint, AxisName.Y);
					if (vertAxis != null && vertAxis.chartArea != null && vertAxis.chartArea.switchValueAxes)
					{
						vertAxis = GetDataPointAxis(AnchorDataPoint, AxisName.X);
					}
				}
			}
			if ((vertAxis != null || horizAxis != null) && GetAnnotationGroup() != null)
			{
				throw new InvalidOperationException(SR.ExceptionAnnotationGroupedAxisMustBeEmpty);
			}
		}

		internal void Invalidate()
		{
		}
	}
}
