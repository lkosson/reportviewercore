using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class GaugeRow : Row, IPersistable
	{
		private GaugeCellList m_cells;

		[Reference]
		private GaugePanel m_gaugePanel;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal override CellList Cells => m_cells;

		internal GaugeCell GaugeCell
		{
			get
			{
				if (m_cells != null && m_cells.Count > 0)
				{
					return m_cells[0];
				}
				return null;
			}
			set
			{
				if (m_cells == null)
				{
					m_cells = new GaugeCellList();
				}
				else
				{
					m_cells.Clear();
				}
				m_cells.Add(value);
			}
		}

		internal GaugeRow()
		{
		}

		internal GaugeRow(int id, GaugePanel gaugePanel)
			: base(id)
		{
			m_gaugePanel = gaugePanel;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new ReadOnlyMemberInfo(MemberName.GaugeCell, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeCell));
			list.Add(new MemberInfo(MemberName.GaugePanel, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePanel, Token.Reference));
			list.Add(new MemberInfo(MemberName.Cells, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeCell));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeRow, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Row, list);
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			GaugeRow gaugeRow = (GaugeRow)base.PublishClone(context);
			if (m_cells != null)
			{
				gaugeRow.m_cells = new GaugeCellList();
				gaugeRow.GaugeCell = (GaugeCell)GaugeCell.PublishClone(context);
			}
			return gaugeRow;
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.GaugePanel:
					writer.WriteReference(m_gaugePanel);
					break;
				case MemberName.Cells:
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
				case MemberName.GaugeCell:
					GaugeCell = (GaugeCell)reader.ReadRIFObject();
					break;
				case MemberName.GaugePanel:
					m_gaugePanel = reader.ReadReference<GaugePanel>(this);
					break;
				case MemberName.Cells:
					m_cells = reader.ReadListOfRIFObjects<GaugeCellList>();
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
				if (memberName == MemberName.GaugePanel)
				{
					Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
					m_gaugePanel = (GaugePanel)referenceableItems[item.RefID];
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeRow;
		}
	}
}
