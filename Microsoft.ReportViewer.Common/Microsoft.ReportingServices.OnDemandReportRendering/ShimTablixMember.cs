using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class ShimTablixMember : TablixMember, IShimDataRegionMember
	{
		protected bool m_isColumn;

		protected PageBreakLocation m_propagatedPageBreak;

		internal override string UniqueName => ID;

		public override string ID
		{
			get
			{
				if (m_group != null && m_group.RenderGroups != null)
				{
					return m_group.CurrentShimRenderGroup.ID;
				}
				return base.DefinitionPath;
			}
		}

		public override string DataElementName
		{
			get
			{
				if (m_group != null && m_group.CurrentShimRenderGroup != null)
				{
					return m_group.CurrentShimRenderGroup.DataCollectionName;
				}
				return null;
			}
		}

		public override DataElementOutputTypes DataElementOutput
		{
			get
			{
				if (IsStatic)
				{
					if (TablixHeader != null)
					{
						return DataElementOutputTypes.Output;
					}
					return DataElementOutputTypes.ContentsOnly;
				}
				return DataElementOutputTypes.Output;
			}
		}

		public override CustomPropertyCollection CustomProperties
		{
			get
			{
				if (m_customPropertyCollection == null)
				{
					if (m_group != null && m_group.CustomProperties != null)
					{
						m_customPropertyCollection = m_group.CustomProperties;
					}
					else
					{
						m_customPropertyCollection = new CustomPropertyCollection();
					}
				}
				return m_customPropertyCollection;
			}
		}

		public override TablixHeader TablixHeader => null;

		public override bool IsColumn => m_isColumn;

		public override bool HideIfNoRows => false;

		internal override Microsoft.ReportingServices.ReportIntermediateFormat.TablixMember MemberDefinition => null;

		public override bool FixedData => false;

		public override KeepWithGroup KeepWithGroup => KeepWithGroup.None;

		public override bool RepeatOnNewPage => false;

		internal override IRIFReportScope RIFReportScope => null;

		internal override IReportScopeInstance ReportScopeInstance => null;

		internal override IReportScope ReportScope => null;

		internal ShimTablixMember(IDefinitionPath parentDefinitionPath, Tablix owner, TablixMember parent, int parentCollectionIndex, bool isColumn)
			: base(parentDefinitionPath, owner, parent, parentCollectionIndex)
		{
			m_isColumn = isColumn;
		}

		internal override void ResetContext()
		{
			base.ResetContext();
		}

		internal virtual void SetPropagatedPageBreak(PageBreakLocation pageBreakLocation)
		{
			m_propagatedPageBreak = pageBreakLocation;
		}

		internal abstract bool SetNewContext(int index);

		bool IShimDataRegionMember.SetNewContext(int index)
		{
			return SetNewContext(index);
		}

		void IShimDataRegionMember.ResetContext()
		{
			ResetContext();
		}
	}
}
