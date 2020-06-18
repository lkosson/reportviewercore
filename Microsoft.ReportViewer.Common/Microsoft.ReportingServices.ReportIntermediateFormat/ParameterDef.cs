using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class ParameterDef : ParameterBase, IPersistable, IParameterDef, IReferenceable
	{
		private ParameterDataSource m_validValuesDataSource;

		private List<ExpressionInfo> m_validValuesValueExpressions;

		private List<ExpressionInfo> m_validValuesLabelExpressions;

		private ParameterDataSource m_defaultDataSource;

		private List<ExpressionInfo> m_defaultExpressions;

		[Reference]
		private List<ParameterDef> m_dependencyList;

		private int m_exprHostID = -1;

		private ExpressionInfo m_prompt;

		private int m_referenceId = -2;

		[NonSerialized]
		private ReportParamExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

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

		Microsoft.ReportingServices.ReportProcessing.ObjectType IParameterDef.ParameterObjectType => Microsoft.ReportingServices.ReportProcessing.ObjectType.ReportParameter;

		DataType IParameterDef.DataType => base.DataType;

		bool IParameterDef.MultiValue => base.MultiValue;

		IParameterDataSource IParameterDef.DefaultDataSource => DefaultDataSource;

		IParameterDataSource IParameterDef.ValidValuesDataSource => ValidValuesDataSource;

		public override string Prompt
		{
			get
			{
				return m_prompt.StringValue;
			}
			set
			{
				m_prompt = ExpressionInfo.CreateConstExpression(value);
			}
		}

		public ExpressionInfo PromptExpression
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

		internal List<ExpressionInfo> DefaultExpressions
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

		internal List<ExpressionInfo> ValidValuesValueExpressions
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

		internal List<ExpressionInfo> ValidValuesLabelExpressions
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

		internal List<ParameterDef> DependencyList
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

		public int ID => m_referenceId;

		internal ParameterDef()
		{
		}

		internal ParameterDef(int referenceId)
		{
			m_referenceId = referenceId;
		}

		bool IParameterDef.HasDefaultValuesExpressions()
		{
			return DefaultExpressions != null;
		}

		bool IParameterDef.HasValidValuesLabelExpressions()
		{
			return ValidValuesLabelExpressions != null;
		}

		bool IParameterDef.HasValidValuesValueExpressions()
		{
			return ValidValuesValueExpressions != null;
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
			return ParameterBase.ValidateValueForNull(newValue, base.Nullable, errorContext, Microsoft.ReportingServices.ReportProcessing.ObjectType.ReportParameter, base.Name, parameterValueProperty);
		}

		bool IParameterDef.ValidateValueForBlank(object newValue, ErrorContext errorContext, string parameterValueProperty)
		{
			return ValidateValueForBlank(newValue, errorContext, parameterValueProperty);
		}

		internal void Initialize(InitializationContext context)
		{
			context.Location |= Microsoft.ReportingServices.ReportPublishing.LocationFlags.InParameter;
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
			if (m_prompt != null)
			{
				context.ExprHostBuilder.ReportParameterPromptExpression(m_prompt);
			}
			ExprHostID = context.ExprHostBuilder.ReportParameterEnd();
		}

		internal void SetExprHost(ReportExprHost reportExprHost, OnDemandObjectModel reportObjectModel)
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

		internal void Parse(string name, List<string> defaultValues, string type, string nullable, ExpressionInfo prompt, string promptUser, string allowBlank, string multiValue, string usedInQuery, bool hidden, ErrorContext errorContext, CultureInfo language)
		{
			base.Parse(name, defaultValues, type, nullable, prompt, promptUser, allowBlank, multiValue, usedInQuery, hidden, errorContext, language);
			if (hidden)
			{
				m_prompt = ExpressionInfo.CreateConstExpression("");
			}
			else if (prompt == null)
			{
				m_prompt = ExpressionInfo.CreateConstExpression(name + ":");
			}
			else
			{
				m_prompt = prompt;
			}
			ValidateExpressionDataTypes(m_validValuesValueExpressions, errorContext, name, "ValidValue", fromValidValues: true, language);
			ValidateExpressionDataTypes(m_defaultExpressions, errorContext, name, "DefaultValue", fromValidValues: false, language);
		}

		private void ValidateExpressionDataTypes(List<ExpressionInfo> expressions, ErrorContext errorContext, string paramName, string memberName, bool fromValidValues, CultureInfo language)
		{
			if (expressions == null)
			{
				return;
			}
			int num = expressions.Count - 1;
			while (true)
			{
				if (num < 0)
				{
					return;
				}
				ExpressionInfo expressionInfo = expressions[num];
				if (fromValidValues && expressionInfo == null && base.MultiValue)
				{
					m_validValuesValueExpressions.RemoveAt(num);
				}
				else if (expressionInfo != null && ExpressionInfo.Types.Constant == expressionInfo.Type)
				{
					if (!ParameterBase.CastFromString(expressionInfo.StringValue, out object newValue, base.DataType, language))
					{
						if (errorContext == null)
						{
							break;
						}
						errorContext.Register(ProcessingErrorCode.rsParameterPropertyTypeMismatch, Severity.Error, Microsoft.ReportingServices.ReportProcessing.ObjectType.ReportParameter, paramName, memberName);
					}
					else
					{
						ValidateValue(newValue, errorContext, Microsoft.ReportingServices.ReportProcessing.ObjectType.ReportParameter, memberName);
						if (newValue != null && base.DataType != DataType.String)
						{
							ExpressionInfo expressionInfo2 = new ExpressionInfo();
							expressionInfo2.Type = ExpressionInfo.Types.Constant;
							expressionInfo2.OriginalText = expressionInfo.OriginalText;
							expressionInfo2.ConstantType = base.DataType;
							expressions[num] = expressionInfo2;
							switch (base.DataType)
							{
							case DataType.Boolean:
								expressionInfo2.BoolValue = (bool)newValue;
								break;
							case DataType.DateTime:
								if (newValue is DateTimeOffset)
								{
									expressionInfo2.SetDateTimeValue((DateTimeOffset)newValue);
								}
								else
								{
									expressionInfo2.SetDateTimeValue((DateTime)newValue);
								}
								break;
							case DataType.Float:
								expressionInfo2.FloatValue = (double)newValue;
								break;
							case DataType.Integer:
								expressionInfo2.IntValue = (int)newValue;
								break;
							}
						}
					}
				}
				num--;
			}
			throw new ReportParameterTypeMismatchException(paramName);
		}

		[SkipMemberStaticValidation(MemberName.DependencyList)]
		private new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.ValidValuesDataSource, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterDataSource));
			list.Add(new MemberInfo(MemberName.ValidValuesValueExpression, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ValidValuesLabelExpression, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.DefaultValueDataSource, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterDataSource));
			list.Add(new MemberInfo(MemberName.ExpressionList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.DependencyList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterDef));
			list.Add(new MemberInfo(MemberName.DependencyRefList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Token.Reference, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterDef));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.Prompt, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ReferenceID, Token.Int32));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterDef, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterBase, list);
		}

		void IPersistable.Serialize(IntermediateFormatWriter writer)
		{
			Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Prompt:
					writer.Write(m_prompt);
					break;
				case MemberName.ValidValuesDataSource:
					writer.Write(m_validValuesDataSource);
					break;
				case MemberName.ValidValuesValueExpression:
					writer.Write(m_validValuesValueExpressions);
					break;
				case MemberName.ValidValuesLabelExpression:
					writer.Write(m_validValuesLabelExpressions);
					break;
				case MemberName.DefaultValueDataSource:
					writer.Write(m_defaultDataSource);
					break;
				case MemberName.ExpressionList:
					writer.Write(m_defaultExpressions);
					break;
				case MemberName.DependencyList:
					writer.Write((List<ParameterDef>)null);
					break;
				case MemberName.DependencyRefList:
					writer.WriteListOfReferences(m_dependencyList);
					break;
				case MemberName.ExprHostID:
					writer.Write(m_exprHostID);
					break;
				case MemberName.ReferenceID:
					writer.Write(m_referenceId);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		void IPersistable.Deserialize(IntermediateFormatReader reader)
		{
			Deserialize(reader);
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Prompt:
					m_prompt = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ValidValuesDataSource:
					m_validValuesDataSource = (ParameterDataSource)reader.ReadRIFObject();
					break;
				case MemberName.ValidValuesValueExpression:
					m_validValuesValueExpressions = reader.ReadGenericListOfRIFObjects<ExpressionInfo>();
					break;
				case MemberName.ValidValuesLabelExpression:
					m_validValuesLabelExpressions = reader.ReadGenericListOfRIFObjects<ExpressionInfo>();
					break;
				case MemberName.DefaultValueDataSource:
					m_defaultDataSource = (ParameterDataSource)reader.ReadRIFObject();
					break;
				case MemberName.ExpressionList:
					m_defaultExpressions = reader.ReadGenericListOfRIFObjects<ExpressionInfo>();
					break;
				case MemberName.DependencyList:
				{
					List<ParameterDef> list = reader.ReadGenericListOfRIFObjects<ParameterDef>();
					if (list != null)
					{
						m_dependencyList = list;
					}
					break;
				}
				case MemberName.DependencyRefList:
					m_dependencyList = reader.ReadGenericListOfReferences<ParameterDef>(this);
					break;
				case MemberName.ExprHostID:
					m_exprHostID = reader.ReadInt32();
					break;
				case MemberName.ReferenceID:
					m_referenceId = reader.ReadInt32();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		void IPersistable.ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			if (!memberReferencesCollection.TryGetValue(m_Declaration.ObjectType, out List<MemberReference> value))
			{
				return;
			}
			foreach (MemberReference item2 in value)
			{
				MemberName memberName = item2.MemberName;
				if (memberName == MemberName.DependencyRefList)
				{
					referenceableItems.TryGetValue(item2.RefID, out IReferenceable value2);
					ParameterDef item = value2 as ParameterDef;
					if (m_dependencyList == null)
					{
						m_dependencyList = new List<ParameterDef>();
					}
					m_dependencyList.Add(item);
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterDef;
		}
	}
}
