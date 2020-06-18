using Microsoft.Reporting.Chart.WebForms.ChartTypes;
using Microsoft.Reporting.Chart.WebForms.Design;
using Microsoft.Reporting.Chart.WebForms.Utilities;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Microsoft.Reporting.Chart.WebForms
{
	[DefaultProperty("Axes")]
	[SRDescription("DescriptionAttributeChartArea_ChartArea")]
	internal class ChartArea : ChartArea3D
	{
		internal class ChartTypeAndSeriesInfo
		{
			internal string ChartType = string.Empty;

			internal Series Series;

			public ChartTypeAndSeriesInfo()
			{
			}

			public ChartTypeAndSeriesInfo(string chartType)
			{
				ChartType = chartType;
			}

			public ChartTypeAndSeriesInfo(Series series)
			{
				ChartType = series.ChartTypeName;
				Series = series;
			}
		}

		internal Chart chart;

		private Axis[] axisArray = new Axis[4];

		private Color backColor = Color.Empty;

		private ChartHatchStyle backHatchStyle;

		private string backImage = "";

		private ChartImageWrapMode backImageMode;

		private Color backImageTranspColor = Color.Empty;

		private ChartImageAlign backImageAlign;

		private GradientType backGradientType;

		private Color backGradientEndColor = Color.Empty;

		private Color borderColor = Color.Black;

		private int borderWidth = 1;

		private ChartDashStyle borderStyle;

		private int shadowOffset;

		private Color shadowColor = Color.FromArgb(128, 0, 0, 0);

		private ElementPosition areaPosition = new ElementPosition();

		private ElementPosition innerPlotPosition = new ElementPosition();

		private Cursor cursorX = new Cursor();

		private Cursor cursorY = new Cursor();

		internal int IterationCounter;

		private bool equallySizedAxesFont;

		internal float axesAutoFontSize = 8f;

		private string alignWithChartArea = "NotSet";

		private AreaAlignOrientations alignOrientation = AreaAlignOrientations.Vertical;

		private AreaAlignTypes alignType = AreaAlignTypes.All;

		private int circularSectorNumber = int.MinValue;

		private int circularUsePolygons = int.MinValue;

		internal bool alignmentInProcess;

		internal RectangleF originalAreaPosition = RectangleF.Empty;

		internal RectangleF originalInnerPlotPosition = RectangleF.Empty;

		internal PointF circularCenter = PointF.Empty;

		private ArrayList circularAxisList;

		internal SmartLabels smartLabels = new SmartLabels();

		private bool visible = true;

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[SRCategory("CategoryAttributeCursor")]
		[Bindable(true)]
		[DefaultValue(null)]
		[SRDescription("DescriptionAttributeChartArea_CursorX")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		public Cursor CursorX
		{
			get
			{
				return cursorX;
			}
			set
			{
				cursorX = value;
				cursorX.Initialize(this, AxisName.X);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[SRCategory("CategoryAttributeCursor")]
		[Bindable(true)]
		[DefaultValue(null)]
		[SRDescription("DescriptionAttributeChartArea_CursorY")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		public Cursor CursorY
		{
			get
			{
				return cursorY;
			}
			set
			{
				cursorY = value;
				cursorY.Initialize(this, AxisName.Y);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeChartArea_Visible")]
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
				Invalidate(invalidateAreaOnly: false);
			}
		}

		[SRCategory("CategoryAttributeAlignment")]
		[Bindable(true)]
		[DefaultValue("NotSet")]
		[SRDescription("DescriptionAttributeChartArea_AlignWithChartArea")]
		[TypeConverter(typeof(LegendAreaNameConverter))]
		public string AlignWithChartArea
		{
			get
			{
				return alignWithChartArea;
			}
			set
			{
				if (value.Length == 0)
				{
					alignWithChartArea = "NotSet";
				}
				else
				{
					alignWithChartArea = value;
				}
				Invalidate(invalidateAreaOnly: false);
			}
		}

		[SRCategory("CategoryAttributeAlignment")]
		[Bindable(true)]
		[DefaultValue(AreaAlignOrientations.Vertical)]
		[SRDescription("DescriptionAttributeChartArea_AlignOrientation")]
		public AreaAlignOrientations AlignOrientation
		{
			get
			{
				return alignOrientation;
			}
			set
			{
				alignOrientation = value;
				Invalidate(invalidateAreaOnly: false);
			}
		}

		[SRCategory("CategoryAttributeAlignment")]
		[Bindable(true)]
		[DefaultValue(AreaAlignTypes.All)]
		[SRDescription("DescriptionAttributeChartArea_AlignType")]
		public AreaAlignTypes AlignType
		{
			get
			{
				return alignType;
			}
			set
			{
				alignType = value;
				Invalidate(invalidateAreaOnly: false);
			}
		}

		[SRCategory("CategoryAttributeAxes")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeChartArea_Axes")]
		[TypeConverter(typeof(AxesArrayConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public Axis[] Axes
		{
			get
			{
				return axisArray;
			}
			set
			{
				AxisX = value[0];
				AxisY = value[1];
				AxisX2 = value[2];
				AxisY2 = value[3];
				Invalidate(invalidateAreaOnly: true);
			}
		}

		[SRCategory("CategoryAttributeAxis")]
		[Bindable(true)]
		[Browsable(false)]
		[SRDescription("DescriptionAttributeChartArea_AxisY")]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		public Axis AxisY
		{
			get
			{
				return axisY;
			}
			set
			{
				axisY = value;
				axisY.Initialize(this, AxisName.Y);
				axisArray[1] = axisY;
				Invalidate(invalidateAreaOnly: true);
			}
		}

		[SRCategory("CategoryAttributeAxis")]
		[Bindable(true)]
		[Browsable(false)]
		[SRDescription("DescriptionAttributeChartArea_AxisX")]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		public Axis AxisX
		{
			get
			{
				return axisX;
			}
			set
			{
				axisX = value;
				axisX.Initialize(this, AxisName.X);
				axisArray[0] = axisX;
				Invalidate(invalidateAreaOnly: true);
			}
		}

		[SRCategory("CategoryAttributeAxis")]
		[Bindable(true)]
		[Browsable(false)]
		[SRDescription("DescriptionAttributeChartArea_AxisX2")]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		public Axis AxisX2
		{
			get
			{
				return axisX2;
			}
			set
			{
				axisX2 = value;
				axisX2.Initialize(this, AxisName.X2);
				axisArray[2] = axisX2;
				Invalidate(invalidateAreaOnly: true);
			}
		}

		[SRCategory("CategoryAttributeAxis")]
		[Bindable(true)]
		[Browsable(false)]
		[SRDescription("DescriptionAttributeChartArea_AxisY2")]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		public Axis AxisY2
		{
			get
			{
				return axisY2;
			}
			set
			{
				axisY2 = value;
				axisY2.Initialize(this, AxisName.Y2);
				axisArray[3] = axisY2;
				Invalidate(invalidateAreaOnly: true);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeChartArea_Position")]
		[DefaultValue(typeof(ElementPosition), "Auto")]
		[NotifyParentProperty(true)]
		[TypeConverter(typeof(ElementPositionConverter))]
		[SerializationVisibility(SerializationVisibility.Element)]
		public ElementPosition Position
		{
			get
			{
				if (base.Common != null && base.Common.Chart != null && base.Common.Chart.serializationStatus == SerializationStatus.Saving)
				{
					if (areaPosition.Auto)
					{
						return new ElementPosition();
					}
					ElementPosition elementPosition = new ElementPosition();
					elementPosition.Auto = true;
					elementPosition.SetPositionNoAuto(areaPosition.X, areaPosition.Y, areaPosition.Width, areaPosition.Height);
					return elementPosition;
				}
				return areaPosition;
			}
			set
			{
				areaPosition = value;
				areaPosition.common = base.Common;
				areaPosition.resetAreaAutoPosition = true;
				Invalidate(invalidateAreaOnly: false);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeChartArea_InnerPlotPosition")]
		[DefaultValue(typeof(ElementPosition), "Auto")]
		[NotifyParentProperty(true)]
		[TypeConverter(typeof(ElementPositionConverter))]
		[SerializationVisibility(SerializationVisibility.Element)]
		public ElementPosition InnerPlotPosition
		{
			get
			{
				if (base.Common != null && base.Common.Chart != null && base.Common.Chart.serializationStatus == SerializationStatus.Saving)
				{
					if (innerPlotPosition.Auto)
					{
						return new ElementPosition();
					}
					ElementPosition elementPosition = new ElementPosition();
					elementPosition.Auto = true;
					elementPosition.SetPositionNoAuto(innerPlotPosition.X, innerPlotPosition.Y, innerPlotPosition.Width, innerPlotPosition.Height);
					return elementPosition;
				}
				return innerPlotPosition;
			}
			set
			{
				innerPlotPosition = value;
				innerPlotPosition.common = base.Common;
				Invalidate(invalidateAreaOnly: true);
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeChartArea_BackColor")]
		[NotifyParentProperty(true)]
		public Color BackColor
		{
			get
			{
				return backColor;
			}
			set
			{
				backColor = value;
				Invalidate(invalidateAreaOnly: true);
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(ChartHatchStyle.None)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeBackHatchStyle9")]
		public ChartHatchStyle BackHatchStyle
		{
			get
			{
				return backHatchStyle;
			}
			set
			{
				backHatchStyle = value;
				Invalidate(invalidateAreaOnly: true);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeBackImage15")]
		[NotifyParentProperty(true)]
		public string BackImage
		{
			get
			{
				return backImage;
			}
			set
			{
				backImage = value;
				Invalidate(invalidateAreaOnly: true);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(ChartImageWrapMode.Tile)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeChartArea_BackImageMode")]
		public ChartImageWrapMode BackImageMode
		{
			get
			{
				return backImageMode;
			}
			set
			{
				backImageMode = value;
				Invalidate(invalidateAreaOnly: true);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "")]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeBackImageTransparentColor")]
		public Color BackImageTransparentColor
		{
			get
			{
				return backImageTranspColor;
			}
			set
			{
				backImageTranspColor = value;
				Invalidate(invalidateAreaOnly: true);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(ChartImageAlign.TopLeft)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeBackImageAlign")]
		public ChartImageAlign BackImageAlign
		{
			get
			{
				return backImageAlign;
			}
			set
			{
				backImageAlign = value;
				Invalidate(invalidateAreaOnly: true);
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(GradientType.None)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeChartArea_BackGradientType")]
		public GradientType BackGradientType
		{
			get
			{
				return backGradientType;
			}
			set
			{
				backGradientType = value;
				Invalidate(invalidateAreaOnly: true);
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "")]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeChartArea_BackGradientEndColor")]
		public Color BackGradientEndColor
		{
			get
			{
				return backGradientEndColor;
			}
			set
			{
				backGradientEndColor = value;
				Invalidate(invalidateAreaOnly: true);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "128,0,0,0")]
		[SRDescription("DescriptionAttributeChartArea_ShadowColor")]
		[NotifyParentProperty(true)]
		public Color ShadowColor
		{
			get
			{
				return shadowColor;
			}
			set
			{
				shadowColor = value;
				Invalidate(invalidateAreaOnly: true);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(0)]
		[SRDescription("DescriptionAttributeChartArea_ShadowOffset")]
		[NotifyParentProperty(true)]
		public int ShadowOffset
		{
			get
			{
				return shadowOffset;
			}
			set
			{
				shadowOffset = value;
				Invalidate(invalidateAreaOnly: true);
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "Black")]
		[SRDescription("DescriptionAttributeChartArea_BorderColor")]
		[NotifyParentProperty(true)]
		public Color BorderColor
		{
			get
			{
				return borderColor;
			}
			set
			{
				borderColor = value;
				Invalidate(invalidateAreaOnly: true);
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(1)]
		[SRDescription("DescriptionAttributeChartArea_BorderWidth")]
		[NotifyParentProperty(true)]
		public int BorderWidth
		{
			get
			{
				return borderWidth;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("value", SR.ExceptionBorderWidthIsNegative);
				}
				borderWidth = value;
				Invalidate(invalidateAreaOnly: true);
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(ChartDashStyle.NotSet)]
		[SRDescription("DescriptionAttributeChartArea_BorderStyle")]
		[NotifyParentProperty(true)]
		public ChartDashStyle BorderStyle
		{
			get
			{
				return borderStyle;
			}
			set
			{
				borderStyle = value;
				Invalidate(invalidateAreaOnly: true);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[SRCategory("CategoryAttributeMisc")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeChartArea_Name")]
		[NotifyParentProperty(true)]
		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				if (!(name != value))
				{
					return;
				}
				if (base.Common != null)
				{
					foreach (ChartArea item in base.Common.chartAreaCollection)
					{
						if (item.Name == value)
						{
							throw new ArgumentException(SR.ExceptionChartAreaAlreadyExistsShort);
						}
					}
				}
				if (value == null || value.Length == 0)
				{
					throw new ArgumentException(SR.ExceptionChartAreaNameIsEmpty);
				}
				name = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(false)]
		[SRDescription("DescriptionAttributeChartArea_EquallySizedAxesFont")]
		[NotifyParentProperty(true)]
		public bool EquallySizedAxesFont
		{
			get
			{
				return equallySizedAxesFont;
			}
			set
			{
				equallySizedAxesFont = value;
				Invalidate(invalidateAreaOnly: true);
			}
		}

		public CategoryNodeCollection CategoryNodes
		{
			get;
			set;
		}

		internal bool CircularUsePolygons
		{
			get
			{
				if (circularUsePolygons == int.MinValue)
				{
					circularUsePolygons = 0;
					foreach (Series item in base.Common.DataManager.Series)
					{
						if (item.ChartArea == Name && item.IsVisible() && item.IsAttributeSet("AreaDrawingStyle"))
						{
							if (string.Compare(item["AreaDrawingStyle"], "Polygon", StringComparison.OrdinalIgnoreCase) == 0)
							{
								circularUsePolygons = 1;
								break;
							}
							if (string.Compare(item["AreaDrawingStyle"], "Circle", StringComparison.OrdinalIgnoreCase) == 0)
							{
								circularUsePolygons = 0;
								break;
							}
							throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid(item["AreaDrawingStyle"], "AreaDrawingStyle"));
						}
					}
				}
				return circularUsePolygons == 1;
			}
		}

		internal int CircularSectorsNumber
		{
			get
			{
				if (circularSectorNumber == int.MinValue)
				{
					circularSectorNumber = GetCircularSectorNumber();
				}
				return circularSectorNumber;
			}
		}

		protected bool ShouldSerializeAxes()
		{
			return false;
		}

		public ChartArea()
		{
			Initialize();
		}

		internal void Invalidate(bool invalidateAreaOnly)
		{
		}

		internal void Restore3DAnglesAndReverseMode()
		{
			if (base.Area3DStyle.Enable3D && !chartAreaIsCurcular)
			{
				AxisX.Reverse = oldReverseX;
				AxisX2.Reverse = oldReverseX;
				AxisY.Reverse = oldReverseY;
				AxisY2.Reverse = oldReverseY;
				base.Area3DStyle.YAngle = oldYAngle;
			}
		}

		internal void Set3DAnglesAndReverseMode()
		{
			reverseSeriesOrder = false;
			if (!base.Area3DStyle.Enable3D)
			{
				return;
			}
			AxisX2.Reverse = AxisX.Reverse;
			AxisY2.Reverse = AxisY.Reverse;
			oldReverseX = AxisX.Reverse;
			oldReverseY = AxisY.Reverse;
			oldYAngle = base.Area3DStyle.YAngle;
			if (base.Area3DStyle.YAngle > 90 || base.Area3DStyle.YAngle < -90)
			{
				reverseSeriesOrder = true;
				if (!switchValueAxes)
				{
					AxisX.Reverse = !AxisX.Reverse;
					AxisX2.Reverse = !AxisX2.Reverse;
				}
				else
				{
					AxisY.Reverse = !AxisY.Reverse;
					AxisY2.Reverse = !AxisY2.Reverse;
				}
				if (base.Area3DStyle.YAngle > 90)
				{
					base.Area3DStyle.YAngle = base.Area3DStyle.YAngle - 90 - 90;
				}
				else if (base.Area3DStyle.YAngle < -90)
				{
					base.Area3DStyle.YAngle = base.Area3DStyle.YAngle + 90 + 90;
				}
			}
		}

		internal void SetTempValues()
		{
			if (!Position.Auto)
			{
				originalAreaPosition = Position.ToRectangleF();
			}
			if (!InnerPlotPosition.Auto)
			{
				originalInnerPlotPosition = InnerPlotPosition.ToRectangleF();
			}
			circularSectorNumber = int.MinValue;
			circularUsePolygons = int.MinValue;
			circularAxisList = null;
			axisX.StoreAxisValues();
			axisX2.StoreAxisValues();
			axisY.StoreAxisValues();
			axisY2.StoreAxisValues();
		}

		internal void GetTempValues()
		{
			axisX.ResetAxisValues();
			axisX2.ResetAxisValues();
			axisY.ResetAxisValues();
			axisY2.ResetAxisValues();
			if (!originalAreaPosition.IsEmpty)
			{
				Position.SetPositionNoAuto(originalAreaPosition.X, originalAreaPosition.Y, originalAreaPosition.Width, originalAreaPosition.Height);
				originalAreaPosition = RectangleF.Empty;
			}
			if (!originalInnerPlotPosition.IsEmpty)
			{
				InnerPlotPosition.SetPositionNoAuto(originalInnerPlotPosition.X, originalInnerPlotPosition.Y, originalInnerPlotPosition.Width, originalInnerPlotPosition.Height);
				originalInnerPlotPosition = RectangleF.Empty;
			}
		}

		internal void Initialize()
		{
			axisY = new Axis();
			axisX = new Axis();
			axisX2 = new Axis();
			axisY2 = new Axis();
			axisX.Initialize(this, AxisName.X);
			axisY.Initialize(this, AxisName.Y);
			axisX2.Initialize(this, AxisName.X2);
			axisY2.Initialize(this, AxisName.Y2);
			axisArray[0] = axisX;
			axisArray[1] = axisY;
			axisArray[2] = axisX2;
			axisArray[3] = axisY2;
			areaPosition.resetAreaAutoPosition = true;
			if (base.PlotAreaPosition == null)
			{
				base.PlotAreaPosition = new ElementPosition();
			}
			cursorX.Initialize(this, AxisName.X);
			cursorY.Initialize(this, AxisName.Y);
		}

		internal void ResetMinMaxFromData()
		{
			axisArray[0].refreshMinMaxFromData = true;
			axisArray[1].refreshMinMaxFromData = true;
			axisArray[2].refreshMinMaxFromData = true;
			axisArray[3].refreshMinMaxFromData = true;
		}

		public void Recalculate()
		{
			ResetMinMaxFromData();
			axisArray[0].ReCalc(base.PlotAreaPosition);
			axisArray[1].ReCalc(base.PlotAreaPosition);
			axisArray[2].ReCalc(base.PlotAreaPosition);
			axisArray[3].ReCalc(base.PlotAreaPosition);
			SetData();
		}

		internal void ReCalcInternal()
		{
			axisArray[0].ReCalc(base.PlotAreaPosition);
			axisArray[1].ReCalc(base.PlotAreaPosition);
			axisArray[2].ReCalc(base.PlotAreaPosition);
			axisArray[3].ReCalc(base.PlotAreaPosition);
			SetData();
		}

		internal void ResetAutoValues()
		{
			axisArray[0].ResetAutoValues();
			axisArray[1].ResetAutoValues();
			axisArray[2].ResetAutoValues();
			axisArray[3].ResetAutoValues();
		}

		internal RectangleF GetBackgroundPosition(bool withScrollBars)
		{
			RectangleF result = base.PlotAreaPosition.ToRectangleF();
			if (!requireAxes)
			{
				result = Position.ToRectangleF();
			}
			if (!withScrollBars)
			{
				return result;
			}
			RectangleF result2 = new RectangleF(result.Location, result.Size);
			if (requireAxes)
			{
				Axis[] axes = Axes;
				foreach (Axis axis in axes)
				{
					if (axis.ScrollBar.IsVisible() && axis.ScrollBar.PositionInside)
					{
						if (axis.AxisPosition == AxisPosition.Bottom)
						{
							result2.Height += (float)axis.ScrollBar.GetScrollBarRelativeSize();
						}
						else if (axis.AxisPosition == AxisPosition.Top)
						{
							result2.Y -= (float)axis.ScrollBar.GetScrollBarRelativeSize();
							result2.Height += (float)axis.ScrollBar.GetScrollBarRelativeSize();
						}
						else if (axis.AxisPosition == AxisPosition.Left)
						{
							result2.X -= (float)axis.ScrollBar.GetScrollBarRelativeSize();
							result2.Width += (float)axis.ScrollBar.GetScrollBarRelativeSize();
						}
						else if (axis.AxisPosition == AxisPosition.Left)
						{
							result2.Width += (float)axis.ScrollBar.GetScrollBarRelativeSize();
						}
					}
				}
			}
			return result2;
		}

		internal void SetCommon(CommonElements common)
		{
			base.Common = common;
			axisX.Common = common;
			axisX2.Common = common;
			axisY.Common = common;
			axisY2.Common = common;
			areaPosition.common = common;
			innerPlotPosition.common = common;
			if (common != null)
			{
				chart = common.Chart;
			}
			axisX.Initialize(this, AxisName.X);
			axisY.Initialize(this, AxisName.Y);
			axisX2.Initialize(this, AxisName.X2);
			axisY2.Initialize(this, AxisName.Y2);
			cursorX.Initialize(this, AxisName.X);
			cursorY.Initialize(this, AxisName.Y);
		}

		internal void Resize(ChartGraphics chartGraph)
		{
			RectangleF plotArea = Position.ToRectangleF();
			if (!InnerPlotPosition.Auto)
			{
				plotArea.X += Position.Width / 100f * InnerPlotPosition.X;
				plotArea.Y += Position.Height / 100f * InnerPlotPosition.Y;
				plotArea.Width = Position.Width / 100f * InnerPlotPosition.Width;
				plotArea.Height = Position.Height / 100f * InnerPlotPosition.Height;
			}
			int num = 0;
			int num2 = 0;
			Axis[] axes = Axes;
			foreach (Axis axis in axes)
			{
				if (axis.enabled)
				{
					if (axis.AxisPosition == AxisPosition.Bottom)
					{
						num2++;
					}
					else if (axis.AxisPosition == AxisPosition.Top)
					{
						num2++;
					}
					else if (axis.AxisPosition == AxisPosition.Left)
					{
						num++;
					}
					else if (axis.AxisPosition == AxisPosition.Right)
					{
						num++;
					}
				}
			}
			if (num2 <= 0)
			{
				num2 = 1;
			}
			if (num <= 0)
			{
				num = 1;
			}
			Axis[] array = (!switchValueAxes) ? new Axis[4]
			{
				AxisY,
				AxisY2,
				AxisX,
				AxisX2
			} : new Axis[4]
			{
				AxisX,
				AxisX2,
				AxisY,
				AxisY2
			};
			if (EquallySizedAxesFont)
			{
				axesAutoFontSize = 20f;
				axes = array;
				foreach (Axis axis2 in axes)
				{
					if (axis2.enabled)
					{
						if (axis2.AxisPosition == AxisPosition.Bottom || axis2.AxisPosition == AxisPosition.Top)
						{
							axis2.Resize(chartGraph, base.PlotAreaPosition, plotArea, num2, InnerPlotPosition.Auto);
						}
						else
						{
							axis2.Resize(chartGraph, base.PlotAreaPosition, plotArea, num, InnerPlotPosition.Auto);
						}
						if (axis2.LabelsAutoFit && axis2.autoLabelFont != null)
						{
							axesAutoFontSize = Math.Min(axesAutoFontSize, axis2.autoLabelFont.Size);
						}
					}
				}
			}
			RectangleF empty = RectangleF.Empty;
			axes = array;
			foreach (Axis axis3 in axes)
			{
				if (!axis3.enabled)
				{
					if (InnerPlotPosition.Auto && base.Area3DStyle.Enable3D && !chartAreaIsCurcular)
					{
						SizeF relativeSize = chartGraph.GetRelativeSize(new SizeF(base.Area3DStyle.WallWidth, base.Area3DStyle.WallWidth));
						if (axis3.AxisPosition == AxisPosition.Bottom)
						{
							plotArea.Height -= relativeSize.Height;
						}
						else if (axis3.AxisPosition == AxisPosition.Top)
						{
							plotArea.Y += relativeSize.Height;
							plotArea.Height -= relativeSize.Height;
						}
						else if (axis3.AxisPosition == AxisPosition.Right)
						{
							plotArea.Width -= relativeSize.Width;
						}
						else if (axis3.AxisPosition == AxisPosition.Left)
						{
							plotArea.X += relativeSize.Width;
							plotArea.Width -= relativeSize.Width;
						}
					}
					continue;
				}
				if (axis3.AxisPosition == AxisPosition.Bottom || axis3.AxisPosition == AxisPosition.Top)
				{
					axis3.Resize(chartGraph, base.PlotAreaPosition, plotArea, num2, InnerPlotPosition.Auto);
				}
				else
				{
					axis3.Resize(chartGraph, base.PlotAreaPosition, plotArea, num, InnerPlotPosition.Auto);
				}
				PreventTopBottomAxesLabelsOverlapping(axis3);
				float num3 = (float)axis3.GetAxisPosition();
				if (axis3.AxisPosition == AxisPosition.Bottom)
				{
					if (!axis3.IsMarksNextToAxis())
					{
						num3 = plotArea.Bottom;
					}
					num3 = plotArea.Bottom - num3;
				}
				else if (axis3.AxisPosition == AxisPosition.Top)
				{
					if (!axis3.IsMarksNextToAxis())
					{
						num3 = plotArea.Y;
					}
					num3 -= plotArea.Top;
				}
				else if (axis3.AxisPosition == AxisPosition.Right)
				{
					if (!axis3.IsMarksNextToAxis())
					{
						num3 = plotArea.Right;
					}
					num3 = plotArea.Right - num3;
				}
				else if (axis3.AxisPosition == AxisPosition.Left)
				{
					if (!axis3.IsMarksNextToAxis())
					{
						num3 = plotArea.X;
					}
					num3 -= plotArea.Left;
				}
				float num4 = axis3.markSize + axis3.labelSize;
				num4 -= num3;
				if (num4 < 0f)
				{
					num4 = 0f;
				}
				num4 += axis3.titleSize + axis3.scrollBarSize;
				if (chartAreaIsCurcular && (axis3.AxisPosition == AxisPosition.Top || axis3.AxisPosition == AxisPosition.Bottom))
				{
					num4 = axis3.titleSize + axis3.markSize + axis3.scrollBarSize;
				}
				if (!InnerPlotPosition.Auto)
				{
					continue;
				}
				if (axis3.AxisPosition == AxisPosition.Bottom)
				{
					plotArea.Height -= num4;
				}
				else if (axis3.AxisPosition == AxisPosition.Top)
				{
					plotArea.Y += num4;
					plotArea.Height -= num4;
				}
				else if (axis3.AxisPosition == AxisPosition.Left)
				{
					plotArea.X += num4;
					plotArea.Width -= num4;
				}
				else if (axis3.AxisPosition == AxisPosition.Right)
				{
					plotArea.Width -= num4;
				}
				if (1 == 0)
				{
					continue;
				}
				if (axis3.AxisPosition == AxisPosition.Bottom || axis3.AxisPosition == AxisPosition.Top)
				{
					if (axis3.labelNearOffset != 0f && axis3.labelNearOffset < Position.X)
					{
						float num5 = Position.X - axis3.labelNearOffset;
						if (Math.Abs(num5) > plotArea.Width * 0.3f)
						{
							num5 = plotArea.Width * 0.3f;
						}
						empty.X = Math.Max(num5, empty.X);
					}
					if (axis3.labelFarOffset > Position.Right())
					{
						if (axis3.labelFarOffset - Position.Right() < plotArea.Width * 0.3f)
						{
							empty.Width = Math.Max(axis3.labelFarOffset - Position.Right(), empty.Width);
						}
						else
						{
							empty.Width = Math.Max(plotArea.Width * 0.3f, empty.Width);
						}
					}
					continue;
				}
				if (axis3.labelNearOffset != 0f && axis3.labelNearOffset < Position.Y)
				{
					float num6 = Position.Y - axis3.labelNearOffset;
					if (Math.Abs(num6) > plotArea.Height * 0.3f)
					{
						num6 = plotArea.Height * 0.3f;
					}
					empty.Y = Math.Max(num6, empty.Y);
				}
				if (axis3.labelFarOffset > Position.Bottom())
				{
					if (axis3.labelFarOffset - Position.Bottom() < plotArea.Height * 0.3f)
					{
						empty.Height = Math.Max(axis3.labelFarOffset - Position.Bottom(), empty.Height);
					}
					else
					{
						empty.Height = Math.Max(plotArea.Height * 0.3f, empty.Height);
					}
				}
			}
			if (!chartAreaIsCurcular)
			{
				try
				{
					if (empty.Y > 0f && empty.Y > plotArea.Y - Position.Y)
					{
						float num7 = plotArea.Y - Position.Y - empty.Y;
						plotArea.Y -= num7;
						plotArea.Height += num7;
					}
					if (empty.X > 0f && empty.X > plotArea.X - Position.X)
					{
						float num8 = plotArea.X - Position.X - empty.X;
						plotArea.X -= num8;
						plotArea.Width += num8;
					}
					if (empty.Height > 0f && empty.Height > Position.Bottom() - plotArea.Bottom)
					{
						plotArea.Height += Position.Bottom() - plotArea.Bottom - empty.Height;
					}
					if (empty.Width > 0f && empty.Width > Position.Right() - plotArea.Right)
					{
						plotArea.Width += Position.Right() - plotArea.Right - empty.Width;
					}
				}
				catch (Exception)
				{
				}
			}
			if (chartAreaIsCurcular)
			{
				float num9 = Math.Max(AxisY.titleSize, AxisY2.titleSize);
				if (num9 > 0f)
				{
					plotArea.X += num9;
					plotArea.Width -= 2f * num9;
				}
				float num10 = Math.Max(AxisX.titleSize, AxisX2.titleSize);
				if (num10 > 0f)
				{
					plotArea.Y += num10;
					plotArea.Height -= 2f * num10;
				}
				RectangleF absoluteRectangle = chartGraph.GetAbsoluteRectangle(plotArea);
				if (absoluteRectangle.Width > absoluteRectangle.Height)
				{
					absoluteRectangle.X += (absoluteRectangle.Width - absoluteRectangle.Height) / 2f;
					absoluteRectangle.Width = absoluteRectangle.Height;
				}
				else
				{
					absoluteRectangle.Y += (absoluteRectangle.Height - absoluteRectangle.Width) / 2f;
					absoluteRectangle.Height = absoluteRectangle.Width;
				}
				plotArea = chartGraph.GetRelativeRectangle(absoluteRectangle);
				circularCenter = new PointF(plotArea.X + plotArea.Width / 2f, plotArea.Y + plotArea.Height / 2f);
				FitCircularLabels(chartGraph, base.PlotAreaPosition, ref plotArea, num9, num10);
			}
			if (plotArea.Width < 0f)
			{
				plotArea.Width = 0f;
			}
			if (plotArea.Height < 0f)
			{
				plotArea.Height = 0f;
			}
			base.PlotAreaPosition.FromRectangleF(plotArea);
			InnerPlotPosition.SetPositionNoAuto((float)Math.Round((plotArea.X - Position.X) / (Position.Width / 100f), 5), (float)Math.Round((plotArea.Y - Position.Y) / (Position.Height / 100f), 5), (float)Math.Round(plotArea.Width / (Position.Width / 100f), 5), (float)Math.Round(plotArea.Height / (Position.Height / 100f), 5));
			AxisY2.AdjustLabelFontAtSecondPass(chartGraph, InnerPlotPosition.Auto);
			AxisY.AdjustLabelFontAtSecondPass(chartGraph, InnerPlotPosition.Auto);
			if (InnerPlotPosition.Auto)
			{
				AxisX2.AdjustLabelFontAtSecondPass(chartGraph, InnerPlotPosition.Auto);
				AxisX.AdjustLabelFontAtSecondPass(chartGraph, InnerPlotPosition.Auto);
			}
		}

		private Axis FindAxis(AxisPosition axisPosition)
		{
			Axis[] axes = Axes;
			foreach (Axis axis in axes)
			{
				if (axis.AxisPosition == axisPosition)
				{
					return axis;
				}
			}
			return null;
		}

		private void PreventTopBottomAxesLabelsOverlapping(Axis axis)
		{
			if (!axis.IsAxisOnAreaEdge())
			{
				return;
			}
			if (axis.AxisPosition == AxisPosition.Bottom)
			{
				float num = (float)axis.GetAxisPosition();
				if (!axis.IsMarksNextToAxis())
				{
					num = axis.PlotAreaPosition.Bottom();
				}
				if (Math.Round(num, 2) < Math.Round(axis.PlotAreaPosition.Bottom(), 2))
				{
					return;
				}
				Axis axis2 = FindAxis(AxisPosition.Left);
				if (axis2 != null && axis2.enabled && axis2.labelFarOffset != 0f && axis2.labelFarOffset > num && axis.labelNearOffset != 0f && axis.labelNearOffset < base.PlotAreaPosition.X)
				{
					float num2 = (axis2.labelFarOffset - num) * 0.75f;
					if (num2 > axis.markSize)
					{
						axis.markSize += num2 - axis.markSize;
					}
				}
				Axis axis3 = FindAxis(AxisPosition.Right);
				if (axis3 != null && axis3.enabled && axis3.labelFarOffset != 0f && axis3.labelFarOffset > num && axis.labelFarOffset != 0f && axis.labelFarOffset > base.PlotAreaPosition.Right())
				{
					float num3 = (axis3.labelFarOffset - num) * 0.75f;
					if (num3 > axis.markSize)
					{
						axis.markSize += num3 - axis.markSize;
					}
				}
			}
			else
			{
				if (axis.AxisPosition != AxisPosition.Top)
				{
					return;
				}
				float num4 = (float)axis.GetAxisPosition();
				if (!axis.IsMarksNextToAxis())
				{
					num4 = axis.PlotAreaPosition.Y;
				}
				if (Math.Round(num4, 2) < Math.Round(axis.PlotAreaPosition.Y, 2))
				{
					return;
				}
				Axis axis4 = FindAxis(AxisPosition.Left);
				if (axis4 != null && axis4.enabled && axis4.labelNearOffset != 0f && axis4.labelNearOffset < num4 && axis.labelNearOffset != 0f && axis.labelNearOffset < base.PlotAreaPosition.X)
				{
					float num5 = (num4 - axis4.labelNearOffset) * 0.75f;
					if (num5 > axis.markSize)
					{
						axis.markSize += num5 - axis.markSize;
					}
				}
				Axis axis5 = FindAxis(AxisPosition.Right);
				if (axis5 != null && axis5.enabled && axis5.labelNearOffset != 0f && axis5.labelNearOffset < num4 && axis.labelFarOffset != 0f && axis.labelFarOffset > base.PlotAreaPosition.Right())
				{
					float num6 = (num4 - axis5.labelNearOffset) * 0.75f;
					if (num6 > axis.markSize)
					{
						axis.markSize += num6 - axis.markSize;
					}
				}
			}
		}

		private void PaintAreaBack(ChartGraphics graph, RectangleF position, bool borderOnly)
		{
			if (!borderOnly)
			{
				if (!base.Area3DStyle.Enable3D || !requireAxes || chartAreaIsCurcular)
				{
					graph.FillRectangleRel(position, BackColor, BackHatchStyle, BackImage, BackImageMode, BackImageTransparentColor, BackImageAlign, BackGradientType, BackGradientEndColor, requireAxes ? Color.Empty : BorderColor, (!requireAxes) ? BorderWidth : 0, BorderStyle, ShadowColor, ShadowOffset, PenAlignment.Outset, chartAreaIsCurcular, (chartAreaIsCurcular && CircularUsePolygons) ? CircularSectorsNumber : 0, base.Area3DStyle.Enable3D);
				}
				else
				{
					DrawArea3DScene(graph, position);
				}
			}
			else if ((!base.Area3DStyle.Enable3D || !requireAxes || chartAreaIsCurcular) && BorderColor != Color.Empty && BorderWidth > 0)
			{
				graph.FillRectangleRel(position, Color.Transparent, ChartHatchStyle.None, "", ChartImageWrapMode.Tile, Color.Empty, ChartImageAlign.Center, GradientType.None, Color.Empty, BorderColor, BorderWidth, BorderStyle, Color.Empty, 0, PenAlignment.Outset, chartAreaIsCurcular, (chartAreaIsCurcular && CircularUsePolygons) ? CircularSectorsNumber : 0, base.Area3DStyle.Enable3D);
			}
		}

		internal void Paint(ChartGraphics graph)
		{
			if (base.PlotAreaPosition.Width == 0f && base.PlotAreaPosition.Height == 0f && !InnerPlotPosition.Auto && !Position.Auto)
			{
				RectangleF rect = Position.ToRectangleF();
				if (!InnerPlotPosition.Auto)
				{
					rect.X += Position.Width / 100f * InnerPlotPosition.X;
					rect.Y += Position.Height / 100f * InnerPlotPosition.Y;
					rect.Width = Position.Width / 100f * InnerPlotPosition.Width;
					rect.Height = Position.Height / 100f * InnerPlotPosition.Height;
				}
				base.PlotAreaPosition.FromRectangleF(rect);
			}
			RectangleF backgroundPosition = GetBackgroundPosition(withScrollBars: true);
			RectangleF backgroundPosition2 = GetBackgroundPosition(withScrollBars: false);
			if (base.Common.ProcessModeRegions)
			{
				base.Common.HotRegionsList.AddHotRegion(backgroundPosition2, this, ChartElementType.PlottingArea, relativeCoordinates: true);
			}
			graph.StartAnimation();
			PaintAreaBack(graph, backgroundPosition, borderOnly: false);
			graph.StopAnimation();
			base.Common.EventsManager.OnBackPaint(this, new ChartPaintEventArgs(graph, base.Common, base.PlotAreaPosition));
			if (!requireAxes && base.ChartTypes.Count != 0)
			{
				for (int i = 0; i < base.ChartTypes.Count; i++)
				{
					IChartType chartType = base.Common.ChartTypeRegistry.GetChartType((string)base.ChartTypes[i]);
					if (!chartType.RequireAxes)
					{
						chartType.Paint(graph, base.Common, this, null);
						break;
					}
				}
				base.Common.EventsManager.OnPaint(this, new ChartPaintEventArgs(graph, base.Common, base.PlotAreaPosition));
				return;
			}
			smartLabels.Reset(base.Common, this);
			Axis[] array = axisArray;
			foreach (Axis axis in array)
			{
				axis.optimizedGetPosition = true;
				axis.paintViewMax = axis.GetViewMaximum();
				axis.paintViewMin = axis.GetViewMinimum();
				axis.paintRange = axis.paintViewMax - axis.paintViewMin;
				axis.paintAreaPosition = base.PlotAreaPosition.ToRectangleF();
				if (axis.chartArea != null && axis.chartArea.chartAreaIsCurcular)
				{
					axis.paintAreaPosition.Width /= 2f;
					axis.paintAreaPosition.Height /= 2f;
				}
				axis.paintAreaPositionBottom = axis.paintAreaPosition.Y + axis.paintAreaPosition.Height;
				axis.paintAreaPositionRight = axis.paintAreaPosition.X + axis.paintAreaPosition.Width;
				if (axis.AxisPosition == AxisPosition.Top || axis.AxisPosition == AxisPosition.Bottom)
				{
					axis.paintChartAreaSize = axis.paintAreaPosition.Width;
				}
				else
				{
					axis.paintChartAreaSize = axis.paintAreaPosition.Height;
				}
				axis.valueMultiplier = 0.0;
				if (axis.paintRange != 0.0)
				{
					axis.valueMultiplier = axis.paintChartAreaSize / axis.paintRange;
				}
			}
			Axis[] array2 = new Axis[4]
			{
				axisY,
				axisY2,
				axisX,
				axisX2
			};
			array = array2;
			foreach (Axis axis2 in array)
			{
				if (axis2.ScaleSegments.Count <= 0)
				{
					axis2.PaintStrips(graph, drawLinesOnly: false);
					continue;
				}
				foreach (AxisScaleSegment scaleSegment in axis2.ScaleSegments)
				{
					scaleSegment.SetTempAxisScaleAndInterval();
					axis2.PaintStrips(graph, drawLinesOnly: false);
					scaleSegment.RestoreAxisScaleAndInterval();
				}
			}
			array2 = new Axis[4]
			{
				axisY,
				axisX2,
				axisY2,
				axisX
			};
			array = array2;
			foreach (Axis axis3 in array)
			{
				if (axis3.ScaleSegments.Count <= 0)
				{
					axis3.PaintGrids(graph);
					continue;
				}
				foreach (AxisScaleSegment scaleSegment2 in axis3.ScaleSegments)
				{
					scaleSegment2.SetTempAxisScaleAndInterval();
					axis3.PaintGrids(graph);
					scaleSegment2.RestoreAxisScaleAndInterval();
				}
			}
			array = array2;
			foreach (Axis axis4 in array)
			{
				if (axis4.ScaleSegments.Count <= 0)
				{
					axis4.PaintStrips(graph, drawLinesOnly: true);
					continue;
				}
				foreach (AxisScaleSegment scaleSegment3 in axis4.ScaleSegments)
				{
					scaleSegment3.SetTempAxisScaleAndInterval();
					axis4.PaintStrips(graph, drawLinesOnly: true);
					scaleSegment3.RestoreAxisScaleAndInterval();
				}
			}
			if (base.Area3DStyle.Enable3D && !chartAreaIsCurcular)
			{
				array = array2;
				foreach (Axis axis5 in array)
				{
					if (axis5.ScaleSegments.Count <= 0)
					{
						axis5.PrePaint(graph);
						continue;
					}
					foreach (AxisScaleSegment scaleSegment4 in axis5.ScaleSegments)
					{
						scaleSegment4.SetTempAxisScaleAndInterval();
						axis5.PrePaint(graph);
						scaleSegment4.RestoreAxisScaleAndInterval();
					}
				}
			}
			bool flag = false;
			if (base.Area3DStyle.Enable3D || !IsBorderOnTopSeries())
			{
				flag = true;
				PaintAreaBack(graph, backgroundPosition2, borderOnly: true);
			}
			if (!base.Area3DStyle.Enable3D || chartAreaIsCurcular)
			{
				foreach (ChartTypeAndSeriesInfo item in GetChartTypesAndSeriesToDraw())
				{
					IterationCounter = 0;
					base.Common.ChartTypeRegistry.GetChartType(item.ChartType).Paint(graph, base.Common, this, item.Series);
				}
			}
			else
			{
				PaintChartSeries3D(graph);
			}
			if (!flag)
			{
				PaintAreaBack(graph, backgroundPosition2, borderOnly: true);
			}
			array = array2;
			foreach (Axis axis6 in array)
			{
				if (axis6.ScaleSegments.Count <= 0)
				{
					axis6.Paint(graph);
					continue;
				}
				foreach (AxisScaleSegment scaleSegment5 in axis6.ScaleSegments)
				{
					scaleSegment5.SetTempAxisScaleAndInterval();
					axis6.PaintOnSegmentedScalePassOne(graph);
					scaleSegment5.RestoreAxisScaleAndInterval();
				}
				axis6.PaintOnSegmentedScalePassTwo(graph);
			}
			base.Common.EventsManager.OnPaint(this, new ChartPaintEventArgs(graph, base.Common, base.PlotAreaPosition));
			array2 = new Axis[2]
			{
				axisY,
				axisY2
			};
			array = array2;
			foreach (Axis axis7 in array)
			{
				for (int k = 0; k < axis7.ScaleSegments.Count - 1; k++)
				{
					axis7.ScaleSegments[k].PaintBreakLine(graph, axis7.ScaleSegments[k + 1]);
				}
			}
			array = axisArray;
			foreach (Axis obj6 in array)
			{
				obj6.optimizedGetPosition = false;
				obj6.prefferedNumberofIntervals = 5;
				obj6.scaleSegmentsUsed = false;
			}
		}

		private bool IsBorderOnTopSeries()
		{
			bool result = true;
			foreach (Series item in base.Common.Chart.Series)
			{
				if (item.ChartArea == Name && (item.ChartType == SeriesChartType.Bubble || item.ChartType == SeriesChartType.Point))
				{
					return false;
				}
			}
			return result;
		}

		internal void PaintCursors(ChartGraphics graph, bool cursorOnly)
		{
			if (!base.Area3DStyle.Enable3D && requireAxes && !chartAreaIsCurcular && (base.Common == null || base.Common.ChartPicture == null || !base.Common.ChartPicture.isPrinting) && Position.Width != 0f && Position.Height != 0f && double.IsNaN(cursorX.SelectionStart) && double.IsNaN(cursorX.SelectionEnd) && double.IsNaN(cursorX.Position) && double.IsNaN(cursorY.SelectionStart) && double.IsNaN(cursorY.SelectionEnd))
			{
				double.IsNaN(cursorY.Position);
			}
		}

		internal ICircularChartType GetCircularChartType()
		{
			foreach (Series item in base.Common.DataManager.Series)
			{
				if (item.IsVisible() && item.ChartArea == Name)
				{
					ICircularChartType circularChartType = base.Common.ChartTypeRegistry.GetChartType(item.ChartTypeName) as ICircularChartType;
					if (circularChartType != null)
					{
						return circularChartType;
					}
				}
			}
			return null;
		}

		internal void FitCircularLabels(ChartGraphics chartGraph, ElementPosition chartAreaPosition, ref RectangleF plotArea, float xTitleSize, float yTitleSize)
		{
			if (AxisX.LabelStyle.Enabled)
			{
				SizeF absoluteSize = chartGraph.GetAbsoluteSize(new SizeF(xTitleSize, yTitleSize));
				RectangleF absoluteRectangle = chartGraph.GetAbsoluteRectangle(plotArea);
				RectangleF absoluteRectangle2 = chartGraph.GetAbsoluteRectangle(chartAreaPosition.ToRectangleF());
				float y = chartGraph.GetAbsolutePoint(new PointF(0f, AxisX.markSize + 1f)).Y;
				ArrayList axisList = GetCircularAxisList();
				CircularAxisLabelsStyle circularAxisLabelsStyle = GetCircularAxisLabelsStyle();
				if (AxisX.LabelStyle.Enabled && AxisX.LabelsAutoFit)
				{
					AxisX.autoLabelFont = new Font(AxisX.LabelStyle.Font.FontFamily, 14f, AxisX.LabelStyle.Font.Style, GraphicsUnit.Pixel);
					float circularLabelsSize = GetCircularLabelsSize(chartGraph, absoluteRectangle2, absoluteRectangle, absoluteSize);
					circularLabelsSize = Math.Min(circularLabelsSize * 1.1f, absoluteRectangle.Width / 5f);
					circularLabelsSize += y;
					AxisX.GetCircularAxisLabelsAutoFitFont(chartGraph, axisList, circularAxisLabelsStyle, absoluteRectangle, absoluteRectangle2, circularLabelsSize);
				}
				float circularLabelsSize2 = GetCircularLabelsSize(chartGraph, absoluteRectangle2, absoluteRectangle, absoluteSize);
				circularLabelsSize2 = Math.Min(circularLabelsSize2, absoluteRectangle.Width / 2.5f);
				circularLabelsSize2 += y;
				absoluteRectangle.X += circularLabelsSize2;
				absoluteRectangle.Width -= 2f * circularLabelsSize2;
				absoluteRectangle.Y += circularLabelsSize2;
				absoluteRectangle.Height -= 2f * circularLabelsSize2;
				if (absoluteRectangle.Width < 1f)
				{
					absoluteRectangle.Width = 1f;
				}
				if (absoluteRectangle.Height < 1f)
				{
					absoluteRectangle.Height = 1f;
				}
				plotArea = chartGraph.GetRelativeRectangle(absoluteRectangle);
				SizeF relativeSize = chartGraph.GetRelativeSize(new SizeF(circularLabelsSize2, circularLabelsSize2));
				AxisX.labelSize = relativeSize.Height;
				AxisX2.labelSize = relativeSize.Height;
				AxisY.labelSize = relativeSize.Width;
				AxisY2.labelSize = relativeSize.Width;
			}
		}

		internal float GetCircularLabelsSize(ChartGraphics chartGraph, RectangleF areaRectAbs, RectangleF plotAreaRectAbs, SizeF titleSize)
		{
			SizeF sizeF = new SizeF(plotAreaRectAbs.X - areaRectAbs.X, plotAreaRectAbs.Y - areaRectAbs.Y);
			sizeF.Width -= titleSize.Width;
			sizeF.Height -= titleSize.Height;
			PointF absolutePoint = chartGraph.GetAbsolutePoint(circularCenter);
			ArrayList arrayList = GetCircularAxisList();
			CircularAxisLabelsStyle circularAxisLabelsStyle = GetCircularAxisLabelsStyle();
			float num = 0f;
			foreach (CircularChartAreaAxis item in arrayList)
			{
				SizeF sizeF2 = chartGraph.MeasureString(item.Title.Replace("\\n", "\n"), (AxisX.autoLabelFont == null) ? AxisX.LabelStyle.font : AxisX.autoLabelFont);
				sizeF2.Width = (float)Math.Ceiling(sizeF2.Width * 1.1f);
				sizeF2.Height = (float)Math.Ceiling(sizeF2.Height * 1.1f);
				switch (circularAxisLabelsStyle)
				{
				case CircularAxisLabelsStyle.Circular:
					num = Math.Max(num, sizeF2.Height);
					break;
				case CircularAxisLabelsStyle.Radial:
				{
					float num3 = item.AxisPosition + 90f;
					float num4 = (float)Math.Cos((double)(num3 / 180f) * Math.PI) * sizeF2.Width;
					float num5 = (float)Math.Sin((double)(num3 / 180f) * Math.PI) * sizeF2.Width;
					num4 = (float)Math.Abs(Math.Ceiling(num4));
					num5 = (float)Math.Abs(Math.Ceiling(num5));
					num4 -= sizeF.Width;
					num5 -= sizeF.Height;
					if (num4 < 0f)
					{
						num4 = 0f;
					}
					if (num5 < 0f)
					{
						num5 = 0f;
					}
					num = Math.Max(num, Math.Max(num4, num5));
					break;
				}
				case CircularAxisLabelsStyle.Horizontal:
				{
					float num2 = item.AxisPosition;
					if (num2 > 180f)
					{
						num2 -= 180f;
					}
					PointF[] array = new PointF[1]
					{
						new PointF(absolutePoint.X, plotAreaRectAbs.Y)
					};
					Matrix matrix = new Matrix();
					matrix.RotateAt(num2, absolutePoint);
					matrix.TransformPoints(array);
					float width = sizeF2.Width;
					width -= areaRectAbs.Right - array[0].X;
					if (width < 0f)
					{
						width = 0f;
					}
					num = Math.Max(num, Math.Max(width, sizeF2.Height));
					break;
				}
				}
			}
			return num;
		}

		internal CircularAxisLabelsStyle GetCircularAxisLabelsStyle()
		{
			CircularAxisLabelsStyle circularAxisLabelsStyle = CircularAxisLabelsStyle.Auto;
			foreach (Series item in base.Common.DataManager.Series)
			{
				if (!item.IsVisible() || !(item.ChartArea == Name) || !item.IsAttributeSet("CircularLabelsStyle"))
				{
					continue;
				}
				string text = item["CircularLabelsStyle"];
				if (string.Compare(text, "Auto", StringComparison.OrdinalIgnoreCase) == 0)
				{
					circularAxisLabelsStyle = CircularAxisLabelsStyle.Auto;
					continue;
				}
				if (string.Compare(text, "Circular", StringComparison.OrdinalIgnoreCase) == 0)
				{
					circularAxisLabelsStyle = CircularAxisLabelsStyle.Circular;
					continue;
				}
				if (string.Compare(text, "Radial", StringComparison.OrdinalIgnoreCase) == 0)
				{
					circularAxisLabelsStyle = CircularAxisLabelsStyle.Radial;
					continue;
				}
				if (string.Compare(text, "Horizontal", StringComparison.OrdinalIgnoreCase) == 0)
				{
					circularAxisLabelsStyle = CircularAxisLabelsStyle.Horizontal;
					continue;
				}
				throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid(text, "CircularLabelsStyle"));
			}
			if (circularAxisLabelsStyle == CircularAxisLabelsStyle.Auto)
			{
				int circularSectorsNumber = CircularSectorsNumber;
				circularAxisLabelsStyle = CircularAxisLabelsStyle.Horizontal;
				if (circularSectorsNumber > 30)
				{
					circularAxisLabelsStyle = CircularAxisLabelsStyle.Radial;
				}
			}
			return circularAxisLabelsStyle;
		}

		private int GetCircularSectorNumber()
		{
			return GetCircularChartType()?.GetNumerOfSectors(this, base.Common.DataManager.Series) ?? 0;
		}

		internal ArrayList GetCircularAxisList()
		{
			if (circularAxisList == null)
			{
				circularAxisList = new ArrayList();
				int num = GetCircularSectorNumber();
				for (int i = 0; i < num; i++)
				{
					CircularChartAreaAxis circularChartAreaAxis = new CircularChartAreaAxis((float)i * 360f / (float)num, 360f / (float)num);
					if (AxisX.CustomLabels.Count > 0)
					{
						if (i < AxisX.CustomLabels.Count)
						{
							circularChartAreaAxis.Title = AxisX.CustomLabels[i].Text;
							circularChartAreaAxis.TitleColor = AxisX.CustomLabels[i].TextColor;
						}
					}
					else
					{
						foreach (Series item in base.Common.DataManager.Series)
						{
							if (item.IsVisible() && item.ChartArea == Name && i < item.Points.Count && item.Points[i].AxisLabel.Length > 0)
							{
								circularChartAreaAxis.Title = item.Points[i].AxisLabel;
								break;
							}
						}
					}
					circularAxisList.Add(circularChartAreaAxis);
				}
			}
			return circularAxisList;
		}

		internal float CircularPositionToAngle(double position)
		{
			double num = 360.0 / Math.Abs(AxisX.Maximum - AxisX.Minimum);
			return (float)(position * num + AxisX.Crossing);
		}

		private ArrayList GetChartTypesAndSeriesToDraw()
		{
			ArrayList arrayList = new ArrayList();
			if (base.ChartTypes.Count > 1 && (base.ChartTypes.Contains("Area") || base.ChartTypes.Contains("SplineArea")))
			{
				ArrayList arrayList2 = new ArrayList();
				ArrayList arrayList3 = new ArrayList();
				ChartAreaCollection chartAreas = base.Common.Chart.ChartAreas;
				bool flag = chartAreas.Count > 0 && chartAreas[0] == this && chartAreas.GetIndex("Default") == -1;
				int num = 0;
				{
					foreach (Series item in base.Common.DataManager.Series)
					{
						if (item.IsVisible() && item.Points.Count > 0 && (Name == item.ChartArea || (flag && item.ChartArea == "Default")) && !arrayList2.Contains(item.ChartTypeName))
						{
							bool flag2 = false;
							if (item.ChartType == SeriesChartType.Point || item.ChartType == SeriesChartType.Line || item.ChartType == SeriesChartType.Spline || item.ChartType == SeriesChartType.StepLine)
							{
								flag2 = true;
							}
							if (!flag2)
							{
								arrayList.Add(new ChartTypeAndSeriesInfo(item.ChartTypeName));
								arrayList2.Add(item.ChartTypeName);
							}
							else
							{
								bool flag3 = false;
								if (arrayList3.Contains(item.ChartTypeName))
								{
									flag3 = true;
								}
								else
								{
									bool flag4 = false;
									for (int i = num + 1; i < base.Common.DataManager.Series.Count; i++)
									{
										if (item.ChartTypeName == base.Common.DataManager.Series[i].ChartTypeName)
										{
											if (flag4)
											{
												flag3 = true;
												arrayList3.Add(item.ChartTypeName);
											}
										}
										else if (base.Common.DataManager.Series[i].ChartType == SeriesChartType.Area || base.Common.DataManager.Series[i].ChartType == SeriesChartType.SplineArea)
										{
											flag4 = true;
										}
									}
								}
								if (flag3)
								{
									arrayList.Add(new ChartTypeAndSeriesInfo(item));
								}
								else
								{
									arrayList.Add(new ChartTypeAndSeriesInfo(item.ChartTypeName));
									arrayList2.Add(item.ChartTypeName);
								}
							}
						}
						num++;
					}
					return arrayList;
				}
			}
			foreach (string chartType in base.ChartTypes)
			{
				arrayList.Add(new ChartTypeAndSeriesInfo(chartType));
			}
			return arrayList;
		}
	}
}
