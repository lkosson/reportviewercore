using Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel;
using Microsoft.ReportingServices.ReportProcessing.Persistence;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ChartTitle
	{
		internal enum Positions
		{
			Center,
			Near,
			Far
		}

		private ExpressionInfo m_caption;

		private Style m_styleClass;

		private Positions m_position;

		[NonSerialized]
		private ChartTitleExprHost m_exprHost;

		internal ExpressionInfo Caption
		{
			get
			{
				return m_caption;
			}
			set
			{
				m_caption = value;
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

		internal Positions Position
		{
			get
			{
				return m_position;
			}
			set
			{
				m_position = value;
			}
		}

		internal ChartTitleExprHost ExprHost => m_exprHost;

		internal void SetExprHost(ChartTitleExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			m_exprHost = exprHost;
			m_exprHost.SetReportObjectModel(reportObjectModel);
			if (m_styleClass != null)
			{
				m_styleClass.SetStyleExprHost(m_exprHost);
			}
		}

		internal void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.ChartTitleStart();
			if (m_caption != null)
			{
				m_caption.Initialize("Caption", context);
				context.ExprHostBuilder.ChartCaption(m_caption);
			}
			if (m_styleClass != null)
			{
				m_styleClass.Initialize(context);
			}
			context.ExprHostBuilder.ChartTitleEnd();
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Caption, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfo));
			memberInfoList.Add(new MemberInfo(MemberName.StyleClass, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.Style));
			memberInfoList.Add(new MemberInfo(MemberName.Position, Token.Enum));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
