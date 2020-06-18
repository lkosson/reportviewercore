using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal sealed class TablixCell : TablixCellBase, IPersistable
	{
		private string m_dataElementName;

		private DataElementOutputTypes m_dataElementOutput = DataElementOutputTypes.Auto;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		[NonSerialized]
		private string m_cellIDForRendering;

		[NonSerialized]
		private ReportSize m_cellWidthForRendering;

		[NonSerialized]
		private ReportSize m_cellHeightForRendering;

		protected override bool IsDataRegionBodyCell => true;

		internal string DataElementName
		{
			get
			{
				return m_dataElementName;
			}
			set
			{
				m_dataElementName = value;
			}
		}

		internal DataElementOutputTypes DataElementOutput
		{
			get
			{
				return m_dataElementOutput;
			}
			set
			{
				m_dataElementOutput = value;
			}
		}

		internal string CellIDForRendering
		{
			get
			{
				return m_cellIDForRendering;
			}
			set
			{
				m_cellIDForRendering = value;
			}
		}

		internal ReportSize CellWidthForRendering
		{
			get
			{
				return m_cellWidthForRendering;
			}
			set
			{
				m_cellWidthForRendering = value;
			}
		}

		internal ReportSize CellHeightForRendering
		{
			get
			{
				return m_cellHeightForRendering;
			}
			set
			{
				m_cellHeightForRendering = value;
			}
		}

		internal TablixCell()
		{
		}

		internal TablixCell(int id, DataRegion dataRegion)
			: base(id, dataRegion)
		{
		}

		internal override void DataRendererInitialize(InitializationContext context)
		{
			if (m_dataElementOutput == DataElementOutputTypes.Auto)
			{
				m_dataElementOutput = DataElementOutputTypes.ContentsOnly;
			}
			Microsoft.ReportingServices.ReportPublishing.CLSNameValidator.ValidateDataElementName(ref m_dataElementName, "Cell", context.ObjectType, context.ObjectName, "DataElementName", context.ErrorContext);
		}

		internal void InitializeRVDirectionDependentItems(InitializationContext context)
		{
			bool flag = false;
			if (context.HasUserSorts)
			{
				context.Location |= (Microsoft.ReportingServices.ReportPublishing.LocationFlags.InDataRegionCellTopLevelItem | Microsoft.ReportingServices.ReportPublishing.LocationFlags.InTablixCell);
				if (context.IsDataRegionCellScope)
				{
					flag = true;
					context.RegisterIndividualCellScope(this);
				}
			}
			if (m_cellContents != null)
			{
				m_cellContents.InitializeRVDirectionDependentItems(context);
			}
			if (m_altCellContents != null)
			{
				m_altCellContents.InitializeRVDirectionDependentItems(context);
			}
			if (flag)
			{
				context.UnRegisterIndividualCellScope(this);
			}
		}

		internal void DetermineGroupingExprValueCount(InitializationContext context, int groupingExprCount)
		{
			if (m_cellContents != null)
			{
				m_cellContents.DetermineGroupingExprValueCount(context, groupingExprCount);
			}
			if (m_altCellContents != null)
			{
				m_altCellContents.DetermineGroupingExprValueCount(context, groupingExprCount);
			}
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			TablixCell tablixCell = (TablixCell)base.PublishClone(context);
			if (m_dataElementName != null)
			{
				tablixCell.m_dataElementName = (string)m_dataElementName.Clone();
			}
			return tablixCell;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.DataElementName, Token.String));
			list.Add(new MemberInfo(MemberName.DataElementOutput, Token.Enum));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TablixCell, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TablixCellBase, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.DataElementName:
					writer.Write(m_dataElementName);
					break;
				case MemberName.DataElementOutput:
					writer.WriteEnum((int)m_dataElementOutput);
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
				case MemberName.DataElementName:
					m_dataElementName = reader.ReadString();
					break;
				case MemberName.DataElementOutput:
					m_dataElementOutput = (DataElementOutputTypes)reader.ReadEnum();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			base.ResolveReferences(memberReferencesCollection, referenceableItems);
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TablixCell;
		}

		internal void SetExprHost(TablixCellExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			BaseSetExprHost(exprHost, reportObjectModel);
		}
	}
}
