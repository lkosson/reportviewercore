using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal sealed class DefaultValueConstantAttribute : DefaultValueAttribute
	{
		public DefaultValueConstantAttribute(string field)
			: base(GetConstant(field))
		{
		}

		internal static object GetConstant(string field)
		{
			return typeof(Constants).InvokeMember(field, BindingFlags.GetField, null, null, null, CultureInfo.InvariantCulture);
		}
	}
}
