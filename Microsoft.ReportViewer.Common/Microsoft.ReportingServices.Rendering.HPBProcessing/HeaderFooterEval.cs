using Microsoft.ReportingServices.OnDemandReportRendering;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.Rendering.HPBProcessing
{
	internal static class HeaderFooterEval
	{
		private enum TablixState
		{
			RowMembers,
			ColMembers,
			DetailRows
		}

		internal static void CollectTextBoxes(ReportItem reportItem, PageContext pageContext, bool useForPageHFEval, Dictionary<string, List<object>> textBoxes)
		{
			if (reportItem == null || !useForPageHFEval || !ShouldBeCollected(reportItem))
			{
				return;
			}
			if (reportItem is Microsoft.ReportingServices.OnDemandReportRendering.TextBox)
			{
				Microsoft.ReportingServices.OnDemandReportRendering.TextBox textBox = (Microsoft.ReportingServices.OnDemandReportRendering.TextBox)reportItem;
				List<object> list = null;
				if (!textBoxes.ContainsKey(textBox.Name))
				{
					list = new List<object>();
					textBoxes[textBox.Name] = list;
				}
				else
				{
					list = textBoxes[textBox.Name];
				}
				list.Add(((TextBoxInstance)textBox.Instance).OriginalValue);
			}
			else if (reportItem is Microsoft.ReportingServices.OnDemandReportRendering.Rectangle)
			{
				CollectTextBoxes(((Microsoft.ReportingServices.OnDemandReportRendering.Rectangle)reportItem).ReportItemCollection, pageContext, useForPageHFEval: true, textBoxes);
			}
			else if (!(reportItem is Microsoft.ReportingServices.OnDemandReportRendering.SubReport) && reportItem is Microsoft.ReportingServices.OnDemandReportRendering.Tablix)
			{
				CollectTextBoxes((Microsoft.ReportingServices.OnDemandReportRendering.Tablix)reportItem, pageContext, useForPageHFEval: true, textBoxes);
			}
		}

		private static void CollectTextBoxes(ReportItemCollection collection, PageContext pageContext, bool useForPageHFEval, Dictionary<string, List<object>> textBoxes)
		{
			if (collection != null && collection.Count != 0)
			{
				for (int i = 0; i < collection.Count; i++)
				{
					CollectTextBoxes(collection[i], pageContext, useForPageHFEval, textBoxes);
				}
			}
		}

		private static void CollectTextBoxes(Microsoft.ReportingServices.OnDemandReportRendering.Tablix tablix, PageContext pageContext, bool useForPageHFEval, Dictionary<string, List<object>> textBoxes)
		{
			if (tablix != null && useForPageHFEval && ShouldBeCollected(tablix))
			{
				TablixInstance tablixInstance = (TablixInstance)tablix.Instance;
				CollectTablixMembersContents(tablix, null, -1, TablixState.ColMembers, tablixInstance.NoRows, pageContext, useForPageHFEval: true, textBoxes);
				CollectTablixMembersContents(tablix, null, 0, TablixState.RowMembers, tablixInstance.NoRows, pageContext, useForPageHFEval: true, textBoxes);
			}
		}

		private static bool ShouldBeCollected(ReportItem reportItem)
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

		private static bool ShouldBeCollected(Microsoft.ReportingServices.OnDemandReportRendering.Tablix tablix)
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

		private static bool ShouldBeCollected(TablixMember tablixMember, ref bool useForPageHFEval)
		{
			if (useForPageHFEval && tablixMember.Visibility != null && tablixMember.Visibility.ToggleItem == null && tablixMember.Visibility.Hidden.IsExpression && tablixMember.Instance.Visibility.CurrentlyHidden)
			{
				useForPageHFEval = false;
			}
			if (useForPageHFEval)
			{
				return true;
			}
			return false;
		}

		private static int CollectTablixMembersContents(Microsoft.ReportingServices.OnDemandReportRendering.Tablix tablix, TablixMember memberParent, int rowMemberIndexCell, TablixState state, bool noRows, PageContext context, bool useForPageHFEval, Dictionary<string, List<object>> textBoxes)
		{
			TablixMemberCollection tablixMemberCollection = null;
			if (memberParent == null)
			{
				if (state == TablixState.RowMembers)
				{
					tablixMemberCollection = tablix.RowHierarchy.MemberCollection;
				}
				else
				{
					if (state == TablixState.ColMembers)
					{
						CollectTablixCornerContents(tablix.Corner, context, useForPageHFEval, textBoxes);
					}
					tablixMemberCollection = tablix.ColumnHierarchy.MemberCollection;
				}
			}
			else
			{
				tablixMemberCollection = memberParent.Children;
			}
			if (tablixMemberCollection == null)
			{
				if (state == TablixState.RowMembers)
				{
					CollectTablixMembersContents(tablix, null, memberParent.MemberCellIndex, TablixState.DetailRows, noRows, context, useForPageHFEval, textBoxes);
				}
				else
				{
					CollectDetailCellContents(tablix, memberParent.MemberCellIndex, rowMemberIndexCell, context, useForPageHFEval, textBoxes);
				}
				if (!useForPageHFEval)
				{
					return 0;
				}
				return 1;
			}
			bool flag = true;
			bool flag2 = true;
			TablixMember tablixMember = null;
			TablixMemberInstance tablixMemberInstance = null;
			TablixDynamicMemberInstance tablixDynamicMemberInstance = null;
			int num = 0;
			bool flag3 = useForPageHFEval;
			for (int i = 0; i < tablixMemberCollection.Count; i++)
			{
				flag3 = useForPageHFEval;
				tablixMember = tablixMemberCollection[i];
				if (noRows && tablixMember.HideIfNoRows)
				{
					continue;
				}
				flag = true;
				tablixMemberInstance = tablixMember.Instance;
				if (tablixMember.IsStatic)
				{
					flag2 = ShouldBeCollected(tablixMember, ref flag3);
				}
				else
				{
					tablixDynamicMemberInstance = (TablixDynamicMemberInstance)tablixMemberInstance;
					tablixDynamicMemberInstance.ResetContext();
					flag = tablixDynamicMemberInstance.MoveNext();
					if (flag)
					{
						flag2 = ShouldBeCollected(tablixMember, ref flag3);
					}
				}
				while (flag)
				{
					if (flag2)
					{
						int num2 = CollectTablixMembersContents(tablix, tablixMember, rowMemberIndexCell, state, noRows, context, flag3, textBoxes);
						if (state != TablixState.DetailRows && tablixMember.TablixHeader != null && num2 > 0)
						{
							CollectTextBoxes(tablixMember.TablixHeader.CellContents.ReportItem, context, flag3, textBoxes);
							num++;
						}
					}
					if (tablixMember.IsStatic)
					{
						flag = false;
						continue;
					}
					flag = tablixDynamicMemberInstance.MoveNext();
					if (flag)
					{
						flag3 = useForPageHFEval;
						flag2 = ShouldBeCollected(tablixMember, ref flag3);
					}
				}
				tablixMemberInstance = null;
			}
			return num;
		}

		private static void CollectTablixCornerContents(TablixCorner corner, PageContext context, bool useForPageHFEval, Dictionary<string, List<object>> textBoxes)
		{
			if (corner == null)
			{
				return;
			}
			TablixCornerRowCollection rowCollection = corner.RowCollection;
			TablixCornerRow tablixCornerRow = null;
			for (int i = 0; i < rowCollection.Count; i++)
			{
				tablixCornerRow = rowCollection[i];
				for (int j = 0; j < tablixCornerRow.Count; j++)
				{
					if (tablixCornerRow[j] != null && tablixCornerRow[j].CellContents != null)
					{
						CollectTextBoxes(tablixCornerRow[j].CellContents.ReportItem, context, useForPageHFEval, textBoxes);
					}
				}
			}
		}

		private static void CollectDetailCellContents(Microsoft.ReportingServices.OnDemandReportRendering.Tablix tablix, int colMemberIndexCell, int rowMemberIndexCell, PageContext context, bool useForPageHFEval, Dictionary<string, List<object>> textBoxes)
		{
			if (rowMemberIndexCell >= 0)
			{
				TablixCell tablixCell = tablix.Body.RowCollection[rowMemberIndexCell][colMemberIndexCell];
				if (tablixCell != null && tablixCell.CellContents != null)
				{
					CollectTextBoxes(tablixCell.CellContents.ReportItem, context, useForPageHFEval, textBoxes);
				}
			}
		}
	}
}
