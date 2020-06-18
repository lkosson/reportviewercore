using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ParametersGridLayout : IPersistable
	{
		public int NumberOfColumns;

		public int NumberOfRows;

		public ParametersGridCellDefinitionList CellDefinitions;

		private static Declaration m_Declaration = GetDeclaration();

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.ParametersLayoutNumberOfColumns, Token.Int32, Lifetime.AddedIn(300)));
			list.Add(new MemberInfo(MemberName.ParametersLayoutNumberOfRows, Token.Int32, Lifetime.AddedIn(300)));
			list.Add(new MemberInfo(MemberName.ParametersLayoutCellDefinitions, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterGridLayoutCellDefinition, Lifetime.AddedIn(300)));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParametersLayout, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.ParametersLayoutNumberOfColumns:
					writer.Write(NumberOfColumns);
					break;
				case MemberName.ParametersLayoutNumberOfRows:
					writer.Write(NumberOfRows);
					break;
				case MemberName.ParametersLayoutCellDefinitions:
					writer.Write(CellDefinitions);
					break;
				default:
					Global.Tracer.Assert(condition: false, "Unexpected RIF Member for ParametersGridLayout");
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
				case MemberName.ParametersLayoutNumberOfColumns:
					NumberOfColumns = reader.ReadInt32();
					break;
				case MemberName.ParametersLayoutNumberOfRows:
					NumberOfRows = reader.ReadInt32();
					break;
				case MemberName.ParametersLayoutCellDefinitions:
					CellDefinitions = reader.ReadListOfRIFObjects<ParametersGridCellDefinitionList>();
					break;
				default:
					Global.Tracer.Assert(condition: false, "Unexpected RIF Member for ParametersGridLayout");
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			throw new NotImplementedException();
		}

		public Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParametersLayout;
		}
	}
}
