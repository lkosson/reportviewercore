using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.HPBProcessing
{
	internal sealed class Report
	{
		private Microsoft.ReportingServices.OnDemandReportRendering.Report m_report;

		private PageContext m_pageContext;

		private PaginationSettings m_pageSettings;

		private long m_offset;

		private long m_pageOffset;

		private List<ReportSection> m_sections = new List<ReportSection>();

		private Version m_rplVersion = new Version(10, 6, 0);

		internal bool Done
		{
			get
			{
				if (m_sections.Count == 0)
				{
					return true;
				}
				return false;
			}
		}

		internal IJobContext JobContext => m_report.JobContext;

		internal string InitialPageName => m_report.Instance.InitialPageName;

		internal Report(Microsoft.ReportingServices.OnDemandReportRendering.Report report, PageContext pageContext, PaginationSettings aPagination)
		{
			m_report = report;
			m_pageContext = pageContext;
			m_pageSettings = aPagination;
		}

		internal static string GetReportLanguage(Microsoft.ReportingServices.OnDemandReportRendering.Report report)
		{
			string result = null;
			ReportStringProperty language = report.Language;
			if (language != null)
			{
				result = ((!language.IsExpression) ? language.Value : report.Instance.Language);
			}
			return result;
		}

		internal void SetContext()
		{
			ReportSectionCollection reportSections = m_report.ReportSections;
			for (int i = 0; i < reportSections.Count; i++)
			{
				ReportSection reportSection = new ReportSection(reportSections[i], m_pageContext, m_pageSettings, m_pageSettings.SectionPaginationSettings[i]);
				reportSection.SetContext();
				m_sections.Add(reportSection);
			}
		}

		internal void NextPage(RPLWriter rplWriter, int totalPages)
		{
			WriteStartItemToStream(rplWriter);
			double num = 0.0;
			ReportSection reportSection = m_sections[0];
			_ = m_sections.Count;
			double num2 = 0.0;
			List<ReportSection> list = new List<ReportSection>();
			List<ReportSection> list2 = new List<ReportSection>();
			bool isFirstSectionOnPage = true;
			RoundedDouble roundedDouble = new RoundedDouble(m_pageContext.UsablePageHeight);
			for (int i = 0; i < m_sections.Count; i++)
			{
				ReportSection reportSection2 = m_sections[i];
				ReportSection nextSection = null;
				if (i < m_sections.Count - 1)
				{
					nextSection = m_sections[i + 1];
				}
				double num3 = reportSection2.NextPage(rplWriter, m_pageContext.PageNumber, totalPages, num, roundedDouble.Value - num, nextSection, isFirstSectionOnPage);
				if (num3 > 0.0)
				{
					num += num3;
					list2.Add(reportSection2);
				}
				num2 = Math.Max(num2, reportSection2.LeftEdge);
				if (reportSection2.Done)
				{
					list.Add(reportSection2);
				}
				if (roundedDouble == num)
				{
					break;
				}
				isFirstSectionOnPage = false;
			}
			if (num2 == 0.0)
			{
				foreach (ReportSection item in list)
				{
					m_sections.Remove(item);
				}
			}
			if (rplWriter != null)
			{
				reportSection.WriteDelayedSectionHeader(rplWriter, Done);
				WriteEndItemToStream(rplWriter, list2);
			}
			if (m_pageContext.PropertyCacheState == PageContext.CacheState.RPLStream)
			{
				m_pageContext.ItemPropsStart = null;
				m_pageContext.SharedImages = null;
			}
		}

		internal void WriteStartItemToStream(RPLWriter rplWriter)
		{
			if (rplWriter == null)
			{
				return;
			}
			BinaryWriter binaryWriter = rplWriter.BinaryWriter;
			string reportLanguage = GetReportLanguage(m_report);
			if (binaryWriter != null)
			{
				Stream baseStream = binaryWriter.BaseStream;
				m_offset = baseStream.Position;
				binaryWriter.Write((byte)0);
				binaryWriter.Write((byte)2);
				if (m_report.Name != null)
				{
					binaryWriter.Write((byte)15);
					binaryWriter.Write(m_report.Name);
				}
				if (m_report.Description != null)
				{
					binaryWriter.Write((byte)9);
					binaryWriter.Write(m_report.Description);
				}
				if (m_report.Author != null)
				{
					binaryWriter.Write((byte)13);
					binaryWriter.Write(m_report.Author);
				}
				if (m_report.AutoRefresh > 0)
				{
					binaryWriter.Write((byte)14);
					binaryWriter.Write(m_report.AutoRefresh);
				}
				DateTime executionTime = m_report.ExecutionTime;
				binaryWriter.Write((byte)12);
				binaryWriter.Write(executionTime.ToBinary());
				ReportUrl location = m_report.Location;
				if (location != null)
				{
					binaryWriter.Write((byte)10);
					binaryWriter.Write(location.ToString());
				}
				if (reportLanguage != null)
				{
					binaryWriter.Write((byte)11);
					binaryWriter.Write(reportLanguage);
				}
				binaryWriter.Write(byte.MaxValue);
				m_pageOffset = baseStream.Position;
				binaryWriter.Write((byte)19);
				binaryWriter.Write((byte)3);
				binaryWriter.Write((byte)16);
				binaryWriter.Write((float)m_pageSettings.PhysicalPageHeight);
				binaryWriter.Write((byte)17);
				binaryWriter.Write((float)m_pageSettings.PhysicalPageWidth);
				binaryWriter.Write((byte)20);
				binaryWriter.Write((float)m_pageSettings.MarginBottom);
				binaryWriter.Write((byte)19);
				binaryWriter.Write((float)m_pageSettings.MarginLeft);
				binaryWriter.Write((byte)21);
				binaryWriter.Write((float)m_pageSettings.MarginRight);
				binaryWriter.Write((byte)18);
				binaryWriter.Write((float)m_pageSettings.MarginTop);
				new ReportSectionPage(m_report.ReportSections[0].Page).WriteItemStyle(rplWriter, m_pageContext);
				binaryWriter.Write(byte.MaxValue);
			}
			else
			{
				RPLReport rPLReport = new RPLReport();
				rPLReport.ReportName = m_report.Name;
				rPLReport.Description = m_report.Description;
				rPLReport.Author = m_report.Author;
				rPLReport.AutoRefresh = m_report.AutoRefresh;
				rPLReport.ExecutionTime = m_report.ExecutionTime;
				rPLReport.Location = m_report.Location.ToString();
				rPLReport.Language = reportLanguage;
				rPLReport.RPLVersion = m_rplVersion;
				rPLReport.RPLPaginatedPages = new RPLPageContent[1];
				rplWriter.Report = rPLReport;
			}
		}

		internal void WriteEndItemToStream(RPLWriter rplWriter, List<ReportSection> sectionsOnPage)
		{
			BinaryWriter binaryWriter = rplWriter.BinaryWriter;
			if (binaryWriter != null)
			{
				rplWriter.BinaryWriter.Write(byte.MaxValue);
				Stream baseStream = binaryWriter.BaseStream;
				long position = baseStream.Position;
				binaryWriter.Write((byte)16);
				binaryWriter.Write(m_pageOffset);
				int count = sectionsOnPage.Count;
				binaryWriter.Write(count);
				for (int i = 0; i < count; i++)
				{
					sectionsOnPage[i].WriteMeasurement(binaryWriter);
				}
				m_pageOffset = baseStream.Position;
				binaryWriter.Write((byte)254);
				binaryWriter.Write(position);
				binaryWriter.Write(byte.MaxValue);
				position = baseStream.Position;
				binaryWriter.Write((byte)18);
				binaryWriter.Write(m_offset);
				binaryWriter.Write(1);
				binaryWriter.Write(m_pageOffset);
				binaryWriter.Write((byte)254);
				binaryWriter.Write(position);
				binaryWriter.Write(byte.MaxValue);
			}
			else
			{
				int count2 = sectionsOnPage.Count;
				RPLPageLayout pageLayout = sectionsOnPage[0].WritePageLayout(rplWriter, m_pageContext);
				RPLPageContent rPLPageContent = new RPLPageContent(count2, pageLayout);
				for (int j = 0; j < count2; j++)
				{
					RPLMeasurement measurement = null;
					sectionsOnPage[j].AddToPage(rPLPageContent, out measurement);
					rPLPageContent.ReportSectionSizes[j] = measurement;
				}
				rplWriter.Report.RPLPaginatedPages[0] = rPLPageContent;
			}
		}
	}
}
