namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapBindingFieldPairInstance : BaseInstance
	{
		private MapBindingFieldPair m_defObject;

		private string m_fieldName;

		private object m_bindingExpression;

		public string FieldName
		{
			get
			{
				if (m_fieldName == null)
				{
					m_fieldName = m_defObject.MapBindingFieldPairDef.EvaluateFieldName(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_fieldName;
			}
		}

		public object BindingExpression
		{
			get
			{
				if (m_bindingExpression == null)
				{
					m_bindingExpression = m_defObject.MapBindingFieldPairDef.EvaluateBindingExpression(m_defObject.ReportScope.ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext).Value;
				}
				return m_bindingExpression;
			}
		}

		internal MapBindingFieldPairInstance(MapBindingFieldPair defObject)
			: base(defObject.MapDef.ReportScope)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			m_fieldName = null;
			m_bindingExpression = null;
		}
	}
}
