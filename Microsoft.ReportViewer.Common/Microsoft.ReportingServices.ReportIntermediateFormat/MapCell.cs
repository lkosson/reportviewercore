using Microsoft.ReportingServices.RdlExpressions;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class MapCell : Cell, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		protected override bool IsDataRegionBodyCell => true;

		public Microsoft.ReportingServices.ReportProcessing.ObjectType ObjectType => Microsoft.ReportingServices.ReportProcessing.ObjectType.MapCell;

		public override Microsoft.ReportingServices.ReportProcessing.ObjectType DataScopeObjectType => ObjectType;

		protected override Microsoft.ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode ExprHostDataRegionMode => Microsoft.ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode.MapDataRegion;

		internal MapCell()
		{
		}

		internal MapCell(int id, MapDataRegion mapDataRegion)
			: base(id, mapDataRegion)
		{
		}

		internal override void InternalInitialize(int parentRowID, int parentColumnID, int rowindex, int colIndex, InitializationContext context)
		{
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> memberInfoList = new List<MemberInfo>();
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapCell, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Cell, memberInfoList);
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapCell;
		}
	}
}
