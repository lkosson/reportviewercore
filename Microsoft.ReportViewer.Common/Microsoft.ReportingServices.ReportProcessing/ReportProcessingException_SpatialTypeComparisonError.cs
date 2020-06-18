using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ReportProcessingException_SpatialTypeComparisonError : Exception
	{
		private const string TypeSerializationID = "type";

		private string m_type;

		internal string Type => m_type;

		internal ReportProcessingException_SpatialTypeComparisonError(string type)
		{
			m_type = type;
		}

		internal ReportProcessingException_SpatialTypeComparisonError(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			m_type = info.GetString("type");
		}

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("type", m_type);
		}
	}
}
