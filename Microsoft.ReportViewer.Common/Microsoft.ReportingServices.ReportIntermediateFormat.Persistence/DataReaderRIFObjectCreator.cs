using Microsoft.ReportingServices.ReportProcessing;
using System.Runtime.InteropServices;

namespace Microsoft.ReportingServices.ReportIntermediateFormat.Persistence
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	internal struct DataReaderRIFObjectCreator : IRIFObjectCreator
	{
		public IPersistable CreateRIFObject(ObjectType objectType, ref IntermediateFormatReader context)
		{
			IPersistable persistable = null;
			switch (objectType)
			{
			case ObjectType.Null:
				return null;
			case ObjectType.RecordSetInfo:
				persistable = new RecordSetInfo();
				break;
			case ObjectType.RecordRow:
				persistable = new RecordRow();
				break;
			case ObjectType.RecordField:
				persistable = new RecordField();
				break;
			case ObjectType.IntermediateFormatVersion:
				persistable = new IntermediateFormatVersion();
				break;
			case ObjectType.RecordSetPropertyNames:
				persistable = new RecordSetPropertyNames();
				break;
			default:
				Global.Tracer.Assert(condition: false);
				break;
			}
			persistable.Deserialize(context);
			return persistable;
		}
	}
}
