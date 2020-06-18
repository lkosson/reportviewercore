using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal sealed class TablixRow : Row, IPersistable
	{
		private string m_height;

		private double m_heightValue;

		private TablixCellList m_cells;

		[NonSerialized]
		private bool m_forAutoSubtotal;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal string Height
		{
			get
			{
				return m_height;
			}
			set
			{
				m_height = value;
			}
		}

		internal double HeightValue
		{
			get
			{
				return m_heightValue;
			}
			set
			{
				m_heightValue = value;
			}
		}

		internal override CellList Cells => m_cells;

		internal TablixCellList TablixCells
		{
			get
			{
				return m_cells;
			}
			set
			{
				m_cells = value;
			}
		}

		internal bool ForAutoSubtotal
		{
			get
			{
				return m_forAutoSubtotal;
			}
			set
			{
				m_forAutoSubtotal = value;
			}
		}

		internal TablixRow()
		{
		}

		internal TablixRow(int id)
			: base(id)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			m_heightValue = context.ValidateSize(m_height, "Height");
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			TablixRow tablixRow = (TablixRow)base.PublishClone(context);
			if (m_height != null)
			{
				tablixRow.m_height = (string)m_height.Clone();
			}
			if (m_cells != null)
			{
				tablixRow.m_cells = new TablixCellList(m_cells.Count);
				{
					foreach (TablixCell cell in m_cells)
					{
						tablixRow.m_cells.Add((TablixCell)cell.PublishClone(context));
					}
					return tablixRow;
				}
			}
			return tablixRow;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Height, Token.String));
			list.Add(new MemberInfo(MemberName.HeightValue, Token.Double));
			list.Add(new MemberInfo(MemberName.TablixCells, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TablixCell));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TablixRow, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Row, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Height:
					writer.Write(m_height);
					break;
				case MemberName.HeightValue:
					writer.Write(m_heightValue);
					break;
				case MemberName.TablixCells:
					writer.Write(m_cells);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Height:
					m_height = reader.ReadString();
					break;
				case MemberName.HeightValue:
					m_heightValue = reader.ReadDouble();
					break;
				case MemberName.TablixCells:
					m_cells = reader.ReadListOfRIFObjects<TablixCellList>();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false);
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TablixRow;
		}
	}
}
