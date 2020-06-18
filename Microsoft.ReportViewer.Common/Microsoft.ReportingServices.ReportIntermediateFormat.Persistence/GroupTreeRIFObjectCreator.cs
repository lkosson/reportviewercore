using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Microsoft.ReportingServices.ReportIntermediateFormat.Persistence
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	internal struct GroupTreeRIFObjectCreator : IRIFObjectCreator, IScalabilityObjectCreator
	{
		public IPersistable CreateRIFObject(ObjectType objectType, ref IntermediateFormatReader context)
		{
			IPersistable persistObj = null;
			if (objectType == ObjectType.Null)
			{
				return null;
			}
			Global.Tracer.Assert(TryCreateObject(objectType, out persistObj));
			persistObj.Deserialize(context);
			return persistObj;
		}

		public bool TryCreateObject(ObjectType objectType, out IPersistable persistObj)
		{
			switch (objectType)
			{
			case ObjectType.DataCellInstance:
				persistObj = new DataCellInstance();
				break;
			case ObjectType.DataAggregateObjResult:
				persistObj = new DataAggregateObjResult();
				break;
			case ObjectType.DataRegionMemberInstance:
				persistObj = new DataRegionMemberInstance();
				break;
			case ObjectType.DataRegionInstance:
				persistObj = new DataRegionInstance();
				break;
			case ObjectType.DataSetInstance:
				persistObj = new DataSetInstance();
				break;
			case ObjectType.ReportInstance:
				persistObj = new ReportInstance();
				break;
			case ObjectType.OnDemandMetadata:
				persistObj = new OnDemandMetadata();
				break;
			case ObjectType.GroupTreePartition:
				persistObj = new GroupTreePartition();
				break;
			case ObjectType.IntermediateFormatVersion:
				persistObj = new IntermediateFormatVersion();
				break;
			case ObjectType.ReportSnapshot:
				persistObj = new ReportSnapshot();
				break;
			case ObjectType.SubReportInstance:
				persistObj = new SubReportInstance();
				break;
			case ObjectType.Parameters:
				persistObj = new ParametersImplWrapper();
				break;
			case ObjectType.Parameter:
				persistObj = new ParameterImplWrapper();
				break;
			case ObjectType.SubReportInfo:
				persistObj = new SubReportInfo();
				break;
			case ObjectType.CommonSubReportInfo:
				persistObj = new CommonSubReportInfo();
				break;
			case ObjectType.ParameterInfo:
				persistObj = new ParameterInfo();
				break;
			case ObjectType.ParameterInfoCollection:
				persistObj = new ParameterInfoCollection();
				break;
			case ObjectType.ParametersLayout:
				persistObj = new ParametersGridLayout();
				break;
			case ObjectType.ParameterGridLayoutCellDefinition:
				persistObj = new ParameterGridLayoutCellDefinition();
				break;
			case ObjectType.ValidValue:
				persistObj = new ValidValue();
				break;
			case ObjectType.FieldInfo:
				persistObj = new FieldInfo();
				break;
			case ObjectType.ImageInfo:
				persistObj = new ImageInfo();
				break;
			case ObjectType.TreePartitionManager:
				persistObj = new TreePartitionManager();
				break;
			case ObjectType.LookupObjResult:
				persistObj = new LookupObjResult();
				break;
			case ObjectType.ShapefileInfo:
				persistObj = new ShapefileInfo();
				break;
			case ObjectType.UpdatedVariableValues:
				persistObj = new UpdatedVariableValues();
				break;
			case ObjectType.DataCellInstanceList:
				persistObj = new DataCellInstanceList();
				break;
			default:
				persistObj = null;
				return false;
			}
			return true;
		}

		public List<Declaration> GetDeclarations()
		{
			return ChunkManager.OnDemandProcessingManager.GetChunkDeclarations();
		}
	}
}
