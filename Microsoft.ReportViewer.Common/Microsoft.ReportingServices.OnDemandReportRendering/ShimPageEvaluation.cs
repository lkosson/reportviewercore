using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using Microsoft.ReportingServices.ReportRendering;
using System;
using System.Collections;
using System.Globalization;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimPageEvaluation : PageEvaluation
	{
		private Microsoft.ReportingServices.ReportProcessing.Report m_report;

		private CultureInfo m_reportCulture;

		private Hashtable m_aggregatesOverReportItems;

		private AggregatesImpl m_aggregates;

		private Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ProcessingContext m_processingContext;

		internal ShimPageEvaluation(Report report)
			: base(report)
		{
			InitializeEnvironment();
			PageInit();
		}

		internal override void Reset(ReportSection section, int newPageNumber, int newTotalPages, int newOverallPageNumber, int newOverallTotalPages)
		{
			base.Reset(section, newPageNumber, newTotalPages, newOverallPageNumber, newOverallTotalPages);
			PageInit();
		}

		internal override void Add(string textboxName, object textboxValue)
		{
			if (textboxName == null)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
			}
			foreach (DataAggregateObj @object in m_aggregates.Objects)
			{
				m_processingContext.ReportObjectModel.AggregatesImpl.Add(@object);
			}
			if (!m_processingContext.ReportItemsReferenced)
			{
				return;
			}
			((TextBoxImpl)m_processingContext.ReportObjectModel.ReportItemsImpl[textboxName])?.SetResult(new VariantResult(errorOccurred: false, textboxValue));
			AggregatesImpl aggregatesImpl = (AggregatesImpl)m_aggregatesOverReportItems[textboxName];
			if (aggregatesImpl == null)
			{
				return;
			}
			foreach (DataAggregateObj object2 in aggregatesImpl.Objects)
			{
				object2.Update();
			}
		}

		internal override void UpdatePageSections(ReportSection section)
		{
			Microsoft.ReportingServices.ReportRendering.PageSection header = null;
			Microsoft.ReportingServices.ReportRendering.PageSection footer = null;
			foreach (AggregatesImpl value in m_aggregatesOverReportItems.Values)
			{
				foreach (DataAggregateObj @object in value.Objects)
				{
					m_processingContext.ReportObjectModel.AggregatesImpl.Add(@object);
				}
			}
			if (m_report.PageHeaderEvaluation)
			{
				header = GenerateRenderPageSection(m_report.PageHeader, "ph");
			}
			if (m_report.PageFooterEvaluation)
			{
				footer = GenerateRenderPageSection(m_report.PageFooter, "pf");
			}
			m_aggregates = null;
			m_aggregatesOverReportItems = null;
			section.Page.UpdateWithCurrentPageSections(header, footer);
		}

		private Microsoft.ReportingServices.ReportRendering.PageSection GenerateRenderPageSection(Microsoft.ReportingServices.ReportProcessing.PageSection pageSection, string uniqueNamePrefix)
		{
			Microsoft.ReportingServices.ReportProcessing.PageSectionInstance pageSectionInstance = new Microsoft.ReportingServices.ReportProcessing.PageSectionInstance(m_processingContext, m_currentPageNumber, pageSection);
			Microsoft.ReportingServices.ReportProcessing.ReportProcessing.PageMerge.CreateInstances(m_processingContext, pageSectionInstance.ReportItemColInstance, pageSection.ReportItems);
			string text = m_currentPageNumber.ToString(CultureInfo.InvariantCulture) + uniqueNamePrefix;
			Microsoft.ReportingServices.ReportRendering.RenderingContext renderingContext = new Microsoft.ReportingServices.ReportRendering.RenderingContext(m_romReport.RenderReport.RenderingContext, text);
			return new Microsoft.ReportingServices.ReportRendering.PageSection(text, pageSection, pageSectionInstance, m_romReport.RenderReport, renderingContext, pageDef: false);
		}

		private void InitializeEnvironment()
		{
			m_report = m_romReport.RenderReport.ReportDef;
			Microsoft.ReportingServices.ReportProcessing.ReportInstance reportInstance = m_romReport.RenderReport.ReportInstance;
			Microsoft.ReportingServices.ReportRendering.RenderingContext renderingContext = m_romReport.RenderReport.RenderingContext;
			ReportSnapshot reportSnapshot = renderingContext.ReportSnapshot;
			ReportInstanceInfo reportInstanceInfo = (ReportInstanceInfo)reportInstance.GetInstanceInfo(renderingContext.ChunkManager);
			m_processingContext = new Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ProcessingContext(renderingContext.TopLevelReportContext, m_report.ShowHideType, renderingContext.GetResourceCallback, m_report.EmbeddedImages, m_report.ImageStreamNames, new ProcessingErrorContext(), !m_report.PageMergeOnePass, renderingContext.AllowUserProfileState, renderingContext.ReportRuntimeSetup, renderingContext.DataProtection);
			m_reportCulture = Localization.DefaultReportServerSpecificCulture;
			if (m_report.Language != null)
			{
				string text = null;
				text = ((m_report.Language.Type != ExpressionInfo.Types.Constant) ? reportInstance.Language : m_report.Language.Value);
				if (text != null)
				{
					try
					{
						m_reportCulture = new CultureInfo(text, useUserOverride: false);
						if (m_reportCulture.IsNeutralCulture)
						{
							m_reportCulture = CultureInfo.CreateSpecificCulture(text);
							m_reportCulture = new CultureInfo(m_reportCulture.Name, useUserOverride: false);
						}
					}
					catch (Exception e)
					{
						if (AsynchronousExceptionDetection.IsStoppingException(e))
						{
							throw;
						}
					}
				}
			}
			m_processingContext.ReportObjectModel = new ObjectModelImpl(m_processingContext);
			Global.Tracer.Assert(m_processingContext.ReportRuntime == null, "(m_processingContext.ReportRuntime == null)");
			m_processingContext.ReportRuntime = new ReportRuntime(m_processingContext.ReportObjectModel, m_processingContext.ErrorContext);
			m_processingContext.ReportObjectModel.FieldsImpl = new FieldsImpl();
			m_processingContext.ReportObjectModel.ParametersImpl = new ParametersImpl(reportInstanceInfo.Parameters.Count);
			m_processingContext.ReportObjectModel.GlobalsImpl = new GlobalsImpl(reportInstanceInfo.ReportName, m_currentPageNumber, m_totalPages, reportSnapshot.ExecutionTime, reportSnapshot.ReportServerUrl, reportSnapshot.ReportFolder);
			m_processingContext.ReportObjectModel.UserImpl = new UserImpl(reportSnapshot.RequestUserName, reportSnapshot.Language, m_processingContext.AllowUserProfileState);
			m_processingContext.ReportObjectModel.DataSetsImpl = new DataSetsImpl();
			m_processingContext.ReportObjectModel.DataSourcesImpl = new DataSourcesImpl(m_report.DataSourceCount);
			for (int i = 0; i < reportInstanceInfo.Parameters.Count; i++)
			{
				m_processingContext.ReportObjectModel.ParametersImpl.Add(reportInstanceInfo.Parameters[i].Name, new ParameterImpl(reportInstanceInfo.Parameters[i].Values, reportInstanceInfo.Parameters[i].Labels, reportInstanceInfo.Parameters[i].MultiValue));
			}
			m_processingContext.ReportRuntime.LoadCompiledCode(m_report, parametersOnly: false, m_processingContext.ReportObjectModel, m_processingContext.ReportRuntimeSetup);
		}

		private void PageInit()
		{
			m_processingContext.ReportObjectModel.GlobalsImpl.SetPageNumbers(m_currentPageNumber, m_totalPages);
			m_processingContext.ReportObjectModel.ReportItemsImpl = new ReportItemsImpl();
			m_processingContext.ReportObjectModel.AggregatesImpl = new AggregatesImpl(m_processingContext.ReportRuntime);
			if (m_processingContext.ReportRuntime.ReportExprHost != null)
			{
				m_processingContext.RuntimeInitializeReportItemObjs(m_report.ReportItems, traverseDataRegions: true, setValue: true);
				if (m_report.PageHeader != null)
				{
					if (m_processingContext.ReportRuntime.ReportExprHost != null)
					{
						m_report.PageHeader.SetExprHost(m_processingContext.ReportRuntime.ReportExprHost, m_processingContext.ReportObjectModel);
					}
					m_processingContext.RuntimeInitializeReportItemObjs(m_report.PageHeader.ReportItems, traverseDataRegions: false, setValue: false);
				}
				if (m_report.PageFooter != null)
				{
					if (m_processingContext.ReportRuntime.ReportExprHost != null)
					{
						m_report.PageFooter.SetExprHost(m_processingContext.ReportRuntime.ReportExprHost, m_processingContext.ReportObjectModel);
					}
					m_processingContext.RuntimeInitializeReportItemObjs(m_report.PageFooter.ReportItems, traverseDataRegions: false, setValue: false);
				}
			}
			m_aggregates = new AggregatesImpl(m_processingContext.ReportRuntime);
			m_aggregatesOverReportItems = new Hashtable();
			m_processingContext.ReportObjectModel.ReportItemsImpl.SpecialMode = true;
			if (m_report.PageAggregates != null)
			{
				for (int i = 0; i < m_report.PageAggregates.Count; i++)
				{
					DataAggregateInfo dataAggregateInfo = m_report.PageAggregates[i];
					dataAggregateInfo.ExprHostInitialized = false;
					DataAggregateObj dataAggregateObj = new DataAggregateObj(dataAggregateInfo, m_processingContext);
					dataAggregateObj.EvaluateParameters(out object[] _, out DataFieldStatus _);
					string specialModeIndex = m_processingContext.ReportObjectModel.ReportItemsImpl.GetSpecialModeIndex();
					if (specialModeIndex == null)
					{
						m_aggregates.Add(dataAggregateObj);
					}
					else
					{
						AggregatesImpl aggregatesImpl = (AggregatesImpl)m_aggregatesOverReportItems[specialModeIndex];
						if (aggregatesImpl == null)
						{
							aggregatesImpl = new AggregatesImpl(m_processingContext.ReportRuntime);
							m_aggregatesOverReportItems.Add(specialModeIndex, aggregatesImpl);
						}
						aggregatesImpl.Add(dataAggregateObj);
					}
					dataAggregateObj.Init();
				}
			}
			m_processingContext.ReportObjectModel.ReportItemsImpl.SpecialMode = false;
		}
	}
}
