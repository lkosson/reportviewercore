using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal sealed class RuntimeTablixObj : RuntimeDataTablixWithScopedItemsObj, IStorable, IPersistable
	{
		[NonSerialized]
		private static Declaration m_declaration = GetDeclaration();

		internal RuntimeTablixObj()
		{
		}

		internal RuntimeTablixObj(IReference<IScope> outerScope, Microsoft.ReportingServices.ReportIntermediateFormat.Tablix tablixDef, ref DataActions dataAction, OnDemandProcessingContext odpContext, bool onePassProcess)
			: base(outerScope, tablixDef, ref dataAction, odpContext, onePassProcess, Microsoft.ReportingServices.ReportProcessing.ObjectType.Tablix)
		{
		}

		protected override List<Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem> GetDataRegionScopedItems()
		{
			return ((Microsoft.ReportingServices.ReportIntermediateFormat.Tablix)m_dataRegionDef).DataRegionScopedItemsForDataProcessing;
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeTablixObj;
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
		}

		public override void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			base.ResolveReferences(memberReferencesCollection, referenceableItems);
		}

		internal new static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> memberInfoList = new List<MemberInfo>();
				m_declaration = new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeTablixObj, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixWithScopedItemsObj, memberInfoList);
			}
			return m_declaration;
		}
	}
}
