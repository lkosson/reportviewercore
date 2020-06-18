using Microsoft.ReportingServices.RdlExpressions;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class DataCell : Cell, IPersistable
	{
		private DataValueList m_dataValues;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		protected override bool IsDataRegionBodyCell => true;

		internal DataValueList DataValues
		{
			get
			{
				return m_dataValues;
			}
			set
			{
				m_dataValues = value;
			}
		}

		public override Microsoft.ReportingServices.ReportProcessing.ObjectType DataScopeObjectType => Microsoft.ReportingServices.ReportProcessing.ObjectType.DataCell;

		protected override Microsoft.ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode ExprHostDataRegionMode => Microsoft.ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode.CustomReportItem;

		internal DataCell()
		{
		}

		internal DataCell(int id, DataRegion dataRegion)
			: base(id, dataRegion)
		{
		}

		internal override void InternalInitialize(int parentRowID, int parentColumnID, int rowindex, int colIndex, InitializationContext context)
		{
			m_dataValues.Initialize(null, rowindex, colIndex, isCustomProperty: false, context);
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			DataCell dataCell = (DataCell)base.PublishClone(context);
			if (m_dataValues != null)
			{
				dataCell.m_dataValues = new DataValueList(m_dataValues.Count);
				{
					foreach (DataValue dataValue in m_dataValues)
					{
						dataCell.m_dataValues.Add(dataValue.PublishClone(context));
					}
					return dataCell;
				}
			}
			return dataCell;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.DataValues, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataValue));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataCell, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Cell, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.DataValues)
				{
					writer.Write(m_dataValues);
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				if (memberName == MemberName.DataValues)
				{
					m_dataValues = reader.ReadListOfRIFObjects<DataValueList>();
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public override void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			base.ResolveReferences(memberReferencesCollection, referenceableItems);
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataCell;
		}
	}
}
