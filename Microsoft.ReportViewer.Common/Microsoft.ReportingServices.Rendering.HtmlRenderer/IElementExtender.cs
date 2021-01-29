namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
	internal interface IElementExtender
	{
		bool HasSetupRequirements();

		string SetupRequirements();

		bool ShouldApplyToElement(bool isTopLevel);

		string ApplyToElement();
	}
}
