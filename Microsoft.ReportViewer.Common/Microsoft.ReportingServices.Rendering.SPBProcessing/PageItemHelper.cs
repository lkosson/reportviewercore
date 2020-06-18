using Microsoft.ReportingServices.Diagnostics.Utilities;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.SPBProcessing
{
	internal class PageItemHelper
	{
		private PaginationInfoItems m_type;

		private ItemSizes m_itemPageSizes;

		private PageItem.State m_state;

		private List<int> m_pageItemsAbove;

		private List<int> m_pageItemsLeft;

		private double m_defLeftValue;

		private double m_prevPageEnd;

		private PageItemHelper m_childPage;

		private int m_bodyIndex = -1;

		internal PaginationInfoItems Type => m_type;

		internal ItemSizes ItemPageSizes
		{
			get
			{
				return m_itemPageSizes;
			}
			set
			{
				m_itemPageSizes = value;
			}
		}

		internal PageItem.State State
		{
			get
			{
				return m_state;
			}
			set
			{
				m_state = value;
			}
		}

		internal List<int> PageItemsAbove
		{
			get
			{
				return m_pageItemsAbove;
			}
			set
			{
				m_pageItemsAbove = value;
			}
		}

		internal List<int> PageItemsLeft
		{
			get
			{
				return m_pageItemsLeft;
			}
			set
			{
				m_pageItemsLeft = value;
			}
		}

		internal double PrevPageEnd
		{
			get
			{
				return m_prevPageEnd;
			}
			set
			{
				m_prevPageEnd = value;
			}
		}

		internal PageItemHelper ChildPage
		{
			get
			{
				return m_childPage;
			}
			set
			{
				m_childPage = value;
			}
		}

		internal double DefLeftValue
		{
			get
			{
				return m_defLeftValue;
			}
			set
			{
				m_defLeftValue = value;
			}
		}

		internal int BodyIndex
		{
			get
			{
				return m_bodyIndex;
			}
			set
			{
				m_bodyIndex = value;
			}
		}

		internal PageItemHelper(byte type)
		{
			m_type = (PaginationInfoItems)type;
		}

		internal static PageItemHelper ReadItems(BinaryReader reader, long offsetEndPage)
		{
			if (reader == null || offsetEndPage <= 0)
			{
				return null;
			}
			_ = reader.BaseStream.Position;
			PageItemContainerHelper pageItemContainerHelper = null;
			byte b = reader.ReadByte();
			if (b == 7)
			{
				pageItemContainerHelper = new PageItemContainerHelper(b);
				ReadPageItemContainerProperties(pageItemContainerHelper, reader, offsetEndPage);
			}
			else
			{
				reader.BaseStream.Position--;
			}
			if (reader.BaseStream.Position > offsetEndPage)
			{
				throw new InvalidDataException(SPBRes.InvalidPaginationStream);
			}
			return pageItemContainerHelper;
		}

		private static void ReadRepeatWithItemProperties(PageItemRepeatWithHelper item, BinaryReader reader, long offsetEndPage)
		{
			RSTrace.RenderingTracer.Assert(item != null, "The item helper is null.");
			RSTrace.RenderingTracer.Assert(reader != null, "The pagination stream is null.");
			RSTrace.RenderingTracer.Assert(offsetEndPage > 0, "The pagination stream is corrupt.");
			byte b = reader.ReadByte();
			while (b != byte.MaxValue && reader.BaseStream.Position <= offsetEndPage)
			{
				switch (b)
				{
				case 12:
					item.RelativeTop = reader.ReadDouble();
					break;
				case 13:
					item.RelativeBottom = reader.ReadDouble();
					break;
				case 14:
					item.RelativeTopToBottom = reader.ReadDouble();
					break;
				case 15:
					item.DataRegionIndex = reader.ReadInt32();
					break;
				case 1:
					item.RenderItemSize = new ItemSizes();
					if (item.RenderItemSize.ReadPaginationInfo(reader, offsetEndPage) != 0)
					{
						throw new InvalidDataException(SPBRes.InvalidPaginationStream);
					}
					break;
				case 2:
					item.RenderItemSize = new PaddItemSizes();
					if (item.RenderItemSize.ReadPaginationInfo(reader, offsetEndPage) != 0)
					{
						throw new InvalidDataException(SPBRes.InvalidPaginationStream);
					}
					break;
				case 19:
				{
					byte b2 = reader.ReadByte();
					PageItemHelper pageItemHelper = null;
					switch (b2)
					{
					case 5:
					case 6:
						ReadPageItemContainerProperties((PageItemContainerHelper)(pageItemHelper = new PageItemContainerHelper(b2)), reader, offsetEndPage);
						break;
					case 1:
					case 8:
					case 9:
					case 12:
						pageItemHelper = new PageItemHelper(b2);
						ReadPageItemProperties(pageItemHelper, reader, offsetEndPage);
						break;
					default:
						throw new InvalidDataException(SPBRes.InvalidTokenPaginationItems(b2.ToString("x", CultureInfo.InvariantCulture)));
					}
					item.ChildPage = pageItemHelper;
					break;
				}
				default:
					throw new InvalidDataException(SPBRes.InvalidTokenPaginationProperties(b.ToString("x", CultureInfo.InvariantCulture)));
				}
				b = reader.ReadByte();
			}
			if (reader.BaseStream.Position > offsetEndPage)
			{
				throw new InvalidDataException(SPBRes.InvalidPaginationStream);
			}
		}

		private static void ReadPageItemContainerProperties(PageItemContainerHelper item, BinaryReader reader, long offsetEndPage)
		{
			RSTrace.RenderingTracer.Assert(item != null, "The item helper is null.");
			RSTrace.RenderingTracer.Assert(reader != null, "The pagination stream is null.");
			RSTrace.RenderingTracer.Assert(offsetEndPage > 0, "The pagination stream is corrupt.");
			byte b = reader.ReadByte();
			while (b != byte.MaxValue && reader.BaseStream.Position <= offsetEndPage)
			{
				switch (b)
				{
				case 6:
					item.ItemsCreated = reader.ReadBoolean();
					break;
				case 11:
					item.PrevPageEnd = reader.ReadDouble();
					break;
				case 9:
				{
					if (reader.ReadByte() != 3)
					{
						throw new InvalidDataException(SPBRes.InvalidPaginationStream);
					}
					PageItemHelper pageItemHelper = new PageItemHelper(3);
					ReadPageItemProperties(pageItemHelper, reader, offsetEndPage);
					item.RightEdgeItem = pageItemHelper;
					break;
				}
				case 7:
				{
					int num3 = reader.ReadInt32();
					int[] array3 = new int[num3];
					for (int k = 0; k < num3; k++)
					{
						array3[k] = reader.ReadInt32();
					}
					item.IndexesLeftToRight = array3;
					break;
				}
				case 20:
				{
					int num4 = reader.ReadInt32();
					int[] array4 = new int[num4];
					for (int l = 0; l < num4; l++)
					{
						array4[l] = reader.ReadInt32();
					}
					item.IndexesTopToBottom = array4;
					break;
				}
				case 10:
				{
					int num2 = reader.ReadInt32();
					PageItemHelper[] array2 = new PageItemHelper[num2];
					for (int j = 0; j < num2; j++)
					{
						byte b3 = reader.ReadByte();
						switch (b3)
						{
						case 5:
						case 6:
							ReadPageItemContainerProperties((PageItemContainerHelper)(array2[j] = new PageItemContainerHelper(b3)), reader, offsetEndPage);
							break;
						case 1:
						case 2:
						case 3:
						case 8:
						case 9:
						case 10:
						case 12:
						case 15:
						case 17:
							array2[j] = new PageItemHelper(b3);
							ReadPageItemProperties(array2[j], reader, offsetEndPage);
							break;
						case 4:
							array2[j] = new PageItemHelper(b3);
							ReadSubReportProperties(array2[j], reader, offsetEndPage);
							break;
						case 11:
							ReadTablixProperties((PageTablixHelper)(array2[j] = new PageTablixHelper(b3)), reader, offsetEndPage);
							break;
						case 14:
							reader.ReadByte();
							array2[j] = null;
							break;
						default:
							throw new InvalidDataException(SPBRes.InvalidTokenPaginationItems(b3.ToString("x", CultureInfo.InvariantCulture)));
						}
					}
					item.Children = array2;
					break;
				}
				case 8:
				{
					int num = reader.ReadInt32();
					PageItemRepeatWithHelper[] array = new PageItemRepeatWithHelper[num];
					for (int i = 0; i < num; i++)
					{
						byte b2 = reader.ReadByte();
						array[i] = new PageItemRepeatWithHelper(b2);
						if (b2 != 14)
						{
							ReadRepeatWithItemProperties(array[i], reader, offsetEndPage);
							continue;
						}
						reader.ReadByte();
						array[i] = null;
					}
					item.RepeatWithItems = array;
					break;
				}
				default:
					item.ProcessPageItemToken(b, reader, offsetEndPage);
					break;
				}
				b = reader.ReadByte();
			}
			if (reader.BaseStream.Position > offsetEndPage)
			{
				throw new InvalidDataException(SPBRes.InvalidPaginationStream);
			}
		}

		private static void ReadPageItemProperties(PageItemHelper item, BinaryReader reader, long offsetEndPage)
		{
			RSTrace.RenderingTracer.Assert(item != null, "The item helper is null.");
			RSTrace.RenderingTracer.Assert(reader != null, "The pagination stream is null.");
			RSTrace.RenderingTracer.Assert(offsetEndPage > 0, "The pagination stream is corrupt.");
			byte b = reader.ReadByte();
			while (b != byte.MaxValue && reader.BaseStream.Position <= offsetEndPage)
			{
				item.ProcessPageItemToken(b, reader, offsetEndPage);
				b = reader.ReadByte();
			}
			if (reader.BaseStream.Position > offsetEndPage)
			{
				throw new InvalidDataException(SPBRes.InvalidPaginationStream);
			}
		}

		private static void ReadSubReportProperties(PageItemHelper item, BinaryReader reader, long offsetEndPage)
		{
			RSTrace.RenderingTracer.Assert(item != null, "The item helper is null.");
			RSTrace.RenderingTracer.Assert(reader != null, "The pagination stream is null.");
			RSTrace.RenderingTracer.Assert(offsetEndPage > 0, "The pagination stream is corrupt.");
			byte b = reader.ReadByte();
			while (b != byte.MaxValue && reader.BaseStream.Position <= offsetEndPage)
			{
				switch (b)
				{
				case 23:
					item.BodyIndex = reader.ReadInt32();
					break;
				case 11:
					item.PrevPageEnd = reader.ReadDouble();
					break;
				case 19:
				{
					byte b2 = reader.ReadByte();
					if (b2 != 7)
					{
						throw new InvalidDataException(SPBRes.InvalidTokenPaginationItems(b2.ToString("x", CultureInfo.InvariantCulture)));
					}
					PageItemContainerHelper pageItemContainerHelper = new PageItemContainerHelper(b2);
					ReadPageItemContainerProperties(pageItemContainerHelper, reader, offsetEndPage);
					item.ChildPage = pageItemContainerHelper;
					break;
				}
				default:
					item.ProcessPageItemToken(b, reader, offsetEndPage);
					break;
				}
				b = reader.ReadByte();
			}
			if (reader.BaseStream.Position > offsetEndPage)
			{
				throw new InvalidDataException(SPBRes.InvalidPaginationStream);
			}
		}

		private static void ReadTablixProperties(PageTablixHelper item, BinaryReader reader, long offsetEndPage)
		{
			RSTrace.RenderingTracer.Assert(item != null, "The item helper is null.");
			RSTrace.RenderingTracer.Assert(reader != null, "The pagination stream is null.");
			RSTrace.RenderingTracer.Assert(offsetEndPage > 0, "The pagination stream is corrupt.");
			byte b = reader.ReadByte();
			while (b != byte.MaxValue && reader.BaseStream.Position <= offsetEndPage)
			{
				switch (b)
				{
				case 16:
					item.LevelForRepeat = reader.ReadInt32();
					break;
				case 22:
					item.IgnoreTotalsOnLastLevel = reader.ReadBoolean();
					break;
				case 17:
					item.TablixCreateState = ReadIntList(reader, offsetEndPage);
					break;
				case 18:
					item.MembersInstanceIndex = ReadIntList(reader, offsetEndPage);
					break;
				case 19:
				{
					byte b2 = reader.ReadByte();
					PageItemHelper pageItemHelper = null;
					switch (b2)
					{
					case 5:
					case 6:
						ReadPageItemContainerProperties((PageItemContainerHelper)(pageItemHelper = new PageItemContainerHelper(b2)), reader, offsetEndPage);
						break;
					case 1:
					case 2:
					case 3:
					case 8:
					case 9:
					case 10:
					case 12:
					case 15:
					case 17:
						pageItemHelper = new PageItemHelper(b2);
						ReadPageItemProperties(pageItemHelper, reader, offsetEndPage);
						break;
					case 4:
						pageItemHelper = new PageItemHelper(b2);
						ReadSubReportProperties(pageItemHelper, reader, offsetEndPage);
						break;
					case 11:
						ReadTablixProperties((PageTablixHelper)(pageItemHelper = new PageTablixHelper(b2)), reader, offsetEndPage);
						break;
					default:
						throw new InvalidDataException(SPBRes.InvalidTokenPaginationItems(b2.ToString("x", CultureInfo.InvariantCulture)));
					}
					item.ChildPage = pageItemHelper;
					break;
				}
				default:
					item.ProcessPageItemToken(b, reader, offsetEndPage);
					break;
				}
				b = reader.ReadByte();
			}
			if (reader.BaseStream.Position > offsetEndPage)
			{
				throw new InvalidDataException(SPBRes.InvalidPaginationStream);
			}
		}

		private void ProcessPageItemToken(byte token, BinaryReader reader, long offsetEndPage)
		{
			RSTrace.RenderingTracer.Assert(reader != null, "The pagination stream is null.");
			RSTrace.RenderingTracer.Assert(offsetEndPage > 0, "The pagination stream is corrupt.");
			switch (token)
			{
			case 1:
				m_itemPageSizes = new ItemSizes();
				if (m_itemPageSizes.ReadPaginationInfo(reader, offsetEndPage) != 0)
				{
					throw new InvalidDataException(SPBRes.InvalidPaginationStream);
				}
				break;
			case 2:
				m_itemPageSizes = new PaddItemSizes();
				if (m_itemPageSizes.ReadPaginationInfo(reader, offsetEndPage) != 0)
				{
					throw new InvalidDataException(SPBRes.InvalidPaginationStream);
				}
				break;
			case 3:
				m_state = (PageItem.State)reader.ReadByte();
				break;
			case 21:
				m_defLeftValue = reader.ReadDouble();
				break;
			case 4:
				m_pageItemsAbove = ReadIntList(reader, offsetEndPage);
				break;
			case 5:
				m_pageItemsLeft = ReadIntList(reader, offsetEndPage);
				break;
			default:
				throw new InvalidDataException(SPBRes.InvalidTokenPaginationProperties(token.ToString(CultureInfo.InvariantCulture)));
			}
			if (reader.BaseStream.Position > offsetEndPage)
			{
				throw new InvalidDataException(SPBRes.InvalidPaginationStream);
			}
		}

		private static List<int> ReadIntList(BinaryReader reader, long offsetEndPage)
		{
			RSTrace.RenderingTracer.Assert(reader != null, "The pagination stream is null.");
			RSTrace.RenderingTracer.Assert(offsetEndPage > 0, "The pagination stream is corrupt.");
			List<int> list = null;
			int num = reader.ReadInt32();
			if (num <= 0)
			{
				throw new InvalidDataException(SPBRes.InvalidPaginationStream);
			}
			list = new List<int>(num);
			for (int i = 0; i < num; i++)
			{
				int item = reader.ReadInt32();
				list.Add(item);
			}
			if (reader.BaseStream.Position > offsetEndPage)
			{
				throw new InvalidDataException(SPBRes.InvalidPaginationStream);
			}
			return list;
		}
	}
}
