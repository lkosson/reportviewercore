using Microsoft.Cloud.Platform.Utils;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Xml;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ParameterInfo : ParameterBase, IPersistable
	{
		internal const string ParametersXmlElement = "Parameters";

		internal const string ParameterXmlElement = "Parameter";

		internal const string ValidValueXmlElement = "ValidValue";

		internal const string LabelXmlElement = "Label";

		internal const string ValuesXmlElement = "Values";

		internal const string ParametersLayoutXmlElement = "ParametersLayout";

		internal const string ParametersGridLayoutDefinitionXmlElement = "ParametersGridLayoutDefinition";

		internal const string ColumnsDefinitionXmlElement = "ColumnsDefinition";

		internal const string NumberOfColumnsXmlElement = "NumberOfColumns";

		internal const string NumberOfRowsXmlElement = "NumberOfRows";

		internal const string CellDefinitionsXmlElement = "CellDefinitions";

		internal const string CellDefinitionXmlElement = "CellDefinition";

		internal const string RowIndexXmlElement = "RowIndex";

		internal const string ColumnsIndexXmlElement = "ColumnIndex";

		internal const string ParameterNameXmlElement = "ParameterName";

		internal const string DynamicValidValuesXmlElement = "DynamicValidValues";

		internal const string DynamicDefaultValueXmlElement = "DynamicDefaultValue";

		internal const string DynamicPromptXmlElement = "DynamicPrompt";

		internal const string DependenciesXmlElement = "Dependencies";

		internal const string DependencyXmlElement = "Dependency";

		internal const string UserProfileStateElement = "UserProfileState";

		internal const string UseExplicitDefaultValueXmlElement = "UseExplicitDefaultValue";

		internal const string ValuesChangedXmlElement = "ValuesChanged";

		internal const string IsUserSuppliedXmlElement = "IsUserSupplied";

		internal const string NilXmlAttribute = "nil";

		private object[] m_values;

		private string[] m_labels;

		private bool m_isUserSupplied;

		private bool m_dynamicValidValues;

		private bool m_dynamicDefaultValue;

		private bool m_dynamicPrompt;

		private string m_prompt;

		private ParameterInfoCollection m_dependencyList;

		private ValidValueList m_validValues;

		private int[] m_dependencyIndexList;

		[NonSerialized]
		private bool m_valuesChanged;

		[NonSerialized]
		private ReportParameterState m_state = ReportParameterState.MissingValidValue;

		[NonSerialized]
		private bool m_othersDependOnMe;

		[NonSerialized]
		private bool m_useExplicitDefaultValue;

		[NonSerialized]
		private int m_indexInCollection = -1;

		[NonSerialized]
		private bool m_missingUpstreamDataSourcePrompt;

		[NonSerialized]
		private static readonly Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.Declaration m_Declaration = GetNewDeclaration();

		public object[] Values
		{
			get
			{
				return m_values;
			}
			set
			{
				m_values = value;
			}
		}

		public string[] Labels
		{
			get
			{
				return m_labels;
			}
			set
			{
				m_labels = value;
			}
		}

		public ValidValueList ValidValues
		{
			get
			{
				return m_validValues;
			}
			set
			{
				m_validValues = value;
			}
		}

		public bool DynamicValidValues
		{
			get
			{
				return m_dynamicValidValues;
			}
			set
			{
				m_dynamicValidValues = value;
			}
		}

		public bool DynamicDefaultValue
		{
			get
			{
				return m_dynamicDefaultValue;
			}
			set
			{
				m_dynamicDefaultValue = value;
			}
		}

		public bool UseExplicitDefaultValue
		{
			get
			{
				return m_useExplicitDefaultValue;
			}
			set
			{
				m_useExplicitDefaultValue = value;
			}
		}

		public ParameterInfoCollection DependencyList
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

		internal bool IsUserSupplied
		{
			get
			{
				return m_isUserSupplied;
			}
			set
			{
				m_isUserSupplied = value;
			}
		}

		internal bool ValuesChanged
		{
			get
			{
				return m_valuesChanged;
			}
			set
			{
				m_valuesChanged = value;
			}
		}

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

		public bool DynamicPrompt
		{
			get
			{
				return m_dynamicPrompt;
			}
			set
			{
				m_dynamicPrompt = value;
			}
		}

		public ReportParameterState State
		{
			get
			{
				return m_state;
			}
			set
			{
				m_state = value;
			}
		}

		public bool OthersDependOnMe
		{
			get
			{
				return m_othersDependOnMe;
			}
			set
			{
				m_othersDependOnMe = value;
			}
		}

		public bool IsVisible
		{
			get
			{
				if (base.PromptUser && Prompt != null)
				{
					return Prompt.Length > 0;
				}
				return false;
			}
		}

		internal int IndexInCollection
		{
			get
			{
				return m_indexInCollection;
			}
			set
			{
				m_indexInCollection = value;
			}
		}

		internal bool MissingUpstreamDataSourcePrompt
		{
			get
			{
				return m_missingUpstreamDataSourcePrompt;
			}
			set
			{
				m_missingUpstreamDataSourcePrompt = value;
			}
		}

		public ParameterInfo()
		{
		}

		internal ParameterInfo(ParameterInfo source)
			: base(source)
		{
			m_isUserSupplied = source.m_isUserSupplied;
			m_valuesChanged = source.m_valuesChanged;
			m_dynamicValidValues = source.m_dynamicValidValues;
			m_dynamicDefaultValue = source.m_dynamicDefaultValue;
			m_state = source.State;
			m_othersDependOnMe = source.m_othersDependOnMe;
			m_useExplicitDefaultValue = source.m_useExplicitDefaultValue;
			m_prompt = source.m_prompt;
			m_dynamicPrompt = source.m_dynamicPrompt;
			if (source.m_values != null)
			{
				int num = source.m_values.Length;
				m_values = new object[num];
				for (int i = 0; i < num; i++)
				{
					m_values[i] = source.m_values[i];
				}
			}
			if (source.m_labels != null)
			{
				int num2 = source.m_labels.Length;
				m_labels = new string[num2];
				for (int j = 0; j < num2; j++)
				{
					m_labels[j] = source.m_labels[j];
				}
			}
			if (source.m_dependencyList != null)
			{
				int count = source.m_dependencyList.Count;
				m_dependencyList = new ParameterInfoCollection(count);
				for (int k = 0; k < count; k++)
				{
					m_dependencyList.Add(source.m_dependencyList[k]);
				}
			}
			if (source.m_validValues != null)
			{
				int count2 = source.m_validValues.Count;
				m_validValues = new ValidValueList(count2);
				for (int l = 0; l < count2; l++)
				{
					m_validValues.Add(source.m_validValues[l]);
				}
			}
		}

		internal ParameterInfo(ParameterBase source)
			: base(source)
		{
			m_prompt = source.Prompt;
		}

		internal ParameterInfo(DataSetParameterValue source, bool usedInQuery)
			: base(source, usedInQuery)
		{
		}

		internal ParameterInfo(Microsoft.ReportingServices.ReportIntermediateFormat.ParameterValue source)
			: base(source)
		{
			m_isUserSupplied = true;
		}

		internal new static Microsoft.ReportingServices.ReportProcessing.Persistence.Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new Microsoft.ReportingServices.ReportProcessing.Persistence.MemberInfo(Microsoft.ReportingServices.ReportProcessing.Persistence.MemberName.IsUserSupplied, Microsoft.ReportingServices.ReportProcessing.Persistence.Token.Boolean));
			memberInfoList.Add(new Microsoft.ReportingServices.ReportProcessing.Persistence.MemberInfo(Microsoft.ReportingServices.ReportProcessing.Persistence.MemberName.Value, Microsoft.ReportingServices.ReportProcessing.Persistence.Token.Array, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.Variant));
			memberInfoList.Add(new Microsoft.ReportingServices.ReportProcessing.Persistence.MemberInfo(Microsoft.ReportingServices.ReportProcessing.Persistence.MemberName.DynamicValidValues, Microsoft.ReportingServices.ReportProcessing.Persistence.Token.Boolean));
			memberInfoList.Add(new Microsoft.ReportingServices.ReportProcessing.Persistence.MemberInfo(Microsoft.ReportingServices.ReportProcessing.Persistence.MemberName.DynamicDefaultValue, Microsoft.ReportingServices.ReportProcessing.Persistence.Token.Boolean));
			memberInfoList.Add(new Microsoft.ReportingServices.ReportProcessing.Persistence.MemberInfo(Microsoft.ReportingServices.ReportProcessing.Persistence.MemberName.DependencyList, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ParameterInfoCollection));
			memberInfoList.Add(new Microsoft.ReportingServices.ReportProcessing.Persistence.MemberInfo(Microsoft.ReportingServices.ReportProcessing.Persistence.MemberName.ValidValues, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ValidValueList));
			memberInfoList.Add(new Microsoft.ReportingServices.ReportProcessing.Persistence.MemberInfo(Microsoft.ReportingServices.ReportProcessing.Persistence.MemberName.Label, Microsoft.ReportingServices.ReportProcessing.Persistence.Token.Array, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.String));
			return new Microsoft.ReportingServices.ReportProcessing.Persistence.Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ParameterBase, memberInfoList);
		}

		[SkipMemberStaticValidation(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.DependencyList)]
		internal new static Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.Declaration GetNewDeclaration()
		{
			List<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo> list = new List<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo>();
			list.Add(new Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.IsUserSupplied, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.Token.Boolean));
			list.Add(new Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.Value, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveArray, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.Token.Object));
			list.Add(new Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.DynamicValidValues, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.Token.Boolean));
			list.Add(new Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.DynamicDefaultValue, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.Token.Boolean));
			list.Add(new ReadOnlyMemberInfo(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.DependencyList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterInfo));
			list.Add(new Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.ValidValues, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ValidValue));
			list.Add(new Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.Label, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveArray, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.Token.String));
			list.Add(new Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.Prompt, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.Token.String));
			list.Add(new Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.DynamicPrompt, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.Token.Boolean));
			list.Add(new Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.DependencyIndexList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveTypedArray, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.Token.Int32));
			return new Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterInfo, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterBase, list);
		}

		void IPersistable.Serialize(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.IntermediateFormatWriter writer)
		{
			Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.Prompt:
					writer.Write(m_prompt);
					break;
				case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.DynamicPrompt:
					writer.Write(m_dynamicPrompt);
					break;
				case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.IsUserSupplied:
					writer.Write(m_isUserSupplied);
					break;
				case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.Value:
					writer.Write(m_values);
					break;
				case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.DynamicValidValues:
					writer.Write(m_dynamicValidValues);
					break;
				case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.DynamicDefaultValue:
					writer.Write(m_dynamicDefaultValue);
					break;
				case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.ValidValues:
					writer.Write(m_validValues);
					break;
				case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.Label:
					writer.Write(m_labels);
					break;
				case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.DependencyIndexList:
					m_dependencyIndexList = null;
					if (m_dependencyList != null)
					{
						m_dependencyIndexList = new int[m_dependencyList.Count];
						for (int i = 0; i < m_dependencyList.Count; i++)
						{
							m_dependencyIndexList[i] = m_dependencyList[i].IndexInCollection;
						}
					}
					writer.Write(m_dependencyIndexList);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		void IPersistable.Deserialize(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.IntermediateFormatReader reader)
		{
			Deserialize(reader);
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.Prompt:
					m_prompt = reader.ReadString();
					break;
				case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.DynamicPrompt:
					m_dynamicPrompt = reader.ReadBoolean();
					break;
				case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.IsUserSupplied:
					m_isUserSupplied = reader.ReadBoolean();
					break;
				case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.Value:
					m_values = reader.ReadVariantArray();
					break;
				case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.DynamicValidValues:
					m_dynamicValidValues = reader.ReadBoolean();
					break;
				case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.DynamicDefaultValue:
					m_dynamicDefaultValue = reader.ReadBoolean();
					break;
				case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.DependencyList:
					m_dependencyList = reader.ReadListOfRIFObjects<ParameterInfoCollection>();
					break;
				case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.ValidValues:
					m_validValues = reader.ReadListOfRIFObjects<ValidValueList>();
					break;
				case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.Label:
					m_labels = reader.ReadStringArray();
					break;
				case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.DependencyIndexList:
					m_dependencyIndexList = reader.ReadInt32Array();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		void IPersistable.ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false);
		}

		Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType IPersistable.GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterInfo;
		}

		internal void ResolveDependencies(ParameterInfoCollection containingCollection)
		{
			if (m_dependencyIndexList != null)
			{
				m_dependencyList = new ParameterInfoCollection(m_dependencyIndexList.Length);
				for (int i = 0; i < m_dependencyIndexList.Length; i++)
				{
					m_dependencyList.Add(containingCollection[m_dependencyIndexList[i]]);
				}
			}
			m_dependencyIndexList = null;
		}

		public void SetValuesFromQueryParameter(object value)
		{
			m_values = (value as object[]);
			if (m_values == null)
			{
				m_values = new object[1];
				m_values[0] = value;
			}
		}

		public bool AllDependenciesSpecified()
		{
			return CalculateDependencyStatus() == ReportParameterDependencyState.AllDependenciesSpecified;
		}

		internal ReportParameterDependencyState CalculateDependencyStatus()
		{
			ReportParameterDependencyState result = ReportParameterDependencyState.AllDependenciesSpecified;
			if (DependencyList != null)
			{
				for (int i = 0; i < DependencyList.Count; i++)
				{
					ParameterInfo parameterInfo = DependencyList[i];
					if (parameterInfo.MissingUpstreamDataSourcePrompt)
					{
						result = ReportParameterDependencyState.MissingUpstreamDataSourcePrompt;
						break;
					}
					if (parameterInfo.State != 0)
					{
						result = ReportParameterDependencyState.HasOutstandingDependencies;
					}
				}
			}
			return result;
		}

		public bool ValueIsValid()
		{
			if (Values == null || Values.Length == 0)
			{
				return false;
			}
			for (int i = 0; i < Values.Length; i++)
			{
				object obj = Values[i];
				if (!base.Nullable && obj == null)
				{
					if (Global.Tracer.TraceVerbose)
					{
						Global.Tracer.Trace(TraceLevel.Verbose, "Value provided for parameter '{0}' is null and parameter is not nullable.", base.Name.MarkAsPrivate());
					}
					return false;
				}
				if (base.DataType == DataType.String && !base.AllowBlank && obj != null && ((string)obj).Length == 0)
				{
					if (Global.Tracer.TraceVerbose)
					{
						Global.Tracer.Trace(TraceLevel.Verbose, "Value provided for string parameter '{0}' is either null or blank and parameter does not allow blanks.", base.Name.MarkAsPrivate());
					}
					return false;
				}
				if (ValidValues == null)
				{
					continue;
				}
				bool flag = false;
				for (int j = 0; j < ValidValues.Count; j++)
				{
					if (ParameterBase.ParameterValuesEqual(obj, ValidValues[j].Value))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					if (Global.Tracer.TraceVerbose)
					{
						Global.Tracer.Trace(TraceLevel.Verbose, "The provided value '{0}' for parameter '{1}' is not a valid value.", obj.ToString().MarkAsPrivate(), base.Name.MarkAsPrivate());
					}
					return false;
				}
			}
			return true;
		}

		internal void StoreLabels()
		{
			EnsureLabelsAreGenerated();
			if (Values == null)
			{
				return;
			}
			m_labels = new string[Values.Length];
			for (int i = 0; i < Values.Length; i++)
			{
				string text = null;
				object obj = Values[i];
				bool flag = false;
				if (ValidValues != null)
				{
					for (int j = 0; j < ValidValues.Count; j++)
					{
						if (ParameterBase.ParameterValuesEqual(obj, ValidValues[j].Value))
						{
							flag = true;
							text = ValidValues[j].Label;
							break;
						}
					}
				}
				if (!flag && obj != null)
				{
					text = CastValueToLabelString(obj, Thread.CurrentThread.CurrentCulture);
				}
				m_labels[i] = text;
			}
		}

		internal void EnsureLabelsAreGenerated()
		{
			if (ValidValues != null)
			{
				for (int i = 0; i < ValidValues.Count; i++)
				{
					ValidValues[i].EnsureLabelIsGenerated();
				}
			}
		}

		internal void AddValidValue(object paramValue, string paramLabel)
		{
			if (paramLabel == null)
			{
				paramLabel = CastValueToLabelString(paramValue, Thread.CurrentThread.CurrentCulture);
			}
			AddValidValueExplicit(paramValue, paramLabel);
		}

		internal void AddValidValue(string paramValue, string paramLabel, ErrorContext errorContext, CultureInfo language)
		{
			if (!ParameterBase.CastFromString(paramValue, out object newValue, base.DataType, language))
			{
				if (errorContext == null)
				{
					throw new ReportParameterTypeMismatchException(base.Name);
				}
				errorContext.Register(ProcessingErrorCode.rsParameterPropertyTypeMismatch, Severity.Error, ObjectType.ReportParameter, base.Name, "ValidValue");
			}
			else
			{
				AddValidValueExplicit(newValue, paramLabel);
			}
		}

		internal void AddValidValueExplicit(object paramValue, string paramLabel)
		{
			if (ValidValues == null)
			{
				ValidValues = new ValidValueList();
			}
			ValidValues.Add(new ValidValue(paramValue, paramLabel));
		}

		internal void Parse(string name, List<string> defaultValues, string type, string nullable, string prompt, bool promptIsExpr, string promptUser, string allowBlank, string multiValue, ValidValueList validValues, string usedInQuery, bool hidden, ErrorContext errorContext, CultureInfo language)
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
			DynamicPrompt = promptIsExpr;
			if (validValues == null)
			{
				return;
			}
			int count = validValues.Count;
			for (int i = 0; i < count; i++)
			{
				if (!ParameterBase.CastFromString(validValues[i].StringValue, out object newValue, base.DataType, language))
				{
					if (errorContext == null)
					{
						throw new ReportParameterTypeMismatchException(name);
					}
					errorContext.Register(ProcessingErrorCode.rsParameterPropertyTypeMismatch, Severity.Error, ObjectType.ReportParameter, name, "ValidValue");
				}
				else
				{
					validValues[i].Value = newValue;
					ValidateValue(newValue, errorContext, base.ParameterObjectType, "ValidValue");
				}
			}
			m_validValues = validValues;
		}

		internal void Parse(string name, string type, string nullable, string allowBlank, string multiValue, string usedInQuery, string state, string dynamicPrompt, string prompt, string promptUser, ParameterInfoCollection dependencies, string dynamicValidValues, ValidValueList validValues, string dynamicDefaultValue, List<string> defaultValues, List<string> values, string[] labels, CultureInfo language)
		{
			bool hidden = prompt != null && prompt.Length == 0;
			bool promptIsExpr = false;
			if (dynamicPrompt != null)
			{
				promptIsExpr = bool.Parse(dynamicPrompt);
			}
			Parse(name, defaultValues, type, nullable, prompt, promptIsExpr, promptUser, allowBlank, multiValue, validValues, usedInQuery, hidden, null, language);
			if (state != null)
			{
				State = (ReportParameterState)Enum.Parse(typeof(ReportParameterState), state);
			}
			DependencyList = dependencies;
			if (dynamicValidValues != null)
			{
				DynamicValidValues = bool.Parse(dynamicValidValues);
			}
			if (dynamicDefaultValue != null)
			{
				DynamicDefaultValue = bool.Parse(dynamicDefaultValue);
			}
			if (values != null)
			{
				Values = new object[values.Count];
				for (int i = 0; i < values.Count; i++)
				{
					if (!ParameterBase.CastFromString(values[i], out Values[i], base.DataType, language))
					{
						throw new InternalCatalogException("Can not cast report parameter to correct type when reading from XML");
					}
				}
			}
			Labels = labels;
		}

		internal void WriteToXml(XmlTextWriter xml, bool writeTransientState)
		{
			xml.WriteStartElement("Parameter");
			xml.WriteElementString("Name", base.Name);
			xml.WriteElementString("Type", base.DataType.ToString());
			xml.WriteElementString("Nullable", base.Nullable.ToString(CultureInfo.InvariantCulture));
			xml.WriteElementString("AllowBlank", base.AllowBlank.ToString(CultureInfo.InvariantCulture));
			xml.WriteElementString("MultiValue", base.MultiValue.ToString(CultureInfo.InvariantCulture));
			xml.WriteElementString("UsedInQuery", base.UsedInQuery.ToString(CultureInfo.InvariantCulture));
			xml.WriteElementString("State", State.ToString());
			if (Prompt != null)
			{
				xml.WriteElementString("Prompt", Prompt);
			}
			if (Prompt != null)
			{
				xml.WriteElementString("DynamicPrompt", DynamicPrompt.ToString(CultureInfo.InvariantCulture));
			}
			xml.WriteElementString("PromptUser", base.PromptUser.ToString(CultureInfo.InvariantCulture));
			if (DependencyList != null)
			{
				xml.WriteStartElement("Dependencies");
				for (int i = 0; i < DependencyList.Count; i++)
				{
					if (DependencyList[i] != null)
					{
						xml.WriteElementString("Dependency", DependencyList[i].Name);
					}
				}
				xml.WriteEndElement();
			}
			if (DynamicValidValues)
			{
				xml.WriteElementString("DynamicValidValues", DynamicValidValues.ToString(CultureInfo.InvariantCulture));
			}
			if (ValidValues != null)
			{
				xml.WriteStartElement("ValidValues");
				for (int j = 0; j < ValidValues.Count; j++)
				{
					xml.WriteStartElement("ValidValue");
					if (ValidValues[j] != null)
					{
						if (ValidValues[j].Value != null)
						{
							WriteValueToXml(xml, base.DataType, ValidValues[j].Value);
						}
						if (ValidValues[j].LabelRaw != null)
						{
							xml.WriteElementString("Label", ValidValues[j].LabelRaw);
						}
					}
					xml.WriteEndElement();
				}
				xml.WriteEndElement();
			}
			if (DynamicDefaultValue)
			{
				xml.WriteElementString("DynamicDefaultValue", DynamicDefaultValue.ToString(CultureInfo.InvariantCulture));
			}
			if (base.DefaultValues != null)
			{
				xml.WriteStartElement("DefaultValues");
				for (int k = 0; k < base.DefaultValues.Length; k++)
				{
					WriteValueToXml(xml, base.DataType, base.DefaultValues[k]);
				}
				xml.WriteEndElement();
			}
			if (Values != null)
			{
				xml.WriteStartElement("Values");
				for (int l = 0; l < Values.Length; l++)
				{
					WriteValueToXml(xml, base.DataType, Values[l]);
				}
				xml.WriteEndElement();
			}
			if (writeTransientState)
			{
				xml.WriteElementString("IsUserSupplied", IsUserSupplied.ToString(CultureInfo.InvariantCulture));
				xml.WriteElementString("ValuesChanged", ValuesChanged.ToString(CultureInfo.InvariantCulture));
				xml.WriteElementString("UseExplicitDefaultValue", UseExplicitDefaultValue.ToString(CultureInfo.InvariantCulture));
			}
			xml.WriteEndElement();
		}

		internal static string EncodeObjectAsBase64String(object originalValue, bool convertValueToString)
		{
			if (originalValue == null)
			{
				return null;
			}
			try
			{
				if (convertValueToString)
				{
					if (originalValue is bool)
					{
						originalValue = ((bool)originalValue).ToString(CultureInfo.InvariantCulture);
					}
					else if (originalValue is sbyte)
					{
						originalValue = ((sbyte)originalValue).ToString(CultureInfo.InvariantCulture);
					}
					else if (originalValue is short)
					{
						originalValue = ((short)originalValue).ToString(CultureInfo.InvariantCulture);
					}
					else if (originalValue is int)
					{
						originalValue = ((int)originalValue).ToString(CultureInfo.InvariantCulture);
					}
					else if (originalValue is long)
					{
						originalValue = ((long)originalValue).ToString(CultureInfo.InvariantCulture);
					}
					else if (originalValue is byte)
					{
						originalValue = ((byte)originalValue).ToString(CultureInfo.InvariantCulture);
					}
					else if (originalValue is ushort)
					{
						originalValue = ((ushort)originalValue).ToString(CultureInfo.InvariantCulture);
					}
					else if (originalValue is uint)
					{
						originalValue = ((uint)originalValue).ToString(CultureInfo.InvariantCulture);
					}
					else if (originalValue is ulong)
					{
						originalValue = ((ulong)originalValue).ToString(CultureInfo.InvariantCulture);
					}
					else if (originalValue is float)
					{
						originalValue = ((float)originalValue).ToString("r", CultureInfo.InvariantCulture);
					}
					else if (originalValue is double)
					{
						originalValue = ((double)originalValue).ToString("r", CultureInfo.InvariantCulture);
					}
					else if (originalValue is decimal)
					{
						originalValue = ((decimal)originalValue).ToString("f", CultureInfo.InvariantCulture);
					}
					else if (originalValue is DateTime)
					{
						originalValue = ((DateTime)originalValue).ToString("s", CultureInfo.InvariantCulture);
					}
				}
				using (MemoryStream memoryStream = new MemoryStream())
				{
					new Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.IntermediateFormatWriter(memoryStream, 0).Write(new RIFVariantContainer(originalValue));
					memoryStream.Flush();
					byte[] inArray = memoryStream.ToArray();
					memoryStream.Close();
					return Convert.ToBase64String(inArray);
				}
			}
			catch (Exception innerException)
			{
				throw new InternalCatalogException(innerException, "Parameter value encoding failed for type='" + originalValue.GetType().ToString() + "', value='" + originalValue.ToString().MarkAsPrivate() + "'");
			}
		}

		private void WriteValueToXml(XmlTextWriter xml, DataType parameterType, object val)
		{
			WriteValueToXml(xml, parameterType, val, convertValueToString: false);
		}

		private void WriteValueToXml(XmlTextWriter xml, DataType parameterType, object val, bool convertValueToString)
		{
			if (parameterType == DataType.Object)
			{
				WriteValueToXml(xml, EncodeObjectAsBase64String(val, convertValueToString));
			}
			else
			{
				WriteValueToXml(xml, val);
			}
		}

		private void WriteValueToXml(XmlTextWriter xml, object val)
		{
			xml.WriteStartElement("Value");
			if (val == null)
			{
				xml.WriteAttributeString("nil", bool.TrueString);
			}
			else
			{
				string text = val as string;
				if (text == null)
				{
					xml.WriteString(CastToString(val, CultureInfo.InvariantCulture));
				}
				else
				{
					xml.WriteString(text);
				}
			}
			xml.WriteEndElement();
		}

		internal void WriteNameValueToXml(XmlTextWriter xml, bool convertToString)
		{
			xml.WriteStartElement("Parameter");
			xml.WriteElementString("Name", base.Name);
			if (Values != null)
			{
				xml.WriteStartElement("Values");
				for (int i = 0; i < Values.Length; i++)
				{
					WriteValueToXml(xml, base.DataType, Values[i], convertToString);
				}
				xml.WriteEndElement();
			}
			xml.WriteEndElement();
		}

		internal static ParameterInfo Cast(ParameterInfo oldValue, ParameterInfo newType, CultureInfo language)
		{
			bool metaChanges = false;
			return Cast(oldValue, newType, language, ref metaChanges);
		}

		internal static ParameterInfo Cast(ParameterInfo oldValue, ParameterInfo newType, CultureInfo language, ref bool metaChanges)
		{
			object[] array = null;
			object[] array2 = null;
			if (oldValue.Values != null)
			{
				array = new object[oldValue.Values.Length];
				for (int i = 0; i < oldValue.Values.Length; i++)
				{
					if (!ParameterBase.Cast(oldValue.Values[i], oldValue.DataType, out array[i], newType.DataType, language))
					{
						return null;
					}
				}
			}
			if (oldValue.DefaultValues != null)
			{
				array2 = new object[oldValue.DefaultValues.Length];
				for (int j = 0; j < oldValue.DefaultValues.Length; j++)
				{
					if (!ParameterBase.Cast(oldValue.DefaultValues[j], oldValue.DataType, out array2[j], newType.DataType, language))
					{
						return null;
					}
				}
			}
			if (oldValue.DataType != newType.DataType)
			{
				metaChanges = true;
			}
			ParameterInfo parameterInfo = new ParameterInfo(newType);
			parameterInfo.Values = array;
			parameterInfo.DefaultValues = array2;
			parameterInfo.StoreLabels();
			return parameterInfo;
		}

		public static string CastToString(object val, DataType type, CultureInfo language)
		{
			if (!ParameterBase.Cast(val, type, out object _, DataType.String, language))
			{
				throw new InternalCatalogException("Can not cast value of report parameter to string.");
			}
			return CastValueToLabelString(val, language);
		}

		public string CastToString(object val, CultureInfo language)
		{
			return CastToString(val, base.DataType, language);
		}

		internal static string CastValueToLabelString(object val, CultureInfo language)
		{
			if (val == null)
			{
				return null;
			}
			return Convert.ToString(val, language);
		}
	}
}
