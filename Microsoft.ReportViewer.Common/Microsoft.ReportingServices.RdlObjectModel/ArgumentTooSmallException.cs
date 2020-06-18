using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	[Serializable]
	internal class ArgumentTooSmallException : ArgumentConstraintException
	{
		public ArgumentTooSmallException(object component, string property, object value, object minimum)
			: base(component, property, value, minimum, SRErrors.InvalidParamGreaterThan(property, minimum))
		{
		}

		protected ArgumentTooSmallException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
