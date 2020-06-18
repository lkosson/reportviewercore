using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal class DataAggregateObjResult : IStorable, IPersistable
	{
		private struct CloneHelperStruct
		{
			internal object Value;

			internal CloneHelperStruct(object value)
			{
				Value = value;
			}
		}

		internal bool ErrorOccurred;

		internal object Value;

		internal bool HasCode;

		internal ProcessingErrorCode Code;

		internal Severity Severity;

		internal string[] Arguments;

		internal DataFieldStatus FieldStatus;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		public int Size => 1 + ItemSizes.SizeOf(Value) + 1 + 4 + 4 + ItemSizes.SizeOf(Arguments) + 4;

		internal DataAggregateObjResult()
		{
		}

		internal DataAggregateObjResult(DataAggregateObjResult original)
		{
			ErrorOccurred = original.ErrorOccurred;
			HasCode = original.HasCode;
			Code = original.Code;
			Severity = original.Severity;
			FieldStatus = original.FieldStatus;
			CloneHelperStruct cloneHelperStruct = new CloneHelperStruct(original.Value);
			Value = cloneHelperStruct.Value;
			Arguments = original.Arguments;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.ErrorOccurred, Token.Boolean));
			list.Add(new MemberInfo(MemberName.Value, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Variant));
			list.Add(new MemberInfo(MemberName.HasCode, Token.Boolean));
			list.Add(new MemberInfo(MemberName.Code, Token.Enum));
			list.Add(new MemberInfo(MemberName.Severity, Token.Enum));
			list.Add(new MemberInfo(MemberName.FieldStatus, Token.Enum));
			list.Add(new MemberInfo(MemberName.Arguments, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveArray, Token.String));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateObjResult, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public virtual void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.ErrorOccurred:
					writer.Write(ErrorOccurred);
					break;
				case MemberName.Value:
					writer.Write(Value);
					break;
				case MemberName.HasCode:
					writer.Write(HasCode);
					break;
				case MemberName.Code:
					writer.WriteEnum((int)Code);
					break;
				case MemberName.Severity:
					writer.WriteEnum((int)Severity);
					break;
				case MemberName.FieldStatus:
					writer.WriteEnum((int)FieldStatus);
					break;
				case MemberName.Arguments:
					writer.Write(Arguments);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public virtual void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.ErrorOccurred:
					ErrorOccurred = reader.ReadBoolean();
					break;
				case MemberName.Value:
					Value = reader.ReadVariant();
					break;
				case MemberName.HasCode:
					HasCode = reader.ReadBoolean();
					break;
				case MemberName.Code:
					Code = (ProcessingErrorCode)reader.ReadEnum();
					break;
				case MemberName.Severity:
					Severity = (Severity)reader.ReadEnum();
					break;
				case MemberName.FieldStatus:
					FieldStatus = (DataFieldStatus)reader.ReadEnum();
					break;
				case MemberName.Arguments:
					Arguments = reader.ReadStringArray();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public virtual void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false);
		}

		public virtual Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateObjResult;
		}
	}
}
