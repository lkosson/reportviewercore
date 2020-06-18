namespace Microsoft.ReportingServices.ReportProcessing.Persistence
{
	internal sealed class DeclarationList
	{
		private Declaration[] m_declarations;

		internal static readonly DeclarationList Current = CreateCurrentDeclarations();

		internal int Count => m_declarations.Length;

		internal Declaration this[ObjectType objectType]
		{
			get
			{
				return this[(int)objectType];
			}
			set
			{
				this[(int)objectType] = value;
			}
		}

		private Declaration this[int index]
		{
			get
			{
				Global.Tracer.Assert(index >= 0 && index < m_declarations.Length);
				return m_declarations[index];
			}
			set
			{
				Global.Tracer.Assert(index >= 0 && index < m_declarations.Length);
				m_declarations[index] = value;
			}
		}

		internal DeclarationList()
		{
			m_declarations = new Declaration[227];
		}

		internal bool ContainsKey(ObjectType objectType)
		{
			return this[objectType] != null;
		}

		private static DeclarationList CreateCurrentDeclarations()
		{
			DeclarationList declarationList = new DeclarationList();
			int num = 1;
			declarationList[num++] = IDOwner.GetDeclaration();
			declarationList[num++] = ReportItem.GetDeclaration();
			num++;
			declarationList[num++] = Report.GetDeclaration();
			declarationList[num++] = PageSection.GetDeclaration();
			declarationList[num++] = Line.GetDeclaration();
			declarationList[num++] = Rectangle.GetDeclaration();
			declarationList[num++] = Image.GetDeclaration();
			num++;
			declarationList[num++] = CheckBox.GetDeclaration();
			declarationList[num++] = TextBox.GetDeclaration();
			declarationList[num++] = SubReport.GetDeclaration();
			declarationList[num++] = ActiveXControl.GetDeclaration();
			declarationList[num++] = DataRegion.GetDeclaration();
			num++;
			declarationList[num++] = ReportHierarchyNode.GetDeclaration();
			declarationList[num++] = Grouping.GetDeclaration();
			declarationList[num++] = Sorting.GetDeclaration();
			declarationList[num++] = List.GetDeclaration();
			declarationList[num++] = Pivot.GetDeclaration();
			declarationList[num++] = Matrix.GetDeclaration();
			declarationList[num++] = PivotHeading.GetDeclaration();
			declarationList[num++] = MatrixHeading.GetDeclaration();
			declarationList[num++] = MatrixColumn.GetDeclaration();
			num++;
			declarationList[num++] = MatrixRow.GetDeclaration();
			num++;
			declarationList[num++] = Subtotal.GetDeclaration();
			declarationList[num++] = Table.GetDeclaration();
			declarationList[num++] = TableColumn.GetDeclaration();
			num++;
			declarationList[num++] = TableGroup.GetDeclaration();
			declarationList[num++] = TableRow.GetDeclaration();
			num++;
			declarationList[num++] = OWCChart.GetDeclaration();
			declarationList[num++] = ChartColumn.GetDeclaration();
			num++;
			declarationList[num++] = ReportItemCollection.GetDeclaration();
			declarationList[num++] = ReportItemIndexer.GetDeclaration();
			num++;
			declarationList[num++] = Style.GetDeclaration();
			num++;
			declarationList[num++] = AttributeInfo.GetDeclaration();
			declarationList[num++] = Visibility.GetDeclaration();
			declarationList[num++] = ExpressionInfo.GetDeclaration();
			num++;
			declarationList[num++] = DataAggregateInfo.GetDeclaration();
			num++;
			declarationList[num++] = RunningValueInfo.GetDeclaration();
			num++;
			num++;
			declarationList[num++] = Filter.GetDeclaration();
			num++;
			declarationList[num++] = DataSource.GetDeclaration();
			num++;
			declarationList[num++] = DataSet.GetDeclaration();
			num++;
			declarationList[num++] = ReportQuery.GetDeclaration();
			declarationList[num++] = Field.GetDeclaration();
			num++;
			declarationList[num++] = ParameterValue.GetDeclaration();
			num++;
			num++;
			num++;
			num++;
			declarationList[num++] = ReportSnapshot.GetDeclaration();
			declarationList[num++] = SenderInformation.GetDeclaration();
			declarationList[num++] = InstanceInfo.GetDeclaration();
			declarationList[num++] = ReceiverInformation.GetDeclaration();
			declarationList[num++] = InstanceInfo.GetDeclaration();
			declarationList[num++] = DocumentMapNode.GetDeclaration();
			declarationList[num++] = InfoBase.GetDeclaration();
			declarationList[num++] = OffsetInfo.GetDeclaration();
			declarationList[num++] = InstanceInfo.GetDeclaration();
			declarationList[num++] = ReportItemInstanceInfo.GetDeclaration();
			declarationList[num++] = ReportInstanceInfo.GetDeclaration();
			declarationList[num++] = ReportItemColInstanceInfo.GetDeclaration();
			declarationList[num++] = LineInstanceInfo.GetDeclaration();
			declarationList[num++] = TextBoxInstanceInfo.GetDeclaration();
			declarationList[num++] = RectangleInstanceInfo.GetDeclaration();
			declarationList[num++] = CheckBoxInstanceInfo.GetDeclaration();
			declarationList[num++] = ImageInstanceInfo.GetDeclaration();
			declarationList[num++] = SubReportInstanceInfo.GetDeclaration();
			declarationList[num++] = ActiveXControlInstanceInfo.GetDeclaration();
			declarationList[num++] = ListInstanceInfo.GetDeclaration();
			declarationList[num++] = ListContentInstanceInfo.GetDeclaration();
			declarationList[num++] = MatrixInstanceInfo.GetDeclaration();
			declarationList[num++] = MatrixHeadingInstanceInfo.GetDeclaration();
			declarationList[num++] = MatrixCellInstanceInfo.GetDeclaration();
			declarationList[num++] = TableInstanceInfo.GetDeclaration();
			declarationList[num++] = TableGroupInstanceInfo.GetDeclaration();
			declarationList[num++] = TableRowInstanceInfo.GetDeclaration();
			declarationList[num++] = OWCChartInstanceInfo.GetDeclaration();
			declarationList[num++] = ChartInstanceInfo.GetDeclaration();
			declarationList[num++] = NonComputedUniqueNames.GetDeclaration();
			declarationList[num++] = InstanceInfoOwner.GetDeclaration();
			declarationList[num++] = ReportItemInstance.GetDeclaration();
			num++;
			declarationList[num++] = ReportInstance.GetDeclaration();
			declarationList[num++] = ReportItemColInstance.GetDeclaration();
			declarationList[num++] = LineInstance.GetDeclaration();
			declarationList[num++] = TextBoxInstance.GetDeclaration();
			declarationList[num++] = RectangleInstance.GetDeclaration();
			declarationList[num++] = CheckBoxInstance.GetDeclaration();
			declarationList[num++] = ImageInstance.GetDeclaration();
			declarationList[num++] = SubReportInstance.GetDeclaration();
			declarationList[num++] = ActiveXControlInstance.GetDeclaration();
			declarationList[num++] = ListInstance.GetDeclaration();
			declarationList[num++] = ListContentInstance.GetDeclaration();
			num++;
			declarationList[num++] = MatrixInstance.GetDeclaration();
			declarationList[num++] = MatrixHeadingInstance.GetDeclaration();
			num++;
			declarationList[num++] = MatrixCellInstance.GetDeclaration();
			num++;
			num++;
			declarationList[num++] = TableInstance.GetDeclaration();
			declarationList[num++] = TableRowInstance.GetDeclaration();
			declarationList[num++] = TableColumnInstance.GetDeclaration();
			declarationList[num++] = TableGroupInstance.GetDeclaration();
			num++;
			declarationList[num++] = OWCChartInstance.GetDeclaration();
			declarationList[num++] = ParameterInfo.GetDeclaration();
			num++;
			num++;
			num++;
			declarationList[num++] = InstanceInfo.GetDeclaration();
			num++;
			declarationList[num++] = RecordSetInfo.GetDeclaration();
			declarationList[num++] = RecordRow.GetDeclaration();
			declarationList[num++] = RecordField.GetDeclaration();
			declarationList[num++] = ValidValue.GetDeclaration();
			num++;
			declarationList[num++] = ParameterDataSource.GetDeclaration();
			declarationList[num++] = ParameterDef.GetDeclaration();
			num++;
			declarationList[num++] = ParameterBase.GetDeclaration();
			num++;
			declarationList[num++] = ProcessingMessage.GetDeclaration();
			declarationList[num++] = MatrixSubtotalHeadingInstanceInfo.GetDeclaration();
			declarationList[num++] = MatrixSubtotalCellInstance.GetDeclaration();
			declarationList[num++] = CodeClass.GetDeclaration();
			num++;
			declarationList[num++] = TableDetail.GetDeclaration();
			declarationList[num++] = TableDetailInstance.GetDeclaration();
			num++;
			declarationList[num++] = TableDetailInstanceInfo.GetDeclaration();
			num++;
			declarationList[num++] = Action.GetDeclaration();
			declarationList[num++] = ActionInstance.GetDeclaration();
			declarationList[num++] = Chart.GetDeclaration();
			declarationList[num++] = ChartHeading.GetDeclaration();
			declarationList[num++] = ChartDataPoint.GetDeclaration();
			num++;
			declarationList[num++] = MultiChart.GetDeclaration();
			declarationList[num++] = MultiChartInstance.GetDeclaration();
			num++;
			declarationList[num++] = Axis.GetDeclaration();
			declarationList[num++] = AxisInstance.GetDeclaration();
			declarationList[num++] = ChartTitle.GetDeclaration();
			declarationList[num++] = ChartTitleInstance.GetDeclaration();
			declarationList[num++] = ThreeDProperties.GetDeclaration();
			declarationList[num++] = PlotArea.GetDeclaration();
			declarationList[num++] = Legend.GetDeclaration();
			declarationList[num++] = GridLines.GetDeclaration();
			declarationList[num++] = ChartDataLabel.GetDeclaration();
			declarationList[num++] = ChartInstance.GetDeclaration();
			declarationList[num++] = ChartHeadingInstance.GetDeclaration();
			declarationList[num++] = ChartHeadingInstanceInfo.GetDeclaration();
			num++;
			declarationList[num++] = ChartDataPointInstance.GetDeclaration();
			declarationList[num++] = ChartDataPointInstanceInfo.GetDeclaration();
			num++;
			num++;
			declarationList[num++] = RenderingPagesRanges.GetDeclaration();
			num++;
			declarationList[num++] = IntermediateFormatVersion.GetDeclaration();
			declarationList[num++] = ImageInfo.GetDeclaration();
			declarationList[num++] = ActionItem.GetDeclaration();
			declarationList[num++] = ActionItemInstance.GetDeclaration();
			num++;
			num++;
			declarationList[num++] = DataValue.GetDeclaration();
			declarationList[num++] = DataValueInstance.GetDeclaration();
			num++;
			num++;
			declarationList[num++] = Tablix.GetDeclaration();
			declarationList[num++] = TablixHeading.GetDeclaration();
			declarationList[num++] = CustomReportItem.GetDeclaration();
			declarationList[num++] = CustomReportItemInstance.GetDeclaration();
			declarationList[num++] = CustomReportItemHeading.GetDeclaration();
			declarationList[num++] = CustomReportItemHeadingInstance.GetDeclaration();
			num++;
			num++;
			num++;
			num++;
			declarationList[num++] = CustomReportItemCellInstance.GetDeclaration();
			num++;
			num++;
			declarationList[num++] = DataValueCRIList.GetDeclaration();
			declarationList[num++] = BookmarkInformation.GetDeclaration();
			declarationList[num++] = InstanceInfo.GetDeclaration();
			declarationList[num++] = DrillthroughInformation.GetDeclaration();
			declarationList[num++] = InstanceInfo.GetDeclaration();
			num++;
			declarationList[num++] = CustomReportItemInstanceInfo.GetDeclaration();
			declarationList[num++] = ImageMapAreaInstanceList.GetDeclaration();
			declarationList[num++] = ImageMapAreaInstance.GetDeclaration();
			num++;
			declarationList[num++] = InstanceInfo.GetDeclaration();
			declarationList[num++] = SortFilterEventInfo.GetDeclaration();
			declarationList[num++] = EndUserSort.GetDeclaration();
			num++;
			num++;
			declarationList[num++] = RecordSetPropertyNames.GetDeclaration();
			num++;
			num++;
			num++;
			declarationList[num++] = PageSectionInstance.GetDeclaration();
			num++;
			declarationList[num++] = PageSectionInstanceInfo.GetDeclaration();
			declarationList[num++] = SimpleTextBoxInstanceInfo.GetDeclaration();
			declarationList[num++] = ScopeLookupTable.GetDeclaration();
			num++;
			declarationList[num++] = ReportDrillthroughInfo.GetDeclaration();
			declarationList[num++] = InstanceInfo.GetDeclaration();
			Global.Tracer.Assert(declarationList.Count == num, "(current.Count == index)");
			return declarationList;
		}
	}
}
