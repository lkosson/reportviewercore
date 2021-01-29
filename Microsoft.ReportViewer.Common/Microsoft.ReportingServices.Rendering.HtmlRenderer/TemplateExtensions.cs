using System.ComponentModel;

namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
	internal static class TemplateExtensions
	{
		public static string ToFullName(this Template template)
		{
			switch (template)
			{
			case Template.FireTelemetryEvent:
				return "FireTelemetryEvent.html";
			case Template.InitTelemetry:
				return "InitTelemetry.html";
			case Template.HiddenReportUrl:
				return "HiddenReportURL.html";
			case Template.FixPageBackground:
				return "FixPageBackground.html";
			case Template.PageProperties:
				return "PageProperties.html";
			case Template.PinDialog:
				return "PinDialog.html";
			case Template.PinMask:
				return "PinMask.html";
			case Template.ResultDialog:
				return "ResultDialog.html";
			case Template.PrintDialog:
				return "PrintDialog.html";
			case Template.ToolbarStylings:
				return "ToolbarStyling.css";
			default:
				throw new InvalidEnumArgumentException();
			}
		}
	}
}
