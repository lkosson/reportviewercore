using Microsoft.ReportingServices.Rendering.ExcelRenderer.Excel;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.SPBIFReader.Callbacks;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.ReportingServices.Rendering.ExcelRenderer.Layout
{
	internal sealed class HeaderFooterLayout : ALayout
	{
		private List<ReportItemInfo> m_fullList;

		private int m_centerWidth;

		private int m_rightWidth;

		private List<ReportItemInfo> m_leftList;

		private List<ReportItemInfo> m_rightList;

		private List<ReportItemInfo> m_centerList;

		private float m_height;

		internal float Height => m_height;

		internal override bool HeaderInBody => true;

		internal override bool FooterInBody => true;

		internal override bool? SummaryRowAfter
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		internal override bool? SummaryColumnAfter
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		internal HeaderFooterLayout(RPLReport report, float aWidth, float aHeight)
			: base(report)
		{
			m_fullList = new List<ReportItemInfo>();
			int num = LayoutConvert.ConvertMMTo20thPoints(aWidth);
			m_height = aHeight;
			m_centerWidth = num / 3;
			m_rightWidth = m_centerWidth * 2;
			m_leftList = new List<ReportItemInfo>();
			m_centerList = new List<ReportItemInfo>();
			m_rightList = new List<ReportItemInfo>();
		}

		internal override void AddReportItem(object rplSource, int top, int left, int width, int height, int generationIndex, byte state, string subreportLanguage, Dictionary<string, ToggleParent> toggleParents)
		{
			ReportItemInfo item = new ReportItemInfo(rplSource, top, left, left + width, (state & 0x20) != 0, toggleParents);
			m_fullList.Add(item);
		}

		internal override void AddStructuralItem(int top, int left, int width, int height, bool isToggglable, int generationIndex, RPLTablixMemberCell member, TogglePosition togglePosition)
		{
		}

		internal override void AddStructuralItem(int top, int left, int width, int height, int generationIndex, int rowHeaderWidth, int columnHeaderHeight, bool rtl)
		{
		}

		internal override ALayout GetPageHeaderLayout(float aWidth, float aHeight)
		{
			return this;
		}

		internal override ALayout GetPageFooterLayout(float aWidth, float aHeight)
		{
			return this;
		}

		internal override void CompleteSection()
		{
		}

		internal override void CompletePage()
		{
		}

		internal override void SetIsLastSection(bool isLastSection)
		{
		}

		internal void RenderStrings(RPLReport report, IExcelGenerator excel, out string left, out string center, out string right)
		{
			foreach (ReportItemInfo full in m_fullList)
			{
				RPLPageLayout rPLPageLayout = full.RPLSource as RPLPageLayout;
				if (rPLPageLayout != null)
				{
					continue;
				}
				RPLTextBox rPLTextBox = full.RPLSource as RPLTextBox;
				RPLItemProps rPLItemProps;
				byte elementType;
				if (rPLTextBox != null)
				{
					if (rPLTextBox.StartOffset > 0)
					{
						rPLItemProps = m_report.GetItemProps(rPLTextBox.StartOffset, out elementType);
					}
					else
					{
						rPLItemProps = (RPLItemProps)rPLTextBox.ElementProps;
						elementType = 7;
					}
				}
				else
				{
					rPLItemProps = m_report.GetItemProps(full.RPLSource, out elementType);
				}
				if (elementType == 7)
				{
					full.Values = (RPLTextBoxProps)rPLItemProps;
					RPLElementStyle style = rPLItemProps.Style;
					HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left;
					object obj = style[25];
					if (obj != null)
					{
						horizontalAlignment = LayoutConvert.ToHorizontalAlignEnum((RPLFormat.TextAlignments)obj);
					}
					int num = 0;
					int num2 = 0;
					string text = (string)rPLItemProps.Style[15];
					string text2 = (string)rPLItemProps.Style[16];
					if (text != null)
					{
						num = LayoutConvert.ConvertMMTo20thPoints(LayoutConvert.ToMillimeters(text));
					}
					if (text2 != null)
					{
						num2 = LayoutConvert.ConvertMMTo20thPoints(LayoutConvert.ToMillimeters(text2));
					}
					switch (horizontalAlignment)
					{
					case HorizontalAlignment.Left:
						full.AlignmentPoint = full.Left + num;
						break;
					case HorizontalAlignment.Right:
						full.AlignmentPoint = full.Right - num2;
						break;
					default:
						full.AlignmentPoint = full.Left + (full.Right - full.Left + num - num2) / 2;
						break;
					}
					if (full.AlignmentPoint < m_centerWidth)
					{
						m_leftList.Add(full);
					}
					else if (full.AlignmentPoint < m_rightWidth)
					{
						m_centerList.Add(full);
					}
					else
					{
						m_rightList.Add(full);
					}
				}
			}
			m_leftList.Sort(ReportItemInfo.CompareTopsThenLefts);
			m_centerList.Sort(ReportItemInfo.CompareTopsThenLefts);
			m_rightList.Sort(ReportItemInfo.CompareTopsThenLefts);
			left = RenderString(m_leftList, excel);
			center = RenderString(m_centerList, excel);
			right = RenderString(m_rightList, excel);
		}

		private string RenderString(List<ReportItemInfo> list, IExcelGenerator excel)
		{
			StringBuilder stringBuilder = new StringBuilder();
			HeaderFooterRichTextInfo headerFooterRichTextInfo = null;
			double lastFontSize = 0.0;
			string lastFont = string.Empty;
			bool flag = false;
			foreach (ReportItemInfo item in list)
			{
				if (stringBuilder.Length > 0 && !stringBuilder[stringBuilder.Length - 1].Equals("\n"))
				{
					stringBuilder.Append("\n");
				}
				if ((item.Values.Definition as RPLTextBoxPropsDef).IsSimple)
				{
					if (flag)
					{
						headerFooterRichTextInfo.CompleteCurrentFormatting();
					}
					excel.BuildHeaderFooterString(stringBuilder, item.Values, ref lastFont, ref lastFontSize);
					flag = false;
					continue;
				}
				flag = true;
				RPLTextBox rPLTextBox = (RPLTextBox)item.RPLSource;
				if (headerFooterRichTextInfo == null)
				{
					headerFooterRichTextInfo = new HeaderFooterRichTextInfo(stringBuilder);
				}
				HorizontalAlignment horizontalAlign = HorizontalAlignment.General;
				bool renderListPrefixes = true;
				object obj = rPLTextBox.ElementProps.Style[29];
				if (obj != null)
				{
					renderListPrefixes = ((RPLFormat.Directions)obj == RPLFormat.Directions.LTR);
				}
				LayoutEngine.RenderRichText(null, rPLTextBox, headerFooterRichTextInfo, inHeaderAndFooter: true, null, renderListPrefixes, ref horizontalAlign);
				lastFontSize = headerFooterRichTextInfo.LastFontSize;
				lastFont = headerFooterRichTextInfo.LastFontName;
				headerFooterRichTextInfo.CompleteRun();
			}
			return stringBuilder.ToString(0, Math.Min(stringBuilder.Length, 256));
		}
	}
}
