using Microsoft.ReportingServices.DataExtensions;
using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System;

namespace Microsoft.ReportingServices.ReportPublishing
{
	[Serializable]
	internal abstract class PublishingContextBase
	{
		private readonly bool m_isRdlx;

		private readonly PublishingContextKind m_publishingContextKind;

		private readonly ICatalogItemContext m_catalogContext;

		private readonly IChunkFactory m_createChunkFactory;

		private readonly AppDomain m_compilationTempAppDomain;

		private readonly bool m_generateExpressionHostWithRefusedPermissions;

		private ReportProcessingFlags m_processingFlags;

		private readonly Microsoft.ReportingServices.ReportProcessing.ReportProcessing.CheckSharedDataSource m_checkDataSourceCallback;

		private readonly Microsoft.ReportingServices.ReportProcessing.ReportProcessing.CheckSharedDataSet m_checkDataSetCallback;

		private readonly Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ResolveTemporaryDataSource m_resolveTemporaryDataSourceCallback;

		private readonly Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ResolveTemporaryDataSet m_resolveTemporaryDataSetCallback;

		private readonly DataSourceInfoCollection m_originalDataSources;

		private readonly DataSetInfoCollection m_originalDataSets;

		private readonly IConfiguration m_configuration;

		private readonly IDataProtection m_dataProtection;

		private readonly bool m_isInternalRepublish;

		private readonly bool m_traceAtomicScopes;

		private readonly bool m_isPackagedReportArchive;

		private readonly PublishingVersioning m_publishingVersioning;

		internal PublishingVersioning PublishingVersioning => m_publishingVersioning;

		internal bool IsInternalRepublish => m_isInternalRepublish;

		internal PublishingContextKind PublishingContextKind => m_publishingContextKind;

		internal ICatalogItemContext CatalogContext => m_catalogContext;

		internal IChunkFactory CreateChunkFactory => m_createChunkFactory;

		internal AppDomain CompilationTempAppDomain => m_compilationTempAppDomain;

		internal bool GenerateExpressionHostWithRefusedPermissions => m_generateExpressionHostWithRefusedPermissions;

		internal ReportProcessingFlags ProcessingFlags
		{
			get
			{
				return m_processingFlags;
			}
			set
			{
				m_processingFlags = value;
			}
		}

		internal Microsoft.ReportingServices.ReportProcessing.ReportProcessing.CheckSharedDataSource CheckDataSourceCallback => m_checkDataSourceCallback;

		internal Microsoft.ReportingServices.ReportProcessing.ReportProcessing.CheckSharedDataSet CheckDataSetCallback => m_checkDataSetCallback;

		internal Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ResolveTemporaryDataSource ResolveTemporaryDataSourceCallback => m_resolveTemporaryDataSourceCallback;

		internal Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ResolveTemporaryDataSet ResolveTemporaryDataSetCallback => m_resolveTemporaryDataSetCallback;

		internal DataSourceInfoCollection OriginalDataSources => m_originalDataSources;

		internal DataSetInfoCollection OriginalDataSets => m_originalDataSets;

		internal IConfiguration Configuration => m_configuration;

		internal IDataProtection DataProtection => m_dataProtection;

		internal bool TraceAtomicScopes => m_traceAtomicScopes;

		internal bool IsRdlx => m_isRdlx;

		internal bool IsPackagedReportArchive => m_isPackagedReportArchive;

		internal bool IsRdlSandboxingEnabled
		{
			get
			{
				if (Configuration != null)
				{
					return Configuration.RdlSandboxing != null;
				}
				return false;
			}
		}

		protected PublishingContextBase(PublishingContextKind publishingContextKind, ICatalogItemContext catalogContext, IChunkFactory createChunkFactory, AppDomain compilationTempAppDomain, bool generateExpressionHostWithRefusedPermissions, ReportProcessingFlags processingFlags, Microsoft.ReportingServices.ReportProcessing.ReportProcessing.CheckSharedDataSource checkDataSourceCallback, Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ResolveTemporaryDataSource resolveTemporaryDataSourceCallback, DataSourceInfoCollection originalDataSources, Microsoft.ReportingServices.ReportProcessing.ReportProcessing.CheckSharedDataSet checkDataSetCallback, Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ResolveTemporaryDataSet resolveTemporaryDataSetCallback, DataSetInfoCollection originalDataSets, IConfiguration configuration, IDataProtection dataProtection, bool isInternalRepublish, bool isPackagedReportArchive, bool isRdlx, bool traceAtomicScopes)
		{
			m_publishingContextKind = publishingContextKind;
			m_catalogContext = catalogContext;
			m_createChunkFactory = createChunkFactory;
			m_compilationTempAppDomain = compilationTempAppDomain;
			m_generateExpressionHostWithRefusedPermissions = generateExpressionHostWithRefusedPermissions;
			m_processingFlags = processingFlags;
			m_checkDataSourceCallback = checkDataSourceCallback;
			m_checkDataSetCallback = checkDataSetCallback;
			m_resolveTemporaryDataSourceCallback = resolveTemporaryDataSourceCallback;
			m_resolveTemporaryDataSetCallback = resolveTemporaryDataSetCallback;
			m_originalDataSources = originalDataSources;
			m_originalDataSets = originalDataSets;
			m_configuration = configuration;
			m_dataProtection = dataProtection;
			m_isInternalRepublish = isInternalRepublish;
			m_traceAtomicScopes = traceAtomicScopes;
			m_isPackagedReportArchive = isPackagedReportArchive;
			m_isRdlx = isRdlx;
			m_publishingVersioning = new PublishingVersioning(m_configuration, this);
		}

		internal bool IsRestrictedDataRegionSort(bool isDataRowSort)
		{
			if (isDataRowSort)
			{
				return m_publishingVersioning.IsRdlFeatureRestricted(RdlFeatures.Sort_DataRegion);
			}
			return false;
		}

		internal bool IsRestrictedGroupSort(bool isDataRowSort, Microsoft.ReportingServices.ReportIntermediateFormat.Sorting sorting)
		{
			if (!sorting.NaturalSort && !sorting.DeferredSort)
			{
				return m_publishingVersioning.IsRdlFeatureRestricted(RdlFeatures.Sort_Group_Applied);
			}
			return false;
		}

		internal bool IsRestrictedNaturalGroupSort(Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo)
		{
			if (expressionInfo.Type != Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Field)
			{
				return m_publishingVersioning.IsRdlFeatureRestricted(RdlFeatures.SortGroupExpression_OnlySimpleField);
			}
			return false;
		}
	}
}
