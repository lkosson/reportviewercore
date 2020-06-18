using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel.Serialization
{
	internal class RdlSerializerSettings
	{
		private ISerializerHost m_serializerHost;

		private XmlAttributeOverrides m_xmlOverrides;

		private XmlSchema m_xmlSchema;

		private bool m_validate = true;

		private ValidationEventHandler m_validationEventHandler;

		private bool m_normalize = true;

		private bool m_ignoreWhitespace;

		internal ISerializerHost Host
		{
			get
			{
				return m_serializerHost;
			}
			set
			{
				m_serializerHost = value;
			}
		}

		internal XmlAttributeOverrides XmlAttributeOverrides
		{
			get
			{
				return m_xmlOverrides;
			}
			set
			{
				m_xmlOverrides = value;
			}
		}

		internal XmlSchema XmlSchema
		{
			get
			{
				return m_xmlSchema;
			}
			set
			{
				m_xmlSchema = value;
			}
		}

		internal bool ValidateXml
		{
			get
			{
				return m_validate;
			}
			set
			{
				m_validate = value;
			}
		}

		internal ValidationEventHandler XmlValidationEventHandler
		{
			get
			{
				return m_validationEventHandler;
			}
			set
			{
				m_validationEventHandler = value;
			}
		}

		internal bool IgnoreWhitespace
		{
			get
			{
				return m_ignoreWhitespace;
			}
			set
			{
				m_ignoreWhitespace = value;
			}
		}

		internal bool Normalize
		{
			get
			{
				return m_normalize;
			}
			set
			{
				m_normalize = value;
			}
		}
	}
}
