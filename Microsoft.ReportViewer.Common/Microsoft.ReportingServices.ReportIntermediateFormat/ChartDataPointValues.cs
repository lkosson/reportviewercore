using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.RdlExpressions;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class ChartDataPointValues : IPersistable
	{
		private ExpressionInfo m_x;

		private ExpressionInfo m_y;

		private ExpressionInfo m_size;

		private ExpressionInfo m_high;

		private ExpressionInfo m_low;

		private ExpressionInfo m_start;

		private ExpressionInfo m_end;

		private ExpressionInfo m_mean;

		private ExpressionInfo m_median;

		private ExpressionInfo m_highlightX;

		private ExpressionInfo m_highlightY;

		private ExpressionInfo m_highlightSize;

		private ExpressionInfo m_formatX;

		private ExpressionInfo m_formatY;

		private ExpressionInfo m_formatSize;

		private ExpressionInfo m_currencyLanguageX;

		private ExpressionInfo m_currencyLanguageY;

		private ExpressionInfo m_currencyLanguageSize;

		[Reference]
		private ChartDataPoint m_dataPoint;

		[Reference]
		private Chart m_chart;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal ExpressionInfo X
		{
			get
			{
				return m_x;
			}
			set
			{
				m_x = value;
			}
		}

		internal ExpressionInfo Y
		{
			get
			{
				return m_y;
			}
			set
			{
				m_y = value;
			}
		}

		internal ExpressionInfo Size
		{
			get
			{
				return m_size;
			}
			set
			{
				m_size = value;
			}
		}

		internal ExpressionInfo High
		{
			get
			{
				return m_high;
			}
			set
			{
				m_high = value;
			}
		}

		internal ExpressionInfo Low
		{
			get
			{
				return m_low;
			}
			set
			{
				m_low = value;
			}
		}

		internal ExpressionInfo Start
		{
			get
			{
				return m_start;
			}
			set
			{
				m_start = value;
			}
		}

		internal ExpressionInfo End
		{
			get
			{
				return m_end;
			}
			set
			{
				m_end = value;
			}
		}

		internal ExpressionInfo Mean
		{
			get
			{
				return m_mean;
			}
			set
			{
				m_mean = value;
			}
		}

		internal ExpressionInfo Median
		{
			get
			{
				return m_median;
			}
			set
			{
				m_median = value;
			}
		}

		internal ChartDataPoint DataPoint
		{
			set
			{
				m_dataPoint = value;
			}
		}

		internal ExpressionInfo HighlightX
		{
			get
			{
				return m_highlightX;
			}
			set
			{
				m_highlightX = value;
			}
		}

		internal ExpressionInfo HighlightY
		{
			get
			{
				return m_highlightY;
			}
			set
			{
				m_highlightY = value;
			}
		}

		internal ExpressionInfo HighlightSize
		{
			get
			{
				return m_highlightSize;
			}
			set
			{
				m_highlightSize = value;
			}
		}

		internal ExpressionInfo FormatX
		{
			get
			{
				return m_formatX;
			}
			set
			{
				m_formatX = value;
			}
		}

		internal ExpressionInfo FormatY
		{
			get
			{
				return m_formatY;
			}
			set
			{
				m_formatY = value;
			}
		}

		internal ExpressionInfo FormatSize
		{
			get
			{
				return m_formatSize;
			}
			set
			{
				m_formatSize = value;
			}
		}

		internal ExpressionInfo CurrencyLanguageX
		{
			get
			{
				return m_currencyLanguageX;
			}
			set
			{
				m_currencyLanguageX = value;
			}
		}

		internal ExpressionInfo CurrencyLanguageY
		{
			get
			{
				return m_currencyLanguageY;
			}
			set
			{
				m_currencyLanguageY = value;
			}
		}

		internal ExpressionInfo CurrencyLanguageSize
		{
			get
			{
				return m_currencyLanguageSize;
			}
			set
			{
				m_currencyLanguageSize = value;
			}
		}

		internal ChartDataPointValues()
		{
		}

		internal ChartDataPointValues(Chart chart, ChartDataPoint dataPoint)
		{
			m_dataPoint = dataPoint;
			m_chart = chart;
		}

		internal void Initialize(InitializationContext context)
		{
			if (m_x != null)
			{
				m_x.Initialize("X", context);
				context.ExprHostBuilder.ChartDataPointValueX(m_x);
			}
			if (m_y != null)
			{
				m_y.Initialize("Y", context);
				context.ExprHostBuilder.ChartDataPointValueY(m_y);
			}
			if (m_size != null)
			{
				m_size.Initialize("Size", context);
				context.ExprHostBuilder.ChartDataPointValueSize(m_size);
			}
			if (m_high != null)
			{
				m_high.Initialize("High", context);
				context.ExprHostBuilder.ChartDataPointValueHigh(m_high);
			}
			if (m_low != null)
			{
				m_low.Initialize("Low", context);
				context.ExprHostBuilder.ChartDataPointValueLow(m_low);
			}
			if (m_start != null)
			{
				m_start.Initialize("Start", context);
				context.ExprHostBuilder.ChartDataPointValueStart(m_start);
			}
			if (m_end != null)
			{
				m_end.Initialize("End", context);
				context.ExprHostBuilder.ChartDataPointValueEnd(m_end);
			}
			if (m_mean != null)
			{
				m_mean.Initialize("Mean", context);
				context.ExprHostBuilder.ChartDataPointValueMean(m_mean);
			}
			if (m_median != null)
			{
				m_median.Initialize("Median", context);
				context.ExprHostBuilder.ChartDataPointValueMedian(m_median);
			}
			if (m_highlightX != null)
			{
				m_highlightX.Initialize("HighlightX", context);
				context.ExprHostBuilder.ChartDataPointValueHighlightX(m_highlightX);
			}
			if (m_highlightY != null)
			{
				m_highlightY.Initialize("HighlightY", context);
				context.ExprHostBuilder.ChartDataPointValueHighlightY(m_highlightY);
			}
			if (m_highlightSize != null)
			{
				m_highlightSize.Initialize("HighlightSize", context);
				context.ExprHostBuilder.ChartDataPointValueHighlightSize(m_highlightSize);
			}
			if (m_formatX != null)
			{
				m_formatX.Initialize("FormatX", context);
				context.ExprHostBuilder.ChartDataPointValueFormatX(m_formatX);
			}
			if (m_formatY != null)
			{
				m_formatY.Initialize("FormatY", context);
				context.ExprHostBuilder.ChartDataPointValueFormatY(m_formatY);
			}
			if (m_formatSize != null)
			{
				m_formatSize.Initialize("FormatSize", context);
				context.ExprHostBuilder.ChartDataPointValueFormatSize(m_formatSize);
			}
			if (m_currencyLanguageX != null)
			{
				m_currencyLanguageX.Initialize("CurrencyLanguageX", context);
				context.ExprHostBuilder.ChartDataPointValueCurrencyLanguageX(m_currencyLanguageX);
			}
			if (m_currencyLanguageY != null)
			{
				m_currencyLanguageY.Initialize("CurrencyLanguageY", context);
				context.ExprHostBuilder.ChartDataPointValueCurrencyLanguageY(m_currencyLanguageY);
			}
			if (m_currencyLanguageSize != null)
			{
				m_currencyLanguageSize.Initialize("CurrencyLanguageSize", context);
				context.ExprHostBuilder.ChartDataPointValueCurrencyLanguageSize(m_currencyLanguageSize);
			}
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.X, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Y, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Size, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.High, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Low, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Start, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.End, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Mean, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Median, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ChartDataPoint, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartDataPoint, Token.Reference));
			list.Add(new MemberInfo(MemberName.Chart, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Chart, Token.Reference));
			list.Add(new MemberInfo(MemberName.HighlightX, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.HighlightY, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.HighlightSize, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.FormatX, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo, Lifetime.AddedIn(200)));
			list.Add(new MemberInfo(MemberName.FormatY, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo, Lifetime.AddedIn(200)));
			list.Add(new MemberInfo(MemberName.FormatSize, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo, Lifetime.AddedIn(200)));
			list.Add(new MemberInfo(MemberName.CurrencyLanguageX, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo, Lifetime.AddedIn(200)));
			list.Add(new MemberInfo(MemberName.CurrencyLanguageY, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo, Lifetime.AddedIn(200)));
			list.Add(new MemberInfo(MemberName.CurrencyLanguageSize, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo, Lifetime.AddedIn(200)));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartDataPointValues, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.X:
					writer.Write(m_x);
					break;
				case MemberName.Y:
					writer.Write(m_y);
					break;
				case MemberName.Size:
					writer.Write(m_size);
					break;
				case MemberName.High:
					writer.Write(m_high);
					break;
				case MemberName.Low:
					writer.Write(m_low);
					break;
				case MemberName.Start:
					writer.Write(m_start);
					break;
				case MemberName.End:
					writer.Write(m_end);
					break;
				case MemberName.Mean:
					writer.Write(m_mean);
					break;
				case MemberName.Median:
					writer.Write(m_median);
					break;
				case MemberName.HighlightX:
					writer.Write(m_highlightX);
					break;
				case MemberName.HighlightY:
					writer.Write(m_highlightY);
					break;
				case MemberName.HighlightSize:
					writer.Write(m_highlightSize);
					break;
				case MemberName.FormatX:
					writer.Write(m_formatX);
					break;
				case MemberName.FormatY:
					writer.Write(m_formatY);
					break;
				case MemberName.FormatSize:
					writer.Write(m_formatSize);
					break;
				case MemberName.CurrencyLanguageX:
					writer.Write(m_currencyLanguageX);
					break;
				case MemberName.CurrencyLanguageY:
					writer.Write(m_currencyLanguageY);
					break;
				case MemberName.CurrencyLanguageSize:
					writer.Write(m_currencyLanguageSize);
					break;
				case MemberName.ChartDataPoint:
					writer.WriteReference(m_dataPoint);
					break;
				case MemberName.Chart:
					writer.WriteReference(m_chart);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.X:
					m_x = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Y:
					m_y = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Size:
					m_size = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.High:
					m_high = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Low:
					m_low = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Start:
					m_start = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.End:
					m_end = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Mean:
					m_mean = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Median:
					m_median = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.HighlightX:
					m_highlightX = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.HighlightY:
					m_highlightY = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.HighlightSize:
					m_highlightSize = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.FormatX:
					m_formatX = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.FormatY:
					m_formatY = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.FormatSize:
					m_formatSize = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.CurrencyLanguageX:
					m_currencyLanguageX = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.CurrencyLanguageY:
					m_currencyLanguageY = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.CurrencyLanguageSize:
					m_currencyLanguageSize = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ChartDataPoint:
					m_dataPoint = reader.ReadReference<ChartDataPoint>(this);
					break;
				case MemberName.Chart:
					m_chart = reader.ReadReference<Chart>(this);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			if (!memberReferencesCollection.TryGetValue(m_Declaration.ObjectType, out List<MemberReference> value))
			{
				return;
			}
			foreach (MemberReference item in value)
			{
				switch (item.MemberName)
				{
				case MemberName.ChartDataPoint:
					Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
					m_dataPoint = (ChartDataPoint)referenceableItems[item.RefID];
					break;
				case MemberName.Chart:
					Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
					m_chart = (Chart)referenceableItems[item.RefID];
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartDataPointValues;
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			ChartDataPointValues chartDataPointValues = (ChartDataPointValues)MemberwiseClone();
			chartDataPointValues.m_chart = (Chart)context.CurrentDataRegionClone;
			if (m_x != null)
			{
				chartDataPointValues.m_x = (ExpressionInfo)m_x.PublishClone(context);
			}
			if (m_y != null)
			{
				chartDataPointValues.m_y = (ExpressionInfo)m_y.PublishClone(context);
			}
			if (m_size != null)
			{
				chartDataPointValues.m_size = (ExpressionInfo)m_size.PublishClone(context);
			}
			if (m_high != null)
			{
				chartDataPointValues.m_high = (ExpressionInfo)m_high.PublishClone(context);
			}
			if (m_low != null)
			{
				chartDataPointValues.m_low = (ExpressionInfo)m_low.PublishClone(context);
			}
			if (m_start != null)
			{
				chartDataPointValues.m_start = (ExpressionInfo)m_start.PublishClone(context);
			}
			if (m_end != null)
			{
				chartDataPointValues.m_end = (ExpressionInfo)m_end.PublishClone(context);
			}
			if (m_mean != null)
			{
				chartDataPointValues.m_mean = (ExpressionInfo)m_mean.PublishClone(context);
			}
			if (m_median != null)
			{
				chartDataPointValues.m_median = (ExpressionInfo)m_median.PublishClone(context);
			}
			return chartDataPointValues;
		}

		internal Microsoft.ReportingServices.RdlExpressions.VariantResult EvaluateX(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_dataPoint, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartDataPointValuesXExpression(m_dataPoint, m_chart.Name);
		}

		internal Microsoft.ReportingServices.RdlExpressions.VariantResult EvaluateY(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_dataPoint, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartDataPointValuesYExpression(m_dataPoint, m_chart.Name);
		}

		internal Microsoft.ReportingServices.RdlExpressions.VariantResult EvaluateSize(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_dataPoint, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartDataPointValueSizesExpression(m_dataPoint, m_chart.Name);
		}

		internal Microsoft.ReportingServices.RdlExpressions.VariantResult EvaluateHigh(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_dataPoint, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartDataPointValuesHighExpression(m_dataPoint, m_chart.Name);
		}

		internal Microsoft.ReportingServices.RdlExpressions.VariantResult EvaluateLow(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_dataPoint, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartDataPointValuesLowExpression(m_dataPoint, m_chart.Name);
		}

		internal Microsoft.ReportingServices.RdlExpressions.VariantResult EvaluateStart(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_dataPoint, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartDataPointValuesStartExpression(m_dataPoint, m_chart.Name);
		}

		internal Microsoft.ReportingServices.RdlExpressions.VariantResult EvaluateEnd(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_dataPoint, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartDataPointValuesEndExpression(m_dataPoint, m_chart.Name);
		}

		internal Microsoft.ReportingServices.RdlExpressions.VariantResult EvaluateMean(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_dataPoint, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartDataPointValuesMeanExpression(m_dataPoint, m_chart.Name);
		}

		internal Microsoft.ReportingServices.RdlExpressions.VariantResult EvaluateMedian(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_dataPoint, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartDataPointValuesMedianExpression(m_dataPoint, m_chart.Name);
		}

		internal Microsoft.ReportingServices.RdlExpressions.VariantResult EvaluateHighlightX(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_dataPoint, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartDataPointValuesHighlightXExpression(m_dataPoint, m_chart.Name);
		}

		internal Microsoft.ReportingServices.RdlExpressions.VariantResult EvaluateHighlightY(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_dataPoint, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartDataPointValuesHighlightYExpression(m_dataPoint, m_chart.Name);
		}

		internal Microsoft.ReportingServices.RdlExpressions.VariantResult EvaluateHighlightSize(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_dataPoint, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartDataPointValuesHighlightSizeExpression(m_dataPoint, m_chart.Name);
		}

		internal string EvaluateFormatX(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_dataPoint, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartDataPointValuesFormatXExpression(m_dataPoint, m_chart.Name);
		}

		internal string EvaluateFormatY(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_dataPoint, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartDataPointValuesFormatYExpression(m_dataPoint, m_chart.Name);
		}

		internal string EvaluateFormatSize(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_dataPoint, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartDataPointValuesFormatSizeExpression(m_dataPoint, m_chart.Name);
		}

		internal string EvaluateCurrencyLanguageX(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_dataPoint, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartDataPointValuesCurrencyLanguageXExpression(m_dataPoint, m_chart.Name);
		}

		internal string EvaluateCurrencyLanguageY(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_dataPoint, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartDataPointValuesCurrencyLanguageYExpression(m_dataPoint, m_chart.Name);
		}

		internal string EvaluateCurrencyLanguageSize(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_dataPoint, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartDataPointValuesCurrencyLanguageSizeExpression(m_dataPoint, m_chart.Name);
		}
	}
}
