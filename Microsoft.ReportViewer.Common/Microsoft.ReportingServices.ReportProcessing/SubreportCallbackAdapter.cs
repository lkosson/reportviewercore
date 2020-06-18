using Microsoft.Cloud.Platform.Utils;
using Microsoft.ReportingServices.DataExtensions;
using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using System.Diagnostics;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal class SubreportCallbackAdapter
	{
		private ReportProcessing.OnDemandSubReportCallback m_subreportCallback;

		private ReportProcessing.OnDemandSubReportDataSourcesCallback m_subreportDataSourcesCallback;

		private ErrorContext m_errorContext;

		public SubreportCallbackAdapter(ReportProcessing.OnDemandSubReportCallback subreportCallback, ErrorContext errorContext)
		{
			m_subreportCallback = subreportCallback;
			m_errorContext = errorContext;
		}

		public SubreportCallbackAdapter(ReportProcessing.OnDemandSubReportDataSourcesCallback dataSourcesCallback)
		{
			m_subreportDataSourcesCallback = dataSourcesCallback;
		}

		public void SubReportCallback(ICatalogItemContext reportContext, string subreportPath, out ICatalogItemContext subreportContext, out string description, out ReportProcessing.GetReportChunk getCompiledDefinitionCallback, out ParameterInfoCollection parameters)
		{
			getCompiledDefinitionCallback = null;
			IChunkFactory getCompiledDefinitionCallback2 = null;
			m_subreportCallback(reportContext, subreportPath, null, NeedsUpgrade, null, out subreportContext, out description, out getCompiledDefinitionCallback2, out parameters);
			if (getCompiledDefinitionCallback2 != null)
			{
				if (ReportProcessing.ContainsFlag(getCompiledDefinitionCallback2.ReportProcessingFlags, ReportProcessingFlags.OnDemandEngine))
				{
					subreportContext = null;
					description = null;
					getCompiledDefinitionCallback = null;
					parameters = null;
					string text = subreportPath.MarkAsPrivate();
					string text2 = reportContext.ItemPathAsString.MarkAsPrivate();
					Global.Tracer.Trace(TraceLevel.Warning, "The subreport '{0}' could not be processed.  Parent report '{1}' failed to automatically republish, or it contains a Reporting Services 2005-style CustomReportItem, and is therefore incompatible with the subreport. To correct this error, please attempt to republish the parent report manually. If it contains a CustomReportItem, please upgrade the report to the latest version.", text, text2);
					if (m_errorContext != null)
					{
						m_errorContext.Register(ProcessingErrorCode.rsEngineMismatchParentReport, Severity.Warning, ObjectType.Subreport, text, null, text, text2);
					}
					throw new ReportProcessingException(ErrorCode.rsInvalidOperation, RPRes.rsEngineMismatchParentReport(ObjectType.Subreport.ToString(), subreportPath, null, subreportPath, reportContext.ItemPathAsString));
				}
				ChunkFactoryAdapter @object = new ChunkFactoryAdapter(getCompiledDefinitionCallback2);
				getCompiledDefinitionCallback = @object.GetReportChunk;
			}
			else if (subreportContext == null)
			{
				throw new ReportProcessingException(RPRes.rsMissingSubReport(subreportPath, subreportPath), ErrorCode.rsItemNotFound);
			}
		}

		public void SubReportDataSourcesCallback(ICatalogItemContext reportContext, string subreportPath, out ICatalogItemContext subreportContext, out ReportProcessing.GetReportChunk getCompiledDefinitionCallback, out DataSourceInfoCollection dataSources)
		{
			getCompiledDefinitionCallback = null;
			IChunkFactory getCompiledDefinitionCallback2 = null;
			m_subreportDataSourcesCallback(reportContext, subreportPath, NeedsUpgrade, out subreportContext, out getCompiledDefinitionCallback2, out dataSources, out DataSetInfoCollection _);
			if (getCompiledDefinitionCallback2 != null)
			{
				if (ReportProcessing.ContainsFlag(getCompiledDefinitionCallback2.ReportProcessingFlags, ReportProcessingFlags.OnDemandEngine))
				{
					subreportContext = null;
					getCompiledDefinitionCallback = null;
					dataSources = null;
					string text = subreportPath.MarkAsPrivate();
					string text2 = reportContext.ItemPathAsString.MarkAsPrivate();
					Global.Tracer.Trace(TraceLevel.Warning, "The subreport '{0}' could not be processed.  Parent report '{1}' failed to automatically republish, or it contains a Reporting Services 2005-style CustomReportItem, and is therefore incompatible with the subreport. To correct this error, please attempt to republish the parent report manually. If it contains a CustomReportItem, please upgrade the report to the latest version.", text, text2);
					throw new ReportProcessingException(ErrorCode.rsInvalidOperation, RPRes.rsEngineMismatchParentReport(ObjectType.Subreport.ToString(), text, null, text, text2));
				}
				ChunkFactoryAdapter @object = new ChunkFactoryAdapter(getCompiledDefinitionCallback2);
				getCompiledDefinitionCallback = @object.GetReportChunk;
			}
			else if (subreportContext == null)
			{
				throw new ReportProcessingException(RPRes.rsMissingSubReport(subreportPath, subreportPath), ErrorCode.rsItemNotFound);
			}
		}

		public bool NeedsUpgrade(ReportProcessingFlags processingFlags)
		{
			return false;
		}
	}
}
