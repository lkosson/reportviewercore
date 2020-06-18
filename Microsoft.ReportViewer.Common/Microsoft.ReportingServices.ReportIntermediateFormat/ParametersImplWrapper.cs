using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[SkipStaticValidation]
	internal class ParametersImplWrapper : IPersistable
	{
		private ParametersImpl m_opdParameters;

		[NonSerialized]
		private int m_hash;

		private static readonly Declaration m_Declaration = GetDeclaration();

		internal ParametersImpl WrappedParametersImpl => m_opdParameters;

		internal ParametersImplWrapper()
		{
			m_opdParameters = new ParametersImpl();
		}

		internal ParametersImplWrapper(ParametersImpl odpParameters)
		{
			m_opdParameters = odpParameters;
		}

		internal bool ValuesAreEqual(ParametersImplWrapper obj)
		{
			ParameterImpl[] collection = m_opdParameters.Collection;
			ParameterImpl[] collection2 = obj.WrappedParametersImpl.Collection;
			if (collection == null)
			{
				if (collection2 == null)
				{
					return true;
				}
				return false;
			}
			if (collection2 == null)
			{
				return false;
			}
			if (collection.Length != collection2.Length)
			{
				return false;
			}
			for (int i = 0; i < collection.Length; i++)
			{
				if (!collection[i].ValuesAreEqual(collection2[i]))
				{
					return false;
				}
			}
			return true;
		}

		internal int GetValuesHashCode()
		{
			ParameterImpl[] collection = m_opdParameters.Collection;
			if (m_hash == 0)
			{
				m_hash = 4051;
				if (collection != null)
				{
					m_hash |= collection.Length << 16;
					for (int i = 0; i < collection.Length; i++)
					{
						m_hash ^= collection[i].GetValuesHashCode();
					}
				}
			}
			return m_hash;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Parameters, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectArray, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Parameter));
			list.Add(new MemberInfo(MemberName.Names, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StringInt32Hashtable, Token.Int32));
			list.Add(new MemberInfo(MemberName.Count, Token.Int32));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Parameters, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Parameters:
				{
					ParameterImplWrapper[] array = null;
					if (m_opdParameters.Collection != null)
					{
						array = new ParameterImplWrapper[m_opdParameters.Collection.Length];
						for (int i = 0; i < array.Length; i++)
						{
							if (m_opdParameters.Collection[i] != null)
							{
								array[i] = new ParameterImplWrapper(m_opdParameters.Collection[i]);
							}
						}
					}
					writer.Write(array);
					break;
				}
				case MemberName.Names:
					writer.WriteStringInt32Hashtable(m_opdParameters.NameMap);
					break;
				case MemberName.Count:
					writer.Write(m_opdParameters.Count);
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
				case MemberName.Parameters:
				{
					ParameterImplWrapper[] array = reader.ReadArrayOfRIFObjects<ParameterImplWrapper>();
					if (array == null)
					{
						break;
					}
					m_opdParameters.Collection = new ParameterImpl[array.Length];
					for (int i = 0; i < array.Length; i++)
					{
						if (array[i] != null)
						{
							m_opdParameters.Collection[i] = array[i].WrappedParameterImpl;
						}
					}
					break;
				}
				case MemberName.Names:
					m_opdParameters.NameMap = reader.ReadStringInt32Hashtable<Hashtable>();
					break;
				case MemberName.Count:
					m_opdParameters.Count = reader.ReadInt32();
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
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Parameters;
		}
	}
}
