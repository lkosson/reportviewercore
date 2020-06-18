using Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel;
using Microsoft.ReportingServices.ReportProcessing.Persistence;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using Microsoft.ReportingServices.ReportRendering;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class MatrixHeading : PivotHeading, IPageBreakItem
	{
		private string m_size;

		private double m_sizeValue;

		private ReportItemCollection m_reportItems;

		private bool m_owcGroupExpression;

		[NonSerialized]
		private bool m_inFirstPage = true;

		[NonSerialized]
		private BoolList m_firstHeadingInstances;

		[NonSerialized]
		private MatrixDynamicGroupExprHost m_exprHost;

		[NonSerialized]
		private bool m_startHidden;

		[NonSerialized]
		private bool m_inOutermostSubtotalCell;

		[NonSerialized]
		private string m_renderingModelID;

		[NonSerialized]
		private string[] m_renderingModelIDs;

		[NonSerialized]
		private ReportSize m_sizeForRendering;

		internal new MatrixHeading SubHeading
		{
			get
			{
				return (MatrixHeading)m_innerHierarchy;
			}
			set
			{
				m_innerHierarchy = value;
			}
		}

		internal string Size
		{
			get
			{
				return m_size;
			}
			set
			{
				m_size = value;
			}
		}

		internal double SizeValue
		{
			get
			{
				return m_sizeValue;
			}
			set
			{
				m_sizeValue = value;
			}
		}

		internal bool StartHidden
		{
			get
			{
				return m_startHidden;
			}
			set
			{
				m_startHidden = value;
			}
		}

		internal ReportItem ReportItem
		{
			get
			{
				if (m_reportItems != null && 0 < m_reportItems.Count)
				{
					return m_reportItems[0];
				}
				return null;
			}
		}

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

		internal bool OwcGroupExpression
		{
			get
			{
				return m_owcGroupExpression;
			}
			set
			{
				m_owcGroupExpression = value;
			}
		}

		internal bool InFirstPage
		{
			get
			{
				return m_inFirstPage;
			}
			set
			{
				m_inFirstPage = value;
			}
		}

		internal BoolList FirstHeadingInstances
		{
			get
			{
				return m_firstHeadingInstances;
			}
			set
			{
				m_firstHeadingInstances = value;
			}
		}

		internal string RenderingModelID
		{
			get
			{
				return m_renderingModelID;
			}
			set
			{
				m_renderingModelID = value;
			}
		}

		internal string[] RenderingModelIDs
		{
			get
			{
				return m_renderingModelIDs;
			}
			set
			{
				m_renderingModelIDs = value;
			}
		}

		internal ReportSize SizeForRendering
		{
			get
			{
				return m_sizeForRendering;
			}
			set
			{
				m_sizeForRendering = value;
			}
		}

		internal MatrixDynamicGroupExprHost ExprHost => m_exprHost;

		internal bool InOutermostSubtotalCell
		{
			get
			{
				return m_inOutermostSubtotalCell;
			}
			set
			{
				m_inOutermostSubtotalCell = value;
			}
		}

		internal MatrixHeading()
		{
		}

		internal MatrixHeading(int id, int idForReportItems, Matrix matrixDef)
			: base(id, matrixDef)
		{
			m_reportItems = new ReportItemCollection(idForReportItems, normal: false);
		}

		internal int DynamicInitialize(bool column, int level, InitializationContext context, ref double cornerSize)
		{
			m_level = level;
			m_isColumn = column;
			m_sizeValue = context.ValidateSize(ref m_size, column ? "Height" : "Width");
			cornerSize = Math.Round(cornerSize + m_sizeValue, Validator.DecimalPrecision);
			if (m_grouping == null)
			{
				if (SubHeading != null)
				{
					context.RegisterReportItems(m_reportItems);
					SubHeading.DynamicInitialize(column, ++level, context, ref cornerSize);
					context.UnRegisterReportItems(m_reportItems);
				}
				return 1;
			}
			context.ExprHostBuilder.MatrixDynamicGroupStart(m_grouping.Name);
			if (m_subtotal != null)
			{
				m_subtotal.RegisterReportItems(context);
				m_subtotal.Initialize(context);
			}
			context.Location |= LocationFlags.InGrouping;
			context.RegisterGroupingScope(m_grouping.Name, m_grouping.SimpleGroupExpressions, m_grouping.Aggregates, m_grouping.PostSortAggregates, m_grouping.RecursiveAggregates, m_grouping, isMatrixGrouping: true);
			ObjectType objectType = context.ObjectType;
			string objectName = context.ObjectName;
			context.ObjectType = ObjectType.Grouping;
			context.ObjectName = m_grouping.Name;
			Initialize(context);
			context.RegisterReportItems(m_reportItems);
			if (m_visibility != null)
			{
				m_visibility.Initialize(context, isContainer: true, tableRowCol: false);
			}
			if (SubHeading != null)
			{
				m_subtotalSpan = SubHeading.DynamicInitialize(column, ++level, context, ref cornerSize);
			}
			else
			{
				m_subtotalSpan = 1;
			}
			m_reportItems.Initialize(context, registerRunningValues: true);
			if (m_visibility != null)
			{
				m_visibility.UnRegisterReceiver(context);
			}
			context.UnRegisterReportItems(m_reportItems);
			context.ObjectType = objectType;
			context.ObjectName = objectName;
			context.UnRegisterGroupingScope(m_grouping.Name, isMatrixGrouping: true);
			if (m_subtotal != null)
			{
				m_subtotal.UnregisterReportItems(context);
			}
			m_hasExprHost = context.ExprHostBuilder.MatrixDynamicGroupEnd(column);
			return m_subtotalSpan + 1;
		}

		internal void DynamicRegisterReceiver(InitializationContext context)
		{
			if (m_grouping == null)
			{
				if (SubHeading != null)
				{
					context.RegisterReportItems(m_reportItems);
					SubHeading.DynamicRegisterReceiver(context);
					context.UnRegisterReportItems(m_reportItems);
				}
				return;
			}
			if (m_subtotal != null)
			{
				m_subtotal.RegisterReceiver(context);
			}
			context.RegisterReportItems(m_reportItems);
			if (m_visibility != null)
			{
				m_visibility.RegisterReceiver(context, isContainer: true);
			}
			if (SubHeading != null)
			{
				SubHeading.DynamicRegisterReceiver(context);
			}
			m_reportItems.RegisterReceiver(context);
			if (m_visibility != null)
			{
				m_visibility.UnRegisterReceiver(context);
			}
			context.UnRegisterReportItems(m_reportItems);
		}

		internal int StaticInitialize(InitializationContext context)
		{
			if (m_grouping != null)
			{
				int num = 1;
				if (SubHeading != null)
				{
					context.Location |= LocationFlags.InGrouping;
					context.RegisterGroupingScope(m_grouping.Name, m_grouping.SimpleGroupExpressions, m_aggregates, m_postSortAggregates, m_recursiveAggregates, m_grouping, isMatrixGrouping: true);
					context.RegisterReportItems(m_reportItems);
					num = SubHeading.StaticInitialize(context);
					context.UnRegisterReportItems(m_reportItems);
					context.UnRegisterGroupingScope(m_grouping.Name, isMatrixGrouping: true);
				}
				return num + 1;
			}
			context.RegisterReportItems(m_reportItems);
			if (SubHeading != null)
			{
				m_subtotalSpan = SubHeading.StaticInitialize(context);
			}
			else
			{
				m_subtotalSpan = 1;
			}
			m_reportItems.Initialize(context, registerRunningValues: true);
			context.UnRegisterReportItems(m_reportItems);
			return 0;
		}

		internal void StaticRegisterReceiver(InitializationContext context)
		{
			if (m_grouping != null)
			{
				if (SubHeading != null)
				{
					context.RegisterReportItems(m_reportItems);
					SubHeading.StaticRegisterReceiver(context);
					context.UnRegisterReportItems(m_reportItems);
				}
				return;
			}
			context.RegisterReportItems(m_reportItems);
			if (SubHeading != null)
			{
				SubHeading.StaticRegisterReceiver(context);
			}
			m_reportItems.RegisterReceiver(context);
			context.UnRegisterReportItems(m_reportItems);
		}

		bool IPageBreakItem.IgnorePageBreaks()
		{
			return IgnorePageBreaks(m_visibility);
		}

		internal void SetExprHost(MatrixDynamicGroupExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null && base.HasExprHost);
			m_exprHost = exprHost;
			m_exprHost.SetReportObjectModel(reportObjectModel);
			ReportHierarchyNodeSetExprHost(m_exprHost, reportObjectModel);
		}

		internal ReportItem GetContent(out bool computed)
		{
			ReportItem reportItem = null;
			computed = false;
			if (m_reportItems != null && 0 < m_reportItems.Count)
			{
				m_reportItems.GetReportItem(0, out computed, out int _, out reportItem);
			}
			return reportItem;
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Size, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.SizeValue, Token.Double));
			memberInfoList.Add(new MemberInfo(MemberName.ReportItems, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemCollection));
			memberInfoList.Add(new MemberInfo(MemberName.OwcGroupExpression, Token.Boolean));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.PivotHeading, memberInfoList);
		}
	}
}
