using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class StateIndicator : GaugePanelItem, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private GaugeInputValue m_gaugeInputValue;

		private ExpressionInfo m_transformationType;

		private string m_transformationScope;

		private GaugeInputValue m_maximumValue;

		private GaugeInputValue m_minimumValue;

		private ExpressionInfo m_indicatorStyle;

		private IndicatorImage m_indicatorImage;

		private ExpressionInfo m_scaleFactor;

		private List<IndicatorState> m_indicatorStates;

		private ExpressionInfo m_resizeMode;

		private ExpressionInfo m_angle;

		private string m_stateDataElementName;

		private DataElementOutputTypes m_stateDataElementOutput;

		internal GaugeInputValue GaugeInputValue
		{
			get
			{
				return m_gaugeInputValue;
			}
			set
			{
				m_gaugeInputValue = value;
			}
		}

		internal ExpressionInfo TransformationType
		{
			get
			{
				return m_transformationType;
			}
			set
			{
				m_transformationType = value;
			}
		}

		internal string TransformationScope
		{
			get
			{
				return m_transformationScope;
			}
			set
			{
				m_transformationScope = value;
			}
		}

		internal GaugeInputValue MaximumValue
		{
			get
			{
				return m_maximumValue;
			}
			set
			{
				m_maximumValue = value;
			}
		}

		internal GaugeInputValue MinimumValue
		{
			get
			{
				return m_minimumValue;
			}
			set
			{
				m_minimumValue = value;
			}
		}

		internal ExpressionInfo IndicatorStyle
		{
			get
			{
				return m_indicatorStyle;
			}
			set
			{
				m_indicatorStyle = value;
			}
		}

		internal IndicatorImage IndicatorImage
		{
			get
			{
				return m_indicatorImage;
			}
			set
			{
				m_indicatorImage = value;
			}
		}

		internal ExpressionInfo ScaleFactor
		{
			get
			{
				return m_scaleFactor;
			}
			set
			{
				m_scaleFactor = value;
			}
		}

		internal List<IndicatorState> IndicatorStates
		{
			get
			{
				return m_indicatorStates;
			}
			set
			{
				m_indicatorStates = value;
			}
		}

		internal ExpressionInfo ResizeMode
		{
			get
			{
				return m_resizeMode;
			}
			set
			{
				m_resizeMode = value;
			}
		}

		internal ExpressionInfo Angle
		{
			get
			{
				return m_angle;
			}
			set
			{
				m_angle = value;
			}
		}

		internal string StateDataElementName
		{
			get
			{
				return m_stateDataElementName;
			}
			set
			{
				m_stateDataElementName = value;
			}
		}

		internal DataElementOutputTypes StateDataElementOutput
		{
			get
			{
				return m_stateDataElementOutput;
			}
			set
			{
				m_stateDataElementOutput = value;
			}
		}

		internal new StateIndicatorExprHost ExprHost => (StateIndicatorExprHost)m_exprHost;

		internal StateIndicator()
		{
		}

		internal StateIndicator(GaugePanel gaugePanel, int id)
			: base(gaugePanel, id)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.StateIndicatorStart(m_name);
			base.Initialize(context);
			if (m_transformationType != null)
			{
				m_transformationType.Initialize("TransformationType", context);
				context.ExprHostBuilder.StateIndicatorTransformationType(m_transformationType);
			}
			if (m_indicatorStyle != null)
			{
				m_indicatorStyle.Initialize("IndicatorStyle", context);
				context.ExprHostBuilder.StateIndicatorIndicatorStyle(m_indicatorStyle);
			}
			if (m_indicatorImage != null)
			{
				m_indicatorImage.Initialize(context);
			}
			if (m_scaleFactor != null)
			{
				m_scaleFactor.Initialize("ScaleFactor", context);
				context.ExprHostBuilder.StateIndicatorScaleFactor(m_scaleFactor);
			}
			if (m_indicatorStates != null)
			{
				for (int i = 0; i < m_indicatorStates.Count; i++)
				{
					m_indicatorStates[i].Initialize(context);
				}
			}
			if (m_resizeMode != null)
			{
				m_resizeMode.Initialize("ResizeMode", context);
				context.ExprHostBuilder.StateIndicatorResizeMode(m_resizeMode);
			}
			if (m_angle != null)
			{
				m_angle.Initialize("Angle", context);
				context.ExprHostBuilder.StateIndicatorAngle(m_angle);
			}
			m_exprHostID = context.ExprHostBuilder.StateIndicatorEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			StateIndicator stateIndicator = (StateIndicator)base.PublishClone(context);
			if (m_gaugeInputValue != null)
			{
				stateIndicator.m_gaugeInputValue = (GaugeInputValue)m_gaugeInputValue.PublishClone(context);
			}
			if (m_transformationType != null)
			{
				stateIndicator.m_transformationType = (ExpressionInfo)m_transformationType.PublishClone(context);
			}
			if (m_maximumValue != null)
			{
				stateIndicator.m_maximumValue = (GaugeInputValue)m_maximumValue.PublishClone(context);
			}
			if (m_minimumValue != null)
			{
				stateIndicator.m_minimumValue = (GaugeInputValue)m_minimumValue.PublishClone(context);
			}
			if (m_indicatorStyle != null)
			{
				stateIndicator.m_indicatorStyle = (ExpressionInfo)m_indicatorStyle.PublishClone(context);
			}
			if (m_indicatorImage != null)
			{
				stateIndicator.m_indicatorImage = (IndicatorImage)m_indicatorImage.PublishClone(context);
			}
			if (m_scaleFactor != null)
			{
				stateIndicator.m_scaleFactor = (ExpressionInfo)m_scaleFactor.PublishClone(context);
			}
			if (m_indicatorStates != null)
			{
				stateIndicator.m_indicatorStates = new List<IndicatorState>(m_indicatorStates.Count);
				foreach (IndicatorState indicatorState in m_indicatorStates)
				{
					stateIndicator.m_indicatorStates.Add((IndicatorState)indicatorState.PublishClone(context));
				}
			}
			if (m_resizeMode != null)
			{
				stateIndicator.m_resizeMode = (ExpressionInfo)m_resizeMode.PublishClone(context);
			}
			if (m_angle != null)
			{
				stateIndicator.m_angle = (ExpressionInfo)m_angle.PublishClone(context);
			}
			return stateIndicator;
		}

		internal void SetExprHost(StateIndicatorExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			SetExprHost((GaugePanelItemExprHost)exprHost, reportObjectModel);
			if (m_gaugeInputValue != null && ExprHost.GaugeInputValueHost != null)
			{
				m_gaugeInputValue.SetExprHost(ExprHost.GaugeInputValueHost, reportObjectModel);
			}
			if (m_maximumValue != null && ExprHost.MaximumValueHost != null)
			{
				m_maximumValue.SetExprHost(ExprHost.MaximumValueHost, reportObjectModel);
			}
			if (m_minimumValue != null && ExprHost.MinimumValueHost != null)
			{
				m_minimumValue.SetExprHost(ExprHost.MinimumValueHost, reportObjectModel);
			}
			if (m_indicatorImage != null && ExprHost.IndicatorImageHost != null)
			{
				m_indicatorImage.SetExprHost(ExprHost.IndicatorImageHost, reportObjectModel);
			}
			IList<IndicatorStateExprHost> indicatorStatesHostsRemotable = ExprHost.IndicatorStatesHostsRemotable;
			if (m_indicatorStates == null || indicatorStatesHostsRemotable == null)
			{
				return;
			}
			for (int i = 0; i < m_indicatorStates.Count; i++)
			{
				IndicatorState indicatorState = m_indicatorStates[i];
				if (indicatorState != null && indicatorState.ExpressionHostID > -1)
				{
					indicatorState.SetExprHost(indicatorStatesHostsRemotable[indicatorState.ExpressionHostID], reportObjectModel);
				}
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.GaugeInputValue, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeInputValue));
			list.Add(new MemberInfo(MemberName.IndicatorStyle, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.IndicatorImage, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IndicatorImage));
			list.Add(new MemberInfo(MemberName.ScaleFactor, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.IndicatorStates, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IndicatorState));
			list.Add(new MemberInfo(MemberName.ResizeMode, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Angle, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.TransformationType, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.TransformationScope, Token.String));
			list.Add(new MemberInfo(MemberName.MaximumValue, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeInputValue));
			list.Add(new MemberInfo(MemberName.MinimumValue, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeInputValue));
			list.Add(new MemberInfo(MemberName.StateDataElementName, Token.String));
			list.Add(new MemberInfo(MemberName.StateDataElementOutput, Token.Enum));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StateIndicator, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePanelItem, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.GaugeInputValue:
					writer.Write(m_gaugeInputValue);
					break;
				case MemberName.TransformationType:
					writer.Write(m_transformationType);
					break;
				case MemberName.TransformationScope:
					writer.Write(m_transformationScope);
					break;
				case MemberName.MaximumValue:
					writer.Write(m_maximumValue);
					break;
				case MemberName.MinimumValue:
					writer.Write(m_minimumValue);
					break;
				case MemberName.IndicatorStyle:
					writer.Write(m_indicatorStyle);
					break;
				case MemberName.IndicatorImage:
					writer.Write(m_indicatorImage);
					break;
				case MemberName.ScaleFactor:
					writer.Write(m_scaleFactor);
					break;
				case MemberName.IndicatorStates:
					writer.Write(m_indicatorStates);
					break;
				case MemberName.ResizeMode:
					writer.Write(m_resizeMode);
					break;
				case MemberName.Angle:
					writer.Write(m_angle);
					break;
				case MemberName.StateDataElementName:
					writer.Write(m_stateDataElementName);
					break;
				case MemberName.StateDataElementOutput:
					writer.WriteEnum((int)m_stateDataElementOutput);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.GaugeInputValue:
					m_gaugeInputValue = (GaugeInputValue)reader.ReadRIFObject();
					break;
				case MemberName.TransformationType:
					m_transformationType = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.TransformationScope:
					m_transformationScope = reader.ReadString();
					break;
				case MemberName.MaximumValue:
					m_maximumValue = (GaugeInputValue)reader.ReadRIFObject();
					break;
				case MemberName.MinimumValue:
					m_minimumValue = (GaugeInputValue)reader.ReadRIFObject();
					break;
				case MemberName.IndicatorStyle:
					m_indicatorStyle = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.IndicatorImage:
					m_indicatorImage = (IndicatorImage)reader.ReadRIFObject();
					break;
				case MemberName.ScaleFactor:
					m_scaleFactor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.IndicatorStates:
					m_indicatorStates = reader.ReadGenericListOfRIFObjects<IndicatorState>();
					break;
				case MemberName.ResizeMode:
					m_resizeMode = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Angle:
					m_angle = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.StateDataElementName:
					m_stateDataElementName = reader.ReadString();
					break;
				case MemberName.StateDataElementOutput:
					m_stateDataElementOutput = (DataElementOutputTypes)reader.ReadEnum();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
				if (reader.IntermediateFormatVersion.CompareTo(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.IntermediateFormatVersion.SQL16) < 0)
				{
					if (!m_styleClass.GetAttributeInfo("BorderStyle", out AttributeInfo styleAttribute))
					{
						m_styleClass.AddAttribute("BorderStyle", new ExpressionInfo
						{
							StringValue = "Solid",
							ConstantType = DataType.String
						});
					}
					else
					{
						styleAttribute.IsExpression = false;
						styleAttribute.Value = "Solid";
					}
				}
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StateIndicator;
		}

		internal GaugeTransformationType EvaluateTransformationType(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return EnumTranslator.TranslateGaugeTransformationType(context.ReportRuntime.EvaluateStateIndicatorTransformationTypeExpression(this, m_gaugePanel.Name), context.ReportRuntime);
		}

		internal GaugeStateIndicatorStyles EvaluateIndicatorStyle(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return EnumTranslator.TranslateGaugeStateIndicatorStyles(context.ReportRuntime.EvaluateStateIndicatorIndicatorStyleExpression(this, m_gaugePanel.Name), context.ReportRuntime);
		}

		internal double EvaluateScaleFactor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateStateIndicatorScaleFactorExpression(this, m_gaugePanel.Name);
		}

		internal GaugeResizeModes EvaluateResizeMode(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return EnumTranslator.TranslateGaugeResizeModes(context.ReportRuntime.EvaluateStateIndicatorResizeModeExpression(this, m_gaugePanel.Name), context.ReportRuntime);
		}

		internal double EvaluateAngle(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateStateIndicatorAngleExpression(this, m_gaugePanel.Name);
		}
	}
}
