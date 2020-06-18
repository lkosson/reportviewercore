using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.RdlObjectModel.Serialization
{
	internal class ArrayMapping : TypeMapping
	{
		public Dictionary<string, Type> ElementTypes;

		public Type ItemType;

		public ArrayMapping(Type type)
			: base(type)
		{
			if (type.IsArray)
			{
				Name = Type.GetElementType().Name + "Array";
			}
			else if (type.IsGenericType)
			{
				Type[] genericArguments = type.GetGenericArguments();
				if (genericArguments != null && genericArguments.Length != 0)
				{
					Name = genericArguments[0].Name + "Collection";
				}
			}
		}
	}
}
