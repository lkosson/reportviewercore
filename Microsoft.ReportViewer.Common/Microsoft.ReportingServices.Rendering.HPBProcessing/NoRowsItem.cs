using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.Rendering.HPBProcessing
{
	internal sealed class NoRowsItem : PageItem, IStorable, IPersistable
	{
		private static Declaration m_declaration = GetDeclaration();

		internal override bool ContentOnPage => false;

		internal NoRowsItem()
		{
		}

		internal NoRowsItem(ReportItem source)
			: base(source)
		{
			m_itemPageSizes = new ItemSizes(source);
			base.KeepTogetherHorizontal = false;
			base.KeepTogetherVertical = false;
			base.UnresolvedKTV = (base.UnresolvedKTH = false);
		}

		protected override void DetermineVerticalSize(PageContext pageContext, double topInParentSystem, double bottomInParentSystem, List<PageItem> ancestors, ref bool anyAncestorHasKT, bool hasUnpinnedAncestors)
		{
			m_itemPageSizes.AdjustHeightTo(0.0);
		}

		protected override void DetermineHorizontalSize(PageContext pageContext, double leftInParentSystem, double rightInParentSystem, List<PageItem> ancestors, bool anyAncestorHasKT, bool hasUnpinnedAncestors)
		{
			m_itemPageSizes.AdjustWidthTo(0.0);
		}

		internal override void WriteStartItemToStream(RPLWriter rplWriter, PageContext pageContext)
		{
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_declaration);
			while (writer.NextMember())
			{
				_ = writer.CurrentMember.MemberName;
				RSTrace.RenderingTracer.Assert(condition: false, string.Empty);
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(m_declaration);
			while (reader.NextMember())
			{
				_ = reader.CurrentMember.MemberName;
				RSTrace.RenderingTracer.Assert(condition: false, string.Empty);
			}
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.NoRowsItem;
		}

		internal new static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> memberInfoList = new List<MemberInfo>();
				return new Declaration(ObjectType.NoRowsItem, ObjectType.PageItem, memberInfoList);
			}
			return m_declaration;
		}
	}
}
