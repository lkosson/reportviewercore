using Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel;
using Microsoft.ReportingServices.ReportProcessing.Persistence;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using Microsoft.ReportingServices.ReportRendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class CustomReportItem : Tablix, IRunningValueHolder, IErrorContext
	{
		private string m_type;

		private ReportItemCollection m_altReportItem;

		private CustomReportItemHeadingList m_columns;

		private CustomReportItemHeadingList m_rows;

		private DataCellsList m_dataRowCells;

		private RunningValueInfoList m_cellRunningValues;

		private IntList m_cellExprHostIDs;

		private ReportItemCollection m_renderReportItem;

		[NonSerialized]
		private int m_expectedColumns;

		[NonSerialized]
		private int m_expectedRows;

		[NonSerialized]
		private CustomReportItemExprHost m_exprHost;

		[NonSerialized]
		private CustomReportItemHeadingList m_staticColumns;

		[NonSerialized]
		private bool m_staticColumnsInitialized;

		[NonSerialized]
		private CustomReportItemHeadingList m_staticRows;

		[NonSerialized]
		private bool m_staticRowsInitialized;

		[NonSerialized]
		private CustomReportItemInstance m_criInstance;

		[NonSerialized]
		private CustomReportItemInstanceInfo m_criInstanceInfo;

		[NonSerialized]
		private ReportProcessing.ProcessingContext m_processingContext;

		[NonSerialized]
		private int m_repeatedSiblingIndex = -1;

		[NonSerialized]
		private ObjectType m_customObjectType;

		[NonSerialized]
		private string m_customObjectName;

		[NonSerialized]
		private string m_customPropertyName;

		[NonSerialized]
		private string m_customTopLevelRenderItemName;

		[NonSerialized]
		private bool m_firstInstance = true;

		internal override ObjectType ObjectType => ObjectType.CustomReportItem;

		internal override TablixHeadingList TablixColumns => m_columns;

		internal override TablixHeadingList TablixRows => m_rows;

		internal override RunningValueInfoList TablixCellRunningValues => m_cellRunningValues;

		internal string Type
		{
			get
			{
				return m_type;
			}
			set
			{
				m_type = value;
			}
		}

		internal ReportItemCollection AltReportItem
		{
			get
			{
				return m_altReportItem;
			}
			set
			{
				m_altReportItem = value;
			}
		}

		internal CustomReportItemHeadingList Columns
		{
			get
			{
				return m_columns;
			}
			set
			{
				m_columns = value;
			}
		}

		internal CustomReportItemHeadingList Rows
		{
			get
			{
				return m_rows;
			}
			set
			{
				m_rows = value;
			}
		}

		internal DataCellsList DataRowCells
		{
			get
			{
				return m_dataRowCells;
			}
			set
			{
				m_dataRowCells = value;
			}
		}

		internal RunningValueInfoList CellRunningValues
		{
			get
			{
				return m_cellRunningValues;
			}
			set
			{
				m_cellRunningValues = value;
			}
		}

		internal IntList CellExprHostIDs
		{
			get
			{
				return m_cellExprHostIDs;
			}
			set
			{
				m_cellExprHostIDs = value;
			}
		}

		internal int ExpectedColumns
		{
			get
			{
				return m_expectedColumns;
			}
			set
			{
				m_expectedColumns = value;
			}
		}

		internal int ExpectedRows
		{
			get
			{
				return m_expectedRows;
			}
			set
			{
				m_expectedRows = value;
			}
		}

		internal CustomReportItemHeadingList StaticColumns
		{
			get
			{
				if (!m_staticColumnsInitialized)
				{
					InitializeStaticGroups(isRows: false);
					m_staticColumnsInitialized = true;
				}
				return m_staticColumns;
			}
		}

		internal CustomReportItemHeadingList StaticRows
		{
			get
			{
				if (!m_staticRowsInitialized)
				{
					InitializeStaticGroups(isRows: true);
					m_staticRowsInitialized = true;
				}
				return m_staticRows;
			}
		}

		internal ReportItemCollection RenderReportItem
		{
			get
			{
				return m_renderReportItem;
			}
			set
			{
				m_renderReportItem = value;
			}
		}

		internal bool FirstInstanceOfRenderReportItem
		{
			get
			{
				return m_firstInstance;
			}
			set
			{
				m_firstInstance = value;
			}
		}

		internal ReportProcessing.ProcessingContext ProcessingContext
		{
			get
			{
				return m_processingContext;
			}
			set
			{
				m_processingContext = value;
			}
		}

		internal ObjectType CustomObjectType
		{
			get
			{
				return m_customObjectType;
			}
			set
			{
				m_customObjectType = value;
			}
		}

		internal string CustomObjectName
		{
			get
			{
				return m_customObjectName;
			}
			set
			{
				m_customObjectName = value;
			}
		}

		protected override DataRegionExprHost DataRegionExprHost => m_exprHost;

		internal CustomReportItem(ReportItem parent)
			: base(parent)
		{
		}

		internal CustomReportItem(int id, int idAltReportitem, ReportItem parent)
			: base(id, parent)
		{
			m_dataRowCells = new DataCellsList();
			m_cellRunningValues = new RunningValueInfoList();
			m_altReportItem = new ReportItemCollection(idAltReportitem, normal: false);
		}

		RunningValueInfoList IRunningValueHolder.GetRunningValueList()
		{
			return m_runningValues;
		}

		void IRunningValueHolder.ClearIfEmpty()
		{
			Global.Tracer.Assert(m_cellRunningValues != null);
			if (m_cellRunningValues.Count == 0)
			{
				m_cellRunningValues = null;
			}
			Global.Tracer.Assert(m_runningValues != null);
			if (m_runningValues.Count == 0)
			{
				m_runningValues = null;
			}
		}

		internal override bool Initialize(InitializationContext context)
		{
			context.ObjectType = ObjectType;
			context.ObjectName = m_name;
			context.ExprHostBuilder.CustomReportItemStart(m_name);
			if (m_dataSetName != null)
			{
				context.RegisterDataRegion(this);
			}
			base.Initialize(context);
			if (m_altReportItem != null)
			{
				if (m_altReportItem.Count == 0)
				{
					m_altReportItem = null;
				}
				else
				{
					context.RegisterReportItems(m_altReportItem);
					m_altReportItem.Initialize(context, registerRunningValues: false);
					context.UnRegisterReportItems(m_altReportItem);
				}
			}
			context.RegisterRunningValues(m_runningValues);
			if (m_visibility != null)
			{
				m_visibility.Initialize(context, isContainer: true, tableRowCol: false);
			}
			if (m_dataSetName != null)
			{
				CustomInitialize(context);
				context.UnRegisterDataRegion(this);
			}
			if (m_visibility != null)
			{
				m_visibility.UnRegisterReceiver(context);
			}
			context.UnRegisterRunningValues(m_runningValues);
			base.ExprHostID = context.ExprHostBuilder.CustomReportItemEnd();
			return false;
		}

		private void CustomInitialize(InitializationContext context)
		{
			context.Location = (context.Location | LocationFlags.InDataSet | LocationFlags.InDataRegion);
			context.Location &= ~LocationFlags.InMatrixCellTopLevelItem;
			Global.Tracer.Assert(m_columns == null || m_expectedColumns > 0);
			Global.Tracer.Assert(m_rows == null || m_expectedRows > 0);
			if (!ValidateRDLStructure(context) || !ValidateProcessingRestrictions(context))
			{
				return;
			}
			context.AggregateEscalateScopes = new StringList();
			context.AggregateEscalateScopes.Add(m_name);
			if (m_columns != null)
			{
				int currentIndex = 0;
				int maxLevel = 0;
				m_expectedColumns += m_columns.Initialize(0, m_dataRowCells, ref currentIndex, ref maxLevel, context);
				base.ColumnCount = maxLevel + 1;
				if (1 == base.ColumnCount && m_columns[0].Static)
				{
					for (int i = 0; i < m_columns.Count; i++)
					{
						context.SpecialTransferRunningValues(m_columns[i].RunningValues);
					}
				}
			}
			if (m_rows != null)
			{
				int currentIndex2 = 0;
				int maxLevel2 = 0;
				m_expectedRows += m_rows.Initialize(0, m_dataRowCells, ref currentIndex2, ref maxLevel2, context);
				base.RowCount = maxLevel2 + 1;
				if (1 == base.RowCount && m_rows[0].Static)
				{
					for (int j = 0; j < m_rows.Count; j++)
					{
						context.SpecialTransferRunningValues(m_rows[j].RunningValues);
					}
				}
			}
			context.AggregateEscalateScopes = null;
			context.AggregateRewriteScopes = null;
			DataCellInitialize(context);
			CopyHeadingAggregates(m_rows);
			m_rows.TransferHeadingAggregates();
			CopyHeadingAggregates(m_columns);
			m_columns.TransferHeadingAggregates();
		}

		private void InitializeStaticGroups(bool isRows)
		{
			Global.Tracer.Assert(m_rows != null && m_columns != null);
			CustomReportItemHeadingList customReportItemHeadingList = isRows ? m_rows : m_columns;
			int num = 0;
			while (true)
			{
				if (customReportItemHeadingList != null)
				{
					num++;
					if (customReportItemHeadingList[0].Static)
					{
						break;
					}
					customReportItemHeadingList = (CustomReportItemHeadingList)customReportItemHeadingList.InnerHeadings();
					continue;
				}
				return;
			}
			if (isRows)
			{
				m_staticRows = customReportItemHeadingList;
			}
			else
			{
				m_staticColumns = customReportItemHeadingList;
			}
			Global.Tracer.Assert(customReportItemHeadingList.InnerHeadings() == null);
		}

		private bool ValidateProcessingRestrictions(InitializationContext context)
		{
			if (m_expectedColumns * m_expectedRows == 0)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsMissingDataGroupings, Severity.Error, context.ObjectType, context.ObjectName, (m_expectedColumns == 0) ? "DataColumnGroupings" : "DataRowGroupings");
				return false;
			}
			if (m_dataRowCells == null || m_dataRowCells.Count == 0)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsMissingDataCells, Severity.Error, context.ObjectType, context.ObjectName, "DataRows");
				return false;
			}
			Global.Tracer.Assert(m_rows != null && m_columns != null);
			if (CustomReportItemHeading.ValidateProcessingRestrictions(m_rows, isColumn: false, hasStatic: false, context))
			{
				return CustomReportItemHeading.ValidateProcessingRestrictions(m_columns, isColumn: true, hasStatic: false, context);
			}
			return false;
		}

		private bool ValidateRDLStructure(InitializationContext context)
		{
			if (m_dataRowCells == null || m_dataRowCells.Count == 0)
			{
				if (m_expectedColumns * m_expectedRows != 0)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsWrongNumberOfDataRows, Severity.Error, context.ObjectType, context.ObjectName, m_expectedRows.ToString(CultureInfo.InvariantCulture.NumberFormat));
					return false;
				}
			}
			else
			{
				if (m_expectedRows == 0)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsMissingDataGrouping, Severity.Error, context.ObjectType, context.ObjectName, "DataRowGroupings");
				}
				if (m_expectedColumns == 0)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsMissingDataGrouping, Severity.Error, context.ObjectType, context.ObjectName, "DataColumnGroupings");
				}
				if (m_expectedRows * m_expectedColumns == 0)
				{
					return false;
				}
				int count = m_dataRowCells.Count;
				if (m_expectedRows != count)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsWrongNumberOfDataRows, Severity.Error, context.ObjectType, context.ObjectName, m_expectedRows.ToString(CultureInfo.InvariantCulture.NumberFormat));
					return false;
				}
				bool flag = false;
				for (int i = 0; i < count; i++)
				{
					if (flag)
					{
						break;
					}
					Global.Tracer.Assert(m_dataRowCells[i] != null);
					if (m_dataRowCells[i].Count != m_expectedColumns)
					{
						flag = true;
					}
				}
				if (flag)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsWrongNumberOfDataCellsInDataRow, Severity.Error, context.ObjectType, context.ObjectName, m_expectedColumns.ToString(CultureInfo.InvariantCulture.NumberFormat));
					return false;
				}
			}
			return true;
		}

		private void DataCellInitialize(InitializationContext context)
		{
			context.Location |= LocationFlags.InMatrixCell;
			context.MatrixName = m_name;
			context.RegisterTablixCellScope(1 == m_expectedColumns && m_columns[0].Grouping == null, m_cellAggregates, m_cellPostSortAggregates);
			Global.Tracer.Assert(m_expectedColumns * m_expectedRows != 0);
			m_cellExprHostIDs = new IntList(m_expectedColumns * m_expectedRows);
			context.RegisterRunningValues(m_cellRunningValues);
			SetupRowScopesAndInitialize(m_rows, 0, context);
			context.UnRegisterRunningValues(m_cellRunningValues);
			if (context.IsRunningValueDirectionColumn())
			{
				m_processingInnerGrouping = Pivot.ProcessingInnerGroupings.Row;
			}
			context.UnRegisterTablixCellScope();
		}

		private void SetupRowScopesAndInitialize(CustomReportItemHeadingList rowHeadings, int cellRowIndex, InitializationContext context)
		{
			Global.Tracer.Assert(rowHeadings != null);
			int count = rowHeadings.Count;
			for (int i = 0; i < count; i++)
			{
				CustomReportItemHeading customReportItemHeading = rowHeadings[i];
				if (customReportItemHeading.Grouping != null)
				{
					context.Location |= LocationFlags.InGrouping;
					context.RegisterGroupingScopeForTablixCell(customReportItemHeading.Grouping.Name, column: false, customReportItemHeading.Grouping.SimpleGroupExpressions, customReportItemHeading.Aggregates, customReportItemHeading.PostSortAggregates, customReportItemHeading.RecursiveAggregates, customReportItemHeading.Grouping);
				}
				if (customReportItemHeading.InnerHeadings != null)
				{
					SetupRowScopesAndInitialize(customReportItemHeading.InnerHeadings, cellRowIndex, context);
				}
				else
				{
					Global.Tracer.Assert(m_dataRowCells != null && cellRowIndex < m_dataRowCells.Count);
					int cellIndex = 0;
					SetupColumnScopesAndInitialize(m_columns, m_dataRowCells[cellRowIndex], ref cellIndex, context);
					Global.Tracer.Assert(cellIndex == m_dataRowCells[cellRowIndex].Count);
					cellRowIndex++;
				}
				if (customReportItemHeading.Grouping != null)
				{
					context.UnRegisterGroupingScopeForTablixCell(customReportItemHeading.Grouping.Name, column: false);
				}
			}
		}

		private void SetupColumnScopesAndInitialize(CustomReportItemHeadingList columnHeadings, DataCellList cellList, ref int cellIndex, InitializationContext context)
		{
			Global.Tracer.Assert(columnHeadings != null);
			int count = columnHeadings.Count;
			for (int i = 0; i < count; i++)
			{
				CustomReportItemHeading customReportItemHeading = columnHeadings[i];
				if (customReportItemHeading.Grouping != null)
				{
					context.Location |= LocationFlags.InGrouping;
					context.RegisterGroupingScopeForTablixCell(customReportItemHeading.Grouping.Name, column: true, customReportItemHeading.Grouping.SimpleGroupExpressions, customReportItemHeading.Aggregates, customReportItemHeading.PostSortAggregates, customReportItemHeading.RecursiveAggregates, customReportItemHeading.Grouping);
				}
				if (customReportItemHeading.InnerHeadings != null)
				{
					SetupColumnScopesAndInitialize(customReportItemHeading.InnerHeadings, cellList, ref cellIndex, context);
				}
				else
				{
					context.ExprHostBuilder.DataCellStart();
					cellList[cellIndex].Initialize(null, context);
					m_cellExprHostIDs.Add(context.ExprHostBuilder.DataCellEnd());
					cellIndex++;
				}
				if (customReportItemHeading.Grouping != null)
				{
					context.UnRegisterGroupingScopeForTablixCell(customReportItemHeading.Grouping.Name, column: true);
				}
			}
		}

		internal void CopyHeadingAggregates(CustomReportItemHeadingList headings)
		{
			if (headings != null)
			{
				int count = headings.Count;
				for (int i = 0; i < count; i++)
				{
					CustomReportItemHeading customReportItemHeading = headings[i];
					customReportItemHeading.CopySubHeadingAggregates();
					Tablix.CopyAggregates(customReportItemHeading.Aggregates, m_aggregates);
					Tablix.CopyAggregates(customReportItemHeading.PostSortAggregates, m_postSortAggregates);
					Tablix.CopyAggregates(customReportItemHeading.RecursiveAggregates, m_aggregates);
				}
			}
		}

		internal override int GetDynamicHeadingCount(bool outerGroupings)
		{
			int num;
			if ((outerGroupings && m_processingInnerGrouping == Pivot.ProcessingInnerGroupings.Column) || (!outerGroupings && m_processingInnerGrouping == Pivot.ProcessingInnerGroupings.Row))
			{
				num = base.RowCount;
				if (StaticRows != null)
				{
					num--;
				}
			}
			else
			{
				num = base.ColumnCount;
				if (StaticColumns != null)
				{
					num--;
				}
			}
			return num;
		}

		internal override TablixHeadingList SkipStatics(TablixHeadingList headings)
		{
			Global.Tracer.Assert(headings != null && 1 <= headings.Count && headings is CustomReportItemHeadingList);
			if (((CustomReportItemHeadingList)headings)[0].Static)
			{
				return null;
			}
			return headings;
		}

		internal override void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			if (base.ExprHostID < 0)
			{
				return;
			}
			Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null);
			m_exprHost = reportExprHost.CustomReportItemHostsRemotable[base.ExprHostID];
			DataRegionSetExprHost(m_exprHost, reportObjectModel);
			if (m_exprHost.DataCellHostsRemotable == null)
			{
				return;
			}
			IList<DataCellExprHost> dataCellHostsRemotable = m_exprHost.DataCellHostsRemotable;
			int num = 0;
			Global.Tracer.Assert(m_dataRowCells.Count <= m_cellExprHostIDs.Count);
			for (int i = 0; i < m_dataRowCells.Count; i++)
			{
				DataCellList dataCellList = m_dataRowCells[i];
				Global.Tracer.Assert(dataCellList != null);
				for (int j = 0; j < dataCellList.Count; j++)
				{
					DataValueList dataValueList = dataCellList[j];
					Global.Tracer.Assert(dataValueList != null);
					int num2 = m_cellExprHostIDs[num++];
					if (num2 >= 0)
					{
						dataValueList.SetExprHost(dataCellHostsRemotable[num2].DataValueHostsRemotable, reportObjectModel);
					}
				}
			}
		}

		internal override Hashtable GetOuterScopeNames(int dynamicLevel)
		{
			Hashtable hashtable = new Hashtable();
			CustomReportItemHeadingList customReportItemHeadingList = (CustomReportItemHeadingList)GetOuterHeading();
			int num = 0;
			while (num <= dynamicLevel && customReportItemHeadingList != null)
			{
				if (customReportItemHeadingList[0].Grouping != null)
				{
					hashtable.Add(customReportItemHeadingList[0].Grouping.Name, customReportItemHeadingList[0].Grouping);
					num++;
				}
				customReportItemHeadingList = (CustomReportItemHeadingList)customReportItemHeadingList.InnerHeadings();
			}
			return hashtable;
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Type, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.ReportItemColDef, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemCollection));
			memberInfoList.Add(new MemberInfo(MemberName.Columns, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.CustomReportItemHeadingList));
			memberInfoList.Add(new MemberInfo(MemberName.Rows, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.CustomReportItemHeadingList));
			memberInfoList.Add(new MemberInfo(MemberName.DataRowCells, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.DataCellsList));
			memberInfoList.Add(new MemberInfo(MemberName.CellRunningValues, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.RunningValueInfoList));
			memberInfoList.Add(new MemberInfo(MemberName.CellExprHostIDs, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.IntList));
			memberInfoList.Add(new MemberInfo(MemberName.RenderReportItemColDef, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemCollection));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.Tablix, memberInfoList);
		}

		internal void CustomProcessingInitialize(CustomReportItemInstance instance, CustomReportItemInstanceInfo instanceInfo, ReportProcessing.ProcessingContext context, int repeatedSiblingIndex)
		{
			m_criInstance = instance;
			m_criInstanceInfo = instanceInfo;
			m_processingContext = context;
			m_repeatedSiblingIndex = repeatedSiblingIndex;
			m_customObjectType = ObjectType.CustomReportItem;
			m_customObjectName = m_name;
			m_customPropertyName = null;
		}

		internal void CustomProcessingReset()
		{
			m_criInstance = null;
			m_criInstanceInfo = null;
			m_processingContext = null;
			m_repeatedSiblingIndex = -1;
			m_customObjectType = ObjectType.CustomReportItem;
			m_customObjectName = m_name;
			m_customPropertyName = null;
		}

		internal void DeconstructRenderItem(Microsoft.ReportingServices.ReportRendering.ReportItem renderItem, CustomReportItemInstance criInstance)
		{
			if (FirstInstanceOfRenderReportItem)
			{
				m_renderReportItem = null;
				FirstInstanceOfRenderReportItem = false;
			}
			if (renderItem == null)
			{
				if (FirstInstanceOfRenderReportItem)
				{
					m_renderReportItem = null;
					m_processingContext.ErrorContext.Register(ProcessingErrorCode.rsCRIRenderItemNull, Severity.Error, m_customObjectType, m_customObjectName, m_type);
					throw new ReportProcessingException(m_processingContext.ErrorContext.Messages);
				}
				m_renderReportItem = null;
				m_processingContext.ErrorContext.Register(ProcessingErrorCode.rsCRIRenderInstanceNull, Severity.Error, m_customObjectType, m_customObjectName, m_type);
				throw new ReportProcessingException(m_processingContext.ErrorContext.Messages);
			}
			bool flag = m_renderReportItem == null;
			if (flag)
			{
				m_renderReportItem = new ReportItemCollection();
				m_renderReportItem.ID = m_processingContext.CreateIDForSubreport();
			}
			else
			{
				Global.Tracer.Assert(1 == m_renderReportItem.Count);
				if (renderItem.Name != m_customTopLevelRenderItemName)
				{
					m_processingContext.ErrorContext.Register(ProcessingErrorCode.rsCRIRenderItemDefinitionName, Severity.Error, ObjectType.CustomReportItem, renderItem.Name, m_type, m_name, m_customTopLevelRenderItemName);
					throw new ReportProcessingException(m_processingContext.ErrorContext.Messages);
				}
			}
			if (!(renderItem is Microsoft.ReportingServices.ReportRendering.Image))
			{
				return;
			}
			Microsoft.ReportingServices.ReportRendering.Image image = renderItem as Microsoft.ReportingServices.ReportRendering.Image;
			m_customObjectType = ObjectType.Image;
			m_customObjectName = image.Name;
			Image image2 = null;
			if (m_renderReportItem.Count == 0)
			{
				m_customTopLevelRenderItemName = image.Name;
				image2 = DeconstructImageDefinition(image, isRootItem: true);
				m_renderReportItem.AddCustomRenderItem(image2);
			}
			else
			{
				image2 = (m_renderReportItem[0] as Image);
				if (image2 == null)
				{
					m_processingContext.ErrorContext.Register(ProcessingErrorCode.rsCRIRenderItemInstanceType, Severity.Error, m_renderReportItem[0].ObjectType, m_renderReportItem[0].Name, m_type, m_name, ErrorContext.GetLocalizedObjectTypeString(m_customObjectType));
					throw new ReportProcessingException(m_processingContext.ErrorContext.Messages);
				}
			}
			ImageInstance imageInstance = new ImageInstance(m_processingContext, image2, m_repeatedSiblingIndex, customCreated: true);
			ImageInstanceInfo instanceInfo = imageInstance.InstanceInfo;
			DeconstructImageInstance(image2, imageInstance, instanceInfo, image, flag, isRootItem: true, this);
			criInstance.AltReportItemColInstance = new ReportItemColInstance(m_processingContext, m_renderReportItem);
			criInstance.AltReportItemColInstance.Add(imageInstance);
		}

		private void DeconstructImageInstance(Image image, ImageInstance imageInstance, ImageInstanceInfo imageInstanceInfo, Microsoft.ReportingServices.ReportRendering.Image renderImage, bool isfirstInstance, bool isRootItem, IErrorContext errorContext)
		{
			DeconstructReportItemInstance(image, imageInstance, imageInstanceInfo, renderImage, isRootItem);
			byte[] imageData = renderImage.Processing.m_imageData;
			string mimeType = renderImage.Processing.m_mimeType;
			if (!Validator.ValidateMimeType(mimeType))
			{
				m_customPropertyName = "MIMEType";
				if (mimeType == null)
				{
					errorContext.Register(ProcessingErrorCode.rsMissingMIMEType, Severity.Warning);
				}
				else
				{
					errorContext.Register(ProcessingErrorCode.rsInvalidMIMEType, Severity.Warning, mimeType);
				}
				imageInstanceInfo.BrokenImage = true;
			}
			if (imageData == null)
			{
				imageInstanceInfo.BrokenImage = true;
			}
			else if (m_processingContext.InPageSection && !m_processingContext.CreatePageSectionImageChunks)
			{
				imageInstanceInfo.Data = new ImageData(imageData, mimeType);
			}
			else if (m_processingContext.CreateReportChunkCallback != null && imageData.Length != 0)
			{
				string text = Guid.NewGuid().ToString();
				using (Stream stream = m_processingContext.CreateReportChunkCallback(text, ReportProcessing.ReportChunkTypes.Image, mimeType))
				{
					stream.Write(imageData, 0, imageData.Length);
				}
				m_processingContext.ImageStreamNames[text] = new ImageInfo(text, mimeType);
				imageInstanceInfo.ImageValue = text;
			}
			else
			{
				imageInstanceInfo.BrokenImage = true;
			}
			Global.Tracer.Assert(image.Action == null || renderImage.ActionInfo != null);
			if (renderImage.ActionInfo != null)
			{
				Action action = image.Action;
				renderImage.ActionInfo.Deconstruct(imageInstance.UniqueName, ref action, out ActionInstance actionInstance, this);
				image.Action = action;
				imageInstanceInfo.Action = actionInstance;
			}
			Style style = image.StyleClass;
			DeconstructRenderStyle(isfirstInstance, ((Microsoft.ReportingServices.ReportRendering.ReportItem)renderImage).Processing.SharedStyles, ((Microsoft.ReportingServices.ReportRendering.ReportItem)renderImage).Processing.NonSharedStyles, ref style, out object[] styleAttributeValues, this);
			image.StyleClass = style;
			imageInstanceInfo.StyleAttributeValues = styleAttributeValues;
			if (renderImage.ImageMap != null)
			{
				imageInstanceInfo.ImageMapAreas = renderImage.ImageMap.Deconstruct(m_processingContext, this);
			}
		}

		private Image DeconstructImageDefinition(Microsoft.ReportingServices.ReportRendering.Image renderItem, bool isRootItem)
		{
			Image image = new Image(m_processingContext.CreateIDForSubreport(), m_parent);
			DeconstructReportItemDefinition(image, renderItem, isRootItem);
			image.Source = Image.SourceType.External;
			image.Value = new ExpressionInfo(ExpressionInfo.Types.Expression);
			image.MIMEType = new ExpressionInfo(ExpressionInfo.Types.Expression);
			image.Sizing = (Image.Sizings)renderItem.Sizing;
			return image;
		}

		private void SetLabel(string label, ReportItem definition, ReportItemInstance instance, ReportItemInstanceInfo instanceInfo)
		{
			if (label != null)
			{
				Global.Tracer.Assert(instance != null && instanceInfo != null);
				instanceInfo.Label = label;
				m_processingContext.NavigationInfo.AddToDocumentMap(instance.GetDocumentMapUniqueName(), isContainer: false, definition.StartPage, label);
			}
		}

		private void SetBookmark(string bookmark, ReportItem definition, ReportItemInstance instance, ReportItemInstanceInfo instanceInfo)
		{
			if (bookmark != null)
			{
				Global.Tracer.Assert(instance != null && instanceInfo != null);
				m_processingContext.NavigationInfo.ProcessBookmark(definition, instance, instanceInfo, bookmark);
			}
		}

		private void DeconstructReportItemInstance(ReportItem definition, ReportItemInstance instance, ReportItemInstanceInfo instanceInfo, Microsoft.ReportingServices.ReportRendering.ReportItem renderItem, bool isRootItem)
		{
			Global.Tracer.Assert(definition != null && instanceInfo != null && renderItem != null && renderItem.Processing != null);
			instanceInfo.ToolTip = renderItem.Processing.Tooltip;
			definition.StartPage = m_criInstance.ReportItemDef.StartPage;
			if (!isRootItem)
			{
				SetLabel(renderItem.Processing.Label, definition, instance, instanceInfo);
			}
			else
			{
				string text = null;
				if (base.Label != null)
				{
					text = ((base.Label.Type != 0) ? base.Label.Value : m_criInstanceInfo.Label);
				}
				if (text == null)
				{
					text = renderItem.Processing.Label;
				}
				SetLabel(text, definition, instance, instanceInfo);
			}
			if (!isRootItem)
			{
				SetBookmark(renderItem.Processing.Bookmark, definition, instance, instanceInfo);
			}
			else
			{
				string text2 = null;
				if (base.Bookmark != null)
				{
					text2 = ((base.Bookmark.Type != 0) ? base.Bookmark.Value : m_criInstanceInfo.Bookmark);
				}
				if (text2 == null)
				{
					text2 = renderItem.Processing.Bookmark;
				}
				SetBookmark(text2, definition, instance, instanceInfo);
			}
			if (definition.CustomProperties == null && renderItem.CustomProperties != null)
			{
				Global.Tracer.Assert(0 < renderItem.CustomProperties.Count);
				m_processingContext.ErrorContext.Register(ProcessingErrorCode.rsCRIRenderItemProperties, Severity.Error, m_customObjectType, m_customObjectName, m_type, base.Name, "CustomProperties", "0", renderItem.CustomProperties.Count.ToString(CultureInfo.InvariantCulture));
				throw new ReportProcessingException(m_processingContext.ErrorContext.Messages);
			}
			if (definition.CustomProperties != null && (renderItem.CustomProperties == null || definition.CustomProperties.Count != renderItem.CustomProperties.Count))
			{
				int num = (renderItem.CustomProperties != null) ? renderItem.CustomProperties.Count : 0;
				m_processingContext.ErrorContext.Register(ProcessingErrorCode.rsCRIRenderItemProperties, Severity.Error, m_customObjectType, m_customObjectName, m_type, base.Name, "CustomProperties", definition.CustomProperties.Count.ToString(CultureInfo.InvariantCulture), num.ToString(CultureInfo.InvariantCulture));
				throw new ReportProcessingException(m_processingContext.ErrorContext.Messages);
			}
			if (renderItem.CustomProperties != null)
			{
				int count = renderItem.CustomProperties.Count;
				Global.Tracer.Assert(definition.CustomProperties != null && count == renderItem.CustomProperties.Count);
				instanceInfo.CustomPropertyInstances = renderItem.CustomProperties.Deconstruct();
			}
			if (isRootItem)
			{
				instanceInfo.StartHidden = base.StartHidden;
			}
		}

		private void DeconstructReportItemDefinition(ReportItem definition, Microsoft.ReportingServices.ReportRendering.ReportItem renderItem, bool isRootItem)
		{
			Global.Tracer.Assert(definition != null && renderItem != null);
			definition.Name = base.Name + "." + renderItem.Name;
			definition.DataElementName = definition.Name;
			definition.ZIndex = renderItem.ZIndex;
			definition.ToolTip = new ExpressionInfo(ExpressionInfo.Types.Expression);
			definition.Label = new ExpressionInfo(ExpressionInfo.Types.Expression);
			definition.Bookmark = new ExpressionInfo(ExpressionInfo.Types.Expression);
			definition.IsFullSize = false;
			definition.RepeatedSibling = false;
			definition.Computed = true;
			if (renderItem.CustomProperties != null)
			{
				int count = renderItem.CustomProperties.Count;
				definition.CustomProperties = new DataValueList(count);
				for (int i = 0; i < count; i++)
				{
					DataValue dataValue = new DataValue();
					dataValue.Name = new ExpressionInfo(ExpressionInfo.Types.Expression);
					dataValue.Value = new ExpressionInfo(ExpressionInfo.Types.Expression);
					definition.CustomProperties.Add(dataValue);
				}
			}
			if (!isRootItem)
			{
				definition.Top = renderItem.Top.ToString();
				definition.TopValue = renderItem.Top.ToMillimeters();
				definition.Left = renderItem.Left.ToString();
				definition.LeftValue = renderItem.Left.ToMillimeters();
				definition.Height = renderItem.Height.ToString();
				definition.HeightValue = renderItem.Height.ToMillimeters();
				definition.Width = renderItem.Width.ToString();
				definition.WidthValue = renderItem.Width.ToMillimeters();
			}
			else
			{
				OverrideDefinitionSettings(definition);
			}
		}

		internal static void DeconstructRenderStyle(bool firstStyleInstance, DataValueInstanceList sharedStyles, DataValueInstanceList nonSharedStyles, ref Style style, out object[] styleAttributeValues, CustomReportItem context)
		{
			styleAttributeValues = null;
			int num = sharedStyles?.Count ?? 0;
			int num2 = nonSharedStyles?.Count ?? 0;
			int num3 = num + num2;
			if (style != null && num3 == 0)
			{
				Global.Tracer.Assert(style.StyleAttributes != null && 0 <= style.StyleAttributes.Count - style.CustomSharedStyleCount);
				styleAttributeValues = new object[style.StyleAttributes.Count - style.CustomSharedStyleCount];
				return;
			}
			if (style == null && !firstStyleInstance && 0 < num3)
			{
				context.ProcessingContext.ErrorContext.Register(ProcessingErrorCode.rsCRIRenderItemProperties, Severity.Error, context.CustomObjectType, context.CustomObjectName, context.Type, context.Name, "Styles", "0", num3.ToString(CultureInfo.InvariantCulture));
				throw new ReportProcessingException(context.ProcessingContext.ErrorContext.Messages);
			}
			if (firstStyleInstance)
			{
				if (0 >= num3)
				{
					return;
				}
				style = new Style(ConstructionPhase.Deserializing);
				style.CustomSharedStyleCount = num;
				style.StyleAttributes = new StyleAttributeHashtable(num3);
				if (num2 > 0)
				{
					style.ExpressionList = new ExpressionInfoList(num2);
					styleAttributeValues = new object[num2];
				}
				for (int i = 0; i < num; i++)
				{
					string name = sharedStyles[i].Name;
					Global.Tracer.Assert(!style.StyleAttributes.ContainsKey(name));
					context.m_customPropertyName = name;
					object obj = ProcessingValidator.ValidateCustomStyle(name, sharedStyles[i].Value, context);
					AttributeInfo attributeInfo = new AttributeInfo();
					attributeInfo.IsExpression = false;
					if (obj != null)
					{
						if ("NumeralVariant" == name)
						{
							attributeInfo.IntValue = (int)obj;
						}
						else
						{
							attributeInfo.Value = (string)obj;
						}
					}
					style.StyleAttributes.Add(name, attributeInfo);
				}
				int num4 = 0;
				string name2;
				while (true)
				{
					if (num4 < num2)
					{
						name2 = nonSharedStyles[num4].Name;
						if (style.StyleAttributes.ContainsKey(name2))
						{
							break;
						}
						context.m_customPropertyName = name2;
						object obj2 = ProcessingValidator.ValidateCustomStyle(name2, nonSharedStyles[num4].Value, context);
						int count = style.ExpressionList.Count;
						ExpressionInfo value = new ExpressionInfo(ExpressionInfo.Types.Expression);
						style.ExpressionList.Add(value);
						AttributeInfo attributeInfo2 = new AttributeInfo();
						attributeInfo2.IsExpression = true;
						attributeInfo2.IntValue = count;
						style.StyleAttributes.Add(name2, attributeInfo2);
						styleAttributeValues[num4] = obj2;
						num4++;
						continue;
					}
					return;
				}
				context.ProcessingContext.ErrorContext.Register(ProcessingErrorCode.rsCRIRenderItemDuplicateStyle, Severity.Error, context.CustomObjectType, context.CustomObjectName, context.Type, context.Name, name2);
				throw new ReportProcessingException(context.ProcessingContext.ErrorContext.Messages);
			}
			if (sharedStyles != null && sharedStyles.Count != style.CustomSharedStyleCount)
			{
				context.ProcessingContext.ErrorContext.Register(ProcessingErrorCode.rsCRIRenderItemProperties, Severity.Error, context.CustomObjectType, context.CustomObjectName, context.Type, context.Name, "SharedStyles", style.CustomSharedStyleCount.ToString(CultureInfo.InvariantCulture), sharedStyles.Count.ToString(CultureInfo.InvariantCulture));
				throw new ReportProcessingException(context.ProcessingContext.ErrorContext.Messages);
			}
			int num5 = 0;
			string text;
			while (true)
			{
				if (num5 < num2)
				{
					if (num5 == 0)
					{
						styleAttributeValues = new object[style.StyleAttributes.Count - style.CustomSharedStyleCount];
					}
					text = (context.m_customPropertyName = nonSharedStyles[num5].Name);
					object obj3 = ProcessingValidator.ValidateCustomStyle(text, nonSharedStyles[num5].Value, context);
					AttributeInfo attributeInfo3 = style.StyleAttributes[text];
					if (attributeInfo3 == null)
					{
						context.ProcessingContext.ErrorContext.Register(ProcessingErrorCode.rsCRIRenderItemInvalidStyle, Severity.Error, context.CustomObjectType, context.CustomObjectName, context.Type, context.Name, text);
						throw new ReportProcessingException(context.ProcessingContext.ErrorContext.Messages);
					}
					if (!attributeInfo3.IsExpression)
					{
						break;
					}
					int intValue = attributeInfo3.IntValue;
					Global.Tracer.Assert(0 <= intValue && intValue < styleAttributeValues.Length);
					styleAttributeValues[intValue] = obj3;
					num5++;
					continue;
				}
				return;
			}
			context.ProcessingContext.ErrorContext.Register(ProcessingErrorCode.rsCRIRenderItemInvalidStyleType, Severity.Error, context.CustomObjectType, context.CustomObjectName, context.Type, context.Name, text);
			throw new ReportProcessingException(context.ProcessingContext.ErrorContext.Messages);
		}

		private void OverrideDefinitionSettings(ReportItem target)
		{
			Global.Tracer.Assert(target != null && target.Name != null);
			target.Top = m_top;
			target.TopValue = m_topValue;
			target.Left = m_left;
			target.LeftValue = m_leftValue;
			target.Height = m_height;
			target.HeightValue = m_heightValue;
			target.Width = m_width;
			target.WidthValue = m_widthValue;
			target.Visibility = m_visibility;
			target.IsFullSize = m_isFullSize;
			target.DataElementOutput = m_dataElementOutput;
			target.DistanceBeforeTop = m_distanceBeforeTop;
			target.DistanceFromReportTop = m_distanceFromReportTop;
			target.SiblingAboveMe = m_siblingAboveMe;
		}

		internal static bool CloneObject(object o, out object clone)
		{
			clone = null;
			if (o == null || DBNull.Value == o)
			{
				return true;
			}
			if (o is string)
			{
				clone = string.Copy(o as string);
			}
			else if (o is char)
			{
				clone = '\0';
				clone = (char)o;
			}
			else if (o is bool)
			{
				clone = ((bool)o).CompareTo(value: true);
			}
			else if (o is short)
			{
				clone = (short)0;
				clone = (short)o;
			}
			else if (o is int)
			{
				clone = 0;
				clone = (int)o;
			}
			else if (o is long)
			{
				clone = 0L;
				clone = (long)o;
			}
			else if (o is ushort)
			{
				clone = (ushort)0;
				clone = (ushort)o;
			}
			else if (o is uint)
			{
				clone = 0u;
				clone = (uint)o;
			}
			else if (o is ulong)
			{
				clone = 0uL;
				clone = (ulong)o;
			}
			else if (o is byte)
			{
				clone = (byte)0;
				clone = (byte)o;
			}
			else if (o is sbyte)
			{
				clone = (sbyte)0;
				clone = (sbyte)o;
			}
			else if (o is TimeSpan)
			{
				clone = new TimeSpan(((TimeSpan)o).Ticks);
			}
			else if (o is DateTime)
			{
				clone = new DateTime(((DateTime)o).Ticks);
			}
			else if (o is float)
			{
				clone = 0f;
				clone = (float)o;
			}
			else if (o is double)
			{
				clone = 0.0;
				clone = (double)o;
			}
			else if (o is decimal)
			{
				clone = 0m;
				clone = (decimal)o;
			}
			return clone != null;
		}

		void IErrorContext.Register(ProcessingErrorCode code, Severity severity, params string[] arguments)
		{
			Global.Tracer.Assert(m_processingContext != null, "An unexpected error happened during deconstructing a custom report item");
			m_processingContext.ErrorContext.Register(code, severity, m_customObjectType, m_customObjectName, m_customPropertyName, arguments);
		}

		void IErrorContext.Register(ProcessingErrorCode code, Severity severity, ObjectType objectType, string objectName, string propertyName, params string[] arguments)
		{
			Global.Tracer.Assert(m_processingContext != null, "An unexpected error happened during deconstructing a custom report item");
			m_processingContext.ErrorContext.Register(code, severity, objectType, objectName, propertyName, arguments);
		}
	}
}
