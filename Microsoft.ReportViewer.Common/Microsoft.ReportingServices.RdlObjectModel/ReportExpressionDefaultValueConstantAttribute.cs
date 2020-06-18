using System;
using System.ComponentModel;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal sealed class ReportExpressionDefaultValueConstantAttribute : DefaultValueAttribute
	{
		public ReportExpressionDefaultValueConstantAttribute(string field)
			: base(new ReportExpression(DefaultValueConstantAttribute.GetConstant(field) as string))
		{
		}

		public ReportExpressionDefaultValueConstantAttribute(Type type, string field)
			: base(ReportExpressionDefaultValueAttribute.CreateInstance(type, DefaultValueConstantAttribute.GetConstant(field)))
		{
		}
	}
}
