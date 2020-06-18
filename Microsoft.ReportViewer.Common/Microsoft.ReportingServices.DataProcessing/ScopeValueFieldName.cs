namespace Microsoft.ReportingServices.DataProcessing
{
	internal class ScopeValueFieldName
	{
		private readonly string m_fieldName;

		private readonly object m_value;

		internal object ScopeValue => m_value;

		internal string FieldName => m_fieldName;

		internal ScopeValueFieldName(string fieldName, object value)
		{
			m_fieldName = fieldName;
			m_value = value;
		}
	}
}
