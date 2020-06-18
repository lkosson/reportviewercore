using Microsoft.ReportingServices.Common;
using System.Globalization;

namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLReportSize
	{
		private string m_size;

		private double m_sizeInMM;

		public RPLReportSize(string size)
		{
			m_size = size;
			ParseSize();
		}

		public RPLReportSize(double sizeInMM)
		{
			m_size = sizeInMM.ToString(CultureInfo.InvariantCulture) + "mm";
			m_sizeInMM = sizeInMM;
		}

		public override string ToString()
		{
			return m_size;
		}

		public double ToMillimeters()
		{
			return m_sizeInMM;
		}

		public double ToInches()
		{
			return m_sizeInMM / 25.4;
		}

		public double ToPoints()
		{
			return m_sizeInMM / 0.3528;
		}

		public double ToCentimeters()
		{
			return m_sizeInMM / 10.0;
		}

		internal void ParseSize()
		{
			m_sizeInMM = RVUnit.Parse(m_size, CultureInfo.InvariantCulture).ToMillimeters();
		}
	}
}
