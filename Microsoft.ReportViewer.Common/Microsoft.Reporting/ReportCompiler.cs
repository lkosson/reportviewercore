using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Library;
using Microsoft.ReportingServices.ReportProcessing;
using System;

namespace Microsoft.Reporting
{
	internal static class ReportCompiler
	{
		public static PublishingResult CompileReport(ICatalogItemContext context, byte[] reportDefinition, bool generateExpressionHostWithRefusedPermissions, out ControlSnapshot snapshot)
		{
			PublishingResult publishingResult = null;
			snapshot = null;
			AppDomain appDomain = null;
			try
			{
				ReportProcessing reportProcessing = new ReportProcessing();
				snapshot = new ControlSnapshot();
				appDomain = CreateCompilationTempAppDomain();
				PublishingContext reportPublishingContext = new PublishingContext(context, reportDefinition, snapshot, appDomain, generateExpressionHostWithRefusedPermissions, snapshot.ReportProcessingFlags, reportProcessing.Configuration, DataProtectionLocal.Instance);
				return reportProcessing.CreateIntermediateFormat(reportPublishingContext);
			}
			catch (Exception inner)
			{
				string text = context.ItemPathAsString;
				if (text == null)
				{
					text = ProcessingStrings.MainReport;
				}
				throw new DefinitionInvalidException(text, inner);
			}
			finally
			{
				if (appDomain != null)
				{
					AppDomain.Unload(appDomain);
				}
			}
		}

		private static AppDomain CreateCompilationTempAppDomain()
		{
			return AppDomain.CreateDomain("report_compilation_temp_appdomain", null, AppDomain.CurrentDomain.SetupInformation);
		}
	}
}
