using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal sealed class RuntimeSortDataHolder : IStorable, IPersistable
	{
		private DataFieldRow m_firstRow;

		private ScalableList<DataFieldRow> m_dataRows;

		private static Declaration m_declaration = GetDeclaration();

		public int Size => ItemSizes.SizeOf(m_firstRow) + ItemSizes.SizeOf(m_dataRows);

		internal RuntimeSortDataHolder()
		{
		}

		internal void NextRow(OnDemandProcessingContext odpContext, int depth)
		{
			DataFieldRow dataFieldRow = new DataFieldRow(odpContext.ReportObjectModel.FieldsImpl, getAndSave: true);
			if (m_firstRow == null)
			{
				m_firstRow = dataFieldRow;
				return;
			}
			if (m_dataRows == null)
			{
				m_dataRows = new ScalableList<DataFieldRow>(depth, odpContext.TablixProcessingScalabilityCache);
			}
			m_dataRows.Add(dataFieldRow);
		}

		internal void Traverse(ProcessingStages operation, ITraversalContext traversalContext, IHierarchyObj owner)
		{
			Global.Tracer.Assert(ProcessingStages.UserSortFilter == operation || owner.InDataRowSortPhase, "Invalid call to RuntimeSortDataHolder.Traverse.  Must be in UserSortFilter stage or InDataRowSortPhase");
			if (m_firstRow == null)
			{
				return;
			}
			DataRowSortOwnerTraversalContext context = traversalContext as DataRowSortOwnerTraversalContext;
			Traverse(m_firstRow, operation, context, owner);
			if (m_dataRows != null)
			{
				for (int i = 0; i < m_dataRows.Count; i++)
				{
					Traverse(m_dataRows[i], operation, context, owner);
				}
			}
		}

		private void Traverse(DataFieldRow dataRow, ProcessingStages operation, DataRowSortOwnerTraversalContext context, IHierarchyObj owner)
		{
			dataRow.SetFields(owner.OdpContext.ReportObjectModel.FieldsImpl);
			if (operation == ProcessingStages.UserSortFilter)
			{
				owner.ReadRow();
			}
			else
			{
				context.SortOwner.PostDataRowSortNextRow();
			}
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.FirstRow:
					writer.Write(m_firstRow);
					break;
				case MemberName.DataRows:
					writer.Write(m_dataRows);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.FirstRow:
					m_firstRow = (DataFieldRow)reader.ReadRIFObject();
					break;
				case MemberName.DataRows:
					m_dataRows = reader.ReadRIFObject<ScalableList<DataFieldRow>>();
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
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeSortDataHolder;
		}

		public static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.FirstRow, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataFieldRow));
				list.Add(new MemberInfo(MemberName.DataRows, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableList));
				return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeSortDataHolder, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return m_declaration;
		}
	}
}
