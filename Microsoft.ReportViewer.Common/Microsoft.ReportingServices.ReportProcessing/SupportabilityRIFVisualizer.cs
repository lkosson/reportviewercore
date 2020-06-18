using Microsoft.ReportingServices.ReportIntermediateFormat;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal class SupportabilityRIFVisualizer
	{
		internal static void DumpTablixes(Microsoft.ReportingServices.ReportIntermediateFormat.Report report)
		{
			StreamWriter streamWriter = new StreamWriter(new FileStream("TablixDump.html", FileMode.Create));
			streamWriter.WriteLine("<html><body>");
			for (int i = 0; i < report.ReportSections.Count; i++)
			{
				ReportSection reportSection = report.ReportSections[i];
				for (int j = 0; j < reportSection.ReportItems.Count; j++)
				{
					if (reportSection.ReportItems[j].ObjectType == ObjectType.Tablix)
					{
						DumpTablix((Microsoft.ReportingServices.ReportIntermediateFormat.Tablix)reportSection.ReportItems[j], streamWriter);
					}
				}
			}
			streamWriter.WriteLine("</body></html>");
			streamWriter.Flush();
			streamWriter.Close();
		}

		private static void DumpTablix(Microsoft.ReportingServices.ReportIntermediateFormat.Tablix tablix, StreamWriter stream)
		{
			stream.Write("Name: ");
			stream.WriteLine(tablix.Name);
			stream.WriteLine("<BR>");
			stream.Write("Width: ");
			stream.WriteLine(tablix.Width);
			stream.WriteLine("<BR>");
			stream.Write("Height: ");
			stream.WriteLine(tablix.Height);
			stream.WriteLine("<BR>");
			if (tablix.InScopeTextBoxes != null)
			{
				stream.WriteLine("<font color=\"darkgreen\"><b>TextBoxesInScope:</b></font> <BR>");
				foreach (Microsoft.ReportingServices.ReportIntermediateFormat.TextBox inScopeTextBox in tablix.InScopeTextBoxes)
				{
					stream.WriteLine("<font color=\"darkgreen\"><b>" + inScopeTextBox.Name + "</b></font> <BR>");
				}
			}
			stream.Write("<div style='border:solid darkblue 1px;width:");
			stream.Write(tablix.Width);
			stream.Write(";height:");
			stream.Write(tablix.Height);
			stream.WriteLine(";'>");
			stream.WriteLine("<Table cellpadding='0' cellspacing='0' rules='all' border='1'>");
			if (tablix.Corner != null)
			{
				stream.Write("<tr><td colspan='");
				stream.Write(tablix.RowHeaderColumnCount.ToString(CultureInfo.InvariantCulture));
				stream.Write("' rowspan='");
				stream.Write(tablix.ColumnHeaderRowCount.ToString(CultureInfo.InvariantCulture));
				stream.Write("'>Corner</td>");
			}
			Queue<TablixMember> queue = new Queue<TablixMember>();
			if (tablix.TablixColumnMembers != null)
			{
				foreach (TablixMember tablixColumnMember in tablix.TablixColumnMembers)
				{
					queue.Enqueue(tablixColumnMember);
				}
				DumpTablixMembers(tablix.TablixColumns, queue, stream, 0, 0);
			}
			Global.Tracer.Assert(queue.Count == 0, "(members.Count == 0)");
			int index = 0;
			if (tablix.TablixRowMembers != null)
			{
				foreach (TablixMember tablixRowMember in tablix.TablixRowMembers)
				{
					DumpTablixMembers(tablix.TablixRows, tablixRowMember, stream, -1, ref index);
				}
			}
			stream.WriteLine("</table>");
			stream.WriteLine("</div>");
		}

		private static void DumpTablixMembers(List<TablixColumn> tablixColumns, Queue<TablixMember> members, StreamWriter stream, int lastLevel, int index)
		{
			if (members.Count == 0)
			{
				stream.Write("</tr>");
				return;
			}
			TablixMember tablixMember = members.Dequeue();
			if (tablixMember.HeaderLevel > lastLevel)
			{
				stream.Write("</tr><tr>");
				lastLevel = tablixMember.HeaderLevel;
			}
			if (tablixMember.ID > -1)
			{
				DumpTablixMember(tablixMember, stream);
			}
			if (tablixMember.SubMembers != null)
			{
				if (tablixMember.RowSpan > 1)
				{
					if (tablixMember.ID > -1)
					{
						TablixMember tablixMember2 = new TablixMember();
						tablixMember2.SubMembers = new TablixMemberList();
						tablixMember2.RowSpan = tablixMember.RowSpan - 1;
						tablixMember2.ID = -1;
						tablixMember2.HeaderLevel = tablixMember.HeaderLevel + 1;
						members.Enqueue(tablixMember2);
						foreach (TablixMember subMember in tablixMember.SubMembers)
						{
							tablixMember2.SubMembers.Add(subMember);
						}
					}
					else
					{
						tablixMember.RowSpan--;
						members.Enqueue(tablixMember);
					}
				}
				else
				{
					foreach (TablixMember subMember2 in tablixMember.SubMembers)
					{
						members.Enqueue(subMember2);
					}
				}
			}
			DumpTablixMembers(tablixColumns, members, stream, lastLevel, index);
		}

		private static void DumpTablixMembers(TablixRowList tablixRows, TablixMember member, StreamWriter stream, int lastLevel, ref int index)
		{
			if (index > lastLevel)
			{
				stream.Write("<tr style='border:solid darkred 1px;height:" + tablixRows[index].Height + ";'>");
				lastLevel = index;
			}
			if (member.ID > -1)
			{
				DumpTablixMember(member, stream);
			}
			if (member.SubMembers != null)
			{
				if (member.ColSpan > 1)
				{
					if (member.ID > -1)
					{
						TablixMember tablixMember = new TablixMember();
						tablixMember.SubMembers = new TablixMemberList();
						tablixMember.ColSpan = member.ColSpan - 1;
						tablixMember.ID = -1;
						tablixMember.HeaderLevel = member.HeaderLevel + 1;
						foreach (TablixMember subMember in member.SubMembers)
						{
							tablixMember.SubMembers.Add(subMember);
						}
						member = tablixMember;
					}
					else
					{
						member.ColSpan--;
					}
					DumpTablixMembers(tablixRows, member, stream, lastLevel, ref index);
					return;
				}
				foreach (TablixMember subMember2 in member.SubMembers)
				{
					DumpTablixMembers(tablixRows, subMember2, stream, lastLevel, ref index);
				}
				return;
			}
			foreach (TablixCell tablixCell in tablixRows[index].TablixCells)
			{
				if (tablixCell.ColSpan <= 0)
				{
					continue;
				}
				stream.Write("<td colspan='");
				stream.Write(tablixCell.ColSpan);
				stream.Write("'>");
				if (tablixCell.CellContents != null)
				{
					stream.Write("<div style='overflow:auto;border:solid blue 1px;width:");
					stream.Write(tablixCell.CellContents.Width);
					stream.Write(";height:");
					stream.Write(tablixCell.CellContents.Height);
					stream.WriteLine(";'>");
					stream.Write("CellContents Type: ");
					stream.WriteLine(tablixCell.CellContents.ObjectType.ToString());
					stream.Write("<BR>Name: <font color=\"darkgreen\"><b>");
					stream.WriteLine(tablixCell.CellContents.Name);
					stream.Write(" </b></font><BR>Width: ");
					stream.WriteLine(tablixCell.CellContents.Width);
					stream.Write(" <BR>Height: ");
					stream.WriteLine(tablixCell.CellContents.Height);
					if (tablixCell.CellContents.ObjectType == ObjectType.Textbox)
					{
						stream.WriteLine("<b>");
						stream.WriteLine("</b><BR>");
					}
					else if (tablixCell.CellContents.ObjectType == ObjectType.Tablix)
					{
						DumpTablix((Microsoft.ReportingServices.ReportIntermediateFormat.Tablix)tablixCell.CellContents, stream);
					}
					stream.Write("</div>");
				}
				stream.Write("</td>");
			}
			index++;
			stream.Write("</tr>");
		}

		private static void DumpTablixMember(TablixMember member, StreamWriter stream)
		{
			if (member.TablixHeader == null)
			{
				return;
			}
			stream.Write("<td ");
			stream.Write("rowspan='");
			stream.Write(member.RowSpan);
			stream.Write("' colSpan='");
			stream.Write(member.ColSpan);
			stream.Write("'>");
			stream.Write("<div style='overflow:auto;border:solid darkgreen 1px;");
			if (member.TablixHeader.CellContents != null)
			{
				stream.Write("height:");
				stream.Write(member.TablixHeader.CellContents.Height);
				stream.Write(";");
				stream.Write("width:");
				stream.Write(member.TablixHeader.CellContents.Width);
				stream.Write(";");
			}
			stream.WriteLine("'>");
			stream.WriteLine("MemberCellIndex = " + member.MemberCellIndex);
			if (member.Grouping != null)
			{
				stream.WriteLine("Dynamic<BR>");
				stream.Write("Grouping ");
				stream.Write("Name: <b><font color=\"darkblue\">");
				stream.WriteLine(member.Grouping.Name);
				stream.WriteLine("</font></b><BR>");
				if (member.Grouping.Variables != null)
				{
					foreach (Variable variable in member.Grouping.Variables)
					{
						stream.WriteLine("<font color=\"darkred\"><b>" + variable.Name + "</b></font> " + variable.Value.OriginalText + "<BR>");
					}
				}
				if (member.InScopeTextBoxes != null)
				{
					stream.WriteLine("<font color=\"darkgreen\"><b>TextBoxesInScope:</b></font> <BR>");
					foreach (Microsoft.ReportingServices.ReportIntermediateFormat.TextBox inScopeTextBox in member.InScopeTextBoxes)
					{
						stream.WriteLine("<font color=\"darkgreen\"><b>" + inScopeTextBox.Name + "</b></font> <BR>");
					}
				}
			}
			else
			{
				stream.WriteLine("Static<BR>");
			}
			stream.WriteLine("RowSpan: " + member.RowSpan + "<BR>");
			stream.WriteLine("ColSpan: " + member.ColSpan + "<BR>");
			if (member.HasConditionalOrToggleableVisibility)
			{
				stream.WriteLine("HasConditionalOrToggleableVisibility<BR>");
			}
			if (member.IsAutoSubtotal)
			{
				stream.WriteLine("IsAutoSubtotal<BR>");
			}
			if (member.IsInnerMostMemberWithHeader)
			{
				stream.WriteLine("IsInnerMostMemberWithHeader<BR>");
			}
			stream.Write("HeaderSize: ");
			stream.WriteLine(member.TablixHeader.Size);
			stream.WriteLine("<BR>");
			if (member.TablixHeader.CellContents != null)
			{
				stream.Write("CellContents Type: ");
				stream.WriteLine(member.TablixHeader.CellContents.ObjectType.ToString());
				stream.WriteLine("<BR>");
				stream.Write(" \tName: <b><font color=\"darkgreen\">");
				stream.WriteLine(member.TablixHeader.CellContents.Name);
				stream.WriteLine("</font></b><BR>");
				stream.Write(" \tWidth: ");
				stream.WriteLine(member.TablixHeader.CellContents.Width);
				stream.WriteLine("<BR>");
				stream.Write(" \tHeight: ");
				stream.WriteLine(member.TablixHeader.CellContents.Height);
				stream.WriteLine("<BR>");
				if (member.TablixHeader.CellContents.ObjectType == ObjectType.Textbox)
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.TextBox textBox = (Microsoft.ReportingServices.ReportIntermediateFormat.TextBox)member.TablixHeader.CellContents;
					stream.Write("<b>");
					stream.Write("</b>");
					if (textBox.UserSort != null)
					{
						stream.WriteLine("sort expr scope: " + textBox.UserSort.SortExpressionScopeString);
						stream.WriteLine("<BR>");
						stream.WriteLine("sort target: " + textBox.UserSort.SortTargetString);
						stream.WriteLine("<BR>");
					}
					stream.WriteLine("<BR>");
				}
				else if (member.TablixHeader.CellContents.ObjectType == ObjectType.Tablix)
				{
					DumpTablix((Microsoft.ReportingServices.ReportIntermediateFormat.Tablix)member.TablixHeader.CellContents, stream);
				}
			}
			stream.Write("</div>");
			stream.Write("</td>");
		}
	}
}
