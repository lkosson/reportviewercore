using System;
using System.Globalization;

namespace Microsoft.Reporting.Chart.WebForms.Formulas
{
	internal class GeneralTechnicalIndicators : PriceIndicators
	{
		public override string Name => SR.FormulaNameGeneralTechnicalIndicators;

		private void StandardDeviation(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList)
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
			StandardDeviation(inputValues[1], out outputValues[1], num2, flag);
			outputValues[0] = new double[outputValues[1].Length];
			for (int i = 0; i < outputValues[1].Length; i++)
			{
				if (flag)
				{
					outputValues[0][i] = inputValues[0][i];
				}
				else
				{
					outputValues[0][i] = inputValues[0][i + num2 - 1];
				}
			}
		}

		private void AverageTrueRange(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList)
		{
			if (inputValues.Length != 4)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsFormulaRequiresThreeArrays);
			}
			CheckNumOfValues(inputValues, 3);
			int num;
			try
			{
				num = int.Parse(parameterList[0], CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				num = 14;
			}
			if (num <= 0)
			{
				throw new InvalidOperationException(SR.ExceptionPeriodParameterIsNegative);
			}
			double[] array = new double[inputValues[0].Length - 1];
			for (int i = 1; i < inputValues[0].Length; i++)
			{
				double val = Math.Abs(inputValues[1][i] - inputValues[2][i]);
				double val2 = Math.Abs(inputValues[3][i - 1] - inputValues[1][i]);
				double val3 = Math.Abs(inputValues[3][i - 1] - inputValues[2][i]);
				array[i - 1] = Math.Max(Math.Max(val, val2), val3);
			}
			outputValues = new double[2][];
			outputValues[0] = new double[inputValues[0].Length - num];
			MovingAverage(array, out outputValues[1], num, FromFirst: false);
			for (int j = num; j < inputValues[0].Length; j++)
			{
				outputValues[0][j - num] = inputValues[0][j];
			}
		}

		private void EaseOfMovement(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList)
		{
			if (inputValues.Length != 4)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsFormulaRequiresThreeArrays);
			}
			CheckNumOfValues(inputValues, 3);
			outputValues = new double[2][];
			outputValues[0] = new double[inputValues[0].Length - 1];
			outputValues[1] = new double[inputValues[0].Length - 1];
			for (int i = 1; i < inputValues[0].Length; i++)
			{
				outputValues[0][i - 1] = inputValues[0][i];
				double num = (inputValues[1][i] + inputValues[2][i]) / 2.0 - (inputValues[1][i - 1] + inputValues[2][i - 1]) / 2.0;
				double num2 = inputValues[3][i] / (inputValues[1][i] - inputValues[2][i]);
				outputValues[1][i - 1] = num / num2;
			}
		}

		private void MassIndex(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList)
		{
			if (inputValues.Length != 3)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsFormulaRequiresTwoArrays);
			}
			CheckNumOfValues(inputValues, 2);
			int num;
			try
			{
				num = int.Parse(parameterList[0], CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				num = 25;
			}
			if (num <= 0)
			{
				throw new InvalidOperationException(SR.ExceptionPeriodParameterIsNegative);
			}
			int num2;
			try
			{
				num2 = int.Parse(parameterList[1], CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				num2 = 9;
			}
			if (num <= 0)
			{
				throw new InvalidOperationException(SR.ExceptionPeriodAverageParameterIsNegative);
			}
			double[] array = new double[inputValues[0].Length];
			for (int i = 0; i < inputValues[0].Length; i++)
			{
				array[i] = inputValues[1][i] - inputValues[2][i];
			}
			ExponentialMovingAverage(array, out double[] outputValues2, num2, startFromFirst: false);
			ExponentialMovingAverage(outputValues2, out double[] outputValues3, num2, startFromFirst: false);
			outputValues = new double[2][];
			outputValues[0] = new double[outputValues3.Length - num + 1];
			outputValues[1] = new double[outputValues3.Length - num + 1];
			int num3 = 0;
			double num4 = 0.0;
			for (int j = 2 * num2 - 3 + num; j < inputValues[0].Length; j++)
			{
				outputValues[0][num3] = inputValues[0][j];
				num4 = 0.0;
				for (int k = j - num + 1; k <= j; k++)
				{
					num4 += outputValues2[k - num2 + 1] / outputValues3[k - 2 * num2 + 2];
				}
				outputValues[1][num3] = num4;
				num3++;
			}
		}

		private void Performance(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList)
		{
			if (inputValues.Length != 2)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsFormulaRequiresOneArray);
			}
			CheckNumOfValues(inputValues, 1);
			outputValues = new double[2][];
			outputValues[0] = new double[inputValues[0].Length];
			outputValues[1] = new double[inputValues[0].Length];
			for (int i = 0; i < inputValues[0].Length; i++)
			{
				outputValues[0][i] = inputValues[0][i];
				outputValues[1][i] = (inputValues[1][i] - inputValues[1][0]) / inputValues[1][0] * 100.0;
			}
		}

		private void RateOfChange(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList)
		{
			if (inputValues.Length != 2)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsFormulaRequiresOneArray);
			}
			CheckNumOfValues(inputValues, 1);
			int num;
			try
			{
				num = int.Parse(parameterList[0], CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				num = 10;
			}
			if (num <= 0)
			{
				throw new InvalidOperationException(SR.ExceptionPeriodParameterIsNegative);
			}
			outputValues = new double[2][];
			outputValues[0] = new double[inputValues[0].Length - num];
			outputValues[1] = new double[inputValues[0].Length - num];
			for (int i = num; i < inputValues[0].Length; i++)
			{
				outputValues[0][i - num] = inputValues[0][i];
				outputValues[1][i - num] = (inputValues[1][i] - inputValues[1][i - num]) / inputValues[1][i - num] * 100.0;
			}
		}

		private void RelativeStrengthIndex(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList)
		{
			if (inputValues.Length != 2)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsFormulaRequiresOneArray);
			}
			CheckNumOfValues(inputValues, 1);
			int num;
			try
			{
				num = int.Parse(parameterList[0], CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				num = 10;
			}
			if (num <= 0)
			{
				throw new InvalidOperationException(SR.ExceptionPeriodParameterIsNegative);
			}
			double[] array = new double[inputValues[0].Length - 1];
			double[] array2 = new double[inputValues[0].Length - 1];
			for (int i = 1; i < inputValues[0].Length; i++)
			{
				if (inputValues[1][i - 1] < inputValues[1][i])
				{
					array[i - 1] = inputValues[1][i] - inputValues[1][i - 1];
					array2[i - 1] = 0.0;
				}
				if (inputValues[1][i - 1] > inputValues[1][i])
				{
					array[i - 1] = 0.0;
					array2[i - 1] = inputValues[1][i - 1] - inputValues[1][i];
				}
			}
			double[] outputValues2 = new double[inputValues[0].Length];
			double[] outputValues3 = new double[inputValues[0].Length];
			ExponentialMovingAverage(array2, out outputValues3, num, startFromFirst: false);
			ExponentialMovingAverage(array, out outputValues2, num, startFromFirst: false);
			outputValues = new double[2][];
			outputValues[0] = new double[outputValues3.Length];
			outputValues[1] = new double[outputValues3.Length];
			for (int j = 0; j < outputValues3.Length; j++)
			{
				outputValues[0][j] = inputValues[0][j + num];
				outputValues[1][j] = 100.0 - 100.0 / (1.0 + outputValues2[j] / outputValues3[j]);
			}
		}

		private void Trix(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList)
		{
			if (inputValues.Length != 2)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsFormulaRequiresOneArray);
			}
			CheckNumOfValues(inputValues, 1);
			int num;
			try
			{
				num = int.Parse(parameterList[0], CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				num = 12;
			}
			if (num <= 0)
			{
				throw new InvalidOperationException(SR.ExceptionPeriodParameterIsNegative);
			}
			ExponentialMovingAverage(inputValues[1], out double[] outputValues2, num, startFromFirst: false);
			ExponentialMovingAverage(outputValues2, out double[] outputValues3, num, startFromFirst: false);
			ExponentialMovingAverage(outputValues3, out double[] outputValues4, num, startFromFirst: false);
			outputValues = new double[2][];
			outputValues[0] = new double[inputValues[0].Length - num * 3 + 2];
			outputValues[1] = new double[inputValues[0].Length - num * 3 + 2];
			int num2 = 0;
			for (int i = num * 3 - 2; i < inputValues[0].Length; i++)
			{
				outputValues[0][num2] = inputValues[0][i];
				outputValues[1][num2] = (outputValues4[num2 + 1] - outputValues4[num2]) / outputValues4[num2];
				num2++;
			}
		}

		private void Macd(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList)
		{
			if (inputValues.Length != 2)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsFormulaRequiresOneArray);
			}
			CheckNumOfValues(inputValues, 1);
			int num;
			try
			{
				num = int.Parse(parameterList[0], CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				num = 12;
			}
			if (num <= 0)
			{
				throw new InvalidOperationException(SR.ExceptionPeriodShortParameterIsNegative);
			}
			int num2;
			try
			{
				num2 = int.Parse(parameterList[1], CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				num2 = 26;
			}
			if (num2 <= 0)
			{
				throw new InvalidOperationException(SR.ExceptionPeriodLongParameterIsNegative);
			}
			if (num2 <= num)
			{
				throw new InvalidOperationException(SR.ExceptionIndicatorsLongPeriodLessThenShortPeriod);
			}
			ExponentialMovingAverage(inputValues[1], out double[] outputValues2, num2, startFromFirst: false);
			ExponentialMovingAverage(inputValues[1], out double[] outputValues3, num, startFromFirst: false);
			outputValues = new double[2][];
			outputValues[0] = new double[inputValues[0].Length - num2 + 1];
			outputValues[1] = new double[inputValues[0].Length - num2 + 1];
			int num3 = 0;
			for (int i = num2 - 1; i < inputValues[0].Length; i++)
			{
				outputValues[0][num3] = inputValues[0][i];
				outputValues[1][num3] = outputValues3[num3 + num2 - num] - outputValues2[num3];
				num3++;
			}
		}

		private void CommodityChannelIndex(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList)
		{
			if (inputValues.Length != 4)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsFormulaRequiresThreeArrays);
			}
			CheckNumOfValues(inputValues, 3);
			int num;
			try
			{
				num = int.Parse(parameterList[0], CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				num = 10;
			}
			if (num <= 0)
			{
				throw new InvalidOperationException(SR.ExceptionPeriodParameterIsNegative);
			}
			double[] array = new double[inputValues[0].Length];
			for (int i = 0; i < inputValues[0].Length; i++)
			{
				array[i] = (inputValues[1][i] + inputValues[2][i] + inputValues[3][i]) / 3.0;
			}
			MovingAverage(array, out double[] outputValues2, num, FromFirst: false);
			double[] array2 = new double[outputValues2.Length];
			double num2 = 0.0;
			for (int j = 0; j < outputValues2.Length; j++)
			{
				num2 = 0.0;
				for (int k = j; k < j + num; k++)
				{
					num2 += Math.Abs(outputValues2[j] - array[k]);
				}
				array2[j] = num2 / (double)num;
			}
			outputValues = new double[2][];
			outputValues[0] = new double[array2.Length];
			outputValues[1] = new double[array2.Length];
			for (int l = 0; l < array2.Length; l++)
			{
				outputValues[0][l] = inputValues[0][l + num - 1];
				outputValues[1][l] = (array[l + num - 1] - outputValues2[l]) / (0.015 * array2[l]);
			}
		}

		public override void Formula(string formulaName, double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList, out string[][] outLabels)
		{
			outputValues = null;
			string text = formulaName.ToUpper(CultureInfo.InvariantCulture);
			outLabels = null;
			try
			{
				switch (text)
				{
				case "STANDARDDEVIATION":
					StandardDeviation(inputValues, out outputValues, parameterList, extraParameterList);
					break;
				case "AVERAGETRUERANGE":
					AverageTrueRange(inputValues, out outputValues, parameterList, extraParameterList);
					break;
				case "EASEOFMOVEMENT":
					EaseOfMovement(inputValues, out outputValues, parameterList, extraParameterList);
					break;
				case "MASSINDEX":
					MassIndex(inputValues, out outputValues, parameterList, extraParameterList);
					break;
				case "PERFORMANCE":
					Performance(inputValues, out outputValues, parameterList, extraParameterList);
					break;
				case "RATEOFCHANGE":
					RateOfChange(inputValues, out outputValues, parameterList, extraParameterList);
					break;
				case "RELATIVESTRENGTHINDEX":
					RelativeStrengthIndex(inputValues, out outputValues, parameterList, extraParameterList);
					break;
				case "TRIX":
					Trix(inputValues, out outputValues, parameterList, extraParameterList);
					break;
				case "MACD":
					Macd(inputValues, out outputValues, parameterList, extraParameterList);
					break;
				case "COMMODITYCHANNELINDEX":
					CommodityChannelIndex(inputValues, out outputValues, parameterList, extraParameterList);
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
