using Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel;
using Microsoft.ReportingServices.ReportProcessing.Persistence;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using Microsoft.ReportingServices.ReportRendering;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class Rectangle : ReportItem, IPageBreakItem, IIndexInto
	{
		private ReportItemCollection m_reportItems;

		private bool m_pageBreakAtEnd;

		private bool m_pageBreakAtStart;

		private int m_linkToChild = -1;

		[NonSerialized]
		private PageBreakStates m_pagebreakState;

		[NonSerialized]
		private ReportItemExprHost m_exprHost;

		internal override ObjectType ObjectType => ObjectType.Rectangle;

		internal ReportItemCollection ReportItems
		{
			get
			{
				return m_reportItems;
			}
			set
			{
				m_reportItems = value;
			}
		}

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

		internal bool PageBreakAtStart
		{
			get
			{
				return m_pageBreakAtStart;
			}
			set
			{
				m_pageBreakAtStart = value;
			}
		}

		internal int LinkToChild
		{
			get
			{
				return m_linkToChild;
			}
			set
			{
				m_linkToChild = value;
			}
		}

		internal Rectangle(ReportItem parent)
			: base(parent)
		{
		}

		internal Rectangle(int id, int idForReportItems, ReportItem parent)
			: base(id, parent)
		{
			m_reportItems = new ReportItemCollection(idForReportItems, normal: true);
		}

		internal override void CalculateSizes(double width, double height, InitializationContext context, bool overwrite)
		{
			base.CalculateSizes(width, height, context, overwrite);
			m_reportItems.CalculateSizes(context, overwrite: false);
		}

		internal override bool Initialize(InitializationContext context)
		{
			context.ObjectType = ObjectType;
			context.ObjectName = m_name;
			context.ExprHostBuilder.RectangleStart(m_name);
			base.Initialize(context);
			if (m_visibility != null)
			{
				m_visibility.Initialize(context, isContainer: true, tableRowCol: false);
			}
			m_reportItems.Initialize(context, registerRunningValues: false);
			if (m_visibility != null)
			{
				m_visibility.UnRegisterReceiver(context);
			}
			base.ExprHostID = context.ExprHostBuilder.RectangleEnd();
			return false;
		}

		protected override void DataRendererInitialize(InitializationContext context)
		{
			if (DataElementOutputTypesRDL.Auto == m_dataElementOutputRDL)
			{
				m_dataElementOutputRDL = DataElementOutputTypesRDL.ContentsOnly;
			}
			base.DataRendererInitialize(context);
		}

		internal override void RegisterReceiver(InitializationContext context)
		{
			if (m_visibility != null)
			{
				m_visibility.RegisterReceiver(context, isContainer: true);
			}
			m_reportItems.RegisterReceiver(context);
			if (m_visibility != null)
			{
				m_visibility.UnRegisterReceiver(context);
			}
		}

		internal bool ContainsDataRegionOrSubReport()
		{
			for (int i = 0; i < m_reportItems.Count; i++)
			{
				ReportItem reportItem = m_reportItems[i];
				if (reportItem is DataRegion)
				{
					return true;
				}
				if (reportItem is SubReport)
				{
					return true;
				}
				if (reportItem is Rectangle && ((Rectangle)reportItem).ContainsDataRegionOrSubReport())
				{
					return true;
				}
			}
			return false;
		}

		bool IPageBreakItem.IgnorePageBreaks()
		{
			if (m_pagebreakState == PageBreakStates.Unknown)
			{
				if (m_repeatedSibling || SharedHiddenState.Never != Visibility.GetSharedHidden(m_visibility))
				{
					m_pagebreakState = PageBreakStates.CanIgnore;
				}
				else
				{
					m_pagebreakState = PageBreakStates.CannotIgnore;
				}
			}
			if (PageBreakStates.CanIgnore == m_pagebreakState)
			{
				return true;
			}
			return false;
		}

		bool IPageBreakItem.HasPageBreaks(bool atStart)
		{
			if ((atStart && m_pageBreakAtStart) || (!atStart && m_pageBreakAtEnd))
			{
				return true;
			}
			return false;
		}

		internal override void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			if (base.ExprHostID >= 0)
			{
				Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null);
				m_exprHost = reportExprHost.RectangleHostsRemotable[base.ExprHostID];
				ReportItemSetExprHost(m_exprHost, reportObjectModel);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.ReportItems, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemCollection));
			memberInfoList.Add(new MemberInfo(MemberName.PageBreakAtEnd, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.PageBreakAtStart, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.LinkToChild, Token.Int32));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItem, memberInfoList);
		}

		internal object SearchChildren(int targetUniqueName, ref NonComputedUniqueNames nonCompNames, ChunkManager.RenderingChunkManager chunkManager)
		{
			if (nonCompNames.ChildrenUniqueNames == null)
			{
				return null;
			}
			NonComputedUniqueNames nonCompNames2 = null;
			int count = m_reportItems.Count;
			object obj = null;
			for (int i = 0; i < count; i++)
			{
				nonCompNames2 = nonCompNames.ChildrenUniqueNames[i];
				obj = ((ISearchByUniqueName)m_reportItems[i]).Find(targetUniqueName, ref nonCompNames2, chunkManager);
				if (obj != null)
				{
					break;
				}
			}
			if (obj != null)
			{
				nonCompNames = nonCompNames2;
				return obj;
			}
			return null;
		}

		internal override void ProcessDrillthroughAction(ReportProcessing.ProcessingContext processingContext, NonComputedUniqueNames nonCompNames)
		{
			if (nonCompNames != null && nonCompNames.ChildrenUniqueNames != null)
			{
				NonComputedUniqueNames nonComputedUniqueNames = null;
				int count = m_reportItems.Count;
				for (int i = 0; i < count; i++)
				{
					nonComputedUniqueNames = nonCompNames.ChildrenUniqueNames[i];
					m_reportItems[i].ProcessDrillthroughAction(processingContext, nonComputedUniqueNames);
				}
			}
		}

		internal int ProcessNavigationChildren(ReportProcessing.NavigationInfo navigationInfo, NonComputedUniqueNames nonCompNames, int startPage)
		{
			if (nonCompNames.ChildrenUniqueNames == null)
			{
				return -1;
			}
			NonComputedUniqueNames nonComputedUniqueNames = null;
			int count = m_reportItems.Count;
			int result = -1;
			for (int i = 0; i < count; i++)
			{
				nonComputedUniqueNames = nonCompNames.ChildrenUniqueNames[i];
				if (i == m_linkToChild)
				{
					result = nonComputedUniqueNames.UniqueName;
				}
				m_reportItems[i].ProcessNavigationAction(navigationInfo, nonComputedUniqueNames, startPage);
			}
			return result;
		}

		object IIndexInto.GetChildAt(int index, out NonComputedUniqueNames nonCompNames)
		{
			nonCompNames = null;
			if (index < 0 || index >= m_reportItems.Count)
			{
				return null;
			}
			return m_reportItems[index];
		}
	}
}
