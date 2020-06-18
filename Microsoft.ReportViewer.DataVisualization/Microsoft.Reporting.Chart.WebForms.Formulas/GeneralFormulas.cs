using System;
using System.Globalization;

namespace Microsoft.Reporting.Chart.WebForms.Formulas
{
	internal class GeneralFormulas : PriceIndicators
	{
		public override string Name => SR.FormulaNameGeneralFormulas;

		private void RuningTotal(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList)
		{
			if (inputValues.Length != 2)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsFormulaRequiresOneArray);
			}
			CheckNumOfValues(inputValues, 1);
			outputValues = new double[2][];
			outputValues[0] = new double[inputValues[0].Length];
			outputValues[1] = new double[inputValues[1].Length];
			for (int i = 0; i < inputValues[0].Length; i++)
			{
				outputValues[0][i] = inputValues[0][i];
				if (i > 0)
				{
					outputValues[1][i] = inputValues[1][i] + outputValues[1][i - 1];
				}
				else
				{
					outputValues[1][i] = inputValues[1][i];
				}
			}
		}

		private void RuningAverage(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList)
		{
			if (inputValues.Length != 2)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsFormulaRequiresOneArray);
			}
			CheckNumOfValues(inputValues, 1);
			outputValues = new double[2][];
			outputValues[0] = new double[inputValues[0].Length];
			outputValues[1] = new double[inputValues[1].Length];
			double num = 0.0;
			for (int i = 0; i < inputValues[0].Length; i++)
			{
				num += inputValues[1][i];
			}
			for (int j = 0; j < inputValues[0].Length; j++)
			{
				outputValues[0][j] = inputValues[0][j];
				if (j > 0)
				{
					outputValues[1][j] = inputValues[1][j] / num * 100.0 + outputValues[1][j - 1];
				}
				else
				{
					outputValues[1][j] = inputValues[1][j] / num * 100.0;
				}
			}
		}

		public override void Formula(string formulaName, double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList, out string[][] outLabels)
		{
			outputValues = null;
			outLabels = null;
			string text = formulaName.ToUpper(CultureInfo.InvariantCulture);
			try
			{
				if (!(text == "RUNINGTOTAL"))
				{
					if (text == "RUNINGAVERAGE")
					{
						RuningAverage(inputValues, out outputValues, parameterList, extraParameterList);
					}
					else
					{
						outputValues = null;
					}
				}
				else
				{
					RuningTotal(inputValues, out outputValues, parameterList, extraParameterList);
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
