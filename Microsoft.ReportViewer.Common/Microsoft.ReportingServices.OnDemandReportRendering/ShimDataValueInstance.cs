namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimDataValueInstance : DataValueInstance
	{
		private string m_name;

		private object m_value;

		public override string Name => m_name;

		public override object Value => m_value;

		internal ShimDataValueInstance(string name, object value)
			: base(null)
		{
			m_name = name;
			m_value = value;
		}

		internal void Update(string name, object value)
		{
			m_name = name;
			m_value = value;
		}

		protected override void ResetInstanceCache()
		{
		}
	}
}
