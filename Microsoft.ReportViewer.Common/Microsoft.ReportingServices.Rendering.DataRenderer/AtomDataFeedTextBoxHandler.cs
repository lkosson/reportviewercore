using System;
using Microsoft.ReportingServices.OnDemandReportRendering;

namespace Microsoft.ReportingServices.Rendering.DataRenderer;

internal class AtomDataFeedTextBoxHandler : HandlerBase
{
	protected AtomDataFeedVisitor m_visitor;

	internal AtomDataFeedTextBoxHandler(AtomDataFeedVisitor visitor)
	{
		m_visitor = visitor;
	}

	public override void OnTextBoxBegin(TextBox textBox, bool output, ref bool walkTextBox)
	{
		if (textBox.DataElementOutput == DataElementOutputTypes.NoOutput)
		{
			return;
		}
		walkTextBox = true;
		if (output)
		{
			m_visitor.AddColumnName(textBox.DataElementName);
			object obj = null;
			TypeCode typeCode = TypeCode.String;
			if (textBox.Instance != null)
			{
				TextBoxInstance textBoxInstance = (TextBoxInstance)textBox.Instance;
				typeCode = textBoxInstance.TypeCode;
				obj = textBoxInstance.OriginalValue;
			}
			string value = null;
			if (obj != null)
			{
				value = AtomDataFeedHandler.XmlConvertOrginalValue(obj);
			}
			m_visitor.WriteFeedLevelValue(value, typeCode);
		}
	}
}
