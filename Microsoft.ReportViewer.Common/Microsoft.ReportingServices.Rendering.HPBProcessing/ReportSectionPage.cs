using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.HPBProcessing
{
	internal sealed class ReportSectionPage : PageItem
	{
		private new Page m_source;

		private RPLPageLayout m_pageLayout;

		internal override string SourceUniqueName => m_source.Instance.UniqueName;

		internal override string SourceID => m_source.ID;

		internal RPLPageLayout PageLayout
		{
			set
			{
				m_pageLayout = value;
			}
		}

		internal ReportSectionPage(Page source)
			: base(null)
		{
			m_source = source;
		}

		internal override void WriteStartItemToStream(RPLWriter rplWriter, PageContext pageContext)
		{
		}

		internal void WriteItemStyle(RPLWriter rplWriter, PageContext pageContext)
		{
			if (rplWriter == null)
			{
				return;
			}
			Style style = m_source.Style;
			if (style == null)
			{
				return;
			}
			BinaryWriter binaryWriter = rplWriter.BinaryWriter;
			if (binaryWriter != null)
			{
				WriteSharedStyle(binaryWriter, style, pageContext);
				StyleInstance style2 = m_source.Instance.Style;
				if (style2 != null)
				{
					WriteNonSharedStyleWithoutTag(binaryWriter, style, style2, pageContext);
				}
				binaryWriter.Write(byte.MaxValue);
				return;
			}
			RPLPageLayout pageLayout = m_pageLayout;
			if (pageLayout != null)
			{
				RPLStyleProps rPLStyleProps = WriteSharedStyle(style, pageContext);
				RPLStyleProps rPLStyleProps2 = null;
				StyleInstance style3 = m_source.Instance.Style;
				if (style3 != null)
				{
					rPLStyleProps2 = WriteNonSharedStyle(style, style3, pageContext);
				}
				if (rPLStyleProps != null || rPLStyleProps2 != null)
				{
					pageLayout.Style = new RPLElementStyle(rPLStyleProps2, rPLStyleProps);
				}
			}
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
