using Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel;
using Microsoft.ReportingServices.ReportProcessing.Persistence;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class DataSet : IDOwner, IAggregateHolder, ISortFilterScope
	{
		internal enum Sensitivity
		{
			Auto,
			True,
			False
		}

		internal const uint CompareFlag_Default = 0u;

		internal const uint CompareFlag_IgnoreCase = 1u;

		internal const uint CompareFlag_IgnoreNonSpace = 2u;

		internal const uint CompareFlag_IgnoreKanatype = 65536u;

		internal const uint CompareFlag_IgnoreWidth = 131072u;

		private string m_name;

		private DataFieldList m_fields;

		private ReportQuery m_query;

		private Sensitivity m_caseSensitivity;

		private string m_collation;

		private Sensitivity m_accentSensitivity;

		private Sensitivity m_kanatypeSensitivity;

		private Sensitivity m_widthSensitivity;

		private DataRegionList m_dataRegions;

		private DataAggregateInfoList m_aggregates;

		private FilterList m_filters;

		private bool m_usedOnlyInParameters;

		private int m_nonCalculatedFieldCount = -1;

		private int m_exprHostID = -1;

		private DataAggregateInfoList m_postSortAggregates;

		private bool m_hasDetailUserSortFilter;

		private ExpressionInfoList m_userSortExpressions;

		private bool m_dynamicFieldReferences;

		private bool? m_interpretSubtotalsAsDetails;

		private int m_recordSetSize = -1;

		private uint m_lcid = DataSetValidator.LOCALE_SYSTEM_DEFAULT;

		[NonSerialized]
		private DataSetExprHost m_exprHost;

		[NonSerialized]
		private string m_autoDetectedCollation;

		[NonSerialized]
		private bool[] m_isSortFilterTarget;

		[NonSerialized]
		private Hashtable m_referencedFieldProperties;

		[NonSerialized]
		private bool m_usedInAggregates;

		internal ObjectType ObjectType => ObjectType.DataSet;

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

		internal DataFieldList Fields
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

		internal Sensitivity CaseSensitivity
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

		internal Sensitivity AccentSensitivity
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

		internal Sensitivity KanatypeSensitivity
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

		internal Sensitivity WidthSensitivity
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

		internal DataRegionList DataRegions
		{
			get
			{
				return m_dataRegions;
			}
			set
			{
				m_dataRegions = value;
			}
		}

		internal DataAggregateInfoList Aggregates
		{
			get
			{
				return m_aggregates;
			}
			set
			{
				m_aggregates = value;
			}
		}

		internal FilterList Filters
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

		internal bool UsedOnlyInParameters
		{
			get
			{
				return m_usedOnlyInParameters;
			}
			set
			{
				m_usedOnlyInParameters = value;
			}
		}

		internal int NonCalculatedFieldCount
		{
			get
			{
				return m_nonCalculatedFieldCount;
			}
			set
			{
				m_nonCalculatedFieldCount = value;
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

		internal DataAggregateInfoList PostSortAggregates
		{
			get
			{
				return m_postSortAggregates;
			}
			set
			{
				m_postSortAggregates = value;
			}
		}

		internal int RecordSetSize
		{
			get
			{
				return m_recordSetSize;
			}
			set
			{
				m_recordSetSize = value;
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

		internal bool HasDetailUserSortFilter
		{
			get
			{
				return m_hasDetailUserSortFilter;
			}
			set
			{
				m_hasDetailUserSortFilter = value;
			}
		}

		internal ExpressionInfoList UserSortExpressions
		{
			get
			{
				return m_userSortExpressions;
			}
			set
			{
				m_userSortExpressions = value;
			}
		}

		internal DataSetExprHost ExprHost => m_exprHost;

		internal string AutoDetectedCollation
		{
			get
			{
				return m_autoDetectedCollation;
			}
			set
			{
				m_autoDetectedCollation = value;
			}
		}

		internal bool[] IsSortFilterTarget
		{
			get
			{
				return m_isSortFilterTarget;
			}
			set
			{
				m_isSortFilterTarget = value;
			}
		}

		int ISortFilterScope.ID => m_ID;

		string ISortFilterScope.ScopeName => m_name;

		bool[] ISortFilterScope.IsSortFilterTarget
		{
			get
			{
				return m_isSortFilterTarget;
			}
			set
			{
				m_isSortFilterTarget = value;
			}
		}

		bool[] ISortFilterScope.IsSortFilterExpressionScope
		{
			get
			{
				return null;
			}
			set
			{
				Global.Tracer.Assert(condition: false, string.Empty);
			}
		}

		ExpressionInfoList ISortFilterScope.UserSortExpressions
		{
			get
			{
				return m_userSortExpressions;
			}
			set
			{
				m_userSortExpressions = value;
			}
		}

		IndexedExprHost ISortFilterScope.UserSortExpressionsHost
		{
			get
			{
				if (m_exprHost == null)
				{
					return null;
				}
				return m_exprHost.UserSortExpressionsHost;
			}
		}

		internal bool DynamicFieldReferences
		{
			get
			{
				return m_dynamicFieldReferences;
			}
			set
			{
				m_dynamicFieldReferences = value;
			}
		}

		internal bool UsedInAggregates
		{
			get
			{
				return m_usedInAggregates;
			}
			set
			{
				m_usedInAggregates = value;
			}
		}

		internal bool InterpretSubtotalsAsDetailsIsAuto => !m_interpretSubtotalsAsDetails.HasValue;

		internal bool InterpretSubtotalsAsDetails
		{
			get
			{
				if (m_interpretSubtotalsAsDetails.HasValue)
				{
					return m_interpretSubtotalsAsDetails.Value;
				}
				return false;
			}
			set
			{
				m_interpretSubtotalsAsDetails = value;
			}
		}

		internal DataSet(int id)
			: base(id)
		{
			m_fields = new DataFieldList();
			m_dataRegions = new DataRegionList();
			m_aggregates = new DataAggregateInfoList();
			m_postSortAggregates = new DataAggregateInfoList();
		}

		internal DataSet()
		{
		}

		internal void Initialize(InitializationContext context)
		{
			context.ObjectType = ObjectType;
			context.ObjectName = m_name;
			context.RegisterDataSet(this);
			InternalInitialize(context);
			context.UnRegisterDataSet(this);
		}

		private void InternalInitialize(InitializationContext context)
		{
			context.ExprHostBuilder.DataSetStart(m_name);
			context.Location |= LocationFlags.InDataSet;
			if (m_query != null)
			{
				m_query.Initialize(context);
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
			if (m_userSortExpressions != null)
			{
				context.ExprHostBuilder.UserSortExpressionsStart();
				for (int k = 0; k < m_userSortExpressions.Count; k++)
				{
					ExpressionInfo expression = m_userSortExpressions[k];
					context.ExprHostBuilder.UserSortExpression(expression);
				}
				context.ExprHostBuilder.UserSortExpressionsEnd();
			}
			m_exprHostID = context.ExprHostBuilder.DataSetEnd();
		}

		DataAggregateInfoList[] IAggregateHolder.GetAggregateLists()
		{
			return new DataAggregateInfoList[1]
			{
				m_aggregates
			};
		}

		DataAggregateInfoList[] IAggregateHolder.GetPostSortAggregateLists()
		{
			return new DataAggregateInfoList[1]
			{
				m_postSortAggregates
			};
		}

		void IAggregateHolder.ClearIfEmpty()
		{
			Global.Tracer.Assert(m_aggregates != null);
			if (m_aggregates.Count == 0)
			{
				m_aggregates = null;
			}
			Global.Tracer.Assert(m_postSortAggregates != null);
			if (m_postSortAggregates.Count == 0)
			{
				m_postSortAggregates = null;
			}
		}

		internal void CheckNonCalculatedFieldCount()
		{
			if (m_nonCalculatedFieldCount < 0 && m_fields != null)
			{
				int i;
				for (i = 0; i < m_fields.Count && !m_fields[i].IsCalculatedField; i++)
				{
				}
				m_nonCalculatedFieldCount = i;
			}
		}

		internal void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			if (ExprHostID >= 0)
			{
				Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null);
				m_exprHost = reportExprHost.DataSetHostsRemotable[ExprHostID];
				m_exprHost.SetReportObjectModel(reportObjectModel);
				if (m_exprHost.QueryParametersHost != null)
				{
					m_query.SetExprHost(m_exprHost.QueryParametersHost, reportObjectModel);
				}
				if (m_exprHost.UserSortExpressionsHost != null)
				{
					m_exprHost.UserSortExpressionsHost.SetReportObjectModel(reportObjectModel);
				}
			}
		}

		internal bool NeedAutoDetectCollation()
		{
			if (DataSetValidator.LOCALE_SYSTEM_DEFAULT != m_lcid && m_accentSensitivity != 0 && m_caseSensitivity != 0 && m_kanatypeSensitivity != 0)
			{
				return m_widthSensitivity == Sensitivity.Auto;
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
					errorContext.Register(ProcessingErrorCode.rsInvalidCollationCultureName, Severity.Warning, ObjectType.DataSet, m_name, dataSourceType, cultureName);
				}
			}
			if (DataSetValidator.LOCALE_SYSTEM_DEFAULT == m_lcid)
			{
				m_lcid = lcid;
			}
			MergeSensitivity(ref m_accentSensitivity, accentSensitive);
			MergeSensitivity(ref m_caseSensitivity, caseSensitive);
			MergeSensitivity(ref m_kanatypeSensitivity, kanatypeSensitive);
			MergeSensitivity(ref m_widthSensitivity, widthSensitive);
		}

		private void MergeSensitivity(ref Sensitivity current, bool detectedValue)
		{
			if (current == Sensitivity.Auto)
			{
				if (detectedValue)
				{
					current = Sensitivity.True;
				}
				else
				{
					current = Sensitivity.False;
				}
			}
		}

		internal uint GetSQLSortCompareFlags()
		{
			return DataSetValidator.GetSQLSortCompareMask(Sensitivity.True == m_caseSensitivity, Sensitivity.True == m_accentSensitivity, Sensitivity.True == m_kanatypeSensitivity, Sensitivity.True == m_widthSensitivity);
		}

		internal CompareOptions GetCLRCompareOptions()
		{
			CompareOptions compareOptions = CompareOptions.None;
			if (Sensitivity.True != m_caseSensitivity)
			{
				compareOptions |= CompareOptions.IgnoreCase;
			}
			if (Sensitivity.True != m_accentSensitivity)
			{
				compareOptions |= CompareOptions.IgnoreNonSpace;
			}
			if (Sensitivity.True != m_kanatypeSensitivity)
			{
				compareOptions |= CompareOptions.IgnoreKanaType;
			}
			if (Sensitivity.True != m_widthSensitivity)
			{
				compareOptions |= CompareOptions.IgnoreWidth;
			}
			return compareOptions;
		}

		internal void MergeFieldProperties(ExpressionInfo expressionInfo)
		{
			if (m_dynamicFieldReferences)
			{
				return;
			}
			Global.Tracer.Assert(expressionInfo != null);
			if (expressionInfo.DynamicFieldReferences)
			{
				m_dynamicFieldReferences = true;
				m_referencedFieldProperties = null;
			}
			else
			{
				if (expressionInfo.ReferencedFieldProperties == null || expressionInfo.ReferencedFieldProperties.Count == 0)
				{
					return;
				}
				if (m_referencedFieldProperties == null)
				{
					m_referencedFieldProperties = new Hashtable();
				}
				IDictionaryEnumerator enumerator = expressionInfo.ReferencedFieldProperties.GetEnumerator();
				while (enumerator.MoveNext())
				{
					string key = enumerator.Entry.Key as string;
					bool flag = m_referencedFieldProperties.ContainsKey(key);
					Hashtable hashtable = m_referencedFieldProperties[key] as Hashtable;
					if (flag && hashtable == null)
					{
						continue;
					}
					Hashtable hashtable2 = enumerator.Entry.Value as Hashtable;
					if (!flag)
					{
						m_referencedFieldProperties.Add(key, hashtable2);
						continue;
					}
					if (hashtable2 == null)
					{
						m_referencedFieldProperties[key] = null;
						continue;
					}
					Global.Tracer.Assert(hashtable != null && hashtable2 != null);
					IEnumerator enumerator2 = hashtable2.Keys.GetEnumerator();
					while (enumerator2.MoveNext())
					{
						string key2 = enumerator2.Current as string;
						if (!hashtable.ContainsKey(key2))
						{
							hashtable.Add(key2, null);
						}
					}
				}
			}
		}

		internal void PopulateReferencedFieldProperties()
		{
			if (m_dynamicFieldReferences || m_fields == null || m_referencedFieldProperties == null)
			{
				return;
			}
			for (int i = 0; i < m_fields.Count; i++)
			{
				Field field = m_fields[i];
				if (!m_referencedFieldProperties.ContainsKey(field.Name))
				{
					continue;
				}
				Hashtable hashtable = m_referencedFieldProperties[field.Name] as Hashtable;
				if (hashtable == null)
				{
					field.DynamicPropertyReferences = true;
					continue;
				}
				IEnumerator enumerator = hashtable.Keys.GetEnumerator();
				FieldPropertyHashtable fieldPropertyHashtable = new FieldPropertyHashtable(hashtable.Count);
				while (enumerator.MoveNext())
				{
					fieldPropertyHashtable.Add(enumerator.Current as string);
				}
				field.ReferencedProperties = fieldPropertyHashtable;
			}
		}

		internal bool IsShareable()
		{
			if (m_query == null || m_query.CommandText == null)
			{
				return true;
			}
			if (ExpressionInfo.Types.Constant == m_query.CommandText.Type)
			{
				if (m_query.Parameters == null)
				{
					return true;
				}
				int count = m_query.Parameters.Count;
				for (int i = 0; i < count; i++)
				{
					ExpressionInfo value = m_query.Parameters[i].Value;
					if (value != null && ExpressionInfo.Types.Constant != value.Type)
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Name, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.Fields, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.DataFieldList));
			memberInfoList.Add(new MemberInfo(MemberName.Query, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.CaseSensitivity, Token.Enum));
			memberInfoList.Add(new MemberInfo(MemberName.Collation, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.AccentSensitivity, Token.Enum));
			memberInfoList.Add(new MemberInfo(MemberName.KanatypeSensitivity, Token.Enum));
			memberInfoList.Add(new MemberInfo(MemberName.WidthSensitivity, Token.Enum));
			memberInfoList.Add(new MemberInfo(MemberName.DataRegions, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.DataRegionList));
			memberInfoList.Add(new MemberInfo(MemberName.Aggregates, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.DataAggregateInfoList));
			memberInfoList.Add(new MemberInfo(MemberName.Filters, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.FilterList));
			memberInfoList.Add(new MemberInfo(MemberName.RecordSetSize, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.UsedOnlyInParameters, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.NonCalculatedFieldCount, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.PostSortAggregates, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.DataAggregateInfoList));
			memberInfoList.Add(new MemberInfo(MemberName.LCID, Token.UInt32));
			memberInfoList.Add(new MemberInfo(MemberName.HasDetailUserSortFilter, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.UserSortExpressions, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfoList));
			memberInfoList.Add(new MemberInfo(MemberName.DynamicFieldReferences, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.InterpretSubtotalsAsDetails, Token.Boolean));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.IDOwner, memberInfoList);
		}
	}
}
