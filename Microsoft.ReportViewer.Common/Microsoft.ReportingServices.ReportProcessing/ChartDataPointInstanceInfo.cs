using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ChartDataPointInstanceInfo : InstanceInfo
	{
		private int m_dataPointIndex = -1;

		private object[] m_dataValues;

		private string m_dataLabelValue;

		private object[] m_dataLabelStyleAttributeValues;

		private ActionInstance m_action;

		private object[] m_styleAttributeValues;

		private object[] m_markerStyleAttributeValues;

		private DataValueInstanceList m_customPropertyInstances;

		internal int DataPointIndex
		{
			get
			{
				return m_dataPointIndex;
			}
			set
			{
				m_dataPointIndex = value;
			}
		}

		internal object[] DataValues
		{
			get
			{
				return m_dataValues;
			}
			set
			{
				m_dataValues = value;
			}
		}

		internal string DataLabelValue
		{
			get
			{
				return m_dataLabelValue;
			}
			set
			{
				m_dataLabelValue = value;
			}
		}

		internal object[] DataLabelStyleAttributeValues
		{
			get
			{
				return m_dataLabelStyleAttributeValues;
			}
			set
			{
				m_dataLabelStyleAttributeValues = value;
			}
		}

		internal ActionInstance Action
		{
			get
			{
				return m_action;
			}
			set
			{
				m_action = value;
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

		internal object[] MarkerStyleAttributeValues
		{
			get
			{
				return m_markerStyleAttributeValues;
			}
			set
			{
				m_markerStyleAttributeValues = value;
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

		internal ChartDataPointInstanceInfo(ReportProcessing.ProcessingContext pc, Chart chart, ChartDataPoint dataPointDef, int dataPointIndex, ChartDataPointInstance owner)
		{
			m_dataPointIndex = dataPointIndex;
			int count = dataPointDef.DataValues.Count;
			m_dataValues = new object[count];
			bool flag = false;
			if (dataPointDef.Action != null)
			{
				flag = dataPointDef.Action.ResetObjectModelForDrillthroughContext(pc.ReportObjectModel, dataPointDef);
			}
			for (int i = 0; i < count; i++)
			{
				m_dataValues[i] = pc.ReportRuntime.EvaluateChartDataPointDataValueExpression(dataPointDef, dataPointDef.DataValues[i], chart.Name);
			}
			if (flag)
			{
				dataPointDef.Action.GetSelectedItemsForDrillthroughContext(pc.ReportObjectModel, dataPointDef);
			}
			if (dataPointDef.DataLabel != null)
			{
				m_dataLabelStyleAttributeValues = Chart.CreateStyle(pc, dataPointDef.DataLabel.StyleClass, chart.Name + ".DataLabel", owner.UniqueName);
				m_dataLabelValue = pc.ReportRuntime.EvaluateChartDataLabelValueExpression(dataPointDef, chart.Name, m_dataLabelStyleAttributeValues);
			}
			if (dataPointDef.Action != null)
			{
				m_action = ReportProcessing.RuntimeRICollection.CreateActionInstance(pc, dataPointDef, owner.UniqueName, chart.ObjectType, chart.Name + ".DataPoint");
			}
			m_styleAttributeValues = Chart.CreateStyle(pc, dataPointDef.StyleClass, chart.Name + ".DataPoint", owner.UniqueName);
			if (dataPointDef.MarkerStyleClass != null)
			{
				m_markerStyleAttributeValues = Chart.CreateStyle(pc, dataPointDef.MarkerStyleClass, chart.Name + ".DataPoint.Marker", owner.UniqueName);
			}
			if (dataPointDef.CustomProperties != null)
			{
				m_customPropertyInstances = dataPointDef.CustomProperties.EvaluateExpressions(chart.ObjectType, chart.Name, "DataPoint(" + (dataPointIndex + 1).ToString(CultureInfo.InvariantCulture) + ").", pc);
			}
			pc.ChunkManager.AddInstance(this, owner, pc.InPageSection);
		}

		internal ChartDataPointInstanceInfo()
		{
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.DataPointIndex, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.DataValues, Token.Array, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.Variant));
			memberInfoList.Add(new MemberInfo(MemberName.DataLabelValue, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.DataLabelStyleAttributeValues, Token.Array, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.Variant));
			memberInfoList.Add(new MemberInfo(MemberName.Action, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ActionInstance));
			memberInfoList.Add(new MemberInfo(MemberName.StyleAttributeValues, Token.Array, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.Variant));
			memberInfoList.Add(new MemberInfo(MemberName.MarkerStyleAttributeValues, Token.Array, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.Variant));
			memberInfoList.Add(new MemberInfo(MemberName.CustomPropertyInstances, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.DataValueInstanceList));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.InstanceInfo, memberInfoList);
		}
	}
}
