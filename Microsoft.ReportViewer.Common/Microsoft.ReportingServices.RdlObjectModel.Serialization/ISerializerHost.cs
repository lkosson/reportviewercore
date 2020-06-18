using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.RdlObjectModel.Serialization
{
	internal interface ISerializerHost
	{
		Type GetSubstituteType(Type type);

		void OnDeserialization(object value);

		IEnumerable<ExtensionNamespace> GetExtensionNamespaces();
	}
}
