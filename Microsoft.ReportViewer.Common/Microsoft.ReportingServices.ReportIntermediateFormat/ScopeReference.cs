namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal sealed class ScopeReference
	{
		private readonly string m_scopeName;

		private readonly string m_fieldName;

		public string ScopeName => m_scopeName;

		public string FieldName => m_fieldName;

		public bool HasFieldName => m_fieldName != null;

		public ScopeReference(string scopeName)
			: this(scopeName, null)
		{
		}

		public ScopeReference(string scopeName, string fieldName)
		{
			m_scopeName = scopeName;
			m_fieldName = fieldName;
		}
	}
}
