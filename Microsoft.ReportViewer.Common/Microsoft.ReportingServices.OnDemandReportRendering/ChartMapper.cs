using Microsoft.Reporting.Chart.Helpers;
using Microsoft.Reporting.Chart.WebForms;
using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal class ChartMapper : MapperBase, IChartMapper, IDVMappingLayer, IDisposable
	{
		private class ChartAreaInfo
		{
			public List<SeriesInfo> SeriesInfoList = new List<SeriesInfo>();

			public List<string> CategoryAxesScalar;

			public List<Microsoft.Reporting.Chart.WebForms.Axis> CategoryAxesAutoMargin;

			public bool PrimaryAxisSet;

			public bool SecondaryAxisSet;
		}

		private class SeriesInfo
		{
			public ChartAreaInfo ChartAreaInfo;

			public ChartAxis ChartCategoryAxis;

			public Series Series;

			public List<Series> DerivedSeries;

			public ChartSeries ChartSeries;

			public ChartMember SeriesGrouping;

			public bool XValueSet;

			public bool XValueSetFailed;

			public List<DataPoint> NullXValuePoints = new List<DataPoint>();

			public bool IsLine;

			public bool IsExploded;

			public bool IsRange;

			public bool IsBubble;

			public bool IsAttachedToScalarAxis;

			public bool? IsDataPointColorEmpty;

			public bool IsDataPointHatchDefined;

			public DataPointBackgroundImageInfoCollection DataPointBackgroundImageInfoCollection = new DataPointBackgroundImageInfoCollection();

			public DataPoint DefaultDataPointAppearance;

			public bool IsGradientPerDataPointSupported = true;

			public bool IsGradientSupported = true;

			public GradientType? BackGradientType;

			public Color? Color;

			public Color? BackGradientEndColor;
		}

		private class DataPointBackgroundImageInfoCollection : List<DataPointBackgroundImageInfo>
		{
			public void Initialize(ChartSeries chartSeries)
			{
				Clear();
				for (int i = 0; i < chartSeries.Count; i++)
				{
					DataPointBackgroundImageInfo dataPointBackgroundImageInfo = new DataPointBackgroundImageInfo();
					dataPointBackgroundImageInfo.Initialize(chartSeries[i]);
					Add(dataPointBackgroundImageInfo);
				}
			}
		}

		private class DataPointBackgroundImageInfo
		{
			public BackgroundImageInfo DataPointBackgroundImage = new BackgroundImageInfo();

			public BackgroundImageInfo MarkerBackgroundImage = new BackgroundImageInfo();

			public void Initialize(ChartDataPoint chartDataPoint)
			{
				DataPointBackgroundImage.Initialize(chartDataPoint.Style);
				if (chartDataPoint.Marker != null)
				{
					MarkerBackgroundImage.Initialize(chartDataPoint.Marker.Style);
				}
			}
		}

		private class BackgroundImageInfo
		{
			public bool CanShareBackgroundImage;

			public string SharedBackgroundImageName;

			public void Initialize(Style style)
			{
				if (style != null && style.BackgroundImage != null && style.BackgroundImage.MIMEType != null && style.BackgroundImage.Value != null)
				{
					CanShareBackgroundImage = (!style.BackgroundImage.MIMEType.IsExpression && !style.BackgroundImage.Value.IsExpression);
				}
			}
		}

		private class Hatcher
		{
			private int m_current = -1;

			private static ChartHatchStyle[] m_hatchArray = new ChartHatchStyle[54]
			{
				ChartHatchStyle.BackwardDiagonal,
				ChartHatchStyle.Cross,
				ChartHatchStyle.DarkDownwardDiagonal,
				ChartHatchStyle.DarkHorizontal,
				ChartHatchStyle.DarkUpwardDiagonal,
				ChartHatchStyle.DarkVertical,
				ChartHatchStyle.DashedDownwardDiagonal,
				ChartHatchStyle.DashedHorizontal,
				ChartHatchStyle.DashedUpwardDiagonal,
				ChartHatchStyle.DashedVertical,
				ChartHatchStyle.DiagonalBrick,
				ChartHatchStyle.DiagonalCross,
				ChartHatchStyle.Divot,
				ChartHatchStyle.DottedDiamond,
				ChartHatchStyle.DottedGrid,
				ChartHatchStyle.ForwardDiagonal,
				ChartHatchStyle.Horizontal,
				ChartHatchStyle.HorizontalBrick,
				ChartHatchStyle.LargeCheckerBoard,
				ChartHatchStyle.LargeConfetti,
				ChartHatchStyle.LargeGrid,
				ChartHatchStyle.LightDownwardDiagonal,
				ChartHatchStyle.LightHorizontal,
				ChartHatchStyle.LightUpwardDiagonal,
				ChartHatchStyle.LightVertical,
				ChartHatchStyle.NarrowHorizontal,
				ChartHatchStyle.NarrowVertical,
				ChartHatchStyle.OutlinedDiamond,
				ChartHatchStyle.Percent05,
				ChartHatchStyle.Percent10,
				ChartHatchStyle.Percent20,
				ChartHatchStyle.Percent25,
				ChartHatchStyle.Percent30,
				ChartHatchStyle.Percent40,
				ChartHatchStyle.Percent50,
				ChartHatchStyle.Percent60,
				ChartHatchStyle.Percent70,
				ChartHatchStyle.Percent75,
				ChartHatchStyle.Percent80,
				ChartHatchStyle.Percent90,
				ChartHatchStyle.Plaid,
				ChartHatchStyle.Shingle,
				ChartHatchStyle.SmallCheckerBoard,
				ChartHatchStyle.SmallConfetti,
				ChartHatchStyle.SmallGrid,
				ChartHatchStyle.SolidDiamond,
				ChartHatchStyle.Sphere,
				ChartHatchStyle.Trellis,
				ChartHatchStyle.Vertical,
				ChartHatchStyle.Wave,
				ChartHatchStyle.Weave,
				ChartHatchStyle.WideDownwardDiagonal,
				ChartHatchStyle.WideUpwardDiagonal,
				ChartHatchStyle.ZigZag
			};

			internal ChartHatchStyle Current
			{
				get
				{
					m_current = (m_current + 1) % m_hatchArray.Length;
					return m_hatchArray[m_current];
				}
			}
		}

		private class AutoMarker
		{
			private int m_current;

			private bool m_currentUsed;

			private static MarkerStyle[] m_markerArray = new MarkerStyle[9]
			{
				MarkerStyle.Square,
				MarkerStyle.Circle,
				MarkerStyle.Diamond,
				MarkerStyle.Triangle,
				MarkerStyle.Cross,
				MarkerStyle.Star4,
				MarkerStyle.Star5,
				MarkerStyle.Star6,
				MarkerStyle.Star10
			};

			internal MarkerStyle Current
			{
				get
				{
					m_currentUsed = true;
					return m_markerArray[m_current];
				}
			}

			internal void MoveNext()
			{
				if (m_currentUsed)
				{
					m_currentUsed = false;
					m_current = (m_current + 1) % m_markerArray.Length;
				}
			}
		}

		private static class FormulaHelper
		{
			public const string PARAM_NAME_START_FROM_FIRST = "StartFromFirst";

			public const string PARAM_NAME_OUTPUT = "Output";

			public const string PARAM_NAME_INPUT = "Input";

			public const string PARAM_NAME_PERIOD = "Period";

			public const string PARAM_DEFAULT_PERIOD = "2";

			public const string PARAM_NAME_SHORT_PERIOD = "ShortPeriod";

			public const string PARAM_DEFAULT_SHORT_PERIOD = "12";

			public const string PARAM_NAME_LONG_PERIOD = "LongPeriod";

			public const string PARAM_DEFAULT_LONG_PERIOD = "26";

			public const string PARAM_NAME_DEVIATION = "Deviation";

			public const string PARAM_DEFAULT_DEVIATION = "2";

			public const string PARAM_NAME_SHIFT = "Shift";

			public const string PARAM_DEFAULT_SHIFT = "7";

			public const string OUTPUT_SERIES_KEYWORD = "#OUTPUTSERIES";

			public const string DERIVED_SERIES_NAME_SUFFIX = "_Formula";

			public const string NEW_CHART_AREA_NAME = "#NewChartArea";

			internal static void RenderFormulaParameters(ChartFormulaParameterCollection chartFormulaParameters, ChartSeriesFormula formula, string sourceSeriesName, string derivedSeriesName, out string formulaParameters, out string inputValues, out string outputValues, out bool startFromFirst)
			{
				Dictionary<string, string> parameters = new Dictionary<string, string>();
				GetParameters(chartFormulaParameters, parameters);
				formulaParameters = ConstructFormulaParameters(parameters, formula);
				outputValues = GetOutputValues(parameters, formula, derivedSeriesName);
				inputValues = GetInputValues(parameters, sourceSeriesName);
				if (GetParameter(parameters, "StartFromFirst").Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase))
				{
					startFromFirst = true;
				}
				else
				{
					startFromFirst = false;
				}
			}

			internal static string GetInputValues(Dictionary<string, string> parameters, string sourceSeriesName)
			{
				string text = GetParameter(parameters, "Input");
				if (text == "")
				{
					text = sourceSeriesName;
				}
				return text;
			}

			internal static string GetOutputValues(Dictionary<string, string> parameters, ChartSeriesFormula formula, string derivedSeriesName)
			{
				string parameter = GetParameter(parameters, "Output");
				if (parameter != "")
				{
					return parameter.Replace("#OUTPUTSERIES", derivedSeriesName);
				}
				if (formula == ChartSeriesFormula.BollingerBands || (uint)(formula - 6) <= 1u)
				{
					return derivedSeriesName + ":Y, " + derivedSeriesName + ":Y2";
				}
				return derivedSeriesName;
			}

			internal static string ConstructFormulaParameters(Dictionary<string, string> parameters, ChartSeriesFormula formula)
			{
				string formulaParameters = "";
				switch (formula)
				{
				case ChartSeriesFormula.BollingerBands:
					AppendParameter(parameters, "Period", "2", ref formulaParameters);
					AppendParameter(parameters, "Deviation", "2", ref formulaParameters);
					break;
				case ChartSeriesFormula.MovingAverage:
					AppendParameter(parameters, "Period", "2", ref formulaParameters);
					break;
				case ChartSeriesFormula.ExponentialMovingAverage:
					AppendParameter(parameters, "Period", "2", ref formulaParameters);
					break;
				case ChartSeriesFormula.TriangularMovingAverage:
					AppendParameter(parameters, "Period", "2", ref formulaParameters);
					break;
				case ChartSeriesFormula.WeightedMovingAverage:
					AppendParameter(parameters, "Period", "2", ref formulaParameters);
					break;
				case ChartSeriesFormula.Envelopes:
					AppendParameter(parameters, "Period", "2", ref formulaParameters);
					AppendParameter(parameters, "Shift", "7", ref formulaParameters);
					break;
				case ChartSeriesFormula.MACD:
					AppendParameter(parameters, "ShortPeriod", "12", ref formulaParameters);
					AppendParameter(parameters, "LongPeriod", "26", ref formulaParameters);
					break;
				case ChartSeriesFormula.DetrendedPriceOscillator:
					AppendParameter(parameters, "Period", "2", ref formulaParameters);
					break;
				case ChartSeriesFormula.RateOfChange:
					AppendParameter(parameters, "Period", "10", ref formulaParameters);
					break;
				case ChartSeriesFormula.RelativeStrengthIndex:
					AppendParameter(parameters, "Period", "10", ref formulaParameters);
					break;
				case ChartSeriesFormula.StandardDeviation:
					AppendParameter(parameters, "Period", "2", ref formulaParameters);
					break;
				case ChartSeriesFormula.TRIX:
					AppendParameter(parameters, "Period", "2", ref formulaParameters);
					break;
				}
				return formulaParameters;
			}

			private static void GetParameters(ChartFormulaParameterCollection chartFormulaParameters, Dictionary<string, string> parameters)
			{
				if (chartFormulaParameters == null)
				{
					return;
				}
				foreach (ChartFormulaParameter chartFormulaParameter in chartFormulaParameters)
				{
					GetParameter(chartFormulaParameter, out string name, out string value);
					parameters.Add(name, value);
				}
			}

			private static void GetParameter(ChartFormulaParameter chartFormulaParameter, out string name, out string value)
			{
				name = "";
				value = "";
				if (chartFormulaParameter.Name != null)
				{
					name = chartFormulaParameter.Name;
				}
				if (chartFormulaParameter.Value != null)
				{
					object value2 = chartFormulaParameter.Value.IsExpression ? chartFormulaParameter.Instance.Value : chartFormulaParameter.Value.Value;
					value = Convert.ToString(value2, CultureInfo.InvariantCulture);
				}
			}

			private static void AppendParameter(Dictionary<string, string> parameters, string parameterName, string defaultValue, ref string formulaParameters)
			{
				if (formulaParameters != string.Empty)
				{
					formulaParameters += ",";
				}
				string text = null;
				if (parameters.ContainsKey(parameterName))
				{
					text = parameters[parameterName];
				}
				if (text != null)
				{
					formulaParameters += text;
				}
				else
				{
					formulaParameters += defaultValue;
				}
			}

			private static string GetParameter(Dictionary<string, string> parameters, string parameterName)
			{
				string text = null;
				if (parameters.ContainsKey(parameterName))
				{
					text = parameters[parameterName];
				}
				if (text == null)
				{
					return "";
				}
				return text;
			}

			internal static string GetDerivedSeriesName(string sourceSeriesName)
			{
				return sourceSeriesName + "_Formula";
			}

			internal static bool IsNewAreaRequired(ChartSeriesFormula formula)
			{
				if (formula == ChartSeriesFormula.MACD || (uint)(formula - 8) <= 4u)
				{
					return true;
				}
				return false;
			}

			internal static bool ShouldSendDerivedSeriesBack(SeriesChartType type)
			{
				if (type != SeriesChartType.Line)
				{
					return type != SeriesChartType.Spline;
				}
				return false;
			}
		}

		private class TraceContext : ITraceContext
		{
			private DateTime m_startTime;

			private DateTime m_lastOperation;

			public bool TraceEnabled => true;

			public TraceContext()
			{
				m_startTime = (m_lastOperation = DateTime.Now);
			}

			public void Write(string category, string message)
			{
				RSTrace.ProcessingTracer.Trace(category + "; " + message + "; " + (DateTime.Now - m_startTime).TotalMilliseconds + "; " + (DateTime.Now - m_lastOperation).TotalMilliseconds);
				m_lastOperation = DateTime.Now;
			}
		}

		private Chart m_chart;

		private ActionInfoWithDynamicImageMapCollection m_actions = new ActionInfoWithDynamicImageMapCollection();

		private Microsoft.Reporting.Chart.WebForms.Chart m_coreChart;

		private bool m_multiColumn;

		private bool m_multiRow;

		private Formatter m_formatter;

		private Dictionary<string, ChartAreaInfo> m_chartAreaInfoDictionary = new Dictionary<string, ChartAreaInfo>();

		private Hatcher m_hatcher;

		private AutoMarker m_autoMarker;

		private static string m_legendTextSeparator = " - ";

		private static string m_defaulChartAreaName = "Default";

		private static string m_imagePrefix = "KatmaiChartMapperImage";

		private static string m_pieAutoAxisLabelsName = "AutoAxisLabels";

		private static string m_defaultMarkerSizeString = "3.75pt";

		private static string m_defaultCalloutLineWidthString = "0.75pt";

		private static string m_defaultMaxMovingDistanceString = "23pt";

		private static ReportSize m_defaultMarkerSize = new ReportSize(m_defaultMarkerSizeString);

		private static ReportSize m_defaultCalloutLineWidth = new ReportSize(m_defaultCalloutLineWidthString);

		private static ReportSize m_defaultMaxMovingDistance = new ReportSize(m_defaultMaxMovingDistanceString);

		private static LabelsAutoFitStyles m_defaultLabelsAngleStep = LabelsAutoFitStyles.LabelsAngleStep90;

		private static ChartDashStyle m_defaultCoreDataPointBorderStyle = ChartDashStyle.Solid;

		private static int m_defaultCoreDataPointBorderWidth = 1;

		public ChartMapper(Chart chart, string defaultFontFamily)
			: base(defaultFontFamily)
		{
			m_chart = chart;
		}

		public void RenderChart()
		{
			try
			{
				if (m_chart != null)
				{
					InitializeChart();
					SetChartProperties();
					RenderChartStyle();
					RenderBorderSkin();
					RenderPalettes();
					RenderChartAreas();
					RenderLegends();
					RenderTitles();
					RenderAnnotations();
					RenderData();
					if (IsChartEmpty())
					{
						m_coreChart.Series.Clear();
						RenderNoDataMessage();
					}
					m_coreChart.SuppressExceptions = true;
				}
			}
			catch (RSException)
			{
				throw;
			}
			catch (Exception ex2)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(ex2))
				{
					throw;
				}
				throw new RenderingObjectModelException(ex2);
			}
		}

		public Stream GetCoreXml()
		{
			try
			{
				m_coreChart.Serializer.Content = SerializationContents.All;
				m_coreChart.Serializer.NonSerializableContent = "";
				MemoryStream memoryStream = new MemoryStream();
				m_coreChart.Serializer.Save(memoryStream);
				memoryStream.Position = 0L;
				return memoryStream;
			}
			catch (Exception ex)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(ex))
				{
					throw;
				}
				Global.Tracer.Trace(TraceLevel.Verbose, ex.Message);
				return null;
			}
		}

		public Stream GetImage(DynamicImageInstance.ImageType imageType)
		{
			try
			{
				if (m_coreChart == null)
				{
					return null;
				}
				ChartImageFormat format = ChartImageFormat.Png;
				Stream stream = null;
				switch (imageType)
				{
				case DynamicImageInstance.ImageType.EMF:
					format = ChartImageFormat.EmfPlus;
					stream = m_chart.RenderingContext.OdpContext.CreateStreamCallback(m_chart.Name, "emf", null, "image/emf", willSeek: true, StreamOper.CreateOnly);
					break;
				case DynamicImageInstance.ImageType.PNG:
					format = ChartImageFormat.Png;
					stream = new MemoryStream();
					break;
				}
				Microsoft.Reporting.Chart.WebForms.Chart coreChart = m_coreChart;
				coreChart.FormatNumberHandler = (FormatNumberHandler)Delegate.Combine(coreChart.FormatNumberHandler, new FormatNumberHandler(FormatNumber));
				m_coreChart.CustomizeLegend += AdjustSeriesInLegend;
				m_coreChart.ImageResolution = base.DpiX;
				m_coreChart.Save(stream, format);
				stream.Position = 0L;
				return stream;
			}
			catch (RSException)
			{
				throw;
			}
			catch (Exception ex2)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(ex2))
				{
					throw;
				}
				throw new RenderingObjectModelException(ex2);
			}
		}

		private string FormatNumber(object sender, double value, string format, ChartValueTypes valueType, int elementId, ChartElementType elementType)
		{
			if (m_formatter == null)
			{
				m_formatter = new Formatter(m_chart.ChartDef.StyleClass, m_chart.RenderingContext.OdpContext, ObjectType.Chart, m_chart.Name);
			}
			bool addDateTimeOffsetSuffix = false;
			if (format.Length == 0)
			{
				switch (valueType)
				{
				case ChartValueTypes.DateTime:
				case ChartValueTypes.Date:
				case ChartValueTypes.DateTimeOffset:
					format = "d";
					break;
				case ChartValueTypes.Time:
					format = "t";
					break;
				}
				addDateTimeOffsetSuffix = (valueType == ChartValueTypes.DateTimeOffset);
			}
			TypeCode typeCode = GetTypeCode(valueType);
			object value2 = (typeCode != TypeCode.DateTime) ? ((object)value) : ((object)DateTime.FromOADate(value));
			return m_formatter.FormatValue(value2, format, typeCode, addDateTimeOffsetSuffix);
		}

		private TypeCode GetTypeCode(ChartValueTypes chartValueType)
		{
			if ((uint)(chartValueType - 8) <= 3u)
			{
				return TypeCode.DateTime;
			}
			return TypeCode.Double;
		}

		private ImageMapArea.ImageMapAreaShape GetMapAreaShape(MapAreaShape shape)
		{
			if (shape == MapAreaShape.Rectangle)
			{
				return ImageMapArea.ImageMapAreaShape.Rectangle;
			}
			if (MapAreaShape.Circle == shape)
			{
				return ImageMapArea.ImageMapAreaShape.Circle;
			}
			return ImageMapArea.ImageMapAreaShape.Polygon;
		}

		public ActionInfoWithDynamicImageMapCollection GetImageMaps()
		{
			return MappingHelper.GetImageMaps(GetMapAreaInfoList(), m_actions, m_chart);
		}

		internal IEnumerable<MappingHelper.MapAreaInfo> GetMapAreaInfoList()
		{
			foreach (MapArea mapArea in m_coreChart.MapAreas)
			{
				yield return new MappingHelper.MapAreaInfo(mapArea.ToolTip, ((IMapAreaAttributes)mapArea).Tag, GetMapAreaShape(mapArea.Shape), mapArea.Coordinates);
			}
		}

		private void InitializeChart()
		{
			m_coreChart = new Microsoft.Reporting.Chart.WebForms.Chart();
			if (RSTrace.ProcessingTracer.TraceVerbose)
			{
				((TraceManager)m_coreChart.GetService(typeof(TraceManager))).TraceContext = new TraceContext();
			}
			m_coreChart.ChartAreas.Clear();
			m_coreChart.Series.Clear();
			m_coreChart.Titles.Clear();
			m_coreChart.Legends.Clear();
			m_coreChart.Annotations.Clear();
			OnPostInitialize();
		}

		private void SetChartProperties()
		{
			int width = 300;
			int height = 300;
			if (base.WidthOverrideInPixels.HasValue)
			{
				width = base.WidthOverrideInPixels.Value;
			}
			else if (m_chart.DynamicWidth != null)
			{
				if (!m_chart.DynamicWidth.IsExpression)
				{
					if (m_chart.DynamicWidth.Value != null)
					{
						width = MappingHelper.ToIntPixels(m_chart.DynamicWidth.Value, base.DpiX);
					}
				}
				else if (((ChartInstance)m_chart.Instance).DynamicWidth != null)
				{
					width = MappingHelper.ToIntPixels(((ChartInstance)m_chart.Instance).DynamicWidth, base.DpiX);
				}
			}
			m_coreChart.Width = width;
			if (base.HeightOverrideInPixels.HasValue)
			{
				height = base.HeightOverrideInPixels.Value;
			}
			else if (m_chart.DynamicHeight != null)
			{
				if (!m_chart.DynamicHeight.IsExpression)
				{
					if (m_chart.DynamicHeight.Value != null)
					{
						height = MappingHelper.ToIntPixels(m_chart.DynamicHeight.Value, base.DpiY);
					}
				}
				else if (((ChartInstance)m_chart.Instance).DynamicHeight != null)
				{
					height = MappingHelper.ToIntPixels(((ChartInstance)m_chart.Instance).DynamicHeight, base.DpiY);
				}
			}
			m_coreChart.Height = height;
		}

		private void RenderNoDataMessage()
		{
			if (m_chart.NoDataMessage != null)
			{
				Title title = new Title();
				m_coreChart.Titles.Add(title);
				RenderTitle(m_chart.NoDataMessage, title);
			}
		}

		private void RenderPalettes()
		{
			RenderStandardPalettes();
			RenderCustomPalette();
			RenderPaletteHatchBehavior();
		}

		private void RenderStandardPalettes()
		{
			if (m_chart.Palette == null)
			{
				m_coreChart.Palette = ChartColorPalette.Default;
				return;
			}
			ChartPalette chartPalette = ChartPalette.BrightPastel;
			switch (m_chart.Palette.IsExpression ? ((ChartInstance)m_chart.Instance).Palette : m_chart.Palette.Value)
			{
			case ChartPalette.Default:
				m_coreChart.Palette = ChartColorPalette.Default;
				break;
			case ChartPalette.EarthTones:
				m_coreChart.Palette = ChartColorPalette.EarthTones;
				break;
			case ChartPalette.Excel:
				m_coreChart.Palette = ChartColorPalette.Excel;
				break;
			case ChartPalette.GrayScale:
				m_coreChart.Palette = ChartColorPalette.Grayscale;
				break;
			case ChartPalette.Light:
				m_coreChart.Palette = ChartColorPalette.Light;
				break;
			case ChartPalette.Pastel:
				m_coreChart.Palette = ChartColorPalette.Pastel;
				break;
			case ChartPalette.SemiTransparent:
				m_coreChart.Palette = ChartColorPalette.Semitransparent;
				break;
			case ChartPalette.Berry:
				m_coreChart.Palette = ChartColorPalette.Berry;
				break;
			case ChartPalette.BrightPastel:
				m_coreChart.Palette = ChartColorPalette.BrightPastel;
				break;
			case ChartPalette.Chocolate:
				m_coreChart.Palette = ChartColorPalette.Chocolate;
				break;
			case ChartPalette.Custom:
				m_coreChart.Palette = ChartColorPalette.None;
				break;
			case ChartPalette.Fire:
				m_coreChart.Palette = ChartColorPalette.Fire;
				break;
			case ChartPalette.SeaGreen:
				m_coreChart.Palette = ChartColorPalette.SeaGreen;
				break;
			case ChartPalette.Pacific:
				m_coreChart.Palette = ChartColorPalette.Pacific;
				break;
			case ChartPalette.PacificLight:
				m_coreChart.Palette = ChartColorPalette.PacificLight;
				break;
			case ChartPalette.PacificSemiTransparent:
				m_coreChart.Palette = ChartColorPalette.PacificSemiTransparent;
				break;
			}
		}

		private void RenderPaletteHatchBehavior()
		{
			if (m_chart.PaletteHatchBehavior != null)
			{
				ReportEnumProperty<PaletteHatchBehavior> paletteHatchBehavior = m_chart.PaletteHatchBehavior;
				PaletteHatchBehavior paletteHatchBehavior2 = PaletteHatchBehavior.None;
				paletteHatchBehavior2 = (paletteHatchBehavior.IsExpression ? ((ChartInstance)m_chart.Instance).PaletteHatchBehavior : paletteHatchBehavior.Value);
				if (paletteHatchBehavior2 == PaletteHatchBehavior.Always)
				{
					m_hatcher = new Hatcher();
				}
			}
		}

		private void RenderCustomPalette()
		{
			if (m_chart.CustomPaletteColors == null || m_chart.CustomPaletteColors.Count == 0 || m_coreChart.Palette != 0)
			{
				return;
			}
			Color[] array = new Color[m_chart.CustomPaletteColors.Count];
			Color color = Color.Empty;
			for (int i = 0; i < m_chart.CustomPaletteColors.Count; i++)
			{
				ChartCustomPaletteColor chartCustomPaletteColor = m_chart.CustomPaletteColors[i];
				ReportColorProperty color2 = chartCustomPaletteColor.Color;
				if (!color2.IsExpression)
				{
					if (MappingHelper.GetColorFromReportColorProperty(color2, ref color))
					{
						array[i] = color;
					}
				}
				else
				{
					array[i] = chartCustomPaletteColor.Instance.Color.ToColor();
				}
			}
			m_coreChart.PaletteCustomColors = array;
		}

		private void RenderChartStyle()
		{
			Border border = null;
			m_coreChart.BackColor = Color.Transparent;
			Style style = m_chart.Style;
			if (style != null)
			{
				StyleInstance style2 = m_chart.Instance.Style;
				if (MappingHelper.IsStylePropertyDefined(m_chart.Style.BackgroundColor))
				{
					m_coreChart.BackColor = MappingHelper.GetStyleBackgroundColor(style, style2);
				}
				if (MappingHelper.IsStylePropertyDefined(m_chart.Style.BackgroundGradientEndColor))
				{
					m_coreChart.BackGradientEndColor = MappingHelper.GetStyleBackGradientEndColor(style, style2);
				}
				if (MappingHelper.IsStylePropertyDefined(m_chart.Style.BackgroundGradientType))
				{
					m_coreChart.BackGradientType = GetGradientType(MappingHelper.GetStyleBackGradientType(style, style2));
				}
				if (MappingHelper.IsStylePropertyDefined(m_chart.Style.BackgroundHatchType))
				{
					m_coreChart.BackHatchStyle = GetHatchType(MappingHelper.GetStyleBackgroundHatchType(style, style2));
				}
				m_coreChart.RightToLeft = MappingHelper.GetStyleDirection(m_chart.Style, m_chart.Instance.Style);
				RenderChartBackgroundImage(m_chart.Style.BackgroundImage);
				border = m_chart.Style.Border;
			}
			if (m_coreChart.BackColor.A != byte.MaxValue)
			{
				m_coreChart.AntiAlias = AntiAlias.Off;
			}
			if (m_chart.SpecialBorderHandling)
			{
				RenderChartBorder(border);
			}
		}

		private void RenderBorderSkin()
		{
			if (m_chart.BorderSkin != null)
			{
				RenderBorderSkinStyle(m_chart.BorderSkin);
				RenderBorderSkinType(m_chart.BorderSkin);
			}
		}

		private void RenderBorderSkinType(ChartBorderSkin borderSkin)
		{
			if (borderSkin.BorderSkinType != null)
			{
				if (!borderSkin.BorderSkinType.IsExpression)
				{
					_ = borderSkin.BorderSkinType.Value;
				}
				else
				{
					_ = borderSkin.Instance.BorderSkinType;
				}
				BorderSkinStyle skinStyle = BorderSkinStyle.None;
				switch (borderSkin.Instance.BorderSkinType)
				{
				case ChartBorderSkinType.Emboss:
					skinStyle = BorderSkinStyle.Emboss;
					break;
				case ChartBorderSkinType.FrameThin1:
					skinStyle = BorderSkinStyle.FrameThin1;
					break;
				case ChartBorderSkinType.FrameThin2:
					skinStyle = BorderSkinStyle.FrameThin2;
					break;
				case ChartBorderSkinType.FrameThin3:
					skinStyle = BorderSkinStyle.FrameThin3;
					break;
				case ChartBorderSkinType.FrameThin4:
					skinStyle = BorderSkinStyle.FrameThin4;
					break;
				case ChartBorderSkinType.FrameThin5:
					skinStyle = BorderSkinStyle.FrameThin5;
					break;
				case ChartBorderSkinType.FrameThin6:
					skinStyle = BorderSkinStyle.FrameThin6;
					break;
				case ChartBorderSkinType.FrameTitle1:
					skinStyle = BorderSkinStyle.FrameTitle1;
					break;
				case ChartBorderSkinType.FrameTitle2:
					skinStyle = BorderSkinStyle.FrameTitle2;
					break;
				case ChartBorderSkinType.FrameTitle3:
					skinStyle = BorderSkinStyle.FrameTitle3;
					break;
				case ChartBorderSkinType.FrameTitle4:
					skinStyle = BorderSkinStyle.FrameTitle4;
					break;
				case ChartBorderSkinType.FrameTitle5:
					skinStyle = BorderSkinStyle.FrameTitle5;
					break;
				case ChartBorderSkinType.FrameTitle6:
					skinStyle = BorderSkinStyle.FrameTitle6;
					break;
				case ChartBorderSkinType.FrameTitle7:
					skinStyle = BorderSkinStyle.FrameTitle7;
					break;
				case ChartBorderSkinType.FrameTitle8:
					skinStyle = BorderSkinStyle.FrameTitle8;
					break;
				case ChartBorderSkinType.None:
					skinStyle = BorderSkinStyle.None;
					break;
				case ChartBorderSkinType.Raised:
					skinStyle = BorderSkinStyle.Raised;
					break;
				case ChartBorderSkinType.Sunken:
					skinStyle = BorderSkinStyle.Sunken;
					break;
				}
				m_coreChart.BorderSkin.SkinStyle = skinStyle;
			}
		}

		private void RenderBorderSkinStyle(ChartBorderSkin chartBorderSkin)
		{
			Style style = chartBorderSkin.Style;
			if (style != null)
			{
				StyleInstance style2 = chartBorderSkin.Instance.Style;
				BorderSkinAttributes borderSkin = m_coreChart.BorderSkin;
				if (MappingHelper.IsStylePropertyDefined(chartBorderSkin.Style.Color))
				{
					borderSkin.PageColor = MappingHelper.GetStyleColor(style, style2);
				}
				if (MappingHelper.IsStylePropertyDefined(chartBorderSkin.Style.BackgroundColor))
				{
					borderSkin.FrameBackColor = MappingHelper.GetStyleBackgroundColor(style, style2);
				}
				if (MappingHelper.IsStylePropertyDefined(chartBorderSkin.Style.BackgroundGradientEndColor))
				{
					borderSkin.FrameBackGradientEndColor = MappingHelper.GetStyleBackGradientEndColor(style, style2);
				}
				if (MappingHelper.IsStylePropertyDefined(chartBorderSkin.Style.BackgroundGradientType))
				{
					borderSkin.FrameBackGradientType = GetGradientType(MappingHelper.GetStyleBackGradientType(style, style2));
				}
				if (MappingHelper.IsStylePropertyDefined(chartBorderSkin.Style.BackgroundHatchType))
				{
					borderSkin.FrameBackHatchStyle = GetHatchType(MappingHelper.GetStyleBackgroundHatchType(style, style2));
				}
				RenderBorderSkinBorder(chartBorderSkin.Style.Border, borderSkin);
				if (style.BackgroundImage != null)
				{
					RenderBorderSkinBackgroundImage(style.BackgroundImage, borderSkin);
				}
			}
		}

		private void RenderChartAreas()
		{
			if (m_chart.ChartAreas == null || m_chart.ChartAreas.Count == 0)
			{
				m_coreChart.ChartAreas.Add(m_defaulChartAreaName);
				return;
			}
			foreach (ChartArea chartArea in m_chart.ChartAreas)
			{
				RenderChartArea(chartArea);
			}
		}

		private void RenderChartArea(ChartArea chartArea)
		{
			Microsoft.Reporting.Chart.WebForms.ChartArea chartArea2 = new Microsoft.Reporting.Chart.WebForms.ChartArea();
			m_coreChart.ChartAreas.Add(chartArea2);
			SetChartAreaProperties(chartArea, chartArea2);
			RenderElementPosition(chartArea.ChartElementPosition, chartArea2.Position);
			RenderElementPosition(chartArea.ChartInnerPlotPosition, chartArea2.InnerPlotPosition);
			RenderChartAreaStyle(chartArea, chartArea2);
			if (!m_chartAreaInfoDictionary.ContainsKey(chartArea2.Name))
			{
				m_chartAreaInfoDictionary.Add(chartArea2.Name, new ChartAreaInfo());
			}
			RenderAxes(chartArea, chartArea2);
			Render3DProperties(chartArea.ThreeDProperties, chartArea2.Area3DStyle);
		}

		private void SetChartAreaProperties(ChartArea chartArea, Microsoft.Reporting.Chart.WebForms.ChartArea area)
		{
			RenderAlignType(chartArea.ChartAlignType, area);
			if (chartArea.Name != null)
			{
				area.Name = chartArea.Name;
			}
			else
			{
				area.Name = m_defaulChartAreaName;
			}
			if (chartArea.AlignOrientation != null)
			{
				if (!chartArea.AlignOrientation.IsExpression)
				{
					area.AlignOrientation = GetAreaAlignOrientation(chartArea.AlignOrientation.Value);
				}
				else
				{
					area.AlignOrientation = GetAreaAlignOrientation(chartArea.Instance.AlignOrientation);
				}
			}
			else
			{
				area.AlignOrientation = AreaAlignOrientations.None;
			}
			if (chartArea.AlignWithChartArea != null)
			{
				area.AlignWithChartArea = chartArea.AlignWithChartArea;
			}
			if (chartArea.EquallySizedAxesFont != null)
			{
				if (!chartArea.EquallySizedAxesFont.IsExpression)
				{
					area.EquallySizedAxesFont = chartArea.EquallySizedAxesFont.Value;
				}
				else
				{
					area.EquallySizedAxesFont = chartArea.Instance.EquallySizedAxesFont;
				}
			}
			if (chartArea.Hidden != null)
			{
				if (!chartArea.Hidden.IsExpression)
				{
					area.Visible = !chartArea.Hidden.Value;
				}
				else
				{
					area.Visible = !chartArea.Instance.Hidden;
				}
			}
		}

		private AreaAlignOrientations GetAreaAlignOrientation(ChartAreaAlignOrientations chartAreaOrientation)
		{
			switch (chartAreaOrientation)
			{
			case ChartAreaAlignOrientations.All:
				return AreaAlignOrientations.All;
			case ChartAreaAlignOrientations.Horizontal:
				return AreaAlignOrientations.Horizontal;
			case ChartAreaAlignOrientations.Vertical:
				return AreaAlignOrientations.Vertical;
			default:
				return AreaAlignOrientations.None;
			}
		}

		private void RenderAlignType(ChartAlignType chartAlignType, Microsoft.Reporting.Chart.WebForms.ChartArea area)
		{
			area.AlignType = AreaAlignTypes.None;
			if (chartAlignType == null)
			{
				return;
			}
			if (chartAlignType.AxesView != null)
			{
				if (!chartAlignType.AxesView.IsExpression)
				{
					if (chartAlignType.AxesView.Value)
					{
						area.AlignType |= AreaAlignTypes.AxesView;
					}
				}
				else if (chartAlignType.Instance.AxesView)
				{
					area.AlignType |= AreaAlignTypes.AxesView;
				}
			}
			if (chartAlignType.Cursor != null)
			{
				if (!chartAlignType.Cursor.IsExpression)
				{
					if (chartAlignType.Cursor.Value)
					{
						area.AlignType |= AreaAlignTypes.Cursor;
					}
				}
				else if (chartAlignType.Instance.Cursor)
				{
					area.AlignType |= AreaAlignTypes.Cursor;
				}
			}
			if (chartAlignType.InnerPlotPosition != null)
			{
				if (!chartAlignType.InnerPlotPosition.IsExpression)
				{
					if (chartAlignType.InnerPlotPosition.Value)
					{
						area.AlignType |= AreaAlignTypes.PlotPosition;
					}
				}
				else if (chartAlignType.Instance.InnerPlotPosition)
				{
					area.AlignType |= AreaAlignTypes.PlotPosition;
				}
			}
			if (chartAlignType.Position == null)
			{
				return;
			}
			if (!chartAlignType.Position.IsExpression)
			{
				if (chartAlignType.Position.Value)
				{
					area.AlignType |= AreaAlignTypes.Position;
				}
			}
			else if (chartAlignType.Instance.Position)
			{
				area.AlignType |= AreaAlignTypes.Position;
			}
		}

		private void RenderChartAreaStyle(ChartArea chartArea, Microsoft.Reporting.Chart.WebForms.ChartArea area)
		{
			Style style = chartArea.Style;
			if (style != null)
			{
				StyleInstance style2 = chartArea.Instance.Style;
				if (MappingHelper.IsStylePropertyDefined(chartArea.Style.BackgroundColor))
				{
					area.BackColor = MappingHelper.GetStyleBackgroundColor(style, style2);
				}
				if (MappingHelper.IsStylePropertyDefined(chartArea.Style.BackgroundGradientEndColor))
				{
					area.BackGradientEndColor = MappingHelper.GetStyleBackGradientEndColor(style, style2);
				}
				if (MappingHelper.IsStylePropertyDefined(chartArea.Style.BackgroundGradientType))
				{
					area.BackGradientType = GetGradientType(MappingHelper.GetStyleBackGradientType(style, style2));
				}
				if (MappingHelper.IsStylePropertyDefined(chartArea.Style.BackgroundHatchType))
				{
					area.BackHatchStyle = GetHatchType(MappingHelper.GetStyleBackgroundHatchType(style, style2));
				}
				if (MappingHelper.IsStylePropertyDefined(chartArea.Style.ShadowColor))
				{
					area.ShadowColor = MappingHelper.GetStyleShadowColor(style, style2);
				}
				if (MappingHelper.IsStylePropertyDefined(chartArea.Style.ShadowOffset))
				{
					area.ShadowOffset = MappingHelper.GetStyleShadowOffset(style, style2, base.DpiX);
				}
				RenderChartAreaBorder(chartArea.Style.Border, area);
				RenderChartAreaBackgroundImage(chartArea.Style.BackgroundImage, area);
			}
		}

		private void RenderAxes(ChartArea chartArea, Microsoft.Reporting.Chart.WebForms.ChartArea area)
		{
			if (chartArea.CategoryAxes != null)
			{
				RenderCategoryAxes(chartArea.CategoryAxes, area);
			}
			if (chartArea.ValueAxes != null)
			{
				RenderValueAxes(chartArea.ValueAxes, area);
			}
		}

		private void RenderCategoryAxes(ChartAxisCollection categoryAxes, Microsoft.Reporting.Chart.WebForms.ChartArea area)
		{
			bool flag = false;
			foreach (ChartAxis categoryAxis in categoryAxes)
			{
				if (categoryAxis.Location != null)
				{
					if (!categoryAxis.Location.IsExpression)
					{
						if (categoryAxis.Location.Value == ChartAxisLocation.Default && !flag)
						{
							RenderAxis(categoryAxis, area.AxisX, area, isCategory: true);
							flag = true;
						}
						else
						{
							RenderAxis(categoryAxis, area.AxisX2, area, isCategory: true);
						}
					}
					else if (categoryAxis.Instance.Location == ChartAxisLocation.Default && !flag)
					{
						RenderAxis(categoryAxis, area.AxisX, area, isCategory: true);
						flag = true;
					}
					else
					{
						RenderAxis(categoryAxis, area.AxisX2, area, isCategory: true);
					}
				}
				else if (!flag)
				{
					RenderAxis(categoryAxis, area.AxisX, area, isCategory: true);
					flag = true;
				}
				else
				{
					RenderAxis(categoryAxis, area.AxisX2, area, isCategory: true);
				}
			}
		}

		private void RenderValueAxes(ChartAxisCollection valueAxes, Microsoft.Reporting.Chart.WebForms.ChartArea area)
		{
			bool flag = false;
			foreach (ChartAxis valueAxis in valueAxes)
			{
				if (valueAxis.Location != null)
				{
					if (!valueAxis.Location.IsExpression)
					{
						if (valueAxis.Location.Value == ChartAxisLocation.Default && !flag)
						{
							RenderAxis(valueAxis, area.AxisY, area, isCategory: false);
							flag = true;
						}
						else
						{
							RenderAxis(valueAxis, area.AxisY2, area, isCategory: false);
						}
					}
					else if (valueAxis.Instance.Location == ChartAxisLocation.Default && !flag)
					{
						RenderAxis(valueAxis, area.AxisY, area, isCategory: false);
						flag = true;
					}
					else
					{
						RenderAxis(valueAxis, area.AxisY2, area, isCategory: false);
					}
				}
				else if (!flag)
				{
					RenderAxis(valueAxis, area.AxisY, area, isCategory: false);
					flag = true;
				}
				else
				{
					RenderAxis(valueAxis, area.AxisY2, area, isCategory: false);
				}
			}
		}

		private void Render3DProperties(ChartThreeDProperties chartThreeDProperties, ChartArea3DStyle threeDProperties)
		{
			if (chartThreeDProperties == null)
			{
				return;
			}
			if (chartThreeDProperties.Clustered != null)
			{
				if (!chartThreeDProperties.Clustered.IsExpression)
				{
					threeDProperties.Clustered = chartThreeDProperties.Clustered.Value;
				}
				else
				{
					threeDProperties.Clustered = chartThreeDProperties.Instance.Clustered;
				}
			}
			if (chartThreeDProperties.DepthRatio != null)
			{
				if (!chartThreeDProperties.DepthRatio.IsExpression)
				{
					threeDProperties.PointDepth = chartThreeDProperties.DepthRatio.Value;
				}
				else
				{
					threeDProperties.PointDepth = chartThreeDProperties.Instance.DepthRatio;
				}
			}
			if (chartThreeDProperties.Enabled != null)
			{
				if (!chartThreeDProperties.Enabled.IsExpression)
				{
					threeDProperties.Enable3D = chartThreeDProperties.Enabled.Value;
				}
				else
				{
					threeDProperties.Enable3D = chartThreeDProperties.Instance.Enabled;
				}
			}
			if (chartThreeDProperties.GapDepth != null)
			{
				if (!chartThreeDProperties.GapDepth.IsExpression)
				{
					threeDProperties.PointGapDepth = chartThreeDProperties.GapDepth.Value;
				}
				else
				{
					threeDProperties.PointGapDepth = chartThreeDProperties.Instance.GapDepth;
				}
			}
			if (chartThreeDProperties.Inclination != null)
			{
				if (!chartThreeDProperties.Inclination.IsExpression)
				{
					threeDProperties.XAngle = chartThreeDProperties.Inclination.Value;
				}
				else
				{
					threeDProperties.XAngle = chartThreeDProperties.Instance.Inclination;
				}
			}
			if (chartThreeDProperties.Perspective != null)
			{
				if (!chartThreeDProperties.Perspective.IsExpression)
				{
					threeDProperties.Perspective = chartThreeDProperties.Perspective.Value;
				}
				else
				{
					threeDProperties.Perspective = chartThreeDProperties.Instance.Perspective;
				}
			}
			if (chartThreeDProperties.ProjectionMode != null)
			{
				ChartThreeDProjectionModes chartThreeDProjectionModes = ChartThreeDProjectionModes.Perspective;
				chartThreeDProjectionModes = (chartThreeDProperties.ProjectionMode.IsExpression ? chartThreeDProperties.Instance.ProjectionMode : chartThreeDProperties.ProjectionMode.Value);
				threeDProperties.RightAngleAxes = (chartThreeDProjectionModes == ChartThreeDProjectionModes.Oblique);
			}
			else
			{
				threeDProperties.RightAngleAxes = true;
			}
			if (chartThreeDProperties.Rotation != null)
			{
				if (!chartThreeDProperties.Rotation.IsExpression)
				{
					threeDProperties.YAngle = chartThreeDProperties.Rotation.Value;
				}
				else
				{
					threeDProperties.YAngle = chartThreeDProperties.Instance.Rotation;
				}
			}
			if (chartThreeDProperties.Shading != null)
			{
				if (!chartThreeDProperties.Shading.IsExpression)
				{
					threeDProperties.Light = GetThreeDLight(chartThreeDProperties.Shading.Value);
				}
				else
				{
					threeDProperties.Light = GetThreeDLight(chartThreeDProperties.Instance.Shading);
				}
			}
			else
			{
				threeDProperties.Light = LightStyle.Realistic;
			}
			if (chartThreeDProperties.WallThickness != null)
			{
				if (!chartThreeDProperties.WallThickness.IsExpression)
				{
					threeDProperties.WallWidth = chartThreeDProperties.WallThickness.Value;
				}
				else
				{
					threeDProperties.WallWidth = chartThreeDProperties.Instance.WallThickness;
				}
			}
		}

		private LightStyle GetThreeDLight(ChartThreeDShadingTypes shading)
		{
			switch (shading)
			{
			case ChartThreeDShadingTypes.Real:
				return LightStyle.Realistic;
			case ChartThreeDShadingTypes.Simple:
				return LightStyle.Simplistic;
			default:
				return LightStyle.None;
			}
		}

		private void RenderAxis(ChartAxis chartAxis, Microsoft.Reporting.Chart.WebForms.Axis axis, Microsoft.Reporting.Chart.WebForms.ChartArea area, bool isCategory)
		{
			RenderAxisStyle(chartAxis, axis);
			RenderAxisTitle(chartAxis.Title, axis);
			RenderAxisStripLines(chartAxis, axis);
			RenderAxisGridLines(chartAxis.MajorGridLines, axis.MajorGrid, isMajor: true);
			RenderAxisGridLines(chartAxis.MinorGridLines, axis.MinorGrid, isMajor: false);
			RenderAxisTickMarks(chartAxis.MajorTickMarks, axis.MajorTickMark, isMajor: true);
			RenderAxisTickMarks(chartAxis.MinorTickMarks, axis.MinorTickMark, isMajor: false);
			RenderAxisScaleBreak(chartAxis.AxisScaleBreak, axis.ScaleBreakStyle);
			RenderCustomProperties(chartAxis.CustomProperties, axis);
			SetAxisProperties(chartAxis, axis, area, isCategory);
		}

		private void SetAxisProperties(ChartAxis chartAxis, Microsoft.Reporting.Chart.WebForms.Axis axis, Microsoft.Reporting.Chart.WebForms.ChartArea area, bool isCategory)
		{
			SetAxisArrow(chartAxis, axis);
			SetAxisCrossing(chartAxis, axis);
			SetAxisLabelsProperties(chartAxis, axis);
			if (chartAxis.IncludeZero != null)
			{
				if (!chartAxis.IncludeZero.IsExpression)
				{
					axis.StartFromZero = chartAxis.IncludeZero.Value;
				}
				else
				{
					axis.StartFromZero = chartAxis.Instance.IncludeZero;
				}
			}
			if (chartAxis.Interlaced != null)
			{
				if (!chartAxis.Interlaced.IsExpression)
				{
					axis.Interlaced = chartAxis.Interlaced.Value;
				}
				else
				{
					axis.Interlaced = chartAxis.Instance.Interlaced;
				}
			}
			if (chartAxis.InterlacedColor != null)
			{
				Color color = Color.Empty;
				if (MappingHelper.GetColorFromReportColorProperty(chartAxis.InterlacedColor, ref color))
				{
					axis.InterlacedColor = color;
				}
				else if (chartAxis.Instance.InterlacedColor != null)
				{
					axis.InterlacedColor = chartAxis.Instance.InterlacedColor.ToColor();
				}
			}
			if (chartAxis.VariableAutoInterval != null)
			{
				if (!chartAxis.VariableAutoInterval.IsExpression)
				{
					axis.IntervalAutoMode = GetIntervalAutoMode(chartAxis.VariableAutoInterval.Value);
				}
				else
				{
					axis.IntervalAutoMode = GetIntervalAutoMode(chartAxis.Instance.VariableAutoInterval);
				}
			}
			if (chartAxis.Interval != null)
			{
				double num = chartAxis.Interval.IsExpression ? chartAxis.Instance.Interval : chartAxis.Interval.Value;
				if (num == 0.0)
				{
					num = double.NaN;
				}
				axis.Interval = num;
			}
			else
			{
				axis.Interval = double.NaN;
			}
			if (chartAxis.IntervalType != null)
			{
				if (!chartAxis.IntervalType.IsExpression)
				{
					axis.IntervalType = GetDateTimeIntervalType(chartAxis.IntervalType.Value);
				}
				else
				{
					axis.IntervalType = GetDateTimeIntervalType(chartAxis.Instance.IntervalType);
				}
			}
			if (chartAxis.IntervalOffset != null)
			{
				if (!chartAxis.IntervalOffset.IsExpression)
				{
					axis.IntervalOffset = chartAxis.IntervalOffset.Value;
				}
				else
				{
					axis.IntervalOffset = chartAxis.Instance.IntervalOffset;
				}
			}
			if (chartAxis.IntervalOffsetType != null)
			{
				if (!chartAxis.IntervalOffsetType.IsExpression)
				{
					axis.IntervalOffsetType = GetDateTimeIntervalType(chartAxis.IntervalOffsetType.Value);
				}
				else
				{
					axis.IntervalOffsetType = GetDateTimeIntervalType(chartAxis.Instance.IntervalOffsetType);
				}
			}
			if (chartAxis.LabelInterval != null)
			{
				double num2 = chartAxis.LabelInterval.IsExpression ? chartAxis.Instance.LabelInterval : chartAxis.LabelInterval.Value;
				if (num2 == 0.0)
				{
					num2 = double.NaN;
				}
				axis.LabelStyle.Interval = num2;
			}
			else
			{
				axis.LabelStyle.Interval = double.NaN;
			}
			if (chartAxis.LabelIntervalType != null)
			{
				if (!chartAxis.LabelIntervalType.IsExpression)
				{
					axis.LabelStyle.IntervalType = GetDateTimeIntervalType(chartAxis.LabelIntervalType.Value);
				}
				else
				{
					axis.LabelStyle.IntervalType = GetDateTimeIntervalType(chartAxis.Instance.LabelIntervalType);
				}
			}
			if (chartAxis.LabelIntervalOffset != null)
			{
				if (!chartAxis.LabelIntervalOffset.IsExpression)
				{
					axis.LabelStyle.IntervalOffset = chartAxis.LabelIntervalOffset.Value;
				}
				else
				{
					axis.LabelStyle.IntervalOffset = chartAxis.Instance.LabelIntervalOffset;
				}
			}
			if (chartAxis.LabelIntervalOffsetType != null)
			{
				if (!chartAxis.LabelIntervalOffsetType.IsExpression)
				{
					axis.LabelStyle.IntervalOffsetType = GetDateTimeIntervalType(chartAxis.LabelIntervalOffsetType.Value);
				}
				else
				{
					axis.LabelStyle.IntervalOffsetType = GetDateTimeIntervalType(chartAxis.Instance.LabelIntervalOffsetType);
				}
			}
			if (chartAxis.LogBase != null)
			{
				if (!chartAxis.LogBase.IsExpression)
				{
					axis.LogarithmBase = chartAxis.LogBase.Value;
				}
				else
				{
					axis.LogarithmBase = chartAxis.Instance.LogBase;
				}
			}
			if (chartAxis.LogScale != null)
			{
				if (!chartAxis.LogScale.IsExpression)
				{
					axis.Logarithmic = chartAxis.LogScale.Value;
				}
				else
				{
					axis.Logarithmic = chartAxis.Instance.LogScale;
				}
			}
			ChartAutoBool chartAutoBool = (chartAxis.Margin != null) ? (chartAxis.Margin.IsExpression ? chartAxis.Instance.Margin : chartAxis.Margin.Value) : ChartAutoBool.Auto;
			if (chartAutoBool == ChartAutoBool.Auto && isCategory)
			{
				List<Microsoft.Reporting.Chart.WebForms.Axis> list = m_chartAreaInfoDictionary[area.Name].CategoryAxesAutoMargin;
				if (list == null)
				{
					list = new List<Microsoft.Reporting.Chart.WebForms.Axis>();
					m_chartAreaInfoDictionary[area.Name].CategoryAxesAutoMargin = list;
				}
				list.Add(axis);
				axis.Margin = false;
			}
			else
			{
				axis.Margin = GetMargin(chartAutoBool);
			}
			if (chartAxis.MarksAlwaysAtPlotEdge != null)
			{
				if (!chartAxis.MarksAlwaysAtPlotEdge.IsExpression)
				{
					axis.MarksNextToAxis = !chartAxis.MarksAlwaysAtPlotEdge.Value;
				}
				else
				{
					axis.MarksNextToAxis = !chartAxis.Instance.MarksAlwaysAtPlotEdge;
				}
			}
			if (chartAxis.Maximum != null)
			{
				if (!chartAxis.Maximum.IsExpression)
				{
					axis.Maximum = ConvertToDouble(chartAxis.Maximum.Value);
				}
				else
				{
					axis.Maximum = ConvertToDouble(chartAxis.Instance.Maximum);
				}
			}
			if (chartAxis.Minimum != null)
			{
				if (!chartAxis.Minimum.IsExpression)
				{
					axis.Minimum = ConvertToDouble(chartAxis.Minimum.Value);
				}
				else
				{
					axis.Minimum = ConvertToDouble(chartAxis.Instance.Minimum);
				}
			}
			if (chartAxis.Name != null)
			{
				axis.Name = chartAxis.Name;
			}
			if (chartAxis.Reverse != null)
			{
				if (!chartAxis.Reverse.IsExpression)
				{
					axis.Reverse = chartAxis.Reverse.Value;
				}
				else
				{
					axis.Reverse = chartAxis.Instance.Reverse;
				}
			}
			if (chartAxis.Scalar && isCategory && m_chartAreaInfoDictionary.ContainsKey(area.Name))
			{
				ChartAreaInfo chartAreaInfo = m_chartAreaInfoDictionary[area.Name];
				if (chartAreaInfo.CategoryAxesScalar == null)
				{
					chartAreaInfo.CategoryAxesScalar = new List<string>();
				}
				chartAreaInfo.CategoryAxesScalar.Add(chartAxis.Name);
			}
			if (chartAxis.Visible != null)
			{
				if (!chartAxis.Visible.IsExpression)
				{
					axis.Enabled = GetAxisEnabled(chartAxis.Visible.Value);
				}
				else
				{
					axis.Enabled = GetAxisEnabled(chartAxis.Instance.Visible);
				}
			}
			SetAxisLabelAutoFitStyle(chartAxis, axis);
		}

		private void SetAxisLabelsProperties(ChartAxis chartAxis, Microsoft.Reporting.Chart.WebForms.Axis axis)
		{
			if (chartAxis.HideLabels != null)
			{
				if (!chartAxis.HideLabels.IsExpression)
				{
					axis.LabelStyle.Enabled = !chartAxis.HideLabels.Value;
				}
				else
				{
					axis.LabelStyle.Enabled = !chartAxis.Instance.HideLabels;
				}
			}
			if (chartAxis.OffsetLabels != null)
			{
				if (!chartAxis.OffsetLabels.IsExpression)
				{
					axis.LabelStyle.OffsetLabels = chartAxis.OffsetLabels.Value;
				}
				else
				{
					axis.LabelStyle.OffsetLabels = chartAxis.Instance.OffsetLabels;
				}
			}
			if (chartAxis.HideEndLabels != null)
			{
				if (!chartAxis.HideEndLabels.IsExpression)
				{
					axis.LabelStyle.ShowEndLabels = !chartAxis.HideEndLabels.Value;
				}
				else
				{
					axis.LabelStyle.ShowEndLabels = !chartAxis.Instance.HideEndLabels;
				}
			}
			if (chartAxis.Angle != null)
			{
				if (!chartAxis.Angle.IsExpression)
				{
					axis.LabelStyle.FontAngle = (int)Math.Round(chartAxis.Angle.Value);
				}
				else
				{
					axis.LabelStyle.FontAngle = (int)Math.Round(chartAxis.Instance.Angle);
				}
			}
		}

		private void RenderAxisLabelFont(ChartAxis chartAxis, Microsoft.Reporting.Chart.WebForms.Axis axis)
		{
			Style style = chartAxis.Style;
			if (style == null)
			{
				axis.LabelStyle.Font = GetDefaultFont();
			}
			else
			{
				axis.LabelStyle.Font = GetFont(style, chartAxis.Instance.Style);
			}
		}

		private void SetAxisCrossing(ChartAxis chartAxis, Microsoft.Reporting.Chart.WebForms.Axis axis)
		{
			if (chartAxis.CrossAt == null)
			{
				return;
			}
			if (!chartAxis.CrossAt.IsExpression)
			{
				if (chartAxis.CrossAt.Value != null)
				{
					double num = ConvertToDouble(chartAxis.CrossAt.Value, checkForMaxMinValue: true);
					if (!double.IsNaN(num))
					{
						axis.Crossing = num;
					}
				}
			}
			else if (chartAxis.Instance.CrossAt != null)
			{
				double num = ConvertToDouble(chartAxis.Instance.CrossAt, checkForMaxMinValue: true);
				if (!double.IsNaN(num))
				{
					axis.Crossing = num;
				}
			}
		}

		private void SetAxisLabelAutoFitStyle(ChartAxis chartAxis, Microsoft.Reporting.Chart.WebForms.Axis axis)
		{
			if (chartAxis.MaxFontSize != null)
			{
				if (!chartAxis.MaxFontSize.IsExpression)
				{
					axis.LabelsAutoFitMaxFontSize = (int)Math.Round(chartAxis.MaxFontSize.Value.ToPoints());
				}
				else
				{
					axis.LabelsAutoFitMaxFontSize = (int)Math.Round(chartAxis.Instance.MaxFontSize.ToPoints());
				}
			}
			if (chartAxis.MinFontSize != null)
			{
				if (!chartAxis.MinFontSize.IsExpression)
				{
					axis.LabelsAutoFitMinFontSize = (int)Math.Round(chartAxis.MinFontSize.Value.ToPoints());
				}
				else
				{
					axis.LabelsAutoFitMinFontSize = (int)Math.Round(chartAxis.Instance.MinFontSize.ToPoints());
				}
			}
			axis.LabelsAutoFitStyle = LabelsAutoFitStyles.None;
			if (chartAxis.PreventFontGrow != null)
			{
				if (!(chartAxis.PreventFontGrow.IsExpression ? chartAxis.Instance.PreventFontGrow : chartAxis.PreventFontGrow.Value))
				{
					axis.LabelsAutoFitStyle |= LabelsAutoFitStyles.IncreaseFont;
				}
			}
			else
			{
				axis.LabelsAutoFitStyle |= LabelsAutoFitStyles.IncreaseFont;
			}
			if (chartAxis.PreventFontShrink != null)
			{
				if (!(chartAxis.PreventFontShrink.IsExpression ? chartAxis.Instance.PreventFontShrink : chartAxis.PreventFontShrink.Value))
				{
					axis.LabelsAutoFitStyle |= LabelsAutoFitStyles.DecreaseFont;
				}
			}
			else
			{
				axis.LabelsAutoFitStyle |= LabelsAutoFitStyles.DecreaseFont;
			}
			if (chartAxis.PreventLabelOffset != null)
			{
				if (!(chartAxis.PreventLabelOffset.IsExpression ? chartAxis.Instance.PreventLabelOffset : chartAxis.PreventLabelOffset.Value))
				{
					axis.LabelsAutoFitStyle |= LabelsAutoFitStyles.OffsetLabels;
				}
			}
			else
			{
				axis.LabelsAutoFitStyle |= LabelsAutoFitStyles.OffsetLabels;
			}
			if (chartAxis.AllowLabelRotation != null)
			{
				switch (chartAxis.AllowLabelRotation.IsExpression ? chartAxis.Instance.AllowLabelRotation : chartAxis.AllowLabelRotation.Value)
				{
				case ChartAxisLabelRotation.Rotate30:
					axis.LabelsAutoFitStyle |= LabelsAutoFitStyles.LabelsAngleStep30;
					break;
				case ChartAxisLabelRotation.Rotate45:
					axis.LabelsAutoFitStyle |= LabelsAutoFitStyles.LabelsAngleStep45;
					break;
				case ChartAxisLabelRotation.Rotate90:
					axis.LabelsAutoFitStyle |= LabelsAutoFitStyles.LabelsAngleStep90;
					break;
				}
			}
			else
			{
				axis.LabelsAutoFitStyle |= m_defaultLabelsAngleStep;
			}
			if (chartAxis.PreventWordWrap != null)
			{
				if (!(chartAxis.PreventWordWrap.IsExpression ? chartAxis.Instance.PreventWordWrap : chartAxis.PreventWordWrap.Value))
				{
					axis.LabelsAutoFitStyle |= LabelsAutoFitStyles.WordWrap;
				}
			}
			else
			{
				axis.LabelsAutoFitStyle |= LabelsAutoFitStyles.WordWrap;
			}
			if (chartAxis.LabelsAutoFitDisabled != null)
			{
				if (!chartAxis.LabelsAutoFitDisabled.IsExpression)
				{
					axis.LabelsAutoFit = !chartAxis.LabelsAutoFitDisabled.Value;
				}
				else
				{
					axis.LabelsAutoFit = !chartAxis.Instance.LabelsAutoFitDisabled;
				}
			}
			else
			{
				axis.LabelsAutoFit = true;
			}
		}

		private void SetAxisArrow(ChartAxis chartAxis, Microsoft.Reporting.Chart.WebForms.Axis axis)
		{
			ChartAxisArrow chartAxisArrow = ChartAxisArrow.None;
			if (chartAxis.Arrows != null)
			{
				switch (chartAxis.Arrows.IsExpression ? chartAxis.Instance.Arrows : chartAxis.Arrows.Value)
				{
				case ChartAxisArrow.None:
					axis.Arrows = ArrowsType.None;
					break;
				case ChartAxisArrow.Lines:
					axis.Arrows = ArrowsType.Lines;
					break;
				case ChartAxisArrow.SharpTriangle:
					axis.Arrows = ArrowsType.SharpTriangle;
					break;
				case ChartAxisArrow.Triangle:
					axis.Arrows = ArrowsType.Triangle;
					break;
				}
			}
		}

		private void RenderAxisStripLines(ChartAxis chartAxis, Microsoft.Reporting.Chart.WebForms.Axis axis)
		{
			if (chartAxis.StripLines == null)
			{
				return;
			}
			foreach (ChartStripLine stripLine2 in chartAxis.StripLines)
			{
				StripLine stripLine = new StripLine();
				RenderStripLine(stripLine2, stripLine);
				axis.StripLines.Add(stripLine);
			}
		}

		private void RenderStripLine(ChartStripLine chartStripLine, StripLine stripLine)
		{
			SetStripLineProperties(chartStripLine, stripLine);
			RenderStripLineStyle(chartStripLine, stripLine);
			RenderActionInfo(chartStripLine.ActionInfo, stripLine.ToolTip, stripLine);
		}

		private void SetStripLineProperties(ChartStripLine chartStripLine, StripLine stripLine)
		{
			if (chartStripLine.Interval != null)
			{
				if (!chartStripLine.Interval.IsExpression)
				{
					stripLine.Interval = chartStripLine.Interval.Value;
				}
				else
				{
					stripLine.Interval = chartStripLine.Instance.Interval;
				}
			}
			if (chartStripLine.IntervalType != null)
			{
				if (!chartStripLine.IntervalType.IsExpression)
				{
					stripLine.IntervalType = GetDateTimeIntervalType(chartStripLine.IntervalType.Value);
				}
				else
				{
					stripLine.IntervalType = GetDateTimeIntervalType(chartStripLine.Instance.IntervalType);
				}
			}
			if (chartStripLine.IntervalOffset != null)
			{
				if (!chartStripLine.IntervalOffset.IsExpression)
				{
					stripLine.IntervalOffset = chartStripLine.IntervalOffset.Value;
				}
				else
				{
					stripLine.IntervalOffset = chartStripLine.Instance.IntervalOffset;
				}
			}
			if (chartStripLine.IntervalOffsetType != null)
			{
				if (!chartStripLine.IntervalOffsetType.IsExpression)
				{
					stripLine.IntervalOffsetType = GetDateTimeIntervalType(chartStripLine.IntervalOffsetType.Value);
				}
				else
				{
					stripLine.IntervalOffsetType = GetDateTimeIntervalType(chartStripLine.Instance.IntervalOffsetType);
				}
			}
			if (chartStripLine.StripWidth != null)
			{
				if (!chartStripLine.StripWidth.IsExpression)
				{
					stripLine.StripWidth = chartStripLine.StripWidth.Value;
				}
				else
				{
					stripLine.StripWidth = chartStripLine.Instance.StripWidth;
				}
			}
			if (chartStripLine.StripWidthType != null)
			{
				if (!chartStripLine.StripWidthType.IsExpression)
				{
					stripLine.StripWidthType = GetDateTimeIntervalType(chartStripLine.StripWidthType.Value);
				}
				else
				{
					stripLine.StripWidthType = GetDateTimeIntervalType(chartStripLine.Instance.StripWidthType);
				}
			}
			if (chartStripLine.Title != null)
			{
				if (!chartStripLine.Title.IsExpression)
				{
					if (chartStripLine.Title.Value != null)
					{
						stripLine.Title = chartStripLine.Title.Value;
					}
				}
				else if (chartStripLine.Instance.Title != null)
				{
					stripLine.Title = chartStripLine.Instance.Title;
				}
			}
			if (chartStripLine.ToolTip != null)
			{
				if (!chartStripLine.ToolTip.IsExpression)
				{
					if (chartStripLine.ToolTip.Value != null)
					{
						stripLine.ToolTip = chartStripLine.ToolTip.Value;
					}
				}
				else if (chartStripLine.Instance.ToolTip != null)
				{
					stripLine.ToolTip = chartStripLine.Instance.ToolTip;
				}
			}
			if (chartStripLine.TextOrientation != null)
			{
				if (!chartStripLine.TextOrientation.IsExpression)
				{
					stripLine.TextOrientation = GetTextOrientation(chartStripLine.TextOrientation.Value);
				}
				else
				{
					stripLine.TextOrientation = GetTextOrientation(chartStripLine.Instance.TextOrientation);
				}
			}
		}

		private void RenderStripLineStyle(ChartStripLine chartStripLine, StripLine stripLine)
		{
			stripLine.TitleAlignment = StringAlignment.Near;
			Style style = chartStripLine.Style;
			Border border = null;
			if (style != null)
			{
				StyleInstance style2 = chartStripLine.Instance.Style;
				if (MappingHelper.IsStylePropertyDefined(chartStripLine.Style.Color))
				{
					stripLine.TitleColor = MappingHelper.GetStyleColor(style, style2);
				}
				if (MappingHelper.IsStylePropertyDefined(chartStripLine.Style.BackgroundColor))
				{
					stripLine.BackColor = MappingHelper.GetStyleBackgroundColor(style, style2);
				}
				if (MappingHelper.IsStylePropertyDefined(chartStripLine.Style.BackgroundGradientEndColor))
				{
					stripLine.BackGradientEndColor = MappingHelper.GetStyleBackGradientEndColor(style, style2);
				}
				if (MappingHelper.IsStylePropertyDefined(chartStripLine.Style.BackgroundGradientType))
				{
					stripLine.BackGradientType = GetGradientType(MappingHelper.GetStyleBackGradientType(style, style2));
				}
				if (MappingHelper.IsStylePropertyDefined(chartStripLine.Style.BackgroundHatchType))
				{
					stripLine.BackHatchStyle = GetHatchType(MappingHelper.GetStyleBackgroundHatchType(style, style2));
				}
				if (MappingHelper.IsStylePropertyDefined(chartStripLine.Style.TextAlign))
				{
					stripLine.TitleAlignment = GetStringAlignmentFromTextAlignments(MappingHelper.GetStyleTextAlign(style, style2));
				}
				if (MappingHelper.IsStylePropertyDefined(chartStripLine.Style.VerticalAlign))
				{
					stripLine.TitleLineAlignment = GetStringAlignmentFromVericalAlignments(MappingHelper.GetStyleVerticalAlignment(style, style2));
				}
				RenderStripLineBackgroundImage(chartStripLine.Style.BackgroundImage, stripLine);
				border = style.Border;
			}
			RenderStripLineBorder(border, stripLine);
			RenderStripLineFont(chartStripLine, stripLine);
		}

		private void RenderAxisTitle(ChartAxisTitle chartAxisTitle, Microsoft.Reporting.Chart.WebForms.Axis axis)
		{
			if (chartAxisTitle != null)
			{
				SetAxisTitleProperties(chartAxisTitle, axis);
				RenderAxisTitleStyle(chartAxisTitle, axis);
			}
		}

		private void SetAxisTitleProperties(ChartAxisTitle chartAxisTitle, Microsoft.Reporting.Chart.WebForms.Axis axis)
		{
			if (chartAxisTitle.Caption != null)
			{
				if (chartAxisTitle.Caption.Value != null)
				{
					axis.Title = chartAxisTitle.Caption.Value;
				}
				else if (chartAxisTitle.Instance.Caption != null)
				{
					axis.Title = chartAxisTitle.Instance.Caption;
				}
			}
			if (chartAxisTitle.Position != null)
			{
				if (!chartAxisTitle.Position.IsExpression)
				{
					axis.TitleAlignment = GetAlignment(chartAxisTitle.Position.Value);
				}
				else
				{
					axis.TitleAlignment = GetAlignment(chartAxisTitle.Instance.Position);
				}
			}
			if (chartAxisTitle.TextOrientation != null)
			{
				if (!chartAxisTitle.TextOrientation.IsExpression)
				{
					axis.TextOrientation = GetTextOrientation(chartAxisTitle.TextOrientation.Value);
				}
				else
				{
					axis.TextOrientation = GetTextOrientation(chartAxisTitle.Instance.TextOrientation);
				}
			}
		}

		private StringAlignment GetAlignment(ChartAxisTitlePositions position)
		{
			switch (position)
			{
			case ChartAxisTitlePositions.Center:
				return StringAlignment.Center;
			case ChartAxisTitlePositions.Far:
				return StringAlignment.Far;
			default:
				return StringAlignment.Near;
			}
		}

		private void RenderAxisTitleStyle(ChartAxisTitle axisTitle, Microsoft.Reporting.Chart.WebForms.Axis axis)
		{
			Style style = axisTitle.Style;
			if (style != null)
			{
				StyleInstance style2 = axisTitle.Instance.Style;
				if (MappingHelper.IsStylePropertyDefined(axisTitle.Style.Color))
				{
					axis.TitleColor = MappingHelper.GetStyleColor(style, style2);
				}
			}
			RenderAxisTitleFont(axisTitle, axis);
		}

		private void RenderAxisTitleFont(ChartAxisTitle axisTitle, Microsoft.Reporting.Chart.WebForms.Axis axis)
		{
			Style style = axisTitle.Style;
			if (style == null)
			{
				axis.TitleFont = GetDefaultFont();
			}
			else
			{
				axis.TitleFont = GetFont(style, axisTitle.Instance.Style);
			}
		}

		private void RenderAxisStyle(ChartAxis chartAxis, Microsoft.Reporting.Chart.WebForms.Axis axis)
		{
			Style style = chartAxis.Style;
			if (style != null)
			{
				StyleInstance style2 = chartAxis.Instance.Style;
				if (MappingHelper.IsStylePropertyDefined(chartAxis.Style.Format))
				{
					axis.LabelStyle.Format = MappingHelper.GetStyleFormat(style, style2);
				}
				if (MappingHelper.IsStylePropertyDefined(chartAxis.Style.Color))
				{
					axis.LabelStyle.FontColor = MappingHelper.GetStyleColor(style, style2);
				}
				RenderAxisBorder(chartAxis.Style.Border, axis);
			}
			RenderAxisLabelFont(chartAxis, axis);
		}

		private void RenderAxisBorder(Border border, Microsoft.Reporting.Chart.WebForms.Axis axis)
		{
			if (border != null)
			{
				if (MappingHelper.IsStylePropertyDefined(border.Color))
				{
					axis.LineColor = MappingHelper.GetStyleBorderColor(border);
				}
				if (MappingHelper.IsStylePropertyDefined(border.Style))
				{
					axis.LineStyle = GetBorderStyle(MappingHelper.GetStyleBorderStyle(border), isLine: false);
				}
				axis.LineWidth = MappingHelper.GetStyleBorderWidth(border, base.DpiX);
			}
		}

		private void RenderAxisGridLines(ChartGridLines chartGridLines, Grid gridLines, bool isMajor)
		{
			if (chartGridLines != null)
			{
				SetAxisGridLinesProperties(chartGridLines, gridLines, isMajor);
				if (chartGridLines.Style != null)
				{
					RenderAxisGridLinesBorder(chartGridLines.Style.Border, gridLines);
				}
			}
		}

		private void RenderAxisGridLinesBorder(Border border, Grid gridLines)
		{
			if (border != null)
			{
				if (MappingHelper.IsStylePropertyDefined(border.Color))
				{
					gridLines.LineColor = MappingHelper.GetStyleBorderColor(border);
				}
				if (MappingHelper.IsStylePropertyDefined(border.Style))
				{
					gridLines.LineStyle = GetBorderStyle(MappingHelper.GetStyleBorderStyle(border), isLine: false);
				}
				gridLines.LineWidth = MappingHelper.GetStyleBorderWidth(border, base.DpiX);
			}
		}

		private void SetAxisGridLinesProperties(ChartGridLines chartGridLines, Grid gridLines, bool isMajor)
		{
			if (chartGridLines.Enabled != null)
			{
				ChartAutoBool chartAutoBool = ChartAutoBool.Auto;
				switch (chartGridLines.Enabled.IsExpression ? chartGridLines.Instance.Enabled : chartGridLines.Enabled.Value)
				{
				case ChartAutoBool.Auto:
					gridLines.Enabled = (isMajor ? true : false);
					break;
				case ChartAutoBool.True:
					gridLines.Enabled = true;
					break;
				case ChartAutoBool.False:
					gridLines.Enabled = false;
					break;
				}
			}
			if (chartGridLines.Interval != null)
			{
				double num = chartGridLines.Interval.IsExpression ? chartGridLines.Instance.Interval : chartGridLines.Interval.Value;
				if (num == 0.0)
				{
					num = double.NaN;
				}
				gridLines.Interval = num;
			}
			else
			{
				gridLines.Interval = double.NaN;
			}
			if (chartGridLines.IntervalType != null)
			{
				if (!chartGridLines.IntervalType.IsExpression)
				{
					gridLines.IntervalType = GetDateTimeIntervalType(chartGridLines.IntervalType.Value);
				}
				else
				{
					gridLines.IntervalType = GetDateTimeIntervalType(chartGridLines.Instance.IntervalType);
				}
			}
			if (chartGridLines.IntervalOffset != null)
			{
				if (!chartGridLines.IntervalOffset.IsExpression)
				{
					gridLines.IntervalOffset = chartGridLines.IntervalOffset.Value;
				}
				else
				{
					gridLines.IntervalOffset = chartGridLines.Instance.IntervalOffset;
				}
			}
			if (chartGridLines.IntervalOffsetType != null)
			{
				if (!chartGridLines.IntervalOffsetType.IsExpression)
				{
					gridLines.IntervalOffsetType = GetDateTimeIntervalType(chartGridLines.IntervalOffsetType.Value);
				}
				else
				{
					gridLines.IntervalOffsetType = GetDateTimeIntervalType(chartGridLines.Instance.IntervalOffsetType);
				}
			}
		}

		private void RenderAxisTickMarks(ChartTickMarks chartTickMarks, TickMark tickMarks, bool isMajor)
		{
			if (chartTickMarks != null)
			{
				SetAxisTickMarkProperties(chartTickMarks, tickMarks, isMajor);
				RenderTickMarkStyle(chartTickMarks, tickMarks);
			}
		}

		private void SetAxisTickMarkProperties(ChartTickMarks chartTickMarks, TickMark tickMarks, bool isMajor)
		{
			if (chartTickMarks.Enabled != null)
			{
				if (!chartTickMarks.Enabled.IsExpression)
				{
					tickMarks.Enabled = GetChartTickMarksEnabled(chartTickMarks.Enabled.Value, isMajor);
				}
				else
				{
					tickMarks.Enabled = GetChartTickMarksEnabled(chartTickMarks.Instance.Enabled, isMajor);
				}
			}
			if (chartTickMarks.Interval != null)
			{
				double num = chartTickMarks.Interval.IsExpression ? chartTickMarks.Instance.Interval : chartTickMarks.Interval.Value;
				if (num == 0.0)
				{
					num = double.NaN;
				}
				tickMarks.Interval = num;
			}
			else
			{
				tickMarks.Interval = double.NaN;
			}
			if (chartTickMarks.IntervalOffset != null)
			{
				if (!chartTickMarks.IntervalOffset.IsExpression)
				{
					tickMarks.IntervalOffset = chartTickMarks.IntervalOffset.Value;
				}
				else
				{
					tickMarks.IntervalOffset = chartTickMarks.Instance.IntervalOffset;
				}
			}
			if (chartTickMarks.IntervalOffsetType != null)
			{
				if (!chartTickMarks.IntervalOffsetType.IsExpression)
				{
					tickMarks.IntervalOffsetType = GetDateTimeIntervalType(chartTickMarks.IntervalOffsetType.Value);
				}
				else
				{
					tickMarks.IntervalOffsetType = GetDateTimeIntervalType(chartTickMarks.Instance.IntervalOffsetType);
				}
			}
			if (chartTickMarks.IntervalType != null)
			{
				if (!chartTickMarks.IntervalType.IsExpression)
				{
					tickMarks.IntervalType = GetDateTimeIntervalType(chartTickMarks.IntervalType.Value);
				}
				else
				{
					tickMarks.IntervalType = GetDateTimeIntervalType(chartTickMarks.Instance.IntervalType);
				}
			}
			if (chartTickMarks.Length != null)
			{
				if (!chartTickMarks.Length.IsExpression)
				{
					tickMarks.Size = (float)chartTickMarks.Length.Value;
				}
				else
				{
					tickMarks.Size = (float)chartTickMarks.Instance.Length;
				}
			}
			if (chartTickMarks.Type != null)
			{
				if (!chartTickMarks.Type.IsExpression)
				{
					tickMarks.Style = GetTickMarkStyle(chartTickMarks.Type.Value);
				}
				else
				{
					tickMarks.Style = GetTickMarkStyle(chartTickMarks.Instance.Type);
				}
			}
		}

		private bool GetChartTickMarksEnabled(ChartAutoBool enabled, bool isMajor)
		{
			switch (enabled)
			{
			case ChartAutoBool.False:
				return false;
			case ChartAutoBool.True:
				return true;
			default:
				return isMajor;
			}
		}

		private void RenderTickMarkStyle(ChartTickMarks chartTickMarks, TickMark tickMarks)
		{
			if (chartTickMarks.Style != null)
			{
				RenderTickMarkBorder(chartTickMarks.Style.Border, tickMarks);
			}
		}

		private void RenderTickMarkBorder(Border border, TickMark tickMark)
		{
			if (border != null)
			{
				if (MappingHelper.IsStylePropertyDefined(border.Color))
				{
					tickMark.LineColor = MappingHelper.GetStyleBorderColor(border);
				}
				if (MappingHelper.IsStylePropertyDefined(border.Style))
				{
					tickMark.LineStyle = GetBorderStyle(MappingHelper.GetStyleBorderStyle(border), isLine: false);
				}
				tickMark.LineWidth = MappingHelper.GetStyleBorderWidth(border, base.DpiX);
			}
		}

		private void RenderAxisScaleBreak(ChartAxisScaleBreak chartAxisScaleBreak, AxisScaleBreakStyle axisScaleBreak)
		{
			if (chartAxisScaleBreak == null)
			{
				return;
			}
			if (chartAxisScaleBreak.BreakLineType != null)
			{
				if (!chartAxisScaleBreak.BreakLineType.IsExpression)
				{
					axisScaleBreak.BreakLineType = GetScaleBreakLineType(chartAxisScaleBreak.BreakLineType.Value);
				}
				else
				{
					axisScaleBreak.BreakLineType = GetScaleBreakLineType(chartAxisScaleBreak.Instance.BreakLineType);
				}
			}
			if (chartAxisScaleBreak.CollapsibleSpaceThreshold != null)
			{
				if (!chartAxisScaleBreak.CollapsibleSpaceThreshold.IsExpression)
				{
					axisScaleBreak.CollapsibleSpaceThreshold = chartAxisScaleBreak.CollapsibleSpaceThreshold.Value;
				}
				else
				{
					axisScaleBreak.CollapsibleSpaceThreshold = chartAxisScaleBreak.Instance.CollapsibleSpaceThreshold;
				}
			}
			if (chartAxisScaleBreak.Enabled != null)
			{
				if (!chartAxisScaleBreak.Enabled.IsExpression)
				{
					axisScaleBreak.Enabled = chartAxisScaleBreak.Enabled.Value;
				}
				else
				{
					axisScaleBreak.Enabled = chartAxisScaleBreak.Instance.Enabled;
				}
			}
			if (chartAxisScaleBreak.IncludeZero != null)
			{
				if (!chartAxisScaleBreak.IncludeZero.IsExpression)
				{
					axisScaleBreak.StartFromZero = GetAutoBool(chartAxisScaleBreak.IncludeZero.Value);
				}
				else
				{
					axisScaleBreak.StartFromZero = GetAutoBool(chartAxisScaleBreak.Instance.IncludeZero);
				}
			}
			if (chartAxisScaleBreak.MaxNumberOfBreaks != null)
			{
				if (!chartAxisScaleBreak.MaxNumberOfBreaks.IsExpression)
				{
					axisScaleBreak.MaxNumberOfBreaks = chartAxisScaleBreak.MaxNumberOfBreaks.Value;
				}
				else
				{
					axisScaleBreak.MaxNumberOfBreaks = chartAxisScaleBreak.Instance.MaxNumberOfBreaks;
				}
			}
			if (chartAxisScaleBreak.Spacing != null)
			{
				if (!chartAxisScaleBreak.Spacing.IsExpression)
				{
					axisScaleBreak.Spacing = chartAxisScaleBreak.Spacing.Value;
				}
				else
				{
					axisScaleBreak.Spacing = chartAxisScaleBreak.Instance.Spacing;
				}
			}
			if (chartAxisScaleBreak.Style != null)
			{
				RenderAxisScaleBreakBorder(chartAxisScaleBreak.Style.Border, axisScaleBreak);
			}
		}

		private void RenderCustomProperties(CustomPropertyCollection customProperties, Microsoft.Reporting.Chart.WebForms.Axis axis)
		{
		}

		private void RenderLegends()
		{
			if (m_chart.Legends == null)
			{
				return;
			}
			foreach (ChartLegend legend2 in m_chart.Legends)
			{
				Microsoft.Reporting.Chart.WebForms.Legend legend = new Microsoft.Reporting.Chart.WebForms.Legend();
				m_coreChart.Legends.Add(legend);
				RenderLegend(legend2, legend);
			}
		}

		private void RenderLegend(ChartLegend chartLegend, Microsoft.Reporting.Chart.WebForms.Legend legend)
		{
			if (chartLegend.Name != null)
			{
				legend.Name = chartLegend.Name;
			}
			RenderLegendStyle(chartLegend, legend);
			SetLegendProperties(chartLegend, legend);
			RenderElementPosition(chartLegend.ChartElementPosition, legend.Position);
			RenderLegendTitle(chartLegend.LegendTitle, legend);
			RenderLegendColumns(chartLegend.LegendColumns, legend.CellColumns);
			RenderLegendCustomItems(chartLegend.LegendCustomItems, legend.CustomItems);
		}

		private void SetLegendProperties(ChartLegend chartLegend, Microsoft.Reporting.Chart.WebForms.Legend legend)
		{
			if (chartLegend.Hidden != null)
			{
				if (!chartLegend.Hidden.IsExpression)
				{
					legend.Enabled = !chartLegend.Hidden.Value;
				}
				else
				{
					legend.Enabled = !chartLegend.Instance.Hidden;
				}
			}
			if (chartLegend.DockOutsideChartArea != null)
			{
				if (!chartLegend.DockOutsideChartArea.IsExpression)
				{
					legend.DockInsideChartArea = !chartLegend.DockOutsideChartArea.Value;
				}
				else
				{
					legend.DockInsideChartArea = !chartLegend.Instance.DockOutsideChartArea;
				}
			}
			if (chartLegend.DockToChartArea != null)
			{
				legend.DockToChartArea = chartLegend.DockToChartArea;
			}
			if (chartLegend.Position != null)
			{
				if (!chartLegend.Position.IsExpression)
				{
					legend.Alignment = GetLegendAlignment(chartLegend.Position.Value);
					legend.Docking = GetLegendDocking(chartLegend.Position.Value);
				}
				else
				{
					legend.Alignment = GetLegendAlignment(chartLegend.Instance.Position);
					legend.Docking = GetLegendDocking(chartLegend.Instance.Position);
				}
			}
			if (chartLegend.Layout != null)
			{
				if (!chartLegend.Layout.IsExpression)
				{
					SetLegendLayout(chartLegend.Layout.Value, legend);
				}
				else
				{
					SetLegendLayout(chartLegend.Instance.Layout, legend);
				}
			}
			if (chartLegend.AutoFitTextDisabled != null)
			{
				if (!chartLegend.AutoFitTextDisabled.IsExpression)
				{
					legend.AutoFitText = !chartLegend.AutoFitTextDisabled.Value;
				}
				else
				{
					legend.AutoFitText = !chartLegend.Instance.AutoFitTextDisabled;
				}
			}
			else
			{
				legend.AutoFitText = true;
			}
			_ = chartLegend.EquallySpacedItems;
			if (chartLegend.InterlacedRows != null)
			{
				if (!chartLegend.InterlacedRows.IsExpression)
				{
					legend.InterlacedRows = chartLegend.InterlacedRows.Value;
				}
				else
				{
					legend.InterlacedRows = chartLegend.Instance.InterlacedRows;
				}
			}
			if (chartLegend.InterlacedRowsColor != null)
			{
				Color color = Color.Empty;
				if (MappingHelper.GetColorFromReportColorProperty(chartLegend.InterlacedRowsColor, ref color))
				{
					legend.InterlacedRowsColor = color;
				}
				else if (chartLegend.Instance.InterlacedRowsColor != null)
				{
					legend.InterlacedRowsColor = chartLegend.Instance.InterlacedRowsColor.ToColor();
				}
			}
			if (chartLegend.MaxAutoSize != null)
			{
				if (!chartLegend.MaxAutoSize.IsExpression)
				{
					legend.MaxAutoSize = chartLegend.MaxAutoSize.Value;
				}
				else
				{
					legend.MaxAutoSize = chartLegend.Instance.MaxAutoSize;
				}
			}
			if (chartLegend.MinFontSize != null)
			{
				if (!chartLegend.MinFontSize.IsExpression)
				{
					legend.AutoFitMinFontSize = (int)Math.Round(chartLegend.MinFontSize.Value.ToPoints());
				}
				else
				{
					legend.AutoFitMinFontSize = (int)Math.Round(chartLegend.Instance.MinFontSize.ToPoints());
				}
			}
			if (chartLegend.Reversed != null)
			{
				ChartAutoBool chartAutoBool = ChartAutoBool.Auto;
				switch (chartLegend.Reversed.IsExpression ? chartLegend.Instance.Reversed : chartLegend.Reversed.Value)
				{
				case ChartAutoBool.Auto:
					legend.Reversed = AutoBool.Auto;
					break;
				case ChartAutoBool.False:
					legend.Reversed = AutoBool.False;
					break;
				case ChartAutoBool.True:
					legend.Reversed = AutoBool.True;
					break;
				}
			}
			if (chartLegend.TextWrapThreshold != null)
			{
				if (!chartLegend.TextWrapThreshold.IsExpression)
				{
					legend.TextWrapThreshold = chartLegend.TextWrapThreshold.Value;
				}
				else
				{
					legend.TextWrapThreshold = chartLegend.Instance.TextWrapThreshold;
				}
			}
		}

		private void RenderLegendTitle(ChartLegendTitle chartLegendTitle, Microsoft.Reporting.Chart.WebForms.Legend legend)
		{
			if (chartLegendTitle == null)
			{
				return;
			}
			if (chartLegendTitle.Caption != null)
			{
				if (!chartLegendTitle.Caption.IsExpression)
				{
					if (chartLegendTitle.Caption.Value != null)
					{
						legend.Title = chartLegendTitle.Caption.Value;
					}
				}
				else if (chartLegendTitle.Instance.Caption != null)
				{
					legend.Title = chartLegendTitle.Instance.Caption;
				}
			}
			if (chartLegendTitle.TitleSeparator != null)
			{
				if (!chartLegendTitle.TitleSeparator.IsExpression)
				{
					legend.TitleSeparator = GetLegendSeparatorStyle(chartLegendTitle.TitleSeparator.Value);
				}
				else
				{
					legend.TitleSeparator = GetLegendSeparatorStyle(chartLegendTitle.Instance.TitleSeparator);
				}
			}
			RenderLegendTitleStyle(chartLegendTitle, legend);
		}

		private LegendSeparatorType GetLegendSeparatorStyle(ChartSeparators chartLegendSeparator)
		{
			switch (chartLegendSeparator)
			{
			case ChartSeparators.DashLine:
				return LegendSeparatorType.DashLine;
			case ChartSeparators.DotLine:
				return LegendSeparatorType.DotLine;
			case ChartSeparators.DoubleLine:
				return LegendSeparatorType.DoubleLine;
			case ChartSeparators.GradientLine:
				return LegendSeparatorType.GradientLine;
			case ChartSeparators.Line:
				return LegendSeparatorType.Line;
			case ChartSeparators.ThickGradientLine:
				return LegendSeparatorType.ThickGradientLine;
			case ChartSeparators.ThickLine:
				return LegendSeparatorType.ThickLine;
			default:
				return LegendSeparatorType.None;
			}
		}

		private void RenderLegendTitleStyle(ChartLegendTitle chartLegendTitle, Microsoft.Reporting.Chart.WebForms.Legend legend)
		{
			Style style = chartLegendTitle.Style;
			if (style != null)
			{
				StyleInstance style2 = chartLegendTitle.Instance.Style;
				if (MappingHelper.IsStylePropertyDefined(style.Color))
				{
					legend.TitleColor = MappingHelper.GetStyleColor(style, style2);
				}
				if (MappingHelper.IsStylePropertyDefined(style.BackgroundColor))
				{
					legend.TitleBackColor = MappingHelper.GetStyleBackgroundColor(style, style2);
				}
				RenderLegendTitleBorder(style.Border, legend);
				legend.TitleAlignment = GetLegendTitleAlign(MappingHelper.GetStyleTextAlign(style, style2));
			}
			RenderLegendTitleFont(chartLegendTitle, legend);
		}

		private void RenderLegendTitleBorder(Border border, Microsoft.Reporting.Chart.WebForms.Legend legend)
		{
			if (border != null && MappingHelper.IsStylePropertyDefined(border.Color))
			{
				legend.TitleSeparatorColor = MappingHelper.GetStyleBorderColor(border);
			}
		}

		private void SetLegendLayout(ChartLegendLayouts layout, Microsoft.Reporting.Chart.WebForms.Legend legend)
		{
			switch (layout)
			{
			case ChartLegendLayouts.Row:
				legend.LegendStyle = LegendStyle.Row;
				break;
			case ChartLegendLayouts.Column:
				legend.LegendStyle = LegendStyle.Column;
				break;
			case ChartLegendLayouts.AutoTable:
				legend.LegendStyle = LegendStyle.Table;
				legend.TableStyle = LegendTableStyle.Auto;
				break;
			case ChartLegendLayouts.TallTable:
				legend.LegendStyle = LegendStyle.Table;
				legend.TableStyle = LegendTableStyle.Tall;
				break;
			case ChartLegendLayouts.WideTable:
				legend.LegendStyle = LegendStyle.Table;
				legend.TableStyle = LegendTableStyle.Wide;
				break;
			}
		}

		private void GetChartTitlePosition(ChartTitlePositions position, out ContentAlignment alignment, out Docking docking)
		{
			docking = Docking.Top;
			alignment = ContentAlignment.MiddleLeft;
			switch (position)
			{
			case ChartTitlePositions.TopLeft:
				docking = Docking.Top;
				alignment = ContentAlignment.MiddleLeft;
				break;
			case ChartTitlePositions.TopCenter:
				docking = Docking.Top;
				alignment = ContentAlignment.MiddleCenter;
				break;
			case ChartTitlePositions.TopRight:
				docking = Docking.Top;
				alignment = ContentAlignment.MiddleRight;
				break;
			case ChartTitlePositions.LeftTop:
				docking = Docking.Left;
				alignment = ContentAlignment.MiddleRight;
				break;
			case ChartTitlePositions.LeftCenter:
				docking = Docking.Left;
				alignment = ContentAlignment.MiddleCenter;
				break;
			case ChartTitlePositions.LeftBottom:
				docking = Docking.Left;
				alignment = ContentAlignment.MiddleLeft;
				break;
			case ChartTitlePositions.RightTop:
				docking = Docking.Right;
				alignment = ContentAlignment.MiddleLeft;
				break;
			case ChartTitlePositions.RightCenter:
				docking = Docking.Right;
				alignment = ContentAlignment.MiddleCenter;
				break;
			case ChartTitlePositions.RightBottom:
				docking = Docking.Right;
				alignment = ContentAlignment.MiddleRight;
				break;
			case ChartTitlePositions.BottomRight:
				docking = Docking.Bottom;
				alignment = ContentAlignment.MiddleRight;
				break;
			case ChartTitlePositions.BottomCenter:
				docking = Docking.Bottom;
				alignment = ContentAlignment.MiddleCenter;
				break;
			case ChartTitlePositions.BottomLeft:
				docking = Docking.Bottom;
				alignment = ContentAlignment.MiddleLeft;
				break;
			}
		}

		private TextOrientation GetTextOrientation(TextOrientations textOrientations)
		{
			switch (textOrientations)
			{
			case TextOrientations.Horizontal:
				return TextOrientation.Horizontal;
			case TextOrientations.Rotated270:
				return TextOrientation.Rotated270;
			case TextOrientations.Rotated90:
				return TextOrientation.Rotated90;
			case TextOrientations.Stacked:
				return TextOrientation.Stacked;
			default:
				return TextOrientation.Auto;
			}
		}

		private StringAlignment GetLegendAlignment(ChartLegendPositions position)
		{
			switch (position)
			{
			case ChartLegendPositions.TopCenter:
			case ChartLegendPositions.LeftCenter:
			case ChartLegendPositions.RightCenter:
			case ChartLegendPositions.BottomCenter:
				return StringAlignment.Center;
			case ChartLegendPositions.TopRight:
			case ChartLegendPositions.LeftBottom:
			case ChartLegendPositions.RightBottom:
			case ChartLegendPositions.BottomRight:
				return StringAlignment.Far;
			default:
				return StringAlignment.Near;
			}
		}

		private StringAlignment GetLegendTitleAlign(TextAlignments textAlignment)
		{
			switch (textAlignment)
			{
			case TextAlignments.Left:
				return StringAlignment.Near;
			case TextAlignments.Right:
				return StringAlignment.Far;
			default:
				return StringAlignment.Center;
			}
		}

		private LegendDocking GetLegendDocking(ChartLegendPositions position)
		{
			switch (position)
			{
			case ChartLegendPositions.BottomLeft:
			case ChartLegendPositions.BottomCenter:
			case ChartLegendPositions.BottomRight:
				return LegendDocking.Bottom;
			case ChartLegendPositions.TopLeft:
			case ChartLegendPositions.TopCenter:
			case ChartLegendPositions.TopRight:
				return LegendDocking.Top;
			case ChartLegendPositions.LeftTop:
			case ChartLegendPositions.LeftCenter:
			case ChartLegendPositions.LeftBottom:
				return LegendDocking.Left;
			default:
				return LegendDocking.Right;
			}
		}

		private void RenderLegendStyle(ChartLegend chartLegend, Microsoft.Reporting.Chart.WebForms.Legend legend)
		{
			Border border = null;
			Style style = chartLegend.Style;
			if (style != null)
			{
				StyleInstance style2 = chartLegend.Instance.Style;
				if (MappingHelper.IsStylePropertyDefined(chartLegend.Style.Color))
				{
					legend.FontColor = MappingHelper.GetStyleColor(style, style2);
				}
				if (MappingHelper.IsStylePropertyDefined(chartLegend.Style.BackgroundColor))
				{
					legend.BackColor = MappingHelper.GetStyleBackgroundColor(style, style2);
				}
				if (MappingHelper.IsStylePropertyDefined(chartLegend.Style.BackgroundGradientEndColor))
				{
					legend.BackGradientEndColor = MappingHelper.GetStyleBackGradientEndColor(style, style2);
				}
				if (MappingHelper.IsStylePropertyDefined(chartLegend.Style.BackgroundGradientType))
				{
					legend.BackGradientType = GetGradientType(MappingHelper.GetStyleBackGradientType(style, style2));
				}
				if (MappingHelper.IsStylePropertyDefined(chartLegend.Style.BackgroundHatchType))
				{
					legend.BackHatchStyle = GetHatchType(MappingHelper.GetStyleBackgroundHatchType(style, style2));
				}
				if (MappingHelper.IsStylePropertyDefined(chartLegend.Style.ShadowColor))
				{
					legend.ShadowColor = MappingHelper.GetStyleShadowColor(style, style2);
				}
				if (MappingHelper.IsStylePropertyDefined(chartLegend.Style.ShadowOffset))
				{
					legend.ShadowOffset = MappingHelper.GetStyleShadowOffset(style, style2, base.DpiX);
				}
				RenderLegendBackgroundImage(chartLegend.Style.BackgroundImage, legend);
				border = chartLegend.Style.Border;
			}
			RenderLegendBorder(border, legend);
			RenderLegendFont(chartLegend, legend);
		}

		private void RenderLegendColumns(ChartLegendColumnCollection chartLegendColumns, LegendCellColumnCollection legendColumns)
		{
		}

		private void RenderLegendCustomItems(ChartLegendCustomItemCollection chartLegendCustomItems, LegendItemsCollection legendCustomItems)
		{
		}

		private void RenderElementPosition(ChartElementPosition chartElementPosition, ElementPosition elementPosition)
		{
			if (chartElementPosition == null)
			{
				return;
			}
			ReportDoubleProperty left = chartElementPosition.Left;
			if (left != null)
			{
				if (!left.IsExpression)
				{
					elementPosition.X = (float)left.Value;
				}
				else
				{
					elementPosition.X = (float)chartElementPosition.Instance.Left;
				}
			}
			else
			{
				elementPosition.X = 0f;
			}
			left = chartElementPosition.Top;
			if (left != null)
			{
				if (!left.IsExpression)
				{
					elementPosition.Y = (float)left.Value;
				}
				else
				{
					elementPosition.Y = (float)chartElementPosition.Instance.Top;
				}
			}
			else
			{
				elementPosition.Y = 0f;
			}
			left = chartElementPosition.Width;
			if (left != null)
			{
				if (!left.IsExpression)
				{
					elementPosition.Width = (float)left.Value;
				}
				else
				{
					elementPosition.Width = (float)chartElementPosition.Instance.Width;
				}
			}
			else
			{
				elementPosition.Width = 100f - elementPosition.X;
			}
			left = chartElementPosition.Height;
			if (left != null)
			{
				if (!left.IsExpression)
				{
					elementPosition.Height = (float)left.Value;
				}
				else
				{
					elementPosition.Height = (float)chartElementPosition.Instance.Height;
				}
			}
			else
			{
				elementPosition.Height = 100f - elementPosition.Y;
			}
		}

		private void RenderTitles()
		{
			if (m_chart.Titles == null)
			{
				return;
			}
			foreach (ChartTitle title2 in m_chart.Titles)
			{
				Title title = new Title();
				m_coreChart.Titles.Add(title);
				RenderTitle(title2, title);
			}
		}

		private void RenderTitle(ChartTitle chartTitle, Title title)
		{
			SetTitleProperties(chartTitle, title);
			RenderElementPosition(chartTitle.ChartElementPosition, title.Position);
			RenderActionInfo(chartTitle.ActionInfo, title.ToolTip, title);
			RenderTitleStyle(chartTitle, title);
		}

		private void SetTitleProperties(ChartTitle chartTitle, Title title)
		{
			if (chartTitle.Name != null)
			{
				title.Name = chartTitle.Name;
			}
			if (chartTitle.Caption != null)
			{
				if (!chartTitle.Caption.IsExpression)
				{
					if (chartTitle.Caption.Value != null)
					{
						title.Text = chartTitle.Caption.Value;
					}
				}
				else if (chartTitle.Instance.Caption != null)
				{
					title.Text = chartTitle.Instance.Caption;
				}
			}
			if (chartTitle.Position != null)
			{
				ChartTitlePositions chartTitlePositions = ChartTitlePositions.TopCenter;
				chartTitlePositions = (chartTitle.Position.IsExpression ? chartTitle.Instance.Position : chartTitle.Position.Value);
				GetChartTitlePosition(chartTitlePositions, out ContentAlignment alignment, out Docking docking);
				title.Docking = docking;
				title.Alignment = alignment;
			}
			if (chartTitle.DockOffset != null)
			{
				if (!chartTitle.DockOffset.IsExpression)
				{
					title.DockOffset = chartTitle.DockOffset.Value;
				}
				else
				{
					title.DockOffset = chartTitle.Instance.DockOffset;
				}
			}
			if (chartTitle.DockOutsideChartArea != null)
			{
				if (!chartTitle.DockOutsideChartArea.IsExpression)
				{
					title.DockInsideChartArea = !chartTitle.DockOutsideChartArea.Value;
				}
				else
				{
					title.DockInsideChartArea = !chartTitle.Instance.DockOutsideChartArea;
				}
			}
			if (chartTitle.DockToChartArea != null)
			{
				title.DockToChartArea = chartTitle.DockToChartArea;
			}
			if (chartTitle.Hidden != null)
			{
				if (!chartTitle.Hidden.IsExpression)
				{
					title.Visible = !chartTitle.Hidden.Value;
				}
				else
				{
					title.Visible = !chartTitle.Instance.Hidden;
				}
			}
			ReportStringProperty toolTip = chartTitle.ToolTip;
			if (toolTip != null)
			{
				if (!toolTip.IsExpression)
				{
					if (toolTip.Value != null)
					{
						title.ToolTip = toolTip.Value;
					}
				}
				else if (chartTitle.Instance.ToolTip != null)
				{
					title.ToolTip = chartTitle.Instance.ToolTip;
				}
			}
			if (chartTitle.TextOrientation != null)
			{
				if (!chartTitle.TextOrientation.IsExpression)
				{
					title.TextOrientation = GetTextOrientation(chartTitle.TextOrientation.Value);
				}
				else
				{
					title.TextOrientation = GetTextOrientation(chartTitle.Instance.TextOrientation);
				}
			}
		}

		private void RenderTitleStyle(ChartTitle chartTitle, Title title)
		{
			Border border = null;
			Style style = chartTitle.Style;
			if (style != null)
			{
				StyleInstance style2 = chartTitle.Instance.Style;
				if (MappingHelper.IsStylePropertyDefined(chartTitle.Style.Color))
				{
					title.Color = MappingHelper.GetStyleColor(style, style2);
				}
				if (MappingHelper.IsStylePropertyDefined(chartTitle.Style.BackgroundColor))
				{
					title.BackColor = MappingHelper.GetStyleBackgroundColor(style, style2);
				}
				else
				{
					title.BackColor = Color.Transparent;
				}
				if (MappingHelper.IsStylePropertyDefined(chartTitle.Style.BackgroundGradientEndColor))
				{
					title.BackGradientEndColor = MappingHelper.GetStyleBackGradientEndColor(style, style2);
				}
				if (MappingHelper.IsStylePropertyDefined(chartTitle.Style.BackgroundGradientType))
				{
					title.BackGradientType = GetGradientType(MappingHelper.GetStyleBackGradientType(style, style2));
				}
				if (MappingHelper.IsStylePropertyDefined(chartTitle.Style.BackgroundHatchType))
				{
					title.BackHatchStyle = GetHatchType(MappingHelper.GetStyleBackgroundHatchType(style, style2));
				}
				if (MappingHelper.IsStylePropertyDefined(chartTitle.Style.ShadowColor))
				{
					title.ShadowColor = MappingHelper.GetStyleShadowColor(style, style2);
				}
				if (MappingHelper.IsStylePropertyDefined(chartTitle.Style.ShadowOffset))
				{
					title.ShadowOffset = MappingHelper.GetStyleShadowOffset(style, style2, base.DpiX);
				}
				if (MappingHelper.IsStylePropertyDefined(chartTitle.Style.TextEffect))
				{
					title.Style = GetTextStyle(MappingHelper.GetStyleTextEffect(style, style2));
				}
				RenderTitleBackgroundImage(chartTitle.Style.BackgroundImage, title);
				border = chartTitle.Style.Border;
			}
			RenderTitleBorder(border, title);
			RenderTitleFont(chartTitle, title);
		}

		private TextStyle GetTextStyle(TextEffects textEffects)
		{
			switch (textEffects)
			{
			case TextEffects.Embed:
				return TextStyle.Embed;
			case TextEffects.Emboss:
				return TextStyle.Emboss;
			case TextEffects.Frame:
				return TextStyle.Frame;
			case TextEffects.Shadow:
				return TextStyle.Shadow;
			default:
				return TextStyle.Default;
			}
		}

		private void RenderDataLabel(ChartDataLabel chartDataLabel, DataPointAttributes dataPointAttributes, bool isDataPoint)
		{
			if (chartDataLabel != null)
			{
				SetDataLabelProperties(chartDataLabel, dataPointAttributes, isDataPoint);
				RenderDataLabelStyle(chartDataLabel, dataPointAttributes, isDataPoint);
				RenderDataLabelActionInfo(chartDataLabel.ActionInfo, dataPointAttributes.LabelToolTip, dataPointAttributes);
			}
		}

		private void SetDataLabelProperties(ChartDataLabel chartDataLabel, DataPointAttributes dataPointAttributes, bool isDataPoint)
		{
			if (chartDataLabel == null)
			{
				return;
			}
			if (chartDataLabel.Position != null)
			{
				ChartDataLabelPositions dataLabelPositionValue = GetDataLabelPositionValue(chartDataLabel);
				dataPointAttributes.SetAttribute("LabelStyle", GetDataLabelPosition(dataLabelPositionValue));
			}
			if (chartDataLabel.Rotation != null)
			{
				if (!chartDataLabel.Rotation.IsExpression)
				{
					dataPointAttributes.FontAngle = chartDataLabel.Rotation.Value;
				}
				else
				{
					dataPointAttributes.FontAngle = chartDataLabel.Instance.Rotation;
				}
			}
			if (chartDataLabel.UseValueAsLabel != null)
			{
				if (!chartDataLabel.UseValueAsLabel.IsExpression)
				{
					dataPointAttributes.ShowLabelAsValue = chartDataLabel.UseValueAsLabel.Value;
				}
				else
				{
					dataPointAttributes.ShowLabelAsValue = chartDataLabel.Instance.UseValueAsLabel;
				}
			}
			if (!dataPointAttributes.ShowLabelAsValue && chartDataLabel.Label != null)
			{
				if (!chartDataLabel.Label.IsExpression)
				{
					if (chartDataLabel.Label != null)
					{
						dataPointAttributes.Label = chartDataLabel.Label.Value;
					}
				}
				else if (chartDataLabel.Instance.Label != null)
				{
					dataPointAttributes.Label = chartDataLabel.Instance.Label;
				}
			}
			if (chartDataLabel.Visible != null)
			{
				bool flag = false;
				if (!(chartDataLabel.Visible.IsExpression ? chartDataLabel.Instance.Visible : chartDataLabel.Visible.Value))
				{
					if (isDataPoint)
					{
						dataPointAttributes.DeleteAttribute(CommonAttributes.Label);
						dataPointAttributes.DeleteAttribute(CommonAttributes.ShowLabelAsValue);
						HideDataPointLabels(dataPointAttributes);
					}
					else
					{
						dataPointAttributes.Label = "";
						dataPointAttributes.ShowLabelAsValue = false;
					}
				}
			}
			else if (isDataPoint)
			{
				dataPointAttributes.DeleteAttribute(CommonAttributes.Label);
				dataPointAttributes.DeleteAttribute(CommonAttributes.ShowLabelAsValue);
				HideDataPointLabels(dataPointAttributes);
			}
			else
			{
				dataPointAttributes.Label = "";
				dataPointAttributes.ShowLabelAsValue = false;
			}
			ReportStringProperty toolTip = chartDataLabel.ToolTip;
			if (toolTip == null)
			{
				return;
			}
			if (!toolTip.IsExpression)
			{
				if (toolTip.Value != null)
				{
					dataPointAttributes.LabelToolTip = toolTip.Value;
				}
				return;
			}
			string toolTip2 = chartDataLabel.Instance.ToolTip;
			if (toolTip2 != null)
			{
				dataPointAttributes.LabelToolTip = toolTip2;
			}
		}

		private static void HideDataPointLabels(DataPointAttributes dataPointAttributes)
		{
			dataPointAttributes.SetAttribute("LabelsVisible", "false");
		}

		private ChartDataLabelPositions GetDataLabelPositionValue(ChartDataLabel chartDataLabel)
		{
			ChartDataLabelPositions chartDataLabelPositions = ChartDataLabelPositions.Auto;
			if (!chartDataLabel.Position.IsExpression)
			{
				return chartDataLabel.Position.Value;
			}
			return chartDataLabel.Instance.Position;
		}

		private string GetDataLabelPosition(ChartDataLabelPositions position)
		{
			switch (position)
			{
			case ChartDataLabelPositions.Bottom:
				return "Bottom";
			case ChartDataLabelPositions.BottomLeft:
				return "BottomLeft";
			case ChartDataLabelPositions.BottomRight:
				return "BottomRight";
			case ChartDataLabelPositions.Center:
				return "Center";
			case ChartDataLabelPositions.Left:
				return "Left";
			case ChartDataLabelPositions.Outside:
				return "Outside";
			case ChartDataLabelPositions.Right:
				return "Right";
			case ChartDataLabelPositions.Top:
				return "Top";
			case ChartDataLabelPositions.TopLeft:
				return "TopLeft";
			case ChartDataLabelPositions.TopRight:
				return "TopRight";
			default:
				return "Auto";
			}
		}

		private void RenderDataLabelStyle(ChartDataLabel chartDataLabel, DataPointAttributes dataPointAttributes, bool isDataPoint)
		{
			Border border = null;
			Style style = chartDataLabel.Style;
			if (style != null)
			{
				StyleInstance style2 = chartDataLabel.Instance.Style;
				if (MappingHelper.IsStylePropertyDefined(chartDataLabel.Style.BackgroundColor))
				{
					dataPointAttributes.LabelBackColor = MappingHelper.GetStyleBackgroundColor(style, style2);
				}
				if (MappingHelper.IsStylePropertyDefined(chartDataLabel.Style.Color))
				{
					dataPointAttributes.FontColor = MappingHelper.GetStyleColor(style, style2);
				}
				if (MappingHelper.IsStylePropertyDefined(chartDataLabel.Style.Format))
				{
					dataPointAttributes.LabelFormat = MappingHelper.GetStyleFormat(style, style2);
				}
				border = chartDataLabel.Style.Border;
			}
			RenderDataLabelBorder(border, dataPointAttributes);
			RenderDataLabelFont(chartDataLabel, dataPointAttributes, isDataPoint);
		}

		private void RenderDataLabelBorder(Border border, DataPointAttributes dataPointAttributes)
		{
			dataPointAttributes.LabelBorderColor = Color.Black;
			dataPointAttributes.LabelBorderStyle = ChartDashStyle.NotSet;
			if (border != null)
			{
				if (MappingHelper.IsStylePropertyDefined(border.Color))
				{
					dataPointAttributes.LabelBorderColor = MappingHelper.GetStyleBorderColor(border);
				}
				if (MappingHelper.IsStylePropertyDefined(border.Style))
				{
					dataPointAttributes.LabelBorderStyle = GetBorderStyle(MappingHelper.GetStyleBorderStyle(border), isLine: false);
				}
				dataPointAttributes.LabelBorderWidth = MappingHelper.GetStyleBorderWidth(border, base.DpiX);
			}
		}

		private void RenderDataLabelActionInfo(ActionInfo actionInfo, string toolTip, DataPointAttributes dataPointAttributes)
		{
			if (actionInfo == null && string.IsNullOrEmpty(toolTip))
			{
				return;
			}
			string href;
			ActionInfoWithDynamicImageMap actionInfoWithDynamicImageMap = MappingHelper.CreateActionInfoDynamic(m_chart, actionInfo, toolTip, out href);
			if (actionInfoWithDynamicImageMap != null)
			{
				if (href != null)
				{
					dataPointAttributes.LabelHref = href;
				}
				int count = m_actions.Count;
				m_actions.InternalList.Add(actionInfoWithDynamicImageMap);
				dataPointAttributes.LabelTag = count;
			}
		}

		private void RenderDataPointMarker(ChartMarker chartMarker, DataPoint dataPoint, BackgroundImageInfo backgroundImageInfo)
		{
			if (chartMarker != null)
			{
				SetMarkerProperties(chartMarker, dataPoint);
				RenderDataPointMarkerStyle(chartMarker, dataPoint, backgroundImageInfo);
			}
		}

		private void RenderSeriesMarker(ChartMarker chartMarker, Series series)
		{
			if (chartMarker != null)
			{
				SetMarkerProperties(chartMarker, series);
				RenderSeriesMarkerStyle(chartMarker, series);
			}
		}

		private void RenderEmptyPointMarker(ChartMarker chartMarker, DataPointAttributes dataPointAttributes)
		{
			if (chartMarker != null)
			{
				SetMarkerProperties(chartMarker, dataPointAttributes);
				RenderEmptyPointMarkerStyle(chartMarker, dataPointAttributes);
			}
		}

		private void SetMarkerProperties(ChartMarker chartMarker, DataPointAttributes dataPointAttributes)
		{
			if (chartMarker.Size != null)
			{
				if (!chartMarker.Size.IsExpression)
				{
					if (chartMarker.Size.Value != null)
					{
						dataPointAttributes.MarkerSize = MappingHelper.ToIntPixels(chartMarker.Size.Value, base.DpiX);
					}
				}
				else if (chartMarker.Instance.Size != null)
				{
					dataPointAttributes.MarkerSize = MappingHelper.ToIntPixels(chartMarker.Instance.Size, base.DpiX);
				}
			}
			else
			{
				dataPointAttributes.MarkerSize = MappingHelper.ToIntPixels(m_defaultMarkerSize, base.DpiX);
			}
			if (chartMarker.Type != null)
			{
				if (!chartMarker.Type.IsExpression)
				{
					dataPointAttributes.MarkerStyle = GetMarkerStyle(chartMarker.Type.Value);
				}
				else
				{
					dataPointAttributes.MarkerStyle = GetMarkerStyle(chartMarker.Instance.Type);
				}
			}
		}

		private MarkerStyle GetMarkerStyle(ChartMarkerTypes chartMarkerType)
		{
			switch (chartMarkerType)
			{
			case ChartMarkerTypes.Auto:
				if (m_autoMarker == null)
				{
					m_autoMarker = new AutoMarker();
				}
				return m_autoMarker.Current;
			case ChartMarkerTypes.Circle:
				return MarkerStyle.Circle;
			case ChartMarkerTypes.Cross:
				return MarkerStyle.Cross;
			case ChartMarkerTypes.Diamond:
				return MarkerStyle.Diamond;
			case ChartMarkerTypes.Square:
				return MarkerStyle.Square;
			case ChartMarkerTypes.Star10:
				return MarkerStyle.Star10;
			case ChartMarkerTypes.Star4:
				return MarkerStyle.Star4;
			case ChartMarkerTypes.Star5:
				return MarkerStyle.Star5;
			case ChartMarkerTypes.Star6:
				return MarkerStyle.Star6;
			case ChartMarkerTypes.Triangle:
				return MarkerStyle.Triangle;
			default:
				return MarkerStyle.None;
			}
		}

		private void RenderDataPointMarkerStyle(ChartMarker chartMarker, DataPoint dataPoint, BackgroundImageInfo backgroundImageInfo)
		{
			RenderMarkerStyle(chartMarker, dataPoint);
			if (chartMarker.Style != null)
			{
				RenderMarkerBackgroundImage(chartMarker.Style.BackgroundImage, dataPoint, backgroundImageInfo);
			}
		}

		private void RenderSeriesMarkerStyle(ChartMarker chartMarker, Series series)
		{
			RenderMarkerStyle(chartMarker, series);
			if (chartMarker.Style != null)
			{
				RenderMarkerBackgroundImage(chartMarker.Style.BackgroundImage, series, null);
			}
		}

		private void RenderEmptyPointMarkerStyle(ChartMarker chartMarker, DataPointAttributes emptyPoint)
		{
			RenderMarkerStyle(chartMarker, emptyPoint);
			if (chartMarker.Style != null)
			{
				RenderMarkerBackgroundImage(chartMarker.Style.BackgroundImage, emptyPoint, null);
			}
		}

		private void RenderMarkerStyle(ChartMarker chartMarker, DataPointAttributes dataPointAttributes)
		{
			Style style = chartMarker.Style;
			if (style != null)
			{
				StyleInstance style2 = chartMarker.Instance.Style;
				if (MappingHelper.IsStylePropertyDefined(chartMarker.Style.Color))
				{
					dataPointAttributes.MarkerColor = MappingHelper.GetStyleColor(style, style2);
				}
				RenderMarkerBorder(chartMarker.Style.Border, dataPointAttributes);
			}
		}

		private void RenderMarkerBorder(Border border, DataPointAttributes dataPointAttributes)
		{
			if (border != null)
			{
				if (MappingHelper.IsStylePropertyDefined(border.Color))
				{
					dataPointAttributes.MarkerBorderColor = MappingHelper.GetStyleBorderColor(border);
				}
				dataPointAttributes.MarkerBorderWidth = MappingHelper.GetStyleBorderWidth(border, base.DpiX);
			}
		}

		private void RenderMarkerBackgroundImage(BackgroundImage backgroundImage, DataPointAttributes dataPointAttributes, BackgroundImageInfo backgroundImageInfo)
		{
			dataPointAttributes.MarkerImage = GetImageName(backgroundImage, backgroundImageInfo);
			GetBackgroundImageProperties(backgroundImage, out ChartImageWrapMode _, out ChartImageAlign _, out Color backImageTransparentColor);
			dataPointAttributes.MarkerImageTransparentColor = backImageTransparentColor;
		}

		private void RenderSmartLabels(ChartSmartLabel chartSmartLabels, SmartLabelsStyle smartLabels)
		{
			smartLabels.Enabled = true;
			smartLabels.CalloutLineWidth = MappingHelper.ToIntPixels(m_defaultCalloutLineWidth, base.DpiX);
			smartLabels.MaxMovingDistance = MappingHelper.ToPixels(m_defaultMaxMovingDistance, base.DpiX);
			if (chartSmartLabels != null)
			{
				SetSmartLabelsProperties(chartSmartLabels, smartLabels);
				RenderNoMoveDirections(chartSmartLabels.NoMoveDirections, smartLabels);
			}
		}

		private void SetSmartLabelsProperties(ChartSmartLabel chartSmartLabels, SmartLabelsStyle smartLabels)
		{
			if (chartSmartLabels.Disabled != null)
			{
				if (!chartSmartLabels.Disabled.IsExpression)
				{
					smartLabels.Enabled = !chartSmartLabels.Disabled.Value;
				}
				else
				{
					smartLabels.Enabled = !chartSmartLabels.Instance.Disabled;
				}
			}
			if (chartSmartLabels.AllowOutSidePlotArea != null)
			{
				if (!chartSmartLabels.AllowOutSidePlotArea.IsExpression)
				{
					smartLabels.AllowOutsidePlotArea = GetLabelOutsidePlotAreaStyle(chartSmartLabels.AllowOutSidePlotArea.Value);
				}
				else
				{
					smartLabels.AllowOutsidePlotArea = GetLabelOutsidePlotAreaStyle(chartSmartLabels.Instance.AllowOutSidePlotArea);
				}
			}
			Color color = Color.Empty;
			if (chartSmartLabels.CalloutBackColor != null)
			{
				if (MappingHelper.GetColorFromReportColorProperty(chartSmartLabels.CalloutBackColor, ref color))
				{
					smartLabels.CalloutBackColor = color;
				}
				else if (chartSmartLabels.Instance.CalloutBackColor != null)
				{
					smartLabels.CalloutBackColor = chartSmartLabels.Instance.CalloutBackColor.ToColor();
				}
			}
			if (chartSmartLabels.CalloutLineAnchor != null)
			{
				if (!chartSmartLabels.CalloutLineAnchor.IsExpression)
				{
					smartLabels.CalloutLineAnchorCap = GetCalloutLineAnchor(chartSmartLabels.CalloutLineAnchor.Value);
				}
				else
				{
					smartLabels.CalloutLineAnchorCap = GetCalloutLineAnchor(chartSmartLabels.Instance.CalloutLineAnchor);
				}
			}
			if (chartSmartLabels.CalloutLineColor != null)
			{
				if (MappingHelper.GetColorFromReportColorProperty(chartSmartLabels.CalloutLineColor, ref color))
				{
					smartLabels.CalloutLineColor = color;
				}
				else if (chartSmartLabels.Instance.CalloutLineColor != null)
				{
					smartLabels.CalloutLineColor = chartSmartLabels.Instance.CalloutLineColor.ToColor();
				}
			}
			if (chartSmartLabels.CalloutLineStyle != null)
			{
				if (!chartSmartLabels.CalloutLineStyle.IsExpression)
				{
					smartLabels.CalloutLineStyle = GetCalloutLineStyle(chartSmartLabels.CalloutLineStyle.Value);
				}
				else
				{
					smartLabels.CalloutLineStyle = GetCalloutLineStyle(chartSmartLabels.Instance.CalloutLineStyle);
				}
			}
			if (chartSmartLabels.CalloutLineWidth != null)
			{
				if (!chartSmartLabels.CalloutLineWidth.IsExpression)
				{
					if (chartSmartLabels.CalloutLineWidth.Value != null)
					{
						smartLabels.CalloutLineWidth = MappingHelper.ToIntPixels(chartSmartLabels.CalloutLineWidth.Value, base.DpiX);
					}
				}
				else if (chartSmartLabels.Instance.CalloutLineWidth != null)
				{
					smartLabels.CalloutLineWidth = MappingHelper.ToIntPixels(chartSmartLabels.Instance.CalloutLineWidth, base.DpiX);
				}
			}
			if (chartSmartLabels.CalloutStyle != null)
			{
				if (!chartSmartLabels.CalloutStyle.IsExpression)
				{
					smartLabels.CalloutStyle = GetCalloutStyle(chartSmartLabels.CalloutStyle.Value);
				}
				else
				{
					smartLabels.CalloutStyle = GetCalloutStyle(chartSmartLabels.Instance.CalloutStyle);
				}
			}
			if (chartSmartLabels.ShowOverlapped != null)
			{
				if (!chartSmartLabels.ShowOverlapped.IsExpression)
				{
					smartLabels.HideOverlapped = !chartSmartLabels.ShowOverlapped.Value;
				}
				else
				{
					smartLabels.HideOverlapped = !chartSmartLabels.Instance.ShowOverlapped;
				}
			}
			if (chartSmartLabels.MarkerOverlapping != null)
			{
				if (!chartSmartLabels.MarkerOverlapping.IsExpression)
				{
					smartLabels.MarkerOverlapping = chartSmartLabels.MarkerOverlapping.Value;
				}
				else
				{
					smartLabels.MarkerOverlapping = chartSmartLabels.Instance.MarkerOverlapping;
				}
			}
			if (chartSmartLabels.MaxMovingDistance != null)
			{
				if (!chartSmartLabels.MaxMovingDistance.IsExpression)
				{
					if (chartSmartLabels.MaxMovingDistance.Value != null)
					{
						smartLabels.MaxMovingDistance = MappingHelper.ToPixels(chartSmartLabels.MaxMovingDistance.Value, base.DpiX);
					}
				}
				else if (chartSmartLabels.Instance.MaxMovingDistance != null)
				{
					smartLabels.MaxMovingDistance = MappingHelper.ToPixels(chartSmartLabels.Instance.MaxMovingDistance, base.DpiX);
				}
			}
			if (chartSmartLabels.MinMovingDistance == null)
			{
				return;
			}
			if (!chartSmartLabels.MinMovingDistance.IsExpression)
			{
				if (chartSmartLabels.MinMovingDistance.Value != null)
				{
					smartLabels.MinMovingDistance = MappingHelper.ToPixels(chartSmartLabels.MinMovingDistance.Value, base.DpiX);
				}
			}
			else if (chartSmartLabels.Instance.MinMovingDistance != null)
			{
				smartLabels.MinMovingDistance = MappingHelper.ToPixels(chartSmartLabels.Instance.MinMovingDistance, base.DpiX);
			}
		}

		private LineAnchorCap GetCalloutLineAnchor(ChartCalloutLineAnchor chartCalloutLineAnchor)
		{
			switch (chartCalloutLineAnchor)
			{
			case ChartCalloutLineAnchor.Arrow:
				return LineAnchorCap.Arrow;
			case ChartCalloutLineAnchor.Diamond:
				return LineAnchorCap.Diamond;
			case ChartCalloutLineAnchor.Round:
				return LineAnchorCap.Round;
			case ChartCalloutLineAnchor.Square:
				return LineAnchorCap.Square;
			default:
				return LineAnchorCap.None;
			}
		}

		private ChartDashStyle GetCalloutLineStyle(ChartCalloutLineStyle chartCalloutLineStyle)
		{
			switch (chartCalloutLineStyle)
			{
			case ChartCalloutLineStyle.DashDot:
				return ChartDashStyle.DashDot;
			case ChartCalloutLineStyle.DashDotDot:
				return ChartDashStyle.DashDotDot;
			case ChartCalloutLineStyle.Dashed:
				return ChartDashStyle.Dash;
			case ChartCalloutLineStyle.Dotted:
				return ChartDashStyle.Dot;
			case ChartCalloutLineStyle.Solid:
			case ChartCalloutLineStyle.Double:
				return ChartDashStyle.Solid;
			default:
				return ChartDashStyle.NotSet;
			}
		}

		private LabelCalloutStyle GetCalloutStyle(ChartCalloutStyle chartCalloutStyle)
		{
			switch (chartCalloutStyle)
			{
			case ChartCalloutStyle.Box:
				return LabelCalloutStyle.Box;
			case ChartCalloutStyle.Underline:
				return LabelCalloutStyle.Underlined;
			default:
				return LabelCalloutStyle.None;
			}
		}

		private void RenderNoMoveDirections(ChartNoMoveDirections chartNoMoveDirections, SmartLabelsStyle smartLabelsStyle)
		{
			if (chartNoMoveDirections == null)
			{
				return;
			}
			LabelAlignmentTypes labelAlignmentTypes = (LabelAlignmentTypes)0;
			if (chartNoMoveDirections.Down != null)
			{
				if (!chartNoMoveDirections.Down.IsExpression)
				{
					if (!chartNoMoveDirections.Down.Value)
					{
						labelAlignmentTypes |= LabelAlignmentTypes.Bottom;
					}
				}
				else if (!chartNoMoveDirections.Instance.Down)
				{
					labelAlignmentTypes |= LabelAlignmentTypes.Bottom;
				}
			}
			else
			{
				labelAlignmentTypes |= LabelAlignmentTypes.Bottom;
			}
			if (chartNoMoveDirections.DownLeft != null)
			{
				if (!chartNoMoveDirections.DownLeft.IsExpression)
				{
					if (!chartNoMoveDirections.DownLeft.Value)
					{
						labelAlignmentTypes |= LabelAlignmentTypes.BottomLeft;
					}
				}
				else if (!chartNoMoveDirections.Instance.DownLeft)
				{
					labelAlignmentTypes |= LabelAlignmentTypes.BottomLeft;
				}
			}
			else
			{
				labelAlignmentTypes |= LabelAlignmentTypes.BottomLeft;
			}
			if (chartNoMoveDirections.DownRight != null)
			{
				if (!chartNoMoveDirections.DownRight.IsExpression)
				{
					if (!chartNoMoveDirections.DownRight.Value)
					{
						labelAlignmentTypes |= LabelAlignmentTypes.BottomRight;
					}
				}
				else if (!chartNoMoveDirections.Instance.DownRight)
				{
					labelAlignmentTypes |= LabelAlignmentTypes.BottomRight;
				}
			}
			else
			{
				labelAlignmentTypes |= LabelAlignmentTypes.BottomRight;
			}
			if (chartNoMoveDirections.Left != null)
			{
				if (!chartNoMoveDirections.Left.IsExpression)
				{
					if (!chartNoMoveDirections.Left.Value)
					{
						labelAlignmentTypes |= LabelAlignmentTypes.Left;
					}
				}
				else if (!chartNoMoveDirections.Instance.Left)
				{
					labelAlignmentTypes |= LabelAlignmentTypes.Left;
				}
			}
			else
			{
				labelAlignmentTypes |= LabelAlignmentTypes.Left;
			}
			if (chartNoMoveDirections.Right != null)
			{
				if (!chartNoMoveDirections.Right.IsExpression)
				{
					if (!chartNoMoveDirections.Right.Value)
					{
						labelAlignmentTypes |= LabelAlignmentTypes.Right;
					}
				}
				else if (!chartNoMoveDirections.Instance.Right)
				{
					labelAlignmentTypes |= LabelAlignmentTypes.Right;
				}
			}
			else
			{
				labelAlignmentTypes |= LabelAlignmentTypes.Right;
			}
			if (chartNoMoveDirections.Up != null)
			{
				if (!chartNoMoveDirections.Up.IsExpression)
				{
					if (!chartNoMoveDirections.Up.Value)
					{
						labelAlignmentTypes |= LabelAlignmentTypes.Top;
					}
				}
				else if (!chartNoMoveDirections.Instance.Up)
				{
					labelAlignmentTypes |= LabelAlignmentTypes.Top;
				}
			}
			else
			{
				labelAlignmentTypes |= LabelAlignmentTypes.Top;
			}
			if (chartNoMoveDirections.UpLeft != null)
			{
				if (!chartNoMoveDirections.UpLeft.IsExpression)
				{
					if (!chartNoMoveDirections.UpLeft.Value)
					{
						labelAlignmentTypes |= LabelAlignmentTypes.TopLeft;
					}
				}
				else if (!chartNoMoveDirections.Instance.UpLeft)
				{
					labelAlignmentTypes |= LabelAlignmentTypes.TopLeft;
				}
			}
			else
			{
				labelAlignmentTypes |= LabelAlignmentTypes.TopLeft;
			}
			if (chartNoMoveDirections.UpRight != null)
			{
				if (!chartNoMoveDirections.UpRight.IsExpression)
				{
					if (!chartNoMoveDirections.UpRight.Value)
					{
						labelAlignmentTypes |= LabelAlignmentTypes.TopRight;
					}
				}
				else if (!chartNoMoveDirections.Instance.UpRight)
				{
					labelAlignmentTypes |= LabelAlignmentTypes.TopRight;
				}
			}
			else
			{
				labelAlignmentTypes |= LabelAlignmentTypes.TopRight;
			}
			smartLabelsStyle.MovingDirection = labelAlignmentTypes;
		}

		private void RenderAnnotations()
		{
		}

		private void RenderData()
		{
			RenderSeriesGroupings();
			PostProcessData();
			RenderCategoryGrouping();
			RenderSpecialChartTypes();
			OnPostApplyData();
			RenderDerivedSeriesCollecion();
		}

		private void RenderSeriesGroupings()
		{
			RenderSeriesGroupingCollection(m_chart.SeriesHierarchy.MemberCollection);
		}

		private void RenderSeriesGroupingCollection(ChartMemberCollection seriesGroupingCollection)
		{
			if (!m_multiRow)
			{
				m_multiRow = (seriesGroupingCollection.Count > 1);
			}
			foreach (ChartMember item in seriesGroupingCollection)
			{
				RenderSeriesGrouping(item);
			}
		}

		private void RenderSeriesGrouping(ChartMember seriesGrouping)
		{
			if (!seriesGrouping.IsStatic)
			{
				ChartDynamicMemberInstance chartDynamicMemberInstance = (ChartDynamicMemberInstance)seriesGrouping.Instance;
				chartDynamicMemberInstance.ResetContext();
				m_multiRow = true;
				while (chartDynamicMemberInstance.MoveNext())
				{
					if (seriesGrouping.Children != null)
					{
						RenderSeriesGroupingCollection(seriesGrouping.Children);
					}
					else
					{
						RenderSeries(seriesGrouping);
					}
				}
			}
			else if (seriesGrouping.Children != null)
			{
				RenderSeriesGroupingCollection(seriesGrouping.Children);
			}
			else
			{
				RenderSeries(seriesGrouping);
			}
		}

		private void RenderCategoryGroupings(ChartSeries chartSeries, ChartMember seriesGrouping, SeriesInfo seriesInfo)
		{
			RenderCategoryGroupingCollection(chartSeries, seriesGrouping, m_chart.CategoryHierarchy.MemberCollection, seriesInfo);
		}

		private void RenderCategoryGrouping(ChartSeries chartSeries, ChartMember seriesGrouping, ChartMember categoryGrouping, SeriesInfo seriesInfo)
		{
			if (!categoryGrouping.IsStatic)
			{
				ChartDynamicMemberInstance chartDynamicMemberInstance = (ChartDynamicMemberInstance)categoryGrouping.Instance;
				chartDynamicMemberInstance.ResetContext();
				m_multiColumn = true;
				while (chartDynamicMemberInstance.MoveNext())
				{
					if (categoryGrouping.Children != null)
					{
						RenderCategoryGroupingCollection(chartSeries, seriesGrouping, categoryGrouping.Children, seriesInfo);
					}
					else
					{
						RenderDataPoint(chartSeries, seriesGrouping, categoryGrouping, seriesInfo, DataPointShowsInLegend(chartSeries));
					}
				}
			}
			else if (categoryGrouping.Children != null)
			{
				RenderCategoryGroupingCollection(chartSeries, seriesGrouping, categoryGrouping.Children, seriesInfo);
			}
			else
			{
				RenderDataPoint(chartSeries, seriesGrouping, categoryGrouping, seriesInfo, DataPointShowsInLegend(chartSeries));
			}
		}

		private void RenderCategoryGroupingCollection(ChartSeries chartSeries, ChartMember seriesGrouping, ChartMemberCollection categoryGroupingCollection, SeriesInfo seriesInfo)
		{
			_ = seriesInfo.Series.Points.Count;
			if (!m_multiColumn)
			{
				m_multiColumn = (categoryGroupingCollection.Count > 1);
			}
			foreach (ChartMember item in categoryGroupingCollection)
			{
				RenderCategoryGrouping(chartSeries, seriesGrouping, item, seriesInfo);
			}
		}

		private void RenderCategoryGrouping()
		{
			foreach (KeyValuePair<string, ChartAreaInfo> item in m_chartAreaInfoDictionary)
			{
				bool flag = CanSetCategoryGroupingLabels(item.Value);
				bool flag2 = VisualizeCategoryGrouping(item.Value);
				CategoryNodeCollection categoryNodes = (!flag2) ? null : GetCategoryNodes(item.Key);
				if (flag || flag2)
				{
					RenderChartAreaCategoryGroupings(item, flag, categoryNodes);
				}
			}
		}

		private bool VisualizeCategoryGrouping(ChartAreaInfo chartAreaInfo)
		{
			foreach (SeriesInfo seriesInfo in chartAreaInfo.SeriesInfoList)
			{
				if (seriesInfo.Series.ChartType == SeriesChartType.Sunburst)
				{
					return true;
				}
			}
			return false;
		}

		private void RenderChartAreaCategoryGroupings(KeyValuePair<string, ChartAreaInfo> seriesInfoList, bool setCategoryGroupingLabels, CategoryNodeCollection categoryNodes)
		{
			int numberOfPoints = 0;
			foreach (ChartMember item in m_chart.CategoryHierarchy.MemberCollection)
			{
				RenderCategoryGrouping(item, seriesInfoList, ref numberOfPoints, setCategoryGroupingLabels, categoryNodes);
			}
		}

		private void RenderCategoryGrouping(ChartMember categoryGrouping, KeyValuePair<string, ChartAreaInfo> seriesInfoList, ref int numberOfPoints, bool setCategoryGroupingLabels, CategoryNodeCollection categoryNodes)
		{
			if (!categoryGrouping.IsStatic)
			{
				ChartDynamicMemberInstance chartDynamicMemberInstance = (ChartDynamicMemberInstance)categoryGrouping.Instance;
				chartDynamicMemberInstance.ResetContext();
				while (chartDynamicMemberInstance.MoveNext())
				{
					CategoryNode categoryNode = GetCategoryNode(categoryGrouping, categoryNodes);
					if (categoryGrouping.Children != null)
					{
						RenderCategoryGroupingChildren(categoryGrouping, seriesInfoList, ref numberOfPoints, setCategoryGroupingLabels, categoryNode);
						continue;
					}
					SetDataPointsCategoryGrouping(categoryGrouping, seriesInfoList, numberOfPoints, setCategoryGroupingLabels, categoryNode);
					numberOfPoints++;
				}
			}
			else
			{
				CategoryNode categoryNode2 = GetCategoryNode(categoryGrouping, categoryNodes);
				if (categoryGrouping.Children != null)
				{
					RenderCategoryGroupingChildren(categoryGrouping, seriesInfoList, ref numberOfPoints, setCategoryGroupingLabels, categoryNode2);
					return;
				}
				SetDataPointsCategoryGrouping(categoryGrouping, seriesInfoList, numberOfPoints, setCategoryGroupingLabels, categoryNode2);
				numberOfPoints++;
			}
		}

		private CategoryNodeCollection GetCategoryNodes(string chartAreaName)
		{
			Microsoft.Reporting.Chart.WebForms.ChartArea chartArea = GetChartArea(chartAreaName);
			if (chartArea != null)
			{
				return chartArea.CategoryNodes = new CategoryNodeCollection(null);
			}
			return null;
		}

		private CategoryNode GetCategoryNode(ChartMember categoryGrouping, CategoryNodeCollection categoryNodes)
		{
			if (categoryNodes != null)
			{
				CategoryNode categoryNode = new CategoryNode(categoryNodes, IsCategoryEmpty(categoryGrouping), GetGroupingLabel(categoryGrouping));
				categoryNodes.Add(categoryNode);
				return categoryNode;
			}
			return null;
		}

		private static CategoryNodeCollection GetCategoryChildren(CategoryNode categoryNode)
		{
			if (categoryNode != null)
			{
				categoryNode.Children = new CategoryNodeCollection(categoryNode);
				return categoryNode.Children;
			}
			return null;
		}

		private bool IsCategoryEmpty(ChartMember categoryGrouping)
		{
			if (categoryGrouping.Group != null)
			{
				GroupExpressionValueCollection groupExpressions = categoryGrouping.Group.Instance.GroupExpressions;
				if (groupExpressions != null)
				{
					for (int i = 0; i < groupExpressions.Count; i++)
					{
						if (categoryGrouping.Group.Instance.GroupExpressions[i] != null)
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		private bool IsDynamicOrHasDynamicParentMember(ChartMember member)
		{
			if (member == null)
			{
				return false;
			}
			if (!member.IsStatic)
			{
				return true;
			}
			if (IsDynamicOrHasDynamicParentMember(member.Parent))
			{
				return true;
			}
			return false;
		}

		private bool HasDynamicMember(ChartMemberCollection members)
		{
			if (members == null)
			{
				return false;
			}
			foreach (ChartMember member in members)
			{
				if (!member.IsStatic)
				{
					return true;
				}
				if (HasDynamicMember(member.Children))
				{
					return true;
				}
			}
			return false;
		}

		private void RenderCategoryGroupingChildren(ChartMember categoryGrouping, KeyValuePair<string, ChartAreaInfo> seriesInfoList, ref int numberOfPoints, bool setCategoryGroupingLabels, CategoryNode categoryNode)
		{
			int num = numberOfPoints;
			CategoryNodeCollection categoryChildren = GetCategoryChildren(categoryNode);
			foreach (ChartMember child in categoryGrouping.Children)
			{
				RenderCategoryGrouping(child, seriesInfoList, ref numberOfPoints, setCategoryGroupingLabels, categoryChildren);
			}
			if (setCategoryGroupingLabels)
			{
				AddAxisGroupingLabel(categoryGrouping, seriesInfoList, num + 1, numberOfPoints);
			}
		}

		private void SetDataPointsCategoryGrouping(ChartMember categoryGrouping, KeyValuePair<string, ChartAreaInfo> seriesInfoList, int index, bool setCategoryGroupingLabels, CategoryNode categoryNode)
		{
			foreach (SeriesInfo seriesInfo in seriesInfoList.Value.SeriesInfoList)
			{
				DataPoint dataPoint;
				try
				{
					dataPoint = seriesInfo.Series.Points[index];
				}
				catch (Exception ex)
				{
					if (AsynchronousExceptionDetection.IsStoppingException(ex))
					{
						throw;
					}
					Global.Tracer.Trace(TraceLevel.Verbose, ex.Message);
					continue;
				}
				SetDataPointGrouping(categoryGrouping, seriesInfoList, dataPoint, seriesInfo, setCategoryGroupingLabels, categoryNode);
			}
		}

		private void SetDataPointGrouping(ChartMember categoryGrouping, KeyValuePair<string, ChartAreaInfo> seriesInfoList, DataPoint dataPoint, SeriesInfo seriesInfo, bool setCategoryGroupingLabels, CategoryNode categoryNode)
		{
			if (categoryNode == null)
			{
				if (setCategoryGroupingLabels && CanSetDataPointAxisLabel(seriesInfo.Series, dataPoint))
				{
					dataPoint.AxisLabel = GetFormattedGroupingLabel(categoryGrouping, seriesInfoList.Key, seriesInfo.ChartCategoryAxis);
				}
			}
			else
			{
				categoryNode.AddDataPoint(dataPoint);
			}
		}

		private void AddAxisGroupingLabel(ChartMember categoryGrouping, KeyValuePair<string, ChartAreaInfo> seriesInfoList, double startPointIndex, double endPointIndex)
		{
			int groupingLevel = GetGroupingLevel(categoryGrouping);
			LabelMark mark = LabelMark.LineSideMark;
			double num = 0.4;
			bool flag = false;
			bool flag2 = false;
			foreach (SeriesInfo seriesInfo in seriesInfoList.Value.SeriesInfoList)
			{
				if (seriesInfo.Series.XAxisType == AxisType.Primary)
				{
					flag = true;
				}
				else if (seriesInfo.Series.XAxisType == AxisType.Secondary)
				{
					flag2 = true;
				}
			}
			Microsoft.Reporting.Chart.WebForms.ChartArea chartArea = GetChartArea(seriesInfoList.Key);
			if (chartArea == null)
			{
				return;
			}
			if (flag)
			{
				if (!chartArea.AxisX.Margin)
				{
					chartArea.AxisX.LabelStyle.ShowEndLabels = true;
				}
				chartArea.AxisX.CustomLabels.Add(startPointIndex - num, endPointIndex + num, GetGroupingLabel(categoryGrouping), groupingLevel, mark);
			}
			if (flag2 && !flag)
			{
				if (!chartArea.AxisX2.Margin)
				{
					chartArea.AxisX2.LabelStyle.ShowEndLabels = true;
				}
				chartArea.AxisX2.CustomLabels.Add(startPointIndex - num, endPointIndex + num, GetGroupingLabel(categoryGrouping), groupingLevel, mark);
			}
		}

		private void RenderSeries(ChartMember seriesGrouping)
		{
			ChartSeries chartSeries = m_chart.ChartData.SeriesCollection[seriesGrouping.MemberCellIndex];
			SeriesInfo seriesInfo = null;
			bool num = DataPointShowsInLegend(chartSeries);
			if (num)
			{
				seriesInfo = GetShapeSeriesOnSameChartArea(chartSeries);
				if (seriesInfo == null)
				{
					seriesInfo = CreateSeries(seriesGrouping, chartSeries);
					seriesInfo.IsExploded = IsSeriesExploded(chartSeries);
					seriesInfo.IsAttachedToScalarAxis = IsSeriesAttachedToScalarAxis(seriesInfo);
					seriesInfo.Series.SetAttribute(m_pieAutoAxisLabelsName, "False");
					RenderSeries(seriesGrouping, seriesInfo.ChartSeries, seriesInfo.Series, seriesInfo.IsLine);
				}
			}
			else
			{
				seriesInfo = CreateSeries(seriesGrouping, chartSeries);
				seriesInfo.IsLine = IsSeriesLine(chartSeries);
				seriesInfo.IsRange = IsSeriesRange(chartSeries);
				seriesInfo.IsBubble = IsSeriesBubble(chartSeries);
				seriesInfo.IsAttachedToScalarAxis = IsSeriesAttachedToScalarAxis(seriesInfo);
				seriesInfo.IsGradientPerDataPointSupported = IsGradientPerDataPointSupported(chartSeries);
				RenderSeries(seriesGrouping, seriesInfo.ChartSeries, seriesInfo.Series, seriesInfo.IsLine);
			}
			seriesInfo.DataPointBackgroundImageInfoCollection.Initialize(chartSeries);
			RenderCategoryGroupings(chartSeries, seriesGrouping, seriesInfo);
			if (!num)
			{
				AdjustNonShapeSeriesAppearance(seriesInfo);
			}
			OnPostApplySeriesData(seriesInfo.Series);
		}

		private void AdjustNonShapeSeriesAppearance(SeriesInfo seriesInfo)
		{
			bool flag = false;
			if (seriesInfo.IsDataPointColorEmpty.HasValue)
			{
				flag = seriesInfo.IsDataPointColorEmpty.Value;
			}
			else if (seriesInfo.DefaultDataPointAppearance != null)
			{
				flag = (seriesInfo.DefaultDataPointAppearance.Color == Color.Empty);
			}
			if (!flag)
			{
				DataPoint dataPoint = GetFirstNonEmptyDataPoint(seriesInfo.Series);
				if (dataPoint == null)
				{
					dataPoint = seriesInfo.DefaultDataPointAppearance;
				}
				if (dataPoint != null)
				{
					seriesInfo.Series.Color = dataPoint.Color;
				}
				else
				{
					seriesInfo.Series.Color = Color.White;
				}
			}
			if (seriesInfo.IsDataPointHatchDefined && m_hatcher != null)
			{
				seriesInfo.Series.BackHatchStyle = ChartHatchStyle.None;
			}
			if (!seriesInfo.IsGradientPerDataPointSupported && seriesInfo.IsGradientSupported)
			{
				if (seriesInfo.Color.HasValue)
				{
					seriesInfo.Series.Color = seriesInfo.Color.Value;
				}
				if (seriesInfo.BackGradientEndColor.HasValue)
				{
					seriesInfo.Series.BackGradientEndColor = seriesInfo.BackGradientEndColor.Value;
				}
				if (seriesInfo.BackGradientType.HasValue)
				{
					seriesInfo.Series.BackGradientType = seriesInfo.BackGradientType.Value;
				}
			}
		}

		private void RenderSpecialChartTypes()
		{
			foreach (ChartAreaInfo value in m_chartAreaInfoDictionary.Values)
			{
				foreach (SeriesInfo seriesInfo in value.SeriesInfoList)
				{
					if (IsSeriesCollectedPie(seriesInfo.Series))
					{
						ShowPieAsCollected(seriesInfo.Series);
						continue;
					}
					bool flag = IsSeriesPareto(seriesInfo.Series);
					bool flag2 = IsSeriesHistogram(seriesInfo.Series);
					if ((flag || flag2) && ChartAreaHasMultipleSeries(seriesInfo.Series) && IsCategoryHierarchyValidForHistogramAndPareto(seriesInfo) && !IsDynamicOrHasDynamicParentMember(seriesInfo.SeriesGrouping))
					{
						if (flag)
						{
							MakeParetoChart(seriesInfo.Series);
						}
						else if (flag2)
						{
							MakeHistogramChart(seriesInfo.Series);
						}
					}
				}
			}
		}

		private bool ChartAreaHasMultipleSeries(Series series)
		{
			bool result = true;
			foreach (Series item in m_coreChart.Series)
			{
				if (series != item && series.ChartArea == item.ChartArea)
				{
					return false;
				}
			}
			return result;
		}

		private bool IsCategoryHierarchyValidForHistogramAndPareto(SeriesInfo seriesInfo)
		{
			if (m_chart.CategoryHierarchy.MemberCollection.Count == 0)
			{
				return true;
			}
			if (HasDynamicMember(m_chart.CategoryHierarchy.MemberCollection[0].Children) || seriesInfo.ChartSeries.Count > 1)
			{
				return false;
			}
			return true;
		}

		private void MakeParetoChart(Series series)
		{
			try
			{
				new ParetoHelper().MakeParetoChart(m_coreChart, series.Name, series.LegendText + " Pareto Line");
			}
			catch (Exception ex)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(ex))
				{
					throw;
				}
				Global.Tracer.Trace(TraceLevel.Verbose, ex.Message);
			}
		}

		private void MakeHistogramChart(Series series)
		{
			try
			{
				HistogramHelper histogramHelper = new HistogramHelper();
				try
				{
					histogramHelper.SegmentIntervalNumber = int.Parse(series["HistogramSegmentIntervalNumber"], CultureInfo.InvariantCulture);
				}
				catch (Exception ex)
				{
					if (AsynchronousExceptionDetection.IsStoppingException(ex))
					{
						throw;
					}
					Global.Tracer.Trace(TraceLevel.Verbose, ex.Message);
				}
				try
				{
					histogramHelper.SegmentIntervalWidth = double.Parse(series["HistogramSegmentIntervalWidth"], CultureInfo.InvariantCulture);
				}
				catch (Exception ex2)
				{
					if (AsynchronousExceptionDetection.IsStoppingException(ex2))
					{
						throw;
					}
					Global.Tracer.Trace(TraceLevel.Verbose, ex2.Message);
				}
				try
				{
					histogramHelper.ShowPercentOnSecondaryYAxis = bool.Parse(series["HistogramShowPercentOnSecondaryYAxis"]);
				}
				catch (Exception ex3)
				{
					if (AsynchronousExceptionDetection.IsStoppingException(ex3))
					{
						throw;
					}
					Global.Tracer.Trace(TraceLevel.Verbose, ex3.Message);
				}
				histogramHelper.CreateHistogram(m_coreChart, series.Name, series.Name + "_Histogram", series.LegendText + " Histogram");
			}
			catch (Exception ex4)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(ex4))
				{
					throw;
				}
				Global.Tracer.Trace(TraceLevel.Verbose, ex4.Message);
			}
		}

		private void ShowPieAsCollected(Series series)
		{
			try
			{
				CollectedPieHelper collectedPieHelper = new CollectedPieHelper(m_coreChart);
				bool result = false;
				bool.TryParse(series["CollectedChartShowLegend"], out result);
				collectedPieHelper.ShowCollectedLegend = result;
				bool result2 = false;
				bool.TryParse(series["CollectedChartShowLabels"], out result2);
				collectedPieHelper.ShowCollectedPointLabels = result2;
				string text = series["CollectedLabel"];
				if (text != null)
				{
					collectedPieHelper.CollectedLabel = text;
				}
				string text2 = series["CollectedColor"];
				if (text2 != null && text2 != "")
				{
					Color empty = Color.Empty;
					try
					{
						ColorConverter colorConverter = new ColorConverter();
						empty = ((string.Compare(text2, "Empty", StringComparison.OrdinalIgnoreCase) != 0) ? ((Color)colorConverter.ConvertFromString(null, CultureInfo.InvariantCulture, text2)) : Color.Empty);
					}
					catch (Exception e)
					{
						if (AsynchronousExceptionDetection.IsStoppingException(e))
						{
							throw;
						}
						empty = Color.Empty;
					}
					collectedPieHelper.SliceColor = empty;
				}
				double num = 5.0;
				try
				{
					num = double.Parse(series["CollectedThreshold"], CultureInfo.InvariantCulture);
				}
				catch
				{
					num = 5.0;
				}
				collectedPieHelper.CollectedPercentage = num;
				collectedPieHelper.ShowCollectedDataAsOneSlice = true;
				collectedPieHelper.SupplementedAreaSizeRatio = 0.9f;
				collectedPieHelper.ShowSmallSegmentsAsSupplementalPie(series);
			}
			catch (Exception ex)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(ex))
				{
					throw;
				}
				Global.Tracer.Trace(TraceLevel.Verbose, ex.Message);
			}
		}

		private SeriesInfo GetShapeSeriesOnSameChartArea(ChartSeries chartSeries)
		{
			foreach (KeyValuePair<string, ChartAreaInfo> item in m_chartAreaInfoDictionary)
			{
				if (!(item.Key == GetSeriesChartAreaName(chartSeries)))
				{
					continue;
				}
				foreach (SeriesInfo seriesInfo in item.Value.SeriesInfoList)
				{
					if (DataPointShowsInLegend(seriesInfo.ChartSeries))
					{
						return seriesInfo;
					}
				}
			}
			return null;
		}

		private SeriesInfo CreateSeries(ChartMember seriesGrouping, ChartSeries chartSeries)
		{
			Series series = new Series();
			SeriesInfo seriesInfo = new SeriesInfo();
			seriesInfo.Series = series;
			seriesInfo.ChartSeries = chartSeries;
			seriesInfo.SeriesGrouping = seriesGrouping;
			series.ChartArea = GetSeriesChartAreaName(chartSeries);
			m_coreChart.Series.Add(series);
			AddSeriesToDictionary(seriesInfo);
			return seriesInfo;
		}

		private void RenderSeries(ChartMember seriesGrouping, ChartSeries chartSeries, Series series, bool isLine)
		{
			SetSeriesProperties(chartSeries, seriesGrouping, series);
			if (!DataPointShowsInLegend(chartSeries) && m_hatcher != null)
			{
				series.BackHatchStyle = m_hatcher.Current;
			}
			if (m_autoMarker != null)
			{
				m_autoMarker.MoveNext();
			}
			if (chartSeries.Style != null)
			{
				RenderSeriesStyle(chartSeries.Style, chartSeries.Instance.Style, series, isLine);
			}
			RenderItemInLegendActionInfo(chartSeries.ActionInfo, series.LegendToolTip, series);
			RenderItemInLegend(chartSeries.ChartItemInLegend, series, DataPointShowsInLegend(chartSeries));
			RenderCustomProperties(chartSeries.CustomProperties, series);
			RenderEmptyPoint(chartSeries.EmptyPoints, series.EmptyPointStyle, isLine);
			RenderSmartLabels(chartSeries.SmartLabel, series.SmartLabels);
			RenderDataLabel(chartSeries.DataLabel, series, isDataPoint: false);
			RenderSeriesMarker(chartSeries.Marker, series);
			if (!m_chartAreaInfoDictionary.ContainsKey(series.ChartArea))
			{
				return;
			}
			ChartAreaInfo chartAreaInfo = m_chartAreaInfoDictionary[series.ChartArea];
			if (chartAreaInfo == null)
			{
				return;
			}
			Microsoft.Reporting.Chart.WebForms.ChartArea chartArea = GetChartArea(series.ChartArea);
			if (chartArea != null)
			{
				Microsoft.Reporting.Chart.WebForms.Axis categoryAxis = GetCategoryAxis(chartArea, series.XAxisType);
				if (categoryAxis != null && IsAxisAutoMargin(chartAreaInfo, categoryAxis) && DoesSeriesRequireMargin(chartSeries))
				{
					categoryAxis.Margin = true;
					chartAreaInfo.CategoryAxesAutoMargin.Remove(categoryAxis);
				}
			}
		}

		private Microsoft.Reporting.Chart.WebForms.Axis GetCategoryAxis(Microsoft.Reporting.Chart.WebForms.ChartArea area, AxisType axisType)
		{
			for (int i = 0; i < area.Axes.Length; i++)
			{
				Microsoft.Reporting.Chart.WebForms.Axis axis = area.Axes[i];
				if (axis.Type != AxisName.Y && axis.Type != AxisName.Y2 && ((axis.Type == AxisName.X && axisType == AxisType.Primary) || (axis.Type == AxisName.X2 && axisType == AxisType.Secondary)))
				{
					return axis;
				}
			}
			return null;
		}

		private string GetSeriesChartAreaName(ChartSeries chartSeries)
		{
			if (chartSeries.ChartAreaName != null)
			{
				if (!chartSeries.ChartAreaName.IsExpression)
				{
					if (chartSeries.ChartAreaName.Value != null)
					{
						return chartSeries.ChartAreaName.Value;
					}
				}
				else if (chartSeries.Instance.ChartAreaName != null)
				{
					return chartSeries.Instance.ChartAreaName;
				}
			}
			return m_defaulChartAreaName;
		}

		private string GetSeriesCategoryAxisName(ChartSeries chartSeries)
		{
			if (chartSeries.CategoryAxisName != null)
			{
				if (!chartSeries.CategoryAxisName.IsExpression)
				{
					if (chartSeries.CategoryAxisName.Value != null)
					{
						return chartSeries.CategoryAxisName.Value;
					}
				}
				else if (chartSeries.Instance.CategoryAxisName != null)
				{
					return chartSeries.Instance.CategoryAxisName;
				}
			}
			return "Primary";
		}

		private void SetSeriesProperties(ChartSeries chartSeries, ChartMember seriesGrouping, Series series)
		{
			SetSeriesType(chartSeries, series);
			if (seriesGrouping == null || !IsDynamicOrHasDynamicParentMember(seriesGrouping))
			{
				series.Name = chartSeries.Name;
			}
			if (chartSeries.LegendName != null)
			{
				if (!chartSeries.LegendName.IsExpression)
				{
					if (chartSeries.LegendName.Value != null)
					{
						series.Legend = chartSeries.LegendName.Value;
					}
				}
				else if (chartSeries.Instance.LegendName != null)
				{
					series.Legend = chartSeries.Instance.LegendName;
				}
			}
			if (chartSeries.LegendText != null)
			{
				if (!chartSeries.LegendText.IsExpression)
				{
					if (chartSeries.LegendText.Value != null)
					{
						series.LegendText = chartSeries.LegendText.Value;
					}
				}
				else if (chartSeries.Instance.LegendText != null)
				{
					series.LegendText = chartSeries.Instance.LegendText;
				}
			}
			if (chartSeries.HideInLegend != null)
			{
				if (!chartSeries.HideInLegend.IsExpression)
				{
					series.ShowInLegend = !chartSeries.HideInLegend.Value;
				}
				else
				{
					series.ShowInLegend = !chartSeries.Instance.HideInLegend;
				}
			}
			if (GetSeriesCategoryAxisName(chartSeries) == "Secondary")
			{
				series.XAxisType = AxisType.Secondary;
			}
			else
			{
				series.XAxisType = AxisType.Primary;
			}
			if (chartSeries.ValueAxisName != null)
			{
				if (!chartSeries.ValueAxisName.IsExpression)
				{
					if (chartSeries.ValueAxisName.Value != null && chartSeries.ValueAxisName.Value == "Secondary")
					{
						series.YAxisType = AxisType.Secondary;
					}
				}
				else if (chartSeries.Instance.ValueAxisName == "Secondary")
				{
					series.YAxisType = AxisType.Secondary;
				}
			}
			ReportStringProperty toolTip = chartSeries.ToolTip;
			if (toolTip != null)
			{
				if (!toolTip.IsExpression)
				{
					if (toolTip.Value != null)
					{
						series.LegendToolTip = toolTip.Value;
					}
				}
				else
				{
					string toolTip2 = chartSeries.Instance.ToolTip;
					if (toolTip2 != null)
					{
						series.LegendToolTip = toolTip2;
					}
				}
			}
			ReportBoolProperty hidden = chartSeries.Hidden;
			if (hidden != null)
			{
				if (!hidden.IsExpression)
				{
					series.Enabled = !hidden.Value;
				}
				else
				{
					series.Enabled = !chartSeries.Instance.Hidden;
				}
			}
			if (seriesGrouping != null && !DataPointShowsInLegend(chartSeries) && series.LegendText == "")
			{
				series.LegendText = GetSeriesLegendText(seriesGrouping);
			}
		}

		private void RenderDataPointStyle(Style style, StyleInstance styleInstance, DataPoint dataPoint, SeriesInfo seriesInfo, int cellIndex)
		{
			RenderDataPointAttributesStyle(style, styleInstance, dataPoint, seriesInfo.IsLine);
			if (seriesInfo.IsGradientPerDataPointSupported)
			{
				RenderDataPointAttributesGradient(style, styleInstance, dataPoint);
			}
			else if (seriesInfo.IsGradientSupported)
			{
				seriesInfo.IsGradientSupported = CheckGradientSupport(style, styleInstance, dataPoint, seriesInfo);
			}
			RenderDataPointBackgroundImage(style.BackgroundImage, dataPoint, seriesInfo.DataPointBackgroundImageInfoCollection[cellIndex].DataPointBackgroundImage);
		}

		private bool CheckGradientSupport(Style style, StyleInstance styleInstance, DataPoint dataPoint, SeriesInfo seriesInfo)
		{
			if (!seriesInfo.BackGradientType.HasValue)
			{
				seriesInfo.BackGradientType = GetGradientType(MappingHelper.GetStyleBackGradientType(style, styleInstance));
				if (seriesInfo.BackGradientType == GradientType.None)
				{
					return false;
				}
			}
			else if (seriesInfo.BackGradientType != GetGradientType(MappingHelper.GetStyleBackGradientType(style, styleInstance)))
			{
				return false;
			}
			if (!seriesInfo.Color.HasValue)
			{
				seriesInfo.Color = dataPoint.Color;
			}
			else if (seriesInfo.Color != dataPoint.Color)
			{
				return false;
			}
			if (!seriesInfo.BackGradientEndColor.HasValue)
			{
				seriesInfo.BackGradientEndColor = MappingHelper.GetStyleBackGradientEndColor(style, styleInstance);
			}
			else if (seriesInfo.BackGradientEndColor != MappingHelper.GetStyleBackGradientEndColor(style, styleInstance))
			{
				return false;
			}
			return true;
		}

		private string GetImageName(BackgroundImage backgroundImage, BackgroundImageInfo backgroundImageInfo)
		{
			string text = "";
			if (backgroundImageInfo == null || !backgroundImageInfo.CanShareBackgroundImage || backgroundImageInfo.SharedBackgroundImageName == null)
			{
				text = CreateImage(backgroundImage);
				if (backgroundImageInfo != null && backgroundImageInfo.CanShareBackgroundImage)
				{
					backgroundImageInfo.SharedBackgroundImageName = text;
				}
			}
			else
			{
				text = backgroundImageInfo.SharedBackgroundImageName;
			}
			return text;
		}

		private void RenderDataPointBackgroundImage(BackgroundImage backgroundImage, DataPoint dataPoint, BackgroundImageInfo backgroundImageInfo)
		{
			dataPoint.BackImage = GetImageName(backgroundImage, backgroundImageInfo);
			GetBackgroundImageProperties(backgroundImage, out ChartImageWrapMode backImageMode, out ChartImageAlign backImageAlign, out Color backImageTransparentColor);
			dataPoint.BackImageMode = backImageMode;
			dataPoint.BackImageAlign = backImageAlign;
			dataPoint.BackImageTransparentColor = backImageTransparentColor;
		}

		private void RenderSeriesStyle(Style style, StyleInstance styleInstance, Series series, bool isSeriesLine)
		{
			RenderDataPointAttributesStyle(style, styleInstance, series, isSeriesLine);
			RenderDataPointAttributesGradient(style, styleInstance, series);
			if (MappingHelper.IsStylePropertyDefined(style.ShadowColor))
			{
				series.ShadowColor = MappingHelper.GetStyleShadowColor(style, styleInstance);
			}
			if (MappingHelper.IsStylePropertyDefined(style.ShadowOffset))
			{
				series.ShadowOffset = MappingHelper.GetStyleShadowOffset(style, styleInstance, base.DpiX);
			}
			RenderDataPointAttributesBackgroundImage(style.BackgroundImage, series);
		}

		private void RenderEmptyPointStyle(Style style, StyleInstance styleInstance, DataPointAttributes dataPointAttributes, bool isSeriesLine)
		{
			RenderDataPointAttributesStyle(style, styleInstance, dataPointAttributes, isSeriesLine);
			RenderDataPointAttributesGradient(style, styleInstance, dataPointAttributes);
			RenderDataPointAttributesBackgroundImage(style.BackgroundImage, dataPointAttributes);
		}

		private void RenderDataPointAttributesGradient(Style style, StyleInstance styleInstance, DataPointAttributes dataPointAttributes)
		{
			if (MappingHelper.IsStylePropertyDefined(style.BackgroundGradientEndColor))
			{
				dataPointAttributes.BackGradientEndColor = MappingHelper.GetStyleBackGradientEndColor(style, styleInstance);
			}
			if (MappingHelper.IsStylePropertyDefined(style.BackgroundGradientType))
			{
				dataPointAttributes.BackGradientType = GetGradientType(MappingHelper.GetStyleBackGradientType(style, styleInstance));
			}
		}

		private void RenderDataPointAttributesStyle(Style style, StyleInstance styleInstance, DataPointAttributes dataPointAttributes, bool isSeriesLine)
		{
			if (MappingHelper.IsStylePropertyDefined(style.Color) && (!style.Color.IsExpression || (styleInstance.Color != null && styleInstance.Color.ToString() != "#00000000")))
			{
				dataPointAttributes.Color = MappingHelper.GetStyleColor(style, styleInstance);
			}
			if (MappingHelper.IsStylePropertyDefined(style.BackgroundHatchType))
			{
				dataPointAttributes.BackHatchStyle = GetHatchType(MappingHelper.GetStyleBackgroundHatchType(style, styleInstance));
			}
			RenderDataPointAttributesBorder(style.Border, dataPointAttributes, isSeriesLine);
		}

		private void RenderDataLabelFont(ChartDataLabel chartDataLabel, DataPointAttributes dataPointAttributes, bool dataPoint)
		{
			Style style = chartDataLabel.Style;
			if (style == null)
			{
				if (dataPoint)
				{
					dataPointAttributes.Font = GetDefaultFontFromCache(m_coreChart.Series.Count);
				}
				else
				{
					dataPointAttributes.Font = GetDefaultFont();
				}
			}
			else if (dataPoint)
			{
				dataPointAttributes.Font = GetFontFromCache(m_coreChart.Series.Count, style, chartDataLabel.Instance.Style);
			}
			else
			{
				dataPointAttributes.Font = GetFont(style, chartDataLabel.Instance.Style);
			}
		}

		private void RenderTitleFont(ChartTitle chartTitle, Title title)
		{
			Style style = chartTitle.Style;
			if (style == null)
			{
				title.Font = GetDefaultFont();
			}
			else
			{
				title.Font = GetFont(style, chartTitle.Instance.Style);
			}
		}

		private void RenderLegendTitleFont(ChartLegendTitle chartLegendTitle, Microsoft.Reporting.Chart.WebForms.Legend legend)
		{
			Style style = chartLegendTitle.Style;
			if (style == null)
			{
				legend.TitleFont = GetDefaultFont();
			}
			else
			{
				legend.TitleFont = GetFont(style, chartLegendTitle.Instance.Style);
			}
		}

		private void RenderLegendFont(ChartLegend chartLegend, Microsoft.Reporting.Chart.WebForms.Legend legend)
		{
			Style style = chartLegend.Style;
			if (style == null)
			{
				legend.Font = GetDefaultFont();
			}
			else
			{
				legend.Font = GetFont(style, chartLegend.Instance.Style);
			}
		}

		private void RenderStripLineFont(ChartStripLine chartStripLine, StripLine stripLine)
		{
			Style style = chartStripLine.Style;
			if (style == null)
			{
				stripLine.TitleFont = GetDefaultFont();
			}
			else
			{
				stripLine.TitleFont = GetFont(style, chartStripLine.Instance.Style);
			}
		}

		private void RenderAxisScaleBreakBorder(Border border, AxisScaleBreakStyle axisScaleBreak)
		{
			if (border != null)
			{
				if (MappingHelper.IsStylePropertyDefined(border.Color))
				{
					axisScaleBreak.LineColor = MappingHelper.GetStyleBorderColor(border);
				}
				if (MappingHelper.IsStylePropertyDefined(border.Style))
				{
					axisScaleBreak.LineStyle = GetBorderStyle(MappingHelper.GetStyleBorderStyle(border), isLine: true);
				}
				axisScaleBreak.LineWidth = MappingHelper.GetStyleBorderWidth(border, base.DpiX);
			}
		}

		private void RenderChartBorder(Border border)
		{
			m_coreChart.BorderColor = Color.Black;
			if (border != null)
			{
				if (MappingHelper.IsStylePropertyDefined(border.Color))
				{
					m_coreChart.BorderColor = MappingHelper.GetStyleBorderColor(border);
				}
				if (MappingHelper.IsStylePropertyDefined(border.Style))
				{
					m_coreChart.BorderStyle = GetBorderStyle(MappingHelper.GetStyleBorderStyle(border), isLine: false);
				}
				m_coreChart.BorderWidth = MappingHelper.GetStyleBorderWidth(border, base.DpiX);
			}
		}

		private void RenderDataPointAttributesBorder(Border border, DataPointAttributes dataPointAttributes, bool isLine)
		{
			if (border != null)
			{
				if (MappingHelper.IsStylePropertyDefined(border.Color))
				{
					dataPointAttributes.BorderColor = MappingHelper.GetStyleBorderColor(border);
				}
				if (MappingHelper.IsStylePropertyDefined(border.Style))
				{
					dataPointAttributes.BorderStyle = GetBorderStyle(MappingHelper.GetStyleBorderStyle(border), isLine);
				}
				dataPointAttributes.BorderWidth = MappingHelper.GetStyleBorderWidth(border, base.DpiX);
			}
		}

		private void RenderTitleBorder(Border border, Title title)
		{
			title.BorderColor = Color.Black;
			title.BorderStyle = ChartDashStyle.NotSet;
			if (border != null)
			{
				if (MappingHelper.IsStylePropertyDefined(border.Color))
				{
					title.BorderColor = MappingHelper.GetStyleBorderColor(border);
				}
				if (MappingHelper.IsStylePropertyDefined(border.Style))
				{
					title.BorderStyle = GetBorderStyle(MappingHelper.GetStyleBorderStyle(border), isLine: false);
				}
				title.BorderWidth = MappingHelper.GetStyleBorderWidth(border, base.DpiX);
			}
		}

		private void RenderChartAreaBorder(Border border, Microsoft.Reporting.Chart.WebForms.ChartArea area)
		{
			if (border != null)
			{
				if (MappingHelper.IsStylePropertyDefined(border.Color))
				{
					area.BorderColor = MappingHelper.GetStyleBorderColor(border);
				}
				if (MappingHelper.IsStylePropertyDefined(border.Style))
				{
					area.BorderStyle = GetBorderStyle(MappingHelper.GetStyleBorderStyle(border), isLine: false);
				}
				area.BorderWidth = MappingHelper.GetStyleBorderWidth(border, base.DpiX);
			}
		}

		private void RenderStripLineBorder(Border border, StripLine stripLine)
		{
			stripLine.BorderStyle = ChartDashStyle.NotSet;
			if (border != null)
			{
				if (MappingHelper.IsStylePropertyDefined(border.Color))
				{
					stripLine.BorderColor = MappingHelper.GetStyleBorderColor(border);
				}
				if (MappingHelper.IsStylePropertyDefined(border.Style))
				{
					stripLine.BorderStyle = GetBorderStyle(MappingHelper.GetStyleBorderStyle(border), isLine: false);
				}
				stripLine.BorderWidth = MappingHelper.GetStyleBorderWidth(border, base.DpiX);
			}
		}

		private void RenderBorderSkinBorder(Border border, BorderSkinAttributes borderSkin)
		{
			if (border != null)
			{
				if (MappingHelper.IsStylePropertyDefined(border.Color))
				{
					borderSkin.FrameBorderColor = MappingHelper.GetStyleBorderColor(border);
				}
				if (MappingHelper.IsStylePropertyDefined(border.Style))
				{
					borderSkin.FrameBorderStyle = GetBorderStyle(MappingHelper.GetStyleBorderStyle(border), isLine: false);
				}
				borderSkin.FrameBorderWidth = MappingHelper.GetStyleBorderWidth(border, base.DpiX);
			}
		}

		private void RenderLegendBorder(Border border, Microsoft.Reporting.Chart.WebForms.Legend legend)
		{
			legend.BorderStyle = ChartDashStyle.NotSet;
			if (border != null)
			{
				if (MappingHelper.IsStylePropertyDefined(border.Color))
				{
					legend.BorderColor = MappingHelper.GetStyleBorderColor(border);
				}
				if (MappingHelper.IsStylePropertyDefined(border.Style))
				{
					legend.BorderStyle = GetBorderStyle(MappingHelper.GetStyleBorderStyle(border), isLine: false);
				}
				legend.BorderWidth = MappingHelper.GetStyleBorderWidth(border, base.DpiX);
			}
		}

		private void RenderChartBackgroundImage(BackgroundImage backgroundImage)
		{
			RenderBackgroundImage(backgroundImage, out string backImage, out ChartImageWrapMode backImageMode, out ChartImageAlign backImageAlign, out Color backImageTransparentColor);
			m_coreChart.BackImage = backImage;
			m_coreChart.BackImageMode = backImageMode;
			m_coreChart.BackImageAlign = backImageAlign;
			m_coreChart.BackImageTransparentColor = backImageTransparentColor;
		}

		private void RenderBorderSkinBackgroundImage(BackgroundImage backgroundImage, BorderSkinAttributes borderSkin)
		{
			RenderBackgroundImage(backgroundImage, out string backImage, out ChartImageWrapMode backImageMode, out ChartImageAlign backImageAlign, out Color backImageTransparentColor);
			borderSkin.FrameBackImage = backImage;
			borderSkin.FrameBackImageMode = backImageMode;
			borderSkin.FrameBackImageAlign = backImageAlign;
			borderSkin.FrameBackImageTransparentColor = backImageTransparentColor;
		}

		private void RenderDataPointAttributesBackgroundImage(BackgroundImage backgroundImage, DataPointAttributes dataPointAttributes)
		{
			RenderBackgroundImage(backgroundImage, out string backImage, out ChartImageWrapMode backImageMode, out ChartImageAlign backImageAlign, out Color backImageTransparentColor);
			dataPointAttributes.BackImage = backImage;
			dataPointAttributes.BackImageMode = backImageMode;
			dataPointAttributes.BackImageAlign = backImageAlign;
			dataPointAttributes.BackImageTransparentColor = backImageTransparentColor;
		}

		private void RenderTitleBackgroundImage(BackgroundImage backgroundImage, Title title)
		{
			RenderBackgroundImage(backgroundImage, out string backImage, out ChartImageWrapMode backImageMode, out ChartImageAlign backImageAlign, out Color backImageTransparentColor);
			title.BackImage = backImage;
			title.BackImageMode = backImageMode;
			title.BackImageAlign = backImageAlign;
			title.BackImageTransparentColor = backImageTransparentColor;
		}

		private void RenderChartAreaBackgroundImage(BackgroundImage backgroundImage, Microsoft.Reporting.Chart.WebForms.ChartArea chartArea)
		{
			RenderBackgroundImage(backgroundImage, out string backImage, out ChartImageWrapMode backImageMode, out ChartImageAlign backImageAlign, out Color backImageTransparentColor);
			chartArea.BackImage = backImage;
			chartArea.BackImageMode = backImageMode;
			chartArea.BackImageAlign = backImageAlign;
			chartArea.BackImageTransparentColor = backImageTransparentColor;
		}

		private void RenderLegendBackgroundImage(BackgroundImage backgroundImage, Microsoft.Reporting.Chart.WebForms.Legend legend)
		{
			RenderBackgroundImage(backgroundImage, out string backImage, out ChartImageWrapMode backImageMode, out ChartImageAlign backImageAlign, out Color backImageTransparentColor);
			legend.BackImage = backImage;
			legend.BackImageMode = backImageMode;
			legend.BackImageAlign = backImageAlign;
			legend.BackImageTransparentColor = backImageTransparentColor;
		}

		private void RenderBackgroundImage(BackgroundImage backgroundImage, out string backImage, out ChartImageWrapMode backImageMode, out ChartImageAlign backImageAlign, out Color backImageTransparentColor)
		{
			GetBackgroundImageProperties(backgroundImage, out backImageMode, out backImageAlign, out backImageTransparentColor);
			backImage = CreateImage(backgroundImage);
		}

		private void GetBackgroundImageProperties(BackgroundImage backgroundImage, out ChartImageWrapMode backImageMode, out ChartImageAlign backImageAlign, out Color backImageTransparentColor)
		{
			backImageMode = ChartImageWrapMode.Scaled;
			backImageAlign = ChartImageAlign.Center;
			backImageTransparentColor = Color.Empty;
			if (backgroundImage == null)
			{
				return;
			}
			if (MappingHelper.IsStylePropertyDefined(backgroundImage.BackgroundRepeat))
			{
				if (!backgroundImage.BackgroundRepeat.IsExpression)
				{
					backImageMode = GetBackImageMode(backgroundImage.BackgroundRepeat.Value);
				}
				else
				{
					backImageMode = GetBackImageMode(backgroundImage.Instance.BackgroundRepeat);
				}
			}
			if (MappingHelper.IsStylePropertyDefined(backgroundImage.Position))
			{
				if (!backgroundImage.Position.IsExpression)
				{
					backImageAlign = GetBackImageAlign(backgroundImage.Position.Value);
				}
				else
				{
					backImageAlign = GetBackImageAlign(backgroundImage.Instance.Position);
				}
			}
			if (MappingHelper.IsStylePropertyDefined(backgroundImage.TransparentColor))
			{
				Color color = Color.Empty;
				if (MappingHelper.GetColorFromReportColorProperty(backgroundImage.TransparentColor, ref color))
				{
					backImageTransparentColor = color;
				}
				else if (backgroundImage.Instance.TransparentColor != null)
				{
					backImageTransparentColor = backgroundImage.Instance.TransparentColor.ToColor();
				}
			}
		}

		private string CreateImage(BackgroundImage backgroundImage)
		{
			string text = "";
			if (backgroundImage != null)
			{
				System.Drawing.Image imageFromStream = GetImageFromStream(backgroundImage);
				if (imageFromStream != null)
				{
					text = m_imagePrefix + m_coreChart.Images.Count;
					m_coreChart.Images.Add(text, imageFromStream);
				}
			}
			return text;
		}

		private void RenderStripLineBackgroundImage(BackgroundImage backgroundImage, StripLine stripLine)
		{
			RenderBackgroundImage(backgroundImage, out string backImage, out ChartImageWrapMode backImageMode, out ChartImageAlign backImageAlign, out Color backImageTransparentColor);
			stripLine.BackImage = backImage;
			stripLine.BackImageMode = backImageMode;
			stripLine.BackImageAlign = backImageAlign;
			stripLine.BackImageTransparentColor = backImageTransparentColor;
		}

		private System.Drawing.Image GetImageFromStream(BackgroundImage backgroundImage)
		{
			if (backgroundImage.Instance.ImageData == null)
			{
				return null;
			}
			return System.Drawing.Image.FromStream(new MemoryStream(backgroundImage.Instance.ImageData, writable: false));
		}

		private void RenderActionInfo(ActionInfo actionInfo, string toolTip, IMapAreaAttributes mapAreaAttributes)
		{
			if (actionInfo == null && string.IsNullOrEmpty(toolTip))
			{
				return;
			}
			string href;
			ActionInfoWithDynamicImageMap actionInfoWithDynamicImageMap = MappingHelper.CreateActionInfoDynamic(m_chart, actionInfo, toolTip, out href);
			if (actionInfoWithDynamicImageMap != null)
			{
				if (href != null)
				{
					mapAreaAttributes.Href = href;
				}
				int count = m_actions.Count;
				m_actions.InternalList.Add(actionInfoWithDynamicImageMap);
				mapAreaAttributes.Tag = count;
			}
		}

		private void RenderEmptyPoint(ChartEmptyPoints chartEmptyPoint, DataPointAttributes emptyPoint, bool isSeriesLine)
		{
			if (chartEmptyPoint != null)
			{
				SetEmptyPointProperties(chartEmptyPoint, emptyPoint);
				if (chartEmptyPoint.Style != null)
				{
					RenderEmptyPointStyle(chartEmptyPoint.Style, chartEmptyPoint.Instance.Style, emptyPoint, isSeriesLine);
				}
				RenderActionInfo(chartEmptyPoint.ActionInfo, emptyPoint.ToolTip, emptyPoint);
				RenderEmptyPointMarker(chartEmptyPoint.Marker, emptyPoint);
				RenderDataLabel(chartEmptyPoint.DataLabel, emptyPoint, isDataPoint: true);
				RenderCustomProperties(chartEmptyPoint.CustomProperties, emptyPoint);
			}
		}

		private void SetEmptyPointProperties(ChartEmptyPoints chartEmptyPoint, DataPointAttributes emptyPoint)
		{
			if (chartEmptyPoint.AxisLabel != null)
			{
				object obj = null;
				obj = (chartEmptyPoint.AxisLabel.IsExpression ? chartEmptyPoint.Instance.AxisLabel : chartEmptyPoint.AxisLabel.Value);
				if (obj != null)
				{
					emptyPoint.AxisLabel = GetFormattedValue(obj, "");
				}
			}
			ReportStringProperty toolTip = chartEmptyPoint.ToolTip;
			if (toolTip == null)
			{
				return;
			}
			if (!toolTip.IsExpression)
			{
				if (toolTip.Value != null)
				{
					emptyPoint.ToolTip = toolTip.Value;
				}
				return;
			}
			string toolTip2 = chartEmptyPoint.Instance.ToolTip;
			if (toolTip2 != null)
			{
				emptyPoint.ToolTip = toolTip2;
			}
		}

		private void RenderItemInLegend(ChartItemInLegend chartItemInLegend, DataPointAttributes dataPointAttributes, bool dataPointShowsInLegend)
		{
			if (dataPointShowsInLegend || chartItemInLegend == null)
			{
				return;
			}
			if (chartItemInLegend.LegendText != null)
			{
				if (!chartItemInLegend.LegendText.IsExpression)
				{
					if (chartItemInLegend.LegendText.Value != null)
					{
						dataPointAttributes.LegendText = chartItemInLegend.LegendText.Value;
					}
				}
				else if (chartItemInLegend.Instance.LegendText != null)
				{
					dataPointAttributes.LegendText = chartItemInLegend.Instance.LegendText;
				}
			}
			ReportStringProperty toolTip = chartItemInLegend.ToolTip;
			if (toolTip != null)
			{
				if (!toolTip.IsExpression)
				{
					if (toolTip.Value != null)
					{
						dataPointAttributes.LegendToolTip = toolTip.Value;
					}
				}
				else
				{
					string toolTip2 = chartItemInLegend.Instance.ToolTip;
					if (toolTip2 != null)
					{
						dataPointAttributes.LegendToolTip = toolTip2;
					}
				}
			}
			ReportBoolProperty hidden = chartItemInLegend.Hidden;
			if (hidden != null)
			{
				if (!hidden.IsExpression)
				{
					dataPointAttributes.ShowInLegend = !hidden.Value;
				}
				else
				{
					dataPointAttributes.ShowInLegend = !chartItemInLegend.Instance.Hidden;
				}
			}
			RenderItemInLegendActionInfo(chartItemInLegend.ActionInfo, dataPointAttributes.LegendToolTip, dataPointAttributes);
		}

		private void RenderItemInLegendActionInfo(ActionInfo actionInfo, string toolTip, DataPointAttributes dataPointAttributes)
		{
			if (actionInfo == null && string.IsNullOrEmpty(toolTip))
			{
				return;
			}
			string href;
			ActionInfoWithDynamicImageMap actionInfoWithDynamicImageMap = MappingHelper.CreateActionInfoDynamic(m_chart, actionInfo, toolTip, out href);
			if (actionInfoWithDynamicImageMap != null)
			{
				if (href != null)
				{
					dataPointAttributes.LegendHref = href;
				}
				int count = m_actions.Count;
				m_actions.InternalList.Add(actionInfoWithDynamicImageMap);
				dataPointAttributes.LegendTag = count;
			}
		}

		private bool GetCustomProperty(CustomProperty customProperty, ref string name, ref string value)
		{
			if (customProperty.Name == null)
			{
				return false;
			}
			if (customProperty.Value == null)
			{
				return false;
			}
			if (!customProperty.Name.IsExpression)
			{
				if (customProperty.Name.Value == null)
				{
					return false;
				}
				name = customProperty.Name.Value;
			}
			else
			{
				if (customProperty.Instance.Name == null)
				{
					return false;
				}
				name = customProperty.Instance.Name;
			}
			if (!customProperty.Value.IsExpression)
			{
				if (customProperty.Value.Value == null)
				{
					return false;
				}
				value = Convert.ToString(customProperty.Value.Value, CultureInfo.InvariantCulture);
			}
			else
			{
				if (customProperty.Instance.Value == null)
				{
					return false;
				}
				value = Convert.ToString(customProperty.Instance.Value, CultureInfo.InvariantCulture);
			}
			return true;
		}

		private void RenderCustomProperties(CustomPropertyCollection customProperties, DataPointAttributes dataPointAttributes)
		{
			if (customProperties == null)
			{
				return;
			}
			string name = null;
			string value = null;
			foreach (CustomProperty customProperty in customProperties)
			{
				if (GetCustomProperty(customProperty, ref name, ref value))
				{
					dataPointAttributes.SetAttribute(name, value);
				}
			}
		}

		private void SetSeriesType(ChartSeries chartSeries, Series series)
		{
			ChartSeriesType chartSeriesType = ChartSeriesType.Column;
			ChartSeriesSubtype chartSeriesSubtype = ChartSeriesSubtype.Plain;
			chartSeriesType = GetSeriesType(chartSeries);
			chartSeriesSubtype = GetValidSeriesSubType(chartSeriesType, GetSeriesSubType(chartSeries));
			switch (chartSeriesType)
			{
			case ChartSeriesType.Area:
				SetSeriesTypeArea(series, chartSeriesSubtype);
				break;
			case ChartSeriesType.Bar:
				SetSeriesTypeBar(series, chartSeriesSubtype);
				break;
			case ChartSeriesType.Column:
				SetSeriesTypeColumn(series, chartSeriesSubtype);
				break;
			case ChartSeriesType.Line:
				SetSeriesTypeLine(series, chartSeriesSubtype);
				break;
			case ChartSeriesType.Polar:
				SetSeriesTypePolar(series, chartSeriesSubtype);
				break;
			case ChartSeriesType.Range:
				SetSeriesTypeRange(series, chartSeriesSubtype);
				break;
			case ChartSeriesType.Scatter:
				SetSeriesTypeScatter(series, chartSeriesSubtype);
				break;
			case ChartSeriesType.Shape:
				SetSeriesTypeShape(series, chartSeriesSubtype);
				break;
			}
		}

		private ChartSeriesType GetSeriesType(ChartSeries chartSeries)
		{
			if (chartSeries.Type != null)
			{
				if (!chartSeries.Type.IsExpression)
				{
					return chartSeries.Type.Value;
				}
				if (chartSeries.Instance != null)
				{
					return chartSeries.Instance.Type;
				}
			}
			return ChartSeriesType.Column;
		}

		private ChartSeriesSubtype GetSeriesSubType(ChartSeries chartSeries)
		{
			if (chartSeries.Subtype != null)
			{
				if (!chartSeries.Subtype.IsExpression)
				{
					return chartSeries.Subtype.Value;
				}
				if (chartSeries.Instance != null)
				{
					return chartSeries.Instance.Subtype;
				}
			}
			return ChartSeriesSubtype.Plain;
		}

		private ChartSeriesSubtype GetValidSeriesSubType(ChartSeriesType type, ChartSeriesSubtype subtype)
		{
			switch (type)
			{
			case ChartSeriesType.Area:
				if (subtype == ChartSeriesSubtype.Smooth || subtype == ChartSeriesSubtype.Stacked || subtype == ChartSeriesSubtype.PercentStacked)
				{
					return subtype;
				}
				break;
			case ChartSeriesType.Column:
			case ChartSeriesType.Bar:
				if (subtype == ChartSeriesSubtype.Stacked || subtype == ChartSeriesSubtype.PercentStacked)
				{
					return subtype;
				}
				break;
			case ChartSeriesType.Line:
				if (subtype == ChartSeriesSubtype.Smooth || subtype == ChartSeriesSubtype.Stepped)
				{
					return subtype;
				}
				break;
			case ChartSeriesType.Polar:
				if (subtype == ChartSeriesSubtype.Radar)
				{
					return subtype;
				}
				break;
			case ChartSeriesType.Range:
				if (subtype == ChartSeriesSubtype.Smooth || (uint)(subtype - 12) <= 5u)
				{
					return subtype;
				}
				break;
			case ChartSeriesType.Scatter:
				if (subtype == ChartSeriesSubtype.Bubble)
				{
					return subtype;
				}
				break;
			case ChartSeriesType.Shape:
				if ((uint)(subtype - 6) <= 4u || (uint)(subtype - 19) <= 1u)
				{
					return subtype;
				}
				return ChartSeriesSubtype.Pie;
			}
			return ChartSeriesSubtype.Plain;
		}

		private void SetSeriesTypeArea(Series series, ChartSeriesSubtype subtype)
		{
			switch (subtype)
			{
			case ChartSeriesSubtype.Smooth:
				series.ChartType = SeriesChartType.SplineArea;
				break;
			case ChartSeriesSubtype.Stacked:
				series.ChartType = SeriesChartType.StackedArea;
				break;
			case ChartSeriesSubtype.PercentStacked:
				series.ChartType = SeriesChartType.StackedArea100;
				break;
			default:
				series.ChartType = SeriesChartType.Area;
				break;
			}
		}

		private void SetSeriesTypeBar(Series series, ChartSeriesSubtype subtype)
		{
			switch (subtype)
			{
			case ChartSeriesSubtype.Stacked:
				series.ChartType = SeriesChartType.StackedBar;
				break;
			case ChartSeriesSubtype.PercentStacked:
				series.ChartType = SeriesChartType.StackedBar100;
				break;
			default:
				series.ChartType = SeriesChartType.Bar;
				break;
			}
		}

		private void SetSeriesTypeColumn(Series series, ChartSeriesSubtype subtype)
		{
			switch (subtype)
			{
			case ChartSeriesSubtype.Stacked:
				series.ChartType = SeriesChartType.StackedColumn;
				break;
			case ChartSeriesSubtype.PercentStacked:
				series.ChartType = SeriesChartType.StackedColumn100;
				break;
			default:
				series.ChartType = SeriesChartType.Column;
				break;
			}
		}

		private void SetSeriesTypeLine(Series series, ChartSeriesSubtype subtype)
		{
			switch (subtype)
			{
			case ChartSeriesSubtype.Smooth:
				series.ChartType = SeriesChartType.Spline;
				break;
			case ChartSeriesSubtype.Stepped:
				series.ChartType = SeriesChartType.StepLine;
				break;
			default:
				series.ChartType = SeriesChartType.Line;
				break;
			}
		}

		private void SetSeriesTypePolar(Series series, ChartSeriesSubtype subtype)
		{
			if (subtype == ChartSeriesSubtype.Radar)
			{
				series.ChartType = SeriesChartType.Radar;
			}
			else
			{
				series.ChartType = SeriesChartType.Polar;
			}
		}

		private void SetSeriesTypeRange(Series series, ChartSeriesSubtype subtype)
		{
			switch (subtype)
			{
			case ChartSeriesSubtype.Candlestick:
				series.ChartType = SeriesChartType.Candlestick;
				break;
			case ChartSeriesSubtype.Stock:
				series.ChartType = SeriesChartType.Stock;
				break;
			case ChartSeriesSubtype.Smooth:
				series.ChartType = SeriesChartType.SplineRange;
				break;
			case ChartSeriesSubtype.Column:
				series.ChartType = SeriesChartType.RangeColumn;
				break;
			case ChartSeriesSubtype.Bar:
				series.ChartType = SeriesChartType.Gantt;
				break;
			case ChartSeriesSubtype.BoxPlot:
				series.ChartType = SeriesChartType.BoxPlot;
				break;
			case ChartSeriesSubtype.ErrorBar:
				series.ChartType = SeriesChartType.ErrorBar;
				break;
			default:
				series.ChartType = SeriesChartType.Range;
				break;
			}
		}

		private void SetSeriesTypeScatter(Series series, ChartSeriesSubtype subtype)
		{
			if (subtype == ChartSeriesSubtype.Bubble)
			{
				series.ChartType = SeriesChartType.Bubble;
			}
			else
			{
				series.ChartType = SeriesChartType.Point;
			}
		}

		private void SetSeriesTypeShape(Series series, ChartSeriesSubtype subtype)
		{
			switch (subtype)
			{
			case ChartSeriesSubtype.Doughnut:
			case ChartSeriesSubtype.ExplodedDoughnut:
				series.ChartType = SeriesChartType.Doughnut;
				break;
			case ChartSeriesSubtype.Funnel:
				series.ChartType = SeriesChartType.Funnel;
				break;
			case ChartSeriesSubtype.Pyramid:
				series.ChartType = SeriesChartType.Pyramid;
				break;
			case ChartSeriesSubtype.TreeMap:
				series.ChartType = SeriesChartType.TreeMap;
				break;
			case ChartSeriesSubtype.Sunburst:
				series.ChartType = SeriesChartType.Sunburst;
				break;
			default:
				series.ChartType = SeriesChartType.Pie;
				break;
			}
		}

		private void RenderDataPoint(ChartSeries chartSeries, ChartMember seriesGrouping, ChartMember categoryGrouping, SeriesInfo seriesInfo, bool dataPointShowsInLegend)
		{
			ChartDataPoint chartDataPoint = chartSeries[categoryGrouping.MemberCellIndex];
			if (chartDataPoint == null)
			{
				return;
			}
			DataPoint dataPoint = new DataPoint();
			int yValuesCount = GetYValuesCount(seriesInfo.Series.ChartType);
			if (yValuesCount != 1)
			{
				dataPoint.YValues = new double[yValuesCount];
			}
			RenderDataPointValues(chartDataPoint, dataPoint, seriesInfo, categoryGrouping);
			Style style = chartDataPoint.Style;
			if (!dataPoint.Empty)
			{
				SetDataPointProperties(chartDataPoint, dataPoint);
				if (dataPointShowsInLegend && m_hatcher != null)
				{
					dataPoint.BackHatchStyle = m_hatcher.Current;
				}
				if (style != null)
				{
					RenderDataPointStyle(style, chartDataPoint.Instance.Style, dataPoint, seriesInfo, categoryGrouping.MemberCellIndex);
					if (!seriesInfo.IsDataPointHatchDefined)
					{
						seriesInfo.IsDataPointHatchDefined = MappingHelper.IsStylePropertyDefined(style.BackgroundHatchType);
					}
				}
				if (!seriesInfo.IsDataPointColorEmpty.HasValue || !seriesInfo.IsDataPointColorEmpty.Value)
				{
					seriesInfo.IsDataPointColorEmpty = (dataPoint.Color == Color.Empty);
				}
				RenderActionInfo(chartDataPoint.ActionInfo, dataPoint.ToolTip, dataPoint);
				RenderDataLabel(chartDataPoint.DataLabel, dataPoint, isDataPoint: true);
				RenderDataPointMarker(chartDataPoint.Marker, dataPoint, seriesInfo.DataPointBackgroundImageInfoCollection[categoryGrouping.MemberCellIndex].MarkerBackgroundImage);
				RenderItemInLegend(chartDataPoint.ItemInLegend, dataPoint, dataPointShowsInLegend: false);
				RenderCustomProperties(chartDataPoint.CustomProperties, dataPoint);
			}
			else if (!dataPointShowsInLegend && seriesInfo.DefaultDataPointAppearance == null)
			{
				seriesInfo.DefaultDataPointAppearance = new DataPoint();
				if (style != null)
				{
					RenderDefaultDataPointStyle(style, chartDataPoint.Instance.Style, seriesInfo.DefaultDataPointAppearance, seriesInfo, categoryGrouping.MemberCellIndex);
				}
				RenderDataPointMarker(chartDataPoint.Marker, seriesInfo.DefaultDataPointAppearance, seriesInfo.DataPointBackgroundImageInfoCollection[categoryGrouping.MemberCellIndex].MarkerBackgroundImage);
				RenderItemInLegend(chartDataPoint.ItemInLegend, seriesInfo.DefaultDataPointAppearance, dataPointShowsInLegend: false);
			}
			seriesInfo.Series.Points.Add(dataPoint);
			if (dataPointShowsInLegend)
			{
				if (seriesInfo.IsExploded)
				{
					dataPoint.SetAttribute("Exploded", "true");
				}
				if (CanSetPieDataPointLegendText(seriesInfo.Series, dataPoint))
				{
					dataPoint.LegendText = GetDataPointLegendText(categoryGrouping, seriesGrouping);
				}
			}
			OnPostApplySeriesPointData(seriesInfo.Series, seriesInfo.Series.Points.Count - 1);
		}

		private void RenderDefaultDataPointStyle(Style style, StyleInstance styleInstance, DataPoint dataPoint, SeriesInfo seriesInfo, int cellIndex)
		{
			RenderDataPointAttributesStyle(style, styleInstance, dataPoint, seriesInfo.IsLine);
			RenderDataPointAttributesGradient(style, styleInstance, dataPoint);
			RenderDataPointBackgroundImage(style.BackgroundImage, dataPoint, seriesInfo.DataPointBackgroundImageInfoCollection[cellIndex].DataPointBackgroundImage);
		}

		public static int GetYValuesCount(SeriesChartType seriesType)
		{
			switch (seriesType)
			{
			default:
				return 1;
			case SeriesChartType.Bubble:
			case SeriesChartType.Range:
			case SeriesChartType.SplineRange:
			case SeriesChartType.Gantt:
			case SeriesChartType.RangeColumn:
			case SeriesChartType.PointAndFigure:
				return 2;
			case SeriesChartType.ErrorBar:
				return 3;
			case SeriesChartType.Stock:
			case SeriesChartType.Candlestick:
				return 4;
			case SeriesChartType.BoxPlot:
				return 6;
			}
		}

		private void SetDataPointProperties(ChartDataPoint chartDataPoint, DataPoint dataPoint)
		{
			if (chartDataPoint.AxisLabel != null)
			{
				object obj = null;
				obj = (chartDataPoint.AxisLabel.IsExpression ? chartDataPoint.Instance.AxisLabel : chartDataPoint.AxisLabel.Value);
				if (obj != null)
				{
					dataPoint.AxisLabel = GetFormattedValue(obj, "");
				}
			}
			ReportStringProperty toolTip = chartDataPoint.ToolTip;
			if (toolTip == null)
			{
				return;
			}
			if (!toolTip.IsExpression)
			{
				if (toolTip.Value != null)
				{
					dataPoint.ToolTip = toolTip.Value;
				}
				return;
			}
			string toolTip2 = chartDataPoint.Instance.ToolTip;
			if (toolTip2 != null)
			{
				dataPoint.ToolTip = toolTip2;
			}
		}

		private void RenderDataPointValues(ChartDataPoint chartDataPoint, DataPoint dataPoint, SeriesInfo seriesInfo, ChartMember categoryGrouping)
		{
			if (chartDataPoint.DataPointValues != null)
			{
				SetDataPointXValue(chartDataPoint, dataPoint, seriesInfo, categoryGrouping);
				SetDataPointYValues(chartDataPoint, dataPoint, seriesInfo);
			}
		}

		private void SetDataPointYValue(DataPoint dataPoint, int index, object value, ref ChartValueTypes? dateTimeType)
		{
			if (index < dataPoint.YValues.Length)
			{
				dataPoint.YValues[index] = ConvertToDouble(value, ref dateTimeType);
			}
		}

		private void SetDataPointYValues(ChartDataPoint chartDataPoint, DataPoint dataPoint, SeriesInfo seriesInfo)
		{
			ChartValueTypes? dateTimeType = null;
			if (chartDataPoint.DataPointValues.Y != null)
			{
				SetDataPointYValue(dataPoint, 0, chartDataPoint.DataPointValues.Instance.Y, ref dateTimeType);
			}
			if (seriesInfo.IsBubble && chartDataPoint.DataPointValues.Size != null)
			{
				SetDataPointYValue(dataPoint, 1, chartDataPoint.DataPointValues.Instance.Size, ref dateTimeType);
			}
			if (seriesInfo.IsRange)
			{
				if (chartDataPoint.DataPointValues.High != null)
				{
					SetDataPointYValue(dataPoint, GetHighYValueIndex(seriesInfo), chartDataPoint.DataPointValues.Instance.High, ref dateTimeType);
				}
				if (chartDataPoint.DataPointValues.Low != null)
				{
					SetDataPointYValue(dataPoint, GetLowYValueIndex(seriesInfo), chartDataPoint.DataPointValues.Instance.Low, ref dateTimeType);
				}
				if (chartDataPoint.DataPointValues.Start != null)
				{
					SetDataPointYValue(dataPoint, GetStartYValueIndex(seriesInfo), chartDataPoint.DataPointValues.Instance.Start, ref dateTimeType);
				}
				if (chartDataPoint.DataPointValues.End != null)
				{
					SetDataPointYValue(dataPoint, GetEndYValueIndex(seriesInfo), chartDataPoint.DataPointValues.Instance.End, ref dateTimeType);
				}
				if (chartDataPoint.DataPointValues.Mean != null)
				{
					SetDataPointYValue(dataPoint, 4, chartDataPoint.DataPointValues.Instance.Mean, ref dateTimeType);
				}
				if (chartDataPoint.DataPointValues.Median != null)
				{
					SetDataPointYValue(dataPoint, 5, chartDataPoint.DataPointValues.Instance.Median, ref dateTimeType);
				}
			}
			if (dateTimeType.HasValue)
			{
				seriesInfo.Series.YValueType = dateTimeType.Value;
			}
			int num = 0;
			while (true)
			{
				if (num < dataPoint.YValues.Length)
				{
					if (double.IsNaN(dataPoint.YValues[num]))
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			dataPoint.Empty = true;
			dataPoint.ShowLabelAsValue = false;
		}

		private int GetStartYValueIndex(SeriesInfo seriesInfo)
		{
			SeriesChartType chartType = seriesInfo.Series.ChartType;
			if (chartType == SeriesChartType.Gantt)
			{
				return 0;
			}
			return 2;
		}

		private int GetEndYValueIndex(SeriesInfo seriesInfo)
		{
			SeriesChartType chartType = seriesInfo.Series.ChartType;
			if (chartType == SeriesChartType.Gantt)
			{
				return 1;
			}
			return 3;
		}

		private int GetHighYValueIndex(SeriesInfo seriesInfo)
		{
			switch (seriesInfo.Series.ChartType)
			{
			case SeriesChartType.BoxPlot:
				return 1;
			case SeriesChartType.ErrorBar:
				return 2;
			default:
				return 0;
			}
		}

		private int GetLowYValueIndex(SeriesInfo seriesInfo)
		{
			SeriesChartType chartType = seriesInfo.Series.ChartType;
			if (chartType == SeriesChartType.BoxPlot)
			{
				return 0;
			}
			return 1;
		}

		private bool IsSeriesAttachedToScalarAxis(SeriesInfo seriesInfo)
		{
			if (seriesInfo.ChartAreaInfo.CategoryAxesScalar == null)
			{
				return false;
			}
			return seriesInfo.ChartAreaInfo.CategoryAxesScalar.Contains(GetSeriesCategoryAxisName(seriesInfo.ChartSeries));
		}

		private bool IsAxisAutoMargin(ChartAreaInfo chartAreaInfo, Microsoft.Reporting.Chart.WebForms.Axis axis)
		{
			if (chartAreaInfo.CategoryAxesAutoMargin == null)
			{
				return false;
			}
			return chartAreaInfo.CategoryAxesAutoMargin.Contains(axis);
		}

		private void SetDataPointXValue(ChartDataPoint chartDataPoint, DataPoint dataPoint, SeriesInfo seriesInfo, ChartMember categoryGrouping)
		{
			if (chartDataPoint.DataPointValues.X == null && !seriesInfo.IsAttachedToScalarAxis)
			{
				return;
			}
			object obj = null;
			ChartValueTypes? dateTimeType = null;
			if (chartDataPoint.DataPointValues.X != null)
			{
				obj = chartDataPoint.DataPointValues.Instance.X;
			}
			else if (categoryGrouping.Group != null && categoryGrouping.Group.Instance.GroupExpressions != null && categoryGrouping.Group.Instance.GroupExpressions.Count > 0)
			{
				obj = categoryGrouping.Group.Instance.GroupExpressions[0];
			}
			if (obj == null)
			{
				seriesInfo.NullXValuePoints.Add(dataPoint);
			}
			else
			{
				double num = ConvertToDouble(obj, ref dateTimeType);
				if (!double.IsNaN(num))
				{
					dataPoint.XValue = num;
					seriesInfo.XValueSet = true;
				}
				else
				{
					seriesInfo.XValueSetFailed = true;
					if (DataPointShowsInLegend(seriesInfo.ChartSeries))
					{
						if (CanSetPieDataPointLegendText(seriesInfo.Series, dataPoint))
						{
							dataPoint.LegendText = GetFormattedValue(obj, "");
						}
					}
					else if (CanSetDataPointAxisLabel(seriesInfo.Series, dataPoint))
					{
						Microsoft.Reporting.Chart.WebForms.ChartArea chartArea = null;
						try
						{
							chartArea = m_coreChart.ChartAreas[seriesInfo.Series.ChartArea];
						}
						catch (Exception ex)
						{
							if (AsynchronousExceptionDetection.IsStoppingException(ex))
							{
								throw;
							}
							Global.Tracer.Trace(TraceLevel.Verbose, ex.Message);
						}
						string format = (chartArea == null || seriesInfo.ChartCategoryAxis == null || seriesInfo.ChartCategoryAxis.Style == null || !MappingHelper.IsStylePropertyDefined(seriesInfo.ChartCategoryAxis.Style.Format)) ? "" : MappingHelper.GetStyleFormat(seriesInfo.ChartCategoryAxis.Style, seriesInfo.ChartCategoryAxis.Instance.Style);
						dataPoint.AxisLabel = GetFormattedValue(obj, format);
					}
				}
			}
			if (dateTimeType.HasValue)
			{
				seriesInfo.Series.XValueType = dateTimeType.Value;
			}
		}

		private void RenderDerivedSeriesCollecion()
		{
			if (m_chart.ChartData.DerivedSeriesCollection == null)
			{
				return;
			}
			foreach (ChartDerivedSeries item in m_chart.ChartData.DerivedSeriesCollection)
			{
				RenderDerivedSeries(item);
			}
		}

		private void RenderDerivedSeries(ChartDerivedSeries chartDerivedSeries)
		{
			ChartSeriesFormula derivedSeriesFormula = chartDerivedSeries.DerivedSeriesFormula;
			string sourceChartSeriesName = chartDerivedSeries.SourceChartSeriesName;
			if (sourceChartSeriesName != null && !(sourceChartSeriesName == ""))
			{
				GetSeriesInfo(sourceChartSeriesName);
				string text = "";
				if (chartDerivedSeries.Series != null)
				{
					text = chartDerivedSeries.Series.Name;
				}
				if (text == "")
				{
					text = FormulaHelper.GetDerivedSeriesName(sourceChartSeriesName);
				}
				FormulaHelper.RenderFormulaParameters(chartDerivedSeries.FormulaParameters, derivedSeriesFormula, sourceChartSeriesName, text, out string formulaParameters, out string inputValues, out string outputValues, out bool startFromFirst);
				RenderDerivedSeriesProperties(chartDerivedSeries, text, sourceChartSeriesName, derivedSeriesFormula);
				ApplyFormula(derivedSeriesFormula, formulaParameters, inputValues, outputValues, startFromFirst);
			}
		}

		private void RenderDerivedSeriesProperties(ChartDerivedSeries chartDerivedSeries, string derivedSeriesName, string sourceSeriesName, ChartSeriesFormula formula)
		{
			if (chartDerivedSeries.Series == null)
			{
				return;
			}
			Series series = new Series();
			series.Name = derivedSeriesName;
			SeriesInfo seriesInfo = GetSeriesInfo(sourceSeriesName);
			if (seriesInfo.DerivedSeries == null)
			{
				seriesInfo.DerivedSeries = new List<Series>();
			}
			seriesInfo.DerivedSeries.Add(series);
			RenderSeries(null, chartDerivedSeries.Series, series, IsSeriesLine(chartDerivedSeries.Series));
			if (FormulaHelper.ShouldSendDerivedSeriesBack(series.ChartType))
			{
				m_coreChart.Series.Insert(0, series);
			}
			else
			{
				m_coreChart.Series.Add(series);
			}
			series.ChartArea = GetSeriesChartAreaName(chartDerivedSeries.Series);
			if (!(series.ChartArea == "#NewChartArea"))
			{
				return;
			}
			Series series2 = seriesInfo.Series;
			if (series2 != null)
			{
				Microsoft.Reporting.Chart.WebForms.ChartArea chartArea = GetChartArea(series2.ChartArea);
				if (chartArea != null)
				{
					Microsoft.Reporting.Chart.WebForms.ChartArea chartArea2 = CreateNewChartArea(chartArea, !FormulaHelper.IsNewAreaRequired(formula));
					chartArea2.AlignWithChartArea = chartArea.Name;
					chartArea2.AlignType = AreaAlignTypes.All;
					series.ChartArea = chartArea2.Name;
				}
				else
				{
					series.ChartArea = series2.ChartArea;
				}
			}
		}

		private void AdjustSeriesInLegend(object sender, CustomizeLegendEventArgs e)
		{
			foreach (ChartAreaInfo value in m_chartAreaInfoDictionary.Values)
			{
				foreach (SeriesInfo seriesInfo in value.SeriesInfoList)
				{
					LegendItem seriesLegendItem = GetSeriesLegendItem(seriesInfo.Series, e.LegendItems);
					if (seriesLegendItem != null)
					{
						if (!DataPointShowsInLegend(seriesInfo.ChartSeries))
						{
							AdjustSeriesAppearanceInLegend(seriesInfo, seriesLegendItem);
						}
						if (seriesInfo.DerivedSeries != null)
						{
							AdjustDerivedSeriesInLegend(seriesInfo, seriesLegendItem, e.LegendItems);
						}
					}
				}
			}
		}

		private void AdjustSeriesAppearanceInLegend(SeriesInfo seriesInfo, LegendItem seriesLegendItem)
		{
			DataPoint dataPoint = GetFirstNonEmptyDataPoint(seriesInfo.Series);
			if (dataPoint == null)
			{
				if (seriesInfo.DefaultDataPointAppearance == null)
				{
					return;
				}
				dataPoint = seriesInfo.DefaultDataPointAppearance;
			}
			if (dataPoint.Color != Color.Empty)
			{
				seriesLegendItem.Color = dataPoint.Color;
			}
			if (dataPoint.BackGradientEndColor != Color.Empty)
			{
				seriesLegendItem.BackGradientEndColor = dataPoint.BackGradientEndColor;
			}
			if (dataPoint.BackGradientType != 0)
			{
				seriesLegendItem.BackGradientType = dataPoint.BackGradientType;
			}
			if (dataPoint.BackHatchStyle != 0)
			{
				seriesLegendItem.BackHatchStyle = dataPoint.BackHatchStyle;
			}
			if (dataPoint.BorderColor != Color.Empty)
			{
				seriesLegendItem.BorderColor = dataPoint.BorderColor;
			}
			if (dataPoint.BorderStyle != m_defaultCoreDataPointBorderStyle)
			{
				seriesLegendItem.BorderStyle = dataPoint.BorderStyle;
			}
			if (dataPoint.BorderWidth != m_defaultCoreDataPointBorderWidth)
			{
				seriesLegendItem.BorderWidth = dataPoint.BorderWidth;
			}
			if (dataPoint.BackImage != "")
			{
				seriesLegendItem.Image = dataPoint.BackImage;
			}
			if (dataPoint.BackImageTransparentColor != Color.Empty)
			{
				seriesLegendItem.BackImageTransparentColor = dataPoint.BackImageTransparentColor;
			}
			if (dataPoint.MarkerColor != Color.Empty)
			{
				seriesLegendItem.MarkerColor = dataPoint.MarkerColor;
			}
			if (dataPoint.MarkerBorderColor != Color.Empty)
			{
				seriesLegendItem.MarkerBorderColor = dataPoint.MarkerBorderColor;
			}
			if (dataPoint.MarkerBorderWidth != m_defaultCoreDataPointBorderWidth)
			{
				seriesLegendItem.MarkerBorderWidth = dataPoint.MarkerBorderWidth;
			}
			seriesLegendItem.MarkerSize = dataPoint.MarkerSize;
			if (dataPoint.MarkerStyle != 0)
			{
				seriesLegendItem.MarkerStyle = dataPoint.MarkerStyle;
			}
			if (dataPoint.MarkerImage != "")
			{
				seriesLegendItem.MarkerImage = dataPoint.MarkerImage;
			}
			if (dataPoint.MarkerImageTransparentColor != Color.Empty)
			{
				seriesLegendItem.MarkerImageTransparentColor = dataPoint.MarkerImageTransparentColor;
			}
		}

		private DataPoint GetFirstNonEmptyDataPoint(Series series)
		{
			foreach (DataPoint point in series.Points)
			{
				if (!point.Empty)
				{
					return point;
				}
			}
			return null;
		}

		private void AdjustDerivedSeriesInLegend(SeriesInfo seriesInfo, LegendItem seriesLegendItem, LegendItemsCollection legendItems)
		{
			List<LegendItem> list = new List<LegendItem>();
			foreach (Series item in seriesInfo.DerivedSeries)
			{
				LegendItem seriesLegendItem2 = GetSeriesLegendItem(item, legendItems);
				if (seriesLegendItem2 != null)
				{
					list.Add(seriesLegendItem2);
					legendItems.Remove(seriesLegendItem2);
				}
			}
			int num = legendItems.IndexOf(seriesLegendItem);
			for (int i = 0; i < list.Count; i++)
			{
				num++;
				legendItems.Insert(num, list[i]);
			}
		}

		private LegendItem GetSeriesLegendItem(Series series, LegendItemsCollection legendItemCollection)
		{
			foreach (LegendItem item in legendItemCollection)
			{
				if (series.Name == item.SeriesName)
				{
					return item;
				}
			}
			return null;
		}

		private Series GetSeries(string seriesName)
		{
			try
			{
				return m_coreChart.Series[seriesName];
			}
			catch (Exception ex)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(ex))
				{
					throw;
				}
				Global.Tracer.Trace(TraceLevel.Verbose, ex.Message);
				return null;
			}
		}

		private SeriesInfo GetSeriesInfo(string seriesName)
		{
			foreach (ChartAreaInfo value in m_chartAreaInfoDictionary.Values)
			{
				foreach (SeriesInfo seriesInfo in value.SeriesInfoList)
				{
					if (seriesInfo.ChartSeries.Name == seriesName)
					{
						return seriesInfo;
					}
				}
			}
			return null;
		}

		private Microsoft.Reporting.Chart.WebForms.ChartArea GetChartArea(string chartAreaName)
		{
			try
			{
				return m_coreChart.ChartAreas[chartAreaName];
			}
			catch
			{
				return null;
			}
		}

		private Microsoft.Reporting.Chart.WebForms.ChartArea CreateNewChartArea(Microsoft.Reporting.Chart.WebForms.ChartArea originalChartArea, bool copyYAxisProperties)
		{
			Microsoft.Reporting.Chart.WebForms.ChartArea chartArea = new Microsoft.Reporting.Chart.WebForms.ChartArea();
			m_coreChart.ChartAreas.Add(chartArea);
			if (originalChartArea != null)
			{
				chartArea.BackColor = originalChartArea.BackColor;
				chartArea.BackGradientEndColor = originalChartArea.BackGradientEndColor;
				chartArea.BackGradientType = originalChartArea.BackGradientType;
				chartArea.BackHatchStyle = originalChartArea.BackHatchStyle;
				chartArea.BorderColor = originalChartArea.BorderColor;
				chartArea.BorderStyle = originalChartArea.BorderStyle;
				chartArea.BorderWidth = originalChartArea.BorderWidth;
				chartArea.ShadowColor = originalChartArea.ShadowColor;
				chartArea.ShadowOffset = originalChartArea.ShadowOffset;
				for (int i = 0; i < originalChartArea.Axes.Length; i++)
				{
					Microsoft.Reporting.Chart.WebForms.Axis axis = originalChartArea.Axes[i];
					Microsoft.Reporting.Chart.WebForms.Axis obj = chartArea.Axes[i];
					Grid majorGrid = obj.MajorGrid;
					Grid majorGrid2 = axis.MajorGrid;
					Grid minorGrid = obj.MinorGrid;
					Grid minorGrid2 = axis.MinorGrid;
					TickMark majorTickMark = obj.MajorTickMark;
					TickMark majorTickMark2 = axis.MajorTickMark;
					TickMark minorTickMark = obj.MinorTickMark;
					TickMark minorTickMark2 = axis.MinorTickMark;
					majorGrid.LineColor = majorGrid2.LineColor;
					minorGrid.LineColor = minorGrid2.LineColor;
					majorTickMark.LineColor = majorTickMark2.LineColor;
					minorTickMark.LineColor = minorTickMark2.LineColor;
					majorGrid.LineStyle = majorGrid2.LineStyle;
					minorGrid.LineStyle = minorGrid2.LineStyle;
					majorTickMark.LineStyle = majorTickMark2.LineStyle;
					minorTickMark.LineStyle = minorTickMark2.LineStyle;
					majorGrid.LineWidth = majorGrid2.LineWidth;
					minorGrid.LineWidth = minorGrid2.LineWidth;
					majorTickMark.LineWidth = majorTickMark2.LineWidth;
					minorTickMark.LineWidth = minorTickMark2.LineWidth;
					majorGrid.Enabled = majorGrid2.Enabled;
					minorGrid.Enabled = minorGrid2.Enabled;
					majorTickMark.Enabled = majorTickMark2.Enabled;
					minorTickMark.Enabled = minorTickMark2.Enabled;
					obj.StartFromZero = axis.StartFromZero;
					obj.Margin = axis.Margin;
					obj.Enabled = axis.Enabled;
				}
				CopyAxisRootProperties(originalChartArea.AxisX, chartArea.AxisX);
				if (copyYAxisProperties)
				{
					CopyAxisRootProperties(originalChartArea.AxisY, chartArea.AxisY);
				}
			}
			return chartArea;
		}

		private void CopyAxisRootProperties(Microsoft.Reporting.Chart.WebForms.Axis source, Microsoft.Reporting.Chart.WebForms.Axis target)
		{
			Label labelStyle = target.LabelStyle;
			Label labelStyle2 = source.LabelStyle;
			labelStyle.Font = labelStyle2.Font;
			labelStyle.FontAngle = labelStyle2.FontAngle;
			labelStyle.FontColor = labelStyle2.FontColor;
			labelStyle.Format = labelStyle2.Format;
			labelStyle.Interval = labelStyle2.Interval;
			labelStyle.IntervalOffset = labelStyle2.IntervalOffset;
			labelStyle.IntervalOffsetType = labelStyle2.IntervalOffsetType;
			labelStyle.IntervalType = labelStyle2.IntervalType;
			labelStyle.OffsetLabels = labelStyle2.OffsetLabels;
			labelStyle.ShowEndLabels = labelStyle2.ShowEndLabels;
			labelStyle.TruncatedLabels = labelStyle2.TruncatedLabels;
			Grid majorGrid = source.MajorGrid;
			Grid majorGrid2 = target.MajorGrid;
			if (majorGrid.Enabled)
			{
				majorGrid2.Interval = majorGrid.Interval;
				majorGrid2.IntervalOffset = majorGrid.IntervalOffset;
				majorGrid2.IntervalOffsetType = majorGrid.IntervalOffsetType;
				majorGrid2.IntervalType = majorGrid.IntervalType;
			}
			else
			{
				majorGrid2.Enabled = majorGrid.Enabled;
			}
			Grid minorGrid = source.MinorGrid;
			Grid minorGrid2 = target.MinorGrid;
			if (minorGrid.Enabled)
			{
				minorGrid2.Interval = minorGrid.Interval;
				minorGrid2.IntervalOffset = minorGrid.IntervalOffset;
				minorGrid2.IntervalOffsetType = minorGrid.IntervalOffsetType;
				minorGrid2.IntervalType = minorGrid.IntervalType;
			}
			else
			{
				minorGrid2.Enabled = minorGrid.Enabled;
			}
			TickMark majorTickMark = source.MajorTickMark;
			TickMark majorTickMark2 = target.MajorTickMark;
			if (majorTickMark.Enabled)
			{
				majorTickMark2.Interval = majorTickMark.Interval;
				majorTickMark2.IntervalOffset = majorTickMark.IntervalOffset;
				majorTickMark2.IntervalOffsetType = majorTickMark.IntervalOffsetType;
				majorTickMark2.IntervalType = majorTickMark.IntervalType;
				majorTickMark2.Size = majorTickMark.Size;
				majorTickMark2.Style = majorTickMark.Style;
			}
			else
			{
				majorTickMark2.Enabled = majorTickMark.Enabled;
			}
			TickMark minorTickMark = source.MinorTickMark;
			TickMark minorTickMark2 = target.MinorTickMark;
			if (minorTickMark.Enabled)
			{
				minorTickMark2.Interval = minorTickMark.Interval;
				minorTickMark2.IntervalOffset = minorTickMark.IntervalOffset;
				minorTickMark2.IntervalOffsetType = minorTickMark.IntervalOffsetType;
				minorTickMark2.IntervalType = minorTickMark.IntervalType;
				minorTickMark2.Size = minorTickMark.Size;
				minorTickMark2.Style = minorTickMark.Style;
			}
			else
			{
				minorTickMark2.Enabled = minorTickMark.Enabled;
			}
			target.Arrows = source.Arrows;
			target.Crossing = source.Crossing;
			target.Interlaced = source.Interlaced;
			target.InterlacedColor = source.InterlacedColor;
			target.Interval = source.Interval;
			target.IntervalAutoMode = source.IntervalAutoMode;
			target.IntervalOffset = source.IntervalOffset;
			target.IntervalOffsetType = source.IntervalOffsetType;
			target.IntervalType = source.IntervalType;
			target.LabelsAutoFitMaxFontSize = source.LabelsAutoFitMaxFontSize;
			target.LabelsAutoFitMinFontSize = source.LabelsAutoFitMinFontSize;
			target.LabelsAutoFitStyle = source.LabelsAutoFitStyle;
			target.LabelsAutoFit = source.LabelsAutoFit;
			target.LineColor = source.LineColor;
			target.LineStyle = source.LineStyle;
			target.LineWidth = source.LineWidth;
			target.LogarithmBase = source.LogarithmBase;
			target.Logarithmic = source.Logarithmic;
			target.MarksNextToAxis = source.MarksNextToAxis;
			target.Reverse = source.Reverse;
			target.ScaleBreakStyle = source.ScaleBreakStyle;
			target.ValueType = source.ValueType;
		}

		private void ApplyFormula(ChartSeriesFormula formula, string formulaParameters, string inputValues, string outputValues, bool startFromFirst)
		{
			if (formula == ChartSeriesFormula.Mean || formula == ChartSeriesFormula.Median)
			{
				double num = 0.0;
				num = ((formula != ChartSeriesFormula.Mean) ? m_coreChart.DataManipulator.Statistics.Median(inputValues) : m_coreChart.DataManipulator.Statistics.Mean(inputValues));
				Series series = GetSeries(inputValues);
				Series series2 = GetSeries(outputValues);
				if (series == null || series2 == null)
				{
					return;
				}
				foreach (DataPoint point in series.Points)
				{
					DataPoint dataPoint2 = new DataPoint();
					dataPoint2.XValue = point.XValue;
					dataPoint2.YValues = new double[point.YValues.Length];
					point.YValues.CopyTo(dataPoint2.YValues, 0);
					dataPoint2.AxisLabel = point.AxisLabel;
					if (dataPoint2.YValues.Length != 0)
					{
						dataPoint2.YValues[0] = num;
						series2.Points.Add(dataPoint2);
					}
				}
			}
			else
			{
				m_coreChart.DataManipulator.StartFromFirst = startFromFirst;
				m_coreChart.DataManipulator.FormulaFinancial(GetFinancialFormula(formula), formulaParameters, inputValues, outputValues);
			}
		}

		private FinancialFormula GetFinancialFormula(ChartSeriesFormula formula)
		{
			switch (formula)
			{
			case ChartSeriesFormula.BollingerBands:
				return FinancialFormula.BollingerBands;
			case ChartSeriesFormula.DetrendedPriceOscillator:
				return FinancialFormula.DetrendedPriceOscillator;
			case ChartSeriesFormula.Envelopes:
				return FinancialFormula.Envelopes;
			case ChartSeriesFormula.ExponentialMovingAverage:
				return FinancialFormula.ExponentialMovingAverage;
			case ChartSeriesFormula.MACD:
				return FinancialFormula.MACD;
			case ChartSeriesFormula.MovingAverage:
				return FinancialFormula.MovingAverage;
			case ChartSeriesFormula.Performance:
				return FinancialFormula.Performance;
			case ChartSeriesFormula.RateOfChange:
				return FinancialFormula.RateOfChange;
			case ChartSeriesFormula.RelativeStrengthIndex:
				return FinancialFormula.RelativeStrengthIndex;
			case ChartSeriesFormula.StandardDeviation:
				return FinancialFormula.StandardDeviation;
			case ChartSeriesFormula.TriangularMovingAverage:
				return FinancialFormula.TriangularMovingAverage;
			case ChartSeriesFormula.TRIX:
				return FinancialFormula.TRIX;
			default:
				return FinancialFormula.WeightedMovingAverage;
			}
		}

		private void PostProcessData()
		{
			foreach (KeyValuePair<string, ChartAreaInfo> item in m_chartAreaInfoDictionary)
			{
				AdjustChartAreaData(item);
				AdjustAxesMargin(item);
			}
		}

		private void AdjustAxesMargin(KeyValuePair<string, ChartAreaInfo> chartAreaInfoKeyPair)
		{
			Microsoft.Reporting.Chart.WebForms.ChartArea chartArea = GetChartArea(chartAreaInfoKeyPair.Key);
			ChartAreaInfo value = chartAreaInfoKeyPair.Value;
			if (chartArea == null)
			{
				return;
			}
			List<Microsoft.Reporting.Chart.WebForms.Axis> categoryAxesAutoMargin = value.CategoryAxesAutoMargin;
			if (categoryAxesAutoMargin == null)
			{
				return;
			}
			foreach (Microsoft.Reporting.Chart.WebForms.Axis item in categoryAxesAutoMargin)
			{
				if (item.Enabled == AxisEnabled.True)
				{
					if (item == chartArea.AxisX2)
					{
						item.Margin = chartArea.AxisX.Margin;
					}
					else if (item == chartArea.AxisX)
					{
						item.Margin = chartArea.AxisX2.Margin;
					}
				}
			}
		}

		private void AdjustChartAreaData(KeyValuePair<string, ChartAreaInfo> chartAreaInfo)
		{
			ChartAreaInfo value = chartAreaInfo.Value;
			bool flag = IsXValueSet(value);
			bool flag2 = IsXValueSetFailed(value);
			if (flag && flag2)
			{
				foreach (SeriesInfo seriesInfo in value.SeriesInfoList)
				{
					ClearSeriesXValues(seriesInfo.Series);
				}
			}
			else if (flag && !HasStackedSeries(value))
			{
				foreach (SeriesInfo seriesInfo2 in value.SeriesInfoList)
				{
					ClearNullXValues(seriesInfo2);
				}
			}
			if (!(flag && flag2) && flag)
			{
				return;
			}
			try
			{
				Microsoft.Reporting.Chart.WebForms.ChartArea chartArea = m_coreChart.ChartAreas[chartAreaInfo.Key];
				chartArea.AxisX.Logarithmic = false;
				chartArea.AxisX2.Logarithmic = false;
			}
			catch (Exception ex)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(ex))
				{
					throw;
				}
				Global.Tracer.Trace(TraceLevel.Verbose, ex.Message);
			}
		}

		private void AddSeriesToDictionary(SeriesInfo seriesInfo)
		{
			string chartArea = seriesInfo.Series.ChartArea;
			if (!m_chartAreaInfoDictionary.ContainsKey(chartArea))
			{
				m_chartAreaInfoDictionary.Add(chartArea, new ChartAreaInfo());
			}
			seriesInfo.ChartAreaInfo = m_chartAreaInfoDictionary[chartArea];
			if (m_chart.ChartAreas != null)
			{
				ChartArea byName = m_chart.ChartAreas.GetByName(chartArea);
				if (byName != null && byName.CategoryAxes != null)
				{
					seriesInfo.ChartCategoryAxis = byName.CategoryAxes.GetByName(GetSeriesCategoryAxisName(seriesInfo.ChartSeries));
				}
			}
			m_chartAreaInfoDictionary[chartArea].SeriesInfoList.Add(seriesInfo);
		}

		private void ClearNullXValues(SeriesInfo seriesInfo)
		{
			foreach (DataPoint nullXValuePoint in seriesInfo.NullXValuePoints)
			{
				seriesInfo.Series.Points.Remove(nullXValuePoint);
			}
			seriesInfo.NullXValuePoints.Clear();
		}

		private void ClearSeriesXValues(Series series)
		{
			foreach (DataPoint point in series.Points)
			{
				point.AxisLabel = "";
				point.XValue = 0.0;
			}
		}

		public override void Dispose()
		{
			if (m_coreChart != null)
			{
				m_coreChart.Dispose();
			}
			m_coreChart = null;
			base.Dispose();
		}

		private void OnPostInitialize()
		{
		}

		private void OnPostApplySeriesPointData(Series series, int index)
		{
		}

		private void OnPostApplySeriesData(Series series)
		{
		}

		private void OnPostApplyData()
		{
		}

		private double ConvertToDouble(object value)
		{
			ChartValueTypes? dateTimeType = null;
			return ConvertToDouble(value, checkForMaxMinValue: false, ref dateTimeType);
		}

		private double ConvertToDouble(object value, bool checkForMaxMinValue)
		{
			ChartValueTypes? dateTimeType = null;
			return ConvertToDouble(value, checkForMaxMinValue, ref dateTimeType);
		}

		private double ConvertToDouble(object value, ref ChartValueTypes? dateTimeType)
		{
			return ConvertToDouble(value, checkForMaxMinValue: false, ref dateTimeType);
		}

		private double ConvertToDouble(object value, bool checkForMaxMinValue, ref ChartValueTypes? dateTimeType)
		{
			if (value == null)
			{
				return double.NaN;
			}
			switch (Type.GetTypeCode(value.GetType()))
			{
			case TypeCode.Byte:
				return (int)(byte)value;
			case TypeCode.Char:
				return (int)(char)value;
			case TypeCode.Decimal:
				return decimal.ToDouble((decimal)value);
			case TypeCode.Double:
				return (double)value;
			case TypeCode.Int16:
				return (short)value;
			case TypeCode.Int32:
				return (int)value;
			case TypeCode.Int64:
				return (long)value;
			case TypeCode.SByte:
				return (sbyte)value;
			case TypeCode.Single:
				return (float)value;
			case TypeCode.UInt16:
				return (int)(ushort)value;
			case TypeCode.UInt32:
				return (uint)value;
			case TypeCode.UInt64:
				return (ulong)value;
			case TypeCode.DateTime:
				dateTimeType = ChartValueTypes.DateTime;
				return ConvertDateTimeToDouble((DateTime)value);
			case TypeCode.String:
			{
				string text = value.ToString().Trim();
				if (double.TryParse(text, out double result))
				{
					return result;
				}
				if (checkForMaxMinValue)
				{
					if (text == "MaxValue")
					{
						return double.MaxValue;
					}
					if (text == "MinValue")
					{
						return double.MinValue;
					}
				}
				if (DateTimeUtil.TryParseDateTime(text, null, out DateTimeOffset dateTimeOffset, out bool hasTimeOffset))
				{
					if (hasTimeOffset)
					{
						return ConvertToDouble(dateTimeOffset, checkForMaxMinValue, ref dateTimeType);
					}
					return ConvertToDouble(dateTimeOffset.DateTime, checkForMaxMinValue, ref dateTimeType);
				}
				break;
			}
			}
			if (value is DateTimeOffset)
			{
				dateTimeType = ChartValueTypes.DateTimeOffset;
				return ConvertDateTimeToDouble(((DateTimeOffset)value).UtcDateTime);
			}
			if (value is TimeSpan)
			{
				dateTimeType = ChartValueTypes.Time;
				return ConvertDateTimeToDouble(DateTime.MinValue + (TimeSpan)value);
			}
			return double.NaN;
		}

		private static double ConvertDateTimeToDouble(DateTime dateTime)
		{
			return dateTime.ToOADate();
		}

		private string GetFormattedValue(object value, string format)
		{
			if (m_formatter == null)
			{
				m_formatter = new Formatter(m_chart.ChartDef.StyleClass, m_chart.RenderingContext.OdpContext, ObjectType.Chart, m_chart.Name);
			}
			TypeCode typeCode = Type.GetTypeCode(value.GetType());
			if (typeCode == TypeCode.Object && value is DateTimeOffset)
			{
				typeCode = TypeCode.DateTime;
			}
			return m_formatter.FormatValue(value, format, typeCode);
		}

		private BreakLineType GetScaleBreakLineType(ChartBreakLineType chartBreakLineType)
		{
			switch (chartBreakLineType)
			{
			case ChartBreakLineType.None:
				return BreakLineType.None;
			case ChartBreakLineType.Straight:
				return BreakLineType.Straight;
			case ChartBreakLineType.Wave:
				return BreakLineType.Wave;
			default:
				return BreakLineType.Ragged;
			}
		}

		private AutoBool GetAutoBool(ChartAutoBool autoBool)
		{
			switch (autoBool)
			{
			case ChartAutoBool.True:
				return AutoBool.True;
			case ChartAutoBool.False:
				return AutoBool.False;
			default:
				return AutoBool.Auto;
			}
		}

		private AxisEnabled GetAxisEnabled(ChartAutoBool autoBool)
		{
			switch (autoBool)
			{
			case ChartAutoBool.True:
				return AxisEnabled.True;
			case ChartAutoBool.False:
				return AxisEnabled.False;
			default:
				return AxisEnabled.Auto;
			}
		}

		private bool GetMargin(ChartAutoBool autoBool)
		{
			if (autoBool == ChartAutoBool.False)
			{
				return false;
			}
			return true;
		}

		private bool DoesSeriesRequireMargin(ChartSeries chartSeries)
		{
			ChartSeriesType seriesType = GetSeriesType(chartSeries);
			ChartSeriesSubtype seriesSubType = GetSeriesSubType(chartSeries);
			if (seriesType == ChartSeriesType.Area || (seriesType == ChartSeriesType.Range && (seriesSubType == ChartSeriesSubtype.Plain || seriesSubType == ChartSeriesSubtype.Smooth)))
			{
				return false;
			}
			return true;
		}

		private StringAlignment GetStringAlignmentFromTextAlignments(TextAlignments value)
		{
			switch (value)
			{
			case TextAlignments.Center:
				return StringAlignment.Center;
			case TextAlignments.Right:
				return StringAlignment.Far;
			default:
				return StringAlignment.Near;
			}
		}

		private StringAlignment GetStringAlignmentFromVericalAlignments(VerticalAlignments value)
		{
			switch (value)
			{
			case VerticalAlignments.Middle:
				return StringAlignment.Center;
			case VerticalAlignments.Bottom:
				return StringAlignment.Far;
			default:
				return StringAlignment.Near;
			}
		}

		private string GetSeriesLegendText(ChartMember seriesGrouping)
		{
			return GetGroupingLegendText(seriesGrouping);
		}

		private string GetDataPointLegendText(ChartMember categoryGrouping, ChartMember seriesGrouping)
		{
			string text = "";
			if (m_multiColumn)
			{
				text = GetGroupingLegendText(categoryGrouping);
			}
			if (m_multiRow)
			{
				if (text != "")
				{
					text += m_legendTextSeparator;
				}
				text += GetGroupingLegendText(seriesGrouping);
			}
			return text;
		}

		private string GetGroupingLegendText(ChartMember grouping)
		{
			ChartMember chartMember = grouping;
			string text = "";
			do
			{
				string text2 = GetGroupingLabel(chartMember);
				if (chartMember.Children != null && text2 != "" && text != "")
				{
					text2 += m_legendTextSeparator;
				}
				text = text.Insert(0, text2);
				chartMember = chartMember.Parent;
			}
			while (chartMember != null);
			return text;
		}

		private string GetGroupingLabel(ChartMember grouping)
		{
			if (grouping.Instance.Label == null)
			{
				return "";
			}
			return grouping.Instance.Label;
		}

		private string GetFormattedGroupingLabel(ChartMember categoryGrouping, string chartAreaName, ChartAxis categoryAxis)
		{
			object labelObject = categoryGrouping.Instance.LabelObject;
			if (labelObject == null)
			{
				return " ";
			}
			string format = (GetChartArea(chartAreaName) == null || categoryAxis == null || categoryAxis.Style == null || !MappingHelper.IsStylePropertyDefined(categoryAxis.Style.Format)) ? "" : MappingHelper.GetStyleFormat(categoryAxis.Style, categoryAxis.Instance.Style);
			return GetFormattedValue(labelObject, format);
		}

		private int GetGroupingLevel(ChartMember grouping)
		{
			int num = -1;
			if (grouping.Children != null)
			{
				foreach (ChartMember child in grouping.Children)
				{
					int groupingLevel = GetGroupingLevel(child);
					if (num < groupingLevel)
					{
						num = groupingLevel;
					}
				}
			}
			return num + 1;
		}

		private bool IsChartEmpty()
		{
			foreach (Series item in m_coreChart.Series)
			{
				if (item.Points.Count > 0)
				{
					return false;
				}
			}
			return true;
		}

		private bool DataPointShowsInLegend(ChartSeries chartSeries)
		{
			if (GetSeriesType(chartSeries) != ChartSeriesType.Shape)
			{
				return false;
			}
			ChartSeriesSubtype seriesSubType = GetSeriesSubType(chartSeries);
			if (seriesSubType == ChartSeriesSubtype.TreeMap || seriesSubType == ChartSeriesSubtype.Sunburst)
			{
				return false;
			}
			return true;
		}

		private bool IsSeriesCollectedPie(Series series)
		{
			if (series.ChartType != SeriesChartType.Pie && series.ChartType != SeriesChartType.Doughnut)
			{
				return false;
			}
			if (series["CollectedStyle"] == null)
			{
				return false;
			}
			return Microsoft.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(series["CollectedStyle"], "CollectedPie");
		}

		private bool IsSeriesPareto(Series series)
		{
			if (series.ChartType != SeriesChartType.Column)
			{
				return false;
			}
			if (series["ShowColumnAs"] == null)
			{
				return false;
			}
			return Microsoft.ReportingServices.ReportProcessing.ReportProcessing.CompareWithInvariantCulture(series["ShowColumnAs"], "pareto", ignoreCase: true) == 0;
		}

		private bool IsSeriesHistogram(Series series)
		{
			if (series.ChartType != SeriesChartType.Column)
			{
				return false;
			}
			if (series["ShowColumnAs"] == null)
			{
				return false;
			}
			return Microsoft.ReportingServices.ReportProcessing.ReportProcessing.CompareWithInvariantCulture(series["ShowColumnAs"], "histogram", ignoreCase: true) == 0;
		}

		private bool IsGradientPerDataPointSupported(ChartSeries chartSeries)
		{
			ChartSeriesType seriesType = GetSeriesType(chartSeries);
			switch (seriesType)
			{
			case ChartSeriesType.Area:
				return false;
			case ChartSeriesType.Range:
			{
				ChartSeriesSubtype validSeriesSubType = GetValidSeriesSubType(seriesType, GetSeriesSubType(chartSeries));
				if (validSeriesSubType == ChartSeriesSubtype.Plain || validSeriesSubType == ChartSeriesSubtype.Smooth)
				{
					return false;
				}
				break;
			}
			}
			return true;
		}

		private bool IsSeriesStacked(ChartSeries chartSeries)
		{
			ChartSeriesSubtype validSeriesSubType = GetValidSeriesSubType(GetSeriesType(chartSeries), GetSeriesSubType(chartSeries));
			if (validSeriesSubType != ChartSeriesSubtype.Stacked)
			{
				return validSeriesSubType == ChartSeriesSubtype.PercentStacked;
			}
			return true;
		}

		private bool IsSeriesLine(ChartSeries chartSeries)
		{
			return GetSeriesType(chartSeries) == ChartSeriesType.Line;
		}

		private bool IsSeriesRange(ChartSeries chartSeries)
		{
			return GetSeriesType(chartSeries) == ChartSeriesType.Range;
		}

		private bool IsSeriesBubble(ChartSeries chartSeries)
		{
			return GetValidSeriesSubType(GetSeriesType(chartSeries), GetSeriesSubType(chartSeries)) == ChartSeriesSubtype.Bubble;
		}

		private bool IsSeriesExploded(ChartSeries chartSeries)
		{
			ChartSeriesSubtype validSeriesSubType = GetValidSeriesSubType(GetSeriesType(chartSeries), GetSeriesSubType(chartSeries));
			if (validSeriesSubType != ChartSeriesSubtype.ExplodedDoughnut)
			{
				return validSeriesSubType == ChartSeriesSubtype.ExplodedPie;
			}
			return true;
		}

		private bool CanSetCategoryGroupingLabels(ChartAreaInfo seriesInfoList)
		{
			bool flag = IsXValueSet(seriesInfoList);
			if (!flag || !IsXValueSetFailed(seriesInfoList))
			{
				return !flag;
			}
			return true;
		}

		private bool CanSetPieDataPointLegendText(Series series, DataPoint dataPoint)
		{
			if (!dataPoint.Empty)
			{
				return dataPoint.LegendText == string.Empty;
			}
			return series.EmptyPointStyle.LegendText == string.Empty;
		}

		private bool CanSetDataPointAxisLabel(Series series, DataPoint dataPoint)
		{
			if (!dataPoint.Empty)
			{
				return dataPoint.AxisLabel == string.Empty;
			}
			return series.EmptyPointStyle.AxisLabel == string.Empty;
		}

		private bool IsXValueSet(ChartAreaInfo seriesInfoList)
		{
			foreach (SeriesInfo seriesInfo in seriesInfoList.SeriesInfoList)
			{
				if (seriesInfo.XValueSet)
				{
					return true;
				}
			}
			return false;
		}

		private bool IsXValueSetFailed(ChartAreaInfo seriesInfoList)
		{
			foreach (SeriesInfo seriesInfo in seriesInfoList.SeriesInfoList)
			{
				if (seriesInfo.XValueSetFailed)
				{
					return true;
				}
			}
			return false;
		}

		private bool HasStackedSeries(ChartAreaInfo seriesInfoList)
		{
			foreach (SeriesInfo seriesInfo in seriesInfoList.SeriesInfoList)
			{
				if (IsSeriesStacked(seriesInfo.ChartSeries))
				{
					return true;
				}
			}
			return false;
		}

		private GradientType GetGradientType(BackgroundGradients chartGradientType)
		{
			switch (chartGradientType)
			{
			case BackgroundGradients.Center:
				return GradientType.Center;
			case BackgroundGradients.DiagonalLeft:
				return GradientType.DiagonalLeft;
			case BackgroundGradients.DiagonalRight:
				return GradientType.DiagonalRight;
			case BackgroundGradients.HorizontalCenter:
				return GradientType.HorizontalCenter;
			case BackgroundGradients.LeftRight:
				return GradientType.LeftRight;
			case BackgroundGradients.TopBottom:
				return GradientType.TopBottom;
			case BackgroundGradients.VerticalCenter:
				return GradientType.VerticalCenter;
			default:
				return GradientType.None;
			}
		}

		private ChartHatchStyle GetHatchType(BackgroundHatchTypes chartHatchType)
		{
			switch (chartHatchType)
			{
			case BackgroundHatchTypes.BackwardDiagonal:
				return ChartHatchStyle.BackwardDiagonal;
			case BackgroundHatchTypes.Cross:
				return ChartHatchStyle.Cross;
			case BackgroundHatchTypes.DarkDownwardDiagonal:
				return ChartHatchStyle.DarkDownwardDiagonal;
			case BackgroundHatchTypes.DarkHorizontal:
				return ChartHatchStyle.DarkHorizontal;
			case BackgroundHatchTypes.DarkUpwardDiagonal:
				return ChartHatchStyle.DarkUpwardDiagonal;
			case BackgroundHatchTypes.DarkVertical:
				return ChartHatchStyle.DarkVertical;
			case BackgroundHatchTypes.DashedDownwardDiagonal:
				return ChartHatchStyle.DashedDownwardDiagonal;
			case BackgroundHatchTypes.DashedHorizontal:
				return ChartHatchStyle.DashedHorizontal;
			case BackgroundHatchTypes.DashedUpwardDiagonal:
				return ChartHatchStyle.DashedUpwardDiagonal;
			case BackgroundHatchTypes.DashedVertical:
				return ChartHatchStyle.DashedVertical;
			case BackgroundHatchTypes.DiagonalBrick:
				return ChartHatchStyle.DiagonalBrick;
			case BackgroundHatchTypes.DiagonalCross:
				return ChartHatchStyle.DiagonalCross;
			case BackgroundHatchTypes.Divot:
				return ChartHatchStyle.Divot;
			case BackgroundHatchTypes.DottedDiamond:
				return ChartHatchStyle.DottedDiamond;
			case BackgroundHatchTypes.DottedGrid:
				return ChartHatchStyle.DottedGrid;
			case BackgroundHatchTypes.ForwardDiagonal:
				return ChartHatchStyle.ForwardDiagonal;
			case BackgroundHatchTypes.Horizontal:
				return ChartHatchStyle.Horizontal;
			case BackgroundHatchTypes.HorizontalBrick:
				return ChartHatchStyle.HorizontalBrick;
			case BackgroundHatchTypes.LargeCheckerBoard:
				return ChartHatchStyle.LargeCheckerBoard;
			case BackgroundHatchTypes.LargeConfetti:
				return ChartHatchStyle.LargeConfetti;
			case BackgroundHatchTypes.LargeGrid:
				return ChartHatchStyle.LargeGrid;
			case BackgroundHatchTypes.LightDownwardDiagonal:
				return ChartHatchStyle.LightDownwardDiagonal;
			case BackgroundHatchTypes.LightHorizontal:
				return ChartHatchStyle.LightHorizontal;
			case BackgroundHatchTypes.LightUpwardDiagonal:
				return ChartHatchStyle.LightUpwardDiagonal;
			case BackgroundHatchTypes.LightVertical:
				return ChartHatchStyle.LightVertical;
			case BackgroundHatchTypes.NarrowHorizontal:
				return ChartHatchStyle.NarrowHorizontal;
			case BackgroundHatchTypes.OutlinedDiamond:
				return ChartHatchStyle.OutlinedDiamond;
			case BackgroundHatchTypes.Percent05:
				return ChartHatchStyle.Percent05;
			case BackgroundHatchTypes.Percent10:
				return ChartHatchStyle.Percent10;
			case BackgroundHatchTypes.Percent20:
				return ChartHatchStyle.Percent20;
			case BackgroundHatchTypes.Percent25:
				return ChartHatchStyle.Percent25;
			case BackgroundHatchTypes.Percent30:
				return ChartHatchStyle.Percent30;
			case BackgroundHatchTypes.Percent40:
				return ChartHatchStyle.Percent40;
			case BackgroundHatchTypes.Percent50:
				return ChartHatchStyle.Percent50;
			case BackgroundHatchTypes.Percent60:
				return ChartHatchStyle.Percent60;
			case BackgroundHatchTypes.Percent70:
				return ChartHatchStyle.Percent70;
			case BackgroundHatchTypes.Percent75:
				return ChartHatchStyle.Percent75;
			case BackgroundHatchTypes.Percent80:
				return ChartHatchStyle.Percent80;
			case BackgroundHatchTypes.Percent90:
				return ChartHatchStyle.Percent90;
			case BackgroundHatchTypes.Plaid:
				return ChartHatchStyle.Plaid;
			case BackgroundHatchTypes.Shingle:
				return ChartHatchStyle.Shingle;
			case BackgroundHatchTypes.SmallCheckerBoard:
				return ChartHatchStyle.SmallCheckerBoard;
			case BackgroundHatchTypes.SmallConfetti:
				return ChartHatchStyle.SmallConfetti;
			case BackgroundHatchTypes.SmallGrid:
				return ChartHatchStyle.SmallGrid;
			case BackgroundHatchTypes.SolidDiamond:
				return ChartHatchStyle.SolidDiamond;
			case BackgroundHatchTypes.Sphere:
				return ChartHatchStyle.Sphere;
			case BackgroundHatchTypes.Trellis:
				return ChartHatchStyle.Trellis;
			case BackgroundHatchTypes.Vertical:
				return ChartHatchStyle.Vertical;
			case BackgroundHatchTypes.Wave:
				return ChartHatchStyle.Wave;
			case BackgroundHatchTypes.Weave:
				return ChartHatchStyle.Weave;
			case BackgroundHatchTypes.WideDownwardDiagonal:
				return ChartHatchStyle.WideDownwardDiagonal;
			case BackgroundHatchTypes.WideUpwardDiagonal:
				return ChartHatchStyle.WideUpwardDiagonal;
			case BackgroundHatchTypes.ZigZag:
				return ChartHatchStyle.ZigZag;
			default:
				return ChartHatchStyle.None;
			}
		}

		private ChartDashStyle GetBorderStyle(BorderStyles chartBorderStyle, bool isLine)
		{
			switch (chartBorderStyle)
			{
			case BorderStyles.DashDot:
				return ChartDashStyle.DashDot;
			case BorderStyles.DashDotDot:
				return ChartDashStyle.DashDotDot;
			case BorderStyles.Dashed:
				return ChartDashStyle.Dash;
			case BorderStyles.Dotted:
				return ChartDashStyle.Dot;
			case BorderStyles.None:
				return ChartDashStyle.NotSet;
			case BorderStyles.Solid:
				return ChartDashStyle.Solid;
			default:
				if (isLine)
				{
					return ChartDashStyle.Solid;
				}
				return ChartDashStyle.NotSet;
			}
		}

		private ChartImageWrapMode GetBackImageMode(BackgroundRepeatTypes backgroundImageRepeatType)
		{
			switch (backgroundImageRepeatType)
			{
			case BackgroundRepeatTypes.Repeat:
				return ChartImageWrapMode.Tile;
			case BackgroundRepeatTypes.Clip:
				return ChartImageWrapMode.Unscaled;
			default:
				return ChartImageWrapMode.Scaled;
			}
		}

		private ChartImageAlign GetBackImageAlign(Positions position)
		{
			switch (position)
			{
			case Positions.Bottom:
				return ChartImageAlign.Bottom;
			case Positions.BottomLeft:
				return ChartImageAlign.BottomLeft;
			case Positions.BottomRight:
				return ChartImageAlign.BottomRight;
			case Positions.Center:
				return ChartImageAlign.Center;
			case Positions.Left:
				return ChartImageAlign.Left;
			case Positions.Right:
				return ChartImageAlign.Right;
			case Positions.Top:
				return ChartImageAlign.Top;
			case Positions.TopLeft:
				return ChartImageAlign.TopLeft;
			case Positions.TopRight:
				return ChartImageAlign.TopRight;
			default:
				return ChartImageAlign.TopLeft;
			}
		}

		private LabelOutsidePlotAreaStyle GetLabelOutsidePlotAreaStyle(ChartAllowOutsideChartArea chartAllowOutsideChartArea)
		{
			switch (chartAllowOutsideChartArea)
			{
			case ChartAllowOutsideChartArea.False:
				return LabelOutsidePlotAreaStyle.No;
			case ChartAllowOutsideChartArea.True:
				return LabelOutsidePlotAreaStyle.Yes;
			default:
				return LabelOutsidePlotAreaStyle.Partial;
			}
		}

		private DateTimeIntervalType GetDateTimeIntervalType(ChartIntervalType chartIntervalType)
		{
			switch (chartIntervalType)
			{
			case ChartIntervalType.Days:
				return DateTimeIntervalType.Days;
			case ChartIntervalType.Hours:
				return DateTimeIntervalType.Hours;
			case ChartIntervalType.Milliseconds:
				return DateTimeIntervalType.Milliseconds;
			case ChartIntervalType.Minutes:
				return DateTimeIntervalType.Minutes;
			case ChartIntervalType.Months:
				return DateTimeIntervalType.Months;
			case ChartIntervalType.Number:
				return DateTimeIntervalType.Number;
			case ChartIntervalType.Seconds:
				return DateTimeIntervalType.Seconds;
			case ChartIntervalType.Weeks:
				return DateTimeIntervalType.Weeks;
			case ChartIntervalType.Years:
				return DateTimeIntervalType.Years;
			case ChartIntervalType.Default:
				return DateTimeIntervalType.Auto;
			default:
				return DateTimeIntervalType.Auto;
			}
		}

		private IntervalAutoMode GetIntervalAutoMode(bool variableAutoInterval)
		{
			if (!variableAutoInterval)
			{
				return IntervalAutoMode.FixedCount;
			}
			return IntervalAutoMode.VariableCount;
		}

		private Microsoft.Reporting.Chart.WebForms.TickMarkStyle GetTickMarkStyle(ChartTickMarksType chartTickMarksType)
		{
			switch (chartTickMarksType)
			{
			case ChartTickMarksType.Cross:
				return Microsoft.Reporting.Chart.WebForms.TickMarkStyle.Cross;
			case ChartTickMarksType.Inside:
				return Microsoft.Reporting.Chart.WebForms.TickMarkStyle.Inside;
			case ChartTickMarksType.Outside:
				return Microsoft.Reporting.Chart.WebForms.TickMarkStyle.Outside;
			default:
				return Microsoft.Reporting.Chart.WebForms.TickMarkStyle.None;
			}
		}
	}
}
