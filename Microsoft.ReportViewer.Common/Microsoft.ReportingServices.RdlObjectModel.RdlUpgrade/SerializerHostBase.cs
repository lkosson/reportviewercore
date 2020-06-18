using Microsoft.ReportingServices.RdlObjectModel.Serialization;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.RdlObjectModel.RdlUpgrade
{
	internal abstract class SerializerHostBase : ISerializerHost
	{
		protected string m_extraStringData;

		protected bool m_serializing;

		public string ExtraStringData
		{
			get
			{
				return m_extraStringData;
			}
			set
			{
				m_extraStringData = value;
			}
		}

		internal SerializerHostBase(bool serializing)
		{
			m_serializing = serializing;
		}

		public abstract Type GetSubstituteType(Type type);

		public virtual void OnDeserialization(object value)
		{
		}

		public abstract IEnumerable<ExtensionNamespace> GetExtensionNamespaces();
	}
}
