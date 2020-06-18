using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.ReportPublishing
{
	internal class PublishingVersioning
	{
		private readonly IConfiguration m_configuration;

		private readonly PublishingContextBase m_publishingContext;

		private readonly int m_configVersion;

		private static readonly RdlVersionedFeatures m_rdlFeatureVersioningStructure = CreateRdlFeatureVersioningStructure();

		internal RenderMode RenderMode
		{
			get
			{
				if (m_publishingContext.IsRdlx)
				{
					return RenderMode.RenderEdit;
				}
				return RenderMode.FullOdp;
			}
		}

		internal PublishingVersioning(IConfiguration configuration, PublishingContextBase publishingContext)
		{
			m_configuration = configuration;
			m_publishingContext = publishingContext;
			m_configVersion = ReportProcessingCompatibilityVersion.GetCompatibilityVersion(m_configuration);
		}

		private static RdlVersionedFeatures CreateRdlFeatureVersioningStructure()
		{
			RdlVersionedFeatures rdlVersionedFeatures = new RdlVersionedFeatures();
			rdlVersionedFeatures.Add(RdlFeatures.SharedDataSetReferences, 0, RenderMode.FullOdp);
			rdlVersionedFeatures.Add(RdlFeatures.Image_Embedded, 200, RenderMode.Both);
			rdlVersionedFeatures.Add(RdlFeatures.Sort_Group_Applied, 0, RenderMode.FullOdp);
			rdlVersionedFeatures.Add(RdlFeatures.DeferredSort, 100, RenderMode.RenderEdit);
			rdlVersionedFeatures.Add(RdlFeatures.Sort_DataRegion, 0, RenderMode.FullOdp);
			rdlVersionedFeatures.Add(RdlFeatures.Filters, 0, RenderMode.FullOdp);
			rdlVersionedFeatures.Add(RdlFeatures.Lookup, 0, RenderMode.FullOdp);
			rdlVersionedFeatures.Add(RdlFeatures.RunningValue, 0, RenderMode.FullOdp);
			rdlVersionedFeatures.Add(RdlFeatures.Previous, 0, RenderMode.FullOdp);
			rdlVersionedFeatures.Add(RdlFeatures.RowNumber, 0, RenderMode.FullOdp);
			rdlVersionedFeatures.Add(RdlFeatures.GroupParent, 0, RenderMode.FullOdp);
			rdlVersionedFeatures.Add(RdlFeatures.Variables, 0, RenderMode.FullOdp);
			rdlVersionedFeatures.Add(RdlFeatures.SubReports, 0, RenderMode.FullOdp);
			rdlVersionedFeatures.Add(RdlFeatures.AutomaticSubtotals, 0, RenderMode.FullOdp);
			rdlVersionedFeatures.Add(RdlFeatures.DomainScope, 0, RenderMode.FullOdp);
			rdlVersionedFeatures.Add(RdlFeatures.InScope, 0, RenderMode.FullOdp);
			rdlVersionedFeatures.Add(RdlFeatures.Level, 0, RenderMode.FullOdp);
			rdlVersionedFeatures.Add(RdlFeatures.CreateDrillthroughContext, 0, RenderMode.FullOdp);
			rdlVersionedFeatures.Add(RdlFeatures.UserSort, 0, RenderMode.FullOdp);
			rdlVersionedFeatures.Add(RdlFeatures.AggregatesOfAggregates, 0, RenderMode.FullOdp);
			rdlVersionedFeatures.Add(RdlFeatures.PageHeaderFooter, 0, RenderMode.FullOdp);
			rdlVersionedFeatures.Add(RdlFeatures.SortGroupExpression_OnlySimpleField, 0, RenderMode.FullOdp);
			rdlVersionedFeatures.Add(RdlFeatures.PeerGroups, 0, RenderMode.FullOdp);
			rdlVersionedFeatures.Add(RdlFeatures.ImageTag, 100, RenderMode.RenderEdit);
			rdlVersionedFeatures.Add(RdlFeatures.ReportSectionName, 100, RenderMode.RenderEdit);
			rdlVersionedFeatures.Add(RdlFeatures.EmbeddingMode, 200, RenderMode.RenderEdit);
			rdlVersionedFeatures.Add(RdlFeatures.EmbeddingMode_Inline, 0, RenderMode.FullOdp);
			rdlVersionedFeatures.Add(RdlFeatures.ReportSection_LayoutDirection, 200, RenderMode.RenderEdit);
			rdlVersionedFeatures.Add(RdlFeatures.ThemeFonts, 200, RenderMode.RenderEdit);
			rdlVersionedFeatures.Add(RdlFeatures.TablixHierarchy_EnableDrilldown, 200, RenderMode.RenderEdit);
			rdlVersionedFeatures.Add(RdlFeatures.ChartHierarchy_EnableDrilldown, 200, RenderMode.RenderEdit);
			rdlVersionedFeatures.Add(RdlFeatures.ScopesCollection, 200, RenderMode.RenderEdit);
			rdlVersionedFeatures.Add(RdlFeatures.ThemeColors, 200, RenderMode.RenderEdit);
			rdlVersionedFeatures.Add(RdlFeatures.Report_Code, 0, RenderMode.FullOdp);
			rdlVersionedFeatures.Add(RdlFeatures.Report_Classes, 0, RenderMode.FullOdp);
			rdlVersionedFeatures.Add(RdlFeatures.Report_CodeModules, 0, RenderMode.FullOdp);
			rdlVersionedFeatures.Add(RdlFeatures.ComplexExpression, 0, RenderMode.FullOdp);
			rdlVersionedFeatures.Add(RdlFeatures.BackgroundImageFitting, 200, RenderMode.RenderEdit);
			rdlVersionedFeatures.Add(RdlFeatures.BackgroundImageTransparency, 200, RenderMode.RenderEdit);
			rdlVersionedFeatures.Add(RdlFeatures.LabelData_KeyFields, 200, RenderMode.Both);
			rdlVersionedFeatures.Add(RdlFeatures.ImageTagsCollection, 200, RenderMode.RenderEdit);
			rdlVersionedFeatures.Add(RdlFeatures.CellLevelFormatting, 200, RenderMode.RenderEdit);
			rdlVersionedFeatures.Add(RdlFeatures.ParametersLayout, 300, RenderMode.FullOdp);
			rdlVersionedFeatures.Add(RdlFeatures.DefaultFontFamily, 400, RenderMode.FullOdp);
			rdlVersionedFeatures.VerifyAllFeaturesAreAdded();
			return rdlVersionedFeatures;
		}

		internal bool IsRdlFeatureRestricted(RdlFeatures feature)
		{
			return !m_rdlFeatureVersioningStructure.IsRdlFeatureAllowed(feature, m_configVersion, m_publishingContext.PublishingVersioning.RenderMode);
		}
	}
}
