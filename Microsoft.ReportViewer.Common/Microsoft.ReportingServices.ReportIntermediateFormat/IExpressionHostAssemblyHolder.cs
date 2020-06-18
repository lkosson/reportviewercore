using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal interface IExpressionHostAssemblyHolder
	{
		ObjectType ObjectType
		{
			get;
		}

		string ExprHostAssemblyName
		{
			get;
		}

		byte[] CompiledCode
		{
			get;
			set;
		}

		bool CompiledCodeGeneratedWithRefusedPermissions
		{
			get;
			set;
		}

		List<string> CodeModules
		{
			get;
			set;
		}

		List<CodeClass> CodeClasses
		{
			get;
			set;
		}
	}
}
