using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ParameterGridLayoutCellDefinition : IPersistable
	{
		public int RowIndex;

		public int ColumnIndex;

		public string ParameterName;

		private static Declaration m_Declaration = GetDeclaration();

		public void WriteXml(XmlTextWriter resultXml)
		{
			resultXml.WriteStartElement("CellDefinition");
			resultXml.WriteElementString("ColumnIndex", ColumnIndex.ToString(CultureInfo.InvariantCulture));
			resultXml.WriteElementString("RowIndex", RowIndex.ToString(CultureInfo.InvariantCulture));
			resultXml.WriteElementString("ParameterName", ParameterName);
			resultXml.WriteEndElement();
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.ParameterCellColumnIndex, Token.Int32, Lifetime.AddedIn(300)));
			list.Add(new MemberInfo(MemberName.ParameterCellRowIndex, Token.Int32, Lifetime.AddedIn(300)));
			list.Add(new MemberInfo(MemberName.ParameterName, Token.String, Lifetime.AddedIn(300)));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterGridLayoutCellDefinition, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.ParameterCellColumnIndex:
					writer.Write(ColumnIndex);
					break;
				case MemberName.ParameterCellRowIndex:
					writer.Write(RowIndex);
					break;
				case MemberName.ParameterName:
					writer.Write(ParameterName);
					break;
				default:
					Global.Tracer.Assert(condition: false, "Unexpected RIF Member for ParametersGridCellDefinition");
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
				case MemberName.ParameterCellColumnIndex:
					ColumnIndex = reader.ReadInt32();
					break;
				case MemberName.ParameterCellRowIndex:
					RowIndex = reader.ReadInt32();
					break;
				case MemberName.ParameterName:
					ParameterName = reader.ReadString();
					break;
				default:
					Global.Tracer.Assert(condition: false, "Unexpected RIF Member for ParametersGridCellDefinition");
					break;
				}
			}
		}

		public Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterGridLayoutCellDefinition;
		}

		public void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			throw new NotImplementedException();
		}
	}
}
