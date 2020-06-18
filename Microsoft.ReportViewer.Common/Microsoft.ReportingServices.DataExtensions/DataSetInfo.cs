using System;

namespace Microsoft.ReportingServices.DataExtensions
{
	[Serializable]
	internal sealed class DataSetInfo
	{
		private Guid m_id = Guid.Empty;

		private Guid m_linkSharedDataSetId = Guid.Empty;

		private string m_absolutePath;

		private string m_dataSetName;

		private byte[] m_secDesc;

		private Guid m_compiledDefinitionId = Guid.Empty;

		private Guid m_dataSourceId = Guid.Empty;

		private string m_parameters;

		private bool m_referenceValid;

		private readonly byte[] m_definition;

		public Guid ID
		{
			get
			{
				return m_id;
			}
			set
			{
				m_id = value;
			}
		}

		public Guid LinkedSharedDataSetID
		{
			get
			{
				return m_linkSharedDataSetId;
			}
			set
			{
				m_linkSharedDataSetId = value;
			}
		}

		public string AbsolutePath
		{
			get
			{
				return m_absolutePath;
			}
			set
			{
				m_absolutePath = value;
			}
		}

		public string DataSetName
		{
			get
			{
				return m_dataSetName;
			}
			set
			{
				m_dataSetName = value;
			}
		}

		public byte[] SecurityDescriptor => m_secDesc;

		public Guid CompiledDefinitionId => m_compiledDefinitionId;

		public Guid DataSourceId
		{
			get
			{
				return m_dataSourceId;
			}
			set
			{
				m_dataSourceId = value;
			}
		}

		public byte[] Definition => m_definition;

		public string ParametersXml => m_parameters;

		public DataSetInfo(Guid id, Guid linkId, string name, string absolutePath, byte[] secDesc, Guid compiledDefinitionId, string parameters)
		{
			m_id = id;
			m_linkSharedDataSetId = linkId;
			m_dataSetName = name;
			m_absolutePath = absolutePath;
			m_secDesc = secDesc;
			m_compiledDefinitionId = compiledDefinitionId;
			m_referenceValid = (m_linkSharedDataSetId != Guid.Empty);
			m_parameters = parameters;
		}

		public DataSetInfo(string reportDataSetName, string absolutePath, Guid linkId)
		{
			m_id = Guid.NewGuid();
			m_dataSetName = reportDataSetName;
			m_absolutePath = absolutePath;
			m_linkSharedDataSetId = linkId;
			m_referenceValid = (m_linkSharedDataSetId != Guid.Empty);
		}

		public DataSetInfo(string reportDataSetName, string absolutePath)
		{
			m_id = Guid.NewGuid();
			m_dataSetName = reportDataSetName;
			m_absolutePath = absolutePath;
			m_linkSharedDataSetId = Guid.Empty;
			m_referenceValid = false;
		}

		public DataSetInfo(string reportDataSetName, string absolutePath, byte[] definition)
		{
			if (definition == null)
			{
				throw new ArgumentNullException("definition");
			}
			m_definition = definition;
			m_id = Guid.NewGuid();
			m_dataSetName = reportDataSetName;
			m_absolutePath = absolutePath;
			m_linkSharedDataSetId = Guid.Empty;
			m_referenceValid = true;
		}

		public bool IsValidReference()
		{
			return m_referenceValid;
		}
	}
}
