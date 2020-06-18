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
	internal sealed class NumericIndicatorRange : IPersistable
	{
		private int m_exprHostID = -1;

		[NonSerialized]
		private NumericIndicatorRangeExprHost m_exprHost;

		[Reference]
		private GaugePanel m_gaugePanel;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private string m_name;

		private GaugeInputValue m_startValue;

		private GaugeInputValue m_endValue;

		private ExpressionInfo m_decimalDigitColor;

		private ExpressionInfo m_digitColor;

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

		internal ExpressionInfo DecimalDigitColor
		{
			get
			{
				return m_decimalDigitColor;
			}
			set
			{
				m_decimalDigitColor = value;
			}
		}

		internal ExpressionInfo DigitColor
		{
			get
			{
				return m_digitColor;
			}
			set
			{
				m_digitColor = value;
			}
		}

		internal string OwnerName => m_gaugePanel.Name;

		internal NumericIndicatorRangeExprHost ExprHost => m_exprHost;

		internal int ExpressionHostID => m_exprHostID;

		internal NumericIndicatorRange()
		{
		}

		internal NumericIndicatorRange(GaugePanel gaugePanel)
		{
			m_gaugePanel = gaugePanel;
		}

		internal void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.NumericIndicatorRangeStart(m_name);
			if (m_decimalDigitColor != null)
			{
				m_decimalDigitColor.Initialize("DecimalDigitColor", context);
				context.ExprHostBuilder.NumericIndicatorRangeDecimalDigitColor(m_decimalDigitColor);
			}
			if (m_digitColor != null)
			{
				m_digitColor.Initialize("DigitColor", context);
				context.ExprHostBuilder.NumericIndicatorRangeDigitColor(m_digitColor);
			}
			m_exprHostID = context.ExprHostBuilder.NumericIndicatorRangeEnd();
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			NumericIndicatorRange numericIndicatorRange = (NumericIndicatorRange)MemberwiseClone();
			numericIndicatorRange.m_gaugePanel = (GaugePanel)context.CurrentDataRegionClone;
			if (m_startValue != null)
			{
				numericIndicatorRange.m_startValue = (GaugeInputValue)m_startValue.PublishClone(context);
			}
			if (m_endValue != null)
			{
				numericIndicatorRange.m_endValue = (GaugeInputValue)m_endValue.PublishClone(context);
			}
			if (m_decimalDigitColor != null)
			{
				numericIndicatorRange.m_decimalDigitColor = (ExpressionInfo)m_decimalDigitColor.PublishClone(context);
			}
			if (m_digitColor != null)
			{
				numericIndicatorRange.m_digitColor = (ExpressionInfo)m_digitColor.PublishClone(context);
			}
			return numericIndicatorRange;
		}

		internal void SetExprHost(NumericIndicatorRangeExprHost exprHost, ObjectModelImpl reportObjectModel)
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
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Name, Token.String));
			list.Add(new MemberInfo(MemberName.StartValue, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeInputValue));
			list.Add(new MemberInfo(MemberName.EndValue, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeInputValue));
			list.Add(new MemberInfo(MemberName.DecimalDigitColor, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.DigitColor, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.GaugePanel, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePanel, Token.Reference));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.NumericIndicatorRange, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
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
				case MemberName.DecimalDigitColor:
					writer.Write(m_decimalDigitColor);
					break;
				case MemberName.DigitColor:
					writer.Write(m_digitColor);
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
				case MemberName.DecimalDigitColor:
					m_decimalDigitColor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.DigitColor:
					m_digitColor = (ExpressionInfo)reader.ReadRIFObject();
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
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.NumericIndicatorRange;
		}

		internal string EvaluateDecimalDigitColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateNumericIndicatorRangeDecimalDigitColorExpression(this, m_gaugePanel.Name);
		}

		internal string EvaluateDigitColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateNumericIndicatorRangeDigitColorExpression(this, m_gaugePanel.Name);
		}
	}
}
