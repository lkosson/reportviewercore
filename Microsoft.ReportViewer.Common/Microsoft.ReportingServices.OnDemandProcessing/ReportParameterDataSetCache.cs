using Microsoft.ReportingServices.RdlExpressions;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal abstract class ReportParameterDataSetCache
	{
		private ProcessReportParameters m_paramProcessor;

		private ParameterInfo m_parameter;

		private IParameterDef m_parameterDef;

		private List<object> m_defaultValues;

		private bool m_processValidValues;

		private bool m_processDefaultValues;

		internal List<object> DefaultValues => m_defaultValues;

		internal ReportParameterDataSetCache(ProcessReportParameters aParamProcessor, ParameterInfo aParameter, IParameterDef aParamDef, bool aProcessValidValues, bool aProcessDefaultValues)
		{
			m_paramProcessor = aParamProcessor;
			m_parameter = aParameter;
			m_parameterDef = aParamDef;
			m_processDefaultValues = aProcessDefaultValues;
			m_processValidValues = aProcessValidValues;
			if (m_processDefaultValues)
			{
				m_defaultValues = new List<object>();
			}
			if (m_processValidValues)
			{
				m_parameter.ValidValues = new ValidValueList();
			}
		}

		internal void NextRow(object aRow)
		{
			if (m_processValidValues)
			{
				IParameterDataSource validValuesDataSource = m_parameterDef.ValidValuesDataSource;
				object obj = null;
				object value = null;
				string output = null;
				bool flag = false;
				try
				{
					flag = false;
					obj = GetFieldValue(aRow, validValuesDataSource.ValueFieldIndex);
					if (validValuesDataSource.LabelFieldIndex >= 0)
					{
						flag = true;
						value = GetFieldValue(aRow, validValuesDataSource.LabelFieldIndex);
					}
					if (!Microsoft.ReportingServices.RdlExpressions.ReportRuntime.ProcessObjectToString(value, autocast: true, out output))
					{
						m_paramProcessor.ProcessingContext.ErrorContext.Register(ProcessingErrorCode.rsParameterPropertyTypeMismatch, Severity.Warning, ObjectType.ReportParameter, m_parameterDef.Name, "Label");
					}
					m_paramProcessor.ConvertAndAddValidValue(m_parameter, m_parameterDef, obj, output);
				}
				catch (ReportProcessingException_FieldError aError)
				{
					int aFieldIndex = flag ? validValuesDataSource.LabelFieldIndex : validValuesDataSource.ValueFieldIndex;
					m_paramProcessor.ThrowExceptionForQueryBackedParameter(aError, m_parameterDef.Name, validValuesDataSource.DataSourceIndex, validValuesDataSource.DataSetIndex, aFieldIndex, "ValidValue");
				}
			}
			if (!m_processDefaultValues)
			{
				return;
			}
			IParameterDataSource defaultDataSource = m_parameterDef.DefaultDataSource;
			try
			{
				if (m_parameterDef.MultiValue || m_defaultValues.Count == 0)
				{
					object fieldValue = GetFieldValue(aRow, defaultDataSource.ValueFieldIndex);
					fieldValue = m_paramProcessor.ConvertValue(fieldValue, m_parameterDef, isDefaultValue: true);
					m_defaultValues.Add(fieldValue);
				}
			}
			catch (ReportProcessingException_FieldError aError2)
			{
				m_paramProcessor.ThrowExceptionForQueryBackedParameter(aError2, m_parameterDef.Name, defaultDataSource.DataSourceIndex, defaultDataSource.DataSetIndex, defaultDataSource.ValueFieldIndex, "DefaultValue");
			}
		}

		internal abstract object GetFieldValue(object aRow, int col);
	}
}
