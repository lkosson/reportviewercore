using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportRendering;
using System;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class SubReport : ReportItem
	{
		private Report m_report;

		private ReportStringProperty m_noRowsMessage;

		private bool m_processedWithError;

		private SubReportErrorCodes m_errorCode;

		private string m_errorMessage;

		private bool m_noRows;

		private bool m_isNewContext = true;

		public string ReportName
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return ((Microsoft.ReportingServices.ReportProcessing.SubReport)m_renderReportItem.ReportItemDef).ReportPath;
				}
				return ((Microsoft.ReportingServices.ReportIntermediateFormat.SubReport)m_reportItemDef).ReportName;
			}
		}

		public Report Report
		{
			get
			{
				RetrieveSubreport();
				return m_report;
			}
		}

		public ReportStringProperty NoRowsMessage
		{
			get
			{
				if (m_noRowsMessage == null)
				{
					if (m_isOldSnapshot)
					{
						m_noRowsMessage = new ReportStringProperty(((Microsoft.ReportingServices.ReportProcessing.SubReport)m_renderReportItem.ReportItemDef).NoRows);
					}
					else
					{
						Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo noRowsMessage = ((Microsoft.ReportingServices.ReportIntermediateFormat.SubReport)m_reportItemDef).NoRowsMessage;
						if (noRowsMessage == null)
						{
							m_noRowsMessage = new ReportStringProperty(isExpression: false, null, null);
						}
						else
						{
							m_noRowsMessage = new ReportStringProperty(noRowsMessage.IsExpression, noRowsMessage.OriginalText, noRowsMessage.StringValue);
						}
					}
				}
				return m_noRowsMessage;
			}
		}

		public bool OmitBorderOnPageBreak
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return false;
				}
				return ((Microsoft.ReportingServices.ReportIntermediateFormat.SubReport)m_reportItemDef).OmitBorderOnPageBreak;
			}
		}

		public bool KeepTogether
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return true;
				}
				return ((Microsoft.ReportingServices.ReportIntermediateFormat.SubReport)m_reportItemDef).KeepTogether;
			}
		}

		internal bool ProcessedWithError
		{
			get
			{
				RetrieveSubreport();
				return m_processedWithError;
			}
		}

		internal SubReportErrorCodes ErrorCode
		{
			get
			{
				RetrieveSubreport();
				return m_errorCode;
			}
		}

		internal string ErrorMessage
		{
			get
			{
				RetrieveSubreport();
				return m_errorMessage;
			}
		}

		internal bool NoRows
		{
			get
			{
				RetrieveSubreport();
				return m_noRows;
			}
		}

		internal SubReport(IReportScope reportScope, IDefinitionPath parentDefinitionPath, int indexIntoParentCollectionDef, Microsoft.ReportingServices.ReportIntermediateFormat.SubReport reportItemDef, RenderingContext renderingContext)
			: base(reportScope, parentDefinitionPath, indexIntoParentCollectionDef, reportItemDef, renderingContext)
		{
		}

		internal SubReport(IDefinitionPath parentDefinitionPath, int indexIntoParentCollectionDef, bool inSubtotal, Microsoft.ReportingServices.ReportRendering.SubReport renderSubReport, RenderingContext renderingContext)
			: base(parentDefinitionPath, indexIntoParentCollectionDef, inSubtotal, renderSubReport, renderingContext)
		{
		}

		internal override ReportItemInstance GetOrCreateInstance()
		{
			if (m_instance == null)
			{
				m_instance = new SubReportInstance(this);
			}
			return m_instance;
		}

		internal void RetrieveSubreport()
		{
			if (!m_isNewContext)
			{
				return;
			}
			if (m_isOldSnapshot)
			{
				Microsoft.ReportingServices.ReportRendering.SubReport subReport = (Microsoft.ReportingServices.ReportRendering.SubReport)m_renderReportItem;
				if (subReport.Report != null)
				{
					if (m_report == null)
					{
						m_report = new Report(this, m_inSubtotal, subReport, m_renderingContext);
					}
					else
					{
						m_report.UpdateSubReportContents(this, subReport);
					}
				}
				m_noRows = subReport.NoRows;
				m_processedWithError = subReport.ProcessedWithError;
			}
			else
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.SubReport subReport2 = (Microsoft.ReportingServices.ReportIntermediateFormat.SubReport)m_reportItemDef;
				RenderingContext renderingContext = null;
				try
				{
					if (subReport2.ExceededMaxLevel)
					{
						m_errorCode = SubReportErrorCodes.ExceededMaxRecursionLevel;
						m_errorMessage = RPRes.rsExceededMaxRecursionLevel(subReport2.Name);
						FinalizeErrorMessageAndThrow();
					}
					else
					{
						CheckRetrievalStatus(subReport2.RetrievalStatus);
					}
					if (m_renderingContext.InstanceAccessDisallowed)
					{
						renderingContext = GetOrCreateRenderingContext(subReport2, null);
						renderingContext.SubReportHasNoInstance = true;
					}
					else
					{
						m_renderingContext.OdpContext.SetupContext(subReport2, base.Instance.ReportScopeInstance);
						if (subReport2.CurrentSubReportInstance == null)
						{
							renderingContext = GetOrCreateRenderingContext(subReport2, null);
							renderingContext.SubReportHasNoInstance = true;
						}
						else
						{
							Microsoft.ReportingServices.ReportIntermediateFormat.SubReportInstance subReportInstance = subReport2.CurrentSubReportInstance.Value();
							m_noRows = subReportInstance.NoRows;
							m_processedWithError = subReportInstance.ProcessedWithError;
							if (m_processedWithError)
							{
								CheckRetrievalStatus(subReportInstance.RetrievalStatus);
							}
							Microsoft.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance = subReportInstance.ReportInstance.Value();
							renderingContext = GetOrCreateRenderingContext(subReport2, reportInstance);
							renderingContext.OdpContext.LoadExistingSubReportDataChunkNameModifier(subReportInstance);
							renderingContext.OdpContext.SetSubReportContext(subReportInstance, setupReportOM: true);
							reportInstance.SetupEnvironment(renderingContext.OdpContext);
						}
					}
				}
				catch (Exception e)
				{
					m_processedWithError = true;
					ErrorContext subReportErrorContext = null;
					if (subReport2.OdpContext != null)
					{
						subReportErrorContext = subReport2.OdpContext.ErrorContext;
					}
					if (renderingContext == null && m_report != null)
					{
						renderingContext = m_report.RenderingContext;
					}
					Microsoft.ReportingServices.ReportProcessing.ReportProcessing.HandleSubReportProcessingError(m_renderingContext.OdpContext.TopLevelContext.ErrorContext, subReport2, subReport2.UniqueName, subReportErrorContext, e);
				}
				if (renderingContext != null)
				{
					renderingContext.SubReportProcessedWithError = m_processedWithError;
				}
			}
			if (m_processedWithError)
			{
				m_noRows = false;
				if (m_errorCode == SubReportErrorCodes.Success)
				{
					m_errorCode = SubReportErrorCodes.ProcessingError;
					m_errorMessage = RPRes.rsRenderSubreportError;
				}
			}
			m_isNewContext = false;
		}

		private RenderingContext GetOrCreateRenderingContext(Microsoft.ReportingServices.ReportIntermediateFormat.SubReport subReport, Microsoft.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance)
		{
			RenderingContext renderingContext = null;
			if (m_report == null)
			{
				renderingContext = new RenderingContext(m_renderingContext, subReport.OdpContext);
				m_report = new Report(this, subReport.Report, reportInstance, renderingContext, subReport.ReportName, subReport.Description, m_inSubtotal);
			}
			else
			{
				renderingContext = m_report.RenderingContext;
				m_report.SetNewContext(reportInstance);
			}
			return renderingContext;
		}

		private void CheckRetrievalStatus(Microsoft.ReportingServices.ReportIntermediateFormat.SubReport.Status status)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.SubReport subReport = (Microsoft.ReportingServices.ReportIntermediateFormat.SubReport)m_reportItemDef;
			switch (status)
			{
			case Microsoft.ReportingServices.ReportIntermediateFormat.SubReport.Status.NotRetrieved:
			case Microsoft.ReportingServices.ReportIntermediateFormat.SubReport.Status.DefinitionRetrieveFailed:
				m_errorCode = SubReportErrorCodes.MissingSubReport;
				m_errorMessage = RPRes.rsMissingSubReport(subReport.Name, subReport.OriginalCatalogPath);
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.SubReport.Status.DataRetrieveFailed:
				m_errorCode = SubReportErrorCodes.DataRetrievalFailed;
				m_errorMessage = RPRes.rsSubReportDataRetrievalFailed(subReport.Name, subReport.OriginalCatalogPath);
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.SubReport.Status.DataNotRetrieved:
				m_errorCode = SubReportErrorCodes.DataNotRetrieved;
				m_errorMessage = RPRes.rsSubReportDataNotRetrieved(subReport.Name, subReport.OriginalCatalogPath);
				break;
			case Microsoft.ReportingServices.ReportIntermediateFormat.SubReport.Status.ParametersNotSpecified:
				m_errorCode = SubReportErrorCodes.ParametersNotSpecified;
				m_errorMessage = RPRes.rsSubReportParametersNotSpecified(subReport.Name, subReport.OriginalCatalogPath);
				break;
			default:
				m_errorCode = SubReportErrorCodes.Success;
				m_errorMessage = null;
				break;
			}
			FinalizeErrorMessageAndThrow();
		}

		private void FinalizeErrorMessageAndThrow()
		{
			if (m_errorMessage != null)
			{
				IConfiguration configuration = m_renderingContext.OdpContext.Configuration;
				string errorMessage = m_errorMessage;
				if (configuration == null || !configuration.ShowSubreportErrorDetails)
				{
					m_errorMessage = RPRes.rsRenderSubreportError;
				}
				throw new RenderingObjectModelException(errorMessage);
			}
		}

		internal override void UpdateRenderReportItem(Microsoft.ReportingServices.ReportRendering.ReportItem renderReportItem)
		{
			base.UpdateRenderReportItem(renderReportItem);
			SetNewContext();
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			m_isNewContext = true;
			m_noRows = true;
			m_processedWithError = false;
			m_errorCode = SubReportErrorCodes.Success;
		}

		internal override void SetNewContextChildren()
		{
			if (m_report != null)
			{
				m_report.SetNewContext();
			}
		}
	}
}
