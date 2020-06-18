using Microsoft.ReportingServices.OnDemandReportRendering;

namespace Microsoft.ReportingServices.Rendering.SPBProcessing
{
	internal abstract class HeaderFooterEval
	{
		internal static bool AddToCurrentPage(ReportItem reportItem)
		{
			if (reportItem == null)
			{
				return false;
			}
			if (reportItem.Visibility != null && reportItem.Visibility.ToggleItem == null && reportItem.Visibility.Hidden.IsExpression && reportItem.Instance.Visibility.CurrentlyHidden)
			{
				return false;
			}
			return true;
		}

		internal static bool AddToCurrentPage(Microsoft.ReportingServices.OnDemandReportRendering.Tablix tablix)
		{
			TablixInstance tablixInstance = (TablixInstance)tablix.Instance;
			if (tablixInstance.NoRows)
			{
				if (tablix.NoRowsMessage != null)
				{
					string text = null;
					text = ((!tablix.NoRowsMessage.IsExpression) ? tablix.NoRowsMessage.Value : tablixInstance.NoRowsMessage);
					if (text != null)
					{
						return false;
					}
				}
				if (tablix.HideStaticsIfNoRows)
				{
					return false;
				}
			}
			return true;
		}
	}
}
