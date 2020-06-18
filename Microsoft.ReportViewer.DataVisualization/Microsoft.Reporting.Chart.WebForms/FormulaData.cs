using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Reporting.Chart.WebForms
{
	internal class FormulaData : ChartElement
	{
		internal const string IndexedSeriesLabelsSourceAttr = "__IndexedSeriesLabelsSource__";

		private bool ignoreEmptyPoints = true;

		private string[] extraParameters;

		internal FinancialMarkersCollection markers;

		private bool zeroXValues;

		private Statistics statistics;

		public bool IgnoreEmptyPoints
		{
			get
			{
				return ignoreEmptyPoints;
			}
			set
			{
				ignoreEmptyPoints = value;
			}
		}

		public bool StartFromFirst
		{
			get
			{
				return bool.Parse(extraParameters[0]);
			}
			set
			{
				if (value)
				{
					extraParameters[0] = true.ToString(CultureInfo.InvariantCulture);
				}
				else
				{
					extraParameters[0] = false.ToString(CultureInfo.InvariantCulture);
				}
			}
		}

		public Statistics Statistics => statistics;

		public FormulaData()
		{
			if (markers == null)
			{
				markers = new FinancialMarkersCollection();
			}
			statistics = new Statistics(this);
			extraParameters = new string[1];
			extraParameters[0] = false.ToString(CultureInfo.InvariantCulture);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void Formula(string formulaName, string parameters, string inputSeries, string outputSeries)
		{
			bool flag = false;
			double[][] output = null;
			string[][] outLabels = null;
			SplitParameters(parameters, out string[] parameterList);
			ConvertToArrays(inputSeries, out Series[] seiesArray, out int[] valueArray, inputSeries: true);
			ConvertToArrays(outputSeries, out Series[] seiesArray2, out int[] valueArray2, inputSeries: false);
			Series[] array = seiesArray2;
			foreach (Series series in array)
			{
				if (seiesArray[0] != null)
				{
					series.XValueType = seiesArray[0].XValueType;
				}
			}
			GetDoubleArray(seiesArray, valueArray, out double[][] output2);
			double[][] output3;
			if (!DifferentNumberOfSeries(output2))
			{
				RemoveEmptyValues(output2, out output3);
			}
			else
			{
				output3 = output2;
			}
			string text = null;
			for (int j = 0; j < base.Common.FormulaRegistry.Count; j++)
			{
				text = base.Common.FormulaRegistry.GetModuleName(j);
				base.Common.FormulaRegistry.GetFormulaModule(text).Formula(formulaName, output3, out output, parameterList, extraParameters, out outLabels);
				if (output != null)
				{
					if (text == SR.FormulaNameStatisticalAnalysis)
					{
						flag = true;
					}
					break;
				}
			}
			if (output == null)
			{
				throw new ArgumentException(SR.ExceptionFormulaNotFound(formulaName));
			}
			if (!flag)
			{
				InsertEmptyDataPoints(output2, output, out output);
			}
			SetDoubleArray(seiesArray2, valueArray2, output, outLabels);
			if (zeroXValues)
			{
				array = seiesArray2;
				foreach (Series series2 in array)
				{
					if (series2.Points.Count <= 0)
					{
						continue;
					}
					double xValue = series2.Points[series2.Points.Count - 1].XValue;
					base.Common.Chart.DataManipulator.InsertEmptyPoints(1.0, IntervalType.Number, 0.0, IntervalType.Number, 1.0, xValue, series2);
					foreach (DataPoint point in series2.Points)
					{
						point.XValue = 0.0;
					}
				}
			}
			CopyAxisLabels(seiesArray, seiesArray2);
		}

		private void CopyAxisLabels(Series[] inSeries, Series[] outSeries)
		{
			for (int i = 0; i < inSeries.Length && i < outSeries.Length; i++)
			{
				Series series = inSeries[i];
				Series series2 = outSeries[i];
				if (zeroXValues)
				{
					series2["__IndexedSeriesLabelsSource__"] = series.Name;
					continue;
				}
				int num = 0;
				foreach (DataPoint point in series.Points)
				{
					if (!string.IsNullOrEmpty(point.AxisLabel))
					{
						if (num < series2.Points.Count && point.XValue == series2.Points[num].XValue)
						{
							series2.Points[num].AxisLabel = point.AxisLabel;
						}
						else
						{
							num = 0;
							foreach (DataPoint point2 in series2.Points)
							{
								if (point.XValue == point2.XValue)
								{
									point2.AxisLabel = point.AxisLabel;
									break;
								}
								num++;
							}
						}
					}
					num++;
				}
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void Formula(string formulaName, Series inputSeries)
		{
			Formula(formulaName, "", inputSeries, inputSeries);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void Formula(string formulaName, Series inputSeries, Series outputSeries)
		{
			Formula(formulaName, "", inputSeries, outputSeries);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void Formula(string formulaName, string parameters, Series inputSeries, Series outputSeries)
		{
			Formula(formulaName, parameters, inputSeries.Name, outputSeries.Name);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void Formula(string formulaName, string inputSeries)
		{
			Formula(formulaName, "", inputSeries, inputSeries);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void Formula(string formulaName, string inputSeries, string outputSeries)
		{
			Formula(formulaName, "", inputSeries, outputSeries);
		}

		private void SetDoubleArray(Series[] outputSeries, int[] valueIndex, double[][] outputValues, string[][] outputLabels)
		{
			if (outputSeries.Length != valueIndex.Length)
			{
				throw new ArgumentException(SR.ExceptionFormulaDataItemsNumberMismatch);
			}
			if (outputSeries.Length < outputValues.Length - 1)
			{
				throw new ArgumentException(SR.ExceptionFormulaDataOutputSeriesNumberYValuesIncorrect);
			}
			int num = 0;
			foreach (Series series in outputSeries)
			{
				if (num + 1 > outputValues.Length - 1)
				{
					break;
				}
				if (series.Points.Count != outputValues[num].Length)
				{
					series.Points.Clear();
				}
				if (series.YValuesPerPoint < valueIndex[num])
				{
					series.YValuesPerPoint = valueIndex[num];
				}
				for (int j = 0; j < outputValues[0].Length; j++)
				{
					if (series.Points.Count != outputValues[num].Length)
					{
						series.Points.AddXY(outputValues[0][j], 0.0);
						if (outputLabels != null)
						{
							series.Points[j].Label = outputLabels[num][j];
						}
						if (double.IsNaN(outputValues[num + 1][j]))
						{
							series.Points[j].Empty = true;
						}
						else
						{
							series.Points[j].YValues[valueIndex[num] - 1] = outputValues[num + 1][j];
						}
						continue;
					}
					if (series.Points[j].XValue != outputValues[0][j] && !zeroXValues)
					{
						throw new InvalidOperationException(SR.ExceptionFormulaXValuesNotAligned);
					}
					if (double.IsNaN(outputValues[num + 1][j]))
					{
						series.Points[j].Empty = true;
						continue;
					}
					series.Points[j].YValues[valueIndex[num] - 1] = outputValues[num + 1][j];
					if (outputLabels != null)
					{
						series.Points[j].Label = outputLabels[num][j];
					}
				}
				num++;
			}
		}

		private void ConvertToArrays(string inputString, out Series[] seiesArray, out int[] valueArray, bool inputSeries)
		{
			string[] array = inputString.Split(',');
			seiesArray = new Series[array.Length];
			valueArray = new int[array.Length];
			int num = 0;
			string[] array2 = array;
			int num2 = 0;
			string text;
			while (true)
			{
				if (num2 >= array2.Length)
				{
					return;
				}
				text = array2[num2];
				string[] array3 = text.Split(':');
				if (array3.Length < 1 && array3.Length > 2)
				{
					throw new ArgumentException(SR.ExceptionFormulaDataFormatInvalid(text));
				}
				int num3 = 1;
				if (array3.Length == 2)
				{
					if (!array3[1].StartsWith("Y", StringComparison.Ordinal))
					{
						break;
					}
					array3[1] = array3[1].TrimStart('Y');
					if (array3[1].Length == 0)
					{
						num3 = 1;
					}
					else
					{
						try
						{
							num3 = int.Parse(array3[1], CultureInfo.InvariantCulture);
						}
						catch (Exception)
						{
							throw new ArgumentException(SR.ExceptionFormulaDataFormatInvalid(text));
						}
					}
				}
				valueArray[num] = num3;
				try
				{
					seiesArray[num] = base.Common.DataManager.Series[array3[0].Trim()];
				}
				catch (Exception)
				{
					if (inputSeries)
					{
						throw new ArgumentException(SR.ExceptionFormulaDataSeriesNameNotFoundInCollection(text));
					}
					base.Common.DataManager.Series.Add(array3[0]);
					seiesArray[num] = base.Common.DataManager.Series[array3[0]];
				}
				num++;
				num2++;
			}
			throw new ArgumentException(SR.ExceptionFormulaDataSeriesNameNotFound(text));
		}

		private void GetDoubleArray(Series[] inputSeries, int[] valueIndex, out double[][] output)
		{
			GetDoubleArray(inputSeries, valueIndex, out output, ignoreZeroX: false);
		}

		private void GetDoubleArray(Series[] inputSeries, int[] valueIndex, out double[][] output, bool ignoreZeroX)
		{
			output = new double[inputSeries.Length + 1][];
			if (inputSeries.Length != valueIndex.Length)
			{
				throw new ArgumentException(SR.ExceptionFormulaDataItemsNumberMismatch2);
			}
			int num = int.MinValue;
			Series series = null;
			Series[] array = inputSeries;
			foreach (Series series2 in array)
			{
				if (num < series2.Points.Count)
				{
					num = series2.Points.Count;
					series = series2;
				}
			}
			foreach (DataPoint point in inputSeries[0].Points)
			{
				zeroXValues = true;
				if (point.XValue != 0.0)
				{
					zeroXValues = false;
					break;
				}
			}
			if (zeroXValues && !ignoreZeroX)
			{
				CheckXValuesAlignment(inputSeries);
			}
			int num2 = 0;
			output[0] = new double[num];
			foreach (DataPoint point2 in series.Points)
			{
				if (zeroXValues)
				{
					output[0][num2] = (double)num2 + 1.0;
				}
				else
				{
					output[0][num2] = point2.XValue;
				}
				num2++;
			}
			int num3 = 1;
			array = inputSeries;
			foreach (Series series3 in array)
			{
				output[num3] = new double[series3.Points.Count];
				num2 = 0;
				foreach (DataPoint point3 in series3.Points)
				{
					if (point3.Empty)
					{
						output[num3][num2] = double.NaN;
					}
					else
					{
						try
						{
							output[num3][num2] = point3.YValues[valueIndex[num3 - 1] - 1];
						}
						catch (Exception)
						{
							throw new ArgumentException(SR.ExceptionFormulaYIndexInvalid);
						}
					}
					num2++;
				}
				num3++;
			}
		}

		public void CopySeriesValues(string inputSeries, string outputSeries)
		{
			ConvertToArrays(inputSeries, out Series[] seiesArray, out int[] valueArray, inputSeries: true);
			ConvertToArrays(outputSeries, out Series[] seiesArray2, out int[] valueArray2, inputSeries: false);
			if (seiesArray.Length != seiesArray2.Length)
			{
				throw new ArgumentException(SR.ExceptionFormulaInputOutputSeriesMismatch);
			}
			for (int i = 0; i < seiesArray.Length; i++)
			{
				Series[] array = new Series[2]
				{
					seiesArray[i],
					seiesArray2[i]
				};
				if (array[1].Points.Count != 0)
				{
					continue;
				}
				foreach (DataPoint point in array[0].Points)
				{
					DataPoint dataPoint = point.Clone();
					dataPoint.series = array[1];
					array[1].Points.Add(dataPoint);
				}
			}
			for (int j = 0; j < seiesArray.Length; j++)
			{
				CheckXValuesAlignment(new Series[2]
				{
					seiesArray[j],
					seiesArray2[j]
				});
			}
			GetDoubleArray(seiesArray, valueArray, out double[][] output, ignoreZeroX: true);
			double[][] array2 = new double[output.Length][];
			for (int k = 0; k < output.Length; k++)
			{
				array2[k] = new double[output[k].Length];
				for (int l = 0; l < output[k].Length; l++)
				{
					array2[k][l] = output[k][l];
				}
			}
			int num;
			for (num = 0; num < seiesArray.Length; num++)
			{
				if (seiesArray2[num].XValueType == ChartValueTypes.Auto)
				{
					seiesArray2[num].XValueType = seiesArray[num].XValueType;
					seiesArray2[num].autoXValueType = seiesArray[num].autoXValueType;
				}
				if (seiesArray2[num].YValueType == ChartValueTypes.Auto)
				{
					seiesArray2[num].YValueType = seiesArray[num].YValueType;
					seiesArray2[num].autoYValueType = seiesArray[num].autoYValueType;
				}
				num++;
			}
			SetDoubleArray(seiesArray2, valueArray2, array2, null);
		}

		private void RemoveEmptyValues(double[][] input, out double[][] output)
		{
			output = new double[input.Length][];
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < input[0].Length; i++)
			{
				bool flag = false;
				for (num = 0; num < input.Length; num++)
				{
					if (num < input[num].Length && double.IsNaN(input[num][i]))
					{
						flag = true;
					}
				}
				if (!flag)
				{
					num2++;
				}
				if (flag)
				{
					for (num = 1; num < input.Length; num++)
					{
						input[num][i] = double.NaN;
					}
				}
			}
			for (num = 0; num < input.Length; num++)
			{
				output[num] = new double[num2];
				int num3 = 0;
				for (int j = 0; j < input[0].Length; j++)
				{
					if (j < input[num].Length && !double.IsNaN(input[1][j]))
					{
						output[num][num3] = input[num][j];
						num3++;
					}
				}
			}
		}

		private void InsertEmptyDataPoints(double[][] input, double[][] inputWithoutEmpty, out double[][] output)
		{
			output = inputWithoutEmpty;
		}

		private void SplitParameters(string parameters, out string[] parameterList)
		{
			parameterList = parameters.Split(',');
			for (int i = 0; i < parameterList.Length; i++)
			{
				parameterList[i] = parameterList[i].Trim();
			}
		}

		private bool DifferentNumberOfSeries(double[][] input)
		{
			for (int i = 0; i < input.Length - 1; i++)
			{
				if (input[i].Length != input[i + 1].Length)
				{
					return true;
				}
			}
			return false;
		}

		internal void CheckXValuesAlignment(Series[] series)
		{
			if (series.Length <= 1)
			{
				return;
			}
			int num = 0;
			while (true)
			{
				if (num >= series.Length - 1)
				{
					return;
				}
				if (series[num].Points.Count != series[num + 1].Points.Count)
				{
					break;
				}
				for (int i = 0; i < series[num].Points.Count; i++)
				{
					if (series[num].Points[i].XValue != series[num + 1].Points[i].XValue)
					{
						throw new ArgumentException(SR.ExceptionFormulaDataSeriesAreNotAlignedDifferentXValues(series[num].Name, series[num + 1].Name));
					}
				}
				num++;
			}
			throw new ArgumentException(SR.ExceptionFormulaDataSeriesAreNotAlignedDifferentDataPoints(series[num].Name, series[num + 1].Name));
		}

		public void FormulaFinancial(FinancialFormula formulaName, string parameters, string inputSeries, string outputSeries)
		{
			Formula(formulaName.ToString(), parameters, inputSeries, outputSeries);
		}

		public void FormulaFinancial(FinancialFormula formulaName, Series inputSeries)
		{
			Formula(formulaName.ToString(), "", inputSeries, inputSeries);
		}

		public void FormulaFinancial(FinancialFormula formulaName, Series inputSeries, Series outputSeries)
		{
			Formula(formulaName.ToString(), "", inputSeries, outputSeries);
		}

		public void FormulaFinancial(FinancialFormula formulaName, string parameters, Series inputSeries, Series outputSeries)
		{
			Formula(formulaName.ToString(), parameters, inputSeries.Name, outputSeries.Name);
		}

		public void FormulaFinancial(FinancialFormula formulaName, string inputSeries)
		{
			Formula(formulaName.ToString(), "", inputSeries, inputSeries);
		}

		public void FormulaFinancial(FinancialFormula formulaName, string inputSeries, string outputSeries)
		{
			Formula(formulaName.ToString(), "", inputSeries, outputSeries);
		}
	}
}
