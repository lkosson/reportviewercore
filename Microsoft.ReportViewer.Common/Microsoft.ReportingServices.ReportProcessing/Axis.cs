using Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel;
using Microsoft.ReportingServices.ReportProcessing.Persistence;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class Axis
	{
		internal enum TickMarks
		{
			None,
			Inside,
			Outside,
			Cross
		}

		internal enum Mode
		{
			CategoryAxis,
			CategoryAxisSecondary,
			ValueAxis,
			ValueAxisSecondary
		}

		internal enum ExpressionType
		{
			Min,
			Max,
			CrossAt,
			MajorInterval,
			MinorInterval
		}

		private bool m_visible;

		private Style m_styleClass;

		private ChartTitle m_title;

		private bool m_margin;

		private TickMarks m_majorTickMarks;

		private TickMarks m_minorTickMarks;

		private GridLines m_majorGridLines;

		private GridLines m_minorGridLines;

		private ExpressionInfo m_majorInterval;

		private ExpressionInfo m_minorInterval;

		private bool m_reverse;

		private ExpressionInfo m_crossAt;

		private bool m_autoCrossAt = true;

		private bool m_interlaced;

		private bool m_scalar;

		private ExpressionInfo m_min;

		private ExpressionInfo m_max;

		private bool m_autoScaleMin = true;

		private bool m_autoScaleMax = true;

		private bool m_logScale;

		private DataValueList m_customProperties;

		[NonSerialized]
		private AxisExprHost m_exprHost;

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

		internal ChartTitle Title
		{
			get
			{
				return m_title;
			}
			set
			{
				m_title = value;
			}
		}

		internal bool Margin
		{
			get
			{
				return m_margin;
			}
			set
			{
				m_margin = value;
			}
		}

		internal TickMarks MajorTickMarks
		{
			get
			{
				return m_majorTickMarks;
			}
			set
			{
				m_majorTickMarks = value;
			}
		}

		internal TickMarks MinorTickMarks
		{
			get
			{
				return m_minorTickMarks;
			}
			set
			{
				m_minorTickMarks = value;
			}
		}

		internal GridLines MajorGridLines
		{
			get
			{
				return m_majorGridLines;
			}
			set
			{
				m_majorGridLines = value;
			}
		}

		internal GridLines MinorGridLines
		{
			get
			{
				return m_minorGridLines;
			}
			set
			{
				m_minorGridLines = value;
			}
		}

		internal ExpressionInfo MajorInterval
		{
			get
			{
				return m_majorInterval;
			}
			set
			{
				m_majorInterval = value;
			}
		}

		internal ExpressionInfo MinorInterval
		{
			get
			{
				return m_minorInterval;
			}
			set
			{
				m_minorInterval = value;
			}
		}

		internal bool Reverse
		{
			get
			{
				return m_reverse;
			}
			set
			{
				m_reverse = value;
			}
		}

		internal ExpressionInfo CrossAt
		{
			get
			{
				return m_crossAt;
			}
			set
			{
				m_crossAt = value;
			}
		}

		internal bool AutoCrossAt
		{
			get
			{
				return m_autoCrossAt;
			}
			set
			{
				m_autoCrossAt = value;
			}
		}

		internal bool Interlaced
		{
			get
			{
				return m_interlaced;
			}
			set
			{
				m_interlaced = value;
			}
		}

		internal bool Scalar
		{
			get
			{
				return m_scalar;
			}
			set
			{
				m_scalar = value;
			}
		}

		internal ExpressionInfo Min
		{
			get
			{
				return m_min;
			}
			set
			{
				m_min = value;
			}
		}

		internal ExpressionInfo Max
		{
			get
			{
				return m_max;
			}
			set
			{
				m_max = value;
			}
		}

		internal bool AutoScaleMin
		{
			get
			{
				return m_autoScaleMin;
			}
			set
			{
				m_autoScaleMin = value;
			}
		}

		internal bool AutoScaleMax
		{
			get
			{
				return m_autoScaleMax;
			}
			set
			{
				m_autoScaleMax = value;
			}
		}

		internal bool LogScale
		{
			get
			{
				return m_logScale;
			}
			set
			{
				m_logScale = value;
			}
		}

		internal DataValueList CustomProperties
		{
			get
			{
				return m_customProperties;
			}
			set
			{
				m_customProperties = value;
			}
		}

		internal AxisExprHost ExprHost => m_exprHost;

		internal void SetExprHost(AxisExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			m_exprHost = exprHost;
			m_exprHost.SetReportObjectModel(reportObjectModel);
			if (m_title != null && m_exprHost.TitleHost != null)
			{
				m_title.SetExprHost(m_exprHost.TitleHost, reportObjectModel);
			}
			if (m_styleClass != null)
			{
				m_styleClass.SetStyleExprHost(m_exprHost);
			}
			if (m_majorGridLines != null && m_majorGridLines.StyleClass != null && m_exprHost.MajorGridLinesHost != null)
			{
				m_majorGridLines.SetExprHost(m_exprHost.MajorGridLinesHost, reportObjectModel);
			}
			if (m_minorGridLines != null && m_minorGridLines.StyleClass != null && m_exprHost.MinorGridLinesHost != null)
			{
				m_minorGridLines.SetExprHost(m_exprHost.MinorGridLinesHost, reportObjectModel);
			}
			if (m_exprHost.CustomPropertyHostsRemotable != null)
			{
				Global.Tracer.Assert(m_customProperties != null);
				m_customProperties.SetExprHost(m_exprHost.CustomPropertyHostsRemotable, reportObjectModel);
			}
		}

		internal void Initialize(InitializationContext context, Mode mode)
		{
			if (m_styleClass != null)
			{
				m_styleClass.Initialize(context);
			}
			if (m_title != null)
			{
				m_title.Initialize(context);
			}
			if (m_minorGridLines != null)
			{
				context.ExprHostBuilder.MinorGridLinesStyleStart();
				m_minorGridLines.Initialize(context);
				context.ExprHostBuilder.MinorGridLinesStyleEnd();
			}
			if (m_majorGridLines != null)
			{
				context.ExprHostBuilder.MajorGridLinesStyleStart();
				m_majorGridLines.Initialize(context);
				context.ExprHostBuilder.MajorGridLinesStyleEnd();
			}
			string str = mode.ToString();
			if (m_min != null)
			{
				m_min.Initialize(str + ".Min", context);
				context.ExprHostBuilder.AxisMin(m_min);
			}
			if (m_max != null)
			{
				m_max.Initialize(str + ".Max", context);
				context.ExprHostBuilder.AxisMax(m_max);
			}
			if (m_crossAt != null)
			{
				m_crossAt.Initialize(str + ".CrossAt", context);
				context.ExprHostBuilder.AxisCrossAt(m_crossAt);
			}
			if (m_majorInterval != null)
			{
				m_majorInterval.Initialize(str + ".MajorInterval", context);
				context.ExprHostBuilder.AxisMajorInterval(m_majorInterval);
			}
			if (m_minorInterval != null)
			{
				m_minorInterval.Initialize(str + ".MinorInterval", context);
				context.ExprHostBuilder.AxisMinorInterval(m_minorInterval);
			}
			if (m_customProperties != null)
			{
				m_customProperties.Initialize(str + ".", isCustomProperty: true, context);
			}
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Visible, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.StyleClass, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.Style));
			memberInfoList.Add(new MemberInfo(MemberName.Title, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ChartTitle));
			memberInfoList.Add(new MemberInfo(MemberName.Margin, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.MajorTickMarks, Token.Enum));
			memberInfoList.Add(new MemberInfo(MemberName.MinorTickMarks, Token.Enum));
			memberInfoList.Add(new MemberInfo(MemberName.MajorGridLines, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.GridLines));
			memberInfoList.Add(new MemberInfo(MemberName.MinorGridLines, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.GridLines));
			memberInfoList.Add(new MemberInfo(MemberName.MajorInterval, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfo));
			memberInfoList.Add(new MemberInfo(MemberName.MinorInterval, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfo));
			memberInfoList.Add(new MemberInfo(MemberName.Reverse, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.CrossAt, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfo));
			memberInfoList.Add(new MemberInfo(MemberName.AutoCrossAt, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.Interlaced, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.Scalar, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.Min, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfo));
			memberInfoList.Add(new MemberInfo(MemberName.Max, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfo));
			memberInfoList.Add(new MemberInfo(MemberName.AutoScaleMin, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.AutoScaleMax, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.LogScale, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.CustomProperties, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.DataValueList));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
