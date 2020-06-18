using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal class SubReportInitializer
	{
		internal static void InitializeSubReportOdpContext(Microsoft.ReportingServices.ReportIntermediateFormat.Report report, OnDemandProcessingContext parentOdpContext)
		{
			foreach (Microsoft.ReportingServices.ReportIntermediateFormat.SubReport subReport in report.SubReports)
			{
				if (!subReport.ExceededMaxLevel)
				{
					OnDemandProcessingContext parentOdpContext2 = subReport.OdpContext = new OnDemandProcessingContext(parentOdpContext, subReport.ReportContext, subReport);
					if (subReport.RetrievalStatus != Microsoft.ReportingServices.ReportIntermediateFormat.SubReport.Status.DefinitionRetrieveFailed && subReport.Report.HasSubReports)
					{
						InitializeSubReportOdpContext(subReport.Report, parentOdpContext2);
					}
				}
			}
		}

		internal static bool InitializeSubReports(Microsoft.ReportingServices.ReportIntermediateFormat.Report report, Microsoft.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance, OnDemandProcessingContext odpContext, bool inDataRegion, bool fromCreateSubReportInstance)
		{
			try
			{
				odpContext.IsTopLevelSubReportProcessing = true;
				bool flag = true;
				OnDemandProcessingContext onDemandProcessingContext = odpContext;
				foreach (Microsoft.ReportingServices.ReportIntermediateFormat.SubReport subReport in report.SubReports)
				{
					if (subReport.ExceededMaxLevel)
					{
						return flag;
					}
					IReference<Microsoft.ReportingServices.ReportIntermediateFormat.SubReportInstance> reference = null;
					try
					{
						bool prefetchSuccess = false;
						if (subReport.RetrievalStatus != Microsoft.ReportingServices.ReportIntermediateFormat.SubReport.Status.DefinitionRetrieveFailed)
						{
							onDemandProcessingContext = InitializeSubReport(odpContext, subReport, reportInstance, inDataRegion || subReport.InDataRegion, fromCreateSubReportInstance, out prefetchSuccess);
							if (!inDataRegion && !subReport.InDataRegion && (!odpContext.SnapshotProcessing || odpContext.ReprocessSnapshot))
							{
								reference = subReport.CurrentSubReportInstance;
							}
						}
						if (prefetchSuccess && subReport.Report.HasSubReports)
						{
							flag &= InitializeSubReports(subReport.Report, (subReport.CurrentSubReportInstance != null) ? subReport.CurrentSubReportInstance.Value().ReportInstance.Value() : null, onDemandProcessingContext, inDataRegion || subReport.InDataRegion, fromCreateSubReportInstance);
						}
						if (onDemandProcessingContext.ErrorContext.Messages != null && 0 < onDemandProcessingContext.ErrorContext.Messages.Count)
						{
							odpContext.TopLevelContext.ErrorContext.Register(ProcessingErrorCode.rsWarningExecutingSubreport, Severity.Warning, subReport.ObjectType, subReport.Name, null, onDemandProcessingContext.ErrorContext.Messages);
						}
						flag = (flag && prefetchSuccess);
					}
					catch (Exception e)
					{
						flag = false;
						Microsoft.ReportingServices.ReportProcessing.ReportProcessing.HandleSubReportProcessingError(onDemandProcessingContext.TopLevelContext.ErrorContext, subReport, InstancePathItem.GenerateInstancePathString(subReport.InstancePath), onDemandProcessingContext.ErrorContext, e);
					}
					finally
					{
						reference?.Value().InstanceComplete();
					}
				}
				return flag;
			}
			finally
			{
				odpContext.IsTopLevelSubReportProcessing = false;
			}
		}

		internal static bool InitializeSubReport(Microsoft.ReportingServices.ReportIntermediateFormat.SubReport subReport)
		{
			bool result = false;
			OnDemandProcessingContext onDemandProcessingContext = null;
			try
			{
				onDemandProcessingContext = subReport.OdpContext;
				result = new Merge(subReport.Report, onDemandProcessingContext).InitAndSetupSubReport(subReport);
				if (onDemandProcessingContext.ErrorContext.Messages != null)
				{
					if (0 < onDemandProcessingContext.ErrorContext.Messages.Count)
					{
						onDemandProcessingContext.TopLevelContext.ErrorContext.Register(ProcessingErrorCode.rsWarningExecutingSubreport, Severity.Warning, subReport.ObjectType, subReport.Name, null, onDemandProcessingContext.ErrorContext.Messages);
						return result;
					}
					return result;
				}
				return result;
			}
			catch (Exception e)
			{
				Microsoft.ReportingServices.ReportProcessing.ReportProcessing.HandleSubReportProcessingError(onDemandProcessingContext.TopLevelContext.ErrorContext, subReport, InstancePathItem.GenerateInstancePathString(subReport.InstancePath), onDemandProcessingContext.ErrorContext, e);
				return result;
			}
		}

		private static OnDemandProcessingContext InitializeSubReport(OnDemandProcessingContext parentOdpContext, Microsoft.ReportingServices.ReportIntermediateFormat.SubReport subReport, Microsoft.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance, bool inDataRegion, bool fromCreateSubReportInstance, out bool prefetchSuccess)
		{
			Global.Tracer.Assert(subReport.OdpContext != null, "(null != subReport.OdpContext)");
			prefetchSuccess = true;
			if (!inDataRegion)
			{
				IReference<Microsoft.ReportingServices.ReportIntermediateFormat.SubReportInstance> reference2 = subReport.CurrentSubReportInstance = ((subReport.OdpContext.SnapshotProcessing && !subReport.OdpContext.ReprocessSnapshot) ? reportInstance.SubreportInstances[subReport.IndexInCollection] : Microsoft.ReportingServices.ReportIntermediateFormat.SubReportInstance.CreateInstance(reportInstance, subReport, parentOdpContext.OdpMetadata));
				if (!fromCreateSubReportInstance)
				{
					ReportSection containingSection = subReport.GetContainingSection(parentOdpContext);
					parentOdpContext.SetupContext(containingSection, null);
				}
				Merge merge = new Merge(subReport.Report, subReport.OdpContext);
				prefetchSuccess = merge.InitAndSetupSubReport(subReport);
			}
			return subReport.OdpContext;
		}
	}
}
