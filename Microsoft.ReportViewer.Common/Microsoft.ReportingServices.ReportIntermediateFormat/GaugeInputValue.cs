using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.RdlExpressions;
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
	internal class GaugeInputValue : IPersistable
	{
		[NonSerialized]
		private GaugeInputValueExprHost m_exprHost;

		[Reference]
		private GaugePanel m_gaugePanel;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private ExpressionInfo m_value;

		private ExpressionInfo m_formula;

		private ExpressionInfo m_minPercent;

		private ExpressionInfo m_maxPercent;

		private ExpressionInfo m_multiplier;

		private ExpressionInfo m_addConstant;

		private string m_dataElementName;

		private DataElementOutputTypes m_dataElementOutput;

		private int m_exprHostID = -1;

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

		internal ExpressionInfo Formula
		{
			get
			{
				return m_formula;
			}
			set
			{
				m_formula = value;
			}
		}

		internal ExpressionInfo MinPercent
		{
			get
			{
				return m_minPercent;
			}
			set
			{
				m_minPercent = value;
			}
		}

		internal ExpressionInfo MaxPercent
		{
			get
			{
				return m_maxPercent;
			}
			set
			{
				m_maxPercent = value;
			}
		}

		internal ExpressionInfo Multiplier
		{
			get
			{
				return m_multiplier;
			}
			set
			{
				m_multiplier = value;
			}
		}

		internal ExpressionInfo AddConstant
		{
			get
			{
				return m_addConstant;
			}
			set
			{
				m_addConstant = value;
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

		internal string OwnerName => m_gaugePanel.Name;

		internal GaugeInputValueExprHost ExprHost => m_exprHost;

		internal int ExpressionHostID => m_exprHostID;

		internal GaugeInputValue()
		{
		}

		internal GaugeInputValue(GaugePanel gaugePanel)
		{
			m_gaugePanel = gaugePanel;
		}

		internal void Initialize(InitializationContext context, int index)
		{
			context.ExprHostBuilder.GaugeInputValueStart(index);
			if (m_value != null)
			{
				InitializeValue(context);
			}
			if (m_formula != null)
			{
				m_formula.Initialize("Formula", context);
				context.ExprHostBuilder.GaugeInputValueFormula(m_formula);
			}
			if (m_minPercent != null)
			{
				m_minPercent.Initialize("MinPercent", context);
				context.ExprHostBuilder.GaugeInputValueMinPercent(m_minPercent);
			}
			if (m_maxPercent != null)
			{
				m_maxPercent.Initialize("MaxPercent", context);
				context.ExprHostBuilder.GaugeInputValueMaxPercent(m_maxPercent);
			}
			if (m_multiplier != null)
			{
				m_multiplier.Initialize("Multiplier", context);
				context.ExprHostBuilder.GaugeInputValueMultiplier(m_multiplier);
			}
			if (m_addConstant != null)
			{
				m_addConstant.Initialize("AddConstant", context);
				context.ExprHostBuilder.GaugeInputValueAddConstant(m_addConstant);
			}
			m_exprHostID = context.ExprHostBuilder.GaugeInputValueEnd();
		}

		protected virtual void InitializeValue(InitializationContext context)
		{
			m_value.Initialize("Value", context);
			context.ExprHostBuilder.GaugeInputValueValue(m_value);
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			GaugeInputValue gaugeInputValue = (GaugeInputValue)MemberwiseClone();
			gaugeInputValue.m_gaugePanel = (GaugePanel)context.CurrentDataRegionClone;
			if (m_value != null)
			{
				gaugeInputValue.m_value = (ExpressionInfo)m_value.PublishClone(context);
			}
			if (m_formula != null)
			{
				gaugeInputValue.m_formula = (ExpressionInfo)m_formula.PublishClone(context);
			}
			if (m_minPercent != null)
			{
				gaugeInputValue.m_minPercent = (ExpressionInfo)m_minPercent.PublishClone(context);
			}
			if (m_maxPercent != null)
			{
				gaugeInputValue.m_maxPercent = (ExpressionInfo)m_maxPercent.PublishClone(context);
			}
			if (m_multiplier != null)
			{
				gaugeInputValue.m_multiplier = (ExpressionInfo)m_multiplier.PublishClone(context);
			}
			if (m_addConstant != null)
			{
				gaugeInputValue.m_addConstant = (ExpressionInfo)m_addConstant.PublishClone(context);
			}
			return gaugeInputValue;
		}

		internal void SetExprHost(GaugeInputValueExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			m_exprHost = exprHost;
			m_exprHost.SetReportObjectModel(reportObjectModel);
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Value, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Formula, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MinPercent, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MaxPercent, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Multiplier, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.AddConstant, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.DataElementName, Token.String));
			list.Add(new MemberInfo(MemberName.DataElementOutput, Token.Enum));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.GaugePanel, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePanel, Token.Reference));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeInputValue, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
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
				case MemberName.Value:
					writer.Write(m_value);
					break;
				case MemberName.Formula:
					writer.Write(m_formula);
					break;
				case MemberName.MinPercent:
					writer.Write(m_minPercent);
					break;
				case MemberName.MaxPercent:
					writer.Write(m_maxPercent);
					break;
				case MemberName.Multiplier:
					writer.Write(m_multiplier);
					break;
				case MemberName.AddConstant:
					writer.Write(m_addConstant);
					break;
				case MemberName.DataElementName:
					writer.Write(m_dataElementName);
					break;
				case MemberName.DataElementOutput:
					writer.WriteEnum((int)m_dataElementOutput);
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
				case MemberName.Value:
					m_value = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Formula:
					m_formula = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MinPercent:
					m_minPercent = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MaxPercent:
					m_maxPercent = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Multiplier:
					m_multiplier = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.AddConstant:
					m_addConstant = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.DataElementName:
					m_dataElementName = reader.ReadString();
					break;
				case MemberName.DataElementOutput:
					m_dataElementOutput = (DataElementOutputTypes)reader.ReadEnum();
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

		public Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeInputValue;
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

		internal Microsoft.ReportingServices.RdlExpressions.VariantResult EvaluateValue(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			Global.Tracer.Assert(m_gaugePanel.GaugeRow != null && m_gaugePanel.GaugeRow.GaugeCell != null);
			context.SetupContext(m_gaugePanel.GaugeRow.GaugeCell, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugeInputValueValueExpression(this, m_gaugePanel.Name);
		}

		internal GaugeInputValueFormulas EvaluateFormula(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			Global.Tracer.Assert(m_gaugePanel.GaugeRow != null && m_gaugePanel.GaugeRow.GaugeCell != null);
			context.SetupContext(m_gaugePanel.GaugeRow.GaugeCell, reportScopeInstance);
			return EnumTranslator.TranslateGaugeInputValueFormulas(context.ReportRuntime.EvaluateGaugeInputValueFormulaExpression(this, m_gaugePanel.Name), context.ReportRuntime);
		}

		internal double EvaluateMinPercent(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			Global.Tracer.Assert(m_gaugePanel.GaugeRow != null && m_gaugePanel.GaugeRow.GaugeCell != null);
			context.SetupContext(m_gaugePanel.GaugeRow.GaugeCell, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugeInputValueMinPercentExpression(this, m_gaugePanel.Name);
		}

		internal double EvaluateMaxPercent(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			Global.Tracer.Assert(m_gaugePanel.GaugeRow != null && m_gaugePanel.GaugeRow.GaugeCell != null);
			context.SetupContext(m_gaugePanel.GaugeRow.GaugeCell, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugeInputValueMaxPercentExpression(this, m_gaugePanel.Name);
		}

		internal double EvaluateMultiplier(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			Global.Tracer.Assert(m_gaugePanel.GaugeRow != null && m_gaugePanel.GaugeRow.GaugeCell != null);
			context.SetupContext(m_gaugePanel.GaugeRow.GaugeCell, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugeInputValueMultiplierExpression(this, m_gaugePanel.Name);
		}

		internal double EvaluateAddConstant(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			Global.Tracer.Assert(m_gaugePanel.GaugeRow != null && m_gaugePanel.GaugeRow.GaugeCell != null);
			context.SetupContext(m_gaugePanel.GaugeRow.GaugeCell, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugeInputValueAddConstantExpression(this, m_gaugePanel.Name);
		}
	}
}
