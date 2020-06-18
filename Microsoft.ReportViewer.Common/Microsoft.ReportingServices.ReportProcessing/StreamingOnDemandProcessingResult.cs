using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal sealed class StreamingOnDemandProcessingResult : OnDemandProcessingResult
	{
		public override bool SnapshotChanged => false;

		internal StreamingOnDemandProcessingResult(Microsoft.ReportingServices.ReportIntermediateFormat.ReportSnapshot newOdpSnapshot, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.OnDemandProcessingManager chunkManager, bool newOdpSnapshotChanged, IChunkFactory createChunkFactory, ParameterInfoCollection parameters, int autoRefresh, int numberOfPages, ProcessingMessageList warnings, bool eventInfoChanged, EventInformation newEventInfo, PaginationMode updatedPaginationMode, ReportProcessingFlags updatedProcessingFlags, UserProfileState usedUserProfileState, ExecutionLogContext executionLogContext)
			: base(createChunkFactory, newOdpSnapshot.DefinitionTreeHasDocumentMap, newOdpSnapshot.HasShowHide || newOdpSnapshot.HasUserSortFilter, parameters, autoRefresh, numberOfPages, warnings, eventInfoChanged, newEventInfo, updatedPaginationMode, updatedProcessingFlags, usedUserProfileState, executionLogContext)
		{
		}

		public override void Save()
		{
		}
	}
}
