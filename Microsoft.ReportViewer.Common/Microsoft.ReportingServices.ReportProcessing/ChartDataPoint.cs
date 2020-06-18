using Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel;
using Microsoft.ReportingServices.ReportProcessing.Persistence;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using Microsoft.ReportingServices.ReportRendering;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ChartDataPoint : IActionOwner
	{
		internal enum MarkerTypes
		{
			None,
			Square,
			Circle,
			Diamond,
			Triangle,
			Cross,
			Auto
		}

		private ExpressionInfoList m_dataValues;

		private ChartDataLabel m_dataLabel;

		private Action m_action;

		private Style m_styleClass;

		private MarkerTypes m_markerType;

		private string m_markerSize;

		private Style m_markerStyleClass;

		private string m_dataElementName;

		private DataElementOutputTypes m_dataElementOutput;

		private int m_exprHostID = -1;

		private DataValueList m_customProperties;

		[NonSerialized]
		private ChartDataPointExprHost m_exprHost;

		[NonSerialized]
		private List<string> m_fieldsUsedInValueExpression;

		internal ExpressionInfoList DataValues
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

		internal ChartDataLabel DataLabel
		{
			get
			{
				return m_dataLabel;
			}
			set
			{
				m_dataLabel = value;
			}
		}

		internal Action Action
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

		internal MarkerTypes MarkerType
		{
			get
			{
				return m_markerType;
			}
			set
			{
				m_markerType = value;
			}
		}

		internal string MarkerSize
		{
			get
			{
				return m_markerSize;
			}
			set
			{
				m_markerSize = value;
			}
		}

		internal Style MarkerStyleClass
		{
			get
			{
				return m_markerStyleClass;
			}
			set
			{
				m_markerStyleClass = value;
			}
		}

		internal string DataElementName
		{
			get
			{
				return m_dataElementName;
			}
			set
			{
				m_dataElementName = value;
			}
		}

		internal DataElementOutputTypes DataElementOutput
		{
			get
			{
				return m_dataElementOutput;
			}
			set
			{
				m_dataElementOutput = value;
			}
		}

		internal int ExprHostID
		{
			get
			{
				return m_exprHostID;
			}
			set
			{
				m_exprHostID = value;
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

		internal ChartDataPointExprHost ExprHost => m_exprHost;

		Action IActionOwner.Action => m_action;

		List<string> IActionOwner.FieldsUsedInValueExpression
		{
			get
			{
				return m_fieldsUsedInValueExpression;
			}
			set
			{
				m_fieldsUsedInValueExpression = value;
			}
		}

		internal ChartDataPoint()
		{
			m_dataValues = new ExpressionInfoList();
		}

		internal void Initialize(InitializationContext context)
		{
			ExprHostBuilder exprHostBuilder = context.ExprHostBuilder;
			exprHostBuilder.ChartDataPointStart();
			for (int i = 0; i < m_dataValues.Count; i++)
			{
				m_dataValues[i].Initialize("DataPoint", context);
				exprHostBuilder.ChartDataPointDataValue(m_dataValues[i]);
			}
			if (m_dataLabel != null)
			{
				m_dataLabel.Initialize(context);
			}
			if (m_action != null)
			{
				m_action.Initialize(context);
			}
			if (m_styleClass != null)
			{
				exprHostBuilder.DataPointStyleStart();
				m_styleClass.Initialize(context);
				exprHostBuilder.DataPointStyleEnd();
			}
			if (m_markerStyleClass != null)
			{
				exprHostBuilder.DataPointMarkerStyleStart();
				m_markerStyleClass.Initialize(context);
				exprHostBuilder.DataPointMarkerStyleEnd();
			}
			if (m_markerSize != null)
			{
				double size = context.ValidateSize(m_markerSize, "MarkerSize");
				m_markerSize = Converter.ConvertSize(size);
			}
			if (m_customProperties != null)
			{
				m_customProperties.Initialize(null, isCustomProperty: true, context);
			}
			DataRendererInitialize(context);
			m_exprHostID = exprHostBuilder.ChartDataPointEnd();
		}

		internal void DataRendererInitialize(InitializationContext context)
		{
			CLSNameValidator.ValidateDataElementName(ref m_dataElementName, "Value", context.ObjectType, context.ObjectName, "DataElementName", context.ErrorContext);
		}

		internal void SetExprHost(ChartDataPointExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			m_exprHost = exprHost;
			m_exprHost.SetReportObjectModel(reportObjectModel);
			if (m_action != null)
			{
				if (m_exprHost.ActionInfoHost != null)
				{
					m_action.SetExprHost(m_exprHost.ActionInfoHost, reportObjectModel);
				}
				else if (m_exprHost.ActionHost != null)
				{
					m_action.SetExprHost(m_exprHost.ActionHost, reportObjectModel);
				}
			}
			if (m_styleClass != null && m_exprHost.StyleHost != null)
			{
				m_exprHost.StyleHost.SetReportObjectModel(reportObjectModel);
				m_styleClass.SetStyleExprHost(m_exprHost.StyleHost);
			}
			if (m_markerStyleClass != null && m_exprHost.MarkerStyleHost != null)
			{
				m_exprHost.MarkerStyleHost.SetReportObjectModel(reportObjectModel);
				m_markerStyleClass.SetStyleExprHost(m_exprHost.MarkerStyleHost);
			}
			if (m_dataLabel != null && m_dataLabel.StyleClass != null && m_exprHost.DataLabelStyleHost != null)
			{
				m_dataLabel.SetExprHost(m_exprHost.DataLabelStyleHost, reportObjectModel);
			}
			if (m_customProperties != null && m_exprHost.CustomPropertyHostsRemotable != null)
			{
				m_customProperties.SetExprHost(m_exprHost.CustomPropertyHostsRemotable, reportObjectModel);
			}
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.DataValues, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfoList));
			memberInfoList.Add(new MemberInfo(MemberName.DataLabel, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ChartDataLabel));
			memberInfoList.Add(new MemberInfo(MemberName.Action, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.Action));
			memberInfoList.Add(new MemberInfo(MemberName.StyleClass, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.Style));
			memberInfoList.Add(new MemberInfo(MemberName.MarkerType, Token.Enum));
			memberInfoList.Add(new MemberInfo(MemberName.MarkerSize, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.MarkerStyleClass, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.Style));
			memberInfoList.Add(new MemberInfo(MemberName.DataElementName, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.DataElementOutput, Token.Enum));
			memberInfoList.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.CustomProperties, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.DataValueList));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
