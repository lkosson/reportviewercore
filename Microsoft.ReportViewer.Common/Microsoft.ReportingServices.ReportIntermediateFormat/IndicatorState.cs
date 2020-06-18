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
	internal sealed class IndicatorState : IPersistable
	{
		private int m_exprHostID = -1;

		[NonSerialized]
		private IndicatorStateExprHost m_exprHost;

		[Reference]
		private GaugePanel m_gaugePanel;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private string m_name;

		private GaugeInputValue m_startValue;

		private GaugeInputValue m_endValue;

		private ExpressionInfo m_color;

		private ExpressionInfo m_scaleFactor;

		private ExpressionInfo m_indicatorStyle;

		private IndicatorImage m_indicatorImage;

		internal string Name
		{
			get
			{
				return m_name;
			}
			set
			{
				m_name = value;
			}
		}

		internal GaugeInputValue StartValue
		{
			get
			{
				return m_startValue;
			}
			set
			{
				m_startValue = value;
			}
		}

		internal GaugeInputValue EndValue
		{
			get
			{
				return m_endValue;
			}
			set
			{
				m_endValue = value;
			}
		}

		internal ExpressionInfo Color
		{
			get
			{
				return m_color;
			}
			set
			{
				m_color = value;
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

		internal string OwnerName => m_gaugePanel.Name;

		internal IndicatorStateExprHost ExprHost => m_exprHost;

		internal int ExpressionHostID => m_exprHostID;

		internal IndicatorState()
		{
		}

		internal IndicatorState(GaugePanel gaugePanel)
		{
			m_gaugePanel = gaugePanel;
		}

		internal void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.IndicatorStateStart(m_name);
			if (m_color != null)
			{
				m_color.Initialize("Color", context);
				context.ExprHostBuilder.IndicatorStateColor(m_color);
			}
			if (m_scaleFactor != null)
			{
				m_scaleFactor.Initialize("ScaleFactor", context);
				context.ExprHostBuilder.IndicatorStateScaleFactor(m_scaleFactor);
			}
			if (m_indicatorStyle != null)
			{
				m_indicatorStyle.Initialize("IndicatorStyle", context);
				context.ExprHostBuilder.IndicatorStateIndicatorStyle(m_indicatorStyle);
			}
			if (m_indicatorImage != null)
			{
				m_indicatorImage.Initialize(context);
			}
			m_exprHostID = context.ExprHostBuilder.IndicatorStateEnd();
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			IndicatorState indicatorState = (IndicatorState)MemberwiseClone();
			indicatorState.m_gaugePanel = (GaugePanel)context.CurrentDataRegionClone;
			if (m_startValue != null)
			{
				indicatorState.m_startValue = (GaugeInputValue)m_startValue.PublishClone(context);
			}
			if (m_endValue != null)
			{
				indicatorState.m_endValue = (GaugeInputValue)m_endValue.PublishClone(context);
			}
			if (m_color != null)
			{
				indicatorState.m_color = (ExpressionInfo)m_color.PublishClone(context);
			}
			if (m_scaleFactor != null)
			{
				indicatorState.m_scaleFactor = (ExpressionInfo)m_scaleFactor.PublishClone(context);
			}
			if (m_indicatorStyle != null)
			{
				indicatorState.m_indicatorStyle = (ExpressionInfo)m_indicatorStyle.PublishClone(context);
			}
			if (m_indicatorImage != null)
			{
				indicatorState.m_indicatorImage = (IndicatorImage)m_indicatorImage.PublishClone(context);
			}
			return indicatorState;
		}

		internal void SetExprHost(IndicatorStateExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			m_exprHost = exprHost;
			m_exprHost.SetReportObjectModel(reportObjectModel);
			if (m_startValue != null && ExprHost.StartValueHost != null)
			{
				m_startValue.SetExprHost(ExprHost.StartValueHost, reportObjectModel);
			}
			if (m_endValue != null && ExprHost.EndValueHost != null)
			{
				m_endValue.SetExprHost(ExprHost.EndValueHost, reportObjectModel);
			}
			if (m_indicatorImage != null && ExprHost.IndicatorImageHost != null)
			{
				m_indicatorImage.SetExprHost(ExprHost.IndicatorImageHost, reportObjectModel);
			}
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Name, Token.String));
			list.Add(new MemberInfo(MemberName.StartValue, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeInputValue));
			list.Add(new MemberInfo(MemberName.EndValue, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeInputValue));
			list.Add(new MemberInfo(MemberName.Color, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ScaleFactor, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.IndicatorStyle, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.IndicatorImage, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IndicatorImage));
			list.Add(new MemberInfo(MemberName.GaugePanel, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePanel, Token.Reference));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IndicatorState, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.GaugePanel:
					writer.WriteReference(m_gaugePanel);
					break;
				case MemberName.Name:
					writer.Write(m_name);
					break;
				case MemberName.StartValue:
					writer.Write(m_startValue);
					break;
				case MemberName.EndValue:
					writer.Write(m_endValue);
					break;
				case MemberName.Color:
					writer.Write(m_color);
					break;
				case MemberName.ScaleFactor:
					writer.Write(m_scaleFactor);
					break;
				case MemberName.IndicatorStyle:
					writer.Write(m_indicatorStyle);
					break;
				case MemberName.IndicatorImage:
					writer.Write(m_indicatorImage);
					break;
				case MemberName.ExprHostID:
					writer.Write(m_exprHostID);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.GaugePanel:
					m_gaugePanel = reader.ReadReference<GaugePanel>(this);
					break;
				case MemberName.Name:
					m_name = reader.ReadString();
					break;
				case MemberName.StartValue:
					m_startValue = (GaugeInputValue)reader.ReadRIFObject();
					break;
				case MemberName.EndValue:
					m_endValue = (GaugeInputValue)reader.ReadRIFObject();
					break;
				case MemberName.Color:
					m_color = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ScaleFactor:
					m_scaleFactor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.IndicatorStyle:
					m_indicatorStyle = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.IndicatorImage:
					m_indicatorImage = (IndicatorImage)reader.ReadRIFObject();
					break;
				case MemberName.ExprHostID:
					m_exprHostID = reader.ReadInt32();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			if (!memberReferencesCollection.TryGetValue(m_Declaration.ObjectType, out List<MemberReference> value))
			{
				return;
			}
			foreach (MemberReference item in value)
			{
				MemberName memberName = item.MemberName;
				if (memberName == MemberName.GaugePanel)
				{
					Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
					m_gaugePanel = (GaugePanel)referenceableItems[item.RefID];
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IndicatorState;
		}

		internal string EvaluateColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateIndicatorStateColorExpression(this, m_gaugePanel.Name);
		}

		internal double EvaluateScaleFactor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateIndicatorStateScaleFactorExpression(this, m_gaugePanel.Name);
		}

		internal GaugeStateIndicatorStyles EvaluateIndicatorStyle(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return EnumTranslator.TranslateGaugeStateIndicatorStyles(context.ReportRuntime.EvaluateIndicatorStateIndicatorStyleExpression(this, m_gaugePanel.Name), context.ReportRuntime);
		}
	}
}
