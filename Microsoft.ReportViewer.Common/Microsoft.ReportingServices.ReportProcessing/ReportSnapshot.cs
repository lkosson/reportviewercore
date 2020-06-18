using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ReportSnapshot
	{
		private DateTime m_executionTime;

		private Report m_report;

		private ParameterInfoCollection m_parameters;

		private ReportInstance m_reportInstance;

		private bool m_hasDocumentMap;

		private bool m_hasShowHide;

		private bool m_hasBookmarks;

		private bool m_hasImageStreams;

		private string m_requestUserName;

		private string m_reportServerUrl;

		private string m_reportFolder;

		private string m_language;

		private ProcessingMessageList m_processingMessages;

		private Int64List m_pageSectionOffsets;

		[NonSerialized]
		private InfoBase m_documentMap;

		[NonSerialized]
		private InfoBase m_showHideSenderInfo;

		[NonSerialized]
		private InfoBase m_showHideReceiverInfo;

		[NonSerialized]
		private InfoBase m_quickFind;

		[NonSerialized]
		private BookmarksHashtable m_bookmarksInfo;

		[NonSerialized]
		private ReportDrillthroughInfo m_drillthroughInfo;

		[NonSerialized]
		private InfoBase m_sortFilterEventInfo;

		[NonSerialized]
		private List<PageSectionInstance> m_pageSections;

		[NonSerialized]
		private string m_reportName;

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

		internal ParameterInfoCollection Parameters
		{
			get
			{
				return m_parameters;
			}
			set
			{
				m_parameters = value;
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

		internal bool HasDocumentMap
		{
			get
			{
				return m_hasDocumentMap;
			}
			set
			{
				m_hasDocumentMap = value;
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

		internal string RequestUserName
		{
			get
			{
				return m_requestUserName;
			}
			set
			{
				m_requestUserName = value;
			}
		}

		internal DateTime ExecutionTime
		{
			get
			{
				return m_executionTime;
			}
			set
			{
				m_executionTime = value;
			}
		}

		internal string ReportServerUrl
		{
			get
			{
				return m_reportServerUrl;
			}
			set
			{
				m_reportServerUrl = value;
			}
		}

		internal string ReportFolder
		{
			get
			{
				return m_reportFolder;
			}
			set
			{
				m_reportFolder = value;
			}
		}

		internal string Language
		{
			get
			{
				return m_language;
			}
			set
			{
				m_language = value;
			}
		}

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

		internal Int64List PageSectionOffsets
		{
			get
			{
				return m_pageSectionOffsets;
			}
			set
			{
				m_pageSectionOffsets = value;
			}
		}

		internal List<PageSectionInstance> PageSections
		{
			get
			{
				return m_pageSections;
			}
			set
			{
				m_pageSections = value;
			}
		}

		internal OffsetInfo DocumentMapOffset
		{
			set
			{
				m_documentMap = value;
			}
		}

		internal OffsetInfo ShowHideSenderInfoOffset
		{
			set
			{
				m_showHideSenderInfo = value;
			}
		}

		internal OffsetInfo ShowHideReceiverInfoOffset
		{
			set
			{
				m_showHideReceiverInfo = value;
			}
		}

		internal OffsetInfo QuickFindOffset
		{
			set
			{
				m_quickFind = value;
			}
		}

		internal DocumentMapNode DocumentMap
		{
			get
			{
				if (m_documentMap == null)
				{
					return null;
				}
				if (m_documentMap is DocumentMapNode)
				{
					return (DocumentMapNode)m_documentMap;
				}
				Global.Tracer.Assert(condition: false, string.Empty);
				return null;
			}
			set
			{
				m_documentMap = value;
			}
		}

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

		internal SenderInformationHashtable ShowHideSenderInfo
		{
			get
			{
				if (m_showHideSenderInfo == null)
				{
					return null;
				}
				if (m_showHideSenderInfo is SenderInformationHashtable)
				{
					return (SenderInformationHashtable)m_showHideSenderInfo;
				}
				Global.Tracer.Assert(condition: false, string.Empty);
				return null;
			}
			set
			{
				m_showHideSenderInfo = value;
			}
		}

		internal ReceiverInformationHashtable ShowHideReceiverInfo
		{
			get
			{
				if (m_showHideReceiverInfo == null)
				{
					return null;
				}
				if (m_showHideReceiverInfo is ReceiverInformationHashtable)
				{
					return (ReceiverInformationHashtable)m_showHideReceiverInfo;
				}
				Global.Tracer.Assert(condition: false, string.Empty);
				return null;
			}
			set
			{
				m_showHideReceiverInfo = value;
			}
		}

		internal QuickFindHashtable QuickFind
		{
			get
			{
				if (m_quickFind == null)
				{
					return null;
				}
				if (m_quickFind is QuickFindHashtable)
				{
					return (QuickFindHashtable)m_quickFind;
				}
				Global.Tracer.Assert(condition: false, string.Empty);
				return null;
			}
			set
			{
				m_quickFind = value;
			}
		}

		internal SortFilterEventInfoHashtable SortFilterEventInfo
		{
			get
			{
				if (m_sortFilterEventInfo == null)
				{
					return null;
				}
				if (m_sortFilterEventInfo is SortFilterEventInfoHashtable)
				{
					return (SortFilterEventInfoHashtable)m_sortFilterEventInfo;
				}
				Global.Tracer.Assert(condition: false, string.Empty);
				return null;
			}
			set
			{
				m_sortFilterEventInfo = value;
			}
		}

		internal OffsetInfo SortFilterEventInfoOffset
		{
			get
			{
				if (m_sortFilterEventInfo == null)
				{
					return null;
				}
				Global.Tracer.Assert(m_sortFilterEventInfo is OffsetInfo);
				return (OffsetInfo)m_sortFilterEventInfo;
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
		}

		internal ReportSnapshot()
		{
			m_executionTime = DateTime.Now;
		}

		internal void CreateNavigationActions(ReportProcessing.NavigationInfo navigationInfo)
		{
			if (m_reportInstance != null)
			{
				if (navigationInfo.DocumentMapChildren != null && 0 < navigationInfo.DocumentMapChildren.Count && navigationInfo.DocumentMapChildren[0] != null)
				{
					m_documentMap = new DocumentMapNode(m_reportInstance.UniqueName.ToString(CultureInfo.InvariantCulture), m_reportName, 0, navigationInfo.DocumentMapChildren[0]);
					m_hasDocumentMap = true;
				}
				if (navigationInfo.BookmarksInfo != null)
				{
					m_bookmarksInfo = navigationInfo.BookmarksInfo;
					m_hasBookmarks = true;
				}
			}
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.ExecutionTime, Token.DateTime));
			memberInfoList.Add(new MemberInfo(MemberName.Report, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.Report));
			memberInfoList.Add(new MemberInfo(MemberName.Parameters, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ParameterInfoCollection));
			memberInfoList.Add(new MemberInfo(MemberName.ReportInstance, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportInstance));
			memberInfoList.Add(new MemberInfo(MemberName.HasDocumentMap, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.HasShowHide, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.HasBookmarks, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.HasImageStreams, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.RequestUserName, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.ReportServerUrl, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.ReportFolder, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.Language, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.ProcessingMessages, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ProcessingMessageList));
			memberInfoList.Add(new MemberInfo(MemberName.PageSectionOffsets, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.Int64List));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}

		internal DocumentMapNode GetDocumentMap(ChunkManager.RenderingChunkManager chunkManager)
		{
			IntermediateFormatReader intermediateFormatReader = null;
			if (m_documentMap != null)
			{
				if (!(m_documentMap is OffsetInfo))
				{
					return (DocumentMapNode)m_documentMap;
				}
				intermediateFormatReader = chunkManager.GetReaderForSpecialChunk(((OffsetInfo)m_documentMap).Offset);
			}
			else if (m_hasDocumentMap)
			{
				intermediateFormatReader = chunkManager.GetSpecialChunkReader(ChunkManager.SpecialChunkName.DocumentMap);
			}
			return intermediateFormatReader?.ReadDocumentMapNode();
		}

		private void GetShowHideInfo(ChunkManager.RenderingChunkManager chunkManager)
		{
			if (m_showHideSenderInfo == null && m_showHideReceiverInfo == null)
			{
				IntermediateFormatReader specialChunkReader = chunkManager.GetSpecialChunkReader(ChunkManager.SpecialChunkName.ShowHideInfo);
				if (specialChunkReader != null)
				{
					m_showHideSenderInfo = specialChunkReader.ReadSenderInformationHashtable();
					m_showHideReceiverInfo = specialChunkReader.ReadReceiverInformationHashtable();
				}
			}
		}

		internal SenderInformationHashtable GetShowHideSenderInfo(ChunkManager.RenderingChunkManager chunkManager)
		{
			if (m_showHideSenderInfo == null)
			{
				GetShowHideInfo(chunkManager);
			}
			else if (m_showHideSenderInfo is OffsetInfo)
			{
				IntermediateFormatReader readerForSpecialChunk = chunkManager.GetReaderForSpecialChunk(((OffsetInfo)m_showHideSenderInfo).Offset);
				m_showHideSenderInfo = readerForSpecialChunk.ReadSenderInformationHashtable();
			}
			return (SenderInformationHashtable)m_showHideSenderInfo;
		}

		internal ReceiverInformationHashtable GetShowHideReceiverInfo(ChunkManager.RenderingChunkManager chunkManager)
		{
			if (m_showHideReceiverInfo == null)
			{
				GetShowHideInfo(chunkManager);
			}
			else if (m_showHideReceiverInfo is OffsetInfo)
			{
				IntermediateFormatReader readerForSpecialChunk = chunkManager.GetReaderForSpecialChunk(((OffsetInfo)m_showHideReceiverInfo).Offset);
				m_showHideReceiverInfo = readerForSpecialChunk.ReadReceiverInformationHashtable();
			}
			return (ReceiverInformationHashtable)m_showHideReceiverInfo;
		}

		internal QuickFindHashtable GetQuickFind(ChunkManager.RenderingChunkManager chunkManager)
		{
			IntermediateFormatReader intermediateFormatReader = null;
			if (m_quickFind != null)
			{
				if (!(m_quickFind is OffsetInfo))
				{
					return (QuickFindHashtable)m_quickFind;
				}
				intermediateFormatReader = chunkManager.GetReaderForSpecialChunk(((OffsetInfo)m_quickFind).Offset);
			}
			else
			{
				intermediateFormatReader = chunkManager.GetSpecialChunkReader(ChunkManager.SpecialChunkName.QuickFind);
			}
			return intermediateFormatReader?.ReadQuickFindHashtable();
		}

		internal BookmarksHashtable GetBookmarksInfo(ChunkManager.RenderingChunkManager chunkManager)
		{
			if (m_bookmarksInfo != null)
			{
				return m_bookmarksInfo;
			}
			return chunkManager.GetSpecialChunkReader(ChunkManager.SpecialChunkName.Bookmark)?.ReadBookmarksHashtable();
		}

		internal SortFilterEventInfoHashtable GetSortFilterEventInfo(ChunkManager.RenderingChunkManager chunkManager)
		{
			IntermediateFormatReader intermediateFormatReader = null;
			if (m_sortFilterEventInfo != null)
			{
				return (SortFilterEventInfoHashtable)m_sortFilterEventInfo;
			}
			return chunkManager.GetSpecialChunkReader(ChunkManager.SpecialChunkName.SortFilterEventInfo)?.ReadSortFilterEventInfoHashtable();
		}

		internal List<PageSectionInstance> GetPageSections(int pageNumber, ChunkManager.RenderingChunkManager chunkManager, PageSection headerDef, PageSection footerDef)
		{
			List<PageSectionInstance> result = null;
			int currentPageNumber;
			IntermediateFormatReader pageSectionReader = chunkManager.GetPageSectionReader(pageNumber, out currentPageNumber);
			if (pageSectionReader != null)
			{
				result = pageSectionReader.ReadPageSections(pageNumber, currentPageNumber, headerDef, footerDef);
				chunkManager.SetPageSectionReaderState(pageSectionReader.ReaderState, pageNumber);
			}
			return result;
		}
	}
}
