using System;
using System.Data;
using System.Diagnostics;

namespace Microsoft.Reporting.Gauge.WebForms
{
	[DebuggerStepThrough]
	internal class ValuesRow : DataRow
	{
		private ValuesDataTable tableValues;

		public DateTime DateStamp
		{
			get
			{
				return (DateTime)base[tableValues.DateStampColumn];
			}
			set
			{
				base[tableValues.DateStampColumn] = value;
			}
		}

		public double Value
		{
			get
			{
				try
				{
					return (double)base[tableValues.ValueColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException(Utils.SRGetStr("ExceptionValueDbNull"), innerException);
				}
			}
			set
			{
				base[tableValues.ValueColumn] = value;
			}
		}

		internal ValuesRow(DataRowBuilder rb)
			: base(rb)
		{
			tableValues = (ValuesDataTable)base.Table;
		}

		public bool IsValueNull()
		{
			return IsNull(tableValues.ValueColumn);
		}

		public void SetValueNull()
		{
			base[tableValues.ValueColumn] = Convert.DBNull;
		}
	}
}
