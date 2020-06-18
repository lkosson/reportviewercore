namespace Microsoft.ReportingServices.ReportProcessing
{
	internal interface IParameterDef
	{
		int DefaultValuesExpressionCount
		{
			get;
		}

		int ValidValuesValueExpressionCount
		{
			get;
		}

		int ValidValuesLabelExpressionCount
		{
			get;
		}

		string Name
		{
			get;
		}

		ObjectType ParameterObjectType
		{
			get;
		}

		DataType DataType
		{
			get;
		}

		bool MultiValue
		{
			get;
		}

		IParameterDataSource DefaultDataSource
		{
			get;
		}

		IParameterDataSource ValidValuesDataSource
		{
			get;
		}

		bool HasDefaultValuesExpressions();

		bool HasDefaultValuesDataSource();

		bool HasValidValuesValueExpressions();

		bool HasValidValuesLabelExpressions();

		bool HasValidValuesDataSource();

		bool ValidateValueForNull(object newValue, ErrorContext errorContext, string parameterValueProperty);

		bool ValidateValueForBlank(object newValue, ErrorContext errorContext, string parameterValueProperty);
	}
}
