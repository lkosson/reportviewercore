using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal class SubReportInstance : ScopeInstance, IReportInstanceContainer
	{
		private ParametersImpl m_parameters;

		private IReference<ReportInstance> m_reportInstance;

		private string m_instanceUniqueName;

		private CultureInfo m_threadCulture;

		private SubReport.Status m_status;

		private bool m_processedWithError;

		private SubReport m_subReportDef;

		private bool? m_isInstanceShared;

		private int? m_dataChunkNameModifier;

		[NonSerialized]
		private bool m_initialized;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType ObjectType => Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SubReportInstance;

		internal SubReport SubReportDef => m_subReportDef;

		internal bool Initialized
		{
			get
			{
				return m_initialized;
			}
			set
			{
				m_initialized = value;
			}
		}

		internal ParametersImpl Parameters
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

		internal bool NoRows
		{
			get
			{
				if (m_reportInstance != null)
				{
					return m_reportInstance.Value().NoRows;
				}
				return false;
			}
		}

		public IReference<ReportInstance> ReportInstance => m_reportInstance;

		internal string InstanceUniqueName
		{
			get
			{
				return m_instanceUniqueName;
			}
			set
			{
				m_instanceUniqueName = value;
			}
		}

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

		internal SubReport.Status RetrievalStatus
		{
			get
			{
				return m_status;
			}
			set
			{
				m_status = value;
			}
		}

		internal bool ProcessedWithError
		{
			get
			{
				return m_processedWithError;
			}
			set
			{
				m_processedWithError = value;
			}
		}

		public override int Size => base.Size + ItemSizes.SizeOf(m_parameters) + ItemSizes.SizeOf(m_reportInstance) + ItemSizes.SizeOf(m_instanceUniqueName) + ItemSizes.ReferenceSize + 4 + 1 + ItemSizes.ReferenceSize + 1 + ItemSizes.SizeOf(m_dataChunkNameModifier) + ItemSizes.NullableInt32Size + ItemSizes.NullableBoolSize;

		internal SubReportInstance()
		{
		}

		private SubReportInstance(SubReport subreport, OnDemandMetadata odpMetadata)
		{
			m_subReportDef = subreport;
			m_reportInstance = odpMetadata.GroupTreeScalabilityCache.AllocateEmptyTreePartition<ReportInstance>(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportInstanceReference);
		}

		public IReference<ReportInstance> SetReportInstance(ReportInstance reportInstance, OnDemandMetadata odpMetadata)
		{
			odpMetadata.GroupTreeScalabilityCache.SetTreePartitionContentsAndPin(m_reportInstance, reportInstance);
			return m_reportInstance;
		}

		internal override void AddChildScope(IReference<ScopeInstance> child, int indexInCollection)
		{
			Global.Tracer.Assert(condition: false);
		}

		internal string GetChunkNameModifier(SubReportInfo subReportInfo, bool useCachedValue, bool addEntry, out bool isShared)
		{
			if (!useCachedValue || !m_dataChunkNameModifier.HasValue)
			{
				if (!useCachedValue)
				{
					m_isInstanceShared = null;
				}
				m_dataChunkNameModifier = subReportInfo.GetChunkNameModifierForParamValues(m_parameters, addEntry, ref m_isInstanceShared, out m_parameters);
			}
			isShared = m_isInstanceShared.Value;
			return m_dataChunkNameModifier.Value.ToString(CultureInfo.InvariantCulture);
		}

		internal override void InstanceComplete()
		{
			if (m_reportInstance != null)
			{
				m_reportInstance.Value()?.InstanceComplete();
			}
			IReference<SubReportInstance> obj = (IReference<SubReportInstance>)m_cleanupRef;
			base.InstanceComplete();
			obj.PinValue();
		}

		internal static IReference<SubReportInstance> CreateInstance(ScopeInstance parentInstance, SubReport subReport, OnDemandMetadata odpMetadata)
		{
			SubReportInstance subReportInstance = new SubReportInstance(subReport, odpMetadata);
			IReference<SubReportInstance> reference = odpMetadata.GroupTreeScalabilityCache.AllocateAndPin(subReportInstance, 0);
			subReportInstance.m_cleanupRef = (IDisposable)reference;
			parentInstance.AddChildScope((IReference<ScopeInstance>)reference, subReport.IndexInCollection);
			return reference;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.SubReport, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SubReport, Token.GlobalReference));
			list.Add(new MemberInfo(MemberName.ReportInstance, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportInstanceReference));
			list.Add(new MemberInfo(MemberName.DataSetUniqueName, Token.String));
			list.Add(new MemberInfo(MemberName.ThreadCulture, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.CultureInfo));
			list.Add(new MemberInfo(MemberName.Parameters, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Parameters));
			list.Add(new MemberInfo(MemberName.Status, Token.Enum));
			list.Add(new MemberInfo(MemberName.ProcessedWithError, Token.Boolean));
			list.Add(new MemberInfo(MemberName.IsInstanceShared, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Nullable, Token.Boolean));
			list.Add(new MemberInfo(MemberName.DataChunkNameModifier, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Nullable, Token.Int32));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SubReportInstance, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScopeInstance, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.SubReport:
					writer.WriteGlobalReference(m_subReportDef);
					break;
				case MemberName.Parameters:
					if (m_parameters != null)
					{
						writer.Write(new ParametersImplWrapper(m_parameters));
					}
					else
					{
						writer.WriteNull();
					}
					break;
				case MemberName.ReportInstance:
					writer.Write(m_reportInstance);
					break;
				case MemberName.DataSetUniqueName:
					writer.Write(m_instanceUniqueName);
					break;
				case MemberName.ThreadCulture:
					writer.Write(m_threadCulture);
					break;
				case MemberName.Status:
					writer.WriteEnum((int)m_status);
					break;
				case MemberName.ProcessedWithError:
					writer.Write(m_processedWithError);
					break;
				case MemberName.IsInstanceShared:
					writer.Write(m_isInstanceShared);
					break;
				case MemberName.DataChunkNameModifier:
					writer.Write(m_dataChunkNameModifier);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.SubReport:
					m_subReportDef = reader.ReadGlobalReference<SubReport>();
					break;
				case MemberName.Parameters:
				{
					ParametersImplWrapper parametersImplWrapper = (ParametersImplWrapper)reader.ReadRIFObject();
					if (parametersImplWrapper != null)
					{
						m_parameters = parametersImplWrapper.WrappedParametersImpl;
					}
					break;
				}
				case MemberName.ReportInstance:
					m_reportInstance = (IReference<ReportInstance>)reader.ReadRIFObject();
					break;
				case MemberName.DataSetUniqueName:
					m_instanceUniqueName = reader.ReadString();
					break;
				case MemberName.ThreadCulture:
					m_threadCulture = reader.ReadCultureInfo();
					break;
				case MemberName.Status:
					m_status = (SubReport.Status)reader.ReadEnum();
					break;
				case MemberName.ProcessedWithError:
					m_processedWithError = reader.ReadBoolean();
					break;
				case MemberName.IsInstanceShared:
				{
					object obj2 = reader.ReadVariant();
					if (obj2 != null)
					{
						m_isInstanceShared = (bool)obj2;
					}
					break;
				}
				case MemberName.DataChunkNameModifier:
				{
					object obj = reader.ReadVariant();
					if (obj != null)
					{
						m_dataChunkNameModifier = (int)obj;
					}
					break;
				}
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false);
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SubReportInstance;
		}
	}
}
