using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.RdlExpressions;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class OnDemandPageEvaluation : PageEvaluation
	{
		private OnDemandProcessingContext m_processingContext;

		private Dictionary<string, ReportSection> m_reportItemToReportSection = new Dictionary<string, ReportSection>();

		internal OnDemandPageEvaluation(Report report)
			: base(report)
		{
			InitializeEnvironment();
		}

		internal override void Add(string textboxName, object textboxValue)
		{
			if (textboxName == null)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
			}
			if (!m_processingContext.ReportItemsReferenced)
			{
				return;
			}
			((TextBoxImpl)m_processingContext.ReportObjectModel.ReportItemsImpl[textboxName])?.SetResult(new Microsoft.ReportingServices.RdlExpressions.VariantResult(errorOccurred: false, textboxValue));
			if (!m_reportItemToReportSection.TryGetValue(textboxName, out ReportSection value) || !value.PageAggregatesOverReportItems.TryGetValue(textboxName, out AggregatesImpl value2))
			{
				return;
			}
			foreach (Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj @object in value2.Objects)
			{
				@object.Update();
			}
		}

		internal override void UpdatePageSections(ReportSection section)
		{
			if (section.Page.PageHeader == null && section.Page.PageFooter == null)
			{
				return;
			}
			ObjectModelImpl reportObjectModel = m_processingContext.ReportObjectModel;
			reportObjectModel.GlobalsImpl.SetPageName(m_pageName);
			if (section.PageAggregatesOverReportItems == null)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidPageSectionState, section.SectionIndex);
			}
			foreach (AggregatesImpl value in section.PageAggregatesOverReportItems.Values)
			{
				foreach (Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj @object in value.Objects)
				{
					reportObjectModel.AggregatesImpl.Add(@object);
				}
			}
			section.PageAggregatesOverReportItems = null;
		}

		internal override void Reset(ReportSection section, int newPageNumber, int newTotalPages, int newOverallPageNumber, int newOverallTotalPages)
		{
			base.Reset(section, newPageNumber, newTotalPages, newOverallPageNumber, newOverallTotalPages);
			if (section.Page.PageHeader != null || section.Page.PageFooter != null)
			{
				PageInit(section);
			}
		}

		private void InitializeEnvironment()
		{
			m_processingContext = m_romReport.HeaderFooterRenderingContext.OdpContext;
			Microsoft.ReportingServices.ReportIntermediateFormat.Report reportDef = m_romReport.ReportDef;
			ObjectModelImpl reportObjectModel = m_processingContext.ReportObjectModel;
			if (reportDef.DataSetsNotOnlyUsedInParameters == 1)
			{
				m_processingContext.SetupFieldsForNewDataSetPageSection(reportDef.FirstDataSet);
			}
			else
			{
				m_processingContext.SetupEmptyTopLevelFields();
			}
			reportObjectModel.VariablesImpl = new VariablesImpl(lockAdd: false);
			if (reportDef.HasVariables)
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.ReportInstance currentReportInstance = m_romReport.RenderingContext.OdpContext.CurrentReportInstance;
				m_processingContext.RuntimeInitializePageSectionVariables(reportDef, currentReportInstance?.VariableValues);
			}
			reportObjectModel.LookupsImpl = new LookupsImpl();
			if (reportDef.HasLookups)
			{
				m_processingContext.RuntimeInitializeLookups(reportDef);
			}
			ReportItemsImpl reportItemsImpl = new ReportItemsImpl(lockAdd: false);
			foreach (ReportSection reportSection in m_romReport.ReportSections)
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.ReportSection sectionDef = reportSection.SectionDef;
				reportSection.BodyItemsForHeadFoot = new ReportItemsImpl(lockAdd: false);
				reportSection.PageSectionItemsForHeadFoot = new ReportItemsImpl(lockAdd: false);
				reportObjectModel.ReportItemsImpl = reportSection.BodyItemsForHeadFoot;
				m_processingContext.RuntimeInitializeTextboxObjs(sectionDef.ReportItems, setExprHost: false);
				reportObjectModel.ReportItemsImpl = reportSection.PageSectionItemsForHeadFoot;
				Microsoft.ReportingServices.ReportIntermediateFormat.Page page = sectionDef.Page;
				if (page.PageHeader != null)
				{
					if (m_processingContext.ReportRuntime.ReportExprHost != null)
					{
						page.PageHeader.SetExprHost(m_processingContext.ReportRuntime.ReportExprHost, reportObjectModel);
					}
					m_processingContext.RuntimeInitializeReportItemObjs(page.PageHeader.ReportItems, traverseDataRegions: false);
					m_processingContext.RuntimeInitializeTextboxObjs(page.PageHeader.ReportItems, setExprHost: true);
				}
				if (page.PageFooter != null)
				{
					if (m_processingContext.ReportRuntime.ReportExprHost != null)
					{
						page.PageFooter.SetExprHost(m_processingContext.ReportRuntime.ReportExprHost, reportObjectModel);
					}
					m_processingContext.RuntimeInitializeReportItemObjs(page.PageFooter.ReportItems, traverseDataRegions: false);
					m_processingContext.RuntimeInitializeTextboxObjs(page.PageFooter.ReportItems, setExprHost: true);
				}
				reportItemsImpl.AddAll(reportSection.BodyItemsForHeadFoot);
				reportItemsImpl.AddAll(reportSection.PageSectionItemsForHeadFoot);
			}
			reportObjectModel.ReportItemsImpl = reportItemsImpl;
			reportObjectModel.AggregatesImpl = new AggregatesImpl(m_processingContext);
		}

		private void PageInit(ReportSection section)
		{
			ObjectModelImpl reportObjectModel = m_processingContext.ReportObjectModel;
			AggregatesImpl aggregatesImpl = reportObjectModel.AggregatesImpl;
			Global.Tracer.Assert(section.BodyItemsForHeadFoot != null, "Missing cached BodyItemsForHeadFoot collection");
			Global.Tracer.Assert(section.PageSectionItemsForHeadFoot != null, "Missing cached PageSectionItemsForHeadFoot collection");
			section.BodyItemsForHeadFoot.ResetAll(default(Microsoft.ReportingServices.RdlExpressions.VariantResult));
			section.PageSectionItemsForHeadFoot.ResetAll();
			reportObjectModel.GlobalsImpl.SetPageNumbers(m_currentPageNumber, m_totalPages, m_currentOverallPageNumber, m_overallTotalPages);
			reportObjectModel.GlobalsImpl.SetPageName(m_pageName);
			_ = m_romReport.ReportDef;
			Microsoft.ReportingServices.ReportIntermediateFormat.ReportSection sectionDef = section.SectionDef;
			Microsoft.ReportingServices.ReportIntermediateFormat.Page page = sectionDef.Page;
			section.PageAggregatesOverReportItems = new Dictionary<string, AggregatesImpl>();
			m_processingContext.ReportObjectModel.ReportItemsImpl.SpecialMode = true;
			if (page.PageAggregates != null)
			{
				for (int i = 0; i < page.PageAggregates.Count; i++)
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo dataAggregateInfo = page.PageAggregates[i];
					aggregatesImpl.Remove(dataAggregateInfo);
					dataAggregateInfo.ExprHostInitialized = false;
					Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj dataAggregateObj = new Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObj(dataAggregateInfo, m_processingContext);
					dataAggregateObj.EvaluateParameters(out object[] _, out DataFieldStatus _);
					string specialModeIndex = reportObjectModel.ReportItemsImpl.GetSpecialModeIndex();
					if (specialModeIndex == null)
					{
						aggregatesImpl.Add(dataAggregateObj);
					}
					else
					{
						if (!section.PageAggregatesOverReportItems.TryGetValue(specialModeIndex, out AggregatesImpl value))
						{
							value = new AggregatesImpl(m_processingContext);
							section.PageAggregatesOverReportItems.Add(specialModeIndex, value);
						}
						value.Add(dataAggregateObj);
						m_reportItemToReportSection[specialModeIndex] = section;
					}
					dataAggregateObj.Init();
				}
			}
			reportObjectModel.ReportItemsImpl.SpecialMode = false;
			Microsoft.ReportingServices.ReportIntermediateFormat.PageSection rifObject = null;
			IReportScopeInstance romInstance = null;
			if (sectionDef.Page.PageHeader != null)
			{
				rifObject = sectionDef.Page.PageHeader;
				romInstance = section.Page.PageHeader.Instance.ReportScopeInstance;
				section.Page.PageHeader.SetNewContext();
			}
			if (sectionDef.Page.PageFooter != null)
			{
				rifObject = sectionDef.Page.PageFooter;
				romInstance = section.Page.PageFooter.Instance.ReportScopeInstance;
				section.Page.PageFooter.SetNewContext();
			}
			if (sectionDef != null)
			{
				m_processingContext.SetupContext(rifObject, romInstance);
			}
		}
	}
}
