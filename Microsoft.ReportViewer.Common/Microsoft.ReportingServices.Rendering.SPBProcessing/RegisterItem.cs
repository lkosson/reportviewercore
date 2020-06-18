using Microsoft.ReportingServices.OnDemandReportRendering;

namespace Microsoft.ReportingServices.Rendering.SPBProcessing
{
	internal abstract class RegisterItem
	{
		internal static void RegisterPageItem(PageItem pageItem, PageContext pageContext, bool useForPageHFEval, Interactivity interactivity)
		{
			if (!useForPageHFEval && interactivity == null)
			{
				return;
			}
			ReportItem source = pageItem.Source;
			bool flag = false;
			if (useForPageHFEval)
			{
				if (pageItem.ItemState == PageItem.State.OnPageHidden)
				{
					if (source.Visibility.ToggleItem != null || !source.Visibility.Hidden.IsExpression)
					{
						flag = true;
					}
				}
				else
				{
					Microsoft.ReportingServices.OnDemandReportRendering.TextBox textBox = source as Microsoft.ReportingServices.OnDemandReportRendering.TextBox;
					if (textBox != null)
					{
						((TextBoxInstance)textBox.Instance).AddToCurrentPage();
					}
				}
			}
			if (interactivity != null && pageItem.ItemState != PageItem.State.OnPageHidden)
			{
				interactivity = null;
			}
			if (flag || interactivity != null)
			{
				RegisterHiddenItem(source, pageContext, flag, interactivity);
			}
		}

		internal static void RegisterHiddenItem(ReportItem reportItem, PageContext pageContext, bool useForPageHFEval, Interactivity interactivity)
		{
			if (reportItem == null)
			{
				return;
			}
			bool flag = false;
			if (useForPageHFEval)
			{
				flag = HeaderFooterEval.AddToCurrentPage(reportItem);
			}
			if (interactivity != null && !interactivity.RegisterHiddenItem(reportItem, pageContext))
			{
				interactivity = null;
			}
			if (!flag && interactivity == null)
			{
				return;
			}
			if (reportItem is Microsoft.ReportingServices.OnDemandReportRendering.TextBox)
			{
				if (flag)
				{
					Microsoft.ReportingServices.OnDemandReportRendering.TextBox textBox = reportItem as Microsoft.ReportingServices.OnDemandReportRendering.TextBox;
					if (textBox != null)
					{
						((TextBoxInstance)textBox.Instance).AddToCurrentPage();
					}
				}
			}
			else if (reportItem is Microsoft.ReportingServices.OnDemandReportRendering.Rectangle)
			{
				RegisterHiddenItem(((Microsoft.ReportingServices.OnDemandReportRendering.Rectangle)reportItem).ReportItemCollection, pageContext, flag, interactivity);
			}
			else if (reportItem is Microsoft.ReportingServices.OnDemandReportRendering.SubReport)
			{
				if (interactivity == null)
				{
					return;
				}
				Microsoft.ReportingServices.OnDemandReportRendering.SubReport subReport = (Microsoft.ReportingServices.OnDemandReportRendering.SubReport)reportItem;
				SubReportInstance subReportInstance = (SubReportInstance)subReport.Instance;
				if (!subReportInstance.ProcessedWithError && !subReportInstance.NoRows)
				{
					for (int i = 0; i < subReport.Report.ReportSections.Count; i++)
					{
						RegisterHiddenItem(subReport.Report.ReportSections[i].Body.ReportItemCollection, pageContext, useForPageHFEval: false, interactivity);
					}
				}
			}
			else if (reportItem is Microsoft.ReportingServices.OnDemandReportRendering.Tablix)
			{
				RegisterHiddenItem((Microsoft.ReportingServices.OnDemandReportRendering.Tablix)reportItem, pageContext, flag, interactivity);
			}
		}

		private static void RegisterHiddenItem(ReportItemCollection collection, PageContext pageContext, bool useForPageHFEval, Interactivity interactivity)
		{
			if (collection == null || collection.Count == 0)
			{
				return;
			}
			for (int i = 0; i < collection.Count; i++)
			{
				RegisterHiddenItem(collection[i], pageContext, useForPageHFEval, interactivity);
				if (!useForPageHFEval && interactivity.Done)
				{
					break;
				}
			}
		}

		private static void RegisterHiddenItem(Microsoft.ReportingServices.OnDemandReportRendering.Tablix tablix, PageContext pageContext, bool useForPageHFEval, Interactivity interactivity)
		{
			if (tablix != null)
			{
				bool flag = false;
				if (useForPageHFEval)
				{
					flag = HeaderFooterEval.AddToCurrentPage(tablix);
				}
				if (flag || interactivity != null)
				{
					TablixInstance tablixInstance = (TablixInstance)tablix.Instance;
					WalkTablix.AddMembersToCurrentPage(tablix, null, -1, WalkTablix.State.ColMembers, createDetail: false, tablixInstance.NoRows, pageContext, flag, interactivity);
					WalkTablix.AddMembersToCurrentPage(tablix, null, 0, WalkTablix.State.RowMembers, createDetail: true, tablixInstance.NoRows, pageContext, flag, interactivity);
				}
			}
		}
	}
}
