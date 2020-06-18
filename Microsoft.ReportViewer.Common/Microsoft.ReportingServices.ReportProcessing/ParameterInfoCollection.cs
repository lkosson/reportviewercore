using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using System.Xml;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ParameterInfoCollection : ArrayList, IPersistable, ISerializable
	{
		private ParametersGridLayout m_parametersLayout;

		private UserProfileState m_userProfileState;

		private bool m_validated;

		private const string SerializationXmlPropName = "Xml";

		private static Declaration m_Declaration = GetDeclaration();

		public new ParameterInfo this[int index]
		{
			get
			{
				return (ParameterInfo)base[index];
			}
			set
			{
				base[index] = value;
			}
		}

		public ParameterInfo this[string name]
		{
			get
			{
				for (int i = 0; i < base.Count; i++)
				{
					if (this[i].Name == name)
					{
						return this[i];
					}
				}
				return null;
			}
		}

		public NameValueCollection AsNameValueCollectionInUserCulture
		{
			get
			{
				NameValueCollection nameValueCollection = new NameValueCollection(Count, StringComparer.Ordinal);
				IEnumerator enumerator = GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						ParameterInfo parameterInfo = (ParameterInfo)enumerator.Current;
						if (parameterInfo.Values != null && parameterInfo.Values.Length != 0)
						{
							for (int i = 0; i < parameterInfo.Values.Length; i++)
							{
								nameValueCollection.Add(parameterInfo.Name, parameterInfo.CastToString(parameterInfo.Values[i], Localization.ClientPrimaryCulture));
							}
						}
					}
					return nameValueCollection;
				}
				finally
				{
					IDisposable disposable = enumerator as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
			}
		}

		public int VisibleCount
		{
			get
			{
				int num = 0;
				IEnumerator enumerator = GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						if (((ParameterInfo)enumerator.Current).IsVisible)
						{
							num++;
						}
					}
					return num;
				}
				finally
				{
					IDisposable disposable = enumerator as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
			}
		}

		public bool Validated
		{
			get
			{
				return m_validated;
			}
			set
			{
				m_validated = value;
			}
		}

		public bool IsAnyParameterDynamic
		{
			get
			{
				IEnumerator enumerator = GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						ParameterInfo parameterInfo = (ParameterInfo)enumerator.Current;
						if (parameterInfo.DynamicDefaultValue || parameterInfo.DynamicValidValues)
						{
							return true;
						}
					}
				}
				finally
				{
					IDisposable disposable = enumerator as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
				return false;
			}
		}

		public UserProfileState UserProfileState
		{
			get
			{
				return m_userProfileState;
			}
			set
			{
				m_userProfileState = value;
			}
		}

		public ParametersGridLayout ParametersLayout
		{
			get
			{
				return m_parametersLayout;
			}
			set
			{
				m_parametersLayout = value;
			}
		}

		public ParameterInfoCollection()
		{
		}

		internal ParameterInfoCollection(int capacity)
			: base(capacity)
		{
		}

		public ParameterInfoCollection(SerializationInfo info, StreamingContext context)
		{
			PopulateFromXml(info.GetString("Xml"));
		}

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Xml", ToXmlWithTransientState());
		}

		public void Add(ParameterInfo parameterInfo)
		{
			base.Add(parameterInfo);
		}

		public void CopyTo(ParameterInfoCollection target)
		{
			if (target == null)
			{
				return;
			}
			for (int i = 0; i < base.Count; i++)
			{
				target.Add(new ParameterInfo(this[i]));
			}
			if (ParametersLayout != null)
			{
				target.ParametersLayout = new ParametersGridLayout
				{
					NumberOfColumns = ParametersLayout.NumberOfColumns,
					NumberOfRows = ParametersLayout.NumberOfRows,
					CellDefinitions = new ParametersGridCellDefinitionList()
				};
				if (ParametersLayout.CellDefinitions != null)
				{
					foreach (ParameterGridLayoutCellDefinition cellDefinition in ParametersLayout.CellDefinitions)
					{
						target.ParametersLayout.CellDefinitions.Add(new ParameterGridLayoutCellDefinition
						{
							ColumnIndex = cellDefinition.ColumnIndex,
							RowIndex = cellDefinition.RowIndex,
							ParameterName = cellDefinition.ParameterName
						});
					}
				}
			}
			target.FixupDependencies();
		}

		public string GetParameterWithNoValue()
		{
			for (int i = 0; i < Count; i++)
			{
				ParameterInfo parameterInfo = this[i];
				if (!parameterInfo.DynamicDefaultValue && parameterInfo.Values == null && parameterInfo.DefaultValues == null)
				{
					return parameterInfo.Name;
				}
			}
			return null;
		}

		public bool ValuesAreValid(out bool satisfiable, bool throwOnUnsatisfiable, out bool hasMissingValidValue)
		{
			hasMissingValidValue = false;
			if (!Validated)
			{
				for (int i = 0; i < Count; i++)
				{
					ParameterInfo parameterInfo = this[i];
					if (parameterInfo.DynamicValidValues)
					{
						parameterInfo.State = ReportParameterState.DynamicValuesUnavailable;
						continue;
					}
					if (parameterInfo.ValueIsValid())
					{
						parameterInfo.State = ReportParameterState.HasValidValue;
						continue;
					}
					hasMissingValidValue = true;
					parameterInfo.State = ReportParameterState.MissingValidValue;
				}
			}
			bool result = true;
			for (int j = 0; j < Count; j++)
			{
				ParameterInfo parameterInfo2 = this[j];
				if (parameterInfo2.State == ReportParameterState.HasValidValue)
				{
					continue;
				}
				result = false;
				if (!parameterInfo2.PromptUser && parameterInfo2.State == ReportParameterState.MissingValidValue)
				{
					hasMissingValidValue = true;
					satisfiable = false;
					if (throwOnUnsatisfiable)
					{
						throw new ReportProcessingException(ErrorCode.rsParameterError);
					}
					return false;
				}
			}
			satisfiable = true;
			return result;
		}

		public bool ValuesAreValid()
		{
			bool satisfiable;
			bool hasMissingValidValue;
			return ValuesAreValid(out satisfiable, throwOnUnsatisfiable: false, out hasMissingValidValue);
		}

		public bool ValuesAreValid(out bool hasMissingValidValue)
		{
			bool satisfiable;
			return ValuesAreValid(out satisfiable, throwOnUnsatisfiable: false, out hasMissingValidValue);
		}

		public bool ValuesAreValid(out bool satisfiable, bool throwOnUnsatisfiable)
		{
			bool hasMissingValidValue;
			return ValuesAreValid(out satisfiable, throwOnUnsatisfiable, out hasMissingValidValue);
		}

		public bool NeedPrompts()
		{
			for (int i = 0; i < Count; i++)
			{
				if (this[i].State != 0)
				{
					return true;
				}
			}
			return false;
		}

		public void ThrowIfNotValid()
		{
			for (int i = 0; i < Count; i++)
			{
				ParameterInfo parameterInfo = this[i];
				switch (parameterInfo.State)
				{
				case ReportParameterState.InvalidValueProvided:
				case ReportParameterState.DefaultValueInvalid:
					throw new InvalidReportParameterException(parameterInfo.Name);
				case ReportParameterState.MissingValidValue:
				{
					bool isSharedDataSetParameter = ParameterBase.IsSharedDataSetParameterObjectType(parameterInfo.ParameterObjectType);
					ThrowParameterValueNotSetException(parameterInfo.Name, isSharedDataSetParameter);
					break;
				}
				case ReportParameterState.HasOutstandingDependencies:
				case ReportParameterState.DynamicValuesUnavailable:
					throw new ReportProcessingException(ErrorCode.rsReportParameterProcessingError, parameterInfo.Name);
				default:
					throw new InternalCatalogException("Invalid parameter state encountered");
				case ReportParameterState.HasValidValue:
					break;
				}
			}
		}

		public void ValidateInputValues(ParamValues inputValues, bool isSnapshotExecution, bool isSharedDataSetParameters)
		{
			ParameterInfo parameterInfo = null;
			object obj = new object();
			for (int i = 0; i < Count; i++)
			{
				parameterInfo = this[i];
				ParamValueList paramValueList = inputValues[parameterInfo.Name];
				bool flag = isSnapshotExecution && parameterInfo.UsedInQuery && parameterInfo.DefaultValues != null && parameterInfo.DefaultValues.Length != 0;
				Dictionary<object, bool> dictionary = null;
				if (flag)
				{
					dictionary = new Dictionary<object, bool>(parameterInfo.DefaultValues.Length);
					for (int j = 0; j < parameterInfo.DefaultValues.Length; j++)
					{
						object obj2 = parameterInfo.DefaultValues[j];
						if (obj2 == null)
						{
							obj2 = obj;
						}
						if (!dictionary.ContainsKey(obj2))
						{
							dictionary.Add(obj2, value: false);
						}
					}
				}
				if (paramValueList == null)
				{
					if (parameterInfo.PromptUser && parameterInfo.DefaultValues == null && !parameterInfo.Nullable)
					{
						if (isSharedDataSetParameters)
						{
							Global.Tracer.Assert(ParameterBase.IsSharedDataSetParameterObjectType(parameterInfo.ParameterObjectType), "param.IsSharedDataSetParameter");
						}
						ThrowParameterValueNotSetException(parameterInfo.Name, isSharedDataSetParameters);
					}
					continue;
				}
				for (int k = 0; k < paramValueList.Count; k++)
				{
					ParamValue paramValue = paramValueList[k];
					if (paramValue.UseField)
					{
						continue;
					}
					object newValue = null;
					string text = paramValue.Value;
					if (text != null && text.Length == 0 && parameterInfo.DataType != DataType.String)
					{
						text = null;
					}
					if (text != null && parameterInfo.DataType != DataType.Object && !ParameterBase.CastFromString(text, out newValue, parameterInfo.DataType, Localization.ClientPrimaryCulture))
					{
						throw new ReportParameterTypeMismatchException(paramValue.Name);
					}
					if (flag)
					{
						object key = newValue;
						if (newValue == null)
						{
							newValue = obj;
						}
						if (dictionary.ContainsKey(key))
						{
							dictionary[key] = true;
							continue;
						}
						throw new InvalidReportParameterException(paramValue.Name);
					}
					if (text == null)
					{
						if (parameterInfo.Nullable)
						{
							continue;
						}
						if (isSharedDataSetParameters)
						{
							Global.Tracer.Assert(ParameterBase.IsSharedDataSetParameterObjectType(parameterInfo.ParameterObjectType), "fixedInputValu == null -> param.IsSharedDataSetParameter");
						}
						ThrowParameterValueNotSetException(parameterInfo.Name, isSharedDataSetParameters);
					}
					else if (parameterInfo.DataType == DataType.String && text == string.Empty)
					{
						if (!parameterInfo.AllowBlank)
						{
							throw new InvalidReportParameterException(parameterInfo.Name);
						}
						continue;
					}
					if (parameterInfo.ValidValues == null)
					{
						continue;
					}
					bool flag2 = false;
					int count = parameterInfo.ValidValues.Count;
					for (int l = 0; l < count; l++)
					{
						if (ParameterBase.ParameterValuesEqual(parameterInfo.ValidValues[l].Value, newValue))
						{
							flag2 = true;
							break;
						}
					}
					if (flag2 || parameterInfo.ValidValues.Count <= 0)
					{
						continue;
					}
					throw new InvalidReportParameterException(paramValue.Name);
				}
				if (!flag)
				{
					continue;
				}
				foreach (KeyValuePair<object, bool> item in dictionary)
				{
					if (!item.Value)
					{
						throw new InvalidReportParameterException(parameterInfo.Name);
					}
				}
			}
			if (inputValues == null)
			{
				return;
			}
			foreach (ParamValueList value in inputValues.Values)
			{
				string name = value[0].Name;
				if (this[name] == null)
				{
					ThrowUnknownParameterException(name, isSharedDataSetParameters);
				}
			}
		}

		private static void ThrowParameterValueNotSetException(string name, bool isSharedDataSetParameter)
		{
			if (isSharedDataSetParameter)
			{
				throw new DataSetParameterValueNotSetException(name);
			}
			throw new ReportParameterValueNotSetException(name);
		}

		private static void ThrowUnknownParameterException(string name, bool isSharedDataSetParameter)
		{
			if (isSharedDataSetParameter)
			{
				throw new UnknownDataSetParameterException(name);
			}
			throw new UnknownReportParameterException(name);
		}

		private static void ThrowReadOnlyParameterException(string name, bool isSharedDataSetParameter)
		{
			if (isSharedDataSetParameter)
			{
				throw new ReadOnlyDataSetParameterException(name);
			}
			throw new ReadOnlyReportParameterException(name);
		}

		public string ToUrl(bool skipInternalParameters)
		{
			return ToUrl(skipInternalParameters, null);
		}

		public string ToUrl(bool skipInternalParameters, Func<object, string> cs)
		{
			StringBuilder stringBuilder = new StringBuilder();
			IEnumerator enumerator = GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ParameterInfo parameterInfo = (ParameterInfo)enumerator.Current;
					if (skipInternalParameters && !parameterInfo.PromptUser)
					{
						continue;
					}
					if (parameterInfo.Values != null)
					{
						object[] values = parameterInfo.Values;
						foreach (object val in values)
						{
							UrlEncodeSingleParam(stringBuilder, parameterInfo.Name, val, cs);
						}
					}
					else
					{
						UrlEncodeSingleParam(stringBuilder, parameterInfo.Name, null, cs);
					}
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			return stringBuilder.ToString();
		}

		private static string UrlEncodeString(string param)
		{
			return UrlUtil.UrlEncode(param).Replace("'", "%27");
		}

		private static void UrlEncodeSingleParam(StringBuilder url, string name, object val)
		{
			UrlEncodeSingleParam(url, name, val, null);
		}

		private static void UrlEncodeSingleParam(StringBuilder url, string name, object val, Func<object, string> cs)
		{
			if (url.Length > 0)
			{
				url.Append('&');
			}
			url.Append(UrlEncodeString(name));
			if (val == null)
			{
				url.Append(":isnull=true");
				return;
			}
			url.Append('=');
			try
			{
				url.Append(UrlEncodeString((cs == null) ? val.ToString() : cs(val)));
			}
			catch (UriFormatException innnerException)
			{
				throw new InvalidParameterException(name, innnerException);
			}
		}

		public static string ToUrl(NameValueCollection coll)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < coll.Count; i++)
			{
				string key = coll.GetKey(i);
				string[] values = coll.GetValues(i);
				if (values == null)
				{
					UrlEncodeSingleParam(stringBuilder, key, null);
					continue;
				}
				string[] array = values;
				foreach (string val in array)
				{
					UrlEncodeSingleParam(stringBuilder, key, val);
				}
			}
			return stringBuilder.ToString();
		}

		public string ToXmlWithTransientState()
		{
			return ToXml(usedInQueryValuesOnly: false, writeTransientState: true, convertToString: false);
		}

		public string ToXml(bool usedInQueryValuesOnly)
		{
			return ToXml(usedInQueryValuesOnly, writeTransientState: false, usedInQueryValuesOnly);
		}

		private string ToXml(bool usedInQueryValuesOnly, bool writeTransientState, bool convertToString)
		{
			XmlTextWriter xmlTextWriter = null;
			try
			{
				StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
				xmlTextWriter = new XmlTextWriter(stringWriter);
				xmlTextWriter.Formatting = Formatting.Indented;
				xmlTextWriter.WriteStartElement("Parameters");
				XmlTextWriter xmlTextWriter2 = xmlTextWriter;
				int userProfileState = (int)m_userProfileState;
				xmlTextWriter2.WriteElementString("UserProfileState", userProfileState.ToString(CultureInfo.InvariantCulture));
				for (int i = 0; i < Count; i++)
				{
					ParameterInfo parameterInfo = this[i];
					if (usedInQueryValuesOnly)
					{
						if (parameterInfo.UsedInQuery)
						{
							parameterInfo.WriteNameValueToXml(xmlTextWriter, convertToString);
						}
					}
					else
					{
						parameterInfo.WriteToXml(xmlTextWriter, writeTransientState);
					}
				}
				if (!usedInQueryValuesOnly)
				{
					WriteParametersLayoutXml(xmlTextWriter);
				}
				xmlTextWriter.WriteEndElement();
				xmlTextWriter.Flush();
				return stringWriter.ToString();
			}
			finally
			{
				xmlTextWriter?.Close();
			}
		}

		private void WriteParametersLayoutXml(XmlTextWriter resultXml)
		{
			if (m_parametersLayout == null)
			{
				return;
			}
			resultXml.WriteStartElement("ParametersLayout");
			resultXml.WriteStartElement("ParametersGridLayoutDefinition");
			resultXml.WriteElementString("NumberOfColumns", m_parametersLayout.NumberOfColumns.ToString(CultureInfo.InvariantCulture));
			resultXml.WriteElementString("NumberOfRows", m_parametersLayout.NumberOfRows.ToString(CultureInfo.InvariantCulture));
			if (m_parametersLayout.CellDefinitions != null && m_parametersLayout.CellDefinitions.Count > 0)
			{
				resultXml.WriteStartElement("CellDefinitions");
				foreach (object cellDefinition in m_parametersLayout.CellDefinitions)
				{
					(cellDefinition as ParameterGridLayoutCellDefinition).WriteXml(resultXml);
				}
				resultXml.WriteEndElement();
			}
			resultXml.WriteEndElement();
			resultXml.WriteEndElement();
		}

		public static ParameterInfoCollection DecodeFromXml(string paramString)
		{
			ParameterInfoCollection parameterInfoCollection = new ParameterInfoCollection();
			parameterInfoCollection.PopulateFromXml(paramString);
			return parameterInfoCollection;
		}

		private void PopulateFromXml(string paramString)
		{
			if (paramString == null || paramString == string.Empty)
			{
				return;
			}
			XmlReader xmlReader = XmlUtil.SafeCreateXmlTextReader(paramString);
			try
			{
				xmlReader.MoveToContent();
				if (xmlReader.NodeType != XmlNodeType.Element || xmlReader.Name != "Parameters")
				{
					throw new InvalidXmlException();
				}
				while (true)
				{
					if (!xmlReader.Read())
					{
						return;
					}
					if (xmlReader.IsStartElement())
					{
						if (xmlReader.IsEmptyElement)
						{
							break;
						}
						switch (xmlReader.Name)
						{
						case "Parameter":
							xmlReader.Read();
							ParseOneParameter(xmlReader, this, CultureInfo.InvariantCulture);
							break;
						case "UserProfileState":
						{
							string s = xmlReader.ReadString();
							UserProfileState = (UserProfileState)int.Parse(s, CultureInfo.InvariantCulture);
							break;
						}
						case "ParametersLayout":
							xmlReader.Read();
							ParseParametersLayout(xmlReader, this, CultureInfo.InvariantCulture);
							break;
						default:
							throw new InvalidXmlException();
						}
					}
				}
				throw new InvalidXmlException();
			}
			catch (XmlException ex)
			{
				throw new MalformedXmlException(ex);
			}
		}

		public static ParameterInfoCollection DecodeFromNameValueCollectionAndUserCulture(NameValueCollection collection)
		{
			return DecodeFromNameValueCollectionAndUserCulture(collection, isDataSetParameters: false);
		}

		public static ParameterInfoCollection DecodeFromNameValueCollectionAndUserCulture(NameValueCollection collection, bool isDataSetParameters)
		{
			ParameterInfoCollection parameterInfoCollection = new ParameterInfoCollection();
			if (collection == null || collection.Count == 0)
			{
				return parameterInfoCollection;
			}
			for (int i = 0; i < collection.Count; i++)
			{
				ParameterInfo parameterInfo = new ParameterInfo();
				List<string> list = new List<string>();
				string[] values = collection.GetValues(i);
				if (values == null)
				{
					list.Add(null);
				}
				else
				{
					for (int j = 0; j < values.Length; j++)
					{
						list.Add(values[j]);
					}
				}
				parameterInfo.Parse(collection.GetKey(i), null, null, null, null, null, null, null, null, null, null, null, null, null, null, list, null, Localization.ClientPrimaryCulture);
				if (isDataSetParameters)
				{
					parameterInfo.DataType = DataType.Object;
				}
				parameterInfoCollection.Add(parameterInfo);
			}
			return parameterInfoCollection;
		}

		private static void ParseOneParameter(XmlReader sourceXmlReader, ParameterInfoCollection result, CultureInfo culture)
		{
			string name = null;
			string type = null;
			string nullable = null;
			string prompt = null;
			string allowBlank = null;
			string multiValue = null;
			string promptUser = null;
			string usedInQuery = null;
			string state = null;
			ValidValueList validValues = null;
			List<string> defaultValues = null;
			List<string> values = null;
			List<string> dependencies = null;
			string dynamicValidValues = null;
			string dynamicDefaultValue = null;
			string dynamicPrompt = null;
			string text = null;
			string text2 = null;
			string text3 = null;
			while (sourceXmlReader.IsStartElement())
			{
				bool isEmptyElement = sourceXmlReader.IsEmptyElement;
				string name2 = sourceXmlReader.Name;
				string text4 = sourceXmlReader.ReadString();
				switch (name2)
				{
				case "Name":
					name = text4;
					break;
				case "Type":
					type = text4;
					break;
				case "Nullable":
					nullable = text4;
					break;
				case "AllowBlank":
					allowBlank = text4;
					break;
				case "MultiValue":
					multiValue = text4;
					break;
				case "UsedInQuery":
					usedInQuery = text4;
					break;
				case "State":
					state = text4;
					break;
				case "Prompt":
					prompt = text4;
					break;
				case "DynamicPrompt":
					dynamicPrompt = text4;
					break;
				case "PromptUser":
					promptUser = text4;
					break;
				case "Dependencies":
					if (!isEmptyElement)
					{
						dependencies = ParseXmlList(sourceXmlReader, "Dependency");
					}
					break;
				case "DynamicValidValues":
					dynamicValidValues = text4;
					break;
				case "ValidValues":
					if (!isEmptyElement)
					{
						validValues = ParseValidValues(sourceXmlReader);
					}
					break;
				case "DynamicDefaultValue":
					dynamicDefaultValue = text4;
					break;
				case "DefaultValues":
					if (!isEmptyElement)
					{
						defaultValues = ParseXmlList(sourceXmlReader, "Value");
					}
					break;
				case "Values":
					if (!isEmptyElement)
					{
						values = ParseXmlList(sourceXmlReader, "Value");
					}
					break;
				case "IsUserSupplied":
					if (!isEmptyElement)
					{
						text3 = text4;
					}
					break;
				case "ValuesChanged":
					if (!isEmptyElement)
					{
						text = text4;
					}
					break;
				case "UseExplicitDefaultValue":
					if (!isEmptyElement)
					{
						text2 = text4;
					}
					break;
				}
				if (!isEmptyElement)
				{
					sourceXmlReader.ReadEndElement();
				}
				else
				{
					sourceXmlReader.Read();
				}
			}
			ParameterInfo parameterInfo = new ParameterInfo();
			ParameterInfoCollection dependencies2 = ParseDependencies(result, dependencies);
			parameterInfo.Parse(name, type, nullable, allowBlank, multiValue, usedInQuery, state, dynamicPrompt, prompt, promptUser, dependencies2, dynamicValidValues, validValues, dynamicDefaultValue, defaultValues, values, null, culture);
			if (text3 != null && bool.TryParse(text3, out bool result2))
			{
				parameterInfo.IsUserSupplied = result2;
			}
			if (text != null && bool.TryParse(text, out result2))
			{
				parameterInfo.ValuesChanged = result2;
			}
			if (text2 != null && bool.TryParse(text2, out result2))
			{
				parameterInfo.UseExplicitDefaultValue = result2;
			}
			try
			{
				result.Add(parameterInfo);
			}
			catch (ArgumentException)
			{
				throw new InvalidXmlException();
			}
		}

		private static void ParseParametersLayout(XmlReader sourceXmlReader, ParameterInfoCollection result, CultureInfo culture)
		{
			while (sourceXmlReader.IsStartElement())
			{
				_ = sourceXmlReader.IsEmptyElement;
				string name = sourceXmlReader.Name;
				sourceXmlReader.ReadString();
				if (name == "ParametersGridLayoutDefinition")
				{
					ParseParametersGridLayoutDefinition(sourceXmlReader, result, culture);
					sourceXmlReader.ReadEndElement();
				}
				sourceXmlReader.ReadEndElement();
			}
		}

		private static void ParseParametersGridLayoutDefinition(XmlReader sourceXmlReader, ParameterInfoCollection result, CultureInfo culture)
		{
			ParametersGridLayout parametersGridLayout = new ParametersGridLayout();
			parametersGridLayout.CellDefinitions = new ParametersGridCellDefinitionList();
			while (sourceXmlReader.IsStartElement())
			{
				_ = sourceXmlReader.IsEmptyElement;
				string name = sourceXmlReader.Name;
				string s = sourceXmlReader.ReadString();
				switch (name)
				{
				case "NumberOfColumns":
					parametersGridLayout.NumberOfColumns = int.Parse(s, CultureInfo.InvariantCulture);
					break;
				case "NumberOfRows":
					parametersGridLayout.NumberOfRows = int.Parse(s, CultureInfo.InvariantCulture);
					break;
				case "CellDefinitions":
					ParseParametersCellDefinitions(parametersGridLayout, sourceXmlReader, result, culture);
					break;
				}
				sourceXmlReader.ReadEndElement();
			}
			result.ParametersLayout = parametersGridLayout;
		}

		private static void ParseParametersCellDefinitions(ParametersGridLayout gridLayout, XmlReader sourceXmlReader, ParameterInfoCollection result, CultureInfo culture)
		{
			gridLayout.CellDefinitions = new ParametersGridCellDefinitionList();
			while (sourceXmlReader.IsStartElement())
			{
				_ = sourceXmlReader.IsEmptyElement;
				string name = sourceXmlReader.Name;
				sourceXmlReader.ReadString();
				if (name == "CellDefinition")
				{
					ParameterGridLayoutCellDefinition value = ParseParameterCellDefinition(sourceXmlReader, result, culture);
					gridLayout.CellDefinitions.Add(value);
					sourceXmlReader.ReadEndElement();
				}
			}
		}

		private static ParameterGridLayoutCellDefinition ParseParameterCellDefinition(XmlReader sourceXmlReader, ParameterInfoCollection result, CultureInfo culture)
		{
			ParameterGridLayoutCellDefinition parameterGridLayoutCellDefinition = new ParameterGridLayoutCellDefinition();
			while (sourceXmlReader.IsStartElement())
			{
				_ = sourceXmlReader.IsEmptyElement;
				string name = sourceXmlReader.Name;
				string text = sourceXmlReader.ReadString();
				switch (name)
				{
				case "RowIndex":
					parameterGridLayoutCellDefinition.RowIndex = int.Parse(text, CultureInfo.InvariantCulture);
					break;
				case "ColumnIndex":
					parameterGridLayoutCellDefinition.ColumnIndex = int.Parse(text, CultureInfo.InvariantCulture);
					break;
				case "ParameterName":
					parameterGridLayoutCellDefinition.ParameterName = text;
					break;
				}
				sourceXmlReader.ReadEndElement();
			}
			return parameterGridLayoutCellDefinition;
		}

		private static ParameterInfoCollection ParseDependencies(ParameterInfoCollection parameters, List<string> dependencies)
		{
			if (dependencies == null)
			{
				return null;
			}
			ParameterInfoCollection parameterInfoCollection = new ParameterInfoCollection();
			for (int i = 0; i < dependencies.Count; i++)
			{
				ParameterInfo parameterInfo = parameters[dependencies[i]];
				if (parameterInfo == null)
				{
					throw new InternalCatalogException(string.Format(CultureInfo.InvariantCulture, "Found that parameter '{0}' depend on parameter that is not found before it.", dependencies[i]));
				}
				parameterInfoCollection.Add(parameterInfo);
			}
			return parameterInfoCollection;
		}

		private static List<string> ParseXmlList(XmlReader sourceXmlReader, string expectedElement)
		{
			List<string> list = null;
			while (sourceXmlReader.IsStartElement())
			{
				bool isEmptyElement = sourceXmlReader.IsEmptyElement;
				string name = sourceXmlReader.Name;
				string attribute = sourceXmlReader.GetAttribute("nil");
				string item = sourceXmlReader.ReadString();
				if (expectedElement != null && name != expectedElement)
				{
					throw new InvalidXmlException();
				}
				if (list == null)
				{
					list = new List<string>();
				}
				if (attribute == bool.TrueString)
				{
					list.Add(null);
				}
				else
				{
					list.Add(item);
				}
				if (!isEmptyElement)
				{
					sourceXmlReader.ReadEndElement();
				}
				else
				{
					sourceXmlReader.Read();
				}
			}
			return list;
		}

		private static ValidValueList ParseValidValues(XmlReader sourceXmlReader)
		{
			ValidValueList validValueList = null;
			while (sourceXmlReader.IsStartElement())
			{
				bool isEmptyElement = sourceXmlReader.IsEmptyElement;
				string name = sourceXmlReader.Name;
				sourceXmlReader.ReadString();
				if (name != "ValidValue")
				{
					throw new InvalidXmlException();
				}
				if (validValueList == null)
				{
					validValueList = new ValidValueList();
				}
				string val = null;
				string label = null;
				if (!isEmptyElement)
				{
					ParseValueLabel(sourceXmlReader, out val, out label);
				}
				ValidValue value = new ValidValue(val, label);
				validValueList.Add(value);
				if (!isEmptyElement)
				{
					sourceXmlReader.ReadEndElement();
				}
				else
				{
					sourceXmlReader.Read();
				}
			}
			return validValueList;
		}

		private static void ParseValueLabel(XmlReader sourceXmlReader, out string val, out string label)
		{
			val = null;
			label = null;
			while (sourceXmlReader.IsStartElement())
			{
				bool isEmptyElement = sourceXmlReader.IsEmptyElement;
				string name = sourceXmlReader.Name;
				string text = sourceXmlReader.ReadString();
				if (name == "Value")
				{
					val = text;
				}
				else if (name == "Label")
				{
					label = text;
				}
				if (!isEmptyElement)
				{
					sourceXmlReader.ReadEndElement();
				}
				else
				{
					sourceXmlReader.Read();
				}
			}
		}

		public static ParameterInfoCollection Match(ParameterInfoCollection oldParameters, ParameterInfoCollection newParameters)
		{
			bool metaChanges;
			return Match(oldParameters, newParameters, out metaChanges);
		}

		public static ParameterInfoCollection Match(ParameterInfoCollection oldParameters, ParameterInfoCollection newParameters, out bool metaChanges)
		{
			metaChanges = false;
			ParameterInfoCollection parameterInfoCollection = new ParameterInfoCollection();
			for (int i = 0; i < newParameters.Count; i++)
			{
				ParameterInfo parameterInfo = newParameters[i];
				ParameterInfo parameterInfo2 = oldParameters[parameterInfo.Name];
				if (parameterInfo2 != null)
				{
					parameterInfo.PromptUser = parameterInfo2.PromptUser;
					if (parameterInfo.DynamicDefaultValue)
					{
						parameterInfoCollection.Add(parameterInfo);
						continue;
					}
					ParameterInfo parameterInfo3 = ParameterInfo.Cast(parameterInfo2, parameterInfo, Localization.ClientPrimaryCulture, ref metaChanges);
					if (parameterInfo3 != null)
					{
						parameterInfoCollection.Add(parameterInfo3);
					}
					else
					{
						parameterInfoCollection.Add(parameterInfo);
					}
				}
				else
				{
					parameterInfoCollection.Add(parameterInfo);
					metaChanges = true;
				}
			}
			for (int j = 0; j < oldParameters.Count; j++)
			{
				ParameterInfo parameterInfo4 = oldParameters[j];
				ParameterInfo parameterInfo5 = newParameters[parameterInfo4.Name];
				if (parameterInfo5 == null || j != newParameters.IndexOf(parameterInfo5))
				{
					metaChanges = true;
					break;
				}
			}
			parameterInfoCollection.ParametersLayout = newParameters.ParametersLayout;
			return parameterInfoCollection;
		}

		public static ParameterInfoCollection Combine(ParameterInfoCollection oldParameters, ParameterInfoCollection newParameters, bool checkReadOnly, bool ignoreNewQueryParams, bool isParameterDefinitionUpdate, bool isSharedDataSetParameter)
		{
			return Combine(oldParameters, newParameters, checkReadOnly, ignoreNewQueryParams, isParameterDefinitionUpdate, isSharedDataSetParameter, Localization.ClientPrimaryCulture);
		}

		public static ParameterInfoCollection Combine(ParameterInfoCollection oldParameters, ParameterInfoCollection newParameters, bool checkReadOnly, bool ignoreNewQueryParams, bool isParameterDefinitionUpdate, bool isSharedDataSetParameter, CultureInfo culture)
		{
			if (newParameters == null)
			{
				return oldParameters;
			}
			ParameterInfoCollection parameterInfoCollection = new ParameterInfoCollection();
			for (int i = 0; i < oldParameters.Count; i++)
			{
				ParameterInfo parameterInfo = oldParameters[i];
				ParameterInfo parameterInfo2 = newParameters[parameterInfo.Name];
				if (parameterInfo2 == null || (ignoreNewQueryParams && parameterInfo.UsedInQuery))
				{
					parameterInfo.ValuesChanged = false;
					parameterInfoCollection.Add(parameterInfo);
					continue;
				}
				if (checkReadOnly && !parameterInfo.PromptUser)
				{
					ThrowReadOnlyParameterException(parameterInfo.Name, isSharedDataSetParameter);
				}
				ParameterInfo parameterInfo3 = ParameterInfo.Cast(parameterInfo2, parameterInfo, culture);
				if (parameterInfo3 != null)
				{
					if (!checkReadOnly)
					{
						parameterInfo3.PromptUser = parameterInfo2.PromptUser;
						parameterInfo3.Prompt = parameterInfo2.Prompt;
					}
					parameterInfo3.IsUserSupplied = true;
					if (isParameterDefinitionUpdate)
					{
						if (parameterInfo2.UseExplicitDefaultValue)
						{
							parameterInfo3.DynamicDefaultValue = parameterInfo2.DynamicDefaultValue;
						}
						else
						{
							Global.Tracer.Assert(parameterInfo3.Values == null && parameterInfo3.DefaultValues == null, "(null == casted.Values && null == casted.DefaultValues)");
							parameterInfo3.Values = parameterInfo.Values;
							parameterInfo3.DefaultValues = parameterInfo.DefaultValues;
						}
					}
					parameterInfo3.ValuesChanged = !SameParameterValues(parameterInfo3, parameterInfo);
					parameterInfoCollection.Add(parameterInfo3);
					continue;
				}
				throw new ReportParameterTypeMismatchException(parameterInfo2.Name);
			}
			for (int j = 0; j < newParameters.Count; j++)
			{
				ParameterInfo parameterInfo4 = newParameters[j];
				if (oldParameters[parameterInfo4.Name] == null)
				{
					ThrowUnknownParameterException(parameterInfo4.Name, isSharedDataSetParameter);
				}
			}
			parameterInfoCollection.FixupDependencies();
			parameterInfoCollection.UserProfileState = (oldParameters.UserProfileState | newParameters.UserProfileState);
			parameterInfoCollection.ParametersLayout = oldParameters.ParametersLayout;
			return parameterInfoCollection;
		}

		private void FixupDependencies()
		{
			for (int i = 0; i < Count; i++)
			{
				ParameterInfo parameterInfo = this[i];
				if (parameterInfo.DependencyList != null)
				{
					for (int j = 0; j < parameterInfo.DependencyList.Count; j++)
					{
						ParameterInfo parameterInfo2 = this[parameterInfo.DependencyList[j].Name];
						parameterInfo.DependencyList[j] = parameterInfo2;
						parameterInfo2.OthersDependOnMe = true;
					}
				}
			}
		}

		public void SameParameters(ParameterInfoCollection otherParameters, out bool sameQueryParameters, out bool sameSnapshotParameters)
		{
			sameQueryParameters = true;
			sameSnapshotParameters = true;
			bool flag = false;
			for (int i = 0; i < Count; i++)
			{
				if (flag)
				{
					break;
				}
				ParameterInfo parameterInfo = this[i];
				ParameterInfo otherParameter = otherParameters[parameterInfo.Name];
				if (!SameParameterValues(parameterInfo, otherParameter))
				{
					flag = UpdateParameterFlagsAndBreak(parameterInfo.UsedInQuery, ref sameQueryParameters, ref sameSnapshotParameters);
				}
			}
		}

		private bool UpdateParameterFlagsAndBreak(bool usedInQuery, ref bool sameQueryParameters, ref bool sameSnapshotParameters)
		{
			if (usedInQuery)
			{
				sameQueryParameters = false;
				if (!sameSnapshotParameters)
				{
					return true;
				}
			}
			else
			{
				sameSnapshotParameters = false;
				if (!sameQueryParameters)
				{
					return true;
				}
			}
			return false;
		}

		private static bool SameParameterValues(ParameterInfo thisParameter, ParameterInfo otherParameter)
		{
			Global.Tracer.Assert(thisParameter != null, "thisParameter");
			if (thisParameter == null != (otherParameter == null) || thisParameter.Values == null != (otherParameter.Values == null))
			{
				return false;
			}
			if (thisParameter.Values != null)
			{
				int num = thisParameter.Values.Length;
				if (num != otherParameter.Values.Length)
				{
					return false;
				}
				if (num == 1)
				{
					if (!ParameterBase.ParameterValuesEqual(thisParameter.Values[0], otherParameter.Values[0]))
					{
						return false;
					}
				}
				else
				{
					Hashtable hashtable = new Hashtable();
					for (int i = 0; i < num; i++)
					{
						if (!hashtable.ContainsKey(thisParameter.Values[i].GetHashCode()))
						{
							hashtable.Add(thisParameter.Values[i].GetHashCode(), thisParameter.Values[i]);
						}
					}
					for (int j = 0; j < num; j++)
					{
						int hashCode = otherParameter.Values[j].GetHashCode();
						if (hashtable.ContainsKey(hashCode))
						{
							if (!ParameterBase.ParameterValuesEqual(hashtable[hashCode], otherParameter.Values[j]))
							{
								return false;
							}
							hashtable.Remove(hashCode);
						}
					}
					if (hashtable.Count != 0)
					{
						return false;
					}
				}
			}
			return true;
		}

		private bool SameReportParameters(NameValueCollection otherParams, bool ignoreQueryParams)
		{
			if (otherParams == null || otherParams.Count == 0)
			{
				return true;
			}
			for (int i = 0; i < Count; i++)
			{
				ParameterInfo parameterInfo = this[i];
				string[] values = otherParams.GetValues(parameterInfo.Name);
				if ((ignoreQueryParams && parameterInfo.UsedInQuery) || values == null)
				{
					continue;
				}
				if (parameterInfo.Values == null || parameterInfo.Values.Length == 0)
				{
					return false;
				}
				if (values == null || values.Length != parameterInfo.Values.Length)
				{
					return false;
				}
				for (int j = 0; j < parameterInfo.Values.Length; j++)
				{
					object o = parameterInfo.Values[j];
					if (!ParameterBase.CastFromString(values[j], out object newValue, parameterInfo.DataType, Localization.ClientPrimaryCulture))
					{
						return false;
					}
					if (!ParameterBase.ParameterValuesEqual(o, newValue))
					{
						return false;
					}
				}
			}
			return true;
		}

		public bool SameSnapshotParameters(NameValueCollection otherParams)
		{
			return SameReportParameters(otherParams, ignoreQueryParams: true);
		}

		public bool SameReportParameters(string passedInParameters)
		{
			if (passedInParameters == null)
			{
				return true;
			}
			ParameterInfoCollection parameterInfoCollection = DecodeFromXml(passedInParameters);
			if (parameterInfoCollection == null || parameterInfoCollection.Count == 0)
			{
				return true;
			}
			return SameReportParameters(parameterInfoCollection.AsNameValueCollectionInUserCulture, ignoreQueryParams: false);
		}

		internal void StoreLabels()
		{
			for (int i = 0; i < Count; i++)
			{
				this[i].StoreLabels();
			}
		}

		internal static Declaration GetDeclaration()
		{
			if (m_Declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.UserProfileState, Token.Enum));
				list.Add(new MemberInfo(MemberName.Parameters, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterInfo));
				list.Add(new MemberInfo(MemberName.ParametersLayout, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParametersLayout, Lifetime.AddedIn(300)));
				return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterInfoCollection, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return m_Declaration;
		}

		public ParameterInfoCollection GetQueryParameters()
		{
			ParameterInfoCollection parameterInfoCollection = new ParameterInfoCollection();
			IEnumerator enumerator = GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ParameterInfo parameterInfo = (ParameterInfo)enumerator.Current;
					if (parameterInfo.UsedInQuery)
					{
						parameterInfoCollection.Add(parameterInfo);
					}
				}
				return parameterInfoCollection;
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
		}

		void IPersistable.Serialize(IntermediateFormatWriter writer)
		{
			for (int i = 0; i < Count; i++)
			{
				this[i].IndexInCollection = i;
			}
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.UserProfileState:
					writer.WriteEnum((int)m_userProfileState);
					break;
				case MemberName.Parameters:
					writer.Write((ArrayList)this);
					break;
				case MemberName.ParametersLayout:
					writer.Write(m_parametersLayout);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		void IPersistable.Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.UserProfileState:
					m_userProfileState = (UserProfileState)reader.ReadEnum();
					break;
				case MemberName.Parameters:
					reader.ReadListOfRIFObjects(this);
					break;
				case MemberName.ParametersLayout:
					m_parametersLayout = (ParametersGridLayout)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
			for (int i = 0; i < Count; i++)
			{
				this[i].ResolveDependencies(this);
			}
		}

		void IPersistable.ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false);
		}

		Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType IPersistable.GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterInfoCollection;
		}
	}
}
