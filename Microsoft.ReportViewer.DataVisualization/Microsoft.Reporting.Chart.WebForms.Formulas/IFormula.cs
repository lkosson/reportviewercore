namespace Microsoft.Reporting.Chart.WebForms.Formulas
{
	internal interface IFormula
	{
		string Name
		{
			get;
		}

		void Formula(string formulaName, double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList, out string[][] outLabels);
	}
}
