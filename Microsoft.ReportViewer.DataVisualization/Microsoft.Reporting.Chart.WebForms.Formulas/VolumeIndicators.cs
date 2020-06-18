using System;
using System.Globalization;

namespace Microsoft.Reporting.Chart.WebForms.Formulas
{
	internal class VolumeIndicators : PriceIndicators
	{
		public override string Name => SR.FormulaNameVolumeIndicators;

		public override void Formula(string formulaName, double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList, out string[][] outLabels)
		{
			string text = formulaName.ToUpper(CultureInfo.InvariantCulture);
			outLabels = null;
			try
			{
				switch (text)
				{
				case "MONEYFLOW":
					MoneyFlow(inputValues, out outputValues, parameterList, extraParameterList);
					break;
				case "ONBALANCEVOLUME":
					OnBalanceVolume(inputValues, out outputValues, parameterList, extraParameterList);
					break;
				case "NEGATIVEVOLUMEINDEX":
					NegativeVolumeIndex(inputValues, out outputValues, parameterList, extraParameterList);
					break;
				case "POSITIVEVOLUMEINDEX":
					PositiveVolumeIndex(inputValues, out outputValues, parameterList, extraParameterList);
					break;
				case "PRICEVOLUMETREND":
					PriceVolumeTrend(inputValues, out outputValues, parameterList, extraParameterList);
					break;
				case "ACCUMULATIONDISTRIBUTION":
					AccumulationDistribution(inputValues, out outputValues, parameterList, extraParameterList);
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

		private void MoneyFlow(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList)
		{
			if (inputValues.Length != 5)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsFormulaRequiresFourArrays);
			}
			CheckNumOfValues(inputValues, 4);
			int num;
			try
			{
				num = int.Parse(parameterList[0], CultureInfo.InvariantCulture);
			}
			catch (Exception ex)
			{
				if (ex.Message == SR.ExceptionObjectReferenceIsNull)
				{
					throw new InvalidOperationException(SR.ExceptionPriceIndicatorsPeriodMissing);
				}
				throw new InvalidOperationException(SR.ExceptionPriceIndicatorsPeriodMissing + ex.Message);
			}
			if (num <= 0)
			{
				throw new InvalidOperationException(SR.ExceptionPeriodParameterIsNegative);
			}
			if (inputValues[0].Length < num)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsNotEnoughPoints);
			}
			outputValues = new double[2][];
			outputValues[0] = new double[inputValues[0].Length - num + 1];
			outputValues[1] = new double[inputValues[0].Length - num + 1];
			double[] array = new double[inputValues[1].Length];
			double[] array2 = new double[inputValues[1].Length];
			double[] array3 = new double[inputValues[1].Length];
			double[] array4 = new double[inputValues[1].Length];
			for (int i = 0; i < inputValues[1].Length; i++)
			{
				array[i] = (inputValues[1][i] + inputValues[2][i] + inputValues[3][i]) / 3.0;
				array2[i] = (inputValues[1][i] + inputValues[2][i] + inputValues[3][i]) / 3.0 * inputValues[4][i];
			}
			for (int j = 1; j < inputValues[1].Length; j++)
			{
				if (array[j] > array[j - 1])
				{
					array3[j] = array2[j];
					array4[j] = 0.0;
				}
				if (array[j] < array[j - 1])
				{
					array4[j] = array2[j];
					array3[j] = 0.0;
				}
			}
			double num2 = 0.0;
			double num3 = 0.0;
			for (int k = num - 1; k < inputValues[1].Length; k++)
			{
				num2 = 0.0;
				num3 = 0.0;
				for (int l = k - num + 1; l <= k; l++)
				{
					num3 += array4[l];
					num2 += array3[l];
				}
				outputValues[0][k - num + 1] = inputValues[0][k];
				outputValues[1][k - num + 1] = 100.0 - 100.0 / (1.0 + num2 / num3);
			}
		}

		private void PriceVolumeTrend(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList)
		{
			if (inputValues.Length != 3)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsFormulaRequiresTwoArrays);
			}
			CheckNumOfValues(inputValues, 2);
			outputValues = new double[2][];
			outputValues[0] = new double[inputValues[0].Length];
			outputValues[1] = new double[inputValues[0].Length];
			outputValues[0][0] = inputValues[0][0];
			outputValues[1][0] = 0.0;
			for (int i = 1; i < inputValues[1].Length; i++)
			{
				outputValues[0][i] = inputValues[0][i];
				double num = inputValues[1][i - 1];
				double num2 = inputValues[1][i];
				outputValues[1][i] = (num2 - num) / num * inputValues[2][i] + outputValues[1][i - 1];
			}
		}

		private void OnBalanceVolume(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList)
		{
			if (inputValues.Length != 3)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsFormulaRequiresTwoArrays);
			}
			CheckNumOfValues(inputValues, 2);
			outputValues = new double[2][];
			outputValues[0] = new double[inputValues[0].Length];
			outputValues[1] = new double[inputValues[0].Length];
			outputValues[0][0] = inputValues[0][0];
			outputValues[1][0] = inputValues[2][0];
			for (int i = 1; i < inputValues[1].Length; i++)
			{
				outputValues[0][i] = inputValues[0][i];
				if (inputValues[1][i - 1] < inputValues[1][i])
				{
					outputValues[1][i] = outputValues[1][i - 1] + inputValues[2][i];
				}
				else if (inputValues[1][i - 1] > inputValues[1][i])
				{
					outputValues[1][i] = outputValues[1][i - 1] - inputValues[2][i];
				}
				else
				{
					outputValues[1][i] = outputValues[1][i - 1];
				}
			}
		}

		private void NegativeVolumeIndex(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList)
		{
			if (inputValues.Length != 3)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsFormulaRequiresTwoArrays);
			}
			CheckNumOfValues(inputValues, 2);
			double num;
			try
			{
				num = double.Parse(parameterList[0], CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				throw new InvalidOperationException(SR.ExceptionVolumeIndicatorStartValueMissing);
			}
			outputValues = new double[2][];
			outputValues[0] = new double[inputValues[0].Length];
			outputValues[1] = new double[inputValues[0].Length];
			outputValues[0][0] = inputValues[0][0];
			outputValues[1][0] = num;
			for (int i = 1; i < inputValues[1].Length; i++)
			{
				outputValues[0][i] = inputValues[0][i];
				if (inputValues[2][i] < inputValues[2][i - 1])
				{
					double num2 = inputValues[1][i - 1];
					double num3 = inputValues[1][i];
					outputValues[1][i] = (num3 - num2) / num2 * outputValues[1][i - 1] + outputValues[1][i - 1];
				}
				else
				{
					outputValues[1][i] = outputValues[1][i - 1];
				}
			}
		}

		private void PositiveVolumeIndex(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList)
		{
			if (inputValues.Length != 3)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsFormulaRequiresTwoArrays);
			}
			CheckNumOfValues(inputValues, 2);
			double num;
			try
			{
				num = double.Parse(parameterList[0], CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				throw new InvalidOperationException(SR.ExceptionVolumeIndicatorStartValueMissing);
			}
			outputValues = new double[2][];
			outputValues[0] = new double[inputValues[0].Length];
			outputValues[1] = new double[inputValues[0].Length];
			outputValues[0][0] = inputValues[0][0];
			outputValues[1][0] = num;
			for (int i = 1; i < inputValues[1].Length; i++)
			{
				outputValues[0][i] = inputValues[0][i];
				if (inputValues[2][i] > inputValues[2][i - 1])
				{
					double num2 = inputValues[1][i - 1];
					double num3 = inputValues[1][i];
					outputValues[1][i] = (num3 - num2) / num2 * outputValues[1][i - 1] + outputValues[1][i - 1];
				}
				else
				{
					outputValues[1][i] = outputValues[1][i - 1];
				}
			}
		}

		internal void AccumulationDistribution(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList)
		{
			if (inputValues.Length != 5)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsFormulaRequiresFourArrays);
			}
			CheckNumOfValues(inputValues, 4);
			outputValues = new double[2][];
			outputValues[0] = new double[inputValues[0].Length];
			outputValues[1] = new double[inputValues[0].Length];
			double[] array = new double[inputValues[0].Length];
			outputValues[0][0] = inputValues[0][0];
			outputValues[1][0] = 0.0;
			for (int i = 0; i < inputValues[1].Length; i++)
			{
				outputValues[0][i] = inputValues[0][i];
				array[i] = (inputValues[3][i] - inputValues[2][i] - (inputValues[1][i] - inputValues[3][i])) / (inputValues[1][i] - inputValues[2][i]) * inputValues[4][i];
			}
			double num = 0.0;
			for (int j = 0; j < inputValues[1].Length; j++)
			{
				num += array[j];
				outputValues[1][j] = num;
			}
		}
	}
}
