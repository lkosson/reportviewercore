using System;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel.Serialization
{
	internal abstract class RdlReaderWriterBase
	{
		private RdlSerializerSettings m_settings;

		private ISerializerHost m_host;

		private XmlAttributeOverrides m_xmlOverrides;

		protected RdlSerializerSettings Settings => m_settings;

		protected ISerializerHost Host => m_host;

		protected XmlAttributeOverrides XmlOverrides => m_xmlOverrides;

		protected RdlReaderWriterBase(RdlSerializerSettings settings)
		{
			m_settings = settings;
			if (m_settings != null)
			{
				m_host = m_settings.Host;
				m_xmlOverrides = m_settings.XmlAttributeOverrides;
			}
		}

		protected Type GetSerializationType(object obj)
		{
			return GetSerializationType(obj.GetType());
		}

		protected Type GetSerializationType(Type type)
		{
			if (m_host != null)
			{
				return m_host.GetSubstituteType(type);
			}
			return type;
		}
	}
}
