using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;

namespace Microsoft.Reporting.NETCore
{
	public sealed class ReportDataSource
	{
		private string m_name = "";

		private object m_value;

		[NotifyParentProperty(true)]
		[DefaultValue("")]
		public string Name
		{
			get
			{
				return m_name;
			}
			set
			{
				m_name = value;
				OnChanged();
			}
		}

		public object Value
		{
			get
			{
				return m_value;
			}
			set
			{
				if (value != null && !(value is DataTable) && !(value is Type) && !(value is IEnumerable))
				{
					throw new ArgumentException(ReportPreviewStrings.BadReportDataSourceType);
				}
				m_value = value;
				OnChanged();
			}
		}

		internal event EventHandler Changed;

		public ReportDataSource()
		{
		}

		public ReportDataSource(string name)
		{
			Name = name;
		}

		public ReportDataSource(string name, object dataSourceValue)
			: this(name)
		{
			Value = dataSourceValue;
		}

		public ReportDataSource(string name, DataTable dataSourceValue)
			: this(name)
		{
			Value = dataSourceValue;
		}

		public ReportDataSource(string name, Type dataSourceValue)
			: this(name)
		{
			Value = dataSourceValue;
		}

		public ReportDataSource(string name, IEnumerable dataSourceValue)
			: this(name)
		{
			Value = dataSourceValue;
		}

		internal void OnChanged()
		{
			if (this.Changed != null)
			{
				this.Changed(this, null);
			}
		}
	}
}
