namespace Microsoft.ReportingServices.ReportRendering
{
	internal interface ICustomReportItem
	{
		CustomReportItem CustomItem
		{
			set;
		}

		ReportItem RenderItem
		{
			get;
		}

		Action Action
		{
			get;
		}

		ChangeType Process();
	}
}
