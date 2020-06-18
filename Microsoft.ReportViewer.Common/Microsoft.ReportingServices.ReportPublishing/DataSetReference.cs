namespace Microsoft.ReportingServices.ReportPublishing
{
	internal sealed class DataSetReference
	{
		private string m_dataSet;

		private string m_valueAlias;

		private string m_labelAlias;

		internal string DataSet => m_dataSet;

		internal string ValueAlias => m_valueAlias;

		internal string LabelAlias => m_labelAlias;

		internal DataSetReference(string dataSet, string valueAlias, string labelAlias)
		{
			m_dataSet = dataSet;
			m_valueAlias = valueAlias;
			m_labelAlias = labelAlias;
		}
	}
}
