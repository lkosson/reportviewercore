using System;
using System.Globalization;

namespace Microsoft.Reporting.Chart.WebForms.Formulas
{
	internal class Oscillators : PriceIndicators
	{
		public override string Name => "Oscillators";

		private void ChaikinOscillator(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList)
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
			catch (Exception)
			{
				num = 3;
			}
			int num2;
			try
			{
				num2 = int.Parse(parameterList[1], CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				num2 = 10;
			}
			if (num > num2 || num2 <= 0 || num <= 0)
			{
				throw new ArgumentException(SR.ExceptionOscillatorObjectInvalidPeriod);
			}
			bool flag = bool.Parse(extraParameterList[0]);
			VolumeIndicators volumeIndicators = new VolumeIndicators();
			double[][] outputValues2 = new double[2][];
			volumeIndicators.AccumulationDistribution(inputValues, out outputValues2, parameterList, extraParameterList);
			ExponentialMovingAverage(outputValues2[1], out double[] outputValues3, num2, flag);
			ExponentialMovingAverage(outputValues2[1], out double[] outputValues4, num, flag);
			outputValues = new double[2][];
			int num3 = Math.Min(outputValues4.Length, outputValues3.Length);
			outputValues[0] = new double[num3];
			outputValues[1] = new double[num3];
			int num4 = 0;
			for (int i = inputValues[1].Length - num3; i < inputValues[1].Length; i++)
			{
				outputValues[0][num4] = inputValues[0][i];
				if (flag)
				{
					outputValues[1][num4] = outputValues4[num4] - outputValues3[num4];
				}
				else if (num4 + num2 - num < outputValues4.Length)
				{
					outputValues[1][num4] = outputValues4[num4 + num2 - num] - outputValues3[num4];
				}
				else
				{
					outputValues[1][num4] = double.NaN;
				}
				num4++;
			}
		}

		private void DetrendedPriceOscillator(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList)
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
			MovingAverage(inputValues[1], out double[] outputValues2, num, FromFirst: false);
			outputValues = new double[2][];
			outputValues[0] = new double[inputValues[0].Length - num * 3 / 2];
			outputValues[1] = new double[inputValues[1].Length - num * 3 / 2];
			for (int i = 0; i < outputValues[1].Length; i++)
			{
				outputValues[0][i] = inputValues[0][i + num + num / 2];
				outputValues[1][i] = inputValues[1][i + num + num / 2] - outputValues2[i];
			}
		}

		private void VolatilityChaikins(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList)
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
				num = 10;
			}
			if (num <= 0)
			{
				throw new InvalidOperationException(SR.ExceptionOscillatorNegativePeriodParameter);
			}
			int num2;
			try
			{
				num2 = int.Parse(parameterList[1], CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				num2 = 10;
			}
			if (num2 <= 0)
			{
				throw new InvalidOperationException(SR.ExceptionOscillatorNegativeSignalPeriod);
			}
			double[] array = new double[inputValues[1].Length];
			for (int i = 0; i < inputValues[1].Length; i++)
			{
				array[i] = inputValues[1][i] - inputValues[2][i];
			}
			ExponentialMovingAverage(array, out double[] outputValues2, num2, startFromFirst: false);
			outputValues = new double[2][];
			outputValues[0] = new double[outputValues2.Length - num];
			outputValues[1] = new double[outputValues2.Length - num];
			for (int j = 0; j < outputValues[1].Length; j++)
			{
				outputValues[0][j] = inputValues[0][j + num + num2 - 1];
				if (outputValues2[j] != 0.0)
				{
					outputValues[1][j] = (outputValues2[j + num] - outputValues2[j]) / outputValues2[j] * 100.0;
				}
				else
				{
					outputValues[1][j] = 0.0;
				}
			}
		}

		private void VolumeOscillator(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList)
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
				num = 5;
			}
			int num2;
			try
			{
				num2 = int.Parse(parameterList[1], CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				num2 = 10;
			}
			if (num > num2 || num2 <= 0 || num <= 0)
			{
				throw new ArgumentException(SR.ExceptionOscillatorObjectInvalidPeriod);
			}
			bool flag;
			try
			{
				flag = bool.Parse(parameterList[2]);
			}
			catch (Exception)
			{
				flag = true;
			}
			MovingAverage(inputValues[1], out double[] outputValues2, num, FromFirst: false);
			MovingAverage(inputValues[1], out double[] outputValues3, num2, FromFirst: false);
			outputValues = new double[2][];
			outputValues[0] = new double[outputValues3.Length];
			outputValues[1] = new double[outputValues3.Length];
			for (int i = 0; i < outputValues3.Length; i++)
			{
				outputValues[0][i] = inputValues[0][i + num2 - 1];
				outputValues[1][i] = outputValues2[i + num] - outputValues3[i];
				if (flag)
				{
					if (outputValues3[i] == 0.0)
					{
						outputValues[1][i] = 0.0;
					}
					else
					{
						outputValues[1][i] = outputValues[1][i] / outputValues2[i + num] * 100.0;
					}
				}
			}
		}

		internal void StochasticIndicator(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList)
		{
			if (inputValues.Length != 4)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsFormulaRequiresThreeArrays);
			}
			CheckNumOfValues(inputValues, 3);
			int num;
			try
			{
				num = int.Parse(parameterList[1], CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				num = 10;
			}
			if (num <= 0)
			{
				throw new InvalidOperationException(SR.ExceptionPeriodParameterIsNegative);
			}
			int num2;
			try
			{
				num2 = int.Parse(parameterList[0], CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				num2 = 10;
			}
			if (num2 <= 0)
			{
				throw new InvalidOperationException(SR.ExceptionPeriodParameterIsNegative);
			}
			outputValues = new double[3][];
			outputValues[0] = new double[inputValues[0].Length - num2 - num + 2];
			outputValues[1] = new double[inputValues[0].Length - num2 - num + 2];
			outputValues[2] = new double[inputValues[0].Length - num2 - num + 2];
			double[] array = new double[inputValues[0].Length - num2 + 1];
			for (int i = num2 - 1; i < inputValues[0].Length; i++)
			{
				double num3 = double.MaxValue;
				double num4 = double.MinValue;
				for (int j = i - num2 + 1; j <= i; j++)
				{
					if (num3 > inputValues[2][j])
					{
						num3 = inputValues[2][j];
					}
					if (num4 < inputValues[1][j])
					{
						num4 = inputValues[1][j];
					}
				}
				array[i - num2 + 1] = (inputValues[3][i] - num3) / (num4 - num3) * 100.0;
				if (i >= num2 + num - 2)
				{
					outputValues[0][i - num2 - num + 2] = inputValues[0][i];
					outputValues[1][i - num2 - num + 2] = array[i - num2 + 1];
				}
			}
			MovingAverage(array, out outputValues[2], num, FromFirst: false);
		}

		internal void WilliamsR(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList)
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
			outputValues = new double[2][];
			outputValues[0] = new double[inputValues[0].Length - num + 1];
			outputValues[1] = new double[inputValues[0].Length - num + 1];
			for (int i = num - 1; i < inputValues[0].Length; i++)
			{
				double num2 = double.MaxValue;
				double num3 = double.MinValue;
				for (int j = i - num + 1; j <= i; j++)
				{
					if (num2 > inputValues[2][j])
					{
						num2 = inputValues[2][j];
					}
					if (num3 < inputValues[1][j])
					{
						num3 = inputValues[1][j];
					}
				}
				outputValues[0][i - num + 1] = inputValues[0][i];
				outputValues[1][i - num + 1] = (num3 - inputValues[3][i]) / (num3 - num2) * -100.0;
			}
		}

		public override void Formula(string formulaName, double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList, out string[][] outLabels)
		{
			outputValues = null;
			outLabels = null;
			string text = formulaName.ToUpper(CultureInfo.InvariantCulture);
			try
			{
				switch (text)
				{
				case "STOCHASTICINDICATOR":
					StochasticIndicator(inputValues, out outputValues, parameterList, extraParameterList);
					break;
				case "CHAIKINOSCILLATOR":
					ChaikinOscillator(inputValues, out outputValues, parameterList, extraParameterList);
					break;
				case "DETRENDEDPRICEOSCILLATOR":
					DetrendedPriceOscillator(inputValues, out outputValues, parameterList, extraParameterList);
					break;
				case "VOLATILITYCHAIKINS":
					VolatilityChaikins(inputValues, out outputValues, parameterList, extraParameterList);
					break;
				case "VOLUMEOSCILLATOR":
					VolumeOscillator(inputValues, out outputValues, parameterList, extraParameterList);
					break;
				case "WILLIAMSR":
					WilliamsR(inputValues, out outputValues, parameterList, extraParameterList);
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
