using System;
using System.Globalization;

namespace Microsoft.Reporting.Chart.WebForms.Formulas
{
	internal class PriceIndicators : IFormula
	{
		public virtual string Name => SR.FormulaNamePriceIndicators;

		internal void MovingAverage(double[] inputValues, out double[] outputValues, int period, bool FromFirst)
		{
			double[][] array = new double[2][];
			double[][] outputValues2 = new double[2][];
			string[] array2 = new string[1];
			string[] array3 = new string[1];
			array2[0] = period.ToString(CultureInfo.InvariantCulture);
			array3[0] = FromFirst.ToString(CultureInfo.InvariantCulture);
			array[0] = new double[inputValues.Length];
			array[1] = inputValues;
			MovingAverage(array, out outputValues2, array2, array3);
			outputValues = outputValues2[1];
		}

		private void MovingAverage(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList)
		{
			int num = inputValues.Length;
			int num2;
			try
			{
				num2 = int.Parse(parameterList[0], CultureInfo.InvariantCulture);
			}
			catch (Exception ex)
			{
				if (ex.Message == SR.ExceptionObjectReferenceIsNull)
				{
					throw new InvalidOperationException(SR.ExceptionPriceIndicatorsPeriodMissing);
				}
				throw new InvalidOperationException(SR.ExceptionPriceIndicatorsPeriodMissing + ex.Message);
			}
			if (num2 <= 0)
			{
				throw new InvalidOperationException(SR.ExceptionPeriodParameterIsNegative);
			}
			bool num3 = bool.Parse(extraParameterList[0]);
			if (num != 2)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsFormulaRequiresOneArray);
			}
			if (inputValues[0].Length != inputValues[1].Length)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsSameXYNumber);
			}
			if (inputValues[0].Length < num2)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsNotEnoughPoints);
			}
			outputValues = new double[2][];
			if (num3)
			{
				outputValues[0] = new double[inputValues[0].Length];
				outputValues[1] = new double[inputValues[1].Length];
				for (int i = 0; i < inputValues[0].Length; i++)
				{
					outputValues[0][i] = inputValues[0][i];
					double num4 = 0.0;
					int num5 = 0;
					if (i - num2 + 1 > 0)
					{
						num5 = i - num2 + 1;
					}
					for (int j = num5; j <= i; j++)
					{
						num4 += inputValues[1][j];
					}
					int num6 = num2;
					if (num2 > i + 1)
					{
						num6 = i + 1;
					}
					outputValues[1][i] = num4 / (double)num6;
				}
				return;
			}
			outputValues[0] = new double[inputValues[0].Length - num2 + 1];
			outputValues[1] = new double[inputValues[1].Length - num2 + 1];
			double num7 = 0.0;
			for (int k = 0; k < num2; k++)
			{
				num7 += inputValues[1][k];
			}
			for (int l = 0; l < outputValues[0].Length; l++)
			{
				outputValues[0][l] = inputValues[0][l + num2 - 1];
				outputValues[1][l] = num7 / (double)num2;
				if (l < outputValues[0].Length - 1)
				{
					num7 -= inputValues[1][l];
					num7 += inputValues[1][l + num2];
				}
			}
		}

		internal void ExponentialMovingAverage(double[] inputValues, out double[] outputValues, int period, bool startFromFirst)
		{
			double[][] array = new double[2][];
			double[][] outputValues2 = new double[2][];
			string[] array2 = new string[1];
			string[] array3 = new string[1];
			array2[0] = period.ToString(CultureInfo.InvariantCulture);
			array3[0] = startFromFirst.ToString(CultureInfo.InvariantCulture);
			array[0] = new double[inputValues.Length];
			array[1] = inputValues;
			ExponentialMovingAverage(array, out outputValues2, array2, array3);
			outputValues = outputValues2[1];
		}

		private void ExponentialMovingAverage(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList)
		{
			int num = inputValues.Length;
			int num2;
			try
			{
				num2 = int.Parse(parameterList[0], CultureInfo.InvariantCulture);
			}
			catch (Exception ex)
			{
				if (ex.Message == SR.ExceptionObjectReferenceIsNull)
				{
					throw new InvalidOperationException(SR.ExceptionPriceIndicatorsPeriodMissing);
				}
				throw new InvalidOperationException(SR.ExceptionPriceIndicatorsPeriodMissing + ex.Message);
			}
			if (num2 <= 0)
			{
				throw new InvalidOperationException(SR.ExceptionPeriodParameterIsNegative);
			}
			double num3 = 2.0 / ((double)num2 + 1.0);
			bool num4 = bool.Parse(extraParameterList[0]);
			if (num != 2)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsFormulaRequiresOneArray);
			}
			if (inputValues[0].Length != inputValues[1].Length)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsSameXYNumber);
			}
			if (inputValues[0].Length < num2)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsNotEnoughPoints);
			}
			outputValues = new double[2][];
			if (num4)
			{
				outputValues[0] = new double[inputValues[0].Length];
				outputValues[1] = new double[inputValues[1].Length];
				for (int i = 0; i < inputValues[0].Length; i++)
				{
					outputValues[0][i] = inputValues[0][i];
					double num5 = 0.0;
					int num6 = 0;
					if (i - num2 + 1 > 0)
					{
						num6 = i - num2 + 1;
					}
					for (int j = num6; j < i; j++)
					{
						num5 += inputValues[1][j];
					}
					int num7 = num2;
					if (num2 > i + 1)
					{
						num7 = i + 1;
					}
					double num8 = (num7 > 1) ? (num5 / (double)(num7 - 1)) : 0.0;
					num3 = 2.0 / ((double)num7 + 1.0);
					outputValues[1][i] = num8 * (1.0 - num3) + inputValues[1][i] * num3;
				}
				return;
			}
			outputValues[0] = new double[inputValues[0].Length - num2 + 1];
			outputValues[1] = new double[inputValues[1].Length - num2 + 1];
			for (int k = 0; k < outputValues[0].Length; k++)
			{
				outputValues[0][k] = inputValues[0][k + num2 - 1];
				double num10;
				if (k == 0)
				{
					double num9 = 0.0;
					for (int l = k; l < k + num2; l++)
					{
						num9 += inputValues[1][l];
					}
					num10 = num9 / (double)num2;
				}
				else
				{
					num10 = outputValues[1][k - 1];
				}
				outputValues[1][k] = num10 * (1.0 - num3) + inputValues[1][k + num2 - 1] * num3;
			}
		}

		private void TriangularMovingAverage(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList)
		{
			int num = inputValues.Length;
			int num2;
			try
			{
				num2 = int.Parse(parameterList[0], CultureInfo.InvariantCulture);
			}
			catch (Exception ex)
			{
				if (ex.Message == SR.ExceptionObjectReferenceIsNull)
				{
					throw new InvalidOperationException(SR.ExceptionPriceIndicatorsPeriodMissing);
				}
				throw new InvalidOperationException(SR.ExceptionPriceIndicatorsPeriodMissing + ex.Message);
			}
			if (num2 <= 0)
			{
				throw new InvalidOperationException(SR.ExceptionPeriodParameterIsNegative);
			}
			bool flag = bool.Parse(extraParameterList[0]);
			if (num != 2)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsFormulaRequiresOneArray);
			}
			if (inputValues[0].Length != inputValues[1].Length)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsSameXYNumber);
			}
			if (inputValues[0].Length < num2)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsNotEnoughPoints);
			}
			outputValues = new double[2][];
			double a = ((double)num2 + 1.0) / 2.0;
			a = Math.Round(a);
			double[] inputValues2 = inputValues[1];
			MovingAverage(inputValues2, out double[] outputValues2, (int)a, flag);
			MovingAverage(outputValues2, out outputValues2, (int)a, flag);
			outputValues[1] = outputValues2;
			outputValues[0] = new double[outputValues[1].Length];
			if (flag)
			{
				outputValues[0] = inputValues[0];
				return;
			}
			for (int i = 0; i < outputValues[1].Length; i++)
			{
				outputValues[0][i] = inputValues[0][((int)a - 1) * 2 + i];
			}
		}

		private void WeightedMovingAverage(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList)
		{
			int num = inputValues.Length;
			int num2;
			try
			{
				num2 = int.Parse(parameterList[0], CultureInfo.InvariantCulture);
			}
			catch (Exception ex)
			{
				if (ex.Message == SR.ExceptionObjectReferenceIsNull)
				{
					throw new InvalidOperationException(SR.ExceptionPriceIndicatorsPeriodMissing);
				}
				throw new InvalidOperationException(SR.ExceptionPriceIndicatorsPeriodMissing + ex.Message);
			}
			if (num2 <= 0)
			{
				throw new InvalidOperationException(SR.ExceptionPeriodParameterIsNegative);
			}
			_ = 2.0 / ((double)num2 + 1.0);
			bool num3 = bool.Parse(extraParameterList[0]);
			if (num != 2)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsFormulaRequiresOneArray);
			}
			if (inputValues[0].Length != inputValues[1].Length)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsSameXYNumber);
			}
			if (inputValues[0].Length < num2)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsNotEnoughPoints);
			}
			outputValues = new double[2][];
			if (num3)
			{
				outputValues[0] = new double[inputValues[0].Length];
				outputValues[1] = new double[inputValues[1].Length];
				for (int i = 0; i < inputValues[0].Length; i++)
				{
					outputValues[0][i] = inputValues[0][i];
					double num4 = 0.0;
					int num5 = 0;
					if (i - num2 + 1 > 0)
					{
						num5 = i - num2 + 1;
					}
					int num6 = 1;
					int num7 = 0;
					for (int j = num5; j <= i; j++)
					{
						num4 += inputValues[1][j] * (double)num6;
						num7 += num6;
						num6++;
					}
					double num8 = (i != 0) ? (num4 / (double)num7) : inputValues[1][0];
					outputValues[1][i] = num8;
				}
				return;
			}
			outputValues[0] = new double[inputValues[0].Length - num2 + 1];
			outputValues[1] = new double[inputValues[1].Length - num2 + 1];
			for (int k = 0; k < outputValues[0].Length; k++)
			{
				outputValues[0][k] = inputValues[0][k + num2 - 1];
				double num9 = 0.0;
				int num10 = 1;
				int num11 = 0;
				for (int l = k; l < k + num2; l++)
				{
					num9 += inputValues[1][l] * (double)num10;
					num11 += num10;
					num10++;
				}
				double num12 = num9 / (double)num11;
				outputValues[1][k] = num12;
			}
		}

		private void BollingerBands(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList)
		{
			int num = inputValues.Length;
			int num2;
			try
			{
				num2 = int.Parse(parameterList[0], CultureInfo.InvariantCulture);
			}
			catch (Exception ex)
			{
				if (ex.Message == SR.ExceptionObjectReferenceIsNull)
				{
					throw new InvalidOperationException(SR.ExceptionPriceIndicatorsPeriodMissing);
				}
				throw new InvalidOperationException(SR.ExceptionPriceIndicatorsPeriodMissing + ex.Message);
			}
			if (num2 <= 0)
			{
				throw new InvalidOperationException(SR.ExceptionPeriodParameterIsNegative);
			}
			double num3;
			try
			{
				num3 = double.Parse(parameterList[1], CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				throw new InvalidOperationException(SR.ExceptionIndicatorsDeviationMissing);
			}
			bool num4 = bool.Parse(extraParameterList[0]);
			if (num != 2)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsFormulaRequiresOneArray);
			}
			if (inputValues[0].Length != inputValues[1].Length)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsSameXYNumber);
			}
			if (inputValues[0].Length < num2)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsNotEnoughPoints);
			}
			outputValues = new double[3][];
			if (num4)
			{
				outputValues[0] = new double[inputValues[0].Length];
				outputValues[1] = new double[inputValues[1].Length];
				outputValues[2] = new double[inputValues[1].Length];
				double[] outputValues2 = new double[inputValues[1].Length];
				MovingAverage(inputValues[1], out outputValues2, num2, FromFirst: true);
				for (int i = 0; i < outputValues[0].Length; i++)
				{
					outputValues[0][i] = inputValues[0][i];
					double num5 = 0.0;
					int num6 = 0;
					if (i - num2 + 1 > 0)
					{
						num6 = i - num2 + 1;
					}
					for (int j = num6; j <= i; j++)
					{
						num5 += (inputValues[1][j] - outputValues2[i]) * (inputValues[1][j] - outputValues2[i]);
					}
					outputValues[1][i] = outputValues2[i] + Math.Sqrt(num5 / (double)num2) * num3;
					outputValues[2][i] = outputValues2[i] - Math.Sqrt(num5 / (double)num2) * num3;
				}
				return;
			}
			outputValues[0] = new double[inputValues[0].Length - num2 + 1];
			outputValues[1] = new double[inputValues[1].Length - num2 + 1];
			outputValues[2] = new double[inputValues[1].Length - num2 + 1];
			double[] outputValues3 = new double[inputValues[1].Length - num2 + 1];
			MovingAverage(inputValues[1], out outputValues3, num2, FromFirst: false);
			for (int k = 0; k < outputValues[0].Length; k++)
			{
				outputValues[0][k] = inputValues[0][k + num2 - 1];
				double num7 = 0.0;
				for (int l = k; l < k + num2; l++)
				{
					num7 += (inputValues[1][l] - outputValues3[k]) * (inputValues[1][l] - outputValues3[k]);
				}
				outputValues[1][k] = outputValues3[k] + Math.Sqrt(num7 / (double)num2) * num3;
				outputValues[2][k] = outputValues3[k] - Math.Sqrt(num7 / (double)num2) * num3;
			}
		}

		private void TypicalPrice(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList)
		{
			if (inputValues.Length != 4)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsFormulaRequiresThreeArrays);
			}
			CheckNumOfValues(inputValues, 3);
			outputValues = new double[2][];
			outputValues[0] = new double[inputValues[0].Length];
			outputValues[1] = new double[inputValues[1].Length];
			for (int i = 0; i < inputValues[1].Length; i++)
			{
				outputValues[0][i] = inputValues[0][i];
				outputValues[1][i] = (inputValues[1][i] + inputValues[2][i] + inputValues[3][i]) / 3.0;
			}
		}

		private void MedianPrice(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList)
		{
			if (inputValues.Length != 3)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsFormulaRequiresTwoArrays);
			}
			CheckNumOfValues(inputValues, 2);
			outputValues = new double[2][];
			outputValues[0] = new double[inputValues[0].Length];
			outputValues[1] = new double[inputValues[1].Length];
			for (int i = 0; i < inputValues[1].Length; i++)
			{
				outputValues[0][i] = inputValues[0][i];
				outputValues[1][i] = (inputValues[1][i] + inputValues[2][i]) / 2.0;
			}
		}

		private void WeightedClose(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList)
		{
			if (inputValues.Length != 4)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsFormulaRequiresThreeArrays);
			}
			CheckNumOfValues(inputValues, 3);
			outputValues = new double[2][];
			outputValues[0] = new double[inputValues[0].Length];
			outputValues[1] = new double[inputValues[1].Length];
			for (int i = 0; i < inputValues[1].Length; i++)
			{
				outputValues[0][i] = inputValues[0][i];
				outputValues[1][i] = (inputValues[1][i] + inputValues[2][i] + inputValues[3][i] * 2.0) / 4.0;
			}
		}

		private void Envelopes(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList)
		{
			int num = inputValues.Length;
			int num2;
			try
			{
				num2 = int.Parse(parameterList[0], CultureInfo.InvariantCulture);
			}
			catch (Exception ex)
			{
				if (ex.Message == SR.ExceptionObjectReferenceIsNull)
				{
					throw new InvalidOperationException(SR.ExceptionPriceIndicatorsPeriodMissing);
				}
				throw new InvalidOperationException(SR.ExceptionPriceIndicatorsPeriodMissing + ex.Message);
			}
			if (num2 <= 0)
			{
				throw new InvalidOperationException(SR.ExceptionPeriodParameterIsNegative);
			}
			double num3;
			try
			{
				num3 = double.Parse(parameterList[1], CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				throw new InvalidOperationException(SR.ExceptionPriceIndicatorsShiftParameterMissing);
			}
			bool.Parse(extraParameterList[0]);
			if (num != 2)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsFormulaRequiresOneArray);
			}
			if (inputValues[0].Length != inputValues[1].Length)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsSameXYNumber);
			}
			MovingAverage(inputValues, out double[][] outputValues2, parameterList, extraParameterList);
			outputValues = new double[3][];
			outputValues[0] = new double[outputValues2[0].Length];
			outputValues[1] = new double[outputValues2[0].Length];
			outputValues[2] = new double[outputValues2[0].Length];
			for (int i = 0; i < outputValues2[0].Length; i++)
			{
				outputValues[0][i] = outputValues2[0][i];
				outputValues[1][i] = outputValues2[1][i] + num3 * outputValues2[1][i] / 100.0;
				outputValues[2][i] = outputValues2[1][i] - num3 * outputValues2[1][i] / 100.0;
			}
		}

		internal void StandardDeviation(double[] inputValues, out double[] outputValues, int period, bool startFromFirst)
		{
			double[] outputValues2;
			if (startFromFirst)
			{
				outputValues = new double[inputValues.Length];
				MovingAverage(inputValues, out outputValues2, period, startFromFirst);
				int num = 0;
				for (int i = 0; i < inputValues.Length; i++)
				{
					double num2 = 0.0;
					int num3 = 0;
					if (i - period + 1 > 0)
					{
						num3 = i - period + 1;
					}
					for (int j = num3; j <= i; j++)
					{
						num2 += (inputValues[j] - outputValues2[num]) * (inputValues[j] - outputValues2[num]);
					}
					outputValues[num] = Math.Sqrt(num2 / (double)period);
					num++;
				}
				return;
			}
			outputValues = new double[inputValues.Length - period + 1];
			MovingAverage(inputValues, out outputValues2, period, startFromFirst);
			int num4 = 0;
			for (int k = period - 1; k < inputValues.Length; k++)
			{
				double num5 = 0.0;
				for (int l = k - period + 1; l <= k; l++)
				{
					num5 += (inputValues[l] - outputValues2[num4]) * (inputValues[l] - outputValues2[num4]);
				}
				outputValues[num4] = Math.Sqrt(num5 / (double)period);
				num4++;
			}
		}

		public void CheckNumOfValues(double[][] inputValues, int numOfYValues)
		{
			if (inputValues[0].Length != inputValues[1].Length)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsSameXYNumber);
			}
			int num = 1;
			while (true)
			{
				if (num < numOfYValues)
				{
					if (inputValues[num].Length != inputValues[num + 1].Length)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			throw new ArgumentException(SR.ExceptionPriceIndicatorsSameYNumber);
		}

		public virtual void Formula(string formulaName, double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList, out string[][] outLabels)
		{
			string text = formulaName.ToUpper(CultureInfo.InvariantCulture);
			outLabels = null;
			try
			{
				switch (text)
				{
				case "MOVINGAVERAGE":
					MovingAverage(inputValues, out outputValues, parameterList, extraParameterList);
					break;
				case "EXPONENTIALMOVINGAVERAGE":
					ExponentialMovingAverage(inputValues, out outputValues, parameterList, extraParameterList);
					break;
				case "TRIANGULARMOVINGAVERAGE":
					TriangularMovingAverage(inputValues, out outputValues, parameterList, extraParameterList);
					break;
				case "WEIGHTEDMOVINGAVERAGE":
					WeightedMovingAverage(inputValues, out outputValues, parameterList, extraParameterList);
					break;
				case "BOLLINGERBANDS":
					BollingerBands(inputValues, out outputValues, parameterList, extraParameterList);
					break;
				case "MEDIANPRICE":
					MedianPrice(inputValues, out outputValues, parameterList, extraParameterList);
					break;
				case "TYPICALPRICE":
					TypicalPrice(inputValues, out outputValues, parameterList, extraParameterList);
					break;
				case "WEIGHTEDCLOSE":
					WeightedClose(inputValues, out outputValues, parameterList, extraParameterList);
					break;
				case "ENVELOPES":
					Envelopes(inputValues, out outputValues, parameterList, extraParameterList);
					break;
				default:
					outputValues = null;
					break;
				}
			}
			catch (IndexOutOfRangeException)
			{
				throw new InvalidOperationException(SR.ExceptionFormulaInvalidPeriod(text));
			}
			catch (OverflowException)
			{
				throw new InvalidOperationException(SR.ExceptionFormulaNotEnoughDataPoints(text));
			}
		}
	}
}
