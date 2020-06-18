using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class CustomReportItemHeadingList : TablixHeadingList
	{
		internal new CustomReportItemHeading this[int index] => (CustomReportItemHeading)base[index];

		internal CustomReportItemHeadingList()
		{
		}

		internal CustomReportItemHeadingList(int capacity)
			: base(capacity)
		{
		}

		internal int Initialize(int level, DataCellsList dataRowCells, ref int currentIndex, ref int maxLevel, InitializationContext context)
		{
			int num = Count;
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				Global.Tracer.Assert(this[i] != null);
				if (this[i].Initialize(level, this, i, dataRowCells, ref currentIndex, ref maxLevel, context))
				{
					num++;
					num2 += this[i].HeadingSpan;
				}
			}
			return num2;
		}

		internal void TransferHeadingAggregates()
		{
			int count = Count;
			for (int i = 0; i < count; i++)
			{
				Global.Tracer.Assert(this[i] != null);
				this[i].TransferHeadingAggregates();
			}
		}

		internal override TablixHeadingList InnerHeadings()
		{
			if (Count > 0)
			{
				return this[0].InnerHeadings;
			}
			return null;
		}
	}
}
