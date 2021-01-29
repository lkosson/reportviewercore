using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer;
using Microsoft.ReportingServices.Rendering.ExcelRenderer;
using Microsoft.ReportingServices.Rendering.ImageRenderer;
using Microsoft.ReportingServices.Rendering.RPLRendering;
using Microsoft.ReportingServices.Rendering.WordRenderer;
using Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer;
using System;
using System.Collections.Generic;
using Microsoft.ReportingServices;

namespace Microsoft.Reporting
{
	[Serializable]
	internal sealed class ControlService : LocalService
	{
		[NonSerialized]
		private List<LocalRenderingExtensionInfo> m_renderingExtensions;

		private LocalProcessingHostMapTileServerConfiguration m_mapTileServerConfiguration;

		public override LocalProcessingHostMapTileServerConfiguration MapTileServerConfiguration => m_mapTileServerConfiguration;

		private static Evidence InternetZoneEvidence
		{
			get
			{
				Evidence evidence = new Evidence();
				return evidence;
			}
		}

		public ControlService(ILocalCatalog catalog)
			: base(catalog, InternetZoneEvidence, new ControlPolicyManager())
		{
			m_mapTileServerConfiguration = new LocalProcessingHostMapTileServerConfiguration();
		}

		public override IEnumerable<LocalRenderingExtensionInfo> ListRenderingExtensions()
		{
			if (m_renderingExtensions == null)
			{
				List<LocalRenderingExtensionInfo> list = new List<LocalRenderingExtensionInfo>();
				RPLRenderer rPLRenderer = new RPLRenderer();
				list.Add(new LocalRenderingExtensionInfo("RPL", rPLRenderer.LocalizedName, isVisible: false, typeof(RPLRenderer), isExposedExternally: false));
				ExcelRenderer excelRenderer = new ExcelRenderer();
				list.Add(new LocalRenderingExtensionInfo("Excel", excelRenderer.LocalizedName, isVisible: false, typeof(ExcelRenderer), isExposedExternally: true));
				ExcelOpenXmlRenderer excelOpenXmlRenderer = new ExcelOpenXmlRenderer();
				list.Add(new LocalRenderingExtensionInfo("EXCELOPENXML", excelOpenXmlRenderer.LocalizedName, isVisible: true, typeof(ExcelOpenXmlRenderer), isExposedExternally: true));
				ImageRenderer imageRenderer = new ImageRenderer();
				list.Add(new LocalRenderingExtensionInfo("IMAGE", imageRenderer.LocalizedName, isVisible: false, typeof(ImageRenderer), isExposedExternally: true));
				PDFRenderer pDFRenderer = new PDFRenderer();
				list.Add(new LocalRenderingExtensionInfo("PDF", pDFRenderer.LocalizedName, isVisible: true, typeof(PDFRenderer), isExposedExternally: true));
				WordDocumentRenderer wordDocumentRenderer = new WordDocumentRenderer();
				list.Add(new LocalRenderingExtensionInfo("WORD", wordDocumentRenderer.LocalizedName, isVisible: false, typeof(WordDocumentRenderer), isExposedExternally: true));
				WordOpenXmlDocumentRenderer wordOpenXmlDocumentRenderer = new WordOpenXmlDocumentRenderer();
				list.Add(new LocalRenderingExtensionInfo("WORDOPENXML", wordOpenXmlDocumentRenderer.LocalizedName, isVisible: true, typeof(WordOpenXmlDocumentRenderer), isExposedExternally: true));

				list.Add(new LocalRenderingExtensionInfo("MHTML", new ReportingServices.Rendering.HtmlRenderer.MHtmlRenderingExtension().LocalizedName, isVisible: true, typeof(ReportingServices.Rendering.HtmlRenderer.MHtmlRenderingExtension), isExposedExternally: true));
				list.Add(new LocalRenderingExtensionInfo("HTML4.0", new ReportingServices.Rendering.HtmlRenderer.Html40RenderingExtension().LocalizedName, isVisible: true, typeof(ReportingServices.Rendering.HtmlRenderer.Html40RenderingExtension), isExposedExternally: true));
				list.Add(new LocalRenderingExtensionInfo("HTML5", new ReportingServices.Rendering.HtmlRenderer.Html5RenderingExtension().LocalizedName, isVisible: true, typeof(ReportingServices.Rendering.HtmlRenderer.Html5RenderingExtension), isExposedExternally: true));

				m_renderingExtensions = list;
			}
			return m_renderingExtensions;
		}

		protected override IConfiguration CreateProcessingConfiguration()
		{
			ControlProcessingConfiguration controlProcessingConfiguration = new ControlProcessingConfiguration();
			controlProcessingConfiguration.ShowSubreportErrorDetails = base.ShowDetailedSubreportMessages;
			controlProcessingConfiguration.SetMapTileServerConfiguration(MapTileServerConfiguration);
			return controlProcessingConfiguration;
		}

		protected override void SetProcessingCulture()
		{
		}

		protected override SubreportCallbackHandler CreateSubreportCallbackHandler()
		{
			return new SubreportCallbackHandler(this);
		}

		protected override IRenderingExtension CreateRenderer(string format, bool allowInternal)
		{
			if (format == null)
			{
				return null;
			}
			foreach (LocalRenderingExtensionInfo item in ListRenderingExtensions())
			{
				if (string.Compare(item.Name, format, StringComparison.OrdinalIgnoreCase) == 0)
				{
					if (!allowInternal && !item.IsExposedExternally)
					{
						break;
					}
					return item.Instantiate();
				}
			}
			throw new ArgumentOutOfRangeException("format");
		}
	}
}
