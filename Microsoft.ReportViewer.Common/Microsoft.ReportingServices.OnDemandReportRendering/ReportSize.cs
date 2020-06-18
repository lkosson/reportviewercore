using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportPublishing;
using Microsoft.ReportingServices.ReportRendering;
using System;
using System.Globalization;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ReportSize
	{
		private const string m_zeroMM = "0mm";

		private string m_size;

		private double m_sizeInMM;

		private bool m_parsed;

		private bool m_allowNegative;

		public ReportSize(string size)
			: this(size, validate: true, allowNegative: false)
		{
		}

		public ReportSize(string size, bool allowNegative)
			: this(size, validate: true, allowNegative)
		{
		}

		internal ReportSize(string size, bool validate, bool allowNegative)
		{
			if (string.IsNullOrEmpty(size))
			{
				m_size = "0mm";
			}
			else
			{
				m_size = size;
			}
			m_allowNegative = allowNegative;
			if (validate)
			{
				Validate();
				m_parsed = true;
			}
			else
			{
				m_parsed = false;
			}
		}

		internal ReportSize(string size, double sizeInMM)
		{
			m_sizeInMM = sizeInMM;
			m_parsed = true;
			if (string.IsNullOrEmpty(size))
			{
				m_size = ConvertToMM(m_sizeInMM);
			}
			else
			{
				m_size = size;
			}
		}

		internal ReportSize(Microsoft.ReportingServices.ReportRendering.ReportSize oldSize)
		{
			m_size = oldSize.ToString();
			m_parsed = oldSize.Parsed;
			if (m_parsed)
			{
				m_sizeInMM = oldSize.ToMillimeters();
			}
		}

		private ReportSize()
		{
		}

		public override string ToString()
		{
			return m_size;
		}

		public double ToMillimeters()
		{
			ParseSize();
			return m_sizeInMM;
		}

		public double ToInches()
		{
			ParseSize();
			return m_sizeInMM / 25.4;
		}

		public double ToPoints()
		{
			ParseSize();
			return m_sizeInMM / 0.35277777777777775;
		}

		public double ToCentimeters()
		{
			ParseSize();
			return m_sizeInMM / 10.0;
		}

		public static ReportSize SumSizes(ReportSize size1, ReportSize size2)
		{
			if (size1 == null)
			{
				return size2;
			}
			if (size2 == null)
			{
				return size1;
			}
			return FromMillimeters(size1.ToMillimeters() + size2.ToMillimeters());
		}

		public static ReportSize FromMillimeters(double millimeters)
		{
			return new ReportSize(ConvertToMM(millimeters), millimeters);
		}

		private static string ConvertToMM(double millimeters)
		{
			return Convert.ToString(millimeters, CultureInfo.InvariantCulture) + "mm";
		}

		internal void ParseSize()
		{
			if (!m_parsed)
			{
				Microsoft.ReportingServices.ReportPublishing.Validator.ParseSize(m_size, out m_sizeInMM);
				m_parsed = true;
			}
		}

		internal void Validate()
		{
			if (!Microsoft.ReportingServices.ReportPublishing.Validator.ValidateSizeString(m_size, out RVUnit sizeValue))
			{
				throw new RenderingObjectModelException(ErrorCode.rrInvalidSize, m_size);
			}
			if (!Microsoft.ReportingServices.ReportPublishing.Validator.ValidateSizeUnitType(sizeValue))
			{
				throw new RenderingObjectModelException(ErrorCode.rrInvalidMeasurementUnit, m_size);
			}
			if (!m_allowNegative && !Microsoft.ReportingServices.ReportPublishing.Validator.ValidateSizeIsPositive(sizeValue))
			{
				throw new RenderingObjectModelException(ErrorCode.rrNegativeSize, m_size);
			}
			double sizeInMM = Microsoft.ReportingServices.ReportPublishing.Converter.ConvertToMM(sizeValue);
			if (!Microsoft.ReportingServices.ReportPublishing.Validator.ValidateSizeValue(sizeInMM, m_allowNegative ? Microsoft.ReportingServices.ReportPublishing.Validator.NegativeMin : Microsoft.ReportingServices.ReportPublishing.Validator.NormalMin, Microsoft.ReportingServices.ReportPublishing.Validator.NormalMax))
			{
				throw new RenderingObjectModelException(ErrorCode.rrOutOfRange, m_size);
			}
			m_sizeInMM = sizeInMM;
		}

		internal ReportSize DeepClone()
		{
			ReportSize reportSize = new ReportSize();
			if (m_size != null)
			{
				reportSize.m_size = string.Copy(m_size);
			}
			reportSize.m_parsed = m_parsed;
			reportSize.m_sizeInMM = m_sizeInMM;
			return reportSize;
		}

		public static bool TryParse(string value, out ReportSize reportSize)
		{
			return TryParse(value, allowNegative: false, out reportSize);
		}

		public static bool TryParse(string value, bool allowNegative, out ReportSize reportSize)
		{
			if (Microsoft.ReportingServices.ReportPublishing.Validator.ValidateSize(value, allowNegative, out double sizeInMM))
			{
				reportSize = new ReportSize(value, sizeInMM);
				return true;
			}
			reportSize = null;
			return false;
		}
	}
}
