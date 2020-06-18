using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class Report : IDocumentMapEntry
	{
		public enum DataElementStyles
		{
			AttributeNormal,
			ElementNormal
		}

		private Microsoft.ReportingServices.ReportProcessing.Report m_reportDef;

		private ReportInstance m_reportInstance;

		private ReportInstanceInfo m_reportInstanceInfo;

		private RenderingContext m_renderingContext;

		private Rectangle m_reportBody;

		private PageSection m_pageHeader;

		private PageSection m_pageFooter;

		private PageCollection m_reportPagination;

		private string m_name;

		private string m_description;

		private ReportUrl m_reportUrl;

		private DocumentMapNode m_documentMapRoot;

		private ReportParameterCollection m_reportParameters;

		private string m_reportLanguage;

		private CustomPropertyCollection m_customProperties;

		private Bookmarks m_bookmarksInfo;

		private bool? m_bodyStyleConstainsBorder;

		public string UniqueName
		{
			get
			{
				if (m_reportInstance == null)
				{
					return null;
				}
				return m_reportInstance.UniqueName.ToString(CultureInfo.InvariantCulture);
			}
		}

		public string ShowHideToggle => m_renderingContext.TopLevelReportContext.RSRequestParameters.ShowHideToggleParamValue;

		public string SortItem => m_renderingContext.TopLevelReportContext.RSRequestParameters.SortIdParamValue;

		public bool InDocumentMap => m_renderingContext.ReportSnapshot.HasDocumentMap;

		public bool HasBookmarks => m_renderingContext.ReportSnapshot.HasBookmarks;

		public string Name => m_name;

		internal DocumentMapNode DocumentMap
		{
			get
			{
				DocumentMapNode documentMapNode = m_documentMapRoot;
				if (m_documentMapRoot == null && InDocumentMap)
				{
					documentMapNode = new DocumentMapNode(m_renderingContext.ReportSnapshot.GetDocumentMap(m_renderingContext.ChunkManager));
					if (m_renderingContext.CacheState)
					{
						m_documentMapRoot = documentMapNode;
					}
				}
				return documentMapNode;
			}
		}

		internal Bookmarks ReportBookmarks
		{
			get
			{
				Bookmarks bookmarks = m_bookmarksInfo;
				if (m_bookmarksInfo == null && HasBookmarks)
				{
					bookmarks = new Bookmarks(m_renderingContext.ReportSnapshot.GetBookmarksInfo(m_renderingContext.ChunkManager));
					if (m_renderingContext.CacheState)
					{
						m_bookmarksInfo = bookmarks;
					}
				}
				return bookmarks;
			}
		}

		public string Description => m_description;

		public ReportUrl Location
		{
			get
			{
				ReportUrl reportUrl = m_reportUrl;
				if (m_reportUrl == null)
				{
					reportUrl = new ReportUrl(m_renderingContext, null);
					if (m_renderingContext.CacheState)
					{
						m_reportUrl = reportUrl;
					}
				}
				return reportUrl;
			}
		}

		public string ReportLanguage => m_reportLanguage;

		public bool CacheState
		{
			get
			{
				return m_renderingContext.CacheState;
			}
			set
			{
				m_renderingContext.CacheState = value;
			}
		}

		public DateTime ExecutionTime => m_renderingContext.ExecutionTime;

		public string Author => m_reportDef.Author;

		public string DataSetName => m_reportDef.OneDataSetName;

		public bool NeedsHeaderFooterEvaluation
		{
			get
			{
				if (!m_reportDef.PageHeaderEvaluation)
				{
					return m_reportDef.PageFooterEvaluation;
				}
				return true;
			}
		}

		public PageSection PageHeader
		{
			get
			{
				PageSection pageSection = m_pageHeader;
				if (m_pageHeader == null)
				{
					if (m_reportDef.PageHeader == null)
					{
						return null;
					}
					string text = "ph";
					RenderingContext renderingContext = new RenderingContext(m_renderingContext, text);
					pageSection = new PageSection(text, m_reportDef.PageHeader, null, this, renderingContext, m_reportDef.PageHeaderEvaluation);
					if (m_renderingContext.CacheState)
					{
						m_pageHeader = pageSection;
					}
				}
				return pageSection;
			}
		}

		public PageSection PageFooter
		{
			get
			{
				PageSection pageSection = m_pageFooter;
				if (m_pageFooter == null)
				{
					if (m_reportDef.PageFooter == null)
					{
						return null;
					}
					string text = "pf";
					RenderingContext renderingContext = new RenderingContext(m_renderingContext, text);
					pageSection = new PageSection(text, m_reportDef.PageFooter, null, this, renderingContext, m_reportDef.PageFooterEvaluation);
					if (m_renderingContext.CacheState)
					{
						m_pageFooter = pageSection;
					}
				}
				return pageSection;
			}
		}

		public int AutoRefresh => m_reportDef.AutoRefresh;

		public ReportSize Width
		{
			get
			{
				if (m_reportDef.WidthForRendering == null)
				{
					m_reportDef.WidthForRendering = new ReportSize(m_reportDef.Width, m_reportDef.WidthValue);
				}
				return m_reportDef.WidthForRendering;
			}
		}

		public ReportSize PageHeight
		{
			get
			{
				if (m_reportDef.PageHeightForRendering == null)
				{
					m_reportDef.PageHeightForRendering = new ReportSize(m_reportDef.PageHeight, m_reportDef.PageHeightValue);
				}
				return m_reportDef.PageHeightForRendering;
			}
		}

		public ReportSize PageWidth
		{
			get
			{
				if (m_reportDef.PageWidthForRendering == null)
				{
					m_reportDef.PageWidthForRendering = new ReportSize(m_reportDef.PageWidth, m_reportDef.PageWidthValue);
				}
				return m_reportDef.PageWidthForRendering;
			}
		}

		public int Columns => m_reportDef.Columns;

		public ReportSize ColumnSpacing
		{
			get
			{
				if (m_reportDef.ColumnSpacingForRendering == null)
				{
					m_reportDef.ColumnSpacingForRendering = new ReportSize(m_reportDef.ColumnSpacing, m_reportDef.ColumnSpacingValue);
				}
				return m_reportDef.ColumnSpacingForRendering;
			}
		}

		public PageCollection Pages
		{
			get
			{
				PageCollection pageCollection = m_reportPagination;
				if (m_reportPagination == null)
				{
					pageCollection = new PageCollection(m_renderingContext.RenderingInfoManager.PaginationInfo, this);
					if (m_renderingContext.CacheState)
					{
						m_reportPagination = pageCollection;
					}
				}
				return pageCollection;
			}
		}

		public ReportParameterCollection Parameters
		{
			get
			{
				ReportParameterCollection reportParameterCollection = m_reportParameters;
				if (m_reportInstance != null && m_reportParameters == null)
				{
					reportParameterCollection = new ReportParameterCollection(InstanceInfo.Parameters);
					if (m_renderingContext.CacheState)
					{
						m_reportParameters = reportParameterCollection;
					}
				}
				return reportParameterCollection;
			}
		}

		public Rectangle Body
		{
			get
			{
				Rectangle rectangle = m_reportBody;
				if (m_reportBody == null)
				{
					rectangle = new Rectangle(null, (m_reportInstance != null) ? InstanceInfo.BodyUniqueName : 0, m_reportDef, m_reportInstance, m_renderingContext, null);
					if (m_renderingContext.CacheState)
					{
						m_reportBody = rectangle;
					}
				}
				return rectangle;
			}
		}

		public ReportSize LeftMargin
		{
			get
			{
				if (m_reportDef.LeftMarginForRendering == null)
				{
					m_reportDef.LeftMarginForRendering = new ReportSize(m_reportDef.LeftMargin, m_reportDef.LeftMarginValue);
				}
				return m_reportDef.LeftMarginForRendering;
			}
		}

		public ReportSize RightMargin
		{
			get
			{
				if (m_reportDef.RightMarginForRendering == null)
				{
					m_reportDef.RightMarginForRendering = new ReportSize(m_reportDef.RightMargin, m_reportDef.RightMarginValue);
				}
				return m_reportDef.RightMarginForRendering;
			}
		}

		public ReportSize TopMargin
		{
			get
			{
				if (m_reportDef.TopMarginForRendering == null)
				{
					m_reportDef.TopMarginForRendering = new ReportSize(m_reportDef.TopMargin, m_reportDef.TopMarginValue);
				}
				return m_reportDef.TopMarginForRendering;
			}
		}

		public ReportSize BottomMargin
		{
			get
			{
				if (m_reportDef.BottomMarginForRendering == null)
				{
					m_reportDef.BottomMarginForRendering = new ReportSize(m_reportDef.BottomMargin, m_reportDef.BottomMarginValue);
				}
				return m_reportDef.BottomMarginForRendering;
			}
		}

		public object SharedRenderingInfo
		{
			get
			{
				return m_renderingContext.RenderingInfoManager.SharedRenderingInfo[m_reportDef.ID];
			}
			set
			{
				m_renderingContext.RenderingInfoManager.SharedRenderingInfo[m_reportDef.ID] = value;
			}
		}

		public object RenderingInfo
		{
			get
			{
				if (m_reportInstance == null)
				{
					return null;
				}
				return m_renderingContext.RenderingInfoManager.RenderingInfo[m_reportInstance.UniqueName];
			}
			set
			{
				if (m_reportInstance == null)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				m_renderingContext.RenderingInfoManager.RenderingInfo[m_reportInstance.UniqueName] = value;
			}
		}

		public string Custom
		{
			get
			{
				string text = m_reportDef.Custom;
				if (text == null && CustomProperties != null)
				{
					CustomProperty customProperty = CustomProperties["Custom"];
					if (customProperty != null && customProperty.Value != null)
					{
						text = DataTypeUtility.ConvertToInvariantString(customProperty.Value);
					}
				}
				return text;
			}
		}

		public CustomPropertyCollection CustomProperties
		{
			get
			{
				CustomPropertyCollection customPropertyCollection = m_customProperties;
				if (m_customProperties == null && m_reportDef.CustomProperties != null)
				{
					customPropertyCollection = ((m_reportInstance == null) ? new CustomPropertyCollection(m_reportDef.CustomProperties, null) : new CustomPropertyCollection(m_reportDef.CustomProperties, InstanceInfo.CustomPropertyInstances));
					if (m_renderingContext.CacheState)
					{
						m_customProperties = customPropertyCollection;
					}
				}
				return customPropertyCollection;
			}
		}

		public string DataTransform => m_reportDef.DataTransform;

		public string DataSchema => m_reportDef.DataSchema;

		public string DataElementName => m_reportDef.DataElementName;

		public DataElementStyles DataElementStyle
		{
			get
			{
				if (!m_reportDef.DataElementStyleAttribute)
				{
					return DataElementStyles.ElementNormal;
				}
				return DataElementStyles.AttributeNormal;
			}
		}

		public bool ShowHideStateChanged => m_renderingContext.ShowHideStateChanged;

		internal Microsoft.ReportingServices.ReportProcessing.Report ReportDef => m_reportDef;

		internal ReportInstance ReportInstance => m_reportInstance;

		internal ReportInstanceInfo InstanceInfo
		{
			get
			{
				if (m_reportInstance == null)
				{
					return null;
				}
				if (m_reportInstanceInfo == null)
				{
					m_reportInstanceInfo = m_reportInstance.GetCachedReportInstanceInfo(m_renderingContext.ChunkManager);
				}
				return m_reportInstanceInfo;
			}
		}

		internal RenderingContext RenderingContext => m_renderingContext;

		public int NumberOfPages
		{
			get
			{
				if (m_reportInstance == null)
				{
					return 0;
				}
				return m_reportInstance.NumberOfPages;
			}
		}

		internal bool BodyHasBorderStyles
		{
			get
			{
				if (!m_bodyStyleConstainsBorder.HasValue)
				{
					m_bodyStyleConstainsBorder = false;
					Microsoft.ReportingServices.ReportProcessing.Style styleClass = m_reportDef.StyleClass;
					if (styleClass != null && styleClass.StyleAttributes != null && styleClass.StyleAttributes.Count > 0)
					{
						StyleAttributeHashtable styleAttributes = styleClass.StyleAttributes;
						if (styleAttributes.ContainsKey("BorderStyle"))
						{
							AttributeInfo attributeInfo = styleAttributes["BorderStyle"];
							if (attributeInfo.IsExpression || !Validator.CompareWithInvariantCulture(attributeInfo.Value, "None"))
							{
								m_bodyStyleConstainsBorder = true;
								return true;
							}
						}
						if (styleAttributes.ContainsKey("BorderStyleLeft"))
						{
							AttributeInfo attributeInfo2 = styleAttributes["BorderStyleLeft"];
							if (attributeInfo2.IsExpression || !Validator.CompareWithInvariantCulture(attributeInfo2.Value, "None"))
							{
								m_bodyStyleConstainsBorder = true;
								return true;
							}
						}
						if (styleAttributes.ContainsKey("BorderStyleRight"))
						{
							AttributeInfo attributeInfo3 = styleAttributes["BorderStyleRight"];
							if (attributeInfo3.IsExpression || !Validator.CompareWithInvariantCulture(attributeInfo3.Value, "None"))
							{
								m_bodyStyleConstainsBorder = true;
								return true;
							}
						}
						if (styleAttributes.ContainsKey("BorderStyleTop"))
						{
							AttributeInfo attributeInfo4 = styleAttributes["BorderStyleTop"];
							if (attributeInfo4.IsExpression || !Validator.CompareWithInvariantCulture(attributeInfo4.Value, "None"))
							{
								m_bodyStyleConstainsBorder = true;
								return true;
							}
						}
						if (styleAttributes.ContainsKey("BorderStyleBottom"))
						{
							AttributeInfo attributeInfo5 = styleAttributes["BorderStyleBottom"];
							if (attributeInfo5.IsExpression || !Validator.CompareWithInvariantCulture(attributeInfo5.Value, "None"))
							{
								m_bodyStyleConstainsBorder = true;
								return true;
							}
						}
					}
				}
				return m_bodyStyleConstainsBorder.Value;
			}
		}

		internal Report(Microsoft.ReportingServices.ReportProcessing.Report reportDef, ReportInstance reportInstance, RenderingContext renderingContext, string reportName, string description, CultureInfo defaultLanguage)
		{
			m_reportDef = reportDef;
			m_reportInstance = reportInstance;
			m_renderingContext = renderingContext;
			m_reportBody = null;
			m_pageHeader = null;
			m_pageFooter = null;
			m_reportPagination = null;
			m_name = reportName;
			m_description = description;
			m_reportUrl = null;
			m_documentMapRoot = null;
			m_reportParameters = null;
			if (reportDef.Language != null)
			{
				if (reportDef.Language.Type == ExpressionInfo.Types.Constant)
				{
					m_reportLanguage = reportDef.Language.Value;
				}
				else if (reportInstance != null)
				{
					m_reportLanguage = reportInstance.Language;
				}
			}
			if (m_reportLanguage == null && defaultLanguage != null)
			{
				m_reportLanguage = defaultLanguage.Name;
			}
			AdjustBodyWhitespace();
		}

		private void AdjustBodyWhitespace()
		{
			if (m_reportDef.ReportItems != null && m_reportDef.ReportItems.Count != 0)
			{
				double num = 0.0;
				double num2 = 0.0;
				int count = m_reportDef.ReportItems.Count;
				for (int i = 0; i < count; i++)
				{
					Microsoft.ReportingServices.ReportProcessing.ReportItem reportItem = m_reportDef.ReportItems[i];
					num = Math.Max(num, reportItem.LeftValue + reportItem.WidthValue);
					num2 = Math.Max(num2, reportItem.TopValue + reportItem.HeightValue);
				}
				m_reportDef.HeightValue = Math.Min(m_reportDef.HeightValue, num2);
				string format = "{0:0.#####}mm";
				m_reportDef.Height = string.Format(CultureInfo.InvariantCulture, format, m_reportDef.HeightValue);
				double num3 = Math.Max(1.0, m_reportDef.PageWidthValue - m_reportDef.LeftMarginValue - m_reportDef.RightMarginValue);
				if (m_reportDef.Columns > 1)
				{
					num3 -= (double)(m_reportDef.Columns - 1) * m_reportDef.ColumnSpacingValue;
					num3 = Math.Max(1.0, num3 / (double)m_reportDef.Columns);
				}
				num = Math.Round(num, 1);
				num3 = Math.Round(num3, 1);
				m_reportDef.WidthValue = Math.Min(m_reportDef.WidthValue, num3 * Math.Ceiling(num / num3));
				m_reportDef.Width = string.Format(CultureInfo.InvariantCulture, format, m_reportDef.WidthValue);
			}
		}

		public string StreamURL(bool useSessionId, string streamName)
		{
			return m_renderingContext.TopLevelReportContext.RSRequestParameters.GetImageUrl(useSessionId, streamName, m_renderingContext.TopLevelReportContext);
		}

		public ReportUrlBuilder GetReportUrlBuilder(string initialUrl, bool useReplacementRoot, bool addReportParameters)
		{
			return new ReportUrlBuilder(m_renderingContext, initialUrl, useReplacementRoot, addReportParameters);
		}

		public bool GetResource(string resourcePath, out byte[] resource, out string mimeType)
		{
			if (m_renderingContext.GetResourceCallback != null)
			{
				m_renderingContext.GetResourceCallback.GetResource(m_renderingContext.CurrentReportContext, resourcePath, out resource, out mimeType, out bool _, out bool _);
				return true;
			}
			resource = null;
			mimeType = null;
			return false;
		}

		public ReportItem Find(string uniqueName)
		{
			if (uniqueName == null || uniqueName.Length <= 0)
			{
				return null;
			}
			int num = ReportItem.StringToInt(uniqueName);
			if (num < 0)
			{
				return null;
			}
			return m_renderingContext.FindReportItemInBody(num);
		}

		public void EnableNativeCustomReportItem()
		{
			Global.Tracer.Assert(m_renderingContext != null);
			m_renderingContext.NativeCRITypes = null;
			m_renderingContext.NativeAllCRITypes = true;
		}

		public void EnableNativeCustomReportItem(string type)
		{
			Global.Tracer.Assert(m_renderingContext != null);
			if (type == null)
			{
				m_renderingContext.NativeCRITypes = null;
				m_renderingContext.NativeAllCRITypes = true;
			}
			if (m_renderingContext.NativeCRITypes == null)
			{
				m_renderingContext.NativeCRITypes = new Hashtable();
			}
			if (!m_renderingContext.NativeCRITypes.ContainsKey(type))
			{
				m_renderingContext.NativeCRITypes.Add(type, null);
			}
		}

		internal bool Search(int searchPage, string findValue)
		{
			SearchContext searchContext = new SearchContext(searchPage, findValue, 0, NumberOfPages - 1);
			PageSection pageSection = PageHeader;
			PageSection pageSection2 = PageFooter;
			bool flag = false;
			bool flag2 = false;
			if (pageSection != null && ((searchPage > 0 && searchPage < NumberOfPages - 1) || (searchPage == 0 && pageSection.PrintOnFirstPage) || (searchPage != 0 && searchPage == NumberOfPages - 1 && pageSection.PrintOnLastPage)))
			{
				flag = true;
			}
			if (pageSection2 != null && ((searchPage > 0 && searchPage < NumberOfPages - 1) || (searchPage != NumberOfPages - 1 && searchPage == 0 && pageSection2.PrintOnFirstPage) || (searchPage == NumberOfPages - 1 && pageSection2.PrintOnLastPage)))
			{
				flag2 = true;
			}
			if ((flag || flag2) && NeedsHeaderFooterEvaluation)
			{
				PageSection pageHeader = null;
				PageSection pageFooter = null;
				Microsoft.ReportingServices.ReportProcessing.ReportProcessing.EvaluateHeaderFooterExpressions(searchPage + 1, NumberOfPages, this, null, out pageHeader, out pageFooter);
				if (m_reportDef.PageHeaderEvaluation)
				{
					pageSection = pageHeader;
				}
				if (m_reportDef.PageFooterEvaluation)
				{
					pageSection2 = pageFooter;
				}
			}
			bool flag3 = false;
			if (flag)
			{
				flag3 = pageSection.Search(searchContext);
			}
			if (!flag3)
			{
				flag3 = Body.Search(searchContext);
				if (!flag3 && flag2)
				{
					flag3 = pageSection2.Search(searchContext);
				}
			}
			return flag3;
		}
	}
}
