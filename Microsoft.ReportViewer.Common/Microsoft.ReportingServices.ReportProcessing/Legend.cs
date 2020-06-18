using Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel;
using Microsoft.ReportingServices.ReportProcessing.Persistence;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class Legend
	{
		internal enum LegendLayout
		{
			Column,
			Row,
			Table
		}

		internal enum Positions
		{
			RightTop,
			TopLeft,
			TopCenter,
			TopRight,
			LeftTop,
			LeftCenter,
			LeftBottom,
			RightCenter,
			RightBottom,
			BottomLeft,
			BottomCenter,
			BottomRight
		}

		private bool m_visible;

		private Style m_styleClass;

		private Positions m_position;

		private LegendLayout m_layout;

		private bool m_insidePlotArea;

		internal bool Visible
		{
			get
			{
				return m_visible;
			}
			set
			{
				m_visible = value;
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

		internal LegendLayout Layout
		{
			get
			{
				return m_layout;
			}
			set
			{
				m_layout = value;
			}
		}

		internal bool InsidePlotArea
		{
			get
			{
				return m_insidePlotArea;
			}
			set
			{
				m_insidePlotArea = value;
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

		internal void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.ChartLegendStart();
			if (m_styleClass != null)
			{
				m_styleClass.Initialize(context);
			}
			context.ExprHostBuilder.ChartLegendEnd();
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Visible, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.StyleClass, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.Style));
			memberInfoList.Add(new MemberInfo(MemberName.Position, Token.Enum));
			memberInfoList.Add(new MemberInfo(MemberName.Layout, Token.Enum));
			memberInfoList.Add(new MemberInfo(MemberName.InsidePlotArea, Token.Boolean));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
