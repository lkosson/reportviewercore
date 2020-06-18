using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.RdlObjectModel.Serialization
{
	internal class TypeMapping
	{
		public Type Type;

		public string Name;

		public string Namespace;

		public List<MemberMapping> ChildAttributes;

		public TypeMapping(Type type)
		{
			Type = type;
			Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition";
			Name = type.Name;
		}

		internal void AddChildAttribute(MemberMapping mapping)
		{
			if (ChildAttributes == null)
			{
				ChildAttributes = new List<MemberMapping>();
			}
			ChildAttributes.Add(mapping);
		}
	}
}
