using System;
using System.ComponentModel;
using System.Drawing.Printing;
using System.Runtime.InteropServices;

namespace Microsoft.Reporting.WinForms
{
	[ComVisible(false)]
	public sealed class ReportPrintEventArgs : CancelEventArgs
	{
		private PrinterSettings m_printerSettings;

		public PrinterSettings PrinterSettings
		{
			get
			{
				return m_printerSettings;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				m_printerSettings = value;
			}
		}

		internal ReportPrintEventArgs(PrinterSettings printerSettings)
		{
			if (printerSettings == null)
			{
				throw new ArgumentNullException("printerSettings");
			}
			PrinterSettings = printerSettings;
		}
	}
}
