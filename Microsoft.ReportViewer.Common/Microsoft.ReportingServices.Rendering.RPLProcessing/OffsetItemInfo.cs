using System.IO;

namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class OffsetItemInfo : OffsetInfo, IRPLItemFactory
	{
		internal OffsetItemInfo(long endOffset, RPLContext context)
			: base(endOffset, context)
		{
		}

		public RPLItem GetRPLItem()
		{
			if (m_endOffset <= 0)
			{
				return null;
			}
			byte itemType = 0;
			BinaryReader binaryReader = m_context.BinaryReader;
			long num = RPLReader.ResolveReportItemEnd(m_endOffset, binaryReader, ref itemType);
			switch (itemType)
			{
			case 16:
			{
				RPLItemMeasurement[] children = RPLReader.ReadItemMeasurements(m_context, binaryReader);
				itemType = RPLReader.ReadItemType(num, binaryReader);
				return RPLContainer.CreateItem(num, m_context, children, itemType);
			}
			case 17:
			{
				RPLTablix rPLTablix = new RPLTablix(num, m_context);
				RPLReader.ReadTablixStructure(rPLTablix, m_context, binaryReader);
				return rPLTablix;
			}
			case 18:
				return RPLReader.ReadTextBoxStructure(num, m_context);
			default:
				return RPLItem.CreateItem(num, m_context, itemType);
			}
		}
	}
}
