using System;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel.Serialization
{
	internal abstract class MemberMapping : TypeMapping
	{
		public bool IsReadOnly;

		public XmlAttributes XmlAttributes;

		public MemberMapping(Type type, string name, string ns, bool isReadOnly)
			: base(type)
		{
			Name = name;
			Namespace = ns;
			IsReadOnly = isReadOnly;
		}

		public abstract void SetValue(object obj, object value);

		public abstract object GetValue(object obj);

		public abstract bool HasValue(object obj);
	}
}
