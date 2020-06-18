using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal class DataAggregateObjBucket : AggregateBucket<DataAggregateObj>
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal DataAggregateObjBucket()
		{
		}

		internal DataAggregateObjBucket(int level)
			: base(level)
		{
		}

		protected override Declaration GetSpecificDeclaration()
		{
			return m_Declaration;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Aggregates, ObjectType.RIFObjectList, ObjectType.DataAggregateObj));
			list.Add(new MemberInfo(MemberName.Level, Token.Int32));
			return new Declaration(ObjectType.DataAggregateObjBucket, ObjectType.None, list);
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.DataAggregateObjBucket;
		}
	}
}
