using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal abstract class RuntimeGroupObj : RuntimeHierarchyObj
	{
		protected RuntimeGroupLeafObjReference m_lastChild;

		protected RuntimeGroupLeafObjReference m_firstChild;

		private static Declaration m_declaration = GetDeclaration();

		internal RuntimeGroupLeafObjReference LastChild
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

		internal RuntimeGroupLeafObjReference FirstChild
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

		public override int Size => base.Size + ItemSizes.SizeOf(m_lastChild) + ItemSizes.SizeOf(m_firstChild);

		protected RuntimeGroupObj()
		{
		}

		protected RuntimeGroupObj(OnDemandProcessingContext odpContext, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, int level)
			: base(odpContext, objectType, level)
		{
		}

		internal void AddChild(RuntimeGroupLeafObjReference child)
		{
			if (null != m_lastChild)
			{
				using (m_lastChild.PinValue())
				{
					m_lastChild.Value().NextLeaf = child;
				}
			}
			else
			{
				m_firstChild = child;
			}
			using (child.PinValue())
			{
				RuntimeGroupLeafObj runtimeGroupLeafObj = child.Value();
				runtimeGroupLeafObj.PrevLeaf = m_lastChild;
				runtimeGroupLeafObj.NextLeaf = null;
				runtimeGroupLeafObj.Parent = (RuntimeGroupObjReference)m_selfReference;
			}
			m_lastChild = child;
		}

		internal void InsertToSortTree(RuntimeGroupLeafObjReference groupLeaf)
		{
			using (m_hierarchyRoot.PinValue())
			{
				RuntimeGroupRootObj runtimeGroupRootObj = (RuntimeGroupRootObj)m_hierarchyRoot.Value();
				Microsoft.ReportingServices.ReportIntermediateFormat.Grouping grouping = runtimeGroupRootObj.HierarchyDef.Grouping;
				if (runtimeGroupRootObj.ProcessSecondPassSorting)
				{
					Global.Tracer.Assert(m_grouping != null, "(m_grouping != null)");
					runtimeGroupRootObj.LastChild = groupLeaf;
					Global.Tracer.Assert(grouping != null, "(null != groupingDef)");
					object keyValue = m_odpContext.ReportRuntime.EvaluateRuntimeExpression(m_expression, Microsoft.ReportingServices.ReportProcessing.ObjectType.Grouping, grouping.Name, "Sort");
					m_grouping.NextRow(keyValue);
				}
				else
				{
					Global.Tracer.Assert(runtimeGroupRootObj.HierarchyDef.HasFilters || runtimeGroupRootObj.HierarchyDef.HasInnerFilters, "(groupRoot.HierarchyDef.HasFilters || groupRoot.HierarchyDef.HasInnerFilters)");
					AddChild(groupLeaf);
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
				case MemberName.FirstChild:
					writer.Write(m_firstChild);
					break;
				case MemberName.LastChild:
					writer.Write(m_lastChild);
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
				case MemberName.FirstChild:
					m_firstChild = (RuntimeGroupLeafObjReference)reader.ReadRIFObject();
					break;
				case MemberName.LastChild:
					m_lastChild = (RuntimeGroupLeafObjReference)reader.ReadRIFObject();
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
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupObj;
		}

		public new static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.LastChild, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupLeafObjReference));
				list.Add(new MemberInfo(MemberName.FirstChild, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupLeafObjReference));
				return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupObj, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeHierarchyObj, list);
			}
			return m_declaration;
		}
	}
}
