using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Security;
using System.Security.Permissions;

namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class ReportObjectModelProxy : MarshalByRefObject, IReportObjectModelProxyForCustomCode
	{
		private OnDemandObjectModel m_reportObjectModel;

		protected Fields Fields => m_reportObjectModel.Fields;

		protected Parameters Parameters => m_reportObjectModel.Parameters;

		protected Globals Globals => m_reportObjectModel.Globals;

		protected User User => m_reportObjectModel.User;

		protected ReportItems ReportItems => m_reportObjectModel.ReportItems;

		protected Aggregates Aggregates => m_reportObjectModel.Aggregates;

		protected Lookups Lookups => m_reportObjectModel.Lookups;

		protected DataSets DataSets => m_reportObjectModel.DataSets;

		protected DataSources DataSources => m_reportObjectModel.DataSources;

		protected Variables Variables => m_reportObjectModel.Variables;

		Parameters IReportObjectModelProxyForCustomCode.Parameters => Parameters;

		Globals IReportObjectModelProxyForCustomCode.Globals => Globals;

		User IReportObjectModelProxyForCustomCode.User => User;

		Variables IReportObjectModelProxyForCustomCode.Variables => Variables;

		internal void SetReportObjectModel(OnDemandObjectModel reportObjectModel)
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

		protected object MinValue(params object[] arguments)
		{
			return m_reportObjectModel.MinValue(arguments);
		}

		protected object MaxValue(params object[] arguments)
		{
			return m_reportObjectModel.MaxValue(arguments);
		}

		protected string CreateDrillthroughContext()
		{
			throw new NotSupportedException();
		}

		[SecurityCritical]
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
		public override object InitializeLifetimeService()
		{
			return null;
		}
	}
}
