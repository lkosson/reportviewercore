using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class AxisInstance
	{
		private int m_uniqueName;

		private ChartTitleInstance m_title;

		private object[] m_styleAttributeValues;

		private object[] m_majorGridLinesStyleAttributeValues;

		private object[] m_minorGridLinesStyleAttributeValues;

		private object m_minValue;

		private object m_maxValue;

		private object m_crossAtValue;

		private object m_majorIntervalValue;

		private object m_minorIntervalValue;

		private DataValueInstanceList m_customPropertyInstances;

		internal int UniqueName
		{
			get
			{
				return m_uniqueName;
			}
			set
			{
				m_uniqueName = value;
			}
		}

		internal ChartTitleInstance Title
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

		internal object[] StyleAttributeValues
		{
			get
			{
				return m_styleAttributeValues;
			}
			set
			{
				m_styleAttributeValues = value;
			}
		}

		internal object[] MajorGridLinesStyleAttributeValues
		{
			get
			{
				return m_majorGridLinesStyleAttributeValues;
			}
			set
			{
				m_majorGridLinesStyleAttributeValues = value;
			}
		}

		internal object[] MinorGridLinesStyleAttributeValues
		{
			get
			{
				return m_minorGridLinesStyleAttributeValues;
			}
			set
			{
				m_minorGridLinesStyleAttributeValues = value;
			}
		}

		internal object MinValue
		{
			get
			{
				return m_minValue;
			}
			set
			{
				m_minValue = value;
			}
		}

		internal object MaxValue
		{
			get
			{
				return m_maxValue;
			}
			set
			{
				m_maxValue = value;
			}
		}

		internal object CrossAtValue
		{
			get
			{
				return m_crossAtValue;
			}
			set
			{
				m_crossAtValue = value;
			}
		}

		internal object MajorIntervalValue
		{
			get
			{
				return m_majorIntervalValue;
			}
			set
			{
				m_majorIntervalValue = value;
			}
		}

		internal object MinorIntervalValue
		{
			get
			{
				return m_minorIntervalValue;
			}
			set
			{
				m_minorIntervalValue = value;
			}
		}

		internal DataValueInstanceList CustomPropertyInstances
		{
			get
			{
				return m_customPropertyInstances;
			}
			set
			{
				m_customPropertyInstances = value;
			}
		}

		internal AxisInstance(ReportProcessing.ProcessingContext pc, Chart chart, Axis axisDef, Axis.Mode mode)
		{
			m_uniqueName = pc.CreateUniqueName();
			string text = mode.ToString();
			if (axisDef.Title != null)
			{
				m_title = new ChartTitleInstance(pc, chart, axisDef.Title, text);
			}
			m_styleAttributeValues = Chart.CreateStyle(pc, axisDef.StyleClass, chart.Name + "." + text, m_uniqueName);
			if (axisDef.MajorGridLines != null)
			{
				m_majorGridLinesStyleAttributeValues = Chart.CreateStyle(pc, axisDef.MajorGridLines.StyleClass, chart.Name + "." + text + ".MajorGridLines", m_uniqueName);
			}
			if (axisDef.MinorGridLines != null)
			{
				m_minorGridLinesStyleAttributeValues = Chart.CreateStyle(pc, axisDef.MinorGridLines.StyleClass, chart.Name + "." + text + ".MinorGridLines", m_uniqueName);
			}
			if (axisDef.Min != null && ExpressionInfo.Types.Constant != axisDef.Min.Type)
			{
				m_minValue = pc.ReportRuntime.EvaluateChartAxisValueExpression(axisDef.ExprHost, axisDef.Min, chart.Name, text + ".Min", Axis.ExpressionType.Min);
			}
			if (axisDef.Max != null && ExpressionInfo.Types.Constant != axisDef.Max.Type)
			{
				m_maxValue = pc.ReportRuntime.EvaluateChartAxisValueExpression(axisDef.ExprHost, axisDef.Max, chart.Name, text + ".Max", Axis.ExpressionType.Max);
			}
			if (axisDef.CrossAt != null && ExpressionInfo.Types.Constant != axisDef.CrossAt.Type)
			{
				m_crossAtValue = pc.ReportRuntime.EvaluateChartAxisValueExpression(axisDef.ExprHost, axisDef.CrossAt, chart.Name, text + ".CrossAt", Axis.ExpressionType.CrossAt);
			}
			if (axisDef.MajorInterval != null && ExpressionInfo.Types.Constant != axisDef.MajorInterval.Type)
			{
				m_majorIntervalValue = pc.ReportRuntime.EvaluateChartAxisValueExpression(axisDef.ExprHost, axisDef.MajorInterval, chart.Name, text + ".MajorInterval", Axis.ExpressionType.MajorInterval);
			}
			if (axisDef.MinorInterval != null && ExpressionInfo.Types.Constant != axisDef.MinorInterval.Type)
			{
				m_minorIntervalValue = pc.ReportRuntime.EvaluateChartAxisValueExpression(axisDef.ExprHost, axisDef.MinorInterval, chart.Name, text + ".MinorInterval", Axis.ExpressionType.MinorInterval);
			}
			if (axisDef.CustomProperties != null)
			{
				m_customPropertyInstances = axisDef.CustomProperties.EvaluateExpressions(chart.ObjectType, chart.Name, text + ".", pc);
			}
		}

		internal AxisInstance()
		{
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.UniqueName, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.Title, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ChartTitleInstance));
			memberInfoList.Add(new MemberInfo(MemberName.StyleAttributeValues, Token.Array, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.Variant));
			memberInfoList.Add(new MemberInfo(MemberName.MajorGridLinesStyleAttributeValues, Token.Array, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.Variant));
			memberInfoList.Add(new MemberInfo(MemberName.MinorGridLinesStyleAttributeValues, Token.Array, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.Variant));
			memberInfoList.Add(new MemberInfo(MemberName.Min, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.Variant));
			memberInfoList.Add(new MemberInfo(MemberName.Max, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.Variant));
			memberInfoList.Add(new MemberInfo(MemberName.CrossAt, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.Variant));
			memberInfoList.Add(new MemberInfo(MemberName.MajorInterval, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.Variant));
			memberInfoList.Add(new MemberInfo(MemberName.MinorInterval, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.Variant));
			memberInfoList.Add(new MemberInfo(MemberName.CustomPropertyInstances, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.DataValueInstanceList));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
