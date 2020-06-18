using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal class SubReportInfo : IPersistable
	{
		internal class ParametersImplValuesComparer : IEqualityComparer<ParametersImplWrapper>
		{
			internal static readonly ParametersImplValuesComparer Instance = new ParametersImplValuesComparer();

			private ParametersImplValuesComparer()
			{
			}

			public bool Equals(ParametersImplWrapper obj1, ParametersImplWrapper obj2)
			{
				if (obj1 == obj2)
				{
					return true;
				}
				if (obj1 == null || obj2 == null)
				{
					return false;
				}
				return obj1.ValuesAreEqual(obj2);
			}

			public int GetHashCode(ParametersImplWrapper obj)
			{
				return obj.GetValuesHashCode();
			}
		}

		private int m_lastID;

		private string m_uniqueName;

		[NonSerialized]
		private Dictionary<ParametersImplWrapper, int> m_parameterValuesToInfoIndexMap;

		private List<ParametersImplWrapper> m_instanceParameterValues;

		private int m_userSortDataSetGlobalId = -1;

		[NonSerialized]
		private CommonSubReportInfo m_commonSubReportInfo;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal CommonSubReportInfo CommonSubReportInfo
		{
			get
			{
				return m_commonSubReportInfo;
			}
			set
			{
				m_commonSubReportInfo = value;
			}
		}

		internal string UniqueName => m_uniqueName;

		internal int LastID
		{
			get
			{
				return m_lastID;
			}
			set
			{
				m_lastID = value;
			}
		}

		internal int UserSortDataSetGlobalId
		{
			get
			{
				return m_userSortDataSetGlobalId;
			}
			set
			{
				m_userSortDataSetGlobalId = value;
			}
		}

		internal SubReportInfo()
		{
		}

		internal SubReportInfo(Guid uniqueName)
		{
			m_uniqueName = uniqueName.ToString("N");
		}

		internal int GetChunkNameModifierForParamValues(ParametersImpl parameterValues, bool addEntry, ref bool? isShared, out ParametersImpl fullParameterValues)
		{
			if (parameterValues == null)
			{
				parameterValues = new ParametersImpl();
			}
			if (m_parameterValuesToInfoIndexMap == null)
			{
				m_instanceParameterValues = new List<ParametersImplWrapper>();
				m_parameterValuesToInfoIndexMap = new Dictionary<ParametersImplWrapper, int>(ParametersImplValuesComparer.Instance);
			}
			ParametersImplWrapper parametersImplWrapper = new ParametersImplWrapper(parameterValues);
			if (m_parameterValuesToInfoIndexMap.TryGetValue(parametersImplWrapper, out int value))
			{
				fullParameterValues = m_instanceParameterValues[value].WrappedParametersImpl;
				if (!isShared.HasValue)
				{
					isShared = true;
				}
			}
			else
			{
				isShared = false;
				fullParameterValues = parameterValues;
				if (addEntry)
				{
					value = m_instanceParameterValues.Count;
					m_instanceParameterValues.Add(parametersImplWrapper);
					m_parameterValuesToInfoIndexMap.Add(parametersImplWrapper, value);
				}
			}
			return value;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.LastID, Token.Int32));
			list.Add(new MemberInfo(MemberName.UniqueName, Token.String));
			list.Add(new MemberInfo(MemberName.InstanceParameterValues, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Parameters));
			list.Add(new MemberInfo(MemberName.DataSetID, Token.Int32));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SubReportInfo, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.LastID:
					writer.Write(m_lastID);
					break;
				case MemberName.UniqueName:
					writer.Write(m_uniqueName);
					break;
				case MemberName.InstanceParameterValues:
					writer.Write(m_instanceParameterValues);
					break;
				case MemberName.DataSetID:
					writer.Write(m_userSortDataSetGlobalId);
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
				case MemberName.LastID:
					m_lastID = reader.ReadInt32();
					break;
				case MemberName.UniqueName:
					m_uniqueName = reader.ReadString();
					break;
				case MemberName.InstanceParameterValues:
					m_instanceParameterValues = reader.ReadListOfRIFObjects<List<ParametersImplWrapper>>();
					break;
				case MemberName.DataSetID:
					m_userSortDataSetGlobalId = reader.ReadInt32();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
			if (m_instanceParameterValues != null)
			{
				m_parameterValuesToInfoIndexMap = new Dictionary<ParametersImplWrapper, int>(ParametersImplValuesComparer.Instance);
				for (int i = 0; i < m_instanceParameterValues.Count; i++)
				{
					m_parameterValuesToInfoIndexMap[m_instanceParameterValues[i]] = i;
				}
			}
		}

		public void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false);
		}

		public Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SubReportInfo;
		}
	}
}
