using Microsoft.ReportingServices.DataExtensions;
using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class PublishingResult
	{
		private string m_reportDescription;

		private string m_reportLanguage;

		private ParameterInfoCollection m_parameters;

		private DataSourceInfoCollection m_dataSources;

		private ProcessingMessageList m_warnings;

		private UserLocationFlags m_userReferenceLocation;

		private PageProperties m_pageProperties;

		private string[] m_dataSetsName;

		private bool m_hasExternalImages;

		private bool m_hasHyperlinks;

		private ReportProcessingFlags m_reportProcessingFlags;

		private byte[] m_dataSetsHash;

		private DataSetInfoCollection m_sharedDataSets;

		public string ReportDescription => m_reportDescription;

		public string ReportLanguage => m_reportLanguage;

		public bool HasUserProfileQueryDependencies
		{
			get
			{
				if ((m_userReferenceLocation & UserLocationFlags.ReportQueries) == 0)
				{
					return false;
				}
				return true;
			}
		}

		public bool HasUserProfileReportDependencies
		{
			get
			{
				if ((m_userReferenceLocation & UserLocationFlags.ReportBody) == 0 && (m_userReferenceLocation & UserLocationFlags.ReportPageSection) == 0)
				{
					return false;
				}
				return true;
			}
		}

		public ParameterInfoCollection Parameters => m_parameters;

		public DataSourceInfoCollection DataSources => m_dataSources;

		public ProcessingMessageList Warnings => m_warnings;

		public PageProperties PageProperties => m_pageProperties;

		public string[] DataSetsName => m_dataSetsName;

		public bool HasExternalImages => m_hasExternalImages;

		public bool HasHyperlinks => m_hasHyperlinks;

		public ReportProcessingFlags ReportProcessingFlags => m_reportProcessingFlags;

		public byte[] DataSetsHash => m_dataSetsHash;

		public DataSetInfoCollection SharedDataSets => m_sharedDataSets;

		internal PublishingResult(string reportDescription, string reportLanguage, ParameterInfoCollection parameters, DataSourceInfoCollection dataSources, DataSetInfoCollection sharedDataSetReferences, ProcessingMessageList warnings, UserLocationFlags userReferenceLocation, double pageHeight, double pageWidth, double topMargin, double bottomMargin, double leftMargin, double rightMargin, ArrayList dataSetsName, bool hasExternalImages, bool hasHyperlinks, ReportProcessingFlags reportProcessingFlags, byte[] dataSetsHash)
		{
			m_reportDescription = reportDescription;
			m_reportLanguage = reportLanguage;
			m_parameters = parameters;
			m_dataSources = dataSources;
			m_sharedDataSets = sharedDataSetReferences;
			m_warnings = warnings;
			m_userReferenceLocation = userReferenceLocation;
			m_hasExternalImages = hasExternalImages;
			m_hasHyperlinks = hasHyperlinks;
			m_reportProcessingFlags = reportProcessingFlags;
			m_dataSetsHash = dataSetsHash;
			m_pageProperties = new PageProperties(pageHeight, pageWidth, topMargin, bottomMargin, leftMargin, rightMargin);
			if (dataSetsName != null && dataSetsName.Count > 0)
			{
				m_dataSetsName = (string[])dataSetsName.ToArray(typeof(string));
			}
		}
	}
}
