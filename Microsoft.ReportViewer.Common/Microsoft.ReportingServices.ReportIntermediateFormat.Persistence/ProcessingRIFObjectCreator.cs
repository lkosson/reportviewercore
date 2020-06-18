using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.ReportIntermediateFormat.Persistence
{
	internal class ProcessingRIFObjectCreator : IRIFObjectCreator
	{
		private IDOwner m_parentIDOwner;

		private ReportItem m_parentReportItem;

		internal ProcessingRIFObjectCreator(IDOwner parentIDOwner, ReportItem parentReportItem)
		{
			m_parentIDOwner = parentIDOwner;
			m_parentReportItem = parentReportItem;
		}

		public IPersistable CreateRIFObject(ObjectType objectType, ref IntermediateFormatReader context)
		{
			IPersistable persistable = null;
			if (objectType == ObjectType.Null)
			{
				return null;
			}
			IDOwner parentIDOwner = m_parentIDOwner;
			ReportItem parentReportItem = m_parentReportItem;
			switch (objectType)
			{
			case ObjectType.PageSection:
				persistable = new PageSection(m_parentReportItem);
				m_parentReportItem = (ReportItem)persistable;
				break;
			case ObjectType.Line:
				persistable = new Line(m_parentReportItem);
				m_parentReportItem = (ReportItem)persistable;
				break;
			case ObjectType.Rectangle:
				persistable = new Rectangle(m_parentReportItem);
				m_parentReportItem = (ReportItem)persistable;
				break;
			case ObjectType.Image:
				persistable = new Image(m_parentReportItem);
				m_parentReportItem = (ReportItem)persistable;
				break;
			case ObjectType.TextBox:
				persistable = new TextBox(m_parentReportItem);
				m_parentReportItem = (ReportItem)persistable;
				break;
			case ObjectType.SubReport:
				persistable = new SubReport(m_parentReportItem);
				m_parentReportItem = (ReportItem)persistable;
				break;
			case ObjectType.Grouping:
				persistable = new Grouping(ConstructionPhase.Deserializing);
				break;
			case ObjectType.Sorting:
				persistable = new Sorting(ConstructionPhase.Deserializing);
				break;
			case ObjectType.ReportItemCollection:
				persistable = new ReportItemCollection();
				break;
			case ObjectType.ReportItemIndexer:
				persistable = default(ReportItemIndexer);
				break;
			case ObjectType.Style:
				persistable = new Style(ConstructionPhase.Deserializing);
				break;
			case ObjectType.AttributeInfo:
				persistable = new AttributeInfo();
				break;
			case ObjectType.Visibility:
				persistable = new Visibility();
				break;
			case ObjectType.ExpressionInfo:
				persistable = new ExpressionInfo();
				break;
			case ObjectType.ExpressionInfoTypeValuePair:
				persistable = new ExpressionInfoTypeValuePair();
				break;
			case ObjectType.DataAggregateInfo:
				persistable = new DataAggregateInfo();
				break;
			case ObjectType.RunningValueInfo:
				persistable = new RunningValueInfo();
				break;
			case ObjectType.Filter:
				persistable = new Filter();
				break;
			case ObjectType.DataSource:
				persistable = new DataSource();
				break;
			case ObjectType.DataSet:
				persistable = new DataSet();
				break;
			case ObjectType.ReportQuery:
				persistable = new ReportQuery();
				break;
			case ObjectType.Field:
				persistable = new Field();
				break;
			case ObjectType.ParameterValue:
				persistable = new ParameterValue();
				break;
			case ObjectType.ReportSnapshot:
				persistable = new ReportSnapshot();
				break;
			case ObjectType.DocumentMapNode:
				persistable = new DocumentMapNode();
				break;
			case ObjectType.DocumentMapBeginContainer:
				persistable = DocumentMapBeginContainer.Instance;
				break;
			case ObjectType.DocumentMapEndContainer:
				persistable = DocumentMapEndContainer.Instance;
				break;
			case ObjectType.ReportInstance:
				persistable = new ReportInstance();
				break;
			case ObjectType.ParameterInfo:
				persistable = new ParameterInfo();
				break;
			case ObjectType.ValidValue:
				persistable = new ValidValue();
				break;
			case ObjectType.ParameterDataSource:
				persistable = new ParameterDataSource();
				break;
			case ObjectType.ParameterDef:
				persistable = new ParameterDef();
				break;
			case ObjectType.ProcessingMessage:
				persistable = new ProcessingMessage();
				break;
			case ObjectType.CodeClass:
				persistable = default(CodeClass);
				break;
			case ObjectType.Action:
				persistable = new Action();
				break;
			case ObjectType.RenderingPagesRanges:
				persistable = default(RenderingPagesRanges);
				break;
			case ObjectType.IntermediateFormatVersion:
				persistable = new IntermediateFormatVersion();
				break;
			case ObjectType.ImageInfo:
				persistable = new ImageInfo();
				break;
			case ObjectType.ActionItem:
				persistable = new ActionItem();
				break;
			case ObjectType.DataValue:
				persistable = new DataValue();
				break;
			case ObjectType.CustomReportItem:
				persistable = new CustomReportItem(m_parentReportItem);
				m_parentReportItem = (ReportItem)persistable;
				break;
			case ObjectType.SortFilterEventInfoMap:
				persistable = new SortFilterEventInfoMap();
				break;
			case ObjectType.SortFilterEventInfo:
				persistable = new SortFilterEventInfo();
				break;
			case ObjectType.EndUserSort:
				persistable = new EndUserSort();
				break;
			case ObjectType.ScopeLookupTable:
				persistable = new ScopeLookupTable();
				break;
			case ObjectType.Tablix:
				persistable = new Tablix(m_parentReportItem);
				m_parentReportItem = (ReportItem)persistable;
				break;
			case ObjectType.TablixHeader:
				persistable = new TablixHeader();
				break;
			case ObjectType.TablixMember:
				persistable = new TablixMember();
				break;
			case ObjectType.TablixColumn:
				persistable = new TablixColumn();
				break;
			case ObjectType.TablixRow:
				persistable = new TablixRow();
				break;
			case ObjectType.TablixCornerCell:
				persistable = new TablixCornerCell();
				break;
			case ObjectType.TablixCell:
				persistable = new TablixCell();
				break;
			case ObjectType.Chart:
				persistable = new Chart(m_parentReportItem);
				m_parentReportItem = (ReportItem)persistable;
				break;
			case ObjectType.ChartMember:
				persistable = new ChartMember();
				break;
			case ObjectType.ChartSeries:
				persistable = new ChartSeries();
				break;
			case ObjectType.ChartDataPoint:
				persistable = new ChartDataPoint();
				break;
			case ObjectType.ChartDataPointValues:
				persistable = new ChartDataPointValues();
				break;
			case ObjectType.ChartArea:
				persistable = new ChartArea();
				break;
			case ObjectType.ChartLegend:
				persistable = new ChartLegend();
				break;
			case ObjectType.ChartLegendTitle:
				persistable = new ChartLegendTitle();
				break;
			case ObjectType.ChartAxis:
				persistable = new ChartAxis();
				break;
			case ObjectType.ThreeDProperties:
				persistable = new ChartThreeDProperties();
				break;
			case ObjectType.ChartDataLabel:
				persistable = new ChartDataLabel();
				break;
			case ObjectType.ChartMarker:
				persistable = new ChartMarker();
				break;
			case ObjectType.ChartTitle:
				persistable = new ChartTitle();
				break;
			case ObjectType.ChartAxisScaleBreak:
				persistable = new ChartAxisScaleBreak();
				break;
			case ObjectType.ChartDerivedSeries:
				persistable = new ChartDerivedSeries();
				break;
			case ObjectType.ChartBorderSkin:
				persistable = new ChartBorderSkin();
				break;
			case ObjectType.ChartNoDataMessage:
				persistable = new ChartNoDataMessage();
				break;
			case ObjectType.ChartItemInLegend:
				persistable = new ChartItemInLegend();
				break;
			case ObjectType.ChartEmptyPoints:
				persistable = new ChartEmptyPoints();
				break;
			case ObjectType.ChartNoMoveDirections:
				persistable = new ChartNoMoveDirections();
				break;
			case ObjectType.ChartFormulaParameter:
				persistable = new ChartFormulaParameter();
				break;
			case ObjectType.ChartLegendColumn:
				persistable = new ChartLegendColumn();
				break;
			case ObjectType.ChartLegendColumnHeader:
				persistable = new ChartLegendColumnHeader();
				break;
			case ObjectType.ChartLegendCustomItem:
				persistable = new ChartLegendCustomItem();
				break;
			case ObjectType.ChartLegendCustomItemCell:
				persistable = new ChartLegendCustomItemCell();
				break;
			case ObjectType.ChartAlignType:
				persistable = new ChartAlignType();
				break;
			case ObjectType.ChartElementPosition:
				persistable = new ChartElementPosition();
				break;
			case ObjectType.ChartSmartLabel:
				persistable = new ChartSmartLabel();
				break;
			case ObjectType.ChartStripLine:
				persistable = new ChartStripLine();
				break;
			case ObjectType.ChartAxisTitle:
				persistable = new ChartAxisTitle();
				break;
			case ObjectType.ChartCustomPaletteColor:
				persistable = new ChartCustomPaletteColor();
				break;
			case ObjectType.GridLines:
				persistable = new ChartGridLines();
				break;
			case ObjectType.ChartTickMarks:
				persistable = new ChartTickMarks();
				break;
			case ObjectType.DataMember:
				persistable = new DataMember();
				break;
			case ObjectType.CustomDataRow:
				persistable = new CustomDataRow();
				break;
			case ObjectType.DataCell:
				persistable = new DataCell();
				break;
			case ObjectType.Variable:
				persistable = new Variable();
				break;
			case ObjectType.Page:
				persistable = new Page();
				break;
			case ObjectType.Paragraph:
				persistable = new Paragraph();
				break;
			case ObjectType.TextRun:
				persistable = new TextRun();
				break;
			case ObjectType.Report:
				persistable = new Report(m_parentReportItem);
				m_parentReportItem = (ReportItem)persistable;
				break;
			case ObjectType.GaugePanel:
				persistable = new GaugePanel(m_parentReportItem);
				m_parentReportItem = (ReportItem)persistable;
				break;
			case ObjectType.GaugeMember:
				persistable = new GaugeMember();
				break;
			case ObjectType.GaugeRow:
				persistable = new GaugeRow();
				break;
			case ObjectType.GaugeCell:
				persistable = new GaugeCell();
				break;
			case ObjectType.BackFrame:
				persistable = new BackFrame();
				break;
			case ObjectType.CapImage:
				persistable = new CapImage();
				break;
			case ObjectType.FrameBackground:
				persistable = new FrameBackground();
				break;
			case ObjectType.FrameImage:
				persistable = new FrameImage();
				break;
			case ObjectType.CustomLabel:
				persistable = new CustomLabel();
				break;
			case ObjectType.GaugeImage:
				persistable = new GaugeImage();
				break;
			case ObjectType.GaugeInputValue:
				persistable = new GaugeInputValue();
				break;
			case ObjectType.GaugeLabel:
				persistable = new GaugeLabel();
				break;
			case ObjectType.GaugePanelItem:
				persistable = new GaugePanelItem();
				break;
			case ObjectType.GaugeTickMarks:
				persistable = new GaugeTickMarks();
				break;
			case ObjectType.LinearGauge:
				persistable = new LinearGauge();
				break;
			case ObjectType.LinearPointer:
				persistable = new LinearPointer();
				break;
			case ObjectType.LinearScale:
				persistable = new LinearScale();
				break;
			case ObjectType.NumericIndicator:
				persistable = new NumericIndicator();
				break;
			case ObjectType.PinLabel:
				persistable = new PinLabel();
				break;
			case ObjectType.PointerCap:
				persistable = new PointerCap();
				break;
			case ObjectType.PointerImage:
				persistable = new PointerImage();
				break;
			case ObjectType.RadialGauge:
				persistable = new RadialGauge();
				break;
			case ObjectType.RadialPointer:
				persistable = new RadialPointer();
				break;
			case ObjectType.RadialScale:
				persistable = new RadialScale();
				break;
			case ObjectType.ScaleLabels:
				persistable = new ScaleLabels();
				break;
			case ObjectType.ScalePin:
				persistable = new ScalePin();
				break;
			case ObjectType.ScaleRange:
				persistable = new ScaleRange();
				break;
			case ObjectType.IndicatorImage:
				persistable = new IndicatorImage();
				break;
			case ObjectType.StateIndicator:
				persistable = new StateIndicator();
				break;
			case ObjectType.Thermometer:
				persistable = new Thermometer();
				break;
			case ObjectType.TickMarkStyle:
				persistable = new TickMarkStyle();
				break;
			case ObjectType.TopImage:
				persistable = new TopImage();
				break;
			case ObjectType.LookupInfo:
				persistable = new LookupInfo();
				break;
			case ObjectType.LookupDestinationInfo:
				persistable = new LookupDestinationInfo();
				break;
			case ObjectType.ReportSection:
				persistable = new ReportSection();
				break;
			case ObjectType.MapFieldDefinition:
				persistable = new MapFieldDefinition();
				break;
			case ObjectType.MapFieldName:
				persistable = new MapFieldName();
				break;
			case ObjectType.MapLineLayer:
				persistable = new MapLineLayer();
				break;
			case ObjectType.MapShapefile:
				persistable = new MapShapefile();
				break;
			case ObjectType.MapPolygonLayer:
				persistable = new MapPolygonLayer();
				break;
			case ObjectType.MapSpatialDataRegion:
				persistable = new MapSpatialDataRegion();
				break;
			case ObjectType.MapSpatialDataSet:
				persistable = new MapSpatialDataSet();
				break;
			case ObjectType.MapPointLayer:
				persistable = new MapPointLayer();
				break;
			case ObjectType.MapTile:
				persistable = new MapTile();
				break;
			case ObjectType.MapTileLayer:
				persistable = new MapTileLayer();
				break;
			case ObjectType.MapField:
				persistable = new MapField();
				break;
			case ObjectType.MapLine:
				persistable = new MapLine();
				break;
			case ObjectType.MapPolygon:
				persistable = new MapPolygon();
				break;
			case ObjectType.MapPoint:
				persistable = new MapPoint();
				break;
			case ObjectType.MapLineTemplate:
				persistable = new MapLineTemplate();
				break;
			case ObjectType.MapPolygonTemplate:
				persistable = new MapPolygonTemplate();
				break;
			case ObjectType.MapMarkerTemplate:
				persistable = new MapMarkerTemplate();
				break;
			case ObjectType.Map:
				persistable = new Map(m_parentReportItem);
				m_parentReportItem = (ReportItem)persistable;
				break;
			case ObjectType.MapBorderSkin:
				persistable = new MapBorderSkin();
				break;
			case ObjectType.MapDataRegion:
				persistable = new MapDataRegion(m_parentReportItem);
				break;
			case ObjectType.MapMember:
				persistable = new MapMember();
				break;
			case ObjectType.MapRow:
				persistable = new MapRow();
				break;
			case ObjectType.MapCell:
				persistable = new MapCell();
				break;
			case ObjectType.MapLocation:
				persistable = new MapLocation();
				break;
			case ObjectType.MapSize:
				persistable = new MapSize();
				break;
			case ObjectType.MapGridLines:
				persistable = new MapGridLines();
				break;
			case ObjectType.MapBindingFieldPair:
				persistable = new MapBindingFieldPair();
				break;
			case ObjectType.MapCustomView:
				persistable = new MapCustomView();
				break;
			case ObjectType.MapDataBoundView:
				persistable = new MapDataBoundView();
				break;
			case ObjectType.MapElementView:
				persistable = new MapElementView();
				break;
			case ObjectType.MapViewport:
				persistable = new MapViewport();
				break;
			case ObjectType.MapLimits:
				persistable = new MapLimits();
				break;
			case ObjectType.MapColorScale:
				persistable = new MapColorScale();
				break;
			case ObjectType.MapColorScaleTitle:
				persistable = new MapColorScaleTitle();
				break;
			case ObjectType.MapDistanceScale:
				persistable = new MapDistanceScale();
				break;
			case ObjectType.MapTitle:
				persistable = new MapTitle();
				break;
			case ObjectType.MapLegend:
				persistable = new MapLegend();
				break;
			case ObjectType.MapLegendTitle:
				persistable = new MapLegendTitle();
				break;
			case ObjectType.MapBucket:
				persistable = new MapBucket();
				break;
			case ObjectType.MapColorPaletteRule:
				persistable = new MapColorPaletteRule();
				break;
			case ObjectType.MapColorRangeRule:
				persistable = new MapColorRangeRule();
				break;
			case ObjectType.MapCustomColorRule:
				persistable = new MapCustomColorRule();
				break;
			case ObjectType.MapCustomColor:
				persistable = new MapCustomColor();
				break;
			case ObjectType.MapLineRules:
				persistable = new MapLineRules();
				break;
			case ObjectType.MapPolygonRules:
				persistable = new MapPolygonRules();
				break;
			case ObjectType.MapSizeRule:
				persistable = new MapSizeRule();
				break;
			case ObjectType.MapMarkerImage:
				persistable = new MapMarkerImage();
				break;
			case ObjectType.MapMarker:
				persistable = new MapMarker();
				break;
			case ObjectType.MapMarkerRule:
				persistable = new MapMarkerRule();
				break;
			case ObjectType.MapPointRules:
				persistable = new MapPointRules();
				break;
			case ObjectType.PageBreak:
				persistable = new PageBreak();
				break;
			case ObjectType.DataScopeInfo:
				persistable = new DataScopeInfo();
				break;
			case ObjectType.LinearJoinInfo:
				persistable = new LinearJoinInfo();
				break;
			case ObjectType.IntersectJoinInfo:
				persistable = new IntersectJoinInfo();
				break;
			case ObjectType.BucketedDataAggregateInfos:
				persistable = new BucketedDataAggregateInfos();
				break;
			case ObjectType.DataAggregateInfoBucket:
				persistable = new DataAggregateInfoBucket();
				break;
			case ObjectType.NumericIndicatorRange:
				persistable = new NumericIndicatorRange();
				break;
			case ObjectType.IndicatorState:
				persistable = new IndicatorState();
				break;
			case ObjectType.SharedDataSetQuery:
				persistable = new SharedDataSetQuery();
				break;
			case ObjectType.DataSetCore:
				persistable = new DataSetCore();
				break;
			case ObjectType.DataSetParameterValue:
				persistable = new DataSetParameterValue();
				break;
			case ObjectType.RIFVariantContainer:
				persistable = new RIFVariantContainer();
				break;
			case ObjectType.IdcRelationship:
				persistable = new IdcRelationship();
				break;
			case ObjectType.DefaultRelationship:
				persistable = new DefaultRelationship();
				break;
			case ObjectType.JoinCondition:
				persistable = new Relationship.JoinCondition();
				break;
			case ObjectType.BandLayoutOptions:
				persistable = new BandLayoutOptions();
				break;
			case ObjectType.LabelData:
				persistable = new LabelData();
				break;
			case ObjectType.Slider:
				persistable = new Slider();
				break;
			case ObjectType.Coverflow:
				persistable = new Coverflow();
				break;
			case ObjectType.PlayAxis:
				persistable = new PlayAxis();
				break;
			case ObjectType.BandNavigationCell:
				persistable = new BandNavigationCell();
				break;
			case ObjectType.Tabstrip:
				persistable = new Tabstrip();
				break;
			case ObjectType.NavigationItem:
				persistable = new NavigationItem();
				break;
			case ObjectType.ScopedFieldInfo:
				persistable = new ScopedFieldInfo();
				break;
			default:
				Global.Tracer.Assert(condition: false, "Unsupported object type: " + objectType);
				break;
			}
			IDOwner iDOwner = persistable as IDOwner;
			if (iDOwner != null)
			{
				iDOwner.ParentInstancePath = m_parentIDOwner;
				m_parentIDOwner = iDOwner;
			}
			persistable.Deserialize(context);
			m_parentIDOwner = parentIDOwner;
			m_parentReportItem = parentReportItem;
			return persistable;
		}
	}
}
