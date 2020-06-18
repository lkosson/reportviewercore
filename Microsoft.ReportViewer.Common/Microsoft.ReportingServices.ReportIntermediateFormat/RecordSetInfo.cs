using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class RecordSetInfo : IPersistable
	{
		private bool m_readerExtensionsSupported;

		private RecordSetPropertyNamesList m_fieldPropertyNames;

		private string[] m_fieldNames;

		private CompareOptions m_compareOptions;

		private string m_commandText;

		private string m_rewrittenCommandText;

		private string m_cultureName;

		private DateTime m_executionTime = DateTime.MinValue;

		[NonSerialized]
		private bool m_validCompareOptions;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal bool ReaderExtensionsSupported => m_readerExtensionsSupported;

		internal RecordSetPropertyNamesList FieldPropertyNames => m_fieldPropertyNames;

		internal CompareOptions CompareOptions => m_compareOptions;

		internal string[] FieldNames => m_fieldNames;

		internal string CommandText => m_commandText;

		internal string RewrittenCommandText => m_rewrittenCommandText;

		internal string CultureName => m_cultureName;

		internal DateTime ExecutionTime => m_executionTime;

		internal bool ValidCompareOptions => m_validCompareOptions;

		internal RecordSetInfo()
		{
		}

		internal RecordSetInfo(bool readerExtensionsSupported, bool persistCalculatedFields, DataSetInstance dataSetInstance, DateTime reportExecutionTime)
		{
			m_readerExtensionsSupported = readerExtensionsSupported;
			m_compareOptions = dataSetInstance.DataSetDef.GetCLRCompareOptions();
			m_commandText = dataSetInstance.CommandText;
			m_rewrittenCommandText = dataSetInstance.RewrittenCommandText;
			m_cultureName = dataSetInstance.DataSetDef.CreateCultureInfoFromLcid().Name;
			m_executionTime = dataSetInstance.GetQueryExecutionTime(reportExecutionTime);
			int count = dataSetInstance.DataSetDef.Fields.Count;
			if (count <= 0)
			{
				return;
			}
			int num = 0;
			if (persistCalculatedFields)
			{
				m_fieldNames = new string[count];
			}
			else
			{
				m_fieldNames = new string[dataSetInstance.DataSetDef.NonCalculatedFieldCount];
			}
			for (int i = 0; i < count; i++)
			{
				if (persistCalculatedFields || !dataSetInstance.DataSetDef.Fields[i].IsCalculatedField)
				{
					m_fieldNames[num++] = dataSetInstance.DataSetDef.Fields[i].Name;
				}
			}
		}

		internal void PopulateExtendedFieldsProperties(DataSetInstance dataSetInstance)
		{
			if (dataSetInstance.FieldInfos == null)
			{
				return;
			}
			int num = dataSetInstance.FieldInfos.Length;
			m_fieldPropertyNames = new RecordSetPropertyNamesList(num);
			for (int i = 0; i < num; i++)
			{
				FieldInfo fieldInfo = dataSetInstance.FieldInfos[i];
				RecordSetPropertyNames recordSetPropertyNames = null;
				if (fieldInfo != null && fieldInfo.PropertyCount != 0)
				{
					recordSetPropertyNames = new RecordSetPropertyNames();
					recordSetPropertyNames.PropertyNames = new List<string>(fieldInfo.PropertyCount);
					recordSetPropertyNames.PropertyNames.AddRange(fieldInfo.PropertyNames);
				}
				m_fieldPropertyNames.Add(recordSetPropertyNames);
			}
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.ReaderExtensionsSupported, Token.Boolean));
			list.Add(new MemberInfo(MemberName.FieldPropertyNames, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RecordSetPropertyNames));
			list.Add(new MemberInfo(MemberName.CompareOptions, Token.Enum));
			list.Add(new MemberInfo(MemberName.FieldNames, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveArray, Token.String));
			list.Add(new MemberInfo(MemberName.CommandText, Token.String));
			list.Add(new MemberInfo(MemberName.RewrittenCommandText, Token.String));
			list.Add(new MemberInfo(MemberName.CultureName, Token.String));
			list.Add(new MemberInfo(MemberName.ExecutionTime, Token.DateTime));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RecordSetInfo, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.ReaderExtensionsSupported:
					writer.Write(m_readerExtensionsSupported);
					break;
				case MemberName.FieldPropertyNames:
					writer.Write(m_fieldPropertyNames);
					break;
				case MemberName.CompareOptions:
					writer.WriteEnum((int)m_compareOptions);
					break;
				case MemberName.FieldNames:
					writer.Write(m_fieldNames);
					break;
				case MemberName.CommandText:
					writer.Write(m_commandText);
					break;
				case MemberName.RewrittenCommandText:
					writer.Write(m_rewrittenCommandText);
					break;
				case MemberName.CultureName:
					writer.Write(m_cultureName);
					break;
				case MemberName.ExecutionTime:
					writer.Write(m_executionTime);
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
				case MemberName.ReaderExtensionsSupported:
					m_readerExtensionsSupported = reader.ReadBoolean();
					break;
				case MemberName.FieldPropertyNames:
					m_fieldPropertyNames = reader.ReadListOfRIFObjects<RecordSetPropertyNamesList>();
					break;
				case MemberName.CompareOptions:
					m_compareOptions = (CompareOptions)reader.ReadEnum();
					break;
				case MemberName.FieldNames:
					m_fieldNames = reader.ReadStringArray();
					break;
				case MemberName.CommandText:
					m_commandText = reader.ReadString();
					break;
				case MemberName.RewrittenCommandText:
					m_rewrittenCommandText = reader.ReadString();
					break;
				case MemberName.CultureName:
					m_cultureName = reader.ReadString();
					break;
				case MemberName.ExecutionTime:
					m_executionTime = reader.ReadDateTime();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false);
		}

		public Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RecordSetInfo;
		}
	}
}
