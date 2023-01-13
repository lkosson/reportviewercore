using System;
using System.Security.Permissions;

namespace Microsoft.ReportingServices.ReportProcessing.ReportObjectModel
{
	public abstract class User : MarshalByRefObject
	{
		public abstract object this[string key]
		{
			get;
		}

		public abstract string UserID
		{
			get;
		}

		public abstract string Language
		{
			get;
		}
	}
}
