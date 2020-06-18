namespace Microsoft.ReportingServices.ReportProcessing.ReportObjectModel
{
	internal sealed class TextBoxImpl : ReportItemImpl
	{
		private TextBox m_textBox;

		private VariantResult m_result;

		private bool m_isValueReady;

		private bool m_isVisited;

		public override object Value
		{
			get
			{
				GetResult();
				return m_result.Value;
			}
		}

		internal TextBoxImpl(TextBox itemDef, ReportRuntime reportRT, IErrorContext iErrorContext)
			: base(itemDef, reportRT, iErrorContext)
		{
			m_textBox = itemDef;
		}

		internal void SetResult(VariantResult result)
		{
			m_result = result;
			m_isValueReady = true;
		}

		internal VariantResult GetResult()
		{
			if (!m_isValueReady)
			{
				if (m_isVisited)
				{
					m_iErrorContext.Register(ProcessingErrorCode.rsCyclicExpression, Severity.Warning, m_textBox.ObjectType, m_textBox.Name, "Value");
					throw new ReportProcessingException_InvalidOperationException();
				}
				m_isVisited = true;
				ObjectType objectType = m_reportRT.ObjectType;
				string objectName = m_reportRT.ObjectName;
				string propertyName = m_reportRT.PropertyName;
				ReportProcessing.IScope currentScope = m_reportRT.CurrentScope;
				m_reportRT.CurrentScope = m_scope;
				m_result = m_reportRT.EvaluateTextBoxValueExpression(m_textBox);
				m_reportRT.CurrentScope = currentScope;
				m_reportRT.ObjectType = objectType;
				m_reportRT.ObjectName = objectName;
				m_reportRT.PropertyName = propertyName;
				m_isVisited = false;
				m_isValueReady = true;
			}
			return m_result;
		}

		internal void Reset()
		{
			m_isValueReady = false;
		}
	}
}
