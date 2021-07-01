using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.HPBProcessing
{
	internal abstract class PageItem : IStorable, IPersistable
	{
		[Flags]
		protected enum StateFlags : ushort
		{
			Clear = 0x0,
			OnThisVerticalPage = 0x1,
			KeepTogetherVert = 0x2,
			KeepTogetherHoriz = 0x4,
			UnresolvedPBS = 0x8,
			UnresolvedPBE = 0x10,
			UnresolvedKTV = 0x20,
			UnresolvedKTH = 0x40,
			UnresolvedCKTV = 0x80,
			UnresolvedCKTH = 0x100,
			UnresolvedCPB = 0x200,
			SplitsVerticalPage = 0x400,
			ResolveChildren = 0x800,
			Duplicate = 0x1000,
			NeedResolve = 0x2000,
			TablixCellTopItem = 0x4000,
			FullyCreated = 0x8000
		}

		[Flags]
		internal enum RepeatState : byte
		{
			None = 0x0,
			Vertical = 0x1,
			Horizontal = 0x2
		}

		[StaticReference]
		protected ReportItem m_source;

		protected ItemSizes m_itemPageSizes;

		protected PageBreakProperties m_pageBreakProperties;

		protected string m_pageName;

		protected List<int> m_pageItemsAbove;

		protected List<int> m_pageItemsLeft;

		protected StateFlags m_stateFlags;

		protected long m_offset;

		[StaticReference]
		protected RPLElement m_rplElement;

		protected long m_nonSharedOffset = -1L;

		protected byte m_rplItemState;

		private static Declaration m_declaration = GetDeclaration();

		internal ReportItem Source => m_source;

		internal virtual string SourceID => m_source.ID;

		internal virtual string SourceUniqueName => m_source.Instance.UniqueName;

		internal ItemSizes ItemPageSizes => m_itemPageSizes;

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

		public long Offset
		{
			get
			{
				return m_offset;
			}
			set
			{
				m_offset = value;
			}
		}

		public RPLElement RPLElement
		{
			get
			{
				return m_rplElement;
			}
			set
			{
				m_rplElement = value;
			}
		}

		internal byte RplItemState => m_rplItemState;

		internal PageBreakProperties PageBreakProperties => m_pageBreakProperties;

		internal bool PageBreakAtStart
		{
			get
			{
				if (m_pageBreakProperties == null)
				{
					return false;
				}
				if (IgnorePageBreaks)
				{
					return false;
				}
				return m_pageBreakProperties.PageBreakAtStart;
			}
		}

		internal bool PageBreakAtEnd
		{
			get
			{
				if (m_pageBreakProperties == null)
				{
					return false;
				}
				if (IgnorePageBreaks)
				{
					return false;
				}
				return m_pageBreakProperties.PageBreakAtEnd;
			}
		}

		internal string PageName
		{
			get
			{
				if (m_pageName != null && IgnorePageBreaks)
				{
					return null;
				}
				return m_pageName;
			}
		}

		internal bool IgnorePageBreaks
		{
			get
			{
				if (m_source == null)
				{
					return false;
				}
				if (m_source.Visibility == null)
				{
					return false;
				}
				if (m_source.Visibility.ToggleItem != null)
				{
					return true;
				}
				return false;
			}
		}

		internal bool OnThisVerticalPage
		{
			get
			{
				return GetFlagValue(StateFlags.OnThisVerticalPage);
			}
			set
			{
				SetFlagValue(StateFlags.OnThisVerticalPage, value);
			}
		}

		internal bool SplitsVerticalPage
		{
			get
			{
				return GetFlagValue(StateFlags.SplitsVerticalPage);
			}
			set
			{
				SetFlagValue(StateFlags.SplitsVerticalPage, value);
			}
		}

		internal bool Duplicate
		{
			get
			{
				return GetFlagValue(StateFlags.Duplicate);
			}
			set
			{
				SetFlagValue(StateFlags.Duplicate, value);
			}
		}

		internal bool ResolveChildren
		{
			get
			{
				return GetFlagValue(StateFlags.ResolveChildren);
			}
			set
			{
				SetFlagValue(StateFlags.ResolveChildren, value);
			}
		}

		internal bool KeepTogetherVertical
		{
			get
			{
				return GetFlagValue(StateFlags.KeepTogetherVert);
			}
			set
			{
				SetFlagValue(StateFlags.KeepTogetherVert, value);
			}
		}

		internal bool KeepTogetherHorizontal
		{
			get
			{
				return GetFlagValue(StateFlags.KeepTogetherHoriz);
			}
			set
			{
				SetFlagValue(StateFlags.KeepTogetherHoriz, value);
			}
		}

		internal bool UnresolvedPBS
		{
			get
			{
				return GetFlagValue(StateFlags.UnresolvedPBS);
			}
			set
			{
				SetFlagValue(StateFlags.UnresolvedPBS, value);
			}
		}

		internal bool UnresolvedPBE
		{
			get
			{
				return GetFlagValue(StateFlags.UnresolvedPBE);
			}
			set
			{
				SetFlagValue(StateFlags.UnresolvedPBE, value);
			}
		}

		internal bool UnresolvedKTV
		{
			get
			{
				return GetFlagValue(StateFlags.UnresolvedKTV);
			}
			set
			{
				SetFlagValue(StateFlags.UnresolvedKTV, value);
			}
		}

		internal bool UnresolvedKTH
		{
			get
			{
				return GetFlagValue(StateFlags.UnresolvedKTH);
			}
			set
			{
				SetFlagValue(StateFlags.UnresolvedKTH, value);
			}
		}

		internal bool UnresolvedCKTV
		{
			get
			{
				return GetFlagValue(StateFlags.UnresolvedCKTV);
			}
			set
			{
				SetFlagValue(StateFlags.UnresolvedCKTV, value);
			}
		}

		internal bool UnresolvedCKTH
		{
			get
			{
				return GetFlagValue(StateFlags.UnresolvedCKTH);
			}
			set
			{
				SetFlagValue(StateFlags.UnresolvedCKTH, value);
			}
		}

		internal bool UnresolvedCPB
		{
			get
			{
				return GetFlagValue(StateFlags.UnresolvedCPB);
			}
			set
			{
				SetFlagValue(StateFlags.UnresolvedCPB, value);
			}
		}

		internal bool NeedResolve
		{
			get
			{
				return GetFlagValue(StateFlags.NeedResolve);
			}
			set
			{
				SetFlagValue(StateFlags.NeedResolve, value);
			}
		}

		internal bool TablixCellTopItem
		{
			get
			{
				return GetFlagValue(StateFlags.TablixCellTopItem);
			}
			set
			{
				SetFlagValue(StateFlags.TablixCellTopItem, value);
			}
		}

		internal bool FullyCreated
		{
			get
			{
				return GetFlagValue(StateFlags.FullyCreated);
			}
			set
			{
				SetFlagValue(StateFlags.FullyCreated, value);
			}
		}

		internal bool PBAreUnresolved
		{
			get
			{
				if (UnresolvedPBS || UnresolvedPBE)
				{
					return true;
				}
				if (UnresolvedCPB)
				{
					return true;
				}
				return false;
			}
		}

		internal bool KTVIsUnresolved
		{
			get
			{
				if (UnresolvedKTV)
				{
					return true;
				}
				if (UnresolvedCKTV)
				{
					return true;
				}
				return false;
			}
		}

		internal bool KTHIsUnresolved
		{
			get
			{
				if (UnresolvedKTH)
				{
					return true;
				}
				if (UnresolvedCKTH)
				{
					return true;
				}
				return false;
			}
		}

		internal virtual double OriginalLeft => Source.Left.ToMillimeters();

		internal virtual double OriginalWidth => Source.Width.ToMillimeters();

		internal virtual double OriginalRight => OriginalLeft + OriginalWidth;

		internal virtual Style SharedStyle => Source.Style;

		internal virtual StyleInstance NonSharedStyle => Source.Instance.Style;

		public virtual int Size => 2 * Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.ReferenceSize + 1 + 16 + 2 + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(m_itemPageSizes) + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(m_pageBreakProperties) + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(m_pageItemsAbove) + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(m_pageItemsLeft) + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(m_pageName);

		internal virtual bool ContentOnPage => true;

		internal PageItem()
		{
		}

		protected PageItem(ReportItem source)
		{
			m_source = source;
			KeepTogetherHorizontal = true;
			KeepTogetherVertical = true;
			UnresolvedKTH = (UnresolvedKTV = true);
			FullyCreated = true;
		}

		private bool GetFlagValue(StateFlags aFlag)
		{
			return (int)(m_stateFlags & aFlag) > 0;
		}

		private void SetFlagValue(StateFlags aFlag, bool aValue)
		{
			if (aValue)
			{
				m_stateFlags |= aFlag;
			}
			else
			{
				m_stateFlags &= (StateFlags)(ushort)(~(uint)aFlag);
			}
		}

		public virtual void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_declaration);
			IScalabilityCache scalabilityCache = writer.PersistenceHelper as IScalabilityCache;
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Source:
				{
					int value2 = scalabilityCache.StoreStaticReference(m_source);
					writer.Write(value2);
					break;
				}
				case MemberName.ItemPageSizes:
					writer.Write(m_itemPageSizes);
					break;
				case MemberName.PageBreakProperties:
					writer.Write(m_pageBreakProperties);
					break;
				case MemberName.PageName:
					writer.Write(m_pageName);
					break;
				case MemberName.ItemsAbove:
					writer.WriteListOfPrimitives(m_pageItemsAbove);
					break;
				case MemberName.ItemsLeft:
					writer.WriteListOfPrimitives(m_pageItemsLeft);
					break;
				case MemberName.State:
					writer.Write((ushort)m_stateFlags);
					break;
				case MemberName.Offset:
					writer.Write(m_offset);
					break;
				case MemberName.RPLElement:
				{
					int value = scalabilityCache.StoreStaticReference(m_rplElement);
					writer.Write(value);
					break;
				}
				case MemberName.NonSharedOffset:
					writer.Write(m_nonSharedOffset);
					break;
				case MemberName.RPLState:
					writer.Write(m_rplItemState);
					break;
				default:
					RSTrace.RenderingTracer.Assert(condition: false, string.Empty);
					break;
				}
			}
		}

		public virtual void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_declaration);
			IScalabilityCache scalabilityCache = reader.PersistenceHelper as IScalabilityCache;
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Source:
				{
					int id2 = reader.ReadInt32();
					m_source = (ReportItem)scalabilityCache.FetchStaticReference(id2);
					break;
				}
				case MemberName.ItemPageSizes:
					m_itemPageSizes = (ItemSizes)reader.ReadRIFObject();
					break;
				case MemberName.PageBreakProperties:
					m_pageBreakProperties = (PageBreakProperties)reader.ReadRIFObject();
					break;
				case MemberName.PageName:
					m_pageName = reader.ReadString();
					break;
				case MemberName.ItemsAbove:
					m_pageItemsAbove = reader.ReadListOfPrimitives<int>();
					break;
				case MemberName.ItemsLeft:
					m_pageItemsLeft = reader.ReadListOfPrimitives<int>();
					break;
				case MemberName.State:
					m_stateFlags = (StateFlags)reader.ReadUInt16();
					break;
				case MemberName.Offset:
					m_offset = reader.ReadInt64();
					break;
				case MemberName.RPLElement:
				{
					int id = reader.ReadInt32();
					m_rplElement = (RPLElement)scalabilityCache.FetchStaticReference(id);
					break;
				}
				case MemberName.NonSharedOffset:
					m_nonSharedOffset = reader.ReadInt64();
					break;
				case MemberName.RPLState:
					m_rplItemState = reader.ReadByte();
					break;
				default:
					RSTrace.RenderingTracer.Assert(condition: false, string.Empty);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public virtual ObjectType GetObjectType()
		{
			return ObjectType.PageItem;
		}

		internal static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.Source, Token.Int32));
				list.Add(new MemberInfo(MemberName.ItemPageSizes, ObjectType.ItemSizes));
				list.Add(new MemberInfo(MemberName.PageBreakProperties, ObjectType.PageBreakProperties));
				list.Add(new MemberInfo(MemberName.PageName, ObjectType.String));
				list.Add(new MemberInfo(MemberName.ItemsAbove, ObjectType.PrimitiveList, Token.Int32));
				list.Add(new MemberInfo(MemberName.ItemsLeft, ObjectType.PrimitiveList, Token.Int32));
				list.Add(new MemberInfo(MemberName.State, Token.UInt16));
				list.Add(new MemberInfo(MemberName.Offset, Token.Int64));
				list.Add(new MemberInfo(MemberName.RPLElement, Token.Int32));
				list.Add(new MemberInfo(MemberName.NonSharedOffset, Token.Int64));
				list.Add(new MemberInfo(MemberName.RPLState, Token.Byte));
				return new Declaration(ObjectType.PageItem, ObjectType.None, list);
			}
			return m_declaration;
		}

		internal static PageItem Create(ReportItem source, bool tablixCellParent, PageContext pageContext)
		{
			return Create(source, tablixCellParent, ignoreKT: false, pageContext);
		}

		internal static PageItem Create(ReportItem source, bool tablixCellParent, bool ignoreKT, PageContext pageContext)
		{
			if (source == null)
			{
				return null;
			}
			PageItem pageItem = null;
			bool flag = true;
			if (source.Visibility != null && source.Instance.Visibility.CurrentlyHidden)
			{
				flag = false;
				pageItem = new HiddenPageItem(source, pageContext, checkHiddenState: false);
			}
			else if (source is Microsoft.ReportingServices.OnDemandReportRendering.TextBox)
			{
				pageItem = new TextBox((Microsoft.ReportingServices.OnDemandReportRendering.TextBox)source, pageContext);
			}
			else if (source is Microsoft.ReportingServices.OnDemandReportRendering.Line)
			{
				pageItem = new Line((Microsoft.ReportingServices.OnDemandReportRendering.Line)source);
			}
			else if (source is Microsoft.ReportingServices.OnDemandReportRendering.Image)
			{
				pageItem = new Image((Microsoft.ReportingServices.OnDemandReportRendering.Image)source);
			}
			else if (source is Microsoft.ReportingServices.OnDemandReportRendering.Chart)
			{
				Microsoft.ReportingServices.OnDemandReportRendering.Chart source2 = (Microsoft.ReportingServices.OnDemandReportRendering.Chart)source;
				pageItem = ((!TransformToTextBox(source2)) ? ((PageItem)new Chart(source2, pageContext)) : ((PageItem)new TextBox(source2, pageContext)));
			}
			else if (source is Microsoft.ReportingServices.OnDemandReportRendering.GaugePanel)
			{
				Microsoft.ReportingServices.OnDemandReportRendering.GaugePanel source3 = (Microsoft.ReportingServices.OnDemandReportRendering.GaugePanel)source;
				pageItem = ((!TransformToTextBox(source3)) ? ((PageItem)new GaugePanel(source3, pageContext)) : ((PageItem)new TextBox(source3, pageContext)));
			}
			else if (source is Microsoft.ReportingServices.OnDemandReportRendering.Map)
			{
				pageItem = new Map((Microsoft.ReportingServices.OnDemandReportRendering.Map)source, pageContext);
			}
			else if (source is Microsoft.ReportingServices.OnDemandReportRendering.Tablix)
			{
				bool noRows = false;
				Microsoft.ReportingServices.OnDemandReportRendering.Tablix source4 = (Microsoft.ReportingServices.OnDemandReportRendering.Tablix)source;
				if (TransformToTextBox(source4, tablixCellParent, ref noRows))
				{
					pageItem = new TextBox(source4, pageContext);
				}
				else if (noRows)
				{
					flag = false;
					pageItem = new NoRowsItem(source4);
				}
				else
				{
					pageItem = new Tablix(source4, pageContext);
					pageContext.InitCache();
				}
			}
			else if (source is Microsoft.ReportingServices.OnDemandReportRendering.Rectangle)
			{
				pageItem = new Rectangle((Microsoft.ReportingServices.OnDemandReportRendering.Rectangle)source, pageContext);
			}
			else if (source is Microsoft.ReportingServices.OnDemandReportRendering.SubReport)
			{
				bool noRows2 = false;
				Microsoft.ReportingServices.OnDemandReportRendering.SubReport source5 = (Microsoft.ReportingServices.OnDemandReportRendering.SubReport)source;
				if (TransformToTextBox(source5, tablixCellParent, ref noRows2))
				{
					pageItem = new TextBox(source5, pageContext);
				}
				else if (noRows2)
				{
					flag = false;
					pageItem = new NoRowsItem(source5);
				}
				else
				{
					pageItem = new SubReport(source5);
				}
			}
			if (flag)
			{
				pageItem.CacheNonSharedProperties(pageContext);
			}
			if (ignoreKT)
			{
				pageItem.KeepTogetherHorizontal = false;
				pageItem.KeepTogetherVertical = false;
				PageItem pageItem2 = pageItem;
				bool unresolvedKTV = pageItem.UnresolvedKTH = false;
				pageItem2.UnresolvedKTV = unresolvedKTV;
			}
			pageItem.TablixCellTopItem = tablixCellParent;
			return pageItem;
		}

		internal static bool TransformToTextBox(DataRegion source)
		{
			bool result = false;
			DataRegionInstance dataRegionInstance = (DataRegionInstance)source.Instance;
			if (dataRegionInstance.NoRows && source.NoRowsMessage != null)
			{
				string text = null;
				text = ((!source.NoRowsMessage.IsExpression) ? source.NoRowsMessage.Value : dataRegionInstance.NoRowsMessage);
				if (text != null)
				{
					result = true;
				}
			}
			return result;
		}

		internal static bool TransformToTextBox(Microsoft.ReportingServices.OnDemandReportRendering.Tablix source, bool tablixCellParent, ref bool noRows)
		{
			bool flag = false;
			TablixInstance tablixInstance = (TablixInstance)source.Instance;
			if (tablixInstance.NoRows)
			{
				noRows = true;
				if (source.NoRowsMessage != null)
				{
					string text = null;
					text = ((!source.NoRowsMessage.IsExpression) ? source.NoRowsMessage.Value : tablixInstance.NoRowsMessage);
					if (text != null)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					if (!source.HideStaticsIfNoRows)
					{
						noRows = false;
					}
					else if (tablixCellParent)
					{
						flag = true;
					}
				}
			}
			return flag;
		}

		internal static bool TransformToTextBox(Microsoft.ReportingServices.OnDemandReportRendering.SubReport source, bool tablixCellParent, ref bool noRows)
		{
			bool result = false;
			SubReportInstance subReportInstance = (SubReportInstance)source.Instance;
			if (subReportInstance.NoRows)
			{
				noRows = true;
				if (tablixCellParent)
				{
					result = true;
				}
				else if (source.NoRowsMessage != null)
				{
					string text = null;
					text = ((!source.NoRowsMessage.IsExpression) ? source.NoRowsMessage.Value : subReportInstance.NoRowsMessage);
					if (text != null)
					{
						result = true;
					}
				}
			}
			else if (subReportInstance.ProcessedWithError)
			{
				result = true;
			}
			return result;
		}

		internal void CalculateVertical(PageContext pageContext, double topInParentSystem, double bottomInParentSystem, PageItem[] siblings, List<PageItem> ancestors, ref bool anyAncestorHasKT, bool hasUnpinnedAncestors)
		{
			CalculateVertical(pageContext, topInParentSystem, bottomInParentSystem, siblings, ancestors, ref anyAncestorHasKT, hasUnpinnedAncestors, null);
		}

		internal void CalculateVertical(PageContext pageContext, double topInParentSystem, double bottomInParentSystem, PageItem[] siblings, List<PageItem> ancestors, ref bool anyAncestorHasKT, bool hasUnpinnedAncestors, double? sourceWidth)
		{
			AdjustOriginFromItemsAbove(siblings);
			OnThisVerticalPage = false;
			UnresolvedCKTV = false;
			UnresolvedCPB = false;
			ResolveChildren = true;
			if (pageContext.IgnorePageBreaks)
			{
				bool unresolvedPBE = UnresolvedPBS = false;
				UnresolvedPBE = unresolvedPBE;
			}
			else if (anyAncestorHasKT && (PageBreakAtStart || PageBreakAtEnd))
			{
				anyAncestorHasKT = false;
				foreach (PageItem ancestor in ancestors)
				{
					ancestor.KeepTogetherVertical = false;
					ancestor.UnresolvedKTV = false;
				}
			}
			if (pageContext.FullOnPage || hasUnpinnedAncestors)
			{
				UnresolvedKTV = KeepTogetherVertical;
				if (!pageContext.IgnorePageBreaks)
				{
					UnresolvedPBS = PageBreakAtStart;
					UnresolvedPBE = PageBreakAtEnd;
				}
			}
			double bottomInParentSystem2 = anyAncestorHasKT ? (topInParentSystem + pageContext.ColumnHeight) : bottomInParentSystem;
			if (!HitsCurrentVerticalPage(topInParentSystem, bottomInParentSystem2) || (!anyAncestorHasKT && !hasUnpinnedAncestors && !pageContext.FullOnPage && ResolvePageBreakAtStart(pageContext, topInParentSystem, bottomInParentSystem)))
			{
				return;
			}
			if (new RoundedDouble(ItemPageSizes.Top) < topInParentSystem)
			{
				ResetHorizontal(spanPages: true, sourceWidth);
			}
			OnThisVerticalPage = true;
			bool canOverwritePageBreak = pageContext.Common.CanOverwritePageBreak;
			bool canSetPageName = pageContext.Common.CanSetPageName;
			DetermineVerticalSize(pageContext, topInParentSystem, bottomInParentSystem, ancestors, ref anyAncestorHasKT, hasUnpinnedAncestors);
			if (InvalidateVerticalKT(anyAncestorHasKT, pageContext))
			{
				if (anyAncestorHasKT)
				{
					anyAncestorHasKT = false;
					foreach (PageItem ancestor2 in ancestors)
					{
						ancestor2.KeepTogetherVertical = false;
						ancestor2.UnresolvedKTV = false;
					}
				}
				KeepTogetherVertical = false;
				UnresolvedKTV = false;
			}
			if (anyAncestorHasKT)
			{
				UnresolvedKTV = KeepTogetherVertical;
			}
			if (!anyAncestorHasKT && !hasUnpinnedAncestors && !pageContext.FullOnPage)
			{
				ResolveVertical(pageContext, topInParentSystem, bottomInParentSystem, null, resolveItem: false, canOverwritePageBreak, canSetPageName);
			}
		}

		protected virtual bool InvalidateVerticalKT(bool anyAncestorHasKT, PageContext pageContext)
		{
			if ((anyAncestorHasKT || KeepTogetherVertical) && new RoundedDouble(ItemPageSizes.Height) > pageContext.ColumnHeight)
			{
				TraceInvalidatedKeepTogetherVertical(pageContext);
				return true;
			}
			return false;
		}

		protected virtual void DetermineVerticalSize(PageContext pageContext, double topInParentSystem, double bottomInParentSystem, List<PageItem> ancestors, ref bool anyAncestorHasKT, bool hasUnpinnedAncestors)
		{
		}

		internal virtual bool ResolveDuplicates(PageContext pageContext, double topInParentSystem, PageItem[] siblings, bool recalculate)
		{
			if (recalculate)
			{
				AdjustOriginFromItemsAbove(siblings);
			}
			return false;
		}

		internal void ResolveVertical(PageContext pageContext, double topInParentSystem, double bottomInParentSystem, PageItem[] siblings, bool resolveItem, bool canOverwritePageBreak, bool canSetPageName)
		{
			if (resolveItem && NeedResolve)
			{
				ResolveChildren = true;
			}
			if (!ResolveChildren)
			{
				return;
			}
			ResolveChildren = false;
			AdjustOriginFromItemsAbove(siblings);
			OnThisVerticalPage = false;
			if (!HitsCurrentVerticalPage(topInParentSystem, bottomInParentSystem) || ResolvePageBreakAtStart(pageContext, topInParentSystem, bottomInParentSystem))
			{
				return;
			}
			OnThisVerticalPage = true;
			ResolveKeepTogetherVertical(pageContext, topInParentSystem, bottomInParentSystem, resolveItem, canOverwritePageBreak, canSetPageName);
			if (!UnresolvedPBE)
			{
				return;
			}
			if (PageBreakAtEnd)
			{
				if (ItemPageSizes.Bottom <= bottomInParentSystem)
				{
					double num = bottomInParentSystem - ItemPageSizes.Bottom;
					ItemPageSizes.DeltaY += num;
					UnresolvedPBE = false;
					pageContext.Common.RegisterPageBreakProperties(PageBreakProperties, canOverwritePageBreak);
					if (pageContext.Common.DiagnosticsEnabled && num == 0.0)
					{
						pageContext.Common.TracePageBreakIgnoredAtBottomOfPage(this);
					}
				}
			}
			else
			{
				UnresolvedPBE = false;
			}
		}

		internal void CalculateHorizontal(PageContext pageContext, double leftInParentSystem, double rightInParentSystem, PageItem[] siblings, List<PageItem> ancestors, ref bool anyAncestorHasKT, bool hasUnpinnedAncestors)
		{
			CalculateHorizontal(pageContext, leftInParentSystem, rightInParentSystem, siblings, ancestors, ref anyAncestorHasKT, hasUnpinnedAncestors, null);
		}

		internal void CalculateHorizontal(PageContext pageContext, double leftInParentSystem, double rightInParentSystem, PageItem[] siblings, List<PageItem> ancestors, ref bool anyAncestorHasKT, bool hasUnpinnedAncestors, double? sourceWidth)
		{
			if (pageContext.FullOnPage || hasUnpinnedAncestors)
			{
				UnresolvedKTH = KeepTogetherHorizontal;
			}
			ResolveChildren = true;
			if (!OnThisVerticalPage)
			{
				return;
			}
			if (pageContext.ResetHorizontal)
			{
				ResetHorizontal(spanPages: false, sourceWidth);
			}
			AdjustOriginFromItemsAtLeft(siblings);
			double rightInParentSystem2 = anyAncestorHasKT ? (leftInParentSystem + pageContext.ColumnWidth) : rightInParentSystem;
			if (!HitsCurrentHorizontalPage(leftInParentSystem, rightInParentSystem2))
			{
				return;
			}
			DetermineHorizontalSize(pageContext, leftInParentSystem, rightInParentSystem, ancestors, anyAncestorHasKT, hasUnpinnedAncestors);
			if (InvalidateHorizontalKT(anyAncestorHasKT, pageContext))
			{
				if (anyAncestorHasKT)
				{
					anyAncestorHasKT = false;
					foreach (PageItem ancestor in ancestors)
					{
						ancestor.KeepTogetherHorizontal = false;
						ancestor.UnresolvedKTH = false;
					}
				}
				KeepTogetherHorizontal = false;
				UnresolvedKTH = false;
			}
			if (anyAncestorHasKT)
			{
				UnresolvedKTH = KeepTogetherHorizontal;
			}
			if (!anyAncestorHasKT && !hasUnpinnedAncestors && !pageContext.FullOnPage)
			{
				ResolveHorizontal(pageContext, leftInParentSystem, rightInParentSystem, null, resolveItem: false);
			}
		}

		protected virtual bool InvalidateHorizontalKT(bool anyAncestorHasKT, PageContext pageContext)
		{
			if ((anyAncestorHasKT || KeepTogetherHorizontal) && new RoundedDouble(ItemPageSizes.Width) > pageContext.ColumnWidth)
			{
				TraceInvalidatedKeepTogetherHorizontal(pageContext);
				return true;
			}
			return false;
		}

		protected virtual void DetermineHorizontalSize(PageContext pageContext, double leftInParentSystem, double rightInParentSystem, List<PageItem> ancestors, bool anyAncestorHasKT, bool hasUnpinnedAncestors)
		{
		}

		internal void ResolveHorizontal(PageContext pageContext, double leftInParentSystem, double rightInParentSystem, PageItem[] siblings, bool resolveItem)
		{
			if (resolveItem && NeedResolve)
			{
				ResolveChildren = true;
			}
			if (!ResolveChildren)
			{
				return;
			}
			ResolveChildren = false;
			if (OnThisVerticalPage)
			{
				AdjustOriginFromItemsAtLeft(siblings);
				if (HitsCurrentHorizontalPage(leftInParentSystem, rightInParentSystem))
				{
					ResolveKeepTogetherHorizontal(pageContext, leftInParentSystem, rightInParentSystem, resolveItem);
				}
			}
		}

		private bool ResolvePageBreakAtStart(PageContext pageContext, double topInParentSystem, double bottomInParentSystem)
		{
			if (UnresolvedPBS)
			{
				UnresolvedPBS = false;
				if (PageBreakAtStart)
				{
					if (ItemPageSizes.Top > topInParentSystem)
					{
						RoundedDouble roundedDouble = new RoundedDouble(bottomInParentSystem - ItemPageSizes.Top);
						if (roundedDouble < pageContext.ColumnHeight)
						{
							ItemPageSizes.MoveVertical(roundedDouble.Value);
							pageContext.Common.RegisterPageBreakProperties(PageBreakProperties, overwrite: false);
							return true;
						}
					}
					if (pageContext.Common.DiagnosticsEnabled && ItemPageSizes.Top == topInParentSystem)
					{
						pageContext.Common.TracePageBreakIgnoredAtTopOfPage(this);
					}
				}
			}
			return false;
		}

		protected bool StartsOnThisPage(PageContext pageContext, double topInParentSystem, double bottomInParentSystem)
		{
			if (OnThisVerticalPage && ItemPageSizes.Top >= topInParentSystem)
			{
				return true;
			}
			return false;
		}

		protected virtual bool ResolveKeepTogetherVertical(PageContext pageContext, double topInParentSystem, double bottomInParentSystem, bool resolveItem, bool canOverwritePageBreak, bool canSetPageName)
		{
			if (!UnresolvedKTV)
			{
				if (StartsOnThisPage(pageContext, topInParentSystem, bottomInParentSystem))
				{
					pageContext.Common.SetPageName(PageName, canSetPageName);
				}
				return false;
			}
			UnresolvedKTV = false;
			if (KeepTogetherVertical)
			{
				double num = bottomInParentSystem - ItemPageSizes.Top;
				RoundedDouble x = new RoundedDouble(ItemPageSizes.Height);
				if (x <= num)
				{
					if (StartsOnThisPage(pageContext, topInParentSystem, bottomInParentSystem))
					{
						pageContext.Common.SetPageName(PageName, canSetPageName);
					}
					return true;
				}
				if (x <= pageContext.ColumnHeight)
				{
					ItemPageSizes.MoveVertical(num);
					OnThisVerticalPage = false;
					TraceKeepTogetherVertical(pageContext);
					return true;
				}
				TraceInvalidatedKeepTogetherVertical(pageContext);
			}
			if (StartsOnThisPage(pageContext, topInParentSystem, bottomInParentSystem))
			{
				pageContext.Common.SetPageName(PageName, canSetPageName);
			}
			return false;
		}

		protected virtual bool ResolveKeepTogetherHorizontal(PageContext pageContext, double leftInParentSystem, double rightInParentSystem, bool resolveItem)
		{
			if (!UnresolvedKTH)
			{
				return false;
			}
			UnresolvedKTH = false;
			if (KeepTogetherHorizontal)
			{
				double num = rightInParentSystem - ItemPageSizes.Left;
				RoundedDouble x = new RoundedDouble(ItemPageSizes.Width);
				if (x <= num)
				{
					return true;
				}
				if (x <= pageContext.ColumnWidth)
				{
					ItemPageSizes.MoveHorizontal(num);
					TraceKeepTogetherHorizontal(pageContext);
					return true;
				}
				TraceInvalidatedKeepTogetherHorizontal(pageContext);
			}
			return false;
		}

		internal bool AboveCurrentVerticalPage(double topInParentSystem)
		{
			RoundedDouble x = new RoundedDouble(ItemPageSizes.Bottom);
			if (x < topInParentSystem)
			{
				return true;
			}
			if (x == topInParentSystem && ItemPageSizes.Height > 0.0)
			{
				return true;
			}
			return false;
		}

		internal bool HitsCurrentVerticalPage(double topInParentSystem, double bottomInParentSystem)
		{
			RoundedDouble roundedDouble = new RoundedDouble(ItemPageSizes.Bottom);
			if (roundedDouble < topInParentSystem)
			{
				return false;
			}
			if (roundedDouble == topInParentSystem && ItemPageSizes.Height > 0.0)
			{
				return false;
			}
			roundedDouble.Value = ItemPageSizes.Top;
			if (roundedDouble >= bottomInParentSystem)
			{
				return false;
			}
			return true;
		}

		internal bool HitsCurrentHorizontalPage(double leftInParentSystem, double rightInParentSystem)
		{
			RoundedDouble roundedDouble = new RoundedDouble(ItemPageSizes.Right);
			if (roundedDouble < leftInParentSystem)
			{
				return false;
			}
			if (roundedDouble == leftInParentSystem && ItemPageSizes.Width > 0.0)
			{
				return false;
			}
			roundedDouble.Value = ItemPageSizes.Left;
			if (roundedDouble >= rightInParentSystem)
			{
				return false;
			}
			return true;
		}

		internal virtual bool HitsCurrentPage(double pageLeft, double pageTop, double pageRight, double pageBottom)
		{
			if (HitsCurrentVerticalPage(pageTop, pageBottom) && HitsCurrentHorizontalPage(pageLeft, pageRight))
			{
				return true;
			}
			return false;
		}

		internal bool Release(double pageBottom, double pageRight)
		{
			RoundedDouble roundedDouble = new RoundedDouble(ItemPageSizes.Bottom);
			if (roundedDouble <= pageBottom)
			{
				roundedDouble.Value = ItemPageSizes.Right;
				if (roundedDouble <= pageRight)
				{
					return true;
				}
			}
			return false;
		}

		internal virtual bool AddToPage(RPLWriter rplWriter, PageContext pageContext, double pageLeft, double pageTop, double pageRight, double pageBottom, RepeatState repeatState)
		{
			if (HitsCurrentPage(pageLeft, pageTop, pageRight, pageBottom))
			{
				WriteStartItemToStream(rplWriter, pageContext);
				RegisterTextBoxes(rplWriter, pageContext);
				OmitBorderOnPageBreak(rplWriter, pageLeft, pageTop, pageRight, pageBottom);
				return true;
			}
			return false;
		}

		internal virtual void RegisterTextBoxes(RPLWriter rplWriter, PageContext pageContext)
		{
		}

		internal virtual void ResetHorizontal(bool spanPages, double? width)
		{
			ItemPageSizes.Left = OriginalLeft;
			if (width.HasValue)
			{
				ItemPageSizes.Width = width.Value;
			}
			else
			{
				ItemPageSizes.Width = OriginalWidth;
			}
		}

		internal virtual void OmitBorderOnPageBreak(RPLWriter rplWriter, double pageLeft, double pageTop, double pageRight, double pageBottom)
		{
		}

		internal void OmitBorderOnPageBreak(double pageLeft, double pageTop, double pageRight, double pageBottom)
		{
			byte b = 15;
			b = (byte)(~b);
			m_rplItemState &= b;
			if (new RoundedDouble(m_itemPageSizes.Top) < pageTop)
			{
				m_rplItemState |= 1;
			}
			if (new RoundedDouble(m_itemPageSizes.Left) < pageLeft)
			{
				m_rplItemState |= 4;
			}
			if (new RoundedDouble(m_itemPageSizes.Bottom) > pageBottom)
			{
				m_rplItemState |= 2;
			}
			if (new RoundedDouble(m_itemPageSizes.Right) > pageRight)
			{
				m_rplItemState |= 8;
			}
		}

		internal void AdjustOriginFromItemsAtLeft(PageItem[] siblings)
		{
			if (m_pageItemsLeft == null || siblings == null)
			{
				return;
			}
			double num = double.MinValue;
			double num2 = double.MaxValue;
			double num3 = double.MaxValue;
			double num4 = double.MaxValue;
			double num5 = 0.0;
			PageItem pageItem = null;
			ItemSizes itemSizes = null;
			for (int i = 0; i < m_pageItemsLeft.Count; i++)
			{
				pageItem = siblings[m_pageItemsLeft[i]];
				if (pageItem == null)
				{
					m_pageItemsLeft.RemoveAt(i);
					i--;
				}
				else if (pageItem.OnThisVerticalPage)
				{
					itemSizes = pageItem.ItemPageSizes;
					num5 = ItemPageSizes.Left - (itemSizes.Right - itemSizes.DeltaX);
					if ((RoundedDouble)itemSizes.DeltaX < num4)
					{
						num4 = itemSizes.DeltaX;
					}
					if ((RoundedDouble)itemSizes.DeltaX > num)
					{
						num = itemSizes.DeltaX;
						num3 = num5;
					}
					else if ((RoundedDouble)itemSizes.DeltaX == num)
					{
						num3 = Math.Min(num3, num5);
					}
					num2 = Math.Min(num2, num5);
				}
			}
			if (num != double.MinValue)
			{
				if (num2 < 0.01 || num2 == double.MaxValue)
				{
					num2 = 0.0;
				}
				if (num3 < 0.01 || num3 == double.MaxValue)
				{
					num3 = 0.0;
				}
				num += num2 - num3;
				if (num < num4)
				{
					num = num4;
				}
				ItemPageSizes.Left += num;
				ItemPageSizes.DeltaX += num;
			}
		}

		internal void AdjustOriginFromItemsAbove(PageItem[] siblings)
		{
			if (m_pageItemsAbove == null || siblings == null)
			{
				return;
			}
			double num = double.MinValue;
			double num2 = double.MaxValue;
			double num3 = double.MaxValue;
			double num4 = double.MaxValue;
			double num5 = 0.0;
			PageItem pageItem = null;
			ItemSizes itemSizes = null;
			for (int i = 0; i < m_pageItemsAbove.Count; i++)
			{
				pageItem = siblings[m_pageItemsAbove[i]];
				if (pageItem == null)
				{
					m_pageItemsAbove.RemoveAt(i);
					i--;
					continue;
				}
				itemSizes = pageItem.ItemPageSizes;
				num5 = ItemPageSizes.Top - (itemSizes.Bottom - itemSizes.DeltaY);
				if ((RoundedDouble)itemSizes.DeltaY < num3)
				{
					num3 = itemSizes.DeltaY;
				}
				if ((RoundedDouble)itemSizes.DeltaY > num)
				{
					num = itemSizes.DeltaY;
					num4 = num5;
				}
				else if ((RoundedDouble)itemSizes.DeltaY == num)
				{
					num4 = Math.Min(num4, num5);
				}
				num2 = Math.Min(num2, num5);
			}
			if (num != double.MinValue)
			{
				if (num2 < 0.01 || num2 == double.MaxValue)
				{
					num2 = 0.0;
				}
				if (num4 < 0.01 || num4 == double.MaxValue)
				{
					num4 = 0.0;
				}
				num += num2 - num4;
				if (num < num3)
				{
					num = num3;
				}
				ItemPageSizes.Top += num;
				ItemPageSizes.DeltaY += num;
			}
			if (m_pageItemsAbove.Count == 0)
			{
				m_pageItemsAbove = null;
			}
		}

		protected void TraceKeepTogetherHorizontal(PageContext pageContext)
		{
			if (pageContext.Common.DiagnosticsEnabled && Source != null)
			{
				RenderingDiagnostics.Trace(RenderingArea.KeepTogether, TraceLevel.Info, "PR-DIAG [Page {0}] Item '{1}' kept together horizontally - Explicit - Pushed to next page", pageContext.Common.PageNumber, Source.Name);
			}
		}

		protected void TraceKeepTogetherVertical(PageContext pageContext)
		{
			if (pageContext.Common.DiagnosticsEnabled && Source != null)
			{
				RenderingDiagnostics.Trace(RenderingArea.KeepTogether, TraceLevel.Info, "PR-DIAG [Page {0}] Item '{1}' kept together vertically - Explicit - Pushed to next page", pageContext.Common.PageNumber, Source.Name);
			}
		}

		protected void TraceInvalidatedKeepTogetherHorizontal(PageContext pageContext)
		{
			if (pageContext.Common.DiagnosticsEnabled && Source != null)
			{
				RenderingDiagnostics.Trace(RenderingArea.KeepTogether, TraceLevel.Info, "PR-DIAG [Page {0}] Horizontal KeepTogether on Item '{1}' not honored - larger than page", pageContext.Common.PageNumber, Source.Name);
			}
		}

		protected void TraceInvalidatedKeepTogetherVertical(PageContext pageContext)
		{
			if (pageContext.Common.DiagnosticsEnabled && Source != null)
			{
				RenderingDiagnostics.Trace(RenderingArea.KeepTogether, TraceLevel.Info, "PR-DIAG [Page {0}] Vertical KeepTogether on Item '{1}' not honored - larger than page", pageContext.Common.PageNumber, Source.Name);
			}
		}

		internal void WritePageItemSizes(BinaryWriter spbifWriter)
		{
			spbifWriter.Write((float)m_itemPageSizes.Left);
			spbifWriter.Write((float)m_itemPageSizes.Top);
			spbifWriter.Write((float)m_itemPageSizes.Width);
			spbifWriter.Write((float)m_itemPageSizes.Height);
			if (m_source == null)
			{
				spbifWriter.Write(0);
			}
			else
			{
				spbifWriter.Write(m_source.ZIndex);
			}
			spbifWriter.Write(m_rplItemState);
			spbifWriter.Write(m_offset);
		}

		internal RPLItemMeasurement WritePageItemSizes()
		{
			RPLItemMeasurement rPLItemMeasurement = new RPLItemMeasurement();
			rPLItemMeasurement.Left = (float)m_itemPageSizes.Left;
			rPLItemMeasurement.Top = (float)m_itemPageSizes.Top;
			rPLItemMeasurement.Width = (float)m_itemPageSizes.Width;
			rPLItemMeasurement.Height = (float)m_itemPageSizes.Height;
			if (m_source != null)
			{
				rPLItemMeasurement.ZIndex = m_source.ZIndex;
			}
			rPLItemMeasurement.State = m_rplItemState;
			rPLItemMeasurement.Element = (m_rplElement as RPLItem);
			return rPLItemMeasurement;
		}

		internal abstract void WriteStartItemToStream(RPLWriter rplWriter, PageContext pageContext);

		internal virtual void CacheNonSharedProperties(PageContext pageContext)
		{
			if (!pageContext.CacheNonSharedProps)
			{
				return;
			}
			m_nonSharedOffset = 0L;
			if (pageContext.PropertyCacheState == PageContext.CacheState.CountPages)
			{
				return;
			}
			if (pageContext.PropertyCacheState == PageContext.CacheState.RPLStream)
			{
				BinaryWriter propertyCacheWriter = pageContext.PropertyCacheWriter;
				long position = propertyCacheWriter.BaseStream.Position;
				WriteNonSharedItemProps(propertyCacheWriter, null, pageContext);
				long position2 = propertyCacheWriter.BaseStream.Position;
				if (pageContext.ItemCacheSharedImageInfo != null)
				{
					propertyCacheWriter.Write(pageContext.ItemCacheSharedImageInfo.StreamName);
					propertyCacheWriter.Write(pageContext.ItemCacheSharedImageInfo.ImageBounderies.StartOffset);
					propertyCacheWriter.Write(pageContext.ItemCacheSharedImageInfo.ImageBounderies.EndOffset);
					pageContext.ItemCacheSharedImageInfo = null;
				}
				m_nonSharedOffset = propertyCacheWriter.BaseStream.Position;
				propertyCacheWriter.Write(position);
				propertyCacheWriter.Write(position2);
			}
			else
			{
				WriteStartItemToStream(new RPLWriter(), pageContext);
			}
		}

		internal void WriteElementProps(BinaryWriter spbifWriter, RPLWriter rplWriter, PageContext pageContext, long offset)
		{
			spbifWriter.Write((byte)15);
			WriteSharedItemProps(spbifWriter, rplWriter, pageContext, offset);
			if (m_nonSharedOffset != -1)
			{
				CopyCachedData(rplWriter, pageContext);
			}
			else
			{
				WriteNonSharedItemProps(spbifWriter, rplWriter, pageContext);
			}
			spbifWriter.Write(byte.MaxValue);
		}

		internal virtual void CopyCachedData(RPLWriter rplWriter, PageContext pageContext)
		{
			BinaryReader propertyCacheReader = pageContext.PropertyCacheReader;
			propertyCacheReader.BaseStream.Seek(m_nonSharedOffset, SeekOrigin.Begin);
			long num = propertyCacheReader.ReadInt64();
			long num2 = propertyCacheReader.ReadInt64();
			if (num2 == m_nonSharedOffset)
			{
				propertyCacheReader.BaseStream.Seek(num, SeekOrigin.Begin);
				CopyData(propertyCacheReader, rplWriter, m_nonSharedOffset - num);
			}
			else
			{
				CopyDataAndResolve(rplWriter, pageContext, num, num2, ignoreDelimiter: false);
			}
		}

		internal void CopyDataAndResolve(RPLWriter rplWriter, PageContext pageContext, long startOffset, long endOffset, bool ignoreDelimiter)
		{
			BinaryReader propertyCacheReader = pageContext.PropertyCacheReader;
			propertyCacheReader.BaseStream.Seek(endOffset, SeekOrigin.Begin);
			string key = propertyCacheReader.ReadString();
			long num = propertyCacheReader.ReadInt64();
			long num2 = propertyCacheReader.ReadInt64();
			if (ignoreDelimiter)
			{
				endOffset--;
			}
			propertyCacheReader.BaseStream.Seek(startOffset, SeekOrigin.Begin);
			CopyData(propertyCacheReader, rplWriter, num - startOffset);
			bool flag = false;
			if (pageContext.SharedImages != null)
			{
				object obj = pageContext.SharedImages[key];
				if (obj != null)
				{
					rplWriter.BinaryWriter.Write((byte)42);
					rplWriter.BinaryWriter.Write((byte)2);
					rplWriter.BinaryWriter.Write((long)obj);
					flag = true;
				}
			}
			else
			{
				pageContext.SharedImages = new Hashtable();
			}
			if (!flag)
			{
				pageContext.SharedImages.Add(key, rplWriter.BinaryWriter.BaseStream.Position);
				propertyCacheReader.BaseStream.Seek(num, SeekOrigin.Begin);
				propertyCacheReader.ReadByte();
				if (propertyCacheReader.ReadByte() == 0)
				{
					propertyCacheReader.BaseStream.Seek(num, SeekOrigin.Begin);
					CopyData(propertyCacheReader, rplWriter, endOffset - num);
					return;
				}
				ItemBoundaries itemBoundaries = pageContext.CacheSharedImages[key] as ItemBoundaries;
				propertyCacheReader.BaseStream.Seek(itemBoundaries.StartOffset, SeekOrigin.Begin);
				CopyData(propertyCacheReader, rplWriter, itemBoundaries.EndOffset - itemBoundaries.StartOffset);
			}
			propertyCacheReader.BaseStream.Seek(num2, SeekOrigin.Begin);
			CopyData(propertyCacheReader, rplWriter, endOffset - num2);
		}

		internal void CopyData(BinaryReader cacheReader, RPLWriter rplWriter, long length)
		{
			byte[] copyBuffer = rplWriter.CopyBuffer;
			while (length >= copyBuffer.Length)
			{
				cacheReader.Read(copyBuffer, 0, copyBuffer.Length);
				rplWriter.BinaryWriter.Write(copyBuffer, 0, copyBuffer.Length);
				length -= copyBuffer.Length;
			}
			if (length > 0)
			{
				cacheReader.Read(copyBuffer, 0, (int)length);
				rplWriter.BinaryWriter.Write(copyBuffer, 0, (int)length);
			}
		}

		internal void WriteElementProps(RPLElementProps elemProps, PageContext pageContext)
		{
			WriteSharedItemProps(elemProps, pageContext);
			WriteNonSharedItemProps(elemProps, pageContext);
		}

		protected void WriteSharedItemProps(BinaryWriter spbifWriter, RPLWriter rplWriter, PageContext pageContext, long offset)
		{
			Hashtable hashtable = pageContext.ItemPropsStart;
			if (hashtable != null)
			{
				object obj = hashtable[SourceID];
				if (obj != null)
				{
					spbifWriter.Write((byte)2);
					spbifWriter.Write((long)obj);
					return;
				}
			}
			if (hashtable == null)
			{
				hashtable = (pageContext.ItemPropsStart = new Hashtable());
			}
			hashtable.Add(SourceID, offset);
			spbifWriter.Write((byte)0);
			spbifWriter.Write((byte)1);
			spbifWriter.Write(SourceID);
			if (m_source != null && !m_source.DocumentMapLabel.IsExpression && m_source.DocumentMapLabel.Value != null)
			{
				spbifWriter.Write((byte)3);
				spbifWriter.Write(m_source.DocumentMapLabel.Value);
			}
			WriteCustomSharedItemProps(spbifWriter, rplWriter, pageContext);
			WriteSharedStyle(spbifWriter, null, pageContext);
			spbifWriter.Write(byte.MaxValue);
		}

		protected void WriteSharedItemProps(RPLElementProps elemProps, PageContext pageContext)
		{
			Hashtable hashtable = pageContext.ItemPropsStart;
			if (hashtable != null)
			{
				object obj = hashtable[SourceID];
				if (obj != null)
				{
					elemProps.Definition = (RPLElementPropsDef)obj;
					return;
				}
			}
			RPLItemPropsDef rPLItemPropsDef = (elemProps as RPLItemProps).Definition as RPLItemPropsDef;
			if (hashtable == null)
			{
				hashtable = (pageContext.ItemPropsStart = new Hashtable());
			}
			hashtable.Add(SourceID, rPLItemPropsDef);
			rPLItemPropsDef.ID = SourceID;
			if (m_source != null && !m_source.DocumentMapLabel.IsExpression)
			{
				rPLItemPropsDef.Label = m_source.DocumentMapLabel.Value;
			}
			WriteCustomSharedItemProps(rPLItemPropsDef, pageContext);
			rPLItemPropsDef.SharedStyle = WriteSharedStyle(null, pageContext);
		}

		internal virtual void WriteCustomSharedItemProps(BinaryWriter spbifWriter, RPLWriter rplWriter, PageContext pageContext)
		{
		}

		internal virtual void WriteCustomSharedItemProps(RPLElementPropsDef sharedProps, PageContext pageContext)
		{
		}

		internal void WriteStyleProp(Style styleDef, StyleInstance style, BinaryWriter spbifWriter, StyleAttributeNames name, byte spbifType)
		{
			if (styleDef == null)
			{
				styleDef = m_source.Style;
			}
			if (styleDef == null)
			{
				return;
			}
			ReportProperty reportProperty = styleDef[name];
			if (reportProperty == null || !reportProperty.IsExpression)
			{
				return;
			}
			object obj = style[name];
			if (obj == null)
			{
				WriteStyleReportProperty(reportProperty, spbifWriter, spbifType);
				return;
			}
			switch (spbifType)
			{
			case 33:
				return;
			case 37:
			{
				int num = (int)obj;
				if (num > 0)
				{
					spbifWriter.Write(spbifType);
					spbifWriter.Write(num);
				}
				return;
			}
			}
			bool convertToString = true;
			byte? stylePropByte = GetStylePropByte(spbifType, obj, ref convertToString);
			if (stylePropByte.HasValue)
			{
				spbifWriter.Write(spbifType);
				spbifWriter.Write(stylePropByte.Value);
			}
			else if (convertToString)
			{
				string text = obj.ToString();
				if (text != null)
				{
					spbifWriter.Write(spbifType);
					spbifWriter.Write(text);
				}
			}
		}

		internal void WriteStyleProp(Style styleDef, StyleInstance style, RPLStyleProps rplStyleProps, StyleAttributeNames name, byte spbifType)
		{
			if (styleDef == null)
			{
				styleDef = m_source.Style;
			}
			if (styleDef == null)
			{
				return;
			}
			ReportProperty reportProperty = styleDef[name];
			if (reportProperty == null || !reportProperty.IsExpression)
			{
				return;
			}
			object obj = style[name];
			if (obj == null)
			{
				WriteStyleReportProperty(reportProperty, rplStyleProps, spbifType);
				return;
			}
			switch (spbifType)
			{
			case 33:
				return;
			case 37:
			{
				int num = (int)obj;
				if (num > 0)
				{
					rplStyleProps.Add(spbifType, num);
				}
				return;
			}
			}
			bool convertToString = true;
			byte? stylePropByte = GetStylePropByte(spbifType, obj, ref convertToString);
			if (stylePropByte.HasValue)
			{
				rplStyleProps.Add(spbifType, stylePropByte.Value);
			}
			else if (convertToString)
			{
				string text = obj.ToString();
				if (text != null)
				{
					rplStyleProps.Add(spbifType, text);
				}
			}
		}

		internal static byte? GetStylePropByte(byte spbifType, object styleProp, ref bool convertToString)
		{
			switch (spbifType)
			{
			case 19:
				return StyleEnumConverter.Translate((FontStyles)styleProp);
			case 22:
				return StyleEnumConverter.Translate((FontWeights)styleProp);
			case 24:
				return StyleEnumConverter.Translate((TextDecorations)styleProp);
			case 25:
				return StyleEnumConverter.Translate((TextAlignments)styleProp);
			case 26:
				return StyleEnumConverter.Translate((VerticalAlignments)styleProp);
			case 29:
				return StyleEnumConverter.Translate((Directions)styleProp);
			case 30:
				return StyleEnumConverter.Translate((WritingModes)styleProp);
			case 31:
				return StyleEnumConverter.Translate((UnicodeBiDiTypes)styleProp);
			case 38:
				return StyleEnumConverter.Translate((Calendars)styleProp);
			case 5:
				return StyleEnumConverter.Translate((BorderStyles)styleProp);
			case 6:
			case 7:
			case 8:
			case 9:
				convertToString = false;
				return StyleEnumConverter.Translate((BorderStyles)styleProp);
			case 35:
				return StyleEnumConverter.Translate((BackgroundRepeatTypes)styleProp);
			default:
				return null;
			}
		}

		internal static void WriteStyleReportProperty(ReportProperty styleProp, BinaryWriter spbifWriter, byte spbifType)
		{
			if (styleProp == null)
			{
				return;
			}
			if (styleProp is ReportStringProperty)
			{
				ReportStringProperty reportStringProperty = (ReportStringProperty)styleProp;
				if (reportStringProperty.Value != null)
				{
					spbifWriter.Write(spbifType);
					spbifWriter.Write(reportStringProperty.Value);
				}
			}
			else if (styleProp is ReportSizeProperty)
			{
				ReportSizeProperty reportSizeProperty = (ReportSizeProperty)styleProp;
				if (reportSizeProperty.Value != null)
				{
					spbifWriter.Write(spbifType);
					spbifWriter.Write(reportSizeProperty.Value.ToString());
				}
			}
			else if (styleProp is ReportColorProperty)
			{
				ReportColorProperty reportColorProperty = (ReportColorProperty)styleProp;
				if (reportColorProperty.Value != null)
				{
					spbifWriter.Write(spbifType);
					spbifWriter.Write(reportColorProperty.Value.ToString());
				}
			}
			else if (styleProp is ReportIntProperty)
			{
				ReportIntProperty reportIntProperty = (ReportIntProperty)styleProp;
				if (reportIntProperty.Value > 0)
				{
					spbifWriter.Write(spbifType);
					spbifWriter.Write(reportIntProperty.Value);
				}
			}
			else
			{
				byte? stylePropByte = GetStylePropByte(spbifType, styleProp);
				if (stylePropByte.HasValue)
				{
					spbifWriter.Write(spbifType);
					spbifWriter.Write(stylePropByte.Value);
				}
			}
		}

		internal static void WriteStyleReportProperty(ReportProperty styleProp, RPLStyleProps rplStyleProps, byte spbifType)
		{
			if (styleProp == null)
			{
				return;
			}
			if (styleProp is ReportStringProperty)
			{
				ReportStringProperty reportStringProperty = (ReportStringProperty)styleProp;
				if (reportStringProperty.Value != null)
				{
					rplStyleProps.Add(spbifType, reportStringProperty.Value);
				}
			}
			else if (styleProp is ReportSizeProperty)
			{
				ReportSizeProperty reportSizeProperty = (ReportSizeProperty)styleProp;
				if (reportSizeProperty.Value != null)
				{
					rplStyleProps.Add(spbifType, reportSizeProperty.Value.ToString());
				}
			}
			else if (styleProp is ReportColorProperty)
			{
				ReportColorProperty reportColorProperty = (ReportColorProperty)styleProp;
				if (reportColorProperty.Value != null)
				{
					rplStyleProps.Add(spbifType, reportColorProperty.Value.ToString());
				}
			}
			else if (styleProp is ReportIntProperty)
			{
				ReportIntProperty reportIntProperty = (ReportIntProperty)styleProp;
				if (reportIntProperty.Value > 0)
				{
					rplStyleProps.Add(spbifType, reportIntProperty.Value);
				}
			}
			else
			{
				byte? stylePropByte = GetStylePropByte(spbifType, styleProp);
				if (stylePropByte.HasValue)
				{
					rplStyleProps.Add(spbifType, stylePropByte.Value);
				}
			}
		}

		internal static byte? GetStylePropByte(byte spbifType, ReportProperty styleProp)
		{
			switch (spbifType)
			{
			case 19:
				return StyleEnumConverter.Translate(((ReportEnumProperty<FontStyles>)styleProp).Value);
			case 22:
				return StyleEnumConverter.Translate(((ReportEnumProperty<FontWeights>)styleProp).Value);
			case 24:
				return StyleEnumConverter.Translate(((ReportEnumProperty<TextDecorations>)styleProp).Value);
			case 25:
				return StyleEnumConverter.Translate(((ReportEnumProperty<TextAlignments>)styleProp).Value);
			case 26:
				return StyleEnumConverter.Translate(((ReportEnumProperty<VerticalAlignments>)styleProp).Value);
			case 29:
				return StyleEnumConverter.Translate(((ReportEnumProperty<Directions>)styleProp).Value);
			case 30:
				return StyleEnumConverter.Translate(((ReportEnumProperty<WritingModes>)styleProp).Value);
			case 31:
				return StyleEnumConverter.Translate(((ReportEnumProperty<UnicodeBiDiTypes>)styleProp).Value);
			case 38:
				return StyleEnumConverter.Translate(((ReportEnumProperty<Calendars>)styleProp).Value);
			case 5:
			case 6:
			case 7:
			case 8:
			case 9:
				return StyleEnumConverter.Translate(((ReportEnumProperty<BorderStyles>)styleProp).Value);
			case 35:
				return StyleEnumConverter.Translate(((ReportEnumProperty<BackgroundRepeatTypes>)styleProp).Value);
			default:
				return null;
			}
		}

		internal void WriteStyleProp(Style style, BinaryWriter spbifWriter, StyleAttributeNames name, byte spbifType)
		{
			ReportProperty reportProperty = style[name];
			if (reportProperty != null && !reportProperty.IsExpression)
			{
				WriteStyleReportProperty(reportProperty, spbifWriter, spbifType);
			}
		}

		internal static void WriteStyleProp(Style style, RPLStyleProps rplStyleProps, StyleAttributeNames name, byte spbifType)
		{
			ReportProperty reportProperty = style[name];
			if (reportProperty != null && !reportProperty.IsExpression)
			{
				WriteStyleReportProperty(reportProperty, rplStyleProps, spbifType);
			}
		}

		internal void WriteBackgroundImage(Style style, bool writeShared, BinaryWriter spbifWriter, PageContext pageContext)
		{
			if (style == null)
			{
				style = m_source.Style;
			}
			if (style == null)
			{
				return;
			}
			BackgroundImage backgroundImage = style.BackgroundImage;
			if (backgroundImage == null || backgroundImage.Instance == null || (backgroundImage.Instance.StreamName == null && backgroundImage.Instance.ImageData == null))
			{
				return;
			}
			if (backgroundImage.Value != null)
			{
				if (backgroundImage.Value.IsExpression)
				{
					if (!writeShared)
					{
						spbifWriter.Write((byte)33);
						WriteImage(backgroundImage.Instance, null, spbifWriter, pageContext, null, writeShared: false);
					}
				}
				else if (writeShared && backgroundImage.Value.Value != null)
				{
					spbifWriter.Write((byte)33);
					WriteImage(backgroundImage.Instance, null, spbifWriter, pageContext, null, writeShared: true);
				}
			}
			if (backgroundImage.BackgroundRepeat == null)
			{
				return;
			}
			if (backgroundImage.BackgroundRepeat.IsExpression)
			{
				if (!writeShared)
				{
					spbifWriter.Write((byte)35);
					spbifWriter.Write(StyleEnumConverter.Translate(backgroundImage.Instance.BackgroundRepeat));
				}
			}
			else if (writeShared)
			{
				spbifWriter.Write((byte)35);
				spbifWriter.Write(StyleEnumConverter.Translate(backgroundImage.BackgroundRepeat.Value));
			}
		}

		internal void WriteBackgroundImage(Style style, bool writeShared, RPLStyleProps rplStyleProps, PageContext pageContext)
		{
			if (style == null)
			{
				style = m_source.Style;
			}
			if (style == null)
			{
				return;
			}
			BackgroundImage backgroundImage = style.BackgroundImage;
			if (backgroundImage == null || backgroundImage.Instance == null || (backgroundImage.Instance.StreamName == null && backgroundImage.Instance.ImageData == null))
			{
				return;
			}
			if (backgroundImage.Value != null)
			{
				if (backgroundImage.Value.IsExpression)
				{
					if (!writeShared)
					{
						RPLImageData imageData = new RPLImageData();
						WriteImage(backgroundImage.Instance, null, ref imageData, pageContext, null);
						rplStyleProps.Add(33, imageData);
					}
				}
				else if (writeShared && backgroundImage.Value.Value != null)
				{
					RPLImageData imageData2 = new RPLImageData();
					WriteImage(backgroundImage.Instance, null, ref imageData2, pageContext, null);
					rplStyleProps.Add(33, imageData2);
				}
			}
			if (backgroundImage.BackgroundRepeat == null)
			{
				return;
			}
			if (backgroundImage.BackgroundRepeat.IsExpression)
			{
				if (!writeShared)
				{
					rplStyleProps.Add(35, StyleEnumConverter.Translate(backgroundImage.Instance.BackgroundRepeat));
				}
			}
			else if (writeShared)
			{
				rplStyleProps.Add(35, StyleEnumConverter.Translate(backgroundImage.BackgroundRepeat.Value));
			}
		}

		internal void WriteSharedStyle(BinaryWriter spbifWriter, Style style, PageContext pageContext)
		{
			if (style == null)
			{
				style = SharedStyle;
			}
			if (style != null)
			{
				spbifWriter.Write((byte)6);
				spbifWriter.Write((byte)0);
				WriteItemSharedStyleProps(spbifWriter, style, pageContext);
				WriteBorderProps(spbifWriter, style);
				spbifWriter.Write(byte.MaxValue);
			}
		}

		internal virtual void WriteBorderProps(BinaryWriter spbifWriter, Style style)
		{
			WriteStyleProp(style, spbifWriter, StyleAttributeNames.BorderColor, 0);
			WriteStyleProp(style, spbifWriter, StyleAttributeNames.BorderColorBottom, 4);
			WriteStyleProp(style, spbifWriter, StyleAttributeNames.BorderColorLeft, 1);
			WriteStyleProp(style, spbifWriter, StyleAttributeNames.BorderColorRight, 2);
			WriteStyleProp(style, spbifWriter, StyleAttributeNames.BorderColorTop, 3);
			WriteStyleProp(style, spbifWriter, StyleAttributeNames.BorderStyle, 5);
			WriteStyleProp(style, spbifWriter, StyleAttributeNames.BorderStyleBottom, 9);
			WriteStyleProp(style, spbifWriter, StyleAttributeNames.BorderStyleLeft, 6);
			WriteStyleProp(style, spbifWriter, StyleAttributeNames.BorderStyleRight, 7);
			WriteStyleProp(style, spbifWriter, StyleAttributeNames.BorderStyleTop, 8);
			WriteStyleProp(style, spbifWriter, StyleAttributeNames.BorderWidth, 10);
			WriteStyleProp(style, spbifWriter, StyleAttributeNames.BorderWidthBottom, 14);
			WriteStyleProp(style, spbifWriter, StyleAttributeNames.BorderWidthLeft, 11);
			WriteStyleProp(style, spbifWriter, StyleAttributeNames.BorderWidthRight, 12);
			WriteStyleProp(style, spbifWriter, StyleAttributeNames.BorderWidthTop, 13);
		}

		internal RPLStyleProps WriteSharedStyle(Style style, PageContext pageContext)
		{
			if (style == null)
			{
				style = SharedStyle;
			}
			if (style == null)
			{
				return null;
			}
			RPLStyleProps rPLStyleProps = new RPLStyleProps();
			WriteItemSharedStyleProps(rPLStyleProps, style, pageContext);
			WriteBorderProps(rPLStyleProps, style);
			return rPLStyleProps;
		}

		internal virtual void WriteBorderProps(RPLStyleProps rplStyleProps, Style style)
		{
			WriteStyleProp(style, rplStyleProps, StyleAttributeNames.BorderColor, 0);
			WriteStyleProp(style, rplStyleProps, StyleAttributeNames.BorderColorBottom, 4);
			WriteStyleProp(style, rplStyleProps, StyleAttributeNames.BorderColorLeft, 1);
			WriteStyleProp(style, rplStyleProps, StyleAttributeNames.BorderColorRight, 2);
			WriteStyleProp(style, rplStyleProps, StyleAttributeNames.BorderColorTop, 3);
			WriteStyleProp(style, rplStyleProps, StyleAttributeNames.BorderStyle, 5);
			WriteStyleProp(style, rplStyleProps, StyleAttributeNames.BorderStyleBottom, 9);
			WriteStyleProp(style, rplStyleProps, StyleAttributeNames.BorderStyleLeft, 6);
			WriteStyleProp(style, rplStyleProps, StyleAttributeNames.BorderStyleRight, 7);
			WriteStyleProp(style, rplStyleProps, StyleAttributeNames.BorderStyleTop, 8);
			WriteStyleProp(style, rplStyleProps, StyleAttributeNames.BorderWidth, 10);
			WriteStyleProp(style, rplStyleProps, StyleAttributeNames.BorderWidthBottom, 14);
			WriteStyleProp(style, rplStyleProps, StyleAttributeNames.BorderWidthLeft, 11);
			WriteStyleProp(style, rplStyleProps, StyleAttributeNames.BorderWidthRight, 12);
			WriteStyleProp(style, rplStyleProps, StyleAttributeNames.BorderWidthTop, 13);
		}

		internal virtual void WriteItemSharedStyleProps(BinaryWriter spbifWriter, Style style, PageContext pageContext)
		{
		}

		internal virtual void WriteItemSharedStyleProps(RPLStyleProps rplStyleProps, Style style, PageContext pageContext)
		{
		}

		private void WriteNonSharedItemProps(BinaryWriter spbifWriter, RPLWriter rplWriter, PageContext pageContext)
		{
			spbifWriter.Write((byte)1);
			spbifWriter.Write((byte)0);
			spbifWriter.Write(SourceUniqueName);
			if (m_source != null && m_source.DocumentMapLabel.IsExpression && m_source.Instance.DocumentMapLabel != null)
			{
				spbifWriter.Write((byte)3);
				spbifWriter.Write(m_source.Instance.DocumentMapLabel);
			}
			WriteCustomNonSharedItemProps(spbifWriter, pageContext);
			WriteNonSharedStyle(spbifWriter, null, null, pageContext);
			spbifWriter.Write(byte.MaxValue);
		}

		private void WriteNonSharedItemProps(RPLElementProps elemProps, PageContext pageContext)
		{
			elemProps.UniqueName = SourceUniqueName;
			RPLItemProps rPLItemProps = elemProps as RPLItemProps;
			if (m_source != null && m_source.DocumentMapLabel.IsExpression)
			{
				rPLItemProps.Label = m_source.Instance.DocumentMapLabel;
			}
			WriteCustomNonSharedItemProps(rPLItemProps, pageContext);
			rPLItemProps.NonSharedStyle = WriteNonSharedStyle(null, null, pageContext);
		}

		internal virtual void WriteCustomNonSharedItemProps(BinaryWriter spbifWriter, PageContext pageContext)
		{
		}

		internal virtual void WriteCustomNonSharedItemProps(RPLElementProps elemProps, PageContext pageContext)
		{
		}

		internal virtual void WriteNonSharedStyleProp(BinaryWriter spbifWriter, Style styleDef, StyleInstance style, StyleAttributeNames styleAttribute, PageContext pageContext)
		{
			switch (styleAttribute)
			{
			case StyleAttributeNames.BorderColor:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderColor, 0);
				break;
			case StyleAttributeNames.BorderColorBottom:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderColorBottom, 4);
				break;
			case StyleAttributeNames.BorderColorLeft:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderColorLeft, 1);
				break;
			case StyleAttributeNames.BorderColorRight:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderColorRight, 2);
				break;
			case StyleAttributeNames.BorderColorTop:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderColorTop, 3);
				break;
			case StyleAttributeNames.BorderStyle:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderStyle, 5);
				break;
			case StyleAttributeNames.BorderStyleBottom:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderStyleBottom, 9);
				break;
			case StyleAttributeNames.BorderStyleLeft:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderStyleLeft, 6);
				break;
			case StyleAttributeNames.BorderStyleRight:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderStyleRight, 7);
				break;
			case StyleAttributeNames.BorderStyleTop:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderStyleTop, 8);
				break;
			case StyleAttributeNames.BorderWidth:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderWidth, 10);
				break;
			case StyleAttributeNames.BorderWidthBottom:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderWidthBottom, 14);
				break;
			case StyleAttributeNames.BorderWidthLeft:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderWidthLeft, 11);
				break;
			case StyleAttributeNames.BorderWidthRight:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderWidthRight, 12);
				break;
			case StyleAttributeNames.BorderWidthTop:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderWidthTop, 13);
				break;
			default:
				WriteItemNonSharedStyleProps(spbifWriter, styleDef, style, styleAttribute, pageContext);
				break;
			}
		}

		internal virtual void WriteNonSharedStyle(BinaryWriter spbifWriter, Style styleDef, StyleInstance style, PageContext pageContext)
		{
			if (styleDef == null)
			{
				styleDef = SharedStyle;
			}
			List<StyleAttributeNames> nonSharedStyleAttributes = styleDef.NonSharedStyleAttributes;
			if (nonSharedStyleAttributes == null || nonSharedStyleAttributes.Count == 0)
			{
				return;
			}
			if (style == null)
			{
				style = NonSharedStyle;
			}
			if (style == null)
			{
				return;
			}
			bool flag = false;
			spbifWriter.Write((byte)6);
			spbifWriter.Write((byte)1);
			for (int i = 0; i < nonSharedStyleAttributes.Count; i++)
			{
				StyleAttributeNames styleAttributeNames = nonSharedStyleAttributes[i];
				if ((uint)(styleAttributeNames - 45) <= 1u)
				{
					if (!flag)
					{
						flag = true;
						WriteItemNonSharedStyleProps(spbifWriter, styleDef, style, StyleAttributeNames.BackgroundImage, pageContext);
					}
				}
				else
				{
					WriteNonSharedStyleProp(spbifWriter, styleDef, style, nonSharedStyleAttributes[i], pageContext);
				}
			}
			spbifWriter.Write(byte.MaxValue);
		}

		internal virtual void WriteNonSharedStyleWithoutTag(BinaryWriter spbifWriter, Style styleDef, StyleInstance style, PageContext pageContext)
		{
			if (styleDef == null)
			{
				styleDef = SharedStyle;
			}
			List<StyleAttributeNames> nonSharedStyleAttributes = styleDef.NonSharedStyleAttributes;
			if (nonSharedStyleAttributes == null || nonSharedStyleAttributes.Count == 0)
			{
				return;
			}
			if (style == null)
			{
				style = NonSharedStyle;
			}
			if (style == null)
			{
				return;
			}
			bool flag = false;
			spbifWriter.Write((byte)1);
			for (int i = 0; i < nonSharedStyleAttributes.Count; i++)
			{
				StyleAttributeNames styleAttributeNames = nonSharedStyleAttributes[i];
				if ((uint)(styleAttributeNames - 45) <= 1u)
				{
					if (!flag)
					{
						flag = true;
						WriteItemNonSharedStyleProps(spbifWriter, styleDef, style, StyleAttributeNames.BackgroundImage, pageContext);
					}
				}
				else
				{
					WriteNonSharedStyleProp(spbifWriter, styleDef, style, nonSharedStyleAttributes[i], pageContext);
				}
			}
			spbifWriter.Write(byte.MaxValue);
		}

		internal virtual void WriteNonSharedStyleProp(RPLStyleProps rplStyleProps, Style styleDef, StyleInstance style, StyleAttributeNames styleAttribute, PageContext pageContext)
		{
			switch (styleAttribute)
			{
			case StyleAttributeNames.BorderColor:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderColor, 0);
				break;
			case StyleAttributeNames.BorderColorBottom:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderColorBottom, 4);
				break;
			case StyleAttributeNames.BorderColorLeft:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderColorLeft, 1);
				break;
			case StyleAttributeNames.BorderColorRight:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderColorRight, 2);
				break;
			case StyleAttributeNames.BorderColorTop:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderColorTop, 3);
				break;
			case StyleAttributeNames.BorderStyle:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderStyle, 5);
				break;
			case StyleAttributeNames.BorderStyleBottom:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderStyleBottom, 9);
				break;
			case StyleAttributeNames.BorderStyleLeft:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderStyleLeft, 6);
				break;
			case StyleAttributeNames.BorderStyleRight:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderStyleRight, 7);
				break;
			case StyleAttributeNames.BorderStyleTop:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderStyleTop, 8);
				break;
			case StyleAttributeNames.BorderWidth:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderWidth, 10);
				break;
			case StyleAttributeNames.BorderWidthBottom:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderWidthBottom, 14);
				break;
			case StyleAttributeNames.BorderWidthLeft:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderWidthLeft, 11);
				break;
			case StyleAttributeNames.BorderWidthRight:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderWidthRight, 12);
				break;
			case StyleAttributeNames.BorderWidthTop:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderWidthTop, 13);
				break;
			default:
				WriteItemNonSharedStyleProps(rplStyleProps, styleDef, style, styleAttribute, pageContext);
				break;
			}
		}

		internal virtual RPLStyleProps WriteNonSharedStyle(Style styleDef, StyleInstance style, PageContext pageContext)
		{
			if (styleDef == null)
			{
				styleDef = SharedStyle;
			}
			List<StyleAttributeNames> nonSharedStyleAttributes = styleDef.NonSharedStyleAttributes;
			if (nonSharedStyleAttributes == null || nonSharedStyleAttributes.Count == 0)
			{
				return null;
			}
			if (style == null)
			{
				style = NonSharedStyle;
			}
			if (style == null)
			{
				return null;
			}
			bool flag = false;
			RPLStyleProps rPLStyleProps = new RPLStyleProps();
			for (int i = 0; i < nonSharedStyleAttributes.Count; i++)
			{
				StyleAttributeNames styleAttributeNames = nonSharedStyleAttributes[i];
				if ((uint)(styleAttributeNames - 45) <= 1u)
				{
					if (!flag)
					{
						flag = true;
						WriteItemNonSharedStyleProps(rPLStyleProps, styleDef, style, StyleAttributeNames.BackgroundImage, pageContext);
					}
				}
				else
				{
					WriteNonSharedStyleProp(rPLStyleProps, styleDef, style, nonSharedStyleAttributes[i], pageContext);
				}
			}
			return rPLStyleProps;
		}

		internal virtual void WriteItemNonSharedStyleProps(BinaryWriter spbifWriter, Style styleDef, StyleInstance style, StyleAttributeNames styleAtt, PageContext pageContext)
		{
		}

		internal virtual void WriteItemNonSharedStyleProps(RPLStyleProps rplStyleProps, Style styleDef, StyleInstance style, StyleAttributeNames styleAtt, PageContext pageContext)
		{
		}

		internal void WriteInvalidImage(BinaryWriter spbifWriter, PageContext pageContext, GDIImageProps gdiImageProps)
		{
			long position = spbifWriter.BaseStream.Position;
			spbifWriter.Write((byte)42);
			Hashtable hashtable = pageContext.SharedImages;
			if (hashtable != null)
			{
				object obj = hashtable["InvalidImage"];
				if (obj != null)
				{
					spbifWriter.Write((byte)2);
					spbifWriter.Write((long)obj);
					return;
				}
			}
			if (hashtable == null)
			{
				hashtable = (pageContext.SharedImages = new Hashtable());
			}
			hashtable.Add("InvalidImage", position);
			spbifWriter.Write((byte)0);
			System.Drawing.Image image = Microsoft.ReportingServices.InvalidImage.Image;
			if (image != null)
			{
				MemoryStream memoryStream = new MemoryStream();
				image.Save(memoryStream, image.RawFormat);
				spbifWriter.Write((byte)2);
				spbifWriter.Write((int)memoryStream.Length);
				WriteStreamContent(memoryStream, spbifWriter);
				if (gdiImageProps == null)
				{
					gdiImageProps = new GDIImageProps(image);
				}
				image.Dispose();
			}
			WriteImageProperties(null, "InvalidImage", spbifWriter, gdiImageProps);
			spbifWriter.Write(byte.MaxValue);
		}

		internal void WriteImage(IImageInstance imageInstance, string resourceName, BinaryWriter spbifWriter, PageContext pageContext, GDIImageProps gdiImage, bool writeShared)
		{
			string text = resourceName;
			if (imageInstance != null)
			{
				text = imageInstance.StreamName;
			}
			long position = spbifWriter.BaseStream.Position;
			spbifWriter.Write((byte)42);
			if (text != null)
			{
				WriteShareableImages(imageInstance, text, spbifWriter, pageContext, position, gdiImage, writeShared);
				return;
			}
			spbifWriter.Write((byte)1);
			WriteImageProperties(imageInstance, null, spbifWriter, gdiImage);
			spbifWriter.Write(byte.MaxValue);
		}

		internal void WriteInvalidImage(RPLImageProps elemProps, PageContext pageContext, GDIImageProps gdiImageProps)
		{
			Hashtable hashtable = pageContext.SharedImages;
			if (hashtable != null)
			{
				object obj = hashtable["InvalidImage"];
				if (obj != null)
				{
					elemProps.Image = (RPLImageData)obj;
					return;
				}
			}
			RPLImageData rPLImageData = new RPLImageData();
			if (hashtable == null)
			{
				hashtable = (pageContext.SharedImages = new Hashtable());
			}
			rPLImageData.IsShared = true;
			hashtable.Add("InvalidImage", rPLImageData);
			rPLImageData.ImageName = "InvalidImage";
			System.Drawing.Image image = Microsoft.ReportingServices.InvalidImage.Image;
			if (image != null)
			{
				MemoryStream memoryStream = new MemoryStream();
				image.Save(memoryStream, image.RawFormat);
				rPLImageData.ImageData = memoryStream.ToArray();
				if (gdiImageProps == null)
				{
					rPLImageData.GDIImageProps = new GDIImageProps(image);
				}
				image.Dispose();
			}
			elemProps.Image = rPLImageData;
		}

		internal void WriteImage(IImageInstance imageInstance, string resourceName, RPLImageProps elemProps, PageContext pageContext, GDIImageProps gdiImage)
		{
			string text = resourceName;
			if (imageInstance != null)
			{
				text = imageInstance.StreamName;
			}
			RPLImageData imageData = new RPLImageData();
			if (text != null)
			{
				WriteShareableImages(imageInstance, text, ref imageData, pageContext, gdiImage);
			}
			else
			{
				WriteImageProperties(imageInstance, null, imageData, gdiImage);
			}
			elemProps.Image = imageData;
		}

		internal void WriteImage(IImageInstance imageInstance, string resourceName, ref RPLImageData imageData, PageContext pageContext, GDIImageProps gdiImage)
		{
			string text = resourceName;
			if (imageInstance != null)
			{
				text = imageInstance.StreamName;
			}
			if (text != null)
			{
				WriteShareableImages(imageInstance, text, ref imageData, pageContext, gdiImage);
			}
			else
			{
				WriteImageProperties(imageInstance, null, imageData, gdiImage);
			}
		}

		private void WriteImageProperties(IImageInstance imageInstance, string streamName, BinaryWriter spbifWriter, GDIImageProps gdiImage)
		{
			if (streamName != null)
			{
				spbifWriter.Write((byte)1);
				spbifWriter.Write(streamName);
			}
			if (imageInstance != null)
			{
				if (imageInstance.MIMEType != null)
				{
					spbifWriter.Write((byte)0);
					spbifWriter.Write(imageInstance.MIMEType);
				}
				if (imageInstance.ImageData != null)
				{
					spbifWriter.Write((byte)2);
					spbifWriter.Write(imageInstance.ImageData.Length);
					spbifWriter.Write(imageInstance.ImageData);
				}
			}
			if (gdiImage != null)
			{
				spbifWriter.Write((byte)3);
				spbifWriter.Write(gdiImage.Width);
				spbifWriter.Write((byte)4);
				spbifWriter.Write(gdiImage.Height);
				spbifWriter.Write((byte)5);
				spbifWriter.Write(gdiImage.HorizontalResolution);
				spbifWriter.Write((byte)6);
				spbifWriter.Write(gdiImage.VerticalResolution);
				if (gdiImage.RawFormat.Equals(ImageFormat.Bmp))
				{
					spbifWriter.Write((byte)7);
					spbifWriter.Write((byte)0);
				}
				else if (gdiImage.RawFormat.Equals(ImageFormat.Jpeg))
				{
					spbifWriter.Write((byte)7);
					spbifWriter.Write((byte)1);
				}
				else if (gdiImage.RawFormat.Equals(ImageFormat.Gif))
				{
					spbifWriter.Write((byte)7);
					spbifWriter.Write((byte)2);
				}
				else if (gdiImage.RawFormat.Equals(ImageFormat.Png))
				{
					spbifWriter.Write((byte)7);
					spbifWriter.Write((byte)3);
				}
			}
		}

		private void WriteImageProperties(IImageInstance imageInstance, string streamName, RPLImageData imageData, GDIImageProps gdiImage)
		{
			imageData.ImageName = streamName;
			if (imageInstance != null)
			{
				imageData.ImageMimeType = imageInstance.MIMEType;
				imageData.ImageData = imageInstance.ImageData;
			}
			imageData.GDIImageProps = gdiImage;
		}

		private void WriteStreamContent(Stream sourceStream, BinaryWriter spbifWriter)
		{
			byte[] buffer = new byte[1024];
			long num = 0L;
			int num2 = 0;
			sourceStream.Position = 0L;
			for (; num < sourceStream.Length; num += num2)
			{
				num2 = sourceStream.Read(buffer, 0, 1024);
				spbifWriter.Write(buffer, 0, num2);
			}
		}

		private void WriteShareableImages(IImageInstance imageInstance, string streamName, BinaryWriter spbifWriter, PageContext pageContext, long offsetStart, GDIImageProps gdiImage, bool writeShared)
		{
			if (m_nonSharedOffset == -1 || writeShared)
			{
				Hashtable hashtable = pageContext.SharedImages;
				if (hashtable != null)
				{
					object obj = hashtable[streamName];
					if (obj != null)
					{
						spbifWriter.Write((byte)2);
						spbifWriter.Write((long)obj);
						return;
					}
				}
				if (hashtable == null)
				{
					hashtable = (pageContext.SharedImages = new Hashtable());
				}
				hashtable.Add(streamName, offsetStart);
				spbifWriter.Write((byte)0);
				WriteImageProperties(imageInstance, streamName, spbifWriter, gdiImage);
				spbifWriter.Write(byte.MaxValue);
				return;
			}
			Hashtable hashtable3 = pageContext.CacheSharedImages;
			if (hashtable3 == null)
			{
				hashtable3 = (pageContext.CacheSharedImages = new Hashtable());
			}
			else
			{
				object obj2 = hashtable3[streamName];
				if (obj2 != null)
				{
					spbifWriter.Write((byte)2);
					pageContext.ItemCacheSharedImageInfo = new CachedSharedImageInfo(streamName, new ItemBoundaries(offsetStart, spbifWriter.BaseStream.Position));
					return;
				}
			}
			spbifWriter.Write((byte)0);
			WriteImageProperties(imageInstance, streamName, spbifWriter, gdiImage);
			spbifWriter.Write(byte.MaxValue);
			ItemBoundaries itemBoundaries = new ItemBoundaries(offsetStart, spbifWriter.BaseStream.Position);
			pageContext.ItemCacheSharedImageInfo = new CachedSharedImageInfo(streamName, itemBoundaries);
			hashtable3.Add(streamName, itemBoundaries);
		}

		private void WriteShareableImages(IImageInstance imageInstance, string streamName, ref RPLImageData imageData, PageContext pageContext, GDIImageProps gdiImage)
		{
			Hashtable hashtable = pageContext.SharedImages;
			if (hashtable != null)
			{
				object obj = hashtable[streamName];
				if (obj != null)
				{
					imageData = (RPLImageData)obj;
					return;
				}
			}
			if (hashtable == null)
			{
				hashtable = (pageContext.SharedImages = new Hashtable());
			}
			imageData.IsShared = true;
			hashtable.Add(streamName, imageData);
			WriteImageProperties(imageInstance, streamName, imageData, gdiImage);
		}

		internal void WriteActionInfo(ActionInfo actionInfo, BinaryWriter spbifWriter)
		{
			if (actionInfo == null)
			{
				return;
			}
			ActionCollection actions = actionInfo.Actions;
			if (actions == null || actions.Count == 0)
			{
				return;
			}
			Microsoft.ReportingServices.OnDemandReportRendering.Action action = null;
			ActionInstance actionInstance = null;
			spbifWriter.Write((byte)7);
			spbifWriter.Write((byte)2);
			spbifWriter.Write(actions.Count);
			for (int i = 0; i < actions.Count; i++)
			{
				action = actions[i];
				actionInstance = action.Instance;
				spbifWriter.Write((byte)3);
				if (action.Hyperlink != null)
				{
					ReportUrl hyperlink = actionInstance.Hyperlink;
					if (hyperlink != null)
					{
						Uri uri = hyperlink.ToUri();
						if (null != uri)
						{
							spbifWriter.Write((byte)6);
							spbifWriter.Write(uri.AbsoluteUri);
						}
					}
				}
				spbifWriter.Write(byte.MaxValue);
			}
			spbifWriter.Write(byte.MaxValue);
		}

		internal static RPLActionInfo WriteActionInfo(ActionInfo actionInfo)
		{
			if (actionInfo == null)
			{
				return null;
			}
			ActionCollection actions = actionInfo.Actions;
			if (actions == null || actions.Count == 0)
			{
				return null;
			}
			RPLActionInfo rPLActionInfo = new RPLActionInfo(actions.Count);
			WriteAction(actions, rPLActionInfo);
			return rPLActionInfo;
		}

		internal static void WriteAction(ActionCollection actions, RPLActionInfo rplActionInfo)
		{
			Microsoft.ReportingServices.OnDemandReportRendering.Action action = null;
			ActionInstance actionInstance = null;
			RPLAction rPLAction = null;
			for (int i = 0; i < actions.Count; i++)
			{
				action = actions[i];
				actionInstance = action.Instance;
				rPLAction = new RPLAction();
				if (action.Hyperlink != null)
				{
					ReportUrl hyperlink = actionInstance.Hyperlink;
					if (hyperlink != null)
					{
						Uri uri = hyperlink.ToUri();
						if (null != uri)
						{
							rPLAction.Hyperlink = uri.AbsoluteUri;
						}
					}
				}
				rplActionInfo.Actions[i] = rPLAction;
			}
		}

		internal bool WriteObjectValue(BinaryWriter spbifWriter, byte name, TypeCode typeCode, object value)
		{
			bool result = false;
			spbifWriter.Write(name);
			switch (typeCode)
			{
			case TypeCode.Byte:
				spbifWriter.Write((byte)value);
				break;
			case TypeCode.Decimal:
				spbifWriter.Write((decimal)value);
				break;
			case TypeCode.Double:
				spbifWriter.Write((double)value);
				break;
			case TypeCode.Int16:
				spbifWriter.Write((short)value);
				break;
			case TypeCode.Int32:
				spbifWriter.Write((int)value);
				break;
			case TypeCode.Int64:
				spbifWriter.Write((long)value);
				break;
			case TypeCode.SByte:
				spbifWriter.Write((sbyte)value);
				break;
			case TypeCode.Single:
				spbifWriter.Write((float)value);
				break;
			case TypeCode.UInt16:
				spbifWriter.Write((ushort)value);
				break;
			case TypeCode.UInt32:
				spbifWriter.Write((uint)value);
				break;
			case TypeCode.UInt64:
				spbifWriter.Write((ulong)value);
				break;
			case TypeCode.DateTime:
				spbifWriter.Write(((DateTime)value).ToBinary());
				break;
			case TypeCode.Boolean:
				spbifWriter.Write((bool)value);
				break;
			default:
				spbifWriter.Write(value.ToString());
				result = true;
				break;
			}
			return result;
		}

		internal bool WriteObjectValue(RPLTextBoxProps textBoxProps, TypeCode typeCode, object value)
		{
			bool result = false;
			if (typeCode != TypeCode.Boolean && (uint)(typeCode - 5) > 11u)
			{
				result = true;
			}
			return result;
		}
	}
}
