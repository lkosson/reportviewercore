using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.RdlObjectModel.Serialization
{
	internal class StructMapping : TypeMapping
	{
		private struct UseTypeInfo
		{
			public string Name;

			public string Namespace;
		}

		public NameTable<MemberMapping> Elements;

		public NameTable<MemberMapping> Attributes;

		private List<MemberMapping> m_members;

		private List<UseTypeInfo> m_useTypes;

		public List<MemberMapping> Members => m_members;

		public StructMapping(Type type)
			: base(type)
		{
			Elements = new NameTable<MemberMapping>();
			Attributes = new NameTable<MemberMapping>();
			m_members = new List<MemberMapping>();
		}

		public MemberMapping GetAttribute(string name, string ns)
		{
			return Attributes[name, ns];
		}

		public MemberMapping GetElement(string name, string ns)
		{
			return Elements[name, ns];
		}

		public void AddUseTypeInfo(string name, string ns)
		{
			UseTypeInfo item = default(UseTypeInfo);
			item.Name = name;
			item.Namespace = ns;
			if (m_useTypes == null)
			{
				m_useTypes = new List<UseTypeInfo>();
			}
			m_useTypes.Add(item);
		}

		public List<MemberMapping> GetTypeNameElements()
		{
			if (m_useTypes != null)
			{
				List<MemberMapping> list = new List<MemberMapping>();
				{
					foreach (UseTypeInfo useType in m_useTypes)
					{
						list.Add(Elements[useType.Name, useType.Namespace]);
					}
					return list;
				}
			}
			return null;
		}

		internal void ResolveChildAttributes()
		{
			if (ChildAttributes != null)
			{
				for (int i = 0; i < ChildAttributes.Count; i++)
				{
					MemberMapping memberMapping = ChildAttributes[i];
					string elementName = ((XmlChildAttributeAttribute)memberMapping.XmlAttributes.XmlAttribute).ElementName;
					GetElement(elementName, "").AddChildAttribute(memberMapping);
				}
				ChildAttributes = null;
			}
		}
	}
}
