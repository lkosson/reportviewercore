using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportRendering;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class Report : IDefinitionPath, IReportScope
	{
		public enum DataElementStyles
		{
			Attribute,
			Element
		}

		public enum SnapshotPageSize
		{
			Unknown,
			Small,
			Large
		}

		public enum ChunkTypes
		{
			Interactivity = 6,
			Pagination,
			Rendering
		}

		private const int m_bytesPerPage = 1000000;

		private bool m_isOldSnapshot;

		private bool m_subreportInSubtotal;

		private IDefinitionPath m_parentDefinitionPath;

		private Microsoft.ReportingServices.ReportIntermediateFormat.Report m_reportDef;

		private Microsoft.ReportingServices.ReportRendering.Report m_renderReport;

		private RenderingContext m_renderingContext;

		private string m_name;

		private string m_description;

		private ReportSectionCollection m_reportSections;

		private DataSetCollection m_dataSets;

		private ReportParameterCollection m_parameters;

		private Microsoft.ReportingServices.ReportIntermediateFormat.ReportInstance m_reportInstance;

		private ReportInstance m_instance;

		private CustomPropertyCollection m_customProperties;

		private PageEvaluation m_pageEvaluation;

		private ReportUrl m_location;

		private ReportStringProperty m_language;

		private ReportStringProperty m_initialPageName;

		private DocumentMap m_cachedDocumentMap;

		private bool m_cachedNeedsOverallTotalPages;

		private bool m_cachedNeedsPageBreakTotalPages;

		private bool m_cachedNeedsReportItemsOnPage;

		private bool m_hasCachedHeaderFooterFlags;

		private RenderingContext m_headerFooterRenderingContext;

		public string DefinitionPath
		{
			get
			{
				if (m_parentDefinitionPath != null)
				{
					return m_parentDefinitionPath.DefinitionPath + "xS";
				}
				return "xA";
			}
		}

		public IDefinitionPath ParentDefinitionPath => m_parentDefinitionPath;

		public string ID
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return m_renderReport.Body.ID + "xB";
				}
				return ReportDef.RenderingModelID;
			}
		}

		public string Name => m_name;

		public string Description => m_description;

		public string Author
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return m_renderReport.Author;
				}
				return m_reportDef.Author;
			}
		}

		public string DefaultFontFamily => m_reportDef.DefaultFontFamily;

		public string DataSetName
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return m_renderReport.DataSetName;
				}
				return m_reportDef.OneDataSetName;
			}
		}

		[Obsolete("Use ReportSection.NeedsHeaderFooterEvaluation instead.")]
		public bool NeedsHeaderFooterEvaluation
		{
			get
			{
				ReportSection firstSection = FirstSection;
				if (!firstSection.NeedsTotalPages)
				{
					return firstSection.NeedsReportItemsOnPage;
				}
				return true;
			}
		}

		public bool NeedsTotalPages
		{
			get
			{
				if (!NeedsPageBreakTotalPages)
				{
					return NeedsOverallTotalPages;
				}
				return true;
			}
		}

		public bool NeedsPageBreakTotalPages
		{
			get
			{
				CacheHeaderFooterFlags();
				return m_cachedNeedsPageBreakTotalPages;
			}
		}

		public bool NeedsOverallTotalPages
		{
			get
			{
				CacheHeaderFooterFlags();
				return m_cachedNeedsOverallTotalPages;
			}
		}

		public bool NeedsReportItemsOnPage
		{
			get
			{
				CacheHeaderFooterFlags();
				return m_cachedNeedsReportItemsOnPage;
			}
		}

		public int AutoRefresh
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return m_renderReport.AutoRefresh;
				}
				return Instance.AutoRefresh;
			}
		}

		[Obsolete("Use ReportSection.Width instead.")]
		public ReportSize Width => FirstSection.Width;

		internal DataSetCollection DataSets
		{
			get
			{
				if (m_dataSets == null)
				{
					if (m_isOldSnapshot)
					{
						return null;
					}
					m_dataSets = new DataSetCollection(m_reportDef, m_renderingContext);
				}
				return m_dataSets;
			}
		}

		[Obsolete("Use ReportSection.Body instead.")]
		public Body Body => FirstSection.Body;

		[Obsolete("Use ReportSection.Page instead.")]
		public Page Page => FirstSection.Page;

		public ReportSectionCollection ReportSections
		{
			get
			{
				if (m_reportSections == null)
				{
					if (m_isOldSnapshot)
					{
						m_reportSections = new ReportSectionCollection(this, m_renderReport);
					}
					else
					{
						m_reportSections = new ReportSectionCollection(this);
					}
				}
				return m_reportSections;
			}
		}

		public ReportParameterCollection Parameters
		{
			get
			{
				if (m_parameters == null)
				{
					if (m_isOldSnapshot)
					{
						if (m_renderReport.ReportDef.Parameters != null)
						{
							m_parameters = new ReportParameterCollection(m_renderReport.ReportDef.Parameters, m_renderReport.Parameters);
						}
					}
					else if (m_reportDef.Parameters != null)
					{
						m_parameters = new ReportParameterCollection(m_renderingContext.OdpContext, m_reportDef.Parameters, m_reportInstance != null || m_renderingContext.OdpContext.ContextMode == OnDemandProcessingContext.Mode.DefinitionOnly);
					}
				}
				return m_parameters;
			}
		}

		public CustomPropertyCollection CustomProperties
		{
			get
			{
				if (m_customProperties == null)
				{
					if (m_isOldSnapshot)
					{
						m_customProperties = new CustomPropertyCollection(m_renderingContext, m_renderReport.CustomProperties);
					}
					else
					{
						m_customProperties = new CustomPropertyCollection(Instance, m_renderingContext, null, m_reportDef, Microsoft.ReportingServices.ReportProcessing.ObjectType.Report, m_reportDef.Name);
					}
				}
				return m_customProperties;
			}
		}

		public string DataTransform
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return m_renderReport.DataTransform;
				}
				return m_reportDef.DataTransform;
			}
		}

		public string DataSchema
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return m_renderReport.DataSchema;
				}
				return m_reportDef.DataSchema;
			}
		}

		public string DataElementName
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return m_renderReport.DataElementName;
				}
				return m_reportDef.DataElementName;
			}
		}

		public DataElementStyles DataElementStyle
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return (DataElementStyles)m_renderReport.DataElementStyle;
				}
				if (!m_reportDef.DataElementStyleAttribute)
				{
					return DataElementStyles.Element;
				}
				return DataElementStyles.Attribute;
			}
		}

		public bool HasDocumentMap
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return m_renderReport.RenderingContext.ReportSnapshot.HasDocumentMap;
				}
				return m_renderingContext.ReportSnapshot.HasDocumentMap;
			}
		}

		public DocumentMap DocumentMap
		{
			get
			{
				if (!HasDocumentMap)
				{
					return null;
				}
				if (m_cachedDocumentMap != null && !m_cachedDocumentMap.IsClosed)
				{
					m_cachedDocumentMap.Reset();
				}
				else
				{
					m_cachedDocumentMap = null;
					if (m_isOldSnapshot)
					{
						Microsoft.ReportingServices.ReportProcessing.DocumentMapNode documentMap = m_renderReport.RenderingContext.ReportSnapshot.GetDocumentMap(m_renderReport.RenderingContext.ChunkManager);
						if (documentMap == null)
						{
							return null;
						}
						m_cachedDocumentMap = new ShimDocumentMap(documentMap);
					}
					else
					{
						OnDemandProcessingContext odpContext = RenderingContext.OdpContext;
						Stream stream = Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.OnDemandProcessingManager.OpenExistingDocumentMapStream(odpContext.OdpMetadata, odpContext.TopLevelContext.ReportContext, odpContext.ChunkFactory);
						if (stream == null)
						{
							NullRenderer nullRenderer = new NullRenderer();
							nullRenderer.Process(this, RenderingContext.OdpContext, generateDocumentMap: true, createSnapshot: false);
							stream = nullRenderer.DocumentMapStream;
							if (stream == null)
							{
								RenderingContext.ReportSnapshot.HasDocumentMap = false;
								m_cachedDocumentMap = null;
							}
							else
							{
								stream.Seek(0L, SeekOrigin.Begin);
								DocumentMapReader aReader = new DocumentMapReader(stream);
								m_cachedDocumentMap = new InternalDocumentMap(aReader);
							}
						}
						else
						{
							DocumentMapReader aReader2 = new DocumentMapReader(stream);
							m_cachedDocumentMap = new InternalDocumentMap(aReader2);
						}
					}
				}
				return m_cachedDocumentMap;
			}
		}

		public bool HasBookmarks
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return m_renderReport.RenderingContext.ReportSnapshot.HasBookmarks;
				}
				return RenderingContext.OdpContext.HasBookmarks;
			}
		}

		public ReportUrl Location
		{
			get
			{
				if (m_location == null)
				{
					if (m_isOldSnapshot)
					{
						if (m_renderReport.Location != null)
						{
							m_location = new ReportUrl(m_renderReport.Location);
						}
					}
					else
					{
						m_location = new ReportUrl(m_renderingContext.OdpContext.ReportContext, null);
					}
				}
				return m_location;
			}
		}

		public DateTime ExecutionTime
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return m_renderReport.RenderingContext.ExecutionTime;
				}
				return m_renderingContext.OdpContext.ExecutionTime;
			}
		}

		public ReportStringProperty Language
		{
			get
			{
				if (m_language == null)
				{
					if (m_isOldSnapshot)
					{
						Microsoft.ReportingServices.ReportProcessing.ExpressionInfo language = m_renderReport.ReportDef.Language;
						if (language == null)
						{
							m_language = new ReportStringProperty(isExpression: false, m_renderReport.ReportLanguage, m_renderReport.ReportLanguage);
						}
						else
						{
							m_language = new ReportStringProperty(language);
						}
					}
					else
					{
						Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo language2 = m_reportDef.Language;
						if (language2 == null)
						{
							string text = Localization.DefaultReportServerSpecificCulture.ToString();
							m_language = new ReportStringProperty(isExpression: false, text, text);
						}
						else
						{
							m_language = new ReportStringProperty(language2);
						}
					}
				}
				return m_language;
			}
		}

		public bool ConsumeContainerWhitespace
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return true;
				}
				return m_reportDef.ConsumeContainerWhitespace;
			}
		}

		public string ShowHideToggle => GetRSRequestParameters()?.ShowHideToggleParamValue;

		public string SortItem => GetRSRequestParameters()?.SortIdParamValue;

		public SnapshotPageSize SnapshotPageSizeInfo
		{
			get
			{
				if (m_isOldSnapshot)
				{
					if (m_renderReport.ReportDef.MainChunkSize <= 0 || m_renderReport.ReportInstance.NumberOfPages <= 0)
					{
						return SnapshotPageSize.Unknown;
					}
					if (1000000 < m_renderReport.ReportDef.MainChunkSize / m_renderReport.ReportInstance.NumberOfPages)
					{
						return SnapshotPageSize.Large;
					}
					return SnapshotPageSize.Small;
				}
				return SnapshotPageSize.Unknown;
			}
		}

		internal PageEvaluation PageEvaluation => m_pageEvaluation;

		internal RenderingContext HeaderFooterRenderingContext
		{
			get
			{
				if (m_headerFooterRenderingContext == null)
				{
					m_headerFooterRenderingContext = new RenderingContext(m_renderingContext, NeedsReportItemsOnPage);
				}
				return m_headerFooterRenderingContext;
			}
		}

		internal IJobContext JobContext
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return m_renderReport.RenderingContext.JobContext;
				}
				return RenderingContext.OdpContext.JobContext;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.Report ReportDef
		{
			get
			{
				if (m_isOldSnapshot)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				return m_reportDef;
			}
		}

		internal RenderingContext RenderingContext => m_renderingContext;

		internal bool IsOldSnapshot => m_isOldSnapshot;

		internal Microsoft.ReportingServices.ReportRendering.Report RenderReport
		{
			get
			{
				if (!m_isOldSnapshot)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				return m_renderReport;
			}
		}

		internal bool SubreportInSubtotal => m_subreportInSubtotal;

		IReportScopeInstance IReportScope.ReportScopeInstance => Instance;

		IRIFReportScope IReportScope.RIFReportScope => m_reportDef;

		internal ReportSection FirstSection => ReportSections[0];

		public ReportStringProperty InitialPageName
		{
			get
			{
				if (m_initialPageName == null)
				{
					if (m_isOldSnapshot)
					{
						m_initialPageName = new ReportStringProperty();
					}
					else
					{
						m_initialPageName = new ReportStringProperty(m_reportDef.InitialPageName);
					}
				}
				return m_initialPageName;
			}
		}

		public ReportInstance Instance
		{
			get
			{
				if (RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					if (m_isOldSnapshot)
					{
						m_instance = new ReportInstance(this);
					}
					else
					{
						m_instance = new ReportInstance(this, m_reportInstance);
					}
				}
				return m_instance;
			}
		}

		internal Report(Microsoft.ReportingServices.ReportIntermediateFormat.Report reportDef, Microsoft.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance, RenderingContext renderingContext, string reportName, string description)
		{
			m_parentDefinitionPath = null;
			m_isOldSnapshot = false;
			m_reportDef = reportDef;
			m_reportInstance = reportInstance;
			m_renderingContext = renderingContext;
			m_name = reportName;
			m_description = description;
			if (reportDef.HasHeadersOrFooters)
			{
				m_pageEvaluation = new OnDemandPageEvaluation(this);
				m_renderingContext.SetPageEvaluation(m_pageEvaluation);
			}
		}

		internal Report(Microsoft.ReportingServices.ReportIntermediateFormat.Report reportDef, RenderingContext renderingContext, string reportName, string description)
		{
			m_parentDefinitionPath = null;
			m_isOldSnapshot = false;
			m_reportDef = reportDef;
			m_reportInstance = null;
			m_renderingContext = renderingContext;
			m_name = reportName;
			m_description = description;
		}

		internal Report(Microsoft.ReportingServices.ReportProcessing.Report reportDef, Microsoft.ReportingServices.ReportProcessing.ReportInstance reportInstance, Microsoft.ReportingServices.ReportRendering.RenderingContext oldRenderingContext, RenderingContext renderingContext, string reportName, string description)
		{
			m_renderReport = new Microsoft.ReportingServices.ReportRendering.Report(reportDef, reportInstance, oldRenderingContext, reportName, description, Localization.DefaultReportServerSpecificCulture);
			m_parentDefinitionPath = null;
			m_isOldSnapshot = true;
			m_subreportInSubtotal = false;
			m_renderingContext = renderingContext;
			m_name = reportName;
			m_description = description;
			if (m_renderReport.NeedsHeaderFooterEvaluation)
			{
				m_pageEvaluation = new ShimPageEvaluation(this);
				m_renderingContext.SetPageEvaluation(m_pageEvaluation);
			}
		}

		internal Report(IDefinitionPath parentDefinitionPath, Microsoft.ReportingServices.ReportIntermediateFormat.Report reportDef, Microsoft.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance, RenderingContext renderingContext, string reportName, string description, bool subreportInSubtotal)
		{
			m_parentDefinitionPath = parentDefinitionPath;
			m_isOldSnapshot = false;
			m_reportDef = reportDef;
			m_reportInstance = reportInstance;
			m_isOldSnapshot = false;
			m_subreportInSubtotal = subreportInSubtotal;
			m_renderingContext = renderingContext;
			m_name = reportName;
			m_description = description;
			m_pageEvaluation = null;
		}

		internal Report(IDefinitionPath parentDefinitionPath, bool subreportInSubtotal, Microsoft.ReportingServices.ReportRendering.SubReport subReport, RenderingContext renderingContext)
		{
			m_parentDefinitionPath = parentDefinitionPath;
			m_renderReport = subReport.Report;
			m_isOldSnapshot = true;
			m_subreportInSubtotal = subreportInSubtotal;
			if (m_renderReport != null)
			{
				m_name = m_renderReport.Name;
				m_description = m_renderReport.Description;
			}
			m_renderingContext = new RenderingContext(renderingContext);
			m_pageEvaluation = null;
		}

		[Obsolete("Use ReportSection.SetPage(Int32, Int32) instead.")]
		public void SetPage(int pageNumber, int totalPages)
		{
			FirstSection.SetPage(pageNumber, totalPages);
		}

		[Obsolete("Use ReportSection.GetPageSections() instead.")]
		public void GetPageSections()
		{
			FirstSection.GetPageSections();
		}

		public void AddToCurrentPage(string textboxDefinitionName, object textboxInstanceOriginalValue)
		{
			if (m_pageEvaluation != null)
			{
				m_pageEvaluation.Add(textboxDefinitionName, textboxInstanceOriginalValue);
			}
		}

		public void EnableNativeCustomReportItem()
		{
			if (IsOldSnapshot)
			{
				m_renderReport.RenderingContext.NativeCRITypes = null;
				m_renderReport.RenderingContext.NativeAllCRITypes = true;
			}
			else
			{
				m_renderingContext.NativeCRITypes = null;
				m_renderingContext.NativeAllCRITypes = true;
			}
		}

		public void EnableNativeCustomReportItem(string type)
		{
			if (type == null)
			{
				EnableNativeCustomReportItem();
			}
			else if (IsOldSnapshot)
			{
				if (m_renderReport.RenderingContext.NativeCRITypes == null)
				{
					m_renderReport.RenderingContext.NativeCRITypes = new Hashtable();
				}
				if (!m_renderReport.RenderingContext.NativeCRITypes.ContainsKey(type))
				{
					m_renderReport.RenderingContext.NativeCRITypes.Add(type, null);
				}
			}
			else
			{
				if (m_renderingContext.NativeCRITypes == null)
				{
					m_renderingContext.NativeCRITypes = new Hashtable();
				}
				if (!m_renderingContext.NativeCRITypes.ContainsKey(type))
				{
					m_renderingContext.NativeCRITypes.Add(type, null);
				}
			}
		}

		public Stream GetOrCreateChunk(ChunkTypes type, string chunkName, out bool isNewChunk)
		{
			return m_renderingContext.GetOrCreateChunk((Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes)type, chunkName, createChunkIfNotExists: true, out isNewChunk);
		}

		public Stream CreateChunk(ChunkTypes type, string chunkName)
		{
			return m_renderingContext.CreateChunk((Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes)type, chunkName);
		}

		public Stream GetChunk(ChunkTypes type, string chunkName)
		{
			bool isNewChunk;
			return m_renderingContext.GetOrCreateChunk((Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes)type, chunkName, createChunkIfNotExists: false, out isNewChunk);
		}

		private void CacheHeaderFooterFlags()
		{
			if (m_hasCachedHeaderFooterFlags)
			{
				return;
			}
			m_cachedNeedsPageBreakTotalPages = false;
			m_cachedNeedsReportItemsOnPage = false;
			m_cachedNeedsOverallTotalPages = false;
			foreach (ReportSection reportSection in ReportSections)
			{
				m_cachedNeedsReportItemsOnPage |= reportSection.NeedsReportItemsOnPage;
				m_cachedNeedsOverallTotalPages |= reportSection.NeedsOverallTotalPages;
				m_cachedNeedsPageBreakTotalPages |= reportSection.NeedsPageBreakTotalPages;
			}
			m_hasCachedHeaderFooterFlags = true;
		}

		public string GetReportUrl(bool addReportParameters)
		{
			if (m_isOldSnapshot)
			{
				return new ReportUrlBuilder(m_renderReport.RenderingContext, null, useReplacementRoot: true, addReportParameters).ToString();
			}
			CatalogItemUrlBuilder catalogItemUrlBuilder = new CatalogItemUrlBuilder(m_renderingContext.OdpContext.ReportContext);
			if (addReportParameters && Parameters != null)
			{
				NameValueCollection toNameValueCollection = Parameters.ToNameValueCollection;
				catalogItemUrlBuilder.AppendReportParameters(toNameValueCollection);
			}
			return catalogItemUrlBuilder.ToString();
		}

		public string GetStreamUrl(bool useSessionId, string streamName)
		{
			ICatalogItemContext catalogItemContext = m_isOldSnapshot ? m_renderReport.RenderingContext.TopLevelReportContext : m_renderingContext.OdpContext.ReportContext;
			_ = catalogItemContext.HostSpecificItemPath;
			_ = catalogItemContext.HostRootUri;
			return catalogItemContext.RSRequestParameters.GetImageUrl(useSessionId, streamName, catalogItemContext);
		}

		public bool GetResource(string resourcePath, out byte[] resource, out string mimeType)
		{
			resource = null;
			mimeType = null;
			if (m_isOldSnapshot)
			{
				if (m_renderReport.RenderingContext.GetResourceCallback != null)
				{
					m_renderReport.RenderingContext.GetResourceCallback.GetResource(m_renderReport.RenderingContext.CurrentReportContext, resourcePath, out resource, out mimeType, out bool _, out bool _);
					return true;
				}
				return false;
			}
			bool registerInvalidSizeWarning2;
			return m_renderingContext.OdpContext.GetResource(resourcePath, out resource, out mimeType, out registerInvalidSizeWarning2);
		}

		private RSRequestParameters GetRSRequestParameters()
		{
			if (m_isOldSnapshot)
			{
				return m_renderReport.RenderingContext.TopLevelReportContext.RSRequestParameters;
			}
			return RenderingContext.OdpContext.TopLevelContext.ReportContext.RSRequestParameters;
		}

		internal void UpdateSubReportContents(SubReport subReport, Microsoft.ReportingServices.ReportRendering.SubReport renderSubreport)
		{
			if (renderSubreport != null)
			{
				m_renderReport = renderSubreport.Report;
			}
			if (m_reportSections != null)
			{
				m_reportSections[0].UpdateSubReportContents(m_renderReport);
			}
			if (m_parameters != null)
			{
				m_parameters.UpdateRenderReportItem(m_renderReport.Parameters);
			}
		}

		internal void SetNewContext(Microsoft.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance)
		{
			m_reportInstance = reportInstance;
			SetNewContext();
		}

		internal void SetNewContext()
		{
			if (m_reportSections != null)
			{
				m_reportSections.SetNewContext();
			}
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
			if (m_parameters != null)
			{
				m_parameters.SetNewContext(m_reportInstance != null);
			}
			if (m_dataSets != null)
			{
				m_dataSets.SetNewContext();
			}
		}
	}
}
