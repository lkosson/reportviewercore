using Microsoft.ReportingServices.OnDemandReportRendering;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;

namespace Microsoft.ReportingServices.Rendering.HPBProcessing
{
	internal sealed class PaginationSettings
	{
		internal enum DeviceInfoTags
		{
			StartPage,
			EndPage,
			PageWidth,
			PageHeight,
			Columns,
			ColumnSpacing,
			MarginTop,
			MarginLeft,
			MarginBottom,
			MarginRight,
			DpiX,
			DpiY,
			OutputFormat,
			ReportItemPath,
			Unknown
		}

		public enum FormatEncoding
		{
			BMP,
			EMF,
			EMFPLUS,
			GIF,
			JPEG,
			PNG,
			TIFF,
			PDF
		}

		public const double MINIMUMCOLUMNHEIGHT = 2.5399999618530273;

		public const double MINIMUMCOLUMNWIDTH = 2.5399999618530273;

		public const double DEFAULTCOLUMNSPACING = 12.699999809265137;

		public const int DEFAULTRESOLUTIONX = 96;

		public const int DEFAULTRESOLUTIONY = 96;

		private int m_startPage;

		private int m_endPage;

		private double m_physicalPageWidth = 215.9;

		private double m_physicalPageHeight = 279.4;

		private double m_usableWidth;

		private double m_usableHeight;

		private double m_currentColumnWidth;

		private double m_currentColumnHeight;

		private double m_marginTop;

		private double m_marginLeft;

		private double m_marginBottom;

		private double m_marginRight;

		private FormatEncoding m_outputFormat = FormatEncoding.TIFF;

		private string m_reportItemPath;

		private int m_dpiX = 96;

		private int m_dpiY = 96;

		private int m_dynamicImageDpiX = 96;

		private int m_dynamicImageDpiY = 96;

		private bool m_useGenericDefault;

		private bool m_useEmSquare;

		private int m_measureTextDpi = 96;

		private int m_measureImageDpiX = 96;

		private int m_measureImageDpiY = 96;

		private SectionPaginationSettings[] m_sectionPaginationSettings;

		private static Hashtable DeviceInfoTagLookup;

		public int StartPage
		{
			get
			{
				return m_startPage;
			}
			set
			{
				m_startPage = value;
			}
		}

		public int EndPage
		{
			get
			{
				return m_endPage;
			}
			set
			{
				m_endPage = value;
			}
		}

		private string PageHeightStr
		{
			set
			{
				m_physicalPageHeight = ParseSize(value, 279.4);
			}
		}

		private string PageWidthStr
		{
			set
			{
				m_physicalPageWidth = ParseSize(value, 215.9);
			}
		}

		internal SectionPaginationSettings[] SectionPaginationSettings => m_sectionPaginationSettings;

		internal double MarginTop
		{
			get
			{
				return m_marginTop;
			}
			set
			{
				m_marginTop = value;
			}
		}

		internal double MarginLeft
		{
			get
			{
				return m_marginLeft;
			}
			set
			{
				m_marginLeft = value;
			}
		}

		internal double MarginBottom
		{
			get
			{
				return m_marginBottom;
			}
			set
			{
				m_marginBottom = value;
			}
		}

		internal double MarginRight
		{
			get
			{
				return m_marginRight;
			}
			set
			{
				m_marginRight = value;
			}
		}

		public int DpiX
		{
			get
			{
				return m_dpiX;
			}
			set
			{
				m_dpiX = value;
			}
		}

		public int DpiY
		{
			get
			{
				return m_dpiY;
			}
			set
			{
				m_dpiY = value;
			}
		}

		public int DynamicImageDpiX
		{
			get
			{
				return m_dynamicImageDpiX;
			}
			set
			{
				m_dynamicImageDpiX = value;
			}
		}

		public int DynamicImageDpiY
		{
			get
			{
				return m_dynamicImageDpiY;
			}
			set
			{
				m_dynamicImageDpiY = value;
			}
		}

		public bool UseGenericDefault
		{
			get
			{
				return m_useGenericDefault;
			}
			set
			{
				m_useGenericDefault = value;
			}
		}

		public double CurrentColumnHeight
		{
			get
			{
				return m_currentColumnHeight;
			}
			set
			{
				m_currentColumnHeight = value;
			}
		}

		public double CurrentColumnWidth
		{
			get
			{
				return m_currentColumnWidth;
			}
			set
			{
				m_currentColumnWidth = value;
			}
		}

		public double UsablePageHeight => m_usableHeight;

		public double UsablePageWidth => m_usableWidth;

		public double PhysicalPageHeight => m_physicalPageHeight;

		public double PhysicalPageWidth => m_physicalPageWidth;

		public FormatEncoding OutputFormat => m_outputFormat;

		public string ReportItemPath
		{
			get
			{
				return m_reportItemPath;
			}
			set
			{
				m_reportItemPath = value;
			}
		}

		public bool UseEmSquare
		{
			get
			{
				return m_useEmSquare;
			}
			set
			{
				m_useEmSquare = value;
			}
		}

		public int MeasureTextDpi
		{
			get
			{
				return m_measureTextDpi;
			}
			set
			{
				m_measureTextDpi = value;
			}
		}

		public int MeasureImageDpiX
		{
			get
			{
				return m_measureImageDpiX;
			}
			set
			{
				m_measureImageDpiX = value;
			}
		}

		public int MeasureImageDpiY
		{
			get
			{
				return m_measureImageDpiY;
			}
			set
			{
				m_measureImageDpiY = value;
			}
		}

		internal bool EMFOutputFormat
		{
			get
			{
				if (m_outputFormat == FormatEncoding.EMF || m_outputFormat == FormatEncoding.EMFPLUS)
				{
					return true;
				}
				return false;
			}
		}

		static PaginationSettings()
		{
			string[] names = Enum.GetNames(typeof(DeviceInfoTags));
			DeviceInfoTagLookup = CollectionsUtil.CreateCaseInsensitiveHashtable(names.Length);
			for (int i = 0; i < names.Length; i++)
			{
				DeviceInfoTagLookup.Add(names[i], Enum.Parse(typeof(DeviceInfoTags), names[i]));
			}
		}

		private void Init(Microsoft.ReportingServices.OnDemandReportRendering.Report report)
		{
			Page page = report.ReportSections[0].Page;
			PageHeightStr = page.PageHeight.ToString();
			PageWidthStr = page.PageWidth.ToString();
			MarginTop = page.TopMargin.ToMillimeters();
			MarginLeft = page.LeftMargin.ToMillimeters();
			MarginBottom = page.BottomMargin.ToMillimeters();
			MarginRight = page.RightMargin.ToMillimeters();
			int count = report.ReportSections.Count;
			m_sectionPaginationSettings = new SectionPaginationSettings[count];
			for (int i = 0; i < count; i++)
			{
				m_sectionPaginationSettings[i] = new SectionPaginationSettings(report.ReportSections[i]);
			}
		}

		public PaginationSettings(Microsoft.ReportingServices.OnDemandReportRendering.Report report)
		{
			Init(report);
			ValidateFields(0, -1.0);
		}

		public PaginationSettings(Microsoft.ReportingServices.OnDemandReportRendering.Report report, NameValueCollection aDeviceInfo)
		{
			Init(report);
			ParseDeviceInfo(aDeviceInfo, out int columns, out double columnSpacing);
			ValidateFields(columns, columnSpacing);
			m_dynamicImageDpiX = m_dpiX;
			m_dynamicImageDpiY = m_dpiY;
		}

		private void ValidateDeviceInfoValue(ref double currValue, double defaultValue)
		{
			if (currValue <= 0.0)
			{
				currValue = defaultValue;
			}
		}

		private void ValidateDeviceInfoValue(ref int currValue, int defaultValue)
		{
			if (currValue <= 0)
			{
				currValue = defaultValue;
			}
		}

		private void ValidateFields(int columns, double columnSpacing)
		{
			if (m_startPage < 0)
			{
				m_startPage = 0;
			}
			if (m_endPage < 0)
			{
				m_endPage = 0;
			}
			if (m_endPage < m_startPage)
			{
				m_endPage = m_startPage;
			}
			else if (m_endPage > m_startPage && m_startPage == 0)
			{
				m_startPage = 1;
			}
			ValidateDeviceInfoValue(ref m_physicalPageWidth, 215.9);
			ValidateDeviceInfoValue(ref m_physicalPageHeight, 279.4);
			if (m_marginTop < 0.0)
			{
				m_marginTop = 0.0;
			}
			if (m_marginBottom < 0.0)
			{
				m_marginBottom = 0.0;
			}
			if (m_marginTop + m_marginBottom >= m_physicalPageHeight)
			{
				m_marginTop = (m_marginBottom = 0.0);
			}
			if (m_marginLeft < 0.0)
			{
				m_marginLeft = 0.0;
			}
			if (m_marginRight < 0.0)
			{
				m_marginRight = 0.0;
			}
			if (m_marginLeft + m_marginRight >= m_physicalPageWidth)
			{
				m_marginLeft = (m_marginRight = 0.0);
			}
			ValidateDeviceInfoValue(ref m_dpiX, 96);
			ValidateDeviceInfoValue(ref m_dpiY, 96);
			double pageWidth = PhysicalPageWidth;
			double pageHeight = PhysicalPageHeight;
			SectionPaginationSettings[] sectionPaginationSettings = m_sectionPaginationSettings;
			for (int i = 0; i < sectionPaginationSettings.Length; i++)
			{
				sectionPaginationSettings[i].Validate(this, columns, columnSpacing, ref pageHeight, ref pageWidth);
			}
			m_physicalPageWidth = pageWidth;
			m_physicalPageHeight = pageHeight;
			m_usableWidth = pageWidth - MarginLeft - MarginRight;
			m_usableHeight = pageHeight - MarginTop - MarginBottom;
			sectionPaginationSettings = m_sectionPaginationSettings;
			for (int i = 0; i < sectionPaginationSettings.Length; i++)
			{
				sectionPaginationSettings[i].SetColumnArea(this);
			}
		}

		private void ParseDeviceInfo(NameValueCollection deviceInfo, out int columns, out double columnSpacing)
		{
			int count = deviceInfo.Count;
			columns = 0;
			columnSpacing = -1.0;
			for (int i = 0; i < count; i++)
			{
				string key = deviceInfo.GetKey(i);
				string text = deviceInfo.Get(i);
				object obj = DeviceInfoTagLookup[key];
				switch ((obj != null) ? ((DeviceInfoTags)obj) : DeviceInfoTags.Unknown)
				{
				case DeviceInfoTags.StartPage:
					m_startPage = ParseInt(text, 0);
					break;
				case DeviceInfoTags.EndPage:
					m_endPage = ParseInt(text, 0);
					break;
				case DeviceInfoTags.PageWidth:
					m_physicalPageWidth = ParseSize(text, m_physicalPageWidth);
					break;
				case DeviceInfoTags.PageHeight:
					m_physicalPageHeight = ParseSize(text, m_physicalPageHeight);
					break;
				case DeviceInfoTags.Columns:
					columns = ParseInt(text, columns);
					break;
				case DeviceInfoTags.ColumnSpacing:
					columnSpacing = ParseSize(text, columnSpacing);
					break;
				case DeviceInfoTags.MarginTop:
					m_marginTop = ParseSize(text, m_marginTop);
					break;
				case DeviceInfoTags.MarginLeft:
					m_marginLeft = ParseSize(text, m_marginLeft);
					break;
				case DeviceInfoTags.MarginBottom:
					m_marginBottom = ParseSize(text, m_marginBottom);
					break;
				case DeviceInfoTags.MarginRight:
					m_marginRight = ParseSize(text, m_marginRight);
					break;
				case DeviceInfoTags.DpiX:
					m_dpiX = ParseInt(text, 96);
					break;
				case DeviceInfoTags.DpiY:
					m_dpiY = ParseInt(text, 96);
					break;
				case DeviceInfoTags.OutputFormat:
					m_outputFormat = ParseFormat(text, FormatEncoding.TIFF);
					break;
				case DeviceInfoTags.ReportItemPath:
					m_reportItemPath = text;
					break;
				}
			}
		}

		internal int ParseInt(string intValue, int defaultValue)
		{
			int result = defaultValue;
			if (!string.IsNullOrEmpty(intValue))
			{
				try
				{
					result = int.Parse(intValue, CultureInfo.InvariantCulture);
					return result;
				}
				catch (FormatException)
				{
					return result;
				}
			}
			return result;
		}

		private double ParseSize(string sizeValue, double defaultValue)
		{
			double result = defaultValue;
			if (sizeValue != null && sizeValue.Length > 0 && ReportSize.TryParse(sizeValue, out ReportSize reportSize))
			{
				result = reportSize.ToMillimeters();
			}
			return result;
		}

		private FormatEncoding ParseFormat(string enumValue, FormatEncoding defaultValue)
		{
			switch (enumValue.Trim().ToUpperInvariant())
			{
			case "BMP":
				return FormatEncoding.BMP;
			case "EMF":
				return FormatEncoding.EMF;
			case "EMFPLUS":
				return FormatEncoding.EMFPLUS;
			case "GIF":
				return FormatEncoding.GIF;
			case "JPEG":
				return FormatEncoding.JPEG;
			case "PNG":
				return FormatEncoding.PNG;
			case "TIFF":
				return FormatEncoding.TIFF;
			default:
				return defaultValue;
			}
		}
	}
}
