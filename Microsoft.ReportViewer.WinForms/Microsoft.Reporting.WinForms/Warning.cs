using Microsoft.Reporting.WinForms.Internal.Soap.ReportingServices2005.Execution;
using Microsoft.ReportingServices.ReportProcessing;
using System;

namespace Microsoft.Reporting.WinForms
{
	public sealed class Warning
	{
		public string Code
		{
			get;
			private set;
		}

		public string Message
		{
			get;
			private set;
		}

		public string ObjectName
		{
			get;
			private set;
		}

		public string ObjectType
		{
			get;
			private set;
		}

		public Severity Severity
		{
			get;
			private set;
		}

		internal Warning(string code, string message, string objectName, string objectType, string severity)
		{
			Code = code;
			Message = message;
			ObjectName = objectName;
			ObjectType = objectType;
			if (string.Compare(severity, "Warning", StringComparison.OrdinalIgnoreCase) == 0)
			{
				Severity = Severity.Warning;
			}
			else
			{
				Severity = Severity.Error;
			}
		}

		internal static Warning[] FromSoapWarnings(Microsoft.Reporting.WinForms.Internal.Soap.ReportingServices2005.Execution.Warning[] soapWarnings)
		{
			if (soapWarnings == null)
			{
				return new Warning[0];
			}
			Warning[] array = new Warning[soapWarnings.Length];
			for (int i = 0; i < soapWarnings.Length; i++)
			{
				array[i] = new Warning(soapWarnings[i].Code, soapWarnings[i].Message, soapWarnings[i].ObjectName, soapWarnings[i].ObjectType, soapWarnings[i].Severity);
			}
			return array;
		}

		internal static Warning[] FromProcessingMessageList(ProcessingMessageList processingWarnings)
		{
			if (processingWarnings == null)
			{
				return new Warning[0];
			}
			Warning[] array = new Warning[processingWarnings.Count];
			for (int i = 0; i < processingWarnings.Count; i++)
			{
				array[i] = new Warning(processingWarnings[i].Code.ToString(), processingWarnings[i].Message, processingWarnings[i].ObjectName, processingWarnings[i].ObjectType.ToString(), processingWarnings[i].Severity.ToString());
			}
			return array;
		}
	}
}
