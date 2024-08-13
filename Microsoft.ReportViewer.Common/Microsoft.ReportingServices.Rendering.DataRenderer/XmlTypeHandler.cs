using System;
using System.Collections;
using Microsoft.ReportingServices.OnDemandReportRendering;

namespace Microsoft.ReportingServices.Rendering.DataRenderer;

internal class XmlTypeHandler : HandlerBase
{
	private bool m_AllTextboxTypesResolved = true;

	private Hashtable m_TextBoxTypes = new Hashtable();

	public override bool Done => m_AllTextboxTypesResolved;

	public Hashtable TextBoxTypes => m_TextBoxTypes;

	public override void OnTextBoxBegin(TextBox textBox, bool output, ref bool render)
	{
		render = false;
		if (textBox == null || textBox.DataElementOutput == DataElementOutputTypes.NoOutput)
		{
			return;
		}
		TypeCode typeCode = TypeCode.Object;
		ReportStringProperty reportStringProperty = null;
		if (textBox.IsSimple)
		{
			reportStringProperty = textBox.Paragraphs[0].TextRuns[0].Value;
		}
		if (reportStringProperty == null || !reportStringProperty.IsExpression)
		{
			typeCode = textBox.SharedTypeCode;
		}
		else if (textBox.Instance is TextBoxInstance textBoxInstance)
		{
			if (textBoxInstance.OriginalValue == null)
			{
				m_AllTextboxTypesResolved = false;
				return;
			}
			typeCode = Type.GetTypeCode(textBoxInstance.OriginalValue.GetType());
		}
		if (!m_TextBoxTypes.Contains(textBox.ID))
		{
			m_TextBoxTypes.Add(textBox.ID, typeCode);
		}
	}

	public override void OnRowBegin()
	{
		m_AllTextboxTypesResolved = true;
	}
}
