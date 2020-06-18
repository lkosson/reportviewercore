using Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class DataSetCore : IPersistable, IExpressionHostAssemblyHolder
	{
		private string m_name;

		private List<Field> m_fields;

		private ReportQuery m_query;

		private SharedDataSetQuery m_sharedDataSetQuery;

		private string m_collation;

		private string m_collationCulture;

		private uint m_lcid = DataSetValidator.LOCALE_SYSTEM_DEFAULT;

		private DataSet.TriState m_caseSensitivity;

		private DataSet.TriState m_accentSensitivity;

		private DataSet.TriState m_kanatypeSensitivity;

		private DataSet.TriState m_widthSensitivity;

		private bool m_nullsAsBlanks;

		[NonSerialized]
		private bool m_useOrdinalStringKeyGeneration;

		private List<Filter> m_filters;

		private DataSet.TriState m_interpretSubtotalsAsDetails;

		private Guid m_catalogID = Guid.Empty;

		private int m_nonCalculatedFieldCount = -1;

		private byte[] m_compiledCode;

		private bool m_compiledCodeGeneratedWithRefusedPermissions;

		private int m_exprHostID = -1;

		private Guid m_exprHostAssemblyId = Guid.Empty;

		[NonSerialized]
		private bool? m_cachedUsesAggregateIndicatorFields;

		[NonSerialized]
		private DataSetExprHost m_exprHost;

		[NonSerialized]
		private FieldsContext m_fieldsContext;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

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

		internal List<Field> Fields
		{
			get
			{
				return m_fields;
			}
			set
			{
				m_fields = value;
			}
		}

		internal bool HasAggregateIndicatorFields
		{
			get
			{
				if (!m_cachedUsesAggregateIndicatorFields.HasValue)
				{
					m_cachedUsesAggregateIndicatorFields = false;
					if (m_fields != null)
					{
						m_cachedUsesAggregateIndicatorFields = m_fields.Any((Field field) => field.HasAggregateIndicatorField);
					}
				}
				return m_cachedUsesAggregateIndicatorFields.Value;
			}
		}

		internal ReportQuery Query
		{
			get
			{
				return m_query;
			}
			set
			{
				m_query = value;
			}
		}

		internal SharedDataSetQuery SharedDataSetQuery
		{
			get
			{
				return m_sharedDataSetQuery;
			}
			set
			{
				m_sharedDataSetQuery = value;
			}
		}

		internal string Collation
		{
			get
			{
				return m_collation;
			}
			set
			{
				m_collation = value;
			}
		}

		internal string CollationCulture
		{
			get
			{
				return m_collationCulture;
			}
			set
			{
				m_collationCulture = value;
			}
		}

		internal uint LCID
		{
			get
			{
				return m_lcid;
			}
			set
			{
				m_lcid = value;
			}
		}

		internal DataSet.TriState CaseSensitivity
		{
			get
			{
				return m_caseSensitivity;
			}
			set
			{
				m_caseSensitivity = value;
			}
		}

		internal DataSet.TriState AccentSensitivity
		{
			get
			{
				return m_accentSensitivity;
			}
			set
			{
				m_accentSensitivity = value;
			}
		}

		internal DataSet.TriState KanatypeSensitivity
		{
			get
			{
				return m_kanatypeSensitivity;
			}
			set
			{
				m_kanatypeSensitivity = value;
			}
		}

		internal DataSet.TriState WidthSensitivity
		{
			get
			{
				return m_widthSensitivity;
			}
			set
			{
				m_widthSensitivity = value;
			}
		}

		internal bool NullsAsBlanks
		{
			get
			{
				return m_nullsAsBlanks;
			}
			set
			{
				m_nullsAsBlanks = value;
			}
		}

		internal bool UseOrdinalStringKeyGeneration
		{
			get
			{
				return m_useOrdinalStringKeyGeneration;
			}
			set
			{
				m_useOrdinalStringKeyGeneration = value;
			}
		}

		internal List<Filter> Filters
		{
			get
			{
				return m_filters;
			}
			set
			{
				m_filters = value;
			}
		}

		internal DataSet.TriState InterpretSubtotalsAsDetails
		{
			get
			{
				return m_interpretSubtotalsAsDetails;
			}
			set
			{
				m_interpretSubtotalsAsDetails = value;
			}
		}

		internal int NonCalculatedFieldCount
		{
			get
			{
				if (m_nonCalculatedFieldCount < 0 && m_fields != null)
				{
					int i;
					for (i = 0; i < m_fields.Count && !m_fields[i].IsCalculatedField; i++)
					{
					}
					m_nonCalculatedFieldCount = i;
				}
				return m_nonCalculatedFieldCount;
			}
			set
			{
				m_nonCalculatedFieldCount = value;
			}
		}

		internal Guid CatalogID => m_catalogID;

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

		internal DataSetExprHost ExprHost
		{
			get
			{
				return m_exprHost;
			}
			set
			{
				m_exprHost = value;
			}
		}

		internal FieldsContext FieldsContext
		{
			get
			{
				return m_fieldsContext;
			}
			set
			{
				m_fieldsContext = value;
			}
		}

		Microsoft.ReportingServices.ReportProcessing.ObjectType IExpressionHostAssemblyHolder.ObjectType => Microsoft.ReportingServices.ReportProcessing.ObjectType.SharedDataSet;

		string IExpressionHostAssemblyHolder.ExprHostAssemblyName
		{
			get
			{
				if (m_exprHostAssemblyId == Guid.Empty)
				{
					m_exprHostAssemblyId = Guid.NewGuid();
					m_exprHostID = 0;
				}
				return "expression_host_RSD_" + m_exprHostAssemblyId.ToString().Replace("-", "");
			}
		}

		byte[] IExpressionHostAssemblyHolder.CompiledCode
		{
			get
			{
				return m_compiledCode;
			}
			set
			{
				m_compiledCode = value;
			}
		}

		bool IExpressionHostAssemblyHolder.CompiledCodeGeneratedWithRefusedPermissions
		{
			get
			{
				return m_compiledCodeGeneratedWithRefusedPermissions;
			}
			set
			{
				m_compiledCodeGeneratedWithRefusedPermissions = value;
			}
		}

		List<string> IExpressionHostAssemblyHolder.CodeModules
		{
			get
			{
				return null;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		List<CodeClass> IExpressionHostAssemblyHolder.CodeClasses
		{
			get
			{
				return null;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		internal DataSetCore()
		{
		}

		internal CultureInfo CreateCultureInfoFromLcid()
		{
			return new CultureInfo((int)LCID, useUserOverride: false);
		}

		internal void SetCatalogID(Guid id)
		{
			if (m_catalogID == Guid.Empty)
			{
				m_catalogID = id;
			}
		}

		internal void Initialize(InitializationContext context)
		{
			if (m_query != null)
			{
				m_query.Initialize(context);
			}
			else if (m_sharedDataSetQuery != null)
			{
				m_sharedDataSetQuery.Initialize(context);
			}
			if (m_fields != null)
			{
				int count = m_fields.Count;
				for (int i = 0; i < count; i++)
				{
					m_fields[i].Initialize(context);
				}
			}
			if (m_filters != null)
			{
				for (int j = 0; j < m_filters.Count; j++)
				{
					m_filters[j].Initialize(context);
				}
			}
		}

		internal CompareOptions GetCLRCompareOptions()
		{
			CompareOptions compareOptions = CompareOptions.None;
			if (DataSet.TriState.True != m_caseSensitivity)
			{
				compareOptions |= CompareOptions.IgnoreCase;
			}
			if (DataSet.TriState.True != m_accentSensitivity)
			{
				compareOptions |= CompareOptions.IgnoreNonSpace;
			}
			if (DataSet.TriState.True != m_kanatypeSensitivity)
			{
				compareOptions |= CompareOptions.IgnoreKanaType;
			}
			if (DataSet.TriState.True != m_widthSensitivity)
			{
				compareOptions |= CompareOptions.IgnoreWidth;
			}
			return compareOptions;
		}

		internal bool NeedAutoDetectCollation()
		{
			if (DataSetValidator.LOCALE_SYSTEM_DEFAULT != m_lcid && m_accentSensitivity != 0 && m_caseSensitivity != 0 && m_kanatypeSensitivity != 0)
			{
				return m_widthSensitivity == DataSet.TriState.Auto;
			}
			return true;
		}

		internal void MergeCollationSettings(ErrorContext errorContext, string dataSourceType, string cultureName, bool caseSensitive, bool accentSensitive, bool kanatypeSensitive, bool widthSensitive)
		{
			if (!NeedAutoDetectCollation())
			{
				return;
			}
			uint lcid = DataSetValidator.LOCALE_SYSTEM_DEFAULT;
			if (cultureName != null)
			{
				try
				{
					lcid = (uint)CultureInfo.GetCultureInfo(cultureName).LCID;
				}
				catch (Exception)
				{
					errorContext?.Register(ProcessingErrorCode.rsInvalidCollationCultureName, Severity.Warning, Microsoft.ReportingServices.ReportProcessing.ObjectType.DataSet, m_name, dataSourceType, cultureName);
				}
			}
			if (DataSetValidator.LOCALE_SYSTEM_DEFAULT == m_lcid)
			{
				m_lcid = lcid;
			}
			m_accentSensitivity = MergeSensitivity(m_accentSensitivity, accentSensitive);
			m_caseSensitivity = MergeSensitivity(m_caseSensitivity, caseSensitive);
			m_kanatypeSensitivity = MergeSensitivity(m_kanatypeSensitivity, kanatypeSensitive);
			m_widthSensitivity = MergeSensitivity(m_widthSensitivity, widthSensitive);
		}

		private DataSet.TriState MergeSensitivity(DataSet.TriState current, bool detectedValue)
		{
			if (current != 0)
			{
				return current;
			}
			if (detectedValue)
			{
				return DataSet.TriState.True;
			}
			return DataSet.TriState.False;
		}

		internal bool HasCalculatedFields()
		{
			if (m_fields != null)
			{
				return NonCalculatedFieldCount != m_fields.Count;
			}
			return false;
		}

		internal void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			if (m_exprHostID < 0)
			{
				return;
			}
			Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null, "(reportExprHost != null && reportObjectModel != null)");
			m_exprHost = reportExprHost.DataSetHostsRemotable[m_exprHostID];
			m_exprHost.SetReportObjectModel(reportObjectModel);
			if (m_exprHost.QueryParametersHost != null)
			{
				if (m_query != null)
				{
					m_query.SetExprHost(m_exprHost.QueryParametersHost, reportObjectModel);
				}
				else
				{
					m_sharedDataSetQuery.SetExprHost(m_exprHost.QueryParametersHost, reportObjectModel);
				}
			}
			if (m_exprHost.UserSortExpressionsHost != null)
			{
				m_exprHost.UserSortExpressionsHost.SetReportObjectModel(reportObjectModel);
			}
		}

		internal void SetFilterExprHost(ObjectModelImpl reportObjectModel)
		{
			if (m_filters != null && m_exprHost != null)
			{
				for (int i = 0; i < m_filters.Count; i++)
				{
					m_filters[i].SetExprHost(m_exprHost.FilterHostsRemotable, reportObjectModel);
				}
			}
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Name, Token.String));
			list.Add(new MemberInfo(MemberName.Fields, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Field));
			list.Add(new MemberInfo(MemberName.Query, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportQuery));
			list.Add(new MemberInfo(MemberName.SharedDataSetQuery, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SharedDataSetQuery));
			list.Add(new MemberInfo(MemberName.Collation, Token.String));
			list.Add(new MemberInfo(MemberName.LCID, Token.UInt32));
			list.Add(new MemberInfo(MemberName.CaseSensitivity, Token.Enum));
			list.Add(new MemberInfo(MemberName.AccentSensitivity, Token.Enum));
			list.Add(new MemberInfo(MemberName.KanatypeSensitivity, Token.Enum));
			list.Add(new MemberInfo(MemberName.WidthSensitivity, Token.Enum));
			list.Add(new MemberInfo(MemberName.Filters, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Filter));
			list.Add(new MemberInfo(MemberName.InterpretSubtotalsAsDetails, Token.Enum));
			list.Add(new MemberInfo(MemberName.CatalogID, Token.Guid));
			list.Add(new MemberInfo(MemberName.NonCalculatedFieldCount, Token.Int32));
			list.Add(new MemberInfo(MemberName.CompiledCode, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveTypedArray, Token.Byte));
			list.Add(new MemberInfo(MemberName.CompiledCodeGeneratedWithRefusedPermissions, Token.Boolean));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.ExprHostAssemblyID, Token.Guid));
			list.Add(new MemberInfo(MemberName.NullsAsBlanks, Token.Boolean));
			list.Add(new MemberInfo(MemberName.CollationCulture, Token.String));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataSetCore, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Name:
					writer.Write(m_name);
					break;
				case MemberName.Fields:
					writer.Write(m_fields);
					break;
				case MemberName.Query:
					writer.Write(m_query);
					break;
				case MemberName.SharedDataSetQuery:
					writer.Write(m_sharedDataSetQuery);
					break;
				case MemberName.Collation:
					writer.Write(m_collation);
					break;
				case MemberName.CollationCulture:
					writer.Write(m_collationCulture);
					break;
				case MemberName.LCID:
					writer.Write(m_lcid);
					break;
				case MemberName.CaseSensitivity:
					writer.WriteEnum((int)m_caseSensitivity);
					break;
				case MemberName.AccentSensitivity:
					writer.WriteEnum((int)m_accentSensitivity);
					break;
				case MemberName.KanatypeSensitivity:
					writer.WriteEnum((int)m_kanatypeSensitivity);
					break;
				case MemberName.WidthSensitivity:
					writer.WriteEnum((int)m_widthSensitivity);
					break;
				case MemberName.Filters:
					writer.Write(m_filters);
					break;
				case MemberName.InterpretSubtotalsAsDetails:
					writer.WriteEnum((int)m_interpretSubtotalsAsDetails);
					break;
				case MemberName.CatalogID:
					writer.Write(m_catalogID);
					break;
				case MemberName.NonCalculatedFieldCount:
					writer.Write(m_nonCalculatedFieldCount);
					break;
				case MemberName.CompiledCode:
					writer.Write(m_compiledCode);
					break;
				case MemberName.CompiledCodeGeneratedWithRefusedPermissions:
					writer.Write(m_compiledCodeGeneratedWithRefusedPermissions);
					break;
				case MemberName.ExprHostID:
					writer.Write(m_exprHostID);
					break;
				case MemberName.ExprHostAssemblyID:
					writer.Write(m_exprHostAssemblyId);
					break;
				case MemberName.NullsAsBlanks:
					writer.Write(m_nullsAsBlanks);
					break;
				default:
					Global.Tracer.Assert(condition: false, string.Empty);
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
				case MemberName.Name:
					m_name = reader.ReadString();
					break;
				case MemberName.Fields:
					m_fields = reader.ReadGenericListOfRIFObjects<Field>();
					break;
				case MemberName.Query:
					m_query = (ReportQuery)reader.ReadRIFObject();
					break;
				case MemberName.SharedDataSetQuery:
					m_sharedDataSetQuery = (SharedDataSetQuery)reader.ReadRIFObject();
					break;
				case MemberName.Collation:
					m_collation = reader.ReadString();
					break;
				case MemberName.CollationCulture:
					m_collationCulture = reader.ReadString();
					break;
				case MemberName.LCID:
					m_lcid = reader.ReadUInt32();
					break;
				case MemberName.CaseSensitivity:
					m_caseSensitivity = (DataSet.TriState)reader.ReadEnum();
					break;
				case MemberName.AccentSensitivity:
					m_accentSensitivity = (DataSet.TriState)reader.ReadEnum();
					break;
				case MemberName.KanatypeSensitivity:
					m_kanatypeSensitivity = (DataSet.TriState)reader.ReadEnum();
					break;
				case MemberName.WidthSensitivity:
					m_widthSensitivity = (DataSet.TriState)reader.ReadEnum();
					break;
				case MemberName.Filters:
					m_filters = reader.ReadGenericListOfRIFObjects<Filter>();
					break;
				case MemberName.InterpretSubtotalsAsDetails:
					m_interpretSubtotalsAsDetails = (DataSet.TriState)reader.ReadEnum();
					break;
				case MemberName.CatalogID:
					m_catalogID = reader.ReadGuid();
					break;
				case MemberName.NonCalculatedFieldCount:
					m_nonCalculatedFieldCount = reader.ReadInt32();
					break;
				case MemberName.CompiledCode:
					m_compiledCode = reader.ReadByteArray();
					break;
				case MemberName.CompiledCodeGeneratedWithRefusedPermissions:
					m_compiledCodeGeneratedWithRefusedPermissions = reader.ReadBoolean();
					break;
				case MemberName.ExprHostID:
					m_exprHostID = reader.ReadInt32();
					break;
				case MemberName.ExprHostAssemblyID:
					m_exprHostAssemblyId = reader.ReadGuid();
					break;
				case MemberName.NullsAsBlanks:
					m_nullsAsBlanks = reader.ReadBoolean();
					break;
				default:
					Global.Tracer.Assert(condition: false, string.Empty);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false, string.Empty);
		}

		public Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataSetCore;
		}
	}
}
