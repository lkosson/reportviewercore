using Microsoft.ReportingServices.ReportProcessing;
using System;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class InternalDataValueInstance : DataValueInstance
	{
		private DataValue m_dataValueDef;

		private string m_name;

		private object m_value;

		public override string Name
		{
			get
			{
				if (m_name == null)
				{
					EvaluateNameAndValue();
				}
				return m_name;
			}
		}

		public override object Value
		{
			get
			{
				if (m_value == null)
				{
					EvaluateNameAndValue();
				}
				return m_value;
			}
		}

		internal InternalDataValueInstance(IReportScope reportScope, DataValue dataValueDef)
			: base(reportScope)
		{
			m_dataValueDef = dataValueDef;
		}

		private void EvaluateNameAndValue()
		{
			m_dataValueDef.DataValueDef.EvaluateNameAndValue(null, ReportScopeInstance, m_dataValueDef.InstancePath, m_dataValueDef.RenderingContext.OdpContext, m_dataValueDef.IsChart ? ObjectType.Chart : ObjectType.CustomReportItem, m_dataValueDef.ObjectName, out m_name, out m_value, out TypeCode _);
		}

		protected override void ResetInstanceCache()
		{
			m_name = null;
			m_value = null;
		}
	}
}
