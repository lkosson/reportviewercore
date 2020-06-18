using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class ChartLegend : ChartStyleContainer, IPersistable
	{
		private string m_name;

		private ExpressionInfo m_hidden;

		private ExpressionInfo m_position;

		private ExpressionInfo m_layout;

		private List<ChartLegendCustomItem> m_chartLegendCustomItems;

		private string m_dockToChartArea;

		private ExpressionInfo m_dockOutsideChartArea;

		private ChartLegendTitle m_chartLegendTitle;

		private ExpressionInfo m_autoFitTextDisabled;

		private ExpressionInfo m_minFontSize;

		private ExpressionInfo m_headerSeparator;

		private ExpressionInfo m_headerSeparatorColor;

		private ExpressionInfo m_columnSeparator;

		private ExpressionInfo m_columnSeparatorColor;

		private ExpressionInfo m_columnSpacing;

		private ExpressionInfo m_interlacedRows;

		private ExpressionInfo m_interlacedRowsColor;

		private ExpressionInfo m_equallySpacedItems;

		private ExpressionInfo m_reversed;

		private ExpressionInfo m_maxAutoSize;

		private ExpressionInfo m_textWrapThreshold;

		private List<ChartLegendColumn> m_chartLegendColumns;

		private int m_exprHostID;

		private ChartElementPosition m_chartElementPosition;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		[NonSerialized]
		private ChartLegendExprHost m_exprHost;

		internal string LegendName
		{
			get
			{
				return m_name;
			}
			set
			{
				m_name = value;
			}
		}

		internal ExpressionInfo Hidden
		{
			get
			{
				return m_hidden;
			}
			set
			{
				m_hidden = value;
			}
		}

		internal ExpressionInfo Position
		{
			get
			{
				return m_position;
			}
			set
			{
				m_position = value;
			}
		}

		internal ExpressionInfo Layout
		{
			get
			{
				return m_layout;
			}
			set
			{
				m_layout = value;
			}
		}

		internal string DockToChartArea
		{
			get
			{
				return m_dockToChartArea;
			}
			set
			{
				m_dockToChartArea = value;
			}
		}

		internal ExpressionInfo DockOutsideChartArea
		{
			get
			{
				return m_dockOutsideChartArea;
			}
			set
			{
				m_dockOutsideChartArea = value;
			}
		}

		internal ChartLegendTitle LegendTitle
		{
			get
			{
				return m_chartLegendTitle;
			}
			set
			{
				m_chartLegendTitle = value;
			}
		}

		internal ExpressionInfo AutoFitTextDisabled
		{
			get
			{
				return m_autoFitTextDisabled;
			}
			set
			{
				m_autoFitTextDisabled = value;
			}
		}

		internal ExpressionInfo MinFontSize
		{
			get
			{
				return m_minFontSize;
			}
			set
			{
				m_minFontSize = value;
			}
		}

		internal ExpressionInfo HeaderSeparator
		{
			get
			{
				return m_headerSeparator;
			}
			set
			{
				m_headerSeparator = value;
			}
		}

		internal ExpressionInfo HeaderSeparatorColor
		{
			get
			{
				return m_headerSeparatorColor;
			}
			set
			{
				m_headerSeparatorColor = value;
			}
		}

		internal ExpressionInfo ColumnSeparator
		{
			get
			{
				return m_columnSeparator;
			}
			set
			{
				m_columnSeparator = value;
			}
		}

		internal ExpressionInfo ColumnSeparatorColor
		{
			get
			{
				return m_columnSeparatorColor;
			}
			set
			{
				m_columnSeparatorColor = value;
			}
		}

		internal ExpressionInfo ColumnSpacing
		{
			get
			{
				return m_columnSpacing;
			}
			set
			{
				m_columnSpacing = value;
			}
		}

		internal ExpressionInfo InterlacedRows
		{
			get
			{
				return m_interlacedRows;
			}
			set
			{
				m_interlacedRows = value;
			}
		}

		internal ExpressionInfo InterlacedRowsColor
		{
			get
			{
				return m_interlacedRowsColor;
			}
			set
			{
				m_interlacedRowsColor = value;
			}
		}

		internal ExpressionInfo EquallySpacedItems
		{
			get
			{
				return m_equallySpacedItems;
			}
			set
			{
				m_equallySpacedItems = value;
			}
		}

		internal ExpressionInfo Reversed
		{
			get
			{
				return m_reversed;
			}
			set
			{
				m_reversed = value;
			}
		}

		internal ExpressionInfo MaxAutoSize
		{
			get
			{
				return m_maxAutoSize;
			}
			set
			{
				m_maxAutoSize = value;
			}
		}

		internal ExpressionInfo TextWrapThreshold
		{
			get
			{
				return m_textWrapThreshold;
			}
			set
			{
				m_textWrapThreshold = value;
			}
		}

		internal List<ChartLegendCustomItem> LegendCustomItems
		{
			get
			{
				return m_chartLegendCustomItems;
			}
			set
			{
				m_chartLegendCustomItems = value;
			}
		}

		internal List<ChartLegendColumn> LegendColumns
		{
			get
			{
				return m_chartLegendColumns;
			}
			set
			{
				m_chartLegendColumns = value;
			}
		}

		internal ChartElementPosition ChartElementPosition
		{
			get
			{
				return m_chartElementPosition;
			}
			set
			{
				m_chartElementPosition = value;
			}
		}

		internal ChartLegendExprHost ExprHost => m_exprHost;

		internal int ExpressionHostID => m_exprHostID;

		internal ChartLegend()
		{
		}

		internal ChartLegend(Chart chart)
			: base(chart)
		{
		}

		internal void SetExprHost(ChartLegendExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
			m_exprHost = exprHost;
			if (m_chartLegendTitle != null && exprHost.TitleExprHost != null)
			{
				m_chartLegendTitle.SetExprHost(exprHost.TitleExprHost, reportObjectModel);
			}
			IList<ChartLegendCustomItemExprHost> chartLegendCustomItemsHostsRemotable = m_exprHost.ChartLegendCustomItemsHostsRemotable;
			if (m_chartLegendCustomItems != null && chartLegendCustomItemsHostsRemotable != null)
			{
				for (int i = 0; i < m_chartLegendCustomItems.Count; i++)
				{
					ChartLegendCustomItem chartLegendCustomItem = m_chartLegendCustomItems[i];
					if (chartLegendCustomItem != null && chartLegendCustomItem.ExpressionHostID > -1)
					{
						chartLegendCustomItem.SetExprHost(chartLegendCustomItemsHostsRemotable[chartLegendCustomItem.ExpressionHostID], reportObjectModel);
					}
				}
			}
			IList<ChartLegendColumnExprHost> chartLegendColumnsHostsRemotable = m_exprHost.ChartLegendColumnsHostsRemotable;
			if (m_chartLegendColumns != null && chartLegendColumnsHostsRemotable != null)
			{
				for (int j = 0; j < m_chartLegendColumns.Count; j++)
				{
					ChartLegendColumn chartLegendColumn = m_chartLegendColumns[j];
					if (chartLegendColumn != null && chartLegendColumn.ExpressionHostID > -1)
					{
						chartLegendColumn.SetExprHost(chartLegendColumnsHostsRemotable[chartLegendColumn.ExpressionHostID], reportObjectModel);
					}
				}
			}
			if (m_chartElementPosition != null && m_exprHost.ChartElementPositionHost != null)
			{
				m_chartElementPosition.SetExprHost(m_exprHost.ChartElementPositionHost, reportObjectModel);
			}
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.ChartLegendStart(m_name);
			base.Initialize(context);
			_ = m_dockToChartArea;
			if (m_position != null)
			{
				m_position.Initialize("Position", context);
				context.ExprHostBuilder.ChartLegendPosition(m_position);
			}
			if (m_layout != null)
			{
				m_layout.Initialize("Layout", context);
				context.ExprHostBuilder.ChartLegendLayout(m_layout);
			}
			if (m_hidden != null)
			{
				m_hidden.Initialize("Hidden", context);
				context.ExprHostBuilder.ChartLegendHidden(m_hidden);
			}
			if (m_dockOutsideChartArea != null)
			{
				m_dockOutsideChartArea.Initialize("DockOutsideChartArea", context);
				context.ExprHostBuilder.ChartLegendDockOutsideChartArea(m_dockOutsideChartArea);
			}
			if (m_chartLegendTitle != null)
			{
				m_chartLegendTitle.Initialize(context);
			}
			if (m_autoFitTextDisabled != null)
			{
				m_autoFitTextDisabled.Initialize("AutoFitTextDisabled", context);
				context.ExprHostBuilder.ChartLegendAutoFitTextDisabled(m_autoFitTextDisabled);
			}
			if (m_minFontSize != null)
			{
				m_minFontSize.Initialize("MinFontSize", context);
				context.ExprHostBuilder.ChartLegendMinFontSize(m_minFontSize);
			}
			if (m_headerSeparator != null)
			{
				m_headerSeparator.Initialize("HeaderSeparator", context);
				context.ExprHostBuilder.ChartLegendHeaderSeparator(m_headerSeparator);
			}
			if (m_headerSeparatorColor != null)
			{
				m_headerSeparatorColor.Initialize("HeaderSeparatorColor", context);
				context.ExprHostBuilder.ChartLegendHeaderSeparatorColor(m_headerSeparatorColor);
			}
			if (m_columnSeparator != null)
			{
				m_columnSeparator.Initialize("ColumnSeparator", context);
				context.ExprHostBuilder.ChartLegendColumnSeparator(m_columnSeparator);
			}
			if (m_columnSeparatorColor != null)
			{
				m_columnSeparatorColor.Initialize("ColumnSeparatorColor", context);
				context.ExprHostBuilder.ChartLegendColumnSeparatorColor(m_columnSeparatorColor);
			}
			if (m_columnSpacing != null)
			{
				m_columnSpacing.Initialize("ColumnSpacing", context);
				context.ExprHostBuilder.ChartLegendColumnSpacing(m_columnSpacing);
			}
			if (m_interlacedRows != null)
			{
				m_interlacedRows.Initialize("InterlacedRows", context);
				context.ExprHostBuilder.ChartLegendInterlacedRows(m_interlacedRows);
			}
			if (m_interlacedRowsColor != null)
			{
				m_interlacedRowsColor.Initialize("InterlacedRowsColor", context);
				context.ExprHostBuilder.ChartLegendInterlacedRowsColor(m_interlacedRowsColor);
			}
			if (m_equallySpacedItems != null)
			{
				m_equallySpacedItems.Initialize("EquallySpacedItems", context);
				context.ExprHostBuilder.ChartLegendEquallySpacedItems(m_equallySpacedItems);
			}
			if (m_reversed != null)
			{
				m_reversed.Initialize("Reversed", context);
				context.ExprHostBuilder.ChartLegendReversed(m_reversed);
			}
			if (m_maxAutoSize != null)
			{
				m_maxAutoSize.Initialize("MaxAutoSize", context);
				context.ExprHostBuilder.ChartLegendMaxAutoSize(m_maxAutoSize);
			}
			if (m_textWrapThreshold != null)
			{
				m_textWrapThreshold.Initialize("TextWrapThreshold", context);
				context.ExprHostBuilder.ChartLegendTextWrapThreshold(m_textWrapThreshold);
			}
			if (m_chartLegendCustomItems != null)
			{
				for (int i = 0; i < m_chartLegendCustomItems.Count; i++)
				{
					m_chartLegendCustomItems[i].Initialize(context);
				}
			}
			if (m_chartLegendColumns != null)
			{
				for (int j = 0; j < m_chartLegendColumns.Count; j++)
				{
					m_chartLegendColumns[j].Initialize(context);
				}
			}
			if (m_chartElementPosition != null)
			{
				m_chartElementPosition.Initialize(context);
			}
			m_exprHostID = context.ExprHostBuilder.ChartLegendEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			ChartLegend chartLegend = (ChartLegend)base.PublishClone(context);
			if (m_position != null)
			{
				chartLegend.m_position = (ExpressionInfo)m_position.PublishClone(context);
			}
			if (m_layout != null)
			{
				chartLegend.m_layout = (ExpressionInfo)m_layout.PublishClone(context);
			}
			if (m_hidden != null)
			{
				chartLegend.m_hidden = (ExpressionInfo)m_hidden.PublishClone(context);
			}
			if (m_dockOutsideChartArea != null)
			{
				chartLegend.m_dockOutsideChartArea = (ExpressionInfo)m_dockOutsideChartArea.PublishClone(context);
			}
			if (m_chartLegendTitle != null)
			{
				chartLegend.m_chartLegendTitle = (ChartLegendTitle)m_chartLegendTitle.PublishClone(context);
			}
			if (m_autoFitTextDisabled != null)
			{
				chartLegend.m_autoFitTextDisabled = (ExpressionInfo)m_autoFitTextDisabled.PublishClone(context);
			}
			if (m_minFontSize != null)
			{
				chartLegend.m_minFontSize = (ExpressionInfo)m_minFontSize.PublishClone(context);
			}
			if (m_headerSeparator != null)
			{
				chartLegend.m_headerSeparator = (ExpressionInfo)m_headerSeparator.PublishClone(context);
			}
			if (m_headerSeparatorColor != null)
			{
				chartLegend.m_headerSeparatorColor = (ExpressionInfo)m_headerSeparatorColor.PublishClone(context);
			}
			if (m_columnSeparator != null)
			{
				chartLegend.m_columnSeparator = (ExpressionInfo)m_columnSeparator.PublishClone(context);
			}
			if (m_columnSeparatorColor != null)
			{
				chartLegend.m_columnSeparatorColor = (ExpressionInfo)m_columnSeparatorColor.PublishClone(context);
			}
			if (m_columnSpacing != null)
			{
				chartLegend.m_columnSpacing = (ExpressionInfo)m_columnSpacing.PublishClone(context);
			}
			if (m_interlacedRows != null)
			{
				chartLegend.m_interlacedRows = (ExpressionInfo)m_interlacedRows.PublishClone(context);
			}
			if (m_interlacedRowsColor != null)
			{
				chartLegend.m_interlacedRowsColor = (ExpressionInfo)m_interlacedRowsColor.PublishClone(context);
			}
			if (m_equallySpacedItems != null)
			{
				chartLegend.m_equallySpacedItems = (ExpressionInfo)m_equallySpacedItems.PublishClone(context);
			}
			if (m_reversed != null)
			{
				chartLegend.m_reversed = (ExpressionInfo)m_reversed.PublishClone(context);
			}
			if (m_maxAutoSize != null)
			{
				chartLegend.m_maxAutoSize = (ExpressionInfo)m_maxAutoSize.PublishClone(context);
			}
			if (m_textWrapThreshold != null)
			{
				chartLegend.m_textWrapThreshold = (ExpressionInfo)m_textWrapThreshold.PublishClone(context);
			}
			if (m_chartLegendCustomItems != null)
			{
				chartLegend.m_chartLegendCustomItems = new List<ChartLegendCustomItem>(m_chartLegendCustomItems.Count);
				foreach (ChartLegendCustomItem chartLegendCustomItem in m_chartLegendCustomItems)
				{
					chartLegend.m_chartLegendCustomItems.Add((ChartLegendCustomItem)chartLegendCustomItem.PublishClone(context));
				}
			}
			if (m_chartLegendColumns != null)
			{
				chartLegend.m_chartLegendColumns = new List<ChartLegendColumn>(m_chartLegendColumns.Count);
				foreach (ChartLegendColumn chartLegendColumn in m_chartLegendColumns)
				{
					chartLegend.m_chartLegendColumns.Add((ChartLegendColumn)chartLegendColumn.PublishClone(context));
				}
			}
			if (m_chartElementPosition != null)
			{
				chartLegend.m_chartElementPosition = (ChartElementPosition)m_chartElementPosition.PublishClone(context);
			}
			return chartLegend;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Name, Token.String));
			list.Add(new MemberInfo(MemberName.Hidden, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Position, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Layout, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.DockToChartArea, Token.String));
			list.Add(new MemberInfo(MemberName.DockOutsideChartArea, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ChartLegendTitle, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartLegendTitle));
			list.Add(new MemberInfo(MemberName.AutoFitTextDisabled, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MinFontSize, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.HeaderSeparator, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.HeaderSeparatorColor, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ColumnSeparator, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ColumnSeparatorColor, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ColumnSpacing, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.InterlacedRows, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.InterlacedRowsColor, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.EquallySpacedItems, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Reversed, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MaxAutoSize, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.TextWrapThreshold, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ChartLegendCustomItems, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartLegendCustomItem));
			list.Add(new MemberInfo(MemberName.ChartLegendColumns, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartLegendColumn));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.ChartElementPosition, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartElementPosition));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartLegend, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartStyleContainer, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Name:
					writer.Write(m_name);
					break;
				case MemberName.Hidden:
					writer.Write(m_hidden);
					break;
				case MemberName.Position:
					writer.Write(m_position);
					break;
				case MemberName.Layout:
					writer.Write(m_layout);
					break;
				case MemberName.DockToChartArea:
					writer.Write(m_dockToChartArea);
					break;
				case MemberName.DockOutsideChartArea:
					writer.Write(m_dockOutsideChartArea);
					break;
				case MemberName.ChartLegendTitle:
					writer.Write(m_chartLegendTitle);
					break;
				case MemberName.AutoFitTextDisabled:
					writer.Write(m_autoFitTextDisabled);
					break;
				case MemberName.MinFontSize:
					writer.Write(m_minFontSize);
					break;
				case MemberName.HeaderSeparator:
					writer.Write(m_headerSeparator);
					break;
				case MemberName.HeaderSeparatorColor:
					writer.Write(m_headerSeparatorColor);
					break;
				case MemberName.ColumnSeparator:
					writer.Write(m_columnSeparator);
					break;
				case MemberName.ColumnSeparatorColor:
					writer.Write(m_columnSeparatorColor);
					break;
				case MemberName.ColumnSpacing:
					writer.Write(m_columnSpacing);
					break;
				case MemberName.InterlacedRows:
					writer.Write(m_interlacedRows);
					break;
				case MemberName.InterlacedRowsColor:
					writer.Write(m_interlacedRowsColor);
					break;
				case MemberName.EquallySpacedItems:
					writer.Write(m_equallySpacedItems);
					break;
				case MemberName.Reversed:
					writer.Write(m_reversed);
					break;
				case MemberName.MaxAutoSize:
					writer.Write(m_maxAutoSize);
					break;
				case MemberName.TextWrapThreshold:
					writer.Write(m_textWrapThreshold);
					break;
				case MemberName.ChartLegendColumns:
					writer.Write(m_chartLegendColumns);
					break;
				case MemberName.ExprHostID:
					writer.Write(m_exprHostID);
					break;
				case MemberName.ChartLegendCustomItems:
					writer.Write(m_chartLegendCustomItems);
					break;
				case MemberName.ChartElementPosition:
					writer.Write(m_chartElementPosition);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Name:
					m_name = reader.ReadString();
					break;
				case MemberName.Hidden:
					m_hidden = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Position:
					m_position = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Layout:
					m_layout = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.DockToChartArea:
					m_dockToChartArea = reader.ReadString();
					break;
				case MemberName.DockOutsideChartArea:
					m_dockOutsideChartArea = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ChartLegendTitle:
					m_chartLegendTitle = (ChartLegendTitle)reader.ReadRIFObject();
					break;
				case MemberName.AutoFitTextDisabled:
					m_autoFitTextDisabled = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MinFontSize:
					m_minFontSize = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.HeaderSeparator:
					m_headerSeparator = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.HeaderSeparatorColor:
					m_headerSeparatorColor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ColumnSeparator:
					m_columnSeparator = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ColumnSeparatorColor:
					m_columnSeparatorColor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ColumnSpacing:
					m_columnSpacing = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.InterlacedRows:
					m_interlacedRows = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.InterlacedRowsColor:
					m_interlacedRowsColor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.EquallySpacedItems:
					m_equallySpacedItems = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Reversed:
					m_reversed = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MaxAutoSize:
					m_maxAutoSize = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.TextWrapThreshold:
					m_textWrapThreshold = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ExprHostID:
					m_exprHostID = reader.ReadInt32();
					break;
				case MemberName.ChartLegendCustomItems:
					m_chartLegendCustomItems = reader.ReadGenericListOfRIFObjects<ChartLegendCustomItem>();
					break;
				case MemberName.ChartLegendColumns:
					m_chartLegendColumns = reader.ReadGenericListOfRIFObjects<ChartLegendColumn>();
					break;
				case MemberName.ChartElementPosition:
					m_chartElementPosition = (ChartElementPosition)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			base.ResolveReferences(memberReferencesCollection, referenceableItems);
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartLegend;
		}

		internal bool EvaluateHidden(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendHiddenExpression(this, m_chart.Name, "Hidden");
		}

		internal ChartLegendPositions EvaluatePosition(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return EnumTranslator.TranslateChartLegendPositions(context.ReportRuntime.EvaluateChartLegendPositionExpression(this, m_chart.Name, "Position"), context.ReportRuntime);
		}

		internal ChartLegendLayouts EvaluateLayout(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return EnumTranslator.TranslateChartLegendLayout(context.ReportRuntime.EvaluateChartLegendLayoutExpression(this, m_chart.Name, "Layout"), context.ReportRuntime);
		}

		internal bool EvaluateDockOutsideChartArea(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendDockOutsideChartAreaExpression(this, m_chart.Name, "DockOutsideChartArea");
		}

		internal bool EvaluateAutoFitTextDisabled(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendAutoFitTextDisabledExpression(this, m_chart.Name, "AutoFitTextDisabled");
		}

		internal string EvaluateMinFontSize(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendMinFontSizeExpression(this, m_chart.Name, "MinFontSize");
		}

		internal ChartSeparators EvaluateHeaderSeparator(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return EnumTranslator.TranslateChartSeparator(context.ReportRuntime.EvaluateChartLegendHeaderSeparatorExpression(this, m_chart.Name, "HeaderSeparator"), context.ReportRuntime);
		}

		internal string EvaluateHeaderSeparatorColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendHeaderSeparatorColorExpression(this, m_chart.Name, "HeaderSeparatorColor");
		}

		internal ChartSeparators EvaluateColumnSeparator(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return EnumTranslator.TranslateChartSeparator(context.ReportRuntime.EvaluateChartLegendColumnSeparatorExpression(this, m_chart.Name, "ColumnSeparator"), context.ReportRuntime);
		}

		internal string EvaluateColumnSeparatorColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendColumnSeparatorColorExpression(this, m_chart.Name, "ColumnSeparatorColor");
		}

		internal int EvaluateColumnSpacing(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendColumnSpacingExpression(this, m_chart.Name, "ColumnSpacing");
		}

		internal bool EvaluateInterlacedRows(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendInterlacedRowsExpression(this, m_chart.Name, "InterlacedRows");
		}

		internal string EvaluateInterlacedRowsColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendInterlacedRowsColorExpression(this, m_chart.Name, "InterlacedRowsColor");
		}

		internal bool EvaluateEquallySpacedItems(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendEquallySpacedItemsExpression(this, m_chart.Name, "EquallySpacedItems");
		}

		internal ChartAutoBool EvaluateReversed(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return EnumTranslator.TranslateChartAutoBool(context.ReportRuntime.EvaluateChartLegendReversedExpression(this, m_chart.Name), context.ReportRuntime);
		}

		internal int EvaluateMaxAutoSize(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendMaxAutoSizeExpression(this, m_chart.Name, "MaxAutoSize");
		}

		internal int EvaluateTextWrapThreshold(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendTextWrapThresholdExpression(this, m_chart.Name, "TextWrapThreshold");
		}
	}
}
