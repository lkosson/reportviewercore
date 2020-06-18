using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.RdlObjectModel;
using Microsoft.ReportingServices.RdlObjectModel.Serialization;
using Microsoft.ReportingServices.RdlObjectModel2008.Upgrade;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.ReportingServices.RdlObjectModel2005.Upgrade
{
	internal class UpgradeImpl2005 : UpgradeImpl2008
	{
		private class Cloner
		{
			private UpgradeImpl2005 m_upgrader;

			private Dictionary<string, string> m_nameTable;

			private ArrayList m_clonedObjects;

			private Dictionary<string, string> m_textboxNameValueExprTable;

			public Dictionary<string, string> TextboxNameValueExprTable => m_textboxNameValueExprTable;

			public Cloner(UpgradeImpl2005 upgrader)
			{
				m_upgrader = upgrader;
				m_nameTable = new Dictionary<string, string>();
				m_clonedObjects = new ArrayList();
				m_textboxNameValueExprTable = new Dictionary<string, string>();
			}

			public object Clone(object obj)
			{
				if (obj is ReportObject)
				{
					StructMapping mapping = (StructMapping)TypeMapper.GetTypeMapping(SerializerHost2005.GetSubstituteType(obj.GetType(), serializing: true));
					object obj2 = CloneStructure(obj, mapping);
					m_clonedObjects.Add(obj2);
					return obj2;
				}
				if (obj is IList)
				{
					object obj2 = CloneList((IList)obj);
					m_clonedObjects.Add(obj2);
					return obj2;
				}
				return obj;
			}

			private object CloneStructure(object obj, StructMapping mapping)
			{
				Type type = mapping.Type;
				object obj2 = Activator.CreateInstance(type);
				foreach (MemberMapping member in mapping.Members)
				{
					object value = member.GetValue(obj);
					member.SetValue(obj2, Clone(value));
				}
				if (obj2 is IGlobalNamedObject)
				{
					string baseName;
					if (obj2 is Microsoft.ReportingServices.RdlObjectModel.Group)
					{
						baseName = m_upgrader.GetParentReportItemName((IContainedObject)obj) + "_Group";
					}
					else
					{
						baseName = type.Name;
						baseName = char.ToLower(baseName[0], CultureInfo.InvariantCulture) + baseName.Substring(1);
					}
					string text = m_upgrader.UniqueName(baseName, obj2, allowBaseName: false);
					m_nameTable.Add(((IGlobalNamedObject)obj2).Name, text);
					((IGlobalNamedObject)obj2).Name = text;
				}
				return obj2;
			}

			private object CloneList(IList obj)
			{
				IList list = (IList)Activator.CreateInstance(obj.GetType());
				foreach (object item in obj)
				{
					list.Add(Clone(item));
				}
				return list;
			}

			public void FixReferences()
			{
				foreach (object clonedObject in m_clonedObjects)
				{
					FixReferences(clonedObject);
				}
			}

			public void FixReferences(object obj)
			{
				if (obj is IList)
				{
					foreach (object item in (IList)obj)
					{
						FixReferences(item);
					}
				}
				else
				{
					if (!(obj is ReportObject))
					{
						return;
					}
					Type type = obj.GetType();
					StructMapping obj2 = (StructMapping)TypeMapper.GetTypeMapping(type);
					if (typeof(Microsoft.ReportingServices.RdlObjectModel.ReportItem).IsAssignableFrom(type))
					{
						Microsoft.ReportingServices.RdlObjectModel.ReportItem reportItem = (Microsoft.ReportingServices.RdlObjectModel.ReportItem)obj;
						reportItem.RepeatWith = FixReference(reportItem.RepeatWith);
						if (type == typeof(Microsoft.ReportingServices.RdlObjectModel.Rectangle))
						{
							((Microsoft.ReportingServices.RdlObjectModel.Rectangle)reportItem).LinkToChild = FixReference(((Microsoft.ReportingServices.RdlObjectModel.Rectangle)reportItem).LinkToChild);
						}
						else if (type == typeof(Textbox))
						{
							((Textbox)obj).HideDuplicates = FixReference(((Textbox)obj).HideDuplicates);
						}
					}
					else if (type == typeof(Microsoft.ReportingServices.RdlObjectModel.Visibility))
					{
						((Microsoft.ReportingServices.RdlObjectModel.Visibility)obj).ToggleItem = FixReference(((Microsoft.ReportingServices.RdlObjectModel.Visibility)obj).ToggleItem);
					}
					else if (type == typeof(UserSort))
					{
						((UserSort)obj).SortExpressionScope = FixReference(((UserSort)obj).SortExpressionScope);
					}
					foreach (MemberMapping member in obj2.Members)
					{
						object value = member.GetValue(obj);
						if (typeof(IExpression).IsAssignableFrom(member.Type))
						{
							member.SetValue(obj, FixReference((IExpression)value));
						}
						else
						{
							FixReferences(value);
						}
					}
				}
			}

			private string FixReference(string value)
			{
				if (value != null && m_nameTable.ContainsKey(value))
				{
					return m_nameTable[value];
				}
				return value;
			}

			private IExpression FixReference(IExpression value)
			{
				if (value != null && value.IsExpression)
				{
					string expression = value.Expression;
					foreach (KeyValuePair<string, string> item in m_nameTable)
					{
						expression = m_upgrader.ReplaceReference(expression, item.Key, item.Value);
					}
					expression = m_upgrader.ReplaceReportItemReferenceWithValue(expression, m_textboxNameValueExprTable);
					value = (IExpression)Activator.CreateInstance(value.GetType());
					value.Expression = expression;
				}
				return value;
			}

			public void AddNameMapping(string oldName, string newName)
			{
				m_nameTable.Add(oldName, newName);
			}

			public void ApplySubTotalStyleOverrides(Microsoft.ReportingServices.RdlObjectModel.ReportItem item, SubtotalStyle2005 style)
			{
				if (item == null || style == null)
				{
					return;
				}
				if (item is Textbox)
				{
					if (item.Style == null)
					{
						item.Style = new Microsoft.ReportingServices.RdlObjectModel.Style();
					}
					Textbox textbox = (Textbox)item;
					Microsoft.ReportingServices.RdlObjectModel.Style style2 = textbox.Style;
					Paragraph paragraph = null;
					Microsoft.ReportingServices.RdlObjectModel.Style style3 = null;
					Microsoft.ReportingServices.RdlObjectModel.Style style4 = null;
					if (textbox.Paragraphs.Count > 0)
					{
						paragraph = textbox.Paragraphs[0];
						if (paragraph.Style == null)
						{
							Microsoft.ReportingServices.RdlObjectModel.Style style6 = paragraph.Style = new Microsoft.ReportingServices.RdlObjectModel.Style();
							style3 = style6;
						}
						else
						{
							style3 = paragraph.Style;
						}
						if (paragraph.TextRuns.Count > 0)
						{
							TextRun textRun = paragraph.TextRuns[0];
							if (textRun.Style == null)
							{
								Microsoft.ReportingServices.RdlObjectModel.Style style6 = textRun.Style = new Microsoft.ReportingServices.RdlObjectModel.Style();
								style4 = style6;
							}
							else
							{
								style4 = textRun.Style;
							}
						}
					}
					ApplySubTotalStyleOverrides(style, style2, style3, style4);
				}
				else if (item.Style != null)
				{
					ApplySubTotalStyleOverrides(style, item.Style, item.Style, item.Style);
				}
			}

			private void ApplySubTotalStyleOverrides(SubtotalStyle2005 subTotalStyle, Microsoft.ReportingServices.RdlObjectModel.Style style1, Microsoft.ReportingServices.RdlObjectModel.Style style2, Microsoft.ReportingServices.RdlObjectModel.Style style3)
			{
				foreach (MemberMapping member in ((StructMapping)TypeMapper.GetTypeMapping(typeof(Microsoft.ReportingServices.RdlObjectModel.Style))).Members)
				{
					if (!member.HasValue(subTotalStyle) || subTotalStyle.IsPropertyDefinedOnInitialize(member.Name))
					{
						continue;
					}
					switch (member.Name)
					{
					case "TextAlign":
					case "LineHeight":
						if (style2 != null)
						{
							member.SetValue(style2, member.GetValue(subTotalStyle));
						}
						break;
					case "FontStyle":
					case "FontFamily":
					case "FontSize":
					case "FontWeight":
					case "Format":
					case "TextDecoration":
					case "Color":
					case "Language":
					case "Calendar":
					case "NumeralLanguage":
					case "NumeralVariant":
						if (style3 != null)
						{
							member.SetValue(style3, member.GetValue(subTotalStyle));
						}
						break;
					default:
						member.SetValue(style1, member.GetValue(subTotalStyle));
						break;
					}
				}
			}
		}

		internal delegate Microsoft.ReportingServices.RdlObjectModel.Group GroupAccessor(object member);

		internal delegate IList<CustomProperty> CustomPropertiesAccessor(object member);

		private delegate string AggregateFunctionFixup(string expression, int currentOffset, string specialFunctionName, int specialFunctionPos, int argumentsPos, int scopePos, int scopeLength, ref int offset);

		private Hashtable m_nameTable;

		private Hashtable m_dataSourceNameTable;

		private Hashtable m_dataSourceCaseSensitiveNameTable;

		private List<DataSource2005> m_dataSources;

		private List<IUpgradeable> m_upgradeable;

		private ReportRegularExpressions m_regexes;

		private bool m_throwUpgradeException = true;

		private bool m_upgradeDundasCRIToNative;

		private bool m_renameInvalidDataSources = true;

		private static string[] m_scatterChartDataPointNames = new string[2]
		{
			"X",
			"Y"
		};

		private static string[] m_bubbleChartDataPointNames = new string[3]
		{
			"X",
			"Y",
			"Size"
		};

		private static string[] m_highLowCloseDataPointNames = new string[3]
		{
			"High",
			"Low",
			"Close"
		};

		private static string[] m_openHighLowCloseDataPointNames = new string[4]
		{
			"High",
			"Low",
			"Open",
			"Close"
		};

		private const string DundasChartControl = "DundasChartControl";

		private const string DundasGaugeControl = "DundasGaugeControl";

		private const string UpgradedYukonChart = "__Upgraded2005__";

		private const double YukonDefaultPointWidth = 0.8;

		private const double YukonDefaultBarAndColumnPointWidth = 0.6;

		private const double YukonDefaultLineWidthInPoints = 2.25;

		private const double YukonDefaultBorderWidthInPoints = 0.75;

		private const double YukonBorderWidthFactor = 0.75;

		private const double KatmaiMinimumVisibleBorderWidth = 0.376;

		private const double KatmaiMinimumBorderWidth = 0.25;

		private static Microsoft.ReportingServices.RdlObjectModel.Style.Definition.Properties[] m_ParagraphAvailableStyles = new Microsoft.ReportingServices.RdlObjectModel.Style.Definition.Properties[2]
		{
			Microsoft.ReportingServices.RdlObjectModel.Style.Definition.Properties.TextAlign,
			Microsoft.ReportingServices.RdlObjectModel.Style.Definition.Properties.LineHeight
		};

		private static Microsoft.ReportingServices.RdlObjectModel.Style.Definition.Properties[] m_TextRunAvailableStyles = new Microsoft.ReportingServices.RdlObjectModel.Style.Definition.Properties[11]
		{
			Microsoft.ReportingServices.RdlObjectModel.Style.Definition.Properties.FontStyle,
			Microsoft.ReportingServices.RdlObjectModel.Style.Definition.Properties.FontFamily,
			Microsoft.ReportingServices.RdlObjectModel.Style.Definition.Properties.FontSize,
			Microsoft.ReportingServices.RdlObjectModel.Style.Definition.Properties.FontWeight,
			Microsoft.ReportingServices.RdlObjectModel.Style.Definition.Properties.Format,
			Microsoft.ReportingServices.RdlObjectModel.Style.Definition.Properties.TextDecoration,
			Microsoft.ReportingServices.RdlObjectModel.Style.Definition.Properties.Color,
			Microsoft.ReportingServices.RdlObjectModel.Style.Definition.Properties.Language,
			Microsoft.ReportingServices.RdlObjectModel.Style.Definition.Properties.Calendar,
			Microsoft.ReportingServices.RdlObjectModel.Style.Definition.Properties.NumeralLanguage,
			Microsoft.ReportingServices.RdlObjectModel.Style.Definition.Properties.NumeralVariant
		};

		private const double PointsPerPixel = 0.75;

		private const string DundasCRIExpressionPrefixLowerCase = "expression:";

		private const string DundasCRIDefaultFont = "Microsoft Sans Serif, 8pt";

		private const string DundasCRIDefaultBoldFont = "Microsoft Sans Serif, 8pt, style=Bold";

		private const string DundasCRIDefaultCollectedPieStyle = "CollectedPie";

		private const string DundasCRISizeExpressionWrapper = "=CStr(({0})*{1})&\"pt\"";

		private const string EmptySeriesName = "emptySeriesName";

		private const string EmptyNamePrefix = "chart";

		private const string ChartElementDefaultName = "Default";

		private const string ChartPrimaryAxisName = "Primary";

		private const string ChartSecondaryAxisName = "Secondary";

		private const string NewChartAreaName = "ChartArea";

		private const string NewChartSeriesName = "Series";

		private const string NewChartTitleName = "Title";

		private const string ChartNoDataMessageTitleName = "NoDataMessageTitle";

		private const string NewChartLegendName = "Legend";

		private const string ChartFormulaNamePostfix = "_Formula";

		private const string NewChartAreaNameForFormulaSeries = "#NewChartArea";

		private const string PointWidthAttributeName = "PointWidth";

		private const string DrawingStyleAttributeName = "DrawingStyle";

		private const string PieLabelStyleAttributeName = "PieLabelStyle";

		private const string PieLabelStyleAttributeDefaultValueForYukon = "Outside";

		private const double DefaultSmartLabelMaxMovingDistance = 30.0;

		private const double DefaultBorderLineWidthInPixels = 1.0;

		private const string GaugeElementDefaultName = "Default";

		private const string DefaultRadialGaugeCollectionPrefix = "RadialGauges.";

		private const string DefaultLinearGaugeCollectionPrefix = "LinearGauges.";

		private const string DefaultGaugeLabelCollectionPrefix = "GaugeLabels.";

		private const string GaugeFontUnitPercentValue = "Percent";

		private const string GaugeFontUnitDefaultValue = "Default";

		private const string DefaultDundasCircularGaugeCollectionPrefix = "CircularGauges.";

		private const string DefaultDundasLinearGaugeCollectionPrefix = "LinearGauges.";

		private const string DefaultDundasGaugeLabelCollectionPrefix = "GaugeLabels.";

		private const string DefaultGaugeScaleLabelFont = "Microsoft Sans Serif, 14pt";

		private const string DefaultGaugeScalePinFont = "Microsoft Sans Serif, 12pt";

		private const string DefaultGaugeLabelFont = "Microsoft Sans Serif, 8.25pt";

		private const string DefaultGaugeScaleName = "Default";

		private const string DefaultGaugeTextPropertyValue = "Text";

		private const double DefaultLinearScaleRangeStartWidth = 10.0;

		private const double DefaultRadialScaleRangeStartWidth = 15.0;

		private const double DefaultLinearScaleRangeEndWidth = 10.0;

		private const double DefaultRadialScaleRangeEndWidth = 30.0;

		private const double DefaultLinearScaleRangeDistanceFromScale = 10.0;

		private const double DefaultRadialScaleRangeDistanceFromScale = 30.0;

		private const double DefaultGaugeTickMarkWidth = 3.0;

		private const double DefaultLinearScaleMajorTickMarkWidth = 4.0;

		private const double DefaultLinearScaleMinorTickMarkWidth = 3.0;

		private const double DefaultRadialScaleMajorTickMarkWidth = 8.0;

		private const double DefaultRadialScaleMinorTickMarkWidth = 3.0;

		private const double DefaultLinearScaleMajorTickMarkLength = 15.0;

		private const double DefaultLinearScaleMinorTickMarkLength = 9.0;

		private const double DefaultRadialScaleMajorTickMarkLength = 14.0;

		private const double DefaultRadialScaleMinorTickMarkLength = 8.0;

		private const double DefaultLinearGaugePointerWidth = 20.0;

		private const double DefaultRadialGaugePointerWidth = 15.0;

		private const double DefaultLinearGaugePointerMarkerLength = 20.0;

		private const double DefaultRadialGaugePointerMarkerLength = 10.0;

		private const double DefaultDefaultScalePinWidth = 6.0;

		private const double DefaultDefaultScalePinLength = 6.0;

		private const int DefaultGaugeScaleShadowOffset = 1;

		private const int DefaultGaugePointerShadowOffset = 2;

		internal void UpgradeMatrix(Matrix2005 matrix)
		{
			UpgradeReportItem(matrix);
			UpgradePageBreak(matrix);
			TablixBody tablixBody = matrix.TablixBody;
			matrix.RepeatRowHeaders = true;
			matrix.RepeatColumnHeaders = true;
			int count = matrix.ColumnGroupings.Count;
			int count2 = matrix.RowGroupings.Count;
			int i;
			if (matrix.Corner != null)
			{
				TablixCorner tablixCorner2 = matrix.TablixCorner = new TablixCorner();
				for (i = 0; i < count; i++)
				{
					TablixCornerRow tablixCornerRow = new TablixCornerRow();
					tablixCorner2.TablixCornerRows.Add(tablixCornerRow);
					for (int j = 0; j < count2; j++)
					{
						tablixCornerRow.Add(new TablixCornerCell());
					}
				}
				TablixCornerCell tablixCornerCell = tablixCorner2.TablixCornerRows[0][0];
				tablixCornerCell.CellContents = new CellContents();
				tablixCornerCell.CellContents.RowSpan = count;
				tablixCornerCell.CellContents.ColSpan = count2;
				if (matrix.Corner.ReportItems.Count > 0)
				{
					tablixCornerCell.CellContents.ReportItem = matrix.Corner.ReportItems[0];
				}
			}
			IList<TablixMember> list = null;
			TablixMember tablixMember = null;
			TablixMember tablixMember2 = null;
			TablixMember keepTogether = null;
			ColumnGrouping2005 columnGrouping = null;
			RowGrouping2005 rowGrouping = null;
			int num = 1;
			int num2 = 1;
			foreach (ColumnGrouping2005 columnGrouping3 in matrix.ColumnGroupings)
			{
				if (list == null)
				{
					matrix.TablixColumnHierarchy = new TablixHierarchy();
					list = matrix.TablixColumnHierarchy.TablixMembers;
				}
				if (columnGrouping3.FixedHeader)
				{
					matrix.FixedColumnHeaders = true;
				}
				DynamicColumns2005 dynamicColumns = columnGrouping3.DynamicColumns;
				if (dynamicColumns != null)
				{
					TablixMember tablixMember3 = new TablixMember();
					list.Add(tablixMember3);
					list = tablixMember3.TablixMembers;
					tablixMember = tablixMember3;
					keepTogether = tablixMember3;
					tablixMember3.Group = dynamicColumns.Grouping;
					tablixMember3.SortExpressions = dynamicColumns.Sorting;
					tablixMember3.Visibility = dynamicColumns.Visibility;
					tablixMember3.DataElementName = dynamicColumns.Grouping.DataCollectionName;
					tablixMember3.DataElementOutput = dynamicColumns.Grouping.DataElementOutput;
					TransferGroupingCustomProperties(tablixMember3, TablixMemberGroupAccessor, TablixMemberCustomPropertiesAccessor);
					TablixHeader tablixHeader2 = tablixMember3.TablixHeader = new TablixHeader();
					tablixHeader2.Size = columnGrouping3.Height;
					tablixHeader2.CellContents = new CellContents();
					if (dynamicColumns.ReportItems.Count > 0)
					{
						tablixHeader2.CellContents.ReportItem = dynamicColumns.ReportItems[0];
					}
					continue;
				}
				if (columnGrouping3.StaticColumns.Count > 0)
				{
					if (columnGrouping != null)
					{
						throw new ArgumentException("More than one ColumnGrouping with StaticColumns.");
					}
					columnGrouping = columnGrouping3;
					num = columnGrouping3.StaticColumns.Count;
					for (int j = 0; j < num; j++)
					{
						TablixMember tablixMember3 = new TablixMember();
						list.Add(tablixMember3);
						TablixHeader tablixHeader4 = tablixMember3.TablixHeader = new TablixHeader();
						tablixHeader4.Size = columnGrouping3.Height;
						tablixHeader4.CellContents = new CellContents();
						if (columnGrouping3.StaticColumns[j].ReportItems.Count > 0)
						{
							tablixHeader4.CellContents.ReportItem = columnGrouping3.StaticColumns[j].ReportItems[0];
						}
						int k;
						for (k = 0; k < matrix.MatrixRows.Count; k++)
						{
							MatrixRow2005 matrixRow = matrix.MatrixRows[k];
							if (matrixRow.MatrixCells.Count > j && matrixRow.MatrixCells[j].ReportItems.Count > 0 && matrixRow.MatrixCells[j].ReportItems[0].DataElementOutput != DataElementOutputTypes.NoOutput)
							{
								break;
							}
						}
						if (k == matrix.MatrixRows.Count)
						{
							tablixMember3.DataElementOutput = DataElementOutputTypes.NoOutput;
						}
					}
					tablixMember = list[0];
					list = tablixMember.TablixMembers;
					continue;
				}
				throw new ArgumentException("No DynamicColumns or StaticColumns.");
			}
			SetKeepTogether(keepTogether);
			list = null;
			keepTogether = null;
			foreach (RowGrouping2005 rowGrouping3 in matrix.RowGroupings)
			{
				if (list == null)
				{
					matrix.TablixRowHierarchy = new TablixHierarchy();
					list = matrix.TablixRowHierarchy.TablixMembers;
				}
				if (rowGrouping3.FixedHeader)
				{
					matrix.FixedRowHeaders = true;
				}
				DynamicRows2005 dynamicRows = rowGrouping3.DynamicRows;
				if (dynamicRows != null)
				{
					TablixMember tablixMember4 = new TablixMember();
					list.Add(tablixMember4);
					list = tablixMember4.TablixMembers;
					tablixMember2 = tablixMember4;
					keepTogether = tablixMember4;
					tablixMember4.Group = dynamicRows.Grouping;
					tablixMember4.SortExpressions = dynamicRows.Sorting;
					tablixMember4.Visibility = dynamicRows.Visibility;
					tablixMember4.DataElementName = dynamicRows.Grouping.DataCollectionName;
					tablixMember4.DataElementOutput = dynamicRows.Grouping.DataElementOutput;
					TransferGroupingCustomProperties(tablixMember4, TablixMemberGroupAccessor, TablixMemberCustomPropertiesAccessor);
					TablixHeader tablixHeader6 = tablixMember4.TablixHeader = new TablixHeader();
					tablixHeader6.Size = rowGrouping3.Width;
					tablixHeader6.CellContents = new CellContents();
					if (dynamicRows.ReportItems.Count > 0)
					{
						tablixHeader6.CellContents.ReportItem = dynamicRows.ReportItems[0];
					}
					continue;
				}
				if (rowGrouping3.StaticRows.Count > 0)
				{
					if (rowGrouping != null)
					{
						throw new ArgumentException("More than one RowGrouping with StaticRows.");
					}
					rowGrouping = rowGrouping3;
					num2 = rowGrouping3.StaticRows.Count;
					for (int j = 0; j < num2; j++)
					{
						TablixMember tablixMember4 = new TablixMember();
						list.Add(tablixMember4);
						TablixHeader tablixHeader8 = tablixMember4.TablixHeader = new TablixHeader();
						tablixHeader8.Size = rowGrouping3.Width;
						tablixHeader8.CellContents = new CellContents();
						if (rowGrouping3.StaticRows[j].ReportItems.Count > 0)
						{
							tablixHeader8.CellContents.ReportItem = rowGrouping3.StaticRows[j].ReportItems[0];
						}
						if (matrix.MatrixRows.Count > j)
						{
							MatrixRow2005 matrixRow2 = matrix.MatrixRows[j];
							int k;
							for (k = 0; k < matrixRow2.MatrixCells.Count && (matrixRow2.MatrixCells[k].ReportItems.Count <= 0 || matrixRow2.MatrixCells[k].ReportItems[0].DataElementOutput == DataElementOutputTypes.NoOutput); k++)
							{
							}
							if (k == matrixRow2.MatrixCells.Count)
							{
								tablixMember4.DataElementOutput = DataElementOutputTypes.NoOutput;
							}
						}
					}
					tablixMember2 = list[0];
					list = tablixMember2.TablixMembers;
					continue;
				}
				throw new ArgumentException("No DynamicRows or StaticRows.");
			}
			SetKeepTogether(keepTogether);
			UpgradePageBreaks(matrix, isTable: false);
			if (matrix.MatrixColumns.Count != num)
			{
				throw new ArgumentException("Wrong number of MatrixColumns.");
			}
			if (matrix.MatrixRows.Count != num2)
			{
				throw new ArgumentException("Wrong number of MatrixRows.");
			}
			foreach (MatrixRow2005 matrixRow3 in matrix.MatrixRows)
			{
				TablixRow tablixRow = new TablixRow();
				tablixBody.TablixRows.Add(tablixRow);
				tablixRow.Height = matrixRow3.Height;
				if (matrixRow3.MatrixCells.Count != num)
				{
					throw new ArgumentException("Wrong number of MatrixCells.");
				}
				foreach (MatrixCell2005 matrixCell in matrixRow3.MatrixCells)
				{
					TablixCell tablixCell = new TablixCell();
					tablixRow.TablixCells.Add(tablixCell);
					tablixCell.DataElementName = matrix.CellDataElementName;
					tablixCell.DataElementOutput = matrix.CellDataElementOutput;
					tablixCell.CellContents = new CellContents();
					if (matrixCell.ReportItems.Count > 0)
					{
						tablixCell.CellContents.ReportItem = matrixCell.ReportItems[0];
					}
				}
			}
			List<int> subTotalRows = new List<int>();
			int outerStaticMembers = num2;
			i = matrix.RowGroupings.Count;
			while (--i >= 0)
			{
				RowGrouping2005 rowGrouping2 = matrix.RowGroupings[i];
				if (rowGrouping2 == rowGrouping)
				{
					outerStaticMembers = 1;
					if (i < matrix.RowGroupings.Count - 1)
					{
						CloneTablixHierarchy(matrix, tablixMember2, cloneRows: true);
					}
				}
				else if (rowGrouping2.DynamicRows != null && rowGrouping2.DynamicRows.Subtotal != null)
				{
					CloneTablixSubtotal(matrix, tablixMember2, rowGrouping2.DynamicRows.Subtotal, outerStaticMembers, num2, rowSubtotal: true, subTotalRows);
				}
				if (i > 0)
				{
					tablixMember2 = (TablixMember)tablixMember2.Parent;
				}
			}
			outerStaticMembers = num;
			i = matrix.ColumnGroupings.Count;
			while (--i >= 0)
			{
				ColumnGrouping2005 columnGrouping2 = matrix.ColumnGroupings[i];
				if (columnGrouping2.StaticColumns.Count > 0)
				{
					outerStaticMembers = 1;
					if (i < matrix.ColumnGroupings.Count - 1)
					{
						CloneTablixHierarchy(matrix, tablixMember, cloneRows: false);
					}
				}
				else if (columnGrouping2.DynamicColumns != null && columnGrouping2.DynamicColumns.Subtotal != null)
				{
					CloneTablixSubtotal(matrix, tablixMember, columnGrouping2.DynamicColumns.Subtotal, outerStaticMembers, num, rowSubtotal: false, subTotalRows);
				}
				if (i > 0)
				{
					tablixMember = (TablixMember)tablixMember.Parent;
				}
			}
		}

		private void SetKeepTogether(TablixMember innerMostDynamicMember)
		{
			if (innerMostDynamicMember == null)
			{
				return;
			}
			innerMostDynamicMember.KeepTogether = true;
			if (innerMostDynamicMember.TablixMembers == null)
			{
				return;
			}
			foreach (TablixMember tablixMember in innerMostDynamicMember.TablixMembers)
			{
				tablixMember.KeepTogether = true;
			}
		}

		private void CloneTablixHierarchy(Microsoft.ReportingServices.RdlObjectModel.Tablix tablix, TablixMember staticMember, bool cloneRows)
		{
			if (staticMember.TablixMembers.Count == 0)
			{
				return;
			}
			TablixBody tablixBody = tablix.TablixBody;
			IList<TablixMember> siblingTablixMembers = GetSiblingTablixMembers(staticMember);
			int count = siblingTablixMembers.Count;
			for (int i = 1; i < count; i++)
			{
				TablixMember tablixMember = siblingTablixMembers[i];
				Cloner cloner = new Cloner(this);
				tablixMember.TablixMembers = (IList<TablixMember>)cloner.Clone(staticMember.TablixMembers);
				cloner.FixReferences();
				if (!cloneRows)
				{
					int num = tablixBody.TablixColumns.Count / count;
					foreach (TablixRow tablixRow in tablixBody.TablixRows)
					{
						int num2 = num;
						int num3 = i * num;
						while (num2-- > 0)
						{
							cloner.FixReferences(tablixRow.TablixCells[num3].CellContents.ReportItem);
							num3++;
						}
					}
				}
				else
				{
					int num = tablixBody.TablixRows.Count / count;
					int num2 = num;
					int num3 = i * num;
					while (num2-- > 0)
					{
						cloner.FixReferences(tablixBody.TablixRows[num3].TablixCells);
						num3++;
					}
				}
			}
		}

		private IList<TablixMember> GetSiblingTablixMembers(TablixMember tablixMember)
		{
			if (!(tablixMember.Parent is TablixHierarchy))
			{
				return ((TablixMember)tablixMember.Parent).TablixMembers;
			}
			return ((TablixHierarchy)tablixMember.Parent).TablixMembers;
		}

		private void CloneTablixSubtotal(Microsoft.ReportingServices.RdlObjectModel.Tablix tablix, TablixMember dynamicMember, Subtotal2005 subtotal, int outerStaticMembers, int originalCount, bool rowSubtotal, List<int> subTotalRows)
		{
			string name = tablix.Name;
			bool flag = true;
			for (TablixMember tablixMember = dynamicMember.Parent as TablixMember; tablixMember != null; tablixMember = (tablixMember.Parent as TablixMember))
			{
				if (tablixMember.Group != null)
				{
					flag = false;
					name = tablixMember.Group.Name;
					break;
				}
			}
			Cloner cloner = new Cloner(this);
			ProcessClonedDynamicTablixMember(dynamicMember, cloner, name);
			TablixMember tablixMember2 = new TablixMember();
			if (flag)
			{
				tablixMember2.HideIfNoRows = true;
			}
			GetSiblingTablixMembers(dynamicMember).Insert((subtotal.Position != SubtotalPositions.Before) ? 1 : 0, tablixMember2);
			tablixMember2.DataElementName = subtotal.DataElementName;
			tablixMember2.DataElementOutput = subtotal.DataElementOutput;
			TablixHeader tablixHeader2 = tablixMember2.TablixHeader = new TablixHeader();
			tablixHeader2.Size = dynamicMember.TablixHeader.Size;
			tablixHeader2.CellContents = new CellContents();
			tablixHeader2.CellContents.ReportItem = subtotal.ReportItems[0];
			CloneSubtotalTablixMembers(cloner, tablixMember2, dynamicMember.TablixMembers, name);
			FixupMutualReferences(cloner.TextboxNameValueExprTable);
			TablixBody tablixBody = tablix.TablixBody;
			if (!rowSubtotal)
			{
				int num = originalCount / outerStaticMembers;
				int num2 = tablixBody.TablixColumns.Count / outerStaticMembers;
				for (int i = 0; i < outerStaticMembers; i++)
				{
					int num3 = i * (num + num2);
					int num4 = (subtotal.Position == SubtotalPositions.Before) ? num3 : (num3 + num2);
					int num5 = 0;
					while (num5 < num)
					{
						for (int j = 0; j < tablixBody.TablixRows.Count; j++)
						{
							TablixRow tablixRow = tablixBody.TablixRows[j];
							TablixCell tablixCell = (TablixCell)cloner.Clone(tablixRow.TablixCells[num3]);
							if (!subTotalRows.Contains(j))
							{
								cloner.ApplySubTotalStyleOverrides(tablixCell.CellContents.ReportItem, subtotal.Style);
							}
							tablixRow.TablixCells.Insert(num4, tablixCell);
						}
						TablixColumn item = (TablixColumn)cloner.Clone(tablixBody.TablixColumns[num3]);
						tablixBody.TablixColumns.Insert(num4, item);
						if (num3 >= num4)
						{
							num3++;
						}
						num5++;
						num3++;
						num4++;
					}
				}
			}
			else
			{
				int num = originalCount / outerStaticMembers;
				int num2 = tablixBody.TablixRows.Count / outerStaticMembers;
				for (int i = 0; i < outerStaticMembers; i++)
				{
					int num3 = i * (num + num2);
					int num4 = (subtotal.Position == SubtotalPositions.Before) ? num3 : (num3 + num2);
					int num5 = 0;
					while (num5 < num)
					{
						TablixRow tablixRow2 = (TablixRow)cloner.Clone(tablixBody.TablixRows[num3]);
						foreach (TablixCell tablixCell2 in tablixRow2.TablixCells)
						{
							cloner.ApplySubTotalStyleOverrides(tablixCell2.CellContents.ReportItem, subtotal.Style);
						}
						tablixBody.TablixRows.Insert(num4, tablixRow2);
						subTotalRows.Add(num4);
						if (num3 >= num4)
						{
							num3++;
						}
						num5++;
						num3++;
						num4++;
					}
				}
			}
			cloner.FixReferences();
		}

		private void CloneSubtotalTablixMembers(Cloner cloner, TablixMember tablixMember, IList<TablixMember> tablixMembers, string parentScope)
		{
			if (tablixMembers.Count <= 0)
			{
				return;
			}
			TablixMember tablixMember2 = null;
			if (tablixMembers[0].Group != null)
			{
				tablixMember2 = tablixMembers[0];
			}
			else if (tablixMembers.Count > 1 && tablixMembers[1].Group != null)
			{
				tablixMember2 = tablixMembers[1];
			}
			if (tablixMember2 != null)
			{
				ReportSize size = tablixMember2.TablixHeader.Size;
				tablixMember.TablixHeader.Size += size;
				ProcessClonedDynamicTablixMember(tablixMember2, cloner, parentScope);
				CloneSubtotalTablixMembers(cloner, tablixMember, tablixMember2.TablixMembers, parentScope);
				return;
			}
			foreach (TablixMember tablixMember4 in tablixMembers)
			{
				TablixMember tablixMember3 = new TablixMember();
				tablixMember.TablixMembers.Add(tablixMember3);
				tablixMember3.Visibility = (Microsoft.ReportingServices.RdlObjectModel.Visibility)cloner.Clone(tablixMember4.Visibility);
				tablixMember3.TablixHeader = (TablixHeader)cloner.Clone(tablixMember4.TablixHeader);
				tablixMember3.DataElementName = tablixMember4.DataElementName;
				tablixMember3.DataElementOutput = tablixMember4.DataElementOutput;
				CloneSubtotalTablixMembers(cloner, tablixMember3, tablixMember4.TablixMembers, parentScope);
			}
		}

		private void ProcessClonedDynamicTablixMember(TablixMember dynamicMember, Cloner cloner, string parentScope)
		{
			cloner.AddNameMapping(dynamicMember.Group.Name, parentScope);
			if (dynamicMember.TablixHeader.CellContents != null)
			{
				CollectInScopeTextboxValues(dynamicMember.TablixHeader.CellContents.ReportItem, cloner.TextboxNameValueExprTable);
			}
		}

		private void CollectInScopeTextboxValues(Microsoft.ReportingServices.RdlObjectModel.ReportItem reportItem, Dictionary<string, string> nameValueExprTable)
		{
			if (reportItem == null)
			{
				return;
			}
			if (reportItem is Textbox)
			{
				Textbox textbox = (Textbox)reportItem;
				string text = textbox.Paragraphs[0].TextRuns[0].Value.Value;
				if (ReportExpression.IsExpressionString(text))
				{
					text = text.Substring(1);
				}
				ReplaceReportItemReferenceWithValue(text, nameValueExprTable);
				nameValueExprTable[textbox.Name] = text;
			}
			else if (reportItem is Microsoft.ReportingServices.RdlObjectModel.Rectangle)
			{
				Microsoft.ReportingServices.RdlObjectModel.Rectangle rectangle = (Microsoft.ReportingServices.RdlObjectModel.Rectangle)reportItem;
				CollectInScopeTextboxValues(rectangle.ReportItems, nameValueExprTable);
			}
			else
			{
				if (!(reportItem is Microsoft.ReportingServices.RdlObjectModel.Tablix))
				{
					return;
				}
				Microsoft.ReportingServices.RdlObjectModel.Tablix tablix = (Microsoft.ReportingServices.RdlObjectModel.Tablix)reportItem;
				if (tablix.TablixCorner != null)
				{
					IList<IList<TablixCornerCell>> tablixCornerRows = tablix.TablixCorner.TablixCornerRows;
					if (tablixCornerRows != null)
					{
						foreach (IList<TablixCornerCell> item in tablixCornerRows)
						{
							if (item == null)
							{
								continue;
							}
							foreach (TablixCornerCell item2 in item)
							{
								if (item2 != null && item2.CellContents != null)
								{
									CollectInScopeTextboxValues(item2.CellContents.ReportItem, nameValueExprTable);
								}
							}
						}
					}
				}
				CollectInScopeTextboxValues(tablix.TablixColumnHierarchy, nameValueExprTable);
				CollectInScopeTextboxValues(tablix.TablixRowHierarchy, nameValueExprTable);
			}
		}

		private void CollectInScopeTextboxValues(TablixHierarchy hierarchy, Dictionary<string, string> nameValueExprTable)
		{
			if (hierarchy != null && hierarchy.TablixMembers != null)
			{
				CollectInScopeTextboxValues(hierarchy.TablixMembers, nameValueExprTable);
			}
		}

		private void CollectInScopeTextboxValues(IList<TablixMember> tablixMembers, Dictionary<string, string> nameValueExprTable)
		{
			foreach (TablixMember tablixMember in tablixMembers)
			{
				if (tablixMember != null && tablixMember.Group == null)
				{
					if (tablixMember.TablixHeader != null && tablixMember.TablixHeader.CellContents != null)
					{
						CollectInScopeTextboxValues(tablixMember.TablixHeader.CellContents.ReportItem, nameValueExprTable);
					}
					CollectInScopeTextboxValues(tablixMember.TablixMembers, nameValueExprTable);
				}
			}
		}

		private void CollectInScopeTextboxValues(IList<Microsoft.ReportingServices.RdlObjectModel.ReportItem> reportItems, Dictionary<string, string> nameValueExprTable)
		{
			if (reportItems == null)
			{
				return;
			}
			foreach (Microsoft.ReportingServices.RdlObjectModel.ReportItem reportItem in reportItems)
			{
				CollectInScopeTextboxValues(reportItem, nameValueExprTable);
			}
		}

		private string ReplaceReference(string expression, string oldValue, string newValue)
		{
			MatchCollection matchCollection = m_regexes.ReportItemName.Matches(expression);
			int num = 0;
			int newLength = newValue.Length;
			int oldLength = oldValue.Length;
			foreach (Match item in matchCollection)
			{
				System.Text.RegularExpressions.Group group = item.Groups["reportitemname"];
				if (group != null && group.Value.Equals(oldValue, StringComparison.OrdinalIgnoreCase))
				{
					expression = expression.Substring(0, num + group.Index) + newValue + expression.Substring(num + group.Index + oldLength);
					num += newLength - oldLength;
				}
			}
			expression = FixAggregateFunctions(expression, delegate(string expr, int currentOffset, string specialFunctionName, int specialFunctionPos, int argumentsPos, int scopePos, int scopeLength, ref int offset)
			{
				if (scopeLength != 0)
				{
					Match match = m_regexes.StringLiteralOnly.Match(expr, scopePos, scopeLength);
					if (match.Success && match.Groups["string"].Value.Equals(oldValue, StringComparison.OrdinalIgnoreCase))
					{
						scopePos = match.Groups["string"].Index;
						expr = expr.Substring(0, scopePos) + newValue + expr.Substring(scopePos + oldLength);
						offset += newLength - oldLength;
					}
				}
				return expr;
			});
			return expression;
		}

		private string ReplaceReportItemReferenceWithValue(string expression, Dictionary<string, string> nameValueExprTable)
		{
			if (nameValueExprTable.Count == 0)
			{
				return expression;
			}
			MatchCollection matchCollection = m_regexes.ReportItemValueReference.Matches(expression);
			int num = 0;
			foreach (Match item in matchCollection)
			{
				System.Text.RegularExpressions.Group group = item.Groups["reportitemname"];
				if (group != null && nameValueExprTable.TryGetValue(group.Value, out string value))
				{
					value = "(" + value + ")";
					int length = value.Length;
					int length2 = item.Value.Length;
					expression = expression.Substring(0, num + item.Index) + value + expression.Substring(num + item.Index + length2);
					num += length - length2;
				}
			}
			return expression;
		}

		private void FixupMutualReferences(Dictionary<string, string> nameValueExprTable)
		{
			if (nameValueExprTable.Count == 0)
			{
				return;
			}
			string[] array = new string[nameValueExprTable.Count];
			nameValueExprTable.Keys.CopyTo(array, 0);
			for (int i = 0; i < array.Length - 1; i++)
			{
				foreach (string key in array)
				{
					string text2 = nameValueExprTable[key] = ReplaceReportItemReferenceWithValue(nameValueExprTable[key], nameValueExprTable);
				}
			}
		}

		private int GetScopeArgumentIndex(string function)
		{
			switch (function.ToUpperInvariant())
			{
			case "RUNNINGVALUE":
				return 2;
			case "ROWNUMBER":
			case "COUNTROWS":
				return 0;
			default:
				return 1;
			}
		}

		private bool FindArgument(int currentPos, string expression, out int newPos, int argumentIndex, out int argumentPos, out int argumentLength)
		{
			int num = 1;
			int num2 = 0;
			argumentPos = currentPos;
			argumentLength = 0;
			while (0 < num && currentPos < expression.Length)
			{
				Match match = m_regexes.Arguments.Match(expression, currentPos);
				if (!match.Success)
				{
					currentPos = expression.Length;
					continue;
				}
				string text = match.Result("${openParen}");
				string text2 = match.Result("${closeParen}");
				string text3 = match.Result("${comma}");
				if (text != null && text.Length != 0)
				{
					num++;
				}
				else if (text2 != null && text2.Length != 0)
				{
					num--;
					if (num == 0)
					{
						if (num2 == argumentIndex)
						{
							argumentLength = match.Index - argumentPos;
						}
						num2++;
					}
				}
				else if (text3 != null && text3.Length != 0 && 1 == num)
				{
					if (num2 == argumentIndex)
					{
						argumentLength = match.Index - argumentPos;
					}
					num2++;
					if (num2 == argumentIndex)
					{
						argumentPos = match.Index + 1;
					}
				}
				currentPos = match.Index + match.Length;
			}
			newPos = currentPos;
			return argumentLength != 0;
		}

		public UpgradeImpl2005(bool throwUpgradeException)
			: this(throwUpgradeException, upgradeDundasCRIToNative: true, renameInvalidDataSources: true)
		{
		}

		public UpgradeImpl2005(bool throwUpgradeException, bool upgradeDundasCRIToNative, bool renameInvalidDataSources)
		{
			m_throwUpgradeException = throwUpgradeException;
			m_upgradeDundasCRIToNative = upgradeDundasCRIToNative;
			m_renameInvalidDataSources = renameInvalidDataSources;
			m_regexes = ReportRegularExpressions.Value;
		}

		internal override Type GetReportType()
		{
			return typeof(Report2005);
		}

		protected override void InitUpgrade()
		{
			m_dataSourceNameTable = new Hashtable();
			m_dataSourceCaseSensitiveNameTable = new Hashtable();
			m_upgradeable = new List<IUpgradeable>();
			m_dataSources = new List<DataSource2005>();
			m_nameTable = new Hashtable();
			base.InitUpgrade();
		}

		protected override void Upgrade(Microsoft.ReportingServices.RdlObjectModel.Report report)
		{
			if (m_dataSources != null)
			{
				foreach (DataSource2005 dataSource in m_dataSources)
				{
					dataSource.Upgrade(this);
				}
			}
			foreach (IUpgradeable item in m_upgradeable)
			{
				item.Upgrade(this);
				if (item is CustomReportItem2005)
				{
					CustomReportItem2005 customReportItem = (CustomReportItem2005)item;
					if (customReportItem.Type == "DundasChartControl" && m_upgradeDundasCRIToNative)
					{
						Microsoft.ReportingServices.RdlObjectModel.Chart chart = new Microsoft.ReportingServices.RdlObjectModel.Chart();
						UpgradeDundasCRIChart(customReportItem, chart);
						ChangeReportItem(customReportItem.Parent, customReportItem, chart);
					}
					else if (customReportItem.Type == "DundasGaugeControl" && m_upgradeDundasCRIToNative)
					{
						GaugePanel gaugePanel = new GaugePanel();
						UpgradeDundasCRIGaugePanel(customReportItem, gaugePanel);
						ChangeReportItem(customReportItem.Parent, customReportItem, gaugePanel);
					}
					else if (m_throwUpgradeException)
					{
						throw new CRI2005UpgradeException();
					}
				}
			}
			AdjustBodyWhitespace((Report2005)report);
			base.Upgrade(report);
		}

		protected override RdlSerializerSettings CreateReaderSettings()
		{
			return UpgradeSerializerSettings2005.CreateReaderSettings();
		}

		protected override RdlSerializerSettings CreateWriterSettings()
		{
			return UpgradeSerializerSettings2005.CreateWriterSettings();
		}

		protected override void SetupReaderSettings(RdlSerializerSettings settings)
		{
			SerializerHost2005 obj = (SerializerHost2005)settings.Host;
			obj.Upgradeable = m_upgradeable;
			obj.DataSources = m_dataSources;
			obj.NameTable = m_nameTable;
			base.SetupReaderSettings(settings);
		}

		private void ChangeReportItem(object parentObject, object oldReportItem, object newReportItem)
		{
			TypeMapping typeMapping = TypeMapper.GetTypeMapping(parentObject.GetType());
			if (!(typeMapping is StructMapping))
			{
				return;
			}
			foreach (MemberMapping member in ((StructMapping)typeMapping).Members)
			{
				object value = member.GetValue(parentObject);
				if (member.Type == typeof(RdlCollection<Microsoft.ReportingServices.RdlObjectModel.ReportItem>))
				{
					RdlCollection<Microsoft.ReportingServices.RdlObjectModel.ReportItem> rdlCollection = (RdlCollection<Microsoft.ReportingServices.RdlObjectModel.ReportItem>)value;
					int num = rdlCollection.IndexOf((Microsoft.ReportingServices.RdlObjectModel.ReportItem)oldReportItem);
					if (num != -1)
					{
						rdlCollection[num] = (Microsoft.ReportingServices.RdlObjectModel.ReportItem)newReportItem;
						break;
					}
				}
				else if (value == oldReportItem)
				{
					member.SetValue(parentObject, newReportItem);
					break;
				}
			}
		}

		private static void AdjustBodyWhitespace(Report2005 report)
		{
			if (report.Body.ReportItems == null || report.Body.ReportItems.Count == 0)
			{
				return;
			}
			double num = 0.0;
			double num2 = 0.0;
			double num3 = report.Width.ToPixels();
			double num4 = report.Body.Height.ToPixels();
			foreach (Microsoft.ReportingServices.RdlObjectModel.ReportItem reportItem in report.Body.ReportItems)
			{
				num = ((!reportItem.Width.IsEmpty) ? Math.Max(num, reportItem.Left.ToPixels() + reportItem.Width.ToPixels()) : num3);
				num2 = ((!reportItem.Height.IsEmpty) ? Math.Max(num2, reportItem.Top.ToPixels() + reportItem.Height.ToPixels()) : num4);
			}
			num4 = Math.Min(num4, num2);
			report.Body.Height = ReportSize.FromPixels(num4, report.Body.Height.Type);
			double num5 = Math.Max(1.0, report.Page.PageWidth.ToPixels() - report.Page.LeftMargin.ToPixels() - report.Page.RightMargin.ToPixels());
			if (report.Page.Columns > 1)
			{
				num5 -= (double)(report.Page.Columns - 1) * report.Page.ColumnSpacing.ToPixels();
				num5 = Math.Max(1.0, num5 / (double)report.Page.Columns);
			}
			num3 = Math.Min(num3, num5 * Math.Ceiling(num / num5));
			report.Width = ReportSize.FromPixels(num3, report.Width.Type);
		}

		internal static Microsoft.ReportingServices.RdlObjectModel.Group TablixMemberGroupAccessor(object member)
		{
			return ((TablixMember)member).Group;
		}

		internal static IList<CustomProperty> TablixMemberCustomPropertiesAccessor(object member)
		{
			return ((TablixMember)member).CustomProperties;
		}

		internal static Microsoft.ReportingServices.RdlObjectModel.Group ChartMemberGroupAccessor(object member)
		{
			return ((ChartMember)member).Group;
		}

		internal static IList<CustomProperty> ChartMemberCustomPropertiesAccessor(object member)
		{
			return ((ChartMember)member).CustomProperties;
		}

		internal static Microsoft.ReportingServices.RdlObjectModel.Group DataMemberGroupAccessor(object member)
		{
			return ((DataMember)member).Group;
		}

		internal static IList<CustomProperty> DataMemberCustomPropertiesAccessor(object member)
		{
			return ((DataMember)member).CustomProperties;
		}

		internal static string SplitName(string name)
		{
			return Regex.Replace(name, "(\\p{Ll})(\\p{Lu})|_+", "$1 $2");
		}

		internal void UpgradeReport(Report2005 report)
		{
			report.ConsumeContainerWhitespace = true;
			Body2005 body = report.Body as Body2005;
			if (body != null)
			{
				report.Page.Columns = body.Columns;
				report.Page.ColumnSpacing = body.ColumnSpacing;
			}
			Microsoft.ReportingServices.RdlObjectModel.Style style = body.Style;
			if (style != null && (style.Border == null || style.Border.Style == BorderStyles.None) && (style.TopBorder == null || style.TopBorder.Style == BorderStyles.None) && (style.BottomBorder == null || style.BottomBorder.Style == BorderStyles.None) && (style.LeftBorder == null || style.LeftBorder.Style == BorderStyles.None) && (style.RightBorder == null || style.RightBorder.Style == BorderStyles.None))
			{
				report.Page.Style = style;
				report.Body.Style = null;
				style = null;
			}
			foreach (ReportParameter2005 reportParameter in report.ReportParameters)
			{
				if (reportParameter.Nullable && (reportParameter.DefaultValue == null || (reportParameter.DefaultValue.Values.Count == 0 && reportParameter.DefaultValue.DataSetReference == null)))
				{
					if (reportParameter.DefaultValue == null)
					{
						reportParameter.DefaultValue = new DefaultValue();
					}
					reportParameter.DefaultValue.Values.Add(null);
				}
				if (reportParameter.Prompt.HasValue && reportParameter.Prompt.Value.Value == "")
				{
					reportParameter.Hidden = true;
					reportParameter.Prompt = reportParameter.Name;
				}
			}
			if (report.Page.InteractiveHeight == report.Page.PageHeight)
			{
				report.Page.InteractiveHeight = ReportSize.Empty;
			}
			if (report.Page.InteractiveWidth == report.Page.PageWidth)
			{
				report.Page.InteractiveWidth = ReportSize.Empty;
			}
		}

		internal void UpgradeReportItem(Microsoft.ReportingServices.RdlObjectModel.ReportItem item)
		{
			IReportItem2005 reportItem = (IReportItem2005)item;
			if (reportItem.Action != null)
			{
				item.ActionInfo = new ActionInfo();
				item.ActionInfo.Actions.Add(reportItem.Action);
			}
			UpgradeDataElementOutput(item);
		}

		internal void UpgradeDataElementOutput(Microsoft.ReportingServices.RdlObjectModel.ReportItem reportItem)
		{
			if (reportItem.DataElementOutput == DataElementOutputTypes.Auto && reportItem.Visibility != null && reportItem.Visibility.Hidden.IsExpression)
			{
				reportItem.DataElementOutput = DataElementOutputTypes.NoOutput;
			}
		}

		internal void UpgradePageBreak(IPageBreakLocation2005 item)
		{
			if (item.PageBreak == null && (item.PageBreakAtStart || item.PageBreakAtEnd))
			{
				item.PageBreak = new PageBreak();
				item.PageBreak.BreakLocation = ((!item.PageBreakAtStart) ? BreakLocations.End : ((!item.PageBreakAtEnd) ? BreakLocations.Start : BreakLocations.StartAndEnd));
			}
		}

		internal void UpgradeRectangle(Rectangle2005 rectangle)
		{
			if (rectangle.DataElementOutput == DataElementOutputTypes.Auto)
			{
				rectangle.DataElementOutput = DataElementOutputTypes.ContentsOnly;
			}
			UpgradeReportItem(rectangle);
			UpgradePageBreak(rectangle);
		}

		internal void UpgradeCustomReportItem(CustomReportItem2005 cri)
		{
			UpgradeReportItem(cri);
		}

		internal void UpgradeDataGrouping(DataGrouping2005 dataGrouping)
		{
			if (!dataGrouping.Static && dataGrouping.Group == null)
			{
				Microsoft.ReportingServices.RdlObjectModel.Group group = new Microsoft.ReportingServices.RdlObjectModel.Group();
				string parentReportItemName = GetParentReportItemName(dataGrouping);
				group.Name = UniqueName(parentReportItemName + "_Group", group);
				dataGrouping.Group = group;
			}
			else
			{
				TransferGroupingCustomProperties(dataGrouping, DataMemberGroupAccessor, DataMemberCustomPropertiesAccessor);
			}
		}

		internal void UpgradeList(List2005 list)
		{
			UpgradeReportItem(list);
			UpgradePageBreak(list);
			list.TablixColumnHierarchy = new TablixHierarchy();
			TablixMember item = new TablixMember();
			list.TablixColumnHierarchy.TablixMembers.Add(item);
			TablixMember tablixMember = new TablixMember();
			list.TablixRowHierarchy = new TablixHierarchy();
			list.TablixRowHierarchy.TablixMembers.Add(tablixMember);
			if (list.Grouping == null)
			{
				Microsoft.ReportingServices.RdlObjectModel.Group group = new Microsoft.ReportingServices.RdlObjectModel.Group();
				group = new Microsoft.ReportingServices.RdlObjectModel.Group();
				group.Name = UniqueName(list.Name + "_Details_Group", group);
				tablixMember.Group = group;
				if (list.DataInstanceName == null)
				{
					tablixMember.Group.DataElementName = "Item";
					tablixMember.DataElementName = "Item_Collection";
				}
				else
				{
					tablixMember.Group.DataElementName = list.DataInstanceName;
					tablixMember.DataElementName = list.DataInstanceName + "_Collection";
				}
			}
			else
			{
				tablixMember.Group = list.Grouping;
				_ = (Grouping2005)list.Grouping;
				UpgradePageBreaks(list, isTable: false);
			}
			tablixMember.DataElementOutput = list.DataInstanceElementOutput;
			tablixMember.KeepTogether = true;
			TransferGroupingCustomProperties(tablixMember, TablixMemberGroupAccessor, TablixMemberCustomPropertiesAccessor);
			TablixColumn tablixColumn = new TablixColumn();
			list.TablixBody.TablixColumns.Add(tablixColumn);
			tablixColumn.Width = GetReportItemWidth(list);
			TablixRow tablixRow = new TablixRow();
			list.TablixBody.TablixRows.Add(tablixRow);
			tablixRow.Height = GetReportItemHeight(list);
			TablixCell tablixCell = new TablixCell();
			tablixRow.TablixCells.Add(tablixCell);
			Microsoft.ReportingServices.RdlObjectModel.Rectangle rectangle = new Microsoft.ReportingServices.RdlObjectModel.Rectangle();
			tablixCell.CellContents = new CellContents();
			tablixCell.CellContents.ReportItem = rectangle;
			rectangle.KeepTogether = true;
			rectangle.Name = UniqueName(list.Name + "_Contents", rectangle);
			rectangle.ReportItems = list.ReportItems;
			bool containsPostSortAggregate = false;
			if (IsUpgradedListDetailMember(tablixMember))
			{
				FixAggregateFunction(rectangle.ReportItems, list.Name, list.Name, fixPreviousAggregate: false, ref containsPostSortAggregate);
			}
			else
			{
				FixAggregateFunction(rectangle.ReportItems);
			}
			if (list.Visibility != null)
			{
				string toggleItem = list.Visibility.ToggleItem;
				bool flag = false;
				if (toggleItem != null && m_nameTable.ContainsKey(toggleItem))
				{
					flag = true;
					if (tablixMember.Group.Parent != null && TextBoxExistsInCollection(rectangle.ReportItems, toggleItem))
					{
						tablixMember.Visibility = list.Visibility;
						list.Visibility = null;
					}
				}
				if (!flag && tablixMember.Visibility == null)
				{
					tablixMember.Visibility = new Microsoft.ReportingServices.RdlObjectModel.Visibility();
					tablixMember.Visibility.Hidden = list.Visibility.Hidden;
					list.Visibility.Hidden = null;
				}
				if (IsUpgradedListDetailMember(tablixMember))
				{
					FixAggregateFunction(tablixMember.Visibility, list.Name, list.Name, fixPreviousAggregate: false, ref containsPostSortAggregate);
				}
				else
				{
					FixAggregateFunction(tablixMember.Visibility);
				}
			}
			if (list.Sorting != null && list.Sorting.Count != 0)
			{
				if (!containsPostSortAggregate || list.Grouping != null || SortingContainsAggregate(list.Sorting))
				{
					tablixMember.SortExpressions = list.Sorting;
				}
				else
				{
					list.SortExpressions = list.Sorting;
				}
			}
		}

		private bool IsUpgradedListDetailMember(TablixMember rowMember)
		{
			if (rowMember.Group.GroupExpressions != null)
			{
				return rowMember.Group.GroupExpressions.Count == 0;
			}
			return true;
		}

		private bool TextBoxExistsInCollection(IList<Microsoft.ReportingServices.RdlObjectModel.ReportItem> reportItems, string name)
		{
			if (reportItems != null)
			{
				foreach (Microsoft.ReportingServices.RdlObjectModel.ReportItem reportItem in reportItems)
				{
					if (reportItem is Microsoft.ReportingServices.RdlObjectModel.Rectangle)
					{
						if (TextBoxExistsInCollection(((Microsoft.ReportingServices.RdlObjectModel.Rectangle)reportItem).ReportItems, name))
						{
							return true;
						}
					}
					else if (reportItem is Matrix2005)
					{
						if (((Matrix2005)reportItem).Corner != null && TextBoxExistsInCollection(((Matrix2005)reportItem).Corner.ReportItems, name))
						{
							return true;
						}
					}
					else if (reportItem is Table2005)
					{
						Table2005 table = (Table2005)reportItem;
						if (table.Header != null && TextBoxExistsInCollection(table.Header.TableRows, name))
						{
							return true;
						}
						if (table.Footer != null && TextBoxExistsInCollection(table.Footer.TableRows, name))
						{
							return true;
						}
					}
					else if (reportItem is Textbox && reportItem.Name == name)
					{
						return true;
					}
				}
			}
			return false;
		}

		private bool TextBoxExistsInCollection(IList<TableRow2005> rows, string name)
		{
			if (rows != null && rows.Count > 0)
			{
				foreach (TableRow2005 row in rows)
				{
					IList<TableCell2005> tableCells = row.TableCells;
					if (tableCells == null || tableCells.Count <= 0)
					{
						continue;
					}
					foreach (TableCell2005 item in tableCells)
					{
						if (TextBoxExistsInCollection(item.ReportItems, name))
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		internal void UpgradeTable(Table2005 table)
		{
			UpgradeReportItem(table);
			UpgradePageBreak(table);
			int count = table.TableColumns.Count;
			table.TablixColumnHierarchy = new TablixHierarchy();
			for (int i = 0; i < count; i++)
			{
				TableColumn2005 tableColumn = table.TableColumns[i];
				TablixMember tablixMember = new TablixMember();
				tablixMember.Visibility = tableColumn.Visibility;
				tablixMember.FixedData = tableColumn.FixedHeader;
				table.TablixColumnHierarchy.TablixMembers.Add(tablixMember);
				TablixColumn tablixColumn = new TablixColumn();
				tablixColumn.Width = tableColumn.Width;
				table.TablixBody.TablixColumns.Add(tablixColumn);
			}
			table.TablixRowHierarchy = new TablixHierarchy();
			IList<TablixMember> tablixMembers = table.TablixRowHierarchy.TablixMembers;
			int index = 0;
			int num = 0;
			bool containsPostSortAggregate = false;
			if (table.Header != null)
			{
				int i;
				for (i = 0; i < table.Header.TableRows.Count; i++)
				{
					TablixMember tablixMember = new TablixMember();
					tablixMembers.Add(tablixMember);
					tablixMember.FixedData = table.Header.FixedHeader;
					tablixMember.KeepTogether = true;
					tablixMember.KeepWithGroup = KeepWithGroupTypes.After;
					if (table.Header.RepeatOnNewPage)
					{
						tablixMember.RepeatOnNewPage = true;
					}
					TablixRow obj = UpgradeTableRow(table.Header.TableRows[i], table, i, tablixMember);
					FixAggregateFunction(obj, ref containsPostSortAggregate);
				}
				index = (num = i);
			}
			if (table.Footer != null)
			{
				for (int i = 0; i < table.Footer.TableRows.Count; i++)
				{
					TablixMember tablixMember = new TablixMember();
					tablixMembers.Add(tablixMember);
					tablixMember.KeepTogether = true;
					tablixMember.KeepWithGroup = KeepWithGroupTypes.Before;
					if (table.Footer.RepeatOnNewPage)
					{
						tablixMember.RepeatOnNewPage = true;
					}
					TablixRow obj2 = UpgradeTableRow(table.Footer.TableRows[i], table, num + i, tablixMember);
					FixAggregateFunction(obj2, ref containsPostSortAggregate);
				}
			}
			for (int i = 0; i < table.TableGroups.Count; i++)
			{
				TableGroup2005 tableGroup = table.TableGroups[i];
				TablixMember tablixMember = new TablixMember();
				tablixMembers.Insert(index, tablixMember);
				tablixMember.Visibility = tableGroup.Visibility;
				tablixMember.Group = tableGroup.Grouping;
				tablixMember.SortExpressions = tableGroup.Sorting;
				TransferGroupingCustomProperties(tablixMember, TablixMemberGroupAccessor, TablixMemberCustomPropertiesAccessor);
				tablixMembers = tablixMember.TablixMembers;
				index = 0;
				if (tableGroup.Header != null)
				{
					int j;
					for (j = 0; j < tableGroup.Header.TableRows.Count; j++)
					{
						tablixMember = new TablixMember();
						tablixMembers.Add(tablixMember);
						tablixMember.KeepTogether = true;
						tablixMember.KeepWithGroup = KeepWithGroupTypes.After;
						if (tableGroup.Header.RepeatOnNewPage)
						{
							tablixMember.RepeatOnNewPage = true;
						}
						TablixRow obj3 = UpgradeTableRow(tableGroup.Header.TableRows[j], table, num + j, tablixMember);
						FixAggregateFunction(obj3);
					}
					index = j;
					num += j;
				}
				if (tableGroup.Footer != null)
				{
					for (int j = 0; j < tableGroup.Footer.TableRows.Count; j++)
					{
						tablixMember = new TablixMember();
						tablixMembers.Add(tablixMember);
						tablixMember.KeepTogether = true;
						tablixMember.KeepWithGroup = KeepWithGroupTypes.Before;
						if (tableGroup.Footer.RepeatOnNewPage)
						{
							tablixMember.RepeatOnNewPage = true;
						}
						TablixRow obj4 = UpgradeTableRow(tableGroup.Footer.TableRows[j], table, num + j, tablixMember);
						FixAggregateFunction(obj4);
					}
				}
				if (i == table.TableGroups.Count - 1 && tableGroup.Header == null && tableGroup.Footer == null && table.Details == null)
				{
					tablixMember = new TablixMember();
					tablixMembers.Add(tablixMember);
					tablixMember.Visibility = new Microsoft.ReportingServices.RdlObjectModel.Visibility();
					tablixMember.Visibility.Hidden = true;
					TablixRow tablixRow = new TablixRow();
					table.TablixBody.TablixRows.Insert(num, tablixRow);
					for (int j = 0; j < count; j++)
					{
						TablixCell tablixCell = new TablixCell();
						tablixCell.CellContents = new CellContents();
						tablixRow.TablixCells.Add(tablixCell);
					}
					num++;
				}
			}
			Details2005 details = table.Details;
			if (details != null)
			{
				TablixMember tablixMember = new TablixMember();
				tablixMembers.Insert(index, tablixMember);
				tablixMember.Visibility = details.Visibility;
				Microsoft.ReportingServices.RdlObjectModel.Group group = details.Grouping;
				if (group == null)
				{
					group = new Microsoft.ReportingServices.RdlObjectModel.Group();
					group.Name = UniqueName(table.Name + "_Details_Group", group);
				}
				tablixMember.Group = group;
				tablixMember.DataElementOutput = table.DetailDataElementOutput;
				if (table.DetailDataElementName == null)
				{
					tablixMember.Group.DataElementName = "Detail";
				}
				else
				{
					tablixMember.Group.DataElementName = table.DetailDataElementName;
				}
				if (table.DetailDataCollectionName == null)
				{
					tablixMember.DataElementName = tablixMember.Group.DataElementName + "_Collection";
				}
				else
				{
					tablixMember.DataElementName = table.DetailDataCollectionName;
				}
				TransferGroupingCustomProperties(tablixMember, TablixMemberGroupAccessor, TablixMemberCustomPropertiesAccessor);
				for (int i = 0; i < details.TableRows.Count; i++)
				{
					TablixMember tablixMember2 = new TablixMember();
					tablixMember.TablixMembers.Add(tablixMember2);
					tablixMember.KeepTogether = true;
					TablixRow obj5 = UpgradeTableRow(details.TableRows[i], table, num + i, tablixMember2);
					if (group.GroupExpressions.Count == 0)
					{
						string name = table.Name;
						if (table.TableGroups.Count > 0)
						{
							name = table.TableGroups[table.TableGroups.Count - 1].Grouping.Name;
						}
						FixAggregateFunction(obj5, name, table.Name, fixPreviousAggregate: false, ref containsPostSortAggregate);
						FixAggregateFunction(tablixMember2, name, table.Name, fixPreviousAggregate: false, ref containsPostSortAggregate);
					}
				}
				if (details.Sorting != null && details.Sorting.Count != 0)
				{
					if (!containsPostSortAggregate || table.TableGroups.Count != 0 || SortingContainsAggregate(details.Sorting))
					{
						tablixMember.SortExpressions = details.Sorting;
					}
					else
					{
						table.SortExpressions = details.Sorting;
					}
				}
			}
			if (table.TablixBody.TablixRows.Count == 0)
			{
				if (table.TablixBody.TablixColumns.Count > 0)
				{
					TablixMember tablixMember = new TablixMember();
					table.TablixRowHierarchy.TablixMembers.Add(tablixMember);
					tablixMember.Visibility = new Microsoft.ReportingServices.RdlObjectModel.Visibility();
					tablixMember.Visibility.Hidden = true;
					TablixRow tablixRow2 = new TablixRow();
					table.TablixBody.TablixRows.Insert(num, tablixRow2);
					for (int j = 0; j < count; j++)
					{
						TablixCell tablixCell2 = new TablixCell();
						tablixCell2.CellContents = new CellContents();
						tablixRow2.TablixCells.Add(tablixCell2);
					}
				}
				else
				{
					table.TablixBody = null;
				}
			}
			UpgradePageBreaks(table, isTable: true);
		}

		private bool SortingContainsAggregate(IList<SortExpression> sortExpressions)
		{
			if (sortExpressions == null || sortExpressions.Count == 0)
			{
				return false;
			}
			for (int i = 0; i < sortExpressions.Count; i++)
			{
				if (SortExpressionContainsAggregate(sortExpressions[i]))
				{
					return true;
				}
			}
			return false;
		}

		private bool SortExpressionContainsAggregate(SortExpression sortExpression)
		{
			if (sortExpression == null || null == sortExpression.Value || sortExpression.Value.Value == null || !sortExpression.Value.IsExpression)
			{
				return false;
			}
			return ContainsRegexMatch(sortExpression.Value.Value, m_regexes.SpecialFunction, "sfname");
		}

		private bool ContainsRegexMatch(string expression, Regex regex, string pattern)
		{
			if (string.IsNullOrEmpty(expression))
			{
				return false;
			}
			int num = 0;
			while (num < expression.Length)
			{
				Match match = regex.Match(expression, num);
				if (!match.Success)
				{
					return false;
				}
				if (match.Groups[pattern].Value.Length == 0)
				{
					num = match.Index + match.Length;
					continue;
				}
				return true;
			}
			return false;
		}

		private static bool IsToggleable(Microsoft.ReportingServices.RdlObjectModel.Visibility visibility)
		{
			if (visibility != null)
			{
				if (visibility.ToggleItem == null)
				{
					return visibility.Hidden.IsExpression;
				}
				return true;
			}
			return false;
		}

		private void MergePageBreakLocation(BreakLocations breakLocation, PageBreak pageBreak)
		{
			switch (breakLocation)
			{
			case BreakLocations.Start:
				if (pageBreak.BreakLocation == BreakLocations.End || pageBreak.BreakLocation == BreakLocations.StartAndEnd)
				{
					pageBreak.BreakLocation = BreakLocations.StartAndEnd;
				}
				break;
			case BreakLocations.End:
				if (pageBreak.BreakLocation == BreakLocations.Start || pageBreak.BreakLocation == BreakLocations.StartAndEnd)
				{
					pageBreak.BreakLocation = BreakLocations.StartAndEnd;
				}
				break;
			}
		}

		private void UpgradePageBreaks(Microsoft.ReportingServices.RdlObjectModel.Tablix tablix, bool isTable)
		{
			if (tablix.TablixRowHierarchy == null)
			{
				return;
			}
			IList<TablixMember> tablixMembers = tablix.TablixRowHierarchy.TablixMembers;
			if (tablixMembers == null || tablixMembers.Count <= 0)
			{
				return;
			}
			BreakLocations? breakLocations = UpgradePageBreaks(tablixMembers, IsToggleable(tablix.Visibility), isTable);
			if (breakLocations.HasValue)
			{
				if (tablix.PageBreak == null)
				{
					tablix.PageBreak = new PageBreak();
					tablix.PageBreak.BreakLocation = breakLocations.Value;
				}
				else
				{
					MergePageBreakLocation(breakLocations.Value, tablix.PageBreak);
				}
			}
		}

		private BreakLocations? UpgradePageBreaks(IList<TablixMember> members, bool thisOrAnscestorHasToggle, bool isTable)
		{
			BreakLocations? result = null;
			bool flag = false;
			int num = (!isTable) ? 1 : members.Count;
			TablixMember tablixMember = null;
			for (int i = 0; i < num; i++)
			{
				TablixMember tablixMember2 = members[i];
				if (tablixMember2.Group == null)
				{
					if (isTable)
					{
						if (tablixMember2.RepeatOnNewPage)
						{
							flag = true;
						}
						continue;
					}
					IList<TablixMember> tablixMembers = tablixMember2.TablixMembers;
					if (tablixMembers != null && tablixMembers.Count > 0)
					{
						result = UpgradePageBreaks(tablixMembers, thisOrAnscestorHasToggle, isTable);
					}
					continue;
				}
				tablixMember = tablixMember2;
				break;
			}
			if (tablixMember != null)
			{
				thisOrAnscestorHasToggle |= IsToggleable(tablixMember.Visibility);
				IList<TablixMember> tablixMembers2 = tablixMember.TablixMembers;
				Microsoft.ReportingServices.RdlObjectModel.Group group = tablixMember.Group;
				PageBreak pageBreak = group.PageBreak;
				if (tablixMembers2 != null && tablixMembers2.Count > 0)
				{
					result = UpgradePageBreaks(tablixMembers2, thisOrAnscestorHasToggle, isTable);
					if (result.HasValue)
					{
						if (pageBreak == null)
						{
							pageBreak = new PageBreak();
							pageBreak.BreakLocation = result.Value;
							group.PageBreak = pageBreak;
						}
						else
						{
							MergePageBreakLocation(result.Value, pageBreak);
						}
					}
				}
				if ((!isTable || flag) && pageBreak != null)
				{
					if (!thisOrAnscestorHasToggle)
					{
						result = pageBreak.BreakLocation;
					}
					pageBreak.BreakLocation = BreakLocations.Between;
				}
			}
			return result;
		}

		private ReportSize GetReportItemWidth(Microsoft.ReportingServices.RdlObjectModel.ReportItem reportItem)
		{
			ReportSize result = reportItem.Width;
			ReportSize reportSize = default(ReportSize);
			IContainedObject containedObject = reportItem;
			while (result.IsEmpty && containedObject != null)
			{
				if (containedObject is Microsoft.ReportingServices.RdlObjectModel.ReportItem)
				{
					result = ((Microsoft.ReportingServices.RdlObjectModel.ReportItem)containedObject).Width;
					reportSize += ((Microsoft.ReportingServices.RdlObjectModel.ReportItem)containedObject).Left;
				}
				else
				{
					if (containedObject is Microsoft.ReportingServices.RdlObjectModel.Report)
					{
						result = ((Microsoft.ReportingServices.RdlObjectModel.Report)containedObject).Width;
						break;
					}
					if (containedObject is TableCell2005)
					{
						TableCell2005 obj = (TableCell2005)containedObject;
						TableRow2005 tableRow = (TableRow2005)containedObject.Parent;
						Table2005 parentTable = GetParentTable(tableRow);
						int num = tableRow.TableCells.IndexOf((TableCell2005)containedObject);
						if (num < parentTable.TableColumns.Count)
						{
							result = parentTable.TableColumns[num].Width;
						}
						int num2 = obj.ColSpan;
						while (--num2 > 0 && ++num < parentTable.TableColumns.Count)
						{
							result += parentTable.TableColumns[num].Width;
						}
						break;
					}
					if (containedObject is MatrixCell2005)
					{
						MatrixRow2005 obj2 = (MatrixRow2005)containedObject.Parent;
						Matrix2005 matrix = (Matrix2005)obj2.Parent;
						int num3 = obj2.MatrixCells.IndexOf((MatrixCell2005)containedObject);
						if (num3 < matrix.MatrixColumns.Count)
						{
							result = matrix.MatrixColumns[num3].Width;
						}
						break;
					}
				}
				containedObject = containedObject.Parent;
			}
			if (result.IsEmpty)
			{
				result = new ReportSize(0.0);
			}
			else
			{
				result -= reportSize;
			}
			return result;
		}

		private ReportSize GetReportItemHeight(Microsoft.ReportingServices.RdlObjectModel.ReportItem reportItem)
		{
			ReportSize result = reportItem.Height;
			ReportSize reportSize = default(ReportSize);
			IContainedObject containedObject = reportItem;
			while (result.IsEmpty && containedObject != null)
			{
				if (containedObject is Microsoft.ReportingServices.RdlObjectModel.ReportItem)
				{
					result = ((Microsoft.ReportingServices.RdlObjectModel.ReportItem)containedObject).Height;
					if (result.IsEmpty)
					{
						reportSize += ((Microsoft.ReportingServices.RdlObjectModel.ReportItem)containedObject).Top;
					}
				}
				else
				{
					if (containedObject is Body)
					{
						result = ((Body)containedObject).Height;
						break;
					}
					if (containedObject is TableCell2005)
					{
						result = ((TableRow2005)containedObject.Parent).Height;
						break;
					}
					if (containedObject is MatrixCell2005)
					{
						result = ((MatrixRow2005)containedObject.Parent).Height;
						break;
					}
				}
				containedObject = containedObject.Parent;
			}
			if (result.IsEmpty)
			{
				result = new ReportSize(0.0);
			}
			else
			{
				result -= reportSize;
			}
			return result;
		}

		private Table2005 GetParentTable(TableRow2005 row)
		{
			for (IContainedObject parent = row.Parent; parent != null; parent = parent.Parent)
			{
				if (parent is Table2005)
				{
					return (Table2005)parent;
				}
			}
			return null;
		}

		private void FixAggregateFunction(object obj)
		{
			bool containsPostSortAggregate = false;
			FixAggregateFunction(obj, null, null, fixPreviousAggregate: true, ref containsPostSortAggregate);
		}

		private void FixAggregateFunction(object obj, ref bool containsPostSortAggregate)
		{
			FixAggregateFunction(obj, null, null, fixPreviousAggregate: true, ref containsPostSortAggregate);
		}

		private void FixAggregateFunction(object obj, string defaultScope, string dataRegion, bool fixPreviousAggregate, ref bool containsPostSortAggregate)
		{
			if (obj is IList)
			{
				foreach (object item in (IList)obj)
				{
					FixAggregateFunction(item, defaultScope, dataRegion, fixPreviousAggregate, ref containsPostSortAggregate);
				}
			}
			else
			{
				if (!(obj is ReportObject))
				{
					return;
				}
				foreach (MemberMapping member in ((StructMapping)TypeMapper.GetTypeMapping(obj.GetType())).Members)
				{
					object value = member.GetValue(obj);
					if (value != null)
					{
						if (typeof(IExpression).IsAssignableFrom(value.GetType()))
						{
							member.SetValue(obj, FixAggregateFunction((IExpression)value, defaultScope, dataRegion, fixPreviousAggregate, ref containsPostSortAggregate));
						}
						else
						{
							FixAggregateFunction(value, defaultScope, dataRegion, fixPreviousAggregate, ref containsPostSortAggregate);
						}
					}
				}
			}
		}

		private IExpression FixAggregateFunction(IExpression value, string defaultScope, string dataRegion, bool fixPreviousAggregates, ref bool containsPostSortAggregate)
		{
			if (value != null && value.IsExpression && !string.IsNullOrEmpty(value.Expression))
			{
				string expression = value.Expression;
				if (ContainsRegexMatch(expression, m_regexes.PSAFunction, "psaname"))
				{
					containsPostSortAggregate = true;
				}
				if (m_regexes.SpecialFunction.IsMatch(expression))
				{
					expression = FixAggregateFunctions(expression, delegate(string expr, int currentOffset, string specialFunctionName, int specialFunctionPos, int argumentsPos, int scopePos, int scopeLength, ref int offset)
					{
						if (!specialFunctionName.Equals("Previous", StringComparison.OrdinalIgnoreCase))
						{
							if (scopeLength == 0 && defaultScope != null)
							{
								string text = (GetScopeArgumentIndex(specialFunctionName) > 0) ? ", " : "";
								expr = expr.Substring(0, offset - 1) + text + "\"" + defaultScope + "\")" + expr.Substring(offset);
								offset += defaultScope.Length + 4;
							}
						}
						else if (fixPreviousAggregates)
						{
							expr = FixPreviousAggregate(expr, currentOffset, specialFunctionPos, argumentsPos, ref offset);
						}
						return expr;
					});
					value = (IExpression)Activator.CreateInstance(value.GetType());
					value.Expression = expression;
				}
			}
			return value;
		}

		private string FixPreviousAggregate(string expr, int currentOffset, int specialFunctionPos, int argumentsPos, ref int offset)
		{
			Match match = m_regexes.SpecialFunction.Match(expr, argumentsPos);
			if (match.Success)
			{
				System.Text.RegularExpressions.Group group = match.Groups["sfname"];
				if (group.Length > 0 && group.Index <= offset)
				{
					return expr;
				}
			}
			if (FindArgument(currentOffset, expr, out int _, 0, out int argumentPos, out int argumentLength))
			{
				Match match2 = m_regexes.FieldDetection.Match(expr, argumentPos - 1);
				if (match2.Success)
				{
					int num = 0;
					int num2 = argumentPos;
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append(expr.Substring(0, num2));
					while (match2.Success && num2 < argumentPos + argumentLength)
					{
						System.Text.RegularExpressions.Group group2 = match2.Groups["detected"];
						if (group2.Length > 0)
						{
							stringBuilder.Append(expr.Substring(num2, group2.Index - num2));
							int num3 = 0;
							int i;
							for (i = group2.Index; i < argumentPos + argumentLength && !IsNotPartOfReference(expr[i]); i++)
							{
								if (expr[i] == '(')
								{
									num3++;
								}
								else if (expr[i] == ')')
								{
									if (num3 == 0)
									{
										break;
									}
									num3--;
								}
							}
							stringBuilder.Append("Last(");
							stringBuilder.Append(expr.Substring(group2.Index, i - group2.Index));
							stringBuilder.Append(")");
							num2 = i;
							num++;
						}
						match2 = match2.NextMatch();
					}
					stringBuilder.Append(expr.Substring(num2));
					expr = stringBuilder.ToString();
					offset += num * 6;
				}
			}
			return expr;
		}

		private bool IsNotPartOfReference(char c)
		{
			switch (c)
			{
			case '\t':
			case '\n':
			case '\r':
			case ' ':
			case '&':
			case '*':
			case '+':
			case '-':
			case '/':
			case '<':
			case '=':
			case '>':
			case '\\':
			case '^':
				return true;
			default:
				return false;
			}
		}

		private string FixAggregateFunctions(string expression, AggregateFunctionFixup fixup)
		{
			int newPos = 0;
			while (newPos < expression.Length)
			{
				Match match = m_regexes.SpecialFunction.Match(expression, newPos);
				if (!match.Success)
				{
					break;
				}
				System.Text.RegularExpressions.Group group = match.Groups["sfname"];
				string value = group.Value;
				if (value.Length == 0)
				{
					newPos = match.Index + match.Length;
					continue;
				}
				int index = group.Index;
				int argumentsPos = index + group.Length;
				newPos = match.Index + match.Length;
				int scopeArgumentIndex = GetScopeArgumentIndex(value);
				FindArgument(newPos, expression, out newPos, scopeArgumentIndex, out int argumentPos, out int argumentLength);
				expression = fixup(expression, match.Index + match.Length, value, index, argumentsPos, argumentPos, argumentLength, ref newPos);
			}
			return expression;
		}

		private TablixRow UpgradeTableRow(TableRow2005 tableRow, Microsoft.ReportingServices.RdlObjectModel.Tablix tablix, int rowIndex, TablixMember tablixMember)
		{
			tablixMember.Visibility = tableRow.Visibility;
			TablixRow tablixRow = new TablixRow();
			tablix.TablixBody.TablixRows.Insert(rowIndex, tablixRow);
			tablixRow.Height = tableRow.Height;
			IList<TablixMember> tablixMembers = tablix.TablixColumnHierarchy.TablixMembers;
			int num = 0;
			foreach (TableCell2005 tableCell in tableRow.TableCells)
			{
				TablixCell tablixCell = new TablixCell();
				tablixRow.TablixCells.Add(tablixCell);
				tablixCell.CellContents = new CellContents();
				if (tableCell.ReportItems.Count > 0)
				{
					tablixCell.CellContents.ReportItem = tableCell.ReportItems[0];
				}
				if (tableCell.ColSpan > 1)
				{
					int index = num + tableCell.ColSpan - 1;
					bool flag = tablixMembers[num].FixedData || tablixMembers[index].FixedData;
					tablixCell.CellContents.ColSpan = tableCell.ColSpan;
					for (int i = 0; i < tableCell.ColSpan; i++)
					{
						if (i > 0)
						{
							tablixRow.TablixCells.Add(new TablixCell());
						}
						if (flag)
						{
							tablixMembers[num].FixedData = true;
						}
						num++;
					}
				}
				else
				{
					num++;
				}
			}
			return tablixRow;
		}

		internal void UpgradeChart(Chart2005 chart2005)
		{
			UpgradeReportItem(chart2005);
			UpgradePageBreak(chart2005);
			if (chart2005.CustomProperties == null)
			{
				chart2005.CustomProperties = new List<CustomProperty>();
			}
			CustomProperty customProperty = new CustomProperty();
			customProperty.Name = "__Upgraded2005__";
			customProperty.Value = "__Upgraded2005__";
			chart2005.CustomProperties.Add(customProperty);
			IList<ChartMember> list = null;
			ChartMember chartMember = null;
			foreach (SeriesGrouping2005 seriesGrouping in chart2005.SeriesGroupings)
			{
				if (list == null)
				{
					chart2005.ChartSeriesHierarchy = new ChartSeriesHierarchy();
					list = chart2005.ChartSeriesHierarchy.ChartMembers;
				}
				DynamicSeries2005 dynamicSeries = seriesGrouping.DynamicSeries;
				if (dynamicSeries != null)
				{
					ChartMember chartMember2 = new ChartMember();
					list.Add(chartMember2);
					list = chartMember2.ChartMembers;
					chartMember = chartMember2;
					chartMember2.Group = dynamicSeries.Grouping;
					chartMember2.SortExpressions = dynamicSeries.Sorting;
					chartMember2.Label = dynamicSeries.Label;
					chartMember2.PropertyStore.SetObject(4, dynamicSeries.LabelLocID);
					chartMember2.DataElementName = dynamicSeries.Grouping.DataCollectionName;
					chartMember2.DataElementOutput = dynamicSeries.Grouping.DataElementOutput;
					TransferGroupingCustomProperties(chartMember2, ChartMemberGroupAccessor, ChartMemberCustomPropertiesAccessor);
					continue;
				}
				foreach (StaticMember2005 item in seriesGrouping.StaticSeries)
				{
					ChartMember chartMember2 = new ChartMember();
					list.Add(chartMember2);
					chartMember2.Label = item.Label;
					chartMember2.PropertyStore.SetObject(4, item.LabelLocID);
				}
				if (list.Count > 0)
				{
					chartMember = list[0];
					list = chartMember.ChartMembers;
				}
			}
			list = null;
			foreach (CategoryGrouping2005 categoryGrouping in chart2005.CategoryGroupings)
			{
				if (list == null)
				{
					chart2005.ChartCategoryHierarchy = new ChartCategoryHierarchy();
					list = chart2005.ChartCategoryHierarchy.ChartMembers;
				}
				DynamicSeries2005 dynamicCategories = categoryGrouping.DynamicCategories;
				if (dynamicCategories != null)
				{
					ChartMember chartMember3 = new ChartMember();
					list.Add(chartMember3);
					list = chartMember3.ChartMembers;
					chartMember3.Group = dynamicCategories.Grouping;
					chartMember3.SortExpressions = dynamicCategories.Sorting;
					chartMember3.Label = dynamicCategories.Label;
					chartMember3.PropertyStore.SetObject(4, dynamicCategories.LabelLocID);
					chartMember3.DataElementName = dynamicCategories.Grouping.DataCollectionName;
					chartMember3.DataElementOutput = dynamicCategories.Grouping.DataElementOutput;
					TransferGroupingCustomProperties(chartMember3, ChartMemberGroupAccessor, ChartMemberCustomPropertiesAccessor);
					continue;
				}
				foreach (StaticMember2005 staticCategory in categoryGrouping.StaticCategories)
				{
					ChartMember chartMember3 = new ChartMember();
					list.Add(chartMember3);
					chartMember3.Label = staticCategory.Label;
					chartMember3.PropertyStore.SetObject(4, staticCategory.LabelLocID);
				}
				break;
			}
			if (chart2005.Palette.Value == ChartPalettes.GrayScale)
			{
				chart2005.PaletteHatchBehavior = ChartPaletteHatchBehaviorTypes.Always;
			}
			if (chart2005.Action != null)
			{
				chart2005.ActionInfo = new ActionInfo();
				chart2005.ActionInfo.Actions.Add(chart2005.Action);
			}
			if (chart2005.NoRows != null)
			{
				chart2005.ChartNoDataMessage = new Microsoft.ReportingServices.RdlObjectModel.ChartTitle();
				chart2005.ChartNoDataMessage.Name = "NoDataMessageTitle";
				chart2005.ChartNoDataMessage.Caption = chart2005.NoRows.ToString();
			}
			ChartArea chartArea = new ChartArea();
			chart2005.ChartAreas.Add(chartArea);
			chartArea.Name = "Default";
			if (chart2005.ThreeDProperties != null)
			{
				chartArea.ChartThreeDProperties = new ChartThreeDProperties();
				chartArea.ChartThreeDProperties.Clustered = !chart2005.ThreeDProperties.Clustered.Value;
				chartArea.ChartThreeDProperties.DepthRatio = chart2005.ThreeDProperties.DepthRatio;
				chartArea.ChartThreeDProperties.Enabled = chart2005.ThreeDProperties.Enabled;
				chartArea.ChartThreeDProperties.GapDepth = chart2005.ThreeDProperties.GapDepth;
				chartArea.ChartThreeDProperties.Inclination = chart2005.ThreeDProperties.Rotation;
				chartArea.ChartThreeDProperties.Rotation = chart2005.ThreeDProperties.Inclination;
				chartArea.ChartThreeDProperties.Shading = chart2005.ThreeDProperties.Shading;
				ChartProjectionModes projectionMode = (ChartProjectionModes)chart2005.ThreeDProperties.ProjectionMode;
				chartArea.ChartThreeDProperties.ProjectionMode = projectionMode;
				if (projectionMode == ChartProjectionModes.Perspective)
				{
					chartArea.ChartThreeDProperties.Perspective = chart2005.ThreeDProperties.Perspective;
				}
				int num = (int)(30.0 * ((double)chart2005.ThreeDProperties.WallThickness / 100.0));
				chartArea.ChartThreeDProperties.WallThickness = ((num > 30 || num < 0) ? 7 : num);
			}
			if (chart2005.PlotArea != null)
			{
				chartArea.Style = chart2005.PlotArea.Style;
				FixYukonChartBorderWidth(chartArea.Style, roundValue: false);
			}
			if (((ReportElement)chart2005).Style != null && ((ReportElement)chart2005).Style.BackgroundImage != null)
			{
				if (chartArea.Style == null)
				{
					chartArea.Style = new Microsoft.ReportingServices.RdlObjectModel.Style();
				}
				chartArea.Style.BackgroundImage = ((ReportElement)chart2005).Style.BackgroundImage;
				((ReportElement)chart2005).Style.BackgroundImage = null;
			}
			if (chart2005.Title != null && chart2005.Title.Caption.Value.Length > 0)
			{
				Microsoft.ReportingServices.RdlObjectModel.ChartTitle chartTitle = new Microsoft.ReportingServices.RdlObjectModel.ChartTitle();
				chartTitle.Name = "Default";
				chart2005.ChartTitles.Add(chartTitle);
				chartTitle.Caption = chart2005.Title.Caption;
				chartTitle.PropertyStore.SetObject(2, chart2005.Title.PropertyStore.GetObject(2));
				chartTitle.Style = chart2005.Title.Style;
			}
			ChartLegend chartLegend = new ChartLegend();
			chartLegend.AutoFitTextDisabled = true;
			chartLegend.Name = "Default";
			chart2005.ChartLegends.Add(chartLegend);
			chartLegend.Hidden = !chart2005.Legend.Visible;
			chartLegend.Style = FixYukonEmptyBorderStyle(chart2005.Legend.Style);
			FixYukonChartBorderWidth(chartLegend.Style, roundValue: false);
			chartLegend.Position = chart2005.Legend.Position;
			chartLegend.Layout = new ReportExpression<ChartLegendLayouts>((ChartLegendLayouts)chart2005.Legend.Layout);
			if (chart2005.Legend.InsidePlotArea)
			{
				chartLegend.DockOutsideChartArea = !chart2005.Legend.InsidePlotArea;
				chartLegend.DockToChartArea = chartArea.Name;
			}
			if (chart2005.CategoryAxis != null)
			{
				chartArea.ChartCategoryAxes.Add(UpgradeChartAxis(chart2005.CategoryAxis.Axis, categoryAxis: true, chart2005.Type));
				chartArea.ChartCategoryAxes[0].Name = "Primary";
			}
			if (chart2005.ValueAxis != null)
			{
				chartArea.ChartValueAxes.Add(UpgradeChartAxis(chart2005.ValueAxis.Axis, categoryAxis: false, chart2005.Type));
				chartArea.ChartValueAxes[0].Name = "Primary";
			}
			if (chart2005.ChartData != null && chart2005.ChartData.Count > 0)
			{
				((Microsoft.ReportingServices.RdlObjectModel.Chart)chart2005).ChartData = new ChartData();
				foreach (ChartSeries2005 chartDatum in chart2005.ChartData)
				{
					ChartSeries chartSeries = new ChartSeries();
					chartSeries.CategoryAxisName = "Primary";
					chartSeries.ValueAxisName = "Primary";
					SetChartTypes(chart2005, chartDatum.PlotType, chartSeries);
					double num2 = 0.8;
					if (chart2005.PointWidth > 0)
					{
						num2 = Math.Min((double)chart2005.PointWidth / 100.0, 2.0);
					}
					else if (chart2005.Type == ChartTypes2005.Bar || chart2005.Type == ChartTypes2005.Column)
					{
						num2 = 0.6;
					}
					if (num2 != 0.8)
					{
						CustomProperty customProperty2 = new CustomProperty();
						customProperty2.Name = "PointWidth";
						customProperty2.Value = num2.ToString(CultureInfo.InvariantCulture.NumberFormat);
						chartSeries.CustomProperties.Add(customProperty2);
					}
					if ((chart2005.Type == ChartTypes2005.Bar || chart2005.Type == ChartTypes2005.Column) && chart2005.ThreeDProperties != null && chart2005.ThreeDProperties.Enabled == true && chart2005.ThreeDProperties.DrawingStyle != 0)
					{
						CustomProperty customProperty3 = new CustomProperty();
						customProperty3.Name = "DrawingStyle";
						customProperty3.Value = chart2005.ThreeDProperties.DrawingStyle.ToString();
						chartSeries.CustomProperties.Add(customProperty3);
					}
					((Microsoft.ReportingServices.RdlObjectModel.Chart)chart2005).ChartData.ChartSeriesCollection.Add(chartSeries);
					chartSeries.Name = "Series" + ((Microsoft.ReportingServices.RdlObjectModel.Chart)chart2005).ChartData.ChartSeriesCollection.Count.ToString(CultureInfo.InvariantCulture.NumberFormat);
					foreach (DataPoint2005 dataPoint in chartDatum.DataPoints)
					{
						Microsoft.ReportingServices.RdlObjectModel.ChartDataPoint chartDataPoint = dataPoint;
						chartDataPoint.DataElementName = dataPoint.DataElementName;
						chartDataPoint.DataElementOutput = ((dataPoint.DataElementOutput == DataElementOutputTypes.Output) ? DataElementOutputTypes.ContentsOnly : dataPoint.DataElementOutput);
						if (dataPoint.Style != null)
						{
							chartDataPoint.Style = new EmptyColorStyle(dataPoint.Style.PropertyStore);
							if (chartDataPoint.Style.Border != null && chartDataPoint.Style.Border.Style == BorderStyles.None)
							{
								chartDataPoint.Style.Border.Style = BorderStyles.Solid;
							}
						}
						if (chartSeries.Type == ChartTypes.Line)
						{
							if (chartDataPoint.Style == null)
							{
								chartDataPoint.Style = new EmptyColorStyle();
							}
							if (chartDataPoint.Style.Border == null)
							{
								chartDataPoint.Style.Border = new EmptyBorder();
							}
							if (!chartDataPoint.Style.Border.Width.IsExpression && chartDataPoint.Style.Border.Width.Value.IsEmpty)
							{
								chartDataPoint.Style.Border.Width = new ReportSize(2.25, SizeTypes.Point);
							}
							else
							{
								FixYukonChartBorderWidth(chartDataPoint.Style, roundValue: false);
							}
							if (!chartDataPoint.Style.Border.Color.Value.IsEmpty || chartDataPoint.Style.Border.Color.IsExpression)
							{
								chartDataPoint.Style.Color = chartDataPoint.Style.Border.Color;
								chartDataPoint.Style.Border.Color = ReportColor.Empty;
							}
						}
						else
						{
							if (chartDataPoint.Style != null && (!chartDataPoint.Style.BackgroundColor.Value.IsEmpty || chartDataPoint.Style.BackgroundColor.IsExpression))
							{
								chartDataPoint.Style.Color = chartDataPoint.Style.BackgroundColor;
								chartDataPoint.Style.BackgroundColor = ReportColor.Empty;
							}
							FixYukonChartBorderWidth(chartDataPoint.Style, roundValue: false);
						}
						if (chart2005.Type == ChartTypes2005.Pie || chart2005.Type == ChartTypes2005.Doughnut)
						{
							if (chartDataPoint.Style == null)
							{
								chartDataPoint.Style = new EmptyColorStyle();
							}
							if (chartDataPoint.Style.Border == null)
							{
								chartDataPoint.Style.Border = new EmptyBorder();
							}
							if (!chartDataPoint.Style.Border.Color.IsExpression && chartDataPoint.Style.Border.Color.Value.IsEmpty)
							{
								chartDataPoint.Style.Border.Color = new ReportColor(Color.Black);
							}
						}
						if (dataPoint.DataValues != null && dataPoint.DataValues.Count > 0)
						{
							chartDataPoint.ChartDataPointValues = new ChartDataPointValues();
							switch (chart2005.Type)
							{
							case ChartTypes2005.Scatter:
								SetChartDataPointNames(dataPoint, m_scatterChartDataPointNames);
								break;
							case ChartTypes2005.Bubble:
								SetChartDataPointNames(dataPoint, m_bubbleChartDataPointNames);
								break;
							case ChartTypes2005.Stock:
							{
								string[] names = (chart2005.Subtype != ChartSubtypes2005.HighLowClose) ? m_openHighLowCloseDataPointNames : m_highLowCloseDataPointNames;
								SetChartDataPointNames(dataPoint, names);
								break;
							}
							}
							foreach (DataValue2005 dataValue in dataPoint.DataValues)
							{
								switch (dataValue.Name)
								{
								case "X":
									chartDataPoint.ChartDataPointValues.X = dataValue.Value;
									break;
								case "High":
									chartDataPoint.ChartDataPointValues.High = dataValue.Value;
									break;
								case "Low":
									chartDataPoint.ChartDataPointValues.Low = dataValue.Value;
									break;
								case "Open":
									chartDataPoint.ChartDataPointValues.Start = dataValue.Value;
									break;
								case "Close":
									chartDataPoint.ChartDataPointValues.End = dataValue.Value;
									break;
								case "Size":
									chartDataPoint.ChartDataPointValues.Size = dataValue.Value;
									break;
								default:
									chartDataPoint.ChartDataPointValues.Y = dataValue.Value;
									break;
								}
							}
						}
						if (dataPoint.Action != null)
						{
							dataPoint.ActionInfo = new ActionInfo();
							dataPoint.ActionInfo.Actions.Add(dataPoint.Action);
						}
						if (dataPoint.DataLabel != null)
						{
							Microsoft.ReportingServices.RdlObjectModel.ChartDataLabel chartDataLabel2 = chartDataPoint.ChartDataLabel = new Microsoft.ReportingServices.RdlObjectModel.ChartDataLabel();
							chartDataLabel2.Visible = dataPoint.DataLabel.Visible;
							chartDataLabel2.UseValueAsLabel = (dataPoint.DataLabel.Visible && dataPoint.DataLabel.Value == null);
							chartDataLabel2.Style = dataPoint.DataLabel.Style;
							chartDataLabel2.Label = dataPoint.DataLabel.Value;
							chartDataLabel2.PropertyStore.SetObject(2, dataPoint.DataLabel.ValueLocID);
							chartDataLabel2.Rotation = dataPoint.DataLabel.Rotation;
							if (dataPoint.DataLabel.Position != ChartDataLabelPositions.Auto)
							{
								if (chartSeries.ChartSmartLabel == null)
								{
									chartSeries.ChartSmartLabel = new ChartSmartLabel();
								}
								chartSeries.ChartSmartLabel.Disabled = true;
							}
							if ((chart2005.Type == ChartTypes2005.Pie || chart2005.Type == ChartTypes2005.Doughnut) && dataPoint.DataLabel.Position != ChartDataLabelPositions.Auto && dataPoint.DataLabel.Position != ChartDataLabelPositions.Center)
							{
								CustomProperty customProperty4 = new CustomProperty();
								customProperty4.Name = "PieLabelStyle";
								customProperty4.Value = "Outside";
								dataPoint.CustomProperties.Add(customProperty4);
							}
							else
							{
								chartDataLabel2.Position = dataPoint.DataLabel.Position;
							}
						}
						if (dataPoint.Marker != null)
						{
							chartDataPoint.ChartMarker = new ChartMarker();
							chartDataPoint.ChartMarker.Type = dataPoint.Marker.Type;
							chartDataPoint.ChartMarker.Size = dataPoint.Marker.Size;
							if (dataPoint.Marker.Style != null)
							{
								chartDataPoint.ChartMarker.Style = new EmptyColorStyle(dataPoint.Marker.Style.PropertyStore);
								chartDataPoint.ChartMarker.Style.Color = chartDataPoint.ChartMarker.Style.BackgroundColor;
								chartDataPoint.ChartMarker.Style.BackgroundColor = ReportColor.Empty;
							}
						}
						if (chart2005.Type == ChartTypes2005.Bubble)
						{
							if (chartDataPoint.ChartMarker == null)
							{
								chartDataPoint.ChartMarker = new ChartMarker();
							}
							if (chartDataPoint.ChartMarker.Type == ChartMarkerTypes.None)
							{
								chartDataPoint.ChartMarker.Type = ChartMarkerTypes.Circle;
							}
						}
						if (chart2005.Palette.Value == ChartPalettes.GrayScale)
						{
							if (chartDataPoint.Style == null)
							{
								chartDataPoint.Style = new EmptyColorStyle();
							}
							else if (chartDataPoint.Style.Color.IsExpression || !chartDataPoint.Style.Color.Value.IsEmpty || chartDataPoint.Style.BackgroundGradientType.IsExpression || (chartDataPoint.Style.BackgroundGradientType.Value != 0 && chartDataPoint.Style.BackgroundGradientType.Value != BackgroundGradients.None))
							{
								chartDataPoint.Style.BackgroundHatchType = BackgroundHatchTypes.None;
							}
							if (chartDataPoint.Style.Border == null)
							{
								chartDataPoint.Style.Border = new EmptyBorder();
								chartDataPoint.Style.Border.Color = new ReportColor(Color.Black);
								chartDataPoint.Style.Border.Width = new ReportSize(0.75, SizeTypes.Point);
								chartDataPoint.Style.Border.Style = BorderStyles.Solid;
							}
							else if (!chartDataPoint.Style.Border.Color.IsExpression && chartDataPoint.Style.Border.Color.Value.IsEmpty)
							{
								chartDataPoint.Style.Border.Color = new ReportColor(Color.Black);
							}
						}
						chartSeries.ChartDataPoints.Add(chartDataPoint);
					}
				}
			}
			if (chart2005.ChartCategoryHierarchy == null || chart2005.ChartCategoryHierarchy.ChartMembers == null || chart2005.ChartCategoryHierarchy.ChartMembers.Count == 0)
			{
				if (chart2005.ChartCategoryHierarchy == null)
				{
					chart2005.ChartCategoryHierarchy = new ChartCategoryHierarchy();
				}
				if (chart2005.ChartCategoryHierarchy.ChartMembers == null)
				{
					chart2005.ChartCategoryHierarchy.ChartMembers = new RdlCollection<ChartMember>();
				}
				if (((Microsoft.ReportingServices.RdlObjectModel.Chart)chart2005).ChartData != null && ((Microsoft.ReportingServices.RdlObjectModel.Chart)chart2005).ChartData.ChartSeriesCollection != null && ((Microsoft.ReportingServices.RdlObjectModel.Chart)chart2005).ChartData.ChartSeriesCollection.Count > 0)
				{
					foreach (Microsoft.ReportingServices.RdlObjectModel.ChartDataPoint chartDataPoint2 in ((Microsoft.ReportingServices.RdlObjectModel.Chart)chart2005).ChartData.ChartSeriesCollection[0].ChartDataPoints)
					{
						_ = chartDataPoint2;
						chart2005.ChartCategoryHierarchy.ChartMembers.Add(new ChartMember());
					}
				}
				else
				{
					chart2005.ChartCategoryHierarchy.ChartMembers.Add(new ChartMember());
				}
			}
			if (chart2005.ChartSeriesHierarchy == null || chart2005.ChartSeriesHierarchy.ChartMembers == null || chart2005.ChartSeriesHierarchy.ChartMembers.Count == 0)
			{
				if (chart2005.ChartSeriesHierarchy == null)
				{
					chart2005.ChartSeriesHierarchy = new ChartSeriesHierarchy();
				}
				if (chart2005.ChartSeriesHierarchy.ChartMembers == null)
				{
					chart2005.ChartSeriesHierarchy.ChartMembers = new RdlCollection<ChartMember>();
				}
				chart2005.ChartSeriesHierarchy.ChartMembers.Add(new ChartMember());
			}
			if (((Microsoft.ReportingServices.RdlObjectModel.Chart)chart2005).ChartData == null || ((Microsoft.ReportingServices.RdlObjectModel.Chart)chart2005).ChartData.ChartSeriesCollection == null || ((Microsoft.ReportingServices.RdlObjectModel.Chart)chart2005).ChartData.ChartSeriesCollection.Count == 0)
			{
				if (((Microsoft.ReportingServices.RdlObjectModel.Chart)chart2005).ChartData == null)
				{
					((Microsoft.ReportingServices.RdlObjectModel.Chart)chart2005).ChartData = new ChartData();
				}
				if (((Microsoft.ReportingServices.RdlObjectModel.Chart)chart2005).ChartData.ChartSeriesCollection == null)
				{
					((Microsoft.ReportingServices.RdlObjectModel.Chart)chart2005).ChartData.ChartSeriesCollection = new RdlCollection<ChartSeries>();
				}
				ChartSeries chartSeries2 = new ChartSeries();
				chartSeries2.Name = "emptySeriesName";
				chartSeries2.ChartDataPoints.Add(new Microsoft.ReportingServices.RdlObjectModel.ChartDataPoint());
				((Microsoft.ReportingServices.RdlObjectModel.Chart)chart2005).ChartData.ChartSeriesCollection.Add(chartSeries2);
			}
			int num3 = chart2005.SeriesGroupings.Count;
			while (--num3 >= 0)
			{
				if (chart2005.SeriesGroupings[num3].StaticSeries.Count > 0 && num3 < chart2005.SeriesGroupings.Count - 1)
				{
					CloneChartSeriesHierarchy(chart2005, chartMember);
				}
				if (num3 > 0)
				{
					chartMember = (ChartMember)chartMember.Parent;
				}
			}
		}

		private void CloneChartSeriesHierarchy(Microsoft.ReportingServices.RdlObjectModel.Chart chart, ChartMember staticMember)
		{
			if (staticMember.ChartMembers.Count == 0)
			{
				return;
			}
			ChartData chartData = chart.ChartData;
			IList<ChartMember> siblingChartMembers = GetSiblingChartMembers(staticMember);
			int count = siblingChartMembers.Count;
			for (int i = 1; i < count; i++)
			{
				ChartMember chartMember = siblingChartMembers[i];
				Cloner cloner = new Cloner(this);
				chartMember.ChartMembers = (IList<ChartMember>)cloner.Clone(staticMember.ChartMembers);
				cloner.FixReferences();
				int num = chartData.ChartSeriesCollection.Count / count;
				int num2 = num;
				int num3 = i * num;
				while (num2-- > 0)
				{
					cloner.FixReferences(chartData.ChartSeriesCollection[num3].ChartDataPoints);
					num3++;
				}
			}
		}

		private IList<ChartMember> GetSiblingChartMembers(ChartMember chartMember)
		{
			if (chartMember.Parent is ChartSeriesHierarchy)
			{
				return ((ChartSeriesHierarchy)chartMember.Parent).ChartMembers;
			}
			if (chartMember.Parent is ChartCategoryHierarchy)
			{
				return ((ChartCategoryHierarchy)chartMember.Parent).ChartMembers;
			}
			return ((ChartMember)chartMember.Parent).ChartMembers;
		}

		private ChartAxis UpgradeChartAxis(Axis2005 axis2005, bool categoryAxis, ChartTypes2005 charType)
		{
			ChartAxis chartAxis = new ChartAxis();
			chartAxis.HideLabels = !axis2005.Visible;
			chartAxis.Margin = new ReportExpression<ChartAxisMarginVisibleTypes>(axis2005.Margin.ToString(), CultureInfo.InvariantCulture);
			chartAxis.Reverse = axis2005.Reverse;
			chartAxis.CrossAt = axis2005.CrossAt;
			chartAxis.Interlaced = axis2005.Interlaced;
			chartAxis.Scalar = axis2005.Scalar;
			chartAxis.LogScale = axis2005.LogScale;
			chartAxis.PreventFontShrink = true;
			chartAxis.PreventFontGrow = true;
			chartAxis.Style = FixYukonEmptyBorderStyle(axis2005.Style);
			FixYukonChartBorderWidth(chartAxis.Style, roundValue: true);
			if (!categoryAxis || chartAxis.Scalar)
			{
				chartAxis.Minimum = axis2005.Min;
				chartAxis.Maximum = axis2005.Max;
			}
			chartAxis.IncludeZero = false;
			double num = double.NaN;
			double num2 = double.NaN;
			if (axis2005.MajorGridLines != null)
			{
				chartAxis.ChartMajorGridLines = new ChartGridLines();
				chartAxis.ChartMajorGridLines.Enabled = new ReportExpression<ChartGridLinesEnabledTypes>(axis2005.MajorGridLines.ShowGridLines.ToString(), CultureInfo.InvariantCulture);
				chartAxis.ChartMajorGridLines.Style = FixYukonEmptyBorderStyle(axis2005.MajorGridLines.Style);
				FixYukonChartBorderWidth(chartAxis.ChartMajorGridLines.Style, roundValue: true);
				if (axis2005.MajorInterval.IsExpression)
				{
					chartAxis.ChartMajorGridLines.Interval = new ReportExpression<double>(axis2005.MajorInterval.ToString(), CultureInfo.InvariantCulture);
				}
				else
				{
					num = ConvertToDouble(axis2005.MajorInterval.Value);
					if (num < 0.0)
					{
						num = double.NaN;
					}
					chartAxis.ChartMajorGridLines.Interval = num;
					if (!chartAxis.Scalar && chartAxis.Margin == ChartAxisMarginVisibleTypes.True)
					{
						chartAxis.ChartMajorGridLines.IntervalOffset = 1.0;
					}
				}
			}
			if (axis2005.MinorGridLines != null)
			{
				chartAxis.ChartMinorGridLines = new ChartGridLines();
				chartAxis.ChartMinorGridLines.Enabled = new ReportExpression<ChartGridLinesEnabledTypes>(axis2005.MinorGridLines.ShowGridLines.ToString(), CultureInfo.InvariantCulture);
				chartAxis.ChartMinorGridLines.Style = FixYukonEmptyBorderStyle(axis2005.MinorGridLines.Style);
				FixYukonChartBorderWidth(chartAxis.ChartMinorGridLines.Style, roundValue: true);
				if (axis2005.MinorInterval.IsExpression)
				{
					chartAxis.ChartMinorGridLines.Interval = new ReportExpression<double>(axis2005.MinorInterval.ToString(), CultureInfo.InvariantCulture);
				}
				else
				{
					num2 = ConvertToDouble(axis2005.MinorInterval.Value);
					if (num2 < 0.0)
					{
						num2 = double.NaN;
					}
					chartAxis.ChartMinorGridLines.Interval = num2;
					if (!chartAxis.Scalar && chartAxis.Margin == ChartAxisMarginVisibleTypes.False)
					{
						chartAxis.ChartMinorGridLines.IntervalOffset = -1.0;
					}
				}
			}
			chartAxis.ChartMajorTickMarks = new ChartTickMarks();
			chartAxis.ChartMajorTickMarks.Type = new ReportExpression<ChartTickMarkTypes>(axis2005.MajorTickMarks.ToString(), CultureInfo.InvariantCulture);
			if (axis2005.MajorTickMarks != TickMarks2005.None)
			{
				if (chartAxis.ChartMajorGridLines != null)
				{
					chartAxis.ChartMajorTickMarks.Style = chartAxis.ChartMajorGridLines.Style;
					chartAxis.ChartMajorTickMarks.Interval = chartAxis.ChartMajorGridLines.Interval;
					chartAxis.ChartMajorTickMarks.IntervalOffset = chartAxis.ChartMajorGridLines.IntervalOffset;
				}
				chartAxis.ChartMajorTickMarks.Enabled = ChartTickMarksEnabledTypes.True;
			}
			chartAxis.ChartMinorTickMarks = new ChartTickMarks();
			chartAxis.ChartMinorTickMarks.Type = new ReportExpression<ChartTickMarkTypes>(axis2005.MinorTickMarks.ToString(), CultureInfo.InvariantCulture);
			if (axis2005.MinorTickMarks != TickMarks2005.None)
			{
				if (chartAxis.ChartMinorGridLines != null)
				{
					chartAxis.ChartMinorTickMarks.Style = chartAxis.ChartMinorGridLines.Style;
					chartAxis.ChartMinorTickMarks.Interval = chartAxis.ChartMinorGridLines.Interval;
					chartAxis.ChartMinorTickMarks.IntervalOffset = chartAxis.ChartMinorGridLines.IntervalOffset;
				}
				chartAxis.ChartMinorTickMarks.Enabled = ChartTickMarksEnabledTypes.True;
			}
			if (axis2005.Title != null && axis2005.Title.Caption != null)
			{
				ChartAxisTitle chartAxisTitle2 = chartAxis.ChartAxisTitle = new ChartAxisTitle();
				chartAxisTitle2.Caption = axis2005.Title.Caption;
				chartAxisTitle2.PropertyStore.SetObject(1, axis2005.Title.PropertyStore.GetObject(2));
				chartAxisTitle2.Position = new ReportExpression<ChartAxisTitlePositions>(axis2005.Title.Position.ToString(), CultureInfo.InvariantCulture);
				chartAxisTitle2.Style = axis2005.Title.Style;
			}
			if (categoryAxis)
			{
				if (!chartAxis.Scalar)
				{
					if (!double.IsNaN(num))
					{
						chartAxis.Interval = (double.IsNaN(num2) ? num : Math.Min(num, num2));
					}
					else if (!double.IsNaN(num2))
					{
						chartAxis.Interval = num2;
					}
					else if (charType != ChartTypes2005.Bar)
					{
						chartAxis.Interval = 1.0;
					}
				}
				else
				{
					chartAxis.Interval = num;
				}
			}
			return chartAxis;
		}

		private double ConvertToDouble(string value)
		{
			double result = double.NaN;
			if (double.TryParse(value, out result))
			{
				return result;
			}
			return double.NaN;
		}

		private void SetChartDataPointNames(DataPoint2005 dataPoint, string[] names)
		{
			int num = Math.Min(dataPoint.DataValues.Count, names.Length);
			for (int i = 0; i < num; i++)
			{
				dataPoint.DataValues[i].Name = names[i];
			}
		}

		private void SetChartTypes(Chart2005 oldChart, PlotTypes2005 plotType, ChartSeries newSeries)
		{
			if (plotType == PlotTypes2005.Line && oldChart.Type != ChartTypes2005.Line)
			{
				newSeries.Type = ChartTypes.Line;
				newSeries.Subtype = ChartSubtypes.Plain;
				return;
			}
			switch (oldChart.Type)
			{
			case (ChartTypes2005)3:
				break;
			case ChartTypes2005.Column:
			case ChartTypes2005.Bar:
			case ChartTypes2005.Line:
			case ChartTypes2005.Area:
				newSeries.Type = new ReportExpression<ChartTypes>(oldChart.Type.ToString(), CultureInfo.InvariantCulture);
				newSeries.Subtype = new ReportExpression<ChartSubtypes>(oldChart.Subtype.ToString(), CultureInfo.InvariantCulture);
				break;
			case ChartTypes2005.Pie:
				newSeries.Type = ChartTypes.Shape;
				if (oldChart.Subtype == ChartSubtypes2005.OpenHighLowClose)
				{
					newSeries.Subtype = ChartSubtypes.ExplodedPie;
				}
				else
				{
					newSeries.Subtype = ChartSubtypes.Pie;
				}
				break;
			case ChartTypes2005.Scatter:
				if (oldChart.Subtype == ChartSubtypes2005.Plain)
				{
					newSeries.Type = ChartTypes.Scatter;
					newSeries.Subtype = ChartSubtypes.Plain;
					break;
				}
				newSeries.Type = ChartTypes.Line;
				if (oldChart.Subtype == ChartSubtypes2005.Line)
				{
					newSeries.Subtype = ChartSubtypes.Plain;
				}
				else
				{
					newSeries.Subtype = ChartSubtypes.Smooth;
				}
				break;
			case ChartTypes2005.Bubble:
				newSeries.Type = ChartTypes.Scatter;
				newSeries.Subtype = ChartSubtypes.Bubble;
				break;
			case ChartTypes2005.Doughnut:
				newSeries.Type = ChartTypes.Shape;
				if (oldChart.Subtype == ChartSubtypes2005.OpenHighLowClose)
				{
					newSeries.Subtype = ChartSubtypes.ExplodedDoughnut;
				}
				else
				{
					newSeries.Subtype = ChartSubtypes.Doughnut;
				}
				break;
			case ChartTypes2005.Stock:
				newSeries.Type = ChartTypes.Range;
				if (oldChart.Subtype == ChartSubtypes2005.Candlestick)
				{
					newSeries.Subtype = ChartSubtypes.Candlestick;
				}
				else
				{
					newSeries.Subtype = ChartSubtypes.Stock;
				}
				break;
			}
		}

		private void FixYukonChartBorderWidth(Microsoft.ReportingServices.RdlObjectModel.Style style, bool roundValue)
		{
			if (style == null)
			{
				return;
			}
			if (style.Border == null)
			{
				if (style is EmptyColorStyle)
				{
					style.Border = new EmptyBorder();
				}
				else
				{
					style.Border = new Border();
				}
			}
			if (!style.Border.Width.IsExpression)
			{
				double value = roundValue ? Math.Max(Math.Round(style.Border.Width.Value.Value) * 0.75, 0.25) : Math.Max(style.Border.Width.Value.Value * 0.75, 0.376);
				style.Border.Width = new ReportSize(value, SizeTypes.Point);
			}
		}

		private Microsoft.ReportingServices.RdlObjectModel.Style FixYukonEmptyBorderStyle(Style2005 style2005)
		{
			Microsoft.ReportingServices.RdlObjectModel.Style style2006 = style2005;
			if (style2006 == null)
			{
				style2006 = new Microsoft.ReportingServices.RdlObjectModel.Style();
			}
			if (style2006.Border == null)
			{
				style2006.Border = new Border();
			}
			if (style2006.Border.Style == BorderStyles.Default)
			{
				style2006.Border.Style = BorderStyles.None;
			}
			return style2006;
		}

		internal void UpgradeTextbox(Textbox2005 textbox)
		{
			UpgradeReportItem(textbox);
			textbox.KeepTogether = true;
			RdlCollection<Paragraph> rdlCollection = new RdlCollection<Paragraph>();
			Paragraph paragraph = new Paragraph();
			rdlCollection.Add(paragraph);
			textbox.Paragraphs = rdlCollection;
			RdlCollection<TextRun> rdlCollection2 = new RdlCollection<TextRun>();
			TextRun textRun = new TextRun();
			rdlCollection2.Add(textRun);
			paragraph.TextRuns = rdlCollection2;
			textRun.Value = textbox.Value;
			textRun.PropertyStore.SetObject(3, textbox.ValueLocID);
			Microsoft.ReportingServices.RdlObjectModel.Style style = textbox.Style;
			if (style != null)
			{
				Microsoft.ReportingServices.RdlObjectModel.Style style2 = CreateAndMoveStyleProperties(style, m_TextRunAvailableStyles);
				if (style2 != null)
				{
					textRun.Style = style2;
				}
				style2 = CreateAndMoveStyleProperties(style, m_ParagraphAvailableStyles, convertMeDotValue: true, textbox.Name);
				if (style2 != null)
				{
					paragraph.Style = style2;
				}
			}
			textbox.Value = null;
			textbox.ValueLocID = null;
		}

		private Microsoft.ReportingServices.RdlObjectModel.Style CreateAndMoveStyleProperties(Microsoft.ReportingServices.RdlObjectModel.Style srcStyle, Microsoft.ReportingServices.RdlObjectModel.Style.Definition.Properties[] availableStyles)
		{
			return CreateAndMoveStyleProperties(srcStyle, availableStyles, convertMeDotValue: false, null);
		}

		private Microsoft.ReportingServices.RdlObjectModel.Style CreateAndMoveStyleProperties(Microsoft.ReportingServices.RdlObjectModel.Style srcStyle, Microsoft.ReportingServices.RdlObjectModel.Style.Definition.Properties[] availableStyles, bool convertMeDotValue, string textboxName)
		{
			Microsoft.ReportingServices.RdlObjectModel.Style style = null;
			for (int i = 0; i < availableStyles.Length; i++)
			{
				int propertyIndex = (int)availableStyles[i];
				if (srcStyle.PropertyStore.ContainsObject(propertyIndex))
				{
					if (style == null)
					{
						style = new Microsoft.ReportingServices.RdlObjectModel.Style();
					}
					IExpression expression = (IExpression)srcStyle.PropertyStore.GetObject(propertyIndex);
					if (convertMeDotValue && expression.IsExpression)
					{
						expression.Expression = ConvertMeDotValue(expression.ToString(), textboxName);
					}
					style.PropertyStore.SetObject(propertyIndex, expression);
					switch (availableStyles[i])
					{
					case Microsoft.ReportingServices.RdlObjectModel.Style.Definition.Properties.FontFamily:
						srcStyle.FontFamily = "Arial";
						break;
					case Microsoft.ReportingServices.RdlObjectModel.Style.Definition.Properties.FontSize:
						srcStyle.FontSize = Microsoft.ReportingServices.RdlObjectModel.Constants.DefaultFontSize;
						break;
					case Microsoft.ReportingServices.RdlObjectModel.Style.Definition.Properties.Color:
						srcStyle.Color = Microsoft.ReportingServices.RdlObjectModel.Constants.DefaultColor;
						break;
					case Microsoft.ReportingServices.RdlObjectModel.Style.Definition.Properties.NumeralVariant:
						srcStyle.NumeralVariant = 1;
						break;
					default:
						srcStyle.PropertyStore.RemoveObject(propertyIndex);
						break;
					}
				}
			}
			return style;
		}

		private string ConvertMeDotValue(string expression, string textboxName)
		{
			int num = 0;
			StringBuilder stringBuilder = new StringBuilder();
			MatchCollection matchCollection = m_regexes.MeDotValueExpression.Matches(expression);
			for (int i = 0; i < matchCollection.Count; i++)
			{
				System.Text.RegularExpressions.Group group = matchCollection[i].Groups["medotvalue"];
				if (group.Value != null && group.Value.Length > 0)
				{
					stringBuilder.Append(expression.Substring(num, group.Index - num));
					stringBuilder.Append("ReportItems!");
					stringBuilder.Append(textboxName);
					stringBuilder.Append(".Value");
					num = group.Index + group.Length;
				}
			}
			if (num == 0)
			{
				return expression;
			}
			if (num < expression.Length)
			{
				stringBuilder.Append(expression.Substring(num));
			}
			return stringBuilder.ToString();
		}

		internal void UpgradeQuery(Query2005 query)
		{
			query.DataSourceName = GetDataSourceName(query.DataSourceName);
		}

		internal void UpgradeDataSource(DataSource2005 dataSource)
		{
			if (m_renameInvalidDataSources)
			{
				dataSource.Name = CreateUniqueDataSourceName(dataSource.Name);
			}
		}

		internal void UpgradeSubreport(Subreport2005 subreport)
		{
			UpgradeReportItem(subreport);
			subreport.KeepTogether = true;
		}

		internal void UpgradeStyle(Style2005 style2005)
		{
			((Microsoft.ReportingServices.RdlObjectModel.Style)style2005).FontWeight = UpgradeStyleEnum<FontWeights, FontWeight2005>(style2005.FontWeight);
			if (!style2005.FontWeight.IsExpression)
			{
				switch (style2005.FontWeight.Value.Value)
				{
				case FontWeight2005.Normal:
					((Microsoft.ReportingServices.RdlObjectModel.Style)style2005).FontWeight = FontWeights.Normal;
					break;
				case FontWeight2005.Lighter:
					((Microsoft.ReportingServices.RdlObjectModel.Style)style2005).FontWeight = FontWeights.Light;
					break;
				case FontWeight2005.Bold:
					((Microsoft.ReportingServices.RdlObjectModel.Style)style2005).FontWeight = FontWeights.Bold;
					break;
				case FontWeight2005.Bolder:
					((Microsoft.ReportingServices.RdlObjectModel.Style)style2005).FontWeight = FontWeights.Bold;
					break;
				}
			}
			((Microsoft.ReportingServices.RdlObjectModel.Style)style2005).WritingMode = UpgradeStyleEnum<WritingModes, WritingMode2005>(style2005.WritingMode);
			((Microsoft.ReportingServices.RdlObjectModel.Style)style2005).Calendar = UpgradeStyleEnum<Calendars, Calendar2005>(style2005.Calendar);
			((Microsoft.ReportingServices.RdlObjectModel.Style)style2005).UnicodeBiDi = UpgradeStyleEnum<UnicodeBiDiTypes, UnicodeBiDi2005>(style2005.UnicodeBiDi);
			Border border = null;
			Border border2 = null;
			Border border3 = null;
			Border border4 = null;
			Border border5 = null;
			BorderColor2005 borderColor = style2005.BorderColor;
			if (borderColor != null)
			{
				SetBorderColor(borderColor.Default, ref border, Microsoft.ReportingServices.RdlObjectModel.Constants.DefaultBorderColor);
				SetBorderColor(borderColor.Top, ref border2, ReportColor.Empty);
				SetBorderColor(borderColor.Bottom, ref border3, ReportColor.Empty);
				SetBorderColor(borderColor.Left, ref border4, ReportColor.Empty);
				SetBorderColor(borderColor.Right, ref border5, ReportColor.Empty);
			}
			BorderStyle2005 borderStyle = style2005.BorderStyle;
			if (borderStyle != null)
			{
				SetBorderStyle(borderStyle.Default, ref border, BorderStyles2005.None);
				SetBorderStyle(borderStyle.Top, ref border2, BorderStyles2005.Default);
				SetBorderStyle(borderStyle.Bottom, ref border3, BorderStyles2005.Default);
				SetBorderStyle(borderStyle.Left, ref border4, BorderStyles2005.Default);
				SetBorderStyle(borderStyle.Right, ref border5, BorderStyles2005.Default);
			}
			BorderWidth2005 borderWidth = style2005.BorderWidth;
			if (borderWidth != null)
			{
				SetBorderWidth(borderWidth.Default, ref border, Microsoft.ReportingServices.RdlObjectModel.Constants.DefaultBorderWidth);
				SetBorderWidth(borderWidth.Top, ref border2, ReportSize.Empty);
				SetBorderWidth(borderWidth.Bottom, ref border3, ReportSize.Empty);
				SetBorderWidth(borderWidth.Left, ref border4, ReportSize.Empty);
				SetBorderWidth(borderWidth.Right, ref border5, ReportSize.Empty);
			}
			BackgroundImage2005 backgroundImage = style2005.BackgroundImage;
			if (backgroundImage != null)
			{
				BackgroundImage backgroundImage3 = ((Microsoft.ReportingServices.RdlObjectModel.Style)style2005).BackgroundImage = backgroundImage;
				style2005.BackgroundImage = null;
				if (!backgroundImage.BackgroundRepeat.IsExpression)
				{
					if (style2005.Parent is Chart2005)
					{
						if (backgroundImage.BackgroundRepeat.Value != BackgroundRepeatTypes2005.NoRepeat)
						{
							backgroundImage3.BackgroundRepeat = BackgroundRepeatTypes.Repeat;
						}
					}
					else if (backgroundImage.BackgroundRepeat.Value == BackgroundRepeatTypes2005.NoRepeat)
					{
						backgroundImage3.BackgroundRepeat = BackgroundRepeatTypes.Clip;
					}
					else
					{
						backgroundImage3.BackgroundRepeat = (BackgroundRepeatTypes)backgroundImage.BackgroundRepeat.Value;
					}
				}
				else
				{
					backgroundImage3.BackgroundRepeat = new ReportExpression<BackgroundRepeatTypes>(backgroundImage.BackgroundRepeat.Expression, CultureInfo.InvariantCulture);
				}
			}
			if (border != null)
			{
				style2005.Border = border;
			}
			if (border2 != null)
			{
				style2005.TopBorder = border2;
			}
			if (border3 != null)
			{
				style2005.BottomBorder = border3;
			}
			if (border4 != null)
			{
				style2005.LeftBorder = border4;
			}
			if (border5 != null)
			{
				style2005.RightBorder = border5;
			}
		}

		internal void UpgradeEmptyColorStyle(EmptyColorStyle2005 emptyColorStyle2005)
		{
			if (emptyColorStyle2005.BorderColor == null)
			{
				emptyColorStyle2005.BorderColor = new EmptyBorderColor2005();
			}
			if (emptyColorStyle2005.BorderStyle == null)
			{
				emptyColorStyle2005.BorderStyle = new BorderStyle2005();
			}
			((Microsoft.ReportingServices.RdlObjectModel.Style)emptyColorStyle2005).FontWeight = UpgradeStyleEnum<FontWeights, FontWeight2005>(emptyColorStyle2005.FontWeight);
			if (!emptyColorStyle2005.FontWeight.IsExpression)
			{
				switch (emptyColorStyle2005.FontWeight.Value.Value)
				{
				case FontWeight2005.Normal:
					((Microsoft.ReportingServices.RdlObjectModel.Style)emptyColorStyle2005).FontWeight = FontWeights.Normal;
					break;
				case FontWeight2005.Lighter:
					((Microsoft.ReportingServices.RdlObjectModel.Style)emptyColorStyle2005).FontWeight = FontWeights.Light;
					break;
				case FontWeight2005.Bold:
					((Microsoft.ReportingServices.RdlObjectModel.Style)emptyColorStyle2005).FontWeight = FontWeights.Bold;
					break;
				case FontWeight2005.Bolder:
					((Microsoft.ReportingServices.RdlObjectModel.Style)emptyColorStyle2005).FontWeight = FontWeights.Bold;
					break;
				}
			}
			((Microsoft.ReportingServices.RdlObjectModel.Style)emptyColorStyle2005).WritingMode = UpgradeStyleEnum<WritingModes, WritingMode2005>(emptyColorStyle2005.WritingMode);
			((Microsoft.ReportingServices.RdlObjectModel.Style)emptyColorStyle2005).Calendar = UpgradeStyleEnum<Calendars, Calendar2005>(emptyColorStyle2005.Calendar);
			((Microsoft.ReportingServices.RdlObjectModel.Style)emptyColorStyle2005).UnicodeBiDi = UpgradeStyleEnum<UnicodeBiDiTypes, UnicodeBiDi2005>(emptyColorStyle2005.UnicodeBiDi);
			EmptyBorder border = null;
			EmptyBorder border2 = null;
			EmptyBorder border3 = null;
			EmptyBorder border4 = null;
			EmptyBorder border5 = null;
			EmptyBorderColor2005 borderColor = emptyColorStyle2005.BorderColor;
			if (borderColor != null)
			{
				SetEmptyBorderColor(borderColor.Default, ref border, Microsoft.ReportingServices.RdlObjectModel.Constants.DefaultEmptyColor);
				SetEmptyBorderColor(borderColor.Top, ref border2, ReportColor.Empty);
				SetEmptyBorderColor(borderColor.Bottom, ref border3, ReportColor.Empty);
				SetEmptyBorderColor(borderColor.Left, ref border4, ReportColor.Empty);
				SetEmptyBorderColor(borderColor.Right, ref border5, ReportColor.Empty);
			}
			BorderStyle2005 borderStyle = emptyColorStyle2005.BorderStyle;
			if (borderStyle != null)
			{
				SetEmptyBorderStyle(borderStyle.Default, ref border, BorderStyles2005.Solid);
				SetEmptyBorderStyle(borderStyle.Top, ref border2, BorderStyles2005.Default);
				SetEmptyBorderStyle(borderStyle.Bottom, ref border3, BorderStyles2005.Default);
				SetEmptyBorderStyle(borderStyle.Left, ref border4, BorderStyles2005.Default);
				SetEmptyBorderStyle(borderStyle.Right, ref border5, BorderStyles2005.Default);
			}
			BorderWidth2005 borderWidth = emptyColorStyle2005.BorderWidth;
			if (borderWidth != null)
			{
				SetEmptyBorderWidth(borderWidth.Default, ref border, Microsoft.ReportingServices.RdlObjectModel.Constants.DefaultBorderWidth);
				SetEmptyBorderWidth(borderWidth.Top, ref border2, ReportSize.Empty);
				SetEmptyBorderWidth(borderWidth.Bottom, ref border3, ReportSize.Empty);
				SetEmptyBorderWidth(borderWidth.Left, ref border4, ReportSize.Empty);
				SetEmptyBorderWidth(borderWidth.Right, ref border5, ReportSize.Empty);
			}
			emptyColorStyle2005.Border = border;
			emptyColorStyle2005.TopBorder = border2;
			emptyColorStyle2005.BottomBorder = border3;
			emptyColorStyle2005.LeftBorder = border4;
			emptyColorStyle2005.RightBorder = border5;
		}

		private ReportExpression<T> UpgradeStyleEnum<T, T2005>(ReportExpression<ReportEnum<T2005>> value2005) where T : struct where T2005 : struct, IConvertible
		{
			ReportExpression<T> result = default(ReportExpression<T>);
			if (value2005.IsExpression)
			{
				result.Expression = value2005.Expression;
			}
			else
			{
				result.Value = (T)Enum.ToObject(typeof(T), value2005.Value.Value.ToInt32(null));
			}
			return result;
		}

		private ReportExpression<T> UpgradeStyleEnum<T, T2005>(ReportExpression<T2005> value2005) where T : struct where T2005 : struct, IConvertible
		{
			ReportExpression<T> result = default(ReportExpression<T>);
			if (value2005.IsExpression)
			{
				result.Expression = value2005.Expression;
			}
			else
			{
				result.Value = (T)Enum.ToObject(typeof(T), value2005.Value.ToInt32(null));
			}
			return result;
		}

		private void SetBorderColor(ReportExpression<ReportColor> color, ref Border border, ReportColor defaultColor)
		{
			if (color != defaultColor)
			{
				if (border == null)
				{
					border = new Border();
				}
				border.Color = color;
			}
		}

		private void SetEmptyBorderColor(ReportExpression<ReportColor> color, ref EmptyBorder border, ReportColor defaultColor)
		{
			if (color != defaultColor)
			{
				if (border == null)
				{
					border = new EmptyBorder();
				}
				border.Color = color;
			}
		}

		private void SetBorderStyle(ReportExpression<BorderStyles2005> style, ref Border border, BorderStyles2005 defaultStyle)
		{
			if (!(style != defaultStyle))
			{
				return;
			}
			if (border == null)
			{
				border = new Border();
			}
			border.Style = UpgradeStyleEnum<BorderStyles, BorderStyles2005>(style);
			if (!border.Style.IsExpression)
			{
				BorderStyles2005 value = style.Value;
				if ((uint)(value - 6) <= 4u)
				{
					border.Style = BorderStyles.Solid;
				}
			}
		}

		private void SetEmptyBorderStyle(ReportExpression<BorderStyles2005> style, ref EmptyBorder border, BorderStyles2005 defaultStyle)
		{
			if (!(style != defaultStyle))
			{
				return;
			}
			if (border == null)
			{
				border = new EmptyBorder();
			}
			border.Style = UpgradeStyleEnum<BorderStyles, BorderStyles2005>(style);
			if (!border.Style.IsExpression)
			{
				BorderStyles2005 value = style.Value;
				if ((uint)(value - 6) <= 4u)
				{
					border.Style = BorderStyles.Solid;
				}
			}
		}

		private void SetBorderWidth(ReportExpression<ReportSize> width, ref Border border, ReportSize defaultWidth)
		{
			if (width != defaultWidth)
			{
				if (border == null)
				{
					border = new Border();
				}
				border.Width = width;
			}
		}

		private void SetEmptyBorderWidth(ReportExpression<ReportSize> width, ref EmptyBorder border, ReportSize defaultWidth)
		{
			if (width != defaultWidth)
			{
				if (border == null)
				{
					border = new EmptyBorder();
				}
				border.Width = width;
			}
		}

		private void TransferGroupingCustomProperties(object member, GroupAccessor groupAccessor, CustomPropertiesAccessor propertiesAccessor)
		{
			Grouping2005 grouping = groupAccessor(member) as Grouping2005;
			if (grouping == null)
			{
				return;
			}
			IList<CustomProperty> list = propertiesAccessor(member);
			foreach (CustomProperty customProperty in grouping.CustomProperties)
			{
				list.Add(customProperty);
			}
		}

		private string UniqueName(string baseName, object obj)
		{
			return UniqueName(baseName, obj, allowBaseName: true);
		}

		private string UniqueName(string baseName, object obj, bool allowBaseName)
		{
			string text = baseName;
			int num = (!allowBaseName) ? 1 : 0;
			while (true)
			{
				if (num > 0)
				{
					text = baseName + num;
				}
				if (!m_nameTable.ContainsKey(text))
				{
					break;
				}
				num++;
			}
			m_nameTable.Add(text, obj);
			return text;
		}

		private string CreateUniqueDataSourceName(string oldName)
		{
			string text = oldName;
			if (!ReportRegularExpressions.Value.ClsIdentifierRegex.Match(oldName).Success)
			{
				text = Regex.Replace(oldName, "[^\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}\\p{Mn}\\p{Mc}\\p{Nd}\\p{Pc}\\p{Cf}]", "_");
				if (!ReportRegularExpressions.Value.ClsIdentifierRegex.Match(text).Success)
				{
					text = "AutoGen_" + text;
				}
			}
			string text2 = text.ToUpperInvariant();
			string text3 = text;
			string key = text2;
			int num = 0;
			while (true)
			{
				if (num > 0)
				{
					text3 = text + num;
					key = text2 + num;
				}
				if (!m_dataSourceNameTable.ContainsKey(key))
				{
					break;
				}
				num++;
			}
			m_dataSourceNameTable.Add(key, text3);
			if (!m_dataSourceCaseSensitiveNameTable.ContainsKey(oldName))
			{
				m_dataSourceCaseSensitiveNameTable.Add(oldName, text3);
			}
			return text3;
		}

		private string GetDataSourceName(string dataSourceName)
		{
			if (m_dataSourceCaseSensitiveNameTable.ContainsKey(dataSourceName))
			{
				return (string)m_dataSourceCaseSensitiveNameTable[dataSourceName];
			}
			string key = dataSourceName.ToUpperInvariant();
			if (m_dataSourceNameTable.ContainsKey(key))
			{
				return (string)m_dataSourceNameTable[key];
			}
			return dataSourceName;
		}

		private string GetParentReportItemName(IContainedObject obj)
		{
			IContainedObject containedObject = null;
			for (containedObject = obj.Parent; containedObject != null; containedObject = containedObject.Parent)
			{
				if (containedObject is Microsoft.ReportingServices.RdlObjectModel.ReportItem)
				{
					return ((Microsoft.ReportingServices.RdlObjectModel.ReportItem)containedObject).Name;
				}
			}
			return "";
		}

		private void UpgradeDundasCRIChart(Microsoft.ReportingServices.RdlObjectModel.CustomReportItem cri, Microsoft.ReportingServices.RdlObjectModel.Chart chart)
		{
			OrderedDictionary orderedDictionary = new OrderedDictionary();
			OrderedDictionary orderedDictionary2 = new OrderedDictionary();
			chart.Name = cri.Name;
			chart.ActionInfo = cri.ActionInfo;
			chart.Bookmark = cri.Bookmark;
			chart.DataElementName = cri.DataElementName;
			chart.DataElementOutput = cri.DataElementOutput;
			chart.DocumentMapLabel = cri.DocumentMapLabel;
			chart.PropertyStore.SetObject(12, cri.PropertyStore.GetObject(12));
			chart.Height = cri.Height;
			chart.Left = cri.Left;
			chart.Parent = cri.Parent;
			chart.RepeatWith = cri.RepeatWith;
			chart.ToolTip = cri.ToolTip;
			chart.PropertyStore.SetObject(10, cri.PropertyStore.GetObject(10));
			chart.Top = cri.Top;
			chart.Visibility = cri.Visibility;
			chart.Width = cri.Width;
			chart.ZIndex = cri.ZIndex;
			if (cri.CustomData != null)
			{
				chart.DataSetName = cri.CustomData.DataSetName;
				chart.Filters = cri.CustomData.Filters;
			}
			Hashtable hashtable = new Hashtable();
			List<Hashtable> list = new List<Hashtable>();
			List<Hashtable> list2 = new List<Hashtable>();
			List<Hashtable> list3 = new List<Hashtable>();
			foreach (CustomProperty customProperty in cri.CustomProperties)
			{
				string text = customProperty.Name.Value;
				if (text.StartsWith("expression:", StringComparison.OrdinalIgnoreCase))
				{
					text = text.Substring("expression:".Length);
				}
				if (!AddToPropertyList(list, "Chart.Titles.", text, customProperty.Value) && !AddToPropertyList(list2, "Chart.Legends.", text, customProperty.Value) && !AddToPropertyList(list3, "Chart.ChartAreas.", text, customProperty.Value))
				{
					hashtable.Add(text, customProperty.Value);
				}
				if (text.StartsWith("CHART.ANNOTATIONS.", StringComparison.OrdinalIgnoreCase))
				{
					base.UpgradeResults.HasUnsupportedDundasChartFeatures = true;
				}
			}
			if (hashtable["CUSTOM_CODE_CS"] != null || hashtable["CUSTOM_CODE_VB"] != null || hashtable["CUSTOM_CODE_COMPILED_ASSEMBLY"] != null)
			{
				if (m_throwUpgradeException)
				{
					throw new CRI2005UpgradeException();
				}
				base.UpgradeResults.HasUnsupportedDundasChartFeatures = true;
			}
			StringCollection stringCollection = new StringCollection();
			foreach (Hashtable item in list3)
			{
				ChartArea chartArea = new ChartArea();
				chart.ChartAreas.Add(chartArea);
				string text2 = ConvertDundasCRIStringProperty(item["ChartArea.Name"]);
				string text3 = CreateNewName(stringCollection, text2, "ChartArea");
				stringCollection.Add(text3);
				if (!orderedDictionary.Contains(text2))
				{
					orderedDictionary.Add(text2, text3);
				}
				chartArea.Name = text3;
			}
			int num = 0;
			foreach (Hashtable item2 in list3)
			{
				ChartArea chartArea2 = chart.ChartAreas[num];
				num++;
				chartArea2.AlignWithChartArea = GetNewName(orderedDictionary, ConvertDundasCRIStringProperty(item2["ChartArea.AlignWithChartArea"]));
				chartArea2.AlignOrientation = new ReportExpression<ChartAlignOrientations>(ConvertDundasCRIStringProperty(ChartAlignOrientations.Vertical.ToString(), item2["ChartArea.AlignOrientation"]), CultureInfo.InvariantCulture);
				bool? flag = ConvertDundasCRIBoolProperty(item2["EquallySizedAxesFont"]);
				if (flag.HasValue)
				{
					chartArea2.EquallySizedAxesFont = flag.Value;
				}
				bool? flag2 = ConvertDundasCRIBoolProperty(item2["ChartArea.Visible"]);
				if (flag2.HasValue)
				{
					chartArea2.Hidden = !flag2.Value;
				}
				string text4 = ConvertDundasCRIStringProperty(item2["ChartArea.AlignType"]);
				if (text4 != string.Empty)
				{
					ChartAlignType chartAlignType = new ChartAlignType();
					text4 = " " + text4.Replace(',', ' ') + " ";
					chartAlignType.AxesView = text4.Contains("AxesView");
					chartAlignType.Cursor = text4.Contains("Cursor");
					chartAlignType.InnerPlotPosition = text4.Contains("PlotPosition");
					chartAlignType.Position = text4.Contains("Position");
				}
				chartArea2.Style = ConvertDundasCRIStyleProperty(null, item2["ChartArea.BackColor"], item2["ChartArea.BackGradientType"], item2["ChartArea.BackGradientEndColor"], item2["ChartArea.BackHatchStyle"], item2["ChartArea.ShadowColor"], item2["ChartArea.ShadowOffset"], item2["ChartArea.BorderColor"], item2["ChartArea.BorderStyle"], item2["ChartArea.BorderWidth"], item2["ChartArea.BackImage"], item2["ChartArea.BackImageTranspColor"], item2["ChartArea.BackImageAlign"], item2["ChartArea.BackImageMode"], null, null, null, null, null);
				chartArea2.ChartElementPosition = ConvertDundasCRIChartElementPosition(item2["ChartArea.Position.Y"], item2["ChartArea.Position.X"], item2["ChartArea.Position.Height"], item2["ChartArea.Position.Width"]);
				chartArea2.ChartInnerPlotPosition = ConvertDundasCRIChartElementPosition(item2["ChartArea.InnerPlotPosition.Y"], item2["ChartArea.InnerPlotPosition.X"], item2["ChartArea.InnerPlotPosition.Height"], item2["ChartArea.InnerPlotPosition.Width"]);
				int counter = 0;
				ChartThreeDProperties chartThreeDProperties = new ChartThreeDProperties();
				chartThreeDProperties.Perspective = ConvertDundasCRIIntegerReportExpressionProperty(item2["ChartArea.Area3DStyle.Perspective"], ref counter);
				chartThreeDProperties.Rotation = ConvertDundasCRIIntegerReportExpressionProperty(chartThreeDProperties.Rotation, item2["ChartArea.Area3DStyle.YAngle"], ref counter);
				chartThreeDProperties.Inclination = ConvertDundasCRIIntegerReportExpressionProperty(chartThreeDProperties.Inclination, item2["ChartArea.Area3DStyle.XAngle"], ref counter);
				chartThreeDProperties.DepthRatio = ConvertDundasCRIIntegerReportExpressionProperty(chartThreeDProperties.DepthRatio, item2["ChartArea.Area3DStyle.PointDepth"], ref counter);
				chartThreeDProperties.GapDepth = ConvertDundasCRIIntegerReportExpressionProperty(chartThreeDProperties.GapDepth, item2["ChartArea.Area3DStyle.PointGapDepth"], ref counter);
				chartThreeDProperties.WallThickness = ConvertDundasCRIIntegerReportExpressionProperty(chartThreeDProperties.WallThickness, item2["ChartArea.Area3DStyle.WallWidth"], ref counter);
				bool? flag3 = ConvertDundasCRIBoolProperty(item2["ChartArea.Area3DStyle.Clustered"], ref counter);
				if (flag3.HasValue)
				{
					chartThreeDProperties.Clustered = !flag3.Value;
				}
				else
				{
					chartThreeDProperties.Clustered = true;
				}
				bool? flag4 = ConvertDundasCRIBoolProperty(item2["ChartArea.Area3DStyle.Enable3D"], ref counter);
				if (flag4.HasValue)
				{
					chartThreeDProperties.Enabled = flag4.Value;
				}
				bool? flag5 = ConvertDundasCRIBoolProperty(item2["ChartArea.Area3DStyle.RightAngleAxes"], ref counter);
				if (!flag5.HasValue || flag5.Value)
				{
					chartThreeDProperties.ProjectionMode = ChartProjectionModes.Oblique;
				}
				else
				{
					chartThreeDProperties.ProjectionMode = ChartProjectionModes.Perspective;
				}
				string a = ConvertDundasCRIStringProperty(item2["ChartArea.Area3DStyle.Light"], ref counter);
				if (a == "None")
				{
					chartThreeDProperties.Shading = ChartShadings.None;
				}
				else if (a == "Realistic")
				{
					chartThreeDProperties.Shading = ChartShadings.Real;
				}
				else
				{
					chartThreeDProperties.Shading = ChartShadings.Simple;
				}
				if (counter > 0)
				{
					chartArea2.ChartThreeDProperties = chartThreeDProperties;
				}
				ChartAxis chartAxis = new ChartAxis();
				ChartAxis chartAxis2 = new ChartAxis();
				string text7 = chartAxis.Name = (chartAxis2.Name = "Primary");
				ReportExpression<ChartAxisLocations> reportExpression3 = chartAxis.Location = (chartAxis2.Location = ChartAxisLocations.Default);
				ChartAxis chartAxis3 = new ChartAxis();
				ChartAxis chartAxis4 = new ChartAxis();
				text7 = (chartAxis3.Name = (chartAxis4.Name = "Secondary"));
				reportExpression3 = (chartAxis3.Location = (chartAxis4.Location = ChartAxisLocations.Opposite));
				chartArea2.ChartCategoryAxes.Add(chartAxis);
				chartArea2.ChartCategoryAxes.Add(chartAxis3);
				chartArea2.ChartValueAxes.Add(chartAxis2);
				chartArea2.ChartValueAxes.Add(chartAxis4);
				UpgradeDundasCRIChartAxis(chartAxis, item2, "ChartArea.AxisX.");
				UpgradeDundasCRIChartAxis(chartAxis3, item2, "ChartArea.AxisX2.");
				UpgradeDundasCRIChartAxis(chartAxis2, item2, "ChartArea.AxisY.");
				UpgradeDundasCRIChartAxis(chartAxis4, item2, "ChartArea.AxisY2.");
			}
			chart.ToolTip = ConvertDundasCRIStringProperty(hashtable["Chart.ToolTip"]);
			chart.Style = ConvertDundasCRIStyleProperty(null, ConvertDundasCRIColorProperty(Color.White.Name, hashtable["Chart.BackColor"]), hashtable["Chart.BackGradientType"], hashtable["Chart.BackGradientEndColor"], hashtable["Chart.BackHatchStyle"], null, null, hashtable["Chart.BorderLineColor"], hashtable["Chart.BorderLineStyle"] ?? ((object)BorderStyles.None), hashtable["Chart.BorderLineWidth"], hashtable["Chart.BackImage"], null, null, null, null, null, null, null, null);
			if (cri.Style != null && cri.Style.Language != null)
			{
				if (chart.Style == null)
				{
					chart.Style = new Microsoft.ReportingServices.RdlObjectModel.Style();
				}
				chart.Style.Language = cri.Style.Language;
			}
			string text10 = ConvertDundasCRIStringProperty(hashtable["Chart.Palette"]);
			if (text10 == string.Empty || text10 == "Dundas")
			{
				chart.Palette = ChartPalettes.BrightPastel;
			}
			else if (text10 != "None")
			{
				chart.Palette = new ReportExpression<ChartPalettes>(text10, CultureInfo.InvariantCulture);
			}
			string text11 = ConvertDundasCRIStringProperty(hashtable["Chart.PaletteCustomColors"]);
			if (text11 != string.Empty)
			{
				string[] array = text11.Split(';');
				string[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					string text12 = array2[i].Trim();
					chart.ChartCustomPaletteColors.Add(ConvertDundasCRIColorProperty(Color.Transparent.Name, string.IsNullOrEmpty(text12) ? null : text12.Trim()));
				}
				if (array.Length != 0)
				{
					chart.Palette = new ReportExpression<ChartPalettes>(ChartPalettes.Custom);
				}
			}
			int counter2 = 0;
			ChartBorderSkin chartBorderSkin = new ChartBorderSkin();
			chartBorderSkin.ChartBorderSkinType = new ReportExpression<ChartBorderSkinTypes>(ConvertDundasCRIStringProperty(hashtable["Chart.BorderSkin.SkinStyle"], ref counter2), CultureInfo.InvariantCulture);
			chartBorderSkin.Style = ConvertDundasCRIStyleProperty(ConvertDundasCRIColorProperty(Color.White.Name, hashtable["Chart.BorderSkin.PageColor"]), hashtable["Chart.BorderSkin.FrameBackColor"], hashtable["Chart.BorderSkin.FrameBackGradientType"], hashtable["Chart.BorderSkin.FrameBackGradientEndColor"], hashtable["Chart.BorderSkin.FrameBackHatchStyle"], null, null, hashtable["Chart.BorderSkin.FrameBorderColor"], hashtable["Chart.BorderSkin.FrameBorderStyle"], hashtable["Chart.BorderSkin.FrameBorderWidth"], null, null, null, null, null, null, null, null, null, ref counter2);
			if (counter2 > 0)
			{
				chart.ChartBorderSkin = chartBorderSkin;
			}
			StringCollection stringCollection2 = new StringCollection();
			foreach (Hashtable item3 in list)
			{
				Microsoft.ReportingServices.RdlObjectModel.ChartTitle chartTitle = new Microsoft.ReportingServices.RdlObjectModel.ChartTitle();
				chart.ChartTitles.Add(chartTitle);
				string text13 = CreateNewName(stringCollection2, ConvertDundasCRIStringProperty(item3["Title.Name"]), "Title");
				stringCollection2.Add(text13);
				chartTitle.DockToChartArea = GetNewName(orderedDictionary, ConvertDundasCRIStringProperty(item3["Title.DockToChartArea"]));
				chartTitle.Name = text13;
				UpgradeDundasCRIChartTitle(chartTitle, item3, "Title.");
			}
			Microsoft.ReportingServices.RdlObjectModel.ChartTitle chartTitle2 = new Microsoft.ReportingServices.RdlObjectModel.ChartTitle();
			if (UpgradeDundasCRIChartTitle(chartTitle2, hashtable, "Chart.NoDataMessage."))
			{
				chartTitle2.Name = "NoDataMessageTitle";
				chart.ChartNoDataMessage = chartTitle2;
			}
			StringCollection stringCollection3 = new StringCollection();
			foreach (Hashtable item4 in list2)
			{
				ChartLegend chartLegend = new ChartLegend();
				chart.ChartLegends.Add(chartLegend);
				string text14 = ConvertDundasCRIStringProperty(item4["Legend.Name"]);
				string text15 = CreateNewName(stringCollection3, text14, "Legend");
				stringCollection3.Add(text15);
				if (!orderedDictionary2.Contains(text14))
				{
					orderedDictionary2.Add(text14, text15);
				}
				chartLegend.DockToChartArea = GetNewName(orderedDictionary, ConvertDundasCRIStringProperty(item4["Legend.DockToChartArea"]));
				chartLegend.Name = text15;
				UpgradeDundasCRIChartLegend(chartLegend, item4, "Legend.");
			}
			if (cri.CustomData != null)
			{
				List<Hashtable> list4 = new List<Hashtable>();
				List<Hashtable> list5 = new List<Hashtable>();
				if (cri.CustomData.DataColumnHierarchy != null)
				{
					IList<DataMember> dataMembers = cri.CustomData.DataColumnHierarchy.DataMembers;
					IList<ChartMember> chartMembers = chart.ChartCategoryHierarchy.ChartMembers;
					while (dataMembers != null && dataMembers.Count > 0)
					{
						foreach (DataMember item5 in dataMembers)
						{
							ChartMember chartMember = new ChartMember();
							chartMembers.Add(chartMember);
							chartMember.Group = item5.Group;
							chartMember.SortExpressions = item5.SortExpressions;
							foreach (CustomProperty customProperty2 in item5.CustomProperties)
							{
								if (customProperty2.Name == "GroupLabel")
								{
									chartMember.Label = customProperty2.Value.ToString();
									break;
								}
							}
							dataMembers = item5.DataMembers;
							chartMembers = chartMember.ChartMembers;
						}
					}
				}
				if (cri.CustomData.DataRowHierarchy != null)
				{
					IList<DataMember> dataMembers = cri.CustomData.DataRowHierarchy.DataMembers;
					IList<ChartMember> chartMembers = chart.ChartSeriesHierarchy.ChartMembers;
					while (dataMembers != null)
					{
						bool flag6 = false;
						foreach (DataMember item6 in dataMembers)
						{
							if (item6.DataMembers != null && item6.DataMembers.Count > 0)
							{
								ChartMember chartMember2 = new ChartMember();
								chartMembers.Add(chartMember2);
								chartMember2.Group = item6.Group;
								chartMember2.SortExpressions = item6.SortExpressions;
								foreach (CustomProperty customProperty3 in item6.CustomProperties)
								{
									if (customProperty3.Name == "GroupLabel")
									{
										chartMember2.Label = customProperty3.Value.ToString();
										break;
									}
								}
								dataMembers = item6.DataMembers;
								chartMembers = chartMember2.ChartMembers;
								continue;
							}
							flag6 = true;
							Hashtable hashtable2 = new Hashtable(item6.CustomProperties.Count);
							if (item6.CustomProperties != null)
							{
								foreach (CustomProperty customProperty4 in item6.CustomProperties)
								{
									hashtable2.Add(customProperty4.Name, customProperty4.Value);
								}
								list4.Add(hashtable2);
							}
							ChartMember chartMember3 = new ChartMember();
							chartMember3.Label = ConvertDundasCRIStringProperty(hashtable2["SeriesLabel"]);
							chartMembers.Add(chartMember3);
						}
						if (flag6)
						{
							dataMembers = null;
						}
					}
					if (cri.CustomData.DataRows != null)
					{
						foreach (IList<IList<Microsoft.ReportingServices.RdlObjectModel.DataValue>> dataRow in cri.CustomData.DataRows)
						{
							foreach (IList<Microsoft.ReportingServices.RdlObjectModel.DataValue> item7 in dataRow)
							{
								Hashtable hashtable3 = new Hashtable(item7.Count);
								foreach (Microsoft.ReportingServices.RdlObjectModel.DataValue item8 in item7)
								{
									if (item8.Name.Value.StartsWith("CUSTOMVALUE:", StringComparison.OrdinalIgnoreCase))
									{
										if (m_throwUpgradeException)
										{
											throw new CRI2005UpgradeException();
										}
										base.UpgradeResults.HasUnsupportedDundasChartFeatures = true;
									}
									hashtable3.Add(item8.Name, item8.Value);
								}
								list5.Add(hashtable3);
							}
						}
					}
					if (chart.ChartData == null)
					{
						chart.ChartData = new ChartData();
					}
					foreach (Hashtable item9 in list4)
					{
						ChartSeries chartSeries = new ChartSeries();
						chart.ChartData.ChartSeriesCollection.Add(chartSeries);
						chartSeries.Name = "Series" + chart.ChartData.ChartSeriesCollection.Count.ToString(CultureInfo.InvariantCulture.NumberFormat);
						chartSeries.ChartAreaName = GetNewName(orderedDictionary, ConvertDundasCRIStringProperty(item9["ChartArea"]));
						chartSeries.LegendName = GetNewName(orderedDictionary2, ConvertDundasCRIStringProperty(item9["Legend"]));
						UpgradeDundasCRIChartSeries(chartSeries, chart.ChartData.ChartDerivedSeriesCollection, item9, list5);
					}
				}
			}
			else
			{
				chart.ChartCategoryHierarchy.ChartMembers.Add(new ChartMember());
				chart.ChartSeriesHierarchy.ChartMembers.Add(new ChartMember());
				chart.ChartData = new ChartData();
				ChartSeries chartSeries2 = new ChartSeries();
				chartSeries2.Name = "emptySeriesName";
				chartSeries2.ChartDataPoints.Add(new Microsoft.ReportingServices.RdlObjectModel.ChartDataPoint());
				chart.ChartData.ChartSeriesCollection.Add(chartSeries2);
			}
			FixChartAxisStriplineTitleAngle(chart);
		}

		private void UpgradeDundasCRIChartAxis(ChartAxis axis, Hashtable axisProperties, string propertyPrefix)
		{
			axis.Visible = new ReportExpression<ChartVisibleTypes>(ConvertDundasCRIStringProperty(axisProperties[propertyPrefix + "Enabled"]), CultureInfo.InvariantCulture);
			axis.Interval = ConvertDundasCRIDoubleReportExpressionProperty(axisProperties[propertyPrefix + "Interval"]);
			axis.IntervalType = new ReportExpression<ChartIntervalTypes>(ConvertDundasCRIStringProperty(ChartIntervalTypes.Auto.ToString(), axisProperties[propertyPrefix + "IntervalType"]), CultureInfo.InvariantCulture);
			axis.IntervalOffset = ConvertDundasCRIDoubleReportExpressionProperty(axisProperties[propertyPrefix + "IntervalOffset"]);
			axis.IntervalOffsetType = new ReportExpression<ChartIntervalOffsetTypes>(ConvertDundasCRIStringProperty(ChartIntervalOffsetTypes.Auto.ToString(), axisProperties[propertyPrefix + "IntervalOffsetType"]), CultureInfo.InvariantCulture);
			axis.CrossAt = ConvertDundasCRIStringProperty(axisProperties[propertyPrefix + "Crossing"]);
			axis.Arrows = new ReportExpression<ChartArrowsTypes>(ConvertDundasCRIStringProperty(axisProperties[propertyPrefix + "Arrows"]), CultureInfo.InvariantCulture);
			axis.Minimum = ConvertDundasCRIStringProperty(axisProperties[propertyPrefix + "Minimum"]);
			axis.Maximum = ConvertDundasCRIStringProperty(axisProperties[propertyPrefix + "Maximum"]);
			axis.LogBase = ConvertDundasCRIDoubleReportExpressionProperty(axis.LogBase, axisProperties[propertyPrefix + "LogarithmBase"]);
			axis.Angle = ConvertDundasCRIDoubleReportExpressionProperty(axisProperties[propertyPrefix + "LabelStyle.FontAngle"]);
			axis.LabelInterval = ConvertDundasCRIDoubleReportExpressionProperty(axisProperties[propertyPrefix + "LabelStyle.Interval"]);
			axis.LabelIntervalType = new ReportExpression<ChartIntervalTypes>(ConvertDundasCRIStringProperty(axisProperties[propertyPrefix + "LabelStyle.IntervalType"]), CultureInfo.InvariantCulture);
			axis.LabelIntervalOffset = ConvertDundasCRIDoubleReportExpressionProperty(axisProperties[propertyPrefix + "LabelStyle.IntervalOffset"]);
			axis.LabelIntervalOffsetType = new ReportExpression<ChartIntervalOffsetTypes>(ConvertDundasCRIStringProperty(axisProperties[propertyPrefix + "LabelStyle.IntervalOffsetType"]), CultureInfo.InvariantCulture);
			axis.MinFontSize = ConvertDundasCRIPointReportSizeProperty(axis.MinFontSize, axisProperties[propertyPrefix + "LabelsAutoFitMinFontSize"]);
			axis.MaxFontSize = ConvertDundasCRIPointReportSizeProperty(axis.MaxFontSize, axisProperties[propertyPrefix + "LabelsAutoFitMaxFontSize"]);
			axis.InterlacedColor = ConvertDundasCRIColorProperty(axis.InterlacedColor, axisProperties[propertyPrefix + "InterlacedColor"]);
			bool? flag = ConvertDundasCRIBoolProperty(axisProperties[propertyPrefix + "Reverse"]);
			if (flag.HasValue)
			{
				axis.Reverse = flag.Value;
			}
			bool? flag2 = ConvertDundasCRIBoolProperty(axisProperties[propertyPrefix + "Interlaced"]);
			if (flag2.HasValue)
			{
				axis.Interlaced = flag2.Value;
			}
			bool? flag3 = ConvertDundasCRIBoolProperty(axisProperties[propertyPrefix + "Logarithmic"]);
			if (flag3.HasValue)
			{
				axis.LogScale = flag3.Value;
			}
			bool? flag4 = ConvertDundasCRIBoolProperty(axisProperties[propertyPrefix + "LabelStyle.Enabled"]);
			if (flag4.HasValue)
			{
				axis.HideLabels = !flag4.Value;
			}
			bool? flag5 = ConvertDundasCRIBoolProperty(axisProperties[propertyPrefix + "StartFromZero"]);
			if (flag5.HasValue)
			{
				axis.IncludeZero = flag5.Value;
			}
			bool? flag6 = ConvertDundasCRIBoolProperty(axisProperties[propertyPrefix + "LabelsAutoFit"]);
			if (flag6.HasValue)
			{
				axis.LabelsAutoFitDisabled = !flag6.Value;
			}
			bool? flag7 = ConvertDundasCRIBoolProperty(axisProperties[propertyPrefix + "LabelStyle.OffsetLabels"]);
			if (flag7.HasValue)
			{
				axis.OffsetLabels = flag7.Value;
			}
			bool? flag8 = ConvertDundasCRIBoolProperty(axisProperties[propertyPrefix + "LabelStyle.ShowEndLabels"]);
			if (flag8.HasValue)
			{
				axis.HideEndLabels = !flag8.Value;
			}
			string text = ConvertDundasCRIStringProperty(axisProperties[propertyPrefix + "LabelsAutoFitStyle"]);
			axis.PreventFontGrow = (!(text == string.Empty) && !text.Contains("IncreaseFont"));
			axis.PreventFontShrink = (!(text == string.Empty) && !text.Contains("DecreaseFont"));
			axis.PreventLabelOffset = (!(text == string.Empty) && !text.Contains("OffsetLabels"));
			axis.PreventWordWrap = (!(text == string.Empty) && !text.Contains("WordWrap"));
			if (text == string.Empty || text.Contains("LabelsAngleStep30"))
			{
				axis.AllowLabelRotation = ChartLabelRotationTypes.Rotate30;
			}
			else if (text.Contains("LabelsAngleStep45"))
			{
				axis.AllowLabelRotation = ChartLabelRotationTypes.Rotate45;
			}
			else if (text.Contains("LabelsAngleStep90"))
			{
				axis.AllowLabelRotation = ChartLabelRotationTypes.Rotate90;
			}
			bool? flag9 = ConvertDundasCRIBoolProperty(axisProperties[propertyPrefix + "MarksNextToAxis"]);
			if (flag9.HasValue)
			{
				axis.MarksAlwaysAtPlotEdge = !flag9.Value;
			}
			bool? flag10 = ConvertDundasCRIBoolProperty(axisProperties[propertyPrefix + "Margin"]);
			if (flag10.HasValue && !flag10.Value)
			{
				axis.Margin = ChartAxisMarginVisibleTypes.False;
			}
			else
			{
				axis.Margin = ChartAxisMarginVisibleTypes.True;
			}
			axis.Style = ConvertDundasCRIStyleProperty(axisProperties[propertyPrefix + "LabelStyle.FontColor"], null, null, null, null, null, null, axisProperties[propertyPrefix + "LineColor"], axisProperties[propertyPrefix + "LineStyle"], axisProperties[propertyPrefix + "LineWidth"], null, null, null, null, ConvertDundasCRIStringProperty("Microsoft Sans Serif, 8pt", axisProperties[propertyPrefix + "LabelStyle.Font"]), axisProperties[propertyPrefix + "LabelStyle.Format"], null, null, null);
			int counter = 0;
			ChartAxisTitle chartAxisTitle = new ChartAxisTitle();
			chartAxisTitle.Caption = ConvertDundasCRIStringProperty(axisProperties[propertyPrefix + "Title"], ref counter);
			chartAxisTitle.Position = new ReportExpression<ChartAxisTitlePositions>(ConvertDundasCRIStringProperty(axisProperties[propertyPrefix + "TitleAlignment"], ref counter), CultureInfo.InvariantCulture);
			chartAxisTitle.Style = ConvertDundasCRIStyleProperty(axisProperties[propertyPrefix + "TitleColor"], null, null, null, null, null, null, null, null, null, null, null, null, null, ConvertDundasCRIStringProperty("Microsoft Sans Serif, 8pt", axisProperties[propertyPrefix + "TitleFont"]), null, null, null, null, ref counter);
			if (counter > 0)
			{
				axis.ChartAxisTitle = chartAxisTitle;
			}
			counter = 0;
			ChartAxisScaleBreak chartAxisScaleBreak = new ChartAxisScaleBreak();
			bool? flag11 = ConvertDundasCRIBoolProperty(axisProperties[propertyPrefix + "ScaleBreakStyle.Enabled"], ref counter);
			if (flag11.HasValue)
			{
				chartAxisScaleBreak.Enabled = flag11.Value;
			}
			chartAxisScaleBreak.BreakLineType = new ReportExpression<ChartBreakLineTypes>(ConvertDundasCRIStringProperty(axisProperties[propertyPrefix + "ScaleBreakStyle.BreakLineType"], ref counter), CultureInfo.InvariantCulture);
			chartAxisScaleBreak.CollapsibleSpaceThreshold = ConvertDundasCRIIntegerReportExpressionProperty(chartAxisScaleBreak.CollapsibleSpaceThreshold, axisProperties[propertyPrefix + "ScaleBreakStyle.CollapsibleSpaceThreshold"], ref counter);
			chartAxisScaleBreak.MaxNumberOfBreaks = ConvertDundasCRIIntegerReportExpressionProperty(chartAxisScaleBreak.MaxNumberOfBreaks, axisProperties[propertyPrefix + "ScaleBreakStyle.MaxNumberOfBreaks"], ref counter);
			chartAxisScaleBreak.Spacing = ConvertDundasCRIDoubleReportExpressionProperty(chartAxisScaleBreak.Spacing, axisProperties[propertyPrefix + "ScaleBreakStyle.Spacing"], ref counter);
			chartAxisScaleBreak.IncludeZero = new ReportExpression<ChartIncludeZeroTypes>(ConvertDundasCRIStringProperty(axisProperties[propertyPrefix + "ScaleBreakStyle.StartFromZero"], ref counter), CultureInfo.InvariantCulture);
			chartAxisScaleBreak.Style = ConvertDundasCRIStyleProperty(null, null, null, null, null, null, null, axisProperties[propertyPrefix + "ScaleBreakStyle.LineColor"], axisProperties[propertyPrefix + "ScaleBreakStyle.LineStyle"], axisProperties[propertyPrefix + "ScaleBreakStyle.LineWidth"], null, null, null, null, null, null, null, null, null, ref counter);
			if (counter > 0)
			{
				axis.ChartAxisScaleBreak = chartAxisScaleBreak;
			}
			ChartTickMarks chartTickMarks = new ChartTickMarks();
			if (UpgradeDundasCRIChartTickMarks(chartTickMarks, axisProperties, propertyPrefix + "MajorTickMark.", isMajor: true))
			{
				axis.ChartMajorTickMarks = chartTickMarks;
			}
			ChartTickMarks chartTickMarks2 = new ChartTickMarks();
			if (UpgradeDundasCRIChartTickMarks(chartTickMarks2, axisProperties, propertyPrefix + "MinorTickMark.", isMajor: false))
			{
				axis.ChartMinorTickMarks = chartTickMarks2;
			}
			ChartGridLines chartGridLines = new ChartGridLines();
			if (UpgradeDundasCRIChartGridLines(chartGridLines, axisProperties, propertyPrefix + "MajorGrid.", isMajor: true))
			{
				axis.ChartMajorGridLines = chartGridLines;
			}
			ChartGridLines chartGridLines2 = new ChartGridLines();
			if (UpgradeDundasCRIChartGridLines(chartGridLines2, axisProperties, propertyPrefix + "MinorGrid.", isMajor: false))
			{
				axis.ChartMinorGridLines = chartGridLines2;
			}
			List<Hashtable> list = new List<Hashtable>();
			foreach (DictionaryEntry axisProperty in axisProperties)
			{
				AddToPropertyList(list, propertyPrefix + "StripLines.", axisProperty.Key.ToString(), axisProperty.Value.ToString());
			}
			foreach (Hashtable item in list)
			{
				ChartStripLine chartStripLine = new ChartStripLine();
				chartStripLine.Title = ConvertDundasCRIStringProperty(item["StripLine.Title"]);
				chartStripLine.Interval = ConvertDundasCRIDoubleReportExpressionProperty(item["StripLine.Interval"]);
				chartStripLine.IntervalType = new ReportExpression<ChartIntervalTypes>(ConvertDundasCRIStringProperty(ChartIntervalTypes.Auto.ToString(), item["StripLine.IntervalType"]), CultureInfo.InvariantCulture);
				chartStripLine.IntervalOffset = ConvertDundasCRIDoubleReportExpressionProperty(item["StripLine.IntervalOffset"]);
				chartStripLine.IntervalOffsetType = new ReportExpression<ChartIntervalOffsetTypes>(ConvertDundasCRIStringProperty(ChartIntervalTypes.Auto.ToString(), item["StripLine.IntervalOffsetType"]), CultureInfo.InvariantCulture);
				chartStripLine.StripWidth = ConvertDundasCRIDoubleReportExpressionProperty(item["StripLine.StripWidth"]);
				chartStripLine.StripWidthType = new ReportExpression<ChartStripWidthTypes>(ConvertDundasCRIStringProperty(ChartIntervalTypes.Auto.ToString(), item["StripLine.StripWidthType"]), CultureInfo.InvariantCulture);
				switch (ConvertDundasCRIStringProperty(item["StripLine.TitleAngle"]))
				{
				case "90":
					chartStripLine.TextOrientation = TextOrientations.Rotated90;
					break;
				case "180":
					chartStripLine.TextOrientation = TextOrientations.Stacked;
					break;
				case "270":
					chartStripLine.TextOrientation = TextOrientations.Rotated270;
					break;
				default:
					chartStripLine.TextOrientation = TextOrientations.Horizontal;
					break;
				}
				string text2 = ConvertDundasCRIStringProperty(item["StripLine.Href"]);
				if (text2 != string.Empty)
				{
					chartStripLine.ActionInfo = new ActionInfo();
					Microsoft.ReportingServices.RdlObjectModel.Action action = new Microsoft.ReportingServices.RdlObjectModel.Action();
					action.Hyperlink = text2;
					chartStripLine.ActionInfo.Actions.Add(action);
				}
				Microsoft.ReportingServices.RdlObjectModel.Style style = new Microsoft.ReportingServices.RdlObjectModel.Style();
				int counter2 = 0;
				string a = ConvertDundasCRIStringProperty(item["StripLine.TitleAlignment"], ref counter2);
				int counter3 = 0;
				string a2 = ConvertDundasCRIStringProperty(item["StripLine.TitleLineAlignment"], ref counter3);
				style = (chartStripLine.Style = ConvertDundasCRIStyleProperty(item["StripLine.TitleColor"], item["StripLine.BackColor"], item["StripLine.BackGradientType"], item["StripLine.BackGradientEndColor"], item["StripLine.BackHatchStyle"], null, null, item["StripLine.BorderColor"], item["StripLine.BorderStyle"], item["StripLine.BorderWidth"], item["StripLine.BackImage"], item["StripLine.BackImageTranspColor"], item["StripLine.BackImageAlign"], item["StripLine.BackImageMode"], item["StripLine.TitleFont"] ?? "Microsoft Sans Serif, 8pt", null, null, (a == "Near") ? TextAlignments.Left.ToString() : ((a == "Center") ? TextAlignments.Center.ToString() : TextAlignments.Right.ToString()), (counter3 == 0) ? null : ((a2 == "Center") ? VerticalAlignments.Middle.ToString() : ((a2 == "Far") ? VerticalAlignments.Bottom.ToString() : VerticalAlignments.Top.ToString()))));
				axis.ChartStripLines.Add(chartStripLine);
			}
		}

		private void UpgradeDundasCRIChartSeries(ChartSeries series, IList<ChartDerivedSeries> derivedSeriesCollection, Hashtable seriesProperties, List<Hashtable> dataPointCustomProperties)
		{
			string text = ConvertDundasCRIStringProperty(seriesProperties["Type"]);
			SetChartSeriesType(series, text);
			ConvertDundasCRICustomProperties(series.CustomProperties, seriesProperties["CustomAttributes"]);
			ReportExpression reportExpression = null;
			foreach (CustomProperty customProperty2 in series.CustomProperties)
			{
				switch (customProperty2.Name.Value)
				{
				case "ShowPieAsCollected":
					customProperty2.Name = "CollectedStyle";
					customProperty2.Value = "CollectedPie";
					break;
				case "CollectedPercentage":
					customProperty2.Name = "CollectedThreshold";
					break;
				case "CollectedSliceLabel":
					customProperty2.Name = "CollectedLabel";
					reportExpression = customProperty2.Value;
					break;
				case "CollectedSliceColor":
					customProperty2.Name = "CollectedColor";
					break;
				case "ShowCollectedLegend":
					customProperty2.Name = "CollectedChartShowLegend";
					break;
				case "ShowCollectedPointLabels":
					customProperty2.Name = "CollectedChartShowLabels";
					break;
				}
			}
			if (reportExpression != null)
			{
				CustomProperty customProperty = new CustomProperty();
				customProperty.Name = "CollectedLegendText";
				customProperty.Value = reportExpression;
				series.CustomProperties.Add(customProperty);
			}
			series.ValueAxisName = ConvertDundasCRIStringProperty("Primary", seriesProperties["YAxisType"]);
			series.CategoryAxisName = ConvertDundasCRIStringProperty("Primary", seriesProperties["XAxisType"]);
			series.Style.ShadowOffset = ConvertDundasCRIPixelReportSizeProperty(seriesProperties["ShadowOffset"]);
			int counter = 0;
			ChartItemInLegend chartItemInLegend = new ChartItemInLegend();
			chartItemInLegend.LegendText = ConvertDundasCRIStringProperty(seriesProperties["LegendText"], ref counter);
			bool? flag = ConvertDundasCRIBoolProperty(seriesProperties["ShowInLegend"], ref counter);
			if (flag.HasValue)
			{
				chartItemInLegend.Hidden = !flag.Value;
			}
			if (counter > 0)
			{
				series.ChartItemInLegend = chartItemInLegend;
			}
			counter = 0;
			ChartSmartLabel chartSmartLabel = new ChartSmartLabel();
			chartSmartLabel.CalloutBackColor = ConvertDundasCRIColorProperty(chartSmartLabel.CalloutBackColor, seriesProperties["SmartLabels.CalloutBackColor"], ref counter);
			chartSmartLabel.CalloutLineAnchor = new ReportExpression<ChartCalloutLineAnchorTypes>(ConvertDundasCRIStringProperty(seriesProperties["SmartLabels.CalloutLineAnchorCap"], ref counter), CultureInfo.InvariantCulture);
			chartSmartLabel.CalloutLineColor = ConvertDundasCRIColorProperty(chartSmartLabel.CalloutLineColor, seriesProperties["SmartLabels.CalloutLineColor"], ref counter);
			chartSmartLabel.CalloutLineStyle = new ReportExpression<ChartCalloutLineStyles>(ConvertDundasCRIStringProperty(seriesProperties["SmartLabels.CalloutLineStyle"], ref counter), CultureInfo.InvariantCulture);
			chartSmartLabel.CalloutLineWidth = ConvertDundasCRIPixelReportSizeProperty(seriesProperties["SmartLabels.CalloutLineWidth"], ref counter);
			chartSmartLabel.MaxMovingDistance = ConvertDundasCRIPixelReportSizeProperty(30.0, seriesProperties["SmartLabels.MaxMovingDistance"], ref counter);
			chartSmartLabel.MinMovingDistance = ConvertDundasCRIPixelReportSizeProperty(seriesProperties["SmartLabels.MinMovingDistance"], ref counter);
			string a = ConvertDundasCRIStringProperty(seriesProperties["SmartLabels.AllowOutsidePlotArea"], ref counter);
			if (a == "Yes")
			{
				chartSmartLabel.AllowOutSidePlotArea = ChartAllowOutSidePlotAreaTypes.True;
			}
			else if (a == "No")
			{
				chartSmartLabel.AllowOutSidePlotArea = ChartAllowOutSidePlotAreaTypes.False;
			}
			else
			{
				chartSmartLabel.AllowOutSidePlotArea = ChartAllowOutSidePlotAreaTypes.Partial;
			}
			string text2 = ConvertDundasCRIStringProperty(seriesProperties["SmartLabels.CalloutStyle"], ref counter);
			chartSmartLabel.CalloutStyle = new ReportExpression<ChartCalloutStyles>((!(text2 == "Underlined")) ? text2 : (text2 = ChartCalloutStyles.Underline.ToString()), CultureInfo.InvariantCulture);
			bool? flag2 = ConvertDundasCRIBoolProperty(seriesProperties["SmartLabels.Enabled"], ref counter);
			if (flag2.HasValue)
			{
				chartSmartLabel.Disabled = !flag2.Value;
			}
			bool? flag3 = ConvertDundasCRIBoolProperty(seriesProperties["SmartLabels.HideOverlapped"], ref counter);
			if (flag3.HasValue)
			{
				chartSmartLabel.ShowOverlapped = !flag3.Value;
			}
			bool? flag4 = ConvertDundasCRIBoolProperty(seriesProperties["SmartLabels.MarkerOverlapping"], ref counter);
			if (flag4.HasValue)
			{
				chartSmartLabel.MarkerOverlapping = flag4.Value;
			}
			string text3 = ConvertDundasCRIStringProperty(seriesProperties["SmartLabels.MovingDirection"], ref counter);
			if (text3 != string.Empty)
			{
				ChartNoMoveDirections chartNoMoveDirections2 = chartSmartLabel.ChartNoMoveDirections = new ChartNoMoveDirections();
				text3 = " " + text3.Replace(',', ' ') + " ";
				chartNoMoveDirections2.Down = !text3.Contains(" Bottom ");
				chartNoMoveDirections2.DownLeft = !text3.Contains(" BottomLeft ");
				chartNoMoveDirections2.DownRight = !text3.Contains(" BottomRight ");
				chartNoMoveDirections2.Left = !text3.Contains(" Left ");
				chartNoMoveDirections2.Right = !text3.Contains(" Right ");
				chartNoMoveDirections2.Up = !text3.Contains(" Top ");
				chartNoMoveDirections2.UpLeft = !text3.Contains(" TopLeft ");
				chartNoMoveDirections2.UpRight = !text3.Contains(" TopRight ");
			}
			if (counter > 0)
			{
				series.ChartSmartLabel = chartSmartLabel;
			}
			counter = 0;
			ChartEmptyPoints chartEmptyPoints = new ChartEmptyPoints();
			ConvertDundasCRICustomProperties(chartEmptyPoints.CustomProperties, seriesProperties["EmptyPointStyle.CustomAttributes"], ref counter);
			chartEmptyPoints.AxisLabel = ConvertDundasCRIStringProperty(seriesProperties["EmptyPointStyle.AxisLabel"], ref counter);
			chartEmptyPoints.ActionInfo = ConvertDundasCRIActionInfoProperty(seriesProperties["EmptyPointStyle.Href"], ref counter);
			chartEmptyPoints.Style = ConvertDundasCRIEmptyColorStyleProperty(seriesProperties["EmptyPointStyle.Color"], null, seriesProperties["EmptyPointStyle.BackGradientType"], seriesProperties["EmptyPointStyle.BackGradientEndColor"], seriesProperties["EmptyPointStyle.BackHatchStyle"], seriesProperties["EmptyPointStyle.BorderColor"], seriesProperties["EmptyPointStyle.BorderStyle"], seriesProperties["EmptyPointStyle.BorderWidth"], seriesProperties["EmptyPointStyle.BackImage"], seriesProperties["EmptyPointStyle.BackImageTranspColor"], seriesProperties["EmptyPointStyle.BackImageAlign"], seriesProperties["EmptyPointStyle.BackImageMode"], null, null, ref counter);
			int counter2 = 0;
			ChartMarker chartMarker = new ChartMarker();
			chartMarker.Style = ConvertDundasCRIEmptyColorStyleProperty(seriesProperties["EmptyPointStyle.MarkerColor"], null, null, null, null, seriesProperties["EmptyPointStyle.MarkerBorderColor"], null, seriesProperties["EmptyPointStyle.MarkerBorderWidth"], seriesProperties["EmptyPointStyle.MarkerImage"], seriesProperties["EmptyPointStyle.MarkerImageTranspColor"], seriesProperties["EmptyPointStyle.MarkerImageAlign"], seriesProperties["EmptyPointStyle.MarkerImageMode"], null, null, ref counter2);
			chartMarker.Type = new ReportExpression<ChartMarkerTypes>(ConvertDundasCRIStringProperty(seriesProperties["EmptyPointStyle.MarkerStyle"], ref counter2), CultureInfo.InvariantCulture);
			chartMarker.Size = ConvertDundasCRIPixelReportSizeProperty(seriesProperties["EmptyPointStyle.MarkerSize"], ref counter2);
			if (counter2 > 0)
			{
				chartEmptyPoints.ChartMarker = chartMarker;
				counter++;
			}
			int counter3 = 0;
			Microsoft.ReportingServices.RdlObjectModel.ChartDataLabel chartDataLabel = new Microsoft.ReportingServices.RdlObjectModel.ChartDataLabel();
			string value = ConvertDundasCRIStringProperty(seriesProperties["EmptyPointStyle.Label"], ref counter3);
			chartDataLabel.Label = value;
			chartDataLabel.Visible = !string.IsNullOrEmpty(value);
			chartDataLabel.Rotation = ConvertDundasCRIIntegerReportExpressionProperty(seriesProperties["EmptyPointStyle.FontAngle"], ref counter3);
			chartDataLabel.ActionInfo = ConvertDundasCRIActionInfoProperty(seriesProperties["ChartEmptyPointstyle.LabelHref"], ref counter3);
			chartDataLabel.Style = ConvertDundasCRIEmptyColorStyleProperty(seriesProperties["EmptyPointStyle.FontColor"], seriesProperties["EmptyPointStyle.LabelBackColor"], null, null, null, seriesProperties["EmptyPointStyle.LabelBorderColor"], seriesProperties["EmptyPointStyle.LabelBorderStyle"], seriesProperties["EmptyPointStyle.LabelBorderWidth"], null, null, null, null, seriesProperties["EmptyPointStyle.Font"] ?? "Microsoft Sans Serif, 8pt", null, ref counter2);
			if (counter3 > 0)
			{
				chartEmptyPoints.ChartDataLabel = chartDataLabel;
				counter++;
			}
			if (counter > 0)
			{
				series.ChartEmptyPoints = chartEmptyPoints;
			}
			foreach (DictionaryEntry seriesProperty in seriesProperties)
			{
				string text4 = seriesProperty.Key.ToString();
				if (text4.Equals("ERRORFORMULA:BOXPLOT", StringComparison.OrdinalIgnoreCase) || text4.Equals("FINANCIALFORMULA:FORECASTING", StringComparison.OrdinalIgnoreCase))
				{
					if (m_throwUpgradeException)
					{
						throw new CRI2005UpgradeException();
					}
					base.UpgradeResults.HasUnsupportedDundasChartFeatures = true;
				}
				if (!text4.StartsWith("FINANCIALFORMULA:", StringComparison.OrdinalIgnoreCase) && !text4.StartsWith("STATISTICALFORMULA:", StringComparison.OrdinalIgnoreCase))
				{
					continue;
				}
				ChartDerivedSeries chartDerivedSeries = new ChartDerivedSeries();
				chartDerivedSeries.SourceChartSeriesName = series.Name;
				string text6 = chartDerivedSeries.ChartSeries.Name = series.Name + "_Formula";
				string str = text6;
				int num = 1;
				bool flag5 = false;
				do
				{
					flag5 = false;
					foreach (ChartDerivedSeries item in derivedSeriesCollection)
					{
						if (item.ChartSeries.Name == str + ((num > 1) ? num.ToString(CultureInfo.InvariantCulture) : string.Empty))
						{
							flag5 = true;
							num++;
							break;
						}
					}
				}
				while (flag5);
				if (num > 1)
				{
					chartDerivedSeries.ChartSeries.Name += num.ToString(CultureInfo.InvariantCulture);
				}
				string value2 = text4.Substring(text4.IndexOf(':') + 1);
				try
				{
					chartDerivedSeries.DerivedSeriesFormula = (ChartFormulas)Enum.Parse(typeof(ChartFormulas), value2);
				}
				catch
				{
					goto IL_0cdc;
				}
				string[] array = seriesProperty.Value.ToString().Split(';');
				for (int i = 0; i < array.Length; i++)
				{
					string[] array2 = array[i].Split('=');
					string text7 = (array2.Length != 0) ? array2[0].ToUpperInvariant().Trim() : string.Empty;
					string text8 = (array2.Length > 1) ? array2[1] : string.Empty;
					switch (text7)
					{
					case "SERIESTYPE":
						SetChartSeriesType(chartDerivedSeries.ChartSeries, text8);
						break;
					case "SHOWLEGEND":
					{
						if (bool.TryParse(text8, out bool result2))
						{
							if (chartDerivedSeries.ChartSeries.ChartItemInLegend == null)
							{
								chartDerivedSeries.ChartSeries.ChartItemInLegend = new ChartItemInLegend();
							}
							chartDerivedSeries.ChartSeries.ChartItemInLegend.Hidden = !result2;
						}
						break;
					}
					case "LEGENDTEXT":
						if (chartDerivedSeries.ChartSeries.ChartItemInLegend == null)
						{
							chartDerivedSeries.ChartSeries.ChartItemInLegend = new ChartItemInLegend();
						}
						chartDerivedSeries.ChartSeries.ChartItemInLegend.LegendText = text8.Replace("_x003B_", ";").Replace("_x003D_", "=");
						break;
					case "FORMULAPARAMETERS":
					{
						ChartFormulaParameter chartFormulaParameter4 = new ChartFormulaParameter();
						chartFormulaParameter4.Name = "FormulaParameters";
						chartFormulaParameter4.Value = text8;
						chartDerivedSeries.ChartFormulaParameters.Add(chartFormulaParameter4);
						break;
					}
					case "NEWAREA":
					{
						if (bool.TryParse(text8, out bool result) && result)
						{
							chartDerivedSeries.ChartSeries.ChartAreaName = "#NewChartArea";
						}
						break;
					}
					case "STARTFROMFIRST":
					{
						ChartFormulaParameter chartFormulaParameter3 = new ChartFormulaParameter();
						chartFormulaParameter3.Name = "StartFromFirst";
						chartFormulaParameter3.Value = text8;
						chartDerivedSeries.ChartFormulaParameters.Add(chartFormulaParameter3);
						break;
					}
					case "OUTPUT":
					{
						ChartFormulaParameter chartFormulaParameter2 = new ChartFormulaParameter();
						chartFormulaParameter2.Name = "Output";
						chartFormulaParameter2.Value = text8.Replace("#OUTPUTSERIES", chartDerivedSeries.ChartSeries.Name);
						chartDerivedSeries.ChartFormulaParameters.Add(chartFormulaParameter2);
						break;
					}
					case "INPUT":
					{
						ChartFormulaParameter chartFormulaParameter = new ChartFormulaParameter();
						chartFormulaParameter.Name = "Input";
						chartFormulaParameter.Value = text8;
						chartDerivedSeries.ChartFormulaParameters.Add(chartFormulaParameter);
						break;
					}
					case "SECONDARYAXIS":
						if (m_throwUpgradeException)
						{
							throw new CRI2005UpgradeException();
						}
						base.UpgradeResults.HasUnsupportedDundasChartFeatures = true;
						break;
					}
				}
				derivedSeriesCollection.Add(chartDerivedSeries);
			}
			goto IL_0cdc;
			IL_0cdc:
			string text9 = ConvertDundasCRIStringProperty(seriesProperties["ID"]);
			if (text9 == null)
			{
				return;
			}
			foreach (Hashtable dataPointCustomProperty in dataPointCustomProperties)
			{
				if (!(ConvertDundasCRIStringProperty(dataPointCustomProperty["ID"]) == text9))
				{
					continue;
				}
				Microsoft.ReportingServices.RdlObjectModel.ChartDataPoint chartDataPoint = new Microsoft.ReportingServices.RdlObjectModel.ChartDataPoint();
				chartDataPoint.ChartDataPointValues = new ChartDataPointValues();
				series.ChartDataPoints.Add(chartDataPoint);
				ConvertDundasCRICustomProperties(chartDataPoint.CustomProperties, dataPointCustomProperty["CustomAttributes"]);
				chartDataPoint.AxisLabel = ConvertDundasCRIStringProperty(dataPointCustomProperty["AxisLabel"]);
				chartDataPoint.ChartDataPointValues.X = ConvertDundasCRIStringProperty(dataPointCustomProperty["XValue"]);
				if (series.Type.Value == ChartTypes.Range)
				{
					switch (series.Subtype.Value)
					{
					case ChartSubtypes.BoxPlot:
						chartDataPoint.ChartDataPointValues.Median = ConvertDundasCRIStringProperty(dataPointCustomProperty["Y5"]);
						chartDataPoint.ChartDataPointValues.Mean = ConvertDundasCRIStringProperty(dataPointCustomProperty["Y4"]);
						chartDataPoint.ChartDataPointValues.End = ConvertDundasCRIStringProperty(dataPointCustomProperty["Y3"]);
						chartDataPoint.ChartDataPointValues.Start = ConvertDundasCRIStringProperty(dataPointCustomProperty["Y2"]);
						chartDataPoint.ChartDataPointValues.High = ConvertDundasCRIStringProperty(dataPointCustomProperty["Y1"]);
						chartDataPoint.ChartDataPointValues.Low = ConvertDundasCRIStringProperty(dataPointCustomProperty["Y0"]);
						break;
					case ChartSubtypes.Candlestick:
					case ChartSubtypes.Stock:
						chartDataPoint.ChartDataPointValues.End = ConvertDundasCRIStringProperty(dataPointCustomProperty["Y3"]);
						chartDataPoint.ChartDataPointValues.Start = ConvertDundasCRIStringProperty(dataPointCustomProperty["Y2"]);
						chartDataPoint.ChartDataPointValues.Low = ConvertDundasCRIStringProperty(dataPointCustomProperty["Y1"]);
						chartDataPoint.ChartDataPointValues.High = ConvertDundasCRIStringProperty(dataPointCustomProperty["Y0"]);
						break;
					case ChartSubtypes.ErrorBar:
						chartDataPoint.ChartDataPointValues.High = ConvertDundasCRIStringProperty(dataPointCustomProperty["Y2"]);
						chartDataPoint.ChartDataPointValues.Low = ConvertDundasCRIStringProperty(dataPointCustomProperty["Y1"]);
						chartDataPoint.ChartDataPointValues.Y = ConvertDundasCRIStringProperty(dataPointCustomProperty["Y0"]);
						break;
					case ChartSubtypes.Bar:
						chartDataPoint.ChartDataPointValues.End = ConvertDundasCRIStringProperty(dataPointCustomProperty["Y1"]);
						chartDataPoint.ChartDataPointValues.Start = ConvertDundasCRIStringProperty(dataPointCustomProperty["Y0"]);
						break;
					default:
						chartDataPoint.ChartDataPointValues.Low = ConvertDundasCRIStringProperty(dataPointCustomProperty["Y1"]);
						chartDataPoint.ChartDataPointValues.High = ConvertDundasCRIStringProperty(dataPointCustomProperty["Y0"]);
						break;
					}
				}
				else
				{
					if (series.Subtype.Value == ChartSubtypes.Bubble)
					{
						chartDataPoint.ChartDataPointValues.Size = ConvertDundasCRIStringProperty(dataPointCustomProperty["Y1"]);
					}
					chartDataPoint.ChartDataPointValues.Y = ConvertDundasCRIStringProperty(dataPointCustomProperty["Y0"]);
				}
				if (!text.StartsWith("fast", StringComparison.OrdinalIgnoreCase))
				{
					chartDataPoint.Style = ConvertDundasCRIEmptyColorStyleProperty(dataPointCustomProperty["Color"], null, dataPointCustomProperty["BackGradientType"], dataPointCustomProperty["BackGradientEndColor"], dataPointCustomProperty["BackHatchStyle"], dataPointCustomProperty["BorderColor"] ?? seriesProperties["BorderColor"], dataPointCustomProperty["BorderStyle"] ?? seriesProperties["BorderStyle"], dataPointCustomProperty["BorderWidth"] ?? seriesProperties["BorderWidth"], dataPointCustomProperty["BackImage"], dataPointCustomProperty["BackImageTranspColor"], dataPointCustomProperty["BackImageAlign"], dataPointCustomProperty["BackImageMode"], null, null, ref counter);
				}
				else
				{
					chartDataPoint.Style = ConvertDundasCRIEmptyColorStyleProperty(dataPointCustomProperty["Color"], null, dataPointCustomProperty["BackGradientType"], dataPointCustomProperty["BackGradientEndColor"], dataPointCustomProperty["BackHatchStyle"], null, null, null, null, null, null, null, null, null, ref counter);
				}
				counter = 0;
				ChartMarker chartMarker2 = new ChartMarker();
				chartMarker2.Type = new ReportExpression<ChartMarkerTypes>(ConvertDundasCRIStringProperty(dataPointCustomProperty["MarkerStyle"], ref counter), CultureInfo.InvariantCulture);
				chartMarker2.Size = ConvertDundasCRIPixelReportSizeProperty(dataPointCustomProperty["MarkerSize"] ?? seriesProperties["MarkerSize"], ref counter);
				chartMarker2.Style = ConvertDundasCRIEmptyColorStyleProperty(dataPointCustomProperty["MarkerColor"], null, null, null, null, (!IsZero(dataPointCustomProperty["MarkerBorderWidth"])) ? dataPointCustomProperty["MarkerBorderColor"] : ((object)ReportColor.Empty), null, (!IsZero(dataPointCustomProperty["MarkerBorderWidth"])) ? dataPointCustomProperty["MarkerBorderWidth"] : null, dataPointCustomProperty["MarkerImage"], dataPointCustomProperty["MarkerImageTranspColor"], null, null, null, null, ref counter);
				if (counter > 0)
				{
					chartDataPoint.ChartMarker = chartMarker2;
				}
				counter = 0;
				Microsoft.ReportingServices.RdlObjectModel.ChartDataLabel chartDataLabel2 = new Microsoft.ReportingServices.RdlObjectModel.ChartDataLabel();
				bool? flag6 = ConvertDundasCRIBoolProperty(seriesProperties["ShowLabelAsValue"], ref counter);
				if (flag6.HasValue)
				{
					chartDataLabel2.UseValueAsLabel = flag6.Value;
				}
				string value3 = ConvertDundasCRIStringProperty(dataPointCustomProperty["Label"], ref counter);
				chartDataLabel2.Label = value3;
				chartDataLabel2.Visible = (!string.IsNullOrEmpty(value3) || chartDataLabel2.UseValueAsLabel.Value);
				chartDataLabel2.Rotation = ConvertDundasCRIIntegerReportExpressionProperty(dataPointCustomProperty["FontAngle"], ref counter);
				chartDataLabel2.ActionInfo = ConvertDundasCRIActionInfoProperty(dataPointCustomProperty["LabelHref"], ref counter);
				chartDataLabel2.Style = ConvertDundasCRIStyleProperty(dataPointCustomProperty["FontColor"], dataPointCustomProperty["LabelBackColor"], null, null, null, null, null, dataPointCustomProperty["LabelBorderColor"], dataPointCustomProperty["LabelBorderStyle"], dataPointCustomProperty["LabelBorderWidth"], null, null, null, null, dataPointCustomProperty["Font"] ?? "Microsoft Sans Serif, 8pt", dataPointCustomProperty["LabelFormat"], null, null, null, ref counter);
				if (counter > 0)
				{
					chartDataPoint.ChartDataLabel = chartDataLabel2;
				}
				chartDataPoint.ActionInfo = (UpgradeDundasCRIChartActionInfo(dataPointCustomProperty) ?? ConvertDundasCRIActionInfoProperty(dataPointCustomProperty["Href"]));
				counter = 0;
				ChartItemInLegend chartItemInLegend2 = new ChartItemInLegend();
				chartItemInLegend2.ActionInfo = ConvertDundasCRIActionInfoProperty(dataPointCustomProperty["LegendHref"], ref counter);
				chartItemInLegend2.LegendText = ConvertDundasCRIStringProperty(dataPointCustomProperty["LegendText"], ref counter);
				if (counter > 0)
				{
					chartDataPoint.ChartItemInLegend = chartItemInLegend2;
				}
				string text10 = ConvertDundasCRIStringProperty(dataPointCustomProperty["MarkerBorderWidth"]);
				if (series.ChartEmptyPoints != null && series.ChartEmptyPoints.ChartMarker != null && text10 != string.Empty)
				{
					if (series.ChartEmptyPoints.ChartMarker.Style == null)
					{
						series.ChartEmptyPoints.ChartMarker.Style = new EmptyColorStyle();
					}
					if (series.ChartEmptyPoints.ChartMarker.Style.Border == null)
					{
						series.ChartEmptyPoints.ChartMarker.Style.Border = new EmptyBorder();
					}
					if (IsZero(text10))
					{
						series.ChartEmptyPoints.ChartMarker.Style.Border.Color = ReportColor.Empty;
					}
					else
					{
						series.ChartEmptyPoints.ChartMarker.Style.Border.Width = ConvertDundasCRIPixelReportSizeProperty(text10);
					}
				}
			}
		}

		private bool UpgradeDundasCRIChartTitle(Microsoft.ReportingServices.RdlObjectModel.ChartTitle title, Hashtable titleProperties, string propertyPrefix)
		{
			int counter = 0;
			title.Caption = ConvertDundasCRIStringProperty(titleProperties[propertyPrefix + "Text"], ref counter);
			title.DockOffset = ConvertDundasCRIIntegerReportExpressionProperty(titleProperties[propertyPrefix + "DockOffset"], ref counter);
			string docking = ConvertDundasCRIStringProperty(titleProperties[propertyPrefix + "Docking"], ref counter);
			string alignment = ConvertDundasCRIStringProperty(titleProperties[propertyPrefix + "Alignment"], ref counter);
			title.Position = ConvertDundasCRIPosition(docking, alignment);
			bool? flag = ConvertDundasCRIBoolProperty(titleProperties[propertyPrefix + "DockInsideChartArea"], ref counter);
			if (flag.HasValue)
			{
				title.DockOutsideChartArea = !flag.Value;
			}
			bool? flag2 = ConvertDundasCRIBoolProperty(titleProperties[propertyPrefix + "Visible"], ref counter);
			if (flag2.HasValue)
			{
				title.Hidden = !flag2.Value;
			}
			title.Style = ConvertDundasCRIStyleProperty(titleProperties[propertyPrefix + "Color"], titleProperties[propertyPrefix + "BackColor"], titleProperties[propertyPrefix + "BackGradientType"], titleProperties[propertyPrefix + "BackGradientEndColor"], titleProperties[propertyPrefix + "BackHatchStyle"], titleProperties[propertyPrefix + "ShadowColor"], titleProperties[propertyPrefix + "ShadowOffset"], titleProperties[propertyPrefix + "BorderColor"] ?? ((object)Color.Transparent), titleProperties[propertyPrefix + "BorderStyle"], titleProperties[propertyPrefix + "BorderWidth"], titleProperties[propertyPrefix + "BackImage"], titleProperties[propertyPrefix + "BackImageTranspColor"], titleProperties[propertyPrefix + "BackImageAlign"], titleProperties[propertyPrefix + "BackImageMode"], ConvertDundasCRIStringProperty("Microsoft Sans Serif, 8pt", titleProperties[propertyPrefix + "Font"]), null, titleProperties[propertyPrefix + "Style"], null, null, ref counter);
			title.ChartElementPosition = ConvertDundasCRIChartElementPosition(titleProperties[propertyPrefix + "Position.Y"], titleProperties[propertyPrefix + "Position.X"], titleProperties[propertyPrefix + "Position.Height"], titleProperties[propertyPrefix + "Position.Width"], ref counter);
			return counter > 0;
		}

		private void UpgradeDundasCRIChartLegend(ChartLegend legend, Hashtable legendProperties, string propertyPrefix)
		{
			legend.HeaderSeparator = new ReportExpression<ChartHeaderSeparatorTypes>(ConvertDundasCRIStringProperty(legendProperties[propertyPrefix + "HeaderSeparator"]), CultureInfo.InvariantCulture);
			legend.InterlacedRowsColor = ConvertDundasCRIColorProperty(legend.InterlacedRowsColor, legendProperties[propertyPrefix + "InterlacedRowsColor"]);
			legend.Reversed = new ReportExpression<ChartLegendReversedTypes>(ConvertDundasCRIStringProperty(legendProperties[propertyPrefix + "Reversed"]), CultureInfo.InvariantCulture);
			legend.ColumnSeparator = new ReportExpression<ChartColumnSeparatorTypes>(ConvertDundasCRIStringProperty(legendProperties[propertyPrefix + "ItemColumnSeparator"]), CultureInfo.InvariantCulture);
			legend.ColumnSeparatorColor = ConvertDundasCRIColorProperty(legend.ColumnSeparatorColor, legendProperties[propertyPrefix + "ItemColumnSeparatorColor"]);
			legend.ColumnSpacing = ConvertDundasCRIIntegerReportExpressionProperty(legend.ColumnSpacing, legendProperties[propertyPrefix + "ItemColumnSpacing"]);
			legend.MaxAutoSize = ConvertDundasCRIIntegerReportExpressionProperty(legend.MaxAutoSize, legendProperties[propertyPrefix + "MaxAutoSize"]);
			legend.TextWrapThreshold = ConvertDundasCRIIntegerReportExpressionProperty(legend.TextWrapThreshold, legendProperties[propertyPrefix + "TextWrapThreshold"]);
			legend.MinFontSize = ConvertDundasCRIPointReportSizeProperty(legend.MinFontSize, legendProperties[propertyPrefix + "AutoFitMinFontSize"]);
			bool? flag = ConvertDundasCRIBoolProperty(legendProperties[propertyPrefix + "AutoFitText"]);
			if (flag.HasValue)
			{
				legend.AutoFitTextDisabled = !flag.Value;
			}
			bool? flag2 = ConvertDundasCRIBoolProperty(legendProperties[propertyPrefix + "Enabled"]);
			if (flag2.HasValue)
			{
				legend.Hidden = !flag2.Value;
			}
			bool? flag3 = ConvertDundasCRIBoolProperty(legendProperties[propertyPrefix + "DockInsideChartArea"]);
			if (flag3.HasValue)
			{
				legend.DockOutsideChartArea = !flag3.Value;
			}
			bool? flag4 = ConvertDundasCRIBoolProperty(legendProperties[propertyPrefix + "InterlacedRows"]);
			if (flag4.HasValue)
			{
				legend.InterlacedRows = flag4.Value;
			}
			bool? flag5 = ConvertDundasCRIBoolProperty(legendProperties[propertyPrefix + "EquallySpacedItems"]);
			if (flag5.HasValue)
			{
				legend.EquallySpacedItems = flag5.Value;
			}
			legend.Style = ConvertDundasCRIStyleProperty(legendProperties[propertyPrefix + "FontColor"], legendProperties[propertyPrefix + "BackColor"], legendProperties[propertyPrefix + "BackGradientType"], legendProperties[propertyPrefix + "BackGradientEndColor"], legendProperties[propertyPrefix + "BackHatchStyle"], legendProperties[propertyPrefix + "ShadowColor"], legendProperties[propertyPrefix + "ShadowOffset"], legendProperties[propertyPrefix + "BorderColor"], legendProperties[propertyPrefix + "BorderStyle"] ?? ((object)BorderStyles.Solid), legendProperties[propertyPrefix + "BorderWidth"], legendProperties[propertyPrefix + "BackImage"], legendProperties[propertyPrefix + "BackImageTranspColor"], legendProperties[propertyPrefix + "BackImageAlign"], legendProperties[propertyPrefix + "BackImageMode"], ConvertDundasCRIStringProperty("Microsoft Sans Serif, 8pt", legendProperties[propertyPrefix + "Font"]), null, null, null, null);
			string text = ConvertDundasCRIStringProperty("Right", legendProperties[propertyPrefix + "Docking"]);
			string text2 = ConvertDundasCRIStringProperty("Near", legendProperties[propertyPrefix + "Alignment"]);
			text2 = ((!(text == "Top") && !(text == "Bottom")) ? text2.Replace("Near", "Top").Replace("Far", "Bottom") : text2.Replace("Near", "Left").Replace("Far", "Right"));
			legend.Position = new ReportExpression<ChartPositions>(text + text2, CultureInfo.InvariantCulture);
			string text3 = ConvertDundasCRIStringProperty(legendProperties[propertyPrefix + "LegendStyle"]);
			string a = ConvertDundasCRIStringProperty(legendProperties[propertyPrefix + "TableStyle"]);
			if (text3 != "" && text3 != "Table")
			{
				legend.Layout = new ReportExpression<ChartLegendLayouts>(text3, CultureInfo.InvariantCulture);
			}
			else if (a == "Wide")
			{
				legend.Layout = ChartLegendLayouts.WideTable;
			}
			else if (a == "Tall")
			{
				legend.Layout = ChartLegendLayouts.TallTable;
			}
			else
			{
				legend.Layout = ChartLegendLayouts.AutoTable;
			}
			legend.ChartElementPosition = ConvertDundasCRIChartElementPosition(legendProperties[propertyPrefix + "Position.Y"], legendProperties[propertyPrefix + "Position.X"], legendProperties[propertyPrefix + "Position.Height"], legendProperties[propertyPrefix + "Position.Width"]);
			int counter = 0;
			ChartLegendTitle chartLegendTitle = new ChartLegendTitle();
			chartLegendTitle.Caption = ConvertDundasCRIStringProperty(legendProperties[propertyPrefix + "Title"], ref counter);
			int counter2 = 0;
			string a2 = ConvertDundasCRIStringProperty(legendProperties[propertyPrefix + "TitleAlignment"], ref counter2);
			chartLegendTitle.Style = ConvertDundasCRIStyleProperty(legendProperties[propertyPrefix + "TitleColor"], legendProperties[propertyPrefix + "TitleBackColor"], null, null, null, null, null, legendProperties[propertyPrefix + "TitleSeparatorColor"], null, null, null, null, null, null, ConvertDundasCRIStringProperty("Microsoft Sans Serif, 8pt, style=Bold", legendProperties[propertyPrefix + "TitleFont"]), null, null, (counter2 == 0) ? null : ((a2 == "Near") ? TextAlignments.Left.ToString() : ((a2 == "Far") ? TextAlignments.Right.ToString() : TextAlignments.Center.ToString())), null, ref counter);
			try
			{
				chartLegendTitle.TitleSeparator = new ReportExpression<ChartTitleSeparatorTypes>(ConvertDundasCRIStringProperty(legendProperties[propertyPrefix + "TitleSeparator"], ref counter), CultureInfo.InvariantCulture);
			}
			catch
			{
			}
			if (counter > 0)
			{
				legend.ChartLegendTitle = chartLegendTitle;
			}
			foreach (DictionaryEntry legendProperty in legendProperties)
			{
				string text4 = legendProperty.Key.ToString();
				if (text4.StartsWith("LEGEND.CUSTOMITEMS.", StringComparison.OrdinalIgnoreCase) || text4.StartsWith("LEGEND.CELLCOLUMNS.", StringComparison.OrdinalIgnoreCase))
				{
					base.UpgradeResults.HasUnsupportedDundasChartFeatures = true;
				}
			}
		}

		private bool UpgradeDundasCRIChartGridLines(ChartGridLines gridLines, Hashtable properties, string propertyPrefix, bool isMajor)
		{
			int counter = 0;
			gridLines.Interval = ConvertDundasCRIDoubleReportExpressionProperty(properties[propertyPrefix + "Interval"], ref counter);
			gridLines.IntervalType = new ReportExpression<ChartIntervalTypes>(ConvertDundasCRIStringProperty(properties[propertyPrefix + "IntervalType"], ref counter), CultureInfo.InvariantCulture);
			gridLines.IntervalOffset = ConvertDundasCRIDoubleReportExpressionProperty(double.NaN, properties[propertyPrefix + "IntervalOffset"], ref counter);
			gridLines.IntervalOffsetType = new ReportExpression<ChartIntervalOffsetTypes>(ConvertDundasCRIStringProperty(properties[propertyPrefix + "IntervalOffsetType"], ref counter), CultureInfo.InvariantCulture);
			if (isMajor)
			{
				bool? flag = ConvertDundasCRIBoolProperty(properties[propertyPrefix + "Enabled"], ref counter);
				if (flag.HasValue && !flag.Value)
				{
					gridLines.Enabled = ChartGridLinesEnabledTypes.False;
				}
				else
				{
					gridLines.Enabled = ChartGridLinesEnabledTypes.True;
				}
			}
			else
			{
				bool? flag2 = ConvertDundasCRIBoolProperty(properties[propertyPrefix + "Disabled"], ref counter);
				if (flag2.HasValue && !flag2.Value)
				{
					gridLines.Enabled = ChartGridLinesEnabledTypes.True;
				}
				else
				{
					gridLines.Enabled = ChartGridLinesEnabledTypes.False;
				}
			}
			gridLines.Style = ConvertDundasCRIStyleProperty(null, null, null, null, null, null, null, properties[propertyPrefix + "LineColor"], properties[propertyPrefix + "LineStyle"], properties[propertyPrefix + "LineWidth"], null, null, null, null, null, null, null, null, null, ref counter);
			return counter > 0;
		}

		private bool UpgradeDundasCRIChartTickMarks(ChartTickMarks tickMarks, Hashtable properties, string propertyPrefix, bool isMajor)
		{
			int counter = 0;
			tickMarks.Type = new ReportExpression<ChartTickMarkTypes>(ConvertDundasCRIStringProperty(properties[propertyPrefix + "Style"], ref counter), CultureInfo.InvariantCulture);
			tickMarks.Length = ConvertDundasCRIDoubleReportExpressionProperty(tickMarks.Length, properties[propertyPrefix + "Size"], ref counter);
			tickMarks.Interval = ConvertDundasCRIDoubleReportExpressionProperty(properties[propertyPrefix + "Interval"], ref counter);
			tickMarks.IntervalType = new ReportExpression<ChartIntervalTypes>(ConvertDundasCRIStringProperty(properties[propertyPrefix + "IntervalType"], ref counter), CultureInfo.InvariantCulture);
			tickMarks.IntervalOffset = ConvertDundasCRIDoubleReportExpressionProperty(properties[propertyPrefix + "IntervalOffset"], ref counter);
			tickMarks.IntervalOffsetType = new ReportExpression<ChartIntervalOffsetTypes>(ConvertDundasCRIStringProperty(properties[propertyPrefix + "IntervalOffsetType"], ref counter), CultureInfo.InvariantCulture);
			if (isMajor)
			{
				bool? flag = ConvertDundasCRIBoolProperty(properties[propertyPrefix + "Enabled"], ref counter);
				if (flag.HasValue && !flag.Value)
				{
					tickMarks.Enabled = ChartTickMarksEnabledTypes.False;
				}
				else
				{
					tickMarks.Enabled = ChartTickMarksEnabledTypes.True;
				}
			}
			else
			{
				bool? flag2 = ConvertDundasCRIBoolProperty(properties[propertyPrefix + "Disabled"], ref counter);
				if (flag2.HasValue && !flag2.Value)
				{
					tickMarks.Enabled = ChartTickMarksEnabledTypes.True;
				}
				else
				{
					tickMarks.Enabled = ChartTickMarksEnabledTypes.False;
				}
			}
			tickMarks.Style = ConvertDundasCRIStyleProperty(null, null, null, null, null, null, null, properties[propertyPrefix + "LineColor"], properties[propertyPrefix + "LineStyle"], properties[propertyPrefix + "LineWidth"], null, null, null, null, null, null, null, null, null, ref counter);
			return counter > 0;
		}

		private ActionInfo UpgradeDundasCRIChartActionInfo(Hashtable properties)
		{
			return UpgradeDundasCRIActionInfo(properties, string.Empty, "Hyperlink");
		}

		private ChartPositions ConvertDundasCRIPosition(string docking, string alignment)
		{
			switch (docking)
			{
			case "Left":
				if (alignment.EndsWith("Right", StringComparison.Ordinal))
				{
					return ChartPositions.LeftTop;
				}
				if (alignment.EndsWith("Left", StringComparison.Ordinal))
				{
					return ChartPositions.LeftBottom;
				}
				return ChartPositions.LeftCenter;
			case "Right":
				if (alignment.EndsWith("Left", StringComparison.Ordinal))
				{
					return ChartPositions.RightTop;
				}
				if (alignment.EndsWith("Right", StringComparison.Ordinal))
				{
					return ChartPositions.RightBottom;
				}
				return ChartPositions.RightCenter;
			case "Bottom":
				if (alignment.EndsWith("Left", StringComparison.Ordinal))
				{
					return ChartPositions.BottomLeft;
				}
				if (alignment.EndsWith("Right", StringComparison.Ordinal))
				{
					return ChartPositions.BottomRight;
				}
				return ChartPositions.BottomCenter;
			default:
				if (alignment.EndsWith("Left", StringComparison.Ordinal))
				{
					return ChartPositions.TopLeft;
				}
				if (alignment.EndsWith("Right", StringComparison.Ordinal))
				{
					return ChartPositions.TopRight;
				}
				return ChartPositions.TopCenter;
			}
		}

		private void SetChartSeriesType(ChartSeries series, string dundasSeriesType)
		{
			switch (dundasSeriesType)
			{
			case "Column":
				series.Type = ChartTypes.Column;
				series.Subtype = ChartSubtypes.Plain;
				break;
			case "StackedColumn":
				series.Type = ChartTypes.Column;
				series.Subtype = ChartSubtypes.Stacked;
				break;
			case "StackedColumn100":
				series.Type = ChartTypes.Column;
				series.Subtype = ChartSubtypes.PercentStacked;
				break;
			case "Bar":
				series.Type = ChartTypes.Bar;
				series.Subtype = ChartSubtypes.Plain;
				break;
			case "StackedBar":
				series.Type = ChartTypes.Bar;
				series.Subtype = ChartSubtypes.Stacked;
				break;
			case "StackedBar100":
				series.Type = ChartTypes.Bar;
				series.Subtype = ChartSubtypes.PercentStacked;
				break;
			case "Area":
				series.Type = ChartTypes.Area;
				series.Subtype = ChartSubtypes.Plain;
				break;
			case "SplineArea":
				series.Type = ChartTypes.Area;
				series.Subtype = ChartSubtypes.Smooth;
				break;
			case "StackedArea":
				series.Type = ChartTypes.Area;
				series.Subtype = ChartSubtypes.Stacked;
				break;
			case "StackedArea100":
				series.Type = ChartTypes.Area;
				series.Subtype = ChartSubtypes.PercentStacked;
				break;
			case "Line":
			case "FastLine":
				series.Type = ChartTypes.Line;
				series.Subtype = ChartSubtypes.Plain;
				break;
			case "Spline":
				series.Type = ChartTypes.Line;
				series.Subtype = ChartSubtypes.Smooth;
				break;
			case "StepLine":
				series.Type = ChartTypes.Line;
				series.Subtype = ChartSubtypes.Stepped;
				break;
			case "Point":
			case "FastPoint":
				series.Type = ChartTypes.Scatter;
				series.Subtype = ChartSubtypes.Plain;
				break;
			case "Bubble":
				series.Type = ChartTypes.Scatter;
				series.Subtype = ChartSubtypes.Bubble;
				break;
			case "Stock":
				series.Type = ChartTypes.Range;
				series.Subtype = ChartSubtypes.Stock;
				break;
			case "CandleStick":
				series.Type = ChartTypes.Range;
				series.Subtype = ChartSubtypes.Candlestick;
				break;
			case "RangeColumn":
				series.Type = ChartTypes.Range;
				series.Subtype = ChartSubtypes.Column;
				break;
			case "Range":
				series.Type = ChartTypes.Range;
				series.Subtype = ChartSubtypes.Plain;
				break;
			case "SplineRange":
				series.Type = ChartTypes.Range;
				series.Subtype = ChartSubtypes.Smooth;
				break;
			case "ErrorBar":
				series.Type = ChartTypes.Range;
				series.Subtype = ChartSubtypes.ErrorBar;
				break;
			case "BoxPlot":
				series.Type = ChartTypes.Range;
				series.Subtype = ChartSubtypes.BoxPlot;
				break;
			case "Gantt":
				series.Type = ChartTypes.Range;
				series.Subtype = ChartSubtypes.Bar;
				break;
			case "Pie":
				series.Type = ChartTypes.Shape;
				series.Subtype = ChartSubtypes.Pie;
				break;
			case "Doughnut":
				series.Type = ChartTypes.Shape;
				series.Subtype = ChartSubtypes.Doughnut;
				break;
			case "Funnel":
				series.Type = ChartTypes.Shape;
				series.Subtype = ChartSubtypes.Funnel;
				break;
			case "Pyramid":
				series.Type = ChartTypes.Shape;
				series.Subtype = ChartSubtypes.Pyramid;
				break;
			case "Polar":
				series.Type = ChartTypes.Polar;
				series.Subtype = ChartSubtypes.Plain;
				break;
			case "Radar":
				series.Type = ChartTypes.Polar;
				series.Subtype = ChartSubtypes.Radar;
				break;
			}
		}

		private BackgroundImage ConvertDundasCRIChartBackgroundImageProperty(object imageReference, object transparentColor, object align, object mode, ref int counter)
		{
			int counter2 = 0;
			BackgroundImage backgroundImage = new BackgroundImage();
			backgroundImage.Source = SourceType.External;
			backgroundImage.Value = ConvertDundasCRIStringProperty(imageReference, ref counter2);
			backgroundImage.TransparentColor = ConvertDundasCRIColorProperty(backgroundImage.TransparentColor, transparentColor, ref counter2);
			backgroundImage.Position = new ReportExpression<BackgroundPositions>(ConvertDundasCRIStringProperty(BackgroundPositions.TopLeft.ToString(), align, ref counter2), CultureInfo.InvariantCulture);
			string text = ConvertDundasCRIStringProperty(mode, ref counter2);
			switch (text)
			{
			case "Tile":
			case "TileFlipX":
			case "TileFlipY":
			case "TileFlipXY":
				backgroundImage.BackgroundRepeat = BackgroundRepeatTypes.Repeat;
				break;
			case "Scaled":
				backgroundImage.BackgroundRepeat = BackgroundRepeatTypes.Fit;
				break;
			case "Unscaled":
				backgroundImage.BackgroundRepeat = BackgroundRepeatTypes.Clip;
				break;
			default:
				if (text != string.Empty)
				{
					backgroundImage.BackgroundRepeat = new ReportExpression<BackgroundRepeatTypes>(text, CultureInfo.InvariantCulture);
				}
				break;
			}
			if (counter2 > 0)
			{
				counter++;
				return backgroundImage;
			}
			return null;
		}

		private void FixChartAxisStriplineTitleAngle(Microsoft.ReportingServices.RdlObjectModel.Chart chart)
		{
			foreach (ChartArea chartArea in chart.ChartAreas)
			{
				bool flag = false;
				if (chart.ChartData != null && chart.ChartData.ChartSeriesCollection != null)
				{
					foreach (ChartSeries item in chart.ChartData.ChartSeriesCollection)
					{
						if (item.ChartAreaName == chartArea.Name && item.Type.Value == ChartTypes.Bar)
						{
							flag = true;
							break;
						}
					}
				}
				IList<ChartAxis> list = flag ? chartArea.ChartValueAxes : chartArea.ChartCategoryAxes;
				IList<ChartAxis> list2 = flag ? chartArea.ChartCategoryAxes : chartArea.ChartValueAxes;
				foreach (ChartAxis item2 in list)
				{
					if (item2.ChartStripLines == null)
					{
						continue;
					}
					foreach (ChartStripLine chartStripLine in item2.ChartStripLines)
					{
						if (!chartStripLine.TextOrientation.IsExpression)
						{
							switch (chartStripLine.TextOrientation.Value)
							{
							case TextOrientations.Rotated90:
							case TextOrientations.Rotated270:
								chartStripLine.TextOrientation = TextOrientations.Horizontal;
								break;
							case TextOrientations.Stacked:
								chartStripLine.TextOrientation = TextOrientations.Rotated90;
								break;
							default:
								chartStripLine.TextOrientation = TextOrientations.Auto;
								break;
							}
						}
					}
				}
				foreach (ChartAxis item3 in list2)
				{
					if (item3.ChartStripLines == null)
					{
						continue;
					}
					foreach (ChartStripLine chartStripLine2 in item3.ChartStripLines)
					{
						if (!chartStripLine2.TextOrientation.IsExpression)
						{
							TextOrientations value = chartStripLine2.TextOrientation.Value;
							if (value == TextOrientations.Horizontal || value == TextOrientations.Stacked)
							{
								chartStripLine2.TextOrientation = TextOrientations.Auto;
							}
						}
					}
				}
			}
		}

		private void UpgradeDundasCRIGaugePanel(Microsoft.ReportingServices.RdlObjectModel.CustomReportItem cri, GaugePanel gaugePanel)
		{
			gaugePanel.Name = cri.Name;
			gaugePanel.ActionInfo = cri.ActionInfo;
			gaugePanel.Bookmark = cri.Bookmark;
			gaugePanel.DataElementName = cri.DataElementName;
			gaugePanel.DataElementOutput = cri.DataElementOutput;
			gaugePanel.DocumentMapLabel = cri.DocumentMapLabel;
			gaugePanel.PropertyStore.SetObject(12, cri.PropertyStore.GetObject(12));
			gaugePanel.Height = cri.Height;
			gaugePanel.Left = cri.Left;
			gaugePanel.Parent = cri.Parent;
			gaugePanel.RepeatWith = cri.RepeatWith;
			gaugePanel.Style = cri.Style;
			gaugePanel.ToolTip = cri.ToolTip;
			gaugePanel.PropertyStore.SetObject(10, cri.PropertyStore.GetObject(10));
			gaugePanel.Top = cri.Top;
			gaugePanel.Visibility = cri.Visibility;
			gaugePanel.Width = cri.Width;
			gaugePanel.ZIndex = cri.ZIndex;
			if (cri.CustomData != null)
			{
				gaugePanel.DataSetName = cri.CustomData.DataSetName;
				gaugePanel.Filters = cri.CustomData.Filters;
			}
			Hashtable hashtable = new Hashtable();
			Hashtable hashtable2 = new Hashtable();
			Hashtable hashtable3 = new Hashtable();
			List<Hashtable> list = new List<Hashtable>();
			List<Hashtable> list2 = new List<Hashtable>();
			List<Hashtable> list3 = new List<Hashtable>();
			foreach (CustomProperty customProperty in cri.CustomProperties)
			{
				string text = customProperty.Name.Value;
				if (text.StartsWith("expression:", StringComparison.OrdinalIgnoreCase))
				{
					text = text.Substring("expression:".Length);
				}
				if (!AddToPropertyList(list, "GaugeCore.Labels.", text, customProperty.Value) && !AddToPropertyList(list2, "GaugeCore.CircularGauges.", text, customProperty.Value) && !AddToPropertyList(list3, "GaugeCore.LinearGauges.", text, customProperty.Value))
				{
					hashtable.Add(text, customProperty.Value);
				}
				if (text.StartsWith("GAUGECORE.STATEINDICATORS.", StringComparison.OrdinalIgnoreCase) || text.StartsWith("GAUGECORE.NUMERICINDICATORS.", StringComparison.OrdinalIgnoreCase) || text.StartsWith("GAUGECORE.NAMEDIMAGES.", StringComparison.OrdinalIgnoreCase) || text.StartsWith("GAUGECORE.IMAGES.", StringComparison.OrdinalIgnoreCase))
				{
					base.UpgradeResults.HasUnsupportedDundasGaugeFeatures = true;
				}
			}
			if (hashtable["CUSTOM_CODE_CS"] != null || hashtable["CUSTOM_CODE_VB"] != null || hashtable["CUSTOM_CODE_COMPILED_ASSEMBLY"] != null)
			{
				if (m_throwUpgradeException)
				{
					throw new CRI2005UpgradeException();
				}
				base.UpgradeResults.HasUnsupportedDundasGaugeFeatures = true;
			}
			if (cri.CustomData != null && cri.CustomData.DataRowHierarchy != null && cri.CustomData.DataRowHierarchy.DataMembers != null && cri.CustomData.DataRowHierarchy.DataMembers.Count > 0)
			{
				foreach (CustomProperty customProperty2 in cri.CustomData.DataRowHierarchy.DataMembers[0].CustomProperties)
				{
					hashtable3.Add(customProperty2.Name, customProperty2.Value);
				}
			}
			if (cri.CustomData != null && cri.CustomData.DataRows != null && cri.CustomData.DataRows.Count > 0 && cri.CustomData.DataRows[0].Count > 0)
			{
				foreach (Microsoft.ReportingServices.RdlObjectModel.DataValue item in cri.CustomData.DataRows[0][0])
				{
					hashtable2.Add(item.Name, item.Value);
				}
			}
			gaugePanel.ToolTip = ConvertDundasCRIStringProperty(hashtable["GaugeCore.ToolTip"]);
			gaugePanel.AntiAliasing = new ReportExpression<AntiAliasingTypes>(ConvertDundasCRIStringProperty(hashtable["GaugeCore.AntiAliasing"]), CultureInfo.InvariantCulture);
			gaugePanel.TextAntiAliasingQuality = new ReportExpression<TextAntiAliasingQualityTypes>(ConvertDundasCRIStringProperty(hashtable["GaugeCore.TextAntiAliasingQuality"]), CultureInfo.InvariantCulture);
			gaugePanel.ShadowIntensity = ConvertDundasCRIDoubleReportExpressionProperty(gaugePanel.ShadowIntensity, hashtable["GaugeCore.ShadowIntensity"]);
			bool? flag = ConvertDundasCRIBoolProperty(hashtable["GaugeCore.AutoLayout"]);
			if (flag.HasValue)
			{
				gaugePanel.AutoLayout = flag.Value;
			}
			else
			{
				gaugePanel.AutoLayout = true;
			}
			gaugePanel.Style = ConvertDundasCRIStyleProperty(null, hashtable["GaugeCore.BackColor"] ?? ((object)Color.White), null, null, null, null, null, null, null, null, null, null, null);
			BackFrame backFrame = new BackFrame();
			if (UpgradeDundasCRIGaugeBackFrame(backFrame, hashtable, "GaugeCore.BackFrame."))
			{
				gaugePanel.BackFrame = backFrame;
			}
			foreach (Hashtable item2 in list)
			{
				GaugeLabel gaugeLabel = new GaugeLabel();
				gaugePanel.GaugeLabels.Add(gaugeLabel);
				UpgradeDundasCRIGaugeLabel(gaugeLabel, item2, "GaugeLabel.");
			}
			foreach (Hashtable item3 in list2)
			{
				RadialGauge radialGauge = new RadialGauge();
				gaugePanel.RadialGauges.Add(radialGauge);
				UpgradeDundasCRIGaugeRadial(radialGauge, item3, "CircularGauge.", hashtable3, hashtable2);
			}
			foreach (Hashtable item4 in list3)
			{
				LinearGauge linearGauge = new LinearGauge();
				gaugePanel.LinearGauges.Add(linearGauge);
				UpgradeDundasCRIGaugeLinear(linearGauge, item4, "LinearGauge.", hashtable3, hashtable2);
			}
			if (cri.CustomData != null && cri.CustomData.DataColumnHierarchy != null)
			{
				IList<DataMember> dataMembers = cri.CustomData.DataColumnHierarchy.DataMembers;
				GaugeMember gaugeMember = null;
				while (dataMembers != null && dataMembers.Count > 0)
				{
					DataMember dataMember = dataMembers[0];
					if (((DataGrouping2005)dataMember).Static)
					{
						break;
					}
					if (gaugeMember == null)
					{
						GaugeMember gaugeMember3 = gaugePanel.GaugeMember = new GaugeMember();
						gaugeMember = gaugeMember3;
					}
					else
					{
						GaugeMember gaugeMember3 = gaugeMember.ChildGaugeMember = new GaugeMember();
						gaugeMember = gaugeMember3;
					}
					gaugeMember.SortExpressions = dataMember.SortExpressions;
					if (dataMember.Group != null)
					{
						gaugeMember.Group = dataMember.Group;
					}
					dataMembers = dataMember.DataMembers;
				}
			}
			FixGaugeElementNames(gaugePanel);
		}

		private bool UpgradeDundasCRIGaugeBackFrame(BackFrame backFrame, Hashtable backFrameProperties, string propertyPrefix)
		{
			int counter = 0;
			string value = ConvertDundasCRIStringProperty(backFrameProperties[propertyPrefix + "FrameStyle"], ref counter);
			if (string.IsNullOrEmpty(value))
			{
				backFrame.FrameStyle = new ReportExpression<FrameStyles>(ConvertDundasCRIStringProperty(backFrameProperties[propertyPrefix + "Style"], ref counter), CultureInfo.InvariantCulture);
			}
			else
			{
				backFrame.FrameStyle = new ReportExpression<FrameStyles>(value, CultureInfo.InvariantCulture);
			}
			string value2 = ConvertDundasCRIStringProperty(backFrameProperties[propertyPrefix + "FrameShape"], ref counter);
			if (string.IsNullOrEmpty(value2))
			{
				backFrame.FrameShape = new ReportExpression<FrameShapes>(ConvertDundasCRIStringProperty(backFrameProperties[propertyPrefix + "Shape"], ref counter), CultureInfo.InvariantCulture);
			}
			else
			{
				backFrame.FrameShape = new ReportExpression<FrameShapes>(value2, CultureInfo.InvariantCulture);
			}
			backFrame.FrameWidth = ConvertDundasCRIDoubleReportExpressionProperty(backFrame.FrameWidth, backFrameProperties[propertyPrefix + "FrameWidth"], ref counter);
			backFrame.GlassEffect = new ReportExpression<GlassEffects>(ConvertDundasCRIStringProperty(backFrameProperties[propertyPrefix + "GlassEffect"], ref counter), CultureInfo.InvariantCulture);
			backFrame.Style = ConvertDundasCRIStyleProperty(null, backFrameProperties[propertyPrefix + "FrameColor"] ?? ((object)Color.Gainsboro), backFrameProperties[propertyPrefix + "FrameGradientType"] ?? ((object)BackgroundGradients.DiagonalLeft), backFrameProperties[propertyPrefix + "FrameGradientEndColor"] ?? ((object)Color.Gray), backFrameProperties[propertyPrefix + "FrameHatchStyle"], backFrameProperties[propertyPrefix + "ShadowOffset"], backFrameProperties[propertyPrefix + "BorderColor"], backFrameProperties[propertyPrefix + "BorderStyle"], backFrameProperties[propertyPrefix + "BorderWidth"], null, null, null, null, ref counter);
			int counter2 = 0;
			Microsoft.ReportingServices.RdlObjectModel.Style style = ConvertDundasCRIStyleProperty(null, backFrameProperties[propertyPrefix + "BackColor"] ?? ((object)Color.Silver), backFrameProperties[propertyPrefix + "BackGradientType"] ?? ((object)BackgroundGradients.DiagonalLeft), backFrameProperties[propertyPrefix + "BackGradientEndColor"] ?? ((object)Color.Gray), backFrameProperties[propertyPrefix + "BackHatchStyle"], null, null, null, null, null, null, null, null, ref counter2);
			if (counter2 > 0)
			{
				backFrame.FrameBackground = new FrameBackground();
				backFrame.FrameBackground.Style = style;
				counter++;
			}
			return counter > 0;
		}

		private void UpgradeDundasCRIGaugeLabel(GaugeLabel label, Hashtable labelProperties, string propertyPrefix)
		{
			label.Name = ConvertDundasCRIStringProperty(labelProperties[propertyPrefix + "Name"]);
			label.ParentItem = ConvertDundasCRIStringProperty(labelProperties[propertyPrefix + "Parent"]);
			label.ZIndex = ConvertDundasCRIIntegerReportExpressionProperty(labelProperties[propertyPrefix + "ZOrder"]);
			label.Left = ConvertDundasCRIDoubleReportExpressionProperty(labelProperties[propertyPrefix + "Location.X"]);
			label.Top = ConvertDundasCRIDoubleReportExpressionProperty(labelProperties[propertyPrefix + "Location.Y"]);
			label.Width = ConvertDundasCRIDoubleReportExpressionProperty(labelProperties[propertyPrefix + "Size.Width"]);
			label.Height = ConvertDundasCRIDoubleReportExpressionProperty(labelProperties[propertyPrefix + "Size.Height"]);
			label.ResizeMode = new ReportExpression<ResizeModes>(ConvertDundasCRIStringProperty(labelProperties[propertyPrefix + "ResizeMode"]), CultureInfo.InvariantCulture);
			label.Text = ConvertDundasCRIStringProperty("Text", labelProperties[propertyPrefix + "Text"]);
			label.TextShadowOffset = ConvertDundasCRIPixelReportSizeProperty(labelProperties[propertyPrefix + "TextShadowOffset"]);
			label.Angle = ConvertDundasCRIDoubleReportExpressionProperty(labelProperties[propertyPrefix + "Angle"]);
			bool? flag = ConvertDundasCRIBoolProperty(labelProperties[propertyPrefix + "Visible"]);
			if (flag.HasValue)
			{
				label.Hidden = !flag.Value;
			}
			if (ConvertDundasCRIStringProperty("Default", labelProperties[propertyPrefix + "FontUnit"]) == "Percent")
			{
				label.UseFontPercent = true;
			}
			string text = ConvertDundasCRIStringProperty(labelProperties[propertyPrefix + "TextAlignment"]);
			TextAlignments textAlignments = (string.IsNullOrEmpty(text) || text.EndsWith("LEFT", StringComparison.OrdinalIgnoreCase)) ? TextAlignments.Left : (text.EndsWith("CENTER", StringComparison.OrdinalIgnoreCase) ? TextAlignments.Center : TextAlignments.Right);
			VerticalAlignments verticalAlignments = (string.IsNullOrEmpty(text) || text.StartsWith("TOP", StringComparison.OrdinalIgnoreCase)) ? VerticalAlignments.Top : (text.StartsWith("MIDDLE", StringComparison.OrdinalIgnoreCase) ? VerticalAlignments.Middle : VerticalAlignments.Bottom);
			label.Style = ConvertDundasCRIStyleProperty(labelProperties[propertyPrefix + "TextColor"], labelProperties[propertyPrefix + "BackColor"], labelProperties[propertyPrefix + "BackGradientType"], labelProperties[propertyPrefix + "BackGradientEndColor"], labelProperties[propertyPrefix + "BackHatchStyle"], labelProperties[propertyPrefix + "BackShadowOffset"], labelProperties[propertyPrefix + "BorderColor"], labelProperties[propertyPrefix + "BorderStyle"], labelProperties[propertyPrefix + "BorderWidth"], labelProperties.ContainsKey(propertyPrefix + "Font") ? labelProperties[propertyPrefix + "Font"] : "Microsoft Sans Serif, 8.25pt", null, textAlignments, verticalAlignments);
		}

		private void UpgradeDundasCRIGauge(Gauge gauge, Hashtable gaugeProperties, string propertyPrefix, Hashtable formulaProperties, Hashtable dataValueProperties)
		{
			string str = gauge.Name = ConvertDundasCRIStringProperty(gaugeProperties[propertyPrefix + "Name"]);
			Hashtable hashtable = new Hashtable();
			Hashtable hashtable2 = new Hashtable();
			string text2 = ((gauge is LinearGauge) ? "LinearGauge" : "CircularGauge") + ":" + str;
			foreach (DictionaryEntry formulaProperty in formulaProperties)
			{
				if (formulaProperty.Key.ToString().StartsWith(text2, StringComparison.Ordinal))
				{
					hashtable.Add(formulaProperty.Key.ToString().Remove(0, text2.Length + 1), formulaProperty.Value);
				}
			}
			foreach (DictionaryEntry dataValueProperty in dataValueProperties)
			{
				if (dataValueProperty.Key.ToString().StartsWith(text2, StringComparison.Ordinal))
				{
					hashtable2.Add(dataValueProperty.Key.ToString().Remove(0, text2.Length + 1), dataValueProperty.Value);
				}
			}
			gauge.ActionInfo = UpgradeDundasCRIGaugeActionInfo(formulaProperties, text2 + ":");
			gauge.ParentItem = ConvertDundasCRIStringProperty(gaugeProperties[propertyPrefix + "Parent"]);
			gauge.ZIndex = ConvertDundasCRIIntegerReportExpressionProperty(gaugeProperties[propertyPrefix + "ZOrder"]);
			gauge.Left = ConvertDundasCRIDoubleReportExpressionProperty(gaugeProperties[propertyPrefix + "Location.X"]);
			gauge.Top = ConvertDundasCRIDoubleReportExpressionProperty(gaugeProperties[propertyPrefix + "Location.Y"]);
			gauge.Width = ConvertDundasCRIDoubleReportExpressionProperty(gaugeProperties[propertyPrefix + "Size.Width"]);
			gauge.Height = ConvertDundasCRIDoubleReportExpressionProperty(gaugeProperties[propertyPrefix + "Size.Height"]);
			bool? flag = ConvertDundasCRIBoolProperty(gaugeProperties[propertyPrefix + "Visible"]);
			if (flag.HasValue)
			{
				gauge.Hidden = !flag.Value;
			}
			bool? flag2 = ConvertDundasCRIBoolProperty(gaugeProperties[propertyPrefix + "ClipContent"]);
			if (flag2.HasValue)
			{
				gauge.ClipContent = flag2.Value;
			}
			else
			{
				gauge.ClipContent = true;
			}
			BackFrame backFrame = new BackFrame();
			if (UpgradeDundasCRIGaugeBackFrame(backFrame, gaugeProperties, propertyPrefix + "BackFrame."))
			{
				gauge.BackFrame = backFrame;
			}
			List<Hashtable> list = new List<Hashtable>();
			List<Hashtable> list2 = new List<Hashtable>();
			List<Hashtable> list3 = new List<Hashtable>();
			foreach (DictionaryEntry gaugeProperty in gaugeProperties)
			{
				string key = gaugeProperty.Key.ToString();
				string value = gaugeProperty.Value.ToString();
				AddToPropertyList(list, propertyPrefix + "Scales.", key, value);
				AddToPropertyList(list2, propertyPrefix + "Ranges.", key, value);
				AddToPropertyList(list3, propertyPrefix + "Pointers.", key, value);
			}
			Hashtable hashtable3 = new Hashtable();
			foreach (Hashtable item in list)
			{
				if (gauge is LinearGauge)
				{
					LinearScale linearScale = new LinearScale();
					UpgradeDundasCRIGaugeScaleLinear(linearScale, item, "LinearScale.", hashtable, hashtable2);
					gauge.GaugeScales.Add(linearScale);
					hashtable3.Add(linearScale.Name, linearScale);
				}
				else
				{
					RadialScale radialScale = new RadialScale();
					UpgradeDundasCRIGaugeScaleRadial(radialScale, item, "CircularScale.", hashtable, hashtable2);
					gauge.GaugeScales.Add(radialScale);
					hashtable3.Add(radialScale.Name, radialScale);
				}
			}
			string text3 = (gauge.GaugeScales.Count > 0) ? gauge.GaugeScales[0].Name : "Default";
			foreach (Hashtable item2 in list2)
			{
				ScaleRange scaleRange = new ScaleRange();
				string text4 = text3;
				if (gauge is LinearGauge)
				{
					UpgradeDundasCRIGaugeScaleRange(scaleRange, item2, "LinearRange.", isLinear: true, hashtable, hashtable2);
					text4 = ConvertDundasCRIStringProperty(text3, item2["LinearRange.ScaleName"]);
				}
				else
				{
					UpgradeDundasCRIGaugeScaleRange(scaleRange, item2, "CircularRange.", isLinear: false, hashtable, hashtable2);
					text4 = ConvertDundasCRIStringProperty(text3, item2["CircularRange.ScaleName"]);
				}
				if (hashtable3.Contains(text4))
				{
					((GaugeScale)hashtable3[text4]).ScaleRanges.Add(scaleRange);
				}
			}
			foreach (Hashtable item3 in list3)
			{
				GaugePointer gaugePointer = null;
				string text5 = text3;
				if (gauge is LinearGauge)
				{
					gaugePointer = new LinearPointer();
					UpgradeDundasCRIGaugePointerLinear((LinearPointer)gaugePointer, item3, "LinearPointer.", hashtable, hashtable2);
					text5 = ConvertDundasCRIStringProperty(text3, item3["LinearPointer.ScaleName"]);
				}
				else
				{
					gaugePointer = new RadialPointer();
					UpgradeDundasCRIGaugePointerRadial((RadialPointer)gaugePointer, item3, "CircularPointer.", hashtable, hashtable2);
					text5 = ConvertDundasCRIStringProperty(text3, item3["CircularPointer.ScaleName"]);
				}
				if (hashtable3.Contains(text5))
				{
					((GaugeScale)hashtable3[text5]).GaugePointers.Add(gaugePointer);
				}
			}
		}

		private void UpgradeDundasCRIGaugeRadial(RadialGauge gauge, Hashtable gaugeProperties, string propertyPrefix, Hashtable formulaProperties, Hashtable dataValueProperties)
		{
			UpgradeDundasCRIGauge(gauge, gaugeProperties, propertyPrefix, formulaProperties, dataValueProperties);
			gauge.PivotX = ConvertDundasCRIDoubleReportExpressionProperty(gauge.PivotX, gaugeProperties[propertyPrefix + "PivotPoint.X"]);
			gauge.PivotY = ConvertDundasCRIDoubleReportExpressionProperty(gauge.PivotY, gaugeProperties[propertyPrefix + "PivotPoint.Y"]);
		}

		private void UpgradeDundasCRIGaugeLinear(LinearGauge gauge, Hashtable gaugeProperties, string propertyPrefix, Hashtable formulaCustomProperties, Hashtable dataValueCustomProperties)
		{
			UpgradeDundasCRIGauge(gauge, gaugeProperties, propertyPrefix, formulaCustomProperties, dataValueCustomProperties);
			gauge.Orientation = new ReportExpression<Orientations>(ConvertDundasCRIStringProperty(gaugeProperties[propertyPrefix + "Orientation"]), CultureInfo.InvariantCulture);
		}

		private void UpgradeDundasCRIGaugeScale(GaugeScale scale, Hashtable scaleProperties, string propertyPrefix, Hashtable formulaProperties, Hashtable dataValueProperties)
		{
			string str2 = string.Concat(str2: scale.Name = ConvertDundasCRIStringProperty(scaleProperties[propertyPrefix + "Name"]), str0: (scale is LinearScale) ? "LinearScale" : "CircularScale", str1: ":", str3: ":");
			scale.MinimumValue = UpgradeDundasCRIGaugeInputValue(formulaProperties, dataValueProperties, str2 + "Minimum", scaleProperties, propertyPrefix + "Minimum");
			scale.MaximumValue = UpgradeDundasCRIGaugeInputValue(formulaProperties, dataValueProperties, str2 + "Maximum", scaleProperties, propertyPrefix + "Maximum");
			scale.Multiplier = ConvertDundasCRIDoubleReportExpressionProperty(scale.Multiplier, scaleProperties[propertyPrefix + "Multiplier"]);
			scale.Interval = ConvertDundasCRIDoubleReportExpressionProperty(scaleProperties[propertyPrefix + "Interval"]);
			scale.IntervalOffset = ConvertDundasCRIDoubleReportExpressionProperty(double.NaN, scaleProperties[propertyPrefix + "IntervalOffset"]);
			scale.LogarithmicBase = ConvertDundasCRIDoubleReportExpressionProperty(scale.LogarithmicBase, scaleProperties[propertyPrefix + "LogarithmicBase"]);
			scale.Width = ConvertDundasCRIDoubleReportExpressionProperty(scale.Width, scaleProperties[propertyPrefix + "Width"]);
			scale.Style = ConvertDundasCRIStyleProperty(null, scaleProperties[propertyPrefix + "FillColor"] ?? ((object)Color.CornflowerBlue), scaleProperties[propertyPrefix + "FillGradientType"], scaleProperties[propertyPrefix + "FillGradientEndColor"] ?? ((object)Color.White), scaleProperties[propertyPrefix + "FillHatchStyle"], scaleProperties[propertyPrefix + "ShadowOffset"] ?? ((object)1), scaleProperties[propertyPrefix + "BorderColor"], scaleProperties[propertyPrefix + "BorderStyle"], scaleProperties[propertyPrefix + "BorderWidth"], null, null, null, null);
			bool? flag = ConvertDundasCRIBoolProperty(scaleProperties[propertyPrefix + "Visible"]);
			if (flag.HasValue)
			{
				scale.Hidden = !flag.Value;
			}
			bool? flag2 = ConvertDundasCRIBoolProperty(scaleProperties[propertyPrefix + "TickMarksOnTop"]);
			if (flag2.HasValue)
			{
				scale.TickMarksOnTop = flag2.Value;
			}
			bool? flag3 = ConvertDundasCRIBoolProperty(scaleProperties[propertyPrefix + "Reversed"]);
			if (flag3.HasValue)
			{
				scale.Reversed = flag3.Value;
			}
			bool? flag4 = ConvertDundasCRIBoolProperty(scaleProperties[propertyPrefix + "Logarithmic"]);
			if (flag4.HasValue)
			{
				scale.Logarithmic = flag4.Value;
			}
			ScalePin scalePin = new ScalePin();
			if (UpgradeDundasCRIGaugeScalePin(scalePin, scaleProperties, propertyPrefix + "MinimumPin.", 6.0, 6.0))
			{
				scale.MinimumPin = scalePin;
			}
			ScalePin scalePin2 = new ScalePin();
			if (UpgradeDundasCRIGaugeScalePin(scalePin2, scaleProperties, propertyPrefix + "MaximumPin.", 6.0, 6.0))
			{
				scale.MaximumPin = scalePin2;
			}
			int counter = 0;
			ScaleLabels scaleLabels = new ScaleLabels();
			scaleLabels.Placement = new ReportExpression<Placements>(ConvertDundasCRIStringProperty(Placements.Inside.ToString(), scaleProperties[propertyPrefix + "LabelStyle.Placement"], ref counter), CultureInfo.InvariantCulture);
			scaleLabels.Interval = ConvertDundasCRIDoubleReportExpressionProperty(scaleProperties[propertyPrefix + "LabelStyle.Interval"], ref counter);
			scaleLabels.IntervalOffset = ConvertDundasCRIDoubleReportExpressionProperty(double.NaN, scaleProperties[propertyPrefix + "LabelStyle.IntervalOffset"], ref counter);
			scaleLabels.FontAngle = ConvertDundasCRIDoubleReportExpressionProperty(scaleProperties[propertyPrefix + "LabelStyle.FontAngle"], ref counter);
			scaleLabels.DistanceFromScale = ConvertDundasCRIDoubleReportExpressionProperty(scaleLabels.DistanceFromScale, scaleProperties[propertyPrefix + "LabelStyle.DistanceFromScale"], ref counter);
			bool? flag5 = ConvertDundasCRIBoolProperty(scaleProperties[propertyPrefix + "LabelStyle.Visible"], ref counter);
			if (flag5.HasValue)
			{
				scaleLabels.Hidden = !flag5.Value;
			}
			bool? flag6 = ConvertDundasCRIBoolProperty(scaleProperties[propertyPrefix + "LabelStyle.AllowUpsideDown"], ref counter);
			if (flag6.HasValue)
			{
				scaleLabels.AllowUpsideDown = flag6.Value;
			}
			bool? flag7 = ConvertDundasCRIBoolProperty(scaleProperties[propertyPrefix + "LabelStyle.ShowEndLabels"], ref counter);
			if (flag7.HasValue)
			{
				scaleLabels.ShowEndLabels = flag7.Value;
			}
			else
			{
				scaleLabels.ShowEndLabels = true;
			}
			if (ConvertDundasCRIStringProperty("Percent", scaleProperties[propertyPrefix + "LabelStyle.FontUnit"], ref counter) == "Percent")
			{
				scaleLabels.UseFontPercent = true;
			}
			scaleLabels.Style = ConvertDundasCRIStyleProperty(scaleProperties[propertyPrefix + "LabelStyle.TextColor"], null, null, null, null, null, null, null, null, scaleProperties.ContainsKey(propertyPrefix + "LabelStyle.Font") ? scaleProperties[propertyPrefix + "LabelStyle.Font"] : "Microsoft Sans Serif, 14pt", scaleProperties[propertyPrefix + "LabelStyle.FormatString"], null, null, ref counter);
			if (counter > 0)
			{
				scale.ScaleLabels = scaleLabels;
			}
			List<Hashtable> list = new List<Hashtable>();
			foreach (DictionaryEntry scaleProperty in scaleProperties)
			{
				AddToPropertyList(list, propertyPrefix + "CustomLabels.", scaleProperty.Key.ToString(), scaleProperty.Value.ToString());
			}
			foreach (Hashtable item in list)
			{
				CustomLabel customLabel = new CustomLabel();
				UpgradeDundasCRIGaugeCustomLabel(customLabel, item, "CustomLabel.");
				scale.CustomLabels.Add(customLabel);
			}
		}

		private void UpgradeDundasCRIGaugeScaleRadial(RadialScale scale, Hashtable scaleProperties, string propertyPrefix, Hashtable formulaProperties, Hashtable dataValueProperties)
		{
			UpgradeDundasCRIGaugeScale(scale, scaleProperties, propertyPrefix, formulaProperties, dataValueProperties);
			scale.Radius = ConvertDundasCRIDoubleReportExpressionProperty(scale.Radius, scaleProperties[propertyPrefix + "Radius"]);
			scale.StartAngle = ConvertDundasCRIDoubleReportExpressionProperty(scale.StartAngle, scaleProperties[propertyPrefix + "StartAngle"]);
			scale.SweepAngle = ConvertDundasCRIDoubleReportExpressionProperty(scale.SweepAngle, scaleProperties[propertyPrefix + "SweepAngle"]);
			bool? flag = ConvertDundasCRIBoolProperty(scaleProperties[propertyPrefix + "LabelStyle.RotateLabels"]);
			if (!flag.HasValue)
			{
				flag = true;
			}
			if (flag.Value)
			{
				if (scale.ScaleLabels == null)
				{
					scale.ScaleLabels = new ScaleLabels();
				}
				scale.ScaleLabels.RotateLabels = true;
			}
			GaugeTickMarks gaugeTickMarks = new GaugeTickMarks();
			if (UpgradeDundasCRIGaugeTickMarks(gaugeTickMarks, scaleProperties, propertyPrefix + "MajorTickMark.", 8.0, 14.0, MarkerStyles.Trapezoid))
			{
				scale.GaugeMajorTickMarks = gaugeTickMarks;
			}
			GaugeTickMarks gaugeTickMarks2 = new GaugeTickMarks();
			if (UpgradeDundasCRIGaugeTickMarks(gaugeTickMarks2, scaleProperties, propertyPrefix + "MinorTickMark.", 3.0, 8.0, MarkerStyles.Rectangle))
			{
				scale.GaugeMinorTickMarks = gaugeTickMarks2;
			}
		}

		private void UpgradeDundasCRIGaugeScaleLinear(LinearScale scale, Hashtable scaleProperties, string propertyPrefix, Hashtable formulaProperties, Hashtable dataValueProperties)
		{
			UpgradeDundasCRIGaugeScale(scale, scaleProperties, propertyPrefix, formulaProperties, dataValueProperties);
			scale.StartMargin = ConvertDundasCRIDoubleReportExpressionProperty(scale.StartMargin, scaleProperties[propertyPrefix + "StartMargin"]);
			scale.EndMargin = ConvertDundasCRIDoubleReportExpressionProperty(scale.EndMargin, scaleProperties[propertyPrefix + "EndMargin"]);
			scale.Position = ConvertDundasCRIDoubleReportExpressionProperty(scale.Position, scaleProperties[propertyPrefix + "Position"]);
			GaugeTickMarks gaugeTickMarks = new GaugeTickMarks();
			if (UpgradeDundasCRIGaugeTickMarks(gaugeTickMarks, scaleProperties, propertyPrefix + "MajorTickMark.", 4.0, 15.0, MarkerStyles.Rectangle))
			{
				scale.GaugeMajorTickMarks = gaugeTickMarks;
			}
			GaugeTickMarks gaugeTickMarks2 = new GaugeTickMarks();
			if (UpgradeDundasCRIGaugeTickMarks(gaugeTickMarks2, scaleProperties, propertyPrefix + "MinorTickMark.", 3.0, 9.0, MarkerStyles.Rectangle))
			{
				scale.GaugeMinorTickMarks = gaugeTickMarks2;
			}
		}

		private void UpgradeDundasCRIGaugeScaleRange(ScaleRange range, Hashtable rangeProperties, string propertyPrefix, bool isLinear, Hashtable formulaProperties, Hashtable dataValueProperties)
		{
			string str2 = string.Concat(str2: range.Name = ConvertDundasCRIStringProperty(rangeProperties[propertyPrefix + "Name"]), str0: isLinear ? "LinearRange" : "CircularRange", str1: ":", str3: ":");
			range.StartValue = UpgradeDundasCRIGaugeInputValue(formulaProperties, dataValueProperties, str2 + "StartValue", rangeProperties, propertyPrefix + "StartValue");
			range.EndValue = UpgradeDundasCRIGaugeInputValue(formulaProperties, dataValueProperties, str2 + "EndValue", rangeProperties, propertyPrefix + "EndValue");
			range.StartWidth = ConvertDundasCRIDoubleReportExpressionProperty(isLinear ? 10.0 : 15.0, rangeProperties[propertyPrefix + "StartWidth"]);
			range.EndWidth = ConvertDundasCRIDoubleReportExpressionProperty(isLinear ? 10.0 : 30.0, rangeProperties[propertyPrefix + "EndWidth"]);
			range.DistanceFromScale = ConvertDundasCRIDoubleReportExpressionProperty(isLinear ? 10.0 : 30.0, rangeProperties[propertyPrefix + "DistanceFromScale"]);
			range.Placement = new ReportExpression<Placements>(ConvertDundasCRIStringProperty(isLinear ? Placements.Outside.ToString() : Placements.Inside.ToString(), rangeProperties[propertyPrefix + "Placement"]), CultureInfo.InvariantCulture);
			range.InRangeTickMarksColor = ConvertDundasCRIColorProperty(range.InRangeTickMarksColor, rangeProperties[propertyPrefix + "InRangeTickMarkColor"]);
			range.InRangeLabelColor = ConvertDundasCRIColorProperty(range.InRangeLabelColor, rangeProperties[propertyPrefix + "InRangeLabelColor"]);
			range.InRangeBarPointerColor = ConvertDundasCRIColorProperty(range.InRangeBarPointerColor, rangeProperties[propertyPrefix + "InRangeBarPointerColor"]);
			range.BackgroundGradientType = new ReportExpression<GaugeBackgroundGradients>(ConvertDundasCRIStringProperty(rangeProperties[propertyPrefix + "FillGradientType"]), CultureInfo.InvariantCulture);
			bool? flag = ConvertDundasCRIBoolProperty(rangeProperties[propertyPrefix + "Visible"]);
			if (flag.HasValue)
			{
				range.Hidden = !flag.Value;
			}
			range.ActionInfo = ConvertDundasCRIActionInfoProperty(rangeProperties[propertyPrefix + "Href"]);
			range.Style = ConvertDundasCRIStyleProperty(null, rangeProperties[propertyPrefix + "FillColor"] ?? ((object)Color.Lime), null, rangeProperties[propertyPrefix + "FillGradientEndColor"] ?? ((object)Color.Red), rangeProperties[propertyPrefix + "FillHatchStyle"], rangeProperties[propertyPrefix + "ShadowOffset"], rangeProperties[propertyPrefix + "BorderColor"], rangeProperties[propertyPrefix + "BorderStyle"], rangeProperties[propertyPrefix + "BorderWidth"], null, null, null, null);
		}

		private bool UpgradeDundasCRIGaugeTickMarkStyle(TickMarkStyle tickMarkStyle, Hashtable properties, string propertyPrefix, ReportExpression<double> defaultWidth, ReportExpression<double> defaultLength, MarkerStyles? defaultShape)
		{
			int counter = 0;
			tickMarkStyle.Shape = new ReportExpression<MarkerStyles>(ConvertDundasCRIStringProperty(defaultShape.HasValue ? defaultShape.Value.ToString() : string.Empty, properties[propertyPrefix + "Shape"], ref counter), CultureInfo.InvariantCulture);
			tickMarkStyle.Placement = new ReportExpression<Placements>(ConvertDundasCRIStringProperty(Placements.Cross.ToString(), properties[propertyPrefix + "Placement"], ref counter), CultureInfo.InvariantCulture);
			tickMarkStyle.GradientDensity = ConvertDundasCRIDoubleReportExpressionProperty(tickMarkStyle.GradientDensity, properties[propertyPrefix + "GradientDensity"], ref counter);
			tickMarkStyle.DistanceFromScale = ConvertDundasCRIDoubleReportExpressionProperty(properties[propertyPrefix + "DistanceFromScale"], ref counter);
			tickMarkStyle.Width = ConvertDundasCRIDoubleReportExpressionProperty(defaultWidth, properties[propertyPrefix + "Width"], ref counter);
			tickMarkStyle.Length = ConvertDundasCRIDoubleReportExpressionProperty(defaultLength, properties[propertyPrefix + "Length"], ref counter);
			bool? flag = ConvertDundasCRIBoolProperty(properties[propertyPrefix + "Visible"], ref counter);
			if (flag.HasValue)
			{
				tickMarkStyle.Hidden = !flag.Value;
			}
			bool? flag2 = ConvertDundasCRIBoolProperty(properties[propertyPrefix + "EnableGradient"], ref counter);
			if (flag2.HasValue)
			{
				tickMarkStyle.EnableGradient = flag2.Value;
			}
			else
			{
				tickMarkStyle.EnableGradient = true;
			}
			tickMarkStyle.Style = ConvertDundasCRIStyleProperty(null, properties[propertyPrefix + "FillColor"] ?? ((object)Color.WhiteSmoke), null, null, null, null, properties[propertyPrefix + "BorderColor"] ?? ((object)Color.DimGray), null, properties[propertyPrefix + "BorderWidth"], null, null, null, null, ref counter);
			return counter > 0;
		}

		private bool UpgradeDundasCRIGaugeTickMarks(GaugeTickMarks tickMarks, Hashtable properties, string propertyPrefix, ReportExpression<double> defaultWidth, ReportExpression<double> defaultLength, MarkerStyles defaultShape)
		{
			int counter = 0;
			if (UpgradeDundasCRIGaugeTickMarkStyle(tickMarks, properties, propertyPrefix, defaultWidth, defaultLength, defaultShape))
			{
				counter++;
			}
			tickMarks.Interval = ConvertDundasCRIDoubleReportExpressionProperty(properties[propertyPrefix + "Interval"], ref counter);
			tickMarks.IntervalOffset = ConvertDundasCRIDoubleReportExpressionProperty(double.NaN, properties[propertyPrefix + "IntervalOffset"], ref counter);
			return counter > 0;
		}

		private bool UpgradeDundasCRIGaugeScalePin(ScalePin pin, Hashtable properties, string propertyPrefix, ReportExpression<double> defaultWidth, ReportExpression<double> defaultLength)
		{
			int counter = 0;
			if (UpgradeDundasCRIGaugeTickMarkStyle(pin, properties, propertyPrefix, defaultWidth, defaultLength, MarkerStyles.Circle))
			{
				counter++;
			}
			pin.Location = ConvertDundasCRIDoubleReportExpressionProperty(pin.Location, properties[propertyPrefix + "Location"], ref counter);
			bool? flag = ConvertDundasCRIBoolProperty(properties[propertyPrefix + "Enable"], ref counter);
			if (flag.HasValue)
			{
				pin.Enable = flag.Value;
			}
			int counter2 = 0;
			PinLabel pinLabel = new PinLabel();
			pinLabel.Placement = new ReportExpression<Placements>(ConvertDundasCRIStringProperty(Placements.Inside.ToString(), properties[propertyPrefix + "LabelStyle.Placement"], ref counter2), CultureInfo.InvariantCulture);
			pinLabel.Text = ConvertDundasCRIStringProperty(properties[propertyPrefix + "LabelStyle.Text"], ref counter2);
			pinLabel.FontAngle = ConvertDundasCRIDoubleReportExpressionProperty(properties[propertyPrefix + "LabelStyle.FontAngle"], ref counter2);
			pinLabel.DistanceFromScale = ConvertDundasCRIDoubleReportExpressionProperty(pinLabel.DistanceFromScale, properties[propertyPrefix + "LabelStyle.DistanceFromScale"], ref counter2);
			bool? flag2 = ConvertDundasCRIBoolProperty(properties[propertyPrefix + "LabelStyle.RotateLabel"], ref counter2);
			if (flag2.HasValue)
			{
				pinLabel.RotateLabel = flag2.Value;
			}
			bool? flag3 = ConvertDundasCRIBoolProperty(properties[propertyPrefix + "LabelStyle.AllowUpsideDown"], ref counter2);
			if (flag3.HasValue)
			{
				pinLabel.AllowUpsideDown = flag3.Value;
			}
			if (ConvertDundasCRIStringProperty("Percent", properties[propertyPrefix + "LabelStyle.FontUnit"], ref counter2) == "Percent")
			{
				pinLabel.UseFontPercent = true;
			}
			pinLabel.Style = ConvertDundasCRIStyleProperty(properties[propertyPrefix + "LabelStyle.TextColor"], null, null, null, null, null, null, null, null, properties.ContainsKey(propertyPrefix + "LabelStyle.Font") ? properties[propertyPrefix + "LabelStyle.Font"] : "Microsoft Sans Serif, 12pt", null, null, null, ref counter2);
			if (counter2 > 0)
			{
				pin.PinLabel = pinLabel;
				counter++;
			}
			return counter > 0;
		}

		private void UpgradeDundasCRIGaugeCustomLabel(CustomLabel customLabel, Hashtable properties, string propertyPrefix)
		{
			customLabel.Name = ConvertDundasCRIStringProperty(properties[propertyPrefix + "Name"]);
			customLabel.Text = ConvertDundasCRIStringProperty(properties[propertyPrefix + "Text"]);
			customLabel.Value = ConvertDundasCRIStringProperty(properties[propertyPrefix + "Value"]);
			customLabel.Placement = new ReportExpression<Placements>(ConvertDundasCRIStringProperty(Placements.Inside.ToString(), properties[propertyPrefix + "Placement"]), CultureInfo.InvariantCulture);
			customLabel.FontAngle = ConvertDundasCRIDoubleReportExpressionProperty(properties[propertyPrefix + "FontAngle"]);
			customLabel.DistanceFromScale = ConvertDundasCRIDoubleReportExpressionProperty(properties[propertyPrefix + "DistanceFromScale"]);
			bool? flag = ConvertDundasCRIBoolProperty(properties[propertyPrefix + "Visible"]);
			if (flag.HasValue)
			{
				customLabel.Hidden = !flag.Value;
			}
			bool? flag2 = ConvertDundasCRIBoolProperty(properties[propertyPrefix + "RotateLabel"]);
			if (flag2.HasValue)
			{
				customLabel.RotateLabel = flag2.Value;
			}
			bool? flag3 = ConvertDundasCRIBoolProperty(properties[propertyPrefix + "AllowUpsideDown"]);
			if (flag3.HasValue)
			{
				customLabel.AllowUpsideDown = flag3.Value;
			}
			if (ConvertDundasCRIStringProperty("Percent", properties[propertyPrefix + "FontUnit"]) == "Percent")
			{
				customLabel.UseFontPercent = true;
			}
			customLabel.Style = ConvertDundasCRIStyleProperty(properties[propertyPrefix + "TextColor"], null, null, null, null, null, null, null, null, properties.ContainsKey(propertyPrefix + "Font") ? properties[propertyPrefix + "Font"] : "Microsoft Sans Serif, 14pt", null, null, null);
			TickMarkStyle tickMarkStyle = new TickMarkStyle();
			if (UpgradeDundasCRIGaugeTickMarkStyle(tickMarkStyle, properties, propertyPrefix + "TickMarkStyle.", 3.0, null, null))
			{
				customLabel.TickMarkStyle = tickMarkStyle;
			}
		}

		private void UpgradeDundasCRIGaugePointer(GaugePointer pointer, Hashtable properties, string propertyPrefix, ReportExpression<double> defaultWidth, ReportExpression<double> defaultMarkerLength, Hashtable formulaProperties, Hashtable dataValueProperties)
		{
			string dataValuePropertyKey = string.Concat(str2: pointer.Name = ConvertDundasCRIStringProperty(properties[propertyPrefix + "Name"]), str0: (pointer is LinearPointer) ? "LinearPointer" : "CircularPointer", str1: ":");
			pointer.GaugeInputValue = UpgradeDundasCRIGaugeInputValue(formulaProperties, dataValueProperties, dataValuePropertyKey, properties, propertyPrefix + "Value");
			pointer.BarStart = new ReportExpression<BarStartTypes>(ConvertDundasCRIStringProperty(properties[propertyPrefix + "BarStart"]), CultureInfo.InvariantCulture);
			pointer.Width = ConvertDundasCRIDoubleReportExpressionProperty(defaultWidth, properties[propertyPrefix + "Width"]);
			pointer.MarkerLength = ConvertDundasCRIDoubleReportExpressionProperty(defaultMarkerLength, properties[propertyPrefix + "MarkerLength"]);
			pointer.DistanceFromScale = ConvertDundasCRIDoubleReportExpressionProperty(properties[propertyPrefix + "DistanceFromScale"]);
			bool? flag = ConvertDundasCRIBoolProperty(properties[propertyPrefix + "Visible"]);
			if (flag.HasValue)
			{
				pointer.Hidden = !flag.Value;
			}
		}

		private void UpgradeDundasCRIGaugePointerLinear(LinearPointer pointer, Hashtable properties, string propertyPrefix, Hashtable formulaProperties, Hashtable dataValueProperties)
		{
			UpgradeDundasCRIGaugePointer(pointer, properties, propertyPrefix, 20.0, 20.0, formulaProperties, dataValueProperties);
			pointer.MarkerStyle = new ReportExpression<MarkerStyles>(ConvertDundasCRIStringProperty(MarkerStyles.Triangle.ToString(), properties[propertyPrefix + "MarkerStyle"]), CultureInfo.InvariantCulture);
			pointer.Type = new ReportExpression<LinearPointerTypes>(ConvertDundasCRIStringProperty(properties[propertyPrefix + "Type"]), CultureInfo.InvariantCulture);
			pointer.Placement = new ReportExpression<Placements>(ConvertDundasCRIStringProperty(Placements.Outside.ToString(), properties[propertyPrefix + "Placement"]), CultureInfo.InvariantCulture);
			int counter = 0;
			Thermometer thermometer = new Thermometer();
			thermometer.ThermometerStyle = new ReportExpression<ThermometerStyles>(ConvertDundasCRIStringProperty(properties[propertyPrefix + "ThermometerStyle"], ref counter), CultureInfo.InvariantCulture);
			thermometer.BulbOffset = ConvertDundasCRIDoubleReportExpressionProperty(thermometer.BulbOffset, properties[propertyPrefix + "ThermometerBulbOffset"], ref counter);
			thermometer.BulbSize = ConvertDundasCRIDoubleReportExpressionProperty(thermometer.BulbSize, properties[propertyPrefix + "ThermometerBulbSize"], ref counter);
			thermometer.Style = ConvertDundasCRIStyleProperty(null, properties[propertyPrefix + "ThermometerBackColor"], properties[propertyPrefix + "ThermometerBackGradientType"], properties[propertyPrefix + "ThermometerBackGradientEndColor"], properties[propertyPrefix + "ThermometerBackHatchStyle"], null, null, null, null, null, null, null, null, ref counter);
			pointer.Style = ConvertDundasCRIStyleProperty(null, properties[propertyPrefix + "FillColor"] ?? ((object)Color.White), properties[propertyPrefix + "FillGradientType"] ?? ((object)BackgroundGradients.DiagonalLeft), properties[propertyPrefix + "FillGradientEndColor"] ?? ((object)Color.Red), properties[propertyPrefix + "FillHatchStyle"], properties[propertyPrefix + "ShadowOffset"] ?? ((object)2), properties[propertyPrefix + "BorderColor"], properties[propertyPrefix + "BorderStyle"] ?? ((object)BorderStyles.Solid), properties[propertyPrefix + "BorderWidth"], null, null, null, null);
			if (counter > 0)
			{
				pointer.Thermometer = thermometer;
			}
		}

		private void UpgradeDundasCRIGaugePointerRadial(RadialPointer pointer, Hashtable properties, string propertyPrefix, Hashtable formulaProperties, Hashtable dataValueProperties)
		{
			UpgradeDundasCRIGaugePointer(pointer, properties, propertyPrefix, 15.0, 10.0, formulaProperties, dataValueProperties);
			pointer.MarkerStyle = new ReportExpression<MarkerStyles>(ConvertDundasCRIStringProperty(MarkerStyles.Diamond.ToString(), properties[propertyPrefix + "MarkerStyle"]), CultureInfo.InvariantCulture);
			pointer.Type = new ReportExpression<RadialPointerTypes>(ConvertDundasCRIStringProperty(properties[propertyPrefix + "Type"]), CultureInfo.InvariantCulture);
			pointer.Placement = new ReportExpression<Placements>(ConvertDundasCRIStringProperty(Placements.Cross.ToString(), properties[propertyPrefix + "Placement"]), CultureInfo.InvariantCulture);
			pointer.NeedleStyle = ConvertDundasCRIGaugeNeedleStyles(ConvertDundasCRIStringProperty(properties[propertyPrefix + "NeedleStyle"]));
			int counter = 0;
			PointerCap pointerCap = new PointerCap();
			pointerCap.Width = ConvertDundasCRIDoubleReportExpressionProperty(pointerCap.Width, properties[propertyPrefix + "CapWidth"], ref counter);
			pointerCap.CapStyle = ConvertDundasCRIGaugeCapStyle(ConvertDundasCRIStringProperty(properties[propertyPrefix + "CapStyle"], ref counter));
			bool? flag = ConvertDundasCRIBoolProperty(properties[propertyPrefix + "CapVisible"], ref counter);
			if (flag.HasValue)
			{
				pointerCap.Hidden = !flag.Value;
			}
			bool? flag2 = ConvertDundasCRIBoolProperty(properties[propertyPrefix + "CapOnTop"], ref counter);
			if (flag2.HasValue)
			{
				pointerCap.OnTop = flag2.Value;
			}
			else
			{
				pointerCap.OnTop = true;
			}
			bool? flag3 = ConvertDundasCRIBoolProperty(properties[propertyPrefix + "CapReflection"], ref counter);
			if (flag3.HasValue)
			{
				pointerCap.Reflection = flag3.Value;
			}
			pointerCap.Style = ConvertDundasCRIStyleProperty(null, properties[propertyPrefix + "CapFillColor"] ?? ((object)Color.Gainsboro), properties[propertyPrefix + "CapFillGradientType"] ?? ((object)BackgroundGradients.DiagonalLeft), properties[propertyPrefix + "CapFillGradientEndColor"] ?? ((object)Color.DimGray), properties[propertyPrefix + "CapFillHatchStyle"], null, null, null, null, null, null, null, null, ref counter);
			pointer.Style = ConvertDundasCRIStyleProperty(null, properties[propertyPrefix + "FillColor"] ?? ((object)Color.White), properties[propertyPrefix + "FillGradientType"] ?? ((object)BackgroundGradients.LeftRight), properties[propertyPrefix + "FillGradientEndColor"] ?? ((object)Color.Red), properties[propertyPrefix + "FillHatchStyle"], properties[propertyPrefix + "ShadowOffset"] ?? ((object)2), properties[propertyPrefix + "BorderColor"], properties[propertyPrefix + "BorderStyle"] ?? ((object)BorderStyles.Solid), properties[propertyPrefix + "BorderWidth"], null, null, null, null);
			if (counter > 0)
			{
				pointer.PointerCap = pointerCap;
			}
		}

		private GaugeInputValue UpgradeDundasCRIGaugeInputValue(Hashtable formulaProperties, Hashtable dataValueProperties, string dataValuePropertyKey, Hashtable customProperties, string customPropertyKey)
		{
			GaugeInputValue gaugeInputValue = null;
			if (formulaProperties.Contains(dataValuePropertyKey))
			{
				if (gaugeInputValue == null)
				{
					gaugeInputValue = new GaugeInputValue();
				}
				string text = ConvertDundasCRIStringProperty(formulaProperties[dataValuePropertyKey]);
				int num = text.IndexOf('(');
				int num2 = text.LastIndexOf(')');
				string text2 = (num > -1 && num < num2) ? text.Substring(num + 1, num2 - num - 1) : string.Empty;
				switch (((num > -1) ? text.Remove(num) : text).ToUpperInvariant())
				{
				case "CALCULATEDVALUEAVERAGE":
					gaugeInputValue.Formula = FormulaTypes.Average;
					break;
				case "CALCULATEDVALUELINEAR":
					gaugeInputValue.Formula = FormulaTypes.Linear;
					break;
				case "CALCULATEDVALUEMAX":
					gaugeInputValue.Formula = FormulaTypes.Max;
					break;
				case "CALCULATEDVALUEMIN":
					gaugeInputValue.Formula = FormulaTypes.Min;
					break;
				case "MEDIAN":
					gaugeInputValue.Formula = FormulaTypes.Median;
					break;
				case "OPENCLOSE":
					gaugeInputValue.Formula = FormulaTypes.OpenClose;
					break;
				case "PERCENTILE":
					gaugeInputValue.Formula = FormulaTypes.Percentile;
					break;
				case "VARIANCE":
					gaugeInputValue.Formula = FormulaTypes.Variance;
					break;
				case "CALCULATEDVALUERATEOFCHANGE":
					gaugeInputValue.Formula = FormulaTypes.RateOfChange;
					break;
				case "CALCULATEDVALUEINTEGRAL":
					gaugeInputValue.Formula = FormulaTypes.Integral;
					break;
				}
				if (!string.IsNullOrEmpty(text2))
				{
					string[] array = text2.Split(',');
					if (gaugeInputValue.Formula == FormulaTypes.Percentile)
					{
						if (array.Length != 0)
						{
							gaugeInputValue.MinPercent = new ReportExpression<double>(array[0], CultureInfo.InvariantCulture);
						}
						if (array.Length > 1)
						{
							gaugeInputValue.MaxPercent = new ReportExpression<double>(array[1], CultureInfo.InvariantCulture);
						}
					}
					else if (gaugeInputValue.Formula == FormulaTypes.Linear)
					{
						if (array.Length != 0)
						{
							gaugeInputValue.Multiplier = new ReportExpression<double>(array[0], CultureInfo.InvariantCulture);
						}
						if (array.Length > 1)
						{
							gaugeInputValue.AddConstant = new ReportExpression<double>(array[1], CultureInfo.InvariantCulture);
						}
					}
				}
			}
			if (dataValueProperties.Contains(dataValuePropertyKey))
			{
				if (gaugeInputValue == null)
				{
					gaugeInputValue = new GaugeInputValue();
				}
				gaugeInputValue.Value = ConvertDundasCRIStringProperty(dataValueProperties[dataValuePropertyKey]);
			}
			else if (customProperties.Contains(customPropertyKey))
			{
				if (gaugeInputValue == null)
				{
					gaugeInputValue = new GaugeInputValue();
				}
				gaugeInputValue.Value = ConvertDundasCRIStringProperty(customProperties[customPropertyKey]);
			}
			return gaugeInputValue;
		}

		private ActionInfo UpgradeDundasCRIGaugeActionInfo(Hashtable formulaProperties, string propertyPrefix)
		{
			return UpgradeDundasCRIActionInfo(formulaProperties, propertyPrefix, "Href");
		}

		private ActionInfo UpgradeDundasCRIActionInfo(Hashtable properties, string propertyPrefix, string hyperLinkKey)
		{
			ActionInfo actionInfo = null;
			object obj = properties[propertyPrefix + hyperLinkKey];
			if (obj != null)
			{
				string text = ConvertDundasCRIStringProperty(properties[propertyPrefix + "MapAreaType"]);
				actionInfo = new ActionInfo();
				Microsoft.ReportingServices.RdlObjectModel.Action action = new Microsoft.ReportingServices.RdlObjectModel.Action();
				actionInfo.Actions.Add(action);
				switch (text)
				{
				case "Url":
					action.Hyperlink = ConvertDundasCRIStringProperty(obj);
					break;
				case "Bookmark":
					action.BookmarkLink = ConvertDundasCRIStringProperty(obj);
					break;
				case "Report":
				{
					action.Drillthrough = new Drillthrough();
					action.Drillthrough.ReportName = ConvertDundasCRIStringProperty(obj);
					string text2 = propertyPrefix + "REPORTPARAM:";
					{
						foreach (DictionaryEntry property in properties)
						{
							if (property.Key.ToString().StartsWith(text2, StringComparison.Ordinal))
							{
								Parameter parameter = new Parameter();
								parameter.Name = property.Key.ToString().Remove(0, text2.Length);
								parameter.Value = property.Value.ToString();
								action.Drillthrough.Parameters.Add(parameter);
							}
						}
						return actionInfo;
					}
				}
				}
			}
			return actionInfo;
		}

		private void FixGaugeElementNames(GaugePanel gaugePanel)
		{
			StringCollection stringCollection = new StringCollection();
			OrderedDictionary orderedDictionary = new OrderedDictionary(gaugePanel.RadialGauges.Count);
			OrderedDictionary orderedDictionary2 = new OrderedDictionary(gaugePanel.LinearGauges.Count);
			OrderedDictionary orderedDictionary3 = new OrderedDictionary(gaugePanel.GaugeLabels.Count);
			foreach (RadialGauge radialGauge in gaugePanel.RadialGauges)
			{
				string text = CreateNewName(stringCollection, radialGauge.Name, "Default");
				stringCollection.Add(text);
				if (!orderedDictionary.Contains(radialGauge.Name))
				{
					orderedDictionary.Add(radialGauge.Name, text);
				}
				radialGauge.Name = text;
				FixGaugeSubElementNames(radialGauge);
			}
			stringCollection.Clear();
			foreach (LinearGauge linearGauge in gaugePanel.LinearGauges)
			{
				string text2 = CreateNewName(stringCollection, linearGauge.Name, "Default");
				stringCollection.Add(text2);
				if (!orderedDictionary2.Contains(linearGauge.Name))
				{
					orderedDictionary2.Add(linearGauge.Name, text2);
				}
				linearGauge.Name = text2;
				FixGaugeSubElementNames(linearGauge);
			}
			stringCollection.Clear();
			foreach (GaugeLabel gaugeLabel in gaugePanel.GaugeLabels)
			{
				string text3 = CreateNewName(stringCollection, gaugeLabel.Name, "Default");
				stringCollection.Add(text3);
				if (!orderedDictionary3.Contains(gaugeLabel.Name))
				{
					orderedDictionary3.Add(gaugeLabel.Name, text3);
				}
				gaugeLabel.Name = text3;
			}
			foreach (RadialGauge radialGauge2 in gaugePanel.RadialGauges)
			{
				FixGaugeElementParentItemNames(radialGauge2, orderedDictionary, orderedDictionary2, orderedDictionary3);
			}
			foreach (LinearGauge linearGauge2 in gaugePanel.LinearGauges)
			{
				FixGaugeElementParentItemNames(linearGauge2, orderedDictionary, orderedDictionary2, orderedDictionary3);
			}
			foreach (GaugeLabel gaugeLabel2 in gaugePanel.GaugeLabels)
			{
				FixGaugeElementParentItemNames(gaugeLabel2, orderedDictionary, orderedDictionary2, orderedDictionary3);
			}
		}

		private void FixGaugeSubElementNames(Gauge gauge)
		{
			StringCollection stringCollection = new StringCollection();
			foreach (GaugeScale gaugeScale in gauge.GaugeScales)
			{
				gaugeScale.Name = CreateNewName(stringCollection, gaugeScale.Name, "Default");
				stringCollection.Add(gaugeScale.Name);
				StringCollection stringCollection2 = new StringCollection();
				foreach (ScaleRange scaleRange in gaugeScale.ScaleRanges)
				{
					scaleRange.Name = CreateNewName(stringCollection2, scaleRange.Name, "Default");
					stringCollection2.Add(scaleRange.Name);
				}
				stringCollection2.Clear();
				foreach (GaugePointer gaugePointer in gaugeScale.GaugePointers)
				{
					gaugePointer.Name = CreateNewName(stringCollection2, gaugePointer.Name, "Default");
					stringCollection2.Add(gaugePointer.Name);
				}
				stringCollection2.Clear();
				foreach (CustomLabel customLabel in gaugeScale.CustomLabels)
				{
					customLabel.Name = CreateNewName(stringCollection2, customLabel.Name, "Default");
					stringCollection2.Add(customLabel.Name);
				}
			}
		}

		private void FixGaugeElementParentItemNames(GaugePanelItem gaugeElement, OrderedDictionary radialGaugeNameMapping, OrderedDictionary linearGaugeNameMapping, OrderedDictionary gaugeLabelNameMapping)
		{
			string text = string.Empty;
			if (gaugeElement.ParentItem.StartsWith("CircularGauges.", StringComparison.Ordinal))
			{
				text = GetNewName(radialGaugeNameMapping, gaugeElement.ParentItem.Substring("CircularGauges.".Length));
				if (!string.IsNullOrEmpty(text))
				{
					text = "RadialGauges." + text;
				}
			}
			else if (gaugeElement.ParentItem.StartsWith("LinearGauges.", StringComparison.Ordinal))
			{
				text = GetNewName(linearGaugeNameMapping, gaugeElement.ParentItem.Substring("LinearGauges.".Length));
				if (!string.IsNullOrEmpty(text))
				{
					text = "LinearGauges." + text;
				}
			}
			else if (gaugeElement is GaugeLabel && gaugeElement.ParentItem.StartsWith("GaugeLabels.", StringComparison.Ordinal))
			{
				text = GetNewName(gaugeLabelNameMapping, gaugeElement.ParentItem.Substring("GaugeLabels.".Length));
				if (!string.IsNullOrEmpty(text))
				{
					text = "GaugeLabels." + text;
				}
			}
			gaugeElement.ParentItem = text;
		}

		private CapStyles ConvertDundasCRIGaugeCapStyle(string capStyle)
		{
			switch (capStyle)
			{
			case "CustomCap1":
				return CapStyles.Rounded;
			case "CustomCap2":
				return CapStyles.RoundedLight;
			case "CustomCap3":
				return CapStyles.RoundedWithAdditionalTop;
			case "CustomCap4":
				return CapStyles.RoundedWithWideIndentation;
			case "CustomCap5":
				return CapStyles.FlattenedWithIndentation;
			case "CustomCap6":
				return CapStyles.FlattenedWithWideIndentation;
			case "CustomCap7":
				return CapStyles.RoundedGlossyWithIndentation;
			case "CustomCap8":
				return CapStyles.RoundedWithIndentation;
			default:
				return CapStyles.RoundedDark;
			}
		}

		private NeedleStyles ConvertDundasCRIGaugeNeedleStyles(string needleStyle)
		{
			switch (needleStyle)
			{
			case "NeedleStyle2":
				return NeedleStyles.Rectangular;
			case "NeedleStyle3":
				return NeedleStyles.TaperedWithTail;
			case "NeedleStyle4":
				return NeedleStyles.Tapered;
			case "NeedleStyle5":
				return NeedleStyles.ArrowWithTail;
			case "NeedleStyle6":
				return NeedleStyles.Arrow;
			case "NeedleStyle7":
				return NeedleStyles.StealthArrowWithTail;
			case "NeedleStyle8":
				return NeedleStyles.StealthArrow;
			case "NeedleStyle9":
				return NeedleStyles.TaperedWithStealthArrow;
			case "NeedleStyle10":
				return NeedleStyles.StealthArrowWithWideTail;
			case "NeedleStyle11":
				return NeedleStyles.TaperedWithRoundedPoint;
			default:
				return NeedleStyles.Triangular;
			}
		}

		private string GetNewName(OrderedDictionary oldAndNewNameMapping, string oldName)
		{
			if (oldAndNewNameMapping.Contains(oldName))
			{
				return oldAndNewNameMapping[oldName].ToString();
			}
			return string.Empty;
		}

		private string CreateNewName(StringCollection newNamesCollection, string oldName, string defaultNewName)
		{
			int num = 1;
			string text = (oldName.Trim() == string.Empty) ? (defaultNewName + num.ToString(CultureInfo.InvariantCulture)) : StringUtil.GetClsCompliantIdentifier(oldName, "chart");
			if (newNamesCollection.Contains(text))
			{
				while (newNamesCollection.Contains(text + "_" + num.ToString(CultureInfo.InvariantCulture)))
				{
					num++;
				}
				text = text + "_" + num.ToString(CultureInfo.InvariantCulture);
			}
			return text;
		}

		private Font FontFromString(string fontString)
		{
			string text = fontString;
			byte b = 1;
			bool flag = false;
			int num = fontString.IndexOf(", GdiCharSet=", StringComparison.Ordinal);
			if (num >= 0)
			{
				string text2 = fontString.Substring(num + 13);
				int num2 = text2.IndexOf(',');
				if (num2 >= 0)
				{
					text2 = text2.Substring(0, num2);
				}
				b = (byte)int.Parse(text2, CultureInfo.InvariantCulture);
				if (text.Length > num)
				{
					text = text.Substring(0, num);
				}
			}
			num = fontString.IndexOf(", GdiVerticalFont", StringComparison.Ordinal);
			if (num >= 0)
			{
				flag = true;
				if (text.Length > num)
				{
					text = text.Substring(0, num);
				}
			}
			Font font = (Font)new FontConverter().ConvertFromInvariantString(text);
			float sizeInPoints = font.SizeInPoints;
			sizeInPoints = Math.Min(Math.Max(font.SizeInPoints, (float)Microsoft.ReportingServices.RdlObjectModel.Constants.MinimumFontSize.ToPoints()), (float)Microsoft.ReportingServices.RdlObjectModel.Constants.MaximumFontSize.ToPoints());
			if (flag || b != 1 || sizeInPoints != font.SizeInPoints)
			{
				Font result = new Font(font.Name, sizeInPoints, font.Style, GraphicsUnit.Point, b, flag);
				font.Dispose();
				return result;
			}
			return font;
		}

		private bool AddToPropertyList(List<Hashtable> propertyList, string counterPrefix, string key, ReportExpression value)
		{
			if (!key.StartsWith(counterPrefix, StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			if (!counterPrefix.EndsWith(".", StringComparison.Ordinal))
			{
				counterPrefix += ".";
			}
			key = key.Substring(counterPrefix.Length);
			int num = key.IndexOf('.');
			int result = 0;
			if (!int.TryParse(key.Substring(0, num), out result))
			{
				return false;
			}
			key = key.Substring(num + 1);
			while (result >= propertyList.Count)
			{
				propertyList.Add(new Hashtable());
			}
			propertyList[result].Add(key, value);
			return true;
		}

		private bool IsZero(object value)
		{
			if (value == null)
			{
				return false;
			}
			if (double.TryParse(value.ToString(), out double result) && result == 0.0)
			{
				return true;
			}
			return false;
		}

		private void ConvertDundasCRICustomProperties(IList<CustomProperty> customProperties, object property)
		{
			int counter = 0;
			ConvertDundasCRICustomProperties(customProperties, property, ref counter);
		}

		private void ConvertDundasCRICustomProperties(IList<CustomProperty> customProperties, object property, ref int counter)
		{
			if (property != null)
			{
				counter++;
				if (customProperties != null)
				{
					customProperties.Clear();
				}
				else
				{
					customProperties = new List<CustomProperty>();
				}
				string[] array = property.ToString().Replace("\\,", "\\x45").Replace("\\=", "\\x46")
					.Split(new char[1]
					{
						','
					}, StringSplitOptions.RemoveEmptyEntries);
				foreach (string text in array)
				{
					int num = text.IndexOf('=');
					CustomProperty customProperty = new CustomProperty();
					customProperty.Name = text.Substring(0, num).Trim();
					customProperty.Value = text.Substring(num + 1).Replace("\\x45", ",").Replace("\\x46", "=")
						.Trim();
					customProperties.Add(customProperty);
				}
			}
		}

		private ReportExpression<ReportColor> ConvertDundasCRIColorProperty(string defaultValue, object color)
		{
			return ConvertDundasCRIColorProperty(new ReportExpression<ReportColor>(defaultValue, CultureInfo.InvariantCulture), color);
		}

		private ReportExpression<ReportColor> ConvertDundasCRIColorProperty(ReportExpression<ReportColor> defaultValue, object color)
		{
			int counter = 0;
			return ConvertDundasCRIColorProperty(defaultValue, color, ref counter);
		}

		private ReportExpression<ReportColor> ConvertDundasCRIColorProperty(ReportExpression<ReportColor> defaultValue, object color, ref int counter)
		{
			if (color != null)
			{
				counter++;
				if (color is ReportExpression<ReportColor>)
				{
					return (ReportExpression<ReportColor>)color;
				}
				ColorConverter colorConverter = new ColorConverter();
				try
				{
					Color color2 = (color is Color) ? ((Color)color) : ((Color)colorConverter.ConvertFromInvariantString(color.ToString()));
					if (color2.IsSystemColor)
					{
						return new ReportExpression<ReportColor>(new ReportColor(Color.FromArgb(color2.ToArgb())));
					}
					return new ReportExpression<ReportColor>(new ReportColor(color2));
				}
				catch
				{
					try
					{
						return new ReportExpression<ReportColor>(color.ToString(), CultureInfo.InvariantCulture);
					}
					catch
					{
						return defaultValue;
					}
				}
			}
			return defaultValue;
		}

		private bool? ConvertDundasCRIBoolProperty(object property)
		{
			int counter = 0;
			return ConvertDundasCRIBoolProperty(property, ref counter);
		}

		private bool? ConvertDundasCRIBoolProperty(object property, ref int counter)
		{
			if (property != null && bool.TryParse(property.ToString(), out bool result))
			{
				counter++;
				return result;
			}
			return null;
		}

		private string ConvertDundasCRIStringProperty(object property)
		{
			int counter = 0;
			return ConvertDundasCRIStringProperty(string.Empty, property, ref counter);
		}

		private string ConvertDundasCRIStringProperty(string defaultValue, object property)
		{
			int counter = 0;
			return ConvertDundasCRIStringProperty(defaultValue, property, ref counter);
		}

		private string ConvertDundasCRIStringProperty(object property, ref int counter)
		{
			return ConvertDundasCRIStringProperty(string.Empty, property, ref counter);
		}

		private string ConvertDundasCRIStringProperty(string defaultValue, object property, ref int counter)
		{
			if (property != null)
			{
				counter++;
				return property.ToString();
			}
			return defaultValue;
		}

		private ReportExpression<int> ConvertDundasCRIIntegerReportExpressionProperty(object property)
		{
			int counter = 0;
			return ConvertDundasCRIIntegerReportExpressionProperty(null, property, ref counter);
		}

		private ReportExpression<int> ConvertDundasCRIIntegerReportExpressionProperty(object property, ref int counter)
		{
			return ConvertDundasCRIIntegerReportExpressionProperty(null, property, ref counter);
		}

		private ReportExpression<int> ConvertDundasCRIIntegerReportExpressionProperty(ReportExpression<int> defaultValue, object property)
		{
			int counter = 0;
			return ConvertDundasCRIIntegerReportExpressionProperty(defaultValue, property, ref counter);
		}

		private ReportExpression<int> ConvertDundasCRIIntegerReportExpressionProperty(ReportExpression<int> defaultValue, object property, ref int counter)
		{
			if (property != null)
			{
				counter++;
				return new ReportExpression<int>(property.ToString(), CultureInfo.InvariantCulture.NumberFormat);
			}
			return defaultValue;
		}

		private ReportExpression<double> ConvertDundasCRIDoubleReportExpressionProperty(object property)
		{
			int counter = 0;
			return ConvertDundasCRIDoubleReportExpressionProperty(null, property, ref counter);
		}

		private ReportExpression<double> ConvertDundasCRIDoubleReportExpressionProperty(ReportExpression<double> defaultValue, object property)
		{
			int counter = 0;
			return ConvertDundasCRIDoubleReportExpressionProperty(defaultValue, property, ref counter);
		}

		private ReportExpression<double> ConvertDundasCRIDoubleReportExpressionProperty(object property, ref int counter)
		{
			return ConvertDundasCRIDoubleReportExpressionProperty(null, property, ref counter);
		}

		private ReportExpression<double> ConvertDundasCRIDoubleReportExpressionProperty(ReportExpression<double> defaultValue, object property, ref int counter)
		{
			if (property != null)
			{
				string a = property.ToString();
				counter++;
				if (a == "Auto")
				{
					return new ReportExpression<double>(double.NaN);
				}
				return new ReportExpression<double>(property.ToString(), CultureInfo.InvariantCulture.NumberFormat);
			}
			return defaultValue;
		}

		private ReportExpression<ReportSize> ConvertDundasCRIPointReportSizeProperty(ReportExpression<ReportSize> defaultValue, object property, ref int counter)
		{
			if (property != null)
			{
				string text = property.ToString();
				counter++;
				if (property is ReportExpression && ((ReportExpression)property).IsExpression)
				{
					return new ReportExpression<ReportSize>(text, CultureInfo.InvariantCulture);
				}
				double result = 0.0;
				if (!double.TryParse(text, out result))
				{
					return defaultValue;
				}
				return new ReportExpression<ReportSize>(new ReportSize(result, SizeTypes.Point));
			}
			return defaultValue;
		}

		private ReportExpression<ReportSize> ConvertDundasCRIPointReportSizeProperty(ReportExpression<ReportSize> defaultValue, object property)
		{
			int counter = 0;
			return ConvertDundasCRIPointReportSizeProperty(defaultValue, property, ref counter);
		}

		private ReportExpression<ReportSize> ConvertDundasCRIPixelReportSizeProperty(double? defaultValue, object property, ref int counter)
		{
			if (property != null)
			{
				string text = property.ToString();
				counter++;
				if (property is ReportExpression && ((ReportExpression)property).IsExpression)
				{
					text = text.Substring(text.IndexOf('=') + 1);
					return new ReportExpression<ReportSize>("=CStr(({0})*{1})&\"pt\"".Replace("{1}", 0.75.ToString(CultureInfo.InvariantCulture.NumberFormat)).Replace("{0}", text), CultureInfo.InvariantCulture);
				}
				double result = 0.0;
				if (double.TryParse(text, out result))
				{
					return new ReportExpression<ReportSize>(new ReportSize(result * 0.75, SizeTypes.Point));
				}
			}
			if (!defaultValue.HasValue)
			{
				return default(ReportExpression<ReportSize>);
			}
			return new ReportExpression<ReportSize>(new ReportSize(defaultValue.Value * 0.75, SizeTypes.Point));
		}

		private ReportExpression<ReportSize> ConvertDundasCRIPixelReportSizeProperty(object property, ref int counter)
		{
			return ConvertDundasCRIPixelReportSizeProperty(null, property, ref counter);
		}

		private ReportExpression<ReportSize> ConvertDundasCRIPixelReportSizeProperty(object property)
		{
			int counter = 0;
			return ConvertDundasCRIPixelReportSizeProperty(null, property, ref counter);
		}

		private ActionInfo ConvertDundasCRIActionInfoProperty(object hyperlink)
		{
			int counter = 0;
			return ConvertDundasCRIActionInfoProperty(hyperlink, ref counter);
		}

		private ActionInfo ConvertDundasCRIActionInfoProperty(object hyperlink, ref int counter)
		{
			if (hyperlink != null && hyperlink.ToString() != string.Empty)
			{
				Microsoft.ReportingServices.RdlObjectModel.Action action = new Microsoft.ReportingServices.RdlObjectModel.Action();
				action.Hyperlink = hyperlink.ToString();
				ActionInfo result = new ActionInfo
				{
					Actions = 
					{
						action
					}
				};
				counter++;
				return result;
			}
			return null;
		}

		private ChartElementPosition ConvertDundasCRIChartElementPosition(object top, object left, object height, object width)
		{
			int counter = 0;
			return ConvertDundasCRIChartElementPosition(top, left, height, width, ref counter);
		}

		private ChartElementPosition ConvertDundasCRIChartElementPosition(object top, object left, object height, object width, ref int counter)
		{
			int counter2 = 0;
			ChartElementPosition chartElementPosition = new ChartElementPosition();
			chartElementPosition.Top = ConvertDundasCRIDoubleReportExpressionProperty(top, ref counter2);
			chartElementPosition.Left = ConvertDundasCRIDoubleReportExpressionProperty(left, ref counter2);
			chartElementPosition.Height = ConvertDundasCRIDoubleReportExpressionProperty(height, ref counter2);
			chartElementPosition.Width = ConvertDundasCRIDoubleReportExpressionProperty(width, ref counter2);
			if (counter2 > 0)
			{
				counter++;
				return chartElementPosition;
			}
			return null;
		}

		private Microsoft.ReportingServices.RdlObjectModel.Style ConvertDundasCRIStyleProperty(object color, object backgroundColor, object backgroundGradientType, object backgroundGradientEndColor, object backgroundHatchType, object shadowOffset, object borderColor, object borderStyle, object borderWidth, object font, object format, object textAlign, object textVerticalAlign)
		{
			int counter = 0;
			return (Microsoft.ReportingServices.RdlObjectModel.Style)ConvertDundasCRIStyleProperty(color, backgroundColor, backgroundGradientType, backgroundGradientEndColor, backgroundHatchType, null, shadowOffset, borderColor, borderStyle, borderWidth, null, null, null, null, font, format, null, textAlign, textVerticalAlign, ref counter, new Microsoft.ReportingServices.RdlObjectModel.Style(), new Border());
		}

		private Microsoft.ReportingServices.RdlObjectModel.Style ConvertDundasCRIStyleProperty(object color, object backgroundColor, object backgroundGradientType, object backgroundGradientEndColor, object backgroundHatchType, object shadowOffset, object borderColor, object borderStyle, object borderWidth, object font, object format, object textAlign, object textVerticalAlign, ref int counter)
		{
			return (Microsoft.ReportingServices.RdlObjectModel.Style)ConvertDundasCRIStyleProperty(color, backgroundColor, backgroundGradientType, backgroundGradientEndColor, backgroundHatchType, null, shadowOffset, borderColor, borderStyle, borderWidth, null, null, null, null, font, format, null, textAlign, textVerticalAlign, ref counter, new Microsoft.ReportingServices.RdlObjectModel.Style(), new Border());
		}

		private Microsoft.ReportingServices.RdlObjectModel.Style ConvertDundasCRIStyleProperty(object color, object backgroundColor, object backgroundGradientType, object backgroundGradientEndColor, object backgroundHatchType, object shadowColor, object shadowOffset, object borderColor, object borderStyle, object borderWidth, object imageReference, object imageTransColor, object imageAlign, object imageMode, object font, object format, object textEffect, object textAlign, object textVerticalAlign)
		{
			int counter = 0;
			return (Microsoft.ReportingServices.RdlObjectModel.Style)ConvertDundasCRIStyleProperty(color, backgroundColor, backgroundGradientType, backgroundGradientEndColor, backgroundHatchType, shadowColor, shadowOffset, borderColor, borderStyle, borderWidth, imageReference, imageTransColor, imageAlign, imageMode, font, format, textEffect, textAlign, textVerticalAlign, ref counter, new Microsoft.ReportingServices.RdlObjectModel.Style(), new Border());
		}

		private Microsoft.ReportingServices.RdlObjectModel.Style ConvertDundasCRIStyleProperty(object color, object backgroundColor, object backgroundGradientType, object backgroundGradientEndColor, object backgroundHatchType, object shadowColor, object shadowOffset, object borderColor, object borderStyle, object borderWidth, object imageReference, object imageTransColor, object imageAlign, object imageMode, object font, object format, object textEffect, object textAlign, object textVerticalAlign, ref int counter)
		{
			return (Microsoft.ReportingServices.RdlObjectModel.Style)ConvertDundasCRIStyleProperty(color, backgroundColor, backgroundGradientType, backgroundGradientEndColor, backgroundHatchType, shadowColor, shadowOffset, borderColor, borderStyle, borderWidth, imageReference, imageTransColor, imageAlign, imageMode, font, format, textEffect, textAlign, textVerticalAlign, ref counter, new Microsoft.ReportingServices.RdlObjectModel.Style(), new Border());
		}

		private EmptyColorStyle ConvertDundasCRIEmptyColorStyleProperty(object color, object backgroundColor, object backgroundGradientType, object backgroundGradientEndColor, object backgroundHatchType, object borderColor, object borderStyle, object borderWidth, object imageReference, object imageTransColor, object imageAlign, object imageMode, object font, object format, ref int counter)
		{
			return (EmptyColorStyle)ConvertDundasCRIStyleProperty(color, backgroundColor, backgroundGradientType, backgroundGradientEndColor, backgroundHatchType, null, null, borderColor, borderStyle, borderWidth, imageReference, imageTransColor, imageAlign, imageMode, font, format, null, null, null, ref counter, new EmptyColorStyle(), new EmptyBorder());
		}

		private object ConvertDundasCRIStyleProperty(object color, object backgroundColor, object backgroundGradientType, object backgroundGradientEndColor, object backgroundHatchType, object shadowColor, object shadowOffset, object borderColor, object borderStyle, object borderWidth, object imageReference, object imageTransColor, object imageAlign, object imageMode, object font, object format, object textEffect, object textAlign, object textVerticalAlign, ref int counter, Microsoft.ReportingServices.RdlObjectModel.Style style, Border border)
		{
			int counter2 = 0;
			style.Color = ConvertDundasCRIColorProperty(style.Color, color, ref counter2);
			style.BackgroundColor = ConvertDundasCRIColorProperty(style.BackgroundColor, backgroundColor, ref counter2);
			style.BackgroundGradientEndColor = ConvertDundasCRIColorProperty(style.BackgroundGradientEndColor, backgroundGradientEndColor, ref counter2);
			style.ShadowColor = ConvertDundasCRIColorProperty(style.ShadowColor, shadowColor, ref counter2);
			style.ShadowOffset = ConvertDundasCRIPixelReportSizeProperty(shadowOffset, ref counter2);
			style.Format = ConvertDundasCRIStringProperty(format, ref counter2);
			style.TextEffect = new ReportExpression<TextEffects>(ConvertDundasCRIStringProperty(textEffect, ref counter2), CultureInfo.InvariantCulture);
			try
			{
				style.BackgroundGradientType = new ReportExpression<BackgroundGradients>(ConvertDundasCRIStringProperty(backgroundGradientType, ref counter2), CultureInfo.InvariantCulture);
			}
			catch
			{
			}
			try
			{
				style.BackgroundHatchType = new ReportExpression<BackgroundHatchTypes>(ConvertDundasCRIStringProperty(backgroundHatchType, ref counter2), CultureInfo.InvariantCulture);
			}
			catch
			{
			}
			style.TextAlign = new ReportExpression<TextAlignments>(ConvertDundasCRIStringProperty(textAlign, ref counter2), CultureInfo.InvariantCulture);
			style.VerticalAlign = new ReportExpression<VerticalAlignments>(ConvertDundasCRIStringProperty(textVerticalAlign, ref counter2), CultureInfo.InvariantCulture);
			int counter3 = 0;
			border.Color = ConvertDundasCRIColorProperty(border.Color, borderColor, ref counter3);
			border.Width = ConvertDundasCRIPixelReportSizeProperty(1.0, borderWidth, ref counter3);
			string text = ConvertDundasCRIStringProperty(BorderStyles.Solid.ToString(), borderStyle, ref counter3);
			switch (text)
			{
			case "NotSet":
				border.Style = BorderStyles.None;
				break;
			case "Dash":
				border.Style = BorderStyles.Dashed;
				break;
			case "Dot":
				border.Style = BorderStyles.Dotted;
				break;
			default:
				try
				{
					border.Style = new ReportExpression<BorderStyles>(text, CultureInfo.InvariantCulture);
				}
				catch
				{
					border.Style = BorderStyles.Solid;
				}
				break;
			}
			if (counter3 > 0)
			{
				if (borderWidth != null && !border.Width.IsExpression)
				{
					if (border.Width.Value < Microsoft.ReportingServices.RdlObjectModel.Constants.MinimumBorderWidth)
					{
						border.Width = Microsoft.ReportingServices.RdlObjectModel.Constants.DefaultBorderWidth;
						border.Style = BorderStyles.None;
					}
					else if (border.Width.Value > Microsoft.ReportingServices.RdlObjectModel.Constants.MaximumBorderWidth)
					{
						border.Width = Microsoft.ReportingServices.RdlObjectModel.Constants.MaximumBorderWidth;
					}
				}
				if (style is EmptyColorStyle)
				{
					((EmptyColorStyle)style).Border = (EmptyBorder)border;
				}
				else
				{
					style.Border = border;
				}
				counter2++;
			}
			style.BackgroundImage = ConvertDundasCRIChartBackgroundImageProperty(imageReference, imageTransColor, imageAlign, imageMode, ref counter2);
			string text2 = ConvertDundasCRIStringProperty(font, ref counter2);
			if (text2 != string.Empty)
			{
				Font font2 = FontFromString(text2);
				style.FontFamily = font2.FontFamily.Name;
				style.FontSize = new ReportSize(font2.Size, SizeTypes.Point);
				if (font2.Bold)
				{
					style.FontWeight = FontWeights.Bold;
				}
				if (font2.Italic)
				{
					style.FontStyle = FontStyles.Italic;
				}
				if (font2.Strikeout)
				{
					style.TextDecoration = TextDecorations.LineThrough;
				}
				else if (font2.Underline)
				{
					style.TextDecoration = TextDecorations.Underline;
				}
			}
			if (counter2 > 0)
			{
				counter++;
				return style;
			}
			return null;
		}
	}
}
