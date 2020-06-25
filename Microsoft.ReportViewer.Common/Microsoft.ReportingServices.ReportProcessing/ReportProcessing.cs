using Microsoft.Cloud.Platform.Utils;
using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.DataExtensions;
using Microsoft.ReportingServices.DataProcessing;
using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Diagnostics.Internal;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing.Execution;
using Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel;
using Microsoft.ReportingServices.ReportProcessing.Persistence;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using Microsoft.ReportingServices.ReportProcessing.Upgrade;
using Microsoft.ReportingServices.ReportPublishing;
using Microsoft.ReportingServices.ReportRendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal sealed class ReportProcessing
	{
		private sealed class ShowHideProcessing
		{
			private bool m_showHideInfoChanged;

			private SenderInformationHashtable m_senderInfo;

			private ReceiverInformationHashtable m_receiverInfo;

			private Hashtable m_overrideToggleStateInfo;

			private Hashtable m_overrideHiddenInfo;

			internal void Process(string showHideToggle, ReportSnapshot reportSnapshot, EventInformation oldOverrideInformation, ChunkManager.RenderingChunkManager chunkManager, out bool showHideInformationChanged, out EventInformation newOverrideInformation)
			{
				try
				{
					showHideInformationChanged = false;
					newOverrideInformation = null;
					if (showHideToggle != null)
					{
						Global.Tracer.Assert(reportSnapshot != null, "(null != reportSnapshot)");
						m_senderInfo = reportSnapshot.GetShowHideSenderInfo(chunkManager);
						m_receiverInfo = reportSnapshot.GetShowHideReceiverInfo(chunkManager);
						Process(showHideToggle, oldOverrideInformation, ref showHideInformationChanged, ref newOverrideInformation);
					}
				}
				finally
				{
					m_showHideInfoChanged = false;
					m_senderInfo = null;
					m_receiverInfo = null;
					m_overrideToggleStateInfo = null;
					m_overrideHiddenInfo = null;
				}
			}

			internal bool Process(string showHideToggle, EventInformation oldOverrideInformation, ChunkManager.EventsChunkManager eventsChunkManager, out bool showHideInformationChanged, out EventInformation newOverrideInformation)
			{
				try
				{
					showHideInformationChanged = false;
					newOverrideInformation = null;
					if (showHideToggle == null)
					{
						return false;
					}
					Global.Tracer.Assert(eventsChunkManager != null, "(null != eventsChunkManager)");
					eventsChunkManager.GetShowHideInfo(out m_senderInfo, out m_receiverInfo);
					return Process(showHideToggle, oldOverrideInformation, ref showHideInformationChanged, ref newOverrideInformation);
				}
				finally
				{
					m_showHideInfoChanged = false;
					m_senderInfo = null;
					m_receiverInfo = null;
					m_overrideToggleStateInfo = null;
					m_overrideHiddenInfo = null;
				}
			}

			private bool Process(string showHideToggle, EventInformation oldOverrideInformation, ref bool showHideInformationChanged, ref EventInformation newOverrideInformation)
			{
				if (m_senderInfo == null || m_receiverInfo == null)
				{
					return false;
				}
				if (!int.TryParse(showHideToggle, NumberStyles.None, CultureInfo.InvariantCulture, out int result))
				{
					return false;
				}
				EventInformation.SortEventInfo sortInfo = null;
				if (oldOverrideInformation == null || (oldOverrideInformation.ToggleStateInfo == null && oldOverrideInformation.HiddenInfo == null))
				{
					m_overrideToggleStateInfo = new Hashtable();
					m_overrideHiddenInfo = new Hashtable();
					if (oldOverrideInformation != null)
					{
						sortInfo = oldOverrideInformation.SortInfo;
					}
				}
				else
				{
					m_overrideToggleStateInfo = (Hashtable)oldOverrideInformation.ToggleStateInfo.Clone();
					m_overrideHiddenInfo = (Hashtable)oldOverrideInformation.HiddenInfo.Clone();
				}
				bool result2 = ProcessSender(result);
				showHideInformationChanged = m_showHideInfoChanged;
				if (!m_showHideInfoChanged)
				{
					newOverrideInformation = null;
					return result2;
				}
				if (m_overrideToggleStateInfo.Count == 0 && m_overrideHiddenInfo.Count == 0)
				{
					newOverrideInformation = null;
					return result2;
				}
				newOverrideInformation = new EventInformation();
				newOverrideInformation.ToggleStateInfo = m_overrideToggleStateInfo;
				newOverrideInformation.HiddenInfo = m_overrideHiddenInfo;
				newOverrideInformation.SortInfo = sortInfo;
				return result2;
			}

			private bool ProcessSender(int senderUniqueName)
			{
				SenderInformation senderInformation = m_senderInfo[senderUniqueName];
				if (senderInformation == null)
				{
					return false;
				}
				UpdateOverrideToggleStateInfo(senderUniqueName);
				for (int i = 0; i < senderInformation.ReceiverUniqueNames.Count; i++)
				{
					ProcessReceiver(senderInformation.ReceiverUniqueNames[i]);
				}
				return true;
			}

			private void ProcessReceiver(int receiverUniqueName)
			{
				ReceiverInformation receiverInformation = m_receiverInfo[receiverUniqueName];
				Global.Tracer.Assert(receiverInformation != null, "(null != receiver)");
				UpdateOverrideHiddenInfo(receiverUniqueName);
			}

			private void UpdateOverrideToggleStateInfo(int uniqueName)
			{
				m_showHideInfoChanged = true;
				if (!m_overrideToggleStateInfo.ContainsKey(uniqueName))
				{
					m_overrideToggleStateInfo.Add(uniqueName, null);
				}
				else
				{
					m_overrideToggleStateInfo.Remove(uniqueName);
				}
			}

			private void UpdateOverrideHiddenInfo(int uniqueName)
			{
				m_showHideInfoChanged = true;
				if (!m_overrideHiddenInfo.ContainsKey(uniqueName))
				{
					m_overrideHiddenInfo.Add(uniqueName, null);
				}
				else
				{
					m_overrideHiddenInfo.Remove(uniqueName);
				}
			}
		}

		public delegate void OnDemandSubReportCallback(ICatalogItemContext reportContext, string subreportPath, string newChunkName, NeedsUpgrade upgradeCheck, ParameterInfoCollection parentQueryParameters, out ICatalogItemContext subreportContext, out string description, out IChunkFactory getCompiledDefinitionCallback, out ParameterInfoCollection parameters);

		public delegate void OnDemandSubReportDataSourcesCallback(ICatalogItemContext reportContext, string subreportPath, NeedsUpgrade upgradeCheck, out ICatalogItemContext subreportContext, out IChunkFactory getCompiledDefinitionCallback, out DataSourceInfoCollection dataSources, out DataSetInfoCollection dataSetReferences);

		public delegate bool NeedsUpgrade(ReportProcessingFlags processingFlags);

		internal delegate void SubReportCallback(ICatalogItemContext reportContext, string subreportPath, out ICatalogItemContext subreportContext, out string description, out GetReportChunk getCompiledDefinitionCallback, out ParameterInfoCollection parameters);

		internal delegate void SubReportDataSourcesCallback(ICatalogItemContext reportContext, string subreportPath, out ICatalogItemContext subreportContext, out GetReportChunk getCompiledDefinitionCallback, out DataSourceInfoCollection dataSources);

		public interface IErasable
		{
			bool Erase();
		}

		public delegate Microsoft.ReportingServices.DataExtensions.DataSourceInfo CheckSharedDataSource(string dataSourcePath, out Guid catalogItemId);

		internal delegate Stream CreateReportChunk(string name, ReportChunkTypes type, string mimeType);

		internal delegate Stream GetReportChunk(string name, ReportChunkTypes type, out string mimeType);

		internal delegate string GetChunkMimeType(string name, ReportChunkTypes type);

		public delegate NameValueCollection StoreServerParameters(ICatalogItemContext item, NameValueCollection reportParameters, bool[] sharedParameters, out bool replaced);

		public delegate IDbConnection CreateDataExtensionInstance(string extensionName, Guid modelID);

		public delegate IDbConnection CreateAndSetupDataExtensionInstance(Microsoft.ReportingServices.ReportIntermediateFormat.DataSource dataSource, Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet, OnDemandProcessingContext odpContext);

		public delegate void ResolveTemporaryDataSource(Microsoft.ReportingServices.DataExtensions.DataSourceInfo dataSourceInfo, DataSourceInfoCollection originalDataSources);

		[Serializable]
		internal sealed class DataCacheUnavailableException : Exception
		{
			public DataCacheUnavailableException()
			{
			}

			public DataCacheUnavailableException(DataCacheUnavailableException ex)
				: base(ex.Message, ex)
			{
			}

			private DataCacheUnavailableException(SerializationInfo info, StreamingContext context)
				: base(info, context)
			{
			}
		}

		public enum ReportChunkTypes
		{
			Main,
			Image,
			Other,
			StaticImage,
			ServerRdlMapping,
			Data,
			Interactivity,
			Pagination,
			Rendering,
			CompiledDefinition,
			GeneratedReportItems,
			LookupInfo,
			Shapefile
		}

		public enum ExecutionType
		{
			Live,
			ServiceAccount,
			SurrogateAccount
		}

		internal class ProcessingComparer : IDataComparer, IEqualityComparer, IEqualityComparer<object>, IComparer, IComparer<object>, IStaticReferenceable
		{
			private readonly CompareInfo m_compareInfo;

			private readonly CompareOptions m_compareOptions;

			private readonly bool m_nullsAsBlanks;

			private readonly bool m_defaultThrowExceptionOnComparisonFailure;

			private CultureInfo m_cultureInfo;

			private int m_staticRefId = int.MaxValue;

			int IStaticReferenceable.ID => m_staticRefId;

			internal ProcessingComparer(CompareInfo compareInfo, CompareOptions compareOptions, bool nullsAsBlanks)
				: this(compareInfo, compareOptions, nullsAsBlanks, defaultThrowExceptionOnComparisonFailure: true)
			{
			}

			internal ProcessingComparer(CompareInfo compareInfo, CompareOptions compareOptions, bool nullsAsBlanks, bool defaultThrowExceptionOnComparisonFailure)
			{
				m_compareInfo = compareInfo;
				m_compareOptions = compareOptions;
				m_nullsAsBlanks = nullsAsBlanks;
				m_defaultThrowExceptionOnComparisonFailure = defaultThrowExceptionOnComparisonFailure;
			}

			void IStaticReferenceable.SetID(int id)
			{
				m_staticRefId = id;
			}

			Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType IStaticReferenceable.GetObjectType()
			{
				return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ProcessingComparer;
			}

			bool IEqualityComparer.Equals(object x, object y)
			{
				return InternalEquals(x, y);
			}

			bool IEqualityComparer<object>.Equals(object x, object y)
			{
				return InternalEquals(x, y);
			}

			private bool InternalEquals(object x, object y)
			{
				bool validComparisonResult;
				return Compare(x, y, m_defaultThrowExceptionOnComparisonFailure, extendedTypeComparisons: false, out validComparisonResult) == 0;
			}

			int IComparer.Compare(object x, object y)
			{
				return Compare(x, y);
			}

			public int Compare(object x, object y)
			{
				bool validComparisonResult;
				return Compare(x, y, m_defaultThrowExceptionOnComparisonFailure, extendedTypeComparisons: false, out validComparisonResult);
			}

			public int Compare(object x, object y, bool extendedTypeComparisons)
			{
				bool validComparisonResult;
				return Compare(x, y, m_defaultThrowExceptionOnComparisonFailure, extendedTypeComparisons, out validComparisonResult);
			}

			public int Compare(object x, object y, bool throwExceptionOnComparisonFailure, bool extendedTypeComparisons, out bool validComparisonResult)
			{
				return CompareTo(x, y, m_nullsAsBlanks, m_compareInfo, m_compareOptions, throwExceptionOnComparisonFailure, extendedTypeComparisons, out validComparisonResult);
			}

			public int GetHashCode(object obj)
			{
				bool valid;
				DataAggregate.DataTypeCode typeCode = DataAggregate.GetTypeCode(obj, throwException: false, out valid);
				if (!valid)
				{
					return obj.GetHashCode();
				}
				switch (typeCode)
				{
				case DataAggregate.DataTypeCode.String:
				{
					string text = (string)obj;
					if (CompareOptions.None < (CompareOptions.IgnoreCase & m_compareOptions))
					{
						if (m_cultureInfo == null)
						{
							m_cultureInfo = new CultureInfo(m_compareInfo.Name, useUserOverride: false);
						}
						text = text.ToUpper(m_cultureInfo);
					}
					return InternalGetHashCode(text);
				}
				case DataAggregate.DataTypeCode.Null:
					return 0;
				case DataAggregate.DataTypeCode.Int32:
					return (int)obj;
				case DataAggregate.DataTypeCode.Int16:
					return (short)obj;
				case DataAggregate.DataTypeCode.Byte:
					return (byte)obj;
				case DataAggregate.DataTypeCode.UInt16:
					return (ushort)obj;
				case DataAggregate.DataTypeCode.SByte:
					return (sbyte)obj;
				case DataAggregate.DataTypeCode.UInt32:
					return (int)(uint)obj;
				case DataAggregate.DataTypeCode.Int64:
				{
					long num6 = (long)obj;
					if (num6 < int.MaxValue)
					{
						return (int)num6;
					}
					return InternalGetHashCode(num6);
				}
				case DataAggregate.DataTypeCode.UInt64:
				{
					ulong num5 = (ulong)obj;
					if (num5 < int.MaxValue)
					{
						return (int)num5;
					}
					return (int)num5 ^ (int)(num5 >> 32);
				}
				case DataAggregate.DataTypeCode.Double:
				{
					double num = (double)obj;
					int num2 = (int)num;
					if (num == (double)num2)
					{
						return num2;
					}
					return InternalGetHashCode(num);
				}
				case DataAggregate.DataTypeCode.Single:
				{
					float num7 = (float)obj;
					int num8 = (int)num7;
					if (num7 == (float)num8)
					{
						return num8;
					}
					return InternalGetHashCode(num7);
				}
				case DataAggregate.DataTypeCode.Decimal:
				{
					decimal num3 = (decimal)obj;
					decimal num4 = decimal.Truncate(num3);
					if (num3 == num4)
					{
						if (num4 >= -2147483648m && num4 <= 2147483647m)
						{
							return (int)num4;
						}
						if (num4 >= new decimal(long.MinValue) && num4 <= new decimal(long.MaxValue))
						{
							return InternalGetHashCode((long)num4);
						}
					}
					return InternalGetHashCode((double)num3);
				}
				case DataAggregate.DataTypeCode.DateTime:
					return InternalGetHashCode((DateTime)obj);
				case DataAggregate.DataTypeCode.DateTimeOffset:
					return InternalGetHashCode(((DateTimeOffset)obj).UtcDateTime);
				case DataAggregate.DataTypeCode.TimeSpan:
				{
					long ticks = ((TimeSpan)obj).Ticks;
					return (int)ticks ^ (int)(ticks >> 32);
				}
				case DataAggregate.DataTypeCode.Char:
				{
					char c = (char)obj;
					return (int)(c | ((uint)c << 16));
				}
				case DataAggregate.DataTypeCode.Boolean:
					if ((bool)obj)
					{
						return 1;
					}
					return 0;
				default:
					return obj.GetHashCode();
				}
			}

			private static int InternalGetHashCode(string input)
			{
				int num = 5381;
				int num2 = num;
				int num3 = input.Length - 1;
				int length = input.Length;
				int num4;
				for (num4 = 0; num4 < length; num4++)
				{
					num = (((num << 5) + num) ^ input[num4]);
					if (num4 == num3)
					{
						break;
					}
					num4++;
					num2 = (((num2 << 5) + num2) ^ input[num4]);
				}
				return num + num2 * 1566083941;
			}

			private static int InternalGetHashCode(double value)
			{
				if (value == 0.0)
				{
					return 0;
				}
				long num = BitConverter.DoubleToInt64Bits(value);
				return (int)num ^ (int)(num >> 32);
			}

			private static int InternalGetHashCode(DateTime value)
			{
				long ticks = value.Ticks;
				return (int)ticks ^ (int)(ticks >> 32);
			}

			private static int InternalGetHashCode(long value)
			{
				return (int)value ^ (int)(value >> 32);
			}
		}

		internal enum ProcessingStages
		{
			Grouping = 1,
			SortAndFilter,
			RunningValues,
			CreatingInstances,
			UserSortFilter
		}

		[Flags]
		internal enum DataActions
		{
			None = 0x0,
			RecursiveAggregates = 0x1,
			PostSortAggregates = 0x2,
			UserSort = 0x4
		}

		internal sealed class DataSourceInfoHashtable : Hashtable
		{
			internal DataSourceInfo this[string dataSourceName] => (DataSourceInfo)base[dataSourceName];

			internal void Add(IProcessingDataSource procDataSource, IDbConnection connection, TransactionInfo transInfo, Microsoft.ReportingServices.DataExtensions.DataSourceInfo dataExtDataSourceInfo)
			{
				DataSourceInfo value = new DataSourceInfo(procDataSource, connection, transInfo, dataExtDataSourceInfo);
				Add(procDataSource.Name, value);
			}
		}

		internal sealed class TransactionInfo
		{
			internal bool RollbackRequired;

			private IDbTransaction m_transaction;

			internal IDbTransaction Transaction => m_transaction;

			internal TransactionInfo(IDbTransaction transaction)
			{
				Global.Tracer.Assert(transaction != null, "A transaction information object cannot have a null transaction.");
				m_transaction = transaction;
			}
		}

		internal struct TableColumnInfo
		{
			internal int StartIndex;

			internal int Span;
		}

		internal sealed class DataSourceInfo
		{
			private IDbConnection m_connection;

			private TransactionInfo m_transactionInfo;

			private Microsoft.ReportingServices.DataExtensions.DataSourceInfo m_dataExtDataSourceInfo;

			private IProcessingDataSource m_procDataSourceInfo;

			internal string DataSourceName => m_procDataSourceInfo.Name;

			internal Microsoft.ReportingServices.DataExtensions.DataSourceInfo DataExtDataSourceInfo => m_dataExtDataSourceInfo;

			internal IProcessingDataSource ProcDataSourceInfo => m_procDataSourceInfo;

			internal IDbConnection Connection => m_connection;

			internal TransactionInfo TransactionInfo => m_transactionInfo;

			internal DataSourceInfo(IProcessingDataSource procDataSourceInfo, IDbConnection connection, TransactionInfo ti, Microsoft.ReportingServices.DataExtensions.DataSourceInfo dataExtDataSourceInfo)
			{
				m_procDataSourceInfo = procDataSourceInfo;
				m_connection = connection;
				m_transactionInfo = ti;
				m_dataExtDataSourceInfo = dataExtDataSourceInfo;
			}
		}

		internal sealed class ProcessingAbortEventArgs : EventArgs
		{
			private int m_reportUniqueName;

			internal int ReportUniqueName => m_reportUniqueName;

			internal ProcessingAbortEventArgs(int reportUniqueName)
			{
				m_reportUniqueName = reportUniqueName;
			}
		}

		internal sealed class ThreadSet
		{
			private int m_threadsRemaining;

			private ManualResetEvent m_allThreadsDone;

			internal ThreadSet(int threadCount)
			{
				if (Global.Tracer.TraceVerbose)
				{
					Global.Tracer.Trace(TraceLevel.Verbose, "ThreadSet object created. {0} threads remaining.", threadCount);
				}
				m_threadsRemaining = threadCount;
				m_allThreadsDone = new ManualResetEvent(threadCount <= 0);
			}

			internal void ThreadCompleted()
			{
				int num = Interlocked.Decrement(ref m_threadsRemaining);
				if (Global.Tracer.TraceVerbose)
				{
					Global.Tracer.Trace(TraceLevel.Verbose, "Thread completed. {0} thread remaining.", num);
				}
				if (num <= 0)
				{
					m_allThreadsDone.Set();
				}
			}

			internal void WaitForCompletion()
			{
				m_allThreadsDone.WaitOne();
				if (Global.Tracer.TraceVerbose)
				{
					Global.Tracer.Trace(TraceLevel.Verbose, "All the processing threads have completed.");
				}
			}
		}

		internal class ProcessingContext : IInternalProcessingContext
		{
			[Flags]
			internal enum SecondPassOperations
			{
				Sorting = 0x1,
				Filtering = 0x2
			}

			internal sealed class AbortHelper : IAbortHelper, IDisposable
			{
				private ProcessingStatus m_overallStatus;

				private Exception m_exception;

				private Hashtable m_reportStatus;

				private IJobContext m_jobContext;

				internal event EventHandler ProcessingAbortEvent;

				internal AbortHelper(IJobContext jobContext)
				{
					m_reportStatus = new Hashtable();
					if (jobContext != null)
					{
						m_jobContext = jobContext;
						jobContext.AddAbortHelper(this);
					}
				}

				internal void ThrowAbortException(int reportUniqueName)
				{
					if (GetStatus(reportUniqueName) == ProcessingStatus.AbnormalTermination)
					{
						throw new ProcessingAbortedException(m_exception);
					}
					throw new ProcessingAbortedException();
				}

				public bool Abort(ProcessingStatus status)
				{
					return Abort(-1, status);
				}

				internal bool Abort(int reportUniqueName, ProcessingStatus status)
				{
					if (!Monitor.TryEnter(this))
					{
						if (Global.Tracer.TraceInfo)
						{
							Global.Tracer.Trace(TraceLevel.Info, "Some other thread is aborting processing.");
						}
						return false;
					}
					if (GetStatus(reportUniqueName) != 0)
					{
						if (Global.Tracer.TraceInfo)
						{
							Global.Tracer.Trace(TraceLevel.Info, "Some other thread has already aborted processing.");
						}
						Monitor.Exit(this);
						return false;
					}
					bool result = false;
					try
					{
						SetStatus(reportUniqueName, status);
						if (this.ProcessingAbortEvent != null)
						{
							try
							{
								this.ProcessingAbortEvent(this, new ProcessingAbortEventArgs(reportUniqueName));
								result = true;
								if (!Global.Tracer.TraceVerbose)
								{
									return result;
								}
								Global.Tracer.Trace(TraceLevel.Verbose, "Abort callback successful.");
								return result;
							}
							catch (Exception ex)
							{
								if (!Global.Tracer.TraceError)
								{
									return result;
								}
								Global.Tracer.Trace(TraceLevel.Error, "Exception in abort callback. Details: {0}", ex.ToString());
								return result;
							}
						}
						if (Global.Tracer.TraceVerbose)
						{
							Global.Tracer.Trace(TraceLevel.Verbose, "No abort callback.");
							return result;
						}
						return result;
					}
					finally
					{
						Monitor.Exit(this);
					}
				}

				internal void AddSubreportInstance(int subreportInstanceUniqueName)
				{
					Hashtable hashtable = Hashtable.Synchronized(m_reportStatus);
					Global.Tracer.Assert(!hashtable.ContainsKey(subreportInstanceUniqueName), "(false == reportStatus.ContainsKey(subreportInstanceUniqueName))");
					hashtable.Add(subreportInstanceUniqueName, ProcessingStatus.Success);
				}

				internal ProcessingStatus GetStatus(int uniqueName)
				{
					if (-1 == uniqueName)
					{
						return m_overallStatus;
					}
					Global.Tracer.Assert(m_reportStatus.ContainsKey(uniqueName), "(m_reportStatus.ContainsKey(uniqueName))");
					return (ProcessingStatus)m_reportStatus[uniqueName];
				}

				private void SetStatus(int uniqueName, ProcessingStatus newStatus)
				{
					if (-1 == uniqueName)
					{
						m_overallStatus = newStatus;
						return;
					}
					Hashtable hashtable = Hashtable.Synchronized(m_reportStatus);
					Global.Tracer.Assert(hashtable.ContainsKey(uniqueName), "(reportStatus.ContainsKey(uniqueName))");
					hashtable[uniqueName] = newStatus;
				}

				internal bool SetError(int reportUniqueName, Exception e)
				{
					if (Global.Tracer.TraceVerbose)
					{
						Global.Tracer.Trace(TraceLevel.Verbose, "An exception has occurred. Trying to abort processing. Details: {0}", e.ToString());
					}
					m_exception = e;
					if (!Abort(reportUniqueName, ProcessingStatus.AbnormalTermination))
					{
						return false;
					}
					return true;
				}

				public void Dispose()
				{
					if (m_jobContext != null)
					{
						m_jobContext.RemoveAbortHelper();
					}
				}
			}

			private sealed class CommonInfo
			{
				private bool m_isOnePass;

				private int m_uniqueNameCounter;

				private long m_dataProcessingDurationMs;

				private string m_owcChartName;

				private OWCChartInstanceInfo m_owcChartInstance;

				private string m_requestUserName;

				private CultureInfo m_userLanguage;

				private SubReportCallback m_subReportCallback;

				private ChunkManager.ProcessingChunkManager m_chunkManager;

				private CreateReportChunk m_createReportChunkCallback;

				private IChunkFactory m_createReportChunkFactory;

				private QuickFindHashtable m_quickFind;

				private IGetResource m_getResourceCallback;

				private int m_idCounterForSubreports;

				private ExecutionType m_interactiveExecution;

				private bool m_hasImageStreams;

				private DateTime m_executionTime;

				private UserProfileState m_allowUserProfileState;

				private bool m_hasUserSortFilter;

				private bool m_saveSnapshotData = true;

				private bool m_stopSaveSnapshotDataOnError;

				private bool m_errorSavingSnapshotData;

				private bool m_isHistorySnapshot;

				private bool m_snapshotProcessing;

				private bool m_userSortFilterProcessing;

				private bool m_resetForSubreportDataPrefetch;

				private bool m_processWithCachedData;

				private GetReportChunk m_getReportChunkCallback;

				private CreateReportChunk m_cacheDataCallback;

				private bool m_dataCached;

				private Hashtable m_cachedDataChunkMapping;

				private ReportDrillthroughInfo m_drillthroughInfo;

				private CustomReportItemControls m_criControls;

				private EventInformation m_userSortFilterInfo;

				private SortFilterEventInfoHashtable m_oldSortFilterEventInfo;

				private SortFilterEventInfoHashtable m_newSortFilterEventInfo;

				private RuntimeSortFilterEventInfoList m_reportRuntimeUserSortFilterInfo;

				private ReportRuntimeSetup m_reportRuntimeSetup;

				internal IGetResource GetResourceCallback => m_getResourceCallback;

				internal CreateReportChunk CreateReportChunkCallback => m_createReportChunkCallback;

				internal IChunkFactory CreateReportChunkFactory
				{
					get
					{
						return m_createReportChunkFactory;
					}
					set
					{
						m_createReportChunkFactory = value;
					}
				}

				internal bool IsOnePass => m_isOnePass;

				internal long DataProcessingDurationMs
				{
					get
					{
						return m_dataProcessingDurationMs;
					}
					set
					{
						m_dataProcessingDurationMs = value;
					}
				}

				internal string OWCChartName => m_owcChartName;

				internal OWCChartInstanceInfo OWCChartInstance
				{
					get
					{
						return m_owcChartInstance;
					}
					set
					{
						m_owcChartInstance = value;
					}
				}

				internal string RequestUserName => m_requestUserName;

				internal DateTime ExecutionTime => m_executionTime;

				internal CultureInfo UserLanguage => m_userLanguage;

				internal SubReportCallback SubReportCallback => m_subReportCallback;

				internal ChunkManager.ProcessingChunkManager ChunkManager => m_chunkManager;

				internal QuickFindHashtable QuickFind => m_quickFind;

				internal ReportDrillthroughInfo DrillthroughInfo
				{
					get
					{
						return m_drillthroughInfo;
					}
					set
					{
						m_drillthroughInfo = value;
					}
				}

				internal int UniqueNameCounter => m_uniqueNameCounter;

				internal ExecutionType InteractiveExecution => m_interactiveExecution;

				internal bool HasImageStreams
				{
					get
					{
						return m_hasImageStreams;
					}
					set
					{
						m_hasImageStreams = value;
					}
				}

				internal UserProfileState AllowUserProfileState => m_allowUserProfileState;

				internal bool IsHistorySnapshot => m_isHistorySnapshot;

				internal bool SnapshotProcessing
				{
					get
					{
						return m_snapshotProcessing;
					}
					set
					{
						m_snapshotProcessing = value;
					}
				}

				internal bool UserSortFilterProcessing
				{
					get
					{
						return m_userSortFilterProcessing;
					}
					set
					{
						m_userSortFilterProcessing = value;
					}
				}

				internal bool ResetForSubreportDataPrefetch
				{
					get
					{
						return m_resetForSubreportDataPrefetch;
					}
					set
					{
						m_resetForSubreportDataPrefetch = value;
					}
				}

				internal bool ProcessWithCachedData => m_processWithCachedData;

				internal GetReportChunk GetReportChunkCallback => m_getReportChunkCallback;

				internal CreateReportChunk CacheDataCallback => m_cacheDataCallback;

				internal bool HasUserSortFilter
				{
					get
					{
						return m_hasUserSortFilter;
					}
					set
					{
						m_hasUserSortFilter = value;
					}
				}

				internal bool SaveSnapshotData
				{
					get
					{
						return m_saveSnapshotData;
					}
					set
					{
						m_saveSnapshotData = value;
					}
				}

				internal bool StopSaveSnapshotDataOnError
				{
					get
					{
						return m_stopSaveSnapshotDataOnError;
					}
					set
					{
						m_stopSaveSnapshotDataOnError = value;
					}
				}

				internal bool ErrorSavingSnapshotData
				{
					get
					{
						return m_errorSavingSnapshotData;
					}
					set
					{
						lock (this)
						{
							m_errorSavingSnapshotData = value;
						}
					}
				}

				internal bool DataCached
				{
					get
					{
						return m_dataCached;
					}
					set
					{
						lock (this)
						{
							m_dataCached = value;
						}
					}
				}

				internal Hashtable CachedDataChunkMapping
				{
					get
					{
						return m_cachedDataChunkMapping;
					}
					set
					{
						lock (this)
						{
							m_cachedDataChunkMapping = value;
						}
					}
				}

				internal CustomReportItemControls CriProcessingControls
				{
					get
					{
						return m_criControls;
					}
					set
					{
						lock (this)
						{
							m_criControls = value;
						}
					}
				}

				internal EventInformation UserSortFilterInfo
				{
					get
					{
						return m_userSortFilterInfo;
					}
					set
					{
						m_userSortFilterInfo = value;
					}
				}

				internal SortFilterEventInfoHashtable OldSortFilterEventInfo
				{
					get
					{
						return m_oldSortFilterEventInfo;
					}
					set
					{
						m_oldSortFilterEventInfo = value;
					}
				}

				internal SortFilterEventInfoHashtable NewSortFilterEventInfo
				{
					get
					{
						return m_newSortFilterEventInfo;
					}
					set
					{
						m_newSortFilterEventInfo = value;
					}
				}

				internal RuntimeSortFilterEventInfoList ReportRuntimeUserSortFilterInfo
				{
					get
					{
						return m_reportRuntimeUserSortFilterInfo;
					}
					set
					{
						m_reportRuntimeUserSortFilterInfo = value;
					}
				}

				internal ReportRuntimeSetup ReportRuntimeSetup => m_reportRuntimeSetup;

				internal CommonInfo(string owcChartName, string requestUserName, CultureInfo userLanguage, SubReportCallback subReportCallback, Report report, CreateReportChunk createReportChunkCallback, IGetResource getResourceCallback, ExecutionType interactiveExecution, DateTime executionTime, UserProfileState allowUserProfileState, bool isHistorySnapshot, bool snapshotProcessing, bool processWithCachedData, GetReportChunk getChunkCallback, CreateReportChunk cacheDataCallback, ReportRuntimeSetup reportRuntimeSetup)
				{
					m_uniqueNameCounter = 0;
					m_dataProcessingDurationMs = 0L;
					m_owcChartName = owcChartName;
					m_owcChartInstance = null;
					m_requestUserName = requestUserName;
					m_userLanguage = userLanguage;
					m_subReportCallback = subReportCallback;
					m_isOnePass = (report?.MergeOnePass ?? false);
					m_createReportChunkCallback = createReportChunkCallback;
					m_chunkManager = new ChunkManager.ProcessingChunkManager(createReportChunkCallback, m_isOnePass);
					m_quickFind = new QuickFindHashtable();
					m_drillthroughInfo = new ReportDrillthroughInfo();
					m_getResourceCallback = getResourceCallback;
					m_idCounterForSubreports = (report?.LastID ?? 0);
					m_interactiveExecution = interactiveExecution;
					m_hasImageStreams = false;
					m_executionTime = executionTime;
					m_allowUserProfileState = allowUserProfileState;
					m_isHistorySnapshot = isHistorySnapshot;
					m_snapshotProcessing = snapshotProcessing;
					m_processWithCachedData = processWithCachedData;
					m_getReportChunkCallback = getChunkCallback;
					m_cacheDataCallback = cacheDataCallback;
					if (cacheDataCallback != null)
					{
						m_dataCached = true;
					}
					m_cachedDataChunkMapping = new Hashtable();
					m_criControls = new CustomReportItemControls();
					m_reportRuntimeSetup = reportRuntimeSetup;
				}

				internal CommonInfo(IGetResource getResourceCallback, UserProfileState allowUserProfileState, ReportRuntimeSetup reportRuntimeSetup)
				{
					m_uniqueNameCounter = 0;
					m_dataProcessingDurationMs = 0L;
					m_owcChartName = null;
					m_owcChartInstance = null;
					m_requestUserName = null;
					m_userLanguage = null;
					m_subReportCallback = null;
					m_chunkManager = null;
					m_createReportChunkCallback = null;
					m_quickFind = new QuickFindHashtable();
					m_drillthroughInfo = new ReportDrillthroughInfo();
					m_getResourceCallback = getResourceCallback;
					m_idCounterForSubreports = 0;
					m_interactiveExecution = ExecutionType.Live;
					m_hasImageStreams = false;
					m_executionTime = DateTime.MinValue;
					m_allowUserProfileState = allowUserProfileState;
					m_snapshotProcessing = false;
					m_processWithCachedData = false;
					m_getReportChunkCallback = null;
					m_cacheDataCallback = null;
					m_dataCached = false;
					m_cachedDataChunkMapping = null;
					m_criControls = new CustomReportItemControls();
					m_reportRuntimeSetup = reportRuntimeSetup;
				}

				internal CommonInfo(IGetResource getResourceCallback, UserProfileState allowUserProfileState, ReportRuntimeSetup reportRuntimeSetup, CreateReportChunk createChunkCallback, ChunkManager.ProcessingChunkManager processingChunkManager, int uniqueNameCounter, ref ReportDrillthroughInfo drillthroughInfo)
				{
					m_uniqueNameCounter = uniqueNameCounter;
					m_dataProcessingDurationMs = 0L;
					m_owcChartName = null;
					m_owcChartInstance = null;
					m_requestUserName = null;
					m_userLanguage = null;
					m_subReportCallback = null;
					m_chunkManager = processingChunkManager;
					m_createReportChunkCallback = createChunkCallback;
					m_quickFind = new QuickFindHashtable();
					m_getResourceCallback = getResourceCallback;
					m_idCounterForSubreports = 0;
					m_interactiveExecution = ExecutionType.Live;
					m_hasImageStreams = false;
					m_executionTime = DateTime.MinValue;
					m_allowUserProfileState = allowUserProfileState;
					m_snapshotProcessing = false;
					m_processWithCachedData = false;
					m_getReportChunkCallback = null;
					m_cacheDataCallback = null;
					m_dataCached = false;
					m_cachedDataChunkMapping = null;
					m_criControls = new CustomReportItemControls();
					m_reportRuntimeSetup = reportRuntimeSetup;
					if (drillthroughInfo == null)
					{
						drillthroughInfo = new ReportDrillthroughInfo();
					}
					m_drillthroughInfo = drillthroughInfo;
				}

				internal int CreateUniqueName()
				{
					if (m_isOnePass)
					{
						return Interlocked.Increment(ref m_uniqueNameCounter);
					}
					return ++m_uniqueNameCounter;
				}

				internal int CreateIDForSubreport()
				{
					if (m_isOnePass)
					{
						return Interlocked.Increment(ref m_idCounterForSubreports);
					}
					return ++m_idCounterForSubreports;
				}

				internal int GetLastIDForReport()
				{
					return m_idCounterForSubreports;
				}

				internal EventInformation GetUserSortFilterInformation(ref int sourceUniqueName, ref int page)
				{
					if (m_reportRuntimeUserSortFilterInfo == null || m_reportRuntimeUserSortFilterInfo.Count == 0)
					{
						return null;
					}
					EventInformation.SortEventInfo sortEventInfo = new EventInformation.SortEventInfo();
					for (int i = 0; i < m_reportRuntimeUserSortFilterInfo.Count; i++)
					{
						RuntimeSortFilterEventInfo runtimeSortFilterEventInfo = m_reportRuntimeUserSortFilterInfo[i];
						if (-1 == runtimeSortFilterEventInfo.NewUniqueName)
						{
							runtimeSortFilterEventInfo.NewUniqueName = runtimeSortFilterEventInfo.OldUniqueName;
						}
						Hashtable hashtable = null;
						if (runtimeSortFilterEventInfo.PeerSortFilters != null)
						{
							hashtable = new Hashtable(runtimeSortFilterEventInfo.PeerSortFilters.Count);
							IDictionaryEnumerator enumerator = runtimeSortFilterEventInfo.PeerSortFilters.GetEnumerator();
							while (enumerator.MoveNext())
							{
								if (enumerator.Value != null)
								{
									hashtable.Add(enumerator.Value, null);
								}
							}
						}
						sortEventInfo.Add(runtimeSortFilterEventInfo.NewUniqueName, runtimeSortFilterEventInfo.SortDirection, hashtable);
						if (runtimeSortFilterEventInfo.OldUniqueName == sourceUniqueName)
						{
							sourceUniqueName = runtimeSortFilterEventInfo.NewUniqueName;
							page = runtimeSortFilterEventInfo.Page + 1;
						}
					}
					return new EventInformation
					{
						SortInfo = sortEventInfo
					};
				}
			}

			private sealed class ShowHideInfo
			{
				private sealed class SenderInfo
				{
					internal int UniqueName;

					internal bool StartHidden;

					internal int[] Containers;

					internal SenderInfo(int uniqueName, bool startHidden, int[] containers)
					{
						UniqueName = uniqueName;
						StartHidden = startHidden;
						Containers = containers;
					}
				}

				private sealed class ReceiverInfo
				{
					internal int UniqueName;

					internal bool StartHidden;

					internal ReceiverInfo(int uniqueName, bool startHidden)
					{
						UniqueName = uniqueName;
						StartHidden = startHidden;
					}
				}

				private sealed class ReceiverInfoList : ArrayList
				{
					internal new ReceiverInfo this[int index] => (ReceiverInfo)base[index];
				}

				private sealed class IgnoreRange
				{
					private int m_startIgnoreRangeIndex;

					private int m_endIgnoreRangeIndex;

					private bool m_useAllContainers = true;

					private bool m_ignoreFromStart;

					internal bool IgnoreAllFromStart
					{
						set
						{
							m_ignoreFromStart = value;
						}
					}

					internal bool UseAllContainers
					{
						set
						{
							m_useAllContainers = value;
						}
					}

					internal int EndIgnoreRange
					{
						set
						{
							m_endIgnoreRangeIndex = value;
							m_useAllContainers = false;
							m_ignoreFromStart = false;
						}
					}

					internal IgnoreRange(int startIgnoreRange)
					{
						m_startIgnoreRangeIndex = startIgnoreRange;
					}

					internal int[] GetContainerUniqueNames(IntList containerUniqueNames)
					{
						if (containerUniqueNames.Count == 0)
						{
							return null;
						}
						int[] array = null;
						if (!m_useAllContainers)
						{
							int count = containerUniqueNames.Count;
							int num = m_endIgnoreRangeIndex;
							if (m_ignoreFromStart)
							{
								num = count - 1;
							}
							if (num >= m_startIgnoreRangeIndex)
							{
								count -= m_endIgnoreRangeIndex - m_startIgnoreRangeIndex + 1;
								if (count == 0)
								{
									return null;
								}
								array = new int[count];
								containerUniqueNames.CopyTo(0, array, 0, m_startIgnoreRangeIndex);
								containerUniqueNames.CopyTo(num + 1, array, m_startIgnoreRangeIndex, containerUniqueNames.Count - num - 1);
								return array;
							}
						}
						array = new int[containerUniqueNames.Count];
						containerUniqueNames.CopyTo(array);
						return array;
					}
				}

				private sealed class IgnoreRangeList : ArrayList
				{
					internal new IgnoreRange this[int index] => (IgnoreRange)base[index];
				}

				private sealed class CommonInfo
				{
					private SenderInformationHashtable m_senderInformation;

					private ReceiverInformationHashtable m_receiverInformation;

					internal void UpdateSenderAndReceiverInfo(SenderInfo sender, ReceiverInfo receiver)
					{
						SenderInformation senderInformation = null;
						if (m_senderInformation == null)
						{
							m_senderInformation = new SenderInformationHashtable();
						}
						else
						{
							senderInformation = m_senderInformation[sender.UniqueName];
						}
						if (senderInformation == null)
						{
							senderInformation = new SenderInformation(sender.StartHidden, sender.Containers);
							m_senderInformation[sender.UniqueName] = senderInformation;
						}
						senderInformation.ReceiverUniqueNames.Add(receiver.UniqueName);
						if (m_receiverInformation == null)
						{
							m_receiverInformation = new ReceiverInformationHashtable();
						}
						m_receiverInformation[receiver.UniqueName] = new ReceiverInformation(receiver.StartHidden, sender.UniqueName);
					}

					internal void GetSenderAndReceiverInfo(out SenderInformationHashtable senderInformation, out ReceiverInformationHashtable receiverInformation)
					{
						senderInformation = m_senderInformation;
						receiverInformation = m_receiverInformation;
						m_senderInformation = null;
						m_receiverInformation = null;
					}
				}

				private CommonInfo m_commonInfo;

				private ArrayList m_recursiveSenders;

				private ArrayList m_localSenders;

				private Hashtable m_localReceivers;

				private IntList m_containerUniqueNames;

				private IgnoreRangeList m_ignoreRangeList;

				private IgnoreRange m_currentIgnoreRange;

				internal bool IgnoreAllFromStart
				{
					set
					{
						m_currentIgnoreRange.IgnoreAllFromStart = value;
					}
				}

				internal bool UseAllContainers
				{
					set
					{
						m_currentIgnoreRange.UseAllContainers = value;
					}
				}

				internal ShowHideInfo()
				{
					m_commonInfo = new CommonInfo();
					m_recursiveSenders = new ArrayList();
					m_localSenders = new ArrayList();
					m_localSenders.Add(null);
					m_localReceivers = null;
					m_containerUniqueNames = new IntList();
				}

				internal ShowHideInfo(ShowHideInfo copy)
				{
					m_commonInfo = copy.m_commonInfo;
					m_recursiveSenders = new ArrayList();
					m_localSenders = new ArrayList();
					m_localSenders.Add(null);
					m_localReceivers = null;
					m_containerUniqueNames = new IntList();
				}

				internal void EnterGrouping()
				{
					m_localSenders.Add(null);
				}

				internal void ExitGrouping()
				{
					m_localSenders.RemoveAt(m_localSenders.Count - 1);
				}

				internal void EnterChildGroupings()
				{
					m_recursiveSenders.Add(null);
				}

				internal void ExitChildGroupings()
				{
					m_recursiveSenders.RemoveAt(m_recursiveSenders.Count - 1);
				}

				internal void RegisterSender(string senderName, int senderUniqueName, bool startHidden, bool recursiveSender)
				{
					int[] containerUniqueNames = GetContainerUniqueNames();
					SenderInfo sender = new SenderInfo(senderUniqueName, startHidden, containerUniqueNames);
					ReceiverInfoList receiverInfoList = RemoveLocalReceivers(senderName);
					if (receiverInfoList != null)
					{
						for (int i = 0; i < receiverInfoList.Count; i++)
						{
							m_commonInfo.UpdateSenderAndReceiverInfo(sender, receiverInfoList[i]);
						}
					}
					AddLocalSender(senderName, sender);
					if (recursiveSender)
					{
						AddRecursiveSender(senderName, sender);
					}
				}

				internal void RegisterReceiver(string senderName, int receiverUniqueName, bool startHidden, bool recursiveReceiver)
				{
					ReceiverInfo receiver = new ReceiverInfo(receiverUniqueName, startHidden);
					if (!recursiveReceiver)
					{
						SenderInfo senderInfo = FindLocalSender(senderName);
						if (senderInfo != null)
						{
							m_commonInfo.UpdateSenderAndReceiverInfo(senderInfo, receiver);
						}
						else
						{
							AddLocalReceiver(senderName, receiver);
						}
					}
					else
					{
						SenderInfo senderInfo2 = FindRecursiveSender(senderName);
						if (senderInfo2 != null)
						{
							m_commonInfo.UpdateSenderAndReceiverInfo(senderInfo2, receiver);
						}
					}
				}

				internal void RegisterContainer(int containerUniqueName)
				{
					m_containerUniqueNames.Add(containerUniqueName);
				}

				internal void UnRegisterContainer(int containerUniqueName)
				{
					Global.Tracer.Assert(containerUniqueName == m_containerUniqueNames[m_containerUniqueNames.Count - 1]);
					m_containerUniqueNames.RemoveAt(m_containerUniqueNames.Count - 1);
				}

				internal void GetSenderAndReceiverInfo(out SenderInformationHashtable senderInfo, out ReceiverInformationHashtable receiverInfo)
				{
					m_commonInfo.GetSenderAndReceiverInfo(out senderInfo, out receiverInfo);
					m_commonInfo = null;
					m_recursiveSenders = null;
					m_localSenders = null;
					m_localReceivers = null;
					m_containerUniqueNames = null;
				}

				private void AddRecursiveSender(string senderName, SenderInfo sender)
				{
					Hashtable hashtable = (Hashtable)m_recursiveSenders[m_recursiveSenders.Count - 1];
					if (hashtable == null)
					{
						hashtable = new Hashtable();
						m_recursiveSenders[m_recursiveSenders.Count - 1] = hashtable;
					}
					hashtable[senderName] = sender;
				}

				private SenderInfo FindRecursiveSender(string senderName)
				{
					if (m_recursiveSenders.Count - 2 >= 0)
					{
						Hashtable hashtable = (Hashtable)m_recursiveSenders[m_recursiveSenders.Count - 2];
						if (hashtable != null && hashtable.ContainsKey(senderName))
						{
							return (SenderInfo)hashtable[senderName];
						}
					}
					return null;
				}

				private void AddLocalSender(string senderName, SenderInfo sender)
				{
					Hashtable hashtable = (Hashtable)m_localSenders[m_localSenders.Count - 1];
					if (hashtable == null)
					{
						hashtable = new Hashtable();
						m_localSenders[m_localSenders.Count - 1] = hashtable;
					}
					hashtable[senderName] = sender;
				}

				private SenderInfo FindLocalSender(string senderName)
				{
					for (int num = m_localSenders.Count - 1; num >= 0; num--)
					{
						Hashtable hashtable = (Hashtable)m_localSenders[num];
						if (hashtable != null && hashtable.ContainsKey(senderName))
						{
							return (SenderInfo)hashtable[senderName];
						}
					}
					return null;
				}

				private void AddLocalReceiver(string senderName, ReceiverInfo receiver)
				{
					if (m_localReceivers == null)
					{
						m_localReceivers = new Hashtable();
					}
					ReceiverInfoList receiverInfoList = (ReceiverInfoList)m_localReceivers[senderName];
					if (receiverInfoList == null)
					{
						receiverInfoList = new ReceiverInfoList();
						m_localReceivers[senderName] = receiverInfoList;
					}
					receiverInfoList.Add(receiver);
				}

				private ReceiverInfoList RemoveLocalReceivers(string senderName)
				{
					if (m_localReceivers != null)
					{
						ReceiverInfoList receiverInfoList = (ReceiverInfoList)m_localReceivers[senderName];
						if (receiverInfoList != null)
						{
							m_localReceivers.Remove(senderName);
							return receiverInfoList;
						}
					}
					return null;
				}

				private int[] GetContainerUniqueNames()
				{
					if (m_containerUniqueNames.Count == 0)
					{
						return null;
					}
					int[] array = null;
					if (m_currentIgnoreRange != null)
					{
						array = m_currentIgnoreRange.GetContainerUniqueNames(m_containerUniqueNames);
					}
					else
					{
						array = new int[m_containerUniqueNames.Count];
						m_containerUniqueNames.CopyTo(array);
					}
					return array;
				}

				internal void EndIgnoreRange()
				{
					m_currentIgnoreRange.EndIgnoreRange = m_containerUniqueNames.Count - 1;
				}

				internal void RegisterIgnoreRange()
				{
					if (m_ignoreRangeList == null)
					{
						m_ignoreRangeList = new IgnoreRangeList();
					}
					m_currentIgnoreRange = new IgnoreRange(m_containerUniqueNames.Count);
					m_ignoreRangeList.Add(m_currentIgnoreRange);
				}

				internal void UnRegisterIgnoreRange()
				{
					m_ignoreRangeList.RemoveAt(m_ignoreRangeList.Count - 1);
					if (m_ignoreRangeList.Count > 0)
					{
						m_currentIgnoreRange = m_ignoreRangeList[m_ignoreRangeList.Count - 1];
					}
					else
					{
						m_currentIgnoreRange = null;
					}
				}
			}

			private AbortHelper m_abortHelper;

			private CommonInfo m_commonInfo;

			private uint m_subReportLevel;

			private ICatalogItemContext m_reportContext;

			private ObjectModelImpl m_reportObjectModel;

			private bool m_reportItemsReferenced;

			private bool m_reportItemThisDotValueReferenced;

			private ShowHideInfo m_showHideInfo;

			private DataSourceInfoHashtable m_dataSourceInfo;

			private Report.ShowHideTypes m_showHideType;

			private EmbeddedImageHashtable m_embeddedImages;

			private ImageStreamNames m_imageStreamNames;

			private bool m_inPageSection;

			private FiltersList m_specialDataRegionFilters;

			private ErrorContext m_errorContext;

			private bool m_processReportParameters;

			private List<bool> m_pivotRunningValueScopes;

			private ReportRuntime m_reportRuntime;

			private MatrixHeadingInstance m_headingInstance;

			private MatrixHeadingInstance m_headingInstanceOld;

			private bool m_delayAddingInstanceInfo;

			private bool m_specialRecursiveAggregates;

			private SecondPassOperations m_secondPassOperation;

			private CultureInfo m_threadCulture;

			private uint m_languageInstanceId;

			private AggregatesImpl m_globalRVCollection;

			private string m_transparentImageGuid;

			private Pagination m_pagination;

			private NavigationInfo m_navigationInfo;

			private CompareInfo m_compareInfo = Thread.CurrentThread.CurrentCulture.CompareInfo;

			private CompareOptions m_clrCompareOptions;

			private int m_dataSetUniqueName = -1;

			private bool m_createPageSectionImageChunks;

			private PageSectionContext m_pageSectionContext;

			private UserSortFilterContext m_userSortFilterContext;

			private IJobContext m_jobContext;

			private IExtensionFactory m_extFactory;

			private IProcessingDataExtensionConnection m_dataExtensionConnection;

			private IDataProtection m_dataProtection;

			public bool EnableDataBackedParameters => true;

			internal CreateReportChunk CreateReportChunkCallback => m_commonInfo.CreateReportChunkCallback;

			internal IChunkFactory CreateReportChunkFactory
			{
				get
				{
					return m_commonInfo.CreateReportChunkFactory;
				}
				set
				{
					m_commonInfo.CreateReportChunkFactory = value;
				}
			}

			internal bool IsOnePass => m_commonInfo.IsOnePass;

			internal long DataProcessingDurationMs
			{
				get
				{
					return m_commonInfo.DataProcessingDurationMs;
				}
				set
				{
					m_commonInfo.DataProcessingDurationMs = value;
				}
			}

			internal string OWCChartName => m_commonInfo.OWCChartName;

			internal OWCChartInstanceInfo OWCChartInstance
			{
				get
				{
					return m_commonInfo.OWCChartInstance;
				}
				set
				{
					m_commonInfo.OWCChartInstance = value;
				}
			}

			internal string RequestUserName => m_commonInfo.RequestUserName;

			public DateTime ExecutionTime => m_commonInfo.ExecutionTime;

			internal CultureInfo UserLanguage => m_commonInfo.UserLanguage;

			internal SubReportCallback SubReportCallback => m_commonInfo.SubReportCallback;

			internal UserProfileState HasUserProfileState
			{
				get
				{
					if (m_reportObjectModel != null && m_reportObjectModel.UserImpl != null)
					{
						return m_reportObjectModel.UserImpl.HasUserProfileState;
					}
					return UserProfileState.None;
				}
			}

			internal ChunkManager.ProcessingChunkManager ChunkManager => m_commonInfo.ChunkManager;

			internal QuickFindHashtable QuickFind => m_commonInfo.QuickFind;

			internal ReportDrillthroughInfo DrillthroughInfo
			{
				get
				{
					return m_commonInfo.DrillthroughInfo;
				}
				set
				{
					m_commonInfo.DrillthroughInfo = value;
				}
			}

			internal int UniqueNameCounter => m_commonInfo.UniqueNameCounter;

			internal ExecutionType InteractiveExecution => m_commonInfo.InteractiveExecution;

			internal bool HasImageStreams
			{
				get
				{
					return m_commonInfo.HasImageStreams;
				}
				set
				{
					m_commonInfo.HasImageStreams = value;
				}
			}

			internal UserProfileState AllowUserProfileState => m_commonInfo.AllowUserProfileState;

			internal bool HasUserSortFilter
			{
				get
				{
					return m_commonInfo.HasUserSortFilter;
				}
				set
				{
					m_commonInfo.HasUserSortFilter = value;
				}
			}

			internal bool SaveSnapshotData
			{
				get
				{
					return m_commonInfo.SaveSnapshotData;
				}
				set
				{
					m_commonInfo.SaveSnapshotData = value;
				}
			}

			internal bool StopSaveSnapshotDataOnError
			{
				get
				{
					return m_commonInfo.StopSaveSnapshotDataOnError;
				}
				set
				{
					m_commonInfo.StopSaveSnapshotDataOnError = value;
				}
			}

			internal bool ErrorSavingSnapshotData
			{
				get
				{
					return m_commonInfo.ErrorSavingSnapshotData;
				}
				set
				{
					m_commonInfo.ErrorSavingSnapshotData = value;
				}
			}

			internal bool IsHistorySnapshot => m_commonInfo.IsHistorySnapshot;

			public bool SnapshotProcessing
			{
				get
				{
					return m_commonInfo.SnapshotProcessing;
				}
				set
				{
					m_commonInfo.SnapshotProcessing = value;
				}
			}

			internal bool UserSortFilterProcessing
			{
				get
				{
					return m_commonInfo.UserSortFilterProcessing;
				}
				set
				{
					m_commonInfo.UserSortFilterProcessing = value;
				}
			}

			internal bool ResetForSubreportDataPrefetch
			{
				get
				{
					return m_commonInfo.ResetForSubreportDataPrefetch;
				}
				set
				{
					m_commonInfo.ResetForSubreportDataPrefetch = value;
				}
			}

			internal bool ProcessWithCachedData => m_commonInfo.ProcessWithCachedData;

			internal GetReportChunk GetReportChunkCallback => m_commonInfo.GetReportChunkCallback;

			internal CreateReportChunk CacheDataCallback => m_commonInfo.CacheDataCallback;

			internal bool DataCached
			{
				get
				{
					return m_commonInfo.DataCached;
				}
				set
				{
					m_commonInfo.DataCached = value;
				}
			}

			internal Hashtable CachedDataChunkMapping
			{
				get
				{
					return m_commonInfo.CachedDataChunkMapping;
				}
				set
				{
					m_commonInfo.CachedDataChunkMapping = value;
				}
			}

			internal CustomReportItemControls CriProcessingControls
			{
				get
				{
					return m_commonInfo.CriProcessingControls;
				}
				set
				{
					m_commonInfo.CriProcessingControls = value;
				}
			}

			internal EventInformation UserSortFilterInfo
			{
				get
				{
					return m_commonInfo.UserSortFilterInfo;
				}
				set
				{
					m_commonInfo.UserSortFilterInfo = value;
				}
			}

			internal SortFilterEventInfoHashtable OldSortFilterEventInfo
			{
				get
				{
					return m_commonInfo.OldSortFilterEventInfo;
				}
				set
				{
					m_commonInfo.OldSortFilterEventInfo = value;
				}
			}

			internal SortFilterEventInfoHashtable NewSortFilterEventInfo
			{
				get
				{
					return m_commonInfo.NewSortFilterEventInfo;
				}
				set
				{
					m_commonInfo.NewSortFilterEventInfo = value;
				}
			}

			internal RuntimeSortFilterEventInfoList ReportRuntimeUserSortFilterInfo
			{
				get
				{
					return m_commonInfo.ReportRuntimeUserSortFilterInfo;
				}
				set
				{
					m_commonInfo.ReportRuntimeUserSortFilterInfo = value;
				}
			}

			internal ReportRuntimeSetup ReportRuntimeSetup => m_commonInfo.ReportRuntimeSetup;

			internal CultureInfo ThreadCulture
			{
				get
				{
					return m_threadCulture;
				}
				set
				{
					m_threadCulture = value;
				}
			}

			internal uint LanguageInstanceId
			{
				get
				{
					return m_languageInstanceId;
				}
				set
				{
					m_languageInstanceId = value;
				}
			}

			internal uint SubReportLevel => m_subReportLevel;

			internal ICatalogItemContext ReportContext => m_reportContext;

			internal ReportRuntime ReportRuntime
			{
				get
				{
					return m_reportRuntime;
				}
				set
				{
					m_reportRuntime = value;
				}
			}

			internal ObjectModelImpl ReportObjectModel
			{
				get
				{
					return m_reportObjectModel;
				}
				set
				{
					m_reportObjectModel = value;
				}
			}

			internal bool ReportItemsReferenced => m_reportItemsReferenced;

			internal bool ReportItemThisDotValueReferenced => m_reportItemThisDotValueReferenced;

			internal DataSourceInfoHashtable GlobalDataSourceInfo => m_dataSourceInfo;

			internal Report.ShowHideTypes ShowHideType => m_showHideType;

			internal EmbeddedImageHashtable EmbeddedImages => m_embeddedImages;

			internal ImageStreamNames ImageStreamNames
			{
				get
				{
					return m_imageStreamNames;
				}
				set
				{
					m_imageStreamNames = value;
				}
			}

			internal bool InPageSection => m_inPageSection;

			internal AbortHelper AbortInfo
			{
				get
				{
					return m_abortHelper;
				}
				set
				{
					m_abortHelper = value;
				}
			}

			public ErrorContext ErrorContext => m_errorContext;

			internal bool ProcessReportParameters => m_processReportParameters;

			internal MatrixHeadingInstance HeadingInstance
			{
				get
				{
					return m_headingInstance;
				}
				set
				{
					m_headingInstance = value;
				}
			}

			internal MatrixHeadingInstance HeadingInstanceOld
			{
				get
				{
					return m_headingInstanceOld;
				}
				set
				{
					m_headingInstanceOld = value;
				}
			}

			internal bool DelayAddingInstanceInfo
			{
				get
				{
					return m_delayAddingInstanceInfo;
				}
				set
				{
					m_delayAddingInstanceInfo = value;
				}
			}

			internal bool SpecialRecursiveAggregates => m_specialRecursiveAggregates;

			internal SecondPassOperations SecondPassOperation
			{
				get
				{
					return m_secondPassOperation;
				}
				set
				{
					m_secondPassOperation = value;
				}
			}

			internal AggregatesImpl GlobalRVCollection
			{
				get
				{
					return m_globalRVCollection;
				}
				set
				{
					m_globalRVCollection = value;
				}
			}

			internal string TransparentImageGuid
			{
				get
				{
					return m_transparentImageGuid;
				}
				set
				{
					m_transparentImageGuid = value;
				}
			}

			internal Pagination Pagination
			{
				get
				{
					return m_pagination;
				}
				set
				{
					m_pagination = value;
				}
			}

			internal NavigationInfo NavigationInfo
			{
				get
				{
					return m_navigationInfo;
				}
				set
				{
					m_navigationInfo = value;
				}
			}

			internal CompareInfo CompareInfo
			{
				get
				{
					return m_compareInfo;
				}
				set
				{
					m_compareInfo = value;
				}
			}

			internal CompareOptions ClrCompareOptions
			{
				get
				{
					return m_clrCompareOptions;
				}
				set
				{
					m_clrCompareOptions = value;
				}
			}

			internal int DataSetUniqueName => m_dataSetUniqueName;

			internal bool CreatePageSectionImageChunks => m_createPageSectionImageChunks;

			internal PageSectionContext PageSectionContext
			{
				get
				{
					return m_pageSectionContext;
				}
				set
				{
					m_pageSectionContext = value;
				}
			}

			internal UserSortFilterContext UserSortFilterContext
			{
				get
				{
					return m_userSortFilterContext;
				}
				set
				{
					m_userSortFilterContext = value;
				}
			}

			internal RuntimeSortFilterEventInfoList RuntimeSortFilterInfo
			{
				get
				{
					return m_userSortFilterContext.RuntimeSortFilterInfo;
				}
				set
				{
					m_userSortFilterContext.RuntimeSortFilterInfo = value;
				}
			}

			internal IJobContext JobContext => m_jobContext;

			internal IExtensionFactory ExtFactory => m_extFactory;

			internal IProcessingDataExtensionConnection DataExtensionConnection => m_dataExtensionConnection;

			internal IDataProtection DataProtection => m_dataProtection;

			internal bool IgnoreAllFromStart
			{
				set
				{
					Global.Tracer.Assert(m_showHideInfo != null, "(null != m_showHideInfo)");
					m_showHideInfo.IgnoreAllFromStart = value;
				}
			}

			internal bool UseAllContainers
			{
				set
				{
					Global.Tracer.Assert(m_showHideInfo != null, "(null != m_showHideInfo)");
					m_showHideInfo.UseAllContainers = value;
				}
			}

			protected ProcessingContext(string chartName, string requestUserName, CultureInfo userLanguage, SubReportCallback subReportCallback, ICatalogItemContext reportContext, Report report, ErrorContext errorContext, CreateReportChunk createReportChunkCallback, IGetResource getResourceCallback, ExecutionType interactiveExecution, DateTime executionTime, UserProfileState allowUserProfileState, bool isHistorySnapshot, bool snapshotProcessing, bool processWithCachedData, GetReportChunk getChunkCallback, CreateReportChunk cacheDataCallback, ReportRuntimeSetup reportRuntimeSetup, IJobContext jobContext, IExtensionFactory extFactory, IProcessingDataExtensionConnection dataExtensionConnection, IDataProtection dataProtection)
			{
				m_commonInfo = new CommonInfo(chartName, requestUserName, userLanguage, subReportCallback, report, createReportChunkCallback, getResourceCallback, interactiveExecution, executionTime, allowUserProfileState, isHistorySnapshot, snapshotProcessing, processWithCachedData, getChunkCallback, cacheDataCallback, reportRuntimeSetup);
				m_subReportLevel = 0u;
				m_reportContext = reportContext;
				m_reportObjectModel = null;
				m_reportItemsReferenced = report.HasReportItemReferences;
				m_reportItemThisDotValueReferenced = false;
				m_showHideInfo = new ShowHideInfo();
				m_dataSourceInfo = new DataSourceInfoHashtable();
				m_showHideType = report.ShowHideType;
				m_embeddedImages = report.EmbeddedImages;
				m_imageStreamNames = report.ImageStreamNames;
				m_abortHelper = new AbortHelper(jobContext);
				m_inPageSection = false;
				m_specialDataRegionFilters = null;
				m_errorContext = errorContext;
				m_processReportParameters = false;
				m_pivotRunningValueScopes = null;
				m_reportRuntime = null;
				m_delayAddingInstanceInfo = false;
				m_specialRecursiveAggregates = report.HasSpecialRecursiveAggregates;
				m_pagination = new Pagination(report.InteractiveHeightValue);
				m_navigationInfo = new NavigationInfo();
				m_pageSectionContext = new PageSectionContext(report.PageHeaderEvaluation || report.PageFooterEvaluation, report.MergeOnePass);
				m_userSortFilterContext = new UserSortFilterContext();
				m_jobContext = jobContext;
				m_extFactory = extFactory;
				m_dataExtensionConnection = dataExtensionConnection;
				m_dataProtection = dataProtection;
			}

			protected ProcessingContext(ProcessingContext parentContext, string requestUserName, CultureInfo userlanguage, ICatalogItemContext reportContext, ErrorContext errorContext, ExecutionType interactiveExecution, DateTime executionTime, UserProfileState allowUserProfileState, bool snapshotProcessing, ReportRuntimeSetup reportRuntimeSetup, IJobContext jobContext, IExtensionFactory extFactory, IProcessingDataExtensionConnection dataExtensionConnection, IDataProtection dataProtection)
			{
				m_commonInfo = new CommonInfo(null, requestUserName, userlanguage, null, null, null, null, interactiveExecution, executionTime, allowUserProfileState, isHistorySnapshot: false, snapshotProcessing, processWithCachedData: false, null, null, reportRuntimeSetup);
				m_subReportLevel = 0u;
				m_reportContext = reportContext;
				m_reportObjectModel = null;
				m_reportItemsReferenced = false;
				m_reportItemThisDotValueReferenced = false;
				m_showHideInfo = new ShowHideInfo();
				m_dataSourceInfo = new DataSourceInfoHashtable();
				m_showHideType = Report.ShowHideTypes.None;
				m_embeddedImages = null;
				m_imageStreamNames = null;
				if (parentContext != null)
				{
					m_abortHelper = parentContext.AbortInfo;
				}
				else
				{
					m_abortHelper = new AbortHelper(jobContext);
				}
				m_inPageSection = false;
				m_specialDataRegionFilters = null;
				m_errorContext = errorContext;
				m_processReportParameters = true;
				m_pivotRunningValueScopes = null;
				m_reportRuntime = null;
				m_delayAddingInstanceInfo = false;
				m_specialRecursiveAggregates = false;
				m_pagination = new Pagination(0.0);
				m_navigationInfo = new NavigationInfo();
				m_userSortFilterContext = new UserSortFilterContext();
				m_jobContext = jobContext;
				m_extFactory = extFactory;
				m_dataExtensionConnection = dataExtensionConnection;
				m_dataProtection = dataProtection;
			}

			protected ProcessingContext(SubReport subReport, ErrorContext errorContext, ProcessingContext copy, int subReportDataSetUniqueName)
			{
				m_commonInfo = copy.m_commonInfo;
				m_subReportLevel = copy.m_subReportLevel + 1;
				m_threadCulture = copy.ThreadCulture;
				m_languageInstanceId = copy.m_languageInstanceId;
				m_reportContext = subReport.ReportContext;
				m_reportObjectModel = null;
				m_reportItemsReferenced = subReport.Report.HasReportItemReferences;
				m_reportItemThisDotValueReferenced = false;
				m_showHideInfo = new ShowHideInfo(copy.m_showHideInfo);
				m_dataSourceInfo = copy.m_dataSourceInfo;
				m_showHideType = subReport.Report.ShowHideType;
				m_embeddedImages = subReport.Report.EmbeddedImages;
				m_imageStreamNames = subReport.Report.ImageStreamNames;
				m_abortHelper = copy.AbortInfo;
				m_inPageSection = false;
				m_specialDataRegionFilters = null;
				m_errorContext = errorContext;
				m_processReportParameters = false;
				m_pivotRunningValueScopes = null;
				m_reportRuntime = null;
				m_delayAddingInstanceInfo = false;
				m_specialRecursiveAggregates = subReport.Report.HasSpecialRecursiveAggregates;
				m_pagination = copy.m_pagination;
				m_dataSetUniqueName = subReportDataSetUniqueName;
				m_navigationInfo = copy.m_navigationInfo;
				m_pageSectionContext = copy.PageSectionContext;
				m_userSortFilterContext = new UserSortFilterContext(copy.UserSortFilterContext, subReport);
				m_jobContext = copy.JobContext;
				m_extFactory = copy.m_extFactory;
				m_dataExtensionConnection = copy.DataExtensionConnection;
				m_dataProtection = copy.DataProtection;
			}

			internal ProcessingContext(ICatalogItemContext reportContext, Report.ShowHideTypes showHideType, IGetResource getResourceCallback, EmbeddedImageHashtable embeddedImages, ImageStreamNames imageStreamNames, ErrorContext errorContext, bool reportItemsReferenced, UserProfileState allowUserProfileState, ReportRuntimeSetup reportRuntimeSetup, IDataProtection dataProtection)
			{
				m_commonInfo = new CommonInfo(getResourceCallback, allowUserProfileState, reportRuntimeSetup);
				m_subReportLevel = 0u;
				m_reportContext = reportContext;
				m_reportObjectModel = null;
				m_reportItemsReferenced = reportItemsReferenced;
				m_reportItemThisDotValueReferenced = false;
				m_showHideInfo = null;
				m_dataSourceInfo = null;
				m_showHideType = showHideType;
				m_embeddedImages = embeddedImages;
				m_imageStreamNames = imageStreamNames;
				m_abortHelper = null;
				m_inPageSection = true;
				m_specialDataRegionFilters = null;
				m_errorContext = errorContext;
				m_processReportParameters = false;
				m_pivotRunningValueScopes = null;
				m_reportRuntime = null;
				m_delayAddingInstanceInfo = false;
				m_specialRecursiveAggregates = false;
				m_pagination = new Pagination(0.0);
				m_navigationInfo = new NavigationInfo();
				m_pageSectionContext = new PageSectionContext(hasPageSections: true, isOnePass: false);
				m_userSortFilterContext = new UserSortFilterContext();
				m_dataProtection = dataProtection;
			}

			internal ProcessingContext(ICatalogItemContext reportContext, Report.ShowHideTypes showHideType, IGetResource getResourceCallback, EmbeddedImageHashtable embeddedImages, ImageStreamNames imageStreamNames, ErrorContext errorContext, bool reportItemsReferenced, UserProfileState allowUserProfileState, ReportRuntimeSetup reportRuntimeSetup, CreateReportChunk createChunkCallback, ChunkManager.ProcessingChunkManager processingChunkManager, int uniqueNameCounter, IDataProtection dataProtection, ref ReportDrillthroughInfo drillthroughInfo)
			{
				m_commonInfo = new CommonInfo(getResourceCallback, allowUserProfileState, reportRuntimeSetup, createChunkCallback, processingChunkManager, uniqueNameCounter, ref drillthroughInfo);
				m_inPageSection = true;
				m_createPageSectionImageChunks = true;
				m_subReportLevel = 0u;
				m_reportContext = reportContext;
				m_reportObjectModel = null;
				m_reportItemsReferenced = reportItemsReferenced;
				m_reportItemThisDotValueReferenced = false;
				m_showHideInfo = null;
				m_dataSourceInfo = null;
				m_showHideType = showHideType;
				m_embeddedImages = embeddedImages;
				m_imageStreamNames = imageStreamNames;
				m_abortHelper = null;
				m_inPageSection = true;
				m_specialDataRegionFilters = null;
				m_errorContext = errorContext;
				m_processReportParameters = false;
				m_pivotRunningValueScopes = null;
				m_reportRuntime = null;
				m_delayAddingInstanceInfo = false;
				m_specialRecursiveAggregates = false;
				m_pagination = new Pagination(0.0);
				m_navigationInfo = new NavigationInfo();
				m_userSortFilterContext = new UserSortFilterContext();
				m_dataProtection = dataProtection;
			}

			protected ProcessingContext(ProcessingContext copy)
			{
				m_commonInfo = copy.m_commonInfo;
				m_subReportLevel = copy.m_subReportLevel;
				m_reportContext = copy.m_reportContext;
				m_threadCulture = copy.m_threadCulture;
				m_reportObjectModel = new ObjectModelImpl(copy.ReportObjectModel, this);
				m_reportItemsReferenced = copy.ReportItemsReferenced;
				m_reportItemThisDotValueReferenced = false;
				m_showHideInfo = copy.m_showHideInfo;
				m_dataSourceInfo = copy.m_dataSourceInfo;
				m_showHideType = copy.m_showHideType;
				m_embeddedImages = copy.m_embeddedImages;
				m_imageStreamNames = copy.m_imageStreamNames;
				m_abortHelper = copy.m_abortHelper;
				m_inPageSection = copy.m_inPageSection;
				m_specialDataRegionFilters = null;
				m_errorContext = copy.m_errorContext;
				m_processReportParameters = copy.m_processReportParameters;
				m_pivotRunningValueScopes = null;
				m_reportRuntime = ((copy.ReportRuntime == null) ? null : new ReportRuntime(m_reportObjectModel, m_errorContext, copy.ReportRuntime.ReportExprHost, copy.ReportRuntime));
				m_delayAddingInstanceInfo = false;
				m_specialRecursiveAggregates = copy.m_specialRecursiveAggregates;
				m_pagination = copy.m_pagination;
				m_dataSetUniqueName = copy.DataSetUniqueName;
				m_navigationInfo = copy.m_navigationInfo;
				m_pageSectionContext = copy.PageSectionContext;
				m_userSortFilterContext = new UserSortFilterContext(copy.UserSortFilterContext);
				m_jobContext = copy.m_jobContext;
				m_extFactory = copy.m_extFactory;
				m_dataExtensionConnection = copy.m_dataExtensionConnection;
				m_dataProtection = copy.m_dataProtection;
			}

			internal virtual ProcessingContext ParametersContext(ICatalogItemContext reportContext, ProcessingErrorContext subReportErrorContext)
			{
				return null;
			}

			internal virtual ProcessingContext SubReportContext(SubReport subReport, int subReportDataSetUniqueName, ProcessingErrorContext subReportErrorContext)
			{
				return null;
			}

			internal virtual ProcessingContext CloneContext(ProcessingContext context)
			{
				return null;
			}

			internal void CheckAndThrowIfAborted()
			{
				if (m_abortHelper.GetStatus(DataSetUniqueName) != 0)
				{
					m_abortHelper.ThrowAbortException(DataSetUniqueName);
				}
			}

			internal void RuntimeInitializeReportItemObjs(ReportItemCollection reportItems, bool traverseDataRegions, bool setValue)
			{
				for (int i = 0; i < reportItems.Count; i++)
				{
					RuntimeInitializeReportItemObjs(reportItems[i], traverseDataRegions, setValue);
				}
			}

			internal void RuntimeInitializeReportItemObjs(ReportItem reportItem, bool traverseDataRegions, bool setValue)
			{
				ReportItemImpl reportItemImpl = null;
				if (reportItem == null)
				{
					return;
				}
				if (!(reportItem is DataRegion))
				{
					if (m_reportRuntime.ReportExprHost != null)
					{
						reportItem.SetExprHost(m_reportRuntime.ReportExprHost, m_reportObjectModel);
					}
					if (reportItem is TextBox)
					{
						TextBox textBox = (TextBox)reportItem;
						if (m_reportItemsReferenced || textBox.ValueReferenced)
						{
							TextBoxImpl textBoxImpl = new TextBoxImpl(textBox, m_reportRuntime, m_reportRuntime);
							if (setValue)
							{
								textBoxImpl.SetResult(default(VariantResult));
							}
							if (textBox.ValueReferenced)
							{
								Global.Tracer.Assert(textBox.ExprHost != null, "(textBoxDef.ExprHost != null)");
								m_reportItemThisDotValueReferenced = true;
								textBox.TextBoxExprHost.SetTextBox(textBoxImpl);
							}
							if (m_reportItemsReferenced)
							{
								reportItemImpl = textBoxImpl;
							}
						}
					}
					else if (reportItem is Rectangle)
					{
						RuntimeInitializeReportItemObjs(((Rectangle)reportItem).ReportItems, traverseDataRegions, setValue);
					}
				}
				else
				{
					if (reportItem is CustomReportItem && ((CustomReportItem)reportItem).DataSetName == null && m_reportRuntime.ReportExprHost != null)
					{
						reportItem.SetExprHost(m_reportRuntime.ReportExprHost, m_reportObjectModel);
					}
					if (traverseDataRegions)
					{
						if (m_reportRuntime.ReportExprHost != null)
						{
							reportItem.SetExprHost(m_reportRuntime.ReportExprHost, m_reportObjectModel);
						}
						if (reportItem is List)
						{
							RuntimeInitializeReportItemObjs(((List)reportItem).ReportItems, traverseDataRegions, setValue);
						}
						else if (reportItem is Matrix)
						{
							Matrix matrix = (Matrix)reportItem;
							RuntimeInitializeReportItemObjs(matrix.CornerReportItems, traverseDataRegions, setValue);
							RuntimeInitializeReportItemObjs(matrix.CellReportItems, traverseDataRegions, setValue);
							InitializeMatrixHeadingRuntimeObjs(matrix.Rows, (matrix.ExprHost != null) ? matrix.MatrixExprHost.RowGroupingsHost : null, traverseDataRegions, setValue);
							InitializeMatrixHeadingRuntimeObjs(matrix.Columns, (matrix.ExprHost != null) ? matrix.MatrixExprHost.ColumnGroupingsHost : null, traverseDataRegions, setValue);
						}
						else if (reportItem is Chart)
						{
							Chart chart = (Chart)reportItem;
							InitializeChartHeadingRuntimeObjs(chart.Rows, (chart.ExprHost != null) ? chart.ChartExprHost.RowGroupingsHost : null);
							InitializeChartHeadingRuntimeObjs(chart.Columns, (chart.ExprHost != null) ? chart.ChartExprHost.ColumnGroupingsHost : null);
						}
						else if (reportItem is CustomReportItem)
						{
							CustomReportItem customReportItem = (CustomReportItem)reportItem;
							InitializeCRIHeadingRuntimeObjs(customReportItem.Rows, (customReportItem.ExprHost == null) ? null : ((CustomReportItemExprHost)customReportItem.ExprHost).DataGroupingHostsRemotable);
							InitializeCRIHeadingRuntimeObjs(customReportItem.Columns, (customReportItem.ExprHost == null) ? null : ((CustomReportItemExprHost)customReportItem.ExprHost).DataGroupingHostsRemotable);
						}
						else if (reportItem is Table)
						{
							Table table = (Table)reportItem;
							if (table.HeaderRows != null)
							{
								for (int i = 0; i < table.HeaderRows.Count; i++)
								{
									RuntimeInitializeReportItemObjs(table.HeaderRows[i].ReportItems, traverseDataRegions, setValue);
								}
							}
							if (table.FooterRows != null)
							{
								for (int j = 0; j < table.FooterRows.Count; j++)
								{
									RuntimeInitializeReportItemObjs(table.FooterRows[j].ReportItems, traverseDataRegions, setValue);
								}
							}
							if (table.TableDetail != null)
							{
								for (int k = 0; k < table.TableDetail.DetailRows.Count; k++)
								{
									RuntimeInitializeReportItemObjs(table.TableDetail.DetailRows[k].ReportItems, traverseDataRegions, setValue);
								}
							}
							InitializeTableGroupRuntimeObjs(table, table.TableGroups, (table.ExprHost != null) ? table.TableExprHost.TableGroupsHost : null, this, m_reportObjectModel, traverseDataRegions, setValue);
						}
					}
				}
				if (reportItemImpl != null)
				{
					m_reportObjectModel.ReportItemsImpl.Add(reportItemImpl);
				}
			}

			internal void InitializeMatrixHeadingRuntimeObjs(MatrixHeading heading, MatrixDynamicGroupExprHost headingExprHost, bool traverseDataRegions, bool setValue)
			{
				while (heading != null)
				{
					RuntimeInitializeReportItemObjs(heading.ReportItems, traverseDataRegions, setValue);
					if (heading.Subtotal != null)
					{
						if (heading.HasExprHost && headingExprHost.SubtotalHost != null)
						{
							heading.Subtotal.SetExprHost(headingExprHost.SubtotalHost, m_reportObjectModel);
						}
						RuntimeInitializeReportItemObjs(heading.Subtotal.ReportItems, traverseDataRegions, setValue);
					}
					if (heading.HasExprHost)
					{
						heading.SetExprHost(headingExprHost, m_reportObjectModel);
						headingExprHost = (MatrixDynamicGroupExprHost)headingExprHost.SubGroupHost;
					}
					heading = heading.SubHeading;
				}
			}

			internal void InitializeChartHeadingRuntimeObjs(ChartHeading heading, ChartDynamicGroupExprHost headingExprHost)
			{
				while (heading != null)
				{
					if (heading.HasExprHost)
					{
						heading.SetExprHost(headingExprHost, m_reportObjectModel);
						headingExprHost = (ChartDynamicGroupExprHost)headingExprHost.SubGroupHost;
					}
					heading = heading.SubHeading;
				}
			}

			internal static void InitializeTableGroupRuntimeObjs(Table table, TableGroup group, TableGroupExprHost groupExprHost, ProcessingContext processingContext, ObjectModelImpl reportObjectModel, bool traverseDataRegions, bool setValue)
			{
				while (group != null)
				{
					if (group.HasExprHost)
					{
						group.SetExprHost(groupExprHost, reportObjectModel);
						groupExprHost = (TableGroupExprHost)groupExprHost.SubGroupHost;
					}
					if (processingContext != null)
					{
						if (group.HeaderRows != null)
						{
							for (int i = 0; i < group.HeaderRows.Count; i++)
							{
								processingContext.RuntimeInitializeReportItemObjs(group.HeaderRows[i].ReportItems, traverseDataRegions, setValue);
							}
						}
						if (group.FooterRows != null)
						{
							for (int j = 0; j < group.FooterRows.Count; j++)
							{
								processingContext.RuntimeInitializeReportItemObjs(group.FooterRows[j].ReportItems, traverseDataRegions, setValue);
							}
						}
					}
					group = group.SubGroup;
				}
				if (table.TableDetail != null && table.TableDetail.HasExprHost)
				{
					table.TableDetail.SetExprHost(groupExprHost, reportObjectModel);
				}
			}

			internal void InitializeCRIHeadingRuntimeObjs(CustomReportItemHeadingList headings, IList<DataGroupingExprHost> headingExprHosts)
			{
				if (headings == null)
				{
					return;
				}
				for (int i = 0; i < headings.Count; i++)
				{
					if (headings[i].HasExprHost)
					{
						headings[i].SetExprHost(headingExprHosts, m_reportObjectModel);
						if (headings[i].ExprHostID >= 0 && headingExprHosts[headings[i].ExprHostID].DataGroupingHostsRemotable != null)
						{
							Global.Tracer.Assert(headings[i].InnerHeadings != null, "(null != headings[i].InnerHeadings)");
							InitializeCRIHeadingRuntimeObjs(headings[i].InnerHeadings, headingExprHosts[headings[i].ExprHostID].DataGroupingHostsRemotable);
						}
					}
				}
			}

			internal void EndIgnoreRange()
			{
				Global.Tracer.Assert(m_showHideInfo != null, "(null != m_showHideInfo)");
				m_showHideInfo.EndIgnoreRange();
			}

			internal void RegisterIgnoreRange()
			{
				Global.Tracer.Assert(m_showHideInfo != null, "(null != m_showHideInfo)");
				m_showHideInfo.RegisterIgnoreRange();
			}

			internal void UnRegisterIgnoreRange()
			{
				Global.Tracer.Assert(m_showHideInfo != null, "(null != m_showHideInfo)");
				m_showHideInfo.UnRegisterIgnoreRange();
			}

			internal void BeginProcessContainer(int uniqueName, Visibility visibility)
			{
				if (visibility != null && visibility.Toggle != null)
				{
					Global.Tracer.Assert(m_showHideInfo != null, "(null != m_showHideInfo)");
					m_showHideInfo.RegisterContainer(uniqueName);
				}
			}

			internal void EndProcessContainer(int uniqueName, Visibility visibility)
			{
				if (visibility != null && visibility.Toggle != null)
				{
					Global.Tracer.Assert(m_showHideInfo != null, "(null != m_showHideInfo)");
					m_showHideInfo.UnRegisterContainer(uniqueName);
				}
			}

			internal bool ProcessSender(int uniqueName, bool startHidden, TextBox textBox)
			{
				bool result = false;
				if (textBox.InitialToggleState != null)
				{
					result = m_reportRuntime.EvaluateTextBoxInitialToggleStateExpression(textBox);
				}
				if (textBox.IsToggle)
				{
					Global.Tracer.Assert(m_showHideInfo != null, "(null != m_showHideInfo)");
					m_showHideInfo.RegisterSender(textBox.Name, uniqueName, startHidden, textBox.RecursiveSender);
				}
				return result;
			}

			internal bool ProcessReceiver(int uniqueName, Visibility visibility, IVisibilityHiddenExprHost visibilityExprHostRI, ObjectType objectType, string objectName)
			{
				bool flag = false;
				if (visibility != null)
				{
					flag = m_reportRuntime.EvaluateStartHiddenExpression(visibility, visibilityExprHostRI, objectType, objectName);
					if (visibility.Toggle != null)
					{
						Global.Tracer.Assert(m_showHideInfo != null, "(null != m_showHideInfo)");
						m_showHideInfo.RegisterReceiver(visibility.Toggle, uniqueName, flag, visibility.RecursiveReceiver);
					}
				}
				return flag;
			}

			internal bool ProcessReceiver(int uniqueName, Visibility visibility, IndexedExprHost visibilityExprHostIdx, ObjectType objectType, string objectName)
			{
				bool flag = false;
				if (visibility != null)
				{
					flag = m_reportRuntime.EvaluateStartHiddenExpression(visibility, visibilityExprHostIdx, objectType, objectName);
					if (visibility.Toggle != null)
					{
						Global.Tracer.Assert(m_showHideInfo != null, "(null != m_showHideInfo)");
						m_showHideInfo.RegisterReceiver(visibility.Toggle, uniqueName, flag, visibility.RecursiveReceiver);
					}
				}
				return flag;
			}

			internal void EnterGrouping()
			{
				Global.Tracer.Assert(m_showHideInfo != null, "(null != m_showHideInfo)");
				m_showHideInfo.EnterGrouping();
			}

			internal void EnterChildGroupings()
			{
				Global.Tracer.Assert(m_showHideInfo != null, "(null != m_showHideInfo)");
				m_showHideInfo.EnterChildGroupings();
			}

			internal void ExitGrouping()
			{
				Global.Tracer.Assert(m_showHideInfo != null, "(null != m_showHideInfo)");
				m_showHideInfo.ExitGrouping();
			}

			internal void ExitChildGroupings()
			{
				Global.Tracer.Assert(m_showHideInfo != null, "(null != m_showHideInfo)");
				m_showHideInfo.ExitChildGroupings();
			}

			internal void GetSenderAndReceiverInfo(out SenderInformationHashtable senderInfo, out ReceiverInformationHashtable receiverInfo)
			{
				Global.Tracer.Assert(m_showHideInfo != null, "(null != m_showHideInfo)");
				m_showHideInfo.GetSenderAndReceiverInfo(out senderInfo, out receiverInfo);
			}

			internal void AddSpecialDataRegionFilters(Filters filters)
			{
				if (m_specialDataRegionFilters == null)
				{
					m_specialDataRegionFilters = new FiltersList();
				}
				m_specialDataRegionFilters.Add(filters);
			}

			private void ProcessDataRegionsWithSpecialFilters()
			{
				if (m_specialDataRegionFilters != null)
				{
					int count = m_specialDataRegionFilters.Count;
					for (int i = 0; i < count; i++)
					{
						m_specialDataRegionFilters[i].FinishReadingRows();
						count = m_specialDataRegionFilters.Count;
					}
					m_specialDataRegionFilters = null;
				}
			}

			internal void EnterPivotCell(bool escalateScope)
			{
				if (m_pivotRunningValueScopes == null)
				{
					m_pivotRunningValueScopes = new List<bool>();
				}
				m_pivotRunningValueScopes.Add(escalateScope);
			}

			internal void ExitPivotCell()
			{
				Global.Tracer.Assert(m_pivotRunningValueScopes != null, "(null != m_pivotRunningValueScopes)");
				m_pivotRunningValueScopes.RemoveAt(m_pivotRunningValueScopes.Count - 1);
			}

			internal bool PivotEscalateScope()
			{
				if (m_pivotRunningValueScopes == null || 0 >= m_pivotRunningValueScopes.Count)
				{
					return false;
				}
				return m_pivotRunningValueScopes[m_pivotRunningValueScopes.Count - 1];
			}

			internal bool PopulateRuntimeSortFilterEventInfo(DataSet myDataSet)
			{
				return m_userSortFilterContext.PopulateRuntimeSortFilterEventInfo(this, myDataSet);
			}

			internal bool IsSortFilterTarget(bool[] isSortFilterTarget, IScope outerScope, IHierarchyObj target, ref RuntimeUserSortTargetInfo userSortTargetInfo)
			{
				return m_userSortFilterContext.IsSortFilterTarget(isSortFilterTarget, outerScope, target, ref userSortTargetInfo);
			}

			internal EventInformation GetUserSortFilterInformation(ref int oldUniqueName, ref int page)
			{
				return m_commonInfo.GetUserSortFilterInformation(ref oldUniqueName, ref page);
			}

			internal void RegisterSortFilterExpressionScope(IScope container, RuntimeDataRegionObj scopeObj, bool[] isSortFilterExpressionScope)
			{
				m_userSortFilterContext.RegisterSortFilterExpressionScope(container, scopeObj, isSortFilterExpressionScope);
			}

			internal void ProcessUserSortForTarget(IHierarchyObj target, ref DataRowList dataRows, bool targetForNonDetailSort)
			{
				m_userSortFilterContext.ProcessUserSortForTarget(m_reportObjectModel, m_reportRuntime, target, ref dataRows, targetForNonDetailSort);
			}

			internal ProcessingMessageList RegisterComparisonErrorForSortFilterEvent(string propertyName)
			{
				Global.Tracer.Assert(m_userSortFilterContext.CurrentSortFilterEventSource != null, "(null != m_userSortFilterContext.CurrentSortFilterEventSource)");
				m_errorContext.Register(ProcessingErrorCode.rsComparisonError, Severity.Error, m_userSortFilterContext.CurrentSortFilterEventSource.ObjectType, m_userSortFilterContext.CurrentSortFilterEventSource.Name, propertyName);
				return m_errorContext.Messages;
			}

			internal void FirstPassPostProcess()
			{
				do
				{
					ProcessDataRegionsWithSpecialFilters();
				}
				while (m_userSortFilterContext.ProcessUserSort(this));
			}

			internal VariantList[] GetScopeValues(GroupingList containingScopes, IScope containingScope)
			{
				VariantList[] array = null;
				if (containingScopes != null && 0 < containingScopes.Count)
				{
					array = new VariantList[containingScopes.Count];
					int index = 0;
					containingScope.GetScopeValues(null, array, ref index);
				}
				return array;
			}

			internal int CreateUniqueName()
			{
				return m_commonInfo.CreateUniqueName();
			}

			internal int CreateIDForSubreport()
			{
				return m_commonInfo.CreateIDForSubreport();
			}

			internal int GetLastIDForReport()
			{
				return m_commonInfo.GetLastIDForReport();
			}

			internal bool GetResource(string path, out byte[] resource, out string mimeType)
			{
				if (m_commonInfo.GetResourceCallback != null)
				{
					m_commonInfo.GetResourceCallback.GetResource(m_reportContext, path, out resource, out mimeType, out bool registerExternalWarning, out bool _);
					if (registerExternalWarning)
					{
						ErrorContext.Register(ProcessingErrorCode.rsWarningFetchingExternalImages, Severity.Warning, ObjectType.Report, null, null);
					}
					return true;
				}
				resource = null;
				mimeType = null;
				return false;
			}
		}

		internal sealed class ReportProcessingContext : ProcessingContext
		{
			private RuntimeDataSourceInfoCollection m_dataSourceInfos;

			internal RuntimeDataSourceInfoCollection DataSourceInfos => m_dataSourceInfos;

			internal ReportProcessingContext(string chartName, RuntimeDataSourceInfoCollection dataSourceInfos, string requestUserName, CultureInfo userLanguage, SubReportCallback subReportCallback, ICatalogItemContext reportContext, Report report, ErrorContext errorContext, CreateReportChunk createReportChunkCallback, IGetResource getResourceCallback, ExecutionType interactiveExecution, DateTime executionTime, UserProfileState allowUserProfileState, bool isHistorySnapshot, bool snapshotProcessing, bool processWithCachedData, GetReportChunk getChunkCallback, CreateReportChunk cacheDataCallback, IProcessingDataExtensionConnection dataExtensionConnection, ReportRuntimeSetup reportRuntimeSetup, IJobContext jobContext, IExtensionFactory extFactory, IDataProtection dataProtection)
				: base(chartName, requestUserName, userLanguage, subReportCallback, reportContext, report, errorContext, createReportChunkCallback, getResourceCallback, interactiveExecution, executionTime, allowUserProfileState, isHistorySnapshot, snapshotProcessing, processWithCachedData, getChunkCallback, cacheDataCallback, reportRuntimeSetup, jobContext, extFactory, dataExtensionConnection, dataProtection)
			{
				m_dataSourceInfos = dataSourceInfos;
			}

			internal ReportProcessingContext(ProcessingContext parentContext, RuntimeDataSourceInfoCollection dataSourceInfos, string requestUserName, CultureInfo userlanguage, ICatalogItemContext reportContext, ErrorContext errorContext, ExecutionType interactiveExecution, DateTime executionTime, UserProfileState allowUserProfileState, bool snapshotProcessing, IProcessingDataExtensionConnection dataExtensionConnection, ReportRuntimeSetup reportRuntimeSetup, IJobContext jobContext, IExtensionFactory extFactory, IDataProtection dataProtection)
				: base(parentContext, requestUserName, userlanguage, reportContext, errorContext, interactiveExecution, executionTime, allowUserProfileState, snapshotProcessing, reportRuntimeSetup, jobContext, extFactory, dataExtensionConnection, dataProtection)
			{
				m_dataSourceInfos = dataSourceInfos;
			}

			private ReportProcessingContext(SubReport subReport, ErrorContext errorContext, ReportProcessingContext copy, int subReportUniqueName)
				: base(subReport, errorContext, copy, subReportUniqueName)
			{
				m_dataSourceInfos = copy.DataSourceInfos;
			}

			private ReportProcessingContext(ReportProcessingContext copy)
				: base(copy)
			{
				m_dataSourceInfos = copy.DataSourceInfos;
			}

			internal override ProcessingContext ParametersContext(ICatalogItemContext reportContext, ProcessingErrorContext errorContext)
			{
				return new ReportProcessingContext(this, DataSourceInfos, base.RequestUserName, base.UserLanguage, reportContext, errorContext, base.InteractiveExecution, base.ExecutionTime, base.AllowUserProfileState, base.SnapshotProcessing, base.DataExtensionConnection, base.ReportRuntimeSetup, base.JobContext, base.ExtFactory, base.DataProtection);
			}

			internal override ProcessingContext SubReportContext(SubReport subReport, int subReportDataSetUniqueName, ProcessingErrorContext subReportErrorContext)
			{
				return new ReportProcessingContext(subReport, subReportErrorContext, this, subReportDataSetUniqueName);
			}

			internal override ProcessingContext CloneContext(ProcessingContext context)
			{
				return new ReportProcessingContext((ReportProcessingContext)context);
			}
		}

		internal sealed class Pagination
		{
			private double m_currentPageHeight;

			private double m_pageMaxHeight;

			private int m_ignorePageBreak;

			private int m_ignoreHeight;

			private const double MINIMUM_START_ON_PAGE = 12.7;

			internal double PageHeight => m_pageMaxHeight;

			internal double CurrentPageHeight => m_currentPageHeight;

			internal bool IgnorePageBreak => m_ignorePageBreak != 0;

			internal bool IgnoreHeight => m_ignoreHeight != 0;

			internal Pagination(double pageMaxHeight)
			{
				m_pageMaxHeight = pageMaxHeight;
			}

			internal void EnterIgnorePageBreak(Visibility visibility, bool ignoreAlways)
			{
				if (ignoreAlways || Microsoft.ReportingServices.ReportRendering.SharedHiddenState.Never != Visibility.GetSharedHidden(visibility))
				{
					m_ignorePageBreak++;
				}
			}

			internal void LeaveIgnorePageBreak(Visibility visibility, bool ignoreAlways)
			{
				if (ignoreAlways || Microsoft.ReportingServices.ReportRendering.SharedHiddenState.Never != Visibility.GetSharedHidden(visibility))
				{
					m_ignorePageBreak--;
				}
				Global.Tracer.Assert(0 <= m_ignorePageBreak, "(0 <= m_ignorePageBreak)");
			}

			internal void EnterIgnoreHeight(bool startHidden)
			{
				if (startHidden)
				{
					m_ignoreHeight++;
				}
			}

			internal void LeaveIgnoreHeight(bool startHidden)
			{
				if (startHidden)
				{
					m_ignoreHeight--;
				}
				Global.Tracer.Assert(0 <= m_ignoreHeight, "(0 <= m_ignoreHeight)");
			}

			internal void CopyPaginationInfo(Pagination pagination)
			{
				m_ignoreHeight = pagination.m_ignoreHeight;
				m_ignorePageBreak = pagination.m_ignorePageBreak;
			}

			internal bool CalculateSoftPageBreak(ReportItem reportItem, double itemHeight, double distanceBeforeOrAfter, bool ignoreSoftPageBreak)
			{
				return CalculateSoftPageBreak(reportItem, itemHeight, distanceBeforeOrAfter, ignoreSoftPageBreak, PageBreakAtStart(reportItem));
			}

			internal bool CalculateSoftPageBreak(ReportItem reportItem, double itemHeight, double distanceBeforeOrAfter, bool ignoreSoftPageBreak, bool logicalPageBreak)
			{
				if (!IgnorePageBreak && logicalPageBreak)
				{
					if (0.0 == m_currentPageHeight)
					{
						return false;
					}
					SetCurrentPageHeight(reportItem, 0.0);
					return true;
				}
				if (IgnoreHeight)
				{
					return false;
				}
				if (reportItem != null)
				{
					ComputeReportItemTrueTop(reportItem);
				}
				m_currentPageHeight += itemHeight + distanceBeforeOrAfter;
				if (!IgnorePageBreak && m_currentPageHeight > m_pageMaxHeight && !ignoreSoftPageBreak)
				{
					SetCurrentPageHeight(reportItem, 0.0);
					return true;
				}
				return false;
			}

			internal void ProcessEndPage(IPageItem riInstance, ReportItem reportItem, bool pageBreakAtEnd, bool childrenOnThisPage)
			{
				riInstance.StartPage = reportItem.StartPage;
				riInstance.EndPage = reportItem.EndPage;
				if (!(reportItem is List))
				{
					LeaveIgnoreHeight(reportItem.StartHidden);
				}
				reportItem.BottomInEndPage = m_currentPageHeight;
				if (reportItem.Parent != null && reportItem.EndPage > reportItem.Parent.EndPage)
				{
					reportItem.Parent.EndPage = reportItem.EndPage;
					reportItem.Parent.BottomInEndPage = reportItem.BottomInEndPage;
					if (reportItem.Parent is List)
					{
						((List)reportItem.Parent).ContentStartPage = reportItem.EndPage;
					}
				}
				if (!IgnorePageBreak && pageBreakAtEnd)
				{
					if (!IgnoreHeight)
					{
						AddToCurrentPageHeight(reportItem, m_pageMaxHeight + 1.0);
					}
					reportItem.ShareMyLastPage = !childrenOnThisPage;
				}
				else if (reportItem.Parent != null)
				{
					reportItem.Parent.ShareMyLastPage = true;
				}
			}

			internal void ProcessEndGroupPage(double distance, bool pageBreakAtEnd, ReportItem parent, bool childrenOnThisPage, bool startHidden)
			{
				LeaveIgnoreHeight(startHidden);
				if (!IgnoreHeight)
				{
					m_currentPageHeight += distance;
				}
				if (!IgnorePageBreak && pageBreakAtEnd)
				{
					if (!IgnoreHeight)
					{
						m_currentPageHeight += m_pageMaxHeight + 1.0;
					}
					if (parent != null)
					{
						parent.ShareMyLastPage = !childrenOnThisPage;
					}
				}
				else if (parent != null)
				{
					parent.ShareMyLastPage = true;
				}
				if (parent != null)
				{
					parent.BottomInEndPage = m_currentPageHeight;
				}
			}

			internal void SetReportItemStartPage(ReportItem reportItem, bool softPageAtStart)
			{
				ReportItemCollection reportItemCollection = null;
				int num = reportItem.StartPage;
				ReportItem parent = reportItem.Parent;
				if (parent != null)
				{
					if (parent is Rectangle)
					{
						reportItemCollection = ((Rectangle)parent).ReportItems;
					}
					else if (parent is List)
					{
						reportItemCollection = ((List)parent).ReportItems;
						num = ((List)parent).ContentStartPage;
					}
					else if (parent is Table)
					{
						num = ((Table)parent).CurrentPage;
					}
					else if (parent is Matrix)
					{
						num = ((Matrix)parent).CurrentPage;
					}
					else if (parent is Report)
					{
						reportItemCollection = ((Report)parent).ReportItems;
					}
					if (-1 == num)
					{
						num = parent.StartPage;
					}
				}
				bool flag = false;
				bool flag2 = false;
				if (reportItemCollection != null && reportItem.SiblingAboveMe != null)
				{
					for (int i = 0; i < reportItem.SiblingAboveMe.Count; i++)
					{
						ReportItem reportItem2 = reportItemCollection[reportItem.SiblingAboveMe[i]];
						int num2 = reportItem2.EndPage;
						if (!reportItemCollection.IsReportItemComputed(reportItem.SiblingAboveMe[i]))
						{
							flag = true;
						}
						bool num3 = reportItem2.TopValue + reportItem2.HeightValue > reportItem.TopValue + 0.0009;
						if (num3)
						{
							num2 = reportItem2.StartPage;
						}
						if (num2 > num)
						{
							flag2 = false;
						}
						if (!num3 && PageBreakAtEnd(reportItem2))
						{
							flag2 = (num2 >= num);
						}
						num = Math.Max(num, num2);
					}
				}
				else if (reportItem.Parent != null)
				{
					num = Math.Max(num, reportItem.Parent.StartPage);
				}
				bool flag3 = PageBreakAtStart(reportItem);
				if (flag2 || softPageAtStart || CanMoveToNextPage(flag3))
				{
					num++;
					m_currentPageHeight = 0.0;
				}
				if (flag && !IgnoreHeight && 0.0 == m_currentPageHeight)
				{
					m_currentPageHeight += 1.0;
					if (flag3)
					{
						if (flag2)
						{
							num++;
						}
						else if (!softPageAtStart)
						{
							num++;
						}
					}
				}
				reportItem.StartPage = num;
				reportItem.EndPage = num;
				if ((reportItem is TextBox || reportItem is Image || reportItem is Chart) && !IgnoreHeight && 0.0 == m_currentPageHeight)
				{
					m_currentPageHeight += 1.0;
				}
				reportItem.TopInStartPage = m_currentPageHeight;
				reportItem.BottomInEndPage = m_currentPageHeight;
			}

			internal bool PageBreakAtEnd(ReportItem reportItem)
			{
				if (reportItem.SoftPageBreak)
				{
					return true;
				}
				if (reportItem is List && ((List)reportItem).PropagatedPageBreakAtEnd)
				{
					return true;
				}
				if (reportItem is Table && ((Table)reportItem).PropagatedPageBreakAtEnd)
				{
					return true;
				}
				if (reportItem is Matrix && ((Matrix)reportItem).PropagatedPageBreakAtEnd)
				{
					return true;
				}
				return CheckPageBreak(reportItem, start: false);
			}

			internal bool PageBreakAtStart(ReportItem reportItem)
			{
				if (reportItem is List && ((List)reportItem).PropagatedPageBreakAtStart)
				{
					return true;
				}
				if (reportItem is Table && ((Table)reportItem).PropagatedPageBreakAtStart)
				{
					return true;
				}
				if (reportItem is Matrix && ((Matrix)reportItem).PropagatedPageBreakAtStart)
				{
					return true;
				}
				return CheckPageBreak(reportItem, start: true);
			}

			private bool CheckPageBreak(ReportItem reportItem, bool start)
			{
				if (IgnorePageBreak)
				{
					return false;
				}
				if (!(reportItem is DataRegion) && !(reportItem is Rectangle))
				{
					return false;
				}
				IPageBreakItem pageBreakItem = (IPageBreakItem)reportItem;
				if (pageBreakItem != null)
				{
					if (pageBreakItem.IgnorePageBreaks())
					{
						return false;
					}
					return pageBreakItem.HasPageBreaks(start);
				}
				return false;
			}

			internal bool CanMoveToNextPage(bool pageBreakAtStart)
			{
				if (IgnorePageBreak)
				{
					return false;
				}
				if (!pageBreakAtStart || 0.0 == m_currentPageHeight)
				{
					return false;
				}
				return true;
			}

			internal void ProcessListRenderingPages(ListInstance listInstance, List listDef)
			{
				RenderingPagesRangesList childrenStartAndEndPages = listInstance.ChildrenStartAndEndPages;
				Global.Tracer.Assert(childrenStartAndEndPages != null, "(null != listPagesList)");
				bool childrenOnThisPage = false;
				if (listDef.Grouping == null)
				{
					if (listInstance.NumberOfContentsOnThisPage > 0)
					{
						RenderingPagesRanges renderingPagesRanges = default(RenderingPagesRanges);
						renderingPagesRanges.StartRow = listInstance.ListContents.Count - listInstance.NumberOfContentsOnThisPage;
						renderingPagesRanges.NumberOfDetails = listInstance.NumberOfContentsOnThisPage;
						childrenStartAndEndPages.Add(renderingPagesRanges);
						childrenOnThisPage = true;
					}
					if (childrenStartAndEndPages != null && childrenStartAndEndPages.Count > 0)
					{
						listDef.EndPage = listDef.StartPage + childrenStartAndEndPages.Count - 1;
					}
					else
					{
						listDef.EndPage = listDef.StartPage;
					}
				}
				else if (childrenStartAndEndPages.Count > 0)
				{
					listDef.StartPage = childrenStartAndEndPages[0].StartPage;
					listDef.EndPage = childrenStartAndEndPages[listInstance.ListContents.Count - 1].EndPage;
					childrenOnThisPage = true;
				}
				else
				{
					listDef.EndPage = listDef.StartPage;
				}
				ProcessEndPage(listInstance, listDef, PageBreakAtEnd(listDef), childrenOnThisPage);
			}

			internal void InitProcessTableRenderingPages(TableInstance tableInstance, Table table)
			{
				double headerHeightValue = table.HeaderHeightValue;
				if (!IgnorePageBreak && headerHeightValue < m_pageMaxHeight && headerHeightValue + m_currentPageHeight > m_pageMaxHeight)
				{
					SetCurrentPageHeight(table, 0.0);
					table.StartPage++;
					((IPageItem)tableInstance).StartPage = table.StartPage;
					tableInstance.CurrentPage++;
					table.CurrentPage = tableInstance.CurrentPage;
				}
				else if (!IgnoreHeight)
				{
					AddToCurrentPageHeight(table, headerHeightValue);
				}
			}

			internal void InitProcessingTableGroup(TableInstance tableInstance, Table table, TableGroupInstance tableGroupInstance, TableGroup tableGroup, ref RenderingPagesRanges renderingPagesRanges, bool ignorePageBreakAtStart)
			{
				EnterIgnorePageBreak(tableGroup.Visibility, ignoreAlways: false);
				tableGroup.StartPage = tableInstance.CurrentPage;
				if (tableGroup.InnerHierarchy == null && table.TableDetail == null)
				{
					double headerHeightValue = tableGroup.HeaderHeightValue;
					if (!IgnorePageBreak && headerHeightValue + m_currentPageHeight > m_pageMaxHeight)
					{
						SetCurrentPageHeight(table, 0.0);
						tableInstance.CurrentPage++;
						table.CurrentPage = tableInstance.CurrentPage;
					}
					else if (!IgnoreHeight)
					{
						AddToCurrentPageHeight(table, headerHeightValue);
					}
				}
				bool flag = false;
				if (!ignorePageBreakAtStart)
				{
					flag = CalculateSoftPageBreak(null, 0.0, 0.0, ignoreSoftPageBreak: false, tableGroup.PropagatedPageBreakAtStart || tableGroup.Grouping.PageBreakAtEnd);
					if (!IgnorePageBreak && flag)
					{
						SetCurrentPageHeight(table, 0.0);
						tableInstance.CurrentPage++;
						table.CurrentPage = tableInstance.CurrentPage;
					}
				}
				renderingPagesRanges.StartPage = tableInstance.CurrentPage;
			}

			internal void ProcessTableDetails(Table tableDef, TableDetailInstance detailInstance, IList detailInstances, ref double detailHeightValue, TableRowList rowDefs, RenderingPagesRangesList pagesList, ref int numberOfChildrenOnThisPage)
			{
				if (-1.0 == detailHeightValue)
				{
					detailHeightValue = tableDef.DetailHeightValue;
				}
				if (!IgnoreHeight)
				{
					AddToCurrentPageHeight(tableDef, detailHeightValue);
				}
				if (!IgnorePageBreak && m_currentPageHeight >= m_pageMaxHeight && numberOfChildrenOnThisPage > 0)
				{
					RenderingPagesRanges renderingPagesRanges = default(RenderingPagesRanges);
					renderingPagesRanges.StartRow = detailInstances.Count - numberOfChildrenOnThisPage;
					renderingPagesRanges.NumberOfDetails = numberOfChildrenOnThisPage;
					SetCurrentPageHeight(tableDef, 0.0);
					tableDef.CurrentPage++;
					pagesList.Add(renderingPagesRanges);
					numberOfChildrenOnThisPage = 1;
				}
				else
				{
					numberOfChildrenOnThisPage++;
				}
			}

			internal void ProcessTableRenderingPages(TableInstance tableInstance, Table reportItem)
			{
				ProcessEndPage(tableInstance, reportItem, PageBreakAtEnd(reportItem), tableInstance.NumberOfChildrenOnThisPage > 0);
				reportItem.EndPage = tableInstance.CurrentPage;
				reportItem.CurrentPage = tableInstance.CurrentPage;
			}

			internal void ComputeReportItemTrueTop(ReportItem reportItem)
			{
				ReportItemCollection reportItemCollection = null;
				int num = reportItem.StartPage;
				ReportItem parent = reportItem.Parent;
				double num2 = 0.0;
				if (parent != null)
				{
					if (parent is Rectangle)
					{
						reportItemCollection = ((Rectangle)parent).ReportItems;
						num2 = parent.TopInStartPage;
					}
					else if (parent is List)
					{
						reportItemCollection = ((List)parent).ReportItems;
						num = ((List)parent).ContentStartPage;
						num2 = parent.BottomInEndPage;
					}
					else if (parent is Table)
					{
						num = ((Table)parent).CurrentPage;
						num2 = parent.BottomInEndPage;
					}
					else if (parent is Matrix)
					{
						num = ((Matrix)parent).CurrentPage;
						num2 = parent.BottomInEndPage;
					}
					else if (parent is Report)
					{
						reportItemCollection = ((Report)parent).ReportItems;
						num2 = parent.TopInStartPage;
					}
					if (-1 == num)
					{
						num = parent.StartPage;
					}
				}
				if (reportItemCollection != null && reportItem.SiblingAboveMe != null)
				{
					for (int i = 0; i < reportItem.SiblingAboveMe.Count; i++)
					{
						ReportItem reportItem2 = reportItemCollection[reportItem.SiblingAboveMe[i]];
						int num3 = reportItem2.EndPage;
						double num4 = reportItem2.BottomInEndPage;
						if (reportItem2.TopValue + reportItem2.HeightValue > reportItem.TopValue + 0.0009)
						{
							num3 = reportItem2.StartPage;
							num4 = reportItem2.TopInStartPage;
						}
						if (num3 > num)
						{
							num = num3;
							num2 = num4;
						}
						else if (num3 == num)
						{
							num2 = Math.Max(num2, num4);
						}
					}
				}
				m_currentPageHeight = num2;
				reportItem.TopInStartPage = num2;
			}

			internal void AddToCurrentPageHeight(ReportItem reportItem, double distance)
			{
				m_currentPageHeight += distance;
				if (reportItem != null)
				{
					reportItem.BottomInEndPage = m_currentPageHeight;
				}
			}

			internal void SetCurrentPageHeight(ReportItem reportItem, double distance)
			{
				m_currentPageHeight = distance;
				if (reportItem != null)
				{
					reportItem.BottomInEndPage = m_currentPageHeight;
				}
			}

			internal bool ShouldItemMoveToChildStartPage(ReportItem reportItem)
			{
				List list = reportItem as List;
				if (list == null)
				{
					return false;
				}
				if (IgnoreHeight && m_pageMaxHeight < 25.4)
				{
					return false;
				}
				if (list.KeepWithChildFirstPage == 0)
				{
					return false;
				}
				if (-1 == list.KeepWithChildFirstPage)
				{
					ReportItemCollection reportItems = list.ReportItems;
					int keepWithChildFirstPage = 0;
					if (reportItems != null && reportItems.Count > 0)
					{
						ReportItem reportItem2 = reportItems[0];
						if (!PageBreakAtStart(reportItem2) && reportItem2.TopValue < 12.7)
						{
							keepWithChildFirstPage = 1;
						}
					}
					list.KeepWithChildFirstPage = keepWithChildFirstPage;
				}
				if (1 == list.KeepWithChildFirstPage)
				{
					return true;
				}
				return false;
			}

			internal int GetTextBoxStartPage(TextBox textBox)
			{
				if (-1 == textBox.StartPage)
				{
					Global.Tracer.Assert(textBox.Parent is Table || textBox.Parent is Matrix || textBox.Parent is CustomReportItem);
					if (textBox.Parent is Table)
					{
						return ((Table)textBox.Parent).CurrentPage;
					}
					if (textBox.Parent is Matrix)
					{
						return ((Matrix)textBox.Parent).CurrentPage;
					}
				}
				return textBox.StartPage;
			}
		}

		internal sealed class NavigationInfo
		{
			internal sealed class DocumentMapNodeList : ArrayList
			{
				internal new DocumentMapNode this[int index]
				{
					get
					{
						return (DocumentMapNode)base[index];
					}
					set
					{
						base[index] = value;
					}
				}

				internal DocumentMapNodeList()
				{
				}
			}

			internal sealed class DocumentMapNodeLists : ArrayList
			{
				internal new DocumentMapNodeList this[int index]
				{
					get
					{
						return (DocumentMapNodeList)base[index];
					}
					set
					{
						base[index] = value;
					}
				}

				internal DocumentMapNodeLists()
				{
				}
			}

			private DocumentMapNodeLists m_reportDocumentMapChildren;

			private string m_currentLabel;

			private ArrayList m_matrixColumnDocumentMaps = new ArrayList();

			private int m_inMatrixColumn = -1;

			private BookmarksHashtable m_bookmarksInfo;

			internal DocumentMapNodeLists DocumentMapChildren => CurrentDocumentMapChildren;

			internal DocumentMapNodeList CurrentDocumentMapSiblings
			{
				get
				{
					DocumentMapNodeLists currentDocumentMapChildren = CurrentDocumentMapChildren;
					if (currentDocumentMapChildren != null && 0 < currentDocumentMapChildren.Count)
					{
						return currentDocumentMapChildren[currentDocumentMapChildren.Count - 1];
					}
					return null;
				}
			}

			internal string CurrentLabel => m_currentLabel;

			internal BookmarksHashtable BookmarksInfo
			{
				get
				{
					return m_bookmarksInfo;
				}
				set
				{
					m_bookmarksInfo = value;
				}
			}

			private DocumentMapNodeLists CurrentDocumentMapChildren
			{
				get
				{
					if (0 <= m_inMatrixColumn)
					{
						return (DocumentMapNodeLists)m_matrixColumnDocumentMaps[m_inMatrixColumn];
					}
					return m_reportDocumentMapChildren;
				}
				set
				{
					if (0 <= m_inMatrixColumn)
					{
						m_matrixColumnDocumentMaps[m_inMatrixColumn] = value;
					}
					else
					{
						m_reportDocumentMapChildren = value;
					}
				}
			}

			private DocumentMapNodeLists CurrentMatrixColumnDocumentMapChildren
			{
				get
				{
					if (m_matrixColumnDocumentMaps == null || m_matrixColumnDocumentMaps.Count == 0)
					{
						return null;
					}
					if (0 <= m_inMatrixColumn)
					{
						return (DocumentMapNodeLists)m_matrixColumnDocumentMaps[m_inMatrixColumn];
					}
					return (DocumentMapNodeLists)m_matrixColumnDocumentMaps[0];
				}
				set
				{
					if (m_matrixColumnDocumentMaps != null && 0 < m_matrixColumnDocumentMaps.Count)
					{
						if (0 <= m_inMatrixColumn)
						{
							m_matrixColumnDocumentMaps[m_inMatrixColumn] = value;
						}
						else
						{
							m_matrixColumnDocumentMaps[0] = value;
						}
					}
				}
			}

			internal void GetCurrentDocumentMapPosition(out int siblingIndex, out int nodeIndex)
			{
				siblingIndex = 0;
				nodeIndex = 0;
				DocumentMapNodeLists currentDocumentMapChildren = CurrentDocumentMapChildren;
				if (currentDocumentMapChildren != null && 0 < currentDocumentMapChildren.Count)
				{
					siblingIndex = currentDocumentMapChildren.Count - 1;
					DocumentMapNodeList documentMapNodeList = currentDocumentMapChildren[siblingIndex];
					if (documentMapNodeList != null)
					{
						nodeIndex = documentMapNodeList.Count;
					}
				}
			}

			internal void EnterMatrixColumn()
			{
				m_inMatrixColumn++;
				if (m_matrixColumnDocumentMaps == null)
				{
					m_matrixColumnDocumentMaps = new ArrayList();
				}
				if (m_matrixColumnDocumentMaps.Count <= m_inMatrixColumn)
				{
					Global.Tracer.Assert(m_matrixColumnDocumentMaps.Count == m_inMatrixColumn, "(m_matrixColumnDocumentMaps.Count == m_inMatrixColumn)");
					m_matrixColumnDocumentMaps.Add(null);
				}
			}

			internal void LeaveMatrixColumn()
			{
				m_inMatrixColumn--;
				Global.Tracer.Assert(-1 <= m_inMatrixColumn, "(-1 <= m_inMatrixColumn)");
			}

			internal void InsertMatrixColumnDocumentMap(int siblingIndex, int nodeIndex)
			{
				DocumentMapNodeLists currentMatrixColumnDocumentMapChildren = CurrentMatrixColumnDocumentMapChildren;
				if (currentMatrixColumnDocumentMapChildren == null || 0 >= currentMatrixColumnDocumentMapChildren.Count)
				{
					return;
				}
				DocumentMapNodeLists currentDocumentMapChildren = CurrentDocumentMapChildren;
				DocumentMapNodeList documentMapNodeList = null;
				if (currentDocumentMapChildren != null && 0 <= currentDocumentMapChildren.Count)
				{
					documentMapNodeList = currentDocumentMapChildren[siblingIndex];
				}
				if (documentMapNodeList == null)
				{
					if (currentDocumentMapChildren != null)
					{
						documentMapNodeList = (currentDocumentMapChildren[siblingIndex] = currentMatrixColumnDocumentMapChildren[0]);
					}
					else
					{
						DocumentMapNodeLists documentMapNodeLists2 = CurrentDocumentMapChildren = currentMatrixColumnDocumentMapChildren;
						currentDocumentMapChildren = documentMapNodeLists2;
					}
				}
				else
				{
					Global.Tracer.Assert(currentDocumentMapChildren != null, "(null != currentDocMap)");
					Global.Tracer.Assert(0 <= nodeIndex && nodeIndex <= documentMapNodeList.Count, "(0 <= nodeIndex && nodeIndex <= siblings.Count)");
					documentMapNodeList.InsertRange(nodeIndex, currentMatrixColumnDocumentMapChildren[0]);
				}
				CurrentMatrixColumnDocumentMapChildren = null;
			}

			internal void AppendNavigationInfo(string label, NavigationInfo navigationInfo, int startPage)
			{
				DocumentMapNodeLists currentDocumentMapChildren = CurrentDocumentMapChildren;
				DocumentMapNodeLists currentDocumentMapChildren2 = navigationInfo.CurrentDocumentMapChildren;
				if (currentDocumentMapChildren2 != null && 0 < currentDocumentMapChildren2.Count)
				{
					navigationInfo.UpdateDocumentMapChildrenPage(startPage);
					if (label == null)
					{
						if (currentDocumentMapChildren == null)
						{
							DocumentMapNodeLists documentMapNodeLists2 = CurrentDocumentMapChildren = currentDocumentMapChildren2;
							currentDocumentMapChildren = documentMapNodeLists2;
						}
						else
						{
							currentDocumentMapChildren.AddRange(currentDocumentMapChildren2[0]);
						}
					}
					else
					{
						EnterDocumentMapChildren();
						currentDocumentMapChildren[currentDocumentMapChildren.Count - 1] = currentDocumentMapChildren2[0];
					}
				}
				if (navigationInfo.m_bookmarksInfo != null && 0 < navigationInfo.m_bookmarksInfo.Count)
				{
					if (m_bookmarksInfo == null)
					{
						m_bookmarksInfo = new BookmarksHashtable();
					}
					IDictionaryEnumerator enumerator = navigationInfo.m_bookmarksInfo.GetEnumerator();
					while (enumerator.MoveNext())
					{
						string bookmark = (string)enumerator.Key;
						BookmarkInformation bookmarkInformation = (BookmarkInformation)enumerator.Value;
						m_bookmarksInfo.Add(bookmark, bookmarkInformation.Page + startPage, bookmarkInformation.Id);
					}
				}
			}

			private void UpdateDocumentMapChildrenPage(int startPage)
			{
				DocumentMapNodeLists currentDocumentMapChildren = CurrentDocumentMapChildren;
				if (currentDocumentMapChildren == null)
				{
					return;
				}
				for (int i = 0; i < currentDocumentMapChildren.Count; i++)
				{
					DocumentMapNodeList documentMapNodeList = currentDocumentMapChildren[i];
					if (documentMapNodeList != null)
					{
						for (int j = 0; j < documentMapNodeList.Count; j++)
						{
							UpdateDocumentMapNodePage(documentMapNodeList[j], startPage);
						}
					}
				}
			}

			private void UpdateDocumentMapNodePage(DocumentMapNode node, int startPage)
			{
				node.Page += startPage;
				if (node.Children != null)
				{
					for (int i = 0; i < node.Children.Length; i++)
					{
						UpdateDocumentMapNodePage(node.Children[i], startPage);
					}
				}
			}

			internal void EnterDocumentMapChildren()
			{
				DocumentMapNodeLists documentMapNodeLists = CurrentDocumentMapChildren;
				if (documentMapNodeLists == null)
				{
					DocumentMapNodeLists documentMapNodeLists3 = CurrentDocumentMapChildren = new DocumentMapNodeLists();
					documentMapNodeLists = documentMapNodeLists3;
					documentMapNodeLists.Add(null);
				}
				documentMapNodeLists.Add(null);
			}

			internal void AddToDocumentMap(int uniqueName, bool isContainer, int startPage, string label)
			{
				if (label != null)
				{
					DocumentMapNodeLists documentMapNodeLists = CurrentDocumentMapChildren;
					if (documentMapNodeLists == null)
					{
						DocumentMapNodeLists documentMapNodeLists3 = CurrentDocumentMapChildren = new DocumentMapNodeLists();
						documentMapNodeLists = documentMapNodeLists3;
						documentMapNodeLists.Add(null);
					}
					DocumentMapNodeList children = null;
					int num = documentMapNodeLists.Count - 1;
					if (isContainer)
					{
						Global.Tracer.Assert(1 < documentMapNodeLists.Count, "(1 < currentDocMap.Count)");
						children = documentMapNodeLists[documentMapNodeLists.Count - 1];
						num--;
					}
					DocumentMapNodeList documentMapNodeList = documentMapNodeLists[num];
					if (documentMapNodeList == null)
					{
						documentMapNodeList = (documentMapNodeLists[num] = new DocumentMapNodeList());
					}
					documentMapNodeList.Add(new DocumentMapNode(uniqueName.ToString(CultureInfo.InvariantCulture), label, startPage, children));
					if (isContainer)
					{
						documentMapNodeLists.RemoveAt(documentMapNodeLists.Count - 1);
					}
				}
			}

			internal string RegisterLabel(VariantResult labelResult)
			{
				string text = null;
				if (labelResult.ErrorOccurred)
				{
					text = RPRes.rsExpressionErrorValue;
				}
				else if (labelResult.Value != null)
				{
					if (labelResult.Value is string)
					{
						text = (string)labelResult.Value;
					}
					else
					{
						try
						{
							text = labelResult.Value.ToString();
						}
						catch (Exception ex)
						{
							if (AsynchronousExceptionDetection.IsStoppingException(ex))
							{
								throw;
							}
							Global.Tracer.Trace(TraceLevel.Verbose, ex.Message);
						}
					}
				}
				m_currentLabel = text;
				return text;
			}

			internal void ProcessBookmark(ProcessingContext processingContext, ReportItem reportItem, ReportItemInstance riInstance, ReportItemInstanceInfo riInstanceInfo)
			{
				string bookmark = processingContext.ReportRuntime.EvaluateReportItemBookmarkExpression(reportItem);
				ProcessBookmark(reportItem, riInstance, riInstanceInfo, bookmark);
			}

			internal void ProcessBookmark(ReportItem reportItem, ReportItemInstance riInstance, ReportItemInstanceInfo riInstanceInfo, string bookmark)
			{
				if (bookmark != null)
				{
					riInstanceInfo.Bookmark = bookmark;
					if (m_bookmarksInfo == null)
					{
						m_bookmarksInfo = new BookmarksHashtable();
					}
					m_bookmarksInfo.Add(bookmark, reportItem.StartPage, riInstance.UniqueName.ToString(CultureInfo.InvariantCulture));
				}
			}

			internal void ProcessBookmark(string bookmark, int startPage, int uniqueName)
			{
				if (bookmark != null)
				{
					if (m_bookmarksInfo == null)
					{
						m_bookmarksInfo = new BookmarksHashtable();
					}
					m_bookmarksInfo.Add(bookmark, startPage, uniqueName.ToString(CultureInfo.InvariantCulture));
				}
			}
		}

		internal sealed class RuntimeGroupingObj
		{
			internal enum GroupingTypes
			{
				None,
				Hash,
				Sort
			}

			private RuntimeHierarchyObj m_owner;

			private GroupingTypes m_type;

			private Hashtable m_hashtable;

			private BTreeNode m_tree;

			private ParentInformation m_parentInfo;

			internal BTreeNode Tree
			{
				get
				{
					return m_tree;
				}
				set
				{
					m_tree = value;
				}
			}

			internal GroupingTypes GroupingType
			{
				set
				{
					Global.Tracer.Assert(value == GroupingTypes.None, "(GroupingTypes.None == value)");
					m_type = value;
					m_tree = null;
					m_hashtable = null;
				}
			}

			internal RuntimeGroupingObj(RuntimeHierarchyObj owner, GroupingTypes type)
			{
				m_type = type;
				m_owner = owner;
				if (GroupingTypes.Sort == type)
				{
					m_tree = new BTreeNode(owner);
				}
				else
				{
					m_hashtable = new Hashtable(new ProcessingComparer(m_owner.ProcessingContext.CompareInfo, m_owner.ProcessingContext.ClrCompareOptions, nullsAsBlanks: false));
				}
			}

			internal void NextRow(object keyValue)
			{
				NextRow(keyValue, hasParent: false, null);
			}

			internal void NextRow(object keyValue, bool hasParent, object parentKey)
			{
				if (GroupingTypes.Sort == m_type)
				{
					m_tree.NextRow(keyValue);
					Global.Tracer.Assert(!hasParent, "(!hasParent)");
					return;
				}
				RuntimeHierarchyObj runtimeHierarchyObj = null;
				Global.Tracer.Assert(GroupingTypes.Hash == m_type, "(GroupingTypes.Hash == m_type)");
				try
				{
					runtimeHierarchyObj = (RuntimeHierarchyObj)m_hashtable[keyValue];
				}
				catch (ReportProcessingException_SpatialTypeComparisonError reportProcessingException_SpatialTypeComparisonError)
				{
					throw new ReportProcessingException(m_owner.RegisterSpatialTypeComparisonError(reportProcessingException_SpatialTypeComparisonError.Type));
				}
				catch (ReportProcessingException_ComparisonError e)
				{
					throw new ReportProcessingException(m_owner.RegisterComparisonError("GroupExpression", e));
				}
				if (runtimeHierarchyObj != null)
				{
					runtimeHierarchyObj.NextRow();
					return;
				}
				runtimeHierarchyObj = new RuntimeHierarchyObj(m_owner);
				m_hashtable.Add(keyValue, runtimeHierarchyObj);
				runtimeHierarchyObj.NextRow();
				if (hasParent)
				{
					RuntimeHierarchyObj runtimeHierarchyObj2 = null;
					RuntimeGroupLeafObj runtimeGroupLeafObj = null;
					try
					{
						runtimeHierarchyObj2 = (RuntimeHierarchyObj)m_hashtable[parentKey];
					}
					catch (ReportProcessingException_SpatialTypeComparisonError reportProcessingException_SpatialTypeComparisonError2)
					{
						throw new ReportProcessingException(m_owner.RegisterSpatialTypeComparisonError(reportProcessingException_SpatialTypeComparisonError2.Type));
					}
					catch (ReportProcessingException_ComparisonError e2)
					{
						throw new ReportProcessingException(m_owner.RegisterComparisonError("Parent", e2));
					}
					if (runtimeHierarchyObj2 != null)
					{
						Global.Tracer.Assert(runtimeHierarchyObj2.HierarchyObjs != null, "(null != parentHierarchyObj.HierarchyObjs)");
						runtimeGroupLeafObj = (RuntimeGroupLeafObj)runtimeHierarchyObj2.HierarchyObjs[0];
					}
					Global.Tracer.Assert(runtimeHierarchyObj.HierarchyObjs != null, "(null != hierarchyObj.HierarchyObjs)");
					RuntimeGroupLeafObj runtimeGroupLeafObj2 = (RuntimeGroupLeafObj)runtimeHierarchyObj.HierarchyObjs[0];
					bool addToWaitList = true;
					if (runtimeGroupLeafObj == runtimeGroupLeafObj2)
					{
						runtimeGroupLeafObj = null;
						addToWaitList = false;
					}
					ProcessChildren(keyValue, runtimeGroupLeafObj, runtimeGroupLeafObj2);
					ProcessParent(parentKey, runtimeGroupLeafObj, runtimeGroupLeafObj2, addToWaitList);
				}
			}

			internal void Traverse(ProcessingStages operation, bool ascending)
			{
				if (GroupingTypes.Sort == m_type)
				{
					m_tree.Traverse(operation, ascending);
				}
				else if (((RuntimeGroupRootObj)m_owner).FirstChild != null)
				{
					((RuntimeGroupRootObj)m_owner).FirstChild.TraverseAllLeafNodes(operation);
				}
			}

			private void ProcessParent(object parentKey, RuntimeGroupLeafObj parentObj, RuntimeGroupLeafObj childObj, bool addToWaitList)
			{
				if (parentObj != null)
				{
					parentObj.AddChild(childObj);
					return;
				}
				((RuntimeGroupRootObj)m_owner).AddChild(childObj);
				if (addToWaitList)
				{
					RuntimeGroupLeafObjList runtimeGroupLeafObjList = null;
					if (m_parentInfo == null)
					{
						m_parentInfo = new ParentInformation();
					}
					else
					{
						runtimeGroupLeafObjList = m_parentInfo[parentKey];
					}
					if (runtimeGroupLeafObjList == null)
					{
						runtimeGroupLeafObjList = new RuntimeGroupLeafObjList();
						m_parentInfo.Add(parentKey, runtimeGroupLeafObjList);
					}
					runtimeGroupLeafObjList.Add(childObj);
				}
			}

			private void ProcessChildren(object thisKey, RuntimeGroupLeafObj parentObj, RuntimeGroupLeafObj thisObj)
			{
				RuntimeGroupLeafObjList runtimeGroupLeafObjList = null;
				if (m_parentInfo != null)
				{
					runtimeGroupLeafObjList = m_parentInfo[thisKey];
				}
				if (runtimeGroupLeafObjList == null)
				{
					return;
				}
				for (int i = 0; i < runtimeGroupLeafObjList.Count; i++)
				{
					RuntimeGroupLeafObj runtimeGroupLeafObj = runtimeGroupLeafObjList[i];
					bool flag = false;
					RuntimeGroupLeafObj runtimeGroupLeafObj2 = parentObj;
					while (!flag && runtimeGroupLeafObj2 != null)
					{
						if (runtimeGroupLeafObj2 == runtimeGroupLeafObj)
						{
							flag = true;
						}
						runtimeGroupLeafObj2 = (runtimeGroupLeafObj2.Parent as RuntimeGroupLeafObj);
					}
					if (!flag)
					{
						runtimeGroupLeafObj.RemoveFromParent((RuntimeGroupRootObj)m_owner);
						thisObj.AddChild(runtimeGroupLeafObj);
					}
				}
				m_parentInfo.Remove(thisKey);
			}
		}

		internal sealed class BTreeNode
		{
			private const int BTreeOrder = 3;

			private BTreeNodeTupleList m_tuples;

			private BTreeNode m_parent;

			private int m_indexInParent;

			private IHierarchyObj m_owner;

			internal BTreeNode Parent
			{
				set
				{
					m_parent = value;
				}
			}

			internal int IndexInParent
			{
				set
				{
					m_indexInParent = value;
				}
			}

			internal BTreeNode(IHierarchyObj owner)
			{
				m_owner = owner;
				m_tuples = new BTreeNodeTupleList(this, 3);
				BTreeNodeTuple tuple = new BTreeNodeTuple(new BTreeNodeValue(null, owner), null);
				m_tuples.Add(tuple);
			}

			internal void NextRow(object keyValue)
			{
				try
				{
					SearchAndInsert(keyValue);
				}
				catch (ReportProcessingException_SpatialTypeComparisonError)
				{
					throw new ReportProcessingException(m_owner.RegisterComparisonError("SortExpression"));
				}
				catch (ReportProcessingException_ComparisonError)
				{
					throw new ReportProcessingException(m_owner.RegisterComparisonError("SortExpression"));
				}
			}

			internal void Traverse(ProcessingStages operation, bool ascending)
			{
				if (ascending)
				{
					for (int i = 0; i < m_tuples.Count; i++)
					{
						m_tuples[i].Traverse(operation, ascending);
					}
					return;
				}
				for (int num = m_tuples.Count - 1; num >= 0; num--)
				{
					m_tuples[num].Traverse(operation, ascending);
				}
			}

			private void SetFirstChild(BTreeNode child)
			{
				Global.Tracer.Assert(1 <= m_tuples.Count, "(1 <= m_tuples.Count)");
				m_tuples[0].Child = child;
				if (m_tuples[0].Child != null)
				{
					m_tuples[0].Child.Parent = this;
					m_tuples[0].Child.IndexInParent = 0;
				}
			}

			private void SearchAndInsert(object keyValue)
			{
				int num = -1;
				int i;
				for (i = 1; i < m_tuples.Count; i++)
				{
					num = m_tuples[i].Value.CompareTo(keyValue);
					if (num >= 0)
					{
						break;
					}
				}
				if (num == 0)
				{
					m_tuples[i].Value.AddRow();
				}
				else if (m_tuples[i - 1].Child == null)
				{
					InsertBTreeNode(new BTreeNodeValue(keyValue, m_owner), null, i);
				}
				else
				{
					m_tuples[i - 1].Child.SearchAndInsert(keyValue);
				}
			}

			private void InsertBTreeNode(BTreeNodeValue nodeValueToInsert, BTreeNode subTreeToInsert, int nodeIndexToInsert)
			{
				if (3 > m_tuples.Count)
				{
					m_tuples.Insert(nodeIndexToInsert, new BTreeNodeTuple(nodeValueToInsert, subTreeToInsert));
					return;
				}
				int num = 2;
				BTreeNode bTreeNode = new BTreeNode(m_owner);
				BTreeNodeValue bTreeNodeValue;
				if (num < nodeIndexToInsert)
				{
					bTreeNodeValue = m_tuples[num].Value;
					bTreeNode.SetFirstChild(m_tuples[num].Child);
					for (int i = num + 1; i < ((m_tuples.Count <= nodeIndexToInsert) ? m_tuples.Count : nodeIndexToInsert); i++)
					{
						bTreeNode.m_tuples.Add(m_tuples[i]);
					}
					bTreeNode.m_tuples.Add(new BTreeNodeTuple(nodeValueToInsert, subTreeToInsert));
					for (int j = nodeIndexToInsert; j < m_tuples.Count; j++)
					{
						bTreeNode.m_tuples.Add(m_tuples[j]);
					}
					int count = m_tuples.Count;
					for (int k = num; k < count; k++)
					{
						m_tuples.RemoveAtEnd();
					}
				}
				else if (num > nodeIndexToInsert)
				{
					bTreeNodeValue = m_tuples[num - 1].Value;
					bTreeNode.SetFirstChild(m_tuples[num - 1].Child);
					for (int l = num; l < m_tuples.Count; l++)
					{
						bTreeNode.m_tuples.Add(m_tuples[l]);
					}
					int count2 = m_tuples.Count;
					for (int m = num - 1; m < count2; m++)
					{
						m_tuples.RemoveAtEnd();
					}
					m_tuples.Insert(nodeIndexToInsert, new BTreeNodeTuple(nodeValueToInsert, subTreeToInsert));
				}
				else
				{
					bTreeNodeValue = nodeValueToInsert;
					bTreeNode.SetFirstChild(subTreeToInsert);
					for (int n = num; n < m_tuples.Count; n++)
					{
						bTreeNode.m_tuples.Add(m_tuples[n]);
					}
					int count3 = m_tuples.Count;
					for (int num2 = num; num2 < count3; num2++)
					{
						m_tuples.RemoveAtEnd();
					}
				}
				if (m_parent != null)
				{
					m_parent.InsertBTreeNode(bTreeNodeValue, bTreeNode, m_indexInParent + 1);
					return;
				}
				BTreeNode bTreeNode2 = new BTreeNode(m_owner);
				bTreeNode2.SetFirstChild(this);
				bTreeNode2.m_tuples.Add(new BTreeNodeTuple(bTreeNodeValue, bTreeNode));
				m_owner.SortTree = bTreeNode2;
			}
		}

		internal sealed class BTreeNodeTuple
		{
			private BTreeNodeValue m_value;

			private BTreeNode m_child;

			internal BTreeNodeValue Value => m_value;

			internal BTreeNode Child
			{
				get
				{
					return m_child;
				}
				set
				{
					m_child = value;
				}
			}

			internal BTreeNodeTuple(BTreeNodeValue value, BTreeNode child)
			{
				m_value = value;
				m_child = child;
			}

			internal void Traverse(ProcessingStages operation, bool ascending)
			{
				if (ascending)
				{
					if (m_value != null)
					{
						m_value.Traverse(operation);
					}
					if (m_child != null)
					{
						m_child.Traverse(operation, ascending);
					}
				}
				else
				{
					if (m_child != null)
					{
						m_child.Traverse(operation, ascending);
					}
					if (m_value != null)
					{
						m_value.Traverse(operation);
					}
				}
			}
		}

		internal sealed class BTreeNodeValue
		{
			private object m_key;

			private IHierarchyObj m_hierarchyNode;

			internal BTreeNodeValue(object key, IHierarchyObj owner)
			{
				m_key = key;
				if (key != null)
				{
					m_hierarchyNode = owner.CreateHierarchyObj();
					m_hierarchyNode.NextRow();
				}
			}

			internal void AddRow()
			{
				m_hierarchyNode.NextRow();
			}

			internal void Traverse(ProcessingStages operation)
			{
				if (m_hierarchyNode != null)
				{
					m_hierarchyNode.Traverse(operation);
				}
			}

			internal int CompareTo(object keyValue)
			{
				return ReportProcessing.CompareTo(m_key, keyValue, m_hierarchyNode.ProcessingContext.CompareInfo, m_hierarchyNode.ProcessingContext.ClrCompareOptions);
			}
		}

		private sealed class BTreeNodeTupleList
		{
			private ArrayList m_list;

			private int m_capacity;

			private BTreeNode m_owner;

			internal BTreeNodeTuple this[int index] => (BTreeNodeTuple)m_list[index];

			internal int Count => m_list.Count;

			internal BTreeNodeTupleList(BTreeNode owner, int capacity)
			{
				m_owner = owner;
				m_list = new ArrayList(capacity);
				m_capacity = capacity;
			}

			internal void Add(BTreeNodeTuple tuple)
			{
				if (m_list.Count == m_capacity)
				{
					throw new ReportProcessingException(ErrorCode.rsInvalidOperation);
				}
				m_list.Add(tuple);
				if (tuple.Child != null)
				{
					tuple.Child.Parent = m_owner;
					tuple.Child.IndexInParent = m_list.Count - 1;
				}
			}

			internal void Insert(int index, BTreeNodeTuple tuple)
			{
				if (m_list.Count == m_capacity)
				{
					throw new ReportProcessingException(ErrorCode.rsInvalidOperation);
				}
				m_list.Insert(index, tuple);
				if (tuple.Child != null)
				{
					tuple.Child.Parent = m_owner;
				}
				for (int i = index; i < m_list.Count; i++)
				{
					BTreeNode child = ((BTreeNodeTuple)m_list[i]).Child;
					if (child != null)
					{
						child.IndexInParent = i;
					}
				}
			}

			internal void RemoveAtEnd()
			{
				m_list.RemoveAt(m_list.Count - 1);
			}
		}

		internal sealed class RuntimeHierarchyObjList : ArrayList
		{
			internal new RuntimeHierarchyObj this[int index] => (RuntimeHierarchyObj)base[index];
		}

		internal sealed class DataRowList : ArrayList
		{
			internal new FieldImpl[] this[int index] => (FieldImpl[])base[index];
		}

		internal sealed class PageTextboxes
		{
			private ArrayList m_pages;

			internal PageTextboxes()
			{
				m_pages = new ArrayList();
			}

			internal int GetPageCount()
			{
				return m_pages.Count;
			}

			internal void IntegrateRepeatingTextboxValues(PageTextboxes source, int targetStartPage, int targetEndPage)
			{
				if (source == null || targetStartPage > targetEndPage)
				{
					return;
				}
				int num = source.GetPageCount() - 1;
				if (num < 0)
				{
					return;
				}
				Hashtable textboxesOnPage = source.GetTextboxesOnPage(num);
				if (textboxesOnPage == null)
				{
					return;
				}
				for (int i = targetStartPage; i <= targetEndPage; i++)
				{
					IDictionaryEnumerator enumerator = textboxesOnPage.GetEnumerator();
					while (enumerator.MoveNext())
					{
						AddTextboxValue(i, enumerator.Key as string, enumerator.Value as ArrayList);
					}
				}
			}

			internal void IntegrateNonRepeatingTextboxValues(PageTextboxes source)
			{
				if (source == null)
				{
					return;
				}
				int pageCount = source.GetPageCount();
				for (int i = 0; i < pageCount; i++)
				{
					Hashtable textboxesOnPage = source.GetTextboxesOnPage(i);
					if (textboxesOnPage != null)
					{
						IDictionaryEnumerator enumerator = textboxesOnPage.GetEnumerator();
						while (enumerator.MoveNext())
						{
							AddTextboxValue(i, enumerator.Key as string, enumerator.Value as ArrayList);
						}
					}
				}
			}

			internal void AddTextboxValue(int page, string name, object value)
			{
				ArrayList arrayList = new ArrayList();
				arrayList.Add(value);
				AddTextboxValue(page, name, arrayList);
			}

			internal void AddTextboxValue(int page, string name, ArrayList values)
			{
				Global.Tracer.Assert(0 <= page && name != null && values != null, "(0 <= page && null != name && null != values)");
				if (0 > page)
				{
					return;
				}
				int count = m_pages.Count;
				if (count <= page)
				{
					for (int i = count; i <= page; i++)
					{
						m_pages.Add(null);
					}
				}
				Hashtable hashtable = m_pages[page] as Hashtable;
				if (hashtable == null)
				{
					hashtable = new Hashtable();
					m_pages[page] = hashtable;
				}
				ArrayList arrayList = hashtable[name] as ArrayList;
				if (arrayList == null)
				{
					arrayList = new ArrayList();
					hashtable.Add(name, arrayList);
				}
				arrayList.AddRange(values);
			}

			internal Hashtable GetTextboxesOnPage(int page)
			{
				Global.Tracer.Assert(0 <= page, "(0 <= page)");
				if (page >= m_pages.Count)
				{
					return null;
				}
				return m_pages[page] as Hashtable;
			}

			internal ArrayList GetTextboxValues(int page, string name)
			{
				Global.Tracer.Assert(0 <= page && name != null, "(0 <= page && null != name)");
				Hashtable textboxesOnPage = GetTextboxesOnPage(page);
				if (textboxesOnPage == null)
				{
					return null;
				}
				return textboxesOnPage[name] as ArrayList;
			}
		}

		internal sealed class PageSectionContext
		{
			private bool m_needPageSectionEvaluation;

			private bool m_isOnePass;

			private PageTextboxes m_pageTextboxes;

			private List<bool> m_pageItemVisibility;

			private List<bool> m_matrixColumnVisibility;

			private List<bool> m_matrixRowVisibility;

			private List<bool> m_matrixInColumnHeader;

			private ArrayList m_tableColumnVisibility;

			private IntList m_tableColumnPosition;

			private IntList m_tableColumnSpans;

			private List<PageTextboxes> m_repeatingItemList;

			private bool m_inMatrixSubtotal;

			private bool m_inMatrixCell;

			private int m_subreportLevel;

			internal PageTextboxes PageTextboxes
			{
				get
				{
					return m_pageTextboxes;
				}
				set
				{
					m_pageTextboxes = value;
				}
			}

			internal bool InMatrixSubtotal
			{
				get
				{
					return m_inMatrixSubtotal;
				}
				set
				{
					m_inMatrixSubtotal = value;
				}
			}

			internal bool InMatrixCell
			{
				get
				{
					return m_inMatrixCell;
				}
				set
				{
					m_inMatrixCell = value;
				}
			}

			internal bool InSubreport => m_subreportLevel != 0;

			internal bool InRepeatingItem
			{
				get
				{
					if (m_repeatingItemList == null)
					{
						return false;
					}
					return m_repeatingItemList.Count > 1;
				}
			}

			internal IntList TableColumnSpans
			{
				get
				{
					return m_tableColumnSpans;
				}
				set
				{
					m_tableColumnSpans = value;
				}
			}

			internal bool HasPageSections => m_needPageSectionEvaluation;

			internal PageSectionContext(bool hasPageSections, bool isOnePass)
			{
				ConstructorHelper(hasPageSections, isOnePass, parentVisible: true, inMatrixSubtotal: false, inMatrixCell: false, 0);
			}

			internal PageSectionContext(PageSectionContext copy)
			{
				ConstructorHelper(copy.m_needPageSectionEvaluation, copy.m_isOnePass, copy.IsParentVisible(), copy.InMatrixSubtotal, copy.InMatrixCell, copy.m_subreportLevel);
			}

			private void ConstructorHelper(bool needPageSectionEvaluation, bool isOnePass, bool parentVisible, bool inMatrixSubtotal, bool inMatrixCell, int subreportLevel)
			{
				m_needPageSectionEvaluation = needPageSectionEvaluation;
				m_isOnePass = isOnePass;
				if (needPageSectionEvaluation)
				{
					m_pageTextboxes = new PageTextboxes();
					m_inMatrixSubtotal = inMatrixSubtotal;
					m_inMatrixCell = inMatrixCell;
					m_subreportLevel = subreportLevel;
					m_repeatingItemList = new List<PageTextboxes>();
					m_repeatingItemList.Add(m_pageTextboxes);
				}
				if ((isOnePass || needPageSectionEvaluation) && !parentVisible)
				{
					m_pageItemVisibility = new List<bool>();
					m_pageItemVisibility.Add(item: false);
				}
			}

			internal void EnterSubreport()
			{
				if (m_needPageSectionEvaluation || m_isOnePass)
				{
					m_subreportLevel++;
				}
			}

			internal void ExitSubreport()
			{
				if (m_needPageSectionEvaluation || m_isOnePass)
				{
					m_subreportLevel--;
				}
			}

			internal void EnterRepeatingItem()
			{
				if (m_needPageSectionEvaluation)
				{
					m_repeatingItemList.Insert(0, m_pageTextboxes);
					m_pageTextboxes = new PageTextboxes();
				}
			}

			internal PageTextboxes ExitRepeatingItem()
			{
				if (!m_needPageSectionEvaluation)
				{
					return null;
				}
				PageTextboxes pageTextboxes = m_pageTextboxes;
				m_pageTextboxes = m_repeatingItemList[0];
				m_repeatingItemList.RemoveAt(0);
				return pageTextboxes;
			}

			internal void RegisterTableColumnVisibility(bool isOnePass, TableColumnList columns, bool[] columnsStartHidden)
			{
				if (!m_isOnePass && (!m_needPageSectionEvaluation || InSubreport))
				{
					return;
				}
				Global.Tracer.Assert(columns != null, "(null != columns)");
				bool[] array;
				if (isOnePass && columnsStartHidden == null)
				{
					array = null;
				}
				else
				{
					Global.Tracer.Assert(columns.Count == columnsStartHidden.Length, "(columns.Count == columnsStartHidden.Length)");
					array = new bool[columnsStartHidden.Length];
					for (int i = 0; i < columnsStartHidden.Length; i++)
					{
						array[i] = Visibility.IsVisible(columns[i].Visibility, columnsStartHidden[i]);
					}
				}
				if (m_tableColumnVisibility == null)
				{
					m_tableColumnVisibility = new ArrayList();
					m_tableColumnPosition = new IntList();
				}
				m_tableColumnVisibility.Insert(0, array);
				m_tableColumnPosition.Insert(0, -1);
			}

			internal void UnregisterTableColumnVisibility()
			{
				if (m_isOnePass || (m_needPageSectionEvaluation && !InSubreport))
				{
					Global.Tracer.Assert(m_tableColumnVisibility != null && m_tableColumnPosition != null && m_tableColumnPosition.Count > 0 && m_tableColumnPosition.Count == m_tableColumnPosition.Count);
					m_tableColumnVisibility.RemoveAt(0);
					m_tableColumnPosition.RemoveAt(0);
					if (m_tableColumnPosition.Count == 0)
					{
						m_tableColumnPosition = null;
						m_tableColumnVisibility = null;
					}
				}
			}

			internal void SetTableCellIndex(bool isOnePass, int position)
			{
				if (m_isOnePass || (m_needPageSectionEvaluation && !InSubreport))
				{
					if (isOnePass && m_tableColumnPosition == null)
					{
						m_tableColumnPosition = new IntList();
						m_tableColumnPosition.Add(-1);
						m_tableColumnVisibility = new ArrayList();
						m_tableColumnVisibility.Add(null);
					}
					Global.Tracer.Assert(m_tableColumnPosition != null && position >= 0);
					m_tableColumnPosition[0] = position;
				}
			}

			internal TableColumnInfo GetOnePassTableCellProperties()
			{
				if (!m_isOnePass && (!m_needPageSectionEvaluation || InSubreport))
				{
					return default(TableColumnInfo);
				}
				Global.Tracer.Assert(m_tableColumnPosition != null, "(null != m_tableColumnPosition)");
				return GetTableCellProperties(m_tableColumnPosition[0]);
			}

			internal bool IsTableColumnVisible(TableColumnInfo columnInfo)
			{
				if (!m_isOnePass && (!m_needPageSectionEvaluation || InSubreport))
				{
					return false;
				}
				Global.Tracer.Assert(m_tableColumnVisibility != null && 0 < m_tableColumnVisibility.Count);
				return Visibility.IsTableCellVisible(m_tableColumnVisibility[0] as bool[], columnInfo.StartIndex, columnInfo.Span);
			}

			internal void EnterVisibilityScope(Visibility visibility, bool startHidden)
			{
				if (m_isOnePass || m_needPageSectionEvaluation)
				{
					if (m_pageItemVisibility == null)
					{
						m_pageItemVisibility = new List<bool>();
					}
					bool flag = true;
					if (0 < m_pageItemVisibility.Count)
					{
						flag = m_pageItemVisibility[0];
					}
					if (flag)
					{
						m_pageItemVisibility.Insert(0, Visibility.IsVisible(visibility, startHidden));
					}
					else
					{
						m_pageItemVisibility.Insert(0, item: false);
					}
				}
			}

			internal void ExitVisibilityScope()
			{
				if (m_isOnePass || m_needPageSectionEvaluation)
				{
					Global.Tracer.Assert(m_pageItemVisibility != null && m_pageItemVisibility.Count > 0, "(null != m_pageItemVisibility && m_pageItemVisibility.Count > 0)");
					m_pageItemVisibility.RemoveAt(0);
				}
			}

			private bool IsMatrixHeadingVisible()
			{
				if (!m_isOnePass && !m_needPageSectionEvaluation)
				{
					return false;
				}
				if (m_matrixInColumnHeader == null)
				{
					return true;
				}
				Global.Tracer.Assert(0 < m_matrixInColumnHeader.Count, "(0 < m_matrixInColumnHeader.Count)");
				if (m_matrixInColumnHeader[0])
				{
					return m_matrixColumnVisibility[0];
				}
				return m_matrixRowVisibility[0];
			}

			private bool IsMatrixCellVisible()
			{
				if (!m_isOnePass && !m_needPageSectionEvaluation)
				{
					return false;
				}
				if (m_matrixInColumnHeader == null)
				{
					return true;
				}
				bool flag = true;
				if (m_matrixColumnVisibility != null)
				{
					flag &= m_matrixColumnVisibility[0];
				}
				if (m_matrixRowVisibility != null)
				{
					flag &= m_matrixRowVisibility[0];
				}
				return flag;
			}

			private bool IsParentMatrixHeadingVisible(bool newMatrixHeadingColumn)
			{
				if (!m_isOnePass && !m_needPageSectionEvaluation)
				{
					return false;
				}
				if (m_matrixInColumnHeader == null)
				{
					return true;
				}
				Global.Tracer.Assert(0 < m_matrixInColumnHeader.Count, "(0 < m_matrixInColumnHeader.Count)");
				if (m_matrixInColumnHeader[0] == newMatrixHeadingColumn)
				{
					return IsMatrixHeadingVisible();
				}
				return true;
			}

			internal void EnterMatrixSubtotalScope(bool isColumn)
			{
				if (m_needPageSectionEvaluation)
				{
					if (m_matrixInColumnHeader == null)
					{
						m_matrixInColumnHeader = new List<bool>();
						m_matrixColumnVisibility = new List<bool>();
						m_matrixRowVisibility = new List<bool>();
					}
					m_matrixInColumnHeader.Insert(0, isColumn);
					if (isColumn)
					{
						m_matrixColumnVisibility.Insert(0, item: false);
					}
					else
					{
						m_matrixRowVisibility.Insert(0, item: false);
					}
				}
			}

			internal void EnterMatrixHeadingScope(bool currentHeadingIsVisible, bool isColumn)
			{
				if (m_needPageSectionEvaluation && !InSubreport)
				{
					bool flag = IsParentMatrixHeadingVisible(isColumn);
					if (m_matrixInColumnHeader == null)
					{
						m_matrixInColumnHeader = new List<bool>();
						m_matrixColumnVisibility = new List<bool>();
						m_matrixRowVisibility = new List<bool>();
					}
					m_matrixInColumnHeader.Insert(0, isColumn);
					if (isColumn)
					{
						m_matrixColumnVisibility.Insert(0, flag && currentHeadingIsVisible);
					}
					else
					{
						m_matrixRowVisibility.Insert(0, flag && currentHeadingIsVisible);
					}
				}
			}

			internal void ExitMatrixHeadingScope(bool isColumn)
			{
				if (m_needPageSectionEvaluation && !InSubreport)
				{
					Global.Tracer.Assert(m_matrixInColumnHeader != null && 0 < m_matrixInColumnHeader.Count, "(null != m_matrixInColumnHeader && 0 < m_matrixInColumnHeader.Count)");
					if (isColumn)
					{
						m_matrixColumnVisibility.RemoveAt(0);
					}
					else
					{
						m_matrixRowVisibility.RemoveAt(0);
					}
					m_matrixInColumnHeader.RemoveAt(0);
					if (m_matrixInColumnHeader.Count == 0)
					{
						m_matrixInColumnHeader = null;
						m_matrixColumnVisibility = null;
						m_matrixRowVisibility = null;
					}
				}
			}

			internal bool IsParentVisible()
			{
				if (!m_isOnePass && !m_needPageSectionEvaluation)
				{
					return false;
				}
				if (m_subreportLevel != 0 || m_inMatrixSubtotal)
				{
					return false;
				}
				if (!IsMatrixHeadingVisible())
				{
					return false;
				}
				if (m_inMatrixCell && !IsMatrixCellVisible())
				{
					return false;
				}
				if (m_pageItemVisibility != null && 0 < m_pageItemVisibility.Count)
				{
					if (m_tableColumnVisibility != null && 0 < m_tableColumnVisibility.Count && m_tableColumnVisibility[0] != null && m_tableColumnSpans != null)
					{
						TableColumnInfo tableCellProperties = GetTableCellProperties(m_tableColumnPosition[0]);
						if (m_pageItemVisibility[0])
						{
							return Visibility.IsTableCellVisible(m_tableColumnVisibility[0] as bool[], tableCellProperties.StartIndex, tableCellProperties.Span);
						}
						return false;
					}
					return m_pageItemVisibility[0];
				}
				return true;
			}

			internal TableColumnInfo GetTableCellProperties(int cellIndex)
			{
				if (cellIndex < 0 || m_tableColumnSpans == null)
				{
					return default(TableColumnInfo);
				}
				Global.Tracer.Assert(cellIndex < m_tableColumnSpans.Count, "(cellIndex < m_tableColumnSpans.Count)");
				TableColumnInfo result = default(TableColumnInfo);
				result.StartIndex = 0;
				for (int i = 0; i < cellIndex; i++)
				{
					result.StartIndex += m_tableColumnSpans[i];
				}
				result.Span = m_tableColumnSpans[cellIndex];
				return result;
			}
		}

		internal sealed class AggregateRow
		{
			private FieldImpl[] m_fields;

			private bool m_isAggregateRow;

			private int m_aggregationFieldCount;

			private bool m_validAggregateRow;

			internal AggregateRow(ProcessingContext processingContext)
			{
				FieldsImpl fieldsImpl = processingContext.ReportObjectModel.FieldsImpl;
				m_fields = fieldsImpl.GetAndSaveFields();
				m_isAggregateRow = fieldsImpl.IsAggregateRow;
				m_aggregationFieldCount = fieldsImpl.AggregationFieldCount;
				m_validAggregateRow = fieldsImpl.ValidAggregateRow;
			}

			internal void SetFields(ProcessingContext processingContext)
			{
				processingContext.ReportObjectModel.FieldsImpl.SetFields(m_fields, m_isAggregateRow, m_aggregationFieldCount, m_validAggregateRow);
			}
		}

		internal sealed class AggregateRowList : ArrayList
		{
			internal new AggregateRow this[int index] => (AggregateRow)base[index];
		}

		internal sealed class AggregateRowInfo
		{
			private bool[] m_aggregationFieldChecked;

			private int m_aggregationFieldCount;

			private bool m_validAggregateRow;

			internal void SaveAggregateInfo(ProcessingContext processingContext)
			{
				FieldsImpl fieldsImpl = processingContext.ReportObjectModel.FieldsImpl;
				m_aggregationFieldCount = fieldsImpl.AggregationFieldCount;
				if (m_aggregationFieldChecked == null)
				{
					m_aggregationFieldChecked = new bool[fieldsImpl.Count];
				}
				for (int i = 0; i < fieldsImpl.Count; i++)
				{
					m_aggregationFieldChecked[i] = fieldsImpl[i].AggregationFieldChecked;
				}
				m_validAggregateRow = fieldsImpl.ValidAggregateRow;
			}

			internal void RestoreAggregateInfo(ProcessingContext processingContext)
			{
				FieldsImpl fieldsImpl = processingContext.ReportObjectModel.FieldsImpl;
				fieldsImpl.AggregationFieldCount = m_aggregationFieldCount;
				Global.Tracer.Assert(m_aggregationFieldChecked != null, "(null != m_aggregationFieldChecked)");
				for (int i = 0; i < fieldsImpl.Count; i++)
				{
					fieldsImpl[i].AggregationFieldChecked = m_aggregationFieldChecked[i];
				}
				fieldsImpl.ValidAggregateRow = m_validAggregateRow;
			}

			internal void CombineAggregateInfo(ProcessingContext processingContext, AggregateRowInfo updated)
			{
				FieldsImpl fieldsImpl = processingContext.ReportObjectModel.FieldsImpl;
				if (updated == null)
				{
					fieldsImpl.ValidAggregateRow = false;
					return;
				}
				if (!updated.m_validAggregateRow)
				{
					fieldsImpl.ValidAggregateRow = false;
				}
				for (int i = 0; i < fieldsImpl.Count; i++)
				{
					if (updated.m_aggregationFieldChecked[i] && !fieldsImpl[i].AggregationFieldChecked)
					{
						fieldsImpl[i].AggregationFieldChecked = true;
						fieldsImpl.AggregationFieldCount--;
					}
				}
			}
		}

		private sealed class RuntimeRICollectionList : ArrayList
		{
			internal new RuntimeRICollection this[int index] => (RuntimeRICollection)base[index];

			internal RuntimeRICollectionList()
			{
			}

			internal RuntimeRICollectionList(int capacity)
				: base(capacity)
			{
			}

			internal void FirstPassNextDataRow()
			{
				for (int i = 0; i < Count; i++)
				{
					this[i].FirstPassNextDataRow();
				}
			}

			internal void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
				for (int i = 0; i < Count; i++)
				{
					this[i].CalculateRunningValues(globalRVCol, groupCol, lastGroup);
				}
			}

			internal void CalculatePreviousAggregates(AggregatesImpl globalRVCol)
			{
				for (int i = 0; i < Count; i++)
				{
					this[i].CalculatePreviousAggregates(globalRVCol);
				}
			}

			internal void ResetReportItemObjs()
			{
				for (int i = 0; i < Count; i++)
				{
					this[i].ResetReportItemObjs();
				}
			}
		}

		internal interface IFilterOwner
		{
			void PostFilterNextRow();
		}

		internal interface IScope
		{
			bool TargetForNonDetailSort
			{
				get;
			}

			int[] SortFilterExpressionScopeInfoIndices
			{
				get;
			}

			bool IsTargetForSort(int index, bool detailSort);

			void ReadRow(DataActions dataAction);

			bool InScope(string scope);

			IScope GetOuterScope(bool includeSubReportContainingScope);

			string GetScopeName();

			int RecursiveLevel(string scope);

			bool TargetScopeMatched(int index, bool detailSort);

			void GetScopeValues(IHierarchyObj targetScopeObj, VariantList[] scopeValues, ref int index);

			void GetGroupNameValuePairs(Dictionary<string, object> pairs);
		}

		internal interface IHierarchyObj
		{
			IHierarchyObj HierarchyRoot
			{
				get;
			}

			ProcessingContext ProcessingContext
			{
				get;
			}

			BTreeNode SortTree
			{
				get;
				set;
			}

			int ExpressionIndex
			{
				get;
			}

			IntList SortFilterInfoIndices
			{
				get;
			}

			bool IsDetail
			{
				get;
			}

			IHierarchyObj CreateHierarchyObj();

			ProcessingMessageList RegisterComparisonError(string propertyName);

			void NextRow();

			void Traverse(ProcessingStages operation);

			void ReadRow();

			void ProcessUserSort();

			void MarkSortInfoProcessed(RuntimeSortFilterEventInfoList runtimeSortFilterInfo);

			void AddSortInfoIndex(int sortInfoIndex, RuntimeSortFilterEventInfo sortInfo);
		}

		internal interface ISortDataHolder
		{
			void NextRow();

			void Traverse(ProcessingStages operation);
		}

		private sealed class RuntimeDRCollection
		{
			private RuntimeDataRegionObjList m_dataRegionObjs;

			private ProcessingContext m_processingContext;

			internal RuntimeDRCollection(IScope outerScope, DataRegionList dataRegionDefs, ProcessingContext processingContext, bool onePass)
			{
				m_processingContext = processingContext;
				m_dataRegionObjs = new RuntimeDataRegionObjList();
				CreateDataRegions(outerScope, dataRegionDefs, onePass);
			}

			private void CreateDataRegions(IScope outerScope, DataRegionList dataRegionDefs, bool onePass)
			{
				DataActions dataAction = DataActions.None;
				for (int i = 0; i < dataRegionDefs.Count; i++)
				{
					DataRegion dataRegion = dataRegionDefs[i];
					RuntimeDataRegionObj runtimeDataRegionObj = null;
					if (dataRegion is List)
					{
						List list = (List)dataRegion;
						if (!onePass)
						{
							runtimeDataRegionObj = (RuntimeDataRegionObj)((list.Grouping == null) ? ((object)new RuntimeListDetailObj(outerScope, list, ref dataAction, m_processingContext)) : ((object)new RuntimeListGroupRootObj(outerScope, list, ref dataAction, m_processingContext)));
						}
						else
						{
							Global.Tracer.Assert(list.Grouping == null, "(null == list.Grouping)");
							runtimeDataRegionObj = new RuntimeOnePassListDetailObj(outerScope, list, m_processingContext);
						}
					}
					else if (dataRegion is Matrix)
					{
						runtimeDataRegionObj = new RuntimeMatrixObj(outerScope, (Matrix)dataRegion, ref dataAction, m_processingContext, onePass);
					}
					else if (dataRegion is Chart)
					{
						runtimeDataRegionObj = new RuntimeChartObj(outerScope, (Chart)dataRegion, ref dataAction, m_processingContext, onePass);
					}
					else if (dataRegion is Table)
					{
						runtimeDataRegionObj = new RuntimeTableObj(outerScope, (Table)dataRegion, ref dataAction, m_processingContext, onePass);
					}
					else if (dataRegion is CustomReportItem)
					{
						CustomReportItem customReportItem = dataRegion as CustomReportItem;
						Global.Tracer.Assert(customReportItem != null, "(null != crItem)");
						if (customReportItem.DataSetName != null)
						{
							runtimeDataRegionObj = new RuntimeCustomReportItemObj(outerScope, customReportItem, ref dataAction, m_processingContext, onePass);
						}
					}
					else if (dataRegion is OWCChart)
					{
						OWCChart oWCChart = (OWCChart)dataRegion;
						oWCChart.SetExprHost(m_processingContext.ReportRuntime.ReportExprHost, m_processingContext.ReportObjectModel);
						runtimeDataRegionObj = (RuntimeDataRegionObj)((!onePass) ? ((object)new RuntimeOWCChartDetailObj(outerScope, oWCChart, ref dataAction, m_processingContext)) : ((object)new RuntimeOnePassOWCChartDetailObj(outerScope, oWCChart, m_processingContext)));
					}
					if (runtimeDataRegionObj != null)
					{
						m_dataRegionObjs.Add(runtimeDataRegionObj);
						dataRegion.RuntimeDataRegionObj = runtimeDataRegionObj;
					}
				}
			}

			internal void FirstPassNextDataRow()
			{
				AggregateRowInfo aggregateRowInfo = new AggregateRowInfo();
				aggregateRowInfo.SaveAggregateInfo(m_processingContext);
				for (int i = 0; i < m_dataRegionObjs.Count; i++)
				{
					m_dataRegionObjs[i].NextRow();
					aggregateRowInfo.RestoreAggregateInfo(m_processingContext);
				}
			}

			internal void SortAndFilter()
			{
				for (int i = 0; i < m_dataRegionObjs.Count; i++)
				{
					RuntimeDataRegionObj runtimeDataRegionObj = m_dataRegionObjs[i];
					if (!(runtimeDataRegionObj is RuntimeDetailObj))
					{
						runtimeDataRegionObj.SortAndFilter();
					}
				}
			}

			internal void CalculateRunningValues(AggregatesImpl globalRunningValueCollection, RuntimeGroupRootObjList groupCollection)
			{
				for (int i = 0; i < m_dataRegionObjs.Count; i++)
				{
					m_dataRegionObjs[i].CalculateRunningValues(globalRunningValueCollection, groupCollection, null);
				}
			}
		}

		internal sealed class RuntimeRICollection
		{
			internal enum SubReportInitialization
			{
				AssignIDsOnly,
				RuntimeOnly,
				All
			}

			private IScope m_owner;

			private ReportItemCollection m_reportItemsDef;

			private ProcessingContext m_processingContext;

			private RuntimeDataRegionObjList m_dataRegionObjs;

			private DataAggregateObjResult[] m_runningValueValues;

			private int m_currDataRegion;

			internal ReportItemCollection ReportItemsDef => m_reportItemsDef;

			internal RuntimeRICollection(IScope owner, ReportItemCollection RIColDef, ref DataActions dataAction, ProcessingContext processingContext, bool createDataRegions)
			{
				m_owner = owner;
				m_reportItemsDef = RIColDef;
				m_processingContext = processingContext;
				if (createDataRegions && RIColDef != null)
				{
					CreateDataRegions(owner, RIColDef.ComputedReportItems, ref dataAction);
				}
			}

			internal RuntimeRICollection(IScope owner, ReportItemCollection RIColDef, ProcessingContext processingContext, bool createDataRegions)
			{
				m_owner = owner;
				m_reportItemsDef = RIColDef;
				m_processingContext = processingContext;
				if (createDataRegions)
				{
					CreateDataRegions(owner, RIColDef.ComputedReportItems);
				}
			}

			private void CreateDataRegions(IScope owner, ReportItemList computedRIs, ref DataActions dataAction)
			{
				if (computedRIs == null)
				{
					return;
				}
				RuntimeDataRegionObj runtimeDataRegionObj = null;
				for (int i = 0; i < computedRIs.Count; i++)
				{
					ReportItem reportItem = computedRIs[i];
					runtimeDataRegionObj = null;
					if (reportItem is Rectangle)
					{
						ReportItemCollection reportItems = ((Rectangle)reportItem).ReportItems;
						if (reportItems != null && reportItems.ComputedReportItems != null)
						{
							CreateDataRegions(owner, reportItems.ComputedReportItems, ref dataAction);
						}
					}
					else if (reportItem is DataRegion && !(owner is RuntimeDetailObj))
					{
						if (reportItem is List)
						{
							List list = (List)reportItem;
							runtimeDataRegionObj = (RuntimeDataRegionObj)((list.Grouping == null) ? ((object)new RuntimeListDetailObj(owner, list, ref dataAction, m_processingContext)) : ((object)new RuntimeListGroupRootObj(owner, list, ref dataAction, m_processingContext)));
						}
						else if (reportItem is Matrix)
						{
							runtimeDataRegionObj = new RuntimeMatrixObj(owner, (Matrix)reportItem, ref dataAction, m_processingContext, onePassProcess: false);
						}
						else if (reportItem is Table)
						{
							runtimeDataRegionObj = new RuntimeTableObj(owner, (Table)reportItem, ref dataAction, m_processingContext, onePassProcess: false);
						}
						else if (reportItem is Chart)
						{
							runtimeDataRegionObj = new RuntimeChartObj(owner, (Chart)reportItem, ref dataAction, m_processingContext, onePassProcess: false);
						}
						else if (reportItem is OWCChart)
						{
							runtimeDataRegionObj = new RuntimeOWCChartDetailObj(owner, (OWCChart)reportItem, ref dataAction, m_processingContext);
						}
						else if (reportItem is CustomReportItem && ((CustomReportItem)reportItem).DataSetName != null)
						{
							runtimeDataRegionObj = new RuntimeCustomReportItemObj(owner, (CustomReportItem)reportItem, ref dataAction, m_processingContext, onePassProcess: false);
						}
					}
					if (runtimeDataRegionObj != null)
					{
						if (m_dataRegionObjs == null)
						{
							m_dataRegionObjs = new RuntimeDataRegionObjList();
						}
						m_dataRegionObjs.Add(runtimeDataRegionObj);
					}
				}
			}

			private void CreateDataRegions(IScope owner, ReportItemList computedRIs)
			{
				if (computedRIs == null)
				{
					return;
				}
				DataActions dataAction = DataActions.None;
				RuntimeDataRegionObj runtimeDataRegionObj = null;
				for (int i = 0; i < computedRIs.Count; i++)
				{
					ReportItem reportItem = computedRIs[i];
					runtimeDataRegionObj = null;
					if (reportItem is Rectangle)
					{
						ReportItemCollection reportItems = ((Rectangle)reportItem).ReportItems;
						if (reportItems != null && reportItems.ComputedReportItems != null)
						{
							CreateDataRegions(owner, reportItems.ComputedReportItems);
						}
					}
					else if (reportItem is DataRegion)
					{
						if (reportItem is List)
						{
							List list = (List)reportItem;
							Global.Tracer.Assert(list.Grouping == null, "(null == list.Grouping)");
							runtimeDataRegionObj = new RuntimeOnePassListDetailObj(owner, list, m_processingContext);
						}
						else if (reportItem is Matrix)
						{
							runtimeDataRegionObj = new RuntimeMatrixObj(owner, (Matrix)reportItem, ref dataAction, m_processingContext, onePassProcess: true);
						}
						else if (reportItem is Table)
						{
							runtimeDataRegionObj = new RuntimeTableObj(owner, (Table)reportItem, ref dataAction, m_processingContext, onePassProcess: true);
						}
						else if (reportItem is Chart)
						{
							runtimeDataRegionObj = new RuntimeChartObj(owner, (Chart)reportItem, ref dataAction, m_processingContext, onePassProcess: true);
						}
						else if (reportItem is OWCChart)
						{
							runtimeDataRegionObj = new RuntimeOnePassOWCChartDetailObj(owner, (OWCChart)reportItem, m_processingContext);
						}
						else if (reportItem is CustomReportItem)
						{
							runtimeDataRegionObj = new RuntimeCustomReportItemObj(owner, (CustomReportItem)reportItem, ref dataAction, m_processingContext, onePassProcess: true);
						}
					}
					if (runtimeDataRegionObj != null)
					{
						if (m_dataRegionObjs == null)
						{
							m_dataRegionObjs = new RuntimeDataRegionObjList();
						}
						m_dataRegionObjs.Add(runtimeDataRegionObj);
					}
				}
			}

			internal void FirstPassNextDataRow()
			{
				if (m_dataRegionObjs != null)
				{
					AggregateRowInfo aggregateRowInfo = new AggregateRowInfo();
					aggregateRowInfo.SaveAggregateInfo(m_processingContext);
					for (int i = 0; i < m_dataRegionObjs.Count; i++)
					{
						m_dataRegionObjs[i].NextRow();
						aggregateRowInfo.RestoreAggregateInfo(m_processingContext);
					}
				}
			}

			internal void SortAndFilter()
			{
				if (m_dataRegionObjs == null)
				{
					return;
				}
				for (int i = 0; i < m_dataRegionObjs.Count; i++)
				{
					RuntimeDataRegionObj runtimeDataRegionObj = m_dataRegionObjs[i];
					if (!(runtimeDataRegionObj is RuntimeDetailObj))
					{
						runtimeDataRegionObj.SortAndFilter();
					}
				}
			}

			internal void CalculatePreviousAggregates(AggregatesImpl globalRunningValueCollection)
			{
				DoneReadingRows(globalRunningValueCollection, m_reportItemsDef.RunningValues, ref m_runningValueValues, processPreviousAggregates: true);
			}

			internal void CalculateRunningValues(AggregatesImpl globalRunningValueCollection, RuntimeGroupRootObjList groupCollection, RuntimeGroupRootObj lastGroup)
			{
				CalculateInnerRunningValues(globalRunningValueCollection, groupCollection, lastGroup);
				DoneReadingRows(globalRunningValueCollection, m_reportItemsDef.RunningValues, ref m_runningValueValues, processPreviousAggregates: false);
			}

			internal void CalculateInnerRunningValues(AggregatesImpl globalRunningValueCollection, RuntimeGroupRootObjList groupCollection, RuntimeGroupRootObj lastGroup)
			{
				if (m_dataRegionObjs != null)
				{
					for (int i = 0; i < m_dataRegionObjs.Count; i++)
					{
						m_dataRegionObjs[i].CalculateRunningValues(globalRunningValueCollection, groupCollection, lastGroup);
					}
				}
			}

			internal static void DoneReadingRows(AggregatesImpl globalRVCol, RunningValueInfoList runningValues, ref DataAggregateObjResult[] runningValueValues, bool processPreviousAggregates)
			{
				if (runningValues != null && 0 < runningValues.Count)
				{
					if (runningValueValues == null)
					{
						runningValueValues = new DataAggregateObjResult[runningValues.Count];
					}
					for (int i = 0; i < runningValues.Count; i++)
					{
						if (processPreviousAggregates == (DataAggregateInfo.AggregateTypes.Previous == runningValues[i].AggregateType))
						{
							runningValueValues[i] = globalRVCol.GetAggregateObj(runningValues[i].Name).AggregateResult();
						}
					}
				}
				else
				{
					runningValueValues = null;
				}
			}

			private void SetupEnvironment()
			{
				RunningValueInfoList runningValues = m_reportItemsDef.RunningValues;
				if (runningValues != null && m_runningValueValues != null)
				{
					for (int i = 0; i < runningValues.Count; i++)
					{
						m_processingContext.ReportObjectModel.AggregatesImpl.Set(runningValues[i].Name, runningValues[i], runningValues[i].DuplicateNames, m_runningValueValues[i]);
					}
				}
				SetReportItemObjScope();
			}

			private void SetupEnvironment(ReportItemCollection reportItemsDef)
			{
				SetupEnvironment();
				if (reportItemsDef.ComputedReportItems == null)
				{
					return;
				}
				int num = 0;
				for (int i = 0; i < reportItemsDef.ComputedReportItems.Count; i++)
				{
					ReportItem reportItem = reportItemsDef.ComputedReportItems[i];
					if (reportItem is DataRegion)
					{
						if (reportItem is Table || reportItem is Matrix)
						{
							RuntimeDataRegionObj runtimeDataRegionObj = (m_dataRegionObjs == null) ? ((DataRegion)reportItem).RuntimeDataRegionObj : m_dataRegionObjs[num];
							Global.Tracer.Assert(runtimeDataRegionObj != null, "(null != dataRegionObj)");
							runtimeDataRegionObj.SetupEnvironment();
						}
						if (m_dataRegionObjs != null)
						{
							num++;
						}
					}
				}
			}

			internal void CreateInstances(ReportItemColInstance collectionInstance, bool ignorePageBreaks, bool ignoreInstances)
			{
				if (ignorePageBreaks)
				{
					m_processingContext.ChunkManager.EnterIgnorePageBreakItem();
				}
				if (ignoreInstances)
				{
					m_processingContext.ChunkManager.EnterIgnoreInstances();
				}
				CreateInstances(collectionInstance);
				if (ignoreInstances)
				{
					m_processingContext.ChunkManager.LeaveIgnoreInstances();
				}
				if (ignorePageBreaks)
				{
					m_processingContext.ChunkManager.LeaveIgnorePageBreakItem();
				}
			}

			internal void CreateInstances(ReportItemColInstance collectionInstance)
			{
				SetupEnvironment(m_reportItemsDef);
				m_currDataRegion = 0;
				CreateInstances(collectionInstance, m_reportItemsDef);
			}

			private void CreateInstances(ReportItemColInstance collectionInstance, ReportItemCollection reportItemsDef)
			{
				if (reportItemsDef == null || reportItemsDef.Count < 1)
				{
					return;
				}
				reportItemsDef.ProcessDrillthroughAction(m_processingContext, collectionInstance.ChildrenNonComputedUniqueNames);
				m_processingContext.ChunkManager.EnterReportItemCollection();
				ReportItemInstance reportItemInstance = null;
				ReportItem parent = reportItemsDef[0].Parent;
				int num = parent.StartPage;
				bool flag = false;
				bool flag2 = parent is Table;
				if (parent is Report || parent is List || parent is Rectangle || parent is SubReport || parent is CustomReportItem)
				{
					flag = true;
					collectionInstance.ChildrenStartAndEndPages = new RenderingPagesRangesList(reportItemsDef.Count);
				}
				List<DataRegion> list = new List<DataRegion>();
				for (int i = 0; i < reportItemsDef.Count; i++)
				{
					if (flag2)
					{
						m_processingContext.PageSectionContext.SetTableCellIndex(m_processingContext.IsOnePass, i);
					}
					reportItemsDef.GetReportItem(i, out bool computed, out int internalIndex, out ReportItem reportItem);
					if (reportItem is DataRegion && ((DataRegion)reportItem).RepeatSiblings != null)
					{
						list.Add(reportItem as DataRegion);
					}
					if (reportItem.RepeatedSibling)
					{
						m_processingContext.PageSectionContext.EnterRepeatingItem();
					}
					if (computed)
					{
						reportItem = reportItemsDef.ComputedReportItems[internalIndex];
						reportItemInstance = CreateInstance(reportItem, setupEnvironment: false, i);
						if (reportItemInstance != null)
						{
							collectionInstance.Add(reportItemInstance);
						}
					}
					else
					{
						collectionInstance.SetPaginationForNonComputedChild(m_processingContext.Pagination, reportItem, parent);
						reportItem.ProcessNavigationAction(m_processingContext.NavigationInfo, collectionInstance.ChildrenNonComputedUniqueNames[internalIndex], reportItem.StartPage);
						AddNonComputedPageTextboxes(reportItem, reportItem.StartPage, m_processingContext);
					}
					if (reportItem.RepeatedSibling)
					{
						reportItem.RepeatedSiblingTextboxes = m_processingContext.PageSectionContext.ExitRepeatingItem();
					}
					num = Math.Max(num, reportItem.EndPage);
					if (flag)
					{
						RenderingPagesRanges renderingPagesRanges = default(RenderingPagesRanges);
						renderingPagesRanges.StartPage = reportItem.StartPage;
						renderingPagesRanges.EndPage = reportItem.EndPage;
						collectionInstance.ChildrenStartAndEndPages.Add(renderingPagesRanges);
					}
					if (reportItem.RepeatedSiblingTextboxes != null && m_processingContext.PageSectionContext.PageTextboxes != null)
					{
						m_processingContext.PageSectionContext.PageTextboxes.IntegrateRepeatingTextboxValues(reportItem.RepeatedSiblingTextboxes, reportItem.StartPage, reportItem.EndPage);
					}
				}
				if (m_processingContext.PageSectionContext.PageTextboxes != null)
				{
					for (int j = 0; j < list.Count; j++)
					{
						DataRegion dataRegion = list[j];
						Global.Tracer.Assert(dataRegion.RepeatSiblings != null, "(null != dataRegion.RepeatSiblings)");
						for (int k = 0; k < dataRegion.RepeatSiblings.Count; k++)
						{
							ReportItem reportItem2 = reportItemsDef[dataRegion.RepeatSiblings[k]];
							Global.Tracer.Assert(reportItem2 != null, "(null != sibling)");
							m_processingContext.PageSectionContext.PageTextboxes.IntegrateRepeatingTextboxValues(reportItem2.RepeatedSiblingTextboxes, dataRegion.StartPage, dataRegion.EndPage);
						}
					}
				}
				if (num > parent.EndPage)
				{
					parent.EndPage = num;
					m_processingContext.Pagination.SetCurrentPageHeight(parent, 1.0);
				}
				m_processingContext.ChunkManager.LeaveReportItemCollection();
			}

			internal static void AddNonComputedPageTextboxes(ReportItem reportItem, int startPage, ProcessingContext processingContext)
			{
				if (!processingContext.PageSectionContext.IsParentVisible() || !Visibility.IsVisible(reportItem))
				{
					return;
				}
				if (reportItem is TextBox)
				{
					TextBox textBox = reportItem as TextBox;
					object value = null;
					if (textBox.Value != null)
					{
						value = textBox.Value.Value;
					}
					if (0 <= startPage)
					{
						textBox.StartPage = startPage;
					}
					AddPageTextbox(processingContext, textBox, null, null, value);
				}
				else
				{
					if (!(reportItem is Rectangle))
					{
						return;
					}
					Rectangle rectangle = reportItem as Rectangle;
					if (rectangle.ReportItems != null)
					{
						for (int i = 0; i < rectangle.ReportItems.Count; i++)
						{
							AddNonComputedPageTextboxes(rectangle.ReportItems[i], startPage, processingContext);
						}
					}
				}
			}

			internal ReportItemInstance CreateInstance(ReportItem reportItem, bool setupEnvironment, bool ignorePageBreaks, bool ignoreInstances)
			{
				if (ignorePageBreaks)
				{
					m_processingContext.ChunkManager.EnterIgnorePageBreakItem();
				}
				if (ignoreInstances)
				{
					m_processingContext.ChunkManager.EnterIgnoreInstances();
				}
				ReportItemInstance result = CreateInstance(reportItem, setupEnvironment, -1);
				if (ignoreInstances)
				{
					m_processingContext.ChunkManager.LeaveIgnoreInstances();
				}
				if (ignorePageBreaks)
				{
					m_processingContext.ChunkManager.LeaveIgnorePageBreakItem();
				}
				return result;
			}

			private ReportItemInstance CreateInstance(ReportItem reportItem, bool setupEnvironment, int index)
			{
				ReportItemInstance reportItemInstance = null;
				string label = null;
				if (setupEnvironment)
				{
					SetupEnvironment();
				}
				bool flag = reportItem is SubReport || reportItem is Rectangle || reportItem is DataRegion;
				m_processingContext.Pagination.EnterIgnorePageBreak(reportItem.Visibility, ignoreAlways: false);
				if (!(reportItem is Rectangle) && !(reportItem is DataRegion) && reportItem.Parent != null)
				{
					if (reportItem.Parent is Rectangle || reportItem.Parent is Report || reportItem.Parent is List)
					{
						bool softPageAtStart = m_processingContext.Pagination.CalculateSoftPageBreak(reportItem, 0.0, reportItem.DistanceBeforeTop, ignoreSoftPageBreak: false, logicalPageBreak: false);
						m_processingContext.Pagination.SetReportItemStartPage(reportItem, softPageAtStart);
					}
					else
					{
						int num = reportItem.Parent.StartPage;
						if (reportItem.Parent is Table)
						{
							num = ((Table)reportItem.Parent).CurrentPage;
						}
						else if (reportItem.Parent is Matrix)
						{
							num = ((Matrix)reportItem.Parent).CurrentPage;
						}
						reportItem.StartPage = num;
						reportItem.EndPage = num;
					}
				}
				if (reportItem is TextBox)
				{
					reportItemInstance = CreateTextBoxInstance((TextBox)reportItem, m_processingContext, index, m_owner);
				}
				else if (reportItem is Line)
				{
					reportItemInstance = CreateLineInstance((Line)reportItem, m_processingContext, index);
				}
				else if (reportItem is Image)
				{
					reportItemInstance = CreateImageInstance((Image)reportItem, m_processingContext, index);
				}
				else if (reportItem is ActiveXControl)
				{
					reportItemInstance = CreateActiveXControlInstance((ActiveXControl)reportItem, m_processingContext, index);
				}
				else if (reportItem is SubReport)
				{
					reportItemInstance = CreateSubReportInstance((SubReport)reportItem, m_processingContext, index, m_owner, out label);
				}
				else if (reportItem is Rectangle)
				{
					Rectangle rectangle = (Rectangle)reportItem;
					m_processingContext.ChunkManager.CheckPageBreak(rectangle, atStart: true);
					bool softPageAtStart2 = m_processingContext.Pagination.CalculateSoftPageBreak(rectangle, 0.0, rectangle.DistanceBeforeTop, ignoreSoftPageBreak: false);
					m_processingContext.Pagination.SetReportItemStartPage(rectangle, softPageAtStart2);
					RectangleInstance rectangleInstance = new RectangleInstance(m_processingContext, rectangle, index);
					if (reportItem.Label != null)
					{
						label = m_processingContext.NavigationInfo.CurrentLabel;
						if (label != null)
						{
							m_processingContext.NavigationInfo.EnterDocumentMapChildren();
						}
					}
					if (Report.ShowHideTypes.Interactive == m_processingContext.ShowHideType)
					{
						((IShowHideContainer)rectangleInstance).BeginProcessContainer(m_processingContext);
					}
					m_processingContext.PageSectionContext.EnterVisibilityScope(rectangle.Visibility, rectangle.StartHidden);
					CreateInstances(rectangleInstance.ReportItemColInstance, rectangle.ReportItems);
					m_processingContext.PageSectionContext.ExitVisibilityScope();
					if (Report.ShowHideTypes.Interactive == m_processingContext.ShowHideType)
					{
						((IShowHideContainer)rectangleInstance).EndProcessContainer(m_processingContext);
					}
					m_processingContext.ChunkManager.CheckPageBreak(rectangle, atStart: false);
					m_processingContext.Pagination.ProcessEndPage(rectangleInstance, reportItem, rectangle.PageBreakAtEnd, childrenOnThisPage: false);
					reportItemInstance = rectangleInstance;
				}
				else if (reportItem is DataRegion)
				{
					DataRegion dataRegion = (DataRegion)reportItem;
					bool flag2 = true;
					RuntimeDataRegionObj runtimeDataRegionObj;
					if (m_dataRegionObjs != null)
					{
						runtimeDataRegionObj = m_dataRegionObjs[m_currDataRegion];
					}
					else
					{
						runtimeDataRegionObj = dataRegion.RuntimeDataRegionObj;
						dataRegion.RuntimeDataRegionObj = null;
					}
					bool flag3;
					if (reportItem is CustomReportItem && runtimeDataRegionObj == null)
					{
						Global.Tracer.Assert(((CustomReportItem)reportItem).DataSetName == null, "(null == ((CustomReportItem)reportItem).DataSetName)");
						flag3 = false;
					}
					else
					{
						Global.Tracer.Assert(runtimeDataRegionObj != null, "(null != dataRegionObj)");
						runtimeDataRegionObj.SetupEnvironment();
						flag3 = true;
						m_processingContext.ChunkManager.CheckPageBreak(dataRegion, atStart: true);
						m_processingContext.ChunkManager.AddRepeatSiblings(dataRegion, index);
					}
					bool softPageAtStart3 = m_processingContext.Pagination.CalculateSoftPageBreak(dataRegion, 0.0, dataRegion.DistanceBeforeTop, ignoreSoftPageBreak: false);
					m_processingContext.Pagination.SetReportItemStartPage(dataRegion, softPageAtStart3);
					if (reportItem is List)
					{
						List list = (List)reportItem;
						list.ContentStartPage = list.StartPage;
						RuntimeOnePassListDetailObj runtimeOnePassListDetailObj = null;
						RenderingPagesRangesList renderingPagesRangesList = null;
						ListInstance listInstance;
						if (runtimeDataRegionObj is RuntimeOnePassListDetailObj)
						{
							runtimeOnePassListDetailObj = (RuntimeOnePassListDetailObj)runtimeDataRegionObj;
							renderingPagesRangesList = runtimeOnePassListDetailObj.ChildrenStartAndEndPages;
							listInstance = new ListInstance(m_processingContext, list, runtimeOnePassListDetailObj.ListContentInstances, renderingPagesRangesList);
							if (renderingPagesRangesList != null && (!m_processingContext.PageSectionContext.IsParentVisible() || !Visibility.IsOnePassHierarchyVisible(list)))
							{
								runtimeOnePassListDetailObj.MoveAllToFirstPage();
								int totalCount = (runtimeOnePassListDetailObj.ListContentInstances != null) ? runtimeOnePassListDetailObj.ListContentInstances.Count : 0;
								renderingPagesRangesList.MoveAllToFirstPage(totalCount);
								runtimeOnePassListDetailObj.NumberOfContentsOnThisPage = 0;
							}
							if (runtimeOnePassListDetailObj.NumberOfContentsOnThisPage > 0)
							{
								if (renderingPagesRangesList != null && 0 < renderingPagesRangesList.Count)
								{
									m_processingContext.Pagination.SetCurrentPageHeight(list, runtimeOnePassListDetailObj.Pagination.CurrentPageHeight);
								}
								else
								{
									m_processingContext.Pagination.AddToCurrentPageHeight(list, runtimeOnePassListDetailObj.Pagination.CurrentPageHeight);
								}
							}
							listInstance.NumberOfContentsOnThisPage = runtimeOnePassListDetailObj.NumberOfContentsOnThisPage;
							if (reportItem.Label != null)
							{
								label = m_processingContext.NavigationInfo.CurrentLabel;
							}
							m_processingContext.NavigationInfo.AppendNavigationInfo(label, runtimeOnePassListDetailObj.NavigationInfo, list.StartPage);
						}
						else
						{
							listInstance = new ListInstance(m_processingContext, list);
							bool delayAddingInstanceInfo = m_processingContext.DelayAddingInstanceInfo;
							m_processingContext.DelayAddingInstanceInfo = false;
							if (list.Label != null)
							{
								label = m_processingContext.NavigationInfo.CurrentLabel;
								if (label != null)
								{
									m_processingContext.NavigationInfo.EnterDocumentMapChildren();
								}
							}
							runtimeDataRegionObj.CreateInstances(listInstance, listInstance.ListContents, listInstance.ChildrenStartAndEndPages);
							m_processingContext.DelayAddingInstanceInfo = delayAddingInstanceInfo;
						}
						m_processingContext.Pagination.ProcessListRenderingPages(listInstance, list);
						if (m_processingContext.PageSectionContext.IsParentVisible() && runtimeOnePassListDetailObj != null)
						{
							if (renderingPagesRangesList != null && 0 < renderingPagesRangesList.Count)
							{
								for (int i = 0; i < renderingPagesRangesList.Count; i++)
								{
									runtimeOnePassListDetailObj.ProcessOnePassDetailTextboxes(i, list.StartPage + i);
								}
							}
							else if (listInstance.ListContents != null)
							{
								runtimeOnePassListDetailObj.ProcessOnePassDetailTextboxes(0, list.StartPage);
							}
						}
						reportItemInstance = listInstance;
					}
					else if (reportItem is Matrix)
					{
						MatrixHeadingInstance headingInstance = m_processingContext.HeadingInstance;
						MatrixHeadingInstance headingInstanceOld = m_processingContext.HeadingInstanceOld;
						m_processingContext.HeadingInstance = null;
						m_processingContext.HeadingInstanceOld = null;
						RuntimeMatrixObj runtimeMatrixObj = (RuntimeMatrixObj)runtimeDataRegionObj;
						MatrixInstance matrixInstance = new MatrixInstance(m_processingContext, (Matrix)reportItem);
						if (reportItem.Label != null)
						{
							label = m_processingContext.NavigationInfo.CurrentLabel;
							if (label != null)
							{
								m_processingContext.NavigationInfo.EnterDocumentMapChildren();
							}
						}
						if (Report.ShowHideTypes.Interactive == m_processingContext.ShowHideType)
						{
							((IShowHideContainer)matrixInstance).BeginProcessContainer(m_processingContext);
						}
						runtimeMatrixObj.CreateInstances(matrixInstance, matrixInstance.Cells, matrixInstance.ChildrenStartAndEndPages);
						if (Report.ShowHideTypes.Interactive == m_processingContext.ShowHideType)
						{
							((IShowHideContainer)matrixInstance).EndProcessContainer(m_processingContext);
						}
						if (setupEnvironment)
						{
							runtimeMatrixObj.ResetReportItems();
						}
						m_processingContext.HeadingInstance = headingInstance;
						m_processingContext.HeadingInstanceOld = headingInstanceOld;
						reportItemInstance = matrixInstance;
					}
					else if (reportItem is CustomReportItem)
					{
						CustomReportItem customReportItem = (CustomReportItem)reportItem;
						CustomReportItemInstance customReportItemInstance = new CustomReportItemInstance(m_processingContext, customReportItem);
						if (customReportItem.DataSetName != null)
						{
							bool delayAddingInstanceInfo2 = m_processingContext.DelayAddingInstanceInfo;
							m_processingContext.DelayAddingInstanceInfo = false;
							((RuntimeCustomReportItemObj)runtimeDataRegionObj).CreateInstances(customReportItemInstance, customReportItemInstance.Cells, null);
							m_processingContext.DelayAddingInstanceInfo = delayAddingInstanceInfo2;
							m_processingContext.Pagination.ProcessEndPage(customReportItemInstance, reportItem, customReportItem.PageBreakAtEnd, childrenOnThisPage: false);
						}
						else
						{
							flag2 = false;
						}
						Microsoft.ReportingServices.ReportRendering.ICustomReportItem controlInstance = m_processingContext.CriProcessingControls.GetControlInstance(customReportItem.Type, m_processingContext.ExtFactory);
						if (controlInstance == null)
						{
							if (customReportItem.AltReportItem != null)
							{
								customReportItemInstance.AltReportItemColInstance = new ReportItemColInstance(m_processingContext, customReportItem.AltReportItem);
								Global.Tracer.Assert(1 == customReportItem.AltReportItem.Count, "(1 == criDef.AltReportItem.Count)");
								m_processingContext.RuntimeInitializeReportItemObjs(customReportItem.AltReportItem, traverseDataRegions: false, setValue: false);
								CreateInstances(customReportItemInstance.AltReportItemColInstance, customReportItem.AltReportItem);
							}
						}
						else
						{
							CustomReportItemInstanceInfo instanceInfo = (CustomReportItemInstanceInfo)customReportItemInstance.GetInstanceInfo(null);
							customReportItem.CustomProcessingInitialize(customReportItemInstance, instanceInfo, m_processingContext, index);
							Microsoft.ReportingServices.ReportRendering.CustomReportItem customItem = new Microsoft.ReportingServices.ReportRendering.CustomReportItem(customReportItem, customReportItemInstance, instanceInfo);
							Microsoft.ReportingServices.ReportRendering.ReportItem reportItem2 = null;
							try
							{
								controlInstance.CustomItem = customItem;
								controlInstance.Process();
								reportItem2 = controlInstance.RenderItem;
								if (reportItem2 != null)
								{
									reportItem2 = ((IDeepCloneable)reportItem2).DeepClone();
								}
								customReportItem.DeconstructRenderItem(reportItem2, customReportItemInstance);
								customReportItem.CustomProcessingReset();
							}
							catch (Exception innerException)
							{
								throw new ReportProcessingException(ErrorCode.rsCRIProcessingError, innerException, customReportItem.Name, customReportItem.Type);
							}
						}
						reportItemInstance = customReportItemInstance;
					}
					else if (reportItem is Chart)
					{
						_ = (RuntimeChartObj)runtimeDataRegionObj;
						ChartInstance chartInstance = new ChartInstance(m_processingContext, (Chart)reportItem);
						bool delayAddingInstanceInfo3 = m_processingContext.DelayAddingInstanceInfo;
						m_processingContext.DelayAddingInstanceInfo = false;
						runtimeDataRegionObj.CreateInstances(chartInstance, chartInstance.MultiCharts, null);
						m_processingContext.DelayAddingInstanceInfo = delayAddingInstanceInfo3;
						m_processingContext.Pagination.ProcessEndPage(chartInstance, reportItem, ((Chart)reportItem).PageBreakAtEnd, childrenOnThisPage: false);
						reportItemInstance = chartInstance;
					}
					else if (reportItem is Table)
					{
						RuntimeTableObj runtimeTableObj = (RuntimeTableObj)runtimeDataRegionObj;
						Table table = (Table)reportItem;
						table.CurrentPage = reportItem.StartPage;
						TableInstance tableInstance = (runtimeTableObj.TableDetailInstances != null) ? new TableInstance(m_processingContext, table, runtimeTableObj.TableDetailInstances, runtimeTableObj.ChildrenStartAndEndPages) : new TableInstance(m_processingContext, table);
						if (table.Label != null)
						{
							label = m_processingContext.NavigationInfo.CurrentLabel;
							if (label != null)
							{
								m_processingContext.NavigationInfo.EnterDocumentMapChildren();
							}
						}
						if (Report.ShowHideTypes.Interactive == m_processingContext.ShowHideType)
						{
							((IShowHideContainer)tableInstance).BeginProcessContainer(m_processingContext);
						}
						runtimeTableObj.CreateInstances(tableInstance, tableInstance.TableGroupInstances, tableInstance.ChildrenStartAndEndPages);
						if (Report.ShowHideTypes.Interactive == m_processingContext.ShowHideType)
						{
							((IShowHideContainer)tableInstance).EndProcessContainer(m_processingContext);
						}
						if (setupEnvironment)
						{
							runtimeTableObj.ResetReportItems();
						}
						m_processingContext.Pagination.ProcessTableRenderingPages(tableInstance, table);
						reportItemInstance = tableInstance;
					}
					else if (reportItem is OWCChart)
					{
						OWCChartInstance oWCChartInstance;
						if (runtimeDataRegionObj is RuntimeOnePassOWCChartDetailObj)
						{
							oWCChartInstance = new OWCChartInstance(m_processingContext, (OWCChart)reportItem, ((RuntimeOnePassOWCChartDetailObj)runtimeDataRegionObj).OWCChartData);
						}
						else
						{
							oWCChartInstance = new OWCChartInstance(m_processingContext, (OWCChart)reportItem);
							bool delayAddingInstanceInfo4 = m_processingContext.DelayAddingInstanceInfo;
							m_processingContext.DelayAddingInstanceInfo = false;
							runtimeDataRegionObj.CreateInstances(oWCChartInstance, oWCChartInstance.InstanceInfo.ChartData, null);
							m_processingContext.DelayAddingInstanceInfo = delayAddingInstanceInfo4;
						}
						m_processingContext.ChunkManager.AddInstance(oWCChartInstance.InstanceInfo, oWCChartInstance, m_processingContext.InPageSection);
						m_processingContext.Pagination.ProcessEndPage(oWCChartInstance, reportItem, ((OWCChart)reportItem).PageBreakAtEnd, childrenOnThisPage: false);
						reportItemInstance = oWCChartInstance;
						if (m_processingContext.OWCChartName == reportItem.Name && m_processingContext.OWCChartInstance == null)
						{
							m_processingContext.OWCChartInstance = oWCChartInstance.InstanceInfo;
						}
					}
					if (flag3)
					{
						m_processingContext.ChunkManager.CheckPageBreak(dataRegion, atStart: false);
					}
					if (flag2 && m_dataRegionObjs != null)
					{
						m_dataRegionObjs[m_currDataRegion++] = null;
					}
				}
				if (!flag && reportItem.Label != null)
				{
					label = m_processingContext.NavigationInfo.CurrentLabel;
				}
				if (label != null)
				{
					m_processingContext.NavigationInfo.AddToDocumentMap(reportItemInstance.GetDocumentMapUniqueName(), flag, reportItem.StartPage, label);
				}
				if (reportItem.Parent != null)
				{
					if (reportItem.EndPage > reportItem.Parent.EndPage)
					{
						reportItem.Parent.EndPage = reportItem.EndPage;
						reportItem.Parent.BottomInEndPage = reportItem.BottomInEndPage;
						if (reportItem.Parent is List)
						{
							((List)reportItem.Parent).ContentStartPage = reportItem.EndPage;
						}
					}
					else if (reportItem.EndPage == reportItem.Parent.EndPage)
					{
						reportItem.Parent.BottomInEndPage = Math.Max(reportItem.Parent.BottomInEndPage, reportItem.BottomInEndPage);
					}
				}
				m_processingContext.Pagination.LeaveIgnorePageBreak(reportItem.Visibility, ignoreAlways: false);
				return reportItemInstance;
			}

			internal void ResetReportItemObjs()
			{
				if (m_processingContext.ReportItemsReferenced || m_processingContext.ReportItemThisDotValueReferenced)
				{
					TraverseReportItemObjs(m_reportItemsDef, m_processingContext, reset: true, m_owner);
				}
			}

			internal static void ResetReportItemObjs(ReportItemCollection reportItems, ProcessingContext processingContext)
			{
				if (processingContext.ReportItemsReferenced || processingContext.ReportItemThisDotValueReferenced)
				{
					TraverseReportItemObjs(reportItems, processingContext, reset: true, null);
				}
			}

			internal void SetReportItemObjScope()
			{
				if (m_processingContext.ReportItemsReferenced || m_processingContext.ReportItemThisDotValueReferenced)
				{
					TraverseReportItemObjs(m_reportItemsDef, m_processingContext, reset: false, m_owner);
				}
			}

			private static void TraverseReportItemObjs(ReportItemCollection reportItems, ProcessingContext processingContext, bool reset, IScope scope)
			{
				if (reportItems == null || reportItems.ComputedReportItems == null)
				{
					return;
				}
				for (int i = 0; i < reportItems.ComputedReportItems.Count; i++)
				{
					ReportItem reportItem = reportItems.ComputedReportItems[i];
					if (reportItem is TextBox)
					{
						TextBox textBox = (TextBox)reportItem;
						TextBoxImpl textBoxImpl = null;
						if (processingContext.ReportItemsReferenced)
						{
							textBoxImpl = (TextBoxImpl)processingContext.ReportObjectModel.ReportItemsImpl[textBox.Name];
						}
						else if (textBox.ValueReferenced)
						{
							textBoxImpl = (TextBoxImpl)textBox.TextBoxExprHost.ReportObjectModelTextBox;
						}
						if (textBoxImpl != null)
						{
							if (reset)
							{
								textBoxImpl.Reset();
							}
							else
							{
								textBoxImpl.Scope = scope;
							}
						}
					}
					else if (reportItem is Rectangle)
					{
						TraverseReportItemObjs(((Rectangle)reportItem).ReportItems, processingContext, reset, scope);
					}
					else if (reportItem is Table)
					{
						Table table = (Table)reportItem;
						if (table.HeaderRows != null)
						{
							for (int j = 0; j < table.HeaderRows.Count; j++)
							{
								TraverseReportItemObjs(table.HeaderRows[j].ReportItems, processingContext, reset, scope);
							}
						}
						if (table.FooterRows != null)
						{
							for (int k = 0; k < table.FooterRows.Count; k++)
							{
								TraverseReportItemObjs(table.FooterRows[k].ReportItems, processingContext, reset, scope);
							}
						}
					}
					else if (reportItem is Matrix)
					{
						Matrix matrix = (Matrix)reportItem;
						TraverseReportItemObjs(matrix.CornerReportItems, processingContext, reset, scope);
						TraverseReportItemObjs(matrix.Rows.ReportItems, processingContext, reset, scope);
						if (matrix.Rows.Subtotal != null)
						{
							TraverseReportItemObjs(matrix.Rows.Subtotal.ReportItems, processingContext, reset, scope);
						}
						TraverseReportItemObjs(matrix.Columns.ReportItems, processingContext, reset, scope);
						if (matrix.Columns.Subtotal != null)
						{
							TraverseReportItemObjs(matrix.Columns.Subtotal.ReportItems, processingContext, reset, scope);
						}
					}
				}
			}

			internal static bool GetExternalImage(ProcessingContext processingContext, string currentPath, ObjectType objectType, string objectName, out byte[] imageData, out string mimeType)
			{
				imageData = null;
				mimeType = null;
				try
				{
					if (!processingContext.ReportContext.IsSupportedProtocol(currentPath, protocolRestriction: true))
					{
						processingContext.ErrorContext.Register(ProcessingErrorCode.rsUnsupportedProtocol, Severity.Error, objectType, objectName, "Value", currentPath.MarkAsPrivate(), "http://, https://, ftp://, file:, mailto:, or news:");
					}
					else
					{
						processingContext.GetResource(currentPath, out imageData, out mimeType);
						if (imageData != null && !Validator.ValidateMimeType(mimeType))
						{
							processingContext.ErrorContext.Register(ProcessingErrorCode.rsInvalidMIMEType, Severity.Warning, objectType, objectName, "MIMEType", mimeType.MarkAsPrivate());
							mimeType = null;
							imageData = null;
						}
					}
				}
				catch (Exception ex)
				{
					processingContext.ErrorContext.Register(ProcessingErrorCode.rsInvalidImageReference, Severity.Warning, objectType, objectName, "Value", ex.Message);
					return false;
				}
				return true;
			}

			internal static ActionInstance CreateActionInstance(ProcessingContext processingContext, IActionOwner actionOwner, int ownerUniqueName, ObjectType objectType, string objectName)
			{
				Action action = actionOwner.Action;
				if (action == null)
				{
					return null;
				}
				processingContext.ReportRuntime.CurrentActionOwner = actionOwner;
				ActionInstance actionInstance = null;
				object[] array = null;
				ActionItemInstanceList actionItemInstanceList = null;
				string text = objectName + ".ActionInfo";
				Style styleClass = action.StyleClass;
				if (styleClass != null && styleClass.ExpressionList != null && 0 < styleClass.ExpressionList.Count)
				{
					actionInstance = new ActionInstance(processingContext);
					array = new object[styleClass.ExpressionList.Count];
					EvaluateStyleAttributes(objectType, text, styleClass, actionInstance.UniqueName, array, processingContext);
					actionInstance.StyleAttributeValues = array;
				}
				if (action.ComputedActionItemsCount > 0)
				{
					if (actionInstance == null)
					{
						actionInstance = new ActionInstance(processingContext);
					}
					actionItemInstanceList = new ActionItemInstanceList();
					ActionItemInstance actionItemInstance = null;
					ActionItem actionItem = null;
					text += ".Action";
					for (int i = 0; i < action.ActionItems.Count; i++)
					{
						actionItem = action.ActionItems[i];
						if (actionItem.ComputedIndex >= 0)
						{
							actionItemInstance = CreateActionItemInstance(processingContext, actionItem, ownerUniqueName, objectType, text, i);
							actionItemInstanceList.Add(actionItemInstance);
						}
						else
						{
							actionItem.ProcessDrillthroughAction(processingContext, ownerUniqueName, i);
						}
					}
					actionInstance.ActionItemsValues = actionItemInstanceList;
				}
				else
				{
					action.ProcessDrillthroughAction(processingContext, ownerUniqueName);
				}
				return actionInstance;
			}

			internal static ActionItemInstance CreateActionItemInstance(ProcessingContext processingContext, ActionItem actionItemDef, int ownerUniqueName, ObjectType objectType, string objectName, int index)
			{
				if (actionItemDef == null)
				{
					return null;
				}
				ActionItemInstance actionItemInstance = new ActionItemInstance(processingContext, actionItemDef);
				actionItemInstance.HyperLinkURL = processingContext.ReportRuntime.EvaluateReportItemHyperlinkURLExpression(actionItemDef, actionItemDef.HyperLinkURL, objectType, objectName);
				string text2 = actionItemInstance.DrillthroughReportName = processingContext.ReportRuntime.EvaluateReportItemDrillthroughReportName(actionItemDef, actionItemDef.DrillthroughReportName, objectType, objectName);
				actionItemInstance.BookmarkLink = processingContext.ReportRuntime.EvaluateReportItemBookmarkLinkExpression(actionItemDef, actionItemDef.BookmarkLink, objectType, objectName);
				ParameterValueList drillthroughParameters = actionItemDef.DrillthroughParameters;
				object[] drillthroughParametersValues = actionItemInstance.DrillthroughParametersValues;
				BoolList drillthroughParametersOmits = actionItemInstance.DrillthroughParametersOmits;
				DrillthroughParameters drillthroughParameters2 = null;
				IntList intList = null;
				if (drillthroughParameters != null && drillthroughParametersValues != null)
				{
					for (int i = 0; i < drillthroughParameters.Count; i++)
					{
						bool flag = false;
						if (drillthroughParameters[i].Omit != null)
						{
							flag = processingContext.ReportRuntime.EvaluateParamValueOmitExpression(drillthroughParameters[i], objectType, objectName);
						}
						drillthroughParametersOmits.Add(flag);
						if (flag)
						{
							drillthroughParametersValues[i] = null;
							continue;
						}
						drillthroughParametersValues[i] = processingContext.ReportRuntime.EvaluateParamVariantValueExpression(drillthroughParameters[i], objectType, objectName, "DrillthroughParameterValue");
						if (intList == null)
						{
							intList = new IntList();
						}
						if (drillthroughParameters2 == null)
						{
							drillthroughParameters2 = new DrillthroughParameters();
						}
						if (drillthroughParameters[i].Value != null && drillthroughParameters[i].Value.Type == ExpressionInfo.Types.Token)
						{
							intList.Add(drillthroughParameters[i].Value.IntValue);
							drillthroughParameters2.Add(drillthroughParameters[i].Name, null);
						}
						else
						{
							intList.Add(-1);
							drillthroughParameters2.Add(drillthroughParameters[i].Name, drillthroughParametersValues[i]);
						}
					}
				}
				actionItemInstance.Label = processingContext.ReportRuntime.EvaluateActionLabelExpression(actionItemDef, actionItemDef.Label, objectType, objectName);
				if (text2 != null)
				{
					DrillthroughInformation drillthroughInfo = new DrillthroughInformation(text2, drillthroughParameters2, intList);
					string drillthroughId = ownerUniqueName.ToString(CultureInfo.InvariantCulture) + ":" + index.ToString(CultureInfo.InvariantCulture);
					processingContext.DrillthroughInfo.AddDrillthrough(drillthroughId, drillthroughInfo);
				}
				return actionItemInstance;
			}

			internal static TextBoxInstance CreateTextBoxInstance(TextBox textBox, ProcessingContext processingContext, int index, IScope containingScope)
			{
				TextBoxInstance textBoxInstance = new TextBoxInstance(processingContext, textBox, index);
				bool flag = textBox.IsSimpleTextBox();
				SimpleTextBoxInstanceInfo simpleTextBoxInstanceInfo = null;
				TextBoxInstanceInfo textBoxInstanceInfo = null;
				if (flag)
				{
					simpleTextBoxInstanceInfo = (SimpleTextBoxInstanceInfo)textBoxInstance.InstanceInfo;
				}
				else
				{
					textBoxInstanceInfo = (TextBoxInstanceInfo)textBoxInstance.InstanceInfo;
				}
				bool flag2 = false;
				if (textBox.Action != null)
				{
					flag2 = textBox.Action.ResetObjectModelForDrillthroughContext(processingContext.ReportObjectModel, textBox);
				}
				VariantResult textBoxResult;
				if (processingContext.ReportItemsReferenced)
				{
					textBoxResult = ((TextBoxImpl)processingContext.ReportObjectModel.ReportItemsImpl[textBox.Name]).GetResult();
				}
				else if (textBox.ValueReferenced)
				{
					Global.Tracer.Assert(textBox.TextBoxExprHost != null, "(textBox.TextBoxExprHost != null)");
					textBoxResult = ((TextBoxImpl)textBox.TextBoxExprHost.ReportObjectModelTextBox).GetResult();
				}
				else
				{
					textBoxResult = processingContext.ReportRuntime.EvaluateTextBoxValueExpression(textBox);
				}
				if (flag2)
				{
					textBox.Action.GetSelectedItemsForDrillthroughContext(processingContext.ReportObjectModel, textBox);
				}
				if (flag)
				{
					simpleTextBoxInstanceInfo.OriginalValue = textBoxResult.Value;
				}
				else
				{
					textBoxInstanceInfo.OriginalValue = textBoxResult.Value;
				}
				if (!flag)
				{
					textBoxInstanceInfo.Duplicate = CalculateDuplicates(textBoxResult, textBox, processingContext);
				}
				AddPageTextbox(processingContext, textBox, textBoxInstance, textBoxInstanceInfo, textBoxResult.Value);
				if (!(textBoxResult.Value is string))
				{
					string formattedTextBoxValue = GetFormattedTextBoxValue(textBoxInstanceInfo, textBoxResult, textBox, processingContext);
					if (flag)
					{
						simpleTextBoxInstanceInfo.FormattedValue = formattedTextBoxValue;
					}
					else
					{
						textBoxInstanceInfo.FormattedValue = formattedTextBoxValue;
					}
				}
				if (!flag)
				{
					textBoxInstanceInfo.Action = CreateActionInstance(processingContext, textBox, textBoxInstance.UniqueName, textBox.ObjectType, textBox.Name);
				}
				textBox.SetValueType(textBoxResult.Value);
				if (textBox.UserSort != null)
				{
					SortFilterEventInfo sortFilterEventInfo = new SortFilterEventInfo(textBox);
					sortFilterEventInfo.EventSourceScopeInfo = processingContext.GetScopeValues(textBox.ContainingScopes, containingScope);
					if (processingContext.NewSortFilterEventInfo == null)
					{
						processingContext.NewSortFilterEventInfo = new SortFilterEventInfoHashtable();
					}
					processingContext.NewSortFilterEventInfo.Add(textBoxInstance.UniqueName, sortFilterEventInfo);
					RuntimeSortFilterEventInfoList runtimeSortFilterEventInfoList = processingContext.RuntimeSortFilterInfo;
					if (runtimeSortFilterEventInfoList == null && -1 == processingContext.DataSetUniqueName)
					{
						runtimeSortFilterEventInfoList = processingContext.ReportRuntimeUserSortFilterInfo;
					}
					if (runtimeSortFilterEventInfoList != null)
					{
						for (int i = 0; i < runtimeSortFilterEventInfoList.Count; i++)
						{
							runtimeSortFilterEventInfoList[i].MatchEventSource(textBox, textBoxInstance, containingScope, processingContext);
						}
					}
				}
				return textBoxInstance;
			}

			private static void AddPageTextbox(ProcessingContext processingContext, TextBox textbox, TextBoxInstance textboxInstance, TextBoxInstanceInfo textboxInstanceInfo, object value)
			{
				if (processingContext.SubReportLevel != 0 || processingContext.InPageSection || !processingContext.PageSectionContext.HasPageSections || processingContext.PageSectionContext.InMatrixSubtotal || !processingContext.PageSectionContext.IsParentVisible() || !Visibility.IsVisible(textbox, textboxInstance, textboxInstanceInfo))
				{
					return;
				}
				int page = 0;
				if (!processingContext.PageSectionContext.InRepeatingItem)
				{
					if (processingContext.ReportRuntime.CurrentScope is RuntimeListGroupLeafObj)
					{
						page = ((RuntimeListGroupLeafObj)processingContext.ReportRuntime.CurrentScope).StartPage;
					}
					else if (processingContext.IsOnePass)
					{
						RuntimeOnePassDetailObj runtimeOnePassDetailObj = processingContext.ReportRuntime.CurrentScope as RuntimeOnePassDetailObj;
						if (runtimeOnePassDetailObj != null)
						{
							page = runtimeOnePassDetailObj.GetDetailPage();
							runtimeOnePassDetailObj.OnePassTextboxes.AddTextboxValue(page, textbox.Name, value);
							if (page == 0)
							{
								RuntimeOnePassTableDetailObj runtimeOnePassTableDetailObj = runtimeOnePassDetailObj as RuntimeOnePassTableDetailObj;
								if (runtimeOnePassTableDetailObj != null && !runtimeOnePassTableDetailObj.TextboxColumnPositions.ContainsKey(textbox.Name))
								{
									((RuntimeOnePassTableDetailObj)runtimeOnePassDetailObj).TextboxColumnPositions.Add(textbox.Name, processingContext.PageSectionContext.GetOnePassTableCellProperties());
								}
							}
							return;
						}
						page = processingContext.Pagination.GetTextBoxStartPage(textbox);
					}
					else
					{
						page = processingContext.Pagination.GetTextBoxStartPage(textbox);
					}
				}
				processingContext.PageSectionContext.PageTextboxes.AddTextboxValue(page, textbox.Name, value);
			}

			internal static LineInstance CreateLineInstance(Line line, ProcessingContext processingContext, int index)
			{
				return new LineInstance(processingContext, line, index);
			}

			internal static ReportItemInstance CreateImageInstance(Image image, ProcessingContext processingContext, int index)
			{
				ImageInstance imageInstance = new ImageInstance(processingContext, image, index);
				ImageInstanceInfo instanceInfo = imageInstance.InstanceInfo;
				bool errorOccurred = false;
				bool flag = false;
				if (image.Action != null)
				{
					flag = image.Action.ResetObjectModelForDrillthroughContext(processingContext.ReportObjectModel, image);
				}
				switch (image.Source)
				{
				case Image.SourceType.External:
				{
					string text2 = processingContext.ReportRuntime.EvaluateImageStringValueExpression(image, out errorOccurred);
					if (flag)
					{
						image.Action.GetSelectedItemsForDrillthroughContext(processingContext.ReportObjectModel, image);
					}
					instanceInfo.ImageValue = text2;
					if (text2 == null || processingContext.ImageStreamNames.ContainsKey(text2))
					{
						break;
					}
					string mimeType2 = null;
					byte[] imageData = null;
					if (ExpressionInfo.Types.Constant != image.Value.Type)
					{
						GetExternalImage(processingContext, text2, image.ObjectType, image.Name, out imageData, out mimeType2);
					}
					if (imageData == null)
					{
						instanceInfo.BrokenImage = true;
					}
					else if (processingContext.InPageSection && !processingContext.CreatePageSectionImageChunks)
					{
						instanceInfo.Data = new ImageData(imageData, mimeType2);
					}
					else if (processingContext.CreateReportChunkCallback != null)
					{
						string text3 = Guid.NewGuid().ToString();
						using (Stream stream2 = processingContext.CreateReportChunkCallback(text3, ReportChunkTypes.Image, mimeType2))
						{
							stream2.Write(imageData, 0, imageData.Length);
						}
						processingContext.ImageStreamNames[text2] = new ImageInfo(text3, mimeType2);
					}
					break;
				}
				case Image.SourceType.Embedded:
				{
					string text = processingContext.ReportRuntime.EvaluateImageStringValueExpression(image, out errorOccurred);
					if (flag)
					{
						image.Action.GetSelectedItemsForDrillthroughContext(processingContext.ReportObjectModel, image);
					}
					if (errorOccurred)
					{
						instanceInfo.BrokenImage = true;
						break;
					}
					instanceInfo.ImageValue = ProcessingValidator.ValidateEmbeddedImageName(text, processingContext.EmbeddedImages, image.ObjectType, image.Name, "Value", processingContext.ErrorContext);
					instanceInfo.BrokenImage = (text != null);
					break;
				}
				case Image.SourceType.Database:
				{
					Global.Tracer.Assert(instanceInfo != null, "(null != imageInstanceInfo)");
					byte[] array = processingContext.ReportRuntime.EvaluateImageBinaryValueExpression(image, out errorOccurred);
					if (flag)
					{
						image.Action.GetSelectedItemsForDrillthroughContext(processingContext.ReportObjectModel, image);
					}
					if (errorOccurred)
					{
						instanceInfo.BrokenImage = true;
						processingContext.ErrorContext.Register(ProcessingErrorCode.rsInvalidDatabaseImage, Severity.Warning, image.ObjectType, image.Name, "Value");
					}
					Global.Tracer.Assert(image.MIMEType != null, "(null != image.MIMEType)");
					string mimeType = (ExpressionInfo.Types.Constant == image.MIMEType.Type) ? image.MIMEType.Value : ProcessingValidator.ValidateMimeType(processingContext.ReportRuntime.EvaluateImageMIMETypeExpression(image), image.ObjectType, image.Name, "MIMEType", processingContext.ErrorContext);
					if (array == null)
					{
						break;
					}
					if (processingContext.InPageSection && !processingContext.CreatePageSectionImageChunks)
					{
						instanceInfo.Data = new ImageData(array, mimeType);
					}
					else if (processingContext.CreateReportChunkCallback != null)
					{
						instanceInfo.ImageValue = Guid.NewGuid().ToString();
						using (Stream stream = processingContext.CreateReportChunkCallback(instanceInfo.ImageValue, ReportChunkTypes.Image, mimeType))
						{
							stream.Write(array, 0, array.Length);
						}
					}
					break;
				}
				}
				if (instanceInfo != null && instanceInfo.ImageValue == null && !instanceInfo.BrokenImage && processingContext.CreateReportChunkCallback != null)
				{
					if (processingContext.TransparentImageGuid == null)
					{
						string mimeType3 = "image/gif";
						string text5 = processingContext.TransparentImageGuid = Guid.NewGuid().ToString();
						using (Stream outputStream = processingContext.CreateReportChunkCallback(text5, ReportChunkTypes.Image, mimeType3))
						{
							FetchTransparentImage(outputStream);
						}
						processingContext.ImageStreamNames[text5] = new ImageInfo(text5, mimeType3);
						if (processingContext.EmbeddedImages != null)
						{
							processingContext.EmbeddedImages.Add(text5, new ImageInfo(text5, mimeType3));
						}
					}
					instanceInfo.ImageValue = processingContext.TransparentImageGuid;
				}
				instanceInfo.Action = CreateActionInstance(processingContext, image, imageInstance.UniqueName, image.ObjectType, image.Name);
				return imageInstance;
			}

			internal static void FetchTransparentImage(Stream outputStream)
			{
				byte[] transparentImageBytes = Microsoft.ReportingServices.ReportIntermediateFormat.Image.TransparentImageBytes;
				outputStream.Write(transparentImageBytes, 0, transparentImageBytes.Length);
			}

			internal static ActiveXControlInstance CreateActiveXControlInstance(ActiveXControl activeXControl, ProcessingContext processingContext, int index)
			{
				ActiveXControlInstance activeXControlInstance = new ActiveXControlInstance(processingContext, activeXControl, index);
				ActiveXControlInstanceInfo instanceInfo = activeXControlInstance.InstanceInfo;
				ParameterValueList parameters = activeXControl.Parameters;
				object[] parameterValues = instanceInfo.ParameterValues;
				if (parameters != null && parameterValues != null)
				{
					for (int i = 0; i < parameters.Count; i++)
					{
						parameterValues[i] = processingContext.ReportRuntime.EvaluateParamVariantValueExpression(parameters[i], activeXControl.ObjectType, activeXControl.Name, "ParameterValue");
					}
				}
				return activeXControlInstance;
			}

			internal static void RetrieveSubReport(SubReport subReport, ProcessingContext processingContext, ProcessingErrorContext subReportErrorContext, bool isProcessingPrefetch)
			{
				Global.Tracer.Assert(isProcessingPrefetch || subReportErrorContext != null, "(isProcessingPrefetch || (null != subReportErrorContext))");
				try
				{
					ICatalogItemContext subreportContext;
					string description;
					GetReportChunk getCompiledDefinitionCallback;
					ParameterInfoCollection parameters;
					try
					{
						if (!isProcessingPrefetch && processingContext.IsOnePass)
						{
							Monitor.Enter(processingContext.SubReportCallback);
						}
						processingContext.SubReportCallback(processingContext.ReportContext, subReport.ReportPath, out subreportContext, out description, out getCompiledDefinitionCallback, out parameters);
					}
					finally
					{
						if (!isProcessingPrefetch && processingContext.IsOnePass)
						{
							Monitor.Exit(processingContext.SubReportCallback);
						}
					}
					Global.Tracer.Assert(subreportContext != null, "(null != subreportContext)");
					subReport.ReportContext = subreportContext;
					subReport.Report = DeserializeReport(getCompiledDefinitionCallback, subReport);
					subReport.Report = AssignIDsForSubReport(subReport, processingContext, (!isProcessingPrefetch) ? SubReportInitialization.All : SubReportInitialization.AssignIDsOnly);
					subReport.RetrievalStatus = ((!isProcessingPrefetch) ? SubReport.Status.Retrieved : SubReport.Status.PreFetched);
					subReport.Description = description;
					subReport.ReportName = subreportContext.ItemName;
					subReport.StringUri = new CatalogItemUrlBuilder(subreportContext).ToString();
					subReport.ParametersFromCatalog = parameters;
				}
				catch (VersionMismatchException)
				{
					throw;
				}
				catch (Exception ex2)
				{
					if (ex2 is DataCacheUnavailableException)
					{
						throw;
					}
					HandleSubReportProcessingError(processingContext, subReport, subReportErrorContext, ex2);
					subReport.ReportContext = null;
					subReport.Report = null;
					subReport.Description = null;
					subReport.ReportName = null;
					subReport.StringUri = null;
					subReport.ParametersFromCatalog = null;
					subReport.RetrievalStatus = SubReport.Status.RetrieveFailed;
				}
			}

			private static SubReportInstance CreateSubReportInstance(SubReport subReport, ProcessingContext processingContext, int index, IScope containingScope, out string label)
			{
				processingContext.ChunkManager.CheckPageBreak(subReport, atStart: true);
				processingContext.PageSectionContext.EnterSubreport();
				SubReportInstance subReportInstance = new SubReportInstance(processingContext, subReport, index);
				if (subReport.Label != null)
				{
					label = processingContext.NavigationInfo.CurrentLabel;
					if (label != null)
					{
						processingContext.NavigationInfo.EnterDocumentMapChildren();
					}
				}
				else
				{
					label = null;
				}
				bool delayAddingInstanceInfo = processingContext.DelayAddingInstanceInfo;
				processingContext.DelayAddingInstanceInfo = false;
				ProcessingErrorContext processingErrorContext = new ProcessingErrorContext();
				bool firstSubreportInstance = false;
				if (processingContext.SubReportLevel <= 20)
				{
					if (SubReport.Status.PreFetched == subReport.RetrievalStatus)
					{
						firstSubreportInstance = true;
						subReport.Report = AssignIDsForSubReport(subReport, processingContext, SubReportInitialization.RuntimeOnly);
						subReport.RetrievalStatus = SubReport.Status.Retrieved;
					}
					else if (subReport.RetrievalStatus == SubReport.Status.NotRetrieved && processingContext.SubReportCallback != null)
					{
						RetrieveSubReport(subReport, processingContext, processingErrorContext, isProcessingPrefetch: false);
						firstSubreportInstance = true;
					}
					else if (processingContext.SnapshotProcessing)
					{
						subReport.ReportContext = processingContext.ReportContext.GetSubreportContext(subReport.ReportPath);
					}
					if (containingScope == null)
					{
						containingScope = processingContext.UserSortFilterContext.CurrentContainingScope;
					}
					VariantList[] scopeValues = processingContext.GetScopeValues(subReport.ContainingScopes, containingScope);
					int num;
					if (processingContext.SnapshotProcessing && subReport.DataSetUniqueNameMap != null && !subReport.SaveDataSetUniqueName)
					{
						num = subReport.GetDataSetUniqueName(scopeValues);
					}
					else
					{
						subReport.AddDataSetUniqueName(scopeValues, subReportInstance.UniqueName);
						num = subReportInstance.UniqueName;
					}
					if (SubReport.Status.Retrieved == subReport.RetrievalStatus)
					{
						CultureInfo cultureInfo = null;
						try
						{
							ParameterInfoCollection parameterInfoCollection = new ParameterInfoCollection();
							if (subReport.Parameters != null && subReport.ParametersFromCatalog != null)
							{
								for (int i = 0; i < subReport.Parameters.Count; i++)
								{
									string name = subReport.Parameters[i].Name;
									ParameterInfo parameterInfo = subReport.ParametersFromCatalog[name];
									if (parameterInfo == null)
									{
										throw new UnknownReportParameterException(name);
									}
									ParameterValueResult parameterValueResult = processingContext.ReportRuntime.EvaluateParameterValueExpression(subReport.Parameters[i], subReport.ObjectType, subReport.Name, "ParameterValue");
									if (parameterValueResult.ErrorOccurred)
									{
										throw new ReportProcessingException(ErrorCode.rsReportParameterProcessingError, name);
									}
									object[] array = null;
									object[] array2 = parameterValueResult.Value as object[];
									array = ((array2 == null) ? new object[1]
									{
										parameterValueResult.Value
									} : array2);
									ParameterInfo parameterInfo2 = new ParameterInfo();
									parameterInfo2.Name = name;
									parameterInfo2.Values = array;
									parameterInfo2.DataType = parameterValueResult.Type;
									parameterInfoCollection.Add(parameterInfo2);
								}
							}
							ParameterInfoCollection parameterInfoCollection2 = ParameterInfoCollection.Combine(subReport.ParametersFromCatalog, parameterInfoCollection, checkReadOnly: true, ignoreNewQueryParams: false, isParameterDefinitionUpdate: false, isSharedDataSetParameter: false, Localization.ClientPrimaryCulture);
							ProcessingContext subReportParametersAndContext = GetSubReportParametersAndContext(processingContext, subReport, num, parameterInfoCollection2, processingErrorContext);
							subReportParametersAndContext.AbortInfo.AddSubreportInstance(num);
							cultureInfo = Thread.CurrentThread.CurrentCulture;
							subReportParametersAndContext.UserSortFilterContext.CurrentContainingScope = containingScope;
							Merge merge = new Merge(subReport.Report, subReportParametersAndContext, firstSubreportInstance);
							if (!subReportParametersAndContext.SnapshotProcessing && !subReportParametersAndContext.ProcessWithCachedData && merge.PrefetchData(parameterInfoCollection2, subReport.MergeTransactions))
							{
								subReportParametersAndContext.SnapshotProcessing = true;
							}
							subReportInstance.ReportInstance = merge.Process(parameterInfoCollection2, subReport.MergeTransactions);
							Thread.CurrentThread.CurrentCulture = cultureInfo;
							cultureInfo = null;
							if (subReport.Report.HasImageStreams)
							{
								processingContext.HasImageStreams = true;
							}
							if (processingErrorContext.Messages != null && 0 < processingErrorContext.Messages.Count)
							{
								processingContext.ErrorContext.Register(ProcessingErrorCode.rsWarningExecutingSubreport, Severity.Warning, subReport.ObjectType, subReport.Name, null, processingErrorContext.Messages);
							}
						}
						catch (Exception ex)
						{
							if (ex.InnerException is DataCacheUnavailableException)
							{
								throw;
							}
							HandleSubReportProcessingError(processingContext, subReport, processingErrorContext, ex);
							subReportInstance.ReportInstance = null;
						}
						finally
						{
							if (cultureInfo != null)
							{
								Thread.CurrentThread.CurrentCulture = cultureInfo;
							}
						}
					}
				}
				processingContext.PageSectionContext.ExitSubreport();
				processingContext.ChunkManager.CheckPageBreak(subReport, atStart: false);
				processingContext.DelayAddingInstanceInfo = delayAddingInstanceInfo;
				processingContext.Pagination.ProcessEndPage(subReportInstance, subReport, pageBreakAtEnd: false, childrenOnThisPage: false);
				return subReportInstance;
			}

			private static ProcessingContext GetSubReportParametersAndContext(ProcessingContext processingContext, SubReport subReport, int subReportDataSetUniqueName, ParameterInfoCollection effectiveParameters, ProcessingErrorContext subReportErrorContext)
			{
				ProcessingContext processingContext2 = processingContext.ParametersContext(subReport.ReportContext, subReportErrorContext);
				if (processingContext.ResetForSubreportDataPrefetch)
				{
					processingContext2.SnapshotProcessing = false;
				}
				ProcessReportParameters(subReport.Report, processingContext2, effectiveParameters);
				if (!effectiveParameters.ValuesAreValid())
				{
					throw new ReportProcessingException(ErrorCode.rsParametersNotSpecified);
				}
				ProcessingContext processingContext3 = processingContext.SubReportContext(subReport, subReportDataSetUniqueName, subReportErrorContext);
				if (processingContext.ResetForSubreportDataPrefetch)
				{
					processingContext3.SnapshotProcessing = false;
				}
				return processingContext3;
			}

			private static void HandleSubReportProcessingError(ProcessingContext processingContext, SubReport subReport, ProcessingErrorContext subReportErrorContext, Exception e)
			{
				Global.Tracer.Assert(e != null, "(e != null)");
				if (!(e is ProcessingAbortedException) && Global.Tracer.TraceError)
				{
					Global.Tracer.Trace(TraceLevel.Error, "An error has occurred while processing a sub-report. Details: {0} Stack trace:\r\n{1}", e.Message, e.StackTrace);
				}
				if (subReportErrorContext == null)
				{
					processingContext.ErrorContext.Register(ProcessingErrorCode.rsErrorExecutingSubreport, Severity.Warning, subReport.ObjectType, subReport.Name, null, e.Message);
					return;
				}
				Global.Tracer.Assert(subReportErrorContext != null, "(null != subReportErrorContext)");
				if (e is RSException)
				{
					subReportErrorContext.Register((RSException)e, subReport.ObjectType);
				}
				processingContext.ErrorContext.Register(ProcessingErrorCode.rsErrorExecutingSubreport, Severity.Warning, subReport.ObjectType, subReport.Name, null, subReportErrorContext.Messages, e.Message);
			}

			private static Report AssignIDsForSubReport(SubReport subReport, ProcessingContext context, SubReportInitialization initializationAction)
			{
				Report report = subReport.Report;
				if (initializationAction != 0)
				{
					subReport.UpdateSubReportScopes(context.UserSortFilterContext);
				}
				if (report != null)
				{
					ArrayList arrayList = null;
					Hashtable iDMap = null;
					if (SubReportInitialization.RuntimeOnly != initializationAction)
					{
						AssignIDsForIDOwnerBase(report, context);
						report.BodyID = context.CreateIDForSubreport();
						arrayList = new ArrayList();
						arrayList.Add(report);
						iDMap = new Hashtable();
						AssignIDsForDataSets(report.DataSources, context, iDMap);
					}
					AssignIDsForReportItemCollection(report.ReportItems, context, iDMap, subReport, arrayList, initializationAction);
					AssignIDsForPageSection(report.PageHeader, context, iDMap, subReport, arrayList, initializationAction);
					AssignIDsForPageSection(report.PageFooter, context, iDMap, subReport, arrayList, initializationAction);
					if (SubReportInitialization.RuntimeOnly != initializationAction)
					{
						for (int i = 0; i < arrayList.Count; i++)
						{
							object obj = arrayList[i];
							if (obj is Report)
							{
								((Report)obj).NonDetailSortFiltersInScope = UpdateSortFilterTable(((Report)obj).NonDetailSortFiltersInScope, iDMap);
								((Report)obj).DetailSortFiltersInScope = UpdateSortFilterTable(((Report)obj).DetailSortFiltersInScope, iDMap);
							}
							else if (obj is DataRegion)
							{
								((DataRegion)obj).DetailSortFiltersInScope = UpdateSortFilterTable(((DataRegion)obj).DetailSortFiltersInScope, iDMap);
							}
							else if (obj is Grouping)
							{
								((Grouping)obj).NonDetailSortFiltersInScope = UpdateSortFilterTable(((Grouping)obj).NonDetailSortFiltersInScope, iDMap);
								((Grouping)obj).DetailSortFiltersInScope = UpdateSortFilterTable(((Grouping)obj).DetailSortFiltersInScope, iDMap);
							}
						}
					}
				}
				return report;
			}

			private static InScopeSortFilterHashtable UpdateSortFilterTable(InScopeSortFilterHashtable oldTable, Hashtable IDMap)
			{
				if (oldTable != null)
				{
					InScopeSortFilterHashtable inScopeSortFilterHashtable = new InScopeSortFilterHashtable(oldTable.Count);
					IDictionaryEnumerator enumerator = oldTable.GetEnumerator();
					while (enumerator.MoveNext())
					{
						int num = (int)enumerator.Key;
						IntList intList = (IntList)enumerator.Value;
						IntList intList2 = new IntList(intList.Count);
						for (int i = 0; i < intList.Count; i++)
						{
							intList2.Add((int)IDMap[intList[i]]);
						}
						inScopeSortFilterHashtable.Add(IDMap[num], intList2);
					}
					return inScopeSortFilterHashtable;
				}
				return null;
			}

			private static void AssignIDsForDataSets(DataSourceList dataSources, ProcessingContext context, Hashtable IDMap)
			{
				if (dataSources == null)
				{
					return;
				}
				for (int i = 0; i < dataSources.Count; i++)
				{
					DataSource dataSource = dataSources[i];
					if (dataSource.DataSets != null)
					{
						for (int j = 0; j < dataSource.DataSets.Count; j++)
						{
							DataSet dataSet = dataSource.DataSets[j];
							int iD = dataSet.ID;
							AssignIDsForIDOwnerBase(dataSet, context);
							AddToIDMap(dataSet, iD, IDMap);
						}
					}
				}
			}

			private static void AssignIDsForPageSection(PageSection pageSection, ProcessingContext context, Hashtable IDMap, SubReport subReport, ArrayList sortFilterOwners, SubReportInitialization initializationAction)
			{
				if (pageSection != null)
				{
					if (SubReportInitialization.RuntimeOnly != initializationAction)
					{
						AssignIDsForIDOwnerBase(pageSection, context);
					}
					AssignIDsForReportItemCollection(pageSection.ReportItems, context, IDMap, subReport, sortFilterOwners, initializationAction);
				}
			}

			private static void AssignIDsForReportItemCollection(ReportItemCollection reportItems, ProcessingContext context, Hashtable IDMap, SubReport subReport, ArrayList sortFilterOwners, SubReportInitialization initializationAction)
			{
				if (reportItems == null)
				{
					return;
				}
				if (SubReportInitialization.RuntimeOnly != initializationAction)
				{
					AssignIDsForIDOwnerBase(reportItems, context);
				}
				for (int i = 0; i < reportItems.Count; i++)
				{
					ReportItem reportItem = reportItems[i];
					int iD = reportItem.ID;
					if (SubReportInitialization.RuntimeOnly != initializationAction)
					{
						AssignIDsForIDOwnerBase(reportItem, context);
					}
					if (reportItem is TextBox)
					{
						IDMap?.Add(iD, reportItem.ID);
						TextBox textBox = (TextBox)reportItem;
						EndUserSort userSort = textBox.UserSort;
						if (userSort != null)
						{
							if (-1 != context.UserSortFilterContext.DataSetID)
							{
								userSort.DataSetID = context.UserSortFilterContext.DataSetID;
							}
							else if (IDMap != null)
							{
								userSort.DataSetID = (int)IDMap[userSort.DataSetID];
							}
							userSort.DetailScopeSubReports = subReport.DetailScopeSubReports;
						}
						if (initializationAction != 0 && subReport.ContainingScopes != null && 0 < subReport.ContainingScopes.Count)
						{
							if (textBox.ContainingScopes != null && 0 < textBox.ContainingScopes.Count)
							{
								textBox.ContainingScopes.InsertRange(0, subReport.ContainingScopes);
							}
							else
							{
								textBox.IsSubReportTopLevelScope = true;
								textBox.ContainingScopes = subReport.ContainingScopes;
							}
						}
					}
					else if (reportItem is Rectangle)
					{
						AssignIDsForReportItemCollection(((Rectangle)reportItem).ReportItems, context, IDMap, subReport, sortFilterOwners, initializationAction);
					}
					else if (reportItem is List)
					{
						List list = (List)reportItem;
						int iD2 = list.HierarchyDef.ID;
						if (SubReportInitialization.RuntimeOnly != initializationAction)
						{
							AssignIDsForIDOwnerBase(list.HierarchyDef, context);
							AddToIDMap(list.HierarchyDef.Grouping, iD2, IDMap);
							AddToSortFilterOwners(list.HierarchyDef, sortFilterOwners);
						}
						AssignIDsForReportItemCollection(list.ReportItems, context, IDMap, subReport, sortFilterOwners, initializationAction);
					}
					else if (reportItem is Matrix)
					{
						Matrix matrix = (Matrix)reportItem;
						AssignIDsForReportItemCollection(matrix.CornerReportItems, context, IDMap, subReport, sortFilterOwners, initializationAction);
						AssignIDsForMatrixHeading(matrix.Columns, context, IDMap, subReport, sortFilterOwners, initializationAction);
						AssignIDsForMatrixHeading(matrix.Rows, context, IDMap, subReport, sortFilterOwners, initializationAction);
						AssignIDsForReportItemCollection(matrix.CellReportItems, context, IDMap, subReport, sortFilterOwners, initializationAction);
						if (SubReportInitialization.RuntimeOnly != initializationAction)
						{
							AssignArrayOfIDs(matrix.CellIDs, context);
						}
					}
					else if (reportItem is Table)
					{
						Table obj = (Table)reportItem;
						AssignIDsForTableRows(obj.HeaderRows, context, IDMap, subReport, sortFilterOwners, initializationAction);
						AssignIDsForTableGroup(obj.TableGroups, context, IDMap, subReport, sortFilterOwners, initializationAction);
						AssignIDsForTableDetail(obj.TableDetail, context, IDMap, subReport, sortFilterOwners, initializationAction);
						AssignIDsForTableRows(obj.FooterRows, context, IDMap, subReport, sortFilterOwners, initializationAction);
					}
					else if (reportItem is Chart)
					{
						if (SubReportInitialization.RuntimeOnly != initializationAction)
						{
							Chart chart = (Chart)reportItem;
							AssignIDsForChartHeading(chart.Columns, context, IDMap);
							AssignIDsForChartHeading(chart.Rows, context, IDMap);
							if (chart.MultiChart != null)
							{
								AssignIDsForIDOwnerBase(chart.MultiChart, context);
							}
						}
					}
					else if (reportItem is CustomReportItem)
					{
						CustomReportItem customReportItem = (CustomReportItem)reportItem;
						if (SubReportInitialization.RuntimeOnly != initializationAction)
						{
							AssignIDsForCRIHeading(customReportItem.Columns, context, IDMap);
							AssignIDsForCRIHeading(customReportItem.Rows, context, IDMap);
						}
						if (customReportItem.AltReportItem != null)
						{
							AssignIDsForReportItemCollection(customReportItem.AltReportItem, context, IDMap, subReport, sortFilterOwners, initializationAction);
						}
						if (customReportItem.RenderReportItem != null)
						{
							AssignIDsForReportItemCollection(customReportItem.RenderReportItem, context, IDMap, subReport, sortFilterOwners, initializationAction);
						}
					}
					if (SubReportInitialization.RuntimeOnly != initializationAction && reportItem is DataRegion)
					{
						AddToIDMap((DataRegion)reportItem, iD, IDMap);
						sortFilterOwners.Add(reportItem);
					}
				}
			}

			private static void AddToIDMap(ISortFilterScope scope, int oldID, Hashtable IDMap)
			{
				if (scope != null)
				{
					IDMap.Add(oldID, scope.ID);
				}
			}

			private static void AddToSortFilterOwners(ReportHierarchyNode scope, ArrayList sortFilterOwners)
			{
				if (scope.Grouping != null)
				{
					sortFilterOwners.Add(scope.Grouping);
				}
			}

			private static void AssignIDsForPivotHeading(PivotHeading heading, ProcessingContext context, Hashtable IDMap)
			{
				if (heading != null)
				{
					int iD = heading.ID;
					AssignIDsForIDOwnerBase(heading, context);
					AddToIDMap(heading.Grouping, iD, IDMap);
					AssignArrayOfIDs(heading.IDs, context);
				}
			}

			private static void AssignIDsForTablixHeading(TablixHeading heading, ProcessingContext context, Hashtable IDMap)
			{
				if (heading != null)
				{
					int iD = heading.ID;
					AssignIDsForIDOwnerBase(heading, context);
					AddToIDMap(heading.Grouping, iD, IDMap);
				}
			}

			private static void AssignIDsForCRIHeading(CustomReportItemHeadingList headings, ProcessingContext context, Hashtable IDMap)
			{
				if (headings != null)
				{
					for (int i = 0; i < headings.Count; i++)
					{
						AssignIDsForTablixHeading(headings[i], context, IDMap);
						AssignIDsForCRIHeading(headings[i].InnerHeadings, context, IDMap);
					}
				}
			}

			private static void AssignIDsForChartHeading(ChartHeading heading, ProcessingContext context, Hashtable IDMap)
			{
				if (heading != null)
				{
					AssignIDsForPivotHeading(heading, context, IDMap);
					AssignIDsForChartHeading(heading.SubHeading, context, IDMap);
				}
			}

			private static void AssignIDsForMatrixHeading(MatrixHeading heading, ProcessingContext context, Hashtable IDMap, SubReport subReport, ArrayList sortFilterOwners, SubReportInitialization initializationAction)
			{
				if (heading != null)
				{
					if (SubReportInitialization.RuntimeOnly != initializationAction)
					{
						AssignIDsForPivotHeading(heading, context, IDMap);
						AddToSortFilterOwners(heading, sortFilterOwners);
					}
					AssignIDsForMatrixHeading(heading.SubHeading, context, IDMap, subReport, sortFilterOwners, initializationAction);
					AssignIDsForSubtotal(heading.Subtotal, context, IDMap, subReport, sortFilterOwners, initializationAction);
					AssignIDsForReportItemCollection(heading.ReportItems, context, IDMap, subReport, sortFilterOwners, initializationAction);
				}
			}

			private static void AssignIDsForSubtotal(Subtotal subtotal, ProcessingContext context, Hashtable IDMap, SubReport subReport, ArrayList sortFilterOwners, SubReportInitialization initializationAction)
			{
				if (subtotal != null)
				{
					if (SubReportInitialization.RuntimeOnly != initializationAction)
					{
						AssignIDsForIDOwnerBase(subtotal, context);
					}
					AssignIDsForReportItemCollection(subtotal.ReportItems, context, IDMap, subReport, sortFilterOwners, initializationAction);
				}
			}

			private static void AssignIDsForTableRows(TableRowList rows, ProcessingContext context, Hashtable IDMap, SubReport subReport, ArrayList sortFilterOwners, SubReportInitialization initializationAction)
			{
				if (rows == null)
				{
					return;
				}
				for (int i = 0; i < rows.Count; i++)
				{
					if (rows[i] != null)
					{
						if (SubReportInitialization.RuntimeOnly != initializationAction)
						{
							AssignIDsForIDOwnerBase(rows[i], context);
						}
						AssignIDsForReportItemCollection(rows[i].ReportItems, context, IDMap, subReport, sortFilterOwners, initializationAction);
						if (SubReportInitialization.RuntimeOnly != initializationAction)
						{
							AssignArrayOfIDs(rows[i].IDs, context);
						}
					}
				}
			}

			private static void AssignIDsForTableGroup(TableGroup group, ProcessingContext context, Hashtable IDMap, SubReport subReport, ArrayList sortFilterOwners, SubReportInitialization initializationAction)
			{
				if (group != null)
				{
					int iD = group.ID;
					if (SubReportInitialization.RuntimeOnly != initializationAction)
					{
						AssignIDsForIDOwnerBase(group, context);
						AddToIDMap(group.Grouping, iD, IDMap);
						AddToSortFilterOwners(group, sortFilterOwners);
					}
					AssignIDsForTableGroup(group.SubGroup, context, IDMap, subReport, sortFilterOwners, initializationAction);
					AssignIDsForTableRows(group.HeaderRows, context, IDMap, subReport, sortFilterOwners, initializationAction);
					AssignIDsForTableRows(group.FooterRows, context, IDMap, subReport, sortFilterOwners, initializationAction);
				}
			}

			private static void AssignIDsForTableDetail(TableDetail detail, ProcessingContext context, Hashtable IDMap, SubReport subReport, ArrayList sortFilterOwners, SubReportInitialization initializationAction)
			{
				if (detail != null)
				{
					if (SubReportInitialization.RuntimeOnly != initializationAction)
					{
						AssignIDsForIDOwnerBase(detail, context);
					}
					AssignIDsForTableRows(detail.DetailRows, context, IDMap, subReport, sortFilterOwners, initializationAction);
				}
			}

			private static void AssignIDsForIDOwnerBase(IDOwner idOwner, ProcessingContext context)
			{
				if (idOwner != null)
				{
					idOwner.ID = context.CreateIDForSubreport();
				}
			}

			private static void AssignArrayOfIDs(IntList ids, ProcessingContext context)
			{
				if (ids != null)
				{
					for (int i = 0; i < ids.Count; i++)
					{
						ids[i] = context.CreateIDForSubreport();
					}
				}
			}

			private static bool CalculateDuplicates(VariantResult textBoxResult, TextBox textBox, ProcessingContext processingContext)
			{
				bool flag = false;
				if (textBox.HideDuplicates != null)
				{
					if (textBox.HasOldResult)
					{
						if (textBoxResult.ErrorOccurred && textBox.OldResult.ErrorOccurred)
						{
							flag = true;
						}
						else if (textBoxResult.ErrorOccurred)
						{
							flag = false;
						}
						else if (textBox.OldResult.ErrorOccurred)
						{
							flag = false;
						}
						else if (textBoxResult.Value == null && textBox.OldResult.Value == null)
						{
							flag = true;
						}
						else if (textBoxResult.Value == null)
						{
							flag = false;
						}
						else if (textBoxResult.Value.Equals(textBox.OldResult.Value))
						{
							flag = true;
						}
					}
					if (!flag)
					{
						textBox.OldResult = textBoxResult;
					}
				}
				return flag;
			}

			private static string GetFormattedTextBoxValue(TextBoxInstanceInfo textBoxInstanceInfo, VariantResult textBoxResult, TextBox textBox, ProcessingContext processingContext)
			{
				if (textBoxResult.ErrorOccurred)
				{
					return RPRes.rsExpressionErrorValue;
				}
				if (textBoxResult.Value == null)
				{
					return null;
				}
				if (textBoxInstanceInfo != null && textBoxInstanceInfo.Duplicate && textBox.SharedFormatSettings)
				{
					return textBox.FormattedValue;
				}
				TypeCode typeCode = Type.GetTypeCode(textBoxResult.Value.GetType());
				string text = FormatTextboxValue(textBoxInstanceInfo, textBoxResult.Value, textBox, typeCode, processingContext);
				if (textBox.HideDuplicates != null)
				{
					textBox.FormattedValue = text;
				}
				return text;
			}

			private static int GetTextBoxStyleAttribute(Style style, string styleAttributeName, TextBoxInstanceInfo textBoxInstanceInfo, ref bool sharedFormatSettings, out string styleStringValue)
			{
				styleStringValue = null;
				int result = 0;
				object obj = null;
				AttributeInfo attributeInfo = style.StyleAttributes[styleAttributeName];
				if (attributeInfo != null)
				{
					if (attributeInfo.IsExpression)
					{
						result = 1;
						sharedFormatSettings = false;
						obj = textBoxInstanceInfo.GetStyleAttributeValue(attributeInfo.IntValue);
					}
					else
					{
						result = 2;
						obj = attributeInfo.Value;
					}
				}
				if (obj != null)
				{
					styleStringValue = (string)obj;
				}
				return result;
			}

			private static void GetTextBoxStyleAttribute(Style style, string styleAttributeName, TextBoxInstanceInfo textBoxInstanceInfo, ref bool sharedFormatSettings, out int styleIntValue)
			{
				styleIntValue = 0;
				AttributeInfo attributeInfo = style.StyleAttributes[styleAttributeName];
				if (attributeInfo == null)
				{
					return;
				}
				if (attributeInfo.IsExpression)
				{
					sharedFormatSettings = false;
					object styleAttributeValue = textBoxInstanceInfo.GetStyleAttributeValue(attributeInfo.IntValue);
					if (styleAttributeValue != null)
					{
						styleIntValue = (int)styleAttributeValue;
					}
				}
				else
				{
					styleIntValue = attributeInfo.IntValue;
				}
			}

			private static void GetAndValidateCalendar(Style style, TextBox textBox, TextBoxInstanceInfo textBoxInstanceInfo, int languageState, ref bool sharedFormatSettings, CultureInfo formattingCulture, ProcessingContext context, out Calendar formattingCalendar)
			{
				AttributeInfo attributeInfo = style.StyleAttributes["Calendar"];
				string text = null;
				Calendar calendar = null;
				bool flag = false;
				formattingCalendar = null;
				if (attributeInfo != null)
				{
					if (attributeInfo.IsExpression)
					{
						flag = true;
						text = (string)textBoxInstanceInfo.GetStyleAttributeValue(attributeInfo.IntValue);
						sharedFormatSettings = false;
					}
					else
					{
						text = attributeInfo.Value;
						switch (languageState)
						{
						case 1:
							flag = true;
							break;
						default:
							if (!textBox.CalendarValidated)
							{
								textBox.CalendarValidated = true;
								textBox.Calendar = (formattingCalendar = ProcessingValidator.CreateCalendar(text));
								return;
							}
							break;
						case 0:
							break;
						}
					}
				}
				if (flag || !textBox.CalendarValidated)
				{
					if (text != null && ProcessingValidator.ValidateCalendar(formattingCulture, text, textBox.ObjectType, textBox.Name, "Calendar", context.ErrorContext))
					{
						calendar = (formattingCalendar = ProcessingValidator.CreateCalendar(text));
					}
					if (!flag)
					{
						textBox.Calendar = calendar;
						textBox.CalendarValidated = true;
					}
				}
			}

			private static string FormatTextboxValue(TextBoxInstanceInfo textBoxInstanceInfo, object textBoxValue, TextBox textBox, TypeCode typeCode, ProcessingContext processingContext)
			{
				string styleStringValue = null;
				Style styleClass = textBox.StyleClass;
				CultureInfo cultureInfo = null;
				string styleStringValue2 = null;
				bool sharedFormatSettings = true;
				bool flag = false;
				bool flag2 = false;
				bool flag3 = false;
				int num = 0;
				bool flag4 = false;
				string text = null;
				Calendar calendar = null;
				Calendar formattingCalendar = null;
				try
				{
					if (styleClass != null)
					{
						GetTextBoxStyleAttribute(styleClass, "Format", textBoxInstanceInfo, ref sharedFormatSettings, out styleStringValue);
						num = GetTextBoxStyleAttribute(styleClass, "Language", textBoxInstanceInfo, ref sharedFormatSettings, out styleStringValue2);
						if (styleStringValue2 != null)
						{
							cultureInfo = new CultureInfo(styleStringValue2, useUserOverride: false);
							if (cultureInfo.IsNeutralCulture)
							{
								cultureInfo = CultureInfo.CreateSpecificCulture(styleStringValue2);
								cultureInfo = new CultureInfo(cultureInfo.Name, useUserOverride: false);
							}
						}
						else
						{
							num = 0;
							flag2 = true;
							cultureInfo = Thread.CurrentThread.CurrentCulture;
							if (processingContext.LanguageInstanceId != textBox.LanguageInstanceId)
							{
								textBox.CalendarValidated = false;
								textBox.Calendar = null;
								textBox.LanguageInstanceId = processingContext.LanguageInstanceId;
							}
						}
						if (typeCode == TypeCode.DateTime)
						{
							if (textBox.CalendarValidated)
							{
								if (textBox.Calendar != null)
								{
									formattingCalendar = textBox.Calendar;
								}
							}
							else
							{
								GetAndValidateCalendar(styleClass, textBox, textBoxInstanceInfo, num, ref sharedFormatSettings, cultureInfo, processingContext, out formattingCalendar);
							}
						}
					}
					if (cultureInfo != null && formattingCalendar != null)
					{
						if (flag2)
						{
							if (cultureInfo.DateTimeFormat.IsReadOnly)
							{
								cultureInfo = (CultureInfo)cultureInfo.Clone();
								flag3 = true;
							}
							else
							{
								calendar = cultureInfo.DateTimeFormat.Calendar;
							}
						}
						cultureInfo.DateTimeFormat.Calendar = formattingCalendar;
					}
					bool flag5 = false;
					if (styleStringValue != null && textBoxValue is IFormattable)
					{
						try
						{
							if (cultureInfo == null)
							{
								cultureInfo = Thread.CurrentThread.CurrentCulture;
								flag2 = true;
							}
							if (CompareWithInvariantCulture(styleStringValue, "x", ignoreCase: true) == 0)
							{
								flag4 = true;
							}
							text = ((IFormattable)textBoxValue).ToString(styleStringValue, cultureInfo);
							flag5 = true;
						}
						catch
						{
						}
					}
					if (!flag5)
					{
						CultureInfo cultureInfo2 = null;
						if ((!flag2 && cultureInfo != null) || flag3)
						{
							cultureInfo2 = Thread.CurrentThread.CurrentCulture;
							Thread.CurrentThread.CurrentCulture = cultureInfo;
							try
							{
								text = textBoxValue.ToString();
							}
							finally
							{
								if (cultureInfo2 != null)
								{
									Thread.CurrentThread.CurrentCulture = cultureInfo2;
								}
							}
						}
						else
						{
							text = textBoxValue.ToString();
						}
					}
				}
				finally
				{
					if (flag2 && calendar != null)
					{
						Global.Tracer.Assert(!Thread.CurrentThread.CurrentCulture.DateTimeFormat.IsReadOnly, "(!System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.IsReadOnly)");
						Thread.CurrentThread.CurrentCulture.DateTimeFormat.Calendar = calendar;
					}
				}
				if (!flag4 && styleClass != null)
				{
					if ((uint)(typeCode - 5) <= 10u)
					{
						flag = true;
					}
					if (flag)
					{
						int styleIntValue = 1;
						GetTextBoxStyleAttribute(styleClass, "NumeralVariant", textBoxInstanceInfo, ref sharedFormatSettings, out styleIntValue);
						if (styleIntValue > 2)
						{
							CultureInfo cultureInfo3 = cultureInfo;
							if (cultureInfo3 == null)
							{
								cultureInfo3 = Thread.CurrentThread.CurrentCulture;
							}
							string numberDecimalSeparator = cultureInfo3.NumberFormat.NumberDecimalSeparator;
							GetTextBoxStyleAttribute(styleClass, "NumeralLanguage", textBoxInstanceInfo, ref sharedFormatSettings, out styleStringValue2);
							if (styleStringValue2 != null)
							{
								cultureInfo = new CultureInfo(styleStringValue2, useUserOverride: false);
							}
							else if (cultureInfo == null)
							{
								cultureInfo = cultureInfo3;
							}
							bool numberTranslated = true;
							text = FormatDigitReplacement.FormatNumeralVariant(text, styleIntValue, cultureInfo, numberDecimalSeparator, out numberTranslated);
							if (!numberTranslated)
							{
								processingContext.ErrorContext.Register(ProcessingErrorCode.rsInvalidNumeralVariantForLanguage, Severity.Warning, textBox.ObjectType, textBox.Name, "NumeralVariant", styleIntValue.ToString(CultureInfo.InvariantCulture), cultureInfo.Name);
							}
						}
					}
				}
				textBox.SharedFormatSettings = sharedFormatSettings;
				return text;
			}

			internal static void EvalReportItemAttr(ReportItem reportItem, ReportItemInstance riInstance, ReportItemInstanceInfo riInstanceInfo, ProcessingContext processingContext)
			{
				if (processingContext.ShowHideType != 0)
				{
					if (!(reportItem is List))
					{
						((IShowHideReceiver)riInstanceInfo).ProcessReceiver(processingContext, riInstance.UniqueName);
					}
					if (reportItem is TextBox)
					{
						((IShowHideSender)riInstanceInfo).ProcessSender(processingContext, riInstance.UniqueName);
					}
				}
				EvaluateStyleAttributes(reportItem.ObjectType, reportItem.Name, reportItem.StyleClass, riInstance.UniqueName, riInstanceInfo.StyleAttributeValues, processingContext);
				ResetSubtotalReferences(processingContext);
				if (reportItem.Label != null)
				{
					string text = processingContext.NavigationInfo.RegisterLabel(processingContext.ReportRuntime.EvaluateReportItemLabelExpression(reportItem));
					if (text != null)
					{
						riInstanceInfo.Label = text;
					}
				}
				if (reportItem.Bookmark != null)
				{
					processingContext.NavigationInfo.ProcessBookmark(processingContext, reportItem, riInstance, riInstanceInfo);
				}
				if (reportItem.ToolTip != null && ExpressionInfo.Types.Constant != reportItem.ToolTip.Type)
				{
					riInstanceInfo.ToolTip = processingContext.ReportRuntime.EvaluateReportItemToolTipExpression(reportItem);
				}
			}

			internal static void ResetSubtotalReferences(ProcessingContext processingContext)
			{
				if (processingContext.HeadingInstance != null)
				{
					MatrixHeading matrixHeadingDef = processingContext.HeadingInstance.MatrixHeadingDef;
					bool num = ((Matrix)matrixHeadingDef.DataRegionDef).ProcessingInnerGrouping == Pivot.ProcessingInnerGroupings.Column;
					if (num && matrixHeadingDef.IsColumn)
					{
						processingContext.HeadingInstance = null;
					}
					if (!num && !matrixHeadingDef.IsColumn)
					{
						processingContext.HeadingInstance = processingContext.HeadingInstanceOld;
						processingContext.HeadingInstanceOld = null;
					}
				}
			}

			internal static void EvaluateStyleAttributes(ObjectType objectType, string objectName, Style style, int itemUniqueName, object[] values, ProcessingContext processingContext)
			{
				if (style != null && style.ExpressionList != null)
				{
					AttributeInfo attributeInfo = style.StyleAttributes["BorderColor"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleBorderColor(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["BorderColorLeft"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleBorderColorLeft(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["BorderColorRight"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleBorderColorRight(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["BorderColorTop"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleBorderColorTop(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["BorderColorBottom"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleBorderColorBottom(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["BorderStyle"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleBorderStyle(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["BorderStyleLeft"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleBorderStyleLeft(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["BorderStyleRight"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleBorderStyleRight(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["BorderStyleTop"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleBorderStyleTop(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["BorderStyleBottom"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleBorderStyleBottom(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["BorderWidth"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleBorderWidth(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["BorderWidthLeft"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleBorderWidthLeft(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["BorderWidthRight"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleBorderWidthRight(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["BorderWidthTop"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleBorderWidthTop(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["BorderWidthBottom"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleBorderWidthBottom(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["BackgroundColor"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleBackgroundColor(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["BackgroundGradientType"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleBackgroundGradientType(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["BackgroundGradientEndColor"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleBackgroundGradientEndColor(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					EvaluateBackgroundImage(objectType, objectName, itemUniqueName, style, values, processingContext);
					attributeInfo = style.StyleAttributes["BackgroundRepeat"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleBackgroundRepeat(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["FontStyle"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleFontStyle(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["FontFamily"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleFontFamily(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["FontSize"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleFontSize(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["FontWeight"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleFontWeight(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["Format"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleFormat(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["TextDecoration"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleTextDecoration(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["TextAlign"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleTextAlign(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["VerticalAlign"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleVerticalAlign(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["Color"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleColor(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["PaddingLeft"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStylePaddingLeft(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["PaddingRight"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStylePaddingRight(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["PaddingTop"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStylePaddingTop(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["PaddingBottom"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStylePaddingBottom(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["LineHeight"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleLineHeight(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["Direction"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleDirection(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["WritingMode"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleWritingMode(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["UnicodeBiDi"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleUnicodeBiDi(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["Language"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleLanguage(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["Calendar"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleCalendar(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["NumeralLanguage"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleNumeralLanguage(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
					attributeInfo = style.StyleAttributes["NumeralVariant"];
					if (attributeInfo != null && attributeInfo.IsExpression)
					{
						values[attributeInfo.IntValue] = processingContext.ReportRuntime.EvaluateStyleNumeralVariant(style, style.ExpressionList[attributeInfo.IntValue], objectType, objectName);
					}
				}
			}

			private static void EvaluateBackgroundImage(ObjectType objectType, string objectName, int itemUniqueName, Style style, object[] values, ProcessingContext processingContext)
			{
				AttributeInfo attributeInfo = style.StyleAttributes["BackgroundImageSource"];
				if (attributeInfo == null)
				{
					return;
				}
				Global.Tracer.Assert(!attributeInfo.IsExpression, "(!sourceAttribute.IsExpression)");
				Microsoft.ReportingServices.OnDemandReportRendering.Image.SourceType intValue = (Microsoft.ReportingServices.OnDemandReportRendering.Image.SourceType)attributeInfo.IntValue;
				AttributeInfo attributeInfo2 = style.StyleAttributes["BackgroundImageValue"];
				Global.Tracer.Assert(attributeInfo2 != null, "(null != valueAttribute)");
				object obj = null;
				AttributeInfo attributeInfo3 = null;
				string mimeType = null;
				switch (intValue)
				{
				case Microsoft.ReportingServices.OnDemandReportRendering.Image.SourceType.External:
				{
					string text2 = (!attributeInfo2.IsExpression) ? attributeInfo2.Value : processingContext.ReportRuntime.EvaluateStyleBackgroundUrlImageValue(style, style.ExpressionList[attributeInfo2.IntValue], objectType, objectName);
					obj = text2;
					if (text2 == null || processingContext.ImageStreamNames.ContainsKey(text2))
					{
						break;
					}
					byte[] imageData = null;
					GetExternalImage(processingContext, text2, objectType, objectName, out imageData, out mimeType);
					if (imageData == null)
					{
						break;
					}
					if (processingContext.InPageSection && !processingContext.CreatePageSectionImageChunks)
					{
						obj = new ImageData(imageData, mimeType);
					}
					else
					{
						string text3 = "BG" + Guid.NewGuid().ToString();
						using (Stream stream2 = processingContext.CreateReportChunkCallback(text3, ReportChunkTypes.Image, mimeType))
						{
							stream2.Write(imageData, 0, imageData.Length);
						}
						processingContext.ImageStreamNames[text2] = new ImageInfo(text3, mimeType);
					}
					attributeInfo3 = style.StyleAttributes["BackgroundImageMIMEType"];
					break;
				}
				case Microsoft.ReportingServices.OnDemandReportRendering.Image.SourceType.Embedded:
				{
					string text4 = (!attributeInfo2.IsExpression) ? attributeInfo2.Value : processingContext.ReportRuntime.EvaluateStyleBackgroundEmbeddedImageValue(style, style.ExpressionList[attributeInfo2.IntValue], processingContext.EmbeddedImages, objectType, objectName);
					obj = text4;
					attributeInfo3 = null;
					mimeType = null;
					break;
				}
				case Microsoft.ReportingServices.OnDemandReportRendering.Image.SourceType.Database:
				{
					byte[] array = (!attributeInfo2.IsExpression) ? null : processingContext.ReportRuntime.EvaluateStyleBackgroundDatabaseImageValue(style, style.ExpressionList[attributeInfo2.IntValue], objectType, objectName);
					attributeInfo3 = style.StyleAttributes["BackgroundImageMIMEType"];
					Global.Tracer.Assert(attributeInfo3 != null, "(null != mimeTypeAttribute)");
					mimeType = ((!attributeInfo3.IsExpression) ? attributeInfo3.Value : processingContext.ReportRuntime.EvaluateStyleBackgroundImageMIMEType(style, style.ExpressionList[attributeInfo2.IntValue], objectType, objectName));
					if (array == null)
					{
						break;
					}
					if (processingContext.InPageSection && !processingContext.CreatePageSectionImageChunks)
					{
						obj = new ImageData(array, mimeType);
					}
					else if (processingContext.CreateReportChunkCallback != null)
					{
						string text = "BG" + Guid.NewGuid().ToString();
						using (Stream stream = processingContext.CreateReportChunkCallback(text, ReportChunkTypes.Image, mimeType))
						{
							stream.Write(array, 0, array.Length);
						}
						obj = text;
					}
					break;
				}
				default:
					obj = null;
					attributeInfo3 = null;
					mimeType = null;
					break;
				}
				if (attributeInfo2.IsExpression)
				{
					values[attributeInfo2.IntValue] = obj;
				}
				if (attributeInfo3 != null && attributeInfo3.IsExpression)
				{
					values[attributeInfo3.IntValue] = mimeType;
				}
			}
		}

		internal abstract class RuntimeDataRegionObj : IScope
		{
			protected ProcessingContext m_processingContext;

			protected bool m_processedPreviousAggregates;

			internal ProcessingContext ProcessingContext => m_processingContext;

			protected abstract IScope OuterScope
			{
				get;
			}

			protected virtual string ScopeName => null;

			internal virtual bool TargetForNonDetailSort
			{
				get
				{
					if (OuterScope != null)
					{
						return OuterScope.TargetForNonDetailSort;
					}
					return false;
				}
			}

			protected virtual int[] SortFilterExpressionScopeInfoIndices
			{
				get
				{
					Global.Tracer.Assert(condition: false);
					return null;
				}
			}

			bool IScope.TargetForNonDetailSort => TargetForNonDetailSort;

			int[] IScope.SortFilterExpressionScopeInfoIndices => SortFilterExpressionScopeInfoIndices;

			protected RuntimeDataRegionObj(ProcessingContext processingContext)
			{
				m_processingContext = processingContext;
			}

			protected RuntimeDataRegionObj(RuntimeDataRegionObj outerDataRegion)
			{
				m_processingContext = outerDataRegion.ProcessingContext;
			}

			internal virtual bool IsTargetForSort(int index, bool detailSort)
			{
				if (OuterScope != null)
				{
					return OuterScope.IsTargetForSort(index, detailSort);
				}
				return false;
			}

			internal abstract void NextRow();

			internal abstract bool SortAndFilter();

			internal abstract void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup);

			internal abstract void CreateInstances(ReportItemInstance riInstance, IList instanceList, RenderingPagesRangesList pagesList);

			internal abstract void SetupEnvironment();

			bool IScope.IsTargetForSort(int index, bool detailSort)
			{
				return IsTargetForSort(index, detailSort);
			}

			void IScope.ReadRow(DataActions dataAction)
			{
				ReadRow(dataAction);
			}

			bool IScope.InScope(string scope)
			{
				return InScope(scope);
			}

			IScope IScope.GetOuterScope(bool includeSubReportContainingScope)
			{
				return OuterScope;
			}

			string IScope.GetScopeName()
			{
				return ScopeName;
			}

			int IScope.RecursiveLevel(string scope)
			{
				return GetRecursiveLevel(scope);
			}

			bool IScope.TargetScopeMatched(int index, bool detailSort)
			{
				return TargetScopeMatched(index, detailSort);
			}

			void IScope.GetScopeValues(IHierarchyObj targetScopeObj, VariantList[] scopeValues, ref int index)
			{
				GetScopeValues(targetScopeObj, scopeValues, ref index);
			}

			void IScope.GetGroupNameValuePairs(Dictionary<string, object> pairs)
			{
				GetGroupNameValuePairs(pairs);
			}

			internal static void AddAggregate(ref DataAggregateObjList aggregates, DataAggregateObj aggregate)
			{
				if (aggregates == null)
				{
					aggregates = new DataAggregateObjList();
				}
				aggregates.Add(aggregate);
			}

			internal static void CreateAggregates(ProcessingContext processingContext, DataAggregateInfoList aggDefs, ref DataAggregateObjList nonCustomAggregates, ref DataAggregateObjList customAggregates)
			{
				if (aggDefs == null || 0 >= aggDefs.Count)
				{
					return;
				}
				for (int i = 0; i < aggDefs.Count; i++)
				{
					DataAggregateObj aggregate = new DataAggregateObj(aggDefs[i], processingContext);
					if (DataAggregateInfo.AggregateTypes.Aggregate == aggDefs[i].AggregateType)
					{
						AddAggregate(ref customAggregates, aggregate);
					}
					else
					{
						AddAggregate(ref nonCustomAggregates, aggregate);
					}
				}
			}

			internal static void CreateAggregates(ProcessingContext processingContext, DataAggregateInfoList aggDefs, ref DataAggregateObjList aggregates)
			{
				if (aggDefs != null && 0 < aggDefs.Count)
				{
					for (int i = 0; i < aggDefs.Count; i++)
					{
						DataAggregateObj aggregate = new DataAggregateObj(aggDefs[i], processingContext);
						AddAggregate(ref aggregates, aggregate);
					}
				}
			}

			internal static void CreateAggregates(ProcessingContext processingContext, RunningValueInfoList aggDefs, ref DataAggregateObjList aggregates)
			{
				if (aggDefs != null && 0 < aggDefs.Count)
				{
					for (int i = 0; i < aggDefs.Count; i++)
					{
						DataAggregateObj aggregate = new DataAggregateObj(aggDefs[i], processingContext);
						AddAggregate(ref aggregates, aggregate);
					}
				}
			}

			internal static void UpdateAggregates(ProcessingContext processingContext, DataAggregateObjList aggregates, bool updateAndSetup)
			{
				if (aggregates == null)
				{
					return;
				}
				for (int i = 0; i < aggregates.Count; i++)
				{
					DataAggregateObj dataAggregateObj = aggregates[i];
					dataAggregateObj.Update();
					if (updateAndSetup)
					{
						processingContext.ReportObjectModel.AggregatesImpl.Set(dataAggregateObj.Name, dataAggregateObj.AggregateDef, dataAggregateObj.DuplicateNames, dataAggregateObj.AggregateResult());
					}
				}
			}

			protected void SetupAggregates(DataAggregateObjList aggregates)
			{
				if (aggregates != null)
				{
					for (int i = 0; i < aggregates.Count; i++)
					{
						DataAggregateObj dataAggregateObj = aggregates[i];
						m_processingContext.ReportObjectModel.AggregatesImpl.Set(dataAggregateObj.Name, dataAggregateObj.AggregateDef, dataAggregateObj.DuplicateNames, dataAggregateObj.AggregateResult());
					}
				}
			}

			protected void SetupEnvironment(DataAggregateObjList nonCustomAggregates, DataAggregateObjList customAggregates, FieldImpl[] dataRow)
			{
				SetupAggregates(nonCustomAggregates);
				SetupAggregates(customAggregates);
				SetupFields(dataRow);
				m_processingContext.ReportRuntime.CurrentScope = this;
			}

			protected void SetupFields(FieldImpl[] dataRow)
			{
				m_processingContext.ReportObjectModel.FieldsImpl.SetFields(dataRow);
			}

			internal static void SetupRunningValues(ProcessingContext processingContext, RunningValueInfoList rvDefs, DataAggregateObjResult[] rvValues)
			{
				int startIndex = 0;
				SetupRunningValues(processingContext, ref startIndex, rvDefs, rvValues);
			}

			protected void SetupRunningValues(RunningValueInfoList rvDefs, DataAggregateObjResult[] rvValues)
			{
				int startIndex = 0;
				SetupRunningValues(m_processingContext, ref startIndex, rvDefs, rvValues);
			}

			protected void SetupRunningValues(ref int startIndex, RunningValueInfoList rvDefs, DataAggregateObjResult[] rvValues)
			{
				SetupRunningValues(m_processingContext, ref startIndex, rvDefs, rvValues);
			}

			private static void SetupRunningValues(ProcessingContext processingContext, ref int startIndex, RunningValueInfoList rvDefs, DataAggregateObjResult[] rvValues)
			{
				if (rvDefs != null && rvValues != null)
				{
					for (int i = 0; i < rvDefs.Count; i++)
					{
						processingContext.ReportObjectModel.AggregatesImpl.Set(rvDefs[i].Name, rvDefs[i], rvDefs[i].DuplicateNames, rvValues[startIndex + i]);
					}
					startIndex += rvDefs.Count;
				}
			}

			internal abstract void ReadRow(DataActions dataAction);

			internal abstract bool InScope(string scope);

			protected Hashtable GetScopeNames(RuntimeDataRegionObj currentScope, string targetScope, ref bool inPivotCell, out bool inScope)
			{
				inScope = false;
				Hashtable hashtable = null;
				if (!inPivotCell)
				{
					hashtable = new Hashtable();
				}
				for (IScope scope = currentScope; scope != null; scope = scope.GetOuterScope(includeSubReportContainingScope: false))
				{
					string scopeName = scope.GetScopeName();
					if (scopeName != null)
					{
						if (!inScope && scopeName.Equals(targetScope))
						{
							inScope = true;
							if (hashtable == null)
							{
								return null;
							}
						}
						if (hashtable != null)
						{
							Grouping value = null;
							if (scope is RuntimeGroupLeafObj)
							{
								value = ((RuntimeGroupLeafObj)scope).GroupingDef;
							}
							hashtable.Add(scopeName, value);
						}
					}
					else if (scope is RuntimePivotCell || scope is RuntimeTablixCell)
					{
						inPivotCell = true;
						if (!inScope)
						{
							inScope = scope.InScope(targetScope);
						}
						return null;
					}
				}
				return hashtable;
			}

			protected Hashtable GetScopeNames(RuntimeDataRegionObj currentScope, string targetScope, ref bool inPivotCell, out int level)
			{
				level = -1;
				Hashtable hashtable = null;
				if (!inPivotCell)
				{
					hashtable = new Hashtable();
				}
				for (IScope scope = currentScope; scope != null; scope = scope.GetOuterScope(includeSubReportContainingScope: false))
				{
					string scopeName = scope.GetScopeName();
					if (scopeName != null)
					{
						Grouping grouping = null;
						if (scope is RuntimeGroupLeafObj)
						{
							grouping = ((RuntimeGroupLeafObj)scope).GroupingDef;
							if (-1 == level && scopeName.Equals(targetScope))
							{
								level = grouping.RecursiveLevel;
								if (hashtable == null)
								{
									return null;
								}
							}
						}
						hashtable?.Add(scopeName, grouping);
					}
					else if (scope is RuntimePivotCell || scope is RuntimeTablixCell)
					{
						inPivotCell = true;
						if (-1 == level)
						{
							level = scope.RecursiveLevel(targetScope);
						}
						return null;
					}
				}
				return hashtable;
			}

			protected Hashtable GetScopeNames(RuntimeDataRegionObj currentScope, ref bool inPivotCell, Dictionary<string, object> nameValuePairs)
			{
				Hashtable hashtable = null;
				if (!inPivotCell)
				{
					hashtable = new Hashtable();
				}
				for (IScope scope = currentScope; scope != null; scope = scope.GetOuterScope(includeSubReportContainingScope: false))
				{
					string scopeName = scope.GetScopeName();
					if (scopeName != null)
					{
						Grouping grouping = null;
						if (scope is RuntimeGroupLeafObj)
						{
							grouping = ((RuntimeGroupLeafObj)scope).GroupingDef;
							AddGroupNameValuePair(m_processingContext, grouping, nameValuePairs);
						}
						hashtable?.Add(scopeName, grouping);
					}
					else if (scope is RuntimePivotCell || scope is RuntimeTablixCell)
					{
						inPivotCell = true;
						scope.GetGroupNameValuePairs(nameValuePairs);
						hashtable = null;
					}
				}
				return hashtable;
			}

			internal static void AddGroupNameValuePair(ProcessingContext processingContext, Grouping grouping, Dictionary<string, object> nameValuePairs)
			{
				if (grouping == null)
				{
					return;
				}
				Global.Tracer.Assert(grouping.GroupExpressions != null && 0 < grouping.GroupExpressions.Count);
				ExpressionInfo expressionInfo = grouping.GroupExpressions[0];
				if (expressionInfo.Type != ExpressionInfo.Types.Field)
				{
					return;
				}
				try
				{
					FieldImpl fieldImpl = processingContext.ReportObjectModel.FieldsImpl[expressionInfo.IntValue];
					if (fieldImpl.FieldDef != null)
					{
						object value = fieldImpl.Value;
						if (!nameValuePairs.ContainsKey(fieldImpl.FieldDef.DataField))
						{
							nameValuePairs.Add(fieldImpl.FieldDef.DataField, (value is DBNull) ? null : value);
						}
					}
				}
				catch (Exception ex)
				{
					if (AsynchronousExceptionDetection.IsStoppingException(ex))
					{
						throw;
					}
					Global.Tracer.Trace(TraceLevel.Verbose, ex.Message);
				}
			}

			protected bool DataRegionInScope(DataRegion dataRegionDef, string scope)
			{
				if (dataRegionDef.ScopeNames == null)
				{
					bool inPivotCell = dataRegionDef.InPivotCell;
					dataRegionDef.ScopeNames = GetScopeNames(this, scope, ref inPivotCell, out bool inScope);
					dataRegionDef.InPivotCell = inPivotCell;
					return inScope;
				}
				return dataRegionDef.ScopeNames.Contains(scope);
			}

			protected virtual int GetRecursiveLevel(string scope)
			{
				return -1;
			}

			protected int DataRegionRecursiveLevel(DataRegion dataRegionDef, string scope)
			{
				if (scope == null)
				{
					return -1;
				}
				if (dataRegionDef.ScopeNames == null)
				{
					bool inPivotCell = dataRegionDef.InPivotCell;
					dataRegionDef.ScopeNames = GetScopeNames(this, scope, ref inPivotCell, out int level);
					dataRegionDef.InPivotCell = inPivotCell;
					return level;
				}
				return (dataRegionDef.ScopeNames[scope] as Grouping)?.RecursiveLevel ?? (-1);
			}

			protected void DataRegionGetGroupNameValuePairs(DataRegion dataRegionDef, Dictionary<string, object> nameValuePairs)
			{
				if (dataRegionDef.ScopeNames == null)
				{
					bool inPivotCell = dataRegionDef.InPivotCell;
					dataRegionDef.ScopeNames = GetScopeNames(this, ref inPivotCell, nameValuePairs);
					dataRegionDef.InPivotCell = inPivotCell;
				}
				else
				{
					IEnumerator enumerator = dataRegionDef.ScopeNames.Values.GetEnumerator();
					while (enumerator.MoveNext())
					{
						AddGroupNameValuePair(m_processingContext, enumerator.Current as Grouping, nameValuePairs);
					}
				}
			}

			protected void ScopeNextNonAggregateRow(DataAggregateObjList aggregates, DataRowList dataRows)
			{
				UpdateAggregates(m_processingContext, aggregates, updateAndSetup: true);
				CommonNextRow(dataRows);
			}

			internal static void CommonFirstRow(FieldsImpl fields, ref bool firstRowIsAggregate, ref FieldImpl[] firstRow)
			{
				if (firstRowIsAggregate || firstRow == null)
				{
					firstRow = fields.GetAndSaveFields();
					firstRowIsAggregate = fields.IsAggregateRow;
				}
			}

			protected void CommonNextRow(DataRowList dataRows)
			{
				if (dataRows != null)
				{
					RuntimeDetailObj.SaveData(dataRows, m_processingContext);
				}
				SendToInner();
			}

			protected virtual void SendToInner()
			{
				Global.Tracer.Assert(condition: false);
			}

			protected void ScopeNextAggregateRow(RuntimeUserSortTargetInfo sortTargetInfo)
			{
				if (sortTargetInfo != null)
				{
					if (sortTargetInfo.AggregateRows == null)
					{
						sortTargetInfo.AggregateRows = new AggregateRowList();
					}
					AggregateRow value = new AggregateRow(m_processingContext);
					sortTargetInfo.AggregateRows.Add(value);
					if (!sortTargetInfo.TargetForNonDetailSort)
					{
						return;
					}
				}
				SendToInner();
			}

			protected void ScopeFinishSorting(ref FieldImpl[] firstRow, RuntimeUserSortTargetInfo sortTargetInfo)
			{
				Global.Tracer.Assert(sortTargetInfo != null, "(null != sortTargetInfo)");
				firstRow = null;
				sortTargetInfo.SortTree.Traverse(ProcessingStages.UserSortFilter, ascending: true);
				sortTargetInfo.SortTree = null;
				if (sortTargetInfo.AggregateRows != null)
				{
					for (int i = 0; i < sortTargetInfo.AggregateRows.Count; i++)
					{
						sortTargetInfo.AggregateRows[i].SetFields(m_processingContext);
						SendToInner();
					}
					sortTargetInfo.AggregateRows = null;
				}
			}

			internal virtual bool TargetScopeMatched(int index, bool detailSort)
			{
				Global.Tracer.Assert(condition: false);
				return false;
			}

			internal virtual void GetScopeValues(IHierarchyObj targetScopeObj, VariantList[] scopeValues, ref int index)
			{
				Global.Tracer.Assert(condition: false);
			}

			protected void ReleaseDataRows(DataActions finishedDataAction, ref DataActions dataAction, ref DataRowList dataRows)
			{
				dataAction &= ~finishedDataAction;
				if (dataAction == DataActions.None)
				{
					dataRows = null;
				}
			}

			protected void DetailHandleSortFilterEvent(DataRegion dataRegionDef, IScope outerScope, int rowIndex)
			{
				RuntimeSortFilterEventInfoList runtimeSortFilterInfo = m_processingContext.RuntimeSortFilterInfo;
				if (runtimeSortFilterInfo == null || dataRegionDef.SortFilterSourceDetailScopeInfo == null || outerScope.TargetForNonDetailSort)
				{
					return;
				}
				for (int i = 0; i < runtimeSortFilterInfo.Count; i++)
				{
					RuntimeSortFilterEventInfo runtimeSortFilterEventInfo = runtimeSortFilterInfo[i];
					if (runtimeSortFilterEventInfo.EventSource.ContainingScopes == null || 0 >= runtimeSortFilterEventInfo.EventSource.ContainingScopes.Count || -1 == dataRegionDef.SortFilterSourceDetailScopeInfo[i] || !outerScope.TargetScopeMatched(i, detailSort: false) || m_processingContext.ReportObjectModel.FieldsImpl.GetRowIndex() != dataRegionDef.SortFilterSourceDetailScopeInfo[i])
					{
						continue;
					}
					if (runtimeSortFilterEventInfo.EventSource.ContainingScopes.LastEntry == null)
					{
						ReportItem parent = runtimeSortFilterEventInfo.EventSource.Parent;
						if (runtimeSortFilterEventInfo.EventSource.IsSubReportTopLevelScope)
						{
							while (parent != null && !(parent is SubReport))
							{
								parent = parent.Parent;
							}
							Global.Tracer.Assert(parent is SubReport, "(parent is SubReport)");
							parent = parent.Parent;
						}
						if (parent == dataRegionDef)
						{
							Global.Tracer.Assert(runtimeSortFilterEventInfo.EventSourceScope == null, "(null == sortFilterInfo.EventSourceScope)");
							runtimeSortFilterEventInfo.EventSourceScope = this;
							runtimeSortFilterEventInfo.EventSourceDetailIndex = rowIndex;
						}
					}
					if (runtimeSortFilterEventInfo.DetailScopes == null)
					{
						runtimeSortFilterEventInfo.DetailScopes = new RuntimeDataRegionObjList();
						runtimeSortFilterEventInfo.DetailScopeIndices = new IntList();
					}
					runtimeSortFilterEventInfo.DetailScopes.Add(this);
					runtimeSortFilterEventInfo.DetailScopeIndices.Add(rowIndex);
				}
			}

			protected void DetailGetScopeValues(IScope outerScope, IHierarchyObj targetScopeObj, VariantList[] scopeValues, ref int index)
			{
				Global.Tracer.Assert(targetScopeObj == null, "(null == targetScopeObj)");
				outerScope.GetScopeValues(targetScopeObj, scopeValues, ref index);
				Global.Tracer.Assert(index < scopeValues.Length, "(index < scopeValues.Length)");
				VariantList variantList = new VariantList(1);
				variantList.Add(m_processingContext.ReportObjectModel.FieldsImpl.GetRowIndex());
				scopeValues[index++] = variantList;
			}

			protected bool DetailTargetScopeMatched(DataRegion dataRegionDef, IScope outerScope, int index)
			{
				if (m_processingContext.RuntimeSortFilterInfo != null)
				{
					RuntimeSortFilterEventInfo runtimeSortFilterEventInfo = m_processingContext.RuntimeSortFilterInfo[index];
					if (runtimeSortFilterEventInfo != null && runtimeSortFilterEventInfo.DetailScopes != null)
					{
						for (int i = 0; i < runtimeSortFilterEventInfo.DetailScopes.Count; i++)
						{
							if (this == runtimeSortFilterEventInfo.DetailScopes[i] && dataRegionDef.CurrentDetailRowIndex == runtimeSortFilterEventInfo.DetailScopeIndices[i])
							{
								return true;
							}
						}
					}
				}
				return false;
			}

			protected virtual void GetGroupNameValuePairs(Dictionary<string, object> pairs)
			{
			}
		}

		internal class RuntimeSortHierarchyObj : IHierarchyObj
		{
			private class SortHierarchyStructure
			{
				internal RuntimeSortFilterEventInfo SortInfo;

				internal int SortIndex;

				internal BTreeNode SortTree;

				internal SortHierarchyStructure(IHierarchyObj owner, int sortIndex, RuntimeSortFilterEventInfoList sortInfoList, IntList sortInfoIndices)
				{
					SortIndex = sortIndex;
					SortInfo = sortInfoList[sortInfoIndices[sortIndex]];
					SortTree = new BTreeNode(owner);
				}
			}

			private IHierarchyObj m_hierarchyRoot;

			private SortHierarchyStructure m_sortHierarchyStruct;

			private ISortDataHolder m_dataHolder;

			IHierarchyObj IHierarchyObj.HierarchyRoot => m_hierarchyRoot;

			ProcessingContext IHierarchyObj.ProcessingContext => m_hierarchyRoot.ProcessingContext;

			BTreeNode IHierarchyObj.SortTree
			{
				get
				{
					if (m_sortHierarchyStruct != null)
					{
						return m_sortHierarchyStruct.SortTree;
					}
					return null;
				}
				set
				{
					if (m_sortHierarchyStruct != null)
					{
						m_sortHierarchyStruct.SortTree = value;
					}
					else
					{
						Global.Tracer.Assert(condition: false);
					}
				}
			}

			int IHierarchyObj.ExpressionIndex
			{
				get
				{
					if (m_sortHierarchyStruct != null)
					{
						return m_sortHierarchyStruct.SortIndex;
					}
					return -1;
				}
			}

			IntList IHierarchyObj.SortFilterInfoIndices => m_hierarchyRoot.SortFilterInfoIndices;

			bool IHierarchyObj.IsDetail => false;

			internal RuntimeSortHierarchyObj(IHierarchyObj outerHierarchy)
			{
				m_hierarchyRoot = outerHierarchy.HierarchyRoot;
				ProcessingContext processingContext = m_hierarchyRoot.ProcessingContext;
				IntList sortFilterInfoIndices = m_hierarchyRoot.SortFilterInfoIndices;
				int num = outerHierarchy.ExpressionIndex + 1;
				if (sortFilterInfoIndices == null || num >= sortFilterInfoIndices.Count)
				{
					if (m_hierarchyRoot is RuntimeListDetailObj)
					{
						m_dataHolder = new RuntimeListDetailObj((RuntimeListDetailObj)m_hierarchyRoot);
					}
					else if (m_hierarchyRoot is RuntimeTableDetailObj)
					{
						m_dataHolder = new RuntimeTableDetailObj((RuntimeTableDetailObj)m_hierarchyRoot);
					}
					else
					{
						m_dataHolder = new RuntimeSortDataHolder(m_hierarchyRoot);
					}
				}
				else
				{
					m_sortHierarchyStruct = new SortHierarchyStructure(this, num, processingContext.RuntimeSortFilterInfo, sortFilterInfoIndices);
				}
			}

			IHierarchyObj IHierarchyObj.CreateHierarchyObj()
			{
				return new RuntimeSortHierarchyObj(this);
			}

			ProcessingMessageList IHierarchyObj.RegisterComparisonError(string propertyName)
			{
				return m_hierarchyRoot.ProcessingContext.RegisterComparisonErrorForSortFilterEvent(propertyName);
			}

			void IHierarchyObj.NextRow()
			{
				if (m_dataHolder != null)
				{
					m_dataHolder.NextRow();
				}
				else if (m_sortHierarchyStruct != null)
				{
					object sortOrder = m_sortHierarchyStruct.SortInfo.GetSortOrder(m_hierarchyRoot.ProcessingContext.ReportRuntime);
					m_sortHierarchyStruct.SortTree.NextRow(sortOrder);
				}
			}

			void IHierarchyObj.Traverse(ProcessingStages operation)
			{
				if (m_sortHierarchyStruct != null)
				{
					bool ascending = true;
					if (m_sortHierarchyStruct.SortInfo.EventSource.UserSort.SortExpressionScope == null)
					{
						ascending = m_sortHierarchyStruct.SortInfo.SortDirection;
					}
					m_sortHierarchyStruct.SortTree.Traverse(operation, ascending);
				}
				if (m_dataHolder != null)
				{
					m_dataHolder.Traverse(operation);
				}
			}

			void IHierarchyObj.ReadRow()
			{
				Global.Tracer.Assert(condition: false);
			}

			void IHierarchyObj.ProcessUserSort()
			{
				Global.Tracer.Assert(condition: false);
			}

			void IHierarchyObj.MarkSortInfoProcessed(RuntimeSortFilterEventInfoList runtimeSortFilterInfo)
			{
				Global.Tracer.Assert(condition: false);
			}

			void IHierarchyObj.AddSortInfoIndex(int sortInfoIndex, RuntimeSortFilterEventInfo sortInfo)
			{
				Global.Tracer.Assert(condition: false);
			}
		}

		internal class RuntimeHierarchyObj : RuntimeDataRegionObj, IHierarchyObj
		{
			protected RuntimeGroupingObj m_grouping;

			protected RuntimeExpressionInfo m_expression;

			protected RuntimeHierarchyObj m_hierarchyRoot;

			protected RuntimeHierarchyObjList m_hierarchyObjs;

			internal RuntimeHierarchyObjList HierarchyObjs => m_hierarchyObjs;

			protected override IScope OuterScope
			{
				get
				{
					Global.Tracer.Assert(condition: false);
					return null;
				}
			}

			protected virtual IHierarchyObj HierarchyRoot => m_hierarchyRoot;

			protected virtual BTreeNode SortTree
			{
				get
				{
					return m_grouping.Tree;
				}
				set
				{
					m_grouping.Tree = value;
				}
			}

			protected virtual int ExpressionIndex
			{
				get
				{
					if (m_expression != null)
					{
						return m_expression.ExpressionIndex;
					}
					Global.Tracer.Assert(condition: false);
					return -1;
				}
			}

			protected virtual DataRowList SortDataRows
			{
				get
				{
					Global.Tracer.Assert(condition: false);
					return null;
				}
			}

			protected virtual IntList SortFilterInfoIndices
			{
				get
				{
					Global.Tracer.Assert(condition: false);
					return null;
				}
			}

			protected virtual bool IsDetail => false;

			IHierarchyObj IHierarchyObj.HierarchyRoot => HierarchyRoot;

			ProcessingContext IHierarchyObj.ProcessingContext => m_processingContext;

			BTreeNode IHierarchyObj.SortTree
			{
				get
				{
					return SortTree;
				}
				set
				{
					SortTree = value;
				}
			}

			int IHierarchyObj.ExpressionIndex => ExpressionIndex;

			IntList IHierarchyObj.SortFilterInfoIndices => SortFilterInfoIndices;

			bool IHierarchyObj.IsDetail => IsDetail;

			protected RuntimeHierarchyObj(ProcessingContext processingContext)
				: base(processingContext)
			{
			}

			internal RuntimeHierarchyObj(RuntimeHierarchyObj outerHierarchy)
				: base(outerHierarchy)
			{
				ConstructorHelper(outerHierarchy.m_expression.ExpressionIndex + 1, outerHierarchy.m_hierarchyRoot);
			}

			IHierarchyObj IHierarchyObj.CreateHierarchyObj()
			{
				return CreateHierarchyObj();
			}

			ProcessingMessageList IHierarchyObj.RegisterComparisonError(string propertyName)
			{
				return RegisterComparisonError(propertyName);
			}

			void IHierarchyObj.NextRow()
			{
				NextRow();
			}

			void IHierarchyObj.Traverse(ProcessingStages operation)
			{
				switch (operation)
				{
				case ProcessingStages.SortAndFilter:
					SortAndFilter();
					break;
				case ProcessingStages.RunningValues:
					CalculateRunningValues();
					break;
				case ProcessingStages.CreatingInstances:
					CreateInstances();
					break;
				default:
					Global.Tracer.Assert(condition: false, "Invalid processing stage for RuntimeHierarchyObj");
					break;
				}
			}

			void IHierarchyObj.ReadRow()
			{
				ReadRow(DataActions.UserSort);
			}

			void IHierarchyObj.ProcessUserSort()
			{
				ProcessUserSort();
			}

			void IHierarchyObj.MarkSortInfoProcessed(RuntimeSortFilterEventInfoList runtimeSortFilterInfo)
			{
				MarkSortInfoProcessed(runtimeSortFilterInfo);
			}

			void IHierarchyObj.AddSortInfoIndex(int sortInfoIndex, RuntimeSortFilterEventInfo sortInfo)
			{
				AddSortInfoIndex(sortInfoIndex, sortInfo);
			}

			private void ConstructorHelper(int exprIndex, RuntimeHierarchyObj hierarchyRoot)
			{
				m_hierarchyRoot = hierarchyRoot;
				RuntimeGroupRootObj runtimeGroupRootObj = null;
				RuntimeDetailObj runtimeDetailObj = null;
				ExpressionInfoList expressionInfoList;
				IndexedExprHost expressionsHost;
				BoolList directions;
				if (m_hierarchyRoot is RuntimeGroupRootObj)
				{
					runtimeGroupRootObj = (RuntimeGroupRootObj)m_hierarchyRoot;
					if (ProcessingStages.Grouping == runtimeGroupRootObj.ProcessingStage)
					{
						expressionInfoList = runtimeGroupRootObj.GroupExpressions;
						expressionsHost = runtimeGroupRootObj.GroupExpressionHost;
						directions = runtimeGroupRootObj.GroupDirections;
					}
					else
					{
						Global.Tracer.Assert(ProcessingStages.SortAndFilter == runtimeGroupRootObj.ProcessingStage, "(ProcessingStages.SortAndFilter == groupRoot.ProcessingStage)");
						expressionInfoList = runtimeGroupRootObj.SortExpressions;
						expressionsHost = runtimeGroupRootObj.SortExpressionHost;
						directions = runtimeGroupRootObj.SortDirections;
					}
				}
				else
				{
					Global.Tracer.Assert(m_hierarchyRoot is RuntimeDetailObj, "(m_hierarchyRoot is RuntimeDetailObj)");
					runtimeDetailObj = (RuntimeDetailObj)m_hierarchyRoot;
					expressionInfoList = runtimeDetailObj.SortExpressions;
					expressionsHost = runtimeDetailObj.SortExpressionHost;
					directions = runtimeDetailObj.SortDirections;
				}
				if (exprIndex >= expressionInfoList.Count)
				{
					m_hierarchyObjs = new RuntimeHierarchyObjList();
					RuntimeHierarchyObj runtimeHierarchyObj = null;
					if (runtimeGroupRootObj != null)
					{
						if (ProcessingStages.Grouping == runtimeGroupRootObj.ProcessingStage)
						{
							if (m_hierarchyRoot is RuntimeListGroupRootObj)
							{
								runtimeHierarchyObj = new RuntimeListGroupLeafObj((RuntimeListGroupRootObj)m_hierarchyRoot);
							}
							else if (m_hierarchyRoot is RuntimeTableGroupRootObj)
							{
								runtimeHierarchyObj = new RuntimeTableGroupLeafObj((RuntimeTableGroupRootObj)m_hierarchyRoot);
							}
							else if (m_hierarchyRoot is RuntimeMatrixGroupRootObj)
							{
								runtimeHierarchyObj = new RuntimeMatrixGroupLeafObj((RuntimeMatrixGroupRootObj)m_hierarchyRoot);
							}
							else if (m_hierarchyRoot is RuntimeChartGroupRootObj)
							{
								runtimeHierarchyObj = new RuntimeChartGroupLeafObj((RuntimeChartGroupRootObj)m_hierarchyRoot);
							}
							else if (m_hierarchyRoot is RuntimeCustomReportItemGroupRootObj)
							{
								runtimeHierarchyObj = new RuntimeCustomReportItemGroupLeafObj((RuntimeCustomReportItemGroupRootObj)m_hierarchyRoot);
							}
							if (!runtimeGroupRootObj.HasParent)
							{
								runtimeGroupRootObj.AddChildWithNoParent((RuntimeGroupLeafObj)runtimeHierarchyObj);
							}
						}
					}
					else if (runtimeDetailObj is RuntimeListDetailObj)
					{
						runtimeHierarchyObj = new RuntimeListDetailObj((RuntimeListDetailObj)runtimeDetailObj);
					}
					else if (runtimeDetailObj is RuntimeTableDetailObj)
					{
						runtimeHierarchyObj = new RuntimeTableDetailObj((RuntimeTableDetailObj)runtimeDetailObj);
					}
					else if (runtimeDetailObj is RuntimeOWCChartDetailObj)
					{
						runtimeHierarchyObj = new RuntimeOWCChartDetailObj((RuntimeOWCChartDetailObj)runtimeDetailObj);
					}
					if (runtimeHierarchyObj != null)
					{
						m_hierarchyObjs.Add(runtimeHierarchyObj);
					}
				}
				else
				{
					m_expression = new RuntimeExpressionInfo(expressionInfoList, expressionsHost, directions, exprIndex);
					if (runtimeGroupRootObj != null)
					{
						m_grouping = new RuntimeGroupingObj(this, runtimeGroupRootObj.GroupingType);
						return;
					}
					Global.Tracer.Assert(runtimeDetailObj != null, "(null != detailRoot)");
					m_grouping = new RuntimeGroupingObj(this, RuntimeGroupingObj.GroupingTypes.Sort);
				}
			}

			internal ProcessingMessageList RegisterComparisonError(string propertyName)
			{
				return RegisterComparisonError(propertyName, null);
			}

			internal ProcessingMessageList RegisterComparisonError(string propertyName, ReportProcessingException_ComparisonError e)
			{
				ObjectType objectType;
				string name;
				if (m_hierarchyRoot is RuntimeGroupRootObj)
				{
					RuntimeGroupRootObj obj = (RuntimeGroupRootObj)m_hierarchyRoot;
					objectType = obj.HierarchyDef.DataRegionDef.ObjectType;
					name = obj.HierarchyDef.DataRegionDef.Name;
				}
				else
				{
					Global.Tracer.Assert(m_hierarchyRoot is RuntimeDetailObj, "(m_hierarchyRoot is RuntimeDetailObj)");
					RuntimeDetailObj obj2 = (RuntimeDetailObj)m_hierarchyRoot;
					objectType = obj2.DataRegionDef.ObjectType;
					name = obj2.DataRegionDef.Name;
				}
				if (e == null)
				{
					m_processingContext.ErrorContext.Register(ProcessingErrorCode.rsComparisonError, Severity.Error, objectType, name, propertyName);
				}
				else
				{
					m_processingContext.ErrorContext.Register(ProcessingErrorCode.rsComparisonTypeError, Severity.Error, objectType, name, propertyName, e.TypeX, e.TypeY);
				}
				return m_processingContext.ErrorContext.Messages;
			}

			internal ProcessingMessageList RegisterSpatialTypeComparisonError(string type)
			{
				ObjectType objectType;
				string name;
				if (m_hierarchyRoot is RuntimeGroupRootObj)
				{
					RuntimeGroupRootObj obj = (RuntimeGroupRootObj)m_hierarchyRoot;
					objectType = obj.HierarchyDef.DataRegionDef.ObjectType;
					name = obj.HierarchyDef.DataRegionDef.Name;
				}
				else
				{
					Global.Tracer.Assert(m_hierarchyRoot is RuntimeDetailObj, "(m_hierarchyRoot is RuntimeDetailObj)");
					RuntimeDetailObj obj2 = (RuntimeDetailObj)m_hierarchyRoot;
					objectType = obj2.DataRegionDef.ObjectType;
					name = obj2.DataRegionDef.Name;
				}
				m_processingContext.ErrorContext.Register(ProcessingErrorCode.rsCannotCompareSpatialType, Severity.Error, objectType, name, type);
				return m_processingContext.ErrorContext.Messages;
			}

			internal override void NextRow()
			{
				bool flag = true;
				RuntimeGroupRootObj runtimeGroupRootObj = null;
				if (m_hierarchyRoot is RuntimeGroupRootObj)
				{
					runtimeGroupRootObj = (RuntimeGroupRootObj)m_hierarchyRoot;
					if (ProcessingStages.SortAndFilter == runtimeGroupRootObj.ProcessingStage)
					{
						flag = false;
					}
				}
				if (m_hierarchyObjs != null)
				{
					if (flag)
					{
						Global.Tracer.Assert(m_hierarchyObjs[0] != null, "(null != m_hierarchyObjs[0])");
						m_hierarchyObjs[0].NextRow();
					}
					else if (runtimeGroupRootObj != null)
					{
						Global.Tracer.Assert(runtimeGroupRootObj.LastChild != null, "(null != groupRoot.LastChild)");
						m_hierarchyObjs.Add(runtimeGroupRootObj.LastChild);
					}
				}
				else
				{
					if (m_grouping == null)
					{
						return;
					}
					ObjectType objectType;
					string name;
					string propertyName;
					if (runtimeGroupRootObj != null)
					{
						objectType = runtimeGroupRootObj.HierarchyDef.DataRegionDef.ObjectType;
						name = runtimeGroupRootObj.HierarchyDef.DataRegionDef.Name;
						propertyName = "GroupExpression";
					}
					else
					{
						Global.Tracer.Assert(m_hierarchyRoot is RuntimeDetailObj, "(m_hierarchyRoot is RuntimeDetailObj)");
						RuntimeDetailObj obj = (RuntimeDetailObj)m_hierarchyRoot;
						objectType = obj.DataRegionDef.ObjectType;
						name = obj.DataRegionDef.Name;
						propertyName = "SortExpression";
					}
					object obj2 = m_processingContext.ReportRuntime.EvaluateRuntimeExpression(m_expression, objectType, name, propertyName);
					if (runtimeGroupRootObj != null && flag)
					{
						Grouping grouping = runtimeGroupRootObj.HierarchyDef.Grouping;
						if (runtimeGroupRootObj.SaveGroupExprValues)
						{
							grouping.CurrentGroupExpressionValues.Add(obj2);
						}
						MatchSortFilterScope(runtimeGroupRootObj, grouping, obj2, m_expression.ExpressionIndex);
					}
					m_grouping.NextRow(obj2);
				}
			}

			internal override bool SortAndFilter()
			{
				if (m_grouping != null)
				{
					m_grouping.Traverse(ProcessingStages.SortAndFilter, ascending: true);
				}
				if (m_hierarchyObjs != null)
				{
					for (int i = 0; i < m_hierarchyObjs.Count; i++)
					{
						m_hierarchyObjs[i].SortAndFilter();
					}
				}
				return true;
			}

			internal virtual void CalculateRunningValues()
			{
				if (m_grouping != null)
				{
					m_grouping.Traverse(ProcessingStages.RunningValues, m_expression.Direction);
				}
				if (m_hierarchyObjs == null)
				{
					return;
				}
				bool flag = true;
				for (int i = 0; i < m_hierarchyObjs.Count; i++)
				{
					RuntimeHierarchyObj runtimeHierarchyObj = m_hierarchyObjs[i];
					if (!flag || runtimeHierarchyObj is RuntimeGroupLeafObj)
					{
						((RuntimeGroupLeafObj)runtimeHierarchyObj).TraverseAllLeafNodes(ProcessingStages.RunningValues);
						flag = false;
					}
					else
					{
						((RuntimeDetailObj)runtimeHierarchyObj).ReadRows(DataActions.PostSortAggregates);
					}
				}
			}

			internal override void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
				Global.Tracer.Assert(condition: false);
			}

			internal override void CreateInstances(ReportItemInstance riInstance, IList instanceList, RenderingPagesRangesList pagesList)
			{
				Global.Tracer.Assert(condition: false);
			}

			internal void CreateInstances()
			{
				if (m_grouping != null)
				{
					m_grouping.Traverse(ProcessingStages.CreatingInstances, m_expression.Direction);
				}
				if (m_hierarchyObjs == null)
				{
					return;
				}
				bool flag = true;
				for (int i = 0; i < m_hierarchyObjs.Count; i++)
				{
					RuntimeHierarchyObj runtimeHierarchyObj = m_hierarchyObjs[i];
					if (!flag || runtimeHierarchyObj is RuntimeGroupLeafObj)
					{
						((RuntimeGroupLeafObj)runtimeHierarchyObj).TraverseAllLeafNodes(ProcessingStages.CreatingInstances);
						flag = false;
					}
					else
					{
						((RuntimeDetailObj)runtimeHierarchyObj).CreateInstance();
					}
				}
			}

			internal virtual void CreateInstance()
			{
				Global.Tracer.Assert(condition: false);
			}

			internal override void SetupEnvironment()
			{
			}

			internal override void ReadRow(DataActions dataAction)
			{
				Global.Tracer.Assert(condition: false);
			}

			internal override bool InScope(string scope)
			{
				Global.Tracer.Assert(condition: false);
				return false;
			}

			protected void MatchSortFilterScope(IScope outerScope, Grouping groupDef, object groupExprValue, int groupExprIndex)
			{
				if (m_processingContext.RuntimeSortFilterInfo == null || groupDef.SortFilterScopeInfo == null)
				{
					return;
				}
				RuntimeSortFilterEventInfoList runtimeSortFilterInfo = m_processingContext.RuntimeSortFilterInfo;
				if (groupDef.SortFilterScopeMatched == null)
				{
					groupDef.SortFilterScopeMatched = new bool[runtimeSortFilterInfo.Count];
				}
				for (int i = 0; i < runtimeSortFilterInfo.Count; i++)
				{
					_ = runtimeSortFilterInfo[i];
					VariantList variantList = groupDef.SortFilterScopeInfo[i];
					if (variantList != null && outerScope.TargetScopeMatched(i, detailSort: false))
					{
						if (CompareTo(variantList[groupExprIndex], groupExprValue, m_processingContext.CompareInfo, m_processingContext.ClrCompareOptions) == 0)
						{
							groupDef.SortFilterScopeMatched[i] = true;
						}
						else
						{
							groupDef.SortFilterScopeMatched[i] = false;
						}
					}
					else
					{
						groupDef.SortFilterScopeMatched[i] = false;
					}
				}
			}

			protected virtual IHierarchyObj CreateHierarchyObj()
			{
				return new RuntimeHierarchyObj(this);
			}

			protected virtual void ProcessUserSort()
			{
				Global.Tracer.Assert(condition: false);
			}

			protected virtual void MarkSortInfoProcessed(RuntimeSortFilterEventInfoList runtimeSortFilterInfo)
			{
				Global.Tracer.Assert(condition: false);
			}

			protected virtual void AddSortInfoIndex(int sortInfoIndex, RuntimeSortFilterEventInfo sortInfo)
			{
				Global.Tracer.Assert(condition: false);
			}
		}

		internal abstract class RuntimeGroupObj : RuntimeHierarchyObj
		{
			protected RuntimeGroupLeafObj m_lastChild;

			protected RuntimeGroupLeafObj m_firstChild;

			internal RuntimeGroupLeafObj LastChild
			{
				get
				{
					return m_lastChild;
				}
				set
				{
					m_lastChild = value;
				}
			}

			internal RuntimeGroupLeafObj FirstChild
			{
				get
				{
					return m_firstChild;
				}
				set
				{
					m_firstChild = value;
				}
			}

			internal virtual int RecursiveLevel => -1;

			protected RuntimeGroupObj(ProcessingContext processingContext)
				: base(processingContext)
			{
			}

			internal void AddChild(RuntimeGroupLeafObj child)
			{
				if (m_lastChild != null)
				{
					m_lastChild.NextLeaf = child;
				}
				else
				{
					m_firstChild = child;
				}
				child.PrevLeaf = m_lastChild;
				child.NextLeaf = null;
				child.Parent = this;
				m_lastChild = child;
			}

			internal void InsertToSortTree(RuntimeGroupLeafObj groupLeaf)
			{
				RuntimeGroupRootObj runtimeGroupRootObj = (RuntimeGroupRootObj)m_hierarchyRoot;
				Grouping grouping = runtimeGroupRootObj.HierarchyDef.Grouping;
				if (!runtimeGroupRootObj.BuiltinSortOverridden && (ProcessingContext.SecondPassOperations.Sorting & m_processingContext.SecondPassOperation) != 0 && runtimeGroupRootObj.HierarchyDef.Sorting != null)
				{
					Global.Tracer.Assert(m_grouping != null, "(m_grouping != null)");
					runtimeGroupRootObj.LastChild = groupLeaf;
					object keyValue = m_processingContext.ReportRuntime.EvaluateRuntimeExpression(m_expression, ObjectType.Grouping, grouping.Name, "Sort");
					m_grouping.NextRow(keyValue);
				}
				else
				{
					Global.Tracer.Assert(grouping.Filters != null || grouping.HasInnerFilters, "(null != groupingDef.Filters || groupingDef.HasInnerFilters)");
					AddChild(groupLeaf);
				}
			}
		}

		internal abstract class RuntimeGroupRootObj : RuntimeGroupObj, IFilterOwner
		{
			protected ReportHierarchyNode m_hierarchyDef;

			protected IScope m_outerScope;

			protected ProcessingStages m_processingStage = ProcessingStages.Grouping;

			protected AggregatesImpl m_scopedRunningValues;

			protected DataAggregateObjList m_runningValuesInGroup;

			protected AggregatesImpl m_globalRunningValueCollection;

			protected RuntimeGroupRootObjList m_groupCollection;

			protected DataActions m_dataAction;

			protected DataActions m_outerDataAction;

			protected ReportItemInstance m_reportItemInstance;

			protected IList m_instanceList;

			protected RuntimeGroupingObj.GroupingTypes m_groupingType;

			protected Filters m_groupFilters;

			protected RuntimeExpressionInfo m_parentExpression;

			protected RenderingPagesRangesList m_pagesList;

			protected bool m_saveGroupExprValues;

			protected int[] m_sortFilterExpressionScopeInfoIndices;

			private bool[] m_builtinSortOverridden;

			internal ReportHierarchyNode HierarchyDef => m_hierarchyDef;

			internal ExpressionInfoList GroupExpressions => m_hierarchyDef.Grouping.GroupExpressions;

			internal GroupingExprHost GroupExpressionHost => m_hierarchyDef.Grouping.ExprHost;

			internal ExpressionInfoList SortExpressions => m_hierarchyDef.Sorting.SortExpressions;

			internal SortingExprHost SortExpressionHost => m_hierarchyDef.Sorting.ExprHost;

			internal BoolList GroupDirections => m_hierarchyDef.Grouping.SortDirections;

			internal BoolList SortDirections => m_hierarchyDef.Sorting.SortDirections;

			internal RuntimeExpressionInfo Expression => m_expression;

			internal AggregatesImpl ScopedRunningValues => m_scopedRunningValues;

			internal AggregatesImpl GlobalRunningValueCollection => m_globalRunningValueCollection;

			internal RuntimeGroupRootObjList GroupCollection => m_groupCollection;

			internal DataActions DataAction => m_dataAction;

			internal ProcessingStages ProcessingStage
			{
				get
				{
					return m_processingStage;
				}
				set
				{
					m_processingStage = value;
				}
			}

			internal ReportItemInstance ReportItemInstance => m_reportItemInstance;

			internal IList InstanceList => m_instanceList;

			internal RenderingPagesRangesList PagesList => m_pagesList;

			internal RuntimeGroupingObj.GroupingTypes GroupingType => m_groupingType;

			internal Filters GroupFilters => m_groupFilters;

			internal bool HasParent => m_parentExpression != null;

			protected override IScope OuterScope => m_outerScope;

			internal bool SaveGroupExprValues => m_saveGroupExprValues;

			protected override int[] SortFilterExpressionScopeInfoIndices
			{
				get
				{
					if (m_sortFilterExpressionScopeInfoIndices == null)
					{
						m_sortFilterExpressionScopeInfoIndices = new int[m_processingContext.RuntimeSortFilterInfo.Count];
						for (int i = 0; i < m_processingContext.RuntimeSortFilterInfo.Count; i++)
						{
							m_sortFilterExpressionScopeInfoIndices[i] = -1;
						}
					}
					return m_sortFilterExpressionScopeInfoIndices;
				}
			}

			internal bool BuiltinSortOverridden
			{
				get
				{
					if (m_processingContext.RuntimeSortFilterInfo != null && m_builtinSortOverridden != null)
					{
						for (int i = 0; i < m_processingContext.RuntimeSortFilterInfo.Count; i++)
						{
							if (m_processingContext.UserSortFilterContext.InProcessUserSortPhase(i) && m_builtinSortOverridden[i])
							{
								return true;
							}
						}
					}
					return false;
				}
			}

			protected RuntimeGroupRootObj(IScope outerScope, ReportHierarchyNode hierarchyDef, DataActions dataAction, ProcessingContext processingContext)
				: base(processingContext)
			{
				m_hierarchyRoot = this;
				m_outerScope = outerScope;
				m_hierarchyDef = hierarchyDef;
				Grouping grouping = hierarchyDef.Grouping;
				Global.Tracer.Assert(grouping != null, "(null != groupDef)");
				m_expression = new RuntimeExpressionInfo(grouping.GroupExpressions, grouping.ExprHost, grouping.SortDirections, 0);
				if (m_processingContext.RuntimeSortFilterInfo != null && grouping.IsSortFilterExpressionScope != null)
				{
					int count = m_processingContext.RuntimeSortFilterInfo.Count;
					for (int i = 0; i < count; i++)
					{
						RuntimeSortFilterEventInfo runtimeSortFilterEventInfo = m_processingContext.RuntimeSortFilterInfo[i];
						if ((runtimeSortFilterEventInfo.EventSource.ContainingScopes == null || runtimeSortFilterEventInfo.EventSource.ContainingScopes.Count == 0 || runtimeSortFilterEventInfo.EventSourceScope != null) && grouping.IsSortFilterExpressionScope[i] && m_processingContext.UserSortFilterContext.InProcessUserSortPhase(i) && TargetScopeMatched(i, detailSort: false))
						{
							if (m_builtinSortOverridden == null)
							{
								m_builtinSortOverridden = new bool[count];
							}
							m_builtinSortOverridden[i] = true;
						}
					}
				}
				if (grouping.GroupAndSort && !BuiltinSortOverridden)
				{
					m_groupingType = RuntimeGroupingObj.GroupingTypes.Sort;
				}
				else
				{
					m_groupingType = RuntimeGroupingObj.GroupingTypes.Hash;
				}
				m_grouping = new RuntimeGroupingObj(this, m_groupingType);
				if (grouping.Filters == null)
				{
					m_dataAction = dataAction;
					m_outerDataAction = dataAction;
				}
				if (grouping.RecursiveAggregates != null)
				{
					m_dataAction |= DataActions.RecursiveAggregates;
				}
				if (grouping.PostSortAggregates != null)
				{
					m_dataAction |= DataActions.PostSortAggregates;
				}
				if (grouping.Parent != null)
				{
					m_parentExpression = new RuntimeExpressionInfo(grouping.Parent, grouping.ParentExprHost, null, 0);
				}
				m_saveGroupExprValues = grouping.SaveGroupExprValues;
				if (m_hierarchyDef.Grouping.NeedScopeInfoForSortFilterExpression == null || m_processingContext.RuntimeSortFilterInfo == null)
				{
					return;
				}
				int num = 0;
				while (true)
				{
					if (num < m_processingContext.RuntimeSortFilterInfo.Count)
					{
						if (m_hierarchyDef.Grouping.NeedScopeInfoForSortFilterExpression[num] && m_outerScope.TargetScopeMatched(num, detailSort: false))
						{
							break;
						}
						num++;
						continue;
					}
					return;
				}
				m_saveGroupExprValues = true;
			}

			internal override void GetScopeValues(IHierarchyObj targetScopeObj, VariantList[] scopeValues, ref int index)
			{
				if (this != targetScopeObj)
				{
					m_outerScope.GetScopeValues(targetScopeObj, scopeValues, ref index);
				}
			}

			internal override bool TargetScopeMatched(int index, bool detailSort)
			{
				return m_outerScope.TargetScopeMatched(index, detailSort);
			}

			internal override void NextRow()
			{
				if (ProcessThisRow())
				{
					Global.Tracer.Assert(m_grouping != null, "(null != m_grouping)");
					object obj = EvaluateGroupExpression(m_expression, "Group");
					Grouping grouping = m_hierarchyDef.Grouping;
					if (m_saveGroupExprValues)
					{
						grouping.CurrentGroupExpressionValues = new VariantList();
						grouping.CurrentGroupExpressionValues.Add(obj);
					}
					MatchSortFilterScope(m_outerScope, grouping, obj, 0);
					object parentKey = null;
					bool flag = m_parentExpression != null;
					if (flag)
					{
						parentKey = EvaluateGroupExpression(m_parentExpression, "Parent");
					}
					m_grouping.NextRow(obj, flag, parentKey);
				}
			}

			protected object EvaluateGroupExpression(RuntimeExpressionInfo expression, string propertyName)
			{
				Global.Tracer.Assert(m_hierarchyDef.Grouping != null, "(null != m_hierarchyDef.Grouping)");
				return m_processingContext.ReportRuntime.EvaluateRuntimeExpression(expression, ObjectType.Grouping, m_hierarchyDef.Grouping.Name, propertyName);
			}

			protected bool ProcessThisRow()
			{
				FieldsImpl fieldsImpl = m_processingContext.ReportObjectModel.FieldsImpl;
				if (fieldsImpl.IsAggregateRow && 0 > fieldsImpl.AggregationFieldCount)
				{
					return false;
				}
				int[] groupExpressionFieldIndices = m_hierarchyDef.Grouping.GetGroupExpressionFieldIndices();
				if (groupExpressionFieldIndices == null)
				{
					fieldsImpl.ValidAggregateRow = false;
				}
				else
				{
					foreach (int num in groupExpressionFieldIndices)
					{
						if (-1 > num || (0 <= num && !fieldsImpl[num].IsAggregationField))
						{
							fieldsImpl.ValidAggregateRow = false;
						}
					}
				}
				if (fieldsImpl.IsAggregateRow && !fieldsImpl.ValidAggregateRow)
				{
					return false;
				}
				return true;
			}

			internal void AddChildWithNoParent(RuntimeGroupLeafObj child)
			{
				if (RuntimeGroupingObj.GroupingTypes.Sort == m_groupingType)
				{
					child.Parent = this;
				}
				else
				{
					AddChild(child);
				}
			}

			internal override bool SortAndFilter()
			{
				RuntimeGroupingObj grouping = m_grouping;
				bool direction = m_expression.Direction;
				bool result = true;
				bool flag = !BuiltinSortOverridden && (ProcessingContext.SecondPassOperations.Sorting & m_processingContext.SecondPassOperation) != 0 && m_hierarchyDef.Sorting != null;
				bool flag2 = (ProcessingContext.SecondPassOperations.Filtering & m_processingContext.SecondPassOperation) != 0 && (m_hierarchyDef.Grouping.Filters != null || m_hierarchyDef.Grouping.HasInnerFilters);
				if (flag)
				{
					m_expression = new RuntimeExpressionInfo(m_hierarchyDef.Sorting.SortExpressions, m_hierarchyDef.Sorting.ExprHost, m_hierarchyDef.Sorting.SortDirections, 0);
					m_groupingType = RuntimeGroupingObj.GroupingTypes.Sort;
					m_grouping = new RuntimeGroupingObj(this, m_groupingType);
				}
				else if (flag2)
				{
					m_groupingType = RuntimeGroupingObj.GroupingTypes.None;
					m_grouping = new RuntimeGroupingObj(this, m_groupingType);
				}
				if (flag2)
				{
					m_groupFilters = new Filters(Filters.FilterTypes.GroupFilter, this, m_hierarchyDef.Grouping.Filters, ObjectType.Grouping, m_hierarchyDef.Grouping.Name, m_processingContext);
				}
				m_processingStage = ProcessingStages.SortAndFilter;
				m_lastChild = null;
				grouping.Traverse(ProcessingStages.SortAndFilter, direction);
				if (flag2)
				{
					m_groupFilters.FinishReadingRows();
					if (!flag && m_lastChild == null)
					{
						m_firstChild = null;
						result = false;
					}
				}
				return result;
			}

			void IFilterOwner.PostFilterNextRow()
			{
				Global.Tracer.Assert(condition: false);
			}

			internal virtual void AddScopedRunningValue(DataAggregateObj runningValueObj, bool escalate)
			{
				if (m_scopedRunningValues == null)
				{
					m_scopedRunningValues = new AggregatesImpl(m_processingContext.ReportRuntime);
				}
				if (m_scopedRunningValues.GetAggregateObj(runningValueObj.Name) == null)
				{
					m_scopedRunningValues.Add(runningValueObj);
				}
			}

			internal override void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
				SetupRunningValues(globalRVCol, groupCol);
			}

			protected void SetupRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol)
			{
				m_globalRunningValueCollection = globalRVCol;
				m_groupCollection = groupCol;
				if (m_hierarchyDef.Grouping.Name != null)
				{
					groupCol[m_hierarchyDef.Grouping.Name] = this;
				}
			}

			protected void AddRunningValues(RunningValueInfoList runningValues)
			{
				AddRunningValues(runningValues, ref m_runningValuesInGroup, m_globalRunningValueCollection, m_groupCollection);
			}

			protected void AddRunningValues(RunningValueInfoList runningValues, ref DataAggregateObjList runningValuesInGroup, AggregatesImpl globalRunningValueCollection, RuntimeGroupRootObjList groupCollection)
			{
				if (runningValues == null || 0 >= runningValues.Count)
				{
					return;
				}
				if (runningValuesInGroup == null)
				{
					runningValuesInGroup = new DataAggregateObjList();
				}
				for (int i = 0; i < runningValues.Count; i++)
				{
					RunningValueInfo runningValueInfo = runningValues[i];
					DataAggregateObj dataAggregateObj = globalRunningValueCollection.GetAggregateObj(runningValueInfo.Name);
					if (dataAggregateObj == null)
					{
						dataAggregateObj = new DataAggregateObj(runningValueInfo, m_processingContext);
						globalRunningValueCollection.Add(dataAggregateObj);
					}
					if (runningValueInfo.Scope != null)
					{
						RuntimeGroupRootObj runtimeGroupRootObj = groupCollection[runningValueInfo.Scope];
						if (runtimeGroupRootObj != null)
						{
							runtimeGroupRootObj.AddScopedRunningValue(dataAggregateObj, escalate: false);
						}
						else if (m_processingContext.PivotEscalateScope())
						{
							AddScopedRunningValue(dataAggregateObj, escalate: true);
						}
					}
					runningValuesInGroup.Add(dataAggregateObj);
				}
			}

			internal override void ReadRow(DataActions dataAction)
			{
				if (DataActions.PostSortAggregates == dataAction && m_runningValuesInGroup != null)
				{
					for (int i = 0; i < m_runningValuesInGroup.Count; i++)
					{
						m_runningValuesInGroup[i].Update();
					}
				}
				if (m_outerScope != null && (dataAction & m_outerDataAction) != 0)
				{
					m_outerScope.ReadRow(dataAction);
				}
			}

			internal override void CreateInstances(ReportItemInstance riInstance, IList instanceList, RenderingPagesRangesList pagesList)
			{
				m_reportItemInstance = riInstance;
				m_instanceList = instanceList;
				m_pagesList = pagesList;
				if (Report.ShowHideTypes.Interactive == m_processingContext.ShowHideType && m_parentExpression != null)
				{
					m_processingContext.EnterChildGroupings();
				}
				m_grouping.Traverse(ProcessingStages.CreatingInstances, m_expression.Direction);
				if (Report.ShowHideTypes.Interactive == m_processingContext.ShowHideType && m_parentExpression != null)
				{
					m_processingContext.ExitChildGroupings();
				}
			}
		}

		internal abstract class RuntimeGroupLeafObj : RuntimeGroupObj
		{
			protected DataAggregateObjList m_nonCustomAggregates;

			protected DataAggregateObjList m_customAggregates;

			protected FieldImpl[] m_firstRow;

			protected bool m_firstRowIsAggregate;

			protected RuntimeGroupLeafObj m_nextLeaf;

			protected RuntimeGroupLeafObj m_prevLeaf;

			protected DataRowList m_dataRows;

			protected RuntimeGroupObj m_parent;

			protected DataAggregateObjList m_recursiveAggregates;

			protected DataAggregateObjList m_postSortAggregates;

			protected int m_recursiveLevel;

			protected VariantList m_groupExprValues;

			protected bool[] m_targetScopeMatched;

			protected DataActions m_dataAction;

			protected RuntimeUserSortTargetInfo m_userSortTargetInfo;

			protected int[] m_sortFilterExpressionScopeInfoIndices;

			internal RuntimeGroupLeafObj NextLeaf
			{
				set
				{
					m_nextLeaf = value;
				}
			}

			internal RuntimeGroupLeafObj PrevLeaf
			{
				set
				{
					m_prevLeaf = value;
				}
			}

			internal RuntimeGroupObj Parent
			{
				get
				{
					return m_parent;
				}
				set
				{
					m_parent = value;
				}
			}

			protected override IScope OuterScope => m_hierarchyRoot;

			protected override string ScopeName => ((RuntimeGroupRootObj)m_hierarchyRoot).HierarchyDef.Grouping.Name;

			internal override int RecursiveLevel => m_recursiveLevel;

			internal Grouping GroupingDef => ((RuntimeGroupRootObj)m_hierarchyRoot).HierarchyDef.Grouping;

			protected override IHierarchyObj HierarchyRoot
			{
				get
				{
					if (ProcessingStages.UserSortFilter == ((RuntimeGroupRootObj)m_hierarchyRoot).ProcessingStage)
					{
						return this;
					}
					return m_hierarchyRoot;
				}
			}

			protected override BTreeNode SortTree
			{
				get
				{
					if (ProcessingStages.UserSortFilter == ((RuntimeGroupRootObj)m_hierarchyRoot).ProcessingStage)
					{
						if (m_userSortTargetInfo != null)
						{
							return m_userSortTargetInfo.SortTree;
						}
						return null;
					}
					return m_grouping.Tree;
				}
				set
				{
					if (ProcessingStages.UserSortFilter == ((RuntimeGroupRootObj)m_hierarchyRoot).ProcessingStage)
					{
						if (m_userSortTargetInfo != null)
						{
							m_userSortTargetInfo.SortTree = value;
						}
						else
						{
							Global.Tracer.Assert(condition: false);
						}
					}
					else
					{
						m_grouping.Tree = value;
					}
				}
			}

			protected override int ExpressionIndex
			{
				get
				{
					if (ProcessingStages.UserSortFilter == ((RuntimeGroupRootObj)m_hierarchyRoot).ProcessingStage)
					{
						return 0;
					}
					Global.Tracer.Assert(condition: false);
					return -1;
				}
			}

			protected override IntList SortFilterInfoIndices
			{
				get
				{
					if (m_userSortTargetInfo != null)
					{
						return m_userSortTargetInfo.SortFilterInfoIndices;
					}
					return null;
				}
			}

			internal override bool TargetForNonDetailSort
			{
				get
				{
					if (m_userSortTargetInfo != null && m_userSortTargetInfo.TargetForNonDetailSort)
					{
						return true;
					}
					return m_hierarchyRoot.TargetForNonDetailSort;
				}
			}

			protected override int[] SortFilterExpressionScopeInfoIndices
			{
				get
				{
					if (m_sortFilterExpressionScopeInfoIndices == null)
					{
						m_sortFilterExpressionScopeInfoIndices = new int[m_processingContext.RuntimeSortFilterInfo.Count];
						for (int i = 0; i < m_processingContext.RuntimeSortFilterInfo.Count; i++)
						{
							m_sortFilterExpressionScopeInfoIndices[i] = -1;
						}
					}
					return m_sortFilterExpressionScopeInfoIndices;
				}
			}

			protected RuntimeGroupLeafObj(RuntimeGroupRootObj groupRoot)
				: base(groupRoot.ProcessingContext)
			{
				ReportHierarchyNode hierarchyDef = groupRoot.HierarchyDef;
				m_hierarchyRoot = groupRoot;
				Grouping grouping = hierarchyDef.Grouping;
				RuntimeDataRegionObj.CreateAggregates(m_processingContext, grouping.Aggregates, ref m_nonCustomAggregates, ref m_customAggregates);
				RuntimeDataRegionObj.CreateAggregates(m_processingContext, grouping.RecursiveAggregates, ref m_recursiveAggregates);
				RuntimeDataRegionObj.CreateAggregates(m_processingContext, grouping.PostSortAggregates, ref m_postSortAggregates);
				if (groupRoot.SaveGroupExprValues)
				{
					m_groupExprValues = grouping.CurrentGroupExpressionValues;
				}
			}

			internal override bool IsTargetForSort(int index, bool detailSort)
			{
				if (m_userSortTargetInfo != null && m_userSortTargetInfo.IsTargetForSort(index, detailSort))
				{
					return true;
				}
				return m_hierarchyRoot.IsTargetForSort(index, detailSort);
			}

			protected virtual void ConstructRuntimeStructure(ref bool handleMyDataAction, out DataActions innerDataAction)
			{
				if (m_postSortAggregates != null || (m_recursiveAggregates != null && m_processingContext.SpecialRecursiveAggregates))
				{
					handleMyDataAction = true;
				}
				if (handleMyDataAction)
				{
					innerDataAction = DataActions.None;
				}
				else
				{
					innerDataAction = ((RuntimeGroupRootObj)m_hierarchyRoot).DataAction;
				}
			}

			protected bool HandleSortFilterEvent()
			{
				if (m_processingContext.RuntimeSortFilterInfo == null)
				{
					return false;
				}
				Grouping groupingDef = GroupingDef;
				int count = m_processingContext.RuntimeSortFilterInfo.Count;
				if (groupingDef.SortFilterScopeMatched != null || groupingDef.NeedScopeInfoForSortFilterExpression != null)
				{
					m_targetScopeMatched = new bool[count];
					for (int i = 0; i < count; i++)
					{
						if (groupingDef.SortFilterScopeMatched != null && -1 != groupingDef.SortFilterScopeIndex[i])
						{
							if (groupingDef.SortFilterScopeMatched[i])
							{
								m_targetScopeMatched[i] = true;
								RuntimeSortFilterEventInfo runtimeSortFilterEventInfo = m_processingContext.RuntimeSortFilterInfo[i];
								if (groupingDef.IsSortFilterTarget != null && groupingDef.IsSortFilterTarget[i] && !m_hierarchyRoot.TargetForNonDetailSort)
								{
									runtimeSortFilterEventInfo.EventTarget = this;
									if (m_userSortTargetInfo == null)
									{
										m_userSortTargetInfo = new RuntimeUserSortTargetInfo(this, i, runtimeSortFilterEventInfo);
									}
									else
									{
										m_userSortTargetInfo.AddSortInfo(this, i, runtimeSortFilterEventInfo);
									}
								}
								Global.Tracer.Assert(runtimeSortFilterEventInfo.EventSource.ContainingScopes != null, "(null != sortFilterInfo.EventSource.ContainingScopes)");
								if (groupingDef == runtimeSortFilterEventInfo.EventSource.ContainingScopes.LastEntry && !runtimeSortFilterEventInfo.EventSource.IsMatrixCellScope && !m_hierarchyRoot.TargetForNonDetailSort)
								{
									Global.Tracer.Assert(runtimeSortFilterEventInfo.EventSourceScope == null, "(null == sortFilterInfo.EventSourceScope)");
									runtimeSortFilterEventInfo.EventSourceScope = this;
								}
							}
							else
							{
								m_targetScopeMatched[i] = false;
							}
						}
						else
						{
							m_targetScopeMatched[i] = ((RuntimeGroupRootObj)m_hierarchyRoot).TargetScopeMatched(i, detailSort: false);
						}
					}
				}
				m_processingContext.RegisterSortFilterExpressionScope(m_hierarchyRoot, this, groupingDef.IsSortFilterExpressionScope);
				if (m_userSortTargetInfo != null && m_userSortTargetInfo.TargetForNonDetailSort)
				{
					return true;
				}
				return false;
			}

			internal override void GetScopeValues(IHierarchyObj targetScopeObj, VariantList[] scopeValues, ref int index)
			{
				if (this != targetScopeObj)
				{
					m_hierarchyRoot.GetScopeValues(targetScopeObj, scopeValues, ref index);
					Global.Tracer.Assert(m_groupExprValues != null, "(null != m_groupExprValues)");
					Global.Tracer.Assert(index < scopeValues.Length, "(index < scopeValues.Length)");
					scopeValues[index++] = m_groupExprValues;
				}
			}

			internal override bool TargetScopeMatched(int index, bool detailSort)
			{
				if (detailSort && GroupingDef.SortFilterScopeInfo == null)
				{
					return true;
				}
				if (m_targetScopeMatched != null)
				{
					return m_targetScopeMatched[index];
				}
				return false;
			}

			internal override void NextRow()
			{
				UpdateAggregateInfo();
				InternalNextRow();
			}

			protected void UpdateAggregateInfo()
			{
				FieldsImpl fieldsImpl = m_processingContext.ReportObjectModel.FieldsImpl;
				if (!fieldsImpl.ValidAggregateRow)
				{
					return;
				}
				int[] groupExpressionFieldIndices = GroupingDef.GetGroupExpressionFieldIndices();
				if (groupExpressionFieldIndices != null)
				{
					foreach (int num in groupExpressionFieldIndices)
					{
						if (num >= 0)
						{
							FieldImpl fieldImpl = fieldsImpl[num];
							if (!fieldImpl.AggregationFieldChecked && fieldImpl.IsAggregationField)
							{
								fieldImpl.AggregationFieldChecked = true;
								fieldsImpl.AggregationFieldCount--;
							}
						}
					}
				}
				if (fieldsImpl.AggregationFieldCount == 0 && m_customAggregates != null)
				{
					RuntimeDataRegionObj.UpdateAggregates(m_processingContext, m_customAggregates, updateAndSetup: false);
				}
			}

			protected void InternalNextRow()
			{
				RuntimeGroupRootObj obj = (RuntimeGroupRootObj)m_hierarchyRoot;
				ProcessingStages processingStage = obj.ProcessingStage;
				obj.ProcessingStage = ProcessingStages.UserSortFilter;
				RuntimeDataRegionObj.CommonFirstRow(m_processingContext.ReportObjectModel.FieldsImpl, ref m_firstRowIsAggregate, ref m_firstRow);
				if (m_processingContext.ReportObjectModel.FieldsImpl.IsAggregateRow)
				{
					ScopeNextAggregateRow(m_userSortTargetInfo);
				}
				else
				{
					ScopeNextNonAggregateRow(m_nonCustomAggregates, m_dataRows);
				}
				obj.ProcessingStage = processingStage;
			}

			protected override void SendToInner()
			{
				((RuntimeGroupRootObj)m_hierarchyRoot).ProcessingStage = ProcessingStages.Grouping;
			}

			internal void RemoveFromParent(RuntimeGroupObj parent)
			{
				if (m_prevLeaf == null)
				{
					parent.FirstChild = m_nextLeaf;
				}
				else
				{
					m_prevLeaf.m_nextLeaf = m_nextLeaf;
				}
				if (m_nextLeaf == null)
				{
					parent.LastChild = m_prevLeaf;
				}
				else
				{
					m_nextLeaf.m_prevLeaf = m_prevLeaf;
				}
			}

			private RuntimeGroupLeafObj Traverse(ProcessingStages operation)
			{
				RuntimeGroupLeafObj nextLeaf = m_nextLeaf;
				if (((RuntimeGroupRootObj)m_hierarchyRoot).HasParent)
				{
					m_recursiveLevel = m_parent.RecursiveLevel + 1;
				}
				bool flag = IsSpecialFilteringPass(operation);
				if (flag)
				{
					m_lastChild = null;
					ProcessChildren(operation);
				}
				switch (operation)
				{
				case ProcessingStages.SortAndFilter:
					SortAndFilter();
					break;
				case ProcessingStages.RunningValues:
					CalculateRunningValues();
					break;
				case ProcessingStages.CreatingInstances:
					CreateInstance();
					break;
				}
				if (!flag)
				{
					ProcessChildren(operation);
				}
				return nextLeaf;
			}

			internal void TraverseAllLeafNodes(ProcessingStages operation)
			{
				for (RuntimeGroupLeafObj runtimeGroupLeafObj = this; runtimeGroupLeafObj != null; runtimeGroupLeafObj = runtimeGroupLeafObj.Traverse(operation))
				{
				}
			}

			private void ProcessChildren(ProcessingStages operation)
			{
				RuntimeGroupRootObj runtimeGroupRootObj = (RuntimeGroupRootObj)m_hierarchyRoot;
				if (m_firstChild != null || m_grouping != null)
				{
					if (ProcessingStages.CreatingInstances == operation && Report.ShowHideTypes.Interactive == m_processingContext.ShowHideType)
					{
						m_processingContext.EnterChildGroupings();
					}
					if (m_firstChild != null)
					{
						m_firstChild.TraverseAllLeafNodes(operation);
						if (operation == ProcessingStages.SortAndFilter)
						{
							if ((ProcessingContext.SecondPassOperations.Filtering & m_processingContext.SecondPassOperation) != 0 && runtimeGroupRootObj.HierarchyDef.Grouping.Filters != null)
							{
								if (m_lastChild == null)
								{
									m_firstChild = null;
								}
							}
							else if (m_grouping != null)
							{
								m_firstChild = null;
							}
						}
					}
					else if (m_grouping != null)
					{
						m_grouping.Traverse(operation, runtimeGroupRootObj.Expression.Direction);
					}
					if (ProcessingStages.CreatingInstances == operation && Report.ShowHideTypes.Interactive == m_processingContext.ShowHideType)
					{
						m_processingContext.ExitChildGroupings();
					}
				}
				if (ProcessingStages.CreatingInstances == operation)
				{
					AddToDocumentMap();
				}
			}

			private bool IsSpecialFilteringPass(ProcessingStages operation)
			{
				if (ProcessingStages.SortAndFilter == operation && m_processingContext.SpecialRecursiveAggregates && (ProcessingContext.SecondPassOperations.Filtering & m_processingContext.SecondPassOperation) != 0)
				{
					return true;
				}
				return false;
			}

			internal override bool SortAndFilter()
			{
				bool flag = true;
				bool specialFilter = false;
				RuntimeGroupRootObj runtimeGroupRootObj = (RuntimeGroupRootObj)m_hierarchyRoot;
				Global.Tracer.Assert(runtimeGroupRootObj != null, "(null != groupRoot)");
				if (!runtimeGroupRootObj.BuiltinSortOverridden && (ProcessingContext.SecondPassOperations.Sorting & m_processingContext.SecondPassOperation) != 0 && runtimeGroupRootObj.HierarchyDef.Sorting != null && m_firstChild != null)
				{
					m_expression = runtimeGroupRootObj.Expression;
					m_grouping = new RuntimeGroupingObj(this, RuntimeGroupingObj.GroupingTypes.Sort);
				}
				m_lastChild = null;
				if ((!runtimeGroupRootObj.BuiltinSortOverridden && runtimeGroupRootObj.HierarchyDef.Sorting != null) || runtimeGroupRootObj.GroupFilters != null || m_recursiveAggregates != null)
				{
					if ((ProcessingContext.SecondPassOperations.Filtering & m_processingContext.SecondPassOperation) != 0)
					{
						if (m_processingContext.SpecialRecursiveAggregates && m_recursiveAggregates != null)
						{
							Global.Tracer.Assert(m_dataRows != null, "(null != m_dataRows)");
							ReadRows(sendToParent: false);
						}
						if (runtimeGroupRootObj.GroupFilters != null)
						{
							SetupEnvironment();
							flag = runtimeGroupRootObj.GroupFilters.PassFilters(this, out specialFilter);
						}
					}
					if (flag)
					{
						PostFilterNextRow();
					}
					else if (!specialFilter)
					{
						FailFilter();
					}
				}
				return flag;
			}

			internal void FailFilter()
			{
				RuntimeGroupLeafObj runtimeGroupLeafObj = null;
				bool flag = false;
				if (IsSpecialFilteringPass(ProcessingStages.SortAndFilter))
				{
					flag = true;
				}
				for (RuntimeGroupLeafObj runtimeGroupLeafObj2 = m_firstChild; runtimeGroupLeafObj2 != null; runtimeGroupLeafObj2 = runtimeGroupLeafObj)
				{
					runtimeGroupLeafObj = runtimeGroupLeafObj2.m_nextLeaf;
					runtimeGroupLeafObj2.m_parent = m_parent;
					if (flag)
					{
						m_parent.AddChild(runtimeGroupLeafObj2);
					}
				}
			}

			internal void PostFilterNextRow()
			{
				RuntimeGroupRootObj runtimeGroupRootObj = (RuntimeGroupRootObj)m_hierarchyRoot;
				if ((ProcessingContext.SecondPassOperations.Filtering & m_processingContext.SecondPassOperation) != 0 && m_dataRows != null && (m_dataAction & DataActions.RecursiveAggregates) != 0)
				{
					if (m_processingContext.SpecialRecursiveAggregates)
					{
						ReadRows(sendToParent: true);
					}
					else
					{
						ReadRows(DataActions.RecursiveAggregates);
					}
					ReleaseDataRows(DataActions.RecursiveAggregates, ref m_dataAction, ref m_dataRows);
				}
				if ((ProcessingContext.SecondPassOperations.Sorting & m_processingContext.SecondPassOperation) != 0 && !runtimeGroupRootObj.BuiltinSortOverridden && runtimeGroupRootObj.HierarchyDef.Sorting != null)
				{
					SetupEnvironment();
				}
				if ((!runtimeGroupRootObj.BuiltinSortOverridden && runtimeGroupRootObj.HierarchyDef.Sorting != null) || runtimeGroupRootObj.GroupFilters != null)
				{
					m_nextLeaf = null;
					m_parent.InsertToSortTree(this);
				}
			}

			internal override void CalculateRunningValues()
			{
				ResetScopedRunningValues();
			}

			internal override void ReadRow(DataActions dataAction)
			{
				Global.Tracer.Assert(DataActions.UserSort != dataAction, "(DataActions.UserSort != dataAction)");
				if (DataActions.PostSortAggregates == dataAction)
				{
					if (m_postSortAggregates != null)
					{
						RuntimeDataRegionObj.UpdateAggregates(m_processingContext, m_postSortAggregates, updateAndSetup: false);
					}
					Global.Tracer.Assert(m_hierarchyRoot != null, "(null != m_hierarchyRoot)");
					((IScope)m_hierarchyRoot).ReadRow(DataActions.PostSortAggregates);
				}
				else
				{
					Global.Tracer.Assert(DataActions.RecursiveAggregates == dataAction, "(DataActions.RecursiveAggregates == dataAction)");
					if (m_recursiveAggregates != null)
					{
						RuntimeDataRegionObj.UpdateAggregates(m_processingContext, m_recursiveAggregates, updateAndSetup: false);
					}
					((IScope)m_parent).ReadRow(DataActions.RecursiveAggregates);
				}
			}

			private void ReadRow(bool sendToParent)
			{
				if (!sendToParent)
				{
					Global.Tracer.Assert(m_recursiveAggregates != null, "(null != m_recursiveAggregates)");
					RuntimeDataRegionObj.UpdateAggregates(m_processingContext, m_recursiveAggregates, updateAndSetup: false);
				}
				else
				{
					((IScope)m_parent).ReadRow(DataActions.RecursiveAggregates);
				}
			}

			internal override void CreateInstances(ReportItemInstance riInstance, IList instanceList, RenderingPagesRangesList pagesList)
			{
				Global.Tracer.Assert(condition: false);
			}

			protected virtual void AddToDocumentMap()
			{
			}

			internal override void SetupEnvironment()
			{
				SetupEnvironment(m_nonCustomAggregates, m_customAggregates, m_firstRow);
				SetupAggregates(m_recursiveAggregates);
				SetupAggregates(m_postSortAggregates);
				if (((RuntimeGroupRootObj)m_hierarchyRoot).HasParent)
				{
					GroupingDef.RecursiveLevel = m_recursiveLevel;
				}
				if (((RuntimeGroupRootObj)m_hierarchyRoot).SaveGroupExprValues)
				{
					GroupingDef.CurrentGroupExpressionValues = m_groupExprValues;
				}
			}

			protected void ReadRows(DataActions action)
			{
				for (int i = 0; i < m_dataRows.Count; i++)
				{
					FieldImpl[] fields = m_dataRows[i];
					m_processingContext.ReportObjectModel.FieldsImpl.SetFields(fields);
					ReadRow(action);
				}
			}

			private void ReadRows(bool sendToParent)
			{
				for (int i = 0; i < m_dataRows.Count; i++)
				{
					FieldImpl[] fields = m_dataRows[i];
					m_processingContext.ReportObjectModel.FieldsImpl.SetFields(fields);
					ReadRow(sendToParent);
				}
			}

			protected virtual void ResetScopedRunningValues()
			{
				RuntimeGroupRootObj runtimeGroupRootObj = (RuntimeGroupRootObj)m_hierarchyRoot;
				if (runtimeGroupRootObj.ScopedRunningValues == null)
				{
					return;
				}
				foreach (DataAggregateObj @object in runtimeGroupRootObj.ScopedRunningValues.Objects)
				{
					@object.Init();
				}
			}

			protected void ResetReportItemsWithHideDuplicates()
			{
				ReportItemList reportItemsWithHideDuplicates = ((RuntimeGroupRootObj)m_hierarchyRoot).HierarchyDef.Grouping.ReportItemsWithHideDuplicates;
				if (reportItemsWithHideDuplicates != null)
				{
					for (int i = 0; i < reportItemsWithHideDuplicates.Count; i++)
					{
						TextBox textBox = reportItemsWithHideDuplicates[i] as TextBox;
						Global.Tracer.Assert(textBox != null && textBox.HideDuplicates != null);
						textBox.HasOldResult = false;
					}
				}
			}

			internal override bool InScope(string scope)
			{
				Grouping grouping = ((RuntimeGroupRootObj)m_hierarchyRoot).HierarchyDef.Grouping;
				if (grouping.ScopeNames == null)
				{
					bool inPivotCell = grouping.InPivotCell;
					grouping.ScopeNames = GetScopeNames(this, scope, ref inPivotCell, out bool inScope);
					grouping.InPivotCell = inPivotCell;
					return inScope;
				}
				return grouping.ScopeNames.Contains(scope);
			}

			protected override int GetRecursiveLevel(string scope)
			{
				if (scope == null)
				{
					return m_recursiveLevel;
				}
				Grouping grouping = ((RuntimeGroupRootObj)m_hierarchyRoot).HierarchyDef.Grouping;
				if (grouping.ScopeNames == null)
				{
					bool inPivotCell = grouping.InPivotCell;
					grouping.ScopeNames = GetScopeNames(this, scope, ref inPivotCell, out int level);
					grouping.InPivotCell = inPivotCell;
					return level;
				}
				return (grouping.ScopeNames[scope] as Grouping)?.RecursiveLevel ?? (-1);
			}

			protected override void ProcessUserSort()
			{
				((RuntimeGroupRootObj)m_hierarchyRoot).ProcessingStage = ProcessingStages.UserSortFilter;
				m_processingContext.ProcessUserSortForTarget(this, ref m_dataRows, m_userSortTargetInfo.TargetForNonDetailSort);
				if (m_userSortTargetInfo.TargetForNonDetailSort)
				{
					m_dataAction &= ~DataActions.UserSort;
					m_userSortTargetInfo.ResetTargetForNonDetailSort();
					m_userSortTargetInfo.EnterProcessUserSortPhase(m_processingContext);
					bool handleMyDataAction = false;
					ConstructRuntimeStructure(ref handleMyDataAction, out DataActions innerDataAction);
					if (!handleMyDataAction)
					{
						Global.Tracer.Assert(innerDataAction == m_dataAction, "(innerDataAction == m_dataAction)");
					}
					if (m_dataAction != 0)
					{
						m_dataRows = new DataRowList();
					}
					ScopeFinishSorting(ref m_firstRow, m_userSortTargetInfo);
					m_userSortTargetInfo.LeaveProcessUserSortPhase(m_processingContext);
				}
			}

			protected override IHierarchyObj CreateHierarchyObj()
			{
				if (ProcessingStages.UserSortFilter == ((RuntimeGroupRootObj)m_hierarchyRoot).ProcessingStage)
				{
					return new RuntimeSortHierarchyObj(this);
				}
				return base.CreateHierarchyObj();
			}

			protected override void MarkSortInfoProcessed(RuntimeSortFilterEventInfoList runtimeSortFilterInfo)
			{
				if (m_userSortTargetInfo != null)
				{
					m_userSortTargetInfo.MarkSortInfoProcessed(runtimeSortFilterInfo, this);
				}
			}

			protected override void AddSortInfoIndex(int sortInfoIndex, RuntimeSortFilterEventInfo sortInfo)
			{
				if (m_userSortTargetInfo != null)
				{
					m_userSortTargetInfo.AddSortInfoIndex(sortInfoIndex, sortInfo);
				}
			}

			protected override void GetGroupNameValuePairs(Dictionary<string, object> pairs)
			{
				Grouping grouping = ((RuntimeGroupRootObj)m_hierarchyRoot).HierarchyDef.Grouping;
				if (grouping.ScopeNames == null)
				{
					bool inPivotCell = grouping.InPivotCell;
					grouping.ScopeNames = GetScopeNames(this, ref inPivotCell, pairs);
					grouping.InPivotCell = inPivotCell;
				}
				else
				{
					IEnumerator enumerator = grouping.ScopeNames.Values.GetEnumerator();
					while (enumerator.MoveNext())
					{
						RuntimeDataRegionObj.AddGroupNameValuePair(m_processingContext, enumerator.Current as Grouping, pairs);
					}
				}
			}
		}

		internal abstract class RuntimeDetailObj : RuntimeHierarchyObj
		{
			protected IScope m_outerScope;

			protected DataRegion m_dataRegionDef;

			protected DataRowList m_dataRows;

			protected DataAggregateObjResultsList m_rvValueList;

			protected DataAggregateObjList m_runningValuesInGroup;

			protected AggregatesImpl m_globalRunningValueCollection;

			protected RuntimeGroupRootObjList m_groupCollection;

			protected DataActions m_outerDataAction;

			protected ReportItemInstance m_reportItemInstance;

			protected IList m_instanceList;

			protected RenderingPagesRangesList m_pagesList;

			protected int m_numberOfChildrenOnThisPage;

			internal DataRegion DataRegionDef => m_dataRegionDef;

			internal virtual ExpressionInfoList SortExpressions => null;

			internal virtual SortingExprHost SortExpressionHost => null;

			internal virtual BoolList SortDirections => null;

			protected override IScope OuterScope => m_outerScope;

			protected override bool IsDetail => true;

			protected RuntimeDetailObj(IScope outerScope, DataRegion dataRegionDef, DataActions dataAction, ProcessingContext processingContext)
				: base(processingContext)
			{
				m_hierarchyRoot = this;
				m_outerScope = outerScope;
				m_dataRegionDef = dataRegionDef;
				m_outerDataAction = dataAction;
			}

			internal RuntimeDetailObj(RuntimeDetailObj detailRoot)
				: base(detailRoot.ProcessingContext)
			{
				m_hierarchyRoot = detailRoot;
				m_outerScope = detailRoot.m_outerScope;
				m_dataRegionDef = detailRoot.m_dataRegionDef;
			}

			protected void HandleSortFilterEvent(ref RuntimeUserSortTargetInfo userSortTargetInfo)
			{
				RuntimeSortFilterEventInfoList runtimeSortFilterInfo = m_processingContext.RuntimeSortFilterInfo;
				if (runtimeSortFilterInfo == null || m_outerScope.TargetForNonDetailSort)
				{
					return;
				}
				for (int i = 0; i < runtimeSortFilterInfo.Count; i++)
				{
					RuntimeSortFilterEventInfo runtimeSortFilterEventInfo = runtimeSortFilterInfo[i];
					if ((runtimeSortFilterEventInfo.EventSource.ContainingScopes == null || runtimeSortFilterEventInfo.EventSource.ContainingScopes.Count == 0 || runtimeSortFilterEventInfo.EventSourceScope != null || runtimeSortFilterEventInfo.EventSource.IsSubReportTopLevelScope) && IsTargetForSort(i, detailSort: true) && runtimeSortFilterEventInfo.EventTarget != this && m_outerScope.TargetScopeMatched(i, detailSort: true))
					{
						if (userSortTargetInfo == null)
						{
							userSortTargetInfo = new RuntimeUserSortTargetInfo(this, i, runtimeSortFilterEventInfo);
						}
						else
						{
							userSortTargetInfo.AddSortInfo(this, i, runtimeSortFilterInfo[i]);
						}
					}
				}
			}

			private void HandleSortFilterEvent(int rowIndex)
			{
				DetailHandleSortFilterEvent(m_dataRegionDef, m_outerScope, rowIndex);
			}

			protected void ProcessDetailSort(RuntimeUserSortTargetInfo userSortTargetInfo)
			{
				if (userSortTargetInfo != null && !userSortTargetInfo.TargetForNonDetailSort)
				{
					object sortOrder = m_processingContext.RuntimeSortFilterInfo[userSortTargetInfo.SortFilterInfoIndices[0]].GetSortOrder(m_processingContext.ReportRuntime);
					userSortTargetInfo.SortTree.NextRow(sortOrder);
				}
			}

			internal override void NextRow()
			{
				if (m_processingContext.ReportObjectModel.FieldsImpl.IsAggregateRow)
				{
					return;
				}
				if (m_grouping != null)
				{
					object keyValue = m_processingContext.ReportRuntime.EvaluateRuntimeExpression(m_expression, m_dataRegionDef.ObjectType, m_dataRegionDef.Name, "Sort");
					m_grouping.NextRow(keyValue);
					return;
				}
				if (m_dataRows == null)
				{
					m_dataRows = new DataRowList();
				}
				HandleSortFilterEvent(m_dataRows.Count);
				SaveData(m_dataRows, m_processingContext);
			}

			internal override bool SortAndFilter()
			{
				if ((ProcessingContext.SecondPassOperations.Filtering & m_processingContext.SecondPassOperation) != 0 && m_dataRows != null && (m_outerDataAction & DataActions.RecursiveAggregates) != 0)
				{
					ReadRows(DataActions.RecursiveAggregates);
				}
				return true;
			}

			internal override void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
				if (m_grouping != null)
				{
					m_grouping.Traverse(ProcessingStages.RunningValues, m_expression.Direction);
					m_grouping = null;
				}
				else
				{
					ReadRows(DataActions.PostSortAggregates);
				}
			}

			internal void ReadRows(DataActions dataAction)
			{
				if (m_dataRows == null)
				{
					return;
				}
				RuntimeDetailObj runtimeDetailObj = (RuntimeDetailObj)m_hierarchyRoot;
				bool flag = false;
				if (this != m_hierarchyRoot)
				{
					flag = true;
					if (runtimeDetailObj.m_dataRows == null)
					{
						runtimeDetailObj.m_dataRows = new DataRowList();
					}
					UpdateSortFilterInfo(runtimeDetailObj, runtimeDetailObj.m_dataRows.Count);
				}
				for (int i = 0; i < m_dataRows.Count; i++)
				{
					FieldImpl[] array = m_dataRows[i];
					m_processingContext.ReportObjectModel.FieldsImpl.SetFields(array);
					if (flag)
					{
						runtimeDetailObj.m_dataRows.Add(array);
					}
					runtimeDetailObj.ReadRow(dataAction);
				}
			}

			private void UpdateSortFilterInfo(RuntimeDetailObj detailRoot, int rootRowCount)
			{
				RuntimeSortFilterEventInfoList runtimeSortFilterInfo = m_processingContext.RuntimeSortFilterInfo;
				if (runtimeSortFilterInfo == null || m_dataRegionDef.SortFilterSourceDetailScopeInfo == null)
				{
					return;
				}
				for (int i = 0; i < runtimeSortFilterInfo.Count; i++)
				{
					RuntimeSortFilterEventInfo runtimeSortFilterEventInfo = runtimeSortFilterInfo[i];
					if (this == runtimeSortFilterEventInfo.EventSourceScope)
					{
						runtimeSortFilterEventInfo.EventSourceScope = detailRoot;
						runtimeSortFilterEventInfo.EventSourceDetailIndex += rootRowCount;
					}
					if (runtimeSortFilterEventInfo.DetailScopes == null)
					{
						continue;
					}
					for (int j = 0; j < runtimeSortFilterEventInfo.DetailScopes.Count; j++)
					{
						if (this == runtimeSortFilterEventInfo.DetailScopes[j])
						{
							runtimeSortFilterEventInfo.DetailScopes[j] = detailRoot;
							runtimeSortFilterEventInfo.DetailScopeIndices[j] += rootRowCount;
						}
					}
				}
			}

			internal override void ReadRow(DataActions dataAction)
			{
				if (DataActions.PostSortAggregates == dataAction && m_runningValuesInGroup != null && 0 < m_runningValuesInGroup.Count)
				{
					DataAggregateObjResult[] array = new DataAggregateObjResult[m_runningValuesInGroup.Count];
					for (int i = 0; i < m_runningValuesInGroup.Count; i++)
					{
						m_runningValuesInGroup[i].Update();
						array[i] = m_runningValuesInGroup[i].AggregateResult();
					}
					m_rvValueList.Add(array);
				}
				if (m_outerScope != null && (m_outerDataAction & dataAction) != 0)
				{
					m_outerScope.ReadRow(dataAction);
				}
			}

			internal void CreateInstances(ReportItemInstance riInstance, IList instanceList, RenderingPagesRangesList pagesList, out int numberOfChildrenOnThisPage)
			{
				m_numberOfChildrenOnThisPage = 0;
				CreateInstances(riInstance, instanceList, pagesList);
				numberOfChildrenOnThisPage = m_numberOfChildrenOnThisPage;
			}

			internal override void CreateInstances(ReportItemInstance riInstance, IList instanceList, RenderingPagesRangesList pagesList)
			{
				m_reportItemInstance = riInstance;
				m_instanceList = instanceList;
				m_pagesList = pagesList;
				if (m_grouping != null)
				{
					m_grouping.Traverse(ProcessingStages.CreatingInstances, m_expression.Direction);
				}
				else
				{
					CreateInstance();
				}
			}

			internal override void CreateInstance()
			{
				Global.Tracer.Assert(condition: false);
			}

			internal static void SaveData(DataRowList dataRows, ProcessingContext processingContext)
			{
				Global.Tracer.Assert(dataRows != null, "(null != dataRows)");
				FieldImpl[] andSaveFields = processingContext.ReportObjectModel.FieldsImpl.GetAndSaveFields();
				dataRows.Add(andSaveFields);
			}

			protected void AddRunningValues(RunningValueInfoList runningValues, RuntimeGroupRootObj lastGroup)
			{
				AddRunningValues(m_processingContext, runningValues, ref m_runningValuesInGroup, m_globalRunningValueCollection, m_groupCollection, lastGroup);
			}

			internal static void AddRunningValues(ProcessingContext processingContext, RunningValueInfoList runningValues, ref DataAggregateObjList runningValuesInGroup, AggregatesImpl globalRunningValueCollection, RuntimeGroupRootObjList groupCollection, RuntimeGroupRootObj lastGroup)
			{
				if (runningValues == null || 0 >= runningValues.Count)
				{
					return;
				}
				if (runningValuesInGroup == null)
				{
					runningValuesInGroup = new DataAggregateObjList();
				}
				for (int i = 0; i < runningValues.Count; i++)
				{
					RunningValueInfo runningValueInfo = runningValues[i];
					DataAggregateObj dataAggregateObj = globalRunningValueCollection.GetAggregateObj(runningValueInfo.Name);
					if (dataAggregateObj == null)
					{
						dataAggregateObj = new DataAggregateObj(runningValueInfo, processingContext);
						globalRunningValueCollection.Add(dataAggregateObj);
					}
					if (runningValueInfo.Scope != null)
					{
						RuntimeGroupRootObj runtimeGroupRootObj = groupCollection[runningValueInfo.Scope];
						if (runtimeGroupRootObj != null)
						{
							runtimeGroupRootObj.AddScopedRunningValue(dataAggregateObj, escalate: false);
						}
						else if (processingContext.PivotEscalateScope())
						{
							lastGroup?.AddScopedRunningValue(dataAggregateObj, escalate: true);
						}
					}
					runningValuesInGroup.Add(dataAggregateObj);
				}
			}

			protected void SetupEnvironment(int dataRowIndex, RunningValueInfoList runningValueDefs)
			{
				SetupFields(m_dataRows[dataRowIndex]);
				if (runningValueDefs != null && 0 < runningValueDefs.Count)
				{
					SetupRunningValues(runningValueDefs, m_rvValueList[dataRowIndex]);
				}
				m_processingContext.ReportRuntime.CurrentScope = this;
			}

			protected void SetupEnvironment(int dataRowIndex, ref int startIndex, RunningValueInfoList runningValueDefs, bool rvOnly)
			{
				if (!rvOnly)
				{
					SetupFields(m_dataRows[dataRowIndex]);
					m_processingContext.ReportRuntime.CurrentScope = this;
				}
				if (runningValueDefs != null && 0 < runningValueDefs.Count)
				{
					SetupRunningValues(ref startIndex, runningValueDefs, m_rvValueList[dataRowIndex]);
				}
			}

			internal override bool InScope(string scope)
			{
				Global.Tracer.Assert(m_outerScope != null, "(null != m_outerScope)");
				return m_outerScope.InScope(scope);
			}

			protected override int GetRecursiveLevel(string scope)
			{
				Global.Tracer.Assert(m_outerScope != null, "(null != m_outerScope)");
				return m_outerScope.RecursiveLevel(scope);
			}

			internal override void GetScopeValues(IHierarchyObj targetScopeObj, VariantList[] scopeValues, ref int index)
			{
				DetailGetScopeValues(m_outerScope, targetScopeObj, scopeValues, ref index);
			}

			internal override bool TargetScopeMatched(int index, bool detailSort)
			{
				return DetailTargetScopeMatched(m_dataRegionDef, m_outerScope, index);
			}

			protected override void GetGroupNameValuePairs(Dictionary<string, object> pairs)
			{
				Global.Tracer.Assert(m_outerScope != null, "(null != m_outerScope)");
				m_outerScope.GetGroupNameValuePairs(pairs);
			}
		}

		internal abstract class RuntimeOnePassDetailObj : RuntimeDataRegionObj
		{
			protected IScope m_outerScope;

			protected DataRegion m_dataRegionDef;

			protected DataAggregateObjList m_runningValues;

			protected Pagination m_pagination;

			protected int m_numberOfContentsOnThisPage;

			protected RenderingPagesRangesList m_renderingPages;

			protected NavigationInfo m_navigationInfo;

			protected PageTextboxes m_onePassTextboxes;

			protected override IScope OuterScope => m_outerScope;

			internal Pagination Pagination => m_pagination;

			internal int NumberOfContentsOnThisPage
			{
				get
				{
					return m_numberOfContentsOnThisPage;
				}
				set
				{
					m_numberOfContentsOnThisPage = value;
				}
			}

			internal RenderingPagesRangesList ChildrenStartAndEndPages => m_renderingPages;

			internal NavigationInfo NavigationInfo => m_navigationInfo;

			internal PageTextboxes OnePassTextboxes => m_onePassTextboxes;

			internal DataRegion DataRegionDef => m_dataRegionDef;

			protected RuntimeOnePassDetailObj(IScope outerScope, DataRegion dataRegionDef, ProcessingContext processingContext)
				: base(processingContext)
			{
				m_outerScope = outerScope;
				m_dataRegionDef = dataRegionDef;
				m_pagination = new Pagination(m_processingContext.Pagination.PageHeight);
				m_renderingPages = new RenderingPagesRangesList();
				m_navigationInfo = new NavigationInfo();
				m_onePassTextboxes = new PageTextboxes();
			}

			internal abstract int GetDetailPage();

			internal override void NextRow()
			{
				FieldsImpl fieldsImpl = m_processingContext.ReportObjectModel.FieldsImpl;
				if (fieldsImpl.IsAggregateRow)
				{
					return;
				}
				if (m_runningValues != null)
				{
					for (int i = 0; i < m_runningValues.Count; i++)
					{
						m_runningValues[i].Update();
					}
				}
				m_processingContext.ReportRuntime.CurrentScope = this;
				if (fieldsImpl.AddRowIndex)
				{
					DetailHandleSortFilterEvent(m_dataRegionDef, m_outerScope, fieldsImpl.GetRowIndex());
				}
				CreateInstance();
			}

			internal override bool SortAndFilter()
			{
				return true;
			}

			internal override void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
			}

			protected abstract void CreateInstance();

			internal override void CreateInstances(ReportItemInstance riInstance, IList instanceList, RenderingPagesRangesList pagesList)
			{
			}

			protected void AddRunningValues(RunningValueInfoList runningValues)
			{
				if (runningValues != null && 0 < runningValues.Count)
				{
					if (m_runningValues == null)
					{
						m_runningValues = new DataAggregateObjList();
					}
					for (int i = 0; i < runningValues.Count; i++)
					{
						DataAggregateObj dataAggregateObj = new DataAggregateObj(runningValues[i], m_processingContext);
						m_runningValues.Add(dataAggregateObj);
						m_processingContext.ReportObjectModel.AggregatesImpl.Add(dataAggregateObj);
					}
				}
			}

			internal override void SetupEnvironment()
			{
			}

			internal override void ReadRow(DataActions dataAction)
			{
			}

			internal override bool InScope(string scope)
			{
				Global.Tracer.Assert(m_outerScope != null, "(null != m_outerScope)");
				return m_outerScope.InScope(scope);
			}

			internal virtual bool IsVisible(string textboxName)
			{
				return true;
			}

			internal void MoveAllToFirstPage()
			{
				int pageCount = m_onePassTextboxes.GetPageCount();
				for (int i = 1; i < pageCount; i++)
				{
					Hashtable textboxesOnPage = m_onePassTextboxes.GetTextboxesOnPage(i);
					if (textboxesOnPage != null)
					{
						IDictionaryEnumerator enumerator = textboxesOnPage.GetEnumerator();
						while (enumerator.MoveNext())
						{
							string text = enumerator.Key as string;
							ArrayList arrayList = enumerator.Value as ArrayList;
							Global.Tracer.Assert(text != null && arrayList != null, "(null != textboxName && null != values)");
							m_onePassTextboxes.AddTextboxValue(0, text, arrayList);
						}
					}
				}
			}

			internal virtual void ProcessOnePassDetailTextboxes(int sourcePage, int targetPage)
			{
				Hashtable textboxesOnPage = m_onePassTextboxes.GetTextboxesOnPage(sourcePage);
				if (textboxesOnPage == null)
				{
					return;
				}
				IDictionaryEnumerator enumerator = textboxesOnPage.GetEnumerator();
				while (enumerator.MoveNext())
				{
					string text = enumerator.Key as string;
					ArrayList arrayList = enumerator.Value as ArrayList;
					Global.Tracer.Assert(text != null && arrayList != null, "(null != textboxName && null != values)");
					if (IsVisible(text))
					{
						m_processingContext.PageSectionContext.PageTextboxes.AddTextboxValue(targetPage, text, arrayList);
					}
				}
			}

			internal override void GetScopeValues(IHierarchyObj targetScopeObj, VariantList[] scopeValues, ref int index)
			{
				DetailGetScopeValues(m_outerScope, targetScopeObj, scopeValues, ref index);
			}

			internal override bool TargetScopeMatched(int index, bool detailSort)
			{
				return DetailTargetScopeMatched(m_dataRegionDef, m_outerScope, index);
			}
		}

		private sealed class RuntimeListGroupRootObj : RuntimeGroupRootObj, IFilterOwner
		{
			private List m_listDef;

			private Filters m_filters;

			private DataAggregateObjList m_nonCustomAggregates;

			private DataAggregateObjList m_customAggregates;

			private FieldImpl[] m_firstRow;

			private bool m_firstRowIsAggregate;

			private DataAggregateObjList m_postSortAggregates;

			private DataRowList m_dataRows;

			private RuntimeUserSortTargetInfo m_userSortTargetInfo;

			internal ReportItemCollection ReportItemsDef => m_listDef.ReportItems;

			protected override string ScopeName => m_listDef.Name;

			protected override BTreeNode SortTree
			{
				get
				{
					if (ProcessingStages.UserSortFilter == m_processingStage)
					{
						if (m_userSortTargetInfo != null)
						{
							return m_userSortTargetInfo.SortTree;
						}
						return null;
					}
					return m_grouping.Tree;
				}
				set
				{
					if (ProcessingStages.UserSortFilter == m_processingStage)
					{
						if (m_userSortTargetInfo != null)
						{
							m_userSortTargetInfo.SortTree = value;
						}
						else
						{
							Global.Tracer.Assert(condition: false);
						}
					}
					else
					{
						m_grouping.Tree = value;
					}
				}
			}

			protected override int ExpressionIndex => 0;

			protected override IntList SortFilterInfoIndices
			{
				get
				{
					if (m_userSortTargetInfo != null)
					{
						return m_userSortTargetInfo.SortFilterInfoIndices;
					}
					return null;
				}
			}

			internal override bool TargetForNonDetailSort
			{
				get
				{
					if (m_userSortTargetInfo != null && m_userSortTargetInfo.TargetForNonDetailSort)
					{
						return true;
					}
					return m_outerScope.TargetForNonDetailSort;
				}
			}

			internal RuntimeListGroupRootObj(IScope outerScope, List listDef, ref DataActions dataAction, ProcessingContext processingContext)
				: base(outerScope, listDef.HierarchyDef, (listDef.PostSortAggregates == null && listDef.Filters == null) ? dataAction : DataActions.None, processingContext)
			{
				m_listDef = listDef;
				ReportItemCollection reportItems = listDef.ReportItems;
				RuntimeDataRegionObj.CreateAggregates(m_processingContext, listDef.Aggregates, ref m_nonCustomAggregates, ref m_customAggregates);
				if (listDef.Filters != null)
				{
					m_filters = new Filters(Filters.FilterTypes.DataRegionFilter, this, listDef.Filters, listDef.ObjectType, listDef.Name, m_processingContext);
				}
				bool flag = false;
				if (listDef.PostSortAggregates != null)
				{
					RuntimeDataRegionObj.CreateAggregates(m_processingContext, listDef.PostSortAggregates, ref m_postSortAggregates);
					flag = true;
				}
				if (reportItems != null && reportItems.RunningValues != null && 0 < reportItems.RunningValues.Count)
				{
					m_dataAction |= DataActions.PostSortAggregates;
				}
				if (listDef.Filters == null && (m_hierarchyDef.Grouping.Filters == null || listDef.PostSortAggregates != null))
				{
					dataAction = DataActions.None;
				}
				if (m_processingContext.IsSortFilterTarget(listDef.IsSortFilterTarget, m_outerScope, this, ref m_userSortTargetInfo) && m_userSortTargetInfo.TargetForNonDetailSort)
				{
					flag = true;
				}
				m_processingContext.RegisterSortFilterExpressionScope(m_outerScope, this, listDef.IsSortFilterExpressionScope);
				if (flag)
				{
					m_dataRows = new DataRowList();
				}
			}

			internal override bool IsTargetForSort(int index, bool detailSort)
			{
				if (m_userSortTargetInfo != null && m_userSortTargetInfo.IsTargetForSort(index, detailSort))
				{
					return true;
				}
				return m_outerScope.IsTargetForSort(index, detailSort);
			}

			internal override void NextRow()
			{
				if (m_processingContext.ReportObjectModel.FieldsImpl.AggregationFieldCount == 0)
				{
					RuntimeDataRegionObj.UpdateAggregates(m_processingContext, m_customAggregates, updateAndSetup: false);
				}
				if (m_processingContext.ReportObjectModel.FieldsImpl.IsAggregateRow)
				{
					ScopeNextAggregateRow(m_userSortTargetInfo);
				}
				else
				{
					NextNonAggregateRow();
				}
			}

			private void NextNonAggregateRow()
			{
				bool flag = true;
				if (m_filters != null)
				{
					flag = m_filters.PassFilters(m_processingContext.ReportObjectModel.FieldsImpl.GetFields());
				}
				if (flag)
				{
					((IFilterOwner)this).PostFilterNextRow();
				}
			}

			void IFilterOwner.PostFilterNextRow()
			{
				ProcessingStages processingStage = m_processingStage;
				m_processingStage = ProcessingStages.UserSortFilter;
				RuntimeDataRegionObj.CommonFirstRow(m_processingContext.ReportObjectModel.FieldsImpl, ref m_firstRowIsAggregate, ref m_firstRow);
				ScopeNextNonAggregateRow(m_nonCustomAggregates, m_dataRows);
				m_processingStage = processingStage;
			}

			protected override void SendToInner()
			{
				m_processingStage = ProcessingStages.Grouping;
				base.NextRow();
			}

			internal override bool SortAndFilter()
			{
				SetupEnvironment();
				if (m_userSortTargetInfo != null)
				{
					m_userSortTargetInfo.EnterProcessUserSortPhase(m_processingContext);
				}
				if ((ProcessingContext.SecondPassOperations.Filtering & m_processingContext.SecondPassOperation) != 0 && m_dataRows != null && (m_outerDataAction & DataActions.RecursiveAggregates) != 0)
				{
					ReadRows(DataActions.RecursiveAggregates);
				}
				bool result = base.SortAndFilter();
				if (m_userSortTargetInfo != null)
				{
					m_userSortTargetInfo.LeaveProcessUserSortPhase(m_processingContext);
				}
				return result;
			}

			private void ReadRows(DataActions dataAction)
			{
				for (int i = 0; i < m_dataRows.Count; i++)
				{
					FieldImpl[] fields = m_dataRows[i];
					m_processingContext.ReportObjectModel.FieldsImpl.SetFields(fields);
					if (DataActions.PostSortAggregates == dataAction)
					{
						RuntimeDataRegionObj.UpdateAggregates(m_processingContext, m_postSortAggregates, updateAndSetup: false);
					}
					if (m_outerScope != null && (dataAction & m_outerDataAction) != 0)
					{
						m_outerScope.ReadRow(dataAction);
					}
				}
			}

			internal override void ReadRow(DataActions dataAction)
			{
				if (DataActions.UserSort == dataAction)
				{
					RuntimeDataRegionObj.CommonFirstRow(m_processingContext.ReportObjectModel.FieldsImpl, ref m_firstRowIsAggregate, ref m_firstRow);
					CommonNextRow(m_dataRows);
				}
				else if (m_postSortAggregates == null)
				{
					base.ReadRow(dataAction);
				}
				else if (DataActions.PostSortAggregates == dataAction && m_runningValuesInGroup != null)
				{
					for (int i = 0; i < m_runningValuesInGroup.Count; i++)
					{
						m_runningValuesInGroup[i].Update();
					}
				}
			}

			internal override void SetupEnvironment()
			{
				SetupEnvironment(m_nonCustomAggregates, m_customAggregates, m_firstRow);
				SetupAggregates(m_postSortAggregates);
			}

			internal override void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
				base.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
				if (m_listDef.ReportItems != null)
				{
					AddRunningValues(m_listDef.ReportItems.RunningValues);
				}
				if (m_dataRows != null)
				{
					ReadRows(DataActions.PostSortAggregates);
					m_dataRows = null;
				}
				m_grouping.Traverse(ProcessingStages.RunningValues, m_expression.Direction);
			}

			internal override bool InScope(string scope)
			{
				return DataRegionInScope(m_listDef, scope);
			}

			protected override int GetRecursiveLevel(string scope)
			{
				return DataRegionRecursiveLevel(m_listDef, scope);
			}

			internal override bool TargetScopeMatched(int index, bool detailSort)
			{
				return m_outerScope.TargetScopeMatched(index, detailSort);
			}

			protected override IHierarchyObj CreateHierarchyObj()
			{
				if (ProcessingStages.UserSortFilter == m_processingStage)
				{
					return new RuntimeSortHierarchyObj(this);
				}
				return base.CreateHierarchyObj();
			}

			protected override void ProcessUserSort()
			{
				m_processingStage = ProcessingStages.UserSortFilter;
				m_processingContext.ProcessUserSortForTarget(this, ref m_dataRows, m_userSortTargetInfo.TargetForNonDetailSort);
				if (m_userSortTargetInfo.TargetForNonDetailSort)
				{
					m_userSortTargetInfo.ResetTargetForNonDetailSort();
					m_userSortTargetInfo.EnterProcessUserSortPhase(m_processingContext);
					m_grouping = new RuntimeGroupingObj(this, m_groupingType);
					m_firstChild = (m_lastChild = null);
					if (m_listDef.PostSortAggregates != null)
					{
						m_dataRows = new DataRowList();
					}
					ScopeFinishSorting(ref m_firstRow, m_userSortTargetInfo);
					m_userSortTargetInfo.LeaveProcessUserSortPhase(m_processingContext);
				}
			}

			protected override void MarkSortInfoProcessed(RuntimeSortFilterEventInfoList runtimeSortFilterInfo)
			{
				if (m_userSortTargetInfo != null)
				{
					m_userSortTargetInfo.MarkSortInfoProcessed(runtimeSortFilterInfo, this);
				}
			}

			protected override void AddSortInfoIndex(int sortInfoIndex, RuntimeSortFilterEventInfo sortInfo)
			{
				if (m_userSortTargetInfo != null)
				{
					m_userSortTargetInfo.AddSortInfoIndex(sortInfoIndex, sortInfo);
				}
			}

			protected override void GetGroupNameValuePairs(Dictionary<string, object> pairs)
			{
				DataRegionGetGroupNameValuePairs(m_listDef, pairs);
			}
		}

		private sealed class RuntimeListGroupLeafObj : RuntimeGroupLeafObj
		{
			private RuntimeRICollection m_reportItemCol;

			private ListContentInstance m_listContentInstance;

			private string m_label;

			private int m_startPage = -1;

			internal int StartPage => m_startPage;

			internal RuntimeListGroupLeafObj(RuntimeListGroupRootObj groupRoot)
				: base(groupRoot)
			{
				m_dataAction = groupRoot.DataAction;
				bool handleMyDataAction = false;
				bool num = HandleSortFilterEvent();
				ConstructRuntimeStructure(ref handleMyDataAction, out DataActions innerDataAction);
				if (!handleMyDataAction)
				{
					m_dataAction = innerDataAction;
				}
				if (num)
				{
					m_dataAction |= DataActions.UserSort;
				}
				if (m_dataAction != 0)
				{
					m_dataRows = new DataRowList();
				}
			}

			protected override void ConstructRuntimeStructure(ref bool handleMyDataAction, out DataActions innerDataAction)
			{
				base.ConstructRuntimeStructure(ref handleMyDataAction, out innerDataAction);
				m_reportItemCol = new RuntimeRICollection(this, ((RuntimeListGroupRootObj)m_hierarchyRoot).ReportItemsDef, ref innerDataAction, m_processingContext, createDataRegions: true);
			}

			protected override void SendToInner()
			{
				base.SendToInner();
				m_reportItemCol.FirstPassNextDataRow();
			}

			internal override bool SortAndFilter()
			{
				SetupEnvironment();
				if (m_userSortTargetInfo != null)
				{
					m_userSortTargetInfo.EnterProcessUserSortPhase(m_processingContext);
				}
				m_reportItemCol.SortAndFilter();
				bool result = base.SortAndFilter();
				if (m_userSortTargetInfo != null)
				{
					m_userSortTargetInfo.LeaveProcessUserSortPhase(m_processingContext);
				}
				return result;
			}

			internal override void ReadRow(DataActions dataAction)
			{
				if (DataActions.UserSort == dataAction)
				{
					RuntimeDataRegionObj.CommonFirstRow(m_processingContext.ReportObjectModel.FieldsImpl, ref m_firstRowIsAggregate, ref m_firstRow);
					CommonNextRow(m_dataRows);
					return;
				}
				base.ReadRow(dataAction);
				if (DataActions.PostSortAggregates == dataAction)
				{
					CalculatePreviousAggregates();
				}
			}

			private void CalculatePreviousAggregates()
			{
				if (!m_processedPreviousAggregates && m_processingContext.GlobalRVCollection != null)
				{
					if (m_reportItemCol != null)
					{
						m_reportItemCol.CalculatePreviousAggregates(m_processingContext.GlobalRVCollection);
					}
					m_processedPreviousAggregates = true;
				}
			}

			internal override void CalculateRunningValues()
			{
				RuntimeGroupRootObj runtimeGroupRootObj = (RuntimeGroupRootObj)m_hierarchyRoot;
				AggregatesImpl globalRunningValueCollection = runtimeGroupRootObj.GlobalRunningValueCollection;
				RuntimeGroupRootObjList groupCollection = runtimeGroupRootObj.GroupCollection;
				if (m_dataRows != null)
				{
					ReadRows(DataActions.PostSortAggregates);
					m_dataRows = null;
				}
				if (m_reportItemCol != null)
				{
					m_reportItemCol.CalculateRunningValues(globalRunningValueCollection, groupCollection, runtimeGroupRootObj);
				}
				base.CalculateRunningValues();
			}

			internal override void CreateInstance()
			{
				SetupEnvironment();
				RuntimeListGroupRootObj obj = (RuntimeListGroupRootObj)m_hierarchyRoot;
				ReportItemInstance reportItemInstance = obj.ReportItemInstance;
				IList instanceList = obj.InstanceList;
				ReportHierarchyNode hierarchyDef = obj.HierarchyDef;
				List list = (List)hierarchyDef.DataRegionDef;
				Global.Tracer.Assert(m_reportItemCol != null, "(null != m_reportItemCol)");
				m_processingContext.ChunkManager.CheckPageBreak(hierarchyDef, atStart: true);
				m_processingContext.Pagination.EnterIgnorePageBreak(list.Visibility, ignoreAlways: false);
				bool flag = false;
				if (instanceList.Count != 0)
				{
					flag = m_processingContext.Pagination.CalculateSoftPageBreak(null, 0.0, 0.0, ignoreSoftPageBreak: false, list.Grouping.PageBreakAtStart);
					if (!m_processingContext.Pagination.IgnorePageBreak && (m_processingContext.Pagination.CanMoveToNextPage(list.Grouping.PageBreakAtStart) || flag))
					{
						list.ContentStartPage++;
						m_processingContext.Pagination.SetCurrentPageHeight(list, 0.0);
					}
				}
				m_listContentInstance = new ListContentInstance(m_processingContext, list);
				RenderingPagesRanges renderingPagesRanges = default(RenderingPagesRanges);
				renderingPagesRanges.StartPage = list.ContentStartPage;
				m_startPage = renderingPagesRanges.StartPage;
				if (Report.ShowHideTypes.Interactive == m_processingContext.ShowHideType)
				{
					m_processingContext.EnterGrouping();
					((IShowHideContainer)m_listContentInstance).BeginProcessContainer(m_processingContext);
				}
				if (list.Grouping.GroupLabel != null)
				{
					m_label = m_processingContext.NavigationInfo.CurrentLabel;
					if (m_label != null)
					{
						m_processingContext.NavigationInfo.EnterDocumentMapChildren();
					}
				}
				int startPage = list.StartPage;
				list.StartPage = list.ContentStartPage;
				m_processingContext.PageSectionContext.EnterVisibilityScope(list.Visibility, list.StartHidden);
				m_reportItemCol.CreateInstances(m_listContentInstance.ReportItemColInstance);
				m_processingContext.PageSectionContext.ExitVisibilityScope();
				if (Report.ShowHideTypes.Interactive == m_processingContext.ShowHideType)
				{
					((IShowHideContainer)m_listContentInstance).EndProcessContainer(m_processingContext);
				}
				m_processingContext.ChunkManager.CheckPageBreak(hierarchyDef, atStart: false);
				if (Report.ShowHideTypes.Interactive == m_processingContext.ShowHideType)
				{
					m_processingContext.ExitGrouping();
				}
				m_processingContext.Pagination.ProcessEndGroupPage(list.IsListMostInner ? list.HeightValue : 0.0, list.Grouping.PageBreakAtEnd, list, childrenOnThisPage: true, list.StartHidden);
				list.ContentStartPage = list.EndPage;
				list.StartPage = startPage;
				if (m_processingContext.Pagination.ShouldItemMoveToChildStartPage(list))
				{
					int num = m_startPage = (renderingPagesRanges.StartPage = m_listContentInstance.ReportItemColInstance.ChildrenStartAndEndPages[0].StartPage);
				}
				renderingPagesRanges.EndPage = list.EndPage;
				((ListInstance)reportItemInstance).ChildrenStartAndEndPages.Add(renderingPagesRanges);
				m_processingContext.Pagination.LeaveIgnorePageBreak(list.Visibility, ignoreAlways: false);
				instanceList.Add(m_listContentInstance);
				if (m_reportItemCol != null)
				{
					m_reportItemCol.ResetReportItemObjs();
				}
				ResetReportItemsWithHideDuplicates();
			}

			protected override void AddToDocumentMap()
			{
				if (base.GroupingDef.GroupLabel != null && m_label != null)
				{
					m_processingContext.NavigationInfo.AddToDocumentMap(m_listContentInstance.UniqueName, isContainer: true, m_startPage, m_label);
				}
			}
		}

		private sealed class RuntimeListDetailObj : RuntimeDetailObj, IFilterOwner, ISortDataHolder
		{
			private Filters m_filters;

			private DataAggregateObjList m_nonCustomAggregates;

			private DataAggregateObjList m_customAggregates;

			private DataAggregateObjList m_postSortAggregates;

			private RuntimeUserSortTargetInfo m_userSortTargetInfo;

			internal override ExpressionInfoList SortExpressions
			{
				get
				{
					Sorting sorting = ((List)m_dataRegionDef).Sorting;
					if (sorting != null && 0 < sorting.SortExpressions.Count)
					{
						return sorting.SortExpressions;
					}
					return null;
				}
			}

			internal override SortingExprHost SortExpressionHost => ((List)m_dataRegionDef).Sorting?.ExprHost;

			internal override BoolList SortDirections
			{
				get
				{
					Sorting sorting = ((List)m_dataRegionDef).Sorting;
					if (sorting != null && 0 < sorting.SortDirections.Count)
					{
						return sorting.SortDirections;
					}
					return null;
				}
			}

			protected override string ScopeName => m_dataRegionDef.Name;

			protected override BTreeNode SortTree
			{
				get
				{
					if (m_userSortTargetInfo != null)
					{
						return m_userSortTargetInfo.SortTree;
					}
					if (m_grouping != null)
					{
						return m_grouping.Tree;
					}
					return null;
				}
				set
				{
					if (m_userSortTargetInfo != null)
					{
						m_userSortTargetInfo.SortTree = value;
					}
					else if (m_grouping != null)
					{
						m_grouping.Tree = value;
					}
					else
					{
						Global.Tracer.Assert(condition: false);
					}
				}
			}

			protected override int ExpressionIndex => 0;

			protected override IntList SortFilterInfoIndices
			{
				get
				{
					if (m_userSortTargetInfo != null)
					{
						return m_userSortTargetInfo.SortFilterInfoIndices;
					}
					return null;
				}
			}

			internal override bool TargetForNonDetailSort
			{
				get
				{
					if (m_userSortTargetInfo != null && m_userSortTargetInfo.TargetForNonDetailSort)
					{
						return true;
					}
					return m_outerScope.TargetForNonDetailSort;
				}
			}

			internal RuntimeListDetailObj(IScope outerScope, List listDef, ref DataActions dataAction, ProcessingContext processingContext)
				: base(outerScope, listDef, (listDef.Filters == null) ? dataAction : DataActions.None, processingContext)
			{
				RuntimeDataRegionObj.CreateAggregates(m_processingContext, listDef.Aggregates, ref m_nonCustomAggregates, ref m_customAggregates);
				RuntimeDataRegionObj.CreateAggregates(m_processingContext, listDef.PostSortAggregates, ref m_postSortAggregates);
				if (listDef.Filters != null)
				{
					m_filters = new Filters(Filters.FilterTypes.DataRegionFilter, this, listDef.Filters, listDef.ObjectType, listDef.Name, m_processingContext);
				}
				else
				{
					dataAction = DataActions.None;
				}
				RunningValueInfoList runningValueInfoList = null;
				if (listDef.ReportItems != null)
				{
					runningValueInfoList = listDef.ReportItems.RunningValues;
				}
				if (runningValueInfoList != null && 0 < runningValueInfoList.Count)
				{
					m_rvValueList = new DataAggregateObjResultsList();
				}
				if (m_processingContext.IsSortFilterTarget(listDef.IsSortFilterTarget, m_outerScope, this, ref m_userSortTargetInfo) && m_userSortTargetInfo.TargetForNonDetailSort)
				{
					m_dataRows = new DataRowList();
				}
				HandleSortFilterEvent(ref m_userSortTargetInfo);
				if (m_userSortTargetInfo == null && listDef.Sorting != null && 0 < listDef.Sorting.SortExpressions.Count)
				{
					m_expression = new RuntimeExpressionInfo(listDef.Sorting.SortExpressions, listDef.Sorting.ExprHost, listDef.Sorting.SortDirections, 0);
					m_grouping = new RuntimeGroupingObj(this, RuntimeGroupingObj.GroupingTypes.Sort);
				}
				m_processingContext.RegisterSortFilterExpressionScope(m_outerScope, this, listDef.IsSortFilterExpressionScope);
			}

			internal RuntimeListDetailObj(RuntimeListDetailObj detailRoot)
				: base(detailRoot)
			{
			}

			internal override bool IsTargetForSort(int index, bool detailSort)
			{
				if (m_userSortTargetInfo != null && m_userSortTargetInfo.IsTargetForSort(index, detailSort))
				{
					return true;
				}
				return m_outerScope.IsTargetForSort(index, detailSort);
			}

			void ISortDataHolder.NextRow()
			{
				NextRow();
			}

			void ISortDataHolder.Traverse(ProcessingStages operation)
			{
				switch (operation)
				{
				case ProcessingStages.SortAndFilter:
					SortAndFilter();
					break;
				case ProcessingStages.RunningValues:
					ReadRows(DataActions.PostSortAggregates);
					break;
				case ProcessingStages.CreatingInstances:
					CreateInstance();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}

			internal override void NextRow()
			{
				if (m_processingContext.ReportObjectModel.FieldsImpl.IsAggregateRow)
				{
					NextAggregateRow();
				}
				else
				{
					NextNonAggregateRow();
				}
			}

			private void NextNonAggregateRow()
			{
				bool flag = true;
				if (m_filters != null)
				{
					flag = m_filters.PassFilters(m_processingContext.ReportObjectModel.FieldsImpl.GetFields());
				}
				if (flag)
				{
					((IFilterOwner)this).PostFilterNextRow();
				}
			}

			private void NextAggregateRow()
			{
				if (m_processingContext.ReportObjectModel.FieldsImpl.AggregationFieldCount == 0)
				{
					RuntimeDataRegionObj.UpdateAggregates(m_processingContext, m_customAggregates, updateAndSetup: false);
				}
			}

			void IFilterOwner.PostFilterNextRow()
			{
				RuntimeDataRegionObj.UpdateAggregates(m_processingContext, m_nonCustomAggregates, updateAndSetup: false);
				if (m_userSortTargetInfo != null)
				{
					ProcessDetailSort(m_userSortTargetInfo);
				}
				else
				{
					base.NextRow();
				}
			}

			protected override void SendToInner()
			{
			}

			internal override void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
				if (m_rvValueList != null || m_postSortAggregates != null || (m_outerDataAction & DataActions.PostSortAggregates) != 0)
				{
					m_globalRunningValueCollection = globalRVCol;
					m_groupCollection = groupCol;
					ReportItemCollection reportItemCollection = null;
					RunningValueInfoList runningValueInfoList = null;
					reportItemCollection = ((List)m_dataRegionDef).ReportItems;
					if (reportItemCollection != null)
					{
						runningValueInfoList = reportItemCollection.RunningValues;
					}
					if (runningValueInfoList != null && 0 < runningValueInfoList.Count)
					{
						AddRunningValues(runningValueInfoList, lastGroup);
					}
					if (m_userSortTargetInfo != null)
					{
						bool sortDirection = m_processingContext.RuntimeSortFilterInfo[m_userSortTargetInfo.SortFilterInfoIndices[0]].SortDirection;
						m_userSortTargetInfo.SortTree.Traverse(ProcessingStages.RunningValues, sortDirection);
						m_userSortTargetInfo = null;
					}
					else
					{
						base.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
					}
				}
			}

			internal override void ReadRow(DataActions dataAction)
			{
				if (DataActions.PostSortAggregates == dataAction && m_postSortAggregates != null)
				{
					RuntimeDataRegionObj.UpdateAggregates(m_processingContext, m_postSortAggregates, updateAndSetup: false);
				}
				base.ReadRow(dataAction);
			}

			internal override void CreateInstances(ReportItemInstance riInstance, IList instanceList, RenderingPagesRangesList pagesList)
			{
				if (m_userSortTargetInfo != null)
				{
					m_reportItemInstance = riInstance;
					m_instanceList = instanceList;
					m_pagesList = pagesList;
					bool sortDirection = m_processingContext.RuntimeSortFilterInfo[m_userSortTargetInfo.SortFilterInfoIndices[0]].SortDirection;
					m_userSortTargetInfo.SortTree.Traverse(ProcessingStages.CreatingInstances, sortDirection);
					m_userSortTargetInfo = null;
				}
				else
				{
					base.CreateInstances(riInstance, instanceList, pagesList);
				}
			}

			internal override void CreateInstance()
			{
				RuntimeListDetailObj obj = (RuntimeListDetailObj)m_hierarchyRoot;
				ListInstance listInstance = (ListInstance)obj.m_reportItemInstance;
				IList instanceList = obj.m_instanceList;
				ReportItemCollection reportItems = ((List)m_dataRegionDef).ReportItems;
				List list = (List)m_dataRegionDef;
				Pagination pagination = m_processingContext.Pagination;
				if (reportItems == null || m_dataRows == null)
				{
					return;
				}
				DataActions dataAction = DataActions.None;
				RuntimeRICollection runtimeRICollection = new RuntimeRICollection(this, reportItems, ref dataAction, m_processingContext, createDataRegions: false);
				double heightValue = list.HeightValue;
				pagination.EnterIgnorePageBreak(list.Visibility, ignoreAlways: false);
				for (int i = 0; i < m_dataRows.Count; i++)
				{
					SetupEnvironment(i, reportItems.RunningValues);
					ListContentInstance listContentInstance = new ListContentInstance(m_processingContext, (List)m_dataRegionDef);
					if (Report.ShowHideTypes.Interactive == m_processingContext.ShowHideType)
					{
						m_processingContext.EnterGrouping();
						((IShowHideContainer)listContentInstance).BeginProcessContainer(m_processingContext);
					}
					if (!pagination.IgnoreHeight)
					{
						pagination.AddToCurrentPageHeight(list, heightValue);
					}
					if (!pagination.IgnorePageBreak && pagination.CurrentPageHeight >= pagination.PageHeight && listInstance.NumberOfContentsOnThisPage > 0)
					{
						RenderingPagesRanges renderingPagesRanges = default(RenderingPagesRanges);
						renderingPagesRanges.StartRow = instanceList.Count - listInstance.NumberOfContentsOnThisPage;
						renderingPagesRanges.NumberOfDetails = listInstance.NumberOfContentsOnThisPage;
						pagination.SetCurrentPageHeight(list, 0.0);
						list.ContentStartPage++;
						list.BottomInEndPage = 0.0;
						listInstance.ChildrenStartAndEndPages.Add(renderingPagesRanges);
						listInstance.NumberOfContentsOnThisPage = 1;
					}
					else
					{
						listInstance.NumberOfContentsOnThisPage++;
					}
					int startPage = list.StartPage;
					list.StartPage = list.ContentStartPage;
					pagination.EnterIgnorePageBreak(null, ignoreAlways: true);
					pagination.EnterIgnoreHeight(startHidden: true);
					m_dataRegionDef.CurrentDetailRowIndex = i;
					m_processingContext.PageSectionContext.EnterVisibilityScope(list.Visibility, list.StartHidden);
					runtimeRICollection.CreateInstances(listContentInstance.ReportItemColInstance);
					m_processingContext.PageSectionContext.ExitVisibilityScope();
					pagination.LeaveIgnoreHeight(startHidden: true);
					pagination.LeaveIgnorePageBreak(null, ignoreAlways: true);
					pagination.ProcessEndGroupPage(0.0, pageBreakAtEnd: false, list, listInstance.NumberOfContentsOnThisPage > 0, list.StartHidden);
					if (Report.ShowHideTypes.Interactive == m_processingContext.ShowHideType)
					{
						((IShowHideContainer)listContentInstance).EndProcessContainer(m_processingContext);
						m_processingContext.ExitGrouping();
					}
					list.StartPage = startPage;
					instanceList.Add(listContentInstance);
					runtimeRICollection.ResetReportItemObjs();
				}
				list.EndPage = Math.Max(list.ContentStartPage, list.EndPage);
				pagination.LeaveIgnorePageBreak(list.Visibility, ignoreAlways: false);
			}

			internal override void SetupEnvironment()
			{
				SetupEnvironment(m_nonCustomAggregates, m_customAggregates, (m_dataRows == null) ? null : m_dataRows[0]);
				SetupAggregates(m_postSortAggregates);
			}

			internal override bool InScope(string scope)
			{
				return DataRegionInScope(m_dataRegionDef, scope);
			}

			protected override int GetRecursiveLevel(string scope)
			{
				return DataRegionRecursiveLevel(m_dataRegionDef, scope);
			}

			protected override IHierarchyObj CreateHierarchyObj()
			{
				if (m_userSortTargetInfo != null)
				{
					return new RuntimeSortHierarchyObj(this);
				}
				return base.CreateHierarchyObj();
			}

			protected override void ProcessUserSort()
			{
				m_processingContext.ProcessUserSortForTarget(this, ref m_dataRows, m_userSortTargetInfo.TargetForNonDetailSort);
			}

			protected override void MarkSortInfoProcessed(RuntimeSortFilterEventInfoList runtimeSortFilterInfo)
			{
				if (m_userSortTargetInfo != null)
				{
					m_userSortTargetInfo.MarkSortInfoProcessed(runtimeSortFilterInfo, this);
				}
			}

			protected override void AddSortInfoIndex(int sortInfoIndex, RuntimeSortFilterEventInfo sortInfo)
			{
				if (m_userSortTargetInfo != null)
				{
					m_userSortTargetInfo.AddSortInfoIndex(sortInfoIndex, sortInfo);
				}
			}

			internal override bool TargetScopeMatched(int index, bool detailSort)
			{
				return m_outerScope.TargetScopeMatched(index, detailSort);
			}

			internal override void GetScopeValues(IHierarchyObj targetScopeObj, VariantList[] scopeValues, ref int index)
			{
				if (targetScopeObj == null)
				{
					base.GetScopeValues(targetScopeObj, scopeValues, ref index);
				}
				else if (this != targetScopeObj)
				{
					m_outerScope.GetScopeValues(targetScopeObj, scopeValues, ref index);
				}
			}

			protected override void GetGroupNameValuePairs(Dictionary<string, object> pairs)
			{
				DataRegionGetGroupNameValuePairs(m_dataRegionDef, pairs);
			}
		}

		private sealed class RuntimeOnePassListDetailObj : RuntimeOnePassDetailObj, IFilterOwner
		{
			private RuntimeRICollection m_reportItemCol;

			private Filters m_filters;

			private ListContentInstanceList m_listContentInstances;

			internal ListContentInstanceList ListContentInstances => m_listContentInstances;

			protected override string ScopeName => m_dataRegionDef.Name;

			internal RuntimeOnePassListDetailObj(IScope outerScope, List listDef, ProcessingContext processingContext)
				: base(outerScope, listDef, processingContext)
			{
				Global.Tracer.Assert(listDef.Sorting == null, "(null == listDef.Sorting)");
				Global.Tracer.Assert(listDef.Aggregates == null, "(null == listDef.Aggregates)");
				if (listDef.Filters != null)
				{
					m_filters = new Filters(Filters.FilterTypes.DataRegionFilter, this, listDef.Filters, listDef.ObjectType, listDef.Name, m_processingContext);
				}
				if (listDef.ReportItems != null)
				{
					m_reportItemCol = new RuntimeRICollection(this, listDef.ReportItems, m_processingContext, createDataRegions: false);
					AddRunningValues(listDef.ReportItems.RunningValues);
				}
				m_listContentInstances = new ListContentInstanceList();
				listDef.ContentStartPage = 0;
			}

			internal override int GetDetailPage()
			{
				return ((List)m_dataRegionDef).ContentStartPage;
			}

			internal override void NextRow()
			{
				if (!m_processingContext.ReportObjectModel.FieldsImpl.IsAggregateRow)
				{
					bool flag = true;
					if (m_filters != null)
					{
						flag = m_filters.PassFilters(m_processingContext.ReportObjectModel.FieldsImpl.GetFields());
					}
					if (flag)
					{
						((IFilterOwner)this).PostFilterNextRow();
					}
				}
			}

			void IFilterOwner.PostFilterNextRow()
			{
				base.NextRow();
			}

			protected override void CreateInstance()
			{
				List list = (List)m_dataRegionDef;
				double heightValue = list.HeightValue;
				Pagination pagination = m_processingContext.Pagination;
				m_pagination.CopyPaginationInfo(pagination);
				m_processingContext.Pagination = m_pagination;
				NavigationInfo navigationInfo = m_processingContext.NavigationInfo;
				m_processingContext.NavigationInfo = m_navigationInfo;
				m_pagination.EnterIgnorePageBreak(list.Visibility, ignoreAlways: false);
				ListContentInstance listContentInstance = new ListContentInstance(m_processingContext, list);
				if (Report.ShowHideTypes.Interactive == m_processingContext.ShowHideType)
				{
					m_processingContext.EnterGrouping();
					((IShowHideContainer)listContentInstance).BeginProcessContainer(m_processingContext);
				}
				if (!m_pagination.IgnoreHeight)
				{
					m_pagination.AddToCurrentPageHeight(list, heightValue);
				}
				if (!m_pagination.IgnorePageBreak && m_pagination.CurrentPageHeight >= m_pagination.PageHeight && m_numberOfContentsOnThisPage > 0)
				{
					RenderingPagesRanges renderingPagesRanges = default(RenderingPagesRanges);
					renderingPagesRanges.StartRow = m_listContentInstances.Count - m_numberOfContentsOnThisPage;
					renderingPagesRanges.NumberOfDetails = m_numberOfContentsOnThisPage;
					m_pagination.SetCurrentPageHeight(list, 0.0);
					list.ContentStartPage++;
					list.BottomInEndPage = 0.0;
					m_renderingPages.Add(renderingPagesRanges);
					m_numberOfContentsOnThisPage = 1;
				}
				else
				{
					m_numberOfContentsOnThisPage++;
				}
				int startPage = list.StartPage;
				list.StartPage = list.ContentStartPage;
				if (m_processingContext.ReportObjectModel.FieldsImpl.AddRowIndex)
				{
					m_dataRegionDef.CurrentDetailRowIndex = m_processingContext.ReportObjectModel.FieldsImpl.GetRowIndex();
				}
				m_pagination.EnterIgnorePageBreak(null, ignoreAlways: true);
				m_pagination.EnterIgnoreHeight(startHidden: true);
				m_processingContext.PageSectionContext.EnterVisibilityScope(list.Visibility, list.StartHidden);
				m_reportItemCol.CreateInstances(listContentInstance.ReportItemColInstance);
				m_processingContext.PageSectionContext.ExitVisibilityScope();
				m_pagination.LeaveIgnoreHeight(startHidden: true);
				m_pagination.LeaveIgnorePageBreak(null, ignoreAlways: true);
				if (Report.ShowHideTypes.Interactive == m_processingContext.ShowHideType)
				{
					((IShowHideContainer)listContentInstance).EndProcessContainer(m_processingContext);
				}
				if (Report.ShowHideTypes.Interactive == m_processingContext.ShowHideType)
				{
					m_processingContext.ExitGrouping();
				}
				m_pagination.ProcessEndGroupPage(0.0, pageBreakAtEnd: false, list, m_numberOfContentsOnThisPage > 0, list.StartHidden);
				list.StartPage = startPage;
				list.EndPage = Math.Max(list.ContentStartPage, list.EndPage);
				m_pagination.LeaveIgnorePageBreak(list.Visibility, ignoreAlways: false);
				m_processingContext.Pagination = pagination;
				m_processingContext.NavigationInfo = navigationInfo;
				m_listContentInstances.Add(listContentInstance);
			}

			internal override bool InScope(string scope)
			{
				return DataRegionInScope(m_dataRegionDef, scope);
			}
		}

		internal abstract class RuntimeRDLDataRegionObj : RuntimeDataRegionObj, IFilterOwner, IHierarchyObj
		{
			protected IScope m_outerScope;

			protected FieldImpl[] m_firstRow;

			protected bool m_firstRowIsAggregate;

			protected Filters m_filters;

			protected DataAggregateObjList m_nonCustomAggregates;

			protected DataAggregateObjList m_customAggregates;

			protected DataActions m_dataAction;

			protected DataActions m_outerDataAction;

			protected DataAggregateObjList m_runningValues;

			protected DataAggregateObjResult[] m_runningValueValues;

			protected DataAggregateObjList m_postSortAggregates;

			protected DataRowList m_dataRows;

			protected DataActions m_innerDataAction;

			protected RuntimeUserSortTargetInfo m_userSortTargetInfo;

			protected int[] m_sortFilterExpressionScopeInfoIndices;

			protected override IScope OuterScope => m_outerScope;

			protected abstract DataRegion DataRegionDef
			{
				get;
			}

			internal override bool TargetForNonDetailSort
			{
				get
				{
					if (m_userSortTargetInfo != null && m_userSortTargetInfo.TargetForNonDetailSort)
					{
						return true;
					}
					return m_outerScope.TargetForNonDetailSort;
				}
			}

			protected override int[] SortFilterExpressionScopeInfoIndices
			{
				get
				{
					if (m_sortFilterExpressionScopeInfoIndices == null)
					{
						m_sortFilterExpressionScopeInfoIndices = new int[m_processingContext.RuntimeSortFilterInfo.Count];
						for (int i = 0; i < m_processingContext.RuntimeSortFilterInfo.Count; i++)
						{
							m_sortFilterExpressionScopeInfoIndices[i] = -1;
						}
					}
					return m_sortFilterExpressionScopeInfoIndices;
				}
			}

			IHierarchyObj IHierarchyObj.HierarchyRoot => this;

			ProcessingContext IHierarchyObj.ProcessingContext => m_processingContext;

			BTreeNode IHierarchyObj.SortTree
			{
				get
				{
					if (m_userSortTargetInfo != null)
					{
						return m_userSortTargetInfo.SortTree;
					}
					return null;
				}
				set
				{
					if (m_userSortTargetInfo != null)
					{
						m_userSortTargetInfo.SortTree = value;
					}
					else
					{
						Global.Tracer.Assert(condition: false);
					}
				}
			}

			int IHierarchyObj.ExpressionIndex => 0;

			IntList IHierarchyObj.SortFilterInfoIndices
			{
				get
				{
					if (m_userSortTargetInfo != null)
					{
						return m_userSortTargetInfo.SortFilterInfoIndices;
					}
					return null;
				}
			}

			bool IHierarchyObj.IsDetail => false;

			internal RuntimeRDLDataRegionObj(IScope outerScope, DataRegion dataRegionDef, ref DataActions dataAction, ProcessingContext processingContext, bool onePassProcess, RunningValueInfoList runningValues)
				: base(processingContext)
			{
				m_outerScope = outerScope;
				RuntimeDataRegionObj.CreateAggregates(m_processingContext, dataRegionDef.Aggregates, ref m_nonCustomAggregates, ref m_customAggregates);
				if (dataRegionDef.Filters != null)
				{
					m_filters = new Filters(Filters.FilterTypes.DataRegionFilter, this, dataRegionDef.Filters, dataRegionDef.ObjectType, dataRegionDef.Name, m_processingContext);
				}
				else
				{
					m_outerDataAction = dataAction;
					m_dataAction = dataAction;
					dataAction = DataActions.None;
				}
				if (onePassProcess)
				{
					if (runningValues != null && 0 < runningValues.Count)
					{
						RuntimeDataRegionObj.CreateAggregates(m_processingContext, runningValues, ref m_nonCustomAggregates);
					}
				}
				else if (runningValues != null && 0 < runningValues.Count)
				{
					m_dataAction |= DataActions.PostSortAggregates;
				}
			}

			internal override bool IsTargetForSort(int index, bool detailSort)
			{
				if (m_userSortTargetInfo != null && m_userSortTargetInfo.IsTargetForSort(index, detailSort))
				{
					return true;
				}
				return m_outerScope.IsTargetForSort(index, detailSort);
			}

			IHierarchyObj IHierarchyObj.CreateHierarchyObj()
			{
				return new RuntimeSortHierarchyObj(this);
			}

			ProcessingMessageList IHierarchyObj.RegisterComparisonError(string propertyName)
			{
				return m_processingContext.RegisterComparisonErrorForSortFilterEvent(propertyName);
			}

			void IHierarchyObj.NextRow()
			{
				Global.Tracer.Assert(condition: false);
			}

			void IHierarchyObj.Traverse(ProcessingStages operation)
			{
				Global.Tracer.Assert(condition: false);
			}

			void IHierarchyObj.ReadRow()
			{
				ReadRow(DataActions.UserSort);
			}

			void IHierarchyObj.ProcessUserSort()
			{
				m_processingContext.ProcessUserSortForTarget(this, ref m_dataRows, m_userSortTargetInfo.TargetForNonDetailSort);
				m_dataAction &= ~DataActions.UserSort;
				if (m_userSortTargetInfo.TargetForNonDetailSort)
				{
					m_userSortTargetInfo.ResetTargetForNonDetailSort();
					m_userSortTargetInfo.EnterProcessUserSortPhase(m_processingContext);
					DataActions innerDataAction = m_innerDataAction;
					ConstructRuntimeStructure(ref innerDataAction);
					if (m_dataAction != 0)
					{
						m_dataRows = new DataRowList();
					}
					ScopeFinishSorting(ref m_firstRow, m_userSortTargetInfo);
					m_userSortTargetInfo.LeaveProcessUserSortPhase(m_processingContext);
				}
			}

			void IHierarchyObj.MarkSortInfoProcessed(RuntimeSortFilterEventInfoList runtimeSortFilterInfo)
			{
				if (m_userSortTargetInfo != null)
				{
					m_userSortTargetInfo.MarkSortInfoProcessed(runtimeSortFilterInfo, this);
				}
			}

			void IHierarchyObj.AddSortInfoIndex(int sortInfoIndex, RuntimeSortFilterEventInfo sortInfo)
			{
				if (m_userSortTargetInfo != null)
				{
					m_userSortTargetInfo.AddSortInfoIndex(sortInfoIndex, sortInfo);
				}
			}

			protected abstract void ConstructRuntimeStructure(ref DataActions innerDataAction);

			protected DataActions HandleSortFilterEvent()
			{
				DataActions result = DataActions.None;
				if (m_processingContext.IsSortFilterTarget(DataRegionDef.IsSortFilterTarget, m_outerScope, this, ref m_userSortTargetInfo) && m_userSortTargetInfo.TargetForNonDetailSort)
				{
					result = DataActions.UserSort;
				}
				m_processingContext.RegisterSortFilterExpressionScope(m_outerScope, this, DataRegionDef.IsSortFilterExpressionScope);
				return result;
			}

			internal override bool TargetScopeMatched(int index, bool detailSort)
			{
				return m_outerScope.TargetScopeMatched(index, detailSort);
			}

			internal override void GetScopeValues(IHierarchyObj targetScopeObj, VariantList[] scopeValues, ref int index)
			{
				if (this != targetScopeObj)
				{
					m_outerScope.GetScopeValues(targetScopeObj, scopeValues, ref index);
				}
			}

			internal override void NextRow()
			{
				if (m_processingContext.ReportObjectModel.FieldsImpl.AggregationFieldCount == 0)
				{
					RuntimeDataRegionObj.UpdateAggregates(m_processingContext, m_customAggregates, updateAndSetup: false);
				}
				if (m_processingContext.ReportObjectModel.FieldsImpl.IsAggregateRow)
				{
					ScopeNextAggregateRow(m_userSortTargetInfo);
				}
				else
				{
					NextNonAggregateRow();
				}
			}

			private void NextNonAggregateRow()
			{
				bool flag = true;
				if (m_filters != null)
				{
					flag = m_filters.PassFilters(m_processingContext.ReportObjectModel.FieldsImpl.GetFields());
				}
				if (flag)
				{
					((IFilterOwner)this).PostFilterNextRow();
				}
			}

			void IFilterOwner.PostFilterNextRow()
			{
				RuntimeDataRegionObj.CommonFirstRow(m_processingContext.ReportObjectModel.FieldsImpl, ref m_firstRowIsAggregate, ref m_firstRow);
				ScopeNextNonAggregateRow(m_nonCustomAggregates, m_dataRows);
			}

			internal override bool SortAndFilter()
			{
				if ((ProcessingContext.SecondPassOperations.Filtering & m_processingContext.SecondPassOperation) != 0 && m_dataRows != null && (m_outerDataAction & DataActions.RecursiveAggregates) != 0)
				{
					ReadRows(DataActions.RecursiveAggregates);
					ReleaseDataRows(DataActions.RecursiveAggregates, ref m_dataAction, ref m_dataRows);
				}
				return true;
			}

			protected void ReadRows(DataActions action)
			{
				for (int i = 0; i < m_dataRows.Count; i++)
				{
					FieldImpl[] fields = m_dataRows[i];
					m_processingContext.ReportObjectModel.FieldsImpl.SetFields(fields);
					ReadRow(action);
				}
			}

			protected void SetupEnvironment(RunningValueInfoList runningValues)
			{
				SetupEnvironment(m_nonCustomAggregates, m_customAggregates, m_firstRow);
				SetupAggregates(m_postSortAggregates);
				SetupRunningValues(runningValues, m_runningValueValues);
			}

			internal override bool InScope(string scope)
			{
				return DataRegionInScope(DataRegionDef, scope);
			}

			protected override int GetRecursiveLevel(string scope)
			{
				return DataRegionRecursiveLevel(DataRegionDef, scope);
			}

			protected override void GetGroupNameValuePairs(Dictionary<string, object> pairs)
			{
				DataRegionGetGroupNameValuePairs(DataRegionDef, pairs);
			}
		}

		private sealed class RuntimeTableObj : RuntimeRDLDataRegionObj
		{
			private Table m_tableDef;

			private RuntimeGroupRootObj m_tableGroups;

			private RuntimeDataRegionObj m_detailObj;

			private RuntimeRICollectionList m_headerReportItemCols;

			private RuntimeRICollectionList m_footerReportItemCols;

			internal TableDetailInstanceList TableDetailInstances
			{
				get
				{
					if (m_detailObj is RuntimeOnePassTableDetailObj)
					{
						return ((RuntimeOnePassTableDetailObj)m_detailObj).TableDetailInstances;
					}
					return null;
				}
			}

			internal RenderingPagesRangesList ChildrenStartAndEndPages
			{
				get
				{
					if (m_detailObj is RuntimeOnePassTableDetailObj)
					{
						return ((RuntimeOnePassTableDetailObj)m_detailObj).ChildrenStartAndEndPages;
					}
					return null;
				}
			}

			protected override string ScopeName => m_tableDef.Name;

			protected override DataRegion DataRegionDef => m_tableDef;

			internal RuntimeTableObj(IScope outerScope, Table tableDef, ref DataActions dataAction, ProcessingContext processingContext, bool onePassProcess)
				: base(outerScope, tableDef, ref dataAction, processingContext, onePassProcess, tableDef.RunningValues)
			{
				m_tableDef = tableDef;
				DataActions dataActions = HandleSortFilterEvent();
				if (onePassProcess)
				{
					RuntimeDataRegionObj.CreateAggregates(m_processingContext, tableDef.PostSortAggregates, ref m_nonCustomAggregates);
					if (tableDef.TableDetail != null)
					{
						m_detailObj = new RuntimeOnePassTableDetailObj(this, tableDef, processingContext);
					}
					TableRowList headerRows = tableDef.HeaderRows;
					if (headerRows != null)
					{
						m_headerReportItemCols = new RuntimeRICollectionList(headerRows.Count);
						for (int i = 0; i < headerRows.Count; i++)
						{
							m_headerReportItemCols.Add(new RuntimeRICollection(this, headerRows[i].ReportItems, m_processingContext, createDataRegions: true));
						}
					}
					headerRows = tableDef.FooterRows;
					if (headerRows != null)
					{
						m_footerReportItemCols = new RuntimeRICollectionList(headerRows.Count);
						for (int j = 0; j < headerRows.Count; j++)
						{
							m_footerReportItemCols.Add(new RuntimeRICollection(this, headerRows[j].ReportItems, m_processingContext, createDataRegions: true));
						}
					}
				}
				else
				{
					bool flag = false;
					if (tableDef.PostSortAggregates != null)
					{
						RuntimeDataRegionObj.CreateAggregates(m_processingContext, tableDef.PostSortAggregates, ref m_postSortAggregates);
						m_dataAction |= DataActions.PostSortAggregates;
						if (tableDef.TableDetail == null || tableDef.TableGroups != null)
						{
							flag = true;
						}
					}
					DataActions innerDataAction = m_innerDataAction = ((!flag) ? m_dataAction : DataActions.None);
					ConstructRuntimeStructure(ref innerDataAction);
					if (!flag)
					{
						m_dataAction = innerDataAction;
					}
				}
				m_dataAction |= dataActions;
				if (m_dataAction != 0)
				{
					m_dataRows = new DataRowList();
				}
			}

			protected override void ConstructRuntimeStructure(ref DataActions innerDataAction)
			{
				if (m_tableDef.TableGroups != null)
				{
					m_tableGroups = new RuntimeTableGroupRootObj(this, m_tableDef.TableGroups, ref innerDataAction, m_processingContext);
				}
				else if (m_tableDef.TableDetail != null)
				{
					innerDataAction = m_dataAction;
					m_detailObj = new RuntimeTableDetailObj(this, m_tableDef, ref innerDataAction, m_processingContext);
				}
				TableRowList headerRows = m_tableDef.HeaderRows;
				if (headerRows != null)
				{
					m_headerReportItemCols = new RuntimeRICollectionList(headerRows.Count);
					for (int i = 0; i < headerRows.Count; i++)
					{
						RuntimeRICollection value = new RuntimeRICollection(this, headerRows[i].ReportItems, ref innerDataAction, m_processingContext, createDataRegions: true);
						m_headerReportItemCols.Add(value);
					}
				}
				headerRows = m_tableDef.FooterRows;
				if (headerRows != null)
				{
					m_footerReportItemCols = new RuntimeRICollectionList(headerRows.Count);
					for (int j = 0; j < headerRows.Count; j++)
					{
						RuntimeRICollection value = new RuntimeRICollection(this, headerRows[j].ReportItems, ref innerDataAction, m_processingContext, createDataRegions: true);
						m_footerReportItemCols.Add(value);
					}
				}
			}

			protected override void SendToInner()
			{
				if (m_headerReportItemCols != null)
				{
					m_headerReportItemCols.FirstPassNextDataRow();
				}
				if (m_footerReportItemCols != null)
				{
					m_footerReportItemCols.FirstPassNextDataRow();
				}
				if (m_tableGroups != null)
				{
					m_tableGroups.NextRow();
				}
				if (m_detailObj != null)
				{
					m_detailObj.NextRow();
				}
			}

			internal override bool SortAndFilter()
			{
				SetupEnvironment();
				if (m_userSortTargetInfo != null)
				{
					m_userSortTargetInfo.EnterProcessUserSortPhase(m_processingContext);
				}
				if (m_tableGroups != null)
				{
					m_tableGroups.SortAndFilter();
				}
				if (m_detailObj != null)
				{
					m_detailObj.SortAndFilter();
				}
				bool result = base.SortAndFilter();
				if (m_userSortTargetInfo != null)
				{
					m_userSortTargetInfo.LeaveProcessUserSortPhase(m_processingContext);
				}
				return result;
			}

			internal override void ReadRow(DataActions dataAction)
			{
				if (DataActions.UserSort == dataAction)
				{
					RuntimeDataRegionObj.CommonFirstRow(m_processingContext.ReportObjectModel.FieldsImpl, ref m_firstRowIsAggregate, ref m_firstRow);
					CommonNextRow(m_dataRows);
					return;
				}
				if (DataActions.PostSortAggregates == dataAction)
				{
					if (m_postSortAggregates != null)
					{
						RuntimeDataRegionObj.UpdateAggregates(m_processingContext, m_postSortAggregates, updateAndSetup: false);
					}
					if (m_runningValues != null)
					{
						for (int i = 0; i < m_runningValues.Count; i++)
						{
							m_runningValues[i].Update();
						}
					}
					CalculatePreviousAggregates();
				}
				if (m_outerScope != null && (dataAction & m_outerDataAction) != 0)
				{
					m_outerScope.ReadRow(dataAction);
				}
			}

			private void CalculatePreviousAggregates()
			{
				if (!m_processedPreviousAggregates && m_processingContext.GlobalRVCollection != null)
				{
					Global.Tracer.Assert(m_runningValueValues == null, "(null == m_runningValueValues)");
					RunningValueInfoList runningValues = m_tableDef.RunningValues;
					if (runningValues != null && 0 < runningValues.Count)
					{
						RuntimeRICollection.DoneReadingRows(m_processingContext.GlobalRVCollection, runningValues, ref m_runningValueValues, processPreviousAggregates: true);
					}
					if (m_headerReportItemCols != null)
					{
						m_headerReportItemCols.CalculatePreviousAggregates(m_processingContext.GlobalRVCollection);
					}
					if (m_footerReportItemCols != null)
					{
						m_footerReportItemCols.CalculatePreviousAggregates(m_processingContext.GlobalRVCollection);
					}
					m_processedPreviousAggregates = true;
				}
			}

			internal override void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
				if (m_tableDef.RunningValues != null)
				{
					RuntimeDetailObj.AddRunningValues(m_processingContext, m_tableDef.RunningValues, ref m_runningValues, globalRVCol, groupCol, lastGroup);
				}
				if (m_headerReportItemCols != null)
				{
					m_headerReportItemCols.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
				}
				if (m_footerReportItemCols != null)
				{
					m_footerReportItemCols.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
				}
				if (m_tableGroups != null)
				{
					m_tableGroups.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
				}
				if (m_detailObj != null)
				{
					m_detailObj.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
				}
				if (m_dataRows != null)
				{
					ReadRows(DataActions.PostSortAggregates);
					m_dataRows = null;
				}
				RuntimeRICollection.DoneReadingRows(globalRVCol, m_tableDef.RunningValues, ref m_runningValueValues, processPreviousAggregates: false);
			}

			internal static void CreateRowInstances(ProcessingContext processingContext, RuntimeRICollectionList rowRICols, TableRowInstance[] rowInstances, bool repeatOnNewPages, bool enterGrouping)
			{
				if (rowRICols == null)
				{
					return;
				}
				if (Report.ShowHideTypes.Interactive == processingContext.ShowHideType && enterGrouping)
				{
					processingContext.EnterGrouping();
				}
				for (int i = 0; i < rowRICols.Count; i++)
				{
					if (Report.ShowHideTypes.Interactive == processingContext.ShowHideType)
					{
						((IShowHideContainer)rowInstances[i]).BeginProcessContainer(processingContext);
					}
					processingContext.Pagination.EnterIgnorePageBreak(null, ignoreAlways: true);
					processingContext.Pagination.EnterIgnoreHeight(startHidden: true);
					processingContext.PageSectionContext.EnterVisibilityScope(rowInstances[i].TableRowDef.Visibility, rowInstances[i].TableRowDef.StartHidden);
					IntList tableColumnSpans = processingContext.PageSectionContext.TableColumnSpans;
					processingContext.PageSectionContext.TableColumnSpans = rowInstances[i].TableRowDef.ColSpans;
					rowRICols[i].CreateInstances(rowInstances[i].TableRowReportItemColInstance, ignorePageBreaks: true, repeatOnNewPages);
					processingContext.PageSectionContext.TableColumnSpans = tableColumnSpans;
					processingContext.PageSectionContext.ExitVisibilityScope();
					processingContext.Pagination.LeaveIgnoreHeight(startHidden: true);
					processingContext.Pagination.LeaveIgnorePageBreak(null, ignoreAlways: true);
					if (Report.ShowHideTypes.Interactive == processingContext.ShowHideType)
					{
						((IShowHideContainer)rowInstances[i]).EndProcessContainer(processingContext);
					}
				}
				if (Report.ShowHideTypes.Interactive == processingContext.ShowHideType && enterGrouping)
				{
					processingContext.ExitGrouping();
				}
			}

			internal override void CreateInstances(ReportItemInstance riInstance, IList instanceList, RenderingPagesRangesList pagesList)
			{
				SetupEnvironment();
				TableInstance tableInstance = (TableInstance)riInstance;
				Table table = (Table)tableInstance.ReportItemDef;
				m_processingContext.Pagination.InitProcessTableRenderingPages(tableInstance, table);
				m_processingContext.PageSectionContext.RegisterTableColumnVisibility(m_processingContext.IsOnePass, table.TableColumns, table.ColumnsStartHidden);
				m_processingContext.PageSectionContext.EnterRepeatingItem();
				CreateRowInstances(m_processingContext, m_headerReportItemCols, tableInstance.HeaderRowInstances, table.HeaderRepeatOnNewPage, enterGrouping: false);
				PageTextboxes source = m_processingContext.PageSectionContext.ExitRepeatingItem();
				m_processingContext.PageSectionContext.EnterRepeatingItem();
				CreateRowInstances(m_processingContext, m_footerReportItemCols, tableInstance.FooterRowInstances, table.FooterRepeatOnNewPage, enterGrouping: false);
				PageTextboxes source2 = m_processingContext.PageSectionContext.ExitRepeatingItem();
				bool delayAddingInstanceInfo = m_processingContext.DelayAddingInstanceInfo;
				m_processingContext.DelayAddingInstanceInfo = false;
				if (m_tableGroups != null)
				{
					m_tableGroups.CreateInstances(tableInstance, tableInstance.TableGroupInstances, tableInstance.ChildrenStartAndEndPages);
					m_tableGroups = null;
				}
				else if (m_detailObj != null)
				{
					int numberOfChildrenOnThisPage = 0;
					if (m_detailObj is RuntimeDetailObj)
					{
						((RuntimeDetailObj)m_detailObj).CreateInstances(tableInstance, tableInstance.TableDetailInstances, tableInstance.ChildrenStartAndEndPages, out numberOfChildrenOnThisPage);
						tableInstance.NumberOfChildrenOnThisPage = numberOfChildrenOnThisPage;
					}
					else
					{
						RenderingPagesRangesList childrenStartAndEndPages = tableInstance.ChildrenStartAndEndPages;
						m_detailObj.CreateInstances(tableInstance, tableInstance.TableDetailInstances, childrenStartAndEndPages);
						Global.Tracer.Assert(m_detailObj is RuntimeOnePassTableDetailObj, "(m_detailObj is RuntimeOnePassTableDetailObj)");
						RuntimeOnePassTableDetailObj runtimeOnePassTableDetailObj = (RuntimeOnePassTableDetailObj)m_detailObj;
						if (childrenStartAndEndPages != null && (!m_processingContext.PageSectionContext.IsParentVisible() || !Visibility.IsOnePassHierarchyVisible(tableInstance.ReportItemDef)))
						{
							runtimeOnePassTableDetailObj.MoveAllToFirstPage();
							int totalCount = (tableInstance.TableDetailInstances != null) ? tableInstance.TableDetailInstances.Count : 0;
							childrenStartAndEndPages.MoveAllToFirstPage(totalCount);
							runtimeOnePassTableDetailObj.NumberOfContentsOnThisPage = 0;
						}
						numberOfChildrenOnThisPage = (tableInstance.NumberOfChildrenOnThisPage = runtimeOnePassTableDetailObj.NumberOfContentsOnThisPage);
						if (numberOfChildrenOnThisPage > 0)
						{
							if (childrenStartAndEndPages != null && 0 < childrenStartAndEndPages.Count)
							{
								m_processingContext.Pagination.SetCurrentPageHeight(table, runtimeOnePassTableDetailObj.Pagination.CurrentPageHeight);
							}
							else
							{
								m_processingContext.Pagination.AddToCurrentPageHeight(table, runtimeOnePassTableDetailObj.Pagination.CurrentPageHeight);
							}
						}
						m_processingContext.NavigationInfo.AppendNavigationInfo(m_processingContext.NavigationInfo.CurrentLabel, runtimeOnePassTableDetailObj.NavigationInfo, table.StartPage);
					}
					if (numberOfChildrenOnThisPage > 0)
					{
						RenderingPagesRanges renderingPagesRanges = default(RenderingPagesRanges);
						renderingPagesRanges.StartRow = tableInstance.TableDetailInstances.Count - numberOfChildrenOnThisPage;
						renderingPagesRanges.NumberOfDetails = numberOfChildrenOnThisPage;
						tableInstance.ChildrenStartAndEndPages.Add(renderingPagesRanges);
					}
					if (m_detailObj is RuntimeOnePassTableDetailObj)
					{
						RenderingPagesRangesList childrenStartAndEndPages2 = tableInstance.ChildrenStartAndEndPages;
						RuntimeOnePassDetailObj runtimeOnePassDetailObj = m_detailObj as RuntimeOnePassDetailObj;
						if (childrenStartAndEndPages2 != null && 0 < childrenStartAndEndPages2.Count)
						{
							int num3 = tableInstance.CurrentPage = (table.CurrentPage = table.StartPage + childrenStartAndEndPages2.Count - 1);
							if (m_processingContext.PageSectionContext.IsParentVisible())
							{
								for (int i = 0; i < childrenStartAndEndPages2.Count; i++)
								{
									runtimeOnePassDetailObj.ProcessOnePassDetailTextboxes(i, table.StartPage + i);
								}
							}
						}
						else
						{
							int num3 = tableInstance.CurrentPage = (table.CurrentPage = table.StartPage);
							if (m_processingContext.PageSectionContext.IsParentVisible() && tableInstance.TableDetailInstances != null)
							{
								runtimeOnePassDetailObj.ProcessOnePassDetailTextboxes(0, tableInstance.CurrentPage);
							}
						}
					}
					m_detailObj = null;
				}
				m_processingContext.PageSectionContext.UnregisterTableColumnVisibility();
				Global.Tracer.Assert(table.StartPage <= table.CurrentPage, "(tableDef.StartPage <= tableDef.CurrentPage)");
				if (m_processingContext.PageSectionContext.PageTextboxes != null)
				{
					m_processingContext.PageSectionContext.PageTextboxes.IntegrateRepeatingTextboxValues(source, table.StartPage, table.HeaderRepeatOnNewPage ? table.CurrentPage : table.StartPage);
					m_processingContext.PageSectionContext.PageTextboxes.IntegrateRepeatingTextboxValues(source2, table.FooterRepeatOnNewPage ? table.StartPage : table.CurrentPage, table.CurrentPage);
				}
				m_processingContext.DelayAddingInstanceInfo = delayAddingInstanceInfo;
			}

			internal override void SetupEnvironment()
			{
				SetupEnvironment(m_tableDef.RunningValues);
			}

			internal void ResetReportItems()
			{
				if (m_headerReportItemCols != null)
				{
					m_headerReportItemCols.ResetReportItemObjs();
				}
				if (m_footerReportItemCols != null)
				{
					m_footerReportItemCols.ResetReportItemObjs();
				}
				m_headerReportItemCols = null;
				m_footerReportItemCols = null;
			}
		}

		private sealed class RuntimeTableGroupRootObj : RuntimeGroupRootObj
		{
			internal RuntimeTableGroupRootObj(IScope outerScope, TableGroup tableGroupDef, ref DataActions dataAction, ProcessingContext processingContext)
				: base(outerScope, tableGroupDef, dataAction, processingContext)
			{
				if (tableGroupDef.RunningValues != null && 0 < tableGroupDef.RunningValues.Count)
				{
					m_dataAction |= DataActions.PostSortAggregates;
				}
				if ((m_dataAction & DataActions.PostSortAggregates) == 0 && tableGroupDef.HeaderRows != null)
				{
					for (int i = 0; i < tableGroupDef.HeaderRows.Count; i++)
					{
						if (tableGroupDef.HeaderRows[i].ReportItems.RunningValues != null && 0 < tableGroupDef.HeaderRows[i].ReportItems.RunningValues.Count)
						{
							m_dataAction |= DataActions.PostSortAggregates;
							break;
						}
					}
				}
				if ((m_dataAction & DataActions.PostSortAggregates) == 0 && tableGroupDef.FooterRows != null)
				{
					for (int j = 0; j < tableGroupDef.FooterRows.Count; j++)
					{
						if (tableGroupDef.FooterRows[j].ReportItems.RunningValues != null && 0 < tableGroupDef.FooterRows[j].ReportItems.RunningValues.Count)
						{
							m_dataAction |= DataActions.PostSortAggregates;
							break;
						}
					}
				}
				if (tableGroupDef.Grouping.Filters == null)
				{
					dataAction = DataActions.None;
				}
			}

			internal override void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
				base.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
				TableGroup tableGroup = (TableGroup)m_hierarchyDef;
				if (tableGroup.HeaderRows != null)
				{
					for (int i = 0; i < tableGroup.HeaderRows.Count; i++)
					{
						AddRunningValues(tableGroup.HeaderRows[i].ReportItems.RunningValues);
					}
				}
				if (tableGroup.FooterRows != null)
				{
					for (int j = 0; j < tableGroup.FooterRows.Count; j++)
					{
						AddRunningValues(tableGroup.FooterRows[j].ReportItems.RunningValues);
					}
				}
				AddRunningValues(tableGroup.RunningValues);
				m_grouping.Traverse(ProcessingStages.RunningValues, m_expression.Direction);
			}
		}

		private sealed class RuntimeTableGroupLeafObj : RuntimeGroupLeafObj
		{
			private RuntimeRICollectionList m_headerReportItemCols;

			private RuntimeRICollectionList m_footerReportItemCols;

			private RuntimeHierarchyObj m_innerHierarchy;

			private DataAggregateObjResult[] m_runningValueValues;

			private TableGroupInstance m_tableGroupInstance;

			private string m_label;

			private int m_startPage = -1;

			internal RuntimeTableGroupLeafObj(RuntimeTableGroupRootObj groupRoot)
				: base(groupRoot)
			{
				m_dataAction = groupRoot.DataAction;
				bool handleMyDataAction = false;
				bool num = HandleSortFilterEvent();
				ConstructRuntimeStructure(ref handleMyDataAction, out DataActions innerDataAction);
				if (!handleMyDataAction)
				{
					m_dataAction = innerDataAction;
				}
				if (num)
				{
					m_dataAction |= DataActions.UserSort;
				}
				if (m_dataAction != 0)
				{
					m_dataRows = new DataRowList();
				}
			}

			protected override void ConstructRuntimeStructure(ref bool handleMyDataAction, out DataActions innerDataAction)
			{
				TableGroup tableGroup = (TableGroup)((RuntimeGroupRootObj)m_hierarchyRoot).HierarchyDef;
				base.ConstructRuntimeStructure(ref handleMyDataAction, out innerDataAction);
				if (tableGroup.InnerHierarchy != null)
				{
					m_innerHierarchy = new RuntimeTableGroupRootObj(this, (TableGroup)tableGroup.InnerHierarchy, ref innerDataAction, m_processingContext);
				}
				else if (((Table)tableGroup.DataRegionDef).TableDetail != null)
				{
					m_innerHierarchy = new RuntimeTableDetailObj(this, (Table)tableGroup.DataRegionDef, ref innerDataAction, m_processingContext);
				}
				TableRowList headerRows = tableGroup.HeaderRows;
				if (headerRows != null)
				{
					m_headerReportItemCols = new RuntimeRICollectionList(headerRows.Count);
					for (int i = 0; i < headerRows.Count; i++)
					{
						RuntimeRICollection value = new RuntimeRICollection(this, headerRows[i].ReportItems, ref innerDataAction, m_processingContext, createDataRegions: true);
						m_headerReportItemCols.Add(value);
					}
				}
				headerRows = tableGroup.FooterRows;
				if (headerRows != null)
				{
					m_footerReportItemCols = new RuntimeRICollectionList(headerRows.Count);
					for (int j = 0; j < headerRows.Count; j++)
					{
						RuntimeRICollection value = new RuntimeRICollection(this, headerRows[j].ReportItems, ref innerDataAction, m_processingContext, createDataRegions: true);
						m_footerReportItemCols.Add(value);
					}
				}
			}

			protected override void SendToInner()
			{
				base.SendToInner();
				if (m_headerReportItemCols != null)
				{
					m_headerReportItemCols.FirstPassNextDataRow();
				}
				if (m_footerReportItemCols != null)
				{
					m_footerReportItemCols.FirstPassNextDataRow();
				}
				if (m_innerHierarchy != null)
				{
					m_innerHierarchy.NextRow();
				}
			}

			internal override bool SortAndFilter()
			{
				SetupEnvironment();
				if (m_userSortTargetInfo != null)
				{
					m_userSortTargetInfo.EnterProcessUserSortPhase(m_processingContext);
				}
				if (m_headerReportItemCols != null)
				{
					for (int i = 0; i < m_headerReportItemCols.Count; i++)
					{
						m_headerReportItemCols[i].SortAndFilter();
					}
				}
				if (m_footerReportItemCols != null)
				{
					for (int j = 0; j < m_footerReportItemCols.Count; j++)
					{
						m_footerReportItemCols[j].SortAndFilter();
					}
				}
				if (m_innerHierarchy != null)
				{
					m_innerHierarchy.SortAndFilter();
				}
				bool result = base.SortAndFilter();
				if (m_userSortTargetInfo != null)
				{
					m_userSortTargetInfo.LeaveProcessUserSortPhase(m_processingContext);
				}
				return result;
			}

			internal override void ReadRow(DataActions dataAction)
			{
				if (DataActions.UserSort == dataAction)
				{
					RuntimeDataRegionObj.CommonFirstRow(m_processingContext.ReportObjectModel.FieldsImpl, ref m_firstRowIsAggregate, ref m_firstRow);
					CommonNextRow(m_dataRows);
					return;
				}
				base.ReadRow(dataAction);
				if (DataActions.PostSortAggregates == dataAction)
				{
					CalculatePreviousAggregates();
				}
			}

			private void CalculatePreviousAggregates()
			{
				if (!m_processedPreviousAggregates && m_processingContext.GlobalRVCollection != null)
				{
					Global.Tracer.Assert(m_runningValueValues == null, "(null == m_runningValueValues)");
					RunningValueInfoList runningValues = ((TableGroup)((RuntimeGroupRootObj)m_hierarchyRoot).HierarchyDef).RunningValues;
					if (runningValues != null && 0 < runningValues.Count)
					{
						RuntimeRICollection.DoneReadingRows(m_processingContext.GlobalRVCollection, runningValues, ref m_runningValueValues, processPreviousAggregates: true);
					}
					if (m_headerReportItemCols != null)
					{
						m_headerReportItemCols.CalculatePreviousAggregates(m_processingContext.GlobalRVCollection);
					}
					if (m_footerReportItemCols != null)
					{
						m_footerReportItemCols.CalculatePreviousAggregates(m_processingContext.GlobalRVCollection);
					}
					m_processedPreviousAggregates = true;
				}
			}

			internal override void CalculateRunningValues()
			{
				RuntimeGroupRootObj runtimeGroupRootObj = (RuntimeGroupRootObj)m_hierarchyRoot;
				AggregatesImpl globalRunningValueCollection = runtimeGroupRootObj.GlobalRunningValueCollection;
				RuntimeGroupRootObjList groupCollection = runtimeGroupRootObj.GroupCollection;
				if (m_innerHierarchy != null)
				{
					m_innerHierarchy.CalculateRunningValues(globalRunningValueCollection, groupCollection, runtimeGroupRootObj);
				}
				if (m_dataRows != null)
				{
					ReadRows(DataActions.PostSortAggregates);
					m_dataRows = null;
				}
				if (m_headerReportItemCols != null)
				{
					m_headerReportItemCols.CalculateRunningValues(globalRunningValueCollection, groupCollection, runtimeGroupRootObj);
				}
				if (m_footerReportItemCols != null)
				{
					m_footerReportItemCols.CalculateRunningValues(globalRunningValueCollection, groupCollection, runtimeGroupRootObj);
				}
				RunningValueInfoList runningValues = ((TableGroup)runtimeGroupRootObj.HierarchyDef).RunningValues;
				if (runningValues != null && 0 < runningValues.Count)
				{
					RuntimeRICollection.DoneReadingRows(globalRunningValueCollection, runningValues, ref m_runningValueValues, processPreviousAggregates: false);
				}
				base.CalculateRunningValues();
			}

			internal override void CreateInstance()
			{
				SetupEnvironment();
				RuntimeGroupRootObj obj = (RuntimeGroupRootObj)m_hierarchyRoot;
				TableInstance tableInstance = (TableInstance)obj.ReportItemInstance;
				Table table = (Table)tableInstance.ReportItemDef;
				IList instanceList = obj.InstanceList;
				TableGroup tableGroup = (TableGroup)obj.HierarchyDef;
				m_processingContext.ChunkManager.CheckPageBreak(tableGroup, atStart: true);
				SetupRunningValues(tableGroup.RunningValues, m_runningValueValues);
				m_tableGroupInstance = new TableGroupInstance(m_processingContext, tableGroup);
				RenderingPagesRanges renderingPagesRanges = default(RenderingPagesRanges);
				m_processingContext.Pagination.InitProcessingTableGroup(tableInstance, table, m_tableGroupInstance, tableGroup, ref renderingPagesRanges, instanceList.Count == 0);
				m_startPage = renderingPagesRanges.StartPage;
				if (Report.ShowHideTypes.Interactive == m_processingContext.ShowHideType)
				{
					m_processingContext.EnterGrouping();
					((IShowHideContainer)m_tableGroupInstance).BeginProcessContainer(m_processingContext);
				}
				if (tableGroup.Grouping.GroupLabel != null)
				{
					m_label = m_processingContext.NavigationInfo.CurrentLabel;
					if (m_label != null)
					{
						m_processingContext.NavigationInfo.EnterDocumentMapChildren();
					}
				}
				m_processingContext.PageSectionContext.EnterVisibilityScope(tableGroup.Visibility, tableGroup.StartHidden);
				m_processingContext.PageSectionContext.EnterRepeatingItem();
				RuntimeTableObj.CreateRowInstances(m_processingContext, m_headerReportItemCols, m_tableGroupInstance.HeaderRowInstances, tableGroup.HeaderRepeatOnNewPage, enterGrouping: false);
				PageTextboxes source = m_processingContext.PageSectionContext.ExitRepeatingItem();
				m_processingContext.PageSectionContext.EnterRepeatingItem();
				RuntimeTableObj.CreateRowInstances(m_processingContext, m_footerReportItemCols, m_tableGroupInstance.FooterRowInstances, tableGroup.FooterRepeatOnNewPage, enterGrouping: false);
				PageTextboxes source2 = m_processingContext.PageSectionContext.ExitRepeatingItem();
				if (m_innerHierarchy != null)
				{
					if (m_tableGroupInstance.SubGroupInstances != null)
					{
						m_innerHierarchy.CreateInstances(tableInstance, m_tableGroupInstance.SubGroupInstances, m_tableGroupInstance.ChildrenStartAndEndPages);
					}
					else
					{
						Global.Tracer.Assert(m_innerHierarchy is RuntimeDetailObj, "(m_innerHierarchy is RuntimeDetailObj)");
						int numberOfChildrenOnThisPage = 0;
						((RuntimeDetailObj)m_innerHierarchy).CreateInstances(tableInstance, m_tableGroupInstance.TableDetailInstances, m_tableGroupInstance.ChildrenStartAndEndPages, out numberOfChildrenOnThisPage);
						m_tableGroupInstance.NumberOfChildrenOnThisPage = numberOfChildrenOnThisPage;
						if (numberOfChildrenOnThisPage > 0)
						{
							RenderingPagesRanges renderingPagesRanges2 = new RenderingPagesRanges
							{
								StartRow = m_tableGroupInstance.TableDetailInstances.Count - numberOfChildrenOnThisPage,
								NumberOfDetails = numberOfChildrenOnThisPage
							};
							m_tableGroupInstance.ChildrenStartAndEndPages.Add(renderingPagesRanges2);
						}
					}
				}
				m_processingContext.PageSectionContext.ExitVisibilityScope();
				if (Report.ShowHideTypes.Interactive == m_processingContext.ShowHideType)
				{
					((IShowHideContainer)m_tableGroupInstance).EndProcessContainer(m_processingContext);
					m_processingContext.ExitGrouping();
				}
				m_processingContext.ChunkManager.CheckPageBreak(tableGroup, atStart: false);
				double footerHeightValue = tableGroup.FooterHeightValue;
				tableGroup.EndPage = tableInstance.CurrentPage;
				m_processingContext.Pagination.ProcessEndGroupPage(footerHeightValue, tableGroup.PropagatedPageBreakAtEnd || tableGroup.Grouping.PageBreakAtEnd, table, m_tableGroupInstance.NumberOfChildrenOnThisPage > 0, tableGroup.StartHidden);
				renderingPagesRanges.EndPage = tableGroup.EndPage;
				obj.PagesList.Add(renderingPagesRanges);
				if (m_processingContext.PageSectionContext.PageTextboxes != null)
				{
					m_processingContext.PageSectionContext.PageTextboxes.IntegrateRepeatingTextboxValues(source, renderingPagesRanges.StartPage, tableGroup.HeaderRepeatOnNewPage ? renderingPagesRanges.EndPage : renderingPagesRanges.StartPage);
					m_processingContext.PageSectionContext.PageTextboxes.IntegrateRepeatingTextboxValues(source2, tableGroup.FooterRepeatOnNewPage ? renderingPagesRanges.StartPage : renderingPagesRanges.EndPage, renderingPagesRanges.EndPage);
				}
				m_processingContext.Pagination.LeaveIgnorePageBreak(tableGroup.Visibility, ignoreAlways: false);
				instanceList.Add(m_tableGroupInstance);
				if (m_headerReportItemCols != null)
				{
					m_headerReportItemCols.ResetReportItemObjs();
				}
				if (m_footerReportItemCols != null)
				{
					m_footerReportItemCols.ResetReportItemObjs();
				}
				ResetReportItemsWithHideDuplicates();
			}

			protected override void AddToDocumentMap()
			{
				if (base.GroupingDef.GroupLabel != null && m_label != null)
				{
					m_processingContext.NavigationInfo.AddToDocumentMap(m_tableGroupInstance.UniqueName, isContainer: true, m_startPage, m_label);
				}
			}
		}

		private sealed class RuntimeTableDetailObj : RuntimeDetailObj, ISortDataHolder
		{
			private TableDetail m_detailDef;

			private RuntimeUserSortTargetInfo m_userSortTargetInfo;

			internal override ExpressionInfoList SortExpressions
			{
				get
				{
					Sorting sorting = ((Table)m_dataRegionDef).TableDetail.Sorting;
					if (sorting != null && 0 < sorting.SortExpressions.Count)
					{
						return sorting.SortExpressions;
					}
					return null;
				}
			}

			internal override SortingExprHost SortExpressionHost => ((Table)m_dataRegionDef).TableDetail.Sorting?.ExprHost;

			internal override BoolList SortDirections
			{
				get
				{
					Sorting sorting = ((Table)m_dataRegionDef).TableDetail.Sorting;
					if (sorting != null && 0 < sorting.SortDirections.Count)
					{
						return sorting.SortDirections;
					}
					return null;
				}
			}

			protected override BTreeNode SortTree
			{
				get
				{
					if (m_userSortTargetInfo != null)
					{
						return m_userSortTargetInfo.SortTree;
					}
					if (m_grouping != null)
					{
						return m_grouping.Tree;
					}
					return null;
				}
				set
				{
					if (m_userSortTargetInfo != null)
					{
						m_userSortTargetInfo.SortTree = value;
					}
					else if (m_grouping != null)
					{
						m_grouping.Tree = value;
					}
					else
					{
						Global.Tracer.Assert(condition: false);
					}
				}
			}

			protected override int ExpressionIndex => 0;

			protected override IntList SortFilterInfoIndices
			{
				get
				{
					if (m_userSortTargetInfo != null)
					{
						return m_userSortTargetInfo.SortFilterInfoIndices;
					}
					return null;
				}
			}

			internal RuntimeTableDetailObj(IScope outerScope, Table tableDef, ref DataActions dataAction, ProcessingContext processingContext)
				: base(outerScope, tableDef, dataAction, processingContext)
			{
				bool flag = false;
				m_detailDef = tableDef.TableDetail;
				if (m_detailDef.RunningValues != null && 0 < m_detailDef.RunningValues.Count)
				{
					flag = true;
				}
				if (!flag && m_detailDef.DetailRows != null)
				{
					for (int i = 0; i < m_detailDef.DetailRows.Count; i++)
					{
						if (m_detailDef.DetailRows[i].ReportItems.RunningValues != null && 0 < m_detailDef.DetailRows[i].ReportItems.RunningValues.Count)
						{
							flag = true;
							break;
						}
					}
				}
				if (flag)
				{
					m_rvValueList = new DataAggregateObjResultsList();
				}
				HandleSortFilterEvent(ref m_userSortTargetInfo);
				if (m_userSortTargetInfo == null && m_detailDef.Sorting != null && 0 < m_detailDef.Sorting.SortExpressions.Count)
				{
					m_expression = new RuntimeExpressionInfo(m_detailDef.Sorting.SortExpressions, m_detailDef.Sorting.ExprHost, m_detailDef.Sorting.SortDirections, 0);
					m_grouping = new RuntimeGroupingObj(this, RuntimeGroupingObj.GroupingTypes.Sort);
				}
				dataAction = DataActions.None;
			}

			internal RuntimeTableDetailObj(RuntimeTableDetailObj detailRoot)
				: base(detailRoot)
			{
				m_detailDef = detailRoot.m_detailDef;
			}

			void ISortDataHolder.NextRow()
			{
				NextRow();
			}

			void ISortDataHolder.Traverse(ProcessingStages operation)
			{
				switch (operation)
				{
				case ProcessingStages.SortAndFilter:
					SortAndFilter();
					break;
				case ProcessingStages.RunningValues:
					ReadRows(DataActions.PostSortAggregates);
					break;
				case ProcessingStages.CreatingInstances:
					CreateInstance();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}

			internal override void NextRow()
			{
				if (m_userSortTargetInfo != null)
				{
					ProcessDetailSort(m_userSortTargetInfo);
				}
				else
				{
					base.NextRow();
				}
			}

			internal override void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
				if (m_rvValueList == null && (m_outerDataAction & DataActions.PostSortAggregates) == 0)
				{
					return;
				}
				m_globalRunningValueCollection = globalRVCol;
				m_groupCollection = groupCol;
				if (m_detailDef != null)
				{
					AddRunningValues(m_detailDef.RunningValues, lastGroup);
					TableRowList detailRows = m_detailDef.DetailRows;
					if (detailRows != null)
					{
						for (int i = 0; i < detailRows.Count; i++)
						{
							RunningValueInfoList runningValues = null;
							if (detailRows[i].ReportItems != null)
							{
								runningValues = detailRows[i].ReportItems.RunningValues;
							}
							AddRunningValues(runningValues, lastGroup);
						}
					}
				}
				if (m_userSortTargetInfo != null)
				{
					bool sortDirection = m_processingContext.RuntimeSortFilterInfo[m_userSortTargetInfo.SortFilterInfoIndices[0]].SortDirection;
					m_userSortTargetInfo.SortTree.Traverse(ProcessingStages.RunningValues, sortDirection);
					m_userSortTargetInfo = null;
				}
				else
				{
					base.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
				}
			}

			internal override void CreateInstances(ReportItemInstance riInstance, IList instanceList, RenderingPagesRangesList pagesList)
			{
				if (m_userSortTargetInfo != null)
				{
					m_reportItemInstance = riInstance;
					m_instanceList = instanceList;
					m_pagesList = pagesList;
					bool sortDirection = m_processingContext.RuntimeSortFilterInfo[m_userSortTargetInfo.SortFilterInfoIndices[0]].SortDirection;
					m_userSortTargetInfo.SortTree.Traverse(ProcessingStages.CreatingInstances, sortDirection);
					m_userSortTargetInfo = null;
				}
				else
				{
					base.CreateInstances(riInstance, instanceList, pagesList);
				}
			}

			internal override void CreateInstance()
			{
				if (m_detailDef == null)
				{
					return;
				}
				TableRowList detailRows = m_detailDef.DetailRows;
				RuntimeTableDetailObj runtimeTableDetailObj = (RuntimeTableDetailObj)m_hierarchyRoot;
				TableInstance tableInstance = (TableInstance)runtimeTableDetailObj.m_reportItemInstance;
				Table table = (Table)tableInstance.ReportItemDef;
				IList instanceList = runtimeTableDetailObj.m_instanceList;
				if (m_dataRows == null)
				{
					return;
				}
				DataActions dataAction = DataActions.None;
				RuntimeRICollectionList runtimeRICollectionList = new RuntimeRICollectionList(detailRows.Count);
				for (int i = 0; i < detailRows.Count; i++)
				{
					runtimeRICollectionList.Add(new RuntimeRICollection(this, detailRows[i].ReportItems, ref dataAction, m_processingContext, createDataRegions: false));
				}
				m_processingContext.ChunkManager.EnterIgnorePageBreakItem();
				double detailHeightValue = -1.0;
				m_processingContext.Pagination.EnterIgnorePageBreak(detailRows[0].Visibility, ignoreAlways: false);
				for (int j = 0; j < m_dataRows.Count; j++)
				{
					int startIndex = 0;
					SetupEnvironment(j, ref startIndex, m_detailDef.RunningValues, rvOnly: false);
					TableDetailInstance tableDetailInstance = new TableDetailInstance(m_processingContext, m_detailDef, (Table)m_dataRegionDef);
					m_processingContext.Pagination.ProcessTableDetails(table, tableDetailInstance, instanceList, ref detailHeightValue, detailRows, runtimeTableDetailObj.m_pagesList, ref runtimeTableDetailObj.m_numberOfChildrenOnThisPage);
					tableInstance.CurrentPage = table.CurrentPage;
					tableInstance.NumberOfChildrenOnThisPage = runtimeTableDetailObj.m_numberOfChildrenOnThisPage;
					m_processingContext.Pagination.EnterIgnorePageBreak(null, ignoreAlways: true);
					m_processingContext.Pagination.EnterIgnoreHeight(startHidden: true);
					if (Report.ShowHideTypes.Interactive == m_processingContext.ShowHideType)
					{
						m_processingContext.EnterGrouping();
					}
					m_dataRegionDef.CurrentDetailRowIndex = j;
					for (int k = 0; k < detailRows.Count; k++)
					{
						ReportItemCollection reportItems = detailRows[k].ReportItems;
						SetupEnvironment(j, ref startIndex, reportItems.RunningValues, rvOnly: true);
						TableRowInstance tableRowInstance = tableDetailInstance.DetailRowInstances[k];
						if (Report.ShowHideTypes.Interactive == m_processingContext.ShowHideType)
						{
							((IShowHideContainer)tableRowInstance).BeginProcessContainer(m_processingContext);
						}
						m_processingContext.PageSectionContext.EnterVisibilityScope(detailRows[k].Visibility, detailRows[k].StartHidden);
						IntList tableColumnSpans = m_processingContext.PageSectionContext.TableColumnSpans;
						m_processingContext.PageSectionContext.TableColumnSpans = detailRows[k].ColSpans;
						runtimeRICollectionList[k].CreateInstances(tableRowInstance.TableRowReportItemColInstance);
						m_processingContext.PageSectionContext.TableColumnSpans = tableColumnSpans;
						m_processingContext.PageSectionContext.ExitVisibilityScope();
						if (Report.ShowHideTypes.Interactive == m_processingContext.ShowHideType)
						{
							((IShowHideContainer)tableRowInstance).EndProcessContainer(m_processingContext);
						}
					}
					if (Report.ShowHideTypes.Interactive == m_processingContext.ShowHideType)
					{
						m_processingContext.ExitGrouping();
					}
					m_processingContext.Pagination.LeaveIgnorePageBreak(null, ignoreAlways: true);
					m_processingContext.Pagination.LeaveIgnoreHeight(startHidden: true);
					m_processingContext.Pagination.LeaveIgnoreHeight(m_detailDef.StartHidden);
					instanceList.Add(tableDetailInstance);
					runtimeRICollectionList.ResetReportItemObjs();
				}
				m_processingContext.Pagination.LeaveIgnorePageBreak(detailRows[0].Visibility, ignoreAlways: false);
				m_processingContext.ChunkManager.LeaveIgnorePageBreakItem();
			}

			protected override IHierarchyObj CreateHierarchyObj()
			{
				if (m_userSortTargetInfo != null)
				{
					return new RuntimeSortHierarchyObj(this);
				}
				return base.CreateHierarchyObj();
			}

			protected override void ProcessUserSort()
			{
				m_processingContext.ProcessUserSortForTarget(this, ref m_dataRows, m_userSortTargetInfo.TargetForNonDetailSort);
			}

			protected override void MarkSortInfoProcessed(RuntimeSortFilterEventInfoList runtimeSortFilterInfo)
			{
				if (m_userSortTargetInfo != null)
				{
					m_userSortTargetInfo.MarkSortInfoProcessed(runtimeSortFilterInfo, this);
				}
			}

			protected override void AddSortInfoIndex(int sortInfoIndex, RuntimeSortFilterEventInfo sortInfo)
			{
				if (m_userSortTargetInfo != null)
				{
					m_userSortTargetInfo.AddSortInfoIndex(sortInfoIndex, sortInfo);
				}
			}
		}

		private sealed class RuntimeOnePassTableDetailObj : RuntimeOnePassDetailObj
		{
			private RuntimeRICollectionList m_reportItemCols;

			private TableDetailInstanceList m_tableDetailInstances;

			private Hashtable m_textboxColumnPositions;

			internal TableDetailInstanceList TableDetailInstances => m_tableDetailInstances;

			internal Hashtable TextboxColumnPositions => m_textboxColumnPositions;

			internal RuntimeOnePassTableDetailObj(IScope outerScope, Table tableDef, ProcessingContext processingContext)
				: base(outerScope, tableDef, processingContext)
			{
				TableDetail tableDetail = tableDef.TableDetail;
				if (tableDetail.RunningValues != null && 0 < tableDetail.RunningValues.Count)
				{
					AddRunningValues(tableDetail.RunningValues);
				}
				TableRowList detailRows = tableDetail.DetailRows;
				if (detailRows != null)
				{
					m_reportItemCols = new RuntimeRICollectionList(detailRows.Count);
					for (int i = 0; i < detailRows.Count; i++)
					{
						if (detailRows[i].ReportItems != null)
						{
							m_reportItemCols.Add(new RuntimeRICollection(this, detailRows[i].ReportItems, m_processingContext, createDataRegions: false));
							AddRunningValues(detailRows[i].ReportItems.RunningValues);
						}
					}
				}
				m_tableDetailInstances = new TableDetailInstanceList();
				m_textboxColumnPositions = new Hashtable();
				tableDef.CurrentPage = 0;
			}

			internal override int GetDetailPage()
			{
				return ((Table)m_dataRegionDef).CurrentPage;
			}

			protected override void CreateInstance()
			{
				Table table = (Table)m_dataRegionDef;
				TableRowList detailRows = table.TableDetail.DetailRows;
				double detailHeightValue = -1.0;
				Pagination pagination = m_processingContext.Pagination;
				m_pagination.CopyPaginationInfo(pagination);
				m_processingContext.Pagination = m_pagination;
				NavigationInfo navigationInfo = m_processingContext.NavigationInfo;
				m_processingContext.NavigationInfo = m_navigationInfo;
				TableDetailInstance tableDetailInstance = new TableDetailInstance(m_processingContext, table.TableDetail, table);
				if (table.Visibility != null && table.Visibility.Toggle != null)
				{
					m_processingContext.Pagination.EnterIgnoreHeight(startHidden: true);
				}
				m_processingContext.Pagination.ProcessTableDetails(table, tableDetailInstance, m_tableDetailInstances, ref detailHeightValue, detailRows, m_renderingPages, ref m_numberOfContentsOnThisPage);
				if (table.Visibility != null && table.Visibility.Toggle != null)
				{
					m_processingContext.Pagination.LeaveIgnoreHeight(startHidden: true);
				}
				if (m_processingContext.ReportObjectModel.FieldsImpl.AddRowIndex)
				{
					m_dataRegionDef.CurrentDetailRowIndex = m_processingContext.ReportObjectModel.FieldsImpl.GetRowIndex();
				}
				RuntimeTableObj.CreateRowInstances(m_processingContext, m_reportItemCols, tableDetailInstance.DetailRowInstances, repeatOnNewPages: false, enterGrouping: true);
				m_processingContext.Pagination = pagination;
				m_processingContext.NavigationInfo = navigationInfo;
				m_tableDetailInstances.Add(tableDetailInstance);
			}

			internal override bool IsVisible(string textboxName)
			{
				Global.Tracer.Assert(m_textboxColumnPositions != null, "(null != m_textboxColumnPositions)");
				object obj = m_textboxColumnPositions[textboxName];
				if (obj != null)
				{
					return m_processingContext.PageSectionContext.IsTableColumnVisible((TableColumnInfo)obj);
				}
				return false;
			}
		}

		private sealed class RuntimeOWCChartDetailObj : RuntimeDetailObj, IFilterOwner
		{
			private Filters m_filters;

			private DataAggregateObjList m_nonCustomAggregates;

			private DataAggregateObjList m_customAggregates;

			private DataAggregateObjList m_postSortAggregates;

			private DataAggregateObjList m_runningValues;

			private DataAggregateObjResult[] m_runningValueValues;

			protected override string ScopeName => m_dataRegionDef.Name;

			internal RuntimeOWCChartDetailObj(IScope outerScope, OWCChart chartDef, ref DataActions dataAction, ProcessingContext processingContext)
				: base(outerScope, chartDef, (chartDef.Filters == null) ? dataAction : DataActions.None, processingContext)
			{
				RuntimeDataRegionObj.CreateAggregates(m_processingContext, chartDef.Aggregates, ref m_nonCustomAggregates, ref m_customAggregates);
				RuntimeDataRegionObj.CreateAggregates(m_processingContext, chartDef.PostSortAggregates, ref m_postSortAggregates);
				if (chartDef.Filters != null)
				{
					m_filters = new Filters(Filters.FilterTypes.DataRegionFilter, this, chartDef.Filters, chartDef.ObjectType, chartDef.Name, m_processingContext);
				}
				else
				{
					dataAction = DataActions.None;
				}
				RunningValueInfoList detailRunningValues = chartDef.DetailRunningValues;
				if (detailRunningValues != null && 0 < detailRunningValues.Count)
				{
					m_rvValueList = new DataAggregateObjResultsList();
				}
			}

			internal RuntimeOWCChartDetailObj(RuntimeOWCChartDetailObj detailRoot)
				: base(detailRoot)
			{
			}

			internal override void NextRow()
			{
				if (m_processingContext.ReportObjectModel.FieldsImpl.IsAggregateRow)
				{
					NextAggregateRow();
				}
				else
				{
					NextNonAggregateRow();
				}
			}

			private void NextNonAggregateRow()
			{
				bool flag = true;
				if (m_filters != null)
				{
					flag = m_filters.PassFilters(m_processingContext.ReportObjectModel.FieldsImpl.GetFields());
				}
				if (flag)
				{
					((IFilterOwner)this).PostFilterNextRow();
				}
			}

			private void NextAggregateRow()
			{
				if (m_processingContext.ReportObjectModel.FieldsImpl.AggregationFieldCount == 0)
				{
					RuntimeDataRegionObj.UpdateAggregates(m_processingContext, m_customAggregates, updateAndSetup: false);
				}
			}

			void IFilterOwner.PostFilterNextRow()
			{
				RuntimeDataRegionObj.UpdateAggregates(m_processingContext, m_nonCustomAggregates, updateAndSetup: false);
				base.NextRow();
			}

			internal override void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
				OWCChart oWCChart = (OWCChart)m_dataRegionDef;
				RunningValueInfoList runningValues = oWCChart.RunningValues;
				if (m_rvValueList != null || m_postSortAggregates != null || (m_outerDataAction & DataActions.PostSortAggregates) != 0 || (runningValues != null && runningValues.Count != 0))
				{
					m_globalRunningValueCollection = globalRVCol;
					m_groupCollection = groupCol;
					if (runningValues != null)
					{
						RuntimeDetailObj.AddRunningValues(m_processingContext, runningValues, ref m_runningValues, globalRVCol, groupCol, lastGroup);
					}
					runningValues = oWCChart.DetailRunningValues;
					if (runningValues != null && 0 < runningValues.Count)
					{
						AddRunningValues(runningValues, lastGroup);
					}
					base.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
					RuntimeRICollection.DoneReadingRows(globalRVCol, oWCChart.RunningValues, ref m_runningValueValues, processPreviousAggregates: false);
				}
			}

			internal override void ReadRow(DataActions dataAction)
			{
				if (DataActions.PostSortAggregates == dataAction)
				{
					if (m_postSortAggregates != null)
					{
						RuntimeDataRegionObj.UpdateAggregates(m_processingContext, m_postSortAggregates, updateAndSetup: false);
					}
					if (m_runningValues != null)
					{
						for (int i = 0; i < m_runningValues.Count; i++)
						{
							m_runningValues[i].Update();
						}
					}
				}
				base.ReadRow(dataAction);
			}

			internal override void CreateInstance()
			{
				if (m_dataRows == null)
				{
					return;
				}
				OWCChart oWCChart = (OWCChart)m_dataRegionDef;
				OWCChartInstance oWCChartInstance = (OWCChartInstance)((RuntimeOWCChartDetailObj)m_hierarchyRoot).m_reportItemInstance;
				for (int i = 0; i < m_dataRows.Count; i++)
				{
					SetupEnvironment(i, oWCChart.RunningValues);
					for (int num = oWCChart.ChartData.Count - 1; num >= 0; num--)
					{
						oWCChartInstance.InstanceInfo.ChartData[num].Add(m_processingContext.ReportRuntime.EvaluateOWCChartData(oWCChart, oWCChart.ChartData[num].Value));
					}
				}
			}

			internal override void SetupEnvironment()
			{
				SetupEnvironment(m_nonCustomAggregates, m_customAggregates, (m_dataRows == null) ? null : m_dataRows[0]);
				SetupAggregates(m_postSortAggregates);
				SetupRunningValues(((OWCChart)m_dataRegionDef).RunningValues, m_runningValueValues);
			}

			internal override bool InScope(string scope)
			{
				return DataRegionInScope(m_dataRegionDef, scope);
			}

			protected override void GetGroupNameValuePairs(Dictionary<string, object> pairs)
			{
				DataRegionGetGroupNameValuePairs(m_dataRegionDef, pairs);
			}
		}

		internal abstract class RuntimePivotObj : RuntimeRDLDataRegionObj
		{
			protected Pivot m_pivotDef;

			protected RuntimePivotHeadingsObj m_pivotRows;

			protected RuntimePivotHeadingsObj m_pivotColumns;

			protected RuntimePivotHeadingsObj m_outerGroupings;

			protected RuntimePivotHeadingsObj m_innerGroupings;

			protected int[] m_outerGroupingCounters;

			protected override string ScopeName => m_pivotDef.Name;

			protected override DataRegion DataRegionDef => m_pivotDef;

			internal int[] OuterGroupingCounters => m_outerGroupingCounters;

			internal RuntimePivotObj(IScope outerScope, Pivot pivotDef, ref DataActions dataAction, ProcessingContext processingContext, bool onePassProcess)
				: base(outerScope, pivotDef, ref dataAction, processingContext, onePassProcess, pivotDef.RunningValues)
			{
				m_pivotDef = pivotDef;
			}

			protected void ConstructorHelper(ref DataActions dataAction, bool onePassProcess, out bool handleMyDataAction, out DataActions innerDataAction, out PivotHeading outermostColumn, out bool outermostColumnSubtotal, out PivotHeading staticColumn, out PivotHeading outermostRow, out bool outermostRowSubtotal, out PivotHeading staticRow)
			{
				m_pivotDef.GetHeadingDefState(out outermostColumn, out outermostColumnSubtotal, out staticColumn, out outermostRow, out outermostRowSubtotal, out staticRow);
				innerDataAction = m_dataAction;
				handleMyDataAction = false;
				bool flag = false;
				if (onePassProcess)
				{
					flag = true;
					RuntimeDataRegionObj.CreateAggregates(m_processingContext, m_pivotDef.PostSortAggregates, ref m_nonCustomAggregates);
					Global.Tracer.Assert(outermostRow == null && outermostColumn == null, "((null == outermostRow) && (null == outermostColumn))");
					RuntimeDataRegionObj.CreateAggregates(m_processingContext, m_pivotDef.CellPostSortAggregates, ref m_nonCustomAggregates);
				}
				else
				{
					if (m_pivotDef.PostSortAggregates != null)
					{
						RuntimeDataRegionObj.CreateAggregates(m_processingContext, m_pivotDef.PostSortAggregates, ref m_postSortAggregates);
						handleMyDataAction = true;
					}
					if ((outermostRowSubtotal & outermostColumnSubtotal) || (outermostRow == null && outermostColumn == null))
					{
						flag = true;
						if (m_pivotDef.CellPostSortAggregates != null)
						{
							RuntimeDataRegionObj.CreateAggregates(m_processingContext, m_pivotDef.CellPostSortAggregates, ref m_postSortAggregates);
							handleMyDataAction = true;
						}
					}
					if (handleMyDataAction)
					{
						m_dataAction |= DataActions.PostSortAggregates;
						innerDataAction = DataActions.None;
					}
					else
					{
						innerDataAction = m_dataAction;
					}
				}
				if (flag)
				{
					RuntimeDataRegionObj.CreateAggregates(m_processingContext, m_pivotDef.CellAggregates, ref m_nonCustomAggregates, ref m_customAggregates);
					RunningValueInfoList pivotCellRunningValues = m_pivotDef.PivotCellRunningValues;
					if (pivotCellRunningValues != null && 0 < pivotCellRunningValues.Count)
					{
						if (m_nonCustomAggregates == null)
						{
							m_nonCustomAggregates = new DataAggregateObjList();
						}
						for (int i = 0; i < pivotCellRunningValues.Count; i++)
						{
							m_nonCustomAggregates.Add(new DataAggregateObj(pivotCellRunningValues[i], m_processingContext));
						}
					}
				}
				int num = m_pivotDef.CreateOuterGroupingIndexList();
				m_outerGroupingCounters = new int[num];
				for (int j = 0; j < m_outerGroupingCounters.Length; j++)
				{
					m_outerGroupingCounters[j] = -1;
				}
			}

			protected void HandleDataAction(bool handleMyDataAction, DataActions innerDataAction, DataActions userSortDataAction)
			{
				if (!handleMyDataAction)
				{
					m_dataAction = innerDataAction;
				}
				m_dataAction |= userSortDataAction;
				if (m_dataAction != 0)
				{
					m_dataRows = new DataRowList();
				}
			}

			protected override void SendToInner()
			{
				m_pivotDef.RuntimeDataRegionObj = this;
				m_pivotDef.ResetOutergGroupingAggregateRowInfo();
				m_pivotDef.SavePivotAggregateRowInfo(m_processingContext);
				if (m_outerGroupings != null)
				{
					m_outerGroupings.NextRow();
				}
				m_pivotDef.RestorePivotAggregateRowInfo(m_processingContext);
				if (m_innerGroupings != null)
				{
					m_innerGroupings.NextRow();
				}
			}

			internal override bool SortAndFilter()
			{
				if (m_pivotRows != null)
				{
					m_pivotRows.SortAndFilter();
				}
				if (m_pivotColumns != null)
				{
					m_pivotColumns.SortAndFilter();
				}
				return base.SortAndFilter();
			}

			internal override void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
				if (m_pivotDef.RunningValues != null && m_runningValues == null)
				{
					RuntimeDetailObj.AddRunningValues(m_processingContext, m_pivotDef.RunningValues, ref m_runningValues, globalRVCol, groupCol, lastGroup);
				}
				if (m_dataRows != null)
				{
					ReadRows(DataActions.PostSortAggregates);
					m_dataRows = null;
				}
				if (m_outerGroupings != null)
				{
					m_outerGroupings.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
				}
				if ((m_outerGroupings == null || m_outerGroupings.Headings == null) && m_innerGroupings != null)
				{
					m_innerGroupings.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
				}
			}

			protected virtual void CalculatePreviousAggregates()
			{
				if (!m_processedPreviousAggregates && m_processingContext.GlobalRVCollection != null)
				{
					Global.Tracer.Assert(m_runningValueValues == null, "(null == m_runningValueValues)");
					AggregatesImpl globalRVCollection = m_processingContext.GlobalRVCollection;
					RunningValueInfoList runningValues = m_pivotDef.RunningValues;
					RuntimeRICollection.DoneReadingRows(globalRVCollection, runningValues, ref m_runningValueValues, processPreviousAggregates: true);
					if (m_pivotRows != null)
					{
						m_pivotRows.CalculatePreviousAggregates(globalRVCollection);
					}
					if (m_pivotColumns != null)
					{
						m_pivotColumns.CalculatePreviousAggregates(globalRVCollection);
					}
					m_processedPreviousAggregates = true;
				}
			}

			internal override void ReadRow(DataActions dataAction)
			{
				if (DataActions.UserSort == dataAction)
				{
					RuntimeDataRegionObj.CommonFirstRow(m_processingContext.ReportObjectModel.FieldsImpl, ref m_firstRowIsAggregate, ref m_firstRow);
					CommonNextRow(m_dataRows);
				}
				else
				{
					if (m_pivotDef.ProcessCellRunningValues)
					{
						return;
					}
					if (DataActions.PostSortAggregates == dataAction)
					{
						if (m_postSortAggregates != null)
						{
							RuntimeDataRegionObj.UpdateAggregates(m_processingContext, m_postSortAggregates, updateAndSetup: false);
						}
						if (m_runningValues != null)
						{
							for (int i = 0; i < m_runningValues.Count; i++)
							{
								m_runningValues[i].Update();
							}
						}
						CalculatePreviousAggregates();
					}
					if (m_outerScope != null && (dataAction & m_outerDataAction) != 0)
					{
						m_outerScope.ReadRow(dataAction);
					}
				}
			}

			internal override void SetupEnvironment()
			{
				SetupEnvironment(m_pivotDef.RunningValues);
			}
		}

		internal abstract class RuntimePivotHeadingsObj
		{
			protected IScope m_owner;

			protected RuntimePivotGroupRootObj m_pivotHeadings;

			protected PivotHeading m_staticHeadingDef;

			internal RuntimePivotGroupRootObj Headings => m_pivotHeadings;

			internal RuntimePivotHeadingsObj(IScope owner, PivotHeading headingDef, ref DataActions dataAction, ProcessingContext processingContext, PivotHeading staticHeadingDef, RuntimePivotHeadingsObj innerGroupings, bool outermostHeadingSubtotal, int headingLevel)
			{
				m_owner = owner;
				if (staticHeadingDef != null)
				{
					m_staticHeadingDef = staticHeadingDef;
				}
			}

			internal virtual void NextRow()
			{
				if (m_pivotHeadings != null)
				{
					m_pivotHeadings.NextRow();
				}
			}

			internal virtual bool SortAndFilter()
			{
				if (m_pivotHeadings != null)
				{
					return m_pivotHeadings.SortAndFilter();
				}
				return true;
			}

			internal virtual void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
				if (m_pivotHeadings != null)
				{
					m_pivotHeadings.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
				}
			}

			internal abstract void CalculatePreviousAggregates(AggregatesImpl globalRVCol);
		}

		private sealed class RuntimeChartObj : RuntimePivotObj
		{
			private bool m_subtotalCorner;

			internal RuntimeChartObj(IScope outerScope, Chart chartDef, ref DataActions dataAction, ProcessingContext processingContext, bool onePassProcess)
				: base(outerScope, chartDef, ref dataAction, processingContext, onePassProcess)
			{
				ConstructorHelper(ref dataAction, onePassProcess, out bool handleMyDataAction, out DataActions innerDataAction, out PivotHeading outermostColumn, out bool outermostColumnSubtotal, out PivotHeading staticColumn, out PivotHeading outermostRow, out bool outermostRowSubtotal, out PivotHeading staticRow);
				m_innerDataAction = innerDataAction;
				DataActions userSortDataAction = HandleSortFilterEvent();
				ChartConstructRuntimeStructure(ref innerDataAction, onePassProcess, outermostColumn, outermostColumnSubtotal, staticColumn, outermostRow, outermostRowSubtotal, staticRow);
				if (onePassProcess || (outermostRowSubtotal && outermostColumnSubtotal) || (outermostRow == null && outermostColumn == null))
				{
					m_subtotalCorner = true;
				}
				HandleDataAction(handleMyDataAction, innerDataAction, userSortDataAction);
			}

			protected override void ConstructRuntimeStructure(ref DataActions innerDataAction)
			{
				m_pivotDef.GetHeadingDefState(out PivotHeading outermostColumn, out bool outermostColumnSubtotal, out PivotHeading staticColumn, out PivotHeading outermostRow, out bool outermostRowSubtotal, out PivotHeading staticRow);
				ChartConstructRuntimeStructure(ref innerDataAction, onePassProcess: false, outermostColumn, outermostColumnSubtotal, staticColumn, outermostRow, outermostRowSubtotal, staticRow);
			}

			private void ChartConstructRuntimeStructure(ref DataActions innerDataAction, bool onePassProcess, PivotHeading outermostColumn, bool outermostColumnSubtotal, PivotHeading staticColumn, PivotHeading outermostRow, bool outermostRowSubtotal, PivotHeading staticRow)
			{
				DataActions dataAction = DataActions.None;
				if (m_pivotDef.ProcessingInnerGrouping == Pivot.ProcessingInnerGroupings.Column)
				{
					m_innerGroupings = (m_pivotColumns = new RuntimeChartHeadingsObj(this, (ChartHeading)outermostColumn, ref dataAction, m_processingContext, (ChartHeading)staticColumn, null, outermostRowSubtotal, 0));
					m_outerGroupings = (m_pivotRows = new RuntimeChartHeadingsObj(this, (ChartHeading)outermostRow, ref innerDataAction, m_processingContext, (ChartHeading)staticRow, (RuntimeChartHeadingsObj)m_innerGroupings, outermostColumnSubtotal, 0));
				}
				else
				{
					m_innerGroupings = (m_pivotRows = new RuntimeChartHeadingsObj(this, (ChartHeading)outermostRow, ref dataAction, m_processingContext, (ChartHeading)staticRow, null, outermostColumnSubtotal, 0));
					m_outerGroupings = (m_pivotColumns = new RuntimeChartHeadingsObj(this, (ChartHeading)outermostColumn, ref innerDataAction, m_processingContext, (ChartHeading)staticColumn, (RuntimeChartHeadingsObj)m_innerGroupings, outermostRowSubtotal, 0));
				}
			}

			internal override bool SortAndFilter()
			{
				SetupEnvironment();
				if (m_userSortTargetInfo != null)
				{
					m_userSortTargetInfo.EnterProcessUserSortPhase(m_processingContext);
				}
				base.SortAndFilter();
				if (m_userSortTargetInfo != null)
				{
					m_userSortTargetInfo.LeaveProcessUserSortPhase(m_processingContext);
				}
				return true;
			}

			internal override void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
				base.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
				RuntimeRICollection.DoneReadingRows(globalRVCol, m_pivotDef.RunningValues, ref m_runningValueValues, processPreviousAggregates: false);
			}

			internal override void CreateInstances(ReportItemInstance riInstance, IList instanceList, RenderingPagesRangesList pagesList)
			{
				if (m_firstRow != null)
				{
					_ = (Chart)m_pivotDef;
					ChartInstance chartInstance = (ChartInstance)riInstance;
					if (m_outerGroupings == m_pivotRows)
					{
						chartInstance.InnerHeadingInstanceList = chartInstance.ColumnInstances;
						((RuntimeChartHeadingsObj)m_outerGroupings).CreateInstances(this, m_processingContext, chartInstance, outerGroupings: true, null, chartInstance.RowInstances);
					}
					else
					{
						chartInstance.InnerHeadingInstanceList = chartInstance.RowInstances;
						((RuntimeChartHeadingsObj)m_outerGroupings).CreateInstances(this, m_processingContext, chartInstance, outerGroupings: true, null, chartInstance.ColumnInstances);
					}
				}
			}

			internal void CreateOutermostSubtotalCells(ChartInstance chartInstance, bool outerGroupings)
			{
				if (outerGroupings)
				{
					SetupEnvironment();
					((RuntimeChartHeadingsObj)m_innerGroupings).CreateInstances(this, m_processingContext, chartInstance, outerGroupings: false, null, chartInstance.InnerHeadingInstanceList);
				}
				else if (m_subtotalCorner)
				{
					SetupEnvironment();
					chartInstance.AddCell(m_processingContext, -1);
				}
			}
		}

		private sealed class RuntimeChartHeadingsObj : RuntimePivotHeadingsObj
		{
			private DataAggregateObjResult[] m_runningValueValues;

			internal RuntimeChartHeadingsObj(IScope owner, ChartHeading headingDef, ref DataActions dataAction, ProcessingContext processingContext, ChartHeading staticHeadingDef, RuntimeChartHeadingsObj innerGroupings, bool outermostHeadingSubtotal, int headingLevel)
				: base(owner, headingDef, ref dataAction, processingContext, staticHeadingDef, innerGroupings, outermostHeadingSubtotal, headingLevel)
			{
				if (headingDef != null)
				{
					m_pivotHeadings = new RuntimeChartGroupRootObj(owner, headingDef, ref dataAction, processingContext, innerGroupings, outermostHeadingSubtotal, headingLevel);
				}
			}

			internal override void CalculatePreviousAggregates(AggregatesImpl globalRVCol)
			{
				if (m_staticHeadingDef != null)
				{
					RuntimeRICollection.DoneReadingRows(globalRVCol, ((ChartHeading)m_staticHeadingDef).RunningValues, ref m_runningValueValues, processPreviousAggregates: true);
				}
			}

			internal override void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
				base.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
				if (m_staticHeadingDef != null && m_owner is RuntimeChartGroupLeafObj)
				{
					RuntimeRICollection.DoneReadingRows(globalRVCol, ((ChartHeading)m_staticHeadingDef).RunningValues, ref m_runningValueValues, processPreviousAggregates: false);
				}
			}

			private void SetupEnvironment(ProcessingContext processingContext)
			{
				if (m_staticHeadingDef != null && m_runningValueValues != null)
				{
					RuntimeDataRegionObj.SetupRunningValues(processingContext, ((ChartHeading)m_staticHeadingDef).RunningValues, m_runningValueValues);
				}
			}

			internal void CreateInstances(RuntimeDataRegionObj outerGroup, ProcessingContext processingContext, ChartInstance chartInstance, bool outerGroupings, RuntimePivotGroupRootObj currOuterHeadingGroupRoot, ChartHeadingInstanceList headingInstances)
			{
				bool flag = outerGroupings || chartInstance.CurrentCellOuterIndex == 0;
				int num = 1;
				SetupEnvironment(processingContext);
				if (m_staticHeadingDef != null && ((ChartHeading)m_staticHeadingDef).Labels != null)
				{
					num = ((ChartHeading)m_staticHeadingDef).Labels.Count;
				}
				ChartHeadingInstanceList chartHeadingInstanceList = headingInstances;
				for (int i = 0; i < num; i++)
				{
					if (m_staticHeadingDef != null)
					{
						if (flag)
						{
							chartHeadingInstanceList = CreateHeadingInstance(processingContext, chartInstance, (ChartHeading)m_staticHeadingDef, headingInstances, outerGroupings, i);
						}
						if (outerGroupings)
						{
							chartInstance.CurrentOuterStaticIndex = i;
						}
						else
						{
							chartInstance.CurrentInnerStaticIndex = i;
						}
					}
					if (m_pivotHeadings != null)
					{
						((Chart)m_pivotHeadings.HierarchyDef.DataRegionDef).CurrentOuterHeadingGroupRoot = currOuterHeadingGroupRoot;
						m_pivotHeadings.CreateInstances(chartInstance, chartHeadingInstanceList, null);
						if (flag)
						{
							SetHeadingSpan(chartInstance, chartHeadingInstanceList, outerGroupings, processingContext);
						}
					}
					else if (outerGroup is RuntimeChartGroupLeafObj)
					{
						RuntimeChartGroupLeafObj runtimeChartGroupLeafObj = (RuntimeChartGroupLeafObj)outerGroup;
						if (!outerGroupings && runtimeChartGroupLeafObj.IsOuterGrouping())
						{
							runtimeChartGroupLeafObj.CreateSubtotalOrStaticCells(chartInstance, currOuterHeadingGroupRoot, outerGroupings);
						}
						else
						{
							runtimeChartGroupLeafObj.CreateInnerGroupingsOrCells(chartInstance, currOuterHeadingGroupRoot);
						}
					}
					else
					{
						((RuntimeChartObj)outerGroup).CreateOutermostSubtotalCells(chartInstance, outerGroupings);
					}
				}
				if (m_staticHeadingDef != null && flag)
				{
					SetHeadingSpan(chartInstance, headingInstances, outerGroupings, processingContext);
				}
			}

			private void SetHeadingSpan(ChartInstance chartInstance, ChartHeadingInstanceList headingInstances, bool outerGroupings, ProcessingContext processingContext)
			{
				int currentCellIndex = (!outerGroupings) ? chartInstance.CurrentCellInnerIndex : (chartInstance.CurrentCellOuterIndex + 1);
				headingInstances.SetLastHeadingSpan(currentCellIndex, processingContext);
			}

			private ChartHeadingInstanceList CreateHeadingInstance(ProcessingContext processingContext, ChartInstance chartInstance, ChartHeading headingDef, ChartHeadingInstanceList headingInstances, bool outerGroupings, int labelIndex)
			{
				ChartHeadingInstance chartHeadingInstance = null;
				int headingCellIndex;
				if (outerGroupings)
				{
					chartInstance.NewOuterCells();
					headingCellIndex = chartInstance.CurrentCellOuterIndex;
				}
				else
				{
					headingCellIndex = chartInstance.CurrentCellInnerIndex;
				}
				chartHeadingInstance = new ChartHeadingInstance(processingContext, headingCellIndex, headingDef, labelIndex, null);
				headingInstances.Add(chartHeadingInstance, processingContext);
				return chartHeadingInstance.SubHeadingInstances;
			}
		}

		private sealed class RuntimeChartGroupRootObj : RuntimePivotGroupRootObj
		{
			internal RuntimeChartGroupRootObj(IScope outerScope, ChartHeading chartHeadingDef, ref DataActions dataAction, ProcessingContext processingContext, RuntimeChartHeadingsObj innerGroupings, bool outermostSubtotal, int headingLevel)
				: base(outerScope, chartHeadingDef, ref dataAction, processingContext, innerGroupings, outermostSubtotal, headingLevel)
			{
				if (m_processOutermostSTCells)
				{
					Chart chart = (Chart)chartHeadingDef.DataRegionDef;
					if (chart.CellRunningValues != null && 0 < chart.CellRunningValues.Count)
					{
						m_dataAction |= DataActions.PostSortAggregates;
					}
				}
				if (chartHeadingDef.ChartGroupExpression)
				{
					m_saveGroupExprValues = true;
				}
			}

			protected override void NeedProcessDataActions(PivotHeading heading)
			{
				ChartHeading chartHeading = (ChartHeading)heading;
				if (chartHeading != null)
				{
					NeedProcessDataActions(chartHeading.RunningValues);
				}
			}

			internal override void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
				base.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
				AddRunningValues(((ChartHeading)m_hierarchyDef).RunningValues);
				if (m_staticHeadingDef != null)
				{
					AddRunningValues(((ChartHeading)m_staticHeadingDef).RunningValues);
				}
				m_grouping.Traverse(ProcessingStages.RunningValues, m_expression.Direction);
				if (m_hierarchyDef.Grouping.Name != null)
				{
					groupCol.Remove(m_hierarchyDef.Grouping.Name);
				}
			}

			protected override void AddCellRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, ref DataAggregateObjList runningValues)
			{
				Chart chart = (Chart)m_hierarchyDef.DataRegionDef;
				if (chart.CellRunningValues != null && 0 < chart.CellRunningValues.Count)
				{
					m_dataAction |= DataActions.PostSortAggregates;
					if (runningValues == null)
					{
						AddRunningValues(chart.CellRunningValues, ref runningValues, globalRVCol, groupCol);
					}
				}
			}
		}

		private sealed class RuntimeChartCell : RuntimePivotCell
		{
			private DataAggregateObjResult[] m_runningValueValues;

			internal RuntimeChartCell(RuntimeChartGroupLeafObj owner, int cellLevel, DataAggregateInfoList aggDefs, ChartDataPointList cellDef, bool innermost)
				: base(owner, cellLevel, aggDefs, innermost)
			{
				Chart chart = (Chart)owner.PivotDef;
				DataActions dataActions = DataActions.None;
				bool flag = chart.CellRunningValues != null && 0 < chart.CellRunningValues.Count;
				if (m_innermost && (flag || m_owner.CellPostSortAggregates != null))
				{
					dataActions = DataActions.PostSortAggregates;
				}
				if (dataActions != 0)
				{
					m_dataRows = new DataRowList();
				}
			}

			internal override void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
				base.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
				RuntimeRICollection.DoneReadingRows(globalRVCol, ((Chart)m_owner.PivotDef).CellRunningValues, ref m_runningValueValues, processPreviousAggregates: false);
			}

			internal void CreateInstance(ChartInstance chartInstance)
			{
				int currentCellDPIndex = chartInstance.GetCurrentCellDPIndex();
				SetupEnvironment();
				RuntimeDataRegionObj.SetupRunningValues(m_owner.ProcessingContext, ((Chart)m_owner.PivotDef).CellRunningValues, m_runningValueValues);
				chartInstance.AddCell(m_owner.ProcessingContext, currentCellDPIndex);
			}
		}

		private sealed class RuntimeChartGroupLeafObj : RuntimePivotGroupLeafObj
		{
			private DataAggregateObjResult[] m_runningValueValues;

			private DataAggregateObjResult[] m_cellRunningValueValues;

			internal RuntimeChartGroupLeafObj(RuntimeChartGroupRootObj groupRoot)
				: base(groupRoot)
			{
				Chart pivotDef = (Chart)((ChartHeading)groupRoot.HierarchyDef).DataRegionDef;
				ChartHeading headingDef = (ChartHeading)groupRoot.InnerHeading;
				bool handleMyDataAction = false;
				bool num = HandleSortFilterEvent();
				ConstructorHelper(groupRoot, pivotDef, out handleMyDataAction, out DataActions innerDataAction);
				m_pivotHeadings = new RuntimeChartHeadingsObj(this, headingDef, ref innerDataAction, groupRoot.ProcessingContext, (ChartHeading)groupRoot.StaticHeadingDef, (RuntimeChartHeadingsObj)groupRoot.InnerGroupings, groupRoot.OutermostSubtotal, groupRoot.HeadingLevel + 1);
				m_innerHierarchy = m_pivotHeadings.Headings;
				if (!handleMyDataAction)
				{
					m_dataAction = innerDataAction;
				}
				if (num)
				{
					m_dataAction |= DataActions.UserSort;
				}
				if (m_dataAction != 0)
				{
					m_dataRows = new DataRowList();
				}
			}

			internal override RuntimePivotCell CreateCell(int index, Pivot pivotDef)
			{
				return new RuntimeChartCell(this, index, pivotDef.CellAggregates, ((Chart)pivotDef).ChartDataPoints, m_innerHierarchy == null);
			}

			internal override bool SortAndFilter()
			{
				SetupEnvironment();
				if (m_userSortTargetInfo != null)
				{
					m_userSortTargetInfo.EnterProcessUserSortPhase(m_processingContext);
				}
				bool result = base.SortAndFilter();
				if (m_userSortTargetInfo != null)
				{
					m_userSortTargetInfo.LeaveProcessUserSortPhase(m_processingContext);
				}
				return result;
			}

			internal override void CalculateRunningValues()
			{
				base.CalculateRunningValues();
				if (m_processHeading)
				{
					RuntimePivotGroupRootObj runtimePivotGroupRootObj = (RuntimePivotGroupRootObj)m_hierarchyRoot;
					AggregatesImpl globalRunningValueCollection = runtimePivotGroupRootObj.GlobalRunningValueCollection;
					_ = runtimePivotGroupRootObj.GroupCollection;
					RuntimeRICollection.DoneReadingRows(globalRunningValueCollection, ((ChartHeading)runtimePivotGroupRootObj.HierarchyDef).RunningValues, ref m_runningValueValues, processPreviousAggregates: false);
					if (runtimePivotGroupRootObj.ProcessOutermostSTCells)
					{
						RuntimeRICollection.DoneReadingRows(runtimePivotGroupRootObj.OutermostSTCellRVCol, ((Chart)base.PivotDef).CellRunningValues, ref m_cellRunningValueValues, processPreviousAggregates: false);
					}
					m_processHeading = false;
				}
				ResetScopedRunningValues();
			}

			internal override void CreateInstance()
			{
				SetupEnvironment();
				RuntimeChartGroupRootObj runtimeChartGroupRootObj = (RuntimeChartGroupRootObj)m_hierarchyRoot;
				Chart chart = (Chart)base.PivotDef;
				ChartInstance chartInstance = (ChartInstance)runtimeChartGroupRootObj.ReportItemInstance;
				ChartHeadingInstanceList chartHeadingInstanceList = (ChartHeadingInstanceList)runtimeChartGroupRootObj.InstanceList;
				ChartHeading chartHeading = (ChartHeading)runtimeChartGroupRootObj.HierarchyDef;
				bool flag = IsOuterGrouping();
				SetupRunningValues(chartHeading.RunningValues, m_runningValueValues);
				if (m_cellRunningValueValues != null)
				{
					SetupRunningValues(chart.CellRunningValues, m_cellRunningValueValues);
				}
				RuntimePivotGroupRootObj currOuterHeadingGroupRoot;
				int headingCellIndex;
				if (flag)
				{
					currOuterHeadingGroupRoot = (chart.CurrentOuterHeadingGroupRoot = runtimeChartGroupRootObj);
					chart.OuterGroupingIndexes[runtimeChartGroupRootObj.HeadingLevel] = m_groupLeafIndex;
					chartInstance.NewOuterCells();
					headingCellIndex = chartInstance.CurrentCellOuterIndex;
				}
				else
				{
					currOuterHeadingGroupRoot = chart.CurrentOuterHeadingGroupRoot;
					headingCellIndex = chartInstance.CurrentCellInnerIndex;
				}
				if (flag || chartInstance.CurrentCellOuterIndex == 0)
				{
					ChartHeadingInstance chartHeadingInstance = new ChartHeadingInstance(m_processingContext, headingCellIndex, chartHeading, 0, m_groupExprValues);
					chartHeadingInstanceList.Add(chartHeadingInstance, m_processingContext);
					chartHeadingInstanceList = chartHeadingInstance.SubHeadingInstances;
				}
				((RuntimeChartHeadingsObj)m_pivotHeadings).CreateInstances(this, m_processingContext, chartInstance, flag, currOuterHeadingGroupRoot, chartHeadingInstanceList);
			}

			internal void CreateInnerGroupingsOrCells(ChartInstance chartInstance, RuntimePivotGroupRootObj currOuterHeadingGroupRoot)
			{
				SetupEnvironment();
				if (IsOuterGrouping())
				{
					((RuntimeChartHeadingsObj)((RuntimeChartGroupRootObj)m_hierarchyRoot).InnerGroupings).CreateInstances(this, m_processingContext, chartInstance, outerGroupings: false, currOuterHeadingGroupRoot, chartInstance.InnerHeadingInstanceList);
				}
				else if (currOuterHeadingGroupRoot == null)
				{
					CreateOutermostSubtotalCell(chartInstance);
				}
				else
				{
					CreateCellInstance(chartInstance, currOuterHeadingGroupRoot);
				}
			}

			private void CreateCellInstance(ChartInstance chartInstance, RuntimePivotGroupRootObj currOuterHeadingGroupRoot)
			{
				Global.Tracer.Assert(m_cellsList != null && m_cellsList[currOuterHeadingGroupRoot.HeadingLevel] != null);
				RuntimeChartCell runtimeChartCell = (RuntimeChartCell)m_cellsList[currOuterHeadingGroupRoot.HeadingLevel].GetCell(base.PivotDef, this, currOuterHeadingGroupRoot.HeadingLevel);
				Global.Tracer.Assert(runtimeChartCell != null, "(null != cell)");
				runtimeChartCell.CreateInstance(chartInstance);
			}

			private void CreateOutermostSubtotalCell(ChartInstance chartInstance)
			{
				SetupEnvironment();
				chartInstance.AddCell(m_processingContext, -1);
			}

			internal void CreateSubtotalOrStaticCells(ChartInstance chartInstance, RuntimePivotGroupRootObj currOuterHeadingGroupRoot, bool outerGroupingSubtotal)
			{
				_ = (RuntimeChartHeadingsObj)((RuntimeChartGroupRootObj)m_hierarchyRoot).InnerGroupings;
				if (IsOuterGrouping() && !outerGroupingSubtotal)
				{
					CreateOutermostSubtotalCell(chartInstance);
				}
				else
				{
					CreateInnerGroupingsOrCells(chartInstance, currOuterHeadingGroupRoot);
				}
			}
		}

		internal abstract class RuntimePivotGroupRootObj : RuntimeGroupRootObj
		{
			protected RuntimePivotHeadingsObj m_innerGroupings;

			protected PivotHeading m_staticHeadingDef;

			protected bool m_outermostSubtotal;

			protected PivotHeading m_innerHeading;

			protected Subtotal m_innerSubtotal;

			protected PivotHeading m_innerSubtotalStaticHeading;

			protected bool m_processOutermostSTCells;

			protected DataAggregateObjList m_outermostSTCellRVs;

			protected DataAggregateObjList m_cellRVs;

			protected int m_headingLevel;

			internal RuntimePivotHeadingsObj InnerGroupings => m_innerGroupings;

			internal PivotHeading StaticHeadingDef => m_staticHeadingDef;

			internal bool OutermostSubtotal => m_outermostSubtotal;

			internal PivotHeading InnerHeading => m_innerHeading;

			internal bool ProcessOutermostSTCells => m_processOutermostSTCells;

			internal AggregatesImpl OutermostSTCellRVCol => ((PivotHeading)m_hierarchyDef).OutermostSTCellRVCol;

			internal AggregatesImpl[] OutermostSTScopedCellRVCollections => ((PivotHeading)m_hierarchyDef).OutermostSTCellScopedRVCollections;

			internal AggregatesImpl CellRVCol => ((PivotHeading)m_hierarchyDef).CellRVCol;

			internal AggregatesImpl[] CellScopedRVCollections => ((PivotHeading)m_hierarchyDef).CellScopedRVCollections;

			internal int HeadingLevel => m_headingLevel;

			internal RuntimePivotGroupRootObj(IScope outerScope, PivotHeading pivotHeadingDef, ref DataActions dataAction, ProcessingContext processingContext, RuntimePivotHeadingsObj innerGroupings, bool outermostSubtotal, int headingLevel)
				: base(outerScope, pivotHeadingDef, dataAction, processingContext)
			{
				Pivot pivot = (Pivot)pivotHeadingDef.DataRegionDef;
				m_innerHeading = (PivotHeading)pivotHeadingDef.InnerHierarchy;
				pivot.SkipStaticHeading(ref m_innerHeading, ref m_staticHeadingDef);
				if (m_innerHeading != null)
				{
					m_innerSubtotal = m_innerHeading.Subtotal;
					if (m_innerSubtotal != null)
					{
						m_innerSubtotalStaticHeading = m_innerHeading.GetInnerStaticHeading();
					}
				}
				if (outermostSubtotal && (m_innerHeading == null || m_innerSubtotal != null))
				{
					m_processOutermostSTCells = true;
					if (pivot.CellPostSortAggregates != null)
					{
						m_dataAction |= DataActions.PostSortAggregates;
					}
				}
				NeedProcessDataActions(pivotHeadingDef);
				NeedProcessDataActions(m_staticHeadingDef);
				if ((m_dataAction & DataActions.PostSortAggregates) == 0 && m_innerSubtotal != null && m_innerSubtotal.ReportItems.RunningValues != null && 0 < m_innerSubtotal.ReportItems.RunningValues.Count)
				{
					m_dataAction |= DataActions.PostSortAggregates;
				}
				NeedProcessDataActions(m_innerSubtotalStaticHeading);
				m_outermostSubtotal = outermostSubtotal;
				m_innerGroupings = innerGroupings;
				m_headingLevel = headingLevel;
				if (pivotHeadingDef.Grouping.Filters == null)
				{
					dataAction = DataActions.None;
				}
			}

			protected abstract void NeedProcessDataActions(PivotHeading heading);

			protected void NeedProcessDataActions(RunningValueInfoList runningValues)
			{
				if ((m_dataAction & DataActions.PostSortAggregates) == 0 && runningValues != null && 0 < runningValues.Count)
				{
					m_dataAction |= DataActions.PostSortAggregates;
				}
			}

			internal override bool SortAndFilter()
			{
				Pivot pivot = (Pivot)m_hierarchyDef.DataRegionDef;
				PivotHeading pivotHeading = (PivotHeading)m_hierarchyDef;
				if ((ProcessingContext.SecondPassOperations.Filtering & m_processingContext.SecondPassOperation) != 0 && m_hierarchyDef.Grouping.Filters == null && ((pivotHeading.IsColumn && m_headingLevel < pivot.InnermostColumnFilterLevel) || (!pivotHeading.IsColumn && m_headingLevel < pivot.InnermostRowFilterLevel)))
				{
					pivotHeading.Grouping.HasInnerFilters = true;
				}
				bool result = base.SortAndFilter();
				pivotHeading.Grouping.HasInnerFilters = false;
				return result;
			}

			internal override void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
				base.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
				Pivot pivot = (Pivot)m_hierarchyDef.DataRegionDef;
				PivotHeading pivotHeading = (PivotHeading)m_hierarchyDef;
				AggregatesImpl globalCellRVCol = pivotHeading.OutermostSTCellRVCol;
				AggregatesImpl[] cellScopedRVLists = pivotHeading.OutermostSTCellScopedRVCollections;
				if (SetupCellRunningValues(ref globalCellRVCol, ref cellScopedRVLists))
				{
					pivotHeading.OutermostSTCellRVCol = globalCellRVCol;
					pivotHeading.OutermostSTCellScopedRVCollections = cellScopedRVLists;
				}
				if (m_processOutermostSTCells)
				{
					if (m_innerGroupings != null)
					{
						pivot.CurrentOuterHeadingGroupRoot = this;
					}
					m_processingContext.EnterPivotCell(m_innerGroupings != null);
					pivot.ProcessOutermostSTCellRunningValues = true;
					AddCellRunningValues(globalCellRVCol, groupCol, ref m_outermostSTCellRVs);
					pivot.ProcessOutermostSTCellRunningValues = false;
					m_processingContext.ExitPivotCell();
				}
				if (m_innerGroupings != null)
				{
					AggregatesImpl globalCellRVCol2 = pivotHeading.CellRVCol;
					AggregatesImpl[] cellScopedRVLists2 = pivotHeading.CellScopedRVCollections;
					if (SetupCellRunningValues(ref globalCellRVCol2, ref cellScopedRVLists2))
					{
						pivotHeading.CellRVCol = globalCellRVCol2;
						pivotHeading.CellScopedRVCollections = cellScopedRVLists2;
					}
					return;
				}
				RuntimePivotGroupRootObj currentOuterHeadingGroupRoot = pivot.CurrentOuterHeadingGroupRoot;
				if (m_innerHeading == null && currentOuterHeadingGroupRoot != null)
				{
					m_processingContext.EnterPivotCell(escalateScope: true);
					pivot.ProcessCellRunningValues = true;
					m_cellRVs = null;
					AddCellRunningValues(currentOuterHeadingGroupRoot.CellRVCol, groupCol, ref m_cellRVs);
					pivot.ProcessCellRunningValues = false;
					m_processingContext.ExitPivotCell();
				}
			}

			private bool SetupCellRunningValues(ref AggregatesImpl globalCellRVCol, ref AggregatesImpl[] cellScopedRVLists)
			{
				if (globalCellRVCol == null || cellScopedRVLists == null)
				{
					globalCellRVCol = new AggregatesImpl(m_processingContext.ReportRuntime);
					cellScopedRVLists = CreateScopedCellRVCollections();
					return true;
				}
				return false;
			}

			protected abstract void AddCellRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, ref DataAggregateObjList runningValues);

			internal override void AddScopedRunningValue(DataAggregateObj runningValueObj, bool escalate)
			{
				Pivot pivot = (Pivot)m_hierarchyDef.DataRegionDef;
				if (pivot.ProcessOutermostSTCellRunningValues || pivot.ProcessCellRunningValues)
				{
					RuntimePivotGroupRootObj currentOuterHeadingGroupRoot = pivot.CurrentOuterHeadingGroupRoot;
					int headingLevel = currentOuterHeadingGroupRoot.HeadingLevel;
					PivotHeading pivotHeading = (!escalate) ? ((PivotHeading)m_hierarchyDef) : ((PivotHeading)currentOuterHeadingGroupRoot.HierarchyDef);
					if (pivot.ProcessOutermostSTCellRunningValues)
					{
						AddCellScopedRunningValue(runningValueObj, pivotHeading.OutermostSTCellScopedRVCollections, headingLevel);
					}
					else if (pivot.ProcessCellRunningValues)
					{
						AddCellScopedRunningValue(runningValueObj, pivotHeading.CellScopedRVCollections, headingLevel);
					}
				}
				else
				{
					base.AddScopedRunningValue(runningValueObj, escalate);
				}
			}

			private void AddCellScopedRunningValue(DataAggregateObj runningValueObj, AggregatesImpl[] cellScopedRVLists, int currentOuterHeadingLevel)
			{
				if (cellScopedRVLists != null)
				{
					AggregatesImpl aggregatesImpl = cellScopedRVLists[currentOuterHeadingLevel];
					if (aggregatesImpl == null)
					{
						aggregatesImpl = (cellScopedRVLists[currentOuterHeadingLevel] = new AggregatesImpl(m_processingContext.ReportRuntime));
					}
					if (aggregatesImpl.GetAggregateObj(runningValueObj.Name) == null)
					{
						aggregatesImpl.Add(runningValueObj);
					}
				}
			}

			internal override void ReadRow(DataActions dataAction)
			{
				Pivot pivot = (Pivot)m_hierarchyDef.DataRegionDef;
				if (pivot.ProcessCellRunningValues)
				{
					Global.Tracer.Assert(DataActions.PostSortAggregates == dataAction, "(DataActions.PostSortAggregates == dataAction)");
					if (m_cellRVs != null)
					{
						for (int i = 0; i < m_cellRVs.Count; i++)
						{
							m_cellRVs[i].Update();
						}
					}
					if (m_outerScope != null && pivot.CellPostSortAggregates != null)
					{
						m_outerScope.ReadRow(dataAction);
					}
					return;
				}
				if (DataActions.PostSortAggregates == dataAction && m_outermostSTCellRVs != null)
				{
					for (int j = 0; j < m_outermostSTCellRVs.Count; j++)
					{
						m_outermostSTCellRVs[j].Update();
					}
				}
				base.ReadRow(dataAction);
			}

			private AggregatesImpl[] CreateScopedCellRVCollections()
			{
				int dynamicHeadingCount = ((Pivot)m_hierarchyDef.DataRegionDef).GetDynamicHeadingCount(outerGroupings: true);
				if (0 < dynamicHeadingCount)
				{
					return new AggregatesImpl[dynamicHeadingCount];
				}
				return null;
			}

			internal bool GetCellTargetForNonDetailSort()
			{
				if (m_outerScope is RuntimePivotObj)
				{
					return m_outerScope.TargetForNonDetailSort;
				}
				return ((RuntimePivotGroupLeafObj)m_outerScope).GetCellTargetForNonDetailSort();
			}

			internal bool GetCellTargetForSort(int index, bool detailSort)
			{
				if (m_outerScope is RuntimePivotObj)
				{
					return m_outerScope.IsTargetForSort(index, detailSort);
				}
				return ((RuntimePivotGroupLeafObj)m_outerScope).GetCellTargetForSort(index, detailSort);
			}
		}

		internal abstract class RuntimePivotGroupLeafObj : RuntimeGroupLeafObj
		{
			protected RuntimePivotHeadingsObj m_pivotHeadings;

			protected RuntimePivotGroupRootObj m_innerHierarchy;

			protected DataAggregateObjList m_firstPassCellNonCustomAggs;

			protected DataAggregateObjList m_firstPassCellCustomAggs;

			protected RuntimePivotCells[] m_cellsList;

			protected DataAggregateObjList m_cellPostSortAggregates;

			protected int m_groupLeafIndex = -1;

			protected bool m_processHeading = true;

			internal PivotHeading PivotHeadingDef => (PivotHeading)((RuntimePivotGroupRootObj)m_hierarchyRoot).HierarchyDef;

			internal DataAggregateObjList CellPostSortAggregates => m_cellPostSortAggregates;

			internal Pivot PivotDef => (Pivot)PivotHeadingDef.DataRegionDef;

			internal int HeadingLevel => ((RuntimePivotGroupRootObj)m_hierarchyRoot).HeadingLevel;

			internal RuntimePivotGroupLeafObj(RuntimePivotGroupRootObj groupRoot)
				: base(groupRoot)
			{
			}

			protected void ConstructorHelper(RuntimePivotGroupRootObj groupRoot, Pivot pivotDef, out bool handleMyDataAction, out DataActions innerDataAction)
			{
				_ = groupRoot.InnerHeading;
				m_dataAction = groupRoot.DataAction;
				handleMyDataAction = false;
				if (groupRoot.ProcessOutermostSTCells)
				{
					RuntimeDataRegionObj.CreateAggregates(m_processingContext, pivotDef.CellAggregates, ref m_firstPassCellNonCustomAggs, ref m_firstPassCellCustomAggs);
					if (pivotDef.CellPostSortAggregates != null)
					{
						handleMyDataAction = true;
						RuntimeDataRegionObj.CreateAggregates(m_processingContext, pivotDef.CellPostSortAggregates, ref m_postSortAggregates);
					}
				}
				ConstructRuntimeStructure(ref handleMyDataAction, out innerDataAction);
				if (IsOuterGrouping())
				{
					m_groupLeafIndex = ++((RuntimePivotObj)pivotDef.RuntimeDataRegionObj).OuterGroupingCounters[groupRoot.HeadingLevel];
				}
				PivotHeading pivotHeading = (PivotHeading)groupRoot.HierarchyDef;
				Global.Tracer.Assert(pivotHeading.Grouping != null, "(null != pivotHeading.Grouping)");
				if (pivotHeading.Grouping.Filters == null)
				{
					return;
				}
				if (pivotHeading.IsColumn)
				{
					if (groupRoot.HeadingLevel > pivotDef.InnermostColumnFilterLevel)
					{
						pivotDef.InnermostColumnFilterLevel = groupRoot.HeadingLevel;
					}
				}
				else if (groupRoot.HeadingLevel > pivotDef.InnermostRowFilterLevel)
				{
					pivotDef.InnermostRowFilterLevel = groupRoot.HeadingLevel;
				}
			}

			protected override void ConstructRuntimeStructure(ref bool handleMyDataAction, out DataActions innerDataAction)
			{
				RuntimePivotGroupRootObj runtimePivotGroupRootObj = (RuntimePivotGroupRootObj)m_hierarchyRoot;
				Pivot pivot = (Pivot)runtimePivotGroupRootObj.HierarchyDef.DataRegionDef;
				base.ConstructRuntimeStructure(ref handleMyDataAction, out innerDataAction);
				if (IsOuterGrouping() || (runtimePivotGroupRootObj.InnerHeading != null && runtimePivotGroupRootObj.InnerHeading.Subtotal == null))
				{
					return;
				}
				PivotHeading staticHeading = null;
				PivotHeading pivotHeading = pivot.GetPivotHeading(outerHeading: true);
				int dynamicHeadingCount = pivot.GetDynamicHeadingCount(outerGroupings: true);
				int num = 0;
				pivot.SkipStaticHeading(ref pivotHeading, ref staticHeading);
				while (pivotHeading != null)
				{
					pivotHeading = (PivotHeading)pivotHeading.InnerHierarchy;
					bool flag = pivot.SubtotalInInnerHeading(ref pivotHeading, ref staticHeading);
					if (m_cellsList == null)
					{
						m_cellsList = new RuntimePivotCells[dynamicHeadingCount];
						if (m_cellPostSortAggregates == null)
						{
							RuntimeDataRegionObj.CreateAggregates(m_processingContext, pivot.CellPostSortAggregates, ref m_cellPostSortAggregates);
						}
					}
					RuntimePivotCells runtimePivotCells = null;
					if (pivotHeading == null || flag)
					{
						runtimePivotCells = new RuntimePivotCells();
					}
					m_cellsList[num++] = runtimePivotCells;
				}
			}

			internal abstract RuntimePivotCell CreateCell(int index, Pivot pivotDef);

			internal override void NextRow()
			{
				Pivot pivotDef = PivotDef;
				int headingLevel = HeadingLevel;
				bool num = IsOuterGrouping();
				if (num)
				{
					pivotDef.OuterGroupingIndexes[headingLevel] = m_groupLeafIndex;
				}
				UpdateAggregateInfo();
				if (num)
				{
					pivotDef.SaveOuterGroupingAggregateRowInfo(headingLevel, m_processingContext);
				}
				FieldsImpl fieldsImpl = m_processingContext.ReportObjectModel.FieldsImpl;
				if (fieldsImpl.AggregationFieldCount == 0 && fieldsImpl.ValidAggregateRow)
				{
					RuntimeDataRegionObj.UpdateAggregates(m_processingContext, m_firstPassCellCustomAggs, updateAndSetup: false);
				}
				if (!m_processingContext.ReportObjectModel.FieldsImpl.IsAggregateRow)
				{
					RuntimeDataRegionObj.UpdateAggregates(m_processingContext, m_firstPassCellNonCustomAggs, updateAndSetup: false);
				}
				InternalNextRow();
			}

			protected override void SendToInner()
			{
				base.SendToInner();
				if (m_cellsList != null)
				{
					Global.Tracer.Assert(!IsOuterGrouping(), "(!IsOuterGrouping())");
					Pivot pivotDef = PivotDef;
					int[] outerGroupingIndexes = pivotDef.OuterGroupingIndexes;
					for (int i = 0; i < pivotDef.GetDynamicHeadingCount(outerGroupings: true); i++)
					{
						int num = outerGroupingIndexes[i];
						AggregateRowInfo aggregateRowInfo = new AggregateRowInfo();
						aggregateRowInfo.SaveAggregateInfo(m_processingContext);
						pivotDef.SetCellAggregateRowInfo(i, m_processingContext);
						RuntimePivotCells runtimePivotCells = m_cellsList[i];
						if (runtimePivotCells != null)
						{
							RuntimePivotCell runtimePivotCell = runtimePivotCells[num];
							if (runtimePivotCell == null)
							{
								runtimePivotCell = CreateCell(i, pivotDef);
								runtimePivotCells.Add(num, runtimePivotCell);
							}
							runtimePivotCell.NextRow();
						}
						aggregateRowInfo.RestoreAggregateInfo(m_processingContext);
					}
				}
				if (m_pivotHeadings != null)
				{
					m_pivotHeadings.NextRow();
				}
			}

			internal override bool SortAndFilter()
			{
				RuntimePivotGroupRootObj runtimePivotGroupRootObj = (RuntimePivotGroupRootObj)m_hierarchyRoot;
				bool flag = false;
				if (m_innerHierarchy != null && !m_pivotHeadings.SortAndFilter())
				{
					Global.Tracer.Assert((ProcessingContext.SecondPassOperations.Filtering & m_processingContext.SecondPassOperation) != 0);
					Global.Tracer.Assert(runtimePivotGroupRootObj.GroupFilters != null, "(null != groupRoot.GroupFilters)");
					runtimePivotGroupRootObj.GroupFilters.FailFilters = true;
					flag = true;
				}
				bool flag2 = base.SortAndFilter();
				if (flag)
				{
					runtimePivotGroupRootObj.GroupFilters.FailFilters = false;
				}
				if (flag2 && m_cellsList != null)
				{
					for (int i = 0; i < m_cellsList.Length; i++)
					{
						if (m_cellsList[i] != null)
						{
							m_cellsList[i].SortAndFilter();
						}
					}
				}
				return flag2;
			}

			internal override void CalculateRunningValues()
			{
				Pivot pivotDef = PivotDef;
				RuntimePivotGroupRootObj runtimePivotGroupRootObj = (RuntimePivotGroupRootObj)m_hierarchyRoot;
				AggregatesImpl globalRunningValueCollection = runtimePivotGroupRootObj.GlobalRunningValueCollection;
				RuntimeGroupRootObjList groupCollection = runtimePivotGroupRootObj.GroupCollection;
				bool num = IsOuterGrouping();
				pivotDef.GetDynamicHeadingCount(outerGroupings: true);
				if (m_processHeading)
				{
					if (m_dataRows != null && (DataActions.PostSortAggregates & m_dataAction) != 0)
					{
						ReadRows(DataActions.PostSortAggregates);
						m_dataRows = null;
					}
					m_pivotHeadings.CalculateRunningValues(globalRunningValueCollection, groupCollection, runtimePivotGroupRootObj);
				}
				else if (m_innerHierarchy != null)
				{
					m_innerHierarchy.CalculateRunningValues(globalRunningValueCollection, groupCollection, runtimePivotGroupRootObj);
				}
				if (num)
				{
					if (m_innerHierarchy == null || ((PivotHeading)m_innerHierarchy.HierarchyDef).Subtotal != null)
					{
						pivotDef.CurrentOuterHeadingGroupRoot = runtimePivotGroupRootObj;
						pivotDef.OuterGroupingIndexes[runtimePivotGroupRootObj.HeadingLevel] = m_groupLeafIndex;
						runtimePivotGroupRootObj.InnerGroupings.CalculateRunningValues(globalRunningValueCollection, groupCollection, runtimePivotGroupRootObj);
					}
				}
				else if (m_cellsList != null)
				{
					RuntimePivotGroupRootObj currentOuterHeadingGroupRoot = pivotDef.CurrentOuterHeadingGroupRoot;
					RuntimePivotCells runtimePivotCells = m_cellsList[currentOuterHeadingGroupRoot.HeadingLevel];
					Global.Tracer.Assert(runtimePivotCells != null, "(null != cells)");
					pivotDef.ProcessCellRunningValues = true;
					runtimePivotCells.CalculateRunningValues(pivotDef, currentOuterHeadingGroupRoot.CellRVCol, groupCollection, runtimePivotGroupRootObj, this, currentOuterHeadingGroupRoot.HeadingLevel);
					pivotDef.ProcessCellRunningValues = false;
				}
			}

			protected override void ResetScopedRunningValues()
			{
				base.ResetScopedRunningValues();
				ResetScopedCellRunningValues();
			}

			internal bool IsOuterGrouping()
			{
				RuntimePivotGroupRootObj runtimePivotGroupRootObj = (RuntimePivotGroupRootObj)m_hierarchyRoot;
				return runtimePivotGroupRootObj.InnerGroupings != null;
			}

			internal override void ReadRow(DataActions dataAction)
			{
				if (DataActions.UserSort == dataAction)
				{
					RuntimeDataRegionObj.CommonFirstRow(m_processingContext.ReportObjectModel.FieldsImpl, ref m_firstRowIsAggregate, ref m_firstRow);
					CommonNextRow(m_dataRows);
				}
				else if (PivotDef.ProcessCellRunningValues)
				{
					if (DataActions.PostSortAggregates == dataAction && m_cellPostSortAggregates != null)
					{
						RuntimeDataRegionObj.UpdateAggregates(m_processingContext, m_cellPostSortAggregates, updateAndSetup: false);
					}
					((IScope)m_hierarchyRoot).ReadRow(dataAction);
				}
				else
				{
					base.ReadRow(dataAction);
					if (DataActions.PostSortAggregates == dataAction)
					{
						CalculatePreviousAggregates();
					}
				}
			}

			protected virtual bool CalculatePreviousAggregates()
			{
				if (!m_processedPreviousAggregates && m_processingContext.GlobalRVCollection != null)
				{
					if (m_innerHierarchy != null)
					{
						m_pivotHeadings.CalculatePreviousAggregates(m_processingContext.GlobalRVCollection);
					}
					m_processedPreviousAggregates = true;
					return true;
				}
				return false;
			}

			protected void ResetScopedCellRunningValues()
			{
				RuntimePivotGroupRootObj runtimePivotGroupRootObj = (RuntimePivotGroupRootObj)m_hierarchyRoot;
				if (runtimePivotGroupRootObj.OutermostSTScopedCellRVCollections != null)
				{
					for (int i = 0; i < runtimePivotGroupRootObj.OutermostSTScopedCellRVCollections.Length; i++)
					{
						AggregatesImpl aggregatesImpl = runtimePivotGroupRootObj.OutermostSTScopedCellRVCollections[i];
						if (aggregatesImpl == null)
						{
							continue;
						}
						foreach (DataAggregateObj @object in aggregatesImpl.Objects)
						{
							@object.Init();
						}
					}
				}
				if (runtimePivotGroupRootObj.CellScopedRVCollections == null)
				{
					return;
				}
				for (int j = 0; j < runtimePivotGroupRootObj.CellScopedRVCollections.Length; j++)
				{
					AggregatesImpl aggregatesImpl2 = runtimePivotGroupRootObj.CellScopedRVCollections[j];
					if (aggregatesImpl2 == null)
					{
						continue;
					}
					foreach (DataAggregateObj object2 in aggregatesImpl2.Objects)
					{
						object2.Init();
					}
				}
			}

			internal override void SetupEnvironment()
			{
				base.SetupEnvironment();
				SetupAggregateValues(m_firstPassCellNonCustomAggs, m_firstPassCellCustomAggs);
			}

			private void SetupAggregateValues(DataAggregateObjList nonCustomAggCollection, DataAggregateObjList customAggCollection)
			{
				SetupAggregates(nonCustomAggCollection);
				SetupAggregates(customAggCollection);
			}

			internal bool GetCellTargetForNonDetailSort()
			{
				return ((RuntimePivotGroupRootObj)m_hierarchyRoot).GetCellTargetForNonDetailSort();
			}

			internal bool GetCellTargetForSort(int index, bool detailSort)
			{
				return ((RuntimePivotGroupRootObj)m_hierarchyRoot).GetCellTargetForSort(index, detailSort);
			}

			internal bool NeedHandleCellSortFilterEvent()
			{
				if (base.GroupingDef.SortFilterScopeMatched == null)
				{
					return base.GroupingDef.NeedScopeInfoForSortFilterExpression != null;
				}
				return true;
			}

			internal RuntimePivotObj GetOwnerPivot()
			{
				IScope outerScope = OuterScope;
				while (!(outerScope is RuntimePivotObj))
				{
					outerScope = outerScope.GetOuterScope(includeSubReportContainingScope: false);
				}
				Global.Tracer.Assert(outerScope is RuntimePivotObj, "(outerScope is RuntimePivotObj)");
				return (RuntimePivotObj)outerScope;
			}
		}

		private sealed class RuntimeOnePassOWCChartDetailObj : RuntimeOnePassDetailObj, IFilterOwner
		{
			private Filters m_filters;

			private VariantList[] m_chartData;

			private DataAggregateObjList m_nonCustomAggregates;

			private DataAggregateObjList m_customAggregates;

			private FieldImpl[] m_firstRow;

			internal VariantList[] OWCChartData => m_chartData;

			protected override string ScopeName => m_dataRegionDef.Name;

			internal RuntimeOnePassOWCChartDetailObj(IScope outerScope, OWCChart chartDef, ProcessingContext processingContext)
				: base(outerScope, chartDef, processingContext)
			{
				RuntimeDataRegionObj.CreateAggregates(m_processingContext, chartDef.Aggregates, ref m_nonCustomAggregates, ref m_customAggregates);
				RuntimeDataRegionObj.CreateAggregates(m_processingContext, chartDef.PostSortAggregates, ref m_nonCustomAggregates);
				if (chartDef.RunningValues != null && 0 < chartDef.RunningValues.Count)
				{
					RuntimeDataRegionObj.CreateAggregates(m_processingContext, chartDef.RunningValues, ref m_nonCustomAggregates);
				}
				if (chartDef.Filters != null)
				{
					m_filters = new Filters(Filters.FilterTypes.DataRegionFilter, this, chartDef.Filters, chartDef.ObjectType, chartDef.Name, m_processingContext);
				}
				AddRunningValues(chartDef.DetailRunningValues);
				m_chartData = new VariantList[chartDef.ChartData.Count];
				for (int i = 0; i < chartDef.ChartData.Count; i++)
				{
					m_chartData[i] = new VariantList();
				}
			}

			internal override int GetDetailPage()
			{
				return 0;
			}

			internal override void NextRow()
			{
				if (m_processingContext.ReportObjectModel.FieldsImpl.IsAggregateRow)
				{
					NextAggregateRow();
				}
				else
				{
					NextNonAggregateRow();
				}
			}

			private void NextNonAggregateRow()
			{
				bool flag = true;
				if (m_filters != null)
				{
					flag = m_filters.PassFilters(m_processingContext.ReportObjectModel.FieldsImpl.GetFields());
				}
				if (flag)
				{
					((IFilterOwner)this).PostFilterNextRow();
				}
			}

			private void NextAggregateRow()
			{
				if (m_processingContext.ReportObjectModel.FieldsImpl.AggregationFieldCount == 0)
				{
					RuntimeDataRegionObj.UpdateAggregates(m_processingContext, m_customAggregates, updateAndSetup: false);
				}
			}

			void IFilterOwner.PostFilterNextRow()
			{
				if (m_firstRow == null)
				{
					m_firstRow = m_processingContext.ReportObjectModel.FieldsImpl.GetAndSaveFields();
				}
				base.NextRow();
			}

			protected override void CreateInstance()
			{
				OWCChart oWCChart = (OWCChart)m_dataRegionDef;
				for (int i = 0; i < oWCChart.ChartData.Count; i++)
				{
					m_chartData[i].Add(m_processingContext.ReportRuntime.EvaluateOWCChartData(oWCChart, oWCChart.ChartData[i].Value));
				}
			}

			internal override void SetupEnvironment()
			{
				SetupEnvironment(m_nonCustomAggregates, m_customAggregates, m_firstRow);
			}

			internal override bool InScope(string scope)
			{
				return DataRegionInScope(m_dataRegionDef, scope);
			}
		}

		private sealed class RuntimeMatrixHeadingsObj : RuntimePivotHeadingsObj
		{
			private RuntimeRICollection m_subtotal;

			private MatrixHeading m_subtotalHeadingDef;

			private RuntimeRICollection m_subtotalStaticHeading;

			private MatrixHeading m_subtotalStaticHeadingDef;

			private RuntimeRICollection m_staticHeading;

			private MatrixHeadingInstance[] m_subtotalHeadingInstances;

			internal RuntimeMatrixHeadingsObj(IScope owner, MatrixHeading headingDef, ref DataActions dataAction, ProcessingContext processingContext, MatrixHeading staticHeadingDef, RuntimeMatrixHeadingsObj innerGroupings, bool outermostHeadingSubtotal, int headingLevel)
				: base(owner, headingDef, ref dataAction, processingContext, staticHeadingDef, innerGroupings, outermostHeadingSubtotal, headingLevel)
			{
				if (headingDef != null)
				{
					m_pivotHeadings = new RuntimeMatrixGroupRootObj(owner, headingDef, ref dataAction, processingContext, innerGroupings, outermostHeadingSubtotal, headingLevel);
					if (headingDef.Subtotal != null)
					{
						m_subtotalHeadingDef = headingDef;
						m_subtotal = new RuntimeRICollection(owner, m_subtotalHeadingDef.Subtotal.ReportItems, ref dataAction, processingContext, createDataRegions: true);
						MatrixHeading matrixHeading = (MatrixHeading)headingDef.GetInnerStaticHeading();
						if (matrixHeading != null)
						{
							m_subtotalStaticHeadingDef = matrixHeading;
							m_subtotalStaticHeading = new RuntimeRICollection(owner, m_subtotalStaticHeadingDef.ReportItems, ref dataAction, processingContext, createDataRegions: true);
						}
					}
				}
				if (m_staticHeadingDef != null)
				{
					m_staticHeading = new RuntimeRICollection(owner, ((MatrixHeading)m_staticHeadingDef).ReportItems, ref dataAction, processingContext, createDataRegions: true);
				}
			}

			internal override void NextRow()
			{
				base.NextRow();
				if (m_subtotal != null)
				{
					m_subtotal.FirstPassNextDataRow();
					if (m_subtotalStaticHeading != null)
					{
						m_subtotalStaticHeading.FirstPassNextDataRow();
					}
				}
				if (m_staticHeading != null)
				{
					m_staticHeading.FirstPassNextDataRow();
				}
			}

			internal override bool SortAndFilter()
			{
				bool num = base.SortAndFilter();
				if (num)
				{
					if (m_subtotal != null)
					{
						m_subtotal.SortAndFilter();
						if (m_subtotalStaticHeading != null)
						{
							m_subtotalStaticHeading.SortAndFilter();
						}
					}
					if (m_staticHeading != null)
					{
						m_staticHeading.SortAndFilter();
					}
				}
				return num;
			}

			internal override void CalculatePreviousAggregates(AggregatesImpl globalRVCol)
			{
				if (m_pivotHeadings != null && m_subtotal != null)
				{
					m_subtotal.CalculatePreviousAggregates(globalRVCol);
					if (m_subtotalStaticHeading != null)
					{
						m_subtotalStaticHeading.CalculatePreviousAggregates(globalRVCol);
					}
				}
				if (m_staticHeading != null)
				{
					m_staticHeading.CalculatePreviousAggregates(globalRVCol);
				}
			}

			internal override void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
				base.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
				if (m_owner is RuntimeMatrixObj)
				{
					if (m_subtotal != null)
					{
						m_subtotal.CalculateInnerRunningValues(globalRVCol, groupCol, lastGroup);
						if (m_subtotalStaticHeading != null)
						{
							m_subtotalStaticHeading.CalculateInnerRunningValues(globalRVCol, groupCol, lastGroup);
						}
					}
					if (m_staticHeading != null)
					{
						m_staticHeading.CalculateInnerRunningValues(globalRVCol, groupCol, lastGroup);
					}
					return;
				}
				if (m_subtotal != null)
				{
					m_subtotal.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
					if (m_subtotalStaticHeading != null)
					{
						m_subtotalStaticHeading.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
					}
				}
				if (m_staticHeading != null)
				{
					m_staticHeading.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
				}
			}

			internal void CreateInstances(RuntimeDataRegionObj outerGroup, ProcessingContext processingContext, MatrixInstance matrixInstance, bool outerGroupings, RuntimePivotGroupRootObj currOuterHeadingGroupRoot, MatrixHeadingInstanceList headingInstances, RenderingPagesRangesList pagesList)
			{
				bool flag = outerGroupings || matrixInstance.CurrentCellOuterIndex == 0;
				int num = 1;
				if (m_staticHeading != null)
				{
					num = m_staticHeading.ReportItemsDef.Count;
				}
				MatrixHeadingInstanceList matrixHeadingInstanceList = headingInstances;
				RenderingPagesRangesList innerPagesList = pagesList;
				for (int i = 0; i < num; i++)
				{
					bool flag2 = false;
					RenderingPagesRanges renderingPagesRanges = default(RenderingPagesRanges);
					PageTextboxes rowTextboxes = null;
					if (m_staticHeading != null)
					{
						if (flag)
						{
							matrixHeadingInstanceList = CreateHeadingInstance(processingContext, matrixInstance, (MatrixHeading)m_staticHeadingDef, headingInstances, outerGroupings, m_staticHeading, i, isSubtotal: false, isStatic: true, 0, num, out innerPagesList, out rowTextboxes);
							if (!((MatrixHeading)m_staticHeadingDef).IsColumn)
							{
								processingContext.Pagination.EnterIgnorePageBreak(m_staticHeadingDef.Visibility, ignoreAlways: false);
								renderingPagesRanges.StartPage = ((Matrix)matrixInstance.ReportItemDef).CurrentPage;
								flag2 = true;
							}
						}
						if (outerGroupings)
						{
							matrixInstance.CurrentOuterStaticIndex = i;
						}
						else
						{
							matrixInstance.CurrentInnerStaticIndex = i;
						}
					}
					if (m_pivotHeadings != null)
					{
						((Matrix)m_pivotHeadings.HierarchyDef.DataRegionDef).CurrentOuterHeadingGroupRoot = currOuterHeadingGroupRoot;
						if (m_subtotal == null || m_subtotalHeadingDef.Subtotal.Position == Subtotal.PositionType.After)
						{
							m_pivotHeadings.CreateInstances(matrixInstance, matrixHeadingInstanceList, innerPagesList);
						}
						if (m_subtotal != null)
						{
							MatrixHeadingInstanceList matrixHeadingInstanceList2 = null;
							RenderingPagesRangesList innerPagesList2 = null;
							PageTextboxes rowTextboxes2 = null;
							bool flag3 = false;
							RenderingPagesRanges renderingPagesRanges2 = default(RenderingPagesRanges);
							if (flag)
							{
								matrixHeadingInstanceList2 = CreateHeadingInstance(processingContext, matrixInstance, m_subtotalHeadingDef, matrixHeadingInstanceList, outerGroupings, m_subtotal, 0, isSubtotal: true, isStatic: false, i, num, out innerPagesList2, out rowTextboxes2);
								if (!m_subtotalHeadingDef.IsColumn)
								{
									processingContext.Pagination.EnterIgnorePageBreak(m_subtotalHeadingDef.Visibility, ignoreAlways: true);
									renderingPagesRanges2.StartPage = ((Matrix)matrixInstance.ReportItemDef).CurrentPage;
									flag3 = true;
								}
							}
							Global.Tracer.Assert(m_subtotalHeadingInstances[i] != null, "(null != m_subtotalHeadingInstances[i])");
							if (((Matrix)matrixInstance.ReportItemDef).ProcessingInnerGrouping == Pivot.ProcessingInnerGroupings.Column)
							{
								if (!m_subtotalHeadingInstances[i].MatrixHeadingDef.IsColumn)
								{
									processingContext.HeadingInstance = m_subtotalHeadingInstances[i];
								}
								else if (processingContext.HeadingInstance == null)
								{
									processingContext.HeadingInstance = m_subtotalHeadingInstances[i];
								}
							}
							else
							{
								processingContext.HeadingInstanceOld = processingContext.HeadingInstance;
								processingContext.HeadingInstance = m_subtotalHeadingInstances[i];
							}
							int num2 = 1;
							if (m_subtotalStaticHeading != null)
							{
								num2 = m_subtotalStaticHeading.ReportItemsDef.Count;
							}
							for (int j = 0; j < num2; j++)
							{
								bool flag4 = false;
								RenderingPagesRanges renderingPagesRanges3 = default(RenderingPagesRanges);
								if (m_owner is RuntimeMatrixObj)
								{
									((RuntimeMatrixObj)m_owner).SetupEnvironment();
								}
								else
								{
									((RuntimeMatrixGroupLeafObj)outerGroup).SetupEnvironment();
								}
								MatrixHeadingInstance headingInstance = processingContext.HeadingInstance;
								MatrixHeadingInstance headingInstanceOld = processingContext.HeadingInstanceOld;
								if (m_subtotalStaticHeading != null)
								{
									if (flag)
									{
										Global.Tracer.Assert(matrixHeadingInstanceList2 != null, "(null != subtotalInnerHeadings)");
										PageTextboxes rowTextboxes3 = null;
										CreateHeadingInstance(processingContext, matrixInstance, m_subtotalStaticHeadingDef, matrixHeadingInstanceList2, outerGroupings, m_subtotalStaticHeading, j, isSubtotal: true, isStatic: true, 0, num, out RenderingPagesRangesList _, out rowTextboxes3);
										processingContext.HeadingInstance = headingInstance;
										processingContext.HeadingInstanceOld = headingInstanceOld;
										if (!m_subtotalStaticHeadingDef.IsColumn)
										{
											processingContext.Pagination.EnterIgnorePageBreak(m_subtotalStaticHeadingDef.Visibility, ignoreAlways: true);
											renderingPagesRanges3.StartPage = matrixInstance.MatrixDef.CurrentPage;
											matrixInstance.MatrixDef.RowHeaderPageTextboxes.IntegrateRepeatingTextboxValues(rowTextboxes3, renderingPagesRanges3.StartPage, renderingPagesRanges3.StartPage);
											flag4 = true;
										}
									}
									if (outerGroupings)
									{
										matrixInstance.CurrentOuterStaticIndex = j;
									}
									else
									{
										matrixInstance.CurrentInnerStaticIndex = j;
									}
								}
								if (outerGroup is RuntimeMatrixGroupLeafObj)
								{
									((RuntimeMatrixGroupLeafObj)outerGroup).CreateSubtotalOrStaticCells(matrixInstance, currOuterHeadingGroupRoot, outerGroupings);
								}
								else
								{
									((RuntimeMatrixObj)outerGroup).CreateOutermostSubtotalCells(matrixInstance, outerGroupings);
								}
								if (num2 - 1 != j)
								{
									processingContext.HeadingInstance = headingInstance;
									processingContext.HeadingInstanceOld = headingInstanceOld;
								}
								if (flag4)
								{
									processingContext.Pagination.LeaveIgnorePageBreak(m_subtotalStaticHeadingDef.Visibility, ignoreAlways: true);
									renderingPagesRanges3.EndPage = renderingPagesRanges3.StartPage;
									innerPagesList2.Add(renderingPagesRanges3);
								}
								if (m_subtotalStaticHeading != null)
								{
									m_subtotalStaticHeading.ResetReportItemObjs();
								}
							}
							if (m_subtotalStaticHeading != null && flag)
							{
								SetHeadingSpan(matrixInstance, matrixHeadingInstanceList2, outerGroupings, processingContext);
							}
							if (flag3)
							{
								processingContext.Pagination.LeaveIgnorePageBreak(m_subtotalHeadingDef.Visibility, ignoreAlways: true);
								renderingPagesRanges2.EndPage = matrixInstance.MatrixDef.CurrentPage;
								if (innerPagesList2 == null || innerPagesList2.Count < 1)
								{
									renderingPagesRanges2.EndPage = renderingPagesRanges2.StartPage;
								}
								matrixInstance.MatrixDef.RowHeaderPageTextboxes.IntegrateRepeatingTextboxValues(rowTextboxes2, renderingPagesRanges2.StartPage, renderingPagesRanges2.EndPage);
								innerPagesList.Add(renderingPagesRanges2);
							}
						}
						if (outerGroupings)
						{
							processingContext.HeadingInstance = null;
						}
						if (m_subtotal != null && Subtotal.PositionType.Before == m_subtotalHeadingDef.Subtotal.Position)
						{
							m_pivotHeadings.CreateInstances(matrixInstance, matrixHeadingInstanceList, innerPagesList);
						}
						if (flag)
						{
							SetHeadingSpan(matrixInstance, matrixHeadingInstanceList, outerGroupings, processingContext);
						}
					}
					else if (outerGroup is RuntimeMatrixGroupLeafObj)
					{
						RuntimeMatrixGroupLeafObj runtimeMatrixGroupLeafObj = (RuntimeMatrixGroupLeafObj)outerGroup;
						runtimeMatrixGroupLeafObj.SetContentsPage();
						if (!outerGroupings && runtimeMatrixGroupLeafObj.IsOuterGrouping())
						{
							runtimeMatrixGroupLeafObj.CreateSubtotalOrStaticCells(matrixInstance, currOuterHeadingGroupRoot, outerGroupings);
						}
						else
						{
							runtimeMatrixGroupLeafObj.CreateInnerGroupingsOrCells(matrixInstance, currOuterHeadingGroupRoot);
							if (outerGroupings)
							{
								processingContext.HeadingInstance = null;
							}
						}
					}
					else
					{
						((RuntimeMatrixObj)outerGroup).CreateOutermostSubtotalCells(matrixInstance, outerGroupings);
					}
					if (flag2)
					{
						processingContext.Pagination.LeaveIgnorePageBreak(m_staticHeadingDef.Visibility, ignoreAlways: false);
						renderingPagesRanges.EndPage = matrixInstance.MatrixDef.CurrentPage;
						if (matrixHeadingInstanceList == null || matrixHeadingInstanceList.Count < 1)
						{
							renderingPagesRanges.EndPage = renderingPagesRanges.StartPage;
						}
						pagesList.Add(renderingPagesRanges);
						matrixInstance.MatrixDef.RowHeaderPageTextboxes.IntegrateRepeatingTextboxValues(rowTextboxes, renderingPagesRanges.StartPage, renderingPagesRanges.EndPage);
					}
					ResetReportItemObjs(processingContext);
				}
				if (Report.ShowHideTypes.Interactive == processingContext.ShowHideType && flag && m_staticHeading != null)
				{
					for (int num3 = num - 1; num3 >= 0; num3--)
					{
						((IShowHideContainer)headingInstances[num3]).EndProcessContainer(processingContext);
						processingContext.ExitGrouping();
					}
				}
				if (m_staticHeading != null && flag)
				{
					SetHeadingSpan(matrixInstance, headingInstances, outerGroupings, processingContext);
				}
			}

			private void SetHeadingSpan(MatrixInstance matrixInstance, MatrixHeadingInstanceList headingInstances, bool outerGroupings, ProcessingContext processingContext)
			{
				int currentCellIndex = (!outerGroupings) ? matrixInstance.CurrentCellInnerIndex : (matrixInstance.CurrentCellOuterIndex + 1);
				headingInstances.SetLastHeadingSpan(currentCellIndex, processingContext);
			}

			private MatrixHeadingInstanceList CreateHeadingInstance(ProcessingContext processingContext, MatrixInstance matrixInstance, MatrixHeading headingDef, MatrixHeadingInstanceList headingInstances, bool outerGroupings, RuntimeRICollection headingReportItems, int reportItemCount, bool isSubtotal, bool isStatic, int subtotalHeadingIndex, int staticHeadingCount, out RenderingPagesRangesList innerPagesList, out PageTextboxes rowTextboxes)
			{
				rowTextboxes = null;
				MatrixHeadingInstance matrixHeadingInstance = null;
				bool flag = false;
				int headingCellIndex;
				if (outerGroupings)
				{
					matrixInstance.NewOuterCells();
					headingCellIndex = matrixInstance.CurrentCellOuterIndex;
					if (!headingDef.IsColumn)
					{
						processingContext.ChunkManager.CheckPageBreak(headingDef, atStart: true);
					}
				}
				else
				{
					headingCellIndex = matrixInstance.CurrentCellInnerIndex;
					if (processingContext.ReportItemsReferenced)
					{
						processingContext.DelayAddingInstanceInfo = true;
						flag = true;
					}
				}
				matrixHeadingInstance = new MatrixHeadingInstance(processingContext, headingCellIndex, headingDef, isSubtotal && !isStatic, reportItemCount, null, out NonComputedUniqueNames nonComputedUniqueNames);
				headingInstances.Add(matrixHeadingInstance, processingContext);
				if (isSubtotal && !isStatic)
				{
					if (m_subtotalHeadingInstances == null)
					{
						m_subtotalHeadingInstances = new MatrixHeadingInstance[staticHeadingCount];
					}
					m_subtotalHeadingInstances[subtotalHeadingIndex] = matrixHeadingInstance;
				}
				if (!isSubtotal && isStatic && Report.ShowHideTypes.Interactive == processingContext.ShowHideType)
				{
					processingContext.EnterGrouping();
					((IShowHideContainer)matrixHeadingInstance).BeginProcessContainer(processingContext);
				}
				if (headingReportItems != null)
				{
					ReportItemCollection reportItemsDef = headingReportItems.ReportItemsDef;
					int internalIndex = 0;
					bool computed = false;
					ReportItem reportItem = null;
					reportItemsDef.GetReportItem(reportItemCount, out computed, out internalIndex, out reportItem);
					processingContext.PageSectionContext.EnterRepeatingItem();
					processingContext.PageSectionContext.EnterMatrixHeadingScope(Visibility.IsVisible(headingDef.Visibility, headingDef.StartHidden), headingDef.IsColumn);
					if (reportItem != null)
					{
						if (computed)
						{
							processingContext.Pagination.EnterIgnorePageBreak(null, ignoreAlways: true);
							processingContext.Pagination.EnterIgnoreHeight(startHidden: true);
							matrixHeadingInstance.Content = headingReportItems.CreateInstance(reportItem, setupEnvironment: true, ignorePageBreaks: true, headingDef.IsColumn);
							processingContext.Pagination.LeaveIgnoreHeight(startHidden: true);
							processingContext.Pagination.LeaveIgnorePageBreak(null, ignoreAlways: true);
						}
						else
						{
							reportItem.ProcessDrillthroughAction(processingContext, nonComputedUniqueNames);
							reportItem.ProcessNavigationAction(processingContext.NavigationInfo, nonComputedUniqueNames, ((Matrix)headingDef.DataRegionDef).CurrentPage);
							RuntimeRICollection.AddNonComputedPageTextboxes(reportItem, ((Matrix)headingDef.DataRegionDef).CurrentPage, processingContext);
						}
					}
					processingContext.PageSectionContext.ExitMatrixHeadingScope(headingDef.IsColumn);
					PageTextboxes pageTextboxes = processingContext.PageSectionContext.ExitRepeatingItem();
					if (isStatic && isSubtotal)
					{
						pageTextboxes = null;
					}
					if (headingDef.IsColumn)
					{
						matrixInstance.MatrixDef.ColumnHeaderPageTextboxes.IntegrateNonRepeatingTextboxValues(pageTextboxes);
					}
					else
					{
						rowTextboxes = pageTextboxes;
					}
				}
				if (flag)
				{
					processingContext.DelayAddingInstanceInfo = false;
				}
				if (outerGroupings && !headingDef.IsColumn)
				{
					processingContext.ChunkManager.CheckPageBreak(headingDef, atStart: false);
				}
				innerPagesList = matrixHeadingInstance.ChildrenStartAndEndPages;
				return matrixHeadingInstance.SubHeadingInstances;
			}

			internal void ResetReportItemObjs(ProcessingContext processingContext)
			{
				if (m_subtotal != null)
				{
					m_subtotal.ResetReportItemObjs();
					for (MatrixHeading subHeading = m_subtotalHeadingDef.SubHeading; subHeading != null; subHeading = subHeading.SubHeading)
					{
						if (subHeading.Grouping != null)
						{
							RuntimeRICollection.ResetReportItemObjs(subHeading.ReportItems, processingContext);
						}
					}
				}
				if (m_staticHeading != null)
				{
					m_staticHeading.ResetReportItemObjs();
				}
				if (m_subtotalStaticHeading != null)
				{
					m_subtotalStaticHeading.ResetReportItemObjs();
				}
			}
		}

		private sealed class RuntimeMatrixObj : RuntimePivotObj
		{
			private RuntimeRICollection m_matrixCorner;

			private RuntimeRICollection m_subtotalCorner;

			internal RuntimeMatrixObj(IScope outerScope, Matrix matrixDef, ref DataActions dataAction, ProcessingContext processingContext, bool onePassProcess)
				: base(outerScope, matrixDef, ref dataAction, processingContext, onePassProcess)
			{
				ConstructorHelper(ref dataAction, onePassProcess, out bool handleMyDataAction, out DataActions innerDataAction, out PivotHeading outermostColumn, out bool outermostColumnSubtotal, out PivotHeading staticColumn, out PivotHeading outermostRow, out bool outermostRowSubtotal, out PivotHeading staticRow);
				m_innerDataAction = innerDataAction;
				DataActions userSortDataAction = HandleSortFilterEvent();
				MatrixConstructRuntimeStructure(ref innerDataAction, onePassProcess, outermostColumn, outermostColumnSubtotal, staticColumn, outermostRow, outermostRowSubtotal, staticRow);
				HandleDataAction(handleMyDataAction, innerDataAction, userSortDataAction);
			}

			protected override void ConstructRuntimeStructure(ref DataActions innerDataAction)
			{
				m_pivotDef.GetHeadingDefState(out PivotHeading outermostColumn, out bool outermostColumnSubtotal, out PivotHeading staticColumn, out PivotHeading outermostRow, out bool outermostRowSubtotal, out PivotHeading staticRow);
				MatrixConstructRuntimeStructure(ref innerDataAction, onePassProcess: false, outermostColumn, outermostColumnSubtotal, staticColumn, outermostRow, outermostRowSubtotal, staticRow);
			}

			private void MatrixConstructRuntimeStructure(ref DataActions innerDataAction, bool onePassProcess, PivotHeading outermostColumn, bool outermostColumnSubtotal, PivotHeading staticColumn, PivotHeading outermostRow, bool outermostRowSubtotal, PivotHeading staticRow)
			{
				Matrix matrix = (Matrix)m_pivotDef;
				DataActions dataAction = DataActions.None;
				if (m_pivotDef.ProcessingInnerGrouping == Pivot.ProcessingInnerGroupings.Column)
				{
					m_innerGroupings = (m_pivotColumns = new RuntimeMatrixHeadingsObj(this, (MatrixHeading)outermostColumn, ref dataAction, m_processingContext, (MatrixHeading)staticColumn, null, outermostRowSubtotal, 0));
					m_outerGroupings = (m_pivotRows = new RuntimeMatrixHeadingsObj(this, (MatrixHeading)outermostRow, ref innerDataAction, m_processingContext, (MatrixHeading)staticRow, (RuntimeMatrixHeadingsObj)m_innerGroupings, outermostColumnSubtotal, 0));
				}
				else
				{
					m_innerGroupings = (m_pivotRows = new RuntimeMatrixHeadingsObj(this, (MatrixHeading)outermostRow, ref dataAction, m_processingContext, (MatrixHeading)staticRow, null, outermostColumnSubtotal, 0));
					m_outerGroupings = (m_pivotColumns = new RuntimeMatrixHeadingsObj(this, (MatrixHeading)outermostColumn, ref innerDataAction, m_processingContext, (MatrixHeading)staticColumn, (RuntimeMatrixHeadingsObj)m_innerGroupings, outermostRowSubtotal, 0));
				}
				if (matrix.CornerReportItems != null)
				{
					if (onePassProcess)
					{
						m_matrixCorner = new RuntimeRICollection(this, matrix.CornerReportItems, m_processingContext, createDataRegions: true);
					}
					else
					{
						m_matrixCorner = new RuntimeRICollection(this, matrix.CornerReportItems, ref innerDataAction, m_processingContext, createDataRegions: true);
					}
				}
				matrix.InOutermostSubtotalCell = true;
				if (onePassProcess)
				{
					m_subtotalCorner = new RuntimeRICollection(this, matrix.CellReportItems, m_processingContext, createDataRegions: true);
				}
				else if ((outermostRowSubtotal && outermostColumnSubtotal) || (outermostRow == null && outermostColumn == null))
				{
					m_subtotalCorner = new RuntimeRICollection(this, matrix.CellReportItems, ref innerDataAction, m_processingContext, createDataRegions: true);
				}
				matrix.InOutermostSubtotalCell = false;
			}

			private bool OutermostSTCellTargetScopeMatched(int index, RuntimeSortFilterEventInfo sortFilterInfo)
			{
				VariantList[] sortSourceScopeInfo = sortFilterInfo.SortSourceScopeInfo;
				PivotHeading pivotHeading = m_pivotDef.GetPivotHeading(outerHeading: false);
				PivotHeading staticHeading = null;
				m_pivotDef.SkipStaticHeading(ref pivotHeading, ref staticHeading);
				if (pivotHeading != null)
				{
					Grouping grouping = pivotHeading.Grouping;
					if (grouping.IsOnPathToSortFilterSource(index))
					{
						int dynamicHeadingCount = m_pivotDef.GetDynamicHeadingCount(outerGroupings: false);
						int num = grouping.SortFilterScopeIndex[index];
						int num2 = 0;
						while (num2 < dynamicHeadingCount && num < sortSourceScopeInfo.Length)
						{
							if (sortSourceScopeInfo[num] != null)
							{
								return false;
							}
							num2++;
							num++;
						}
					}
				}
				PivotHeading pivotHeading2 = m_pivotDef.GetPivotHeading(outerHeading: true);
				m_pivotDef.SkipStaticHeading(ref pivotHeading2, ref staticHeading);
				if (pivotHeading2 != null)
				{
					Grouping grouping2 = pivotHeading2.Grouping;
					if (grouping2.IsOnPathToSortFilterSource(index))
					{
						int dynamicHeadingCount2 = m_pivotDef.GetDynamicHeadingCount(outerGroupings: true);
						int num = grouping2.SortFilterScopeIndex[index];
						int num3 = 0;
						while (num3 < dynamicHeadingCount2 && num < sortSourceScopeInfo.Length)
						{
							if (sortSourceScopeInfo[num] != null)
							{
								return false;
							}
							num3++;
							num++;
						}
					}
				}
				return true;
			}

			internal override bool TargetScopeMatched(int index, bool detailSort)
			{
				if (((Matrix)m_pivotDef).InOutermostSubtotalCell && !OutermostSTCellTargetScopeMatched(index, m_processingContext.RuntimeSortFilterInfo[index]))
				{
					return false;
				}
				return base.TargetScopeMatched(index, detailSort);
			}

			private void GetScopeValuesForOutermostSTCell(IHierarchyObj targetScopeObj, VariantList[] scopeValues, ref int index)
			{
				int dynamicHeadingCount = m_pivotDef.GetDynamicHeadingCount(outerGroupings: false);
				for (int i = 0; i < dynamicHeadingCount; i++)
				{
					Global.Tracer.Assert(index < scopeValues.Length, "Inner headings scope values");
					scopeValues[index++] = null;
				}
				dynamicHeadingCount = m_pivotDef.GetDynamicHeadingCount(outerGroupings: true);
				for (int j = 0; j < dynamicHeadingCount; j++)
				{
					Global.Tracer.Assert(index < scopeValues.Length, "Outer headings scope values");
					scopeValues[index++] = null;
				}
			}

			internal override void GetScopeValues(IHierarchyObj targetScopeObj, VariantList[] scopeValues, ref int index)
			{
				base.GetScopeValues(targetScopeObj, scopeValues, ref index);
				if (((Matrix)m_pivotDef).InOutermostSubtotalCell)
				{
					GetScopeValuesForOutermostSTCell(targetScopeObj, scopeValues, ref index);
				}
			}

			protected override void SendToInner()
			{
				base.SendToInner();
				if (m_matrixCorner != null)
				{
					m_matrixCorner.FirstPassNextDataRow();
				}
				if (m_subtotalCorner != null)
				{
					((Matrix)m_pivotDef).InOutermostSubtotalCell = true;
					m_subtotalCorner.FirstPassNextDataRow();
					((Matrix)m_pivotDef).InOutermostSubtotalCell = false;
				}
			}

			internal override bool SortAndFilter()
			{
				SetupEnvironment();
				if (m_userSortTargetInfo != null)
				{
					m_userSortTargetInfo.EnterProcessUserSortPhase(m_processingContext);
				}
				base.SortAndFilter();
				if (m_matrixCorner != null)
				{
					m_matrixCorner.SortAndFilter();
				}
				if (m_subtotalCorner != null)
				{
					m_subtotalCorner.SortAndFilter();
				}
				if (m_userSortTargetInfo != null)
				{
					m_userSortTargetInfo.LeaveProcessUserSortPhase(m_processingContext);
				}
				return true;
			}

			internal override void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
				base.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
				if (m_matrixCorner != null)
				{
					m_matrixCorner.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
				}
				if (m_subtotalCorner != null)
				{
					m_subtotalCorner.CalculateInnerRunningValues(globalRVCol, groupCol, lastGroup);
				}
				RuntimeRICollection.DoneReadingRows(globalRVCol, m_pivotDef.RunningValues, ref m_runningValueValues, processPreviousAggregates: false);
			}

			protected override void CalculatePreviousAggregates()
			{
				if (!m_processedPreviousAggregates && m_processingContext.GlobalRVCollection != null)
				{
					base.CalculatePreviousAggregates();
					if (m_matrixCorner != null)
					{
						m_matrixCorner.CalculatePreviousAggregates(m_processingContext.GlobalRVCollection);
					}
				}
			}

			internal override void CreateInstances(ReportItemInstance riInstance, IList instanceList, RenderingPagesRangesList pagesList)
			{
				if (m_firstRow == null)
				{
					return;
				}
				MatrixInstance matrixInstance = (MatrixInstance)riInstance;
				Matrix matrix = (Matrix)m_pivotDef;
				matrix.InitializePageSectionProcessing();
				PageSectionContext pageSectionContext = m_processingContext.PageSectionContext;
				m_processingContext.PageSectionContext = new PageSectionContext(pageSectionContext);
				m_processingContext.PageSectionContext.EnterVisibilityScope(matrix.Visibility, matrix.StartHidden);
				PageTextboxes source = null;
				if (m_matrixCorner != null)
				{
					ReportItemCollection cornerReportItems = matrix.CornerReportItems;
					int internalIndex = 0;
					bool computed = false;
					ReportItem reportItem = null;
					cornerReportItems.GetReportItem(0, out computed, out internalIndex, out reportItem);
					m_processingContext.PageSectionContext.EnterRepeatingItem();
					if (reportItem != null)
					{
						if (computed)
						{
							m_processingContext.Pagination.EnterIgnorePageBreak(null, ignoreAlways: true);
							m_processingContext.Pagination.EnterIgnoreHeight(startHidden: true);
							matrixInstance.CornerContent = m_matrixCorner.CreateInstance(reportItem, setupEnvironment: true, ignorePageBreaks: true, ignoreInstances: true);
							m_processingContext.Pagination.LeaveIgnoreHeight(startHidden: true);
							m_processingContext.Pagination.LeaveIgnorePageBreak(null, ignoreAlways: true);
						}
						else
						{
							NonComputedUniqueNames cornerNonComputedUniqueNames = matrix.CornerNonComputedUniqueNames;
							reportItem.ProcessDrillthroughAction(m_processingContext, cornerNonComputedUniqueNames);
							reportItem.ProcessNavigationAction(m_processingContext.NavigationInfo, cornerNonComputedUniqueNames, matrix.CurrentPage);
							RuntimeRICollection.AddNonComputedPageTextboxes(reportItem, matrix.CurrentPage, m_processingContext);
						}
					}
					source = m_processingContext.PageSectionContext.ExitRepeatingItem();
				}
				if (Report.ShowHideTypes.Interactive == m_processingContext.ShowHideType)
				{
					m_processingContext.RegisterIgnoreRange();
				}
				bool delayAddingInstanceInfo = m_processingContext.DelayAddingInstanceInfo;
				m_processingContext.DelayAddingInstanceInfo = false;
				m_processingContext.NavigationInfo.GetCurrentDocumentMapPosition(out int siblingIndex, out int nodeIndex);
				if (m_outerGroupings == m_pivotRows)
				{
					matrixInstance.InnerHeadingInstanceList = matrixInstance.ColumnInstances;
					((RuntimeMatrixHeadingsObj)m_outerGroupings).CreateInstances(this, m_processingContext, matrixInstance, outerGroupings: true, null, matrixInstance.RowInstances, pagesList);
				}
				else
				{
					matrixInstance.InnerHeadingInstanceList = matrixInstance.RowInstances;
					((RuntimeMatrixHeadingsObj)m_outerGroupings).CreateInstances(this, m_processingContext, matrixInstance, outerGroupings: true, null, matrixInstance.ColumnInstances, pagesList);
				}
				m_processingContext.DelayAddingInstanceInfo = delayAddingInstanceInfo;
				if (Report.ShowHideTypes.Interactive == m_processingContext.ShowHideType)
				{
					m_processingContext.UnRegisterIgnoreRange();
				}
				m_processingContext.NavigationInfo.InsertMatrixColumnDocumentMap(siblingIndex, nodeIndex);
				if (m_processingContext.ReportItemsReferenced)
				{
					AddInnerHeadingsToChunk(matrixInstance.InnerHeadingInstanceList, matrixInstance.InFirstPage ? true : false);
				}
				int count = matrixInstance.ChildrenStartAndEndPages.Count;
				if (count > 0)
				{
					matrix.EndPage = matrixInstance.ChildrenStartAndEndPages[count - 1].EndPage;
				}
				else
				{
					matrix.EndPage = ((IPageItem)matrixInstance).StartPage;
				}
				m_processingContext.Pagination.ProcessEndPage(matrixInstance, matrix, matrix.PageBreakAtEnd || matrix.PropagatedPageBreakAtEnd, matrixInstance.NumberOfChildrenOnThisPage > 0);
				m_processingContext.PageSectionContext.ExitVisibilityScope();
				if (m_processingContext.PageSectionContext.PageTextboxes != null)
				{
					Global.Tracer.Assert(m_processingContext.PageSectionContext.PageTextboxes.GetPageCount() == 0, "Invalid state");
					pageSectionContext.PageTextboxes.IntegrateNonRepeatingTextboxValues(m_processingContext.PageSectionContext.PageTextboxes);
				}
				m_processingContext.PageSectionContext = pageSectionContext;
				if (m_processingContext.PageSectionContext.PageTextboxes != null)
				{
					m_processingContext.PageSectionContext.PageTextboxes.IntegrateRepeatingTextboxValues(source, matrix.StartPage, matrix.EndPage);
					m_processingContext.PageSectionContext.PageTextboxes.IntegrateRepeatingTextboxValues(matrixInstance.MatrixDef.ColumnHeaderPageTextboxes, matrix.StartPage, matrix.EndPage);
					m_processingContext.PageSectionContext.PageTextboxes.IntegrateNonRepeatingTextboxValues(matrixInstance.MatrixDef.RowHeaderPageTextboxes);
					m_processingContext.PageSectionContext.PageTextboxes.IntegrateNonRepeatingTextboxValues(matrixInstance.MatrixDef.CellPageTextboxes);
				}
			}

			internal void ResetReportItems()
			{
				if (m_matrixCorner != null)
				{
					m_matrixCorner.ResetReportItemObjs();
				}
				if (m_pivotRows != null)
				{
					((RuntimeMatrixHeadingsObj)m_pivotRows).ResetReportItemObjs(m_processingContext);
				}
				if (m_pivotColumns != null)
				{
					((RuntimeMatrixHeadingsObj)m_pivotColumns).ResetReportItemObjs(m_processingContext);
				}
			}

			internal void CreateOutermostSubtotalCells(MatrixInstance matrixInstance, bool outerGroupings)
			{
				if (outerGroupings)
				{
					SetupEnvironment();
					if (Report.ShowHideTypes.Interactive == m_processingContext.ShowHideType)
					{
						m_processingContext.EndIgnoreRange();
					}
					((RuntimeMatrixHeadingsObj)m_innerGroupings).CreateInstances(this, m_processingContext, matrixInstance, outerGroupings: false, null, matrixInstance.InnerHeadingInstanceList, matrixInstance.ChildrenStartAndEndPages);
					if (Report.ShowHideTypes.Interactive == m_processingContext.ShowHideType)
					{
						m_processingContext.UseAllContainers = true;
					}
				}
				else
				{
					if (m_subtotalCorner == null)
					{
						return;
					}
					if (Report.ShowHideTypes.Interactive == m_processingContext.ShowHideType)
					{
						m_processingContext.IgnoreAllFromStart = true;
					}
					bool inMatrixSubtotal = m_processingContext.PageSectionContext.InMatrixSubtotal;
					m_processingContext.PageSectionContext.InMatrixSubtotal = true;
					bool computed;
					ReportItem cellReportItemDef = matrixInstance.GetCellReportItemDef(-1, out computed);
					NonComputedUniqueNames cellNonComputedUniqueNames;
					MatrixCellInstance matrixCellInstance = matrixInstance.AddCell(m_processingContext, out cellNonComputedUniqueNames);
					if (cellReportItemDef != null)
					{
						if (computed)
						{
							SetupEnvironment();
							m_processingContext.Pagination.EnterIgnorePageBreak(null, ignoreAlways: true);
							m_processingContext.Pagination.EnterIgnoreHeight(startHidden: true);
							((Matrix)m_pivotDef).InOutermostSubtotalCell = true;
							matrixCellInstance.Content = m_subtotalCorner.CreateInstance(cellReportItemDef, setupEnvironment: true, ignorePageBreaks: true, ignoreInstances: false);
							((Matrix)m_pivotDef).InOutermostSubtotalCell = false;
							m_processingContext.Pagination.LeaveIgnoreHeight(startHidden: true);
							m_processingContext.Pagination.LeaveIgnorePageBreak(null, ignoreAlways: true);
							m_subtotalCorner.ResetReportItemObjs();
						}
						else
						{
							cellReportItemDef.ProcessDrillthroughAction(m_processingContext, cellNonComputedUniqueNames);
							cellReportItemDef.ProcessNavigationAction(m_processingContext.NavigationInfo, cellNonComputedUniqueNames, ((Matrix)matrixInstance.ReportItemDef).CurrentPage);
						}
					}
					m_processingContext.PageSectionContext.InMatrixSubtotal = inMatrixSubtotal;
					if (Report.ShowHideTypes.Interactive == m_processingContext.ShowHideType)
					{
						m_processingContext.IgnoreAllFromStart = false;
					}
				}
			}

			private void AddInnerHeadingsToChunk(MatrixHeadingInstanceList headings, bool addToFirstPage)
			{
				if (headings == null)
				{
					return;
				}
				for (int i = 0; i < headings.Count; i++)
				{
					MatrixHeadingInstance matrixHeadingInstance = headings[i];
					m_processingContext.ChunkManager.AddInstance(matrixHeadingInstance.InstanceInfo, matrixHeadingInstance, addToFirstPage, m_processingContext.InPageSection);
					if (matrixHeadingInstance.Content != null)
					{
						AddInnerHeadingsToChunk(matrixHeadingInstance.Content, addToFirstPage);
					}
					AddInnerHeadingsToChunk(matrixHeadingInstance.SubHeadingInstances, addToFirstPage);
				}
			}

			private void AddInnerHeadingsToChunk(ReportItemColInstance reportItemColInstance, bool addToFirstPage)
			{
				ReportItemColInstanceInfo instanceInfo = reportItemColInstance.GetInstanceInfo(null, m_processingContext.InPageSection);
				m_processingContext.ChunkManager.AddInstance(instanceInfo, reportItemColInstance, addToFirstPage, m_processingContext.InPageSection);
				if (reportItemColInstance.ReportItemInstances != null)
				{
					for (int i = 0; i < reportItemColInstance.ReportItemInstances.Count; i++)
					{
						AddInnerHeadingsToChunk(reportItemColInstance[i], addToFirstPage);
					}
				}
			}

			private void AddInnerHeadingsToChunk(ReportItemInstance reportItemInstance, bool addToFirstPage)
			{
				if (reportItemInstance is TextBoxInstance)
				{
					m_processingContext.ChunkManager.AddInstance(((TextBoxInstance)reportItemInstance).InstanceInfo, reportItemInstance, addToFirstPage, m_processingContext.InPageSection);
					return;
				}
				ReportItemInstanceInfo instanceInfo = reportItemInstance.GetInstanceInfo(null);
				m_processingContext.ChunkManager.AddInstance(instanceInfo, reportItemInstance, addToFirstPage, m_processingContext.InPageSection);
				if (reportItemInstance is RectangleInstance)
				{
					AddInnerHeadingsToChunk(((RectangleInstance)reportItemInstance).ReportItemColInstance, addToFirstPage);
				}
				else if (reportItemInstance is MatrixInstance)
				{
					AddInnerHeadingsToChunk(((MatrixInstance)reportItemInstance).CornerContent, addToFirstPage);
				}
				else
				{
					if (!(reportItemInstance is TableInstance))
					{
						return;
					}
					TableInstance tableInstance = (TableInstance)reportItemInstance;
					if (tableInstance.HeaderRowInstances != null)
					{
						for (int i = 0; i < tableInstance.HeaderRowInstances.Length; i++)
						{
							TableRowInstance tableRowInstance = tableInstance.HeaderRowInstances[i];
							m_processingContext.ChunkManager.AddInstance(tableRowInstance.GetInstanceInfo(null), tableRowInstance, addToFirstPage, m_processingContext.InPageSection);
							AddInnerHeadingsToChunk(tableRowInstance.TableRowReportItemColInstance, addToFirstPage);
						}
					}
					if (tableInstance.FooterRowInstances != null)
					{
						for (int j = 0; j < tableInstance.FooterRowInstances.Length; j++)
						{
							TableRowInstance tableRowInstance2 = tableInstance.FooterRowInstances[j];
							m_processingContext.ChunkManager.AddInstance(tableRowInstance2.GetInstanceInfo(null), tableRowInstance2, addToFirstPage, m_processingContext.InPageSection);
							AddInnerHeadingsToChunk(tableRowInstance2.TableRowReportItemColInstance, addToFirstPage);
						}
					}
				}
			}
		}

		private sealed class RuntimeMatrixGroupRootObj : RuntimePivotGroupRootObj
		{
			private ReportItemCollection m_cellRIs;

			internal ReportItemCollection CellRIs => m_cellRIs;

			internal RuntimeMatrixGroupRootObj(IScope outerScope, MatrixHeading matrixHeadingDef, ref DataActions dataAction, ProcessingContext processingContext, RuntimeMatrixHeadingsObj innerGroupings, bool outermostSubtotal, int headingLevel)
				: base(outerScope, matrixHeadingDef, ref dataAction, processingContext, innerGroupings, outermostSubtotal, headingLevel)
			{
				if (m_processOutermostSTCells)
				{
					Matrix matrix = (Matrix)matrixHeadingDef.DataRegionDef;
					m_cellRIs = matrix.CellReportItems;
					if (m_cellRIs.RunningValues != null && 0 < m_cellRIs.RunningValues.Count)
					{
						m_dataAction |= DataActions.PostSortAggregates;
					}
				}
				if (matrixHeadingDef.OwcGroupExpression)
				{
					m_saveGroupExprValues = true;
				}
			}

			protected override void NeedProcessDataActions(PivotHeading heading)
			{
				MatrixHeading matrixHeading = (MatrixHeading)heading;
				if (matrixHeading != null)
				{
					NeedProcessDataActions(matrixHeading.ReportItems.RunningValues);
				}
			}

			internal override void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
				base.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
				ReportItemCollection reportItems = ((MatrixHeading)m_hierarchyDef).ReportItems;
				if (reportItems != null)
				{
					AddRunningValues(reportItems.RunningValues);
				}
				if (m_staticHeadingDef != null)
				{
					AddRunningValues(((MatrixHeading)m_staticHeadingDef).ReportItems.RunningValues);
				}
				if (m_innerSubtotal != null)
				{
					AddRunningValues(m_innerSubtotal.ReportItems.RunningValues);
				}
				if (m_innerSubtotalStaticHeading != null)
				{
					AddRunningValues(((MatrixHeading)m_innerSubtotalStaticHeading).ReportItems.RunningValues);
				}
				m_grouping.Traverse(ProcessingStages.RunningValues, m_expression.Direction);
				if (m_hierarchyDef.Grouping.Name != null)
				{
					groupCol.Remove(m_hierarchyDef.Grouping.Name);
				}
			}

			protected override void AddCellRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, ref DataAggregateObjList runningValues)
			{
				ReportItemCollection cellReportItems = ((Matrix)m_hierarchyDef.DataRegionDef).CellReportItems;
				if (cellReportItems.RunningValues != null && 0 < cellReportItems.RunningValues.Count)
				{
					m_dataAction |= DataActions.PostSortAggregates;
					if (runningValues == null)
					{
						AddRunningValues(cellReportItems.RunningValues, ref runningValues, globalRVCol, groupCol);
					}
				}
			}
		}

		internal abstract class RuntimePivotCell : IScope
		{
			protected RuntimePivotGroupLeafObj m_owner;

			protected int m_cellLevel;

			protected DataAggregateObjList m_cellNonCustomAggObjs;

			protected DataAggregateObjList m_cellCustomAggObjs;

			protected DataAggregateObjResult[] m_cellAggValueList;

			protected DataRowList m_dataRows;

			protected bool m_innermost;

			protected FieldImpl[] m_firstRow;

			protected bool m_firstRowIsAggregate;

			protected RuntimePivotCell m_nextCell;

			protected int[] m_sortFilterExpressionScopeInfoIndices;

			internal RuntimePivotCell NextCell
			{
				get
				{
					return m_nextCell;
				}
				set
				{
					m_nextCell = value;
				}
			}

			bool IScope.TargetForNonDetailSort => m_owner.GetCellTargetForNonDetailSort();

			int[] IScope.SortFilterExpressionScopeInfoIndices
			{
				get
				{
					if (m_sortFilterExpressionScopeInfoIndices == null)
					{
						m_sortFilterExpressionScopeInfoIndices = new int[m_owner.ProcessingContext.RuntimeSortFilterInfo.Count];
						for (int i = 0; i < m_owner.ProcessingContext.RuntimeSortFilterInfo.Count; i++)
						{
							m_sortFilterExpressionScopeInfoIndices[i] = -1;
						}
					}
					return m_sortFilterExpressionScopeInfoIndices;
				}
			}

			internal RuntimePivotCell(RuntimePivotGroupLeafObj owner, int cellLevel, DataAggregateInfoList aggDefs, bool innermost)
			{
				m_owner = owner;
				m_cellLevel = cellLevel;
				RuntimeDataRegionObj.CreateAggregates(owner.ProcessingContext, aggDefs, ref m_cellNonCustomAggObjs, ref m_cellCustomAggObjs);
				DataAggregateObjList cellPostSortAggregates = m_owner.CellPostSortAggregates;
				if (cellPostSortAggregates != null && 0 < cellPostSortAggregates.Count)
				{
					m_cellAggValueList = new DataAggregateObjResult[cellPostSortAggregates.Count];
				}
				m_innermost = innermost;
			}

			internal virtual void NextRow()
			{
				RuntimeDataRegionObj.CommonFirstRow(m_owner.ProcessingContext.ReportObjectModel.FieldsImpl, ref m_firstRowIsAggregate, ref m_firstRow);
				NextAggregateRow();
				if (!m_owner.ProcessingContext.ReportObjectModel.FieldsImpl.IsAggregateRow)
				{
					NextNonAggregateRow();
				}
			}

			private void NextNonAggregateRow()
			{
				RuntimeDataRegionObj.UpdateAggregates(m_owner.ProcessingContext, m_cellNonCustomAggObjs, updateAndSetup: false);
				if (m_dataRows != null)
				{
					RuntimeDetailObj.SaveData(m_dataRows, m_owner.ProcessingContext);
				}
			}

			private void NextAggregateRow()
			{
				FieldsImpl fieldsImpl = m_owner.ProcessingContext.ReportObjectModel.FieldsImpl;
				if (fieldsImpl.ValidAggregateRow && fieldsImpl.AggregationFieldCount == 0 && m_cellCustomAggObjs != null)
				{
					RuntimeDataRegionObj.UpdateAggregates(m_owner.ProcessingContext, m_cellCustomAggObjs, updateAndSetup: false);
				}
			}

			internal virtual void SortAndFilter()
			{
			}

			internal virtual void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
				if (m_dataRows != null)
				{
					Global.Tracer.Assert(m_innermost, "(m_innermost)");
					ReadRows();
					m_dataRows = null;
				}
				DataAggregateObjList cellPostSortAggregates = m_owner.CellPostSortAggregates;
				if (cellPostSortAggregates != null && 0 < cellPostSortAggregates.Count)
				{
					for (int i = 0; i < cellPostSortAggregates.Count; i++)
					{
						m_cellAggValueList[i] = cellPostSortAggregates[i].AggregateResult();
						cellPostSortAggregates[i].Init();
					}
				}
			}

			private void ReadRows()
			{
				for (int i = 0; i < m_dataRows.Count; i++)
				{
					FieldImpl[] fields = m_dataRows[i];
					m_owner.ProcessingContext.ReportObjectModel.FieldsImpl.SetFields(fields);
					((IScope)this).ReadRow(DataActions.PostSortAggregates);
				}
			}

			protected void SetupAggregates(DataAggregateObjList aggregates, DataAggregateObjResult[] aggValues)
			{
				if (aggregates != null)
				{
					for (int i = 0; i < aggregates.Count; i++)
					{
						DataAggregateObj dataAggregateObj = aggregates[i];
						m_owner.ProcessingContext.ReportObjectModel.AggregatesImpl.Set(dataAggregateObj.Name, dataAggregateObj.AggregateDef, dataAggregateObj.DuplicateNames, (aggValues == null) ? dataAggregateObj.AggregateResult() : aggValues[i]);
					}
				}
			}

			protected void SetupEnvironment()
			{
				SetupAggregates(m_cellNonCustomAggObjs, null);
				SetupAggregates(m_cellCustomAggObjs, null);
				SetupAggregates(m_owner.CellPostSortAggregates, m_cellAggValueList);
				m_owner.ProcessingContext.ReportObjectModel.FieldsImpl.SetFields(m_firstRow);
				m_owner.ProcessingContext.ReportRuntime.CurrentScope = this;
			}

			private Hashtable GetOuterScopeNames()
			{
				PivotHeading pivotHeadingDef = m_owner.PivotHeadingDef;
				Pivot pivot = (Pivot)pivotHeadingDef.DataRegionDef;
				Hashtable hashtable = null;
				if (pivotHeadingDef.CellScopeNames == null)
				{
					pivotHeadingDef.CellScopeNames = new Hashtable[pivot.GetDynamicHeadingCount(outerGroupings: true)];
				}
				else
				{
					hashtable = pivotHeadingDef.CellScopeNames[m_cellLevel];
				}
				if (hashtable == null)
				{
					hashtable = pivot.GetOuterScopeNames(m_cellLevel);
					pivotHeadingDef.CellScopeNames[m_cellLevel] = hashtable;
				}
				return hashtable;
			}

			bool IScope.IsTargetForSort(int index, bool detailSort)
			{
				return m_owner.GetCellTargetForSort(index, detailSort);
			}

			string IScope.GetScopeName()
			{
				return null;
			}

			IScope IScope.GetOuterScope(bool includeSubReportContainingScope)
			{
				return m_owner;
			}

			void IScope.ReadRow(DataActions dataAction)
			{
				m_owner.ReadRow(dataAction);
			}

			bool IScope.InScope(string scope)
			{
				if (m_owner.InScope(scope))
				{
					return true;
				}
				return GetOuterScopeNames().Contains(scope);
			}

			int IScope.RecursiveLevel(string scope)
			{
				if (scope == null)
				{
					return 0;
				}
				int num = ((IScope)m_owner).RecursiveLevel(scope);
				if (-1 != num)
				{
					return num;
				}
				return (GetOuterScopeNames()[scope] as Grouping)?.RecursiveLevel ?? (-1);
			}

			bool IScope.TargetScopeMatched(int index, bool detailSort)
			{
				if (!m_owner.TargetScopeMatched(index, detailSort))
				{
					return false;
				}
				IDictionaryEnumerator enumerator = GetOuterScopeNames().GetEnumerator();
				while (enumerator.MoveNext())
				{
					Grouping grouping = (Grouping)enumerator.Value;
					if ((!detailSort || grouping.SortFilterScopeInfo != null) && grouping.SortFilterScopeMatched != null && !grouping.SortFilterScopeMatched[index])
					{
						return false;
					}
				}
				if (detailSort)
				{
					return true;
				}
				Pivot pivotDef = m_owner.PivotDef;
				VariantList[] sortSourceScopeInfo = m_owner.ProcessingContext.RuntimeSortFilterInfo[index].SortSourceScopeInfo;
				if (m_owner.GroupingDef.SortFilterScopeIndex != null && -1 != m_owner.GroupingDef.SortFilterScopeIndex[index])
				{
					int num = m_owner.GroupingDef.SortFilterScopeIndex[index] + 1;
					if (!m_innermost)
					{
						int dynamicHeadingCount = pivotDef.GetDynamicHeadingCount(outerGroupings: false);
						int num2 = m_owner.HeadingLevel + 1;
						while (num2 < dynamicHeadingCount && num < sortSourceScopeInfo.Length)
						{
							if (sortSourceScopeInfo[num] != null)
							{
								return false;
							}
							num2++;
							num++;
						}
					}
				}
				PivotHeading outerHeading = pivotDef.GetOuterHeading(m_cellLevel + 1);
				if (outerHeading != null && outerHeading.Grouping.SortFilterScopeIndex != null && -1 != outerHeading.Grouping.SortFilterScopeIndex[index])
				{
					int dynamicHeadingCount2 = pivotDef.GetDynamicHeadingCount(outerGroupings: true);
					int num = outerHeading.Grouping.SortFilterScopeIndex[index];
					int num3 = m_cellLevel + 1;
					while (num3 < dynamicHeadingCount2 && num < sortSourceScopeInfo.Length)
					{
						if (sortSourceScopeInfo[num] != null)
						{
							return false;
						}
						num3++;
						num++;
					}
				}
				return true;
			}

			void IScope.GetScopeValues(IHierarchyObj targetScopeObj, VariantList[] scopeValues, ref int index)
			{
				Pivot pivotDef = m_owner.PivotDef;
				m_owner.GetScopeValues(targetScopeObj, scopeValues, ref index);
				int dynamicHeadingCount;
				if (!m_innermost)
				{
					dynamicHeadingCount = pivotDef.GetDynamicHeadingCount(outerGroupings: false);
					for (int i = m_owner.HeadingLevel + 1; i < dynamicHeadingCount; i++)
					{
						Global.Tracer.Assert(index < scopeValues.Length, "Subtotal inner headings");
						scopeValues[index++] = null;
					}
				}
				Hashtable outerScopeNames = GetOuterScopeNames();
				IDictionaryEnumerator enumerator = outerScopeNames.GetEnumerator();
				while (enumerator.MoveNext())
				{
					Grouping grouping = (Grouping)enumerator.Value;
					Global.Tracer.Assert(index < scopeValues.Length, "Inner headings");
					scopeValues[index++] = grouping.CurrentGroupExpressionValues;
				}
				dynamicHeadingCount = pivotDef.GetDynamicHeadingCount(outerGroupings: true);
				for (int j = outerScopeNames.Count; j < dynamicHeadingCount; j++)
				{
					Global.Tracer.Assert(index < scopeValues.Length, "Outer headings");
					scopeValues[index++] = null;
				}
			}

			void IScope.GetGroupNameValuePairs(Dictionary<string, object> pairs)
			{
				((IScope)m_owner).GetGroupNameValuePairs(pairs);
				Hashtable outerScopeNames = GetOuterScopeNames();
				if (outerScopeNames != null)
				{
					IEnumerator enumerator = outerScopeNames.Values.GetEnumerator();
					while (enumerator.MoveNext())
					{
						RuntimeDataRegionObj.AddGroupNameValuePair(m_owner.ProcessingContext, enumerator.Current as Grouping, pairs);
					}
				}
			}
		}

		private sealed class RuntimeMatrixCell : RuntimePivotCell
		{
			private ReportItemCollection m_cellDef;

			private RuntimeRICollection m_cellReportItems;

			internal RuntimeMatrixCell(RuntimeMatrixGroupLeafObj owner, int cellLevel, DataAggregateInfoList aggDefs, ReportItemCollection cellDef, bool innermost)
				: base(owner, cellLevel, aggDefs, innermost)
			{
				m_cellDef = cellDef;
				DataActions dataAction = DataActions.None;
				bool flag = m_cellDef.RunningValues != null && 0 < m_cellDef.RunningValues.Count;
				if (m_innermost && (flag || m_owner.CellPostSortAggregates != null))
				{
					dataAction = DataActions.PostSortAggregates;
				}
				HandleSortFilterEvent();
				m_cellReportItems = new RuntimeRICollection(this, m_cellDef, ref dataAction, m_owner.ProcessingContext, createDataRegions: true);
				if (dataAction != 0)
				{
					m_dataRows = new DataRowList();
				}
			}

			private void HandleSortFilterEvent()
			{
				if (!m_owner.NeedHandleCellSortFilterEvent())
				{
					return;
				}
				int count = m_owner.ProcessingContext.RuntimeSortFilterInfo.Count;
				for (int i = 0; i < count; i++)
				{
					RuntimeSortFilterEventInfo runtimeSortFilterEventInfo = m_owner.ProcessingContext.RuntimeSortFilterInfo[i];
					if (runtimeSortFilterEventInfo.EventSource.IsMatrixCellScope)
					{
						ReportItem parent = runtimeSortFilterEventInfo.EventSource.Parent;
						while (parent != null && !(parent is Matrix))
						{
							parent = parent.Parent;
						}
						if (parent == m_owner.PivotDef && ((IScope)this).TargetScopeMatched(i, detailSort: false) && !m_owner.GetOwnerPivot().TargetForNonDetailSort && runtimeSortFilterEventInfo.EventSourceScope == null)
						{
							runtimeSortFilterEventInfo.EventSourceScope = this;
						}
					}
				}
			}

			internal override void NextRow()
			{
				base.NextRow();
				m_cellReportItems.FirstPassNextDataRow();
			}

			internal override void SortAndFilter()
			{
				m_cellReportItems.SortAndFilter();
			}

			internal override void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
				base.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
				m_cellReportItems.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
			}

			internal void CreateInstance(MatrixInstance matrixInstance, ReportItem reportItemDef, MatrixCellInstance cellInstance)
			{
				SetupEnvironment();
				m_owner.ProcessingContext.Pagination.EnterIgnorePageBreak(null, ignoreAlways: true);
				m_owner.ProcessingContext.Pagination.EnterIgnoreHeight(startHidden: true);
				cellInstance.Content = m_cellReportItems.CreateInstance(reportItemDef, setupEnvironment: true, ignorePageBreaks: true, ignoreInstances: false);
				m_owner.ProcessingContext.Pagination.LeaveIgnoreHeight(startHidden: true);
				m_owner.ProcessingContext.Pagination.LeaveIgnorePageBreak(null, ignoreAlways: true);
				m_cellReportItems.ResetReportItemObjs();
			}
		}

		private sealed class RuntimeMatrixGroupLeafObj : RuntimePivotGroupLeafObj
		{
			private RuntimeRICollection m_headingReportItemCol;

			private RuntimeRICollection m_firstPassCell;

			private MatrixHeadingInstance m_headingInstance;

			private string m_label;

			private int m_startPage = -1;

			private bool m_startHidden;

			internal RuntimeMatrixGroupLeafObj(RuntimeMatrixGroupRootObj groupRoot)
				: base(groupRoot)
			{
				MatrixHeading matrixHeading = (MatrixHeading)groupRoot.HierarchyDef;
				Matrix pivotDef = (Matrix)matrixHeading.DataRegionDef;
				MatrixHeading headingDef = (MatrixHeading)groupRoot.InnerHeading;
				bool handleMyDataAction = false;
				bool num = HandleSortFilterEvent();
				ConstructorHelper(groupRoot, pivotDef, out handleMyDataAction, out DataActions innerDataAction);
				m_pivotHeadings = new RuntimeMatrixHeadingsObj(this, headingDef, ref innerDataAction, groupRoot.ProcessingContext, (MatrixHeading)groupRoot.StaticHeadingDef, (RuntimeMatrixHeadingsObj)groupRoot.InnerGroupings, groupRoot.OutermostSubtotal, groupRoot.HeadingLevel + 1);
				m_innerHierarchy = m_pivotHeadings.Headings;
				if (matrixHeading.ReportItems != null)
				{
					m_headingReportItemCol = new RuntimeRICollection(this, matrixHeading.ReportItems, ref innerDataAction, m_processingContext, createDataRegions: true);
				}
				if (groupRoot.CellRIs != null)
				{
					DataActions dataAction = DataActions.None;
					matrixHeading.InOutermostSubtotalCell = true;
					m_firstPassCell = new RuntimeRICollection(this, groupRoot.CellRIs, ref dataAction, m_processingContext, createDataRegions: true);
					matrixHeading.InOutermostSubtotalCell = false;
				}
				if (!handleMyDataAction)
				{
					m_dataAction = innerDataAction;
				}
				if (num)
				{
					m_dataAction |= DataActions.UserSort;
				}
				if (m_firstPassCell != null)
				{
					HandleOutermostSTCellSortFilterEvent();
				}
				if (m_dataAction != 0)
				{
					m_dataRows = new DataRowList();
				}
			}

			private void HandleOutermostSTCellSortFilterEvent()
			{
				if (!NeedHandleCellSortFilterEvent())
				{
					return;
				}
				int count = m_processingContext.RuntimeSortFilterInfo.Count;
				for (int i = 0; i < count; i++)
				{
					if (!base.GroupingDef.IsOnPathToSortFilterSource(i))
					{
						continue;
					}
					RuntimeSortFilterEventInfo runtimeSortFilterEventInfo = m_processingContext.RuntimeSortFilterInfo[i];
					if (m_targetScopeMatched[i] && runtimeSortFilterEventInfo.EventSource.IsMatrixCellScope)
					{
						ReportItem parent = runtimeSortFilterEventInfo.EventSource.Parent;
						while (parent != null && !(parent is Matrix))
						{
							parent = parent.Parent;
						}
						if (parent == base.PivotDef && OutermostSTCellTargetScopeMatched(i, runtimeSortFilterEventInfo) && !GetOwnerPivot().TargetForNonDetailSort && runtimeSortFilterEventInfo.EventSourceScope == null)
						{
							runtimeSortFilterEventInfo.EventSourceScope = this;
						}
					}
				}
			}

			private bool OutermostSTCellTargetScopeMatched(int index, RuntimeSortFilterEventInfo sortFilterInfo)
			{
				Pivot pivotDef = base.PivotDef;
				int dynamicHeadingCount = pivotDef.GetDynamicHeadingCount(outerGroupings: false);
				int dynamicHeadingCount2 = pivotDef.GetDynamicHeadingCount(outerGroupings: true);
				VariantList[] sortSourceScopeInfo = sortFilterInfo.SortSourceScopeInfo;
				if (IsOuterGrouping())
				{
					PivotHeading pivotHeading = pivotDef.GetPivotHeading(outerHeading: false);
					PivotHeading staticHeading = null;
					pivotDef.SkipStaticHeading(ref pivotHeading, ref staticHeading);
					if (pivotHeading != null)
					{
						Grouping grouping = pivotHeading.Grouping;
						if (grouping.IsOnPathToSortFilterSource(index))
						{
							int num = grouping.SortFilterScopeIndex[index];
							int num2 = 0;
							while (num2 < dynamicHeadingCount && num < sortSourceScopeInfo.Length)
							{
								if (sortSourceScopeInfo[num] != null)
								{
									return false;
								}
								num2++;
								num++;
							}
						}
					}
					if (base.GroupingDef.IsOnPathToSortFilterSource(index))
					{
						int num = base.GroupingDef.SortFilterScopeIndex[index] + 1;
						int num3 = base.HeadingLevel + 1;
						while (num3 < dynamicHeadingCount2 && num < sortSourceScopeInfo.Length)
						{
							if (sortSourceScopeInfo[num] != null)
							{
								return false;
							}
							num3++;
							num++;
						}
					}
				}
				else
				{
					if (base.GroupingDef.IsOnPathToSortFilterSource(index))
					{
						int num = base.GroupingDef.SortFilterScopeIndex[index] + 1;
						int num4 = base.HeadingLevel + 1;
						while (num4 < dynamicHeadingCount && num < sortSourceScopeInfo.Length)
						{
							if (sortSourceScopeInfo[num] != null)
							{
								return false;
							}
							num4++;
							num++;
						}
					}
					PivotHeading pivotHeading2 = pivotDef.GetPivotHeading(outerHeading: true);
					PivotHeading staticHeading2 = null;
					pivotDef.SkipStaticHeading(ref pivotHeading2, ref staticHeading2);
					if (pivotHeading2 != null)
					{
						Grouping grouping2 = pivotHeading2.Grouping;
						if (grouping2.IsOnPathToSortFilterSource(index))
						{
							int num = grouping2.SortFilterScopeIndex[index];
							int num5 = 0;
							while (num5 < dynamicHeadingCount2 && num < sortSourceScopeInfo.Length)
							{
								if (sortSourceScopeInfo[num] != null)
								{
									return false;
								}
								num5++;
								num++;
							}
						}
					}
				}
				return true;
			}

			internal override bool TargetScopeMatched(int index, bool detailSort)
			{
				if (detailSort && base.GroupingDef.SortFilterScopeInfo == null)
				{
					return true;
				}
				if (m_targetScopeMatched != null && m_targetScopeMatched[index])
				{
					if (((MatrixHeading)base.PivotHeadingDef).InOutermostSubtotalCell)
					{
						return OutermostSTCellTargetScopeMatched(index, m_processingContext.RuntimeSortFilterInfo[index]);
					}
					return true;
				}
				return false;
			}

			internal override RuntimePivotCell CreateCell(int index, Pivot pivotDef)
			{
				return new RuntimeMatrixCell(this, index, pivotDef.CellAggregates, ((Matrix)pivotDef).CellReportItems, m_innerHierarchy == null);
			}

			protected override void SendToInner()
			{
				base.SendToInner();
				if (m_headingReportItemCol != null)
				{
					m_headingReportItemCol.FirstPassNextDataRow();
				}
				if (m_firstPassCell != null)
				{
					((MatrixHeading)base.PivotHeadingDef).InOutermostSubtotalCell = true;
					m_firstPassCell.FirstPassNextDataRow();
					((MatrixHeading)base.PivotHeadingDef).InOutermostSubtotalCell = false;
				}
			}

			internal override bool SortAndFilter()
			{
				SetupEnvironment();
				if (m_userSortTargetInfo != null)
				{
					m_userSortTargetInfo.EnterProcessUserSortPhase(m_processingContext);
				}
				if (m_headingReportItemCol != null)
				{
					m_headingReportItemCol.SortAndFilter();
				}
				if (m_firstPassCell != null)
				{
					m_firstPassCell.SortAndFilter();
				}
				bool result = base.SortAndFilter();
				if (m_userSortTargetInfo != null)
				{
					m_userSortTargetInfo.LeaveProcessUserSortPhase(m_processingContext);
				}
				return result;
			}

			internal override void CalculateRunningValues()
			{
				base.CalculateRunningValues();
				if (m_processHeading)
				{
					RuntimePivotGroupRootObj runtimePivotGroupRootObj = (RuntimePivotGroupRootObj)m_hierarchyRoot;
					AggregatesImpl globalRunningValueCollection = runtimePivotGroupRootObj.GlobalRunningValueCollection;
					RuntimeGroupRootObjList groupCollection = runtimePivotGroupRootObj.GroupCollection;
					if (m_headingReportItemCol != null)
					{
						m_headingReportItemCol.CalculateRunningValues(globalRunningValueCollection, groupCollection, runtimePivotGroupRootObj);
					}
					if (m_firstPassCell != null)
					{
						_ = (Matrix)base.PivotDef;
						m_processingContext.EnterPivotCell(escalateScope: true);
						m_firstPassCell.CalculateRunningValues(runtimePivotGroupRootObj.OutermostSTCellRVCol, groupCollection, runtimePivotGroupRootObj);
						m_processingContext.ExitPivotCell();
					}
					m_processHeading = false;
				}
				ResetScopedRunningValues();
			}

			protected override bool CalculatePreviousAggregates()
			{
				if (base.CalculatePreviousAggregates() && m_headingReportItemCol != null)
				{
					m_headingReportItemCol.CalculatePreviousAggregates(m_processingContext.GlobalRVCollection);
					return true;
				}
				return false;
			}

			internal void SetContentsPage()
			{
				RuntimeMatrixGroupRootObj runtimeMatrixGroupRootObj = (RuntimeMatrixGroupRootObj)m_hierarchyRoot;
				if (!((MatrixHeading)runtimeMatrixGroupRootObj.HierarchyDef).IsColumn)
				{
					((MatrixInstance)runtimeMatrixGroupRootObj.ReportItemInstance).MatrixDef.CellPage = m_startPage;
				}
			}

			internal override void CreateInstance()
			{
				SetupEnvironment();
				RuntimeMatrixGroupRootObj runtimeMatrixGroupRootObj = (RuntimeMatrixGroupRootObj)m_hierarchyRoot;
				Matrix matrix = (Matrix)base.PivotDef;
				MatrixInstance matrixInstance = (MatrixInstance)runtimeMatrixGroupRootObj.ReportItemInstance;
				MatrixHeadingInstanceList matrixHeadingInstanceList = (MatrixHeadingInstanceList)runtimeMatrixGroupRootObj.InstanceList;
				MatrixHeading matrixHeading = (MatrixHeading)runtimeMatrixGroupRootObj.HierarchyDef;
				bool flag = false;
				bool flag2 = IsOuterGrouping();
				RenderingPagesRangesList pagesList = runtimeMatrixGroupRootObj.PagesList;
				PageTextboxes pageTextboxes = null;
				if (m_targetScopeMatched != null)
				{
					matrixHeading.Grouping.SortFilterScopeMatched = m_targetScopeMatched;
				}
				RuntimePivotGroupRootObj currOuterHeadingGroupRoot;
				int headingCellIndex;
				if (flag2)
				{
					currOuterHeadingGroupRoot = (matrix.CurrentOuterHeadingGroupRoot = runtimeMatrixGroupRootObj);
					matrix.OuterGroupingIndexes[runtimeMatrixGroupRootObj.HeadingLevel] = m_groupLeafIndex;
					matrixInstance.NewOuterCells();
					headingCellIndex = matrixInstance.CurrentCellOuterIndex;
					if (!matrixHeading.IsColumn)
					{
						m_processingContext.ChunkManager.CheckPageBreak(matrixHeading, atStart: true);
					}
				}
				else
				{
					currOuterHeadingGroupRoot = matrix.CurrentOuterHeadingGroupRoot;
					headingCellIndex = matrixInstance.CurrentCellInnerIndex;
				}
				RenderingPagesRanges renderingPagesRanges = default(RenderingPagesRanges);
				if (flag2 || matrixInstance.CurrentCellOuterIndex == 0)
				{
					if (matrixHeading.IsColumn)
					{
						m_processingContext.NavigationInfo.EnterMatrixColumn();
					}
					else
					{
						m_processingContext.Pagination.EnterIgnorePageBreak(matrixHeading.Visibility, ignoreAlways: false);
						if (!m_processingContext.Pagination.IgnorePageBreak && matrixHeadingInstanceList.Count != 0 && matrixHeading.Grouping.PageBreakAtStart && matrixInstance.NumberOfChildrenOnThisPage > 0)
						{
							m_processingContext.Pagination.SetCurrentPageHeight(matrix, 0.0);
							matrixInstance.ExtraPagesFilled++;
							matrix.CurrentPage++;
							matrixInstance.NumberOfChildrenOnThisPage = 0;
						}
						renderingPagesRanges.StartPage = matrix.CurrentPage;
						m_startPage = renderingPagesRanges.StartPage;
					}
					flag = true;
					if (!flag2 && m_processingContext.ReportItemsReferenced)
					{
						m_processingContext.DelayAddingInstanceInfo = true;
					}
					m_headingInstance = new MatrixHeadingInstance(m_processingContext, headingCellIndex, matrixHeading, isSubtotal: false, 0, m_groupExprValues, out NonComputedUniqueNames nonComputedUniqueNames);
					m_startHidden = matrixHeading.StartHidden;
					matrixHeadingInstanceList.Add(m_headingInstance, m_processingContext);
					matrixHeadingInstanceList = m_headingInstance.SubHeadingInstances;
					pagesList = m_headingInstance.ChildrenStartAndEndPages;
					if (Report.ShowHideTypes.Interactive == m_processingContext.ShowHideType)
					{
						m_processingContext.EnterGrouping();
						((IShowHideContainer)m_headingInstance).BeginProcessContainer(m_processingContext);
					}
					if (matrixHeading.Grouping.GroupLabel != null)
					{
						m_label = m_processingContext.NavigationInfo.CurrentLabel;
						if (m_label != null)
						{
							m_processingContext.NavigationInfo.EnterDocumentMapChildren();
						}
					}
					if (m_headingReportItemCol != null)
					{
						bool computed;
						ReportItem content = matrixHeading.GetContent(out computed);
						if (content != null)
						{
							m_processingContext.PageSectionContext.EnterRepeatingItem();
							m_processingContext.PageSectionContext.EnterMatrixHeadingScope(Visibility.IsVisible(matrixHeading.Visibility, m_startHidden), matrixHeading.IsColumn);
							if (computed)
							{
								m_processingContext.Pagination.EnterIgnorePageBreak(null, ignoreAlways: true);
								m_processingContext.Pagination.EnterIgnoreHeight(startHidden: true);
								m_headingInstance.Content = m_headingReportItemCol.CreateInstance(content, setupEnvironment: true, ignorePageBreaks: true, matrixHeading.IsColumn);
								m_processingContext.Pagination.LeaveIgnoreHeight(startHidden: true);
								m_processingContext.Pagination.LeaveIgnorePageBreak(null, ignoreAlways: true);
							}
							else
							{
								content.ProcessDrillthroughAction(m_processingContext, nonComputedUniqueNames);
								content.ProcessNavigationAction(m_processingContext.NavigationInfo, nonComputedUniqueNames, matrix.CurrentPage);
							}
							m_processingContext.PageSectionContext.ExitMatrixHeadingScope(matrixHeading.IsColumn);
							if (matrixHeading.IsColumn)
							{
								matrixInstance.MatrixDef.ColumnHeaderPageTextboxes.IntegrateNonRepeatingTextboxValues(m_processingContext.PageSectionContext.ExitRepeatingItem());
							}
							else
							{
								pageTextboxes = m_processingContext.PageSectionContext.ExitRepeatingItem();
							}
						}
					}
					if (!flag2 && m_processingContext.ReportItemsReferenced)
					{
						m_processingContext.DelayAddingInstanceInfo = false;
					}
					if (matrixHeading.IsColumn)
					{
						m_processingContext.NavigationInfo.LeaveMatrixColumn();
					}
				}
				else
				{
					if (m_processingContext.ReportItemsReferenced)
					{
						SetReportItemObjs(m_headingInstance.Content);
					}
					SetContentsPage();
				}
				m_processingContext.PageSectionContext.EnterMatrixHeadingScope(Visibility.IsVisible(matrixHeading.Visibility, m_startHidden), matrixHeading.IsColumn);
				((RuntimeMatrixHeadingsObj)m_pivotHeadings).CreateInstances(this, m_processingContext, matrixInstance, flag2, currOuterHeadingGroupRoot, matrixHeadingInstanceList, pagesList);
				m_processingContext.PageSectionContext.ExitMatrixHeadingScope(matrixHeading.IsColumn);
				if (flag && Report.ShowHideTypes.Interactive == m_processingContext.ShowHideType)
				{
					((IShowHideContainer)m_headingInstance).EndProcessContainer(m_processingContext);
					m_processingContext.ExitGrouping();
				}
				if ((flag2 || matrixInstance.CurrentCellOuterIndex == 0) && !matrixHeading.IsColumn)
				{
					bool pageBreakAtEnd = matrixHeading.Grouping.PageBreakAtEnd;
					renderingPagesRanges.EndPage = ((Matrix)matrixInstance.ReportItemDef).CurrentPage;
					if (m_headingInstance.SubHeadingInstances == null || m_headingInstance.SubHeadingInstances.Count < 1)
					{
						renderingPagesRanges.EndPage = renderingPagesRanges.StartPage;
					}
					else
					{
						renderingPagesRanges.EndPage = m_headingInstance.ChildrenStartAndEndPages[m_headingInstance.ChildrenStartAndEndPages.Count - 1].EndPage;
					}
					if (!m_processingContext.Pagination.IgnorePageBreak && matrixInstance.NumberOfChildrenOnThisPage > 0 && m_processingContext.Pagination.CanMoveToNextPage(pageBreakAtEnd))
					{
						m_processingContext.Pagination.SetCurrentPageHeight(matrix, 0.0);
						matrixInstance.ExtraPagesFilled++;
						matrix.CurrentPage++;
						matrixInstance.NumberOfChildrenOnThisPage = 0;
					}
					runtimeMatrixGroupRootObj.PagesList.Add(renderingPagesRanges);
					m_startPage = renderingPagesRanges.StartPage;
					if (pageTextboxes != null)
					{
						matrixInstance.MatrixDef.RowHeaderPageTextboxes.IntegrateRepeatingTextboxValues(pageTextboxes, renderingPagesRanges.StartPage, renderingPagesRanges.EndPage);
					}
					m_processingContext.Pagination.LeaveIgnoreHeight(matrixHeading.StartHidden);
					m_processingContext.Pagination.LeaveIgnorePageBreak(matrixHeading.Visibility, ignoreAlways: false);
				}
				if (flag2 && !matrixHeading.IsColumn)
				{
					m_processingContext.ChunkManager.CheckPageBreak(matrixHeading, atStart: false);
				}
				if (m_headingReportItemCol != null)
				{
					m_headingReportItemCol.ResetReportItemObjs();
				}
				((RuntimeMatrixHeadingsObj)m_pivotHeadings).ResetReportItemObjs(m_processingContext);
				ResetReportItemsWithHideDuplicates();
			}

			protected override void AddToDocumentMap()
			{
				if (base.GroupingDef.GroupLabel != null && m_label != null)
				{
					bool isColumn = ((MatrixHeading)((RuntimeGroupRootObj)m_hierarchyRoot).HierarchyDef).IsColumn;
					NavigationInfo navigationInfo = m_processingContext.NavigationInfo;
					if (isColumn)
					{
						navigationInfo.EnterMatrixColumn();
					}
					navigationInfo.AddToDocumentMap(m_headingInstance.UniqueName, isContainer: true, m_startPage, m_label);
					if (isColumn)
					{
						navigationInfo.LeaveMatrixColumn();
					}
					m_label = null;
				}
			}

			internal void CreateInnerGroupingsOrCells(MatrixInstance matrixInstance, RuntimePivotGroupRootObj currOuterHeadingGroupRoot)
			{
				SetupEnvironment();
				if (IsOuterGrouping())
				{
					RuntimeMatrixHeadingsObj obj = (RuntimeMatrixHeadingsObj)((RuntimeMatrixGroupRootObj)m_hierarchyRoot).InnerGroupings;
					if (Report.ShowHideTypes.Interactive == m_processingContext.ShowHideType)
					{
						m_processingContext.EndIgnoreRange();
					}
					obj.CreateInstances(this, m_processingContext, matrixInstance, outerGroupings: false, currOuterHeadingGroupRoot, matrixInstance.InnerHeadingInstanceList, matrixInstance.ChildrenStartAndEndPages);
					if (Report.ShowHideTypes.Interactive == m_processingContext.ShowHideType)
					{
						m_processingContext.UseAllContainers = true;
					}
				}
				else if (currOuterHeadingGroupRoot == null)
				{
					CreateOutermostSubtotalCell(matrixInstance);
				}
				else
				{
					CreateCellInstance(matrixInstance, currOuterHeadingGroupRoot);
				}
			}

			private void CreateCellInstance(MatrixInstance matrixInstance, RuntimePivotGroupRootObj currOuterHeadingGroupRoot)
			{
				if (Report.ShowHideTypes.Interactive == m_processingContext.ShowHideType)
				{
					m_processingContext.IgnoreAllFromStart = true;
					m_processingContext.EnterGrouping();
				}
				int currentCellRIIndex = matrixInstance.GetCurrentCellRIIndex();
				bool computed;
				ReportItem cellReportItemDef = matrixInstance.GetCellReportItemDef(currentCellRIIndex, out computed);
				NonComputedUniqueNames cellNonComputedUniqueNames;
				MatrixCellInstance cellInstance = matrixInstance.AddCell(m_processingContext, out cellNonComputedUniqueNames);
				if (cellReportItemDef != null)
				{
					m_processingContext.PageSectionContext.InMatrixCell = true;
					m_processingContext.PageSectionContext.InMatrixSubtotal = (m_processingContext.HeadingInstance != null);
					m_processingContext.PageSectionContext.EnterRepeatingItem();
					if (computed)
					{
						Global.Tracer.Assert(m_cellsList != null && m_cellsList[currOuterHeadingGroupRoot.HeadingLevel] != null);
						RuntimeMatrixCell runtimeMatrixCell = (RuntimeMatrixCell)m_cellsList[currOuterHeadingGroupRoot.HeadingLevel].GetCell(base.PivotDef, this, currOuterHeadingGroupRoot.HeadingLevel);
						Global.Tracer.Assert(runtimeMatrixCell != null, "(null != cell)");
						runtimeMatrixCell.CreateInstance(matrixInstance, cellReportItemDef, cellInstance);
					}
					else
					{
						cellReportItemDef.ProcessDrillthroughAction(m_processingContext, cellNonComputedUniqueNames);
						cellReportItemDef.ProcessNavigationAction(m_processingContext.NavigationInfo, cellNonComputedUniqueNames, matrixInstance.MatrixDef.CurrentPage);
						RuntimeRICollection.AddNonComputedPageTextboxes(cellReportItemDef, matrixInstance.MatrixDef.CurrentPage, m_processingContext);
					}
					matrixInstance.MatrixDef.CellPageTextboxes.IntegrateRepeatingTextboxValues(m_processingContext.PageSectionContext.ExitRepeatingItem(), matrixInstance.MatrixDef.CellPage, matrixInstance.MatrixDef.CellPage);
					m_processingContext.PageSectionContext.InMatrixSubtotal = false;
					m_processingContext.PageSectionContext.InMatrixCell = false;
				}
				if (Report.ShowHideTypes.Interactive == m_processingContext.ShowHideType)
				{
					m_processingContext.IgnoreAllFromStart = false;
					m_processingContext.ExitGrouping();
				}
			}

			private void SetupAggregateValues()
			{
				SetupEnvironment(m_nonCustomAggregates, m_customAggregates, m_firstRow);
			}

			private void SetupEnvironmentForOuterGroupings()
			{
				if (!IsOuterGrouping())
				{
					return;
				}
				IScope outerScope = OuterScope;
				while (outerScope != null && !(outerScope is RuntimeMatrixObj))
				{
					if (outerScope is RuntimeMatrixGroupLeafObj)
					{
						((RuntimeMatrixGroupLeafObj)outerScope).SetupAggregateValues();
					}
					outerScope = outerScope.GetOuterScope(includeSubReportContainingScope: false);
				}
			}

			private void CreateOutermostSubtotalCell(MatrixInstance matrixInstance)
			{
				if (m_firstPassCell == null)
				{
					return;
				}
				bool inMatrixSubtotal = m_processingContext.PageSectionContext.InMatrixSubtotal;
				m_processingContext.PageSectionContext.InMatrixSubtotal = true;
				if (Report.ShowHideTypes.Interactive == m_processingContext.ShowHideType)
				{
					m_processingContext.IgnoreAllFromStart = true;
					m_processingContext.EnterGrouping();
				}
				bool computed;
				ReportItem cellReportItemDef = matrixInstance.GetCellReportItemDef(-1, out computed);
				NonComputedUniqueNames cellNonComputedUniqueNames;
				MatrixCellInstance matrixCellInstance = matrixInstance.AddCell(m_processingContext, out cellNonComputedUniqueNames);
				if (cellReportItemDef != null)
				{
					if (computed)
					{
						SetupEnvironmentForOuterGroupings();
						SetupEnvironment();
						m_processingContext.Pagination.EnterIgnorePageBreak(null, ignoreAlways: true);
						m_processingContext.Pagination.EnterIgnoreHeight(startHidden: true);
						MatrixHeading obj = (MatrixHeading)base.PivotHeadingDef;
						obj.InOutermostSubtotalCell = true;
						matrixCellInstance.Content = m_firstPassCell.CreateInstance(cellReportItemDef, setupEnvironment: true, ignorePageBreaks: true, ignoreInstances: false);
						obj.InOutermostSubtotalCell = false;
						m_processingContext.Pagination.LeaveIgnoreHeight(startHidden: true);
						m_processingContext.Pagination.LeaveIgnorePageBreak(null, ignoreAlways: true);
						m_firstPassCell.ResetReportItemObjs();
					}
					else
					{
						cellReportItemDef.ProcessDrillthroughAction(m_processingContext, cellNonComputedUniqueNames);
						cellReportItemDef.ProcessNavigationAction(m_processingContext.NavigationInfo, cellNonComputedUniqueNames, ((Matrix)matrixInstance.ReportItemDef).CurrentPage);
					}
				}
				if (Report.ShowHideTypes.Interactive == m_processingContext.ShowHideType)
				{
					m_processingContext.IgnoreAllFromStart = false;
					m_processingContext.ExitGrouping();
				}
				m_processingContext.PageSectionContext.InMatrixSubtotal = inMatrixSubtotal;
			}

			internal void CreateSubtotalOrStaticCells(MatrixInstance matrixInstance, RuntimePivotGroupRootObj currOuterHeadingGroupRoot, bool outerGroupingSubtotal)
			{
				_ = (RuntimeMatrixHeadingsObj)((RuntimeMatrixGroupRootObj)m_hierarchyRoot).InnerGroupings;
				if (IsOuterGrouping() && !outerGroupingSubtotal)
				{
					CreateOutermostSubtotalCell(matrixInstance);
				}
				else
				{
					CreateInnerGroupingsOrCells(matrixInstance, currOuterHeadingGroupRoot);
				}
			}

			internal void SetReportItemObjs(ReportItemColInstance reportItemColInstance)
			{
				if (reportItemColInstance.ReportItemInstances != null)
				{
					for (int i = 0; i < reportItemColInstance.ReportItemInstances.Count; i++)
					{
						SetReportItemObjs(reportItemColInstance[i]);
					}
				}
			}

			private void SetReportItemObjs(ReportItemInstance reportItemInstance)
			{
				if (reportItemInstance is TextBoxInstance)
				{
					TextBox textBox = (TextBox)reportItemInstance.ReportItemDef;
					TextBoxInstance textBoxInstance = (TextBoxInstance)reportItemInstance;
					object obj = null;
					obj = ((!textBox.IsSimpleTextBox()) ? ((TextBoxInstanceInfo)textBoxInstance.InstanceInfo).OriginalValue : ((SimpleTextBoxInstanceInfo)textBoxInstance.InstanceInfo).OriginalValue);
					((TextBoxImpl)m_processingContext.ReportObjectModel.ReportItemsImpl[textBox.Name]).SetResult(new VariantResult(errorOccurred: false, obj));
				}
				else if (reportItemInstance is RectangleInstance)
				{
					SetReportItemObjs(((RectangleInstance)reportItemInstance).ReportItemColInstance);
				}
				else if (reportItemInstance is MatrixInstance)
				{
					SetReportItemObjs(((MatrixInstance)reportItemInstance).CornerContent);
				}
				else
				{
					if (!(reportItemInstance is TableInstance))
					{
						return;
					}
					TableInstance tableInstance = (TableInstance)reportItemInstance;
					if (tableInstance.HeaderRowInstances != null)
					{
						for (int i = 0; i < tableInstance.HeaderRowInstances.Length; i++)
						{
							SetReportItemObjs(tableInstance.HeaderRowInstances[i].TableRowReportItemColInstance);
						}
					}
					if (tableInstance.FooterRowInstances != null)
					{
						for (int j = 0; j < tableInstance.FooterRowInstances.Length; j++)
						{
							SetReportItemObjs(tableInstance.FooterRowInstances[j].TableRowReportItemColInstance);
						}
					}
				}
			}

			private void GetScopeValuesForOutermostSTCell(IHierarchyObj targetScopeObj, VariantList[] scopeValues, ref int index)
			{
				Pivot pivotDef = base.PivotDef;
				if (IsOuterGrouping())
				{
					RuntimePivotObj ownerPivot = GetOwnerPivot();
					ownerPivot.GetScopeValues(targetScopeObj, scopeValues, ref index);
					int dynamicHeadingCount = pivotDef.GetDynamicHeadingCount(outerGroupings: false);
					for (int i = 0; i < dynamicHeadingCount; i++)
					{
						Global.Tracer.Assert(index < scopeValues.Length, "Inner headings");
						scopeValues[index++] = null;
					}
					base.GetScopeValues((IHierarchyObj)ownerPivot, scopeValues, ref index);
					if (m_innerHierarchy != null)
					{
						dynamicHeadingCount = pivotDef.GetDynamicHeadingCount(outerGroupings: true);
						for (int j = base.HeadingLevel + 1; j < dynamicHeadingCount; j++)
						{
							Global.Tracer.Assert(index < scopeValues.Length, "Outer headings");
							scopeValues[index++] = null;
						}
					}
					return;
				}
				base.GetScopeValues(targetScopeObj, scopeValues, ref index);
				int dynamicHeadingCount2;
				if (m_innerHierarchy != null)
				{
					dynamicHeadingCount2 = pivotDef.GetDynamicHeadingCount(outerGroupings: false);
					for (int k = base.HeadingLevel + 1; k < dynamicHeadingCount2; k++)
					{
						Global.Tracer.Assert(index < scopeValues.Length, "Subtotal inner headings");
						scopeValues[index++] = null;
					}
				}
				dynamicHeadingCount2 = pivotDef.GetDynamicHeadingCount(outerGroupings: true);
				for (int l = 0; l < dynamicHeadingCount2; l++)
				{
					Global.Tracer.Assert(index < scopeValues.Length, "Subtotal outer headings");
					scopeValues[index++] = null;
				}
			}

			internal override void GetScopeValues(IHierarchyObj targetScopeObj, VariantList[] scopeValues, ref int index)
			{
				if (((MatrixHeading)base.PivotHeadingDef).InOutermostSubtotalCell)
				{
					GetScopeValuesForOutermostSTCell(targetScopeObj, scopeValues, ref index);
				}
				else
				{
					base.GetScopeValues(targetScopeObj, scopeValues, ref index);
				}
			}
		}

		internal sealed class RuntimeExpressionInfo
		{
			private ExpressionInfo m_expression;

			private bool m_direction = true;

			private IndexedExprHost m_expressionsHost;

			private int m_expressionIndex;

			internal ExpressionInfo Expression => m_expression;

			internal bool Direction => m_direction;

			internal IndexedExprHost ExpressionsHost => m_expressionsHost;

			internal int ExpressionIndex => m_expressionIndex;

			internal RuntimeExpressionInfo(ExpressionInfoList expressions, IndexedExprHost expressionsHost, BoolList directions, int expressionIndex)
			{
				m_expressionsHost = expressionsHost;
				m_expressionIndex = expressionIndex;
				m_expression = expressions[m_expressionIndex];
				if (directions != null)
				{
					m_direction = directions[m_expressionIndex];
				}
			}
		}

		internal sealed class RuntimeExpressionInfoList : ArrayList
		{
			internal new RuntimeExpressionInfo this[int index] => (RuntimeExpressionInfo)base[index];

			internal RuntimeExpressionInfoList()
			{
			}
		}

		internal sealed class RuntimeDataRegionObjList : ArrayList
		{
			internal new RuntimeDataRegionObj this[int index]
			{
				get
				{
					return (RuntimeDataRegionObj)base[index];
				}
				set
				{
					base[index] = value;
				}
			}
		}

		internal sealed class RuntimeGroupRootObjList : Hashtable
		{
			internal RuntimeGroupRootObj this[string index]
			{
				get
				{
					return (RuntimeGroupRootObj)base[index];
				}
				set
				{
					base[index] = value;
				}
			}
		}

		private sealed class RuntimeGroupLeafObjList : ArrayList
		{
			internal new RuntimeGroupLeafObj this[int index] => (RuntimeGroupLeafObj)base[index];
		}

		private sealed class ParentInformation : Hashtable
		{
			internal new RuntimeGroupLeafObjList this[object parentKey] => (RuntimeGroupLeafObjList)base[parentKey];

			internal ParentInformation()
			{
			}
		}

		internal sealed class RuntimePivotCells : Hashtable
		{
			private RuntimePivotCell m_firstCell;

			private RuntimePivotCell m_lastCell;

			internal RuntimePivotCell this[int index]
			{
				get
				{
					return (RuntimePivotCell)base[index];
				}
				set
				{
					if (base.Count == 0)
					{
						m_firstCell = value;
					}
					base[index] = value;
				}
			}

			internal void Add(int key, RuntimePivotCell cell)
			{
				if (m_lastCell != null)
				{
					m_lastCell.NextCell = cell;
				}
				else
				{
					m_firstCell = cell;
				}
				m_lastCell = cell;
				base.Add(key, cell);
			}

			internal RuntimePivotCell GetCell(Pivot pivotDef, RuntimePivotGroupLeafObj owner, int cellLevel)
			{
				RuntimePivotGroupRootObj currentOuterHeadingGroupRoot = pivotDef.CurrentOuterHeadingGroupRoot;
				int index = pivotDef.OuterGroupingIndexes[currentOuterHeadingGroupRoot.HeadingLevel];
				RuntimePivotCell runtimePivotCell = this[index];
				if (runtimePivotCell == null)
				{
					runtimePivotCell = (this[index] = owner.CreateCell(cellLevel, pivotDef));
				}
				return runtimePivotCell;
			}

			internal void SortAndFilter()
			{
				for (RuntimePivotCell runtimePivotCell = m_firstCell; runtimePivotCell != null; runtimePivotCell = runtimePivotCell.NextCell)
				{
					runtimePivotCell.SortAndFilter();
				}
			}

			internal void CalculateRunningValues(Pivot pivotDef, AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup, RuntimePivotGroupLeafObj owner, int cellLevel)
			{
				RuntimePivotCell cell = GetCell(pivotDef, owner, cellLevel);
				Global.Tracer.Assert(cell != null, "(null != cell)");
				cell.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
			}
		}

		private sealed class Merge
		{
			private Report m_report;

			private ReportInstance m_reportInstance;

			private string m_reportLanguage;

			private ProcessingContext m_processingContext;

			private RuntimeDataSourceNodeList m_runtimeDataSourceNodes = new RuntimeDataSourceNodeList();

			private bool m_fetchImageStreams;

			private bool m_initialized;

			internal Merge(Report report, ProcessingContext context)
			{
				m_report = report;
				m_processingContext = context;
				m_fetchImageStreams = true;
			}

			internal Merge(Report report, ProcessingContext context, bool firstSubreportInstance)
			{
				m_report = report;
				m_processingContext = context;
				m_fetchImageStreams = firstSubreportInstance;
			}

			internal bool PrefetchData(ParameterInfoCollection parameters, bool mergeTran)
			{
				EventHandler eventHandler = null;
				try
				{
					bool flag = false;
					Init(parameters, prefetchDataOnly: true);
					EvaluateAndSetReportLanguage();
					if (m_report.DataSourceCount != 0)
					{
						int count = m_report.DataSources.Count;
						flag = true;
						for (int i = 0; i < count; i++)
						{
							m_runtimeDataSourceNodes.Add(new ReportRuntimeDataSourceNode(m_report, m_report.DataSources[i], m_processingContext));
						}
						eventHandler = AbortHandler;
						m_processingContext.AbortInfo.ProcessingAbortEvent += eventHandler;
						if (Global.Tracer.TraceVerbose)
						{
							Global.Tracer.Trace(TraceLevel.Verbose, "Abort handler registered.");
						}
						ThreadSet threadSet = null;
						RuntimeDataSourceNode runtimeDataSourceNode;
						if (count > 1)
						{
							threadSet = new ThreadSet(count - 1);
							for (int j = 1; j < count; j++)
							{
								runtimeDataSourceNode = m_runtimeDataSourceNodes[j];
								runtimeDataSourceNode.InitProcessingParams(mergeTran, prefetchDataOnly: true);
								m_processingContext.JobContext.TryQueueWorkItem(runtimeDataSourceNode.ProcessConcurrent, threadSet);
							}
						}
						runtimeDataSourceNode = m_runtimeDataSourceNodes[0];
						runtimeDataSourceNode.InitProcessingParams(mergeTran, prefetchDataOnly: true);
						runtimeDataSourceNode.ProcessConcurrent(null);
						m_processingContext.CheckAndThrowIfAborted();
						if (count > 1)
						{
							threadSet.WaitForCompletion();
						}
						m_processingContext.CheckAndThrowIfAborted();
						for (int k = 0; k < count; k++)
						{
							runtimeDataSourceNode = m_runtimeDataSourceNodes[k];
							if (flag)
							{
								flag = runtimeDataSourceNode.NoRows;
							}
						}
					}
					if (m_report.ParametersNotUsedInQuery && m_processingContext.ErrorSavingSnapshotData)
					{
						for (int l = 0; l < parameters.Count; l++)
						{
							parameters[l].UsedInQuery = true;
						}
						return false;
					}
				}
				finally
				{
					if (eventHandler != null)
					{
						m_processingContext.AbortInfo.ProcessingAbortEvent -= eventHandler;
					}
					if (m_report.DataSources != null && 0 < m_report.DataSources.Count)
					{
						for (int num = m_runtimeDataSourceNodes.Count - 1; num >= 0; num--)
						{
							RuntimeDataSourceNode runtimeDataSourceNode2 = m_runtimeDataSourceNodes[num];
							if (runtimeDataSourceNode2.DataProcessingDurationMs > m_processingContext.DataProcessingDurationMs)
							{
								m_processingContext.DataProcessingDurationMs = runtimeDataSourceNode2.DataProcessingDurationMs;
							}
						}
					}
					m_runtimeDataSourceNodes.Clear();
				}
				return true;
			}

			internal ReportInstance Process(ParameterInfoCollection parameters, bool mergeTran)
			{
				EventHandler eventHandler = null;
				try
				{
					bool flag = false;
					if (Global.Tracer.TraceVerbose)
					{
						Global.Tracer.Trace(TraceLevel.Verbose, "One Pass Processing? {0}", m_report.MergeOnePass ? "Yes" : "No");
					}
					ImageStreamNames imageStreamNames = m_report.ImageStreamNames;
					if (m_fetchImageStreams && imageStreamNames != null && 0 < imageStreamNames.Count)
					{
						ImageStreamNames imageStreamNames2 = new ImageStreamNames();
						IDictionaryEnumerator enumerator = imageStreamNames.GetEnumerator();
						while (enumerator.MoveNext())
						{
							string text = (string)enumerator.Key;
							Global.Tracer.Assert(text != null, "The URL to this image should not be null.");
							string mimeType = null;
							byte[] imageData = null;
							ImageInfo imageInfo = (ImageInfo)enumerator.Value;
							RuntimeRICollection.GetExternalImage(m_processingContext, text, ObjectType.Image, imageInfo.StreamName, out imageData, out mimeType);
							if (imageData != null && m_processingContext.CreateReportChunkCallback != null)
							{
								string text2 = Guid.NewGuid().ToString();
								using (Stream stream = m_processingContext.CreateReportChunkCallback(text2, ReportChunkTypes.Image, mimeType))
								{
									stream.Write(imageData, 0, imageData.Length);
								}
								imageStreamNames2[text] = new ImageInfo(text2, mimeType);
							}
						}
						m_report.ImageStreamNames = imageStreamNames2;
						m_processingContext.ImageStreamNames = imageStreamNames2;
					}
					Init(parameters, prefetchDataOnly: false);
					EvaluateAndSetReportLanguage();
					if (m_processingContext.ReportObjectModel.DataSourcesImpl == null)
					{
						m_processingContext.ReportObjectModel.DataSourcesImpl = new DataSourcesImpl(m_report.DataSourceCount);
					}
					if (m_report.DataSourceCount != 0)
					{
						int count = m_report.DataSources.Count;
						flag = true;
						for (int i = 0; i < count; i++)
						{
							m_runtimeDataSourceNodes.Add(new ReportRuntimeDataSourceNode(m_report, m_report.DataSources[i], m_processingContext));
						}
						eventHandler = AbortHandler;
						m_processingContext.AbortInfo.ProcessingAbortEvent += eventHandler;
						if (Global.Tracer.TraceVerbose)
						{
							Global.Tracer.Trace(TraceLevel.Verbose, "Abort handler registered.");
						}
						ThreadSet threadSet = null;
						RuntimeDataSourceNode runtimeDataSourceNode;
						if (count > 1)
						{
							threadSet = new ThreadSet(count - 1);
							for (int j = 1; j < count; j++)
							{
								runtimeDataSourceNode = m_runtimeDataSourceNodes[j];
								runtimeDataSourceNode.InitProcessingParams(mergeTran, prefetchDataOnly: false);
								m_processingContext.JobContext.TryQueueWorkItem(runtimeDataSourceNode.ProcessConcurrent, threadSet);
							}
						}
						runtimeDataSourceNode = m_runtimeDataSourceNodes[0];
						runtimeDataSourceNode.InitProcessingParams(mergeTran, prefetchDataOnly: false);
						runtimeDataSourceNode.ProcessConcurrent(null);
						m_processingContext.CheckAndThrowIfAborted();
						if (count > 1)
						{
							threadSet.WaitForCompletion();
						}
						if (Global.Tracer.TraceVerbose)
						{
							Global.Tracer.Trace(TraceLevel.Verbose, "The processing of all data sources has been completed.");
						}
						m_processingContext.CheckAndThrowIfAborted();
						for (int k = 0; k < count; k++)
						{
							runtimeDataSourceNode = m_runtimeDataSourceNodes[k];
							if (flag)
							{
								flag = runtimeDataSourceNode.NoRows;
							}
							if (m_processingContext.SaveSnapshotData && m_processingContext.ErrorSavingSnapshotData && m_processingContext.StopSaveSnapshotDataOnError)
							{
								runtimeDataSourceNode.EraseDataChunk();
							}
						}
					}
					if (m_report.ParametersNotUsedInQuery && m_processingContext.ErrorSavingSnapshotData)
					{
						for (int l = 0; l < parameters.Count; l++)
						{
							parameters[l].UsedInQuery = true;
						}
					}
					CreateInstances(parameters, flag);
					m_report.IntermediateFormatVersion.SetCurrent();
					m_report.LastID = m_processingContext.GetLastIDForReport();
					return m_reportInstance;
				}
				finally
				{
					if (eventHandler != null)
					{
						m_processingContext.AbortInfo.ProcessingAbortEvent -= eventHandler;
					}
					if (m_report.DataSources != null && 0 < m_report.DataSources.Count)
					{
						for (int num = m_runtimeDataSourceNodes.Count - 1; num >= 0; num--)
						{
							RuntimeDataSourceNode runtimeDataSourceNode2 = m_runtimeDataSourceNodes[num];
							if (runtimeDataSourceNode2.DataProcessingDurationMs > m_processingContext.DataProcessingDurationMs)
							{
								m_processingContext.DataProcessingDurationMs = runtimeDataSourceNode2.DataProcessingDurationMs;
							}
						}
					}
					if (m_processingContext.ReportRuntime != null)
					{
						m_processingContext.ReportRuntime.Close();
					}
					for (int m = 0; m < m_runtimeDataSourceNodes.Count; m++)
					{
						m_runtimeDataSourceNodes[m].Cleanup();
					}
				}
			}

			internal void CleanupDataChunk(UserProfileState userProfileState)
			{
				if (!m_processingContext.IsHistorySnapshot && m_processingContext.SaveSnapshotData && (!m_processingContext.ErrorSavingSnapshotData || (m_processingContext.ErrorSavingSnapshotData && !m_processingContext.StopSaveSnapshotDataOnError)) && !m_report.ParametersNotUsedInQuery && !m_processingContext.HasUserSortFilter && (userProfileState & UserProfileState.InReport) == 0)
				{
					for (int i = 0; i < m_report.DataSources.Count; i++)
					{
						m_runtimeDataSourceNodes[i].EraseDataChunk();
					}
				}
			}

			private void AbortHandler(object sender, EventArgs e)
			{
				if (((ProcessingAbortEventArgs)e).ReportUniqueName == m_processingContext.DataSetUniqueName)
				{
					if (Global.Tracer.TraceInfo)
					{
						Global.Tracer.Trace(TraceLevel.Info, "Merge abort handler called for ID={0}. Aborting data sources ...", m_processingContext.DataSetUniqueName);
					}
					int count = m_runtimeDataSourceNodes.Count;
					for (int i = 0; i < count; i++)
					{
						m_runtimeDataSourceNodes[i].Abort();
					}
				}
			}

			internal void Init(ParameterInfoCollection parameters, bool prefetchDataOnly)
			{
				if (m_processingContext.ReportObjectModel == null && m_processingContext.ReportRuntime == null)
				{
					m_processingContext.ReportObjectModel = new ObjectModelImpl(m_processingContext);
					m_processingContext.ReportRuntime = new ReportRuntime(m_processingContext.ReportObjectModel, m_processingContext.ErrorContext);
					m_processingContext.ReportObjectModel.ParametersImpl = new ParametersImpl(parameters.Count);
					if (parameters != null && parameters.Count > 0)
					{
						for (int i = 0; i < parameters.Count; i++)
						{
							ParameterInfo parameterInfo = parameters[i];
							m_processingContext.ReportObjectModel.ParametersImpl.Add(parameterInfo.Name, new ParameterImpl(parameterInfo.Values, parameterInfo.Labels, parameterInfo.MultiValue));
						}
					}
					m_processingContext.ReportObjectModel.GlobalsImpl = new GlobalsImpl(m_processingContext.ReportContext.ItemName, m_processingContext.ExecutionTime, m_processingContext.ReportContext.HostRootUri, m_processingContext.ReportContext.ParentPath);
					m_processingContext.ReportObjectModel.UserImpl = new UserImpl(m_processingContext.RequestUserName, m_processingContext.UserLanguage.Name, m_processingContext.AllowUserProfileState);
				}
				if (!prefetchDataOnly)
				{
					m_processingContext.ReportObjectModel.FieldsImpl = new FieldsImpl();
					int num = 0;
					for (int j = 0; j < m_report.DataSourceCount; j++)
					{
						DataSource dataSource = m_report.DataSources[j];
						if (dataSource.DataSets == null)
						{
							continue;
						}
						for (int k = 0; k < dataSource.DataSets.Count; k++)
						{
							if (!dataSource.DataSets[k].UsedOnlyInParameters)
							{
								num++;
							}
						}
					}
					m_processingContext.ReportObjectModel.AggregatesImpl = new AggregatesImpl(num > 1, m_processingContext.ReportRuntime);
					m_processingContext.ReportObjectModel.ReportItemsImpl = new ReportItemsImpl(num > 1);
					m_processingContext.ReportObjectModel.DataSetsImpl = new DataSetsImpl(num > 1, num);
				}
				if (!m_initialized)
				{
					m_initialized = true;
					if (m_processingContext.ReportRuntime.ReportExprHost == null)
					{
						m_processingContext.ReportRuntime.LoadCompiledCode(m_report, parametersOnly: false, m_processingContext.ReportObjectModel, m_processingContext.ReportRuntimeSetup);
					}
					if (m_processingContext.ReportRuntime.ReportExprHost != null)
					{
						m_report.SetExprHost(m_processingContext.ReportRuntime.ReportExprHost, m_processingContext.ReportObjectModel);
					}
					if (m_report.HasUserSortFilter)
					{
						m_processingContext.HasUserSortFilter = true;
					}
				}
			}

			private void CreateInstances(ParameterInfoCollection parameters, bool noRows)
			{
				int num = 0;
				RuntimeReportDataSetNode runtimeReportDataSetNode = null;
				RuntimeReportDataSetNode runtimeReportDataSetNode2 = null;
				if (m_runtimeDataSourceNodes != null)
				{
					for (int i = 0; i < m_runtimeDataSourceNodes.Count; i++)
					{
						RuntimeDataSourceNode runtimeDataSourceNode = m_runtimeDataSourceNodes[i];
						if (runtimeDataSourceNode == null || runtimeDataSourceNode.RuntimeDataSetNodes == null)
						{
							continue;
						}
						for (int j = 0; j < runtimeDataSourceNode.RuntimeDataSetNodes.Count; j++)
						{
							num++;
							runtimeReportDataSetNode = (RuntimeReportDataSetNode)runtimeDataSourceNode.RuntimeDataSetNodes[j];
							if (!runtimeReportDataSetNode.HasSortFilterInfo)
							{
								continue;
							}
							RuntimeSortFilterEventInfoList runtimeSortFilterInfo = runtimeReportDataSetNode.ProcessingContext.RuntimeSortFilterInfo;
							if (runtimeSortFilterInfo != null)
							{
								if (m_processingContext.ReportRuntimeUserSortFilterInfo == null)
								{
									m_processingContext.ReportRuntimeUserSortFilterInfo = new RuntimeSortFilterEventInfoList();
								}
								for (int k = 0; k < runtimeSortFilterInfo.Count; k++)
								{
									m_processingContext.ReportRuntimeUserSortFilterInfo.Add(runtimeSortFilterInfo[k]);
								}
							}
						}
					}
				}
				if (1 == num && runtimeReportDataSetNode != null)
				{
					m_processingContext.ReportObjectModel.FieldsImpl.Clone(runtimeReportDataSetNode.Fields);
					runtimeReportDataSetNode2 = runtimeReportDataSetNode;
				}
				m_reportInstance = new ReportInstance(m_processingContext, m_report, parameters, m_reportLanguage, noRows);
				if (m_report.HasReportItemReferences || m_processingContext.ReportRuntime.ReportExprHost != null)
				{
					m_processingContext.RuntimeInitializeReportItemObjs(m_report.ReportItems, traverseDataRegions: false, setValue: false);
				}
				m_processingContext.ReportRuntime.CurrentScope = runtimeReportDataSetNode2;
				RuntimeRICollection runtimeRICollection = new RuntimeRICollection(runtimeReportDataSetNode2, m_report.ReportItems, m_processingContext, createDataRegions: false);
				if (runtimeReportDataSetNode2 != null)
				{
					m_processingContext.UserSortFilterContext.UpdateContextFromDataSet(runtimeReportDataSetNode2.ProcessingContext.UserSortFilterContext);
				}
				m_processingContext.Pagination.SetReportItemStartPage(m_report, softPageAtStart: false);
				runtimeRICollection.CreateInstances(m_reportInstance.ReportItemColInstance);
				m_processingContext.Pagination.ProcessEndPage(m_reportInstance, m_report, pageBreakAtEnd: false, childrenOnThisPage: false);
				m_reportInstance.NumberOfPages = m_report.EndPage + 1;
			}

			private void EvaluateAndSetReportLanguage()
			{
				CultureInfo language = null;
				if (m_report.Language != null)
				{
					if (m_report.Language.Type != ExpressionInfo.Types.Constant)
					{
						m_processingContext.LanguageInstanceId += 1;
						m_reportLanguage = m_processingContext.ReportRuntime.EvaluateReportLanguageExpression(m_report, out language);
					}
					else
					{
						Exception ex = null;
						try
						{
							language = new CultureInfo(m_report.Language.Value, useUserOverride: false);
						}
						catch (Exception ex2)
						{
							language = null;
							ex = ex2;
						}
						if (language != null && language.IsNeutralCulture)
						{
							try
							{
								language = CultureInfo.CreateSpecificCulture(m_report.Language.Value);
								language = new CultureInfo(language.Name, useUserOverride: false);
							}
							catch (Exception ex3)
							{
								language = null;
								ex = ex3;
							}
						}
						if (ex != null)
						{
							m_processingContext.ErrorContext.Register(ProcessingErrorCode.rsInvalidLanguage, Severity.Warning, ObjectType.Report, m_report.Name, "Language", ex.Message);
						}
					}
				}
				if (language == null && m_processingContext.SubReportLevel == 0)
				{
					language = Localization.DefaultReportServerSpecificCulture;
				}
				if (language != null)
				{
					Thread.CurrentThread.CurrentCulture = language;
					m_processingContext.ThreadCulture = language;
				}
			}
		}

		private sealed class RuntimeDataSourceNodeList : ArrayList
		{
			internal new RuntimeDataSourceNode this[int index] => (RuntimeDataSourceNode)base[index];
		}

		internal abstract class RuntimeDataSourceNode
		{
			protected bool m_noRows = true;

			protected ProcessingContext m_processingContext;

			protected Report m_report;

			protected DataSource m_dataSource;

			protected RuntimeDataSetNodeList m_runtimeDataSetNodes = new RuntimeDataSetNodeList();

			protected bool m_canAbort;

			protected long m_dataProcessingDurationMs;

			protected int m_dataSetIndex = -1;

			internal long DataProcessingDurationMs => m_dataProcessingDurationMs;

			internal bool NoRows => m_noRows;

			internal RuntimeDataSetNodeList RuntimeDataSetNodes => m_runtimeDataSetNodes;

			internal RuntimeDataSourceNode(Report report, DataSource dataSource, ProcessingContext processingContext)
			{
				m_report = report;
				m_dataSource = dataSource;
				m_processingContext = processingContext;
			}

			internal RuntimeDataSourceNode(Report report, DataSource dataSource, int dataSetIndex, ProcessingContext processingContext)
			{
				m_report = report;
				m_dataSource = dataSource;
				m_processingContext = processingContext;
				m_dataSetIndex = dataSetIndex;
			}

			internal void Abort()
			{
				if (Global.Tracer.TraceVerbose)
				{
					Global.Tracer.Trace(TraceLevel.Verbose, "Data source '{0}': Abort handler called. CanAbort = {1}.", m_dataSource.Name.MarkAsModelInfo(), m_canAbort);
				}
				if (m_canAbort)
				{
					int count = m_runtimeDataSetNodes.Count;
					for (int i = 0; i < count; i++)
					{
						m_runtimeDataSetNodes[i].Abort();
					}
				}
			}

			internal void Cleanup()
			{
				for (int i = 0; i < m_runtimeDataSetNodes.Count; i++)
				{
					m_runtimeDataSetNodes[i].Cleanup();
				}
			}

			internal virtual void InitProcessingParams(bool mergeTran, bool prefetchDataOnly)
			{
			}

			internal void ProcessConcurrent(object threadSet)
			{
				CultureInfo cultureInfo = null;
				Global.Tracer.Assert(m_dataSource.Name != null, "The name of a data source cannot be null.");
				try
				{
					if (Global.Tracer.TraceVerbose)
					{
						Global.Tracer.Trace(TraceLevel.Verbose, "Thread has started processing data source '{0}'", m_dataSource.Name.MarkAsModelInfo());
					}
					if (m_processingContext.ThreadCulture != null)
					{
						cultureInfo = Thread.CurrentThread.CurrentCulture;
						Thread.CurrentThread.CurrentCulture = m_processingContext.ThreadCulture;
					}
					Process();
				}
				catch (ProcessingAbortedException)
				{
					if (Global.Tracer.TraceWarning)
					{
						Global.Tracer.Trace(TraceLevel.Warning, "Data source '{0}': Report processing has been aborted.", m_dataSource.Name.MarkAsModelInfo());
					}
				}
				catch (Exception ex2)
				{
					if (Global.Tracer.TraceError)
					{
						Global.Tracer.Trace(TraceLevel.Error, "An exception has occurred in data source '{0}'. Details: {1}", m_dataSource.Name.MarkAsModelInfo(), ex2.ToString());
					}
					if (m_processingContext.AbortInfo != null)
					{
						m_processingContext.AbortInfo.SetError(m_processingContext.DataSetUniqueName, ex2);
					}
				}
				finally
				{
					if (Global.Tracer.TraceVerbose)
					{
						Global.Tracer.Trace(TraceLevel.Verbose, "Processing of data source '{0}' completed.", m_dataSource.Name.MarkAsModelInfo());
					}
					if (cultureInfo != null)
					{
						Thread.CurrentThread.CurrentCulture = cultureInfo;
					}
					(threadSet as ThreadSet)?.ThreadCompleted();
				}
			}

			protected abstract void Process();

			internal void EraseDataChunk()
			{
				for (int i = 0; i < m_runtimeDataSetNodes.Count; i++)
				{
					m_runtimeDataSetNodes[i].EraseDataChunk();
				}
			}
		}

		internal sealed class ReportRuntimeDataSourceNode : RuntimeDataSourceNode
		{
			private enum ConnectionSecurity
			{
				UseIntegratedSecurity,
				ImpersonateWindowsUser,
				UseDataSourceCredentials,
				None
			}

			private bool m_mergeTran;

			private bool m_prefetchDataOnly;

			private LegacyReportParameterDataSetCache m_cache;

			internal ReportRuntimeDataSourceNode(Report report, DataSource dataSource, ProcessingContext processingContext)
				: base(report, dataSource, processingContext)
			{
			}

			internal ReportRuntimeDataSourceNode(Report report, DataSource dataSource, int dataSetIndex, ProcessingContext processingContext, LegacyReportParameterDataSetCache aCache)
				: base(report, dataSource, dataSetIndex, processingContext)
			{
				m_cache = aCache;
			}

			internal override void InitProcessingParams(bool mergeTran, bool prefetchDataOnly)
			{
				m_mergeTran = mergeTran;
				m_prefetchDataOnly = prefetchDataOnly;
			}

			protected override void Process()
			{
				if (m_dataSource.DataSets == null || 0 >= m_dataSource.DataSets.Count)
				{
					return;
				}
				IDbConnection dbConnection = null;
				TransactionInfo transactionInfo = null;
				bool flag = false;
				int num = 1;
				DataSet dataSet = null;
				bool flag2 = false;
				bool flag3 = false;
				if (m_processingContext.ProcessReportParameters)
				{
					m_runtimeDataSetNodes.Add(new RuntimeReportParametersDataSetNode(m_report, m_dataSource.DataSets[m_dataSetIndex], m_processingContext, m_cache));
				}
				else
				{
					num = m_dataSource.DataSets.Count;
					for (int i = 0; i < num; i++)
					{
						dataSet = m_dataSource.DataSets[i];
						if (!dataSet.UsedOnlyInParameters)
						{
							RuntimeDataSetNode value = (!m_prefetchDataOnly) ? ((RuntimeDataSetNode)new RuntimeReportDataSetNode(m_report, dataSet, m_processingContext)) : ((RuntimeDataSetNode)new RuntimePrefetchDataSetNode(m_report, dataSet, m_processingContext));
							m_runtimeDataSetNodes.Add(value);
						}
					}
				}
				num = m_runtimeDataSetNodes.Count;
				if (0 >= num)
				{
					m_noRows = false;
					return;
				}
				m_canAbort = true;
				m_processingContext.CheckAndThrowIfAborted();
				try
				{
					bool flag4 = num > 1 && (!m_processingContext.UserSortFilterProcessing || m_processingContext.SubReportLevel == 0 || !m_report.HasUserSortFilter);
					if (!m_processingContext.SnapshotProcessing && !m_processingContext.ProcessWithCachedData)
					{
						if (m_dataSource.Transaction && m_mergeTran)
						{
							DataSourceInfo dataSourceInfo = m_processingContext.GlobalDataSourceInfo[m_dataSource.Name];
							if (dataSourceInfo != null)
							{
								dbConnection = dataSourceInfo.Connection;
								transactionInfo = dataSourceInfo.TransactionInfo;
							}
						}
						if (Global.Tracer.TraceVerbose)
						{
							Global.Tracer.Trace(TraceLevel.Verbose, "Data source '{0}': Transaction = {1}, MergeTran = {2}, NumDataSets = {3}", m_dataSource.Name.MarkAsModelInfo(), m_dataSource.Transaction, m_mergeTran, num);
						}
						if (dbConnection == null)
						{
							ReportProcessingContext reportProcessingContext = (ReportProcessingContext)m_processingContext;
							Microsoft.ReportingServices.DataExtensions.DataSourceInfo dataSourceInfo2;
							string connectionString = m_dataSource.ResolveConnectionString(reportProcessingContext, out dataSourceInfo2);
							dbConnection = reportProcessingContext.DataExtensionConnection.OpenDataSourceExtensionConnection(m_dataSource, connectionString, dataSourceInfo2, null);
							if (Global.Tracer.TraceVerbose)
							{
								Global.Tracer.Trace(TraceLevel.Verbose, "Data source '{0}': Created a connection.", m_dataSource.Name.MarkAsModelInfo());
							}
							flag = true;
						}
						if (m_dataSource.Transaction)
						{
							if (transactionInfo == null)
							{
								IDbTransaction transaction = dbConnection.BeginTransaction();
								if (Global.Tracer.TraceVerbose)
								{
									Global.Tracer.Trace(TraceLevel.Verbose, "Data source '{0}': Begun a transaction.", m_dataSource.Name.MarkAsModelInfo());
								}
								transactionInfo = new TransactionInfo(transaction);
								flag2 = true;
							}
							flag3 = ((transactionInfo.Transaction as IDbTransactionExtension)?.AllowMultiConnection ?? false);
							flag4 = (flag4 && flag3);
							if (Global.Tracer.TraceVerbose)
							{
								Global.Tracer.Trace(TraceLevel.Verbose, "Data source '{0}': TransactionCanSpanConnections = {1}, ConcurrentDataSets = {2}", m_dataSource.Name.MarkAsModelInfo(), flag3, flag4);
							}
						}
					}
					if (!m_prefetchDataOnly)
					{
						m_processingContext.ReportObjectModel.DataSourcesImpl.Add(m_dataSource);
					}
					if (!m_processingContext.SnapshotProcessing && !m_processingContext.ProcessWithCachedData && dbConnection is IDbCollationProperties && NeedAutoDetectCollation(out int _))
					{
						try
						{
							if (((IDbCollationProperties)dbConnection).GetCollationProperties(out string cultureName, out bool caseSensitive, out bool accentSensitive, out bool kanatypeSensitive, out bool widthSensitive))
							{
								for (int j = 0; j < m_dataSource.DataSets.Count; j++)
								{
									m_dataSource.DataSets[j].MergeCollationSettings(m_processingContext.ErrorContext, m_dataSource.Type, cultureName, caseSensitive, accentSensitive, kanatypeSensitive, widthSensitive);
								}
							}
						}
						catch (Exception ex)
						{
							m_processingContext.ErrorContext.Register(ProcessingErrorCode.rsCollationDetectionFailed, Severity.Warning, ObjectType.DataSource, m_dataSource.Name.MarkAsModelInfo(), "Collation", ex.ToString());
						}
					}
					if (flag4)
					{
						ThreadSet threadSet = new ThreadSet(num - 1);
						RuntimeDataSetNode runtimeDataSetNode;
						for (int k = 1; k < num; k++)
						{
							runtimeDataSetNode = m_runtimeDataSetNodes[k];
							runtimeDataSetNode.InitProcessingParams(m_dataSource, null, transactionInfo);
							m_processingContext.JobContext.TryQueueWorkItem(runtimeDataSetNode.ProcessConcurrent, threadSet);
						}
						runtimeDataSetNode = m_runtimeDataSetNodes[0];
						runtimeDataSetNode.InitProcessingParams(m_dataSource, dbConnection, transactionInfo);
						runtimeDataSetNode.ProcessConcurrent(null);
						m_processingContext.CheckAndThrowIfAborted();
						threadSet.WaitForCompletion();
						if (m_processingContext.JobContext != null)
						{
							for (int l = 0; l < num; l++)
							{
								runtimeDataSetNode = m_runtimeDataSetNodes[l];
								if (runtimeDataSetNode.DataProcessingDurationMs > m_dataProcessingDurationMs)
								{
									m_dataProcessingDurationMs = runtimeDataSetNode.DataProcessingDurationMs;
								}
							}
						}
					}
					else
					{
						for (int m = 0; m < num; m++)
						{
							m_processingContext.CheckAndThrowIfAborted();
							RuntimeDataSetNode runtimeDataSetNode = m_runtimeDataSetNodes[m];
							runtimeDataSetNode.InitProcessingParams(m_dataSource, dbConnection, transactionInfo);
							runtimeDataSetNode.ProcessConcurrent(null);
							m_dataProcessingDurationMs += runtimeDataSetNode.DataProcessingDurationMs;
						}
					}
					if (Global.Tracer.TraceVerbose)
					{
						Global.Tracer.Trace(TraceLevel.Verbose, "Data source '{0}': Processing of all data sets completed.", m_dataSource.Name.MarkAsModelInfo());
					}
					m_processingContext.CheckAndThrowIfAborted();
					long num2 = 0L;
					m_noRows = true;
					for (int n = 0; n < num; n++)
					{
						if (!m_runtimeDataSetNodes[n].NoRows)
						{
							m_noRows = false;
						}
						num2 += m_runtimeDataSetNodes[n].NumRowsRead;
					}
					IJobContext jobContext = m_processingContext.JobContext;
					if (jobContext != null)
					{
						lock (jobContext.SyncRoot)
						{
							jobContext.RowCount += num2;
						}
					}
					if (!flag2)
					{
						return;
					}
					if (!m_report.SubReportMergeTransactions || m_processingContext.ProcessReportParameters)
					{
						if (Global.Tracer.TraceVerbose)
						{
							Global.Tracer.Trace(TraceLevel.Verbose, "Data source '{0}': Committing transaction.", m_dataSource.Name.MarkAsModelInfo());
						}
						try
						{
							transactionInfo.Transaction.Commit();
						}
						catch (Exception innerException)
						{
							throw new ReportProcessingException(ErrorCode.rsErrorCommitTransaction, innerException, m_dataSource.Name.MarkAsModelInfo());
						}
					}
					else
					{
						IDbConnection connection;
						if (flag3)
						{
							connection = null;
						}
						else
						{
							connection = dbConnection;
							flag = false;
						}
						if (Global.Tracer.TraceVerbose)
						{
							Global.Tracer.Trace(TraceLevel.Verbose, "Data source '{0}': Storing trans+conn into GlobalDataSourceInfo. CloseConnection = {1}.", m_dataSource.Name.MarkAsModelInfo(), flag);
						}
						m_processingContext.GlobalDataSourceInfo.Add(m_dataSource, connection, transactionInfo, null);
					}
					flag2 = false;
					transactionInfo = null;
				}
				catch (Exception ex2)
				{
					if (!(ex2 is ProcessingAbortedException) && Global.Tracer.TraceError)
					{
						Global.Tracer.Trace(TraceLevel.Error, "Data source '{0}': An error has occurred. Details: {1}", m_dataSource.Name.MarkAsModelInfo(), ex2.ToString());
					}
					if (transactionInfo != null)
					{
						transactionInfo.RollbackRequired = true;
					}
					throw ex2;
				}
				finally
				{
					if (flag2 && transactionInfo.RollbackRequired)
					{
						if (Global.Tracer.TraceError)
						{
							Global.Tracer.Trace(TraceLevel.Error, "Data source '{0}': Rolling the transaction back.", m_dataSource.Name.MarkAsModelInfo());
						}
						try
						{
							transactionInfo.Transaction.Rollback();
						}
						catch (Exception innerException2)
						{
							throw new ReportProcessingException(ErrorCode.rsErrorRollbackTransaction, innerException2, m_dataSource.Name.MarkAsModelInfo());
						}
					}
					if (flag)
					{
						try
						{
							ReportProcessingContext reportProcessingContext2 = m_processingContext as ReportProcessingContext;
							Global.Tracer.Assert(reportProcessingContext2 != null, "rptContext == null in closeConnection");
							reportProcessingContext2.DataExtensionConnection.CloseConnectionWithoutPool(dbConnection);
						}
						catch (Exception innerException3)
						{
							throw new ReportProcessingException(ErrorCode.rsErrorClosingConnection, innerException3, m_dataSource.Name.MarkAsModelInfo());
						}
					}
				}
			}

			private bool NeedAutoDetectCollation(out int index)
			{
				Global.Tracer.Assert(m_dataSource.DataSets != null, "(null != m_dataSource.DataSets)");
				bool flag = false;
				int count = m_dataSource.DataSets.Count;
				index = 0;
				if (m_processingContext.ProcessReportParameters && m_dataSource.DataSets[m_dataSetIndex].NeedAutoDetectCollation())
				{
					flag = true;
					index = m_dataSetIndex;
				}
				else
				{
					while (index < count && !flag)
					{
						DataSet dataSet = m_dataSource.DataSets[index];
						if (!dataSet.UsedOnlyInParameters && dataSet.NeedAutoDetectCollation())
						{
							flag = true;
						}
						else
						{
							index++;
						}
					}
				}
				return flag;
			}
		}

		internal sealed class RuntimeDataSetNodeList : ArrayList
		{
			internal new RuntimeDataSetNode this[int index] => (RuntimeDataSetNode)base[index];
		}

		internal abstract class RuntimeDataSetNode : IFilterOwner
		{
			protected DataSource m_dataSource;

			protected IDbConnection m_dataSourceConnection;

			protected TransactionInfo m_transInfo;

			protected Report m_report;

			protected DataSet m_dataSet;

			protected IDbCommand m_command;

			protected ProcessingDataReader m_dataReader;

			protected ProcessingContext m_processingContext;

			protected int m_dataRowsRead;

			protected long m_dataProcessingDurationMs;

			private Hashtable[] m_fieldAliasPropertyNames;

			protected Hashtable[] m_referencedAliasPropertyNames;

			protected bool m_foundExtendedProperties;

			protected bool m_hasSortFilterInfo;

			internal DataSet DataSet => m_dataSet;

			internal bool NoRows => m_dataRowsRead <= 0;

			internal int NumRowsRead => m_dataRowsRead;

			internal long DataProcessingDurationMs => m_dataProcessingDurationMs;

			internal FieldsImpl Fields
			{
				get
				{
					if (m_processingContext != null && m_processingContext.ReportObjectModel != null)
					{
						return m_processingContext.ReportObjectModel.FieldsImpl;
					}
					return null;
				}
			}

			internal ProcessingContext ProcessingContext => m_processingContext;

			internal RuntimeDataSetNode(Report report, DataSet dataSet, ProcessingContext processingContext)
			{
				m_report = report;
				m_dataSet = dataSet;
				m_processingContext = processingContext.CloneContext(processingContext);
			}

			internal void Abort()
			{
				IDbCommand command = m_command;
				if (command != null)
				{
					if (Global.Tracer.TraceVerbose)
					{
						Global.Tracer.Trace(TraceLevel.Verbose, "Data set '{0}': Cancelling command.", m_dataSet.Name.MarkAsPrivate());
					}
					command.Cancel();
				}
			}

			internal void Cleanup()
			{
				if (m_processingContext.ReportRuntime != null)
				{
					m_processingContext.ReportRuntime.Close();
				}
			}

			internal void InitProcessingParams(DataSource dataSource, IDbConnection conn, TransactionInfo transInfo)
			{
				m_dataSource = dataSource;
				m_dataSourceConnection = conn;
				m_transInfo = transInfo;
			}

			internal void ProcessConcurrent(object threadSet)
			{
				CultureInfo cultureInfo = null;
				Global.Tracer.Assert(m_dataSet.Name != null, "The name of a data set cannot be null.");
				try
				{
					if (Global.Tracer.TraceVerbose)
					{
						Global.Tracer.Trace(TraceLevel.Verbose, "Thread has started processing data set '{0}'", m_dataSet.Name.MarkAsPrivate());
					}
					if (m_processingContext.ThreadCulture != null)
					{
						cultureInfo = Thread.CurrentThread.CurrentCulture;
						Thread.CurrentThread.CurrentCulture = m_processingContext.ThreadCulture;
					}
					if (DataSetValidator.LOCALE_SYSTEM_DEFAULT == m_dataSet.LCID)
					{
						m_dataSet.LCID = DataSetValidator.LCIDfromRDLCollation(m_dataSet.Collation);
					}
					m_processingContext.CompareInfo = new CultureInfo((int)m_dataSet.LCID, useUserOverride: false).CompareInfo;
					m_processingContext.ClrCompareOptions = m_dataSet.GetCLRCompareOptions();
					Process();
				}
				catch (ProcessingAbortedException)
				{
					if (Global.Tracer.TraceWarning)
					{
						Global.Tracer.Trace(TraceLevel.Warning, "Data set '{0}': Report processing has been aborted.", m_dataSet.Name.MarkAsPrivate());
					}
				}
				catch (Exception ex2)
				{
					if (Global.Tracer.TraceError)
					{
						Global.Tracer.Trace(TraceLevel.Error, "An exception has occurred in data source '{0}'. Details: {1}", m_dataSet.Name.MarkAsPrivate(), ex2.ToString());
					}
					if (m_processingContext.AbortInfo != null)
					{
						m_processingContext.AbortInfo.SetError(m_processingContext.DataSetUniqueName, ex2);
					}
				}
				finally
				{
					if (Global.Tracer.TraceVerbose)
					{
						Global.Tracer.Trace(TraceLevel.Verbose, "Processing of data set '{0}' completed.", m_dataSet.Name.MarkAsPrivate());
					}
					if (cultureInfo != null)
					{
						Thread.CurrentThread.CurrentCulture = cultureInfo;
					}
					(threadSet as ThreadSet)?.ThreadCompleted();
				}
			}

			protected void InitRuntime(bool processReport)
			{
				Global.Tracer.Assert(m_processingContext.ReportObjectModel != null && m_processingContext.ReportRuntime != null);
				if (m_processingContext.ReportRuntime.ReportExprHost != null)
				{
					m_dataSet.SetExprHost(m_processingContext.ReportRuntime.ReportExprHost, m_processingContext.ReportObjectModel);
				}
				DataFieldList fields = m_dataSet.Fields;
				int num = fields?.Count ?? 0;
				m_processingContext.ReportObjectModel.FieldsImpl = new FieldsImpl(num, processReport && m_dataSet.HasDetailUserSortFilter);
				for (int i = 0; i < num; i++)
				{
					Field field = fields[i];
					if (m_dataSet.ExprHost != null)
					{
						field.SetExprHost(m_dataSet.ExprHost, m_processingContext.ReportObjectModel);
					}
					m_processingContext.ReportObjectModel.FieldsImpl.Add(field.Name, null);
				}
				if (processReport && m_dataSet.HasDetailUserSortFilter)
				{
					m_processingContext.ReportObjectModel.FieldsImpl.AddRowIndexField();
				}
				if (m_dataSet.Filters != null && m_dataSet.ExprHost != null)
				{
					int count = m_dataSet.Filters.Count;
					for (int j = 0; j < count; j++)
					{
						m_dataSet.Filters[j].SetExprHost(m_dataSet.ExprHost.FilterHostsRemotable, m_processingContext.ReportObjectModel);
					}
				}
				if (m_processingContext.ReportRuntime.ReportExprHost != null)
				{
					RuntimeInitializeReportItemObjs();
				}
				RegisterAggregates();
			}

			protected abstract void Process();

			protected abstract void FirstPassProcessDetailRow(Filters filters);

			protected abstract bool FirstPassGetNextDetailRow();

			protected abstract void FirstPassInit();

			protected abstract void NextNonAggregateRow();

			internal virtual void RuntimeInitializeReportItemObjs()
			{
			}

			internal virtual void EraseDataChunk()
			{
			}

			protected virtual void RegisterAggregates()
			{
			}

			protected virtual void FirstPassCleanup(bool flushData)
			{
			}

			void IFilterOwner.PostFilterNextRow()
			{
				NextNonAggregateRow();
			}

			protected void FirstPassProcess(ref bool closeConnWhenFinish)
			{
				if (m_dataSourceConnection == null && !m_processingContext.SnapshotProcessing && !m_processingContext.ProcessWithCachedData)
				{
					closeConnWhenFinish = true;
				}
				FirstPassInit();
				m_processingContext.CheckAndThrowIfAborted();
				FirstPass();
				m_processingContext.CheckAndThrowIfAborted();
				if (closeConnWhenFinish)
				{
					Global.Tracer.Assert(m_dataSourceConnection != null, "(null != m_dataSourceConnection)");
					try
					{
						m_processingContext.DataExtensionConnection.CloseConnectionWithoutPool(m_dataSourceConnection);
					}
					catch (Exception innerException)
					{
						throw new ReportProcessingException(ErrorCode.rsErrorClosingConnection, innerException, m_dataSource.Name.MarkAsModelInfo());
					}
					m_dataSourceConnection = null;
				}
			}

			private void FirstPass()
			{
				Filters filters = null;
				if (m_dataSet.Filters != null)
				{
					filters = new Filters(Filters.FilterTypes.DataSetFilter, this, m_dataSet.Filters, m_dataSet.ObjectType, m_dataSet.Name, m_processingContext);
				}
				bool flushData = false;
				try
				{
					m_dataRowsRead = 0;
					while (FirstPassGetNextDetailRow())
					{
						FirstPassProcessDetailRow(filters);
					}
					m_dataSet.RecordSetSize = m_dataRowsRead;
					flushData = true;
				}
				finally
				{
					if (m_dataReader != null)
					{
						((IDisposable)m_dataReader).Dispose();
						m_dataReader = null;
					}
					CloseCommand();
					FirstPassCleanup(flushData);
				}
				filters?.FinishReadingRows();
			}

			private void CloseCommand()
			{
				if (m_command != null)
				{
					IDbCommand command = m_command;
					m_command = null;
					command.Dispose();
				}
			}

			protected bool GetNextDetailRow()
			{
				bool result = false;
				bool flag = m_dataRowsRead == 0;
				Microsoft.ReportingServices.Diagnostics.Timer timer = null;
				if (m_processingContext.JobContext != null)
				{
					timer = new Microsoft.ReportingServices.Diagnostics.Timer();
					timer.StartTimer();
				}
				FieldsImpl fieldsImpl = null;
				if (m_dataReader != null && m_dataReader.GetNextRow())
				{
					fieldsImpl = m_processingContext.ReportObjectModel.FieldsImpl;
					if (flag)
					{
						m_fieldAliasPropertyNames = new Hashtable[fieldsImpl.Count];
						m_referencedAliasPropertyNames = new Hashtable[fieldsImpl.Count];
					}
					fieldsImpl.NewRow();
					if (fieldsImpl.ReaderExtensionsSupported && !m_dataSet.InterpretSubtotalsAsDetails)
					{
						fieldsImpl.IsAggregateRow = m_dataReader.IsAggregateRow;
						fieldsImpl.AggregationFieldCount = m_dataReader.AggregationFieldCount;
						if (!fieldsImpl.IsAggregateRow)
						{
							fieldsImpl.AggregationFieldCountForDetailRow = fieldsImpl.AggregationFieldCount;
						}
					}
					bool flag2 = false;
					for (int i = 0; i < fieldsImpl.Count; i++)
					{
						Field field = m_dataSet.Fields[i];
						if (field.IsCalculatedField)
						{
							CalculatedFieldWrapperImpl value = new CalculatedFieldWrapperImpl(field, m_processingContext.ReportRuntime);
							if (m_dataSet.InterpretSubtotalsAsDetails)
							{
								fieldsImpl[i] = new FieldImpl(value, isAggregationField: true, field);
							}
							else
							{
								fieldsImpl[i] = new FieldImpl(value, (!fieldsImpl.ReaderExtensionsSupported) ? true : false, field);
							}
							flag2 = true;
							continue;
						}
						Global.Tracer.Assert(!flag2, "(!inCalculatedFields)");
						try
						{
							if (flag || !fieldsImpl.IsFieldMissing(i))
							{
								if (m_dataSet.InterpretSubtotalsAsDetails)
								{
									fieldsImpl[i] = new FieldImpl(m_dataReader.GetColumn(i), isAggregationField: true, field);
								}
								else
								{
									fieldsImpl[i] = new FieldImpl(m_dataReader.GetColumn(i), !fieldsImpl.ReaderExtensionsSupported || m_dataReader.IsAggregationField(i), field);
								}
								if (!fieldsImpl.ReaderFieldProperties)
								{
									continue;
								}
								int num = 0;
								if (m_fieldAliasPropertyNames[i] != null)
								{
									num = m_fieldAliasPropertyNames[i].Count;
								}
								else
								{
									num = m_dataReader.GetPropertyCount(i);
									m_fieldAliasPropertyNames[i] = new Hashtable(num);
									m_referencedAliasPropertyNames[i] = new Hashtable(num);
									m_foundExtendedProperties = true;
								}
								for (int j = 0; j < num; j++)
								{
									string text = null;
									if (flag)
									{
										text = m_dataReader.GetPropertyName(i, j);
										m_fieldAliasPropertyNames[i].Add(j, text);
									}
									else
									{
										Global.Tracer.Assert(m_fieldAliasPropertyNames[i].ContainsKey(j), "(m_fieldAliasPropertyNames[i].ContainsKey(j))");
										text = (m_fieldAliasPropertyNames[i][j] as string);
									}
									if (m_processingContext.CacheDataCallback != null || m_dataSet.DynamicFieldReferences || field.DynamicPropertyReferences || (field.ReferencedProperties != null && field.ReferencedProperties.ContainsKey(text)))
									{
										if (flag)
										{
											m_referencedAliasPropertyNames[i].Add(j, text);
										}
										object propertyValue = m_dataReader.GetPropertyValue(i, j);
										fieldsImpl[i].SetProperty(text, propertyValue);
									}
								}
								continue;
							}
							fieldsImpl[i] = new FieldImpl(DataFieldStatus.IsMissing, null, field);
						}
						catch (ReportProcessingException_FieldError reportProcessingException_FieldError)
						{
							bool flag3 = false;
							if (m_dataRowsRead == 0 && DataFieldStatus.UnSupportedDataType != reportProcessingException_FieldError.Status && DataFieldStatus.Overflow != reportProcessingException_FieldError.Status)
							{
								fieldsImpl.SetFieldIsMissing(i);
								fieldsImpl[i] = new FieldImpl(DataFieldStatus.IsMissing, reportProcessingException_FieldError.Message, field);
								flag3 = true;
								m_processingContext.ErrorContext.Register(ProcessingErrorCode.rsMissingFieldInDataSet, Severity.Warning, ObjectType.DataSet, m_dataSet.Name, "Field", field.Name.MarkAsModelInfo());
							}
							if (!flag3)
							{
								fieldsImpl[i] = new FieldImpl(reportProcessingException_FieldError.Status, reportProcessingException_FieldError.Message, field);
							}
							if (!fieldsImpl.IsFieldErrorRegistered(i))
							{
								fieldsImpl.SetFieldErrorRegistered(i);
								if (DataFieldStatus.UnSupportedDataType == reportProcessingException_FieldError.Status)
								{
									m_processingContext.ErrorContext.Register(ProcessingErrorCode.rsDataSetFieldTypeNotSupported, Severity.Warning, ObjectType.DataSet, m_dataSet.Name, "Field", field.Name.MarkAsModelInfo());
								}
								else
								{
									m_processingContext.ErrorContext.Register(ProcessingErrorCode.rsErrorReadingDataSetField, Severity.Warning, ObjectType.DataSet, m_dataSet.Name, "Field", field.Name.MarkAsModelInfo(), reportProcessingException_FieldError.Message);
								}
							}
						}
					}
					m_dataRowsRead++;
					if (fieldsImpl.AddRowIndex)
					{
						fieldsImpl.SetRowIndex(m_dataRowsRead);
					}
					result = true;
				}
				if (m_processingContext.JobContext != null)
				{
					m_dataProcessingDurationMs += timer.ElapsedTimeMs();
				}
				return result;
			}

			protected virtual bool RunDataSetQuery()
			{
				bool result = false;
				bool flag = false;
				if (m_dataSet.Query == null)
				{
					return result;
				}
				ParameterValueList parameters = m_dataSet.Query.Parameters;
				object[] array = new object[parameters?.Count ?? 0];
				for (int i = 0; i < array.Length; i++)
				{
					ParameterValue parameterValue = parameters[i];
					m_processingContext.CheckAndThrowIfAborted();
					array[i] = m_processingContext.ReportRuntime.EvaluateQueryParamValue(parameterValue.Value, (m_dataSet.ExprHost != null) ? m_dataSet.ExprHost.QueryParametersHost : null, ObjectType.QueryParameter, parameterValue.Name);
				}
				m_processingContext.CheckAndThrowIfAborted();
				Microsoft.ReportingServices.Diagnostics.Timer timer = null;
				if (m_processingContext.JobContext != null)
				{
					timer = new Microsoft.ReportingServices.Diagnostics.Timer();
					timer.StartTimer();
				}
				IDataReader dataReader = null;
				IDbCommand dbCommand = null;
				try
				{
					if (m_dataSourceConnection == null)
					{
						ReportProcessingContext reportProcessingContext = (ReportProcessingContext)m_processingContext;
						Microsoft.ReportingServices.DataExtensions.DataSourceInfo dataSourceInfo;
						string connectionString = m_dataSource.ResolveConnectionString(reportProcessingContext, out dataSourceInfo);
						m_dataSourceConnection = reportProcessingContext.DataExtensionConnection.OpenDataSourceExtensionConnection(m_dataSource, connectionString, dataSourceInfo, null);
					}
					try
					{
						dbCommand = m_dataSourceConnection.CreateCommand();
					}
					catch (Exception innerException)
					{
						throw new ReportProcessingException(ErrorCode.rsErrorCreatingCommand, innerException, m_dataSource.Name.MarkAsModelInfo());
					}
					for (int j = 0; j < array.Length; j++)
					{
						IDataParameter dataParameter;
						try
						{
							dataParameter = dbCommand.CreateParameter();
						}
						catch (Exception innerException2)
						{
							throw new ReportProcessingException(ErrorCode.rsErrorCreatingQueryParameter, innerException2, m_dataSet.Name.MarkAsPrivate());
						}
						dataParameter.ParameterName = parameters[j].Name;
						bool flag2 = dataParameter is IDataMultiValueParameter && array[j] is ICollection;
						object obj = array[j];
						if (obj == null)
						{
							obj = DBNull.Value;
						}
						if (!(dataParameter is IDataMultiValueParameter) && array[j] is ICollection)
						{
							throw new ReportProcessingException(ErrorCode.rsErrorAddingMultiValueQueryParameter, null, m_dataSet.Name.MarkAsPrivate(), dataParameter.ParameterName.MarkAsPrivate());
						}
						if (flag2)
						{
							int count = ((ICollection)obj).Count;
							if (1 == count)
							{
								try
								{
									Global.Tracer.Assert(obj is object[], "(paramValue is object[])");
									dataParameter.Value = (obj as object[])[0];
								}
								catch (Exception innerException3)
								{
									throw new ReportProcessingException(ErrorCode.rsErrorAddingQueryParameter, innerException3, m_dataSource.Name.MarkAsModelInfo());
								}
							}
							else
							{
								object[] array2 = new object[count];
								((ICollection)obj).CopyTo(array2, 0);
								((IDataMultiValueParameter)dataParameter).Values = array2;
							}
						}
						else
						{
							try
							{
								dataParameter.Value = obj;
							}
							catch (Exception innerException4)
							{
								throw new ReportProcessingException(ErrorCode.rsErrorAddingQueryParameter, innerException4, m_dataSource.Name.MarkAsModelInfo());
							}
						}
						try
						{
							dbCommand.Parameters.Add(dataParameter);
						}
						catch (Exception innerException5)
						{
							throw new ReportProcessingException(ErrorCode.rsErrorAddingQueryParameter, innerException5, m_dataSource.Name.MarkAsModelInfo());
						}
					}
					m_processingContext.CheckAndThrowIfAborted();
					try
					{
						if (m_dataSet.Query.CommandText != null)
						{
							StringResult stringResult = m_processingContext.ReportRuntime.EvaluateCommandText(m_dataSet);
							if (stringResult.ErrorOccurred)
							{
								throw new ReportProcessingException(ErrorCode.rsQueryCommandTextProcessingError, m_dataSet.Name.MarkAsPrivate());
							}
							dbCommand.CommandText = stringResult.Value;
							m_dataSet.Query.CommandTextValue = stringResult.Value;
						}
					}
					catch (Exception innerException6)
					{
						throw new ReportProcessingException(ErrorCode.rsErrorSettingCommandText, innerException6, m_dataSet.Name.MarkAsPrivate());
					}
					try
					{
						dbCommand.CommandType = (CommandType)m_dataSet.Query.CommandType;
					}
					catch (Exception innerException7)
					{
						throw new ReportProcessingException(ErrorCode.rsErrorSettingCommandType, innerException7, m_dataSet.Name.MarkAsPrivate());
					}
					if (m_transInfo != null)
					{
						try
						{
							dbCommand.Transaction = m_transInfo.Transaction;
						}
						catch (Exception innerException8)
						{
							throw new ReportProcessingException(ErrorCode.rsErrorSettingTransaction, innerException8, m_dataSet.Name.MarkAsPrivate());
						}
					}
					m_processingContext.CheckAndThrowIfAborted();
					try
					{
						if (m_dataSet.Query.TimeOut == 0 && dbCommand is CommandWrapper && ((CommandWrapper)dbCommand).UnderlyingCommand is SqlCommand)
						{
							dbCommand.CommandTimeout = 2147483646;
						}
						else
						{
							dbCommand.CommandTimeout = m_dataSet.Query.TimeOut;
						}
					}
					catch (Exception innerException9)
					{
						throw new ReportProcessingException(ErrorCode.rsErrorSettingQueryTimeout, innerException9, m_dataSet.Name.MarkAsPrivate());
					}
					if (dbCommand is IDbCommandRewriter)
					{
						m_dataSet.Query.RewrittenCommandText = ((IDbCommandRewriter)dbCommand).RewrittenCommandText;
						m_processingContext.DrillthroughInfo.AddRewrittenCommand(m_dataSet.ID, m_dataSet.Query.RewrittenCommandText);
					}
					m_command = dbCommand;
					IJobContext jobContext = m_processingContext.JobContext;
					try
					{
						jobContext?.AddCommand(m_command);
						try
						{
							dataReader = m_command.ExecuteReader(CommandBehavior.SingleResult);
						}
						catch (Exception innerException10)
						{
							throw new ReportProcessingException(ErrorCode.rsErrorExecutingCommand, innerException10, m_dataSet.Name.MarkAsPrivate());
						}
					}
					finally
					{
						jobContext?.RemoveCommand(m_command);
					}
					if (dataReader == null)
					{
						if (Global.Tracer.TraceError)
						{
							Global.Tracer.Trace(TraceLevel.Error, "The source data reader is null. Cannot read results.");
						}
						throw new ReportProcessingException(ErrorCode.rsErrorCreatingDataReader, m_dataSet.Name.MarkAsPrivate());
					}
					result = (dataReader is IDataReaderExtension);
					flag = (dataReader is IDataReaderFieldProperties);
					if (dataReader.FieldCount > 0)
					{
						m_dataSet.CheckNonCalculatedFieldCount();
						DataFieldList fields = m_dataSet.Fields;
						int num = (fields != null) ? m_dataSet.NonCalculatedFieldCount : 0;
						string[] array3 = new string[num];
						string[] array4 = new string[num];
						for (int k = 0; k < num; k++)
						{
							Field field = fields[k];
							array3[k] = field.DataField;
							array4[k] = field.Name;
						}
						m_dataReader = new ProcessingDataReader(m_dataSet.Name, dataReader, array4, array3);
					}
					m_processingContext.ReportObjectModel.FieldsImpl.ReaderExtensionsSupported = result;
					m_processingContext.ReportObjectModel.FieldsImpl.ReaderFieldProperties = flag;
					return result;
				}
				catch (Exception e)
				{
					if (AsynchronousExceptionDetection.IsStoppingException(e))
					{
						throw;
					}
					if (dataReader != null)
					{
						dataReader.Dispose();
						dataReader = null;
					}
					if (dbCommand != null)
					{
						dbCommand.Dispose();
						dbCommand = null;
					}
					throw;
				}
				finally
				{
					if (m_processingContext.JobContext != null)
					{
						m_dataProcessingDurationMs += timer.ElapsedTimeMs();
					}
				}
			}
		}

		private class RuntimeReportDataSetNode : RuntimeDataSetNode, IScope, IHierarchyObj
		{
			private RuntimeDRCollection m_runtimeDataRegions;

			private DataAggregateObjList m_nonCustomAggregates;

			private DataAggregateObjList m_customAggregates;

			private const int m_chartDataRowCount = 1000;

			private ChunkManager.DataChunkWriter m_dataChunkWriter;

			private bool m_dataChunkSaved;

			private DataRowList m_dataRows;

			private RuntimeUserSortTargetInfo m_userSortTargetInfo;

			private int[] m_sortFilterExpressionScopeInfoIndices;

			internal bool HasSortFilterInfo => m_hasSortFilterInfo;

			bool IScope.TargetForNonDetailSort
			{
				get
				{
					if (m_userSortTargetInfo != null)
					{
						return m_userSortTargetInfo.TargetForNonDetailSort;
					}
					return false;
				}
			}

			int[] IScope.SortFilterExpressionScopeInfoIndices
			{
				get
				{
					if (m_sortFilterExpressionScopeInfoIndices == null)
					{
						m_sortFilterExpressionScopeInfoIndices = new int[m_processingContext.RuntimeSortFilterInfo.Count];
						for (int i = 0; i < m_processingContext.RuntimeSortFilterInfo.Count; i++)
						{
							m_sortFilterExpressionScopeInfoIndices[i] = -1;
						}
					}
					return m_sortFilterExpressionScopeInfoIndices;
				}
			}

			IHierarchyObj IHierarchyObj.HierarchyRoot => this;

			ProcessingContext IHierarchyObj.ProcessingContext => m_processingContext;

			BTreeNode IHierarchyObj.SortTree
			{
				get
				{
					if (m_userSortTargetInfo != null)
					{
						return m_userSortTargetInfo.SortTree;
					}
					return null;
				}
				set
				{
					if (m_userSortTargetInfo != null)
					{
						m_userSortTargetInfo.SortTree = value;
					}
					else
					{
						Global.Tracer.Assert(condition: false);
					}
				}
			}

			int IHierarchyObj.ExpressionIndex => 0;

			IntList IHierarchyObj.SortFilterInfoIndices
			{
				get
				{
					if (m_userSortTargetInfo != null)
					{
						return m_userSortTargetInfo.SortFilterInfoIndices;
					}
					return null;
				}
			}

			bool IHierarchyObj.IsDetail => false;

			internal RuntimeReportDataSetNode(Report report, DataSet dataSet, ProcessingContext processingContext)
				: base(report, dataSet, processingContext)
			{
				m_hasSortFilterInfo = m_processingContext.PopulateRuntimeSortFilterEventInfo(m_dataSet);
				UserSortFilterContext userSortFilterContext = m_processingContext.UserSortFilterContext;
				if (-1 == userSortFilterContext.DataSetID)
				{
					userSortFilterContext.DataSetID = m_dataSet.ID;
				}
				if (m_processingContext.IsSortFilterTarget(m_dataSet.IsSortFilterTarget, userSortFilterContext.CurrentContainingScope, this, ref m_userSortTargetInfo) && m_userSortTargetInfo.TargetForNonDetailSort)
				{
					m_dataRows = new DataRowList();
				}
			}

			bool IScope.IsTargetForSort(int index, bool detailSort)
			{
				if (m_userSortTargetInfo != null)
				{
					return m_userSortTargetInfo.IsTargetForSort(index, detailSort);
				}
				return false;
			}

			bool IScope.InScope(string scope)
			{
				if (CompareWithInvariantCulture(m_dataSet.Name, scope, ignoreCase: false) == 0)
				{
					return true;
				}
				return false;
			}

			void IScope.ReadRow(DataActions dataAction)
			{
				Global.Tracer.Assert(condition: false);
			}

			IScope IScope.GetOuterScope(bool includeSubReportContainingScope)
			{
				if (includeSubReportContainingScope)
				{
					return m_processingContext.UserSortFilterContext.CurrentContainingScope;
				}
				return null;
			}

			string IScope.GetScopeName()
			{
				return m_dataSet.Name;
			}

			int IScope.RecursiveLevel(string scope)
			{
				return 0;
			}

			bool IScope.TargetScopeMatched(int index, bool detailSort)
			{
				if (m_processingContext.UserSortFilterContext.CurrentContainingScope != null)
				{
					return m_processingContext.UserSortFilterContext.CurrentContainingScope.TargetScopeMatched(index, detailSort);
				}
				if (m_processingContext.RuntimeSortFilterInfo != null)
				{
					return true;
				}
				return false;
			}

			void IScope.GetScopeValues(IHierarchyObj targetScopeObj, VariantList[] scopeValues, ref int index)
			{
				IScope currentContainingScope = m_processingContext.UserSortFilterContext.CurrentContainingScope;
				if (this != targetScopeObj && currentContainingScope != null)
				{
					Global.Tracer.Assert(targetScopeObj == null, "(null == targetScopeObj)");
					currentContainingScope.GetScopeValues(null, scopeValues, ref index);
				}
			}

			void IScope.GetGroupNameValuePairs(Dictionary<string, object> pairs)
			{
			}

			IHierarchyObj IHierarchyObj.CreateHierarchyObj()
			{
				return new RuntimeSortHierarchyObj(this);
			}

			ProcessingMessageList IHierarchyObj.RegisterComparisonError(string propertyName)
			{
				return m_processingContext.RegisterComparisonErrorForSortFilterEvent(propertyName);
			}

			void IHierarchyObj.NextRow()
			{
				Global.Tracer.Assert(condition: false);
			}

			void IHierarchyObj.Traverse(ProcessingStages operation)
			{
				Global.Tracer.Assert(condition: false);
			}

			void IHierarchyObj.ReadRow()
			{
				SendToInner();
			}

			void IHierarchyObj.ProcessUserSort()
			{
				Global.Tracer.Assert(m_userSortTargetInfo != null, "(null != m_userSortTargetInfo)");
				m_processingContext.ProcessUserSortForTarget(this, ref m_dataRows, m_userSortTargetInfo.TargetForNonDetailSort);
				if (!m_userSortTargetInfo.TargetForNonDetailSort)
				{
					return;
				}
				m_userSortTargetInfo.ResetTargetForNonDetailSort();
				m_userSortTargetInfo.EnterProcessUserSortPhase(m_processingContext);
				m_runtimeDataRegions = new RuntimeDRCollection(this, m_dataSet.DataRegions, m_processingContext, m_report.MergeOnePass);
				m_userSortTargetInfo.SortTree.Traverse(ProcessingStages.UserSortFilter, ascending: true);
				m_userSortTargetInfo.SortTree = null;
				if (m_userSortTargetInfo.AggregateRows != null)
				{
					for (int i = 0; i < m_userSortTargetInfo.AggregateRows.Count; i++)
					{
						m_userSortTargetInfo.AggregateRows[i].SetFields(m_processingContext);
						SendToInner();
					}
					m_userSortTargetInfo.AggregateRows = null;
				}
				m_userSortTargetInfo.LeaveProcessUserSortPhase(m_processingContext);
			}

			void IHierarchyObj.MarkSortInfoProcessed(RuntimeSortFilterEventInfoList runtimeSortFilterInfo)
			{
				if (m_userSortTargetInfo != null)
				{
					m_userSortTargetInfo.MarkSortInfoProcessed(runtimeSortFilterInfo, this);
				}
			}

			void IHierarchyObj.AddSortInfoIndex(int sortInfoIndex, RuntimeSortFilterEventInfo sortInfo)
			{
				if (m_userSortTargetInfo != null)
				{
					m_userSortTargetInfo.AddSortInfoIndex(sortInfoIndex, sortInfo);
				}
			}

			internal override void RuntimeInitializeReportItemObjs()
			{
				for (int i = 0; i < m_dataSet.DataRegions.Count; i++)
				{
					m_processingContext.RuntimeInitializeReportItemObjs(m_dataSet.DataRegions[i], traverseDataRegions: true, setValue: false);
				}
			}

			internal override void EraseDataChunk()
			{
				if (m_dataChunkSaved)
				{
					m_dataChunkWriter = new ChunkManager.DataChunkWriter(m_dataSet, m_processingContext);
					m_dataChunkWriter.CloseAndEraseChunk();
				}
			}

			protected override void Process()
			{
				bool closeConnWhenFinish = false;
				try
				{
					FirstPassProcess(ref closeConnWhenFinish);
					m_processingContext.FirstPassPostProcess();
					if (!m_report.MergeOnePass)
					{
						if (m_processingContext.OWCChartName == null)
						{
							m_processingContext.CheckAndThrowIfAborted();
							SecondPass();
						}
						m_processingContext.CheckAndThrowIfAborted();
						ThirdPass();
					}
				}
				catch (Exception e)
				{
					if (AsynchronousExceptionDetection.IsStoppingException(e))
					{
						throw;
					}
					if (m_transInfo != null)
					{
						m_transInfo.RollbackRequired = true;
					}
					if (closeConnWhenFinish && m_dataSourceConnection != null)
					{
						m_processingContext.DataExtensionConnection.CloseConnectionWithoutPool(m_dataSourceConnection);
					}
					throw;
				}
				finally
				{
					if (m_dataReader != null)
					{
						((IDisposable)m_dataReader).Dispose();
						m_dataReader = null;
					}
					if (m_dataChunkWriter != null)
					{
						m_dataChunkWriter.Close();
						m_dataChunkWriter = null;
					}
				}
			}

			protected override void RegisterAggregates()
			{
				CreateAggregates(m_dataSet.Aggregates);
				CreateAggregates(m_dataSet.PostSortAggregates);
			}

			private void CreateAggregates(DataAggregateInfoList aggDefs)
			{
				if (aggDefs == null || 0 >= aggDefs.Count)
				{
					return;
				}
				for (int i = 0; i < aggDefs.Count; i++)
				{
					DataAggregateInfo dataAggregateInfo = aggDefs[i];
					DataAggregateObj dataAggregateObj = new DataAggregateObj(dataAggregateInfo, m_processingContext);
					m_processingContext.ReportObjectModel.AggregatesImpl.Add(dataAggregateObj);
					if (DataAggregateInfo.AggregateTypes.Previous != dataAggregateInfo.AggregateType)
					{
						if (DataAggregateInfo.AggregateTypes.Aggregate == dataAggregateInfo.AggregateType)
						{
							RuntimeDataRegionObj.AddAggregate(ref m_customAggregates, dataAggregateObj);
						}
						else
						{
							RuntimeDataRegionObj.AddAggregate(ref m_nonCustomAggregates, dataAggregateObj);
						}
					}
				}
			}

			protected override void FirstPassInit()
			{
				bool flag = false;
				InitRuntime(processReport: true);
				m_runtimeDataRegions = new RuntimeDRCollection(this, m_dataSet.DataRegions, m_processingContext, m_report.MergeOnePass);
				if (m_processingContext.SnapshotProcessing || m_processingContext.ProcessWithCachedData)
				{
					m_dataSet.CheckNonCalculatedFieldCount();
					m_dataReader = new ProcessingDataReader(m_dataSet, m_processingContext);
					flag = m_dataReader.ReaderExtensionsSupported;
					m_processingContext.ReportObjectModel.FieldsImpl.ReaderExtensionsSupported = flag;
					m_processingContext.ReportObjectModel.FieldsImpl.ReaderFieldProperties = m_dataReader.ReaderFieldProperties;
					if (m_dataSet.Query.RewrittenCommandText != null)
					{
						m_processingContext.DrillthroughInfo.AddRewrittenCommand(m_dataSet.ID, m_dataSet.Query.RewrittenCommandText);
					}
					m_dataReader.OverrideDataCacheCompareOptions(ref m_processingContext);
				}
				else
				{
					flag = RunDataSetQuery();
					if ((m_processingContext.HasUserProfileState & UserProfileState.InQuery) > UserProfileState.None && m_processingContext.SaveSnapshotData && !m_processingContext.HasUserSortFilter && (m_report.SubReports == null || 0 >= m_report.SubReports.Count))
					{
						m_processingContext.SaveSnapshotData = false;
					}
				}
				Global.Tracer.Assert(m_processingContext.ReportObjectModel.DataSetsImpl != null, "(null != m_processingContext.ReportObjectModel.DataSetsImpl)");
				m_processingContext.ReportObjectModel.DataSetsImpl.Add(m_dataSet);
				if (!m_processingContext.ResetForSubreportDataPrefetch && ((m_processingContext.SaveSnapshotData && m_processingContext.CreateReportChunkCallback != null) || (m_processingContext.DataCached && m_processingContext.CacheDataCallback != null)))
				{
					m_dataChunkWriter = new ChunkManager.DataChunkWriter(m_dataSet, m_processingContext, flag, m_processingContext.StopSaveSnapshotDataOnError);
					if (m_processingContext.SaveSnapshotData && m_processingContext.CreateReportChunkCallback != null)
					{
						m_dataChunkSaved = true;
					}
				}
			}

			protected override void FirstPassProcessDetailRow(Filters filters)
			{
				if (m_processingContext.ReportObjectModel.FieldsImpl.IsAggregateRow)
				{
					NextAggregateRow();
					return;
				}
				bool flag = true;
				if (filters != null)
				{
					flag = filters.PassFilters(m_processingContext.ReportObjectModel.FieldsImpl.GetFields());
				}
				if (flag)
				{
					((IFilterOwner)this).PostFilterNextRow();
				}
			}

			protected override void FirstPassCleanup(bool flushData)
			{
				if (m_dataChunkWriter != null)
				{
					bool flag = true;
					if (flushData)
					{
						flag = !m_dataChunkWriter.FinalFlush();
					}
					else
					{
						m_dataChunkWriter.Close();
					}
					if (flag)
					{
						m_processingContext.ErrorSavingSnapshotData = true;
						m_processingContext.DataCached = false;
						m_dataChunkSaved = !m_processingContext.StopSaveSnapshotDataOnError;
					}
					m_dataChunkWriter = null;
				}
			}

			private void SecondPass()
			{
				if (m_report.NeedPostGroupProcessing)
				{
					if (m_report.HasSpecialRecursiveAggregates)
					{
						m_processingContext.SecondPassOperation = ProcessingContext.SecondPassOperations.Filtering;
					}
					else
					{
						m_processingContext.SecondPassOperation = (ProcessingContext.SecondPassOperations.Sorting | ProcessingContext.SecondPassOperations.Filtering);
					}
					if (m_userSortTargetInfo != null)
					{
						m_userSortTargetInfo.EnterProcessUserSortPhase(m_processingContext);
					}
					m_runtimeDataRegions.SortAndFilter();
					if (m_report.HasSpecialRecursiveAggregates)
					{
						m_processingContext.SecondPassOperation = ProcessingContext.SecondPassOperations.Sorting;
						m_runtimeDataRegions.SortAndFilter();
					}
					if (m_userSortTargetInfo != null)
					{
						m_userSortTargetInfo.LeaveProcessUserSortPhase(m_processingContext);
					}
				}
			}

			private void ThirdPass()
			{
				if (m_report.HasPostSortAggregates)
				{
					AggregatesImpl aggregatesImpl = new AggregatesImpl(m_processingContext.ReportRuntime);
					RuntimeGroupRootObjList groupCollection = new RuntimeGroupRootObjList();
					m_processingContext.GlobalRVCollection = aggregatesImpl;
					m_runtimeDataRegions.CalculateRunningValues(aggregatesImpl, groupCollection);
				}
			}

			protected override bool FirstPassGetNextDetailRow()
			{
				m_processingContext.CheckAndThrowIfAborted();
				bool result = false;
				if (m_processingContext.OWCChartName != null && 1000 < m_dataRowsRead)
				{
					return result;
				}
				result = GetNextDetailRow();
				if (m_dataChunkWriter != null)
				{
					if (1 == m_dataRowsRead && m_foundExtendedProperties)
					{
						m_dataChunkWriter.FieldAliasPropertyNames = m_referencedAliasPropertyNames;
					}
					if (result && m_dataChunkWriter != null && !m_dataChunkWriter.AddRecordRow(m_processingContext.ReportObjectModel.FieldsImpl, m_dataSet.NonCalculatedFieldCount))
					{
						m_processingContext.ErrorSavingSnapshotData = true;
						m_processingContext.DataCached = false;
						if (m_processingContext.StopSaveSnapshotDataOnError)
						{
							m_dataChunkSaved = false;
							m_dataChunkWriter = null;
						}
					}
				}
				return result;
			}

			private void NextAggregateRow()
			{
				if (m_processingContext.ReportObjectModel.FieldsImpl.AggregationFieldCount == 0 && m_customAggregates != null)
				{
					for (int i = 0; i < m_customAggregates.Count; i++)
					{
						m_customAggregates[i].Update();
					}
				}
				if (m_userSortTargetInfo != null && m_userSortTargetInfo.SortTree != null)
				{
					if (m_userSortTargetInfo.AggregateRows == null)
					{
						m_userSortTargetInfo.AggregateRows = new AggregateRowList();
					}
					AggregateRow value = new AggregateRow(m_processingContext);
					m_userSortTargetInfo.AggregateRows.Add(value);
					if (!m_userSortTargetInfo.TargetForNonDetailSort)
					{
						return;
					}
				}
				SendToInner();
			}

			protected override void NextNonAggregateRow()
			{
				if (m_nonCustomAggregates != null)
				{
					for (int i = 0; i < m_nonCustomAggregates.Count; i++)
					{
						m_nonCustomAggregates[i].Update();
					}
				}
				if (m_dataRows != null)
				{
					RuntimeDetailObj.SaveData(m_dataRows, m_processingContext);
				}
				SendToInner();
			}

			private void SendToInner()
			{
				m_runtimeDataRegions.FirstPassNextDataRow();
			}
		}

		private sealed class RuntimeSortDataHolder : ISortDataHolder
		{
			private IHierarchyObj m_owner;

			private DataRowList m_dataRows;

			internal RuntimeSortDataHolder(IHierarchyObj owner)
			{
				m_owner = owner;
				m_dataRows = new DataRowList();
			}

			void ISortDataHolder.NextRow()
			{
				FieldImpl[] andSaveFields = m_owner.ProcessingContext.ReportObjectModel.FieldsImpl.GetAndSaveFields();
				m_dataRows.Add(andSaveFields);
			}

			void ISortDataHolder.Traverse(ProcessingStages operation)
			{
				Global.Tracer.Assert(ProcessingStages.UserSortFilter == operation, "(ProcessingStages.UserSortFilter == operation)");
				if (m_dataRows != null)
				{
					for (int i = 0; i < m_dataRows.Count; i++)
					{
						FieldImpl[] fields = m_dataRows[i];
						m_owner.ProcessingContext.ReportObjectModel.FieldsImpl.SetFields(fields);
						m_owner.ReadRow();
					}
				}
			}
		}

		private sealed class RuntimePrefetchDataSetNode : RuntimeDataSetNode
		{
			private ChunkManager.DataChunkWriter m_dataChunkWriter;

			internal RuntimePrefetchDataSetNode(Report report, DataSet dataSet, ProcessingContext processingContext)
				: base(report, dataSet, processingContext)
			{
			}

			protected override void Process()
			{
				bool closeConnWhenFinish = false;
				if (m_dataSet.IsShareable() && m_processingContext.CachedDataChunkMapping.ContainsKey(m_dataSet.ID))
				{
					return;
				}
				try
				{
					FirstPassProcess(ref closeConnWhenFinish);
				}
				catch (Exception e)
				{
					if (AsynchronousExceptionDetection.IsStoppingException(e))
					{
						throw;
					}
					if (m_transInfo != null)
					{
						m_transInfo.RollbackRequired = true;
					}
					if (closeConnWhenFinish && m_dataSourceConnection != null)
					{
						m_processingContext.DataExtensionConnection.CloseConnectionWithoutPool(m_dataSourceConnection);
					}
					throw;
				}
				finally
				{
					if (m_dataReader != null)
					{
						((IDisposable)m_dataReader).Dispose();
						m_dataReader = null;
					}
				}
			}

			protected override void FirstPassInit()
			{
				InitRuntime(processReport: false);
				bool readerExtensionsSupported = RunDataSetQuery();
				if (m_processingContext.CreateReportChunkCallback != null || m_processingContext.CacheDataCallback != null)
				{
					m_dataChunkWriter = new ChunkManager.DataChunkWriter(m_dataSet, m_processingContext, readerExtensionsSupported, stopSaveOnError: false);
				}
			}

			protected override bool FirstPassGetNextDetailRow()
			{
				m_processingContext.CheckAndThrowIfAborted();
				bool nextDetailRow = GetNextDetailRow();
				if (m_dataChunkWriter != null)
				{
					if (1 == m_dataRowsRead && m_foundExtendedProperties)
					{
						m_dataChunkWriter.FieldAliasPropertyNames = m_referencedAliasPropertyNames;
					}
					if (nextDetailRow)
					{
						m_dataChunkWriter.AddRecordRow(m_processingContext.ReportObjectModel.FieldsImpl, m_dataSet.NonCalculatedFieldCount);
					}
				}
				return nextDetailRow;
			}

			protected override void FirstPassProcessDetailRow(Filters filters)
			{
			}

			protected override void NextNonAggregateRow()
			{
			}

			protected override void FirstPassCleanup(bool flushData)
			{
				if (m_dataChunkWriter != null)
				{
					if (flushData)
					{
						m_dataChunkWriter.FinalFlush();
					}
					else
					{
						m_dataChunkWriter.Close();
					}
					m_dataChunkWriter = null;
				}
			}
		}

		private sealed class RuntimeReportParametersDataSetNode : RuntimeDataSetNode
		{
			private LegacyReportParameterDataSetCache m_reportParameterDataSetObj;

			internal RuntimeReportParametersDataSetNode(Report report, DataSet dataSet, ProcessingContext processingContext, LegacyReportParameterDataSetCache aCache)
				: base(report, dataSet, processingContext)
			{
				m_reportParameterDataSetObj = aCache;
			}

			protected override void FirstPassInit()
			{
				InitRuntime(processReport: false);
				RunDataSetQuery();
			}

			protected override void FirstPassProcessDetailRow(Filters filters)
			{
				bool flag = true;
				if (filters != null)
				{
					flag = filters.PassFilters(m_processingContext.ReportObjectModel.FieldsImpl.GetFields());
				}
				if (flag)
				{
					((IFilterOwner)this).PostFilterNextRow();
				}
			}

			protected override void Process()
			{
				bool closeConnWhenFinish = false;
				try
				{
					FirstPassProcess(ref closeConnWhenFinish);
				}
				catch (Exception e)
				{
					if (AsynchronousExceptionDetection.IsStoppingException(e))
					{
						throw;
					}
					if (m_transInfo != null)
					{
						m_transInfo.RollbackRequired = true;
					}
					if (closeConnWhenFinish && m_dataSourceConnection != null)
					{
						m_processingContext.DataExtensionConnection.CloseConnectionWithoutPool(m_dataSourceConnection);
					}
					throw;
				}
				finally
				{
					if (m_dataReader != null)
					{
						((IDisposable)m_dataReader).Dispose();
						m_dataReader = null;
					}
				}
			}

			protected override bool FirstPassGetNextDetailRow()
			{
				m_processingContext.CheckAndThrowIfAborted();
				return GetNextDetailRow();
			}

			protected override void NextNonAggregateRow()
			{
				m_reportParameterDataSetObj.NextRow(m_processingContext.ReportObjectModel.FieldsImpl.GetAndSaveFields());
			}
		}

		internal sealed class RuntimeReportParameterDataSetObj
		{
			private ProcessingContext m_processingContext;

			private DataRowList m_dataRows;

			internal int Count
			{
				get
				{
					if (m_dataRows == null)
					{
						return 0;
					}
					return m_dataRows.Count;
				}
			}

			internal RuntimeReportParameterDataSetObj(ProcessingContext processingContext)
			{
				m_processingContext = processingContext;
			}

			internal void NextRow()
			{
				if (m_dataRows == null)
				{
					m_dataRows = new DataRowList();
				}
				FieldImpl[] andSaveFields = m_processingContext.ReportObjectModel.FieldsImpl.GetAndSaveFields();
				m_dataRows.Add(andSaveFields);
			}

			internal object GetFieldValue(int row, int col)
			{
				if (Count == 0)
				{
					return null;
				}
				Global.Tracer.Assert(m_dataRows[row][col] != null, "(null != m_dataRows[row][col])");
				if (m_dataRows[row][col].IsMissing)
				{
					return null;
				}
				m_processingContext.ReportObjectModel.FieldsImpl.SetFields(m_dataRows[row]);
				return m_dataRows[row][col].Value;
			}
		}

		internal sealed class ProcessingDataReader : IDisposable
		{
			private MappingDataReader m_dataSourceDataReader;

			private ChunkManager.DataChunkReader m_snapshotDataReader;

			internal bool ReaderExtensionsSupported
			{
				get
				{
					if (m_dataSourceDataReader != null)
					{
						return m_dataSourceDataReader.ReaderExtensionsSupported;
					}
					return m_snapshotDataReader.ReaderExtensionsSupported;
				}
			}

			internal bool ReaderFieldProperties
			{
				get
				{
					if (m_dataSourceDataReader != null)
					{
						return m_dataSourceDataReader.ReaderFieldProperties;
					}
					return m_snapshotDataReader.ReaderFieldProperties;
				}
			}

			public bool IsAggregateRow
			{
				get
				{
					if (m_dataSourceDataReader != null)
					{
						return m_dataSourceDataReader.IsAggregateRow;
					}
					return m_snapshotDataReader.IsAggregateRow;
				}
			}

			public int AggregationFieldCount
			{
				get
				{
					if (m_dataSourceDataReader != null)
					{
						return m_dataSourceDataReader.AggregationFieldCount;
					}
					return m_snapshotDataReader.AggregationFieldCount;
				}
			}

			internal ProcessingDataReader(string dataSetName, IDataReader sourceReader, string[] aliases, string[] names)
			{
				m_dataSourceDataReader = new MappingDataReader(dataSetName, sourceReader, aliases, names, null);
			}

			internal ProcessingDataReader(DataSet dataSet, ProcessingContext context)
			{
				m_snapshotDataReader = new ChunkManager.DataChunkReader(dataSet, context);
			}

			void IDisposable.Dispose()
			{
				if (m_dataSourceDataReader != null)
				{
					((IDisposable)m_dataSourceDataReader).Dispose();
				}
				else
				{
					((IDisposable)m_snapshotDataReader).Dispose();
				}
			}

			internal void OverrideDataCacheCompareOptions(ref ProcessingContext context)
			{
				if (m_snapshotDataReader != null && context.ProcessWithCachedData && m_snapshotDataReader.ValidCompareOptions)
				{
					context.ClrCompareOptions = m_snapshotDataReader.CompareOptions;
				}
			}

			public bool GetNextRow()
			{
				if (m_dataSourceDataReader != null)
				{
					return m_dataSourceDataReader.GetNextRow();
				}
				return m_snapshotDataReader.GetNextRow();
			}

			internal object GetColumn(int aliasIndex)
			{
				object obj = null;
				obj = ((m_dataSourceDataReader == null) ? m_snapshotDataReader.GetFieldValue(aliasIndex) : m_dataSourceDataReader.GetFieldValue(aliasIndex));
				if (obj is DBNull)
				{
					return null;
				}
				return obj;
			}

			internal bool IsAggregationField(int aliasIndex)
			{
				if (m_dataSourceDataReader != null)
				{
					return m_dataSourceDataReader.IsAggregationField(aliasIndex);
				}
				return m_snapshotDataReader.IsAggregationField(aliasIndex);
			}

			internal int GetPropertyCount(int aliasIndex)
			{
				if (m_dataSourceDataReader != null)
				{
					return m_dataSourceDataReader.GetPropertyCount(aliasIndex);
				}
				if (m_snapshotDataReader != null && m_snapshotDataReader.FieldPropertyNames != null && m_snapshotDataReader.FieldPropertyNames[aliasIndex] != null)
				{
					StringList propertyNames = m_snapshotDataReader.FieldPropertyNames.GetPropertyNames(aliasIndex);
					if (propertyNames != null)
					{
						return propertyNames.Count;
					}
				}
				return 0;
			}

			internal string GetPropertyName(int aliasIndex, int propertyIndex)
			{
				if (m_dataSourceDataReader != null)
				{
					return m_dataSourceDataReader.GetPropertyName(aliasIndex, propertyIndex);
				}
				if (m_snapshotDataReader != null && m_snapshotDataReader.FieldPropertyNames != null)
				{
					return m_snapshotDataReader.FieldPropertyNames.GetPropertyName(aliasIndex, propertyIndex);
				}
				return null;
			}

			internal object GetPropertyValue(int aliasIndex, int propertyIndex)
			{
				object obj = null;
				if (m_dataSourceDataReader != null)
				{
					obj = m_dataSourceDataReader.GetPropertyValue(aliasIndex, propertyIndex);
				}
				else if (m_snapshotDataReader != null)
				{
					obj = m_snapshotDataReader.GetPropertyValue(aliasIndex, propertyIndex);
				}
				if (obj is DBNull)
				{
					return null;
				}
				return obj;
			}
		}

		internal sealed class PageMergeInteractive
		{
			private PageTextboxes m_pageTextboxes;

			private ReportSnapshot m_reportSnapshot;

			private Report m_report;

			private ReportInstance m_reportInstance;

			private ParameterInfoCollection m_parameters;

			private ProcessingContext m_processingContext;

			private AggregatesImpl m_aggregates;

			private Hashtable m_aggregatesOverReportItems;

			internal UserProfileState Process(PageTextboxes pageTextboxes, ReportSnapshot reportSnapshot, ICatalogItemContext reportContext, string reportName, ParameterInfoCollection parameters, ChunkManager.ProcessingChunkManager pageSectionManager, CreateReportChunk createChunkCallback, IGetResource getResourceCallback, ErrorContext errorContext, UserProfileState allowUserProfileState, ReportRuntimeSetup reportRuntimeSetup, int uniqueNameCounter, IDataProtection dataProtection, ref ReportDrillthroughInfo drillthroughInfo)
			{
				UserProfileState result = UserProfileState.None;
				try
				{
					m_pageTextboxes = pageTextboxes;
					m_reportSnapshot = reportSnapshot;
					m_report = reportSnapshot.Report;
					m_reportInstance = reportSnapshot.ReportInstance;
					m_parameters = parameters;
					m_processingContext = new ProcessingContext(reportContext, m_report.ShowHideType, getResourceCallback, m_report.EmbeddedImages, m_report.ImageStreamNames, errorContext, !m_report.PageMergeOnePass, allowUserProfileState, reportRuntimeSetup, createChunkCallback, pageSectionManager, uniqueNameCounter, dataProtection, ref drillthroughInfo);
					if (m_report.Language != null)
					{
						string text = null;
						text = ((m_report.Language.Type != ExpressionInfo.Types.Constant) ? m_reportInstance.Language : m_report.Language.Value);
						if (text != null)
						{
							try
							{
								CultureInfo cultureInfo = new CultureInfo(text, useUserOverride: false);
								if (cultureInfo.IsNeutralCulture)
								{
									cultureInfo = CultureInfo.CreateSpecificCulture(text);
									cultureInfo = new CultureInfo(cultureInfo.Name, useUserOverride: false);
								}
								Thread.CurrentThread.CurrentCulture = cultureInfo;
							}
							catch (Exception)
							{
								Thread.CurrentThread.CurrentCulture = Localization.DefaultReportServerSpecificCulture;
							}
						}
						else
						{
							Thread.CurrentThread.CurrentCulture = Localization.DefaultReportServerSpecificCulture;
						}
					}
					else
					{
						Thread.CurrentThread.CurrentCulture = Localization.DefaultReportServerSpecificCulture;
					}
					GlobalInit(reportName, m_reportInstance.NumberOfPages);
					m_reportSnapshot.PageSections = new List<PageSectionInstance>(2 * m_reportInstance.NumberOfPages);
					for (int i = 0; i < m_reportInstance.NumberOfPages; i++)
					{
						PageInit(i);
						if (!m_report.PageMergeOnePass)
						{
							FirstPass(i);
						}
						SecondPass(i);
					}
				}
				finally
				{
					if (m_processingContext != null)
					{
						if (m_processingContext.ReportRuntime != null)
						{
							m_processingContext.ReportRuntime.Close();
						}
						result = m_processingContext.HasUserProfileState;
					}
					m_report = null;
					m_reportInstance = null;
					m_processingContext = null;
				}
				return result;
			}

			private void GlobalInit(string reportName, int totalPages)
			{
				m_processingContext.ReportObjectModel = new ObjectModelImpl(m_processingContext);
				Global.Tracer.Assert(m_processingContext.ReportRuntime == null, "(m_processingContext.ReportRuntime == null)");
				m_processingContext.ReportRuntime = new ReportRuntime(m_processingContext.ReportObjectModel, m_processingContext.ErrorContext);
				m_processingContext.ReportObjectModel.FieldsImpl = new FieldsImpl();
				m_processingContext.ReportObjectModel.ParametersImpl = new ParametersImpl(m_parameters.Count);
				m_processingContext.ReportObjectModel.GlobalsImpl = new GlobalsImpl(reportName, 0, totalPages, m_reportSnapshot.ExecutionTime, m_reportSnapshot.ReportServerUrl, m_reportSnapshot.ReportFolder);
				m_processingContext.ReportObjectModel.UserImpl = new UserImpl(m_reportSnapshot.RequestUserName, m_reportSnapshot.Language, m_processingContext.AllowUserProfileState);
				m_processingContext.ReportObjectModel.ReportItemsImpl = new ReportItemsImpl();
				m_processingContext.ReportObjectModel.AggregatesImpl = new AggregatesImpl(m_processingContext.ReportRuntime);
				m_processingContext.ReportObjectModel.DataSetsImpl = new DataSetsImpl();
				m_processingContext.ReportObjectModel.DataSourcesImpl = new DataSourcesImpl(m_report.DataSourceCount);
				for (int i = 0; i < m_parameters.Count; i++)
				{
					m_processingContext.ReportObjectModel.ParametersImpl.Add(m_parameters[i].Name, new ParameterImpl(m_parameters[i].Values, m_parameters[i].Labels, m_parameters[i].MultiValue));
				}
				m_processingContext.ReportRuntime.LoadCompiledCode(m_report, parametersOnly: false, m_processingContext.ReportObjectModel, m_processingContext.ReportRuntimeSetup);
			}

			private void PageInit(int currentPageNumber)
			{
				m_processingContext.ReportObjectModel.GlobalsImpl.SetPageNumber(currentPageNumber + 1);
				m_processingContext.ReportObjectModel.ReportItemsImpl = new ReportItemsImpl();
				m_processingContext.ReportObjectModel.AggregatesImpl = new AggregatesImpl(m_processingContext.ReportRuntime);
				if (m_processingContext.ReportRuntime.ReportExprHost != null)
				{
					m_processingContext.RuntimeInitializeReportItemObjs(m_report.ReportItems, traverseDataRegions: true, setValue: true);
					if (m_report.PageHeader != null)
					{
						if (m_processingContext.ReportRuntime.ReportExprHost != null)
						{
							m_report.PageHeader.SetExprHost(m_processingContext.ReportRuntime.ReportExprHost, m_processingContext.ReportObjectModel);
						}
						m_processingContext.RuntimeInitializeReportItemObjs(m_report.PageHeader.ReportItems, traverseDataRegions: false, setValue: false);
					}
					if (m_report.PageFooter != null)
					{
						if (m_processingContext.ReportRuntime.ReportExprHost != null)
						{
							m_report.PageFooter.SetExprHost(m_processingContext.ReportRuntime.ReportExprHost, m_processingContext.ReportObjectModel);
						}
						m_processingContext.RuntimeInitializeReportItemObjs(m_report.PageFooter.ReportItems, traverseDataRegions: false, setValue: false);
					}
				}
				m_aggregates = new AggregatesImpl(m_processingContext.ReportRuntime);
				m_aggregatesOverReportItems = new Hashtable();
				m_processingContext.ReportObjectModel.ReportItemsImpl.SpecialMode = true;
				if (m_report.PageAggregates != null)
				{
					for (int i = 0; i < m_report.PageAggregates.Count; i++)
					{
						DataAggregateInfo dataAggregateInfo = m_report.PageAggregates[i];
						dataAggregateInfo.ExprHostInitialized = false;
						DataAggregateObj dataAggregateObj = new DataAggregateObj(dataAggregateInfo, m_processingContext);
						dataAggregateObj.EvaluateParameters(out object[] _, out DataFieldStatus _);
						string specialModeIndex = m_processingContext.ReportObjectModel.ReportItemsImpl.GetSpecialModeIndex();
						if (specialModeIndex == null)
						{
							m_aggregates.Add(dataAggregateObj);
						}
						else
						{
							AggregatesImpl aggregatesImpl = (AggregatesImpl)m_aggregatesOverReportItems[specialModeIndex];
							if (aggregatesImpl == null)
							{
								aggregatesImpl = new AggregatesImpl(m_processingContext.ReportRuntime);
								m_aggregatesOverReportItems.Add(specialModeIndex, aggregatesImpl);
							}
							aggregatesImpl.Add(dataAggregateObj);
						}
						dataAggregateObj.Init();
					}
				}
				m_processingContext.ReportObjectModel.ReportItemsImpl.SpecialMode = false;
			}

			private void FirstPass(int currentPageNumber)
			{
				Hashtable hashtable = null;
				if (m_pageTextboxes != null)
				{
					hashtable = m_pageTextboxes.GetTextboxesOnPage(currentPageNumber);
				}
				try
				{
					foreach (DataAggregateObj @object in m_aggregates.Objects)
					{
						m_processingContext.ReportObjectModel.AggregatesImpl.Add(@object);
					}
					if (hashtable != null)
					{
						IEnumerator enumerator2 = hashtable.Keys.GetEnumerator();
						while (enumerator2.MoveNext())
						{
							string text = enumerator2.Current as string;
							Global.Tracer.Assert(text != null, "(null != name)");
							ArrayList arrayList = hashtable[text] as ArrayList;
							Global.Tracer.Assert(arrayList != null && 0 < arrayList.Count, "(null != values && 0 < values.Count)");
							AggregatesImpl aggregatesImpl = (AggregatesImpl)m_aggregatesOverReportItems[text];
							TextBoxImpl textBoxImpl = (TextBoxImpl)m_processingContext.ReportObjectModel.ReportItemsImpl[text];
							if (aggregatesImpl != null)
							{
								Global.Tracer.Assert(textBoxImpl != null, "(null != textBoxObj)");
								for (int i = 0; i < arrayList.Count; i++)
								{
									textBoxImpl.SetResult(new VariantResult(errorOccurred: false, arrayList[i]));
									foreach (DataAggregateObj object2 in aggregatesImpl.Objects)
									{
										object2.Update();
									}
								}
							}
							else
							{
								textBoxImpl.SetResult(new VariantResult(errorOccurred: false, arrayList[arrayList.Count - 1]));
							}
						}
					}
					foreach (AggregatesImpl value in m_aggregatesOverReportItems.Values)
					{
						foreach (DataAggregateObj object3 in value.Objects)
						{
							m_processingContext.ReportObjectModel.AggregatesImpl.Add(object3);
						}
					}
				}
				finally
				{
					m_aggregates = null;
					m_aggregatesOverReportItems = null;
				}
			}

			private void SecondPass(int currentPageNumber)
			{
				PageSectionInstance pageSectionInstance = null;
				PageSectionInstance pageSectionInstance2 = null;
				if (m_report.PageHeaderEvaluation)
				{
					pageSectionInstance = new PageSectionInstance(m_processingContext, currentPageNumber, m_report.PageHeader);
					CreateInstances(pageSectionInstance.ReportItemColInstance, m_report.PageHeader.ReportItems);
				}
				if (m_report.PageFooterEvaluation)
				{
					pageSectionInstance2 = new PageSectionInstance(m_processingContext, currentPageNumber, m_report.PageFooter);
					CreateInstances(pageSectionInstance2.ReportItemColInstance, m_report.PageFooter.ReportItems);
				}
				m_reportSnapshot.PageSections.Add(pageSectionInstance);
				m_reportSnapshot.PageSections.Add(pageSectionInstance2);
			}

			private void CreateInstances(ReportItemColInstance collectionInstance, ReportItemCollection reportItemsDef)
			{
				reportItemsDef.ProcessDrillthroughAction(m_processingContext, collectionInstance.ChildrenNonComputedUniqueNames);
				if (reportItemsDef.ComputedReportItems == null)
				{
					return;
				}
				ReportItemInstance reportItemInstance = null;
				for (int i = 0; i < reportItemsDef.ComputedReportItems.Count; i++)
				{
					ReportItem reportItem = reportItemsDef.ComputedReportItems[i];
					if (reportItem is TextBox)
					{
						reportItemInstance = RuntimeRICollection.CreateTextBoxInstance((TextBox)reportItem, m_processingContext, i, null);
					}
					else if (reportItem is Line)
					{
						reportItemInstance = RuntimeRICollection.CreateLineInstance((Line)reportItem, m_processingContext, i);
					}
					else if (reportItem is Image)
					{
						reportItemInstance = RuntimeRICollection.CreateImageInstance((Image)reportItem, m_processingContext, i);
					}
					else if (reportItem is Rectangle)
					{
						Rectangle rectangle = (Rectangle)reportItem;
						RectangleInstance rectangleInstance = new RectangleInstance(m_processingContext, rectangle, i);
						CreateInstances(rectangleInstance.ReportItemColInstance, rectangle.ReportItems);
						reportItemInstance = rectangleInstance;
					}
					if (reportItemInstance != null)
					{
						collectionInstance.Add(reportItemInstance);
					}
				}
			}
		}

		internal sealed class PageMerge
		{
			private int m_pageNumber;

			private Microsoft.ReportingServices.ReportRendering.Report m_renderingReport;

			private ReportSnapshot m_reportSnapshot;

			private Report m_report;

			private ReportInstance m_reportInstance;

			private PageReportItems m_pageReportItems;

			private ProcessingContext m_processingContext;

			private Microsoft.ReportingServices.ReportRendering.PageSection m_pageHeader;

			private Microsoft.ReportingServices.ReportRendering.PageSection m_pageFooter;

			private AggregatesImpl m_aggregates;

			private Hashtable m_aggregatesOverReportItems;

			internal void Process(int pageNumber, int totalPages, Microsoft.ReportingServices.ReportRendering.Report report, PageReportItems pageReportItems, ErrorContext errorContext, out Microsoft.ReportingServices.ReportRendering.PageSection pageHeader, out Microsoft.ReportingServices.ReportRendering.PageSection pageFooter)
			{
				if (!report.NeedsHeaderFooterEvaluation)
				{
					pageHeader = null;
					pageFooter = null;
					return;
				}
				try
				{
					m_pageNumber = pageNumber;
					m_renderingReport = report;
					m_reportSnapshot = report.RenderingContext.ReportSnapshot;
					m_report = report.ReportDef;
					m_reportInstance = report.ReportInstance;
					m_pageReportItems = pageReportItems;
					m_processingContext = new ProcessingContext(report.RenderingContext.TopLevelReportContext, m_report.ShowHideType, report.RenderingContext.GetResourceCallback, m_report.EmbeddedImages, m_report.ImageStreamNames, errorContext, !m_report.PageMergeOnePass, report.RenderingContext.AllowUserProfileState, report.RenderingContext.ReportRuntimeSetup, report.RenderingContext.DataProtection);
					if (m_report.Language != null)
					{
						string text = null;
						text = ((m_report.Language.Type != ExpressionInfo.Types.Constant) ? m_reportInstance.Language : m_report.Language.Value);
						if (text != null)
						{
							try
							{
								CultureInfo cultureInfo = new CultureInfo(text, useUserOverride: false);
								if (cultureInfo.IsNeutralCulture)
								{
									cultureInfo = CultureInfo.CreateSpecificCulture(text);
									cultureInfo = new CultureInfo(cultureInfo.Name, useUserOverride: false);
								}
								Thread.CurrentThread.CurrentCulture = cultureInfo;
							}
							catch (Exception)
							{
								Thread.CurrentThread.CurrentCulture = Localization.DefaultReportServerSpecificCulture;
							}
						}
						else
						{
							Thread.CurrentThread.CurrentCulture = Localization.DefaultReportServerSpecificCulture;
						}
					}
					else
					{
						Thread.CurrentThread.CurrentCulture = Localization.DefaultReportServerSpecificCulture;
					}
					FirstPassInit(totalPages);
					FirstPass();
					SecondPass();
					pageHeader = m_pageHeader;
					pageFooter = m_pageFooter;
				}
				finally
				{
					if (m_processingContext != null)
					{
						if (m_processingContext.ReportRuntime != null)
						{
							m_processingContext.ReportRuntime.Close();
						}
						report.RenderingContext.UsedUserProfileState = m_processingContext.HasUserProfileState;
					}
					m_renderingReport = null;
					m_report = null;
					m_reportInstance = null;
					m_pageReportItems = null;
					m_processingContext = null;
					m_pageHeader = null;
					m_pageFooter = null;
				}
			}

			private void FirstPassInit(int totalPages)
			{
				ReportInstanceInfo reportInstanceInfo = (ReportInstanceInfo)m_reportInstance.GetInstanceInfo(m_renderingReport.RenderingContext.ChunkManager);
				m_processingContext.ReportObjectModel = new ObjectModelImpl(m_processingContext);
				Global.Tracer.Assert(m_processingContext.ReportRuntime == null, "(m_processingContext.ReportRuntime == null)");
				m_processingContext.ReportRuntime = new ReportRuntime(m_processingContext.ReportObjectModel, m_processingContext.ErrorContext);
				m_processingContext.ReportObjectModel.FieldsImpl = new FieldsImpl();
				m_processingContext.ReportObjectModel.ParametersImpl = new ParametersImpl(reportInstanceInfo.Parameters.Count);
				m_processingContext.ReportObjectModel.GlobalsImpl = new GlobalsImpl(reportInstanceInfo.ReportName, m_pageNumber, totalPages, m_reportSnapshot.ExecutionTime, m_reportSnapshot.ReportServerUrl, m_reportSnapshot.ReportFolder);
				m_processingContext.ReportObjectModel.UserImpl = new UserImpl(m_reportSnapshot.RequestUserName, m_reportSnapshot.Language, m_processingContext.AllowUserProfileState);
				m_processingContext.ReportObjectModel.ReportItemsImpl = new ReportItemsImpl();
				m_processingContext.ReportObjectModel.AggregatesImpl = new AggregatesImpl(m_processingContext.ReportRuntime);
				m_processingContext.ReportObjectModel.DataSetsImpl = new DataSetsImpl();
				m_processingContext.ReportObjectModel.DataSourcesImpl = new DataSourcesImpl(m_report.DataSourceCount);
				for (int i = 0; i < reportInstanceInfo.Parameters.Count; i++)
				{
					m_processingContext.ReportObjectModel.ParametersImpl.Add(reportInstanceInfo.Parameters[i].Name, new ParameterImpl(reportInstanceInfo.Parameters[i].Values, reportInstanceInfo.Parameters[i].Labels, reportInstanceInfo.Parameters[i].MultiValue));
				}
				m_processingContext.ReportRuntime.LoadCompiledCode(m_report, parametersOnly: false, m_processingContext.ReportObjectModel, m_processingContext.ReportRuntimeSetup);
				if (m_processingContext.ReportRuntime.ReportExprHost != null)
				{
					m_processingContext.RuntimeInitializeReportItemObjs(m_report.ReportItems, traverseDataRegions: true, setValue: true);
					if (m_report.PageHeader != null)
					{
						if (m_processingContext.ReportRuntime.ReportExprHost != null)
						{
							m_report.PageHeader.SetExprHost(m_processingContext.ReportRuntime.ReportExprHost, m_processingContext.ReportObjectModel);
						}
						m_processingContext.RuntimeInitializeReportItemObjs(m_report.PageHeader.ReportItems, traverseDataRegions: false, setValue: false);
					}
					if (m_report.PageFooter != null)
					{
						if (m_processingContext.ReportRuntime.ReportExprHost != null)
						{
							m_report.PageFooter.SetExprHost(m_processingContext.ReportRuntime.ReportExprHost, m_processingContext.ReportObjectModel);
						}
						m_processingContext.RuntimeInitializeReportItemObjs(m_report.PageFooter.ReportItems, traverseDataRegions: false, setValue: false);
					}
				}
				m_aggregates = new AggregatesImpl(m_processingContext.ReportRuntime);
				m_aggregatesOverReportItems = new Hashtable();
				m_processingContext.ReportObjectModel.ReportItemsImpl.SpecialMode = true;
				if (m_report.PageAggregates != null)
				{
					for (int j = 0; j < m_report.PageAggregates.Count; j++)
					{
						DataAggregateInfo dataAggregateInfo = m_report.PageAggregates[j];
						dataAggregateInfo.ExprHostInitialized = false;
						DataAggregateObj dataAggregateObj = new DataAggregateObj(dataAggregateInfo, m_processingContext);
						dataAggregateObj.EvaluateParameters(out object[] _, out DataFieldStatus _);
						string specialModeIndex = m_processingContext.ReportObjectModel.ReportItemsImpl.GetSpecialModeIndex();
						if (specialModeIndex == null)
						{
							m_aggregates.Add(dataAggregateObj);
						}
						else
						{
							AggregatesImpl aggregatesImpl = (AggregatesImpl)m_aggregatesOverReportItems[specialModeIndex];
							if (aggregatesImpl == null)
							{
								aggregatesImpl = new AggregatesImpl(m_processingContext.ReportRuntime);
								m_aggregatesOverReportItems.Add(specialModeIndex, aggregatesImpl);
							}
							aggregatesImpl.Add(dataAggregateObj);
						}
						dataAggregateObj.Init();
					}
				}
				m_processingContext.ReportObjectModel.ReportItemsImpl.SpecialMode = false;
			}

			private void FirstPass()
			{
				try
				{
					if (m_report.PageMergeOnePass)
					{
						return;
					}
					foreach (DataAggregateObj @object in m_aggregates.Objects)
					{
						m_processingContext.ReportObjectModel.AggregatesImpl.Add(@object);
					}
					for (int i = 0; i < m_pageReportItems.Count; i++)
					{
						FirstPassReportItem(m_pageReportItems[i]);
					}
					foreach (AggregatesImpl value in m_aggregatesOverReportItems.Values)
					{
						foreach (DataAggregateObj object2 in value.Objects)
						{
							m_processingContext.ReportObjectModel.AggregatesImpl.Add(object2);
						}
					}
				}
				finally
				{
					m_aggregates = null;
					m_aggregatesOverReportItems = null;
				}
			}

			private void FirstPassReportItems(Microsoft.ReportingServices.ReportRendering.ReportItemCollection reportItems)
			{
				if (reportItems != null)
				{
					for (int i = 0; i < reportItems.Count; i++)
					{
						FirstPassReportItem(reportItems[i]);
					}
				}
			}

			private void FirstPassReportItem(Microsoft.ReportingServices.ReportRendering.ReportItem reportItem)
			{
				if (reportItem == null || !Visibility.IsVisible(reportItem.SharedHidden, reportItem.Hidden, reportItem.HasToggle) || !m_processingContext.PageSectionContext.IsParentVisible())
				{
					return;
				}
				if (reportItem is Microsoft.ReportingServices.ReportRendering.TextBox)
				{
					TextBoxImpl textBoxImpl = (TextBoxImpl)m_processingContext.ReportObjectModel.ReportItemsImpl[reportItem.Name];
					Global.Tracer.Assert(textBoxImpl != null, "(null != textBoxObj)");
					textBoxImpl.SetResult(new VariantResult(errorOccurred: false, ((Microsoft.ReportingServices.ReportRendering.TextBox)reportItem).OriginalValue));
				}
				else if (reportItem is Microsoft.ReportingServices.ReportRendering.Rectangle)
				{
					FirstPassReportItems(((Microsoft.ReportingServices.ReportRendering.Rectangle)reportItem).ReportItemCollection);
				}
				else if (reportItem is Microsoft.ReportingServices.ReportRendering.List)
				{
					Microsoft.ReportingServices.ReportRendering.List list = (Microsoft.ReportingServices.ReportRendering.List)reportItem;
					if (list.Contents != null)
					{
						for (int i = 0; i < list.Contents.Count; i++)
						{
							ListContent listContent = list.Contents[i];
							if (listContent != null && Visibility.IsVisible(listContent.SharedHidden, listContent.Hidden, listContent.HasToggle))
							{
								FirstPassReportItems(listContent.ReportItemCollection);
							}
						}
					}
				}
				else if (reportItem is Microsoft.ReportingServices.ReportRendering.Table)
				{
					Microsoft.ReportingServices.ReportRendering.Table table = (Microsoft.ReportingServices.ReportRendering.Table)reportItem;
					bool[] array = new bool[table.Columns.Count];
					for (int j = 0; j < array.Length; j++)
					{
						array[j] = Visibility.IsVisible(table.Columns[j].SharedHidden, table.Columns[j].Hidden, table.Columns[j].HasToggle);
					}
					FirstPassTableGroups(array, table.TableHeader, table.DetailRows, table.TableGroups, table.TableFooter);
				}
				else if (reportItem is Microsoft.ReportingServices.ReportRendering.Matrix)
				{
					Microsoft.ReportingServices.ReportRendering.Matrix matrix = (Microsoft.ReportingServices.ReportRendering.Matrix)reportItem;
					FirstPassReportItem(matrix.Corner);
					bool[] cellsCanGetReferenced = new bool[matrix.CellRows];
					bool[] cellsCanGetReferenced2 = new bool[matrix.CellColumns];
					FirstPassMatrixHeadings(matrix.ColumnMemberCollection, isColumn: true, ref cellsCanGetReferenced2);
					FirstPassMatrixHeadings(matrix.RowMemberCollection, isColumn: false, ref cellsCanGetReferenced);
					if (matrix.CellCollection != null)
					{
						for (int k = 0; k < matrix.CellRows; k++)
						{
							if (!cellsCanGetReferenced[k])
							{
								continue;
							}
							for (int l = 0; l < matrix.CellColumns; l++)
							{
								if (cellsCanGetReferenced2[l])
								{
									FirstPassReportItem(matrix.CellCollection[k, l].ReportItem);
								}
							}
						}
					}
				}
				AggregatesImpl aggregatesImpl = null;
				if (reportItem.Name != null)
				{
					aggregatesImpl = (AggregatesImpl)m_aggregatesOverReportItems[reportItem.Name];
				}
				if (aggregatesImpl == null)
				{
					return;
				}
				foreach (DataAggregateObj @object in aggregatesImpl.Objects)
				{
					@object.Update();
				}
			}

			private void FirstPassTableRow(bool[] tableColumnsVisible, Microsoft.ReportingServices.ReportRendering.TableRow row)
			{
				if (row == null || row.TableCellCollection == null || !Visibility.IsVisible(row.SharedHidden, row.Hidden, row.HasToggle))
				{
					return;
				}
				int count = row.TableCellCollection.Count;
				Global.Tracer.Assert(count <= tableColumnsVisible.Length, "(cellCount <= tableColumnsVisible.Length)");
				int num = 0;
				for (int i = 0; i < count; i++)
				{
					int colSpan = row.TableCellCollection[i].ColSpan;
					if (Visibility.IsTableCellVisible(tableColumnsVisible, num, colSpan))
					{
						FirstPassReportItem(row.TableCellCollection[i].ReportItem);
					}
					num += colSpan;
				}
			}

			private void FirstPassTableGroups(bool[] tableColumnsVisible, TableHeaderFooterRows header, TableRowsCollection detailRows, TableGroupCollection subGroups, TableHeaderFooterRows footer)
			{
				if (header != null)
				{
					for (int i = 0; i < header.Count; i++)
					{
						FirstPassTableRow(tableColumnsVisible, header[i]);
					}
				}
				Global.Tracer.Assert(detailRows == null || subGroups == null);
				if (detailRows != null)
				{
					for (int j = 0; j < detailRows.Count; j++)
					{
						if (detailRows[j] != null)
						{
							for (int k = 0; k < detailRows[j].Count; k++)
							{
								FirstPassTableRow(tableColumnsVisible, detailRows[j][k]);
							}
						}
					}
				}
				if (subGroups != null)
				{
					for (int l = 0; l < subGroups.Count; l++)
					{
						if (subGroups[l] != null)
						{
							FirstPassTableGroups(tableColumnsVisible, subGroups[l].GroupHeader, subGroups[l].DetailRows, subGroups[l].SubGroups, subGroups[l].GroupFooter);
						}
					}
				}
				if (footer != null)
				{
					for (int m = 0; m < footer.Count; m++)
					{
						FirstPassTableRow(tableColumnsVisible, footer[m]);
					}
				}
			}

			private void FirstPassMatrixHeadings(MatrixMemberCollection headings, bool isColumn, ref bool[] cellsCanGetReferenced)
			{
				if (headings == null)
				{
					return;
				}
				for (int i = 0; i < headings.Count; i++)
				{
					MatrixMember matrixMember = headings[i];
					if (matrixMember != null)
					{
						m_processingContext.PageSectionContext.EnterMatrixHeadingScope(Visibility.IsVisible(matrixMember.SharedHidden, matrixMember.Hidden, matrixMember.HasToggle), isColumn);
						FirstPassReportItem(matrixMember.ReportItem);
						if (matrixMember.IsTotal)
						{
							m_processingContext.PageSectionContext.EnterMatrixSubtotalScope(isColumn);
						}
						int num = isColumn ? matrixMember.ColumnSpan : matrixMember.RowSpan;
						Global.Tracer.Assert(cellsCanGetReferenced != null && matrixMember.MemberCellIndex >= 0 && num > 0 && matrixMember.MemberCellIndex + num <= cellsCanGetReferenced.Length);
						for (int j = 0; j < num; j++)
						{
							cellsCanGetReferenced[matrixMember.MemberCellIndex + j] = m_processingContext.PageSectionContext.IsParentVisible();
						}
						FirstPassMatrixHeadings(matrixMember.Children, isColumn, ref cellsCanGetReferenced);
						if (matrixMember.IsTotal)
						{
							m_processingContext.PageSectionContext.ExitMatrixHeadingScope(isColumn);
						}
						m_processingContext.PageSectionContext.ExitMatrixHeadingScope(isColumn);
					}
				}
			}

			private void SecondPass()
			{
				if (!m_report.PageHeaderEvaluation)
				{
					m_pageHeader = null;
				}
				else
				{
					PageSectionInstance pageSectionInstance = new PageSectionInstance(m_processingContext, m_pageNumber, m_report.PageHeader);
					CreateInstances(m_processingContext, pageSectionInstance.ReportItemColInstance, m_report.PageHeader.ReportItems);
					string text = m_pageNumber.ToString(CultureInfo.InvariantCulture) + "ph";
					Microsoft.ReportingServices.ReportRendering.RenderingContext renderingContext = new Microsoft.ReportingServices.ReportRendering.RenderingContext(m_renderingReport.RenderingContext, text);
					m_pageHeader = new Microsoft.ReportingServices.ReportRendering.PageSection(text, m_report.PageHeader, pageSectionInstance, m_renderingReport, renderingContext, pageDef: false);
				}
				if (!m_report.PageFooterEvaluation)
				{
					m_pageFooter = null;
					return;
				}
				PageSectionInstance pageSectionInstance2 = new PageSectionInstance(m_processingContext, m_pageNumber, m_report.PageFooter);
				CreateInstances(m_processingContext, pageSectionInstance2.ReportItemColInstance, m_report.PageFooter.ReportItems);
				string text2 = m_pageNumber.ToString(CultureInfo.InvariantCulture) + "pf";
				Microsoft.ReportingServices.ReportRendering.RenderingContext renderingContext2 = new Microsoft.ReportingServices.ReportRendering.RenderingContext(m_renderingReport.RenderingContext, text2);
				m_pageFooter = new Microsoft.ReportingServices.ReportRendering.PageSection(text2, m_report.PageFooter, pageSectionInstance2, m_renderingReport, renderingContext2, pageDef: false);
			}

			internal static void CreateInstances(ProcessingContext processingContext, ReportItemColInstance collectionInstance, ReportItemCollection reportItemsDef)
			{
				if (reportItemsDef.ComputedReportItems == null)
				{
					return;
				}
				ReportItemInstance reportItemInstance = null;
				for (int i = 0; i < reportItemsDef.ComputedReportItems.Count; i++)
				{
					ReportItem reportItem = reportItemsDef.ComputedReportItems[i];
					if (reportItem is TextBox)
					{
						reportItemInstance = RuntimeRICollection.CreateTextBoxInstance((TextBox)reportItem, processingContext, i, null);
					}
					else if (reportItem is Line)
					{
						reportItemInstance = RuntimeRICollection.CreateLineInstance((Line)reportItem, processingContext, i);
					}
					else if (reportItem is Image)
					{
						reportItemInstance = RuntimeRICollection.CreateImageInstance((Image)reportItem, processingContext, i);
					}
					else if (reportItem is ActiveXControl)
					{
						reportItemInstance = RuntimeRICollection.CreateActiveXControlInstance((ActiveXControl)reportItem, processingContext, i);
					}
					else if (reportItem is Rectangle)
					{
						Rectangle rectangle = (Rectangle)reportItem;
						RectangleInstance rectangleInstance = new RectangleInstance(processingContext, rectangle, i);
						CreateInstances(processingContext, rectangleInstance.ReportItemColInstance, rectangle.ReportItems);
						reportItemInstance = rectangleInstance;
					}
					if (reportItemInstance != null)
					{
						collectionInstance.Add(reportItemInstance);
					}
				}
			}
		}

		public delegate bool CheckSharedDataSet(string dataSetPath, out Guid catalogItemId);

		public delegate void ResolveTemporaryDataSet(DataSetInfo dataSetInfo, DataSetInfoCollection originalDataSets);

		internal sealed class CustomReportItemControls
		{
			private class CustomControlInfo
			{
				private bool m_valid;

				private Microsoft.ReportingServices.ReportRendering.ICustomReportItem m_instance;

				internal bool IsValid
				{
					get
					{
						return m_valid;
					}
					set
					{
						m_valid = value;
					}
				}

				internal Microsoft.ReportingServices.ReportRendering.ICustomReportItem Instance
				{
					get
					{
						return m_instance;
					}
					set
					{
						m_instance = value;
					}
				}
			}

			private Hashtable m_controls;

			internal CustomReportItemControls()
			{
				m_controls = new Hashtable();
			}

			internal Microsoft.ReportingServices.ReportRendering.ICustomReportItem GetControlInstance(string name, IExtensionFactory extFactory)
			{
				lock (this)
				{
					CustomControlInfo customControlInfo = m_controls[name] as CustomControlInfo;
					if (customControlInfo == null)
					{
						Microsoft.ReportingServices.ReportRendering.ICustomReportItem customReportItem = null;
						Global.Tracer.Assert(extFactory != null, "extFactory != null");
						customReportItem = (extFactory.GetNewCustomReportItemProcessingInstanceClass(name) as Microsoft.ReportingServices.ReportRendering.ICustomReportItem);
						customControlInfo = new CustomControlInfo();
						customControlInfo.Instance = customReportItem;
						customControlInfo.IsValid = (customReportItem != null);
						m_controls.Add(name, customControlInfo);
					}
					Global.Tracer.Assert(customControlInfo != null);
					if (customControlInfo.IsValid)
					{
						return customControlInfo.Instance;
					}
					return null;
				}
			}
		}

		private abstract class RuntimeTablixObj : RuntimeRDLDataRegionObj
		{
			protected Tablix m_tablixDef;

			protected RuntimeTablixHeadingsObj m_tablixRows;

			protected RuntimeTablixHeadingsObj m_tablixColumns;

			protected RuntimeTablixHeadingsObj m_outerGroupings;

			protected RuntimeTablixHeadingsObj m_innerGroupings;

			protected int[] m_outerGroupingCounters;

			protected override IScope OuterScope => m_outerScope;

			protected override string ScopeName => m_tablixDef.Name;

			protected override DataRegion DataRegionDef => m_tablixDef;

			internal int[] OuterGroupingCounters => m_outerGroupingCounters;

			internal RuntimeTablixObj(IScope outerScope, Tablix tablixDef, ref DataActions dataAction, ProcessingContext processingContext, bool onePassProcess)
				: base(outerScope, tablixDef, ref dataAction, processingContext, onePassProcess, tablixDef.RunningValues)
			{
				m_outerScope = outerScope;
				m_tablixDef = tablixDef;
			}

			protected void ConstructorHelper(ref DataActions dataAction, bool onePassProcess, out bool handleMyDataAction, out DataActions innerDataAction, out TablixHeadingList outermostColumns, out TablixHeadingList outermostRows, out TablixHeadingList staticColumns, out TablixHeadingList staticRows)
			{
				if (m_tablixDef.Filters != null)
				{
					m_filters = new Filters(Filters.FilterTypes.DataRegionFilter, this, m_tablixDef.Filters, m_tablixDef.ObjectType, m_tablixDef.Name, m_processingContext);
				}
				else
				{
					m_outerDataAction = dataAction;
					m_dataAction = dataAction;
					dataAction = DataActions.None;
				}
				m_tablixDef.GetHeadingDefState(out outermostColumns, out outermostRows, out staticColumns, out staticRows);
				innerDataAction = m_dataAction;
				handleMyDataAction = false;
				bool flag = false;
				RuntimeDataRegionObj.CreateAggregates(m_processingContext, m_tablixDef.Aggregates, ref m_nonCustomAggregates, ref m_customAggregates);
				if (onePassProcess)
				{
					flag = true;
					if (m_tablixDef.RunningValues != null && 0 < m_tablixDef.RunningValues.Count)
					{
						RuntimeDataRegionObj.CreateAggregates(m_processingContext, m_tablixDef.RunningValues, ref m_nonCustomAggregates);
					}
					RuntimeDataRegionObj.CreateAggregates(m_processingContext, m_tablixDef.PostSortAggregates, ref m_nonCustomAggregates);
					Global.Tracer.Assert(outermostRows == null && outermostColumns == null);
					RuntimeDataRegionObj.CreateAggregates(m_processingContext, m_tablixDef.CellPostSortAggregates, ref m_nonCustomAggregates);
				}
				else
				{
					if (m_tablixDef.RunningValues != null && 0 < m_tablixDef.RunningValues.Count)
					{
						m_dataAction |= DataActions.PostSortAggregates;
					}
					if (m_tablixDef.PostSortAggregates != null)
					{
						RuntimeDataRegionObj.CreateAggregates(m_processingContext, m_tablixDef.PostSortAggregates, ref m_postSortAggregates);
						handleMyDataAction = true;
					}
					if (outermostRows == null && outermostColumns == null)
					{
						flag = true;
						if (m_tablixDef.CellPostSortAggregates != null)
						{
							RuntimeDataRegionObj.CreateAggregates(m_processingContext, m_tablixDef.CellPostSortAggregates, ref m_postSortAggregates);
							handleMyDataAction = true;
						}
					}
					if (handleMyDataAction)
					{
						m_dataAction |= DataActions.PostSortAggregates;
						innerDataAction = DataActions.None;
					}
					else
					{
						innerDataAction = m_dataAction;
					}
				}
				if (flag)
				{
					RuntimeDataRegionObj.CreateAggregates(m_processingContext, m_tablixDef.CellAggregates, ref m_nonCustomAggregates, ref m_customAggregates);
					RunningValueInfoList tablixCellRunningValues = m_tablixDef.TablixCellRunningValues;
					if (tablixCellRunningValues != null && 0 < tablixCellRunningValues.Count)
					{
						if (m_nonCustomAggregates == null)
						{
							m_nonCustomAggregates = new DataAggregateObjList();
						}
						for (int i = 0; i < tablixCellRunningValues.Count; i++)
						{
							m_nonCustomAggregates.Add(new DataAggregateObj(tablixCellRunningValues[i], m_processingContext));
						}
					}
				}
				int num = m_tablixDef.CreateOuterGroupingIndexList();
				m_outerGroupingCounters = new int[num];
				for (int j = 0; j < m_outerGroupingCounters.Length; j++)
				{
					m_outerGroupingCounters[j] = -1;
				}
			}

			protected void HandleDataAction(bool handleMyDataAction, DataActions innerDataAction)
			{
				if (!handleMyDataAction)
				{
					m_dataAction = innerDataAction;
				}
				if (m_dataAction != 0)
				{
					m_dataRows = new DataRowList();
				}
			}

			protected override void SendToInner()
			{
				m_tablixDef.RuntimeDataRegionObj = this;
				m_tablixDef.ResetOutergGroupingAggregateRowInfo();
				m_tablixDef.SaveTablixAggregateRowInfo(m_processingContext);
				if (m_outerGroupings != null)
				{
					m_outerGroupings.NextRow();
				}
				m_tablixDef.RestoreTablixAggregateRowInfo(m_processingContext);
				if (m_innerGroupings != null)
				{
					m_innerGroupings.NextRow();
				}
			}

			internal override bool SortAndFilter()
			{
				SetupEnvironment();
				if (m_tablixRows != null)
				{
					m_tablixRows.SortAndFilter();
				}
				if (m_tablixColumns != null)
				{
					m_tablixColumns.SortAndFilter();
				}
				return base.SortAndFilter();
			}

			internal override void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
				if (m_tablixDef.RunningValues != null && m_runningValues == null)
				{
					RuntimeDetailObj.AddRunningValues(m_processingContext, m_tablixDef.RunningValues, ref m_runningValues, globalRVCol, groupCol, lastGroup);
				}
				if (m_dataRows != null)
				{
					ReadRows(DataActions.PostSortAggregates);
					m_dataRows = null;
				}
				if (m_outerGroupings != null)
				{
					m_outerGroupings.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
				}
				if ((m_outerGroupings == null || m_outerGroupings.Headings == null) && m_innerGroupings != null)
				{
					m_innerGroupings.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
				}
			}

			protected virtual void CalculatePreviousAggregates()
			{
				if (!m_processedPreviousAggregates && m_processingContext.GlobalRVCollection != null)
				{
					Global.Tracer.Assert(m_runningValueValues == null);
					AggregatesImpl globalRVCollection = m_processingContext.GlobalRVCollection;
					RunningValueInfoList runningValues = m_tablixDef.RunningValues;
					RuntimeRICollection.DoneReadingRows(globalRVCollection, runningValues, ref m_runningValueValues, processPreviousAggregates: true);
					if (m_tablixRows != null)
					{
						m_tablixRows.CalculatePreviousAggregates(globalRVCollection);
					}
					if (m_tablixColumns != null)
					{
						m_tablixColumns.CalculatePreviousAggregates(globalRVCollection);
					}
					m_processedPreviousAggregates = true;
				}
			}

			internal override void ReadRow(DataActions dataAction)
			{
				if (m_tablixDef.ProcessCellRunningValues)
				{
					return;
				}
				if (DataActions.UserSort == dataAction)
				{
					RuntimeDataRegionObj.CommonFirstRow(m_processingContext.ReportObjectModel.FieldsImpl, ref m_firstRowIsAggregate, ref m_firstRow);
					CommonNextRow(m_dataRows);
					return;
				}
				if (DataActions.PostSortAggregates == dataAction)
				{
					if (m_postSortAggregates != null)
					{
						RuntimeDataRegionObj.UpdateAggregates(m_processingContext, m_postSortAggregates, updateAndSetup: false);
					}
					if (m_runningValues != null)
					{
						for (int i = 0; i < m_runningValues.Count; i++)
						{
							m_runningValues[i].Update();
						}
					}
					CalculatePreviousAggregates();
				}
				if (m_outerScope != null && (dataAction & m_outerDataAction) != 0)
				{
					m_outerScope.ReadRow(dataAction);
				}
			}

			internal override void SetupEnvironment()
			{
				SetupEnvironment(m_tablixDef.RunningValues);
			}

			internal override bool InScope(string scope)
			{
				return DataRegionInScope(m_tablixDef, scope);
			}

			protected override int GetRecursiveLevel(string scope)
			{
				return DataRegionRecursiveLevel(m_tablixDef, scope);
			}
		}

		internal abstract class RuntimeTablixHeadingsObj
		{
			protected IScope m_owner;

			protected RuntimeTablixGroupRootObj m_tablixHeadings;

			protected TablixHeadingList m_staticHeadingDef;

			internal RuntimeTablixGroupRootObj Headings => m_tablixHeadings;

			internal RuntimeTablixHeadingsObj(IScope owner, TablixHeadingList headingDef, ref DataActions dataAction, ProcessingContext processingContext, TablixHeadingList staticHeadingDef, RuntimeTablixHeadingsObj innerGroupings, int headingLevel)
			{
				m_owner = owner;
				if (staticHeadingDef != null)
				{
					m_staticHeadingDef = staticHeadingDef;
				}
			}

			internal virtual void NextRow()
			{
				if (m_tablixHeadings != null)
				{
					m_tablixHeadings.NextRow();
				}
			}

			internal virtual bool SortAndFilter()
			{
				if (m_tablixHeadings != null)
				{
					return m_tablixHeadings.SortAndFilter();
				}
				return true;
			}

			internal virtual void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
				if (m_tablixHeadings != null)
				{
					m_tablixHeadings.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
				}
			}

			internal abstract void CalculatePreviousAggregates(AggregatesImpl globalRVCol);
		}

		internal abstract class RuntimeTablixGroupRootObj : RuntimeGroupRootObj
		{
			protected RuntimeTablixHeadingsObj m_innerGroupings;

			protected TablixHeadingList m_staticHeadingDef;

			protected bool m_outermostSubtotal;

			protected TablixHeadingList m_innerHeading;

			protected bool m_processOutermostSTCells;

			protected DataAggregateObjList m_outermostSTCellRVs;

			protected DataAggregateObjList m_cellRVs;

			protected int m_headingIndex = -1;

			protected int m_headingLevel;

			protected object m_currentGroupExprValue;

			internal RuntimeTablixHeadingsObj InnerGroupings => m_innerGroupings;

			internal TablixHeadingList StaticHeadingDef => m_staticHeadingDef;

			internal bool OutermostSubtotal => m_outermostSubtotal;

			internal TablixHeadingList InnerHeading => m_innerHeading;

			internal bool ProcessOutermostSTCells => m_processOutermostSTCells;

			internal AggregatesImpl OutermostSTCellRVCol => ((TablixHeading)m_hierarchyDef).OutermostSTCellRVCol;

			internal AggregatesImpl[] OutermostSTScopedCellRVCollections => ((TablixHeading)m_hierarchyDef).OutermostSTCellScopedRVCollections;

			internal AggregatesImpl CellRVCol => ((TablixHeading)m_hierarchyDef).CellRVCol;

			internal AggregatesImpl[] CellScopedRVCollections => ((TablixHeading)m_hierarchyDef).CellScopedRVCollections;

			internal int HeadingLevel => m_headingLevel;

			internal object CurrentGroupExpressionValue => m_currentGroupExprValue;

			internal RuntimeTablixGroupRootObj(IScope outerScope, TablixHeadingList tablixHeadingDef, int headingIndex, ref DataActions dataAction, ProcessingContext processingContext, RuntimeTablixHeadingsObj innerGroupings, bool outermostSubtotal, int headingLevel)
				: base(outerScope, tablixHeadingDef[headingIndex], dataAction, processingContext)
			{
				Tablix tablix = (Tablix)tablixHeadingDef[headingIndex].DataRegionDef;
				Global.Tracer.Assert(tablixHeadingDef != null && headingIndex < tablixHeadingDef.Count && 0 <= headingIndex);
				m_headingIndex = headingIndex;
				m_innerHeading = ((CustomReportItemHeadingList)tablixHeadingDef)[headingIndex].InnerHeadings;
				tablix.SkipStaticHeading(ref m_innerHeading, ref m_staticHeadingDef);
				if (outermostSubtotal && m_innerHeading == null)
				{
					m_processOutermostSTCells = true;
					if (tablix.CellPostSortAggregates != null)
					{
						m_dataAction |= DataActions.PostSortAggregates;
					}
				}
				NeedProcessDataActions(tablixHeadingDef);
				NeedProcessDataActions(m_staticHeadingDef);
				m_outermostSubtotal = outermostSubtotal;
				m_innerGroupings = innerGroupings;
				m_headingLevel = headingLevel;
				if (tablixHeadingDef[headingIndex].Grouping.Filters == null)
				{
					dataAction = DataActions.None;
				}
			}

			protected abstract void NeedProcessDataActions(TablixHeadingList heading);

			protected void NeedProcessDataActions(RunningValueInfoList runningValues)
			{
				if ((m_dataAction & DataActions.PostSortAggregates) == 0 && runningValues != null && 0 < runningValues.Count)
				{
					m_dataAction |= DataActions.PostSortAggregates;
				}
			}

			internal override void NextRow()
			{
				if (ProcessThisRow())
				{
					m_currentGroupExprValue = EvaluateGroupExpression(m_expression, "Group");
					Grouping grouping = m_hierarchyDef.Grouping;
					if (m_saveGroupExprValues)
					{
						grouping.CurrentGroupExpressionValues = new VariantList();
						grouping.CurrentGroupExpressionValues.Add(m_currentGroupExprValue);
					}
					object parentKey = null;
					bool flag = m_parentExpression != null;
					if (flag)
					{
						parentKey = EvaluateGroupExpression(m_parentExpression, "Parent");
					}
					Global.Tracer.Assert(m_grouping != null);
					m_grouping.NextRow(m_currentGroupExprValue, flag, parentKey);
				}
			}

			internal override bool SortAndFilter()
			{
				Tablix tablix = (Tablix)m_hierarchyDef.DataRegionDef;
				TablixHeading tablixHeading = (TablixHeading)m_hierarchyDef;
				Global.Tracer.Assert(m_hierarchyDef.Grouping != null);
				if ((ProcessingContext.SecondPassOperations.Filtering & m_processingContext.SecondPassOperation) != 0 && m_hierarchyDef.Grouping.Filters == null && ((tablixHeading.IsColumn && m_headingLevel < tablix.InnermostColumnFilterLevel) || (!tablixHeading.IsColumn && m_headingLevel < tablix.InnermostRowFilterLevel)))
				{
					tablixHeading.Grouping.HasInnerFilters = true;
				}
				bool result = base.SortAndFilter();
				tablixHeading.Grouping.HasInnerFilters = false;
				return result;
			}

			internal override void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
				base.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
				Tablix tablix = (Tablix)m_hierarchyDef.DataRegionDef;
				TablixHeading tablixHeading = (TablixHeading)m_hierarchyDef;
				AggregatesImpl globalCellRVCol = tablixHeading.OutermostSTCellRVCol;
				AggregatesImpl[] cellScopedRVLists = tablixHeading.OutermostSTCellScopedRVCollections;
				if (SetupCellRunningValues(ref globalCellRVCol, ref cellScopedRVLists))
				{
					tablixHeading.OutermostSTCellRVCol = globalCellRVCol;
					tablixHeading.OutermostSTCellScopedRVCollections = cellScopedRVLists;
				}
				if (m_processOutermostSTCells)
				{
					if (m_innerGroupings != null)
					{
						tablix.CurrentOuterHeadingGroupRoot = this;
					}
					m_processingContext.EnterPivotCell(m_innerGroupings != null);
					tablix.ProcessOutermostSTCellRunningValues = true;
					AddCellRunningValues(globalCellRVCol, groupCol, ref m_outermostSTCellRVs);
					tablix.ProcessOutermostSTCellRunningValues = false;
					m_processingContext.ExitPivotCell();
				}
				if (m_innerGroupings != null)
				{
					AggregatesImpl globalCellRVCol2 = tablixHeading.CellRVCol;
					AggregatesImpl[] cellScopedRVLists2 = tablixHeading.CellScopedRVCollections;
					if (SetupCellRunningValues(ref globalCellRVCol2, ref cellScopedRVLists2))
					{
						tablixHeading.CellRVCol = globalCellRVCol2;
						tablixHeading.CellScopedRVCollections = cellScopedRVLists2;
					}
					return;
				}
				RuntimeTablixGroupRootObj currentOuterHeadingGroupRoot = tablix.CurrentOuterHeadingGroupRoot;
				if (m_innerHeading == null && currentOuterHeadingGroupRoot != null)
				{
					m_processingContext.EnterPivotCell(escalateScope: true);
					tablix.ProcessCellRunningValues = true;
					m_cellRVs = null;
					AddCellRunningValues(currentOuterHeadingGroupRoot.CellRVCol, groupCol, ref m_cellRVs);
					tablix.ProcessCellRunningValues = false;
					m_processingContext.ExitPivotCell();
				}
			}

			private bool SetupCellRunningValues(ref AggregatesImpl globalCellRVCol, ref AggregatesImpl[] cellScopedRVLists)
			{
				if (globalCellRVCol == null || cellScopedRVLists == null)
				{
					globalCellRVCol = new AggregatesImpl(m_processingContext.ReportRuntime);
					cellScopedRVLists = CreateScopedCellRVCollections();
					return true;
				}
				return false;
			}

			protected abstract void AddCellRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, ref DataAggregateObjList runningValues);

			internal override void AddScopedRunningValue(DataAggregateObj runningValueObj, bool escalate)
			{
				Tablix tablix = (Tablix)m_hierarchyDef.DataRegionDef;
				if (tablix.ProcessOutermostSTCellRunningValues || tablix.ProcessCellRunningValues)
				{
					RuntimeTablixGroupRootObj currentOuterHeadingGroupRoot = tablix.CurrentOuterHeadingGroupRoot;
					int headingLevel = currentOuterHeadingGroupRoot.HeadingLevel;
					TablixHeading tablixHeading = (!escalate) ? ((TablixHeading)m_hierarchyDef) : ((TablixHeading)currentOuterHeadingGroupRoot.HierarchyDef);
					if (tablix.ProcessOutermostSTCellRunningValues)
					{
						AddCellScopedRunningValue(runningValueObj, tablixHeading.OutermostSTCellScopedRVCollections, headingLevel);
					}
					else if (tablix.ProcessCellRunningValues)
					{
						AddCellScopedRunningValue(runningValueObj, tablixHeading.CellScopedRVCollections, headingLevel);
					}
				}
				else
				{
					base.AddScopedRunningValue(runningValueObj, escalate);
				}
			}

			private void AddCellScopedRunningValue(DataAggregateObj runningValueObj, AggregatesImpl[] cellScopedRVLists, int currentOuterHeadingLevel)
			{
				if (cellScopedRVLists != null)
				{
					AggregatesImpl aggregatesImpl = cellScopedRVLists[currentOuterHeadingLevel];
					if (aggregatesImpl == null)
					{
						aggregatesImpl = (cellScopedRVLists[currentOuterHeadingLevel] = new AggregatesImpl(m_processingContext.ReportRuntime));
					}
					if (aggregatesImpl.GetAggregateObj(runningValueObj.Name) == null)
					{
						aggregatesImpl.Add(runningValueObj);
					}
				}
			}

			internal override void ReadRow(DataActions dataAction)
			{
				Tablix tablix = (Tablix)m_hierarchyDef.DataRegionDef;
				if (tablix.ProcessCellRunningValues)
				{
					Global.Tracer.Assert(DataActions.PostSortAggregates == dataAction);
					if (m_cellRVs != null)
					{
						for (int i = 0; i < m_cellRVs.Count; i++)
						{
							m_cellRVs[i].Update();
						}
					}
					if (m_outerScope != null && tablix.CellPostSortAggregates != null)
					{
						m_outerScope.ReadRow(dataAction);
					}
					return;
				}
				if (DataActions.PostSortAggregates == dataAction && m_outermostSTCellRVs != null)
				{
					for (int j = 0; j < m_outermostSTCellRVs.Count; j++)
					{
						m_outermostSTCellRVs[j].Update();
					}
				}
				base.ReadRow(dataAction);
			}

			private AggregatesImpl[] CreateScopedCellRVCollections()
			{
				int dynamicHeadingCount = ((Tablix)m_hierarchyDef.DataRegionDef).GetDynamicHeadingCount(outerGroupings: true);
				if (0 < dynamicHeadingCount)
				{
					return new AggregatesImpl[dynamicHeadingCount];
				}
				return null;
			}

			internal bool GetCellTargetForNonDetailSort()
			{
				if (m_outerScope is RuntimeTablixObj)
				{
					return m_outerScope.TargetForNonDetailSort;
				}
				Global.Tracer.Assert(m_outerScope is RuntimeTablixGroupLeafObj);
				return ((RuntimeTablixGroupLeafObj)m_outerScope).GetCellTargetForNonDetailSort();
			}

			internal bool GetCellTargetForSort(int index, bool detailSort)
			{
				if (m_outerScope is RuntimeTablixObj)
				{
					return m_outerScope.IsTargetForSort(index, detailSort);
				}
				Global.Tracer.Assert(m_outerScope is RuntimeTablixGroupLeafObj);
				return ((RuntimeTablixGroupLeafObj)m_outerScope).GetCellTargetForSort(index, detailSort);
			}
		}

		private abstract class RuntimeTablixGroupLeafObj : RuntimeGroupLeafObj
		{
			protected RuntimeTablixHeadingsObj m_tablixHeadings;

			protected RuntimeTablixGroupRootObj m_innerHeadingList;

			protected DataAggregateObjList m_firstPassCellNonCustomAggs;

			protected DataAggregateObjList m_firstPassCellCustomAggs;

			protected RuntimeTablixCells[] m_cellsList;

			protected DataAggregateObjList m_cellPostSortAggregates;

			protected int m_groupLeafIndex = -1;

			protected bool m_processHeading = true;

			internal TablixHeading TablixHeadingDef => (TablixHeading)((RuntimeTablixGroupRootObj)m_hierarchyRoot).HierarchyDef;

			internal DataAggregateObjList CellPostSortAggregates => m_cellPostSortAggregates;

			internal Tablix TablixDef => (Tablix)TablixHeadingDef.DataRegionDef;

			internal int HeadingLevel => ((RuntimeTablixGroupRootObj)m_hierarchyRoot).HeadingLevel;

			internal RuntimeTablixGroupLeafObj(RuntimeTablixGroupRootObj groupRoot)
				: base(groupRoot)
			{
			}

			protected void ConstructorHelper(RuntimeTablixGroupRootObj groupRoot, Tablix tablixDef, out bool handleMyDataAction, out DataActions innerDataAction)
			{
				_ = groupRoot.InnerHeading;
				m_dataAction = groupRoot.DataAction;
				handleMyDataAction = false;
				if (m_postSortAggregates != null || (m_recursiveAggregates != null && m_processingContext.SpecialRecursiveAggregates))
				{
					handleMyDataAction = true;
				}
				if (groupRoot.ProcessOutermostSTCells)
				{
					RuntimeDataRegionObj.CreateAggregates(m_processingContext, tablixDef.CellAggregates, ref m_firstPassCellNonCustomAggs, ref m_firstPassCellCustomAggs);
					if (tablixDef.CellPostSortAggregates != null)
					{
						handleMyDataAction = true;
						RuntimeDataRegionObj.CreateAggregates(m_processingContext, tablixDef.CellPostSortAggregates, ref m_postSortAggregates);
					}
				}
				if (handleMyDataAction)
				{
					innerDataAction = DataActions.None;
				}
				else
				{
					innerDataAction = m_dataAction;
				}
				if (!IsOuterGrouping())
				{
					if (groupRoot.InnerHeading == null)
					{
						TablixHeadingList staticHeading = null;
						TablixHeadingList tablixHeading = tablixDef.GetOuterHeading();
						int dynamicHeadingCount = tablixDef.GetDynamicHeadingCount(outerGroupings: true);
						int num = 0;
						tablixDef.SkipStaticHeading(ref tablixHeading, ref staticHeading);
						while (tablixHeading != null)
						{
							tablixHeading = tablixHeading.InnerHeadings();
							tablixDef.SkipStaticHeading(ref tablixHeading, ref staticHeading);
							if (m_cellsList == null)
							{
								m_cellsList = new RuntimeTablixCells[dynamicHeadingCount];
								RuntimeDataRegionObj.CreateAggregates(m_processingContext, tablixDef.CellPostSortAggregates, ref m_cellPostSortAggregates);
							}
							RuntimeTablixCells runtimeTablixCells = null;
							if (tablixHeading == null)
							{
								runtimeTablixCells = new RuntimeTablixCells();
							}
							m_cellsList[num++] = runtimeTablixCells;
						}
					}
				}
				else
				{
					m_groupLeafIndex = ++((RuntimeTablixObj)tablixDef.RuntimeDataRegionObj).OuterGroupingCounters[groupRoot.HeadingLevel];
				}
				TablixHeading tablixHeading2 = (TablixHeading)groupRoot.HierarchyDef;
				Global.Tracer.Assert(tablixHeading2.Grouping != null);
				if (tablixHeading2.Grouping.Filters == null)
				{
					return;
				}
				if (tablixHeading2.IsColumn)
				{
					if (groupRoot.HeadingLevel > tablixDef.InnermostColumnFilterLevel)
					{
						tablixDef.InnermostColumnFilterLevel = groupRoot.HeadingLevel;
					}
				}
				else if (groupRoot.HeadingLevel > tablixDef.InnermostRowFilterLevel)
				{
					tablixDef.InnermostRowFilterLevel = groupRoot.HeadingLevel;
				}
			}

			internal abstract RuntimeTablixCell CreateCell(int index, Tablix tablixDef);

			internal override void NextRow()
			{
				Tablix tablixDef = TablixDef;
				int headingLevel = HeadingLevel;
				bool num = IsOuterGrouping();
				if (num)
				{
					tablixDef.OuterGroupingIndexes[headingLevel] = m_groupLeafIndex;
				}
				base.NextRow();
				if (num)
				{
					tablixDef.SaveOuterGroupingAggregateRowInfo(headingLevel, m_processingContext);
				}
				FieldsImpl fieldsImpl = m_processingContext.ReportObjectModel.FieldsImpl;
				if (fieldsImpl.AggregationFieldCount == 0 && fieldsImpl.ValidAggregateRow)
				{
					RuntimeDataRegionObj.UpdateAggregates(m_processingContext, m_firstPassCellCustomAggs, updateAndSetup: false);
				}
				if (!m_processingContext.ReportObjectModel.FieldsImpl.IsAggregateRow)
				{
					RuntimeDataRegionObj.UpdateAggregates(m_processingContext, m_firstPassCellNonCustomAggs, updateAndSetup: false);
				}
			}

			protected override void SendToInner()
			{
				base.SendToInner();
				if (m_tablixHeadings != null)
				{
					m_tablixHeadings.NextRow();
				}
				if (m_cellsList == null)
				{
					return;
				}
				Global.Tracer.Assert(!IsOuterGrouping());
				Tablix tablixDef = TablixDef;
				int[] outerGroupingIndexes = tablixDef.OuterGroupingIndexes;
				for (int i = 0; i < tablixDef.GetDynamicHeadingCount(outerGroupings: true); i++)
				{
					int num = outerGroupingIndexes[i];
					AggregateRowInfo aggregateRowInfo = new AggregateRowInfo();
					aggregateRowInfo.SaveAggregateInfo(m_processingContext);
					tablixDef.SetCellAggregateRowInfo(i, m_processingContext);
					RuntimeTablixCells runtimeTablixCells = m_cellsList[i];
					if (runtimeTablixCells != null)
					{
						RuntimeTablixCell runtimeTablixCell = runtimeTablixCells[num];
						if (runtimeTablixCell == null)
						{
							runtimeTablixCell = CreateCell(i, tablixDef);
							runtimeTablixCells.Add(num, runtimeTablixCell);
						}
						runtimeTablixCell.NextRow();
					}
					aggregateRowInfo.RestoreAggregateInfo(m_processingContext);
				}
			}

			internal override bool SortAndFilter()
			{
				SetupEnvironment();
				RuntimeTablixGroupRootObj runtimeTablixGroupRootObj = (RuntimeTablixGroupRootObj)m_hierarchyRoot;
				bool flag = false;
				if (m_innerHeadingList != null && !m_tablixHeadings.SortAndFilter())
				{
					Global.Tracer.Assert((ProcessingContext.SecondPassOperations.Filtering & m_processingContext.SecondPassOperation) != 0);
					Global.Tracer.Assert(runtimeTablixGroupRootObj.GroupFilters != null);
					runtimeTablixGroupRootObj.GroupFilters.FailFilters = true;
					flag = true;
				}
				bool flag2 = base.SortAndFilter();
				if (flag)
				{
					runtimeTablixGroupRootObj.GroupFilters.FailFilters = false;
				}
				if (flag2 && m_cellsList != null)
				{
					for (int i = 0; i < m_cellsList.Length; i++)
					{
						if (m_cellsList[i] != null)
						{
							m_cellsList[i].SortAndFilter();
						}
					}
				}
				return flag2;
			}

			internal override void CalculateRunningValues()
			{
				Tablix tablixDef = TablixDef;
				RuntimeTablixGroupRootObj runtimeTablixGroupRootObj = (RuntimeTablixGroupRootObj)m_hierarchyRoot;
				AggregatesImpl globalRunningValueCollection = runtimeTablixGroupRootObj.GlobalRunningValueCollection;
				RuntimeGroupRootObjList groupCollection = runtimeTablixGroupRootObj.GroupCollection;
				bool num = IsOuterGrouping();
				tablixDef.GetDynamicHeadingCount(outerGroupings: true);
				if (m_processHeading)
				{
					if (m_dataRows != null && (DataActions.PostSortAggregates & m_dataAction) != 0)
					{
						ReadRows(DataActions.PostSortAggregates);
						m_dataRows = null;
					}
					m_tablixHeadings.CalculateRunningValues(globalRunningValueCollection, groupCollection, runtimeTablixGroupRootObj);
				}
				else if (m_innerHeadingList != null)
				{
					m_innerHeadingList.CalculateRunningValues(globalRunningValueCollection, groupCollection, runtimeTablixGroupRootObj);
				}
				if (num)
				{
					if (m_innerHeadingList == null)
					{
						tablixDef.CurrentOuterHeadingGroupRoot = runtimeTablixGroupRootObj;
						tablixDef.OuterGroupingIndexes[runtimeTablixGroupRootObj.HeadingLevel] = m_groupLeafIndex;
						runtimeTablixGroupRootObj.InnerGroupings.CalculateRunningValues(globalRunningValueCollection, groupCollection, runtimeTablixGroupRootObj);
					}
				}
				else if (m_cellsList != null)
				{
					RuntimeTablixGroupRootObj currentOuterHeadingGroupRoot = tablixDef.CurrentOuterHeadingGroupRoot;
					RuntimeTablixCells runtimeTablixCells = m_cellsList[currentOuterHeadingGroupRoot.HeadingLevel];
					Global.Tracer.Assert(runtimeTablixCells != null);
					tablixDef.ProcessCellRunningValues = true;
					runtimeTablixCells.CalculateRunningValues(tablixDef, currentOuterHeadingGroupRoot.CellRVCol, groupCollection, runtimeTablixGroupRootObj, this, currentOuterHeadingGroupRoot.HeadingLevel);
					tablixDef.ProcessCellRunningValues = false;
				}
			}

			protected override void ResetScopedRunningValues()
			{
				base.ResetScopedRunningValues();
				ResetScopedCellRunningValues();
			}

			internal bool IsOuterGrouping()
			{
				RuntimeTablixGroupRootObj runtimeTablixGroupRootObj = (RuntimeTablixGroupRootObj)m_hierarchyRoot;
				return runtimeTablixGroupRootObj.InnerGroupings != null;
			}

			internal override void ReadRow(DataActions dataAction)
			{
				if (DataActions.UserSort == dataAction)
				{
					RuntimeDataRegionObj.CommonFirstRow(m_processingContext.ReportObjectModel.FieldsImpl, ref m_firstRowIsAggregate, ref m_firstRow);
					CommonNextRow(m_dataRows);
				}
				else if (TablixDef.ProcessCellRunningValues)
				{
					if (DataActions.PostSortAggregates == dataAction && m_cellPostSortAggregates != null)
					{
						RuntimeDataRegionObj.UpdateAggregates(m_processingContext, m_cellPostSortAggregates, updateAndSetup: false);
					}
					((IScope)m_hierarchyRoot).ReadRow(dataAction);
				}
				else
				{
					base.ReadRow(dataAction);
					if (DataActions.PostSortAggregates == dataAction)
					{
						CalculatePreviousAggregates();
					}
				}
			}

			protected virtual bool CalculatePreviousAggregates()
			{
				if (!m_processedPreviousAggregates && m_processingContext.GlobalRVCollection != null)
				{
					if (m_innerHeadingList != null)
					{
						m_tablixHeadings.CalculatePreviousAggregates(m_processingContext.GlobalRVCollection);
					}
					m_processedPreviousAggregates = true;
					return true;
				}
				return false;
			}

			protected void ResetScopedCellRunningValues()
			{
				RuntimeTablixGroupRootObj runtimeTablixGroupRootObj = (RuntimeTablixGroupRootObj)m_hierarchyRoot;
				if (runtimeTablixGroupRootObj.OutermostSTScopedCellRVCollections != null)
				{
					for (int i = 0; i < runtimeTablixGroupRootObj.OutermostSTScopedCellRVCollections.Length; i++)
					{
						AggregatesImpl aggregatesImpl = runtimeTablixGroupRootObj.OutermostSTScopedCellRVCollections[i];
						if (aggregatesImpl == null)
						{
							continue;
						}
						foreach (DataAggregateObj @object in aggregatesImpl.Objects)
						{
							@object.Init();
						}
					}
				}
				if (runtimeTablixGroupRootObj.CellScopedRVCollections == null)
				{
					return;
				}
				for (int j = 0; j < runtimeTablixGroupRootObj.CellScopedRVCollections.Length; j++)
				{
					AggregatesImpl aggregatesImpl2 = runtimeTablixGroupRootObj.CellScopedRVCollections[j];
					if (aggregatesImpl2 == null)
					{
						continue;
					}
					foreach (DataAggregateObj object2 in aggregatesImpl2.Objects)
					{
						object2.Init();
					}
				}
			}

			internal override void SetupEnvironment()
			{
				base.SetupEnvironment();
				SetupAggregateValues(m_firstPassCellNonCustomAggs, m_firstPassCellCustomAggs);
			}

			private void SetupAggregateValues(DataAggregateObjList nonCustomAggCollection, DataAggregateObjList customAggCollection)
			{
				SetupAggregates(nonCustomAggCollection);
				SetupAggregates(customAggCollection);
			}

			internal bool GetCellTargetForNonDetailSort()
			{
				return ((RuntimeTablixGroupRootObj)m_hierarchyRoot).GetCellTargetForNonDetailSort();
			}

			internal bool GetCellTargetForSort(int index, bool detailSort)
			{
				return ((RuntimeTablixGroupRootObj)m_hierarchyRoot).GetCellTargetForSort(index, detailSort);
			}
		}

		private sealed class RuntimeTablixCells : Hashtable
		{
			private RuntimeTablixCell m_firstCell;

			private RuntimeTablixCell m_lastCell;

			internal RuntimeTablixCell this[int index]
			{
				get
				{
					return (RuntimeTablixCell)base[index];
				}
				set
				{
					if (base.Count == 0)
					{
						m_firstCell = value;
					}
					base[index] = value;
				}
			}

			internal void Add(int key, RuntimeTablixCell cell)
			{
				if (m_lastCell != null)
				{
					m_lastCell.NextCell = cell;
				}
				else
				{
					m_firstCell = cell;
				}
				m_lastCell = cell;
				base.Add(key, cell);
			}

			internal RuntimeTablixCell GetCell(Tablix tablixDef, RuntimeTablixGroupLeafObj owner, int cellLevel)
			{
				RuntimeTablixGroupRootObj currentOuterHeadingGroupRoot = tablixDef.CurrentOuterHeadingGroupRoot;
				int index = tablixDef.OuterGroupingIndexes[currentOuterHeadingGroupRoot.HeadingLevel];
				RuntimeTablixCell runtimeTablixCell = this[index];
				if (runtimeTablixCell == null)
				{
					runtimeTablixCell = (this[index] = owner.CreateCell(cellLevel, tablixDef));
				}
				return runtimeTablixCell;
			}

			internal void SortAndFilter()
			{
				for (RuntimeTablixCell runtimeTablixCell = m_firstCell; runtimeTablixCell != null; runtimeTablixCell = runtimeTablixCell.NextCell)
				{
					runtimeTablixCell.SortAndFilter();
				}
			}

			internal void CalculateRunningValues(Tablix tablixDef, AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup, RuntimeTablixGroupLeafObj owner, int cellLevel)
			{
				RuntimeTablixCell cell = GetCell(tablixDef, owner, cellLevel);
				Global.Tracer.Assert(cell != null);
				cell.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
			}
		}

		private abstract class RuntimeTablixCell : IScope
		{
			protected RuntimeTablixGroupLeafObj m_owner;

			protected int m_cellLevel;

			protected DataAggregateObjList m_cellNonCustomAggObjs;

			protected DataAggregateObjList m_cellCustomAggObjs;

			protected DataAggregateObjResult[] m_cellAggValueList;

			protected DataRowList m_dataRows;

			protected bool m_innermost;

			protected FieldImpl[] m_firstRow;

			protected bool m_firstRowIsAggregate;

			protected RuntimeTablixCell m_nextCell;

			internal RuntimeTablixCell NextCell
			{
				get
				{
					return m_nextCell;
				}
				set
				{
					m_nextCell = value;
				}
			}

			bool IScope.TargetForNonDetailSort => m_owner.GetCellTargetForNonDetailSort();

			int[] IScope.SortFilterExpressionScopeInfoIndices
			{
				get
				{
					Global.Tracer.Assert(condition: false, string.Empty);
					return null;
				}
			}

			internal RuntimeTablixCell(RuntimeTablixGroupLeafObj owner, int cellLevel, DataAggregateInfoList aggDefs, bool innermost)
			{
				m_owner = owner;
				m_cellLevel = cellLevel;
				RuntimeDataRegionObj.CreateAggregates(owner.ProcessingContext, aggDefs, ref m_cellNonCustomAggObjs, ref m_cellCustomAggObjs);
				DataAggregateObjList cellPostSortAggregates = m_owner.CellPostSortAggregates;
				if (cellPostSortAggregates != null && 0 < cellPostSortAggregates.Count)
				{
					m_cellAggValueList = new DataAggregateObjResult[cellPostSortAggregates.Count];
				}
				m_innermost = innermost;
			}

			internal virtual void NextRow()
			{
				RuntimeDataRegionObj.CommonFirstRow(m_owner.ProcessingContext.ReportObjectModel.FieldsImpl, ref m_firstRowIsAggregate, ref m_firstRow);
				NextAggregateRow();
				if (!m_owner.ProcessingContext.ReportObjectModel.FieldsImpl.IsAggregateRow)
				{
					NextNonAggregateRow();
				}
			}

			private void NextNonAggregateRow()
			{
				RuntimeDataRegionObj.UpdateAggregates(m_owner.ProcessingContext, m_cellNonCustomAggObjs, updateAndSetup: false);
				if (m_dataRows != null)
				{
					RuntimeDetailObj.SaveData(m_dataRows, m_owner.ProcessingContext);
				}
			}

			private void NextAggregateRow()
			{
				FieldsImpl fieldsImpl = m_owner.ProcessingContext.ReportObjectModel.FieldsImpl;
				if (fieldsImpl.ValidAggregateRow && fieldsImpl.AggregationFieldCount == 0 && m_cellCustomAggObjs != null)
				{
					RuntimeDataRegionObj.UpdateAggregates(m_owner.ProcessingContext, m_cellCustomAggObjs, updateAndSetup: false);
				}
			}

			internal virtual void SortAndFilter()
			{
			}

			internal virtual void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
				if (m_dataRows != null)
				{
					Global.Tracer.Assert(m_innermost);
					ReadRows();
					m_dataRows = null;
				}
				DataAggregateObjList cellPostSortAggregates = m_owner.CellPostSortAggregates;
				if (cellPostSortAggregates != null && 0 < cellPostSortAggregates.Count)
				{
					for (int i = 0; i < cellPostSortAggregates.Count; i++)
					{
						m_cellAggValueList[i] = cellPostSortAggregates[i].AggregateResult();
						cellPostSortAggregates[i].Init();
					}
				}
			}

			private void ReadRows()
			{
				for (int i = 0; i < m_dataRows.Count; i++)
				{
					FieldImpl[] fields = m_dataRows[i];
					m_owner.ProcessingContext.ReportObjectModel.FieldsImpl.SetFields(fields);
					((IScope)this).ReadRow(DataActions.PostSortAggregates);
				}
			}

			protected void SetupAggregates(DataAggregateObjList aggregates, DataAggregateObjResult[] aggValues)
			{
				if (aggregates != null)
				{
					for (int i = 0; i < aggregates.Count; i++)
					{
						DataAggregateObj dataAggregateObj = aggregates[i];
						m_owner.ProcessingContext.ReportObjectModel.AggregatesImpl.Set(dataAggregateObj.Name, dataAggregateObj.AggregateDef, dataAggregateObj.DuplicateNames, (aggValues == null) ? dataAggregateObj.AggregateResult() : aggValues[i]);
					}
				}
			}

			protected void SetupEnvironment()
			{
				SetupAggregates(m_cellNonCustomAggObjs, null);
				SetupAggregates(m_cellCustomAggObjs, null);
				SetupAggregates(m_owner.CellPostSortAggregates, m_cellAggValueList);
				m_owner.ProcessingContext.ReportObjectModel.FieldsImpl.SetFields(m_firstRow);
				m_owner.ProcessingContext.ReportRuntime.CurrentScope = this;
			}

			bool IScope.IsTargetForSort(int index, bool detailSort)
			{
				return m_owner.GetCellTargetForSort(index, detailSort);
			}

			string IScope.GetScopeName()
			{
				return null;
			}

			IScope IScope.GetOuterScope(bool includeSubReportContainingScope)
			{
				return m_owner;
			}

			void IScope.ReadRow(DataActions dataAction)
			{
				m_owner.ReadRow(dataAction);
			}

			bool IScope.InScope(string scope)
			{
				if (m_owner.InScope(scope))
				{
					return true;
				}
				return GetOuterScopeNames().Contains(scope);
			}

			int IScope.RecursiveLevel(string scope)
			{
				if (scope == null)
				{
					return 0;
				}
				int num = ((IScope)m_owner).RecursiveLevel(scope);
				if (-1 != num)
				{
					return num;
				}
				return (GetOuterScopeNames()[scope] as Grouping)?.RecursiveLevel ?? (-1);
			}

			private Hashtable GetOuterScopeNames()
			{
				Global.Tracer.Assert(m_owner.TablixHeadingDef != null);
				TablixHeading tablixHeadingDef = m_owner.TablixHeadingDef;
				Tablix tablix = (Tablix)tablixHeadingDef.DataRegionDef;
				Hashtable hashtable = null;
				if (tablixHeadingDef.CellScopeNames == null)
				{
					tablixHeadingDef.CellScopeNames = new Hashtable[tablix.GetDynamicHeadingCount(outerGroupings: true)];
				}
				else
				{
					hashtable = tablixHeadingDef.CellScopeNames[m_cellLevel];
				}
				if (hashtable == null)
				{
					hashtable = tablix.GetOuterScopeNames(m_cellLevel);
					tablixHeadingDef.CellScopeNames[m_cellLevel] = hashtable;
				}
				return hashtable;
			}

			bool IScope.TargetScopeMatched(int index, bool detailSort)
			{
				if (m_owner.TargetScopeMatched(index, detailSort))
				{
					IDictionaryEnumerator enumerator = GetOuterScopeNames().GetEnumerator();
					while (enumerator.MoveNext())
					{
						Grouping grouping = (Grouping)enumerator.Value;
						if ((!detailSort || grouping.SortFilterScopeInfo != null) && (grouping.SortFilterScopeMatched == null || !grouping.SortFilterScopeMatched[index]))
						{
							return false;
						}
					}
					return true;
				}
				return false;
			}

			void IScope.GetScopeValues(IHierarchyObj targetScopeObj, VariantList[] scopeValues, ref int index)
			{
				m_owner.GetScopeValues(targetScopeObj, scopeValues, ref index);
				Global.Tracer.Assert(m_innermost);
				Hashtable outerScopeNames = GetOuterScopeNames();
				IDictionaryEnumerator enumerator = outerScopeNames.GetEnumerator();
				while (enumerator.MoveNext())
				{
					Grouping grouping = (Grouping)enumerator.Value;
					Global.Tracer.Assert(index < scopeValues.Length);
					scopeValues[index++] = grouping.CurrentGroupExpressionValues;
				}
				Global.Tracer.Assert(m_owner.TablixDef.GetDynamicHeadingCount(outerGroupings: true) == outerScopeNames.Count);
			}

			void IScope.GetGroupNameValuePairs(Dictionary<string, object> pairs)
			{
				((IScope)m_owner).GetGroupNameValuePairs(pairs);
				Hashtable outerScopeNames = GetOuterScopeNames();
				if (outerScopeNames != null)
				{
					IEnumerator enumerator = outerScopeNames.Values.GetEnumerator();
					while (enumerator.MoveNext())
					{
						RuntimeDataRegionObj.AddGroupNameValuePair(m_owner.ProcessingContext, enumerator.Current as Grouping, pairs);
					}
				}
			}
		}

		private sealed class RuntimeCustomReportItemCell : RuntimeTablixCell
		{
			private DataAggregateObjResult[] m_runningValueValues;

			internal RuntimeCustomReportItemCell(RuntimeCustomReportItemGroupLeafObj owner, int cellLevel, DataAggregateInfoList aggDefs, DataCellsList dataRowCells, bool innermost)
				: base(owner, cellLevel, aggDefs, innermost)
			{
				CustomReportItem customReportItem = (CustomReportItem)owner.TablixDef;
				DataActions dataActions = DataActions.None;
				bool flag = customReportItem.CellRunningValues != null && 0 < customReportItem.CellRunningValues.Count;
				if (m_innermost && (flag || m_owner.CellPostSortAggregates != null))
				{
					dataActions = DataActions.PostSortAggregates;
				}
				if (dataActions != 0)
				{
					m_dataRows = new DataRowList();
				}
			}

			internal override void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
				base.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
				RuntimeRICollection.DoneReadingRows(globalRVCol, m_owner.TablixDef.TablixCellRunningValues, ref m_runningValueValues, processPreviousAggregates: false);
			}

			internal void CreateInstance(CustomReportItemInstance criInstance)
			{
				SetupEnvironment();
				RuntimeDataRegionObj.SetupRunningValues(m_owner.ProcessingContext, m_owner.TablixDef.TablixCellRunningValues, m_runningValueValues);
				criInstance.AddCell(m_owner.ProcessingContext);
			}
		}

		private sealed class RuntimeCustomReportItemObj : RuntimeTablixObj
		{
			private bool m_subtotalCorner;

			internal RuntimeCustomReportItemObj(IScope outerScope, CustomReportItem crItem, ref DataActions dataAction, ProcessingContext processingContext, bool onePassProcess)
				: base(outerScope, crItem, ref dataAction, processingContext, onePassProcess)
			{
				ConstructorHelper(ref dataAction, onePassProcess, out bool handleMyDataAction, out DataActions innerDataAction, out TablixHeadingList outermostColumns, out TablixHeadingList outermostRows, out TablixHeadingList staticColumns, out TablixHeadingList staticRows);
				m_innerDataAction = innerDataAction;
				CRIConstructRuntimeStructure(ref innerDataAction, onePassProcess, outermostColumns, outermostRows, staticColumns, staticRows);
				if (onePassProcess || (outermostRows == null && outermostColumns == null))
				{
					m_subtotalCorner = true;
				}
				HandleDataAction(handleMyDataAction, innerDataAction);
			}

			protected override void ConstructRuntimeStructure(ref DataActions innerDataAction)
			{
				m_tablixDef.GetHeadingDefState(out TablixHeadingList outermostColumns, out TablixHeadingList outermostRows, out TablixHeadingList staticColumns, out TablixHeadingList staticRows);
				CRIConstructRuntimeStructure(ref innerDataAction, onePassProcess: false, outermostColumns, outermostRows, staticColumns, staticRows);
			}

			private void CRIConstructRuntimeStructure(ref DataActions innerDataAction, bool onePassProcess, TablixHeadingList outermostColumns, TablixHeadingList outermostRows, TablixHeadingList staticColumns, TablixHeadingList staticRows)
			{
				DataActions dataAction = DataActions.None;
				if (m_tablixDef.ProcessingInnerGrouping == Pivot.ProcessingInnerGroupings.Column)
				{
					m_innerGroupings = (m_tablixColumns = new RuntimeCustomReportItemHeadingsObj(this, (CustomReportItemHeadingList)outermostColumns, ref dataAction, m_processingContext, (CustomReportItemHeadingList)staticColumns, null, outermostRows == null, 0));
					m_outerGroupings = (m_tablixRows = new RuntimeCustomReportItemHeadingsObj(this, (CustomReportItemHeadingList)outermostRows, ref innerDataAction, m_processingContext, (CustomReportItemHeadingList)staticRows, (RuntimeCustomReportItemHeadingsObj)m_innerGroupings, outermostColumns == null, 0));
				}
				else
				{
					m_innerGroupings = (m_tablixRows = new RuntimeCustomReportItemHeadingsObj(this, (CustomReportItemHeadingList)outermostRows, ref dataAction, m_processingContext, (CustomReportItemHeadingList)staticRows, null, outermostColumns == null, 0));
					m_outerGroupings = (m_tablixColumns = new RuntimeCustomReportItemHeadingsObj(this, (CustomReportItemHeadingList)outermostColumns, ref innerDataAction, m_processingContext, (CustomReportItemHeadingList)staticColumns, (RuntimeCustomReportItemHeadingsObj)m_innerGroupings, outermostRows == null, 0));
				}
			}

			internal override void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
				base.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
				RuntimeRICollection.DoneReadingRows(globalRVCol, m_tablixDef.RunningValues, ref m_runningValueValues, processPreviousAggregates: false);
			}

			internal override void CreateInstances(ReportItemInstance riInstance, IList instanceList, RenderingPagesRangesList pagesList)
			{
				if (m_firstRow != null)
				{
					_ = (CustomReportItem)m_tablixDef;
					CustomReportItemInstance customReportItemInstance = (CustomReportItemInstance)riInstance;
					if (m_outerGroupings == m_tablixRows)
					{
						customReportItemInstance.InnerHeadingInstanceList = customReportItemInstance.ColumnInstances;
						((RuntimeCustomReportItemHeadingsObj)m_outerGroupings).CreateInstances(this, m_processingContext, customReportItemInstance, outerGroupings: true, null, customReportItemInstance.RowInstances);
					}
					else
					{
						customReportItemInstance.InnerHeadingInstanceList = customReportItemInstance.RowInstances;
						((RuntimeCustomReportItemHeadingsObj)m_outerGroupings).CreateInstances(this, m_processingContext, customReportItemInstance, outerGroupings: true, null, customReportItemInstance.ColumnInstances);
					}
				}
			}

			internal void CreateOutermostSubtotalCells(CustomReportItemInstance criInstance, bool outerGroupings)
			{
				if (outerGroupings)
				{
					SetupEnvironment();
					((RuntimeCustomReportItemHeadingsObj)m_innerGroupings).CreateInstances(this, m_processingContext, criInstance, outerGroupings: false, null, criInstance.InnerHeadingInstanceList);
				}
				else if (m_subtotalCorner)
				{
					SetupEnvironment();
					criInstance.AddCell(m_processingContext);
				}
			}
		}

		private sealed class RuntimeCustomReportItemHeadingsObj : RuntimeTablixHeadingsObj
		{
			private DataAggregateObjResult[] m_runningValueValues;

			internal RuntimeCustomReportItemHeadingsObj(IScope owner, CustomReportItemHeadingList headingDef, ref DataActions dataAction, ProcessingContext processingContext, CustomReportItemHeadingList staticHeadingDef, RuntimeCustomReportItemHeadingsObj innerGroupings, bool outermostSubtotal, int headingLevel)
				: base(owner, headingDef, ref dataAction, processingContext, staticHeadingDef, innerGroupings, headingLevel)
			{
				if (headingDef != null)
				{
					m_tablixHeadings = new RuntimeCustomReportItemGroupRootObj(owner, headingDef, 0, ref dataAction, processingContext, innerGroupings, outermostSubtotal, headingLevel);
				}
			}

			internal override void CalculatePreviousAggregates(AggregatesImpl globalRVCol)
			{
				if (m_staticHeadingDef != null)
				{
					for (int i = 0; i < m_staticHeadingDef.Count; i++)
					{
						RuntimeRICollection.DoneReadingRows(globalRVCol, ((CustomReportItemHeading)m_staticHeadingDef[i]).RunningValues, ref m_runningValueValues, processPreviousAggregates: true);
					}
				}
			}

			internal override void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
				base.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
				if (m_staticHeadingDef != null && m_owner is RuntimeCustomReportItemGroupLeafObj)
				{
					for (int i = 0; i < m_staticHeadingDef.Count; i++)
					{
						RuntimeRICollection.DoneReadingRows(globalRVCol, ((CustomReportItemHeading)m_staticHeadingDef[i]).RunningValues, ref m_runningValueValues, processPreviousAggregates: false);
					}
				}
			}

			private void SetupEnvironment(ProcessingContext processingContext)
			{
				if (m_staticHeadingDef != null && m_runningValueValues != null)
				{
					for (int i = 0; i < m_staticHeadingDef.Count; i++)
					{
						RuntimeDataRegionObj.SetupRunningValues(processingContext, ((CustomReportItemHeading)m_staticHeadingDef[i]).RunningValues, m_runningValueValues);
					}
				}
			}

			internal void CreateInstances(RuntimeDataRegionObj outerGroup, ProcessingContext processingContext, CustomReportItemInstance criInstance, bool outerGroupings, RuntimeTablixGroupRootObj currOuterHeadingGroupRoot, CustomReportItemHeadingInstanceList headingInstances)
			{
				bool flag = outerGroupings || criInstance.CurrentCellOuterIndex == 0;
				CustomReportItemHeadingList customReportItemHeadingList = m_staticHeadingDef as CustomReportItemHeadingList;
				SetupEnvironment(processingContext);
				int num = customReportItemHeadingList?.Count ?? 1;
				CustomReportItemHeadingInstanceList customReportItemHeadingInstanceList = headingInstances;
				for (int i = 0; i < num; i++)
				{
					if (customReportItemHeadingList != null)
					{
						if (flag)
						{
							customReportItemHeadingInstanceList = CreateHeadingInstance(processingContext, criInstance, customReportItemHeadingList, headingInstances, outerGroupings, i);
						}
						if (outerGroupings)
						{
							criInstance.CurrentOuterStaticIndex = i;
						}
						else
						{
							criInstance.CurrentInnerStaticIndex = i;
						}
					}
					if (m_tablixHeadings != null)
					{
						((CustomReportItem)m_tablixHeadings.HierarchyDef.DataRegionDef).CurrentOuterHeadingGroupRoot = currOuterHeadingGroupRoot;
						m_tablixHeadings.CreateInstances(criInstance, customReportItemHeadingInstanceList, null);
						if (flag)
						{
							SetHeadingSpan(criInstance, customReportItemHeadingInstanceList, outerGroupings, processingContext);
						}
					}
					else if (outerGroup is RuntimeCustomReportItemGroupLeafObj)
					{
						RuntimeCustomReportItemGroupLeafObj runtimeCustomReportItemGroupLeafObj = (RuntimeCustomReportItemGroupLeafObj)outerGroup;
						if (!outerGroupings && runtimeCustomReportItemGroupLeafObj.IsOuterGrouping())
						{
							runtimeCustomReportItemGroupLeafObj.CreateSubtotalOrStaticCells(criInstance, currOuterHeadingGroupRoot, outerGroupings);
						}
						else
						{
							runtimeCustomReportItemGroupLeafObj.CreateInnerGroupingsOrCells(criInstance, currOuterHeadingGroupRoot);
						}
					}
					else
					{
						((RuntimeCustomReportItemObj)outerGroup).CreateOutermostSubtotalCells(criInstance, outerGroupings);
					}
				}
				if (customReportItemHeadingList != null && flag)
				{
					SetHeadingSpan(criInstance, headingInstances, outerGroupings, processingContext);
				}
			}

			private void SetHeadingSpan(CustomReportItemInstance criInstance, CustomReportItemHeadingInstanceList headingInstances, bool outerGroupings, ProcessingContext processingContext)
			{
				int currentCellIndex = (!outerGroupings) ? criInstance.CurrentCellInnerIndex : (criInstance.CurrentCellOuterIndex + 1);
				headingInstances.SetLastHeadingSpan(currentCellIndex, processingContext);
			}

			private CustomReportItemHeadingInstanceList CreateHeadingInstance(ProcessingContext processingContext, CustomReportItemInstance criInstance, CustomReportItemHeadingList headingDef, CustomReportItemHeadingInstanceList headingInstances, bool outerGroupings, int headingIndex)
			{
				CustomReportItemHeadingInstance customReportItemHeadingInstance = null;
				int headingCellIndex;
				if (outerGroupings)
				{
					criInstance.NewOuterCells();
					headingCellIndex = criInstance.CurrentCellOuterIndex;
				}
				else
				{
					headingCellIndex = criInstance.CurrentCellInnerIndex;
				}
				customReportItemHeadingInstance = new CustomReportItemHeadingInstance(processingContext, headingCellIndex, headingDef[headingIndex], null, 0);
				headingInstances.Add(customReportItemHeadingInstance, processingContext);
				return customReportItemHeadingInstance.SubHeadingInstances;
			}
		}

		private sealed class RuntimeCustomReportItemGroupRootObj : RuntimeTablixGroupRootObj
		{
			internal RuntimeCustomReportItemGroupRootObj(IScope outerScope, CustomReportItemHeadingList headingDef, int headingIndex, ref DataActions dataAction, ProcessingContext processingContext, RuntimeCustomReportItemHeadingsObj innerGroupings, bool outermostSubtotal, int headingLevel)
				: base(outerScope, headingDef, headingIndex, ref dataAction, processingContext, innerGroupings, outermostSubtotal, headingLevel)
			{
				Global.Tracer.Assert(headingIndex == 0);
				if (m_processOutermostSTCells)
				{
					CustomReportItem customReportItem = (CustomReportItem)headingDef[headingIndex].DataRegionDef;
					if (customReportItem.CellRunningValues != null && 0 < customReportItem.CellRunningValues.Count)
					{
						m_dataAction |= DataActions.PostSortAggregates;
					}
				}
				m_saveGroupExprValues = true;
			}

			protected override void NeedProcessDataActions(TablixHeadingList heading)
			{
				CustomReportItemHeadingList customReportItemHeadingList = (CustomReportItemHeadingList)heading;
				if (customReportItemHeadingList != null)
				{
					NeedProcessDataActions(customReportItemHeadingList[m_headingIndex].RunningValues);
				}
			}

			internal override void CalculateRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, RuntimeGroupRootObj lastGroup)
			{
				base.CalculateRunningValues(globalRVCol, groupCol, lastGroup);
				AddRunningValues(((CustomReportItemHeading)m_hierarchyDef).RunningValues);
				if (m_staticHeadingDef != null)
				{
					for (int i = 0; i < m_staticHeadingDef.Count; i++)
					{
						AddRunningValues(((CustomReportItemHeading)m_staticHeadingDef[i]).RunningValues);
					}
				}
				m_grouping.Traverse(ProcessingStages.RunningValues, m_expression.Direction);
				if (m_hierarchyDef.Grouping.Name != null)
				{
					groupCol.Remove(m_hierarchyDef.Grouping.Name);
				}
			}

			protected override void AddCellRunningValues(AggregatesImpl globalRVCol, RuntimeGroupRootObjList groupCol, ref DataAggregateObjList runningValues)
			{
				CustomReportItem customReportItem = (CustomReportItem)m_hierarchyDef.DataRegionDef;
				if (customReportItem.CellRunningValues != null && 0 < customReportItem.CellRunningValues.Count)
				{
					m_dataAction |= DataActions.PostSortAggregates;
					if (runningValues == null)
					{
						AddRunningValues(customReportItem.CellRunningValues, ref runningValues, globalRVCol, groupCol);
					}
				}
			}
		}

		private sealed class RuntimeCustomReportItemGroupLeafObj : RuntimeTablixGroupLeafObj
		{
			private DataAggregateObjResult[] m_runningValueValues;

			private DataAggregateObjResult[] m_cellRunningValueValues;

			internal RuntimeCustomReportItemGroupLeafObj(RuntimeCustomReportItemGroupRootObj groupRoot)
				: base(groupRoot)
			{
				CustomReportItem tablixDef = (CustomReportItem)((CustomReportItemHeading)groupRoot.HierarchyDef).DataRegionDef;
				CustomReportItemHeadingList headingDef = (CustomReportItemHeadingList)groupRoot.InnerHeading;
				bool handleMyDataAction = false;
				bool num = HandleSortFilterEvent();
				ConstructorHelper(groupRoot, tablixDef, out handleMyDataAction, out DataActions innerDataAction);
				m_tablixHeadings = new RuntimeCustomReportItemHeadingsObj(this, headingDef, ref innerDataAction, groupRoot.ProcessingContext, (CustomReportItemHeadingList)groupRoot.StaticHeadingDef, (RuntimeCustomReportItemHeadingsObj)groupRoot.InnerGroupings, groupRoot.OutermostSubtotal, groupRoot.HeadingLevel + 1);
				m_innerHeadingList = m_tablixHeadings.Headings;
				if (!handleMyDataAction)
				{
					m_dataAction = innerDataAction;
				}
				if (num)
				{
					m_dataAction |= DataActions.UserSort;
				}
				if (m_dataAction != 0)
				{
					m_dataRows = new DataRowList();
				}
			}

			internal override RuntimeTablixCell CreateCell(int index, Tablix tablixDef)
			{
				return new RuntimeCustomReportItemCell(this, index, tablixDef.CellAggregates, ((CustomReportItem)tablixDef).DataRowCells, m_innerHeadingList == null);
			}

			internal override void CalculateRunningValues()
			{
				base.CalculateRunningValues();
				if (m_processHeading)
				{
					RuntimeTablixGroupRootObj runtimeTablixGroupRootObj = (RuntimeTablixGroupRootObj)m_hierarchyRoot;
					AggregatesImpl globalRunningValueCollection = runtimeTablixGroupRootObj.GlobalRunningValueCollection;
					_ = runtimeTablixGroupRootObj.GroupCollection;
					RuntimeRICollection.DoneReadingRows(globalRunningValueCollection, ((CustomReportItemHeading)runtimeTablixGroupRootObj.HierarchyDef).RunningValues, ref m_runningValueValues, processPreviousAggregates: false);
					if (runtimeTablixGroupRootObj.ProcessOutermostSTCells)
					{
						RuntimeRICollection.DoneReadingRows(runtimeTablixGroupRootObj.OutermostSTCellRVCol, ((CustomReportItem)base.TablixDef).CellRunningValues, ref m_cellRunningValueValues, processPreviousAggregates: false);
					}
					m_processHeading = false;
				}
				ResetScopedRunningValues();
			}

			internal override void CreateInstance()
			{
				SetupEnvironment();
				RuntimeCustomReportItemGroupRootObj runtimeCustomReportItemGroupRootObj = (RuntimeCustomReportItemGroupRootObj)m_hierarchyRoot;
				CustomReportItem customReportItem = (CustomReportItem)base.TablixDef;
				CustomReportItemInstance customReportItemInstance = (CustomReportItemInstance)runtimeCustomReportItemGroupRootObj.ReportItemInstance;
				CustomReportItemHeadingInstanceList customReportItemHeadingInstanceList = (CustomReportItemHeadingInstanceList)runtimeCustomReportItemGroupRootObj.InstanceList;
				CustomReportItemHeading customReportItemHeading = (CustomReportItemHeading)runtimeCustomReportItemGroupRootObj.HierarchyDef;
				bool flag = IsOuterGrouping();
				SetupRunningValues(customReportItemHeading.RunningValues, m_runningValueValues);
				if (m_cellRunningValueValues != null)
				{
					SetupRunningValues(customReportItem.CellRunningValues, m_cellRunningValueValues);
				}
				RuntimeTablixGroupRootObj currOuterHeadingGroupRoot;
				int headingCellIndex;
				if (flag)
				{
					currOuterHeadingGroupRoot = (customReportItem.CurrentOuterHeadingGroupRoot = runtimeCustomReportItemGroupRootObj);
					customReportItem.OuterGroupingIndexes[runtimeCustomReportItemGroupRootObj.HeadingLevel] = m_groupLeafIndex;
					customReportItemInstance.NewOuterCells();
					headingCellIndex = customReportItemInstance.CurrentCellOuterIndex;
				}
				else
				{
					currOuterHeadingGroupRoot = customReportItem.CurrentOuterHeadingGroupRoot;
					headingCellIndex = customReportItemInstance.CurrentCellInnerIndex;
				}
				if (flag || customReportItemInstance.CurrentCellOuterIndex == 0)
				{
					CustomReportItemHeadingInstance customReportItemHeadingInstance = new CustomReportItemHeadingInstance(m_processingContext, headingCellIndex, customReportItemHeading, m_groupExprValues, m_recursiveLevel);
					customReportItemHeadingInstanceList.Add(customReportItemHeadingInstance, m_processingContext);
					customReportItemHeadingInstanceList = customReportItemHeadingInstance.SubHeadingInstances;
				}
				((RuntimeCustomReportItemHeadingsObj)m_tablixHeadings).CreateInstances(this, m_processingContext, customReportItemInstance, flag, currOuterHeadingGroupRoot, customReportItemHeadingInstanceList);
			}

			internal void CreateInnerGroupingsOrCells(CustomReportItemInstance criInstance, RuntimeTablixGroupRootObj currOuterHeadingGroupRoot)
			{
				SetupEnvironment();
				if (IsOuterGrouping())
				{
					((RuntimeCustomReportItemHeadingsObj)((RuntimeCustomReportItemGroupRootObj)m_hierarchyRoot).InnerGroupings).CreateInstances(this, m_processingContext, criInstance, outerGroupings: false, currOuterHeadingGroupRoot, criInstance.InnerHeadingInstanceList);
				}
				else if (currOuterHeadingGroupRoot == null)
				{
					CreateOutermostSubtotalCell(criInstance);
				}
				else
				{
					CreateCellInstance(criInstance, currOuterHeadingGroupRoot);
				}
			}

			private void CreateCellInstance(CustomReportItemInstance criInstance, RuntimeTablixGroupRootObj currOuterHeadingGroupRoot)
			{
				Global.Tracer.Assert(m_cellsList != null && m_cellsList[currOuterHeadingGroupRoot.HeadingLevel] != null);
				RuntimeCustomReportItemCell runtimeCustomReportItemCell = (RuntimeCustomReportItemCell)m_cellsList[currOuterHeadingGroupRoot.HeadingLevel].GetCell(base.TablixDef, this, currOuterHeadingGroupRoot.HeadingLevel);
				Global.Tracer.Assert(runtimeCustomReportItemCell != null);
				runtimeCustomReportItemCell.CreateInstance(criInstance);
			}

			private void CreateOutermostSubtotalCell(CustomReportItemInstance criInstance)
			{
				SetupEnvironment();
				criInstance.AddCell(m_processingContext);
			}

			internal void CreateSubtotalOrStaticCells(CustomReportItemInstance criInstance, RuntimeTablixGroupRootObj currOuterHeadingGroupRoot, bool outerGroupingSubtotal)
			{
				_ = (RuntimeCustomReportItemHeadingsObj)((RuntimeCustomReportItemGroupRootObj)m_hierarchyRoot).InnerGroupings;
				if (IsOuterGrouping() && !outerGroupingSubtotal)
				{
					CreateOutermostSubtotalCell(criInstance);
				}
				else
				{
					CreateInnerGroupingsOrCells(criInstance, currOuterHeadingGroupRoot);
				}
			}
		}

		private IConfiguration m_configuration;

		internal const int MaximumChartThreads = 5;

		internal IConfiguration Configuration
		{
			get
			{
				return m_configuration;
			}
			set
			{
				m_configuration = value;
			}
		}

		public bool ProcessToggleEvent(string showHideToggle, IChunkFactory getReportChunkFactory, EventInformation oldShowHideInfo, out EventInformation newShowHideInfo, out bool showHideInfoChanged)
		{
			newShowHideInfo = null;
			showHideInfoChanged = false;
			if (getReportChunkFactory == null)
			{
				return false;
			}
			if (showHideToggle == null)
			{
				return false;
			}
			if (ContainsFlag(getReportChunkFactory.ReportProcessingFlags, ReportProcessingFlags.OnDemandEngine))
			{
				return ProcessOdpToggleEvent(showHideToggle, getReportChunkFactory, oldShowHideInfo, out newShowHideInfo, out showHideInfoChanged);
			}
			ChunkFactoryAdapter @object = new ChunkFactoryAdapter(getReportChunkFactory);
			return ProcessYukonToggleEvent(showHideToggle, @object.GetReportChunk, oldShowHideInfo, out newShowHideInfo, out showHideInfoChanged);
		}

		internal static bool ProcessOdpToggleEvent(string showHideToggle, IChunkFactory getReportChunkFactory, EventInformation oldShowHideInfo, out EventInformation newShowHideInfo, out bool showHideInfoChanged)
		{
			newShowHideInfo = null;
			showHideInfoChanged = false;
			if (showHideToggle == null || oldShowHideInfo == null || !oldShowHideInfo.ValidToggleSender(showHideToggle))
			{
				return false;
			}
			newShowHideInfo = new EventInformation(oldShowHideInfo);
			showHideInfoChanged = true;
			if (newShowHideInfo.ToggleStateInfo == null)
			{
				newShowHideInfo.ToggleStateInfo = new Hashtable();
			}
			Hashtable toggleStateInfo = newShowHideInfo.ToggleStateInfo;
			if (toggleStateInfo.ContainsKey(showHideToggle))
			{
				toggleStateInfo.Remove(showHideToggle);
			}
			else
			{
				toggleStateInfo.Add(showHideToggle, null);
			}
			return true;
		}

		private bool ProcessYukonToggleEvent(string showHideToggle, GetReportChunk getReportChunk, EventInformation oldShowHideInfo, out EventInformation newShowHideInfo, out bool showHideInfoChanged)
		{
			newShowHideInfo = null;
			showHideInfoChanged = false;
			ChunkManager.EventsChunkManager eventsChunkManager = null;
			try
			{
				eventsChunkManager = new ChunkManager.EventsChunkManager(getReportChunk);
				return new ShowHideProcessing().Process(showHideToggle, oldShowHideInfo, eventsChunkManager, out showHideInfoChanged, out newShowHideInfo);
			}
			finally
			{
				eventsChunkManager?.Close();
			}
		}

		public int ProcessFindStringEvent(int startPage, int endPage, string findValue, EventInformation eventInfo, Microsoft.ReportingServices.ReportProcessing.ProcessingContext processingContext, out OnDemandProcessingResult result)
		{
			result = null;
			if (findValue == null || processingContext == null || startPage <= 0 || endPage <= 0)
			{
				return 0;
			}
			if (ContainsFlag(processingContext.ReportProcessingFlags, ReportProcessingFlags.OnDemandEngine))
			{
				return ProcessOdpFindStringEvent(startPage, endPage, findValue, eventInfo, processingContext, out result);
			}
			return ProcessYukonFindStringEvent(startPage, endPage, findValue, processingContext, eventInfo);
		}

		private int ProcessOdpFindStringEvent(int startPage, int endPage, string findValue, EventInformation eventInfo, Microsoft.ReportingServices.ReportProcessing.ProcessingContext processingContext, out OnDemandProcessingResult result)
		{
			ExecutionLogContext executionLogContext = new ExecutionLogContext(processingContext.JobContext);
			ProcessingErrorContext errorContext = null;
			OnDemandProcessingContext odpContext = null;
			Microsoft.ReportingServices.OnDemandReportRendering.RenderingContext odpRenderingContext = null;
			Microsoft.ReportingServices.ReportIntermediateFormat.ReportSnapshot odpReportSnapshot = null;
			Microsoft.ReportingServices.OnDemandReportRendering.Report report = null;
			bool exceptionGenerated = false;
			CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
			try
			{
				report = GenerateEventROM(processingContext, null, eventInfo, executionLogContext, out errorContext, out odpContext, out odpRenderingContext, out odpReportSnapshot);
				return InteractivityPaginationModuleFactory.CreateInteractivityPaginationModule().ProcessFindStringEvent(report, -1, startPage, endPage, findValue);
			}
			catch (Exception exception)
			{
				exceptionGenerated = true;
				if (NeedWrapperException(exception, errorContext, out Exception wrappedException))
				{
					throw wrappedException;
				}
				throw;
			}
			finally
			{
				result = CleanupEventROM(processingContext, executionLogContext, errorContext, odpContext, odpRenderingContext, odpReportSnapshot, exceptionGenerated);
				if (currentCulture != null)
				{
					Thread.CurrentThread.CurrentCulture = currentCulture;
				}
			}
		}

		private int ProcessYukonFindStringEvent(int startPage, int endPage, string findValue, Microsoft.ReportingServices.ReportProcessing.ProcessingContext pc, EventInformation eventInfo)
		{
			Microsoft.ReportingServices.OnDemandReportRendering.Report reportToRender = null;
			try
			{
				GenerateEventShimROM(pc.ChunkFactory, eventInfo, pc, out reportToRender);
				return InteractivityPaginationModuleFactory.CreateInteractivityPaginationModule().ProcessFindStringEvent(reportToRender, -1, startPage, endPage, findValue);
			}
			finally
			{
				reportToRender?.RenderingContext.CloseRenderingChunkManager();
			}
		}

		public int ProcessBookmarkNavigationEvent(string bookmarkId, EventInformation eventInfo, Microsoft.ReportingServices.ReportProcessing.ProcessingContext processingContext, out string uniqueName, out OnDemandProcessingResult result)
		{
			uniqueName = null;
			result = null;
			if (processingContext == null || bookmarkId == null)
			{
				return 0;
			}
			if (ContainsFlag(processingContext.ReportProcessingFlags, ReportProcessingFlags.OnDemandEngine))
			{
				return ProcessOdpBookmarkNavigationEvent(bookmarkId, eventInfo, processingContext, out uniqueName, out result);
			}
			result = null;
			return ProcessYukonBookmarkNavigationEvent(bookmarkId, processingContext, out uniqueName);
		}

		private int ProcessOdpBookmarkNavigationEvent(string bookmarkId, EventInformation eventInfo, Microsoft.ReportingServices.ReportProcessing.ProcessingContext processingContext, out string uniqueName, out OnDemandProcessingResult result)
		{
			uniqueName = null;
			result = null;
			ExecutionLogContext executionLogContext = new ExecutionLogContext(processingContext.JobContext);
			ProcessingErrorContext errorContext = null;
			OnDemandProcessingContext odpContext = null;
			Microsoft.ReportingServices.OnDemandReportRendering.RenderingContext odpRenderingContext = null;
			Microsoft.ReportingServices.ReportIntermediateFormat.ReportSnapshot odpReportSnapshot = null;
			Microsoft.ReportingServices.OnDemandReportRendering.Report report = null;
			bool exceptionGenerated = false;
			CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
			try
			{
				report = GenerateEventROM(processingContext, null, eventInfo, executionLogContext, out errorContext, out odpContext, out odpRenderingContext, out odpReportSnapshot);
				return InteractivityPaginationModuleFactory.CreateInteractivityPaginationModule().ProcessBookmarkNavigationEvent(report, -1, bookmarkId, out uniqueName);
			}
			catch (Exception exception)
			{
				exceptionGenerated = true;
				if (NeedWrapperException(exception, errorContext, out Exception wrappedException))
				{
					throw wrappedException;
				}
				throw;
			}
			finally
			{
				result = CleanupEventROM(processingContext, executionLogContext, errorContext, odpContext, odpRenderingContext, odpReportSnapshot, exceptionGenerated);
				if (currentCulture != null)
				{
					Thread.CurrentThread.CurrentCulture = currentCulture;
				}
			}
		}

		private int ProcessYukonBookmarkNavigationEvent(string bookmarkId, Microsoft.ReportingServices.ReportProcessing.ProcessingContext pc, out string uniqueName)
		{
			uniqueName = null;
			Microsoft.ReportingServices.OnDemandReportRendering.Report reportToRender = null;
			try
			{
				GenerateEventShimROM(pc.ChunkFactory, null, pc, out reportToRender);
				return InteractivityPaginationModuleFactory.CreateInteractivityPaginationModule().ProcessBookmarkNavigationEvent(reportToRender, -1, bookmarkId, out uniqueName);
			}
			finally
			{
				reportToRender?.RenderingContext.CloseRenderingChunkManager();
			}
		}

		public int ProcessDocumentMapNavigationEvent(string documentMapId, EventInformation eventInfo, Microsoft.ReportingServices.ReportProcessing.ProcessingContext processingContext, out OnDemandProcessingResult result)
		{
			result = null;
			if (processingContext == null || documentMapId == null)
			{
				return 0;
			}
			if (ContainsFlag(processingContext.ReportProcessingFlags, ReportProcessingFlags.OnDemandEngine))
			{
				return ProcessOdpDocumentMapNavigationEvent(documentMapId, eventInfo, processingContext, out result);
			}
			result = null;
			return ProcessYukonDocumentMapNavigationEvent(documentMapId, processingContext);
		}

		private int ProcessOdpDocumentMapNavigationEvent(string documentMapId, EventInformation eventInfo, Microsoft.ReportingServices.ReportProcessing.ProcessingContext processingContext, out OnDemandProcessingResult result)
		{
			result = null;
			OnDemandMetadata onDemandMetadata = Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.OnDemandProcessingManager.DeserializeOnDemandMetadata(processingContext.ChunkFactory, null);
			if (!onDemandMetadata.ReportSnapshot.HasDocumentMap)
			{
				return 0;
			}
			ExecutionLogContext executionLogContext = new ExecutionLogContext(processingContext.JobContext);
			ProcessingErrorContext errorContext = null;
			OnDemandProcessingContext odpContext = null;
			Microsoft.ReportingServices.OnDemandReportRendering.RenderingContext odpRenderingContext = null;
			Microsoft.ReportingServices.ReportIntermediateFormat.ReportSnapshot odpReportSnapshot = null;
			Microsoft.ReportingServices.OnDemandReportRendering.Report report = null;
			bool exceptionGenerated = false;
			CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
			try
			{
				report = GenerateEventROM(processingContext, onDemandMetadata, eventInfo, executionLogContext, out errorContext, out odpContext, out odpRenderingContext, out odpReportSnapshot);
				return InteractivityPaginationModuleFactory.CreateInteractivityPaginationModule().ProcessDocumentMapNavigationEvent(report, documentMapId);
			}
			catch (Exception exception)
			{
				exceptionGenerated = true;
				if (NeedWrapperException(exception, errorContext, out Exception wrappedException))
				{
					throw wrappedException;
				}
				throw;
			}
			finally
			{
				result = CleanupEventROM(processingContext, executionLogContext, errorContext, odpContext, odpRenderingContext, odpReportSnapshot, exceptionGenerated);
				if (currentCulture != null)
				{
					Thread.CurrentThread.CurrentCulture = currentCulture;
				}
			}
		}

		private int ProcessYukonDocumentMapNavigationEvent(string documentMapId, Microsoft.ReportingServices.ReportProcessing.ProcessingContext pc)
		{
			Microsoft.ReportingServices.OnDemandReportRendering.Report reportToRender = null;
			try
			{
				GenerateEventShimROM(pc.ChunkFactory, null, pc, out reportToRender);
				return InteractivityPaginationModuleFactory.CreateInteractivityPaginationModule().ProcessDocumentMapNavigationEvent(reportToRender, documentMapId);
			}
			finally
			{
				reportToRender?.RenderingContext.CloseRenderingChunkManager();
			}
		}

		public IDocumentMap GetDocumentMap(EventInformation eventInfo, Microsoft.ReportingServices.ReportProcessing.ProcessingContext processingContext, out OnDemandProcessingResult result)
		{
			result = null;
			if (ContainsFlag(processingContext.ReportProcessingFlags, ReportProcessingFlags.OnDemandEngine))
			{
				return GetOdpDocumentMap(eventInfo, processingContext, out result);
			}
			ChunkFactoryAdapter @object = new ChunkFactoryAdapter(processingContext.ChunkFactory);
			return new ShimDocumentMap(GetYukonDocumentMap(@object.GetReportChunk));
		}

		private IDocumentMap GetOdpDocumentMap(EventInformation eventInfo, Microsoft.ReportingServices.ReportProcessing.ProcessingContext processingContext, out OnDemandProcessingResult result)
		{
			result = null;
			OnDemandMetadata onDemandMetadata = Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.OnDemandProcessingManager.DeserializeOnDemandMetadata(processingContext.ChunkFactory, null);
			if (!onDemandMetadata.ReportSnapshot.HasDocumentMap)
			{
				return null;
			}
			Stream stream = Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.OnDemandProcessingManager.OpenExistingDocumentMapStream(onDemandMetadata, processingContext.ReportContext, processingContext.ChunkFactory);
			if (stream == null)
			{
				ExecutionLogContext executionLogContext = new ExecutionLogContext(processingContext.JobContext);
				ProcessingErrorContext errorContext = null;
				OnDemandProcessingContext odpContext = null;
				Microsoft.ReportingServices.OnDemandReportRendering.RenderingContext odpRenderingContext = null;
				Microsoft.ReportingServices.ReportIntermediateFormat.ReportSnapshot odpReportSnapshot = null;
				Microsoft.ReportingServices.OnDemandReportRendering.Report report = null;
				bool exceptionGenerated = false;
				CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
				try
				{
					report = GenerateEventROM(processingContext, onDemandMetadata, eventInfo, executionLogContext, out errorContext, out odpContext, out odpRenderingContext, out odpReportSnapshot);
					NullRenderer nullRenderer = new NullRenderer();
					nullRenderer.Process(report, odpContext, generateDocumentMap: true, createSnapshot: false);
					stream = nullRenderer.DocumentMapStream;
					if (stream == null)
					{
						onDemandMetadata.ReportSnapshot.HasDocumentMap = false;
					}
					else
					{
						stream.Seek(0L, SeekOrigin.Begin);
					}
				}
				catch (Exception exception)
				{
					exceptionGenerated = true;
					if (NeedWrapperException(exception, errorContext, out Exception wrappedException))
					{
						throw wrappedException;
					}
					throw;
				}
				finally
				{
					result = CleanupEventROM(processingContext, executionLogContext, errorContext, odpContext, odpRenderingContext, odpReportSnapshot, exceptionGenerated);
					if (currentCulture != null)
					{
						Thread.CurrentThread.CurrentCulture = currentCulture;
					}
				}
			}
			if (stream != null)
			{
				return new InternalDocumentMap(new DocumentMapReader(stream));
			}
			return null;
		}

		private DocumentMapNode GetYukonDocumentMap(GetReportChunk getReportChunk)
		{
			if (getReportChunk == null)
			{
				return null;
			}
			DocumentMapNode documentMapNode = null;
			ChunkManager.EventsChunkManager eventsChunkManager = null;
			try
			{
				eventsChunkManager = new ChunkManager.EventsChunkManager(getReportChunk);
				return eventsChunkManager.GetDocumentMapNode();
			}
			finally
			{
				eventsChunkManager?.Close();
			}
		}

		public string ProcessDrillthroughEvent(string drillthroughId, EventInformation eventInfo, Microsoft.ReportingServices.ReportProcessing.ProcessingContext processingContext, out NameValueCollection parameters, out OnDemandProcessingResult result)
		{
			parameters = null;
			result = null;
			if (processingContext == null || drillthroughId == null)
			{
				return null;
			}
			string text = null;
			DrillthroughInfo drillthroughInfo = null;
			if (eventInfo != null)
			{
				drillthroughInfo = eventInfo.GetDrillthroughInfo(drillthroughId);
			}
			if (drillthroughInfo != null)
			{
				text = drillthroughInfo.ReportName;
				DrillthroughParameters reportParameters = drillthroughInfo.ReportParameters;
				parameters = ConvertDrillthroughParametersToNameValueCollection(reportParameters);
				return text;
			}
			ExecutionLogContext executionLogContext = new ExecutionLogContext(processingContext.JobContext);
			ProcessingErrorContext errorContext = null;
			OnDemandProcessingContext odpContext = null;
			Microsoft.ReportingServices.OnDemandReportRendering.RenderingContext odpRenderingContext = null;
			Microsoft.ReportingServices.ReportIntermediateFormat.ReportSnapshot odpReportSnapshot = null;
			Microsoft.ReportingServices.OnDemandReportRendering.Report report = null;
			bool exceptionGenerated = false;
			CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
			try
			{
				report = GenerateEventROM(processingContext, null, eventInfo, executionLogContext, out errorContext, out odpContext, out odpRenderingContext, out odpReportSnapshot);
				return InteractivityPaginationModuleFactory.CreateInteractivityPaginationModule().ProcessDrillthroughEvent(report, -1, drillthroughId, out parameters);
			}
			catch (Exception exception)
			{
				exceptionGenerated = true;
				if (NeedWrapperException(exception, errorContext, out Exception wrappedException))
				{
					throw wrappedException;
				}
				throw;
			}
			finally
			{
				result = CleanupEventROM(processingContext, executionLogContext, errorContext, odpContext, odpRenderingContext, odpReportSnapshot, exceptionGenerated);
				if (currentCulture != null)
				{
					Thread.CurrentThread.CurrentCulture = currentCulture;
				}
			}
		}

		private static NameValueCollection ConvertDrillthroughParametersToNameValueCollection(DrillthroughParameters reportParameters)
		{
			NameValueCollection result = null;
			CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
			try
			{
				if (reportParameters != null)
				{
					Thread.CurrentThread.CurrentCulture = Localization.ClientPrimaryCulture;
					result = new NameValueCollection();
					object obj = null;
					string text = null;
					for (int i = 0; i < reportParameters.Count; i++)
					{
						text = reportParameters.GetKey(i);
						obj = reportParameters.GetValue(i);
						object[] array = obj as object[];
						if (array != null)
						{
							for (int j = 0; j < array.Length; j++)
							{
								result.Add(text, ConvertToStringUsingThreadCulture(array[j]));
							}
						}
						else
						{
							result.Add(text, ConvertToStringUsingThreadCulture(obj));
						}
					}
					return result;
				}
				return result;
			}
			finally
			{
				Thread.CurrentThread.CurrentCulture = currentCulture;
			}
		}

		private static string ConvertToStringUsingThreadCulture(object value)
		{
			return value?.ToString();
		}

		public OnDemandProcessingResult ProcessUserSortEvent(string reportItem, SortOptions sortOption, bool clearOldSorts, Microsoft.ReportingServices.ReportProcessing.ProcessingContext pc, RenderingContext rc, IChunkFactory originalSnapshot, out string newReportItem, out int page)
		{
			if (ContainsFlag(pc.ReportProcessingFlags, ReportProcessingFlags.OnDemandEngine))
			{
				return ProcessOdpUserSortEvent(reportItem, sortOption, clearOldSorts, pc, rc, originalSnapshot, out newReportItem, out page);
			}
			return ProcessYukonUserSortEvent(reportItem, sortOption, clearOldSorts, pc, rc, originalSnapshot, out newReportItem, out page);
		}

		private OnDemandProcessingResult ProcessOdpUserSortEvent(string reportItem, SortOptions sortOption, bool clearOldSorts, Microsoft.ReportingServices.ReportProcessing.ProcessingContext pc, RenderingContext rc, IChunkFactory originalSnapshotChunks, out string newReportItem, out int page)
		{
			page = 1;
			newReportItem = null;
			if (originalSnapshotChunks == null || reportItem == null)
			{
				return null;
			}
			EventInformation userSortInformation = null;
			EventInformation eventInfo = rc.EventInfo;
			if (eventInfo != null)
			{
				userSortInformation = new EventInformation(eventInfo);
			}
			bool flag = ProcessOdpUserSortInformation(reportItem, sortOption, clearOldSorts, ref userSortInformation);
			OnDemandProcessingResult onDemandProcessingResult = null;
			ExecutionLogContext executionLogContext = new ExecutionLogContext(pc.JobContext);
			OnDemandProcessingContext odpContext = null;
			Microsoft.ReportingServices.OnDemandReportRendering.RenderingContext renderingContext = null;
			Microsoft.ReportingServices.ReportIntermediateFormat.ReportSnapshot reportSnapshot = null;
			Microsoft.ReportingServices.OnDemandReportRendering.Report report = null;
			bool exceptionGenerated = false;
			string itemName = pc.ReportContext.ItemName;
			ProcessingErrorContext errorContext = new ProcessingErrorContext();
			int numberOfPages = rc.PreviousTotalPages;
			PaginationMode paginationMode = rc.ClientPaginationMode;
			executionLogContext.StartProcessingTimer();
			CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
			try
			{
				OnDemandMetadata odpMetadata = null;
				newReportItem = reportItem;
				Microsoft.ReportingServices.ReportIntermediateFormat.Report report2;
				GlobalIDOwnerCollection globalIDOwnerCollection = Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.OnDemandProcessingManager.DeserializeOdpReportSnapshot(pc, originalSnapshotChunks, errorContext, fetchSubreports: true, !flag, m_configuration, ref odpMetadata, out report2);
				SortFilterEventInfoMap oldUserSortInformation = Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.OnDemandProcessingManager.DeserializeSortFilterEventInfo(originalSnapshotChunks, globalIDOwnerCollection);
				if (flag)
				{
					reportSnapshot = new ProcessReportOdpUserSort(Configuration, pc, report2, errorContext, rc.StoreServerParametersCallback, globalIDOwnerCollection, executionLogContext, odpMetadata, oldUserSortInformation, userSortInformation, newReportItem).Execute(out odpContext);
					userSortInformation = odpContext.GetUserSortFilterInformation(out newReportItem);
					renderingContext = new Microsoft.ReportingServices.OnDemandReportRendering.RenderingContext(rc.Format, reportSnapshot, userSortInformation, odpContext);
				}
				else
				{
					reportSnapshot = new ProcessReportOdpSnapshot(Configuration, pc, report2, errorContext, rc.StoreServerParametersCallback, globalIDOwnerCollection, executionLogContext, odpMetadata).Execute(out odpContext);
					userSortInformation = null;
					renderingContext = new Microsoft.ReportingServices.OnDemandReportRendering.RenderingContext(rc.Format, reportSnapshot, rc.EventInfo, odpContext);
				}
				if (userSortInformation != null)
				{
					userSortInformation.Changed = true;
				}
				report = new Microsoft.ReportingServices.OnDemandReportRendering.Report(reportSnapshot.Report, reportSnapshot.ReportInstance, renderingContext, itemName, rc.ReportDescription);
				IInteractivityPaginationModule interactivityPaginationModule = InteractivityPaginationModuleFactory.CreateInteractivityPaginationModule();
				page = interactivityPaginationModule.ProcessUserSortEvent(report, newReportItem, ref numberOfPages, ref paginationMode);
				if (page <= 0)
				{
					if (flag && userSortInformation != null && userSortInformation.Changed)
					{
						if (Global.Tracer.TraceVerbose)
						{
							Global.Tracer.Trace(TraceLevel.Verbose, "SortId not found in reprocessed report. Original=" + reportItem + " Reprocessed=" + newReportItem);
						}
					}
					else if (Global.Tracer.TraceVerbose)
					{
						Global.Tracer.Trace(TraceLevel.Verbose, "SortId '" + reportItem + "' not found.");
					}
				}
			}
			catch (Exception exception)
			{
				exceptionGenerated = true;
				if (NeedWrapperException(exception, errorContext, out Exception wrappedException))
				{
					throw wrappedException;
				}
				throw;
			}
			finally
			{
				onDemandProcessingResult = CleanupEventROM(pc, executionLogContext, errorContext, odpContext, renderingContext, reportSnapshot, numberOfPages, paginationMode, exceptionGenerated);
				if (currentCulture != null)
				{
					Thread.CurrentThread.CurrentCulture = currentCulture;
				}
			}
			return onDemandProcessingResult;
		}

		private OnDemandProcessingResult ProcessYukonUserSortEvent(string reportItem, SortOptions sortOption, bool clearOldSorts, Microsoft.ReportingServices.ReportProcessing.ProcessingContext pc, RenderingContext rc, IChunkFactory sourceSnapshotChunks, out string newReportItem, out int page)
		{
			page = 1;
			newReportItem = null;
			EventInformation userSortInformation = null;
			if (sourceSnapshotChunks == null || reportItem == null)
			{
				return null;
			}
			ChunkFactoryAdapter @object = new ChunkFactoryAdapter(sourceSnapshotChunks);
			ChunkFactoryAdapter object2 = new ChunkFactoryAdapter(pc.ChunkFactory);
			EventInformation eventInfo = rc.EventInfo;
			if (eventInfo != null)
			{
				userSortInformation = new EventInformation(eventInfo);
			}
			if (!ProcessUserSortInformation(reportItem, sortOption, clearOldSorts, ref userSortInformation, out int reportItemUniqueName))
			{
				return null;
			}
			ChunkManager.EventsChunkManager eventsChunkManager = null;
			ExecutionLogContext executionLogContext = new ExecutionLogContext(pc.JobContext);
			executionLogContext.StartProcessingTimer();
			string itemName = pc.ReportContext.ItemName;
			ProcessingErrorContext processingErrorContext = new ProcessingErrorContext();
			Microsoft.ReportingServices.OnDemandReportRendering.RenderingContext renderingContext = null;
			try
			{
				DateTime executionTime;
				Hashtable definitionObjects;
				IntermediateFormatVersion intermediateFormatVersion;
				Report report = DeserializeReportFromSnapshot(@object.GetReportChunk, out executionTime, out definitionObjects, out intermediateFormatVersion);
				ProcessingContext processingContext = pc.CreateInternalProcessingContext(null, report, processingErrorContext, executionTime, pc.AllowUserProfileState, isHistorySnapshot: false, snapshotProcessing: true, processWithCachedData: false, @object.GetReportChunk, null);
				processingContext.CreateReportChunkFactory = pc.ChunkFactory;
				processingContext.UserSortFilterProcessing = true;
				eventsChunkManager = new ChunkManager.EventsChunkManager(@object.GetReportChunk, definitionObjects, intermediateFormatVersion);
				processingContext.OldSortFilterEventInfo = eventsChunkManager.GetSortFilterEventInfo();
				processingContext.UserSortFilterInfo = userSortInformation;
				if (pc.Parameters != null)
				{
					pc.Parameters.StoreLabels();
				}
				UserProfileState userProfileState;
				ReportSnapshot reportSnapshot = ProcessReport(report, pc, processingContext, out userProfileState);
				userSortInformation = processingContext.GetUserSortFilterInformation(ref reportItemUniqueName, ref page);
				newReportItem = reportItemUniqueName.ToString(CultureInfo.InvariantCulture);
				ChunkManager.RenderingChunkManager chunkManager = new ChunkManager.RenderingChunkManager(object2.GetReportChunk, null, definitionObjects, null, report.IntermediateFormatVersion);
				Microsoft.ReportingServices.ReportRendering.RenderingContext renderingContext2 = new Microsoft.ReportingServices.ReportRendering.RenderingContext(reportSnapshot, null, executionTime, report.EmbeddedImages, report.ImageStreamNames, null, pc.ReportContext, null, null, object2.GetReportChunk, chunkManager, pc.GetResourceCallback, null, rc.StoreServerParametersCallback, retrieveRenderingInfo: false, pc.AllowUserProfileState, pc.ReportRuntimeSetup, pc.JobContext, pc.DataProtection);
				renderingContext = new Microsoft.ReportingServices.OnDemandReportRendering.RenderingContext(null, reportSnapshot, pc.ChunkFactory, eventInfo);
				Microsoft.ReportingServices.OnDemandReportRendering.Report report2 = new Microsoft.ReportingServices.OnDemandReportRendering.Report(reportSnapshot.Report, reportSnapshot.ReportInstance, renderingContext2, renderingContext, itemName, null);
				int numberOfPages = rc.PreviousTotalPages;
				PaginationMode paginationMode = rc.ClientPaginationMode;
				IInteractivityPaginationModule interactivityPaginationModule = InteractivityPaginationModuleFactory.CreateInteractivityPaginationModule();
				page = interactivityPaginationModule.ProcessUserSortEvent(report2, newReportItem, ref numberOfPages, ref paginationMode);
				return new YukonProcessingResult(reportSnapshot, processingContext.ChunkManager, pc.ChunkFactory, pc.Parameters, report.AutoRefresh, numberOfPages, processingErrorContext.Messages, renderingInfoChanged: false, renderingContext2.RenderingInfoManager, eventInfoChanged: true, userSortInformation, paginationMode, pc.ChunkFactory.ReportProcessingFlags, userProfileState | renderingContext2.UsedUserProfileState, executionLogContext);
			}
			catch (RSException)
			{
				throw;
			}
			catch (Exception innerException)
			{
				throw new ReportProcessingException(innerException, processingErrorContext.Messages);
			}
			finally
			{
				eventsChunkManager?.Close();
				renderingContext?.CloseRenderingChunkManager();
				UpdateHostingEnvironment(processingErrorContext, pc.ReportContext, executionLogContext, ProcessingEngine.YukonEngine, pc.JobContext);
			}
		}

		private Microsoft.ReportingServices.OnDemandReportRendering.Report GenerateEventROM(Microsoft.ReportingServices.ReportProcessing.ProcessingContext pc, OnDemandMetadata odpMetadata, EventInformation eventInfo, ExecutionLogContext executionLogContext, out ProcessingErrorContext errorContext, out OnDemandProcessingContext odpContext, out Microsoft.ReportingServices.OnDemandReportRendering.RenderingContext odpRenderingContext, out Microsoft.ReportingServices.ReportIntermediateFormat.ReportSnapshot odpReportSnapshot)
		{
			Global.Tracer.Assert(executionLogContext != null, "ExecutionLogContext may not be null");
			odpRenderingContext = null;
			odpContext = null;
			errorContext = new ProcessingErrorContext();
			executionLogContext.StartProcessingTimer();
			Microsoft.ReportingServices.ReportIntermediateFormat.Report report;
			GlobalIDOwnerCollection globalIDOwnerCollection = Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.OnDemandProcessingManager.DeserializeOdpReportSnapshot(pc, null, errorContext, fetchSubreports: true, deserializeGroupTree: true, m_configuration, ref odpMetadata, out report);
			odpReportSnapshot = odpMetadata.ReportSnapshot;
			ProcessReportOdpSnapshot processReportOdpSnapshot = new ProcessReportOdpSnapshot(Configuration, pc, report, errorContext, null, globalIDOwnerCollection, executionLogContext, odpMetadata);
			odpReportSnapshot = processReportOdpSnapshot.Execute(out odpContext);
			odpRenderingContext = new Microsoft.ReportingServices.OnDemandReportRendering.RenderingContext(null, odpReportSnapshot, eventInfo, odpContext);
			return new Microsoft.ReportingServices.OnDemandReportRendering.Report(odpReportSnapshot.Report, odpReportSnapshot.ReportInstance, odpRenderingContext, pc.ReportContext.ItemName, null);
		}

		private bool NeedWrapperException(Exception exception, ProcessingErrorContext errorContext, out Exception wrappedException)
		{
			if (exception is RSException)
			{
				wrappedException = null;
				return false;
			}
			wrappedException = new ReportProcessingException(exception, errorContext.Messages);
			return true;
		}

		private OnDemandProcessingResult CleanupEventROM(Microsoft.ReportingServices.ReportProcessing.ProcessingContext pc, ExecutionLogContext executionLogContext, ProcessingErrorContext errorContext, OnDemandProcessingContext odpContext, Microsoft.ReportingServices.OnDemandReportRendering.RenderingContext odpRenderingContext, Microsoft.ReportingServices.ReportIntermediateFormat.ReportSnapshot odpReportSnapshot, bool exceptionGenerated)
		{
			return CleanupEventROM(pc, executionLogContext, errorContext, odpContext, odpRenderingContext, odpReportSnapshot, 0, PaginationMode.Estimate, exceptionGenerated);
		}

		private OnDemandProcessingResult CleanupEventROM(Microsoft.ReportingServices.ReportProcessing.ProcessingContext pc, ExecutionLogContext executionLogContext, ProcessingErrorContext errorContext, OnDemandProcessingContext odpContext, Microsoft.ReportingServices.OnDemandReportRendering.RenderingContext odpRenderingContext, Microsoft.ReportingServices.ReportIntermediateFormat.ReportSnapshot odpReportSnapshot, int pageCount, PaginationMode updatedPaginationMode, bool exceptionGenerated)
		{
			try
			{
				bool eventInfoChanged = false;
				EventInformation newEventInfo = null;
				if (odpRenderingContext != null)
				{
					UpdateEventInfo(odpReportSnapshot, odpContext, odpRenderingContext, ref eventInfoChanged);
					newEventInfo = odpRenderingContext.EventInfo;
					odpRenderingContext.CloseRenderingChunkManager();
				}
				if (errorContext != null && odpReportSnapshot != null)
				{
					errorContext.Combine(odpReportSnapshot.Warnings);
				}
				CleanupOnDemandProcessing(odpContext, resetGroupTreeStorage: false);
				OnDemandProcessingResult result = null;
				if (exceptionGenerated)
				{
					RequestErrorGroupTreeCleanup(odpContext);
				}
				else
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.OnDemandProcessingManager.PreparePartitionedTreesForAsyncSerialization(odpContext);
					result = new FullOnDemandProcessingResult(odpReportSnapshot, odpContext.OdpMetadata.OdpChunkManager, odpContext.OdpMetadata.SnapshotHasChanged, pc.ChunkFactory, pc.Parameters, odpReportSnapshot.Report.EvaluateAutoRefresh(null, odpContext), pageCount, errorContext.Messages, eventInfoChanged, newEventInfo, updatedPaginationMode, pc.ReportProcessingFlags, odpContext.HasUserProfileState, executionLogContext);
				}
				return result;
			}
			finally
			{
				odpContext?.FreeAllResources();
				UpdateHostingEnvironment(errorContext, pc.ReportContext, executionLogContext, ProcessingEngine.OnDemandEngine, pc.JobContext);
			}
		}

		private static void GenerateEventShimROM(IChunkFactory chunkFactory, EventInformation eventInfo, Microsoft.ReportingServices.ReportProcessing.ProcessingContext pc, out Microsoft.ReportingServices.OnDemandReportRendering.Report reportToRender)
		{
			bool flag = false;
			reportToRender = null;
			Stream stream = null;
			ReportSnapshot reportSnapshot = null;
			ChunkManager.RenderingChunkManager renderingChunkManager = null;
			Hashtable instanceObjects = null;
			Hashtable definitionObjects = null;
			Microsoft.ReportingServices.ReportProcessing.Persistence.IntermediateFormatReader.State declarationsRead = null;
			try
			{
				stream = chunkFactory.GetChunk("Main", ReportChunkTypes.Main, ChunkMode.Open, out string _);
				if (stream != null)
				{
					flag = true;
				}
				if (flag)
				{
					Microsoft.ReportingServices.ReportProcessing.Persistence.IntermediateFormatReader intermediateFormatReader = new Microsoft.ReportingServices.ReportProcessing.Persistence.IntermediateFormatReader(stream);
					reportSnapshot = intermediateFormatReader.ReadReportSnapshot();
					instanceObjects = intermediateFormatReader.InstanceObjects;
					declarationsRead = intermediateFormatReader.ReaderState;
					definitionObjects = intermediateFormatReader.DefinitionObjects;
				}
			}
			finally
			{
				stream?.Close();
			}
			Global.Tracer.Assert(reportSnapshot != null, "(null != reportSnapshot)");
			Global.Tracer.Assert(reportSnapshot.Report != null, "(null != reportSnapshot.Report)");
			Global.Tracer.Assert(reportSnapshot.ReportInstance != null, "(null != reportSnapshot.ReportInstance)");
			ChunkFactoryAdapter @object = new ChunkFactoryAdapter(chunkFactory);
			renderingChunkManager = new ChunkManager.RenderingChunkManager(@object.GetReportChunk, instanceObjects, definitionObjects, declarationsRead, reportSnapshot.Report.IntermediateFormatVersion);
			Microsoft.ReportingServices.ReportRendering.RenderingContext oldRenderingContext = new Microsoft.ReportingServices.ReportRendering.RenderingContext(reportSnapshot, null, reportSnapshot.ExecutionTime, null, null, eventInfo, null, null, null, @object.GetReportChunk, renderingChunkManager, null, null, null, retrieveRenderingInfo: false, UserProfileState.None, pc.ReportRuntimeSetup, pc.JobContext, pc.DataProtection);
			Microsoft.ReportingServices.OnDemandReportRendering.RenderingContext renderingContext = new Microsoft.ReportingServices.OnDemandReportRendering.RenderingContext(null, reportSnapshot, chunkFactory, eventInfo);
			reportToRender = new Microsoft.ReportingServices.OnDemandReportRendering.Report(reportSnapshot.Report, reportSnapshot.ReportInstance, oldRenderingContext, renderingContext, null, null);
		}

		internal void ProcessShowHideToggle(string showHideToggle, ReportSnapshot reportSnapshot, EventInformation oldOverrideInformation, ChunkManager.RenderingChunkManager chunkManager, out bool showHideInformationChanged, out EventInformation newOverrideInformation)
		{
			new ShowHideProcessing().Process(showHideToggle, reportSnapshot, oldOverrideInformation, chunkManager, out showHideInformationChanged, out newOverrideInformation);
		}

		private bool ProcessOdpUserSortInformation(string reportItemUniqueName, SortOptions sortOption, bool clearOldSorts, ref EventInformation userSortInformation)
		{
			bool result = false;
			bool eventExists = false;
			if (sortOption == SortOptions.None)
			{
				if (userSortInformation != null && userSortInformation.OdpSortInfo != null && ProcessOdpUserSortInformation(reportItemUniqueName, sortOption, clearOldSorts, ref userSortInformation, out eventExists))
				{
					if (userSortInformation.OdpSortInfo.Count == 0)
					{
						if (userSortInformation.ToggleStateInfo == null && userSortInformation.HiddenInfo == null)
						{
							userSortInformation = null;
						}
						else
						{
							userSortInformation.OdpSortInfo = null;
						}
					}
					result = true;
				}
			}
			else
			{
				if (userSortInformation == null)
				{
					userSortInformation = new EventInformation();
				}
				if (userSortInformation.OdpSortInfo == null)
				{
					userSortInformation.OdpSortInfo = new EventInformation.OdpSortEventInfo();
				}
				result = ProcessOdpUserSortInformation(reportItemUniqueName, sortOption, clearOldSorts, ref userSortInformation, out eventExists);
				if (!eventExists)
				{
					userSortInformation.OdpSortInfo.Add(reportItemUniqueName, (sortOption == SortOptions.Ascending) ? true : false, null);
					result = true;
				}
			}
			return result;
		}

		private bool ProcessOdpUserSortInformation(string reportItemUniqueName, SortOptions sortOption, bool clearOldSorts, ref EventInformation userSortInformation, out bool eventExists)
		{
			eventExists = false;
			bool flag = false;
			if (clearOldSorts)
			{
				flag = userSortInformation.OdpSortInfo.ClearPeerSorts(reportItemUniqueName);
			}
			SortOptions sortState = userSortInformation.OdpSortInfo.GetSortState(reportItemUniqueName);
			if (sortState != 0)
			{
				if (sortState == sortOption)
				{
					eventExists = true;
				}
				else
				{
					flag |= userSortInformation.OdpSortInfo.Remove(reportItemUniqueName);
				}
			}
			return flag;
		}

		private bool ProcessUserSortInformation(string reportItem, SortOptions sortOption, bool clearOldSorts, ref EventInformation userSortInformation, out int reportItemUniqueName)
		{
			reportItemUniqueName = -1;
			if (!int.TryParse(reportItem, NumberStyles.None, CultureInfo.InvariantCulture, out reportItemUniqueName))
			{
				throw new ReportProcessingException(ErrorCode.rsInvalidSortItemID);
			}
			if (0 > reportItemUniqueName)
			{
				throw new ReportProcessingException(ErrorCode.rsInvalidSortItemID);
			}
			bool result = false;
			bool eventExists = false;
			if (sortOption == SortOptions.None)
			{
				if (userSortInformation != null && userSortInformation.SortInfo != null && ProcessUserSortInformation(reportItemUniqueName, sortOption, clearOldSorts, ref userSortInformation, out eventExists))
				{
					if (userSortInformation.SortInfo.Count == 0)
					{
						if (userSortInformation.ToggleStateInfo == null && userSortInformation.HiddenInfo == null)
						{
							userSortInformation = null;
						}
						else
						{
							userSortInformation.SortInfo = null;
						}
					}
					result = true;
				}
			}
			else
			{
				if (userSortInformation == null)
				{
					userSortInformation = new EventInformation();
				}
				if (userSortInformation.SortInfo == null)
				{
					userSortInformation.SortInfo = new EventInformation.SortEventInfo();
				}
				result = ProcessUserSortInformation(reportItemUniqueName, sortOption, clearOldSorts, ref userSortInformation, out eventExists);
				if (!eventExists)
				{
					userSortInformation.SortInfo.Add(reportItemUniqueName, (sortOption == SortOptions.Ascending) ? true : false, null);
					result = true;
				}
			}
			return result;
		}

		private bool ProcessUserSortInformation(int reportItemUniqueName, SortOptions sortOption, bool clearOldSorts, ref EventInformation userSortInformation, out bool eventExists)
		{
			eventExists = false;
			bool flag = false;
			if (clearOldSorts)
			{
				flag = userSortInformation.SortInfo.ClearPeerSorts(reportItemUniqueName);
			}
			SortOptions sortState = userSortInformation.SortInfo.GetSortState(reportItemUniqueName);
			if (sortState != 0)
			{
				if (sortState == sortOption)
				{
					eventExists = true;
				}
				else
				{
					flag |= userSortInformation.SortInfo.Remove(reportItemUniqueName);
				}
			}
			return flag;
		}

		internal ReportProcessing()
		{
		}

		public static bool NeedsUpgradeToLatest(ReportProcessingFlags processingFlags)
		{
			return processingFlags == ReportProcessingFlags.NotSet;
		}

		public static ReportChunkTypes GetImageChunkTypeToCopy(ReportProcessingFlags processingFlags)
		{
			if (ContainsFlag(processingFlags, ReportProcessingFlags.OnDemandEngine))
			{
				return ReportChunkTypes.StaticImage;
			}
			return ReportChunkTypes.Image;
		}

		internal static void CheckReportCredentialsAndConnectionUserDependency(Microsoft.ReportingServices.ReportProcessing.ProcessingContext pc)
		{
			if (pc != null)
			{
				string itemName = (pc.ReportContext != null) ? pc.ReportContext.ItemName : null;
				CheckReportCredentialsAndConnectionUserDependency(pc.DataSources, pc.AllowUserProfileState, itemName);
			}
		}

		internal static void CheckReportCredentialsAndConnectionUserDependency(RuntimeDataSourceInfoCollection dataSources, UserProfileState allowUserProfileState, string itemName)
		{
			if (dataSources != null)
			{
				if (dataSources.NeedPrompt)
				{
					throw new ReportProcessingException(ErrorCode.rsCredentialsNotSpecified);
				}
				if ((allowUserProfileState & UserProfileState.InQuery) == 0 && dataSources.HasConnectionStringUseridReference())
				{
					throw new ReportProcessingException(ErrorCode.rsHasUserProfileDependencies, null, itemName);
				}
			}
		}

		public static bool UpgradeSnapshot(IChunkFactory getChunkFactory, bool isSnapshot, IChunkFactory createChunkFactory, ICatalogItemContext reportContext, out int pageCount, out bool hasDocumentMap)
		{
			pageCount = 0;
			hasDocumentMap = false;
			if (createChunkFactory == null || getChunkFactory == null)
			{
				return false;
			}
			ChunkFactoryAdapter @object = new ChunkFactoryAdapter(createChunkFactory);
			ChunkFactoryAdapter object2 = new ChunkFactoryAdapter(getChunkFactory);
			CreateReportChunk createChunkCallback = @object.CreateReportChunk;
			GetReportChunk getReportChunk = object2.GetReportChunk;
			if (!isSnapshot)
			{
				SerializeReport(DeserializeReport(getReportChunk), createChunkCallback);
				return true;
			}
			Stream stream = null;
			ChunkManager.RenderingChunkManager renderingChunkManager = null;
			ChunkManager.UpgradeManager upgradeManager = null;
			try
			{
				stream = getReportChunk("Main", ReportChunkTypes.Main, out string _);
				Microsoft.ReportingServices.ReportProcessing.Persistence.IntermediateFormatReader intermediateFormatReader = new Microsoft.ReportingServices.ReportProcessing.Persistence.IntermediateFormatReader(stream);
				ReportSnapshot reportSnapshot = intermediateFormatReader.ReadReportSnapshot();
				Hashtable instanceObjects = intermediateFormatReader.InstanceObjects;
				Microsoft.ReportingServices.ReportProcessing.Persistence.IntermediateFormatReader.State readerState = intermediateFormatReader.ReaderState;
				Hashtable definitionObjects = intermediateFormatReader.DefinitionObjects;
				Global.Tracer.Assert(reportSnapshot != null, "(null != reportSnapshot)");
				Global.Tracer.Assert(reportSnapshot.Report != null, "(null != reportSnapshot.Report)");
				Global.Tracer.Assert(reportSnapshot.ReportInstance != null, "(null != reportSnapshot.ReportInstance)");
				renderingChunkManager = new ChunkManager.RenderingChunkManager(getReportChunk, instanceObjects, definitionObjects, readerState, reportSnapshot.Report.IntermediateFormatVersion);
				Upgrader.UpgradeToCurrent(reportSnapshot, renderingChunkManager, createChunkCallback);
				Upgrader.UpgradeDatasetIDs(reportSnapshot.Report);
				reportSnapshot.DocumentMap = reportSnapshot.GetDocumentMap(renderingChunkManager);
				hasDocumentMap = (reportSnapshot.DocumentMap != null);
				pageCount = reportSnapshot.ReportInstance.NumberOfPages;
				reportSnapshot.QuickFind = reportSnapshot.GetQuickFind(renderingChunkManager);
				reportSnapshot.ShowHideReceiverInfo = reportSnapshot.GetShowHideReceiverInfo(renderingChunkManager);
				reportSnapshot.ShowHideSenderInfo = reportSnapshot.GetShowHideSenderInfo(renderingChunkManager);
				upgradeManager = new ChunkManager.UpgradeManager(createChunkCallback);
				Upgrader.CreateBookmarkDrillthroughChunks(reportSnapshot, renderingChunkManager, upgradeManager);
				renderingChunkManager.Close();
				renderingChunkManager = null;
				if (stream != null)
				{
					stream.Close();
					stream = null;
				}
				upgradeManager.FinalFlush();
				upgradeManager.SaveFirstPage();
				upgradeManager.SaveReportSnapshot(reportSnapshot);
			}
			finally
			{
				stream?.Close();
				renderingChunkManager?.Close();
				upgradeManager?.Close();
			}
			return true;
		}

		public OnDemandProcessingResult CreateSnapshot(DateTime executionTimeStamp, Microsoft.ReportingServices.ReportProcessing.ProcessingContext pc, IChunkFactory yukonCompiledDefinition)
		{
			ExecutionLogContext executionLogContext = new ExecutionLogContext(pc.JobContext);
			executionLogContext.StartProcessingTimer();
			ProcessingEngine processingEngine = ProcessingEngine.OnDemandEngine;
			ProcessingErrorContext processingErrorContext = new ProcessingErrorContext();
			Microsoft.ReportingServices.OnDemandReportRendering.RenderingContext renderingContext = null;
			OnDemandProcessingContext odpContext = null;
			CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
			try
			{
				if (!pc.Parameters.ValuesAreValid())
				{
					throw new ReportProcessingException(ErrorCode.rsParameterError);
				}
				string itemName = pc.ReportContext.ItemName;
				CheckReportCredentialsAndConnectionUserDependency(pc.DataSources, pc.AllowUserProfileState, itemName);
				if (pc.ReportProcessingFlags == ReportProcessingFlags.NotSet || ContainsFlag(pc.ReportProcessingFlags, ReportProcessingFlags.YukonEngine))
				{
					processingEngine = ProcessingEngine.YukonEngine;
					ChunkFactoryAdapter @object = new ChunkFactoryAdapter(pc.ChunkFactory);
					Report report = DeserializeReport(new ChunkFactoryAdapter(yukonCompiledDefinition).GetReportChunk);
					ProcessingContext context;
					UserProfileState userProfileState;
					ReportSnapshot reportSnapshot = ProcessReport(report, pc, snapshotProcessing: false, processWithCachedData: false, @object.GetReportChunk, processingErrorContext, executionTimeStamp, null, out context, out userProfileState);
					Global.Tracer.Assert(context != null && context.ChunkManager != null, "(null != context && null != context.ChunkManager)");
					Global.Tracer.Assert(reportSnapshot != null, "(null != reportSnapshot)");
					executionLogContext.AddLegacyDataProcessingTime(context.DataProcessingDurationMs);
					context.ChunkManager.SaveFirstPage();
					context.ChunkManager.SaveReportSnapshot(reportSnapshot);
					return new YukonProcessingResult(reportSnapshot, context.ChunkManager, pc.Parameters, report.AutoRefresh, reportSnapshot.ReportInstance.NumberOfPages, processingErrorContext.Messages, pc.ChunkFactory.ReportProcessingFlags, userProfileState, executionLogContext);
				}
				processingEngine = ProcessingEngine.OnDemandEngine;
				GlobalIDOwnerCollection globalIDOwnerCollection = new GlobalIDOwnerCollection();
				Microsoft.ReportingServices.ReportIntermediateFormat.Report report2 = DeserializeKatmaiReport(pc.ChunkFactory, keepReferences: false, globalIDOwnerCollection);
				Microsoft.ReportingServices.ReportIntermediateFormat.ReportSnapshot reportSnapshot2 = null;
				Microsoft.ReportingServices.OnDemandReportRendering.Report report3 = null;
				reportSnapshot2 = new ProcessReportOdpInitial(Configuration, pc, report2, processingErrorContext, null, globalIDOwnerCollection, executionLogContext, executionTimeStamp).Execute(out odpContext);
				renderingContext = new Microsoft.ReportingServices.OnDemandReportRendering.RenderingContext(null, reportSnapshot2, null, odpContext);
				report3 = new Microsoft.ReportingServices.OnDemandReportRendering.Report(reportSnapshot2.Report, reportSnapshot2.ReportInstance, renderingContext, itemName, null);
				new NullRenderer().Process(report3, odpContext, generateDocumentMap: false, createSnapshot: true);
				CleanupOnDemandProcessing(odpContext, resetGroupTreeStorage: false);
				bool eventInfoChanged = false;
				UpdateEventInfo(reportSnapshot2, odpContext, renderingContext, ref eventInfoChanged);
				Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.OnDemandProcessingManager.PreparePartitionedTreesForSyncSerialization(odpContext);
				FullOnDemandProcessingResult fullOnDemandProcessingResult = new FullOnDemandProcessingResult(reportSnapshot2, odpContext.OdpMetadata.OdpChunkManager, odpContext.OdpMetadata.SnapshotHasChanged, pc.ChunkFactory, pc.Parameters, reportSnapshot2.Report.EvaluateAutoRefresh(null, odpContext), 0, processingErrorContext.Messages, eventInfoChanged, renderingContext.EventInfo, PaginationMode.Estimate, pc.ChunkFactory.ReportProcessingFlags, odpContext.HasUserProfileState, executionLogContext);
				fullOnDemandProcessingResult.Save();
				return fullOnDemandProcessingResult;
			}
			catch (RSException)
			{
				RequestErrorGroupTreeCleanup(odpContext);
				throw;
			}
			catch (Exception innerException)
			{
				RequestErrorGroupTreeCleanup(odpContext);
				throw new ReportProcessingException(innerException, processingErrorContext.Messages);
			}
			finally
			{
				odpContext?.FreeAllResources();
				renderingContext?.CloseRenderingChunkManager();
				UpdateHostingEnvironment(processingErrorContext, pc.ReportContext, executionLogContext, processingEngine, pc.JobContext);
				if (currentCulture != null)
				{
					Thread.CurrentThread.CurrentCulture = currentCulture;
				}
			}
		}

		public static void CreateRenderer(string format, IExtensionFactory extFactory, out IRenderingExtension newRenderer)
		{
			newRenderer = null;
			try
			{
				newRenderer = ReportRendererFactory.GetNewRenderer(format, extFactory);
				if (newRenderer == null)
				{
					throw new ReportProcessingException(ErrorCode.rsRenderingExtensionNotFound);
				}
			}
			catch (RSException)
			{
				throw;
			}
			catch (Exception innerException)
			{
				throw new ReportProcessingException(innerException, null);
			}
		}

		public OnDemandProcessingResult RenderReportAndCacheData(DateTime executionTimeStamp, Microsoft.ReportingServices.ReportProcessing.ProcessingContext pc, RenderingContext rc, IChunkFactory metaDataChunkFactory, IChunkFactory yukonCompiledDefinition)
		{
			CreateRenderer(rc.Format, pc.ExtFactory, out IRenderingExtension newRenderer);
			return RenderReportAndCacheData(newRenderer, executionTimeStamp, pc, rc, metaDataChunkFactory, yukonCompiledDefinition);
		}

		public OnDemandProcessingResult RenderReportAndCacheData(IRenderingExtension newRenderer, DateTime executionTimeStamp, Microsoft.ReportingServices.ReportProcessing.ProcessingContext pc, RenderingContext rc, IChunkFactory metaDataChunkFactory, IChunkFactory yukonCompiledDefinition)
		{
			RenderReport renderReport = (!IsYukonProcessingEngine(pc.ReportProcessingFlags)) ? ((RenderReport)new RenderReportOdpLiveAndCacheData(pc, rc, executionTimeStamp, Configuration, metaDataChunkFactory)) : ((RenderReport)new RenderReportYukonInitial(pc, rc, executionTimeStamp, this, yukonCompiledDefinition));
			return renderReport.Execute(newRenderer);
		}

		public OnDemandProcessingResult RenderDefinitionOnly(DateTime executionTimeStamp, Microsoft.ReportingServices.ReportProcessing.ProcessingContext pc, RenderingContext rc, IChunkFactory yukonCompiledDefinition)
		{
			CreateRenderer(rc.Format, pc.ExtFactory, out IRenderingExtension newRenderer);
			return RenderDefinitionOnly(newRenderer, executionTimeStamp, pc, rc, yukonCompiledDefinition);
		}

		public OnDemandProcessingResult RenderDefinitionOnly(IRenderingExtension newRenderer, DateTime executionTimeStamp, Microsoft.ReportingServices.ReportProcessing.ProcessingContext pc, RenderingContext rc, IChunkFactory yukonCompiledDefinition)
		{
			RenderReport renderReport = (!IsYukonProcessingEngine(pc.ReportProcessingFlags)) ? ((RenderReport)new RenderReportDefinitionOnly(pc, rc, executionTimeStamp, Configuration)) : ((RenderReport)new RenderReportYukonDefinitionOnly(pc, rc, executionTimeStamp, this, yukonCompiledDefinition));
			return renderReport.Execute(newRenderer);
		}

		public OnDemandProcessingResult RenderReport(DateTime executionTimeStamp, Microsoft.ReportingServices.ReportProcessing.ProcessingContext pc, RenderingContext rc, IChunkFactory yukonCompiledDefinition)
		{
			CreateRenderer(rc.Format, pc.ExtFactory, out IRenderingExtension newRenderer);
			return RenderReport(newRenderer, executionTimeStamp, pc, rc, yukonCompiledDefinition);
		}

		public OnDemandProcessingResult RenderReport(IRenderingExtension newRenderer, DateTime executionTimeStamp, Microsoft.ReportingServices.ReportProcessing.ProcessingContext pc, RenderingContext rc, IChunkFactory yukonCompiledDefinition)
		{
			RenderReport renderReport = (!IsYukonProcessingEngine(pc.ReportProcessingFlags)) ? ((RenderReport)new RenderReportOdpInitial(pc, rc, executionTimeStamp, Configuration)) : ((RenderReport)new RenderReportYukonInitial(pc, rc, executionTimeStamp, this, yukonCompiledDefinition));
			return renderReport.Execute(newRenderer);
		}

		public OnDemandProcessingResult RenderSnapshot(RenderingContext rc, Microsoft.ReportingServices.ReportProcessing.ProcessingContext pc)
		{
			CreateRenderer(rc.Format, pc.ExtFactory, out IRenderingExtension newRenderer);
			return RenderSnapshot(newRenderer, rc, pc);
		}

		public OnDemandProcessingResult RenderSnapshot(IRenderingExtension newRenderer, RenderingContext rc, Microsoft.ReportingServices.ReportProcessing.ProcessingContext pc)
		{
			RenderReport renderReport = (!IsYukonProcessingEngine(pc.ReportProcessingFlags)) ? ((RenderReport)new RenderReportOdpSnapshot(pc, rc, Configuration)) : ((RenderReport)new RenderReportYukonSnapshot(pc, rc, this));
			return renderReport.Execute(newRenderer);
		}

		public OnDemandProcessingResult RenderReportWithCachedData(DateTime executionTimeStamp, Microsoft.ReportingServices.ReportProcessing.ProcessingContext pc, RenderingContext rc, IChunkFactory getMetaDataFactory)
		{
			CreateRenderer(rc.Format, pc.ExtFactory, out IRenderingExtension newRenderer);
			return RenderReportWithCachedData(newRenderer, executionTimeStamp, pc, rc, getMetaDataFactory);
		}

		public OnDemandProcessingResult RenderReportWithCachedData(IRenderingExtension newRenderer, DateTime executionTimeStamp, Microsoft.ReportingServices.ReportProcessing.ProcessingContext pc, RenderingContext rc, IChunkFactory getMetaDataFactory)
		{
			if (ContainsFlag(pc.ReportProcessingFlags, ReportProcessingFlags.YukonEngine))
			{
				Global.Tracer.Assert(condition: false, "initial processing based on cached data in Yukon");
				throw new InvalidOperationException();
			}
			return new RenderReportOdpWithCachedData(pc, rc, executionTimeStamp, Configuration, getMetaDataFactory).Execute(newRenderer);
		}

		public OnDemandProcessingResult ProcessAndRenderSnapshot(Microsoft.ReportingServices.ReportProcessing.ProcessingContext pc, RenderingContext rc, IChunkFactory originalSnapshotChunks)
		{
			CreateRenderer(rc.Format, pc.ExtFactory, out IRenderingExtension newRenderer);
			return ProcessAndRenderSnapshot(newRenderer, pc, rc, originalSnapshotChunks);
		}

		public OnDemandProcessingResult ProcessAndRenderSnapshot(IRenderingExtension newRenderer, Microsoft.ReportingServices.ReportProcessing.ProcessingContext pc, RenderingContext rc, IChunkFactory originalSnapshotChunks)
		{
			RenderReport renderReport = (!IsYukonProcessingEngine(pc.ReportProcessingFlags)) ? ((RenderReport)new RenderReportOdpReprocessSnapshot(pc, rc, Configuration, originalSnapshotChunks)) : ((RenderReport)new RenderReportYukonReprocessSnapshot(pc, rc, this, originalSnapshotChunks));
			return renderReport.Execute(newRenderer);
		}

		public OnDemandProcessingResult RenderSnapshotStream(string streamName, RenderingContext rc, Microsoft.ReportingServices.ReportProcessing.ProcessingContext pc)
		{
			CreateRenderer(rc.Format, pc.ExtFactory, out IRenderingExtension newRenderer);
			return RenderSnapshotStream(newRenderer, streamName, rc, pc);
		}

		public OnDemandProcessingResult RenderSnapshotStream(IRenderingExtension newRenderer, string streamName, RenderingContext rc, Microsoft.ReportingServices.ReportProcessing.ProcessingContext pc)
		{
			RenderReport renderReport = (!IsYukonProcessingEngine(pc.ReportProcessingFlags)) ? ((RenderReport)new RenderReportOdpSnapshotStream(pc, rc, Configuration, streamName)) : ((RenderReport)new RenderReportYukonSnapshotStream(pc, rc, this, streamName));
			return renderReport.Execute(newRenderer);
		}

		public void CallRenderer(ICatalogItemContext cc, IExtensionFactory extFactory, CreateAndRegisterStream createAndRegisterStreamCallback)
		{
			CreateRenderer(cc.RSRequestParameters.FormatParamValue, extFactory, out IRenderingExtension newRenderer);
			CallRenderer(newRenderer, cc, createAndRegisterStreamCallback);
		}

		public void CallRenderer(IRenderingExtension newRenderer, ICatalogItemContext cc, CreateAndRegisterStream createAndRegisterStreamCallback)
		{
			try
			{
				newRenderer.GetRenderingResource(createAndRegisterStreamCallback, cc.RSRequestParameters.RenderingParameters);
			}
			catch (RSException)
			{
				throw;
			}
			catch (ReportRenderingException rex)
			{
				HandleRenderingException(rex);
			}
			catch (Exception ex2)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(ex2))
				{
					throw;
				}
				throw new UnhandledReportRenderingException(ex2);
			}
		}

		public void GetAllDataSources(ICatalogItemContext reportContext, IChunkFactory getCompiledDefinitionFactory, OnDemandSubReportDataSourcesCallback subReportCallback, DataSourceInfoCollection dataSources, DataSetInfoCollection dataSetReferences, bool checkIfUsable, ServerDataSourceSettings serverDatasourceSettings, out RuntimeDataSourceInfoCollection allDataSources, out RuntimeDataSetInfoCollection allDataSetReferences)
		{
			try
			{
				allDataSources = new RuntimeDataSourceInfoCollection();
				allDataSetReferences = new RuntimeDataSetInfoCollection();
				Hashtable subReportNames = new Hashtable();
				if (getCompiledDefinitionFactory.ReportProcessingFlags == ReportProcessingFlags.NotSet || ContainsFlag(getCompiledDefinitionFactory.ReportProcessingFlags, ReportProcessingFlags.YukonEngine))
				{
					ChunkFactoryAdapter @object = new ChunkFactoryAdapter(getCompiledDefinitionFactory);
					CheckCredentials(subReportCallback: new SubreportCallbackAdapter(subReportCallback).SubReportDataSourcesCallback, report: DeserializeReport(@object.GetReportChunk), dataSources: dataSources, reportContext: reportContext, allDataSources: allDataSources, subReportLevel: 0, checkIfUsable: checkIfUsable, serverDatasourceSettings: serverDatasourceSettings, subReportNames: subReportNames);
				}
				else
				{
					CheckCredentialsOdp(DeserializeKatmaiReport(getCompiledDefinitionFactory), dataSources, dataSetReferences, reportContext, subReportCallback, allDataSources, allDataSetReferences, 0, checkIfUsable, serverDatasourceSettings, subReportNames);
				}
			}
			catch (RSException)
			{
				throw;
			}
			catch (Exception innerException)
			{
				throw new ReportProcessingException(innerException, null);
			}
		}

		public ParameterInfoCollection GetSnapshotParameters(IChunkFactory getReportChunkFactory)
		{
			ReportProcessingFlags reportProcessingFlags = getReportChunkFactory.ReportProcessingFlags;
			if (reportProcessingFlags == ReportProcessingFlags.NotSet || ContainsFlag(reportProcessingFlags, ReportProcessingFlags.YukonEngine))
			{
				ChunkFactoryAdapter @object = new ChunkFactoryAdapter(getReportChunkFactory);
				return GetYukonSnapshotParameters(@object.GetReportChunk);
			}
			return GetOdpSnapshotParameters(getReportChunkFactory);
		}

		private ParameterInfoCollection GetOdpSnapshotParameters(IChunkFactory chunkFactory)
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.OnDemandProcessingManager.DeserializeOnDemandMetadata(chunkFactory, null).ReportSnapshot.Parameters;
		}

		private ParameterInfoCollection GetYukonSnapshotParameters(GetReportChunk getReportChunkCallback)
		{
			try
			{
				Stream stream = null;
				try
				{
					stream = getReportChunkCallback("Main", ReportChunkTypes.Main, out string _);
					return new Microsoft.ReportingServices.ReportProcessing.Persistence.IntermediateFormatReader(stream).ReadSnapshotParameters();
				}
				finally
				{
					stream?.Close();
				}
			}
			catch (RSException)
			{
				throw;
			}
			catch (Exception innerException)
			{
				throw new ReportProcessingException(innerException, null);
			}
		}

		public ProcessingMessageList ProcessReportParameters(DateTime executionTimeStamp, Microsoft.ReportingServices.ReportProcessing.ProcessingContext pc, bool isSnapshot, out bool needsUpgrade)
		{
			needsUpgrade = false;
			bool flag = true;
			ErrorContext errorContext = new ProcessingErrorContext();
			ProcessingContext processingContext = null;
			Report report = null;
			OnDemandProcessingContext onDemandProcessingContext = null;
			Microsoft.ReportingServices.ReportIntermediateFormat.Report report2 = null;
			CultureInfo cultureInfo = null;
			if (pc.Parameters.IsAnyParameterDynamic)
			{
				if (ContainsFlag(pc.ReportProcessingFlags, ReportProcessingFlags.YukonEngine) || pc.ReportProcessingFlags == ReportProcessingFlags.NotSet)
				{
					flag = false;
					ChunkFactoryAdapter @object = new ChunkFactoryAdapter(pc.ChunkFactory);
					report = ((!isSnapshot) ? DeserializeReport(@object.GetReportChunk) : DeserializeReportFromSnapshot(@object.GetReportChunk, out DateTime _));
				}
				else
				{
					flag = true;
					report2 = DeserializeKatmaiReport(pc.ChunkFactory);
				}
			}
			if (flag)
			{
				onDemandProcessingContext = new OnDemandProcessingContext(pc, report2, errorContext, executionTimeStamp, isSnapshot, m_configuration);
			}
			else
			{
				processingContext = pc.ParametersInternalProcessingContext(errorContext, executionTimeStamp, isSnapshot);
			}
			try
			{
				cultureInfo = Thread.CurrentThread.CurrentCulture;
				Thread.CurrentThread.CurrentCulture = Localization.ClientPrimaryCulture;
				ProcessingMessageList processingMessageList = null;
				if (flag)
				{
					return ProcessReportParameters(report2, onDemandProcessingContext, pc.Parameters);
				}
				return ProcessReportParameters(report, processingContext, pc.Parameters);
			}
			catch (RSException)
			{
				throw;
			}
			catch (Exception innerException)
			{
				throw new ReportProcessingException(innerException, errorContext.Messages);
			}
			finally
			{
				if (flag)
				{
					onDemandProcessingContext.UnregisterAbortInfo();
					pc.Parameters.UserProfileState |= onDemandProcessingContext.HasUserProfileState;
				}
				else
				{
					processingContext.AbortInfo.Dispose();
					processingContext.AbortInfo = null;
					pc.Parameters.UserProfileState |= processingContext.HasUserProfileState;
				}
				if (cultureInfo != null)
				{
					Thread.CurrentThread.CurrentCulture = cultureInfo;
				}
			}
		}

		internal static bool ContainsFlag(ReportProcessingFlags processingFlags, ReportProcessingFlags flag)
		{
			return (processingFlags & flag) == flag;
		}

		internal static bool IsYukonProcessingEngine(ReportProcessingFlags processingFlags)
		{
			if (processingFlags != 0)
			{
				return ContainsFlag(processingFlags, ReportProcessingFlags.YukonEngine);
			}
			return true;
		}

		internal static void RequestErrorGroupTreeCleanup(OnDemandProcessingContext odpContext)
		{
			if (odpContext != null && odpContext.OdpMetadata != null)
			{
				odpContext.OdpMetadata.DisposePersistedTreeScalability();
			}
		}

		internal static void CleanupOnDemandProcessing(OnDemandProcessingContext topLevelOdpContext, bool resetGroupTreeStorage)
		{
			if (topLevelOdpContext == null)
			{
				return;
			}
			topLevelOdpContext.FreeAllResources();
			OnDemandMetadata odpMetadata = topLevelOdpContext.OdpMetadata;
			if (odpMetadata != null)
			{
				if (odpMetadata.OdpChunkManager != null)
				{
					topLevelOdpContext.OdpMetadata.OdpChunkManager.SetOdpContext(topLevelOdpContext);
				}
				if (odpMetadata.GroupTreeScalabilityCache != null)
				{
					UpdateExecutionLogContextForTreeCache(topLevelOdpContext, odpMetadata, odpMetadata.GroupTreeScalabilityCache);
				}
				if (odpMetadata.LookupScalabilityCache != null)
				{
					UpdateExecutionLogContextForTreeCache(topLevelOdpContext, odpMetadata, odpMetadata.LookupScalabilityCache);
				}
			}
			if (resetGroupTreeStorage)
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.OnDemandProcessingManager.PreparePartitionedTreesForAsyncSerialization(topLevelOdpContext);
			}
		}

		private static void UpdateExecutionLogContextForTreeCache(OnDemandProcessingContext topLevelOdpContext, OnDemandMetadata odpMetadata, PartitionedTreeScalabilityCache cache)
		{
			ExecutionLogContext executionLogContext = topLevelOdpContext.ExecutionLogContext;
			long num = cache.SerializationDurationMs;
			if (odpMetadata.IsInitialProcessingRequest)
			{
				num += cache.DeserializationDurationMs;
			}
			executionLogContext.UpdateForTreeScaleCache(num, cache.PeakMemoryUsageKBytes);
		}

		internal static void UpdateHostingEnvironment(ErrorContext errorContext, ICatalogItemContext itemContext, ExecutionLogContext executionLogContext, ProcessingEngine processingEngine, IJobContext jobContext)
		{
			UpdateHostingEnvironment(errorContext, itemContext, executionLogContext, processingEngine, jobContext, null);
		}

		internal static void UpdateHostingEnvironment(ErrorContext errorContext, ICatalogItemContext itemContext, ExecutionLogContext executionLogContext, ProcessingEngine processingEngine, IJobContext jobContext, string sharedDataSetMessage)
		{
			if (jobContext != null)
			{
				Global.Tracer.Assert(executionLogContext != null, "ExecutionLogContext must not be null");
				executionLogContext.StopAllRunningTimers();
				long reportProcessingDurationMsNormalized = executionLogContext.ReportProcessingDurationMsNormalized;
				long dataProcessingDurationMsNormalized = executionLogContext.DataProcessingDurationMsNormalized;
				long reportRenderingDurationMsNormalized = executionLogContext.ReportRenderingDurationMsNormalized;
				_ = executionLogContext.ProcessingScalabilityDurationMsNormalized;
				lock (jobContext.SyncRoot)
				{
					if (dataProcessingDurationMsNormalized != 0L)
					{
						jobContext.TimeDataRetrieval += TimeSpan.FromMilliseconds(dataProcessingDurationMsNormalized);
					}
					if (reportProcessingDurationMsNormalized != 0L)
					{
						jobContext.TimeProcessing += TimeSpan.FromMilliseconds(reportProcessingDurationMsNormalized);
					}
					if (reportRenderingDurationMsNormalized != 0L)
					{
						jobContext.TimeRendering += TimeSpan.FromMilliseconds(reportRenderingDurationMsNormalized);
					}
					if (jobContext.AdditionalInfo.ScalabilityTime == null)
					{
						jobContext.AdditionalInfo.ScalabilityTime = new ScaleTimeCategory();
					}
					jobContext.AdditionalInfo.ScalabilityTime.Processing = executionLogContext.ProcessingScalabilityDurationMsNormalized;
					if (jobContext.AdditionalInfo.EstimatedMemoryUsageKB == null)
					{
						jobContext.AdditionalInfo.EstimatedMemoryUsageKB = new EstimatedMemoryUsageKBCategory();
					}
					jobContext.AdditionalInfo.EstimatedMemoryUsageKB.Processing = executionLogContext.PeakProcesssingMemoryUsage;
					if (sharedDataSetMessage != null)
					{
						jobContext.AdditionalInfo.SharedDataSet = sharedDataSetMessage;
					}
					else
					{
						AdditionalInfo additionalInfo = jobContext.AdditionalInfo;
						int num = (int)processingEngine;
						additionalInfo.ProcessingEngine = num.ToString(CultureInfo.InvariantCulture);
					}
					if (executionLogContext.ExternalImageCount > 0)
					{
						ExternalImageCategory externalImageCategory = new ExternalImageCategory();
						externalImageCategory.Count = executionLogContext.ExternalImageCount.ToString(CultureInfo.InvariantCulture);
						externalImageCategory.ByteCount = executionLogContext.ExternalImageBytes.ToString(CultureInfo.InvariantCulture);
						externalImageCategory.ResourceFetchTime = executionLogContext.ExternalImageDurationMs.ToString(CultureInfo.InvariantCulture);
						jobContext.AdditionalInfo.ExternalImages = externalImageCategory;
					}
					jobContext.AdditionalInfo.Connections = executionLogContext.GetConnectionMetrics();
				}
			}
			TraceProcessingMessages(errorContext, itemContext);
		}

		internal static void TraceProcessingMessages(ErrorContext errorContext, ICatalogItemContext itemContext)
		{
			if (errorContext == null || errorContext.Messages == null)
			{
				return;
			}
			ProcessingMessageList messages = errorContext.Messages;
			int count = messages.Count;
			if (Global.Tracer.TraceVerbose && count > 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("The following messages were generated while processing item: '{0}':", itemContext.ItemPathAsString.MarkAsPrivate());
				for (int i = 0; i < count; i++)
				{
					stringBuilder.AppendLine();
					stringBuilder.Append("\t");
					stringBuilder.Append(messages[i].FormatMessage());
				}
				Global.Tracer.Trace(TraceLevel.Verbose, stringBuilder.ToString());
			}
		}

		internal static void UpdateEventInfo(Microsoft.ReportingServices.ReportIntermediateFormat.ReportSnapshot odpReportSnapshot, OnDemandProcessingContext odpContext, Microsoft.ReportingServices.OnDemandReportRendering.RenderingContext odpRenderingContext, ref bool eventInfoChanged)
		{
			if (odpReportSnapshot != null)
			{
				if (odpContext.NewSortFilterEventInfo != null && odpContext.NewSortFilterEventInfo.Count > 0)
				{
					odpReportSnapshot.SortFilterEventInfo = odpContext.NewSortFilterEventInfo;
				}
				else
				{
					odpReportSnapshot.SortFilterEventInfo = null;
				}
			}
			eventInfoChanged |= odpRenderingContext.EventInfoChanged;
		}

		internal static void UpdateEventInfo(Microsoft.ReportingServices.ReportIntermediateFormat.ReportSnapshot odpReportSnapshot, OnDemandProcessingContext odpContext, Microsoft.ReportingServices.OnDemandReportRendering.RenderingContext odpRenderingContext, RenderingContext rc, ref bool eventInfoChanged)
		{
			UpdateEventInfo(odpReportSnapshot, odpContext, odpRenderingContext, ref eventInfoChanged);
			if (eventInfoChanged)
			{
				rc.EventInfo = odpRenderingContext.EventInfo;
			}
		}

		internal static void HandleRenderingException(ReportRenderingException rex)
		{
			if (rex.InnerException != null && (rex.InnerException is RSException || rex.InnerException is DataCacheUnavailableException))
			{
				if (rex.InnerException is RSException)
				{
					throw new RSException((RSException)rex.InnerException);
				}
				throw new DataCacheUnavailableException((DataCacheUnavailableException)rex.InnerException);
			}
			if (rex.Unexpected)
			{
				throw new UnhandledReportRenderingException(rex);
			}
			throw new HandledReportRenderingException(rex);
		}

		private Report CompileYukonReport(ICatalogItemContext reportContext, byte[] reportDefinition, CreateReportChunk createChunkCallback, CheckSharedDataSource checkDataSourceCallback, ResolveTemporaryDataSource resolveTemporaryDataSourceCallback, DataSourceInfoCollection originalDataSources, PublishingErrorContext errorContext, AppDomain compilationTempAppDomain, bool generateExpressionHostWithRefusedPermissions, IDataProtection dataProtection, out string reportDescription, out string reportLanguage, out ParameterInfoCollection parameters, out DataSourceInfoCollection dataSources, out UserLocationFlags userReferenceLocation, out ArrayList dataSetsName, out bool hasExternalImages, out bool hasHyperlinks)
		{
			Report report = new ReportPublishing().CreateIntermediateFormat(reportContext, reportDefinition, createChunkCallback, checkDataSourceCallback, resolveTemporaryDataSourceCallback, originalDataSources, errorContext, compilationTempAppDomain, generateExpressionHostWithRefusedPermissions, dataProtection, out reportDescription, out reportLanguage, out parameters, out dataSources, out userReferenceLocation, out dataSetsName, out hasExternalImages, out hasHyperlinks);
			if (createChunkCallback != null)
			{
				SerializeReport(report, createChunkCallback);
			}
			return report;
		}

		internal ReportSnapshot ProcessReport(Report report, Microsoft.ReportingServices.ReportProcessing.ProcessingContext pc, bool snapshotProcessing, bool processWithCachedData, GetReportChunk getChunkCallback, ErrorContext errorContext, DateTime executionTime, CreateReportChunk cacheDataCallback, out ProcessingContext context, out UserProfileState userProfileState)
		{
			context = pc.CreateInternalProcessingContext(null, report, errorContext, executionTime, pc.AllowUserProfileState, pc.IsHistorySnapshot, snapshotProcessing, processWithCachedData, getChunkCallback, cacheDataCallback);
			context.CreateReportChunkFactory = pc.ChunkFactory;
			return ProcessReport(report, pc, context, out userProfileState);
		}

		private bool HasUserSortFilter(Report report, uint subreportLevel, ProcessingContext context)
		{
			if (report == null)
			{
				return false;
			}
			if (report.HasUserSortFilter)
			{
				return true;
			}
			if (context.SubReportCallback == null)
			{
				return true;
			}
			if (subreportLevel <= 20 && report.SubReports != null)
			{
				int count = report.SubReports.Count;
				for (int i = 0; i < count; i++)
				{
					SubReport subReport = report.SubReports[i];
					if (subReport.RetrievalStatus == SubReport.Status.NotRetrieved && context.SubReportCallback != null)
					{
						RuntimeRICollection.RetrieveSubReport(subReport, context, null, isProcessingPrefetch: true);
					}
					if (subReport.Report != null && HasUserSortFilter(subReport.Report, subreportLevel + 1, context))
					{
						return true;
					}
				}
			}
			return false;
		}

		internal static void FetchSubReports(Microsoft.ReportingServices.ReportIntermediateFormat.Report report, IChunkFactory getReportChunks, ErrorContext errorContext, OnDemandMetadata odpMetadata, ICatalogItemContext parentReportContext, OnDemandSubReportCallback subReportCallback, int subReportLevel, bool snapshotProcessing, bool processWithCachedData, GlobalIDOwnerCollection globalIDOwnerCollection, ParameterInfoCollection parentQueryParameters)
		{
			if ((long)subReportLevel > 20L)
			{
				foreach (Microsoft.ReportingServices.ReportIntermediateFormat.SubReport subReport in report.SubReports)
				{
					subReport.ExceededMaxLevel = true;
				}
				return;
			}
			foreach (Microsoft.ReportingServices.ReportIntermediateFormat.SubReport subReport2 in report.SubReports)
			{
				try
				{
					string reportName = subReport2.ReportName;
					SubReportInfo subReportInfo;
					if (processWithCachedData)
					{
						if (!odpMetadata.TryGetSubReportInfo(subReportLevel == 0, subReport2.SubReportDefinitionPath, reportName, out subReportInfo))
						{
							throw new DataCacheUnavailableException();
						}
					}
					else if (!snapshotProcessing)
					{
						subReport2.OriginalCatalogPath = parentReportContext.MapUserProvidedPath(subReport2.ReportName);
						subReportInfo = odpMetadata.AddSubReportInfo(subReportLevel == 0, subReport2.SubReportDefinitionPath, reportName, subReport2.OriginalCatalogPath);
					}
					else
					{
						subReportInfo = odpMetadata.GetSubReportInfo(subReportLevel == 0, subReport2.SubReportDefinitionPath, reportName);
						if (subReportInfo != null && subReportInfo.CommonSubReportInfo != null)
						{
							subReport2.OriginalCatalogPath = subReportInfo.CommonSubReportInfo.OriginalCatalogPath;
						}
					}
					DeserializeKatmaiSubReport(subReport2, getReportChunks, parentReportContext, subReportCallback, subReportInfo, snapshotProcessing, errorContext, globalIDOwnerCollection, processWithCachedData, parentQueryParameters);
				}
				catch (DataCacheUnavailableException)
				{
					throw;
				}
				catch (Exception e)
				{
					subReport2.RetrievalStatus = Microsoft.ReportingServices.ReportIntermediateFormat.SubReport.Status.DefinitionRetrieveFailed;
					HandleSubReportProcessingError(errorContext, subReport2, "0", null, e);
				}
				if (subReport2.Report != null)
				{
					if (subReport2.Report.HasSubReports)
					{
						FetchSubReports(subReport2.Report, getReportChunks, errorContext, odpMetadata, subReport2.ReportContext, subReportCallback, subReportLevel + 1, snapshotProcessing, processWithCachedData, globalIDOwnerCollection, parentQueryParameters);
					}
					report.ReportOrDescendentHasUserSortFilter |= subReport2.Report.ReportOrDescendentHasUserSortFilter;
				}
			}
		}

		private static void DeserializeKatmaiSubReport(Microsoft.ReportingServices.ReportIntermediateFormat.SubReport subReport, IChunkFactory getReportChunks, ICatalogItemContext reportContext, OnDemandSubReportCallback subReportCallback, SubReportInfo subReportInfo, bool snapshotProcessing, ErrorContext errorContext, GlobalIDOwnerCollection globalIDOwnerCollection, bool processWithCachedData, ParameterInfoCollection parentQueryParameters)
		{
			CommonSubReportInfo commonSubReportInfo = subReportInfo.CommonSubReportInfo;
			try
			{
				IChunkFactory getCompiledDefinitionCallback;
				string chunkName;
				if (commonSubReportInfo.DefinitionChunkFactory == null)
				{
					if (snapshotProcessing)
					{
						getCompiledDefinitionCallback = getReportChunks;
						chunkName = commonSubReportInfo.DefinitionUniqueName;
						subReport.ReportContext = reportContext.GetSubreportContext(commonSubReportInfo.ReportPath);
						if (!commonSubReportInfo.RetrievalFailed)
						{
							goto IL_011e;
						}
						subReport.RetrievalStatus = Microsoft.ReportingServices.ReportIntermediateFormat.SubReport.Status.DefinitionRetrieveFailed;
						return;
					}
					subReportCallback(reportContext, subReport.ReportName, commonSubReportInfo.DefinitionUniqueName, NeedsUpgradeImpl, parentQueryParameters, out ICatalogItemContext subreportContext, out string description, out getCompiledDefinitionCallback, out ParameterInfoCollection parameters);
					if (getCompiledDefinitionCallback != null)
					{
						if (!ContainsFlag(getCompiledDefinitionCallback.ReportProcessingFlags, ReportProcessingFlags.OnDemandEngine))
						{
							Global.Tracer.Trace(TraceLevel.Warning, "The subreport '{0}' could not be processed within parent report '{1}' due to a mismatch in execution engines. Either the subreport failed to automatically republish, or the subreport contains a Reporting Services 2005-style CustomReportItem. To correct this error, please attempt to republish the subreport manually. If it contains a CustomReportItem, please upgrade the report to the latest version.", subReport.ReportName.MarkAsPrivate(), reportContext.ItemPathAsString.MarkAsPrivate());
							errorContext.Register(ProcessingErrorCode.rsEngineMismatchSubReport, Severity.Warning, subReport.ObjectType, subReport.Name, null, subReport.Name, reportContext.ItemPathAsString.MarkAsPrivate());
							subReport.RetrievalStatus = Microsoft.ReportingServices.ReportIntermediateFormat.SubReport.Status.DefinitionRetrieveFailed;
						}
						chunkName = "CompiledDefinition";
						commonSubReportInfo.ParametersFromCatalog = parameters;
						commonSubReportInfo.Description = description;
						subReport.ReportContext = subreportContext;
						goto IL_011e;
					}
				}
				else
				{
					chunkName = ((!snapshotProcessing) ? "CompiledDefinition" : commonSubReportInfo.DefinitionUniqueName);
					getCompiledDefinitionCallback = commonSubReportInfo.DefinitionChunkFactory;
					subReport.ReportContext = reportContext.GetSubreportContext(commonSubReportInfo.ReportPath);
					if (!commonSubReportInfo.RetrievalFailed)
					{
						goto IL_016b;
					}
					subReport.RetrievalStatus = Microsoft.ReportingServices.ReportIntermediateFormat.SubReport.Status.DefinitionRetrieveFailed;
				}
				goto end_IL_0009;
				IL_016b:
				subReport.ParametersFromCatalog = commonSubReportInfo.ParametersFromCatalog;
				subReport.Description = commonSubReportInfo.Description;
				subReport.Report = DeserializeKatmaiReport(getCompiledDefinitionCallback, chunkName, snapshotProcessing, globalIDOwnerCollection, subReport, subReport);
				subReport.UpdateSubReportEventSourceGlobalDataSetIds(subReportInfo);
				subReport.RetrievalStatus = Microsoft.ReportingServices.ReportIntermediateFormat.SubReport.Status.DefinitionRetrieved;
				goto end_IL_0009;
				IL_011e:
				commonSubReportInfo.DefinitionChunkFactory = getCompiledDefinitionCallback;
				goto IL_016b;
				end_IL_0009:;
			}
			catch (IncompatibleFormatVersionException)
			{
				Global.Tracer.Assert(condition: false, "IncompatibleFormatVersion");
			}
			catch (Exception)
			{
				commonSubReportInfo.RetrievalFailed = true;
				subReport.RetrievalStatus = Microsoft.ReportingServices.ReportIntermediateFormat.SubReport.Status.DefinitionRetrieveFailed;
				throw;
			}
		}

		private static bool NeedsUpgradeImpl(ReportProcessingFlags flags)
		{
			if (flags == ReportProcessingFlags.NotSet)
			{
				return true;
			}
			return false;
		}

		internal static void HandleSubReportProcessingError(ErrorContext errorContext, Microsoft.ReportingServices.ReportIntermediateFormat.SubReport subReport, string instanceID, ErrorContext subReportErrorContext, Exception e)
		{
			if (e is DataCacheUnavailableException || e.InnerException is DataCacheUnavailableException)
			{
				throw e;
			}
			if (e is ItemNotFoundException)
			{
				if (Global.Tracer.TraceError)
				{
					Global.Tracer.Trace(TraceLevel.Error, "An error has occurred while processing a sub-report.  The report definition could not be retrieved. Details: {0}", e.Message);
				}
			}
			else if (!(e is ProcessingAbortedException) && Global.Tracer.TraceError)
			{
				Global.Tracer.Trace(TraceLevel.Error, "An error has occurred while processing a sub-report. Details: {0} Stack trace:\r\n{1}", e.Message, e.StackTrace);
			}
			if (subReportErrorContext == null)
			{
				errorContext.Register(ProcessingErrorCode.rsErrorExecutingSubreport, Severity.Warning, subReport.ObjectType, subReport.Name, instanceID, e.Message);
				return;
			}
			if (e is RSException)
			{
				subReportErrorContext.Register((RSException)e, subReport.ObjectType);
			}
			errorContext.Register(ProcessingErrorCode.rsErrorExecutingSubreport, Severity.Warning, subReport.ObjectType, subReport.Name, instanceID, subReportErrorContext.Messages, e.Message);
		}

		private ReportSnapshot ProcessReport(Report report, Microsoft.ReportingServices.ReportProcessing.ProcessingContext pc, ProcessingContext context, out UserProfileState userProfileState)
		{
			ReportSnapshot reportSnapshot = null;
			CultureInfo cultureInfo = null;
			bool flag = HasUserSortFilter(report, 0u, context);
			if ((pc.InitialUserProfileState & UserProfileState.InQuery) > UserProfileState.None)
			{
				context.SaveSnapshotData = flag;
				context.StopSaveSnapshotDataOnError = !flag;
			}
			else if ((pc.InitialUserProfileState & UserProfileState.InReport) == 0)
			{
				context.SaveSnapshotData = (report.ParametersNotUsedInQuery || flag);
				context.StopSaveSnapshotDataOnError = !flag;
			}
			if (pc.IsHistorySnapshot)
			{
				context.SaveSnapshotData = true;
				context.StopSaveSnapshotDataOnError = false;
			}
			userProfileState = UserProfileState.None;
			try
			{
				reportSnapshot = new ReportSnapshot(report, pc.ReportContext.ItemName, pc.Parameters, pc.RequestUserName, context.ExecutionTime, pc.ReportContext.HostRootUri, pc.ReportContext.ParentPath, pc.UserLanguage.Name);
				cultureInfo = Thread.CurrentThread.CurrentCulture;
				Merge merge = new Merge(report, context);
				if (!context.SnapshotProcessing && !context.ProcessWithCachedData && merge.PrefetchData(pc.Parameters, mergeTran: false))
				{
					context.SnapshotProcessing = true;
					context.ResetForSubreportDataPrefetch = true;
				}
				reportSnapshot.ReportInstance = merge.Process(pc.Parameters, mergeTran: false);
				userProfileState = context.HasUserProfileState;
				ReportDrillthroughInfo drillthroughInfo = context.DrillthroughInfo;
				PageMergeInteractive pageMergeInteractive = new PageMergeInteractive();
				userProfileState |= pageMergeInteractive.Process(context.PageSectionContext.PageTextboxes, reportSnapshot, pc.ReportContext, pc.ReportContext.ItemName, pc.Parameters, context.ChunkManager, pc.CreateReportChunkCallback, pc.GetResourceCallback, context.ErrorContext, context.AllowUserProfileState, context.ReportRuntimeSetup, context.UniqueNameCounter, pc.DataProtection, ref drillthroughInfo);
				merge.CleanupDataChunk(userProfileState);
				context.GetSenderAndReceiverInfo(out SenderInformationHashtable senderInfo, out ReceiverInformationHashtable receiverInfo);
				reportSnapshot.ShowHideSenderInfo = senderInfo;
				if (senderInfo != null || context.HasUserSortFilter)
				{
					reportSnapshot.HasShowHide = true;
				}
				reportSnapshot.ShowHideReceiverInfo = receiverInfo;
				if (context.QuickFind != null && context.QuickFind.Count > 0)
				{
					reportSnapshot.QuickFind = context.QuickFind;
				}
				else
				{
					reportSnapshot.QuickFind = null;
				}
				if (drillthroughInfo != null && drillthroughInfo.Count > 0)
				{
					reportSnapshot.DrillthroughInfo = drillthroughInfo;
				}
				else
				{
					reportSnapshot.DrillthroughInfo = null;
				}
				reportSnapshot.CreateNavigationActions(context.NavigationInfo);
				if (context.HasImageStreams || report.HasImageStreams)
				{
					reportSnapshot.HasImageStreams = true;
				}
				if (context.NewSortFilterEventInfo != null && context.NewSortFilterEventInfo.Count > 0)
				{
					reportSnapshot.SortFilterEventInfo = context.NewSortFilterEventInfo;
				}
				else
				{
					reportSnapshot.SortFilterEventInfo = null;
				}
				context.ChunkManager.PageSectionFlush(reportSnapshot);
				context.ChunkManager.FinalFlush();
				report.MainChunkSize = context.ChunkManager.TotalCount * 50;
				reportSnapshot.Warnings = context.ErrorContext.Messages;
				return reportSnapshot;
			}
			finally
			{
				if (cultureInfo != null)
				{
					Thread.CurrentThread.CurrentCulture = cultureInfo;
				}
				foreach (DataSourceInfo value in context.GlobalDataSourceInfo.Values)
				{
					if (value.TransactionInfo != null)
					{
						if (value.TransactionInfo.RollbackRequired)
						{
							if (Global.Tracer.TraceInfo)
							{
								Global.Tracer.Trace(TraceLevel.Info, "Data source '{0}': Rolling back transaction.", value.DataSourceName.MarkAsModelInfo());
							}
							try
							{
								value.TransactionInfo.Transaction.Rollback();
							}
							catch (Exception innerException)
							{
								throw new ReportProcessingException(ErrorCode.rsErrorRollbackTransaction, innerException, value.DataSourceName);
							}
						}
						else
						{
							if (Global.Tracer.TraceVerbose)
							{
								Global.Tracer.Trace(TraceLevel.Verbose, "Data source '{0}': Committing transaction.", value.DataSourceName.MarkAsModelInfo());
							}
							try
							{
								value.TransactionInfo.Transaction.Commit();
							}
							catch (Exception innerException2)
							{
								throw new ReportProcessingException(ErrorCode.rsErrorCommitTransaction, innerException2, value.DataSourceName.MarkAsModelInfo());
							}
						}
					}
					if (value.Connection != null)
					{
						try
						{
							pc.CreateAndSetupDataExtensionFunction.CloseConnectionWithoutPool(value.Connection);
						}
						catch (Exception innerException3)
						{
							throw new ReportProcessingException(ErrorCode.rsErrorClosingConnection, innerException3, value.DataSourceName);
						}
					}
				}
				if (context != null && context.ChunkManager != null)
				{
					context.ChunkManager.Close();
				}
				context.AbortInfo.Dispose();
				context.AbortInfo = null;
			}
		}

		internal static ProcessingMessageList ProcessReportParameters(Report report, ProcessingContext mergeContext, ParameterInfoCollection parameters)
		{
			return new LegacyProcessReportParameters(report, (ReportProcessingContext)mergeContext).Process(parameters);
		}

		internal static ProcessingMessageList ProcessReportParameters(Microsoft.ReportingServices.ReportIntermediateFormat.Report report, OnDemandProcessingContext context, ParameterInfoCollection parameters)
		{
			return new OnDemandProcessReportParameters(report, context).Process(parameters);
		}

		private static Microsoft.ReportingServices.ReportIntermediateFormat.Report DeserializeKatmaiReport(IChunkFactory chunkFactory)
		{
			return DeserializeKatmaiReport(chunkFactory, "CompiledDefinition", keepReferences: false, null, null, null);
		}

		internal static Microsoft.ReportingServices.ReportIntermediateFormat.Report DeserializeKatmaiReport(IChunkFactory chunkFactory, bool keepReferences, GlobalIDOwnerCollection globalIDOwnerCollection)
		{
			return DeserializeKatmaiReport(chunkFactory, "CompiledDefinition", keepReferences, globalIDOwnerCollection, null, null);
		}

		private static Microsoft.ReportingServices.ReportIntermediateFormat.Report DeserializeKatmaiReport(IChunkFactory chunkFactory, string chunkName, bool keepReferences, GlobalIDOwnerCollection globalIDOwnerCollection, Microsoft.ReportingServices.ReportIntermediateFormat.IDOwner parentIDOwner, Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem parentReportItem)
		{
			Stream stream = null;
			try
			{
				stream = chunkFactory.GetChunk(chunkName, ReportChunkTypes.CompiledDefinition, ChunkMode.Open, out string _);
				return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.DeserializeReport(keepReferences, globalIDOwnerCollection, parentIDOwner, parentReportItem, stream);
			}
			finally
			{
				stream?.Close();
			}
		}

		private static Report DeserializeReport(GetReportChunk getChunkCallback)
		{
			return DeserializeReport(getChunkCallback, null);
		}

		internal static Report DeserializeReport(GetReportChunk getChunkCallback, out Hashtable definitionObjects)
		{
			return DeserializeReport(getChunkCallback, null, out definitionObjects);
		}

		private static Report DeserializeReport(GetReportChunk getChunkCallback, ReportItem parent)
		{
			Hashtable definitionObjects = null;
			return DeserializeReport(getChunkCallback, parent, out definitionObjects);
		}

		private static Report DeserializeReport(GetReportChunk getChunkCallback, ReportItem parent, out Hashtable definitionObjects)
		{
			Stream stream = null;
			try
			{
				stream = getChunkCallback("CompiledDefinition", ReportChunkTypes.Main, out string _);
				Microsoft.ReportingServices.ReportProcessing.Persistence.IntermediateFormatReader intermediateFormatReader = new Microsoft.ReportingServices.ReportProcessing.Persistence.IntermediateFormatReader(stream);
				Report report = intermediateFormatReader.ReadReport(parent);
				definitionObjects = intermediateFormatReader.DefinitionObjects;
				if (report.IntermediateFormatVersion.IsOldVersion)
				{
					Upgrader.UpgradeToCurrent(report);
					Upgrader.UpgradeDatasetIDs(report);
				}
				return report;
			}
			finally
			{
				stream?.Close();
			}
		}

		private static Report DeserializeReportFromSnapshot(GetReportChunk getChunkCallback, out DateTime executionTime)
		{
			Hashtable definitionObjects;
			IntermediateFormatVersion intermediateFormatVersion;
			return DeserializeReportFromSnapshot(getChunkCallback, out executionTime, out definitionObjects, out intermediateFormatVersion);
		}

		internal static Report DeserializeReportFromSnapshot(GetReportChunk getChunkCallback, out DateTime executionTime, out Hashtable definitionObjects)
		{
			IntermediateFormatVersion intermediateFormatVersion;
			return DeserializeReportFromSnapshot(getChunkCallback, out executionTime, out definitionObjects, out intermediateFormatVersion);
		}

		private static Report DeserializeReportFromSnapshot(GetReportChunk getChunkCallback, out DateTime executionTime, out Hashtable definitionObjects, out IntermediateFormatVersion intermediateFormatVersion)
		{
			Stream stream = null;
			try
			{
				stream = getChunkCallback("Main", ReportChunkTypes.Main, out string _);
				Microsoft.ReportingServices.ReportProcessing.Persistence.IntermediateFormatReader intermediateFormatReader = new Microsoft.ReportingServices.ReportProcessing.Persistence.IntermediateFormatReader(stream);
				Report report = intermediateFormatReader.ReadReportFromSnapshot(out executionTime);
				report.MainChunkSize = stream.Length;
				definitionObjects = intermediateFormatReader.DefinitionObjects;
				intermediateFormatVersion = intermediateFormatReader.IntermediateFormatVersion;
				if (report.IntermediateFormatVersion.IsOldVersion)
				{
					Upgrader.UpgradeToCurrent(report);
					Upgrader.UpgradeDatasetIDs(report);
				}
				return report;
			}
			finally
			{
				stream?.Close();
			}
		}

		private static void SerializeReport(Report report, CreateReportChunk createChunkCallback)
		{
			Stream stream = null;
			try
			{
				stream = createChunkCallback("CompiledDefinition", ReportChunkTypes.Main, null);
				new Microsoft.ReportingServices.ReportProcessing.Persistence.IntermediateFormatWriter(stream, writeDeclarations: true).WriteReport(report);
			}
			finally
			{
				stream?.Close();
			}
		}

		private static void SerializeReport(Microsoft.ReportingServices.ReportIntermediateFormat.Report report, IChunkFactory chunkFactory, IConfiguration configuration)
		{
			Stream stream = null;
			try
			{
				stream = chunkFactory.CreateChunk("CompiledDefinition", ReportChunkTypes.CompiledDefinition, null);
				Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.SerializeReport(report, stream, configuration);
			}
			finally
			{
				stream?.Close();
			}
		}

		internal static ReportSnapshot DeserializeReportSnapshot(GetReportChunk getChunkCallback, CreateReportChunk createChunkCallback, IGetResource getResourceCallback, RenderingContext renderingContext, IDataProtection dataProtection, out Hashtable instanceObjects, out Hashtable definitionObjects, out Microsoft.ReportingServices.ReportProcessing.Persistence.IntermediateFormatReader.State declarationsRead, out bool isOldSnapshot)
		{
			Stream stream = null;
			ChunkManager.RenderingChunkManager renderingChunkManager = null;
			isOldSnapshot = false;
			try
			{
				stream = getChunkCallback("Main", ReportChunkTypes.Main, out string _);
				Microsoft.ReportingServices.ReportProcessing.Persistence.IntermediateFormatReader intermediateFormatReader = new Microsoft.ReportingServices.ReportProcessing.Persistence.IntermediateFormatReader(stream);
				ReportSnapshot reportSnapshot = intermediateFormatReader.ReadReportSnapshot();
				reportSnapshot.Report.MainChunkSize = stream.Length;
				instanceObjects = intermediateFormatReader.InstanceObjects;
				declarationsRead = intermediateFormatReader.ReaderState;
				definitionObjects = intermediateFormatReader.DefinitionObjects;
				renderingChunkManager = new ChunkManager.RenderingChunkManager(getChunkCallback, instanceObjects, definitionObjects, declarationsRead, reportSnapshot.Report.IntermediateFormatVersion);
				if (reportSnapshot.Report.IntermediateFormatVersion.IsOldVersion)
				{
					Upgrader.UpgradeToCurrent(reportSnapshot, renderingChunkManager, createChunkCallback);
					Upgrader.UpgradeDatasetIDs(reportSnapshot.Report);
					isOldSnapshot = true;
				}
				Upgrader.UpgradeToPageSectionsChunk(reportSnapshot, renderingContext.ReportContext, renderingChunkManager, createChunkCallback, getResourceCallback, renderingContext, dataProtection);
				return reportSnapshot;
			}
			finally
			{
				stream?.Close();
				renderingChunkManager?.Close();
			}
		}

		internal static int CompareTo(object x, object y, CompareInfo compareInfo, CompareOptions compareOptions)
		{
			bool validComparisonResult;
			return CompareTo(x, y, nullsAsBlanks: false, compareInfo, compareOptions, throwExceptionOnComparisonFailure: true, extendedTypeComparisons: false, out validComparisonResult);
		}

		internal static int CompareTo(object x, object y, bool nullsAsBlanks, CompareInfo compareInfo, CompareOptions compareOptions, bool throwExceptionOnComparisonFailure, bool extendedTypeComparisons, out bool validComparisonResult)
		{
			validComparisonResult = true;
			if (x == null && y == null)
			{
				return 0;
			}
			if (x is string && y is string)
			{
				return compareInfo.Compare((string)x, (string)y, compareOptions);
			}
			if (x is int && y is int)
			{
				return ((int)x).CompareTo(y);
			}
			bool valid;
			DataAggregate.DataTypeCode typeCode = DataAggregate.GetTypeCode(x, throwException: false, out valid);
			bool valid2;
			DataAggregate.DataTypeCode typeCode2 = DataAggregate.GetTypeCode(y, throwException: false, out valid2);
			if (!valid || !valid2)
			{
				Type type = null;
				if (type != null)
				{
					if (throwExceptionOnComparisonFailure)
					{
						throw new ReportProcessingException_SpatialTypeComparisonError(type.ToString());
					}
					validComparisonResult = false;
					return -1;
				}
				return CompareWithIComparable(x, y, throwExceptionOnComparisonFailure, out validComparisonResult);
			}
			if (typeCode == typeCode2)
			{
				IComparable comparable = x as IComparable;
				if (comparable != null)
				{
					return comparable.CompareTo(y);
				}
			}
			DataAggregate.DataTypeCode dataTypeCode = DataTypeUtility.CommonNumericDenominator(typeCode, typeCode2);
			if (dataTypeCode != 0)
			{
				Type numericTypeFromDataTypeCode = DataTypeUtility.GetNumericTypeFromDataTypeCode(dataTypeCode);
				if (DataAggregate.DataTypeCode.Int32 == dataTypeCode)
				{
					int num = (int)Convert.ChangeType(x, numericTypeFromDataTypeCode, CultureInfo.CurrentCulture);
					int value = (int)Convert.ChangeType(y, numericTypeFromDataTypeCode, CultureInfo.CurrentCulture);
					return num.CompareTo(value);
				}
				if (DataAggregate.DataTypeCode.Double == dataTypeCode)
				{
					double num2 = (double)Convert.ChangeType(x, numericTypeFromDataTypeCode, CultureInfo.CurrentCulture);
					double value2 = (double)Convert.ChangeType(y, numericTypeFromDataTypeCode, CultureInfo.CurrentCulture);
					return num2.CompareTo(value2);
				}
				if (DataAggregate.DataTypeCode.Decimal == dataTypeCode)
				{
					decimal num3 = (decimal)Convert.ChangeType(x, numericTypeFromDataTypeCode, CultureInfo.CurrentCulture);
					decimal value3 = (decimal)Convert.ChangeType(y, numericTypeFromDataTypeCode, CultureInfo.CurrentCulture);
					return num3.CompareTo(value3);
				}
				if (DataAggregate.DataTypeCode.UInt32 == dataTypeCode)
				{
					uint num4 = (uint)Convert.ChangeType(x, numericTypeFromDataTypeCode, CultureInfo.CurrentCulture);
					uint value4 = (uint)Convert.ChangeType(y, numericTypeFromDataTypeCode, CultureInfo.CurrentCulture);
					return num4.CompareTo(value4);
				}
				if (DataAggregate.DataTypeCode.Int64 == dataTypeCode)
				{
					long num5 = (long)Convert.ChangeType(x, numericTypeFromDataTypeCode, CultureInfo.CurrentCulture);
					long value5 = (long)Convert.ChangeType(y, numericTypeFromDataTypeCode, CultureInfo.CurrentCulture);
					return num5.CompareTo(value5);
				}
				if (DataAggregate.DataTypeCode.UInt64 == dataTypeCode)
				{
					ulong num6 = (ulong)Convert.ChangeType(x, numericTypeFromDataTypeCode, CultureInfo.CurrentCulture);
					ulong value6 = (ulong)Convert.ChangeType(y, numericTypeFromDataTypeCode, CultureInfo.CurrentCulture);
					return num6.CompareTo(value6);
				}
			}
			if (typeCode == DataAggregate.DataTypeCode.Null && typeCode2 == DataAggregate.DataTypeCode.Null)
			{
				return 0;
			}
			if (typeCode == DataAggregate.DataTypeCode.Null)
			{
				if (nullsAsBlanks && DataTypeUtility.IsNumericLessThanZero(y, typeCode2))
				{
					return 1;
				}
				return -1;
			}
			if (typeCode2 == DataAggregate.DataTypeCode.Null)
			{
				if (nullsAsBlanks && DataTypeUtility.IsNumericLessThanZero(x, typeCode))
				{
					return -1;
				}
				return 1;
			}
			if (extendedTypeComparisons)
			{
				if (typeCode == DataAggregate.DataTypeCode.Int64 && typeCode2 == DataAggregate.DataTypeCode.Double)
				{
					return CompareTo((long)x, (double)y);
				}
				if (typeCode == DataAggregate.DataTypeCode.Double && typeCode2 == DataAggregate.DataTypeCode.Int64)
				{
					return CompareTo((long)y, (double)x) * -1;
				}
				if (typeCode == DataAggregate.DataTypeCode.Decimal && typeCode2 == DataAggregate.DataTypeCode.Double)
				{
					return CompareTo((decimal)x, (double)y);
				}
				if (typeCode == DataAggregate.DataTypeCode.Double && typeCode2 == DataAggregate.DataTypeCode.Decimal)
				{
					return CompareTo((decimal)y, (double)x) * -1;
				}
			}
			return CompareWithIComparable(x, y, throwExceptionOnComparisonFailure, out validComparisonResult);
		}

		private static int CompareTo(long longVal, double doubleVal)
		{
			if (doubleVal > 9.2233720368547758E+18)
			{
				return -1;
			}
			if (doubleVal < -9.2233720368547758E+18)
			{
				return 1;
			}
			if (double.IsNaN(doubleVal))
			{
				if (longVal < 0)
				{
					return -1;
				}
				return 1;
			}
			long num = (long)doubleVal;
			int num2 = longVal.CompareTo(num);
			if (num2 == 0)
			{
				num2 = ((double)num).CompareTo(doubleVal);
			}
			return num2;
		}

		private static int CompareTo(decimal decimalVal, double doubleVal)
		{
			if (doubleVal > 7.9228162514264338E+28)
			{
				return -1;
			}
			if (doubleVal < -7.9228162514264338E+28)
			{
				return 1;
			}
			if (double.IsNaN(doubleVal))
			{
				if (decimalVal < 0m)
				{
					return -1;
				}
				return 1;
			}
			decimal value = (decimal)doubleVal;
			int num = decimalVal.CompareTo(value);
			if (num == 0)
			{
				num = ((double)value).CompareTo(doubleVal);
			}
			return num;
		}

		private static int CompareWithIComparable(object x, object y, bool throwExceptionOnComparisonFailure, out bool validComparisonResult)
		{
			validComparisonResult = true;
			try
			{
				if (x is IComparable)
				{
					return ((IComparable)x).CompareTo(y);
				}
				if (y is IComparable)
				{
					return -((IComparable)y).CompareTo(x);
				}
			}
			catch (Exception e)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(e))
				{
					throw;
				}
				validComparisonResult = false;
				if (throwExceptionOnComparisonFailure)
				{
					throw new ReportProcessingException_ComparisonError(x.GetType().ToString(), y.GetType().ToString());
				}
			}
			return -1;
		}

		internal static int CompareWithInvariantCulture(string x, string y, bool ignoreCase)
		{
			return string.Compare(x, y, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
		}

		private static void GetInteractivePageHeaderFooter(int pageNumber, Microsoft.ReportingServices.ReportRendering.Report report, out Microsoft.ReportingServices.ReportRendering.PageSection pageHeader, out Microsoft.ReportingServices.ReportRendering.PageSection pageFooter)
		{
			if (report == null)
			{
				throw new ArgumentNullException("report");
			}
			pageHeader = null;
			pageFooter = null;
			pageNumber--;
			if (pageNumber < 0 || pageNumber >= report.NumberOfPages)
			{
				return;
			}
			try
			{
				Global.Tracer.Assert(report.RenderingContext != null && report.RenderingContext.ReportSnapshot != null, "Invalid rendering context");
				_ = report.RenderingContext.ReportSnapshot;
				int currentPageNumber;
				Microsoft.ReportingServices.ReportProcessing.Persistence.IntermediateFormatReader pageSectionReader = report.RenderingContext.ChunkManager.GetPageSectionReader(pageNumber, out currentPageNumber);
				if (pageSectionReader != null)
				{
					List<PageSectionInstance> list = pageSectionReader.ReadPageSections(pageNumber, currentPageNumber, report.ReportDef.PageHeader, report.ReportDef.PageFooter);
					report.RenderingContext.ChunkManager.SetPageSectionReaderState(pageSectionReader.ReaderState, pageNumber);
					if (list != null)
					{
						Global.Tracer.Assert(2 == list.Count, "Invalid persisted page section structure");
						pageHeader = GetRenderingPageSection(list[0], report, pageNumber, isHeader: true);
						pageFooter = GetRenderingPageSection(list[1], report, pageNumber, isHeader: false);
					}
				}
			}
			catch (RSException)
			{
				throw;
			}
			catch (Exception innerException)
			{
				throw new ReportProcessingException(innerException, null);
			}
		}

		private static Microsoft.ReportingServices.ReportRendering.PageSection GetRenderingPageSection(PageSectionInstance instance, Microsoft.ReportingServices.ReportRendering.Report report, int pageNumber, bool isHeader)
		{
			Microsoft.ReportingServices.ReportRendering.PageSection result = null;
			if (instance != null)
			{
				string text = pageNumber.ToString(CultureInfo.InvariantCulture) + (isHeader ? "ph" : "pf");
				Microsoft.ReportingServices.ReportRendering.RenderingContext renderingContext = new Microsoft.ReportingServices.ReportRendering.RenderingContext(report.RenderingContext, text);
				result = new Microsoft.ReportingServices.ReportRendering.PageSection(text, isHeader ? report.ReportDef.PageHeader : report.ReportDef.PageFooter, instance, report, renderingContext, pageDef: false);
			}
			return result;
		}

		internal static void EvaluateHeaderFooterExpressions(int pageNumber, int totalPages, Microsoft.ReportingServices.ReportRendering.Report report, PageReportItems pageReportItems, out Microsoft.ReportingServices.ReportRendering.PageSection pageHeader, out Microsoft.ReportingServices.ReportRendering.PageSection pageFooter)
		{
			if (report == null)
			{
				throw new ArgumentNullException("report");
			}
			if (pageReportItems == null)
			{
				GetInteractivePageHeaderFooter(pageNumber, report, out pageHeader, out pageFooter);
				return;
			}
			CultureInfo cultureInfo = null;
			ProcessingErrorContext processingErrorContext = new ProcessingErrorContext();
			try
			{
				cultureInfo = Thread.CurrentThread.CurrentCulture;
				new PageMerge().Process(pageNumber, totalPages, report, pageReportItems, processingErrorContext, out pageHeader, out pageFooter);
			}
			catch (RSException)
			{
				throw;
			}
			catch (Exception innerException)
			{
				throw new ReportProcessingException(innerException, processingErrorContext.Messages);
			}
			finally
			{
				if (cultureInfo != null)
				{
					Thread.CurrentThread.CurrentCulture = cultureInfo;
				}
			}
		}

		internal static void CheckAndConvertDataSources(ICatalogItemContext itemContext, DataSourceInfoCollection dataSources, DataSetInfoCollection dataSetReferences, bool checkIfUsable, ServerDataSourceSettings serverDatasourceSettings, RuntimeDataSourceInfoCollection allDataSources, RuntimeDataSetInfoCollection allDataSetReferences)
		{
			if (dataSetReferences != null)
			{
				foreach (DataSetInfo dataSetReference in dataSetReferences)
				{
					if (checkIfUsable && !dataSetReference.IsValidReference())
					{
						throw new InvalidDataSetReferenceException(dataSetReference.DataSetName.MarkAsPrivate());
					}
					allDataSetReferences.Add(dataSetReference, itemContext);
				}
			}
			if (dataSources == null)
			{
				return;
			}
			foreach (Microsoft.ReportingServices.DataExtensions.DataSourceInfo dataSource in dataSources)
			{
				if (checkIfUsable)
				{
					dataSource.ThrowIfNotUsable(serverDatasourceSettings);
				}
				allDataSources?.Add(dataSource, itemContext);
			}
		}

		private static void TraceError(Exception e)
		{
			if (Global.Tracer.TraceError)
			{
				Global.Tracer.Trace(TraceLevel.Error, "An error has occurred while retrieving datasources for a sub-report. The report definition could not be retrieved. Details: {0}", e.Message);
			}
		}

		private static void CheckCredentialsOdp(Microsoft.ReportingServices.ReportIntermediateFormat.Report report, DataSourceInfoCollection dataSources, DataSetInfoCollection dataSetReferences, ICatalogItemContext reportContext, OnDemandSubReportDataSourcesCallback subReportCallback, RuntimeDataSourceInfoCollection allDataSources, RuntimeDataSetInfoCollection allDataSetReferences, int subReportLevel, bool checkIfUsable, ServerDataSourceSettings serverDatasourceSettings, Hashtable subReportNames)
		{
			if ((long)subReportLevel > 20L)
			{
				return;
			}
			CheckAndConvertDataSources(reportContext, dataSources, dataSetReferences, checkIfUsable, serverDatasourceSettings, allDataSources, allDataSetReferences);
			if (report.SubReports == null)
			{
				return;
			}
			DataSourceInfoCollection dataSources2 = null;
			DataSetInfoCollection dataSetReferences2 = null;
			for (int i = 0; i < report.SubReports.Count; i++)
			{
				string reportName = report.SubReports[i].ReportName;
				if (subReportNames.ContainsKey(reportName))
				{
					continue;
				}
				subReportNames.Add(reportName, null);
				try
				{
					subReportCallback(reportContext, reportName, NeedsUpgradeImpl, out ICatalogItemContext subreportContext, out IChunkFactory getCompiledDefinitionCallback, out dataSources2, out dataSetReferences2);
					if (getCompiledDefinitionCallback != null && ContainsFlag(getCompiledDefinitionCallback.ReportProcessingFlags, ReportProcessingFlags.OnDemandEngine))
					{
						report.SubReports[i].Report = DeserializeKatmaiReport(getCompiledDefinitionCallback);
						CheckCredentialsOdp(report.SubReports[i].Report, dataSources2, dataSetReferences2, subreportContext, subReportCallback, allDataSources, allDataSetReferences, subReportLevel + 1, checkIfUsable, serverDatasourceSettings, subReportNames);
					}
				}
				catch (ReportProcessingException e)
				{
					TraceError(e);
				}
				catch (ReportCatalogException ex)
				{
					TraceError(ex);
					if (ex.Code == ErrorCode.rsReportServerDatabaseUnavailable)
					{
						throw;
					}
				}
			}
		}

		private static void CheckCredentials(Report report, DataSourceInfoCollection dataSources, ICatalogItemContext reportContext, SubReportDataSourcesCallback subReportCallback, RuntimeDataSourceInfoCollection allDataSources, int subReportLevel, bool checkIfUsable, ServerDataSourceSettings serverDatasourceSettings, Hashtable subReportNames)
		{
			if ((long)subReportLevel > 20L)
			{
				return;
			}
			CheckAndConvertDataSources(reportContext, dataSources, null, checkIfUsable, serverDatasourceSettings, allDataSources, null);
			if (report.SubReports == null)
			{
				return;
			}
			DataSourceInfoCollection dataSources2 = null;
			for (int i = 0; i < report.SubReports.Count; i++)
			{
				string reportPath = report.SubReports[i].ReportPath;
				if (subReportNames.ContainsKey(reportPath))
				{
					continue;
				}
				try
				{
					subReportCallback(reportContext, reportPath, out ICatalogItemContext subreportContext, out GetReportChunk getCompiledDefinitionCallback, out dataSources2);
					if (getCompiledDefinitionCallback != null)
					{
						subReportNames.Add(reportPath, null);
						CheckCredentials(DeserializeReport(getCompiledDefinitionCallback), dataSources2, subreportContext, subReportCallback, allDataSources, subReportLevel + 1, checkIfUsable, serverDatasourceSettings, subReportNames);
					}
				}
				catch (VersionMismatchException)
				{
					throw;
				}
				catch (ReportProcessingException e)
				{
					TraceError(e);
				}
				catch (ReportCatalogException ex2)
				{
					TraceError(ex2);
					if (ex2.Code == ErrorCode.rsReportServerDatabaseUnavailable)
					{
						throw;
					}
				}
			}
		}

		public PublishingResult CreateIntermediateFormat(PublishingContext reportPublishingContext)
		{
			if (reportPublishingContext.DataProtection == null)
			{
				throw new ArgumentNullException("reportPublishingContext.DataProtection");
			}
			Global.Tracer.Assert(reportPublishingContext != null && reportPublishingContext.PublishingContextKind != PublishingContextKind.SharedDataSet, "Publishing a report must provide a correct report publishing context");
			PublishingErrorContext publishingErrorContext = new PublishingErrorContext();
			try
			{
				DataSetInfoCollection sharedDataSetReferences = null;
				ArrayList dataSetsName = null;
				byte[] dataSetsHash = null;
				double num = 0.0;
				double num2 = 0.0;
				double num3 = 0.0;
				double num4 = 0.0;
				double num5 = 0.0;
				double num6 = 0.0;
				string reportDescription;
				string reportLanguage;
				ParameterInfoCollection parameters;
				DataSourceInfoCollection dataSources;
				UserLocationFlags userReferenceLocation;
				bool hasExternalImages;
				bool hasHyperlinks;
				try
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.Report report = CompileOdpReport(reportPublishingContext, publishingErrorContext, out reportDescription, out reportLanguage, out parameters, out dataSources, out sharedDataSetReferences, out userReferenceLocation, out dataSetsName, out hasExternalImages, out hasHyperlinks, out dataSetsHash);
					Global.Tracer.Assert(report.ReportSections != null && report.ReportSections.Count > 0, "Report should have at least one section.");
					Microsoft.ReportingServices.ReportIntermediateFormat.ReportSection reportSection = report.ReportSections[0];
					num = reportSection.Page.PageHeightValue;
					num2 = reportSection.Page.PageWidthValue;
					num3 = reportSection.Page.TopMarginValue;
					num4 = reportSection.Page.BottomMarginValue;
					num5 = reportSection.Page.LeftMarginValue;
					num6 = reportSection.Page.RightMarginValue;
					reportPublishingContext.ProcessingFlags = ReportProcessingFlags.OnDemandEngine;
				}
				catch (CRI2005UpgradeException)
				{
					Report report2 = CompileYukonReport(reportPublishingContext.CatalogContext, reportPublishingContext.Definition, reportPublishingContext.CreateChunkFactory.CreateChunk, reportPublishingContext.CheckDataSourceCallback, reportPublishingContext.ResolveTemporaryDataSourceCallback, reportPublishingContext.OriginalDataSources, publishingErrorContext, reportPublishingContext.CompilationTempAppDomain, reportPublishingContext.GenerateExpressionHostWithRefusedPermissions, reportPublishingContext.DataProtection, out reportDescription, out reportLanguage, out parameters, out dataSources, out userReferenceLocation, out dataSetsName, out hasExternalImages, out hasHyperlinks);
					num = report2.InteractiveHeightValue;
					num2 = report2.InteractiveWidthValue;
					num3 = report2.TopMarginValue;
					num4 = report2.BottomMarginValue;
					num5 = report2.LeftMarginValue;
					num6 = report2.RightMarginValue;
					reportPublishingContext.ProcessingFlags = ReportProcessingFlags.YukonEngine;
				}
				return new PublishingResult(reportDescription, reportLanguage, parameters, dataSources, sharedDataSetReferences ?? new DataSetInfoCollection(), publishingErrorContext.Messages, userReferenceLocation, num, num2, num3, num4, num5, num6, dataSetsName, hasExternalImages, hasHyperlinks, reportPublishingContext.ProcessingFlags, dataSetsHash);
			}
			catch (RSException)
			{
				throw;
			}
			catch (Exception innerException)
			{
				ProcessingMessageList processingMessages = (publishingErrorContext != null) ? publishingErrorContext.Messages : new ProcessingMessageList();
				throw new ReportProcessingException(innerException, processingMessages);
			}
		}

		private Microsoft.ReportingServices.ReportIntermediateFormat.Report CompileOdpReport(PublishingContext reportPublishingContext, PublishingErrorContext errorContext, out string reportDescription, out string reportLanguage, out ParameterInfoCollection parameters, out DataSourceInfoCollection dataSources, out DataSetInfoCollection sharedDataSetReferences, out UserLocationFlags userReferenceLocation, out ArrayList dataSetsName, out bool hasExternalImages, out bool hasHyperlinks, out byte[] dataSetsHash)
		{
			ReportUpgradeStrategy reportUpgradeStrategy = new RdlObjectModelUpgradeStrategy(!reportPublishingContext.IsInternalRepublish, !reportPublishingContext.IsRdlSandboxingEnabled);
			Microsoft.ReportingServices.ReportIntermediateFormat.Report report = new Microsoft.ReportingServices.ReportPublishing.ReportPublishing(reportPublishingContext, errorContext, reportUpgradeStrategy).CreateIntermediateFormat(reportPublishingContext.Definition, out reportDescription, out reportLanguage, out parameters, out dataSources, out sharedDataSetReferences, out userReferenceLocation, out dataSetsName, out hasExternalImages, out hasHyperlinks, out dataSetsHash);
			if (reportPublishingContext.CreateChunkFactory != null)
			{
				SerializeReport(report, reportPublishingContext.CreateChunkFactory, m_configuration);
			}
			return report;
		}

		public ProcessingMessageList ProcessDataSetParameters(DataSetContext dc, DataSetDefinition dataSetDefinition)
		{
			if (dc == null)
			{
				throw new ArgumentNullException("dc");
			}
			if (dataSetDefinition == null)
			{
				throw new ArgumentNullException("dataSetDefinition");
			}
			OnDemandProcessingContext onDemandProcessingContext = CreateODPContextForSharedDataSet(dc, dataSetDefinition);
			CultureInfo originalCulture = null;
			try
			{
				if (dc.Culture != null)
				{
					originalCulture = Thread.CurrentThread.CurrentCulture;
					Thread.CurrentThread.CurrentCulture = Localization.ClientPrimaryCulture;
				}
				ProcessDataSetParameters(onDemandProcessingContext, dc, dataSetDefinition);
			}
			finally
			{
				FinallyBlockForSharedDataSetProcessing(onDemandProcessingContext, dc, originalCulture);
			}
			return onDemandProcessingContext.ErrorContext.Messages;
		}

		public DataSetResult ProcessSharedDataSet(DataSetContext dc, DataSetDefinition dataSetDefinition)
		{
			if (dc == null)
			{
				throw new ArgumentNullException("dc");
			}
			if (dataSetDefinition == null)
			{
				throw new ArgumentNullException("dataSetDefinition");
			}
			Global.Tracer.Assert(dataSetDefinition.DataSetCore != null, "Shared dataset definition is missing dataset information");
			OnDemandProcessingContext onDemandProcessingContext = CreateODPContextForSharedDataSet(dc, dataSetDefinition);
			onDemandProcessingContext.SetSharedDataSetUniqueName(onDemandProcessingContext.ExternalDataSetContext.TargetChunkNameInSnapshot ?? dataSetDefinition.DataSetCore.Name);
			onDemandProcessingContext.ExecutionLogContext.StartProcessingTimer();
			if (dc.Parameters != null || dataSetDefinition.DataSetCore.Query != null)
			{
				int num = (dc.Parameters != null) ? dc.Parameters.Count : 0;
				int num2 = (dataSetDefinition.DataSetCore.Query != null && dataSetDefinition.DataSetCore.Query.Parameters != null) ? dataSetDefinition.DataSetCore.Query.Parameters.Count : 0;
				if (num != num2)
				{
					throw new DataSetExecutionException(ErrorCode.rsParameterError);
				}
			}
			if (dc.Parameters != null && !dc.Parameters.ValuesAreValid(out bool _, throwOnUnsatisfiable: true))
			{
				throw new DataSetExecutionException(ErrorCode.rsParametersNotSpecified);
			}
			bool successfulCompletion = false;
			CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
			try
			{
				if (dc.Culture != null && Thread.CurrentThread.CurrentCulture != dc.Culture)
				{
					Thread.CurrentThread.CurrentCulture = dc.Culture;
				}
				successfulCompletion = new RetrievalManager(dataSetDefinition, onDemandProcessingContext).FetchSharedDataSet(dc.Parameters);
			}
			catch (Exception innerException)
			{
				if (innerException is ProcessingAbortedException)
				{
					innerException = innerException.InnerException;
				}
				throw new DataSetExecutionException(((dc.ConsumerRequest == null || dc.ConsumerRequest.ReportDataSetName == null) ? dc.ItemContext.ItemPathAsString : dc.ConsumerRequest.ReportDataSetName).MarkAsPrivate(), innerException);
			}
			finally
			{
				FinallyBlockForSharedDataSetProcessing(onDemandProcessingContext, dc, currentCulture);
			}
			if (dc.DataSources != null && dc.DataSources.HasConnectionStringUseridReference())
			{
				onDemandProcessingContext.MergeHasUserProfileState(UserProfileState.InQuery);
			}
			return new DataSetResult(dc.Parameters, onDemandProcessingContext.ErrorContext.Messages, onDemandProcessingContext.HasUserProfileState, successfulCompletion);
		}

		private static void ProcessDataSetParameters(OnDemandProcessingContext odpContext, DataSetContext dc, DataSetDefinition dataSetDefinition)
		{
			new SharedDataSetProcessReportParameters(dataSetDefinition.DataSetCore, odpContext).Process(dc.Parameters);
		}

		private static void FinallyBlockForSharedDataSetProcessing(OnDemandProcessingContext odpContext, DataSetContext dc, CultureInfo originalCulture)
		{
			if (originalCulture != null && Thread.CurrentThread.CurrentCulture != originalCulture)
			{
				Thread.CurrentThread.CurrentCulture = originalCulture;
			}
			odpContext.UnregisterAbortInfo();
			dc.Parameters.UserProfileState |= odpContext.HasUserProfileState;
			if (odpContext.ExecutionLogContext != null && odpContext.ExecutionLogContext.IsProcessingTimerRunning)
			{
				UpdateHostingEnvironment(null, dc.ItemContext, odpContext.ExecutionLogContext, ProcessingEngine.OnDemandEngine, dc.JobContext, (dc.ConsumerRequest == null) ? "Standalone" : "Inline");
			}
		}

		private OnDemandProcessingContext CreateODPContextForSharedDataSet(DataSetContext dc, DataSetDefinition dataSetDefinition)
		{
			ProcessingErrorContext errorContext = new ProcessingErrorContext();
			return new OnDemandProcessingContext(dc, dataSetDefinition, errorContext, m_configuration);
		}

		public DataSetPublishingResult CreateSharedDataSet(PublishingContext sharedDataSetPublishingContext)
		{
			Global.Tracer.Assert(sharedDataSetPublishingContext != null && sharedDataSetPublishingContext.PublishingContextKind == PublishingContextKind.SharedDataSet, "CreateSharedDataSet must be called with a valid publishing context");
			PublishingErrorContext errorContext = new PublishingErrorContext();
			return new Microsoft.ReportingServices.ReportPublishing.ReportPublishing(sharedDataSetPublishingContext, errorContext).CreateSharedDataSet(sharedDataSetPublishingContext.Definition);
		}
	}
}
