using System;
using System.Collections.Generic;

namespace Microsoft.Reporting.WinForms
{
	internal sealed class ColumnControlCollection : List<ParameterPanel>
	{
		public ColumnControlCollection()
		{
		}

		public ColumnControlCollection(IEnumerable<ParameterPanel> panels)
			: base(panels)
		{
		}

		public void GetMaxWidths(out int maxLabelWidth, out int maxColumnWidth)
		{
			maxLabelWidth = 0;
			maxColumnWidth = 0;
			using (Enumerator enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ParameterPanel current = enumerator.Current;
					maxLabelWidth = Math.Max(maxLabelWidth, current.ContainedLabel.OneLineWidth);
					maxColumnWidth = Math.Max(maxColumnWidth, current.NonLabelWidth);
				}
			}
		}
	}
}
