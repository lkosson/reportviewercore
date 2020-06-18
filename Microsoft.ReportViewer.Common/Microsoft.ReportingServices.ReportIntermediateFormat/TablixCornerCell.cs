using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal sealed class TablixCornerCell : TablixCellBase, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal TablixCornerCell()
		{
		}

		internal TablixCornerCell(int id, DataRegion dataRegion)
			: base(id, dataRegion)
		{
		}

		internal override void InternalInitialize(int parentRowID, int parentColumnID, int rowindex, int colIndex, InitializationContext context)
		{
			base.InternalInitialize(parentRowID, parentColumnID, rowindex, colIndex, context);
			m_hasInnerGroupTreeHierarchy = (Cell.ContainsInnerGroupTreeHierarchy(m_cellContents) | Cell.ContainsInnerGroupTreeHierarchy(m_altCellContents));
		}

		protected override void StartExprHost(InitializationContext context)
		{
		}

		protected override void EndExprHost(InitializationContext context)
		{
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			return (TablixCornerCell)base.PublishClone(context);
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> memberInfoList = new List<MemberInfo>();
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TablixCornerCell, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TablixCellBase, memberInfoList);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				_ = writer.CurrentMember.MemberName;
				Global.Tracer.Assert(condition: false);
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				_ = reader.CurrentMember.MemberName;
				Global.Tracer.Assert(condition: false);
			}
		}

		public override void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			base.ResolveReferences(memberReferencesCollection, referenceableItems);
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TablixCornerCell;
		}
	}
}
