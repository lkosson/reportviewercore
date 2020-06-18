using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel;
using Microsoft.ReportingServices.ReportProcessing.Persistence;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ParameterDef : ParameterBase, IParameterDef
	{
		private ParameterDataSource m_validValuesDataSource;

		private ExpressionInfoList m_validValuesValueExpressions;

		private ExpressionInfoList m_validValuesLabelExpressions;

		private ParameterDataSource m_defaultDataSource;

		private ExpressionInfoList m_defaultExpressions;

		private ParameterDefList m_dependencyList;

		private int m_exprHostID = -1;

		private string m_prompt;

		[NonSerialized]
		private ReportParamExprHost m_exprHost;

		internal ExpressionInfoList DefaultExpressions
		{
			get
			{
				return m_defaultExpressions;
			}
			set
			{
				m_defaultExpressions = value;
			}
		}

		internal ParameterDataSource ValidValuesDataSource
		{
			get
			{
				return m_validValuesDataSource;
			}
			set
			{
				m_validValuesDataSource = value;
			}
		}

		internal ExpressionInfoList ValidValuesValueExpressions
		{
			get
			{
				return m_validValuesValueExpressions;
			}
			set
			{
				m_validValuesValueExpressions = value;
			}
		}

		internal ExpressionInfoList ValidValuesLabelExpressions
		{
			get
			{
				return m_validValuesLabelExpressions;
			}
			set
			{
				m_validValuesLabelExpressions = value;
			}
		}

		internal ParameterDataSource DefaultDataSource
		{
			get
			{
				return m_defaultDataSource;
			}
			set
			{
				m_defaultDataSource = value;
			}
		}

		internal ParameterDefList DependencyList
		{
			get
			{
				return m_dependencyList;
			}
			set
			{
				m_dependencyList = value;
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

		internal ReportParamExprHost ExprHost => m_exprHost;

		public override string Prompt
		{
			get
			{
				return m_prompt;
			}
			set
			{
				m_prompt = value;
			}
		}

		int IParameterDef.DefaultValuesExpressionCount
		{
			get
			{
				if (DefaultExpressions == null)
				{
					return 0;
				}
				return DefaultExpressions.Count;
			}
		}

		int IParameterDef.ValidValuesValueExpressionCount
		{
			get
			{
				if (ValidValuesValueExpressions == null)
				{
					return 0;
				}
				return ValidValuesValueExpressions.Count;
			}
		}

		int IParameterDef.ValidValuesLabelExpressionCount
		{
			get
			{
				if (ValidValuesLabelExpressions == null)
				{
					return 0;
				}
				return ValidValuesLabelExpressions.Count;
			}
		}

		string IParameterDef.Name => base.Name;

		ObjectType IParameterDef.ParameterObjectType => ObjectType.ReportParameter;

		DataType IParameterDef.DataType => base.DataType;

		bool IParameterDef.MultiValue => base.MultiValue;

		IParameterDataSource IParameterDef.DefaultDataSource => DefaultDataSource;

		IParameterDataSource IParameterDef.ValidValuesDataSource => ValidValuesDataSource;

		bool IParameterDef.HasDefaultValuesExpressions()
		{
			return DefaultExpressions != null;
		}

		bool IParameterDef.HasValidValuesValueExpressions()
		{
			return ValidValuesValueExpressions != null;
		}

		bool IParameterDef.HasValidValuesLabelExpressions()
		{
			return ValidValuesLabelExpressions != null;
		}

		bool IParameterDef.HasDefaultValuesDataSource()
		{
			return DefaultDataSource != null;
		}

		bool IParameterDef.HasValidValuesDataSource()
		{
			return ValidValuesDataSource != null;
		}

		bool IParameterDef.ValidateValueForNull(object newValue, ErrorContext errorContext, string parameterValueProperty)
		{
			return ParameterBase.ValidateValueForNull(newValue, base.Nullable, errorContext, ObjectType.ReportParameter, base.Name, parameterValueProperty);
		}

		bool IParameterDef.ValidateValueForBlank(object newValue, ErrorContext errorContext, string parameterValueProperty)
		{
			return ValidateValueForBlank(newValue, errorContext, parameterValueProperty);
		}

		internal void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.ReportParameterStart(base.Name);
			if (m_defaultExpressions != null)
			{
				for (int num = m_defaultExpressions.Count - 1; num >= 0; num--)
				{
					context.ExprHostBuilder.ReportParameterDefaultValue(m_defaultExpressions[num]);
				}
			}
			if (m_validValuesValueExpressions != null)
			{
				context.ExprHostBuilder.ReportParameterValidValuesStart();
				for (int num2 = m_validValuesValueExpressions.Count - 1; num2 >= 0; num2--)
				{
					ExpressionInfo expressionInfo = m_validValuesValueExpressions[num2];
					if (expressionInfo != null)
					{
						context.ExprHostBuilder.ReportParameterValidValue(expressionInfo);
					}
				}
				context.ExprHostBuilder.ReportParameterValidValuesEnd();
			}
			if (m_validValuesLabelExpressions != null)
			{
				context.ExprHostBuilder.ReportParameterValidValueLabelsStart();
				for (int num3 = m_validValuesLabelExpressions.Count - 1; num3 >= 0; num3--)
				{
					ExpressionInfo expressionInfo2 = m_validValuesLabelExpressions[num3];
					if (expressionInfo2 != null)
					{
						context.ExprHostBuilder.ReportParameterValidValueLabel(expressionInfo2);
					}
				}
				context.ExprHostBuilder.ReportParameterValidValueLabelsEnd();
			}
			ExprHostID = context.ExprHostBuilder.ReportParameterEnd();
		}

		internal void SetExprHost(ReportExprHost reportExprHost, ObjectModel reportObjectModel)
		{
			Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null, "(reportExprHost != null && reportObjectModel != null)");
			if (ExprHostID >= 0)
			{
				m_exprHost = reportExprHost.ReportParameterHostsRemotable[ExprHostID];
				m_exprHost.SetReportObjectModel(reportObjectModel);
				if (m_exprHost.ValidValuesHost != null)
				{
					m_exprHost.ValidValuesHost.SetReportObjectModel(reportObjectModel);
				}
				if (m_exprHost.ValidValueLabelsHost != null)
				{
					m_exprHost.ValidValueLabelsHost.SetReportObjectModel(reportObjectModel);
				}
			}
		}

		internal void Parse(string name, List<string> defaultValues, string type, string nullable, string prompt, string promptUser, string allowBlank, string multiValue, string usedInQuery, bool hidden, ErrorContext errorContext, CultureInfo language)
		{
			base.Parse(name, defaultValues, type, nullable, prompt, promptUser, allowBlank, multiValue, usedInQuery, hidden, errorContext, language);
			if (hidden)
			{
				m_prompt = "";
			}
			else if (prompt == null)
			{
				m_prompt = name + ":";
			}
			else
			{
				m_prompt = prompt;
			}
			if (m_validValuesValueExpressions == null || DataType.Boolean == base.DataType)
			{
				return;
			}
			int num = m_validValuesValueExpressions.Count - 1;
			while (true)
			{
				if (num < 0)
				{
					return;
				}
				ExpressionInfo expressionInfo = m_validValuesValueExpressions[num];
				if (expressionInfo == null && base.MultiValue)
				{
					m_validValuesValueExpressions.RemoveAt(num);
				}
				else if (expressionInfo != null && ExpressionInfo.Types.Constant == expressionInfo.Type)
				{
					if (!ParameterBase.CastFromString(expressionInfo.Value, out object newValue, base.DataType, language))
					{
						if (errorContext == null)
						{
							break;
						}
						errorContext.Register(ProcessingErrorCode.rsParameterPropertyTypeMismatch, Severity.Error, base.ParameterObjectType, name, "ValidValue");
					}
					else
					{
						ValidateValue(newValue, errorContext, base.ParameterObjectType, "ValidValue");
					}
				}
				num--;
			}
			throw new ReportParameterTypeMismatchException(name);
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.ValidValuesDataSource, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ParameterDataSource));
			memberInfoList.Add(new MemberInfo(MemberName.ValidValuesValueExpression, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfoList));
			memberInfoList.Add(new MemberInfo(MemberName.ValidValuesLabelExpression, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfoList));
			memberInfoList.Add(new MemberInfo(MemberName.DefaultValueDataSource, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ParameterDataSource));
			memberInfoList.Add(new MemberInfo(MemberName.ExpressionList, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfoList));
			memberInfoList.Add(new MemberInfo(MemberName.DependencyList, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ParameterDefList));
			memberInfoList.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ParameterBase, memberInfoList);
		}
	}
}
