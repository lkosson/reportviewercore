using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ChartTitleInstance
	{
		private int m_uniqueName;

		private string m_caption;

		private object[] m_styleAttributeValues;

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

		internal string Caption
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

		internal ChartTitleInstance(ReportProcessing.ProcessingContext pc, Chart chart, ChartTitle titleDef, string propertyName)
		{
			m_uniqueName = pc.CreateUniqueName();
			m_caption = pc.ReportRuntime.EvaluateChartTitleCaptionExpression(titleDef, chart.Name, propertyName);
			m_styleAttributeValues = Chart.CreateStyle(pc, titleDef.StyleClass, chart.Name + "." + propertyName, m_uniqueName);
		}

		internal ChartTitleInstance()
		{
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.UniqueName, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.Caption, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.StyleAttributeValues, Token.Array, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.Variant));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
