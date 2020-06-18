using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Specialized;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class ReportParameterCollection : NameObjectCollectionBase
	{
		private NameValueCollection m_asNameValueCollection;

		private bool m_isValid;

		public ReportParameter this[string name] => (ReportParameter)BaseGet(name);

		public ReportParameter this[int index] => (ReportParameter)BaseGet(index);

		public NameValueCollection AsNameValueCollection
		{
			get
			{
				if (m_asNameValueCollection == null)
				{
					int count = Count;
					m_asNameValueCollection = new NameValueCollection(count, StringComparer.Ordinal);
					for (int i = 0; i < count; i++)
					{
						ReportParameter reportParameter = this[i];
						m_asNameValueCollection.Add(reportParameter.Name, reportParameter.StringValues);
					}
				}
				return m_asNameValueCollection;
			}
		}

		public bool IsValid => m_isValid;

		internal ReportParameterCollection(ParameterInfoCollection parameters)
		{
			Init(parameters, isValid: true);
		}

		internal ReportParameterCollection(ParameterInfoCollection parameters, bool isValid)
		{
			Init(parameters, isValid);
		}

		private void Init(ParameterInfoCollection parameters, bool isValid)
		{
			m_isValid = isValid;
			int count = parameters.Count;
			for (int i = 0; i < count; i++)
			{
				ParameterInfo parameterInfo = parameters[i];
				if (parameterInfo.PromptUser)
				{
					BaseAdd(parameterInfo.Name, new ReportParameter(parameterInfo));
				}
			}
		}
	}
}
