namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapFieldNameInstance : BaseInstance
	{
		private MapFieldName m_defObject;

		private string m_name;

		public string Name
		{
			get
			{
				if (m_name == null)
				{
					m_name = m_defObject.MapFieldNameDef.EvaluateName(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_name;
			}
		}

		internal MapFieldNameInstance(MapFieldName defObject)
			: base(defObject.MapDef.ReportScope)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			m_name = null;
		}
	}
}
