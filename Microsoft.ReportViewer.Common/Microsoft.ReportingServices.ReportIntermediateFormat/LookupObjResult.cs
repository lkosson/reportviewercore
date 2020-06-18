using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal class LookupObjResult : IStorable, IPersistable, IErrorContext
	{
		private ProcessingErrorCode m_errorCode;

		private bool m_hasErrorCode;

		private Severity m_errorSeverity;

		private string[] m_errorMessageArgs;

		private DataFieldStatus m_dataFieldStatus;

		private ReferenceID m_lookupTablePartitionId = TreePartitionManager.EmptyTreePartitionID;

		[NonSerialized]
		private LookupTable m_lookupTable;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal bool ErrorOccured
		{
			get
			{
				if (!m_hasErrorCode)
				{
					return m_dataFieldStatus != DataFieldStatus.None;
				}
				return true;
			}
		}

		internal DataFieldStatus DataFieldStatus
		{
			get
			{
				return m_dataFieldStatus;
			}
			set
			{
				m_dataFieldStatus = value;
			}
		}

		internal bool HasErrorCode => m_hasErrorCode;

		internal ProcessingErrorCode ErrorCode => m_errorCode;

		internal Severity ErrorSeverity => m_errorSeverity;

		internal string[] ErrorMessageArgs => m_errorMessageArgs;

		internal bool HasBeenTransferred => m_lookupTablePartitionId != TreePartitionManager.EmptyTreePartitionID;

		public int Size => 1 + ItemSizes.SizeOf(m_lookupTable);

		internal LookupObjResult()
		{
		}

		internal LookupObjResult(LookupTable lookupTable)
		{
			m_lookupTable = lookupTable;
		}

		internal LookupTable GetLookupTable(OnDemandProcessingContext odpContext)
		{
			if (m_lookupTable == null)
			{
				Global.Tracer.Assert(HasBeenTransferred, "Invalid LookupObjResult: PartitionID for LookupTable is empty.");
				OnDemandMetadata odpMetadata = odpContext.OdpMetadata;
				odpMetadata.EnsureLookupScalabilitySetup(odpContext.ChunkFactory, odpContext.GetActiveCompatibilityVersion(), odpContext.ProhibitSerializableValues);
				long treePartitionOffset = odpMetadata.LookupPartitionManager.GetTreePartitionOffset(m_lookupTablePartitionId);
				LookupScalabilityCache lookupScalabilityCache = odpMetadata.LookupScalabilityCache;
				m_lookupTable = (LookupTable)lookupScalabilityCache.Storage.Retrieve(treePartitionOffset);
				m_lookupTable.SetEqualityComparer(odpContext.ProcessingComparer);
			}
			return m_lookupTable;
		}

		internal void TransferToLookupCache(OnDemandProcessingContext odpContext)
		{
			Global.Tracer.Assert(m_lookupTable != null, "Can't transfer a missing LookupTable");
			Global.Tracer.Assert(!HasBeenTransferred, "Can't transfer a LookupTable twice");
			OnDemandMetadata odpMetadata = odpContext.OdpMetadata;
			odpMetadata.EnsureLookupScalabilitySetup(odpContext.ChunkFactory, odpContext.GetActiveCompatibilityVersion(), odpContext.ProhibitSerializableValues);
			LookupScalabilityCache lookupScalabilityCache = odpMetadata.LookupScalabilityCache;
			IReference<LookupTable> reference = lookupScalabilityCache.AllocateEmptyTreePartition<LookupTable>(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LookupTableReference);
			m_lookupTable.TransferTo(lookupScalabilityCache);
			lookupScalabilityCache.SetTreePartitionContentsAndPin(reference, m_lookupTable);
			m_lookupTablePartitionId = reference.Id;
			reference.UnPinValue();
		}

		void IErrorContext.Register(ProcessingErrorCode code, Severity severity, params string[] arguments)
		{
			if (!m_hasErrorCode)
			{
				m_hasErrorCode = true;
				m_errorCode = code;
				m_errorSeverity = severity;
				m_errorMessageArgs = arguments;
			}
		}

		void IErrorContext.Register(ProcessingErrorCode code, Severity severity, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName, params string[] arguments)
		{
			((IErrorContext)this).Register(code, severity, arguments);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.LookupTablePartitionID:
					writer.Write(m_lookupTablePartitionId.Value);
					break;
				case MemberName.HasCode:
					writer.Write(m_hasErrorCode);
					break;
				case MemberName.Code:
					writer.WriteEnum((int)m_errorCode);
					break;
				case MemberName.Severity:
					writer.WriteEnum((int)m_errorSeverity);
					break;
				case MemberName.FieldStatus:
					writer.WriteEnum((int)m_dataFieldStatus);
					break;
				case MemberName.Arguments:
					writer.Write(m_errorMessageArgs);
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
				case MemberName.LookupTablePartitionID:
					m_lookupTablePartitionId = new ReferenceID(reader.ReadInt64());
					break;
				case MemberName.HasCode:
					m_hasErrorCode = reader.ReadBoolean();
					break;
				case MemberName.Code:
					m_errorCode = (ProcessingErrorCode)reader.ReadEnum();
					break;
				case MemberName.Severity:
					m_errorSeverity = (Severity)reader.ReadEnum();
					break;
				case MemberName.FieldStatus:
					m_dataFieldStatus = (DataFieldStatus)reader.ReadEnum();
					break;
				case MemberName.Arguments:
					m_errorMessageArgs = reader.ReadStringArray();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LookupObjResult;
		}

		public static Declaration GetDeclaration()
		{
			if (m_Declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.LookupTablePartitionID, Token.Int64));
				list.Add(new MemberInfo(MemberName.HasCode, Token.Boolean));
				list.Add(new MemberInfo(MemberName.Code, Token.Enum));
				list.Add(new MemberInfo(MemberName.Severity, Token.Enum));
				list.Add(new MemberInfo(MemberName.FieldStatus, Token.Enum));
				list.Add(new MemberInfo(MemberName.Arguments, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveArray, Token.String));
				return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LookupInfo, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return m_Declaration;
		}
	}
}
