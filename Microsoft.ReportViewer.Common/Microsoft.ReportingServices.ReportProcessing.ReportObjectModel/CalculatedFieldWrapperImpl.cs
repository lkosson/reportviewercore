namespace Microsoft.ReportingServices.ReportProcessing.ReportObjectModel
{
	internal sealed class CalculatedFieldWrapperImpl : CalculatedFieldWrapper
	{
		private Microsoft.ReportingServices.ReportProcessing.Field m_fieldDef;

		private object m_value;

		private bool m_isValueReady;

		private bool m_isVisited;

		private ReportRuntime m_reportRT;

		private IErrorContext m_iErrorContext;

		public override object Value
		{
			get
			{
				if (!m_isValueReady)
				{
					if (m_isVisited)
					{
						m_iErrorContext.Register(ProcessingErrorCode.rsCyclicExpression, Severity.Warning, ObjectType.Field, m_fieldDef.Name, "Value");
						throw new ReportProcessingException_InvalidOperationException();
					}
					m_isVisited = true;
					m_value = m_reportRT.EvaluateFieldValueExpression(m_fieldDef);
					m_isVisited = false;
					m_isValueReady = true;
				}
				return m_value;
			}
		}

		internal CalculatedFieldWrapperImpl(Microsoft.ReportingServices.ReportProcessing.Field fieldDef, ReportRuntime reportRT)
		{
			m_fieldDef = fieldDef;
			m_reportRT = reportRT;
			m_iErrorContext = reportRT;
		}
	}
}
