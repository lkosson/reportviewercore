using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal class BucketedDataAggregateObjs : BucketedAggregatesCollection<DataAggregateObj>
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		protected override AggregateBucket<DataAggregateObj> CreateBucket(int level)
		{
			return new DataAggregateObjBucket(level);
		}

		protected override Declaration GetSpecificDeclaration()
		{
			return m_Declaration;
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.BucketedDataAggregateObjs;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Buckets, ObjectType.RIFObjectList, ObjectType.DataAggregateObjBucket));
			return new Declaration(ObjectType.BucketedDataAggregateObjs, ObjectType.None, list);
		}
	}
}
