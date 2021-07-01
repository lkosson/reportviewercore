using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.Excel;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.SPBIF.ExcelCallbacks.Convert;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.SPBIFReader.Callbacks;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.Util;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;

namespace Microsoft.ReportingServices.Rendering.ExcelRenderer.Layout
{
	internal sealed class LayoutEngine : ALayout
	{
		internal sealed class RowInfo : IStorable, IPersistable
		{
			private int m_height;

			private List<IRowItemStruct> m_items;

			[NonSerialized]
			private static Declaration m_declaration = GetDeclaration();

			internal int Height
			{
				get
				{
					return m_height;
				}
				set
				{
					m_height = value;
				}
			}

			internal List<IRowItemStruct> Items
			{
				get
				{
					if (m_items == null)
					{
						m_items = new List<IRowItemStruct>();
					}
					return m_items;
				}
			}

			public int Size => 4 + ItemSizes.SizeOf(m_items);

			internal RowInfo()
			{
			}

			public void Serialize(IntermediateFormatWriter writer)
			{
				writer.RegisterDeclaration(m_declaration);
				while (writer.NextMember())
				{
					switch (writer.CurrentMember.MemberName)
					{
					case MemberName.Height:
						writer.Write(m_height);
						break;
					case MemberName.Items:
						writer.Write(m_items);
						break;
					default:
						RSTrace.ExcelRendererTracer.Assert(condition: false);
						break;
					}
				}
			}

			public void Deserialize(IntermediateFormatReader reader)
			{
				reader.RegisterDeclaration(m_declaration);
				while (reader.NextMember())
				{
					switch (reader.CurrentMember.MemberName)
					{
					case MemberName.Height:
						m_height = reader.ReadInt32();
						break;
					case MemberName.Items:
						m_items = reader.ReadGenericListOfRIFObjects<IRowItemStruct>();
						break;
					default:
						RSTrace.ExcelRendererTracer.Assert(condition: false);
						break;
					}
				}
			}

			public void ResolveReferences(Dictionary<ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
			{
			}

			public ObjectType GetObjectType()
			{
				return ObjectType.ExcelRowInfo;
			}

			internal static Declaration GetDeclaration()
			{
				if (m_declaration == null)
				{
					List<MemberInfo> list = new List<MemberInfo>();
					list.Add(new MemberInfo(MemberName.Height, Token.Int32));
					list.Add(new MemberInfo(MemberName.Items, ObjectType.RIFObjectList, ObjectType.IRowItemStruct));
					return new Declaration(ObjectType.ExcelRowInfo, ObjectType.None, list);
				}
				return m_declaration;
			}
		}

		internal sealed class ColumnInfo
		{
			private int m_width;

			private Stack<ItemInfo> m_styleStack = new Stack<ItemInfo>();

			private List<TablixMemberInfo> m_tablixStructures;

			private TablixMemberInfo m_lastColumnHeader;

			private IBlockerInfo m_columnBlocker;

			private Stack<TablixBlockerInfo> m_tablixBlockers;

			private byte m_flags;

			private const byte m_outlineLevelMask = 15;

			private const byte m_collapsedMask = 16;

			internal int Width => m_width;

			internal Stack<ItemInfo> Stack => m_styleStack;

			internal List<TablixMemberInfo> TablixStructs
			{
				get
				{
					return m_tablixStructures;
				}
				set
				{
					m_tablixStructures = value;
				}
			}

			internal IBlockerInfo Blocker
			{
				get
				{
					return m_columnBlocker;
				}
				set
				{
					m_columnBlocker = value;
				}
			}

			internal Stack<TablixBlockerInfo> TablixBlockers
			{
				get
				{
					return m_tablixBlockers;
				}
				set
				{
					m_tablixBlockers = value;
				}
			}

			internal TablixMemberInfo LastColumnHeader
			{
				get
				{
					return m_lastColumnHeader;
				}
				set
				{
					m_lastColumnHeader = value;
				}
			}

			internal byte OutlineLevel
			{
				get
				{
					return (byte)(m_flags & 0xF);
				}
				set
				{
					if (value < 8)
					{
						m_flags &= 240;
						m_flags |= (byte)(value & 0xF);
					}
				}
			}

			internal bool Collapsed
			{
				get
				{
					return (m_flags & 0x10) != 0;
				}
				set
				{
					if (value)
					{
						m_flags |= 16;
					}
					else
					{
						m_flags &= 239;
					}
				}
			}

			internal ColumnInfo()
			{
			}

			internal ColumnInfo(int width)
			{
				m_width = width;
			}
		}

		internal interface IBlockerInfo
		{
			int TopRow
			{
				get;
			}

			int RightColumn
			{
				get;
			}

			int BottomRow
			{
				get;
			}

			int LeftColumn
			{
				get;
			}
		}

		internal class ItemInfo : IBlockerInfo
		{
			private int m_rowTop;

			private int m_rowBottom;

			private int m_colLeft;

			private int m_colRight;

			private IColor m_backgroundColor;

			private BorderProperties m_leftBorder;

			private BorderProperties m_topBorder;

			private BorderProperties m_rightBorder;

			private BorderProperties m_bottomBorder;

			private BorderProperties m_diagonal;

			private ushort m_flags;

			private const ushort m_togglePositionMask = 15;

			private const ushort m_typeMask = 4080;

			private const ushort m_isHiddenMask = 4096;

			private const ushort m_isMergedMask = 8192;

			public int TopRow => m_rowTop;

			public int RightColumn => m_colRight;

			public int BottomRow => m_rowBottom;

			public int LeftColumn => m_colLeft;

			internal IColor BackgroundColor => m_backgroundColor;

			public bool IsHidden
			{
				get
				{
					return (m_flags & 0x1000) != 0;
				}
				set
				{
					if (value)
					{
						m_flags |= 4096;
					}
					else
					{
						m_flags &= 61439;
					}
				}
			}

			public TogglePosition TogglePosition
			{
				get
				{
					return (TogglePosition)(m_flags & 0xF);
				}
				set
				{
					m_flags &= 65520;
					m_flags |= (ushort)(value & (TogglePosition)15);
				}
			}

			internal bool IsMerged => (m_flags & 0x2000) != 0;

			internal byte Type => (byte)((m_flags & 0xFF0) >> 4);

			internal ItemInfo()
			{
			}

			internal ItemInfo(int top, int bottom, int colLeft, int colRight, bool isMerged, BorderInfo borders, byte type, bool isHidden, TogglePosition togglePosition)
			{
				m_rowTop = top;
				m_rowBottom = bottom;
				m_colLeft = colLeft;
				m_colRight = colRight;
				m_flags = (ushort)(togglePosition & (TogglePosition)15);
				m_flags |= (ushort)((type << 4) & 0xFF0);
				if (isMerged)
				{
					m_flags |= 8192;
				}
				if (isHidden)
				{
					m_flags |= 4096;
				}
				FillBorders(borders);
			}

			private void FillBorders(BorderInfo borders)
			{
				if (borders != null)
				{
					if (borders.BackgroundColor != null)
					{
						m_backgroundColor = borders.BackgroundColor;
					}
					if (borders.TopBorder != null && !borders.OmitBorderTop)
					{
						m_topBorder = borders.TopBorder;
					}
					if (borders.BottomBorder != null && !borders.OmitBorderBottom)
					{
						m_bottomBorder = borders.BottomBorder;
					}
					if (borders.LeftBorder != null)
					{
						m_leftBorder = borders.LeftBorder;
					}
					if (borders.RightBorder != null)
					{
						m_rightBorder = borders.RightBorder;
					}
					if (borders.Diagonal != null)
					{
						m_diagonal = borders.Diagonal;
					}
				}
			}

			internal void FillBorders(RPLStyleProps style, bool omitBorderTop, bool omitBorderBottom, IExcelGenerator excel)
			{
				BorderInfo.FillAllBorders(style, ref m_leftBorder, ref m_rightBorder, ref m_topBorder, ref m_bottomBorder, ref m_backgroundColor, excel);
				if (omitBorderTop)
				{
					m_topBorder = null;
				}
				if (omitBorderBottom)
				{
					m_bottomBorder = null;
				}
			}

			internal void RenderBackground(IStyle style)
			{
				if (m_backgroundColor != null)
				{
					style.BackgroundColor = m_backgroundColor;
				}
			}

			internal void RenderBorders(IExcelGenerator excel, int currentRow, int currentColumn)
			{
				if (m_diagonal != null)
				{
					m_diagonal.Render(excel.GetCellStyle());
				}
				if (m_topBorder != null && currentRow == m_rowTop)
				{
					m_topBorder.Render(excel.GetCellStyle());
				}
				if (m_bottomBorder != null && currentRow == m_rowBottom)
				{
					m_bottomBorder.Render(excel.GetCellStyle());
				}
				if (m_leftBorder != null && currentColumn == m_colLeft)
				{
					m_leftBorder.Render(excel.GetCellStyle());
				}
				if (m_rightBorder != null && currentColumn == m_colRight)
				{
					m_rightBorder.Render(excel.GetCellStyle());
				}
			}
		}

		internal class TextBoxItemInfo : ItemInfo
		{
			private bool m_canGrow;

			private bool m_canShrink;

			internal bool CanGrow => m_canGrow;

			internal bool CanShrink => m_canShrink;

			internal TextBoxItemInfo()
			{
			}

			internal TextBoxItemInfo(int top, int bottom, int colLeft, int colRight, bool isMerged, bool canGrow, bool canShrink, BorderInfo borders, byte type, bool isHidden, TogglePosition togglePosition)
				: base(top, bottom, colLeft, colRight, isMerged, borders, type, isHidden, togglePosition)
			{
				m_canGrow = canGrow;
				m_canShrink = canShrink;
			}
		}

		internal class TablixItemInfo : IBlockerInfo
		{
			private int m_rowTop;

			private int m_rowBottom;

			private int m_colLeft;

			private int m_colRight;

			public int TopRow => m_rowTop;

			public int RightColumn => m_colRight;

			public int BottomRow => m_rowBottom;

			public int LeftColumn => m_colLeft;

			internal TablixItemInfo()
			{
			}

			public TablixItemInfo(int top, int bottom, int colLeft, int colRight)
			{
				m_rowTop = top;
				m_rowBottom = bottom;
				m_colLeft = colLeft;
				m_colRight = colRight;
			}
		}

		internal class TablixBlockerInfo : TablixItemInfo
		{
			private BlockOutlines m_blockOutlines;

			private int m_bodyTop;

			private int m_bodyLeft;

			private int m_bodyRight;

			public BlockOutlines BlockOutlines
			{
				get
				{
					return m_blockOutlines;
				}
				set
				{
					m_blockOutlines = value;
				}
			}

			public int BodyTopRow => m_bodyTop;

			public int BodyBottomRow => base.BottomRow;

			public int BodyLeftColumn => m_bodyLeft;

			public int BodyRightColumn => m_bodyRight;

			internal TablixBlockerInfo()
			{
			}

			public TablixBlockerInfo(int top, int bottom, int colLeft, int colRight, int bodyTop, int bodyLeft, int bodyRight)
				: base(top, bottom, colLeft, colRight)
			{
				m_bodyTop = bodyTop;
				m_bodyLeft = bodyLeft;
				m_bodyRight = bodyRight;
			}
		}

		internal class TablixMemberInfo : TablixItemInfo
		{
			private byte m_flags;

			private int m_recursiveToggleLevel;

			private const byte m_togglePositionMask = 15;

			private const byte m_hasToggleMask = 16;

			private const byte m_isHiddenMask = 32;

			public bool IsHidden => (m_flags & 0x20) != 0;

			public TogglePosition TogglePosition => (TogglePosition)(m_flags & 0xF);

			public int RecursiveToggleLevel => m_recursiveToggleLevel;

			public bool HasToggle
			{
				get
				{
					return (m_flags & 0x10) != 0;
				}
				set
				{
					if (value)
					{
						m_flags |= 16;
					}
					else
					{
						m_flags &= 239;
					}
				}
			}

			internal TablixMemberInfo()
			{
			}

			public TablixMemberInfo(int top, int bottom, int colLeft, int colRight, TablixMemberStruct tablixMember)
				: base(top, bottom, colLeft, colRight)
			{
				m_flags = (byte)(tablixMember.TogglePosition & (TogglePosition)15);
				if (tablixMember.HasToggle)
				{
					m_flags |= 16;
				}
				if (tablixMember.IsHidden)
				{
					m_flags |= 32;
				}
				m_recursiveToggleLevel = tablixMember.RecursiveToggleLevel;
			}
		}

		internal interface IRowItemStruct : IComparable, IComparable<IRowItemStruct>, IStorable, IPersistable
		{
			object RPLSource
			{
				get;
			}

			int Left
			{
				get;
			}

			int Width
			{
				get;
			}

			int Height
			{
				get;
			}

			int GenerationIndex
			{
				get;
			}

			byte State
			{
				get;
			}
		}

		internal class RowItemStruct : IRowItemStruct, IComparable, IComparable<IRowItemStruct>, IStorable, IPersistable
		{
			[StaticReference]
			private object m_rplSource;

			private int m_left;

			private int m_width;

			private int m_height;

			private int m_generationIndex;

			private byte m_state;

			private bool m_isDefaultLine = true;

			private string m_subreportLanguage;

			private Dictionary<string, ToggleParent> m_toggleParents;

			private bool m_useRPLStream;

			[NonSerialized]
			private static Declaration m_declaration = GetDeclaration();

			public object RPLSource => m_rplSource;

			public int Left => m_left;

			public int Width => m_width;

			public int Height => m_height;

			public int GenerationIndex => m_generationIndex;

			public byte State => m_state;

			public Dictionary<string, ToggleParent> ToggleParents => m_toggleParents;

			internal bool IsDefaultLine
			{
				get
				{
					return m_isDefaultLine;
				}
				set
				{
					m_isDefaultLine = value;
				}
			}

			internal bool UseRPLStream => m_useRPLStream;

			internal string SubreportLanguage => m_subreportLanguage;

			public int Size => 27 + ItemSizes.SizeOf(m_subreportLanguage) + ItemSizes.SizeOf(m_toggleParents);

			internal RowItemStruct()
			{
			}

			internal RowItemStruct(object rplSource, int left, int width, int height, int generationIndex, byte state, string subreportLanguage, Dictionary<string, ToggleParent> toggleParents)
				: this(rplSource, left, width, height, generationIndex, state, subreportLanguage, isDefaultLine: true, toggleParents)
			{
			}

			internal RowItemStruct(object rplSource, int left, int width, int height, int generationIndex, byte state, string subreportLanguage, bool isDefaultLine, Dictionary<string, ToggleParent> toggleParents)
			{
				m_rplSource = rplSource;
				m_left = left;
				m_width = width;
				m_height = height;
				m_generationIndex = generationIndex;
				m_state = state;
				m_isDefaultLine = isDefaultLine;
				m_subreportLanguage = subreportLanguage;
				m_toggleParents = toggleParents;
				m_useRPLStream = (m_rplSource is long);
			}

			public int CompareTo(object obj)
			{
				return CompareTo((IRowItemStruct)obj);
			}

			public int CompareTo(IRowItemStruct other)
			{
				return m_left.CompareTo(other.Left);
			}

			public void Serialize(IntermediateFormatWriter writer)
			{
				writer.RegisterDeclaration(m_declaration);
				IScalabilityCache scalabilityCache = writer.PersistenceHelper as IScalabilityCache;
				while (writer.NextMember())
				{
					switch (writer.CurrentMember.MemberName)
					{
					case MemberName.UseRPLStream:
						writer.Write(m_useRPLStream);
						break;
					case MemberName.RPLSource:
						if (m_useRPLStream)
						{
							long num = -1L;
							try
							{
								num = (long)m_rplSource;
							}
							catch (InvalidCastException)
							{
								RSTrace.ExcelRendererTracer.Assert(condition: false, "The RPL source object is corrupt");
							}
							if (num >= 0)
							{
								writer.Write(num);
							}
							else
							{
								RSTrace.ExcelRendererTracer.Assert(condition: false, "The RPL source object is corrupt");
							}
						}
						else
						{
							int num2 = scalabilityCache.StoreStaticReference(m_rplSource);
							writer.Write((long)num2);
						}
						break;
					case MemberName.Left:
						writer.Write(m_left);
						break;
					case MemberName.Width:
						writer.Write(m_width);
						break;
					case MemberName.Height:
						writer.Write(m_height);
						break;
					case MemberName.GenerationIndex:
						writer.Write(m_generationIndex);
						break;
					case MemberName.State:
						writer.Write(m_state);
						break;
					case MemberName.IsDefaultLine:
						writer.Write(m_isDefaultLine);
						break;
					case MemberName.Language:
						writer.Write(m_subreportLanguage);
						break;
					case MemberName.ToggleParent:
						writer.WriteStringRIFObjectDictionary(m_toggleParents);
						break;
					default:
						RSTrace.ExcelRendererTracer.Assert(condition: false);
						break;
					}
				}
			}

			public void Deserialize(IntermediateFormatReader reader)
			{
				reader.RegisterDeclaration(m_declaration);
				IScalabilityCache scalabilityCache = reader.PersistenceHelper as IScalabilityCache;
				while (reader.NextMember())
				{
					switch (reader.CurrentMember.MemberName)
					{
					case MemberName.UseRPLStream:
						m_useRPLStream = reader.ReadBoolean();
						break;
					case MemberName.RPLSource:
						if (m_useRPLStream)
						{
							long num = reader.ReadInt64();
							if (num >= 0)
							{
								m_rplSource = num;
							}
							else
							{
								RSTrace.ExcelRendererTracer.Assert(condition: false, "The RPL source object is corrupt");
							}
						}
						else
						{
							long num2 = reader.ReadInt64();
							m_rplSource = scalabilityCache.FetchStaticReference((int)num2);
						}
						break;
					case MemberName.Left:
						m_left = reader.ReadInt32();
						break;
					case MemberName.Width:
						m_width = reader.ReadInt32();
						break;
					case MemberName.Height:
						m_height = reader.ReadInt32();
						break;
					case MemberName.GenerationIndex:
						m_generationIndex = reader.ReadInt32();
						break;
					case MemberName.State:
						m_state = reader.ReadByte();
						break;
					case MemberName.IsDefaultLine:
						m_isDefaultLine = reader.ReadBoolean();
						break;
					case MemberName.Language:
						m_subreportLanguage = reader.ReadString();
						break;
					case MemberName.ToggleParent:
						m_toggleParents = reader.ReadStringRIFObjectDictionary<ToggleParent>();
						break;
					default:
						RSTrace.ExcelRendererTracer.Assert(condition: false);
						break;
					}
				}
			}

			public void ResolveReferences(Dictionary<ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
			{
			}

			public ObjectType GetObjectType()
			{
				return ObjectType.RowItemStruct;
			}

			internal static Declaration GetDeclaration()
			{
				if (m_declaration == null)
				{
					List<MemberInfo> list = new List<MemberInfo>();
					list.Add(new MemberInfo(MemberName.UseRPLStream, Token.Boolean));
					list.Add(new MemberInfo(MemberName.RPLSource, Token.Int64));
					list.Add(new MemberInfo(MemberName.Left, Token.Int32));
					list.Add(new MemberInfo(MemberName.Width, Token.Int32));
					list.Add(new MemberInfo(MemberName.Height, Token.Int32));
					list.Add(new MemberInfo(MemberName.GenerationIndex, Token.Int32));
					list.Add(new MemberInfo(MemberName.State, Token.Byte));
					list.Add(new MemberInfo(MemberName.IsDefaultLine, Token.Boolean));
					list.Add(new MemberInfo(MemberName.Language, Token.String));
					list.Add(new MemberInfo(MemberName.ToggleParent, ObjectType.StringRIFObjectDictionary, ObjectType.ToggleParent));
					return new Declaration(ObjectType.RowItemStruct, ObjectType.None, list);
				}
				return m_declaration;
			}
		}

		internal abstract class TablixItemStruct : IRowItemStruct, IComparable, IComparable<IRowItemStruct>, IStorable, IPersistable
		{
			protected int m_left;

			protected int m_width;

			protected int m_height;

			protected int m_generationIndex;

			[NonSerialized]
			private static Declaration m_declaration = GetDeclaration();

			public object RPLSource => null;

			public int Left => m_left;

			public int Width => m_width;

			public int Height => m_height;

			public int GenerationIndex => m_generationIndex;

			public byte State => 0;

			public virtual bool IsHidden => false;

			public virtual TogglePosition TogglePosition
			{
				get
				{
					return TogglePosition.None;
				}
				set
				{
				}
			}

			public virtual int Size => 16;

			protected TablixItemStruct(int left, int width, int height, int generationIndex)
			{
				m_left = left;
				m_width = width;
				m_height = height;
				m_generationIndex = generationIndex;
			}

			public int CompareTo(object obj)
			{
				return CompareTo((IRowItemStruct)obj);
			}

			public int CompareTo(IRowItemStruct other)
			{
				return m_left.CompareTo(other.Left);
			}

			public virtual void Serialize(IntermediateFormatWriter writer)
			{
				writer.RegisterDeclaration(m_declaration);
				while (writer.NextMember())
				{
					switch (writer.CurrentMember.MemberName)
					{
					case MemberName.Left:
						writer.Write(m_left);
						break;
					case MemberName.Width:
						writer.Write(m_width);
						break;
					case MemberName.Height:
						writer.Write(m_height);
						break;
					case MemberName.GenerationIndex:
						writer.Write(m_generationIndex);
						break;
					default:
						RSTrace.ExcelRendererTracer.Assert(condition: false);
						break;
					}
				}
			}

			public virtual void Deserialize(IntermediateFormatReader reader)
			{
				reader.RegisterDeclaration(m_declaration);
				while (reader.NextMember())
				{
					switch (reader.CurrentMember.MemberName)
					{
					case MemberName.Left:
						m_left = reader.ReadInt32();
						break;
					case MemberName.Width:
						m_width = reader.ReadInt32();
						break;
					case MemberName.Height:
						m_height = reader.ReadInt32();
						break;
					case MemberName.GenerationIndex:
						m_generationIndex = reader.ReadInt32();
						break;
					default:
						RSTrace.ExcelRendererTracer.Assert(condition: false);
						break;
					}
				}
			}

			public virtual void ResolveReferences(Dictionary<ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
			{
			}

			public virtual ObjectType GetObjectType()
			{
				return ObjectType.TablixItemStruct;
			}

			internal static Declaration GetDeclaration()
			{
				if (m_declaration == null)
				{
					List<MemberInfo> list = new List<MemberInfo>();
					list.Add(new MemberInfo(MemberName.Left, Token.Int32));
					list.Add(new MemberInfo(MemberName.Width, Token.Int32));
					list.Add(new MemberInfo(MemberName.Height, Token.Int32));
					list.Add(new MemberInfo(MemberName.GenerationIndex, Token.Int32));
					return new Declaration(ObjectType.TablixItemStruct, ObjectType.None, list);
				}
				return m_declaration;
			}
		}

		internal class TablixStruct : TablixItemStruct
		{
			private int m_rowHeaderWidth;

			private int m_columnHeaderHeight;

			private bool m_rtl;

			[NonSerialized]
			private static Declaration m_declaration = GetDeclaration();

			public int ColumnHeaderHeight => m_columnHeaderHeight;

			public int RowHeaderWidth => m_rowHeaderWidth;

			public bool RTL => m_rtl;

			public override int Size => base.Size + 8 + 1;

			internal TablixStruct()
				: base(0, 0, 0, 0)
			{
			}

			internal TablixStruct(int left, int width, int height, int generationIndex, int rowHeaderWidth, int columnHeaderHeight, bool rtl)
				: base(left, width, height, generationIndex)
			{
				m_rowHeaderWidth = rowHeaderWidth;
				m_columnHeaderHeight = columnHeaderHeight;
				m_rtl = rtl;
			}

			public override void Serialize(IntermediateFormatWriter writer)
			{
				base.Serialize(writer);
				writer.RegisterDeclaration(m_declaration);
				while (writer.NextMember())
				{
					switch (writer.CurrentMember.MemberName)
					{
					case MemberName.RowHeaderWidth:
						writer.Write(m_rowHeaderWidth);
						break;
					case MemberName.ColumnHeaderHeight:
						writer.Write(m_columnHeaderHeight);
						break;
					case MemberName.RTL:
						writer.Write(m_rtl);
						break;
					default:
						RSTrace.ExcelRendererTracer.Assert(condition: false);
						break;
					}
				}
			}

			public override void Deserialize(IntermediateFormatReader reader)
			{
				base.Deserialize(reader);
				reader.RegisterDeclaration(m_declaration);
				while (reader.NextMember())
				{
					switch (reader.CurrentMember.MemberName)
					{
					case MemberName.RowHeaderWidth:
						m_rowHeaderWidth = reader.ReadInt32();
						break;
					case MemberName.ColumnHeaderHeight:
						m_columnHeaderHeight = reader.ReadInt32();
						break;
					case MemberName.RTL:
						m_rtl = reader.ReadBoolean();
						break;
					default:
						RSTrace.ExcelRendererTracer.Assert(condition: false);
						break;
					}
				}
			}

			public override void ResolveReferences(Dictionary<ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
			{
			}

			public override ObjectType GetObjectType()
			{
				return ObjectType.TablixStruct;
			}

			internal new static Declaration GetDeclaration()
			{
				if (m_declaration == null)
				{
					List<MemberInfo> list = new List<MemberInfo>();
					list.Add(new MemberInfo(MemberName.RowHeaderWidth, Token.Int32));
					list.Add(new MemberInfo(MemberName.ColumnHeaderHeight, Token.Int32));
					list.Add(new MemberInfo(MemberName.RTL, Token.Boolean));
					return new Declaration(ObjectType.TablixStruct, ObjectType.None, list);
				}
				return m_declaration;
			}
		}

		internal class TablixMemberStruct : TablixItemStruct
		{
			private byte m_flags;

			private string m_uniqueName;

			private int m_recursiveToggleLevel;

			[NonSerialized]
			private const byte m_togglePositionMask = 15;

			[NonSerialized]
			private const byte m_hasToggleMask = 16;

			[NonSerialized]
			private const byte m_isHiddenMask = 32;

			[NonSerialized]
			private const byte m_hasLabelMask = 64;

			[NonSerialized]
			private static Declaration m_declaration = GetDeclaration();

			public bool HasToggle => (m_flags & 0x10) != 0;

			public override bool IsHidden => (m_flags & 0x20) != 0;

			public bool HasLabel => (m_flags & 0x40) != 0;

			public override TogglePosition TogglePosition => (TogglePosition)(m_flags & 0xF);

			public string UniqueName => m_uniqueName;

			public int RecursiveToggleLevel => m_recursiveToggleLevel;

			public override int Size => base.Size + 1 + 4 + ItemSizes.SizeOf(m_uniqueName);

			internal TablixMemberStruct()
				: base(0, 0, 0, 0)
			{
			}

			internal TablixMemberStruct(int left, int width, int height, int generationIndex, bool hasToggle, bool isHidden, TogglePosition togglePosition, bool hasLabel, string uniqueName, int recursiveToggleLevel)
				: base(left, width, height, generationIndex)
			{
				m_flags = (byte)(togglePosition & (TogglePosition)15);
				if (hasToggle)
				{
					m_flags |= 16;
				}
				if (isHidden)
				{
					m_flags |= 32;
				}
				if (hasLabel)
				{
					m_flags |= 64;
				}
				m_uniqueName = uniqueName;
				m_recursiveToggleLevel = recursiveToggleLevel;
			}

			public override void Serialize(IntermediateFormatWriter writer)
			{
				base.Serialize(writer);
				writer.RegisterDeclaration(m_declaration);
				while (writer.NextMember())
				{
					switch (writer.CurrentMember.MemberName)
					{
					case MemberName.Flags:
						writer.Write(m_flags);
						break;
					case MemberName.UniqueName:
						writer.Write(m_uniqueName);
						break;
					case MemberName.RecursiveLevel:
						writer.Write(m_recursiveToggleLevel);
						break;
					default:
						RSTrace.ExcelRendererTracer.Assert(condition: false);
						break;
					}
				}
			}

			public override void Deserialize(IntermediateFormatReader reader)
			{
				base.Deserialize(reader);
				reader.RegisterDeclaration(m_declaration);
				while (reader.NextMember())
				{
					switch (reader.CurrentMember.MemberName)
					{
					case MemberName.Flags:
						m_flags = reader.ReadByte();
						break;
					case MemberName.UniqueName:
						m_uniqueName = reader.ReadString();
						break;
					case MemberName.RecursiveLevel:
						m_recursiveToggleLevel = reader.ReadInt32();
						break;
					default:
						RSTrace.ExcelRendererTracer.Assert(condition: false);
						break;
					}
				}
			}

			public override void ResolveReferences(Dictionary<ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
			{
			}

			public override ObjectType GetObjectType()
			{
				return ObjectType.TablixMemberStruct;
			}

			internal new static Declaration GetDeclaration()
			{
				if (m_declaration == null)
				{
					List<MemberInfo> list = new List<MemberInfo>();
					list.Add(new MemberInfo(MemberName.Flags, Token.Byte));
					list.Add(new MemberInfo(MemberName.UniqueName, Token.String));
					list.Add(new MemberInfo(MemberName.RecursiveLevel, Token.Int32));
					return new Declaration(ObjectType.TablixMemberStruct, ObjectType.None, list);
				}
				return m_declaration;
			}
		}

		private const int MAX_ROW_HEIGHT = 8180;

		private const string InvalidImage = "InvalidImage";

		private static readonly char[] BulletChars = new char[3]
		{
			'•',
			'◦',
			'▪'
		};

		private static readonly char[,] RomanNumerals = new char[3, 3]
		{
			{
				'i',
				'v',
				'x'
			},
			{
				'x',
				'l',
				'c'
			},
			{
				'c',
				'd',
				'm'
			}
		};

		private const double MinimumRowPrecentage = 0.004;

		private const double MinimumColumnPrecentage = 0.001;

		private string m_reportLanguage = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;

		private bool m_renderFooterInBody;

		private bool m_renderHeaderInBody = true;

		private HeaderFooterLayout m_headerLayout;

		private HeaderFooterLayout m_footerLayout;

		private int m_headerHeight;

		private Microsoft.ReportingServices.Rendering.ExcelRenderer.Util.HashSet<int> m_columnEdges;

		private ColumnInfo[] m_columns;

		private Dictionary<int, int> m_columnEdgesMap;

		private ScalableDictionary<int, RowInfo> m_rows;

		private ScalableList<RowInfo> m_rowInfo;

		private Dictionary<int, int> m_rowEdgesMap;

		private bool? m_summaryRowAfter;

		private bool? m_summaryColumnAfter;

		private Stack<IBlockerInfo> m_rowBlockers = new Stack<IBlockerInfo>();

		private IScalabilityCache m_scalabilityCache;

		private bool m_onFirstSection = true;

		private bool m_onLastSection;

		internal override bool HeaderInBody
		{
			get
			{
				if (!m_renderHeaderInBody)
				{
					return !m_onFirstSection;
				}
				return true;
			}
		}

		internal override bool FooterInBody
		{
			get
			{
				if (!m_renderFooterInBody)
				{
					return !m_onLastSection;
				}
				return true;
			}
		}

		internal override bool? SummaryRowAfter
		{
			get
			{
				return m_summaryRowAfter;
			}
			set
			{
				m_summaryRowAfter = value;
			}
		}

		internal override bool? SummaryColumnAfter
		{
			get
			{
				return m_summaryColumnAfter;
			}
			set
			{
				m_summaryColumnAfter = value;
			}
		}

		internal IScalabilityCache ScalabilityCache => m_scalabilityCache;

		internal LayoutEngine(RPLReport report, bool headerInBody, CreateAndRegisterStream streamDelegate)
			: base(report)
		{
			m_renderHeaderInBody = headerInBody;
			InitCache(streamDelegate);
			m_columnEdges = new Microsoft.ReportingServices.Rendering.ExcelRenderer.Util.HashSet<int>();
			m_rows = new ScalableDictionary<int, RowInfo>(1, ScalabilityCache, 113, 13);
		}

		private void AddColumnEdge(int edge)
		{
			m_columnEdges.Add(edge);
		}

		private void AddRowEdge(int edge)
		{
			IDisposable rowRef = null;
			AddRowEdge(edge, returnRowInfo: false, out rowRef);
		}

		private RowInfo AddRowEdge(int edge, bool returnRowInfo, out IDisposable rowRef)
		{
			RowInfo value = null;
			rowRef = null;
			if (m_rows.TryGetAndPin(edge, out value, out rowRef))
			{
				if (returnRowInfo)
				{
					return value;
				}
				if (rowRef != null)
				{
					rowRef.Dispose();
					rowRef = null;
				}
				return null;
			}
			value = new RowInfo();
			if (returnRowInfo)
			{
				rowRef = m_rows.AddAndPin(edge, value);
				return value;
			}
			m_rows.Add(edge, value);
			return null;
		}

		private void AddRowItemStruct(int top, IRowItemStruct rowItem)
		{
			IDisposable rowRef = null;
			int edge = top + rowItem.Height;
			int edge2 = rowItem.Left + rowItem.Width;
			RowInfo rowInfo = AddRowEdge(top, returnRowInfo: true, out rowRef);
			if (rowRef != null)
			{
				rowInfo.Items.Add(rowItem);
				rowRef.Dispose();
			}
			AddRowEdge(edge);
			AddColumnEdge(rowItem.Left);
			AddColumnEdge(edge2);
		}

		internal override void AddReportItem(object rplSource, int top, int left, int width, int height, int generationIndex, byte state, string subreportLanguage, Dictionary<string, ToggleParent> toggleParents)
		{
			RowItemStruct rowItem = new RowItemStruct(rplSource, left, width, height, generationIndex, state, subreportLanguage, toggleParents);
			AddRowItemStruct(top, rowItem);
		}

		internal override void AddStructuralItem(int top, int left, int width, int height, bool isToggglable, int generationIndex, RPLTablixMemberCell member, TogglePosition togglePosition)
		{
			AddRowItemStruct(top, new TablixMemberStruct(left, width, height, ALayout.TablixStructStart + ALayout.TablixStructGenerationOffset * generationIndex - member.TablixMemberDef.Level, isToggglable, member.ToggleCollapse, togglePosition, member.GroupLabel != null, member.UniqueName, member.RecursiveToggleLevel));
		}

		internal override void AddStructuralItem(int top, int left, int width, int height, int generationIndex, int rowHeaderWidth, int columnHeaderHeight, bool rtl)
		{
			AddRowItemStruct(top, new TablixStruct(left, width, height, generationIndex, rowHeaderWidth, columnHeaderHeight, rtl));
		}

		internal override void SetIsLastSection(bool isLastSection)
		{
			m_onLastSection = isLastSection;
		}

		internal override ALayout GetPageHeaderLayout(float width, float height)
		{
			if (!HeaderInBody)
			{
				if (m_headerLayout == null)
				{
					m_headerLayout = new HeaderFooterLayout(m_report, width, height);
				}
				return m_headerLayout;
			}
			if (m_onFirstSection)
			{
				m_headerHeight = LayoutConvert.ConvertMMTo20thPoints(height);
			}
			return this;
		}

		internal override ALayout GetPageFooterLayout(float width, float height)
		{
			if (!FooterInBody)
			{
				if (m_footerLayout == null)
				{
					m_footerLayout = new HeaderFooterLayout(m_report, width, height);
				}
				return m_footerLayout;
			}
			return this;
		}

		internal override void CompleteSection()
		{
			m_onFirstSection = false;
		}

		internal override void CompletePage()
		{
			m_columnEdges.Add(0);
			List<int> list = new List<int>(m_columnEdges);
			list.Sort();
			int num = list.Count - 1;
			m_columns = new ColumnInfo[num];
			m_columnEdgesMap = new Dictionary<int, int>(num + 1);
			int num2 = list[0];
			for (int i = 1; i < list.Count; i++)
			{
				ColumnInfo columnInfo = new ColumnInfo(list[i] - num2);
				m_columns[i - 1] = columnInfo;
				m_columnEdgesMap.Add(num2, i - 1);
				num2 = list[i];
			}
			m_columnEdgesMap.Add(num2, list.Count - 1);
			m_columnEdges = null;
			AddRowEdge(0);
			List<int> list2 = new List<int>(m_rows.Keys);
			list2.Sort();
			int num3 = list2.Count - 1;
			m_rowInfo = new ScalableList<RowInfo>(0, ScalabilityCache, 100);
			m_rowEdgesMap = new Dictionary<int, int>(num3 + 1);
			int num4 = list2[0];
			int num5 = 0;
			for (int j = 1; j < list2.Count; j++)
			{
				int num6 = list2[j] - num4;
				RowInfo value = null;
				using (m_rows.GetAndPin(num4, out value))
				{
					if (value != null)
					{
						value.Height = Math.Min(num6, 8180);
					}
					else
					{
						RSTrace.ExcelRendererTracer.Assert(value != null, "Null row info object");
					}
				}
				m_rowInfo.Add(value);
				m_rows.Remove(num4);
				m_rowEdgesMap.Add(num4, num5);
				num4 += value.Height;
				num5++;
				while (num6 > 8180)
				{
					num6 -= 8180;
					RowInfo rowInfo = new RowInfo();
					rowInfo.Height = Math.Min(num6, 8180);
					m_rowInfo.Add(rowInfo);
					m_rowEdgesMap.Add(num4, num5);
					num4 += rowInfo.Height;
					num5++;
				}
			}
			m_rowEdgesMap.Add(num4, num5);
			RowInfo value2 = null;
			using (m_rows.GetAndPin(num4, out value2))
			{
				if (value2 != null)
				{
					if (value2.Items != null && value2.Items.Count > 0)
					{
						foreach (IRowItemStruct item2 in value2.Items)
						{
							if (m_rowInfo.Count <= 0 || (item2.Height != 0 && item2.Width != 0))
							{
								continue;
							}
							RowItemStruct rowItemStruct = item2 as RowItemStruct;
							RSTrace.ExcelRendererTracer.Assert(rowItemStruct != null, "The row item structure object corresponding to a line cannot be null");
							rowItemStruct.IsDefaultLine = false;
							RowInfo item = null;
							using (m_rowInfo.GetAndPin(m_rowInfo.Count - 1, out item))
							{
								if (item != null)
								{
									item.Items.Add(rowItemStruct);
								}
								else
								{
									RSTrace.ExcelRendererTracer.Assert(item != null, "Null row info object");
								}
							}
						}
					}
				}
				else
				{
					RSTrace.ExcelRendererTracer.Assert(value2 != null, "Null row info object");
				}
			}
			if (m_rows != null)
			{
				m_rows.Clear();
				m_rows = null;
			}
			if (m_columns.Length == 0)
			{
				m_columns = new ColumnInfo[1];
				m_columns[0] = new ColumnInfo(0);
			}
			if (m_rowInfo.Count == 0)
			{
				m_rowInfo.Add(new RowInfo());
			}
		}

		internal void RenderPageToExcel(IExcelGenerator excel, string key, Dictionary<string, BorderInfo> sharedBorderCache, Dictionary<string, ImageInformation> sharedImageCache)
		{
			if (m_columns.Length > excel.MaxColumns)
			{
				throw new ReportRenderingException(ExcelRenderRes.MaxColExceededInSheet(m_columns.Length.ToString(CultureInfo.InvariantCulture), excel.MaxColumns.ToString(CultureInfo.InvariantCulture)));
			}
			if (m_rowInfo.Count > excel.MaxRows)
			{
				throw new ReportRenderingException(ExcelRenderRes.MaxRowExceededInSheet(m_rowInfo.Count.ToString(CultureInfo.InvariantCulture), excel.MaxRows.ToString(CultureInfo.InvariantCulture)));
			}
			m_reportLanguage = m_report.Language;
			RPLPageContent rPLPageContent = m_report.RPLPaginatedPages[0];
			bool isPortrait;
			int pageSizeIndex = PageSizeIndex.GetPageSizeIndex(rPLPageContent.PageLayout.PageWidth, rPLPageContent.PageLayout.PageHeight, out isPortrait);
			float num = 0f;
			if (m_headerLayout != null)
			{
				num = m_headerLayout.Height;
			}
			float num2 = 0f;
			if (m_footerLayout != null)
			{
				num2 = m_footerLayout.Height;
			}
			excel.SetPageContraints(pageSizeIndex, isPortrait, LayoutConvert.ConvertMMToInches(LayoutConvert.ConvertFloatToDouble(rPLPageContent.PageLayout.MarginTop)), LayoutConvert.ConvertMMToInches(LayoutConvert.ConvertFloatToDouble(rPLPageContent.PageLayout.MarginBottom)));
			excel.SetMargins(LayoutConvert.ConvertMMToInches(LayoutConvert.ConvertFloatToDouble(rPLPageContent.PageLayout.MarginTop + num)), LayoutConvert.ConvertMMToInches(LayoutConvert.ConvertFloatToDouble(rPLPageContent.PageLayout.MarginBottom + num2)), LayoutConvert.ConvertMMToInches(LayoutConvert.ConvertFloatToDouble(rPLPageContent.PageLayout.MarginLeft)), LayoutConvert.ConvertMMToInches(LayoutConvert.ConvertFloatToDouble(rPLPageContent.PageLayout.MarginRight)));
			int num3 = 0;
			excel.SetColumnExtents(0, m_columns.Length - 1);
			if (!m_renderHeaderInBody)
			{
				if (m_headerLayout != null)
				{
					m_headerLayout.RenderStrings(m_report, excel, out string left, out string center, out string right);
					excel.AddHeader(left, center, right);
				}
			}
			else if (m_headerHeight > 0)
			{
				excel.AddPrintTitle(0, m_rowEdgesMap[m_headerHeight] - 1);
			}
			if (!m_renderFooterInBody && m_footerLayout != null)
			{
				m_footerLayout.RenderStrings(m_report, excel, out string left2, out string center2, out string right2);
				excel.AddFooter(left2, center2, right2);
			}
			if (m_headerHeight > 0)
			{
				excel.AddFreezePane(m_rowEdgesMap[m_headerHeight], 0);
			}
			int num4 = 0;
			int num5 = m_rowInfo.Count;
			if (1 == num5 && (m_rowInfo[0].Items == null || m_rowInfo[0].Items.Count == 0))
			{
				RenderEmptyPageToExcel(excel, rPLPageContent.PageLayout, sharedBorderCache);
				return;
			}
			int num6 = 0;
			while (num5 > 0)
			{
				int num7 = Math.Min(excel.RowBlockSize, num5);
				for (int i = 0; i < num7; i++)
				{
					int rowIndex = num6 + i;
					excel.AddRow(rowIndex);
				}
				for (int j = 0; j < num7; j++)
				{
					int num8 = num6 + j;
					bool autoSize = false;
					bool flag = false;
					bool rowCanGrow = false;
					bool rowCanShrink = false;
					bool rowCollapsed = false;
					byte rowOutlineLevel = 0;
					RowInfo item = null;
					IEnumerator<IRowItemStruct> enumerator = null;
					using (m_rowInfo.GetAndPin(num8, out item))
					{
						if (item != null)
						{
							item.Items.Sort();
							enumerator = item.Items.GetEnumerator();
						}
						else
						{
							RSTrace.ExcelRendererTracer.Assert(condition: false, "Missing expected scalable list item with index");
						}
					}
					int num9 = -1;
					if (enumerator.MoveNext())
					{
						num9 = enumerator.Current.Left;
					}
					num3 = 0;
					excel.SetRowContext(num8);
					List<IRowItemStruct> list = new List<IRowItemStruct>();
					for (int k = 0; k < m_columns.Length; k++)
					{
						ColumnInfo columnInfo = m_columns[k];
						excel.SetColumnContext(k);
						if (num9 == num3)
						{
							list.Add(enumerator.Current);
							while (enumerator.MoveNext())
							{
								num9 = enumerator.Current.Left;
								if (num9 != num3)
								{
									break;
								}
								list.Add(enumerator.Current);
							}
							if (num3 == num9)
							{
								num9 = -1;
							}
							list.Sort(CompareGenerationIndex);
							bool autosizableGrow = false;
							bool autosizableShrink = false;
							foreach (IRowItemStruct item2 in list)
							{
								RenderNewItem(item2, num4, num8, excel, key, sharedBorderCache, sharedImageCache, ref autosizableGrow, ref autosizableShrink);
							}
							list.Clear();
							autoSize = (autoSize || autosizableGrow || autosizableShrink);
						}
						if (columnInfo.Stack.Count > 0)
						{
							ItemInfo itemInfo = null;
							foreach (ItemInfo item3 in columnInfo.Stack)
							{
								if (item3.TogglePosition != 0)
								{
									switch (item3.TogglePosition)
									{
									case TogglePosition.Above:
									case TogglePosition.Below:
										if (item3.LeftColumn == k)
										{
											rowOutlineLevel = (byte)(rowOutlineLevel + 1);
											if (item3.IsHidden)
											{
												rowCollapsed = true;
											}
										}
										break;
									case TogglePosition.Left:
									case TogglePosition.Right:
										if (item3.TopRow == num8)
										{
											columnInfo.OutlineLevel++;
											if (item3.IsHidden)
											{
												columnInfo.Collapsed = true;
											}
										}
										break;
									}
								}
								if ((!item3.IsMerged || item3.LeftColumn == k) && item3.BackgroundColor != null && itemInfo == null)
								{
									itemInfo = item3;
								}
								item3.RenderBorders(excel, num8, k);
								if (item3.Type == 9 || item3.Type == 11 || item3.Type == 14 || item3.Type == 21)
								{
									flag = true;
								}
								if (!flag && item3.Type == 7)
								{
									CalculateRowFlagsFromTextBoxes(item3, num8, ref rowCanGrow, ref rowCanShrink, ref autoSize);
								}
							}
							if (itemInfo != null)
							{
								itemInfo.RenderBackground(excel.GetCellStyle());
							}
							else if (columnInfo.Stack.Count > 0)
							{
								excel.GetCellStyle().BackgroundColor = null;
							}
						}
						while (columnInfo.Stack.Count > 0 && columnInfo.Stack.Peek().BottomRow <= num8)
						{
							columnInfo.Stack.Pop();
						}
						if (columnInfo.TablixStructs != null)
						{
							foreach (TablixMemberInfo tablixStruct in columnInfo.TablixStructs)
							{
								if (tablixStruct.HasToggle)
								{
									HandleTablixOutline(tablixStruct, num8, k, ref rowOutlineLevel, ref rowCollapsed);
								}
							}
						}
						num3 += columnInfo.Width;
					}
					ColumnInfo[] columns = m_columns;
					foreach (ColumnInfo columnInfo2 in columns)
					{
						if (columnInfo2.TablixStructs != null)
						{
							bool flag2 = false;
							for (int num10 = columnInfo2.TablixStructs.Count - 1; num10 >= 0; num10--)
							{
								TablixMemberInfo tablixMemberInfo = columnInfo2.TablixStructs[num10];
								if (tablixMemberInfo.BottomRow == num8)
								{
									columnInfo2.TablixStructs.RemoveAt(num10);
									if (!flag2 && (tablixMemberInfo.TogglePosition == TogglePosition.Above || tablixMemberInfo.TogglePosition == TogglePosition.Below))
									{
										flag2 = true;
										columnInfo2.LastColumnHeader = tablixMemberInfo;
									}
								}
							}
						}
						if (columnInfo2.TablixBlockers != null)
						{
							while (columnInfo2.TablixBlockers.Count > 0 && columnInfo2.TablixBlockers.Peek().BottomRow == num8)
							{
								columnInfo2.TablixBlockers.Pop();
							}
							if (columnInfo2.TablixBlockers.Count == 0)
							{
								columnInfo2.TablixBlockers = null;
							}
						}
					}
					while (m_rowBlockers.Count > 0 && m_rowBlockers.Peek().BottomRow == num8)
					{
						m_rowBlockers.Pop();
					}
					CalculateAutoSizeFlag(rowCanGrow, rowCanShrink, m_rowInfo[num8].Height, ref autoSize);
					m_rowEdgesMap.Remove(num4);
					num4 += item.Height;
					excel.SetRowProperties(num8, m_rowInfo[num8].Height, rowOutlineLevel, rowCollapsed, autoSize && !flag);
					m_rowInfo[num8] = null;
				}
				num6 += num7;
				num5 -= num7;
			}
			excel.SetSummaryRowAfter(m_summaryRowAfter ?? true);
			excel.SetSummaryColumnToRight(m_summaryColumnAfter ?? true);
			for (int m = 0; m < m_columns.Length; m++)
			{
				ColumnInfo columnInfo3 = m_columns[m];
				double widthInPoints = (double)columnInfo3.Width / 20.0;
				excel.SetColumnProperties(m, widthInPoints, columnInfo3.OutlineLevel, columnInfo3.Collapsed);
			}
		}

		internal void RenderEmptyPageToExcel(IExcelGenerator excel, RPLPageLayout pageLayout, Dictionary<string, BorderInfo> sharedBorderCache)
		{
			if (pageLayout != null)
			{
				excel.AddRow(0);
				excel.SetRowContext(0);
				excel.SetColumnContext(0);
				BorderInfo borderDefinitionFromCache = GetBorderDefinitionFromCache(string.Empty, sharedBorderCache, pageLayout.Style.SharedProperties, omitBorderTop: false, omitBorderBottom: false, excel);
				ItemInfo itemInfo = new ItemInfo(0, 0, 0, 0, isMerged: false, borderDefinitionFromCache, 3, isHidden: false, TogglePosition.None);
				if (pageLayout.Style.NonSharedProperties != null)
				{
					itemInfo.FillBorders(pageLayout.Style.NonSharedProperties, omitBorderTop: false, omitBorderBottom: false, excel);
				}
				ItemInfo itemInfo2 = null;
				if (itemInfo.BackgroundColor != null)
				{
					itemInfo2 = itemInfo;
				}
				itemInfo.RenderBorders(excel, 0, 0);
				itemInfo2?.RenderBackground(excel.GetCellStyle());
				m_rowEdgesMap.Remove(0);
				int heightIn20thPoints = LayoutConvert.ConvertMMTo20thPoints(pageLayout.PageHeight);
				excel.SetRowProperties(0, heightIn20thPoints, 0, collapsed: false, autoSize: false);
				m_rowInfo[0] = null;
				double widthInPoints = LayoutConvert.ConvertMMToPoints(pageLayout.PageWidth);
				excel.SetColumnProperties(0, widthInPoints, 0, collapsed: false);
				m_columns[0] = null;
			}
		}

		private void RenderNewItem(IRowItemStruct item, int top, int topRow, IExcelGenerator excel, string pageContentKey, Dictionary<string, BorderInfo> sharedBorderCache, Dictionary<string, ImageInformation> sharedImageCache, ref bool autosizableGrow, ref bool autosizableShrink)
		{
			bool flag = false;
			int num;
			if (item.Height > 0)
			{
				num = m_rowEdgesMap[top + item.Height] - 1;
				if (num > topRow && m_rowEdgesMap.ContainsKey(top + item.Height - 1) && !(item.RPLSource is RPLImageProps))
				{
					num--;
					flag = true;
				}
			}
			else
			{
				num = topRow;
			}
			int num2 = m_columnEdgesMap[item.Left];
			int num3;
			if (item.Width > 0)
			{
				num3 = m_columnEdgesMap[item.Left + item.Width] - 1;
				if (num3 > num2 && m_columnEdgesMap.ContainsKey(item.Left + item.Width - 1) && !(item.RPLSource is RPLImageProps))
				{
					num3--;
				}
			}
			else
			{
				num3 = num2;
			}
			int width = item.Width;
			bool isColumnBlocker = false;
			if (item is TablixMemberStruct)
			{
				TablixMemberStruct tablixMemberStruct = (TablixMemberStruct)item;
				if (tablixMemberStruct.HasLabel)
				{
					excel.AddBookmarkTarget(tablixMemberStruct.UniqueName);
				}
				TablixMemberInfo tablixMemberInfo = new TablixMemberInfo(topRow, num, num2, num3, tablixMemberStruct);
				if (m_columns[num2].TablixBlockers != null && m_columns[num2].TablixBlockers.Count > 0)
				{
					TablixBlockerInfo tablixBlockerInfo = m_columns[num2].TablixBlockers.Peek();
					if (tablixBlockerInfo != null && tablixBlockerInfo.BlockOutlines != 0)
					{
						switch (tablixMemberStruct.TogglePosition)
						{
						case TogglePosition.Above:
						case TogglePosition.Below:
							if ((tablixBlockerInfo.BlockOutlines & BlockOutlines.Columns) != 0)
							{
								tablixMemberInfo.HasToggle = false;
							}
							break;
						case TogglePosition.Left:
						case TogglePosition.Right:
							if ((tablixBlockerInfo.BlockOutlines & BlockOutlines.Rows) != 0)
							{
								tablixMemberInfo.HasToggle = false;
							}
							break;
						}
					}
				}
				for (int i = num2; i <= num3; i++)
				{
					if (m_columns[i].TablixStructs == null)
					{
						m_columns[i].TablixStructs = new List<TablixMemberInfo>();
					}
					m_columns[i].TablixStructs.Add(tablixMemberInfo);
				}
				return;
			}
			if (item is TablixStruct)
			{
				TablixStruct tablixStruct = (TablixStruct)item;
				int bodyTop = m_rowEdgesMap[top + tablixStruct.ColumnHeaderHeight];
				int bodyLeft = tablixStruct.RTL ? m_columnEdgesMap[tablixStruct.Left] : (m_columnEdgesMap[tablixStruct.Left + tablixStruct.RowHeaderWidth] - 1);
				int bodyRight = tablixStruct.RTL ? (m_columnEdgesMap[tablixStruct.Left + tablixStruct.Width + tablixStruct.RowHeaderWidth] - 1) : (m_columnEdgesMap[tablixStruct.Left + tablixStruct.Width] - 1);
				TablixBlockerInfo tablixBlockerInfo2 = new TablixBlockerInfo(topRow, num, num2, num3, bodyTop, bodyLeft, bodyRight);
				BlockOutlines blockOutlines = BlockOutlines.None;
				if (HandleBlocking(tablixBlockerInfo2, TogglePosition.Left, out isColumnBlocker))
				{
					for (int j = num2; j <= num3; j++)
					{
						m_columns[j].Blocker = tablixBlockerInfo2;
					}
				}
				else
				{
					blockOutlines = BlockOutlines.Columns;
				}
				if (HandleBlocking(tablixBlockerInfo2, TogglePosition.Above, out isColumnBlocker))
				{
					m_rowBlockers.Push(tablixBlockerInfo2);
				}
				else
				{
					blockOutlines |= BlockOutlines.Rows;
				}
				tablixBlockerInfo2.BlockOutlines = blockOutlines;
				for (int k = num2; k <= num3; k++)
				{
					if (m_columns[k].TablixBlockers == null)
					{
						m_columns[k].TablixBlockers = new Stack<TablixBlockerInfo>();
					}
					m_columns[k].TablixBlockers.Push(tablixBlockerInfo2);
				}
				return;
			}
			ItemInfo itemInfo = null;
			byte elementType = 0;
			BorderInfo borderInfo = null;
			RPLPageLayout rPLPageLayout = item.RPLSource as RPLPageLayout;
			RowItemStruct rowItemStruct = item as RowItemStruct;
			RSTrace.ExcelRendererTracer.Assert(rowItemStruct != null, "The row item cannot be null");
			if (rPLPageLayout == null)
			{
				RPLTextBox rPLTextBox = item.RPLSource as RPLTextBox;
				RPLItemProps rPLItemProps;
				if (rPLTextBox != null)
				{
					if (rPLTextBox.StartOffset > 0)
					{
						rPLItemProps = m_report.GetItemProps(rPLTextBox.StartOffset, out elementType);
					}
					else
					{
						rPLItemProps = (RPLItemProps)rPLTextBox.ElementProps;
						elementType = 7;
					}
				}
				else
				{
					rPLItemProps = m_report.GetItemProps(item.RPLSource, out elementType);
				}
				TogglePosition togglePosition = GetTogglePosition(rowItemStruct, (rPLItemProps.Definition as RPLItemPropsDef).ToggleItem, top);
				bool num4 = elementType == 7 || elementType == 11 || elementType == 14 || elementType == 21 || elementType == 9 || elementType == 8;
				bool flag2 = false;
				bool flag3 = false;
				bool flag4 = false;
				if (num4)
				{
					flag3 = (num3 != num2);
					flag4 = (num != topRow);
					if (elementType == 8)
					{
						if (flag3 && flag4)
						{
							excel.AddMergeCell(topRow, num2, num, num3);
							flag2 = true;
						}
						else if (item.Height != 0 && item.Width != 0 && (flag3 || flag4))
						{
							excel.AddMergeCell(topRow, num2, num, num3);
							flag2 = true;
						}
					}
					else if (flag3 || flag4)
					{
						excel.AddMergeCell(topRow, num2, num, num3);
						flag2 = true;
					}
				}
				if (elementType == 8)
				{
					BorderInfo borders = new BorderInfo(rPLItemProps.Style, width, item.Height, ((RPLLinePropsDef)rPLItemProps.Definition).Slant, omitBorderTop: false, omitBorderBottom: false, rowItemStruct.IsDefaultLine, excel);
					itemInfo = new ItemInfo(topRow, num, num2, num3, flag2, borders, elementType, (rowItemStruct.State & 0x20) != 0, togglePosition);
				}
				else
				{
					string text = rPLItemProps.Definition.ID;
					if (text == null)
					{
						text = string.Empty;
					}
					borderInfo = GetBorderDefinitionFromCache(text, sharedBorderCache, rPLItemProps.Definition.SharedStyle, (rowItemStruct.State & 1) != 0, (rowItemStruct.State & 2) != 0, excel);
					if (elementType == 7)
					{
						RPLTextBoxPropsDef rPLTextBoxPropsDef = rPLItemProps.Definition as RPLTextBoxPropsDef;
						itemInfo = new TextBoxItemInfo(topRow, num, num2, num3, flag2, rPLTextBoxPropsDef.CanGrow, rPLTextBoxPropsDef.CanShrink, borderInfo, elementType, (rowItemStruct.State & 0x20) != 0, togglePosition);
					}
					else
					{
						itemInfo = new ItemInfo(topRow, num, num2, num3, flag2, borderInfo, elementType, (rowItemStruct.State & 0x20) != 0, togglePosition);
					}
					if (rPLItemProps.NonSharedStyle != null)
					{
						itemInfo.FillBorders(rPLItemProps.NonSharedStyle, (rowItemStruct.State & 1) != 0, (rowItemStruct.State & 2) != 0, excel);
					}
				}
				if (HandleBlocking(itemInfo, itemInfo.TogglePosition, out isColumnBlocker))
				{
					if (!isColumnBlocker)
					{
						m_rowBlockers.Push(itemInfo);
					}
				}
				else
				{
					itemInfo.TogglePosition = TogglePosition.None;
				}
				RenderItem(excel, sharedImageCache, rPLItemProps, elementType, topRow, num, num2, num3, flag2, ref autosizableGrow, ref autosizableShrink, borderInfo, item);
			}
			else
			{
				bool omitBorderTop = topRow > 0;
				bool omitBorderBottom = m_rowInfo.Count > num + ((!flag) ? 1 : 2);
				borderInfo = GetBorderDefinitionFromCache(string.Empty, sharedBorderCache, rPLPageLayout.Style.SharedProperties, omitBorderTop, omitBorderBottom, excel);
				itemInfo = new ItemInfo(topRow, num, num2, num3, isMerged: false, borderInfo, 3, isHidden: false, TogglePosition.None);
				if (rPLPageLayout.Style.NonSharedProperties != null)
				{
					itemInfo.FillBorders(rPLPageLayout.Style.NonSharedProperties, omitBorderTop, omitBorderBottom, excel);
				}
			}
			for (int l = num2; l <= num3; l++)
			{
				m_columns[l].Stack.Push(itemInfo);
				if (isColumnBlocker)
				{
					m_columns[l].Blocker = itemInfo;
				}
			}
		}

		private void RenderItem(IExcelGenerator excel, Dictionary<string, ImageInformation> sharedImageCache, RPLElementProps props, byte type, int topRow, int bottomRow, int leftColumn, int rightColumn, bool verticallyMerged, ref bool autosizableGrow, ref bool autosizableShrink, BorderInfo borderDef, IRowItemStruct item)
		{
			RPLItemProps rPLItemProps = props as RPLItemProps;
			RPLItemPropsDef rPLItemPropsDef = rPLItemProps.Definition as RPLItemPropsDef;
			if (rPLItemProps.Bookmark != null)
			{
				excel.AddBookmarkTarget(rPLItemProps.Bookmark);
			}
			else if (rPLItemPropsDef.Bookmark != null)
			{
				excel.AddBookmarkTarget(rPLItemPropsDef.Bookmark);
			}
			if (rPLItemProps.Label != null || rPLItemPropsDef.Label != null)
			{
				excel.AddBookmarkTarget(rPLItemProps.UniqueName);
			}
			switch (type)
			{
			case 7:
			{
				RPLTextBoxProps rPLTextBoxProps = rPLItemProps as RPLTextBoxProps;
				RPLTextBoxPropsDef rPLTextBoxPropsDef = rPLTextBoxProps.Definition as RPLTextBoxPropsDef;
				RPLStyleProps sharedStyle = rPLTextBoxPropsDef.SharedStyle;
				if (!verticallyMerged)
				{
					if (rPLTextBoxPropsDef.CanGrow)
					{
						autosizableGrow = true;
					}
					if (rPLTextBoxPropsDef.CanShrink)
					{
						autosizableShrink = true;
					}
				}
				if (!rPLTextBoxPropsDef.IsSimple)
				{
					RenderRichTextBox(excel, rPLTextBoxProps, rPLTextBoxPropsDef, sharedStyle, borderDef, item, excel.GetCellRichTextInfo());
				}
				else
				{
					RenderSimpleTextBox(excel, rPLTextBoxProps, rPLTextBoxPropsDef, sharedStyle, borderDef, item);
				}
				break;
			}
			case 9:
				RenderImage(excel, sharedImageCache, rPLItemProps, topRow, bottomRow, leftColumn, rightColumn, item);
				break;
			case 11:
			case 14:
			case 21:
				RenderDynamicImage(excel, sharedImageCache, rPLItemProps, topRow, bottomRow, leftColumn, rightColumn, item);
				break;
			}
		}

		private void RenderDynamicImage(IExcelGenerator excel, Dictionary<string, ImageInformation> sharedImageCache, RPLElementProps itemProps, int topRow, int bottomRow, int leftColumn, int rightColumn, IRowItemStruct item)
		{
			Stream stream = null;
			bool isShared = false;
			bool flag = false;
			RPLDynamicImageProps rPLDynamicImageProps = itemProps as RPLDynamicImageProps;
			ImageInformation imageInformation = new ImageInformation();
			if (rPLDynamicImageProps.DynamicImageContent != null)
			{
				stream = excel.CreateStream(rPLDynamicImageProps.UniqueName);
				byte[] array = new byte[4096];
				rPLDynamicImageProps.DynamicImageContent.Position = 0L;
				int num = (int)rPLDynamicImageProps.DynamicImageContent.Length;
				while (num > 0)
				{
					int num2 = rPLDynamicImageProps.DynamicImageContent.Read(array, 0, Math.Min(array.Length, num));
					stream.Write(array, 0, num2);
					num -= num2;
				}
			}
			else if (rPLDynamicImageProps.DynamicImageContentOffset > 0)
			{
				stream = excel.CreateStream(rPLDynamicImageProps.UniqueName);
				m_report.GetImage(rPLDynamicImageProps.DynamicImageContentOffset, stream);
			}
			else
			{
				imageInformation = GetInvalidImage(excel, sharedImageCache, ref isShared);
				flag = true;
			}
			if (!flag)
			{
				imageInformation.ImageData = stream;
				imageInformation.ImageName = rPLDynamicImageProps.UniqueName;
				imageInformation.Sizings = RPLFormat.Sizings.Fit;
				imageInformation.ImageFormat = ImageFormat.Png;
			}
			RenderImage(imageInformation, item, excel, sharedImageCache, flag, topRow, leftColumn, bottomRow, rightColumn);
		}

		private void RenderImage(IExcelGenerator excel, Dictionary<string, ImageInformation> sharedImageCache, RPLElementProps itemProps, int topRow, int bottomRow, int leftColumn, int rightColumn, IRowItemStruct item)
		{
			ImageInformation imageInformation = null;
			bool invalidImage = false;
			bool isShared = false;
			string text = null;
			RPLImageProps rPLImageProps = itemProps as RPLImageProps;
			if (rPLImageProps.Image != null)
			{
				text = rPLImageProps.Image.ImageName;
				if (text == null)
				{
					text = rPLImageProps.UniqueName;
					imageInformation = new ImageInformation();
					imageInformation.ReadImage(excel, rPLImageProps.Image, text, m_report);
					if (imageInformation.ImageData == null || imageInformation.ImageData.Length == 0L)
					{
						imageInformation = GetInvalidImage(excel, sharedImageCache, ref isShared);
						invalidImage = true;
					}
				}
				else
				{
					isShared = true;
					if (sharedImageCache.ContainsKey(text))
					{
						imageInformation = sharedImageCache[text];
					}
					else
					{
						imageInformation = new ImageInformation();
						imageInformation.ReadImage(excel, rPLImageProps.Image, text, m_report);
						if (imageInformation.ImageData == null || imageInformation.ImageData.Length == 0L)
						{
							imageInformation = GetInvalidImage(excel, sharedImageCache, ref isShared);
							invalidImage = true;
						}
						else
						{
							sharedImageCache.Add(text, imageInformation);
						}
					}
				}
			}
			else
			{
				imageInformation = GetInvalidImage(excel, sharedImageCache, ref isShared);
				invalidImage = true;
			}
			imageInformation.Sizings = (itemProps.Definition as RPLImagePropsDef).Sizing;
			if (rPLImageProps.ActionInfo != null)
			{
				RPLAction[] actions = rPLImageProps.ActionInfo.Actions;
				foreach (RPLAction rPLAction in actions)
				{
					if (rPLAction.BookmarkLink != null)
					{
						imageInformation.HyperlinkURL = rPLAction.BookmarkLink;
						imageInformation.HyperlinkIsBookmark = true;
					}
					else if (rPLAction.DrillthroughUrl != null)
					{
						imageInformation.HyperlinkURL = rPLAction.DrillthroughUrl;
						imageInformation.HyperlinkIsBookmark = false;
					}
					else if (rPLAction.Hyperlink != null)
					{
						imageInformation.HyperlinkURL = rPLAction.Hyperlink;
						imageInformation.HyperlinkIsBookmark = false;
					}
				}
			}
			imageInformation.Paddings = GetImagePaddings(rPLImageProps.Style);
			RenderImage(imageInformation, item, excel, sharedImageCache, invalidImage, topRow, leftColumn, bottomRow, rightColumn);
		}

		private void RenderSimpleTextBox(IExcelGenerator excel, RPLTextBoxProps textBox, RPLTextBoxPropsDef textBoxDef, RPLStyleProps defStyle, BorderInfo borderDef, IRowItemStruct item)
		{
			string text = null;
			string text2 = null;
			string text3 = null;
			object obj = null;
			object obj2 = null;
			if (textBox.ProcessedWithError)
			{
				excel.SetCellError(ExcelErrorCode.ValueError);
			}
			else
			{
				text = (string)defStyle[23];
				text2 = (string)defStyle[32];
				text3 = (string)defStyle[36];
				obj = defStyle[38];
				obj2 = defStyle[37];
				if (textBoxDef.Value != null)
				{
					excel.SetCellValue(textBoxDef.Value, TypeCode.String);
				}
				if (textBox.TypeCode != 0)
				{
					RenderTextBoxValue(excel, textBox.Value, textBox.OriginalValue, textBox.TypeCode);
				}
				else
				{
					RenderTextBoxValue(excel, textBox.Value, textBox.OriginalValue, textBoxDef.SharedTypeCode);
				}
			}
			RenderActions(excel, textBox.ActionInfo);
			TextOrientation textOrientation = TextOrientation.Horizontal;
			HorizontalAlignment horizontalAlign = HorizontalAlignment.General;
			VerticalAlignment verticalAlign = VerticalAlignment.Top;
			TextDirection textDirection = TextDirection.LeftToRight;
			if (!excel.UseCachedStyle(textBoxDef.ID))
			{
				excel.DefineCachedStyle(textBoxDef.ID);
				RenderTextBoxStyle(excel, defStyle, ref textOrientation, ref horizontalAlign, ref verticalAlign, ref textDirection);
				borderDef.RenderBorders(excel);
				if (!textBoxDef.FormattedValueExpressionBased && !textBox.ProcessedWithError)
				{
					RenderFormat(excel, text, null, null, textBoxDef.SharedTypeCode, text2, text3, obj, obj2, ref horizontalAlign);
				}
				excel.EndCachedStyle();
				excel.UseCachedStyle(textBoxDef.ID);
			}
			else
			{
				textOrientation = GetTextOrientation(defStyle);
				UpdateHorizontalAlign(excel, defStyle, ref horizontalAlign);
				UpdateVerticalAlign(defStyle, ref verticalAlign);
				UpdateDirection(defStyle, ref textDirection);
			}
			if (textBox.NonSharedStyle != null)
			{
				RenderTextBoxStyle(excel, textBox.NonSharedStyle, ref textOrientation, ref horizontalAlign, ref verticalAlign, ref textDirection);
			}
			excel.SetModifiedRotationForEastAsianChars(textOrientation == TextOrientation.Rotate90);
			bool flag = false;
			if (!textBox.ProcessedWithError)
			{
				TypeCode typeCode = (textBox.TypeCode == TypeCode.Empty) ? textBoxDef.SharedTypeCode : textBox.TypeCode;
				flag = FormatHandler.IsExcelNumberDataType(typeCode);
				if (flag)
				{
					string text4 = null;
					string text5 = null;
					string text6 = null;
					object obj3 = null;
					object obj4 = null;
					bool flag2 = false;
					if (textBox.NonSharedStyle != null)
					{
						text4 = (string)textBox.NonSharedStyle[23];
						if (text4 == null)
						{
							text4 = text;
						}
						else
						{
							flag2 = true;
						}
						text5 = (string)textBox.NonSharedStyle[32];
						if (text5 == null)
						{
							text5 = text2;
						}
						else
						{
							flag2 = true;
						}
						text6 = (string)textBox.NonSharedStyle[36];
						if (text6 == null)
						{
							text6 = text3;
						}
						else
						{
							flag2 = true;
						}
						obj3 = textBox.NonSharedStyle[38];
						if (obj3 == null)
						{
							obj3 = obj;
						}
						else
						{
							flag2 = true;
						}
						obj4 = textBox.NonSharedStyle[37];
						if (obj4 == null)
						{
							obj4 = obj2;
						}
						else
						{
							flag2 = true;
						}
						if (textBox.NonSharedStyle[25] != null)
						{
							flag2 = true;
						}
					}
					if (text5 == null)
					{
						string subreportLanguage = ((RowItemStruct)item).SubreportLanguage;
						if (!string.IsNullOrEmpty(subreportLanguage))
						{
							text5 = subreportLanguage;
							flag2 = true;
						}
					}
					if (textBoxDef.FormattedValueExpressionBased)
					{
						RenderFormat(excel, (text4 != null) ? text4 : text, textBox.OriginalValue, textBox.Value, typeCode, (text5 != null) ? text5 : text2, (text6 != null) ? text6 : text3, (obj3 != null) ? obj3 : obj, (obj4 != null) ? obj4 : obj2, ref horizontalAlign);
					}
					else if (flag2)
					{
						RenderFormat(excel, text4, textBox.OriginalValue, textBox.Value, typeCode, text5, text6, obj3, obj4, ref horizontalAlign);
					}
				}
			}
			FixupAlignments(excel, flag, textOrientation, horizontalAlign, verticalAlign, textDirection);
		}

		private void FixupAlignments(IExcelGenerator excel, bool isNumberType, TextOrientation textOrientation, HorizontalAlignment horizontalAlign, VerticalAlignment verticalAlign, TextDirection textDirection)
		{
			if (textOrientation == TextOrientation.Horizontal)
			{
				if (horizontalAlign == HorizontalAlignment.General && textDirection == TextDirection.RightToLeft)
				{
					horizontalAlign = ResolveGeneralAlignment(textDirection, isNumberType);
					excel.GetCellStyle().HorizontalAlignment = horizontalAlign;
				}
				return;
			}
			if (horizontalAlign == HorizontalAlignment.General)
			{
				horizontalAlign = ResolveGeneralAlignment(textDirection, isNumberType);
			}
			bool isClockwise = textOrientation == TextOrientation.Rotate90;
			excel.GetCellStyle().HorizontalAlignment = LayoutConvert.RotateVerticalToHorizontalAlign(verticalAlign, isClockwise);
			excel.GetCellStyle().VerticalAlignment = LayoutConvert.RotateHorizontalToVerticalAlign(horizontalAlign, isClockwise);
		}

		private HorizontalAlignment ResolveGeneralAlignment(TextDirection textDirection, bool isNumberType)
		{
			bool flag = textDirection == TextDirection.RightToLeft;
			if (isNumberType)
			{
				if (flag)
				{
					return HorizontalAlignment.Left;
				}
				return HorizontalAlignment.Right;
			}
			if (flag)
			{
				return HorizontalAlignment.Right;
			}
			return HorizontalAlignment.Left;
		}

		private void RenderRichTextBox(IExcelGenerator excel, RPLTextBoxProps textBox, RPLTextBoxPropsDef textBoxDef, RPLStyleProps defStyle, BorderInfo borderDef, IRowItemStruct item, IRichTextInfo richTextInfo)
		{
			RPLActionInfo actionInfo = textBox.ActionInfo;
			TextOrientation textOrientation = TextOrientation.Horizontal;
			HorizontalAlignment horizontalAlign = HorizontalAlignment.General;
			VerticalAlignment verticalAlign = VerticalAlignment.Top;
			TextDirection textDirection = TextDirection.LeftToRight;
			if (!excel.UseCachedStyle(textBoxDef.ID))
			{
				excel.DefineCachedStyle(textBoxDef.ID);
				RenderTextBoxStyle(excel, defStyle, null, fontOnly: false, ref textOrientation, ref horizontalAlign, ref verticalAlign, ref textDirection);
				borderDef.RenderBorders(excel);
				excel.EndCachedStyle();
				excel.UseCachedStyle(textBoxDef.ID);
			}
			else
			{
				textOrientation = GetTextOrientation(defStyle);
				UpdateVerticalAlign(defStyle, ref verticalAlign);
				UpdateDirection(defStyle, ref textDirection);
			}
			if (textBox.NonSharedStyle != null)
			{
				RenderTextBoxStyle(excel, textBox.NonSharedStyle, null, fontOnly: false, ref textOrientation, ref horizontalAlign, ref verticalAlign, ref textDirection);
			}
			excel.GetCellStyle().Size = 10.0;
			richTextInfo.CheckForRotatedFarEastChars = (textOrientation == TextOrientation.Rotate90);
			RPLTextBox rplTextbox = (RPLTextBox)item.RPLSource;
			actionInfo = RenderRichText(excel, rplTextbox, richTextInfo, inHeaderAndFooter: false, actionInfo, textDirection != TextDirection.RightToLeft, ref horizontalAlign);
			RenderActions(excel, actionInfo);
			FixupAlignments(excel, isNumberType: false, textOrientation, horizontalAlign, verticalAlign, textDirection);
		}

		internal static RPLActionInfo RenderRichText(IExcelGenerator excel, RPLTextBox rplTextbox, IRichTextInfo richTextInfo, bool inHeaderAndFooter, RPLActionInfo actions, bool renderListPrefixes, ref HorizontalAlignment horizontalAlign)
		{
			List<int> list = null;
			bool flag = true;
			RPLParagraph nextParagraph;
			while ((nextParagraph = rplTextbox.GetNextParagraph()) != null)
			{
				RPLParagraphProps rPLParagraphProps = nextParagraph.ElementProps as RPLParagraphProps;
				RPLParagraphPropsDef rPLParagraphPropsDef = rPLParagraphProps.Definition as RPLParagraphPropsDef;
				RPLReportSize leftIndent;
				if (flag)
				{
					flag = false;
					if (!inHeaderAndFooter)
					{
						if (UpdateHorizontalAlign(excel, rPLParagraphProps.Style[25], ref horizontalAlign))
						{
							excel.GetCellStyle().HorizontalAlignment = horizontalAlign;
						}
						if (horizontalAlign == HorizontalAlignment.Left || horizontalAlign == HorizontalAlignment.General)
						{
							leftIndent = rPLParagraphProps.LeftIndent;
							if (leftIndent == null)
							{
								leftIndent = rPLParagraphPropsDef.LeftIndent;
							}
							if (leftIndent != null)
							{
								int num = (int)(leftIndent.ToInches() * 96.0);
								excel.GetCellStyle().IndentLevel = (num + 6) / 12;
							}
						}
						else if (horizontalAlign == HorizontalAlignment.Right)
						{
							leftIndent = rPLParagraphProps.RightIndent;
							if (leftIndent == null)
							{
								leftIndent = rPLParagraphPropsDef.RightIndent;
							}
							if (leftIndent != null)
							{
								int num2 = (int)(leftIndent.ToInches() * 96.0);
								excel.GetCellStyle().IndentLevel = (num2 + 6) / 12;
							}
						}
					}
				}
				else
				{
					richTextInfo.AppendText("\n", replaceInvalidWhiteSpace: false);
				}
				leftIndent = rPLParagraphProps.SpaceBefore;
				if (leftIndent == null)
				{
					leftIndent = rPLParagraphPropsDef.SpaceBefore;
				}
				if (leftIndent != null && leftIndent.ToMillimeters() >= 0.18)
				{
					IFont font = richTextInfo.AppendTextRun(" \n", replaceInvalidWhitespace: false);
					font.Size = leftIndent.ToPoints();
				}
				if (renderListPrefixes)
				{
					int num3 = rPLParagraphProps.ListLevel ?? rPLParagraphPropsDef.ListLevel;
					if (num3 > 0)
					{
						IFont font;
						switch (rPLParagraphProps.ListStyle ?? rPLParagraphPropsDef.ListStyle)
						{
						case RPLFormat.ListStyles.Bulleted:
							font = richTextInfo.AppendTextRun(new string(' ', num3 * 4 - 2), replaceInvalidWhitespace: false);
							richTextInfo.AppendText(BulletChars[(num3 - 1) % 3]);
							if (list != null && list.Count > num3)
							{
								list.RemoveRange(num3, list.Count - num3);
							}
							break;
						case RPLFormat.ListStyles.Numbered:
						{
							if (list == null)
							{
								list = new List<int>();
							}
							int value = 1;
							int num4 = num3 - list.Count + 1;
							if (num4 <= 0)
							{
								value = ++list[num3];
							}
							else
							{
								for (int i = 0; i < num4; i++)
								{
									list.Add(1);
								}
							}
							string text = null;
							switch (num3 % 3)
							{
							case 1:
								text = GetAsDecimalString(value);
								break;
							case 2:
								text = GetAsRomanNumeralString(value);
								break;
							case 0:
								text = GetAsLatinAlphaString(value);
								break;
							}
							int num5 = num3 * 4 - 2;
							if (text.Length > num5)
							{
								font = richTextInfo.AppendTextRun(text.Substring(text.Length - num5), replaceInvalidWhitespace: false);
							}
							else
							{
								font = richTextInfo.AppendTextRun(new string(' ', num5 - text.Length), replaceInvalidWhitespace: false);
								richTextInfo.AppendText(text, replaceInvalidWhiteSpace: false);
							}
							if (list != null && list.Count > num3 + 1)
							{
								list.RemoveRange(num3 + 1, list.Count - num3 - 1);
							}
							richTextInfo.AppendText('.');
							break;
						}
						default:
							font = richTextInfo.AppendTextRun(new string(' ', num3 * 4 - 1), replaceInvalidWhitespace: false);
							if (list != null && list.Count > num3)
							{
								list.RemoveRange(num3, list.Count - num3);
							}
							break;
						}
						font.Size = 10.0;
						font.Name = "Courier New";
						font.Underline = Underline.None;
						font.Strikethrough = false;
						font.Bold = 400;
						font.Italic = false;
						richTextInfo.AppendText(' ');
					}
				}
				RPLTextRun nextTextRun;
				while ((nextTextRun = nextParagraph.GetNextTextRun()) != null)
				{
					RPLTextRunProps rPLTextRunProps = nextTextRun.ElementProps as RPLTextRunProps;
					RPLTextRunPropsDef rPLTextRunPropsDef = rPLTextRunProps.Definition as RPLTextRunPropsDef;
					RPLStyleProps sharedStyle = rPLTextRunPropsDef.SharedStyle;
					RPLStyleProps nonSharedStyle = rPLTextRunProps.NonSharedStyle;
					string text2 = null;
					bool replaceInvalidWhitespace = true;
					if (!inHeaderAndFooter || string.IsNullOrEmpty(rPLTextRunPropsDef.Formula) || rPLTextRunPropsDef.Markup == RPLFormat.MarkupStyles.HTML || rPLTextRunProps.Markup == RPLFormat.MarkupStyles.HTML)
					{
						text2 = ((rPLTextRunProps.Value == null) ? rPLTextRunPropsDef.Value : rPLTextRunProps.Value);
					}
					else
					{
						string text3 = rPLTextRunPropsDef.Formula;
						replaceInvalidWhitespace = false;
						if (text3.StartsWith("=", StringComparison.Ordinal))
						{
							text3 = text3.Remove(0, 1);
						}
						text2 = FormulaHandler.ProcessHeaderFooterFormula(text3);
					}
					if (!string.IsNullOrEmpty(text2))
					{
						IFont font = richTextInfo.AppendTextRun(text2, replaceInvalidWhitespace);
						RenderTextBoxStyle(excel, sharedStyle, font, fontOnly: true);
						if (nonSharedStyle != null)
						{
							RenderTextBoxStyle(excel, nonSharedStyle, font, fontOnly: true);
						}
						if (!inHeaderAndFooter && actions == null)
						{
							actions = rPLTextRunProps.ActionInfo;
						}
					}
				}
				leftIndent = rPLParagraphProps.SpaceAfter;
				if (leftIndent == null)
				{
					leftIndent = rPLParagraphPropsDef.SpaceAfter;
				}
				if (leftIndent != null && leftIndent.ToMillimeters() >= 0.18)
				{
					IFont font = richTextInfo.AppendTextRun("\n ", replaceInvalidWhitespace: false);
					font.Size = leftIndent.ToPoints();
				}
			}
			return actions;
		}

		private void CalculateRowFlagsFromTextBoxes(ItemInfo itemInfo, int row, ref bool rowCanGrow, ref bool rowCanShrink, ref bool autoSize)
		{
			TextBoxItemInfo textBoxItemInfo = itemInfo as TextBoxItemInfo;
			if (textBoxItemInfo.CanGrow)
			{
				if (!autoSize && textBoxItemInfo.TopRow <= row && textBoxItemInfo.BottomRow >= row && textBoxItemInfo.TopRow != textBoxItemInfo.BottomRow)
				{
					autoSize = true;
				}
				rowCanGrow = true;
			}
			if (textBoxItemInfo.CanShrink)
			{
				rowCanShrink = true;
			}
		}

		private void CalculateAutoSizeFlag(bool rowCanGrow, bool rowCanShrink, int rowHeightIn20thPoints, ref bool autoSize)
		{
			if (!autoSize && !rowCanGrow && rowCanShrink)
			{
				autoSize = true;
			}
			if (autoSize && rowHeightIn20thPoints >= 8180)
			{
				autoSize = false;
			}
		}

		private static string GetAsDecimalString(int value)
		{
			StringBuilder stringBuilder = new StringBuilder();
			while (value > 0)
			{
				stringBuilder.Append((char)(48 + value % 10));
				value /= 10;
			}
			int num = stringBuilder.Length / 2;
			for (int i = 0; i < num; i++)
			{
				char value2 = stringBuilder[i];
				stringBuilder[i] = stringBuilder[stringBuilder.Length - i - 1];
				stringBuilder[stringBuilder.Length - i - 1] = value2;
			}
			return stringBuilder.ToString();
		}

		private static string GetAsLatinAlphaString(int value)
		{
			value--;
			return new string((char)(97 + value % 26), value / 26 + 1);
		}

		private static string GetAsRomanNumeralString(int value)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (value >= 1000)
			{
				stringBuilder.Append('m', value / 1000);
				value %= 1000;
			}
			for (int num = 2; num >= 0; num--)
			{
				int num2 = (int)Math.Pow(10.0, num);
				int num3 = 9 * num2;
				if (value >= num3)
				{
					stringBuilder.Append(RomanNumerals[num, 0]);
					stringBuilder.Append(RomanNumerals[num, 2]);
					value -= num3;
				}
				num3 = 5 * num2;
				if (value >= num3)
				{
					stringBuilder.Append(RomanNumerals[num, 1]);
					value -= num3;
				}
				num3 = 4 * num2;
				if (value >= num3)
				{
					stringBuilder.Append(RomanNumerals[num, 0]);
					stringBuilder.Append(RomanNumerals[num, 1]);
					value -= num3;
				}
				if (value >= num2)
				{
					stringBuilder.Append(RomanNumerals[num, 0], value / num2);
					value %= num2;
				}
			}
			return stringBuilder.ToString();
		}

		private void RenderTextBoxValue(IExcelGenerator excel, string value, object originalValue, TypeCode type)
		{
			if (originalValue != null)
			{
				excel.SetCellValue(originalValue, type);
			}
			else if (value != null)
			{
				excel.SetCellValue(value, TypeCode.String);
			}
		}

		private void RenderActions(IExcelGenerator excel, RPLActionInfo actions)
		{
			if (actions == null)
			{
				return;
			}
			RPLAction[] actions2 = actions.Actions;
			foreach (RPLAction rPLAction in actions2)
			{
				if (rPLAction.BookmarkLink != null)
				{
					excel.AddBookmarkLink(rPLAction.Label, rPLAction.BookmarkLink);
				}
				else if (rPLAction.DrillthroughUrl != null)
				{
					excel.AddHyperlink(rPLAction.Label, rPLAction.DrillthroughUrl);
				}
				else if (rPLAction.Hyperlink != null)
				{
					excel.AddHyperlink(rPLAction.Label, rPLAction.Hyperlink);
				}
			}
		}

		private static void RenderTextBoxStyle(IExcelGenerator excel, RPLStyleProps style, ref TextOrientation textOrientation, ref HorizontalAlignment horizontalAlign, ref VerticalAlignment verticalAlign, ref TextDirection textDirection)
		{
			RenderTextBoxStyle(excel, style, excel.GetCellStyle(), fontOnly: false, ref textOrientation, ref horizontalAlign, ref verticalAlign, ref textDirection);
		}

		private static void RenderTextBoxStyle(IExcelGenerator excel, RPLStyleProps style, IFont font, bool fontOnly)
		{
			TextOrientation textOrientation = TextOrientation.Horizontal;
			HorizontalAlignment horizontalAlign = HorizontalAlignment.General;
			VerticalAlignment verticalAlign = VerticalAlignment.Top;
			TextDirection textDirection = TextDirection.LeftToRight;
			RenderTextBoxStyle(excel, style, font, fontOnly, ref textOrientation, ref horizontalAlign, ref verticalAlign, ref textDirection);
		}

		private static void RenderTextBoxStyle(IExcelGenerator excel, RPLStyleProps style, IFont font, bool fontOnly, ref TextOrientation textOrientation, ref HorizontalAlignment horizontalAlign, ref VerticalAlignment verticalAlign, ref TextDirection textDirection)
		{
			object obj = null;
			if (font != null)
			{
				obj = style[22];
				if (obj != null)
				{
					int num = LayoutConvert.ToFontWeight((RPLFormat.FontWeights)obj);
					if (num == 600)
					{
						num = 700;
					}
					font.Bold = num;
				}
				if (excel != null)
				{
					obj = style[27];
					if (obj != null && !style[27].Equals("Transparent"))
					{
						font.Color = excel.AddColor((string)obj);
					}
				}
				obj = style[19];
				if (obj != null)
				{
					font.Italic = ((RPLFormat.FontStyles)obj == RPLFormat.FontStyles.Italic);
				}
				obj = style[20];
				if (obj != null)
				{
					font.Name = (string)obj;
				}
				obj = style[21];
				if (obj != null)
				{
					font.Size = LayoutConvert.ToPoints((string)obj);
				}
				obj = style[24];
				if (obj != null)
				{
					RPLFormat.TextDecorations textDecorations = (RPLFormat.TextDecorations)obj;
					font.Strikethrough = (textDecorations == RPLFormat.TextDecorations.LineThrough);
					font.Underline = ((textDecorations == RPLFormat.TextDecorations.Underline) ? Underline.Single : Underline.None);
				}
			}
			if (fontOnly)
			{
				return;
			}
			if (UpdateHorizontalAlign(excel, style, ref horizontalAlign))
			{
				excel.GetCellStyle().HorizontalAlignment = horizontalAlign;
			}
			if (UpdateVerticalAlign(style, ref verticalAlign))
			{
				excel.GetCellStyle().VerticalAlignment = verticalAlign;
			}
			obj = style[30];
			if (obj != null)
			{
				switch ((RPLFormat.WritingModes)obj)
				{
				case RPLFormat.WritingModes.Vertical:
					textOrientation = TextOrientation.Rotate90;
					excel.GetCellStyle().Orientation = Orientation.Rotate90ClockWise;
					break;
				case RPLFormat.WritingModes.Rotate270:
					textOrientation = TextOrientation.Rotate270;
					excel.GetCellStyle().Orientation = Orientation.Rotate90CounterClockWise;
					break;
				default:
					textOrientation = TextOrientation.Horizontal;
					excel.GetCellStyle().Orientation = Orientation.Horizontal;
					break;
				}
			}
			if (UpdateDirection(style, ref textDirection))
			{
				excel.GetCellStyle().TextDirection = textDirection;
			}
		}

		private static bool UpdateHorizontalAlign(IExcelGenerator excel, RPLStyleProps style, ref HorizontalAlignment horizontalAlign)
		{
			object value = style[25];
			return UpdateHorizontalAlign(excel, value, ref horizontalAlign);
		}

		private static bool UpdateHorizontalAlign(IExcelGenerator excel, object value, ref HorizontalAlignment horizontalAlign)
		{
			if (value != null)
			{
				RPLFormat.TextAlignments textAlignments = (RPLFormat.TextAlignments)value;
				if (excel.GetCellValueType() == TypeCode.Boolean && textAlignments == RPLFormat.TextAlignments.General)
				{
					textAlignments = RPLFormat.TextAlignments.Left;
				}
				horizontalAlign = LayoutConvert.ToHorizontalAlignEnum(textAlignments);
				return true;
			}
			return false;
		}

		private static bool UpdateVerticalAlign(RPLStyleProps style, ref VerticalAlignment verticalAlign)
		{
			object obj = style[26];
			if (obj != null)
			{
				verticalAlign = LayoutConvert.ToVerticalAlignEnum((RPLFormat.VerticalAlignments)obj);
				return true;
			}
			return false;
		}

		private static bool UpdateDirection(RPLStyleProps style, ref TextDirection textDirection)
		{
			object obj = style[29];
			if (obj != null)
			{
				textDirection = (((RPLFormat.Directions)obj != RPLFormat.Directions.RTL) ? TextDirection.LeftToRight : TextDirection.RightToLeft);
				return true;
			}
			return false;
		}

		private TextOrientation GetTextOrientation(RPLStyleProps style)
		{
			TextOrientation result = TextOrientation.Horizontal;
			object obj = style[30];
			if (obj != null)
			{
				switch ((RPLFormat.WritingModes)obj)
				{
				case RPLFormat.WritingModes.Vertical:
					result = TextOrientation.Rotate90;
					break;
				case RPLFormat.WritingModes.Rotate270:
					result = TextOrientation.Rotate270;
					break;
				}
			}
			return result;
		}

		private void RenderFormat(IExcelGenerator excel, string format, object originalValue, object value, TypeCode typeCode, string language, string numeralLanguage, object rplCalendar, object numeralVariant, ref HorizontalAlignment textAlign)
		{
			if (!FormatHandler.IsExcelNumberDataType(typeCode))
			{
				return;
			}
			string hexFormula = null;
			if (string.IsNullOrEmpty(language))
			{
				language = m_reportLanguage;
			}
			if (numeralLanguage == null)
			{
				numeralLanguage = language;
			}
			if (rplCalendar == null)
			{
				rplCalendar = RPLFormat.Calendars.Gregorian;
			}
			bool invalidFormatCode;
			string excelNumberFormat = FormatHandler.GetExcelNumberFormat(format, language, (RPLFormat.Calendars)rplCalendar, numeralLanguage, (numeralVariant != null) ? ((int)numeralVariant) : 0, typeCode, originalValue, out hexFormula, out invalidFormatCode);
			if (invalidFormatCode)
			{
				excel.SetCellError(ExcelErrorCode.ValueError);
			}
			else if (hexFormula == null)
			{
				excel.GetCellStyle().NumberFormat = excelNumberFormat;
			}
			else if (value != null)
			{
				excel.SetCellValue(value, TypeCode.String);
				if (textAlign == HorizontalAlignment.General)
				{
					textAlign = HorizontalAlignment.Right;
				}
				excel.GetCellStyle().HorizontalAlignment = textAlign;
			}
			else
			{
				excel.GetCellStyle().NumberFormat = excelNumberFormat;
			}
		}

		private void RenderImage(ImageInformation imageInfo, IRowItemStruct item, IExcelGenerator excel, Dictionary<string, ImageInformation> sharedImageCache, bool invalidImage, int rowTop, int columnLeft, int rowBottom, int columnRight)
		{
			if (imageInfo == null || imageInfo.ImageData == null || imageInfo.ImageData.Length == 0L)
			{
				return;
			}
			ImageInformation imageInformation = imageInfo;
			int padding = 0;
			int padding2 = 0;
			int padding3 = 0;
			int padding4 = 0;
			double num = 1.0;
			double num2 = 1.0;
			double num3 = 1.0;
			double num4 = 1.0;
			bool flag = false;
			int num5 = 0;
			int num6 = 0;
			if (imageInfo.Paddings != null)
			{
				padding = imageInfo.Paddings.PaddingTop;
				padding2 = imageInfo.Paddings.PaddingLeft;
				padding3 = imageInfo.Paddings.PaddingBottom;
				padding4 = imageInfo.Paddings.PaddingRight;
			}
			byte elementType;
			RPLElementProps itemProps = m_report.GetItemProps(item.RPLSource, out elementType);
			object obj = itemProps.Style[5];
			RPLFormat.BorderStyles defaultBorderStyle = RPLFormat.BorderStyles.None;
			if (obj != null)
			{
				defaultBorderStyle = (RPLFormat.BorderStyles)obj;
			}
			object obj2 = itemProps.Style[10];
			double defaultBorderWidthInPts = 0.0;
			if (obj2 != null)
			{
				defaultBorderWidthInPts = LayoutConvert.ToPoints((string)obj2);
			}
			int borderWidth = GetBorderWidth(itemProps.Style, 6, 11, defaultBorderStyle, defaultBorderWidthInPts, rightOrBottom: false);
			if (padding2 < borderWidth)
			{
				padding2 = borderWidth;
			}
			borderWidth = GetBorderWidth(itemProps.Style, 7, 12, defaultBorderStyle, defaultBorderWidthInPts, rightOrBottom: true);
			if (padding4 < borderWidth)
			{
				padding4 = borderWidth;
			}
			borderWidth = GetBorderWidth(itemProps.Style, 8, 13, defaultBorderStyle, defaultBorderWidthInPts, rightOrBottom: false);
			if (padding < borderWidth)
			{
				padding = borderWidth;
			}
			borderWidth = GetBorderWidth(itemProps.Style, 9, 14, defaultBorderStyle, defaultBorderWidthInPts, rightOrBottom: true);
			if (padding3 < borderWidth)
			{
				padding3 = borderWidth;
			}
			int rowStart;
			int rowStart2;
			int columnStart;
			int columnStart2;
			if (RPLFormat.Sizings.Fit == imageInformation.ImageSizings)
			{
				rowStart = rowTop;
				num5 = m_rowInfo[rowStart].Height;
				if (ProcessVerticalPaddings(ref rowStart, rowBottom, ref padding, topToBottom: true, ref num5))
				{
					return;
				}
				num = (double)padding / (double)num5;
				num = ((padding <= 0 || !(num < 0.004)) ? Math.Round(num, 3) : 0.004);
				rowStart2 = rowBottom;
				num5 = m_rowInfo[rowStart2].Height;
				if (ProcessVerticalPaddings(ref rowStart2, rowStart, ref padding3, topToBottom: false, ref num5))
				{
					return;
				}
				num2 = (double)(num5 - padding3) / (double)num5;
				num2 = ((padding3 <= 0 || !(num2 < 0.004)) ? Math.Round(num2, 3) : 0.004);
				columnStart = columnLeft;
				num6 = m_columns[columnStart].Width;
				if (ProcessHorizontalPaddings(ref columnStart, columnRight, ref padding2, leftToRight: true, ref num6))
				{
					return;
				}
				num3 = (double)padding2 / (double)num6;
				num3 = ((padding2 <= 0 || !(num3 < 0.001)) ? Math.Round(num3, 3) : 0.001);
				columnStart2 = columnRight;
				num6 = m_columns[columnStart2].Width;
				if (ProcessHorizontalPaddings(ref columnStart2, columnStart, ref padding4, leftToRight: false, ref num6))
				{
					return;
				}
				num4 = (double)(num6 - padding4) / (double)num6;
				num4 = ((padding4 <= 0 || !(num4 < 0.001)) ? Math.Round(num4, 3) : 0.001);
			}
			else
			{
				double num7 = 0.0;
				double num8 = 0.0;
				try
				{
					num7 = (double)((float)imageInformation.Height / imageInformation.VerticalResolution) * 25.4;
					num8 = (double)((float)imageInformation.Width / imageInformation.HorizontalResolution) * 25.4;
				}
				catch
				{
					bool isShared = false;
					imageInformation = GetInvalidImage(excel, sharedImageCache, ref isShared);
					invalidImage = true;
					num7 = (double)((float)imageInformation.Height / imageInformation.VerticalResolution) * 25.4;
					num8 = (double)((float)imageInformation.Width / imageInformation.HorizontalResolution) * 25.4;
				}
				double num9;
				if (invalidImage)
				{
					num9 = 1.0;
				}
				else
				{
					int num10 = Math.Max(item.Height - padding - padding3, 0);
					int num11 = Math.Max(item.Width - padding2 - padding4, 0);
					if (num10 == 0 || num11 == 0)
					{
						return;
					}
					double num12 = LayoutConvert.ConvertPointsToMM((double)num10 / 20.0);
					double num13 = LayoutConvert.ConvertPointsToMM((double)num11 / 20.0);
					double num14 = num12 / num7;
					double num15 = num13 / num8;
					num9 = ((num14 <= num15) ? num14 : num15);
				}
				num7 = Math.Round(num7 * num9, 2);
				num8 = Math.Round(num8 * num9, 2);
				int num16 = LayoutConvert.ConvertMMTo20thPoints(num7);
				int num17 = LayoutConvert.ConvertMMTo20thPoints(num8);
				rowStart = rowTop;
				num5 = m_rowInfo[rowStart].Height;
				flag = ProcessVerticalPaddings(ref rowStart, rowBottom, ref padding, topToBottom: true, ref num5);
				if (flag)
				{
					return;
				}
				num = (double)padding / (double)num5;
				num = ((padding <= 0 || !(num < 0.004)) ? Math.Round(num, 3) : 0.004);
				rowStart2 = rowStart;
				if (num16 <= num5 - padding - padding3)
				{
					num2 = (double)(num16 + padding) / (double)num5;
				}
				else
				{
					num5 = m_rowInfo[rowStart2].Height - padding;
					while (!flag && num16 >= num5)
					{
						num16 -= num5;
						if (num16 <= 0)
						{
							break;
						}
						rowStart2++;
						if (rowStart2 <= rowBottom)
						{
							num5 = m_rowInfo[rowStart2].Height;
						}
						else
						{
							flag = true;
						}
					}
					if (flag)
					{
						return;
					}
					int rowStart3;
					if (num16 != 0)
					{
						num2 = (double)num16 / (double)num5;
						num2 = ((padding3 <= 0 || !(num2 < 0.004)) ? Math.Round(num2, 3) : 0.004);
						rowStart3 = rowStart2;
					}
					else
					{
						num2 = 1.0;
						rowStart3 = rowStart2 + 1;
					}
					if (rowStart3 <= rowBottom)
					{
						num5 = m_rowInfo[rowStart3].Height - num16;
						if (ProcessVerticalPaddings(ref rowStart3, rowBottom, ref padding3, topToBottom: true, ref num5) && padding3 != 0)
						{
							return;
						}
					}
					else if (padding3 != 0)
					{
						return;
					}
				}
				columnStart = columnLeft;
				num6 = m_columns[columnStart].Width;
				flag = ProcessHorizontalPaddings(ref columnStart, columnRight, ref padding2, leftToRight: true, ref num6);
				if (flag)
				{
					return;
				}
				num3 = (double)padding2 / (double)num6;
				num3 = ((padding2 <= 0 || !(num3 < 0.001)) ? Math.Round(num3, 3) : 0.001);
				columnStart2 = columnStart;
				if (num17 <= num6 - padding2 - padding4)
				{
					num4 = (double)(num17 + padding2) / (double)num6;
				}
				else
				{
					num6 = m_columns[columnStart2].Width - padding2;
					while (!flag && num17 >= num6)
					{
						num17 -= num6;
						if (num17 <= 0)
						{
							break;
						}
						columnStart2++;
						if (columnStart2 <= columnRight)
						{
							num6 = m_columns[columnStart2].Width;
						}
						else
						{
							flag = true;
						}
					}
					if (flag)
					{
						return;
					}
					int columnStart3;
					if (num17 != 0)
					{
						num4 = (double)num17 / (double)num6;
						num4 = ((padding4 <= 0 || !(num4 < 0.001)) ? Math.Round(num4, 3) : 0.001);
						columnStart3 = columnStart2;
					}
					else
					{
						num4 = 1.0;
						columnStart3 = columnStart2 + 1;
					}
					if (columnStart3 <= columnRight)
					{
						num6 = m_columns[columnStart3].Width - num17;
						if (ProcessHorizontalPaddings(ref columnStart3, columnRight, ref padding4, leftToRight: true, ref num6) && padding4 != 0)
						{
							return;
						}
					}
					else if (padding4 != 0)
					{
						return;
					}
				}
			}
			excel.AddImage(imageInformation.ImageName, imageInformation.ImageData, imageInformation.ImageFormat, rowStart, num, columnStart, num3, rowStart2, num2, columnStart2, num4, imageInformation.HyperlinkURL, imageInformation.HyperlinkIsBookmark);
		}

		private int GetBorderWidth(RPLElementStyle style, byte borderStyleItem, byte boderWidthItem, RPLFormat.BorderStyles defaultBorderStyle, double defaultBorderWidthInPts, bool rightOrBottom)
		{
			object obj = style[borderStyleItem];
			object obj2 = style[boderWidthItem];
			double borderWidthInPts = defaultBorderWidthInPts;
			RPLFormat.BorderStyles borderStyles = defaultBorderStyle;
			if (obj != null)
			{
				borderStyles = (RPLFormat.BorderStyles)obj;
			}
			if (borderStyles != 0)
			{
				if (obj2 != null)
				{
					borderWidthInPts = LayoutConvert.ToPoints((string)obj2);
				}
				return LayoutConvert.GetBorderWidth(LayoutConvert.ToBorderLineStyle(borderStyles), borderWidthInPts, rightOrBottom);
			}
			return 0;
		}

		private bool HandleBlocking(IBlockerInfo item, TogglePosition togglePosition, out bool isColumnBlocker)
		{
			isColumnBlocker = false;
			switch (togglePosition)
			{
			case TogglePosition.Above:
			case TogglePosition.Below:
				if (m_rowBlockers.Count > 0 && (item.LeftColumn < m_rowBlockers.Peek().LeftColumn || item.LeftColumn > m_rowBlockers.Peek().RightColumn))
				{
					return false;
				}
				if (!m_summaryRowAfter.HasValue)
				{
					m_summaryRowAfter = (togglePosition == TogglePosition.Below);
				}
				return true;
			case TogglePosition.Left:
			case TogglePosition.Right:
			{
				for (int i = item.LeftColumn; i <= item.RightColumn; i++)
				{
					if (m_columns[i].Blocker != null && item.TopRow > m_columns[i].Blocker.BottomRow)
					{
						return false;
					}
				}
				if (!m_summaryColumnAfter.HasValue)
				{
					m_summaryColumnAfter = (togglePosition == TogglePosition.Right);
				}
				isColumnBlocker = true;
				return true;
			}
			default:
				return false;
			}
		}

		private TablixMemberInfo FindVerticalParentTablixMember(TablixMemberInfo member, int currentColumn)
		{
			if (m_columns[currentColumn].TablixStructs.Count > 1)
			{
				for (int num = m_columns[currentColumn].TablixStructs.IndexOf(member) - 1; num >= 0; num--)
				{
					TablixMemberInfo tablixMemberInfo = m_columns[currentColumn].TablixStructs[num];
					if (tablixMemberInfo.TogglePosition == TogglePosition.Above || tablixMemberInfo.TogglePosition == TogglePosition.Below)
					{
						return tablixMemberInfo;
					}
				}
			}
			return m_columns[currentColumn].LastColumnHeader;
		}

		private TablixMemberInfo FindHorizontalParentTablixMember(TablixMemberInfo member, int currentColumn)
		{
			if (m_columns[currentColumn].TablixStructs.Count > 1)
			{
				for (int num = m_columns[currentColumn].TablixStructs.IndexOf(member) - 1; num >= 0; num--)
				{
					TablixMemberInfo tablixMemberInfo = m_columns[currentColumn].TablixStructs[num];
					if (tablixMemberInfo.TogglePosition == TogglePosition.Left || tablixMemberInfo.TogglePosition == TogglePosition.Right)
					{
						return tablixMemberInfo;
					}
				}
			}
			switch (member.TogglePosition)
			{
			case TogglePosition.Left:
			{
				if (member.LeftColumn == 0 || member.LeftColumn != currentColumn)
				{
					return null;
				}
				int num3 = currentColumn - 1;
				if (num3 < 0 || m_columns[num3].TablixStructs == null || m_columns[num3].TablixStructs.Count <= 1)
				{
					return null;
				}
				return m_columns[num3].TablixStructs[m_columns[num3].TablixStructs.Count - 1];
			}
			case TogglePosition.Right:
			{
				if (member.LeftColumn != currentColumn || member.LeftColumn == m_columns.Length - 1)
				{
					return null;
				}
				int num2 = member.RightColumn + 1;
				if (num2 >= m_columns.Length || m_columns[num2].TablixStructs == null || m_columns[num2].TablixStructs.Count <= 1)
				{
					return null;
				}
				return m_columns[num2].TablixStructs[m_columns[num2].TablixStructs.Count - 1];
			}
			default:
				return null;
			}
		}

		private void HandleTablixOutline(TablixMemberInfo member, int currentRow, int currentColumn, ref byte rowOutlineLevel, ref bool rowCollapsed)
		{
			switch (member.TogglePosition)
			{
			case TogglePosition.None:
				break;
			case TogglePosition.Left:
			case TogglePosition.Right:
			{
				if (member.LeftColumn != currentColumn)
				{
					break;
				}
				TablixMemberInfo tablixMemberInfo2 = FindHorizontalParentTablixMember(member, currentColumn);
				if (m_summaryRowAfter ?? true)
				{
					if (tablixMemberInfo2 != null)
					{
						if (member.BottomRow != tablixMemberInfo2.BottomRow)
						{
							BumpRowOutlineLevel(member, ref rowOutlineLevel, ref rowCollapsed);
						}
					}
					else if (member.BottomRow != m_columns[currentColumn].TablixBlockers.Peek().BodyBottomRow)
					{
						BumpRowOutlineLevel(member, ref rowOutlineLevel, ref rowCollapsed);
					}
				}
				else if (tablixMemberInfo2 != null)
				{
					if (member.TopRow != tablixMemberInfo2.TopRow)
					{
						BumpRowOutlineLevel(member, ref rowOutlineLevel, ref rowCollapsed);
					}
				}
				else if (member.TopRow != m_columns[currentColumn].TablixBlockers.Peek().BodyTopRow)
				{
					BumpRowOutlineLevel(member, ref rowOutlineLevel, ref rowCollapsed);
				}
				break;
			}
			case TogglePosition.Above:
			case TogglePosition.Below:
			{
				if (member.TopRow != currentRow)
				{
					break;
				}
				TablixMemberInfo tablixMemberInfo = FindVerticalParentTablixMember(member, currentColumn);
				if (m_summaryColumnAfter ?? true)
				{
					if (tablixMemberInfo != null)
					{
						if (member.RightColumn != tablixMemberInfo.RightColumn)
						{
							BumpColOutlineLevel(member, currentColumn);
						}
					}
					else if (member.RightColumn != m_columns[currentColumn].TablixBlockers.Peek().BodyRightColumn)
					{
						BumpColOutlineLevel(member, currentColumn);
					}
				}
				else if (tablixMemberInfo != null)
				{
					if (member.LeftColumn != tablixMemberInfo.LeftColumn)
					{
						BumpColOutlineLevel(member, currentColumn);
					}
				}
				else if (member.LeftColumn != m_columns[currentColumn].TablixBlockers.Peek().BodyLeftColumn)
				{
					BumpColOutlineLevel(member, currentColumn);
				}
				break;
			}
			}
		}

		private void BumpRowOutlineLevel(TablixMemberInfo member, ref byte rowOutlineLevel, ref bool rowCollapsed)
		{
			rowOutlineLevel += GetOutlineLevelIncrementValue(member, out bool isCollapsed);
			if (isCollapsed)
			{
				rowCollapsed = true;
			}
		}

		private void BumpColOutlineLevel(TablixMemberInfo member, int currentColumn)
		{
			ColumnInfo columnInfo = m_columns[currentColumn];
			columnInfo.OutlineLevel += GetOutlineLevelIncrementValue(member, out bool isCollapsed);
			if (isCollapsed)
			{
				columnInfo.Collapsed = true;
			}
		}

		private byte GetOutlineLevelIncrementValue(TablixMemberInfo member, out bool isCollapsed)
		{
			isCollapsed = member.IsHidden;
			int recursiveToggleLevel = member.RecursiveToggleLevel;
			if (recursiveToggleLevel < 0)
			{
				return 1;
			}
			if (member.RecursiveToggleLevel > 0)
			{
				return (byte)recursiveToggleLevel;
			}
			isCollapsed = false;
			return 0;
		}

		private TogglePosition GetTogglePosition(RowItemStruct rowItem, string toggleParentName, int top)
		{
			if (toggleParentName != null && rowItem != null && rowItem.ToggleParents.TryGetValue(toggleParentName, out ToggleParent value))
			{
				if (value.Left + value.Width < rowItem.Left)
				{
					if (value.Top + value.Height < top)
					{
						return TogglePosition.Above;
					}
					if (value.Top > top + rowItem.Height)
					{
						return TogglePosition.Below;
					}
					return TogglePosition.Left;
				}
				if (value.Left < rowItem.Left + rowItem.Width)
				{
					if (value.Top < top)
					{
						return TogglePosition.Above;
					}
					return TogglePosition.Below;
				}
				if (value.Top + value.Height < top)
				{
					return TogglePosition.Above;
				}
				if (value.Top > top + rowItem.Height)
				{
					return TogglePosition.Below;
				}
				return TogglePosition.Right;
			}
			return TogglePosition.None;
		}

		private ImageInformation GetInvalidImage(IExcelGenerator excel, Dictionary<string, ImageInformation> sharedImageCache, ref bool isShared)
		{
			RSTrace.ExcelRendererTracer.Assert(excel != null, "The excel generator cannot be null");
			RSTrace.ExcelRendererTracer.Assert(sharedImageCache != null, "The shared image collection cannot be null");
			ImageInformation imageInformation = null;
			if (sharedImageCache.ContainsKey("InvalidImage"))
			{
				imageInformation = sharedImageCache["InvalidImage"];
				isShared = true;
			}
			else
			{
				imageInformation = new ImageInformation();
				imageInformation.ImageName = "InvalidImage";
				System.Drawing.Image image = Microsoft.ReportingServices.InvalidImage.Image;
				if (image != null)
				{
					try
					{
						Stream stream = excel.CreateStream("InvalidImage");
						image.Save(stream, ImageFormat.Bmp);
						imageInformation.ImageData = stream;
						imageInformation.ImageSizings = RPLFormat.Sizings.FitProportional;
						sharedImageCache.Add("InvalidImage", imageInformation);
						isShared = true;
						return imageInformation;
					}
					finally
					{
						image.Dispose();
						image = null;
					}
				}
				isShared = false;
			}
			return imageInformation;
		}

		private PaddingInformation GetImagePaddings(RPLElementStyle style)
		{
			object obj = null;
			int paddingLeft = 0;
			int paddingRight = 0;
			int paddingTop = 0;
			int paddingBottom = 0;
			if (style == null)
			{
				return null;
			}
			obj = style[17];
			if (obj != null)
			{
				RSTrace.ExcelRendererTracer.Assert(obj is string, "The padding top object should be a string");
				paddingTop = LayoutConvert.ConvertMMTo20thPoints(LayoutConvert.ToMillimeters((string)obj));
			}
			obj = style[18];
			if (obj != null)
			{
				RSTrace.ExcelRendererTracer.Assert(obj is string, "The padding bottom object should be a string");
				paddingBottom = LayoutConvert.ConvertMMTo20thPoints(LayoutConvert.ToMillimeters((string)obj));
			}
			obj = style[15];
			if (obj != null)
			{
				RSTrace.ExcelRendererTracer.Assert(obj is string, "The padding left object should be a string");
				paddingLeft = LayoutConvert.ConvertMMTo20thPoints(LayoutConvert.ToMillimeters((string)obj));
			}
			obj = style[16];
			if (obj != null)
			{
				RSTrace.ExcelRendererTracer.Assert(obj is string, "The padding right object should be a string");
				paddingRight = LayoutConvert.ConvertMMTo20thPoints(LayoutConvert.ToMillimeters((string)obj));
			}
			return new PaddingInformation(paddingLeft, paddingRight, paddingTop, paddingBottom);
		}

		private BorderInfo GetBorderDefinitionFromCache(string key, Dictionary<string, BorderInfo> sharedBorderCache, RPLStyleProps styleProps, bool omitBorderTop, bool omitBorderBottom, IExcelGenerator excel)
		{
			RSTrace.ExcelRendererTracer.Assert(key != null, "The key cannot be null");
			RSTrace.ExcelRendererTracer.Assert(sharedBorderCache != null, "The shared border collection cannot be null");
			RSTrace.ExcelRendererTracer.Assert(excel != null, "The excel generator cannot be null");
			BorderInfo borderInfo = null;
			if (sharedBorderCache.ContainsKey(key))
			{
				borderInfo = sharedBorderCache[key];
				borderInfo.OmitBorderTop = omitBorderTop;
				borderInfo.OmitBorderBottom = omitBorderBottom;
			}
			else
			{
				borderInfo = ((styleProps == null) ? new BorderInfo() : new BorderInfo(styleProps, omitBorderTop, omitBorderBottom, excel));
				sharedBorderCache.Add(key, borderInfo);
			}
			return borderInfo;
		}

		private bool ProcessVerticalPaddings(ref int rowStart, int rowEnd, ref int padding, bool topToBottom, ref int rowHeight)
		{
			bool flag = false;
			if (padding == 0)
			{
				return flag;
			}
			if (topToBottom)
			{
				if (rowStart > rowEnd)
				{
					flag = true;
				}
			}
			else if (rowStart < rowEnd)
			{
				flag = true;
			}
			while (!flag && padding - rowHeight >= 0)
			{
				padding -= rowHeight;
				bool flag2;
				if (topToBottom)
				{
					rowStart++;
					flag2 = (rowStart <= rowEnd);
				}
				else
				{
					rowStart--;
					flag2 = (rowStart >= rowEnd);
				}
				if (flag2)
				{
					rowHeight = m_rowInfo[rowStart].Height;
				}
				else
				{
					flag = true;
				}
			}
			return flag;
		}

		private bool ProcessHorizontalPaddings(ref int columnStart, int columnEnd, ref int padding, bool leftToRight, ref int columnWidth)
		{
			bool flag = false;
			if (padding == 0)
			{
				return flag;
			}
			if (leftToRight)
			{
				if (columnStart > columnEnd)
				{
					flag = true;
				}
			}
			else if (columnStart < columnEnd)
			{
				flag = true;
			}
			while (!flag && padding - columnWidth >= 0)
			{
				padding -= columnWidth;
				bool flag2;
				if (leftToRight)
				{
					columnStart++;
					flag2 = (columnStart <= columnEnd);
				}
				else
				{
					columnStart--;
					flag2 = (columnStart >= columnEnd);
				}
				if (flag2)
				{
					columnWidth = m_columns[columnStart].Width;
				}
				else
				{
					flag = true;
				}
			}
			return flag;
		}

		private static int CompareGenerationIndex(IRowItemStruct left, IRowItemStruct right)
		{
			return left.GenerationIndex.CompareTo(right.GenerationIndex);
		}

		internal void InitCache(CreateAndRegisterStream streamDelegate)
		{
			if (m_scalabilityCache == null)
			{
				m_scalabilityCache = ScalabilityUtils.CreateCacheForTransientAllocations(streamDelegate, "Excel", StorageObjectCreator.Instance, ExcelReferenceCreator.Instance, ComponentType.Rendering, 1);
			}
		}

		internal void Dispose()
		{
			if (m_scalabilityCache != null)
			{
				m_scalabilityCache.Dispose();
				m_scalabilityCache = null;
			}
		}
	}
}
