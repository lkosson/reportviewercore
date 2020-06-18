using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models
{
	internal sealed class OpenXmlTableGridModel : BaseInterleaver
	{
		private List<int> _columns = new List<int>();

		private static readonly Declaration _declaration;

		public override int Size => base.Size + ItemSizes.SizeOf(_columns);

		public OpenXmlTableGridModel(int index, long location)
			: base(index, location)
		{
		}

		public OpenXmlTableGridModel()
		{
		}

		static OpenXmlTableGridModel()
		{
			_declaration = new Declaration(ObjectType.WordOpenXmlTableGrid, ObjectType.WordOpenXmlBaseInterleaver, new List<MemberInfo>
			{
				new MemberInfo(MemberName.Columns, ObjectType.PrimitiveList, Token.Int32)
			});
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(GetDeclaration());
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.Columns)
				{
					writer.WriteListOfPrimitives(_columns);
				}
				else
				{
					WordOpenXmlUtils.FailSerializable();
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(GetDeclaration());
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				if (memberName == MemberName.Columns)
				{
					_columns = reader.ReadListOfPrimitives<int>();
				}
				else
				{
					WordOpenXmlUtils.FailSerializable();
				}
			}
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.WordOpenXmlTableGrid;
		}

		internal new static Declaration GetDeclaration()
		{
			return _declaration;
		}

		public void AddRow(int[] columns)
		{
			List<int> list = new List<int>();
			int i = 0;
			int j = 0;
			while (i < _columns.Count && j < columns.Length)
			{
				if (columns[j] == 0)
				{
					j++;
				}
				else if (_columns[i] < columns[j])
				{
					list.Add(_columns[i]);
					columns[j] -= _columns[i];
					i++;
				}
				else if (_columns[i] > columns[j])
				{
					list.Add(columns[j]);
					_columns[i] -= columns[j];
					j++;
				}
				else
				{
					list.Add(_columns[i]);
					i++;
					j++;
				}
			}
			for (; i < _columns.Count; i++)
			{
				list.Add(_columns[i]);
			}
			for (; j < columns.Length; j++)
			{
				if (columns[j] > 0)
				{
					list.Add(columns[j]);
				}
			}
			_columns = list;
		}

		public override void Write(TextWriter output)
		{
			output.Write("<w:tblGrid>");
			foreach (int column in _columns)
			{
				output.Write("<w:gridCol w:w=\"");
				output.Write(WordOpenXmlUtils.TwipsToString(column, 0, 31680));
				output.Write("\"/>");
			}
			output.Write("</w:tblGrid>");
		}
	}
}
