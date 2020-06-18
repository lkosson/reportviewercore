using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal sealed class Previous : DataAggregate
	{
		private class ListCloner
		{
			private ListCloner()
			{
			}

			internal static List<object> CloneList(List<object> key, int startIndex)
			{
				if (key == null)
				{
					return null;
				}
				int num = key.Count - startIndex;
				List<object> list = new List<object>(Math.Max(0, num));
				for (int i = 0; i < num; i++)
				{
					list.Add(key[i + startIndex]);
				}
				return list;
			}
		}

		internal class ListOfObjectsEqualityComparer : IEqualityComparer<List<object>>
		{
			internal static readonly ListOfObjectsEqualityComparer Instance = new ListOfObjectsEqualityComparer();

			private ListOfObjectsEqualityComparer()
			{
			}

			public bool Equals(List<object> x, List<object> y)
			{
				if (x == null && y == null)
				{
					return true;
				}
				if (x == null != (y == null) || x.Count != y.Count)
				{
					return false;
				}
				for (int num = x.Count - 1; num >= 0; num--)
				{
					if (!x[num].Equals(y[num]))
					{
						return false;
					}
				}
				return true;
			}

			public int GetHashCode(List<object> obj)
			{
				int count = obj.Count;
				int num = count << 24;
				if (count > 0)
				{
					num ^= obj[count - 1].GetHashCode();
				}
				return num;
			}
		}

		[StaticReference]
		private OnDemandProcessingContext m_odpContext;

		private Dictionary<List<object>, object> m_previousValues;

		private Dictionary<List<object>, object> m_values;

		private int m_startIndex;

		private bool m_isScopedInEvaluationScope;

		private object m_previous;

		private bool m_previousEnabled;

		private bool m_hasNoExplicitScope;

		private object m_value;

		private static Declaration m_declaration = GetDeclaration();

		internal override DataAggregateInfo.AggregateTypes AggregateType => DataAggregateInfo.AggregateTypes.Previous;

		public override int Size => ItemSizes.ReferenceSize + ItemSizes.SizeOf(m_previousValues) + ItemSizes.SizeOf(m_values) + 4 + 1 + ItemSizes.SizeOf(m_previous) + 1 + 1 + ItemSizes.SizeOf(m_value);

		internal Previous()
		{
		}

		internal Previous(OnDemandProcessingContext odpContext, int startIndex, bool isScopedInEvaluationScope, bool hasNoExplicitScope)
		{
			m_odpContext = odpContext;
			m_isScopedInEvaluationScope = isScopedInEvaluationScope;
			m_hasNoExplicitScope = hasNoExplicitScope;
			m_startIndex = startIndex;
		}

		internal override void Init()
		{
			if (!m_isScopedInEvaluationScope && !m_previousEnabled)
			{
				m_previousValues = m_values;
				m_values = new Dictionary<List<object>, object>(ListOfObjectsEqualityComparer.Instance);
			}
			m_previousEnabled = true;
		}

		internal override void Update(object[] expressions, IErrorContext iErrorContext)
		{
			if (m_isScopedInEvaluationScope)
			{
				if (m_previousEnabled || m_hasNoExplicitScope)
				{
					m_previous = m_value;
				}
				m_value = expressions[0];
			}
			else
			{
				List<object> key = ListCloner.CloneList(m_odpContext.GroupExpressionValues, m_startIndex);
				if (m_previousValues != null)
				{
					m_previousValues.TryGetValue(key, out m_previous);
				}
				m_values[key] = expressions[0];
			}
			m_previousEnabled = false;
		}

		internal override object Result()
		{
			return m_previous;
		}

		internal override DataAggregate ConstructAggregator(OnDemandProcessingContext odpContext, DataAggregateInfo aggregateDef)
		{
			RunningValueInfo runningValueInfo = (RunningValueInfo)aggregateDef;
			return new Previous(odpContext, runningValueInfo.TotalGroupingExpressionCount, runningValueInfo.IsScopedInEvaluationScope, string.IsNullOrEmpty(runningValueInfo.Scope));
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_declaration);
			IScalabilityCache scalabilityCache = writer.PersistenceHelper as IScalabilityCache;
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.OdpContext:
				{
					int value = scalabilityCache.StoreStaticReference(m_odpContext);
					writer.Write(value);
					break;
				}
				case MemberName.PreviousValues:
					writer.WriteVariantListVariantDictionary(m_previousValues);
					break;
				case MemberName.Values:
					writer.WriteVariantListVariantDictionary(m_values);
					break;
				case MemberName.StartHidden:
					writer.Write(m_startIndex);
					break;
				case MemberName.IsScopedInEvaluationScope:
					writer.Write(m_isScopedInEvaluationScope);
					break;
				case MemberName.Previous:
					writer.Write(m_previous);
					break;
				case MemberName.PreviousEnabled:
					writer.Write(m_previousEnabled);
					break;
				case MemberName.HasNoExplicitScope:
					writer.Write(m_hasNoExplicitScope);
					break;
				case MemberName.Value:
					writer.Write(m_value);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_declaration);
			IScalabilityCache scalabilityCache = reader.PersistenceHelper as IScalabilityCache;
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.OdpContext:
				{
					int id = reader.ReadInt32();
					m_odpContext = (OnDemandProcessingContext)scalabilityCache.FetchStaticReference(id);
					break;
				}
				case MemberName.PreviousValues:
					m_previousValues = reader.ReadVariantListVariantDictionary();
					break;
				case MemberName.Values:
					m_values = reader.ReadVariantListVariantDictionary();
					break;
				case MemberName.StartHidden:
					m_startIndex = reader.ReadInt32();
					break;
				case MemberName.IsScopedInEvaluationScope:
					m_isScopedInEvaluationScope = reader.ReadBoolean();
					break;
				case MemberName.Previous:
					m_previous = reader.ReadVariant();
					break;
				case MemberName.PreviousEnabled:
					m_previousEnabled = reader.ReadBoolean();
					break;
				case MemberName.HasNoExplicitScope:
					m_hasNoExplicitScope = reader.ReadBoolean();
					break;
				case MemberName.Value:
					m_value = reader.ReadVariant();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Previous;
		}

		public static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.OdpContext, Token.Int32));
				list.Add(new MemberInfo(MemberName.PreviousValues, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.VariantListVariantDictionary));
				list.Add(new MemberInfo(MemberName.Values, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.VariantListVariantDictionary));
				list.Add(new MemberInfo(MemberName.StartIndex, Token.Int32));
				list.Add(new MemberInfo(MemberName.IsScopedInEvaluationScope, Token.Boolean));
				list.Add(new MemberInfo(MemberName.Previous, Token.Object));
				list.Add(new MemberInfo(MemberName.PreviousEnabled, Token.Boolean));
				list.Add(new MemberInfo(MemberName.HasNoExplicitScope, Token.Boolean));
				list.Add(new MemberInfo(MemberName.Value, Token.Object));
				return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Previous, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return m_declaration;
		}
	}
}
