using Microsoft.ReportingServices.OnDemandReportRendering;

namespace Microsoft.ReportingServices.Rendering.SPBProcessing
{
	internal abstract class WalkTablix
	{
		internal enum State
		{
			RowMembers,
			ColMembers,
			DetailRows
		}

		internal static int AddMembersToCurrentPage(Microsoft.ReportingServices.OnDemandReportRendering.Tablix tablix, TablixMember memberParent, int rowMemberIndexCell, State state, bool createDetail, bool noRows, PageContext context, bool useForPageHFEval, Interactivity interactivity)
		{
			TablixMemberCollection tablixMemberCollection = null;
			if (memberParent == null)
			{
				if (state == State.RowMembers)
				{
					tablixMemberCollection = tablix.RowHierarchy.MemberCollection;
				}
				else
				{
					if (state == State.ColMembers)
					{
						AddCornerToCurrentPage(tablix.Corner, context, useForPageHFEval, interactivity);
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
				if (state == State.RowMembers)
				{
					AddMembersToCurrentPage(tablix, null, memberParent.MemberCellIndex, State.DetailRows, createDetail, noRows, context, useForPageHFEval, interactivity);
				}
				else if (createDetail)
				{
					AddDetailCellToCurrentPage(tablix, memberParent.MemberCellIndex, rowMemberIndexCell, context, useForPageHFEval, interactivity);
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
					if (interactivity == null)
					{
						continue;
					}
					flag3 = false;
				}
				flag = true;
				tablixMemberInstance = tablixMember.Instance;
				if (tablixMember.IsStatic)
				{
					flag2 = WalkTablixMember(tablixMember, ref flag3, interactivity);
				}
				else
				{
					tablixDynamicMemberInstance = (TablixDynamicMemberInstance)tablixMemberInstance;
					tablixDynamicMemberInstance.ResetContext();
					flag = tablixDynamicMemberInstance.MoveNext();
					if (flag)
					{
						flag2 = WalkTablixMember(tablixMember, ref flag3, interactivity);
					}
				}
				while (flag)
				{
					if (flag2)
					{
						int num2 = AddMembersToCurrentPage(tablix, tablixMember, rowMemberIndexCell, state, createDetail, noRows, context, flag3, interactivity);
						if (state != State.DetailRows)
						{
							interactivity?.RegisterGroupLabel(tablixMember.Group, context);
							if (tablixMember.TablixHeader != null)
							{
								if (num2 > 0)
								{
									RegisterItem.RegisterHiddenItem(tablixMember.TablixHeader.CellContents.ReportItem, context, flag3, interactivity);
									num++;
								}
								else if (interactivity != null)
								{
									RegisterItem.RegisterHiddenItem(tablixMember.TablixHeader.CellContents.ReportItem, context, useForPageHFEval: false, interactivity);
								}
							}
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
						flag2 = WalkTablixMember(tablixMember, ref flag3, interactivity);
					}
				}
				tablixMemberInstance = null;
			}
			return num;
		}

		private static void AddCornerToCurrentPage(TablixCorner corner, PageContext context, bool useForPageHFEval, Interactivity interactivity)
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
						RegisterItem.RegisterHiddenItem(tablixCornerRow[j].CellContents.ReportItem, context, useForPageHFEval, interactivity);
					}
				}
			}
		}

		private static bool WalkTablixMember(TablixMember tablixMember, ref bool useForPageHFEval, Interactivity interactivity)
		{
			if (useForPageHFEval && tablixMember.Visibility != null && tablixMember.Visibility.ToggleItem == null && tablixMember.Visibility.Hidden.IsExpression && tablixMember.Instance.Visibility.CurrentlyHidden)
			{
				useForPageHFEval = false;
			}
			if (useForPageHFEval || interactivity != null)
			{
				return true;
			}
			return false;
		}

		internal static void AddDetailCellToCurrentPage(Microsoft.ReportingServices.OnDemandReportRendering.Tablix tablix, int colMemberIndexCell, int rowMemberIndexCell, PageContext context, bool useForPageHFEval, Interactivity interactivity)
		{
			if (rowMemberIndexCell >= 0)
			{
				TablixCell tablixCell = tablix.Body.RowCollection[rowMemberIndexCell][colMemberIndexCell];
				if (tablixCell != null && tablixCell.CellContents != null)
				{
					RegisterItem.RegisterHiddenItem(tablixCell.CellContents.ReportItem, context, useForPageHFEval, interactivity);
				}
			}
		}
	}
}
