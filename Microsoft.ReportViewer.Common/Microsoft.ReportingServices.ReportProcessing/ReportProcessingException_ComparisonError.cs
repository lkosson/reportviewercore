using Microsoft.ReportingServices.Common;
using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ReportProcessingException_ComparisonError : Exception, IDataComparisonError
	{
		private const string TypeXSerializationID = "typex";

		private const string TypeYSerializationID = "typey";

		private string m_typeX;

		private string m_typeY;

		public string TypeX => m_typeX;

		public string TypeY => m_typeY;

		internal ReportProcessingException_ComparisonError(string typeX, string typeY)
		{
			m_typeX = typeX;
			m_typeY = typeY;
		}

		private ReportProcessingException_ComparisonError(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			m_typeX = info.GetString("typex");
			m_typeY = info.GetString("typey");
		}

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("typex", m_typeX);
			info.AddValue("typex", m_typeY);
		}
	}
}
