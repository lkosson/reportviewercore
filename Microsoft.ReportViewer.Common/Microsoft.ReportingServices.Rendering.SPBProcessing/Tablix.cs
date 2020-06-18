using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using Microsoft.ReportingServices.Rendering.Utilities;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.SPBProcessing
{
	internal sealed class Tablix : PageItem
	{
		internal enum TablixRegion : byte
		{
			Unknown,
			Corner,
			ColumnHeader,
			RowHeader,
			Data
		}

		internal class RowMemberInfo : IStorable, IPersistable
		{
			private byte m_rowState;

			private double m_height;

			private static Declaration m_declaration = GetDeclaration();

			internal byte RowState
			{
				get
				{
					return m_rowState;
				}
				set
				{
					m_rowState = value;
				}
			}

			internal bool Fixed
			{
				set
				{
					if (value)
					{
						m_rowState = 1;
					}
				}
			}

			internal double Height
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

			public int Size => 9;

			internal RowMemberInfo()
			{
			}

			internal RowMemberInfo(double height)
			{
				m_height = height;
			}

			public void Serialize(IntermediateFormatWriter writer)
			{
				writer.RegisterDeclaration(m_declaration);
				while (writer.NextMember())
				{
					switch (writer.CurrentMember.MemberName)
					{
					case MemberName.State:
						writer.Write(m_rowState);
						break;
					case MemberName.Height:
						writer.Write(m_height);
						break;
					default:
						RSTrace.RenderingTracer.Assert(condition: false);
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
					case MemberName.State:
						m_rowState = reader.ReadByte();
						break;
					case MemberName.Height:
						m_height = reader.ReadDouble();
						break;
					default:
						RSTrace.RenderingTracer.Assert(condition: false);
						break;
					}
				}
			}

			public void ResolveReferences(Dictionary<ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
			{
			}

			public ObjectType GetObjectType()
			{
				return ObjectType.RowMemberInfo;
			}

			internal static Declaration GetDeclaration()
			{
				if (m_declaration == null)
				{
					List<MemberInfo> list = new List<MemberInfo>();
					list.Add(new MemberInfo(MemberName.State, Token.Byte));
					list.Add(new MemberInfo(MemberName.Height, Token.Double));
					return new Declaration(ObjectType.RowMemberInfo, ObjectType.None, list);
				}
				return m_declaration;
			}
		}

		internal class SizeInfo : IStorable, IPersistable
		{
			private double m_size = double.NaN;

			private Hashtable m_spanSize;

			private bool m_fixed;

			private static Declaration m_declaration = GetDeclaration();

			internal Hashtable SpanSize
			{
				get
				{
					return m_spanSize;
				}
				set
				{
					m_spanSize = value;
				}
			}

			internal double Value
			{
				get
				{
					if (double.IsNaN(m_size))
					{
						return 0.0;
					}
					return m_size;
				}
				set
				{
					m_size = value;
				}
			}

			internal bool Fixed
			{
				get
				{
					return m_fixed;
				}
				set
				{
					m_fixed = value;
				}
			}

			internal bool ZeroSized => m_size == 0.0;

			internal bool Empty => double.IsNaN(m_size);

			public int Size => 9 + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(m_spanSize);

			internal SizeInfo()
			{
			}

			internal SizeInfo(bool fixedData)
			{
				m_fixed = fixedData;
			}

			internal SizeInfo(double size)
			{
				m_size = size;
			}

			public void Serialize(IntermediateFormatWriter writer)
			{
				writer.RegisterDeclaration(m_declaration);
				while (writer.NextMember())
				{
					switch (writer.CurrentMember.MemberName)
					{
					case MemberName.Fixed:
						writer.Write(m_fixed);
						break;
					case MemberName.Size:
						writer.Write(m_size);
						break;
					case MemberName.SpanSize:
						writer.WriteVariantVariantHashtable(m_spanSize);
						break;
					default:
						RSTrace.RenderingTracer.Assert(condition: false);
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
					case MemberName.Fixed:
						m_fixed = reader.ReadBoolean();
						break;
					case MemberName.Size:
						m_size = reader.ReadDouble();
						break;
					case MemberName.SpanSize:
						m_spanSize = reader.ReadVariantVariantHashtable();
						break;
					default:
						RSTrace.RenderingTracer.Assert(condition: false);
						break;
					}
				}
			}

			public void ResolveReferences(Dictionary<ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
			{
			}

			public ObjectType GetObjectType()
			{
				return ObjectType.SizeInfo;
			}

			internal static Declaration GetDeclaration()
			{
				if (m_declaration == null)
				{
					List<MemberInfo> list = new List<MemberInfo>();
					list.Add(new MemberInfo(MemberName.Fixed, Token.Boolean));
					list.Add(new MemberInfo(MemberName.Size, Token.Double));
					list.Add(new MemberInfo(MemberName.SpanSize, ObjectType.VariantVariantHashtable));
					return new Declaration(ObjectType.SizeInfo, ObjectType.None, list);
				}
				return m_declaration;
			}

			internal void AddSpanSize(int span, double spanSize)
			{
				if (m_spanSize == null)
				{
					m_spanSize = new Hashtable();
					m_spanSize.Add(span, spanSize);
					return;
				}
				object obj = m_spanSize[span];
				if (obj == null)
				{
					m_spanSize.Add(span, spanSize);
				}
				else
				{
					m_spanSize[span] = Math.Max(spanSize, (double)obj);
				}
			}
		}

		internal class ColumnSpanInfo
		{
			private int m_start;

			private int m_span;

			private double m_spanSize;

			internal int Start => m_start;

			internal int Span => m_span;

			internal double SpanSize => m_spanSize;

			internal ColumnSpanInfo(int start, int span, double spanSize)
			{
				m_start = start;
				m_span = span;
				m_spanSize = spanSize;
			}

			internal int CalculateEmptyColumnns(ScalableList<SizeInfo> sizeInfoList)
			{
				int num = 0;
				for (int i = Start; i < Start + Span; i++)
				{
					SizeInfo sizeInfo = sizeInfoList[i];
					if (sizeInfo == null || sizeInfo.Empty)
					{
						num++;
					}
				}
				return num;
			}
		}

		internal class InnerToggleState
		{
			private bool m_toggle;

			private List<InnerToggleState> m_children;

			internal bool Toggle => m_toggle;

			internal List<InnerToggleState> Children => m_children;

			internal bool HasChildren
			{
				get
				{
					if (m_children == null)
					{
						return false;
					}
					return m_children.Count > 0;
				}
			}

			internal InnerToggleState(bool toggle, List<InnerToggleState> children)
			{
				m_toggle = toggle;
				m_children = children;
			}
		}

		internal abstract class TablixContext
		{
			private bool m_noRows;

			protected bool m_isLTR;

			protected int m_headerRowCols;

			protected int m_headerColumnRows;

			protected int m_rowMemberIndexCell = -1;

			protected int m_colMemberIndexCell = -1;

			protected int m_colsBeforeRowHeaders;

			protected ScalableList<RowMemberInfo> m_rowHeights;

			protected ScalableList<SizeInfo> m_colWidths;

			protected List<SizeInfo> m_colHeaderHeights;

			protected int[] m_detailRowCellsCapacity;

			protected ScalableList<PageMemberCell> m_columnHeaders;

			private bool m_columnHeadersCreated;

			private bool m_omittedOuterColumnHeaders;

			protected ScalableList<PageMemberCell> m_rowHeaders;

			private bool m_partialRow;

			protected double m_maxDetailRowHeight;

			protected double m_maxDetailRowHeightRender;

			protected double m_lastDetailCellWidth;

			protected double m_lastDetailDefCellWidth;

			protected int m_lastDetailCellColIndex;

			protected RPLWriter m_rplWriter;

			protected PageItemHelper m_partialItemHelper;

			private Interactivity m_interactivity;

			private PageContext m_pageContext;

			private double m_cellsTopInPage;

			private int m_ignoreHeight;

			private int m_ignoreGroupPageBreaks;

			private int m_isTotal;

			private int m_ignoreRow;

			private int m_ignoreCol;

			private bool m_keepWith;

			private bool m_repeatWith;

			protected byte m_sharedLayoutRow;

			protected bool m_staticDetailRow = true;

			private bool m_pageBreakNeedsOverride;

			internal bool m_detailsOnPage;

			internal bool m_pageBreakAtEnd;

			protected bool m_propagatedPageBreak;

			private bool m_firstRecursiveToggleRow;

			private TextBox m_textBoxDelayCalc;

			private string m_currentToggleMemberPath;

			internal RPLWriter Writer => m_rplWriter;

			internal IScalabilityCache Cache => m_pageContext.ScalabilityCache;

			internal PageItemHelper PartialItemHelper
			{
				get
				{
					return m_partialItemHelper;
				}
				set
				{
					m_partialItemHelper = value;
				}
			}

			internal PageContext PageContext => m_pageContext;

			internal Interactivity Interactivity => m_interactivity;

			internal bool NoRows => m_noRows;

			internal bool PageBreakAtEnd
			{
				get
				{
					return m_pageBreakAtEnd;
				}
				set
				{
					m_pageBreakAtEnd = value;
				}
			}

			internal bool PageBreakNeedsOverride
			{
				get
				{
					return m_pageBreakNeedsOverride;
				}
				set
				{
					m_pageBreakNeedsOverride = value;
				}
			}

			internal bool PropagatedPageBreak
			{
				get
				{
					return m_propagatedPageBreak;
				}
				set
				{
					m_propagatedPageBreak = value;
				}
			}

			internal bool ColumnHeadersCreated
			{
				get
				{
					return m_columnHeadersCreated;
				}
				set
				{
					m_columnHeadersCreated = value;
				}
			}

			internal bool OmittedOuterColumnHeaders
			{
				get
				{
					return m_omittedOuterColumnHeaders;
				}
				set
				{
					m_omittedOuterColumnHeaders = value;
				}
			}

			internal ScalableList<PageMemberCell> OuterColumnHeaders
			{
				get
				{
					return m_columnHeaders;
				}
				set
				{
					m_columnHeaders = value;
				}
			}

			internal ScalableList<PageMemberCell> OuterRowHeaders
			{
				get
				{
					return m_rowHeaders;
				}
				set
				{
					m_rowHeaders = value;
				}
			}

			internal int RowMemberIndexCell
			{
				get
				{
					return m_rowMemberIndexCell;
				}
				set
				{
					m_rowMemberIndexCell = value;
				}
			}

			internal int ColMemberIndexCell
			{
				get
				{
					return m_colMemberIndexCell;
				}
				set
				{
					m_colMemberIndexCell = value;
				}
			}

			internal int HeaderColumnRows => m_headerColumnRows;

			internal int HeaderRowColumns => m_headerRowCols;

			internal int ColsBeforeRowHeaders
			{
				get
				{
					return m_colsBeforeRowHeaders;
				}
				set
				{
					m_colsBeforeRowHeaders = value;
				}
			}

			internal double TablixBottom => m_cellsTopInPage;

			internal bool CheckPageBreaks
			{
				get
				{
					if (m_pageContext.IgnorePageBreaks)
					{
						return false;
					}
					if (m_ignoreGroupPageBreaks > 0)
					{
						return false;
					}
					return true;
				}
			}

			internal bool IgnoreRow => m_ignoreRow > 0;

			internal bool IgnoreColumn => m_ignoreCol > 0;

			internal bool IsTotal => m_isTotal > 0;

			internal bool AddToggledItems => m_pageContext.AddToggledItems;

			internal bool KeepWith => m_keepWith;

			internal bool RepeatWith
			{
				set
				{
					m_repeatWith = value;
				}
			}

			internal bool IgnoreHeight
			{
				set
				{
					if (value)
					{
						m_ignoreHeight++;
					}
					else
					{
						m_ignoreHeight--;
					}
				}
			}

			internal byte SharedLayoutRow
			{
				get
				{
					return m_sharedLayoutRow;
				}
				set
				{
					m_sharedLayoutRow = value;
				}
			}

			internal bool StaticDetailRow
			{
				set
				{
					m_staticDetailRow = value;
				}
			}

			internal TablixContext(RPLWriter rplWriter, PageItemHelper partialItemHelper, bool noRows, bool isLTR, PageContext pageContext, double cellsTopInPage, int headerRowCols, int headerColumnRows, int[] defDetailRowsCapacity, Interactivity interactivity)
			{
				m_rplWriter = rplWriter;
				m_partialItemHelper = partialItemHelper;
				m_noRows = noRows;
				m_pageContext = pageContext;
				m_cellsTopInPage = cellsTopInPage;
				m_headerRowCols = headerRowCols;
				m_headerColumnRows = headerColumnRows;
				m_detailRowCellsCapacity = defDetailRowsCapacity;
				m_interactivity = interactivity;
				m_isLTR = isLTR;
				m_pageContext.InitCache();
			}

			internal bool AlwaysHiddenMember(Microsoft.ReportingServices.OnDemandReportRendering.Tablix tablix, TablixMember member, Visibility visibility, TablixRegion region, bool createDetail, ref LevelInfo levelInfo)
			{
				int ignored = 0;
				bool result = AlwaysHiddenMember(tablix, member, visibility, region, createDetail, ref ignored);
				levelInfo.IgnoredRowsCols += ignored;
				return result;
			}

			internal bool AlwaysHiddenMember(Microsoft.ReportingServices.OnDemandReportRendering.Tablix tablix, TablixMember member, Visibility visibility, TablixRegion region, bool createDetail, ref int ignored)
			{
				if (visibility == null)
				{
					return false;
				}
				if (visibility.HiddenState == SharedHiddenState.Always)
				{
					AddMemberToCurrentPage(tablix, member, region, createDetail, headerFooterEval: true, ref ignored);
					return true;
				}
				return false;
			}

			internal void AddTotalsToCurrentPage(ref List<int> totalsIndex, TablixMemberCollection members, TablixRegion region, bool createDetail)
			{
				if (totalsIndex != null && totalsIndex.Count != 0)
				{
					int num = 0;
					for (int i = 0; i < totalsIndex.Count; i++)
					{
						num = totalsIndex[i];
						AddMemberToCurrentPage(null, members[num], region, createDetail);
					}
					totalsIndex = null;
				}
			}

			internal void AddMemberToCurrentPage(Microsoft.ReportingServices.OnDemandReportRendering.Tablix tablix, TablixMember member, TablixRegion region, bool createDetail)
			{
				int ignored = 0;
				AddMemberToCurrentPage(tablix, member, region, createDetail, headerFooterEval: false, ref ignored);
			}

			private void AddMemberToCurrentPage(Microsoft.ReportingServices.OnDemandReportRendering.Tablix tablix, TablixMember member, TablixRegion region, bool createDetail, bool headerFooterEval, ref int ignored)
			{
				if (m_pageContext.CancelPage)
				{
					return;
				}
				bool flag = false;
				Interactivity interactivity = null;
				if (headerFooterEval && m_pageContext.EvaluatePageHeaderFooter && m_rplWriter != null)
				{
					flag = true;
				}
				if (m_interactivity != null && m_interactivity.RegisterHiddenItems)
				{
					interactivity = m_interactivity;
				}
				if (!flag && interactivity == null)
				{
					return;
				}
				int num = 0;
				if (region == TablixRegion.RowHeader)
				{
					num = WalkTablix.AddMembersToCurrentPage(tablix, member, member.MemberCellIndex, WalkTablix.State.RowMembers, createDetail: true, m_noRows, m_pageContext, flag, interactivity);
				}
				else
				{
					if (m_columnHeadersCreated)
					{
						WalkTablix.AddMembersToCurrentPage(tablix, member, m_rowMemberIndexCell, WalkTablix.State.DetailRows, createDetail: true, m_noRows, m_pageContext, flag, interactivity);
						return;
					}
					num = WalkTablix.AddMembersToCurrentPage(tablix, member, m_rowMemberIndexCell, WalkTablix.State.ColMembers, createDetail, m_noRows, m_pageContext, flag, interactivity);
				}
				interactivity?.RegisterGroupLabel(member.Group, m_pageContext);
				if (member.TablixHeader != null)
				{
					if (num > 0)
					{
						RegisterItem.RegisterHiddenItem(member.TablixHeader.CellContents.ReportItem, m_pageContext, flag, interactivity);
						ignored += num;
					}
					else if (interactivity != null)
					{
						RegisterItem.RegisterHiddenItem(member.TablixHeader.CellContents.ReportItem, m_pageContext, useForPageHFEval: false, interactivity);
					}
				}
			}

			internal void AddMemberHeaderToCurrentPage(TablixMember member, TablixRegion region, bool headerFooterEval)
			{
				if (member == null || m_pageContext.CancelPage || (m_columnHeadersCreated && region == TablixRegion.ColumnHeader))
				{
					return;
				}
				if (m_interactivity != null)
				{
					m_interactivity.RegisterGroupLabel(member.Group, m_pageContext);
				}
				if (member.TablixHeader != null)
				{
					bool flag = false;
					if (headerFooterEval && m_pageContext.EvaluatePageHeaderFooter && m_rplWriter != null)
					{
						flag = true;
					}
					if ((flag || (m_interactivity != null && m_interactivity.RegisterHiddenItems)) && (region == TablixRegion.RowHeader || !m_columnHeadersCreated))
					{
						RegisterItem.RegisterHiddenItem(member.TablixHeader.CellContents.ReportItem, m_pageContext, flag, m_interactivity);
					}
				}
			}

			internal void AddDetailRowToCurrentPage(Microsoft.ReportingServices.OnDemandReportRendering.Tablix tablix)
			{
				if (!m_pageContext.CancelPage)
				{
					bool flag = false;
					if (m_pageContext.EvaluatePageHeaderFooter && m_rplWriter != null)
					{
						flag = true;
					}
					if (flag || (m_interactivity != null && m_interactivity.RegisterHiddenItems))
					{
						WalkTablix.AddMembersToCurrentPage(tablix, null, m_rowMemberIndexCell, WalkTablix.State.DetailRows, createDetail: true, m_noRows, m_pageContext, flag, m_interactivity);
					}
				}
			}

			internal void AddDetailCellToCurrentPage(Microsoft.ReportingServices.OnDemandReportRendering.Tablix tablix, int colMemberIndex)
			{
				if (!m_pageContext.CancelPage)
				{
					bool flag = false;
					if (m_pageContext.EvaluatePageHeaderFooter && m_rplWriter != null)
					{
						flag = true;
					}
					if (flag || (m_interactivity != null && m_interactivity.RegisterHiddenItems))
					{
						WalkTablix.AddDetailCellToCurrentPage(tablix, colMemberIndex, m_rowMemberIndexCell, m_pageContext, flag, m_interactivity);
					}
				}
			}

			internal bool EnterColMemberInstance(TablixMember colMember, Visibility visibility, bool hasRecursivePeer, bool hasVisibleStaticPeer, out byte memberState)
			{
				memberState = 0;
				if (colMember.IsTotal)
				{
					if (m_pageContext.AddToggledItems && !hasRecursivePeer)
					{
						return !hasVisibleStaticPeer;
					}
					return false;
				}
				if (visibility == null)
				{
					return true;
				}
				if (visibility.HiddenState == SharedHiddenState.Never)
				{
					return true;
				}
				if (visibility.ToggleItem != null)
				{
					if (m_pageContext.AddToggledItems)
					{
						memberState = 2;
					}
					if (colMember.Instance.Visibility.CurrentlyHidden)
					{
						if (m_pageContext.AddToggledItems)
						{
							memberState |= 4;
						}
						else
						{
							m_ignoreCol++;
						}
					}
					return true;
				}
				return !colMember.Instance.Visibility.CurrentlyHidden;
			}

			internal void LeaveColMemberInstance(TablixMember colMember, Visibility visibility)
			{
				if (!colMember.IsTotal && visibility != null && visibility.HiddenState != SharedHiddenState.Never && visibility.ToggleItem != null)
				{
					bool flag = false;
					if (!m_pageContext.AddToggledItems)
					{
						flag = colMember.Instance.Visibility.CurrentlyHidden;
					}
					if (flag)
					{
						m_ignoreCol--;
					}
				}
			}

			internal bool EnterRowMemberInstance(TablixMember rowMember, Visibility visibility, bool hasRecursivePeer)
			{
				byte memberState = 0;
				bool advanceRow = true;
				return EnterRowMemberInstance(null, rowMember, null, visibility, hasRecursivePeer, null, out memberState, ref advanceRow);
			}

			internal bool EnterRowMemberInstance(Tablix tablix, TablixMember rowMember, double[] tablixCellHeights, Visibility visibility, bool hasRecursivePeer, InnerToggleState toggleState, out byte memberState, ref bool advanceRow)
			{
				memberState = 0;
				if (rowMember.IsTotal)
				{
					return m_pageContext.AddToggledItems;
				}
				if (visibility == null || visibility.HiddenState == SharedHiddenState.Never)
				{
					if (hasRecursivePeer)
					{
						EnterRecursiveRowMemberInstance(rowMember, tablixCellHeights, toggleState, ref advanceRow);
					}
					return true;
				}
				if (visibility.ToggleItem != null)
				{
					if (m_pageContext.AddToggledItems)
					{
						memberState = 2;
					}
					if (rowMember.Instance.Visibility.CurrentlyHidden)
					{
						if (m_pageContext.AddToggledItems)
						{
							memberState |= 4;
						}
						else
						{
							m_ignoreRow++;
						}
					}
					return true;
				}
				if (hasRecursivePeer)
				{
					EnterRecursiveRowMemberInstance(rowMember, tablixCellHeights, toggleState, ref advanceRow);
				}
				return !rowMember.Instance.Visibility.CurrentlyHidden;
			}

			internal bool EnterRowMember(Tablix tablix, TablixMember rowMember, Visibility visibility, bool hasRecursivePeer, bool hasVisibleStaticPeer)
			{
				byte memberState = 0;
				return EnterRowMember(tablix, rowMember, visibility, null, hasRecursivePeer, hasVisibleStaticPeer, out memberState);
			}

			internal bool EnterRowMember(Tablix tablix, TablixMember rowMember, Visibility visibility, InnerToggleState toggleState, bool hasRecursivePeer, bool hasVisibleStaticPeer, out byte memberState)
			{
				memberState = 0;
				if (rowMember.IsTotal)
				{
					m_ignoreGroupPageBreaks++;
					m_ignoreHeight++;
					m_isTotal++;
					if (m_pageContext.AddToggledItems && !hasRecursivePeer)
					{
						return !hasVisibleStaticPeer;
					}
					return false;
				}
				if (visibility == null || visibility.HiddenState == SharedHiddenState.Never)
				{
					if (toggleState != null && ((hasRecursivePeer && toggleState.Toggle) || (rowMember.IsStatic && toggleState.HasChildren)))
					{
						m_ignoreGroupPageBreaks++;
						if (hasRecursivePeer)
						{
							m_firstRecursiveToggleRow = true;
						}
						if (PageContext.TracingEnabled)
						{
							RegisterToggleMemberPath(tablix, rowMember);
						}
					}
					return true;
				}
				if (visibility.ToggleItem != null)
				{
					if (m_pageContext.AddToggledItems)
					{
						memberState = 2;
					}
					VisibilityInstance visibility2 = rowMember.Instance.Visibility;
					m_ignoreGroupPageBreaks++;
					if (PageContext.TracingEnabled)
					{
						RegisterToggleMemberPath(tablix, rowMember);
					}
					bool flag = false;
					flag = ((!visibility.Hidden.IsExpression) ? visibility.Hidden.Value : visibility2.StartHidden);
					if (flag && !m_firstRecursiveToggleRow)
					{
						m_ignoreHeight++;
					}
					if (visibility2.CurrentlyHidden)
					{
						if (m_pageContext.AddToggledItems)
						{
							memberState |= 4;
						}
						else
						{
							m_ignoreRow++;
						}
					}
					return true;
				}
				if (toggleState != null && ((hasRecursivePeer && toggleState.Toggle) || (rowMember.IsStatic && toggleState.HasChildren)))
				{
					m_ignoreGroupPageBreaks++;
					if (hasRecursivePeer)
					{
						m_firstRecursiveToggleRow = true;
					}
				}
				return !rowMember.Instance.Visibility.CurrentlyHidden;
			}

			internal void EnterTotalRowMember()
			{
				m_ignoreGroupPageBreaks++;
				m_ignoreHeight++;
			}

			internal void LeaveTotalRowMember(double[] tablixCellHeights, ref bool advanceRow)
			{
				m_ignoreGroupPageBreaks--;
				m_ignoreHeight--;
				LeaveToggleMember(tablixCellHeights, ref advanceRow);
			}

			internal void LeaveRowMemberInstance(TablixMember rowMember, InnerToggleState toggleState, Visibility visibility, bool hasRecursivePeer)
			{
				if (rowMember.IsTotal)
				{
					return;
				}
				if (visibility == null || visibility.HiddenState == SharedHiddenState.Never)
				{
					if (hasRecursivePeer)
					{
						LeaveRecursiveRowMemberInstance(toggleState);
					}
				}
				else if (visibility.ToggleItem != null)
				{
					bool flag = false;
					if (!m_pageContext.AddToggledItems)
					{
						flag = rowMember.Instance.Visibility.CurrentlyHidden;
					}
					if (flag)
					{
						m_ignoreRow--;
					}
				}
				else if (hasRecursivePeer)
				{
					LeaveRecursiveRowMemberInstance(toggleState);
				}
			}

			internal void LeaveRowMember(TablixMember rowMember, double[] tablixCellHeights, Visibility visibility, bool hasRecursivePeer, InnerToggleState toggleState, ref bool advanceRow)
			{
				if (m_pageContext.CancelPage)
				{
					advanceRow = false;
				}
				if (rowMember.IsTotal)
				{
					m_ignoreGroupPageBreaks--;
					m_ignoreHeight--;
					m_isTotal--;
					LeaveToggleMember(tablixCellHeights, ref advanceRow);
				}
				else if (visibility == null || visibility.HiddenState == SharedHiddenState.Never)
				{
					if (toggleState != null)
					{
						if (hasRecursivePeer && toggleState.Toggle)
						{
							m_firstRecursiveToggleRow = false;
						}
						else if (rowMember.IsStatic && toggleState.HasChildren)
						{
							m_ignoreGroupPageBreaks--;
							LeaveToggleMember(tablixCellHeights, ref advanceRow);
						}
					}
				}
				else if (visibility.ToggleItem != null)
				{
					m_ignoreGroupPageBreaks--;
					bool flag = false;
					flag = ((!visibility.Hidden.IsExpression) ? visibility.Hidden.Value : rowMember.Instance.Visibility.StartHidden);
					if (flag && !m_firstRecursiveToggleRow)
					{
						m_ignoreHeight--;
					}
					LeaveToggleMember(tablixCellHeights, ref advanceRow);
				}
				else if (toggleState != null)
				{
					if (hasRecursivePeer && toggleState.Toggle)
					{
						m_firstRecursiveToggleRow = false;
					}
					else if (rowMember.IsStatic && toggleState.HasChildren)
					{
						m_ignoreGroupPageBreaks--;
						LeaveToggleMember(tablixCellHeights, ref advanceRow);
					}
				}
			}

			internal void LeaveRowMember(TablixMember rowMember, double[] tablixCellHeights, double sizeForRepeatWithBefore, Visibility visibility, bool hasRecursivePeer, InnerToggleState toggleState, ref bool advanceRow)
			{
				m_cellsTopInPage -= sizeForRepeatWithBefore;
				LeaveRowMember(rowMember, tablixCellHeights, visibility, hasRecursivePeer, toggleState, ref advanceRow);
			}

			private void LeaveToggleMember(double[] tablixCellHeights, ref bool advanceRow)
			{
				if (m_ignoreGroupPageBreaks == 0 && !m_pageContext.FullOnPage)
				{
					int num = m_rowMemberIndexCell + 1;
					if (num >= tablixCellHeights.Length)
					{
						num = 0;
					}
					CheckPageHeight(num, tablixCellHeights, ref advanceRow);
				}
				if (PageContext.TracingEnabled && m_ignoreGroupPageBreaks == 0)
				{
					ResetToggleMemberPath();
				}
			}

			private void CheckPageHeightOnToggleMember(double[] tablixCellHeights, ref bool advanceRow)
			{
				if (m_ignoreGroupPageBreaks == 0 && !m_pageContext.FullOnPage)
				{
					CheckPageHeight(m_rowMemberIndexCell, tablixCellHeights, ref advanceRow);
				}
			}

			private void CheckPageHeight(int cellIndex, double[] tablixCellHeights, ref bool advanceRow)
			{
				if (!m_detailsOnPage)
				{
					return;
				}
				RoundedDouble roundedDouble = new RoundedDouble(m_cellsTopInPage + tablixCellHeights[cellIndex]);
				if (roundedDouble > m_pageContext.PageHeight)
				{
					advanceRow = false;
					if (m_pageContext.TracingEnabled && new RoundedDouble(m_cellsTopInPage) > m_pageContext.PageHeight)
					{
						TracePageGrownOnImplicitKeepTogetherMember();
					}
				}
				m_pageContext.CheckPageSize(roundedDouble);
			}

			private void EnterRecursiveRowMemberInstance(TablixMember rowMember, double[] tablixCellHeights, InnerToggleState toggleState, ref bool advanceRow)
			{
				if (toggleState != null && toggleState.Toggle)
				{
					if (rowMember.Group.Instance.RecursiveLevel == 0)
					{
						m_firstRecursiveToggleRow = true;
						CheckPageHeightOnToggleMember(tablixCellHeights, ref advanceRow);
					}
					else
					{
						m_firstRecursiveToggleRow = false;
					}
					m_ignoreGroupPageBreaks++;
				}
			}

			private void LeaveRecursiveRowMemberInstance(InnerToggleState toggleState)
			{
				if (toggleState != null && toggleState.Toggle)
				{
					m_ignoreGroupPageBreaks--;
				}
			}

			internal void RegisterKeepWith()
			{
				m_keepWith = true;
			}

			internal void UnRegisterKeepWith(KeepWithGroup keepWith, double[] tablixCellHeights, ref bool advanceRow)
			{
				m_keepWith = false;
				if (keepWith == KeepWithGroup.After)
				{
					if (m_pageContext.CancelPage)
					{
						advanceRow = false;
					}
					else
					{
						advanceRow = true;
					}
				}
				else
				{
					LeaveToggleMember(tablixCellHeights, ref advanceRow);
				}
			}

			internal void CreateCorner(TablixCorner corner)
			{
				if ((m_rplWriter == null && m_interactivity == null) || HasCornerCells() || m_headerColumnRows == 0 || m_headerRowCols == 0)
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
						AddCornerCell(tablixCornerRow[j], i, j);
					}
				}
			}

			private void AddCornerCell(TablixCornerCell tablixCornerCell, int rowIndex, int colIndex)
			{
				if (tablixCornerCell == null || tablixCornerCell.CellContents == null || m_pageContext.CancelPage)
				{
					return;
				}
				CellContents cellContents = tablixCornerCell.CellContents;
				ReportItem reportItem = cellContents.ReportItem;
				PageItem pageItem = null;
				if (reportItem != null)
				{
					PageContext pageContext = new PageContext(m_pageContext, PageContext.PageContextFlags.FullOnPage, PageContext.IgnorePBReasonFlag.TablixParent);
					pageItem = PageItem.Create(reportItem, pageContext, tablixCellParent: true, createForRepeat: false);
					double parentHeight = 0.0;
					bool useForPageHFEval = false;
					pageItem.CalculatePage(m_rplWriter, null, pageContext, null, null, 0.0, ref parentHeight, m_interactivity);
					if (!pageContext.CancelPage)
					{
						if (m_rplWriter != null)
						{
							UpdateSizes(colIndex, cellContents.ColSpan, pageItem.ItemRenderSizes.Width, split: false, ref m_colWidths);
							UpdateSizes(rowIndex, cellContents.RowSpan, pageItem.ItemRenderSizes.Height, split: false, ref m_colHeaderHeights);
							CreateCornerCell(pageItem, cellContents.RowSpan, tablixCornerCell.CellContents.ColSpan, rowIndex, colIndex);
							useForPageHFEval = pageContext.EvaluatePageHeaderFooter;
						}
						RegisterItem.RegisterPageItem(pageItem, pageContext, useForPageHFEval, m_interactivity);
					}
				}
				else if (m_rplWriter != null)
				{
					CreateCornerCell(null, cellContents.RowSpan, cellContents.ColSpan, rowIndex, colIndex);
				}
			}

			internal abstract bool HasCornerCells();

			internal abstract void CreateCornerCell(PageItem topItem, int rowSpan, int colSpan, int rowIndex, int colIndex);

			internal PageMemberCell AddRowMember(TablixMember rowMember, int rowIndex, int colIndex, int rowSpan, int colSpan, byte memberState, int defTreeLevel, LevelInfo childLevelInfo)
			{
				if (m_rplWriter == null && m_interactivity == null)
				{
					return null;
				}
				if (m_pageContext.CancelPage)
				{
					return null;
				}
				return AddRowMemberContent(rowMember, rowIndex, colIndex, rowSpan, colSpan, memberState, defTreeLevel, childLevelInfo, 0.0);
			}

			private PageMemberCell AddRowMemberContent(TablixMember rowMember, int rowIndex, int colIndex, int rowSpan, int colSpan, byte memberState, int defTreeLevel, LevelInfo childLevelInfo, double updateWidth)
			{
				if (m_interactivity != null)
				{
					m_interactivity.RegisterGroupLabel(rowMember.Group, m_pageContext);
				}
				if (rowMember.TablixHeader == null)
				{
					if (m_rplWriter != null)
					{
						UpdateRowHeight(rowIndex, rowSpan, 0.0, rowMember.FixedData, resetState: false);
						return CreateMemberCell(null, rowSpan, 0, rowMember, memberState, defTreeLevel, TablixRegion.RowHeader);
					}
					return null;
				}
				m_sharedLayoutRow = 0;
				ReportItem reportItem = rowMember.TablixHeader.CellContents.ReportItem;
				if (reportItem != null)
				{
					double parentHeight = 0.0;
					bool useForPageHFEval = false;
					PageContext pageContext = new PageContext(m_pageContext, PageContext.PageContextFlags.FullOnPage, PageContext.IgnorePBReasonFlag.TablixParent);
					PageItem pageItem = PageItem.Create(reportItem, pageContext, tablixCellParent: true, createForRepeat: false);
					double num = rowMember.TablixHeader.Size.ToMillimeters() - updateWidth;
					if (childLevelInfo != null)
					{
						num += childLevelInfo.SizeForParent;
						if (childLevelInfo.SourceSize > 0.0)
						{
							pageItem.ItemPageSizes.Height = childLevelInfo.SourceSize;
						}
					}
					if (pageItem is TextBox)
					{
						pageItem.ItemPageSizes.Width = num;
					}
					pageItem.CalculatePage(m_rplWriter, null, pageContext, null, null, 0.0, ref parentHeight, m_interactivity);
					if (pageContext.CancelPage)
					{
						return null;
					}
					if (m_rplWriter != null)
					{
						UpdateRowHeight(rowIndex, rowSpan, pageItem.ItemRenderSizes.Height, rowMember.FixedData, resetState: true);
						num = Math.Max(num, pageItem.ItemRenderSizes.Width);
						if (childLevelInfo == null)
						{
							UpdateSizes(colIndex, colSpan, num, split: false, ref m_colWidths);
						}
						else
						{
							UpdateSizes(colIndex, colSpan + childLevelInfo.SpanForParent, num, split: false, ref m_colWidths);
						}
						useForPageHFEval = pageContext.EvaluatePageHeaderFooter;
					}
					RegisterItem.RegisterPageItem(pageItem, pageContext, useForPageHFEval, m_interactivity);
					if (m_rplWriter != null)
					{
						if (childLevelInfo == null)
						{
							return CreateMemberCell(pageItem, rowSpan, colSpan, rowMember, memberState, defTreeLevel, TablixRegion.RowHeader);
						}
						return CreateMemberCell(pageItem, rowSpan, colSpan + childLevelInfo.SpanForParent, rowMember, memberState, defTreeLevel, TablixRegion.RowHeader);
					}
				}
				else if (m_rplWriter != null)
				{
					UpdateRowHeight(rowIndex, rowSpan, 0.0, rowMember.FixedData, resetState: true);
					double num2 = rowMember.TablixHeader.Size.ToMillimeters() - updateWidth;
					if (childLevelInfo == null)
					{
						UpdateSizes(colIndex, colSpan, num2, split: false, ref m_colWidths);
						return CreateMemberCell(null, rowSpan, colSpan, rowMember, memberState, defTreeLevel, TablixRegion.RowHeader);
					}
					num2 += childLevelInfo.SizeForParent;
					UpdateSizes(colIndex, colSpan + childLevelInfo.SpanForParent, num2, split: false, ref m_colWidths);
					return CreateMemberCell(null, rowSpan, colSpan + childLevelInfo.SpanForParent, rowMember, memberState, defTreeLevel, TablixRegion.RowHeader);
				}
				return null;
			}

			internal PageMemberCell AddTotalRowMember(TablixMember rowMember, int rowIndex, int colIndex, int rowSpan, int colSpan, byte memberState, int defTreeLevel, LevelInfo parentLevelInfo, LevelInfo childLevelInfo)
			{
				RSTrace.RenderingTracer.Assert(parentLevelInfo != null, "The parent LevelInfo is null.");
				if (m_rplWriter == null && m_interactivity == null)
				{
					return null;
				}
				if (m_pageContext.CancelPage)
				{
					return null;
				}
				double updateWidth = 0.0;
				if (parentLevelInfo.SpanForParent > 0)
				{
					colIndex += parentLevelInfo.SpanForParent;
					colSpan -= parentLevelInfo.SpanForParent;
					updateWidth = parentLevelInfo.SizeForParent;
					if (colSpan == 0)
					{
						if (m_rplWriter != null)
						{
							return CreateMemberCell(null, rowSpan, 0, rowMember, memberState, defTreeLevel, TablixRegion.RowHeader);
						}
						return null;
					}
				}
				return AddRowMemberContent(rowMember, rowIndex, colIndex, rowSpan, colSpan, memberState, defTreeLevel, childLevelInfo, updateWidth);
			}

			internal abstract PageMemberCell CreateMemberCell(PageItem topItem, int rowSpan, int colSpan, TablixMember tablixMember, byte state, int defTreeLevel, TablixRegion region);

			internal PageMemberCell AddColMember(TablixMember colMember, int rowIndex, int colIndex, int rowSpan, int colSpan, byte state, int defTreeLevel, LevelInfo childLevelInfo)
			{
				RSTrace.RenderingTracer.Assert(childLevelInfo != null, "The child LevelInfo is null.");
				if ((m_rplWriter == null && m_interactivity == null) || m_columnHeadersCreated)
				{
					return null;
				}
				if (m_pageContext.CancelPage)
				{
					return null;
				}
				if (m_interactivity != null)
				{
					m_interactivity.RegisterGroupLabel(colMember.Group, m_pageContext);
				}
				return AddColumnMemberContent(colMember, rowIndex, colIndex, rowSpan, colSpan, state, defTreeLevel, childLevelInfo, 0.0);
			}

			private PageMemberCell AddColumnMemberContent(TablixMember colMember, int rowIndex, int colIndex, int rowSpan, int colSpan, byte state, int defTreeLevel, LevelInfo childLevelInfo, double updateHeight)
			{
				if (colMember.TablixHeader == null)
				{
					if (m_rplWriter != null)
					{
						UpdateColumnMemberFixedData(colMember, colIndex, colSpan, ref m_colWidths);
						return CreateMemberCell(null, 0, colSpan, colMember, state, defTreeLevel, TablixRegion.ColumnHeader);
					}
					return null;
				}
				if (m_headerColumnRows > 0)
				{
					ReportItem reportItem = colMember.TablixHeader.CellContents.ReportItem;
					if (reportItem != null)
					{
						double parentHeight = 0.0;
						bool useForPageHFEval = false;
						PageContext pageContext = new PageContext(m_pageContext, PageContext.PageContextFlags.FullOnPage, PageContext.IgnorePBReasonFlag.TablixParent);
						PageItem pageItem = PageItem.Create(reportItem, pageContext, tablixCellParent: true, createForRepeat: false);
						if (childLevelInfo.SourceSize > 0.0)
						{
							pageItem.ItemPageSizes.Width = childLevelInfo.SourceSize;
						}
						pageItem.CalculatePage(m_rplWriter, null, pageContext, null, null, 0.0, ref parentHeight, m_interactivity);
						if (pageContext.CancelPage)
						{
							return null;
						}
						if (m_rplWriter != null)
						{
							UpdateSizes(colIndex, colSpan, pageItem.ItemRenderSizes.Width, split: true, ref m_colWidths);
							double val = colMember.TablixHeader.Size.ToMillimeters() + childLevelInfo.SizeForParent - updateHeight;
							val = Math.Max(val, pageItem.ItemRenderSizes.Height);
							UpdateSizes(rowIndex, rowSpan + childLevelInfo.SpanForParent, val, split: false, ref m_colHeaderHeights);
							UpdateColumnMemberFixedData(colMember, colIndex, colSpan, ref m_colWidths);
							useForPageHFEval = pageContext.EvaluatePageHeaderFooter;
						}
						RegisterItem.RegisterPageItem(pageItem, pageContext, useForPageHFEval, m_interactivity);
						if (m_rplWriter != null)
						{
							return CreateMemberCell(pageItem, rowSpan + childLevelInfo.SpanForParent, colSpan, colMember, state, defTreeLevel, TablixRegion.ColumnHeader);
						}
					}
					else if (m_rplWriter != null)
					{
						UpdateSizes(colIndex, colSpan, 0.0, split: true, ref m_colWidths);
						double size = colMember.TablixHeader.Size.ToMillimeters() + childLevelInfo.SizeForParent - updateHeight;
						UpdateSizes(rowIndex, rowSpan + childLevelInfo.SpanForParent, size, split: false, ref m_colHeaderHeights);
						UpdateColumnMemberFixedData(colMember, colIndex, colSpan, ref m_colWidths);
						return CreateMemberCell(null, rowSpan + childLevelInfo.SpanForParent, colSpan, colMember, state, defTreeLevel, TablixRegion.ColumnHeader);
					}
				}
				else if (m_rplWriter != null)
				{
					UpdateSizes(colIndex, colSpan, 0.0, split: true, ref m_colWidths);
					UpdateColumnMemberFixedData(colMember, colIndex, colSpan, ref m_colWidths);
					return CreateMemberCell(null, 0, colSpan, colMember, state, defTreeLevel, TablixRegion.ColumnHeader);
				}
				return null;
			}

			internal PageMemberCell AddTotalColMember(TablixMember colMember, int rowIndex, int colIndex, int rowSpan, int colSpan, byte state, int defTreeLevel, LevelInfo parentLevelInfo, LevelInfo childLevelInfo)
			{
				RSTrace.RenderingTracer.Assert(parentLevelInfo != null, "The parent LevelInfo is null.");
				RSTrace.RenderingTracer.Assert(childLevelInfo != null, "The child LevelInfo is null.");
				if ((m_rplWriter == null && m_interactivity == null) || m_columnHeadersCreated)
				{
					return null;
				}
				if (m_pageContext.CancelPage)
				{
					return null;
				}
				double updateHeight = 0.0;
				if (parentLevelInfo.SpanForParent > 0)
				{
					rowIndex += parentLevelInfo.SpanForParent;
					rowSpan -= parentLevelInfo.SpanForParent;
					updateHeight = parentLevelInfo.SizeForParent;
					if (rowSpan == 0)
					{
						if (m_rplWriter != null)
						{
							return CreateMemberCell(null, 0, colSpan, colMember, state, defTreeLevel, TablixRegion.ColumnHeader);
						}
						return null;
					}
				}
				return AddColumnMemberContent(colMember, rowIndex, colIndex, rowSpan, colSpan, state, defTreeLevel, childLevelInfo, updateHeight);
			}

			internal void AddDetailEmptyCell(int colIndex, double cellColDefWidth, double cellCellDefHeight)
			{
				if (cellCellDefHeight > m_maxDetailRowHeight)
				{
					m_maxDetailRowHeight = cellCellDefHeight;
				}
				if (m_rplWriter != null)
				{
					UpdateLastDetailCellWidth();
					CreateDetailCell(null);
					m_lastDetailCellWidth = cellColDefWidth;
					m_lastDetailCellColIndex = colIndex;
					if (cellCellDefHeight > m_maxDetailRowHeightRender)
					{
						m_maxDetailRowHeightRender = cellCellDefHeight;
					}
				}
				m_lastDetailDefCellWidth = cellColDefWidth;
			}

			internal PageItem AddDetailCellFromState(TablixCell cellDef, bool ignoreCellPageBreaks)
			{
				ReportItem source = null;
				if (cellDef.CellContents != null)
				{
					source = cellDef.CellContents.ReportItem;
				}
				PageContext pageContext = m_pageContext;
				pageContext = (ignoreCellPageBreaks ? new PageContext(m_pageContext, PageContext.PageContextFlags.FullOnPage, PageContext.IgnorePBReasonFlag.TablixParent) : ((m_ignoreGroupPageBreaks <= 0) ? new PageContext(m_pageContext, PageContext.PageContextFlags.StretchPage, PageContext.IgnorePBReasonFlag.TablixParent) : new PageContext(m_pageContext, PageContext.PageContextFlags.IgnorePageBreak | PageContext.PageContextFlags.StretchPage, PageContext.IgnorePBReasonFlag.Toggled)));
				PageItem pageItem = PageItem.Create(source, pageContext, tablixCellParent: true, createForRepeat: false);
				pageItem.UpdateItem(m_partialItemHelper);
				return pageItem;
			}

			internal ItemOffset AddDetailCell(TablixCell cellDef, int colIndex, double cellColDefWidth, double cellCellDefHeight, bool ignoreCellPageBreaks, bool collect, out bool partialItem)
			{
				ReportItem reportItem = null;
				if (cellDef.CellContents != null)
				{
					reportItem = cellDef.CellContents.ReportItem;
				}
				if (reportItem != null)
				{
					PageContext pageContext = m_pageContext;
					pageContext = (ignoreCellPageBreaks ? new PageContext(m_pageContext, PageContext.PageContextFlags.FullOnPage, PageContext.IgnorePBReasonFlag.TablixParent) : ((m_ignoreGroupPageBreaks <= 0) ? new PageContext(m_pageContext, PageContext.PageContextFlags.StretchPage, PageContext.IgnorePBReasonFlag.TablixParent) : new PageContext(m_pageContext, PageContext.PageContextFlags.IgnorePageBreak | PageContext.PageContextFlags.StretchPage, PageContext.IgnorePBReasonFlag.Toggled)));
					PageItem pageItem = PageItem.Create(reportItem, pageContext, tablixCellParent: true, createForRepeat: false);
					bool flag = false;
					if (m_colMemberIndexCell >= 0)
					{
						Microsoft.ReportingServices.OnDemandReportRendering.TextBox textBox = reportItem as Microsoft.ReportingServices.OnDemandReportRendering.TextBox;
						if (textBox != null)
						{
							pageItem.ItemPageSizes.Width = cellColDefWidth;
							if (pageContext.MeasureItems && (textBox.CanGrow || textBox.CanShrink))
							{
								flag = true;
							}
						}
					}
					if (flag)
					{
						m_textBoxDelayCalc = (pageItem as TextBox);
						m_textBoxDelayCalc.CalcSizeState = TextBox.CalcSize.Delay;
						double parentHeight = 0.0;
						pageItem.CalculatePage(m_rplWriter, m_partialItemHelper, pageContext, null, null, m_cellsTopInPage, ref parentHeight, m_interactivity);
						if (parentHeight > m_maxDetailRowHeight)
						{
							m_maxDetailRowHeight = parentHeight;
						}
						if (!pageItem.StaticItem)
						{
							m_staticDetailRow = false;
						}
						if (collect)
						{
							RegisterItem.RegisterPageItem(pageItem, pageContext, pageContext.EvaluatePageHeaderFooter, m_interactivity);
						}
						if (m_rplWriter != null)
						{
							UpdateLastDetailCellWidth();
							CreateDetailCell(pageItem);
							m_lastDetailCellWidth = cellColDefWidth;
							m_lastDetailCellColIndex = colIndex;
						}
						if (m_textBoxDelayCalc.CalcSizeState == TextBox.CalcSize.Done)
						{
							m_textBoxDelayCalc = null;
						}
						partialItem = false;
					}
					else
					{
						partialItem = CalculateDetailCell(pageItem, colIndex, collect, pageContext);
					}
					m_lastDetailDefCellWidth = cellColDefWidth;
					return pageItem;
				}
				partialItem = false;
				AddDetailEmptyCell(colIndex, cellColDefWidth, cellCellDefHeight);
				return new EmptyCell();
			}

			internal bool CalculateDetailCell(PageItem topItem, int colIndex, bool collect)
			{
				return CalculateDetailCell(topItem, colIndex, collect, m_pageContext);
			}

			private bool CalculateDetailCell(PageItem topItem, int colIndex, bool collect, PageContext pageContext)
			{
				double parentHeight = 0.0;
				topItem.CalculatePage(m_rplWriter, m_partialItemHelper, pageContext, null, null, m_cellsTopInPage, ref parentHeight, m_interactivity);
				if (pageContext.CancelPage)
				{
					return true;
				}
				if (topItem.ItemState == State.TopNextPage)
				{
					m_staticDetailRow = false;
					AddDetailEmptyCell(colIndex, topItem.ItemPageSizes.Width, 0.0);
				}
				else
				{
					if (parentHeight > m_maxDetailRowHeight)
					{
						m_maxDetailRowHeight = parentHeight;
					}
					if (!topItem.StaticItem)
					{
						m_staticDetailRow = false;
					}
					if (collect)
					{
						RegisterItem.RegisterPageItem(topItem, pageContext, pageContext.EvaluatePageHeaderFooter, m_interactivity);
					}
					if (m_rplWriter != null)
					{
						UpdateLastDetailCellWidth();
						CreateDetailCell(topItem);
						m_lastDetailCellWidth = topItem.ItemRenderSizes.Width;
						m_lastDetailCellColIndex = colIndex;
						if (topItem.ItemRenderSizes.Height > m_maxDetailRowHeightRender)
						{
							m_maxDetailRowHeightRender = topItem.ItemRenderSizes.Height;
						}
					}
				}
				if (topItem.ItemState != State.OnPage && topItem.ItemState != State.OnPageHidden)
				{
					if (topItem.ItemState == State.OnPagePBEnd)
					{
						m_propagatedPageBreak = true;
						return false;
					}
					m_partialRow = true;
					return true;
				}
				return false;
			}

			internal abstract void CreateDetailCell(PageItem topItem);

			internal abstract void UpdateDetailCell(double cellColDefWidth);

			internal abstract void UpdateLastDetailCellWidth();

			internal bool AdvanceRow(double[] tablixCellHeights, List<int> tablixCreateState, int level)
			{
				NextRow(tablixCellHeights);
				if (m_partialRow)
				{
					if (level >= tablixCreateState.Count)
					{
						tablixCreateState.Add(0);
					}
					else
					{
						tablixCreateState[level] = 0;
					}
					return false;
				}
				if (level < tablixCreateState.Count)
				{
					tablixCreateState.RemoveAt(level);
				}
				if (m_pageContext.CancelPage)
				{
					return false;
				}
				if (m_pageContext.FullOnPage)
				{
					return true;
				}
				if (m_keepWith || m_ignoreGroupPageBreaks > 0)
				{
					return true;
				}
				int num = m_rowMemberIndexCell + 1;
				if (num >= tablixCellHeights.Length)
				{
					num = 0;
				}
				RoundedDouble roundedDouble = new RoundedDouble(m_cellsTopInPage + tablixCellHeights[num]);
				m_pageContext.CheckPageSize(roundedDouble);
				if (roundedDouble <= m_pageContext.PageHeight)
				{
					return true;
				}
				if (!m_detailsOnPage)
				{
					return true;
				}
				return false;
			}

			internal double NextRow(double[] tablixCellHeights)
			{
				double num = 0.0;
				if (m_ignoreHeight == 0)
				{
					num = ((!m_repeatWith && m_ignoreGroupPageBreaks <= 0) ? m_maxDetailRowHeight : tablixCellHeights[m_rowMemberIndexCell]);
					m_cellsTopInPage += num;
				}
				m_maxDetailRowHeight = 0.0;
				m_maxDetailRowHeightRender = 0.0;
				m_colMemberIndexCell = -1;
				return num;
			}

			private void UpdateRowHeight(int start, int span, double height, bool fixedData, bool resetState)
			{
				double num = 0.0;
				RowMemberInfo item = null;
				for (int i = start; i < start + span; i++)
				{
					using (m_rowHeights.GetAndPin(i, out item))
					{
						num += item.Height;
						if (resetState)
						{
							item.RowState = 0;
						}
						item.Fixed = fixedData;
					}
				}
				using (m_rowHeights.GetAndPin(start + span - 1, out item))
				{
					item.Height += Math.Max(0.0, height - num);
				}
			}

			private void UpdateColumnMemberFixedData(TablixMember colMember, int start, int span, ref ScalableList<SizeInfo> sizeInfoList)
			{
				if (!colMember.FixedData)
				{
					return;
				}
				if (sizeInfoList == null)
				{
					sizeInfoList = new ScalableList<SizeInfo>(0, Cache);
				}
				for (int i = start; i < start + span; i++)
				{
					if (i >= sizeInfoList.Count)
					{
						while (sizeInfoList.Count < i)
						{
							sizeInfoList.Add(null);
						}
						sizeInfoList.Add(new SizeInfo(fixedData: true));
						continue;
					}
					SizeInfo item = null;
					using (sizeInfoList.GetAndPin(i, out item))
					{
						if (item == null)
						{
							sizeInfoList[i] = new SizeInfo(fixedData: true);
						}
						else
						{
							item.Fixed = true;
						}
					}
				}
			}

			protected void UpdateSizes(int start, int span, double size, bool split, ref List<SizeInfo> sizeInfoList)
			{
				if (sizeInfoList == null)
				{
					sizeInfoList = new List<SizeInfo>();
				}
				if (split)
				{
					int num = 0;
					double num2 = 0.0;
					for (int i = start; i < start + span; i++)
					{
						if (i >= sizeInfoList.Count || sizeInfoList[i] == null || sizeInfoList[i].Empty)
						{
							num++;
						}
						else
						{
							num2 += sizeInfoList[i].Value;
						}
					}
					if (!(num2 < size))
					{
						return;
					}
					if (num == 0)
					{
						sizeInfoList[start + span - 1].Value += size - num2;
						return;
					}
					num2 = (size - num2) / (double)num;
					for (int j = start; j < start + span; j++)
					{
						if (j >= sizeInfoList.Count)
						{
							while (sizeInfoList.Count < j)
							{
								sizeInfoList.Add(null);
							}
							sizeInfoList.Add(new SizeInfo(num2));
						}
						else if (sizeInfoList[j] == null)
						{
							sizeInfoList[j] = new SizeInfo(num2);
						}
						else if (sizeInfoList[j].Empty)
						{
							sizeInfoList[j].Value = num2;
						}
					}
					return;
				}
				while (sizeInfoList.Count <= start + span - 1)
				{
					sizeInfoList.Add(null);
				}
				if (span == 1)
				{
					if (sizeInfoList[start] == null)
					{
						sizeInfoList[start] = new SizeInfo(size);
					}
					else
					{
						sizeInfoList[start].Value = Math.Max(sizeInfoList[start].Value, size);
					}
					return;
				}
				if (sizeInfoList[start] == null)
				{
					sizeInfoList[start] = new SizeInfo(fixedData: false);
				}
				sizeInfoList[start].AddSpanSize(span, size);
			}

			protected void UpdateSizes(int start, int span, double size, bool split, ref ScalableList<SizeInfo> sizeInfoList)
			{
				if (sizeInfoList == null)
				{
					sizeInfoList = new ScalableList<SizeInfo>(0, Cache);
				}
				SizeInfo item = null;
				if (split)
				{
					int num = 0;
					double num2 = 0.0;
					for (int i = start; i < start + span; i++)
					{
						if (i >= sizeInfoList.Count)
						{
							num++;
							continue;
						}
						item = sizeInfoList[i];
						if (item == null || item.Empty)
						{
							num++;
						}
						else
						{
							num2 += item.Value;
						}
					}
					if (!(num2 < size))
					{
						return;
					}
					if (num == 0)
					{
						using (sizeInfoList.GetAndPin(start + span - 1, out item))
						{
							item.Value += size - num2;
						}
						return;
					}
					num2 = (size - num2) / (double)num;
					for (int j = start; j < start + span; j++)
					{
						if (j >= sizeInfoList.Count)
						{
							while (sizeInfoList.Count < j)
							{
								sizeInfoList.Add(null);
							}
							sizeInfoList.Add(new SizeInfo(num2));
							continue;
						}
						using (sizeInfoList.GetAndPin(j, out item))
						{
							if (item == null)
							{
								sizeInfoList[j] = new SizeInfo(num2);
							}
							else if (item.Empty)
							{
								item.Value = num2;
							}
						}
					}
					return;
				}
				while (sizeInfoList.Count <= start + span - 1)
				{
					sizeInfoList.Add(null);
				}
				using (sizeInfoList.GetAndPin(start, out item))
				{
					if (span == 1)
					{
						if (item == null)
						{
							sizeInfoList[start] = new SizeInfo(size);
						}
						else
						{
							item.Value = Math.Max(item.Value, size);
						}
						return;
					}
					if (item == null)
					{
						item = new SizeInfo(fixedData: false);
						sizeInfoList[start] = item;
					}
					item.AddSpanSize(span, size);
				}
			}

			protected void ResolveSizes(List<SizeInfo> sizeInfoList)
			{
				if (sizeInfoList == null)
				{
					return;
				}
				SizeInfo sizeInfo = null;
				Hashtable hashtable = null;
				double num = 0.0;
				double num2 = 0.0;
				int num3 = 0;
				for (int i = 0; i < sizeInfoList.Count; i++)
				{
					int num4 = 2;
					sizeInfo = sizeInfoList[i];
					hashtable = sizeInfo.SpanSize;
					if (hashtable == null)
					{
						continue;
					}
					while (hashtable.Count > 0)
					{
						if (hashtable[num4] != null)
						{
							num3 = 0;
							num2 = 0.0;
							num = (double)hashtable[num4];
							for (int j = i; j < i + num4; j++)
							{
								if (sizeInfoList[j] == null || sizeInfoList[j].Empty)
								{
									num3++;
								}
								else
								{
									num2 += sizeInfoList[j].Value;
								}
							}
							if (num2 < num)
							{
								if (num3 == 0)
								{
									sizeInfoList[i + num4 - 1].Value += num - num2;
								}
								else
								{
									num2 = (num - num2) / (double)num3;
									for (int k = i; k < i + num4; k++)
									{
										if (sizeInfoList[k] == null)
										{
											sizeInfoList[k] = new SizeInfo(num2);
										}
										else if (sizeInfoList[k].Empty)
										{
											sizeInfoList[k].Value = num2;
										}
									}
								}
							}
							hashtable.Remove(num4);
						}
						num4++;
					}
					sizeInfo.SpanSize = null;
				}
			}

			protected void ResolveSizes(ScalableList<SizeInfo> sizeInfoList)
			{
				if (sizeInfoList == null)
				{
					return;
				}
				List<ColumnSpanInfo> list = new List<ColumnSpanInfo>();
				SizeInfo item = null;
				Hashtable hashtable = null;
				for (int i = 0; i < sizeInfoList.Count; i++)
				{
					int num = 2;
					using (sizeInfoList.GetAndPin(i, out item))
					{
						if (item == null)
						{
							continue;
						}
						hashtable = item.SpanSize;
						if (hashtable == null)
						{
							continue;
						}
						while (hashtable.Count > 0)
						{
							if (hashtable[num] != null)
							{
								double spanSize = (double)hashtable[num];
								list.Add(new ColumnSpanInfo(i, num, spanSize));
								hashtable.Remove(num);
							}
							num++;
						}
						item.SpanSize = null;
					}
				}
				while (list.Count > 0)
				{
					int index = 0;
					ColumnSpanInfo columnSpanInfo = list[index];
					int num2 = columnSpanInfo.CalculateEmptyColumnns(sizeInfoList);
					for (int j = 1; j < list.Count; j++)
					{
						if (num2 == 0)
						{
							break;
						}
						int num3 = list[j].CalculateEmptyColumnns(sizeInfoList);
						if (num3 < num2)
						{
							index = j;
							columnSpanInfo = list[index];
							num2 = num3;
						}
					}
					double num4 = 0.0;
					for (int k = columnSpanInfo.Start; k < columnSpanInfo.Start + columnSpanInfo.Span; k++)
					{
						item = sizeInfoList[k];
						if (item != null && !item.Empty)
						{
							num4 += item.Value;
						}
					}
					if (num4 < columnSpanInfo.SpanSize)
					{
						if (num2 == 0)
						{
							for (int num5 = columnSpanInfo.Start + columnSpanInfo.Span - 1; num5 > columnSpanInfo.Start; num5--)
							{
								item = sizeInfoList[num5];
								if (item == null || !item.ZeroSized)
								{
									break;
								}
							}
							using (sizeInfoList.GetAndPin(columnSpanInfo.Start + columnSpanInfo.Span - 1, out item))
							{
								item.Value += columnSpanInfo.SpanSize - num4;
							}
						}
						else
						{
							num4 = (columnSpanInfo.SpanSize - num4) / (double)num2;
							for (int l = columnSpanInfo.Start; l < columnSpanInfo.Start + columnSpanInfo.Span; l++)
							{
								using (sizeInfoList.GetAndPin(l, out item))
								{
									if (item == null)
									{
										sizeInfoList[l] = new SizeInfo(num4);
									}
									else if (item.Empty)
									{
										item.Value = num4;
									}
								}
							}
						}
					}
					list.RemoveAt(index);
				}
			}

			internal void DelayedCalculation()
			{
				if (m_textBoxDelayCalc == null)
				{
					return;
				}
				if (!m_pageContext.CancelPage)
				{
					PageContext pageContext = new PageContext(m_pageContext, PageContext.PageContextFlags.FullOnPage, PageContext.IgnorePBReasonFlag.TablixParent);
					m_textBoxDelayCalc.CalcSizeState = TextBox.CalcSize.None;
					m_textBoxDelayCalc.ItemPageSizes.Width = m_lastDetailDefCellWidth;
					m_textBoxDelayCalc.ItemRenderSizes.Width = m_lastDetailDefCellWidth;
					m_textBoxDelayCalc.MeasureTextBox(pageContext, null, createForRepeat: false);
					m_textBoxDelayCalc.DelayWriteContent(m_rplWriter, m_pageContext);
					if (m_textBoxDelayCalc.ItemRenderSizes.Height > m_maxDetailRowHeightRender)
					{
						m_maxDetailRowHeightRender = m_textBoxDelayCalc.ItemRenderSizes.Height;
					}
				}
				m_textBoxDelayCalc = null;
			}

			internal abstract void WriteDetailRow(int rowIndex, double[] bodyRowHeights, bool ignoreCellPageBreaks);

			internal abstract void WriteTablixMeasurements(Microsoft.ReportingServices.OnDemandReportRendering.Tablix tablix, int rowMembersDepth, int colMembersDepth, ref double tablixWidth, ref double tablixHeight);

			protected abstract void OpenHeaderRow(bool omittedRow, int headerStart);

			protected abstract void OpenDetailRow(bool newRow);

			protected abstract void WriteCornerCells(int targetRow, ref int targetCol, int colIndex);

			protected abstract void WriteDetailOffsetRows();

			protected virtual void CloseRow()
			{
			}

			protected void WriteTablixContent(int startColForRowMembers, int rowMembersDepth)
			{
				if (m_pageContext.CancelPage)
				{
					return;
				}
				WriteCornerAndColumnMembers();
				if (m_pageContext.CancelPage)
				{
					return;
				}
				int rowIndex = m_headerColumnRows;
				if (m_isLTR)
				{
					bool newRow = true;
					if (m_rowHeaders != null)
					{
						WriteRowMembersLTR(null, m_rowHeaders, ref rowIndex, m_colsBeforeRowHeaders, ref newRow);
					}
					else
					{
						WriteDetailOffsetRows();
					}
				}
				else if (m_rowHeaders != null)
				{
					int[] state = new int[rowMembersDepth];
					int colIndex = startColForRowMembers;
					WriteRowMembersRTL(m_rowHeaders, ref rowIndex, ref colIndex, state, 0, startColForRowMembers);
				}
				else
				{
					WriteDetailOffsetRows();
				}
			}

			protected void WriteRowMembersLTR(PageMemberCell parentMember, ScalableList<PageMemberCell> rowHeaders, ref int rowIndex, int colIndex, ref bool newRow)
			{
				if (rowHeaders == null)
				{
					OpenDetailRow(newRow);
					CloseRow();
					rowIndex++;
					newRow = true;
					if (parentMember != null)
					{
						for (int i = 1; i < parentMember.RowSpan; i++)
						{
							OpenDetailRow(newRow);
							CloseRow();
							rowIndex++;
						}
					}
					return;
				}
				PageMemberCell pageMemberCell = null;
				for (int j = 0; j < rowHeaders.Count; j++)
				{
					pageMemberCell = rowHeaders[j];
					if (pageMemberCell.ColSpan > 0 || pageMemberCell.NeedWrite)
					{
						if (newRow)
						{
							OpenHeaderRow(omittedRow: false, -1);
							newRow = false;
						}
						pageMemberCell.WriteItemToStream(TablixRegion.RowHeader, m_rplWriter, rowIndex, colIndex);
					}
					WriteRowMembersLTR(pageMemberCell, pageMemberCell.Children, ref rowIndex, colIndex + pageMemberCell.ColSpan, ref newRow);
					rowHeaders[j] = null;
				}
				rowHeaders.Clear();
				rowHeaders = null;
			}

			protected void WriteRowMembersRTL(ScalableList<PageMemberCell> rowHeaders, ref int rowIndex, ref int colIndex, int[] state, int stateIndex, int startColIndex)
			{
				if (rowHeaders == null)
				{
					OpenDetailRow(newRow: true);
					colIndex = startColIndex;
					return;
				}
				PageMemberCell pageMemberCell = null;
				bool flag = true;
				int num = 0;
				for (int i = 0; i < rowHeaders.Count; i++)
				{
					pageMemberCell = rowHeaders[i];
					if (pageMemberCell == null)
					{
						if (num == 10)
						{
							rowHeaders.RemoveRange(0, 10);
							i -= 10;
							num = 0;
						}
						num++;
						continue;
					}
					flag = true;
					for (int j = state[stateIndex]; j < pageMemberCell.RowSpan; j++)
					{
						if (pageMemberCell.ColSpan == 0)
						{
							WriteRowMembersRTL(pageMemberCell.Children, ref rowIndex, ref colIndex, state, stateIndex + 1, startColIndex);
							flag = pageMemberCell.NeedWrite;
						}
						else
						{
							WriteRowMembersRTL(pageMemberCell.Children, ref rowIndex, ref colIndex, state, stateIndex + pageMemberCell.ColSpan, startColIndex);
						}
						if (j == 0 && flag)
						{
							pageMemberCell.WriteItemToStream(TablixRegion.RowHeader, m_rplWriter, rowIndex, colIndex);
							colIndex += pageMemberCell.ColSpan;
						}
						state[stateIndex]++;
						if (stateIndex == 0)
						{
							CloseRow();
							rowIndex++;
						}
						else if (state[stateIndex] < pageMemberCell.RowSpan)
						{
							return;
						}
					}
					state[stateIndex] = 0;
					rowHeaders[i] = null;
					num++;
					if (rowHeaders.Count > i + 1 && stateIndex > 0)
					{
						return;
					}
				}
				rowHeaders.Clear();
				rowHeaders = null;
			}

			protected void WriteCornerAndColumnMembers()
			{
				int i = 0;
				int num = 0;
				if (m_columnHeaders == null)
				{
					if (m_headerRowCols != 0 && m_headerColumnRows != 0)
					{
						for (; i < m_headerColumnRows; i++)
						{
							num = 0;
							OpenHeaderRow(omittedRow: false, 0);
							WriteCornerCells(i, ref num, 0);
							CloseRow();
						}
					}
					return;
				}
				int num2 = 0;
				int num3 = 0;
				int num4 = 0;
				if (m_colsBeforeRowHeaders > 0)
				{
					while (num4 < m_columnHeaders.Count)
					{
						num3 += m_columnHeaders[num4].ColSpan;
						num4++;
						if (num3 >= m_colsBeforeRowHeaders)
						{
							break;
						}
					}
				}
				bool writeOmittedHeaders = m_omittedOuterColumnHeaders;
				for (i = 0; i < m_headerColumnRows; i++)
				{
					num2 = 0;
					num = 0;
					if (writeOmittedHeaders)
					{
						WriteOmittedColHeadersRows(num4, i);
						writeOmittedHeaders = false;
					}
					OpenHeaderRow(omittedRow: false, 0);
					if (m_isLTR)
					{
						WriteColMembers(m_columnHeaders, i, 0, ref num2, 0, num4 - 1, ref writeOmittedHeaders);
					}
					else
					{
						WriteColMembers(m_columnHeaders, i, 0, ref num2, num4, m_columnHeaders.Count - 1, ref writeOmittedHeaders);
					}
					WriteCornerCells(i, ref num, num2);
					num2 += m_headerRowCols;
					if (m_isLTR)
					{
						WriteColMembers(m_columnHeaders, i, 0, ref num2, num4, m_columnHeaders.Count - 1, ref writeOmittedHeaders);
					}
					else
					{
						WriteColMembers(m_columnHeaders, i, 0, ref num2, 0, num4 - 1, ref writeOmittedHeaders);
					}
					CloseRow();
				}
				if (writeOmittedHeaders)
				{
					WriteOmittedColHeadersRows(num4, i);
				}
				m_columnHeaders.Clear();
				m_columnHeaders = null;
			}

			private void WriteColMembers(ScalableList<PageMemberCell> colHeaders, int targetRow, int rowIndex, ref int colIndex, int start, int end, ref bool writeOmittedHeaders)
			{
				if (colHeaders == null)
				{
					return;
				}
				PageMemberCell pageMemberCell = null;
				if (targetRow == rowIndex)
				{
					for (int i = start; i <= end; i++)
					{
						pageMemberCell = ((!m_isLTR) ? colHeaders[end - i + start] : colHeaders[i]);
						if (pageMemberCell.RowSpan > 0)
						{
							if (pageMemberCell.HasOmittedChildren && pageMemberCell.RowSpan == 1)
							{
								writeOmittedHeaders = true;
							}
							pageMemberCell.WriteItemToStream(TablixRegion.ColumnHeader, m_rplWriter, rowIndex, colIndex);
							colIndex += pageMemberCell.ColSpan;
						}
						else if (pageMemberCell.Children != null)
						{
							WriteColMembers(pageMemberCell.Children, targetRow, rowIndex, ref colIndex, 0, pageMemberCell.Children.Count - 1, ref writeOmittedHeaders);
						}
						else
						{
							colIndex += pageMemberCell.ColSpan;
						}
					}
					return;
				}
				for (int j = start; j <= end; j++)
				{
					pageMemberCell = ((!m_isLTR) ? colHeaders[end - j + start] : colHeaders[j]);
					if (targetRow < rowIndex + pageMemberCell.RowSpan)
					{
						colIndex += pageMemberCell.ColSpan;
						if (targetRow == rowIndex + pageMemberCell.RowSpan - 1 && pageMemberCell.RowSpan > 0 && pageMemberCell.HasOmittedChildren)
						{
							writeOmittedHeaders = true;
						}
					}
					else if (pageMemberCell.Children != null)
					{
						WriteColMembers(pageMemberCell.Children, targetRow, rowIndex + pageMemberCell.RowSpan, ref colIndex, 0, pageMemberCell.Children.Count - 1, ref writeOmittedHeaders);
					}
					else
					{
						colIndex += pageMemberCell.ColSpan;
					}
				}
			}

			private void WriteOmittedColHeadersRows(int outerGroupsBRHs, int rowIndex)
			{
				int num = 0;
				bool writeOmittedHeaders = true;
				int num2 = 0;
				bool flag = true;
				while (writeOmittedHeaders)
				{
					num = 0;
					writeOmittedHeaders = false;
					flag = true;
					if (m_isLTR)
					{
						WriteOmittedColMembers(m_columnHeaders, rowIndex, 0, num2, ref num, 0, outerGroupsBRHs - 1, ref writeOmittedHeaders, ref flag);
					}
					else
					{
						WriteOmittedColMembers(m_columnHeaders, rowIndex, 0, num2, ref num, outerGroupsBRHs, m_columnHeaders.Count - 1, ref writeOmittedHeaders, ref flag);
					}
					num += m_headerRowCols;
					if (m_isLTR)
					{
						WriteOmittedColMembers(m_columnHeaders, rowIndex, 0, num2, ref num, outerGroupsBRHs, m_columnHeaders.Count - 1, ref writeOmittedHeaders, ref flag);
					}
					else
					{
						WriteOmittedColMembers(m_columnHeaders, rowIndex, 0, num2, ref num, 0, outerGroupsBRHs - 1, ref writeOmittedHeaders, ref flag);
					}
					if (!flag)
					{
						CloseRow();
					}
					num2++;
				}
			}

			private void WriteOmittedColMembers(ScalableList<PageMemberCell> colHeaders, int targetRow, int rowIndex, int targetLevel, ref int colIndex, int start, int end, ref bool writeOmittedHeaders, ref bool openRow)
			{
				if (colHeaders == null)
				{
					return;
				}
				PageMemberCell pageMemberCell = null;
				if (targetRow == rowIndex)
				{
					WriteOmittedColMembersLevel(colHeaders, targetRow, targetLevel, 0, ref colIndex, start, end, ref writeOmittedHeaders, ref openRow);
					return;
				}
				for (int i = start; i <= end; i++)
				{
					pageMemberCell = ((!m_isLTR) ? colHeaders[end - i + start] : colHeaders[i]);
					if (targetRow < rowIndex + pageMemberCell.RowSpan)
					{
						colIndex += pageMemberCell.ColSpan;
					}
					else if (pageMemberCell.Children != null)
					{
						WriteOmittedColMembers(pageMemberCell.Children, targetRow, rowIndex + pageMemberCell.RowSpan, targetLevel, ref colIndex, 0, pageMemberCell.Children.Count - 1, ref writeOmittedHeaders, ref openRow);
					}
					else
					{
						colIndex += pageMemberCell.ColSpan;
					}
				}
			}

			private void WriteOmittedColMembersLevel(ScalableList<PageMemberCell> colHeaders, int targetRow, int targetLevel, int level, ref int colIndex, int start, int end, ref bool writeOmittedHeaders, ref bool openRow)
			{
				if (colHeaders == null)
				{
					return;
				}
				PageMemberCell pageMemberCell = null;
				if (targetLevel == level)
				{
					for (int i = start; i <= end; i++)
					{
						pageMemberCell = ((!m_isLTR) ? colHeaders[end - i + start] : colHeaders[i]);
						if (pageMemberCell.RowSpan == 0)
						{
							if (pageMemberCell.NeedWrite)
							{
								if (openRow)
								{
									OpenHeaderRow(omittedRow: true, -1);
									openRow = false;
								}
								pageMemberCell.WriteItemToStream(TablixRegion.ColumnHeader, m_rplWriter, targetRow, colIndex);
							}
							if (pageMemberCell.HasOmittedChildren)
							{
								writeOmittedHeaders = true;
							}
						}
						colIndex += pageMemberCell.ColSpan;
					}
					return;
				}
				bool flag = false;
				for (int j = start; j <= end; j++)
				{
					pageMemberCell = ((!m_isLTR) ? colHeaders[end - j + start] : colHeaders[j]);
					flag = false;
					if (pageMemberCell.RowSpan == 0 && pageMemberCell.HasOmittedChildren && pageMemberCell.Children != null)
					{
						WriteOmittedColMembersLevel(pageMemberCell.Children, targetRow, targetLevel, level + 1, ref colIndex, 0, pageMemberCell.Children.Count - 1, ref flag, ref openRow);
						if (flag)
						{
							writeOmittedHeaders = true;
						}
						else
						{
							pageMemberCell.HasOmittedChildren = false;
						}
					}
					else
					{
						colIndex += pageMemberCell.ColSpan;
					}
				}
			}

			private void RegisterToggleMemberPath(Tablix tablix, TablixMember rowMember)
			{
				if (tablix != null && rowMember != null && m_currentToggleMemberPath == null)
				{
					m_currentToggleMemberPath = tablix.BuildTablixMemberPath(rowMember);
				}
			}

			private void ResetToggleMemberPath()
			{
				m_currentToggleMemberPath = null;
			}

			private void TracePageGrownOnImplicitKeepTogetherMember()
			{
				if (m_currentToggleMemberPath != null)
				{
					RenderingDiagnostics.Trace(RenderingArea.KeepTogether, TraceLevel.Verbose, "PR-DIAG [Page {0}] Member '{1}' kept together - Implicit - Page grown", PageContext.PageNumber, m_currentToggleMemberPath);
				}
			}
		}

		internal sealed class StreamContext : TablixContext
		{
			private ScalableList<DetailCell> m_cellDetailRow;

			private Queue<long> m_detailRowOffsets;

			private CornerCell[,] m_cornerCells;

			private List<RPLTablixMemberDef> m_rowMemberDefList;

			private Hashtable m_rowMemberDefIndexes;

			private List<RPLTablixMemberDef> m_colMemberDefList;

			private Hashtable m_colMemberDefIndexes;

			internal StreamContext(RPLWriter rplWriter, PageItemHelper partialItemHelper, bool noRows, bool isLTR, PageContext pageContext, double cellsTopInPage, int headerRowCols, int headerColumnRows, int[] defDetailRowsCapacity, Interactivity interactivity)
				: base(rplWriter, partialItemHelper, noRows, isLTR, pageContext, cellsTopInPage, headerRowCols, headerColumnRows, defDetailRowsCapacity, interactivity)
			{
			}

			internal override void CreateDetailCell(PageItem topItem)
			{
				DetailCell detailCell = null;
				detailCell = ((topItem != null) ? new DetailCell(topItem.Offset, topItem.RplItemState) : new DetailCell(0L, 0));
				if (m_cellDetailRow == null)
				{
					m_cellDetailRow = new ScalableList<DetailCell>(0, base.Cache);
				}
				m_cellDetailRow.Add(detailCell);
			}

			internal override void UpdateDetailCell(double cellColDefWidth)
			{
				if (m_cellDetailRow != null)
				{
					DetailCell item = null;
					using (m_cellDetailRow.GetAndPin(m_cellDetailRow.Count - 1, out item))
					{
						item.ColSpan++;
					}
					m_lastDetailDefCellWidth += cellColDefWidth;
				}
			}

			internal override void UpdateLastDetailCellWidth()
			{
				if (m_cellDetailRow != null)
				{
					DetailCell detailCell = m_cellDetailRow[m_cellDetailRow.Count - 1];
					double size = Math.Max(m_lastDetailCellWidth, m_lastDetailDefCellWidth);
					UpdateSizes(m_lastDetailCellColIndex, detailCell.ColSpan, size, split: false, ref m_colWidths);
					m_lastDetailCellWidth = 0.0;
					m_lastDetailDefCellWidth = 0.0;
					m_lastDetailCellColIndex = 0;
				}
			}

			internal override bool HasCornerCells()
			{
				return m_cornerCells != null;
			}

			internal override void CreateCornerCell(PageItem topItem, int rowSpan, int colSpan, int rowIndex, int colIndex)
			{
				CornerCell cornerCell = new CornerCell(topItem.Offset, topItem.RplItemState, rowSpan, colSpan);
				if (m_cornerCells == null)
				{
					m_cornerCells = new CornerCell[m_headerColumnRows, m_headerRowCols];
				}
				m_cornerCells[rowIndex, colIndex] = cornerCell;
			}

			internal override PageMemberCell CreateMemberCell(PageItem topItem, int rowSpan, int colSpan, TablixMember tablixMember, byte state, int defTreeLevel, TablixRegion region)
			{
				MemberCell memberCell = null;
				memberCell = ((topItem != null) ? new MemberCell(topItem.Offset, topItem.RplItemState, rowSpan, colSpan, tablixMember) : new MemberCell(0L, 0, rowSpan, colSpan, tablixMember));
				if (region == TablixRegion.RowHeader)
				{
					memberCell.TablixMemberDefIndex = AddTablixMemberDef(ref m_rowMemberDefIndexes, ref m_rowMemberDefList, tablixMember, state, defTreeLevel, region);
				}
				else
				{
					memberCell.TablixMemberDefIndex = AddTablixMemberDef(ref m_colMemberDefIndexes, ref m_colMemberDefList, tablixMember, state, defTreeLevel, region);
				}
				return new StreamMemberCell(memberCell, state);
			}

			private int AddTablixMemberDef(ref Hashtable memberDefIndexes, ref List<RPLTablixMemberDef> memberDefList, TablixMember tablixMember, byte state, int defTreeLevel, TablixRegion region)
			{
				object obj = null;
				if (memberDefIndexes == null)
				{
					memberDefIndexes = new Hashtable();
					memberDefList = new List<RPLTablixMemberDef>();
				}
				else
				{
					obj = memberDefIndexes[tablixMember.DefinitionPath];
				}
				if (obj == null)
				{
					obj = memberDefList.Count;
					memberDefIndexes.Add(tablixMember.DefinitionPath, obj);
					byte b = 0;
					if (region == TablixRegion.ColumnHeader)
					{
						b = 1;
					}
					if ((state & 1) > 0)
					{
						b = 4;
					}
					if (tablixMember.IsStatic)
					{
						b = (byte)(b | 2);
					}
					RPLTablixMemberDef item = new RPLTablixMemberDef(tablixMember.DefinitionPath, tablixMember.MemberCellIndex, b, defTreeLevel);
					memberDefList.Add(item);
				}
				return (int)obj;
			}

			internal override void WriteDetailRow(int rowIndex, double[] tablixCellHeights, bool ignoreCellPageBreaks)
			{
				m_detailsOnPage = true;
				m_pageBreakAtEnd = false;
				if (m_rplWriter == null)
				{
					return;
				}
				DelayedCalculation();
				if (ignoreCellPageBreaks)
				{
					m_maxDetailRowHeightRender = Math.Max(m_maxDetailRowHeightRender, tablixCellHeights[m_rowMemberIndexCell]);
				}
				UpdateLastDetailCellWidth();
				RowMemberInfo rowMemberInfo = new RowMemberInfo(m_maxDetailRowHeightRender);
				if (m_cellDetailRow != null)
				{
					int num = 0;
					DetailCell detailCell = null;
					int num2 = m_cellDetailRow.Count - 1;
					if (m_sharedLayoutRow > 0)
					{
						if (m_staticDetailRow && num2 == 0)
						{
							rowMemberInfo.RowState = m_sharedLayoutRow;
							if (m_sharedLayoutRow == 2)
							{
								m_sharedLayoutRow = 4;
							}
						}
						else
						{
							m_sharedLayoutRow = 2;
						}
					}
					m_detailRowCellsCapacity[m_rowMemberIndexCell] = num2 + 1;
					BinaryWriter binaryWriter = m_rplWriter.BinaryWriter;
					if (m_detailRowOffsets == null)
					{
						m_detailRowOffsets = new Queue<long>();
					}
					m_detailRowOffsets.Enqueue(binaryWriter.BaseStream.Position);
					binaryWriter.Write((byte)18);
					binaryWriter.Write(rowIndex + m_headerColumnRows);
					for (int i = 0; i <= num2; i++)
					{
						if (m_isLTR)
						{
							detailCell = m_cellDetailRow[i];
							if (num == m_colsBeforeRowHeaders)
							{
								num += m_headerRowCols;
							}
						}
						else
						{
							detailCell = m_cellDetailRow[num2 - i];
							if (num == m_colWidths.Count - m_colsBeforeRowHeaders - m_headerRowCols)
							{
								num += m_headerRowCols;
							}
						}
						detailCell.WriteItemToStream(m_rplWriter, num);
						num += detailCell.ColSpan;
					}
					binaryWriter.Write(byte.MaxValue);
					m_cellDetailRow.Clear();
					m_cellDetailRow = null;
				}
				m_staticDetailRow = true;
				if (m_rowHeights == null)
				{
					m_rowHeights = new ScalableList<RowMemberInfo>(0, base.Cache);
				}
				m_rowHeights.Add(rowMemberInfo);
			}

			internal override void WriteTablixMeasurements(Microsoft.ReportingServices.OnDemandReportRendering.Tablix tablix, int rowMembersDepth, int colMembersDepth, ref double tablixWidth, ref double tablixHeight)
			{
				if (m_rplWriter == null || (m_colWidths == null && m_colHeaderHeights == null && m_rowHeights == null))
				{
					return;
				}
				int num = 0;
				ResolveSizes(m_colWidths);
				BinaryWriter binaryWriter = m_rplWriter.BinaryWriter;
				binaryWriter.Write((byte)0);
				binaryWriter.Write(m_headerColumnRows);
				binaryWriter.Write((byte)1);
				binaryWriter.Write(m_headerRowCols);
				if (m_colWidths != null)
				{
					binaryWriter.Write((byte)2);
					if (m_isLTR)
					{
						num = m_colsBeforeRowHeaders;
						binaryWriter.Write(num);
					}
					else
					{
						num = m_colWidths.Count - m_colsBeforeRowHeaders - m_headerRowCols;
						binaryWriter.Write(num);
						binaryWriter.Write((byte)3);
						binaryWriter.Write((byte)1);
					}
					binaryWriter.Write((byte)4);
					binaryWriter.Write(m_colWidths.Count);
					if (m_isLTR)
					{
						SizeInfo sizeInfo = null;
						if (m_colsBeforeRowHeaders > 0)
						{
							for (int i = m_headerRowCols; i < m_headerRowCols + m_colsBeforeRowHeaders; i++)
							{
								sizeInfo = m_colWidths[i];
								binaryWriter.Write((float)sizeInfo.Value);
								binaryWriter.Write(sizeInfo.Fixed);
								tablixWidth += sizeInfo.Value;
								m_colWidths[i] = null;
							}
							m_colWidths.RemoveRange(m_headerRowCols, m_colsBeforeRowHeaders);
						}
						for (int j = 0; j < m_headerRowCols; j++)
						{
							sizeInfo = m_colWidths[j];
							binaryWriter.Write((float)sizeInfo.Value);
							binaryWriter.Write(tablix.FixedRowHeaders);
							tablixWidth += sizeInfo.Value;
							m_colWidths[j] = null;
						}
						m_colWidths.RemoveRange(0, m_headerRowCols);
						for (int k = 0; k < m_colWidths.Count; k++)
						{
							sizeInfo = m_colWidths[k];
							binaryWriter.Write((float)sizeInfo.Value);
							binaryWriter.Write(sizeInfo.Fixed);
							tablixWidth += sizeInfo.Value;
							m_colWidths[k] = null;
						}
					}
					else
					{
						SizeInfo sizeInfo2 = null;
						int num2 = m_headerRowCols + m_colsBeforeRowHeaders;
						for (int num3 = m_colWidths.Count - 1; num3 >= num2; num3--)
						{
							sizeInfo2 = m_colWidths[num3];
							binaryWriter.Write((float)sizeInfo2.Value);
							binaryWriter.Write(sizeInfo2.Fixed);
							tablixWidth += sizeInfo2.Value;
							m_colWidths[num3] = null;
						}
						m_colWidths.RemoveRange(num2, m_colWidths.Count - num2);
						for (int num4 = m_headerRowCols - 1; num4 >= 0; num4--)
						{
							sizeInfo2 = m_colWidths[num4];
							binaryWriter.Write((float)sizeInfo2.Value);
							binaryWriter.Write(tablix.FixedRowHeaders);
							tablixWidth += sizeInfo2.Value;
							m_colWidths[num4] = null;
						}
						m_colWidths.RemoveRange(0, m_headerRowCols);
						for (int num5 = m_colWidths.Count - 1; num5 >= 0; num5--)
						{
							sizeInfo2 = m_colWidths[num5];
							binaryWriter.Write((float)sizeInfo2.Value);
							binaryWriter.Write(sizeInfo2.Fixed);
							tablixWidth += sizeInfo2.Value;
							m_colWidths[num5] = null;
						}
					}
					m_colWidths.Clear();
					m_colWidths = null;
				}
				if (m_colHeaderHeights != null || m_rowHeights != null)
				{
					binaryWriter.Write((byte)5);
					if (m_colHeaderHeights != null)
					{
						if (m_rowHeights != null)
						{
							binaryWriter.Write(m_rowHeights.Count + m_colHeaderHeights.Count);
						}
						else
						{
							binaryWriter.Write(m_colHeaderHeights.Count);
						}
						ResolveSizes(m_colHeaderHeights);
						for (int l = 0; l < m_colHeaderHeights.Count; l++)
						{
							binaryWriter.Write((float)m_colHeaderHeights[l].Value);
							binaryWriter.Write(tablix.FixedColumnHeaders);
							tablixHeight += m_colHeaderHeights[l].Value;
							m_colHeaderHeights[l] = null;
						}
						m_colHeaderHeights = null;
					}
					else
					{
						binaryWriter.Write(m_rowHeights.Count);
					}
					if (m_rowHeights != null)
					{
						RowMemberInfo rowMemberInfo = null;
						for (int m = 0; m < m_rowHeights.Count; m++)
						{
							rowMemberInfo = m_rowHeights[m];
							binaryWriter.Write((float)rowMemberInfo.Height);
							binaryWriter.Write(rowMemberInfo.RowState);
							tablixHeight += rowMemberInfo.Height;
							m_rowHeights[m] = null;
						}
						m_rowHeights.Clear();
						m_rowHeights = null;
					}
				}
				m_rowMemberDefIndexes = null;
				m_colMemberDefIndexes = null;
				WriteTablixMemberDefList(binaryWriter, ref m_rowMemberDefList, TablixRegion.RowHeader);
				WriteTablixMemberDefList(binaryWriter, ref m_colMemberDefList, TablixRegion.ColumnHeader);
				WriteTablixContent(num, rowMembersDepth);
			}

			private void WriteTablixMemberDefList(BinaryWriter spbifWriter, ref List<RPLTablixMemberDef> membersDefList, TablixRegion region)
			{
				if (membersDefList == null || membersDefList.Count == 0)
				{
					return;
				}
				if (region == TablixRegion.RowHeader)
				{
					spbifWriter.Write((byte)14);
				}
				else
				{
					spbifWriter.Write((byte)15);
				}
				spbifWriter.Write(membersDefList.Count);
				RPLTablixMemberDef rPLTablixMemberDef = null;
				for (int i = 0; i < membersDefList.Count; i++)
				{
					rPLTablixMemberDef = membersDefList[i];
					spbifWriter.Write((byte)16);
					if (rPLTablixMemberDef.DefinitionPath != null)
					{
						spbifWriter.Write((byte)0);
						spbifWriter.Write(rPLTablixMemberDef.DefinitionPath);
					}
					if (rPLTablixMemberDef.Level > 0)
					{
						spbifWriter.Write((byte)1);
						spbifWriter.Write(rPLTablixMemberDef.Level);
					}
					if (rPLTablixMemberDef.MemberCellIndex > 0)
					{
						spbifWriter.Write((byte)2);
						spbifWriter.Write(rPLTablixMemberDef.MemberCellIndex);
					}
					if (rPLTablixMemberDef.State > 0)
					{
						spbifWriter.Write((byte)3);
						spbifWriter.Write(rPLTablixMemberDef.State);
					}
					spbifWriter.Write(byte.MaxValue);
					membersDefList[i] = null;
				}
				membersDefList = null;
			}

			protected override void WriteDetailOffsetRows()
			{
				if (m_detailRowOffsets != null)
				{
					BinaryWriter binaryWriter = m_rplWriter.BinaryWriter;
					while (m_detailRowOffsets.Count > 0)
					{
						binaryWriter.Write((byte)8);
						binaryWriter.Write((byte)9);
						binaryWriter.Write(m_detailRowOffsets.Dequeue());
						binaryWriter.Write(byte.MaxValue);
					}
					m_detailRowOffsets = null;
				}
			}

			protected override void OpenDetailRow(bool newRow)
			{
				if (newRow)
				{
					m_rplWriter.BinaryWriter.Write((byte)8);
				}
				if (m_detailRowOffsets != null)
				{
					m_rplWriter.BinaryWriter.Write((byte)9);
					m_rplWriter.BinaryWriter.Write(m_detailRowOffsets.Dequeue());
				}
			}

			protected override void CloseRow()
			{
				m_rplWriter.BinaryWriter.Write(byte.MaxValue);
			}

			protected override void OpenHeaderRow(bool omittedRow, int headerStart)
			{
				m_rplWriter.BinaryWriter.Write((byte)8);
			}

			protected override void WriteCornerCells(int targetRow, ref int targetCol, int colIndex)
			{
				while (targetCol < m_headerRowCols)
				{
					CornerCell cornerCell = null;
					cornerCell = ((!m_isLTR) ? m_cornerCells[targetRow, m_headerRowCols - targetCol - 1] : m_cornerCells[targetRow, targetCol]);
					if (cornerCell != null)
					{
						if (m_isLTR)
						{
							cornerCell.WriteItemToStream(m_rplWriter, targetRow, colIndex + targetCol);
						}
						else
						{
							cornerCell.WriteItemToStream(m_rplWriter, targetRow, colIndex + targetCol - cornerCell.ColSpan + 1);
						}
						targetCol += cornerCell.ColSpan;
					}
					else
					{
						targetCol++;
					}
				}
			}
		}

		internal class RPLContext : TablixContext
		{
			private RPLTablix m_rplTablix;

			protected List<RPLTablixCell> m_cellDetailRow;

			private Queue<RPLTablixRow> m_detailRowRPLTablixCells;

			protected RPLTablixCornerCell[,] m_cornerCells;

			private Hashtable m_rplTablixMembersDef;

			internal RPLContext(RPLWriter rplWriter, PageItemHelper partialItemHelper, bool noRows, bool isLTR, PageContext pageContext, double cellsTopInPage, int headerRowCols, int headerColumnRows, int[] defDetailRowsCapacity, Interactivity interactivity, RPLTablix rplTablix)
				: base(rplWriter, partialItemHelper, noRows, isLTR, pageContext, cellsTopInPage, headerRowCols, headerColumnRows, defDetailRowsCapacity, interactivity)
			{
				m_rplTablix = rplTablix;
			}

			internal override void CreateDetailCell(PageItem topItem)
			{
				if (!base.PageContext.CancelPage)
				{
					RPLTablixCell rPLTablixCell = null;
					rPLTablixCell = ((topItem != null) ? new RPLTablixCell(topItem.RPLElement, topItem.RplItemState) : new RPLTablixCell());
					if (m_cellDetailRow == null)
					{
						m_cellDetailRow = new List<RPLTablixCell>(m_detailRowCellsCapacity[m_rowMemberIndexCell]);
					}
					m_cellDetailRow.Add(rPLTablixCell);
				}
			}

			internal override void UpdateDetailCell(double cellColDefWidth)
			{
				if (m_cellDetailRow != null)
				{
					m_cellDetailRow[m_cellDetailRow.Count - 1].ColSpan++;
					m_lastDetailDefCellWidth += cellColDefWidth;
				}
			}

			internal override void UpdateLastDetailCellWidth()
			{
				if (m_cellDetailRow != null)
				{
					RPLTablixCell rPLTablixCell = m_cellDetailRow[m_cellDetailRow.Count - 1];
					double size = Math.Max(m_lastDetailCellWidth, m_lastDetailDefCellWidth);
					UpdateSizes(m_lastDetailCellColIndex, rPLTablixCell.ColSpan, size, split: false, ref m_colWidths);
					m_lastDetailCellWidth = 0.0;
					m_lastDetailDefCellWidth = 0.0;
					m_lastDetailCellColIndex = 0;
				}
			}

			internal override bool HasCornerCells()
			{
				return m_cornerCells != null;
			}

			internal override void CreateCornerCell(PageItem topItem, int rowSpan, int colSpan, int rowIndex, int colIndex)
			{
				if (!base.PageContext.CancelPage)
				{
					RPLTablixCornerCell rPLTablixCornerCell = new RPLTablixCornerCell(topItem.RPLElement, topItem.RplItemState, rowSpan, colSpan);
					if (m_cornerCells == null)
					{
						m_cornerCells = new RPLTablixCornerCell[m_headerColumnRows, m_headerRowCols];
					}
					m_cornerCells[rowIndex, colIndex] = rPLTablixCornerCell;
				}
			}

			internal override PageMemberCell CreateMemberCell(PageItem topItem, int rowSpan, int colSpan, TablixMember tablixMember, byte state, int defTreeLevel, TablixRegion region)
			{
				if (base.PageContext.CancelPage)
				{
					return null;
				}
				RPLTablixMemberCell rPLTablixMemberCell = null;
				rPLTablixMemberCell = ((topItem != null) ? new RPLTablixMemberCell(topItem.RPLElement, topItem.RplItemState, rowSpan, colSpan) : new RPLTablixMemberCell(null, 0, rowSpan, colSpan));
				Group group = tablixMember.Group;
				if (group != null)
				{
					GroupInstance instance = group.Instance;
					rPLTablixMemberCell.UniqueName = instance.UniqueName;
					if (group.DocumentMapLabel != null)
					{
						if (group.DocumentMapLabel.IsExpression)
						{
							rPLTablixMemberCell.GroupLabel = instance.DocumentMapLabel;
						}
						else
						{
							rPLTablixMemberCell.GroupLabel = group.DocumentMapLabel.Value;
						}
					}
					rPLTablixMemberCell.RecursiveToggleLevel = -1;
					if (tablixMember.Visibility != null && tablixMember.Visibility.RecursiveToggleReceiver)
					{
						rPLTablixMemberCell.RecursiveToggleLevel = instance.RecursiveLevel;
					}
				}
				RPLTablixMemberDef rPLTablixMemberDef = null;
				if (m_rplTablixMembersDef == null)
				{
					m_rplTablixMembersDef = new Hashtable();
				}
				else
				{
					rPLTablixMemberDef = (m_rplTablixMembersDef[tablixMember.DefinitionPath] as RPLTablixMemberDef);
				}
				if (rPLTablixMemberDef == null)
				{
					byte b = 0;
					if (region == TablixRegion.ColumnHeader)
					{
						b = 1;
					}
					if ((state & 1) > 0)
					{
						b = (byte)(b | 4);
					}
					if (tablixMember.IsStatic)
					{
						b = (byte)(b | 2);
					}
					rPLTablixMemberDef = new RPLTablixMemberDef(tablixMember.DefinitionPath, tablixMember.MemberCellIndex, b, defTreeLevel);
					m_rplTablixMembersDef.Add(tablixMember.DefinitionPath, rPLTablixMemberDef);
				}
				rPLTablixMemberCell.TablixMemberDef = rPLTablixMemberDef;
				return new RPLMemberCell(rPLTablixMemberCell, state);
			}

			internal override void WriteDetailRow(int rowIndex, double[] tablixCellHeights, bool ignoreCellPageBreaks)
			{
				m_detailsOnPage = true;
				m_pageBreakAtEnd = false;
				if (m_rplWriter == null)
				{
					return;
				}
				if (base.PageContext.CancelPage)
				{
					m_cellDetailRow = null;
					m_detailRowRPLTablixCells = null;
					return;
				}
				DelayedCalculation();
				if (ignoreCellPageBreaks)
				{
					m_maxDetailRowHeightRender = Math.Max(m_maxDetailRowHeightRender, tablixCellHeights[m_rowMemberIndexCell]);
				}
				UpdateLastDetailCellWidth();
				RowMemberInfo rowMemberInfo = new RowMemberInfo(m_maxDetailRowHeightRender);
				if (m_cellDetailRow != null)
				{
					int num = 0;
					RPLTablixCell rPLTablixCell = null;
					int num2 = m_cellDetailRow.Count - 1;
					if (m_sharedLayoutRow > 0)
					{
						if (m_staticDetailRow && num2 == 0)
						{
							rowMemberInfo.RowState = m_sharedLayoutRow;
							if (m_sharedLayoutRow == 2)
							{
								m_sharedLayoutRow = 4;
							}
						}
						else
						{
							m_sharedLayoutRow = 2;
						}
					}
					m_detailRowCellsCapacity[m_rowMemberIndexCell] = num2 + 1;
					List<RPLTablixCell> list = null;
					list = ((!m_isLTR) ? new List<RPLTablixCell>(num2) : m_cellDetailRow);
					if (m_detailRowRPLTablixCells == null)
					{
						m_detailRowRPLTablixCells = new Queue<RPLTablixRow>();
					}
					m_detailRowRPLTablixCells.Enqueue(new RPLTablixRow(list));
					for (int i = 0; i <= num2; i++)
					{
						if (m_isLTR)
						{
							rPLTablixCell = m_cellDetailRow[i];
							if (num == m_colsBeforeRowHeaders)
							{
								num += m_headerRowCols;
							}
						}
						else
						{
							rPLTablixCell = m_cellDetailRow[num2 - i];
							if (num == m_colWidths.Count - m_colsBeforeRowHeaders - m_headerRowCols)
							{
								num += m_headerRowCols;
							}
							list.Add(rPLTablixCell);
						}
						rPLTablixCell.ColIndex = num;
						rPLTablixCell.RowIndex = rowIndex + m_headerColumnRows;
						num += rPLTablixCell.ColSpan;
					}
					m_cellDetailRow = null;
				}
				m_staticDetailRow = true;
				if (m_rowHeights == null)
				{
					m_rowHeights = new ScalableList<RowMemberInfo>(0, base.Cache);
				}
				m_rowHeights.Add(rowMemberInfo);
			}

			internal override void WriteTablixMeasurements(Microsoft.ReportingServices.OnDemandReportRendering.Tablix tablix, int rowMembersDepth, int colMembersDepth, ref double tablixWidth, ref double tablixHeight)
			{
				if (m_rplWriter == null || (m_colWidths == null && m_colHeaderHeights == null && m_rowHeights == null))
				{
					return;
				}
				RPLTablix rplTablix = m_rplTablix;
				int num = 0;
				ResolveSizes(m_colWidths);
				rplTablix.ColumnHeaderRows = m_headerColumnRows;
				rplTablix.RowHeaderColumns = m_headerRowCols;
				int num2 = 0;
				if (m_colWidths != null)
				{
					if (m_isLTR)
					{
						num = m_colsBeforeRowHeaders;
					}
					else
					{
						num = m_colWidths.Count - m_colsBeforeRowHeaders - m_headerRowCols;
						rplTablix.LayoutDirection = RPLFormat.Directions.RTL;
					}
					rplTablix.ColsBeforeRowHeaders = num;
					rplTablix.ColumnWidths = new float[m_colWidths.Count];
					rplTablix.FixedColumns = new bool[m_colWidths.Count];
					if (m_isLTR)
					{
						SizeInfo sizeInfo = null;
						if (m_colsBeforeRowHeaders > 0)
						{
							for (int i = m_headerRowCols; i < m_headerRowCols + m_colsBeforeRowHeaders; i++)
							{
								sizeInfo = m_colWidths[i];
								rplTablix.ColumnWidths[num2] = (float)sizeInfo.Value;
								rplTablix.FixedColumns[num2] = sizeInfo.Fixed;
								num2++;
								tablixWidth += sizeInfo.Value;
								m_colWidths[i] = null;
							}
							m_colWidths.RemoveRange(m_headerRowCols, m_colsBeforeRowHeaders);
						}
						for (int j = 0; j < m_headerRowCols; j++)
						{
							sizeInfo = m_colWidths[j];
							rplTablix.ColumnWidths[num2] = (float)sizeInfo.Value;
							rplTablix.FixedColumns[num2] = tablix.FixedRowHeaders;
							num2++;
							tablixWidth += sizeInfo.Value;
							m_colWidths[j] = null;
						}
						m_colWidths.RemoveRange(0, m_headerRowCols);
						for (int k = 0; k < m_colWidths.Count; k++)
						{
							sizeInfo = m_colWidths[k];
							rplTablix.ColumnWidths[num2] = (float)sizeInfo.Value;
							rplTablix.FixedColumns[num2] = sizeInfo.Fixed;
							num2++;
							tablixWidth += sizeInfo.Value;
							m_colWidths[k] = null;
						}
					}
					else
					{
						SizeInfo sizeInfo2 = null;
						int num3 = m_headerRowCols + m_colsBeforeRowHeaders;
						for (int num4 = m_colWidths.Count - 1; num4 >= num3; num4--)
						{
							sizeInfo2 = m_colWidths[num4];
							rplTablix.ColumnWidths[num2] = (float)sizeInfo2.Value;
							rplTablix.FixedColumns[num2] = sizeInfo2.Fixed;
							num2++;
							tablixWidth += sizeInfo2.Value;
							m_colWidths[num4] = null;
						}
						m_colWidths.RemoveRange(num3, m_colWidths.Count - num3);
						for (int num5 = m_headerRowCols - 1; num5 >= 0; num5--)
						{
							sizeInfo2 = m_colWidths[num5];
							rplTablix.ColumnWidths[num2] = (float)sizeInfo2.Value;
							rplTablix.FixedColumns[num2] = tablix.FixedRowHeaders;
							num2++;
							tablixWidth += sizeInfo2.Value;
							m_colWidths[num5] = null;
						}
						m_colWidths.RemoveRange(0, m_headerRowCols);
						for (int num6 = m_colWidths.Count - 1; num6 >= 0; num6--)
						{
							sizeInfo2 = m_colWidths[num6];
							rplTablix.ColumnWidths[num2] = (float)sizeInfo2.Value;
							rplTablix.FixedColumns[num2] = sizeInfo2.Fixed;
							num2++;
							tablixWidth += sizeInfo2.Value;
							m_colWidths[num6] = null;
						}
					}
					m_colWidths = null;
				}
				num2 = 0;
				if (m_colHeaderHeights != null || m_rowHeights != null)
				{
					if (m_colHeaderHeights != null)
					{
						if (m_rowHeights != null)
						{
							rplTablix.RowHeights = new float[m_rowHeights.Count + m_colHeaderHeights.Count];
							rplTablix.RowsState = new byte[m_rowHeights.Count + m_colHeaderHeights.Count];
						}
						else
						{
							rplTablix.RowHeights = new float[m_colHeaderHeights.Count];
							rplTablix.RowsState = new byte[m_colHeaderHeights.Count];
						}
						ResolveSizes(m_colHeaderHeights);
						for (int l = 0; l < m_colHeaderHeights.Count; l++)
						{
							rplTablix.RowHeights[num2] = (float)m_colHeaderHeights[l].Value;
							if (tablix.FixedColumnHeaders)
							{
								rplTablix.RowsState[num2] = 1;
							}
							num2++;
							tablixHeight += m_colHeaderHeights[l].Value;
							m_colHeaderHeights[l] = null;
						}
						m_colHeaderHeights = null;
					}
					else
					{
						rplTablix.RowHeights = new float[m_rowHeights.Count];
						rplTablix.RowsState = new byte[m_rowHeights.Count];
					}
					if (m_rowHeights != null)
					{
						RowMemberInfo rowMemberInfo = null;
						for (int m = 0; m < m_rowHeights.Count; m++)
						{
							rowMemberInfo = m_rowHeights[m];
							rplTablix.RowHeights[num2] = (float)rowMemberInfo.Height;
							rplTablix.RowsState[num2] = rowMemberInfo.RowState;
							num2++;
							tablixHeight += rowMemberInfo.Height;
							m_rowHeights[m] = null;
						}
						m_rowHeights.Clear();
						m_rowHeights = null;
					}
				}
				m_rplTablixMembersDef = null;
				WriteTablixContent(num, rowMembersDepth);
			}

			protected override void WriteDetailOffsetRows()
			{
				if (!base.PageContext.CancelPage && m_detailRowRPLTablixCells != null)
				{
					while (m_detailRowRPLTablixCells.Count > 0)
					{
						m_rplTablix.AddRow(m_detailRowRPLTablixCells.Dequeue());
					}
					m_detailRowRPLTablixCells = null;
				}
				m_detailRowRPLTablixCells = null;
			}

			protected override void OpenHeaderRow(bool omittedRow, int headerStart)
			{
				RPLTablixRow rPLTablixRow = null;
				rPLTablixRow = ((!omittedRow) ? ((RPLTablixRow)new RPLTablixFullRow(headerStart, -1)) : ((RPLTablixRow)new RPLTablixOmittedRow()));
				if (!base.PageContext.CancelPage)
				{
					m_rplTablix.AddRow(rPLTablixRow);
				}
				m_rplWriter.TablixRow = rPLTablixRow;
			}

			protected override void OpenDetailRow(bool newRow)
			{
				if (newRow)
				{
					RPLTablixFullRow rPLTablixFullRow = new RPLTablixFullRow(-1, 0);
					if (!base.PageContext.CancelPage)
					{
						m_rplTablix.AddRow(rPLTablixFullRow);
					}
					m_rplWriter.TablixRow = rPLTablixFullRow;
				}
				if (m_detailRowRPLTablixCells != null)
				{
					RPLTablixRow rPLTablixRow = m_detailRowRPLTablixCells.Dequeue();
					m_rplWriter.TablixRow.SetBodyStart();
					m_rplWriter.TablixRow.AddCells(rPLTablixRow.RowCells);
				}
			}

			protected override void WriteCornerCells(int targetRow, ref int targetCol, int colIndex)
			{
				while (targetCol < m_headerRowCols)
				{
					RPLTablixCornerCell rPLTablixCornerCell = null;
					rPLTablixCornerCell = ((!m_isLTR) ? m_cornerCells[targetRow, m_headerRowCols - targetCol - 1] : m_cornerCells[targetRow, targetCol]);
					if (rPLTablixCornerCell != null)
					{
						rPLTablixCornerCell.RowIndex = targetRow;
						if (m_isLTR)
						{
							rPLTablixCornerCell.ColIndex = colIndex + targetCol;
						}
						else
						{
							rPLTablixCornerCell.ColIndex = colIndex + targetCol - rPLTablixCornerCell.ColSpan + 1;
						}
						m_rplWriter.TablixRow.RowCells.Add(rPLTablixCornerCell);
						targetCol += rPLTablixCornerCell.ColSpan;
					}
					else
					{
						targetCol++;
					}
				}
			}
		}

		internal class LevelInfo
		{
			private int m_spanForParent;

			private double m_sizeForParent;

			private ScalableList<PageMemberCell> m_memberCells;

			private bool m_omittedList = true;

			private bool m_omittedMembersCells;

			private int m_ignoredRowsCols;

			private double m_sourceSize;

			private int m_sourceIndex = -1;

			private bool m_hasVisibleStaticPeer;

			internal int SpanForParent
			{
				get
				{
					return m_spanForParent;
				}
				set
				{
					m_spanForParent = value;
				}
			}

			internal double SizeForParent
			{
				get
				{
					return m_sizeForParent;
				}
				set
				{
					m_sizeForParent = value;
				}
			}

			internal ScalableList<PageMemberCell> MemberCells
			{
				get
				{
					return m_memberCells;
				}
				set
				{
					m_memberCells = value;
				}
			}

			internal bool OmittedMembersCells
			{
				get
				{
					return m_omittedMembersCells;
				}
				set
				{
					m_omittedMembersCells = value;
				}
			}

			internal int IgnoredRowsCols
			{
				get
				{
					return m_ignoredRowsCols;
				}
				set
				{
					m_ignoredRowsCols = value;
				}
			}

			internal bool OmittedList => m_omittedList;

			internal double SourceSize => m_sourceSize;

			internal bool HasVisibleStaticPeer
			{
				get
				{
					return m_hasVisibleStaticPeer;
				}
				set
				{
					m_hasVisibleStaticPeer = value;
				}
			}

			internal void AddMemberCell(PageMemberCell memberCell, int span, int priority, int sourceIndex, double sourceSize, IScalabilityCache cache)
			{
				if (m_memberCells == null)
				{
					m_memberCells = new ScalableList<PageMemberCell>(priority, cache);
				}
				m_memberCells.Add(memberCell);
				if (span > 0 || memberCell.NeedWrite || memberCell.Children != null)
				{
					m_omittedList = false;
				}
				if (sourceIndex > m_sourceIndex)
				{
					m_sourceSize += sourceSize;
					m_sourceIndex = sourceIndex;
				}
			}

			internal void SetDefaults()
			{
				m_spanForParent = 0;
				m_sizeForParent = 0.0;
				m_memberCells = null;
				m_omittedList = true;
				m_omittedMembersCells = false;
				m_ignoredRowsCols = 0;
				m_sourceSize = 0.0;
				m_sourceIndex = -1;
				m_hasVisibleStaticPeer = false;
			}
		}

		internal class EmptyCell : ItemOffset
		{
			public long Offset
			{
				get
				{
					return 0L;
				}
				set
				{
				}
			}
		}

		internal class DetailCell : IStorable, IPersistable
		{
			protected int m_colSpan = 1;

			protected long m_offset;

			protected byte m_cellItemState;

			private static Declaration m_declaration = GetDeclaration();

			internal virtual int RowSpan
			{
				get
				{
					return 1;
				}
				set
				{
				}
			}

			internal virtual int ColSpan
			{
				get
				{
					return m_colSpan;
				}
				set
				{
					m_colSpan = value;
				}
			}

			public virtual int Size => 13;

			internal DetailCell()
			{
			}

			internal DetailCell(long offset, byte itemState)
			{
				m_offset = offset;
				m_cellItemState = itemState;
			}

			internal DetailCell(long offset, byte itemState, int colSpan)
			{
				m_offset = offset;
				m_colSpan = colSpan;
				m_cellItemState = itemState;
			}

			public virtual void Serialize(IntermediateFormatWriter writer)
			{
				writer.RegisterDeclaration(m_declaration);
				while (writer.NextMember())
				{
					switch (writer.CurrentMember.MemberName)
					{
					case MemberName.ColSpan:
						writer.Write(m_colSpan);
						break;
					case MemberName.Offset:
						writer.Write(m_offset);
						break;
					case MemberName.State:
						writer.Write(m_cellItemState);
						break;
					default:
						RSTrace.RenderingTracer.Assert(condition: false);
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
					case MemberName.ColSpan:
						m_colSpan = reader.ReadInt32();
						break;
					case MemberName.Offset:
						m_offset = reader.ReadInt64();
						break;
					case MemberName.State:
						m_cellItemState = reader.ReadByte();
						break;
					default:
						RSTrace.RenderingTracer.Assert(condition: false);
						break;
					}
				}
			}

			public void ResolveReferences(Dictionary<ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
			{
			}

			public virtual ObjectType GetObjectType()
			{
				return ObjectType.DetailCell;
			}

			internal static Declaration GetDeclaration()
			{
				if (m_declaration == null)
				{
					List<MemberInfo> list = new List<MemberInfo>();
					list.Add(new MemberInfo(MemberName.ColSpan, Token.Int32));
					list.Add(new MemberInfo(MemberName.Offset, Token.Int64));
					list.Add(new MemberInfo(MemberName.State, Token.Byte));
					return new Declaration(ObjectType.DetailCell, ObjectType.None, list);
				}
				return m_declaration;
			}

			internal void WriteItemToStream(RPLWriter rplWriter, int colIndex)
			{
				BinaryWriter binaryWriter = rplWriter.BinaryWriter;
				binaryWriter.Write((byte)13);
				if (m_offset > 0)
				{
					binaryWriter.Write((byte)4);
					binaryWriter.Write(m_offset);
				}
				if (m_cellItemState > 0)
				{
					binaryWriter.Write((byte)13);
					binaryWriter.Write(m_cellItemState);
				}
				if (m_colSpan != 1)
				{
					binaryWriter.Write((byte)5);
					binaryWriter.Write(m_colSpan);
				}
				binaryWriter.Write((byte)8);
				binaryWriter.Write(colIndex);
				binaryWriter.Write(byte.MaxValue);
			}
		}

		internal class CornerCell : DetailCell
		{
			protected int m_rowSpan = 1;

			private static Declaration m_declaration = GetDeclaration();

			internal override int RowSpan
			{
				get
				{
					return m_rowSpan;
				}
				set
				{
					m_rowSpan = value;
				}
			}

			public override int Size => base.Size + 4;

			internal CornerCell()
			{
			}

			internal CornerCell(long offset, byte itemState, int rowSpan, int colSpan)
				: base(offset, itemState, colSpan)
			{
				m_rowSpan = rowSpan;
			}

			internal void WriteItemToStream(RPLWriter rplWriter, int rowIndex, int colIndex)
			{
				BinaryWriter binaryWriter = rplWriter.BinaryWriter;
				binaryWriter.Write((byte)10);
				if (m_offset > 0)
				{
					binaryWriter.Write((byte)4);
					binaryWriter.Write(m_offset);
				}
				if (m_cellItemState > 0)
				{
					binaryWriter.Write((byte)13);
					binaryWriter.Write(m_cellItemState);
				}
				if (m_colSpan != 1)
				{
					binaryWriter.Write((byte)5);
					binaryWriter.Write(m_colSpan);
				}
				if (m_rowSpan != 1)
				{
					binaryWriter.Write((byte)6);
					binaryWriter.Write(m_rowSpan);
				}
				binaryWriter.Write((byte)9);
				binaryWriter.Write(rowIndex);
				binaryWriter.Write((byte)8);
				binaryWriter.Write(colIndex);
				binaryWriter.Write(byte.MaxValue);
			}

			public override void Serialize(IntermediateFormatWriter writer)
			{
				base.Serialize(writer);
				writer.RegisterDeclaration(m_declaration);
				while (writer.NextMember())
				{
					MemberName memberName = writer.CurrentMember.MemberName;
					if (memberName == MemberName.RowSpan)
					{
						writer.Write(m_rowSpan);
					}
					else
					{
						RSTrace.RenderingTracer.Assert(condition: false);
					}
				}
			}

			public override void Deserialize(IntermediateFormatReader reader)
			{
				base.Deserialize(reader);
				reader.RegisterDeclaration(m_declaration);
				while (reader.NextMember())
				{
					MemberName memberName = reader.CurrentMember.MemberName;
					if (memberName == MemberName.RowSpan)
					{
						m_rowSpan = reader.ReadInt32();
					}
					else
					{
						RSTrace.RenderingTracer.Assert(condition: false);
					}
				}
			}

			public override ObjectType GetObjectType()
			{
				return ObjectType.CornerCell;
			}

			internal new static Declaration GetDeclaration()
			{
				if (m_declaration == null)
				{
					List<MemberInfo> list = new List<MemberInfo>();
					list.Add(new MemberInfo(MemberName.RowSpan, Token.Int32));
					return new Declaration(ObjectType.CornerCell, ObjectType.DetailCell, list);
				}
				return m_declaration;
			}
		}

		internal class MemberCell : CornerCell
		{
			private string m_uniqueName;

			private string m_label;

			private int m_recursiveToggleLevel = -1;

			private int m_tablixMemberDefIndex = -1;

			private static Declaration m_declaration = GetDeclaration();

			internal string GroupLabel => m_label;

			internal int RecursiveToggleLevel => m_recursiveToggleLevel;

			internal int TablixMemberDefIndex
			{
				set
				{
					m_tablixMemberDefIndex = value;
				}
			}

			public override int Size => base.Size + 8 + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(m_uniqueName) + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(m_label);

			internal MemberCell()
			{
			}

			internal MemberCell(long offset, byte itemState, int rowSpan, int colSpan, TablixMember tablixMember)
				: base(offset, itemState, rowSpan, colSpan)
			{
				Group group = tablixMember.Group;
				if (group == null)
				{
					return;
				}
				GroupInstance instance = group.Instance;
				m_uniqueName = instance.UniqueName;
				if (group.DocumentMapLabel != null)
				{
					if (group.DocumentMapLabel.IsExpression)
					{
						m_label = instance.DocumentMapLabel;
					}
					else
					{
						m_label = group.DocumentMapLabel.Value;
					}
				}
				m_recursiveToggleLevel = -1;
				if (tablixMember.Visibility != null && tablixMember.Visibility.RecursiveToggleReceiver)
				{
					m_recursiveToggleLevel = instance.RecursiveLevel;
				}
			}

			internal void WriteItemToStream(TablixRegion region, RPLWriter rplWriter, int rowIndex, int colIndex, byte state)
			{
				BinaryWriter binaryWriter = rplWriter.BinaryWriter;
				if (region == TablixRegion.ColumnHeader)
				{
					binaryWriter.Write((byte)11);
				}
				else
				{
					binaryWriter.Write((byte)12);
				}
				if (m_offset > 0)
				{
					binaryWriter.Write((byte)4);
					binaryWriter.Write(m_offset);
				}
				if (m_cellItemState > 0)
				{
					binaryWriter.Write((byte)13);
					binaryWriter.Write(m_cellItemState);
				}
				if (m_colSpan != 1)
				{
					binaryWriter.Write((byte)5);
					binaryWriter.Write(m_colSpan);
				}
				if (m_rowSpan != 1)
				{
					binaryWriter.Write((byte)6);
					binaryWriter.Write(m_rowSpan);
				}
				if (m_tablixMemberDefIndex >= 0)
				{
					binaryWriter.Write((byte)7);
					binaryWriter.Write(m_tablixMemberDefIndex);
				}
				binaryWriter.Write((byte)9);
				binaryWriter.Write(rowIndex);
				binaryWriter.Write((byte)8);
				binaryWriter.Write(colIndex);
				if (m_uniqueName != null)
				{
					binaryWriter.Write((byte)11);
					binaryWriter.Write(m_uniqueName);
				}
				if (m_label != null)
				{
					binaryWriter.Write((byte)10);
					binaryWriter.Write(m_label);
				}
				binaryWriter.Write((byte)14);
				binaryWriter.Write(m_recursiveToggleLevel);
				if (state > 0)
				{
					binaryWriter.Write((byte)12);
					binaryWriter.Write(state);
				}
				binaryWriter.Write(byte.MaxValue);
			}

			public override void Serialize(IntermediateFormatWriter writer)
			{
				base.Serialize(writer);
				writer.RegisterDeclaration(m_declaration);
				while (writer.NextMember())
				{
					switch (writer.CurrentMember.MemberName)
					{
					case MemberName.UniqueName:
						writer.Write(m_uniqueName);
						break;
					case MemberName.Label:
						writer.Write(m_label);
						break;
					case MemberName.RecursiveLevel:
						writer.Write(m_recursiveToggleLevel);
						break;
					case MemberName.DefIndex:
						writer.Write(m_tablixMemberDefIndex);
						break;
					default:
						RSTrace.RenderingTracer.Assert(condition: false);
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
					case MemberName.UniqueName:
						m_uniqueName = reader.ReadString();
						break;
					case MemberName.Label:
						m_label = reader.ReadString();
						break;
					case MemberName.RecursiveLevel:
						m_recursiveToggleLevel = reader.ReadInt32();
						break;
					case MemberName.DefIndex:
						m_tablixMemberDefIndex = reader.ReadInt32();
						break;
					default:
						RSTrace.RenderingTracer.Assert(condition: false);
						break;
					}
				}
			}

			public override ObjectType GetObjectType()
			{
				return ObjectType.MemberCell;
			}

			internal new static Declaration GetDeclaration()
			{
				if (m_declaration == null)
				{
					List<MemberInfo> list = new List<MemberInfo>();
					list.Add(new MemberInfo(MemberName.UniqueName, Token.String));
					list.Add(new MemberInfo(MemberName.Label, Token.String));
					list.Add(new MemberInfo(MemberName.DefIndex, Token.Int32));
					list.Add(new MemberInfo(MemberName.RecursiveLevel, Token.Int32));
					return new Declaration(ObjectType.MemberCell, ObjectType.CornerCell, list);
				}
				return m_declaration;
			}
		}

		internal abstract class PageMemberCell : IStorable, IPersistable
		{
			protected byte m_state;

			private ScalableList<PageMemberCell> m_children;

			private static Declaration m_declaration = GetDeclaration();

			internal bool HasOmittedChildren
			{
				get
				{
					return (m_state & 8) > 0;
				}
				set
				{
					if (value)
					{
						m_state |= 8;
					}
					else
					{
						m_state &= 247;
					}
				}
			}

			internal ScalableList<PageMemberCell> Children
			{
				get
				{
					return m_children;
				}
				set
				{
					m_children = value;
				}
			}

			internal abstract bool NeedWrite
			{
				get;
			}

			internal abstract int ColSpan
			{
				get;
			}

			internal abstract int RowSpan
			{
				get;
			}

			public virtual int Size => 1 + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(m_children);

			internal PageMemberCell()
			{
			}

			internal PageMemberCell(byte state)
			{
				m_state = state;
			}

			internal byte ResolveState()
			{
				byte b = 0;
				if (m_state > 1)
				{
					if ((m_state & 2) > 0)
					{
						b = 1;
						if ((m_state & 4) > 0)
						{
							b = (byte)(b | 2);
						}
					}
					if (m_children == null)
					{
						b = (byte)(b | 4);
					}
				}
				return b;
			}

			internal abstract void WriteItemToStream(TablixRegion region, RPLWriter rplWriter, int rowIndex, int colIndex);

			public virtual void Serialize(IntermediateFormatWriter writer)
			{
				writer.RegisterDeclaration(m_declaration);
				while (writer.NextMember())
				{
					switch (writer.CurrentMember.MemberName)
					{
					case MemberName.State:
						writer.Write(m_state);
						break;
					case MemberName.Children:
						writer.Write(m_children);
						break;
					default:
						RSTrace.RenderingTracer.Assert(condition: false);
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
					case MemberName.State:
						m_state = reader.ReadByte();
						break;
					case MemberName.Children:
						m_children = reader.ReadRIFObject<ScalableList<PageMemberCell>>();
						break;
					default:
						RSTrace.RenderingTracer.Assert(condition: false);
						break;
					}
				}
			}

			public void ResolveReferences(Dictionary<ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
			{
			}

			public virtual ObjectType GetObjectType()
			{
				return ObjectType.PageMemberCell;
			}

			internal static Declaration GetDeclaration()
			{
				if (m_declaration == null)
				{
					List<MemberInfo> list = new List<MemberInfo>();
					list.Add(new MemberInfo(MemberName.State, Token.Byte));
					list.Add(new MemberInfo(MemberName.Children, ObjectType.ScalableList, ObjectType.PageMemberCell));
					return new Declaration(ObjectType.PageMemberCell, ObjectType.None, list);
				}
				return m_declaration;
			}
		}

		internal class StreamMemberCell : PageMemberCell
		{
			private MemberCell m_memberCell;

			private static Declaration m_declaration = GetDeclaration();

			internal override int ColSpan => m_memberCell.ColSpan;

			internal override int RowSpan => m_memberCell.RowSpan;

			internal override bool NeedWrite
			{
				get
				{
					if (m_state > 0)
					{
						return true;
					}
					if (m_memberCell.GroupLabel != null)
					{
						return true;
					}
					return false;
				}
			}

			public override int Size => base.Size + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(m_memberCell);

			internal StreamMemberCell()
			{
			}

			internal StreamMemberCell(MemberCell cell, byte state)
				: base(state)
			{
				m_memberCell = cell;
			}

			internal override void WriteItemToStream(TablixRegion region, RPLWriter rplWriter, int rowIndex, int colIndex)
			{
				byte state = ResolveState();
				m_memberCell.WriteItemToStream(region, rplWriter, rowIndex, colIndex, state);
			}

			public override void Serialize(IntermediateFormatWriter writer)
			{
				base.Serialize(writer);
				writer.RegisterDeclaration(m_declaration);
				while (writer.NextMember())
				{
					MemberName memberName = writer.CurrentMember.MemberName;
					if (memberName == MemberName.MemberCell)
					{
						writer.Write(m_memberCell);
					}
					else
					{
						RSTrace.RenderingTracer.Assert(condition: false);
					}
				}
			}

			public override void Deserialize(IntermediateFormatReader reader)
			{
				base.Deserialize(reader);
				reader.RegisterDeclaration(m_declaration);
				while (reader.NextMember())
				{
					MemberName memberName = reader.CurrentMember.MemberName;
					if (memberName == MemberName.MemberCell)
					{
						m_memberCell = (MemberCell)reader.ReadRIFObject();
					}
					else
					{
						RSTrace.RenderingTracer.Assert(condition: false);
					}
				}
			}

			public override ObjectType GetObjectType()
			{
				return ObjectType.StreamMemberCell;
			}

			internal new static Declaration GetDeclaration()
			{
				if (m_declaration == null)
				{
					List<MemberInfo> list = new List<MemberInfo>();
					list.Add(new MemberInfo(MemberName.MemberCell, ObjectType.MemberCell));
					return new Declaration(ObjectType.StreamMemberCell, ObjectType.PageMemberCell, list);
				}
				return m_declaration;
			}
		}

		internal class RPLMemberCell : PageMemberCell
		{
			[StaticReference]
			private RPLTablixMemberCell m_memberCell;

			private static Declaration m_declaration = GetDeclaration();

			internal override int ColSpan => m_memberCell.ColSpan;

			internal override int RowSpan => m_memberCell.RowSpan;

			internal override bool NeedWrite
			{
				get
				{
					if (m_state > 0)
					{
						return true;
					}
					if (m_memberCell.GroupLabel != null)
					{
						return true;
					}
					return false;
				}
			}

			public override int Size => base.Size + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.ReferenceSize;

			internal RPLMemberCell()
			{
			}

			internal RPLMemberCell(RPLTablixMemberCell cell, byte state)
				: base(state)
			{
				m_memberCell = cell;
			}

			internal override void WriteItemToStream(TablixRegion region, RPLWriter rplWriter, int rowIndex, int colIndex)
			{
				m_memberCell.RowIndex = rowIndex;
				m_memberCell.ColIndex = colIndex;
				m_memberCell.State = ResolveState();
				if (m_memberCell.ColSpan == 0 || m_memberCell.RowSpan == 0)
				{
					rplWriter.TablixRow.AddOmittedHeader(m_memberCell);
					return;
				}
				rplWriter.TablixRow.SetHeaderStart();
				rplWriter.TablixRow.RowCells.Add(m_memberCell);
			}

			public override void Serialize(IntermediateFormatWriter writer)
			{
				base.Serialize(writer);
				IScalabilityCache scalabilityCache = writer.PersistenceHelper as IScalabilityCache;
				writer.RegisterDeclaration(m_declaration);
				while (writer.NextMember())
				{
					MemberName memberName = writer.CurrentMember.MemberName;
					if (memberName == MemberName.MemberCell)
					{
						int value = scalabilityCache.StoreStaticReference(m_memberCell);
						writer.Write(value);
					}
					else
					{
						RSTrace.RenderingTracer.Assert(condition: false);
					}
				}
			}

			public override void Deserialize(IntermediateFormatReader reader)
			{
				base.Deserialize(reader);
				IScalabilityCache scalabilityCache = reader.PersistenceHelper as IScalabilityCache;
				reader.RegisterDeclaration(m_declaration);
				while (reader.NextMember())
				{
					MemberName memberName = reader.CurrentMember.MemberName;
					if (memberName == MemberName.MemberCell)
					{
						int id = reader.ReadInt32();
						m_memberCell = (RPLTablixMemberCell)scalabilityCache.FetchStaticReference(id);
					}
					else
					{
						RSTrace.RenderingTracer.Assert(condition: false);
					}
				}
			}

			public override ObjectType GetObjectType()
			{
				return ObjectType.RPLMemberCell;
			}

			internal new static Declaration GetDeclaration()
			{
				if (m_declaration == null)
				{
					List<MemberInfo> list = new List<MemberInfo>();
					list.Add(new MemberInfo(MemberName.MemberCell, Token.Int32));
					return new Declaration(ObjectType.RPLMemberCell, ObjectType.PageMemberCell, list);
				}
				return m_declaration;
			}
		}

		internal const byte BorderHeader = 1;

		internal const byte HasToggle = 2;

		internal const byte CollapsedHeader = 4;

		internal const byte HasOmittedChildren = 8;

		private PageItem m_partialPageItem;

		private List<int> m_tablixCreateState;

		private int m_levelForRepeat;

		private bool m_ignoreTotalsOnLastLevel;

		private TablixRowCollection m_bodyRows;

		private double[] m_bodyRowsHeigths;

		private double[] m_bodyColWidths;

		private bool m_ignoreCellPageBreaks;

		private bool m_hasDetailCellsWithColSpan;

		private int m_rowMembersDepth;

		private int m_colMembersDepth;

		private int m_partialMemberLevel;

		private InnerToggleState m_toggleMemberState;

		private Hashtable m_memberIndexByLevel = new Hashtable();

		private string m_outermostKeepWithMemberPath;

		private bool m_repeatOnNewPageRegistered;

		protected override PageBreak PageBreak => ((Microsoft.ReportingServices.OnDemandReportRendering.Tablix)m_source).PageBreak;

		protected override string PageName => ((TablixInstance)m_source.Instance).PageName;

		internal Tablix(Microsoft.ReportingServices.OnDemandReportRendering.Tablix source, PageContext pageContext, bool createForRepeat)
			: base(source)
		{
			if (pageContext != null)
			{
				if (createForRepeat)
				{
					m_itemPageSizes = pageContext.GetSharedFromRepeatItemSizesElement(source, isPadded: false);
				}
				else
				{
					m_itemPageSizes = pageContext.GetSharedItemSizesElement(source, isPadded: false);
				}
			}
			else
			{
				m_itemPageSizes = new ItemSizes(source);
			}
			TablixColumnCollection columnCollection = source.Body.ColumnCollection;
			m_bodyColWidths = new double[columnCollection.Count];
			for (int i = 0; i < columnCollection.Count; i++)
			{
				m_bodyColWidths[i] = columnCollection[i].Width.ToMillimeters();
			}
			m_bodyRows = source.Body.RowCollection;
			m_bodyRowsHeigths = new double[m_bodyRows.Count];
			for (int j = 0; j < m_bodyRows.Count; j++)
			{
				m_bodyRowsHeigths[j] = m_bodyRows[j].Height.ToMillimeters();
				if (m_hasDetailCellsWithColSpan)
				{
					continue;
				}
				for (int k = 0; k < columnCollection.Count; k++)
				{
					TablixCell tablixCell = m_bodyRows[j][k];
					if (tablixCell != null && tablixCell.CellContents.ColSpan > 1)
					{
						m_hasDetailCellsWithColSpan = true;
						break;
					}
				}
			}
			m_ignoreCellPageBreaks = source.Body.IgnoreCellPageBreaks;
			m_rowMembersDepth = TablixMembersDepthTree(source.RowHierarchy.MemberCollection);
			m_colMembersDepth = TablixMembersDepthTree(source.ColumnHierarchy.MemberCollection);
			if (!pageContext.AddToggledItems)
			{
				m_toggleMemberState = GetInnerToggleStateForParent(source.RowHierarchy.MemberCollection);
			}
		}

		internal override void UpdateItem(PageItemHelper itemHelper)
		{
			if (itemHelper != null)
			{
				base.UpdateItem(itemHelper);
				PageTablixHelper pageTablixHelper = itemHelper as PageTablixHelper;
				RSTrace.RenderingTracer.Assert(pageTablixHelper != null, "This should be a tablix");
				m_levelForRepeat = pageTablixHelper.LevelForRepeat;
				m_tablixCreateState = PageItem.GetNewList(pageTablixHelper.TablixCreateState);
				m_ignoreTotalsOnLastLevel = pageTablixHelper.IgnoreTotalsOnLastLevel;
				SetTablixMembersInstanceIndex(pageTablixHelper.MembersInstanceIndex);
			}
		}

		private int TablixMembersDepthTree(TablixMemberCollection memberCollection)
		{
			if (memberCollection == null || memberCollection.Count == 0)
			{
				return 0;
			}
			int num = 0;
			for (int i = 0; i < memberCollection.Count; i++)
			{
				TablixMember tablixMember = memberCollection[i];
				int num2 = 1;
				if (tablixMember.TablixHeader != null)
				{
					num2 = (tablixMember.IsColumn ? tablixMember.TablixHeader.CellContents.RowSpan : tablixMember.TablixHeader.CellContents.ColSpan);
				}
				num = Math.Max(num, TablixMembersDepthTree(tablixMember.Children) + num2 - 1);
			}
			return num + 1;
		}

		private InnerToggleState GetInnerToggleStateForParent(TablixMemberCollection members)
		{
			if (members == null || members.Count == 0)
			{
				return null;
			}
			bool toggleState = false;
			List<InnerToggleState> list = new List<InnerToggleState>(members.Count);
			if (GetInnerToggleState(members, ref toggleState, list))
			{
				return new InnerToggleState(toggle: false, list);
			}
			if (toggleState)
			{
				return new InnerToggleState(toggle: true, null);
			}
			return null;
		}

		private InnerToggleState GetInnerToggleState(TablixMember rowMember)
		{
			return GetInnerToggleStateForParent(rowMember.Children);
		}

		private bool GetInnerToggleState(TablixMemberCollection members, ref bool toggleState, List<InnerToggleState> membersToggleState)
		{
			bool result = false;
			InnerToggleState innerToggleState = null;
			Visibility visibility = null;
			for (int i = 0; i < members.Count; i++)
			{
				visibility = members[i].Visibility;
				if (visibility != null && visibility.ToggleItem != null)
				{
					if (members[i].IsStatic)
					{
						toggleState = true;
					}
					membersToggleState.Add(null);
					continue;
				}
				if (members[i].IsTotal)
				{
					membersToggleState.Add(null);
					continue;
				}
				innerToggleState = GetInnerToggleState(members[i]);
				if (innerToggleState != null)
				{
					result = true;
				}
				membersToggleState.Add(innerToggleState);
			}
			return result;
		}

		private bool StaticDecendents(TablixMemberCollection children)
		{
			if (children == null || children.Count == 0)
			{
				return true;
			}
			bool flag = true;
			for (int i = 0; i < children.Count && flag; i++)
			{
				flag = (children[i].IsStatic && StaticDecendents(children[i].Children));
			}
			return flag;
		}

		private double CornerSize(TablixMemberCollection memberCollection)
		{
			double num = 0.0;
			if (memberCollection == null || memberCollection.Count == 0)
			{
				return num;
			}
			TablixHeader tablixHeader = memberCollection[0].TablixHeader;
			if (tablixHeader != null)
			{
				num = tablixHeader.Size.ToMillimeters();
			}
			return num + CornerSize(memberCollection[0].Children);
		}

		private void CreateDetailCell(Microsoft.ReportingServices.OnDemandReportRendering.Tablix tablix, TablixMember colMemberParent, int colGridIndex, TablixContext context)
		{
			TablixCell tablixCell = null;
			int memberCellIndex;
			if (context.PartialItemHelper != null)
			{
				memberCellIndex = colMemberParent.MemberCellIndex;
				tablixCell = m_bodyRows[context.RowMemberIndexCell][memberCellIndex];
				context.ColMemberIndexCell = memberCellIndex;
				if (tablixCell.CellContents.ColSpan == 1)
				{
					context.ColMemberIndexCell = -1;
				}
				m_partialPageItem = context.AddDetailCellFromState(tablixCell, m_ignoreCellPageBreaks);
			}
			if (m_partialPageItem != null)
			{
				context.StaticDetailRow = false;
				if (m_partialPageItem.ItemState == State.OnPagePBEnd)
				{
					context.AddDetailEmptyCell(colGridIndex, m_bodyColWidths[colMemberParent.MemberCellIndex], 0.0);
					m_partialPageItem = null;
				}
				else
				{
					m_partialPageItem.UpdateSizes(0.0, null, null);
					if (m_partialPageItem.ItemState == State.TopNextPage)
					{
						m_partialPageItem.ItemState = State.OnPage;
					}
					if (!context.CalculateDetailCell(m_partialPageItem, colGridIndex, collect: true))
					{
						m_partialPageItem = null;
					}
				}
				context.PartialItemHelper = null;
				return;
			}
			memberCellIndex = colMemberParent.MemberCellIndex;
			tablixCell = m_bodyRows[context.RowMemberIndexCell][memberCellIndex];
			double cellColDefWidth = 0.0;
			bool collect = true;
			if (m_ignoreCellPageBreaks)
			{
				cellColDefWidth = m_bodyColWidths[memberCellIndex];
			}
			if (tablixCell == null)
			{
				while (memberCellIndex > 0)
				{
					memberCellIndex--;
					tablixCell = m_bodyRows[context.RowMemberIndexCell][memberCellIndex];
					if (tablixCell != null)
					{
						break;
					}
				}
				collect = false;
			}
			if (context.ColMemberIndexCell >= 0 && memberCellIndex == context.ColMemberIndexCell)
			{
				context.UpdateDetailCell(cellColDefWidth);
				return;
			}
			context.DelayedCalculation();
			context.ColMemberIndexCell = memberCellIndex;
			if (tablixCell.CellContents.ColSpan == 1)
			{
				context.ColMemberIndexCell = -1;
			}
			bool partialItem = false;
			ItemOffset itemOffset = context.AddDetailCell(tablixCell, colGridIndex, cellColDefWidth, m_bodyRowsHeigths[context.RowMemberIndexCell], m_ignoreCellPageBreaks, collect, out partialItem);
			if (partialItem)
			{
				m_partialPageItem = (PageItem)itemOffset;
				context.StaticDetailRow = false;
			}
		}

		private int CreateColumnMemberChildren(Microsoft.ReportingServices.OnDemandReportRendering.Tablix tablix, TablixMember colMemberParent, int defTreeLevel, bool parentBorderHeader, int parentRowIndex, int parentColIndex, TablixContext context, bool createDetail, ref LevelInfo parentLevelInfo)
		{
			if (parentLevelInfo == null)
			{
				parentLevelInfo = new LevelInfo();
			}
			else
			{
				parentLevelInfo.SetDefaults();
			}
			TablixMemberCollection tablixMemberCollection = null;
			int num = 0;
			if (colMemberParent == null)
			{
				if (context.ColsBeforeRowHeaders == 0)
				{
					num = tablix.GroupsBeforeRowHeaders;
				}
				context.CreateCorner(tablix.Corner);
				tablixMemberCollection = tablix.ColumnHierarchy.MemberCollection;
				parentColIndex = context.HeaderRowColumns;
			}
			else
			{
				tablixMemberCollection = colMemberParent.Children;
			}
			if (tablixMemberCollection == null)
			{
				if (context.PageContext.CancelPage)
				{
					return 0;
				}
				if (context.IgnoreColumn)
				{
					parentLevelInfo.IgnoredRowsCols++;
					if (createDetail)
					{
						context.AddDetailCellToCurrentPage(tablix, colMemberParent.MemberCellIndex);
					}
					return 0;
				}
				if (createDetail)
				{
					CreateDetailCell(tablix, colMemberParent, parentColIndex, context);
				}
				return 1;
			}
			int num2 = parentColIndex;
			int num3 = 0;
			int num4 = 0;
			bool flag = true;
			LevelInfo parentLevelInfo2 = null;
			bool flag2 = true;
			int num5 = 0;
			bool flag3 = false;
			byte memberState = 0;
			int num6 = 0;
			bool hasRecursivePeer = false;
			TablixMember tablixMember = null;
			Visibility visibility = null;
			TablixMemberInstance tablixMemberInstance = null;
			TablixDynamicMemberInstance tablixDynamicMemberInstance = null;
			List<int> totalsIndex = null;
			bool flag4 = false;
			parentLevelInfo.HasVisibleStaticPeer = CheckForVisibleStaticPeer(tablixMemberCollection);
			for (int i = 0; i < tablixMemberCollection.Count; i++)
			{
				tablixMember = tablixMemberCollection[i];
				visibility = tablixMember.Visibility;
				flag3 = parentBorderHeader;
				if (context.PageContext.CancelPage)
				{
					break;
				}
				if (context.NoRows && tablixMember.HideIfNoRows)
				{
					context.AddMemberToCurrentPage(tablix, tablixMember, TablixRegion.ColumnHeader, createDetail);
				}
				else
				{
					if (context.AlwaysHiddenMember(tablix, tablixMember, visibility, TablixRegion.ColumnHeader, createDetail, ref parentLevelInfo))
					{
						continue;
					}
					flag = true;
					flag4 = false;
					tablixMemberInstance = tablixMember.Instance;
					if (tablixMember.Group != null)
					{
						hasRecursivePeer = tablixMember.Group.IsRecursive;
					}
					if (tablixMember.IsStatic)
					{
						num = 0;
						flag2 = context.EnterColMemberInstance(tablixMember, visibility, hasRecursivePeer, parentLevelInfo.HasVisibleStaticPeer, out memberState);
						if (tablixMember.IsTotal)
						{
							if (!flag2 && num2 <= num5 && num2 <= parentColIndex)
							{
								if (totalsIndex == null)
								{
									totalsIndex = new List<int>();
								}
								totalsIndex.Add(i);
								flag4 = true;
							}
						}
						else
						{
							if (!flag3)
							{
								flag3 = StaticDecendents(tablixMember.Children);
							}
							if (flag3)
							{
								memberState = (byte)(memberState | 1);
							}
						}
					}
					else
					{
						context.DelayedCalculation();
						context.ColMemberIndexCell = -1;
						num5 = num2;
						if (i > 0)
						{
							num = 0;
						}
						tablixDynamicMemberInstance = (TablixDynamicMemberInstance)tablixMemberInstance;
						tablixDynamicMemberInstance.ResetContext();
						flag = tablixDynamicMemberInstance.MoveNext();
						if (flag)
						{
							flag2 = context.EnterColMemberInstance(tablixMember, visibility, hasRecursivePeer: false, hasVisibleStaticPeer: false, out memberState);
						}
					}
					while (flag)
					{
						num6 = defTreeLevel;
						if (flag2)
						{
							num3 = 0;
							if (tablixMember.TablixHeader != null)
							{
								num3 = tablixMember.TablixHeader.CellContents.RowSpan;
								num6 -= num3;
							}
							else
							{
								num6--;
							}
							num4 = CreateColumnMemberChildren(tablix, tablixMember, num6, flag3, parentRowIndex + num3, num2, context, createDetail, ref parentLevelInfo2);
							if (num4 > 0)
							{
								PageMemberCell pageMemberCell = context.AddColMember(tablixMember, parentRowIndex, num2, num3, num4, memberState, defTreeLevel, parentLevelInfo2);
								if (pageMemberCell != null)
								{
									if (!parentLevelInfo2.OmittedList)
									{
										pageMemberCell.Children = parentLevelInfo2.MemberCells;
										if (parentLevelInfo2.OmittedMembersCells)
										{
											pageMemberCell.HasOmittedChildren = true;
										}
									}
									if (pageMemberCell.RowSpan == 0)
									{
										parentLevelInfo.OmittedMembersCells = true;
									}
									parentLevelInfo.AddMemberCell(pageMemberCell, pageMemberCell.RowSpan, defTreeLevel, i, m_bodyColWidths[tablixMember.MemberCellIndex], context.Cache);
								}
								num2 += num4;
								context.AddTotalsToCurrentPage(ref totalsIndex, tablixMemberCollection, TablixRegion.ColumnHeader, createDetail);
							}
							else if (parentLevelInfo2.IgnoredRowsCols > 0)
							{
								context.AddMemberHeaderToCurrentPage(tablixMember, TablixRegion.ColumnHeader, headerFooterEval: true);
								parentLevelInfo.IgnoredRowsCols += parentLevelInfo2.IgnoredRowsCols;
							}
							else
							{
								context.AddMemberHeaderToCurrentPage(tablixMember, TablixRegion.ColumnHeader, headerFooterEval: false);
							}
						}
						else if (!flag4)
						{
							context.AddMemberToCurrentPage(tablix, tablixMember, TablixRegion.ColumnHeader, createDetail);
						}
						context.LeaveColMemberInstance(tablixMember, visibility);
						if (tablixMember.IsStatic || context.PageContext.CancelPage)
						{
							flag = false;
						}
						else
						{
							context.DelayedCalculation();
							flag = tablixDynamicMemberInstance.MoveNext();
							context.ColMemberIndexCell = -1;
							if (num > 0)
							{
								context.ColsBeforeRowHeaders += num4;
								num--;
							}
							if (flag)
							{
								flag2 = context.EnterColMemberInstance(tablixMember, visibility, hasRecursivePeer: false, hasVisibleStaticPeer: false, out memberState);
							}
						}
						num4 = 0;
					}
					tablixDynamicMemberInstance = null;
				}
			}
			if (num2 <= parentColIndex)
			{
				num2 += CreateColumnMemberTotals(tablix, totalsIndex, tablixMemberCollection, defTreeLevel, parentBorderHeader, parentRowIndex, parentColIndex, context, createDetail, parentLevelInfo);
			}
			return num2 - parentColIndex;
		}

		private int CreateColumnMemberTotals(Microsoft.ReportingServices.OnDemandReportRendering.Tablix tablix, List<int> totals, TablixMemberCollection columnMembers, int defTreeLevel, bool parentBorderHeader, int parentRowIndex, int parentColIndex, TablixContext context, bool createDetail, LevelInfo parentLevelInfo)
		{
			if (totals == null || totals.Count == 0)
			{
				return 0;
			}
			double sizeForParent = 0.0;
			int num = int.MaxValue;
			bool flag = false;
			int num2 = 0;
			TablixMember tablixMember = null;
			if (parentRowIndex > 0)
			{
				for (int i = 0; i < totals.Count; i++)
				{
					num2 = totals[i];
					tablixMember = columnMembers[num2];
					if (tablixMember.TablixHeader != null && tablixMember.TablixHeader.CellContents.RowSpan < num)
					{
						num = tablixMember.TablixHeader.CellContents.RowSpan;
						sizeForParent = tablixMember.TablixHeader.Size.ToMillimeters();
						flag = true;
					}
				}
				if (flag)
				{
					parentLevelInfo.SpanForParent = num;
					parentLevelInfo.SizeForParent = sizeForParent;
				}
			}
			int num3 = parentColIndex;
			int num4 = 0;
			int num5 = 0;
			LevelInfo parentLevelInfo2 = null;
			int num6 = 0;
			for (int j = 0; j < totals.Count; j++)
			{
				num2 = totals[j];
				tablixMember = columnMembers[num2];
				if (context.PageContext.CancelPage)
				{
					break;
				}
				num6 = defTreeLevel;
				num4 = 0;
				if (tablixMember.TablixHeader != null)
				{
					num4 = tablixMember.TablixHeader.CellContents.RowSpan;
					num6 -= num4;
				}
				else
				{
					num6--;
				}
				num5 = CreateColumnMemberChildren(tablix, tablixMember, num6, parentBorderHeader, parentRowIndex + num4, num3, context, createDetail, ref parentLevelInfo2);
				if (num5 > 0)
				{
					PageMemberCell pageMemberCell = context.AddTotalColMember(tablixMember, parentRowIndex, num3, num4, num5, 0, defTreeLevel, parentLevelInfo, parentLevelInfo2);
					if (pageMemberCell != null)
					{
						if (!parentLevelInfo2.OmittedList)
						{
							pageMemberCell.Children = parentLevelInfo2.MemberCells;
							if (parentLevelInfo2.OmittedMembersCells)
							{
								pageMemberCell.HasOmittedChildren = true;
							}
						}
						if (pageMemberCell.RowSpan == 0)
						{
							parentLevelInfo.OmittedMembersCells = true;
						}
						parentLevelInfo.AddMemberCell(pageMemberCell, pageMemberCell.RowSpan, defTreeLevel, num2, m_bodyColWidths[tablixMember.MemberCellIndex], context.Cache);
					}
				}
				else if (parentLevelInfo2.IgnoredRowsCols > 0)
				{
					context.AddMemberHeaderToCurrentPage(tablixMember, TablixRegion.ColumnHeader, headerFooterEval: true);
					parentLevelInfo.IgnoredRowsCols += parentLevelInfo2.IgnoredRowsCols;
				}
				else
				{
					context.AddMemberHeaderToCurrentPage(tablixMember, TablixRegion.ColumnHeader, headerFooterEval: false);
				}
				num3 += num5;
			}
			return num3 - parentColIndex;
		}

		private int CheckKeepWithGroupUp(TablixMemberCollection rowMembers, int start, KeepWithGroup keepWith)
		{
			TablixMember tablixMember = null;
			while (start >= 0)
			{
				tablixMember = rowMembers[start];
				if (tablixMember.IsStatic && tablixMember.KeepWithGroup == keepWith)
				{
					start--;
					continue;
				}
				return start + 1;
			}
			return 0;
		}

		private int CheckKeepWithGroupDown(TablixMemberCollection rowMembers, int start, KeepWithGroup keepWith)
		{
			TablixMember tablixMember = null;
			while (start < rowMembers.Count)
			{
				tablixMember = rowMembers[start];
				if (tablixMember.IsStatic && tablixMember.KeepWithGroup == keepWith)
				{
					start++;
					continue;
				}
				return start - 1;
			}
			return start - 1;
		}

		private int CreateKeepWithRowMemberChildren(Microsoft.ReportingServices.OnDemandReportRendering.Tablix tablix, TablixMember rowMemberParent, InnerToggleState parentToggleState, int defTreeLevel, int parentRowIndex, int parentColIndex, int level, int start, int end, ref LevelInfo parentLevelInfo, TablixContext context)
		{
			if (start > end || context.PageContext.CancelPage)
			{
				return 0;
			}
			TablixMemberCollection tablixMemberCollection = null;
			tablixMemberCollection = ((rowMemberParent != null) ? rowMemberParent.Children : tablix.RowHierarchy.MemberCollection);
			if (tablixMemberCollection == null)
			{
				if (context.PageContext.CancelPage)
				{
					return 0;
				}
				context.RowMemberIndexCell = rowMemberParent.MemberCellIndex;
				if (!context.IgnoreRow)
				{
					LevelInfo parentLevelInfo2 = null;
					bool keepTogether = context.PageContext.KeepTogether;
					context.PageContext.KeepTogether = true;
					CreateColumnMemberChildren(tablix, null, m_colMembersDepth, parentBorderHeader: false, 0, 0, context, createDetail: true, ref parentLevelInfo2);
					context.PageContext.KeepTogether = keepTogether;
					if (!context.ColumnHeadersCreated)
					{
						context.OuterColumnHeaders = parentLevelInfo2.MemberCells;
						context.OmittedOuterColumnHeaders = parentLevelInfo2.OmittedMembersCells;
						context.ColumnHeadersCreated = true;
					}
					context.WriteDetailRow(parentRowIndex, m_bodyRowsHeigths, m_ignoreCellPageBreaks);
				}
				else
				{
					context.AddDetailRowToCurrentPage(tablix);
					parentLevelInfo.IgnoredRowsCols++;
				}
				context.NextRow(m_bodyRowsHeigths);
				if (context.PageContext.TracingEnabled && context.TablixBottom > context.PageContext.PageHeight)
				{
					TracePageGrownOnKeepWithMember(context.PageContext.PageNumber);
				}
				if (!context.IgnoreRow)
				{
					return 1;
				}
				return 0;
			}
			int num = parentRowIndex;
			int num2 = 0;
			int num3 = 0;
			LevelInfo levelInfo = null;
			TablixMember tablixMember = null;
			Visibility visibility = null;
			bool advanceRow = false;
			byte memberState = 0;
			int num4 = 0;
			bool hasRecursivePeer = false;
			InnerToggleState innerToggleState = null;
			if (start == -1 && end == -1)
			{
				start = 0;
				end = tablixMemberCollection.Count - 1;
			}
			for (int i = start; i <= end; i++)
			{
				tablixMember = tablixMemberCollection[i];
				visibility = tablixMember.Visibility;
				if (context.PageContext.CancelPage)
				{
					break;
				}
				if (context.AlwaysHiddenMember(tablix, tablixMember, visibility, TablixRegion.RowHeader, createDetail: true, ref parentLevelInfo))
				{
					continue;
				}
				if (context.PageContext.TracingEnabled)
				{
					RegisterTablixMemberIndex(tablixMember.DefinitionPath, i);
				}
				if (parentToggleState != null && parentToggleState.Children != null)
				{
					innerToggleState = parentToggleState.Children[i];
				}
				if (tablixMember.Group != null)
				{
					hasRecursivePeer = tablixMember.Group.IsRecursive;
				}
				bool num5 = context.EnterRowMember(this, tablixMember, visibility, innerToggleState, hasRecursivePeer, parentLevelInfo.HasVisibleStaticPeer, out memberState);
				memberState = (byte)(memberState | 1);
				num4 = defTreeLevel;
				if (num5)
				{
					num2 = 0;
					if (tablixMember.TablixHeader != null)
					{
						num2 = tablixMember.TablixHeader.CellContents.ColSpan;
						num4 -= num2;
					}
					else
					{
						num4--;
					}
					bool num6 = context.PageContext.TracingEnabled && RegisterOutermostKeepWithMember(tablixMember);
					bool flag = context.PageContext.TracingEnabled && tablixMember.RepeatOnNewPage && RegisterOutermostRepeatOnNewPage();
					levelInfo = new LevelInfo();
					num3 = CreateKeepWithRowMemberChildren(tablix, tablixMember, innerToggleState, num4, num, parentColIndex + num2, level + 1, -1, -1, ref levelInfo, context);
					if (num3 > 0)
					{
						if (context.PageContext.TracingEnabled && flag)
						{
							TraceRepeatOnNewPage(context.PageContext.PageNumber, tablixMember);
						}
						PageMemberCell pageMemberCell = context.AddRowMember(tablixMember, num, parentColIndex, num3, num2, memberState, defTreeLevel, null);
						if (pageMemberCell != null)
						{
							if (!levelInfo.OmittedList)
							{
								pageMemberCell.Children = levelInfo.MemberCells;
							}
							parentLevelInfo.AddMemberCell(pageMemberCell, pageMemberCell.ColSpan, defTreeLevel, i, m_bodyRowsHeigths[tablixMember.MemberCellIndex], context.Cache);
						}
						num += num3;
					}
					else if (levelInfo.IgnoredRowsCols > 0)
					{
						context.AddMemberHeaderToCurrentPage(tablixMember, TablixRegion.RowHeader, headerFooterEval: true);
						parentLevelInfo.IgnoredRowsCols += levelInfo.IgnoredRowsCols;
					}
					else
					{
						context.AddMemberHeaderToCurrentPage(tablixMember, TablixRegion.RowHeader, headerFooterEval: false);
					}
					if (num6)
					{
						UnregisterOutermostKeepWithMember();
					}
					if (flag)
					{
						UnregisterRepeatOnNewPagePath();
					}
				}
				else
				{
					context.AddMemberToCurrentPage(tablix, tablixMember, TablixRegion.RowHeader, createDetail: true);
				}
				context.LeaveRowMemberInstance(tablixMember, innerToggleState, visibility, hasRecursivePeer);
				context.LeaveRowMember(tablixMember, m_bodyRowsHeigths, visibility, hasRecursivePeer, innerToggleState, ref advanceRow);
			}
			return num - parentRowIndex;
		}

		private int CreateRowMemberChildren(Microsoft.ReportingServices.OnDemandReportRendering.Tablix tablix, TablixMember rowMemberParent, InnerToggleState parentToggleState, int defTreeLevel, bool parentBorderHeader, int parentRowIndex, int parentColIndex, int level, TablixContext context, ref bool advanceRow, ref LevelInfo parentLevelInfo, ref List<bool> ignoreTotals, bool keepTogether)
		{
			bool flag = false;
			if (parentLevelInfo == null)
			{
				parentLevelInfo = new LevelInfo();
			}
			else
			{
				parentLevelInfo.SetDefaults();
			}
			TablixMemberCollection tablixMemberCollection = null;
			tablixMemberCollection = ((rowMemberParent != null) ? rowMemberParent.Children : tablix.RowHierarchy.MemberCollection);
			if (tablixMemberCollection == null)
			{
				if (context.PageContext.CancelPage)
				{
					advanceRow = false;
					return 0;
				}
				context.RowMemberIndexCell = rowMemberParent.MemberCellIndex;
				if (!context.IgnoreRow)
				{
					LevelInfo parentLevelInfo2 = null;
					bool keepTogether2 = context.PageContext.KeepTogether;
					context.PageContext.KeepTogether = (keepTogether || keepTogether2);
					CreateColumnMemberChildren(tablix, null, m_colMembersDepth, parentBorderHeader: false, 0, 0, context, createDetail: true, ref parentLevelInfo2);
					context.PageContext.KeepTogether = keepTogether2;
					if (!context.ColumnHeadersCreated)
					{
						context.OuterColumnHeaders = parentLevelInfo2.MemberCells;
						context.OmittedOuterColumnHeaders = parentLevelInfo2.OmittedMembersCells;
						context.ColumnHeadersCreated = true;
					}
					context.WriteDetailRow(parentRowIndex, m_bodyRowsHeigths, m_ignoreCellPageBreaks);
				}
				else
				{
					context.AddDetailRowToCurrentPage(tablix);
					parentLevelInfo.IgnoredRowsCols++;
				}
				advanceRow = context.AdvanceRow(m_bodyRowsHeigths, m_tablixCreateState, level);
				if (!context.IgnoreRow)
				{
					return 1;
				}
				return 0;
			}
			int num = parentRowIndex;
			int num2 = 0;
			int num3 = 0;
			bool flag2 = true;
			LevelInfo parentLevelInfo3 = null;
			bool flag3 = true;
			int num4 = -1;
			int num5 = 0;
			bool flag4 = true;
			bool flag5 = false;
			double num6 = 0.0;
			bool flag6 = false;
			byte memberState = 0;
			int num7 = 0;
			bool flag7 = false;
			bool hasRecursivePeer = false;
			TablixMember tablixMember = null;
			Visibility visibility = null;
			TablixMemberInstance tablixMemberInstance = null;
			TablixDynamicMemberInstance tablixDynamicMemberInstance = null;
			List<int> totalsIndex = null;
			bool flag8 = false;
			bool flag9 = true;
			InnerToggleState innerToggleState = null;
			parentLevelInfo.HasVisibleStaticPeer = CheckForVisibleStaticPeer(tablixMemberCollection);
			if (m_tablixCreateState == null || m_tablixCreateState.Count <= level)
			{
				if (m_tablixCreateState == null)
				{
					m_tablixCreateState = new List<int>();
				}
				m_tablixCreateState.Add(num5);
				if (ignoreTotals == null)
				{
					ignoreTotals = new List<bool>();
				}
				ignoreTotals.Add(item: false);
			}
			else
			{
				num5 = m_tablixCreateState[level];
				if (num5 < 0)
				{
					num5 = -num5;
					m_tablixCreateState[level] = num5;
				}
				else
				{
					flag4 = false;
					if (level <= m_partialMemberLevel)
					{
						flag9 = false;
					}
				}
			}
			for (int i = num5; i < tablixMemberCollection.Count; i++)
			{
				tablixMember = tablixMemberCollection[i];
				visibility = tablixMember.Visibility;
				flag6 = parentBorderHeader;
				if (context.PageContext.TracingEnabled)
				{
					RegisterTablixMemberIndex(tablixMember.DefinitionPath, i);
				}
				if (context.PageContext.CancelPage)
				{
					advanceRow = false;
					break;
				}
				if (context.NoRows && tablixMember.HideIfNoRows)
				{
					context.AddMemberToCurrentPage(tablix, tablixMember, TablixRegion.RowHeader, createDetail: true);
				}
				else
				{
					if (context.AlwaysHiddenMember(tablix, tablixMember, visibility, TablixRegion.RowHeader, createDetail: true, ref parentLevelInfo))
					{
						continue;
					}
					flag5 = false;
					flag3 = true;
					flag2 = true;
					flag8 = false;
					num6 = 0.0;
					tablixMemberInstance = tablixMember.Instance;
					if (parentToggleState != null && parentToggleState.Children != null)
					{
						innerToggleState = parentToggleState.Children[i];
					}
					if (tablixMember.Group != null)
					{
						hasRecursivePeer = tablixMember.Group.IsRecursive;
					}
					if (tablixMember.IsStatic)
					{
						context.SharedLayoutRow = 0;
						if (!advanceRow && !tablixMember.IsTotal && !context.IsTotal)
						{
							if (num4 >= 0 && (num > num4 || flag7))
							{
								int num8 = CheckKeepWithGroupDown(tablixMemberCollection, i, KeepWithGroup.Before);
								if (num8 >= i)
								{
									context.RepeatWith = true;
									num += CreateKeepWithRowMemberChildren(tablix, rowMemberParent, parentToggleState, defTreeLevel, num, parentColIndex, level, i, num8, ref parentLevelInfo, context);
									context.RepeatWith = false;
									i = num8;
									num4 = -1;
									continue;
								}
							}
							m_tablixCreateState[level] = i;
							return num - parentRowIndex;
						}
						flag3 = context.EnterRowMember(this, tablixMember, visibility, innerToggleState, hasRecursivePeer, parentLevelInfo.HasVisibleStaticPeer, out memberState);
						if (tablixMember.IsTotal)
						{
							if (!flag3 && num <= num4 && num <= parentRowIndex && !ignoreTotals[level])
							{
								if (totalsIndex == null)
								{
									totalsIndex = new List<int>();
								}
								totalsIndex.Add(i);
								flag8 = true;
							}
						}
						else
						{
							if (!flag6)
							{
								flag6 = StaticDecendents(tablixMember.Children);
							}
							if (flag6)
							{
								memberState = (byte)(memberState | 1);
							}
						}
						if (flag3)
						{
							if (KeepTogetherStaticHeader(tablixMemberCollection, tablixMember, i, context))
							{
								context.RegisterKeepWith();
							}
							else if (num4 >= 0 && (num > num4 || flag7) && tablixMember.KeepWithGroup == KeepWithGroup.Before)
							{
								context.RegisterKeepWith();
							}
						}
						num4 = -1;
					}
					else
					{
						context.SharedLayoutRow = 2;
						num4 = num;
						tablixDynamicMemberInstance = (TablixDynamicMemberInstance)tablixMemberInstance;
						if (flag4 || i > num5)
						{
							tablixDynamicMemberInstance.ResetContext();
							flag2 = tablixDynamicMemberInstance.MoveNext();
							flag5 = true;
							if (flag2)
							{
								flag2 = CheckAndAdvanceToNextVisibleInstance(tablix, tablixMember, tablixDynamicMemberInstance, context);
							}
							flag9 = true;
						}
						else if (level <= m_levelForRepeat)
						{
							int num9 = CheckKeepWithGroupUp(tablixMemberCollection, num5 - 1, KeepWithGroup.After);
							if (num9 < num5 && tablixMemberCollection[num9].RepeatOnNewPage)
							{
								context.RepeatWith = true;
								num += CreateKeepWithRowMemberChildren(tablix, rowMemberParent, parentToggleState, defTreeLevel, num, parentColIndex, level, num9, num5 - 1, ref parentLevelInfo, context);
								context.RepeatWith = false;
							}
						}
						if (!advanceRow)
						{
							if (flag2)
							{
								m_tablixCreateState[level] = i;
								return num - parentRowIndex;
							}
							continue;
						}
						if (flag2)
						{
							flag3 = context.EnterRowMember(this, tablixMember, visibility, innerToggleState, hasRecursivePeer, hasVisibleStaticPeer: false, out memberState);
						}
						if (flag3)
						{
							int num10 = CheckKeepWithGroupDown(tablixMemberCollection, i + 1, KeepWithGroup.Before);
							if (num10 > i && tablixMemberCollection[num10].RepeatOnNewPage)
							{
								context.RepeatWith = true;
								num6 = RegisterSizeForRepeatWithBefore(tablix, rowMemberParent, i + 1, num10, context);
								context.RepeatWith = false;
							}
						}
					}
					flag7 = false;
					while (flag2)
					{
						num7 = defTreeLevel;
						if (flag3)
						{
							if (!tablixMember.IsStatic && flag5 && context.CheckPageBreaks && (num > parentRowIndex || parentRowIndex > 0))
							{
								PageBreakInfo pageBreakInfo = CreatePageBreakInfo(tablixMember.Group);
								if (pageBreakInfo != null && !pageBreakInfo.Disabled)
								{
									PageBreakLocation breakLocation = pageBreakInfo.BreakLocation;
									if ((breakLocation == PageBreakLocation.Start || breakLocation == PageBreakLocation.StartAndEnd) && DynamicWithVisibleChildren(tablixMember, context))
									{
										advanceRow = false;
										m_tablixCreateState[level] = i;
										context.PageContext.RegisterPageBreak(pageBreakInfo, context.PageBreakNeedsOverride);
										return num - parentRowIndex;
									}
								}
							}
							num3 = 0;
							if (tablixMember.TablixHeader != null)
							{
								num3 = tablixMember.TablixHeader.CellContents.ColSpan;
								num7 -= num3;
							}
							else
							{
								num7--;
							}
							bool overrideChild = !context.PageContext.IsPageNameRegistered;
							num2 = CreateRowMemberChildren(tablix, tablixMember, innerToggleState, num7, flag6, num, parentColIndex + num3, level + 1, context, ref advanceRow, ref parentLevelInfo3, ref ignoreTotals, keepTogether || tablixMember.KeepTogether);
							if (!advanceRow && keepTogether && !context.PageContext.IsPageBreakRegistered)
							{
								if (context.PageContext.TracingEnabled && !flag && rowMemberParent != null && rowMemberParent.KeepTogether)
								{
									TracePageGrownOnKeepTogetherMember(context.PageContext.PageNumber, rowMemberParent);
								}
								flag = true;
								advanceRow = true;
							}
							if (num2 > 0)
							{
								flag5 = false;
								PageMemberCell pageMemberCell = context.AddRowMember(tablixMember, num, parentColIndex, num2, num3, memberState, defTreeLevel, parentLevelInfo3);
								if (flag9 && tablixMember.Group != null && context.CheckPageBreaks)
								{
									context.PageContext.RegisterPageName(tablixMember.Group.Instance.PageName, overrideChild);
								}
								if (pageMemberCell != null)
								{
									if (!parentLevelInfo3.OmittedList)
									{
										pageMemberCell.Children = parentLevelInfo3.MemberCells;
									}
									parentLevelInfo.AddMemberCell(pageMemberCell, pageMemberCell.ColSpan, defTreeLevel, i, m_bodyRowsHeigths[tablixMember.MemberCellIndex], context.Cache);
								}
								num += num2;
								context.AddTotalsToCurrentPage(ref totalsIndex, tablixMemberCollection, TablixRegion.RowHeader, createDetail: true);
								ignoreTotals[level] = true;
							}
							else if (parentLevelInfo3.IgnoredRowsCols > 0)
							{
								context.AddMemberHeaderToCurrentPage(tablixMember, TablixRegion.RowHeader, headerFooterEval: true);
								parentLevelInfo.IgnoredRowsCols += parentLevelInfo3.IgnoredRowsCols;
								flag7 = true;
							}
							else
							{
								context.AddMemberHeaderToCurrentPage(tablixMember, TablixRegion.RowHeader, headerFooterEval: false);
							}
						}
						else if (!flag8)
						{
							context.AddMemberToCurrentPage(tablix, tablixMember, TablixRegion.RowHeader, createDetail: true);
						}
						context.LeaveRowMemberInstance(tablixMember, innerToggleState, visibility, hasRecursivePeer);
						flag9 = true;
						if (tablixMember.IsStatic)
						{
							flag2 = false;
							context.LeaveRowMember(tablixMember, m_bodyRowsHeigths, visibility, hasRecursivePeer, innerToggleState, ref advanceRow);
							if (!advanceRow && level + 1 < m_tablixCreateState.Count)
							{
								m_tablixCreateState[level] = i;
								return num - parentRowIndex;
							}
							if (context.KeepWith)
							{
								int num11 = CheckKeepWithGroupDown(tablixMemberCollection, i + 1, tablixMember.KeepWithGroup);
								if (num11 > i)
								{
									num += CreateKeepWithRowMemberChildren(tablix, rowMemberParent, parentToggleState, defTreeLevel, num, parentColIndex, level, i + 1, num11, ref parentLevelInfo, context);
									i = num11;
								}
								context.UnRegisterKeepWith(tablixMember.KeepWithGroup, m_bodyRowsHeigths, ref advanceRow);
							}
							continue;
						}
						if (advanceRow)
						{
							PageBreakInfo pageBreakInfo2 = CreatePageBreakInfo(tablixMember.Group);
							PageBreakLocation pageBreakLocation = (pageBreakInfo2 != null && !pageBreakInfo2.Disabled) ? pageBreakInfo2.BreakLocation : PageBreakLocation.None;
							flag2 = AdvanceToNextVisibleInstance(tablix, tablixMember, tablixDynamicMemberInstance, context);
							if (context.CheckPageBreaks && num > num4)
							{
								PageBreakInfo pageBreakInfo3 = CreatePageBreakInfo(tablixMember.Group);
								PageBreakLocation pageBreakLocation2 = (pageBreakInfo3 != null && !pageBreakInfo3.Disabled) ? pageBreakInfo3.BreakLocation : PageBreakLocation.None;
								if (flag2)
								{
									if (pageBreakLocation == PageBreakLocation.StartAndEnd || pageBreakLocation == PageBreakLocation.End)
									{
										advanceRow = false;
										num4 = -1;
										int num12 = CheckKeepWithGroupDown(tablixMemberCollection, i + 1, KeepWithGroup.Before);
										if (num12 > i && tablixMemberCollection[num12].RepeatOnNewPage)
										{
											context.IgnoreHeight = true;
											num += CreateKeepWithRowMemberChildren(tablix, rowMemberParent, parentToggleState, defTreeLevel, num, parentColIndex, level, i + 1, num12, ref parentLevelInfo, context);
											context.IgnoreHeight = false;
										}
										context.PageContext.RegisterPageBreak(pageBreakInfo2, context.PageBreakNeedsOverride);
										m_tablixCreateState[level] = i;
										return num - parentRowIndex;
									}
									if ((pageBreakLocation2 == PageBreakLocation.Between || pageBreakLocation2 == PageBreakLocation.Start) && DynamicWithVisibleChildren(tablixMember, context))
									{
										advanceRow = false;
										num4 = -1;
										int num13 = CheckKeepWithGroupDown(tablixMemberCollection, i + 1, KeepWithGroup.Before);
										if (num13 > i && tablixMemberCollection[num13].RepeatOnNewPage)
										{
											context.IgnoreHeight = true;
											num += CreateKeepWithRowMemberChildren(tablix, rowMemberParent, parentToggleState, defTreeLevel, num, parentColIndex, level, i + 1, num13, ref parentLevelInfo, context);
											context.IgnoreHeight = false;
										}
										context.PageContext.RegisterPageBreak(pageBreakInfo3, context.PageBreakNeedsOverride);
										m_tablixCreateState[level] = i;
										return num - parentRowIndex;
									}
								}
								else if (pageBreakLocation == PageBreakLocation.End || pageBreakLocation == PageBreakLocation.StartAndEnd)
								{
									advanceRow = false;
									context.PageBreakAtEnd = true;
									num4 = -1;
									int num14 = CheckKeepWithGroupDown(tablixMemberCollection, i + 1, KeepWithGroup.Before);
									if (num14 > i && tablixMemberCollection[num14].RepeatOnNewPage)
									{
										context.IgnoreHeight = true;
										num += CreateKeepWithRowMemberChildren(tablix, rowMemberParent, parentToggleState, defTreeLevel, num, parentColIndex, level, i + 1, num14, ref parentLevelInfo, context);
										context.IgnoreHeight = false;
									}
									context.PageContext.RegisterPageBreak(pageBreakInfo2, context.PageBreakNeedsOverride);
								}
								if (context.PropagatedPageBreak)
								{
									advanceRow = false;
									context.PageBreakAtEnd = true;
									num4 = -1;
									int num15 = CheckKeepWithGroupDown(tablixMemberCollection, i + 1, KeepWithGroup.Before);
									if (num15 > i && tablixMemberCollection[num15].RepeatOnNewPage)
									{
										context.IgnoreHeight = true;
										num += CreateKeepWithRowMemberChildren(tablix, rowMemberParent, parentToggleState, defTreeLevel, num, parentColIndex, level, i + 1, num15, ref parentLevelInfo, context);
										context.IgnoreHeight = false;
									}
									if (flag2)
									{
										m_tablixCreateState[level] = i;
										return num - parentRowIndex;
									}
								}
							}
							if (flag2)
							{
								flag3 = context.EnterRowMemberInstance(this, tablixMember, m_bodyRowsHeigths, visibility, hasRecursivePeer, innerToggleState, out memberState, ref advanceRow);
							}
							else
							{
								context.LeaveRowMember(tablixMember, m_bodyRowsHeigths, num6, visibility, hasRecursivePeer, innerToggleState, ref advanceRow);
							}
							continue;
						}
						if (level + 1 < m_tablixCreateState.Count)
						{
							m_tablixCreateState[level] = i;
							int num16 = CheckKeepWithGroupDown(tablixMemberCollection, i + 1, KeepWithGroup.Before);
							if (num16 > i && tablixMemberCollection[num16].RepeatOnNewPage)
							{
								context.IgnoreHeight = true;
								num += CreateKeepWithRowMemberChildren(tablix, rowMemberParent, parentToggleState, defTreeLevel, num, parentColIndex, level, i + 1, num16, ref parentLevelInfo, context);
								context.IgnoreHeight = false;
							}
							return num - parentRowIndex;
						}
						bool flag10 = false;
						PageBreakInfo pageBreakInfo4 = CreatePageBreakInfo(tablixMember.Group);
						if (pageBreakInfo4 != null && !pageBreakInfo4.Disabled)
						{
							PageBreakLocation breakLocation2 = pageBreakInfo4.BreakLocation;
							if (breakLocation2 == PageBreakLocation.StartAndEnd || breakLocation2 == PageBreakLocation.End)
							{
								flag10 = true;
								context.PageContext.RegisterPageBreak(pageBreakInfo4, context.PageBreakNeedsOverride);
							}
						}
						flag2 = AdvanceToNextVisibleInstance(tablix, tablixMember, tablixDynamicMemberInstance, context);
						if (flag2)
						{
							m_tablixCreateState[level] = i;
							int num17 = CheckKeepWithGroupDown(tablixMemberCollection, i + 1, KeepWithGroup.Before);
							if (num17 > i && tablixMemberCollection[num17].RepeatOnNewPage)
							{
								context.IgnoreHeight = true;
								num += CreateKeepWithRowMemberChildren(tablix, rowMemberParent, parentToggleState, defTreeLevel, num, parentColIndex, level, i + 1, num17, ref parentLevelInfo, context);
								context.IgnoreHeight = false;
							}
							pageBreakInfo4 = CreatePageBreakInfo(tablixMember.Group);
							if (pageBreakInfo4 != null && !pageBreakInfo4.Disabled)
							{
								PageBreakLocation breakLocation3 = pageBreakInfo4.BreakLocation;
								if (breakLocation3 == PageBreakLocation.Start || breakLocation3 == PageBreakLocation.Between)
								{
									context.PageContext.RegisterPageBreak(pageBreakInfo4, context.PageBreakNeedsOverride);
								}
							}
							return num - parentRowIndex;
						}
						if (flag10)
						{
							num4 = -1;
							context.PageBreakAtEnd = true;
							int num18 = CheckKeepWithGroupDown(tablixMemberCollection, i + 1, KeepWithGroup.Before);
							if (num18 > i && tablixMemberCollection[num18].RepeatOnNewPage)
							{
								context.IgnoreHeight = true;
								num += CreateKeepWithRowMemberChildren(tablix, rowMemberParent, parentToggleState, defTreeLevel, num, parentColIndex, level, i + 1, num18, ref parentLevelInfo, context);
								context.IgnoreHeight = false;
							}
						}
						context.LeaveRowMember(tablixMember, m_bodyRowsHeigths, num6, visibility, hasRecursivePeer, innerToggleState, ref advanceRow);
					}
					tablixDynamicMemberInstance = null;
				}
			}
			if (num <= parentRowIndex)
			{
				num += CreateRowMemberTotals(tablix, totalsIndex, tablixMemberCollection, defTreeLevel, parentBorderHeader, parentRowIndex, parentColIndex, level, context, ref advanceRow, parentLevelInfo, ignoreTotals, keepTogether);
			}
			m_tablixCreateState.RemoveAt(level);
			ignoreTotals.RemoveAt(level);
			if (keepTogether && flag)
			{
				advanceRow = false;
			}
			return num - parentRowIndex;
		}

		private int CreateRowMemberTotals(Microsoft.ReportingServices.OnDemandReportRendering.Tablix tablix, List<int> totals, TablixMemberCollection rowMembers, int defTreeLevel, bool parentBorderHeader, int parentRowIndex, int parentColIndex, int level, TablixContext context, ref bool advanceRow, LevelInfo parentLevelInfo, List<bool> ignoreTotals, bool parentKeepTogether)
		{
			if (totals == null || totals.Count == 0)
			{
				return 0;
			}
			double sizeForParent = 0.0;
			int num = int.MaxValue;
			bool flag = false;
			int num2 = 0;
			TablixMember tablixMember = null;
			if (parentColIndex > 0)
			{
				for (int i = 0; i < totals.Count; i++)
				{
					num2 = totals[i];
					tablixMember = rowMembers[num2];
					if (tablixMember.TablixHeader != null && tablixMember.TablixHeader.CellContents.ColSpan < num)
					{
						num = tablixMember.TablixHeader.CellContents.ColSpan;
						sizeForParent = tablixMember.TablixHeader.Size.ToMillimeters();
						flag = true;
					}
				}
				if (flag)
				{
					parentLevelInfo.SpanForParent = num;
					parentLevelInfo.SizeForParent = sizeForParent;
				}
			}
			int num3 = parentRowIndex;
			int num4 = 0;
			int num5 = 0;
			LevelInfo parentLevelInfo2 = null;
			int num6 = 0;
			context.EnterTotalRowMember();
			for (int j = 0; j < totals.Count; j++)
			{
				num2 = totals[j];
				tablixMember = rowMembers[num2];
				if (context.PageContext.CancelPage)
				{
					advanceRow = false;
					break;
				}
				context.SharedLayoutRow = 0;
				num6 = defTreeLevel;
				num5 = 0;
				if (tablixMember.TablixHeader != null)
				{
					num5 = tablixMember.TablixHeader.CellContents.ColSpan;
					num6 -= num5;
				}
				else
				{
					num6--;
				}
				bool advanceRow2 = true;
				num4 = CreateRowMemberChildren(tablix, tablixMember, null, num6, parentBorderHeader, num3, parentColIndex + num5, level + 1, context, ref advanceRow2, ref parentLevelInfo2, ref ignoreTotals, parentKeepTogether || tablixMember.KeepTogether);
				if (num4 > 0)
				{
					PageMemberCell pageMemberCell = context.AddTotalRowMember(tablixMember, num3, parentColIndex, num4, num5, 0, defTreeLevel, parentLevelInfo, parentLevelInfo2);
					if (pageMemberCell != null)
					{
						if (!parentLevelInfo2.OmittedList)
						{
							pageMemberCell.Children = parentLevelInfo2.MemberCells;
						}
						parentLevelInfo.AddMemberCell(pageMemberCell, pageMemberCell.ColSpan, defTreeLevel, num2, m_bodyRowsHeigths[tablixMember.MemberCellIndex], context.Cache);
					}
					num3 += num4;
				}
				else if (parentLevelInfo2.IgnoredRowsCols > 0)
				{
					context.AddMemberHeaderToCurrentPage(tablixMember, TablixRegion.RowHeader, headerFooterEval: true);
					parentLevelInfo.IgnoredRowsCols += parentLevelInfo2.IgnoredRowsCols;
				}
				else
				{
					context.AddMemberHeaderToCurrentPage(tablixMember, TablixRegion.ColumnHeader, headerFooterEval: false);
				}
			}
			context.LeaveTotalRowMember(m_bodyRowsHeigths, ref advanceRow);
			return num3 - parentRowIndex;
		}

		private bool RenderRowMemberInstance(TablixMember rowMember, bool addToggledItems)
		{
			if (rowMember.IsTotal)
			{
				return addToggledItems;
			}
			Visibility visibility = rowMember.Visibility;
			if (visibility == null)
			{
				return true;
			}
			if (visibility.HiddenState == SharedHiddenState.Never)
			{
				return true;
			}
			if (visibility.ToggleItem != null && addToggledItems)
			{
				return true;
			}
			return !rowMember.Instance.Visibility.CurrentlyHidden;
		}

		private bool KeepTogetherStaticHeader(TablixMemberCollection rowMembers, TablixMember staticMember, int staticIndex, TablixContext context)
		{
			if (staticMember.KeepWithGroup != KeepWithGroup.After)
			{
				return false;
			}
			int num = CheckKeepWithGroupDown(rowMembers, staticIndex + 1, KeepWithGroup.After);
			num++;
			if (num < rowMembers.Count)
			{
				TablixMember tablixMember = rowMembers[num];
				TablixDynamicMemberInstance tablixDynamicMemberInstance = tablixMember.Instance as TablixDynamicMemberInstance;
				if (tablixDynamicMemberInstance == null)
				{
					return false;
				}
				if (tablixMember.Visibility != null)
				{
					if (tablixMember.Visibility.HiddenState == SharedHiddenState.Always)
					{
						return false;
					}
					if (tablixMember.Visibility.ToggleItem != null && num + 1 < rowMembers.Count)
					{
						return true;
					}
				}
				PageBreakInfo pageBreakAtStart = null;
				bool flag = false;
				tablixDynamicMemberInstance.ResetContext();
				bool flag2 = tablixDynamicMemberInstance.MoveNext();
				while (flag2)
				{
					flag = false;
					if (RenderRowMemberInstance(tablixMember, context.AddToggledItems))
					{
						flag = DynamicWithVisibleChildren(tablixMember, context.AddToggledItems, ref pageBreakAtStart);
					}
					if (flag)
					{
						return pageBreakAtStart == null;
					}
					flag2 = tablixDynamicMemberInstance.MoveNext();
				}
			}
			return false;
		}

		private double RegisterSizeForRepeatWithBefore(Microsoft.ReportingServices.OnDemandReportRendering.Tablix tablix, TablixMember rowMemberParent, int start, int end, TablixContext context)
		{
			if (start > end)
			{
				return 0.0;
			}
			TablixMemberCollection tablixMemberCollection = null;
			tablixMemberCollection = ((rowMemberParent != null) ? rowMemberParent.Children : tablix.RowHierarchy.MemberCollection);
			if (tablixMemberCollection == null)
			{
				context.RowMemberIndexCell = rowMemberParent.MemberCellIndex;
				return context.NextRow(m_bodyRowsHeigths);
			}
			double num = 0.0;
			TablixMember tablixMember = null;
			Visibility visibility = null;
			bool advanceRow = false;
			bool hasRecursivePeer = false;
			bool hasVisibleStaticPeer = CheckForVisibleStaticPeer(tablixMemberCollection);
			if (start == -1 && end == -1)
			{
				start = 0;
				end = tablixMemberCollection.Count - 1;
			}
			for (int i = start; i <= end; i++)
			{
				tablixMember = tablixMemberCollection[i];
				visibility = tablixMember.Visibility;
				if (visibility == null || visibility.HiddenState != 0)
				{
					if (tablixMember.Group != null)
					{
						hasRecursivePeer = tablixMember.Group.IsRecursive;
					}
					if (context.EnterRowMember(this, tablixMember, visibility, hasRecursivePeer, hasVisibleStaticPeer))
					{
						num += RegisterSizeForRepeatWithBefore(tablix, tablixMember, -1, -1, context);
					}
					context.LeaveRowMemberInstance(tablixMember, null, visibility, hasRecursivePeer);
					context.LeaveRowMember(tablixMember, m_bodyRowsHeigths, visibility, hasRecursivePeer, null, ref advanceRow);
				}
			}
			return num;
		}

		private bool DynamicWithVisibleChildren(TablixMember rowMemberParent, TablixContext context)
		{
			if (rowMemberParent == null)
			{
				return false;
			}
			PageBreakInfo pageBreakAtStart = null;
			return MemberWithVisibleChildren(rowMemberParent.Children, context.AddToggledItems, ref pageBreakAtStart);
		}

		private bool DynamicWithNonKeepWithVisibleChildren(TablixMember rowMemberParent, int childrenLevel, TablixContext context, ref bool pageBreak)
		{
			if (rowMemberParent == null)
			{
				return false;
			}
			if (rowMemberParent.Children == null)
			{
				if (rowMemberParent.IsStatic && rowMemberParent.KeepWithGroup != 0)
				{
					return false;
				}
				return true;
			}
			bool found = false;
			int startChild = 0;
			if (childrenLevel >= 0 && childrenLevel < m_tablixCreateState.Count)
			{
				startChild = m_tablixCreateState[childrenLevel];
			}
			MemberWithNonKeepWithVisibleChildren(rowMemberParent.Children, context.AddToggledItems, context.CheckPageBreaks, startChild, childrenLevel, ref found, ref pageBreak);
			return found;
		}

		private bool DynamicWithVisibleChildren(TablixMember rowMemberParent, bool addToggledItems, ref PageBreakInfo pageBreakAtStart)
		{
			if (rowMemberParent == null)
			{
				return false;
			}
			bool num = MemberWithVisibleChildren(rowMemberParent.Children, addToggledItems, ref pageBreakAtStart);
			if (num)
			{
				PageBreakInfo pageBreakInfo = CreatePageBreakInfo(rowMemberParent.Group);
				if (pageBreakInfo != null && !pageBreakInfo.Disabled)
				{
					PageBreakLocation breakLocation = pageBreakInfo.BreakLocation;
					if (breakLocation == PageBreakLocation.Start || breakLocation == PageBreakLocation.StartAndEnd)
					{
						pageBreakAtStart = pageBreakInfo;
					}
				}
			}
			return num;
		}

		private bool MemberWithVisibleChildren(TablixMemberCollection rowMembers, bool addToggledItems, ref PageBreakInfo pageBreakAtStart)
		{
			if (rowMembers == null)
			{
				return true;
			}
			bool flag = true;
			bool flag2 = true;
			TablixMember tablixMember = null;
			TablixMemberInstance tablixMemberInstance = null;
			TablixDynamicMemberInstance tablixDynamicMemberInstance = null;
			for (int i = 0; i < rowMembers.Count; i++)
			{
				tablixMember = rowMembers[i];
				if (tablixMember.Visibility != null && tablixMember.Visibility.HiddenState == SharedHiddenState.Always)
				{
					continue;
				}
				flag2 = true;
				flag = true;
				tablixMemberInstance = tablixMember.Instance;
				if (tablixMember.IsStatic)
				{
					flag2 = RenderRowMemberInstance(tablixMember, addToggledItems);
				}
				else
				{
					if (tablixMember.Visibility != null && tablixMember.Visibility.ToggleItem != null)
					{
						int num = i + 1;
						if (num < rowMembers.Count && rowMembers[num].IsTotal)
						{
							return true;
						}
					}
					tablixDynamicMemberInstance = (TablixDynamicMemberInstance)tablixMemberInstance;
					tablixDynamicMemberInstance.ResetContext();
					flag = tablixDynamicMemberInstance.MoveNext();
					if (flag)
					{
						flag2 = RenderRowMemberInstance(tablixMember, addToggledItems);
					}
				}
				while (flag)
				{
					if (flag2 && DynamicWithVisibleChildren(tablixMember, addToggledItems, ref pageBreakAtStart))
					{
						return true;
					}
					if (tablixMember.IsStatic)
					{
						flag = false;
						continue;
					}
					flag = tablixDynamicMemberInstance.MoveNext();
					if (flag)
					{
						flag2 = RenderRowMemberInstance(tablixMember, addToggledItems);
					}
				}
				tablixDynamicMemberInstance = null;
			}
			return false;
		}

		private bool MemberWithNonKeepWithVisibleChildren(TablixMemberCollection rowMembers, bool addToggledItems, bool checkPageBreaks, int startChild, int level, ref bool found, ref bool pageBreak)
		{
			if (rowMembers == null)
			{
				found = true;
				return true;
			}
			bool flag = true;
			bool flag2 = true;
			TablixMember tablixMember = null;
			TablixMemberInstance tablixMemberInstance = null;
			TablixDynamicMemberInstance tablixDynamicMemberInstance = null;
			bool result = false;
			bool flag3 = false;
			bool found2 = false;
			bool flag4 = checkPageBreaks;
			for (int i = startChild; i < rowMembers.Count; i++)
			{
				tablixMember = rowMembers[i];
				if (level >= 0)
				{
					level = -1;
					if (tablixMember.KeepWithGroup == KeepWithGroup.After)
					{
						flag3 = true;
					}
				}
				else
				{
					if (tablixMember.Visibility != null && tablixMember.Visibility.HiddenState == SharedHiddenState.Always)
					{
						continue;
					}
					flag2 = true;
					flag = true;
					tablixMemberInstance = tablixMember.Instance;
					if (tablixMember.IsStatic)
					{
						flag2 = RenderRowMemberInstance(tablixMember, addToggledItems);
					}
					else
					{
						tablixDynamicMemberInstance = (TablixDynamicMemberInstance)tablixMemberInstance;
						tablixDynamicMemberInstance.ResetContext();
						flag = tablixDynamicMemberInstance.MoveNext();
						if (flag)
						{
							flag2 = RenderRowMemberInstance(tablixMember, addToggledItems);
						}
					}
					flag4 = checkPageBreaks;
					if (flag4 && (tablixMember.IsTotal || (tablixMember.Visibility != null && tablixMember.Visibility.ToggleItem != null)))
					{
						flag4 = false;
					}
					while (flag)
					{
						if (flag2 && MemberWithNonKeepWithVisibleChildren(tablixMember.Children, addToggledItems, flag4, 0, -1, ref found2, ref pageBreak))
						{
							if (!tablixMember.IsStatic)
							{
								if (flag4)
								{
									PageBreakInfo pageBreakInfo = CreatePageBreakInfo(tablixMember.Group);
									if (pageBreakInfo != null && !pageBreakInfo.Disabled)
									{
										PageBreakLocation breakLocation = pageBreakInfo.BreakLocation;
										if (breakLocation != 0)
										{
											pageBreak = true;
											if (breakLocation == PageBreakLocation.Start || breakLocation == PageBreakLocation.StartAndEnd)
											{
												return result;
											}
										}
									}
								}
								found = found2;
								return true;
							}
							if (tablixMember.KeepWithGroup != KeepWithGroup.After)
							{
								if (tablixMember.KeepWithGroup == KeepWithGroup.Before)
								{
									found = true;
								}
								else
								{
									found = found2;
								}
								return true;
							}
							flag3 = true;
							result = true;
						}
						if (tablixMember.IsStatic)
						{
							flag = false;
							continue;
						}
						flag = tablixDynamicMemberInstance.MoveNext();
						if (flag)
						{
							flag2 = RenderRowMemberInstance(tablixMember, addToggledItems);
						}
						else if (flag3)
						{
							found = true;
							return true;
						}
					}
					tablixDynamicMemberInstance = null;
				}
			}
			return result;
		}

		private bool CheckAndAdvanceToNextVisibleInstance(Microsoft.ReportingServices.OnDemandReportRendering.Tablix tablix, TablixMember rowMember, TablixDynamicMemberInstance rowDynamicInstance, TablixContext context)
		{
			bool flag = true;
			Visibility visibility = rowMember.Visibility;
			if (visibility != null && visibility.HiddenState == SharedHiddenState.Sometimes && visibility.ToggleItem == null)
			{
				while (flag && rowDynamicInstance.Visibility.CurrentlyHidden)
				{
					context.AddMemberToCurrentPage(tablix, rowMember, TablixRegion.RowHeader, createDetail: true);
					flag = rowDynamicInstance.MoveNext();
				}
			}
			return flag;
		}

		private bool AdvanceToNextVisibleInstance(Microsoft.ReportingServices.OnDemandReportRendering.Tablix tablix, TablixMember rowMember, TablixDynamicMemberInstance rowDynamicInstance, TablixContext context)
		{
			bool flag = rowDynamicInstance.MoveNext();
			if (flag)
			{
				flag = CheckAndAdvanceToNextVisibleInstance(tablix, rowMember, rowDynamicInstance, context);
			}
			return flag;
		}

		private int AdvanceInState(Microsoft.ReportingServices.OnDemandReportRendering.Tablix tablix, TablixMember rowMemberParent, int level, TablixContext context, out int ignoredRows, ref int staticHeaderLevel, List<bool> ignoreTotals)
		{
			ignoredRows = 0;
			TablixMemberCollection tablixMemberCollection = null;
			tablixMemberCollection = ((rowMemberParent != null) ? rowMemberParent.Children : tablix.RowHierarchy.MemberCollection);
			if (tablixMemberCollection == null)
			{
				context.RowMemberIndexCell = rowMemberParent.MemberCellIndex;
				if (context.IgnoreRow)
				{
					context.AddDetailRowToCurrentPage(tablix);
					ignoredRows++;
					context.NextRow(m_bodyRowsHeigths);
				}
				if (!context.IgnoreRow)
				{
					return 1;
				}
				return 0;
			}
			bool flag = true;
			bool flag2 = true;
			int num = 0;
			bool flag3 = true;
			int ignoredRows2 = 0;
			bool advanceRow = false;
			bool hasRecursivePeer = false;
			TablixMember tablixMember = null;
			Visibility visibility = null;
			TablixMemberInstance tablixMemberInstance = null;
			TablixDynamicMemberInstance tablixDynamicMemberInstance = null;
			bool hasVisibleStaticPeer = CheckForVisibleStaticPeer(tablixMemberCollection);
			if (m_tablixCreateState.Count <= level)
			{
				m_tablixCreateState.Add(num);
				ignoreTotals.Add(item: false);
			}
			else
			{
				flag3 = false;
				num = m_tablixCreateState[level];
			}
			for (int i = num; i < tablixMemberCollection.Count; i++)
			{
				tablixMember = tablixMemberCollection[i];
				visibility = tablixMember.Visibility;
				if (context.AlwaysHiddenMember(tablix, tablixMember, visibility, TablixRegion.RowHeader, createDetail: true, ref ignoredRows))
				{
					if (m_levelForRepeat >= level)
					{
						m_levelForRepeat = level - 1;
					}
					if (m_partialMemberLevel >= level)
					{
						m_partialMemberLevel = level - 1;
					}
					continue;
				}
				if (i > num)
				{
					if (m_levelForRepeat >= level)
					{
						m_levelForRepeat = level - 1;
					}
					if (m_partialMemberLevel >= level)
					{
						m_partialMemberLevel = level - 1;
					}
				}
				flag2 = true;
				flag = true;
				tablixMemberInstance = tablixMember.Instance;
				if (tablixMember.Group != null)
				{
					hasRecursivePeer = tablixMember.Group.IsRecursive;
				}
				if (tablixMember.IsStatic)
				{
					flag2 = context.EnterRowMember(this, tablixMember, visibility, hasRecursivePeer, hasVisibleStaticPeer);
					if (staticHeaderLevel < 0 && flag2 && tablixMember.KeepWithGroup != 0 && (tablixMember.KeepWithGroup != KeepWithGroup.Before || i == num))
					{
						staticHeaderLevel = level;
					}
				}
				else
				{
					if (!flag3 && num == i)
					{
						int num2 = CheckKeepWithGroupUp(tablixMemberCollection, num - 1, KeepWithGroup.After);
						if (num2 < num && tablixMemberCollection[num2].RepeatOnNewPage)
						{
							m_levelForRepeat = level;
						}
					}
					if (!ignoreTotals[level] && visibility != null && visibility.ToggleItem != null)
					{
						int num3 = i + 1;
						if (num3 < tablixMemberCollection.Count && tablixMemberCollection[num3].IsTotal)
						{
							if (i == 0)
							{
								m_tablixCreateState.RemoveAt(level);
								ignoreTotals.RemoveAt(level);
							}
							else
							{
								m_tablixCreateState[level] = -i;
							}
							return 1;
						}
					}
					tablixDynamicMemberInstance = (TablixDynamicMemberInstance)tablixMemberInstance;
					if (flag3 || i > num)
					{
						tablixDynamicMemberInstance.ResetContext();
						flag = tablixDynamicMemberInstance.MoveNext();
					}
					if (flag)
					{
						flag2 = context.EnterRowMember(null, tablixMember, visibility, hasRecursivePeer, hasVisibleStaticPeer: false);
					}
				}
				while (flag)
				{
					if (flag2)
					{
						if (AdvanceInState(tablix, tablixMember, level + 1, context, out ignoredRows2, ref staticHeaderLevel, ignoreTotals) > 0)
						{
							context.LeaveRowMemberInstance(tablixMember, null, visibility, hasRecursivePeer);
							context.LeaveRowMember(tablixMember, m_bodyRowsHeigths, visibility, hasRecursivePeer, null, ref advanceRow);
							m_tablixCreateState[level] = i;
							ignoreTotals[level] = true;
							return 1;
						}
						if (ignoredRows2 > 0)
						{
							context.AddMemberHeaderToCurrentPage(tablixMember, TablixRegion.RowHeader, headerFooterEval: true);
							ignoredRows += ignoredRows2;
						}
						else
						{
							context.AddMemberHeaderToCurrentPage(tablixMember, TablixRegion.RowHeader, headerFooterEval: false);
						}
					}
					else
					{
						context.AddMemberToCurrentPage(tablix, tablixMember, TablixRegion.RowHeader, createDetail: true);
					}
					context.LeaveRowMemberInstance(tablixMember, null, visibility, hasRecursivePeer);
					if (tablixMember.IsStatic)
					{
						flag = false;
						context.LeaveRowMember(tablixMember, m_bodyRowsHeigths, visibility, hasRecursivePeer, null, ref advanceRow);
						if (staticHeaderLevel == level)
						{
							staticHeaderLevel = -1;
						}
					}
					else
					{
						flag = tablixDynamicMemberInstance.MoveNext();
						if (flag)
						{
							flag2 = context.EnterRowMemberInstance(tablixMember, visibility, hasRecursivePeer);
						}
						else
						{
							context.LeaveRowMember(tablixMember, m_bodyRowsHeigths, visibility, hasRecursivePeer, null, ref advanceRow);
						}
					}
					if (m_partialMemberLevel >= level)
					{
						m_partialMemberLevel = level - 1;
					}
				}
				tablixDynamicMemberInstance = null;
			}
			m_tablixCreateState.RemoveAt(level);
			ignoreTotals.RemoveAt(level);
			if (m_levelForRepeat >= level)
			{
				m_levelForRepeat = level - 1;
			}
			if (m_partialMemberLevel >= level)
			{
				m_partialMemberLevel = level - 1;
			}
			return 0;
		}

		private List<int> GetTablixMembersInstanceIndex()
		{
			if (m_tablixCreateState == null || m_tablixCreateState.Count == 0)
			{
				return null;
			}
			List<int> list = new List<int>(m_tablixCreateState.Count);
			Microsoft.ReportingServices.OnDemandReportRendering.Tablix tablix = (Microsoft.ReportingServices.OnDemandReportRendering.Tablix)m_source;
			GetTablixMembersInstanceIndex(tablix.RowHierarchy.MemberCollection, 0, list);
			return list;
		}

		private void GetTablixMembersInstanceIndex(TablixMemberCollection rowMembers, int level, List<int> instanceState)
		{
			if (rowMembers != null && m_tablixCreateState.Count > level)
			{
				int i = m_tablixCreateState[level];
				TablixMember tablixMember = rowMembers[i];
				if (tablixMember.IsStatic)
				{
					instanceState.Add(0);
				}
				else
				{
					TablixDynamicMemberInstance tablixDynamicMemberInstance = (TablixDynamicMemberInstance)tablixMember.Instance;
					instanceState.Add(tablixDynamicMemberInstance.GetInstanceIndex());
				}
				GetTablixMembersInstanceIndex(tablixMember.Children, level + 1, instanceState);
			}
		}

		private void SetTablixMembersInstanceIndex(List<int> instanceState)
		{
			if (m_tablixCreateState != null && m_tablixCreateState.Count != 0)
			{
				Microsoft.ReportingServices.OnDemandReportRendering.Tablix tablix = (Microsoft.ReportingServices.OnDemandReportRendering.Tablix)m_source;
				SetTablixMembersInstanceIndex(tablix.RowHierarchy.MemberCollection, 0, instanceState);
			}
		}

		private void SetTablixMembersInstanceIndex(TablixMemberCollection rowMembers, int level, List<int> instanceState)
		{
			if (rowMembers != null && m_tablixCreateState.Count > level)
			{
				int i = m_tablixCreateState[level];
				TablixMember tablixMember = rowMembers[i];
				if (!tablixMember.IsStatic)
				{
					((TablixDynamicMemberInstance)tablixMember.Instance).SetInstanceIndex(instanceState[level]);
				}
				SetTablixMembersInstanceIndex(tablixMember.Children, level + 1, instanceState);
			}
		}

		private void CreateTablixItems(Microsoft.ReportingServices.OnDemandReportRendering.Tablix tablix, TablixContext context)
		{
			bool advanceRow = true;
			LevelInfo parentLevelInfo = null;
			bool flag = true;
			List<bool> ignoreTotals = null;
			int staticHeaderLevel = -1;
			if (m_tablixCreateState != null)
			{
				m_levelForRepeat = -1;
				ignoreTotals = new List<bool>(m_tablixCreateState.Count);
				for (int i = 0; i < m_tablixCreateState.Count - 1; i++)
				{
					ignoreTotals.Add(item: true);
				}
				ignoreTotals.Add(m_ignoreTotalsOnLastLevel);
				m_partialMemberLevel = m_tablixCreateState.Count - 2;
				int ignoredRows = 0;
				AdvanceInState(tablix, null, 0, context, out ignoredRows, ref staticHeaderLevel, ignoreTotals);
				if (m_tablixCreateState.Count == 0)
				{
					flag = false;
				}
				else
				{
					CheckForOnlyStaticHeaders(tablix, staticHeaderLevel, context);
				}
			}
			if (flag)
			{
				if (m_hasDetailCellsWithColSpan && context.HeaderColumnRows > 0 && !context.ColumnHeadersCreated && context.Writer != null)
				{
					CreateColumnMemberChildren(tablix, null, m_colMembersDepth, parentBorderHeader: false, 0, 0, context, createDetail: false, ref parentLevelInfo);
					context.OuterColumnHeaders = parentLevelInfo.MemberCells;
					context.OmittedOuterColumnHeaders = parentLevelInfo.OmittedMembersCells;
					context.ColumnHeadersCreated = true;
				}
				CreateRowMemberChildren(tablix, null, m_toggleMemberState, m_rowMembersDepth, parentBorderHeader: false, 0, 0, 0, context, ref advanceRow, ref parentLevelInfo, ref ignoreTotals, tablix.KeepTogether);
				context.OuterRowHeaders = parentLevelInfo.MemberCells;
				m_ignoreTotalsOnLastLevel = false;
				if (m_tablixCreateState.Count > 0)
				{
					m_ignoreTotalsOnLastLevel = ignoreTotals[ignoreTotals.Count - 1];
				}
			}
			if (!context.PageContext.CancelPage && context.OuterRowHeaders == null && context.HeaderColumnRows > 0 && (context.Writer != null || context.Interactivity != null))
			{
				CreateColumnMemberChildren(tablix, null, m_colMembersDepth, parentBorderHeader: false, 0, 0, context, createDetail: false, ref parentLevelInfo);
				if (!context.ColumnHeadersCreated)
				{
					context.OuterColumnHeaders = parentLevelInfo.MemberCells;
					context.OmittedOuterColumnHeaders = parentLevelInfo.OmittedMembersCells;
					context.ColumnHeadersCreated = true;
				}
			}
		}

		private void CheckForOnlyStaticHeaders(Microsoft.ReportingServices.OnDemandReportRendering.Tablix tablix, int staticHeaderLevel, TablixContext context)
		{
			if (m_tablixCreateState == null || m_tablixCreateState.Count == 0 || m_levelForRepeat < 0 || staticHeaderLevel <= m_levelForRepeat)
			{
				return;
			}
			TablixMemberCollection tablixMemberCollection = tablix.RowHierarchy.MemberCollection;
			TablixMember tablixMember = null;
			m_levelForRepeat = staticHeaderLevel - 1;
			int i;
			for (i = 0; i < staticHeaderLevel; i++)
			{
				if (m_tablixCreateState[i] < 0)
				{
					return;
				}
				if (tablixMemberCollection == null)
				{
					break;
				}
				tablixMember = tablixMemberCollection[m_tablixCreateState[i]];
				tablixMemberCollection = tablixMember.Children;
			}
			bool pageBreak = false;
			bool found = DynamicWithNonKeepWithVisibleChildren(tablixMember, i, context, ref pageBreak);
			bool flag = false;
			List<int> tablixMembersInstanceIndex = null;
			while (!found && !pageBreak)
			{
				TablixMember parent = tablixMember.Parent;
				if (parent == null)
				{
					break;
				}
				if (tablixMember.IsStatic)
				{
					m_levelForRepeat--;
					found = MemberWithNonKeepWithVisibleChildren(parent.Children, context.AddToggledItems, context.CheckPageBreaks, m_tablixCreateState[i - 1] + 1, -1, ref found, ref pageBreak);
					if (found || pageBreak)
					{
						break;
					}
				}
				else
				{
					if (context.CheckPageBreaks)
					{
						PageBreakInfo pageBreakInfo = CreatePageBreakInfo(tablixMember.Group);
						if (pageBreakInfo != null && !pageBreakInfo.Disabled && pageBreakInfo.BreakLocation != 0)
						{
							break;
						}
					}
					TablixDynamicMemberInstance tablixDynamicMemberInstance = (TablixDynamicMemberInstance)tablixMember.Instance;
					if (!flag)
					{
						tablixMembersInstanceIndex = GetTablixMembersInstanceIndex();
						flag = true;
					}
					bool flag2 = tablixDynamicMemberInstance.MoveNext();
					while (flag2 && !found)
					{
						if (context.EnterRowMemberInstance(tablixMember, tablixMember.Visibility, hasRecursivePeer: false))
						{
							found = DynamicWithNonKeepWithVisibleChildren(tablixMember, -1, context, ref pageBreak);
						}
						context.LeaveRowMemberInstance(tablixMember, null, tablixMember.Visibility, hasRecursivePeer: false);
						if (!found && !pageBreak)
						{
							flag2 = tablixDynamicMemberInstance.MoveNext();
						}
					}
					if (found)
					{
						break;
					}
					m_levelForRepeat--;
					found = MemberWithNonKeepWithVisibleChildren(parent.Children, context.AddToggledItems, context.CheckPageBreaks, m_tablixCreateState[i - 1] + 1, -1, ref found, ref pageBreak);
					if (found || pageBreak)
					{
						break;
					}
				}
				i--;
				tablixMember = tablixMember.Parent;
			}
			if (!found)
			{
				m_levelForRepeat = -1;
			}
			if (flag)
			{
				SetTablixMembersInstanceIndex(tablixMembersInstanceIndex);
			}
		}

		private static bool CheckForVisibleStaticPeer(TablixMemberCollection members)
		{
			foreach (TablixMember member in members)
			{
				if (member.IsStatic && !member.IsTotal)
				{
					Visibility visibility = member.Visibility;
					if (visibility == null || visibility.HiddenState == SharedHiddenState.Never)
					{
						return true;
					}
					if (visibility.ToggleItem != null)
					{
						return true;
					}
					if (!member.Instance.Visibility.CurrentlyHidden)
					{
						return true;
					}
				}
			}
			return false;
		}

		private int[] DefDetailRowsCapacity(TablixRowCollection defDetailRows)
		{
			int[] array = new int[defDetailRows.Count];
			for (int i = 0; i < defDetailRows.Count; i++)
			{
				for (int j = 0; j < defDetailRows[i].Count; j++)
				{
					if (defDetailRows[i][j] != null)
					{
						array[i]++;
					}
				}
				if (array[i] < 4)
				{
					array[i] = 4;
				}
			}
			return array;
		}

		internal override bool CalculatePage(RPLWriter rplWriter, PageItemHelper lastPageInfo, PageContext pageContext, PageItem[] siblings, RepeatWithItem[] repeatWithItems, double parentTopInPage, ref double parentPageHeight, Interactivity interactivity)
		{
			Microsoft.ReportingServices.OnDemandReportRendering.Tablix tablix = (Microsoft.ReportingServices.OnDemandReportRendering.Tablix)m_source;
			double num = parentTopInPage + m_itemPageSizes.Top;
			int num2 = 0;
			ItemSizes contentSize = null;
			int[] array = null;
			PageItemHelper pageItemHelper = null;
			bool flag = false;
			if (lastPageInfo != null)
			{
				pageItemHelper = lastPageInfo.ChildPage;
			}
			if (m_partialPageItem != null || pageItemHelper != null)
			{
				num2 = 0;
				array = DefDetailRowsCapacity(m_bodyRows);
				if (tablix.OmitBorderOnPageBreak)
				{
					m_rplItemState |= 1;
				}
			}
			else if (m_tablixCreateState == null)
			{
				num2 = tablix.Columns;
				flag = true;
				AdjustOriginFromItemsAbove(siblings, repeatWithItems);
				if (!HitsCurrentPage(pageContext, parentTopInPage))
				{
					return false;
				}
				if (ResolveItemHiddenState(rplWriter, interactivity, pageContext, createForRepeat: false, ref contentSize))
				{
					parentPageHeight = Math.Max(parentPageHeight, m_itemPageSizes.Bottom);
					if (rplWriter != null && m_itemRenderSizes == null)
					{
						CreateItemRenderSizes(null, pageContext, createForRepeat: false);
					}
					return true;
				}
				num = parentTopInPage + m_itemPageSizes.Top;
				if (num2 > 0)
				{
					num += CornerSize(tablix.ColumnHierarchy.MemberCollection);
				}
				else
				{
					if (!pageContext.IgnorePageBreaks && !base.PageBreakAtStart && (RoundedDouble)num > 0.0)
					{
						PageBreakInfo pageBreakAtStart = null;
						MemberWithVisibleChildren(tablix.RowHierarchy.MemberCollection, pageContext.AddToggledItems, ref pageBreakAtStart);
						if (pageBreakAtStart != null)
						{
							base.ItemState = State.TopNextPage;
							pageContext.RegisterPageBreak(pageBreakAtStart);
							return false;
						}
					}
					if (pageContext.TracingEnabled && pageContext.IgnorePageBreaks)
					{
						TracePageBreakAtStartIgnored(pageContext);
					}
				}
				array = DefDetailRowsCapacity(m_bodyRows);
				if (!pageContext.IgnorePageBreaks)
				{
					pageContext.RegisterPageName(PageName);
				}
			}
			else
			{
				if (tablix.OmitBorderOnPageBreak)
				{
					m_rplItemState |= 1;
				}
				if (tablix.RepeatColumnHeaders)
				{
					num2 = tablix.Columns;
				}
				if (num2 > 0)
				{
					double num3 = CornerSize(tablix.ColumnHierarchy.MemberCollection);
					if (num3 >= pageContext.PageHeight)
					{
						num3 = 0.0;
					}
					num += num3;
				}
				array = DefDetailRowsCapacity(m_bodyRows);
			}
			WriteStartItemToStream(rplWriter, pageContext);
			TablixInstance tablixInstance = (TablixInstance)tablix.Instance;
			PageContext pageContext2 = pageContext;
			if (!pageContext2.FullOnPage)
			{
				if (base.IgnorePageBreaks || tablixInstance.NoRows)
				{
					pageContext2 = new PageContext(pageContext, PageContext.PageContextFlags.FullOnPage, PageContext.IgnorePBReasonFlag.Toggled);
				}
				else if (flag && tablix.KeepTogether && !pageContext2.KeepTogether)
				{
					pageContext2 = new PageContext(pageContext);
					pageContext2.KeepTogether = true;
					if (pageContext.TracingEnabled && parentTopInPage + m_itemPageSizes.Height >= pageContext2.OriginalPageHeight)
					{
						TracePageGrownOnKeepTogetherItem(pageContext.PageNumber);
					}
				}
			}
			TablixContext tablixContext = null;
			tablixContext = ((m_rplElement != null) ? ((TablixContext)new RPLContext(rplWriter, pageItemHelper, tablixInstance.NoRows, tablix.LayoutDirection == TablixLayoutDirection.LTR, pageContext2, num, tablix.Rows, num2, array, interactivity, (RPLTablix)m_rplElement)) : ((TablixContext)new StreamContext(rplWriter, pageItemHelper, tablixInstance.NoRows, tablix.LayoutDirection == TablixLayoutDirection.LTR, pageContext2, num, tablix.Rows, num2, array, interactivity)));
			tablixContext.PageBreakNeedsOverride = !pageContext.IsPageBreakRegistered;
			CreateTablixItems(tablix, tablixContext);
			if (tablixContext.PageContext.CancelPage)
			{
				m_itemState = State.Below;
				m_rplElement = null;
			}
			if (m_partialPageItem == null && m_tablixCreateState.Count == 0)
			{
				m_tablixCreateState = null;
				m_partialPageItem = null;
				m_itemState = State.OnPage;
				if (!pageContext2.IgnorePageBreaks && (base.PageBreakAtEnd || tablixContext.PageBreakAtEnd || tablixContext.PropagatedPageBreak))
				{
					m_itemState = State.OnPagePBEnd;
					if (base.PageBreakAtEnd)
					{
						pageContext.RegisterPageBreak(new PageBreakInfo(PageBreak, base.ItemName), tablixContext.PageBreakNeedsOverride);
					}
				}
				if (pageContext.TracingEnabled && pageContext2.IgnorePageBreaks && base.PageBreakAtEnd)
				{
					TracePageBreakAtEndIgnored(pageContext2);
				}
			}
			else
			{
				m_itemState = State.SpanPages;
				if (tablix.OmitBorderOnPageBreak)
				{
					m_rplItemState |= 2;
				}
			}
			if (contentSize == null)
			{
				m_itemPageSizes.AdjustHeightTo(tablixContext.TablixBottom - parentTopInPage - m_itemPageSizes.Top);
			}
			parentPageHeight = Math.Max(parentPageHeight, m_itemPageSizes.Bottom);
			WriteEndItemToStream(rplWriter, tablixContext, tablix, contentSize);
			return true;
		}

		internal void WriteStartItemToStream(RPLWriter rplWriter, PageContext pageContext)
		{
			if (rplWriter != null)
			{
				BinaryWriter binaryWriter = rplWriter.BinaryWriter;
				if (binaryWriter != null)
				{
					Stream baseStream = binaryWriter.BaseStream;
					m_offset = baseStream.Position;
					binaryWriter.Write((byte)13);
					WriteElementProps(binaryWriter, rplWriter, pageContext, m_offset + 1);
				}
				else
				{
					m_rplElement = new RPLTablix();
					WriteElementProps(m_rplElement.ElementProps, rplWriter, pageContext);
				}
			}
		}

		internal void WriteEndItemToStream(RPLWriter rplWriter, TablixContext context, Microsoft.ReportingServices.OnDemandReportRendering.Tablix tablix, ItemSizes contentSize)
		{
			if (rplWriter != null)
			{
				double tablixWidth = 0.0;
				double tablixHeight = 0.0;
				BinaryWriter binaryWriter = rplWriter.BinaryWriter;
				if (binaryWriter != null)
				{
					Stream baseStream = binaryWriter.BaseStream;
					long position = baseStream.Position;
					binaryWriter.Write((byte)17);
					binaryWriter.Write(m_offset);
					context.WriteTablixMeasurements(tablix, m_rowMembersDepth, m_colMembersDepth, ref tablixWidth, ref tablixHeight);
					binaryWriter.Write(byte.MaxValue);
					binaryWriter.Flush();
					m_offset = baseStream.Position;
					binaryWriter.Write((byte)254);
					binaryWriter.Write(position);
					binaryWriter.Write(byte.MaxValue);
				}
				else
				{
					context.WriteTablixMeasurements(tablix, m_rowMembersDepth, m_colMembersDepth, ref tablixWidth, ref tablixHeight);
				}
				CreateItemRenderSizes(contentSize, context.PageContext, createForRepeat: false);
				m_itemRenderSizes.AdjustHeightTo(tablixHeight);
				m_itemRenderSizes.AdjustWidthTo(tablixWidth);
			}
		}

		internal override void WriteItemSharedStyleProps(BinaryWriter spbifWriter, Style style, PageContext pageContext)
		{
			WriteStyleProp(style, spbifWriter, StyleAttributeNames.BackgroundColor, 34);
			WriteBackgroundImage(spbifWriter, style, writeShared: true, pageContext);
		}

		internal override void WriteItemSharedStyleProps(RPLStyleProps rplStyleProps, Style style, PageContext pageContext)
		{
			WriteStyleProp(style, rplStyleProps, StyleAttributeNames.BackgroundColor, 34);
			WriteBackgroundImage(rplStyleProps, style, writeShared: true, pageContext);
		}

		internal override void WriteItemNonSharedStyleProp(BinaryWriter spbifWriter, Style styleDef, StyleInstance style, StyleAttributeNames styleAtt, PageContext pageContext)
		{
			switch (styleAtt)
			{
			case StyleAttributeNames.BackgroundColor:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BackgroundColor, 34);
				break;
			case StyleAttributeNames.BackgroundImage:
				WriteBackgroundImage(spbifWriter, styleDef, writeShared: false, pageContext);
				break;
			}
		}

		internal override void WriteItemNonSharedStyleProp(RPLStyleProps rplStyleProps, Style styleDef, StyleInstance style, StyleAttributeNames styleAtt, PageContext pageContext)
		{
			switch (styleAtt)
			{
			case StyleAttributeNames.BackgroundColor:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BackgroundColor, 34);
				break;
			case StyleAttributeNames.BackgroundImage:
				WriteBackgroundImage(rplStyleProps, styleDef, writeShared: false, pageContext);
				break;
			}
		}

		internal override void WritePaginationInfo(BinaryWriter reportPageInfo)
		{
			if (reportPageInfo == null)
			{
				return;
			}
			reportPageInfo.Write((byte)11);
			base.WritePaginationInfoProperties(reportPageInfo);
			reportPageInfo.Write((byte)16);
			reportPageInfo.Write(m_levelForRepeat);
			reportPageInfo.Write((byte)22);
			reportPageInfo.Write(m_ignoreTotalsOnLastLevel);
			if (m_tablixCreateState != null && m_tablixCreateState.Count > 0)
			{
				reportPageInfo.Write((byte)17);
				reportPageInfo.Write(m_tablixCreateState.Count);
				for (int i = 0; i < m_tablixCreateState.Count; i++)
				{
					reportPageInfo.Write(m_tablixCreateState[i]);
				}
			}
			List<int> tablixMembersInstanceIndex = GetTablixMembersInstanceIndex();
			if (tablixMembersInstanceIndex != null && tablixMembersInstanceIndex.Count > 0)
			{
				reportPageInfo.Write((byte)18);
				reportPageInfo.Write(tablixMembersInstanceIndex.Count);
				for (int j = 0; j < tablixMembersInstanceIndex.Count; j++)
				{
					reportPageInfo.Write(tablixMembersInstanceIndex[j]);
				}
			}
			if (m_partialPageItem != null)
			{
				reportPageInfo.Write((byte)19);
				m_partialPageItem.WritePaginationInfo(reportPageInfo);
			}
			reportPageInfo.Write(byte.MaxValue);
		}

		internal override PageItemHelper WritePaginationInfo()
		{
			PageTablixHelper pageTablixHelper = new PageTablixHelper(11);
			base.WritePaginationInfoProperties(pageTablixHelper);
			pageTablixHelper.LevelForRepeat = m_levelForRepeat;
			pageTablixHelper.IgnoreTotalsOnLastLevel = m_ignoreTotalsOnLastLevel;
			pageTablixHelper.TablixCreateState = PageItem.GetNewList(m_tablixCreateState);
			List<int> tablixMembersInstanceIndex = GetTablixMembersInstanceIndex();
			pageTablixHelper.MembersInstanceIndex = PageItem.GetNewList(tablixMembersInstanceIndex);
			if (m_partialPageItem != null)
			{
				pageTablixHelper.ChildPage = m_partialPageItem.WritePaginationInfo();
			}
			return pageTablixHelper;
		}

		private static PageBreakInfo CreatePageBreakInfo(Group group)
		{
			PageBreakInfo result = null;
			if (group != null)
			{
				result = new PageBreakInfo(group.PageBreak, group.Name);
			}
			return result;
		}

		internal string BuildTablixMemberPath(TablixMember rowMember)
		{
			return DiagnosticsUtilities.BuildTablixMemberPath(base.ItemName, rowMember, m_memberIndexByLevel);
		}

		private void TracePageGrownOnKeepTogetherMember(int pageNumber, TablixMember rowMember)
		{
			RenderingDiagnostics.Trace(RenderingArea.KeepTogether, TraceLevel.Verbose, "PR-DIAG [Page {0}] Member '{1}' kept together - RowContext - Page grown", pageNumber, DiagnosticsUtilities.BuildTablixMemberPath(base.ItemName, rowMember, m_memberIndexByLevel));
		}

		private void TracePageGrownOnKeepWithMember(int pageNumber)
		{
			if (m_outermostKeepWithMemberPath != null)
			{
				RenderingDiagnostics.Trace(RenderingArea.KeepTogether, TraceLevel.Verbose, "PR-DIAG [Page {0}] Member '{1}' kept together - KeepWith - Page grown", pageNumber, m_outermostKeepWithMemberPath);
			}
		}

		private void TraceRepeatOnNewPage(int pageNumber, TablixMember rowMember)
		{
			RenderingDiagnostics.Trace(RenderingArea.RepeatOnNewPage, TraceLevel.Verbose, "PR-DIAG [Page {0}] '{1}' appears on page due to RepeatOnNewPage", pageNumber, DiagnosticsUtilities.BuildTablixMemberPath(base.ItemName, rowMember, m_memberIndexByLevel));
		}

		private void RegisterTablixMemberIndex(string definitionPath, int index)
		{
			if (definitionPath != null && !m_memberIndexByLevel.Contains(definitionPath))
			{
				m_memberIndexByLevel.Add(definitionPath, index);
			}
		}

		private bool RegisterOutermostKeepWithMember(TablixMember tablixMember)
		{
			if (m_outermostKeepWithMemberPath == null && tablixMember != null)
			{
				m_outermostKeepWithMemberPath = BuildTablixMemberPath(tablixMember);
				return true;
			}
			return false;
		}

		private void UnregisterOutermostKeepWithMember()
		{
			m_outermostKeepWithMemberPath = null;
		}

		private bool RegisterOutermostRepeatOnNewPage()
		{
			if (!m_repeatOnNewPageRegistered)
			{
				m_repeatOnNewPageRegistered = true;
				return true;
			}
			return false;
		}

		private void UnregisterRepeatOnNewPagePath()
		{
			m_repeatOnNewPageRegistered = false;
		}
	}
}
