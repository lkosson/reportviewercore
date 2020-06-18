using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;

namespace Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel
{
	public abstract class ReportObjectModelProxy : MarshalByRefObject, IReportObjectModelProxyForCustomCode
	{
		private ObjectModel m_reportObjectModel;

		protected Fields Fields => m_reportObjectModel.Fields;

		protected Parameters Parameters => m_reportObjectModel.Parameters;

		protected Globals Globals => m_reportObjectModel.Globals;

		protected User User => m_reportObjectModel.User;

		protected ReportItems ReportItems => m_reportObjectModel.ReportItems;

		protected Aggregates Aggregates => m_reportObjectModel.Aggregates;

		protected DataSets DataSets => m_reportObjectModel.DataSets;

		protected DataSources DataSources => m_reportObjectModel.DataSources;

		Parameters IReportObjectModelProxyForCustomCode.Parameters => Parameters;

		Globals IReportObjectModelProxyForCustomCode.Globals => Globals;

		User IReportObjectModelProxyForCustomCode.User => User;

		internal void SetReportObjectModel(ObjectModel reportObjectModel)
		{
			m_reportObjectModel = reportObjectModel;
		}

		protected bool InScope(string scope)
		{
			return m_reportObjectModel.InScope(scope);
		}

		protected int Level()
		{
			return m_reportObjectModel.RecursiveLevel(null);
		}

		protected int Level(string scope)
		{
			return m_reportObjectModel.RecursiveLevel(scope);
		}
	}
}
