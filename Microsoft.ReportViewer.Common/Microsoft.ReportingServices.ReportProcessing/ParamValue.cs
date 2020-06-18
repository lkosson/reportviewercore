using Microsoft.ReportingServices.Diagnostics.Utilities;
using System.Xml;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal sealed class ParamValue
	{
		internal const string _ParameterValue = "ParameterValue";

		private const string _Name = "Name";

		private const string _Value = "Value";

		private const string _Field = "Field";

		private ParamValues m_parent;

		private string m_name;

		private string m_value;

		private string m_fieldName;

		public string Name => m_name;

		public string Value => m_value;

		public string FieldName => m_fieldName;

		public bool UseField => FieldName != null;

		public bool UseValue => !UseField;

		public string FieldValue
		{
			get
			{
				Global.Tracer.Assert(UseField, "Trying to get a field value for a non field setting.");
				return m_parent.GetFieldValue(FieldName);
			}
		}

		public bool IsValid()
		{
			if (UseField && Value != null)
			{
				return false;
			}
			return true;
		}

		public ParamValue(XmlReader reader, ParamValues parent)
		{
			m_parent = parent;
			while (reader.Read() && (reader.NodeType != XmlNodeType.EndElement || !(reader.Name == "ParameterValue")))
			{
				if (reader.IsStartElement("Name"))
				{
					m_name = reader.ReadString();
				}
				else if (reader.IsStartElement("Value"))
				{
					m_value = reader.ReadString();
				}
				else if (reader.IsStartElement("Field"))
				{
					m_fieldName = reader.ReadString();
					m_parent.AddField(FieldName);
				}
				else if (reader.NodeType == XmlNodeType.Element)
				{
					throw new InvalidXmlException();
				}
			}
			if (!IsValid())
			{
				throw new InvalidXmlException();
			}
		}

		internal void ToXml(XmlTextWriter writer, bool outputFieldElements)
		{
			writer.WriteStartElement("ParameterValue");
			writer.WriteElementString("Name", Name);
			if (UseField)
			{
				if (outputFieldElements)
				{
					writer.WriteElementString("Field", FieldName);
				}
				else if (FieldValue != null)
				{
					writer.WriteElementString("Value", FieldValue);
				}
			}
			else if (Value != null)
			{
				writer.WriteElementString("Value", Value);
			}
			writer.WriteEndElement();
		}

		internal void ToOldParameterXml(XmlTextWriter writer)
		{
			writer.WriteStartElement("Parameter");
			writer.WriteElementString("Name", Name);
			if (Value != null)
			{
				writer.WriteElementString("Value", Value);
			}
			writer.WriteEndElement();
		}
	}
}
