using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal sealed class RuntimeDataTablixMemberObj : RuntimeMemberObj
	{
		private bool m_hasStaticMembers;

		private List<int> m_staticLeafCellIndexes;

		[NonSerialized]
		private static Declaration m_declaration = GetDeclaration();

		public override int Size => base.Size + 1 + ItemSizes.SizeOf(m_staticLeafCellIndexes);

		internal RuntimeDataTablixMemberObj()
		{
		}

		private RuntimeDataTablixMemberObj(IReference<IScope> owner, Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode dynamicMember, ref DataActions dataAction, OnDemandProcessingContext odpContext, IReference<RuntimeMemberObj>[] innerGroupings, HierarchyNodeList staticMembers, bool outerMostStatics, int headingLevel, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType)
			: base(owner, dynamicMember)
		{
			int excludedCellIndex = -1;
			if (dynamicMember != null)
			{
				excludedCellIndex = dynamicMember.MemberCellIndex;
				RuntimeDataTablixGroupRootObj runtimeDataTablixGroupRootObj = new RuntimeDataTablixGroupRootObj(owner, dynamicMember, ref dataAction, odpContext, innerGroupings, outerMostStatics, headingLevel, objectType);
				m_groupRoot = (RuntimeDataTablixGroupRootObjReference)runtimeDataTablixGroupRootObj.SelfReference;
				m_groupRoot.UnPinValue();
			}
			if (staticMembers != null && staticMembers.Count != 0)
			{
				_ = staticMembers.Count;
				m_hasStaticMembers = true;
				m_staticLeafCellIndexes = staticMembers.GetLeafCellIndexes(excludedCellIndex);
			}
		}

		internal static IReference<RuntimeMemberObj> CreateRuntimeMemberObject(IReference<IScope> owner, Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode dynamicMemberDef, ref DataActions dataAction, OnDemandProcessingContext odpContext, IReference<RuntimeMemberObj>[] innerGroupings, HierarchyNodeList staticMembers, bool outerMostStatics, int headingLevel, Microsoft.ReportingServices.ReportProcessing.ObjectType dataRegionType)
		{
			RuntimeDataTablixMemberObj obj = new RuntimeDataTablixMemberObj(owner, dynamicMemberDef, ref dataAction, odpContext, innerGroupings, staticMembers, outerMostStatics, headingLevel, dataRegionType);
			return odpContext.TablixProcessingScalabilityCache.Allocate((RuntimeMemberObj)obj, headingLevel);
		}

		internal override void CreateInstances(IReference<RuntimeDataRegionObj> containingScopeRef, OnDemandProcessingContext odpContext, DataRegionInstance dataRegionInstance, bool isOuterGrouping, IReference<RuntimeDataTablixGroupRootObj> currOuterGroupRoot, ScopeInstance parentInstance, IReference<RuntimeMemberObj>[] innerMembers, IReference<RuntimeDataTablixGroupLeafObj> innerGroupLeaf)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef = dataRegionInstance.DataRegionDef;
			if (isOuterGrouping && m_hasStaticMembers)
			{
				dataRegionDef.NewOuterCells();
			}
			if (null != m_groupRoot)
			{
				dataRegionDef.CurrentOuterGroupRoot = currOuterGroupRoot;
				using (m_groupRoot.PinValue())
				{
					m_groupRoot.Value().CreateInstances(parentInstance, innerMembers, innerGroupLeaf);
				}
				if (m_staticLeafCellIndexes == null)
				{
					return;
				}
				if (isOuterGrouping && m_hasStaticMembers)
				{
					dataRegionDef.NewOuterCells();
				}
				IReference<RuntimeDataTablixGroupRootObj> reference = null;
				if (m_owner is IReference<RuntimeDataTablixObj>)
				{
					using (m_owner.PinValue())
					{
						((RuntimeDataTablixObj)m_owner.Value()).SetupEnvironment();
					}
					if (isOuterGrouping && m_hasStaticMembers)
					{
						reference = dataRegionDef.CurrentOuterGroupRoot;
						dataRegionDef.CurrentOuterGroupRoot = null;
						currOuterGroupRoot = null;
					}
				}
				else
				{
					using (containingScopeRef.PinValue())
					{
						containingScopeRef.Value().SetupEnvironment();
					}
				}
				CreateCells(containingScopeRef, odpContext, dataRegionInstance, isOuterGrouping, currOuterGroupRoot, parentInstance, innerMembers, innerGroupLeaf);
				if (reference != null)
				{
					dataRegionDef.CurrentOuterGroupRoot = reference;
				}
			}
			else
			{
				CreateCells(containingScopeRef, odpContext, dataRegionInstance, isOuterGrouping, currOuterGroupRoot, parentInstance, innerMembers, innerGroupLeaf);
			}
		}

		private void CreateCells(IReference<RuntimeDataRegionObj> containingScopeRef, OnDemandProcessingContext odpContext, DataRegionInstance dataRegionInstance, bool isOuterGroup, IReference<RuntimeDataTablixGroupRootObj> currOuterGroupRoot, ScopeInstance parentInstance, IReference<RuntimeMemberObj>[] innerMembers, IReference<RuntimeDataTablixGroupLeafObj> innerGroupLeafRef)
		{
			using (containingScopeRef.PinValue())
			{
				RuntimeDataRegionObj runtimeDataRegionObj = containingScopeRef.Value();
				if (runtimeDataRegionObj is RuntimeDataTablixGroupLeafObj)
				{
					((RuntimeDataTablixGroupLeafObj)runtimeDataRegionObj).CreateStaticCells(dataRegionInstance, parentInstance, currOuterGroupRoot, isOuterGroup, m_staticLeafCellIndexes, innerMembers, innerGroupLeafRef);
				}
				else
				{
					((RuntimeDataTablixObj)runtimeDataRegionObj).CreateOutermostStaticCells(dataRegionInstance, isOuterGroup, innerMembers, innerGroupLeafRef);
				}
			}
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.HasStaticMembers:
					writer.Write(m_hasStaticMembers);
					break;
				case MemberName.StaticLeafCellIndexes:
					writer.WriteListOfPrimitives(m_staticLeafCellIndexes);
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
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.HasStaticMembers:
					m_hasStaticMembers = reader.ReadBoolean();
					break;
				case MemberName.StaticLeafCellIndexes:
					m_staticLeafCellIndexes = reader.ReadListOfPrimitives<int>();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixMemberObj;
		}

		public new static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.HasStaticMembers, Token.Boolean));
				list.Add(new MemberInfo(MemberName.StaticLeafCellIndexes, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.Int32));
				return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixMemberObj, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeMemberObj, list);
			}
			return m_declaration;
		}
	}
}
