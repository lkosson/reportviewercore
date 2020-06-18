using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal class Aggregate : DataAggregate
	{
		private object m_value;

		private static Declaration m_declaration = GetDeclaration();

		internal override DataAggregateInfo.AggregateTypes AggregateType => DataAggregateInfo.AggregateTypes.Aggregate;

		public override int Size => ItemSizes.SizeOf(m_value);

		internal override void Init()
		{
		}

		internal override void Update(object[] expressions, IErrorContext iErrorContext)
		{
			m_value = expressions[0];
		}

		internal override object Result()
		{
			return m_value;
		}

		internal override DataAggregate ConstructAggregator(OnDemandProcessingContext odpContext, DataAggregateInfo aggregateDef)
		{
			return new Aggregate();
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.Value)
				{
					writer.Write(m_value);
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				if (memberName == MemberName.Value)
				{
					m_value = reader.ReadVariant();
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public override void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false, "Aggregate should not resolve references");
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Aggregate;
		}

		internal static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.Value, Token.Object));
				return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Aggregate, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return m_declaration;
		}
	}
}
