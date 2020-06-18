using System;

namespace Microsoft.ReportingServices.ReportProcessing.ReportObjectModel
{
	internal abstract class CalculatedFieldWrapper : MarshalByRefObject
	{
		public abstract object Value
		{
			get;
		}
	}
}
