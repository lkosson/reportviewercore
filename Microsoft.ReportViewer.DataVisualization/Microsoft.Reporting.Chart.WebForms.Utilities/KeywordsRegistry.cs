using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace Microsoft.Reporting.Chart.WebForms.Utilities
{
	internal class KeywordsRegistry : IServiceProvider
	{
		private IServiceContainer serviceContainer;

		internal ArrayList registeredKeywords = new ArrayList();

		private KeywordsRegistry()
		{
		}

		public KeywordsRegistry(IServiceContainer container)
		{
			if (container == null)
			{
				throw new ArgumentNullException(SR.ExceptionInvalidServiceContainer);
			}
			serviceContainer = container;
			RegisterKeywords();
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public object GetService(Type serviceType)
		{
			if (serviceType == typeof(KeywordsRegistry))
			{
				return this;
			}
			throw new ArgumentException(SR.ExceptionKeywordsRegistryUnsupportedType(serviceType.ToString()));
		}

		private void RegisterKeywords()
		{
			string appliesToProperties = "Text,Label,ToolTip,Href,LabelToolTip,MapAreaAttributes,AxisLabel,LegendToolTip,LegendMapAreaAttributes,LegendHref,LegendText";
			Register(SR.DescriptionKeyWordNameIndexDataPoint, "#INDEX", string.Empty, SR.DescriptionKeyWordIndexDataPoint2, "DataPoint", appliesToProperties, supportsFormatting: false, supportsValueIndex: false);
			Register(SR.DescriptionKeyWordNameXValue, "#VALX", string.Empty, SR.DescriptionKeyWordXValue, "Series,DataPoint,Annotation,LegendCellColumn", appliesToProperties, supportsFormatting: true, supportsValueIndex: false);
			Register(SR.DescriptionKeyWordNameYValue, "#VAL", string.Empty, SR.DescriptionKeyWordYValue, "Series,DataPoint,Annotation,LegendCellColumn,LegendCellColumn", appliesToProperties, supportsFormatting: true, supportsValueIndex: true);
			Register(SR.DescriptionKeyWordNameTotalYValues, "#TOTAL", string.Empty, SR.DescriptionKeyWordTotalYValues, "Series,DataPoint,Annotation,LegendCellColumn", appliesToProperties, supportsFormatting: true, supportsValueIndex: false);
			Register(SR.DescriptionKeyWordNameYValuePercentTotal, "#PERCENT", string.Empty, SR.DescriptionKeyWordYValuePercentTotal, "Series,DataPoint,Annotation,LegendCellColumn", appliesToProperties, supportsFormatting: true, supportsValueIndex: true);
			Register(SR.DescriptionKeyWordNameIndexTheDataPoint, "#INDEX", string.Empty, SR.DescriptionKeyWordIndexDataPoint, "Series,DataPoint,Annotation,LegendCellColumn", appliesToProperties, supportsFormatting: false, supportsValueIndex: false);
			Register(SR.DescriptionKeyWordNameLabelDataPoint, "#LABEL", string.Empty, SR.DescriptionKeyWordLabelDataPoint, "Series,DataPoint,Annotation,LegendCellColumn", appliesToProperties, supportsFormatting: false, supportsValueIndex: false);
			Register(SR.DescriptionKeyWordNameAxisLabelDataPoint, "#AXISLABEL", string.Empty, SR.DescriptionKeyWordAxisLabelDataPoint, "Series,DataPoint,Annotation,LegendCellColumn", appliesToProperties, supportsFormatting: false, supportsValueIndex: false);
			Register(SR.DescriptionKeyWordNameLegendText, "#LEGENDTEXT", string.Empty, SR.DescriptionKeyWordLegendText, "Series,DataPoint,Annotation,LegendCellColumn", appliesToProperties, supportsFormatting: false, supportsValueIndex: false);
			Register(SR.DescriptionKeyWordNameSeriesName, "#SERIESNAME", "#SER", SR.DescriptionKeyWordSeriesName, "Series,DataPoint,Annotation,LegendCellColumn", appliesToProperties, supportsFormatting: false, supportsValueIndex: false);
			Register(SR.DescriptionKeyWordNameAverageYValues, "#AVG", string.Empty, SR.DescriptionKeyWordAverageYValues, "Series,DataPoint,Annotation,LegendCellColumn", appliesToProperties, supportsFormatting: true, supportsValueIndex: true);
			Register(SR.DescriptionKeyWordNameMaximumYValues, "#MAX", string.Empty, SR.DescriptionKeyWordMaximumYValues, "Series,DataPoint,Annotation,LegendCellColumn", appliesToProperties, supportsFormatting: true, supportsValueIndex: true);
			Register(SR.DescriptionKeyWordNameMinimumYValues, "#MIN", string.Empty, SR.DescriptionKeyWordMinimumYValues, "Series,DataPoint,Annotation,LegendCellColumn", appliesToProperties, supportsFormatting: true, supportsValueIndex: true);
			Register(SR.DescriptionKeyWordNameLastPointYValue, "#LAST", string.Empty, SR.DescriptionKeyWordLastPointYValue, "Series,DataPoint,Annotation,LegendCellColumn", appliesToProperties, supportsFormatting: true, supportsValueIndex: true);
			Register(SR.DescriptionKeyWordNameFirstPointYValue, "#FIRST", string.Empty, SR.DescriptionKeyWordFirstPointYValue, "Series,DataPoint,Annotation,LegendCellColumn", appliesToProperties, supportsFormatting: true, supportsValueIndex: true);
		}

		public void Register(string name, string keyword, string keywordAliases, string description, string appliesToTypes, string appliesToProperties, bool supportsFormatting, bool supportsValueIndex)
		{
			KeywordInfo value = new KeywordInfo(name, keyword, keywordAliases, description, appliesToTypes, appliesToProperties, supportsFormatting, supportsValueIndex);
			registeredKeywords.Add(value);
		}
	}
}
