using System;
using System.Reflection;

namespace Microsoft.ReportingServices.Common
{
	internal static class AttributeUtil
	{
		public static T GetCustomAttribute<T>(MemberInfo element) where T : Attribute
		{
			return (T)Attribute.GetCustomAttribute(element, typeof(T));
		}

		public static T GetCustomAttribute<T>(MemberInfo element, bool inherit) where T : Attribute
		{
			return (T)Attribute.GetCustomAttribute(element, typeof(T), inherit);
		}

		public static T GetCustomAttribute<T>(ParameterInfo element) where T : Attribute
		{
			return (T)Attribute.GetCustomAttribute(element, typeof(T));
		}

		public static T GetCustomAttribute<T>(ParameterInfo element, bool inherit) where T : Attribute
		{
			return (T)Attribute.GetCustomAttribute(element, typeof(T), inherit);
		}

		public static T GetCustomAttribute<T>(Assembly element) where T : Attribute
		{
			return (T)Attribute.GetCustomAttribute(element, typeof(T));
		}

		public static T GetCustomAttribute<T>(Module element) where T : Attribute
		{
			return (T)Attribute.GetCustomAttribute(element, typeof(T));
		}
	}
}
