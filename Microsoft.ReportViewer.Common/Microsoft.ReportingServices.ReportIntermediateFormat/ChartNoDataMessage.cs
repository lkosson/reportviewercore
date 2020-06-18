using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal class ChartNoDataMessage : ChartTitle
	{
		internal ChartNoDataMessage()
		{
		}

		internal ChartNoDataMessage(Chart chart)
			: base(chart)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.ChartNoDataMessageStart();
			InitializeInternal(context);
			context.ExprHostBuilder.ChartNoDataMessageEnd();
		}

		public override ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartNoDataMessage;
		}
	}
}
