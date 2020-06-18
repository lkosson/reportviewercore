using Microsoft.ReportingServices.ReportProcessing.Persistence;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Collections;
using System.Globalization;
using System.IO;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal sealed class ChunkManager
	{
		internal enum SpecialChunkName
		{
			DocumentMap,
			ShowHideInfo,
			Bookmark,
			QuickFind,
			SortFilterEventInfo
		}

		internal sealed class InstanceInfoOwnerList : ArrayList
		{
			internal new InstanceInfoOwner this[int index] => (InstanceInfoOwner)base[index];
		}

		internal sealed class InstanceInfoList : ArrayList
		{
			internal new InstanceInfo this[int index] => (InstanceInfo)base[index];
		}

		internal sealed class DataChunkWriter
		{
			internal sealed class RecordRowList : ArrayList
			{
				internal new RecordRow this[int index] => (RecordRow)base[index];

				internal RecordRowList()
				{
				}

				internal RecordRowList(int capacity)
					: base(capacity)
				{
				}
			}

			private ReportProcessing.CreateReportChunk m_createChunkCallback;

			private IChunkFactory m_createChunkFactory;

			private string m_dataSetChunkName;

			private RecordSetInfo m_recordSetInfo;

			private bool m_recordSetPopulated;

			private RecordRowList m_recordRows;

			private Stream m_chunkStream;

			private IntermediateFormatWriter m_chunkWriter;

			private ReportProcessing.CreateReportChunk m_cacheDataCallback;

			private Stream m_cacheStream;

			private IntermediateFormatWriter m_cacheWriter;

			private bool m_stopSaveOnError;

			private bool m_errorOccurred;

			private Hashtable[] m_fieldAliasPropertyNames;

			internal Hashtable[] FieldAliasPropertyNames
			{
				set
				{
					m_fieldAliasPropertyNames = value;
				}
			}

			internal RecordSetInfo RecordSetInfo => m_recordSetInfo;

			internal bool RecordSetInfoPopulated
			{
				get
				{
					return m_recordSetPopulated;
				}
				set
				{
					m_recordSetPopulated = value;
				}
			}

			internal DataChunkWriter(DataSet dataSet, ReportProcessing.ProcessingContext context, bool readerExtensionsSupported, bool stopSaveOnError)
			{
				Global.Tracer.Assert(context.CreateReportChunkCallback != null, "(null != context.CreateReportChunkCallback)");
				m_dataSetChunkName = GenerateDataChunkName(dataSet, context, writeOperation: true);
				m_createChunkCallback = context.CreateReportChunkCallback;
				m_createChunkFactory = context.CreateReportChunkFactory;
				m_recordSetInfo = new RecordSetInfo(readerExtensionsSupported, dataSet.GetCLRCompareOptions());
				m_recordRows = new RecordRowList();
				m_cacheDataCallback = context.CacheDataCallback;
				m_stopSaveOnError = stopSaveOnError;
			}

			internal DataChunkWriter(DataSet dataSet, ReportProcessing.ProcessingContext context)
			{
				Global.Tracer.Assert(context.CreateReportChunkCallback != null, "(null != context.CreateReportChunkCallback)");
				m_dataSetChunkName = GenerateDataChunkName(dataSet, context, writeOperation: false);
				m_createChunkCallback = context.CreateReportChunkCallback;
				m_createChunkFactory = context.CreateReportChunkFactory;
			}

			internal bool AddRecordRow(FieldsImpl fields, int fieldCount)
			{
				return AddRecordRow(new RecordRow(fields, fieldCount));
			}

			internal bool AddRecordRow(RecordRow aRow)
			{
				CheckChunkLimit();
				if (!m_errorOccurred || !m_stopSaveOnError)
				{
					m_recordRows.Add(aRow);
				}
				return !m_errorOccurred;
			}

			internal bool FinalFlush()
			{
				Flush();
				if (!m_errorOccurred || !m_stopSaveOnError)
				{
					Close();
				}
				return !m_errorOccurred;
			}

			internal void Close()
			{
				if (m_chunkWriter != null)
				{
					m_chunkWriter = null;
				}
				if (m_chunkStream != null)
				{
					m_chunkStream.Close();
					m_chunkStream = null;
				}
				if (m_cacheWriter != null)
				{
					m_cacheWriter = null;
				}
				if (m_cacheStream != null)
				{
					m_cacheStream.Close();
					m_cacheStream = null;
				}
			}

			internal void CloseAndEraseChunk()
			{
				if (m_chunkWriter != null)
				{
					m_chunkWriter = null;
				}
				if (m_cacheWriter != null)
				{
					m_cacheWriter = null;
				}
				if (m_cacheStream != null)
				{
					m_cacheStream.Close();
					m_cacheStream = null;
				}
				if (m_createChunkFactory == null)
				{
					return;
				}
				try
				{
					if (m_chunkStream != null)
					{
						m_chunkStream.Close();
						m_chunkStream = null;
					}
					m_createChunkFactory.Erase(m_dataSetChunkName, ReportProcessing.ReportChunkTypes.Other);
				}
				catch
				{
				}
			}

			private void CheckChunkLimit()
			{
				if (m_recordRows.Count >= 4096)
				{
					Flush();
					if (!m_errorOccurred || !m_stopSaveOnError)
					{
						m_recordRows = new RecordRowList();
					}
				}
			}

			private void Flush()
			{
				if (m_recordRows == null || (m_createChunkCallback == null && m_cacheDataCallback == null))
				{
					return;
				}
				try
				{
					if (m_fieldAliasPropertyNames != null && !m_recordSetPopulated)
					{
						m_recordSetInfo.FieldPropertyNames = new RecordSetPropertyNamesList(m_fieldAliasPropertyNames.Length);
						for (int i = 0; i < m_fieldAliasPropertyNames.Length; i++)
						{
							RecordSetPropertyNames recordSetPropertyNames = null;
							if (m_fieldAliasPropertyNames[i] != null && m_fieldAliasPropertyNames[i].Count != 0)
							{
								recordSetPropertyNames = new RecordSetPropertyNames();
								recordSetPropertyNames.PropertyNames = new StringList(m_fieldAliasPropertyNames[i].Count);
								recordSetPropertyNames.PropertyNames.AddRange(m_fieldAliasPropertyNames[i].Values);
							}
							m_recordSetInfo.FieldPropertyNames.Add(recordSetPropertyNames);
						}
					}
					if (m_chunkStream == null && m_createChunkCallback != null)
					{
						m_chunkStream = m_createChunkCallback(m_dataSetChunkName, ReportProcessing.ReportChunkTypes.Other, null);
						m_chunkWriter = new IntermediateFormatWriter(m_chunkStream, writeDeclarations: true);
						m_chunkWriter.WriteRecordSetInfo(m_recordSetInfo);
					}
					if (m_cacheStream == null && m_cacheDataCallback != null)
					{
						m_cacheStream = m_cacheDataCallback(m_dataSetChunkName, ReportProcessing.ReportChunkTypes.Other, null);
						m_cacheWriter = new IntermediateFormatWriter(m_cacheStream, writeDeclarations: true);
						m_cacheWriter.WriteRecordSetInfo(m_recordSetInfo);
					}
					Global.Tracer.Assert(m_chunkWriter != null || m_cacheWriter != null, "(null != m_chunkWriter || null != m_cacheWriter)");
					for (int j = 0; j < m_recordRows.Count; j++)
					{
						if (m_chunkWriter != null && !m_chunkWriter.WriteRecordRow(m_recordRows[j], m_recordSetInfo.FieldPropertyNames))
						{
							m_errorOccurred = true;
						}
						if (m_errorOccurred && m_stopSaveOnError)
						{
							CloseAndEraseChunk();
							break;
						}
						if (m_cacheWriter != null)
						{
							m_cacheWriter.WriteRecordRow(m_recordRows[j], m_recordSetInfo.FieldPropertyNames);
						}
					}
					m_recordRows = null;
				}
				catch
				{
					m_chunkWriter = null;
					if (m_chunkStream != null)
					{
						m_chunkStream.Close();
						m_chunkStream = null;
					}
					m_cacheWriter = null;
					if (m_cacheStream != null)
					{
						m_cacheStream.Close();
						m_cacheStream = null;
					}
					throw;
				}
			}
		}

		internal sealed class DataChunkReader : IDisposable
		{
			private Stream m_chunkStream;

			private IntermediateFormatReader m_chunkReader;

			private RecordSetInfo m_recordSetInfo;

			private RecordRow m_recordRow;

			private int m_recordSetSize = -1;

			private int m_currentRow;

			private long m_streamLength = -1L;

			internal bool ReaderExtensionsSupported => m_recordSetInfo.ReaderExtensionsSupported;

			internal bool ReaderFieldProperties
			{
				get
				{
					if (m_recordSetInfo != null)
					{
						return m_recordSetInfo.FieldPropertyNames != null;
					}
					return false;
				}
			}

			internal bool ValidCompareOptions => m_recordSetInfo.ValidCompareOptions;

			internal CompareOptions CompareOptions => m_recordSetInfo.CompareOptions;

			public bool IsAggregateRow => m_recordRow.IsAggregateRow;

			public int AggregationFieldCount => m_recordRow.AggregationFieldCount;

			internal RecordSetPropertyNamesList FieldPropertyNames => m_recordSetInfo.FieldPropertyNames;

			internal DataChunkReader(DataSet dataSet, ReportProcessing.ProcessingContext context)
			{
				m_currentRow = -1;
				if (context.SubReportLevel == 0)
				{
					m_recordSetSize = dataSet.RecordSetSize;
				}
				Global.Tracer.Assert(context.GetReportChunkCallback != null, "(null != context.GetReportChunkCallback)");
				m_chunkStream = context.GetReportChunkCallback(GenerateDataChunkName(dataSet, context, writeOperation: false), ReportProcessing.ReportChunkTypes.Other, out string _);
				m_chunkReader = new IntermediateFormatReader(m_chunkStream);
				m_recordSetInfo = m_chunkReader.ReadRecordSetInfo();
				if (-1 == m_recordSetSize)
				{
					m_streamLength = m_chunkStream.Length;
				}
			}

			internal bool GetNextRow()
			{
				bool flag = false;
				if (-1 == m_recordSetSize)
				{
					if (m_chunkStream.Position < m_streamLength - 1)
					{
						flag = true;
					}
				}
				else if (m_currentRow < m_recordSetSize - 1)
				{
					flag = true;
				}
				if (flag)
				{
					m_currentRow++;
					ReadNextRow();
				}
				return flag;
			}

			internal object GetFieldValue(int aliasIndex)
			{
				if (m_recordRow.RecordFields[aliasIndex] == null)
				{
					throw new ReportProcessingException_FieldError(DataFieldStatus.IsMissing, null);
				}
				return m_recordRow.GetFieldValue(aliasIndex);
			}

			internal bool IsAggregationField(int aliasIndex)
			{
				return m_recordRow.IsAggregationField(aliasIndex);
			}

			internal object GetPropertyValue(int aliasIndex, int propertyIndex)
			{
				if (m_recordSetInfo.FieldPropertyNames != null && m_recordRow.RecordFields[aliasIndex] != null)
				{
					VariantList fieldPropertyValues = m_recordRow.RecordFields[aliasIndex].FieldPropertyValues;
					if (fieldPropertyValues != null && propertyIndex >= 0 && propertyIndex < fieldPropertyValues.Count)
					{
						return fieldPropertyValues[propertyIndex];
					}
				}
				return null;
			}

			void IDisposable.Dispose()
			{
				Dispose(disposing: true);
				GC.SuppressFinalize(this);
			}

			private void Dispose(bool disposing)
			{
				if (disposing)
				{
					if (m_chunkReader != null)
					{
						m_chunkReader = null;
					}
					if (m_chunkStream != null)
					{
						m_chunkStream.Close();
						m_chunkStream = null;
					}
				}
				m_recordRow = null;
				m_recordSetInfo = null;
			}

			private void ReadNextRow()
			{
				m_recordRow = m_chunkReader.ReadRecordRow();
			}
		}

		internal sealed class PageSectionManager
		{
			private InstanceInfoList m_pageSectionInstances;

			private InstanceInfoOwnerList m_pageSectionInstanceOwners;

			internal void AddPageSectionInstance(InstanceInfo instanceInfo, InstanceInfoOwner owner)
			{
				if (m_pageSectionInstances == null)
				{
					m_pageSectionInstances = new InstanceInfoList();
					m_pageSectionInstanceOwners = new InstanceInfoOwnerList();
				}
				m_pageSectionInstances.Add(instanceInfo);
				m_pageSectionInstanceOwners.Add(owner);
			}

			internal void Flush(ReportSnapshot reportSnapshot, ReportProcessing.CreateReportChunk createChunkCallback)
			{
				if (m_pageSectionInstances == null || createChunkCallback == null || reportSnapshot == null)
				{
					return;
				}
				Stream stream = null;
				IntermediateFormatWriter intermediateFormatWriter = null;
				try
				{
					stream = createChunkCallback("PageSectionInstances", ReportProcessing.ReportChunkTypes.Other, null);
					intermediateFormatWriter = new IntermediateFormatWriter(stream, writeDeclarations: false);
					for (int i = 0; i < m_pageSectionInstances.Count; i++)
					{
						long position = stream.Position;
						intermediateFormatWriter.WriteInstanceInfo(m_pageSectionInstances[i]);
						m_pageSectionInstanceOwners[i].SetOffset(position);
					}
					stream.Close();
					bool[] declarationsToWrite = intermediateFormatWriter.DeclarationsToWrite;
					stream = createChunkCallback("PageSections", ReportProcessing.ReportChunkTypes.Main, null);
					intermediateFormatWriter = new IntermediateFormatWriter(stream, declarationsToWrite, null);
					reportSnapshot.PageSectionOffsets = intermediateFormatWriter.WritePageSections(stream, reportSnapshot.PageSections);
					Global.Tracer.Assert(2 * reportSnapshot.PageSectionOffsets.Count == reportSnapshot.PageSections.Count);
					reportSnapshot.PageSections = null;
					intermediateFormatWriter = null;
					stream.Close();
					stream = null;
				}
				finally
				{
					m_pageSectionInstances = null;
					m_pageSectionInstanceOwners = null;
					intermediateFormatWriter = null;
					stream?.Close();
				}
			}
		}

		internal abstract class SnapshotChunkManager
		{
			protected ReportProcessing.CreateReportChunk m_createChunkCallback;

			protected InstanceInfoList m_firstPageChunkInstances;

			protected InstanceInfoOwnerList m_firstPageChunkInstanceOwners;

			protected InstanceInfoList m_chunkInstances;

			protected InstanceInfoOwnerList m_chunkInstanceOwners;

			protected IntermediateFormatWriter m_chunkWriter;

			protected Stream m_chunkStream;

			private bool[] m_firstPageDeclarationsToWrite;

			private bool[] m_otherPageDeclarationsToWrite;

			protected internal SnapshotChunkManager(ReportProcessing.CreateReportChunk createChunkCallback)
			{
				m_createChunkCallback = createChunkCallback;
				m_firstPageChunkInstances = new InstanceInfoList();
				m_firstPageChunkInstanceOwners = new InstanceInfoOwnerList();
			}

			protected void Flush()
			{
				if (m_chunkInstances == null || m_createChunkCallback == null)
				{
					return;
				}
				try
				{
					if (m_chunkStream == null)
					{
						m_chunkStream = m_createChunkCallback("OtherPages", ReportProcessing.ReportChunkTypes.Other, null);
					}
					if (m_chunkWriter == null)
					{
						m_chunkWriter = new IntermediateFormatWriter(m_chunkStream, writeDeclarations: false);
					}
					for (int i = 0; i < m_chunkInstances.Count; i++)
					{
						long position = m_chunkStream.Position;
						m_chunkWriter.WriteInstanceInfo(m_chunkInstances[i]);
						m_chunkInstanceOwners[i].SetOffset(position);
					}
					m_chunkInstances = null;
					m_chunkInstanceOwners = null;
				}
				catch
				{
					m_chunkWriter = null;
					if (m_chunkStream != null)
					{
						m_chunkStream.Close();
						m_chunkStream = null;
					}
					throw;
				}
			}

			internal void FinalFlush()
			{
				Flush();
				if (m_chunkWriter != null)
				{
					m_otherPageDeclarationsToWrite = m_chunkWriter.DeclarationsToWrite;
					m_chunkWriter = null;
				}
				if (m_chunkStream != null)
				{
					m_chunkStream.Close();
					m_chunkStream = null;
				}
			}

			internal void SaveFirstPage()
			{
				if (m_firstPageChunkInstances == null || m_firstPageChunkInstances.Count == 0 || m_createChunkCallback == null)
				{
					return;
				}
				Stream stream = null;
				try
				{
					stream = m_createChunkCallback("FirstPage", ReportProcessing.ReportChunkTypes.Main, null);
					IntermediateFormatWriter intermediateFormatWriter = new IntermediateFormatWriter(stream, writeDeclarations: false);
					for (int i = 0; i < m_firstPageChunkInstances.Count; i++)
					{
						long position = stream.Position;
						intermediateFormatWriter.WriteInstanceInfo(m_firstPageChunkInstances[i]);
						Global.Tracer.Assert(position != 0, "(0 != offset)");
						position = -position;
						m_firstPageChunkInstanceOwners[i].SetOffset(position);
					}
					m_firstPageChunkInstances = null;
					m_firstPageChunkInstanceOwners = null;
					m_firstPageDeclarationsToWrite = intermediateFormatWriter.DeclarationsToWrite;
					intermediateFormatWriter = null;
				}
				finally
				{
					stream?.Close();
				}
			}

			internal void SaveReportSnapshot(ReportSnapshot reportSnapshot)
			{
				if (m_createChunkCallback == null)
				{
					return;
				}
				Stream stream = null;
				try
				{
					if (reportSnapshot.HasDocumentMap)
					{
						stream = m_createChunkCallback("DocumentMap", ReportProcessing.ReportChunkTypes.Main, null);
						new IntermediateFormatWriter(stream, writeDeclarations: true).WriteDocumentMapNode(reportSnapshot.DocumentMap);
						reportSnapshot.DocumentMap = null;
						stream.Close();
						stream = null;
					}
					if (reportSnapshot.HasBookmarks)
					{
						stream = m_createChunkCallback("Bookmarks", ReportProcessing.ReportChunkTypes.Main, null);
						new IntermediateFormatWriter(stream, writeDeclarations: true).WriteBookmarksHashtable(reportSnapshot.BookmarksInfo);
						reportSnapshot.BookmarksInfo = null;
						stream.Close();
						stream = null;
					}
					if (reportSnapshot.DrillthroughInfo != null)
					{
						stream = m_createChunkCallback("Drillthrough", ReportProcessing.ReportChunkTypes.Main, null);
						new IntermediateFormatWriter(stream, writeDeclarations: true).WriteDrillthroughInfo(reportSnapshot.DrillthroughInfo);
						reportSnapshot.DrillthroughInfo = null;
						stream.Close();
						stream = null;
					}
					if (reportSnapshot.ShowHideSenderInfo != null || reportSnapshot.ShowHideReceiverInfo != null)
					{
						stream = m_createChunkCallback("ShowHideInfo", ReportProcessing.ReportChunkTypes.Main, null);
						IntermediateFormatWriter intermediateFormatWriter = new IntermediateFormatWriter(stream, writeDeclarations: true);
						intermediateFormatWriter.WriteSenderInformationHashtable(reportSnapshot.ShowHideSenderInfo);
						reportSnapshot.ShowHideSenderInfo = null;
						intermediateFormatWriter.WriteReceiverInformationHashtable(reportSnapshot.ShowHideReceiverInfo);
						reportSnapshot.ShowHideReceiverInfo = null;
						stream.Close();
						stream = null;
					}
					if (reportSnapshot.QuickFind != null)
					{
						stream = m_createChunkCallback("QuickFind", ReportProcessing.ReportChunkTypes.Main, null);
						new IntermediateFormatWriter(stream, writeDeclarations: true).WriteQuickFindHashtable(reportSnapshot.QuickFind);
						reportSnapshot.QuickFind = null;
						stream.Close();
						stream = null;
					}
					if (reportSnapshot.SortFilterEventInfo != null)
					{
						stream = m_createChunkCallback("SortFilterEventInfo", ReportProcessing.ReportChunkTypes.Main, null);
						new IntermediateFormatWriter(stream, writeDeclarations: true).WriteSortFilterEventInfoHashtable(reportSnapshot.SortFilterEventInfo);
						reportSnapshot.SortFilterEventInfo = null;
						stream.Close();
						stream = null;
					}
					stream = m_createChunkCallback("Main", ReportProcessing.ReportChunkTypes.Main, null);
					new IntermediateFormatWriter(stream, m_firstPageDeclarationsToWrite, m_otherPageDeclarationsToWrite).WriteReportSnapshot(reportSnapshot);
				}
				finally
				{
					stream?.Close();
				}
			}

			internal void Close()
			{
				m_chunkWriter = null;
				if (m_chunkStream != null)
				{
					m_chunkStream.Close();
					m_chunkStream = null;
				}
			}
		}

		internal sealed class UpgradeManager : SnapshotChunkManager
		{
			internal UpgradeManager(ReportProcessing.CreateReportChunk createChunkCallback)
				: base(createChunkCallback)
			{
			}

			internal void AddInstance(InstanceInfo instanceInfo, InstanceInfoOwner owner, long offset)
			{
				Global.Tracer.Assert(offset != 0, "(0 != offset)");
				if (offset < 0)
				{
					m_firstPageChunkInstances.Add(instanceInfo);
					m_firstPageChunkInstanceOwners.Add(owner);
					return;
				}
				if (m_chunkInstances == null)
				{
					m_chunkInstances = new InstanceInfoList();
					m_chunkInstanceOwners = new InstanceInfoOwnerList();
				}
				m_chunkInstances.Add(instanceInfo);
				m_chunkInstanceOwners.Add(owner);
			}
		}

		internal sealed class ProcessingChunkManager : SnapshotChunkManager
		{
			private bool m_inFirstPage = true;

			private bool m_hasLeafNode;

			private int m_ignorePageBreaks;

			private int m_ignoreInstances;

			private int m_reportItemCollectionLevel;

			private int m_instanceCount;

			private Hashtable m_repeatSiblingLists;

			private bool m_isOnePass;

			private PageSectionManager m_pageSectionManager;

			private long m_totalInstanceCount;

			internal bool InFirstPage => m_inFirstPage;

			internal long TotalCount => m_totalInstanceCount + m_instanceCount;

			internal ProcessingChunkManager(ReportProcessing.CreateReportChunk createChunkCallback, bool isOnePass)
				: base(createChunkCallback)
			{
				m_isOnePass = isOnePass;
				m_pageSectionManager = new PageSectionManager();
			}

			internal void PageSectionFlush(ReportSnapshot reportSnapshot)
			{
				m_pageSectionManager.Flush(reportSnapshot, m_createChunkCallback);
			}

			internal void EnterIgnorePageBreakItem()
			{
				if (!m_isOnePass)
				{
					m_ignorePageBreaks++;
				}
			}

			internal void LeaveIgnorePageBreakItem()
			{
				if (!m_isOnePass)
				{
					m_ignorePageBreaks--;
					Global.Tracer.Assert(0 <= m_ignorePageBreaks, "(0 <= m_ignorePageBreaks)");
				}
			}

			internal void EnterIgnoreInstances()
			{
				if (!m_isOnePass)
				{
					m_ignoreInstances++;
				}
			}

			internal void LeaveIgnoreInstances()
			{
				if (!m_isOnePass)
				{
					m_ignoreInstances--;
					Global.Tracer.Assert(0 <= m_ignoreInstances, "(0 <= m_ignoreInstances)");
				}
			}

			internal void EnterReportItemCollection()
			{
				if (!m_isOnePass)
				{
					m_reportItemCollectionLevel++;
				}
			}

			internal void LeaveReportItemCollection()
			{
				if (!m_isOnePass)
				{
					if (m_repeatSiblingLists != null)
					{
						m_repeatSiblingLists.Remove(m_reportItemCollectionLevel);
					}
					m_reportItemCollectionLevel--;
					Global.Tracer.Assert(0 <= m_reportItemCollectionLevel, "(0 <= m_reportItemCollectionLevel)");
				}
			}

			internal void AddRepeatSiblings(DataRegion dataRegion, int index)
			{
				if (m_isOnePass || !m_inFirstPage || dataRegion.RepeatSiblings == null)
				{
					return;
				}
				Hashtable hashtable = null;
				for (int i = 0; i < dataRegion.RepeatSiblings.Count; i++)
				{
					int num = dataRegion.RepeatSiblings[i];
					if (index > num)
					{
						continue;
					}
					if (hashtable == null)
					{
						if (m_repeatSiblingLists == null)
						{
							m_repeatSiblingLists = new Hashtable();
						}
						else
						{
							hashtable = (Hashtable)m_repeatSiblingLists[m_reportItemCollectionLevel];
						}
						if (hashtable == null)
						{
							hashtable = new Hashtable();
							m_repeatSiblingLists.Add(m_reportItemCollectionLevel, hashtable);
						}
					}
					hashtable.Add(num, true);
				}
			}

			internal void CheckPageBreak(IPageBreakItem item, bool atStart)
			{
				if (m_isOnePass || !m_inFirstPage || 0 < m_ignorePageBreaks || m_createChunkCallback == null)
				{
					return;
				}
				if (item.IgnorePageBreaks())
				{
					if (atStart)
					{
						EnterIgnorePageBreakItem();
						if (item is Rectangle && ((Rectangle)item).RepeatedSibling)
						{
							EnterIgnoreInstances();
						}
					}
					else
					{
						LeaveIgnorePageBreakItem();
						if (item is Rectangle && ((Rectangle)item).RepeatedSibling)
						{
							LeaveIgnoreInstances();
						}
					}
				}
				else if ((atStart && m_hasLeafNode && item.HasPageBreaks(atStart)) || (!atStart && item.HasPageBreaks(atStart)))
				{
					m_inFirstPage = false;
				}
			}

			internal void AddInstance(InstanceInfo newInstance, ReportItem reportItemDef, InstanceInfoOwner owner, int index, bool isPageSection)
			{
				if (isPageSection)
				{
					m_pageSectionManager.AddPageSectionInstance(newInstance, owner);
					return;
				}
				if (!m_isOnePass && reportItemDef.RepeatedSibling && !m_inFirstPage)
				{
					Hashtable hashtable = null;
					if (m_repeatSiblingLists != null)
					{
						hashtable = (Hashtable)m_repeatSiblingLists[m_reportItemCollectionLevel];
					}
					if (hashtable != null && hashtable[index] != null)
					{
						SyncAddInstanceToFirstPage(newInstance, owner);
						return;
					}
				}
				AddInstance(newInstance, owner, isPageSection);
			}

			internal void AddInstance(InstanceInfo newInstance, InstanceInfoOwner owner, bool isPageSection)
			{
				if (isPageSection)
				{
					m_pageSectionManager.AddPageSectionInstance(newInstance, owner);
				}
				else if (m_isOnePass)
				{
					lock (this)
					{
						SyncAddInstance(newInstance, owner);
					}
				}
				else
				{
					SyncAddInstance(newInstance, owner);
				}
			}

			internal void AddInstance(InstanceInfo newInstance, InstanceInfoOwner owner, bool addToFirstPage, bool isPageSection)
			{
				if (isPageSection)
				{
					m_pageSectionManager.AddPageSectionInstance(newInstance, owner);
				}
				else if (addToFirstPage)
				{
					AddInstanceToFirstPage(newInstance, owner, isPageSection: false);
				}
				else
				{
					AddInstance(newInstance, owner, isPageSection: false);
				}
			}

			internal void AddInstanceToFirstPage(InstanceInfo newInstance, InstanceInfoOwner owner, bool isPageSection)
			{
				if (isPageSection)
				{
					m_pageSectionManager.AddPageSectionInstance(newInstance, owner);
				}
				else if (m_isOnePass)
				{
					lock (this)
					{
						SyncAddInstanceToFirstPage(newInstance, owner);
					}
				}
				else
				{
					SyncAddInstanceToFirstPage(newInstance, owner);
				}
			}

			private void SyncAddInstanceToFirstPage(InstanceInfo newInstance, InstanceInfoOwner owner)
			{
				m_firstPageChunkInstances.Add(newInstance);
				m_firstPageChunkInstanceOwners.Add(owner);
			}

			private void SyncAddInstance(InstanceInfo newInstance, InstanceInfoOwner owner)
			{
				CheckChunkLimit();
				if (m_inFirstPage)
				{
					SetHasLeafNodes(owner);
					SyncAddInstanceToFirstPage(newInstance, owner);
				}
				else
				{
					m_chunkInstances.Add(newInstance);
					m_chunkInstanceOwners.Add(owner);
				}
				if (newInstance is OWCChartInstanceInfo)
				{
					m_instanceCount += ((OWCChartInstanceInfo)newInstance).Size;
				}
				else
				{
					m_instanceCount++;
				}
			}

			private void SetHasLeafNodes(InstanceInfoOwner owner)
			{
				if (!m_isOnePass && !m_hasLeafNode && 0 >= m_ignoreInstances && (owner is TextBoxInstance || owner is LineInstance || owner is CheckBoxInstance || owner is ImageInstance || owner is ActiveXControlInstance || owner is OWCChartInstance))
				{
					m_hasLeafNode = true;
				}
			}

			private void CheckChunkLimit()
			{
				if (m_createChunkCallback == null)
				{
					return;
				}
				if (m_inFirstPage)
				{
					if (m_instanceCount < 4096)
					{
						return;
					}
					m_inFirstPage = false;
				}
				bool flag = false;
				if (m_chunkInstances == null)
				{
					flag = true;
				}
				else if (m_instanceCount >= 4096)
				{
					Flush();
					flag = true;
				}
				if (flag)
				{
					m_totalInstanceCount += m_instanceCount;
					m_instanceCount = 0;
					m_chunkInstances = new InstanceInfoList();
					m_chunkInstanceOwners = new InstanceInfoOwnerList();
				}
			}
		}

		internal sealed class EventsChunkManager
		{
			private SpecialChunkManager m_specialChunkManager;

			internal EventsChunkManager(ReportProcessing.GetReportChunk getChunkCallback)
			{
				m_specialChunkManager = new SpecialChunkManager(getChunkCallback, null, null, null);
			}

			internal EventsChunkManager(ReportProcessing.GetReportChunk getChunkCallback, Hashtable definitionObjects, IntermediateFormatVersion intermediateFormatVersion)
			{
				m_specialChunkManager = new SpecialChunkManager(getChunkCallback, null, definitionObjects, intermediateFormatVersion);
			}

			internal void Close()
			{
				m_specialChunkManager.Close();
			}

			internal void GetShowHideInfo(out SenderInformationHashtable senderInfo, out ReceiverInformationHashtable receiverInfo)
			{
				senderInfo = null;
				receiverInfo = null;
				IntermediateFormatReader showHideInfoReader = m_specialChunkManager.ShowHideInfoReader;
				if (showHideInfoReader != null)
				{
					senderInfo = showHideInfoReader.ReadSenderInformationHashtable();
					receiverInfo = showHideInfoReader.ReadReceiverInformationHashtable();
				}
			}

			internal BookmarkInformation GetBookmarkIdInfo(string bookmarkId)
			{
				if (bookmarkId == null)
				{
					return null;
				}
				return m_specialChunkManager.BookmarkReader?.FindBookmarkIdInfo(bookmarkId);
			}

			internal DrillthroughInformation GetDrillthroughIdInfo(string drillthroughId)
			{
				if (drillthroughId == null)
				{
					return null;
				}
				return m_specialChunkManager.DrillthroughReader?.FindDrillthroughIdInfo(drillthroughId);
			}

			internal int GetDocumentMapNodePage(string documentMapId)
			{
				if (documentMapId == null)
				{
					return 0;
				}
				int page = 0;
				m_specialChunkManager.DocumentMapReader?.FindDocumentMapNodePage(documentMapId, ref page);
				return page;
			}

			internal DocumentMapNodeInfo GetDocumentMapInfo()
			{
				return m_specialChunkManager.DocumentMapReader?.ReadDocumentMapNodeInfo();
			}

			internal DocumentMapNode GetDocumentMapNode()
			{
				return m_specialChunkManager.DocumentMapReader?.ReadDocumentMapNode();
			}

			internal SortFilterEventInfoHashtable GetSortFilterEventInfo()
			{
				return m_specialChunkManager.SortFilterEventInfoReader?.ReadSortFilterEventInfoHashtable();
			}
		}

		internal sealed class SpecialChunkManager
		{
			private ReportProcessing.GetReportChunk m_getChunkCallback;

			private Hashtable m_definitionObjects;

			private Hashtable m_instanceObjects;

			private IntermediateFormatVersion m_intermediateFormatVersion;

			private Stream m_docMap;

			private bool m_hasDocMap = true;

			private Stream m_showHideInfo;

			private bool m_hasShowHideInfo = true;

			private Stream m_bookmarks;

			private bool m_hasBookmarks = true;

			private Stream m_drillthrough;

			private bool m_hasDrillthrough = true;

			private Stream m_quickFind;

			private bool m_hasQuickFind = true;

			private Stream m_sortFilterEventInfo;

			private bool m_hasSortFilterEventInfo = true;

			internal IntermediateFormatReader DocumentMapReader
			{
				get
				{
					IntermediateFormatReader result = null;
					if (m_hasDocMap && m_docMap == null)
					{
						m_docMap = GetSpecialChunkInfo("DocumentMap", ref m_hasDocMap);
					}
					if (m_docMap != null)
					{
						m_docMap.Position = 0L;
						result = new IntermediateFormatReader(m_docMap, m_instanceObjects);
					}
					return result;
				}
			}

			internal IntermediateFormatReader ShowHideInfoReader
			{
				get
				{
					IntermediateFormatReader result = null;
					if (m_hasShowHideInfo && m_showHideInfo == null)
					{
						m_showHideInfo = GetSpecialChunkInfo("ShowHideInfo", ref m_hasShowHideInfo);
					}
					if (m_showHideInfo != null)
					{
						m_showHideInfo.Position = 0L;
						result = new IntermediateFormatReader(m_showHideInfo, m_instanceObjects);
					}
					return result;
				}
			}

			internal IntermediateFormatReader BookmarkReader
			{
				get
				{
					IntermediateFormatReader result = null;
					if (m_hasBookmarks && m_bookmarks == null)
					{
						m_bookmarks = GetSpecialChunkInfo("Bookmarks", ref m_hasBookmarks);
					}
					if (m_bookmarks != null)
					{
						m_bookmarks.Position = 0L;
						result = new IntermediateFormatReader(m_bookmarks, m_instanceObjects);
					}
					return result;
				}
			}

			internal IntermediateFormatReader DrillthroughReader
			{
				get
				{
					IntermediateFormatReader result = null;
					if (m_hasDrillthrough && m_drillthrough == null)
					{
						m_drillthrough = GetSpecialChunkInfo("Drillthrough", ref m_hasDrillthrough);
					}
					if (m_drillthrough != null)
					{
						m_drillthrough.Position = 0L;
						result = new IntermediateFormatReader(m_drillthrough, m_instanceObjects);
					}
					return result;
				}
			}

			internal IntermediateFormatReader QuickFindReader
			{
				get
				{
					IntermediateFormatReader result = null;
					if (m_hasQuickFind && m_quickFind == null)
					{
						m_quickFind = GetSpecialChunkInfo("QuickFind", ref m_hasQuickFind);
					}
					if (m_quickFind != null)
					{
						m_quickFind.Position = 0L;
						result = new IntermediateFormatReader(m_quickFind, m_instanceObjects);
					}
					return result;
				}
			}

			internal IntermediateFormatReader SortFilterEventInfoReader
			{
				get
				{
					IntermediateFormatReader result = null;
					if (m_hasSortFilterEventInfo && m_sortFilterEventInfo == null)
					{
						m_sortFilterEventInfo = GetSpecialChunkInfo("SortFilterEventInfo", ref m_hasSortFilterEventInfo);
					}
					if (m_sortFilterEventInfo != null)
					{
						m_sortFilterEventInfo.Position = 0L;
						result = new IntermediateFormatReader(m_sortFilterEventInfo, m_instanceObjects, m_definitionObjects, m_intermediateFormatVersion);
					}
					return result;
				}
			}

			internal SpecialChunkManager(ReportProcessing.GetReportChunk getChunkCallback, Hashtable instanceObjects, Hashtable definitionObjects, IntermediateFormatVersion intermediateFormatVersion)
			{
				m_definitionObjects = definitionObjects;
				m_instanceObjects = instanceObjects;
				m_getChunkCallback = getChunkCallback;
				m_intermediateFormatVersion = intermediateFormatVersion;
			}

			private Stream GetSpecialChunkInfo(string chunkName, ref bool hasChunk)
			{
				string mimeType;
				Stream stream = m_getChunkCallback(chunkName, ReportProcessing.ReportChunkTypes.Main, out mimeType);
				if (stream == null)
				{
					hasChunk = false;
				}
				return stream;
			}

			internal void Close()
			{
				if (m_docMap != null)
				{
					m_docMap.Close();
					m_docMap = null;
					m_hasDocMap = true;
				}
				if (m_showHideInfo != null)
				{
					m_showHideInfo.Close();
					m_showHideInfo = null;
					m_hasShowHideInfo = true;
				}
				if (m_bookmarks != null)
				{
					m_bookmarks.Close();
					m_bookmarks = null;
					m_hasBookmarks = true;
				}
				if (m_drillthrough != null)
				{
					m_drillthrough.Close();
					m_drillthrough = null;
					m_hasDrillthrough = true;
				}
				if (m_quickFind != null)
				{
					m_quickFind.Close();
					m_quickFind = null;
					m_hasQuickFind = true;
				}
				if (m_sortFilterEventInfo != null)
				{
					m_sortFilterEventInfo.Close();
					m_sortFilterEventInfo = null;
					m_hasSortFilterEventInfo = true;
				}
			}
		}

		internal sealed class RenderingChunkManager
		{
			private ReportProcessing.GetReportChunk m_getChunkCallback;

			private SpecialChunkManager m_specialChunkManager;

			private Stream m_firstPageChunk;

			private IntermediateFormatReader m_firstPageReader;

			private Stream m_otherPageChunk;

			private IntermediateFormatReader m_otherPageReader;

			private Hashtable m_instanceObjects;

			private Hashtable m_definitionObjects;

			private IntermediateFormatReader.State m_declarationsRead;

			private IntermediateFormatVersion m_intermediateFormatVersion;

			private Stream m_specialChunk;

			private IntermediateFormatReader m_specialChunkReader;

			private Stream m_pageSectionChunk;

			private IntermediateFormatReader m_pageSectionReader;

			private Stream m_pageSectionInstanceChunk;

			private IntermediateFormatReader m_pageSectionInstanceReader;

			private IntermediateFormatVersion m_pageSectionIntermediateFormatVersion;

			private IntermediateFormatReader.State m_pageSectionDeclarationsRead;

			private int m_pageSectionLastReadPage = -1;

			internal RenderingChunkManager(ReportProcessing.GetReportChunk getChunkCallback, Hashtable instanceObjects, Hashtable definitionObjects, IntermediateFormatReader.State declarationsRead, IntermediateFormatVersion intermediateFormatVersion)
			{
				m_getChunkCallback = getChunkCallback;
				m_instanceObjects = instanceObjects;
				m_definitionObjects = definitionObjects;
				m_declarationsRead = declarationsRead;
				m_intermediateFormatVersion = intermediateFormatVersion;
				if (intermediateFormatVersion != null && intermediateFormatVersion.IsRS2005_WithPHFChunks)
				{
					m_pageSectionIntermediateFormatVersion = intermediateFormatVersion;
				}
				else
				{
					m_pageSectionIntermediateFormatVersion = new IntermediateFormatVersion();
				}
			}

			internal IntermediateFormatReader GetReaderForSpecialChunk(long offset)
			{
				if (m_specialChunkReader == null)
				{
					if (m_specialChunk == null)
					{
						m_specialChunk = m_getChunkCallback("Special", ReportProcessing.ReportChunkTypes.Main, out string _);
					}
					m_specialChunkReader = new IntermediateFormatReader(m_specialChunk, m_instanceObjects, m_intermediateFormatVersion);
				}
				m_specialChunk.Position = offset;
				return m_specialChunkReader;
			}

			internal IntermediateFormatReader GetSpecialChunkReader(SpecialChunkName chunkName)
			{
				if (m_specialChunkManager == null)
				{
					m_specialChunkManager = new SpecialChunkManager(m_getChunkCallback, m_instanceObjects, m_definitionObjects, m_intermediateFormatVersion);
				}
				switch (chunkName)
				{
				case SpecialChunkName.DocumentMap:
					return m_specialChunkManager.DocumentMapReader;
				case SpecialChunkName.Bookmark:
					return m_specialChunkManager.BookmarkReader;
				case SpecialChunkName.ShowHideInfo:
					return m_specialChunkManager.ShowHideInfoReader;
				case SpecialChunkName.QuickFind:
					return m_specialChunkManager.QuickFindReader;
				case SpecialChunkName.SortFilterEventInfo:
					return m_specialChunkManager.SortFilterEventInfoReader;
				default:
					return null;
				}
			}

			internal bool PageSectionChunkExists()
			{
				m_pageSectionChunk = m_getChunkCallback("PageSections", ReportProcessing.ReportChunkTypes.Main, out string _);
				if (m_pageSectionChunk != null)
				{
					m_pageSectionChunk.Close();
					m_pageSectionChunk = null;
					return true;
				}
				return false;
			}

			internal IntermediateFormatReader GetPageSectionReader(int requestedPageNumber, out int currentPageNumber)
			{
				currentPageNumber = m_pageSectionLastReadPage + 1;
				bool flag = m_pageSectionReader == null;
				if (m_pageSectionChunk == null)
				{
					m_pageSectionChunk = m_getChunkCallback("PageSections", ReportProcessing.ReportChunkTypes.Main, out string _);
				}
				else if (requestedPageNumber < 0 || requestedPageNumber <= m_pageSectionLastReadPage)
				{
					m_pageSectionChunk.Position = 0L;
					currentPageNumber = 0;
					flag = true;
				}
				if (flag)
				{
					m_pageSectionReader = new IntermediateFormatReader(m_pageSectionChunk, m_pageSectionIntermediateFormatVersion);
				}
				return m_pageSectionReader;
			}

			internal void SetPageSectionReaderState(IntermediateFormatReader.State declarations, int pageSectionLastReadPage)
			{
				m_pageSectionDeclarationsRead = declarations;
				m_pageSectionLastReadPage = pageSectionLastReadPage;
			}

			internal IntermediateFormatReader GetPageSectionInstanceReader(long offset)
			{
				if (m_pageSectionInstanceReader == null)
				{
					if (m_pageSectionInstanceChunk == null)
					{
						m_pageSectionInstanceChunk = m_getChunkCallback("PageSectionInstances", ReportProcessing.ReportChunkTypes.Other, out string _);
					}
					if (m_pageSectionInstanceChunk != null)
					{
						m_pageSectionInstanceReader = new IntermediateFormatReader(m_pageSectionInstanceChunk, m_pageSectionDeclarationsRead, m_pageSectionIntermediateFormatVersion);
					}
				}
				if (m_pageSectionInstanceChunk != null)
				{
					m_pageSectionInstanceChunk.Position = offset;
				}
				return m_pageSectionInstanceReader;
			}

			internal IntermediateFormatReader GetReader(long offset)
			{
				if (offset < 0)
				{
					if (m_firstPageReader == null)
					{
						if (m_firstPageChunk == null)
						{
							m_firstPageChunk = m_getChunkCallback("FirstPage", ReportProcessing.ReportChunkTypes.Main, out string _);
						}
						m_firstPageReader = new IntermediateFormatReader(m_firstPageChunk, m_declarationsRead, m_definitionObjects, m_intermediateFormatVersion);
					}
					m_firstPageChunk.Position = -offset;
					return m_firstPageReader;
				}
				if (m_otherPageReader == null)
				{
					if (m_otherPageChunk == null)
					{
						m_otherPageChunk = m_getChunkCallback("OtherPages", ReportProcessing.ReportChunkTypes.Other, out string _);
					}
					m_otherPageReader = new IntermediateFormatReader(m_otherPageChunk, m_declarationsRead, m_definitionObjects, m_intermediateFormatVersion);
				}
				m_otherPageChunk.Position = offset;
				return m_otherPageReader;
			}

			internal void Close()
			{
				m_firstPageReader = null;
				if (m_firstPageChunk != null)
				{
					m_firstPageChunk.Close();
					m_firstPageChunk = null;
				}
				m_otherPageReader = null;
				if (m_otherPageChunk != null)
				{
					m_otherPageChunk.Close();
					m_otherPageChunk = null;
				}
				if (m_specialChunkManager != null)
				{
					m_specialChunkManager.Close();
					m_specialChunkManager = null;
				}
				m_specialChunkReader = null;
				if (m_specialChunk != null)
				{
					m_specialChunk.Close();
					m_specialChunk = null;
				}
				m_pageSectionReader = null;
				if (m_pageSectionChunk != null)
				{
					m_pageSectionChunk.Close();
					m_pageSectionChunk = null;
				}
				m_pageSectionInstanceReader = null;
				if (m_pageSectionInstanceChunk != null)
				{
					m_pageSectionInstanceChunk.Close();
					m_pageSectionInstanceChunk = null;
				}
			}
		}

		internal const string Definition = "CompiledDefinition";

		internal const string MainChunk = "Main";

		internal const string FirstPageChunk = "FirstPage";

		internal const string OtherPageChunk = "OtherPages";

		internal const string SpecialChunk = "Special";

		internal const string DocumentMap = "DocumentMap";

		internal const string ShowHideInfo = "ShowHideInfo";

		internal const string Bookmarks = "Bookmarks";

		internal const string Drillthrough = "Drillthrough";

		internal const string QuickFind = "QuickFind";

		internal const string SortFilterEventInfo = "SortFilterEventInfo";

		internal const string DataChunkPrefix = "DataChunk";

		internal const string PageSections = "PageSections";

		internal const string PageSectionInstances = "PageSectionInstances";

		internal const string Delimiter = "_";

		private const int InstancePerChunk = 4096;

		private const int RecordRowPerChunk = 4096;

		private static string GenerateDataChunkName(string dataSetName, string subReportName, bool isShareable, int reportUniqueName)
		{
			if (-1 == reportUniqueName)
			{
				return "DataChunk_" + dataSetName;
			}
			if (isShareable && subReportName != null)
			{
				return "DataChunk" + subReportName + "_" + dataSetName;
			}
			return "DataChunk" + reportUniqueName + "_" + dataSetName;
		}

		private static string GenerateDataChunkName(DataSet dataSet, ReportProcessing.ProcessingContext context, bool writeOperation)
		{
			string text = null;
			string subReportName = null;
			if (context.SubReportLevel != 0)
			{
				subReportName = context.ReportContext.StableItemPath;
			}
			if (dataSet.IsShareable())
			{
				text = (context.CachedDataChunkMapping[dataSet.ID] as string);
				if (text == null)
				{
					text = GenerateDataChunkName(dataSet.Name, subReportName, isShareable: true, context.DataSetUniqueName);
					if (writeOperation)
					{
						context.CachedDataChunkMapping.Add(dataSet.ID, text);
					}
				}
			}
			else
			{
				text = GenerateDataChunkName(dataSet.Name, subReportName, isShareable: false, context.DataSetUniqueName);
			}
			return text;
		}
	}
}
