using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal class CommonSubReportInfo : IPersistable
	{
		private string m_description;

		private string m_reportPath;

		private string m_originalCatalogPath;

		private ParameterInfoCollection m_parametersFromCatalog;

		private bool m_retrievalFailed;

		private string m_definitionUniqueName;

		[NonSerialized]
		private IChunkFactory m_definitionChunkFactory;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal string ReportPath
		{
			get
			{
				return m_reportPath;
			}
			set
			{
				m_reportPath = value;
			}
		}

		internal string OriginalCatalogPath
		{
			get
			{
				return m_originalCatalogPath;
			}
			set
			{
				m_originalCatalogPath = value;
			}
		}

		internal string Description
		{
			get
			{
				return m_description;
			}
			set
			{
				m_description = value;
			}
		}

		internal ParameterInfoCollection ParametersFromCatalog
		{
			get
			{
				return m_parametersFromCatalog;
			}
			set
			{
				m_parametersFromCatalog = value;
			}
		}

		internal bool RetrievalFailed
		{
			get
			{
				return m_retrievalFailed;
			}
			set
			{
				m_retrievalFailed = value;
			}
		}

		internal string DefinitionUniqueName
		{
			get
			{
				return m_definitionUniqueName;
			}
			set
			{
				m_definitionUniqueName = value;
			}
		}

		internal IChunkFactory DefinitionChunkFactory
		{
			get
			{
				return m_definitionChunkFactory;
			}
			set
			{
				m_definitionChunkFactory = value;
			}
		}

		internal CommonSubReportInfo()
		{
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.ReportPath, Token.String));
			list.Add(new MemberInfo(MemberName.ParametersFromCatalog, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterInfo));
			list.Add(new MemberInfo(MemberName.RetrievalFailed, Token.Boolean));
			list.Add(new MemberInfo(MemberName.Description, Token.String));
			list.Add(new MemberInfo(MemberName.DefinitionUniqueName, Token.String));
			list.Add(new MemberInfo(MemberName.OriginalCatalogPath, Token.String));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.CommonSubReportInfo, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.DefinitionUniqueName:
					writer.Write(m_definitionUniqueName);
					break;
				case MemberName.ReportPath:
					writer.Write(m_reportPath);
					break;
				case MemberName.ParametersFromCatalog:
					writer.Write((ArrayList)m_parametersFromCatalog);
					break;
				case MemberName.RetrievalFailed:
					writer.Write(m_retrievalFailed);
					break;
				case MemberName.Description:
					writer.Write(m_description);
					break;
				case MemberName.OriginalCatalogPath:
					writer.Write(m_originalCatalogPath);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.DefinitionUniqueName:
					m_definitionUniqueName = reader.ReadString();
					break;
				case MemberName.ReportPath:
					m_reportPath = reader.ReadString();
					break;
				case MemberName.ParametersFromCatalog:
					m_parametersFromCatalog = reader.ReadListOfRIFObjects<ParameterInfoCollection>();
					break;
				case MemberName.RetrievalFailed:
					m_retrievalFailed = reader.ReadBoolean();
					break;
				case MemberName.Description:
					m_description = reader.ReadString();
					break;
				case MemberName.OriginalCatalogPath:
					m_originalCatalogPath = reader.ReadString();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
			if (m_originalCatalogPath == null)
			{
				m_originalCatalogPath = m_reportPath;
			}
		}

		public void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false);
		}

		public Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.CommonSubReportInfo;
		}
	}
}
