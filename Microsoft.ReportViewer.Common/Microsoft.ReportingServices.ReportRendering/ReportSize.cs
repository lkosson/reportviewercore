using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class ReportSize
	{
		private string m_size;

		private double m_sizeInMM;

		private bool m_parsed;

		internal bool Parsed => m_parsed;

		public ReportSize(string size)
		{
			m_size = size;
			Validate();
			m_parsed = true;
		}

		internal ReportSize(string size, double sizeInMM)
		{
			m_size = size;
			m_sizeInMM = sizeInMM;
			m_parsed = true;
		}

		internal ReportSize(ReportSize newSize)
		{
			m_size = newSize.ToString();
			m_sizeInMM = newSize.ToMillimeters();
			m_parsed = true;
		}

		internal ReportSize(string size, bool parsed)
		{
			m_size = size;
			m_parsed = parsed;
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
			return m_sizeInMM / 0.3528;
		}

		public double ToCentimeters()
		{
			ParseSize();
			return m_sizeInMM / 10.0;
		}

		internal void ParseSize()
		{
			if (!m_parsed)
			{
				Validator.ParseSize(m_size, out m_sizeInMM);
				m_parsed = true;
			}
		}

		internal void Validate()
		{
			if (!Validator.ValidateSizeString(m_size, out RVUnit sizeValue))
			{
				throw new ReportRenderingException(ErrorCode.rrInvalidSize, m_size);
			}
			if (!Validator.ValidateSizeUnitType(sizeValue))
			{
				throw new ReportRenderingException(ErrorCode.rrInvalidMeasurementUnit, m_size);
			}
			if (!Validator.ValidateSizeIsPositive(sizeValue))
			{
				throw new ReportRenderingException(ErrorCode.rrNegativeSize, m_size);
			}
			double sizeInMM = Converter.ConvertToMM(sizeValue);
			if (!Validator.ValidateSizeValue(sizeInMM, Validator.NormalMin, Validator.NormalMax))
			{
				throw new ReportRenderingException(ErrorCode.rrOutOfRange, m_size);
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
	}
}
