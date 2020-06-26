using Microsoft.ReportingServices.Rendering.ImageRenderer;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Microsoft.Reporting.NETCore
{
	internal sealed class RenderingReport
	{
		private RectangleF m_position;

		private List<FixedHeaderItem> m_fixedHeaders;

		private List<Action> m_actions;

		private Dictionary<string, LabelPoint> m_labels;

		private Dictionary<string, BookmarkPoint> m_bookmarks;

		private List<Action> m_toolTips;

		private List<RenderingReportSection> m_reportSections;

		internal Color BackgroundColor;

		internal Image BackgroundImage;

		internal RenderingItemBorder BorderLeft;

		internal RenderingItemBorder BorderRight;

		internal RenderingItemBorder BorderTop;

		internal RenderingItemBorder BorderBottom;

		internal RPLFormat.BackgroundRepeatTypes BackgroundRepeat = RPLFormat.BackgroundRepeatTypes.Clip;

		internal RectangleF Position => m_position;

		internal List<Action> Actions
		{
			get
			{
				if (m_actions == null)
				{
					m_actions = new List<Action>();
				}
				return m_actions;
			}
		}

		internal List<FixedHeaderItem> FixedHeaders
		{
			get
			{
				if (m_fixedHeaders == null)
				{
					m_fixedHeaders = new List<FixedHeaderItem>();
				}
				return m_fixedHeaders;
			}
		}

		internal Dictionary<string, BookmarkPoint> Bookmarks
		{
			get
			{
				if (m_bookmarks == null)
				{
					m_bookmarks = new Dictionary<string, BookmarkPoint>();
				}
				return m_bookmarks;
			}
		}

		internal List<Action> ToolTips
		{
			get
			{
				if (m_toolTips == null)
				{
					m_toolTips = new List<Action>();
				}
				return m_toolTips;
			}
		}

		internal Dictionary<string, LabelPoint> Labels
		{
			get
			{
				if (m_labels == null)
				{
					m_labels = new Dictionary<string, LabelPoint>();
				}
				return m_labels;
			}
		}

		internal List<RenderingReportSection> ReportSections => m_reportSections;

		internal RenderingReport(GdiContext context)
		{
			RPLPageContent rPLPageContent = context.RplReport.RPLPaginatedPages[0];
			RPLPageLayout pageLayout = rPLPageContent.PageLayout;
			context.RenderingReport = this;
			float maxSectionWidth = rPLPageContent.MaxSectionWidth;
			m_position.X = float.MaxValue;
			m_position.Y = 0f;
			m_position.Width = maxSectionWidth;
			m_position.Height = 0f;
			m_reportSections = new List<RenderingReportSection>(rPLPageContent.ReportSectionSizes.Length);
			int num = 0;
			while (rPLPageContent.HasNextReportSection())
			{
				RPLReportSection nextReportSection = rPLPageContent.GetNextReportSection();
				RPLSizes rPLSizes = rPLPageContent.ReportSectionSizes[num];
				m_reportSections.Add(new RenderingReportSection(context, nextReportSection, rPLSizes, num, maxSectionWidth, m_position.Height));
				m_position.X = Math.Min(m_position.X, rPLSizes.Left);
				m_position.Height += rPLSizes.Height;
				num++;
			}
			object obj = pageLayout.Style[34];
			if (obj != null)
			{
				BackgroundColor = new RPLReportColor((string)obj).ToColor();
			}
			object obj2 = pageLayout.Style[33];
			if (obj2 != null)
			{
				BackgroundImage = RenderingItem.GetImage(context, (RPLImageData)obj2);
				object obj3 = pageLayout.Style[35];
				if (obj3 == null)
				{
					BackgroundRepeat = RPLFormat.BackgroundRepeatTypes.Repeat;
				}
				else
				{
					BackgroundRepeat = (RPLFormat.BackgroundRepeatTypes)obj3;
				}
			}
			ProcessBorders(context.GdiWriter, pageLayout.Style, m_position, m_position, 0);
		}

		private void ProcessBorders(GdiWriter writer, RPLElementStyle style, RectangleF position, RectangleF bounds, byte state)
		{
			RPLFormat.BorderStyles stylePropertyValueBorderStyle = SharedRenderer.GetStylePropertyValueBorderStyle(style, 5, RPLFormat.BorderStyles.None);
			BorderLeft.Style = SharedRenderer.GetStylePropertyValueBorderStyle(style, 6, stylePropertyValueBorderStyle);
			BorderTop.Style = SharedRenderer.GetStylePropertyValueBorderStyle(style, 8, stylePropertyValueBorderStyle);
			BorderRight.Style = SharedRenderer.GetStylePropertyValueBorderStyle(style, 7, stylePropertyValueBorderStyle);
			BorderBottom.Style = SharedRenderer.GetStylePropertyValueBorderStyle(style, 9, stylePropertyValueBorderStyle);
			if (BorderLeft.Style == RPLFormat.BorderStyles.None && BorderTop.Style == RPLFormat.BorderStyles.None && BorderRight.Style == RPLFormat.BorderStyles.None && BorderBottom.Style == RPLFormat.BorderStyles.None)
			{
				return;
			}
			float reportSizeStyleMM = SharedRenderer.GetReportSizeStyleMM(style, 10);
			BorderLeft.Width = SharedRenderer.GetReportSizeStyleMM(style, 11);
			if (float.IsNaN(BorderLeft.Width) && !float.IsNaN(reportSizeStyleMM))
			{
				BorderLeft.Width = reportSizeStyleMM;
			}
			BorderTop.Width = SharedRenderer.GetReportSizeStyleMM(style, 13);
			if (float.IsNaN(BorderTop.Width) && !float.IsNaN(reportSizeStyleMM))
			{
				BorderTop.Width = reportSizeStyleMM;
			}
			BorderRight.Width = SharedRenderer.GetReportSizeStyleMM(style, 12);
			if (float.IsNaN(BorderRight.Width) && !float.IsNaN(reportSizeStyleMM))
			{
				BorderRight.Width = reportSizeStyleMM;
			}
			BorderBottom.Width = SharedRenderer.GetReportSizeStyleMM(style, 14);
			if (float.IsNaN(BorderBottom.Width) && !float.IsNaN(reportSizeStyleMM))
			{
				BorderBottom.Width = reportSizeStyleMM;
			}
			if (!float.IsNaN(BorderLeft.Width) || !float.IsNaN(BorderTop.Width) || !float.IsNaN(BorderRight.Width) || !float.IsNaN(BorderBottom.Width))
			{
				Color reportColorStyle = SharedRenderer.GetReportColorStyle(style, 0);
				BorderLeft.Color = SharedRenderer.GetReportColorStyle(style, 1);
				if (BorderLeft.Color == Color.Empty && reportColorStyle != Color.Empty)
				{
					BorderLeft.Color = reportColorStyle;
				}
				BorderTop.Color = SharedRenderer.GetReportColorStyle(style, 3);
				if (BorderTop.Color == Color.Empty && reportColorStyle != Color.Empty)
				{
					BorderTop.Color = reportColorStyle;
				}
				BorderRight.Color = SharedRenderer.GetReportColorStyle(style, 2);
				if (BorderRight.Color == Color.Empty && reportColorStyle != Color.Empty)
				{
					BorderRight.Color = reportColorStyle;
				}
				BorderBottom.Color = SharedRenderer.GetReportColorStyle(style, 4);
				if (BorderBottom.Color == Color.Empty && reportColorStyle != Color.Empty)
				{
					BorderBottom.Color = reportColorStyle;
				}
				if (!(BorderLeft.Color == Color.Empty) || !(BorderTop.Color == Color.Empty) || !(BorderRight.Color == Color.Empty) || !(BorderBottom.Color == Color.Empty))
				{
					RenderingItem.ProcessBorders(writer, ref BorderTop, ref BorderLeft, ref BorderBottom, ref BorderRight, position, bounds, state);
				}
			}
		}

		internal void DrawPageBorders(GdiContext context)
		{
			if (BorderTop.IsVisible && BorderTop.Operations != null)
			{
				for (int i = 0; i < BorderTop.Operations.Count; i++)
				{
					BorderTop.Operations[i].Perform(context.GdiWriter);
				}
			}
			if (BorderLeft.IsVisible && BorderLeft.Operations != null)
			{
				for (int j = 0; j < BorderLeft.Operations.Count; j++)
				{
					BorderLeft.Operations[j].Perform(context.GdiWriter);
				}
			}
			if (BorderBottom.IsVisible && BorderBottom.Operations != null)
			{
				for (int k = 0; k < BorderBottom.Operations.Count; k++)
				{
					BorderBottom.Operations[k].Perform(context.GdiWriter);
				}
			}
			if (BorderRight.IsVisible && BorderRight.Operations != null)
			{
				for (int l = 0; l < BorderRight.Operations.Count; l++)
				{
					BorderRight.Operations[l].Perform(context.GdiWriter);
				}
			}
		}

		internal void DrawToPage(GdiContext context)
		{
			if (BackgroundColor != Color.Empty)
			{
				using (SolidBrush brush = new SolidBrush(BackgroundColor))
				{
					context.Graphics.FillRectangle(brush, Position);
				}
			}
			if (BackgroundImage != null)
			{
				RenderingItem.DrawBackgroundImage(context, BackgroundImage, BackgroundRepeat, Position);
			}
			foreach (RenderingReportSection reportSection in m_reportSections)
			{
				reportSection.DrawToPage(context);
			}
			DrawPageBorders(context);
		}

		internal void Search(GdiContext context)
		{
			foreach (RenderingReportSection reportSection in m_reportSections)
			{
				reportSection.Search(context);
			}
		}
	}
}
