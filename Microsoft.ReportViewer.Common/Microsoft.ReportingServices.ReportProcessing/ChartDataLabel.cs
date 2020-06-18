using Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel;
using Microsoft.ReportingServices.ReportProcessing.Persistence;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ChartDataLabel
	{
		internal enum Positions
		{
			Auto,
			Top,
			TopLeft,
			TopRight,
			Left,
			Center,
			Right,
			BottomRight,
			Bottom,
			BottomLeft
		}

		private bool m_visible;

		private ExpressionInfo m_value;

		private Style m_styleClass;

		private Positions m_position;

		private int m_rotation;

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

		internal ExpressionInfo Value
		{
			get
			{
				return m_value;
			}
			set
			{
				m_value = value;
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

		internal int Rotation
		{
			get
			{
				return m_rotation;
			}
			set
			{
				m_rotation = value;
			}
		}

		internal void Initialize(InitializationContext context)
		{
			if (m_value != null)
			{
				m_value.Initialize("DataLabel", context);
				context.ExprHostBuilder.DataLabelValue(m_value);
			}
			if (m_styleClass != null)
			{
				context.ExprHostBuilder.DataLabelStyleStart();
				m_styleClass.Initialize(context);
				context.ExprHostBuilder.DataLabelStyleEnd();
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
			memberInfoList.Add(new MemberInfo(MemberName.Visible, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.Value, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfo));
			memberInfoList.Add(new MemberInfo(MemberName.StyleClass, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.Style));
			memberInfoList.Add(new MemberInfo(MemberName.Position, Token.Enum));
			memberInfoList.Add(new MemberInfo(MemberName.Rotation, Token.Int32));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
