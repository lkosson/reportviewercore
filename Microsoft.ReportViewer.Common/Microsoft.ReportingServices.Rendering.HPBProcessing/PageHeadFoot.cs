using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.HPBProcessing
{
	internal class PageHeadFoot : PageItemContainer
	{
		private new PageSection m_source;

		private bool m_isHeader;

		internal override string SourceUniqueName => m_source.Instance.UniqueName;

		internal override string SourceID => m_source.ID;

		internal override Style SharedStyle => m_source.Style;

		internal override StyleInstance NonSharedStyle => m_source.Instance.Style;

		internal override double OriginalLeft => 0.0;

		internal override double OriginalWidth => m_itemPageSizes.Width;

		internal override byte RPLFormatType
		{
			get
			{
				if (m_isHeader)
				{
					return 4;
				}
				return 5;
			}
		}

		internal PageHeadFoot(PageSection source, double width, bool aIsHeader)
			: base(null)
		{
			m_itemPageSizes = new FixedItemSizes(width, source.Height.ToMillimeters());
			m_source = source;
			m_isHeader = aIsHeader;
		}

		protected override void CreateChildren(PageContext pageContext)
		{
			CreateChildren(m_source.ReportItemCollection, pageContext);
		}

		internal override bool HitsCurrentPage(double pageLeft, double pageTop, double pageRight, double pageBottom)
		{
			return true;
		}

		internal override RPLElement CreateRPLElement()
		{
			return new RPLHeaderFooter();
		}

		internal override RPLElement CreateRPLElement(RPLElementProps props, PageContext pageContext)
		{
			RPLElement rPLElement = CreateRPLElement();
			WriteElementProps(rPLElement.ElementProps, pageContext);
			return rPLElement;
		}

		internal override void WriteItemSharedStyleProps(BinaryWriter spbifWriter, Style style, PageContext pageContext)
		{
			WriteStyleProp(style, spbifWriter, StyleAttributeNames.BackgroundColor, 34);
			WriteBackgroundImage(style, writeShared: true, spbifWriter, pageContext);
		}

		internal override void WriteItemSharedStyleProps(RPLStyleProps rplStyleProps, Style style, PageContext pageContext)
		{
			PageItem.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.BackgroundColor, 34);
			WriteBackgroundImage(style, writeShared: true, rplStyleProps, pageContext);
		}

		internal override void WriteItemNonSharedStyleProps(BinaryWriter spbifWriter, Style styleDef, StyleInstance style, StyleAttributeNames styleAtt, PageContext pageContext)
		{
			switch (styleAtt)
			{
			case StyleAttributeNames.BackgroundColor:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BackgroundColor, 34);
				break;
			case StyleAttributeNames.BackgroundImage:
				WriteBackgroundImage(styleDef, writeShared: false, spbifWriter, pageContext);
				break;
			}
		}

		internal override void WriteItemNonSharedStyleProps(RPLStyleProps rplStyleProps, Style styleDef, StyleInstance style, StyleAttributeNames styleAtt, PageContext pageContext)
		{
			switch (styleAtt)
			{
			case StyleAttributeNames.BackgroundColor:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BackgroundColor, 34);
				break;
			case StyleAttributeNames.BackgroundImage:
				WriteBackgroundImage(styleDef, writeShared: false, rplStyleProps, pageContext);
				break;
			}
		}
	}
}
