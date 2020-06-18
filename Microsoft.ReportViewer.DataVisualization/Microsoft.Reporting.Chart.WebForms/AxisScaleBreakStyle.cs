using Microsoft.Reporting.Chart.WebForms.ChartTypes;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace Microsoft.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeAxisScaleBreakStyle_AxisScaleBreakStyle")]
	[DefaultProperty("Enabled")]
	internal class AxisScaleBreakStyle
	{
		internal Axis axis;

		private bool enabled;

		private BreakLineType breakLineType = BreakLineType.Ragged;

		private double segmentSpacing = 1.5;

		private Color breakLineColor = Color.Black;

		private int breakLineWidth = 1;

		private ChartDashStyle breakLineStyle = ChartDashStyle.Solid;

		private double minSegmentSize = 10.0;

		private int totalNumberOfSegments = 100;

		private int minimumNumberOfEmptySegments = 25;

		private int maximumNumberOfBreaks = 2;

		private AutoBool startFromZero;

		[SRCategory("CategoryAttributeMisc")]
		[DefaultValue(AutoBool.Auto)]
		[SRDescription("DescriptionAttributeAxisScaleBreakStyle_StartFromZero")]
		public AutoBool StartFromZero
		{
			get
			{
				return startFromZero;
			}
			set
			{
				startFromZero = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[DefaultValue(2)]
		[SRDescription("DescriptionAttributeAxisScaleBreakStyle_MaxNumberOfBreaks")]
		public int MaxNumberOfBreaks
		{
			get
			{
				return maximumNumberOfBreaks;
			}
			set
			{
				if (value < 1 || value > 5)
				{
					throw new ArgumentOutOfRangeException("value", SR.ExceptionAxisScaleBreaksNumberInvalid);
				}
				maximumNumberOfBreaks = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[DefaultValue(25)]
		[SRDescription("DescriptionAttributeAxisScaleBreakStyle_CollapsibleSpaceThreshold")]
		public int CollapsibleSpaceThreshold
		{
			get
			{
				return minimumNumberOfEmptySegments;
			}
			set
			{
				if (value < 10 || value > 90)
				{
					throw new ArgumentOutOfRangeException("value", SR.ExceptionAxisScaleBreaksCollapsibleSpaceInvalid);
				}
				minimumNumberOfEmptySegments = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[DefaultValue(false)]
		[SRDescription("DescriptionAttributeAxisScaleBreakStyle_Enabled")]
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
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(BreakLineType.Ragged)]
		[SRDescription("DescriptionAttributeAxisScaleBreakStyle_BreakLineType")]
		public BreakLineType BreakLineType
		{
			get
			{
				return breakLineType;
			}
			set
			{
				breakLineType = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[DefaultValue(1.5)]
		[SRDescription("DescriptionAttributeAxisScaleBreakStyle_Spacing")]
		public double Spacing
		{
			get
			{
				return segmentSpacing;
			}
			set
			{
				if (value < 0.0 || value > 10.0)
				{
					throw new ArgumentOutOfRangeException("value", SR.ExceptionAxisScaleBreaksSpacingInvalid);
				}
				segmentSpacing = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(typeof(Color), "Black")]
		[SRDescription("DescriptionAttributeAxisScaleBreakStyle_LineColor")]
		public Color LineColor
		{
			get
			{
				return breakLineColor;
			}
			set
			{
				breakLineColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(1)]
		[SRDescription("DescriptionAttributeAxisScaleBreakStyle_LineWidth")]
		public int LineWidth
		{
			get
			{
				return breakLineWidth;
			}
			set
			{
				if ((double)value < 1.0 || value > 10)
				{
					throw new ArgumentOutOfRangeException("value", SR.ExceptionAxisScaleBreaksLineWidthInvalid);
				}
				breakLineWidth = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(ChartDashStyle.Solid)]
		[SRDescription("DescriptionAttributeAxisScaleBreakStyle_LineWidth")]
		public ChartDashStyle LineStyle
		{
			get
			{
				return breakLineStyle;
			}
			set
			{
				breakLineStyle = value;
				Invalidate();
			}
		}

		public AxisScaleBreakStyle()
		{
		}

		public AxisScaleBreakStyle(Axis axis)
		{
			this.axis = axis;
		}

		internal bool IsEnabled()
		{
			if (Enabled && CanUseAxisScaleBreaks())
			{
				return true;
			}
			return false;
		}

		internal bool CanUseAxisScaleBreaks()
		{
			if (axis == null || axis.chartArea == null || axis.chartArea.Common.Chart == null)
			{
				return false;
			}
			if (axis.chartArea.Area3DStyle.Enable3D)
			{
				return false;
			}
			if (axis.axisType == AxisName.X || axis.axisType == AxisName.X2)
			{
				return false;
			}
			if (axis.Logarithmic)
			{
				return false;
			}
			if (axis.View.IsZoomed)
			{
				return false;
			}
			foreach (Series item in GetAxisSeries(axis))
			{
				if (item.ChartType == SeriesChartType.Renko || item.ChartType == SeriesChartType.PointAndFigure)
				{
					return false;
				}
				IChartType chartType = axis.chartArea.Common.ChartTypeRegistry.GetChartType(item.ChartTypeName);
				if (chartType == null)
				{
					return false;
				}
				if (chartType.CircularChartArea || chartType.Stacked || !chartType.RequireAxes)
				{
					return false;
				}
			}
			return true;
		}

		internal static ArrayList GetAxisSeries(Axis axis)
		{
			ArrayList arrayList = new ArrayList();
			if (axis != null && axis.chartArea != null && axis.chartArea.Common.Chart != null)
			{
				foreach (Series item in axis.chartArea.Common.Chart.Series)
				{
					if (item.ChartArea == axis.chartArea.Name && item.Enabled && (axis.axisType != AxisName.Y || item.YAxisType != AxisType.Secondary) && (axis.axisType != AxisName.Y2 || item.YAxisType != 0))
					{
						arrayList.Add(item);
					}
				}
				return arrayList;
			}
			return arrayList;
		}

		private void Invalidate()
		{
			if (axis != null)
			{
				axis.Invalidate();
			}
		}

		internal void GetAxisSegmentForScaleBreaks(AxisScaleSegmentCollection axisSegments)
		{
			axisSegments.Clear();
			if (!IsEnabled())
			{
				return;
			}
			FillAxisSegmentCollection(axisSegments);
			if (axisSegments.Count < 1)
			{
				return;
			}
			int startScaleFromZeroSegmentIndex = GetStartScaleFromZeroSegmentIndex(axisSegments);
			int num = 0;
			foreach (AxisScaleSegment axisSegment in axisSegments)
			{
				bool shouldStartFromZero = (num == startScaleFromZeroSegmentIndex) ? true : false;
				double minimum = axisSegment.ScaleMinimum;
				double maximum = axisSegment.ScaleMaximum;
				axisSegment.Interval = axis.EstimateNumberAxis(ref minimum, ref maximum, shouldStartFromZero, axis.prefferedNumberofIntervals, axis.Crossing, autoMaximum: true, autoMinimum: true);
				axisSegment.ScaleMinimum = minimum;
				axisSegment.ScaleMaximum = maximum;
				if (axisSegment.ScaleMinimum < axis.Minimum)
				{
					axisSegment.ScaleMinimum = axis.Minimum;
				}
				if (axisSegment.ScaleMaximum > axis.Maximum)
				{
					axisSegment.ScaleMaximum = axis.Maximum;
				}
				num++;
			}
			bool flag = false;
			AxisScaleSegment axisScaleSegment2 = axisSegments[0];
			for (int i = 1; i < axisSegments.Count; i++)
			{
				AxisScaleSegment axisScaleSegment3 = axisSegments[i];
				if (axisScaleSegment3.ScaleMinimum <= axisScaleSegment2.ScaleMaximum)
				{
					if (axisScaleSegment3.ScaleMaximum > axisScaleSegment2.ScaleMaximum)
					{
						axisScaleSegment2.ScaleMaximum = axisScaleSegment3.ScaleMaximum;
					}
					flag = true;
					axisSegments.RemoveAt(i);
					i--;
				}
				else
				{
					axisScaleSegment2 = axisScaleSegment3;
				}
			}
			if (flag)
			{
				SetAxisSegmentPosition(axisSegments);
			}
		}

		private int GetStartScaleFromZeroSegmentIndex(AxisScaleSegmentCollection axisSegments)
		{
			if (StartFromZero == AutoBool.Auto || StartFromZero == AutoBool.True)
			{
				int num = 0;
				foreach (AxisScaleSegment axisSegment in axisSegments)
				{
					if (axisSegment.ScaleMinimum < 0.0 && axisSegment.ScaleMaximum > 0.0)
					{
						return -1;
					}
					if (axisSegment.ScaleMinimum > 0.0 || num == axisSegments.Count - 1)
					{
						if (StartFromZero == AutoBool.Auto && axisSegment.ScaleMinimum > 2.0 * (axisSegment.ScaleMaximum - axisSegment.ScaleMinimum))
						{
							return -1;
						}
						return num;
					}
					num++;
				}
			}
			return -1;
		}

		private void SetAxisSegmentPosition(AxisScaleSegmentCollection axisSegments)
		{
			int num = 0;
			foreach (AxisScaleSegment axisSegment in axisSegments)
			{
				if (axisSegment.Tag is int)
				{
					num += (int)axisSegment.Tag;
				}
			}
			double num2 = Math.Min(minSegmentSize, Math.Floor(100.0 / (double)axisSegments.Count));
			double num3 = 0.0;
			for (int i = 0; i < axisSegments.Count; i++)
			{
				axisSegments[i].Position = ((num3 > 100.0) ? 100.0 : num3);
				axisSegments[i].Size = Math.Round((double)(int)axisSegments[i].Tag / ((double)num / 100.0), 5);
				if (axisSegments[i].Size < num2)
				{
					axisSegments[i].Size = num2;
				}
				if (i < axisSegments.Count - 1)
				{
					axisSegments[i].Spacing = segmentSpacing;
				}
				num3 += axisSegments[i].Size;
			}
			double num4 = 0.0;
			do
			{
				num4 = 0.0;
				double num5 = double.MinValue;
				int num6 = -1;
				for (int j = 0; j < axisSegments.Count; j++)
				{
					num4 += axisSegments[j].Size;
					if (axisSegments[j].Size > num5)
					{
						num5 = axisSegments[j].Size;
						num6 = j;
					}
				}
				if (num4 > 100.0)
				{
					axisSegments[num6].Size -= num4 - 100.0;
					if (axisSegments[num6].Size < num2)
					{
						axisSegments[num6].Size = num2;
					}
					double num7 = axisSegments[num6].Position + axisSegments[num6].Size;
					for (int k = num6 + 1; k < axisSegments.Count; k++)
					{
						axisSegments[k].Position = num7;
						num7 += axisSegments[k].Size;
					}
				}
			}
			while (num4 > 100.0);
		}

		private void FillAxisSegmentCollection(AxisScaleSegmentCollection axisSegments)
		{
			axisSegments.Clear();
			double minYValue = 0.0;
			double maxYValue = 0.0;
			double segmentSize = 0.0;
			double[] segmentMaxValue = null;
			double[] segmentMinValue = null;
			int[] seriesDataStatistics = GetSeriesDataStatistics(totalNumberOfSegments, out minYValue, out maxYValue, out segmentSize, out segmentMaxValue, out segmentMinValue);
			if (seriesDataStatistics == null)
			{
				return;
			}
			double minimum = minYValue;
			double maximum = maxYValue;
			axis.EstimateNumberAxis(ref minimum, ref maximum, axis.StartFromZero, axis.prefferedNumberofIntervals, axis.Crossing, autoMaximum: true, autoMinimum: true);
			if (maxYValue == minYValue)
			{
				return;
			}
			double num = (maxYValue - minYValue) / ((maximum - minimum) / 100.0);
			ArrayList arrayList = new ArrayList();
			bool flag = false;
			while (!flag)
			{
				flag = true;
				int startSegment = 0;
				int numberOfSegments = 0;
				GetLargestSequenseOfSegmentsWithNoPoints(seriesDataStatistics, out startSegment, out numberOfSegments);
				int num2 = (int)((double)minimumNumberOfEmptySegments * (100.0 / num));
				if (axisSegments.Count > 0 && numberOfSegments > 0)
				{
					foreach (AxisScaleSegment axisSegment in axisSegments)
					{
						if (startSegment > 0 && startSegment + numberOfSegments <= segmentMaxValue.Length - 1 && segmentMaxValue[startSegment - 1] >= axisSegment.ScaleMinimum && segmentMinValue[startSegment + numberOfSegments] <= axisSegment.ScaleMaximum)
						{
							double num3 = axisSegment.ScaleMaximum - axisSegment.ScaleMinimum;
							if ((segmentMinValue[startSegment + numberOfSegments] - segmentMaxValue[startSegment - 1]) / (num3 / 100.0) / 100.0 * axisSegment.Size > (double)num2 && (double)numberOfSegments > minSegmentSize)
							{
								num2 = numberOfSegments;
							}
						}
					}
				}
				if (numberOfSegments >= num2)
				{
					flag = false;
					arrayList.Add(startSegment);
					arrayList.Add(numberOfSegments);
					axisSegments.Clear();
					if (arrayList.Count > 0)
					{
						double num4 = double.NaN;
						double num5 = double.NaN;
						int num6 = 0;
						for (int i = 0; i < seriesDataStatistics.Length; i++)
						{
							bool flag2 = IsExcludedSegment(arrayList, i);
							if (!flag2 && !double.IsNaN(segmentMinValue[i]) && !double.IsNaN(segmentMaxValue[i]))
							{
								num6 += seriesDataStatistics[i];
								if (double.IsNaN(num4))
								{
									num4 = segmentMinValue[i];
									num5 = segmentMaxValue[i];
								}
								else
								{
									num5 = segmentMaxValue[i];
								}
							}
							if (!double.IsNaN(num4) && (flag2 || i == seriesDataStatistics.Length - 1))
							{
								if (num5 == num4)
								{
									num4 -= segmentSize;
									num5 += segmentSize;
								}
								AxisScaleSegment axisScaleSegment2 = new AxisScaleSegment();
								axisScaleSegment2.ScaleMaximum = num5;
								axisScaleSegment2.ScaleMinimum = num4;
								axisScaleSegment2.Tag = num6;
								axisSegments.Add(axisScaleSegment2);
								num4 = double.NaN;
								num5 = double.NaN;
								num6 = 0;
							}
						}
					}
					SetAxisSegmentPosition(axisSegments);
				}
				if (axisSegments.Count - 1 >= maximumNumberOfBreaks)
				{
					flag = true;
				}
			}
		}

		private bool IsExcludedSegment(ArrayList excludedSegments, int segmentIndex)
		{
			for (int i = 0; i < excludedSegments.Count; i += 2)
			{
				if (segmentIndex >= (int)excludedSegments[i] && segmentIndex < (int)excludedSegments[i] + (int)excludedSegments[i + 1])
				{
					return true;
				}
			}
			return false;
		}

		internal int[] GetSeriesDataStatistics(int segmentCount, out double minYValue, out double maxYValue, out double segmentSize, out double[] segmentMaxValue, out double[] segmentMinValue)
		{
			ArrayList axisSeries = GetAxisSeries(axis);
			minYValue = 0.0;
			maxYValue = 0.0;
			axis.Common.DataManager.GetMinMaxYValue(axisSeries, out minYValue, out maxYValue);
			if (axisSeries.Count == 0)
			{
				segmentSize = 0.0;
				segmentMaxValue = null;
				segmentMinValue = null;
				return null;
			}
			segmentSize = (maxYValue - minYValue) / (double)segmentCount;
			int[] array = new int[segmentCount];
			segmentMaxValue = new double[segmentCount];
			segmentMinValue = new double[segmentCount];
			for (int i = 0; i < segmentCount; i++)
			{
				segmentMaxValue[i] = double.NaN;
				segmentMinValue[i] = double.NaN;
			}
			foreach (Series item in axisSeries)
			{
				int num = 1;
				IChartType chartType = axis.chartArea.Common.ChartTypeRegistry.GetChartType(item.ChartTypeName);
				if (chartType != null && chartType.ExtraYValuesConnectedToYAxis && chartType.YValuesPerPoint > 1)
				{
					num = chartType.YValuesPerPoint;
				}
				foreach (DataPoint point in item.Points)
				{
					if (point.Empty)
					{
						continue;
					}
					for (int j = 0; j < num; j++)
					{
						int num2 = (int)Math.Floor((point.YValues[j] - minYValue) / segmentSize);
						if (num2 < 0)
						{
							num2 = 0;
						}
						if (num2 > segmentCount - 1)
						{
							num2 = segmentCount - 1;
						}
						array[num2]++;
						if (array[num2] == 1)
						{
							segmentMaxValue[num2] = point.YValues[j];
							segmentMinValue[num2] = point.YValues[j];
						}
						else
						{
							segmentMaxValue[num2] = Math.Max(segmentMaxValue[num2], point.YValues[j]);
							segmentMinValue[num2] = Math.Min(segmentMinValue[num2], point.YValues[j]);
						}
					}
				}
			}
			return array;
		}

		internal bool GetLargestSequenseOfSegmentsWithNoPoints(int[] segmentPointNumber, out int startSegment, out int numberOfSegments)
		{
			startSegment = -1;
			numberOfSegments = 0;
			int num = -1;
			int num2 = -1;
			for (int i = 0; i < segmentPointNumber.Length; i++)
			{
				if (segmentPointNumber[i] == 0)
				{
					if (num == -1)
					{
						num = i;
						num2 = 1;
					}
					else
					{
						num2++;
					}
				}
				if (num2 > 0 && (segmentPointNumber[i] != 0 || i == segmentPointNumber.Length - 1))
				{
					if (num2 > numberOfSegments)
					{
						startSegment = num;
						numberOfSegments = num2;
					}
					num = -1;
					num2 = 0;
				}
			}
			if (numberOfSegments != 0)
			{
				for (int j = startSegment; j < startSegment + numberOfSegments; j++)
				{
					segmentPointNumber[j] = -1;
				}
				return true;
			}
			return false;
		}
	}
}
