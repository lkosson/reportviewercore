using Microsoft.Reporting.Chart.WebForms.ChartTypes;
using Microsoft.Reporting.Chart.WebForms.Data;
using Microsoft.Reporting.Chart.WebForms.Design;
using Microsoft.Reporting.Chart.WebForms.Utilities;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace Microsoft.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeSeries_Series")]
	[DefaultProperty("Points")]
	internal class Series : DataPointAttributes
	{
		internal enum SeriesValuesFormulaType
		{
			Total,
			Average,
			Maximum,
			Minimum,
			First,
			Last
		}

		private string name = "";

		private ChartValueTypes xValueType;

		private ChartValueTypes yValueType;

		private bool xValueIndexed;

		private int yValuesPerPoint = 1;

		private int markersStep = 1;

		private ChartColorPalette colorPalette;

		private AxisType xAxisType;

		private AxisType yAxisType;

		private DataPointAttributes emptyPointAttributes = new DataPointAttributes(null, pointAttributes: false);

		private DataPointCollection points;

		private int shadowOffset;

		private Color shadowColor = Color.FromArgb(128, 0, 0, 0);

		private string chartType = "Column";

		private string chartArea = "Default";

		private bool enabled = true;

		private string legend = "Default";

		private string dataSourceMemberX = string.Empty;

		private string dataSourceMemberY = string.Empty;

		internal bool autoXValueType;

		internal bool autoYValueType;

		private double totalYvalue = double.NaN;

		private double[] dummyDoubleValues;

		internal ChartValueTypes indexedXValueType;

		internal static DataPointAttributes defaultAttributes;

		internal bool tempMarkerStyleIsSet;

		private bool defaultChartArea;

		private bool checkPointsNumber = true;

		internal Chart chart;

		internal FinancialMarkersCollection financialMarkers;

		private SmartLabelsStyle smartLabels;

		internal bool noLabelsInPoints = true;

		internal bool xValuesZeros;

		internal bool xValuesZerosChecked;

		internal DataPointCollection fakeDataPoints;

		internal string label = "";

		internal string axisLabel = "";

		internal string labelFormat = "";

		internal bool showLabelAsValue;

		internal Color color = Color.Empty;

		internal Color borderColor = Color.Empty;

		internal ChartDashStyle borderStyle = ChartDashStyle.Solid;

		internal int borderWidth = 1;

		internal int markerBorderWidth = 1;

		internal string backImage = "";

		internal ChartImageWrapMode backImageMode;

		internal Color backImageTranspColor = Color.Empty;

		internal ChartImageAlign backImageAlign;

		internal GradientType backGradientType;

		internal Color backGradientEndColor = Color.Empty;

		internal ChartHatchStyle backHatchStyle;

		internal Font font = new Font(ChartPicture.GetDefaultFontFamilyName(), 8f);

		internal Color fontColor = Color.Black;

		internal int fontAngle;

		internal MarkerStyle markerStyle;

		internal int markerSize = 5;

		internal string markerImage = "";

		internal Color markerImageTranspColor = Color.Empty;

		internal Color markerColor = Color.Empty;

		internal Color markerBorderColor = Color.Empty;

		internal string toolTip = "";

		internal string href = "";

		internal string mapAreaAttributes = "";

		internal bool showInLegend = true;

		internal string legendText = "";

		internal string legendToolTip = "";

		internal Color labelBackColor = Color.Empty;

		internal Color labelBorderColor = Color.Empty;

		internal ChartDashStyle labelBorderStyle = ChartDashStyle.Solid;

		internal int labelBorderWidth = 1;

		internal string labelToolTip = "";

		internal string labelHref = "";

		internal string labelMapAreaAttributes = "";

		internal string legendHref = "";

		internal string legendMapAreaAttributes = "";

		[SRCategory("CategoryAttributeData")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeSeries_Name")]
		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				if (!(value != name))
				{
					return;
				}
				if (value == null || value.Length == 0)
				{
					throw new ArgumentException(SR.ExceptionSeriesNameIsEmpty);
				}
				if (serviceContainer != null)
				{
					DataManager dataManager = (DataManager)serviceContainer.GetService(typeof(DataManager));
					if (dataManager != null && dataManager.Series.GetIndex(value) != -1)
					{
						throw new ArgumentException(SR.ExceptionSeriesNameIsNotUnique(value));
					}
				}
				name = value;
				Invalidate(invalidateAreaOnly: false, invalidateLegend: true);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[SRCategory("CategoryAttributeDataSource")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeSeries_ValueMemberX")]
		[DefaultValue("")]
		public string ValueMemberX
		{
			get
			{
				return dataSourceMemberX;
			}
			set
			{
				if (value == "(none)")
				{
					dataSourceMemberX = string.Empty;
				}
				else
				{
					dataSourceMemberX = value;
				}
				if (serviceContainer != null)
				{
					ChartImage chartImage = (ChartImage)serviceContainer.GetService(typeof(ChartImage));
					if (chartImage != null)
					{
						chartImage.boundToDataSource = false;
					}
				}
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[SRCategory("CategoryAttributeDataSource")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeSeries_ValueMembersY")]
		[DefaultValue("")]
		public string ValueMembersY
		{
			get
			{
				return dataSourceMemberY;
			}
			set
			{
				if (value == "(none)")
				{
					dataSourceMemberY = string.Empty;
				}
				else
				{
					dataSourceMemberY = value;
				}
				if (serviceContainer != null)
				{
					ChartImage chartImage = (ChartImage)serviceContainer.GetService(typeof(ChartImage));
					if (chartImage != null)
					{
						chartImage.boundToDataSource = false;
					}
				}
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeLegend")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeSeries_Legend")]
		[DefaultValue("Default")]
		[TypeConverter(typeof(SeriesLegendNameConverter))]
		public string Legend
		{
			get
			{
				return legend;
			}
			set
			{
				if (value.Length == 0)
				{
					legend = "Default";
				}
				else
				{
					legend = value;
				}
				Invalidate(invalidateAreaOnly: false, invalidateLegend: true);
			}
		}

		[SRCategory("CategoryAttributeData")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeSeries_XValueType")]
		[DefaultValue(ChartValueTypes.Auto)]
		public ChartValueTypes XValueType
		{
			get
			{
				return xValueType;
			}
			set
			{
				xValueType = value;
				autoXValueType = false;
				Invalidate(invalidateAreaOnly: true, invalidateLegend: false);
			}
		}

		[SRCategory("CategoryAttributeData")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeSeries_XValueIndexed")]
		[DefaultValue(false)]
		public bool XValueIndexed
		{
			get
			{
				return xValueIndexed;
			}
			set
			{
				xValueIndexed = value;
				Invalidate(invalidateAreaOnly: true, invalidateLegend: false);
			}
		}

		[SRCategory("CategoryAttributeData")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeSeries_YValueType")]
		[DefaultValue(ChartValueTypes.Auto)]
		[TypeConverter(typeof(SeriesYValueTypeConverter))]
		public ChartValueTypes YValueType
		{
			get
			{
				return yValueType;
			}
			set
			{
				yValueType = value;
				autoYValueType = false;
				Invalidate(invalidateAreaOnly: true, invalidateLegend: false);
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeData")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeSeries_YValuesPerPoint")]
		[DefaultValue(1)]
		public int YValuesPerPoint
		{
			get
			{
				if (checkPointsNumber && ChartTypeName.Length > 0 && serviceContainer != null)
				{
					checkPointsNumber = false;
					IChartType chartType = ((ChartTypeRegistry)serviceContainer.GetService(typeof(ChartTypeRegistry))).GetChartType(ChartTypeName);
					if (chartType.YValuesPerPoint > yValuesPerPoint)
					{
						yValuesPerPoint = chartType.YValuesPerPoint;
						if (points.Count > 0)
						{
							foreach (DataPoint point in points)
							{
								point.ResizeYValueArray(yValuesPerPoint);
							}
						}
					}
				}
				return yValuesPerPoint;
			}
			set
			{
				if (value < 1 || value > 32)
				{
					throw new ArgumentOutOfRangeException("value", SR.ExceptionDataSeriesYValueNumberInvalid);
				}
				checkPointsNumber = true;
				if (points.Count > 0)
				{
					foreach (DataPoint point in points)
					{
						point.ResizeYValueArray(value);
					}
				}
				yValuesPerPoint = value;
				Invalidate(invalidateAreaOnly: true, invalidateLegend: false);
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeData")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeSeries_Points")]
		public DataPointCollection Points => points;

		[SRCategory("CategoryAttributeEmptyPoints")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeSeries_EmptyPointStyle")]
		public DataPointAttributes EmptyPointStyle
		{
			get
			{
				return emptyPointAttributes;
			}
			set
			{
				if (value.series == null && emptyPointAttributes.series != null)
				{
					value.series = emptyPointAttributes.series;
				}
				emptyPointAttributes = value;
				emptyPointAttributes.pointAttributes = false;
				emptyPointAttributes.SetDefault(clearAll: false);
				emptyPointAttributes.pointAttributes = true;
				Invalidate(invalidateAreaOnly: true, invalidateLegend: false);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributePalette")]
		[DefaultValue(ChartColorPalette.None)]
		public ChartColorPalette Palette
		{
			get
			{
				return colorPalette;
			}
			set
			{
				colorPalette = value;
				Invalidate(invalidateAreaOnly: true, invalidateLegend: true);
			}
		}

		[SRCategory("CategoryAttributeMarker")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeSeries_MarkerStep")]
		[DefaultValue(1)]
		public int MarkerStep
		{
			get
			{
				return markersStep;
			}
			set
			{
				if (value <= 0)
				{
					throw new ArgumentException(SR.ExceptionMarkerStepNegativeValue, "value");
				}
				markersStep = value;
				Invalidate(invalidateAreaOnly: true, invalidateLegend: false);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeSeries_ShadowOffset")]
		[DefaultValue(0)]
		public int ShadowOffset
		{
			get
			{
				return shadowOffset;
			}
			set
			{
				shadowOffset = value;
				Invalidate(invalidateAreaOnly: true, invalidateLegend: true);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "128,0,0,0")]
		[SRDescription("DescriptionAttributeSeries_ShadowColor")]
		public Color ShadowColor
		{
			get
			{
				return shadowColor;
			}
			set
			{
				shadowColor = value;
				Invalidate(invalidateAreaOnly: true, invalidateLegend: true);
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeFinancialMarkers")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeSeries_FinancialMarkers")]
		public FinancialMarkersCollection FinancialMarkers
		{
			get
			{
				financialMarkers.series = this;
				return financialMarkers;
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeAxes")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeSeries_YSubAxisName")]
		[DefaultValue("")]
		internal string YSubAxisName
		{
			get
			{
				return string.Empty;
			}
			set
			{
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeAxes")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeSeries_XSubAxisName")]
		[DefaultValue("")]
		internal string XSubAxisName
		{
			get
			{
				return string.Empty;
			}
			set
			{
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeAxes")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeSeries_XAxisType")]
		[DefaultValue(AxisType.Primary)]
		public AxisType XAxisType
		{
			get
			{
				return xAxisType;
			}
			set
			{
				xAxisType = value;
				Invalidate(invalidateAreaOnly: true, invalidateLegend: false);
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeAxes")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeSeries_YAxisType")]
		[DefaultValue(AxisType.Primary)]
		public AxisType YAxisType
		{
			get
			{
				return yAxisType;
			}
			set
			{
				yAxisType = value;
				Invalidate(invalidateAreaOnly: true, invalidateLegend: false);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeSeries_Enabled")]
		[NotifyParentProperty(true)]
		[ParenthesizePropertyName(true)]
		public bool Enabled
		{
			get
			{
				return enabled;
			}
			set
			{
				enabled = value;
				Invalidate(invalidateAreaOnly: true, invalidateLegend: true);
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeChart")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeSeries_Type")]
		[DefaultValue(SeriesChartType.Column)]
		[RefreshProperties(RefreshProperties.All)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SeriesChartType ChartType
		{
			get
			{
				SeriesChartType result = SeriesChartType.Column;
				if (string.Compare(ChartTypeName, "100%StackedArea", StringComparison.OrdinalIgnoreCase) == 0)
				{
					return SeriesChartType.StackedArea100;
				}
				if (string.Compare(ChartTypeName, "100%StackedBar", StringComparison.OrdinalIgnoreCase) == 0)
				{
					return SeriesChartType.StackedBar100;
				}
				if (string.Compare(ChartTypeName, "100%StackedColumn", StringComparison.OrdinalIgnoreCase) == 0)
				{
					return SeriesChartType.StackedColumn100;
				}
				try
				{
					result = (SeriesChartType)Enum.Parse(typeof(SeriesChartType), ChartTypeName, ignoreCase: true);
					return result;
				}
				catch
				{
					return result;
				}
			}
			set
			{
				ChartTypeName = GetChartTypeName(value);
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRCategory("CategoryAttributeChart")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeSeries_Type")]
		[DefaultValue("Column")]
		[TypeConverter(typeof(ChartTypeConverter))]
		[RefreshProperties(RefreshProperties.All)]
		public string ChartTypeName
		{
			get
			{
				return chartType;
			}
			set
			{
				if (this.chartType != value && value.Length > 0 && serviceContainer != null)
				{
					ChartTypeRegistry chartTypeRegistry = (ChartTypeRegistry)serviceContainer.GetService(typeof(ChartTypeRegistry));
					if (chartTypeRegistry != null)
					{
						IChartType chartType = chartTypeRegistry.GetChartType(value);
						if (yValuesPerPoint < chartType.YValuesPerPoint)
						{
							yValuesPerPoint = chartType.YValuesPerPoint;
							if (points.Count > 0)
							{
								foreach (DataPoint point in points)
								{
									point.ResizeYValueArray(yValuesPerPoint);
								}
							}
						}
					}
				}
				this.chartType = value;
				Invalidate(invalidateAreaOnly: false, invalidateLegend: true);
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeChart")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeSeries_ChartArea")]
		[DefaultValue("Default")]
		[TypeConverter(typeof(SeriesAreaNameConverter))]
		public string ChartArea
		{
			get
			{
				return chartArea;
			}
			set
			{
				chartArea = value;
				defaultChartArea = false;
				Invalidate(invalidateAreaOnly: false, invalidateLegend: true);
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeMisc")]
		[Bindable(true)]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeAxisLabel")]
		public override string AxisLabel
		{
			get
			{
				return base.AxisLabel;
			}
			set
			{
				base.AxisLabel = value;
				Invalidate(invalidateAreaOnly: true, invalidateLegend: false);
			}
		}

		[Browsable(true)]
		[SRCategory("CategoryAttributeLabel")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeSeries_SmartLabels")]
		public SmartLabelsStyle SmartLabels
		{
			get
			{
				return smartLabels;
			}
			set
			{
				value.chartElement = this;
				smartLabels = value;
				Invalidate(invalidateAreaOnly: false, invalidateLegend: false);
			}
		}

		static Series()
		{
			defaultAttributes = new DataPointAttributes(null, pointAttributes: false);
			defaultAttributes.SetDefault(clearAll: true);
			defaultAttributes.pointAttributes = true;
		}

		public Series()
			: base(null, pointAttributes: false)
		{
			InitProperties(null, 0);
		}

		public Series(string name)
			: base(null, pointAttributes: false)
		{
			if (name == null)
			{
				throw new ArgumentNullException(SR.ExceptionDataSeriesNameIsEmpty);
			}
			InitProperties(name, 0);
		}

		public Series(string name, int yValues)
			: base(null, pointAttributes: false)
		{
			if (name == null)
			{
				throw new ArgumentNullException(SR.ExceptionDataSeriesNameIsEmpty);
			}
			if (YValuesPerPoint < 1)
			{
				throw new ArgumentOutOfRangeException("yValues", SR.ExceptionDataSeriesYValuesPerPointIsZero);
			}
			InitProperties(name, yValues);
		}

		private void InitProperties(string name, int YValuesPerPoint)
		{
			series = this;
			emptyPointAttributes.series = this;
			points = new DataPointCollection(this);
			fakeDataPoints = new DataPointCollection(this);
			if (name != null)
			{
				this.name = name;
			}
			if (YValuesPerPoint != 0)
			{
				yValuesPerPoint = YValuesPerPoint;
			}
			base.SetDefault(clearAll: true);
			emptyPointAttributes.SetDefault(clearAll: true);
			emptyPointAttributes.pointAttributes = true;
			if (financialMarkers == null)
			{
				financialMarkers = new FinancialMarkersCollection();
			}
			smartLabels = new SmartLabelsStyle(this);
		}

		internal string GetCaption()
		{
			if (IsAttributeSet("SeriesCaption"))
			{
				return base["SeriesCaption"];
			}
			return Name;
		}

		internal void GetPointDepthAndGap(ChartGraphics graph, Axis axis, ref double pointDepth, ref double pointGapDepth)
		{
			string text = base["PixelPointDepth"];
			if (text != null)
			{
				try
				{
					pointDepth = CommonElements.ParseDouble(text);
					SizeF relativeSize = graph.GetRelativeSize(new SizeF((float)pointDepth, (float)pointDepth));
					pointDepth = relativeSize.Width;
					if (axis.AxisPosition == AxisPosition.Left || axis.AxisPosition == AxisPosition.Right)
					{
						pointDepth = relativeSize.Height;
					}
				}
				catch
				{
					throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid2("PixelPointDepth"));
				}
			}
			text = base["PixelPointGapDepth"];
			if (text == null)
			{
				return;
			}
			try
			{
				pointGapDepth = CommonElements.ParseDouble(text);
				SizeF relativeSize2 = graph.GetRelativeSize(new SizeF((float)pointGapDepth, (float)pointGapDepth));
				pointGapDepth = relativeSize2.Width;
				if (axis.AxisPosition == AxisPosition.Left || axis.AxisPosition == AxisPosition.Right)
				{
					pointGapDepth = relativeSize2.Height;
				}
			}
			catch
			{
				throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid2("PixelPointGapDepth"));
			}
		}

		internal double GetPointWidth(ChartGraphics graph, Axis axis, double interval, double defaultWidth)
		{
			double num = defaultWidth;
			double num2 = 0.0;
			string text = base["PointWidth"];
			if (text != null)
			{
				num = CommonElements.ParseDouble(text);
			}
			num2 = axis.GetPixelInterval(interval * num);
			SizeF absoluteSize = graph.GetAbsoluteSize(new SizeF((float)num2, (float)num2));
			double num3 = absoluteSize.Width;
			if (axis.AxisPosition == AxisPosition.Left || axis.AxisPosition == AxisPosition.Right)
			{
				num3 = absoluteSize.Height;
			}
			bool flag = false;
			string text2 = base["MinPixelPointWidth"];
			if (text2 != null)
			{
				double num4 = 0.0;
				try
				{
					num4 = CommonElements.ParseDouble(text2);
				}
				catch
				{
					throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid2("MinPixelPointWidth"));
				}
				if (num4 <= 0.0)
				{
					throw new InvalidOperationException(SR.ExceptionCustomAttributeIsNotLargerThenZiro("MinPixelPointWidth"));
				}
				if (num3 < num4)
				{
					flag = true;
					num3 = num4;
				}
			}
			text2 = base["MaxPixelPointWidth"];
			if (text2 != null)
			{
				double num5 = 0.0;
				try
				{
					num5 = CommonElements.ParseDouble(text2);
				}
				catch
				{
					throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid2("MaxPixelPointWidth"));
				}
				if (num5 <= 0.0)
				{
					throw new InvalidOperationException(SR.ExceptionCustomAttributeIsNotLargerThenZiro("MaxPixelPointWidth"));
				}
				if (num3 > num5)
				{
					flag = true;
					num3 = num5;
				}
			}
			text2 = base["PixelPointWidth"];
			if (text2 != null)
			{
				flag = true;
				num3 = 0.0;
				try
				{
					num3 = CommonElements.ParseDouble(text2);
				}
				catch
				{
					throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid2("PixelPointWidth"));
				}
				if (num3 <= 0.0)
				{
					throw new InvalidOperationException(SR.ExceptionCustomAttributeIsNotLargerThenZiro("PixelPointWidth"));
				}
			}
			if (flag)
			{
				SizeF relativeSize = graph.GetRelativeSize(new SizeF((float)num3, (float)num3));
				num2 = relativeSize.Width;
				if (axis.AxisPosition == AxisPosition.Left || axis.AxisPosition == AxisPosition.Right)
				{
					num2 = relativeSize.Height;
				}
			}
			return num2;
		}

		internal static string GetChartTypeName(SeriesChartType type)
		{
			switch (type)
			{
			case SeriesChartType.StackedArea100:
				return "100%StackedArea";
			case SeriesChartType.StackedBar100:
				return "100%StackedBar";
			case SeriesChartType.StackedColumn100:
				return "100%StackedColumn";
			default:
				return Enum.GetName(typeof(SeriesChartType), type);
			}
		}

		internal bool IsYValueDateTime()
		{
			if (YValueType == ChartValueTypes.Date || YValueType == ChartValueTypes.DateTime || YValueType == ChartValueTypes.Time || YValueType == ChartValueTypes.DateTimeOffset)
			{
				return true;
			}
			return false;
		}

		internal bool IsXValueDateTime()
		{
			if (XValueType == ChartValueTypes.Date || XValueType == ChartValueTypes.DateTime || XValueType == ChartValueTypes.Time || XValueType == ChartValueTypes.DateTimeOffset)
			{
				return true;
			}
			return false;
		}

		internal bool IsVisible()
		{
			if (Enabled && ChartArea.Length > 0)
			{
				return true;
			}
			return false;
		}

		internal bool IsFastChartType()
		{
			if (ChartType == SeriesChartType.FastLine)
			{
				return true;
			}
			if (ChartType == SeriesChartType.FastPoint)
			{
				return true;
			}
			return false;
		}

		internal void CheckSupportedTypes(Type type)
		{
			if (type == typeof(double) || type == typeof(DateTime) || type == typeof(string) || type == typeof(int) || type == typeof(uint) || type == typeof(decimal) || type == typeof(float) || type == typeof(short) || type == typeof(ushort) || type == typeof(long) || type == typeof(ulong) || type == typeof(byte) || type == typeof(sbyte) || type == typeof(DBNull) || type == typeof(bool))
			{
				return;
			}
			throw new ArrayTypeMismatchException(SR.ExceptionDataSeriesPointTypeUnsupported(type.ToString()));
		}

		internal void ApplyPaletteColors()
		{
			ChartColorPalette palette = Palette;
			DataManager dataManager = (DataManager)serviceContainer.GetService(typeof(DataManager));
			if (palette == ChartColorPalette.None)
			{
				palette = dataManager.Palette;
			}
			if (palette == ChartColorPalette.None && dataManager.PaletteCustomColors.Length == 0)
			{
				return;
			}
			int num = 0;
			Color[] array = (dataManager.PaletteCustomColors.Length != 0) ? dataManager.PaletteCustomColors : ChartPaletteColors.GetPaletteColors(palette);
			bool flag = !IsAttributeSet("SkipPaletteColorForEmptyPoint") || string.Compare(base["SkipPaletteColorForEmptyPoint"], "FALSE", StringComparison.OrdinalIgnoreCase) != 0;
			foreach (DataPoint point in points)
			{
				if ((!flag || !point.Empty) && (!point.IsAttributeSet(CommonAttributes.Color) || point.tempColorIsSet))
				{
					if (!point.Empty)
					{
						point.SetAttributeObject(CommonAttributes.Color, array[num]);
						point.tempColorIsSet = true;
					}
					num++;
					if (num >= array.Length)
					{
						num = 0;
					}
				}
			}
		}

		internal IEnumerable GetDummyData(ChartValueTypes type)
		{
			string[] result = new string[6]
			{
				"abc1",
				"abc2",
				"abc3",
				"abc4",
				"abc5",
				"abc6"
			};
			DateTime[] result2 = new DateTime[6]
			{
				DateTime.Now.Date,
				DateTime.Now.Date.AddDays(1.0),
				DateTime.Now.Date.AddDays(2.0),
				DateTime.Now.Date.AddDays(3.0),
				DateTime.Now.Date.AddDays(4.0),
				DateTime.Now.Date.AddDays(4.0)
			};
			if (dummyDoubleValues == null)
			{
				int num = 0;
				for (int i = 0; i < name.Length; i++)
				{
					num += name[i];
				}
				Random random = new Random(num);
				dummyDoubleValues = new double[6];
				for (int j = 0; j < 6; j++)
				{
					dummyDoubleValues[j] = random.Next(10, 100);
				}
			}
			switch (type)
			{
			case ChartValueTypes.DateTime:
			case ChartValueTypes.Date:
			case ChartValueTypes.DateTimeOffset:
				return result2;
			case ChartValueTypes.Time:
				return new DateTime[6]
				{
					DateTime.Now,
					DateTime.Now.AddMinutes(1.0),
					DateTime.Now.AddMinutes(2.0),
					DateTime.Now.AddMinutes(3.0),
					DateTime.Now.AddMinutes(4.0),
					DateTime.Now.AddMinutes(4.0)
				};
			case ChartValueTypes.String:
				return result;
			default:
				return dummyDoubleValues;
			}
		}

		internal double GetTotalYValue()
		{
			return GetTotalYValue(0);
		}

		internal double GetTotalYValue(int yValueIndex)
		{
			if (yValueIndex == 0)
			{
				if (!double.IsNaN(totalYvalue))
				{
					return totalYvalue;
				}
				totalYvalue = 0.0;
				foreach (DataPoint point in Points)
				{
					totalYvalue += point.YValues[yValueIndex];
				}
				return totalYvalue;
			}
			if (yValueIndex >= YValuesPerPoint)
			{
				throw new InvalidOperationException(SR.ExceptionDataSeriesYValueIndexNotExists(yValueIndex.ToString(CultureInfo.InvariantCulture), Name));
			}
			double num = 0.0;
			foreach (DataPoint point2 in Points)
			{
				num += point2.YValues[yValueIndex];
			}
			return num;
		}

		internal string ReplaceKeywords(string strOriginal)
		{
			if (strOriginal == null || strOriginal.Length == 0)
			{
				return strOriginal;
			}
			string text = strOriginal.Replace("\\n", "\n");
			text = text.Replace("#SERIESNAME", Name);
			text = text.Replace("#SER", Name);
			text = ReplaceOneKeyword(chart, this, ChartElementType.Nothing, text, "#TOTAL", SeriesValuesFormulaType.Total, YValueType, "");
			text = ReplaceOneKeyword(chart, this, ChartElementType.Nothing, text, "#AVG", SeriesValuesFormulaType.Average, YValueType, "");
			text = ReplaceOneKeyword(chart, this, ChartElementType.Nothing, text, "#MAX", SeriesValuesFormulaType.Maximum, YValueType, "");
			text = ReplaceOneKeyword(chart, this, ChartElementType.Nothing, text, "#MIN", SeriesValuesFormulaType.Minimum, YValueType, "");
			text = ReplaceOneKeyword(chart, this, ChartElementType.Nothing, text, "#FIRST", SeriesValuesFormulaType.First, YValueType, "");
			text = ReplaceOneKeyword(chart, this, ChartElementType.Nothing, text, "#LAST", SeriesValuesFormulaType.Last, YValueType, "");
			return text.Replace("#LEGENDTEXT", base.LegendText);
		}

		internal string ReplaceOneKeyword(Chart chart, object obj, ChartElementType elementType, string strOriginal, string keyword, SeriesValuesFormulaType formulaType, ChartValueTypes valueType, string defaultFormat)
		{
			string text = strOriginal;
			int num = -1;
			while ((num = text.IndexOf(keyword, StringComparison.Ordinal)) != -1)
			{
				int num2 = num + keyword.Length;
				int num3 = 0;
				if (text.Length > num2 + 1 && text[num2] == 'Y' && char.IsDigit(text[num2 + 1]))
				{
					num3 = int.Parse(text.Substring(num2 + 1, 1), CultureInfo.InvariantCulture);
					num2 += 2;
				}
				string format = defaultFormat;
				if (text.Length > num2 && text[num2] == '{')
				{
					int num4 = text.IndexOf('}', num2);
					if (num4 == -1)
					{
						throw new InvalidOperationException(SR.ExceptionDataSeriesKeywordFormatInvalid(text));
					}
					format = text.Substring(num2, num4 - num2).Trim('{', '}');
					num2 = num4 + 1;
				}
				text = text.Remove(num, num2 - num);
				double totalYValue = GetTotalYValue(num3);
				double num5 = 0.0;
				switch (formulaType)
				{
				case SeriesValuesFormulaType.Average:
					if (Points.Count > 0)
					{
						num5 = totalYValue / (double)Points.Count;
					}
					break;
				case SeriesValuesFormulaType.First:
					if (Points.Count > 0)
					{
						num5 = Points[0].YValues[num3];
					}
					break;
				case SeriesValuesFormulaType.Last:
					if (Points.Count > 0)
					{
						num5 = Points[Points.Count - 1].YValues[num3];
					}
					break;
				case SeriesValuesFormulaType.Maximum:
					if (Points.Count <= 0)
					{
						break;
					}
					num5 = double.MinValue;
					foreach (DataPoint point in Points)
					{
						num5 = Math.Max(num5, point.YValues[num3]);
					}
					break;
				case SeriesValuesFormulaType.Minimum:
					if (Points.Count <= 0)
					{
						break;
					}
					num5 = double.MaxValue;
					foreach (DataPoint point2 in Points)
					{
						num5 = Math.Min(num5, point2.YValues[num3]);
					}
					break;
				case SeriesValuesFormulaType.Total:
					num5 = totalYValue;
					break;
				}
				text = text.Insert(num, ValueConverter.FormatValue(chart, obj, num5, format, valueType, elementType));
			}
			return text;
		}

		internal string ReplaceOneKeyword(Chart chart, object obj, ChartElementType elementType, string strOriginal, string keyword, double value, ChartValueTypes valueType, string defaultFormat)
		{
			string text = strOriginal;
			int num = -1;
			while ((num = text.IndexOf(keyword, StringComparison.Ordinal)) != -1)
			{
				int num2 = num + keyword.Length;
				string format = defaultFormat;
				if (text.Length > num2 && text[num2] == '{')
				{
					int num3 = text.IndexOf('}', num2);
					if (num3 == -1)
					{
						throw new InvalidOperationException(SR.ExceptionDataSeriesKeywordFormatInvalid(text));
					}
					format = text.Substring(num2, num3 - num2).Trim('{', '}');
					num2 = num3 + 1;
				}
				text = text.Remove(num, num2 - num);
				text = text.Insert(num, ValueConverter.FormatValue(chart, obj, value, format, valueType, elementType));
			}
			return text;
		}

		internal void TraceWrite(string category, string message)
		{
			if (serviceContainer != null)
			{
				((TraceManager)serviceContainer.GetService(typeof(TraceManager)))?.Write(category, message);
			}
		}

		public void Sort(PointsSortOrder order, string sortBy)
		{
			TraceWrite("ChartData", SR.TraceMessageBeginSortingDataPoints(Name));
			DataPointComparer comparer = new DataPointComparer(this, order, sortBy);
			Points.array.Sort(comparer);
			Invalidate(invalidateAreaOnly: true, invalidateLegend: false);
			TraceWrite("ChartData", SR.TraceMessageEndSortingDataPoints(Name));
		}

		public void Sort(PointsSortOrder order)
		{
			Sort(order, "Y");
		}

		public void Sort(IComparer comparer)
		{
			TraceWrite("ChartData", SR.TraceMessageBeginSortingDataPoints(Name));
			Points.array.Sort(comparer);
			Invalidate(invalidateAreaOnly: true, invalidateLegend: false);
			TraceWrite("ChartData", SR.TraceMessageEndSortingDataPoints(Name));
		}

		internal bool UnPrepareData(ISite controlSite)
		{
			bool result = false;
			if (RenkoChart.UnPrepareData(this, serviceContainer))
			{
				result = true;
			}
			if (ThreeLineBreakChart.UnPrepareData(this, serviceContainer))
			{
				result = true;
			}
			if (KagiChart.UnPrepareData(this, serviceContainer))
			{
				result = true;
			}
			if (PointAndFigureChart.UnPrepareData(this, serviceContainer))
			{
				result = true;
			}
			if (PieChart.UnPrepareData(this, serviceContainer))
			{
				result = true;
			}
			if (xValueIndexed)
			{
				xValueType = indexedXValueType;
			}
			bool reset = true;
			ResetAutoValues(reset);
			return result;
		}

		internal void ResetAutoValues()
		{
			ResetAutoValues(reset: true);
		}

		internal void ResetAutoValues(bool reset)
		{
			if (IsAttributeSet("TempDesignData"))
			{
				DeleteAttribute("TempDesignData");
				bool flag = true;
				if (chart != null && !chart.IsDesignMode())
				{
					flag = false;
				}
				if (flag)
				{
					fakeDataPoints.Clear();
					foreach (DataPoint point in Points)
					{
						fakeDataPoints.Add(point);
					}
				}
				Points.Clear();
			}
			if (defaultChartArea)
			{
				defaultChartArea = false;
				ChartArea = "Default";
			}
			if (tempColorIsSet)
			{
				tempColorIsSet = false;
				base.Color = Color.Empty;
			}
			if (tempMarkerStyleIsSet)
			{
				tempMarkerStyleIsSet = false;
				base.MarkerStyle = MarkerStyle.None;
			}
			foreach (DataPoint point2 in points)
			{
				if (point2.tempColorIsSet)
				{
					point2.Color = Color.Empty;
				}
			}
			if (reset && (chart == null || !chart.serializing))
			{
				if (autoXValueType)
				{
					xValueType = ChartValueTypes.Auto;
					autoXValueType = false;
				}
				if (autoYValueType)
				{
					yValueType = ChartValueTypes.Auto;
					autoYValueType = false;
				}
			}
		}

		internal void PrepareData(ISite controlSite, bool applyPaletteColors)
		{
			if (!IsVisible())
			{
				return;
			}
			TraceWrite("ChartData", SR.TraceMessageBeginPreparingChartDataInSeries(Name));
			bool flag = false;
			if (ChartArea.Length > 0)
			{
				ChartImage chartImage = (ChartImage)serviceContainer.GetService(typeof(ChartImage));
				if (chartImage != null)
				{
					foreach (ChartArea chartArea2 in chartImage.ChartAreas)
					{
						if (chartArea2.Name == ChartArea)
						{
							flag = true;
							break;
						}
					}
				}
				if (!flag && ChartArea == "Default" && ChartArea.Length > 0 && chartImage.ChartAreas.Count > 0)
				{
					flag = true;
					ChartArea = chartImage.ChartAreas[0].Name;
					defaultChartArea = true;
				}
			}
			if (Points.Count > 0 && Points[0].YValues.Length < YValuesPerPoint)
			{
				foreach (DataPoint point in Points)
				{
					point.ResizeYValueArray(YValuesPerPoint);
				}
			}
			if (ChartArea.Length > 0 && !flag)
			{
				throw new InvalidOperationException(SR.ExceptionDataSeriesChartAreaInvalid(ChartArea, Name));
			}
			bool flag2 = false;
			if (Points.Count == 0 && flag)
			{
				if (controlSite != null && controlSite.DesignMode)
				{
					flag2 = true;
				}
				else if (IsAttributeSet("UseDummyData") && string.Compare(base["UseDummyData"], "True", StringComparison.OrdinalIgnoreCase) == 0)
				{
					flag2 = true;
				}
			}
			if (flag2)
			{
				if (IsXValueDateTime() || xValueType == ChartValueTypes.String)
				{
					Points.DataBindXY(GetDummyData(xValueType), GetDummyData(yValueType));
				}
				else
				{
					double[] xValue = new double[6]
					{
						2.0,
						3.0,
						4.0,
						5.0,
						6.0,
						7.0
					};
					if (ChartType == SeriesChartType.Polar)
					{
						xValue = new double[6]
						{
							0.0,
							45.0,
							115.0,
							145.0,
							180.0,
							220.0
						};
					}
					Points.DataBindXY(xValue, GetDummyData(yValueType));
				}
				if (YValuesPerPoint > 1)
				{
					foreach (DataPoint point2 in Points)
					{
						for (int i = 1; i < YValuesPerPoint; i++)
						{
							point2.YValues[i] = point2.YValues[0];
						}
						if (YValuesPerPoint >= 2)
						{
							point2.YValues[1] = point2.YValues[0] / 2.0 - 1.0;
						}
						if (YValuesPerPoint >= 4)
						{
							point2.YValues[2] = point2.YValues[1] + (point2.YValues[0] - point2.YValues[1]) / 3.0;
							point2.YValues[3] = point2.YValues[2] + (point2.YValues[0] - point2.YValues[1]) / 3.0;
						}
						if (YValuesPerPoint >= 6)
						{
							point2.YValues[4] = point2.YValues[2] + (point2.YValues[3] - point2.YValues[2]) / 2.0;
							point2.YValues[5] = point2.YValues[2] + (point2.YValues[3] - point2.YValues[2]) / 3.0;
						}
					}
				}
				base["TempDesignData"] = "true";
			}
			if (xValueType == ChartValueTypes.Auto)
			{
				xValueType = ChartValueTypes.Double;
				autoXValueType = true;
			}
			if (yValueType == ChartValueTypes.Auto)
			{
				yValueType = ChartValueTypes.Double;
				autoYValueType = true;
			}
			indexedXValueType = xValueType;
			totalYvalue = double.NaN;
			if (chart == null)
			{
				chart = (Chart)serviceContainer.GetService(typeof(Chart));
			}
			if (chart != null && chart.chartPicture.SuppressExceptions)
			{
				Axis axis = chart.ChartAreas[ChartArea].GetAxis(AxisName.Y, YAxisType, YSubAxisName);
				Axis axis2 = chart.ChartAreas[ChartArea].GetAxis(AxisName.X, XAxisType, XSubAxisName);
				foreach (DataPoint point3 in Points)
				{
					if (axis2.Logarithmic && point3.XValue <= 0.0)
					{
						point3.XValue = 1.0;
						for (int j = 0; j < point3.YValues.Length; j++)
						{
							point3.YValues[j] = 0.0;
						}
						point3.Empty = true;
					}
					for (int k = 0; k < point3.YValues.Length; k++)
					{
						if (axis.Logarithmic && point3.YValues[k] <= 0.0)
						{
							point3.YValues[k] = 1.0;
							point3.Empty = true;
						}
						if (double.IsNaN(point3.YValues[k]))
						{
							point3.YValues[k] = 0.0;
							point3.Empty = true;
						}
					}
				}
			}
			ErrorBarChart.GetDataFromLinkedSeries(this, serviceContainer);
			ErrorBarChart.CalculateErrorAmount(this);
			BoxPlotChart.CalculateBoxPlotFromLinkedSeries(this, serviceContainer);
			RenkoChart.PrepareData(this, serviceContainer);
			ThreeLineBreakChart.PrepareData(this, serviceContainer);
			KagiChart.PrepareData(this, serviceContainer);
			PointAndFigureChart.PrepareData(this, serviceContainer);
			PieChart.PrepareData(this, serviceContainer);
			if (applyPaletteColors)
			{
				ApplyPaletteColors();
			}
			TraceWrite("ChartData", SR.TraceMessageEndPreparingChartDataInSeries(Name));
		}

		internal void Invalidate(bool invalidateAreaOnly, bool invalidateLegend)
		{
		}
	}
}
