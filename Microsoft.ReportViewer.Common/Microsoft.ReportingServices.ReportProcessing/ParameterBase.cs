using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal abstract class ParameterBase : IPersistable
	{
		internal enum UsedInQueryType
		{
			False,
			True,
			Auto
		}

		internal const string NameXmlElement = "Name";

		internal const string TypeXmlElement = "Type";

		internal const string NullableXmlElement = "Nullable";

		internal const string AllowBlankXmlElement = "AllowBlank";

		internal const string MultiValueXmlElement = "MultiValue";

		internal const string PromptXmlElement = "Prompt";

		internal const string PromptUserXmlElement = "PromptUser";

		internal const string ValueXmlElement = "Value";

		internal const string UsedInQueryXmlElement = "UsedInQuery";

		internal const string DefaultValuesXmlElement = "DefaultValues";

		internal const string ValidValuesXmlElement = "ValidValues";

		internal const string StateXmlElement = "State";

		private string m_name;

		private DataType m_dataType = DataType.String;

		private bool m_nullable;

		private bool m_promptUser;

		private bool m_usedInQuery;

		private bool m_allowBlank;

		private bool m_multiValue;

		private object[] m_defaultValues;

		[NonSerialized]
		private UsedInQueryType m_usedInQueryAsDefined = UsedInQueryType.Auto;

		[NonSerialized]
		private Hashtable m_dependencies;

		[NonSerialized]
		private static readonly Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.Declaration m_Declaration = GetNewDeclaration();

		internal ObjectType ParameterObjectType
		{
			get
			{
				if (m_dataType == DataType.Object)
				{
					return ObjectType.QueryParameter;
				}
				return ObjectType.ReportParameter;
			}
		}

		public string Name
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

		public DataType DataType
		{
			get
			{
				return m_dataType;
			}
			set
			{
				m_dataType = value;
			}
		}

		public bool Nullable
		{
			get
			{
				return m_nullable;
			}
			set
			{
				m_nullable = value;
			}
		}

		public abstract string Prompt
		{
			get;
			set;
		}

		public bool PromptUser
		{
			get
			{
				return m_promptUser;
			}
			set
			{
				m_promptUser = value;
			}
		}

		public bool AllowBlank
		{
			get
			{
				return m_allowBlank;
			}
			set
			{
				m_allowBlank = value;
			}
		}

		public bool MultiValue
		{
			get
			{
				return m_multiValue;
			}
			set
			{
				m_multiValue = value;
			}
		}

		public object[] DefaultValues
		{
			get
			{
				return m_defaultValues;
			}
			set
			{
				m_defaultValues = value;
			}
		}

		internal Hashtable Dependencies
		{
			get
			{
				return m_dependencies;
			}
			set
			{
				m_dependencies = value;
			}
		}

		public bool UsedInQuery
		{
			get
			{
				return m_usedInQuery;
			}
			set
			{
				m_usedInQuery = value;
			}
		}

		internal UsedInQueryType UsedInQueryAsDefined => m_usedInQueryAsDefined;

		public ParameterBase()
		{
		}

		internal ParameterBase(Microsoft.ReportingServices.ReportIntermediateFormat.ParameterValue source)
		{
			m_dataType = DataType.Object;
			m_name = source.Name;
			m_usedInQuery = false;
		}

		internal ParameterBase(DataSetParameterValue source, bool usedInQuery)
		{
			m_dataType = DataType.Object;
			m_name = source.UniqueName;
			m_nullable = source.Nullable;
			m_multiValue = source.MultiValue;
			m_allowBlank = false;
			m_promptUser = !source.ReadOnly;
			m_usedInQuery = usedInQuery;
			if (source.Value != null && !source.Value.IsExpression)
			{
				m_defaultValues = new object[1];
				m_defaultValues[0] = source.Value.Value;
			}
		}

		internal ParameterBase(ParameterBase source)
		{
			m_name = source.m_name;
			m_dataType = source.m_dataType;
			m_nullable = source.m_nullable;
			m_promptUser = source.m_promptUser;
			m_allowBlank = source.m_allowBlank;
			m_multiValue = source.m_multiValue;
			if (source.m_defaultValues != null)
			{
				int num = source.m_defaultValues.Length;
				m_defaultValues = new object[num];
				for (int i = 0; i < num; i++)
				{
					m_defaultValues[i] = source.m_defaultValues[i];
				}
			}
			m_usedInQuery = source.m_usedInQuery;
		}

		internal static Microsoft.ReportingServices.ReportProcessing.Persistence.Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new Microsoft.ReportingServices.ReportProcessing.Persistence.MemberInfo(Microsoft.ReportingServices.ReportProcessing.Persistence.MemberName.Name, Microsoft.ReportingServices.ReportProcessing.Persistence.Token.String));
			memberInfoList.Add(new Microsoft.ReportingServices.ReportProcessing.Persistence.MemberInfo(Microsoft.ReportingServices.ReportProcessing.Persistence.MemberName.DataType, Microsoft.ReportingServices.ReportProcessing.Persistence.Token.Enum));
			memberInfoList.Add(new Microsoft.ReportingServices.ReportProcessing.Persistence.MemberInfo(Microsoft.ReportingServices.ReportProcessing.Persistence.MemberName.Nullable, Microsoft.ReportingServices.ReportProcessing.Persistence.Token.Boolean));
			memberInfoList.Add(new Microsoft.ReportingServices.ReportProcessing.Persistence.MemberInfo(Microsoft.ReportingServices.ReportProcessing.Persistence.MemberName.Prompt, Microsoft.ReportingServices.ReportProcessing.Persistence.Token.String));
			memberInfoList.Add(new Microsoft.ReportingServices.ReportProcessing.Persistence.MemberInfo(Microsoft.ReportingServices.ReportProcessing.Persistence.MemberName.UsedInQuery, Microsoft.ReportingServices.ReportProcessing.Persistence.Token.Boolean));
			memberInfoList.Add(new Microsoft.ReportingServices.ReportProcessing.Persistence.MemberInfo(Microsoft.ReportingServices.ReportProcessing.Persistence.MemberName.AllowBlank, Microsoft.ReportingServices.ReportProcessing.Persistence.Token.Boolean));
			memberInfoList.Add(new Microsoft.ReportingServices.ReportProcessing.Persistence.MemberInfo(Microsoft.ReportingServices.ReportProcessing.Persistence.MemberName.MultiValue, Microsoft.ReportingServices.ReportProcessing.Persistence.Token.Boolean));
			memberInfoList.Add(new Microsoft.ReportingServices.ReportProcessing.Persistence.MemberInfo(Microsoft.ReportingServices.ReportProcessing.Persistence.MemberName.DefaultValue, Microsoft.ReportingServices.ReportProcessing.Persistence.Token.Array, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.Variant));
			memberInfoList.Add(new Microsoft.ReportingServices.ReportProcessing.Persistence.MemberInfo(Microsoft.ReportingServices.ReportProcessing.Persistence.MemberName.PromptUser, Microsoft.ReportingServices.ReportProcessing.Persistence.Token.Boolean));
			return new Microsoft.ReportingServices.ReportProcessing.Persistence.Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}

		internal static Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.Declaration GetNewDeclaration()
		{
			List<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo> list = new List<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo>();
			list.Add(new Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.Name, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.Token.String));
			list.Add(new Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.DataType, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.Token.Enum));
			list.Add(new Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.Nullable, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.Token.Boolean));
			list.Add(new Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.UsedInQuery, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.Token.Boolean));
			list.Add(new Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.AllowBlank, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.Token.Boolean));
			list.Add(new Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.MultiValue, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.Token.Boolean));
			list.Add(new Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.DefaultValue, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveArray, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.Token.Object));
			list.Add(new Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.PromptUser, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.Token.Boolean));
			return new Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterBase, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		internal void Serialize(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.Name:
					writer.Write(m_name);
					break;
				case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.DataType:
					writer.WriteEnum((int)m_dataType);
					break;
				case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.Nullable:
					writer.Write(m_nullable);
					break;
				case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.UsedInQuery:
					writer.Write(m_usedInQuery);
					break;
				case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.AllowBlank:
					writer.Write(m_allowBlank);
					break;
				case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.MultiValue:
					writer.Write(m_multiValue);
					break;
				case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.DefaultValue:
					writer.Write(m_defaultValues);
					break;
				case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.PromptUser:
					writer.Write(m_promptUser);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		internal void Deserialize(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.Name:
					m_name = reader.ReadString();
					break;
				case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.DataType:
					m_dataType = (DataType)reader.ReadEnum();
					break;
				case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.Nullable:
					m_nullable = reader.ReadBoolean();
					break;
				case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.UsedInQuery:
					m_usedInQuery = reader.ReadBoolean();
					break;
				case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.AllowBlank:
					m_allowBlank = reader.ReadBoolean();
					break;
				case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.MultiValue:
					m_multiValue = reader.ReadBoolean();
					break;
				case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.DefaultValue:
					m_defaultValues = reader.ReadVariantArray();
					break;
				case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.PromptUser:
					m_promptUser = reader.ReadBoolean();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		void IPersistable.Serialize(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.IntermediateFormatWriter writer)
		{
			Serialize(writer);
		}

		void IPersistable.Deserialize(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.IntermediateFormatReader reader)
		{
			Deserialize(reader);
		}

		void IPersistable.ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false);
		}

		Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType IPersistable.GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterBase;
		}

		internal static bool ValidateValueForNull(object newValue, bool nullable, ErrorContext errorContext, ObjectType parameterType, string parameterName, string parameterValueProperty)
		{
			bool result = true;
			bool flag = errorContext is PublishingErrorContext;
			if (newValue == null && !nullable)
			{
				result = false;
				errorContext?.Register(flag ? ProcessingErrorCode.rsParameterValueNullOrBlank : ProcessingErrorCode.rsParameterValueDefinitionMismatch, (!flag) ? Severity.Warning : Severity.Error, parameterType, parameterName, "Nullable", parameterValueProperty);
			}
			return result;
		}

		internal bool ValidateValueForBlank(object newValue, ErrorContext errorContext, string parameterValueProperty)
		{
			bool result = true;
			bool flag = errorContext is PublishingErrorContext;
			if (DataType == DataType.String && !AllowBlank && (string)newValue == string.Empty)
			{
				result = false;
				errorContext?.Register(flag ? ProcessingErrorCode.rsParameterValueNullOrBlank : ProcessingErrorCode.rsParameterValueDefinitionMismatch, (!flag) ? Severity.Warning : Severity.Error, ObjectType.ReportParameter, m_name, "AllowBlank", parameterValueProperty);
			}
			return result;
		}

		internal void ValidateValue(object newValue, ErrorContext errorContext, ObjectType parameterType, string parameterValueProperty)
		{
			ValidateValueForNull(newValue, Nullable, errorContext, parameterType, Name, parameterValueProperty);
			ValidateValueForBlank(newValue, errorContext, parameterValueProperty);
		}

		internal virtual void Parse(string name, List<string> defaultValues, string type, string nullable, object prompt, string promptUser, string allowBlank, string multiValue, string usedInQuery, bool hidden, ErrorContext errorContext, CultureInfo language)
		{
			if (name == null || name.Length == 0)
			{
				throw new MissingElementException("Name");
			}
			m_name = name;
			if (type == null || type.Length == 0)
			{
				m_dataType = DataType.String;
			}
			else
			{
				try
				{
					m_dataType = (DataType)Enum.Parse(typeof(DataType), type, ignoreCase: true);
				}
				catch (ArgumentException)
				{
					if (errorContext == null)
					{
						throw new ElementTypeMismatchException("Type");
					}
					errorContext.Register(ProcessingErrorCode.rsParameterPropertyTypeMismatch, Severity.Error, ObjectType.Parameter, name, "DataType");
				}
			}
			if (nullable == null || nullable.Length == 0)
			{
				m_nullable = false;
			}
			else
			{
				try
				{
					m_nullable = bool.Parse(nullable);
				}
				catch (FormatException)
				{
					if (errorContext == null)
					{
						throw new ElementTypeMismatchException("Nullable");
					}
					errorContext.Register(ProcessingErrorCode.rsParameterPropertyTypeMismatch, Severity.Error, ParameterObjectType, name, "Nullable");
				}
			}
			if (allowBlank == null || allowBlank.Length == 0)
			{
				m_allowBlank = false;
			}
			else
			{
				try
				{
					m_allowBlank = bool.Parse(allowBlank);
				}
				catch (FormatException)
				{
					if (errorContext == null)
					{
						throw new ElementTypeMismatchException("AllowBlank");
					}
					errorContext.Register(ProcessingErrorCode.rsParameterPropertyTypeMismatch, Severity.Error, ParameterObjectType, name, "AllowBlank");
				}
			}
			if (multiValue == null || multiValue.Length == 0 || m_dataType == DataType.Boolean)
			{
				m_multiValue = false;
			}
			else
			{
				try
				{
					m_multiValue = bool.Parse(multiValue);
				}
				catch (FormatException)
				{
					if (errorContext == null)
					{
						throw new ElementTypeMismatchException("MultiValue");
					}
					errorContext.Register(ProcessingErrorCode.rsParameterPropertyTypeMismatch, Severity.Error, ParameterObjectType, name, "MultiValue");
				}
			}
			if (promptUser == null || promptUser == string.Empty)
			{
				if (prompt == null)
				{
					m_promptUser = false;
				}
				else
				{
					m_promptUser = true;
				}
			}
			else
			{
				try
				{
					m_promptUser = bool.Parse(promptUser);
				}
				catch (FormatException)
				{
					throw new ElementTypeMismatchException("PromptUser");
				}
			}
			if (defaultValues == null)
			{
				m_defaultValues = null;
			}
			else
			{
				int count = defaultValues.Count;
				m_defaultValues = new object[count];
				for (int i = 0; i < count; i++)
				{
					if (!CastFromString(defaultValues[i], out object newValue, m_dataType, language))
					{
						if (errorContext == null)
						{
							throw new ReportParameterTypeMismatchException(name);
						}
						errorContext.Register(ProcessingErrorCode.rsParameterPropertyTypeMismatch, Severity.Error, ParameterObjectType, name, "DefaultValue");
					}
					else
					{
						ValidateValue(newValue, errorContext, ParameterObjectType, "DefaultValue");
					}
					m_defaultValues[i] = newValue;
				}
			}
			m_usedInQuery = true;
			if (usedInQuery == null || usedInQuery.Length == 0)
			{
				m_usedInQueryAsDefined = UsedInQueryType.Auto;
			}
			else
			{
				try
				{
					m_usedInQueryAsDefined = (UsedInQueryType)Enum.Parse(typeof(UsedInQueryType), usedInQuery, ignoreCase: true);
				}
				catch (ArgumentException)
				{
					if (errorContext == null)
					{
						throw new ElementTypeMismatchException("UsedInQuery");
					}
					errorContext.Register(ProcessingErrorCode.rsParameterPropertyTypeMismatch, Severity.Error, ParameterObjectType, name, "MultiValue");
				}
				if (m_usedInQueryAsDefined == UsedInQueryType.False)
				{
					m_usedInQuery = false;
				}
				else if (m_usedInQueryAsDefined == UsedInQueryType.True)
				{
					m_usedInQuery = true;
				}
			}
			if (usedInQuery == null || usedInQuery.Length == 0)
			{
				m_usedInQueryAsDefined = UsedInQueryType.Auto;
				return;
			}
			try
			{
				m_usedInQueryAsDefined = (UsedInQueryType)Enum.Parse(typeof(UsedInQueryType), usedInQuery, ignoreCase: true);
			}
			catch (ArgumentException)
			{
				throw new ElementTypeMismatchException("UsedInQuery");
			}
			if (m_usedInQueryAsDefined == UsedInQueryType.False)
			{
				m_usedInQuery = false;
			}
			else if (m_usedInQueryAsDefined == UsedInQueryType.True)
			{
				m_usedInQuery = true;
			}
		}

		internal static bool Cast(object oldValue, DataType oldType, out object newValue, DataType newType, CultureInfo language)
		{
			if (oldValue == null)
			{
				newValue = null;
				return true;
			}
			switch (oldType)
			{
			case DataType.Object:
				newValue = oldValue;
				return true;
			case DataType.String:
				return CastFromString((string)oldValue, out newValue, newType, language);
			case DataType.Boolean:
				return CastFromBoolean((bool)oldValue, out newValue, newType, language);
			case DataType.Float:
				return CastFromDouble((double)oldValue, out newValue, newType, language);
			case DataType.DateTime:
				if (oldValue is DateTimeOffset)
				{
					return CastFromDateTimeOffset((DateTimeOffset)oldValue, out newValue, newType, language);
				}
				return CastFromDateTime((DateTime)oldValue, out newValue, newType, language);
			case DataType.Integer:
				return CastFromInteger((int)oldValue, out newValue, newType, language);
			default:
				throw new InternalCatalogException("Parameter type is not one of the supported types in Cast");
			}
		}

		internal static bool DecodeObjectFromBase64String(string originalValue, out object newValue)
		{
			newValue = null;
			if (string.IsNullOrEmpty(originalValue))
			{
				return true;
			}
			try
			{
				using (MemoryStream str = new MemoryStream(Convert.FromBase64String(originalValue)))
				{
					ProcessingRIFObjectCreator rifObjectCreator = new ProcessingRIFObjectCreator(null, null);
					RIFVariantContainer rIFVariantContainer = (RIFVariantContainer)new Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.IntermediateFormatReader(str, rifObjectCreator).ReadRIFObject();
					newValue = rIFVariantContainer.Value;
				}
			}
			catch (Exception innerException)
			{
				throw new InternalCatalogException(innerException, "Parameter value decoding failed for base64 encoded string='" + originalValue + "'");
			}
			return true;
		}

		public static bool CastFromString(string oldString, out object newValue, DataType newType, CultureInfo language)
		{
			newValue = null;
			if (oldString == null)
			{
				return true;
			}
			switch (newType)
			{
			case DataType.Object:
				return DecodeObjectFromBase64String(oldString, out newValue);
			case DataType.String:
				newValue = oldString;
				return true;
			case DataType.Boolean:
				if (string.Compare(oldString, "true", ignoreCase: true, language) == 0 || string.Compare(oldString, "enable", ignoreCase: true, language) == 0 || string.Compare(oldString, "enabled", ignoreCase: true, language) == 0 || string.Compare(oldString, "yes", ignoreCase: true, language) == 0 || string.Compare(oldString, "on", ignoreCase: true, language) == 0 || string.Compare(oldString, "+", ignoreCase: true, language) == 0)
				{
					newValue = true;
					return true;
				}
				if (string.Compare(oldString, "false", ignoreCase: true, language) == 0 || string.Compare(oldString, "disable", ignoreCase: true, language) == 0 || string.Compare(oldString, "disabled", ignoreCase: true, language) == 0 || string.Compare(oldString, "no", ignoreCase: true, language) == 0 || string.Compare(oldString, "off", ignoreCase: true, language) == 0 || string.Compare(oldString, "-", ignoreCase: true, language) == 0)
				{
					newValue = false;
					return true;
				}
				return false;
			case DataType.Float:
				try
				{
					newValue = double.Parse(oldString, language);
					return true;
				}
				catch (Exception ex2)
				{
					if (ex2 is FormatException || ex2 is OverflowException)
					{
						return false;
					}
					throw;
				}
			case DataType.Integer:
				try
				{
					newValue = int.Parse(oldString, language);
					return true;
				}
				catch (Exception ex)
				{
					if (ex is FormatException || ex is OverflowException)
					{
						return false;
					}
					throw;
				}
			case DataType.DateTime:
			{
				if (DateTimeUtil.TryParseDateTime(oldString, language, out DateTimeOffset dateTimeOffset, out bool hasTimeOffset))
				{
					if (hasTimeOffset)
					{
						newValue = dateTimeOffset;
					}
					else
					{
						newValue = dateTimeOffset.DateTime;
					}
					return true;
				}
				return false;
			}
			default:
				throw new InternalCatalogException("Parameter type is not one of the supported types in Cast");
			}
		}

		internal static bool CastFromBoolean(bool oldBoolean, out object newValue, DataType newType, CultureInfo language)
		{
			newValue = null;
			switch (newType)
			{
			case DataType.Object:
				newValue = oldBoolean;
				return true;
			case DataType.String:
				newValue = oldBoolean.ToString(language);
				return true;
			case DataType.Boolean:
				newValue = oldBoolean;
				return true;
			case DataType.Float:
				newValue = (double)(oldBoolean ? 1 : 0);
				return true;
			case DataType.Integer:
				newValue = (oldBoolean ? 1 : 0);
				return true;
			case DataType.DateTime:
				return false;
			default:
				throw new InternalCatalogException("Parameter type is not one of the supported types in Cast");
			}
		}

		internal static bool CastFromDouble(double oldDouble, out object newValue, DataType newType, CultureInfo language)
		{
			newValue = null;
			checked
			{
				switch (newType)
				{
				case DataType.Object:
					newValue = oldDouble;
					return true;
				case DataType.String:
					newValue = oldDouble.ToString(language);
					return true;
				case DataType.Boolean:
					newValue = (oldDouble != 0.0);
					return true;
				case DataType.Float:
					newValue = oldDouble;
					return true;
				case DataType.Integer:
					try
					{
						newValue = (int)oldDouble;
					}
					catch (OverflowException)
					{
						return false;
					}
					return true;
				case DataType.DateTime:
					try
					{
						newValue = new DateTime((long)oldDouble);
					}
					catch (Exception ex)
					{
						if (ex is ArgumentOutOfRangeException || ex is OverflowException)
						{
							return false;
						}
						throw;
					}
					return true;
				default:
					throw new InternalCatalogException("Parameter type is not one of the supported types in Cast");
				}
			}
		}

		internal static bool CastFromInteger(int oldInteger, out object newValue, DataType newType, CultureInfo language)
		{
			newValue = null;
			switch (newType)
			{
			case DataType.Object:
				newValue = oldInteger;
				return true;
			case DataType.String:
				newValue = oldInteger.ToString(language);
				return true;
			case DataType.Boolean:
				newValue = (oldInteger != 0);
				return true;
			case DataType.Float:
				newValue = (double)oldInteger;
				return true;
			case DataType.Integer:
				newValue = oldInteger;
				return true;
			case DataType.DateTime:
				try
				{
					newValue = new DateTime(oldInteger);
				}
				catch (Exception ex)
				{
					if (ex is ArgumentOutOfRangeException || ex is OverflowException)
					{
						return false;
					}
					throw;
				}
				return true;
			default:
				throw new InternalCatalogException("Parameter type is not one of the supported types in Cast");
			}
		}

		internal static bool CastFromDateTime(DateTime oldDateTime, out object newValue, DataType newType, CultureInfo language)
		{
			newValue = null;
			switch (newType)
			{
			case DataType.Object:
				newValue = oldDateTime;
				return true;
			case DataType.String:
				newValue = oldDateTime.ToString(language);
				return true;
			case DataType.Boolean:
				return false;
			case DataType.Float:
				newValue = oldDateTime.Ticks;
				return true;
			case DataType.Integer:
				try
				{
					newValue = Convert.ToInt32(oldDateTime.Ticks);
					return true;
				}
				catch (OverflowException)
				{
					return false;
				}
			case DataType.DateTime:
				newValue = oldDateTime;
				return true;
			default:
				throw new InternalCatalogException("Parameter type is not one of the supported types in Cast");
			}
		}

		internal static bool CastFromDateTimeOffset(DateTimeOffset oldDateTime, out object newValue, DataType newType, CultureInfo language)
		{
			newValue = null;
			switch (newType)
			{
			case DataType.Object:
				newValue = oldDateTime;
				return true;
			case DataType.String:
				newValue = oldDateTime.ToString(language);
				return true;
			case DataType.Boolean:
				return false;
			case DataType.Float:
				return false;
			case DataType.Integer:
				return false;
			case DataType.DateTime:
				newValue = oldDateTime;
				return true;
			default:
				throw new InternalCatalogException("Parameter type is not one of the supported types in Cast");
			}
		}

		public static bool ParameterValuesEqual(object o1, object o2)
		{
			return object.Equals(o1, o2);
		}

		internal static bool IsSharedDataSetParameterObjectType(ObjectType ot)
		{
			switch (ot)
			{
			case ObjectType.QueryParameter:
				return true;
			case ObjectType.ReportParameter:
			case ObjectType.Parameter:
				return false;
			default:
				Global.Tracer.Assert(false, "Unknown ObjectType: {0}", ot);
				return ObjectType.QueryParameter == ot;
			}
		}
	}
}
