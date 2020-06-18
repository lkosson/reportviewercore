using System;
using System.Globalization;

namespace Microsoft.Reporting.Chart.WebForms.Formulas
{
	internal class StatisticalAnalysis : IFormula
	{
		public virtual string Name => SR.FormulaNameStatisticalAnalysis;

		public virtual void Formula(string formulaName, double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList, out string[][] outLabels)
		{
			outLabels = null;
			string text = formulaName.ToUpper(CultureInfo.InvariantCulture);
			try
			{
				switch (text)
				{
				case "TTESTEQUALVARIANCES":
					TTest(inputValues, out outputValues, parameterList, extraParameterList, out outLabels, equalVariances: true);
					break;
				case "TTESTUNEQUALVARIANCES":
					TTest(inputValues, out outputValues, parameterList, extraParameterList, out outLabels, equalVariances: false);
					break;
				case "TTESTPAIRED":
					TTestPaired(inputValues, out outputValues, parameterList, extraParameterList, out outLabels);
					break;
				case "ZTEST":
					ZTest(inputValues, out outputValues, parameterList, extraParameterList, out outLabels);
					break;
				case "FTEST":
					FTest(inputValues, out outputValues, parameterList, extraParameterList, out outLabels);
					break;
				case "COVARIANCE":
					Covariance(inputValues, out outputValues, parameterList, extraParameterList, out outLabels);
					break;
				case "CORRELATION":
					Correlation(inputValues, out outputValues, parameterList, extraParameterList, out outLabels);
					break;
				case "ANOVA":
					Anova(inputValues, out outputValues, parameterList, extraParameterList, out outLabels);
					break;
				case "TDISTRIBUTION":
					TDistribution(inputValues, out outputValues, parameterList, extraParameterList, out outLabels);
					break;
				case "FDISTRIBUTION":
					FDistribution(inputValues, out outputValues, parameterList, extraParameterList, out outLabels);
					break;
				case "NORMALDISTRIBUTION":
					NormalDistribution(inputValues, out outputValues, parameterList, extraParameterList, out outLabels);
					break;
				case "INVERSETDISTRIBUTION":
					TDistributionInverse(inputValues, out outputValues, parameterList, extraParameterList, out outLabels);
					break;
				case "INVERSEFDISTRIBUTION":
					FDistributionInverse(inputValues, out outputValues, parameterList, extraParameterList, out outLabels);
					break;
				case "INVERSENORMALDISTRIBUTION":
					NormalDistributionInverse(inputValues, out outputValues, parameterList, extraParameterList, out outLabels);
					break;
				case "MEAN":
					Average(inputValues, out outputValues, parameterList, extraParameterList, out outLabels);
					break;
				case "VARIANCE":
					Variance(inputValues, out outputValues, parameterList, extraParameterList, out outLabels);
					break;
				case "MEDIAN":
					Median(inputValues, out outputValues, parameterList, extraParameterList, out outLabels);
					break;
				case "BETAFUNCTION":
					BetaFunction(inputValues, out outputValues, parameterList, extraParameterList, out outLabels);
					break;
				case "GAMMAFUNCTION":
					GammaFunction(inputValues, out outputValues, parameterList, extraParameterList, out outLabels);
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

		private void Anova(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList, out string[][] outLabels)
		{
			if (inputValues.Length < 3)
			{
				throw new ArgumentException(SR.ExceptionStatisticalAnalysesNotEnoughInputSeries);
			}
			outLabels = null;
			for (int i = 0; i < inputValues.Length - 1; i++)
			{
				if (inputValues[i].Length != inputValues[i + 1].Length)
				{
					throw new InvalidOperationException(SR.ExceptionStatisticalAnalysesInvalidAnovaTest);
				}
			}
			double num;
			try
			{
				num = double.Parse(parameterList[0], CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				throw new InvalidOperationException(SR.ExceptionStatisticalAnalysesInvalidAlphaValue);
			}
			if (num < 0.0 || num > 1.0)
			{
				throw new InvalidOperationException(SR.ExceptionStatisticalAnalysesInvalidAlphaValue);
			}
			outputValues = new double[2][];
			outLabels = new string[1][];
			outLabels[0] = new string[10];
			outputValues[0] = new double[10];
			outputValues[1] = new double[10];
			int num2 = inputValues.Length - 1;
			int num3 = inputValues[0].Length;
			double[] array = new double[num2];
			double[] array2 = new double[num2];
			for (int j = 0; j < num2; j++)
			{
				array[j] = Mean(inputValues[j + 1]);
			}
			for (int k = 0; k < num2; k++)
			{
				array2[k] = Variance(inputValues[k + 1], sampleVariance: true);
			}
			double num4 = Mean(array);
			double num5 = 0.0;
			double[] array3 = array;
			foreach (double num6 in array3)
			{
				num5 += (num6 - num4) * (num6 - num4);
			}
			num5 /= (double)(num2 - 1);
			double num7 = Mean(array2);
			double num8 = num5 * (double)num3 / num7;
			double num9 = 0.0;
			for (int m = 0; m < num2; m++)
			{
				array3 = inputValues[m + 1];
				foreach (double num10 in array3)
				{
					num9 += num10;
				}
			}
			num9 /= (double)(num2 * num3);
			double num11 = 0.0;
			for (int n = 0; n < num2; n++)
			{
				num11 += (array[n] - num9) * (array[n] - num9);
			}
			num11 *= (double)num3;
			double num12 = 0.0;
			for (int num13 = 0; num13 < num2; num13++)
			{
				array3 = inputValues[num13 + 1];
				foreach (double num14 in array3)
				{
					num12 += (num14 - array[num13]) * (num14 - array[num13]);
				}
			}
			outLabels[0][0] = SR.LabelStatisticalSumOfSquaresBetweenGroups;
			outputValues[0][0] = 1.0;
			outputValues[1][0] = num11;
			outLabels[0][1] = SR.LabelStatisticalSumOfSquaresWithinGroups;
			outputValues[0][1] = 2.0;
			outputValues[1][1] = num12;
			outLabels[0][2] = SR.LabelStatisticalSumOfSquaresTotal;
			outputValues[0][2] = 3.0;
			outputValues[1][2] = num11 + num12;
			outLabels[0][3] = SR.LabelStatisticalDegreesOfFreedomBetweenGroups;
			outputValues[0][3] = 4.0;
			outputValues[1][3] = num2 - 1;
			outLabels[0][4] = SR.LabelStatisticalDegreesOfFreedomWithinGroups;
			outputValues[0][4] = 5.0;
			outputValues[1][4] = num2 * (num3 - 1);
			outLabels[0][5] = SR.LabelStatisticalDegreesOfFreedomTotal;
			outputValues[0][5] = 6.0;
			outputValues[1][5] = num2 * num3 - 1;
			outLabels[0][6] = SR.LabelStatisticalMeanSquareVarianceBetweenGroups;
			outputValues[0][6] = 7.0;
			outputValues[1][6] = num11 / (double)(num2 - 1);
			outLabels[0][7] = SR.LabelStatisticalMeanSquareVarianceWithinGroups;
			outputValues[0][7] = 8.0;
			outputValues[1][7] = num12 / (double)(num2 * (num3 - 1));
			outLabels[0][8] = SR.LabelStatisticalFRatio;
			outputValues[0][8] = 9.0;
			outputValues[1][8] = num8;
			outLabels[0][9] = SR.LabelStatisticalFCrit;
			outputValues[0][9] = 10.0;
			outputValues[1][9] = FDistributionInverse(num, num2 - 1, num2 * (num3 - 1));
		}

		private void Correlation(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList, out string[][] outLabels)
		{
			if (inputValues.Length != 3)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsFormulaRequiresTwoArrays);
			}
			outLabels = null;
			outputValues = new double[2][];
			outLabels = new string[1][];
			outLabels[0] = new string[1];
			outputValues[0] = new double[1];
			outputValues[1] = new double[1];
			double num = Covar(inputValues[1], inputValues[2]);
			double num2 = Variance(inputValues[1], sampleVariance: false);
			double num3 = Variance(inputValues[2], sampleVariance: false);
			double num4 = num / Math.Sqrt(num2 * num3);
			outLabels[0][0] = SR.LabelStatisticalCorrelation;
			outputValues[0][0] = 1.0;
			outputValues[1][0] = num4;
		}

		private void Covariance(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList, out string[][] outLabels)
		{
			if (inputValues.Length != 3)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsFormulaRequiresTwoArrays);
			}
			outLabels = null;
			outputValues = new double[2][];
			outLabels = new string[1][];
			outLabels[0] = new string[1];
			outputValues[0] = new double[1];
			outputValues[1] = new double[1];
			double num = Covar(inputValues[1], inputValues[2]);
			outLabels[0][0] = SR.LabelStatisticalCovariance;
			outputValues[0][0] = 1.0;
			outputValues[1][0] = num;
		}

		private void FTest(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList, out string[][] outLabels)
		{
			if (inputValues.Length != 3)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsFormulaRequiresTwoArrays);
			}
			outLabels = null;
			CheckNumOfPoints(inputValues);
			double num;
			try
			{
				num = double.Parse(parameterList[0], CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				throw new InvalidOperationException(SR.ExceptionStatisticalAnalysesInvalidAlphaValue);
			}
			if (num < 0.0 || num > 1.0)
			{
				throw new InvalidOperationException(SR.ExceptionStatisticalAnalysesInvalidAlphaValue);
			}
			outputValues = new double[2][];
			outLabels = new string[1][];
			outLabels[0] = new string[7];
			outputValues[0] = new double[7];
			outputValues[1] = new double[7];
			double num2 = Variance(inputValues[1], sampleVariance: true);
			double num3 = Variance(inputValues[2], sampleVariance: true);
			double num4 = Mean(inputValues[1]);
			double num5 = Mean(inputValues[2]);
			double num6 = num2 / num3;
			if (num3 == 0.0)
			{
				throw new InvalidOperationException(SR.ExceptionStatisticalAnalysesZeroVariance);
			}
			double num7;
			double num8;
			if (num6 <= 1.0)
			{
				num7 = FDistributionInverse(1.0 - num, inputValues[1].Length - 1, inputValues[2].Length - 1);
				num8 = 1.0 - FDistribution(num6, inputValues[1].Length - 1, inputValues[2].Length - 1);
			}
			else
			{
				num7 = FDistributionInverse(num, inputValues[1].Length - 1, inputValues[2].Length - 1);
				num8 = FDistribution(num6, inputValues[1].Length - 1, inputValues[2].Length - 1);
			}
			outLabels[0][0] = SR.LabelStatisticalTheFirstGroupMean;
			outputValues[0][0] = 1.0;
			outputValues[1][0] = num4;
			outLabels[0][1] = SR.LabelStatisticalTheSecondGroupMean;
			outputValues[0][1] = 2.0;
			outputValues[1][1] = num5;
			outLabels[0][2] = SR.LabelStatisticalTheFirstGroupVariance;
			outputValues[0][2] = 3.0;
			outputValues[1][2] = num2;
			outLabels[0][3] = SR.LabelStatisticalTheSecondGroupVariance;
			outputValues[0][3] = 4.0;
			outputValues[1][3] = num3;
			outLabels[0][4] = SR.LabelStatisticalFValue;
			outputValues[0][4] = 5.0;
			outputValues[1][4] = num6;
			outLabels[0][5] = SR.LabelStatisticalPFLessEqualSmallFOneTail;
			outputValues[0][5] = 6.0;
			outputValues[1][5] = num8;
			outLabels[0][6] = SR.LabelStatisticalFCriticalValueOneTail;
			outputValues[0][6] = 7.0;
			outputValues[1][6] = num7;
		}

		private void ZTest(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList, out string[][] outLabels)
		{
			if (inputValues.Length != 3)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsFormulaRequiresTwoArrays);
			}
			CheckNumOfPoints(inputValues);
			outLabels = null;
			double num;
			try
			{
				num = double.Parse(parameterList[0], CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				throw new InvalidOperationException(SR.ExceptionStatisticalAnalysesInvalidMeanDifference);
			}
			if (num < 0.0)
			{
				throw new InvalidOperationException(SR.ExceptionStatisticalAnalysesNegativeMeanDifference);
			}
			double num2;
			try
			{
				num2 = double.Parse(parameterList[1], CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				throw new InvalidOperationException(SR.ExceptionStatisticalAnalysesInvalidVariance);
			}
			double num3;
			try
			{
				num3 = double.Parse(parameterList[2], CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				throw new InvalidOperationException(SR.ExceptionStatisticalAnalysesInvalidVariance);
			}
			double num4;
			try
			{
				num4 = double.Parse(parameterList[3], CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				throw new InvalidOperationException(SR.ExceptionStatisticalAnalysesInvalidAlphaValue);
			}
			if (num4 < 0.0 || num4 > 1.0)
			{
				throw new InvalidOperationException(SR.ExceptionStatisticalAnalysesInvalidAlphaValue);
			}
			outputValues = new double[2][];
			outLabels = new string[1][];
			outLabels[0] = new string[9];
			outputValues[0] = new double[9];
			outputValues[1] = new double[9];
			double num5 = Mean(inputValues[1]);
			double num6 = Mean(inputValues[2]);
			double num7 = Math.Sqrt(num2 / (double)inputValues[1].Length + num3 / (double)inputValues[2].Length);
			double num8 = (num5 - num6 - num) / num7;
			double num9 = NormalDistributionInverse(1.0 - num4 / 2.0);
			double num10 = NormalDistributionInverse(1.0 - num4);
			double num11 = (!(num8 < 0.0)) ? (1.0 - NormalDistribution(num8)) : NormalDistribution(num8);
			double num12 = 2.0 * num11;
			outLabels[0][0] = SR.LabelStatisticalTheFirstGroupMean;
			outputValues[0][0] = 1.0;
			outputValues[1][0] = num5;
			outLabels[0][1] = SR.LabelStatisticalTheSecondGroupMean;
			outputValues[0][1] = 2.0;
			outputValues[1][1] = num6;
			outLabels[0][2] = SR.LabelStatisticalTheFirstGroupVariance;
			outputValues[0][2] = 3.0;
			outputValues[1][2] = num2;
			outLabels[0][3] = SR.LabelStatisticalTheSecondGroupVariance;
			outputValues[0][3] = 4.0;
			outputValues[1][3] = num3;
			outLabels[0][4] = SR.LabelStatisticalZValue;
			outputValues[0][4] = 5.0;
			outputValues[1][4] = num8;
			outLabels[0][5] = SR.LabelStatisticalPZLessEqualSmallZOneTail;
			outputValues[0][5] = 6.0;
			outputValues[1][5] = num11;
			outLabels[0][6] = SR.LabelStatisticalZCriticalValueOneTail;
			outputValues[0][6] = 7.0;
			outputValues[1][6] = num10;
			outLabels[0][7] = SR.LabelStatisticalPZLessEqualSmallZTwoTail;
			outputValues[0][7] = 8.0;
			outputValues[1][7] = num12;
			outLabels[0][8] = SR.LabelStatisticalZCriticalValueTwoTail;
			outputValues[0][8] = 9.0;
			outputValues[1][8] = num9;
		}

		private void TTest(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList, out string[][] outLabels, bool equalVariances)
		{
			if (inputValues.Length != 3)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsFormulaRequiresTwoArrays);
			}
			outLabels = null;
			double num;
			try
			{
				num = double.Parse(parameterList[0], CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				throw new InvalidOperationException(SR.ExceptionStatisticalAnalysesInvalidMeanDifference);
			}
			if (num < 0.0)
			{
				throw new InvalidOperationException(SR.ExceptionStatisticalAnalysesNegativeMeanDifference);
			}
			double num2;
			try
			{
				num2 = double.Parse(parameterList[1], CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				throw new InvalidOperationException(SR.ExceptionStatisticalAnalysesInvalidAlphaValue);
			}
			if (num2 < 0.0 || num2 > 1.0)
			{
				throw new InvalidOperationException(SR.ExceptionStatisticalAnalysesInvalidAlphaValue);
			}
			CheckNumOfPoints(inputValues);
			outputValues = new double[2][];
			outLabels = new string[1][];
			outLabels[0] = new string[10];
			outputValues[0] = new double[10];
			outputValues[1] = new double[10];
			double num3 = Mean(inputValues[1]);
			double num4 = Mean(inputValues[2]);
			double num5 = Variance(inputValues[1], sampleVariance: true);
			double num6 = Variance(inputValues[2], sampleVariance: true);
			int num7;
			double num9;
			if (equalVariances)
			{
				num7 = inputValues[1].Length + inputValues[2].Length - 2;
				double num8 = ((double)(inputValues[1].Length - 1) * num5 + (double)(inputValues[2].Length - 1) * num6) / (double)(inputValues[1].Length + inputValues[2].Length - 2);
				num9 = (num3 - num4 - num) / Math.Sqrt(num8 * (1.0 / (double)inputValues[1].Length + 1.0 / (double)inputValues[2].Length));
			}
			else
			{
				double num10 = inputValues[1].Length;
				double num11 = inputValues[2].Length;
				double num12 = num5;
				double num13 = num6;
				num7 = (int)Math.Round((num12 / num10 + num13 / num11) * (num12 / num10 + num13 / num11) / (num12 / num10 * (num12 / num10) / (num10 - 1.0) + num13 / num11 * (num13 / num11) / (num11 - 1.0)));
				double num8 = Math.Sqrt(num5 / (double)inputValues[1].Length + num6 / (double)inputValues[2].Length);
				num9 = (num3 - num4 - num) / num8;
			}
			double num14 = StudentsDistributionInverse(num2, num7);
			bool num15 = num2 > 0.5;
			if (num15)
			{
				num2 = 1.0 - num2;
			}
			double num16 = StudentsDistributionInverse(num2 * 2.0, num7);
			if (num15)
			{
				num16 *= -1.0;
			}
			double num17 = StudentsDistribution(num9, num7, oneTailed: false);
			double num18 = StudentsDistribution(num9, num7, oneTailed: true);
			outLabels[0][0] = SR.LabelStatisticalTheFirstGroupMean;
			outputValues[0][0] = 1.0;
			outputValues[1][0] = num3;
			outLabels[0][1] = SR.LabelStatisticalTheSecondGroupMean;
			outputValues[0][1] = 2.0;
			outputValues[1][1] = num4;
			outLabels[0][2] = SR.LabelStatisticalTheFirstGroupVariance;
			outputValues[0][2] = 3.0;
			outputValues[1][2] = num5;
			outLabels[0][3] = SR.LabelStatisticalTheSecondGroupVariance;
			outputValues[0][3] = 4.0;
			outputValues[1][3] = num6;
			outLabels[0][4] = SR.LabelStatisticalTValue;
			outputValues[0][4] = 5.0;
			outputValues[1][4] = num9;
			outLabels[0][5] = SR.LabelStatisticalDegreeOfFreedom;
			outputValues[0][5] = 6.0;
			outputValues[1][5] = num7;
			outLabels[0][6] = SR.LabelStatisticalPTLessEqualSmallTOneTail;
			outputValues[0][6] = 7.0;
			outputValues[1][6] = num18;
			outLabels[0][7] = SR.LabelStatisticalSmallTCrititcalOneTail;
			outputValues[0][7] = 8.0;
			outputValues[1][7] = num16;
			outLabels[0][8] = SR.LabelStatisticalPTLessEqualSmallTTwoTail;
			outputValues[0][8] = 9.0;
			outputValues[1][8] = num17;
			outLabels[0][9] = SR.LabelStatisticalSmallTCrititcalTwoTail;
			outputValues[0][9] = 10.0;
			outputValues[1][9] = num14;
		}

		private void TTestPaired(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList, out string[][] outLabels)
		{
			if (inputValues.Length != 3)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsFormulaRequiresTwoArrays);
			}
			if (inputValues[1].Length != inputValues[2].Length)
			{
				throw new ArgumentException(SR.ExceptionStatisticalAnalysesInvalidVariableRanges);
			}
			outLabels = null;
			double num;
			try
			{
				num = double.Parse(parameterList[0], CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				throw new InvalidOperationException(SR.ExceptionStatisticalAnalysesInvalidMeanDifference);
			}
			if (num < 0.0)
			{
				throw new InvalidOperationException(SR.ExceptionStatisticalAnalysesNegativeMeanDifference);
			}
			double num2;
			try
			{
				num2 = double.Parse(parameterList[1], CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				throw new InvalidOperationException(SR.ExceptionStatisticalAnalysesInvalidAlphaValue);
			}
			if (num2 < 0.0 || num2 > 1.0)
			{
				throw new InvalidOperationException(SR.ExceptionStatisticalAnalysesInvalidAlphaValue);
			}
			CheckNumOfPoints(inputValues);
			outputValues = new double[2][];
			outLabels = new string[1][];
			outLabels[0] = new string[10];
			outputValues[0] = new double[10];
			outputValues[1] = new double[10];
			double[] array = new double[inputValues[1].Length];
			for (int i = 0; i < inputValues[1].Length; i++)
			{
				array[i] = inputValues[1][i] - inputValues[2][i];
			}
			double num3 = Mean(array);
			double num4 = Math.Sqrt(Variance(array, sampleVariance: true));
			double num5 = Math.Sqrt(inputValues[1].Length) * (num3 - num) / num4;
			int num6 = inputValues[1].Length - 1;
			double num7 = StudentsDistributionInverse(num2, num6);
			double num8 = StudentsDistributionInverse(2.0 * num2, num6);
			double num9 = StudentsDistribution(num5, num6, oneTailed: false);
			double num10 = StudentsDistribution(num5, num6, oneTailed: true);
			outLabels[0][0] = SR.LabelStatisticalTheFirstGroupMean;
			outputValues[0][0] = 1.0;
			outputValues[1][0] = Mean(inputValues[1]);
			outLabels[0][1] = SR.LabelStatisticalTheSecondGroupMean;
			outputValues[0][1] = 2.0;
			outputValues[1][1] = Mean(inputValues[2]);
			outLabels[0][2] = SR.LabelStatisticalTheFirstGroupVariance;
			outputValues[0][2] = 3.0;
			outputValues[1][2] = Variance(inputValues[1], sampleVariance: true);
			outLabels[0][3] = SR.LabelStatisticalTheSecondGroupVariance;
			outputValues[0][3] = 4.0;
			outputValues[1][3] = Variance(inputValues[2], sampleVariance: true);
			outLabels[0][4] = SR.LabelStatisticalTValue;
			outputValues[0][4] = 5.0;
			outputValues[1][4] = num5;
			outLabels[0][5] = SR.LabelStatisticalDegreeOfFreedom;
			outputValues[0][5] = 6.0;
			outputValues[1][5] = num6;
			outLabels[0][6] = SR.LabelStatisticalPTLessEqualSmallTOneTail;
			outputValues[0][6] = 7.0;
			outputValues[1][6] = num10;
			outLabels[0][7] = SR.LabelStatisticalSmallTCrititcalOneTail;
			outputValues[0][7] = 8.0;
			outputValues[1][7] = num8;
			outLabels[0][8] = SR.LabelStatisticalPTLessEqualSmallTTwoTail;
			outputValues[0][8] = 9.0;
			outputValues[1][8] = num9;
			outLabels[0][9] = SR.LabelStatisticalSmallTCrititcalTwoTail;
			outputValues[0][9] = 10.0;
			outputValues[1][9] = num7;
		}

		private void TDistribution(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList, out string[][] outLabels)
		{
			double tValue;
			try
			{
				tValue = double.Parse(parameterList[0], CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				throw new InvalidOperationException(SR.ExceptionStatisticalAnalysesInvalidTValue);
			}
			int n;
			try
			{
				n = int.Parse(parameterList[1], CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				throw new InvalidOperationException(SR.ExceptionStatisticalAnalysesInvalidDegreeOfFreedom);
			}
			int num;
			try
			{
				num = int.Parse(parameterList[2], CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				throw new InvalidOperationException(SR.ExceptionStatisticalAnalysesInvalidTailedParameter);
			}
			outLabels = null;
			outputValues = new double[2][];
			outLabels = new string[1][];
			outLabels[0] = new string[1];
			outputValues[0] = new double[1];
			outputValues[1] = new double[1];
			outLabels[0][0] = SR.LabelStatisticalProbability;
			outputValues[0][0] = 1.0;
			outputValues[1][0] = StudentsDistribution(tValue, n, num == 1);
		}

		private void FDistribution(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList, out string[][] outLabels)
		{
			double x;
			try
			{
				x = double.Parse(parameterList[0], CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				throw new InvalidOperationException(SR.ExceptionStatisticalAnalysesInvalidTValue);
			}
			int freedom;
			try
			{
				freedom = int.Parse(parameterList[1], CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				throw new InvalidOperationException(SR.ExceptionStatisticalAnalysesInvalidDegreeOfFreedom);
			}
			int freedom2;
			try
			{
				freedom2 = int.Parse(parameterList[2], CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				throw new InvalidOperationException(SR.ExceptionStatisticalAnalysesInvalidDegreeOfFreedom);
			}
			outLabels = null;
			outputValues = new double[2][];
			outLabels = new string[1][];
			outLabels[0] = new string[1];
			outputValues[0] = new double[1];
			outputValues[1] = new double[1];
			outLabels[0][0] = SR.LabelStatisticalProbability;
			outputValues[0][0] = 1.0;
			outputValues[1][0] = FDistribution(x, freedom, freedom2);
		}

		private void NormalDistribution(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList, out string[][] outLabels)
		{
			double zValue;
			try
			{
				zValue = double.Parse(parameterList[0], CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				throw new InvalidOperationException(SR.ExceptionStatisticalAnalysesInvalidZValue);
			}
			outLabels = null;
			outputValues = new double[2][];
			outLabels = new string[1][];
			outLabels[0] = new string[1];
			outputValues[0] = new double[1];
			outputValues[1] = new double[1];
			outLabels[0][0] = SR.LabelStatisticalProbability;
			outputValues[0][0] = 1.0;
			outputValues[1][0] = NormalDistribution(zValue);
		}

		private void TDistributionInverse(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList, out string[][] outLabels)
		{
			double probability;
			try
			{
				probability = double.Parse(parameterList[0], CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				throw new InvalidOperationException(SR.ExceptionStatisticalAnalysesInvalidProbabilityValue);
			}
			int n;
			try
			{
				n = int.Parse(parameterList[1], CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				throw new InvalidOperationException(SR.ExceptionStatisticalAnalysesInvalidDegreeOfFreedom);
			}
			outLabels = null;
			outputValues = new double[2][];
			outLabels = new string[1][];
			outLabels[0] = new string[1];
			outputValues[0] = new double[1];
			outputValues[1] = new double[1];
			outLabels[0][0] = SR.LabelStatisticalProbability;
			outputValues[0][0] = 1.0;
			outputValues[1][0] = StudentsDistributionInverse(probability, n);
		}

		private void FDistributionInverse(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList, out string[][] outLabels)
		{
			double probability;
			try
			{
				probability = double.Parse(parameterList[0], CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				throw new InvalidOperationException(SR.ExceptionStatisticalAnalysesInvalidProbabilityValue);
			}
			int m;
			try
			{
				m = int.Parse(parameterList[1], CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				throw new InvalidOperationException(SR.ExceptionStatisticalAnalysesInvalidDegreeOfFreedom);
			}
			int n;
			try
			{
				n = int.Parse(parameterList[2], CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				throw new InvalidOperationException(SR.ExceptionStatisticalAnalysesInvalidDegreeOfFreedom);
			}
			outLabels = null;
			outputValues = new double[2][];
			outLabels = new string[1][];
			outLabels[0] = new string[1];
			outputValues[0] = new double[1];
			outputValues[1] = new double[1];
			outLabels[0][0] = SR.LabelStatisticalProbability;
			outputValues[0][0] = 1.0;
			outputValues[1][0] = FDistributionInverse(probability, m, n);
		}

		private void NormalDistributionInverse(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList, out string[][] outLabels)
		{
			double probability;
			try
			{
				probability = double.Parse(parameterList[0], CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				throw new InvalidOperationException(SR.ExceptionStatisticalAnalysesInvalidAlphaValue);
			}
			outLabels = null;
			outputValues = new double[2][];
			outLabels = new string[1][];
			outLabels[0] = new string[1];
			outputValues[0] = new double[1];
			outputValues[1] = new double[1];
			outLabels[0][0] = SR.LabelStatisticalProbability;
			outputValues[0][0] = 1.0;
			outputValues[1][0] = NormalDistributionInverse(probability);
		}

		private void CheckNumOfPoints(double[][] inputValues)
		{
			if (inputValues[1].Length < 2)
			{
				throw new InvalidOperationException(SR.ExceptionStatisticalAnalysesNotEnoughDataPoints);
			}
			if (inputValues.Length > 2 && inputValues[2].Length < 2)
			{
				throw new InvalidOperationException(SR.ExceptionStatisticalAnalysesNotEnoughDataPoints);
			}
		}

		private double Covar(double[] arrayX, double[] arrayY)
		{
			if (arrayX.Length != arrayY.Length)
			{
				throw new InvalidOperationException(SR.ExceptionStatisticalAnalysesCovariance);
			}
			double[] array = new double[arrayX.Length];
			for (int i = 0; i < arrayX.Length; i++)
			{
				array[i] = arrayX[i] * arrayY[i];
			}
			double num = Mean(array);
			double num2 = Mean(arrayX);
			double num3 = Mean(arrayY);
			return num - num2 * num3;
		}

		private double GammLn(double n)
		{
			double[] array = new double[6]
			{
				76.180091729471457,
				-86.505320329416776,
				24.014098240830911,
				-1.231739572450155,
				0.001208650973866179,
				-5.395239384953E-06
			};
			if (n < 0.0)
			{
				throw new InvalidOperationException(SR.ExceptionStatisticalAnalysesGammaBetaNegativeParameters);
			}
			double num;
			double num2 = num = n;
			double num3 = num + 5.5;
			num3 -= (num + 0.5) * Math.Log(num3);
			double num4 = 1.0000000001900149;
			for (int i = 0; i <= 5; i++)
			{
				num4 += array[i] / (num2 += 1.0);
			}
			return 0.0 - num3 + Math.Log(2.5066282746310007 * num4 / num);
		}

		private double BetaFunction(double m, double n)
		{
			return Math.Exp(GammLn(m) + GammLn(n) - GammLn(m + n));
		}

		private double BetaCF(double a, double b, double x)
		{
			int num = 100;
			double num2 = 3E-07;
			double num3 = 1E-30;
			double num4 = a + b;
			double num5 = a + 1.0;
			double num6 = a - 1.0;
			double num7 = 1.0;
			double num8 = 1.0 - num4 * x / num5;
			if (Math.Abs(num8) < num3)
			{
				num8 = num3;
			}
			num8 = 1.0 / num8;
			double num9 = num8;
			int i;
			for (i = 1; i <= num; i++)
			{
				int num10 = 2 * i;
				double num11 = (double)i * (b - (double)i) * x / ((num6 + (double)num10) * (a + (double)num10));
				num8 = 1.0 + num11 * num8;
				if (Math.Abs(num8) < num3)
				{
					num8 = num3;
				}
				num7 = 1.0 + num11 / num7;
				if (Math.Abs(num7) < num3)
				{
					num7 = num3;
				}
				num8 = 1.0 / num8;
				num9 *= num8 * num7;
				num11 = (0.0 - (a + (double)i)) * (num4 + (double)i) * x / ((a + (double)num10) * (num5 + (double)num10));
				num8 = 1.0 + num11 * num8;
				if (Math.Abs(num8) < num3)
				{
					num8 = num3;
				}
				num7 = 1.0 + num11 / num7;
				if (Math.Abs(num7) < num3)
				{
					num7 = num3;
				}
				num8 = 1.0 / num8;
				double num12 = num8 * num7;
				num9 *= num12;
				if (Math.Abs(num12 - 1.0) < num2)
				{
					break;
				}
			}
			if (i > num)
			{
				throw new InvalidOperationException(SR.ExceptionStatisticalAnalysesIncompleteBetaFunction);
			}
			return num9;
		}

		private double NormalDistributionFunction(double t)
		{
			return 0.398942280401433 * Math.Exp((0.0 - t) * t / 2.0);
		}

		private double BetaIncomplete(double a, double b, double x)
		{
			double num;
			if (x == 0.0 || x == 1.0)
			{
				num = 0.0;
				return 0.0;
			}
			num = Math.Exp(GammLn(a + b) - GammLn(a) - GammLn(b) + a * Math.Log(x) + b * Math.Log(1.0 - x));
			if (x < (a + 1.0) / (a + b + 2.0))
			{
				return num * BetaCF(a, b, x) / a;
			}
			return 1.0 - num * BetaCF(b, a, 1.0 - x) / b;
		}

		private void Average(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList, out string[][] outLabels)
		{
			outLabels = null;
			if (inputValues.Length != 2)
			{
				throw new ArgumentException(SR.ExceptionStatisticalAnalysesInvalidSeriesNumber);
			}
			outputValues = new double[2][];
			outLabels = new string[1][];
			outLabels[0] = new string[1];
			outputValues[0] = new double[1];
			outputValues[1] = new double[1];
			outLabels[0][0] = SR.LabelStatisticalAverage;
			outputValues[0][0] = 1.0;
			outputValues[1][0] = Mean(inputValues[1]);
		}

		private void Variance(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList, out string[][] outLabels)
		{
			bool sampleVariance;
			try
			{
				sampleVariance = bool.Parse(parameterList[0]);
			}
			catch (Exception)
			{
				throw new InvalidOperationException(SR.ExceptionStatisticalAnalysesInvalidVariance);
			}
			CheckNumOfPoints(inputValues);
			if (inputValues.Length != 2)
			{
				throw new ArgumentException(SR.ExceptionStatisticalAnalysesInvalidSeriesNumber);
			}
			outLabels = null;
			outputValues = new double[2][];
			outLabels = new string[1][];
			outLabels[0] = new string[1];
			outputValues[0] = new double[1];
			outputValues[1] = new double[1];
			outLabels[0][0] = SR.LabelStatisticalVariance;
			outputValues[0][0] = 1.0;
			outputValues[1][0] = Variance(inputValues[1], sampleVariance);
		}

		private void Median(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList, out string[][] outLabels)
		{
			outLabels = null;
			if (inputValues.Length != 2)
			{
				throw new ArgumentException(SR.ExceptionStatisticalAnalysesInvalidSeriesNumber);
			}
			outputValues = new double[2][];
			outLabels = new string[1][];
			outLabels[0] = new string[1];
			outputValues[0] = new double[1];
			outputValues[1] = new double[1];
			outLabels[0][0] = SR.LabelStatisticalMedian;
			outputValues[0][0] = 1.0;
			outputValues[1][0] = Median(inputValues[1]);
		}

		private void BetaFunction(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList, out string[][] outLabels)
		{
			double m;
			try
			{
				m = double.Parse(parameterList[0], CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				throw new InvalidOperationException(SR.ExceptionStatisticalAnalysesInvalidDegreeOfFreedom);
			}
			double n;
			try
			{
				n = double.Parse(parameterList[1], CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				throw new InvalidOperationException(SR.ExceptionStatisticalAnalysesInvalidDegreeOfFreedom);
			}
			outLabels = null;
			outputValues = new double[2][];
			outLabels = new string[1][];
			outLabels[0] = new string[1];
			outputValues[0] = new double[1];
			outputValues[1] = new double[1];
			outLabels[0][0] = SR.LabelStatisticalBetaFunction;
			outputValues[0][0] = 1.0;
			outputValues[1][0] = BetaFunction(m, n);
		}

		private void GammaFunction(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList, out string[][] outLabels)
		{
			double num;
			try
			{
				num = double.Parse(parameterList[0], CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				throw new InvalidOperationException(SR.ExceptionStatisticalAnalysesInvalidInputParameter);
			}
			if (num < 0.0)
			{
				throw new InvalidOperationException(SR.ExceptionStatisticalAnalysesGammaBetaNegativeParameters);
			}
			outLabels = null;
			outputValues = new double[2][];
			outLabels = new string[1][];
			outLabels[0] = new string[1];
			outputValues[0] = new double[1];
			outputValues[1] = new double[1];
			outLabels[0][0] = SR.LabelStatisticalGammaFunction;
			outputValues[0][0] = 1.0;
			outputValues[1][0] = Math.Exp(GammLn(num));
		}

		private void Sort(ref double[] values)
		{
			for (int i = 0; i < values.Length; i++)
			{
				for (int j = i + 1; j < values.Length; j++)
				{
					if (values[i] > values[j])
					{
						double num = values[i];
						values[i] = values[j];
						values[j] = num;
					}
				}
			}
		}

		private double Median(double[] values)
		{
			if (values.Length == 0)
			{
				throw new InvalidOperationException(SR.ExceptionStatisticalAnalysesInvalidMedianConditions);
			}
			Sort(ref values);
			int num = values.Length / 2;
			if (values.Length % 2 == 0)
			{
				return (values[num - 1] + values[num]) / 2.0;
			}
			return values[num];
		}

		private double Mean(double[] values)
		{
			if (values.Length == 0)
			{
				throw new InvalidOperationException(SR.ExceptionStatisticalAnalysesInvalidMeanConditions);
			}
			double num = 0.0;
			foreach (double num2 in values)
			{
				num += num2;
			}
			return num / (double)values.Length;
		}

		private double Variance(double[] values, bool sampleVariance)
		{
			if (values.Length < 1)
			{
				throw new InvalidOperationException(SR.ExceptionStatisticalAnalysesInvalidVarianceConditions);
			}
			double num = 0.0;
			double num2 = Mean(values);
			foreach (double num3 in values)
			{
				num += (num3 - num2) * (num3 - num2);
			}
			if (sampleVariance)
			{
				return num / (double)(values.Length - 1);
			}
			return num / (double)values.Length;
		}

		private double StudentsDistribution(double tValue, int n, bool oneTailed)
		{
			tValue = Math.Abs(tValue);
			if (tValue > 100000.0)
			{
				throw new InvalidOperationException(SR.ExceptionStatisticalAnalysesStudentsInvalidTValue);
			}
			if (n > 300)
			{
				n = 300;
			}
			if (n < 1)
			{
				throw new InvalidOperationException(SR.ExceptionStatisticalAnalysesStudentsNegativeFreedomDegree);
			}
			double num = 1.0 - BetaIncomplete((double)n / 2.0, 0.5, (double)n / ((double)n + tValue * tValue));
			if (oneTailed)
			{
				return (1.0 - num) / 2.0;
			}
			return 1.0 - num;
		}

		private double NormalDistribution(double zValue)
		{
			double[] array = new double[5]
			{
				0.31938153,
				-0.356563782,
				1.781477937,
				-1.821255978,
				1.330274429
			};
			double num;
			if (zValue < -7.0)
			{
				num = NormalDistributionFunction(zValue) / Math.Sqrt(1.0 + zValue * zValue);
			}
			else if (zValue > 7.0)
			{
				num = 1.0 - NormalDistribution(0.0 - zValue);
			}
			else
			{
				num = 0.2316419;
				num = 1.0 / (1.0 + num * Math.Abs(zValue));
				num = 1.0 - NormalDistributionFunction(zValue) * (num * (array[0] + num * (array[1] + num * (array[2] + num * (array[3] + num * array[4])))));
				if (zValue <= 0.0)
				{
					num = 1.0 - num;
				}
			}
			return num;
		}

		private double FDistribution(double x, int freedom1, int freedom2)
		{
			return BetaIncomplete((double)freedom2 / 2.0, (double)freedom1 / 2.0, (double)freedom2 / ((double)freedom2 + (double)freedom1 * x));
		}

		private double StudentsDistributionInverse(double probability, int n)
		{
			int step = 0;
			return StudentsDistributionSearch(probability, n, step, 0.0, 100000.0);
		}

		private double StudentsDistributionSearch(double probability, int n, int step, double start, double end)
		{
			step++;
			double num = (start + end) / 2.0;
			double num2 = StudentsDistribution(num, n, oneTailed: false);
			if (step > 100)
			{
				return num;
			}
			if (num2 <= probability)
			{
				return StudentsDistributionSearch(probability, n, step, start, num);
			}
			return StudentsDistributionSearch(probability, n, step, num, end);
		}

		private double NormalDistributionInverse(double probability)
		{
			if (probability < 1E-05 || probability > 0.99999)
			{
				throw new InvalidOperationException(SR.ExceptionStatisticalAnalysesNormalInvalidProbabilityValue);
			}
			double[] array = new double[4]
			{
				2.50662823884,
				-18.61500062529,
				41.39119773534,
				-25.44106049637
			};
			double[] array2 = new double[4]
			{
				-8.4735109309,
				23.08336743743,
				-21.06224101826,
				3.13082909833
			};
			double[] array3 = new double[9]
			{
				0.3374754822726147,
				0.9761690190917186,
				0.1607979714918209,
				0.0276438810333863,
				0.0038405729373609,
				0.0003951896511919,
				3.21767881768E-05,
				2.888167364E-07,
				3.960315187E-07
			};
			double num = probability - 0.5;
			double num2;
			if (Math.Abs(num) < 0.42)
			{
				num2 = num * num;
				return num * (((array[3] * num2 + array[2]) * num2 + array[1]) * num2 + array[0]) / ((((array2[3] * num2 + array2[2]) * num2 + array2[1]) * num2 + array2[0]) * num2 + 1.0);
			}
			num2 = probability;
			if (num > 0.0)
			{
				num2 = 1.0 - probability;
			}
			num2 = Math.Log(0.0 - Math.Log(num2));
			num2 = array3[0] + num2 * (array3[1] + num2 * (array3[2] + num2 * (array3[3] + num2 * (array3[4] + num2 * (array3[5] + num2 * (array3[6] + num2 * (array3[7] + num2 * array3[8])))))));
			if (num < 0.0)
			{
				num2 = 0.0 - num2;
			}
			return num2;
		}

		private double FDistributionInverse(double probability, int m, int n)
		{
			int step = 0;
			return FDistributionSearch(probability, m, n, step, 0.0, 10000.0);
		}

		private double FDistributionSearch(double probability, int m, int n, int step, double start, double end)
		{
			step++;
			double num = (start + end) / 2.0;
			double num2 = FDistribution(num, m, n);
			if (step > 30)
			{
				return num;
			}
			if (num2 <= probability)
			{
				return FDistributionSearch(probability, m, n, step, start, num);
			}
			return FDistributionSearch(probability, m, n, step, num, end);
		}
	}
}
