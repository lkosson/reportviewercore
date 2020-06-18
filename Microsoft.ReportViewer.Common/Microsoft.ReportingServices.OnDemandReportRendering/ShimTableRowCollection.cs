using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportRendering;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimTableRowCollection : TablixRowCollection
	{
		private List<TablixRow> m_rows;

		public override TablixRow this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, Count);
				}
				return m_rows[index];
			}
		}

		public override int Count => m_rows.Count;

		internal ShimTableRowCollection(Tablix owner)
			: base(owner)
		{
			m_rows = new List<TablixRow>();
			AppendTableRows(owner.RenderTable.TableHeader);
			if (owner.RenderTable.TableGroups != null)
			{
				AppendTableGroups(owner.RenderTable.TableGroups[0]);
			}
			else if (owner.RenderTable.DetailRows != null)
			{
				AppendTableRows(owner.RenderTable.DetailRows[0]);
			}
			AppendTableRows(owner.RenderTable.TableFooter);
		}

		private void AppendTableGroups(Microsoft.ReportingServices.ReportRendering.TableGroup renderGroup)
		{
			if (renderGroup != null)
			{
				AppendTableRows(renderGroup.GroupHeader);
				if (renderGroup.SubGroups != null)
				{
					AppendTableGroups(renderGroup.SubGroups[0]);
				}
				else if (renderGroup.DetailRows != null)
				{
					AppendTableRows(renderGroup.DetailRows[0]);
				}
				AppendTableRows(renderGroup.GroupFooter);
			}
		}

		private void AppendTableRows(TableRowCollection renderRows)
		{
			if (renderRows != null)
			{
				int count = renderRows.DetailRowDefinitions.Count;
				for (int i = 0; i < count; i++)
				{
					m_rows.Add(new ShimTableRow(m_owner, m_rows.Count, renderRows[i]));
				}
			}
		}
	}
}
