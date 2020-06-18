using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal abstract class RuntimeDetailObj : RuntimeHierarchyObj
	{
		protected IReference<IScope> m_outerScope;

		[StaticReference]
		protected Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion m_dataRegionDef;

		protected ScalableList<DataFieldRow> m_dataRows;

		protected List<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult[]> m_rvValueList;

		protected List<string> m_runningValuesInGroup;

		protected List<string> m_previousValuesInGroup;

		protected Dictionary<string, IReference<RuntimeGroupRootObj>> m_groupCollection;

		protected DataActions m_outerDataAction;

		private static Declaration m_declaration = GetDeclaration();

		public override int Size => base.Size + ItemSizes.SizeOf(m_outerScope) + ItemSizes.ReferenceSize + ItemSizes.SizeOf(m_dataRows) + ItemSizes.SizeOf(m_rvValueList) + ItemSizes.SizeOf(m_runningValuesInGroup) + ItemSizes.SizeOf(m_previousValuesInGroup) + ItemSizes.SizeOf(m_groupCollection) + 4;

		protected RuntimeDetailObj(IReference<IScope> outerScope, Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef, DataActions dataAction, OnDemandProcessingContext odpContext, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType)
			: base(odpContext, objectType, (outerScope == null) ? dataRegionDef.InnerGroupingDynamicMemberCount : (outerScope.Value().Depth + 1))
		{
			m_hierarchyRoot = (RuntimeDetailObjReference)base.SelfReference;
			m_outerScope = outerScope;
			m_dataRegionDef = dataRegionDef;
			m_outerDataAction = dataAction;
		}

		internal RuntimeDetailObj(RuntimeDetailObj detailRoot, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType)
			: base(detailRoot.OdpContext, objectType, detailRoot.Depth)
		{
			m_hierarchyRoot = (RuntimeDetailObjReference)detailRoot.SelfReference;
			m_outerScope = detailRoot.m_outerScope;
			m_dataRegionDef = detailRoot.m_dataRegionDef;
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_declaration);
			IScalabilityCache scalabilityCache = writer.PersistenceHelper as IScalabilityCache;
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.OuterScope:
					writer.Write(m_outerScope);
					break;
				case MemberName.DataRegionDef:
				{
					int value = scalabilityCache.StoreStaticReference(m_dataRegionDef);
					writer.Write(value);
					break;
				}
				case MemberName.DataRows:
					writer.Write(m_dataRows);
					break;
				case MemberName.RunningValueValues:
					writer.Write(m_rvValueList);
					break;
				case MemberName.RunningValuesInGroup:
					writer.WriteListOfPrimitives(m_runningValuesInGroup);
					break;
				case MemberName.PreviousValuesInGroup:
					writer.WriteListOfPrimitives(m_previousValuesInGroup);
					break;
				case MemberName.GroupCollection:
					writer.Write(m_groupCollection);
					break;
				case MemberName.OuterDataAction:
					writer.Write(m_outerDataAction);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(m_declaration);
			IScalabilityCache scalabilityCache = reader.PersistenceHelper as IScalabilityCache;
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.OuterScope:
					m_outerScope = (IReference<IScope>)reader.ReadRIFObject();
					break;
				case MemberName.DataRegionDef:
				{
					int id = reader.ReadInt32();
					m_dataRegionDef = (Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion)scalabilityCache.FetchStaticReference(id);
					break;
				}
				case MemberName.DataRows:
					m_dataRows = reader.ReadRIFObject<ScalableList<DataFieldRow>>();
					break;
				case MemberName.RunningValueValues:
					m_rvValueList = reader.ReadListOfRIFObjectArrays<Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult>();
					break;
				case MemberName.RunningValuesInGroup:
					m_runningValuesInGroup = reader.ReadListOfPrimitives<string>();
					break;
				case MemberName.PreviousValuesInGroup:
					m_previousValuesInGroup = reader.ReadListOfPrimitives<string>();
					break;
				case MemberName.GroupCollection:
					m_groupCollection = reader.ReadStringRIFObjectDictionary<IReference<RuntimeGroupRootObj>>();
					break;
				case MemberName.OuterDataAction:
					m_outerDataAction = (DataActions)reader.ReadEnum();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			base.ResolveReferences(memberReferencesCollection, referenceableItems);
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDetailObj;
		}

		public new static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.OuterScope, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IScopeReference));
				list.Add(new MemberInfo(MemberName.DataRegionDef, Token.Int32));
				list.Add(new MemberInfo(MemberName.DataRows, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataFieldRow));
				list.Add(new MemberInfo(MemberName.RunningValueValues, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectArray));
				list.Add(new MemberInfo(MemberName.RunningValuesInGroup, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.String));
				list.Add(new MemberInfo(MemberName.PreviousValuesInGroup, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.String));
				list.Add(new MemberInfo(MemberName.GroupCollection, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StringRIFObjectDictionary, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupRootObjReference));
				list.Add(new MemberInfo(MemberName.OuterDataAction, Token.Enum));
				return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDetailObj, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeHierarchyObj, list);
			}
			return m_declaration;
		}
	}
}
