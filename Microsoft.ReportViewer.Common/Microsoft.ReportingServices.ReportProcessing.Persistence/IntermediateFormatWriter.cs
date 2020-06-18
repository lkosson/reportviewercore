using Microsoft.ReportingServices.ReportRendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace Microsoft.ReportingServices.ReportProcessing.Persistence
{
	internal sealed class IntermediateFormatWriter
	{
		private sealed class ReportServerBinaryWriter
		{
			private sealed class BinaryWriterWrapper : BinaryWriter
			{
				internal BinaryWriterWrapper(Stream stream)
					: base(stream, Encoding.Unicode)
				{
				}

				internal new void Write7BitEncodedInt(int int32Value)
				{
					base.Write7BitEncodedInt(int32Value);
				}
			}

			private BinaryWriterWrapper m_binaryWriter;

			internal ReportServerBinaryWriter(Stream stream)
			{
				m_binaryWriter = new BinaryWriterWrapper(stream);
			}

			internal void WriteGuid(Guid guid)
			{
				byte[] array = guid.ToByteArray();
				Assert(array != null);
				Assert(16 == array.Length);
				m_binaryWriter.Write((byte)239);
				m_binaryWriter.Write(array);
			}

			internal void WriteString(string stringValue)
			{
				if (stringValue == null)
				{
					WriteNull();
					return;
				}
				m_binaryWriter.Write((byte)240);
				m_binaryWriter.Write(stringValue);
			}

			internal void WriteChar(char charValue)
			{
				m_binaryWriter.Write((byte)243);
				m_binaryWriter.Write(charValue);
			}

			internal void WriteBoolean(bool booleanValue)
			{
				m_binaryWriter.Write((byte)244);
				m_binaryWriter.Write(booleanValue);
			}

			internal void WriteInt16(short int16Value)
			{
				m_binaryWriter.Write((byte)245);
				m_binaryWriter.Write(int16Value);
			}

			internal void WriteInt32(int int32Value)
			{
				m_binaryWriter.Write((byte)246);
				m_binaryWriter.Write(int32Value);
			}

			internal void WriteInt64(long int64Value)
			{
				m_binaryWriter.Write((byte)247);
				m_binaryWriter.Write(int64Value);
			}

			internal void WriteUInt16(ushort uint16Value)
			{
				m_binaryWriter.Write((byte)248);
				m_binaryWriter.Write(uint16Value);
			}

			internal void WriteUInt32(uint uint32Value)
			{
				m_binaryWriter.Write((byte)249);
				m_binaryWriter.Write(uint32Value);
			}

			internal void WriteUInt64(ulong uint64Value)
			{
				m_binaryWriter.Write((byte)250);
				m_binaryWriter.Write(uint64Value);
			}

			internal void WriteByte(byte byteValue)
			{
				m_binaryWriter.Write((byte)251);
				m_binaryWriter.Write(byteValue);
			}

			internal void WriteSByte(sbyte sbyteValue)
			{
				m_binaryWriter.Write((byte)252);
				m_binaryWriter.Write(sbyteValue);
			}

			internal void WriteSingle(float singleValue)
			{
				m_binaryWriter.Write((byte)253);
				m_binaryWriter.Write(singleValue);
			}

			internal void WriteDouble(double doubleValue)
			{
				m_binaryWriter.Write((byte)254);
				m_binaryWriter.Write(doubleValue);
			}

			internal void WriteDecimal(decimal decimalValue)
			{
				m_binaryWriter.Write(byte.MaxValue);
				m_binaryWriter.Write(decimalValue);
			}

			internal void WriteDateTime(DateTime dateTimeValue)
			{
				m_binaryWriter.Write((byte)241);
				m_binaryWriter.Write(dateTimeValue.Ticks);
			}

			internal void WriteTimeSpan(TimeSpan timeSpanValue)
			{
				m_binaryWriter.Write((byte)242);
				m_binaryWriter.Write(timeSpanValue.Ticks);
			}

			internal void WriteBytes(byte[] bytesValue)
			{
				if (bytesValue == null)
				{
					WriteNull();
					return;
				}
				m_binaryWriter.Write((byte)5);
				m_binaryWriter.Write((byte)251);
				m_binaryWriter.Write7BitEncodedInt(bytesValue.Length);
				m_binaryWriter.Write(bytesValue);
			}

			internal void WriteInt32s(int[] int32Values)
			{
				if (int32Values == null)
				{
					WriteNull();
					return;
				}
				m_binaryWriter.Write((byte)5);
				m_binaryWriter.Write((byte)246);
				m_binaryWriter.Write7BitEncodedInt(int32Values.Length);
				for (int i = 0; i < int32Values.Length; i++)
				{
					m_binaryWriter.Write(int32Values[i]);
				}
			}

			internal void WriteFloatArray(float[] values)
			{
				if (values == null)
				{
					WriteNull();
					return;
				}
				m_binaryWriter.Write((byte)5);
				m_binaryWriter.Write((byte)253);
				m_binaryWriter.Write7BitEncodedInt(values.Length);
				for (int i = 0; i < values.Length; i++)
				{
					m_binaryWriter.Write(values[i]);
				}
			}

			internal void WriteChars(char[] charsValue)
			{
				if (charsValue == null)
				{
					WriteNull();
					return;
				}
				m_binaryWriter.Write((byte)5);
				m_binaryWriter.Write((byte)243);
				m_binaryWriter.Write7BitEncodedInt(charsValue.Length);
				m_binaryWriter.Write(charsValue);
			}

			internal void StartObject(ObjectType objectType)
			{
				m_binaryWriter.Write((byte)1);
				m_binaryWriter.Write7BitEncodedInt((int)objectType);
			}

			internal void EndObject()
			{
				m_binaryWriter.Write((byte)2);
			}

			internal void WriteNull()
			{
				m_binaryWriter.Write((byte)0);
			}

			internal void WriteReference(ObjectType objectType, int referenceValue)
			{
				m_binaryWriter.Write((byte)3);
				m_binaryWriter.Write7BitEncodedInt((int)objectType);
				m_binaryWriter.Write(referenceValue);
			}

			internal void WriteNoTypeReference(int referenceValue)
			{
				m_binaryWriter.Write((byte)3);
				m_binaryWriter.Write(referenceValue);
			}

			internal void WriteEnum(int enumValue)
			{
				m_binaryWriter.Write((byte)4);
				m_binaryWriter.Write7BitEncodedInt(enumValue);
			}

			internal void WriteDataFieldStatus(DataFieldStatus status)
			{
				m_binaryWriter.Write((byte)8);
				m_binaryWriter.Write7BitEncodedInt((int)status);
			}

			internal void StartArray(int count)
			{
				m_binaryWriter.Write((byte)6);
				m_binaryWriter.Write7BitEncodedInt(count);
			}

			internal void EndArray()
			{
			}

			internal void DeclareType(ObjectType objectType, Declaration declaration)
			{
				Assert(declaration != null);
				Assert(declaration.Members != null);
				m_binaryWriter.Write((byte)7);
				m_binaryWriter.Write7BitEncodedInt((int)objectType);
				m_binaryWriter.Write7BitEncodedInt((int)declaration.BaseType);
				m_binaryWriter.Write7BitEncodedInt(declaration.Members.Count);
				for (int i = 0; i < declaration.Members.Count; i++)
				{
					Assert(declaration.Members[i] != null);
					m_binaryWriter.Write7BitEncodedInt((int)declaration.Members[i].MemberName);
					m_binaryWriter.Write((byte)declaration.Members[i].Token);
					m_binaryWriter.Write7BitEncodedInt((int)declaration.Members[i].ObjectType);
				}
			}
		}

		private ReportServerBinaryWriter m_writer;

		private bool m_writeDeclarations;

		private bool[] m_declarationsWritten;

		private bool m_writeUniqueName = true;

		internal bool[] DeclarationsToWrite
		{
			get
			{
				Assert(!m_writeDeclarations);
				return m_declarationsWritten;
			}
		}

		internal IntermediateFormatWriter(Stream stream, bool[] firstPageDeclarationsToWrite, bool[] otherPageDeclarationsToWrite)
		{
			Initialize(stream, writeDeclarations: true, firstPageDeclarationsToWrite, otherPageDeclarationsToWrite);
		}

		internal IntermediateFormatWriter(Stream stream, bool writeDeclarations)
		{
			Initialize(stream, writeDeclarations, null, null);
		}

		private void Initialize(Stream stream, bool writeDeclarations, bool[] firstPageDeclarationsToWrite, bool[] otherPageDeclarationsToWrite)
		{
			Assert(stream != null);
			m_writer = new ReportServerBinaryWriter(stream);
			m_writer.WriteBytes(VersionStamp.GetBytes());
			m_writeDeclarations = writeDeclarations;
			m_declarationsWritten = new bool[DeclarationList.Current.Count];
			WriteDeclarations(firstPageDeclarationsToWrite);
			WriteDeclarations(otherPageDeclarationsToWrite);
		}

		private void WriteDeclarations(bool[] declarationsToWrite)
		{
			if (declarationsToWrite == null)
			{
				return;
			}
			Assert(m_declarationsWritten.Length == declarationsToWrite.Length);
			for (int i = 0; i < declarationsToWrite.Length; i++)
			{
				if (declarationsToWrite[i])
				{
					DeclareType((ObjectType)i);
				}
			}
		}

		internal void WriteIntermediateFormatVersion(IntermediateFormatVersion version)
		{
			if (version == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.IntermediateFormatVersion;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			m_writer.WriteInt32(version.Major);
			m_writer.WriteInt32(version.Minor);
			m_writer.WriteInt32(version.Build);
			m_writer.EndObject();
		}

		internal void WriteReport(Report report)
		{
			if (report == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.Report;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			report.IntermediateFormatVersion.SetCurrent();
			WriteIntermediateFormatVersion(report.IntermediateFormatVersion);
			m_writer.WriteGuid(report.ReportVersion);
			WriteReportItemBase(report);
			m_writer.WriteString(report.Author);
			m_writer.WriteInt32(report.AutoRefresh);
			WriteEmbeddedImageHashtable(report.EmbeddedImages);
			WritePageSection(report.PageHeader);
			WritePageSection(report.PageFooter);
			WriteReportItemCollection(report.ReportItems);
			WriteDataSourceList(report.DataSources);
			m_writer.WriteString(report.PageHeight);
			m_writer.WriteDouble(report.PageHeightValue);
			m_writer.WriteString(report.PageWidth);
			m_writer.WriteDouble(report.PageWidthValue);
			m_writer.WriteString(report.LeftMargin);
			m_writer.WriteDouble(report.LeftMarginValue);
			m_writer.WriteString(report.RightMargin);
			m_writer.WriteDouble(report.RightMarginValue);
			m_writer.WriteString(report.TopMargin);
			m_writer.WriteDouble(report.TopMarginValue);
			m_writer.WriteString(report.BottomMargin);
			m_writer.WriteDouble(report.BottomMarginValue);
			m_writer.WriteInt32(report.Columns);
			m_writer.WriteString(report.ColumnSpacing);
			m_writer.WriteDouble(report.ColumnSpacingValue);
			WriteDataAggregateInfoList(report.PageAggregates);
			m_writer.WriteBytes(report.CompiledCode);
			m_writer.WriteBoolean(report.MergeOnePass);
			m_writer.WriteBoolean(report.PageMergeOnePass);
			m_writer.WriteBoolean(report.SubReportMergeTransactions);
			m_writer.WriteBoolean(report.NeedPostGroupProcessing);
			m_writer.WriteBoolean(report.HasPostSortAggregates);
			m_writer.WriteBoolean(report.HasReportItemReferences);
			WriteShowHideTypes(report.ShowHideType);
			WriteImageStreamNames(report.ImageStreamNames);
			m_writer.WriteInt32(report.LastID);
			m_writer.WriteInt32(report.BodyID);
			WriteSubReportList(report.SubReports);
			m_writer.WriteBoolean(report.HasImageStreams);
			m_writer.WriteBoolean(report.HasLabels);
			m_writer.WriteBoolean(report.HasBookmarks);
			m_writer.WriteBoolean(report.ParametersNotUsedInQuery);
			WriteParameterDefList(report.Parameters);
			m_writer.WriteString(report.OneDataSetName);
			WriteStringList(report.CodeModules);
			WriteCodeClassList(report.CodeClasses);
			m_writer.WriteBoolean(report.HasSpecialRecursiveAggregates);
			WriteExpressionInfo(report.Language);
			m_writer.WriteString(report.DataTransform);
			m_writer.WriteString(report.DataSchema);
			m_writer.WriteBoolean(report.DataElementStyleAttribute);
			m_writer.WriteString(report.Code);
			m_writer.WriteBoolean(report.HasUserSortFilter);
			m_writer.WriteBoolean(report.CompiledCodeGeneratedWithRefusedPermissions);
			m_writer.WriteString(report.InteractiveHeight);
			m_writer.WriteDouble(report.InteractiveHeightValue);
			m_writer.WriteString(report.InteractiveWidth);
			m_writer.WriteDouble(report.InteractiveWidthValue);
			WriteInScopeSortFilterHashtable(report.NonDetailSortFiltersInScope);
			WriteInScopeSortFilterHashtable(report.DetailSortFiltersInScope);
			m_writer.EndObject();
		}

		internal void WriteReportSnapshot(ReportSnapshot reportSnapshot)
		{
			if (reportSnapshot == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ReportSnapshot;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			m_writer.WriteDateTime(reportSnapshot.ExecutionTime);
			WriteReport(reportSnapshot.Report);
			WriteParameterInfoCollection(reportSnapshot.Parameters);
			WriteReportInstance(reportSnapshot.ReportInstance);
			m_writer.WriteBoolean(reportSnapshot.HasDocumentMap);
			m_writer.WriteBoolean(reportSnapshot.HasShowHide);
			m_writer.WriteBoolean(reportSnapshot.HasBookmarks);
			m_writer.WriteBoolean(reportSnapshot.HasImageStreams);
			m_writer.WriteString(reportSnapshot.RequestUserName);
			m_writer.WriteString(reportSnapshot.ReportServerUrl);
			m_writer.WriteString(reportSnapshot.ReportFolder);
			m_writer.WriteString(reportSnapshot.Language);
			WriteProcessingMessageList(reportSnapshot.Warnings);
			WriteInt64List(reportSnapshot.PageSectionOffsets);
			m_writer.EndObject();
		}

		internal void WriteDocumentMapNode(DocumentMapNode documentMapNode)
		{
			if (documentMapNode == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.DocumentMapNode;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteInstanceInfoBase(documentMapNode);
			m_writer.WriteString(documentMapNode.Id);
			m_writer.WriteString(documentMapNode.Label);
			m_writer.WriteInt32(documentMapNode.Page);
			WriteDocumentMapNodes(documentMapNode.Children);
			m_writer.EndObject();
		}

		internal void WriteBookmarksHashtable(BookmarksHashtable bookmarks)
		{
			if (bookmarks == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.BookmarksHashtable;
			m_writer.StartObject(objectType);
			m_writer.StartArray(bookmarks.Count);
			int num = 0;
			IDictionaryEnumerator enumerator = bookmarks.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string stringValue = (string)enumerator.Key;
				m_writer.WriteString(stringValue);
				WriteBookmarkInformation((BookmarkInformation)enumerator.Value);
				num++;
			}
			Assert(num == bookmarks.Count);
			m_writer.EndArray();
			m_writer.EndObject();
		}

		internal void WriteDrillthroughInfo(ReportDrillthroughInfo reportDrillthroughInfo)
		{
			if (reportDrillthroughInfo == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ReportDrillthroughInfo;
			m_writer.StartObject(objectType);
			WriteTokensHashtable(reportDrillthroughInfo.RewrittenCommands);
			WriteDrillthroughHashtable(reportDrillthroughInfo.DrillthroughInformation);
			m_writer.EndObject();
		}

		internal void WriteTokensHashtable(TokensHashtable tokens)
		{
			if (tokens == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.TokensHashtable;
			m_writer.StartObject(objectType);
			m_writer.StartArray(tokens.Count);
			int num = 0;
			IDictionaryEnumerator enumerator = tokens.GetEnumerator();
			while (enumerator.MoveNext())
			{
				int int32Value = (int)enumerator.Key;
				m_writer.WriteInt32(int32Value);
				WriteVariant(enumerator.Value);
				num++;
			}
			Assert(num == tokens.Count);
			m_writer.EndArray();
			m_writer.EndObject();
		}

		internal void WriteDrillthroughHashtable(DrillthroughHashtable drillthrough)
		{
			if (drillthrough == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.DrillthroughHashtable;
			m_writer.StartObject(objectType);
			m_writer.StartArray(drillthrough.Count);
			int num = 0;
			IDictionaryEnumerator enumerator = drillthrough.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string stringValue = (string)enumerator.Key;
				m_writer.WriteString(stringValue);
				WriteDrillthroughInformation((DrillthroughInformation)enumerator.Value);
				num++;
			}
			Assert(num == drillthrough.Count);
			m_writer.EndArray();
			m_writer.EndObject();
		}

		internal void WriteSenderInformationHashtable(SenderInformationHashtable senders)
		{
			if (senders == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.SenderInformationHashtable;
			m_writer.StartObject(objectType);
			m_writer.StartArray(senders.Count);
			int num = 0;
			IDictionaryEnumerator enumerator = senders.GetEnumerator();
			while (enumerator.MoveNext())
			{
				int int32Value = (int)enumerator.Key;
				m_writer.WriteInt32(int32Value);
				WriteSenderInformation((SenderInformation)enumerator.Value);
				num++;
			}
			Assert(num == senders.Count);
			m_writer.EndArray();
			m_writer.EndObject();
		}

		internal void WriteReceiverInformationHashtable(ReceiverInformationHashtable receivers)
		{
			if (receivers == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ReceiverInformationHashtable;
			m_writer.StartObject(objectType);
			m_writer.StartArray(receivers.Count);
			int num = 0;
			IDictionaryEnumerator enumerator = receivers.GetEnumerator();
			while (enumerator.MoveNext())
			{
				int int32Value = (int)enumerator.Key;
				m_writer.WriteInt32(int32Value);
				WriteReceiverInformation((ReceiverInformation)enumerator.Value);
				num++;
			}
			Assert(num == receivers.Count);
			m_writer.EndArray();
			m_writer.EndObject();
		}

		internal void WriteQuickFindHashtable(QuickFindHashtable quickFind)
		{
			if (quickFind == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.QuickFindHashtable;
			m_writer.StartObject(objectType);
			m_writer.StartArray(quickFind.Count);
			int num = 0;
			IDictionaryEnumerator enumerator = quickFind.GetEnumerator();
			while (enumerator.MoveNext())
			{
				int int32Value = (int)enumerator.Key;
				m_writer.WriteInt32(int32Value);
				WriteReportItemInstanceReference((ReportItemInstance)enumerator.Value);
				num++;
			}
			Assert(num == quickFind.Count);
			m_writer.EndArray();
			m_writer.EndObject();
		}

		internal void WriteSortFilterEventInfoHashtable(SortFilterEventInfoHashtable sortFilterEventInfo)
		{
			if (sortFilterEventInfo == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.SortFilterEventInfoHashtable;
			m_writer.StartObject(objectType);
			m_writer.StartArray(sortFilterEventInfo.Count);
			int num = 0;
			IDictionaryEnumerator enumerator = sortFilterEventInfo.GetEnumerator();
			while (enumerator.MoveNext())
			{
				int int32Value = (int)enumerator.Key;
				m_writer.WriteInt32(int32Value);
				WriteSortFilterEventInfo((SortFilterEventInfo)enumerator.Value);
				num++;
			}
			Assert(num == sortFilterEventInfo.Count);
			m_writer.EndArray();
			m_writer.EndObject();
		}

		internal Int64List WritePageSections(Stream stream, List<PageSectionInstance> pageSections)
		{
			Int64List int64List = null;
			if (pageSections == null)
			{
				m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.PageSectionInstanceList;
				m_writer.StartObject(objectType);
				int count = pageSections.Count;
				Assert(count % 2 == 0);
				m_writer.StartArray(count);
				int64List = new Int64List(count / 2);
				for (int i = 0; i < count; i++)
				{
					if (i % 2 == 0)
					{
						int64List.Add(stream.Position);
					}
					WritePageSectionInstance(pageSections[i]);
				}
				m_writer.EndArray();
				m_writer.EndObject();
			}
			return int64List;
		}

		internal void WriteSortFilterEventInfo(SortFilterEventInfo sortFilterEventInfo)
		{
			if (sortFilterEventInfo == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.SortFilterEventInfo;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteReportItemReference(sortFilterEventInfo.EventSource);
			WriteVariantLists(sortFilterEventInfo.EventSourceScopeInfo, convertDBNull: true);
			m_writer.EndObject();
		}

		internal void WriteInstanceInfo(InstanceInfo instanceInfo)
		{
			if (instanceInfo == null)
			{
				m_writer.WriteNull();
				return;
			}
			if (instanceInfo is ReportItemInstanceInfo)
			{
				WriteReportItemInstanceInfo((ReportItemInstanceInfo)instanceInfo);
				return;
			}
			if (instanceInfo is SimpleTextBoxInstanceInfo)
			{
				WriteSimpleTextBoxInstanceInfo((SimpleTextBoxInstanceInfo)instanceInfo);
				return;
			}
			if (instanceInfo is ReportItemColInstanceInfo)
			{
				WriteReportItemColInstanceInfo((ReportItemColInstanceInfo)instanceInfo);
				return;
			}
			if (instanceInfo is ListContentInstanceInfo)
			{
				WriteListContentInstanceInfo((ListContentInstanceInfo)instanceInfo);
				return;
			}
			if (instanceInfo is TableRowInstanceInfo)
			{
				WriteTableRowInstanceInfo((TableRowInstanceInfo)instanceInfo);
				return;
			}
			if (instanceInfo is TableGroupInstanceInfo)
			{
				WriteTableGroupInstanceInfo((TableGroupInstanceInfo)instanceInfo);
				return;
			}
			if (instanceInfo is MatrixCellInstanceInfo)
			{
				WriteMatrixCellInstanceInfo((MatrixCellInstanceInfo)instanceInfo);
				return;
			}
			if (instanceInfo is MatrixSubtotalHeadingInstanceInfo)
			{
				WriteMatrixSubtotalHeadingInstanceInfo((MatrixSubtotalHeadingInstanceInfo)instanceInfo);
				return;
			}
			if (instanceInfo is TableDetailInstanceInfo)
			{
				WriteTableDetailInstanceInfo((TableDetailInstanceInfo)instanceInfo);
				return;
			}
			if (instanceInfo is ChartInstanceInfo)
			{
				WriteChartInstanceInfo((ChartInstanceInfo)instanceInfo);
				return;
			}
			if (instanceInfo is ChartHeadingInstanceInfo)
			{
				WriteChartHeadingInstanceInfo((ChartHeadingInstanceInfo)instanceInfo);
				return;
			}
			if (instanceInfo is ChartDataPointInstanceInfo)
			{
				WriteChartDataPointInstanceInfo((ChartDataPointInstanceInfo)instanceInfo);
				return;
			}
			Assert(instanceInfo is MatrixHeadingInstanceInfo);
			WriteMatrixHeadingInstanceInfo((MatrixHeadingInstanceInfo)instanceInfo);
		}

		internal void WriteRecordSetInfo(RecordSetInfo recordSetInfo)
		{
			if (recordSetInfo == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.RecordSetInfo;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			m_writer.WriteBoolean(recordSetInfo.ReaderExtensionsSupported);
			WriteRecordSetPropertyNamesList(recordSetInfo.FieldPropertyNames);
			m_writer.WriteEnum((int)recordSetInfo.CompareOptions);
			m_writer.EndObject();
		}

		internal void WriteRecordSetPropertyNamesList(RecordSetPropertyNamesList list)
		{
			if (list == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.RecordSetPropertyNamesList;
			m_writer.StartObject(objectType);
			m_writer.StartArray(list.Count);
			for (int i = 0; i < list.Count; i++)
			{
				WriteRecordSetPropertyNames(list[i]);
			}
			m_writer.EndArray();
			m_writer.EndObject();
		}

		internal void WriteRecordSetPropertyNames(RecordSetPropertyNames field)
		{
			if (field == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.RecordSetPropertyNames;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteStringList(field.PropertyNames);
			m_writer.EndObject();
		}

		internal bool WriteRecordRow(RecordRow recordRow, RecordSetPropertyNamesList aliasPropertyNames)
		{
			bool result = true;
			if (recordRow == null)
			{
				m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.RecordRow;
				DeclareType(objectType);
				m_writer.StartObject(objectType);
				result = WriteRecordFields(recordRow.RecordFields, aliasPropertyNames);
				m_writer.WriteBoolean(recordRow.IsAggregateRow);
				m_writer.WriteInt32(recordRow.AggregationFieldCount);
				m_writer.EndObject();
			}
			return result;
		}

		private static void Assert(bool condition)
		{
			Global.Tracer.Assert(condition);
		}

		private void DeclareType(ObjectType objectType)
		{
			if (!m_declarationsWritten[(int)objectType])
			{
				if (m_writeDeclarations)
				{
					m_writer.DeclareType(objectType, DeclarationList.Current[objectType]);
				}
				m_declarationsWritten[(int)objectType] = true;
			}
		}

		private void WriteValidValueList(ValidValueList parameters)
		{
			if (parameters == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ValidValueList;
			m_writer.StartObject(objectType);
			m_writer.StartArray(parameters.Count);
			for (int i = 0; i < parameters.Count; i++)
			{
				WriteValidValue(parameters[i]);
			}
			m_writer.EndArray();
			m_writer.EndObject();
		}

		private void WriteParameterDefList(ParameterDefList parameters)
		{
			if (parameters == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ParameterDefList;
			m_writer.StartObject(objectType);
			m_writer.StartArray(parameters.Count);
			for (int i = 0; i < parameters.Count; i++)
			{
				WriteParameterDef(parameters[i]);
			}
			m_writer.EndArray();
			m_writer.EndObject();
		}

		private void WriteParameterDefRefList(ParameterDefList parameters)
		{
			if (parameters == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ParameterDefList;
			m_writer.StartObject(objectType);
			m_writer.StartArray(parameters.Count);
			for (int i = 0; i < parameters.Count; i++)
			{
				m_writer.WriteString(parameters[i].Name);
			}
			m_writer.EndArray();
			m_writer.EndObject();
		}

		private void WriteParameterInfoCollection(ParameterInfoCollection parameters)
		{
			if (parameters == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ParameterInfoCollection;
			m_writer.StartObject(objectType);
			m_writer.StartArray(parameters.Count);
			for (int i = 0; i < parameters.Count; i++)
			{
				WriteParameterInfo(parameters[i]);
			}
			m_writer.EndArray();
			m_writer.EndObject();
		}

		private void WriteParameterInfoRefCollection(ParameterInfoCollection parameters)
		{
			if (parameters == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ParameterInfoCollection;
			m_writer.StartObject(objectType);
			m_writer.StartArray(parameters.Count);
			for (int i = 0; i < parameters.Count; i++)
			{
				m_writer.WriteString(parameters[i].Name);
			}
			m_writer.EndArray();
			m_writer.EndObject();
		}

		private void WriteFilterList(FilterList filters)
		{
			if (filters == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.FilterList;
			m_writer.StartObject(objectType);
			m_writer.StartArray(filters.Count);
			for (int i = 0; i < filters.Count; i++)
			{
				WriteFilter(filters[i]);
			}
			m_writer.EndArray();
			m_writer.EndObject();
		}

		private void WriteDataSourceList(DataSourceList dataSources)
		{
			if (dataSources == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.DataSourceList;
			m_writer.StartObject(objectType);
			m_writer.StartArray(dataSources.Count);
			for (int i = 0; i < dataSources.Count; i++)
			{
				WriteDataSource(dataSources[i]);
			}
			m_writer.EndArray();
			m_writer.EndObject();
		}

		private void WriteDataAggregateInfoList(DataAggregateInfoList aggregates)
		{
			if (aggregates == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.DataAggregateInfoList;
			m_writer.StartObject(objectType);
			m_writer.StartArray(aggregates.Count);
			for (int i = 0; i < aggregates.Count; i++)
			{
				WriteDataAggregateInfo(aggregates[i]);
			}
			m_writer.EndArray();
			m_writer.EndObject();
		}

		private void WriteReportItemIDList(ReportItemList reportItems)
		{
			if (reportItems == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ReportItemList;
			m_writer.StartObject(objectType);
			m_writer.StartArray(reportItems.Count);
			for (int i = 0; i < reportItems.Count; i++)
			{
				WriteReportItemID(reportItems[i]);
			}
			m_writer.EndArray();
			m_writer.EndObject();
		}

		private void WriteReportItemID(ReportItem reportItem)
		{
			if (reportItem == null)
			{
				m_writer.WriteNull();
				return;
			}
			Assert(reportItem is TextBox);
			m_writer.WriteReference(ObjectType.TextBox, reportItem.ID);
		}

		private void WriteReportItemList(ReportItemList reportItems)
		{
			if (reportItems == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ReportItemList;
			m_writer.StartObject(objectType);
			m_writer.StartArray(reportItems.Count);
			for (int i = 0; i < reportItems.Count; i++)
			{
				WriteReportItem(reportItems[i]);
			}
			m_writer.EndArray();
			m_writer.EndObject();
		}

		private void WriteReportItemIndexerList(ReportItemIndexerList reportItemIndexers)
		{
			if (reportItemIndexers == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ReportItemIndexerList;
			m_writer.StartObject(objectType);
			m_writer.StartArray(reportItemIndexers.Count);
			for (int i = 0; i < reportItemIndexers.Count; i++)
			{
				WriteReportItemIndexer(reportItemIndexers[i]);
			}
			m_writer.EndArray();
			m_writer.EndObject();
		}

		private void WriteRunningValueInfoList(RunningValueInfoList runningValues)
		{
			if (runningValues == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.RunningValueInfoList;
			m_writer.StartObject(objectType);
			m_writer.StartArray(runningValues.Count);
			for (int i = 0; i < runningValues.Count; i++)
			{
				WriteRunningValueInfo(runningValues[i]);
			}
			m_writer.EndArray();
			m_writer.EndObject();
		}

		private void WriteStyleAttributeHashtable(StyleAttributeHashtable styleAttributes)
		{
			if (styleAttributes == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.StyleAttributeHashtable;
			m_writer.StartObject(objectType);
			m_writer.StartArray(styleAttributes.Count);
			int num = 0;
			IDictionaryEnumerator enumerator = styleAttributes.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string text = (string)enumerator.Key;
				Assert(text != null);
				m_writer.WriteString(text);
				WriteAttributeInfo((AttributeInfo)enumerator.Value);
				num++;
			}
			Assert(num == styleAttributes.Count);
			m_writer.EndArray();
			m_writer.EndObject();
		}

		private void WriteImageInfo(ImageInfo imageInfo)
		{
			if (imageInfo == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ImageInfo;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			m_writer.WriteString(imageInfo.StreamName);
			m_writer.WriteString(imageInfo.MimeType);
			m_writer.EndObject();
		}

		private void WriteDrillthroughParameters(DrillthroughParameters parameters)
		{
			if (parameters == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.DrillthroughParameters;
			m_writer.StartObject(objectType);
			m_writer.StartArray(parameters.Count);
			for (int i = 0; i < parameters.Count; i++)
			{
				string key = parameters.GetKey(i);
				object value = parameters.GetValue(i);
				object[] array = null;
				Assert(key != null);
				m_writer.WriteString(key);
				if (value != null)
				{
					array = (value as object[]);
				}
				if (array != null)
				{
					WriteVariants(array, isMultiValue: false);
				}
				else
				{
					WriteVariant(value);
				}
			}
			m_writer.EndArray();
			m_writer.EndObject();
		}

		private void WriteImageStreamNames(ImageStreamNames streamNames)
		{
			if (streamNames == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ImageStreamNames;
			m_writer.StartObject(objectType);
			m_writer.StartArray(streamNames.Count);
			int num = 0;
			IDictionaryEnumerator enumerator = streamNames.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string text = (string)enumerator.Key;
				Assert(text != null);
				m_writer.WriteString(text);
				WriteImageInfo((ImageInfo)enumerator.Value);
				num++;
			}
			Assert(num == streamNames.Count);
			m_writer.EndArray();
			m_writer.EndObject();
		}

		private void WriteEmbeddedImageHashtable(EmbeddedImageHashtable embeddedImages)
		{
			if (embeddedImages == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.EmbeddedImageHashtable;
			m_writer.StartObject(objectType);
			m_writer.StartArray(embeddedImages.Count);
			int num = 0;
			IDictionaryEnumerator enumerator = embeddedImages.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string text = (string)enumerator.Key;
				Assert(text != null);
				m_writer.WriteString(text);
				WriteImageInfo((ImageInfo)enumerator.Value);
				num++;
			}
			Assert(num == embeddedImages.Count);
			m_writer.EndArray();
			m_writer.EndObject();
		}

		private void WriteExpressionInfoList(ExpressionInfoList expressions)
		{
			if (expressions == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ExpressionInfoList;
			m_writer.StartObject(objectType);
			m_writer.StartArray(expressions.Count);
			for (int i = 0; i < expressions.Count; i++)
			{
				WriteExpressionInfo(expressions[i]);
			}
			m_writer.EndArray();
			m_writer.EndObject();
		}

		private void WriteDataSetList(DataSetList dataSets)
		{
			if (dataSets == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.DataSetList;
			m_writer.StartObject(objectType);
			m_writer.StartArray(dataSets.Count);
			for (int i = 0; i < dataSets.Count; i++)
			{
				WriteDataSet(dataSets[i]);
			}
			m_writer.EndArray();
			m_writer.EndObject();
		}

		private void WriteExpressionInfos(ExpressionInfo[] expressions)
		{
			if (expressions == null)
			{
				m_writer.WriteNull();
				return;
			}
			m_writer.StartArray(expressions.Length);
			for (int i = 0; i < expressions.Length; i++)
			{
				WriteExpressionInfo(expressions[i]);
			}
			m_writer.EndArray();
		}

		private void WriteStringList(StringList strings)
		{
			if (strings == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.StringList;
			m_writer.StartObject(objectType);
			m_writer.StartArray(strings.Count);
			for (int i = 0; i < strings.Count; i++)
			{
				m_writer.WriteString(strings[i]);
			}
			m_writer.EndArray();
			m_writer.EndObject();
		}

		private void WriteDataFieldList(DataFieldList fields)
		{
			if (fields == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.DataFieldList;
			m_writer.StartObject(objectType);
			m_writer.StartArray(fields.Count);
			for (int i = 0; i < fields.Count; i++)
			{
				WriteDataField(fields[i]);
			}
			m_writer.EndArray();
			m_writer.EndObject();
		}

		private void WriteDataRegionList(DataRegionList dataRegions)
		{
			if (dataRegions == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.DataRegionList;
			m_writer.StartObject(objectType);
			m_writer.StartArray(dataRegions.Count);
			for (int i = 0; i < dataRegions.Count; i++)
			{
				WriteDataRegionReference(dataRegions[i]);
			}
			m_writer.EndArray();
			m_writer.EndObject();
		}

		private void WriteParameterValueList(ParameterValueList parameters)
		{
			if (parameters == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ParameterValueList;
			m_writer.StartObject(objectType);
			m_writer.StartArray(parameters.Count);
			for (int i = 0; i < parameters.Count; i++)
			{
				WriteParameterValue(parameters[i]);
			}
			m_writer.EndArray();
			m_writer.EndObject();
		}

		private void WriteCodeClassList(CodeClassList classes)
		{
			if (classes == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.CodeClassList;
			m_writer.StartObject(objectType);
			m_writer.StartArray(classes.Count);
			for (int i = 0; i < classes.Count; i++)
			{
				WriteCodeClass(classes[i]);
			}
			m_writer.EndArray();
			m_writer.EndObject();
		}

		private void WriteIntList(IntList ints)
		{
			if (ints == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.IntList;
			m_writer.StartObject(objectType);
			m_writer.StartArray(ints.Count);
			for (int i = 0; i < ints.Count; i++)
			{
				m_writer.WriteInt32(ints[i]);
			}
			m_writer.EndArray();
			m_writer.EndObject();
		}

		private void WriteInt64List(Int64List longs)
		{
			if (longs == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.Int64List;
			m_writer.StartObject(objectType);
			m_writer.StartArray(longs.Count);
			for (int i = 0; i < longs.Count; i++)
			{
				m_writer.WriteInt64(longs[i]);
			}
			m_writer.EndArray();
			m_writer.EndObject();
		}

		private void WriteBoolList(BoolList bools)
		{
			if (bools == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.BoolList;
			m_writer.StartObject(objectType);
			m_writer.StartArray(bools.Count);
			for (int i = 0; i < bools.Count; i++)
			{
				m_writer.WriteBoolean(bools[i]);
			}
			m_writer.EndArray();
			m_writer.EndObject();
		}

		private void WriteMatrixRowList(MatrixRowList rows)
		{
			if (rows == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.MatrixRowList;
			m_writer.StartObject(objectType);
			m_writer.StartArray(rows.Count);
			for (int i = 0; i < rows.Count; i++)
			{
				WriteMatrixRow(rows[i]);
			}
			m_writer.EndArray();
			m_writer.EndObject();
		}

		private void WriteMatrixColumnList(MatrixColumnList columns)
		{
			if (columns == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.MatrixColumnList;
			m_writer.StartObject(objectType);
			m_writer.StartArray(columns.Count);
			for (int i = 0; i < columns.Count; i++)
			{
				WriteMatrixColumn(columns[i]);
			}
			m_writer.EndArray();
			m_writer.EndObject();
		}

		private void WriteTableColumnList(TableColumnList columns)
		{
			if (columns == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.TableColumnList;
			m_writer.StartObject(objectType);
			m_writer.StartArray(columns.Count);
			for (int i = 0; i < columns.Count; i++)
			{
				WriteTableColumn(columns[i]);
			}
			m_writer.EndArray();
			m_writer.EndObject();
		}

		private void WriteTableRowList(TableRowList rows)
		{
			if (rows == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.TableRowList;
			m_writer.StartObject(objectType);
			m_writer.StartArray(rows.Count);
			for (int i = 0; i < rows.Count; i++)
			{
				WriteTableRow(rows[i]);
			}
			m_writer.EndArray();
			m_writer.EndObject();
		}

		private void WriteChartColumnList(ChartColumnList columns)
		{
			if (columns == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ChartColumnList;
			m_writer.StartObject(objectType);
			m_writer.StartArray(columns.Count);
			for (int i = 0; i < columns.Count; i++)
			{
				WriteChartColumn(columns[i]);
			}
			m_writer.EndArray();
			m_writer.EndObject();
		}

		private void WriteCustomReportItemHeadingList(CustomReportItemHeadingList headings)
		{
			if (headings == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.CustomReportItemHeadingList;
			m_writer.StartObject(objectType);
			m_writer.StartArray(headings.Count);
			for (int i = 0; i < headings.Count; i++)
			{
				WriteCustomReportItemHeading(headings[i]);
			}
			m_writer.EndArray();
			m_writer.EndObject();
		}

		private void WriteDataCellsList(DataCellsList rows)
		{
			if (rows == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.DataCellsList;
			m_writer.StartObject(objectType);
			m_writer.StartArray(rows.Count);
			for (int i = 0; i < rows.Count; i++)
			{
				WriteDataCellList(rows[i]);
			}
			m_writer.EndArray();
			m_writer.EndObject();
		}

		private void WriteDataCellList(DataCellList cells)
		{
			if (cells == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.DataCellList;
			m_writer.StartObject(objectType);
			m_writer.StartArray(cells.Count);
			for (int i = 0; i < cells.Count; i++)
			{
				WriteDataValueCRIList(cells[i]);
			}
			m_writer.EndArray();
			m_writer.EndObject();
		}

		private void WriteDataValueCRIList(DataValueCRIList values)
		{
			if (values == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.DataValueCRIList;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteDataValueList(values);
			m_writer.WriteInt32(values.RDLRowIndex);
			m_writer.WriteInt32(values.RDLColumnIndex);
			m_writer.EndObject();
		}

		private void WriteDataValueList(DataValueList values)
		{
			if (values == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.DataValueList;
			m_writer.StartObject(objectType);
			m_writer.StartArray(values.Count);
			for (int i = 0; i < values.Count; i++)
			{
				WriteDataValue(values[i]);
			}
			m_writer.EndArray();
			m_writer.EndObject();
		}

		private void WriteDataValueInstanceList(DataValueInstanceList instances)
		{
			if (instances == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.DataValueInstanceList;
			m_writer.StartObject(objectType);
			m_writer.StartArray(instances.Count);
			for (int i = 0; i < instances.Count; i++)
			{
				WriteDataValueInstance(instances[i]);
			}
			m_writer.EndArray();
			m_writer.EndObject();
		}

		private void WriteImageMapAreaInstanceList(ImageMapAreaInstanceList mapAreas)
		{
			if (mapAreas == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ImageMapAreaInstanceList;
			m_writer.StartObject(objectType);
			m_writer.StartArray(mapAreas.Count);
			for (int i = 0; i < mapAreas.Count; i++)
			{
				WriteImageMapAreaInstance(mapAreas[i]);
			}
			m_writer.EndArray();
			m_writer.EndObject();
		}

		private void WriteSubReportList(SubReportList subReports)
		{
			if (subReports == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.SubReportList;
			m_writer.StartObject(objectType);
			m_writer.StartArray(subReports.Count);
			for (int i = 0; i < subReports.Count; i++)
			{
				WriteSubReportReference(subReports[i]);
			}
			m_writer.EndArray();
			m_writer.EndObject();
		}

		private void WriteSubReportReference(SubReport subReport)
		{
			if (subReport == null)
			{
				m_writer.WriteNull();
			}
			else
			{
				m_writer.WriteReference(ObjectType.SubReport, subReport.ID);
			}
		}

		private void WriteNonComputedUniqueNamess(NonComputedUniqueNames[] names)
		{
			if (names == null)
			{
				m_writer.WriteNull();
				return;
			}
			m_writer.StartArray(names.Length);
			for (int i = 0; i < names.Length; i++)
			{
				WriteNonComputedUniqueNames(names[i]);
			}
			m_writer.EndArray();
		}

		private void WriteReportItemInstanceList(ReportItemInstanceList reportItemInstances)
		{
			if (reportItemInstances == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ReportItemInstanceList;
			m_writer.StartObject(objectType);
			m_writer.StartArray(reportItemInstances.Count);
			for (int i = 0; i < reportItemInstances.Count; i++)
			{
				WriteReportItemInstance(reportItemInstances[i]);
			}
			m_writer.EndArray();
			m_writer.EndObject();
		}

		private void WriteRenderingPagesRangesList(RenderingPagesRangesList renderingPagesRanges)
		{
			if (renderingPagesRanges == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.RenderingPagesRangesList;
			m_writer.StartObject(objectType);
			m_writer.StartArray(renderingPagesRanges.Count);
			for (int i = 0; i < renderingPagesRanges.Count; i++)
			{
				WriteRenderingPagesRanges(renderingPagesRanges[i]);
			}
			m_writer.EndArray();
			m_writer.EndObject();
		}

		private void WriteListContentInstanceList(ListContentInstanceList listContents)
		{
			if (listContents == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ListContentInstanceList;
			m_writer.StartObject(objectType);
			m_writer.StartArray(listContents.Count);
			for (int i = 0; i < listContents.Count; i++)
			{
				WriteListContentInstance(listContents[i]);
			}
			m_writer.EndArray();
			m_writer.EndObject();
		}

		private void WriteMatrixHeadingInstanceList(MatrixHeadingInstanceList matrixheadingInstances)
		{
			if (matrixheadingInstances == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.MatrixHeadingInstanceList;
			m_writer.StartObject(objectType);
			m_writer.StartArray(matrixheadingInstances.Count);
			for (int i = 0; i < matrixheadingInstances.Count; i++)
			{
				WriteMatrixHeadingInstance(matrixheadingInstances[i]);
			}
			m_writer.EndArray();
			m_writer.EndObject();
		}

		private void WriteMatrixCellInstancesList(MatrixCellInstancesList matrixCellInstancesList)
		{
			if (matrixCellInstancesList == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.MatrixCellInstancesList;
			m_writer.StartObject(objectType);
			m_writer.StartArray(matrixCellInstancesList.Count);
			for (int i = 0; i < matrixCellInstancesList.Count; i++)
			{
				WriteMatrixCellInstanceList(matrixCellInstancesList[i]);
			}
			m_writer.EndArray();
			m_writer.EndObject();
		}

		private void WriteMatrixCellInstanceList(MatrixCellInstanceList matrixCellInstances)
		{
			if (matrixCellInstances == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.MatrixCellInstanceList;
			m_writer.StartObject(objectType);
			m_writer.StartArray(matrixCellInstances.Count);
			for (int i = 0; i < matrixCellInstances.Count; i++)
			{
				MatrixSubtotalCellInstance matrixSubtotalCellInstance = matrixCellInstances[i] as MatrixSubtotalCellInstance;
				if (matrixSubtotalCellInstance != null)
				{
					WriteMatrixSubtotalCellInstance(matrixSubtotalCellInstance);
				}
				else
				{
					WriteMatrixCellInstance(matrixCellInstances[i]);
				}
			}
			m_writer.EndArray();
			m_writer.EndObject();
		}

		private void WriteMultiChartInstanceList(MultiChartInstanceList multichartInstances)
		{
			if (multichartInstances == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.MultiChartInstanceList;
			m_writer.StartObject(objectType);
			m_writer.StartArray(multichartInstances.Count);
			for (int i = 0; i < multichartInstances.Count; i++)
			{
				WriteMultiChartInstance(multichartInstances[i]);
			}
			m_writer.EndArray();
			m_writer.EndObject();
		}

		private void WriteChartHeadingInstanceList(ChartHeadingInstanceList chartheadingInstances)
		{
			if (chartheadingInstances == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ChartHeadingInstanceList;
			m_writer.StartObject(objectType);
			m_writer.StartArray(chartheadingInstances.Count);
			for (int i = 0; i < chartheadingInstances.Count; i++)
			{
				WriteChartHeadingInstance(chartheadingInstances[i]);
			}
			m_writer.EndArray();
			m_writer.EndObject();
		}

		private void WriteChartDataPointInstancesList(ChartDataPointInstancesList chartDataPointInstancesList)
		{
			if (chartDataPointInstancesList == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ChartDataPointInstancesList;
			m_writer.StartObject(objectType);
			m_writer.StartArray(chartDataPointInstancesList.Count);
			for (int i = 0; i < chartDataPointInstancesList.Count; i++)
			{
				WriteChartDataPointInstanceList(chartDataPointInstancesList[i]);
			}
			m_writer.EndArray();
			m_writer.EndObject();
		}

		private void WriteChartDataPointInstanceList(ChartDataPointInstanceList chartDataPointInstanceList)
		{
			if (chartDataPointInstanceList == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ChartDataPointInstanceList;
			m_writer.StartObject(objectType);
			m_writer.StartArray(chartDataPointInstanceList.Count);
			for (int i = 0; i < chartDataPointInstanceList.Count; i++)
			{
				WriteChartDataPointInstance(chartDataPointInstanceList[i]);
			}
			m_writer.EndArray();
			m_writer.EndObject();
		}

		private void WriteTableRowInstances(TableRowInstance[] tableRowInstances)
		{
			if (tableRowInstances == null)
			{
				m_writer.WriteNull();
				return;
			}
			m_writer.StartArray(tableRowInstances.Length);
			for (int i = 0; i < tableRowInstances.Length; i++)
			{
				WriteTableRowInstance(tableRowInstances[i]);
			}
			m_writer.EndArray();
		}

		private void WriteTableDetailInstanceList(TableDetailInstanceList tableDetailInstanceList)
		{
			if (tableDetailInstanceList == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.TableDetailInstanceList;
			m_writer.StartObject(objectType);
			m_writer.StartArray(tableDetailInstanceList.Count);
			for (int i = 0; i < tableDetailInstanceList.Count; i++)
			{
				WriteTableDetailInstance(tableDetailInstanceList[i]);
			}
			m_writer.EndArray();
			m_writer.EndObject();
		}

		private void WriteTableGroupInstanceList(TableGroupInstanceList tableGroupInstances)
		{
			if (tableGroupInstances == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.TableGroupInstanceList;
			m_writer.StartObject(objectType);
			m_writer.StartArray(tableGroupInstances.Count);
			for (int i = 0; i < tableGroupInstances.Count; i++)
			{
				WriteTableGroupInstance(tableGroupInstances[i]);
			}
			m_writer.EndArray();
			m_writer.EndObject();
		}

		private void WriteTableColumnInstances(TableColumnInstance[] tableColumnInstances)
		{
			if (tableColumnInstances == null)
			{
				m_writer.WriteNull();
				return;
			}
			m_writer.StartArray(tableColumnInstances.Length);
			for (int i = 0; i < tableColumnInstances.Length; i++)
			{
				WriteTableColumnInstance(tableColumnInstances[i]);
			}
			m_writer.EndArray();
		}

		private void WriteCustomReportItemHeadingInstanceList(CustomReportItemHeadingInstanceList headingInstances)
		{
			if (headingInstances == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.CustomReportItemHeadingInstanceList;
			m_writer.StartObject(objectType);
			m_writer.StartArray(headingInstances.Count);
			for (int i = 0; i < headingInstances.Count; i++)
			{
				WriteCustomReportItemHeadingInstance(headingInstances[i]);
			}
			m_writer.EndArray();
			m_writer.EndObject();
		}

		private void WriteCustomReportItemCellInstancesList(CustomReportItemCellInstancesList cellInstancesList)
		{
			if (cellInstancesList == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.CustomReportItemCellInstancesList;
			m_writer.StartObject(objectType);
			m_writer.StartArray(cellInstancesList.Count);
			for (int i = 0; i < cellInstancesList.Count; i++)
			{
				WriteCustomReportItemCellInstanceList(cellInstancesList[i]);
			}
			m_writer.EndArray();
			m_writer.EndObject();
		}

		private void WriteCustomReportItemCellInstanceList(CustomReportItemCellInstanceList cellInstances)
		{
			if (cellInstances == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.CustomReportItemCellInstanceList;
			m_writer.StartObject(objectType);
			m_writer.StartArray(cellInstances.Count);
			for (int i = 0; i < cellInstances.Count; i++)
			{
				WriteCustomReportItemCellInstance(cellInstances[i]);
			}
			m_writer.EndArray();
			m_writer.EndObject();
		}

		private void WriteDocumentMapNodes(DocumentMapNode[] nodes)
		{
			if (nodes == null)
			{
				m_writer.WriteNull();
				return;
			}
			m_writer.StartArray(nodes.Length);
			for (int i = 0; i < nodes.Length; i++)
			{
				WriteDocumentMapNode(nodes[i]);
			}
			m_writer.EndArray();
		}

		private void WriteStrings(string[] strings)
		{
			if (strings == null)
			{
				m_writer.WriteNull();
				return;
			}
			m_writer.StartArray(strings.Length);
			for (int i = 0; i < strings.Length; i++)
			{
				m_writer.WriteString(strings[i]);
			}
			m_writer.EndArray();
		}

		private void WriteVariants(object[] variants)
		{
			WriteVariants(variants, isMultiValue: false);
		}

		private void WriteVariants(object[] variants, bool isMultiValue)
		{
			if (variants == null)
			{
				m_writer.WriteNull();
				return;
			}
			m_writer.StartArray(variants.Length);
			for (int i = 0; i < variants.Length; i++)
			{
				if (isMultiValue && variants[i] is object[])
				{
					WriteVariants(variants[i] as object[], isMultiValue: false);
				}
				else
				{
					WriteVariant(variants[i]);
				}
			}
			m_writer.EndArray();
		}

		private void WriteVariantList(VariantList variants, bool convertDBNull)
		{
			if (variants == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.VariantList;
			m_writer.StartObject(objectType);
			m_writer.StartArray(variants.Count);
			for (int i = 0; i < variants.Count; i++)
			{
				WriteVariant(variants[i], convertDBNull);
			}
			m_writer.EndArray();
			m_writer.EndObject();
		}

		private void WriteVariantLists(VariantList[] variantLists, bool convertDBNull)
		{
			if (variantLists == null)
			{
				m_writer.WriteNull();
				return;
			}
			m_writer.StartArray(variantLists.Length);
			for (int i = 0; i < variantLists.Length; i++)
			{
				WriteVariantList(variantLists[i], convertDBNull);
			}
			m_writer.EndArray();
		}

		private void WriteProcessingMessageList(ProcessingMessageList messages)
		{
			if (messages == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ProcessingMessageList;
			m_writer.StartObject(objectType);
			m_writer.StartArray(messages.Count);
			for (int i = 0; i < messages.Count; i++)
			{
				WriteProcessingMessage(messages[i]);
			}
			m_writer.EndArray();
			m_writer.EndObject();
		}

		private void WriteIDOwnerBase(IDOwner idOwner)
		{
			Assert(idOwner != null);
			ObjectType objectType = ObjectType.IDOwner;
			DeclareType(objectType);
			m_writer.WriteInt32(idOwner.ID);
		}

		private void WriteReportItem(ReportItem reportItem)
		{
			if (reportItem == null)
			{
				m_writer.WriteNull();
				return;
			}
			if (reportItem is Line)
			{
				WriteLine((Line)reportItem);
				return;
			}
			if (reportItem is Rectangle)
			{
				WriteRectangle((Rectangle)reportItem);
				return;
			}
			if (reportItem is Image)
			{
				WriteImage((Image)reportItem);
				return;
			}
			if (reportItem is CheckBox)
			{
				WriteCheckBox((CheckBox)reportItem);
				return;
			}
			if (reportItem is TextBox)
			{
				WriteTextBox((TextBox)reportItem);
				return;
			}
			if (reportItem is SubReport)
			{
				WriteSubReport((SubReport)reportItem);
				return;
			}
			if (reportItem is ActiveXControl)
			{
				WriteActiveXControl((ActiveXControl)reportItem);
				return;
			}
			Assert(reportItem is DataRegion);
			WriteDataRegion((DataRegion)reportItem);
		}

		private void WriteReportItemBase(ReportItem reportItem)
		{
			Assert(reportItem != null);
			ObjectType objectType = ObjectType.ReportItem;
			DeclareType(objectType);
			WriteIDOwnerBase(reportItem);
			m_writer.WriteString(reportItem.Name);
			WriteStyle(reportItem.StyleClass);
			m_writer.WriteString(reportItem.Top);
			m_writer.WriteDouble(reportItem.TopValue);
			m_writer.WriteString(reportItem.Left);
			m_writer.WriteDouble(reportItem.LeftValue);
			m_writer.WriteString(reportItem.Height);
			m_writer.WriteDouble(reportItem.HeightValue);
			m_writer.WriteString(reportItem.Width);
			m_writer.WriteDouble(reportItem.WidthValue);
			m_writer.WriteInt32(reportItem.ZIndex);
			WriteVisibility(reportItem.Visibility);
			WriteExpressionInfo(reportItem.ToolTip);
			WriteExpressionInfo(reportItem.Label);
			WriteExpressionInfo(reportItem.Bookmark);
			m_writer.WriteString(reportItem.Custom);
			m_writer.WriteBoolean(reportItem.RepeatedSibling);
			m_writer.WriteBoolean(reportItem.IsFullSize);
			m_writer.WriteInt32(reportItem.ExprHostID);
			m_writer.WriteString(reportItem.DataElementName);
			WriteDataElementOutputType(reportItem.DataElementOutput);
			m_writer.WriteInt32(reportItem.DistanceFromReportTop);
			m_writer.WriteInt32(reportItem.DistanceBeforeTop);
			WriteIntList(reportItem.SiblingAboveMe);
			WriteDataValueList(reportItem.CustomProperties);
		}

		private void WriteReportItemReference(ReportItem reportItem)
		{
			if (reportItem == null)
			{
				m_writer.WriteNull();
			}
			else
			{
				m_writer.WriteNoTypeReference(reportItem.ID);
			}
		}

		private void WritePageSection(PageSection pageSection)
		{
			if (pageSection == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.PageSection;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteReportItemBase(pageSection);
			m_writer.WriteBoolean(pageSection.PrintOnFirstPage);
			m_writer.WriteBoolean(pageSection.PrintOnLastPage);
			WriteReportItemCollection(pageSection.ReportItems);
			m_writer.WriteBoolean(pageSection.PostProcessEvaluate);
			m_writer.EndObject();
		}

		private void WriteReportItemCollection(ReportItemCollection reportItems)
		{
			if (reportItems == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ReportItemCollection;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteIDOwnerBase(reportItems);
			WriteReportItemList(reportItems.NonComputedReportItems);
			WriteReportItemList(reportItems.ComputedReportItems);
			WriteReportItemIndexerList(reportItems.SortedReportItems);
			WriteRunningValueInfoList(reportItems.RunningValues);
			m_writer.EndObject();
		}

		private void WriteShowHideTypes(Report.ShowHideTypes showHideType)
		{
			m_writer.WriteEnum((int)showHideType);
		}

		private void WriteDataElementOutputType(DataElementOutputTypes element)
		{
			m_writer.WriteEnum((int)element);
		}

		private void WriteStyle(Style style)
		{
			if (style == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.Style;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteStyleAttributeHashtable(style.StyleAttributes);
			WriteExpressionInfoList(style.ExpressionList);
			m_writer.EndObject();
		}

		private void WriteVisibility(Visibility visibility)
		{
			if (visibility == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.Visibility;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteExpressionInfo(visibility.Hidden);
			m_writer.WriteString(visibility.Toggle);
			m_writer.WriteBoolean(visibility.RecursiveReceiver);
			m_writer.EndObject();
		}

		private void WriteFilter(Filter filter)
		{
			if (filter == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.Filter;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteExpressionInfo(filter.Expression);
			WriteOperators(filter.Operator);
			WriteExpressionInfoList(filter.Values);
			m_writer.WriteInt32(filter.ExprHostID);
			m_writer.EndObject();
		}

		private void WriteOperators(Filter.Operators operators)
		{
			m_writer.WriteEnum((int)operators);
		}

		private void WriteDataSource(DataSource dataSource)
		{
			if (dataSource == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.DataSource;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			m_writer.WriteString(dataSource.Name);
			m_writer.WriteBoolean(dataSource.Transaction);
			m_writer.WriteString(dataSource.Type);
			WriteExpressionInfo(dataSource.ConnectStringExpression);
			m_writer.WriteBoolean(dataSource.IntegratedSecurity);
			m_writer.WriteString(dataSource.Prompt);
			m_writer.WriteString(dataSource.DataSourceReference);
			WriteDataSetList(dataSource.DataSets);
			m_writer.WriteGuid(dataSource.ID);
			m_writer.WriteInt32(dataSource.ExprHostID);
			m_writer.WriteString(dataSource.SharedDataSourceReferencePath);
			m_writer.EndObject();
		}

		private void WriteDataAggregateInfo(DataAggregateInfo aggregate)
		{
			if (aggregate == null)
			{
				m_writer.WriteNull();
				return;
			}
			if (aggregate is RunningValueInfo)
			{
				WriteRunningValueInfo((RunningValueInfo)aggregate);
				return;
			}
			ObjectType objectType = ObjectType.DataAggregateInfo;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteDataAggregateInfoBase(aggregate);
			m_writer.EndObject();
		}

		private void WriteDataAggregateInfoBase(DataAggregateInfo aggregate)
		{
			Assert(aggregate != null);
			ObjectType objectType = ObjectType.DataAggregateInfo;
			DeclareType(objectType);
			m_writer.WriteString(aggregate.Name);
			WriteAggregateTypes(aggregate.AggregateType);
			WriteExpressionInfos(aggregate.Expressions);
			WriteStringList(aggregate.DuplicateNames);
		}

		private void WriteExpressionInfo(ExpressionInfo expression)
		{
			if (expression == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ExpressionInfo;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteTypes(expression.Type);
			m_writer.WriteString(expression.Value);
			m_writer.WriteBoolean(expression.BoolValue);
			m_writer.WriteInt32(expression.IntValue);
			m_writer.WriteInt32(expression.ExprHostID);
			m_writer.WriteString(expression.OriginalText);
			m_writer.EndObject();
		}

		private void WriteAggregateTypes(DataAggregateInfo.AggregateTypes aggregateType)
		{
			m_writer.WriteEnum((int)aggregateType);
		}

		private void WriteTypes(ExpressionInfo.Types type)
		{
			m_writer.WriteEnum((int)type);
		}

		private void WriteReportItemIndexer(ReportItemIndexer reportItemIndexer)
		{
			ObjectType objectType = ObjectType.ReportItemIndexer;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			m_writer.WriteBoolean(reportItemIndexer.IsComputed);
			m_writer.WriteInt32(reportItemIndexer.Index);
			m_writer.EndObject();
		}

		private void WriteRenderingPagesRanges(RenderingPagesRanges renderingPagesRanges)
		{
			ObjectType objectType = ObjectType.RenderingPagesRanges;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			m_writer.WriteInt32(renderingPagesRanges.StartPage);
			m_writer.WriteInt32(renderingPagesRanges.EndPage);
			m_writer.EndObject();
		}

		private void WriteRunningValueInfo(RunningValueInfo runningValue)
		{
			if (runningValue == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.RunningValueInfo;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteDataAggregateInfoBase(runningValue);
			m_writer.WriteString(runningValue.Scope);
			m_writer.EndObject();
		}

		private void WriteAttributeInfo(AttributeInfo attributeInfo)
		{
			if (attributeInfo == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.AttributeInfo;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			m_writer.WriteBoolean(attributeInfo.IsExpression);
			m_writer.WriteString(attributeInfo.Value);
			m_writer.WriteBoolean(attributeInfo.BoolValue);
			m_writer.WriteInt32(attributeInfo.IntValue);
			m_writer.EndObject();
		}

		private void WriteDataSet(DataSet dataSet)
		{
			if (dataSet == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.DataSet;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			Global.Tracer.Assert(dataSet.ID > 0);
			WriteIDOwnerBase(dataSet);
			m_writer.WriteString(dataSet.Name);
			dataSet.PopulateReferencedFieldProperties();
			WriteDataFieldList(dataSet.Fields);
			WriteReportQuery(dataSet.Query);
			WriteSensitivity(dataSet.CaseSensitivity);
			m_writer.WriteString(dataSet.Collation);
			WriteSensitivity(dataSet.AccentSensitivity);
			WriteSensitivity(dataSet.KanatypeSensitivity);
			WriteSensitivity(dataSet.WidthSensitivity);
			WriteDataRegionList(dataSet.DataRegions);
			WriteDataAggregateInfoList(dataSet.Aggregates);
			WriteFilterList(dataSet.Filters);
			m_writer.WriteInt32(dataSet.RecordSetSize);
			m_writer.WriteBoolean(dataSet.UsedOnlyInParameters);
			m_writer.WriteInt32(dataSet.NonCalculatedFieldCount);
			m_writer.WriteInt32(dataSet.ExprHostID);
			WriteDataAggregateInfoList(dataSet.PostSortAggregates);
			m_writer.WriteInt32((int)dataSet.LCID);
			m_writer.WriteBoolean(dataSet.HasDetailUserSortFilter);
			WriteExpressionInfoList(dataSet.UserSortExpressions);
			m_writer.WriteBoolean(dataSet.DynamicFieldReferences);
			if (dataSet.InterpretSubtotalsAsDetailsIsAuto)
			{
				dataSet.InterpretSubtotalsAsDetails = true;
			}
			m_writer.WriteBoolean(dataSet.InterpretSubtotalsAsDetails);
			m_writer.EndObject();
		}

		private void WriteReportQuery(ReportQuery query)
		{
			if (query == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ReportQuery;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteCommandType(query.CommandType);
			WriteExpressionInfo(query.CommandText);
			WriteParameterValueList(query.Parameters);
			m_writer.WriteInt32(query.TimeOut);
			m_writer.WriteString(query.CommandTextValue);
			m_writer.WriteString(query.RewrittenCommandText);
			m_writer.EndObject();
		}

		private void WriteSensitivity(DataSet.Sensitivity sensitivity)
		{
			m_writer.WriteEnum((int)sensitivity);
		}

		private void WriteCommandType(CommandType commandType)
		{
			m_writer.WriteEnum((int)commandType);
		}

		private void WriteDataField(Field field)
		{
			if (field == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.Field;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			m_writer.WriteString(field.Name);
			m_writer.WriteString(field.DataField);
			WriteExpressionInfo(field.Value);
			m_writer.WriteInt32(field.ExprHostID);
			m_writer.WriteBoolean(field.DynamicPropertyReferences);
			WriteFieldPropertyHashtable(field.ReferencedProperties);
			m_writer.EndObject();
		}

		internal void WriteFieldPropertyHashtable(FieldPropertyHashtable properties)
		{
			if (properties == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.FieldPropertyHashtable;
			m_writer.StartObject(objectType);
			m_writer.StartArray(properties.Count);
			int num = 0;
			IDictionaryEnumerator enumerator = properties.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string stringValue = enumerator.Key as string;
				m_writer.WriteString(stringValue);
				num++;
			}
			Assert(num == properties.Count);
			m_writer.EndArray();
			m_writer.EndObject();
		}

		private void WriteParameterValue(ParameterValue parameter)
		{
			ObjectType objectType = ObjectType.ParameterValue;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			m_writer.WriteString(parameter.Name);
			WriteExpressionInfo(parameter.Value);
			m_writer.WriteInt32(parameter.ExprHostID);
			WriteExpressionInfo(parameter.Omit);
			m_writer.EndObject();
		}

		private void WriteCodeClass(CodeClass codeClass)
		{
			ObjectType objectType = ObjectType.CodeClass;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			m_writer.WriteString(codeClass.ClassName);
			m_writer.WriteString(codeClass.InstanceName);
			m_writer.EndObject();
		}

		private void WriteAction(Action actionInfo)
		{
			if (actionInfo == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.Action;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteActionItemList(actionInfo.ActionItems);
			WriteStyle(actionInfo.StyleClass);
			m_writer.WriteInt32(actionInfo.ComputedActionItemsCount);
			m_writer.EndObject();
		}

		private void WriteActionItemList(ActionItemList actionItemList)
		{
			if (actionItemList == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ActionItemList;
			m_writer.StartObject(objectType);
			m_writer.StartArray(actionItemList.Count);
			for (int i = 0; i < actionItemList.Count; i++)
			{
				WriteActionItem(actionItemList[i]);
			}
			m_writer.EndArray();
			m_writer.EndObject();
		}

		private void WriteActionItem(ActionItem actionItem)
		{
			if (actionItem == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ActionItem;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteExpressionInfo(actionItem.HyperLinkURL);
			WriteExpressionInfo(actionItem.DrillthroughReportName);
			WriteParameterValueList(actionItem.DrillthroughParameters);
			WriteExpressionInfo(actionItem.DrillthroughBookmarkLink);
			WriteExpressionInfo(actionItem.BookmarkLink);
			WriteExpressionInfo(actionItem.Label);
			m_writer.WriteInt32(actionItem.ExprHostID);
			m_writer.WriteInt32(actionItem.ComputedIndex);
			m_writer.EndObject();
		}

		private void WriteLine(Line line)
		{
			if (line == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.Line;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteReportItemBase(line);
			m_writer.WriteBoolean(line.LineSlant);
			m_writer.EndObject();
		}

		private void WriteRectangle(Rectangle rectangle)
		{
			if (rectangle == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.Rectangle;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteReportItemBase(rectangle);
			WriteReportItemCollection(rectangle.ReportItems);
			m_writer.WriteBoolean(rectangle.PageBreakAtEnd);
			m_writer.WriteBoolean(rectangle.PageBreakAtStart);
			m_writer.WriteInt32(rectangle.LinkToChild);
			m_writer.EndObject();
		}

		private void WriteImage(Image image)
		{
			if (image == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.Image;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteReportItemBase(image);
			WriteAction(image.Action);
			WriteSourceType(image.Source);
			WriteExpressionInfo(image.Value);
			WriteExpressionInfo(image.MIMEType);
			WriteSizings(image.Sizing);
			m_writer.EndObject();
		}

		private void WriteImageMapAreaInstance(ImageMapAreaInstance mapArea)
		{
			if (mapArea == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ImageMapAreaInstance;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			m_writer.WriteString(mapArea.ID);
			WriteImageMapAreaShape(mapArea.Shape);
			m_writer.WriteFloatArray(mapArea.Coordinates);
			WriteAction(mapArea.Action);
			WriteActionInstance(mapArea.ActionInstance);
			m_writer.WriteInt32(mapArea.UniqueName);
			m_writer.EndObject();
		}

		private void WriteImageMapAreaShape(ImageMapArea.ImageMapAreaShape sourceType)
		{
			m_writer.WriteEnum((int)sourceType);
		}

		private void WriteSourceType(Image.SourceType sourceType)
		{
			m_writer.WriteEnum((int)sourceType);
		}

		private void WriteSizings(Image.Sizings sizing)
		{
			m_writer.WriteEnum((int)sizing);
		}

		private void WriteCheckBox(CheckBox checkBox)
		{
			if (checkBox == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.CheckBox;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteReportItemBase(checkBox);
			WriteExpressionInfo(checkBox.Value);
			m_writer.WriteString(checkBox.HideDuplicates);
			m_writer.EndObject();
		}

		private void WriteTextBox(TextBox textBox)
		{
			if (textBox == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.TextBox;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteReportItemBase(textBox);
			WriteExpressionInfo(textBox.Value);
			m_writer.WriteBoolean(textBox.CanGrow);
			m_writer.WriteBoolean(textBox.CanShrink);
			m_writer.WriteString(textBox.HideDuplicates);
			WriteAction(textBox.Action);
			m_writer.WriteBoolean(textBox.IsToggle);
			WriteExpressionInfo(textBox.InitialToggleState);
			WriteTypeCode(textBox.ValueType);
			m_writer.WriteString(textBox.Formula);
			m_writer.WriteBoolean(textBox.ValueReferenced);
			m_writer.WriteBoolean(textBox.RecursiveSender);
			m_writer.WriteBoolean(textBox.DataElementStyleAttribute);
			WriteGroupingReferenceList(textBox.ContainingScopes);
			WriteEndUserSort(textBox.UserSort);
			m_writer.WriteBoolean(textBox.IsMatrixCellScope);
			m_writer.WriteBoolean(textBox.IsSubReportTopLevelScope);
			m_writer.EndObject();
		}

		private void WriteEndUserSort(EndUserSort userSort)
		{
			if (userSort == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.EndUserSort;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			m_writer.WriteInt32(userSort.DataSetID);
			WriteSortFilterScopeReference(userSort.SortExpressionScope);
			WriteGroupingReferenceList(userSort.GroupsInSortTarget);
			WriteSortFilterScopeReference(userSort.SortTarget);
			m_writer.WriteInt32(userSort.SortExpressionIndex);
			WriteSubReportList(userSort.DetailScopeSubReports);
			m_writer.EndObject();
		}

		private void WriteSortFilterScopeReference(ISortFilterScope sortFilterScope)
		{
			if (sortFilterScope == null)
			{
				m_writer.WriteNull();
			}
			else
			{
				m_writer.WriteReference(ObjectType.ISortFilterScope, sortFilterScope.ID);
			}
		}

		private void WriteGroupingReferenceList(GroupingList groups)
		{
			if (groups == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.GroupingList;
			m_writer.StartObject(objectType);
			m_writer.StartArray(groups.Count);
			for (int i = 0; i < groups.Count; i++)
			{
				if (groups[i] == null)
				{
					m_writer.WriteNull();
				}
				else
				{
					m_writer.WriteReference(ObjectType.Grouping, groups[i].Owner.ID);
				}
			}
			m_writer.EndArray();
			m_writer.EndObject();
		}

		private void WriteTypeCode(TypeCode typeCode)
		{
			m_writer.WriteEnum((int)typeCode);
		}

		private void WriteSubReport(SubReport subReport)
		{
			if (subReport == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.SubReport;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteReportItemBase(subReport);
			m_writer.WriteString(subReport.ReportPath);
			WriteParameterValueList(subReport.Parameters);
			WriteExpressionInfo(subReport.NoRows);
			m_writer.WriteBoolean(subReport.MergeTransactions);
			WriteGroupingReferenceList(subReport.ContainingScopes);
			m_writer.WriteBoolean(subReport.IsMatrixCellScope);
			WriteScopeLookupTable(subReport.DataSetUniqueNameMap);
			WriteStatus(subReport.RetrievalStatus);
			m_writer.WriteString(subReport.ReportName);
			m_writer.WriteString(subReport.Description);
			WriteReport(subReport.Report);
			m_writer.WriteString(subReport.StringUri);
			WriteParameterInfoCollection(subReport.ParametersFromCatalog);
			m_writer.EndObject();
		}

		private void WriteScopeLookupTable(ScopeLookupTable scopeTable)
		{
			if (scopeTable == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ScopeLookupTable;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteScopeTableValues(scopeTable.LookupTable);
			m_writer.EndObject();
		}

		private void WriteScopeTableValues(object value)
		{
			if (value is int)
			{
				m_writer.WriteInt32((int)value);
				return;
			}
			Global.Tracer.Assert(value is Hashtable);
			Hashtable hashtable = (Hashtable)value;
			m_writer.StartArray(hashtable.Count);
			int num = 0;
			IDictionaryEnumerator enumerator = hashtable.GetEnumerator();
			while (enumerator.MoveNext())
			{
				WriteVariant(enumerator.Key, convertDBNull: true);
				WriteScopeTableValues(enumerator.Value);
				num++;
			}
			Assert(num == hashtable.Count);
			m_writer.EndArray();
		}

		private void WriteStatus(SubReport.Status status)
		{
			m_writer.WriteEnum((int)status);
		}

		private void WriteActiveXControl(ActiveXControl control)
		{
			if (control == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ActiveXControl;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteReportItemBase(control);
			m_writer.WriteString(control.ClassID);
			m_writer.WriteString(control.CodeBase);
			WriteParameterValueList(control.Parameters);
			m_writer.EndObject();
		}

		private void WriteParameterBase(ParameterBase parameter)
		{
			Assert(parameter != null);
			ObjectType objectType = ObjectType.ParameterBase;
			DeclareType(objectType);
			m_writer.WriteString(parameter.Name);
			WriteDataType(parameter.DataType);
			m_writer.WriteBoolean(parameter.Nullable);
			m_writer.WriteString(parameter.Prompt);
			m_writer.WriteBoolean(parameter.UsedInQuery);
			m_writer.WriteBoolean(parameter.AllowBlank);
			m_writer.WriteBoolean(parameter.MultiValue);
			WriteVariants(parameter.DefaultValues);
			m_writer.WriteBoolean(parameter.PromptUser);
		}

		private void WriteParameterDef(ParameterDef parameter)
		{
			if (parameter == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ParameterDef;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteParameterBase(parameter);
			WriteParameterDataSource(parameter.ValidValuesDataSource);
			WriteExpressionInfoList(parameter.ValidValuesValueExpressions);
			WriteExpressionInfoList(parameter.ValidValuesLabelExpressions);
			WriteParameterDataSource(parameter.DefaultDataSource);
			WriteExpressionInfoList(parameter.DefaultExpressions);
			WriteParameterDefRefList(parameter.DependencyList);
			m_writer.WriteInt32(parameter.ExprHostID);
			m_writer.EndObject();
		}

		private void WriteParameterDataSource(ParameterDataSource paramDataSource)
		{
			if (paramDataSource == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ParameterDataSource;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			m_writer.WriteInt32(paramDataSource.DataSourceIndex);
			m_writer.WriteInt32(paramDataSource.DataSetIndex);
			m_writer.WriteInt32(paramDataSource.ValueFieldIndex);
			m_writer.WriteInt32(paramDataSource.LabelFieldIndex);
			m_writer.EndObject();
		}

		private void WriteValidValue(ValidValue validValue)
		{
			if (validValue == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ValidValue;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			m_writer.WriteString(validValue.LabelRaw);
			WriteVariant(validValue.Value);
			m_writer.EndObject();
		}

		private void WriteDataRegion(DataRegion dataRegion)
		{
			if (dataRegion == null)
			{
				m_writer.WriteNull();
				return;
			}
			if (dataRegion is List)
			{
				WriteList((List)dataRegion);
				return;
			}
			if (dataRegion is Matrix)
			{
				WriteMatrix((Matrix)dataRegion);
				return;
			}
			if (dataRegion is Table)
			{
				WriteTable((Table)dataRegion);
				return;
			}
			if (dataRegion is Chart)
			{
				WriteChart((Chart)dataRegion);
				return;
			}
			if (dataRegion is CustomReportItem)
			{
				WriteCustomReportItem((CustomReportItem)dataRegion);
				return;
			}
			Assert(dataRegion is OWCChart);
			WriteOWCChart((OWCChart)dataRegion);
		}

		private void WriteDataRegionBase(DataRegion dataRegion)
		{
			Assert(dataRegion != null);
			ObjectType objectType = ObjectType.DataRegion;
			DeclareType(objectType);
			WriteReportItemBase(dataRegion);
			m_writer.WriteString(dataRegion.DataSetName);
			WriteExpressionInfo(dataRegion.NoRows);
			m_writer.WriteBoolean(dataRegion.PageBreakAtEnd);
			m_writer.WriteBoolean(dataRegion.PageBreakAtStart);
			m_writer.WriteBoolean(dataRegion.KeepTogether);
			WriteIntList(dataRegion.RepeatSiblings);
			WriteFilterList(dataRegion.Filters);
			WriteDataAggregateInfoList(dataRegion.Aggregates);
			WriteDataAggregateInfoList(dataRegion.PostSortAggregates);
			WriteExpressionInfoList(dataRegion.UserSortExpressions);
			WriteInScopeSortFilterHashtable(dataRegion.DetailSortFiltersInScope);
		}

		private void WriteDataRegionReference(DataRegion dataRegion)
		{
			if (dataRegion == null)
			{
				m_writer.WriteNull();
				return;
			}
			if (dataRegion is List)
			{
				m_writer.WriteReference(ObjectType.List, dataRegion.ID);
				return;
			}
			if (dataRegion is Table)
			{
				m_writer.WriteReference(ObjectType.Table, dataRegion.ID);
				return;
			}
			if (dataRegion is Matrix)
			{
				m_writer.WriteReference(ObjectType.Matrix, dataRegion.ID);
				return;
			}
			if (dataRegion is Chart)
			{
				m_writer.WriteReference(ObjectType.Chart, dataRegion.ID);
				return;
			}
			if (dataRegion is CustomReportItem)
			{
				m_writer.WriteReference(ObjectType.CustomReportItem, dataRegion.ID);
				return;
			}
			Assert(dataRegion is OWCChart);
			m_writer.WriteReference(ObjectType.OWCChart, dataRegion.ID);
		}

		private void WriteReportHierarchyNode(ReportHierarchyNode node)
		{
			if (node == null)
			{
				m_writer.WriteNull();
				return;
			}
			if (node is TableGroup)
			{
				WriteTableGroup((TableGroup)node);
				return;
			}
			if (node is MatrixHeading)
			{
				WriteMatrixHeading((MatrixHeading)node);
				return;
			}
			if (node is ChartHeading)
			{
				WriteChartHeading((ChartHeading)node);
				return;
			}
			if (node is MultiChart)
			{
				WriteMultiChart((MultiChart)node);
				return;
			}
			if (node is CustomReportItemHeading)
			{
				WriteCustomReportItemHeading((CustomReportItemHeading)node);
				return;
			}
			ObjectType objectType = ObjectType.ReportHierarchyNode;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteReportHierarchyNodeBase(node);
			m_writer.EndObject();
		}

		private void WriteReportHierarchyNodeBase(ReportHierarchyNode node)
		{
			Assert(node != null);
			ObjectType objectType = ObjectType.ReportHierarchyNode;
			DeclareType(objectType);
			WriteIDOwnerBase(node);
			WriteGrouping(node.Grouping);
			WriteSorting(node.Sorting);
			WriteReportHierarchyNode(node.InnerHierarchy);
			WriteDataRegionReference(node.DataRegionDef);
		}

		private void WriteGrouping(Grouping grouping)
		{
			if (grouping == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.Grouping;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			m_writer.WriteString(grouping.Name);
			WriteExpressionInfoList(grouping.GroupExpressions);
			WriteExpressionInfo(grouping.GroupLabel);
			WriteBoolList(grouping.SortDirections);
			m_writer.WriteBoolean(grouping.PageBreakAtEnd);
			m_writer.WriteBoolean(grouping.PageBreakAtStart);
			m_writer.WriteString(grouping.Custom);
			WriteDataAggregateInfoList(grouping.Aggregates);
			m_writer.WriteBoolean(grouping.GroupAndSort);
			WriteFilterList(grouping.Filters);
			WriteReportItemIDList(grouping.ReportItemsWithHideDuplicates);
			WriteExpressionInfoList(grouping.Parent);
			WriteDataAggregateInfoList(grouping.RecursiveAggregates);
			WriteDataAggregateInfoList(grouping.PostSortAggregates);
			m_writer.WriteString(grouping.DataElementName);
			m_writer.WriteString(grouping.DataCollectionName);
			WriteDataElementOutputType(grouping.DataElementOutput);
			WriteDataValueList(grouping.CustomProperties);
			m_writer.WriteBoolean(grouping.SaveGroupExprValues);
			WriteExpressionInfoList(grouping.UserSortExpressions);
			WriteInScopeSortFilterHashtable(grouping.NonDetailSortFiltersInScope);
			WriteInScopeSortFilterHashtable(grouping.DetailSortFiltersInScope);
			m_writer.EndObject();
		}

		private void WriteInScopeSortFilterHashtable(InScopeSortFilterHashtable inScopeSortFilters)
		{
			if (inScopeSortFilters == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.InScopeSortFilterHashtable;
			m_writer.StartObject(objectType);
			m_writer.StartArray(inScopeSortFilters.Count);
			int num = 0;
			IDictionaryEnumerator enumerator = inScopeSortFilters.GetEnumerator();
			while (enumerator.MoveNext())
			{
				int int32Value = (int)enumerator.Key;
				m_writer.WriteInt32(int32Value);
				WriteIntList((IntList)enumerator.Value);
				num++;
			}
			Assert(num == inScopeSortFilters.Count);
			m_writer.EndArray();
			m_writer.EndObject();
		}

		private void WriteSorting(Sorting sorting)
		{
			if (sorting == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.Sorting;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteExpressionInfoList(sorting.SortExpressions);
			WriteBoolList(sorting.SortDirections);
			m_writer.EndObject();
		}

		private void WriteTableGroup(TableGroup tableGroup)
		{
			if (tableGroup == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.TableGroup;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteReportHierarchyNodeBase(tableGroup);
			WriteTableRowList(tableGroup.HeaderRows);
			m_writer.WriteBoolean(tableGroup.HeaderRepeatOnNewPage);
			WriteTableRowList(tableGroup.FooterRows);
			m_writer.WriteBoolean(tableGroup.FooterRepeatOnNewPage);
			WriteVisibility(tableGroup.Visibility);
			m_writer.WriteBoolean(tableGroup.PropagatedPageBreakAtStart);
			m_writer.WriteBoolean(tableGroup.PropagatedPageBreakAtEnd);
			WriteRunningValueInfoList(tableGroup.RunningValues);
			m_writer.WriteBoolean(tableGroup.HasExprHost);
			m_writer.EndObject();
		}

		private void WriteTableGroupReference(TableGroup tableGroup)
		{
			if (tableGroup == null)
			{
				m_writer.WriteNull();
			}
			else
			{
				m_writer.WriteReference(ObjectType.TableGroup, tableGroup.ID);
			}
		}

		private void WriteTableDetail(TableDetail tableDetail)
		{
			if (tableDetail == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.TableDetail;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteIDOwnerBase(tableDetail);
			WriteTableRowList(tableDetail.DetailRows);
			WriteSorting(tableDetail.Sorting);
			WriteVisibility(tableDetail.Visibility);
			WriteRunningValueInfoList(tableDetail.RunningValues);
			m_writer.WriteBoolean(tableDetail.HasExprHost);
			m_writer.WriteBoolean(tableDetail.SimpleDetailRows);
			m_writer.EndObject();
		}

		private void WritePivotHeadingBase(PivotHeading pivotHeading)
		{
			if (pivotHeading == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.PivotHeading;
			DeclareType(objectType);
			WriteReportHierarchyNodeBase(pivotHeading);
			WriteVisibility(pivotHeading.Visibility);
			WriteSubtotal(pivotHeading.Subtotal);
			m_writer.WriteInt32(pivotHeading.Level);
			m_writer.WriteBoolean(pivotHeading.IsColumn);
			m_writer.WriteBoolean(pivotHeading.HasExprHost);
			m_writer.WriteInt32(pivotHeading.SubtotalSpan);
			WriteIntList(pivotHeading.IDs);
		}

		private void WriteMatrixHeading(MatrixHeading matrixHeading)
		{
			if (matrixHeading == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.MatrixHeading;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WritePivotHeadingBase(matrixHeading);
			m_writer.WriteString(matrixHeading.Size);
			m_writer.WriteDouble(matrixHeading.SizeValue);
			WriteReportItemCollection(matrixHeading.ReportItems);
			m_writer.WriteBoolean(matrixHeading.OwcGroupExpression);
			m_writer.EndObject();
		}

		private void WriteMatrixHeadingReference(MatrixHeading matrixHeading)
		{
			if (matrixHeading == null)
			{
				m_writer.WriteNull();
			}
			else
			{
				m_writer.WriteReference(ObjectType.MatrixHeading, matrixHeading.ID);
			}
		}

		private void WriteTablixHeadingBase(TablixHeading tablixHeading)
		{
			if (tablixHeading == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.TablixHeading;
			DeclareType(objectType);
			WriteReportHierarchyNodeBase(tablixHeading);
			m_writer.WriteBoolean(tablixHeading.Subtotal);
			m_writer.WriteBoolean(tablixHeading.IsColumn);
			m_writer.WriteInt32(tablixHeading.Level);
			m_writer.WriteBoolean(tablixHeading.HasExprHost);
			m_writer.WriteInt32(tablixHeading.HeadingSpan);
		}

		private void WriteCustomReportItemHeading(CustomReportItemHeading heading)
		{
			if (heading == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.CustomReportItemHeading;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteTablixHeadingBase(heading);
			m_writer.WriteBoolean(heading.Static);
			WriteCustomReportItemHeadingList(heading.InnerHeadings);
			WriteDataValueList(heading.CustomProperties);
			m_writer.WriteInt32(heading.ExprHostID);
			WriteRunningValueInfoList(heading.RunningValues);
			m_writer.EndObject();
		}

		private void WriteCustomReportItemHeadingReference(CustomReportItemHeading heading)
		{
			if (heading == null)
			{
				m_writer.WriteNull();
			}
			else
			{
				m_writer.WriteReference(ObjectType.CustomReportItemHeading, heading.ID);
			}
		}

		private void WriteTableRow(TableRow tableRow)
		{
			if (tableRow == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.TableRow;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteIDOwnerBase(tableRow);
			WriteReportItemCollection(tableRow.ReportItems);
			WriteIntList(tableRow.IDs);
			WriteIntList(tableRow.ColSpans);
			m_writer.WriteString(tableRow.Height);
			m_writer.WriteDouble(tableRow.HeightValue);
			WriteVisibility(tableRow.Visibility);
			m_writer.EndObject();
		}

		private void WriteSubtotal(Subtotal subtotal)
		{
			if (subtotal == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.Subtotal;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteIDOwnerBase(subtotal);
			m_writer.WriteBoolean(subtotal.AutoDerived);
			WriteReportItemCollection(subtotal.ReportItems);
			WriteStyle(subtotal.StyleClass);
			WritePositionType(subtotal.Position);
			m_writer.WriteString(subtotal.DataElementName);
			WriteDataElementOutputType(subtotal.DataElementOutput);
			m_writer.EndObject();
		}

		private void WritePositionType(Subtotal.PositionType positionType)
		{
			m_writer.WriteEnum((int)positionType);
		}

		private void WriteList(List list)
		{
			if (list == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.List;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteDataRegionBase(list);
			WriteReportHierarchyNode(list.HierarchyDef);
			WriteReportItemCollection(list.ReportItems);
			m_writer.WriteBoolean(list.FillPage);
			m_writer.WriteString(list.DataInstanceName);
			WriteDataElementOutputType(list.DataInstanceElementOutput);
			m_writer.WriteBoolean(list.IsListMostInner);
			m_writer.EndObject();
		}

		private void WritePivotBase(Pivot pivot)
		{
			Assert(pivot != null);
			ObjectType objectType = ObjectType.Pivot;
			DeclareType(objectType);
			WriteDataRegionBase(pivot);
			m_writer.WriteInt32(pivot.ColumnCount);
			m_writer.WriteInt32(pivot.RowCount);
			WriteDataAggregateInfoList(pivot.CellAggregates);
			WriteProcessingInnerGrouping(pivot.ProcessingInnerGrouping);
			WriteRunningValueInfoList(pivot.RunningValues);
			WriteDataAggregateInfoList(pivot.CellPostSortAggregates);
			WriteDataElementOutputType(pivot.CellDataElementOutput);
		}

		private void WriteTablixBase(Tablix tablix)
		{
			Assert(tablix != null);
			ObjectType objectType = ObjectType.Tablix;
			DeclareType(objectType);
			WriteDataRegionBase(tablix);
			m_writer.WriteInt32(tablix.ColumnCount);
			m_writer.WriteInt32(tablix.RowCount);
			WriteDataAggregateInfoList(tablix.CellAggregates);
			WriteProcessingInnerGrouping(tablix.ProcessingInnerGrouping);
			WriteRunningValueInfoList(tablix.RunningValues);
			WriteDataAggregateInfoList(tablix.CellPostSortAggregates);
		}

		private void WriteCustomReportItem(CustomReportItem custom)
		{
			if (custom == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.CustomReportItem;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteTablixBase(custom);
			m_writer.WriteString(custom.Type);
			WriteReportItemCollection(custom.AltReportItem);
			WriteCustomReportItemHeadingList(custom.Columns);
			WriteCustomReportItemHeadingList(custom.Rows);
			WriteDataCellsList(custom.DataRowCells);
			WriteRunningValueInfoList(custom.CellRunningValues);
			WriteIntList(custom.CellExprHostIDs);
			WriteReportItemCollection(custom.RenderReportItem);
			m_writer.EndObject();
		}

		private void WriteChartDataPointList(ChartDataPointList datapoints)
		{
			if (datapoints == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ChartDataPointList;
			m_writer.StartObject(objectType);
			m_writer.StartArray(datapoints.Count);
			for (int i = 0; i < datapoints.Count; i++)
			{
				WriteChartDataPoint(datapoints[i]);
			}
			m_writer.EndArray();
			m_writer.EndObject();
		}

		private void WriteChartDataPoint(ChartDataPoint datapoint)
		{
			if (datapoint == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ChartDataPoint;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteExpressionInfoList(datapoint.DataValues);
			WriteChartDataLabel(datapoint.DataLabel);
			WriteAction(datapoint.Action);
			WriteStyle(datapoint.StyleClass);
			m_writer.WriteEnum((int)datapoint.MarkerType);
			m_writer.WriteString(datapoint.MarkerSize);
			WriteStyle(datapoint.MarkerStyleClass);
			m_writer.WriteString(datapoint.DataElementName);
			WriteDataElementOutputType(datapoint.DataElementOutput);
			m_writer.WriteInt32(datapoint.ExprHostID);
			WriteDataValueList(datapoint.CustomProperties);
			m_writer.EndObject();
		}

		private void WriteChartDataLabel(ChartDataLabel label)
		{
			if (label == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ChartDataLabel;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			m_writer.WriteBoolean(label.Visible);
			WriteExpressionInfo(label.Value);
			WriteStyle(label.StyleClass);
			m_writer.WriteEnum((int)label.Position);
			m_writer.WriteInt32(label.Rotation);
			m_writer.EndObject();
		}

		private void WriteMultiChart(MultiChart multiChart)
		{
			if (multiChart == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.MultiChart;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteReportHierarchyNodeBase(multiChart);
			m_writer.WriteEnum((int)multiChart.Layout);
			m_writer.WriteInt32(multiChart.MaxCount);
			m_writer.WriteBoolean(multiChart.SyncScale);
			m_writer.EndObject();
		}

		private void WriteLegend(Legend legend)
		{
			if (legend == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.Legend;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			m_writer.WriteBoolean(legend.Visible);
			WriteStyle(legend.StyleClass);
			m_writer.WriteEnum((int)legend.Position);
			m_writer.WriteEnum((int)legend.Layout);
			m_writer.WriteBoolean(legend.InsidePlotArea);
			m_writer.EndObject();
		}

		private void WriteChartHeading(ChartHeading chartHeading)
		{
			if (chartHeading == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ChartHeading;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WritePivotHeadingBase(chartHeading);
			WriteExpressionInfoList(chartHeading.Labels);
			WriteRunningValueInfoList(chartHeading.RunningValues);
			m_writer.WriteBoolean(chartHeading.ChartGroupExpression);
			WriteBoolList(chartHeading.PlotTypesLine);
			m_writer.EndObject();
		}

		private void WriteChartHeadingReference(ChartHeading chartHeading)
		{
			if (chartHeading == null)
			{
				m_writer.WriteNull();
			}
			else
			{
				m_writer.WriteReference(ObjectType.ChartHeading, chartHeading.ID);
			}
		}

		private void WriteAxis(Axis axis)
		{
			if (axis == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.Axis;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			m_writer.WriteBoolean(axis.Visible);
			WriteStyle(axis.StyleClass);
			WriteChartTitle(axis.Title);
			m_writer.WriteBoolean(axis.Margin);
			m_writer.WriteEnum((int)axis.MajorTickMarks);
			m_writer.WriteEnum((int)axis.MinorTickMarks);
			WriteGridLines(axis.MajorGridLines);
			WriteGridLines(axis.MinorGridLines);
			WriteExpressionInfo(axis.MajorInterval);
			WriteExpressionInfo(axis.MinorInterval);
			m_writer.WriteBoolean(axis.Reverse);
			WriteExpressionInfo(axis.CrossAt);
			m_writer.WriteBoolean(axis.AutoCrossAt);
			m_writer.WriteBoolean(axis.Interlaced);
			m_writer.WriteBoolean(axis.Scalar);
			WriteExpressionInfo(axis.Min);
			WriteExpressionInfo(axis.Max);
			m_writer.WriteBoolean(axis.AutoScaleMin);
			m_writer.WriteBoolean(axis.AutoScaleMax);
			m_writer.WriteBoolean(axis.LogScale);
			WriteDataValueList(axis.CustomProperties);
			m_writer.EndObject();
		}

		private void WriteGridLines(GridLines gridLines)
		{
			if (gridLines == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.GridLines;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			m_writer.WriteBoolean(gridLines.ShowGridLines);
			WriteStyle(gridLines.StyleClass);
			m_writer.EndObject();
		}

		private void WriteChartTitle(ChartTitle chartTitle)
		{
			if (chartTitle == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ChartTitle;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteExpressionInfo(chartTitle.Caption);
			WriteStyle(chartTitle.StyleClass);
			m_writer.WriteEnum((int)chartTitle.Position);
			m_writer.EndObject();
		}

		private void WriteThreeDProperties(ThreeDProperties properties)
		{
			if (properties == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ThreeDProperties;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			m_writer.WriteBoolean(properties.Enabled);
			m_writer.WriteBoolean(properties.PerspectiveProjectionMode);
			m_writer.WriteInt32(properties.Rotation);
			m_writer.WriteInt32(properties.Inclination);
			m_writer.WriteInt32(properties.Perspective);
			m_writer.WriteInt32(properties.HeightRatio);
			m_writer.WriteInt32(properties.DepthRatio);
			m_writer.WriteEnum((int)properties.Shading);
			m_writer.WriteInt32(properties.GapDepth);
			m_writer.WriteInt32(properties.WallThickness);
			m_writer.WriteBoolean(properties.DrawingStyleCube);
			m_writer.WriteBoolean(properties.Clustered);
			m_writer.EndObject();
		}

		private void WritePlotArea(PlotArea plotArea)
		{
			if (plotArea == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.PlotArea;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			m_writer.WriteEnum((int)plotArea.Origin);
			WriteStyle(plotArea.StyleClass);
			m_writer.EndObject();
		}

		private void WriteChart(Chart chart)
		{
			if (chart == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.Chart;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WritePivotBase(chart);
			WriteChartHeading(chart.Columns);
			WriteChartHeading(chart.Rows);
			WriteChartDataPointList(chart.ChartDataPoints);
			WriteRunningValueInfoList(chart.CellRunningValues);
			WriteMultiChart(chart.MultiChart);
			WriteLegend(chart.Legend);
			WriteAxis(chart.CategoryAxis);
			WriteAxis(chart.ValueAxis);
			WriteChartHeadingReference(chart.StaticColumns);
			WriteChartHeadingReference(chart.StaticRows);
			m_writer.WriteEnum((int)chart.Type);
			m_writer.WriteEnum((int)chart.SubType);
			m_writer.WriteEnum((int)chart.Palette);
			WriteChartTitle(chart.Title);
			m_writer.WriteInt32(chart.PointWidth);
			WriteThreeDProperties(chart.ThreeDProperties);
			WritePlotArea(chart.PlotArea);
			m_writer.EndObject();
		}

		private void WriteMatrix(Matrix matrix)
		{
			if (matrix == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.Matrix;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WritePivotBase(matrix);
			WriteReportItemCollection(matrix.CornerReportItems);
			WriteMatrixHeading(matrix.Columns);
			WriteMatrixHeading(matrix.Rows);
			WriteReportItemCollection(matrix.CellReportItems);
			WriteIntList(matrix.CellIDs);
			m_writer.WriteBoolean(matrix.PropagatedPageBreakAtStart);
			m_writer.WriteBoolean(matrix.PropagatedPageBreakAtEnd);
			m_writer.WriteInt32(matrix.InnerRowLevelWithPageBreak);
			WriteMatrixRowList(matrix.MatrixRows);
			WriteMatrixColumnList(matrix.MatrixColumns);
			m_writer.WriteInt32(matrix.GroupsBeforeRowHeaders);
			m_writer.WriteBoolean(matrix.LayoutDirection);
			WriteMatrixHeadingReference(matrix.StaticColumns);
			WriteMatrixHeadingReference(matrix.StaticRows);
			m_writer.WriteBoolean(matrix.UseOWC);
			WriteStringList(matrix.OwcCellNames);
			m_writer.WriteString(matrix.CellDataElementName);
			m_writer.WriteBoolean(matrix.ColumnGroupingFixedHeader);
			m_writer.WriteBoolean(matrix.RowGroupingFixedHeader);
			m_writer.EndObject();
		}

		private void WriteProcessingInnerGrouping(Pivot.ProcessingInnerGroupings directions)
		{
			m_writer.WriteEnum((int)directions);
		}

		private void WriteMatrixRow(MatrixRow row)
		{
			if (row == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.MatrixRow;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			m_writer.WriteString(row.Height);
			m_writer.WriteDouble(row.HeightValue);
			m_writer.EndObject();
		}

		private void WriteMatrixColumn(MatrixColumn column)
		{
			if (column == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.MatrixColumn;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			m_writer.WriteString(column.Width);
			m_writer.WriteDouble(column.WidthValue);
			m_writer.EndObject();
		}

		private void WriteTable(Table table)
		{
			if (table == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.Table;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteDataRegionBase(table);
			WriteTableColumnList(table.TableColumns);
			WriteTableRowList(table.HeaderRows);
			m_writer.WriteBoolean(table.HeaderRepeatOnNewPage);
			WriteTableGroup(table.TableGroups);
			WriteTableDetail(table.TableDetail);
			WriteTableGroupReference(table.DetailGroup);
			WriteTableRowList(table.FooterRows);
			m_writer.WriteBoolean(table.FooterRepeatOnNewPage);
			m_writer.WriteBoolean(table.PropagatedPageBreakAtStart);
			m_writer.WriteBoolean(table.GroupBreakAtStart);
			m_writer.WriteBoolean(table.PropagatedPageBreakAtEnd);
			m_writer.WriteBoolean(table.GroupBreakAtEnd);
			m_writer.WriteBoolean(table.FillPage);
			m_writer.WriteBoolean(table.UseOWC);
			m_writer.WriteBoolean(table.OWCNonSharedStyles);
			WriteRunningValueInfoList(table.RunningValues);
			m_writer.WriteString(table.DetailDataElementName);
			m_writer.WriteString(table.DetailDataCollectionName);
			WriteDataElementOutputType(table.DetailDataElementOutput);
			m_writer.WriteBoolean(table.FixedHeader);
			m_writer.EndObject();
		}

		private void WriteTableColumn(TableColumn column)
		{
			if (column == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.TableColumn;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			m_writer.WriteString(column.Width);
			m_writer.WriteDouble(column.WidthValue);
			WriteVisibility(column.Visibility);
			m_writer.WriteBoolean(column.FixedHeader);
			m_writer.EndObject();
		}

		private void WriteOWCChart(OWCChart chart)
		{
			if (chart == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.OWCChart;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteDataRegionBase(chart);
			WriteChartColumnList(chart.ChartData);
			m_writer.WriteString(chart.ChartDefinition);
			WriteRunningValueInfoList(chart.DetailRunningValues);
			WriteRunningValueInfoList(chart.RunningValues);
			m_writer.EndObject();
		}

		private void WriteChartColumn(ChartColumn column)
		{
			if (column == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ChartColumn;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			m_writer.WriteString(column.Name);
			WriteExpressionInfo(column.Value);
			m_writer.EndObject();
		}

		private void WriteDataValue(DataValue value)
		{
			if (value == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.DataValue;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteExpressionInfo(value.Name);
			WriteExpressionInfo(value.Value);
			m_writer.WriteInt32(value.ExprHostID);
			m_writer.EndObject();
		}

		private void WriteParameterInfo(ParameterInfo parameter)
		{
			if (parameter == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ParameterInfo;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteParameterBase(parameter);
			m_writer.WriteBoolean(parameter.IsUserSupplied);
			WriteVariants(parameter.Values);
			m_writer.WriteBoolean(parameter.DynamicValidValues);
			m_writer.WriteBoolean(parameter.DynamicDefaultValue);
			WriteParameterInfoRefCollection(parameter.DependencyList);
			WriteValidValueList(parameter.ValidValues);
			WriteStrings(parameter.Labels);
			m_writer.EndObject();
		}

		private void WriteProcessingMessage(ProcessingMessage message)
		{
			if (message == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ProcessingMessage;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			m_writer.WriteEnum((int)message.Code);
			m_writer.WriteEnum((int)message.Severity);
			m_writer.WriteEnum((int)message.ObjectType);
			m_writer.WriteString(message.ObjectName);
			m_writer.WriteString(message.PropertyName);
			m_writer.WriteString(message.Message);
			WriteProcessingMessageList(message.ProcessingMessages);
			m_writer.WriteEnum((int)message.CommonCode);
			m_writer.EndObject();
		}

		private void WriteDataValueInstance(DataValueInstance instance)
		{
			if (instance == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.DataValueInstance;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			m_writer.WriteString(instance.Name);
			WriteVariant(instance.Value);
			m_writer.EndObject();
		}

		private void WriteDataType(DataType dataType)
		{
			m_writer.WriteEnum((int)dataType);
		}

		private void WriteBookmarkInformation(BookmarkInformation bookmark)
		{
			if (bookmark == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.BookmarkInformation;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			m_writer.WriteString(bookmark.Id);
			m_writer.WriteInt32(bookmark.Page);
			m_writer.EndObject();
		}

		private void WriteDrillthroughInformation(DrillthroughInformation drillthroughInfo)
		{
			if (drillthroughInfo == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.DrillthroughInformation;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			m_writer.WriteString(drillthroughInfo.ReportName);
			WriteDrillthroughParameters(drillthroughInfo.ReportParameters);
			WriteIntList(drillthroughInfo.DataSetTokenIDs);
			m_writer.EndObject();
		}

		private void WriteSenderInformation(SenderInformation sender)
		{
			if (sender == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.SenderInformation;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			m_writer.WriteBoolean(sender.StartHidden);
			WriteIntList(sender.ReceiverUniqueNames);
			m_writer.WriteInt32s(sender.ContainerUniqueNames);
			m_writer.EndObject();
		}

		private void WriteReceiverInformation(ReceiverInformation receiver)
		{
			if (receiver == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ReceiverInformation;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			m_writer.WriteBoolean(receiver.StartHidden);
			m_writer.WriteInt32(receiver.SenderUniqueName);
			m_writer.EndObject();
		}

		private void WriteInfoBaseBase(InfoBase infoBase)
		{
			Assert(infoBase != null);
			ObjectType objectType = ObjectType.InfoBase;
			DeclareType(objectType);
		}

		private void WriteSimpleOffsetInfo(OffsetInfo offsetInfo)
		{
			m_writer.WriteInt64(offsetInfo.Offset);
		}

		private void WriteActionInstance(ActionInstance actionInstance)
		{
			if (actionInstance == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ActionInstance;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteActionItemInstanceList(actionInstance.ActionItemsValues);
			WriteVariants(actionInstance.StyleAttributeValues);
			m_writer.WriteInt32(actionInstance.UniqueName);
			m_writer.EndObject();
		}

		private void WriteActionItemInstanceList(ActionItemInstanceList actionItemInstanceList)
		{
			if (actionItemInstanceList == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ActionItemInstanceList;
			m_writer.StartObject(objectType);
			m_writer.StartArray(actionItemInstanceList.Count);
			for (int i = 0; i < actionItemInstanceList.Count; i++)
			{
				WriteActionItemInstance(actionItemInstanceList[i]);
			}
			m_writer.EndArray();
			m_writer.EndObject();
		}

		private void WriteActionItemInstance(ActionItemInstance actionItemInstance)
		{
			if (actionItemInstance == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ActionItemInstance;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			m_writer.WriteString(actionItemInstance.HyperLinkURL);
			m_writer.WriteString(actionItemInstance.BookmarkLink);
			m_writer.WriteString(actionItemInstance.DrillthroughReportName);
			WriteDrillthroughVariants(actionItemInstance.DrillthroughParametersValues, actionItemInstance.DataSetTokenIDs);
			WriteBoolList(actionItemInstance.DrillthroughParametersOmits);
			m_writer.WriteString(actionItemInstance.Label);
			m_writer.EndObject();
		}

		private void WriteDrillthroughVariants(object[] variants, IntList tokenIDs)
		{
			if (variants == null)
			{
				m_writer.WriteNull();
				return;
			}
			m_writer.StartArray(variants.Length);
			object obj = null;
			for (int i = 0; i < variants.Length; i++)
			{
				obj = null;
				if (tokenIDs == null || tokenIDs[i] < 0)
				{
					obj = variants[i];
				}
				if (obj is object[])
				{
					WriteVariants(obj as object[], isMultiValue: false);
				}
				else
				{
					WriteVariant(obj);
				}
			}
			m_writer.EndArray();
		}

		private void WriteReportInstanceInfo(ReportInstanceInfo instanceInfo)
		{
			if (instanceInfo == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ReportInstanceInfo;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteReportItemInstanceInfoBase(instanceInfo);
			WriteParameterInfoCollection(instanceInfo.Parameters);
			m_writer.WriteString(instanceInfo.ReportName);
			m_writer.WriteBoolean(instanceInfo.NoRows);
			m_writer.WriteInt32(instanceInfo.BodyUniqueName);
			m_writer.EndObject();
		}

		private void WriteReportItemColInstanceInfo(ReportItemColInstanceInfo instanceInfo)
		{
			if (instanceInfo == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ReportItemColInstanceInfo;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteInstanceInfoBase(instanceInfo);
			WriteNonComputedUniqueNamess(instanceInfo.ChildrenNonComputedUniqueNames);
			m_writer.EndObject();
		}

		private void WriteReportItemInstanceInfo(ReportItemInstanceInfo reportItemInstanceInfo)
		{
			if (reportItemInstanceInfo == null)
			{
				m_writer.WriteNull();
				return;
			}
			if (reportItemInstanceInfo is LineInstanceInfo)
			{
				WriteLineInstanceInfo((LineInstanceInfo)reportItemInstanceInfo);
				return;
			}
			if (reportItemInstanceInfo is RectangleInstanceInfo)
			{
				WriteRectangleInstanceInfo((RectangleInstanceInfo)reportItemInstanceInfo);
				return;
			}
			if (reportItemInstanceInfo is ImageInstanceInfo)
			{
				WriteImageInstanceInfo((ImageInstanceInfo)reportItemInstanceInfo);
				return;
			}
			if (reportItemInstanceInfo is CheckBoxInstanceInfo)
			{
				WriteCheckBoxInstanceInfo((CheckBoxInstanceInfo)reportItemInstanceInfo);
				return;
			}
			if (reportItemInstanceInfo is TextBoxInstanceInfo)
			{
				WriteTextBoxInstanceInfo((TextBoxInstanceInfo)reportItemInstanceInfo);
				return;
			}
			if (reportItemInstanceInfo is SubReportInstanceInfo)
			{
				WriteSubReportInstanceInfo((SubReportInstanceInfo)reportItemInstanceInfo);
				return;
			}
			if (reportItemInstanceInfo is ActiveXControlInstanceInfo)
			{
				WriteActiveXControlInstanceInfo((ActiveXControlInstanceInfo)reportItemInstanceInfo);
				return;
			}
			if (reportItemInstanceInfo is ListInstanceInfo)
			{
				WriteListInstanceInfo((ListInstanceInfo)reportItemInstanceInfo);
				return;
			}
			if (reportItemInstanceInfo is MatrixInstanceInfo)
			{
				WriteMatrixInstanceInfo((MatrixInstanceInfo)reportItemInstanceInfo);
				return;
			}
			if (reportItemInstanceInfo is TableInstanceInfo)
			{
				WriteTableInstanceInfo((TableInstanceInfo)reportItemInstanceInfo);
				return;
			}
			if (reportItemInstanceInfo is OWCChartInstanceInfo)
			{
				WriteOWCChartInstanceInfo((OWCChartInstanceInfo)reportItemInstanceInfo);
				return;
			}
			if (reportItemInstanceInfo is ChartInstanceInfo)
			{
				WriteChartInstanceInfo((ChartInstanceInfo)reportItemInstanceInfo);
				return;
			}
			if (reportItemInstanceInfo is CustomReportItemInstanceInfo)
			{
				WriteCustomReportItemInstanceInfo((CustomReportItemInstanceInfo)reportItemInstanceInfo);
				return;
			}
			if (reportItemInstanceInfo is PageSectionInstanceInfo)
			{
				WritePageSectionInstanceInfo((PageSectionInstanceInfo)reportItemInstanceInfo);
				return;
			}
			Assert(reportItemInstanceInfo is ReportInstanceInfo);
			WriteReportInstanceInfo((ReportInstanceInfo)reportItemInstanceInfo);
		}

		private void WriteLineInstanceInfo(LineInstanceInfo instanceInfo)
		{
			if (instanceInfo == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.LineInstanceInfo;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteReportItemInstanceInfoBase(instanceInfo);
			m_writer.EndObject();
		}

		private void WriteTextBoxInstanceInfo(TextBoxInstanceInfo instanceInfo)
		{
			if (instanceInfo == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.TextBoxInstanceInfo;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteReportItemInstanceInfoBase(instanceInfo);
			m_writer.WriteString(instanceInfo.FormattedValue);
			WriteVariant(instanceInfo.OriginalValue);
			TextBox textBox = (TextBox)instanceInfo.ReportItemDef;
			if (textBox.HideDuplicates != null)
			{
				m_writer.WriteBoolean(instanceInfo.Duplicate);
			}
			if (textBox.Action != null)
			{
				WriteActionInstance(instanceInfo.Action);
			}
			if (textBox.InitialToggleState != null)
			{
				m_writer.WriteBoolean(instanceInfo.InitialToggleState);
			}
			m_writer.EndObject();
		}

		private void WriteSimpleTextBoxInstanceInfo(SimpleTextBoxInstanceInfo simpleTextBoxInstanceInfo)
		{
			if (simpleTextBoxInstanceInfo == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.SimpleTextBoxInstanceInfo;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteInstanceInfoBase(simpleTextBoxInstanceInfo);
			m_writer.WriteString(simpleTextBoxInstanceInfo.FormattedValue);
			WriteVariant(simpleTextBoxInstanceInfo.OriginalValue);
			m_writer.EndObject();
		}

		private void WriteRectangleInstanceInfo(RectangleInstanceInfo instanceInfo)
		{
			if (instanceInfo == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.RectangleInstanceInfo;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteReportItemInstanceInfoBase(instanceInfo);
			m_writer.EndObject();
		}

		private void WriteCheckBoxInstanceInfo(CheckBoxInstanceInfo instanceInfo)
		{
			if (instanceInfo == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.CheckBoxInstanceInfo;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteReportItemInstanceInfoBase(instanceInfo);
			m_writer.WriteBoolean(instanceInfo.Value);
			m_writer.WriteBoolean(instanceInfo.Duplicate);
			m_writer.EndObject();
		}

		private void WriteImageInstanceInfo(ImageInstanceInfo instanceInfo)
		{
			if (instanceInfo == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ImageInstanceInfo;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteReportItemInstanceInfoBase(instanceInfo);
			m_writer.WriteString(instanceInfo.ImageValue);
			WriteActionInstance(instanceInfo.Action);
			m_writer.WriteBoolean(instanceInfo.BrokenImage);
			WriteImageMapAreaInstanceList(instanceInfo.ImageMapAreas);
			m_writer.EndObject();
		}

		private void WriteSubReportInstanceInfo(SubReportInstanceInfo instanceInfo)
		{
			if (instanceInfo == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.SubReportInstanceInfo;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteReportItemInstanceInfoBase(instanceInfo);
			m_writer.WriteString(instanceInfo.NoRows);
			m_writer.EndObject();
		}

		private void WriteActiveXControlInstanceInfo(ActiveXControlInstanceInfo instanceInfo)
		{
			if (instanceInfo == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ActiveXControlInstanceInfo;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteReportItemInstanceInfoBase(instanceInfo);
			WriteVariants(instanceInfo.ParameterValues);
			m_writer.EndObject();
		}

		private void WriteListInstanceInfo(ListInstanceInfo instanceInfo)
		{
			if (instanceInfo == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ListInstanceInfo;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteReportItemInstanceInfoBase(instanceInfo);
			m_writer.WriteString(instanceInfo.NoRows);
			m_writer.EndObject();
		}

		private void WriteListContentInstanceInfo(ListContentInstanceInfo instanceInfo)
		{
			if (instanceInfo == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ListContentInstanceInfo;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteInstanceInfoBase(instanceInfo);
			m_writer.WriteBoolean(instanceInfo.StartHidden);
			m_writer.WriteString(instanceInfo.Label);
			WriteDataValueInstanceList(instanceInfo.CustomPropertyInstances);
			m_writer.EndObject();
		}

		private void WriteMatrixInstanceInfo(MatrixInstanceInfo instanceInfo)
		{
			if (instanceInfo == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.MatrixInstanceInfo;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteReportItemInstanceInfoBase(instanceInfo);
			WriteNonComputedUniqueNames(instanceInfo.CornerNonComputedNames);
			m_writer.WriteString(instanceInfo.NoRows);
			m_writer.EndObject();
		}

		private void WriteMatrixHeadingInstanceInfo(MatrixHeadingInstanceInfo instanceInfo)
		{
			if (instanceInfo == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.MatrixHeadingInstanceInfo;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteInstanceInfoBase(instanceInfo);
			WriteNonComputedUniqueNames(instanceInfo.ContentUniqueNames);
			m_writer.WriteBoolean(instanceInfo.StartHidden);
			m_writer.WriteInt32(instanceInfo.HeadingCellIndex);
			m_writer.WriteInt32(instanceInfo.HeadingSpan);
			WriteVariant(instanceInfo.GroupExpressionValue);
			m_writer.WriteString(instanceInfo.Label);
			WriteDataValueInstanceList(instanceInfo.CustomPropertyInstances);
			m_writer.EndObject();
		}

		private void WriteMatrixSubtotalHeadingInstanceInfo(MatrixSubtotalHeadingInstanceInfo instanceInfo)
		{
			if (instanceInfo == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.MatrixSubtotalHeadingInstanceInfo;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteInstanceInfoBase(instanceInfo);
			WriteMatrixHeadingInstanceInfo(instanceInfo);
			WriteVariants(instanceInfo.StyleAttributeValues);
			m_writer.EndObject();
		}

		private void WriteMatrixCellInstanceInfo(MatrixCellInstanceInfo instanceInfo)
		{
			if (instanceInfo == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.MatrixCellInstanceInfo;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteInstanceInfoBase(instanceInfo);
			WriteNonComputedUniqueNames(instanceInfo.ContentUniqueNames);
			m_writer.WriteInt32(instanceInfo.RowIndex);
			m_writer.WriteInt32(instanceInfo.ColumnIndex);
			m_writer.EndObject();
		}

		private void WriteChartInstanceInfo(ChartInstanceInfo instanceInfo)
		{
			if (instanceInfo == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ChartInstanceInfo;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteReportItemInstanceInfoBase(instanceInfo);
			WriteAxisInstance(instanceInfo.CategoryAxis);
			WriteAxisInstance(instanceInfo.ValueAxis);
			WriteChartTitleInstance(instanceInfo.Title);
			WriteVariants(instanceInfo.PlotAreaStyleAttributeValues);
			WriteVariants(instanceInfo.LegendStyleAttributeValues);
			m_writer.WriteString(instanceInfo.CultureName);
			m_writer.WriteString(instanceInfo.NoRows);
			m_writer.EndObject();
		}

		private void WriteChartHeadingInstanceInfo(ChartHeadingInstanceInfo instanceInfo)
		{
			if (instanceInfo == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ChartHeadingInstanceInfo;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteInstanceInfoBase(instanceInfo);
			WriteVariant(instanceInfo.HeadingLabel);
			m_writer.WriteInt32(instanceInfo.HeadingCellIndex);
			m_writer.WriteInt32(instanceInfo.HeadingSpan);
			WriteVariant(instanceInfo.GroupExpressionValue);
			m_writer.WriteInt32(instanceInfo.StaticGroupingIndex);
			WriteDataValueInstanceList(instanceInfo.CustomPropertyInstances);
			m_writer.EndObject();
		}

		private void WriteChartDataPointInstanceInfo(ChartDataPointInstanceInfo instanceInfo)
		{
			if (instanceInfo == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ChartDataPointInstanceInfo;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteInstanceInfoBase(instanceInfo);
			m_writer.WriteInt32(instanceInfo.DataPointIndex);
			WriteVariants(instanceInfo.DataValues);
			m_writer.WriteString(instanceInfo.DataLabelValue);
			WriteVariants(instanceInfo.DataLabelStyleAttributeValues);
			WriteActionInstance(instanceInfo.Action);
			WriteVariants(instanceInfo.StyleAttributeValues);
			WriteVariants(instanceInfo.MarkerStyleAttributeValues);
			WriteDataValueInstanceList(instanceInfo.CustomPropertyInstances);
			m_writer.EndObject();
		}

		private void WriteTableInstanceInfo(TableInstanceInfo instanceInfo)
		{
			if (instanceInfo == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.TableInstanceInfo;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteReportItemInstanceInfoBase(instanceInfo);
			WriteTableColumnInstances(instanceInfo.ColumnInstances);
			m_writer.WriteString(instanceInfo.NoRows);
			m_writer.EndObject();
		}

		private void WriteTableGroupInstanceInfo(TableGroupInstanceInfo instanceInfo)
		{
			if (instanceInfo == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.TableGroupInstanceInfo;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteInstanceInfoBase(instanceInfo);
			m_writer.WriteBoolean(instanceInfo.StartHidden);
			m_writer.WriteString(instanceInfo.Label);
			WriteDataValueInstanceList(instanceInfo.CustomPropertyInstances);
			m_writer.EndObject();
		}

		private void WriteTableDetailInstanceInfo(TableDetailInstanceInfo instanceInfo)
		{
			if (instanceInfo == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.TableDetailInstanceInfo;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteInstanceInfoBase(instanceInfo);
			m_writer.WriteBoolean(instanceInfo.StartHidden);
			m_writer.EndObject();
		}

		private void WriteTableRowInstanceInfo(TableRowInstanceInfo instanceInfo)
		{
			if (instanceInfo == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.TableRowInstanceInfo;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteInstanceInfoBase(instanceInfo);
			m_writer.WriteBoolean(instanceInfo.StartHidden);
			m_writer.EndObject();
		}

		private void WriteOWCChartInstanceInfo(OWCChartInstanceInfo instanceInfo)
		{
			if (instanceInfo == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.OWCChartInstanceInfo;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteReportItemInstanceInfoBase(instanceInfo);
			WriteVariantLists(instanceInfo.ChartData, convertDBNull: false);
			m_writer.WriteString(instanceInfo.NoRows);
			m_writer.EndObject();
		}

		private void WriteCustomReportItemInstanceInfo(CustomReportItemInstanceInfo instanceInfo)
		{
			if (instanceInfo == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.CustomReportItemInstanceInfo;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteReportItemInstanceInfoBase(instanceInfo);
			m_writer.EndObject();
		}

		private void WritePageSectionInstanceInfo(PageSectionInstanceInfo instanceInfo)
		{
			if (instanceInfo == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.PageSectionInstanceInfo;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteReportItemInstanceInfoBase(instanceInfo);
			m_writer.EndObject();
		}

		private void WriteInstanceInfoBase(InstanceInfo instanceInfo)
		{
			Assert(instanceInfo != null);
			ObjectType objectType = ObjectType.InstanceInfo;
			DeclareType(objectType);
			WriteInfoBaseBase(instanceInfo);
		}

		private void WriteReportItemInstanceInfoBase(ReportItemInstanceInfo instanceInfo)
		{
			Assert(instanceInfo != null);
			ObjectType objectType = ObjectType.ReportItemInstanceInfo;
			DeclareType(objectType);
			WriteInstanceInfoBase(instanceInfo);
			ReportItem reportItemDef = instanceInfo.ReportItemDef;
			if (reportItemDef.StyleClass != null && reportItemDef.StyleClass.ExpressionList != null)
			{
				WriteVariants(instanceInfo.StyleAttributeValues);
			}
			if (reportItemDef.Visibility != null)
			{
				m_writer.WriteBoolean(instanceInfo.StartHidden);
			}
			if (reportItemDef.Label != null)
			{
				m_writer.WriteString(instanceInfo.Label);
			}
			if (reportItemDef.Bookmark != null)
			{
				m_writer.WriteString(instanceInfo.Bookmark);
			}
			if (reportItemDef.ToolTip != null)
			{
				m_writer.WriteString(instanceInfo.ToolTip);
			}
			if (reportItemDef.CustomProperties != null)
			{
				WriteDataValueInstanceList(instanceInfo.CustomPropertyInstances);
			}
		}

		private void WriteNonComputedUniqueNames(NonComputedUniqueNames names)
		{
			if (names == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.NonComputedUniqueNames;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			m_writer.WriteInt32(names.UniqueName);
			WriteNonComputedUniqueNamess(names.ChildrenUniqueNames);
			m_writer.EndObject();
		}

		private void WriteInstanceInfoOwnerBase(InstanceInfoOwner owner)
		{
			Assert(owner != null);
			ObjectType objectType = ObjectType.InstanceInfoOwner;
			DeclareType(objectType);
			WriteSimpleOffsetInfo(owner.OffsetInfo);
		}

		private void WriteReportItemInstance(ReportItemInstance reportItemInstance)
		{
			if (reportItemInstance == null)
			{
				m_writer.WriteNull();
				return;
			}
			if (reportItemInstance is LineInstance)
			{
				WriteLineInstance((LineInstance)reportItemInstance);
				return;
			}
			if (reportItemInstance is RectangleInstance)
			{
				WriteRectangleInstance((RectangleInstance)reportItemInstance);
				return;
			}
			if (reportItemInstance is ImageInstance)
			{
				WriteImageInstance((ImageInstance)reportItemInstance);
				return;
			}
			if (reportItemInstance is CheckBoxInstance)
			{
				WriteCheckBoxInstance((CheckBoxInstance)reportItemInstance);
				return;
			}
			if (reportItemInstance is TextBoxInstance)
			{
				WriteTextBoxInstance((TextBoxInstance)reportItemInstance);
				return;
			}
			if (reportItemInstance is SubReportInstance)
			{
				WriteSubReportInstance((SubReportInstance)reportItemInstance);
				return;
			}
			if (reportItemInstance is ActiveXControlInstance)
			{
				WriteActiveXControlInstance((ActiveXControlInstance)reportItemInstance);
				return;
			}
			if (reportItemInstance is ListInstance)
			{
				WriteListInstance((ListInstance)reportItemInstance);
				return;
			}
			if (reportItemInstance is MatrixInstance)
			{
				WriteMatrixInstance((MatrixInstance)reportItemInstance);
				return;
			}
			if (reportItemInstance is TableInstance)
			{
				WriteTableInstance((TableInstance)reportItemInstance);
				return;
			}
			if (reportItemInstance is ChartInstance)
			{
				WriteChartInstance((ChartInstance)reportItemInstance);
				return;
			}
			if (reportItemInstance is CustomReportItemInstance)
			{
				WriteCustomReportItemInstance((CustomReportItemInstance)reportItemInstance);
				return;
			}
			Assert(reportItemInstance is OWCChartInstance);
			WriteOWCChartInstance((OWCChartInstance)reportItemInstance);
		}

		private void WriteReportItemInstanceBase(ReportItemInstance reportItemInstance)
		{
			Assert(reportItemInstance != null);
			ObjectType objectType = ObjectType.ReportItemInstance;
			DeclareType(objectType);
			WriteInstanceInfoOwnerBase(reportItemInstance);
			if (m_writeUniqueName)
			{
				m_writer.WriteInt32(reportItemInstance.UniqueName);
			}
		}

		private void WriteReportItemInstanceReference(ReportItemInstance reportItemInstance)
		{
			if (reportItemInstance == null)
			{
				m_writer.WriteNull();
				return;
			}
			Assert(reportItemInstance is OWCChartInstance || reportItemInstance is ChartInstance);
			ObjectType objectType = (reportItemInstance is OWCChartInstance) ? ObjectType.OWCChartInstance : ObjectType.ChartInstance;
			m_writer.WriteReference(objectType, reportItemInstance.UniqueName);
		}

		private void WriteReportInstance(ReportInstance reportInstance)
		{
			if (reportInstance == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ReportInstance;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteReportItemInstanceBase(reportInstance);
			WriteReportItemColInstance(reportInstance.ReportItemColInstance);
			m_writer.WriteString(reportInstance.Language);
			m_writer.WriteInt32(reportInstance.NumberOfPages);
			m_writer.EndObject();
		}

		private void WriteReportItemColInstance(ReportItemColInstance reportItemColInstance)
		{
			if (reportItemColInstance == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ReportItemColInstance;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteInstanceInfoOwnerBase(reportItemColInstance);
			WriteReportItemInstanceList(reportItemColInstance.ReportItemInstances);
			WriteRenderingPagesRangesList(reportItemColInstance.ChildrenStartAndEndPages);
			m_writer.EndObject();
		}

		private void WriteLineInstance(LineInstance lineInstance)
		{
			if (lineInstance == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.LineInstance;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteReportItemInstanceBase(lineInstance);
			m_writer.EndObject();
		}

		private void WriteTextBoxInstance(TextBoxInstance textBoxInstance)
		{
			if (textBoxInstance == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.TextBoxInstance;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteReportItemInstanceBase(textBoxInstance);
			m_writer.EndObject();
		}

		private void WriteRectangleInstance(RectangleInstance rectangleInstance)
		{
			if (rectangleInstance == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.RectangleInstance;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteReportItemInstanceBase(rectangleInstance);
			WriteReportItemColInstance(rectangleInstance.ReportItemColInstance);
			m_writer.EndObject();
		}

		private void WriteCheckBoxInstance(CheckBoxInstance checkBoxInstance)
		{
			if (checkBoxInstance == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.CheckBoxInstance;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteReportItemInstanceBase(checkBoxInstance);
			m_writer.EndObject();
		}

		private void WriteImageInstance(ImageInstance imageInstance)
		{
			if (imageInstance == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ImageInstance;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteReportItemInstanceBase(imageInstance);
			m_writer.EndObject();
		}

		private void WriteSubReportInstance(SubReportInstance subReportInstance)
		{
			if (subReportInstance == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.SubReportInstance;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteReportItemInstanceBase(subReportInstance);
			WriteReportInstance(subReportInstance.ReportInstance);
			m_writer.EndObject();
		}

		private void WriteActiveXControlInstance(ActiveXControlInstance activeXControlInstance)
		{
			if (activeXControlInstance == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ActiveXControlInstance;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteReportItemInstanceBase(activeXControlInstance);
			m_writer.EndObject();
		}

		private void WriteListInstance(ListInstance listInstance)
		{
			if (listInstance == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ListInstance;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteReportItemInstanceBase(listInstance);
			WriteListContentInstanceList(listInstance.ListContents);
			WriteRenderingPagesRangesList(listInstance.ChildrenStartAndEndPages);
			m_writer.EndObject();
		}

		private void WriteListContentInstance(ListContentInstance listContentInstance)
		{
			if (listContentInstance == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ListContentInstance;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteInstanceInfoOwnerBase(listContentInstance);
			m_writer.WriteInt32(listContentInstance.UniqueName);
			WriteReportItemColInstance(listContentInstance.ReportItemColInstance);
			m_writer.EndObject();
		}

		private void WriteMatrixInstance(MatrixInstance matrixInstance)
		{
			if (matrixInstance == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.MatrixInstance;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteReportItemInstanceBase(matrixInstance);
			WriteReportItemInstance(matrixInstance.CornerContent);
			WriteMatrixHeadingInstanceList(matrixInstance.ColumnInstances);
			WriteMatrixHeadingInstanceList(matrixInstance.RowInstances);
			WriteMatrixCellInstancesList(matrixInstance.Cells);
			m_writer.WriteInt32(matrixInstance.InstanceCountOfInnerRowWithPageBreak);
			WriteRenderingPagesRangesList(matrixInstance.ChildrenStartAndEndPages);
			m_writer.EndObject();
		}

		private void WriteMatrixHeadingInstance(MatrixHeadingInstance matrixHeadingInstance)
		{
			if (matrixHeadingInstance == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.MatrixHeadingInstance;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteInstanceInfoOwnerBase(matrixHeadingInstance);
			m_writer.WriteInt32(matrixHeadingInstance.UniqueName);
			WriteReportItemInstance(matrixHeadingInstance.Content);
			WriteMatrixHeadingInstanceList(matrixHeadingInstance.SubHeadingInstances);
			m_writer.WriteBoolean(matrixHeadingInstance.IsSubtotal);
			WriteRenderingPagesRangesList(matrixHeadingInstance.ChildrenStartAndEndPages);
			m_writer.EndObject();
		}

		private void WriteMatrixCellInstance(MatrixCellInstance matrixCellInstance)
		{
			if (matrixCellInstance == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.MatrixCellInstance;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteInstanceInfoOwnerBase(matrixCellInstance);
			ReportItem reportItem = matrixCellInstance.Content?.ReportItemDef;
			WriteReportItemReference(reportItem);
			WriteReportItemInstance(matrixCellInstance.Content);
			m_writer.EndObject();
		}

		private void WriteMatrixSubtotalCellInstance(MatrixSubtotalCellInstance matrixSubtotalCellInstance)
		{
			Global.Tracer.Assert(matrixSubtotalCellInstance != null);
			ObjectType objectType = ObjectType.MatrixSubtotalCellInstance;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteInstanceInfoOwnerBase(matrixSubtotalCellInstance);
			Global.Tracer.Assert(matrixSubtotalCellInstance.SubtotalHeadingInstance != null, "(null != matrixSubtotalCellInstance.SubtotalHeadingInstance)");
			WriteMatrixCellInstance(matrixSubtotalCellInstance);
			m_writer.WriteInt32(matrixSubtotalCellInstance.SubtotalHeadingInstance.UniqueName);
			m_writer.EndObject();
		}

		private void WriteChartInstance(ChartInstance chartInstance)
		{
			if (chartInstance == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ChartInstance;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteReportItemInstanceBase(chartInstance);
			WriteMultiChartInstanceList(chartInstance.MultiCharts);
			m_writer.EndObject();
		}

		private void WriteMultiChartInstance(MultiChartInstance multiChartInstance)
		{
			if (multiChartInstance == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.MultiChartInstance;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteChartHeadingInstanceList(multiChartInstance.ColumnInstances);
			WriteChartHeadingInstanceList(multiChartInstance.RowInstances);
			WriteChartDataPointInstancesList(multiChartInstance.DataPoints);
			m_writer.EndObject();
		}

		private void WriteChartHeadingInstance(ChartHeadingInstance chartHeadingInstance)
		{
			if (chartHeadingInstance == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ChartHeadingInstance;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteInstanceInfoOwnerBase(chartHeadingInstance);
			m_writer.WriteInt32(chartHeadingInstance.UniqueName);
			WriteChartHeadingInstanceList(chartHeadingInstance.SubHeadingInstances);
			m_writer.EndObject();
		}

		private void WriteChartDataPointInstance(ChartDataPointInstance dataPointInstance)
		{
			if (dataPointInstance == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ChartDataPointInstance;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteInstanceInfoOwnerBase(dataPointInstance);
			m_writer.WriteInt32(dataPointInstance.UniqueName);
			m_writer.EndObject();
		}

		private void WriteAxisInstance(AxisInstance axisInstance)
		{
			if (axisInstance == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.AxisInstance;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			m_writer.WriteInt32(axisInstance.UniqueName);
			WriteChartTitleInstance(axisInstance.Title);
			WriteVariants(axisInstance.StyleAttributeValues);
			WriteVariants(axisInstance.MajorGridLinesStyleAttributeValues);
			WriteVariants(axisInstance.MinorGridLinesStyleAttributeValues);
			WriteVariant(axisInstance.MinValue);
			WriteVariant(axisInstance.MaxValue);
			WriteVariant(axisInstance.CrossAtValue);
			WriteVariant(axisInstance.MajorIntervalValue);
			WriteVariant(axisInstance.MinorIntervalValue);
			WriteDataValueInstanceList(axisInstance.CustomPropertyInstances);
			m_writer.EndObject();
		}

		private void WriteChartTitleInstance(ChartTitleInstance chartTitleInstance)
		{
			if (chartTitleInstance == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.ChartTitleInstance;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			m_writer.WriteInt32(chartTitleInstance.UniqueName);
			m_writer.WriteString(chartTitleInstance.Caption);
			WriteVariants(chartTitleInstance.StyleAttributeValues);
			m_writer.EndObject();
		}

		private void WriteTableInstance(TableInstance tableInstance)
		{
			if (tableInstance == null)
			{
				m_writer.WriteNull();
				return;
			}
			Table table = (Table)tableInstance.ReportItemDef;
			ObjectType objectType = ObjectType.TableInstance;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteReportItemInstanceBase(tableInstance);
			if (table.HeaderRows != null)
			{
				WriteTableRowInstances(tableInstance.HeaderRowInstances);
			}
			if (table.TableGroups != null)
			{
				WriteTableGroupInstanceList(tableInstance.TableGroupInstances);
			}
			else if (table.TableDetail != null)
			{
				if (table.TableDetail.SimpleDetailRows)
				{
					int int32Value = -1;
					if (tableInstance.TableDetailInstances != null && 0 < tableInstance.TableDetailInstances.Count)
					{
						int32Value = tableInstance.TableDetailInstances[0].UniqueName;
					}
					m_writer.WriteInt32(int32Value);
				}
				WriteTableDetailInstanceList(tableInstance.TableDetailInstances);
			}
			if (table.FooterRows != null)
			{
				WriteTableRowInstances(tableInstance.FooterRowInstances);
			}
			WriteRenderingPagesRangesList(tableInstance.ChildrenStartAndEndPages);
			m_writer.EndObject();
		}

		private void WriteTableGroupInstance(TableGroupInstance tableGroupInstance)
		{
			if (tableGroupInstance == null)
			{
				m_writer.WriteNull();
				return;
			}
			TableGroup tableGroupDef = tableGroupInstance.TableGroupDef;
			Table table = (Table)tableGroupDef.DataRegionDef;
			ObjectType objectType = ObjectType.TableGroupInstance;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteInstanceInfoOwnerBase(tableGroupInstance);
			m_writer.WriteInt32(tableGroupInstance.UniqueName);
			if (tableGroupDef.HeaderRows != null)
			{
				WriteTableRowInstances(tableGroupInstance.HeaderRowInstances);
			}
			if (tableGroupDef.FooterRows != null)
			{
				WriteTableRowInstances(tableGroupInstance.FooterRowInstances);
			}
			if (tableGroupDef.InnerHierarchy != null)
			{
				WriteTableGroupInstanceList(tableGroupInstance.SubGroupInstances);
			}
			else if (table.TableDetail != null)
			{
				if (table.TableDetail.SimpleDetailRows)
				{
					int int32Value = -1;
					if (tableGroupInstance.TableDetailInstances != null && 0 < tableGroupInstance.TableDetailInstances.Count)
					{
						int32Value = tableGroupInstance.TableDetailInstances[0].UniqueName;
					}
					m_writer.WriteInt32(int32Value);
				}
				WriteTableDetailInstanceList(tableGroupInstance.TableDetailInstances);
			}
			WriteRenderingPagesRangesList(tableGroupInstance.ChildrenStartAndEndPages);
			m_writer.EndObject();
		}

		private void WriteTableDetailInstance(TableDetailInstance tableDetailInstance)
		{
			if (tableDetailInstance == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.TableDetailInstance;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteInstanceInfoOwnerBase(tableDetailInstance);
			bool simpleDetailRows = tableDetailInstance.TableDetailDef.SimpleDetailRows;
			if (simpleDetailRows)
			{
				m_writeUniqueName = false;
			}
			else
			{
				m_writer.WriteInt32(tableDetailInstance.UniqueName);
			}
			WriteTableRowInstances(tableDetailInstance.DetailRowInstances);
			if (simpleDetailRows)
			{
				m_writeUniqueName = true;
			}
			m_writer.EndObject();
		}

		private void WriteTableRowInstance(TableRowInstance tableRowInstance)
		{
			if (tableRowInstance == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.TableRowInstance;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteInstanceInfoOwnerBase(tableRowInstance);
			if (m_writeUniqueName)
			{
				m_writer.WriteInt32(tableRowInstance.UniqueName);
			}
			WriteReportItemColInstance(tableRowInstance.TableRowReportItemColInstance);
			m_writer.EndObject();
		}

		private void WriteTableColumnInstance(TableColumnInstance tableColumnInstance)
		{
			if (tableColumnInstance == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.TableColumnInstance;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			m_writer.WriteInt32(tableColumnInstance.UniqueName);
			m_writer.WriteBoolean(tableColumnInstance.StartHidden);
			m_writer.EndObject();
		}

		private void WriteOWCChartInstance(OWCChartInstance chartInstance)
		{
			if (chartInstance == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.OWCChartInstance;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteReportItemInstanceBase(chartInstance);
			m_writer.EndObject();
		}

		private void WriteCustomReportItemInstance(CustomReportItemInstance instance)
		{
			if (instance == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.CustomReportItemInstance;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteReportItemInstanceBase(instance);
			WriteReportItemColInstance(instance.AltReportItemColInstance);
			WriteCustomReportItemHeadingInstanceList(instance.ColumnInstances);
			WriteCustomReportItemHeadingInstanceList(instance.RowInstances);
			WriteCustomReportItemCellInstancesList(instance.Cells);
			m_writer.EndObject();
		}

		private void WriteCustomReportItemHeadingInstance(CustomReportItemHeadingInstance headingInstance)
		{
			if (headingInstance == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.CustomReportItemHeadingInstance;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteCustomReportItemHeadingInstanceList(headingInstance.SubHeadingInstances);
			WriteCustomReportItemHeadingReference(headingInstance.HeadingDefinition);
			m_writer.WriteInt32(headingInstance.HeadingCellIndex);
			m_writer.WriteInt32(headingInstance.HeadingSpan);
			WriteDataValueInstanceList(headingInstance.CustomPropertyInstances);
			m_writer.WriteString(headingInstance.Label);
			WriteVariantList(headingInstance.GroupExpressionValues, convertDBNull: false);
			m_writer.EndObject();
		}

		private void WriteCustomReportItemCellInstance(CustomReportItemCellInstance cellInstance)
		{
			if (cellInstance == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.CustomReportItemCellInstance;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			m_writer.WriteInt32(cellInstance.RowIndex);
			m_writer.WriteInt32(cellInstance.ColumnIndex);
			WriteDataValueInstanceList(cellInstance.DataValueInstances);
			m_writer.EndObject();
		}

		private void WritePageSectionInstance(PageSectionInstance instance)
		{
			if (instance == null)
			{
				m_writer.WriteNull();
				return;
			}
			ObjectType objectType = ObjectType.PageSectionInstance;
			DeclareType(objectType);
			m_writer.StartObject(objectType);
			WriteReportItemInstanceBase(instance);
			m_writer.WriteInt32(instance.PageNumber);
			WriteReportItemColInstance(instance.ReportItemColInstance);
			m_writer.EndObject();
		}

		private void WriteVariant(object variant)
		{
			WriteVariant(variant, convertDBNull: false);
		}

		private void WriteVariant(object variant, bool convertDBNull)
		{
			if (variant == null)
			{
				Global.Tracer.Assert(!convertDBNull);
				m_writer.WriteNull();
			}
			else if (DBNull.Value == variant)
			{
				Global.Tracer.Assert(convertDBNull, "(convertDBNull)");
				m_writer.WriteNull();
			}
			else if (variant is string)
			{
				m_writer.WriteString((string)variant);
			}
			else if (variant is char)
			{
				m_writer.WriteChar((char)variant);
			}
			else if (variant is bool)
			{
				m_writer.WriteBoolean((bool)variant);
			}
			else if (variant is short)
			{
				m_writer.WriteInt16((short)variant);
			}
			else if (variant is int)
			{
				m_writer.WriteInt32((int)variant);
			}
			else if (variant is long)
			{
				m_writer.WriteInt64((long)variant);
			}
			else if (variant is ushort)
			{
				m_writer.WriteUInt16((ushort)variant);
			}
			else if (variant is uint)
			{
				m_writer.WriteUInt32((uint)variant);
			}
			else if (variant is ulong)
			{
				m_writer.WriteUInt64((ulong)variant);
			}
			else if (variant is byte)
			{
				m_writer.WriteByte((byte)variant);
			}
			else if (variant is sbyte)
			{
				m_writer.WriteSByte((sbyte)variant);
			}
			else if (variant is float)
			{
				m_writer.WriteSingle((float)variant);
			}
			else if (variant is double)
			{
				m_writer.WriteDouble((double)variant);
			}
			else if (variant is decimal)
			{
				m_writer.WriteDecimal((decimal)variant);
			}
			else if (variant is DateTime)
			{
				m_writer.WriteDateTime((DateTime)variant);
			}
			else
			{
				Assert(variant is TimeSpan);
				m_writer.WriteTimeSpan((TimeSpan)variant);
			}
		}

		private bool WriteRecordFields(RecordField[] recordFields, RecordSetPropertyNamesList aliasPropertyNames)
		{
			bool result = true;
			if (recordFields == null)
			{
				m_writer.WriteNull();
			}
			else
			{
				m_writer.StartArray(recordFields.Length);
				for (int i = 0; i < recordFields.Length; i++)
				{
					if (aliasPropertyNames != null && aliasPropertyNames[i] != null)
					{
						recordFields[i].PopulateFieldPropertyValues(aliasPropertyNames[i].PropertyNames);
					}
					if (!WriteRecordField(recordFields[i]))
					{
						result = false;
					}
				}
				m_writer.EndArray();
			}
			return result;
		}

		private bool WriteRecordField(RecordField recordField)
		{
			bool result = true;
			if (recordField == null)
			{
				m_writer.WriteNull();
			}
			else
			{
				ObjectType objectType = ObjectType.RecordField;
				DeclareType(objectType);
				m_writer.StartObject(objectType);
				if (recordField.IsOverflow)
				{
					m_writer.WriteDataFieldStatus(DataFieldStatus.Overflow);
				}
				else if (recordField.IsError)
				{
					m_writer.WriteDataFieldStatus(DataFieldStatus.IsError);
				}
				else if (recordField.IsUnSupportedDataType || !WriteFieldValue(recordField.FieldValue))
				{
					m_writer.WriteDataFieldStatus(DataFieldStatus.UnSupportedDataType);
					result = false;
				}
				m_writer.WriteBoolean(recordField.IsAggregationField);
				WriteVariantList(recordField.FieldPropertyValues, convertDBNull: false);
				m_writer.EndObject();
			}
			return result;
		}

		private bool WriteFieldValue(object variant)
		{
			if (variant == null)
			{
				m_writer.WriteNull();
			}
			else if (variant is string)
			{
				m_writer.WriteString((string)variant);
			}
			else if (variant is char)
			{
				m_writer.WriteChar((char)variant);
			}
			else if (variant is char[])
			{
				m_writer.WriteChars((char[])variant);
			}
			else if (variant is bool)
			{
				m_writer.WriteBoolean((bool)variant);
			}
			else if (variant is short)
			{
				m_writer.WriteInt16((short)variant);
			}
			else if (variant is int)
			{
				m_writer.WriteInt32((int)variant);
			}
			else if (variant is long)
			{
				m_writer.WriteInt64((long)variant);
			}
			else if (variant is ushort)
			{
				m_writer.WriteUInt16((ushort)variant);
			}
			else if (variant is uint)
			{
				m_writer.WriteUInt32((uint)variant);
			}
			else if (variant is ulong)
			{
				m_writer.WriteUInt64((ulong)variant);
			}
			else if (variant is byte)
			{
				m_writer.WriteByte((byte)variant);
			}
			else if (variant is byte[])
			{
				m_writer.WriteBytes((byte[])variant);
			}
			else if (variant is sbyte)
			{
				m_writer.WriteSByte((sbyte)variant);
			}
			else if (variant is float)
			{
				m_writer.WriteSingle((float)variant);
			}
			else if (variant is double)
			{
				m_writer.WriteDouble((double)variant);
			}
			else if (variant is decimal)
			{
				m_writer.WriteDecimal((decimal)variant);
			}
			else if (variant is DateTime)
			{
				m_writer.WriteDateTime((DateTime)variant);
			}
			else if (variant is TimeSpan)
			{
				m_writer.WriteTimeSpan((TimeSpan)variant);
			}
			else if (variant is Guid)
			{
				m_writer.WriteGuid((Guid)variant);
			}
			else
			{
				if (!(variant is DBNull))
				{
					return false;
				}
				m_writer.WriteNull();
			}
			return true;
		}
	}
}
