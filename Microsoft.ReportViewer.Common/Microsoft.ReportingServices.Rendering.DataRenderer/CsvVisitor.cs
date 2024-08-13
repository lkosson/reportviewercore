using System.IO;
using System.Text;

namespace Microsoft.ReportingServices.Rendering.DataRenderer;

public class CsvVisitor
{
	private StreamWriter m_writer;

	private string m_fieldDelimiter;

	private string m_recordDelimiter;

	private string m_qualifier;

	private bool m_suppressLineBreaks;

	private char[] m_specialCharacters;

	private Encoding m_encoding;

	private bool m_atRowStart = true;

	public CsvVisitor(Stream outputStream, string fieldDelimiter, string recordDelimiter, string qualifier, bool suppressLineBreaks, Encoding encoding)
	{
		m_writer = new StreamWriter(outputStream, encoding);
		m_writer.AutoFlush = true;
		m_fieldDelimiter = fieldDelimiter;
		m_recordDelimiter = recordDelimiter;
		m_qualifier = qualifier;
		m_suppressLineBreaks = suppressLineBreaks;
		m_encoding = encoding;
		m_specialCharacters = new char[6] { '"', '\r', '\n', ',', '"', '\r' };
		if (m_fieldDelimiter.Length > 0)
		{
			m_specialCharacters[3] = m_fieldDelimiter[0];
		}
		if (m_qualifier.Length > 0)
		{
			m_specialCharacters[4] = m_qualifier[0];
		}
		if (m_recordDelimiter.Length > 0)
		{
			m_specialCharacters[5] = m_recordDelimiter[0];
		}
	}

	internal void EndRow()
	{
		m_writer.Write(m_recordDelimiter);
		m_atRowStart = true;
	}

	internal void EndRegion()
	{
		m_writer.Write(m_recordDelimiter);
	}

	internal void WriteValue(string unformattedValue, bool excelMode)
	{
		if (!m_atRowStart)
		{
			m_writer.Write(m_fieldDelimiter);
		}
		m_atRowStart = false;
		m_writer.Write(FormatValue(unformattedValue, excelMode));
	}

	private string FormatValue(string value, bool excelMode)
	{
		if (value == null)
		{
			return string.Empty;
		}
		if (excelMode)
		{
			value = value.Replace("\t", " ");
		}
		if (m_suppressLineBreaks)
		{
			value = value.Replace("\r", string.Empty).Replace("\n", string.Empty);
		}
		if (value.IndexOfAny(m_specialCharacters) != -1)
		{
			value = value.Replace(m_qualifier, m_qualifier + m_qualifier);
			value = m_qualifier + value + m_qualifier;
		}
		return value;
	}
}
