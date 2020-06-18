using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class ReportSnapshot : IPersistable
	{
		private DateTime m_executionTime;

		private Report m_report;

		private bool m_hasDocumentMap;

		private bool? m_definitionHasDocumentMap = false;

		private string m_documentMapRenderFormat;

		private bool m_hasShowHide;

		private bool m_hasBookmarks;

		private string m_requestUserName;

		private string m_reportServerUrl;

		private string m_reportFolder;

		private string m_language;

		private ProcessingMessageList m_processingMessages;

		private Dictionary<string, string> m_cachedDatabaseImages;

		private Dictionary<string, string> m_cachedGeneratedReportItems;

		private ParameterInfoCollection m_parameters;

		private bool m_hasUserSortFilter;

		private Dictionary<string, List<string>> m_aggregateFieldReferences;

		[NonSerialized]
		private bool m_cachedDataChanged;

		[NonSerialized]
		private ReportInstance m_reportInstance;

		[NonSerialized]
		private SortFilterEventInfoMap m_sortFilterEventInfo;

		[NonSerialized]
		private string m_reportName;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal Report Report
		{
			get
			{
				return m_report;
			}
			set
			{
				m_report = value;
			}
		}

		internal bool HasDocumentMap
		{
			get
			{
				return m_hasDocumentMap;
			}
			set
			{
				m_hasDocumentMap = value;
				m_cachedDataChanged = true;
			}
		}

		private bool DocumentMapHasRenderFormatDependency => m_documentMapRenderFormat != null;

		internal bool DefinitionTreeHasDocumentMap
		{
			get
			{
				if (!m_definitionHasDocumentMap.HasValue)
				{
					m_definitionHasDocumentMap = m_report.HasLabels;
				}
				return m_definitionHasDocumentMap.Value;
			}
			set
			{
				m_definitionHasDocumentMap = value;
			}
		}

		internal bool HasBookmarks
		{
			get
			{
				return m_hasBookmarks;
			}
			set
			{
				m_hasBookmarks = value;
			}
		}

		internal bool HasShowHide
		{
			get
			{
				return m_hasShowHide;
			}
			set
			{
				m_hasShowHide = value;
			}
		}

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

		internal string RequestUserName => m_requestUserName;

		internal DateTime ExecutionTime => m_executionTime;

		internal string ReportServerUrl => m_reportServerUrl;

		internal string ReportFolder => m_reportFolder;

		internal string Language => m_language;

		internal ProcessingMessageList Warnings
		{
			get
			{
				return m_processingMessages;
			}
			set
			{
				m_processingMessages = value;
			}
		}

		internal ReportInstance ReportInstance
		{
			get
			{
				return m_reportInstance;
			}
			set
			{
				m_reportInstance = value;
			}
		}

		internal bool CachedDataChanged => m_cachedDataChanged;

		internal ParameterInfoCollection Parameters => m_parameters;

		internal Dictionary<string, List<string>> AggregateFieldReferences
		{
			get
			{
				if (m_aggregateFieldReferences == null)
				{
					m_aggregateFieldReferences = new Dictionary<string, List<string>>();
				}
				return m_aggregateFieldReferences;
			}
		}

		internal SortFilterEventInfoMap SortFilterEventInfo
		{
			get
			{
				return m_sortFilterEventInfo;
			}
			set
			{
				m_sortFilterEventInfo = value;
			}
		}

		internal ReportSnapshot(Report report, string reportName, ParameterInfoCollection parameters, string requestUserName, DateTime executionTime, string reportServerUrl, string reportFolder, string language)
		{
			m_report = report;
			m_reportName = reportName;
			m_parameters = parameters;
			m_requestUserName = requestUserName;
			m_executionTime = executionTime;
			m_reportServerUrl = reportServerUrl;
			m_reportFolder = reportFolder;
			m_language = language;
			m_hasDocumentMap = report.HasLabels;
			m_definitionHasDocumentMap = report.HasLabels;
			m_hasBookmarks = report.HasBookmarks;
			m_cachedDataChanged = true;
		}

		internal ReportSnapshot()
		{
			m_executionTime = DateTime.Now;
			m_cachedDataChanged = false;
		}

		internal bool CanUseExistingDocumentMapChunk(ICatalogItemContext reportContext)
		{
			if (!HasDocumentMap)
			{
				return false;
			}
			if (!DocumentMapHasRenderFormatDependency)
			{
				return true;
			}
			return RenderFormatImpl.IsRenderFormat(NormalizeRenderFormatForDocumentMap(reportContext), m_documentMapRenderFormat);
		}

		private static string NormalizeRenderFormatForDocumentMap(ICatalogItemContext reportContext)
		{
			bool isInteractiveFormat;
			string result = RenderFormatImpl.NormalizeRenderFormat(reportContext, out isInteractiveFormat);
			if (isInteractiveFormat)
			{
				result = "RPL";
			}
			return result;
		}

		internal void SetRenderFormatDependencyInDocumentMap(OnDemandProcessingContext odpContext)
		{
			m_cachedDataChanged = true;
			m_documentMapRenderFormat = NormalizeRenderFormatForDocumentMap(odpContext.TopLevelContext.ReportContext);
		}

		internal void AddImageChunkName(string definitionKey, string name)
		{
			m_cachedDataChanged = true;
			if (m_cachedDatabaseImages == null)
			{
				m_cachedDatabaseImages = new Dictionary<string, string>(EqualityComparers.StringComparerInstance);
			}
			m_cachedDatabaseImages.Add(definitionKey, name);
		}

		internal bool TryGetImageChunkName(string definitionKey, out string name)
		{
			if (m_cachedDatabaseImages != null)
			{
				return m_cachedDatabaseImages.TryGetValue(definitionKey, out name);
			}
			name = null;
			return false;
		}

		internal void AddGeneratedReportItemChunkName(string definitionKey, string name)
		{
			m_cachedDataChanged = true;
			if (m_cachedGeneratedReportItems == null)
			{
				m_cachedGeneratedReportItems = new Dictionary<string, string>(EqualityComparers.StringComparerInstance);
			}
			m_cachedGeneratedReportItems.Add(definitionKey, name);
		}

		internal bool TryGetGeneratedReportItemChunkName(string definitionKey, out string name)
		{
			if (m_cachedGeneratedReportItems != null)
			{
				return m_cachedGeneratedReportItems.TryGetValue(definitionKey, out name);
			}
			name = null;
			return false;
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.ExecutionTime:
					writer.Write(m_executionTime);
					break;
				case MemberName.Report:
					Global.Tracer.Assert(m_report != null);
					writer.WriteReference(m_report);
					break;
				case MemberName.HasDocumentMap:
					writer.Write(m_hasDocumentMap);
					break;
				case MemberName.HasShowHide:
					writer.Write(m_hasShowHide);
					break;
				case MemberName.HasBookmarks:
					writer.Write(m_hasBookmarks);
					break;
				case MemberName.RequestUserName:
					writer.Write(m_requestUserName);
					break;
				case MemberName.ReportServerUrl:
					writer.Write(m_reportServerUrl);
					break;
				case MemberName.ReportFolder:
					writer.Write(m_reportFolder);
					break;
				case MemberName.Language:
					writer.Write(m_language);
					break;
				case MemberName.ProcessingMessages:
					writer.Write(m_processingMessages);
					break;
				case MemberName.Parameters:
					writer.Write((ArrayList)null);
					break;
				case MemberName.ImageChunkNames:
					writer.WriteStringStringHashtable(m_cachedDatabaseImages);
					break;
				case MemberName.GeneratedReportItemChunkNames:
					writer.WriteStringStringHashtable(m_cachedGeneratedReportItems);
					break;
				case MemberName.HasUserSortFilter:
					writer.Write(m_hasUserSortFilter);
					break;
				case MemberName.AggregateFieldReferences:
					writer.WriteStringListOfStringDictionary(m_aggregateFieldReferences);
					break;
				case MemberName.SnapshotParameters:
					writer.Write((IPersistable)m_parameters);
					break;
				case MemberName.DefinitionHasDocumentMap:
					writer.Write(m_definitionHasDocumentMap);
					break;
				case MemberName.DocumentMapRenderFormat:
					writer.Write(m_documentMapRenderFormat);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			ParameterInfoCollection parameterInfoCollection = null;
			ParameterInfoCollection parameterInfoCollection2 = null;
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.ExecutionTime:
					m_executionTime = reader.ReadDateTime();
					break;
				case MemberName.Report:
					m_report = reader.ReadReference<Report>(this);
					break;
				case MemberName.HasDocumentMap:
					m_hasDocumentMap = reader.ReadBoolean();
					break;
				case MemberName.HasShowHide:
					m_hasShowHide = reader.ReadBoolean();
					break;
				case MemberName.HasBookmarks:
					m_hasBookmarks = reader.ReadBoolean();
					break;
				case MemberName.RequestUserName:
					m_requestUserName = reader.ReadString();
					break;
				case MemberName.ReportServerUrl:
					m_reportServerUrl = reader.ReadString();
					break;
				case MemberName.ReportFolder:
					m_reportFolder = reader.ReadString();
					break;
				case MemberName.Language:
					m_language = reader.ReadString();
					break;
				case MemberName.ProcessingMessages:
					m_processingMessages = reader.ReadListOfRIFObjects<ProcessingMessageList>();
					break;
				case MemberName.Parameters:
					parameterInfoCollection = new ParameterInfoCollection();
					reader.ReadListOfRIFObjects(parameterInfoCollection);
					break;
				case MemberName.ImageChunkNames:
					m_cachedDatabaseImages = reader.ReadStringStringHashtable<Dictionary<string, string>>();
					break;
				case MemberName.GeneratedReportItemChunkNames:
					m_cachedGeneratedReportItems = reader.ReadStringStringHashtable<Dictionary<string, string>>();
					break;
				case MemberName.HasUserSortFilter:
					m_hasUserSortFilter = reader.ReadBoolean();
					break;
				case MemberName.AggregateFieldReferences:
					m_aggregateFieldReferences = reader.ReadStringListOfStringDictionary();
					break;
				case MemberName.SnapshotParameters:
					parameterInfoCollection2 = (ParameterInfoCollection)reader.ReadRIFObject();
					break;
				case MemberName.DefinitionHasDocumentMap:
				{
					object obj = reader.ReadVariant();
					if (obj != null)
					{
						m_definitionHasDocumentMap = (bool)obj;
					}
					break;
				}
				case MemberName.DocumentMapRenderFormat:
					m_documentMapRenderFormat = reader.ReadString();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
			m_parameters = parameterInfoCollection;
			if ((parameterInfoCollection == null || parameterInfoCollection.Count == 0) && parameterInfoCollection2 != null)
			{
				m_parameters = parameterInfoCollection2;
			}
		}

		public void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			if (!memberReferencesCollection.TryGetValue(m_Declaration.ObjectType, out List<MemberReference> value))
			{
				return;
			}
			foreach (MemberReference item in value)
			{
				MemberName memberName = item.MemberName;
				if (memberName == MemberName.Report)
				{
					Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
					m_report = (Report)referenceableItems[item.RefID];
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportSnapshot;
		}

		[SkipMemberStaticValidation(MemberName.Parameters)]
		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.ExecutionTime, Token.DateTime));
			list.Add(new MemberInfo(MemberName.Report, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Report, Token.Reference));
			list.Add(new MemberInfo(MemberName.HasDocumentMap, Token.Boolean));
			list.Add(new MemberInfo(MemberName.HasShowHide, Token.Boolean));
			list.Add(new MemberInfo(MemberName.HasBookmarks, Token.Boolean));
			list.Add(new MemberInfo(MemberName.RequestUserName, Token.String));
			list.Add(new MemberInfo(MemberName.ReportServerUrl, Token.String));
			list.Add(new MemberInfo(MemberName.ReportFolder, Token.String));
			list.Add(new MemberInfo(MemberName.Language, Token.String));
			list.Add(new MemberInfo(MemberName.Parameters, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterInfo));
			list.Add(new MemberInfo(MemberName.ImageChunkNames, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StringStringHashtable));
			list.Add(new MemberInfo(MemberName.HasUserSortFilter, Token.Boolean));
			list.Add(new MemberInfo(MemberName.GeneratedReportItemChunkNames, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StringStringHashtable));
			list.Add(new MemberInfo(MemberName.AggregateFieldReferences, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StringListOfStringDictionary, Token.String, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList));
			list.Add(new MemberInfo(MemberName.SnapshotParameters, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterInfoCollection));
			list.Add(new MemberInfo(MemberName.ProcessingMessages, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ProcessingMessage));
			list.Add(new MemberInfo(MemberName.DefinitionHasDocumentMap, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Nullable, Token.Boolean));
			list.Add(new MemberInfo(MemberName.DocumentMapRenderFormat, Token.String));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportSnapshot, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}
	}
}
