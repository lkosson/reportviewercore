using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal sealed class RuntimeGroupingObjHash : RuntimeGroupingObj
	{
		private ScalableDictionary<object, IReference<RuntimeHierarchyObj>> m_hashtable;

		private ScalableDictionary<object, ChildLeafInfo> m_parentInfo;

		[NonSerialized]
		private static Declaration m_declaration = GetDeclaration();

		public override int Size => base.Size + ItemSizes.SizeOf(m_hashtable) + ItemSizes.SizeOf(m_parentInfo);

		internal RuntimeGroupingObjHash()
		{
		}

		internal RuntimeGroupingObjHash(RuntimeHierarchyObj owner, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType)
			: base(owner, objectType)
		{
			OnDemandProcessingContext odpContext = owner.OdpContext;
			m_hashtable = new ScalableDictionary<object, IReference<RuntimeHierarchyObj>>(owner.Depth + 1, odpContext.TablixProcessingScalabilityCache, 101, 27, odpContext.ProcessingComparer);
		}

		internal override void Cleanup()
		{
			if (m_hashtable != null)
			{
				m_hashtable.Dispose();
				m_hashtable = null;
			}
			if (m_parentInfo != null)
			{
				m_parentInfo.Dispose();
				m_parentInfo = null;
			}
		}

		internal override void NextRow(object keyValue, bool hasParent, object parentKey)
		{
			IReference<RuntimeHierarchyObj> value = null;
			try
			{
				m_hashtable.TryGetValue(keyValue, out value);
			}
			catch (ReportProcessingException_SpatialTypeComparisonError reportProcessingException_SpatialTypeComparisonError)
			{
				throw new ReportProcessingException(m_owner.RegisterSpatialTypeComparisonError(reportProcessingException_SpatialTypeComparisonError.Type));
			}
			catch (ReportProcessingException_ComparisonError e)
			{
				throw new ReportProcessingException(m_owner.RegisterComparisonError("GroupExpression", e));
			}
			if (value != null)
			{
				using (value.PinValue())
				{
					value.Value().NextRow();
				}
				return;
			}
			RuntimeHierarchyObj runtimeHierarchyObj = new RuntimeHierarchyObj(m_owner, m_objectType, ((IScope)m_owner).Depth + 1);
			value = (IReference<RuntimeHierarchyObj>)runtimeHierarchyObj.SelfReference;
			try
			{
				m_hashtable.Add(keyValue, value);
				runtimeHierarchyObj = value.Value();
				runtimeHierarchyObj.NextRow();
				if (hasParent)
				{
					IReference<RuntimeHierarchyObj> value2 = null;
					IReference<RuntimeGroupLeafObj> reference = null;
					try
					{
						m_hashtable.TryGetValue(parentKey, out value2);
					}
					catch (ReportProcessingException_SpatialTypeComparisonError reportProcessingException_SpatialTypeComparisonError2)
					{
						throw new ReportProcessingException(m_owner.RegisterSpatialTypeComparisonError(reportProcessingException_SpatialTypeComparisonError2.Type));
					}
					catch (ReportProcessingException_ComparisonError e2)
					{
						throw new ReportProcessingException(m_owner.RegisterComparisonError("Parent", e2));
					}
					if (value2 != null)
					{
						RuntimeHierarchyObj runtimeHierarchyObj2 = value2.Value();
						Global.Tracer.Assert(runtimeHierarchyObj2.HierarchyObjs != null, "(null != parentHierarchyObj.HierarchyObjs)");
						reference = (RuntimeGroupLeafObjReference)runtimeHierarchyObj2.HierarchyObjs[0];
					}
					Global.Tracer.Assert(runtimeHierarchyObj.HierarchyObjs != null, "(null != hierarchyObj.HierarchyObjs)");
					RuntimeGroupLeafObjReference runtimeGroupLeafObjReference = (RuntimeGroupLeafObjReference)runtimeHierarchyObj.HierarchyObjs[0];
					bool addToWaitList = true;
					if (reference == runtimeGroupLeafObjReference)
					{
						reference = null;
						addToWaitList = false;
					}
					ProcessChildren(keyValue, reference, runtimeGroupLeafObjReference);
					ProcessParent(parentKey, reference, runtimeGroupLeafObjReference, addToWaitList);
				}
			}
			finally
			{
				value.UnPinValue();
			}
		}

		internal override void Traverse(ProcessingStages operation, bool ascending, ITraversalContext traversalContext)
		{
			RuntimeGroupRootObj runtimeGroupRootObj = m_owner as RuntimeGroupRootObj;
			Global.Tracer.Assert(runtimeGroupRootObj != null, "(null != groupRootOwner)");
			runtimeGroupRootObj.TraverseLinkedGroupLeaves(operation, ascending, traversalContext);
		}

		internal override void CopyDomainScopeGroupInstances(RuntimeGroupRootObj destination)
		{
			DomainScopeContext domainScopeContext = m_owner.OdpContext.DomainScopeContext;
			domainScopeContext.CurrentDomainScope = new DomainScopeContext.DomainScopeInfo();
			domainScopeContext.CurrentDomainScope.InitializeKeys((m_owner as RuntimeGroupRootObj).GroupExpressions.Count);
			CopyDomainScopeGroupInstance(destination, m_hashtable);
			domainScopeContext.CurrentDomainScope = null;
		}

		private void CopyDomainScopeGroupInstance(RuntimeGroupRootObj destination, ScalableDictionary<object, IReference<RuntimeHierarchyObj>> runtimeHierarchyObjRefs)
		{
			IReference<RuntimeHierarchyObj> reference = null;
			DomainScopeContext.DomainScopeInfo currentDomainScope = m_owner.OdpContext.DomainScopeContext.CurrentDomainScope;
			foreach (object key in runtimeHierarchyObjRefs.Keys)
			{
				currentDomainScope.AddKey(key);
				reference = runtimeHierarchyObjRefs[key];
				using (reference.PinValue())
				{
					RuntimeHierarchyObj runtimeHierarchyObj = reference.Value();
					if (runtimeHierarchyObj.HierarchyObjs == null)
					{
						RuntimeGroupingObjHash runtimeGroupingObjHash = (RuntimeGroupingObjHash)runtimeHierarchyObj.Grouping;
						CopyDomainScopeGroupInstance(destination, runtimeGroupingObjHash.m_hashtable);
					}
					else
					{
						Global.Tracer.Assert(runtimeHierarchyObj.HierarchyObjs.Count == 1, "hierarchyObject.HierarchyObjs.Count == 1");
						IReference<RuntimeHierarchyObj> reference2 = runtimeHierarchyObj.HierarchyObjs[0];
						using (reference2.PinValue())
						{
							RuntimeDataTablixGroupLeafObj runtimeDataTablixGroupLeafObj = (RuntimeDataTablixGroupLeafObj)reference2.Value();
							currentDomainScope.CurrentRow = runtimeDataTablixGroupLeafObj.FirstRow;
							destination.NextRow();
						}
					}
				}
				currentDomainScope.RemoveKey();
			}
		}

		private void ProcessParent(object parentKey, IReference<RuntimeGroupLeafObj> parentObj, RuntimeGroupLeafObjReference childObj, bool addToWaitList)
		{
			if (parentObj != null)
			{
				using (parentObj.PinValue())
				{
					parentObj.Value().AddChild(childObj);
				}
				return;
			}
			(m_owner as RuntimeGroupRootObj).AddChild(childObj);
			if (!addToWaitList)
			{
				return;
			}
			ChildLeafInfo value = null;
			IDisposable reference = null;
			try
			{
				if (m_parentInfo == null)
				{
					m_parentInfo = CreateParentInfo();
				}
				else
				{
					m_parentInfo.TryGetAndPin(parentKey, out value, out reference);
				}
				if (value == null)
				{
					value = new ChildLeafInfo();
					reference = m_parentInfo.AddAndPin(parentKey, value);
				}
				value.Add(childObj);
			}
			finally
			{
				reference?.Dispose();
			}
		}

		private ScalableDictionary<object, ChildLeafInfo> CreateParentInfo()
		{
			OnDemandProcessingContext odpContext = m_owner.OdpContext;
			return new ScalableDictionary<object, ChildLeafInfo>(m_owner.Depth, odpContext.TablixProcessingScalabilityCache, 101, 27);
		}

		private void ProcessChildren(object thisKey, IReference<RuntimeGroupLeafObj> parentObj, IReference<RuntimeGroupLeafObj> thisObj)
		{
			ChildLeafInfo value = null;
			if (m_parentInfo != null)
			{
				m_parentInfo.TryGetValue(thisKey, out value);
			}
			if (value == null)
			{
				return;
			}
			for (int i = 0; i < value.Count; i++)
			{
				RuntimeGroupLeafObjReference runtimeGroupLeafObjReference = value[i];
				using (runtimeGroupLeafObjReference.PinValue())
				{
					RuntimeGroupLeafObj runtimeGroupLeafObj = runtimeGroupLeafObjReference.Value();
					bool flag = false;
					IReference<RuntimeGroupObj> reference = parentObj as IReference<RuntimeGroupObj>;
					while (reference != null && !flag)
					{
						RuntimeGroupLeafObj runtimeGroupLeafObj2 = reference.Value() as RuntimeGroupLeafObj;
						if (runtimeGroupLeafObj2 == runtimeGroupLeafObj)
						{
							flag = true;
						}
						reference = runtimeGroupLeafObj2?.Parent;
					}
					if (!flag)
					{
						runtimeGroupLeafObj.RemoveFromParent((RuntimeGroupRootObjReference)m_owner.SelfReference);
						using (thisObj.PinValue())
						{
							thisObj.Value().AddChild(runtimeGroupLeafObjReference);
						}
					}
				}
			}
			m_parentInfo.Remove(thisKey);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Hashtable:
					writer.Write(m_hashtable);
					break;
				case MemberName.ParentInfo:
					writer.Write(m_parentInfo);
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
				case MemberName.Hashtable:
					m_hashtable = reader.ReadRIFObject<ScalableDictionary<object, IReference<RuntimeHierarchyObj>>>();
					break;
				case MemberName.ParentInfo:
					m_parentInfo = reader.ReadRIFObject<ScalableDictionary<object, ChildLeafInfo>>();
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
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupingObjHash;
		}

		public new static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.Hashtable, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableDictionary));
				list.Add(new MemberInfo(MemberName.ParentInfo, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableDictionary));
				return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupingObjHash, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupingObj, list);
			}
			return m_declaration;
		}
	}
}
