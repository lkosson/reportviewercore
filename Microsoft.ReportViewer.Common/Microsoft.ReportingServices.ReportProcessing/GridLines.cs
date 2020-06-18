using Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel;
using Microsoft.ReportingServices.ReportProcessing.Persistence;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class GridLines
	{
		private bool m_showGridLines;

		private Style m_styleClass;

		internal bool ShowGridLines
		{
			get
			{
				return m_showGridLines;
			}
			set
			{
				m_showGridLines = value;
			}
		}

		internal Style StyleClass
		{
			get
			{
				return m_styleClass;
			}
			set
			{
				m_styleClass = value;
			}
		}

		internal void Initialize(InitializationContext context)
		{
			if (m_styleClass != null)
			{
				m_styleClass.Initialize(context);
			}
		}

		internal void SetExprHost(StyleExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			exprHost.SetReportObjectModel(reportObjectModel);
			if (m_styleClass != null)
			{
				m_styleClass.SetStyleExprHost(exprHost);
			}
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.ShowGridLines, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.StyleClass, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.Style));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
