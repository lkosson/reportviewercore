using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class MapRow : Row, IPersistable
	{
		[NonSerialized]
		private CellList m_cells;

		[Reference]
		private MapDataRegion m_mapDataRegion;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal override CellList Cells => m_cells;

		internal MapCell Cell
		{
			get
			{
				if (m_cells != null && m_cells.Count == 1)
				{
					return (MapCell)m_cells[0];
				}
				return null;
			}
			set
			{
				if (m_cells == null)
				{
					m_cells = new CellList();
				}
				else
				{
					m_cells.Clear();
				}
				m_cells.Add(value);
			}
		}

		internal MapRow()
		{
		}

		internal MapRow(int id, MapDataRegion mapDataRegion)
			: base(id)
		{
			m_mapDataRegion = mapDataRegion;
		}

		[SkipMemberStaticValidation(MemberName.MapCell)]
		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.MapCell, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapCell));
			list.Add(new MemberInfo(MemberName.MapDataRegion, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapDataRegion, Token.Reference));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapRow, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Row, list);
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapRow mapRow = (MapRow)base.PublishClone(context);
			if (m_cells != null)
			{
				mapRow.m_cells = new CellList();
				mapRow.Cell = (MapCell)Cell.PublishClone(context);
			}
			return mapRow;
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.MapCell:
					writer.Write(Cell);
					break;
				case MemberName.MapDataRegion:
					writer.WriteReference(m_mapDataRegion);
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
				case MemberName.MapCell:
					Cell = (MapCell)reader.ReadRIFObject();
					break;
				case MemberName.MapDataRegion:
					m_mapDataRegion = reader.ReadReference<MapDataRegion>(this);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			if (!memberReferencesCollection.TryGetValue(m_Declaration.ObjectType, out List<MemberReference> value))
			{
				return;
			}
			foreach (MemberReference item in value)
			{
				MemberName memberName = item.MemberName;
				if (memberName == MemberName.MapDataRegion)
				{
					Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
					m_mapDataRegion = (MapDataRegion)referenceableItems[item.RefID];
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapRow;
		}
	}
}
