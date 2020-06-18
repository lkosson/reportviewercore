using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;
using System.Threading;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ChartInstanceInfo : ReportItemInstanceInfo
	{
		private AxisInstance m_categoryAxis;

		private AxisInstance m_valueAxis;

		private ChartTitleInstance m_title;

		private object[] m_plotAreaStyleAttributeValues;

		private object[] m_legendStyleAttributeValues;

		private string m_cultureName;

		private string m_noRows;

		internal AxisInstance CategoryAxis
		{
			get
			{
				return m_categoryAxis;
			}
			set
			{
				m_categoryAxis = value;
			}
		}

		internal AxisInstance ValueAxis
		{
			get
			{
				return m_valueAxis;
			}
			set
			{
				m_valueAxis = value;
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

		internal object[] PlotAreaStyleAttributeValues
		{
			get
			{
				return m_plotAreaStyleAttributeValues;
			}
			set
			{
				m_plotAreaStyleAttributeValues = value;
			}
		}

		internal object[] LegendStyleAttributeValues
		{
			get
			{
				return m_legendStyleAttributeValues;
			}
			set
			{
				m_legendStyleAttributeValues = value;
			}
		}

		internal string CultureName
		{
			get
			{
				return m_cultureName;
			}
			set
			{
				m_cultureName = value;
			}
		}

		internal string NoRows
		{
			get
			{
				return m_noRows;
			}
			set
			{
				m_noRows = value;
			}
		}

		internal ChartInstanceInfo(ReportProcessing.ProcessingContext pc, Chart reportItemDef, ChartInstance owner)
			: base(pc, reportItemDef, owner, addToChunk: true)
		{
			if (reportItemDef.Title != null)
			{
				m_title = new ChartTitleInstance(pc, reportItemDef, reportItemDef.Title, "Title");
			}
			if (reportItemDef.CategoryAxis != null)
			{
				m_categoryAxis = new AxisInstance(pc, reportItemDef, reportItemDef.CategoryAxis, Axis.Mode.CategoryAxis);
			}
			if (reportItemDef.ValueAxis != null)
			{
				m_valueAxis = new AxisInstance(pc, reportItemDef, reportItemDef.ValueAxis, Axis.Mode.ValueAxis);
			}
			if (reportItemDef.Legend != null)
			{
				m_legendStyleAttributeValues = Chart.CreateStyle(pc, reportItemDef.Legend.StyleClass, reportItemDef.Name + ".Legend", owner.UniqueName);
			}
			if (reportItemDef.PlotArea != null)
			{
				m_plotAreaStyleAttributeValues = Chart.CreateStyle(pc, reportItemDef.PlotArea.StyleClass, reportItemDef.Name + ".PlotArea", owner.UniqueName);
			}
			SaveChartCulture();
			m_noRows = pc.ReportRuntime.EvaluateDataRegionNoRowsExpression(reportItemDef, reportItemDef.ObjectType, reportItemDef.Name, "NoRows");
		}

		internal ChartInstanceInfo(Chart reportItemDef)
			: base(reportItemDef)
		{
		}

		private void SaveChartCulture()
		{
			if (m_reportItemDef.StyleClass != null && m_reportItemDef.StyleClass.StyleAttributes != null)
			{
				AttributeInfo attributeInfo = m_reportItemDef.StyleClass.StyleAttributes["Language"];
				if (attributeInfo != null)
				{
					if (attributeInfo.IsExpression)
					{
						m_cultureName = (string)m_styleAttributeValues[attributeInfo.IntValue];
					}
					else
					{
						m_cultureName = attributeInfo.Value;
					}
				}
			}
			if (m_cultureName == null)
			{
				m_cultureName = Thread.CurrentThread.CurrentCulture.Name;
			}
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.CategoryAxis, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.AxisInstance));
			memberInfoList.Add(new MemberInfo(MemberName.ValueAxis, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.AxisInstance));
			memberInfoList.Add(new MemberInfo(MemberName.Title, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ChartTitleInstance));
			memberInfoList.Add(new MemberInfo(MemberName.PlotAreaStyleAttributeValues, Token.Array, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.Variant));
			memberInfoList.Add(new MemberInfo(MemberName.LegendStyleAttributeValues, Token.Array, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.Variant));
			memberInfoList.Add(new MemberInfo(MemberName.CultureName, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.NoRows, Token.String));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
