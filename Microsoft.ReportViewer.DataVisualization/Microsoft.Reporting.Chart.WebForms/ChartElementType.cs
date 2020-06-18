using System;
using System.ComponentModel;

namespace Microsoft.Reporting.Chart.WebForms
{
	internal enum ChartElementType
	{
		Nothing,
		DataPoint,
		Axis,
		PlottingArea,
		LegendArea,
		LegendItem,
		Gridlines,
		StripLines,
		TickMarks,
		Title,
		AxisLabels,
		AxisTitle,
		SBThumbTracker,
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		SBSmallDecrement,
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		SBSmallIncrement,
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		SBLargeDecrement,
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		SBLargeIncrement,
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This item is not supported in web environment")]
		SBZoomReset,
		Annotation,
		DataPointLabel,
		AxisLabelImage,
		LegendTitle,
		LegendHeader
	}
}
